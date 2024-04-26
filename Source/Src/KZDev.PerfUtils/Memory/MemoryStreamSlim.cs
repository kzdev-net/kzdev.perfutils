using System.Diagnostics;

namespace KZDev.PerfUtils
{
    //################################################################################
    /// <summary>
    /// MemoryStreamSlim is a memory stream that is designed to be more efficient 
    /// than the standard MemoryStream class, by using a list of buffers to store
    /// the contents of the stream, minimizing GC memory traffic pressure and avoiding
    /// allocations on the Large Object Heap (LOH).
    /// </summary>
    [DebuggerDisplay("{" + nameof(DisplayValue) + "}")]
    public abstract class MemoryStreamSlim : Stream
    {
        /// <summary>
        /// Gets the (debug) display value.
        /// </summary>
        /// <value>
        /// The (debug) display value.
        /// </value>
        private string DisplayValue => string.Empty;
    }
    //################################################################################
}
