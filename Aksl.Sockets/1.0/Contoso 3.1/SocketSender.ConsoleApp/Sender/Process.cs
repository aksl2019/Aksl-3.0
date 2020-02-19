using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using Contoso.DataSource.Dtos;

namespace Socket.Sender
{
    public partial class SocketSender
    {
        #region DataflowPipe Json Methods
        public async Task<bool> ProcessJsonSegmentBytesDataflowPipeAsync(IEnumerable<byte[]> messages, CancellationToken cancellationToken = default, ILogger logger = null)
        {
            logger = logger ?? _logger;

            //if (messages.Count() > 0)
            if ((messages?.Any()).HasValue)
            {
                using (await _mutex.LockAsync())
                {
                    int index = 0;
                    int ordersCount = 0;
                    int maxPoNumber = 0;
                    var orders = new List<SaleOrderDto>();
                    string orderString = default;
                    SaleOrderDto order = default;
                    byte[] orderByte = default;
                    var executionTime = Stopwatch.StartNew();
                    int posion = 0;

                    foreach (var msg in messages)
                    {
                        try
                        {
                            orderByte = msg;
                            orderString = Encoding.UTF8.GetString(msg);

                            index = 0;
                            posion = msg.ToList().FindIndex(c => c == (byte)'\n');

                            var saleOrders = orderString.Split('\n');
                            Debug.Assert(saleOrders[saleOrders.Count() - 1].Length == 0);
                            for (int i = 0; i < saleOrders.Count() - 1; i++)
                            {
                                order = JsonSerializer.Deserialize<SaleOrderDto>(saleOrders[i]);
                                maxPoNumber = Math.Max(maxPoNumber, order.OrderNumber);

                                Debug.Assert(order.Status == OrderStatus.Shipped);

                                Interlocked.Increment(ref ordersCount);
                                Interlocked.Increment(ref _totalCount);
                                index++;
                                //Console.WriteLine($"Json Order PoNumber：{order.PoNumber},Order Status：{order.Status}");
                            }

                            // ordersCount += index;
                        }
                        catch (Exception ex)
                        {
                            logger?.LogError($"Error while process a order: {ex.Message} in { nameof(SocketSender)}.{ nameof(ProcessJsonSegmentBytesDataflowPipeAsync)}");
                        }
                    }

                    logger?
                        .LogInformation($"-------Json Process Orders TotalCount:{_totalCount}--- MaxPoNumber:{ maxPoNumber}--OrdersCount:{ ordersCount}---now:\"{DateTime.Now.TimeOfDay}\",cost time:\"{executionTime.Elapsed}\"-------");
                }
            }

            return true;
        }

        public async Task<bool> ProcessJsonSegmentBytesDataflowPipeTaskAsync(IEnumerable<byte[]> messages, CancellationToken cancellationToken = default, ILogger logger = null)
        {
            logger = logger ?? _logger;

            if ((messages?.Any()).HasValue)
            {
                using (await _mutex.LockAsync())
                {
                    int index = 0;
                    int ordersCount = 0;
                    int maxPoNumber = 0;
                    var orders = new List<SaleOrderDto>();
                    string orderString = default;
                    SaleOrderDto order = default;
                    byte[] orderByte = default;
                    var executionTime = Stopwatch.StartNew();
                    int posion = 0;

                    foreach (var msg in messages)
                    {
                        try
                        {
                            orderByte = msg;
                            orderString = Encoding.UTF8.GetString(msg);

                            index = 0;
                            posion = msg.ToList().FindIndex(c => c == (byte)'\n');

                            var saleOrders = orderString.Split('\n');
                            Debug.Assert(saleOrders[saleOrders.Count() - 1].Length == 0);
                            for (int i = 0; i < saleOrders.Count() - 1; i++)
                            {
                                order = JsonSerializer.Deserialize<SaleOrderDto>(saleOrders[i]);
                                maxPoNumber = Math.Max(maxPoNumber, order.OrderNumber);

                                //Debug.Assert(_totalCount == maxPoNumber);
                                Debug.Assert(order.Status == OrderStatus.Shipped);

                                Interlocked.Increment(ref ordersCount);
                                Interlocked.Increment(ref _totalCount);
                                index++;
                                //Console.WriteLine($"Json Order PoNumber：{order.PoNumber},Order Status：{order.Status}");
                            }

                            // ordersCount += index;
                        }
                        catch (Exception ex)
                        {
                            logger?.LogError($"Error while process a order: {ex.Message} in { nameof(SocketSender)}.{ nameof(ProcessJsonSegmentBytesDataflowPipeTaskAsync)}");
                        }
                    }

                    logger?
                        .LogInformation($"-------Json Process Orders TotalCount:{_totalCount}--- MaxPoNumber:{ maxPoNumber}--OrdersCount:{ ordersCount}---now:\"{DateTime.Now.TimeOfDay}\",cost time:\"{executionTime.Elapsed}\"-------");
                }
            }

            return true;
        }
        #endregion

