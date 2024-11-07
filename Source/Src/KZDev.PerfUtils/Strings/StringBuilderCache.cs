﻿using System.Numerics;
using System.Text;

using KZDev.PerfUtils.Helpers;

namespace KZDev.PerfUtils
{
    /// <summary>
    /// Provides cached reusable <see cref="StringBuilder"/> instances.
    /// </summary>
    public static class StringBuilderCache
    {
        /// <summary>
        /// The maximum capacity of a <see cref="StringBuilder"/> instance that can be cached.
        /// </summary>
        internal const int MaxCachedCapacity = 2048;

        /// <summary>
        /// The default capacity of a <see cref="StringBuilder"/> instance when a specific capacity
        /// is not provided.
        /// </summary>
        /// <remarks>
        /// This it the value of <see cref="StringBuilder"/>.DefaultCapacity.
        /// </remarks>
        internal const int DefaultCapacity = 16;

        /// <summary>
        /// A set of thread-level cached instances of <see cref="StringBuilder"/> based on 
        /// capacity.
        /// </summary>
        [ThreadStatic]
        private static StringBuilder?[]? _threadCachedInstances;

        //--------------------------------------------------------------------------------
        /// <summary>
        /// Gets the thread-level cached instances of <see cref="StringBuilder"/>.
        /// </summary>
        private static StringBuilder?[] ThreadCachedInstances =>
            _threadCachedInstances ??= new StringBuilder?[GetCacheIndex(MaxCachedCapacity) + 1];
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
        private static int GetCacheIndex (int capacity) =>
            capacity > MaxCachedCapacity ? -1 : BitOperations.Log2((uint)capacity - 1 | 15) - 3;
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
            if ((capacity > MaxCachedCapacity) || (_threadCachedInstances is null))
                return new StringBuilder(capacity);

            // First index is the first index of the cached list we will check, but we can 
            // also return instances that currently have a larger capacity.
            int firstIndex = GetCacheIndex(capacity);
            if (firstIndex < 0)
                return new StringBuilder(capacity);
            for (int checkIndex = firstIndex; checkIndex < _threadCachedInstances.Length; checkIndex++)
            {
                StringBuilder? cachedInstance = _threadCachedInstances[checkIndex];
                if ((cachedInstance is null) || (capacity > cachedInstance.Capacity))
                    continue;

                _threadCachedInstances[checkIndex] = null;
                cachedInstance.Clear();
                return cachedInstance;
            }
            return new StringBuilder(capacity);
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
            int saveIndex = GetCacheIndex(stringBuilder.Capacity);
            StringBuilder?[] cachedInstances = ThreadCachedInstances;
            StringBuilder? currentCachedInstance = cachedInstances[saveIndex];
            // Keep the larger capacity instance.
            if (currentCachedInstance is not null)
            {
                if (currentCachedInstance.Capacity >= stringBuilder.Capacity)
                {
                    // We are willing to store instances in lower slots because we primarily
                    // want to eliminate instantiating new instances, and this is better than
                    // just letting this instance get GC'd.
                    if (saveIndex == 0)
                        return;
                    cachedInstances[--saveIndex] ??= stringBuilder;
                    return;
                }
                // We will place the released instance in the current slot and check if we can
                // move the currently cached instance to a lower slot.
                if (saveIndex > 0)
                {
                    // Let the previous one in the lower index (very likely smaller), be GC'd
                    cachedInstances[--saveIndex] = currentCachedInstance;
                }
            }
            cachedInstances[saveIndex] = stringBuilder;
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
    }
}
