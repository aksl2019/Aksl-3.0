using System;

namespace Aksl.Sockets.Server.Configure
{
    public class PipeSettings
    {
        #region Members
        private const int DefaultPipeBufferSize = 32768;

        private long? _pauseWriterThreshold = null;
        private long? _resumeWriterThreshold = null;
        private int _minimumSegmentSize ;
        private int _minAllocBufferSize;
        #endregion

        public static PipeSettings Default => new PipeSettings()
        {
            PauseWriterThreshold = 1024 * 1024,//1048576=1M
            ResumeWriterThreshold = 1024 * 1024,//1048576=1M
            MinimumSegmentSize = 4 * 1024,//4K
            UseSynchronizationContext = false,
            MinAllocBufferSize = 2 * 1024,//2K
        };

        public PipeSettings()
        {
            PauseWriterThreshold = 32 * 1024;
            ResumeWriterThreshold = PauseWriterThreshold / 4;
            UseSynchronizationContext = false;
            MinimumSegmentSize = 512;
            MinAllocBufferSize = 512;
        }

        #region Properties
        public long? PauseWriterThreshold
        {
            get => _pauseWriterThreshold;
            set
            {
                if (value.HasValue && value <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "Value must be null or a positive number.");
                }

                _pauseWriterThreshold = value;
            }
        }

        public long? ResumeWriterThreshold
        {
            get => _resumeWriterThreshold;
            set
            {
                if (value.HasValue && value <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "Value must be null or a positive number.");
                }

                _resumeWriterThreshold = value;
            }
        }

        public int MinimumSegmentSize
        {
            get => _minimumSegmentSize;
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "Value must be a positive number.");
                }
                _minimumSegmentSize = value;
            }
        }

        public int MinAllocBufferSize
        {
            get => _minAllocBufferSize;
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "Value must be a positive number.");
                }
                _minAllocBufferSize = value;
            }
        }

        public bool UseSynchronizationContext { get; set; }
        #endregion
    }
}