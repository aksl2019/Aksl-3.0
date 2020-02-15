using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Aksl.Retry.ConsoleApp
{
    public partial class RetryListener
    {
        #region Members
        protected bool _isInitialize;

        protected ILoggerFactory _loggerFactory;
        protected ILogger _logger;

        protected CancellationTokenSource _cancellationTokenSource;

        protected TimeSpan DueTime { get; set; } = TimeSpan.FromSeconds(5);
        protected TimeSpan Period { get; set; } = TimeSpan.FromSeconds(10);

        public static RetryListener Instance { get; private set; }
        #endregion

        #region Constructors
        static RetryListener()
        {
            Instance = new RetryListener();
        }

        public RetryListener()
        { }
        #endregion

        #region Initialize Method
        public Task InitializeTask()
        {
            try
            {
                _isInitialize = true;

               // _mutex = new AsyncLock();

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

                this.Services.Configure<ExponentialRetrySettings>((retrySettings) =>
                 {
                     this.Configuration.Bind("Retry:Exponential", retrySettings);
                 });

                this.Services.Configure<LinearRetrySettings>((retrySettings) =>
                {
                    this.Configuration.Bind("Retry:Linear", retrySettings);
                });

                Services.AddSingleton<HttpExponentialRetryStrategy>();
                Services.AddSingleton<HttpLinearRetryStrategy>();
                Services.AddSingleton<SocketExponentialRetryStrategy>();

                Services.AddTransient<IRetrier<HttpExponentialRetryStrategy>, Retrier<HttpExponentialRetryStrategy>>();
                Services.AddTransient<IRetrier<HttpLinearRetryStrategy>, Retrier<HttpLinearRetryStrategy>>();
                Services.AddTransient<IRetrier<SocketExponentialRetryStrategy>, Retrier<SocketExponentialRetryStrategy>>();

                //Services.AddTransient<IRetrier<HttpExponentialRetryStrategy>, Retrier>();
                // Services.AddSingleton<IRetrier<HttpLinearRetryStrategy>, Retrier>();

                Services.AddTransient<IPersistentSocketConnection<SocketExponentialRetryStrategy>, PersistentSocketConnection<SocketExponentialRetryStrategy>>();

                //Services.AddTransient<IRetrier<HttpExponentialRetryStrategy>, Retrier<HttpExponentialRetryStrategy>>((sp) =>
                //{
                //    var loggerFactory = sp.GetRequiredService<ILoggerFactory>();

                //    //var retryStrategy = new HttpExponentialRetryStrategy(minBackoff: 1,
                //    //                                                     maxBackoff: 5,
                //    //                                                     deltaBackoff: 0.5,
                //    //                                                     maxRetryCount: 10);
                //    var httpExponentialRetryStrategy = sp.GetRequiredService<HttpExponentialRetryStrategy>();

                //    var retrier = new Retrier<HttpExponentialRetryStrategy> (httpExponentialRetryStrategy, loggerFactory);
                //    return retrier;

                //});

                //Services.AddTransient<IRetrier<HttpLinearRetryStrategy>, Retrier>((sp) =>
                //{
                //    var loggerFactory = sp.GetRequiredService<ILoggerFactory>();

                //    //HttpLinearRetryStrategy retryStrategy = new HttpLinearRetryStrategy(minBackoff: 0,
                //    //                                                                    maxBackoff: 20,
                //    //                                                                    maxRetryCount: 5);
                //    var httpLinearRetryStrategy = ServiceProvider.GetRequiredService<HttpLinearRetryStrategy > ();
                //    var retrier = new Retrier(httpLinearRetryStrategy , loggerFactory);

                //    return retrier;

                //});

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

                //4.
                this.ServiceProvider = this.Services.BuildServiceProvider();

                //5.
                _loggerFactory = ServiceProvider.GetRequiredService<ILoggerFactory>();
                _logger = _loggerFactory.CreateLogger<RetryListener>();

                //var httpExponentialRetryStrategy = ServiceProvider.GetRequiredService<HttpExponentialRetryStrategy>();
                //var httpLinearRetryStrategy = ServiceProvider.GetRequiredService<HttpLinearRetryStrategy > ();

                var httpExponentialRetrier = ServiceProvider.GetRequiredService<IRetrier<HttpExponentialRetryStrategy>>();
                var httpLinearRetrier = ServiceProvider.GetRequiredService<IRetrier<HttpLinearRetryStrategy>>();

                var persistentSocketConnection = ServiceProvider.GetRequiredService<IPersistentSocketConnection<SocketExponentialRetryStrategy>>();

                _cancellationTokenSource = new CancellationTokenSource(); 


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
            List<Task> tasks = new List<Task>{ InitializeTask() /*,StartExponentialRetryStrategy()*/, StartLinearRetryStrategy() };

            await Task.WhenAll(tasks);
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
