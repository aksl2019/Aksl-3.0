using System;
using System.Buffers;

namespace Aksl.Pipeline
{
    public static class MemoryPoolFactory
    {
        public static MemoryPool<byte> Create()
        {
#if DEBUG
            return new DiagnosticMemoryPool(CreateSlabMemoryPool());
#else
                        return CreateSlabMemoryPool();
#endif

            //return CreateSlabMemoryPool();
        }

        public static MemoryPool<byte> CreateSlabMemoryPool()
        {
            return new SlabMemoryPool();
        }

        public static readonly int MinimumSegmentSize = 4096;
    }
}
