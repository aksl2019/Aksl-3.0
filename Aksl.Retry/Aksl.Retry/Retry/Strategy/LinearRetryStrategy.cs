using System;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Aksl.Retry
{
    public class LinearRetryStrategy : RetryStrategy
    {
        #region Members
        protected LinearRetrySettings _linearRetrySettings;
        #endregion

        #region Constructors
        public LinearRetryStrategy(IOptions<LinearRetrySettings> retryOptions, ILoggerFactory loggerFactory = null) : base(retryOptions, loggerFactory)
        {
        }

        public LinearRetryStrategy(LinearRetrySettings linearRetrySettings, ILoggerFactory loggerFactory = null) : base(linearRetrySettings, loggerFactory)
        {
        }

        public LinearRetryStrategy(double minBackoff = 0, double maxBackoff = 30, int maxRetryCount = 5, ILoggerFactory loggerFactory = null) : base(minBackoff, maxBackoff, maxRetryCount, loggerFactory)
        {
        }
        #endregion

        #region Methods
        protected override TimeSpan TryNewRetryTime(TimeSpan remainingTime, int currentRetryCount)
        {
            var nextRetryTime = currentRetryCount > MaxRetryCount ? MinimalBackoff.TotalMilliseconds : MinimalBackoff.TotalMilliseconds + (((MaximumBackoff.TotalMilliseconds - MinimalBackoff.TotalMilliseconds) * 0.05) * currentRetryCount);
            return TimeSpan.FromMilliseconds(nextRetryTime);
        }

        protected override bool TryDoRetryException(Exception currentException)
        {
            return false;
        }
        #endregion
    }
}
