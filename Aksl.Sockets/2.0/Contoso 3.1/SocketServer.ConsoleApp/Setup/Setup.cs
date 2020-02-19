using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using AutoMapper;

using Aksl.BulkInsert;
using Aksl.Concurrency;
using Aksl.Sockets.Server;
using Aksl.Sockets.Server.Configure;
using Aksl.Sockets.Server.Processor;

using Contoso.DataSource;
using Contoso.DataSource.Configuration;
using Contoso.DataSource.Dtos;
using Contoso.DataSource.SqlServer;
using Contoso.Domain.Models;
using Contoso.Domain.Repository;
using Contoso.Infrastructure.Data.Configuration;
using Contoso.Infrastructure.Data.Context;
using Contoso.Infrastructure.Data.Repository;

namespace SocketServer.ConsoleApp
{
    public partial class SocketListener
    {
        #region Members
        protected bool _isInitialize;

        protected ILoggerFactory _loggerFactory;
        protected ILogger _logger;

        protected AsyncLock _mutex;
        protected AsyncLock _mutexRead;
        protected AsyncManualResetEvent _initializeSignal;
        protected CancellationTokenSource _cancellationTokenSource;

        private static int _totalCount = 0;
        private (decimal TotalBytesInput, decimal TotalBytesOutput, decimal TotalTransportRate, TimeSpan TotalExecutionTime) _totalResult;
        private static DurationManage _durationManage;

        private static int StoreThreshold = 5000;
        private static int AllMaxThreshold = 10;
        List<SaleOrderDto> _saleOrderDtoBuffer = new List<SaleOrderDto>();
        private static int _currentStoreCount = 0;

        protected TimeSpan DueTime { get; set; } = TimeSpan.FromSeconds(5);
        protected TimeSpan Period { get; set; } = TimeSpan.FromSeconds(10);

        private readonly ConcurrentQueue<SaleOrderDto> _queues = new ConcurrentQueue<SaleOrderDto>();

        private DataflowProducerConsumer<SaleOrderDto> _dataflowProducerConsumer;
        private PipeProducerConsumer<SaleOrderDto> _pipeProducerConsumer;
        private PipeProducerConsumerEx<SaleOrderDto> _pipeProducerConsumerEx;
        private TaskChannelProducerConsumer<SaleOrderDto>  _taskChannelProducerConsumer;

        public static SocketListener Instance { get; private set; }
        #endregion

        #region Constructors
        static SocketListener()
        {
            Instance = new SocketListener();
        }

        public SocketListener()
        {
            _initializeSignal = new AsyncManualResetEvent();
        }
        #endregion

