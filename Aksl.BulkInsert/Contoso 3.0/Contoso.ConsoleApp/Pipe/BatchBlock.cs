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

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

using Aksl.Concurrency;
using Aksl.BulkInsert;

using Contoso.Domain.Models;
using Contoso.DataSource.Dtos;
using Contoso.DataSource;

namespace Contoso.ConsoleApp
{
    public partial class WebApiSender
    {
        #region Batch Messages Blocks
        public async Task MultiplePipeBlockAsync(int recieverCount = 1, int messageCount = 10000)
        {
            for (int i = 0; i < recieverCount; i++)
            {
                await PipeBulkInsertBlockAsync(i, messageCount);
            }
        }

        public async Task PipeBulkInsertBlockAsync(int seqNumber = 1, int orderCount = 4000)
        {
            if (!_isInitialize)
            {
                throw new InvalidOperationException("not initialize");
            }

            #region Block Method
            (int blockCount, int minPerBlock, int maxPerBlock) blockBascInfo = BlockHelper.GetBasciBlockInfo(orderCount);
            int[] blockInfos = BlockHelper.GetBlockInfo(messageCount: orderCount, blockCount: blockBascInfo.blockCount, minPerBlock: blockBascInfo.minPerBlock, maxPerBlock: blockBascInfo.maxPerBlock);
            #endregion

            int boundedCapacity = blockInfos.Sum(b => b);
            Debug.Assert(orderCount == boundedCapacity);

            var logger = _loggerFactory.CreateLogger($"PipeBulkInser-{seqNumber}:{orderCount}");

            try
            {
                var pipeBulkInserter = this.ServiceProvider.GetRequiredService<IPipeBulkInserter<OrderDto, OrderDto>>();

                var contosoDataSourceFactory = this.ServiceProvider.GetRequiredService<IDataSourceFactory<IContosoDataSource>>();
                var orderDataSource = contosoDataSourceFactory.Current.OrderDataSource;

                _totalCount = 0;
                var transportTimeWatcher = Stopwatch.StartNew();
                TimeSpan totalTransportTime = TimeSpan.Zero;
                var executionTimeWatcher = Stopwatch.StartNew();

                logger.LogInformation($"----begin pipe bulk insert {orderCount} orders,now:{DateTime.Now.TimeOfDay}----");

                int start = 0;
                for (int i = 0; i < blockInfos.Count(); i++)
                {
                    int insertOrderCount = blockInfos[i];
                    var orders = OrderJsonProvider.CreateOrders(start, insertOrderCount);
                    start += insertOrderCount;

                    transportTimeWatcher.Restart();
                    var dbOrders = await orderDataSource.PipeBulkInsertOrdersAsync(orders);
                    totalTransportTime += transportTimeWatcher.Elapsed;
                    transportTimeWatcher.Reset();

                    if (dbOrders?.Count() > 0)
                    {
                        await ProcessPipeOrdersAsync(dbOrders);
                    }
                }

                logger
                    .LogInformation($"----pipe bulk insert {orderCount} orders,cost time:\"{executionTimeWatcher.Elapsed}\",transport time:{ totalTransportTime },count/time(sec):{Math.Ceiling(orderCount / totalTransportTime.TotalSeconds)},now:\"{DateTime.Now.TimeOfDay}\"----");
            }
            catch (Exception ex)
            {
                logger.LogError($"Error while pipe bulk insert: {ex.Message}");
            }
        }
        #endregion
    }
}
