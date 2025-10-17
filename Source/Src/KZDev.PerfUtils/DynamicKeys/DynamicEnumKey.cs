// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Diagnostics;

namespace KZDev.PerfUtils;

//################################################################################
/// <summary>
///   A type of <see cref="DynamicKey"/> that uses an enum value as the key.
/// </summary>
/// <typeparam name="TEnum">
///   The enum type to be used as the key. Must be a value type that implements <see cref="Enum"/>.
/// </typeparam>
/// <remarks>
///   <para>
///     <see cref="DynamicEnumKey{TEnum}"/> provides an optimized implementation for enum-based keys
///     with thread-static caching for improved performance. It uses the enum's underlying value
///     for hash code generation and comparison operations, avoiding boxing overhead.
///   </para>
///   <para>
///     Key features include:
///   </para>
///   <list type="bullet">
///     <item>
///       <description>Thread-static caching for frequently used enum values</description>
///     </item>
///     <item>
///       <description>Zero-boxing performance using underlying enum values</description>
///     </item>
///     <item>
///       <description>Optimized equality and comparison operations</description>
///     </item>
///     <item>
///       <description>Special handling for default enum values with cached instance</description>
///     </item>
///   </list>
///   <para>
///     This class is used internally by the <see cref="DynamicKey"/> system and is not
///     typically instantiated directly by user code. Instead, use <see cref="DynamicKey.GetEnumKey{TEnum}(TEnum)"/>
///     to create enum-based keys.
///   </para>
/// </remarks>
/// <seealso cref="DynamicKey"/>
[DebuggerDisplay("{" + nameof(DisplayValue) + "}")]
internal sealed class DynamicEnumKey<TEnum> : DynamicKey, IComparable<DynamicEnumKey<TEnum>>
    where TEnum : struct, Enum
{
    /// <summary>
    /// When not null, this is a cached instance used to avoid creating multiple
    /// instances of this class for the same enum value on the same thread.
    /// </summary>
    [ThreadStatic] private static DynamicEnumKey<TEnum>? _cachedInstance;

    /// <summary>
    /// Gets the debugger display value.
    /// </summary>
    /// <value>
    /// The debugger display value.
    /// </value>
    private string DisplayValue => ToString();

    /// <summary>
    /// The object representation of the enum value used for this instance.
    /// </summary>
    private object? _objValue;

    /// <summary>
    /// The enum value used as the key value for this instance.
    /// </summary>
    public TEnum Value { [DebuggerStepThrough] get; }

    //--------------------------------------------------------------------------------
    /// <summary>
    /// Returns a common cached instance used for default enum values.
    /// </summary>
    public static DynamicEnumKey<TEnum> Default { [DebuggerStepThrough] get; } = new(default);
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Initializes a new instance of the <see cref="DynamicEnumKey{TEnum}"/> class.
    /// </summary>
    /// <param name="value">
    /// The enum value to use as a key for this instance.
    /// </param>
    private DynamicEnumKey (TEnum value)
    {
        Value = value;
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Gets a <see cref="DynamicKey"/> instance for the given enum value.
    /// </summary>
    /// <param name="value">
    /// The enum value to use as a key.
    /// </param>
    /// <returns>
    /// An instance of <see cref="DynamicKey"/> that uses the specified enum value to
    /// represent the key or partial key.
    /// </returns>
    public static DynamicKey GetKey (TEnum value)
    {
        if (EqualityComparer<TEnum>.Default.Equals(value, default))
            return Default;
        DynamicEnumKey<TEnum>? cachedInstance = _cachedInstance;
        if ((cachedInstance is not null) && (EqualityComparer<TEnum>.Default.Equals(cachedInstance.Value, value)))
            return cachedInstance;
        DynamicEnumKey<TEnum> returnInstance = new(value);
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
        ((other is DynamicEnumKey<TEnum> enumDynamicKey) &&
         (ReferenceEquals(this, enumDynamicKey) || EqualityComparer<TEnum>.Default.Equals(enumDynamicKey.Value, Value))) ||
        ((other is DynamicCompositeKey compositeKey) && compositeKey.Equals(this)) ||
        ((other?.ObjectValue is DynamicCompositeKey otherCompositeKey) && otherCompositeKey.Equals(this)) ||
        Equals(other?.ObjectValue, ObjectValue);
    //--------------------------------------------------------------------------------
    /// <inheritdoc />
    public override int CompareTo (DynamicKey? other) =>
        other switch
        {
            null => 1,
            DynamicEnumKey<TEnum> enumDynamicKey => CompareTo(enumDynamicKey),
            _ => CompareKey(other)
        };
    //--------------------------------------------------------------------------------

    #endregion

    #region IComparable<DynamicEnumKey<TEnum>> Members

    //--------------------------------------------------------------------------------
    /// <inheritdoc />
    public int CompareTo (DynamicEnumKey<TEnum>? other) => other is null ? 1 :
        Value.CompareTo(other.Value);
    //--------------------------------------------------------------------------------

    #endregion IComparable<DynamicEnumKey<TEnum>> Members

    #region Overrides of DynamicRefKey

    //--------------------------------------------------------------------------------
    /// <inheritdoc />
    protected internal override object ObjectValue { [DebuggerStepThrough] get => _objValue ??= Value; }
    //--------------------------------------------------------------------------------

    #endregion
}
//################################################################################
