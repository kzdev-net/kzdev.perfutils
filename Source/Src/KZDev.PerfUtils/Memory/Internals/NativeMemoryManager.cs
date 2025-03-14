// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Buffers;
using System.Diagnostics.CodeAnalysis;

namespace KZDev.PerfUtils.Internals;

//################################################################################
/// <summary>
/// A custom memory manager implementation to manage native memory allocations.
/// </summary>
internal unsafe class NativeMemoryManager : MemoryManager<byte>
{
    /// <summary>
    /// The pointer to the native memory block.
    /// </summary>
    private readonly byte* _pointer;
    /// <summary>
    /// The length of the native memory block.
    /// </summary>
    private readonly int _length;

    //--------------------------------------------------------------------------------
    /// <summary>
    /// Initializes a new instance of the <see cref="NativeMemoryManager"/> class.
    /// </summary>
    /// <param name="pointer">
    /// The pointer to the native memory block.
    /// </param>
    /// <param name="length">
    /// The length of the native memory block.
    /// </param>
    private NativeMemoryManager (byte* pointer, int length)
    {
        _pointer = pointer;
        _length = length;
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Initializes a new instance of the <see cref="NativeMemoryManager"/> class.
    /// </summary>
    /// <param name="pointer">
    /// The pointer to the native memory block.
    /// </param>
    /// <param name="length">
    /// The length of the native memory block.
    /// </param>
    public static NativeMemoryManager GetManager (byte* pointer, int length) => new(pointer, length);
    //--------------------------------------------------------------------------------
    /// <inheritdoc />
    public override Span<byte> GetSpan () => new(_pointer, _length);
    //--------------------------------------------------------------------------------
    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public override MemoryHandle Pin (int elementIndex = 0)
    {
        if (elementIndex < 0 || elementIndex >= _length)
            throw new ArgumentOutOfRangeException(nameof(elementIndex));
        // We're not actually pinning the memory - it is native memory.
        return new MemoryHandle(_pointer + elementIndex);
    }
    //--------------------------------------------------------------------------------
    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public override void Unpin ()
    {
        // No action required. We didn't actually pin the memory - it is native memory.
    }
    //--------------------------------------------------------------------------------
    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    protected override void Dispose (bool disposing)
    {
        // No managed resources to release - the native memory freeing is handled elsewhere.
    }
    //--------------------------------------------------------------------------------
}
//################################################################################
