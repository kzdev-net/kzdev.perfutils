using KZDev.PerfUtils.Internals;

using Xunit.Abstractions;

namespace KZDev.PerfUtils.Tests
{
    //################################################################################
    /// <summary>
    /// Base class for unit tests that use the 
    /// <see cref="MemorySegmentedBufferGroup"/> system under test.
    /// </summary>
    public abstract class MemorySegmentedBufferGroupUnitTestBase : UnitTestBase
    {
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
        /// Returns an instance of the system under test using the specified segment count.
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
        internal MemorySegmentedBufferGroup GetSut (bool useNativeMemory = false, int segmentCount = 16) => new(segmentCount, useNativeMemory);
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
        /// <returns>
        /// Both the buffer group and buffer pool for testing.
        /// </returns>
        internal (MemorySegmentedBufferGroup BufferGroup, MemorySegmentedBufferPool BufferPool)
            GetTestGroupAndPool (bool useNativeMemory = false, int segmentCount = 16)
        {
            MemorySegmentedBufferGroup bufferGroup = GetSut(useNativeMemory, segmentCount);
            MemorySegmentedBufferPool bufferPool = GetTestBufferPool(useNativeMemory);

            return (bufferGroup, bufferPool);
        }
        //--------------------------------------------------------------------------------

    }
    //################################################################################
}
