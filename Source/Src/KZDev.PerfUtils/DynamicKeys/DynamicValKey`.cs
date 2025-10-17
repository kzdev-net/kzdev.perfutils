using System;
using System.Diagnostics;

namespace KZDev.PerfUtils;

//################################################################################
/// <summary>
///   A specialized <see cref="DynamicKey"/> implementation for value types that provides
///   efficient key generation and comparison for struct types.
/// </summary>
/// <typeparam name="T">
///   The value type to use as the key. Must be a struct.
/// </typeparam>
/// <remarks>
///   <para>
///     This class provides optimized key generation for value types, with built-in support
///     for common primitive types (bool, int, long, uint, ulong, Guid) that delegate to
///     their specialized implementations for maximum performance.
///   </para>
///   <para>
///     For other value types, this class uses thread-static caching to avoid creating
///     multiple instances for the same value on the same thread, improving performance
///     in scenarios where the same key is accessed repeatedly.
///   </para>
///   <para>
///     The class implements <see cref="IComparable{T}"/> and provides efficient comparison
///     operations for value types that implement comparison interfaces.
///   </para>
/// </remarks>
/// <example>
///   <code>
///     // Create keys for different value types
///     var dateKey = DynamicValKey&lt;DateTime&gt;.GetKey(DateTime.Now);
///     var decimalKey = DynamicValKey&lt;decimal&gt;.GetKey(123.45m);
///     var customStructKey = DynamicValKey&lt;MyStruct&gt;.GetKey(new MyStruct { Value = 42 });
///   </code>
/// </example>
[DebuggerDisplay("{" + nameof(DisplayValue) + "}")]
internal sealed class DynamicValKey<T> : DynamicKey, IComparable<DynamicValKey<T>>
    where T : struct
{
    /// <summary>
    ///   When not null, this is a cached instance used to avoid creating multiple
    ///   instances of this class for the same value on the same thread.
    /// </summary>
    /// <remarks>
    ///   This thread-static field provides performance optimization by caching the most
    ///   recently created instance for the current thread, avoiding unnecessary allocations
    ///   when the same value is accessed repeatedly.
    /// </remarks>
    [ThreadStatic] private static DynamicValKey<T>? _cachedInstance;

    //--------------------------------------------------------------------------------
    /// <summary>
    ///   Gets the debugger display value for this key instance.
    /// </summary>
    /// <value>
    ///   A string representation of the key value, suitable for debugging display.
    /// </value>
    private string DisplayValue => ToString();

    /// <summary>
    /// The object representation of the value type used for this instance.
    /// </summary>
    private object? _objValue;

    /// <summary>
    ///   Gets the value used as the key for this instance.
    /// </summary>
    /// <value>
    ///   The value of type <typeparamref name="T"/> that serves as the key.
    /// </value>
    public T Value { [DebuggerStepThrough] get; }

    //--------------------------------------------------------------------------------
    /// <summary>
    ///   Initializes a new instance of the <see cref="DynamicValKey{T}"/> class.
    /// </summary>
    /// <param name="value">
    ///   The value to use as a key for this instance.
    /// </param>
    private DynamicValKey (in T value)
    {
        Value = value;
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    ///   Gets a <see cref="DynamicKey"/> instance for the given value, using thread-static
    ///   caching for performance optimization.
    /// </summary>
    /// <param name="value">
    ///   The value to use as a key.
    /// </param>
    /// <returns>
    ///   An instance of <see cref="DynamicKey"/> that uses the specified value to
    ///   represent the key or partial key.
    /// </returns>
    /// <remarks>
    ///   This method uses thread-static caching to avoid creating multiple instances
    ///   for the same value on the same thread, improving performance in scenarios
    ///   where the same key is accessed repeatedly.
    /// </remarks>
    private static DynamicKey GetKeyInternal (in T value)
    {
        DynamicValKey<T>? cachedInstance = _cachedInstance;
        if ((cachedInstance is not null) && (cachedInstance.Value.Equals(value)))
            return cachedInstance;
        DynamicValKey<T> returnInstance = new(value);
        _cachedInstance = returnInstance;
        return returnInstance;
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    ///   Gets a <see cref="DynamicKey"/> instance for the given value type.
    /// </summary>
    /// <param name="value">
    ///   The value to use as a key.
    /// </param>
    /// <returns>
    ///   An instance of <see cref="DynamicKey"/> that uses the specified value to 
    ///   represent the key or partial key.
    /// </returns>
    /// <remarks>
    ///   <para>
    ///     This method provides optimized key generation for value types. For common
    ///     primitive types (bool, int, long, uint, ulong, Guid), it delegates to their
    ///     specialized implementations for maximum performance.
    ///   </para>
    ///   <para>
    ///     For other value types, it uses the internal caching mechanism to avoid
    ///     unnecessary allocations.
    ///   </para>
    /// </remarks>
    public static DynamicKey GetKey (in T value)
    {
        return value switch
        {
            bool boolValue => DynamicBoolKey.GetKey(boolValue),
            int intValue => DynamicIntKey.GetKey(intValue),
            long longValue => DynamicLongKey.GetKey(longValue),
            uint uintValue => DynamicUIntKey.GetKey(uintValue),
            ulong ulongValue => DynamicULongKey.GetKey(ulongValue),
            Guid guidValue => DynamicGuidKey.GetKey(guidValue),
            _ => GetKeyInternal(value)
        };
    }
    //--------------------------------------------------------------------------------

    #region Overrides of Object

    //--------------------------------------------------------------------------------
    /// <summary>
    ///   Returns the hash code for this value type key.
    /// </summary>
    /// <returns>
    ///   A hash code for the current value.
    /// </returns>
    /// <remarks>
    ///   The hash code is computed using the value's own <see cref="object.GetHashCode()"/>
    ///   implementation, ensuring consistent hashing behavior.
    /// </remarks>
    public override int GetHashCode () => Value.GetHashCode();
    //--------------------------------------------------------------------------------
    /// <summary>
    ///   Returns a string representation of this value type key.
    /// </summary>
    /// <returns>
    ///   A string representation of the value, or an empty string if the value's
    ///   <see cref="object.ToString()"/> method returns null.
    /// </returns>
    public override string ToString () => Value.ToString() ?? string.Empty;
    //--------------------------------------------------------------------------------

    #endregion Overrides of Object

    #region Overrides of DynamicKey

    //--------------------------------------------------------------------------------
    /// <inheritdoc />
    public override bool Equals (DynamicKey? other) =>
        ((other is DynamicValKey<T> valDynamicKey) &&
         (ReferenceEquals(this, valDynamicKey) || EqualityComparer<T>.Default.Equals(valDynamicKey.Value, Value))) ||
        ((other is DynamicCompositeKey compositeKey) && compositeKey.Equals(this)) ||
        ((other?.ObjectValue is DynamicCompositeKey otherCompositeKey) && otherCompositeKey.Equals(this)) ||
        Equals(other?.ObjectValue, ObjectValue);
    //--------------------------------------------------------------------------------
    /// <inheritdoc />
    public override int CompareTo (DynamicKey? other) =>
        other switch
        {
            null => 1,
            DynamicValKey<T> otherKey => CompareTo(otherKey),
            _ => CompareKey(other)
        };
    //--------------------------------------------------------------------------------

    #endregion

    #region IComparable<DynamicValKey<T>> Members

    //--------------------------------------------------------------------------------
    /// <summary>
    ///   Compares this value type key with another value type key of the same type.
    /// </summary>
    /// <param name="other">
    ///   The other value type key to compare with this instance.
    /// </param>
    /// <returns>
    ///   A value indicating the relative order of the keys being compared.
    ///   The return value has these meanings:
    ///   <list type="table">
    ///     <listheader>
    ///       <term>Value</term>
    ///       <description>Meaning</description>
    ///     </listheader>
    ///     <item>
    ///       <term>Less than zero</term>
    ///       <description>This instance precedes <paramref name="other"/> in the sort order.</description>
    ///     </item>
    ///     <item>
    ///       <term>Zero</term>
    ///       <description>This instance occurs in the same position in the sort order as <paramref name="other"/>.</description>
    ///     </item>
    ///     <item>
    ///       <term>Greater than zero</term>
    ///       <description>This instance follows <paramref name="other"/> in the sort order.</description>
    ///     </item>
    ///   </list>
    /// </returns>
    /// <exception cref="InvalidOperationException">
    ///   Thrown when the value type <typeparamref name="T"/> does not implement
    ///   <see cref="IComparable{T}"/> or <see cref="IComparable"/>.
    /// </exception>
    /// <remarks>
    ///   This method attempts to use the value's comparison capabilities in the following order:
    ///   <list type="number">
    ///     <item><description><see cref="IComparable{T}"/> implementation</description></item>
    ///     <item><description><see cref="IComparable"/> implementation</description></item>
    ///     <item><description>Throws <see cref="InvalidOperationException"/> if neither is available</description></item>
    ///   </list>
    /// </remarks>
    public int CompareTo (DynamicValKey<T>? other) =>
        Value switch
        {
            IComparable<T> comparableT => comparableT.CompareTo(other?.Value ?? default),
            IComparable comparable => comparable.CompareTo(other?.Value ?? default),
            _ => other?.Value switch
            {
                IComparable<T> otherComparableT => (otherComparableT.CompareTo(Value) * (-1)),
                IComparable otherComparable => (otherComparable.CompareTo(Value) * (-1)),
                _ => throw new InvalidOperationException(
                    $"DynamicValKey<{typeof(T)}> can't be compared because the generic type {typeof(T)} is not comparable")
            }
        };
    //--------------------------------------------------------------------------------

    #endregion IComparable<DynamicValKey<T>> Members

    #region Overrides of DynamicRefKey

    //--------------------------------------------------------------------------------
    /// <inheritdoc />
    protected internal override object ObjectValue { [DebuggerStepThrough] get => _objValue ??= Value; }
    //--------------------------------------------------------------------------------

    #endregion
}
//################################################################################
