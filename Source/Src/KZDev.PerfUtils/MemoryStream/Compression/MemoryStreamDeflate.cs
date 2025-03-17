// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.IO.Compression;
using System.Text;

namespace KZDev.PerfUtils;

/// <summary>
/// Provides methods for compressing and decompressing data into and out of 
/// <see cref="MemoryStreamSlim"/> instances using the Deflate compression algorithm.
/// </summary>
public static class MemoryStreamDeflate
{
    #region Compression Helpers

    //--------------------------------------------------------------------------------
    /// <summary>
    /// Performs the actual compression operation on the source byte array.
    /// </summary>
    /// <param name="source">
    /// The source data to compress.
    /// </param>
    /// <param name="destination">
    /// The destination stream to write the compressed data to. This stream must be writable and seekable.
    /// </param>
    /// <param name="compressionOptions">
    /// The options to use for the compression operation.
    /// </param>
    private static void Compress(byte[] source, MemoryStreamSlim destination,
        MemoryStreamDeflateOptions? compressionOptions)
    {
        // Create the compression stream and write the source data to it.
#if NET9_0_OR_GREATER
        using (DeflateStream compressionStream = (compressionOptions is null) ?
            new(destination, MemoryStreamZLibBaseOptions.DefaultCompressionLevel, true) :
            (compressionOptions.CompressionStrategy.HasValue ?
            new(destination, new ZLibCompressionOptions { CompressionStrategy = compressionOptions.CompressionStrategy.Value }, true) :
            new DeflateStream(destination, compressionOptions.CompressionLevel!.Value, true)))
#else
        using (DeflateStream compressionStream = new(destination,
            compressionOptions?.CompressionLevel ?? MemoryStreamZLibBaseOptions.DefaultCompressionLevel, true))
#endif
        {
            // Do the actual compression operation
            compressionStream.Write(source, 0, source.Length);
        }
        destination.Position = 0;
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Performs the actual compression operation on the source byte span.
    /// </summary>
    /// <param name="source">
    /// The source data to compress.
    /// </param>
    /// <param name="destination">
    /// The destination stream to write the compressed data to. This stream must be writable and seekable.
    /// </param>
    /// <param name="compressionOptions">
    /// The options to use for the compression operation.
    /// </param>
    private static void Compress(in ReadOnlySpan<byte> source, MemoryStreamSlim destination,
        MemoryStreamDeflateOptions? compressionOptions)
    {
        // Create the compression stream and write the source data to it.
#if NET9_0_OR_GREATER
        using (DeflateStream compressionStream = (compressionOptions is null) ?
            new(destination, MemoryStreamZLibBaseOptions.DefaultCompressionLevel, true) :
            (compressionOptions.CompressionStrategy.HasValue ?
                new(destination, new ZLibCompressionOptions { CompressionStrategy = compressionOptions.CompressionStrategy.Value }, true) :
                new DeflateStream(destination, compressionOptions.CompressionLevel!.Value, true)))
#else
        using (DeflateStream compressionStream = new(destination,
            compressionOptions?.CompressionLevel ?? MemoryStreamZLibBaseOptions.DefaultCompressionLevel, true))
#endif
        {
            // Do the actual compression operation
            compressionStream.Write(source);
        }
        destination.Position = 0;
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Performs the actual compression operation on the source string.
    /// </summary>
    /// <param name="source">
    /// The source data to compress.
    /// </param>
    /// <param name="destination">
    /// The destination stream to write the compressed data to. This stream must be writable and seekable.
    /// </param>
    /// <param name="compressionOptions">
    /// The options to use for the compression operation.
    /// </param>
    private static void Compress(string source, MemoryStreamSlim destination,
        MemoryStreamDeflateOptions? compressionOptions)
    {
        Compress(MemoryStreamCompression.DefaultEncoding.GetBytes(source), destination, compressionOptions);
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Performs the actual compression operation on the source string and encoding.
    /// </summary>
    /// <param name="source">
    /// The source data to compress.
    /// </param>
    /// <param name="encoding">
    /// The character encoding to use.
    /// </param>
    /// <param name="destination">
    /// The destination stream to write the compressed data to. This stream must be writable and seekable.
    /// </param>
    /// <param name="compressionOptions">
    /// The options to use for the compression operation.
    /// </param>
    private static void Compress(string source, Encoding encoding, MemoryStreamSlim destination,
        MemoryStreamDeflateOptions? compressionOptions)
    {
        Compress(encoding.GetBytes(source), destination, compressionOptions);
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Performs the actual compression operation on the source stream.
    /// </summary>
    /// <param name="source">
    /// The source stream to compress. This stream must be readable and seekable.
    /// </param>
    /// <param name="destination">
    /// The destination stream to write the compressed data to. This stream must be writable and seekable.
    /// </param>
    /// <param name="compressionOptions">
    /// The options to use for the compression operation.
    /// </param>
    private static void Compress(Stream source, MemoryStreamSlim destination,
        MemoryStreamDeflateOptions? compressionOptions)
    {
        long startPosition = source.Position;
        source.Position = 0;
        // Create the compression stream and write the source data to it.
#if NET9_0_OR_GREATER
        using (DeflateStream compressionStream = (compressionOptions is null) ?
            new(destination, MemoryStreamZLibBaseOptions.DefaultCompressionLevel, true) :
            (compressionOptions.CompressionStrategy.HasValue ?
                new(destination, new ZLibCompressionOptions { CompressionStrategy = compressionOptions.CompressionStrategy.Value }, true) :
                new DeflateStream(destination, compressionOptions.CompressionLevel!.Value, true)))
#else
        using (DeflateStream compressionStream = new(destination,
            compressionOptions?.CompressionLevel ?? MemoryStreamZLibBaseOptions.DefaultCompressionLevel, true))
#endif
        {
            // Do the actual compression operation
            source.CopyTo(compressionStream);
        }
        destination.Position = 0;
        source.Position = startPosition;
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Performs the actual compression operation on the source stream.
    /// </summary>
    /// <param name="source">
    /// The source stream to compress. This stream must be readable and seekable.
    /// </param>
    /// <param name="destination">
    /// The destination stream to write the compressed data to. This stream must be writable and seekable.
    /// </param>
    /// <param name="compressionOptions">
    /// The options to use for the compression operation.
    /// </param>
    /// <param name="cancellationToken">
    /// The cancellation token to use for the operation.
    /// </param>
    private static async Task CompressAsync(Stream source, MemoryStreamSlim destination,
        MemoryStreamDeflateOptions? compressionOptions, CancellationToken cancellationToken)
    {
        long startPosition = source.Position;
        source.Position = 0;
        // Create the compression stream and write the source data to it.
#if NET9_0_OR_GREATER
        await using (DeflateStream compressionStream = (compressionOptions is null) ?
            new(destination, MemoryStreamZLibBaseOptions.DefaultCompressionLevel, true) :
            (compressionOptions.CompressionStrategy.HasValue ?
                new(destination, new ZLibCompressionOptions { CompressionStrategy = compressionOptions.CompressionStrategy.Value }, true) :
                new DeflateStream(destination, compressionOptions.CompressionLevel!.Value, true)))
#else
        await using (DeflateStream compressionStream = new(destination,
            compressionOptions?.CompressionLevel ?? MemoryStreamZLibBaseOptions.DefaultCompressionLevel, true))
#endif
        {
            // Do the actual compression operation
            await source.CopyToAsync(compressionStream, cancellationToken).ConfigureAwait(false);
        }
        destination.Position = 0;
        source.Position = startPosition;
    }
    //--------------------------------------------------------------------------------

    #endregion Compression Helpers

    #region Decompression Helpers

    //--------------------------------------------------------------------------------
    /// <summary>
    /// Performs the actual decompression operation on the source stream into the 
    /// destination stream.
    /// </summary>
    /// <param name="source">
    /// The source stream to decompress. This stream must be readable and seekable.
    /// </param>
    /// <param name="destination">
    /// The destination stream to write the compressed data to. This stream must be writable and seekable.
    /// </param>
    private static void Decompress(Stream source, MemoryStreamSlim destination)
    {
        long startPosition = source.Position;
        destination.Position = 0;
        source.Position = 0;
        // Create the decompression stream and write the decompressed data to the destination stream.
        using (DeflateStream compressionStream = new(source, CompressionMode.Decompress, true))
        {
            // Do the actual decompression operation
            compressionStream.CopyTo(destination);
        }
        destination.Position = 0;
        source.Position = startPosition;
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Performs the actual decompression operation on the source stream into the 
    /// destination stream.
    /// </summary>
    /// <param name="source">
    /// The source stream to decompress. This stream must be readable and seekable.
    /// </param>
    /// <param name="destination">
    /// The destination stream to write the compressed data to. This stream must be writable and seekable.
    /// </param>
    /// <param name="cancellationToken">
    /// The cancellation token to use for the operation.
    /// </param>
    private static async Task DecompressAsync(Stream source, MemoryStreamSlim destination,
        CancellationToken cancellationToken)
    {
        long startPosition = source.Position;
        destination.Position = 0;
        source.Position = 0;
        // Create the decompression stream and write the decompressed data to the destination stream.
        await using (DeflateStream compressionStream = new(source, CompressionMode.Decompress, true))
        {
            // Do the actual decompression operation
            await compressionStream.CopyToAsync(destination, cancellationToken).ConfigureAwait(false);
        }
        destination.Position = 0;
        source.Position = startPosition;
    }
    //--------------------------------------------------------------------------------

    #endregion Decompression Helpers

    #region Public Compression Methods

    //--------------------------------------------------------------------------------
    /// <summary>
    /// Compresses the data in the source byte array and returns a new 
    /// <see cref="MemoryStreamSlim"/> instance containing the compressed data.
    /// </summary>
    /// <param name="source">
    /// The source data to compress.
    /// </param>
    /// <returns>
    /// A new <see cref="MemoryStreamSlim"/> instance containing the compressed data from the 
    /// source array using the specified compression type. If the source array is empty,
    /// an empty <see cref="MemoryStreamSlim"/> instance is returned.
    /// </returns>
    public static MemoryStreamSlim Compress(byte[] source)
    {
        ArgumentNullException.ThrowIfNull(source, nameof(source));
        if (source.Length == 0)
            return MemoryStreamSlim.Create();

        MemoryStreamSlim returnStream = MemoryStreamCompression.CreateCompressionOutput(source);
        Compress(source, returnStream, null);
        return returnStream;
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Compresses the data in the source byte array and returns a new 
    /// <see cref="MemoryStreamSlim"/> instance containing the compressed data.
    /// </summary>
    /// <param name="source">
    /// The source data to compress.
    /// </param>
    /// <param name="compressionOptions">
    /// The options that control the compression operation. This can be used to specify the
    /// compression type, the compression level, and other options. If null,
    /// then default options are used.
    /// </param>
    /// <returns>
    /// A new <see cref="MemoryStreamSlim"/> instance containing the compressed data from the 
    /// source array using the specified compression type. If the source array is empty,
    /// an empty <see cref="MemoryStreamSlim"/> instance is returned.
    /// </returns>
    public static MemoryStreamSlim Compress(byte[] source,
        MemoryStreamDeflateOptions compressionOptions)
    {
        ArgumentNullException.ThrowIfNull(source, nameof(source));
        if (source.Length == 0)
            return MemoryStreamSlim.Create(compressionOptions.NewStreamOptions);

        MemoryStreamSlim returnStream = MemoryStreamCompression.CreateCompressionOutput(source, compressionOptions.NewStreamOptions);
        Compress(source, returnStream, compressionOptions);
        return returnStream;
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Compresses the data in the source byte array and returns a new 
    /// <see cref="MemoryStreamSlim"/> instance containing the compressed data.
    /// </summary>
    /// <param name="source">
    /// The source data to compress.
    /// </param>
    /// <param name="optionsSetup">
    /// Delegate to set up options to configure the compression operation.
    /// </param>
    /// <returns>
    /// A new <see cref="MemoryStreamSlim"/> instance containing the compressed data from the 
    /// source array using the specified compression type. If the source array is empty,
    /// an empty <see cref="MemoryStreamSlim"/> instance is returned.
    /// </returns>
    public static MemoryStreamSlim Compress(byte[] source,
        Func<MemoryStreamDeflateOptions, MemoryStreamDeflateOptions> optionsSetup)
    {
        ArgumentNullException.ThrowIfNull(source, nameof(source));
        ArgumentNullException.ThrowIfNull(optionsSetup, nameof(optionsSetup));
        MemoryStreamDeflateOptions compressionOptions = optionsSetup(new());
        if (source.Length == 0)
            return MemoryStreamSlim.Create(compressionOptions.NewStreamOptions);

        MemoryStreamSlim returnStream = MemoryStreamCompression.CreateCompressionOutput(source, compressionOptions.NewStreamOptions);
        Compress(source, returnStream, compressionOptions);
        return returnStream;
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Compresses the data in the source byte array and returns a new 
    /// <see cref="MemoryStreamSlim"/> instance containing the compressed data.
    /// </summary>
    /// <param name="source">
    /// The source data to compress.
    /// </param>
    /// <param name="optionsSetup">
    /// Delegate to set up options to configure the compression operation.
    /// </param>
    /// <param name="setupState">
    /// State object to pass to the options setup delegate. This can be used to pass
    /// any type of state information to the delegate and avoid the need for a closure.
    /// </param>
    /// <returns>
    /// A new <see cref="MemoryStreamSlim"/> instance containing the compressed data from the 
    /// source array using the specified compression type. If the source array is empty,
    /// an empty <see cref="MemoryStreamSlim"/> instance is returned.
    /// </returns>
    public static MemoryStreamSlim Compress<TState>(byte[] source,
        Func<MemoryStreamDeflateOptions, TState, MemoryStreamDeflateOptions> optionsSetup, TState setupState)
    {
        ArgumentNullException.ThrowIfNull(source, nameof(source));
        ArgumentNullException.ThrowIfNull(optionsSetup, nameof(optionsSetup));
        MemoryStreamDeflateOptions compressionOptions = optionsSetup(new(), setupState);
        if (source.Length == 0)
            return MemoryStreamSlim.Create(compressionOptions.NewStreamOptions);

        MemoryStreamSlim returnStream = MemoryStreamCompression.CreateCompressionOutput(source, compressionOptions.NewStreamOptions);
        Compress(source, returnStream, compressionOptions);
        return returnStream;
    }
    //--------------------------------------------------------------------------------

    //--------------------------------------------------------------------------------
    /// <summary>
    /// Compresses the data in the source span and returns a new 
    /// <see cref="MemoryStreamSlim"/> instance containing the compressed data.
    /// </summary>
    /// <param name="source">
    /// The source data to compress.
    /// </param>
    /// <returns>
    /// A new <see cref="MemoryStreamSlim"/> instance containing the compressed data from the 
    /// source span using the specified compression type. If the source span is empty,
    /// an empty <see cref="MemoryStreamSlim"/> instance is returned.
    /// </returns>
    public static MemoryStreamSlim Compress(in ReadOnlySpan<byte> source)
    {
        if (source.Length == 0)
            return MemoryStreamSlim.Create();

        MemoryStreamSlim returnStream = MemoryStreamCompression.CreateCompressionOutput(source);
        Compress(source, returnStream, null);
        return returnStream;
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Compresses the data in the source span and returns a new 
    /// <see cref="MemoryStreamSlim"/> instance containing the compressed data.
    /// </summary>
    /// <param name="source">
    /// The source data to compress.
    /// </param>
    /// <param name="compressionOptions">
    /// The options that control the compression operation. This can be used to specify the
    /// compression type, the compression level, and other options. If null,
    /// then default options are used.
    /// </param>
    /// <returns>
    /// A new <see cref="MemoryStreamSlim"/> instance containing the compressed data from the 
    /// source span using the specified compression type. If the source span is empty,
    /// an empty <see cref="MemoryStreamSlim"/> instance is returned.
    /// </returns>
    public static MemoryStreamSlim Compress(in ReadOnlySpan<byte> source,
        MemoryStreamDeflateOptions compressionOptions)
    {
        if (source.Length == 0)
            return MemoryStreamSlim.Create(compressionOptions.NewStreamOptions);

        MemoryStreamSlim returnStream = MemoryStreamCompression.CreateCompressionOutput(source, compressionOptions.NewStreamOptions);
        Compress(source, returnStream, compressionOptions);
        return returnStream;
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Compresses the data in the source span and returns a new 
    /// <see cref="MemoryStreamSlim"/> instance containing the compressed data.
    /// </summary>
    /// <param name="source">
    /// The source data to compress.
    /// </param>
    /// <param name="optionsSetup">
    /// Delegate to set up options to configure the compression operation.
    /// </param>
    /// <returns>
    /// A new <see cref="MemoryStreamSlim"/> instance containing the compressed data from the 
    /// source span using the specified compression type. If the source span is empty,
    /// an empty <see cref="MemoryStreamSlim"/> instance is returned.
    /// </returns>
    public static MemoryStreamSlim Compress(in ReadOnlySpan<byte> source,
        Func<MemoryStreamDeflateOptions, MemoryStreamDeflateOptions> optionsSetup)
    {
        ArgumentNullException.ThrowIfNull(optionsSetup, nameof(optionsSetup));
        MemoryStreamDeflateOptions compressionOptions = optionsSetup(new());
        if (source.Length == 0)
            return MemoryStreamSlim.Create(compressionOptions.NewStreamOptions);

        MemoryStreamSlim returnStream = MemoryStreamCompression.CreateCompressionOutput(source, compressionOptions.NewStreamOptions);
        Compress(source, returnStream, compressionOptions);
        return returnStream;
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Compresses the data in the source span and returns a new 
    /// <see cref="MemoryStreamSlim"/> instance containing the compressed data.
    /// </summary>
    /// <param name="source">
    /// The source data to compress.
    /// </param>
    /// <param name="optionsSetup">
    /// Delegate to set up options to configure the compression operation.
    /// </param>
    /// <param name="setupState">
    /// State object to pass to the options setup delegate. This can be used to pass
    /// any type of state information to the delegate and avoid the need for a closure.
    /// </param>
    /// <returns>
    /// A new <see cref="MemoryStreamSlim"/> instance containing the compressed data from the 
    /// span stream using the specified compression type. If the span stream is empty,
    /// an empty <see cref="MemoryStreamSlim"/> instance is returned.
    /// </returns>
    public static MemoryStreamSlim Compress<TState>(in ReadOnlySpan<byte> source,
        Func<MemoryStreamDeflateOptions, TState, MemoryStreamDeflateOptions> optionsSetup, TState setupState)
    {
        ArgumentNullException.ThrowIfNull(optionsSetup, nameof(optionsSetup));
        MemoryStreamDeflateOptions compressionOptions = optionsSetup(new(), setupState);
        if (source.Length == 0)
            return MemoryStreamSlim.Create(compressionOptions.NewStreamOptions);

        MemoryStreamSlim returnStream = MemoryStreamCompression.CreateCompressionOutput(source, compressionOptions.NewStreamOptions);
        Compress(source, returnStream, compressionOptions);
        return returnStream;
    }
    //--------------------------------------------------------------------------------

    //--------------------------------------------------------------------------------
    /// <summary>
    /// Compresses the data in the source string and returns a new 
    /// <see cref="MemoryStreamSlim"/> instance containing the compressed data.
    /// </summary>
    /// <param name="source">
    /// The source data to compress.
    /// </param>
    /// <returns>
    /// A new <see cref="MemoryStreamSlim"/> instance containing the compressed data from the 
    /// span stream using the specified compression type. If the span stream is empty,
    /// an empty <see cref="MemoryStreamSlim"/> instance is returned.
    /// </returns>
    public static MemoryStreamSlim Compress(string source)
    {
        ArgumentNullException.ThrowIfNull(source, nameof(source));
        if (source.Length == 0)
            return MemoryStreamSlim.Create();

        MemoryStreamSlim returnStream = MemoryStreamCompression.CreateCompressionOutput(source);
        Compress(source, returnStream, null);
        return returnStream;
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Compresses the data in the source string and returns a new 
    /// <see cref="MemoryStreamSlim"/> instance containing the compressed data.
    /// </summary>
    /// <param name="source">
    /// The source data to compress.
    /// </param>
    /// <param name="compressionOptions">
    /// The options that control the compression operation. This can be used to specify the
    /// compression type, the compression level, and other options. If null,
    /// then default options are used.
    /// </param>
    /// <returns>
    /// A new <see cref="MemoryStreamSlim"/> instance containing the compressed data from the 
    /// source string using the specified compression type. If the source string is empty,
    /// an empty <see cref="MemoryStreamSlim"/> instance is returned.
    /// </returns>
    public static MemoryStreamSlim Compress(string source,
        MemoryStreamDeflateOptions compressionOptions)
    {
        ArgumentNullException.ThrowIfNull(source, nameof(source));
        if (source.Length == 0)
            return MemoryStreamSlim.Create(compressionOptions.NewStreamOptions);

        MemoryStreamSlim returnStream = MemoryStreamCompression.CreateCompressionOutput(source, compressionOptions.NewStreamOptions);
        Compress(source, returnStream, compressionOptions);
        return returnStream;
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Compresses the data in the source string and returns a new 
    /// <see cref="MemoryStreamSlim"/> instance containing the compressed data.
    /// </summary>
    /// <param name="source">
    /// The source data to compress.
    /// </param>
    /// <param name="optionsSetup">
    /// Delegate to set up options to configure the compression operation.
    /// </param>
    /// <returns>
    /// A new <see cref="MemoryStreamSlim"/> instance containing the compressed data from the 
    /// source string using the specified compression type. If the source string is empty,
    /// an empty <see cref="MemoryStreamSlim"/> instance is returned.
    /// </returns>
    public static MemoryStreamSlim Compress(string source,
        Func<MemoryStreamDeflateOptions, MemoryStreamDeflateOptions> optionsSetup)
    {
        ArgumentNullException.ThrowIfNull(source, nameof(source));
        ArgumentNullException.ThrowIfNull(optionsSetup, nameof(optionsSetup));
        MemoryStreamDeflateOptions compressionOptions = optionsSetup(new());
        if (source.Length == 0)
            return MemoryStreamSlim.Create(compressionOptions.NewStreamOptions);

        MemoryStreamSlim returnStream = MemoryStreamCompression.CreateCompressionOutput(source, compressionOptions.NewStreamOptions);
        Compress(source, returnStream, compressionOptions);
        return returnStream;
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Compresses the data in the source string and returns a new 
    /// <see cref="MemoryStreamSlim"/> instance containing the compressed data.
    /// </summary>
    /// <param name="source">
    /// The source data to compress.
    /// </param>
    /// <param name="optionsSetup">
    /// Delegate to set up options to configure the compression operation.
    /// </param>
    /// <param name="setupState">
    /// State object to pass to the options setup delegate. This can be used to pass
    /// any type of state information to the delegate and avoid the need for a closure.
    /// </param>
    /// <returns>
    /// A new <see cref="MemoryStreamSlim"/> instance containing the compressed data from the 
    /// source string using the specified compression type. If the source string is empty,
    /// an empty <see cref="MemoryStreamSlim"/> instance is returned.
    /// </returns>
    public static MemoryStreamSlim Compress<TState>(string source,
        Func<MemoryStreamDeflateOptions, TState, MemoryStreamDeflateOptions> optionsSetup, TState setupState)
    {
        ArgumentNullException.ThrowIfNull(source, nameof(source));
        ArgumentNullException.ThrowIfNull(optionsSetup, nameof(optionsSetup));
        MemoryStreamDeflateOptions compressionOptions = optionsSetup(new(), setupState);
        if (source.Length == 0)
            return MemoryStreamSlim.Create(compressionOptions.NewStreamOptions);

        MemoryStreamSlim returnStream = MemoryStreamCompression.CreateCompressionOutput(source, compressionOptions.NewStreamOptions);
        Compress(source, returnStream, compressionOptions);
        return returnStream;
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Compresses the data in the source string with the specified encoding and returns a new 
    /// <see cref="MemoryStreamSlim"/> instance containing the compressed data.
    /// </summary>
    /// <param name="source">
    /// The source data to compress.
    /// </param>
    /// <param name="encoding">
    /// The character encoding to use.
    /// </param>
    /// <returns>
    /// A new <see cref="MemoryStreamSlim"/> instance containing the compressed data from the 
    /// source string using the specified compression type. If the source string is empty,
    /// an empty <see cref="MemoryStreamSlim"/> instance is returned.
    /// </returns>
    public static MemoryStreamSlim Compress(string source, Encoding encoding)
    {
        ArgumentNullException.ThrowIfNull(source, nameof(source));
        if (source.Length == 0)
            return MemoryStreamSlim.Create();

        MemoryStreamSlim returnStream = MemoryStreamCompression.CreateCompressionOutput(source, encoding);
        Compress(source, encoding, returnStream, null);
        return returnStream;
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Compresses the data in the source string with the specified encoding and returns a new 
    /// <see cref="MemoryStreamSlim"/> instance containing the compressed data.
    /// </summary>
    /// <param name="source">
    /// The source data to compress.
    /// </param>
    /// <param name="encoding">
    /// The character encoding to use.
    /// </param>
    /// <param name="compressionOptions">
    /// The options that control the compression operation. This can be used to specify the
    /// compression type, the compression level, and other options. If null,
    /// then default options are used.
    /// </param>
    /// <returns>
    /// A new <see cref="MemoryStreamSlim"/> instance containing the compressed data from the 
    /// source string using the specified compression type. If the source string is empty,
    /// an empty <see cref="MemoryStreamSlim"/> instance is returned.
    /// </returns>
    public static MemoryStreamSlim Compress(string source, Encoding encoding,
        MemoryStreamDeflateOptions compressionOptions)
    {
        ArgumentNullException.ThrowIfNull(source, nameof(source));
        if (source.Length == 0)
            return MemoryStreamSlim.Create(compressionOptions.NewStreamOptions);

        MemoryStreamSlim returnStream = MemoryStreamCompression.CreateCompressionOutput(source, encoding, compressionOptions.NewStreamOptions);
        Compress(source, encoding, returnStream, compressionOptions);
        return returnStream;
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Compresses the data in the source string with the specified encoding and returns a new 
    /// <see cref="MemoryStreamSlim"/> instance containing the compressed data.
    /// </summary>
    /// <param name="source">
    /// The source data to compress.
    /// </param>
    /// <param name="encoding">
    /// The character encoding to use.
    /// </param>
    /// <param name="optionsSetup">
    /// Delegate to set up options to configure the compression operation.
    /// </param>
    /// <returns>
    /// A new <see cref="MemoryStreamSlim"/> instance containing the compressed data from the 
    /// source string using the specified compression type. If the source string is empty,
    /// an empty <see cref="MemoryStreamSlim"/> instance is returned.
    /// </returns>
    public static MemoryStreamSlim Compress(string source, Encoding encoding,
        Func<MemoryStreamDeflateOptions, MemoryStreamDeflateOptions> optionsSetup)
    {
        ArgumentNullException.ThrowIfNull(source, nameof(source));
        ArgumentNullException.ThrowIfNull(optionsSetup, nameof(optionsSetup));
        MemoryStreamDeflateOptions compressionOptions = optionsSetup(new());
        if (source.Length == 0)
            return MemoryStreamSlim.Create(compressionOptions.NewStreamOptions);

        MemoryStreamSlim returnStream = MemoryStreamCompression.CreateCompressionOutput(source, encoding, compressionOptions.NewStreamOptions);
        Compress(source, encoding, returnStream, compressionOptions);
        return returnStream;
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Compresses the data in the source string with the specified encoding and returns a new 
    /// <see cref="MemoryStreamSlim"/> instance containing the compressed data.
    /// </summary>
    /// <param name="source">
    /// The source data to compress.
    /// </param>
    /// <param name="encoding">
    /// The character encoding to use.
    /// </param>
    /// <param name="optionsSetup">
    /// Delegate to set up options to configure the compression operation.
    /// </param>
    /// <param name="setupState">
    /// State object to pass to the options setup delegate. This can be used to pass
    /// any type of state information to the delegate and avoid the need for a closure.
    /// </param>
    /// <returns>
    /// A new <see cref="MemoryStreamSlim"/> instance containing the compressed data from the 
    /// source string using the specified compression type. If the source string is empty,
    /// an empty <see cref="MemoryStreamSlim"/> instance is returned.
    /// </returns>
    public static MemoryStreamSlim Compress<TState>(string source, Encoding encoding,
        Func<MemoryStreamDeflateOptions, TState, MemoryStreamDeflateOptions> optionsSetup, TState setupState)
    {
        ArgumentNullException.ThrowIfNull(source, nameof(source));
        ArgumentNullException.ThrowIfNull(optionsSetup, nameof(optionsSetup));
        MemoryStreamDeflateOptions compressionOptions = optionsSetup(new(), setupState);
        if (source.Length == 0)
            return MemoryStreamSlim.Create(compressionOptions.NewStreamOptions);

        MemoryStreamSlim returnStream = MemoryStreamCompression.CreateCompressionOutput(source, encoding, compressionOptions.NewStreamOptions);
        Compress(source, encoding, returnStream, compressionOptions);
        return returnStream;
    }
    //--------------------------------------------------------------------------------

    //--------------------------------------------------------------------------------
    /// <summary>
    /// Compresses the data in the source stream and returns a new 
    /// <see cref="MemoryStreamSlim"/> instance containing the compressed data.
    /// </summary>
    /// <param name="source">
    /// The source stream to compress. This stream must be readable and seekable.
    /// </param>
    /// <returns>
    /// A new <see cref="MemoryStreamSlim"/> instance containing the compressed data from the 
    /// source stream using the specified compression type. If the source stream is empty,
    /// an empty <see cref="MemoryStreamSlim"/> instance is returned.
    /// </returns>
    public static MemoryStreamSlim Compress(Stream source)
    {
        MemoryStreamCompression.ValidateCompression(source);
        if (source.Length == 0)
            return MemoryStreamSlim.Create();

        MemoryStreamSlim returnStream = MemoryStreamCompression.CreateCompressionOutput(source);
        Compress(source, returnStream, null);
        return returnStream;
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Compresses the data in the source stream and returns a new 
    /// <see cref="MemoryStreamSlim"/> instance containing the compressed data.
    /// </summary>
    /// <param name="source">
    /// The source stream to compress. This stream must be readable and seekable.
    /// </param>
    /// <param name="compressionOptions">
    /// The options that control the compression operation. This can be used to specify the
    /// compression type, the compression level, and other options. If null,
    /// then default options are used.
    /// </param>
    /// <returns>
    /// A new <see cref="MemoryStreamSlim"/> instance containing the compressed data from the 
    /// source stream using the specified compression type. If the source stream is empty,
    /// an empty <see cref="MemoryStreamSlim"/> instance is returned.
    /// </returns>
    public static MemoryStreamSlim Compress(Stream source,
        MemoryStreamDeflateOptions compressionOptions)
    {
        MemoryStreamCompression.ValidateCompression(source);
        if (source.Length == 0)
            return MemoryStreamSlim.Create(compressionOptions.NewStreamOptions);

        MemoryStreamSlim returnStream = MemoryStreamCompression.CreateCompressionOutput(source, compressionOptions.NewStreamOptions);
        Compress(source, returnStream, compressionOptions);
        return returnStream;
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Compresses the data in the source stream and returns a new 
    /// <see cref="MemoryStreamSlim"/> instance containing the compressed data.
    /// </summary>
    /// <param name="source">
    /// The source stream to compress. This stream must be readable and seekable.
    /// </param>
    /// <param name="optionsSetup">
    /// Delegate to set up options to configure the compression operation.
    /// </param>
    /// <returns>
    /// A new <see cref="MemoryStreamSlim"/> instance containing the compressed data from the 
    /// source stream using the specified compression type. If the source stream is empty,
    /// an empty <see cref="MemoryStreamSlim"/> instance is returned.
    /// </returns>
    public static MemoryStreamSlim Compress(Stream source,
        Func<MemoryStreamDeflateOptions, MemoryStreamDeflateOptions> optionsSetup)
    {
        ArgumentNullException.ThrowIfNull(optionsSetup, nameof(optionsSetup));
        MemoryStreamCompression.ValidateCompression(source);
        MemoryStreamDeflateOptions compressionOptions = optionsSetup(new());
        if (source.Length == 0)
            return MemoryStreamSlim.Create(compressionOptions.NewStreamOptions);

        MemoryStreamSlim returnStream = MemoryStreamCompression.CreateCompressionOutput(source, compressionOptions.NewStreamOptions);
        Compress(source, returnStream, compressionOptions);
        return returnStream;
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Compresses the data in the source stream and returns a new 
    /// <see cref="MemoryStreamSlim"/> instance containing the compressed data.
    /// </summary>
    /// <param name="source">
    /// The source stream to compress. This stream must be readable and seekable.
    /// </param>
    /// <param name="optionsSetup">
    /// Delegate to set up options to configure the compression operation.
    /// </param>
    /// <param name="setupState">
    /// State object to pass to the options setup delegate. This can be used to pass
    /// any type of state information to the delegate and avoid the need for a closure.
    /// </param>
    /// <returns>
    /// A new <see cref="MemoryStreamSlim"/> instance containing the compressed data from the 
    /// source stream using the specified compression type. If the source stream is empty,
    /// an empty <see cref="MemoryStreamSlim"/> instance is returned.
    /// </returns>
    public static MemoryStreamSlim Compress<TState>(Stream source,
        Func<MemoryStreamDeflateOptions, TState, MemoryStreamDeflateOptions> optionsSetup, TState setupState)
    {
        ArgumentNullException.ThrowIfNull(optionsSetup, nameof(optionsSetup));
        MemoryStreamCompression.ValidateCompression(source);
        MemoryStreamDeflateOptions compressionOptions = optionsSetup(new(), setupState);
        if (source.Length == 0)
            return MemoryStreamSlim.Create(compressionOptions.NewStreamOptions);

        MemoryStreamSlim returnStream = MemoryStreamCompression.CreateCompressionOutput(source, compressionOptions.NewStreamOptions);
        Compress(source, returnStream, compressionOptions);
        return returnStream;
    }
    //--------------------------------------------------------------------------------

    //--------------------------------------------------------------------------------
    /// <summary>
    /// Asynchronously compresses the data in the source stream and returns a new 
    /// <see cref="MemoryStreamSlim"/> instance containing the compressed data.
    /// </summary>
    /// <param name="source">
    /// The source stream to compress. This stream must be readable and seekable.
    /// </param>
    /// <param name="cancellationToken">
    /// The cancellation token to use for the operation.
    /// </param>
    /// <returns>
    /// An awaitable task that completes when the operation is complete and returns a 
    /// new <see cref="MemoryStreamSlim"/> instance containing the compressed data from the
    /// source stream using the specified compression type. If the source stream is empty,
    /// an empty <see cref="MemoryStreamSlim"/> instance is returned.
    /// </returns>
    public static async Task<MemoryStreamSlim> CompressAsync(Stream source, CancellationToken cancellationToken)
    {
        MemoryStreamCompression.ValidateCompression(source);
        if (source.Length == 0)
            return MemoryStreamSlim.Create();

        MemoryStreamSlim returnStream = MemoryStreamCompression.CreateCompressionOutput(source);
        await CompressAsync(source, returnStream, null, cancellationToken).ConfigureAwait(false);
        return returnStream;
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Asynchronously compresses the data in the source stream and returns a new 
    /// <see cref="MemoryStreamSlim"/> instance containing the compressed data.
    /// </summary>
    /// <param name="source">
    /// The source stream to compress. This stream must be readable and seekable.
    /// </param>
    /// <param name="compressionOptions">
    /// The options that control the compression operation. This can be used to specify the
    /// compression type, the compression level, and other options. If null,
    /// then default options are used.
    /// </param>
    /// <param name="cancellationToken">
    /// The cancellation token to use for the operation.
    /// </param>
    /// <returns>
    /// An awaitable task that completes when the operation is complete and returns a 
    /// new <see cref="MemoryStreamSlim"/> instance containing the compressed data from the
    /// source stream using the specified compression type. If the source stream is empty,
    /// an empty <see cref="MemoryStreamSlim"/> instance is returned.
    /// </returns>
    public static async Task<MemoryStreamSlim> CompressAsync(Stream source,
        MemoryStreamDeflateOptions compressionOptions, CancellationToken cancellationToken)
    {
        MemoryStreamCompression.ValidateCompression(source);
        if (source.Length == 0)
            return MemoryStreamSlim.Create(compressionOptions.NewStreamOptions);

        MemoryStreamSlim returnStream = MemoryStreamCompression.CreateCompressionOutput(source, compressionOptions.NewStreamOptions);
        await CompressAsync(source, returnStream, compressionOptions, cancellationToken).ConfigureAwait(false);
        return returnStream;
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Asynchronously compresses the data in the source stream and returns a new 
    /// <see cref="MemoryStreamSlim"/> instance containing the compressed data.
    /// </summary>
    /// <param name="source">
    /// The source stream to compress. This stream must be readable and seekable.
    /// </param>
    /// <param name="optionsSetup">
    /// Delegate to set up options to configure the compression operation.
    /// </param>
    /// <param name="cancellationToken">
    /// The cancellation token to use for the operation.
    /// </param>
    /// <returns>
    /// An awaitable task that completes when the operation is complete and returns a 
    /// new <see cref="MemoryStreamSlim"/> instance containing the compressed data from the
    /// source stream using the specified compression type. If the source stream is empty,
    /// an empty <see cref="MemoryStreamSlim"/> instance is returned.
    /// </returns>
    public static async Task<MemoryStreamSlim> CompressAsync(Stream source,
        Func<MemoryStreamDeflateOptions, MemoryStreamDeflateOptions> optionsSetup, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(optionsSetup, nameof(optionsSetup));
        MemoryStreamCompression.ValidateCompression(source);
        MemoryStreamDeflateOptions compressionOptions = optionsSetup(new());
        if (source.Length == 0)
            return MemoryStreamSlim.Create(compressionOptions.NewStreamOptions);

        MemoryStreamSlim returnStream = MemoryStreamCompression.CreateCompressionOutput(source, compressionOptions.NewStreamOptions);
        await CompressAsync(source, returnStream, compressionOptions, cancellationToken).ConfigureAwait(false);
        return returnStream;
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Asynchronously compresses the data in the source stream and returns a new 
    /// <see cref="MemoryStreamSlim"/> instance containing the compressed data.
    /// </summary>
    /// <param name="source">
    /// The source stream to compress. This stream must be readable and seekable.
    /// </param>
    /// <param name="optionsSetup">
    /// Delegate to set up options to configure the compression operation.
    /// </param>
    /// <param name="setupState">
    /// State object to pass to the options setup delegate. This can be used to pass
    /// any type of state information to the delegate and avoid the need for a closure.
    /// </param>
    /// <param name="cancellationToken">
    /// The cancellation token to use for the operation.
    /// </param>
    /// <returns>
    /// An awaitable task that completes when the operation is complete and returns a 
    /// new <see cref="MemoryStreamSlim"/> instance containing the compressed data from the
    /// source stream using the specified compression type. If the source stream is empty,
    /// an empty <see cref="MemoryStreamSlim"/> instance is returned.
    /// </returns>
    public static async Task<MemoryStreamSlim> CompressAsync<TState>(Stream source,
        Func<MemoryStreamDeflateOptions, TState, MemoryStreamDeflateOptions> optionsSetup, TState setupState,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(optionsSetup, nameof(optionsSetup));
        MemoryStreamCompression.ValidateCompression(source);
        MemoryStreamDeflateOptions compressionOptions = optionsSetup(new(), setupState);
        if (source.Length == 0)
            return MemoryStreamSlim.Create(compressionOptions.NewStreamOptions);

        MemoryStreamSlim returnStream = MemoryStreamCompression.CreateCompressionOutput(source, compressionOptions.NewStreamOptions);
        await CompressAsync(source, returnStream, compressionOptions, cancellationToken).ConfigureAwait(false);
        return returnStream;
    }
    //--------------------------------------------------------------------------------

    #endregion Public Compression Methods

    #region Public Decompression Methods

    //--------------------------------------------------------------------------------
    /// <summary>
    /// Decompresses the data in the source stream and returns a new 
    /// <see cref="MemoryStreamSlim"/> instance containing the decompressed data.
    /// </summary>
    /// <param name="source">
    /// The source stream to decompress. This stream must be readable and seekable.
    /// </param>
    /// <returns>
    /// A new <see cref="MemoryStreamSlim"/> instance containing the compressed data from the 
    /// source stream using the specified decompression type. If the source stream is empty,
    /// an empty <see cref="MemoryStreamSlim"/> instance is returned.
    /// </returns>
    public static MemoryStreamSlim Decompress(Stream source)
    {
        MemoryStreamCompression.ValidateCompression(source);
        if (source.Length == 0)
            return MemoryStreamSlim.Create();

        MemoryStreamSlim returnStream = MemoryStreamCompression.CreateDecompressionOutput(source);
        Decompress(source, returnStream);
        return returnStream;
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Decompresses the data in the source stream and returns a new 
    /// <see cref="MemoryStreamSlim"/> instance containing the decompressed data.
    /// </summary>
    /// <param name="source">
    /// The source stream to decompress. This stream must be readable and seekable.
    /// </param>
    /// <param name="decompressionStreamOptions">
    /// The options used for creating the returned <see cref="MemoryStreamSlim"/>.
    /// </param>
    /// <returns>
    /// A new <see cref="MemoryStreamSlim"/> instance containing the compressed data from the 
    /// source stream using the specified decompression type. If the source stream is empty,
    /// an empty <see cref="MemoryStreamSlim"/> instance is returned.
    /// </returns>
    public static MemoryStreamSlim Decompress(Stream source,
        MemoryStreamSlimOptions decompressionStreamOptions)
    {
        MemoryStreamCompression.ValidateCompression(source);
        if (source.Length == 0)
            return MemoryStreamSlim.Create(decompressionStreamOptions);

        MemoryStreamSlim returnStream = MemoryStreamCompression.CreateDecompressionOutput(source, decompressionStreamOptions);
        Decompress(source, returnStream);
        return returnStream;
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Decompresses the data in the source stream and returns a new 
    /// <see cref="MemoryStreamSlim"/> instance containing the decompressed data.
    /// </summary>
    /// <param name="source">
    /// The source stream to decompress. This stream must be readable and seekable.
    /// </param>
    /// <param name="optionsSetup">
    /// Delegate to set up options to configure the decompression operation.
    /// </param>
    /// <returns>
    /// A new <see cref="MemoryStreamSlim"/> instance containing the compressed data from the 
    /// source stream using the specified decompression type. If the source stream is empty,
    /// an empty <see cref="MemoryStreamSlim"/> instance is returned.
    /// </returns>
    public static MemoryStreamSlim Decompress(Stream source,
        Func<MemoryStreamSlimOptions, MemoryStreamSlimOptions> optionsSetup)
    {
        ArgumentNullException.ThrowIfNull(optionsSetup, nameof(optionsSetup));
        MemoryStreamCompression.ValidateCompression(source);
        MemoryStreamSlimOptions decompressionStreamOptions = optionsSetup(new MemoryStreamSlimOptions());
        if (source.Length == 0)
            return MemoryStreamSlim.Create(decompressionStreamOptions);

        MemoryStreamSlim returnStream = MemoryStreamCompression.CreateDecompressionOutput(source, decompressionStreamOptions);
        Decompress(source, returnStream);
        return returnStream;
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Decompresses the data in the source stream and returns a new 
    /// <see cref="MemoryStreamSlim"/> instance containing the decompressed data.
    /// </summary>
    /// <param name="source">
    /// The source stream to decompress. This stream must be readable and seekable.
    /// </param>
    /// <param name="optionsSetup">
    /// Delegate to set up options to configure the decompression operation.
    /// </param>
    /// <param name="setupState">
    /// State object to pass to the options setup delegate. This can be used to pass
    /// any type of state information to the delegate and avoid the need for a closure.
    /// </param>
    /// <returns>
    /// A new <see cref="MemoryStreamSlim"/> instance containing the compressed data from the 
    /// source stream using the specified decompression type. If the source stream is empty,
    /// an empty <see cref="MemoryStreamSlim"/> instance is returned.
    /// </returns>
    public static MemoryStreamSlim Decompress<TState>(Stream source,
        Func<MemoryStreamSlimOptions, TState, MemoryStreamSlimOptions> optionsSetup, TState setupState)
    {
        ArgumentNullException.ThrowIfNull(optionsSetup, nameof(optionsSetup));
        MemoryStreamCompression.ValidateCompression(source);
        MemoryStreamSlimOptions decompressionStreamOptions = optionsSetup(new MemoryStreamSlimOptions(), setupState);
        if (source.Length == 0)
            return MemoryStreamSlim.Create(decompressionStreamOptions);

        MemoryStreamSlim returnStream = MemoryStreamCompression.CreateDecompressionOutput(source, decompressionStreamOptions);
        Decompress(source, returnStream);
        return returnStream;
    }
    //--------------------------------------------------------------------------------

    //--------------------------------------------------------------------------------
    /// <summary>
    /// Decompresses the data in the source stream and returns a new 
    /// <see cref="string"/> instance containing the decompressed data decoded with UTF-8.
    /// </summary>
    /// <param name="source">
    /// The source stream to decompress. This stream must be readable and seekable.
    /// </param>
    /// <returns>
    /// A new <see cref="MemoryStreamSlim"/> instance containing the compressed data from the 
    /// source stream using the specified decompression type. If the source stream is empty,
    /// an empty <see cref="MemoryStreamSlim"/> instance is returned.
    /// </returns>
    public static string DecompressToString(Stream source) => DecompressToString(source, MemoryStreamCompression.DefaultEncoding);
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Decompresses the data in the source stream and returns a new 
    /// <see cref="string"/> instance containing the decompressed data decoded with UTF-8.
    /// </summary>
    /// <param name="source">
    /// The source stream to decompress. This stream must be readable and seekable.
    /// </param>
    /// <param name="encoding">
    /// The encoding to use to decode the decompressed data.
    /// </param>
    /// <returns>
    /// A new <see cref="MemoryStreamSlim"/> instance containing the compressed data from the 
    /// source stream using the specified decompression type. If the source stream is empty,
    /// an empty <see cref="MemoryStreamSlim"/> instance is returned.
    /// </returns>
    public static string DecompressToString(Stream source, Encoding encoding)
    {
        MemoryStreamSlim decompressedStream = Decompress(source);
        return decompressedStream.Decode(encoding);
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Decompresses the data in the source stream and returns a new 
    /// <see cref="string"/> instance containing the decompressed data decoded with UTF-8.
    /// </summary>
    /// <param name="source">
    /// The source stream to decompress. This stream must be readable and seekable.
    /// </param>
    /// <param name="decompressionStreamOptions">
    /// The options used for creating the returned <see cref="MemoryStreamSlim"/>.
    /// </param>
    /// <returns>
    /// A new <see cref="MemoryStreamSlim"/> instance containing the compressed data from the 
    /// source stream using the specified decompression type. If the source stream is empty,
    /// an empty <see cref="MemoryStreamSlim"/> instance is returned.
    /// </returns>
    public static string DecompressToString(Stream source,
        MemoryStreamSlimOptions decompressionStreamOptions) =>
        DecompressToString(source, MemoryStreamCompression.DefaultEncoding, decompressionStreamOptions);
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Decompresses the data in the source stream and returns a new 
    /// <see cref="string"/> instance containing the decompressed data decoded with UTF-8.
    /// </summary>
    /// <param name="source">
    /// The source stream to decompress. This stream must be readable and seekable.
    /// </param>
    /// <param name="encoding">
    /// The encoding to use to decode the decompressed data.
    /// </param>
    /// <param name="decompressionStreamOptions">
    /// The options that control the decompression operation. This can be used to specify the
    /// decompression type, the decompression level, and other options. If null,
    /// then default options are used.
    /// </param>
    /// <returns>
    /// A new <see cref="MemoryStreamSlim"/> instance containing the compressed data from the 
    /// source stream using the specified decompression type. If the source stream is empty,
    /// an empty <see cref="MemoryStreamSlim"/> instance is returned.
    /// </returns>
    public static string DecompressToString(Stream source, Encoding encoding,
        MemoryStreamSlimOptions decompressionStreamOptions)
    {
        MemoryStreamSlim decompressedStream = Decompress(source, decompressionStreamOptions);
        return decompressedStream.Decode(encoding);
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Decompresses the data in the source stream and returns a new 
    /// <see cref="string"/> instance containing the decompressed data decoded with UTF-8.
    /// </summary>
    /// <param name="source">
    /// The source stream to decompress. This stream must be readable and seekable.
    /// </param>
    /// <param name="optionsSetup">
    /// Delegate to set up options to configure the decompression operation.
    /// </param>
    /// <returns>
    /// A new <see cref="MemoryStreamSlim"/> instance containing the compressed data from the 
    /// source stream using the specified decompression type. If the source stream is empty,
    /// an empty <see cref="MemoryStreamSlim"/> instance is returned.
    /// </returns>
    public static string DecompressToString(Stream source,
        Func<MemoryStreamSlimOptions, MemoryStreamSlimOptions> optionsSetup) =>
        DecompressToString(source, MemoryStreamCompression.DefaultEncoding, optionsSetup);
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Decompresses the data in the source stream and returns a new 
    /// <see cref="string"/> instance containing the decompressed data decoded with UTF-8.
    /// </summary>
    /// <param name="source">
    /// The source stream to decompress. This stream must be readable and seekable.
    /// </param>
    /// <param name="encoding">
    /// The encoding to use to decode the decompressed data.
    /// </param>
    /// <param name="optionsSetup">
    /// Delegate to set up options to configure the decompression operation.
    /// </param>
    /// <returns>
    /// A new <see cref="MemoryStreamSlim"/> instance containing the compressed data from the 
    /// source stream using the specified decompression type. If the source stream is empty,
    /// an empty <see cref="MemoryStreamSlim"/> instance is returned.
    /// </returns>
    public static string DecompressToString(Stream source, Encoding encoding,
        Func<MemoryStreamSlimOptions, MemoryStreamSlimOptions> optionsSetup)
    {
        MemoryStreamSlim decompressedStream = Decompress(source, optionsSetup);
        return decompressedStream.Decode(encoding);
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Decompresses the data in the source stream and returns a new 
    /// <see cref="string"/> instance containing the decompressed data decoded with UTF-8.
    /// </summary>
    /// <param name="source">
    /// The source stream to decompress. This stream must be readable and seekable.
    /// </param>
    /// <param name="optionsSetup">
    /// Delegate to set up options to configure the decompression operation.
    /// </param>
    /// <param name="setupState">
    /// State object to pass to the options setup delegate. This can be used to pass
    /// any type of state information to the delegate and avoid the need for a closure.
    /// </param>
    /// <returns>
    /// A new <see cref="MemoryStreamSlim"/> instance containing the compressed data from the 
    /// source stream using the specified decompression type. If the source stream is empty,
    /// an empty <see cref="MemoryStreamSlim"/> instance is returned.
    /// </returns>
    public static string DecompressToString<TState>(Stream source,
        Func<MemoryStreamSlimOptions, TState, MemoryStreamSlimOptions> optionsSetup, TState setupState) =>
        DecompressToString(source, MemoryStreamCompression.DefaultEncoding, optionsSetup, setupState);
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Decompresses the data in the source stream and returns a new 
    /// <see cref="string"/> instance containing the decompressed data decoded with UTF-8.
    /// </summary>
    /// <param name="source">
    /// The source stream to decompress. This stream must be readable and seekable.
    /// </param>
    /// <param name="encoding">
    /// The encoding to use to decode the decompressed data.
    /// </param>
    /// <param name="optionsSetup">
    /// Delegate to set up options to configure the decompression operation.
    /// </param>
    /// <param name="setupState">
    /// State object to pass to the options setup delegate. This can be used to pass
    /// any type of state information to the delegate and avoid the need for a closure.
    /// </param>
    /// <returns>
    /// A new <see cref="MemoryStreamSlim"/> instance containing the compressed data from the 
    /// source stream using the specified decompression type. If the source stream is empty,
    /// an empty <see cref="MemoryStreamSlim"/> instance is returned.
    /// </returns>
    public static string DecompressToString<TState>(Stream source, Encoding encoding,
        Func<MemoryStreamSlimOptions, TState, MemoryStreamSlimOptions> optionsSetup, TState setupState)
    {
        MemoryStreamSlim decompressedStream = Decompress(source, optionsSetup, setupState);
        return decompressedStream.Decode(encoding);
    }
    //--------------------------------------------------------------------------------

    //--------------------------------------------------------------------------------
    /// <summary>
    /// Asynchronously decompresses the data in the source stream and returns a new 
    /// <see cref="MemoryStreamSlim"/> instance containing the decompressed data.
    /// </summary>
    /// <param name="source">
    /// The source stream to decompress. This stream must be readable and seekable.
    /// </param>
    /// <param name="cancellationToken">
    /// The cancellation token to use for the operation.
    /// </param>
    /// <returns>
    /// An awaitable task that completes when the operation is complete and returns a 
    /// new <see cref="MemoryStreamSlim"/> instance containing the compressed data from the
    /// source stream using the specified decompression type. If the source stream is empty,
    /// an empty <see cref="MemoryStreamSlim"/> instance is returned.
    /// </returns>
    public static async Task<MemoryStreamSlim> DecompressAsync(Stream source, CancellationToken cancellationToken)
    {
        MemoryStreamCompression.ValidateCompression(source);
        if (source.Length == 0)
            return MemoryStreamSlim.Create();

        MemoryStreamSlim returnStream = MemoryStreamCompression.CreateDecompressionOutput(source);
        await DecompressAsync(source, returnStream, cancellationToken).ConfigureAwait(false);
        return returnStream;
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Asynchronously decompresses the data in the source stream and returns a new 
    /// <see cref="MemoryStreamSlim"/> instance containing the decompressed data.
    /// </summary>
    /// <param name="source">
    /// The source stream to decompress. This stream must be readable and seekable.
    /// </param>
    /// <param name="decompressionStreamOptions">
    /// The options used for creating the returned <see cref="MemoryStreamSlim"/>.
    /// </param>
    /// <param name="cancellationToken">
    /// The cancellation token to use for the operation.
    /// </param>
    /// <returns>
    /// An awaitable task that completes when the operation is complete and returns a 
    /// new <see cref="MemoryStreamSlim"/> instance containing the compressed data from the
    /// source stream using the specified decompression type. If the source stream is empty,
    /// an empty <see cref="MemoryStreamSlim"/> instance is returned.
    /// </returns>
    public static async Task<MemoryStreamSlim> DecompressAsync(Stream source,
        MemoryStreamSlimOptions decompressionStreamOptions, CancellationToken cancellationToken)
    {
        MemoryStreamCompression.ValidateCompression(source);
        if (source.Length == 0)
            return MemoryStreamSlim.Create(decompressionStreamOptions);

        MemoryStreamSlim returnStream = MemoryStreamCompression.CreateDecompressionOutput(source, decompressionStreamOptions);
        await DecompressAsync(source, returnStream, cancellationToken).ConfigureAwait(false);
        return returnStream;
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Asynchronously decompresses the data in the source stream and returns a new 
    /// <see cref="MemoryStreamSlim"/> instance containing the decompressed data.
    /// </summary>
    /// <param name="source">
    /// The source stream to decompress. This stream must be readable and seekable.
    /// </param>
    /// <param name="optionsSetup">
    /// Delegate to set up the options for creating the returned <see cref="MemoryStreamSlim"/>.
    /// </param>
    /// <param name="cancellationToken">
    /// The cancellation token to use for the operation.
    /// </param>
    /// <returns>
    /// An awaitable task that completes when the operation is complete and returns a 
    /// new <see cref="MemoryStreamSlim"/> instance containing the compressed data from the
    /// source stream using the specified decompression type. If the source stream is empty,
    /// an empty <see cref="MemoryStreamSlim"/> instance is returned.
    /// </returns>
    public static async Task<MemoryStreamSlim> DecompressAsync(Stream source,
        Func<MemoryStreamSlimOptions, MemoryStreamSlimOptions> optionsSetup, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(optionsSetup, nameof(optionsSetup));
        MemoryStreamCompression.ValidateCompression(source);
        MemoryStreamSlimOptions decompressionStreamOptions = optionsSetup(new MemoryStreamSlimOptions());
        if (source.Length == 0)
            return MemoryStreamSlim.Create(decompressionStreamOptions);

        MemoryStreamSlim returnStream = MemoryStreamCompression.CreateDecompressionOutput(source, decompressionStreamOptions);
        await DecompressAsync(source, returnStream, cancellationToken).ConfigureAwait(false);
        return returnStream;
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Asynchronously decompresses the data in the source stream and returns a new 
    /// <see cref="MemoryStreamSlim"/> instance containing the decompressed data.
    /// </summary>
    /// <param name="source">
    /// The source stream to decompress. This stream must be readable and seekable.
    /// </param>
    /// <param name="optionsSetup">
    /// Delegate to set up the options for creating the returned <see cref="MemoryStreamSlim"/>.
    /// </param>
    /// <param name="setupState">
    /// State object to pass to the options setup delegate. This can be used to pass
    /// any type of state information to the delegate and avoid the need for a closure.
    /// </param>
    /// <param name="cancellationToken">
    /// The cancellation token to use for the operation.
    /// </param>
    /// <returns>
    /// An awaitable task that completes when the operation is complete and returns a 
    /// new <see cref="MemoryStreamSlim"/> instance containing the compressed data from the
    /// source stream using the specified decompression type. If the source stream is empty,
    /// an empty <see cref="MemoryStreamSlim"/> instance is returned.
    /// </returns>
    public static async Task<MemoryStreamSlim> DecompressAsync<TState>(Stream source,
        Func<MemoryStreamSlimOptions, TState, MemoryStreamSlimOptions> optionsSetup, TState setupState,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(optionsSetup, nameof(optionsSetup));
        MemoryStreamCompression.ValidateCompression(source);
        MemoryStreamSlimOptions decompressionStreamOptions = optionsSetup(new MemoryStreamSlimOptions(), setupState);
        if (source.Length == 0)
            return MemoryStreamSlim.Create(decompressionStreamOptions);

        MemoryStreamSlim returnStream = MemoryStreamCompression.CreateDecompressionOutput(source, decompressionStreamOptions);
        await DecompressAsync(source, returnStream, cancellationToken).ConfigureAwait(false);
        return returnStream;
    }
    //--------------------------------------------------------------------------------

    //--------------------------------------------------------------------------------
    /// <summary>
    /// Asynchronously decompresses the data in the source stream and returns a new 
    /// <see cref="string"/> instance containing the decompressed data decoded with UTF-8.
    /// </summary>
    /// <param name="source">
    /// The source stream to decompress. This stream must be readable and seekable.
    /// </param>
    /// <param name="cancellationToken">
    /// The cancellation token to use for the operation.
    /// </param>
    /// <returns>
    /// An awaitable task that completes when the operation is complete and returns a 
    /// new <see cref="MemoryStreamSlim"/> instance containing the compressed data from the
    /// source stream using the specified decompression type. If the source stream is empty,
    /// an empty <see cref="MemoryStreamSlim"/> instance is returned.
    /// </returns>
    public static Task<string> DecompressToStringAsync(Stream source, CancellationToken cancellationToken) =>
        DecompressToStringAsync(source, MemoryStreamCompression.DefaultEncoding, cancellationToken);
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Asynchronously decompresses the data in the source stream and returns a new 
    /// <see cref="string"/> instance containing the decompressed data decoded with UTF-8.
    /// </summary>
    /// <param name="source">
    /// The source stream to decompress. This stream must be readable and seekable.
    /// </param>
    /// <param name="encoding">
    /// The encoding to use to decode the decompressed data.
    /// </param>
    /// <param name="cancellationToken">
    /// The cancellation token to use for the operation.
    /// </param>
    /// <returns>
    /// An awaitable task that completes when the operation is complete and returns a 
    /// new <see cref="MemoryStreamSlim"/> instance containing the compressed data from the
    /// source stream using the specified decompression type. If the source stream is empty,
    /// an empty <see cref="MemoryStreamSlim"/> instance is returned.
    /// </returns>
    public static async Task<string> DecompressToStringAsync(Stream source,
        Encoding encoding, CancellationToken cancellationToken)
    {
        MemoryStreamSlim decompressedStream = await DecompressAsync(source, cancellationToken);
        return decompressedStream.Decode(encoding);
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Asynchronously decompresses the data in the source stream and returns a new 
    /// <see cref="string"/> instance containing the decompressed data decoded with UTF-8.
    /// </summary>
    /// <param name="source">
    /// The source stream to decompress. This stream must be readable and seekable.
    /// </param>
    /// <param name="decompressionStreamOptions">
    /// The options that control the decompression operation. This can be used to specify the
    /// decompression type, the decompression level, and other options. If null,
    /// then default options are used.
    /// </param>
    /// <param name="cancellationToken">
    /// The cancellation token to use for the operation.
    /// </param>
    /// <returns>
    /// An awaitable task that completes when the operation is complete and returns a 
    /// new <see cref="MemoryStreamSlim"/> instance containing the compressed data from the
    /// source stream using the specified decompression type. If the source stream is empty,
    /// an empty <see cref="MemoryStreamSlim"/> instance is returned.
    /// </returns>
    public static Task<string> DecompressToStringAsync(Stream source,
        MemoryStreamSlimOptions decompressionStreamOptions, CancellationToken cancellationToken) =>
        DecompressToStringAsync(source, MemoryStreamCompression.DefaultEncoding, decompressionStreamOptions, cancellationToken);
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Asynchronously decompresses the data in the source stream and returns a new 
    /// <see cref="string"/> instance containing the decompressed data decoded with UTF-8.
    /// </summary>
    /// <param name="source">
    /// The source stream to decompress. This stream must be readable and seekable.
    /// </param>
    /// <param name="encoding">
    /// The encoding to use to decode the decompressed data.
    /// </param>
    /// <param name="decompressionStreamOptions">
    /// The options that control the decompression operation. This can be used to specify the
    /// decompression type, the decompression level, and other options. If null,
    /// then default options are used.
    /// </param>
    /// <param name="cancellationToken">
    /// The cancellation token to use for the operation.
    /// </param>
    /// <returns>
    /// An awaitable task that completes when the operation is complete and returns a 
    /// new <see cref="MemoryStreamSlim"/> instance containing the compressed data from the
    /// source stream using the specified decompression type. If the source stream is empty,
    /// an empty <see cref="MemoryStreamSlim"/> instance is returned.
    /// </returns>
    public static async Task<string> DecompressToStringAsync(Stream source, Encoding encoding,
        MemoryStreamSlimOptions decompressionStreamOptions, CancellationToken cancellationToken)
    {
        MemoryStreamSlim decompressedStream = await DecompressAsync(source, decompressionStreamOptions, cancellationToken);
        return decompressedStream.Decode(encoding);
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Asynchronously decompresses the data in the source stream and returns a new 
    /// <see cref="string"/> instance containing the decompressed data decoded with UTF-8.
    /// </summary>
    /// <param name="source">
    /// The source stream to decompress. This stream must be readable and seekable.
    /// </param>
    /// <param name="optionsSetup">
    /// Delegate to set up the options for creating the returned <see cref="MemoryStreamSlim"/>.
    /// </param>
    /// <param name="cancellationToken">
    /// The cancellation token to use for the operation.
    /// </param>
    /// <returns>
    /// An awaitable task that completes when the operation is complete and returns a 
    /// new <see cref="MemoryStreamSlim"/> instance containing the compressed data from the
    /// source stream using the specified decompression type. If the source stream is empty,
    /// an empty <see cref="MemoryStreamSlim"/> instance is returned.
    /// </returns>
    public static Task<string> DecompressToStringAsync(Stream source,
        Func<MemoryStreamSlimOptions, MemoryStreamSlimOptions> optionsSetup, CancellationToken cancellationToken) =>
        DecompressToStringAsync(source, MemoryStreamCompression.DefaultEncoding, optionsSetup, cancellationToken);
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Asynchronously decompresses the data in the source stream and returns a new 
    /// <see cref="string"/> instance containing the decompressed data decoded with UTF-8.
    /// </summary>
    /// <param name="source">
    /// The source stream to decompress. This stream must be readable and seekable.
    /// </param>
    /// <param name="encoding">
    /// The encoding to use to decode the decompressed data.
    /// </param>
    /// <param name="optionsSetup">
    /// Delegate to set up the options for creating the returned <see cref="MemoryStreamSlim"/>.
    /// </param>
    /// <param name="cancellationToken">
    /// The cancellation token to use for the operation.
    /// </param>
    /// <returns>
    /// An awaitable task that completes when the operation is complete and returns a 
    /// new <see cref="MemoryStreamSlim"/> instance containing the compressed data from the
    /// source stream using the specified decompression type. If the source stream is empty,
    /// an empty <see cref="MemoryStreamSlim"/> instance is returned.
    /// </returns>
    public static async Task<string> DecompressToStringAsync(Stream source, Encoding encoding,
        Func<MemoryStreamSlimOptions, MemoryStreamSlimOptions> optionsSetup, CancellationToken cancellationToken)
    {
        MemoryStreamSlim decompressedStream = await DecompressAsync(source, optionsSetup, cancellationToken);
        return decompressedStream.Decode(encoding);
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Asynchronously decompresses the data in the source stream and returns a new 
    /// <see cref="string"/> instance containing the decompressed data decoded with UTF-8.
    /// </summary>
    /// <param name="source">
    /// The source stream to decompress. This stream must be readable and seekable.
    /// </param>
    /// <param name="optionsSetup">
    /// Delegate to set up the options for creating the returned <see cref="MemoryStreamSlim"/>.
    /// </param>
    /// <param name="setupState">
    /// State object to pass to the options setup delegate. This can be used to pass
    /// any type of state information to the delegate and avoid the need for a closure.
    /// </param>
    /// <param name="cancellationToken">
    /// The cancellation token to use for the operation.
    /// </param>
    /// <returns>
    /// An awaitable task that completes when the operation is complete and returns a 
    /// new <see cref="MemoryStreamSlim"/> instance containing the compressed data from the
    /// source stream using the specified decompression type. If the source stream is empty,
    /// an empty <see cref="MemoryStreamSlim"/> instance is returned.
    /// </returns>
    public static Task<string> DecompressToStringAsync<TState>(Stream source,
        Func<MemoryStreamSlimOptions, TState, MemoryStreamSlimOptions> optionsSetup, TState setupState,
        CancellationToken cancellationToken) =>
        DecompressToStringAsync(source, MemoryStreamCompression.DefaultEncoding, optionsSetup, setupState, cancellationToken);
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Asynchronously decompresses the data in the source stream and returns a new 
    /// <see cref="string"/> instance containing the decompressed data decoded with UTF-8.
    /// </summary>
    /// <param name="source">
    /// The source stream to decompress. This stream must be readable and seekable.
    /// </param>
    /// <param name="encoding">
    /// The encoding to use to decode the decompressed data.
    /// </param>
    /// <param name="optionsSetup">
    /// Delegate to set up the options for creating the returned <see cref="MemoryStreamSlim"/>.
    /// </param>
    /// <param name="setupState">
    /// State object to pass to the options setup delegate. This can be used to pass
    /// any type of state information to the delegate and avoid the need for a closure.
    /// </param>
    /// <param name="cancellationToken">
    /// The cancellation token to use for the operation.
    /// </param>
    /// <returns>
    /// An awaitable task that completes when the operation is complete and returns a 
    /// new <see cref="MemoryStreamSlim"/> instance containing the compressed data from the
    /// source stream using the specified decompression type. If the source stream is empty,
    /// an empty <see cref="MemoryStreamSlim"/> instance is returned.
    /// </returns>
    public static async Task<string> DecompressToStringAsync<TState>(Stream source, Encoding encoding,
        Func<MemoryStreamSlimOptions, TState, MemoryStreamSlimOptions> optionsSetup, TState setupState,
        CancellationToken cancellationToken)
    {
        MemoryStreamSlim decompressedStream = await DecompressAsync(source, optionsSetup, setupState, cancellationToken);
        return decompressedStream.Decode(encoding);
    }
    //--------------------------------------------------------------------------------

    #endregion Public Decompression Methods

}