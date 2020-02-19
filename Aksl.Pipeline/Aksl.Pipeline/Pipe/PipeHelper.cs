using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Pipelines;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

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
            resumeWriterThreshold: resumeWriterThreshold ?? 0,
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

        public static async ValueTask WriteAsync(PipeWriter writer, IEnumerable<byte[]> byteList, int minAllocBufferSize = 4096)
        {
            var totalBytesCount = byteList.Sum(b => b.Count());

            #region Methods
            foreach (var bytes in byteList)
            {
                var memory = writer.GetMemory(minAllocBufferSize);

                var isArray = MemoryMarshal.TryGetArray<byte>(memory, out var arraySegment);
                Debug.Assert(isArray);

                if (bytes[^1] != (byte)'\n')
                {
                    var destArray = new byte[bytes.Length + 1];
                    Array.Copy(bytes, destArray, bytes.Length);
                    destArray[bytes.Length] = (byte)'\n';

                    destArray.AsMemory().CopyTo(arraySegment);
                    writer.Advance(destArray.Count());
                }
                else
                {
                    bytes.AsMemory().CopyTo(arraySegment);
                    writer.Advance(bytes.Count());
                }
            }

            //_logger.LogInformation($"Write To Pipe Bytes :{totalBytesCount},now:{DateTime.Now.TimeOfDay}");

            var flushTask = writer.FlushAsync();
            if (!flushTask.IsCompleted)
            {
                await flushTask;
            }
            await flushTask;
            #endregion
        }     
    }
}
