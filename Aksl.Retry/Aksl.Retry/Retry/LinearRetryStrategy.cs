using System;

namespace Aksl.Retry
{
    public class LinearRetryStrategy : RetryStrategy
    {
        #region Constructors
        public LinearRetryStrategy(double minBackoff = 0, double maxBackoff = 30, int maxRetryCount = DefaultRetryMaxCount) :
            base(minBackoff, maxBackoff, maxRetryCount)
        {

        }
        #endregion

        #region Methods
        protected override TimeSpan TryNewRetryTime(TimeSpan remainingTime, int currentRetryCount)
        {
            var nextRetryTime = currentRetryCount > MaxRetryCount ? MinimalBackoff.TotalMilliseconds : MinimalBackoff.TotalMilliseconds + (((MaximumBackoff.TotalMilliseconds - MinimalBackoff.TotalMilliseconds) * 0.05) * currentRetryCount);
            return TimeSpan.FromMilliseconds(nextRetryTime);
        }

        protected override bool TryDoNotRetryException(Exception currentException)
        {
            return false;
        }
        #endregion
    }
}