        #region DataflowDuplexPipe Json Methods
        public async Task<bool> ProcessJsonBytesDataflowDuplexPipeAsync(IEnumerable<byte[]> messages, CancellationToken cancellationToken = default, ILogger logger = null)
        {
            logger = logger ?? _logger;

            if ((messages?.Any()).HasValue)
            {
                using (await _mutex.LockAsync())
                {
                    int index = 0;
                    int ordersCount = 0;
                    int maxPoNumber = 0;
                    var orders = new List<SaleOrderDto>();
                    string orderString = default;
                    SaleOrderDto order = default;
                    byte[] orderByte = default;
                    var executionTime = Stopwatch.StartNew();

                    foreach (var msg in messages)
                    {
                        try
                        {
                            orderByte = msg;
                            orderString = Encoding.UTF8.GetString(msg);

                            order = JsonSerializer.Deserialize<SaleOrderDto>(orderString);
                            maxPoNumber = Math.Max(maxPoNumber, order.OrderNumber);
                            Debug.Assert(order.Status == OrderStatus.Shipped);

                            // Debug.Assert(_totalCount == maxPoNumber);
                            //  Console.WriteLine($"Json Order PoNumber：{order.PoNumber},Order Status：{order.Status}");

                            Interlocked.Increment(ref ordersCount);
                            Interlocked.Increment(ref _totalCount);
                            index++;
                        }
                        catch (Exception ex)
                        {
                            logger.LogError($"Error while process a order: {ex.Message} in { nameof(SocketSender)}.{ nameof(ProcessJsonBytesDataflowDuplexPipeAsync)}");
                        }
                    }

                    logger
                       .LogInformation($"-------Json Process Orders TotalCount:{_totalCount}--- MaxPoNumber:{ maxPoNumber}--OrdersCount:{ ordersCount}---now:\"{DateTime.Now.TimeOfDay}\",cost time:\"{executionTime.Elapsed}\"-------");
                }
            }

            return true;
        }

        public async Task<bool> ProcessJsonBytesDataflowDuplexPipeTaskAsync(IEnumerable<byte[]> messages, CancellationToken cancellationToken = default, ILogger logger = null)
        {
            logger = logger ?? _logger;

            if ((messages?.Any()).HasValue)
            {
                using (await _mutex.LockAsync())
                {
                    int index = 0;
                    int ordersCount = 0;
                    int maxPoNumber = 0;
                    var orders = new List<SaleOrderDto>();
                    string orderString = default;
                    SaleOrderDto order = default;
                    byte[] orderByte = default;
                    var executionTime = Stopwatch.StartNew();

                    foreach (var msg in messages)
                    {
                        try
                        {
                            orderByte = msg;
                            orderString = Encoding.UTF8.GetString(msg);

                            order = JsonSerializer.Deserialize<SaleOrderDto>(orderString);
                            maxPoNumber = Math.Max(maxPoNumber, order.OrderNumber);
                            Debug.Assert(order.Status == OrderStatus.Shipped);

                            // Debug.Assert(_totalCount == maxPoNumber);
                            //  Console.WriteLine($"Json Order PoNumber：{order.PoNumber},Order Status：{order.Status}");

                            Interlocked.Increment(ref ordersCount);
                            Interlocked.Increment(ref _totalCount);
                            index++;
                        }
                        catch (Exception ex)
                        {
                            logger.LogError($"Error while process a order: {ex.Message} in { nameof(SocketSender)}.{ nameof(ProcessJsonBytesDataflowDuplexPipeTaskAsync)}");
                        }
                    }

                    logger
                       .LogInformation($"-------Json Process Orders TotalCount:{_totalCount}--- MaxPoNumber:{ maxPoNumber}--OrdersCount:{ ordersCount}---now:\"{DateTime.Now.TimeOfDay}\",cost time:\"{executionTime.Elapsed}\"-------");
                }
            }

            return true;
        }
        #endregion

