using System;
using System.Buffers;
using System.IO.Pipelines;

namespace Aksl.Pipeline
{
   public static class PipeHelper
    {
        public static PipeOptions GetPipeOptions(PipeScheduler writerScheduler, PipeScheduler readerScheduler, MemoryPool<byte> memoryPool, 
                                                 long? pauseWriterThreshold, long? resumeWriterThreshold, int minimumSegmentSize, bool useSynchronizationContext) => new PipeOptions
        (
            pool: memoryPool,
            writerScheduler: writerScheduler,
            readerScheduler: readerScheduler,
            pauseWriterThreshold: pauseWriterThreshold ?? 0,
            resumeWriterThreshold: resumeWriterThreshold ?? 0 ,
            useSynchronizationContext: useSynchronizationContext,
            minimumSegmentSize: minimumSegmentSize
        );

        public static PipeOptions DefaultOptions =
            new PipeOptions(writerScheduler: PipeScheduler.ThreadPool,
                            readerScheduler: PipeScheduler.ThreadPool,
                            useSynchronizationContext: false,
                            pauseWriterThreshold: 32768,
                            resumeWriterThreshold: 16384,
                            minimumSegmentSize: 2048);
    }
}
