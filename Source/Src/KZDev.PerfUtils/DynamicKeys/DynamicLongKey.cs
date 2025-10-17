// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Diagnostics;

namespace KZDev.PerfUtils;

//################################################################################
/// <summary>
///   A type of <see cref="DynamicKey"/> that uses a single 64-bit signed integer as the key.
/// </summary>
/// <remarks>
///   <para>
///     <see cref="DynamicLongKey"/> provides an optimized implementation for 64-bit integer-based keys
///     with thread-static caching for improved performance. It uses the long value directly
///     for hash code generation and comparison operations.
///   </para>
///   <para>
///     Key features include:
///   </para>
///   <list type="bullet">
///     <item>
///       <description>Thread-static caching for frequently used long values</description>
///     </item>
///     <item>
///       <description>Direct long value for hash codes (no computation overhead)</description>
///     </item>
///     <item>
///       <description>Optimized equality and comparison operations</description>
///     </item>
///     <item>
///       <description>Special handling for zero values with cached instance</description>
///     </item>
///   </list>
///   <para>
///     This class is used internally by the <see cref="DynamicKey"/> system and is not
///     typically instantiated directly by user code. Instead, use <see cref="DynamicKey.GetKey(long)"/>
///     to create long-based keys.
///   </para>
/// </remarks>
/// <seealso cref="DynamicKey"/>
[DebuggerDisplay("{" + nameof(DisplayValue) + "}")]
internal sealed class DynamicLongKey : DynamicKey, IComparable<DynamicLongKey>
{
    /// <summary>
    /// When not null, this is a cached instance used to avoid creating multiple
    /// instances of this class for the same long value on the same thread.
    /// </summary>
    [ThreadStatic] private static DynamicLongKey? _cachedInstance;

    /// <summary>
    /// Gets the debugger display value.
    /// </summary>
    /// <value>
    /// The debugger display value.
    /// </value>
    private string DisplayValue => ToString();

    /// <summary>
    /// The object representation of the long value used for this instance.
    /// </summary>
    private object? _objValue;

    /// <summary>
    /// The single 64-bit integer value used as the key value for this instance.
    /// </summary>
    public long Value { [DebuggerStepThrough] get; }

    //--------------------------------------------------------------------------------
    /// <summary>
    /// Returns a common cached instance used for zero values.
    /// </summary>
    public new static DynamicLongKey Zero { [DebuggerStepThrough] get; } = new(0);
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Initializes a new instance of the <see cref="DynamicLongKey"/> class.
    /// </summary>
    /// <param name="value">
    /// The 64-bit integer value to use as a key for this instance.
    /// </param>
    private DynamicLongKey (long value)
    {
        Value = value;
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Gets a <see cref="DynamicKey"/> instance for the given 64-bit integer value.
    /// </summary>
    /// <param name="value">
    /// The 64-bit integer value to use as a key.
    /// </param>
    /// <returns>
    /// An instance of <see cref="DynamicKey"/> that uses the specified 64-bit integer value to
    /// represent the key or partial key.
    /// </returns>
    public new static DynamicKey GetKey (long value)
    {
        if (value == 0)
            return Zero;
        DynamicLongKey? cachedInstance = _cachedInstance;
        if ((cachedInstance is not null) && (cachedInstance.Value == value))
            return cachedInstance;
        DynamicLongKey returnInstance = new(value);
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
        ((other is DynamicLongKey longDynamicKey) &&
         (ReferenceEquals(this, longDynamicKey) || (longDynamicKey.Value == Value))) ||
        ((other is DynamicCompositeKey compositeKey) && compositeKey.Equals(this)) ||
        ((other?.ObjectValue is DynamicCompositeKey otherCompositeKey) && otherCompositeKey.Equals(this)) ||
        Equals(other?.ObjectValue, ObjectValue);
    //--------------------------------------------------------------------------------
    /// <inheritdoc />
    public override int CompareTo (DynamicKey? other) =>
        other switch
        {
            null => 1,
            DynamicLongKey longDynamicKey => CompareTo(longDynamicKey),
            _ => CompareKey(other)
        };
    //--------------------------------------------------------------------------------

    #endregion

    #region IComparable<DynamicLongKey> Members

    //--------------------------------------------------------------------------------
    /// <inheritdoc />
    public int CompareTo (DynamicLongKey? other) => other is null ? 1 :
        Value.CompareTo(other.Value);
    //--------------------------------------------------------------------------------

    #endregion IComparable<DynamicLongKey> Members

    #region Overrides of DynamicRefKey

    //--------------------------------------------------------------------------------
    /// <inheritdoc />
    protected internal override object ObjectValue { [DebuggerStepThrough] get => _objValue ??= Value; }
    //--------------------------------------------------------------------------------

    #endregion
}
//################################################################################
