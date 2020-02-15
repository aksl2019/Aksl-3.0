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

namespace WebSocketServer.ConsoleApp
{
    public partial class WebSocketListener
    {
        #region Read Methods
        public async ValueTask GetAllPagedSaleOrdersAsync()
        {
            int pageIndex = 0;
            int pageSize = 10_000;
            int totalCount = 0;

            var logger = _loggerFactory.CreateLogger($"{ nameof(GetAllPagedSaleOrdersAsync)}");

            try
            {
                var executionTimeWatcher = Stopwatch.StartNew();

                var contosoDataSourceFactory = this.ServiceProvider.GetRequiredService<IDataSourceFactory<IContosoDataSource>>();
                var orderDataSource = contosoDataSourceFactory.Current.OrderDataSource;

                logger.LogInformation($"----begin read sale orders,  pageSize:{pageSize},now:{DateTime.Now.TimeOfDay}----");

                while (true)
                {

                   // logger.LogInformation($"---- pageIndex:{ pageIndex},now:{DateTime.Now.TimeOfDay}----");

                    var pagedOrderDtos = orderDataSource.GetPagedSaleOrderListAsync(pageIndex, pageSize);

                    int currentCount = 0;
                    await foreach (var order in pagedOrderDtos)
                    {
                        totalCount++; 
                        currentCount++;
                        //Console.WriteLine($"OrderId: {order.Id},OrderStatus: {order.Status}");
                        //Console.WriteLine($"OrderId: {order.Id},OrderNumber: {order.OrderNumber},OrderStatus: {order.Status},Customer: {order.CustomerId}");
                    }

                    if (currentCount <= 0)
                    {
                        //_cancellationTokenSource.CancelAfter(TimeSpan.FromMilliseconds(200));
                        break;
                    }

                    pageIndex++;
                }

                logger
                    .LogInformation($"----read { totalCount} sale orders,cost time:\"{executionTimeWatcher.Elapsed}\",count/time(sec):{Math.Ceiling(totalCount / executionTimeWatcher.Elapsed.TotalSeconds)},now:\"{DateTime.Now.TimeOfDay}\"----");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}\n");
            }
        }
        #endregion
    }
}
