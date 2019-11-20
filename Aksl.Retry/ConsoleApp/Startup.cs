using System;
using System.Net.Http;
using System.Threading;
//using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using Aksl.Retry;

namespace Aksl.Retry.ConsoleApp
{
    public class Startup
    {
        public static Startup Instance = new Startup();

        private ILogger _logger;
        private ILoggerFactory _loggerFactory;

        #region Construct
        public Startup()
        { }

        public Startup Initialize()
        {
            _loggerFactory = new LoggerFactory()
                .AddConsole()
                .AddDebug();

            _logger = _loggerFactory.CreateLogger<Startup>();

            return this;
        }

        private async Task<string> GetPerson()
        {
            using (HttpClient clinet = new HttpClient())
            {
                var result = await clinet.GetStringAsync("http://111/");
                return result.Substring(0, 50);
            }
        }

        #endregion

        public async Task StartLinearRetryStrategy()
        {
            CancellationTokenSource tokenSource = new CancellationTokenSource();

            HttpLinearRetryStrategy retryStrategy = new HttpLinearRetryStrategy(minBackoff: 0,
                                                                                maxBackoff: 20,
                                                                                maxRetryCount: 5);

            var logger = _loggerFactory.CreateLogger<RetryInvoker>();

            var retryInvoker = new RetryInvoker(retryStrategy, _logger);

            try
            {
                var result = await retryInvoker.InvokeWithRetryAsync<string>(
                    operation: async (token) =>
                    {
                        return await GetPerson();
                    }, 
                    operationTimeout: TimeSpan.FromMinutes(10),
                    cancellationToken: tokenSource.Token,
                    logger: logger);


                //retryWorker.InvokeWithRetryAsync(
                //   operation: (token) =>
                //   {
                //       Console.WriteLine("Linear Timing");
                //   },
                //   cancellationToken: tokenSource.Token,
                //   logger: _logger);
            }
            catch (Exception ex)
            {
                _logger?.LogError($"{ex.GetType()} with message \'{ex.Message} \'");
            }
        }

        public async Task StartExponentialRetryStrategy()
        {
            CancellationTokenSource tokenSource = new CancellationTokenSource();

            var logger = _loggerFactory.CreateLogger<RetryInvoker>();

            var retryInvoker = new RetryInvoker(new HttpExponentialRetryStrategy(minBackoff: 1,
                                                                                 maxBackoff: 20,
                                                                                 deltaBackoff: 0.5,
                                                                                 maxRetryCount: 10), _logger);

            try
            {
                var result = await retryInvoker.InvokeWithRetryAsync<string>(
                    operation: async (token) =>
                    {
                        return await GetPerson();
                    },
                    operationTimeout: TimeSpan.FromSeconds(30),
                    cancellationToken: tokenSource.Token,
                    logger: logger);
            }
            catch (Exception ex)
            {
                _logger?.LogError($"{ex.GetType()} with message \'{ex.Message} \'");
            }
        }
    }
}
