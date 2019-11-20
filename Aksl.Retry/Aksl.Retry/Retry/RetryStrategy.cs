using System;
using System.Collections.Generic;

namespace Aksl.Retry
{
    public abstract class RetryStrategy : IRetryStrategy
    {
        #region Members
        protected TimeSpan _minimalBackoff;

        protected TimeSpan _maximumBackoff;

        protected int _maxRetryCount;

        protected ISet<Type> _doNotRetryExceptionTypes;

        protected const int DefaultRetryMaxCount = 5;

        protected readonly TimeSpan DefaultRetryMinBackoff = TimeSpan.Zero;

        protected readonly TimeSpan DefaultRetryMaxBackoff = TimeSpan.FromSeconds(30);
        #endregion

        #region Constructors
        public RetryStrategy(double minBackoff = 0, double maxBackoff = 30, int maxRetryCount = DefaultRetryMaxCount) =>
                                                                      InitializeRetryStrategy(minBackoff, maxBackoff, maxRetryCount);

        protected void InitializeRetryStrategy(double minBackoff = 0, double maxBackoff = 30, int maxRetryCount = DefaultRetryMaxCount)
        {
            if (maxRetryCount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxRetryCount), $"The value of the argument { nameof(maxRetryCount) } must be positive.");
            }

            if (minBackoff >= maxBackoff)
            {
                throw new ArgumentException($"The minimum back off period '{minBackoff}' cannot exceed the maximum back off period of '{maxBackoff}'.");
            }

            _maxRetryCount = maxRetryCount;
            _minimalBackoff = minBackoff <= 0 ? DefaultRetryMinBackoff : TimeSpan.FromSeconds(minBackoff);
            _maximumBackoff = TimeSpan.FromSeconds(maxBackoff);

            _doNotRetryExceptionTypes = new HashSet<Type>();
        }
        #endregion

        #region Properties
        public int MaxRetryCount => _maxRetryCount;

        public TimeSpan MinimalBackoff => _minimalBackoff;

        public TimeSpan MaximumBackoff => _maximumBackoff;

        public ISet<Type> DoNotRetryExceptionTypes => _doNotRetryExceptionTypes;
        #endregion

        #region IRetryStrategy
        public virtual Retry ShouldThisBeRetried(TimeSpan remainingTime, Exception currentException, int currentRetryCount)
        {
            //当前重复次数
            if (!TryRetryCount(currentRetryCount))
            {
                return Retry.No;
            }

            //当前异常不需要重试
            if (TryDoNotRetryException(currentException))
            {
                return Retry.No;
            }

            var retryAfter = TryNewRetryTime(remainingTime, currentRetryCount);
            return Retry.YesAfter(retryAfter);
        }

        protected virtual bool TryRetryCount(int currentRetryCount)
        {
            return currentRetryCount <= _maxRetryCount;
        }

        protected virtual TimeSpan TryNewRetryTime(TimeSpan remainingTime, int currentRetryCount)
        {
            // var newRetryCount = GetNextRetryCount(currentRetryCount);
            return TimeSpan.FromMilliseconds(currentRetryCount);
        }

        // protected abstract double GetNextRetryCount(int currentRetryCount);

        /// <summary>
        /// 可重试的异常
        /// </summary>
        /// <param name="exception">Current Exception</param>
        /// <returns>bool</returns>
        protected abstract bool TryDoNotRetryException(Exception currentException);
        #endregion
    }
}
