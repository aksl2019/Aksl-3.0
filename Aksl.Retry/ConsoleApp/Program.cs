using System;
using System.Threading.Tasks;

namespace Aksl.Retry.ConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await RetryListener.Instance.StartAsync();

            //  await RetryListener.Instance.InitializeTask();

            // await RetryListener.Instance.StartLinearRetryStrategy();

            //await RetryListener.Instance.StartExponentialRetryStrategy()
            //                    .ConfigureAwait(false);

            Console.ReadLine();

            // Console.WriteLine("Hello World!");
        }
    }
}
