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
        #region Block Dataflow Pipe Bulk Insert Tasks
        public async Task DataflowPipeBulkInsertBlockTasksAsync(int taskCount = 2,  int orderCount = 5000)
        {
            if (!_isInitialize)
            {
                throw new InvalidOperationException("not initialize");
            }

            _totalCount = 0;
            long totalOrderCount = taskCount  * orderCount;
            var executionTimeWatcher = Stopwatch.StartNew();
            var signals = new AsyncCountdownEvent(taskCount);

            executionTimeWatcher.Restart();

            var insertTasks = from tc in Enumerable.Range(0, taskCount)
                              select Task.Run(() =>
                              {
                                  return DataflowPipeBulkInsertBlockCoreAsync(signals, tc, orderCount);
                              }, _cancellationTokenSource.Token);

            _logger.LogInformation($"----begin dataflow pipe bulk insert { totalOrderCount } orders,now:{DateTime.Now.TimeOfDay}----");

            await Task.WhenAll(insertTasks);

            await signals.WaitAsync();

            _logger
                  .LogInformation($"----finish dataflow pipe bulk insert {totalOrderCount} orders,cost time:\"{executionTimeWatcher.Elapsed},count/time(sec):{Math.Ceiling(totalOrderCount / executionTimeWatcher.Elapsed.TotalSeconds)},now:\"{DateTime.Now.TimeOfDay}\"----");

            await Task.Delay(TimeSpan.FromMilliseconds(200));
        }

        public async Task DataflowPipeBulkInsertBlockCoreAsync(AsyncCountdownEvent signals, int index,  int orderCount)
        {
            #region Block Method
            (int blockCount, int minPerBlock, int maxPerBlock) blockBascInfo = BlockHelper.GetBasciBlockInfo(orderCount);
            int[] blockInfos = BlockHelper.GetBlockInfo(messageCount: orderCount, blockCount: blockBascInfo.blockCount, minPerBlock: blockBascInfo.minPerBlock, maxPerBlock: blockBascInfo.maxPerBlock);
            #endregion

            int boundedCapacity = blockInfos.Sum(b => b);
            Debug.Assert(orderCount == boundedCapacity);

            var logger = _loggerFactory.CreateLogger($"DataflowPipeBulkInserter-{index}");

            try
            {
                var contosoDataSourceFactory = this.ServiceProvider.GetRequiredService<IDataSourceFactory<IContosoDataSource>>();
                var orderDataSource = contosoDataSourceFactory.Current.OrderDataSource;

                var transportTimeWatcher = Stopwatch.StartNew();
                TimeSpan totalTransportTime = TimeSpan.Zero;
                var executionTimeWatcher = Stopwatch.StartNew();

                logger.LogInformation($"----begin dataflow pipe bulk insert {orderCount} orders,now:{DateTime.Now.TimeOfDay}----");

                int start = 0;
                for (int i = 0; i < blockInfos.Count(); i++)
                {
                    int insertOrderCount = blockInfos[i];
                    var orders = OrderJsonProvider.CreateOrders(start, insertOrderCount);
                    start += insertOrderCount;

                    transportTimeWatcher.Restart();
                    var dbOrders = await orderDataSource.DataflowPipeBulkInsertOrdersAsync(orders);
                    totalTransportTime += transportTimeWatcher.Elapsed;
                    transportTimeWatcher.Reset();

                    //if (dbOrders?.Count() > 0)
                    //{
                    //    await ProcessDataflowPipeOrdersAsync(dbOrders);
                    //}
                }

                logger
                  .LogInformation($"----dataflow pipe bulk insert {orderCount} orders,cost time:\"{executionTimeWatcher.Elapsed}\",transport time:{ totalTransportTime },count/time(sec):{Math.Ceiling(orderCount / totalTransportTime.TotalSeconds)},now:\"{DateTime.Now.TimeOfDay}\"----");

                signals?.Signal();
            }
            catch (Exception ex)
            {
                logger.LogError($"Error while dataflow pipe bulk insert orders of {nameof(DataflowPipeBulkInsertLoopCoreAsync)}: {ex.Message}");
            }
        }
        #endregion
    }
}
