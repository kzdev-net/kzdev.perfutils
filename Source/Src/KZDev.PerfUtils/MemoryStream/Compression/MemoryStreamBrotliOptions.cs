// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.IO.Compression;

namespace KZDev.PerfUtils;

//################################################################################
/// <summary>
/// Provides compression options to be used with Brotli compression operations.
/// </summary>
public sealed record MemoryStreamBrotliOptions
{
    /// <summary>
    /// The default compression level to use for the stream.
    /// </summary>
    internal const CompressionLevel DefaultCompressionLevel = System.IO.Compression.CompressionLevel.Optimal;

#if NET9_0_OR_GREATER
    /// <summary>
    /// Gets the compression quality for a Brotli  compression operation. By default, 
    /// this property has no value. If no value is specified, the <see cref="CompressionLevel"/> 
    /// is used instead <b>[Available in .NET 9.0 and later only]</b>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>** NOTE: This property is only available in .NET 9.0 and later. **</b>
    /// </para>
    /// <para>
    /// The higher the quality, the slower the compression. The range is from 0 to 11.
    /// </para>
    /// </remarks>
    /// <seealso cref="BrotliCompressionOptions.Quality"/>
    public int? Quality { [DebuggerStepThrough] get; [DebuggerStepThrough] internal set; } = null;
#endif

#if NET9_0_OR_GREATER
    /// <summary>
    /// The compression level to use for the stream. The default value is <see cref="CompressionLevel.Optimal"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>NOTE: For .NET 9.0 and later:</b> If this is not specified (set to null), 
    /// then <see cref="Quality"/> is used instead. This property is ignored if <see cref="Quality"/> is specified.
    /// </para>
    /// </remarks>
#else
    /// <summary>
    /// The compression level to use for the stream. The default value is <see cref="CompressionLevel.Optimal"/>.
    /// If this is not specified, then the default value is used.
    /// </summary>
#endif
    public CompressionLevel? CompressionLevel { [DebuggerStepThrough] get; [DebuggerStepThrough] internal set; } = DefaultCompressionLevel;

    /// <summary>
    /// The options to use when creating a new <see cref="MemoryStreamSlim"/> instance
    /// for compression operations.
    /// </summary>
    public MemoryStreamSlimOptions NewStreamOptions { [DebuggerStepThrough] get; [DebuggerStepThrough] init; } = new();
}
//################################################################################