        #region Initialize Method
        public Task InitializeTask()
        {
            try
            {
                _isInitialize = true;

                _mutex = new AsyncLock();
                _mutexRead = new AsyncLock();

                //1.
                Services = new ServiceCollection();
                this.Services.AddOptions();

                //2.Configuration
                //string basePath = Directory.GetCurrentDirectory() + @"\..\..\..\..";
                string basePath = Directory.GetCurrentDirectory();
                this.ConfigurationBuilder = new ConfigurationBuilder()
                                               .SetBasePath(basePath)
                                               .AddJsonFile(path: "appsettings.json", optional: true, reloadOnChange: false);

                this.Configuration = ConfigurationBuilder.Build();

                //Socke
                this.Services.Configure<SocketTransportOptions>((socketTransportOptions) =>
                {
                    this.Configuration.Bind("SocketTransport", socketTransportOptions);
                });

                this.Services.UseSockets((socketServerOptions) =>
                {
                    socketServerOptions.ContentRootPath = basePath;

                    // socketServerOptions.ApplicationSchedulingMode = SchedulingMode.ThreadPool;

                    this.Configuration.Bind("Socket", socketServerOptions);

                    var socketSection = this.Configuration.GetSection("Socket");

                    socketServerOptions.ConfigureEndpointDefaults(listenOptions =>
                    {
                        listenOptions.NoDelay = true;
                    });

                    socketServerOptions.Configure(socketSection)
                                       .Load();

                    //var endPointConfigurationLoader = socketServerOptions.Configure();
                    ////.Endpoint("LocalhostHttp", configureOptions =>
                    //// {
                    ////     configureOptions.ListenOptions.NoDelay = false;
                    //// });
                    //endPointConfigurationLoader.Load();
                });

                Services.Configure<Aksl.Pipeline.PipeSettings>((pipeSettings) =>
                {
                    this.Configuration.Bind("Pipe", pipeSettings);
                });


                Services.Configure<Aksl.BulkInsert.Configuration.BlockSettings>((blockSettings) =>
                {
                    this.Configuration.Bind("Block", blockSettings);
                });

                Services.Configure<DataProviderSettings>((dataProviderSettings) =>
                {
                    this.Configuration.Bind("DataProvider", dataProviderSettings);
                });

                Services.Configure<DataSourceTypeSettings>((dataSourceTypeSettings) =>
                {
                    this.Configuration.Bind("DataSource", dataSourceTypeSettings);
                });

                //3.Logging
                Services.AddLogging(builder =>
                {
                    var loggingSection = this.Configuration.GetSection("Logging");
                    var includeScopes = loggingSection.GetValue<bool>("IncludeScopes");

                    builder.AddConfiguration(loggingSection);

                    //加入一个ConsoleLoggerProvider
                    builder.AddConsole(consoleLoggerOptions =>
                    {
                        consoleLoggerOptions.IncludeScopes = includeScopes;
                    });

                    //加入一个DebugLoggerProvider
                    builder.AddDebug();
                });

                //DbContext
                Services.AddScoped<ContosoContext, SqliteContosoContext>((sp) =>
                {
                    var logFactory = sp.GetRequiredService<ILoggerFactory>();
                    string sqliteConnectionString = this.Configuration.GetConnectionString("ContosoSqlite");
                    //var sqliteContosoContext = new SqliteContosoContext(new DbContextOptionsBuilder<ContosoContext>().UseLoggerFactory(logFactory)
                    //                                                                                                 .UseSqlite(sqliteConnectionString).Options);
                    var sqliteContosoContext = new SqliteContosoContext(new DbContextOptionsBuilder<ContosoContext>()
                                                                                           .UseSqlite(sqliteConnectionString, sqliteDbContextOptionsBuilder => sqliteDbContextOptionsBuilder.UseNetTopologySuite()).Options);
                    return sqliteContosoContext;
                });

                Services.AddScoped<ContosoContext, SqlServerContosoContext>((sp) =>
                {
                    var logFactory = sp.GetRequiredService<ILoggerFactory>();
                    string sqlServerConnectString = this.Configuration.GetConnectionString("ContosoSqlServer");
                    //var sqlServerContosoContext = new SqlServerContosoContext(new DbContextOptionsBuilder<ContosoContext>().UseLoggerFactory(logFactory)
                    //                                                                                                       .UseSqlServer(sqlServerConnectString).Options);
                    var sqlServerContosoContext = new SqlServerContosoContext(new DbContextOptionsBuilder<ContosoContext>()
                                                                                           .UseSqlServer(sqlServerConnectString, sqlServerDbContextOptionsBuilder => sqlServerDbContextOptionsBuilder.UseNetTopologySuite()).Options);
                    return sqlServerContosoContext;
                });

                Services.AddScoped<IDbContextFactory<ContosoContext>, DbContextFactory>();

                Services.AddScoped(typeof(IDataflowBulkInserter<,>), typeof(DataflowBulkInserter<,>));
                Services.AddScoped(typeof(IDataflowPipeBulkInserter<,>), typeof(DataflowPipeBulkInserter<,>));
                Services.AddScoped(typeof(IPipeBulkInserter<,>), typeof(PipeBulkInserter<,>));
                Services.AddScoped(typeof(IDataflowNoResultHandler<>), typeof(DataflowNoResultHandler<>));

                //Repository
                Services.AddScoped<ISqlOrderRepository, SqlOrderRepository>();

                //Mapper
                Services.AddAutoMapper(typeof(Contoso.DataSource.AutoMapper.AutoMapperProfileConfiguration));

                //DataSource
                Services.AddScoped<ISqlServerOrderDataSource, Contoso.DataSource.SqlServer.SqlServerOrderDataSource>();

                //DataSourceFactory
                Services.AddScoped<IContosoDataSource, SqlServerContosoDataSource>();
                Services.AddScoped<IDataSourceFactory<IContosoDataSource>, ContosoDataSourceFactory>();

                //4.
                this.ServiceProvider = this.Services.BuildServiceProvider();

                //5.
                _loggerFactory = ServiceProvider.GetRequiredService<ILoggerFactory>();
                _logger = _loggerFactory.CreateLogger<SocketListener>();

                //Socke
                _dataflowProducerConsumer = new  DataflowProducerConsumer<SaleOrderDto>(async (saleOrderDtos) =>
                {
                    await DataflowBulkInsertBlockAsync(saleOrderDtos);
                }, _loggerFactory);

                _taskChannelProducerConsumer = new TaskChannelProducerConsumer<SaleOrderDto>(async (saleOrderDtos) =>
                {
                    await DataflowBulkInsertBlockAsync(saleOrderDtos);
                }, _loggerFactory);

                _pipeProducerConsumer = new  PipeProducerConsumer<SaleOrderDto>(async (saleOrderDtos) =>
                {
                    await DataflowBulkInsertBlockAsync(saleOrderDtos);
                }, _loggerFactory);

                _pipeProducerConsumerEx = new PipeProducerConsumerEx<SaleOrderDto>(async (saleOrderDtos) =>
                {
                    await DataflowBulkInsertBlockAsync(saleOrderDtos);
                }, _loggerFactory);

                // var serverOptions = ServiceProvider.GetRequiredService<IOptions<SocketServerOptions>>();
                var socketServer = ServiceProvider.GetRequiredService<ISocketServer>();
                socketServer.AddJsonStringHandler(async (buffer, token) =>
                {
                    var newOrderBytes = await ProcessJsonStringFromClientAsync(buffer, token);
                    return newOrderBytes;
                });

                var repositoryFactory = this.ServiceProvider.GetRequiredService<IDbContextFactory<ContosoContext>>();
                var dbContext = repositoryFactory.CreateDbContext();

                var dataflowBulkInserter = this.ServiceProvider.GetRequiredService<IDataflowBulkInserter<SaleOrderDto, SaleOrderDto>>();
                var dataflowPipeBulkInserter = this.ServiceProvider.GetRequiredService<IDataflowPipeBulkInserter<SaleOrderDto, SaleOrderDto>>();
                var pipeBulkInserter = this.ServiceProvider.GetRequiredService<IPipeBulkInserter<SaleOrderDto, SaleOrderDto>>();
                var dataflowBulkDeleter = this.ServiceProvider.GetRequiredService<IDataflowNoResultHandler<SaleOrder>>();

                var mapper = this.ServiceProvider.GetRequiredService<IMapper>();

                var contosoDataSourceFactory = this.ServiceProvider.GetRequiredService<IDataSourceFactory<IContosoDataSource>>();
                var orderDataSource = contosoDataSourceFactory.Current.OrderDataSource;

                _cancellationTokenSource = new CancellationTokenSource();
                _durationManage = new DurationManage();

                _initializeSignal.Set();

                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                return Task.FromException(ex);
            }
        }
        #endregion

