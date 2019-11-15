using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Newtonsoft.Json;

using Contoso.DataSource.Dtos;

namespace Contoso.ConsoleApp
{
    public static class OrderJsonProvider
    {
        private static decimal TaxRate = 0.34M;

        private static Random _random;
        private static List<int> _orderIds;
        private static List<int> _productIds;

        private static string[] ProductePrix = { "Blue", "Red", "Green", "White", "Orange" };

        private static readonly string[] OrderStates = { "Pending", "Processed", "Shipped" };

        private static readonly string[] Customers = { "Alice", "Bill", "Cortana", "Smith", "Jack" };

        static OrderJsonProvider()
        {
            // var seed = DateTime.UtcNow.Millisecond;
            var seed = 0;

            //1亿
            _orderIds = (from i in Enumerable.Range(seed + 1, seed + 2 * 5_000_000)
                         select i).ToList();

            _productIds = (from i in Enumerable.Range(seed + 1000, seed + 2000)
                           select i).ToList();

            _random = new Random(seed);
        }

        public static Task InitializeTask()
        {
            return Task.CompletedTask;
        }

        public static IEnumerable<SaleOrderDto> CreateOrders(int start = 0, int count = 10)
                                                                  => OnCreateOrders(start, count);

        private static IEnumerable<SaleOrderDto> OnCreateOrders(int start, int count)
        {
            var allOrders = new List<SaleOrderDto>();
            for (int i = start; i < start + count; i++)
            {
                var newOrderDto = new SaleOrderDto()
                {
                    OrderNumber = _orderIds[i],
                    Status = (OrderStatus)Enum.Parse(typeof(OrderStatus), OrderStates[_random.Next(0, 2)]),
                    CreatedOnUtc = DateTime.UtcNow,
                    CustomerId = Customers[_random.Next(0, 4)],
                    //  ThreadId = Thread.CurrentThread.ManagedThreadId,
                    OrderLineItems = new List<SaleOrderLineItemDto>()
                    {
                        new SaleOrderLineItemDto() { ProductId= $"{ProductePrix[_random.Next(0, 4)]} Widget {_productIds[_random.Next(0,_productIds.Count - 1)]}", Quantity=54*_random.Next(1,10),TaxRate=TaxRate,  UnitPriceExcludeTax=10 *_random.Next(10,500)/100},
                        new SaleOrderLineItemDto() { ProductId= $"{ProductePrix[_random.Next(0, 4)]} Widget {_productIds[_random.Next(0,_productIds.Count - 1)]}", Quantity=90*_random.Next(1,100),TaxRate=TaxRate,UnitPriceExcludeTax= 20 *_random.Next(100,2000)/100 }
                    }.ToArray()
                };

                allOrders.Add(newOrderDto);
            }

            return allOrders;
        }
    }
}
