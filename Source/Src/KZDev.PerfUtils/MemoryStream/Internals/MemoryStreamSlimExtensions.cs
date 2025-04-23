// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

namespace KZDev.PerfUtils.Internals;

//################################################################################
/// <summary>
/// Extension methods for the <see cref="MemoryStreamSlim"/> class.
/// </summary>
internal static class MemoryStreamSlimExtensions
{
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Converts the specified <see cref="MemoryStreamSlimSettings"/> instance to a
    /// <see cref="MemoryStreamSlimOptions"/> instance.
    /// </summary>
    public static MemoryStreamSlimOptions ToOptions(this in MemoryStreamSlimSettings settings)
    {
        return new MemoryStreamSlimOptions
        {
            ZeroBufferBehavior = settings.ZeroBufferBehavior
        };
    }
    //--------------------------------------------------------------------------------
}
//################################################################################
