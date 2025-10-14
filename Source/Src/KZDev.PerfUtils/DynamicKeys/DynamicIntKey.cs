// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Diagnostics;

namespace KZDev.PerfUtils;

//################################################################################
/// <summary>
/// A type of <see cref="DynamicKey"/> that uses a single integer as the key
/// </summary>
[DebuggerDisplay("{" + nameof(DisplayValue) + "}")]
internal sealed class DynamicIntKey : DynamicKey, IComparable<DynamicIntKey>
{
    /// <summary>
    /// When not null, this is a cached instance used to avoid creating multiple
    /// instances of this class for the same integer value on the same thread.
    /// </summary>
    [ThreadStatic] private static DynamicIntKey? _cachedInstance;

    /// <summary>
    /// Gets the debugger display value.
    /// </summary>
    /// <value>
    /// The debugger display value.
    /// </value>
    private string DisplayValue => ToString();

    /// <summary>
    /// The single integer value used as the key value for this instance.
    /// </summary>
    public int Value { [DebuggerStepThrough] get; }

    //--------------------------------------------------------------------------------
    /// <summary>
    /// Returns a common cached instance used for zero values.
    /// </summary>
    public new static DynamicIntKey Zero { [DebuggerStepThrough] get; } = new(0);
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Initializes a new instance of the <see cref="DynamicIntKey"/> class.
    /// </summary>
    /// <param name="value">
    /// The integer value to use as a key for this instance.
    /// </param>
    private DynamicIntKey (int value)
    {
        Value = value;
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Gets a <see cref="DynamicKey"/> instance for the given integer value.
    /// </summary>
    /// <param name="value">
    /// The integer value to use as a key.
    /// </param>
    /// <returns>
    /// An instance of <see cref="DynamicKey"/> that uses the specified integer value to
    /// represent the key or partial key.
    /// </returns>
    public new static DynamicKey GetKey (int value)
    {
        if (value == 0)
            return Zero;
        DynamicIntKey? cachedInstance = _cachedInstance;
        if ((cachedInstance is not null) && (cachedInstance.Value == value))
            return cachedInstance;
        DynamicIntKey returnInstance = new(value);
        _cachedInstance = returnInstance;
        return returnInstance;
    }
    //--------------------------------------------------------------------------------

    #region Overrides of Object

    //--------------------------------------------------------------------------------
    /// <inheritdoc />
    public override int GetHashCode () => Value;
    //--------------------------------------------------------------------------------
    /// <inheritdoc />
    public override string ToString () => Value.ToString();
    //--------------------------------------------------------------------------------

    #endregion Overrides of Object

    #region Overrides of DynamicKey

    //--------------------------------------------------------------------------------
    /// <inheritdoc />
    public override bool Equals (DynamicKey? other) =>
        (other is DynamicIntKey intDynamicKey) &&
        (ReferenceEquals(this, intDynamicKey) || (intDynamicKey.Value == Value));
    //--------------------------------------------------------------------------------
    /// <inheritdoc />
    public override int CompareTo (DynamicKey? other) =>
        other switch
        {
            null => 1,
            DynamicIntKey intDynamicKey => CompareTo(intDynamicKey),
            _ => CompareKey(other)
        };
    //--------------------------------------------------------------------------------

    #endregion

    #region IComparable<DynamicIntKey> Members

    //--------------------------------------------------------------------------------
    /// <inheritdoc />
    public int CompareTo (DynamicIntKey? other) => other is null ? 1 :
        Value.CompareTo(other.Value);
    //--------------------------------------------------------------------------------

    #endregion IComparable<DynamicIntKey> Members
}
//################################################################################