        #region Pipe  Byte Methods
        public async ValueTask ProcessJsonBytesSegmentPipeAsync(IEnumerable<byte[]> buffers, CancellationToken cancellationToken = default)
        {
            await  ProcessJsonBytesSegmentPipeAsync(buffers,  cancellationToken , _logger );
            //if ((buffers?.Any()).HasValue)
            //{
            //    using (await _mutex.LockAsync())
            //    {
            //        int index = 0;
            //        int ordersCount = 0;
            //        int maxPoNumber = 0;
            //        string orderString = default;
            //        SaleOrderDto order = default;
            //        byte[] orderBytes = default;
            //        var executionTime = Stopwatch.StartNew();
            //        int posion = 0;

            //        foreach (var buf in buffers)
            //        {
            //            orderBytes = buf;
            //            orderString = Encoding.UTF8.GetString(buf);

            //            index = 0;
            //            posion = buf.ToList().FindIndex(c => c == (byte)'\n');

            //            var saleOrders = orderString.Split('\n');
            //            Debug.Assert(saleOrders[saleOrders.Count() - 1].Length == 0);
            //            for (int i = 0; i < saleOrders.Count() - 1; i++)
            //            {
            //                try
            //                {
            //                    order = JsonSerializer.Deserialize<SaleOrderDto>(saleOrders[i]);
            //                    maxPoNumber = Math.Max(maxPoNumber, order.OrderNumber);

            //                    Debug.Assert(order.Status == OrderStatus.Shipped);

            //                    Interlocked.Increment(ref ordersCount);
            //                    Interlocked.Increment(ref _totalCount);
            //                    index++;
            //                }
            //                catch (Exception ex)
            //                {
            //                    _logger.LogError($"Error while process a order: {ex.Message}");
            //                }
            //            }
            //        }

            //        _logger.LogInformation($"-------Json Process Orders TotalCount:{_totalCount}--- MaxPoNumber:{ maxPoNumber}--OrdersCount:{ ordersCount}---now:\"{DateTime.Now.TimeOfDay}\",cost time:\"{executionTime.Elapsed}\"-------");
            //    }
            //}
        }

        public async ValueTask ProcessJsonBytesSegmentPipeAsync(IEnumerable<byte[]> buffers, CancellationToken cancellationToken = default, ILogger logger=null)
        {
            logger = logger ?? _logger;

            if ((buffers?.Any()).HasValue)
            {
                using (await _mutex.LockAsync())
                {
                    int index = 0;
                    int ordersCount = 0;
                    int maxPoNumber = 0;
                    var orders = new List<SaleOrderDto>();
                    string orderString = default;
                    SaleOrderDto order = default;
                    byte[] orderByte = default;
                    var executionTime = Stopwatch.StartNew();
                    int posion = 0;

                    foreach (var buf in buffers)
                    {
                        //string orderUTF8String = Encoding.UTF8.GetString(b);
                        orderByte = buf;
                        orderString = Encoding.UTF8.GetString(buf);

                        index = 0;
                        posion = buf.ToList().FindIndex(c => c == (byte)'\n');

                        var saleOrders = orderString.Split('\n');
                        Debug.Assert(saleOrders[saleOrders.Count() - 1].Length == 0);
                        for (int i = 0; i < saleOrders.Count() - 1; i++)
                        {
                            try
                            {
                                order = JsonSerializer.Deserialize<SaleOrderDto>(saleOrders[i]);
                                maxPoNumber = Math.Max(maxPoNumber, order.OrderNumber);

                                Debug.Assert(order.Status == OrderStatus.Shipped);

                                Interlocked.Increment(ref ordersCount);
                                Interlocked.Increment(ref _totalCount);
                                index++;
                            }
                            catch (Exception ex)
                            {
                                logger?.LogError($"Error while process a order: {ex.Message} in { nameof(SocketSender)}.{ nameof(ProcessJsonBytesSegmentPipeAsync)}");
                            }
                        }
                    }

                    logger?.LogInformation($"-------Json Process Orders TotalCount:{_totalCount}--- MaxPoNumber:{ maxPoNumber}--OrdersCount:{ ordersCount}---now:\"{DateTime.Now.TimeOfDay}\",cost time:\"{executionTime.Elapsed}\"-------");
                }
            }
        }

