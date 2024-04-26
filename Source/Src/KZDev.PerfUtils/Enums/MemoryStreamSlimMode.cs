// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

namespace KZDev.PerfUtils
{
    //################################################################################
    /// <summary>
    /// The mode of the memory stream.
    /// </summary>
    public enum MemoryStreamSlimMode
    {
        /// <summary>
        /// The memory stream is fixed in size and wraps an existing byte array buffer.
        /// </summary>
        Fixed,

        /// <summary>
        /// The memory stream is dynamic and can grow and shrink in size as needed.
        /// </summary>
        Dynamic
    }
    //################################################################################
}
