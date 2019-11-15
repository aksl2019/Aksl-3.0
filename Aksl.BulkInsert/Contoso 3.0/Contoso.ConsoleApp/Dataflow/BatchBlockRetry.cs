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
        #region Batch Messages Block Retry
        public async Task MultipleDataflowBulkInsertBlockRetryAsync(int recieverCount = 1, int maxRetryCount = 50, int messageCount = 2000)
        {
            for (int i = 0; i < recieverCount; i++)
            {
                await DataflowBulkInsertBlockRetryAsync(i + 1, maxRetryCount, messageCount);
            }
        }

        public async Task DataflowBulkInsertBlockRetryAsync(int seqNumber = 1, int maxRetryCount = 200, int orderCount = 1000)
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

            var logger = _loggerFactory.CreateLogger($"DataflowBulkInser-{seqNumber}:{maxRetryCount * orderCount}");

            int currentRetryCount = 0;

            try
            {
                var dataflowBulkInserter = this.ServiceProvider.GetRequiredService<IDataflowBulkInserter<OrderDto, OrderDto>>();

                var contosoDataSourceFactory = this.ServiceProvider.GetRequiredService<IDataSourceFactory<IContosoDataSource>>();
                var orderDataSource = contosoDataSourceFactory.Current.OrderDataSource;

                TimeSpan period = TimeSpan.FromMilliseconds(50);

                _totalCount = 0;
                var transportTimeWatcher = Stopwatch.StartNew();
                TimeSpan totalTransportTime = TimeSpan.Zero;
                var executionTimeWatcher = Stopwatch.StartNew();

                logger.LogInformation($"----begin dataflow bulk insert {maxRetryCount * orderCount} orders,now:{DateTime.Now.TimeOfDay}----");

                while (true)
                {
                    if (_cancellationTokenSource.Token.IsCancellationRequested)
                    {
                        _cancellationTokenSource.Token.ThrowIfCancellationRequested();
                        // _logger?.LogCritical($"message was cancelled");
                        break;
                    }

                    int start = 0;
                    for (int i = 0; i < blockInfos.Count(); i++)
                    {
                        int insertOrderCount = blockInfos[i];
                        var orders = OrderJsonProvider.CreateOrders(start, insertOrderCount);
                        start += insertOrderCount;

                        transportTimeWatcher.Restart();
                        var dbOrders = await orderDataSource.DataflowBulkInsertOrdersAsync(orders);
                        totalTransportTime += transportTimeWatcher.Elapsed;
                        transportTimeWatcher.Reset();

                        //if (dbOrders?.Count() > 0)
                        //{
                        //    await ProcessDataflowOrdersAsync(dbOrders);
                        //}
                    }

                    //  await Task.Delay(TimeSpan.FromMilliseconds(200));
                    if (maxRetryCount==99)
                    {
                        await Task.Delay(period);
                    }

                    currentRetryCount++;
                    if (currentRetryCount >= maxRetryCount)
                    {
                         //_cancellationTokenSource.CancelAfter(TimeSpan.FromMilliseconds(200));
                        break;
                    }
                }

                logger
                    .LogInformation($"----dataflow bulk insert {maxRetryCount * orderCount} orders,cost time:\"{executionTimeWatcher.Elapsed}\",transport time:{ totalTransportTime },count/time(sec):{Math.Ceiling(maxRetryCount * orderCount / totalTransportTime.TotalSeconds)},now:\"{DateTime.Now.TimeOfDay}\"----");
            }
            catch (Exception ex)
            {
                logger.LogError($"Error while dataflow bulk insert: {ex.Message}");
            }
        }
        #endregion
    }
}
