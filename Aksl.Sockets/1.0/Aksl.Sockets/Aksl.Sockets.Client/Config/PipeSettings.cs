using System;

namespace Aksl.Sockets.Client.Configuration
{
    public interface IPipeSettings
    {
        long PauseWriterThreshold { get; set; }

        long ResumeWriterThreshold { get; set; }

        int MinimumSegmentSize { get; set; }

        int MinAllocBufferSize { get; set; }

        bool UseSynchronizationContext { get; set; }
    }

    public class PipeSettings
    {
        #region Members
        private const int DefaultPipeBufferSize = 32768;

        private long? _pauseWriterThreshold = null;
        private long? _resumeWriterThreshold = null;
        private int _minimumSegmentSize;
        private int _minAllocBufferSize;
        #endregion

        #region Members
        public static PipeSettings Default => new PipeSettings()
        {
            PauseWriterThreshold = 32 * 1024,
            ResumeWriterThreshold = 32 * 1024 / 4,
            UseSynchronizationContext = false,
            MinimumSegmentSize = 512,
            MinAllocBufferSize = 512
        };

        public static PipeSettings DefaultInput => new PipeSettings()
        {
            PauseWriterThreshold = 1024 * 1024,//1048576=1M
            ResumeWriterThreshold = 1024 * 1024,//1048576=1M
            UseSynchronizationContext = false,
            MinimumSegmentSize = 4 * 1024,//4K
            MinAllocBufferSize = 2 * 1024,//2K
        };

        public static PipeSettings DefaultOutput => new PipeSettings()
        {
            PauseWriterThreshold = 64 * 1024,//65536=64K
            ResumeWriterThreshold = 64 * 1024,//65536=64K
            UseSynchronizationContext = false,
            MinimumSegmentSize = 4 * 1024,//4K
            MinAllocBufferSize = 2 * 1024,//2K
        };
        #endregion

        #region Constructor
        public PipeSettings()
        {
            PauseWriterThreshold = 32 * 1024;
            ResumeWriterThreshold = PauseWriterThreshold / 4;
            UseSynchronizationContext = false;
            MinimumSegmentSize = 512;
            MinAllocBufferSize = 512;
        }
        #endregion

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