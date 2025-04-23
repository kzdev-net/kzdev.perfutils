// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using FluentAssertions;

namespace KZDev.PerfUtils.Tests;

//################################################################################
/// <summary>
/// Unit tests for the <see cref="MemoryStreamSlim"/> class for cases when the stream
/// is created with a buffer that is already allocated.
/// </summary>
[Trait(TestConstants.TestTrait.Category, "Memory")]
public partial class UsingMemoryStreamSlim : UsingMemoryStreamSlimUnitTestBase
{
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Returns a random byte array of the specified length.
    /// </summary>
    /// <param name="length"></param>
    /// <returns>
    /// A byte array filled with random bytes of the specified length to be used as a source buffer
    /// to create a new <see cref="MemoryStreamSlim"/> fixed mode instance.
    /// </returns>
    private static byte[] GetSourceBuffer (int length)
    {
        byte[] buffer = new byte[length];
        GetRandomBytes(SecureRandomSource, buffer, length);
        return buffer;
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Returns a random byte array with a length between the specified minimum and maximum.
    /// </summary>
    /// <param name="maxLength">
    /// The maximum length of the byte array to return
    /// </param>
    /// <param name="minLength">
    /// The minimum length of the byte array to return
    /// </param>
    /// <returns>
    /// A byte array filled with random bytes of length within the specified values to be used as a source buffer
    /// to create a new <see cref="MemoryStreamSlim"/> fixed mode instance.
    /// </returns>
    private static byte[] GetSourceBuffer (int minLength, int maxLength) =>
        GetSourceBuffer(GetTestInteger(SecureRandomSource, minLength, maxLength));
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Initializes a new instance of the <see cref="UsingMemoryStreamSlim"/> class.
    /// </summary>
    /// <param name="xUnitTestOutputHelper">
    /// The Xunit test output helper that can be used to output test messages
    /// </param>
    public UsingMemoryStreamSlim (ITestOutputHelper xUnitTestOutputHelper) : base(xUnitTestOutputHelper)
    {
        CreateTestService = streamSize =>
        {
            byte[] sourceBuffer = GetSourceBuffer(streamSize);
            return MemoryStreamSlim.Create(sourceBuffer);
        };
    }
    //--------------------------------------------------------------------------------

    #region Test Methods

    //================================================================================

    //--------------------------------------------------------------------------------    
    /// <summary>
    /// Tests creating a MemoryStreamSlim instance with default settings and verifying
    /// the capacity and zero buffer behavior are as expected.
    /// </summary>
    [Fact]
    public void UsingMemoryStreamSlim_CreateDefault_HasExpectedSettingsAndCapacity ()
    {
        byte[] sourceBuffer = GetSourceBuffer(20);
        using MemoryStreamSlim stream = MemoryStreamSlim.Create(sourceBuffer);

        stream.Capacity.Should().Be(sourceBuffer.Length);
        stream.Settings.ZeroBufferBehavior.Should().Be(MemoryStreamSlimZeroBufferOption.OnRelease);
    }
    //--------------------------------------------------------------------------------    
    /// <summary>
    /// Tests creating a MemoryStreamSlim instance with a specific capacity and verifying
    /// the capacity and zero buffer behavior are as expected.
    /// </summary>
    [Fact]
    public void UsingMemoryStreamSlim_CreateWithCapacity_HasExpectedSettingsAndCapacity ()
    {
        byte[] sourceBuffer = GetSourceBuffer(100, 1000);
        using MemoryStreamSlim stream = MemoryStreamSlim.Create(sourceBuffer);

        stream.Capacity.Should().Be(sourceBuffer.Length);
        stream.Settings.ZeroBufferBehavior.Should().Be(MemoryStreamSlimZeroBufferOption.OnRelease);
    }
    //--------------------------------------------------------------------------------    
    /// <summary>
    /// Tests creating a MemoryStreamSlim instance with default settings and verifying
    /// the capacity (long version) and zero buffer behavior are as expected.
    /// </summary>
    [Fact]
    public void UsingMemoryStreamSlim_CreateDefault_HasExpectedSettingsAndCapacityLong ()
    {
        byte[] sourceBuffer = GetSourceBuffer(20);
        using MemoryStreamSlim stream = MemoryStreamSlim.Create(sourceBuffer);

        stream.CapacityLong.Should().Be(sourceBuffer.Length);
        stream.Settings.ZeroBufferBehavior.Should().Be(MemoryStreamSlimZeroBufferOption.OnRelease);
    }
    //--------------------------------------------------------------------------------    
    /// <summary>
    /// Tests creating a MemoryStreamSlim instance with a specific capacity and verifying
    /// the capacity (long version) and zero buffer behavior are as expected.
    /// </summary>
    [Fact]
    public void UsingMemoryStreamSlim_CreateWithCapacity_HasExpectedSettingsAndCapacityLong ()
    {
        byte[] sourceBuffer = GetSourceBuffer(100, 1000);
        using MemoryStreamSlim stream = MemoryStreamSlim.Create(sourceBuffer);

        stream.CapacityLong.Should().Be(sourceBuffer.Length);
        stream.Settings.ZeroBufferBehavior.Should().Be(MemoryStreamSlimZeroBufferOption.OnRelease);
    }
    //--------------------------------------------------------------------------------    
    /// <summary>
    /// Tests creating an instance MemoryStreamSlim and verifying the initial property values.
    /// </summary>
    [Fact]
    public void UsingMemoryStreamSlim_ExpandableStream_HasValidInitialPropertyValues ()
    {
        byte[] sourceBuffer = GetSourceBuffer(100, 1000);
        using MemoryStreamSlim stream = MemoryStreamSlim.Create(sourceBuffer);

        stream.CanRead.Should().BeTrue();
        stream.CanSeek.Should().BeTrue();
        stream.CanWrite.Should().BeTrue();
        stream.CanTimeout.Should().BeFalse();
        stream.Length.Should().Be(sourceBuffer.Length);
        stream.Position.Should().Be(0);

        stream.Invoking(s => s.ReadTimeout).Should().Throw<InvalidOperationException>();
        stream.Invoking(s => s.WriteTimeout).Should().Throw<InvalidOperationException>();
    }
    //--------------------------------------------------------------------------------    
    /// <summary>
    /// Tests creating a fixed mode MemoryStreamSlim instance and verifying that attempting
    /// to set the Capacity property throws an exception.
    /// </summary>
    [Fact]
    public void UsingMemoryStreamSlim_SetCapacity_ThrowsException ()
    {
        byte[] sourceBuffer = GetSourceBuffer(100, 1000);
        using MemoryStreamSlim testService = MemoryStreamSlim.Create(sourceBuffer);

        testService.Invoking(s => s.Capacity = sourceBuffer.Length + 1).Should()
            .Throw<NotSupportedException>()
            .WithMessage("Memory stream is not expandable.");
    }
    //--------------------------------------------------------------------------------    
    /// <summary>
    /// Tests creating a fixed mode MemoryStreamSlim instance and verifying that attempting
    /// to set the Capacity property to a negative value throws an exception.
    /// </summary>
    [Fact]
    public void UsingMemoryStreamSlim_SetNegativeCapacity_ThrowsException ()
    {
        byte[] sourceBuffer = GetSourceBuffer(100, 1000);
        using MemoryStreamSlim testService = MemoryStreamSlim.Create(sourceBuffer);

        testService.Invoking(s => s.Capacity = -1).Should()
            .Throw<ArgumentOutOfRangeException>()
            .WithMessage("Capacity must be greater than or equal to zero. (Parameter 'Capacity')");
    }
    //--------------------------------------------------------------------------------    
    /// <summary>
    /// Tests creating a fixed mode MemoryStreamSlim instance and verifying that attempting
    /// to set the Capacity (Long version) property throws an exception.
    /// </summary>
    [Fact]
    public void UsingMemoryStreamSlim_SetCapacityLong_ThrowsException ()
    {
        byte[] sourceBuffer = GetSourceBuffer(100, 1000);
        using MemoryStreamSlim testService = MemoryStreamSlim.Create(sourceBuffer);

        testService.Invoking(s => s.CapacityLong = sourceBuffer.Length + 1).Should()
            .Throw<NotSupportedException>()
            .WithMessage("Memory stream is not expandable.");
    }
    //--------------------------------------------------------------------------------    
    /// <summary>
    /// Tests creating a fixed mode MemoryStreamSlim instance and verifying that attempting
    /// to set the Capacity (Long version) property to a negative value throws an exception.
    /// </summary>
    [Fact]
    public void UsingMemoryStreamSlim_SetNegativeCapacityLong_ThrowsException ()
    {
        byte[] sourceBuffer = GetSourceBuffer(100, 1000);
        using MemoryStreamSlim testService = MemoryStreamSlim.Create(sourceBuffer);

        testService.Invoking(s => s.CapacityLong = -1).Should()
            .Throw<ArgumentOutOfRangeException>()
            .WithMessage("Capacity must be greater than or equal to zero. (Parameter 'Capacity')");
    }
    //--------------------------------------------------------------------------------    
    /// <summary>
    /// Tests creating a fixed mode MemoryStreamSlim instance and verifying that attempting
    /// to set the Length property throws an exception.
    /// </summary>
    [Fact]
    public void UsingMemoryStreamSlim_SetLength_ThrowsException ()
    {
        byte[] sourceBuffer = GetSourceBuffer(100, 1000);
        using MemoryStreamSlim testService = MemoryStreamSlim.Create(sourceBuffer);

        testService.Invoking(s => s.SetLength(sourceBuffer.Length + 1)).Should()
            .Throw<NotSupportedException>()
            .WithMessage("Memory stream is not expandable.");
    }
    //--------------------------------------------------------------------------------    
    /// <summary>
    /// Tests setting the Length property of a <see cref="MemoryStreamSlim"/> instance
    /// to a value below zero, which should throw an exception.
    /// </summary>
    [Fact]
    public void UsingMemoryStreamSlim_SetLengthBelowZero_ThrowsException ()
    {
        byte[] sourceBuffer = GetSourceBuffer(100, 1000);
        using MemoryStreamSlim testService = MemoryStreamSlim.Create(sourceBuffer);

        testService.Invoking(s => s.SetLength(-1)).Should()
            .Throw<ArgumentOutOfRangeException>()
            .WithMessage("Stream length must be non-negative and less than 2^31 - 1 - origin. (Parameter 'value')");
    }
    //--------------------------------------------------------------------------------    
    /// <summary>
    /// Tests creating an instance MemoryStreamSlim as a fixed sized stream and verifying 
    /// that calling GetBuffer throws an exception.
    /// </summary>
    [Fact]
    public void UsingMemoryStreamSlim_GetBuffer_ThrowsException ()
    {
        byte[] sourceBuffer = GetSourceBuffer(100, 1000);
        using MemoryStreamSlim testService = MemoryStreamSlim.Create(sourceBuffer);

        // Fill the stream with random bytes
        MemoryTestPrep.FillStreamAndArrayWithRandomBytes(testService, sourceBuffer.Length, 0x10000);
        testService.Invoking(s => s.GetBuffer())
            .Should()
            .Throw<UnauthorizedAccessException>()
            .WithMessage("MemoryStream's internal buffer cannot be accessed.");
    }
    //--------------------------------------------------------------------------------    
    /// <summary>
    /// Tests creating an instance MemoryStreamSlim as a fixed sized stream with a buffer index
    /// and length then verifying that calling GetBuffer throws an exception.
    /// </summary>
    [Fact]
    public void UsingMemoryStreamSlim_CreateWithBufferLength_GetBuffer_ThrowsException ()
    {
        byte[] sourceBuffer = GetSourceBuffer(100, 1000);
        using MemoryStreamSlim testService = MemoryStreamSlim.Create(sourceBuffer, 0, sourceBuffer.Length);

        // Fill the stream with random bytes
        MemoryTestPrep.FillStreamAndArrayWithRandomBytes(testService, sourceBuffer.Length, 0x10000);
        testService.Invoking(s => s.GetBuffer())
            .Should()
            .Throw<UnauthorizedAccessException>()
            .WithMessage("MemoryStream's internal buffer cannot be accessed.");
    }
    //--------------------------------------------------------------------------------    
    /// <summary>
    /// Tests creating an instance MemoryStreamSlim as a fixed sized stream that is writable 
    /// and verifying that calling GetBuffer throws an exception.
    /// </summary>
    [Fact]
    public void UsingMemoryStreamSlim_CreateWritableWithBufferLength_GetBuffer_ThrowsException ()
    {
        byte[] sourceBuffer = GetSourceBuffer(100, 1000);
        using MemoryStreamSlim testService = MemoryStreamSlim.Create(sourceBuffer, 0, sourceBuffer.Length, true);

        // Fill the stream with random bytes
        MemoryTestPrep.FillStreamAndArrayWithRandomBytes(testService, sourceBuffer.Length, 0x10000);
        testService.Invoking(s => s.GetBuffer())
            .Should()
            .Throw<UnauthorizedAccessException>()
            .WithMessage("MemoryStream's internal buffer cannot be accessed.");
    }
    //--------------------------------------------------------------------------------    
    /// <summary>
    /// Tests creating an instance MemoryStreamSlim as a fixed sized stream that is
    /// publicly visible and verifying that calling GetBuffer throws an exception.
    /// </summary>
    [Fact]
    public void UsingMemoryStreamSlim_CreatePubliclyVisibleWithBufferLength_GetBuffer_ReturnsBuffer ()
    {
        byte[] sourceBuffer = GetSourceBuffer(100, 1000);
        using MemoryStreamSlim testService = MemoryStreamSlim.Create(sourceBuffer, 0, sourceBuffer.Length, true, true);

        // Fill the stream with random bytes
        MemoryTestPrep.FillStreamAndArrayWithRandomBytes(testService, sourceBuffer.Length, 0x10000);
        testService.GetBuffer().Should().BeSameAs(sourceBuffer);
    }
    //--------------------------------------------------------------------------------    
    /// <summary>
    /// Tests creating an instance MemoryStreamSlim as a fixed sized stream and verifying 
    /// that calling TryGetBuffer returns false.
    /// </summary>
    [Fact]
    public void UsingMemoryStreamSlim_TryGetBuffer_ReturnsFalse ()
    {
        byte[] sourceBuffer = GetSourceBuffer(100, 1000);
        using MemoryStreamSlim testService = MemoryStreamSlim.Create(sourceBuffer);

        // Fill the stream with random bytes
        MemoryTestPrep.FillStreamAndArrayWithRandomBytes(testService, sourceBuffer.Length, 0x10000);
        testService.TryGetBuffer(out _).Should().BeFalse();
    }
    //--------------------------------------------------------------------------------    
    /// <summary>
    /// Tests creating an instance MemoryStreamSlim as a fixed sized stream and specifying
    /// the buffer index and length, then verifying that calling TryGetBuffer returns false.
    /// </summary>
    [Fact]
    public void UsingMemoryStreamSlim_CreateWithBufferLength_TryGetBuffer_ReturnsFalse ()
    {
        byte[] sourceBuffer = GetSourceBuffer(100, 1000);
        using MemoryStreamSlim testService = MemoryStreamSlim.Create(sourceBuffer, 0, sourceBuffer.Length);

        // Fill the stream with random bytes
        MemoryTestPrep.FillStreamAndArrayWithRandomBytes(testService, sourceBuffer.Length, 0x10000);
        testService.TryGetBuffer(out _).Should().BeFalse();
    }
    //--------------------------------------------------------------------------------    
    /// <summary>
    /// Tests creating an instance MemoryStreamSlim as a fixed sized stream that is writeable 
    /// and verifying that calling TryGetBuffer returns false.
    /// </summary>
    [Fact]
    public void UsingMemoryStreamSlim_CreateWritableWithBufferLength_TryGetBuffer_ReturnsFalse ()
    {
        byte[] sourceBuffer = GetSourceBuffer(100, 1000);
        using MemoryStreamSlim testService = MemoryStreamSlim.Create(sourceBuffer, 0, sourceBuffer.Length, true);

        // Fill the stream with random bytes
        MemoryTestPrep.FillStreamAndArrayWithRandomBytes(testService, sourceBuffer.Length, 0x10000);
        testService.TryGetBuffer(out _).Should().BeFalse();
    }
    //--------------------------------------------------------------------------------    
    /// <summary>
    /// Tests creating an instance MemoryStreamSlim as a fixed sized stream that is
    /// publicly visible and verifying that calling TryGetBuffer returns false.
    /// </summary>
    [Fact]
    public void UsingMemoryStreamSlim_CreatePubliclyVisibleWithBufferLength_TryGetBuffer_ReturnsBuffer ()
    {
        byte[] sourceBuffer = GetSourceBuffer(100, 1000);
        using MemoryStreamSlim testService = MemoryStreamSlim.Create(sourceBuffer, 0, sourceBuffer.Length, true, true);

        // Fill the stream with random bytes
        MemoryTestPrep.FillStreamAndArrayWithRandomBytes(testService, sourceBuffer.Length, 0x10000);
        testService.TryGetBuffer(out ArraySegment<byte> buffer).Should().BeTrue();
        buffer.Array.Should().BeSameAs(sourceBuffer);
    }
    //--------------------------------------------------------------------------------    

    //================================================================================

    #endregion Test Methods
}
//################################################################################
