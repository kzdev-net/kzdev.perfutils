// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Reflection;
using System.Runtime.CompilerServices;

using KZDev.PerfUtils.Internals;

namespace KZDev.PerfUtils.Tests;

//################################################################################
/// <summary>
/// Extension methods for the <see cref="MemorySegmentedBufferGroup"/> class
/// specifically for unit testing.
/// </summary>
internal static class MemoryStreamSegmentedBufferGroupTestExtensions
{
    /// <summary>
    /// The high bit mask for a ulong.
    /// </summary>
    public static readonly ulong HighBitMask = MemorySegmentedBufferGroup.HighBitMask;

    /// <summary>
    /// The size of the block flag set/group (contained in a ulong)
    /// </summary>
    public static readonly int BlockFlagSetSize = MemorySegmentedBufferGroup.BlockFlagSetSize;
    //================================================================================
    /// <summary>
    /// Holds the field info for the block used flags field.
    /// </summary>
    private static readonly FieldInfo BlockUsedFlagsField =
        typeof(MemorySegmentedBufferGroup).GetField("_blockUsedFlags", BindingFlags.Instance | BindingFlags.NonPublic)!;
    //================================================================================
    /// <summary>
    /// Holds the field info for the block zeroed flags field.
    /// </summary>
    private static readonly FieldInfo BlockZeroFlagsField =
        typeof(MemorySegmentedBufferGroup).GetField("_blockZeroFlags", BindingFlags.Instance | BindingFlags.NonPublic)!;
    //================================================================================
    /// <summary>
    /// Holds the field info for the segments in use field.
    /// </summary>
    private static readonly FieldInfo SegmentsInUseField =
        typeof(MemorySegmentedBufferGroup).GetField("_segmentsInUse", BindingFlags.Instance | BindingFlags.NonPublic)!;

