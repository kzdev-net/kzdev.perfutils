// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.IO.Compression;

namespace KZDev.PerfUtils;

//################################################################################
/// <summary>
/// Extension methods for <see cref="MemoryStreamBrotliOptions"/> instances.
/// </summary>
public static class MemoryStreamBrotliOptionsExtensions
{
#if NET9_0_OR_GREATER
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Sets the <see cref="MemoryStreamBrotliOptions.Quality"/> property on the
    /// <see cref="MemoryStreamBrotliOptions"/> instance <b>[Available in .NET 9.0 and later only]</b>.
    /// </summary>
    /// <param name="options">
    /// The <see cref="MemoryStreamBrotliOptions"/> instance to modify.
    /// </param>
    /// <param name="quality">
    /// The compression quality for the Brotli compression.
    /// </param>
    /// <returns>
    /// A new <see cref="MemoryStreamBrotliOptions"/> instance with the specified
    /// compression quality value.
    /// </returns>
    public static MemoryStreamBrotliOptions WithQuality(this MemoryStreamBrotliOptions options,
        int quality) =>
        options with { Quality = quality, CompressionLevel = null };
    //--------------------------------------------------------------------------------
#endif
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Sets the <see cref="MemoryStreamBrotliOptions.CompressionLevel"/> property on the
    /// <see cref="MemoryStreamBrotliOptions"/> instance.
    /// </summary>
    /// <param name="options">
    /// The <see cref="MemoryStreamBrotliOptions"/> instance to modify.
    /// </param>
    /// <param name="compressionLevel">
    /// The compression level to set.
    /// </param>
    /// <returns>
    /// A new <see cref="MemoryStreamBrotliOptions"/> instance with the specified compression level.
    /// </returns>
    public static MemoryStreamBrotliOptions WithCompressionLevel(this MemoryStreamBrotliOptions options,
        CompressionLevel compressionLevel) =>
#if NET9_0_OR_GREATER
        options with { CompressionLevel = compressionLevel, Quality = null };
#else
        options with { CompressionLevel = compressionLevel };
#endif
    //--------------------------------------------------------------------------------
}
//###################################################