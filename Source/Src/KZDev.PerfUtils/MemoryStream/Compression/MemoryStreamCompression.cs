// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Text;
using KZDev.PerfUtils.Helpers;
using KZDev.PerfUtils.Internals;

namespace KZDev.PerfUtils;

/// <summary>
/// Provides methods for compressing and decompressing data into and out of 
/// <see cref="MemoryStreamSlim"/> instances using various compression algorithms.
/// </summary>
internal static class MemoryStreamCompression
{
    // This is the maximum size of the buffer that will be allocated for compression
    // operations. This is a hard limit to prevent excessive memory usage.
    // The default value is set to 512 MB, which should be sufficient for most use cases.
    // This value is used to set the initial capacity of the stream when it is created.
    // Any size source data below this value will be used as the set capacity for the stream.
    private const int MaxAnticipatedCompressionSize = 0x1000_0000; // 256 MB

    // This is the maximum size of the buffer that will be allocated for decompression
    // operations. This is a hard limit to prevent excessive memory usage.
    // The default value is set to 512 MB, which should be sufficient for most use cases.
    // This value is used to set the initial capacity of the stream when it is created.
    // Any size source data below this value will be used as the set capacity for the stream.
    private const int MaxAllowedDecompressionAllocatedCapacity = 0x2000_0000; // 512 MB

    /// <summary>
    /// The default encoding to use for string compression operations.
    /// </summary>
    public static readonly Encoding DefaultEncoding = Encoding.UTF8;

