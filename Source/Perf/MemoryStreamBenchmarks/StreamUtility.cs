// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

namespace MemoryStreamBenchmarks
{
    //################################################################################
    /// <summary>
    /// Helper class for filling a stream with random data and reading it back.
    /// </summary>
    public class StreamUtility
    {
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Test to fill the stream with the passed fill data and then read it back into the read buffer
        /// </summary>
        /// <param name="stream">
        /// The stream instance being used.
        /// </param>
        /// <param name="destinationStream">
        /// The asynchronous stream to copy the data to
        /// </param>
        /// <param name="fillData">
        /// The test data used to fill the stream
        /// </param>
        /// <param name="dataLength">
        /// The length of the data to fill and read back
        /// </param>
        public async Task CopyToAsync (Stream stream, Stream destinationStream, byte[] fillData, 
            int dataLength)
        {
            stream.Position = 0;
            destinationStream.Position = 0;
            // Write synchronously to the source stream to fill it as rapidly as possible
            // (this is not what we are benchmarking)
            // ReSharper disable once MethodHasAsyncOverload
            stream.Write(fillData, 0, dataLength);

            // Reset the position to the start of the stream for copying
            stream.Position = 0;
            // Copy asynchronously to the destination stream
            await stream.CopyToAsync(destinationStream);
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Test to fill the stream with the passed fill data and then read it back into the read buffer
        /// </summary>
        /// <param name="stream">
        /// The stream instance being used.
        /// </param>
        /// <param name="fillData">
        /// The test data used to fill the stream
        /// </param>
        /// <param name="readBuffer">
        /// The buffer to read back the data to
        /// </param>
        /// <param name="dataLength">
        /// The length of the data to fill and read back
        /// </param>
        public void BulkFillAndRead (Stream stream, byte[] fillData, byte[] readBuffer, int dataLength)
        {
            stream.Write(fillData, 0, dataLength);
            // Reset the position to the start of the stream for reading
            stream.Position = 0;
            if (stream.Read(readBuffer, 0, dataLength) < dataLength)
                throw new Exception("Failed to read all data back");
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Test to fill the stream with the passed fill data and then read it back into the read buffer
        /// </summary>
        /// <param name="stream">
        /// The stream instance being used.
        /// </param>
        /// <param name="fillData">
        /// The test data used to fill the stream
        /// </param>
        /// <param name="readBuffer">
        /// The buffer to read back the data to
        /// </param>
        /// <param name="dataLength">
        /// The length of the data to fill and read back
        /// </param>
        /// <param name="segmentLength">
        /// The length of the segment to fill with each call and to read back with each call
        /// </param>
        public void SegmentFillAndRead (Stream stream, byte[] fillData, byte[] readBuffer, int dataLength, int segmentLength)
        {
            int writeSizeLeft = dataLength;
            while (writeSizeLeft > 0)
            {
                int writeSize = Math.Min(writeSizeLeft, segmentLength);
                stream.Write(fillData, 0, writeSize);
                writeSizeLeft -= writeSize;
            }

            // Reset the position to the start of the stream for reading
            stream.Position = 0;
            int readSizeLeft = dataLength;
            while (readSizeLeft > 0)
            {
                int readSize = Math.Min(readSizeLeft, segmentLength);
                int readInSize = stream.Read(readBuffer, 0, readSize);
                if (readInSize < readSize)
                    throw new Exception("Failed to read all data back");
                readSizeLeft -= readInSize;
            }
        }
        //--------------------------------------------------------------------------------
    }
    //################################################################################
}
