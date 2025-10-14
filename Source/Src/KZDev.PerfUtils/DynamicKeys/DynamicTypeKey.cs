// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Diagnostics;

namespace KZDev.PerfUtils;

//################################################################################
/// <summary>
/// A type of <see cref="DynamicKey"/> that uses a <see cref="Type"/>
/// as the key
/// </summary>
[DebuggerDisplay("{" + nameof(DisplayValue) + "}")]
internal sealed class DynamicTypeKey : DynamicKey, IComparable<DynamicTypeKey>
{
    /// <summary>
    /// When not null, this is a cached instance used to avoid creating multiple
    /// instances of this class for the same Type value on the same thread.
    /// </summary>
    [ThreadStatic] private static DynamicTypeKey? _cachedInstance;

    /// <summary>
    /// Gets the debugger display value.
    /// </summary>
    /// <value>
    /// The debugger display value.
    /// </value>
    private string DisplayValue => ToString();

    /// <summary>
    /// The single Type value used as the key value for this instance.
    /// </summary>
    public Type Value { [DebuggerStepThrough] get; }

    //--------------------------------------------------------------------------------
    /// <summary>
    /// Initializes a new instance of the <see cref="DynamicTypeKey"/> class.
    /// </summary>
    /// <param name="value">
    /// The Type value to use as a key for this instance.
    /// </param>
    private DynamicTypeKey (Type value)
    {
        Value = value;
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Gets a <see cref="DynamicKey"/> instance for the given Type value.
    /// </summary>
    /// <param name="value">The Type value to use</param>
    /// <returns>
    /// An instance of <see cref="DynamicKey"/> that uses the specified Type value to
    /// represent the key or partial key.
    /// </returns>
    public new static DynamicKey GetKey (Type value)
    {
        ArgumentNullException.ThrowIfNull(value);

        DynamicTypeKey? cachedInstance = _cachedInstance;
        if ((cachedInstance is not null) && (cachedInstance.Value == value))
            return cachedInstance;
        DynamicTypeKey returnInstance = new(value);
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
    public override string ToString () => Value.FullName ?? Value.Name;
    //--------------------------------------------------------------------------------

    #endregion Overrides of Object

    #region Overrides of DynamicKey

    //--------------------------------------------------------------------------------
    /// <inheritdoc />
    public override bool Equals (DynamicKey? other) =>
        (other is DynamicTypeKey typeDynamicKey) &&
        (ReferenceEquals(typeDynamicKey.Value, Value) || (typeDynamicKey.Value == Value));
    //--------------------------------------------------------------------------------
    /// <inheritdoc />
    public override int CompareTo (DynamicKey? other) =>
        other switch
        {
            null => 1,
            DynamicTypeKey typeDynamicKey => CompareTo(typeDynamicKey),
            _ => CompareKey(other)
        };
    //--------------------------------------------------------------------------------

    #endregion

    #region IComparable<DynamicTypeKey> Members

    //--------------------------------------------------------------------------------
    /// <inheritdoc />
    public int CompareTo (DynamicTypeKey? other) => other is null ? 1 :
        ReferenceEquals(other.Value, Value) ? 0 :
        string.Compare(Value.FullName, other.Value.FullName, StringComparison.Ordinal);
    //--------------------------------------------------------------------------------

    #endregion IComparable<DynamicTypeKey> Members
}
//################################################################################