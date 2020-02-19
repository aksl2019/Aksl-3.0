using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipelines;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Aksl.Sockets.Server
{
    public partial class DuplexSocketConnection
    {
        #region Send Method
        /// <summary>
        /// 等待transport.Output写入数据,然后发送到客户端,DuplexPipeSender.ProcessInputAsync
        /// </summary>
        /// <returns></returns>
        public async ValueTask DoSendAsync()
        {
            Exception error = null;

            try
            {
                await ProcessSendsAsync();
            }
            catch (SocketException ex) when (IsConnectionResetError(ex.SocketErrorCode))
            {
                // A connection reset can be reported as SocketError.ConnectionAborted on Windows
                error = null;
                _trace.ConnectionReset(ConnectionId);
            }
            catch (SocketException ex) when (IsConnectionAbortError(ex.SocketErrorCode))
            {
                error = null;
            }
            catch (ObjectDisposedException)
            {
                error = null;
            }
            catch (IOException ex)
            {
                error = ex;
                _trace.ConnectionError(ConnectionId, error);
            }
            catch (Exception ex)
            {
                error = new IOException(ex.Message, ex);
                _trace.ConnectionError(ConnectionId, error);
            }
            finally
            {
                Shutdown();

                // Complete the output after disposing the socket
                //  _outputPipe.Reader.Complete(error);
                _application.Input.Complete(error);
            }
        }

        private async ValueTask ProcessSendsAsync()
        {
            while (true)
            {
                // var result = await _outputPipe.Reader.ReadAsync();
                var result = await _application.Input.ReadAsync();// output.Reader(IOQueue)
                var buffer = result.Buffer;

                if (result.IsCanceled)
                {
                    break;
                }

                var end = buffer.End;
                var isCompleted = result.IsCompleted;
                if (!buffer.IsEmpty)
                {
                    var isSingleSegment = buffer.IsSingleSegment;
                    var length = buffer.Length;

                    int minimumSegmentSize = _socketPipeSettings.Input.MinimumSegmentSize;
                    Debug.Assert(length <= minimumSegmentSize);

                    // await Task.Delay(TimeSpan.FromMilliseconds(20));

                    await _sender.SendAsync(buffer);
                }

                // This is not interlocked because there could be a concurrent writer.
                // Instead it's to prevent read tearing on 32-bit systems.
                //         Interlocked.Add(ref _totalBytesWritten, buffer.Length);

                //   _logger.LogInformation($"Total Send Bytes :{TotalBytesWritten},now:{DateTime.Now.TimeOfDay}");

                // tell the pipe that we used everything
                _application.Input.AdvanceTo(end);

                if (isCompleted)
                {
                    break;
                }
            }
        }
        #endregion
    }
}