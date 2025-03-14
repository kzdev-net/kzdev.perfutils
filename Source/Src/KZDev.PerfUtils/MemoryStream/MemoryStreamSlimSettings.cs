// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Diagnostics;

namespace KZDev.PerfUtils;

//################################################################################
/// <summary>
/// The effective settings for <see cref="MemoryStreamSlim"/> instances.
/// </summary>
public readonly struct MemoryStreamSlimSettings
{
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Initializes a new instance of the <see cref="MemoryStreamSlimSettings"/> class
    /// with default values.
    /// </summary>
    public MemoryStreamSlimSettings()
    {
        ZeroBufferBehavior = MemoryStreamSlimOptions.DefaultZeroBufferBehavior;
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Initializes a new instance of the <see cref="MemoryStreamSlimSettings"/> class 
    /// with the values of the specified options.
    /// </summary>
    /// <param name="options">
    /// Options that will configure the settings.
    /// </param>
    internal MemoryStreamSlimSettings(MemoryStreamSlimOptions? options)
    {
        ZeroBufferBehavior = options?.ZeroBufferBehavior ?? MemoryStreamSlimOptions.DefaultZeroBufferBehavior;
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Gets the setting specifying how buffers are zeroed out when memory is released.
    /// </summary>
    /// <remarks>
    /// This maps to the <see cref="MemoryStreamSlimOptions.ZeroBufferBehavior"/> property
    /// that was specified when creating the <see cref="MemoryStreamSlim"/> instance.
    /// </remarks>
    public MemoryStreamSlimZeroBufferOption ZeroBufferBehavior { [DebuggerStepThrough] get; }
    //--------------------------------------------------------------------------------
}
//################################################################################
