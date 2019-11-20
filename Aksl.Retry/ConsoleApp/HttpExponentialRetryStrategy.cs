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

namespace Aksl.Retry.ConsoleApp
{
    public class HttpExponentialRetryStrategy : ExponentialRetryStrategy
    {
        public HttpExponentialRetryStrategy(double minBackoff = 1, double maxBackoff = 200, double deltaBackoff = 3, int maxRetryCount=5) :
              base(minBackoff, maxBackoff, deltaBackoff, maxRetryCount)
        {
           // DoNotRetryExceptionTypes.Add(typeof(HttpRequestException));
        }

        //public override bool TryRetryAttempt(int currentRetryCount)
        //{
        //    return currentRetryCount<3;
        //}

        protected override bool TryDoNotRetryException(Exception exception)
        {
            return DoNotRetryExceptionTypes.Contains(exception.GetType());
        }
    }
}