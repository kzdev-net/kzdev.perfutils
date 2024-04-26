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
        private static IRandomSource? _secureRandomSource;

        //--------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the random number generator to use for cryptographic random operations.
        /// </summary>
        public static IRandomSource SecureRandomSource
        {
            get
            {
                IRandomSource? currentSource = Volatile.Read(ref _secureRandomSource);
                if (currentSource is not null)
                    return currentSource;
                SecureRandomSource newSource = new SecureRandomSource();
                return Interlocked.CompareExchange(ref _secureRandomSource, newSource, null) is null ? newSource : Volatile.Read(ref _secureRandomSource);
            }
            set
            {
                ArgumentNullException.ThrowIfNull(value);
                if (Interlocked.CompareExchange(ref _secureRandomSource, value, null) is null)
                    return;
                throw new InvalidOperationException("The random source has already been set.");
            }
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Returns a random positive integer in the range of [0...int.MaxValue)
        /// </summary>
        /// <returns>A random positive integer</returns>
        public static int GetTestInteger (IRandomSource randomSource) => randomSource.GetRandomInteger(0, int.MaxValue);
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Returns a random positive integer in the range of [0...maxValue)
        /// </summary>
        /// <param name="randomSource">
        /// The <see cref="IRandomSource"/> instance to use for generating values.
        /// </param>
        /// <param name="maxValue">
        /// The maximum value to return.
        /// </param>
        /// <returns>
        /// A random positive integer.
        /// </returns>
        public static int GetTestInteger (IRandomSource randomSource, int maxValue)
        {
            if (maxValue < 0)
                throw new ArgumentOutOfRangeException(nameof(maxValue), $"{nameof(maxValue)} must be greater than or equal to zero");
            return (maxValue == 0) ? 0 : randomSource.GetRandomInteger(0, maxValue);
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        ///  Gets a random integer in the range of [minValue...maxValue)
        /// </summary>
        /// <param name="randomSource">
        /// The <see cref="IRandomSource"/> instance to use for generating random bytes.
        /// </param>
        /// <param name="minValue">
        /// The minimum value to return.
        /// </param>
        /// <param name="maxValue">
        /// The maximum value to return.
        /// </param>
        /// <returns>
        /// A random integer within the passed range of values.
        /// </returns>
        public static int GetTestInteger (IRandomSource randomSource, int minValue, int maxValue)
        {
            if (maxValue < minValue)
                throw new ArgumentOutOfRangeException(nameof(maxValue), $"{nameof(maxValue)} must be greater than or equal to {nameof(minValue)}");
            return (minValue == maxValue) ? minValue : randomSource.GetRandomInteger(minValue, maxValue);
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Gets a collection of test integers in the range of [0...maxNumber)
        /// </summary>
        /// <param name="randomSource">
        /// The <see cref="IRandomSource"/> instance to use for generating random bytes.
        /// </param>
        /// <param name="collectionSize">The size of collection set of integers to get</param>
        /// <param name="maxNumber">The maximum number value to include in the collection.</param>
        /// <returns>
        /// An immutable array of integers.
        /// </returns>
        public static ImmutableArray<int> GetTestIntegerCollection (IRandomSource randomSource, int collectionSize, int maxNumber) =>
            GetTestIntegerCollection(randomSource, collectionSize, 0, maxNumber);
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Gets a collection of test integers in the range of [minNumber...maxNumber)
        /// </summary>
        /// <param name="randomSource">
        /// The <see cref="IRandomSource"/> instance to use for generating random bytes.
        /// </param>
        /// <param name="collectionSize">The size of collection set of integers to get</param>
        /// <param name="maxNumber">The maximum number value to include in the collection.</param>
        /// <param name="minNumber">The minimum number value to include in the collection.</param>
        /// <returns>
        /// An immutable array of integers.
        /// </returns>
        public static ImmutableArray<int> GetTestIntegerCollection (IRandomSource randomSource, int collectionSize, int? minNumber = null, int? maxNumber = null)
        {
            int useMinNumber = minNumber ?? 0;
            int useMaxNumber = maxNumber ?? int.MaxValue;
            if (useMaxNumber < useMinNumber)
                throw new ArgumentOutOfRangeException(nameof(maxNumber), $"{nameof(maxNumber)} must be greater than or equal to {nameof(minNumber)} ({useMinNumber})");
            switch (collectionSize)
            {
                case < 0:
                    throw new ArgumentOutOfRangeException(nameof(collectionSize), $"{nameof(collectionSize)} must be greater than or equal to zero");

                case 0:
                    return ImmutableArray<int>.Empty;

                case 1:
                    return ImmutableArray.Create(GetTestInteger(randomSource, useMinNumber, useMaxNumber));
            }

            ImmutableArray<int>.Builder builder = ImmutableArray.CreateBuilder<int>(collectionSize);

            for (int i = 0; i < collectionSize; i++)
                builder.Add(GetTestInteger(randomSource, useMinNumber, useMaxNumber));

            return builder.ToImmutable();
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Gets a collection of test integers in the range of [0...maxNumber)
        /// </summary>
        /// <param name="randomSource">
        /// The <see cref="IRandomSource"/> instance to use for generating random bytes.
        /// </param>
        /// <param name="collectionSize">The size of collection set of integers to get</param>
        /// <param name="maxNumber">The maximum number value to include in the collection.</param>
        /// <returns>
        /// An immutable array of integers.
        /// </returns>
        public static int[] GetTestIntegerMutableCollection (IRandomSource randomSource, int collectionSize, int maxNumber) =>
            GetTestIntegerMutableCollection(randomSource, collectionSize, 0, maxNumber);
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Gets a collection of test integers in the range of [minNumber...maxNumber)
        /// </summary>
        /// <param name="randomSource">
        /// The <see cref="IRandomSource"/> instance to use for generating random bytes.
        /// </param>
        /// <param name="collectionSize">The size of collection set of integers to get</param>
        /// <param name="maxNumber">The maximum number value to include in the collection.</param>
        /// <param name="minNumber">The minimum number value to include in the collection.</param>
        /// <returns>
        /// An immutable array of integers.
        /// </returns>
        public static int[] GetTestIntegerMutableCollection (IRandomSource randomSource, int collectionSize, int? minNumber = null, int? maxNumber = null)
        {
            int useMinNumber = minNumber ?? 0;
            int useMaxNumber = maxNumber ?? int.MaxValue;
            if (useMaxNumber < useMinNumber)
                throw new ArgumentOutOfRangeException(nameof(maxNumber), $"{nameof(maxNumber)} must be greater than or equal to {nameof(minNumber)} ({useMinNumber})");
            switch (collectionSize)
            {
                case < 0:
                    throw new ArgumentOutOfRangeException(nameof(collectionSize), $"{nameof(collectionSize)} must be greater than or equal to zero");

                case 0:
                    return [];

                case 1:
                    return [GetTestInteger(randomSource, useMinNumber, useMaxNumber)];
            }

            int[] returnArray = new int[collectionSize];

            for (int i = 0; i < collectionSize; i++)
                returnArray[i] = GetTestInteger(randomSource, useMinNumber, useMaxNumber);

            return returnArray;
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Gets a set of unique test integers in the range of [0...maxNumber)
        /// </summary>
        /// <param name="randomSource">
        /// The <see cref="IRandomSource"/> instance to use for generating random bytes.
        /// </param>
        /// <param name="setSize">The size of the set of integers to get.</param>
        /// <param name="maxNumber">The maximum number value to include in the set.</param>
        /// <returns>
        /// An immutable array of unique integers.
        /// </returns>
        public static ImmutableArray<int> GetTestIntegerSet (IRandomSource randomSource, int setSize, int maxNumber) =>
            GetTestIntegerSet(randomSource, setSize, 0, maxNumber);
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Gets a set of unique test integers in the range of [minNumber...maxNumber)
        /// </summary>
        /// <param name="randomSource">
        /// The <see cref="IRandomSource"/> instance to use for generating random bytes.
        /// </param>
        /// <param name="setSize">The size of the set of integers to get.</param>
        /// <param name="maxNumber">The maximum number value to include in the set.</param>
        /// <param name="minNumber">The minimum number value to include in the set.</param>
        /// <returns>
        /// An immutable array of unique integers.
        /// </returns>
        public static ImmutableArray<int> GetTestIntegerSet (IRandomSource randomSource, int setSize, int? minNumber = null, int? maxNumber = null)
        {
            int useMinNumber = minNumber ?? 0;
            int useMaxNumber = maxNumber ?? int.MaxValue;
            if (useMaxNumber < useMinNumber)
                throw new ArgumentOutOfRangeException(nameof(maxNumber), $"{nameof(maxNumber)} must be greater than or equal to {nameof(minNumber)} ({useMinNumber})");
            if (setSize > (useMaxNumber - useMinNumber + 1))
                throw new ArgumentOutOfRangeException(nameof(setSize), $"{nameof(setSize)} must be less than or equal to the difference between {nameof(maxNumber)} ({useMaxNumber}) and {nameof(minNumber)} ({useMinNumber})");
            switch (setSize)
            {
                case < 0:
                    throw new ArgumentOutOfRangeException(nameof(setSize), $"{nameof(setSize)} must be greater than or equal to zero");

                case 0:
                    return ImmutableArray<int>.Empty;

                case 1:
                    return ImmutableArray.Create(GetTestInteger(randomSource, useMinNumber, useMaxNumber));
            }

            // Use a hash set to ensure uniqueness
            HashSet<int> returnList = new();

            while (returnList.Count < setSize)
            {
                int nextNumber = GetTestInteger(randomSource, useMinNumber, useMaxNumber);
                returnList.Add(nextNumber);
            }

            return returnList.ToImmutableArray();
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Gets a mutable set of unique test integers in the range of [0...maxNumber)
        /// </summary>
        /// <param name="randomSource">
        /// The <see cref="IRandomSource"/> instance to use for generating random bytes.
        /// </param>
        /// <param name="setSize">The size of the set of integers to get.</param>
        /// <param name="maxNumber">The maximum number value to include in the set.</param>
        /// <returns>
        /// An immutable array of unique integers.
        /// </returns>
        public static int[] GetTestIntegerMutableSet (IRandomSource randomSource, int setSize, int maxNumber) =>
            GetTestIntegerMutableSet(randomSource, setSize, 0, maxNumber);
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Gets a mutable set of unique test integers in the range of [minNumber...maxNumber)
        /// </summary>
        /// <param name="randomSource">
        /// The <see cref="IRandomSource"/> instance to use for generating random bytes.
        /// </param>
        /// <param name="setSize">The size of the set of integers to get.</param>
        /// <param name="maxNumber">The maximum number value to include in the set.</param>
        /// <param name="minNumber">The minimum number value to include in the set.</param>
        /// <returns>
        /// An immutable array of unique integers.
        /// </returns>
        public static int[] GetTestIntegerMutableSet (IRandomSource randomSource, int setSize, int? minNumber = null, int? maxNumber = null)
        {
            int useMinNumber = minNumber ?? 0;
            int useMaxNumber = maxNumber ?? int.MaxValue;
            if (useMaxNumber < useMinNumber)
                throw new ArgumentOutOfRangeException(nameof(maxNumber), $"{nameof(maxNumber)} must be greater than or equal to {nameof(minNumber)} ({useMinNumber})");
            if (setSize > (useMaxNumber - useMinNumber + 1))
                throw new ArgumentOutOfRangeException(nameof(setSize), $"{nameof(setSize)} must be less than or equal to the difference between {nameof(maxNumber)} ({useMaxNumber}) and {nameof(minNumber)} ({useMinNumber})");
            switch (setSize)
            {
                case < 0:
                    throw new ArgumentOutOfRangeException(nameof(setSize), $"{nameof(setSize)} must be greater than or equal to zero");

                case 0:
                    return [];

                case 1:
                    return [GetTestInteger(randomSource, useMinNumber, useMaxNumber)];
            }

            // Use a hash set to ensure uniqueness
            HashSet<int> returnList = new();

            while (returnList.Count < setSize)
            {
                int nextNumber = GetTestInteger(randomSource, useMinNumber, useMaxNumber);
                returnList.Add(nextNumber);
            }

            return [.. returnList];
        }
        //--------------------------------------------------------------------------------

        //--------------------------------------------------------------------------------
        /// <summary>
        /// Returns a random positive long integer in the range of [0...long.MaxValue)
        /// </summary>
        /// <returns>A random positive long integer</returns>
        public static long GetTestLongInteger (IRandomSource randomSource) => randomSource.GetRandomLongInteger(0, long.MaxValue);
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Returns a random positive long integer in the range of [0...maxValue)
        /// </summary>
        /// <param name="randomSource">
        /// The <see cref="IRandomSource"/> instance to use for generating random bytes.
        /// </param>
        /// <param name="maxValue">
        /// The maximum value to return.
        /// </param>
        /// <returns>
        /// A random positive long integer.
        /// </returns>
        public static long GetTestLongInteger (IRandomSource randomSource, long maxValue)
        {
            if (maxValue < 0)
                throw new ArgumentOutOfRangeException(nameof(maxValue), $"{nameof(maxValue)} must be greater than or equal to zero");
            return (maxValue == 0) ? 0 : randomSource.GetRandomLongInteger(0, maxValue);
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        ///  Gets a random long integer in the range of [minValue...maxValue)
        /// </summary>
        /// <param name="randomSource">
        /// The <see cref="IRandomSource"/> instance to use for generating random bytes.
        /// </param>
        /// <param name="minValue">
        /// The minimum value to return.
        /// </param>
        /// <param name="maxValue">
        /// The maximum value to return.
        /// </param>
        /// <returns>
        /// A random long integer within the passed range of values.
        /// </returns>
        public static long GetTestLongInteger (IRandomSource randomSource, long minValue, long maxValue)
        {
            if (maxValue < minValue)
                throw new ArgumentOutOfRangeException(nameof(maxValue), $"{nameof(maxValue)} must be greater than or equal to {nameof(minValue)}");
            return (minValue == maxValue) ? minValue : randomSource.GetRandomLongInteger(minValue, maxValue);
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Gets a collection of test long integers in the range of [0...maxNumber)
        /// </summary>
        /// <param name="collectionSize">The size of collection set of long integers to get</param>
        /// <param name="maxNumber">The maximum number value to include in the collection.</param>
        /// <returns>
        /// An immutable array of long integers.
        /// </returns>
        public static ImmutableArray<long> GetTestLongIntegerCollection (IRandomSource randomSource, int collectionSize, long maxNumber) =>
            GetTestLongIntegerCollection(randomSource, collectionSize, 0, maxNumber);
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Gets a collection of test long integers in the range of [minNumber...maxNumber)
        /// </summary>
        /// <param name="randomSource">
        /// The <see cref="IRandomSource"/> instance to use for generating random bytes.
        /// </param>
        /// <param name="collectionSize">The size of collection set of long integers to get</param>
        /// <param name="maxNumber">The maximum number value to include in the collection.</param>
        /// <param name="minNumber">The minimum number value to include in the collection.</param>
        /// <returns>
        /// An immutable array of long integers.
        /// </returns>
        public static ImmutableArray<long> GetTestLongIntegerCollection (IRandomSource randomSource, int collectionSize, long? minNumber = null, long? maxNumber = null)
        {
            long useMinNumber = minNumber ?? 0;
            long useMaxNumber = maxNumber ?? long.MaxValue;
            if (useMaxNumber < useMinNumber)
                throw new ArgumentOutOfRangeException(nameof(maxNumber), $"{nameof(maxNumber)} must be greater than or equal to {nameof(minNumber)} ({useMinNumber})");
            switch (collectionSize)
            {
                case < 0:
                    throw new ArgumentOutOfRangeException(nameof(collectionSize), $"{nameof(collectionSize)} must be greater than or equal to zero");

                case 0:
                    return ImmutableArray<long>.Empty;

                case 1:
                    return ImmutableArray.Create(GetTestLongInteger(randomSource, useMinNumber, useMaxNumber));
            }

            ImmutableArray<long>.Builder builder = ImmutableArray.CreateBuilder<long>(collectionSize);

            for (long i = 0; i < collectionSize; i++)
                builder.Add(GetTestLongInteger(randomSource, useMinNumber, useMaxNumber));

            return builder.ToImmutable();
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Gets a collection of test long integers in the range of [0...maxNumber)
        /// </summary>
        /// <param name="randomSource">
        /// The <see cref="IRandomSource"/> instance to use for generating random bytes.
        /// </param>
        /// <param name="collectionSize">The size of collection set of long integers to get</param>
        /// <param name="maxNumber">The maximum number value to include in the collection.</param>
        /// <returns>
        /// An immutable array of long integers.
        /// </returns>
        public static long[] GetTestLongIntegerMutableCollection (IRandomSource randomSource, int collectionSize, long maxNumber) =>
            GetTestLongIntegerMutableCollection(randomSource, collectionSize, 0, maxNumber);
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Gets a collection of test long integers in the range of [minNumber...maxNumber)
        /// </summary>
        /// <param name="randomSource">
        /// The <see cref="IRandomSource"/> instance to use for generating random bytes.
        /// </param>
        /// <param name="collectionSize">The size of collection set of long integers to get</param>
        /// <param name="maxNumber">The maximum number value to include in the collection.</param>
        /// <param name="minNumber">The minimum number value to include in the collection.</param>
        /// <returns>
        /// An immutable array of long integers.
        /// </returns>
        public static long[] GetTestLongIntegerMutableCollection (IRandomSource randomSource, int collectionSize, long? minNumber = null, long? maxNumber = null)
        {
            long useMinNumber = minNumber ?? 0;
            long useMaxNumber = maxNumber ?? long.MaxValue;
            if (useMaxNumber < useMinNumber)
                throw new ArgumentOutOfRangeException(nameof(maxNumber), $"{nameof(maxNumber)} must be greater than or equal to {nameof(minNumber)} ({useMinNumber})");
            switch (collectionSize)
            {
                case < 0:
                    throw new ArgumentOutOfRangeException(nameof(collectionSize), $"{nameof(collectionSize)} must be greater than or equal to zero");

                case 0:
                    return [];

                case 1:
                    return [GetTestLongInteger(randomSource, useMinNumber, useMaxNumber)];
            }

            long[] returnArray = new long[collectionSize];

            for (long i = 0; i < collectionSize; i++)
                returnArray[i] = GetTestLongInteger(randomSource, useMinNumber, useMaxNumber);

            return returnArray;
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Gets a set of unique test long integers in the range of [0...maxNumber)
        /// </summary>
        /// <param name="randomSource">
        /// The <see cref="IRandomSource"/> instance to use for generating random bytes.
        /// </param>
        /// <param name="setSize">The size of the set of long integers to get.</param>
        /// <param name="maxNumber">The maximum number value to include in the set.</param>
        /// <returns>
        /// An immutable array of unique long integers.
        /// </returns>
        public static ImmutableArray<long> GetTestLongIntegerSet (IRandomSource randomSource, int setSize, long maxNumber) =>
            GetTestLongIntegerSet(randomSource, setSize, 0, maxNumber);
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Gets a set of unique test long integers in the range of [minNumber...maxNumber)
        /// </summary>
        /// <param name="randomSource">
        /// The <see cref="IRandomSource"/> instance to use for generating random bytes.
        /// </param>
        /// <param name="setSize">The size of the set of long integers to get.</param>
        /// <param name="maxNumber">The maximum number value to include in the set.</param>
        /// <param name="minNumber">The minimum number value to include in the set.</param>
        /// <returns>
        /// An immutable array of unique long integers.
        /// </returns>
        public static ImmutableArray<long> GetTestLongIntegerSet (IRandomSource randomSource, int setSize, long? minNumber = null, long? maxNumber = null)
        {
            long useMinNumber = minNumber ?? 0;
            long useMaxNumber = maxNumber ?? long.MaxValue;
            if (useMaxNumber < useMinNumber)
                throw new ArgumentOutOfRangeException(nameof(maxNumber), $"{nameof(maxNumber)} must be greater than or equal to {nameof(minNumber)} ({useMinNumber})");
            if (setSize > (useMaxNumber - useMinNumber + 1))
                throw new ArgumentOutOfRangeException(nameof(setSize), $"{nameof(setSize)} must be less than or equal to the difference between {nameof(maxNumber)} ({useMaxNumber}) and {nameof(minNumber)} ({useMinNumber})");
            switch (setSize)
            {
                case < 0:
                    throw new ArgumentOutOfRangeException(nameof(setSize), $"{nameof(setSize)} must be greater than or equal to zero");

                case 0:
                    return ImmutableArray<long>.Empty;

                case 1:
                    return ImmutableArray.Create(GetTestLongInteger(randomSource, useMinNumber, useMaxNumber));
            }

            // Use a hash set to ensure uniqueness
            HashSet<long> returnList = new();

            while (returnList.Count < setSize)
            {
                long nextNumber = GetTestLongInteger(randomSource, useMinNumber, useMaxNumber);
                returnList.Add(nextNumber);
            }

            return returnList.ToImmutableArray();
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Gets a set of unique test long integers in the range of [0...maxNumber)
        /// </summary>
        /// <param name="randomSource">
        /// The <see cref="IRandomSource"/> instance to use for generating random bytes.
        /// </param>
        /// <param name="setSize">The size of the set of long integers to get.</param>
        /// <param name="maxNumber">The maximum number value to include in the set.</param>
        /// <returns>
        /// An immutable array of unique long integers.
        /// </returns>
        public static long[] GetTestLongIntegerMutableSet (IRandomSource randomSource, int setSize, long maxNumber) =>
            GetTestLongIntegerMutableSet(randomSource, setSize, 0, maxNumber);
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Gets a set of unique test long integers in the range of [minNumber...maxNumber)
        /// </summary>
        /// <param name="randomSource">
        /// The <see cref="IRandomSource"/> instance to use for generating random bytes.
        /// </param>
        /// <param name="setSize">The size of the set of long integers to get.</param>
        /// <param name="maxNumber">The maximum number value to include in the set.</param>
        /// <param name="minNumber">The minimum number value to include in the set.</param>
        /// <returns>
        /// An immutable array of unique long integers.
        /// </returns>
        public static long[] GetTestLongIntegerMutableSet (IRandomSource randomSource, int setSize, long? minNumber = null, long? maxNumber = null)
        {
            long useMinNumber = minNumber ?? 0;
            long useMaxNumber = maxNumber ?? long.MaxValue;
            if (useMaxNumber < useMinNumber)
                throw new ArgumentOutOfRangeException(nameof(maxNumber), $"{nameof(maxNumber)} must be greater than or equal to {nameof(minNumber)} ({useMinNumber})");
            if (setSize > (useMaxNumber - useMinNumber + 1))
                throw new ArgumentOutOfRangeException(nameof(setSize), $"{nameof(setSize)} must be less than or equal to the difference between {nameof(maxNumber)} ({useMaxNumber}) and {nameof(minNumber)} ({useMinNumber})");
            switch (setSize)
            {
                case < 0:
                    throw new ArgumentOutOfRangeException(nameof(setSize), $"{nameof(setSize)} must be greater than or equal to zero");

                case 0:
                    return [];

                case 1:
                    return [GetTestLongInteger(randomSource, useMinNumber, useMaxNumber)];
            }

            // Use a hash set to ensure uniqueness
            HashSet<long> returnList = new();

            while (returnList.Count < setSize)
            {
                long nextNumber = GetTestLongInteger(randomSource, useMinNumber, useMaxNumber);
                returnList.Add(nextNumber);
            }

            return [.. returnList];
        }
        //--------------------------------------------------------------------------------

        //--------------------------------------------------------------------------------
        /// <summary>
        /// Allocates and returns a byte array of the specified length filled with 
        /// random byte values.
        /// </summary>
        /// <param name="arraySize">
        /// The size of the array to return.
        /// </param>
        /// <returns>
        /// A byte array of <paramref name="arraySize"/> size filled with random bytes.
        /// </returns>
        public static byte[] GetRandomBytes (int arraySize)
        {
            byte[] bytes = new byte[arraySize];
            SecureRandomSource.GetRandomBytes(bytes);
            return bytes;
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Allocates and returns a byte array of a random length between <paramref name="minSize"/> and
        /// <paramref name="maxSize"/> filled with random byte values.
        /// </summary>
        /// <param name="arraySizeRandomSource">
        /// The <see cref="IRandomSource"/> instance to use for getting a size of the array.
        /// </param>
        /// <param name="minSize">
        /// The minimum array size to return.
        /// </param>
        /// <param name="maxSize">
        /// The maximum array size to return.
        /// </param>
        /// <returns>
        /// A byte array of random size filled with random byte values.
        /// </returns>
        public static byte[] GetRandomBytes (IRandomSource arraySizeRandomSource, int minSize, int maxSize)
        {
            byte[] bytes = new byte[GetTestInteger(arraySizeRandomSource, minSize, maxSize)];
            SecureRandomSource.GetRandomBytes(bytes);
            return bytes;
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Fills the passed byte array with random byte values.
        /// </summary>
        /// <param name="byteArray">
        /// The array to be filled with random byte values.
        /// </param>
        /// <returns>
        /// The number of bytes that were written to the array.
        /// </returns>
        public static int GetRandomBytes (byte[] byteArray) => SecureRandomSource.GetRandomBytes(byteArray);
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Fills <paramref name="byteCount"/> bytes into the passed byte array with random byte values.
        /// </summary>
        /// <param name="byteArray">
        /// The array to be filled with random byte values.
        /// </param>
        /// <param name="byteCount">
        /// The number of bytes to write to the array.
        /// </param>
        /// <returns>
        /// The number of bytes that were written to the array.
        /// </returns>
        public static int GetRandomBytes (byte[] byteArray, int byteCount)
        {
            if (byteCount < 0)
                throw new ArgumentOutOfRangeException(nameof(byteCount), $"{nameof(byteCount)} must be greater than or equal to zero");
            if (byteArray is null)
                throw new ArgumentNullException(nameof(byteArray));
            return SecureRandomSource.GetRandomBytes(byteArray, Math.Min(byteArray.Length, byteCount));
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Returns a random byte value.
        /// </summary>
        /// <returns>
        /// The random byte value
        /// </returns>
        public static byte GetRandomByte ()
        {
            return (byte)SecureRandomSource.GetRandomInteger(byte.MaxValue);
        }
        //--------------------------------------------------------------------------------

    }
    //################################################################################
}
