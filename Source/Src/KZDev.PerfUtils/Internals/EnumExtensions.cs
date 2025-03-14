// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;

namespace KZDev.PerfUtils.Internals;

//################################################################################
/// <summary>
/// Extension methods for enums.
/// </summary>
[ExcludeFromCodeCoverage]
internal static class EnumExtensions
{
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Gets the string representation of the <see cref="MemoryStreamSlimZeroBufferOption"/> enum.
    /// </summary>
    /// <param name="memoryStreamSlimZeroBufferOption">
    /// The enum value to convert to a string.
    /// </param>
    /// <returns>
    /// The string representation of the enum value.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static string GetString (this MemoryStreamSlimZeroBufferOption memoryStreamSlimZeroBufferOption) =>
        (memoryStreamSlimZeroBufferOption) switch
        {
            MemoryStreamSlimZeroBufferOption.None => nameof(MemoryStreamSlimZeroBufferOption.None),
            MemoryStreamSlimZeroBufferOption.OnRelease => nameof(MemoryStreamSlimZeroBufferOption.OnRelease),
            MemoryStreamSlimZeroBufferOption.OutOfBand => nameof(MemoryStreamSlimZeroBufferOption.OutOfBand),
            _ => throw new ArgumentOutOfRangeException(nameof(memoryStreamSlimZeroBufferOption), memoryStreamSlimZeroBufferOption, null)
        };
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Gets the string representation of the <see cref="MemoryStreamSlimMode"/> enum.
    /// </summary>
    /// <param name="memoryStreamSlimMode">
    /// The enum value to convert to a string.
    /// </param>
    /// <returns>
    /// The string representation of the enum value.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static string GetString (this MemoryStreamSlimMode memoryStreamSlimMode) =>
        (memoryStreamSlimMode) switch
        {
            MemoryStreamSlimMode.Fixed => nameof(MemoryStreamSlimMode.Fixed),
            MemoryStreamSlimMode.Dynamic => nameof(MemoryStreamSlimMode.Dynamic),
            _ => throw new ArgumentOutOfRangeException(nameof(memoryStreamSlimMode), memoryStreamSlimMode, null)
        };
    //--------------------------------------------------------------------------------
}
//################################################################################
