using System;
using System.Linq;
using System.Threading.Tasks;

namespace ConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            BulkDataSource bulkDataSource = new BulkDataSource();
            var products =await bulkDataSource.ReadAllProducts();

            //foreach (var p in products)
            //{
            //    Console.WriteLine(p.ProductName);
            //}

            //Console.WriteLine(products.Count());


            Console.WriteLine("Hello World!");

            Console.ReadLine();
           
        }
    }
}
