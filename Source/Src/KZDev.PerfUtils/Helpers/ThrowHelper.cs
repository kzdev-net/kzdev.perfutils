// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using KZDev.PerfUtils.Internals;
using KZDev.PerfUtils.Resources;

namespace KZDev.PerfUtils.Helpers;

//################################################################################
/// <summary>
/// Helper class for throwing exceptions.
/// </summary>
[ExcludeFromCodeCoverage]
internal static class ThrowHelper
{
    #region Argument Errors

    //--------------------------------------------------------------------------------
    /// <summary>
    /// Throws an argument out of range exception with the message for a non-negative number.
    /// </summary>
    /// <param name="argumentName">The name of the invalid argument.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// </exception>
    [DoesNotReturn]
    internal static void ThrowArgumentOutOfRangeException_NeedNonNegNum (string argumentName) =>
        throw new ArgumentOutOfRangeException(argumentName, string.Format(Strings.Arg_OutOfRangeNonNegNum, argumentName));
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Throws an argument out of range exception with the message for a number required 
    /// to be between two values.
    /// </summary>
    /// <param name="argumentName">
    /// The name of the invalid argument.
    /// </param>
    /// <param name="minimumValue">
    /// The required minimum value.
    /// </param>
    /// <param name="maximumValue">
    /// The required maximum value.
    /// </param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// </exception>
    [DoesNotReturn]
    internal static void ThrowArgumentOutOfRangeException_NeedBetween (string argumentName,
        long minimumValue, long maximumValue) =>
        throw new ArgumentOutOfRangeException(argumentName, string.Format(Strings.Arg_OutOfRangeNeedBetween, argumentName, minimumValue, maximumValue));
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Throws an argument out of range exception with the message for a number required 
    /// to be greater than or equal to a minimum value.
    /// </summary>
    /// <param name="argumentName">
    /// The name of the invalid argument.
    /// </param>
    /// <param name="minimumValue">
    /// The required minimum value.
    /// </param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// </exception>
    [DoesNotReturn]
    internal static void ThrowArgumentOutOfRangeException_NeedMinValue (string argumentName,
        long minimumValue) =>
        throw new ArgumentOutOfRangeException(argumentName, string.Format(Strings.Arg_OutOfRangeMinValue, argumentName, minimumValue));
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Throws an argument out of range exception with the message for a number required 
    /// to be less than the size of the collection.
    /// </summary>
    /// <param name="argumentName">
    /// The name of the invalid argument.
    /// </param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    [DoesNotReturn]
    internal static void ThrowArgumentOutOfRange_IndexMustBeLessException (string argumentName) =>
        throw new ArgumentOutOfRangeException(argumentName, Strings.Arg_IndexMustBeLessException);
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Throws an argument out of range exception with the message for a number required 
    /// to be less than or equal to the size of the collection.
    /// </summary>
    /// <param name="argumentName">
    /// The name of the invalid argument.
    /// </param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// </exception>
    [DoesNotReturn]
    internal static void ThrowArgumentOutOfRange_IndexMustBeLessOrEqualException (string argumentName) =>
        throw new ArgumentOutOfRangeException(argumentName, Strings.Arg_IndexMustBeLessOrEqualException);
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Throws an argument exception with the message for an invalid Stream seek origin
    /// value.
    /// </summary>
    /// <param name="argumentName">
    /// The name of the invalid argument.
    /// </param>
    /// <exception cref="ArgumentException">
    /// </exception>
    [DoesNotReturn]
    internal static void ThrowArgumentException_InvalidSeekOrigin (string argumentName) =>
        throw new ArgumentException(Strings.Arg_InvalidSeekOrigin, argumentName);
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Throws an argument with the message for an invalid array segment offset
    /// and length.
    /// </summary>
    /// <exception cref="ArgumentException">
    /// </exception>
    [DoesNotReturn]
    internal static void ThrowArgumentException_InvalidOffsetLength () =>
        throw new ArgumentException(Strings.Arg_InvalidOffsetLength);
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Throws an argument with the message for copy destination that is too short
    /// and length.
    /// </summary>
    /// <param name="argumentName">
    /// The name of the invalid argument.
    /// </param>
    /// <exception cref="ArgumentException">
    /// </exception>
    [DoesNotReturn]
    internal static void ThrowArgumentException_DestinationTooShort (string argumentName) =>
        throw new ArgumentException(Strings.Arg_DestinationTooShort, argumentName);
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Throws an argument with the message for a source stream that is not readable 
    /// </summary>
    /// <param name="argumentName">
    /// The name of the invalid argument.
    /// </param>
    /// <exception cref="ArgumentException">
    /// </exception>
    [DoesNotReturn]
    internal static void ThrowArgumentException_SourceStreamMustBeReadable (string argumentName) =>
        throw new ArgumentException(Strings.Arg_SourceStreamMustBeReadable, argumentName);
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Throws an argument with the message for a source stream that is not seekable
    /// </summary>
    /// <param name="argumentName">
    /// The name of the invalid argument.
    /// </param>
    /// <exception cref="ArgumentException">
    /// </exception>
    [DoesNotReturn]
    internal static void ThrowArgumentException_SourceStreamMustBeSeekable (string argumentName) =>
        throw new ArgumentException(Strings.Arg_SourceStreamMustBeSeekable, argumentName);
    //--------------------------------------------------------------------------------

