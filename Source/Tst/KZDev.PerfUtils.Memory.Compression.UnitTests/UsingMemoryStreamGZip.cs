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
    /// Unit tests for the <see cref="MemoryStreamGZip"/> class.
    /// </summary>
    public class UsingMemoryStreamGZip : UnitTestBase
    {
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Initializes a new instance of the <see cref="UsingMemoryStreamGZip"/> class.
        /// </summary>
        /// <param name="xUnitTestOutputHelper">
        /// The Xunit test output helper that can be used to output test messages
        /// </param>
        public UsingMemoryStreamGZip(ITestOutputHelper xUnitTestOutputHelper) : base(xUnitTestOutputHelper)
        {
        }
        //--------------------------------------------------------------------------------

        #region Test Methods

        //================================================================================

        #region GZip Compression Tests

        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemoryStreamSlim"/> class when utilizing GZip compression by
        /// compressing a byte array and then decompressing it to verify that the original data is
        /// restored.
        /// </summary>
        [Fact]
        public void UsingMemoryStreamGZip_ByteArraySmallestSize_DataIsCompressedAndDecompressIsValid()
        {
            byte[] testData = GetRandomBytes(200_000);

            // Compress the data            
            MemoryStreamSlim compressedStream = MemoryStreamGZip.Compress(testData);

            compressedStream.Position.Should().Be(0);

            // Decompress the data and verify it is the same as the original data
            MemoryStreamSlim decompressedData = MemoryStreamGZip.Decompress(compressedStream);
            decompressedData.Position.Should().Be(0);
            compressedStream.Position.Should().Be(0);
            decompressedData.ToArray().Should().BeEquivalentTo(testData);
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemoryStreamSlim"/> class when utilizing GZip compression by
        /// compressing an empty byte array which should be an empty stream result and then 
        /// decompressing it to verify that the returned stream is also empty.
        /// </summary>
        [Fact]
        public void UsingMemoryStreamGZip_EmptyByteArraySmallestSize_EmptyStreamReturned()
        {
            byte[] testData = [];

            // Compress the data            
            MemoryStreamSlim compressedStream = MemoryStreamGZip.Compress(testData);

            compressedStream.Length.Should().Be(0);

            // Decompress the data and verify it is the same as the original data
            MemoryStreamSlim decompressedData = MemoryStreamGZip.Decompress(compressedStream);
            decompressedData.Length.Should().Be(0);
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemoryStreamSlim"/> class when utilizing GZip compression by
        /// compressing a byte array and then decompressing it to verify that the original data is
        /// restored.
        /// </summary>
        [Fact]
        public void UsingMemoryStreamGZip_ByteArraySmallestSize_WithOptions_DataIsCompressedAndDecompressIsValid()
        {
            byte[] testData = GetRandomBytes(200_000);

            // Compress the data            
            MemoryStreamSlim compressedStream = MemoryStreamGZip.Compress(testData,
                new MemoryStreamGZipOptions().WithCompressionLevel(CompressionLevel.SmallestSize));

            compressedStream.Position.Should().Be(0);

            // Decompress the data and verify it is the same as the original data
            MemoryStreamSlim decompressedData = MemoryStreamGZip.Decompress(compressedStream,
                new MemoryStreamSlimOptions { ZeroBufferBehavior = MemoryStreamSlimZeroBufferOption.None});
            decompressedData.Position.Should().Be(0);
            compressedStream.Position.Should().Be(0);
            decompressedData.ToArray().Should().BeEquivalentTo(testData);
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemoryStreamSlim"/> class when utilizing GZip compression by
        /// compressing an empty byte array which should be an empty stream result and then 
        /// decompressing it to verify that the returned stream is also empty.
        /// </summary>
        [Fact]
        public void UsingMemoryStreamGZip_EmptyByteArraySmallestSize_WithOptions_EmptyStreamReturned()
        {
            byte[] testData = [];

            // Compress the data            
            MemoryStreamSlim compressedStream = MemoryStreamGZip.Compress(testData,
                new MemoryStreamGZipOptions().WithCompressionLevel(CompressionLevel.SmallestSize));

            compressedStream.Length.Should().Be(0);

            // Decompress the data and verify it is the same as the original data
            MemoryStreamSlim decompressedData = MemoryStreamGZip.Decompress(compressedStream,
                new MemoryStreamSlimOptions { ZeroBufferBehavior = MemoryStreamSlimZeroBufferOption.None });
            decompressedData.Length.Should().Be(0);
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemoryStreamSlim"/> class when utilizing GZip compression by
        /// compressing a byte array and then decompressing it to verify that the original data is
        /// restored.
        /// </summary>
        [Fact]
        public void UsingMemoryStreamGZip_ByteArraySmallestSize_WithOptionsSetup_DataIsCompressedAndDecompressIsValid()
        {
            byte[] testData = GetRandomBytes(200_000);

            // Compress the data            
            MemoryStreamSlim compressedStream = MemoryStreamGZip.Compress(testData,
                options => options.WithCompressionLevel(CompressionLevel.SmallestSize));

            compressedStream.Position.Should().Be(0);

            // Decompress the data and verify it is the same as the original data
            MemoryStreamSlim decompressedData = MemoryStreamGZip.Decompress(compressedStream, 
                options => options.WithZeroBufferBehavior( MemoryStreamSlimZeroBufferOption.None));
            decompressedData.Position.Should().Be(0);
            compressedStream.Position.Should().Be(0);
            decompressedData.ToArray().Should().BeEquivalentTo(testData);
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemoryStreamSlim"/> class when utilizing GZip compression by
        /// compressing an empty byte array which should be an empty stream result and then 
        /// decompressing it to verify that the returned stream is also empty.
        /// </summary>
        [Fact]
        public void UsingMemoryStreamGZip_EmptyByteArraySmallestSize_WithOptionsSetup_EmptyStreamReturned()
        {
            byte[] testData = [];

            // Compress the data            
            MemoryStreamSlim compressedStream = MemoryStreamGZip.Compress(testData,
                options => options.WithCompressionLevel(CompressionLevel.SmallestSize));

            compressedStream.Length.Should().Be(0);

            // Decompress the data and verify it is the same as the original data
            MemoryStreamSlim decompressedData = MemoryStreamGZip.Decompress(compressedStream,
                options => options.WithZeroBufferBehavior(MemoryStreamSlimZeroBufferOption.None));
            decompressedData.Length.Should().Be(0);
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemoryStreamSlim"/> class when utilizing GZip compression by
        /// compressing a byte array and then decompressing it to verify that the original data is
        /// restored.
        /// </summary>
        [Fact]
        public void UsingMemoryStreamGZip_ByteArraySmallestSize_WithOptionsStateSetup_DataIsCompressedAndDecompressIsValid()
        {
            byte[] testData = GetRandomBytes(200_000);

            // Compress the data            
            MemoryStreamSlim compressedStream = MemoryStreamGZip.Compress(testData,
                (options, state) => options.WithCompressionLevel(state), CompressionLevel.SmallestSize);

            compressedStream.Position.Should().Be(0);

            // Decompress the data and verify it is the same as the original data
            MemoryStreamSlim decompressedData = MemoryStreamGZip.Decompress(compressedStream);
            decompressedData.Position.Should().Be(0);
            compressedStream.Position.Should().Be(0);
            decompressedData.ToArray().Should().BeEquivalentTo(testData);
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemoryStreamSlim"/> class when utilizing GZip compression by
        /// compressing an empty byte array which should be an empty stream result and then 
        /// decompressing it to verify that the returned stream is also empty.
        /// </summary>
        [Fact]
        public void UsingMemoryStreamGZip_EmptyByteArraySmallestSize_WithOptionsStateSetup_EmptyStreamReturned()
        {
            byte[] testData = [];

            // Compress the data            
            MemoryStreamSlim compressedStream = MemoryStreamGZip.Compress(testData,
                (options, state) => options.WithCompressionLevel(state), CompressionLevel.SmallestSize);

            compressedStream.Length.Should().Be(0);

            // Decompress the data and verify it is the same as the original data
            MemoryStreamSlim decompressedData = MemoryStreamGZip.Decompress(compressedStream,
                (options, state) => options.WithZeroBufferBehavior(state), MemoryStreamSlimZeroBufferOption.None);
            decompressedData.Length.Should().Be(0);
        }
        //--------------------------------------------------------------------------------    

        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemoryStreamSlim"/> class when utilizing GZip compression by
        /// compressing a byte Stream and then decompressing it to verify that the original data is
        /// restored.
        /// </summary>
        [Fact]
        public void UsingMemoryStreamGZip_SimpleStream_DataIsCompressedAndDecompressIsValid()
        {
            MemoryStream testData = new(GetRandomBytes(200_000));

            // Compress the data            
            MemoryStreamSlim compressedStream = MemoryStreamGZip.Compress(testData);

            compressedStream.Position.Should().Be(0);

            // Decompress the data and verify it is the same as the original data
            MemoryStreamSlim decompressedData = MemoryStreamGZip.Decompress(compressedStream);
            decompressedData.Position.Should().Be(0);
            compressedStream.Position.Should().Be(0);
            decompressedData.ToArray().Should().BeEquivalentTo(testData.ToArray());
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemoryStreamSlim"/> class when utilizing GZip compression by
        /// compressing an empty byte stream which should be an empty stream result and then 
        /// decompressing it to verify that the returned stream is also empty.
        /// </summary>
        [Fact]
        public void UsingMemoryStreamGZip_EmptySimpleStream_EmptyStreamReturned()
        {
            MemoryStream testData = new();

            // Compress the data            
            MemoryStreamSlim compressedStream = MemoryStreamGZip.Compress(testData);

            compressedStream.Length.Should().Be(0);

            // Decompress the data and verify it is the same as the original data
            MemoryStreamSlim decompressedData = MemoryStreamGZip.Decompress(compressedStream);
            decompressedData.Length.Should().Be(0);
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemoryStreamSlim"/> class when utilizing GZip compression by
        /// compressing a byte Stream and then decompressing it to verify that the original data is
        /// restored.
        /// </summary>
        [Fact]
        public void UsingMemoryStreamGZip_SimpleStream_WithOptions_DataIsCompressedAndDecompressIsValid()
        {
            MemoryStream testData = new(GetRandomBytes(200_000));

            // Compress the data            
            MemoryStreamSlim compressedStream = MemoryStreamGZip.Compress(testData,
                new MemoryStreamGZipOptions().WithCompressionLevel(CompressionLevel.SmallestSize));

            compressedStream.Position.Should().Be(0);

            // Decompress the data and verify it is the same as the original data
            MemoryStreamSlim decompressedData = MemoryStreamGZip.Decompress(compressedStream,
                new MemoryStreamSlimOptions { ZeroBufferBehavior = MemoryStreamSlimZeroBufferOption.None });
            decompressedData.Position.Should().Be(0);
            compressedStream.Position.Should().Be(0);
            decompressedData.ToArray().Should().BeEquivalentTo(testData.ToArray());
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemoryStreamSlim"/> class when utilizing GZip compression by
        /// compressing an empty byte stream which should be an empty stream result and then 
        /// decompressing it to verify that the returned stream is also empty.
        /// </summary>
        [Fact]
        public void UsingMemoryStreamGZip_EmptySimpleStream_WithOptions_EmptyStreamReturned()
        {
            MemoryStream testData = new();

            // Compress the data            
            MemoryStreamSlim compressedStream = MemoryStreamGZip.Compress(testData,
                new MemoryStreamGZipOptions().WithCompressionLevel(CompressionLevel.SmallestSize));

            compressedStream.Length.Should().Be(0);

            // Decompress the data and verify it is the same as the original data
            MemoryStreamSlim decompressedData = MemoryStreamGZip.Decompress(compressedStream,
                new MemoryStreamSlimOptions { ZeroBufferBehavior = MemoryStreamSlimZeroBufferOption.None });
            decompressedData.Length.Should().Be(0);
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemoryStreamSlim"/> class when utilizing GZip compression by
        /// compressing a byte Stream and then decompressing it to verify that the original data is
        /// restored.
        /// </summary>
        [Fact]
        public void UsingMemoryStreamGZip_SimpleStream_WithOptionsSetup_DataIsCompressedAndDecompressIsValid()
        {
            MemoryStream testData = new(GetRandomBytes(200_000));

            // Compress the data            
            MemoryStreamSlim compressedStream = MemoryStreamGZip.Compress(testData,
                options => options.WithCompressionLevel(CompressionLevel.SmallestSize));

            compressedStream.Position.Should().Be(0);

            // Decompress the data and verify it is the same as the original data
            MemoryStreamSlim decompressedData = MemoryStreamGZip.Decompress(compressedStream,
                options => options.WithZeroBufferBehavior(MemoryStreamSlimZeroBufferOption.None));
            decompressedData.Position.Should().Be(0);
            compressedStream.Position.Should().Be(0);
            decompressedData.ToArray().Should().BeEquivalentTo(testData.ToArray());
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemoryStreamSlim"/> class when utilizing GZip compression by
        /// compressing an empty byte stream which should be an empty stream result and then 
        /// decompressing it to verify that the returned stream is also empty.
        /// </summary>
        [Fact]
        public void UsingMemoryStreamGZip_EmptySimpleStream_WithOptionsSetup_EmptyStreamReturned()
        {
            MemoryStream testData = new();

            // Compress the data            
            MemoryStreamSlim compressedStream = MemoryStreamGZip.Compress(testData,
                options => options.WithCompressionLevel(CompressionLevel.SmallestSize));

            compressedStream.Length.Should().Be(0);

            // Decompress the data and verify it is the same as the original data
            MemoryStreamSlim decompressedData = MemoryStreamGZip.Decompress(compressedStream,
                options => options.WithZeroBufferBehavior(MemoryStreamSlimZeroBufferOption.None));
            decompressedData.Length.Should().Be(0);
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemoryStreamSlim"/> class when utilizing GZip compression by
        /// compressing a byte Stream and then decompressing it to verify that the original data is
        /// restored.
        /// </summary>
        [Fact]
        public void UsingMemoryStreamGZip_SimpleStream_WithOptionsStateSetup_DataIsCompressedAndDecompressIsValid()
        {
            MemoryStream testData = new(GetRandomBytes(200_000));

            // Compress the data            
            MemoryStreamSlim compressedStream = MemoryStreamGZip.Compress(testData,
                (options, state) => options.WithCompressionLevel(state), CompressionLevel.SmallestSize);

            compressedStream.Position.Should().Be(0);

            // Decompress the data and verify it is the same as the original data
            MemoryStreamSlim decompressedData = MemoryStreamGZip.Decompress(compressedStream,
                (options, state) => options.WithZeroBufferBehavior(state), MemoryStreamSlimZeroBufferOption.None);
            decompressedData.Position.Should().Be(0);
            compressedStream.Position.Should().Be(0);
            decompressedData.ToArray().Should().BeEquivalentTo(testData.ToArray());
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemoryStreamSlim"/> class when utilizing GZip compression by
        /// compressing an empty byte stream which should be an empty stream result and then 
        /// decompressing it to verify that the returned stream is also empty.
        /// </summary>
        [Fact]
        public void UsingMemoryStreamGZip_EmptySimpleStream_WithOptionsStateSetup_EmptyStreamReturned()
        {
            MemoryStream testData = new();

            // Compress the data            
            MemoryStreamSlim compressedStream = MemoryStreamGZip.Compress(testData,
                (options, state) => options.WithCompressionLevel(state), CompressionLevel.SmallestSize);

            compressedStream.Length.Should().Be(0);

            // Decompress the data and verify it is the same as the original data
            MemoryStreamSlim decompressedData = MemoryStreamGZip.Decompress(compressedStream,
                (options, state) => options.WithZeroBufferBehavior(state), MemoryStreamSlimZeroBufferOption.None);
            decompressedData.Length.Should().Be(0);
        }
        //--------------------------------------------------------------------------------    

        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemoryStreamSlim"/> class when utilizing GZip compression by
        /// compressing a byte Stream and then decompressing it to verify that the original data is
        /// restored.
        /// </summary>
        [Fact]
        public async Task UsingMemoryStreamGZip_SimpleStream_DataIsCompressedAndDecompressIsValidAsync()
        {
            MemoryStream testData = new(GetRandomBytes(200_000));

            // Compress the data            
            MemoryStreamSlim compressedStream = await MemoryStreamGZip.CompressAsync(testData, CancellationToken.None);

            compressedStream.Position.Should().Be(0);

            // Decompress the data and verify it is the same as the original data
            MemoryStreamSlim decompressedData = await MemoryStreamGZip.DecompressAsync(compressedStream, CancellationToken.None);
            decompressedData.Position.Should().Be(0);
            compressedStream.Position.Should().Be(0);
            decompressedData.ToArray().Should().BeEquivalentTo(testData.ToArray());
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemoryStreamSlim"/> class when utilizing GZip compression by
        /// compressing an empty byte stream which should be an empty stream result and then 
        /// decompressing it to verify that the returned stream is also empty.
        /// </summary>
        [Fact]
        public async Task UsingMemoryStreamGZip_EmptySimpleStream_EmptyStreamReturnedAsync()
        {
            MemoryStream testData = new();

            // Compress the data            
            MemoryStreamSlim compressedStream = await MemoryStreamGZip.CompressAsync(testData, CancellationToken.None);

            compressedStream.Length.Should().Be(0);

            // Decompress the data and verify it is the same as the original data
            MemoryStreamSlim decompressedData = await MemoryStreamGZip.DecompressAsync(compressedStream, CancellationToken.None);
            decompressedData.Length.Should().Be(0);
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemoryStreamSlim"/> class when utilizing GZip compression by
        /// compressing a byte Stream and then decompressing it to verify that the original data is
        /// restored.
        /// </summary>
        [Fact]
        public async Task UsingMemoryStreamGZip_SimpleStream_WithOptions_DataIsCompressedAndDecompressIsValidAsync()
        {
            MemoryStream testData = new(GetRandomBytes(200_000));

            // Compress the data            
            MemoryStreamSlim compressedStream = await MemoryStreamGZip.CompressAsync(testData,
                new MemoryStreamGZipOptions().WithCompressionLevel(CompressionLevel.SmallestSize), CancellationToken.None);

            compressedStream.Position.Should().Be(0);

            // Decompress the data and verify it is the same as the original data
            MemoryStreamSlim decompressedData = await MemoryStreamGZip.DecompressAsync(compressedStream,
                new MemoryStreamSlimOptions { ZeroBufferBehavior = MemoryStreamSlimZeroBufferOption.None }, CancellationToken.None);
            decompressedData.Position.Should().Be(0);
            compressedStream.Position.Should().Be(0);
            decompressedData.ToArray().Should().BeEquivalentTo(testData.ToArray());
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemoryStreamSlim"/> class when utilizing GZip compression by
        /// compressing an empty byte stream which should be an empty stream result and then 
        /// decompressing it to verify that the returned stream is also empty.
        /// </summary>
        [Fact]
        public async Task UsingMemoryStreamGZip_EmptySimpleStream_WithOptions_EmptyStreamReturnedAsync()
        {
            MemoryStream testData = new();

            // Compress the data            
            MemoryStreamSlim compressedStream = await MemoryStreamGZip.CompressAsync(testData,
                new MemoryStreamGZipOptions().WithCompressionLevel(CompressionLevel.SmallestSize), CancellationToken.None);

            compressedStream.Length.Should().Be(0);

            // Decompress the data and verify it is the same as the original data
            MemoryStreamSlim decompressedData = await MemoryStreamGZip.DecompressAsync(compressedStream,
                new MemoryStreamSlimOptions { ZeroBufferBehavior = MemoryStreamSlimZeroBufferOption.None }, CancellationToken.None);
            decompressedData.Length.Should().Be(0);
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemoryStreamSlim"/> class when utilizing GZip compression by
        /// compressing a byte Stream and then decompressing it to verify that the original data is
        /// restored.
        /// </summary>
        [Fact]
        public async Task UsingMemoryStreamGZip_SimpleStream_WithOptionsSetup_DataIsCompressedAndDecompressIsValidAsync()
        {
            MemoryStream testData = new(GetRandomBytes(200_000));

            // Compress the data            
            MemoryStreamSlim compressedStream = await MemoryStreamGZip.CompressAsync(testData,
                options => options.WithCompressionLevel(CompressionLevel.SmallestSize), CancellationToken.None);

            compressedStream.Position.Should().Be(0);

            // Decompress the data and verify it is the same as the original data
            MemoryStreamSlim decompressedData = await MemoryStreamGZip.DecompressAsync(compressedStream,
                options => options.WithZeroBufferBehavior(MemoryStreamSlimZeroBufferOption.None), CancellationToken.None);
            decompressedData.Position.Should().Be(0);
            compressedStream.Position.Should().Be(0);
            decompressedData.ToArray().Should().BeEquivalentTo(testData.ToArray());
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemoryStreamSlim"/> class when utilizing GZip compression by
        /// compressing an empty byte stream which should be an empty stream result and then 
        /// decompressing it to verify that the returned stream is also empty.
        /// </summary>
        [Fact]
        public async Task UsingMemoryStreamGZip_EmptySimpleStream_WithOptionsSetup_EmptyStreamReturnedAsync()
        {
            MemoryStream testData = new();

            // Compress the data            
            MemoryStreamSlim compressedStream = await MemoryStreamGZip.CompressAsync(testData,
                options => options.WithCompressionLevel(CompressionLevel.SmallestSize), CancellationToken.None);

            compressedStream.Length.Should().Be(0);

            // Decompress the data and verify it is the same as the original data
            MemoryStreamSlim decompressedData = await MemoryStreamGZip.DecompressAsync(compressedStream,
                options => options.WithZeroBufferBehavior(MemoryStreamSlimZeroBufferOption.None), CancellationToken.None);
            decompressedData.Length.Should().Be(0);
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemoryStreamSlim"/> class when utilizing GZip compression by
        /// compressing a byte Stream and then decompressing it to verify that the original data is
        /// restored.
        /// </summary>
        [Fact]
        public async Task UsingMemoryStreamGZip_SimpleStream_WithOptionsStateSetup_DataIsCompressedAndDecompressIsValidAsync()
        {
            MemoryStream testData = new(GetRandomBytes(200_000));

            // Compress the data            
            MemoryStreamSlim compressedStream = await MemoryStreamGZip.CompressAsync(testData,
                (options, state) => options.WithCompressionLevel(state), CompressionLevel.SmallestSize, CancellationToken.None);

            compressedStream.Position.Should().Be(0);

            // Decompress the data and verify it is the same as the original data
            MemoryStreamSlim decompressedData = await MemoryStreamGZip.DecompressAsync(compressedStream,
                (options, state) => options.WithZeroBufferBehavior(state), MemoryStreamSlimZeroBufferOption.None, CancellationToken.None);
            decompressedData.Position.Should().Be(0);
            compressedStream.Position.Should().Be(0);
            decompressedData.ToArray().Should().BeEquivalentTo(testData.ToArray());
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemoryStreamSlim"/> class when utilizing GZip compression by
        /// compressing an empty byte stream which should be an empty stream result and then 
        /// decompressing it to verify that the returned stream is also empty.
        /// </summary>
        [Fact]
        public async Task UsingMemoryStreamGZip_EmptySimpleStream_WithOptionsStateSetup_EmptyStreamReturnedAsync()
        {
            MemoryStream testData = new();

            // Compress the data            
            MemoryStreamSlim compressedStream = await MemoryStreamGZip.CompressAsync(testData,
                (options, state) => options.WithCompressionLevel(state), CompressionLevel.SmallestSize, CancellationToken.None);

            compressedStream.Length.Should().Be(0);

            // Decompress the data and verify it is the same as the original data
            MemoryStreamSlim decompressedData = await MemoryStreamGZip.DecompressAsync(compressedStream,
                (options, state) => options.WithZeroBufferBehavior(state), MemoryStreamSlimZeroBufferOption.None, CancellationToken.None);
            decompressedData.Length.Should().Be(0);
        }
        //--------------------------------------------------------------------------------    

        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemoryStreamSlim"/> class when utilizing GZip compression by
        /// compressing a byte ReadOnlySpan and then decompressing it to verify that the original data is
        /// restored.
        /// </summary>
        [Fact]
        public void UsingMemoryStreamGZip_SimpleReadOnlySpan_DataIsCompressedAndDecompressIsValid()
        {
            ReadOnlySpan<byte> testData = GetRandomBytes(200_000);

            // Compress the data            
            MemoryStreamSlim compressedStream = MemoryStreamGZip.Compress(testData);

            compressedStream.Position.Should().Be(0);

            // Decompress the data and verify it is the same as the original data
            MemoryStreamSlim decompressedData = MemoryStreamGZip.Decompress(compressedStream);
            decompressedData.Position.Should().Be(0);
            compressedStream.Position.Should().Be(0);
            decompressedData.ToArray().Should().BeEquivalentTo(testData.ToArray());
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemoryStreamSlim"/> class when utilizing GZip compression by
        /// compressing an empty byte ReadOnlySpan which should be an empty stream result and then 
        /// decompressing it to verify that the returned stream is also empty.
        /// </summary>
        [Fact]
        public void UsingMemoryStreamGZip_EmptyReadOnlySpan_EmptyStreamReturned()
        {
            ReadOnlySpan<byte> testData = [];

            // Compress the data            
            MemoryStreamSlim compressedStream = MemoryStreamGZip.Compress(testData);

            compressedStream.Length.Should().Be(0);

            // Decompress the data and verify it is the same as the original data
            MemoryStreamSlim decompressedData = MemoryStreamGZip.Decompress(compressedStream);
            decompressedData.Length.Should().Be(0);
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemoryStreamSlim"/> class when utilizing GZip compression by
        /// compressing a byte ReadOnlySpan and then decompressing it to verify that the original data is
        /// restored.
        /// </summary>
        [Fact]
        public void UsingMemoryStreamGZip_SimpleReadOnlySpan_WithOptions_DataIsCompressedAndDecompressIsValid()
        {
            ReadOnlySpan<byte> testData = GetRandomBytes(200_000);

            // Compress the data            
            MemoryStreamSlim compressedStream = MemoryStreamGZip.Compress(testData,
                new MemoryStreamGZipOptions().WithCompressionLevel(CompressionLevel.SmallestSize));

            compressedStream.Position.Should().Be(0);

            // Decompress the data and verify it is the same as the original data
            MemoryStreamSlim decompressedData = MemoryStreamGZip.Decompress(compressedStream,
                new MemoryStreamSlimOptions { ZeroBufferBehavior = MemoryStreamSlimZeroBufferOption.None });
            decompressedData.Position.Should().Be(0);
            compressedStream.Position.Should().Be(0);
            decompressedData.ToArray().Should().BeEquivalentTo(testData.ToArray());
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemoryStreamSlim"/> class when utilizing GZip compression by
        /// compressing an empty byte ReadOnlySpan which should be an empty stream result and then 
        /// decompressing it to verify that the returned stream is also empty.
        /// </summary>
        [Fact]
        public void UsingMemoryStreamGZip_EmptyReadOnlySpan_WithOptions_EmptyStreamReturned()
        {
            ReadOnlySpan<byte> testData = [];

            // Compress the data            
            MemoryStreamSlim compressedStream = MemoryStreamGZip.Compress(testData,
                new MemoryStreamGZipOptions().WithCompressionLevel(CompressionLevel.SmallestSize));

            compressedStream.Length.Should().Be(0);

            // Decompress the data and verify it is the same as the original data
            MemoryStreamSlim decompressedData = MemoryStreamGZip.Decompress(compressedStream,
                new MemoryStreamSlimOptions { ZeroBufferBehavior = MemoryStreamSlimZeroBufferOption.None });
            decompressedData.Length.Should().Be(0);
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemoryStreamSlim"/> class when utilizing GZip compression by
        /// compressing a byte ReadOnlySpan and then decompressing it to verify that the original data is
        /// restored.
        /// </summary>
        [Fact]
        public void UsingMemoryStreamGZip_SimpleReadOnlySpan_WithOptionsSetup_DataIsCompressedAndDecompressIsValid()
        {
            ReadOnlySpan<byte> testData = GetRandomBytes(200_000);

            // Compress the data            
            MemoryStreamSlim compressedStream = MemoryStreamGZip.Compress(testData,
                options => options.WithCompressionLevel(CompressionLevel.SmallestSize));

            compressedStream.Position.Should().Be(0);

            // Decompress the data and verify it is the same as the original data
            MemoryStreamSlim decompressedData = MemoryStreamGZip.Decompress(compressedStream,
                options => options.WithZeroBufferBehavior(MemoryStreamSlimZeroBufferOption.None));
            decompressedData.Position.Should().Be(0);
            compressedStream.Position.Should().Be(0);
            decompressedData.ToArray().Should().BeEquivalentTo(testData.ToArray());
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemoryStreamSlim"/> class when utilizing GZip compression by
        /// compressing an empty byte ReadOnlySpan which should be an empty stream result and then 
        /// decompressing it to verify that the returned stream is also empty.
        /// </summary>
        [Fact]
        public void UsingMemoryStreamGZip_EmptyReadOnlySpan_WithOptionsSetup_EmptyStreamReturned()
        {
            ReadOnlySpan<byte> testData = [];

            // Compress the data            
            MemoryStreamSlim compressedStream = MemoryStreamGZip.Compress(testData,
                options => options.WithCompressionLevel(CompressionLevel.SmallestSize));

            compressedStream.Length.Should().Be(0);

            // Decompress the data and verify it is the same as the original data
            MemoryStreamSlim decompressedData = MemoryStreamGZip.Decompress(compressedStream,
                options => options.WithZeroBufferBehavior(MemoryStreamSlimZeroBufferOption.None));
            decompressedData.Length.Should().Be(0);
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemoryStreamSlim"/> class when utilizing GZip compression by
        /// compressing a byte ReadOnlySpan and then decompressing it to verify that the original data is
        /// restored.
        /// </summary>
        [Fact]
        public void UsingMemoryStreamGZip_SimpleReadOnlySpan_WithOptionsStateSetup_DataIsCompressedAndDecompressIsValid()
        {
            ReadOnlySpan<byte> testData = GetRandomBytes(200_000);

            // Compress the data            
            MemoryStreamSlim compressedStream = MemoryStreamGZip.Compress(testData,
                (options, state) => options.WithCompressionLevel(state), CompressionLevel.SmallestSize);

            compressedStream.Position.Should().Be(0);

            // Decompress the data and verify it is the same as the original data
            MemoryStreamSlim decompressedData = MemoryStreamGZip.Decompress(compressedStream,
                (options, state) => options.WithZeroBufferBehavior(state), MemoryStreamSlimZeroBufferOption.None);
            decompressedData.Position.Should().Be(0);
            compressedStream.Position.Should().Be(0);
            decompressedData.ToArray().Should().BeEquivalentTo(testData.ToArray());
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemoryStreamSlim"/> class when utilizing GZip compression by
        /// compressing an empty byte ReadOnlySpan which should be an empty stream result and then 
        /// decompressing it to verify that the returned stream is also empty.
        /// </summary>
        [Fact]
        public void UsingMemoryStreamGZip_EmptyReadOnlySpan_WithOptionsStateSetup_EmptyStreamReturned()
        {
            ReadOnlySpan<byte> testData = [];

            // Compress the data            
            MemoryStreamSlim compressedStream = MemoryStreamGZip.Compress(testData,
                (options, state) => options.WithCompressionLevel(state), CompressionLevel.SmallestSize);

            compressedStream.Length.Should().Be(0);

            // Decompress the data and verify it is the same as the original data
            MemoryStreamSlim decompressedData = MemoryStreamGZip.Decompress(compressedStream,
                (options, state) => options.WithZeroBufferBehavior(state), MemoryStreamSlimZeroBufferOption.None);
            decompressedData.Length.Should().Be(0);
        }
        //--------------------------------------------------------------------------------    

        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemoryStreamSlim"/> class when utilizing GZip compression by
        /// compressing a string and then decompressing it to verify that the original data is
        /// restored.
        /// </summary>
        [Fact]
        public void UsingMemoryStreamGZip_SimpleString_DataIsCompressedAndDecompressIsValid()
        {
            string testData = GetRandomString(1_000, 2_000);

            // Compress the data            
            MemoryStreamSlim compressedStream = MemoryStreamGZip.Compress(testData);

            compressedStream.Position.Should().Be(0);

            // Decompress the data and verify it is the same as the original data
            string decompressedString = MemoryStreamGZip.DecompressToString(compressedStream);
            compressedStream.Position.Should().Be(0);
            decompressedString.Should().Be(testData);
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemoryStreamSlim"/> class when utilizing GZip compression by
        /// compressing an empty string which should be an empty stream result and then 
        /// decompressing it to verify that the returned stream is also empty.
        /// </summary>
        [Fact]
        public void UsingMemoryStreamGZip_EmptyString_EmptyStreamReturned()
        {
            string testData = string.Empty;

            // Compress the data            
            MemoryStreamSlim compressedStream = MemoryStreamGZip.Compress(testData);

            compressedStream.Length.Should().Be(0);

            // Decompress the data and verify it is the same as the original data
            string decompressedString = MemoryStreamGZip.DecompressToString(compressedStream);
            decompressedString.Should().BeEmpty();
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemoryStreamSlim"/> class when utilizing GZip compression by
        /// compressing a string and then decompressing it to verify that the original data is
        /// restored.
        /// </summary>
        [Fact]
        public void UsingMemoryStreamGZip_SimpleString_WithOptions_DataIsCompressedAndDecompressIsValid()
        {
            string testData = GetRandomString(1_000, 2_000);

            // Compress the data            
            MemoryStreamSlim compressedStream = MemoryStreamGZip.Compress(testData,
                new MemoryStreamGZipOptions().WithCompressionLevel(CompressionLevel.SmallestSize));

            compressedStream.Position.Should().Be(0);

            // Decompress the data and verify it is the same as the original data
            string decompressedString = MemoryStreamGZip.DecompressToString(compressedStream,
                new MemoryStreamSlimOptions { ZeroBufferBehavior = MemoryStreamSlimZeroBufferOption.None });
            compressedStream.Position.Should().Be(0);
            decompressedString.Should().Be(testData);
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemoryStreamSlim"/> class when utilizing GZip compression by
        /// compressing an empty string which should be an empty stream result and then 
        /// decompressing it to verify that the returned stream is also empty.
        /// </summary>
        [Fact]
        public void UsingMemoryStreamGZip_EmptyString_WithOptions_EmptyStreamReturned()
        {
            string testData = string.Empty;

            // Compress the data            
            MemoryStreamSlim compressedStream = MemoryStreamGZip.Compress(testData,
                new MemoryStreamGZipOptions().WithCompressionLevel(CompressionLevel.SmallestSize));

            compressedStream.Length.Should().Be(0);

            // Decompress the data and verify it is the same as the original data
            string decompressedString = MemoryStreamGZip.DecompressToString(compressedStream,
                new MemoryStreamSlimOptions { ZeroBufferBehavior = MemoryStreamSlimZeroBufferOption.None });
            decompressedString.Should().BeEmpty();
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemoryStreamSlim"/> class when utilizing GZip compression by
        /// compressing a string and then decompressing it to verify that the original data is
        /// restored.
        /// </summary>
        [Fact]
        public void UsingMemoryStreamGZip_SimpleString_WithOptionsSetup_DataIsCompressedAndDecompressIsValid()
        {
            string testData = GetRandomString(1_000, 2_000);

            // Compress the data            
            MemoryStreamSlim compressedStream = MemoryStreamGZip.Compress(testData,
                options => options.WithCompressionLevel(CompressionLevel.SmallestSize));

            compressedStream.Position.Should().Be(0);

            // Decompress the data and verify it is the same as the original data
            string decompressedString = MemoryStreamGZip.DecompressToString(compressedStream,
                options => options.WithZeroBufferBehavior(MemoryStreamSlimZeroBufferOption.None));
            compressedStream.Position.Should().Be(0);
            decompressedString.Should().Be(testData);
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemoryStreamSlim"/> class when utilizing GZip compression by
        /// compressing an empty string which should be an empty stream result and then 
        /// decompressing it to verify that the returned stream is also empty.
        /// </summary>
        [Fact]
        public void UsingMemoryStreamGZip_EmptyString_WithOptionsSetup_EmptyStreamReturned()
        {
            string testData = string.Empty;

            // Compress the data            
            MemoryStreamSlim compressedStream = MemoryStreamGZip.Compress(testData,
                options => options.WithCompressionLevel(CompressionLevel.SmallestSize));

            compressedStream.Length.Should().Be(0);

            // Decompress the data and verify it is the same as the original data
            string decompressedString = MemoryStreamGZip.DecompressToString(compressedStream,
                options => options.WithZeroBufferBehavior(MemoryStreamSlimZeroBufferOption.None));
            decompressedString.Should().BeEmpty();
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemoryStreamSlim"/> class when utilizing GZip compression by
        /// compressing a string and then decompressing it to verify that the original data is
        /// restored.
        /// </summary>
        [Fact]
        public void UsingMemoryStreamGZip_SimpleString_WithOptionsStateSetup_DataIsCompressedAndDecompressIsValid()
        {
            string testData = GetRandomString(1_000, 2_000);

            // Compress the data            
            MemoryStreamSlim compressedStream = MemoryStreamGZip.Compress(testData,
                (options, state) => options.WithCompressionLevel(state), CompressionLevel.SmallestSize);

            compressedStream.Position.Should().Be(0);

            // Decompress the data and verify it is the same as the original data
            string decompressedString = MemoryStreamGZip.DecompressToString(compressedStream,
                (options, state) => options.WithZeroBufferBehavior(state), MemoryStreamSlimZeroBufferOption.None);
            compressedStream.Position.Should().Be(0);
            decompressedString.Should().Be(testData);
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemoryStreamSlim"/> class when utilizing GZip compression by
        /// compressing an empty string which should be an empty stream result and then 
        /// decompressing it to verify that the returned stream is also empty.
        /// </summary>
        [Fact]
        public void UsingMemoryStreamGZip_EmptyString_WithOptionsStateSetup_EmptyStreamReturned()
        {
            string testData = string.Empty;

            // Compress the data            
            MemoryStreamSlim compressedStream = MemoryStreamGZip.Compress(testData,
                (options, state) => options.WithCompressionLevel(state), CompressionLevel.SmallestSize);

            compressedStream.Length.Should().Be(0);

            // Decompress the data and verify it is the same as the original data
            string decompressedString = MemoryStreamGZip.DecompressToString(compressedStream,
                (options, state) => options.WithZeroBufferBehavior(state), MemoryStreamSlimZeroBufferOption.None);
            decompressedString.Should().BeEmpty();
        }
        //--------------------------------------------------------------------------------    

        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemoryStreamSlim"/> class when utilizing GZip compression by
        /// compressing a string and then decompressing it to verify that the original data is
        /// restored.
        /// </summary>
        [Fact]
        public async Task UsingMemoryStreamGZip_SimpleString_DataIsCompressedAndDecompressIsValidAsync()
        {
            string testData = GetRandomString(1_000, 2_000);

            // Compress the data            
            MemoryStreamSlim compressedStream = MemoryStreamGZip.Compress(testData);

            compressedStream.Position.Should().Be(0);

            // Decompress the data and verify it is the same as the original data
            string decompressedString = await MemoryStreamGZip.DecompressToStringAsync(compressedStream, CancellationToken.None);
            compressedStream.Position.Should().Be(0);
            decompressedString.Should().Be(testData);
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemoryStreamSlim"/> class when utilizing GZip compression by
        /// compressing an empty string which should be an empty stream result and then 
        /// decompressing it to verify that the returned stream is also empty.
        /// </summary>
        [Fact]
        public async Task UsingMemoryStreamGZip_EmptyString_EmptyStreamReturnedAsync()
        {
            string testData = string.Empty;

            // Compress the data            
            MemoryStreamSlim compressedStream = MemoryStreamGZip.Compress(testData);

            compressedStream.Length.Should().Be(0);

            // Decompress the data and verify it is the same as the original data
            string decompressedString = await MemoryStreamGZip.DecompressToStringAsync(compressedStream, CancellationToken.None);
            decompressedString.Should().BeEmpty();
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemoryStreamSlim"/> class when utilizing GZip compression by
        /// compressing a string and then decompressing it to verify that the original data is
        /// restored.
        /// </summary>
        [Fact]
        public async Task UsingMemoryStreamGZip_SimpleString_WithOptions_DataIsCompressedAndDecompressIsValidAsync()
        {
            string testData = GetRandomString(1_000, 2_000);

            // Compress the data            
            MemoryStreamSlim compressedStream = MemoryStreamGZip.Compress(testData,
                new MemoryStreamGZipOptions().WithCompressionLevel(CompressionLevel.SmallestSize));

            compressedStream.Position.Should().Be(0);

            // Decompress the data and verify it is the same as the original data
            string decompressedString = await MemoryStreamGZip.DecompressToStringAsync(compressedStream,
                new MemoryStreamSlimOptions { ZeroBufferBehavior = MemoryStreamSlimZeroBufferOption.None }, CancellationToken.None);
            compressedStream.Position.Should().Be(0);
            decompressedString.Should().Be(testData);
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemoryStreamSlim"/> class when utilizing GZip compression by
        /// compressing an empty string which should be an empty stream result and then 
        /// decompressing it to verify that the returned stream is also empty.
        /// </summary>
        [Fact]
        public async Task UsingMemoryStreamGZip_EmptyString_WithOptions_EmptyStreamReturnedAsync()
        {
            string testData = string.Empty;

            // Compress the data            
            MemoryStreamSlim compressedStream = MemoryStreamGZip.Compress(testData,
                new MemoryStreamGZipOptions().WithCompressionLevel(CompressionLevel.SmallestSize));

            compressedStream.Length.Should().Be(0);

            // Decompress the data and verify it is the same as the original data
            string decompressedString = await MemoryStreamGZip.DecompressToStringAsync(compressedStream,
                new MemoryStreamSlimOptions { ZeroBufferBehavior = MemoryStreamSlimZeroBufferOption.None }, CancellationToken.None);
            decompressedString.Should().BeEmpty();
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemoryStreamSlim"/> class when utilizing GZip compression by
        /// compressing a string and then decompressing it to verify that the original data is
        /// restored.
        /// </summary>
        [Fact]
        public async Task UsingMemoryStreamGZip_SimpleString_WithOptionsSetup_DataIsCompressedAndDecompressIsValidAsync()
        {
            string testData = GetRandomString(1_000, 2_000);

            // Compress the data            
            MemoryStreamSlim compressedStream = MemoryStreamGZip.Compress(testData,
                options => options.WithCompressionLevel(CompressionLevel.SmallestSize));

            compressedStream.Position.Should().Be(0);

            // Decompress the data and verify it is the same as the original data
            string decompressedString = await MemoryStreamGZip.DecompressToStringAsync(compressedStream,
                options => options.WithZeroBufferBehavior(MemoryStreamSlimZeroBufferOption.None), CancellationToken.None);
            compressedStream.Position.Should().Be(0);
            decompressedString.Should().Be(testData);
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemoryStreamSlim"/> class when utilizing GZip compression by
        /// compressing an empty string which should be an empty stream result and then 
        /// decompressing it to verify that the returned stream is also empty.
        /// </summary>
        [Fact]
        public async Task UsingMemoryStreamGZip_EmptyString_WithOptionsSetup_EmptyStreamReturnedAsync()
        {
            string testData = string.Empty;

            // Compress the data            
            MemoryStreamSlim compressedStream = MemoryStreamGZip.Compress(testData,
                options => options.WithCompressionLevel(CompressionLevel.SmallestSize));

            compressedStream.Length.Should().Be(0);

            // Decompress the data and verify it is the same as the original data
            string decompressedString = await MemoryStreamGZip.DecompressToStringAsync(compressedStream,
                options => options.WithZeroBufferBehavior(MemoryStreamSlimZeroBufferOption.None), CancellationToken.None);
            decompressedString.Should().BeEmpty();
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemoryStreamSlim"/> class when utilizing GZip compression by
        /// compressing a string and then decompressing it to verify that the original data is
        /// restored.
        /// </summary>
        [Fact]
        public async Task UsingMemoryStreamGZip_SimpleString_WithOptionsStateSetup_DataIsCompressedAndDecompressIsValidAsync()
        {
            string testData = GetRandomString(1_000, 2_000);

            // Compress the data            
            MemoryStreamSlim compressedStream = MemoryStreamGZip.Compress(testData,
                (options, state) => options.WithCompressionLevel(state), CompressionLevel.SmallestSize);

            compressedStream.Position.Should().Be(0);

            // Decompress the data and verify it is the same as the original data
            string decompressedString = await MemoryStreamGZip.DecompressToStringAsync(compressedStream,
                (options, state) => options.WithZeroBufferBehavior(state), MemoryStreamSlimZeroBufferOption.None, CancellationToken.None);
            compressedStream.Position.Should().Be(0);
            decompressedString.Should().Be(testData);
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemoryStreamSlim"/> class when utilizing GZip compression by
        /// compressing an empty string which should be an empty stream result and then 
        /// decompressing it to verify that the returned stream is also empty.
        /// </summary>
        [Fact]
        public async Task UsingMemoryStreamGZip_EmptyString_WithOptionsStateSetup_EmptyStreamReturnedAsync()
        {
            string testData = string.Empty;

            // Compress the data            
            MemoryStreamSlim compressedStream = MemoryStreamGZip.Compress(testData,
                (options, state) => options.WithCompressionLevel(state), CompressionLevel.SmallestSize);

            compressedStream.Length.Should().Be(0);

            // Decompress the data and verify it is the same as the original data
            string decompressedString = await MemoryStreamGZip.DecompressToStringAsync(compressedStream,
                (options, state) => options.WithZeroBufferBehavior(state), MemoryStreamSlimZeroBufferOption.None, CancellationToken.None);
            decompressedString.Should().BeEmpty();
        }
        //--------------------------------------------------------------------------------    

        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemoryStreamSlim"/> class when utilizing GZip compression by
        /// compressing a string and a specified encoding then decompressing it to verify 
        /// that the original data is restored.
        /// </summary>
        [Fact]
        public void UsingMemoryStreamGZip_SimpleStringWithEncoding_DataIsCompressedAndDecompressIsValid()
        {
            string testData = GetRandomString(1_000, 2_000);
            Encoding encoding = BitConverter.IsLittleEndian ? Encoding.BigEndianUnicode : Encoding.Unicode;

            // Compress the data            
            MemoryStreamSlim compressedStream = MemoryStreamGZip.Compress(testData, encoding);

            compressedStream.Position.Should().Be(0);

            // Decompress the data and verify it is the same as the original data
            string decompressedString = MemoryStreamGZip.DecompressToString(compressedStream, encoding);
            compressedStream.Position.Should().Be(0);
            decompressedString.Should().Be(testData);
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemoryStreamSlim"/> class when utilizing GZip compression by
        /// compressing an empty string and a specified encoding then decompressing it to verify 
        /// that the original data is restored.
        /// </summary>
        [Fact]
        public void UsingMemoryStreamGZip_EmptyStringWithEncoding_EmptyStreamReturned()
        {
            string testData = string.Empty;
            Encoding encoding = BitConverter.IsLittleEndian ? Encoding.BigEndianUnicode : Encoding.Unicode;

            // Compress the data            
            MemoryStreamSlim compressedStream = MemoryStreamGZip.Compress(testData, encoding);

            compressedStream.Length.Should().Be(0);

            // Decompress the data and verify it is the same as the original data
            string decompressedString = MemoryStreamGZip.DecompressToString(compressedStream, encoding);
            decompressedString.Should().BeEmpty();
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemoryStreamSlim"/> class when utilizing GZip compression by
        /// compressing a string and a specified encoding then decompressing it to verify 
        /// that the original data is restored.
        /// </summary>
        [Fact]
        public void UsingMemoryStreamGZip_SimpleStringWithEncoding_WithOptions_DataIsCompressedAndDecompressIsValid()
        {
            string testData = GetRandomString(1_000, 2_000);
            Encoding encoding = BitConverter.IsLittleEndian ? Encoding.BigEndianUnicode : Encoding.Unicode;

            // Compress the data            
            MemoryStreamSlim compressedStream = MemoryStreamGZip.Compress(testData, encoding,
                new MemoryStreamGZipOptions().WithCompressionLevel(CompressionLevel.SmallestSize));

            compressedStream.Position.Should().Be(0);

            // Decompress the data and verify it is the same as the original data
            string decompressedString = MemoryStreamGZip.DecompressToString(compressedStream, encoding,
                new MemoryStreamSlimOptions { ZeroBufferBehavior = MemoryStreamSlimZeroBufferOption.None });
            compressedStream.Position.Should().Be(0);
            decompressedString.Should().Be(testData);
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemoryStreamSlim"/> class when utilizing GZip compression by
        /// compressing an empty string and a specified encoding then decompressing it to verify 
        /// that the original data is restored.
        /// </summary>
        [Fact]
        public void UsingMemoryStreamGZip_EmptyStringWithEncoding_WithOptions_EmptyStreamReturned()
        {
            string testData = string.Empty;
            Encoding encoding = BitConverter.IsLittleEndian ? Encoding.BigEndianUnicode : Encoding.Unicode;

            // Compress the data            
            MemoryStreamSlim compressedStream = MemoryStreamGZip.Compress(testData, encoding,
                new MemoryStreamGZipOptions().WithCompressionLevel(CompressionLevel.SmallestSize));

            compressedStream.Length.Should().Be(0);

            // Decompress the data and verify it is the same as the original data
            string decompressedString = MemoryStreamGZip.DecompressToString(compressedStream, encoding,
                new MemoryStreamSlimOptions { ZeroBufferBehavior = MemoryStreamSlimZeroBufferOption.None });
            decompressedString.Should().BeEmpty();
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemoryStreamSlim"/> class when utilizing GZip compression by
        /// compressing a string and a specified encoding then decompressing it to verify 
        /// that the original data is restored.
        /// </summary>
        [Fact]
        public void UsingMemoryStreamGZip_SimpleStringWithEncoding_WithOptionsSetup_DataIsCompressedAndDecompressIsValid()
        {
            string testData = GetRandomString(1_000, 2_000);
            Encoding encoding = BitConverter.IsLittleEndian ? Encoding.BigEndianUnicode : Encoding.Unicode;

            // Compress the data            
            MemoryStreamSlim compressedStream = MemoryStreamGZip.Compress(testData, encoding,
                options => options.WithCompressionLevel(CompressionLevel.SmallestSize));

            compressedStream.Position.Should().Be(0);

            // Decompress the data and verify it is the same as the original data
            string decompressedString = MemoryStreamGZip.DecompressToString(compressedStream, encoding,
                options => options.WithZeroBufferBehavior(MemoryStreamSlimZeroBufferOption.None));
            compressedStream.Position.Should().Be(0);
            decompressedString.Should().Be(testData);
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemoryStreamSlim"/> class when utilizing GZip compression by
        /// compressing an empty string and a specified encoding then decompressing it to verify 
        /// that the original data is restored.
        /// </summary>
        [Fact]
        public void UsingMemoryStreamGZip_EmptyStringWithEncoding_WithOptionsSetup_EmptyStreamReturned()
        {
            string testData = string.Empty;
            Encoding encoding = BitConverter.IsLittleEndian ? Encoding.BigEndianUnicode : Encoding.Unicode;

            // Compress the data            
            MemoryStreamSlim compressedStream = MemoryStreamGZip.Compress(testData, encoding,
                options => options.WithCompressionLevel(CompressionLevel.SmallestSize));

            compressedStream.Length.Should().Be(0);

            // Decompress the data and verify it is the same as the original data
            string decompressedString = MemoryStreamGZip.DecompressToString(compressedStream, encoding,
                options => options.WithZeroBufferBehavior(MemoryStreamSlimZeroBufferOption.None));
            decompressedString.Should().BeEmpty();
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemoryStreamSlim"/> class when utilizing GZip compression by
        /// compressing a string and a specified encoding then decompressing it to verify 
        /// that the original data is restored.
        /// </summary>
        [Fact]
        public void UsingMemoryStreamGZip_SimpleStringWithEncoding_WithOptionsStateSetup_DataIsCompressedAndDecompressIsValid()
        {
            string testData = GetRandomString(1_000, 2_000);
            Encoding encoding = BitConverter.IsLittleEndian ? Encoding.BigEndianUnicode : Encoding.Unicode;

            // Compress the data            
            MemoryStreamSlim compressedStream = MemoryStreamGZip.Compress(testData, encoding,
                (options, state) => options.WithCompressionLevel(state), CompressionLevel.SmallestSize);

            compressedStream.Position.Should().Be(0);

            // Decompress the data and verify it is the same as the original data
            string decompressedString = MemoryStreamGZip.DecompressToString(compressedStream, encoding,
                (options, state) => options.WithZeroBufferBehavior(state), MemoryStreamSlimZeroBufferOption.None);
            compressedStream.Position.Should().Be(0);
            decompressedString.Should().Be(testData);
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemoryStreamSlim"/> class when utilizing GZip compression by
        /// compressing an empty string and a specified encoding then decompressing it to verify 
        /// that the original data is restored.
        /// </summary>
        [Fact]
        public void UsingMemoryStreamGZip_EmptyStringWithEncoding_WithOptionsStateSetup_EmptyStreamReturned()
        {
            string testData = string.Empty;
            Encoding encoding = BitConverter.IsLittleEndian ? Encoding.BigEndianUnicode : Encoding.Unicode;

            // Compress the data            
            MemoryStreamSlim compressedStream = MemoryStreamGZip.Compress(testData, encoding,
                (options, state) => options.WithCompressionLevel(state), CompressionLevel.SmallestSize);

            compressedStream.Length.Should().Be(0);

            // Decompress the data and verify it is the same as the original data
            string decompressedString = MemoryStreamGZip.DecompressToString(compressedStream, encoding,
                (options, state) => options.WithZeroBufferBehavior(state), MemoryStreamSlimZeroBufferOption.None);
            decompressedString.Should().BeEmpty();
        }
        //--------------------------------------------------------------------------------    

        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemoryStreamSlim"/> class when utilizing GZip compression by
        /// compressing a string and a specified encoding then decompressing it to verify 
        /// that the original data is restored.
        /// </summary>
        [Fact]
        public async Task UsingMemoryStreamGZip_SimpleStringWithEncoding_DataIsCompressedAndDecompressIsValidAsync()
        {
            string testData = GetRandomString(1_000, 2_000);
            Encoding encoding = BitConverter.IsLittleEndian ? Encoding.BigEndianUnicode : Encoding.Unicode;

            // Compress the data            
            MemoryStreamSlim compressedStream = MemoryStreamGZip.Compress(testData, encoding);

            compressedStream.Position.Should().Be(0);

            // Decompress the data and verify it is the same as the original data
            string decompressedString = await MemoryStreamGZip.DecompressToStringAsync(compressedStream, encoding,
                CancellationToken.None);
            compressedStream.Position.Should().Be(0);
            decompressedString.Should().Be(testData);
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemoryStreamSlim"/> class when utilizing GZip compression by
        /// compressing an empty string and a specified encoding then decompressing it to verify 
        /// that the original data is restored.
        /// </summary>
        [Fact]
        public async Task UsingMemoryStreamGZip_EmptyStringWithEncoding_EmptyStreamReturnedAsync()
        {
            string testData = string.Empty;
            Encoding encoding = BitConverter.IsLittleEndian ? Encoding.BigEndianUnicode : Encoding.Unicode;

            // Compress the data            
            MemoryStreamSlim compressedStream = MemoryStreamGZip.Compress(testData, encoding);

            compressedStream.Length.Should().Be(0);

            // Decompress the data and verify it is the same as the original data
            string decompressedString = await MemoryStreamGZip.DecompressToStringAsync(compressedStream, encoding,
                CancellationToken.None);
            decompressedString.Should().BeEmpty();
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemoryStreamSlim"/> class when utilizing GZip compression by
        /// compressing a string and a specified encoding then decompressing it to verify 
        /// that the original data is restored.
        /// </summary>
        [Fact]
        public async Task UsingMemoryStreamGZip_SimpleStringWithEncoding_WithOptions_DataIsCompressedAndDecompressIsValidAsync()
        {
            string testData = GetRandomString(1_000, 2_000);
            Encoding encoding = BitConverter.IsLittleEndian ? Encoding.BigEndianUnicode : Encoding.Unicode;

            // Compress the data            
            MemoryStreamSlim compressedStream = MemoryStreamGZip.Compress(testData, encoding,
                new MemoryStreamGZipOptions().WithCompressionLevel(CompressionLevel.SmallestSize));

            compressedStream.Position.Should().Be(0);

            // Decompress the data and verify it is the same as the original data
            string decompressedString = await MemoryStreamGZip.DecompressToStringAsync(compressedStream, encoding,
                new MemoryStreamSlimOptions { ZeroBufferBehavior = MemoryStreamSlimZeroBufferOption.None },
                CancellationToken.None);
            compressedStream.Position.Should().Be(0);
            decompressedString.Should().Be(testData);
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemoryStreamSlim"/> class when utilizing GZip compression by
        /// compressing an empty string and a specified encoding then decompressing it to verify 
        /// that the original data is restored.
        /// </summary>
        [Fact]
        public async Task UsingMemoryStreamGZip_EmptyStringWithEncoding_WithOptions_EmptyStreamReturnedAsync()
        {
            string testData = string.Empty;
            Encoding encoding = BitConverter.IsLittleEndian ? Encoding.BigEndianUnicode : Encoding.Unicode;

            // Compress the data            
            MemoryStreamSlim compressedStream = MemoryStreamGZip.Compress(testData, encoding,
                new MemoryStreamGZipOptions().WithCompressionLevel(CompressionLevel.SmallestSize));

            compressedStream.Length.Should().Be(0);

            // Decompress the data and verify it is the same as the original data
            string decompressedString = await MemoryStreamGZip.DecompressToStringAsync(compressedStream, encoding,
                new MemoryStreamSlimOptions { ZeroBufferBehavior = MemoryStreamSlimZeroBufferOption.None },
                CancellationToken.None);
            decompressedString.Should().BeEmpty();
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemoryStreamSlim"/> class when utilizing GZip compression by
        /// compressing a string and a specified encoding then decompressing it to verify 
        /// that the original data is restored.
        /// </summary>
        [Fact]
        public async Task UsingMemoryStreamGZip_SimpleStringWithEncoding_WithOptionsSetup_DataIsCompressedAndDecompressIsValidAsync()
        {
            string testData = GetRandomString(1_000, 2_000);
            Encoding encoding = BitConverter.IsLittleEndian ? Encoding.BigEndianUnicode : Encoding.Unicode;

            // Compress the data            
            MemoryStreamSlim compressedStream = MemoryStreamGZip.Compress(testData, encoding,
                options => options.WithCompressionLevel(CompressionLevel.SmallestSize));

            compressedStream.Position.Should().Be(0);

            // Decompress the data and verify it is the same as the original data
            string decompressedString = await MemoryStreamGZip.DecompressToStringAsync(compressedStream, encoding,
                options => options.WithZeroBufferBehavior(MemoryStreamSlimZeroBufferOption.None),
                CancellationToken.None);
            compressedStream.Position.Should().Be(0);
            decompressedString.Should().Be(testData);
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemoryStreamSlim"/> class when utilizing GZip compression by
        /// compressing an empty string and a specified encoding then decompressing it to verify 
        /// that the original data is restored.
        /// </summary>
        [Fact]
        public async Task UsingMemoryStreamGZip_EmptyStringWithEncoding_WithOptionsSetup_EmptyStreamReturnedAsync()
        {
            string testData = string.Empty;
            Encoding encoding = BitConverter.IsLittleEndian ? Encoding.BigEndianUnicode : Encoding.Unicode;

            // Compress the data            
            MemoryStreamSlim compressedStream = MemoryStreamGZip.Compress(testData, encoding,
                options => options.WithCompressionLevel(CompressionLevel.SmallestSize));

            compressedStream.Length.Should().Be(0);

            // Decompress the data and verify it is the same as the original data
            string decompressedString = await MemoryStreamGZip.DecompressToStringAsync(compressedStream, encoding,
                options => options.WithZeroBufferBehavior(MemoryStreamSlimZeroBufferOption.None),
                CancellationToken.None);
            decompressedString.Should().BeEmpty();
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemoryStreamSlim"/> class when utilizing GZip compression by
        /// compressing a string and a specified encoding then decompressing it to verify 
        /// that the original data is restored.
        /// </summary>
        [Fact]
        public async Task UsingMemoryStreamGZip_SimpleStringWithEncoding_WithOptionsStateSetup_DataIsCompressedAndDecompressIsValidAsync()
        {
            string testData = GetRandomString(1_000, 2_000);
            Encoding encoding = BitConverter.IsLittleEndian ? Encoding.BigEndianUnicode : Encoding.Unicode;

            // Compress the data            
            MemoryStreamSlim compressedStream = MemoryStreamGZip.Compress(testData, encoding,
                (options, state) => options.WithCompressionLevel(state), CompressionLevel.SmallestSize);

            compressedStream.Position.Should().Be(0);

            // Decompress the data and verify it is the same as the original data
            string decompressedString = await MemoryStreamGZip.DecompressToStringAsync(compressedStream, encoding,
                (options, state) => options.WithZeroBufferBehavior(state), MemoryStreamSlimZeroBufferOption.None,
                CancellationToken.None);
            compressedStream.Position.Should().Be(0);
            decompressedString.Should().Be(testData);
        }
        //--------------------------------------------------------------------------------    
        /// <summary>
        /// Tests the <see cref="MemoryStreamSlim"/> class when utilizing GZip compression by
        /// compressing an empty string and a specified encoding then decompressing it to verify 
        /// that the original data is restored.
        /// </summary>
        [Fact]
        public async Task UsingMemoryStreamGZip_EmptyStringWithEncoding_WithOptionsStateSetup_EmptyStreamReturnedAsync()
        {
            string testData = string.Empty;
            Encoding encoding = BitConverter.IsLittleEndian ? Encoding.BigEndianUnicode : Encoding.Unicode;

            // Compress the data            
            MemoryStreamSlim compressedStream = MemoryStreamGZip.Compress(testData, encoding,
                (options, state) => options.WithCompressionLevel(state), CompressionLevel.SmallestSize);

            compressedStream.Length.Should().Be(0);

            // Decompress the data and verify it is the same as the original data
            string decompressedString = await MemoryStreamGZip.DecompressToStringAsync(compressedStream, encoding,
                (options, state) => options.WithZeroBufferBehavior(state), MemoryStreamSlimZeroBufferOption.None,
                CancellationToken.None);
            decompressedString.Should().BeEmpty();
        }
        //--------------------------------------------------------------------------------    

        #endregion GZip Compression Tests

        //================================================================================

        #endregion Test Methods
    }
    //################################################################################
}
