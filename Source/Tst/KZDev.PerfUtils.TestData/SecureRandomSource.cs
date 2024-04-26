// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Security.Cryptography;

namespace KZDev.PerfUtils.Tests
{
    //################################################################################
    /// <summary>
    /// Implementation of the <see cref="IRandomSource"/> interface that uses a 
    /// secure random number generator.
    /// </summary>
    internal class SecureRandomSource : IRandomSource
    {
        /// <summary>
        /// The byte array used to generate random integers.
        /// </summary>
        [ThreadStatic]
        private static byte[]? _randomIntegerBytes;

        /// <summary>
        /// The byte array used to generate random long integers.
        /// </summary>
        [ThreadStatic]
        private static byte[]? _randomLongIntegerBytes;

        //--------------------------------------------------------------------------------
        /// <summary>
        /// For creating random delays
        /// </summary>
        private static readonly RandomNumberGenerator Random = RandomNumberGenerator.Create();
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Fills the passed byte array with cryptographically strong random byte values.
        /// </summary>
        /// <param name="byteArray">
        /// The array to be filled with cryptographically strong random byte values.
        /// </param>
        /// <param name="byteCount">
        /// The number of bytes to write to the array. If this value is negative, then the
        /// entire array is filled with random bytes.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when the passed <paramref name="byteArray"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when the passed <paramref name="byteArray"/> is an invalid size.
        /// </exception>
        private static int InternalGetRandomBytes (byte[] byteArray, int byteCount = -1)
        {
            ArgumentNullException.ThrowIfNull(byteArray);
            byteCount = (byteCount < 0) ? byteArray.Length : Math.Min(byteCount, byteArray.Length);
            if (byteArray.Length == 0)
                return 0;
            Random.GetBytes(byteArray, 0, byteCount);
            return byteCount;
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Helper to generate a random integer from any range (int.MinValue to int.MaxValue)
        /// in a truly random way.
        /// </summary>
        /// <returns></returns>
        private static int InternalGetRandomInteger ()
        {
            _randomIntegerBytes ??= new byte[sizeof(int)];
            Random.GetBytes(_randomIntegerBytes);
            // convert 4 bytes to an integer 
            return BitConverter.ToInt32(_randomIntegerBytes, 0);
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Helper to generate a random long integer from any range (long.MinValue to
        /// long.MaxValue) in a truly random way.
        /// </summary>
        /// <returns></returns>
        private static long InternalGetRandomLongInteger ()
        {
            _randomLongIntegerBytes ??= new byte[sizeof(long)];
            Random.GetBytes(_randomLongIntegerBytes);
            // convert 8 bytes to a long integer 
            return BitConverter.ToInt64(_randomLongIntegerBytes, 0);
        }
        //--------------------------------------------------------------------------------

        //--------------------------------------------------------------------------------
        /// <summary>
        /// Internal helper to generate a random integer in the requested range.
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
        private static int InternalGetRandomInteger (int minValue, int maxValue)
        {
            switch (minValue)
            {
                case int.MinValue when (maxValue == int.MaxValue):
                    return InternalGetRandomInteger();

                case < 0 when (maxValue < 0):
                case > 0 when (maxValue > 0):
                case > (int.MinValue) / 3 when (maxValue < (int.MaxValue / 3)):
                    {
                        // Fairly narrow range case, so just use modulo
                        int range = maxValue - minValue;
                        return minValue + (InternalGetRandomInteger(0, int.MaxValue) % range);
                    }
            }
            // Potentially a wide range, so use a long integer to get a random value and then use modulo
            long rangeLong = (long)maxValue - minValue;
            long offset = InternalGetRandomLongInteger(0, long.MaxValue) % rangeLong;
            return minValue + (int)offset;
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Internal helper to generate a random long integer in the requested range.
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
        private static long InternalGetRandomLongInteger (long minValue, long maxValue)
        {
            switch (minValue)
            {
                case long.MinValue when (maxValue == long.MaxValue):
                    return InternalGetRandomLongInteger();

                case < 0 when (maxValue < 0):
                case > 0 when (maxValue > 0):
                case > long.MinValue / 3 when (maxValue < (long.MaxValue / 3)):
                    {
                        long range = maxValue - minValue;
                        return minValue + (InternalGetRandomLongInteger(0, long.MaxValue) % range);
                    }
            }

            while (true)
            {
                long randomValue = InternalGetRandomLongInteger();
                if ((randomValue >= minValue) && (randomValue <= maxValue))
                    return randomValue;
            }
        }
        //--------------------------------------------------------------------------------

        #region Implementation of IRandomSource

        //--------------------------------------------------------------------------------
        /// <inheritdoc />
        public int GetRandomInteger (int maxValue) => (maxValue > 0) ? 
            InternalGetRandomInteger (0, maxValue) : 
            throw new ArgumentException($"{nameof(maxValue)} must be greater than zero");
        //--------------------------------------------------------------------------------
        /// <inheritdoc />
        public int GetRandomInteger (int minValue, int maxValue)
        {
            if (maxValue < minValue)
                throw new ArgumentOutOfRangeException(nameof(maxValue), $"{nameof(maxValue)} must be greater than or equal to {nameof(minValue)}");
            return (minValue == maxValue) ? minValue : InternalGetRandomInteger(minValue, maxValue);
        }
        //--------------------------------------------------------------------------------
        /// <inheritdoc />
        public long GetRandomInteger (long maxValue) => (maxValue > 0) ? 
            InternalGetRandomLongInteger(0, maxValue) :
            throw new ArgumentException($"{nameof(maxValue)} must be greater than zero");
        //--------------------------------------------------------------------------------
        /// <inheritdoc />
        public long GetRandomLongInteger (long minValue, long maxValue)
        {
            if (maxValue < minValue)
                throw new ArgumentOutOfRangeException(nameof(maxValue), $"{nameof(maxValue)} must be greater than or equal to {nameof(minValue)}");
            return (minValue == maxValue) ? minValue : InternalGetRandomLongInteger(minValue, maxValue);
        }
        //--------------------------------------------------------------------------------
        /// <inheritdoc />
        public int GetRandomBytes (byte[] byteArray, int byteCount = -1) => InternalGetRandomBytes(byteArray, byteCount);
        //--------------------------------------------------------------------------------
        /// <inheritdoc />
        public bool GetRandomBoolean () => (0 == (InternalGetRandomInteger(0, byte.MaxValue + 1) % 2));
        //--------------------------------------------------------------------------------
        /// <inheritdoc />
        public bool GetRandomFalse (int falseFrequency) =>
            falseFrequency switch
            {
                0 => true,
                1 => false,
                _ => (0 != InternalGetRandomInteger(0, falseFrequency))
            };
        //--------------------------------------------------------------------------------
        /// <inheritdoc />
        public bool GetRandomTrue (int trueFrequency) =>
            trueFrequency switch
            {
                0 => false,
                1 => true,
                _ => (0 == InternalGetRandomInteger(0, trueFrequency))
            };
        //--------------------------------------------------------------------------------

        #endregion
    }
    //################################################################################
}
