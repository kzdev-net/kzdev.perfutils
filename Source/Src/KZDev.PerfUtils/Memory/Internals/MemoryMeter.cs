// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Diagnostics.Metrics;

namespace KZDev.PerfUtils.Internals
{
    //################################################################################
    /// <summary>
    /// Observable meter for memory related components.
    /// </summary>
    internal static class MemoryMeter
    {
        /// <summary>
        /// The meter used for memory related components.
        /// </summary>
        public static readonly Meter Meter = new("kzdev.perfutils.memory");
    }
    //################################################################################
}
