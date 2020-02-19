using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

using Aksl.Sockets.Server;

namespace SocketServer.ConsoleApp
{
    public partial class SocketListener
    {
        #region SocketListener Method
        public async Task StartSocketListenerAsync()
        {
            await _initializeSignal.WaitAsync();

            var logger = _loggerFactory.CreateLogger($"Socket Listener");

            try
            {
                //_totalCount = 0;
                //_durationManage.Reset();
                //_durationManage.Count = 1;

                var sw = Stopwatch.StartNew();

                try
                {
                    var socketListener = ServiceProvider.GetRequiredService<ISocketServer>();

                    await socketListener.StartAsync(_cancellationTokenSource.Token);

                    //var serverAddressesFeature = socketListener.Features.Get<IServerAddressesFeature>();
                    //foreach (var addresses in serverAddressesFeature.Addresses)
                    //{
                    //    logger.LogInformation($"----Start Socket Listening : {addresses} ,now:{DateTime.Now.TimeOfDay}----");
                    //}
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error while listening: {0}", ex.Message);
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"Error while listening: {ex.Message}");
            }
        }
        #endregion
    }
}
