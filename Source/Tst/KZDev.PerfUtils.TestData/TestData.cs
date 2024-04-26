// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Collections.Immutable;

namespace KZDev.PerfUtils.Tests
{
    //################################################################################
    /// <summary>
    /// A base class that provides methods for generating random test data.
    /// </summary>
    public abstract partial class TestData
    {
        /// <summary>
        /// The random number generator we will use for this test class.
        /// </summary>
        private IRandomSource? _randomSource;

        //--------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the random number generator to use for this test class.
        /// </summary>
        protected virtual IRandomSource RandomSource
        {
            get
            {
                IRandomSource? currentSource = Volatile.Read(ref _randomSource);
                if (currentSource is not null)
                    return currentSource;
                IRandomSource newSource = new SecureRandomSource();
                return Interlocked.CompareExchange(ref _randomSource, newSource, null) is null ? newSource : Volatile.Read(ref _randomSource);
            }
            set
            {
                ArgumentNullException.ThrowIfNull(value);
                if (Interlocked.CompareExchange(ref _randomSource, value, null) is null)
                    return;
                throw new InvalidOperationException("The random source has already been set.");
            }
        }
        //--------------------------------------------------------------------------------

        //--------------------------------------------------------------------------------
        /// <summary>
        /// Returns a random positive integer in the range of [0...int.MaxValue)
        /// </summary>
        /// <returns>A random positive integer</returns>
        public int GetTestInteger () => GetTestInteger(RandomSource);
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Returns a random positive integer in the range of [0...maxValue)
        /// </summary>
        /// <param name="maxValue">
        /// The maximum value to return.
        /// </param>
        /// <returns>
        /// A random positive integer.
        /// </returns>
        public int GetTestInteger (int maxValue) => GetTestInteger(RandomSource, maxValue);
        //--------------------------------------------------------------------------------
        /// <summary>
        ///  Gets a random integer in the range of [minValue...maxValue)
        /// </summary>
        /// <param name="minValue">
        /// The minimum value to return.
        /// </param>
        /// <param name="maxValue">
        /// The maximum value to return.
        /// </param>
        /// <returns>
        /// A random integer within the passed range of values.
        /// </returns>
        public int GetTestInteger (int minValue, int maxValue) =>
            GetTestInteger(RandomSource, minValue, maxValue);
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Gets a collection of test integers in the range of [0...maxNumber)
        /// </summary>
        /// <param name="collectionSize">The size of collection set of integers to get</param>
        /// <param name="maxNumber">The maximum number value to include in the collection.</param>
        /// <returns>
        /// An immutable array of integers.
        /// </returns>
        public ImmutableArray<int> GetTestIntegerCollection (int collectionSize, int maxNumber) =>
            GetTestIntegerCollection(collectionSize, 0, maxNumber);
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Gets a collection of test integers in the range of [minNumber...maxNumber)
        /// </summary>
        /// <param name="collectionSize">The size of collection set of integers to get</param>
        /// <param name="maxNumber">The maximum number value to include in the collection.</param>
        /// <param name="minNumber">The minimum number value to include in the collection.</param>
        /// <returns>
        /// An immutable array of integers.
        /// </returns>
        public ImmutableArray<int> GetTestIntegerCollection (int collectionSize, int? minNumber = null, int? maxNumber = null) =>
            GetTestIntegerCollection(RandomSource, collectionSize, minNumber, maxNumber);
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Gets a collection of test integers in the range of [0...maxNumber)
        /// </summary>
        /// <param name="collectionSize">The size of collection set of integers to get</param>
        /// <param name="maxNumber">The maximum number value to include in the collection.</param>
        /// <returns>
        /// An immutable array of integers.
        /// </returns>
        public int[] GetTestIntegerMutableCollection (int collectionSize, int maxNumber) =>
            GetTestIntegerMutableCollection(collectionSize, 0, maxNumber);
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Gets a collection of test integers in the range of [minNumber...maxNumber)
        /// </summary>
        /// <param name="collectionSize">The size of collection set of integers to get</param>
        /// <param name="maxNumber">The maximum number value to include in the collection.</param>
        /// <param name="minNumber">The minimum number value to include in the collection.</param>
        /// <returns>
        /// An immutable array of integers.
        /// </returns>
        public int[] GetTestIntegerMutableCollection (int collectionSize, int? minNumber = null, int? maxNumber = null) =>
            GetTestIntegerMutableCollection(RandomSource, collectionSize, minNumber, maxNumber);
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Gets a set of unique test integers in the range of [0...maxNumber)
        /// </summary>
        /// <param name="setSize">The size of the set of integers to get.</param>
        /// <param name="maxNumber">The maximum number value to include in the set.</param>
        /// <returns>
        /// An immutable array of unique integers.
        /// </returns>
        public ImmutableArray<int> GetTestIntegerSet (int setSize, int maxNumber) =>
            GetTestIntegerSet(setSize, 0, maxNumber);
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Gets a set of unique test integers in the range of [minNumber...maxNumber)
        /// </summary>
        /// <param name="setSize">The size of the set of integers to get.</param>
        /// <param name="maxNumber">The maximum number value to include in the set.</param>
        /// <param name="minNumber">The minimum number value to include in the set.</param>
        /// <returns>
        /// An immutable array of unique integers.
        /// </returns>
        public ImmutableArray<int> GetTestIntegerSet (int setSize, int? minNumber = null, int? maxNumber = null) =>
            GetTestIntegerSet(RandomSource, setSize, minNumber, maxNumber);
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Gets a mutable set of unique test integers in the range of [0...maxNumber)
        /// </summary>
        /// <param name="setSize">The size of the set of integers to get.</param>
        /// <param name="maxNumber">The maximum number value to include in the set.</param>
        /// <returns>
        /// An immutable array of unique integers.
        /// </returns>
        public int[] GetTestIntegerMutableSet (int setSize, int maxNumber) =>
            GetTestIntegerMutableSet(setSize, 0, maxNumber);
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Gets a mutable set of unique test integers in the range of [minNumber...maxNumber)
        /// </summary>
        /// <param name="setSize">The size of the set of integers to get.</param>
        /// <param name="maxNumber">The maximum number value to include in the set.</param>
        /// <param name="minNumber">The minimum number value to include in the set.</param>
        /// <returns>
        /// An immutable array of unique integers.
        /// </returns>
        public int[] GetTestIntegerMutableSet (int setSize, int? minNumber = null, int? maxNumber = null) =>
            GetTestIntegerMutableSet(RandomSource, setSize, minNumber, maxNumber);
        //--------------------------------------------------------------------------------

