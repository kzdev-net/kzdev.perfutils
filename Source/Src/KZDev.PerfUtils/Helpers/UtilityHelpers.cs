// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace KZDev.PerfUtils.Helpers
{
    //################################################################################
    /// <summary>
    /// A collection of utility helper methods for the library.
    /// </summary>
    internal static class UtilityHelpers
    {
        /// <summary>
        /// The size of the array that will be considered a "large" array for the purposes of
        /// using the split array walk method.
        /// </summary>
        internal const int LargeArraySize = 6;

        //--------------------------------------------------------------------------------
        /// <summary>
        /// Gets the index of the buffer size that will hold the given buffer size by using 
        /// a search cutting the array in half (we don't need an actual binary search given
        /// the known small array sizes expected).
        /// </summary>
        /// <param name="bufferSizeArray">
        /// The array of buffer sizes to check against.
        /// </param>
        /// <param name="bufferSize">The needed buffer size</param>
        /// <returns>
        /// The index in the buffer size array that will hold the given buffer size.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int GetSplitArrayWalkBufferSizeIndex (int[] bufferSizeArray, int bufferSize)
        {
            int checkIndex = bufferSizeArray.Length / 2;
            while (true)
            {
                // Do we need a larger buffer than the current check index?
                if (bufferSize > bufferSizeArray[checkIndex])
                {
                    checkIndex++;
                    continue;
                }
                // Are we at the smallest buffer size?
                if (checkIndex == 0)
                    return 0;
                // Do we need a smaller buffer than the current check index?
                int previousIndex = checkIndex - 1;
                int previousSize = bufferSizeArray[previousIndex];
                if (bufferSize == previousSize)
                    return previousIndex;
                // At this point, if the buffer size is greater than the previous size, then
                // we have found the correct index.
                if (bufferSize > previousSize) return checkIndex;
                checkIndex = previousIndex;
            }
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Gets the index of the buffer size that will hold the given buffer size by walking
        /// the array sequentially (backwards).
        /// </summary>
        /// <param name="bufferSizeArray">
        /// The array of buffer sizes to check against.
        /// </param>
        /// <param name="bufferSize">The needed buffer size</param>
        /// <returns>
        /// The index in the buffer size array that will hold the given buffer size.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int GetSequentialBufferSizeIndex (int[] bufferSizeArray, int bufferSize)
        {
            if (bufferSize <= bufferSizeArray[0])
                return 0;
            for (int checkIndex = bufferSizeArray.Length - 2; checkIndex >= 0; checkIndex--)
            {
                if (bufferSize > bufferSizeArray[checkIndex])
                    return checkIndex + 1;
            }
            Debug.Fail("We should never get to this line because we check the zero index at method enter");
            return 0;
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Gets the index of the buffer size that will hold the given buffer size.
        /// </summary>
        /// <param name="bufferSizeArray">
        /// The array of buffer sizes to check against.
        /// </param>
        /// <param name="bufferSize">The needed buffer size</param>
        /// <returns>
        /// The index in the buffer size array that will hold the given buffer size.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetBufferSizeIndex (int[] bufferSizeArray, int bufferSize)
        {
            Debug.Assert(bufferSize >= 0, "bufferSize < 0");
            return (bufferSizeArray.Length < LargeArraySize) ? 
                GetSequentialBufferSizeIndex(bufferSizeArray, bufferSize) : 
                GetSplitArrayWalkBufferSizeIndex(bufferSizeArray, bufferSize);
        }
        //--------------------------------------------------------------------------------
    }
    //################################################################################
}
