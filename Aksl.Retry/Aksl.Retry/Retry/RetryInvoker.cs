﻿using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

//EventFlow-develop

namespace Aksl.Retry
{
    public class RetryInvoker : IRetryInvoker<IRetryStrategy>
    {
        #region Members
        private readonly IRetryStrategy _retryStrategy;

        protected ILogger _logger;

        protected readonly TimeSpan DefaultOperationTimeout = TimeSpan.FromMinutes(1);
        #endregion

        public static RetryInvoker Create(IRetryStrategy retryStrategy, ILogger logger) =>
                                                                       new RetryInvoker(retryStrategy, logger);

        #region Constructors
        /// <summary>
        /// Constructor
        /// </summary>
        public RetryInvoker(IRetryStrategy retryStrategy, ILogger logger)
        {
            _retryStrategy = retryStrategy ??
                throw new InvalidOperationException("You need to configure the retry strategy using the Use(...) method");
            _logger = logger;
        }
        #endregion

        #region IRetryWorker<IRetryStrategy> 
        public void InvokeWithRetryAsync(Action<CancellationToken> operation, TimeSpan? operationTimeout = null, CancellationToken cancellationToken = default, ILogger logger = null)
        {
            if (operation == null)
                throw new ArgumentNullException(nameof(operation));

            Func<CancellationToken, Task<object>> wrapped = (token) =>
            {
                operation(token);
                return Task.FromResult<object>(null);
            };

            InvokeWithRetryAsync(wrapped, operationTimeout, cancellationToken, logger).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public TResult InvokeWithRetryAsync<TResult>(Func<CancellationToken, TResult> operation, TimeSpan? operationTimeout = null, CancellationToken cancellationToken = default, ILogger logger = null)
        {
            if (operation == null)
                throw new ArgumentNullException(nameof(operation));

            Func<CancellationToken, Task<TResult>> wrapped = (token) =>
            {
                var result = operation(token);
                return Task.FromResult(result);
            };

            var outerResult = InvokeWithRetryAsync(wrapped, operationTimeout, cancellationToken, logger).ConfigureAwait(false).GetAwaiter().GetResult();
            return outerResult;
        }

        public async Task InvokeWithRetryAsync(Func<CancellationToken, Task> operation, TimeSpan? operationTimeout = null, CancellationToken cancellationToken = default, ILogger logger = null)
        {
            if (operation == null)
                throw new ArgumentNullException(nameof(operation));

            Func<CancellationToken, Task<object>> wrapped = async (token) =>
            {
                await operation(token);
                return Task.FromResult<object>(null);
            };

            await InvokeWithRetryAsync(wrapped, operationTimeout, cancellationToken, logger);
        }

        public async Task<TResult> InvokeWithRetryAsync<TResult>(Func<CancellationToken, Task<TResult>> operation, TimeSpan? operationTimeout = null, CancellationToken cancellationToken = default, ILogger logger = null)
        {
            if (operation == null)
                throw new ArgumentNullException(nameof(operation));

            var currentRetryCount = 0;
            TimeoutHelper timeoutHelper = new TimeoutHelper(operationTimeout ?? DefaultOperationTimeout, true);
            timeoutHelper.SetDeadline();
            var stopwatch = Stopwatch.StartNew();
            //TimeSpan timeout = operationTimeout ?? DefaultOperationTimeout;
            //var deadline = DateTime.UtcNow + timeout;

            while (true)
            {
                Exception currentException;
                Retry retry;

                try
                {
                    var result = await operation(cancellationToken).ConfigureAwait(continueOnCapturedContext: false);

                    logger?.LogInformation(
                      $"Finished execute after {currentRetryCount.ToOrdinal()} retries and { stopwatch.Elapsed.TotalSeconds} seconds");

                    return result;
                }
                catch (Exception ex)
                {
                    //logger?.LogError($"{ex.GetType()} with message \'{ex.Message} \' is transient");

                    currentException = ex;
                    var currentTime = stopwatch.Elapsed;
                    var remainingTime = timeoutHelper.RemainingTime();
                    // var remainingTime = deadline - DateTime.UtcNow;
                    currentRetryCount++;
                    retry = _retryStrategy.ShouldThisBeRetried(remainingTime, currentException, currentRetryCount);

                    remainingTime = timeoutHelper.RemainingTime();
                    logger?.LogError($"after {retry.RetryAfter.TotalSeconds} seconds for retry count { currentRetryCount} remainingTime : {remainingTime  }");

                    if (retry.ShouldBeRetried && retry.RetryAfter < timeoutHelper.RemainingTime())
                    {
                        await Task.Delay(retry.RetryAfter, cancellationToken).ConfigureAwait(false);
                        continue;
                    }

                    throw;
                }

                //if (retry.RetryAfter != TimeSpan.Zero)
                //{
                //    //logger?.LogError(
                //    //    $"after {retry.RetryAfter.TotalSeconds} seconds for retry count { currentRetryCount}");
                //    await Task.Delay(retry.RetryAfter, cancellationToken).ConfigureAwait(false);
                //}
                //else
                //{
                //   // logger?.LogError($" after {retry.RetryAfter.TotalSeconds} NOW for retry count { currentRetryCount}");
                //}
            };
        }
        #endregion
    }
}
