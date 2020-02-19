using System;
using System.Threading.Tasks;

namespace Contoso.ConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await WebApiSender.Instance.InitializeTask();

           //await WebApiSender.Instance.BulkCopyAsync();

            await WebApiSender.Instance.DeleteSaleOrdersAsync();

            //await WebApiSender.Instance.UpdateSaleOrdersAsync();

          //  await WebApiSender.Instance.GetAllPagedSaleOrdersAsync();

            // Console.WriteLine("Hello World!");

            Console.ReadLine();
        }
    }
}