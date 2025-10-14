// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Diagnostics;

namespace KZDev.PerfUtils;

//################################################################################
/// <summary>
/// A type of <see cref="DynamicKey"/> that uses a single unsigned 64-bit integer as the key
/// </summary>
[DebuggerDisplay("{" + nameof(DisplayValue) + "}")]
internal class DynamicULongKey : DynamicKey, IComparable<DynamicULongKey>
{
    /// <summary>
    /// When not null, this is a cached instance used to avoid creating multiple
    /// instances of this class for the same ulong value on the same thread.
    /// </summary>
    [ThreadStatic] private static DynamicULongKey? _cachedInstance;

    /// <summary>
    /// Gets the debugger display value.
    /// </summary>
    /// <value>
    /// The debugger display value.
    /// </value>
    private string DisplayValue => ToString();

    /// <summary>
    /// The single unsigned 64-bit integer value used as the key value for this instance.
    /// </summary>
    public ulong Value { [DebuggerStepThrough] get; }

    //--------------------------------------------------------------------------------
    /// <summary>
    /// Returns a common cached instance used for zero values.
    /// </summary>
    public new static DynamicULongKey Zero { [DebuggerStepThrough] get; } = new(0);
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Initializes a new instance of the <see cref="DynamicULongKey"/> class.
    /// </summary>
    /// <param name="value">
    /// The unsigned 64-bit integer value to use as a key for this instance.
    /// </param>
    private DynamicULongKey (ulong value)
    {
        Value = value;
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Gets a <see cref="DynamicKey"/> instance for the given unsigned 64-bit integer value.
    /// </summary>
    /// <param name="value">
    /// The unsigned 64-bit integer value to use as a key.
    /// </param>
    /// <returns>
    /// An instance of <see cref="DynamicKey"/> that uses the specified unsigned 64-bit integer value to
    /// represent the key or partial key.
    /// </returns>
    public new static DynamicKey GetKey (ulong value)
    {
        if (value == 0)
            return Zero;
        DynamicULongKey? cachedInstance = _cachedInstance;
        if ((cachedInstance is not null) && (cachedInstance.Value == value))
            return cachedInstance;
        DynamicULongKey returnInstance = new(value);
        _cachedInstance = returnInstance;
        return returnInstance;
    }
    //--------------------------------------------------------------------------------

    #region Overrides of Object

    //--------------------------------------------------------------------------------
    /// <inheritdoc />
    public override int GetHashCode () => Value.GetHashCode();
    //--------------------------------------------------------------------------------
    /// <inheritdoc />
    public override string ToString () => Value.ToString();
    //--------------------------------------------------------------------------------

    #endregion Overrides of Object

    #region Overrides of DynamicKey

    //--------------------------------------------------------------------------------
    /// <inheritdoc />
    public override bool Equals (DynamicKey? other) =>
        (other is DynamicULongKey ulongDynamicKey) &&
        (ReferenceEquals(this, ulongDynamicKey) || (ulongDynamicKey.Value == Value));
    //--------------------------------------------------------------------------------
    /// <inheritdoc />
    public override int CompareTo (DynamicKey? other) =>
        other switch
        {
            null => 1,
            DynamicULongKey ulongDynamicKey => CompareTo(ulongDynamicKey),
            _ => CompareKey(other)
        };
    //--------------------------------------------------------------------------------

    #endregion

    #region IComparable<DynamicULongKey> Members

    //--------------------------------------------------------------------------------
    /// <inheritdoc />
    public int CompareTo (DynamicULongKey? other) => other is null ? 1 :
        Value.CompareTo(other.Value);
    //--------------------------------------------------------------------------------

    #endregion IComparable<DynamicULongKey> Members
}
//################################################################################
