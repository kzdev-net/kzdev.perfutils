// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

using KZDev.PerfUtils.Helpers;

namespace KZDev.PerfUtils.Internals
{
    //################################################################################
    /// <summary>
    /// An implementation of a buffer pool that is used for the fixed,
    /// standard sized buffers in the <see cref="SegmentMemoryStreamSlim"/> class.
    /// </summary>
    [DebuggerDisplay($"{{{nameof(DebugDisplayValue)},nq}}")]
    internal class MemorySmallBufferPool
    {
        //================================================================================
        /// <summary>
        /// A class that holds a set of buffers that are all the same size.
        /// </summary>
        private class MemorySmallBufferSizedSet
        {
            /// <summary>
            /// The buffer size for this set.
            /// </summary>
            private readonly int _bufferSize;

            /// <summary>
            /// The array of buffers for this set.
            /// </summary>
            private readonly byte[]?[] _buffers;

            /// <summary>
            /// The next index to use for the buffer set to store a cached buffer.
            /// </summary>
            private int _nextStoreIndex;

            /// <summary>
            /// A bitmask that tracks which buffers have been cleared based on the stored index.
            /// </summary>
            private int _clearedBufferFlags;

            /// <summary>
            /// A 0 or 1 lock for the buffer set.
            /// </summary>
            private int _locked;

            //--------------------------------------------------------------------------------
            /// <summary>
            /// Initializes a new instance of the <see cref="MemorySmallBufferSizedSet"/> class.
            /// </summary>
            /// <param name="bufferSize">
            /// The size of the buffers in this set.
            /// </param>
            /// <param name="bufferCount">
            /// The number of buffers to create in this set.
            /// </param>
            public MemorySmallBufferSizedSet (int bufferSize, int bufferCount)
            {
                _bufferSize = bufferSize;
                _buffers = new byte[bufferCount][];
            }
            //--------------------------------------------------------------------------------
            /// <summary>
            /// Returns a buffer from this set.
            /// </summary>
            /// <param name="clearArray">
            /// Indicates if the buffer should be cleared before being returned.
            /// </param>
            /// <returns>
            /// A byte array of the buffer size.
            /// </returns>
            public byte[] Rent (bool clearArray)
            {
                byte[]? returnBuffer = null;

                // Lock this set
                if (Interlocked.CompareExchange(ref _locked, 1, 0) != 0)
                {
                    SpinWait spinner = new();
                    // We are actually OK with this loop and spinning because the operation
                    // we perform once we have the lock is very fast.
                    while (Interlocked.CompareExchange(ref _locked, 1, 0) != 0)
                    {
                        spinner.SpinOnce();
                    }
                }
                try
                {
                    // Check if we have a buffer to return.
                    if (_nextStoreIndex > 0)
                    {
                        returnBuffer = _buffers[--_nextStoreIndex];
                        // If the buffer was cleared, we don't need to clear it again.
                        if (clearArray)
                        {
                            // If the bit flag is set, then the buffer was cleared when stored.
                            clearArray = (0 != ((1 << _nextStoreIndex) & _clearedBufferFlags));
                        }
                    }
                }
                finally
                {
                    // Unlock this set
                    Volatile.Write(ref _locked, 0);
                }

                if (returnBuffer is null)
                    // A newly allocated array will always be cleared, so we don't need to do it here.
                    return new byte[_bufferSize];
                // We do have to check if the buffer needs to be cleared.
                if (clearArray)
                    Array.Clear(returnBuffer, 0, returnBuffer.Length);
                return returnBuffer;
            }
            //--------------------------------------------------------------------------------
            /// <summary>
            /// Returns to this pool the specified buffer that was previously acquired by
            /// a call to <see cref="Rent(bool)"/>.
            /// </summary>
            /// <param name="array">
            /// The raw buffer array being returned.
            /// </param>
            /// <param name="isCleared">
            /// Indicates if the buffer was cleared before being returned.
            /// </param>
            public void Return (byte[] array, bool isCleared)
            {
                // Lock this set
                if (Interlocked.CompareExchange(ref _locked, 1, 0) != 0)
                {
                    SpinWait spinner = new();
                    // We are actually OK with this loop and spinning because the operation
                    // we perform once we have the lock is very fast.
                    while (Interlocked.CompareExchange(ref _locked, 1, 0) != 0)
                    {
                        spinner.SpinOnce();
                    }
                }
                try
                {
                    if (_nextStoreIndex >= _buffers.Length)
                        return;
                    // If the buffer was cleared, we need to set the flag, otherwise we clear it.
                    if (isCleared)
                    {
                        _clearedBufferFlags |= (1 << _nextStoreIndex);
                    }
                    else
                    {
                        _clearedBufferFlags &= ~(1 << _nextStoreIndex);
                    }
                    _buffers[_nextStoreIndex++] = array;
                }
                finally
                {
                    // Unlock this set
                    Volatile.Write(ref _locked, 0);
                }
            }
            //--------------------------------------------------------------------------------
        }
        //================================================================================

