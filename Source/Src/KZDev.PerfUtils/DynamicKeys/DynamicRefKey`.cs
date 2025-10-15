// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Diagnostics;

namespace KZDev.PerfUtils;

//################################################################################
/// <summary>
///   A specialized <see cref="DynamicKey"/> implementation for reference types that provides
///   efficient key generation and comparison for class types.
/// </summary>
/// <typeparam name="T">
///   The reference type to use as the key. Must be a class.
/// </typeparam>
/// <remarks>
///   <para>
///     This class provides optimized key generation for reference types, with built-in support
///     for common reference types (string, Type) that delegate to their specialized
///     implementations for maximum performance.
///   </para>
///   <para>
///     For other reference types, this class uses thread-static caching to avoid creating
///     multiple instances for the same reference on the same thread, improving performance
///     in scenarios where the same key is accessed repeatedly.
///   </para>
///   <para>
///     The class implements <see cref="IComparable{T}"/> and provides efficient comparison
///     operations for reference types that implement comparison interfaces.
///   </para>
///   <para>
///     Null values are handled gracefully and return the <see cref="DynamicKey.Null"/> instance.
///   </para>
/// </remarks>
/// <example>
///   <code>
///     // Create keys for different reference types
///     var stringKey = DynamicRefKey&lt;string&gt;.GetKey("Hello");
///     var typeKey = DynamicRefKey&lt;Type&gt;.GetKey(typeof(string));
///     var customClassKey = DynamicRefKey&lt;MyClass&gt;.GetKey(new MyClass { Value = "Test" });
///     var nullKey = DynamicRefKey&lt;string&gt;.GetKey(null); // Returns DynamicKey.Null
///   </code>
/// </example>
[DebuggerDisplay("{" + nameof(DisplayValue) + "}")]
internal sealed class DynamicRefKey<T> : DynamicKey, IComparable<DynamicRefKey<T>>
    where T : class
{
    /// <summary>
    ///   When not null, this is a cached instance used to avoid creating multiple
    ///   instances of this class for the same reference on the same thread.
    /// </summary>
    /// <remarks>
    ///   This thread-static field provides performance optimization by caching the most
    ///   recently created instance for the current thread, avoiding unnecessary allocations
    ///   when the same reference is accessed repeatedly. The caching uses reference equality
    ///   to ensure that only the exact same object instance is cached.
    /// </remarks>
    [ThreadStatic] private static DynamicRefKey<T>? _cachedInstance;

    //--------------------------------------------------------------------------------
    /// <summary>
    ///   Gets the debugger display value for this key instance.
    /// </summary>
    /// <value>
    ///   A string representation of the key value, suitable for debugging display.
    /// </value>
    private string DisplayValue => ToString();

    /// <summary>
    ///   Gets the value used as the key for this instance.
    /// </summary>
    /// <value>
    ///   The value of type <typeparamref name="T"/> that serves as the key.
    /// </value>
    public T Value { [DebuggerStepThrough] get; }

    //--------------------------------------------------------------------------------
    /// <summary>
    ///   Initializes a new instance of the <see cref="DynamicRefKey{T}"/> class.
    /// </summary>
    /// <param name="value">
    ///   The value to use as a key for this instance.
    /// </param>
    private DynamicRefKey (T value)
    {
        Value = value;
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    ///   Gets a <see cref="DynamicKey"/> instance for the given reference, using thread-static
    ///   caching for performance optimization.
    /// </summary>
    /// <param name="value">
    ///   The reference to use as a key.
    /// </param>
    /// <returns>
    ///   An instance of <see cref="DynamicKey"/> that uses the specified reference to
    ///   represent the key or partial key.
    /// </returns>
    /// <remarks>
    ///   This method uses thread-static caching to avoid creating multiple instances
    ///   for the same reference on the same thread, improving performance in scenarios
    ///   where the same key is accessed repeatedly. The caching uses reference equality
    ///   to ensure that only the exact same object instance is cached.
    /// </remarks>
    private static DynamicKey GetKeyInternal (T value)
    {
        DynamicRefKey<T>? cachedInstance = _cachedInstance;
        // For the cached instance of a reference type, we use ReferenceEquals just in case the object         
        // overrides Equals to do a value comparison instead of a reference comparison, and
        // we want to be sure we are only returning the cached instance if it is actually
        // the same instance.
        if ((cachedInstance is not null) && ReferenceEquals(cachedInstance.Value, value))
            return cachedInstance;
        DynamicRefKey<T> returnInstance = new(value);
        _cachedInstance = returnInstance;
        return returnInstance;
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    ///   Gets a <see cref="DynamicKey"/> instance for the given reference type.
    /// </summary>
    /// <param name="value">
    ///   The reference to use as a key. Can be null.
    /// </param>
    /// <returns>
    ///   An instance of <see cref="DynamicKey"/> that uses the specified reference to 
    ///   represent the key or partial key. Returns <see cref="DynamicKey.Null"/> if the
    ///   value is null.
    /// </returns>
    /// <remarks>
    ///   <para>
    ///     This method provides optimized key generation for reference types. For common
    ///     reference types (string, Type), it delegates to their specialized implementations
    ///     for maximum performance.
    ///   </para>
    ///   <para>
    ///     If the value is a <see cref="DynamicKey"/> instance, it is returned directly.
    ///     For other reference types, it uses the internal caching mechanism to avoid
    ///     unnecessary allocations.
    ///   </para>
    ///   <para>
    ///     Null values are handled gracefully and return the <see cref="DynamicKey.Null"/>
    ///     instance.
    ///   </para>
    /// </remarks>
    public static DynamicKey GetKey (T? value)
    {
        if (value is null)
            return Null;
        return value switch
        {
            string stringValue => DynamicStringKey.GetKey(stringValue),
            Type typeValue => DynamicTypeKey.GetKey(typeValue),
            _ => value is DynamicKey dynamicKey ? dynamicKey : GetKeyInternal(value)
        };
    }
    //--------------------------------------------------------------------------------

    #region Overrides of Object

    //--------------------------------------------------------------------------------
    /// <summary>
    ///   Returns the hash code for this reference type key.
    /// </summary>
    /// <returns>
    ///   A hash code for the current reference value.
    /// </returns>
    /// <remarks>
    ///   The hash code is computed using the reference's own <see cref="object.GetHashCode()"/>
    ///   implementation, ensuring consistent hashing behavior.
    /// </remarks>
    public override int GetHashCode () => Value!.GetHashCode();
    //--------------------------------------------------------------------------------
    /// <summary>
    ///   Returns a string representation of this reference type key.
    /// </summary>
    /// <returns>
    ///   A string representation of the reference value, or an empty string if the
    ///   reference's <see cref="object.ToString()"/> method returns null.
    /// </returns>
    public override string ToString () => Value?.ToString() ?? string.Empty;
    //--------------------------------------------------------------------------------

    #endregion Overrides of Object

    #region Overrides of DynamicKey

    //--------------------------------------------------------------------------------
    /// <summary>
    ///   Determines whether this reference type key is equal to another dynamic key.
    /// </summary>
    /// <param name="other">
    ///   The other dynamic key to compare with this instance.
    /// </param>
    /// <returns>
    ///   <see langword="true"/> if the other key is a <see cref="DynamicRefKey{T}"/>
    ///   with the same reference; otherwise, <see langword="false"/>.
    /// </returns>
    /// <remarks>
    ///   Equality is determined by comparing the references using the default equality
    ///   comparer for type <typeparamref name="T"/>.
    /// </remarks>
    public override bool Equals (DynamicKey? other) =>
        (other is DynamicRefKey<T> objectCacheKey) &&
        EqualityComparer<T>.Default.Equals(Value!, objectCacheKey.Value!);
    //--------------------------------------------------------------------------------
    /// <summary>
    ///   Compares this reference type key with another dynamic key.
    /// </summary>
    /// <param name="other">
    ///   The other dynamic key to compare with this instance.
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
    /// <remarks>
    ///   If the other key is also a <see cref="DynamicRefKey{T}"/>, the references are compared
    ///   directly. Otherwise, the base class comparison logic is used.
    /// </remarks>
    public override int CompareTo (DynamicKey? other) =>
        other switch
        {
            null => 1,
            DynamicRefKey<T> otherKey => CompareTo(otherKey),
            _ => CompareKey(other)
        };
    //--------------------------------------------------------------------------------

    #endregion

    #region IComparable<DynamicRefKey<T>> Members

    //--------------------------------------------------------------------------------
    /// <summary>
    ///   Compares this reference type key with another reference type key of the same type.
    /// </summary>
    /// <param name="other">
    ///   The other reference type key to compare with this instance.
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
    ///   Thrown when the reference type <typeparamref name="T"/> does not implement
    ///   <see cref="IComparable{T}"/> or <see cref="IComparable"/>.
    /// </exception>
    /// <remarks>
    ///   This method attempts to use the reference's comparison capabilities in the following order:
    ///   <list type="number">
    ///     <item><description><see cref="IComparable{T}"/> implementation</description></item>
    ///     <item><description><see cref="IComparable"/> implementation</description></item>
    ///     <item><description>Throws <see cref="InvalidOperationException"/> if neither is available</description></item>
    ///   </list>
    /// </remarks>
    public int CompareTo (DynamicRefKey<T>? other) =>
        Value switch
        {
            IComparable<T> comparableT => comparableT.CompareTo(other?.Value),
            IComparable comparable => comparable.CompareTo(other?.Value),
            _ => other?.Value switch
            {
                IComparable<T> otherComparableT => (otherComparableT.CompareTo(Value) * (-1)),
                IComparable otherComparable => (otherComparable.CompareTo(Value) * (-1)),
                _ => throw new InvalidOperationException(
                    $"DynamicRefKey<{typeof(T)}> can't be compared because the generic type {typeof(T)} is not comparable")
            }
        };
    //--------------------------------------------------------------------------------

    #endregion IComparable<DynamicRefKey<T>> Members
}
//################################################################################