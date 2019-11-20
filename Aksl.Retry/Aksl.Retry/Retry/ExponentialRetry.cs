using System;

namespace Aksl.Retry
{
    public class ExponentialRetryStrategy : RetryStrategy
    {
        #region Members
        protected TimeSpan _deltaBackoff;

        protected static readonly TimeSpan DefaultRetryDeltaBackoff = TimeSpan.FromSeconds(3);
        #endregion

        #region Constructors
        public ExponentialRetryStrategy(double minBackoff = 0, double maxBackoff = 30, double deltaBackoff = 3, int maxRetryCount = DefaultRetryMaxCount) :
            base(minBackoff, maxBackoff, maxRetryCount)
        {
            _deltaBackoff = deltaBackoff <= 0 ? DefaultRetryDeltaBackoff : TimeSpan.FromSeconds(deltaBackoff);
        }
        #endregion

        #region Properties
        public TimeSpan DeltaBackoff => _deltaBackoff;
        #endregion

        #region Methods
        protected override TimeSpan TryNewRetryTime(TimeSpan remainingTime, int currentRetryCount)
        {
            //按指数增加
            var current = ((Math.Pow(2, currentRetryCount) * .1d) * MinimalBackoff.TotalMilliseconds) + MinimalBackoff.TotalMilliseconds;
            // return TimeSpan.FromMilliseconds(current < MaximumBackoff.TotalMilliseconds ? current : MaximumBackoff.TotalMilliseconds);//一直到最大值
            var retryInterval = TimeSpan.FromMilliseconds(Math.Min(current, MaximumBackoff.TotalMilliseconds));//一直到最大值
            return retryInterval;

            //int randomizedInterval = ConcurrentRandom.Next((int)(this.DeltaBackoff.TotalMilliseconds * 0.5), (int)(this.DeltaBackoff.TotalMilliseconds * 1.2));
            //double increment = (Math.Pow(2, currentRetryCount) - 1) * randomizedInterval;
            //double timeToSleepMsec = Math.Min(this.MinimalBackoff.TotalMilliseconds + increment, this.MaximumBackoff.TotalMilliseconds);
            //var retryInterval = TimeSpan.FromMilliseconds(timeToSleepMsec);
            //return retryInterval;
        }

        protected override bool TryDoNotRetryException(Exception currentException)
        {
            return false;
        }
        #endregion
    }
}
