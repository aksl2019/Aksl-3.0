using System;
using System.Collections.Generic;
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
using System.Diagnostics;

namespace Contoso.ConsoleApp
{
    public partial class WebApiSender
    {
        #region Read Methods
        public async Task GetPagedOrdersAsync()
        {
            int pageIndex = 0;
            int pageSize = int.MaxValue;


            var logger = _loggerFactory.CreateLogger($"{ nameof(GetPagedOrdersAsync)}");

            try
            {
                var executionTimeWatcher = Stopwatch.StartNew();

                var contosoDataSourceFactory = this.ServiceProvider.GetRequiredService<IDataSourceFactory<IContosoDataSource>>();
                var orderDataSource = contosoDataSourceFactory.Current.OrderDataSource;

                logger.LogInformation($"----begin read orders, pageIndex:{ pageIndex}, pageSize:{  pageSize},now:{DateTime.Now.TimeOfDay}----");

                var pagedOrderDtos = await orderDataSource.GetPagedOrdersAsync(pageIndex, pageSize);

                logger
                    .LogInformation($"----read {pagedOrderDtos.TotalCount} orders,cost time:\"{executionTimeWatcher.Elapsed}\",count/time(sec):{Math.Ceiling(pagedOrderDtos.TotalCount / executionTimeWatcher.Elapsed.TotalSeconds)},now:\"{DateTime.Now.TimeOfDay}\"----");

                foreach (var order in pagedOrderDtos)
                {
                    Console.WriteLine($"OrderId: {order.Id},OrderNumber: {order.OrderNumber},OrderStatus: {order.Status},Customer: {order.CustomerId}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}\n");
            }
        }
        #endregion
    }
}
