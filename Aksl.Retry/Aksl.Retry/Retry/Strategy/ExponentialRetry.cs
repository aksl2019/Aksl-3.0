using System;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Aksl.Retry
{
    public class ExponentialRetryStrategy : RetryStrategy
    {
        #region Members
        protected static readonly double DefaultRetryDeltaBackoff = 3;

        protected ExponentialRetrySettings _exponentialRetrySettings;
        #endregion

        #region Constructors
        public ExponentialRetryStrategy(IOptions<ExponentialRetrySettings> retryOptions, ILoggerFactory loggerFactory = null) : base(retryOptions, loggerFactory)
        { }

        public ExponentialRetryStrategy(ExponentialRetrySettings exponentialRetrySettings, ILoggerFactory loggerFactory = null) : base(exponentialRetrySettings, loggerFactory)
        {
        }

        public ExponentialRetryStrategy(double minBackoff = 0, double maxBackoff = 30, double deltaBackoff = 3, int maxRetryCount = 5, ILoggerFactory loggerFactory = null)
                                : this(new ExponentialRetrySettings() { MinBackoff = minBackoff, MaxBackoff = maxBackoff, DeltaBackoff = deltaBackoff, MaxRetryCount = maxRetryCount }, loggerFactory)
        { }
       
        #endregion

        #region Properties
        public ExponentialRetrySettings ExponentialRetrySettings => (RetrySettings as ExponentialRetrySettings);

        public double DeltaBackoffValue => ExponentialRetrySettings.DeltaBackoff <= 0 ? DefaultRetryDeltaBackoff : ExponentialRetrySettings.DeltaBackoff;

        public TimeSpan DeltaBackoff=> TimeSpan.FromSeconds(DeltaBackoffValue);
        #endregion

        #region Methods
        protected override TimeSpan TryNewRetryTime(TimeSpan remainingTime, int currentRetryCount)
        {
            //按指数增加
            //var current = ((Math.Pow(2, currentRetryCount) * .1d) * MinimalBackoff.TotalMilliseconds) + MinimalBackoff.TotalMilliseconds;
            //// return TimeSpan.FromMilliseconds(current < MaximumBackoff.TotalMilliseconds ? current : MaximumBackoff.TotalMilliseconds);//一直到最大值
            //var retryInterval = TimeSpan.FromMilliseconds(Math.Min(current, MaximumBackoff.TotalMilliseconds));//一直到最大值
            //return retryInterval;

            int randomizedInterval = ConcurrentRandom.Next((int)(this.DeltaBackoff.TotalMilliseconds * 0.5), (int)(this.DeltaBackoff.TotalMilliseconds * 1.2));
            double increment = (Math.Pow(2, currentRetryCount) - 1) * randomizedInterval;
            double timeToSleepMsec = Math.Min(this.MinimalBackoff.TotalMilliseconds + increment, this.MaximumBackoff.TotalMilliseconds);
            var retryInterval = TimeSpan.FromMilliseconds(timeToSleepMsec);
            return retryInterval;
        }

        protected override bool TryDoRetryException(Exception currentException)
        {
            return false;
        }
        #endregion
    }
}