        public async ValueTask ProcessJsonBytesSegmentPipeTaskAsync(IEnumerable<byte[]> buffers, CancellationToken cancellationToken = default, ILogger logger = null)
        {
            await ProcessJsonBytesSegmentPipeAsync(buffers, cancellationToken, logger);

            //logger = logger ?? _logger;
            //if ((buffers?.Any()).HasValue)
            //{
            //    using (await _mutex.LockAsync())
            //    {
            //        int ordersCount = 0;
            //        int maxPoNumber = 0;
            //        string orderString = default;
            //        SaleOrderDto order = default;
            //        var executionTime = Stopwatch.StartNew();

            //        foreach (var b in buffers)
            //        {
            //            string orderUTF8String = Encoding.UTF8.GetString(b);

            //            var saleOrders = orderUTF8String.Split('\n');
            //            Debug.Assert(saleOrders[saleOrders.Count() - 1].Length == 0);
            //            for (int i = 0; i < saleOrders.Count() - 1; i++)
            //            {
            //                try
            //                {
            //                    orderString = saleOrders[i];
            //                    order = JsonSerializer.Deserialize<SaleOrderDto>(orderString);
            //                    maxPoNumber = Math.Max(maxPoNumber, order.OrderNumber);

            //                    Debug.Assert(order.Status == OrderStatus.Shipped);

            //                    Interlocked.Increment(ref ordersCount);
            //                    Interlocked.Increment(ref _totalCount);
            //                }
            //                catch (Exception ex)
            //                {
            //                    logger?.LogError($"Error while process a order: {ex.Message} in { nameof(ProcessJsonBytesSegmentPipeTaskAsync)}");
            //                }
            //            }
            //        }

            //        logger.LogInformation($"-------Json Process Orders TotalCount:{_totalCount}, MaxPoNumber:{ maxPoNumber},OrdersCount:{ ordersCount},now:\"{DateTime.Now.TimeOfDay}\",cost time:\"{executionTime.Elapsed}\",ThreadId:{Thread.CurrentThread.ManagedThreadId}-------");
            //    }
            //}
        }
        #endregion

        #region DuplexPipe Json Byte Methods
        public async Task<bool> ProcessJsonBytesDuplexPipeAsync(IEnumerable<byte[]> messages, CancellationToken cancellationToken)
        {
            if (messages.Count() > 0)
            {
                using (await _mutex.LockAsync())
                {
                    int ordersCount = 0;
                    int maxPoNumber = 0;
                    var orders = new List<SaleOrderDto>();
                    string orderString = default;
                    SaleOrderDto order = default;
                    byte[] orderByte = default;
                    var executionTime = Stopwatch.StartNew();

                    foreach (var msg in messages)
                    {
                        try
                        {
                            orderByte = msg;
                            orderString = Encoding.UTF8.GetString(msg);

                            order = JsonSerializer.Deserialize<SaleOrderDto>(orderString);
                            maxPoNumber = Math.Max(maxPoNumber, order.OrderNumber);

                            Debug.Assert(order.Status == OrderStatus.Shipped);

                            // Debug.Assert(_totalCount == maxPoNumber);

                            Interlocked.Increment(ref ordersCount);
                            Interlocked.Increment(ref _totalCount);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError($"Error while process a order: {ex.Message}");
                        }

                    }
                    _logger.LogInformation($"-------Json Process Orders TotalCount:{_totalCount}--- MaxPoNumber:{ maxPoNumber}--OrdersCount:{ ordersCount}---now:\"{DateTime.Now.TimeOfDay}\",cost time:\"{executionTime.Elapsed}\"-------");
                }
            }

            return true;
        }

