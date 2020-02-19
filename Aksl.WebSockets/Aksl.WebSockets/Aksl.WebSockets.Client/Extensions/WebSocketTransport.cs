//using System;
//using System.Buffers;
//using System.Diagnostics;
//using System.IO.Pipelines;
//using System.Net;
//using System.Net.WebSockets;
//using System.Runtime.InteropServices;
//using System.Threading;
//using System.Threading.Tasks;

//using Microsoft.Extensions.Logging;

//namespace Aksl.WebSockets.Client
//{
//    public partial class WebSocketTransport
//    {
//        #region Members
//        private static readonly bool IsWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

//        private readonly ClientWebSocket _clientWebSocket;

//        private readonly WebSocketClientSettings _webSocketClientSettings;
//        private readonly MemoryPool<byte> _memoryPool;

//        private readonly PipeScheduler _writerScheduler;
//        private readonly PipeScheduler _readerScheduler;
//        private readonly Pipe _pipe;

//        private volatile bool _aborted;

//        private readonly ILogger _logger;
//        private readonly ILoggerFactory _loggerFactory;
//        #endregion

//        #region Constructors
//        public WebSocketTransport(ClientWebSocket clientWebSocket,  MemoryPool<byte> memoryPool, PipeScheduler writerScheduler, PipeScheduler readerScheduler, WebSocketClientSettings webSocketClientSettings, ILoggerFactory loggerFactory = null)
//        {
//            _clientWebSocket = clientWebSocket ?? throw new ArgumentNullException(nameof(clientWebSocket));
//            _readerScheduler = readerScheduler ?? throw new ArgumentNullException(nameof(readerScheduler));
//            _writerScheduler = writerScheduler ?? throw new ArgumentNullException(nameof(writerScheduler));
//            _memoryPool = memoryPool ?? throw new ArgumentNullException(nameof(memoryPool));
//            _webSocketClientSettings= webSocketClientSettings ?? throw new ArgumentNullException(nameof(webSocketClientSettings));

//            _loggerFactory = loggerFactory ?? new LoggerFactory()
//                                                     .AddDebug(LogLevel.Trace)
//                                                     .AddConsole(LogLevel.Trace);
//            _logger = _loggerFactory.CreateLogger<WebSocketTransport>();

//            var awaiterScheduler = IsWindows ? readerScheduler : PipeScheduler.Inline;
//            var options = ClientPipeOptions.GetPipeOptions(writerScheduler, readerScheduler, memoryPool,
//                                                           _webSocketClientSettings.MaxBufferSize, _webSocketClientSettings.MinimumSegmentSize, _webSocketClientSettings.UseSynchronizationContext);
//            _pipe = new Pipe(options);
//        }
//        #endregion

//        #region Properties
//        public  PipeWriter Input => _pipe.Writer;

//        public PipeReader Output => _pipe.Reader;

//        internal Task Running { get; private set; } = Task.CompletedTask;

//        private int MinAllocBufferSize => _webSocketClientSettings.MinimumSegmentSize / 2;
//        #endregion

//        #region Start Method
//        public async Task StartAsync()
//        {
//            Running = ProcessSocketAsync(_clientWebSocket);
//        }

//        private async Task ProcessSocketAsync(WebSocket socket)
//        {
//            using (socket)
//            {
//                // Begin sending and receiving.
//                var receiving = StartReceiving(socket);
//                var sending = StartSending(socket);

//                // Wait for send or receive to complete
//                var trigger = await Task.WhenAny(receiving, sending);

//                if (trigger == receiving)
//                {
//                    // We're waiting for the application to finish and there are 2 things it could be doing
//                    // 1. Waiting for application data
//                    // 2. Waiting for a websocket send to complete

//                    // Cancel the application so that ReadAsync yields
//                    Input.CancelPendingRead();

//                    using (var delayCts = new CancellationTokenSource())
//                    {
//                        var resultTask = await Task.WhenAny(sending, Task.Delay(_closeTimeout, delayCts.Token));

//                        if (resultTask != sending)
//                        {
//                            _aborted = true;

//                            // Abort the websocket if we're stuck in a pending send to the client
//                            socket.Abort();
//                        }
//                        else
//                        {
//                            // Cancel the timeout
//                            delayCts.Cancel();
//                        }
//                    }
//                }
//                else
//                {
//                    // We're waiting on the websocket to close and there are 2 things it could be doing
//                    // 1. Waiting for websocket data
//                    // 2. Waiting on a flush to complete (backpressure being applied)

//                    _aborted = true;

//                    // Abort the websocket if we're stuck in a pending receive from the client
//                    socket.Abort();

