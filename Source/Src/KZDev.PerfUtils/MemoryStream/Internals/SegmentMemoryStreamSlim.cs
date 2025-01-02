// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

using KZDev.PerfUtils.Helpers;
using KZDev.PerfUtils.Observability;

namespace KZDev.PerfUtils.Internals
{
    //################################################################################
    /// <summary>
    /// A memory stream slim implementation that can expand its capacity as needed 
    /// and uses a combination of small buffers and 'standard sizes' heap allocated 
    /// buffer segments to manage the data.
    /// </summary>
    [DebuggerDisplay($"{{{nameof(DebugDisplayValue)}}}")]
    internal sealed class SegmentMemoryStreamSlim : MemoryStreamSlim
    {
        /// <summary>
        /// The maximum size of a list of standard buffers that we will cache in the buffer list cache.
        /// This list is the actual List&lt;SegmentBufferVirtualInfo&gt; that holds the standard buffers, and we 
        /// will cache lists that can be reused for different streams.
        /// </summary>
        private const int LargestCachedBufferListCapacity = 4096;

        /// <summary>
        /// The size of the buffer list cache growth. This is based on each slot being exponentially (base 2)
        /// larger than the previous slot, where the largest slot is the largest cached buffer list 
        /// capacity (LargestCachedBufferListCapacity).
        /// So, slot 0 is 1, slot 1 is 2, slot 2 is 4, slot 3 is 8, ... slot 12 is 4096.
        /// </summary>
        private const int BufferListCacheSize = 13;

        //================================================================================
        /// <summary>
        /// Information about the needs for allocation based on the requested capacity.
        /// </summary>
        /// <param name="allocatedCapacity">
        /// Total internal allocation capacity needed.
        /// </param>
        /// <param name="standardBufferSegmentCount">
        /// The number of standard size buffer segments needed.
        /// </param>
        /// <param name="smallBufferIndex">
        /// The index in the small buffer size array of the small buffer needed.
        /// </param>
        /// <remarks>
        /// This will return information about either the number of standard size buffers needed
        /// or the index of the small buffer size needed. If the StandardBufferCount is greater than
        /// zero, then the SmallBufferIndex will be -1. If the SmallBufferIndex is greater than or
        /// zero, then the StandardBufferCount will be zero.
        /// </remarks>
        [DebuggerDisplay($"{{{nameof(DebugDisplayValue)}}}")]
        private readonly struct AllocationNeedInfo (long allocatedCapacity, int standardBufferSegmentCount, int smallBufferIndex)
        {
            /// <summary>
            /// Debug helper to display the state of the group.
            /// </summary>
            [ExcludeFromCodeCoverage]
#pragma warning disable HAA0601
            internal string DebugDisplayValue => $"AllocatedCapacity = {AllocatedCapacity}, StdBuffers = {StandardBufferSegmentCount}, SmallIndex = {SmallBufferIndex}";
#pragma warning restore HAA0601

            /// <summary>
            /// Total internal allocation capacity needed.
            /// </summary>
            public long AllocatedCapacity { [DebuggerStepThrough] get; } = allocatedCapacity;
            /// <summary>
            /// The number of standard size buffer segments needed.
            /// </summary>
            public int StandardBufferSegmentCount { [DebuggerStepThrough] get; } = standardBufferSegmentCount;
            /// <summary>
            /// The index in the small buffer size array of the small buffer needed.
            /// </summary>
            public int SmallBufferIndex { [DebuggerStepThrough] get; } = smallBufferIndex;
        }
        //================================================================================
        /// <summary>
        /// Holds information about the current active buffer based on the current stream position.
        /// That is, the index of the buffer in the buffer list and the buffer itself that
        /// maps to the current position of the stream.
        /// </summary>
        /// <param name="index">
        /// The index in the buffer list of the current buffer. If <see cref="IsSmallBuffer"/> is
        /// <c>true</c>, then the index represents the index into the small buffer size array.
        /// Otherwise, it represents the index into the buffer list.
        /// </param>
        /// <param name="isSmallBuffer">
        /// Indicates if the buffer is one of the small buffers or not.
        /// </param>
        /// <param name="buffer">
        /// A reference to the current buffer. This will be null until we have at least some
        /// buffer allocated on first access. Otherwise, this references either a buffer
        /// in the list or one of the static small buffers.
        /// </param>
        [DebuggerDisplay($"{{{nameof(DebugDisplayValue)}}}")]
        private readonly struct CurrentBufferInfo (int index, bool isSmallBuffer, in SegmentBuffer buffer)
        {
            /// <summary>
            /// Debug helper to display the state of the group.
            /// </summary>
            [ExcludeFromCodeCoverage]
#pragma warning disable HAA0601
            internal string DebugDisplayValue => Buffer.IsEmpty ? "Empty" : $"{(IsSmallBuffer ? "Small" : "Std")}, Index {Index}, {Buffer.DebugDisplayValue}";
#pragma warning restore HAA0601

            /// <summary>
            /// An empty or default instance of the <see cref="CurrentBufferInfo"/> struct.
            /// </summary>
            public static CurrentBufferInfo Empty { [DebuggerStepThrough] get; } = new(-1, false, SegmentBuffer.Empty);

            /// <summary>
            /// The index in the buffer list of the current buffer. If <see cref="IsSmallBuffer"/> is
            /// <c>true</c>, then the index represents the index into the small buffer size array.
            /// Otherwise, it represents the index into the buffer list.
            /// </summary>
            public int Index { [DebuggerStepThrough] get; } = index;
            /// <summary>
            /// Indicates if the buffer is one of the small buffers or not.
            /// </summary>
            public bool IsSmallBuffer { [DebuggerStepThrough] get; } = isSmallBuffer;
            /// <summary>
            /// A reference to the current buffer. When this <see cref="CurrentBufferInfo"/>
            /// does not represent a valid buffer, this property will be <see cref="SegmentBuffer.Empty"/>.
            /// </summary>
            public SegmentBuffer Buffer { [DebuggerStepThrough] get; } = buffer;
        }
        //================================================================================
        /*
         * The approach to using buffers is this. If the stream size never approaches the
         * LOH allocation threshold, then we can use a set of small buffers that are cached
         * within the static class, and then backed by the shared MemorySmallBufferPool.
         * If the stream size does get large, then we can allocate buffers in the MemorySegmentedBufferPool that
         * are set sizes, and we simplify managing the data in these larger buffers by always using
         * the same size. The idea being that once we get to a certain size, we're looking for more efficiency
         * in managing the data in the buffers and the buffers themselves, and a little less so
         * on the memory footprint. That is, once we get to a stream that is (10 x 0x10000), then
         * the impact of consuming (11 x 0x10000) for a stream that has a length of ((10 x 0x10000) + 100)
         * is likely not impactful in the big picture to gain overall throughput performance and a huge
         * reduction in GC pressure.
         */

        /// <summary>
        /// The thread static cache of buffer lists that are used for standard size buffers.
        /// </summary>
        [ThreadStatic]
        private static List<SegmentBufferVirtualInfo>?[]? _bufferListCache;

        /// <summary>
        /// Holds the sizes of the buffer list cache slots. This is used to determine the capacity 
        /// size (number of buffers) for each slot in the buffer list cache.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Internal to allow access by friend test assemblies.
        /// </para>
        /// <para>
        /// When we return a buffer list to the cache, we will clear the list before storing it.
        /// </para>
        /// <para>
        /// Also, when we return the buffer list to the cache, we may be placing a list with a capacity
        /// that is smaller than the 'determined' capacity for the slot. This is OK because this is 
        /// just a helper, and the list will expand its capacity as needed anyway. The point is that the
        /// capacity of the list is at least larger than the capacity of the previous slot.
        /// </para>
        /// </remarks>
#if NOT_PACKAGING
        internal
#else
        private
#endif
        static readonly int[] BufferListCacheBufferCountCapacities;

        /// <summary>
        /// The sizes of the 'small' buffers that are used for streams that remain comfortably
        /// below the LOH allocation threshold. We will cache these buffers for reuse.
        /// </summary>
        /// <remarks>
        /// Internal to allow access by friend test assemblies.
        /// </remarks>
#if NOT_PACKAGING
        internal
#else
        private
#endif
        static readonly int[] SmallBufferSizes;

        /// <summary>
        /// A cache of small buffers that are used for streams that remain comfortably below
        /// the LOH allocation threshold and are not zeroed out.
        /// </summary>
        private static readonly byte[]?[] SmallBufferNonZeroedCache;

        /// <summary>
        /// A cache of small buffers that are used for streams that remain comfortably below
        /// the LOH allocation threshold and are zeroed out.
        /// </summary>
        private static readonly byte[]?[] SmallBufferZeroedCache;

        /// <summary>
        /// Gets the array pool to use for renting internal buffers.
        /// </summary>
        private static MemorySegmentedBufferPool _bufferArrayPool;

        //--------------------------------------------------------------------------------
        /// <summary>
        /// Gets the array pool to use for renting internal small buffers, when our static
        /// cache of small buffers does not have a buffer of the needed size.
        /// </summary>
        private static MemorySmallBufferPool SmallBufferArrayPool { [DebuggerStepThrough] get; }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// This is the offset in the current buffer (the buffer referenced in 
        /// <see cref="_currentBufferInfo"/>) where our position is currently set.
        /// </summary>
        private int _currentBufferOffset;

        /// <summary>
        /// If <c>true</c>, this means that the position has been set to a value that may not
        /// point to a valid buffer. This is used to determine if we need to select the current
        /// buffer based on the current position before we read or write data.
        /// </summary>
        private bool _currentBufferInvalid;

        /// <summary>
        /// The actual allocated capacity of the stream. This can be different from the base
        /// class capacity because of the way we manage the buffers.
        /// </summary>
        private long _allocatedCapacity;

        /// <summary>
        /// A list of buffers that are used to store the data when we need to expand the capacity
        /// beyond the largest 'small' buffer size. In other words, this is only created once
        /// we move past the small buffer sizes. Once this list is allocated, we will just keep
        /// using standard size buffers and not use the small buffers anymore for this stream.
        /// </summary>
        private List<SegmentBufferVirtualInfo>? _bufferList;

        /// <summary>
        /// The current buffer information based on the current position.
        /// </summary>
        private CurrentBufferInfo _currentBufferInfo;

