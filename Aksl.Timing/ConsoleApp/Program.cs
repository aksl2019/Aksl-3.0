using System;

namespace Aksl.Timing.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            TimingRuner timingRuner = new TimingRuner();
            timingRuner.Run();

            Console.ReadLine();

            Console.WriteLine("Hello World!");
        }
    }
}
