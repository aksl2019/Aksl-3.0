using System;
using System.Collections.Generic;

namespace Aksl.Retry
{
    public interface IRetryStrategy
    {
        int MaxRetryCount { get; }

         RetrySettings RetrySettings { get; }

        /// <summary>
        /// 非重试异常
        /// </summary>
       // ISet<Type> DoNotRetryExceptionTypes { get; }
        ISet<Type> DoRetryExceptionTypes { get; }

        Retry ShouldThisBeRetried(TimeSpan remainingTime, Exception currentException, int currentRetryCount);
    }
}