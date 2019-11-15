﻿using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace System.Buffers
{
   internal class DiagnosticMemoryPool : MemoryPool<byte>
    {
        private readonly MemoryPool<byte> _pool;

        private readonly bool _allowLateReturn;

        private readonly bool _rentTracking;

        private readonly object _syncObj;

        private readonly HashSet<DiagnosticPoolBlock> _blocks;

        private readonly List<Exception> _blockAccessExceptions;

        private readonly TaskCompletionSource<object> _allBlocksRetuned;

        private int _totalBlocks;

        /// <summary>
        /// This default value passed in to Rent to use the default value for the pool.
        /// </summary>
        private const int AnySize = -1;

        public DiagnosticMemoryPool(MemoryPool<byte> pool, bool allowLateReturn = false, bool rentTracking = false)
        {
            _pool = pool;
            _allowLateReturn = allowLateReturn;
            _rentTracking = rentTracking;
            _blocks = new HashSet<DiagnosticPoolBlock>();
            _syncObj = new object();
            _allBlocksRetuned = new TaskCompletionSource<object>();
            _blockAccessExceptions = new List<Exception>();
        }

        public bool IsDisposed { get; private set; }

        public override IMemoryOwner<byte> Rent(int size = AnySize)
        {
            lock (_syncObj)
            {
                if (IsDisposed)
                {
                    MemoryPoolThrowHelper.ThrowObjectDisposedException(MemoryPoolThrowHelper.ExceptionArgument.MemoryPool);
                }

                var diagnosticPoolBlock = new DiagnosticPoolBlock(this, _pool.Rent(size));
                if (_rentTracking)
                {
                    diagnosticPoolBlock.Track();
                }
                _totalBlocks++;
                _blocks.Add(diagnosticPoolBlock);
                return diagnosticPoolBlock;
            }
        }

        public override int MaxBufferSize => _pool.MaxBufferSize;

        internal void Return(DiagnosticPoolBlock block)
        {
            bool returnedAllBlocks;
            lock (_syncObj)
            {
                _blocks.Remove(block);
                returnedAllBlocks = _blocks.Count == 0;
            }

            if (IsDisposed)
            {
                if (!_allowLateReturn)
                {
                    MemoryPoolThrowHelper.ThrowInvalidOperationException_BlockReturnedToDisposedPool(block);
                }

                if (returnedAllBlocks)
                {
                    SetAllBlocksReturned();
                }
            }

        }

        internal void ReportException(Exception exception)
        {
            lock (_syncObj)
            {
                _blockAccessExceptions.Add(exception);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (IsDisposed)
            {
                MemoryPoolThrowHelper.ThrowInvalidOperationException_DoubleDispose();
            }

            bool allBlocksReturned = false;
            try
            {
                lock (_syncObj)
                {
                    IsDisposed = true;
                    allBlocksReturned = _blocks.Count == 0;
                    if (!allBlocksReturned && !_allowLateReturn)
                    {
                        MemoryPoolThrowHelper.ThrowInvalidOperationException_DisposingPoolWithActiveBlocks(_totalBlocks - _blocks.Count, _totalBlocks, _blocks.ToArray());
                    }

                    if (_blockAccessExceptions.Any())
                    {
                        throw CreateAccessExceptions();
                    }
                }
            }
            finally
            {
                if (allBlocksReturned)
                {
                    SetAllBlocksReturned();
                }
            }
        }

        private void SetAllBlocksReturned()
        {
            if (_blockAccessExceptions.Any())
            {
                _allBlocksRetuned.SetException(CreateAccessExceptions());
            }
            else
            {
                _allBlocksRetuned.SetResult(null);
            }
        }

        private AggregateException CreateAccessExceptions()
        {
            return new AggregateException("Exceptions occurred while accessing blocks", _blockAccessExceptions.ToArray());
        }

        public async Task WhenAllBlocksReturnedAsync(TimeSpan timeout)
        {
            var task = await Task.WhenAny(_allBlocksRetuned.Task, Task.Delay(timeout));
            if (task != _allBlocksRetuned.Task)
            {
                MemoryPoolThrowHelper.ThrowInvalidOperationException_BlocksWereNotReturnedInTime(_totalBlocks - _blocks.Count, _totalBlocks, _blocks.ToArray());
            }

            await task;
        }
    }
}
