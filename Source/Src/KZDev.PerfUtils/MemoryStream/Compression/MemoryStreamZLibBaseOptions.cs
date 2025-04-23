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
    internal const SystemCompressionLevel DefaultCompressionLevel = SystemCompressionLevel.Optimal;

#if NET9_0_OR_GREATER
    /// <summary>
    /// The compression strategy to use for the compression operation. The default
    /// value is <see cref="ZLibCompressionStrategy.Default"/> <b>[Available in .NET 9.0 and later only]</b>.
    /// </summary>
    public ZLibCompressionStrategy? CompressionStrategy { [DebuggerStepThrough] get; [DebuggerStepThrough] internal set; }
#endif

#if NET9_0_OR_GREATER
    /// If this is not specified, then <see cref="CompressionStrategy"/> is used instead.
    /// <summary>
    /// The compression level to use for the stream. The default value is <see cref="CompressionLevel.Optimal"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>NOTE: For .NET 9.0 and later:</b> If this is not specified (set to null),
    /// then <see cref="CompressionStrategy"/> is used instead.
    /// </para>
    /// </remarks>
#else
    /// <summary>
    /// The compression level to use for the stream. The default value is <see cref="System.IO.Compression.CompressionLevel.Optimal"/>.
    /// If this is not specified, then the default value is used.
    /// </summary>
#endif
    public SystemCompressionLevel? CompressionLevel { [DebuggerStepThrough] get; [DebuggerStepThrough] internal set; } = DefaultCompressionLevel;

    /// <summary>
    /// The options to use when creating a new <see cref="MemoryStreamSlim"/> instance
    /// for compression operations.
    /// </summary>
    public MemoryStreamSlimOptions NewStreamOptions { [DebuggerStepThrough] get; [DebuggerStepThrough] init; } = new();
}
//################################################################################
