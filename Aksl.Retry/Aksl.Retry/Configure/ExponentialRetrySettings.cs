using System;

namespace Aksl.Retry
{
    public class ExponentialRetrySettings: RetrySettings
    {
        public ExponentialRetrySettings():base()
        {
            DeltaBackoff = 3d;
        }

        public double DeltaBackoff { get; set; }
    }
}