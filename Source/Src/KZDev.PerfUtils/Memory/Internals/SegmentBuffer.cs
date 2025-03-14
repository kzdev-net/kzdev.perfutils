// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace KZDev.PerfUtils.Internals;

//################################################################################
/// <summary>
/// A buffer of bytes that references either a segment of a larger buffer or a 
/// standalone buffer.
/// </summary>
[DebuggerDisplay($"{{{nameof(DebugDisplayValue)},nq}}")]
internal readonly struct SegmentBuffer
{
    /// <summary>
    /// Debug helper to display the state of this object.
    /// </summary>
    [ExcludeFromCodeCoverage]
#pragma warning disable HAA0601
    internal string DebugDisplayValue => IsEmpty && (BufferInfo.BlockId < 0) ? "Empty" : $"{BufferInfo.DebugDisplayValue}, Length {Length}, {(IsRaw ? "Raw" : "NotRaw")}, Segment: {MemorySegment.DebugDisplayValue}";
#pragma warning restore HAA0601

    /// <summary>
    /// The raw buffer that this segment references, if any.
    /// </summary>
    public byte[]? RawBuffer { [DebuggerStepThrough] get; }

    /// <summary>
    /// The array segment that this segment buffer references.
    /// </summary>
    public MemorySegment MemorySegment { [DebuggerStepThrough] get; }

    /// <summary>
    /// The memory that this segment references as a span.
    /// </summary>
    public Span<byte> Span
    {
        [DebuggerStepThrough]
        get => MemorySegment.AsSpan();
    }

    /// <summary>
    /// Metadata about this segment buffer
    /// </summary>
    public SegmentBufferInfo BufferInfo { [DebuggerStepThrough] get; }

    /// <summary>
    /// The number of standard sized segments that this buffer holds. If this is a 
    /// raw buffer, then this will return 0;
    /// </summary>
    public int SegmentCount { [DebuggerStepThrough] get => BufferInfo.SegmentCount; }

    /// <summary>
    /// Gets a value indicating whether the segment is empty.
    /// </summary>
    public bool IsEmpty
    {
        [DebuggerStepThrough]
        get => MemorySegment.Count == 0;
    }

    /// <summary>
    /// Gets the length of the segment.
    /// </summary>
    public int Length
    {
        [DebuggerStepThrough]
        get => MemorySegment.Count;
    }

    /// <summary>
    /// Gets a value indicating whether this segment references a raw buffer.
    /// </summary>
    public bool IsRaw
    {
        [DebuggerStepThrough]
        get => RawBuffer is not null;
    }

    //--------------------------------------------------------------------------------
    /// <summary>
    /// Returns an empty or default instance of the <see cref="SegmentBuffer"/> struct.
    /// </summary>
    public static SegmentBuffer Empty { [DebuggerStepThrough] get; } = new();
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Initializes a new instance of the <see cref="SegmentBuffer"/> struct with a 
    /// source <see cref="ArraySegment{Byte}"/> instance.
    /// </summary>
    /// <param name="memorySegment">
    /// The source memory segment instance.
    /// </param>
    public SegmentBuffer (in MemorySegment memorySegment)
    {
        Debug.Assert(memorySegment.Count > 0, "memorySegment.Count <= 0");

        MemorySegment = memorySegment;
        BufferInfo = SegmentBufferInfo.NoBlock;
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Initializes a new instance of the <see cref="SegmentBuffer"/> struct with a 
    /// raw byte buffer.
    /// </summary>
    /// <param name="buffer">
    /// The raw byte buffer.
    /// </param>
    public SegmentBuffer (byte[] buffer)
    {
        Debug.Assert(buffer is not null, "buffer is null");
        Debug.Assert(buffer.Length > 0, "buffer.Length <= 0");

        RawBuffer = buffer;
        MemorySegment = new MemorySegment(buffer);
        BufferInfo = SegmentBufferInfo.NoBlock;
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Initializes a new instance of the <see cref="SegmentBuffer"/> struct with a 
    /// source <see cref="ArraySegment{Byte}"/> instance.
    /// </summary>
    /// <param name="memorySegment">
    /// The source memory segment instance.
    /// </param>
    /// <param name="bufferInfo">
    /// The meta-data information about this buffer segment.
    /// </param>
    public SegmentBuffer (in MemorySegment memorySegment, in SegmentBufferInfo bufferInfo)
    {
        Debug.Assert(memorySegment.Count > 0, "memorySegment.Count <= 0");
        Debug.Assert(bufferInfo.SegmentCount > 0, "bufferInfo.SegmentCount <= 0");

        MemorySegment = memorySegment;
        BufferInfo = bufferInfo;
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Merges this segment with another segment which is assumed to follow this segment
    /// in the internal allocated buffer.
    /// </summary>
    /// <param name="nextBuffer">
    /// The next segment to merge with this segment.
    /// </param>
    /// <returns>
    /// A new <see cref="SegmentBuffer"/> instance that represents the merged segments.
    /// </returns>
    public SegmentBuffer Concat (in SegmentBuffer nextBuffer)
    {
        Debug.Assert(!IsRaw, "IsRaw");
        Debug.Assert(!nextBuffer.IsRaw, "other.IsRaw");
        Debug.Assert(BufferInfo.SegmentId + BufferInfo.SegmentCount == nextBuffer.BufferInfo.SegmentId, "next buffer doesn't follow this buffer in the allocated memory");
        Debug.Assert(BufferInfo.BlockId == nextBuffer.BufferInfo.BlockId, "next buffer is from a different block");

        // Get the new memory segment that is the combination of the two segments and create the new segment buffer
        return new SegmentBuffer(MemorySegment.Extend(nextBuffer.Length), BufferInfo.Concat(nextBuffer.BufferInfo));
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Clears the contents of this segment.
    /// </summary>
    public void Clear ()
    {
        Span.Clear();
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Copies the contents of this <see cref="SegmentBuffer"/> object into a destination SegmentBuffer object.
    /// </summary>
    /// <param name="destination">
    /// The destination <see cref="SegmentBuffer"/> object.
    /// </param>
    public void CopyTo (in SegmentBuffer destination) => CopyTo(destination.MemorySegment);
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Copies the contents of this <see cref="SegmentBuffer"/> object into a destination byte[] object.
    /// </summary>
    /// <param name="destination">
    /// The destination byte[] object.
    /// </param>
    public void CopyTo (byte[] destination) => MemorySegment.CopyTo(destination);
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Copies the contents of this <see cref="SegmentBuffer"/> object into a destination <see cref="Memory{Byte}"/> object.
    /// </summary>
    /// <param name="destination">
    /// The destination <see cref="ArraySegment{Byte}"/> object.
    /// </param>
    public void CopyTo (in MemorySegment destination) => MemorySegment.CopyTo(destination);
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Copies the contents of this <see cref="SegmentBuffer"/> object into a destination <see cref="Span{Byte}"/> object.
    /// </summary>
    /// <param name="destination">
    /// The destination <see cref="Span{Byte}"/> object.
    /// </param>
    public void CopyTo (in Span<byte> destination) => Span.CopyTo(destination);
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Creates a new span over a portion of this <see cref="SegmentBuffer"/> from a specified
    /// position for a specified length.
    /// </summary>
    /// <param name="start">
    /// The index at which to begin the span.
    /// </param>
    /// <param name="length">
    /// The number of items in the span.
    /// </param>
    /// <returns>
    /// The <see cref="Span{Byte}"/> representation of the segment.
    /// </returns>
    public Span<byte> AsSpan (int start, int length) => MemorySegment.Slice(start, length);
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Forms a slice out of this <see cref="SegmentBuffer"/> that begins at a specified index.
    /// </summary>
    /// <param name="start">
    /// The index at which to begin the slice.
    /// </param>
    /// <returns>
    /// An instance that contains all bytes of this instance from <paramref name="start"/>
    /// to the end of the instance.
    /// </returns>
    public SegmentBuffer Slice (int start) => MemorySegment[start..];
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Forms a slice out of this <see cref="SegmentBuffer"/> that begins at a specified index.
    /// </summary>
    /// <param name="start">
    /// The index at which to begin the slice.
    /// </param>
    /// <param name="length">
    /// The number of bytes to include in the slice.
    /// </param>
    /// <returns>
    /// An instance that contains <paramref name="length"/> bytes from the current 
    /// instance starting at <paramref name="start"/>.
    /// </returns>
    public SegmentBuffer Slice (int start, int length) => MemorySegment.Slice(start, length);
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Splits the segment into two segments at a specified segment index.
    /// </summary>
    /// <param name="firstSegmentCount">
    /// The number of buffer segments that should be in the first segment.
    /// </param>
    /// <returns>
    /// Two new <see cref="SegmentBuffer"/> instances that represent the split.
    /// </returns>
    public (SegmentBuffer firstSegment, SegmentBuffer secondSegment) Split (int firstSegmentCount)
    {
        Debug.Assert(firstSegmentCount < SegmentCount, "firstSegmentCount >= SegmentCount");
        Debug.Assert(!IsRaw, "IsRaw");

        // Get the metadata for the two new segments
        SegmentBufferInfo firstSegmentInfo = new SegmentBufferInfo(BufferInfo.BlockId, BufferInfo.SegmentId, firstSegmentCount, BufferInfo.BufferPool);
        SegmentBufferInfo secondSegmentInfo = new SegmentBufferInfo(BufferInfo.BlockId, BufferInfo.SegmentId + firstSegmentCount,
            SegmentCount - firstSegmentCount, BufferInfo.BufferPool);
        // Determine the byte length of the first segment
        int firstSegmentLength = firstSegmentCount * MemorySegmentedBufferGroup.StandardBufferSegmentSize;
        // Create the two new segments
        SegmentBuffer firstSegmentBuffer = new SegmentBuffer(MemorySegment[..firstSegmentLength], firstSegmentInfo);
        SegmentBuffer secondSegmentBuffer = new SegmentBuffer(MemorySegment[firstSegmentLength..], secondSegmentInfo);
        return (firstSegmentBuffer, secondSegmentBuffer);
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Gets the byte at the specified index.
    /// </summary>
    /// <param name="index">
    /// The index of the byte to get.
    /// </param>
    /// <returns>
    /// The byte value at the specified index.
    /// </returns>
    public byte this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => MemorySegment[index];
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => Span[index] = value;
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Defines an implicit conversion of <see cref="ArraySegment{Byte}"/> to a <see cref="SegmentBuffer"/>
    /// </summary>
    public static implicit operator SegmentBuffer (in MemorySegment arraySegment) => new(arraySegment);
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Defines an implicit conversion of byte[] to a <see cref="SegmentBuffer"/>
    /// </summary>
    public static implicit operator SegmentBuffer (byte[] buffer) => new(buffer);
    //--------------------------------------------------------------------------------
}
//################################################################################
