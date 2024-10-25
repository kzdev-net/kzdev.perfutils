// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

namespace KZDev.PerfUtils.Tests
{
    //################################################################################
    /// <summary>
    /// A helper class that provides methods for preparing memory tests and benchmarks.
    /// </summary>
    /// <remarks>
    /// This is in effect a class, but we are using a non-class to 
    /// allow for inheritance.
    /// </remarks>
    public partial class MemoryTestPrep
    {
        /*
         * NOTE
         * To help with concurrency testing within Concura, in the Async methods below, we 
         * use the Synchronous writes to the MemoryStream because...the async overloads 
         * simply return Task.FromResult({SynchronousMethod}) in order to support the async overloads
         * but everything is done in memory, so there is no real async operation.
         * However, the concura runtime wants to be able to fully control the async operations
         * for deterministic testing, but we also want to introduce some randomness in the
         * operations, to we instead chose a random delay or yield operation (or nothing) 
         * to simulate an async operation.
         */
        //--------------------------------------------------------------------------------
        /// <summary>
        /// The last buffer used for copying data.
        /// </summary>
        [ThreadStatic]
        private static byte[]? _lastCopyBuffer;
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Gets a buffer that can be used for copying data. The buffer is guaranteed to be
        /// at least as large as the <paramref name="length"/> parameter.
        /// </summary>
        /// <param name="length">
        /// The length of the buffer requested.
        /// </param>
        /// <returns>
        /// A byte array buffer that can be used for copying data that is at least as large
        /// as the <paramref name="length"/> parameter.
        /// </returns>
        private static byte[] GetTempCopyBuffer (int length)
        {
            if (_lastCopyBuffer is null || _lastCopyBuffer.Length < length)
            {
                _lastCopyBuffer = new byte[length];
            }
            byte[] returnBuffer = _lastCopyBuffer;
            _lastCopyBuffer = null;
            return returnBuffer;
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Stores the passed buffer for later use if it is bigger than the last buffer used.
        /// </summary>
        /// <param name="buffer">
        /// The allocated buffer to return for later use.
        /// </param>
        private static void ReturnTempCopyBuffer (byte[] buffer)
        {
            if (_lastCopyBuffer is null || _lastCopyBuffer.Length < buffer.Length)
            {
                _lastCopyBuffer = buffer;
            }
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Allocates an array of the specified size and fills it with random bytes.
        /// </summary>
        /// <param name="byteCount">
        /// The number of bytes to get in the array.
        /// </param>
        /// <returns>
        /// An array that contains random bytes.
        /// </returns>
        public static byte[] GetRandomByteArray (int byteCount)
        {
            byte[] returnData = new byte[byteCount];
            GetRandomBytes(returnData, byteCount);
            return returnData;
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Fills the provided stream with random bytes. The number of bytes written to the
        /// stream is determined by the <paramref name="byteCount"/> parameter.
        /// </summary>
        /// <remarks>
        /// It is assumed that the stream is writable and that the stream position is at the
        /// location where the random bytes should be written.
        /// </remarks>
        /// <param name="randomSource">
        /// The <see cref="IRandomSource"/> instance to use for generating random bytes.
        /// </param>
        /// <param name="stream">
        /// The stream to fill with random bytes.
        /// </param>
        /// <param name="byteCount">
        /// The number of bytes to write to the stream.
        /// </param>
        /// <param name="byteBuffer">
        /// The buffer to use for writing random bytes to the stream.
        /// </param>
        public static void FillStreamWithRandomBytes (IRandomSource randomSource,
            Stream stream, long byteCount, byte[] byteBuffer)
        {
            int bufferLength = byteBuffer.Length;
            if (bufferLength == 0)
            {
                throw new ArgumentException("The byte buffer must have a length greater than zero.", nameof(byteBuffer));
            }
            if (byteCount < 0)
            {
                throw new ArgumentException("The byte count must be greater than or equal to zero.", nameof(byteCount));
            }
            while (byteCount > 0)
            {
                int bytesToGet = (int)Math.Min(byteCount, GetTestLongInteger(randomSource, bufferLength + 1));
                int bytesToWrite = GetRandomBytes(byteBuffer, bytesToGet);
                stream.Write(byteBuffer, 0, bytesToWrite);
                byteCount -= bytesToWrite;
            }
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Fills the provided stream with random bytes. The number of bytes written to the
        /// stream is determined by the <paramref name="byteCount"/> parameter.
        /// </summary>
        /// <remarks>
        /// It is assumed that the stream is writable and that the stream position is at the
        /// location where the random bytes should be written.
        /// </remarks>
        /// <param name="randomSource">
        /// The <see cref="IRandomSource"/> instance to use for generating random bytes.
        /// </param>
        /// <param name="stream">
        /// The stream to fill with random bytes.
        /// </param>
        /// <param name="byteCount">
        /// The number of bytes to write to the stream.
        /// </param>
        /// <param name="byteBuffer">
        /// The buffer to use for writing random bytes to the stream.
        /// </param>
        public static async Task FillStreamWithRandomBytesWithYieldAsync (IRandomSource randomSource,
            Stream stream, long byteCount, byte[] byteBuffer)
        {
            int bufferLength = byteBuffer.Length;
            if (bufferLength == 0)
            {
                throw new ArgumentException("The byte buffer must have a length greater than zero.", nameof(byteBuffer));
            }
            if (byteCount < 0)
            {
                throw new ArgumentException("The byte count must be greater than or equal to zero.", nameof(byteCount));
            }
            while (byteCount > 0)
            {
                int bytesToGet = (int)Math.Min(byteCount, GetTestLongInteger(randomSource, bufferLength + 1));
                int bytesToWrite = GetRandomBytes(byteBuffer, bytesToGet);
                // Randomly yield or just continue.
                if (randomSource.GetRandomTrue(5))
                {
                    await Task.Yield();
                }
                // ReSharper disable once MethodHasAsyncOverload
                stream.Write(byteBuffer, 0, bytesToWrite);
                byteCount -= bytesToWrite;
            }
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Fills the provided stream with random bytes. The number of bytes written to the
        /// stream is determined by the <paramref name="byteCount"/> parameter.
        /// </summary>
        /// <remarks>
        /// It is assumed that the stream is writable and that the stream position is at the
        /// location where the random bytes should be written.
        /// </remarks>
        /// <param name="randomSource">
        /// The <see cref="IRandomSource"/> instance to use for generating random bytes.
        /// </param>
        /// <param name="stream">
        /// The stream to fill with random bytes.
        /// </param>
        /// <param name="byteCount">
        /// The number of bytes to write to the stream.
        /// </param>
        public static void FillStreamWithRandomBytes (IRandomSource randomSource,
            Stream stream, long byteCount)
        {
            // Provide a copy buffer for the operation.
            byte[] copyBuffer = GetTempCopyBuffer((int)Math.Min(byteCount, 0x10000));
            FillStreamWithRandomBytes(randomSource, stream, byteCount, copyBuffer);
            ReturnTempCopyBuffer(copyBuffer);
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Fills the provided stream with random bytes. The number of bytes written to the
        /// stream is determined by the <paramref name="byteCount"/> parameter.
        /// </summary>
        /// <remarks>
        /// It is assumed that the stream is writable and that the stream position is at the
        /// location where the random bytes should be written.
        /// </remarks>
        /// <param name="randomSource">
        /// The <see cref="IRandomSource"/> instance to use for generating random bytes.
        /// </param>
        /// <param name="stream">
        /// The stream to fill with random bytes.
        /// </param>
        /// <param name="byteCount">
        /// The number of bytes to write to the stream.
        /// </param>
        public static async Task FillStreamWithRandomBytesAsync (IRandomSource randomSource,
            Stream stream, long byteCount)
        {
            // Provide a copy buffer for the operation.
            byte[] copyBuffer = GetTempCopyBuffer((int)Math.Min(byteCount, 0x10000));
            await FillStreamWithRandomBytesWithYieldAsync(randomSource, stream, byteCount, copyBuffer);
            ReturnTempCopyBuffer(copyBuffer);
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Fills the provided stream with random bytes. The number of bytes written to the
        /// stream is determined by the <paramref name="byteCount"/> parameter.
        /// </summary>
        /// <remarks>
        /// It is assumed that the stream is writable and that the stream position is at the
        /// location where the random bytes should be written.
        /// </remarks>
        /// <param name="randomSource">
        /// The <see cref="IRandomSource"/> instance to use for generating random bytes.
        /// </param>
        /// <param name="stream">
        /// The stream to fill with random bytes.
        /// </param>
        /// <param name="byteCount">
        /// The number of bytes to write to the stream.
        /// </param>
        /// <returns>
        /// An array that contains a copy of the bytes written to the stream.
        /// </returns>
        public static byte[] FillStreamAndArrayWithRandomBytes (IRandomSource randomSource,
            Stream stream, int byteCount)
        {
            byte[] returnData = new byte[byteCount];
            GetRandomBytes(returnData, byteCount);
            stream.Write(returnData, 0, byteCount);
            return returnData;
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Fills the provided stream with random bytes. The number of bytes written to the
        /// stream is determined by the <paramref name="byteCount"/> parameter.
        /// </summary>
        /// <remarks>
        /// It is assumed that the stream is writable and that the stream position is at the
        /// location where the random bytes should be written.
        /// </remarks>
        /// <param name="stream">
        /// The stream to fill with random bytes.
        /// </param>
        /// <param name="byteCount">
        /// The number of bytes to write to the stream.
        /// </param>
        /// <returns>
        /// An array that contains a copy of the bytes written to the stream.
        /// </returns>
        public static async Task<byte[]> FillStreamAndArrayWithRandomBytesAsync (Stream stream, int byteCount)
        {
            byte[] returnData = new byte[byteCount];
            GetRandomBytes(returnData, byteCount);
            await stream.WriteAsync(returnData, 0, byteCount);
            return returnData;
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Fills the provided stream with random bytes. The number of bytes written to the
        /// stream is determined by the <paramref name="byteCount"/> parameter.
        /// </summary>
        /// <remarks>
        /// It is assumed that the stream is writable and that the stream position is at the
        /// location where the random bytes should be written.
        /// </remarks>
        /// <param name="randomSource">
        /// The <see cref="IRandomSource"/> instance to use for generating random bytes.
        /// </param>
        /// <param name="stream">
        /// The stream to fill with random bytes.
        /// </param>
        /// <param name="byteCount">
        /// The number of bytes to write to the stream.
        /// </param>
        /// <returns>
        /// An array that contains a copy of the bytes written to the stream.
        /// </returns>
        public static async Task<byte[]> FillStreamAndArrayWithRandomBytesWithYieldAsync (IRandomSource randomSource,
            Stream stream, int byteCount)
        {
            byte[] returnData = new byte[byteCount];
            GetRandomBytes(returnData, byteCount);
            // Randomly yield or just continue.
            if (randomSource.GetRandomTrue(5))
            {
                await Task.Yield();
            }
            await stream.WriteAsync(returnData, 0, byteCount);
            return returnData;
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Fills the provided stream with random bytes. The number of bytes written to the
        /// stream is determined by the <paramref name="byteCount"/> parameter.
        /// </summary>
        /// <remarks>
        /// It is assumed that the stream is writable and that the stream position is at the
        /// location where the random bytes should be written.
        /// </remarks>
        /// <param name="randomSource">
        /// The <see cref="IRandomSource"/> instance to use for generating random bytes.
        /// </param>
        /// <param name="stream">
        /// The stream to fill with random bytes.
        /// </param>
        /// <param name="byteCount">
        /// The number of bytes to write to the stream.
        /// </param>
        /// <returns>
        /// An array that contains a copy of the bytes written to the stream.
        /// </returns>
        /// <param name="bySegmentSize">
        /// The size of each segment used to write the data.
        /// </param>
        public static async Task<byte[]> FillStreamAndArrayWithRandomBytesWithYieldAsync (IRandomSource randomSource,
            Stream stream, int byteCount, int bySegmentSize)
        {
            byte[] returnData = new byte[byteCount];
            GetRandomBytes(returnData, byteCount);
            int bytesLeft = byteCount;
            int byteIndex = 0;
            while (bytesLeft > 0)
            {
                int byteCountToWrite = Math.Min(bySegmentSize, bytesLeft);
                // Randomly yield or just continue.
                if (randomSource.GetRandomTrue(5))
                {
                    await Task.Yield();
                }
                await stream.WriteAsync(returnData, byteIndex, byteCountToWrite);
                byteIndex += byteCountToWrite;
                bytesLeft -= byteCountToWrite;
            }
            return returnData;
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Fills the provided stream with random bytes. The number of bytes written to the
        /// stream is determined by the <paramref name="byteCount"/> parameter. This also 
        /// returns the array of bytes that was written to the stream.
        /// </summary>
        /// <remarks>
        /// It is assumed that the stream is writable and that the stream position is at the
        /// location where the random bytes should be written.
        /// </remarks>
        /// <param name="stream">
        /// The stream to fill with random bytes.
        /// </param>
        /// <param name="byteCount">
        /// The number of bytes to write to the stream.
        /// </param>
        /// <returns>
        /// An array that contains a copy of the bytes written to the stream.
        /// </returns>
        /// <param name="bySegmentSize">
        /// The size of each segment used to write the data.
        /// </param>
        public static byte[] FillStreamAndArrayWithRandomBytes (Stream stream, int byteCount, int bySegmentSize)
        {
            byte[] returnData = new byte[byteCount];
            GetRandomBytes(returnData, byteCount);
            int bytesLeft = byteCount;
            int byteIndex = 0;
            while (bytesLeft > 0)
            {
                int byteCountToWrite = Math.Min(bySegmentSize, bytesLeft);
                stream.Write(returnData, byteIndex, byteCountToWrite);
                byteIndex += byteCountToWrite;
                bytesLeft -= byteCountToWrite;
            }
            return returnData;
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Fills the provided stream with random bytes. The number of bytes written to the
        /// stream is determined by the <paramref name="byteCount"/> parameter. This also 
        /// returns the array of bytes that was written to the stream.
        /// </summary>
        /// <remarks>
        /// It is assumed that the stream is writable and that the stream position is at the
        /// location where the random bytes should be written.
        /// </remarks>
        /// <param name="stream">
        /// The stream to fill with random bytes.
        /// </param>
        /// <param name="byteCount">
        /// The number of bytes to write to the stream.
        /// </param>
        /// <returns>
        /// An array that contains a copy of the bytes written to the stream.
        /// </returns>
        /// <param name="bySegmentSize">
        /// The size of each segment used to write the data.
        /// </param>
        public static async Task<byte[]> FillStreamAndArrayWithRandomBytesAsync (Stream stream, int byteCount, int bySegmentSize)
        {
            byte[] returnData = new byte[byteCount];
            GetRandomBytes(returnData, byteCount);
            int bytesLeft = byteCount;
            int byteIndex = 0;
            bool useMemoryInstance = false;

            while (bytesLeft > 0)
            {
                int byteCountToWrite = Math.Min(bySegmentSize, bytesLeft);
                // Alternate between using the memory instance and the array buffer
                if (useMemoryInstance)
                {
                    Memory<byte> memory = new(returnData, byteIndex, byteCountToWrite);
                    await stream.WriteAsync(memory);
                }
                else
                {
                    await stream.WriteAsync(returnData, byteIndex, byteCountToWrite);
                }
                byteIndex += byteCountToWrite;
                bytesLeft -= byteCountToWrite;
                useMemoryInstance = !useMemoryInstance;
            }
            return returnData;
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Fills the provided stream with random bytes. The number of bytes written to the
        /// stream is determined by the <paramref name="byteCount"/> parameter.
        /// </summary>
        /// <remarks>
        /// It is assumed that the stream is writable and that the stream position is at the
        /// location where the random bytes should be written.
        /// </remarks>
        /// <param name="stream">
        /// The stream to fill with random bytes.
        /// </param>
        /// <param name="byteCount">
        /// The number of bytes to write to the stream.
        /// </param>
        /// <returns>
        /// An array that contains a copy of the bytes written to the stream.
        /// </returns>
        /// <param name="bySegmentSize">
        /// The size of each segment used to write the data.
        /// </param>
        public static void FillStreamWithRandomBytes (Stream stream, int byteCount, int bySegmentSize)
        {
            byte[] fillData = new byte[byteCount];
            GetRandomBytes(fillData, byteCount);
            int bytesLeft = byteCount;
            int byteIndex = 0;
            while (bytesLeft > 0)
            {
                int byteCountToWrite = Math.Min(bySegmentSize, bytesLeft);
                stream.Write(fillData, byteIndex, byteCountToWrite);
                byteIndex += byteCountToWrite;
                bytesLeft -= byteCountToWrite;
            }
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Fills the provided stream with random bytes. The number of bytes written to the
        /// stream is determined by the <paramref name="byteCount"/> parameter.
        /// </summary>
        /// <remarks>
        /// It is assumed that the stream is writable and that the stream position is at the
        /// location where the random bytes should be written.
        /// </remarks>
        /// <param name="stream">
        /// The stream to fill with random bytes.
        /// </param>
        /// <param name="byteCount">
        /// The number of bytes to write to the stream.
        /// </param>
        /// <returns>
        /// An array that contains a copy of the bytes written to the stream.
        /// </returns>
        /// <param name="bySegmentSize">
        /// The size of each segment used to write the data.
        /// </param>
        public static async Task FillStreamWithRandomBytesAsync (Stream stream, int byteCount, int bySegmentSize)
        {
            byte[] fillData = new byte[byteCount];
            GetRandomBytes(fillData, byteCount);
            int bytesLeft = byteCount;
            int byteIndex = 0;
            bool useMemoryInstance = false;

            while (bytesLeft > 0)
            {
                int byteCountToWrite = Math.Min(bySegmentSize, bytesLeft);
                // Alternate between using the memory instance and the array buffer
                if (useMemoryInstance)
                {
                    Memory<byte> memory = new(fillData, byteIndex, byteCountToWrite);
                    await stream.WriteAsync(memory);
                }
                else
                {
                    await stream.WriteAsync(fillData, byteIndex, byteCountToWrite);
                }
                byteIndex += byteCountToWrite;
                bytesLeft -= byteCountToWrite;
                useMemoryInstance = !useMemoryInstance;
            }
        }
        //--------------------------------------------------------------------------------
    }
    //################################################################################
}
