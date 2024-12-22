// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

using KZDev.PerfUtils.Helpers;
using KZDev.PerfUtils.Resources;

namespace KZDev.PerfUtils.Internals
{
    //################################################################################
    /// <summary>
    /// Represents a segment of memory that is either a managed or unmanaged byte 
    /// array buffer.
    /// </summary>
    [DebuggerDisplay($"{{{nameof(DebugDisplayValue)}}}")]
    internal readonly unsafe struct MemorySegment
    {
        /// <summary>
        /// Debug helper to display the state of this object.
        /// </summary>
        [ExcludeFromCodeCoverage]
#pragma warning disable HAA0601
        internal string DebugDisplayValue => $"Length {_count}, Offset {_offset}, {(IsNative ? "Native" : "Heap")}";
#pragma warning restore HAA0601

        /// <summary>
        /// This points to the native memory if the segment is a native memory segment.
        /// </summary>
        private readonly byte* _nativePointer;

        /// <summary>
        /// References the managed array if the segment is a managed array segment.
        /// </summary>
        private readonly byte[]? _managedArray;

        /// <summary>
        /// The offset of the segment into the memory buffer.
        /// </summary>
        private readonly int _offset;

        /// <summary>
        /// The number of bytes in the segment.
        /// </summary>
        private readonly int _count;

        //--------------------------------------------------------------------------------
        /// <summary>
        /// Gets the native memory manager for the segment, if any.
        /// </summary>
        /// <remarks>
        /// This is such a rarely used feature that we can create a new instance when
        /// needed here, and the memory manager will be disposed of when it goes out of scope.
        /// It is used internally by the <see cref="MemorySegment"/> to get access to a 
        /// Memory{byte} instance for the native memory block which is implicitly cast to
        /// a ReadOnlyMemory{byte} instance for the AsReadOnlyMemory() method. That 
        /// structure holds the reference to the memory manager for its use and will
        /// release it when it goes out of scope.
        /// </remarks>
        private NativeMemoryManager NativeMemoryManager
        {
            get
            {
                Debug.Assert(_nativePointer is not null, "The memory segment is not a native memory segment");
                // We can get a new instance when requested here because the memory manager 
                return NativeMemoryManager.GetManager(_nativePointer + _offset, _count);
            }
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Initializes a new instance of the <see cref="MemorySegment"/> struct.
        /// </summary>
        /// <param name="array">
        /// The managed array that the segment references.
        /// </param>
        public MemorySegment (byte[] array)
        {
            ArgumentNullException.ThrowIfNull(array);

            _managedArray = array;
            _offset = 0;
            _count = array.Length;
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Initializes a new instance of the <see cref="MemorySegment"/> struct.
        /// </summary>
        /// <param name="array">
        /// The managed array that the segment references.
        /// </param>
        /// <param name="offset">
        /// The offset into the array where the segment starts.
        /// </param>
        /// <param name="count">
        /// The number of bytes in the segment.
        /// </param>
        public MemorySegment (byte[] array, int offset, int count)
        {
            ArgumentNullException.ThrowIfNull(array);
            if (offset < 0)
                ThrowHelper.ThrowArgumentOutOfRangeException_NeedNonNegNum(nameof(offset));
            if (count < 0)
                ThrowHelper.ThrowArgumentOutOfRangeException_NeedNonNegNum(nameof(count));
            if (((uint)offset > (uint)array.Length) || ((uint)count > (uint)(array.Length - offset)))
                ThrowHelper.ThrowArgumentException_InvalidOffsetLength();

            _managedArray = array;
            _offset = offset;
            _count = count;
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Initializes a new instance of the <see cref="MemorySegment"/> struct.
        /// </summary>
        /// <param name="bufferBlock">
        /// The native memory buffer block that the segment references.
        /// </param>
        /// <param name="offset">
        /// The offset into the array where the segment starts.
        /// </param>
        /// <param name="count">
        /// The number of bytes in the segment.
        /// </param>
        public MemorySegment (byte* bufferBlock, int offset, int count)
        {
            if (offset < 0)
                ThrowHelper.ThrowArgumentOutOfRangeException_NeedNonNegNum(nameof(offset));
            if (count < 0)
                ThrowHelper.ThrowArgumentOutOfRangeException_NeedNonNegNum(nameof(count));

            _nativePointer = bufferBlock;
            _offset = offset;
            _count = count;
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Returns a new memory segment that has a count that is the sum of this segment 
        /// count and the specified add length.
        /// </summary>
        /// <param name="addLength">
        /// The number of segments to extend the current segment by.
        /// </param>
        /// <returns>
        /// A new memory segment that represents the segment from the same address as this segment
        /// but with an extended length.
        /// </returns>
        public MemorySegment Extend (int addLength) =>
            IsNative ?
                // Native memory...
                // We create a new memory segment that is the combination of the previous buffer
                // and the new buffer.
                new MemorySegment(NativePointer, Offset, Count + addLength) :
                // We create a new memory segment that is the combination of the previous buffer
                // and the new buffer.
                new MemorySegment(Array, Offset, Count + addLength);
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Merges this memory segment with the specified segment which is assumed to be
        /// contiguous to this segment.
        /// </summary>
        /// <param name="nextSegment">
        /// The segment to merge with this segment.
        /// </param>
        /// <returns>
        /// A new memory segment that represents the concatenation of this segment and the next segment.
        /// </returns>
        public MemorySegment Concat (in MemorySegment nextSegment)
        {
            // TODO - Need unit tests for this.
            Debug.Assert(nextSegment.Offset == Offset + Count, "The next segment is not contiguous with this segment");
            return IsNative ?
                // Native memory...
                // We create a new memory segment that is the combination of the previous buffer
                // and the new buffer.
                new MemorySegment (NativePointer, Offset, Count + nextSegment.Count) :
                // We create a new memory segment that is the combination of the previous buffer
                // and the new buffer.
                new MemorySegment (Array, Offset, Count + nextSegment.Count);
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the byte at the specified index.
        /// </summary>
        /// <param name="index">
        /// The zero-based index of the byte to get or set.
        /// </param>
        public byte this[int index]
        {
            get
            {
                if ((uint)index >= (uint)_count)
                {
                    ThrowHelper.ThrowArgumentOutOfRange_IndexMustBeLessException(nameof(index));
                }

                return IsNative ? _nativePointer[_offset + index] : _managedArray![_offset + index];
            }
            set
            {
                if ((uint)index >= (uint)_count)
                {
                    ThrowHelper.ThrowArgumentOutOfRange_IndexMustBeLessException(nameof(index));
                }

                if (IsNative)
                    _nativePointer[_offset + index] = value;
                else
                    _managedArray![_offset + index] = value;
            }
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Returns a value indicating whether the segment is a native memory segment.
        /// </summary>
        public bool IsNative
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (_nativePointer is not null);
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Gets the native memory pointer that the segment references, if any.
        /// </summary>
        public byte* NativePointer => _nativePointer;
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Gets the managed array that the segment references, if any.
        /// </summary>
        public byte[] Array => _managedArray ?? throw new InvalidOperationException(Strings.InvalidOperation_MemorySegmentIsNative);
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Gets the offset of the segment into the memory buffer.
        /// </summary>
        public int Offset => _offset;
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Gets the number of bytes in the segment.
        /// </summary>
        public int Count => _count;
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Creates a new span over this memory segment.
        /// </summary>
        public Span<byte> AsSpan () => IsNative ?
            new Span<byte>(_nativePointer + _offset, _count) :
            new Span<byte>(_managedArray!, _offset, _count);
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Creates a new span over this memory segment.
        /// </summary>
        public ReadOnlyMemory<byte> AsReadOnlyMemory () => IsNative ?
            NativeMemoryManager.Memory :
            new ReadOnlyMemory<byte>(_managedArray!, _offset, _count);
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Clears the contents of the memory segment (zeroes out the memory).
        /// </summary>
        public void Clear ()
        {
            AsSpan().Clear();
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Copies the contents of this instance into the specified destination byte array.
        /// </summary>
        /// <param name="destination">
        /// The byte array into which the contents of this instance will be copied.
        /// </param>
        public void CopyTo (byte[] destination) => CopyTo(destination, 0);
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Copies the contents of this instance into the specified destination byte array, 
        /// starting at the specified destination index.
        /// </summary>
        /// <param name="destination">
        /// The byte array into which the contents of this instance will be copied.
        /// </param>
        /// <param name="destinationIndex">
        /// The index in destination at which storing begins.
        /// </param>
        public void CopyTo (byte[] destination, int destinationIndex)
        {
            if (IsNative)
            {
                // TODO - Need unit tests for this.
                fixed (byte* pWriteBuffer = destination)
                {
                    Buffer.MemoryCopy(_nativePointer + _offset, pWriteBuffer + destinationIndex, destination.Length - destinationIndex, _count);
                }
                return;
            }
            System.Array.Copy(_managedArray!, _offset, destination, destinationIndex, _count);
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Copies the contents of this instance into the specified destination segment.
        /// </summary>
        /// <param name="destination">
        /// The memory segment into which the contents of this instance will be copied.
        /// </param>
        public void CopyTo (MemorySegment destination)
        {
            if (_count > destination._count)
            {
                ThrowHelper.ThrowArgumentException_DestinationTooShort(nameof(destination));
            }

            if (IsNative)
            {
                // TODO - Need unit tests for this.
                if (destination.IsNative)
                {
                    Buffer.MemoryCopy(_nativePointer + _offset, destination._nativePointer + destination._offset, destination._count, _count);
                    return;
                }
                fixed (byte* pWriteBuffer = destination._managedArray)
                {
                    Buffer.MemoryCopy(_nativePointer + _offset, pWriteBuffer, destination._count, _count);
                }
                return;
            }
            if (destination.IsNative)
            {
                fixed (byte* pWriteBuffer = _managedArray)
                {
                    Buffer.MemoryCopy(pWriteBuffer, destination._nativePointer + destination._offset, destination._count, _count);
                }
                return;
            }
            System.Array.Copy(_managedArray!, _offset, destination._managedArray!, destination._offset, _count);
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Forms a slice out of the current memory segment starting at the specified index.
        /// </summary>
        /// <param name="index">
        /// The index at which to begin the slice.
        /// </param>
        /// <returns>
        /// A memory segment that consists of all bytes of the current memory segment from
        /// index to the end of the segment.
        /// </returns>
        public MemorySegment Slice (int index)
        {
            // TODO - Need unit tests for this.
            if ((uint)index > (uint)_count)
            {
                ThrowHelper.ThrowArgumentOutOfRange_IndexMustBeLessOrEqualException(nameof(index));
            }

            return new MemorySegment(_managedArray!, _offset + index, _count - index);
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Forms a slice of the specified length out of the current memory 
        /// segment starting at the specified index.
        /// </summary>
        /// <param name="index">
        /// The index at which to begin the slice.
        /// </param>
        /// <param name="count">
        /// The desired length of the slice.
        /// </param>
        /// <returns>
        /// A memory segment of count bytes starting at index.
        /// </returns>
        public MemorySegment Slice (int index, int count)
        {
            if ((uint)index > (uint)_count || (uint)count > (uint)(_count - index))
            {
                ThrowHelper.ThrowArgumentOutOfRange_IndexMustBeLessOrEqualException(nameof(index));
            }

            return IsNative ? new MemorySegment(_nativePointer, _offset + index, count) : new MemorySegment(_managedArray!, _offset + index, count);
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Defines an implicit conversion of a <see cref="MemorySegment"/> to a <see cref="Span{Byte}"/>
        /// </summary>
        public static implicit operator Span<byte> (MemorySegment segment) => segment.IsNative ?
            new(segment._nativePointer + segment.Offset, segment.Count) :
            new(segment._managedArray, segment.Offset, segment.Count);
        //--------------------------------------------------------------------------------
    }
    //################################################################################
}
