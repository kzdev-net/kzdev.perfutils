// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

namespace KZDev.PerfUtils.Tests
{
    /// <summary>
    /// Provides a source of random numbers and boolean values.
    /// </summary>
    public interface IRandomSource
    {
        /// <summary>
        /// Gets a random integer in the range of [0...maxValue)
        /// </summary>
        /// <param name="maxValue">
        /// The maximum value that the random number can be.
        /// </param>
        /// <returns>
        /// A random number between zero and the maximum values.
        /// </returns>
        int GetRandomInteger (int maxValue);

        /// <summary>
        /// Gets a random integer in the range of [minValue...maxValue)
        /// </summary>
        /// <param name="minValue">
        /// The minimum value that the random number can be.
        /// </param>
        /// <param name="maxValue">
        /// The maximum value that the random number can be.
        /// </param>
        /// <returns>
        /// A random number between the specified minimum and maximum values.
        /// </returns>
        int GetRandomInteger (int minValue, int maxValue);

        /// <summary>
        /// Gets a random unsigned integer in the range of [0...maxValue)
        /// </summary>
        /// <param name="maxValue">
        /// The maximum value that the random number can be.
        /// </param>
        /// <returns>
        /// A random number between zero and the maximum values.
        /// </returns>
        uint GetRandomUnsignedInteger (uint maxValue);

        /// <summary>
        /// Gets a random unsigned integer in the range of [minValue...maxValue)
        /// </summary>
        /// <param name="minValue">
        /// The minimum value that the random number can be.
        /// </param>
        /// <param name="maxValue">
        /// The maximum value that the random number can be.
        /// </param>
        /// <returns>
        /// A random number between the specified minimum and maximum values.
        /// </returns>
        uint GetRandomUnsignedInteger (uint minValue, uint maxValue);

        /// <summary>
        /// Gets a random long integer in the range of [0...maxValue)
        /// </summary>
        /// <param name="maxValue">
        /// The maximum value that the random number can be.
        /// </param>
        /// <returns>
        /// A random number between the zero and the maximum values.
        /// </returns>
        long GetRandomLongInteger (long maxValue);

        /// <summary>
        /// Gets a random long integer in the range of [minValue...maxValue)
        /// </summary>
        /// <param name="minValue">
        /// The minimum value that the random number can be.
        /// </param>
        /// <param name="maxValue">
        /// The maximum value that the random number can be.
        /// </param>
        /// <returns>
        /// A random long integer between the specified minimum and maximum values.
        /// </returns>
        long GetRandomLongInteger (long minValue, long maxValue);

        /// <summary>
        /// Gets a random unsigned long integer in the range of [0...maxValue)
        /// </summary>
        /// <param name="maxValue">
        /// The maximum value that the random number can be.
        /// </param>
        /// <returns>
        /// A random number between the zero and the maximum values.
        /// </returns>
        ulong GetRandomUnsignedLongInteger (ulong maxValue);

        /// <summary>
        /// Gets a random unsigned long integer in the range of [minValue...maxValue)
        /// </summary>
        /// <param name="minValue">
        /// The minimum value that the random number can be.
        /// </param>
        /// <param name="maxValue">
        /// The maximum value that the random number can be.
        /// </param>
        /// <returns>
        /// A random long integer between the specified minimum and maximum values.
        /// </returns>
        ulong GetRandomUnsignedLongInteger (ulong minValue, ulong maxValue);

        /// <summary>
        /// Gets a series of random bytes and fills the array with them.
        /// </summary>
        /// <param name="byteArray">
        /// The array to fill with random byte values.
        /// </param>
        /// <param name="byteCount">
        /// The number of random bytes to generate. If not specified, the entire array is filled.
        /// </param>
        /// <returns>
        /// The number of random bytes generated.
        /// </returns>
        int GetRandomBytes (byte[] byteArray, int byteCount = -1);

        /// <summary>
        /// Gets a random boolean value.
        /// </summary>
        /// <returns>
        /// A random boolean value.
        /// </returns>
        bool GetRandomBoolean ();

        /// <summary>
        /// Gets a random boolean value, with a specified average frequency of false values.
        /// </summary>
        /// <param name="falseFrequency">
        /// The average frequency of false values. A value of 0 will always return true, 
        /// a value of 1 will always return false. A value of n will return false 1 out of n 
        /// times on average.
        /// </param>
        /// <returns>
        /// A random boolean value.
        /// </returns>
        bool GetRandomFalse (int falseFrequency);

        /// <summary>
        /// Gets a random boolean value, with a specified average frequency of true values.
        /// </summary>
        /// <param name="trueFrequency">
        /// The average frequency of true values. A value of 0 will always return false, a
        /// value of 1 will always return true. A value of n will return true 1 out of n times
        /// on average.
        /// </param>
        /// <returns>
        /// A random boolean value.
        /// </returns>
        bool GetRandomTrue (int trueFrequency);
    }
}
