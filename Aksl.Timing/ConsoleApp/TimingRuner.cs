using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Aksl.Timing;

namespace Aksl.Timing.ConsoleApp
{
    public class TimingRuner
    {
        private ulong[] attempts = new ulong[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 };

        ExponentialTiming _exponentialTiming = new ExponentialTiming();
        LinearTiming _liTiming = new LinearTiming();

        public void Run()
        {
            LinearTiming linearTiming = new LinearTiming(minimumPeriod: 1200, maximumPeriod: 6400);
            ExponentialTiming exponentialTiming = new ExponentialTiming(3200, 32000);
            double newTime = default(double);

            Console.WriteLine("Linear Timing");
            foreach (var attempt in attempts)
            {
                newTime = linearTiming.Get(attempt);
                // _exponentialTiming.Get();
                Console.WriteLine(newTime);
            }

            Console.WriteLine("Exponential Timing");
            foreach (var attempt in attempts)
            {
                newTime = exponentialTiming.Get(attempt);
                Console.WriteLine(newTime);
            }
        }
    }
}
