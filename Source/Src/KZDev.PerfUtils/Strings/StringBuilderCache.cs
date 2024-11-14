using System.Collections.Concurrent;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

using KZDev.PerfUtils.Helpers;

namespace KZDev.PerfUtils
{
    /// <summary>
    /// Provides cached reusable <see cref="StringBuilder"/> instances.
    /// </summary>
    public static class StringBuilderCache
    {
        //================================================================================

        /// <summary>
        /// The maximum number of cached <see cref="StringBuilder"/> instances that will be
        /// in the global cache for each cached capacity size.
        /// </summary>
        /// <remarks>
        /// We don't keep a global cache in a browser environment.
        /// </remarks>
        internal static readonly int MaxGlobalCacheCount = OperatingSystem.IsBrowser() ? 0 : 
            Math.Min(4, Math.Max(2, Environment.ProcessorCount / 2));

        /// <summary>
        /// The maximum capacity of a <see cref="StringBuilder"/> instance that can be cached.
        /// </summary>
#if NOT_PACKAGING
        internal
#else
        private
#endif
        const int MaxCachedCapacity = 2048;

        /// <summary>
        /// The default capacity of a <see cref="StringBuilder"/> instance when a specific capacity
        /// is not provided.
        /// </summary>
        /// <remarks>
        /// This it the value of <see cref="StringBuilder"/>.DefaultCapacity.
        /// </remarks>
#if NOT_PACKAGING
        internal
#else
        private
#endif  
        const int DefaultCapacity = 16;

        /// <summary>
        /// A set of global-level cached instances of <see cref="StringBuilder"/> based on 
        /// capacity. We will keep at most one instance for every other processor per capacity.
        /// </summary>
#if NOT_PACKAGING
        internal
#else
        private
#endif
        // ReSharper disable once InconsistentNaming
        static ConcurrentBag<StringBuilder>?[]? _globalCache;

        /// <summary>
        /// A set of thread-level cached instances of <see cref="StringBuilder"/> based on 
        /// capacity.
        /// </summary>
        [ThreadStatic]
#if NOT_PACKAGING
        internal
#else
        private
#endif
        // ReSharper disable once InconsistentNaming
        static StringBuilder?[]? _threadCache;