        //--------------------------------------------------------------------------------
        /// <summary>
        /// Static constructor for the SegmentMemoryStreamSlim class.
        /// </summary>
        static SegmentMemoryStreamSlim ()
        {
            _bufferArrayPool = new MemorySegmentedBufferPool(InternalUseNativeLargeMemoryBuffers);

            // Get the size of the system page on this system.
            int systemPageSize = Environment.SystemPageSize;
            List<int> buildSmallBufferSize = [];
            // The size of the 'small' buffers we'll use.
            int smallBufferSize = systemPageSize;
            // If the system page size is larger than 1/8th of the max buffer size, 
            // then we will use system page size increments for the small buffer sizes.
            if (systemPageSize > StandardBufferSegmentSize >> 3)
            {
                while (smallBufferSize < StandardBufferSegmentSize)
                {
                    buildSmallBufferSize.Add(smallBufferSize);
                    smallBufferSize += systemPageSize;
                }
            }
            else
            {
                // Otherwise, we will use powers of 2 for the small buffer sizes.
                while (smallBufferSize < StandardBufferSegmentSize)
                {
                    buildSmallBufferSize.Add(smallBufferSize);
                    smallBufferSize <<= 1;
                }
            }
            SmallBufferSizes = buildSmallBufferSize.ToArray();
            SmallBufferNonZeroedCache = new byte[SmallBufferSizes.Length][];
            SmallBufferZeroedCache = new byte[SmallBufferSizes.Length][];
            SmallBufferArrayPool = new MemorySmallBufferPool(SmallBufferSizes);

            // Determine the buffer list cache buffer count capacities
            BufferListCacheBufferCountCapacities = new int[BufferListCacheSize];
            int setBufferCount = 1;
            for (int checkIndex = 0; checkIndex < BufferListCacheSize; checkIndex++)
            {
                BufferListCacheBufferCountCapacities[checkIndex] = setBufferCount;
                setBufferCount <<= 1;
            }
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Gets the index of the small buffer size that will hold the given buffer size.
        /// </summary>
        /// <param name="bufferSize">The needed small buffer size</param>
        /// <returns>
        /// The index in the small buffer size array that will hold the given buffer size.
        /// </returns>
        private static int GetSmallBufferIndex (int bufferSize)
        {
            Debug.Assert(bufferSize < StandardBufferSegmentSize, "bufferSize >= StandardBufferSegmentSize");
            return UtilityHelpers.GetBufferSizeIndex(SmallBufferSizes, bufferSize);
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Returns the number of bytes that are left in the current position buffer based
        /// on the current offset
        /// </summary>
        private int CurrentBufferBytesLeft
        {
            get
            {
                VerifyCurrentBuffer();
                return _currentBufferInfo.Buffer.IsEmpty ?
                    0 :
                    _currentBufferInfo.Buffer.Length - _currentBufferOffset;
            }
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Gets the (debug) display value.
        /// </summary>
        /// <value>
        /// The (debug) display value.
        /// </value>
        [ExcludeFromCodeCoverage]
#pragma warning disable HAA0601
        private string DebugDisplayValue => $@"Length = {Length}, Position = {Position}, Mode = {nameof(MemoryStreamSlimMode.Dynamic)}";
#pragma warning restore HAA0601
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the offset in the current buffer where our position is currently set
        /// </summary>
        private int VirtualPosition
        {
            get => PositionInternal;
            set => SetCurrentVirtualPosition(value);
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Requests that all memory buffers managed by MemoryStreamSlim instances should be released 
        /// back to the system as soon as possible.
        /// </summary>
        /// <remarks>
        /// This is a hint to the system that the caller requests that memory be released, but it 
        /// is not guaranteed that the memory will be released immediately. As soon as all 
        /// currently in-use MemoryStreamSlim instances are disposed or finalized, the currently 
        /// allocated memory buffers will be released.
        /// </remarks>
        public static void ReleaseMemoryBufferPool ()
        {
            // By exchanging the buffer array pool, we are effectively releasing the current pool once
            // all the current buffers are returned to the pool which results in all the references to 
            // the pool being released so the GC can collect it and all the buffers.
            MemorySegmentedBufferPool previousPool = 
                Interlocked.Exchange(ref _bufferArrayPool, new MemorySegmentedBufferPool(InternalUseNativeLargeMemoryBuffers));
            // Fully release the previous pool
            previousPool.Dispose();
        }
        //--------------------------------------------------------------------------------

        #region Standard Buffer Management

        //--------------------------------------------------------------------------------
        /// <summary>
        /// Gets the index into the buffer list cache for the given capacity needed.
        /// </summary>
        /// <param name="buffersNeeded">
        /// The anticipated capacity needed for the buffer list.
        /// </param>
        /// <returns>
        /// The index into the buffer list cache for the given buffer count capacity needed. If this
        /// returns -1, then the capacity needed is too large for the cache.
        /// </returns>
        private static int GetBufferListCacheIndex (int buffersNeeded)
        {
            // If the size needed is larger than the largest cached buffer list capacity, then
            // we won't use the cache.
            if (buffersNeeded > LargestCachedBufferListCapacity)
                return -1;
            for (int cacheIndex = 0; cacheIndex < BufferListCacheSize; ++cacheIndex)
            {
                if (BufferListCacheBufferCountCapacities[cacheIndex] >= buffersNeeded)
                    return cacheIndex;
            }
            // Given the check when we entered this method, we should never get here.
            return -1;
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Gets a buffer list to use for holding standard size buffers with the requested
        /// anticipated capacity.
        /// </summary>
        /// <param name="segmentsNeeded">
        /// The potential anticipated capacity needed for the buffer list (number of buffers in the list)
        /// based on the number of standard size buffer segments needed.
        /// </param>
        /// <returns>
        /// A list of segment buffers that can be used for holding standard size buffers.
        /// </returns>
        private static List<SegmentBufferVirtualInfo> GetBufferList (int segmentsNeeded)
        {
            int cacheIndex = GetBufferListCacheIndex(segmentsNeeded);
            if (cacheIndex == -1)
                return new List<SegmentBufferVirtualInfo>(segmentsNeeded);
            // Do we even have a buffer list cache?
            if (_bufferListCache is null)
                // We're going to return a list that will fit nicely in determined cache slot,
                // even though the use of the list may expand it; in which case it will 
                // just go into a different slot (or allowed to be collected).
                return new List<SegmentBufferVirtualInfo>(BufferListCacheBufferCountCapacities[cacheIndex]);
            // Check the correct slot in the cache, but to save an allocation, we will
            // accept a list with a larger capacity than needed.
            int checkCacheIndex = cacheIndex;
            while (checkCacheIndex < BufferListCacheSize)
            {
                List<SegmentBufferVirtualInfo>? checkReturnList = _bufferListCache[checkCacheIndex];
                if (checkReturnList is not null)
                {
                    _bufferListCache[checkCacheIndex] = null;
                    return checkReturnList;
                }
                checkCacheIndex++;
            }
            // If we get here, then we need to allocate a new list
            // We're going to return a list that will fit nicely in determined cache slot,
            // even though the use of the list may expand it; in which case it will 
            // just go into a different slot (or allowed to be collected).
            return new List<SegmentBufferVirtualInfo>(BufferListCacheBufferCountCapacities[cacheIndex]);
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Stores a buffer list in the buffer list cache (if the list is not too large).
        /// </summary>
        /// <param name="bufferList">
        /// The list to store in the buffer list cache.
        /// </param>
        private static void StoreBufferList (List<SegmentBufferVirtualInfo> bufferList)
        {
            // Which slot in the cache should we store the list?
            int cacheIndex = GetBufferListCacheIndex(bufferList.Capacity);
            if (cacheIndex == -1)
            {
                // We still clear the list before returning it, even if we don't store it.
                bufferList.Clear();
                return;
            }
            // Do we have a buffer list cache?
            _bufferListCache ??= new List<SegmentBufferVirtualInfo>?[BufferListCacheSize];
            // Clear the list before storing it (even if we don't store it, clearly it could be
            // helpful to the GC.).
            bufferList.Clear();
            // Store the list in the cache, we could just overwrite the slot, because either
            // way, one will be collected and one will be stored. But, there is a greater chance
            // that the one we are storing could be in an ephemeral generation, and the stored
            // one could be in a higher generation, so we'll make the check to see if we can
            // store this instance.
            if (_bufferListCache[cacheIndex] is not null)
                return;
            _bufferListCache[cacheIndex] = bufferList;
        }
        //--------------------------------------------------------------------------------

        #endregion Standard Buffer Management

        #region Rent and Release Buffer Management

        //--------------------------------------------------------------------------------
        /// <summary>
        /// Rents a standard buffer from the buffer array pool.
        /// </summary>
        /// <param name="requestedBufferSize">
        /// The desired size of the buffer to rent.
        /// </param>
        /// <param name="forceZeroBytes">
        /// If <c>true</c>, then the buffer will be zeroed out before returning it, regardless
        /// of the settings for zeroing out buffers.
        /// </param>
        /// <param name="preferredBlockInfo">
        /// Information about the last block that was used to allocate a buffer. This is used
        /// to try and allocate from the same block and next segment if possible
        /// </param>
        /// <returns>
        /// The rented buffer and a flag indicating if the returned buffer is the next segment
        /// from the preferred block.
        /// </returns>
        private (SegmentBuffer Buffer, bool IsNextBlockSegment) RentStandardBufferFromPreferredBlock
            (int requestedBufferSize, bool forceZeroBytes, in SegmentBufferInfo preferredBlockInfo)
        {
            // If the preferred block info is the default, then just rent a standard buffer.
            if (preferredBlockInfo.BufferPool is null)
                return (RentStandardBuffer(requestedBufferSize, forceZeroBytes), false);

            // Note - our buffer pool is a custom pool that only we will be using, so we don't 
            // have to worry about non-cleared data from other users; but, it is static and 
            // different instances of the stream may or may not have different security requirements,
            // so if our settings are to clear the buffer, then we will clear it before returning it,
            // but this is only done if the segments weren't cleared when they were stored.
            MemorySegmentedBufferPool currentBufferPool = Volatile.Read(ref _bufferArrayPool);
            // We do need to check if the current buffer pool is the same as the preferred block info.
            if (currentBufferPool != preferredBlockInfo.BufferPool)
                return (RentStandardBuffer(requestedBufferSize, forceZeroBytes), false);
            return currentBufferPool.RentFromPreferredBlock(requestedBufferSize,
                forceZeroBytes || (Settings.ZeroBufferBehavior != MemoryStreamSlimZeroBufferOption.None),
                preferredBlockInfo);
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Rents a standard buffer from the buffer array pool.
        /// </summary>
        /// <param name="requestedBufferSize">
        /// The desired size of the buffer to rent.
        /// </param>
        /// <param name="forceZeroBytes">
        /// If <c>true</c>, then the buffer will be zeroed out before returning it, regardless
        /// of the settings for zeroing out buffers.
        /// </param>
        /// <returns>
        /// The rented buffer.
        /// </returns>
        private SegmentBuffer RentStandardBuffer (int requestedBufferSize, bool forceZeroBytes)
        {
            // Note - our buffer pool is a custom pool that only we will be using, so we don't 
            // have to worry about non-cleared data from other users; but, it is static and 
            // different instances of the stream may or may not have different security requirements,
            // so if our settings are to clear the buffer, then we will clear it before returning it,
            // but this is only done if the segments weren't cleared when they were stored.
            SegmentBuffer returnBuffer = Volatile.Read(ref _bufferArrayPool).Rent(requestedBufferSize,
                forceZeroBytes || (Settings.ZeroBufferBehavior != MemoryStreamSlimZeroBufferOption.None));
            return returnBuffer;
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Rents a buffer of at least the requested size from the small buffer array pool.
        /// </summary>
        /// <param name="bufferSize">
        /// The requested size of the buffer to rent.
        /// </param>
        /// <returns>
        /// A byte array buffer that is at least the size of the requested buffer.
        /// </returns>
        private SegmentBuffer RentSmallBuffer (int bufferSize)
        {
            Debug.Assert(bufferSize > 0, "bufferSize <= 0");
            Debug.Assert(bufferSize < StandardBufferSegmentSize, "bufferSize >= StandardBufferSegmentSize");

            // Get buffers in specific increment sizes based on the small buffer array sizes.
            int smallBufferIndex = GetSmallBufferIndex(bufferSize);
            // Determine which small cache we should use based on the zero buffer behavior.
            bool zeroBuffer = Settings.ZeroBufferBehavior != MemoryStreamSlimZeroBufferOption.None;
            byte[]? returnBuffer;
            if (zeroBuffer)
            {
                // Try to get the buffer from the zeroed cache first.
                returnBuffer = Interlocked.Exchange(ref SmallBufferZeroedCache[smallBufferIndex], null);
                if (returnBuffer is not null)
                {
                    return returnBuffer;
                }
                // If we didn't get a buffer from the zeroed cache, then try to get one from the non-zeroed cache
                // and zero it out before returning it.
                returnBuffer = Interlocked.Exchange(ref SmallBufferNonZeroedCache[smallBufferIndex], null);
                if (returnBuffer is not null)
                {
                    Array.Clear(returnBuffer, 0, returnBuffer.Length);
                    return returnBuffer;
                }
            }
            else
            {
                // Try to get the buffer from the non-zeroed cache first.
                returnBuffer = Interlocked.Exchange(ref SmallBufferNonZeroedCache[smallBufferIndex], null);
                if (returnBuffer is not null)
                {
                    return returnBuffer;
                }
                // If we didn't get a buffer from the non-zeroed cache, then try to get one from the zeroed cache
                returnBuffer = Interlocked.Exchange(ref SmallBufferZeroedCache[smallBufferIndex], null);
                if (returnBuffer is not null)
                {
                    return returnBuffer;
                }
            }

            // We don't have a fast cache instance, so we need to rent a buffer from the shared pool.
            SegmentBuffer returnSegmentBuffer = SmallBufferArrayPool.Rent(smallBufferIndex, zeroBuffer);
            return returnSegmentBuffer;
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Releases a previously rented standard buffer.
        /// </summary>
        /// <param name="buffer"></param>
        private void ReleaseStandardBuffer (in SegmentBuffer buffer)
        {
            Debug.Assert(buffer.BufferInfo.BufferPool is not null, "buffer.BufferPool is null");
            if (buffer.IsEmpty)
                return;
            // For security reasons, we want to clear the buffer before returning it,
            // even though we are using a custom pool, we want to guarantee that the buffer
            // doesn't have any sensitive data in it. We could clear when we get the buffer,
            // but then the data could be hanging around in memory for an indeterminate amount
            // of time.
            buffer.BufferInfo.BufferPool.Return(buffer, Settings.ZeroBufferBehavior);
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Reduces a previously rented standard buffer which releases the lower segments
        /// back to the array pool.
        /// </summary>
        /// <param name="buffer">
        /// The segment buffer to be reduced.
        /// </param>
        /// <param name="newSegmentCount">
        /// The new number of segments that the buffer should hold.
        /// </param>
        private SegmentBuffer ReduceStandardBuffer (in SegmentBuffer buffer, int newSegmentCount)
        {
            Debug.Assert(!buffer.IsRaw, "buffer.IsRaw");
            Debug.Assert(!buffer.IsEmpty, "buffer.IsEmpty");
            Debug.Assert(buffer.BufferInfo.BufferPool is not null, "buffer.BufferInfo.BufferPool is null");
            Debug.Assert(newSegmentCount < buffer.SegmentCount, "newSegmentCount >= buffer.SegmentCount");
            Debug.Assert(newSegmentCount > 0, "newSegmentCount <= 0");

            // For security reasons, we want to clear the buffer before returning it,
            // even though we are using a custom pool, we want to guarantee that the buffer
            // doesn't have any sensitive data in it. We could clear when we get the buffer,
            // but then the data could be hanging around in memory for an indeterminate amount
            // of time.
            return buffer.BufferInfo.BufferPool!.Reduce(buffer, newSegmentCount, Settings.ZeroBufferBehavior);
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Releases a previously rented small buffer.
        /// </summary>
        /// <param name="buffer">
        /// The buffer instance to be released
        /// </param>
        /// <param name="smallBufferIndex">
        /// The index in the static small buffer cache that the buffer should be released to.
        /// </param>
        private void ReleaseSmallBuffer (in SegmentBuffer buffer, int smallBufferIndex)
        {
            Debug.Assert(buffer.IsRaw, "!buffer.IsRaw");
            if (buffer.IsEmpty)
                return;
            byte[] rawBuffer = buffer.RawBuffer!;
            // For small buffers, we always clear the buffer before returning it to the pool if the 
            // options is anything other than None.
            bool zeroBuffer = Settings.ZeroBufferBehavior != MemoryStreamSlimZeroBufferOption.None;
            if (zeroBuffer)
            {
                Array.Clear(rawBuffer, 0, rawBuffer.Length);
                // Try to store in the static (fast) cache first, if we can't, then return to the array pool.
                if (Interlocked.CompareExchange(ref SmallBufferZeroedCache[smallBufferIndex], rawBuffer, null) is null)
                {
                    // We stored the buffer in the fast cache, so we're done.
                    return;
                }
                // Return the buffer to the shared pool.
                SmallBufferArrayPool.Return(rawBuffer, true);
                return;
            }
            // Try to store in the static (fast) cache first, if we can't, then return to the array pool.
            if (Interlocked.CompareExchange(ref SmallBufferNonZeroedCache[smallBufferIndex], rawBuffer, null) is null)
            {
                // We stored the buffer in the fast cache, so we're done.
                return;
            }
            // Return the buffer to the shared pool.
            SmallBufferArrayPool.Return(rawBuffer, false);
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Releases the currently used small buffer if it exists.
        /// </summary>
        private void ReleaseSmallBuffer ()
        {
            if (!_currentBufferInfo.IsSmallBuffer || _currentBufferInfo.Buffer.IsEmpty)
                return;
            ReleaseSmallBuffer(_currentBufferInfo.Buffer, _currentBufferInfo.Index);
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Releases the currently used small buffer if it exists, and also resets the current
        /// buffer information.
        /// </summary>
        private void ReleaseSmallBufferAndResetCurrentBuffer ()
        {
            if (!_currentBufferInfo.IsSmallBuffer || _currentBufferInfo.Buffer.IsEmpty)
                return;
            SetCurrentBufferInfo(CurrentBufferInfo.Empty);
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Adds a new segment buffer to the buffer list.
        /// </summary>
        /// <param name="newBuffer">
        /// The new buffer to add to the buffer list.
        /// </param>
        private void AddSegmentBufferToList (in SegmentBuffer newBuffer)
        {
            Debug.Assert(_bufferList is not null, "_bufferList is null");
            if (_bufferList!.Count == 0)
            {
                _bufferList.Add(new SegmentBufferVirtualInfo(newBuffer, newBuffer.Length, newBuffer.SegmentCount));
                return;
            }
            _bufferList.Add(SegmentBufferVirtualInfo.FromPrevious(newBuffer, _bufferList[^1]));
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Rents a standard buffer and copies the valid data from the existing 
        /// small buffer if it exists and has data in it.  This then returns whether any data
        /// was copied from the small buffer - which also indicates whether the
        /// standard buffer was rented.
        /// </summary>
        /// <param name="requestedBufferSize">
        /// The desired size of the buffer to rent.
        /// </param>
        /// <param name="forceZeroBytes">
        /// If <c>true</c>, then the buffer will be zeroed out before returning it, regardless
        /// of the settings for zeroing out buffers.
        /// </param>
        /// <remarks>
        /// It's assumed that the buffer list has been allocated and is available before this
        /// is called.
        /// </remarks>
        /// <returns>
        /// <c>true</c> if a new standard buffer was allocated, copied existing data from the 
        /// small buffer, and added to the buffer list; otherwise, <c>false</c>.
        /// </returns>
        private bool RentStandardBufferAndCopySmallBuffer (int requestedBufferSize, bool forceZeroBytes)
        {
            Debug.Assert(_bufferList is not null, "_bufferList is null");
            Debug.Assert(_bufferList.Count == 0, "_bufferList.Count != 0");
            // If we don't currently have a small buffer, then we don't need to copy anything
            if (_currentBufferInfo.Buffer.IsEmpty || !_currentBufferInfo.IsSmallBuffer || LengthInternal == 0)
            {
                return false;
            }

            // Allocate a standard buffer and copy the data from the current small buffer
            SegmentBuffer newBuffer = RentStandardBuffer(requestedBufferSize, forceZeroBytes);
            // Our current length can be safely cast to an int because we are currently using small buffers
            _currentBufferInfo.Buffer[..LengthInternal].CopyTo(newBuffer);
            AddSegmentBufferToList(newBuffer);
            ReleaseSmallBufferAndResetCurrentBuffer();
            return true;
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Rents a small buffer and copies the valid data from the existing 
        /// small buffer if it exists and has data in it.  This then returns whether any data
        /// was copied from the small buffer - which also indicates whether the
        /// larger small buffer was rented.
        /// </summary>
        /// <param name="bufferSize">
        /// The size of the buffer needed to rent and copy to.
        /// </param>
        /// <param name="smallBufferIndex">
        /// The small buffer index that is needed for the buffer size, if this has already been calculated.
        /// </param>
        private void RentSmallBufferAndCopySmallBuffer (int bufferSize, int smallBufferIndex)
        {
            Debug.Assert(bufferSize > 0, "bufferSize <= 0");
            Debug.Assert(bufferSize < StandardBufferSegmentSize, "bufferSize >= StandardBufferSegmentSize");
            Debug.Assert(smallBufferIndex < SmallBufferSizes.Length, "smallBufferIndex >= SmallBufferSizes.Length");

            // If we don't currently have a small buffer, then we don't need to copy anything
            if (_currentBufferInfo.Buffer.IsEmpty || LengthInternal == 0)
            {
                _currentBufferInfo = new CurrentBufferInfo(smallBufferIndex, true, RentSmallBuffer(SmallBufferSizes[smallBufferIndex]));
                _allocatedCapacity = _currentBufferInfo.Buffer.Length;
                return;
            }
            // If the allocated small buffer is big enough for this new size, then we don't need to do anything
            if (_currentBufferInfo.Buffer.Length >= bufferSize) return;

            // Allocate a small buffer and copy the data from the current small buffer
            SegmentBuffer newBuffer = RentSmallBuffer(bufferSize);
            // Our current length can be safely cast to an int because we are currently using small buffers
            _currentBufferInfo.Buffer[..LengthInternal].CopyTo(newBuffer);
            // Set the new buffer as the current buffer
            SetCurrentBufferInfo(new CurrentBufferInfo(smallBufferIndex, true, newBuffer));
            _allocatedCapacity = newBuffer.Length;
        }
        //--------------------------------------------------------------------------------

        #endregion Rent and Release Buffer Management

        #region Capacity Management

        //--------------------------------------------------------------------------------
        /// <summary>
        /// Gets the allocation size information needed for the given capacity.
        /// </summary>
        /// <param name="requestedCapacity">
        /// The requested capacity to allocate.
        /// </param>
        /// <returns>
        /// An instance of <see cref="AllocationNeedInfo"/> that contains the 
        /// needed allocation information.
        /// </returns>
        private AllocationNeedInfo GetAllocateCapacity (long requestedCapacity)
        {
            Debug.Assert(requestedCapacity >= 0, "capacity < 0");
            Debug.Assert(requestedCapacity <= MaximumCapacity, "capacity > MaximumCapacity");

            // Determine the number of standard size buffers needed
            int standardBuffersCount = Math.DivRem((int)requestedCapacity, StandardBufferSegmentSize, out int remainder);
            if (standardBuffersCount == 0)
            {
                // If we need more than the largest small buffer size, then we actually do
                // need to use a standard buffer, and we also stick to using standard buffers
                // once we go beyond the small buffer sizes at any point.
                if ((_bufferList is not null) || (requestedCapacity > SmallBufferSizes[^1]))
                    standardBuffersCount = 1;
                else
                {
                    int smallBufferIndex = GetSmallBufferIndex((int)requestedCapacity);
                    return new AllocationNeedInfo(SmallBufferSizes[smallBufferIndex], 0, smallBufferIndex);
                }
            }
            // Do we need additional space beyond the complete standard buffers?
            else if (remainder > 0)
                standardBuffersCount++;
            // Calculate the size of the standard buffer allocation
            long standardBufferSize = Convert.ToInt64(StandardBufferSegmentSize) * standardBuffersCount;
            return new AllocationNeedInfo(standardBufferSize, standardBuffersCount, -1);
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Ensures that we have properly allocation the standard buffers needed for the
        /// requested capacity.
        /// </summary>
        /// <param name="capacityNeeded">
        /// The full allocation capacity needed.
        /// </param>
        /// <param name="forceZeroBytes">
        /// If <c>true</c>, then the buffer will be zeroed out before returning it, regardless
        /// of the settings for zeroing out buffers.
        /// </param>
        /// <returns>
        /// Whether any new buffers were allocated to reach the needed capacity.
        /// </returns>
        private void EnsureStandardBuffersCapacity (long capacityNeeded, bool forceZeroBytes)
        {
            Debug.Assert(capacityNeeded >= 0, "capacityNeeded < 0");

            if (capacityNeeded == 0) return;
            // If we need at least one standard buffer, then make sure the list is created
            _bufferList ??= GetBufferList((int)(capacityNeeded / StandardBufferSegmentSize));

            // Do we need to allocate more buffers?
            if (_bufferList.Count > 0)
                capacityNeeded -= _bufferList[^1].SegmentEndOffset;
            if (capacityNeeded <= 0) return;

            // We need to allocate more buffers - but do we currently have a small
            // buffer that we need to copy?
            if (_currentBufferInfo.IsSmallBuffer && LengthInternal > 0)
            {
                // The last buffer we have is currently a small buffer, so we need to copy
                // the contents into a new buffer.
                if (RentStandardBufferAndCopySmallBuffer((int)capacityNeeded, forceZeroBytes))
                {
                    // The only way we should get here is during a transition from using a small buffer
                    // to using the standard size buffers
                    Debug.Assert(_bufferList.Count == 1, "_bufferList.Count != 1");

                    // If we allocated a new standard buffer, then we can reduce the capacity needed
                    capacityNeeded -= _bufferList[0].SegmentEndOffset;
                }
            }

            // Now, allocate the rest of the standard buffers needed
            while (capacityNeeded > 0)
            {
                if (0 == _bufferList.Count)
                {
                    SegmentBuffer newBuffer = RentStandardBuffer((int)capacityNeeded, forceZeroBytes);
                    AddSegmentBufferToList(newBuffer);
                    capacityNeeded -= newBuffer.Length;
                    continue;
                }
                // Get the next buffer from the preferred block if possible. We want to get some
                // segments that are immediately after the last segment in the last buffer in the list
                // if possible. This means we don't have to have another entry in the list, and we can
                // just extend the last buffer in the list.
                SegmentBuffer lastBuffer = _bufferList[^1].SegmentBuffer;
                SegmentBufferInfo lastBufferInfo = lastBuffer.BufferInfo;
                (SegmentBuffer nextBuffer, bool isNextBlockSegment) =
                    RentStandardBufferFromPreferredBlock((int)capacityNeeded, forceZeroBytes, lastBufferInfo);

                if (!isNextBlockSegment)
                {
                    // We couldn't get the preferred block, so we need to add a new buffer to the list
                    AddSegmentBufferToList(nextBuffer);
                    capacityNeeded -= nextBuffer.Length;
                    continue;
                }

                // If the segment is from the next block, then we have just essentially
                // extended the last buffer in the list. Update the information in the last
                // buffer in the list to reflect the new buffer.
                MemorySegment lastMemorySegment = lastBuffer.MemorySegment;

                // Since we're extending the last buffer in the list, we need to be sure that 
                // the current buffer will be properly selected when it's needed.
                _currentBufferInvalid = true;
                // We also check the current buffer information because....
                //   It is a value type and so it is copied, and 
                //   For efficiency, we don't replace _currentBufferInfo when selecting the current buffer
                //       unless the index is different from the calculated current buffer index (in SelectCurrentBuffer())
                if (_currentBufferInfo.Index == _bufferList.Count - 1)
                    // Clear this out to force the current buffer to be recalculated
                    _currentBufferInfo = CurrentBufferInfo.Empty;

                // Now, we need to replace the last buffer in the list with the new buffer
                SegmentBuffer replaceBuffer = lastBuffer.Concat(nextBuffer);

                // Replace the last buffer in the list with the new buffer
                if (_bufferList.Count == 1)
                    _bufferList[^1] = new SegmentBufferVirtualInfo(replaceBuffer, replaceBuffer.Length, replaceBuffer.SegmentCount);
                else
                {
                    // Adjust the virtual information for the last buffer in the list based on the 
                    // new buffer and the second to last buffer in the list.
                    _bufferList[^1] = SegmentBufferVirtualInfo.FromPrevious(replaceBuffer, _bufferList[^2]);
                }
                capacityNeeded -= nextBuffer.Length;
            }
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Ensures that we have the requested capacity allocated and returns whether
        /// we had to allocate a new buffer (or buffers) to reach this capacity.
        /// </summary>
        /// <param name="neededCapacity">
        /// The capacity needed to be allocated and available.
        /// </param>
        /// <param name="forceZeroBytes">
        /// If <c>true</c>, then the buffer will be zeroed out before returning it, regardless
        /// of the settings for zeroing out buffers.
        /// </param>
        /// <returns>
        /// <c>true</c> if any allocation changes were made; otherwise, <c>false</c>.
        /// </returns>
        private void EnsureCapacity (long neededCapacity, bool forceZeroBytes = false)
        {
            Debug.Assert(neededCapacity >= 0, "neededCapacity < 0");
            Debug.Assert(neededCapacity <= MaximumCapacity, "neededCapacity > MaximumCapacity");

            // Do we already have the capacity?
            if (neededCapacity <= CapacityInternal) return;
            // Do we need to allocate a new buffer?
            if (neededCapacity <= _allocatedCapacity)
            {
                // Just simply update the reported capacity
                CapacityInternal = (int)neededCapacity;
                return;
            }
            // Get the allocation information for the needed capacity
            AllocationNeedInfo allocationInfoForCapacity = GetAllocateCapacity(neededCapacity);
            // Check if we only need a small buffer
            if (allocationInfoForCapacity.StandardBufferSegmentCount > 0)
            {
                // We can cast capacityNeeded to an int because we have an absolute maximum
                // capacity that is less than (or equal to) the maximum value of an int.
                EnsureStandardBuffersCapacity((int)allocationInfoForCapacity.AllocatedCapacity, forceZeroBytes);
                _allocatedCapacity = allocationInfoForCapacity.AllocatedCapacity;
                CapacityInternal = (int)neededCapacity;
                // Be sure that the 'current' buffer gets properly selected.
                _currentBufferInvalid = true;
                return;
            }
            // Allocate the small buffer if we don't have one, or if the current buffer is 
            // not the right size and copy the data if needed.
            RentSmallBufferAndCopySmallBuffer((int)neededCapacity, allocationInfoForCapacity.SmallBufferIndex);
            // Set the capacity to the requested needed capacity
            CapacityInternal = (int)neededCapacity;
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Frees memory to drop the capacity to the specified value.
        /// </summary>
        /// <param name="requestedCapacity">
        /// The capacity needed to be allocated and available.
        /// </param>
        /// <returns>
        /// <c>true</c> if any allocation changes were made; otherwise, <c>false</c>.
        /// </returns>
        private void ReduceCapacity (int requestedCapacity)
        {
            Debug.Assert(requestedCapacity >= 0, "requestedCapacity < 0");
            Debug.Assert(requestedCapacity <= CapacityInternal, "requestedCapacity > _capacity");

            // Note while we are reducing capacity, we are not reducing the length of the stream or
            // changing the position. We are only reducing the capacity of the stream.
            // There is an implied invariant that the length the stream is always less 
            // than or equal to the capacity of the stream.

            // Get the capacity information for the requested capacity
            AllocationNeedInfo capacityInfo = GetAllocateCapacity(requestedCapacity);
            // If the target allocation capacity is the same as the current allocation capacity, or we are
            // currently using a small buffer, then we don't need to do anything.
            if (capacityInfo.AllocatedCapacity == _allocatedCapacity || _currentBufferInfo.IsSmallBuffer)
            {
                // Just simply update the reported capacity. To avoid the overhead of just switching small
                // buffers and copying the data, we will just update the capacity and return for any
                // small buffer size.
                CapacityInternal = requestedCapacity;
                return;
            }

            // If we get here, then we know that the number of standard buffer segments is being reduced.
            // Given that the length of the stream is less than the capacity, we don't worry about
            // any copying needs, so we just reduce the number of standard buffers and release them
            // as we are able.
            int segmentsToRelease = _bufferList!.Count == 0 ? 0 : _bufferList[^1].SegmentEndCount - capacityInfo.StandardBufferSegmentCount;

            while (segmentsToRelease > 0)
            {
                SegmentBuffer checkListSegmentBuffer = _bufferList[^1].SegmentBuffer;
                if (checkListSegmentBuffer.SegmentCount <= segmentsToRelease)
                {
                    segmentsToRelease -= checkListSegmentBuffer.SegmentCount;
                    ReleaseStandardBuffer(checkListSegmentBuffer);
                    _bufferList.RemoveAt(_bufferList.Count - 1);
                }
                else
                {
                    // We need to reduce the buffer
                    SegmentBuffer newBuffer = ReduceStandardBuffer(checkListSegmentBuffer, checkListSegmentBuffer.SegmentCount - segmentsToRelease);
                    if (_bufferList.Count == 1)
                        _bufferList[0] = new SegmentBufferVirtualInfo(newBuffer, newBuffer.Length, newBuffer.SegmentCount);
                    else
                    {
                        _bufferList[^1] = SegmentBufferVirtualInfo.FromPrevious(newBuffer, _bufferList[^2]);
                    }
                    break;
                }
            }
            _allocatedCapacity = _bufferList.Count == 0 ? 0 : _bufferList[^1].SegmentEndOffset;
            CapacityInternal = requestedCapacity;
        }
        //--------------------------------------------------------------------------------

        #endregion Capacity Management

        //--------------------------------------------------------------------------------
        /// <summary>
        /// Helper to release the current buffer (if appropriate)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ReleaseCurrentBuffer ()
        {
            if (_currentBufferInfo is { IsSmallBuffer: true, Buffer.RawBuffer: not null })
            {
                ReleaseSmallBuffer();
            }
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Sets the new buffer information as the current buffer information, and properly
        /// releases any previously used buffer.
        /// </summary>
        /// <param name="newBufferInfo">
        /// The new buffer info to be set as the current buffer info.
        /// </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SetCurrentBufferInfo (in CurrentBufferInfo newBufferInfo)
        {
            ReleaseCurrentBuffer();
            _currentBufferInfo = newBufferInfo;
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Internal helper to set the virtual position to the specified value.
        /// </summary>
        /// <param name="newPosition">
        /// The new position to set the virtual position to.
        /// </param>
        /// <returns>
        /// <c>true</c> if the position was set to the new value; otherwise, <c>false</c>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool SetCurrentVirtualPosition (int newPosition)
        {
            Debug.Assert(newPosition >= 0, "newPosition < 0");
            if (PositionInternal == newPosition)
                return false;
            _currentBufferInvalid = true;
            PositionInternal = newPosition;
            return true;
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Adjusts the virtual position and sets the current buffer.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OffsetPositionAndSetCurrentBuffer (int offsetPosition)
        {
            if (offsetPosition == 0)
                return;
            if (offsetPosition > 0)
            {
                // Check if we just moved within the same current buffer
                if ((_currentBufferOffset + offsetPosition) < _currentBufferInfo.Buffer.Length)
                {
                    _currentBufferOffset += offsetPosition;
                    PositionInternal += offsetPosition;
                    return;
                }
            }
            // offsetPosition is negative - Check if we just moved within the same current buffer
            else if ((_currentBufferOffset + offsetPosition) >= 0)
            {
                _currentBufferOffset += offsetPosition;
                PositionInternal += offsetPosition;
                return;
            }
            SetPositionAndCurrentBuffer(PositionInternal + offsetPosition);
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Sets the virtual position to the specified value, and sets the current buffer
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SetPositionAndCurrentBuffer (int newPosition)
        {
            if (!SetCurrentVirtualPosition(newPosition))
                return;
            SelectCurrentBuffer();
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Checks if the current buffer is dirty and selects the current buffer if needed.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void VerifyCurrentBuffer ()
        {
            if (!_currentBufferInvalid)
                return;
            SelectCurrentBuffer();
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Seeks to the specified offset from the specified location.
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="fromLocation"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="IOException"></exception>
        private long SeekInternal (long offset, long fromLocation)
        {
            long tempPosition = fromLocation + offset;
            switch (tempPosition)
            {
                case > MaxMemoryStreamLength:
                    ThrowHelper.ThrowArgumentOutOfRangeException_StreamLength(nameof(offset));
                    break;

                case < 0:
                    ThrowHelper.ThrowIOException_SeekBeforeBegin();
                    break;
            }

            Position = tempPosition;
            return Position;
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Gets the buffer index and offset into the buffer for the given offset.
        /// </summary>
        /// <param name="offset">
        /// The stream offset to get the buffer index and offset for.
        /// </param>
        /// <returns>
        /// The index of the buffer in the buffer list and the offset into that buffer.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private (int BufferIndex, int BufferOffset) GetBufferInfoForOffset (long offset)
        {
            // We can be assured of int size result (not a long) because of the MaxCapacity value
            // Based on the current position, we need to determine the buffer index and offset into that buffer
            if (offset == 0)
                return (0, 0);
            if (offset >= _bufferList![^1].SegmentEndOffset)
                // This is a special case where the virtual offset is beyond the end of the last buffer, so we need
                // to return -1 as the index.
                return (-1, 0);
            if (_bufferList!.Count == 1)
                return offset == _bufferList[0].SegmentEndOffset ? (1, 0) : (0, (int)offset);
            Debug.Assert(offset <= _bufferList[^1].SegmentEndOffset, "offset > _bufferList[^1].SegmentEndOffset");
            if (offset == _bufferList[^1].SegmentEndOffset)
                // This is a special case where the virtual offset is at the end of the last buffer, so we need to return the next index
                // and a buffer offset of 0.
                return (_bufferList.Count, 0);

            // Now, we need to search the list for the correct buffer
            int lowIndex = 0;
            int highIndex = _bufferList.Count - 1;
            while (lowIndex <= highIndex)
            {
                int checkIndex = lowIndex + highIndex >> 1;
                SegmentBufferVirtualInfo checkListBuffer = _bufferList[checkIndex];
                if (offset > checkListBuffer.SegmentEndOffset)
                    lowIndex = checkIndex + 1;
                else
                {
                    if (offset == checkListBuffer.SegmentEndOffset)
                        return (checkIndex + 1, 0);
                    if (checkIndex == 0)
                        return (0, (int)offset);
                    SegmentBufferVirtualInfo previousCheckListBuffer = _bufferList[checkIndex - 1];
                    if (offset <= checkListBuffer.SegmentEndOffset && offset > previousCheckListBuffer.SegmentEndOffset)
                        return (checkIndex, (int)(offset - previousCheckListBuffer.SegmentEndOffset));
                    // Otherwise, keep looking
                    highIndex = checkIndex - 1;
                }
            }
            Debug.Fail("Invalid search and/or index");
            return (-1, -1);
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Selects the current buffer instance based on the current position value.
        /// </summary>
        private void SelectCurrentBuffer ()
        {
            // Assume we are going to resolve the current buffer, regardless of whether the position
            // is pointing to a valid buffer or not.
            _currentBufferInvalid = false;
            // If we don't have an allocated buffer list, then we don't have any buffers to select
            // from. If we're using a small buffer, then having the proper size small buffer is
            // taken care of during the allocation assurance process, and is set in the
            // _currentBufferInfo instance.
            if (_bufferList is null)
            {
                _currentBufferOffset = PositionInternal;
                return;
            }

            // We can be assured of int size result (not a long) because of the MaxCapacity value
            // Based on the current position, we need to determine the buffer index and offset into that buffer
            (int newBufferListIndex, int newBufferOffset) = GetBufferInfoForOffset(PositionInternal);

            // Any needed allocations should be taken care of before calling this, so make sure 
            // that the new buffer index is within the bounds of the buffer list. But, this could
            // also be called after a read where the position lands just past the end of the last
            // buffer. So, we need to make sure that we are within the bounds of the buffer list.
            // Also, in cases where the length is being changed, but the current length was already
            // smaller than the current position, this method can be called, in which case
            // GetBufferInfoForOffset can return an index that is negative because no buffer
            // exists at the index that would correlate to the position.
            if ((newBufferListIndex < 0) || (newBufferListIndex == _bufferList.Count && newBufferOffset == 0))
            {
                _currentBufferInvalid = true;
                return;
            }
#pragma warning disable HAA0601
            Debug.Assert(newBufferListIndex < _bufferList.Count, $"newBufferIndex ({newBufferListIndex}) >= _bufferList.Count ({_bufferList.Count})");
#pragma warning restore HAA0601
            // Do we need to select a new buffer as the current buffer?
            if (_currentBufferInfo.IsSmallBuffer || newBufferListIndex != _currentBufferInfo.Index || _currentBufferInfo.Buffer.IsEmpty)
            {
                SegmentBufferVirtualInfo setCurrentListBuffer = _bufferList![newBufferListIndex];
                SetCurrentBufferInfo(new CurrentBufferInfo(newBufferListIndex, false, setCurrentListBuffer.SegmentBuffer));
            }
            _currentBufferOffset = newBufferOffset;
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Helper method to clear the gap between the current length and the current position
        /// when the position is set to value greater than the current length.
        /// </summary>
        /// <remarks>
        /// It is assumed that the current buffer is already selected before this method is called,
        /// and that any capacity changes have already been made.
        /// </remarks>
        private void ClearLengthPositionGap (int clearToPosition)
        {
            // How many bytes do we need to clear?
            int gapSize = clearToPosition - LengthInternal;
            Debug.Assert(gapSize > 0, "gapSize <= 0");
            if (_currentBufferInfo.IsSmallBuffer)
            {
                // If the current buffer for the current position is a small buffer, then the
                // gap size is less than the buffer size, so we can just clear the gap in the buffer.
                Array.Clear(_currentBufferInfo.Buffer.RawBuffer!, LengthInternal, gapSize);
                LengthInternal = VirtualPosition;
                return;
            }
            // Get the buffer index and offset into the buffer for the current length
            (int startBufferListIndex, int startBufferOffset) = GetBufferInfoForOffset(LengthInternal);
            // Get the buffer index and offset into the buffer for the 'clear to' position.
            (int targetBufferListIndex, int targetBufferOffset) = GetBufferInfoForOffset(clearToPosition);

            Debug.Assert(targetBufferListIndex >= startBufferListIndex, "targetBufferIndex < startBufferIndex");
            Debug.Assert((targetBufferListIndex != startBufferListIndex) || (targetBufferOffset > startBufferOffset), "targetBufferOffset <= startBufferOffset");

            // If the start and target buffer indexes are the same, then we can just clear the gap
            // in the current buffer.
            if (startBufferListIndex == targetBufferListIndex)
            {
                _bufferList![startBufferListIndex].SegmentBuffer.Slice(startBufferOffset, gapSize).Clear();
                LengthInternal = VirtualPosition;
                return;
            }
            // Now, chip away at the gap in the buffers - first clear the remaining bytes in the start buffer
            SegmentBuffer currentBuffer = _bufferList![startBufferListIndex].SegmentBuffer;
            int startBufferToClearLength = currentBuffer.Length - startBufferOffset;
            currentBuffer.Slice(startBufferOffset, startBufferToClearLength).Clear();
            // Adjust how much we have left to clear
            gapSize -= startBufferToClearLength;
            // Move to the next buffer
            int currentBufferIndex = startBufferListIndex + 1;
            while (gapSize > 0)
            {
                // Get the current buffer and clear the gap
                currentBuffer = _bufferList![currentBufferIndex++].SegmentBuffer;
                // If the gap left is less than the current buffer size, then we can just clear the gap
                if (gapSize <= currentBuffer.Length)
                {
                    Debug.Assert(((0 == targetBufferOffset) || (gapSize == targetBufferOffset)), "gapSize != targetBufferOffset");
                    currentBuffer.Slice(0, gapSize).Clear();
                    LengthInternal = VirtualPosition;
                    return;
                }
                // Clear the entire buffer
                currentBuffer.Clear();
                // Adjust how much we have left to clear
                gapSize -= currentBuffer.Length;
            }
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Internal helper to handle writing across multiple internal buffers.
        /// </summary>
        /// <param name="buffer">
        /// An array of bytes. This method copies count bytes from buffer to the current stream.
        /// </param>
        /// <param name="offset">
        /// The zero-based byte offset in buffer at which to begin copying bytes to the current stream.
        /// </param>
        /// <param name="count">
        /// The number of bytes to be written to the current stream.
        /// </param>
        private unsafe void WriteMultiBuffer (byte[] buffer, int offset, int count)
        {
            // Copy the data to the stream via multiple buffers
            int newPosition = VirtualPosition;
            int sourceBufferOffset = offset;
            // Start with the current buffer and the current buffer offset
            int currentBufferBytesLeft = CurrentBufferBytesLeft;
            SegmentBuffer currentBuffer = _currentBufferInfo.Buffer;
            int currentBufferIndex = _currentBufferInfo.Index;
            int currentBufferOffset = _currentBufferOffset;

            while (count > 0)
            {
                MemorySegment currentMemorySegment = currentBuffer.MemorySegment;
                int destinationBufferOffset = currentBufferOffset + currentMemorySegment.Offset;
                // If we can fit all the remaining bytes into the current buffer
                if (count <= currentBufferBytesLeft)
                {
                    if (currentMemorySegment.IsNative)
                    {
                        // Write the data to the current buffer
                        fixed (byte* pReadBuffer = buffer)
                        {
                            Buffer.MemoryCopy(pReadBuffer + sourceBufferOffset, currentMemorySegment.NativePointer + destinationBufferOffset,
                                currentBuffer.Length - currentBufferOffset, count);
                        }
                    }
                    else
                    {
                        // Write the data to the current buffer
                        fixed (byte* pReadBuffer = buffer, pWriteBuffer = currentMemorySegment.Array!)
                        {
                            Buffer.MemoryCopy(pReadBuffer + sourceBufferOffset, pWriteBuffer + destinationBufferOffset,
                                currentBuffer.Length - currentBufferOffset, count);
                        }
                    }
                    // Update the position and count values
                    newPosition += count;
                    break;
                }

                if (currentMemorySegment.IsNative)
                {
                    // Write the data to the current buffer
                    fixed (byte* pReadBuffer = buffer)
                    {
                        Buffer.MemoryCopy(pReadBuffer + sourceBufferOffset, currentMemorySegment.NativePointer + destinationBufferOffset,
                            currentBuffer.Length - currentBufferOffset, currentBufferBytesLeft);
                    }
                }
                else
                {
                    // Write the data to the current buffer
                    fixed (byte* pReadBuffer = buffer, pWriteBuffer = currentMemorySegment.Array!)
                    {
                        Buffer.MemoryCopy(pReadBuffer + sourceBufferOffset, pWriteBuffer + destinationBufferOffset,
                            currentBuffer.Length - currentBufferOffset, currentBufferBytesLeft);
                    }
                }
                // Update the position and count values
                count -= currentBufferBytesLeft;
                newPosition += currentBufferBytesLeft;
                // We can short-circuit and break here if we are at the end of the stream
                if (count == 0)
                    break;
                // We are working directly with the standard buffer list, so we can just update those values
                sourceBufferOffset += currentBufferBytesLeft;
                currentBufferOffset = 0;
                currentBuffer = _bufferList![++currentBufferIndex].SegmentBuffer;
                currentBufferBytesLeft = currentBuffer.Length;
            }

            if (newPosition > LengthInternal)
                LengthInternal = newPosition;
            // Set the position and current buffer
            SetPositionAndCurrentBuffer(newPosition);
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Internal helper to handle writing across multiple internal buffers.
        /// </summary>
        /// <param name="buffer">
        /// The source buffer to copy from.
        /// </param>
        /// <param name="copyCount">
        /// The number of bytes to copy from the source buffer.
        /// </param>
        private void WriteMultiBuffer (ReadOnlySpan<byte> buffer, int copyCount)
        {
            // Copy the data to the stream via multiple buffers
            int newPosition = VirtualPosition;
            int sourceBufferOffset = 0;
            // Start with the current buffer and the current buffer offset
            int currentBufferBytesLeft = CurrentBufferBytesLeft;
            SegmentBuffer currentBuffer = _currentBufferInfo.Buffer;
            int currentBufferIndex = _currentBufferInfo.Index;
            int currentBufferOffset = _currentBufferOffset;

            while (copyCount > 0)
            {
                // If we can fit all the remaining bytes into the current buffer
                if (copyCount <= currentBufferBytesLeft)
                {
                    // Write the data to the current buffer
                    buffer.Slice(sourceBufferOffset, copyCount).CopyTo(currentBuffer.AsSpan(currentBufferOffset, copyCount));
                    // Update the position and count values
                    newPosition += copyCount;
                    break;
                }
                // Write the data to the current buffer
                buffer.Slice(sourceBufferOffset, currentBufferBytesLeft).CopyTo(currentBuffer.AsSpan(currentBufferOffset, currentBufferBytesLeft));
                // Update the position and count values
                copyCount -= currentBufferBytesLeft;
                newPosition += currentBufferBytesLeft;
                // We can short-circuit and break here if we are at the end of the stream
                if (copyCount == 0)
                    break;
                // We are working directly with the standard buffer list, so we can just update those values
                sourceBufferOffset += currentBufferBytesLeft;
                currentBufferOffset = 0;
                currentBuffer = _bufferList![++currentBufferIndex].SegmentBuffer;
                currentBufferBytesLeft = currentBuffer.Length;
            }

            if (newPosition > LengthInternal)
                LengthInternal = newPosition;
            // Set the position and current buffer
            SetPositionAndCurrentBuffer(newPosition);
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Internal helper to handle reading across multiple internal buffers.
        /// </summary>
        /// <param name="buffer">
        /// An array of bytes. When this method returns, the buffer contains the specified byte array 
        /// with the values between offset and (offset + count - 1) replaced by the bytes read from the current source.
        /// </param>
        /// <param name="offset">
        /// The zero-based byte offset in buffer at which to begin storing the data read from the current stream.
        /// </param>
        /// <param name="count">
        /// The maximum number of bytes to be read from the current stream.
        /// </param>
        private unsafe int ReadMultiBuffer (byte[] buffer, int offset, int count)
        {
            int currentBufferBytesLeft = CurrentBufferBytesLeft;
            if (currentBufferBytesLeft == 0)
                return 0;

            int newPosition = VirtualPosition;
            int returnCount = 0;
            int destinationBufferOffset = offset;
            // Start with the current buffer and the current buffer offset
            SegmentBuffer currentBuffer = _currentBufferInfo.Buffer;
            int currentBufferIndex = _currentBufferInfo.Index;
            int currentBufferOffset = _currentBufferOffset;
            while (count > 0)
            {
                MemorySegment currentMemorySegment = currentBuffer.MemorySegment;
                int sourceBufferOffset = currentBufferOffset + currentMemorySegment.Offset;
                // If we have all the data left that we want to copy here, then just do the copy and return.
                if (count <= currentBufferBytesLeft)
                {
                    if (currentMemorySegment.IsNative)
                    {
                        // Copy the data from the current buffer to the output buffer
                        fixed (byte* pWriteBuffer = buffer)
                        {
                            Buffer.MemoryCopy(currentMemorySegment.NativePointer + sourceBufferOffset, pWriteBuffer + destinationBufferOffset, buffer.Length - destinationBufferOffset, count);
                        }
                    }
                    else
                    {
                        // Copy the data from the current buffer to the output buffer
                        fixed (byte* pReadBuffer = currentMemorySegment.Array!, pWriteBuffer = buffer)
                        {
                            Buffer.MemoryCopy(pReadBuffer + sourceBufferOffset, pWriteBuffer + destinationBufferOffset, buffer.Length - destinationBufferOffset, count);
                        }
                    }
                    // Update the position and count values
                    returnCount += count;
                    newPosition += count;
                    break;
                }

                if (currentMemorySegment.IsNative)
                {
                    // Copy the data from the current buffer to the output buffer
                    fixed (byte* pWriteBuffer = buffer)
                    {
                        Buffer.MemoryCopy(currentMemorySegment.NativePointer + sourceBufferOffset, pWriteBuffer + destinationBufferOffset, buffer.Length - destinationBufferOffset, currentBufferBytesLeft);
                    }
                }
                else
                {
                    // Copy the data from the current buffer to the output buffer
                    fixed (byte* pReadBuffer = currentMemorySegment.Array!, pWriteBuffer = buffer)
                    {
                        Buffer.MemoryCopy(pReadBuffer + sourceBufferOffset, pWriteBuffer + destinationBufferOffset, buffer.Length - destinationBufferOffset, currentBufferBytesLeft);
                    }
                }
                // Update the position and count values
                count -= currentBufferBytesLeft;
                returnCount += currentBufferBytesLeft;
                newPosition += currentBufferBytesLeft;
                // We can short-circuit and break here if we are at the end of the stream
                if (count == 0)
                    break;
                // We are working directly with the standard buffer list, so we can just update those values
                destinationBufferOffset += currentBufferBytesLeft;
                currentBufferOffset = 0;
                currentBuffer = _bufferList![++currentBufferIndex].SegmentBuffer;
                currentBufferBytesLeft = currentBuffer.Length;
            }
            // Update the position and the current buffer information
            SetPositionAndCurrentBuffer(newPosition);
            return returnCount;
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Internal helper to handle reading across multiple internal buffers.
        /// </summary>
        /// <param name="destinationBuffer">
        /// The destination buffer to copy to.
        /// </param>
        /// <param name="copyCount">
        /// The number of bytes to copy from the source buffer.
        /// </param>
        private int ReadMultiBuffer (Span<byte> destinationBuffer, int copyCount)
        {
            int currentBufferBytesLeft = CurrentBufferBytesLeft;
            if (currentBufferBytesLeft == 0)
                return 0;

            int newPosition = VirtualPosition;
            int returnCount = 0;
            int destinationBufferOffset = 0;
            // Start with the current buffer and the current buffer offset
            SegmentBuffer currentBuffer = _currentBufferInfo.Buffer;
            int currentBufferIndex = _currentBufferInfo.Index;
            int currentBufferOffset = _currentBufferOffset;
            while (copyCount > 0)
            {
                // If we have all the data left that we want to copy here, then just do the copy and return.
                if (copyCount <= currentBufferBytesLeft)
                {
                    // Copy the data from the current buffer to the output buffer
                    currentBuffer.AsSpan(currentBufferOffset, copyCount).CopyTo(destinationBuffer.Slice(destinationBufferOffset));
                    // Update the position and count values
                    returnCount += copyCount;
                    newPosition += copyCount;
                    break;
                }
                // Copy the data from the current buffer to the output buffer
                currentBuffer.AsSpan(currentBufferOffset, currentBufferBytesLeft).CopyTo(destinationBuffer.Slice(destinationBufferOffset, currentBufferBytesLeft));
                // Update the position and count values
                copyCount -= currentBufferBytesLeft;
                returnCount += currentBufferBytesLeft;
                newPosition += currentBufferBytesLeft;
                // We can short-circuit and break here if we are at the end of the stream
                if (copyCount == 0)
                    break;
                // We are working directly with the standard buffer list, so we can just update those values
                destinationBufferOffset += currentBufferBytesLeft;
                currentBufferOffset = 0;
                currentBuffer = _bufferList![++currentBufferIndex].SegmentBuffer;
                currentBufferBytesLeft = currentBuffer.Length;
            }
            // Update the position and the current buffer information
            SetPositionAndCurrentBuffer(newPosition);
            return returnCount;
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Finalizes an instance of the <see cref="SegmentMemoryStreamSlim"/> class.
        /// </summary>
        ~SegmentMemoryStreamSlim ()
        {
            UtilsEventSource.Log.MemoryStreamSlimFinalized(Id);
            Dispose(false);
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Initializes a new instance of the <see cref="SegmentMemoryStreamSlim"/> class with an
        /// expandable capacity initialized to zero.
        /// </summary>
        /// <param name="maximumCapacity">
        /// The maximum capacity that was configured when this stream was created.
        /// </param>
        public SegmentMemoryStreamSlim (long maximumCapacity) :
            this(maximumCapacity, null)
        {
            IsOpen = true;
            CanWriteInternal = true;
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Initializes a new instance of the <see cref="SegmentMemoryStreamSlim"/> class with an
        /// expandable capacity initialized as specified.
        /// </summary>
        /// <param name="maximumCapacity">
        /// The maximum capacity that was configured when this stream was created.
        /// </param>
        /// <param name="capacity">
        /// The initial capacity of the stream in bytes.
        /// </param>
        public SegmentMemoryStreamSlim (long maximumCapacity, int capacity) :
            this(maximumCapacity, capacity, null)
        {
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Initializes a new instance of the <see cref="SegmentMemoryStreamSlim"/> class with an
        /// expandable capacity initialized to zero.
        /// </summary>
        /// <param name="maximumCapacity">
        /// The maximum capacity that was configured when this stream was created.
        /// </param>
        /// <param name="options">
        /// The options for configuring the <see cref="MemoryStreamSlim"/> settings.
        /// </param>
        public SegmentMemoryStreamSlim (long maximumCapacity, MemoryStreamSlimOptions? options) :
            base(maximumCapacity, MemoryStreamSlimMode.Dynamic, options)
        {
            IsOpen = true;
            CanWriteInternal = true;
            UtilsEventSource.Log.MemoryStreamSlimCreated(Id, maximumCapacity, MemoryStreamSlimMode.Dynamic, Settings);
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Initializes a new instance of the <see cref="SegmentMemoryStreamSlim"/> class with an
        /// expandable capacity initialized to zero.
        /// </summary>
        /// <param name="maximumCapacity">
        /// The maximum capacity that was configured when this stream was created.
        /// </param>
        /// <param name="settings">
        /// The stream instance settings.
        /// </param>
        public SegmentMemoryStreamSlim (long maximumCapacity, in MemoryStreamSlimSettings settings) :
            base(maximumCapacity, MemoryStreamSlimMode.Dynamic, settings)
        {
            IsOpen = true;
            CanWriteInternal = true;
            UtilsEventSource.Log.MemoryStreamSlimCreated(Id, maximumCapacity, MemoryStreamSlimMode.Dynamic, Settings);
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Initializes a new instance of the <see cref="SegmentMemoryStreamSlim"/> class with an
        /// expandable capacity initialized as specified.
        /// </summary>
        /// <param name="maximumCapacity">
        /// The maximum capacity that was configured when this stream was created.
        /// </param>
        /// <param name="capacity">
        /// The initial capacity of the stream in bytes.
        /// </param>
        /// <param name="options">
        /// The options for configuring the <see cref="MemoryStreamSlim"/> settings.
        /// </param>
        public SegmentMemoryStreamSlim (long maximumCapacity, int capacity, MemoryStreamSlimOptions? options) :
            this(maximumCapacity, options)
        {
            if (capacity < 0)
                ThrowHelper.ThrowArgumentOutOfRangeException_NeedNonNegNum(nameof(capacity));
            if (capacity > MaximumCapacity)
                ThrowHelper.ThrowArgumentOutOfRangeException_NeedBetween(nameof(capacity), 1, MaximumCapacity);
            EnsureCapacity(capacity);
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Initializes a new instance of the <see cref="SegmentMemoryStreamSlim"/> class with an
        /// expandable capacity initialized as specified.
        /// </summary>
        /// <param name="maximumCapacity">
        /// The maximum capacity that was configured when this stream was created.
        /// </param>
        /// <param name="capacity">
        /// The initial capacity of the stream in bytes.
        /// </param>
        /// <param name="settings">
        /// The stream instance settings.
        /// </param>
        public SegmentMemoryStreamSlim (long maximumCapacity, int capacity, in MemoryStreamSlimSettings settings) :
            this(maximumCapacity, settings)
        {
            if (capacity < 0)
                ThrowHelper.ThrowArgumentOutOfRangeException_NeedNonNegNum(nameof(capacity));
            if (capacity > MaximumCapacity)
                ThrowHelper.ThrowArgumentOutOfRangeException_NeedBetween(nameof(capacity), 1, MaximumCapacity);
            EnsureCapacity(capacity);
        }
        //--------------------------------------------------------------------------------

        #region Overrides of MemoryStreamSlim

        //--------------------------------------------------------------------------------
        /// <inheritdoc />
        public override async Task CopyToAsync (Stream destination, int bufferSize, CancellationToken cancellationToken)
        {
            // We don't use bufferSize (right now), but for consistency, check all the 
            // parameters and throw the appropriate exceptions.
            ValidateCopyToArguments(destination, bufferSize);
            EnsureNotClosed();

            // If the buffer size is a conservative size to avoid the LOH, we can bump it up considerably
            // because we don't allocate a  buffer for copying, we are just copying the data from the buffers
            // we already have in memory. But, since we are doing an async copy, it could make sense to 
            // not make any one single operation too large, so we'll keep it at a reasonable size.
            bufferSize = Math.Max(StandardAsyncCopyBufferSize, bufferSize);

            // If we have no data, then we have nothing to copy
            int bytesToCopy = LengthInternal - PositionInternal;
            if (bytesToCopy <= 0)
                return;

            if (destination is MemoryStreamSlim or MemoryStream)
            {
                // We can optimize the copy operation by copying synchronously instead of asynchronously
                await CopyToSyncAsAsync(destination, bufferSize, cancellationToken);
                return;
            }

            VerifyCurrentBuffer();
            // If we have a small buffer, then we can just copy the remaining data from the small buffer
            if (_currentBufferInfo.IsSmallBuffer)
            {
                await destination.WriteAsync(_currentBufferInfo.Buffer.RawBuffer!, _currentBufferOffset, bytesToCopy, cancellationToken).ConfigureAwait(false);
                // Update the position and the current buffer information
                OffsetPositionAndSetCurrentBuffer(bytesToCopy);
                return;
            }

            // If we have a buffer list, then we need to copy the data from the standard buffers
            int bufferOffset = _currentBufferOffset;   // Offset into the buffer being copied from
            int bufferIndex = _currentBufferInfo.Index;
            int newPosition = VirtualPosition;

            // Set up the first write task
            // We are not using a small buffer and length is not zero, so we must have a buffer list
            SegmentBuffer currentBuffer = _bufferList![bufferIndex++].SegmentBuffer;
            int bufferRemainingCount = currentBuffer.Length - bufferOffset;
            // Limit the copy length to the buffer size, the remaining data in the buffer, and the remaining data to copy
            int copyLength = Math.Min(bufferSize, Math.Min(bufferRemainingCount, bytesToCopy));

            MemorySegment memorySegment = currentBuffer.MemorySegment;
            ReadOnlyMemory<byte> writeMemory = memorySegment.AsReadOnlyMemory().Slice(bufferOffset, copyLength);
            // Get the first write task
            ValueTask writeTask = destination.WriteAsync(writeMemory, cancellationToken);
            bytesToCopy -= copyLength;
            newPosition += copyLength;

            while (bytesToCopy > 0)
            {
                // We loop through the buffers and write the data to the destination stream by doing the
                // setup for the next write as the previous write is running asynchronously.

                // Check if we were limited from copying all the bytes in the buffer (based on the buffer size)
                if (copyLength < bufferRemainingCount)
                {
                    // Move the buffer offset
                    bufferOffset += copyLength;
                    bufferRemainingCount = currentBuffer.Length - bufferOffset;
                }
                else
                {
                    // We need to move to the next buffer
                    bufferOffset = 0;
                    currentBuffer = _bufferList![bufferIndex++].SegmentBuffer;
                    bufferRemainingCount = currentBuffer.Length;
                }
                copyLength = Math.Min(bufferSize, Math.Min(bufferRemainingCount, bytesToCopy));

                memorySegment = currentBuffer.MemorySegment;
                writeMemory = memorySegment.AsReadOnlyMemory().Slice(bufferOffset, copyLength);

                // Await the previous write task
                await writeTask.ConfigureAwait(false);
                // Move on to the next write task
                writeTask = destination.WriteAsync(writeMemory, cancellationToken);
                bytesToCopy -= copyLength;
                newPosition += copyLength;
            }
            // Await the final write task
            await writeTask.ConfigureAwait(false);
            SetPositionAndCurrentBuffer(newPosition);
        }
        //--------------------------------------------------------------------------------
        /// <inheritdoc />
        protected override void SetCapacity (int capacityValue)
        {
            if (CapacityInternal == capacityValue)
                return;
            if (capacityValue < CapacityInternal)
                ReduceCapacity(capacityValue);
            else
                EnsureCapacity(capacityValue);
        }
        //--------------------------------------------------------------------------------
        /// <inheritdoc />
        [DoesNotReturn]
        public override byte[] GetBuffer ()
        {
            ThrowHelper.ThrowNotSupportedException_InvalidModeStreamStream(MemoryStreamSlimMode.Dynamic);
            return default;
        }
        //--------------------------------------------------------------------------------
        /// <inheritdoc />
        public override bool TryGetBuffer (out ArraySegment<byte> buffer)
        {
            buffer = default;
            return false;
        }
        //--------------------------------------------------------------------------------

        #endregion Overrides of MemoryStreamSlim


        #region Overrides of MemoryStream

        //--------------------------------------------------------------------------------
        /// <inheritdoc />
        public override long Length => LengthInternal;
        //--------------------------------------------------------------------------------
        /// <inheritdoc />
        public override long Position
        {
            get => VirtualPosition;
            set
            {
                if (value < 0)
                    ThrowHelper.ThrowArgumentOutOfRangeException_NeedNonNegNum(nameof(value));
                if (value > MaximumCapacity)
                    ThrowHelper.ThrowArgumentOutOfRangeException_NeedBetween(nameof(value), 0, MaximumCapacity);
                VirtualPosition = (int)value;
            }
        }
        //--------------------------------------------------------------------------------
        /// <inheritdoc />
        public override unsafe int Read (byte[] buffer, int offset, int count)
        {
            ValidateBufferArguments(buffer, offset, count);
            EnsureNotClosed();

            if (count <= 0)
                return 0;

            if (LengthInternal <= PositionInternal)
                return 0;
            VerifyCurrentBuffer();
            int bytesLeft = LengthInternal - VirtualPosition;
            if (count > bytesLeft)
                count = bytesLeft;
            // If we are going to traverse multiple buffers, then we take a quicker approach
            int bufferBytesLeft = CurrentBufferBytesLeft;

            if (count > bufferBytesLeft)
            {
                return ReadMultiBuffer(buffer, offset, count);
            }

            // Copy the data from the current buffer to the output buffer
            MemorySegment currentMemorySegment = _currentBufferInfo.Buffer.MemorySegment;
            int sourceBufferOffset = _currentBufferOffset + currentMemorySegment.Offset;
            if (currentMemorySegment.IsNative)
            {
                fixed (byte* pWriteBuffer = buffer)
                {
                    Buffer.MemoryCopy(currentMemorySegment.NativePointer + sourceBufferOffset, pWriteBuffer + offset, buffer.Length - offset, count);
                }
            }
            else
            {
                fixed (byte* pReadBuffer = currentMemorySegment.Array!, pWriteBuffer = buffer)
                {
                    Buffer.MemoryCopy(pReadBuffer + sourceBufferOffset, pWriteBuffer + offset, buffer.Length - offset, count);
                }
            }
            // Update the position and the current buffer information
            OffsetPositionAndSetCurrentBuffer(count);
            return count;
        }
        //--------------------------------------------------------------------------------
        /// <inheritdoc />
        public override int Read (Span<byte> destinationBuffer)
        {
            EnsureNotClosed();

            int copyCount = Math.Min(LengthInternal - VirtualPosition, destinationBuffer.Length);
            if (copyCount <= 0)
                return 0;

            if (LengthInternal <= PositionInternal)
                return 0;
            VerifyCurrentBuffer();
            int bytesLeft = LengthInternal - VirtualPosition;
            if (copyCount > bytesLeft)
                copyCount = bytesLeft;
            // If we are going to traverse multiple buffers, then we take a quicker approach
            int bufferBytesLeft = CurrentBufferBytesLeft;

            if (copyCount > bufferBytesLeft)
            {
                return ReadMultiBuffer(destinationBuffer, copyCount);
            }

            // Copy the data from the current buffer to the output buffer
            _currentBufferInfo.Buffer.AsSpan(_currentBufferOffset, copyCount).CopyTo(destinationBuffer);
            // Update the position and the current buffer information
            OffsetPositionAndSetCurrentBuffer(copyCount);
            return copyCount;
        }
        //--------------------------------------------------------------------------------
        /// <inheritdoc />
        public override int ReadByte ()
        {
            EnsureNotClosed();

            if (VirtualPosition >= LengthInternal)
                return -1;
            VerifyCurrentBuffer();

            // The below logic will work for either a small or standard buffer.
            int returnByte = _currentBufferInfo.Buffer[_currentBufferOffset];
            // Advance the position
            OffsetPositionAndSetCurrentBuffer(1);
            // Return the byte at the captured offset
            return returnByte;
        }
        //--------------------------------------------------------------------------------
        /// <inheritdoc />
        public override void CopyTo (Stream destination, int bufferSize)
        {
            // We don't use bufferSize (right now), but for consistency, check all the 
            // parameters and throw the appropriate exceptions.
            ValidateCopyToArguments(destination, bufferSize);
            EnsureNotClosed();
            // If we have no data, then we have nothing to copy
            int bytesToCopy = LengthInternal - PositionInternal;
            if (bytesToCopy <= 0)
                return;

            VerifyCurrentBuffer();
            // If we have a small buffer, then we can just copy the remaining data from the small buffer
            if (_currentBufferInfo.IsSmallBuffer)
            {
                destination.Write(_currentBufferInfo.Buffer.RawBuffer!, _currentBufferOffset, bytesToCopy);
                // Update the position and the current buffer information
                OffsetPositionAndSetCurrentBuffer(bytesToCopy);
                return;
            }
            // If we have a buffer list, then we need to copy the data from the standard buffers
            int bufferOffset = _currentBufferOffset;   // Offset into the buffer being copied from
            int bufferIndex = _currentBufferInfo.Index;
            int newPosition = VirtualPosition;
            while (bytesToCopy > 0)
            {
                // We are not using a small buffer and length is not zero, so we must have a buffer list
                SegmentBuffer currentBuffer = _bufferList![bufferIndex++].SegmentBuffer;
                int copyLength = Math.Min(currentBuffer.Length - bufferOffset, bytesToCopy);

                MemorySegment memorySegment = currentBuffer.MemorySegment;
                if (memorySegment.IsNative)
                {
                    destination.Write(memorySegment.AsSpan().Slice(bufferOffset, copyLength));
                }
                else
                {
                    destination.Write(memorySegment.Array!, memorySegment.Offset + bufferOffset, copyLength);
                }
                bytesToCopy -= copyLength;
                newPosition += copyLength;
                // For the remaining buffers, we will start at the beginning of the buffer
                bufferOffset = 0;
            }
            SetPositionAndCurrentBuffer(newPosition);
        }
        //--------------------------------------------------------------------------------
        /// <inheritdoc />
        public override long Seek (long offset, SeekOrigin origin)
        {
            EnsureNotClosed();

            switch (origin)
            {
                case SeekOrigin.Begin:
                    if (offset < 0)
                        ThrowHelper.ThrowArgumentOutOfRangeException_NeedNonNegNum(nameof(offset));
                    return SeekInternal(offset, 0);

                case SeekOrigin.Current:
                    return SeekInternal(offset, PositionInternal);

                case SeekOrigin.End:
                    return SeekInternal(offset, LengthInternal);

                default:
#pragma warning disable HAA0601
                    throw new ArgumentOutOfRangeException(nameof(origin), origin, null);
#pragma warning restore HAA0601
            }
        }
        //--------------------------------------------------------------------------------
        /// <inheritdoc />
        public override void SetLength (long value)
        {
            if (value == LengthInternal)
                return;
            if (value < 0)
                ThrowHelper.ThrowArgumentOutOfRangeException_NeedNonNegNum(nameof(value));
            if (value > MaximumCapacity)
                ThrowHelper.ThrowArgumentOutOfRangeException_NeedBetween(nameof(value), 0, MaximumCapacity);
            int newLength = (int)value;

            if (newLength > CapacityInternal)
            {
                // We need to ensure that we have the capacity for the new length
                EnsureCapacity(newLength, true);
                // Verify the current buffer in case that buffer is a small buffer. If we are using a buffer list,
                // then we don't really need to verify the current buffer, but it has likely been invalidated by
                // the EnsureCapacity call, so we will verify it anyway. There are cases where calling this while
                // using a buffer list could result in the current buffer still being invalid, but with the buffer list
                // case, we are clearing the buffers directly below anyway.
                // BTW: The case where the buffer could still be invalid is when the current position is right beyond
                // the last buffer in the list (Position == Length == Capacity).
                VerifyCurrentBuffer();
            }
            // If the new length is greater than the current length, then we need to zero out the
            // remainder of our current buffer. Any new buffers will be zeroed out when they are allocated.
            if (newLength > LengthInternal)
            {
                if (_currentBufferInfo.IsSmallBuffer)
                {
                    Array.Clear(_currentBufferInfo.Buffer.RawBuffer!, LengthInternal, newLength - LengthInternal);
                }
                else if (_bufferList is not null)
                {
                    // Get the buffer index and offset for the offset value of the current length.
                    (int lengthBufferIndex, int lengthBufferOffset) = GetBufferInfoForOffset(LengthInternal);
                    // Get the buffer index and offset for the offset value of the new length. This could 
                    // be the current buffer, but doesn't have to be if the current position is somewhere
                    // much earlier than the length offset.
                    SegmentBuffer updateBuffer = _bufferList[lengthBufferIndex].SegmentBuffer;
                    int clearLength = Math.Min((int)(value - LengthInternal), updateBuffer.Length - lengthBufferOffset);
                    updateBuffer.Slice(lengthBufferOffset, clearLength).Clear();
                    // Any other new buffers will be zeroed out when they are allocated, but any buffers that
                    // are currently in the list after the current buffer will need to be zeroed out.
                    for (int clearBufferIndex = lengthBufferIndex + 1; clearBufferIndex < _bufferList.Count; clearBufferIndex++)
                    {
                        _bufferList[clearBufferIndex].SegmentBuffer.Clear();
                    }
                }
            }
            LengthInternal = newLength;
        }
        //--------------------------------------------------------------------------------
        /// <inheritdoc />
        public override byte[] ToArray ()
        {
            EnsureNotClosed();
            // If we have no data, then return an empty array
            int bytesToCopy = LengthInternal;
            if (bytesToCopy == 0)
                return [];

            // If we have a small buffer, then we can just copy the data from the small buffer
            byte[] returnArray = GC.AllocateUninitializedArray<byte>(bytesToCopy);
            // Report the ToArray operation
            UtilsEventSource.Log.MemoryStreamSlimToArray(Id, bytesToCopy);
            Debug.Assert(returnArray.Length == bytesToCopy, "returnArray.Length != bytesToCopy");
            if (_currentBufferInfo.IsSmallBuffer)
            {
                _currentBufferInfo.Buffer[..bytesToCopy].CopyTo(returnArray);
                return returnArray;
            }
            // If we have a buffer list, then we need to copy the data from the buffers
            int bufferOffset = 0;
            int bufferIndex = 0;
            // We are not using a small buffer and length is not zero, so we must have a buffer list
            while (bytesToCopy > 0)
            {
                SegmentBuffer currentBuffer = _bufferList![bufferIndex++].SegmentBuffer;
                int copyLength = Math.Min(currentBuffer.Length, bytesToCopy);
                currentBuffer[..copyLength].CopyTo(returnArray.AsSpan(bufferOffset, copyLength));
                bufferOffset += copyLength;
                bytesToCopy -= copyLength;
            }
            return returnArray;
        }
        //--------------------------------------------------------------------------------
        /// <inheritdoc />
        public override unsafe void Write (byte[] buffer, int offset, int count)
        {
            ValidateBufferArguments(buffer, offset, count);
            EnsureWriteable();

            if (count <= 0)
                return;
            // Make sure we have enough capacity
            long endPosition = VirtualPosition + count;
            ValidateLength(endPosition);

            if (endPosition > CapacityInternal)
            {
                EnsureCapacity(endPosition);
            }
            VerifyCurrentBuffer();
            // Clear the buffer(s) between the current length and current position
            if (PositionInternal > LengthInternal)
                ClearLengthPositionGap(PositionInternal);
            // If we are going to traverse multiple buffers, then we take a quicker approach
            int bufferBytesLeft = CurrentBufferBytesLeft;

            if (count > bufferBytesLeft)
            {
                WriteMultiBuffer(buffer, offset, count);
                return;
            }

            // Copy the data to the stream
            // Write the data to the current buffer
            MemorySegment currentMemorySegment = _currentBufferInfo.Buffer.MemorySegment;
            int destinationBufferOffset = _currentBufferOffset + currentMemorySegment.Offset;
            if (currentMemorySegment.IsNative)
            {
                fixed (byte* pWriteBuffer = buffer)
                {
                    Buffer.MemoryCopy(pWriteBuffer + offset, currentMemorySegment.NativePointer + destinationBufferOffset,
                        _currentBufferInfo.Buffer.Length - _currentBufferOffset, count);
                }
            }
            else
            {
                fixed (byte* pReadBuffer = buffer, pWriteBuffer = currentMemorySegment.Array!)
                {
                    Buffer.MemoryCopy(pReadBuffer + offset, pWriteBuffer + destinationBufferOffset,
                        _currentBufferInfo.Buffer.Length - _currentBufferOffset, count);
                }
            }
            // Update the position and count values
            int newPosition = VirtualPosition + count;

            if (newPosition > LengthInternal)
                LengthInternal = newPosition;
            // Set the position and current buffer
            OffsetPositionAndSetCurrentBuffer(count);
        }
        //--------------------------------------------------------------------------------
        /// <inheritdoc />
        public override void Write (ReadOnlySpan<byte> buffer)
        {
            EnsureWriteable();

            // Make sure we have enough capacity
            int copyCount = buffer.Length;
            if (copyCount <= 0)
                return;
            long endPosition = VirtualPosition + copyCount;
            ValidateLength(endPosition);

            if (endPosition > CapacityInternal)
            {
                EnsureCapacity(endPosition);
            }
            VerifyCurrentBuffer();
            // Clear the buffer(s) between the current length and current position
            if (PositionInternal > LengthInternal)
                ClearLengthPositionGap(PositionInternal);
            // If we are going to traverse multiple buffers, then we take a quicker approach
            int bufferBytesLeft = CurrentBufferBytesLeft;

            if (copyCount > bufferBytesLeft)
            {
                WriteMultiBuffer(buffer, copyCount);
                return;
            }

            // Copy the data to the stream
            // Write the data to the current buffer
            buffer.CopyTo(_currentBufferInfo.Buffer.AsSpan(_currentBufferOffset, copyCount));
            // Update the position and count values
            int newPosition = VirtualPosition + copyCount;

            if (newPosition > LengthInternal)
                LengthInternal = newPosition;
            // Set the position and current buffer
            OffsetPositionAndSetCurrentBuffer(copyCount);
        }
        //--------------------------------------------------------------------------------
        /// <inheritdoc />
        public override void WriteTo (Stream stream)
        {
            EnsureNotClosed();
            // If we have no data, then simply return
            int bytesToCopy = LengthInternal;
            if (bytesToCopy == 0)
                return;
            // If we have a small buffer, then we can just copy the data from the small buffer
            if (_currentBufferInfo.IsSmallBuffer)
            {
                stream.Write(_currentBufferInfo.Buffer.RawBuffer!, 0, bytesToCopy);
                return;
            }
            // If we have a buffer list, then we need to copy the data from the buffers. We are 
            // copying the entire contents, and not changing the position, etc. for WriteTo()
            int bufferIndex = 0;
            while (bytesToCopy > 0)
            {
                // We are not using a small buffer and length is not zero, so we must have a buffer list
                SegmentBuffer currentBuffer = _bufferList![bufferIndex++].SegmentBuffer;
                int copyLength = Math.Min(currentBuffer.Length, bytesToCopy);
                MemorySegment bufferSegment = currentBuffer.MemorySegment;
                if (bufferSegment.IsNative)
                {
                    stream.Write(bufferSegment.AsSpan()[..copyLength]);
                }
                else
                {
                    stream.Write(bufferSegment.Array!, bufferSegment.Offset, copyLength);
                }
                bytesToCopy -= copyLength;
            }
        }
        //--------------------------------------------------------------------------------
        /// <inheritdoc />
        public override void WriteByte (byte value)
        {
            EnsureWriteable();

            // Make sure we have enough capacity
            long endPosition = VirtualPosition + 1;
            ValidateLength(endPosition);

            if (endPosition > CapacityInternal)
            {
                EnsureCapacity(endPosition);
            }
            VerifyCurrentBuffer();
            // Clear the buffer(s) between the current length and current position
            if (PositionInternal > LengthInternal)
                ClearLengthPositionGap(PositionInternal);
            // Write the byte to the stream buffer
            _currentBufferInfo.Buffer.Span[_currentBufferOffset] = value;
            if (endPosition > LengthInternal)
                LengthInternal = (int)endPosition;
            OffsetPositionAndSetCurrentBuffer(1);
        }
        //--------------------------------------------------------------------------------
        /// <inheritdoc />
        /// <exception cref="ObjectDisposedException">
        /// The stream has been disposed.
        /// </exception>
        protected override void Dispose (bool disposing)
        {
            if (IsDisposed)
            {
                if (!disposing) return; // Somehow we are being called from the finalizer after we've been disposed
                throw new ObjectDisposedException(nameof(MemoryStreamSlim));
            }
            if (disposing)
            {
                UtilsEventSource.Log.MemoryStreamSlimDisposed(Id);
                GC.SuppressFinalize(this);
            }

            IsDisposed = true;
            AdjustActiveStreamCount(false);
            IsOpen = false;
            CanWriteInternal = false;
            // Be thorough about releasing all resources
            SetCurrentBufferInfo(CurrentBufferInfo.Empty);
            if (_bufferList is null)
                return;
            for (int bufferIndex = 0; bufferIndex < _bufferList.Count; bufferIndex++)
            {
                ReleaseStandardBuffer(_bufferList[bufferIndex].SegmentBuffer);
            }
            // If we are disposing, then we can clear and cache the buffer list
            if (disposing)
                StoreBufferList(_bufferList);
            _bufferList = null;
        }
        //--------------------------------------------------------------------------------

        #endregion Overrides of MemoryStream
    }
    //################################################################################
}
