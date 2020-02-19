using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Aksl.Data;
using Aksl.Concurrency;
using Aksl.BulkInsert;

using Contoso.DataSource.Dtos;
using Contoso.DataSource;

namespace Contoso.ConsoleApp
{
    public partial class WebApiSender
    {
        #region Delete Method
        public async ValueTask DeleteSaleOrdersAsync()
        {
            int pageIndex = 0;
            int pageSize = 5_000;
            int totalCount = 0;
            int maxRetryCount = 2;
            int currentRetryCount = 0;

            var logger = _loggerFactory.CreateLogger($"{nameof(DeleteSaleOrdersAsync)}");

            try
            {
                var executionTimeWatcher = Stopwatch.StartNew();

                var contosoDataSourceFactory = this.ServiceProvider.GetRequiredService<IDataSourceFactory<IContosoDataSource>>();
                var orderDataSource = contosoDataSourceFactory.Current.OrderDataSource;

                logger.LogInformation($"----begin read sale orders,  pageSize:{pageSize},now:{DateTime.Now.TimeOfDay}----");
                List<SaleOrderDto> saleOrderDtos = new List<SaleOrderDto>();

                while (true)
                {
                    // logger.LogInformation($"---- pageIndex:{ pageIndex},now:{DateTime.Now.TimeOfDay}----");


                    var pagedOrderDtos = orderDataSource.GetPagedSaleOrderListAsync(pageIndex, pageSize);

                    int currentCount = 0;
                    await foreach (var orderDto in pagedOrderDtos)
                    {
                        totalCount++;
                        currentCount++;

                        saleOrderDtos.Add(orderDto);
                        //Console.WriteLine($"OrderId: {order.Id},OrderStatus: {order.Status}");
                        //Console.WriteLine($"OrderId: {order.Id},OrderNumber: {order.OrderNumber},OrderStatus: {order.Status},Customer: {order.CustomerId}");
                    }

                    //if (saleOrderDtos.Any())
                    //{
                    //    await orderDataSource.DeleteSaleOrdersAsync(saleOrderDtos);
                    //}

                    //if (currentCount <= 0)
                    //{
                    //    //_cancellationTokenSource.CancelAfter(TimeSpan.FromMilliseconds(200));
                    //    break;
                    //}

                    currentRetryCount++;
                    if (currentRetryCount >= maxRetryCount)
                    {
                        //_cancellationTokenSource.CancelAfter(TimeSpan.FromMilliseconds(200));
                        break;
                    }

                    pageIndex++;
                }

                if (saleOrderDtos.Any())
                {
                    await DataflowBulkDeleteBlockTasksAsync(saleOrderDtos);
                }

                logger
                    .LogInformation($"----delete { totalCount} sale orders,cost time:\"{executionTimeWatcher.Elapsed}\",count/time(sec):{Math.Ceiling(totalCount / executionTimeWatcher.Elapsed.TotalSeconds)},now:\"{DateTime.Now.TimeOfDay}\"----");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}\n");
            }
        }
        #endregion

        #region Dataflow Bulk Delete Tasks
        public async Task DataflowBulkDeleteBlockTasksAsync(List<SaleOrderDto> saleOrderDtos)
        {
            if (!_isInitialize)
            {
                throw new InvalidOperationException("not initialize");
            }

            var orderCount = saleOrderDtos.Count;

            #region Block Method
            (int blockCount, int minPerBlock, int maxPerBlock) blockBascInfo = BlockHelper.GetBasciBlockInfo(orderCount);
            int[] blockInfos = BlockHelper.GetBlockInfo(messageCount: orderCount, blockCount: blockBascInfo.blockCount, minPerBlock: blockBascInfo.minPerBlock, maxPerBlock: blockBascInfo.maxPerBlock);
            #endregion

            int taskCount = blockInfos.Count();
            var blockOrders = BlockHelper.GetMessageByBlockInfo<SaleOrderDto>(blockInfos, saleOrderDtos.ToArray()).ToList();

            _totalCount = 0;
            long totalOrderCount = taskCount * orderCount;
            var executionTimeWatcher = Stopwatch.StartNew();
            var signals = new AsyncCountdownEvent(taskCount);

            executionTimeWatcher.Restart();

            var insertTasks = from tc in Enumerable.Range(0, taskCount)
                              select Task.Run(() =>
                              {
                                  return DataflowBulkDeleteBlockCoreAsync(signals, tc, blockOrders[tc].ToList());
                              }, _cancellationTokenSource.Token);

            _logger.LogInformation($"----begin dataflow bulk delete { totalOrderCount } orders,now:{DateTime.Now.TimeOfDay}----");

            await Task.WhenAll(insertTasks);

            await signals.WaitAsync();

            _logger
                .LogInformation($"----finish dataflow bulk delete {totalOrderCount} orders,cost time:\"{executionTimeWatcher.Elapsed},count/time(sec):{Math.Ceiling(totalOrderCount / executionTimeWatcher.Elapsed.TotalSeconds)},now:\"{DateTime.Now.TimeOfDay}\"----");

            await Task.Delay(TimeSpan.FromMilliseconds(200));
        }

        public async Task DataflowBulkDeleteBlockCoreAsync(AsyncCountdownEvent signals, int index, List<SaleOrderDto> saleOrderDtos)
        {
            var orderCount = saleOrderDtos.Count;

            //#region Block Method
            //(int blockCount, int minPerBlock, int maxPerBlock) blockBascInfo = BlockHelper.GetBasciBlockInfo(orderCount);
            //int[] blockInfos = BlockHelper.GetBlockInfo(messageCount: orderCount, blockCount: blockBascInfo.blockCount, minPerBlock: blockBascInfo.minPerBlock, maxPerBlock: blockBascInfo.maxPerBlock);
            //#endregion

            //int boundedCapacity = blockInfos.Sum(b => b);
            //Debug.Assert(orderCount == boundedCapacity);

            var logger = _loggerFactory.CreateLogger($"DataflowBulkDeleter-{index}");

            try
            {
                var contosoDataSourceFactory = this.ServiceProvider.GetRequiredService<IDataSourceFactory<IContosoDataSource>>();
                var orderDataSource = contosoDataSourceFactory.Current.OrderDataSource;

                var transportTimeWatcher = Stopwatch.StartNew();
                TimeSpan totalTransportTime = TimeSpan.Zero;
                var executionTimeWatcher = Stopwatch.StartNew();

                logger.LogInformation($"----begin dataflow bulk delete {orderCount} orders,now:{DateTime.Now.TimeOfDay}----");

                //var blockOrders = BlockHelper.GetMessageByBlockInfo<SaleOrderDto>(blockInfos, saleOrderDtos.ToArray());

                //foreach (var orders in blockOrders)
                //{
                transportTimeWatcher.Restart();
                await orderDataSource.DeleteSaleOrdersAsync(saleOrderDtos);
                totalTransportTime += transportTimeWatcher.Elapsed;
                transportTimeWatcher.Reset();
                //}

                logger
                  .LogInformation($"----dataflow bulk delete {orderCount} orders,cost time:\"{executionTimeWatcher.Elapsed}\",transport time:{ totalTransportTime },count/time(sec):{Math.Ceiling(orderCount / totalTransportTime.TotalSeconds)},now:\"{DateTime.Now.TimeOfDay}\"----");

                signals?.Signal();
            }
            catch (Exception ex)
            {
                logger.LogError($"Error while dataflow bulk delete orders of {nameof(DataflowPipeBulkInsertLoopCoreAsync)}: {ex.Message}");
            }
        }
        #endregion
    }
}