        //--------------------------------------------------------------------------------
        /// <summary>
        /// Returns a random positive long integer in the range of [0...long.MaxValue)
        /// </summary>
        /// <returns>A random positive long integer</returns>
        public long GetTestLongInteger () => GetTestLongInteger(RandomSource, long.MaxValue);
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Returns a random positive long integer in the range of [0...maxValue)
        /// </summary>
        /// <param name="maxValue">
        /// The maximum value to return.
        /// </param>
        /// <returns>
        /// A random positive long integer.
        /// </returns>
        public long GetTestLongInteger (long maxValue) => GetTestLongInteger(RandomSource, maxValue);
        //--------------------------------------------------------------------------------
        /// <summary>
        ///  Gets a random long integer in the range of [minValue...maxValue)
        /// </summary>
        /// <param name="minValue">
        /// The minimum value to return.
        /// </param>
        /// <param name="maxValue">
        /// The maximum value to return.
        /// </param>
        /// <returns>
        /// A random long integer within the passed range of values.
        /// </returns>
        public long GetTestLongInteger (long minValue, long maxValue) => GetTestLongInteger(RandomSource, minValue, maxValue);
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Gets a collection of test long integers in the range of [0...maxNumber)
        /// </summary>
        /// <param name="collectionSize">The size of collection set of long integers to get</param>
        /// <param name="maxNumber">The maximum number value to include in the collection.</param>
        /// <returns>
        /// An immutable array of long integers.
        /// </returns>
        public ImmutableArray<long> GetTestLongIntegerCollection (int collectionSize, long maxNumber) =>
            GetTestLongIntegerCollection(RandomSource, collectionSize, maxNumber);
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Gets a collection of test long integers in the range of [minNumber...maxNumber)
        /// </summary>
        /// <param name="collectionSize">The size of collection set of long integers to get</param>
        /// <param name="maxNumber">The maximum number value to include in the collection.</param>
        /// <param name="minNumber">The minimum number value to include in the collection.</param>
        /// <returns>
        /// An immutable array of long integers.
        /// </returns>
        public ImmutableArray<long> GetTestLongIntegerCollection (int collectionSize, long? minNumber = null, long? maxNumber = null) =>
            GetTestLongIntegerCollection(RandomSource, collectionSize, minNumber, maxNumber);
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Gets a collection of test long integers in the range of [0...maxNumber)
        /// </summary>
        /// <param name="collectionSize">The size of collection set of long integers to get</param>
        /// <param name="maxNumber">The maximum number value to include in the collection.</param>
        /// <returns>
        /// An immutable array of long integers.
        /// </returns>
        public long[] GetTestLongIntegerMutableCollection (int collectionSize, long maxNumber) =>
            GetTestLongIntegerMutableCollection(collectionSize, 0, maxNumber);
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Gets a collection of test long integers in the range of [minNumber...maxNumber)
        /// </summary>
        /// <param name="collectionSize">The size of collection set of long integers to get</param>
        /// <param name="maxNumber">The maximum number value to include in the collection.</param>
        /// <param name="minNumber">The minimum number value to include in the collection.</param>
        /// <returns>
        /// An immutable array of long integers.
        /// </returns>
        public long[] GetTestLongIntegerMutableCollection (int collectionSize, long? minNumber = null, long? maxNumber = null) =>
            GetTestLongIntegerMutableCollection(RandomSource, collectionSize, minNumber, maxNumber);
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Gets a set of unique test long integers in the range of [0...maxNumber)
        /// </summary>
        /// <param name="setSize">The size of the set of long integers to get.</param>
        /// <param name="maxNumber">The maximum number value to include in the set.</param>
        /// <returns>
        /// An immutable array of unique long integers.
        /// </returns>
        public ImmutableArray<long> GetTestLongIntegerSet (int setSize, long maxNumber) =>
            GetTestLongIntegerSet(setSize, 0, maxNumber);
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Gets a set of unique test long integers in the range of [minNumber...maxNumber)
        /// </summary>
        /// <param name="setSize">The size of the set of long integers to get.</param>
        /// <param name="maxNumber">The maximum number value to include in the set.</param>
        /// <param name="minNumber">The minimum number value to include in the set.</param>
        /// <returns>
        /// An immutable array of unique long integers.
        /// </returns>
        public ImmutableArray<long> GetTestLongIntegerSet (int setSize, long? minNumber = null, long? maxNumber = null) =>
            GetTestLongIntegerSet(RandomSource, setSize, minNumber, maxNumber);
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Gets a set of unique test long integers in the range of [0...maxNumber)
        /// </summary>
        /// <param name="setSize">The size of the set of long integers to get.</param>
        /// <param name="maxNumber">The maximum number value to include in the set.</param>
        /// <returns>
        /// An immutable array of unique long integers.
        /// </returns>
        public long[] GetTestLongIntegerMutableSet (int setSize, long maxNumber) =>
            GetTestLongIntegerMutableSet(setSize, 0, maxNumber);
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Gets a set of unique test long integers in the range of [minNumber...maxNumber)
        /// </summary>
        /// <param name="setSize">The size of the set of long integers to get.</param>
        /// <param name="maxNumber">The maximum number value to include in the set.</param>
        /// <param name="minNumber">The minimum number value to include in the set.</param>
        /// <returns>
        /// An immutable array of unique long integers.
        /// </returns>
        public long[] GetTestLongIntegerMutableSet (int setSize, long? minNumber = null, long? maxNumber = null) =>
            GetTestLongIntegerMutableSet(RandomSource, setSize, minNumber, maxNumber);
        //--------------------------------------------------------------------------------

        //--------------------------------------------------------------------------------
        /// <summary>
        /// Allocates and returns a byte array of a random length between <paramref name="minSize"/> and
        /// <paramref name="maxSize"/> filled with random byte values.
        /// </summary>
        /// <param name="minSize">
        /// The minimum array size to return.
        /// </param>
        /// <param name="maxSize">
        /// The maximum array size to return.
        /// </param>
        /// <returns>
        /// A byte array of random size filled with random byte values.
        /// </returns>
        public byte[] GetRandomBytes (int minSize, int maxSize) => GetRandomBytes(SecureRandomSource, minSize, maxSize);
        //--------------------------------------------------------------------------------

    }
    //################################################################################
}
