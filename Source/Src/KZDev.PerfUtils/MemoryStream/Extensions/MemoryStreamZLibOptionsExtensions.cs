// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.IO.Compression;

namespace KZDev.PerfUtils;

//################################################################################
/// <summary>
/// Extension methods for <see cref="MemoryStreamZLibOptions"/> instances.
/// </summary>
public static class MemoryStreamZLibOptionsExtensions
{
#if NET9_0_OR_GREATER
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Sets the <see cref="MemoryStreamZLibOptions.CompressionStrategy"/> property on the
    /// <see cref="MemoryStreamZLibOptions"/> instance.
    /// </summary>
    /// <param name="options">
    /// The <see cref="MemoryStreamZLibOptions"/> instance to modify.
    /// </param>
    /// <param name="compressionStrategy">
    /// The compression algorithm to use for the compression operation.
    /// </param>
    /// <returns>
    /// A new <see cref="MemoryStreamZLibOptions"/> instance with the specified
    /// compression algorithm value.
    /// </returns>
    public static MemoryStreamZLibOptions WithCompressionStrategy(this MemoryStreamZLibOptions options,
        ZLibCompressionStrategy compressionStrategy) =>
        options with { CompressionStrategy = compressionStrategy, CompressionLevel = null  };
    //--------------------------------------------------------------------------------
#endif
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Sets the <see cref="MemoryStreamZLibOptions.CompressionLevel"/> property on the
    /// <see cref="MemoryStreamZLibOptions"/> instance.
    /// </summary>
    /// <param name="options">
    /// The <see cref="MemoryStreamZLibOptions"/> instance to modify.
    /// </param>
    /// <param name="compressionLevel">
    /// The compression level to set.
    /// </param>
    /// <returns>
    /// A new <see cref="MemoryStreamZLibOptions"/> instance with the specified compression level.
    /// </returns>
    public static MemoryStreamZLibOptions WithCompressionLevel(this MemoryStreamZLibOptions options,
        CompressionLevel compressionLevel) =>
#if NET9_0_OR_GREATER
        options with { CompressionLevel = compressionLevel, CompressionStrategy = null };
#else
        options with { CompressionLevel = compressionLevel };
#endif
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Sets the <see cref="MemoryStreamSlimOptions.ZeroBufferBehavior"/> property on the
    /// <see cref="MemoryStreamZLibOptions.NewStreamOptions"/> property value.
    /// </summary>
    /// <param name="options">
    /// The <see cref="MemoryStreamZLibOptions"/> instance to modify.
    /// </param>
    /// <param name="zeroBufferBehavior">
    /// The zero buffer behavior to set.
    /// </param>
    /// <returns>
    /// A new <see cref="MemoryStreamZLibOptions"/> instance with the specified zero buffer behavior
    /// applied to the <see cref="MemoryStreamZLibOptions.NewStreamOptions"/> property value.
    /// </returns>
    public static MemoryStreamZLibOptions WithZeroBufferBehavior(this MemoryStreamZLibOptions options,
        MemoryStreamSlimZeroBufferOption zeroBufferBehavior) =>
        options with { NewStreamOptions = options.NewStreamOptions.WithZeroBufferBehavior(zeroBufferBehavior) };
    //--------------------------------------------------------------------------------
}
//################################################################################
