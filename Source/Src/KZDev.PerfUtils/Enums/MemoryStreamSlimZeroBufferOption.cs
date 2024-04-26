// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

namespace KZDev.PerfUtils
{
    //################################################################################
    /// <summary>
    /// Options for how to handle memory clearing when a memory buffer is not in use.
    /// </summary>
    public enum MemoryStreamSlimZeroBufferOption
    {
        /// <summary>
        /// Zero out buffers out-of-band after they are released. Newly allocated
        /// buffers will also be zeroed out when they are acquired as needed. This is
        /// the default value for <see cref="MemoryStreamSlimMode.Dynamic"/> streams.
        /// </summary>
        OutOfBand,
        /// <summary>
        /// Zero out buffers inline as part of the release process. Newly allocated 
        /// buffers will also be zeroed out when they are acquired as needed. This is
        /// the set value for <see cref="MemoryStreamSlimMode.Fixed"/> streams.
        /// </summary>
        OnRelease,
        /// <summary>
        /// Do not zero out buffers. This is the most efficient option, but it may
        /// leave sensitive data in memory that is sitting in the buffer pool or
        /// in stream buffer space that is not yet in use by a stream.
        /// </summary>
        None
    }
    //################################################################################
}
