using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using Aksl.Retry;
using Microsoft.Extensions.Options;

namespace Aksl.Retry.ConsoleApp
{
    public class HttpExponentialRetryStrategy : ExponentialRetryStrategy
    {
        public HttpExponentialRetryStrategy(IOptions<ExponentialRetrySettings> retryOptions, ILoggerFactory loggerFactory = null) : base(retryOptions, loggerFactory)
        {
            DoRetryExceptionTypes.Add(typeof(HttpRequestException));
        }

        public HttpExponentialRetryStrategy(double minBackoff , double maxBackoff , double deltaBackoff , int maxRetryCount, ILoggerFactory loggerFactory = null) : base(minBackoff, maxBackoff, deltaBackoff, maxRetryCount, loggerFactory)
        {
            DoRetryExceptionTypes.Add(typeof(HttpRequestException));
        }

        //public override bool TryRetryAttempt(int currentRetryCount)
        //{
        //    return currentRetryCount<3;
        //}

        protected override bool TryDoRetryException(Exception exception)
        {
            return DoRetryExceptionTypes.Contains(exception.GetType());
        }
    }
}