        #region Start Method
        public async Task StartAsync()
        {
            //List<Task> tasks = new List<Task>() { InitializeTask(), StartSocketListenerAsync(), RunAsync() };

            // List<Task> tasks = new List<Task>() { InitializeTask(), StartSocketListenerAsync(), _dataflowConsumer.StartAsync() };

             List<Task> tasks = new List<Task>() { InitializeTask(), StartSocketListenerAsync(), StartConsumerAsync() };

           // List<Task> tasks = new List<Task>() { InitializeTask(), StartSocketListenerAsync() };

            await Task.WhenAll(tasks);
        }

        public async Task StartConsumerAsync()
        {
            await _initializeSignal.WaitAsync();

           await _dataflowProducerConsumer.StartAsync();

           //await _pipeProducerConsumer.StartAsync();

           //  await _pipeProducerConsumerEx.StartAsync();

           // await _taskChannelProducerConsumer.StartAsync();
        }
        #endregion

        #region Run  Method
        public async Task RunAsync(CancellationToken cancellationToken = default)
        {
            await _initializeSignal.WaitAsync();

            await Task.Delay(DueTime, cancellationToken);

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                try
                {
                    using (await _mutex.LockAsync())
                    {
                        _logger.LogInformation($"----processing sale orders:'{_queues.Count}',now:{DateTime.Now.TimeOfDay}----");

                        if (_currentStoreCount >= AllMaxThreshold)
                        {
                            await InsertSaleOrders();
                            _currentStoreCount = 0;
                        }
                        else
                        {
                            if (_queues.Count >= StoreThreshold)
                            {
                                await InsertSaleOrders();
                                _currentStoreCount = 0;
                            }
                            else
                            {
                                _currentStoreCount++;
                            }
                        }
                    }

                    async Task InsertSaleOrders()
                    {
                        if (_queues.Count > 0)
                        {
                            var saleOrderDtos = _queues.ToArray();
                            _queues.Clear();
                            await SocketListener.Instance.DataflowBulkInsertBlockTasksAsync(saleOrderDtos);
                        }
                    }
                }
                catch (TaskCanceledException)
                {//swallow and move on..
                }
                catch (OperationCanceledException)
                {//swallow and move on..
                }
                catch (ObjectDisposedException)
                {//swallow and move on..
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception caught while processing messages:'{ex.Message}'");
                }
                finally
                {

                }
                await Task.Delay(Period, cancellationToken);
            }
        }
        #endregion

        #region Properties
        public ServiceCollection Services { get; private set; }

        public IServiceProvider ServiceProvider { get; private set; }

        public IConfigurationBuilder ConfigurationBuilder { get; private set; }

        public IConfigurationRoot Configuration { get; private set; }

        public ILoggerFactory LoggerFactory => _loggerFactory;

        #endregion
    }
}
