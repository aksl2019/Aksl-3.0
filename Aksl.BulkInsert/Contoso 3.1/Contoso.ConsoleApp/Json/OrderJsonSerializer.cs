//using System;
//using System.Collections.Generic;
//using System.Text;

//using Newtonsoft.Json;

//using Contoso.DataSource.Dtos;

//namespace Contoso.ConsoleApp
//{
//    public static class OrderJsonSerializer
//    {
//        public static OrderDto DeserializeOrder(byte[] orderBytes)
//                                                  => OnDeserializeOrder(orderBytes);

//        public static byte[] SerializeOrder(OrderDto purchaseOrder)
//                                                   => OnSerializeOrder(purchaseOrder);

//        private static OrderDto OnDeserializeOrder(byte[] message)
//        {
//            var orderString = Encoding.UTF8.GetString(message);
//            var order = JsonConvert.DeserializeObject<OrderDto>(orderString);
//            return order;
//        }

//        public static IEnumerable<OrderDto> DeserializeOrders(IEnumerable<byte[]> messages)
//        {
//            var orders = new List<OrderDto>();
//            string orderString = default;
//            OrderDto order = default;
//            byte[] orderByte = default;

//            foreach (var ob in messages)
//            {
//                try
//                {
//                    orderByte = ob;
//                    orderString = Encoding.UTF8.GetString(ob);
//                    order = JsonConvert.DeserializeObject<OrderDto>(orderString);
//                    //var po = OnDeserializeOrder(ob);
//                    orders.Add(order);
//                }
//                catch (Exception ex)
//                {
//                    Console.WriteLine($"Error while process a order: {ex.Message}");
//                }
//            }
//            return orders;
//        }

//        private static byte[] OnSerializeOrder(OrderDto purchaseOrder)
//        {
//            var po = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(purchaseOrder));

//            return po;
//        }
//    }
//}