        /// <summary>
        /// Determine the maximum number of buffer sets to create for each buffer size.
        /// </summary>
        /// <remarks>
        /// We choose to cap @ (sizeof(int) * 8) to get the number of bits in an integer, which is
        /// how we will track which buffers are cleared and not cleared. We also limit
        /// the number of buffer sets to the number of processors * 4 if that is less
        /// than the number of bits in an integer.
        /// </remarks>
        private static readonly int MaxBufferSetCount = Math.Min(sizeof(int) * 8, Environment.ProcessorCount * 4);

        /// <summary>
        /// Debug helper to display the state of the group.
        /// </summary>
        [ExcludeFromCodeCoverage]
#pragma warning disable HAA0601
        private string DebugDisplayValue => $"{_bufferSizes.Length} Buffer Sets";
#pragma warning restore HAA0601

        /// <summary>
        /// The buffer sizes that are managed by this pool.
        /// </summary>
        private readonly int[] _bufferSizes;

        /// <summary>
        /// The internal array pool that is used to manage the buffers.
        /// </summary>
        private readonly MemorySmallBufferSizedSet?[] _bufferSets;

        /// <summary>
        /// A 0 or 1 lock for the buffer set.
        /// </summary>
        private int _locked;

        //--------------------------------------------------------------------------------
        /// <summary>
        /// Gets the index of the buffer size that will hold the given buffer size.
        /// </summary>
        /// <param name="bufferSize">
        /// The buffer size to check against.
        /// </param>
        /// <returns>
        /// </returns>
        private int GetBufferSizeIndex (int bufferSize) => UtilityHelpers.GetBufferSizeIndex(_bufferSizes, bufferSize);
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Initializes a new instance of the <see cref="MemorySmallBufferPool"/> class.
        /// </summary>
        /// <param name="bufferSizes"></param>
        public MemorySmallBufferPool (IReadOnlyCollection<int> bufferSizes)
        {
            _bufferSizes = bufferSizes.ToArray();
            _bufferSets = new MemorySmallBufferSizedSet[_bufferSizes.Length];
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Returns to this pool the specified buffer that was previously acquired by
        /// a call to <see cref="Rent(int, bool)"/>.
        /// </summary>
        /// <param name="array">
        /// The raw buffer array being returned.
        /// </param>
        /// <param name="isCleared">
        /// Indicates if the buffer was cleared before being returned.
        /// </param>
        public void Return (byte[] array, bool isCleared)
        {
            int poolIndex = GetBufferSizeIndex(array.Length);
            MemorySmallBufferSizedSet bufferSet;

            // Lock this pool
            if (Interlocked.CompareExchange(ref _locked, 1, 0) != 0)
            {
                SpinWait spinner = new();
                // We are actually OK with this loop and spinning because the operation
                // we perform once we have the lock is very fast.
                while (Interlocked.CompareExchange(ref _locked, 1, 0) != 0)
                {
                    spinner.SpinOnce();
                }
            }
            try
            {
                bufferSet = (_bufferSets[poolIndex] ??= new MemorySmallBufferSizedSet(_bufferSizes[poolIndex], MaxBufferSetCount));
            }
            finally
            {
                // Unlock this set
                Volatile.Write(ref _locked, 0);
            }
            bufferSet.Return(array, isCleared);
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Returns a buffer from the pool that is at least the specified length.
        /// </summary>
        /// <param name="sizeIndex">
        /// The index into the buffer size array that we want to rent from.
        /// </param>
        /// <param name="clearArray">
        /// Indicates if the buffer should be cleared before being returned.
        /// </param>
        /// <returns>
        /// A byte array that is at least the specified length.
        /// </returns>
        public byte[] Rent (int sizeIndex, bool clearArray)
        {
            MemorySmallBufferSizedSet? bufferSet;

            // Lock this pool
            if (Interlocked.CompareExchange(ref _locked, 1, 0) != 0)
            {
                SpinWait spinner = new();
                // We are actually OK with this loop and spinning because the operation
                // we perform once we have the lock is very fast.
                while (Interlocked.CompareExchange(ref _locked, 1, 0) != 0)
                {
                    spinner.SpinOnce();
                }
            }
            try
            {
                bufferSet = _bufferSets[sizeIndex];
            }
            finally
            {
                // Unlock this set
                Volatile.Write(ref _locked, 0);
            }
            return bufferSet?.Rent(clearArray) ?? new byte[_bufferSizes[sizeIndex]];
        }
        //--------------------------------------------------------------------------------
    }
    //################################################################################
}
