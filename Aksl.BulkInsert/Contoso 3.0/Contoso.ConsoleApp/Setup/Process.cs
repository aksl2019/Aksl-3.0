using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using Contoso.DataSource.Dtos;
using System.Linq;
using System.Threading;

namespace Contoso.ConsoleApp
{
    public partial class WebApiSender
    {
        #region Process Dataflow Methods
        private async Task ProcessDataflowOrdersAsync(IEnumerable<OrderDto> orderDtos)
        {
            if (orderDtos.Count() > 0)
            {
                using (await _mutex.LockAsync())
                {
                    //int index = 0;
                    int ordersCount = 0;
                    var executionTime = Stopwatch.StartNew();

                    try
                    {
                        foreach (var order in orderDtos)
                        {
                            Debug.Assert(order.Id > 0);

                            Interlocked.Increment(ref ordersCount);
                            Interlocked.Increment(ref _totalCount);
                          //  index++;
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Error while {nameof(ProcessDataflowOrdersAsync)} processing orders: {ex.ToString()}");
                    }

                    //_logger
                    //   .LogInformation($"------- TotalOrderCount={_totalCount},CurrentOrdersCount={ ordersCount},ExecutionTime:\"{executionTime.Elapsed}\", Now:\"{DateTime.Now.TimeOfDay}\"-------");
                }
            }
        }
        #endregion

        #region Dataflow Pipe Process Method
        public async Task ProcessDataflowPipeOrdersAsync(IEnumerable<OrderDto> orderDtos)
        {
            int ordersCount = 0;
            var executionTime = Stopwatch.StartNew();

            using (await _mutexRead.LockAsync())
            {
                try
                {
                    foreach (var order in orderDtos)
                    {
                        Debug.Assert(order.Id > 0);

                        Interlocked.Increment(ref ordersCount);
                        Interlocked.Increment(ref _totalCount);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error while {nameof(ProcessDataflowPipeOrdersAsync)} processing orders: {ex.ToString()}");
                }

                //_logger
                //   .LogInformation($"------- TotalOrderCount={_totalCount},CurrentOrdersCount={ ordersCount},ExecutionTime:\"{executionTime.Elapsed}\", Now:\"{DateTime.Now.TimeOfDay}\"-------");
            }
        }
        #endregion

        #region Pipe Process Method
        public async Task ProcessPipeOrdersAsync(IEnumerable<OrderDto> orderDtos)
        {
            int ordersCount = 0;
            var executionTime = Stopwatch.StartNew();

            using (await _mutexRead.LockAsync())
            {
                try
                {
                    foreach (var order in orderDtos)
                    {
                        Debug.Assert(order.Id > 0);

                        Interlocked.Increment(ref ordersCount);
                        Interlocked.Increment(ref _totalCount);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error while {nameof(ProcessDataflowPipeOrdersAsync)} processing orders: {ex.ToString()}");
                }

                //_logger
                //   .LogInformation($"------- TotalOrderCount={_totalCount},CurrentOrdersCount={ ordersCount},ExecutionTime:\"{executionTime.Elapsed}\", Now:\"{DateTime.Now.TimeOfDay}\"-------");
            }
        }
        #endregion
    }
}