        public async Task<bool> ProcessJsonBytesDuplexPipeAsync(IEnumerable<byte[]> messages, CancellationToken cancellationToken = default, ILogger logger = null)
        {
            if (messages.Count() > 0)
            {
                using (await _mutex.LockAsync())
                {
                    int ordersCount = 0;
                    int maxPoNumber = 0;
                    var orders = new List<SaleOrderDto>();
                    string orderString = default;
                    SaleOrderDto order = default;
                    byte[] orderByte = default;
                    var executionTime = Stopwatch.StartNew();

                    foreach (var msg in messages)
                    {
                        try
                        {
                            orderByte = msg;
                            orderString = Encoding.UTF8.GetString(msg);

                            order = JsonSerializer.Deserialize<SaleOrderDto>(orderString);
                            maxPoNumber = Math.Max(maxPoNumber, order.OrderNumber);

                            Debug.Assert(order.Status == OrderStatus.Shipped);

                            // Debug.Assert(_totalCount == maxPoNumber);

                            Interlocked.Increment(ref ordersCount);
                            Interlocked.Increment(ref _totalCount);
                        }
                        catch (Exception ex)
                        {
                            logger?.LogError($"Error while process a order: {ex.Message}");
                        }
                    }

                    logger?.LogInformation($"-------Json Process Orders TotalCount:{_totalCount}--- MaxPoNumber:{ maxPoNumber}--OrdersCount:{ ordersCount}---now:\"{DateTime.Now.TimeOfDay}\",cost time:\"{executionTime.Elapsed}\"-------");
                }
            }

            return true;
        }

        public async Task<bool> ProcessJsonBytesDuplexPipeConnectionsAsync(IEnumerable<byte[]> messages, CancellationToken cancellationToken)
        {
            if (messages.Count() > 0)
            {
                using (await _mutex.LockAsync())
                {
                    int ordersCount = 0;
                    int maxPoNumber = 0;
                    var orders = new List<SaleOrderDto>();
                    string orderString = default;
                    SaleOrderDto order = default;
                    byte[] orderByte = default;
                    var executionTime = Stopwatch.StartNew();

                    foreach (var msg in messages)
                    {
                        try
                        {
                            orderByte = msg;
                            orderString = Encoding.UTF8.GetString(msg);

                            order = JsonSerializer.Deserialize<SaleOrderDto>(orderString);
                            maxPoNumber = Math.Max(maxPoNumber, order.OrderNumber);

                            Debug.Assert(order.Status == OrderStatus.Shipped);

                            // Debug.Assert(_totalCount == maxPoNumber);

                            Interlocked.Increment(ref ordersCount);
                            Interlocked.Increment(ref _totalCount);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError($"Error while process a order: {ex.Message}");
                        }

                    }
                   // _logger.LogInformation($"-------Json Process Orders TotalCount:{_totalCount}--- MaxPoNumber:{ maxPoNumber}--OrdersCount:{ ordersCount}---now:\"{DateTime.Now.TimeOfDay}\",cost time:\"{executionTime.Elapsed}\"-------");
                }
            }

            return true;
        }

        public async Task<bool> ProcessJsonBytesDuplexPipeTaskAsync(IEnumerable<byte[]> messages, CancellationToken cancellationToken)
        {
            if (messages.Count() > 0)
            {
                using (await _mutex.LockAsync())
                {
                    int ordersCount = 0;
                    int maxPoNumber = 0;
                    var orders = new List<SaleOrderDto>();
                    string orderString = default;
                    SaleOrderDto order = default;
                    byte[] orderByte = default;
                    var executionTime = Stopwatch.StartNew();

                    foreach (var msg in messages)
                    {
                        try
                        {
                            orderByte = msg;
                            orderString = Encoding.UTF8.GetString(msg);

                            order = JsonSerializer.Deserialize<SaleOrderDto>(orderString);
                            maxPoNumber = Math.Max(maxPoNumber, order.OrderNumber);

                            Debug.Assert(order.Status == OrderStatus.Shipped);

                            // Debug.Assert(_totalCount == maxPoNumber);

                            Interlocked.Increment(ref ordersCount);
                            Interlocked.Increment(ref _totalCount);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError($"Error while process a order: {ex.Message}");
                        }
                    }

                    _logger.LogInformation($"-------Json Process Orders TotalCount:{_totalCount}, MaxPoNumber:{ maxPoNumber},OrdersCount:{ ordersCount},now:\"{DateTime.Now.TimeOfDay}\",cost time:\"{executionTime.Elapsed}\",ThreadId:{Thread.CurrentThread.ManagedThreadId}-------");
                }
            }

            return true;
        }
        #endregion