        //--------------------------------------------------------------------------------
        /// <summary>
        /// Gets the reference to the global cache of <see cref="StringBuilder"/> instances.
        /// </summary>
        private static ConcurrentBag<StringBuilder>?[] GlobalCache
        {
            get
            {
                ConcurrentBag<StringBuilder>?[]? returnCache = _globalCache;
                if (returnCache is not null)
                {
                    return returnCache;
                }
                Interlocked.CompareExchange(ref _globalCache, new ConcurrentBag<StringBuilder>?[GetCacheIndex(MaxCachedCapacity) + 1], null);
                return _globalCache;
            }
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Gets the global cache bag for the specified capacity index if it exists, and
        /// creates it if needed.
        /// </summary>
        /// <param name="capacityIndex">
        /// The index of the capacity for the cache bag.
        /// </param>
        /// <param name="createIfNeeded">
        /// Indicates if a cache bag should be created if it does not exist.
        /// </param>
        /// <returns>
        /// The cache bag for the specified capacity index if it exists or if it was created.
        /// </returns>
        private static ConcurrentBag<StringBuilder>? GetGlobalCacheBag (int capacityIndex, bool createIfNeeded = false)
        {
            if (MaxGlobalCacheCount == 0)
                return null;
            if (!createIfNeeded)
            {
                ConcurrentBag<StringBuilder>?[]? localGlobalCache = _globalCache;
                return localGlobalCache?[capacityIndex];
            }

            // Getting the GlobalCache property will create the cache list if needed.
            ConcurrentBag<StringBuilder>?[] globalCache = GlobalCache;
            ConcurrentBag<StringBuilder>? returnBag = globalCache[capacityIndex];
            if (returnBag is not null)
            {
                return returnBag;
            }
            Interlocked.CompareExchange(ref globalCache[capacityIndex], new ConcurrentBag<StringBuilder>(), null);
            return globalCache[capacityIndex];
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Gets the index in the cached instances that should be used for string builder instances
        /// of the given capacity with the provided capacity being less than or equal to the 
        /// current capacity of the string builder at that index, nothing greater than the 
        /// given maximum capacity and all instances below the default capacity being stored in the first index.
        /// </summary>
        /// <param name="capacity">
        /// </param>
        /// <returns>
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if NOT_PACKAGING
        internal
#else
        private
#endif
        static int GetCacheIndex (int capacity) =>
            capacity > MaxCachedCapacity ? -1 : BitOperations.Log2((uint)capacity - 1 | 15) - 3;
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Gets the thread-level cached instances of <see cref="StringBuilder"/>.
        /// </summary>
        private static StringBuilder?[] ThreadCachedInstances =>
            _threadCache ??= new StringBuilder?[GetCacheIndex(MaxCachedCapacity) + 1];
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Put the <see cref="StringBuilder"/> instance into the global cache at the specified index.
        /// </summary>
        /// <param name="storeIndex">
        /// The target index to store the instance in.
        /// </param>
        /// <param name="stringBuilder">
        /// The string builder instance to return.
        /// </param>
        private static void ReleaseToGlobalIndex (StringBuilder stringBuilder, int storeIndex)
        {
            ConcurrentBag<StringBuilder>? cacheBag = GetGlobalCacheBag(storeIndex, true);
            if (cacheBag is null || (cacheBag.Count >= MaxGlobalCacheCount))
            {
                // Not going to cache this instance
                return;
            }
            // By checking the size first, we may exceed the maximum count, but that is
            // really not a big issue since we are only caching a small number of instances
            // and as threads come and go, globally cached instances will be pulled from the 
            // list to be used when thread static instances are initially available.
            cacheBag.Add(stringBuilder);
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Put the <see cref="StringBuilder"/> instance back into the passed cache list
        /// at the specified index (or lower). We are willing to keep larger instances
        /// around in local lists since they are more expensive to allocate, but are
        /// small enough that holding them in a local list is minimal impact.
        /// </summary>
        /// <param name="cachedInstances">
        /// The list of cached instances to store the instance in.
        /// </param>
        /// <param name="storeIndex">
        /// The target index to store the instance in.
        /// </param>
        /// <param name="stringBuilder">
        /// The string builder instance to return.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the instance was stored in the cache list; otherwise <see langword="false"/>.
        /// </returns>
        private static bool ReleaseToIndexOrLower (StringBuilder stringBuilder,
            StringBuilder?[] cachedInstances, int storeIndex)
        {
            if (storeIndex < 0)
                return false;
            while (true)
            {
                StringBuilder? currentCachedInstance = cachedInstances[storeIndex];
                if (currentCachedInstance is null)
                {
                    cachedInstances[storeIndex] = stringBuilder;
                    return true;
                }

                // Larger capacity instances are more expensive to allocate, and we know that
                // the maximum capacity we will cache is relatively small (<= MaxCachedCapacity) AND are only
                // cached at the thread level (which often come and go frequently), so we
                // are willing to store the larger capacity instances in lower slots and keep
                // them around since they have already been allocated.
                if (currentCachedInstance.Capacity >= stringBuilder.Capacity)
                {
                    // We are willing to store instances in lower slots because we primarily
                    // want to eliminate instantiating new instances, and this is better than
                    // just letting this instance get GC'd.
                    if (storeIndex == 0) return false;
                    storeIndex--;
                    continue;
                }

                // We will place the released instance in the current slot and check if we can
                // move the currently cached instance to a lower slot.
                if (!ReleaseToIndexOrLower(currentCachedInstance, cachedInstances, storeIndex - 1))
                    return false;
                cachedInstances[storeIndex] = stringBuilder;
                return true;
            }
            return false;
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Put the <see cref="StringBuilder"/> instance back into the cache at the specified index.
        /// </summary>
        /// <param name="cachedInstances">
        /// The list of cached instances to store the instance in.
        /// </param>
        /// <param name="storeIndex">
        /// The target index to store the instance in.
        /// </param>
        /// <param name="stringBuilder">
        /// The string builder instance to return.
        /// </param>
        private static void ReleaseToIndex (StringBuilder stringBuilder, StringBuilder?[] cachedInstances,
            int storeIndex)
        {
            if (storeIndex < 0)
                return;
            if (ReleaseToIndexOrLower(stringBuilder, cachedInstances, storeIndex))
                return;
            ReleaseToGlobalIndex(stringBuilder, storeIndex);
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Put the <see cref="StringBuilder"/> instance back into the cache at the specified index.
        /// </summary>
        /// <param name="storeIndex">
        /// The target index to store the instance in.
        /// </param>
        /// <param name="stringBuilder">
        /// The string builder instance to return.
        /// </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ReleaseToIndex (StringBuilder stringBuilder, int storeIndex) =>
            ReleaseToIndex(stringBuilder, ThreadCachedInstances, storeIndex);
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Get a StringBuilder for the specified capacity.
        /// </summary>
        /// <param name="capacity">
        /// The requested capacity of the <see cref="StringBuilder"/> instance. The actual 
        /// capacity of the returned instance could be larger than the requested capacity.
        /// </param>
        /// <param name="cacheIndex">
        /// The index into the cache based on the capacity of the <see cref="StringBuilder"/> instance.
        /// </param>
        private static StringBuilder AcquireFromGlobal (int capacity, int cacheIndex)
        {
            if (Volatile.Read(ref _globalCache) is null)
                return new StringBuilder(capacity);
            // If we are looking in the global cache, then we know that the local cache
            // has been checked (and exists), so we will use the size of the local cache
            // to determine the index count we have and check at higher indexes for larger
            // instances if we need them.
            Debug.Assert(_threadCache is not null);
            int cacheBagCount = _threadCache!.Length;
            while (cacheIndex < cacheBagCount)
            {
                // Get the bag that the instance should be in
                ConcurrentBag<StringBuilder>? globalBag = GetGlobalCacheBag(cacheIndex);
                if (globalBag?.TryTake(out StringBuilder? cachedInstance) ?? false)
                {
                    cachedInstance.Clear();
                    return cachedInstance;
                }
                // Try the next larger capacity group
                cacheIndex++;
            }
            return new StringBuilder(capacity);
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Get a StringBuilder for the specified capacity.
        /// </summary>
        /// <param name="capacity">
        /// The requested capacity of the <see cref="StringBuilder"/> instance. The actual 
        /// capacity of the returned instance could be larger than the requested capacity.
        /// </param>
        public static StringBuilder Acquire (int capacity = DefaultCapacity)
        {
            if (capacity < 0)
                ThrowHelper.ThrowArgumentOutOfRangeException_NeedNonNegNum(nameof(capacity));
            // Normalize the capacity to the default capacity if it is less than the default capacity.
            if (capacity < DefaultCapacity)
                capacity = DefaultCapacity;
            if (capacity > MaxCachedCapacity)
                return new StringBuilder(capacity);

            // First index is the first index of the cached list we will check, but we can 
            // also return instances that currently have a larger capacity.
            int firstIndex = GetCacheIndex(capacity);
            if (firstIndex < 0)
                return new StringBuilder(capacity);

            if (_threadCache is null)
                return AcquireFromGlobal(capacity, firstIndex);

            // Get a local reference to the cache.
            StringBuilder?[] threadCache = _threadCache;
            // Check the larger instances now
            for (int checkIndex = firstIndex; checkIndex < threadCache.Length; checkIndex++)
            {
                StringBuilder? cachedInstance = threadCache[checkIndex];
                if (cachedInstance is null)
                    continue;

                threadCache[checkIndex] = null;
                cachedInstance.Clear();
                return cachedInstance;
            }
            // Then resort to the global cache.
            return AcquireFromGlobal(capacity, firstIndex);
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Put the <see cref="StringBuilder"/> instance back into the cache.
        /// </summary>
        public static void Release (StringBuilder stringBuilder)
        {
            if (stringBuilder is null)
                throw new ArgumentNullException(nameof(stringBuilder));
            // We won't cache larger instances
            if (stringBuilder.Capacity > MaxCachedCapacity)
                return;
            ReleaseToIndex(stringBuilder, GetCacheIndex(stringBuilder.Capacity));
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Returns the <paramref name="stringBuilder"/> instance to the cache and returns 
        /// the string value of the <paramref name="stringBuilder"/>.
        /// </summary>
        /// <param name="stringBuilder">
        /// The <see cref="StringBuilder"/> instance to release and get the string value of.
        /// </param>
        public static string GetStringAndRelease (StringBuilder stringBuilder)
        {
            if (stringBuilder is null)
                throw new ArgumentNullException(nameof(stringBuilder));
            string result = stringBuilder.ToString();
            Release(stringBuilder);
            return result;
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Gets a <see cref="StringBuilderScope"/> instance that provides a cached
        /// <see cref="StringBuilder"/> instance and releases it back to the cache when
        /// the scope is disposed.
        /// </summary>
        /// <param name="capacity">
        /// The capacity of the <see cref="StringBuilder"/> instance to acquire.
        /// </param>
        /// <returns>
        /// A <see cref="StringBuilderScope"/> instance that provides a cached 
        /// <see cref="StringBuilder"/> instance.
        /// </returns>
        public static StringBuilderScope GetScope (int capacity = DefaultCapacity) =>
            new StringBuilderScope(capacity);
        //--------------------------------------------------------------------------------
    }
}
