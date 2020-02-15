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
        #region RetryLinear Methods
        private async Task<string> GetPerson()
        {
            using (HttpClient clinet = new HttpClient())
            {
                var result = await clinet.GetStringAsync("http://111/");
                return result.Substring(0, 50);
            }
        }

        public async Task StartLinearRetryStrategy()
        {
            CancellationTokenSource tokenSource = new CancellationTokenSource();

            //HttpLinearRetryStrategy retryStrategy = new HttpLinearRetryStrategy(minBackoff: 0,
            //                                                                    maxBackoff: 20,
            //                                                                    maxRetryCount: 5);

            var logger = _loggerFactory.CreateLogger("Retrier.Linear");

            //var retrier = new Retrier(retryStrategy, _logger);


            var retrier = ServiceProvider.GetRequiredService<IRetrier<HttpLinearRetryStrategy>>();

            try
            {
                var result = await retrier.InvokeWithRetryAsync<string>(
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
        #endregion
    }
}