    #endregion Argument Errors

    #region Stream Errors

    //--------------------------------------------------------------------------------
    /// <summary>
    /// Throws an invalid operation exception with the message for a closed stream.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// </exception>
    [DoesNotReturn]
    internal static void ThrowInvalidOperationException_StreamClosed () =>
        throw new InvalidOperationException(Strings.InvalidOperation_StreamClosed);
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Throws an object disposed exception with the message for a closed stream.
    /// </summary>
    /// <exception cref="ObjectDisposedException">
    /// </exception>
    [DoesNotReturn]
    internal static void ThrowObjectDisposedException_StreamClosed () =>
        throw new ObjectDisposedException(Strings.InvalidOperation_StreamClosed);
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Throws a not supported exception with the message for an unwritable stream.
    /// </summary>
    /// <exception cref="NotSupportedException">
    /// </exception>
    [DoesNotReturn]
    internal static void ThrowNotSupportedException_UnwritableStream () =>
        throw new NotSupportedException(Strings.NotSupported_UnwritableStream);
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Throws a not supported exception with the message for a feature not supported
    /// in the specified stream mode.
    /// </summary>
    /// <param name="streamMode">
    /// The mode of the stream that does not support the feature.
    /// </param>
    /// <exception cref="NotSupportedException">
    /// </exception>
    [DoesNotReturn]
    internal static void ThrowNotSupportedException_InvalidModeStreamStream (MemoryStreamSlimMode streamMode) =>
        throw new NotSupportedException(string.Format(Strings.NotSupported_InvalidModeStreamStream, streamMode.GetString()));
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Throws a not supported exception with the message for a general feature not supported.
    /// </summary>
    /// <exception cref="NotSupportedException">
    /// </exception>
    [DoesNotReturn]
    internal static void ThrowNotSupportedException_FeatureNotSupported () =>
        throw new NotSupportedException(Strings.NotSupported_FeatureNotAvailable);
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Throws an argument out of range exception with the message for a small capacity.
    /// </summary>
    /// <param name="argumentName">
    /// The name of the invalid argument.
    /// </param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// </exception>
    [DoesNotReturn]
    internal static void ThrowArgumentOutOfRangeException_CapacitySmall (string argumentName) =>
        throw new ArgumentOutOfRangeException(argumentName, Strings.Arg_OutOfRangeSmallCapacity);
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Throws an argument out of range exception with the message indicating that the 
    /// stream length would be invalid.
    /// </summary>
    /// <exception cref="IOException">
    /// </exception>
    [DoesNotReturn]
    internal static void ThrowArgumentOutOfRangeException_StreamLength (string argumentName) =>
        throw new ArgumentOutOfRangeException(argumentName, Strings.Arg_OutOfRangeStreamLength);
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Throws an IO exception with the message for the stream being too long.
    /// </summary>
    /// <exception cref="IOException">
    /// </exception>
    [DoesNotReturn]
    internal static void ThrowIOException_StreamTooLong () =>
        throw new IOException(Strings.IO_StreamTooLong);
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Throws an IO exception with the message for an attempt to move the position
    /// before the beginning of the stream.
    /// </summary>
    /// <exception cref="IOException">
    /// </exception>
    [DoesNotReturn]
    internal static void ThrowIOException_SeekBeforeBegin () =>
        throw new IOException(Strings.IO_SeekBeforeBegin);
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Throws an invalid operation exception with the message for an integer overflow
    /// when calculating the new capacity or accessing the Int32 Capacity property of a stream.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    [DoesNotReturn]
    internal static void ThrowInvalidOperationException_IntOverflowCapacity () =>
        throw new InvalidOperationException(Strings.InvalidOperation_IntOverflowCapacity);
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Throws an invalid operation exception with the message for an attempt to copy
    /// the stream into an array but the length of the stream is too large to fit in an array.
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    [DoesNotReturn]
    internal static void ThrowInvalidOperationException_TooLargeToCopyToArray () =>
        throw new InvalidOperationException (Strings.InvalidOperation_TooLargeToCopyToArray);
    //--------------------------------------------------------------------------------

    #endregion Stream Errors

    //--------------------------------------------------------------------------------
    /// <summary>
    /// Throws an invalid operation exception with the message for a type of global settings
    /// that cannot be updated.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// </exception>
    [DoesNotReturn]
    internal static void ThrowInvalidOperation_GlobalSettingsCantBeUpdated (string classTypeName) =>
        throw new InvalidOperationException(Strings.InvalidOperation_GlobalSettingsCantBeUpdated);

    //--------------------------------------------------------------------------------
}
//################################################################################
