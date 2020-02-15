using System;

namespace Aksl.Retry
{
    public class RetrySettings
    {
        #region Members
        public const int DefaultRetryMaxCount = 5;
        public readonly double DefaultRetryMinBackoff = 0;
        public readonly TimeSpan DefaultRetryMaxBackoff = TimeSpan.FromSeconds(30);

        private double _minBackoff;
        private double _maxBackoff;
        #endregion

        public static RetrySettings Default => new RetrySettings()
        {
        };

        public RetrySettings()
        {
            _minBackoff = 0d;
            _maxBackoff = 30d;
            MaxRetryCount = DefaultRetryMaxCount; 
           
        }

        public double MinBackoff
        {
            get => _minBackoff;
            set
            {
                if (_minBackoff >= _maxBackoff)
                {
                    throw new ArgumentException($"The minimum back off period '{_minBackoff}' cannot exceed the maximum back off period of '{_maxBackoff}'.");
                }

                _minBackoff = value <= 0 ? DefaultRetryMinBackoff : _minBackoff;

                _minBackoff = value;
            }
        }

        public double MaxBackoff
        {
            get => _maxBackoff;
            set
            {
                if (_minBackoff >= _maxBackoff)
                {
                    throw new ArgumentException($"The minimum back off period '{_minBackoff}' cannot exceed the maximum back off period of '{_maxBackoff}'.");
                }

                _maxBackoff = value;
            }
        }


        public int MaxRetryCount { get; set; }
    }
}