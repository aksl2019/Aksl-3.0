using System;

namespace Aksl.Retry.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Startup.Instance
                  .Initialize()
                  //.StartLinearRetryStrategy()
                  .StartExponentialRetryStrategy()
                  .ConfigureAwait(false);

            Console.ReadLine();

           // Console.WriteLine("Hello World!");
        }
    }
}
