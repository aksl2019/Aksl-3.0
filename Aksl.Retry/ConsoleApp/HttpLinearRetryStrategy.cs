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
    public class HttpLinearRetryStrategy : LinearRetryStrategy
    {
        public HttpLinearRetryStrategy(double minBackoff = 0, double maxBackoff = 30, int maxRetryCount=5) :
              base(minBackoff, maxBackoff, maxRetryCount)
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