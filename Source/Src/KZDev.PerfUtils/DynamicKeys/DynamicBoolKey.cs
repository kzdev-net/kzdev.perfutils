// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Diagnostics;

namespace KZDev.PerfUtils;

//################################################################################
/// <summary>
/// A type of <see cref="DynamicKey"/> that uses a boolean value as a key.
/// </summary>
[DebuggerDisplay("{" + nameof(DisplayValue) + "}")]
internal sealed class DynamicBoolKey : DynamicKey, IComparable<DynamicBoolKey>
{
    /// <summary>
    /// Gets the debugger display value.
    /// </summary>
    /// <value>
    /// The debugger display value.
    /// </value>
    private string DisplayValue => ToString();

    /// <summary>
    /// The boolean value used as the key value for this instance.
    /// </summary>
    public bool Value { [DebuggerStepThrough] get; }

    //--------------------------------------------------------------------------------
    /// <summary>
    /// Returns a common cached instance used for <c>true</c> boolean values
    /// </summary>
    public new static DynamicBoolKey True { [DebuggerStepThrough] get; } = new(true);
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Returns a common cached instance used for <c>false</c> boolean values
    /// </summary>
    public new static DynamicBoolKey False { [DebuggerStepThrough] get; } = new(false);
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Initializes a new instance of the <see cref="DynamicBoolKey"/> class.
    /// </summary>
    /// <param name="value">
    /// The boolean value to use as a key for this instance.
    /// </param>
    private DynamicBoolKey (bool value)
    {
        Value = value;
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Gets a <see cref="DynamicKey"/> instance for the given boolean value.
    /// </summary>
    /// <param name="value">
    /// The boolean value to use as a key.
    /// </param>
    /// <returns>
    /// An instance of <see cref="DynamicKey"/> that uses the specified boolean value to
    /// represent the key or partial key.
    /// </returns>
    public new static DynamicKey GetKey (bool value) => value ? True : False;
    //--------------------------------------------------------------------------------

    #region Overrides of Object

    //--------------------------------------------------------------------------------
    /// <inheritdoc />
    public override int GetHashCode () => Value ? 1 : 0;
    //--------------------------------------------------------------------------------
    /// <inheritdoc />
    public override string ToString () => Value.ToString();
    //--------------------------------------------------------------------------------

    #endregion Overrides of Object

    #region Overrides of DynamicKey

    //--------------------------------------------------------------------------------
    /// <inheritdoc />
    public override bool Equals (DynamicKey? other) =>
        (other is DynamicBoolKey boolDynamicKey) &&
        (ReferenceEquals(this, boolDynamicKey) || (boolDynamicKey.Value == Value));
    //--------------------------------------------------------------------------------
    /// <inheritdoc />
    public override int CompareTo (DynamicKey? other) =>
        other switch
        {
            null => 1,
            DynamicBoolKey boolDynamicKey => CompareTo(boolDynamicKey),
            _ => CompareKey(other)
        };
    //--------------------------------------------------------------------------------

    #endregion

    #region IComparable<DynamicBoolKey> Members

    //--------------------------------------------------------------------------------
    /// <inheritdoc />
    public int CompareTo (DynamicBoolKey? other) => (other is null) ? 1 :
        Value.CompareTo(other.Value);
    //--------------------------------------------------------------------------------

    #endregion IComparable<DynamicBoolKey> Members
}
//################################################################################