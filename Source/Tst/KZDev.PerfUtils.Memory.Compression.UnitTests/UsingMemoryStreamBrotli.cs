// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.IO.Compression;
using System.Text;
using FluentAssertions;

using KZDev.PerfUtils.Tests;

namespace KZDev.PerfUtils.Memory.Compression.UnitTests
{
    //################################################################################
    /// <summary>
    /// Unit tests for the <see cref="MemoryStreamBrotli"/> class.
    /// </summary>
    public class UsingMemoryStreamBrotli : UnitTestBase
    {
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Initializes a new instance of the <see cref="UsingMemoryStreamBrotli"/> class.
        /// </summary>
        /// <param name="xUnitTestOutputHelper">
        /// The Xunit test output helper that can be used to output test messages
        /// </param>
        public UsingMemoryStreamBrotli(ITestOutputHelper xUnitTestOutputHelper) : base(xUnitTestOutputHelper)
        {
        }
        //--------------------------------------------------------------------------------

        #region Test Methods

        //================================================================================

        #region Brotli Compression Tests

        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemoryStreamSlim"/> class when utilizing Brotli compression by
        /// compressing a byte array and then decompressing it to verify that the original data is
        /// restored.
        /// </summary>
        [Fact]
        public void UsingMemoryStreamBrotli_ByteArraySmallestSize_DataIsCompressedAndDecompressIsValid()
        {
            byte[] testData = GetRandomBytes(200_000);

            // Compress the data            
            MemoryStreamSlim compressedStream = MemoryStreamBrotli.Compress(testData);

            compressedStream.Position.Should().Be(0);

            // Decompress the data and verify it is the same as the original data
            MemoryStreamSlim decompressedData = MemoryStreamBrotli.Decompress(compressedStream);
            decompressedData.Position.Should().Be(0);
            compressedStream.Position.Should().Be(0);
            decompressedData.ToArray().Should().BeEquivalentTo(testData);
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemoryStreamSlim"/> class when utilizing Brotli compression by
        /// compressing an empty byte array which should be an empty stream result and then 
        /// decompressing it to verify that the returned stream is also empty.
        /// </summary>
        [Fact]
        public void UsingMemoryStreamBrotli_EmptyByteArraySmallestSize_EmptyStreamReturned()
        {
            byte[] testData = [];

            // Compress the data            
            MemoryStreamSlim compressedStream = MemoryStreamBrotli.Compress(testData);

            compressedStream.Length.Should().Be(0);

            // Decompress the data and verify it is the same as the original data
            MemoryStreamSlim decompressedData = MemoryStreamBrotli.Decompress(compressedStream);
            decompressedData.Length.Should().Be(0);
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemoryStreamSlim"/> class when utilizing Brotli compression by
        /// compressing a byte array and then decompressing it to verify that the original data is
        /// restored.
        /// </summary>
        [Fact]
        public void UsingMemoryStreamBrotli_ByteArraySmallestSize_WithOptions_DataIsCompressedAndDecompressIsValid()
        {
            byte[] testData = GetRandomBytes(200_000);

            // Compress the data            
            MemoryStreamSlim compressedStream = MemoryStreamBrotli.Compress(testData,
                new MemoryStreamBrotliOptions().WithCompressionLevel(CompressionLevel.SmallestSize));

            compressedStream.Position.Should().Be(0);

            // Decompress the data and verify it is the same as the original data
            MemoryStreamSlim decompressedData = MemoryStreamBrotli.Decompress(compressedStream,
                new MemoryStreamSlimOptions { ZeroBufferBehavior = MemoryStreamSlimZeroBufferOption.None});
            decompressedData.Position.Should().Be(0);
            compressedStream.Position.Should().Be(0);
            decompressedData.ToArray().Should().BeEquivalentTo(testData);
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemoryStreamSlim"/> class when utilizing Brotli compression by
        /// compressing an empty byte array which should be an empty stream result and then 
        /// decompressing it to verify that the returned stream is also empty.
        /// </summary>
        [Fact]
        public void UsingMemoryStreamBrotli_EmptyByteArraySmallestSize_WithOptions_EmptyStreamReturned()
        {
            byte[] testData = [];

            // Compress the data            
            MemoryStreamSlim compressedStream = MemoryStreamBrotli.Compress(testData,
                new MemoryStreamBrotliOptions().WithCompressionLevel(CompressionLevel.SmallestSize));

            compressedStream.Length.Should().Be(0);

            // Decompress the data and verify it is the same as the original data
            MemoryStreamSlim decompressedData = MemoryStreamBrotli.Decompress(compressedStream,
                new MemoryStreamSlimOptions { ZeroBufferBehavior = MemoryStreamSlimZeroBufferOption.None });
            decompressedData.Length.Should().Be(0);
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemoryStreamSlim"/> class when utilizing Brotli compression by
        /// compressing a byte array and then decompressing it to verify that the original data is
        /// restored.
        /// </summary>
        [Fact]
        public void UsingMemoryStreamBrotli_ByteArraySmallestSize_WithOptionsSetup_DataIsCompressedAndDecompressIsValid()
        {
            byte[] testData = GetRandomBytes(200_000);

            // Compress the data            
            MemoryStreamSlim compressedStream = MemoryStreamBrotli.Compress(testData,
                options => options.WithCompressionLevel(CompressionLevel.SmallestSize));

            compressedStream.Position.Should().Be(0);

            // Decompress the data and verify it is the same as the original data
            MemoryStreamSlim decompressedData = MemoryStreamBrotli.Decompress(compressedStream, 
                options => options.WithZeroBufferBehavior( MemoryStreamSlimZeroBufferOption.None));
            decompressedData.Position.Should().Be(0);
            compressedStream.Position.Should().Be(0);
            decompressedData.ToArray().Should().BeEquivalentTo(testData);
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemoryStreamSlim"/> class when utilizing Brotli compression by
        /// compressing an empty byte array which should be an empty stream result and then 
        /// decompressing it to verify that the returned stream is also empty.
        /// </summary>
        [Fact]
        public void UsingMemoryStreamBrotli_EmptyByteArraySmallestSize_WithOptionsSetup_EmptyStreamReturned()
        {
            byte[] testData = [];

            // Compress the data            
            MemoryStreamSlim compressedStream = MemoryStreamBrotli.Compress(testData,
                options => options.WithCompressionLevel(CompressionLevel.SmallestSize));

            compressedStream.Length.Should().Be(0);

            // Decompress the data and verify it is the same as the original data
            MemoryStreamSlim decompressedData = MemoryStreamBrotli.Decompress(compressedStream,
                options => options.WithZeroBufferBehavior(MemoryStreamSlimZeroBufferOption.None));
            decompressedData.Length.Should().Be(0);
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemoryStreamSlim"/> class when utilizing Brotli compression by
        /// compressing a byte array and then decompressing it to verify that the original data is
        /// restored.
        /// </summary>
        [Fact]
        public void UsingMemoryStreamBrotli_ByteArraySmallestSize_WithOptionsStateSetup_DataIsCompressedAndDecompressIsValid()
        {
            byte[] testData = GetRandomBytes(200_000);

            // Compress the data            
            MemoryStreamSlim compressedStream = MemoryStreamBrotli.Compress(testData,
                (options, state) => options.WithCompressionLevel(state), CompressionLevel.SmallestSize);

            compressedStream.Position.Should().Be(0);

            // Decompress the data and verify it is the same as the original data
            MemoryStreamSlim decompressedData = MemoryStreamBrotli.Decompress(compressedStream);
            decompressedData.Position.Should().Be(0);
            compressedStream.Position.Should().Be(0);
            decompressedData.ToArray().Should().BeEquivalentTo(testData);
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemoryStreamSlim"/> class when utilizing Brotli compression by
        /// compressing an empty byte array which should be an empty stream result and then 
        /// decompressing it to verify that the returned stream is also empty.
        /// </summary>
        [Fact]
        public void UsingMemoryStreamBrotli_EmptyByteArraySmallestSize_WithOptionsStateSetup_EmptyStreamReturned()
        {
            byte[] testData = [];

            // Compress the data            
            MemoryStreamSlim compressedStream = MemoryStreamBrotli.Compress(testData,
                (options, state) => options.WithCompressionLevel(state), CompressionLevel.SmallestSize);

            compressedStream.Length.Should().Be(0);

            // Decompress the data and verify it is the same as the original data
            MemoryStreamSlim decompressedData = MemoryStreamBrotli.Decompress(compressedStream,
                (options, state) => options.WithZeroBufferBehavior(state), MemoryStreamSlimZeroBufferOption.None);
            decompressedData.Length.Should().Be(0);
        }
        //--------------------------------------------------------------------------------    

        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemoryStreamSlim"/> class when utilizing Brotli compression by
        /// compressing a byte Stream and then decompressing it to verify that the original data is
        /// restored.
        /// </summary>
        [Fact]
        public void UsingMemoryStreamBrotli_SimpleStream_DataIsCompressedAndDecompressIsValid()
        {
            MemoryStream testData = new(GetRandomBytes(200_000));

            // Compress the data            
            MemoryStreamSlim compressedStream = MemoryStreamBrotli.Compress(testData);

            compressedStream.Position.Should().Be(0);

            // Decompress the data and verify it is the same as the original data
            MemoryStreamSlim decompressedData = MemoryStreamBrotli.Decompress(compressedStream);
            decompressedData.Position.Should().Be(0);
            compressedStream.Position.Should().Be(0);
            decompressedData.ToArray().Should().BeEquivalentTo(testData.ToArray());
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemoryStreamSlim"/> class when utilizing Brotli compression by
        /// compressing an empty byte stream which should be an empty stream result and then 
        /// decompressing it to verify that the returned stream is also empty.
        /// </summary>
        [Fact]
        public void UsingMemoryStreamBrotli_EmptySimpleStream_EmptyStreamReturned()
        {
            MemoryStream testData = new();

            // Compress the data            
            MemoryStreamSlim compressedStream = MemoryStreamBrotli.Compress(testData);

            compressedStream.Length.Should().Be(0);

            // Decompress the data and verify it is the same as the original data
            MemoryStreamSlim decompressedData = MemoryStreamBrotli.Decompress(compressedStream);
            decompressedData.Length.Should().Be(0);
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemoryStreamSlim"/> class when utilizing Brotli compression by
        /// compressing a byte Stream and then decompressing it to verify that the original data is
        /// restored.
        /// </summary>
        [Fact]
        public void UsingMemoryStreamBrotli_SimpleStream_WithOptions_DataIsCompressedAndDecompressIsValid()
        {
            MemoryStream testData = new(GetRandomBytes(200_000));

            // Compress the data            
            MemoryStreamSlim compressedStream = MemoryStreamBrotli.Compress(testData,
                new MemoryStreamBrotliOptions().WithCompressionLevel(CompressionLevel.SmallestSize));

            compressedStream.Position.Should().Be(0);

            // Decompress the data and verify it is the same as the original data
            MemoryStreamSlim decompressedData = MemoryStreamBrotli.Decompress(compressedStream,
                new MemoryStreamSlimOptions { ZeroBufferBehavior = MemoryStreamSlimZeroBufferOption.None });
            decompressedData.Position.Should().Be(0);
            compressedStream.Position.Should().Be(0);
            decompressedData.ToArray().Should().BeEquivalentTo(testData.ToArray());
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemoryStreamSlim"/> class when utilizing Brotli compression by
        /// compressing an empty byte stream which should be an empty stream result and then 
        /// decompressing it to verify that the returned stream is also empty.
        /// </summary>
        [Fact]
        public void UsingMemoryStreamBrotli_EmptySimpleStream_WithOptions_EmptyStreamReturned()
        {
            MemoryStream testData = new();

            // Compress the data            
            MemoryStreamSlim compressedStream = MemoryStreamBrotli.Compress(testData,
                new MemoryStreamBrotliOptions().WithCompressionLevel(CompressionLevel.SmallestSize));

            compressedStream.Length.Should().Be(0);

            // Decompress the data and verify it is the same as the original data
            MemoryStreamSlim decompressedData = MemoryStreamBrotli.Decompress(compressedStream,
                new MemoryStreamSlimOptions { ZeroBufferBehavior = MemoryStreamSlimZeroBufferOption.None });
            decompressedData.Length.Should().Be(0);
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemoryStreamSlim"/> class when utilizing Brotli compression by
        /// compressing a byte Stream and then decompressing it to verify that the original data is
        /// restored.
        /// </summary>
        [Fact]
        public void UsingMemoryStreamBrotli_SimpleStream_WithOptionsSetup_DataIsCompressedAndDecompressIsValid()
        {
            MemoryStream testData = new(GetRandomBytes(200_000));

            // Compress the data            
            MemoryStreamSlim compressedStream = MemoryStreamBrotli.Compress(testData,
                options => options.WithCompressionLevel(CompressionLevel.SmallestSize));

            compressedStream.Position.Should().Be(0);

            // Decompress the data and verify it is the same as the original data
            MemoryStreamSlim decompressedData = MemoryStreamBrotli.Decompress(compressedStream,
                options => options.WithZeroBufferBehavior(MemoryStreamSlimZeroBufferOption.None));
            decompressedData.Position.Should().Be(0);
            compressedStream.Position.Should().Be(0);
            decompressedData.ToArray().Should().BeEquivalentTo(testData.ToArray());
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemoryStreamSlim"/> class when utilizing Brotli compression by
        /// compressing an empty byte stream which should be an empty stream result and then 
        /// decompressing it to verify that the returned stream is also empty.
        /// </summary>
        [Fact]
        public void UsingMemoryStreamBrotli_EmptySimpleStream_WithOptionsSetup_EmptyStreamReturned()
        {
            MemoryStream testData = new();

            // Compress the data            
            MemoryStreamSlim compressedStream = MemoryStreamBrotli.Compress(testData,
                options => options.WithCompressionLevel(CompressionLevel.SmallestSize));

            compressedStream.Length.Should().Be(0);

            // Decompress the data and verify it is the same as the original data
            MemoryStreamSlim decompressedData = MemoryStreamBrotli.Decompress(compressedStream,
                options => options.WithZeroBufferBehavior(MemoryStreamSlimZeroBufferOption.None));
            decompressedData.Length.Should().Be(0);
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemoryStreamSlim"/> class when utilizing Brotli compression by
        /// compressing a byte Stream and then decompressing it to verify that the original data is
        /// restored.
        /// </summary>
        [Fact]
        public void UsingMemoryStreamBrotli_SimpleStream_WithOptionsStateSetup_DataIsCompressedAndDecompressIsValid()
        {
            MemoryStream testData = new(GetRandomBytes(200_000));

            // Compress the data            
            MemoryStreamSlim compressedStream = MemoryStreamBrotli.Compress(testData,
                (options, state) => options.WithCompressionLevel(state), CompressionLevel.SmallestSize);

            compressedStream.Position.Should().Be(0);

            // Decompress the data and verify it is the same as the original data
            MemoryStreamSlim decompressedData = MemoryStreamBrotli.Decompress(compressedStream,
                (options, state) => options.WithZeroBufferBehavior(state), MemoryStreamSlimZeroBufferOption.None);
            decompressedData.Position.Should().Be(0);
            compressedStream.Position.Should().Be(0);
            decompressedData.ToArray().Should().BeEquivalentTo(testData.ToArray());
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemoryStreamSlim"/> class when utilizing Brotli compression by
        /// compressing an empty byte stream which should be an empty stream result and then 
        /// decompressing it to verify that the returned stream is also empty.
        /// </summary>
        [Fact]
        public void UsingMemoryStreamBrotli_EmptySimpleStream_WithOptionsStateSetup_EmptyStreamReturned()
        {
            MemoryStream testData = new();

            // Compress the data            
            MemoryStreamSlim compressedStream = MemoryStreamBrotli.Compress(testData,
                (options, state) => options.WithCompressionLevel(state), CompressionLevel.SmallestSize);

            compressedStream.Length.Should().Be(0);

            // Decompress the data and verify it is the same as the original data
            MemoryStreamSlim decompressedData = MemoryStreamBrotli.Decompress(compressedStream,
                (options, state) => options.WithZeroBufferBehavior(state), MemoryStreamSlimZeroBufferOption.None);
            decompressedData.Length.Should().Be(0);
        }
        //--------------------------------------------------------------------------------    

        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemoryStreamSlim"/> class when utilizing Brotli compression by
        /// compressing a byte Stream and then decompressing it to verify that the original data is
        /// restored.
        /// </summary>
        [Fact]
        public async Task UsingMemoryStreamBrotli_SimpleStream_DataIsCompressedAndDecompressIsValidAsync()
        {
            MemoryStream testData = new(GetRandomBytes(200_000));

            // Compress the data            
            MemoryStreamSlim compressedStream = await MemoryStreamBrotli.CompressAsync(testData, CancellationToken.None);

            compressedStream.Position.Should().Be(0);

            // Decompress the data and verify it is the same as the original data
            MemoryStreamSlim decompressedData = await MemoryStreamBrotli.DecompressAsync(compressedStream, CancellationToken.None);
            decompressedData.Position.Should().Be(0);
            compressedStream.Position.Should().Be(0);
            decompressedData.ToArray().Should().BeEquivalentTo(testData.ToArray());
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemoryStreamSlim"/> class when utilizing Brotli compression by
        /// compressing an empty byte stream which should be an empty stream result and then 
        /// decompressing it to verify that the returned stream is also empty.
        /// </summary>
        [Fact]
        public async Task UsingMemoryStreamBrotli_EmptySimpleStream_EmptyStreamReturnedAsync()
        {
            MemoryStream testData = new();

            // Compress the data            
            MemoryStreamSlim compressedStream = await MemoryStreamBrotli.CompressAsync(testData, CancellationToken.None);

            compressedStream.Length.Should().Be(0);

            // Decompress the data and verify it is the same as the original data
            MemoryStreamSlim decompressedData = await MemoryStreamBrotli.DecompressAsync(compressedStream, CancellationToken.None);
            decompressedData.Length.Should().Be(0);
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemoryStreamSlim"/> class when utilizing Brotli compression by
        /// compressing a byte Stream and then decompressing it to verify that the original data is
        /// restored.
        /// </summary>
        [Fact]
        public async Task UsingMemoryStreamBrotli_SimpleStream_WithOptions_DataIsCompressedAndDecompressIsValidAsync()
        {
            MemoryStream testData = new(GetRandomBytes(200_000));

            // Compress the data            
            MemoryStreamSlim compressedStream = await MemoryStreamBrotli.CompressAsync(testData,
                new MemoryStreamBrotliOptions().WithCompressionLevel(CompressionLevel.SmallestSize), CancellationToken.None);

            compressedStream.Position.Should().Be(0);

            // Decompress the data and verify it is the same as the original data
            MemoryStreamSlim decompressedData = await MemoryStreamBrotli.DecompressAsync(compressedStream,
                new MemoryStreamSlimOptions { ZeroBufferBehavior = MemoryStreamSlimZeroBufferOption.None }, CancellationToken.None);
            decompressedData.Position.Should().Be(0);
            compressedStream.Position.Should().Be(0);
            decompressedData.ToArray().Should().BeEquivalentTo(testData.ToArray());
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemoryStreamSlim"/> class when utilizing Brotli compression by
        /// compressing an empty byte stream which should be an empty stream result and then 
        /// decompressing it to verify that the returned stream is also empty.
        /// </summary>
        [Fact]
        public async Task UsingMemoryStreamBrotli_EmptySimpleStream_WithOptions_EmptyStreamReturnedAsync()
        {
            MemoryStream testData = new();

            // Compress the data            
            MemoryStreamSlim compressedStream = await MemoryStreamBrotli.CompressAsync(testData,
                new MemoryStreamBrotliOptions().WithCompressionLevel(CompressionLevel.SmallestSize), CancellationToken.None);

            compressedStream.Length.Should().Be(0);

            // Decompress the data and verify it is the same as the original data
            MemoryStreamSlim decompressedData = await MemoryStreamBrotli.DecompressAsync(compressedStream,
                new MemoryStreamSlimOptions { ZeroBufferBehavior = MemoryStreamSlimZeroBufferOption.None }, CancellationToken.None);
            decompressedData.Length.Should().Be(0);
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemoryStreamSlim"/> class when utilizing Brotli compression by
        /// compressing a byte Stream and then decompressing it to verify that the original data is
        /// restored.
        /// </summary>
        [Fact]
        public async Task UsingMemoryStreamBrotli_SimpleStream_WithOptionsSetup_DataIsCompressedAndDecompressIsValidAsync()
        {
            MemoryStream testData = new(GetRandomBytes(200_000));

            // Compress the data            
            MemoryStreamSlim compressedStream = await MemoryStreamBrotli.CompressAsync(testData,
                options => options.WithCompressionLevel(CompressionLevel.SmallestSize), CancellationToken.None);

            compressedStream.Position.Should().Be(0);

            // Decompress the data and verify it is the same as the original data
            MemoryStreamSlim decompressedData = await MemoryStreamBrotli.DecompressAsync(compressedStream,
                options => options.WithZeroBufferBehavior(MemoryStreamSlimZeroBufferOption.None), CancellationToken.None);
            decompressedData.Position.Should().Be(0);
            compressedStream.Position.Should().Be(0);
            decompressedData.ToArray().Should().BeEquivalentTo(testData.ToArray());
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemoryStreamSlim"/> class when utilizing Brotli compression by
        /// compressing an empty byte stream which should be an empty stream result and then 
        /// decompressing it to verify that the returned stream is also empty.
        /// </summary>
        [Fact]
        public async Task UsingMemoryStreamBrotli_EmptySimpleStream_WithOptionsSetup_EmptyStreamReturnedAsync()
        {
            MemoryStream testData = new();

            // Compress the data            
            MemoryStreamSlim compressedStream = await MemoryStreamBrotli.CompressAsync(testData,
                options => options.WithCompressionLevel(CompressionLevel.SmallestSize), CancellationToken.None);

            compressedStream.Length.Should().Be(0);

            // Decompress the data and verify it is the same as the original data
            MemoryStreamSlim decompressedData = await MemoryStreamBrotli.DecompressAsync(compressedStream,
                options => options.WithZeroBufferBehavior(MemoryStreamSlimZeroBufferOption.None), CancellationToken.None);
            decompressedData.Length.Should().Be(0);
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemoryStreamSlim"/> class when utilizing Brotli compression by
        /// compressing a byte Stream and then decompressing it to verify that the original data is
        /// restored.
        /// </summary>
        [Fact]
        public async Task UsingMemoryStreamBrotli_SimpleStream_WithOptionsStateSetup_DataIsCompressedAndDecompressIsValidAsync()
        {
            MemoryStream testData = new(GetRandomBytes(200_000));

            // Compress the data            
            MemoryStreamSlim compressedStream = await MemoryStreamBrotli.CompressAsync(testData,
                (options, state) => options.WithCompressionLevel(state), CompressionLevel.SmallestSize, CancellationToken.None);

            compressedStream.Position.Should().Be(0);

            // Decompress the data and verify it is the same as the original data
            MemoryStreamSlim decompressedData = await MemoryStreamBrotli.DecompressAsync(compressedStream,
                (options, state) => options.WithZeroBufferBehavior(state), MemoryStreamSlimZeroBufferOption.None, CancellationToken.None);
            decompressedData.Position.Should().Be(0);
            compressedStream.Position.Should().Be(0);
            decompressedData.ToArray().Should().BeEquivalentTo(testData.ToArray());
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemoryStreamSlim"/> class when utilizing Brotli compression by
        /// compressing an empty byte stream which should be an empty stream result and then 
        /// decompressing it to verify that the returned stream is also empty.
        /// </summary>
        [Fact]
        public async Task UsingMemoryStreamBrotli_EmptySimpleStream_WithOptionsStateSetup_EmptyStreamReturnedAsync()
        {
            MemoryStream testData = new();

            // Compress the data            
            MemoryStreamSlim compressedStream = await MemoryStreamBrotli.CompressAsync(testData,
                (options, state) => options.WithCompressionLevel(state), CompressionLevel.SmallestSize, CancellationToken.None);

            compressedStream.Length.Should().Be(0);

            // Decompress the data and verify it is the same as the original data
            MemoryStreamSlim decompressedData = await MemoryStreamBrotli.DecompressAsync(compressedStream,
                (options, state) => options.WithZeroBufferBehavior(state), MemoryStreamSlimZeroBufferOption.None, CancellationToken.None);
            decompressedData.Length.Should().Be(0);
        }
        //--------------------------------------------------------------------------------    

        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemoryStreamSlim"/> class when utilizing Brotli compression by
        /// compressing a byte ReadOnlySpan and then decompressing it to verify that the original data is
        /// restored.
        /// </summary>
        [Fact]
        public void UsingMemoryStreamBrotli_SimpleReadOnlySpan_DataIsCompressedAndDecompressIsValid()
        {
            ReadOnlySpan<byte> testData = GetRandomBytes(200_000);

            // Compress the data            
            MemoryStreamSlim compressedStream = MemoryStreamBrotli.Compress(testData);

            compressedStream.Position.Should().Be(0);

            // Decompress the data and verify it is the same as the original data
            MemoryStreamSlim decompressedData = MemoryStreamBrotli.Decompress(compressedStream);
            decompressedData.Position.Should().Be(0);
            compressedStream.Position.Should().Be(0);
            decompressedData.ToArray().Should().BeEquivalentTo(testData.ToArray());
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemoryStreamSlim"/> class when utilizing Brotli compression by
        /// compressing an empty byte ReadOnlySpan which should be an empty stream result and then 
        /// decompressing it to verify that the returned stream is also empty.
        /// </summary>
        [Fact]
        public void UsingMemoryStreamBrotli_EmptyReadOnlySpan_EmptyStreamReturned()
        {
            ReadOnlySpan<byte> testData = [];

            // Compress the data            
            MemoryStreamSlim compressedStream = MemoryStreamBrotli.Compress(testData);

            compressedStream.Length.Should().Be(0);

            // Decompress the data and verify it is the same as the original data
            MemoryStreamSlim decompressedData = MemoryStreamBrotli.Decompress(compressedStream);
            decompressedData.Length.Should().Be(0);
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemoryStreamSlim"/> class when utilizing Brotli compression by
        /// compressing a byte ReadOnlySpan and then decompressing it to verify that the original data is
        /// restored.
        /// </summary>
        [Fact]
        public void UsingMemoryStreamBrotli_SimpleReadOnlySpan_WithOptions_DataIsCompressedAndDecompressIsValid()
        {
            ReadOnlySpan<byte> testData = GetRandomBytes(200_000);

            // Compress the data            
            MemoryStreamSlim compressedStream = MemoryStreamBrotli.Compress(testData,
                new MemoryStreamBrotliOptions().WithCompressionLevel(CompressionLevel.SmallestSize));

            compressedStream.Position.Should().Be(0);

            // Decompress the data and verify it is the same as the original data
            MemoryStreamSlim decompressedData = MemoryStreamBrotli.Decompress(compressedStream,
                new MemoryStreamSlimOptions { ZeroBufferBehavior = MemoryStreamSlimZeroBufferOption.None });
            decompressedData.Position.Should().Be(0);
            compressedStream.Position.Should().Be(0);
            decompressedData.ToArray().Should().BeEquivalentTo(testData.ToArray());
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemoryStreamSlim"/> class when utilizing Brotli compression by
        /// compressing an empty byte ReadOnlySpan which should be an empty stream result and then 
        /// decompressing it to verify that the returned stream is also empty.
        /// </summary>
        [Fact]
        public void UsingMemoryStreamBrotli_EmptyReadOnlySpan_WithOptions_EmptyStreamReturned()
        {
            ReadOnlySpan<byte> testData = [];

            // Compress the data            
            MemoryStreamSlim compressedStream = MemoryStreamBrotli.Compress(testData,
                new MemoryStreamBrotliOptions().WithCompressionLevel(CompressionLevel.SmallestSize));

            compressedStream.Length.Should().Be(0);

            // Decompress the data and verify it is the same as the original data
            MemoryStreamSlim decompressedData = MemoryStreamBrotli.Decompress(compressedStream,
                new MemoryStreamSlimOptions { ZeroBufferBehavior = MemoryStreamSlimZeroBufferOption.None });
            decompressedData.Length.Should().Be(0);
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemoryStreamSlim"/> class when utilizing Brotli compression by
        /// compressing a byte ReadOnlySpan and then decompressing it to verify that the original data is
        /// restored.
        /// </summary>
        [Fact]
        public void UsingMemoryStreamBrotli_SimpleReadOnlySpan_WithOptionsSetup_DataIsCompressedAndDecompressIsValid()
        {
            ReadOnlySpan<byte> testData = GetRandomBytes(200_000);

            // Compress the data            
            MemoryStreamSlim compressedStream = MemoryStreamBrotli.Compress(testData,
                options => options.WithCompressionLevel(CompressionLevel.SmallestSize));

            compressedStream.Position.Should().Be(0);

            // Decompress the data and verify it is the same as the original data
            MemoryStreamSlim decompressedData = MemoryStreamBrotli.Decompress(compressedStream,
                options => options.WithZeroBufferBehavior(MemoryStreamSlimZeroBufferOption.None));
            decompressedData.Position.Should().Be(0);
            compressedStream.Position.Should().Be(0);
            decompressedData.ToArray().Should().BeEquivalentTo(testData.ToArray());
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemoryStreamSlim"/> class when utilizing Brotli compression by
        /// compressing an empty byte ReadOnlySpan which should be an empty stream result and then 
        /// decompressing it to verify that the returned stream is also empty.
        /// </summary>
        [Fact]
        public void UsingMemoryStreamBrotli_EmptyReadOnlySpan_WithOptionsSetup_EmptyStreamReturned()
        {
            ReadOnlySpan<byte> testData = [];

            // Compress the data            
            MemoryStreamSlim compressedStream = MemoryStreamBrotli.Compress(testData,
                options => options.WithCompressionLevel(CompressionLevel.SmallestSize));

            compressedStream.Length.Should().Be(0);

            // Decompress the data and verify it is the same as the original data
            MemoryStreamSlim decompressedData = MemoryStreamBrotli.Decompress(compressedStream,
                options => options.WithZeroBufferBehavior(MemoryStreamSlimZeroBufferOption.None));
            decompressedData.Length.Should().Be(0);
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemoryStreamSlim"/> class when utilizing Brotli compression by
        /// compressing a byte ReadOnlySpan and then decompressing it to verify that the original data is
        /// restored.
        /// </summary>
        [Fact]
        public void UsingMemoryStreamBrotli_SimpleReadOnlySpan_WithOptionsStateSetup_DataIsCompressedAndDecompressIsValid()
        {
            ReadOnlySpan<byte> testData = GetRandomBytes(200_000);

            // Compress the data            
            MemoryStreamSlim compressedStream = MemoryStreamBrotli.Compress(testData,
                (options, state) => options.WithCompressionLevel(state), CompressionLevel.SmallestSize);

            compressedStream.Position.Should().Be(0);

            // Decompress the data and verify it is the same as the original data
            MemoryStreamSlim decompressedData = MemoryStreamBrotli.Decompress(compressedStream,
                (options, state) => options.WithZeroBufferBehavior(state), MemoryStreamSlimZeroBufferOption.None);
            decompressedData.Position.Should().Be(0);
            compressedStream.Position.Should().Be(0);
            decompressedData.ToArray().Should().BeEquivalentTo(testData.ToArray());
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemoryStreamSlim"/> class when utilizing Brotli compression by
        /// compressing an empty byte ReadOnlySpan which should be an empty stream result and then 
        /// decompressing it to verify that the returned stream is also empty.
        /// </summary>
        [Fact]
        public void UsingMemoryStreamBrotli_EmptyReadOnlySpan_WithOptionsStateSetup_EmptyStreamReturned()
        {
            ReadOnlySpan<byte> testData = [];

            // Compress the data            
            MemoryStreamSlim compressedStream = MemoryStreamBrotli.Compress(testData,
                (options, state) => options.WithCompressionLevel(state), CompressionLevel.SmallestSize);

            compressedStream.Length.Should().Be(0);

            // Decompress the data and verify it is the same as the original data
            MemoryStreamSlim decompressedData = MemoryStreamBrotli.Decompress(compressedStream,
                (options, state) => options.WithZeroBufferBehavior(state), MemoryStreamSlimZeroBufferOption.None);
            decompressedData.Length.Should().Be(0);
        }
        //--------------------------------------------------------------------------------    

        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemoryStreamSlim"/> class when utilizing Brotli compression by
        /// compressing a string and then decompressing it to verify that the original data is
        /// restored.
        /// </summary>
        [Fact]
        public void UsingMemoryStreamBrotli_SimpleString_DataIsCompressedAndDecompressIsValid()
        {
            string testData = GetRandomString(1_000, 2_000);

            // Compress the data            
            MemoryStreamSlim compressedStream = MemoryStreamBrotli.Compress(testData);

            compressedStream.Position.Should().Be(0);

            // Decompress the data and verify it is the same as the original data
            string decompressedString = MemoryStreamBrotli.DecompressToString(compressedStream);
            compressedStream.Position.Should().Be(0);
            decompressedString.Should().Be(testData);
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemoryStreamSlim"/> class when utilizing Brotli compression by
        /// compressing an empty string which should be an empty stream result and then 
        /// decompressing it to verify that the returned stream is also empty.
        /// </summary>
        [Fact]
        public void UsingMemoryStreamBrotli_EmptyString_EmptyStreamReturned()
        {
            string testData = string.Empty;

            // Compress the data            
            MemoryStreamSlim compressedStream = MemoryStreamBrotli.Compress(testData);

            compressedStream.Length.Should().Be(0);

            // Decompress the data and verify it is the same as the original data
            string decompressedString = MemoryStreamBrotli.DecompressToString(compressedStream);
            decompressedString.Should().BeEmpty();
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemoryStreamSlim"/> class when utilizing Brotli compression by
        /// compressing a string and then decompressing it to verify that the original data is
        /// restored.
        /// </summary>
        [Fact]
        public void UsingMemoryStreamBrotli_SimpleString_WithOptions_DataIsCompressedAndDecompressIsValid()
        {
            string testData = GetRandomString(1_000, 2_000);

            // Compress the data            
            MemoryStreamSlim compressedStream = MemoryStreamBrotli.Compress(testData,
                new MemoryStreamBrotliOptions().WithCompressionLevel(CompressionLevel.SmallestSize));

            compressedStream.Position.Should().Be(0);

            // Decompress the data and verify it is the same as the original data
            string decompressedString = MemoryStreamBrotli.DecompressToString(compressedStream,
                new MemoryStreamSlimOptions { ZeroBufferBehavior = MemoryStreamSlimZeroBufferOption.None });
            compressedStream.Position.Should().Be(0);
            decompressedString.Should().Be(testData);
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemoryStreamSlim"/> class when utilizing Brotli compression by
        /// compressing an empty string which should be an empty stream result and then 
        /// decompressing it to verify that the returned stream is also empty.
        /// </summary>
        [Fact]
        public void UsingMemoryStreamBrotli_EmptyString_WithOptions_EmptyStreamReturned()
        {
            string testData = string.Empty;

            // Compress the data            
            MemoryStreamSlim compressedStream = MemoryStreamBrotli.Compress(testData,
                new MemoryStreamBrotliOptions().WithCompressionLevel(CompressionLevel.SmallestSize));

            compressedStream.Length.Should().Be(0);

            // Decompress the data and verify it is the same as the original data
            string decompressedString = MemoryStreamBrotli.DecompressToString(compressedStream,
                new MemoryStreamSlimOptions { ZeroBufferBehavior = MemoryStreamSlimZeroBufferOption.None });
            decompressedString.Should().BeEmpty();
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemoryStreamSlim"/> class when utilizing Brotli compression by
        /// compressing a string and then decompressing it to verify that the original data is
        /// restored.
        /// </summary>
        [Fact]
        public void UsingMemoryStreamBrotli_SimpleString_WithOptionsSetup_DataIsCompressedAndDecompressIsValid()
        {
            string testData = GetRandomString(1_000, 2_000);

            // Compress the data            
            MemoryStreamSlim compressedStream = MemoryStreamBrotli.Compress(testData,
                options => options.WithCompressionLevel(CompressionLevel.SmallestSize));

            compressedStream.Position.Should().Be(0);

            // Decompress the data and verify it is the same as the original data
            string decompressedString = MemoryStreamBrotli.DecompressToString(compressedStream,
                options => options.WithZeroBufferBehavior(MemoryStreamSlimZeroBufferOption.None));
            compressedStream.Position.Should().Be(0);
            decompressedString.Should().Be(testData);
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemoryStreamSlim"/> class when utilizing Brotli compression by
        /// compressing an empty string which should be an empty stream result and then 
        /// decompressing it to verify that the returned stream is also empty.
        /// </summary>
        [Fact]
        public void UsingMemoryStreamBrotli_EmptyString_WithOptionsSetup_EmptyStreamReturned()
        {
            string testData = string.Empty;

            // Compress the data            
            MemoryStreamSlim compressedStream = MemoryStreamBrotli.Compress(testData,
                options => options.WithCompressionLevel(CompressionLevel.SmallestSize));

            compressedStream.Length.Should().Be(0);

            // Decompress the data and verify it is the same as the original data
            string decompressedString = MemoryStreamBrotli.DecompressToString(compressedStream,
                options => options.WithZeroBufferBehavior(MemoryStreamSlimZeroBufferOption.None));
            decompressedString.Should().BeEmpty();
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemoryStreamSlim"/> class when utilizing Brotli compression by
        /// compressing a string and then decompressing it to verify that the original data is
        /// restored.
        /// </summary>
        [Fact]
        public void UsingMemoryStreamBrotli_SimpleString_WithOptionsStateSetup_DataIsCompressedAndDecompressIsValid()
        {
            string testData = GetRandomString(1_000, 2_000);

            // Compress the data            
            MemoryStreamSlim compressedStream = MemoryStreamBrotli.Compress(testData,
                (options, state) => options.WithCompressionLevel(state), CompressionLevel.SmallestSize);

            compressedStream.Position.Should().Be(0);

            // Decompress the data and verify it is the same as the original data
            string decompressedString = MemoryStreamBrotli.DecompressToString(compressedStream,
                (options, state) => options.WithZeroBufferBehavior(state), MemoryStreamSlimZeroBufferOption.None);
            compressedStream.Position.Should().Be(0);
            decompressedString.Should().Be(testData);
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemoryStreamSlim"/> class when utilizing Brotli compression by
        /// compressing an empty string which should be an empty stream result and then 
        /// decompressing it to verify that the returned stream is also empty.
        /// </summary>
        [Fact]
        public void UsingMemoryStreamBrotli_EmptyString_WithOptionsStateSetup_EmptyStreamReturned()
        {
            string testData = string.Empty;

            // Compress the data            
            MemoryStreamSlim compressedStream = MemoryStreamBrotli.Compress(testData,
                (options, state) => options.WithCompressionLevel(state), CompressionLevel.SmallestSize);

            compressedStream.Length.Should().Be(0);

            // Decompress the data and verify it is the same as the original data
            string decompressedString = MemoryStreamBrotli.DecompressToString(compressedStream,
                (options, state) => options.WithZeroBufferBehavior(state), MemoryStreamSlimZeroBufferOption.None);
            decompressedString.Should().BeEmpty();
        }
        //--------------------------------------------------------------------------------    

        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemoryStreamSlim"/> class when utilizing Brotli compression by
        /// compressing a string and then decompressing it to verify that the original data is
        /// restored.
        /// </summary>
        [Fact]
        public async Task UsingMemoryStreamBrotli_SimpleString_DataIsCompressedAndDecompressIsValidAsync()
        {
            string testData = GetRandomString(1_000, 2_000);

            // Compress the data            
            MemoryStreamSlim compressedStream = MemoryStreamBrotli.Compress(testData);

            compressedStream.Position.Should().Be(0);

            // Decompress the data and verify it is the same as the original data
            string decompressedString = await MemoryStreamBrotli.DecompressToStringAsync(compressedStream, CancellationToken.None);
            compressedStream.Position.Should().Be(0);
            decompressedString.Should().Be(testData);
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemoryStreamSlim"/> class when utilizing Brotli compression by
        /// compressing an empty string which should be an empty stream result and then 
        /// decompressing it to verify that the returned stream is also empty.
        /// </summary>
        [Fact]
        public async Task UsingMemoryStreamBrotli_EmptyString_EmptyStreamReturnedAsync()
        {
            string testData = string.Empty;

            // Compress the data            
            MemoryStreamSlim compressedStream = MemoryStreamBrotli.Compress(testData);

            compressedStream.Length.Should().Be(0);

            // Decompress the data and verify it is the same as the original data
            string decompressedString = await MemoryStreamBrotli.DecompressToStringAsync(compressedStream, CancellationToken.None);
            decompressedString.Should().BeEmpty();
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemoryStreamSlim"/> class when utilizing Brotli compression by
        /// compressing a string and then decompressing it to verify that the original data is
        /// restored.
        /// </summary>
        [Fact]
        public async Task UsingMemoryStreamBrotli_SimpleString_WithOptions_DataIsCompressedAndDecompressIsValidAsync()
        {
            string testData = GetRandomString(1_000, 2_000);

            // Compress the data            
            MemoryStreamSlim compressedStream = MemoryStreamBrotli.Compress(testData,
                new MemoryStreamBrotliOptions().WithCompressionLevel(CompressionLevel.SmallestSize));

            compressedStream.Position.Should().Be(0);

            // Decompress the data and verify it is the same as the original data
            string decompressedString = await MemoryStreamBrotli.DecompressToStringAsync(compressedStream,
                new MemoryStreamSlimOptions { ZeroBufferBehavior = MemoryStreamSlimZeroBufferOption.None }, CancellationToken.None);
            compressedStream.Position.Should().Be(0);
            decompressedString.Should().Be(testData);
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemoryStreamSlim"/> class when utilizing Brotli compression by
        /// compressing an empty string which should be an empty stream result and then 
        /// decompressing it to verify that the returned stream is also empty.
        /// </summary>
        [Fact]
        public async Task UsingMemoryStreamBrotli_EmptyString_WithOptions_EmptyStreamReturnedAsync()
        {
            string testData = string.Empty;

            // Compress the data            
            MemoryStreamSlim compressedStream = MemoryStreamBrotli.Compress(testData,
                new MemoryStreamBrotliOptions().WithCompressionLevel(CompressionLevel.SmallestSize));

            compressedStream.Length.Should().Be(0);

            // Decompress the data and verify it is the same as the original data
            string decompressedString = await MemoryStreamBrotli.DecompressToStringAsync(compressedStream,
                new MemoryStreamSlimOptions { ZeroBufferBehavior = MemoryStreamSlimZeroBufferOption.None }, CancellationToken.None);
            decompressedString.Should().BeEmpty();
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemoryStreamSlim"/> class when utilizing Brotli compression by
        /// compressing a string and then decompressing it to verify that the original data is
        /// restored.
        /// </summary>
        [Fact]
        public async Task UsingMemoryStreamBrotli_SimpleString_WithOptionsSetup_DataIsCompressedAndDecompressIsValidAsync()
        {
            string testData = GetRandomString(1_000, 2_000);

            // Compress the data            
            MemoryStreamSlim compressedStream = MemoryStreamBrotli.Compress(testData,
                options => options.WithCompressionLevel(CompressionLevel.SmallestSize));

            compressedStream.Position.Should().Be(0);

            // Decompress the data and verify it is the same as the original data
            string decompressedString = await MemoryStreamBrotli.DecompressToStringAsync(compressedStream,
                options => options.WithZeroBufferBehavior(MemoryStreamSlimZeroBufferOption.None), CancellationToken.None);
            compressedStream.Position.Should().Be(0);
            decompressedString.Should().Be(testData);
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemoryStreamSlim"/> class when utilizing Brotli compression by
        /// compressing an empty string which should be an empty stream result and then 
        /// decompressing it to verify that the returned stream is also empty.
        /// </summary>
        [Fact]
        public async Task UsingMemoryStreamBrotli_EmptyString_WithOptionsSetup_EmptyStreamReturnedAsync()
        {
            string testData = string.Empty;

            // Compress the data            
            MemoryStreamSlim compressedStream = MemoryStreamBrotli.Compress(testData,
                options => options.WithCompressionLevel(CompressionLevel.SmallestSize));

            compressedStream.Length.Should().Be(0);

            // Decompress the data and verify it is the same as the original data
            string decompressedString = await MemoryStreamBrotli.DecompressToStringAsync(compressedStream,
                options => options.WithZeroBufferBehavior(MemoryStreamSlimZeroBufferOption.None), CancellationToken.None);
            decompressedString.Should().BeEmpty();
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemoryStreamSlim"/> class when utilizing Brotli compression by
        /// compressing a string and then decompressing it to verify that the original data is
        /// restored.
        /// </summary>
        [Fact]
        public async Task UsingMemoryStreamBrotli_SimpleString_WithOptionsStateSetup_DataIsCompressedAndDecompressIsValidAsync()
        {
            string testData = GetRandomString(1_000, 2_000);

            // Compress the data            
            MemoryStreamSlim compressedStream = MemoryStreamBrotli.Compress(testData,
                (options, state) => options.WithCompressionLevel(state), CompressionLevel.SmallestSize);

            compressedStream.Position.Should().Be(0);

            // Decompress the data and verify it is the same as the original data
            string decompressedString = await MemoryStreamBrotli.DecompressToStringAsync(compressedStream,
                (options, state) => options.WithZeroBufferBehavior(state), MemoryStreamSlimZeroBufferOption.None, CancellationToken.None);
            compressedStream.Position.Should().Be(0);
            decompressedString.Should().Be(testData);
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemoryStreamSlim"/> class when utilizing Brotli compression by
        /// compressing an empty string which should be an empty stream result and then 
        /// decompressing it to verify that the returned stream is also empty.
        /// </summary>
        [Fact]
        public async Task UsingMemoryStreamBrotli_EmptyString_WithOptionsStateSetup_EmptyStreamReturnedAsync()
        {
            string testData = string.Empty;

            // Compress the data            
            MemoryStreamSlim compressedStream = MemoryStreamBrotli.Compress(testData,
                (options, state) => options.WithCompressionLevel(state), CompressionLevel.SmallestSize);

            compressedStream.Length.Should().Be(0);

            // Decompress the data and verify it is the same as the original data
            string decompressedString = await MemoryStreamBrotli.DecompressToStringAsync(compressedStream,
                (options, state) => options.WithZeroBufferBehavior(state), MemoryStreamSlimZeroBufferOption.None, CancellationToken.None);
            decompressedString.Should().BeEmpty();
        }
        //--------------------------------------------------------------------------------    

        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemoryStreamSlim"/> class when utilizing Brotli compression by
        /// compressing a string and a specified encoding then decompressing it to verify 
        /// that the original data is restored.
        /// </summary>
        [Fact]
        public void UsingMemoryStreamBrotli_SimpleStringWithEncoding_DataIsCompressedAndDecompressIsValid()
        {
            string testData = GetRandomString(1_000, 2_000);
            Encoding encoding = BitConverter.IsLittleEndian ? Encoding.BigEndianUnicode : Encoding.Unicode;

            // Compress the data            
            MemoryStreamSlim compressedStream = MemoryStreamBrotli.Compress(testData, encoding);

            compressedStream.Position.Should().Be(0);

            // Decompress the data and verify it is the same as the original data
            string decompressedString = MemoryStreamBrotli.DecompressToString(compressedStream, encoding);
            compressedStream.Position.Should().Be(0);
            decompressedString.Should().Be(testData);
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemoryStreamSlim"/> class when utilizing Brotli compression by
        /// compressing an empty string and a specified encoding then decompressing it to verify 
        /// that the original data is restored.
        /// </summary>
        [Fact]
        public void UsingMemoryStreamBrotli_EmptyStringWithEncoding_EmptyStreamReturned()
        {
            string testData = string.Empty;
            Encoding encoding = BitConverter.IsLittleEndian ? Encoding.BigEndianUnicode : Encoding.Unicode;

            // Compress the data            
            MemoryStreamSlim compressedStream = MemoryStreamBrotli.Compress(testData, encoding);

            compressedStream.Length.Should().Be(0);

            // Decompress the data and verify it is the same as the original data
            string decompressedString = MemoryStreamBrotli.DecompressToString(compressedStream, encoding);
            decompressedString.Should().BeEmpty();
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemoryStreamSlim"/> class when utilizing Brotli compression by
        /// compressing a string and a specified encoding then decompressing it to verify 
        /// that the original data is restored.
        /// </summary>
        [Fact]
        public void UsingMemoryStreamBrotli_SimpleStringWithEncoding_WithOptions_DataIsCompressedAndDecompressIsValid()
        {
            string testData = GetRandomString(1_000, 2_000);
            Encoding encoding = BitConverter.IsLittleEndian ? Encoding.BigEndianUnicode : Encoding.Unicode;

            // Compress the data            
            MemoryStreamSlim compressedStream = MemoryStreamBrotli.Compress(testData, encoding,
                new MemoryStreamBrotliOptions().WithCompressionLevel(CompressionLevel.SmallestSize));

            compressedStream.Position.Should().Be(0);

            // Decompress the data and verify it is the same as the original data
            string decompressedString = MemoryStreamBrotli.DecompressToString(compressedStream, encoding,
                new MemoryStreamSlimOptions { ZeroBufferBehavior = MemoryStreamSlimZeroBufferOption.None });
            compressedStream.Position.Should().Be(0);
            decompressedString.Should().Be(testData);
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemoryStreamSlim"/> class when utilizing Brotli compression by
        /// compressing an empty string and a specified encoding then decompressing it to verify 
        /// that the original data is restored.
        /// </summary>
        [Fact]
        public void UsingMemoryStreamBrotli_EmptyStringWithEncoding_WithOptions_EmptyStreamReturned()
        {
            string testData = string.Empty;
            Encoding encoding = BitConverter.IsLittleEndian ? Encoding.BigEndianUnicode : Encoding.Unicode;

            // Compress the data            
            MemoryStreamSlim compressedStream = MemoryStreamBrotli.Compress(testData, encoding,
                new MemoryStreamBrotliOptions().WithCompressionLevel(CompressionLevel.SmallestSize));

            compressedStream.Length.Should().Be(0);

            // Decompress the data and verify it is the same as the original data
            string decompressedString = MemoryStreamBrotli.DecompressToString(compressedStream, encoding,
                new MemoryStreamSlimOptions { ZeroBufferBehavior = MemoryStreamSlimZeroBufferOption.None });
            decompressedString.Should().BeEmpty();
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemoryStreamSlim"/> class when utilizing Brotli compression by
        /// compressing a string and a specified encoding then decompressing it to verify 
        /// that the original data is restored.
        /// </summary>
        [Fact]
        public void UsingMemoryStreamBrotli_SimpleStringWithEncoding_WithOptionsSetup_DataIsCompressedAndDecompressIsValid()
        {
            string testData = GetRandomString(1_000, 2_000);
            Encoding encoding = BitConverter.IsLittleEndian ? Encoding.BigEndianUnicode : Encoding.Unicode;

            // Compress the data            
            MemoryStreamSlim compressedStream = MemoryStreamBrotli.Compress(testData, encoding,
                options => options.WithCompressionLevel(CompressionLevel.SmallestSize));

            compressedStream.Position.Should().Be(0);

            // Decompress the data and verify it is the same as the original data
            string decompressedString = MemoryStreamBrotli.DecompressToString(compressedStream, encoding,
                options => options.WithZeroBufferBehavior(MemoryStreamSlimZeroBufferOption.None));
            compressedStream.Position.Should().Be(0);
            decompressedString.Should().Be(testData);
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemoryStreamSlim"/> class when utilizing Brotli compression by
        /// compressing an empty string and a specified encoding then decompressing it to verify 
        /// that the original data is restored.
        /// </summary>
        [Fact]
        public void UsingMemoryStreamBrotli_EmptyStringWithEncoding_WithOptionsSetup_EmptyStreamReturned()
        {
            string testData = string.Empty;
            Encoding encoding = BitConverter.IsLittleEndian ? Encoding.BigEndianUnicode : Encoding.Unicode;

            // Compress the data            
            MemoryStreamSlim compressedStream = MemoryStreamBrotli.Compress(testData, encoding,
                options => options.WithCompressionLevel(CompressionLevel.SmallestSize));

            compressedStream.Length.Should().Be(0);

            // Decompress the data and verify it is the same as the original data
            string decompressedString = MemoryStreamBrotli.DecompressToString(compressedStream, encoding,
                options => options.WithZeroBufferBehavior(MemoryStreamSlimZeroBufferOption.None));
            decompressedString.Should().BeEmpty();
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemoryStreamSlim"/> class when utilizing Brotli compression by
        /// compressing a string and a specified encoding then decompressing it to verify 
        /// that the original data is restored.
        /// </summary>
        [Fact]
        public void UsingMemoryStreamBrotli_SimpleStringWithEncoding_WithOptionsStateSetup_DataIsCompressedAndDecompressIsValid()
        {
            string testData = GetRandomString(1_000, 2_000);
            Encoding encoding = BitConverter.IsLittleEndian ? Encoding.BigEndianUnicode : Encoding.Unicode;

            // Compress the data            
            MemoryStreamSlim compressedStream = MemoryStreamBrotli.Compress(testData, encoding,
                (options, state) => options.WithCompressionLevel(state), CompressionLevel.SmallestSize);

            compressedStream.Position.Should().Be(0);

            // Decompress the data and verify it is the same as the original data
            string decompressedString = MemoryStreamBrotli.DecompressToString(compressedStream, encoding,
                (options, state) => options.WithZeroBufferBehavior(state), MemoryStreamSlimZeroBufferOption.None);
            compressedStream.Position.Should().Be(0);
            decompressedString.Should().Be(testData);
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemoryStreamSlim"/> class when utilizing Brotli compression by
        /// compressing an empty string and a specified encoding then decompressing it to verify 
        /// that the original data is restored.
        /// </summary>
        [Fact]
        public void UsingMemoryStreamBrotli_EmptyStringWithEncoding_WithOptionsStateSetup_EmptyStreamReturned()
        {
            string testData = string.Empty;
            Encoding encoding = BitConverter.IsLittleEndian ? Encoding.BigEndianUnicode : Encoding.Unicode;

            // Compress the data            
            MemoryStreamSlim compressedStream = MemoryStreamBrotli.Compress(testData, encoding,
                (options, state) => options.WithCompressionLevel(state), CompressionLevel.SmallestSize);

            compressedStream.Length.Should().Be(0);

            // Decompress the data and verify it is the same as the original data
            string decompressedString = MemoryStreamBrotli.DecompressToString(compressedStream, encoding,
                (options, state) => options.WithZeroBufferBehavior(state), MemoryStreamSlimZeroBufferOption.None);
            decompressedString.Should().BeEmpty();
        }
        //--------------------------------------------------------------------------------    

        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemoryStreamSlim"/> class when utilizing Brotli compression by
        /// compressing a string and a specified encoding then decompressing it to verify 
        /// that the original data is restored.
        /// </summary>
        [Fact]
        public async Task UsingMemoryStreamBrotli_SimpleStringWithEncoding_DataIsCompressedAndDecompressIsValidAsync()
        {
            string testData = GetRandomString(1_000, 2_000);
            Encoding encoding = BitConverter.IsLittleEndian ? Encoding.BigEndianUnicode : Encoding.Unicode;

            // Compress the data            
            MemoryStreamSlim compressedStream = MemoryStreamBrotli.Compress(testData, encoding);

            compressedStream.Position.Should().Be(0);

            // Decompress the data and verify it is the same as the original data
            string decompressedString = await MemoryStreamBrotli.DecompressToStringAsync(compressedStream, encoding,
                CancellationToken.None);
            compressedStream.Position.Should().Be(0);
            decompressedString.Should().Be(testData);
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemoryStreamSlim"/> class when utilizing Brotli compression by
        /// compressing an empty string and a specified encoding then decompressing it to verify 
        /// that the original data is restored.
        /// </summary>
        [Fact]
        public async Task UsingMemoryStreamBrotli_EmptyStringWithEncoding_EmptyStreamReturnedAsync()
        {
            string testData = string.Empty;
            Encoding encoding = BitConverter.IsLittleEndian ? Encoding.BigEndianUnicode : Encoding.Unicode;

            // Compress the data            
            MemoryStreamSlim compressedStream = MemoryStreamBrotli.Compress(testData, encoding);

            compressedStream.Length.Should().Be(0);

            // Decompress the data and verify it is the same as the original data
            string decompressedString = await MemoryStreamBrotli.DecompressToStringAsync(compressedStream, encoding,
                CancellationToken.None);
            decompressedString.Should().BeEmpty();
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemoryStreamSlim"/> class when utilizing Brotli compression by
        /// compressing a string and a specified encoding then decompressing it to verify 
        /// that the original data is restored.
        /// </summary>
        [Fact]
        public async Task UsingMemoryStreamBrotli_SimpleStringWithEncoding_WithOptions_DataIsCompressedAndDecompressIsValidAsync()
        {
            string testData = GetRandomString(1_000, 2_000);
            Encoding encoding = BitConverter.IsLittleEndian ? Encoding.BigEndianUnicode : Encoding.Unicode;

            // Compress the data            
            MemoryStreamSlim compressedStream = MemoryStreamBrotli.Compress(testData, encoding,
                new MemoryStreamBrotliOptions().WithCompressionLevel(CompressionLevel.SmallestSize));

            compressedStream.Position.Should().Be(0);

            // Decompress the data and verify it is the same as the original data
            string decompressedString = await MemoryStreamBrotli.DecompressToStringAsync(compressedStream, encoding,
                new MemoryStreamSlimOptions { ZeroBufferBehavior = MemoryStreamSlimZeroBufferOption.None },
                CancellationToken.None);
            compressedStream.Position.Should().Be(0);
            decompressedString.Should().Be(testData);
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemoryStreamSlim"/> class when utilizing Brotli compression by
        /// compressing an empty string and a specified encoding then decompressing it to verify 
        /// that the original data is restored.
        /// </summary>
        [Fact]
        public async Task UsingMemoryStreamBrotli_EmptyStringWithEncoding_WithOptions_EmptyStreamReturnedAsync()
        {
            string testData = string.Empty;
            Encoding encoding = BitConverter.IsLittleEndian ? Encoding.BigEndianUnicode : Encoding.Unicode;

            // Compress the data            
            MemoryStreamSlim compressedStream = MemoryStreamBrotli.Compress(testData, encoding,
                new MemoryStreamBrotliOptions().WithCompressionLevel(CompressionLevel.SmallestSize));

            compressedStream.Length.Should().Be(0);

            // Decompress the data and verify it is the same as the original data
            string decompressedString = await MemoryStreamBrotli.DecompressToStringAsync(compressedStream, encoding,
                new MemoryStreamSlimOptions { ZeroBufferBehavior = MemoryStreamSlimZeroBufferOption.None },
                CancellationToken.None);
            decompressedString.Should().BeEmpty();
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemoryStreamSlim"/> class when utilizing Brotli compression by
        /// compressing a string and a specified encoding then decompressing it to verify 
        /// that the original data is restored.
        /// </summary>
        [Fact]
        public async Task UsingMemoryStreamBrotli_SimpleStringWithEncoding_WithOptionsSetup_DataIsCompressedAndDecompressIsValidAsync()
        {
            string testData = GetRandomString(1_000, 2_000);
            Encoding encoding = BitConverter.IsLittleEndian ? Encoding.BigEndianUnicode : Encoding.Unicode;

            // Compress the data            
            MemoryStreamSlim compressedStream = MemoryStreamBrotli.Compress(testData, encoding,
                options => options.WithCompressionLevel(CompressionLevel.SmallestSize));

            compressedStream.Position.Should().Be(0);

            // Decompress the data and verify it is the same as the original data
            string decompressedString = await MemoryStreamBrotli.DecompressToStringAsync(compressedStream, encoding,
                options => options.WithZeroBufferBehavior(MemoryStreamSlimZeroBufferOption.None),
                CancellationToken.None);
            compressedStream.Position.Should().Be(0);
            decompressedString.Should().Be(testData);
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemoryStreamSlim"/> class when utilizing Brotli compression by
        /// compressing an empty string and a specified encoding then decompressing it to verify 
        /// that the original data is restored.
        /// </summary>
        [Fact]
        public async Task UsingMemoryStreamBrotli_EmptyStringWithEncoding_WithOptionsSetup_EmptyStreamReturnedAsync()
        {
            string testData = string.Empty;
            Encoding encoding = BitConverter.IsLittleEndian ? Encoding.BigEndianUnicode : Encoding.Unicode;

            // Compress the data            
            MemoryStreamSlim compressedStream = MemoryStreamBrotli.Compress(testData, encoding,
                options => options.WithCompressionLevel(CompressionLevel.SmallestSize));

            compressedStream.Length.Should().Be(0);

            // Decompress the data and verify it is the same as the original data
            string decompressedString = await MemoryStreamBrotli.DecompressToStringAsync(compressedStream, encoding,
                options => options.WithZeroBufferBehavior(MemoryStreamSlimZeroBufferOption.None),
                CancellationToken.None);
            decompressedString.Should().BeEmpty();
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemoryStreamSlim"/> class when utilizing Brotli compression by
        /// compressing a string and a specified encoding then decompressing it to verify 
        /// that the original data is restored.
        /// </summary>
        [Fact]
        public async Task UsingMemoryStreamBrotli_SimpleStringWithEncoding_WithOptionsStateSetup_DataIsCompressedAndDecompressIsValidAsync()
        {
            string testData = GetRandomString(1_000, 2_000);
            Encoding encoding = BitConverter.IsLittleEndian ? Encoding.BigEndianUnicode : Encoding.Unicode;

            // Compress the data            
            MemoryStreamSlim compressedStream = MemoryStreamBrotli.Compress(testData, encoding,
                (options, state) => options.WithCompressionLevel(state), CompressionLevel.SmallestSize);

            compressedStream.Position.Should().Be(0);

            // Decompress the data and verify it is the same as the original data
            string decompressedString = await MemoryStreamBrotli.DecompressToStringAsync(compressedStream, encoding,
                (options, state) => options.WithZeroBufferBehavior(state), MemoryStreamSlimZeroBufferOption.None,
                CancellationToken.None);
            compressedStream.Position.Should().Be(0);
            decompressedString.Should().Be(testData);
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemoryStreamSlim"/> class when utilizing Brotli compression by
        /// compressing an empty string and a specified encoding then decompressing it to verify 
        /// that the original data is restored.
        /// </summary>
        [Fact]
        public async Task UsingMemoryStreamBrotli_EmptyStringWithEncoding_WithOptionsStateSetup_EmptyStreamReturnedAsync()
        {
            string testData = string.Empty;
            Encoding encoding = BitConverter.IsLittleEndian ? Encoding.BigEndianUnicode : Encoding.Unicode;

            // Compress the data            
            MemoryStreamSlim compressedStream = MemoryStreamBrotli.Compress(testData, encoding,
                (options, state) => options.WithCompressionLevel(state), CompressionLevel.SmallestSize);

            compressedStream.Length.Should().Be(0);

            // Decompress the data and verify it is the same as the original data
            string decompressedString = await MemoryStreamBrotli.DecompressToStringAsync(compressedStream, encoding,
                (options, state) => options.WithZeroBufferBehavior(state), MemoryStreamSlimZeroBufferOption.None,
                CancellationToken.None);
            decompressedString.Should().BeEmpty();
        }
        //--------------------------------------------------------------------------------    

        #endregion Brotli Compression Tests

        //================================================================================

        #endregion Test Methods
    }
    //################################################################################
}
