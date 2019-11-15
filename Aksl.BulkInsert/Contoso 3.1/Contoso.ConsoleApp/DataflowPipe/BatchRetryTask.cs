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
        public async Task DataflowPipeBulkInsertRetryTasksAsync(int seqNumber = 1, int frequency = 1000, int taskCount = 2, int count = 1, int orderCount = 100, int maxRetryCount = 2)
        {
            if (!_isInitialize)
            {
                throw new InvalidOperationException("not initialize");
            }

            long totalOrderCount = taskCount * count * orderCount * maxRetryCount;

            var logger = _loggerFactory.CreateLogger($"DataflowPipeInsert-{seqNumber}-OrderCount:{totalOrderCount}");

            int currentRetryCount = 0;

            try
            {
                TimeSpan period = TimeSpan.FromMilliseconds(frequency);

                var transportTimeWatcher = Stopwatch.StartNew();
                TimeSpan totalTransportTime = TimeSpan.Zero;
                var executionTimeWatcher = Stopwatch.StartNew();

                logger.LogInformation($"----begin dataflow pipe bulk insert { totalOrderCount } orders,now:{DateTime.Now.TimeOfDay}----");

                while (true)
                {
                    var signals = new AsyncCountdownEvent(taskCount);

                    if (_cancellationTokenSource.Token.IsCancellationRequested)
                    {
                        _cancellationTokenSource.Token.ThrowIfCancellationRequested();
                        // _logger?.LogCritical($"message was cancelled");
                        break;
                    }

                    transportTimeWatcher.Restart();

                    var insertTasks = from tc in Enumerable.Range(0, taskCount)
                                      select Task.Run(() =>
                                      {
                                          return DataflowPipeBulkInsertCoreAsync(signals, tc, count, orderCount);
                                      }, _cancellationTokenSource.Token);

                    await Task.WhenAll(insertTasks);

                    await signals.WaitAsync();

                    totalTransportTime += transportTimeWatcher.Elapsed;
                    transportTimeWatcher.Reset();

                    await Task.Delay(period);

                    currentRetryCount++;
                    if (currentRetryCount >= maxRetryCount)
                    {
                        //_cancellationTokenSource.CancelAfter(TimeSpan.FromMilliseconds(200));
                        break;
                    }
                }

                logger
                   .LogInformation($"----finish dataflow pipe bulk insert {totalOrderCount} orders,cost time:\"{executionTimeWatcher.Elapsed},count/time(sec):{Math.Ceiling(totalOrderCount / executionTimeWatcher.Elapsed.TotalSeconds)},now:\"{DateTime.Now.TimeOfDay}\"----");
            }
            catch (Exception ex)
            {
                logger.LogError($"Error while dataflow pipe bulk insert: {ex.Message}");
            }
        }

        public async Task DataflowPipeBulkInsertCoreAsync(AsyncCountdownEvent signals, int index, int count, int orderCount)
        {
            var logger = _loggerFactory.CreateLogger($"DataflowPipeBulkInserter-TaskCount:{index}");

            int totalOrderCount = count * orderCount;

            try
            {
                var dataflowPipeBulkInserter = this.ServiceProvider.GetRequiredService<IDataflowPipeBulkInserter<SaleOrderDto, SaleOrderDto>>();

                var contosoDataSourceFactory = this.ServiceProvider.GetRequiredService<IDataSourceFactory<IContosoDataSource>>();
                var orderDataSource = contosoDataSourceFactory.Current.OrderDataSource;

                // await Task.Delay(TimeSpan.FromMilliseconds(2000));

                var transportTimeWatcher = Stopwatch.StartNew();
                TimeSpan totalTransportTime = TimeSpan.Zero;
                var executionTimeWatcher = Stopwatch.StartNew();

               // logger.LogInformation($"----begin dataflow bulk insert { totalOrderCount} orders,now:{DateTime.Now.TimeOfDay}----");

                int start = 0;
                for (int i = 0; i < count; i++)
                {
                    var orders = OrderJsonProvider.CreateOrders(start, orderCount);
                    start += orderCount;

                    transportTimeWatcher.Restart();
                    var dbOrders = await orderDataSource.DataflowPipeBulkInsertSaleOrdersAsync(orders);
                    totalTransportTime += transportTimeWatcher.Elapsed;
                    transportTimeWatcher.Reset();

                    if (dbOrders?.Count() > 0)
                    {
                       //await ProcessDataflowPipeOrdersAsync(dbOrders);
                    }
                }

                //logger
                //  .LogInformation($"----dataflow bulk insert {totalOrderCount} orders,cost time:\"{executionTimeWatcher.Elapsed}\",transport time:{ totalTransportTime },count/time(sec):{Math.Ceiling(totalOrderCount / totalTransportTime.TotalSeconds)},now:\"{DateTime.Now.TimeOfDay}\"----");

                signals?.Signal();
            }
            catch (Exception ex)
            {
                logger.LogError($"Error while dataflow pipe bulk insert orders of {nameof(DataflowPipeBulkInsertCoreAsync)}: {ex.Message}");
            }
        }
        #endregion
    }
}
