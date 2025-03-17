// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

namespace KZDev.PerfUtils;

//################################################################################
/// <summary>
/// Extension methods for <see cref="MemoryStreamSlimOptions"/> instances.
/// </summary>
public static class MemoryStreamSlimOptionsExtensions
{
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Sets the <see cref="MemoryStreamSlimOptions.ZeroBufferBehavior"/> property on the
    /// <see cref="MemoryStreamSlimOptions"/> instance.
    /// </summary>
    /// <param name="options">
    /// The <see cref="MemoryStreamSlimOptions"/> instance to modify.
    /// </param>
    /// <param name="zeroBufferBehavior">
    /// The zero buffer behavior to set.
    /// </param>
    /// <returns>
    /// A new <see cref="MemoryStreamSlimOptions"/> instance with the specified zero buffer behavior.
    /// </returns>
    public static MemoryStreamSlimOptions WithZeroBufferBehavior (this MemoryStreamSlimOptions options,
        MemoryStreamSlimZeroBufferOption zeroBufferBehavior) =>
        options with { ZeroBufferBehavior = zeroBufferBehavior };
    //--------------------------------------------------------------------------------
}
//################################################################################