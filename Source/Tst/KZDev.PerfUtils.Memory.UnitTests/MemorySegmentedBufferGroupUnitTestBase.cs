﻿using System.Reflection;

using KZDev.PerfUtils.Internals;

namespace KZDev.PerfUtils.Tests;

//################################################################################
/// <summary>
/// Base class for unit tests that use the 
/// <see cref="MemorySegmentedBufferGroup"/> system under test.
/// </summary>
public abstract class MemorySegmentedBufferGroupUnitTestBase : UnitTestBase
{
    /// <summary>
    /// The size of the block flag set/group (contained in an ulong)
    /// </summary>
    internal const int BlockFlagSetSize = 64;  // 64 bits in an unsigned long

    /// <summary>
    /// The field information for the _segmentCount field in the buffer group.
    /// </summary>
    private static readonly FieldInfo BufferGroupSegmentCountField =
        typeof(MemorySegmentedBufferGroup)
            .GetField("_segmentCount",
                BindingFlags.NonPublic | BindingFlags.Instance)!;

    /// <summary>
    /// The field information for the _segmentsInUse field in the buffer group.
    /// </summary>
    private static readonly FieldInfo BufferGroupSegmentsInUseField =
        typeof(MemorySegmentedBufferGroup)
            .GetField("_segmentsInUse",
                BindingFlags.NonPublic | BindingFlags.Instance)!;

    /// <summary>
    /// The field information for the _blockUsedFlags field in the buffer group.
    /// </summary>
    private static readonly FieldInfo BlockUsedFlagsField =
        typeof(MemorySegmentedBufferGroup)
            .GetField("_blockUsedFlags",
                BindingFlags.NonPublic | BindingFlags.Instance)!;

    /// <summary>
    /// The field information for the _blockZeroFlags field in the buffer group.
    /// </summary>
    private static readonly FieldInfo BlockZeroFlagsField =
        typeof(MemorySegmentedBufferGroup)
            .GetField("_blockZeroFlags",
                BindingFlags.NonPublic | BindingFlags.Instance)!;

    /// <summary>
    /// The field information for the _bufferBlock field in the buffer group.
    /// </summary>
    private static readonly FieldInfo BufferBlockField =
        typeof(MemorySegmentedBufferGroup)
            .GetField("_bufferBlock",
                BindingFlags.NonPublic | BindingFlags.Instance)!;

    /// <summary>
    /// The field information for the _bufferBlockPtr field in the buffer group.
    /// </summary>
    private static readonly FieldInfo BufferBlockPtrField =
        typeof(MemorySegmentedBufferGroup)
            .GetField("_bufferBlockPtr",
                BindingFlags.NonPublic | BindingFlags.Instance)!;

