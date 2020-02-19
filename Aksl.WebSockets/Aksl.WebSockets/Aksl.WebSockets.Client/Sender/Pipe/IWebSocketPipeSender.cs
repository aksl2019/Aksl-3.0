using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;

using Aksl.WebSockets.Client.Configuration;

namespace Aksl.WebSockets.Client
{
    #region ISocketPipeSender
    /// <summary>
    /// WebSocket PipeSender Interface
    /// </summary>
    public interface IWebSocketPipeSender : IDisposable
    {
        #region Properties
        Action<MessageContext> OnSendCallBack { get; set; }

        IEndPointInformation EndPointInformation { get; }

        //   IProcessor Processor { get; set; }

        //  Func<ReadOnlySequence<byte>, CancellationToken, ValueTask> Handler { get; set; }
        #endregion

        #region  Methods
        // ValueTask StartAsync();

        //  ValueTask StartConnectAsync();

       // void CloseSocket();

        Task<IEnumerable<byte[]>> SendBatchAsync(IEnumerable<byte[]> messages, CancellationToken cancellationToken = default);

        //Task<IEnumerable<byte[]>> SendBatchDuplexPipeAsync(IEnumerable<byte[]> messages, CancellationToken cancellationToken = default);

       // Task<IEnumerable<string>> SendBatchStringAsync(IEnumerable<byte[]> messages, CancellationToken cancellationToken = default);
        #endregion
    }
    #endregion
}