using System;

namespace Aksl.Pipeline
{
    public enum SchedulingMode
    {
        Default,
        ThreadPool,
        Inline
    }

    public class PipeSettings
    {
        private const int DefaultPipeBufferSize = 32768;

        public static PipeSettings Default => new PipeSettings()
        {
            ApplicationSchedulingMode = SchedulingMode.Default,

            PauseWriterThreshold = 32 * 1024,
            ResumeWriterThreshold = 32 * 1024 / 4,
            UseSynchronizationContext = false,
            MinimumSegmentSize = 512,
            MinAllocBufferSize = 512
        };

        public static PipeSettings DefaultInput => new PipeSettings()
        {
            //ApplicationSchedulingMode = SchedulingMode.Default,

            PauseWriterThreshold = 1024 * 1024,//1048576=1M
            ResumeWriterThreshold = 1024 * 1024,//1048576=1M
            UseSynchronizationContext = false,
            MinimumSegmentSize = 4 * 1024,//4K
            MinAllocBufferSize = 2 * 1024,//2K
        };

        public static PipeSettings DefaultOutput => new PipeSettings()
        {
            //ApplicationSchedulingMode = SchedulingMode.Default,

            PauseWriterThreshold = 64 * 1024,//65536=64K
            ResumeWriterThreshold  = 64 * 1024,//65536=64K
            UseSynchronizationContext = false,
            MinimumSegmentSize = 4 * 1024,//4K
            MinAllocBufferSize = 2 * 1024,//2K
        };

        //public PipeSettings()
        //{
        //    ApplicationSchedulingMode = SchedulingMode.Default;

        //    PauseWriterThreshold = 32 * 1024;
        //    ResumeWriterThreshold = PauseWriterThreshold / 4;
        //    UseSynchronizationContext = false;
        //    MinimumSegmentSize = 512;
        //    MinAllocBufferSize = 512;
        //}

        public SchedulingMode ApplicationSchedulingMode { get; set; }

        public long PauseWriterThreshold { get; set; }

        public long ResumeWriterThreshold { get; set; }

        public int MinimumSegmentSize { get; set; } 

        public int MinAllocBufferSize { get; set; } 

        public bool UseSynchronizationContext { get; set; } 
    }
}