    //--------------------------------------------------------------------------------
    /// <summary>
    /// For a given segment index, returns the index of the ulong in the block flag
    /// array as well as the mask to use to set or check the flag for the segment.
    /// </summary>
    /// <param name="segmentIndex">
    /// The segment index number to get the flag index and mask for.
    /// </param>
    /// <returns>
    /// The index of the ulong in the block flag array and the mask to use for the
    /// </returns>
    internal static (int Index, ulong Mask) GetFlagIndexAndMask (int segmentIndex)
    {
        int index = Math.DivRem(segmentIndex, BlockFlagSetSize, out int offset);
        return (index, 1UL << offset);
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Uses reflection to get the count of segments in the buffer group.
    /// </summary>
    /// <param name="bufferGroup">
    /// The buffer group to get the segment count for.
    /// </param>
    /// <returns>
    /// The value of the _segmentCount field in the buffer group.
    /// </returns>
    internal static int GetSegmentCount (MemorySegmentedBufferGroup bufferGroup) =>
        (int)BufferGroupSegmentCountField.GetValue(bufferGroup)!;
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Sets the count of segments in the buffer group using reflection.
    /// </summary>
    /// <param name="bufferGroup">
    /// The buffer group to set the segment count for.
    /// </param>
    /// <param name="segmentCount">
    /// The value of the _segmentCount field to set in the buffer group.
    /// </param>
    internal static void SetSegmentCount (MemorySegmentedBufferGroup bufferGroup, int segmentCount)
    {
#pragma warning disable HAA0601
        BufferGroupSegmentCountField.SetValue(bufferGroup, segmentCount);
#pragma warning restore HAA0601
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Gets the number of block flag ulong values in the block flag array that are 
    /// needed to store the block flags for the specified buffer group based on the
    /// number of segments in the group.
    /// </summary>
    /// <param name="bufferGroup">
    /// The buffer group to get the block flag array size needed for.
    /// </param>
    /// <returns>
    /// The number of block flag ulong values needed to store the block flags for the
    /// </returns>
    internal static int GetBlockFlagArraySizeNeeded (MemorySegmentedBufferGroup bufferGroup)
    {
        int segmentCount = GetSegmentCount(bufferGroup);
        return (segmentCount + BlockFlagSetSize - 1) / BlockFlagSetSize;
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Gets the number of segments in the buffer group that are in use.
    /// </summary>
    /// <param name="bufferGroup">
    /// The buffer group to get the number of segments in use for.
    /// </param>
    /// <returns>
    /// The number of segments in the buffer group that are in use.
    /// </returns>
    internal static int GetSegmentsInUse (MemorySegmentedBufferGroup bufferGroup) =>
        (int)BufferGroupSegmentsInUseField.GetValue(bufferGroup)!;
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Sets the number of segments in the buffer group that are in use.
    /// </summary>
    /// <param name="bufferGroup">
    /// The buffer group to set the number of segments in use for.
    /// </param>
    /// <param name="segmentsInUse">
    /// The number of segments in the buffer group that are in use.
    /// </param>
    internal static void SetSegmentsInUse (MemorySegmentedBufferGroup bufferGroup, int segmentsInUse)
    {
#pragma warning disable HAA0601
        BufferGroupSegmentsInUseField.SetValue(bufferGroup, segmentsInUse);
#pragma warning restore HAA0601
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Gets the internal buffer block array from the buffer group using reflection.
    /// </summary>
    /// <param name="bufferGroup">
    /// The buffer group to get the buffer block array for.
    /// </param>
    /// <returns>
    /// The internal buffer block array from the buffer group (which may be null).
    /// </returns>
    internal static byte[]? GetBufferBlock (MemorySegmentedBufferGroup bufferGroup) =>
        (byte[]?)BufferBlockField.GetValue(bufferGroup)!;
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Gets the internal buffer block pointer from the buffer group using reflection.
    /// </summary>
    /// <param name="bufferGroup">
    /// The buffer group to get the buffer block pointer for.
    /// </param>
    /// <returns>
    /// The internal buffer block pointer from the buffer group (which may be IntPtr.Zero).
    /// </returns>
    internal static unsafe byte* GetBufferBlockPtr (MemorySegmentedBufferGroup bufferGroup) =>
        (byte*)Pointer.Unbox(BufferBlockPtrField.GetValue(bufferGroup)!);
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Sets the block used flags for the buffer group to the specified ulong array.
    /// </summary>
    /// <param name="bufferGroup">
    /// The buffer group to set the block used flags for.
    /// </param>
    /// <param name="blockUsedFlags">
    /// The ulong array of block used flags to set in the buffer group.
    /// </param>
    /// <param name="blockZeroFlags">
    /// The ulong array of block zero flags to set in the buffer group.
    /// </param>
    internal static (int availableSegmentCount, Dictionary<int, List<int>> availableSegmentMap)
        SetBlockUsedFlags (MemorySegmentedBufferGroup bufferGroup, ulong[] blockUsedFlags, ulong[] blockZeroFlags)
    {
        // Get the number of segments in the buffer group
        int segmentFlagsToSet = GetSegmentCount(bufferGroup);
        // Build our own array to be sure we have the right size
        int blockFlagArraySize = GetBlockFlagArraySizeNeeded(bufferGroup);
        ulong[] setBlockUsedFlags = new ulong[blockFlagArraySize];
        ulong[] setBlockZeroFlags = new ulong[blockFlagArraySize];
        // Track the number of segments marked in use
        int segmentsAvailable = segmentFlagsToSet;
        int segmentsInUse = 0;
        int blockFlagIndex = 0;
        ulong blockFlagMask = 1;
        Dictionary<int, List<int>> availableSegmentMap = new();
        // Track the running list of available segments
        int? availableSegmentRunStart = null;
        int availableSegmentRunCount = 0;

        // Loop through the requested flags
        for (int segmentIndex = 0; segmentIndex < segmentFlagsToSet; segmentIndex++)
        {
            // Set the block used flag
            if ((blockFlagIndex < blockUsedFlags.Length)  &&
                ((blockUsedFlags[blockFlagIndex] & blockFlagMask) == blockFlagMask))
            {
                setBlockUsedFlags[blockFlagIndex] |= blockFlagMask;
                segmentsInUse++;
                segmentsAvailable--;
                // Store and reset the available segment run tracking
                if (availableSegmentRunCount > 0)
                {
                    UpdateAvailableSegmentMap(availableSegmentMap, ref availableSegmentRunStart, ref availableSegmentRunCount);
                }
            }
            else
            {
                // Update the available segment run tracking
                availableSegmentRunStart??= segmentIndex;
                availableSegmentRunCount++;
            }

            // Set the block zero flag
            if ((blockFlagIndex < blockZeroFlags.Length)  &&
                ((blockZeroFlags[blockFlagIndex] & blockFlagMask) == blockFlagMask))
            {
                setBlockZeroFlags[blockFlagIndex] |= blockFlagMask;
            }

            // Move to the next block flag
            blockFlagMask <<= 1;
            if (blockFlagMask != 0)
            {
                continue;
            }

            blockFlagMask = 1;
            blockFlagIndex++;
        }
        // Do a last update of the available segment map
        UpdateAvailableSegmentMap(availableSegmentMap, ref availableSegmentRunStart, ref availableSegmentRunCount);

        BlockUsedFlagsField.SetValue(bufferGroup, setBlockUsedFlags);
        BlockZeroFlagsField.SetValue(bufferGroup, setBlockZeroFlags);
        SetSegmentsInUse(bufferGroup, segmentsInUse);
        return (segmentsAvailable, availableSegmentMap);
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Sets the block used flags for the buffer group to the specified boolean array,
    /// converting the array to a ulong array for storage in the buffer group.
    /// </summary>
    /// <param name="bufferGroup">
    /// The buffer group to set the block used flags for.
    /// </param>
    /// <param name="blockUsedFlags">
    /// The boolean array of block used flags to set in the buffer group.
    /// </param>
    /// <param name="blockZeroFlags">
    /// The boolean array of block zero flags to set in the buffer group.
    /// </param>
    internal static (int availableSegmentCount, Dictionary<int, List<int>> availableSegmentMap)
        SetBlockUsedFlags (MemorySegmentedBufferGroup bufferGroup, bool[] blockUsedFlags, bool[] blockZeroFlags)
    {
        // Get the number of segments in the buffer group
        int segmentFlagsToSet = GetSegmentCount(bufferGroup);
        // Build our own array to be sure we have the right size
        int blockFlagArraySize = GetBlockFlagArraySizeNeeded(bufferGroup);
        ulong[] setBlockUsedFlags = new ulong[blockFlagArraySize];
        ulong[] setBlockZeroFlags = new ulong[blockFlagArraySize];

        // Set the proper flags in the ulong arrays
        for (int segmentIndex = 0; segmentIndex < segmentFlagsToSet; segmentIndex++)
        {
            int flagIndex = Math.DivRem(segmentIndex, BlockFlagSetSize, out int flagOffset);
            ulong flagMask = 1UL << flagOffset;

            if ((segmentIndex < blockUsedFlags.Length) && blockUsedFlags[segmentIndex])
            {
                setBlockUsedFlags[flagIndex] |= flagMask;
            }

            if ((segmentIndex < blockZeroFlags.Length) && blockZeroFlags[segmentIndex])
            {
                setBlockZeroFlags[flagIndex] |= flagMask;
            }
        }
        return SetBlockUsedFlags(bufferGroup, setBlockUsedFlags, setBlockZeroFlags);
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Helper to update the available segment map with the current run of available segments.
    /// </summary>
    /// <param name="availableSegmentMap">
    /// The map of available segment runs to update. This is keyed by the count of segments in 
    /// a consecutive segment run, and the value is a list of the starting indexes of the runs.
    /// </param>
    /// <param name="availableSegmentRunStart">
    /// The start index of the current run of available segments (if any).
    /// </param>
    /// <param name="availableSegmentRunCount">
    /// The current count of available segments in the run.
    /// </param>
    internal static void UpdateAvailableSegmentMap (Dictionary<int, List<int>> availableSegmentMap,
        ref int? availableSegmentRunStart, ref int availableSegmentRunCount)
    {
        if (availableSegmentRunCount <= 0)
        {
            availableSegmentRunStart = null;
            availableSegmentRunCount = 0;
            return;
        }

        if (availableSegmentMap.TryGetValue(availableSegmentRunCount, out List<int>? segmentList))
        {
            segmentList.Add(availableSegmentRunStart!.Value);
        }
        else
        {
            availableSegmentMap[availableSegmentRunCount] = [availableSegmentRunStart!.Value];
        }
        availableSegmentRunStart = null;
        availableSegmentRunCount = 0;
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Initializes a new instance of the <see cref="MemorySegmentedBufferGroupUnitTestBase"/> class.
    /// </summary>
    /// <param name="xUnitTestOutputHelper">
    /// The Xunit test output helper that can be used to output test messages
    /// </param>
    protected MemorySegmentedBufferGroupUnitTestBase (ITestOutputHelper xUnitTestOutputHelper) : base(xUnitTestOutputHelper)
    {
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Fills the internal native memory buffer block of the buffer group with non-zero values.
    /// </summary>
    /// <param name="bufferGroup">
    /// The buffer group to fill with non-zero values.
    /// </param>
    /// <returns>
    /// The buffer group with the internal native memory buffer block filled with non-zero values.
    /// </returns>
    internal MemorySegmentedBufferGroup FillNativeBufferWithNonZeros (MemorySegmentedBufferGroup bufferGroup)
    {
        unsafe
        {
            byte* bufferBlock = GetBufferBlockPtr(bufferGroup);
            int bufferBlockSize = bufferGroup.SegmentCount * MemorySegmentedBufferGroup.StandardBufferSegmentSize;
            for (int fillIndex = 0; fillIndex < bufferBlockSize; fillIndex++)
            {
                bufferBlock[fillIndex] = (byte)(1 + (fillIndex % 255));
            }
        }
        return bufferGroup;
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Fills the internal GC Heap buffer block of the buffer group with non-zero values.
    /// </summary>
    /// <param name="bufferGroup">
    /// The buffer group to fill with non-zero values.
    /// </param>
    /// <returns>
    /// The buffer group with the internal buffer block filled with non-zero values.
    /// </returns>
    internal MemorySegmentedBufferGroup FillGcHeapBufferWithNonZeros (MemorySegmentedBufferGroup bufferGroup)
    {
        byte[] bufferBlock = GetBufferBlock(bufferGroup)!;
        for (int fillIndex = 0; fillIndex < bufferBlock.Length; fillIndex++)
        {
            bufferBlock[fillIndex] = (byte)(1 + (fillIndex % 255));
        }
        return bufferGroup;
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Fills the internal buffer block of the buffer group with non-zero values.
    /// </summary>
    /// <param name="bufferGroup">
    /// The buffer group to fill with non-zero values.
    /// </param>
    /// <param name="isNativeMemory">
    /// Indicates if the buffer group is using native memory for the large memory segments.
    /// </param>
    /// <returns>
    /// The buffer group with the internal buffer block filled with non-zero values.
    /// </returns>
    internal MemorySegmentedBufferGroup FillWithNonZeros (MemorySegmentedBufferGroup bufferGroup,
        bool isNativeMemory) => isNativeMemory ?
        FillNativeBufferWithNonZeros(bufferGroup) :
        FillGcHeapBufferWithNonZeros(bufferGroup);
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Returns an instance of the test <see cref="MemorySegmentedBufferGroup"/> using 
    /// the specified segment count where the internal buffer is filled with non-zero values.
    /// </summary>
    /// <param name="segmentCount">
    /// The number of segments to use in the buffer group.
    /// </param>
    /// <param name="useNativeMemory">
    /// Indicates if native memory should be used for the large memory buffer segments.
    /// </param>
    /// <returns>
    /// An instance of the <see cref="MemorySegmentedBufferGroup"/> system under test.
    /// </returns>
    internal MemorySegmentedBufferGroup GetNonZeroTestBufferGroup (bool useNativeMemory = false, int segmentCount = 16)
    {
        // Force the group to allocate the buffer block on construction
        MemorySegmentedBufferGroup returnGroup =
            MemorySegmentedBufferGroup.GetMemorySegmentedBufferGroupWithAllocatedBuffer(segmentCount, useNativeMemory);
        return FillWithNonZeros(returnGroup, useNativeMemory);
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Returns an instance of the test <see cref="MemorySegmentedBufferGroup"/> using 
    /// the specified segment count.
    /// </summary>
    /// <param name="segmentCount">
    /// The number of segments to use in the buffer group.
    /// </param>
    /// <param name="useNativeMemory">
    /// Indicates if native memory should be used for the large memory buffer segments.
    /// </param>
    /// <returns>
    /// An instance of the <see cref="MemorySegmentedBufferGroup"/> system under test.
    /// </returns>
    internal MemorySegmentedBufferGroup GetTestBufferGroup (bool useNativeMemory = false, int segmentCount = 16) =>
        new(segmentCount, useNativeMemory);
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Returns an instance of the memory segmented buffer pool.
    /// </summary>
    /// <param name="useNativeMemory">
    /// Indicates if native memory should be used for the large memory buffer segments.
    /// </param>
    /// <returns>
    /// A test instance of the <see cref="MemorySegmentedBufferPool"/>.
    /// </returns>
    internal MemorySegmentedBufferPool GetTestBufferPool (bool useNativeMemory = false) => new(useNativeMemory);
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Gets a test <see cref="MemorySegmentedBufferGroup"/> as the subject under test
    /// and a <see cref="MemorySegmentedBufferPool"/> for testing. The pool is used 
    /// to represent the instance that would use the buffer group in a real application.
    /// </summary>
    /// <param name="useNativeMemory">
    /// Indicates if native memory should be used for the large memory buffer segments.
    /// </param>
    /// <param name="segmentCount">
    /// The number of segments to use in the buffer group.
    /// </param>
    /// <param name="nonZeroFilled">
    /// Indicates if the buffer group should be filled with non-zero values.
    /// </param>
    /// <returns>
    /// Both the buffer group and buffer pool for testing.
    /// </returns>
    internal (MemorySegmentedBufferGroup BufferGroup, MemorySegmentedBufferPool BufferPool)
        GetTestGroupAndPool (bool useNativeMemory = false, int segmentCount = 16,
        bool nonZeroFilled = false)
    {
        MemorySegmentedBufferGroup bufferGroup = nonZeroFilled ?
            GetNonZeroTestBufferGroup(useNativeMemory, segmentCount) :
            GetTestBufferGroup(useNativeMemory, segmentCount);
        MemorySegmentedBufferPool bufferPool = GetTestBufferPool(useNativeMemory);

        return (bufferGroup, bufferPool);
    }
    //--------------------------------------------------------------------------------

}
//################################################################################
