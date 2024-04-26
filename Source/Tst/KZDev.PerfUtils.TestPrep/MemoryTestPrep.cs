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
    public partial class MemoryTestPrep : TestPrepBase
    {
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
        /// <param name="byteBuffer">
        /// The buffer to use for writing random bytes to the stream.
        /// </param>
        public void FillStreamWithRandomBytes (Stream stream, long byteCount, byte[] byteBuffer) =>
            FillStreamWithRandomBytes(RandomSource, stream, byteCount, byteBuffer);
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
        /// <param name="byteBuffer">
        /// The buffer to use for writing random bytes to the stream.
        /// </param>
        public Task FillStreamWithRandomBytesAsync (Stream stream, long byteCount,
            byte[] byteBuffer) => FillStreamWithRandomBytesWithYieldAsync(RandomSource, stream, byteCount, byteBuffer);
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
        public void FillStreamWithRandomBytes (Stream stream, long byteCount) =>
            FillStreamWithRandomBytes(RandomSource, stream, byteCount);
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
        public Task FillStreamWithRandomBytesAsync (Stream stream, long byteCount) =>
            FillStreamWithRandomBytesAsync(stream, byteCount, GetTempCopyBuffer((int)Math.Min(byteCount, 0x10000)));
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
        public byte[] FillStreamAndArrayWithRandomBytes (Stream stream, int byteCount) =>
            FillStreamAndArrayWithRandomBytes(RandomSource, stream, byteCount);
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
        public Task<byte[]> FillStreamAndArrayWithRandomBytesWithYieldAsync (Stream stream,
            int byteCount) => FillStreamAndArrayWithRandomBytesWithYieldAsync(RandomSource, stream, byteCount);
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
        public Task<byte[]> FillStreamAndArrayWithRandomBytesWithYieldAsync (Stream stream,
            int byteCount, int bySegmentSize) =>
            FillStreamAndArrayWithRandomBytesWithYieldAsync(RandomSource, stream, byteCount, bySegmentSize);
        //--------------------------------------------------------------------------------
    }
    //################################################################################
}
