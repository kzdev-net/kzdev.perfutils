// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.IO.Compression;
using SystemCompressionLevel = System.IO.Compression.CompressionLevel;

namespace KZDev.PerfUtils;

//################################################################################
/// <summary>
/// Options for how to handle compression operations with a 
/// <see cref="MemoryStreamSlim"/> that utilizes ZLib-Based compression.
/// </summary>
public abstract record MemoryStreamZLibBaseOptions
{
    /// <summary>
    /// The default compression level to use for the stream.
    /// </summary>
    internal const CompressionLevel DefaultCompressionLevel = SystemCompressionLevel.Optimal;

#if NET9_0_OR_GREATER
    /// <summary>
    /// The compression strategy to use for the compression operation. The default
    /// value is <see cref="ZLibCompressionStrategy.Default"/>.
    /// </summary>
    public ZLibCompressionStrategy? CompressionStrategy { [DebuggerStepThrough] get; [DebuggerStepThrough] internal set; }
#endif

#if NET9_0_OR_GREATER
    /// If this is not specified, then <see cref="CompressionStrategy"/> is used instead.
    /// <summary>
    /// The compression level to use for the stream. The default value is <see cref="CompressionLevel.Optimal"/>.
    /// If this is not specified, then <see cref="CompressionStrategy"/> is used instead.
    /// </summary>
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
