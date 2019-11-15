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
        private System.Threading.Timer _timer;
        private double _frequency;
        private TimeSpan _period;
        private int _maxRetryCount = 200;
        private int _totalOrderCount = 0;
        private Stopwatch _transportTimeWatcher ;
        private TimeSpan _totalTransportTime ;
        private Stopwatch _executionTimeWatcher ;

        #region Batch Messages ThreadTimer
        public async Task StartDataflowBulkInsertBlockTimerAsync()
        {
            var logger = _loggerFactory.CreateLogger($"DataflowBulkInser-Timer:{_period.Milliseconds}-OrderCount:{ _totalOrderCount}");

            _timer.Change(TimeSpan.FromMilliseconds(100), _period);

            await Task.Delay(_period).ConfigureAwait(false);
        }

        public Task InitializeDataflowBulkInsertBlockTimerAsync(double frequency=2000d, int maxRetryCount = 10, int orderCount=1000)
        {
            if (!_isInitialize)
            {
                throw new InvalidOperationException("not initialize");
            }

            _frequency = frequency;
            _period = TimeSpan.FromMilliseconds(frequency);
            _maxRetryCount = maxRetryCount;
            _totalOrderCount = _maxRetryCount * orderCount;

            var logger = _loggerFactory.CreateLogger($"DataflowBulkInser-Timer:{_period.Milliseconds}-OrderCount:{ _totalOrderCount}");

            int currentRetryCount = 0;

            try
            {
                var contosoDataSourceFactory = this.ServiceProvider.GetRequiredService<IDataSourceFactory<IContosoDataSource>>();
                var orderDataSource = contosoDataSourceFactory.Current.OrderDataSource;

                _transportTimeWatcher = Stopwatch.StartNew();
                _totalTransportTime = TimeSpan.Zero;
                _executionTimeWatcher = Stopwatch.StartNew();

                int start = 0;
                //dueTime:调用callback之前延迟的时间量（以毫秒为单位），指定 Timeout.Infinite 以防止计时器开始计时。指定零 (0) 以立即启动计时器
                //period:定时的时间时隔，以毫秒为单位
                _timer = new Timer(async (state) =>
                {
                    var orders = OrderJsonProvider.CreateOrders(start, orderCount); 
                    start += orderCount;

                    _transportTimeWatcher.Restart();
                    var dbOrders = await orderDataSource.DataflowBulkInsertSaleOrdersAsync(orders);
                    _totalTransportTime += _transportTimeWatcher.Elapsed;
                    _transportTimeWatcher.Reset();

                    //currentRetryTime = transportTimeWatcher.Elapsed;
                    //if (currentRetryTime >= maxRetryTime)
                    //{
                    //    _timer.Change(Timeout.Infinite, Timeout.Infinite);
                    //}
                    currentRetryCount++;
                    if (currentRetryCount >= _maxRetryCount)
                    {
                        logger
                          .LogInformation($"----dataflow bulk insert {_totalOrderCount} orders,cost time:\"{_executionTimeWatcher.Elapsed}\",transport time:{ _totalTransportTime },count/time(sec):{Math.Ceiling(_totalOrderCount / _totalTransportTime.TotalSeconds)},now:\"{DateTime.Now.TimeOfDay}\"----");

                        _timer.Change(Timeout.Infinite, Timeout.Infinite);
                        _timer.Dispose();
                    }
                }, 
                this, Timeout.Infinite, Timeout.Infinite);
            }
            catch (Exception ex)
            {
                logger.LogError($"Error while dataflow bulk insert orders of {nameof(InitializeDataflowBulkInsertBlockTimerAsync)}: {ex.Message}");
            }

            return Task.CompletedTask;
        }
    
        #endregion
    }
}
