using System;
using System.Buffers;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Aksl.Concurrency;
using Aksl.Sockets.Client.Configuration;

namespace Socket.Sender
{
    public partial class SocketSender
    {
        #region Members
        protected ILoggerFactory _loggerFactory;
        protected ILogger _logger;

        protected AsyncLock _mutex;
        protected CancellationTokenSource _cancellationTokenSource;

        protected bool _isInitialize;

        private static TimeSpan _totalDuration = TimeSpan.Zero;
        private static int _totalCount = 0;
        private static DurationManage _durationManage;

        private (decimal totalBytesInput, decimal totalBytesOutput, decimal totalTransportRate, TimeSpan maxExecutionTime,TimeSpan maxTransportTime) _totalResult;
        protected AsyncLock _resultMutex;

        public static SocketSender Instance { get; private set; }
        #endregion

        #region Constructors
        static SocketSender()
        {
            Instance = new SocketSender();
        }

        public SocketSender() { }

        public async Task InitializeTask()
        {
            try
            {
                _isInitialize = true;

                _mutex = new AsyncLock();
                _resultMutex = new AsyncLock();

                Services = new ServiceCollection();
                this.Services.AddOptions();

                //1.Configuration
                // string basePath = Directory.GetCurrentDirectory() + @"\..\..\..\..";
                string basePath = Directory.GetCurrentDirectory();
                var configurationBuilder = new ConfigurationBuilder()
                                               .SetBasePath(basePath)
                                               .AddJsonFile(path: "appsettings.json", optional: true, reloadOnChange: false);

                this.Configuration = configurationBuilder.Build();

                // SocketClientSettings socketClientSettings = new SocketClientSettings();
                //this.Configuration.Bind("Socket", socketClientSettings);
                //this.Services.Configure<SocketClientSettings>((socketClientSettings) =>
                // {
                //     this.Configuration.Bind("Socket", socketClientSettings);
                // });

                #region SocketClientOptions Method
                SocketClientOptions socketClientOptions = new SocketClientOptions();
                ConfigSocketClientOptions();

                void ConfigSocketClientOptions()
                {
                    socketClientOptions.ContentRootPath = basePath;

                    this.Configuration.Bind("Socket", socketClientOptions);

                    var socketSection = this.Configuration.GetSection("Socket");

                    socketClientOptions.ConfigureEndpointDefaults(endpointOptions =>
                    {
                        endpointOptions.NoDelay = true;
                    });

                    socketClientOptions.Configure()
                                              .Load();
                }
                #endregion

                #region SocketClientOptions Method
                this.Services.Configure<SocketClientSettings>((socketClientSettings) =>
                {
                    var clientSettings = socketClientOptions.SocketClientSettingsList.FirstOrDefault(p => p.EndPointInformation.DisplayName == "http://127.0.0.1:5000");

                    socketClientSettings.EndPointInformation = clientSettings.EndPointInformation;
                    socketClientSettings.Options = clientSettings.Options;
                    socketClientSettings.BlockSettings = clientSettings.BlockSettings;
                    socketClientSettings.PipeSettings = clientSettings.PipeSettings;

                    // this.Configuration.Bind("Socket:PipeSettings", socketClientSettings.PipeSettings);
                });
                #endregion

                //2.Logger
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

                //3.Configure Services
                #region Method
                //foreach (var socketClientSettings in socketClientOptions.SocketClientSettingsList)
                //{
                //    Services.AddTransient<Aksl.Sockets.Client.IDataflowPipeSocketSender>((sp) =>
                //    {
                //        var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
                //        var sender = new Aksl.Sockets.Client.DataflowPipeSocketSender(socketClientSettings, loggerFactory);
                //        return sender;
                //    });

                //    Services.AddTransient<Aksl.Sockets.Client.IDataflowDuplexPipeSocketSender>((sp) =>
                //    {
                //        var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
                //        var sender = new Aksl.Sockets.Client.DataflowDuplexPipeSocketSender(socketClientSettings, loggerFactory);
                //        return sender;
                //    });

                //    Services.AddTransient<Aksl.Sockets.Client.ISocketPipeSender>((sp) =>
                //    {
                //        var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
                //        var sender = new Aksl.Sockets.Client.SocketPipeSender(socketClientSettings, loggerFactory);
                //        return sender;
                //    });

                //    Services.AddTransient<Aksl.Sockets.Client.ISocketDuplexPipeSender>((sp) =>
                //    {
                //        var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
                //        var sender = new Aksl.Sockets.Client.SocketDuplexPipeSender(socketClientSettings, loggerFactory);
                //        return sender;
                //    });

                //    Services.AddTransient<Aksl.Sockets.Client.ISocketDuplexPipeSenderString>((sp) =>
                //    {
                //        var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
                //        var sender = new Aksl.Sockets.Client.SocketDuplexPipeSenderString(socketClientSettings, loggerFactory);
                //        return sender;
                //    });
                //}

                //Services.AddTransient<Aksl.Sockets.Client.ISocketPipeSender>((sp) =>
                //{
                //    var loggerFactory = sp.GetRequiredService<ILoggerFactory>();

                //    var socketClientSettings = socketClientOptions.SocketClientSettingsSet.FirstOrDefault(p => p.EndPointInformation.DisplayName == "http://127.0.0.1:5000");

                //    var sender = new Aksl.Sockets.Client.SocketPipeSender(socketClientSettings, loggerFactory);
                //    return sender;
                //});
                #endregion

                #region Method
                foreach (var endPointInformation in socketClientOptions.EndPointInformations)
                {
                    Services.AddTransient<Aksl.Sockets.Client.IDataflowPipeSocketSender>((sp) =>
                    {
                        var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
                        var sender = new Aksl.Sockets.Client.DataflowPipeSocketSender(endPointInformation, loggerFactory);
                        return sender;
                    });

                    Services.AddTransient<Aksl.Sockets.Client.IDataflowDuplexPipeSocketSender>((sp) =>
                    {
                        var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
                        var sender = new Aksl.Sockets.Client.DataflowDuplexPipeSocketSender(endPointInformation, loggerFactory);
                        return sender;
                    });

                    Services.AddTransient<Aksl.Sockets.Client.ISocketPipeSender>((sp) =>
                    {
                        var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
                        var sender = new Aksl.Sockets.Client.SocketPipeSender(endPointInformation, loggerFactory);
                        return sender;
                    });

                    Services.AddTransient<Aksl.Sockets.Client.ISocketDuplexPipeSender>((sp) =>
                    {
                        var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
                        var sender = new Aksl.Sockets.Client.SocketDuplexPipeSender(endPointInformation, loggerFactory);
                        return sender;
                    });

                    Services.AddTransient<Aksl.Sockets.Client.ISocketDuplexPipeSenderString>((sp) =>
                    {
                        var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
                        var sender = new Aksl.Sockets.Client.SocketDuplexPipeSenderString(endPointInformation, loggerFactory);
                        return sender;
                    });
                }

                //Services.AddTransient<Aksl.Sockets.Client.ISocketPipeSender>((sp) =>
                //{
                //    var loggerFactory = sp.GetRequiredService<ILoggerFactory>();

                //    var socketClientSettings = socketClientOptions.SocketClientSettingsSet.FirstOrDefault(p => p.EndPointInformation.DisplayName == "http://127.0.0.1:5000");

                //    var sender = new Aksl.Sockets.Client.SocketPipeSender(socketClientSettings, loggerFactory);
                //    return sender;
                //});
                #endregion

                #region Method
                //this.Services.AddTransient<Aksl.Sockets.Client.IDataflowPipeSocketSender, Aksl.Sockets.Client.DataflowPipeSocketSender>();
                //this.Services.AddTransient<Aksl.Sockets.Client.IDataflowDuplexPipeSocketSender, Aksl.Sockets.Client.DataflowDuplexPipeSocketSender>();

                //this.Services.AddTransient<Aksl.Sockets.Client.ISocketPipeSender, Aksl.Sockets.Client.SocketPipeSender>();
                //this.Services.AddTransient<Aksl.Sockets.Client.ISocketDuplexPipeSender, Aksl.Sockets.Client.SocketDuplexPipeSender>();
                //this.Services.AddTransient<Aksl.Sockets.Client.ISocketDuplexPipeSenderString, Aksl.Sockets.Client.SocketDuplexPipeSenderString>();
                #endregion

                //4.
                this.ServiceProvider = this.Services.BuildServiceProvider();

                _loggerFactory = ServiceProvider.GetRequiredService<ILoggerFactory>();
                _logger = _loggerFactory.CreateLogger<SocketSender>();

                //var clientOptions = ServiceProvider.GetRequiredService<IOptions<SocketClientSettings>>()?.Value;

                //var dataflowPipeSocketSender = ServiceProvider.GetRequiredService<Aksl.Sockets.Client.IDataflowPipeSocketSender>();
                //var dataflowDuplexPipeSocketSender = ServiceProvider.GetRequiredService<Aksl.Sockets.Client.IDataflowDuplexPipeSocketSender>();

                //var socketPipeSender = ServiceProvider.GetRequiredService<Aksl.Sockets.Client.ISocketPipeSender>();
                //var socketDuplexPipeSender = ServiceProvider.GetRequiredService<Aksl.Sockets.Client.ISocketDuplexPipeSender>();
                //dataflowPipeSocketSender.Handler = (async (buffers, token) =>
                // {
                //     await ProcessJsonSegmentByteDataflowAsync(buffers, token);
                // });

                //   var socketPipeSender = ServiceProvider.GetRequiredService<Aksl.Sockets.Client.ISocketPipeSender>();
                //socketPipeSender.AddProcessor(new ProcessorFunction(async (buffers, token) =>
                //{
                //    await ProcesJsonBytesDataflowAsync(buffers, token);
                //}));

                //5.
                _cancellationTokenSource = new CancellationTokenSource();

                _durationManage = new DurationManage();

                await OrderJsonProvider.InitializeTask();
                //await OrderMessagePackProvider.InitializeTask();
                //await OrderProtoBufProvider.InitializeTask();

                //return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                //return Task.FromException(ex);
            }
        }
        #endregion

        #region Properties
        public ServiceCollection Services { get; private set; }

        public IServiceProvider ServiceProvider { get; private set; }

        public IConfigurationRoot Configuration { get; private set; }

        public ILoggerFactory LoggerFactory => _loggerFactory;
        #endregion

        #region Get ASCII String Method
        public static string GetASCIIString(ReadOnlySequence<byte> buffers)
        {
            if (buffers.IsSingleSegment)
            {
                return Encoding.ASCII.GetString(buffers.First.Span);
            }

            return string.Create((int)buffers.Length, buffers, (span, sequence) =>
            {
                foreach (var segment in sequence)
                {
                    Encoding.ASCII.GetChars(segment.Span, span);
                    span = span.Slice(segment.Length);
                }
            });
        }
        #endregion
    }
}
