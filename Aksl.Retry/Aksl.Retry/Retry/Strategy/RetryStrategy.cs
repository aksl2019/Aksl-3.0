using System;
using System.Collections.Generic;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Aksl.Retry
{
    public abstract class RetryStrategy : IRetryStrategy
    {
        #region Members
       // protected TimeSpan _minimalBackoff;

       // protected TimeSpan _maximumBackoff;

       // protected int _maxRetryCount;

        // protected ISet<Type> _doNotRetryExceptionTypes;
        protected ISet<Type> _doRetryExceptionTypes;
        protected ILoggerFactory _loggerFactory;
        protected ILogger _logger;
        #endregion

        #region Constructors
        public RetryStrategy(IOptions<RetrySettings> retryOptions, ILoggerFactory loggerFactory = null) =>
                               InitializeRetryStrategy(retryOptions ?? throw new ArgumentNullException(nameof(retryOptions)), loggerFactory);

        public RetryStrategy(RetrySettings retrySettings, ILoggerFactory loggerFactory = null) =>
                           InitializeRetryStrategy(retrySettings, loggerFactory);

        public RetryStrategy(double minBackoff = 0, double maxBackoff = 30, int maxRetryCount = 5, ILoggerFactory loggerFactory = null) =>
                              InitializeRetryStrategy(new RetrySettings {  MaxRetryCount = maxRetryCount, MinBackoff= minBackoff, MaxBackoff= maxBackoff }, loggerFactory);

        protected void InitializeRetryStrategy(IOptions<RetrySettings> retryOptions, ILoggerFactory loggerFactory)
        {
            RetrySettings = retryOptions.Value ?? RetrySettings.Default;

            InitializeRetryStrategy(RetrySettings,  loggerFactory);
        }

        protected void InitializeRetryStrategy(RetrySettings retrySettings, ILoggerFactory loggerFactory)
        {
            RetrySettings = retrySettings ?? RetrySettings.Default;

            if (RetrySettings.MaxRetryCount<= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(RetrySettings.MaxRetryCount), $"The value of the argument {nameof(RetrySettings.MaxRetryCount) } must be positive.");
            }

            if (RetrySettings.MinBackoff >= RetrySettings.MaxBackoff)
            {
                throw new ArgumentException($"The minimum back off period '{ RetrySettings.MinBackoff}' cannot exceed the maximum back off period of '{ RetrySettings.MaxBackoff}'.");
            }

            //_doNotRetryExceptionTypes = new HashSet<Type>();
            _doRetryExceptionTypes = new HashSet<Type>();

            _loggerFactory = loggerFactory ?? NullLoggerFactory.Instance;
            _logger = _loggerFactory.CreateLogger("Aksl.Retry");
        }
        #endregion

        #region Properties
        public virtual RetrySettings RetrySettings { get; private set; }

        public int MaxRetryCount => RetrySettings.MaxRetryCount;

        public TimeSpan MinimalBackoff => RetrySettings.MinBackoff <= 0 ? TimeSpan.Zero : TimeSpan.FromSeconds(RetrySettings.MinBackoff);

        public TimeSpan MaximumBackoff => TimeSpan.FromSeconds(RetrySettings.MaxBackoff);

        // public ISet<Type> DoNotRetryExceptionTypes => _doNotRetryExceptionTypes;
        public ISet<Type> DoRetryExceptionTypes => _doRetryExceptionTypes;
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
            if (!TryDoRetryException(currentException))
            {
                return Retry.No;
            }

            var retryAfter = TryNewRetryTime(remainingTime, currentRetryCount);
            return Retry.YesAfter(retryAfter);
        }

        protected virtual bool TryRetryCount(int currentRetryCount)
        {
            return currentRetryCount <= MaxRetryCount;
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
        protected abstract bool TryDoRetryException(Exception currentException);
        #endregion
    }
}
