using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using System.Text.Json;

using Microsoft.Extensions.Logging;

using Contoso.DataSource.Dtos;

namespace SocketServer.ConsoleApp
{
    public partial class SocketListener
    {
        #region Process Dataflow Methods
        public async Task<IEnumerable<byte[]>> ProcessJsonStringFromClientAsync(IEnumerable<string> messages, CancellationToken cancellationToken = default)
        {
            IList<byte[]> saleOrderByteList = new List<byte[]>(messages.Count());

            #region Methods
            if (messages.Count() > 0)
            {
                using (await _mutexRead.LockAsync())
                {
                    int index = 0;
                    int maxPoNumber = 0;
                    string orderString = default;
                    byte[] orderByte = default;
                    SaleOrderDto order = default;
                    var executionTime = Stopwatch.StartNew();
                    List<SaleOrderDto> saleOrderDtos = new List<SaleOrderDto>();

                    foreach (var msg in messages)
                    {
                        try
                        {
                            orderByte = Encoding.UTF8.GetBytes(msg);
                            orderString = msg;

                            order = System.Text.Json.JsonSerializer.Deserialize<SaleOrderDto>(msg);
                            order.Status = OrderStatus.Shipped;
                            maxPoNumber = Math.Max(maxPoNumber, order.OrderNumber);

                            //using (await _mutex.LockAsync())
                            //{
                            //    _queues.Enqueue(order);
                            //   
                            //}

                            saleOrderDtos.Add(order);

                            //var serializedOrderString = System.Text.Json.JsonSerializer.Serialize(order);
                            //var serializedOrderByte = Encoding.UTF8.GetBytes(serializedOrderString);
                            //var orderByteCount = Encoding.UTF8.GetByteCount(serializedOrderString);
                            //var orderUTF8Bytes = new byte[orderByteCount + 1];
                            //Encoding.UTF8.GetBytes(serializedOrderString, 0, serializedOrderString.Length, orderUTF8Bytes, 0);
                            //orderUTF8Bytes[orderUTF8Bytes.Length - 1] = (byte)'\n';
                            //orderByteList.Add(orderUTF8Bytes);

                            // var shippedOrder = Encoding.UTF8.GetString(orderUTF8Bytes);
                            // Debug.Assert(order.Status == OrderStatus.Shipped);

                            var orderUTF8Bytes = System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(order);
                            var length = orderUTF8Bytes.Length;
                            var orderUTF8BytesEx = new byte[length + 1];
                            // Encoding.UTF8.GetBytes(orderString, 0, orderString.Length, orderUTF8Bytes, 0);
                            Array.Copy(orderUTF8Bytes, orderUTF8BytesEx, length);
                            orderUTF8BytesEx[orderUTF8BytesEx.Length - 1] = (byte)'\n';
                            saleOrderByteList.Add(orderUTF8BytesEx);//这里

                            // Console.WriteLine($"Json Order PoNumber：{order.PoNumber},Order Status：{order.Status}");

                            Interlocked.Increment(ref index);
                            //Interlocked.Increment(ref _totalCount);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError($"Error while process a order: {ex.Message} in { nameof(SocketListener)}.{ nameof(ProcessJsonStringFromClientAsync)}");
                        }
                    }

                    if (saleOrderDtos.Count() > 0)
                    {
                        //await _dataflowProducerConsumer.ProduceAsync(new List<SaleOrderDto>(saleOrderDtos));
                        await _dataflowProducerConsumer.ProduceAsync(new List<SaleOrderDto>(saleOrderDtos))
                                                                          .ConfigureAwait(continueOnCapturedContext: false);
                        // _dataflowProducerConsumer.ProduceAsync(new List<SaleOrderDto>(saleOrderDtos)).GetAwaiter().GetResult();

                        // await _taskChannelProducerConsumer.ProduceAsync(new List<SaleOrderDto>(saleOrderDtos));
                        // await _taskChannelProducerConsumer.ProduceAsync(new List<SaleOrderDto>(saleOrderDtos))
                        // .ConfigureAwait(continueOnCapturedContext: false);
                        // _taskChannelProducerConsumer.ProduceAsync(new List<SaleOrderDto>(saleOrderDtos)).GetAwaiter().GetResult();
                    }

                    //if (saleOrderByteList.Count() > 0)
                    //{
                    //    await _pipeProducerConsumer.ProduceAsync(new List<byte[]>(saleOrderByteList))
                    //                                            .ConfigureAwait(continueOnCapturedContext: false);
                    //   //_pipeProducerConsumer.ProduceAsync(new List<byte[]>(saleOrderByteList)).GetAwaiter().GetResult();

                    //    //await _pipeProducerConsumerEx.ProduceAsync(new List<byte[]>(saleOrderByteList));
                    //}

                    //orderByteList = await SocketListener.Instance.DataflowBulkInsertBlockTasksAsync(saleOrderDtos.ToArray());
                    //_saleOrderDtoBuffer.Clear();

                    //Debug.Assert(messages.Count() == index);

                    // _logger.LogInformation($"-------Json Process Orders TotalCount:{_totalCount}--- MaxPoNumber:{ maxPoNumber}--OrdersCount:{ index}---now:\"{DateTime.Now.TimeOfDay}\",cost time:\"{executionTime.Elapsed}\"-------");
                }
            }

            return saleOrderByteList.AsEnumerable();
            #endregion
        }

        private async Task ProcessDataflowOrdersAsync(IEnumerable<SaleOrderDto> saleOrderDtos)
        {
            if (saleOrderDtos.Count() > 0)
            {
                using (await _mutex.LockAsync())
                {
                    //int index = 0;
                    int ordersCount = 0;
                    var executionTime = Stopwatch.StartNew();

                    try
                    {
                        foreach (var saleOrder in saleOrderDtos)
                        {
                            Debug.Assert(saleOrder.Id > 0);

                            Interlocked.Increment(ref ordersCount);
                            //Interlocked.Increment(ref _totalCount);
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
        public async Task ProcessDataflowPipeOrdersAsync(IEnumerable<SaleOrderDto> saleOrderDtos)
        {
            int ordersCount = 0;
            var executionTime = Stopwatch.StartNew();

            using (await _mutexRead.LockAsync())
            {
                try
                {
                    foreach (var saleOrder in saleOrderDtos)
                    {
                        Debug.Assert(saleOrder.Id > 0);

                        Interlocked.Increment(ref ordersCount);
                       // Interlocked.Increment(ref _totalCount);
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
        public async Task ProcessPipeOrdersAsync(IEnumerable<SaleOrderDto> saleOrderDtos)
        {
            int ordersCount = 0;
            var executionTime = Stopwatch.StartNew();

            using (await _mutexRead.LockAsync())
            {
                try
                {
                    foreach (var saleOrder in saleOrderDtos)
                    {
                        Debug.Assert(saleOrder.Id > 0);

                        Interlocked.Increment(ref ordersCount);
                        //Interlocked.Increment(ref _totalCount);
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