        #region DuplexPipe Json String Methods
        public async ValueTask ProcessJsonStringsDuplexPipeAsync(IEnumerable<string> messages, CancellationToken cancellationToken)
        {
            if (messages.Count() > 0)
            {
                using (await _mutex.LockAsync())
                {
                    int ordersCount = 0;
                    int maxPoNumber = 0;
                    var orders = new List<SaleOrderDto>();
                    string orderString = default;
                    SaleOrderDto order = default;
                    // byte[] orderByte = default;
                    var executionTime = Stopwatch.StartNew();

                    foreach (var msg in messages)
                    {
                        try
                        {
                            orderString = msg;

                            order = JsonSerializer.Deserialize<SaleOrderDto>(orderString);
                            maxPoNumber = Math.Max(maxPoNumber, order.OrderNumber);

                            Debug.Assert(order.Status == OrderStatus.Shipped);

                            // Debug.Assert(_totalCount == maxPoNumber);

                            Interlocked.Increment(ref ordersCount);
                            Interlocked.Increment(ref _totalCount);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError($"Error while process a order: {ex.Message}");
                        }
                    }
                    _logger.LogInformation($"-------Json Process Orders TotalCount:{_totalCount}--- MaxPoNumber:{ maxPoNumber}--OrdersCount:{ ordersCount}---now:\"{DateTime.Now.TimeOfDay}\",cost time:\"{executionTime.Elapsed}\"-------");
                }
            }
        }

        public async ValueTask ProcessJsonStringsDuplexPipeAsync(IEnumerable<string> messages, CancellationToken cancellationToken = default, ILogger logger = null)
        {
            if (messages.Count() > 0)
            {
                using (await _mutex.LockAsync())
                {
                    int ordersCount = 0;
                    int maxPoNumber = 0;
                    var orders = new List<SaleOrderDto>();
                    string orderString = default;
                    SaleOrderDto order = default;
                    // byte[] orderByte = default;
                    var executionTime = Stopwatch.StartNew();

                    foreach (var msg in messages)
                    {
                        try
                        {
                            orderString = msg;

                            order = JsonSerializer.Deserialize<SaleOrderDto>(orderString);
                            maxPoNumber = Math.Max(maxPoNumber, order.OrderNumber);

                            Debug.Assert(order.Status == OrderStatus.Shipped);

                            // Debug.Assert(_totalCount == maxPoNumber);

                            Interlocked.Increment(ref ordersCount);
                            Interlocked.Increment(ref _totalCount);
                        }
                        catch (Exception ex)
                        {
                            logger?.LogError($"Error while process a order: {ex.Message}");
                        }
                    }

                    logger?.LogInformation($"-------Json Process Orders TotalCount:{_totalCount}--- MaxPoNumber:{ maxPoNumber}--OrdersCount:{ ordersCount}---now:\"{DateTime.Now.TimeOfDay}\",cost time:\"{executionTime.Elapsed}\"-------");
                }
            }
        }

        public async ValueTask ProcessJsonStringsDuplexPipeTaskAsync(IEnumerable<string> messages, CancellationToken cancellationToken=default)
        {
            if (messages.Count() > 0)
            {
                using (await _mutex.LockAsync())
                {
                    int ordersCount = 0;
                    int maxPoNumber = 0;
                    var orders = new List<SaleOrderDto>();
                    string orderString = default;
                    SaleOrderDto order = default;
                    // byte[] orderByte = default;
                    var executionTime = Stopwatch.StartNew();

                    foreach (var msg in messages)
                    {
                        try
                        {
                            // orderByte = Encoding.UTF8.GetBytes(msg);
                            orderString = msg;

                            order = JsonSerializer.Deserialize<SaleOrderDto>(orderString);
                            maxPoNumber = Math.Max(maxPoNumber, order.OrderNumber);

                            Debug.Assert(order.Status == OrderStatus.Shipped);

                            // Debug.Assert(_totalCount == maxPoNumber);

                            Interlocked.Increment(ref ordersCount);
                            Interlocked.Increment(ref _totalCount);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError($"Error while process a order: {ex.Message}");
                        }
                    }

                    _logger.LogInformation($"-------Json Process Orders TotalCount:{_totalCount}--- MaxPoNumber:{ maxPoNumber}--OrdersCount:{ ordersCount}---now:\"{DateTime.Now.TimeOfDay}\",cost time:\"{executionTime.Elapsed}\",ThreadId:{Thread.CurrentThread.ManagedThreadId}-------");
                }
            }
        }
        #endregion

