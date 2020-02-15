using Aksl.Retry;
using System;
using System.Threading.Tasks;

namespace Aksl.Retry.ConsoleApp
{
    public interface IPersistentSocketConnection<TRetryStrategy> : IDisposable where TRetryStrategy : IRetryStrategy
    {
        #region Properties
       
        #endregion

        #region Methods
        bool IsConnected { get; }

        Task<bool> TryConnectAsync();
        #endregion
    }
}
