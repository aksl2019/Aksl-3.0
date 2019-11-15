using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using AutoMapper;

using Aksl.BulkInsert;
using Aksl.BulkInsert.Configuration;
using Aksl.Concurrency;
using Aksl.Pipeline;

using Contoso.Domain.Models;
using Contoso.Infrastructure.Data.Configuration;
using Contoso.Infrastructure.Data.Context;
using Contoso.Infrastructure.Data.Repository;
using Contoso.DataSource.Configuration;
using Contoso.DataSource;
using Contoso.Domain.Repository;
using Contoso.DataSource.Dtos;
using Contoso.DataSource.SqlServer;

namespace Contoso.ConsoleApp
{
    public partial class WebApiSender
    {
        #region Members
        protected bool _isInitialize;

        protected ILoggerFactory _loggerFactory;
        protected ILogger _logger;

        protected AsyncLock _mutex;
        protected AsyncLock _mutexRead;
        protected CancellationTokenSource _cancellationTokenSource;

        private static int _totalCount = 0;
        private static DurationManage _durationManage;

        public static WebApiSender Instance { get; private set; }
        #endregion

        #region Constructors
        static WebApiSender()
        {
            Instance = new WebApiSender();
        }

        public WebApiSender()
        {
        }

        public async Task InitializeTask()
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

                Services.Configure<PipeSettings>((pipeSettings) =>
                {
                    this.Configuration.Bind("Pipe", pipeSettings);
                });

                Services.Configure<BlockSettings>((blockSettings) =>
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
                                                                                              .UseSqlite(sqliteConnectionString , sqliteDbContextOptionsBuilder => sqliteDbContextOptionsBuilder.UseNetTopologySuite()).Options);
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
                Services.AddScoped<ISqlServerOrderDataSource, DataSource.SqlServer.SqlServerOrderDataSource>();

                //DataSourceFactory
                Services.AddScoped<IContosoDataSource, SqlServerContosoDataSource>();
                Services.AddScoped<IDataSourceFactory<IContosoDataSource>, ContosoDataSourceFactory>();

                // Services.AddSingleton<IPipeWebApiSender<PurchaseOrderDto, int>, PipeWebApiSender<PurchaseOrderDto, int>>();

                //4.
                this.ServiceProvider = this.Services.BuildServiceProvider();

                //5.
                _loggerFactory = ServiceProvider.GetRequiredService<ILoggerFactory>();
                _logger = _loggerFactory.CreateLogger<WebApiSender>();

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
                await OrderJsonProvider.InitializeTask();

                //  return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                // return Task.FromException(ex);
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