//                    // Cancel any pending flush so that we can quit
//                    _application.Output.CancelPendingFlush();
//                }
//            }
//        }

//        private async Task StartReceiving( byte[] bytes)
//        {
//            try
//            {
//                while (true)
//                {
//#if NETCOREAPP2_2
//                    var result = await socket.ReceiveAsync(Memory<byte>.Empty, CancellationToken.None);

//                    if (result.MessageType == WebSocketMessageType.Close)
//                    {
//                        Log.WebSocketClosed(_logger, _webSocket.CloseStatus);

//                        if (_webSocket.CloseStatus != WebSocketCloseStatus.NormalClosure)
//                        {
//                            throw new InvalidOperationException($"Websocket closed with error: {_webSocket.CloseStatus}.");
//                        }

//                        return;
//                    }
//#endif
//                    var memory = _application.Output.GetMemory();
//#if NETCOREAPP2_2
//                    // Because we checked the CloseStatus from the 0 byte read above, we don't need to check again after reading
//                    var receiveResult = await socket.ReceiveAsync(memory, CancellationToken.None);
//#else
//                    var isArray = MemoryMarshal.TryGetArray<byte>(memory, out var arraySegment);
//                    Debug.Assert(isArray);

//                    // Exceptions are handled above where the send and receive tasks are being run.
//                    var receiveResult = await socket.ReceiveAsync(arraySegment, CancellationToken.None);
//#endif
//                    // Need to check again for NetCoreApp2.2 because a close can happen between a 0-byte read and the actual read
//                    if (receiveResult.MessageType == WebSocketMessageType.Close)
//                    {
//                        Log.WebSocketClosed(_logger, _webSocket.CloseStatus);

//                        if (_webSocket.CloseStatus != WebSocketCloseStatus.NormalClosure)
//                        {
//                            throw new InvalidOperationException($"Websocket closed with error: {_webSocket.CloseStatus}.");
//                        }

//                        return;
//                    }

//                    Log.MessageReceived(_logger, receiveResult.MessageType, receiveResult.Count, receiveResult.EndOfMessage);

//                    _application.Output.Advance(receiveResult.Count);

//                    var flushResult = await _application.Output.FlushAsync();

//                    // We canceled in the middle of applying back pressure
//                    // or if the consumer is done
//                    if (flushResult.IsCanceled || flushResult.IsCompleted)
//                    {
//                        break;
//                    }
//                }
//            }
//            catch (OperationCanceledException)
//            {
//                Log.ReceiveCanceled(_logger);
//            }
//            catch (Exception ex)
//            {
//                if (!_aborted)
//                {
//                    _application.Output.Complete(ex);

//                    // We re-throw here so we can communicate that there was an error when sending
//                    // the close frame
//                    throw;
//                }
//            }
//            finally
//            {
//                // We're done writing
//                _application.Output.Complete();

//                Log.ReceiveStopped(_logger);
//            }
//        }

//        private async Task StartSending(WebSocket socket)
//        {
//            Exception error = null;

//            try
//            {
//                while (true)
//                {
//                    var result = await _application.Input.ReadAsync();
//                    var buffer = result.Buffer;

//                    // Get a frame from the application

//                    try
//                    {
//                        if (result.IsCanceled)
//                        {
//                            break;
//                        }

//                        if (!buffer.IsEmpty)
//                        {
//                            try
//                            {
//                                Log.ReceivedFromApp(_logger, buffer.Length);

//                                if (WebSocketCanSend(socket))
//                                {
//                                    await socket.SendAsync(buffer, _webSocketMessageType);
//                                }
//                                else
//                                {
//                                    break;
//                                }
//                            }
//                            catch (Exception ex)
//                            {
//                                if (!_aborted)
//                                {
//                                    Log.ErrorSendingMessage(_logger, ex);
//                                }
//                                break;
//                            }
//                        }
//                        else if (result.IsCompleted)
//                        {
//                            break;
//                        }
//                    }
//                    finally
//                    {
//                        _application.Input.AdvanceTo(buffer.End);
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                error = ex;
//            }
//            finally
//            {
//                if (WebSocketCanSend(socket))
//                {
//                    // We're done sending, send the close frame to the client if the websocket is still open
//                    await socket.CloseOutputAsync(error != null ? WebSocketCloseStatus.InternalServerError : WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
//                }

//                _application.Input.Complete();

//                Log.SendStopped(_logger);
//            }
//        }

//        private static bool WebSocketCanSend(WebSocket ws)
//        {
//            return !(ws.State == WebSocketState.Aborted ||
//                   ws.State == WebSocketState.Closed ||
//                   ws.State == WebSocketState.CloseSent);
//        }
//        #endregion
//    }
//}
