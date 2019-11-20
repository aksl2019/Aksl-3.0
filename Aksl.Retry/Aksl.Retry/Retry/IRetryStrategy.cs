using System;
using System.Collections.Generic;

namespace Aksl.Retry
{
    public interface IRetryStrategy
    {
        int MaxRetryCount
        {
            get;
        }

        /// <summary>
        /// 非重试异常
        /// </summary>
        ISet<Type> DoNotRetryExceptionTypes
        {
            get;
        }

        Retry ShouldThisBeRetried(TimeSpan remainingTime, Exception currentException, int currentRetryCount);
    }
}