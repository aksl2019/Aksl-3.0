using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

namespace Aksl.Retry
{
    public interface IRetryInvoker<out IRetryStrategy>
    {
        void InvokeWithRetryAsync(Action<CancellationToken> operation, TimeSpan? operationTimeout = null,
                                  CancellationToken cancellationToken = default, ILogger logger = null);

        TResult InvokeWithRetryAsync<TResult>(Func<CancellationToken, TResult> operation,
                                              TimeSpan? operationTimeout = null,
                                              CancellationToken cancellationToken = default, ILogger logger = null);

        Task InvokeWithRetryAsync(Func<CancellationToken, Task> operation, TimeSpan? operationTimeout = null,
                                  CancellationToken cancellationToken = default, ILogger logger = null);

        Task<TResult> InvokeWithRetryAsync<TResult>(Func<CancellationToken, Task<TResult>> operation,
                                                    TimeSpan? operationTimeout = null,
                                                    CancellationToken cancellationToken = default, ILogger logger = null);
    }
}