    //================================================================================
    /// <summary>
    /// Gets the flag index and mask to use for the specified segment index.
    /// </summary>
    /// <param name="segmentIndex">
    /// The index of the segment of the block we want to get the flags for.
    /// </param>
    /// <returns>
    /// The index of the flag set instance and the mask to use for the segment index.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static (int Index, ulong Mask) GetFlagIndexAndMask (int segmentIndex) =>
        (segmentIndex / BlockFlagSetSize, 1UL << (segmentIndex % BlockFlagSetSize));
    //================================================================================
    /// <summary>
    /// Sets (or clears) a flag in the block used flags array.
    /// </summary>
    /// <param name="flags">
    /// The flags block to update.
    /// </param>
    /// <param name="flagIndex">
    /// The index of the first flag to update.
    /// </param>
    /// <param name="flagCount">
    /// The number of flags to update.
    /// </param>
    /// <param name="setFlag">
    /// Indicates if the flag should be set or cleared.
    /// </param>
    /// <returns>
    /// The number of flags that were updated.
    /// </returns>
    private static int SetBitFlags (ulong[] flags, int flagIndex, int flagCount, bool setFlag)
    {
        // For the flag index, get the flag group index and the mask to use for that flag index in the group
        (int flagGroupIndex, ulong flagMask) = GetFlagIndexAndMask(flagIndex);
        int updatedCount = 0;
        // Get the flag group we are working with
        ulong flagGroup = flags[flagGroupIndex];

        // Loop through the flags to update
        while (flagCount-- > 0)
        {
            bool isSet = (flagGroup & flagMask) != 0;
            if (isSet != setFlag)
            {
                updatedCount++;
                flagGroup = setFlag ? flagGroup | flagMask : flagGroup & ~flagMask;
            }

            if (flagMask == HighBitMask)
            {
                // We are moving to the next flag group, so update the flag group we
                // are working with and reset the mask
                flags[flagGroupIndex] = flagGroup;
                if (flagCount == 0)
                {
                    break;
                }
                // Move to the next flag group
                flagGroupIndex++;
                flagGroup = flags[flagGroupIndex];
                flagMask = 1;
            }
            else
            {
                flagMask <<= 1;
            }
        }
        // Do the final update
        flags[flagGroupIndex] = flagGroup;
        return updatedCount;
    }
    //================================================================================
    /// <summary>
    /// Returns a reference to the block used flags array in the buffer group.
    /// </summary>
    /// <param name="group">
    /// The buffer group to get the field from.
    /// </param>
    /// <returns>
    /// The internal flags array in the buffer group.
    /// </returns>
    private static ulong[] GetBlockUsedFlagsArray (MemorySegmentedBufferGroup group)
    {
        return (ulong[])BlockUsedFlagsField.GetValue(group)!;
    }
    //================================================================================
    /// <summary>
    /// Returns a reference to the block zeroed flags array in the buffer group.
    /// </summary>
    /// <param name="group">
    /// The buffer group to get the field from.
    /// </param>
    /// <returns>
    /// The internal flags array in the buffer group.
    /// </returns>
    private static ulong[] GetBlockZeroFlagsArray (MemorySegmentedBufferGroup group)
    {
        return (ulong[])BlockZeroFlagsField.GetValue(group)!;
    }
    //================================================================================
    /// <summary>
    /// Returns the value of the internal segments in use field.
    /// </summary>
    /// <param name="group">
    /// The buffer group to get the field from.
    /// </param>
    /// <returns>
    /// The internal segments in use value in the buffer group.
    /// </returns>
    private static int GetSegmentsInUse (MemorySegmentedBufferGroup group)
    {
        return (int)SegmentsInUseField.GetValue(group)!;
    }
    //================================================================================
    /// <summary>
    /// Updates the value of the internal segments in use field.
    /// </summary>
    /// <param name="group">
    /// The buffer group to get the field from.
    /// </param>
    /// <param name="newValue">
    /// The new value to set.
    /// </param>
    private static void SetSegmentsInUse (MemorySegmentedBufferGroup group, int newValue)
    {
        SegmentsInUseField.SetValue(group, newValue);
    }
    //================================================================================
    /// <summary>
    /// Sets a range of block used flags in the buffer group.
    /// </summary>
    /// <param name="group">
    /// The buffer group to update.
    /// </param>
    /// <param name="firstSegmentIndex">
    /// The first segment index to update.
    /// </param>
    /// <param name="segmentCount">
    /// The number of segments to update.
    /// </param>
    public static int SetSegmentsUsed (this MemorySegmentedBufferGroup group,
        int firstSegmentIndex, int segmentCount)
    {
        int startSegmentsInUse = GetSegmentsInUse(group);
        int updatedCount =
            SetBitFlags(GetBlockUsedFlagsArray(group), firstSegmentIndex, segmentCount, true);
        SetSegmentsInUse(group, startSegmentsInUse + updatedCount);
        return updatedCount;
    }
    //================================================================================
    /// <summary>
    /// Sets all the used flags in the buffer group.
    /// </summary>
    /// <param name="group">
    /// The buffer group to update.
    /// </param>
    public static void SetAllSegmentsUsed (this MemorySegmentedBufferGroup group) =>
        SetSegmentsUsed(group, 0, group.SegmentCount);
    //================================================================================
    /// <summary>
    /// Clears a range of block used flags in the buffer group.
    /// </summary>
    /// <param name="group">
    /// The buffer group to update.
    /// </param>
    /// <param name="firstSegmentIndex">
    /// The first segment index to update.
    /// </param>
    /// <param name="segmentCount">
    /// The number of segments to update.
    /// </param>
    /// <param name="zeroed">
    /// Indicates if the segments are zeroed.
    /// </param>
    public static void SetSegmentsFree (this MemorySegmentedBufferGroup group,
        int firstSegmentIndex, int segmentCount, bool zeroed)
    {
        int startSegmentsInUse = GetSegmentsInUse(group);
        SetBitFlags(GetBlockZeroFlagsArray(group), firstSegmentIndex, segmentCount, zeroed);
        int updatedCount =
            SetBitFlags(GetBlockUsedFlagsArray(group), firstSegmentIndex, segmentCount, false);
        SetSegmentsInUse(group, startSegmentsInUse - updatedCount);
    }
    //================================================================================
    /// <summary>
    /// Clears all the block used flags in the buffer group.
    /// </summary>
    /// <param name="group">
    /// The buffer group to update.
    /// </param>
    /// <param name="zeroed">
    /// Indicates if the segments are zeroed.
    /// </param>
    public static void SetAllSegmentsFree (this MemorySegmentedBufferGroup group, bool zeroed) =>
        SetSegmentsFree(group, 0, group.SegmentCount, zeroed);
    //================================================================================
    /// <summary>
    /// Returns whether all the bytes in the buffer are zeroed.
    /// </summary>
    /// <param name="checkBuffer">
    /// The buffer to check.
    /// </param>
    public static bool IsAllZeroes (this SegmentBuffer checkBuffer)
    {
        for (int checkIndex = 0; checkIndex < checkBuffer.Length; checkIndex++)
        {
            if (checkBuffer[checkIndex] != 0)
                return false;
        }
        return true;
    }
    //================================================================================
}
//################################################################################
