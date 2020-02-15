using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Aksl.Retry;
using System.Net.Sockets;

namespace Aksl.Retry.ConsoleApp
{
    public class SocketExponentialRetryStrategy : ExponentialRetryStrategy
    {
        public SocketExponentialRetryStrategy(IOptions<ExponentialRetrySettings> retryOptions, ILoggerFactory loggerFactory = null) : base(retryOptions, loggerFactory)
        {
            InitializeSocketExponentialRetryStrategy();
        }

        public SocketExponentialRetryStrategy(double minBackoff , double maxBackoff , double deltaBackoff , int maxRetryCount, ILoggerFactory loggerFactory = null) : base(minBackoff, maxBackoff, deltaBackoff, maxRetryCount, loggerFactory)
        {
            InitializeSocketExponentialRetryStrategy();
        }

        protected void InitializeSocketExponentialRetryStrategy()
        {
            DoRetryExceptionTypes.Add(typeof(SocketException));
            DoRetryExceptionTypes.Add(typeof(ObjectDisposedException));
            DoRetryExceptionTypes.Add(typeof(IOException));
        }


        protected override bool TryDoRetryException(Exception exception)
        {
            return DoRetryExceptionTypes.Contains(exception.GetType());

            //switch (exception)
            //{
            //    case (exception is SocketException socketException) when IsConnectionRefused(socketException.ErrorCode)
            //    { }
            //    case SchedulingMode.ThreadPool:
            //        scheduler = PipeScheduler.ThreadPool;
            //        break;
            //    case SchedulingMode.Inline:
            //        scheduler = PipeScheduler.Inline;
            //        break;
            //    default:
            //        throw new NotSupportedException(string.Format(CultureInfo.CurrentCulture, SocketsStrings.UnknownTransportMode, socketServerOptions.ApplicationSchedulingMode));
            //}

        }

        #region Shutdown Method
        private static bool IsConnectionRefused(SocketError errorCode)
        {
            return errorCode == SocketError.ConnectionRefused ||
                   errorCode == SocketError.Interrupted;
        }

        private static bool IsConnectionNoBuffer(SocketError errorCode)
        {
            return errorCode == SocketError.NoBufferSpaceAvailable;
        }

        private static bool IsConnectionResetError(SocketError errorCode)
        {
            return errorCode == SocketError.ConnectionReset ||
                   errorCode == SocketError.ConnectionAborted ||
                   errorCode == SocketError.Shutdown;
        }

        private static bool IsConnectionAbortError(SocketError errorCode)
        {
            return errorCode == SocketError.OperationAborted ||
                   errorCode == SocketError.Interrupted ||
                   errorCode == SocketError.InvalidArgument;
        }
        #endregion
    }
}