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
        #region RetryExponential Methods
        public async Task StartExponentialRetryStrategy()
        {
            CancellationTokenSource tokenSource = new CancellationTokenSource();

            var logger = _loggerFactory.CreateLogger("Retrier.Exponential");

            //var retrier = new Retrier(new HttpExponentialRetryStrategy(minBackoff: 1,
            //                                                           maxBackoff: 5,
            //                                                          deltaBackoff: 0.5,
            //                                                           maxRetryCount: 10), _logger);

            var retrier = ServiceProvider.GetRequiredService<IRetrier<HttpExponentialRetryStrategy>>();

            try
            {
                var result = await retrier.InvokeWithRetryAsync<string>(operation: async (token) =>
                                                                                  {
                                                                                    return await GetPerson();
                                                                                  },
                                                                      operationTimeout: TimeSpan.FromSeconds(20),
                                                                      cancellationToken: tokenSource.Token,
                                                                      logger: logger);
            }
            catch (Exception ex)
            {
                _logger?.LogError($"\'{ex.GetType()} \'  with message \'{ex.Message} \'");
            }
        }

        #endregion
    }
}