        #region Process Methods
        //public async Task<bool> ProcesProtoBufBytesAsync(IEnumerable<byte[]> messages, CancellationToken cancellationToken)
        //{
        //    if (messages.Count() > 0)
        //    {
        //        using (await _mutex.LockAsync())
        //        {
        //            var orders = new List<SaleOrderDto>();
        //            string orderString = default;
        //            SaleOrderDto order = default;
        //            byte[] orderByte = default;
        //            int index = 0;
        //            int maxPoNumber = 0;
        //            foreach (var msg in messages)
        //            {
        //                try
        //                {
        //                    orderByte = msg;
        //                    orderString = Encoding.UTF8.GetString(msg);

        //                    if (orderByte[orderByte.Length] == (byte)'\n')
        //                    {
        //                        var orderUTF8Bytes = new byte[orderByte.Length];
        //                        Array.Copy(orderByte, orderUTF8Bytes, orderByte.Length - 1);

        //                        order = OrderProtoBufSerializer.DeserializeOrder(orderUTF8Bytes);
        //                        maxPoNumber = Math.Max(maxPoNumber, order.PoNumber);

        //                        Console.WriteLine($"ProtoBuf Order PoNumber：{order.PoNumber},Order Status：{order.Status}");

        //                        index++;
        //                        _totalCount++;
        //                    }
        //                }
        //                catch (Exception ex)
        //                {
        //                    _logger.LogError($"Error while process a order: {ex.Message},Posion:{index}");
        //                }
        //            }


        //            // Debug.Assert(messages.Count() == index);
        //            _logger.LogInformation($"-----------------ProtoBuf Process Orders TotalCount:{_totalCount}--- MaxPoNumber:{ maxPoNumber}---ThreadId:{Thread.CurrentThread.ManagedThreadId}-----------------");
        //        }
        //    }

        //    return await Task.FromResult<bool>(true);
        //}

        public async Task<bool> ProcesJsonBytesAsync(IEnumerable<byte[]> messages, CancellationToken cancellationToken)
        {
            if (messages.Count() > 0)
            {
                using (await _mutex.LockAsync())
                {
                    int index = 0;
                    int maxPoNumber = 0;
                    var orders = new List<SaleOrderDto>();
                    string orderString = default;
                    SaleOrderDto order = default;
                    byte[] orderByte = default;

                    foreach (var msg in messages)
                    {
                        try
                        {
                            orderByte = msg;
                            orderString = Encoding.UTF8.GetString(msg);
                            order = JsonSerializer.Deserialize<SaleOrderDto>(orderString);

                            index++;

                            maxPoNumber = Math.Max(maxPoNumber, order.OrderNumber);
                            Console.WriteLine($"Json Order PoNumber：{order.OrderNumber},Order Status：{order.Status}");

                            _totalCount++;
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError($"Error while process a order: {ex.Message}");

                        }
                    }

                    Debug.Assert(messages.Count() == index);
                    _logger.LogInformation($"-----------------Json Process Orders TotalCount:{_totalCount}--- MaxPoNumber:{ maxPoNumber}---ThreadId:{Thread.CurrentThread.ManagedThreadId}-----------------");
                }
            }

            return true;
        }

        //public async Task ProcesBytesAsync(List<IEnumerable<byte[]>> messages, CancellationToken cancellationToken)
        //{
        //    if (messages.Count() > 0)
        //    {
        //        using (await _mutex.LockAsync())
        //        {
        //            foreach (var msg in messages)
        //            {
        //                try
        //                {
        //                    var orders = OrderJsonSerializer.DeserializeOrders(msg);

        //                    foreach (var od in orders)
        //                    {
        //                        Console.WriteLine($"Order Status：{ od.Status}");
        //                    }

        //                    _totalCount += orders.Count();

        //                    _logger.LogInformation($"-------------------Process Orders TotalCount:{_totalCount}---ThreadId:{Thread.CurrentThread.ManagedThreadId}-------------------------");
        //                }
        //                catch (Exception ex)
        //                {
        //                    _logger.LogError($"Error while process a order: {ex.Message}");
        //                }
        //            }
        //        }
        //    }
        //}
        #endregion
    }
}
