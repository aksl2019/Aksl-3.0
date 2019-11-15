using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Aksl.Csv;

using ConsoleApp.Model;

namespace ConsoleApp
{
    public class BulkDataSource
    {
        public const string ProductsFile = @"Data\products.csv";

        private static Random _rnd = new Random(DateTime.UtcNow.Millisecond);
        private CsvTextProvider _csvTextProvider = new CsvTextProvider();

        /// <summary>
        /// Reads all products from the baked in CSV file
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Product>> ReadAllProducts()
        {
            List<Product> allProducts = new List<Product>();
            var productLines = _csvTextProvider.ReadEmbeddedCsvTextAsync(ProductsFile);
            int index = 0;
            await foreach (var pl in productLines)
            {
                if (index++ > 0)
                {
                    allProducts.Add(new Product(int.Parse(pl[0]))
                    {
                        ProductNumber = pl[1],
                        ProductName = pl[2],
                        ModelName = pl[3],
                        StandardCost = decimal.Parse(pl[5]),
                        ListPrice = decimal.Parse(pl[6]),
                        CategoryId = int.Parse(pl[7]),
                        StockTotal = _rnd.Next(100, 1000),
                        StockReserved = 0
                    });
                }
                //product.Skip(1) //skip CSV header
                //.Select(
                //    r => new Product(int.Parse(r[0]))
                //    {
                //        ProductNumber = r[1],
                //        ProductName = r[2],
                //        ModelName = r[3],
                //        StandardCost = decimal.Parse(r[5]),
                //        ListPrice = decimal.Parse(r[6]),
                //        CategoryId = int.Parse(r[7]),
                //        StockTotal = _rnd.Next(100, 1000),
                //        StockReserved = 0
                //    })
                //.ToList();
            }

            return allProducts;
        }

        /// <summary>
        /// Reads products from low Id to high Id from the baked in CSV file
        /// </summary>
        /// <returns></returns>
        //public IEnumerable<Product> ReadProducts(int lowId, int highId)
        //{
        //    List<Product> allProducts = _csvTextProvider.ReadEmbeddedCsvTextAsync(ProductsFile)
        //        .Skip(1) //skip CSV header
        //        .Where(
        //            r =>
        //            {
        //                int id = int.Parse(r[0]);
        //                return id >= lowId && id <= highId;
        //            })
        //        .Select(
        //            r => new Product(int.Parse(r[0]))
        //            {
        //                ProductNumber = r[1],
        //                ProductName = r[2],
        //                ModelName = r[3],
        //                StandardCost = decimal.Parse(r[5]),
        //                ListPrice = decimal.Parse(r[6]),
        //                CategoryId = int.Parse(r[7]),
        //                StockTotal = _rnd.Next(100, 1000),
        //                StockReserved = 0
        //            })
        //        .ToList();


        //    return allProducts;
        //}
    }
}