// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace KZDev.PerfUtils.Internals;

//################################################################################
/// <summary>
/// Information about the needs for allocation based on the requested capacity.
/// </summary>
/// <param name="allocatedCapacity">
/// Total internal allocation capacity needed.
/// </param>
/// <param name="standardBufferSegmentCount">
/// The number of standard size buffer segments needed.
/// </param>
/// <param name="smallBufferIndex">
/// The index in the small buffer size array of the small buffer needed.
/// </param>
/// <remarks>
/// This will return information about either the number of standard size buffers needed
/// or the index of the small buffer size needed. If the StandardBufferCount is greater than
/// zero, then the SmallBufferIndex will be -1. If the SmallBufferIndex is greater than or
/// zero, then the StandardBufferCount will be zero.
/// </remarks>
[DebuggerDisplay($"{{{nameof(DebugDisplayValue)},nq}}")]
internal readonly struct StreamAllocationNeedInfo(long allocatedCapacity, int standardBufferSegmentCount, int smallBufferIndex)
{
    /// <summary>
    /// Debug helper to display the state of the group.
    /// </summary>
    [ExcludeFromCodeCoverage]
#pragma warning disable HAA0601
    internal string DebugDisplayValue => $"AllocatedCapacity = {AllocatedCapacity}, StdBuffers = {StandardBufferSegmentCount}, SmallIndex = {SmallBufferIndex}";
#pragma warning restore HAA0601

    /// <summary>
    /// Total internal allocation capacity needed.
    /// </summary>
    public long AllocatedCapacity { [DebuggerStepThrough] get; } = allocatedCapacity;
    /// <summary>
    /// The number of standard size buffer segments needed.
    /// </summary>
    public int StandardBufferSegmentCount { [DebuggerStepThrough] get; } = standardBufferSegmentCount;
    /// <summary>
    /// The index in the small buffer size array of the small buffer needed.
    /// </summary>
    public int SmallBufferIndex { [DebuggerStepThrough] get; } = smallBufferIndex;
}
//################################################################################
