using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

using Aksl.Retry;

namespace Aksl.Retry.ConsoleApp
{
    public class PersistentSocketConnection<TRetryStrategy> : IPersistentSocketConnection<TRetryStrategy> where TRetryStrategy : IRetryStrategy
    {
        #region Members
        protected IRetrier<TRetryStrategy> _retrier;
        private Socket _clientSocket;

        private bool _disposed;

        protected ILoggerFactory _loggerFactory;
        private ILogger _logger;

        public bool IsConnected => throw new NotImplementedException();
        #endregion

        #region Constructors
        //DI方式
        public PersistentSocketConnection(IRetrier<TRetryStrategy> retrier, ILoggerFactory loggerFactory = null)
        {
            _retrier = retrier;

            _loggerFactory = loggerFactory ?? NullLoggerFactory.Instance;
            _logger = loggerFactory?.CreateLogger(nameof(PersistentSocketConnection<TRetryStrategy>));
        }

        public Task<bool> TryConnectAsync()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
        #endregion


    }
}