    //--------------------------------------------------------------------------------
    /// <summary>
    /// Validates the source stream for compression operations. This ensures that the 
    /// source stream is not null, is readable, and is seekable. If any of these conditions
    /// are not met, an exception is thrown.
    /// </summary>
    /// <param name="source">
    /// The source stream to validate for compression operations. This stream must be
    /// readable and seekable to ensure that the compression operation can be performed.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when the source stream is null. This indicates that a valid stream must 
    /// be provided for compression.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when the source stream is not readable or not seekable.
    /// </exception>
    public static void ValidateCompression(Stream source)
    {
        if (source is null)
            throw new ArgumentNullException(nameof(source));
        if (source.CanRead is false)
            ThrowHelper.ThrowArgumentException_SourceStreamMustBeReadable(nameof(source));
        if (source.CanSeek is false)
            ThrowHelper.ThrowArgumentException_SourceStreamMustBeSeekable(nameof(source));
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Creates a new <see cref="MemoryStreamSlim"/> instance for compression output with
    /// an initial capacity based on the source stream's length. This method is used to optimize
    /// memory usage by setting the initial capacity to a value that is efficient for the
    /// compression operation.
    /// </summary>
    /// <param name="sourceLength">
    /// The length of the source data to compress. This is used to determine the initial capacity
    /// of the stream.
    /// </param>
    /// <returns>
    /// A new <see cref="MemoryStreamSlim"/> instance with a capacity set to an efficient 
    /// value based on the source stream's length. The capacity is set to the minimum of the
    /// source stream's length and the maximum anticipated compression size to optimize memory usage.
    /// </returns>
    public static MemoryStreamSlim CreateCompressionOutput(long sourceLength)
    {
        return sourceLength <= (MemorySegmentedBufferGroup.StandardBufferSegmentSize << 1) ? MemoryStreamSlim.Create() :
            // In order to get some reasonable initial capacity, set the capacity to 2/3 of the 
            // source length.
            MemoryStreamSlim.Create(Math.Min((sourceLength / 3) << 1, MaxAnticipatedCompressionSize));
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Creates a new <see cref="MemoryStreamSlim"/> instance for compression output with
    /// an initial capacity based on the source stream's length and the provided options. 
    /// This method is used to optimize memory usage by setting the initial capacity to a 
    /// value that is efficient for the compression operation.
    /// </summary>
    /// <param name="sourceLength">
    /// The length of the source data to compress. This is used to determine the initial capacity
    /// of the stream.
    /// </param>
    /// <param name="options">
    /// Options to configure the stream instance.
    /// </param>
    /// <returns>
    /// A new <see cref="MemoryStreamSlim"/> instance with a capacity set to an efficient 
    /// value based on the source stream's length. The capacity is set to the minimum of the
    /// source stream's length and the maximum anticipated compression size to optimize memory usage.
    /// </returns>
    public static MemoryStreamSlim CreateCompressionOutput(long sourceLength, in MemoryStreamSlimOptions options)
    {
        return sourceLength <= (MemorySegmentedBufferGroup.StandardBufferSegmentSize << 1) ? MemoryStreamSlim.Create(options) :
            // In order to get some reasonable initial capacity, set the capacity to 2/3 of the 
            // source length.
            MemoryStreamSlim.Create(Math.Min((sourceLength / 3) << 1, MaxAnticipatedCompressionSize), options);
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Creates a new <see cref="MemoryStreamSlim"/> instance for compression output.
    /// </summary>
    /// <param name="source">
    /// The source data to compress.
    /// </param>
    /// <returns>
    /// A new <see cref="MemoryStreamSlim"/> instance with a capacity set to an efficient 
    /// value based on the source stream's length. The capacity is set to the minimum of the
    /// source stream's length and the maximum anticipated compression size to optimize memory usage.
    /// </returns>
    public static MemoryStreamSlim CreateCompressionOutput(byte[] source) =>
        CreateCompressionOutput(source.Length);
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Creates a new <see cref="MemoryStreamSlim"/> instance for compression output.
    /// </summary>
    /// <param name="source">
    /// The source data to compress.
    /// </param>
    /// <param name="options">
    /// Options to configure the stream instance.
    /// </param>
    /// <returns>
    /// A new <see cref="MemoryStreamSlim"/> instance with a capacity set to an efficient 
    /// value based on the source stream's length. The capacity is set to the minimum of the
    /// source stream's length and the maximum anticipated compression size to optimize memory usage.
    /// </returns>
    public static MemoryStreamSlim CreateCompressionOutput(byte[] source, in MemoryStreamSlimOptions options) =>
        CreateCompressionOutput(source.Length, options);
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Creates a new <see cref="MemoryStreamSlim"/> instance for compression output.
    /// </summary>
    /// <param name="source">
    /// The source data to compress.
    /// </param>
    /// <returns>
    /// A new <see cref="MemoryStreamSlim"/> instance with a capacity set to an efficient 
    /// value based on the source stream's length. The capacity is set to the minimum of the
    /// source stream's length and the maximum anticipated compression size to optimize memory usage.
    /// </returns>
    public static MemoryStreamSlim CreateCompressionOutput(in ReadOnlySpan<byte> source) =>
        CreateCompressionOutput(source.Length);
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Creates a new <see cref="MemoryStreamSlim"/> instance for compression output.
    /// </summary>
    /// <param name="source">
    /// The source data to compress.
    /// </param>
    /// <param name="options">
    /// Options to configure the stream instance.
    /// </param>
    /// <returns>
    /// A new <see cref="MemoryStreamSlim"/> instance with a capacity set to an efficient 
    /// value based on the source stream's length. The capacity is set to the minimum of the
    /// source stream's length and the maximum anticipated compression size to optimize memory usage.
    /// </returns>
    public static MemoryStreamSlim CreateCompressionOutput(in ReadOnlySpan<byte> source, in MemoryStreamSlimOptions options) =>
        CreateCompressionOutput(source.Length, options);
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Creates a new <see cref="MemoryStreamSlim"/> instance for compression output.
    /// </summary>
    /// <param name="source">
    /// The source data to compress.
    /// </param>
    /// <returns>
    /// A new <see cref="MemoryStreamSlim"/> instance with a capacity set to an efficient 
    /// value based on the source stream's length. The capacity is set to the minimum of the
    /// source stream's length and the maximum anticipated compression size to optimize memory usage.
    /// </returns>
    public static MemoryStreamSlim CreateCompressionOutput(string source) =>
        CreateCompressionOutput(source.Length * sizeof(char));
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Creates a new <see cref="MemoryStreamSlim"/> instance for compression output.
    /// </summary>
    /// <param name="source">
    /// The source data to compress.
    /// </param>
    /// <param name="options">
    /// Options to configure the stream instance.
    /// </param>
    /// <returns>
    /// A new <see cref="MemoryStreamSlim"/> instance with a capacity set to an efficient 
    /// value based on the source stream's length. The capacity is set to the minimum of the
    /// source stream's length and the maximum anticipated compression size to optimize memory usage.
    /// </returns>
    public static MemoryStreamSlim CreateCompressionOutput(string source, in MemoryStreamSlimOptions options) =>
        CreateCompressionOutput(source.Length * sizeof(char), options);
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Creates a new <see cref="MemoryStreamSlim"/> instance for compression output.
    /// </summary>
    /// <param name="source">
    /// The source data to compress.
    /// </param>
    /// <param name="encoding">
    /// The character encoding to use for the source data.
    /// </param>
    /// <returns>
    /// A new <see cref="MemoryStreamSlim"/> instance with a capacity set to an efficient 
    /// value based on the source stream's length. The capacity is set to the minimum of the
    /// source stream's length and the maximum anticipated compression size to optimize memory usage.
    /// </returns>
    public static MemoryStreamSlim CreateCompressionOutput(string source, Encoding encoding) =>
        CreateCompressionOutput(source.Length * (encoding.IsSingleByte ? 1 : sizeof(char)));
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Creates a new <see cref="MemoryStreamSlim"/> instance for compression output.
    /// </summary>
    /// <param name="source">
    /// The source data to compress.
    /// </param>
    /// <param name="encoding">
    /// The character encoding to use for the source data.
    /// </param>
    /// <param name="options">
    /// Options to configure the stream instance.
    /// </param>
    /// <returns>
    /// A new <see cref="MemoryStreamSlim"/> instance with a capacity set to an efficient 
    /// value based on the source stream's length. The capacity is set to the minimum of the
    /// source stream's length and the maximum anticipated compression size to optimize memory usage.
    /// </returns>
    public static MemoryStreamSlim CreateCompressionOutput(string source, Encoding encoding, in MemoryStreamSlimOptions options) =>
        CreateCompressionOutput(source.Length * (encoding.IsSingleByte ? 1 : sizeof(char)), options);
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Creates a new <see cref="MemoryStreamSlim"/> instance for compression output.
    /// </summary>
    /// <param name="source">
    /// The source stream to use for compression. This stream must be readable and seekable.
    /// </param>
    /// <returns>
    /// A new <see cref="MemoryStreamSlim"/> instance with a capacity set to an efficient 
    /// value based on the source stream's length. The capacity is set to the minimum of the
    /// source stream's length and the maximum anticipated compression size to optimize memory usage.
    /// </returns>
    public static MemoryStreamSlim CreateCompressionOutput(Stream source) =>
        CreateCompressionOutput(source.Length);
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Creates a new <see cref="MemoryStreamSlim"/> instance for compression output.
    /// </summary>
    /// <param name="source">
    /// The source stream to use for compression. This stream must be readable and seekable.
    /// </param>
    /// <param name="options">
    /// Options to configure the stream instance.
    /// </param>
    /// <returns>
    /// A new <see cref="MemoryStreamSlim"/> instance with a capacity set to an efficient 
    /// value based on the source stream's length. The capacity is set to the minimum of the
    /// source stream's length and the maximum anticipated compression size to optimize memory usage.
    /// </returns>
    public static MemoryStreamSlim CreateCompressionOutput(Stream source, in MemoryStreamSlimOptions options) =>
        CreateCompressionOutput(source.Length, options);
    //--------------------------------------------------------------------------------

    //--------------------------------------------------------------------------------
    /// <summary>
    /// Creates a new <see cref="MemoryStreamSlim"/> instance for decompression output with
    /// an initial capacity based on the source stream's length. This method is used to optimize
    /// memory usage by setting the initial capacity to a value that is efficient for the
    /// decompression operation.
    /// </summary>
    /// <param name="sourceLength">
    /// The length of the source data to decompress. This is used to determine the initial capacity
    /// of the stream.
    /// </param>
    /// <returns>
    /// A new <see cref="MemoryStreamSlim"/> instance with a capacity set to an efficient 
    /// value based on the source stream's length. The capacity is set to the minimum of the
    /// source stream's length and the maximum anticipated decompression size to optimize memory usage.
    /// </returns>
    public static MemoryStreamSlim CreateDecompressionOutput(long sourceLength)
    {
        return sourceLength <= (MemorySegmentedBufferGroup.StandardBufferSegmentSize >> 1) ? MemoryStreamSlim.Create() :
            (sourceLength >= MaxAllowedDecompressionAllocatedCapacity ? MemoryStreamSlim.Create(MaxAllowedDecompressionAllocatedCapacity) :
                // In order to get some reasonable initial capacity, set the capacity to 1.25 of the 
                // source length.
                MemoryStreamSlim.Create(Math.Min(sourceLength + (sourceLength >> 2), MaxAllowedDecompressionAllocatedCapacity)));
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Creates a new <see cref="MemoryStreamSlim"/> instance for decompression output with
    /// an initial capacity based on the source stream's length and the provided options. 
    /// This method is used to optimize memory usage by setting the initial capacity to a 
    /// value that is efficient for the decompression operation.
    /// </summary>
    /// <param name="sourceLength">
    /// The length of the source data to decompress. This is used to determine the initial capacity
    /// of the stream.
    /// </param>
    /// <param name="options">
    /// Options to configure the stream instance.
    /// </param>
    /// <returns>
    /// A new <see cref="MemoryStreamSlim"/> instance with a capacity set to an efficient 
    /// value based on the source stream's length. The capacity is set to the minimum of the
    /// source stream's length and the maximum anticipated decompression size to optimize memory usage.
    /// </returns>
    public static MemoryStreamSlim CreateDecompressionOutput(long sourceLength, in MemoryStreamSlimOptions options)
    {
        return sourceLength <= (MemorySegmentedBufferGroup.StandardBufferSegmentSize << 1) ? MemoryStreamSlim.Create(options) :
            (sourceLength >= MaxAllowedDecompressionAllocatedCapacity ? MemoryStreamSlim.Create(MaxAllowedDecompressionAllocatedCapacity, options) :
            // In order to get some reasonable initial capacity, set the capacity to 1.25 of the 
            // source length.
            MemoryStreamSlim.Create(Math.Min(sourceLength + (sourceLength >> 2), MaxAllowedDecompressionAllocatedCapacity), options));
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Creates a new <see cref="MemoryStreamSlim"/> instance for decompression output.
    /// </summary>
    /// <param name="source">
    /// The source stream to use for decompression. This stream must be readable and seekable.
    /// </param>
    /// <returns>
    /// A new <see cref="MemoryStreamSlim"/> instance with a capacity set to an efficient 
    /// value based on the source stream's length. The capacity is set to the minimum of the
    /// source stream's length and the maximum anticipated decompression size to optimize memory usage.
    /// </returns>
    public static MemoryStreamSlim CreateDecompressionOutput(Stream source) =>
        CreateDecompressionOutput(source.Length);
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Creates a new <see cref="MemoryStreamSlim"/> instance for decompression output.
    /// </summary>
    /// <param name="source">
    /// The source stream to use for decompression. This stream must be readable and seekable.
    /// </param>
    /// <param name="options">
    /// Options to configure the stream instance.
    /// </param>
    /// <returns>
    /// A new <see cref="MemoryStreamSlim"/> instance with a capacity set to an efficient 
    /// value based on the source stream's length. The capacity is set to the minimum of the
    /// source stream's length and the maximum anticipated decompression size to optimize memory usage.
    /// </returns>
    public static MemoryStreamSlim CreateDecompressionOutput(Stream source, in MemoryStreamSlimOptions options) =>
        CreateDecompressionOutput(source.Length, options);
    //--------------------------------------------------------------------------------
}