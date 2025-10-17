// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Runtime.CompilerServices;

namespace KZDev.PerfUtils;

//################################################################################
/// <summary>
///   A high-performance, type-safe key system for scenarios requiring dynamic composition
///   of keys from multiple types and values, such as caching, dictionary lookups, and
///   composite key scenarios.
/// </summary>
/// <remarks>
///   <para>
///     <see cref="DynamicKey"/> provides a unified interface for creating keys from various
///     data types including primitives (int, long, bool, string, Guid), enums, value types,
///     reference types, and composite combinations. It implements efficient thread-static
///     caching to minimize allocations and provides optimized hash code generation and
///     comparison operations.
///   </para>
///   <para>
///     Key features include:
///   </para>
///   <list type="bullet">
///     <item>
///       <description>Type-safe key creation with compile-time generic support</description>
///     </item>
///     <item>
///       <description>Thread-static caching for improved performance</description>
///     </item>
///     <item>
///       <description>Composite key support for multi-value scenarios</description>
///     </item>
///     <item>
///       <description>Optimized hash codes and equality comparisons</description>
///     </item>
///     <item>
///       <description>Implicit conversions from common types</description>
///     </item>
///   </list>
///   <para>
///     Example usage:
///   </para>
///   <code>
///     // Single value keys
///     DynamicKey key1 = DynamicKey.GetKey(42);
///     DynamicKey key2 = DynamicKey.GetKey("user123");
///     
///     // Composite keys
///     DynamicKey composite = DynamicKey.GetKey(42, "user123", true);
///     
///     // Using as dictionary keys
///     var cache = new Dictionary&lt;DynamicKey, string&gt;();
///     cache[composite] = "cached_value";
///   </code>
/// </remarks>
/// <seealso cref="DynamicKeyBuilder"/>
/// <seealso cref="DynamicKey{T}"/>
#pragma warning disable CS0659, CS0660, CS0661
public abstract partial class DynamicKey : IEquatable<DynamicKey>, IComparable<DynamicKey>, IComparable
{
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Gets the underlying object value used for this key.
    /// </summary>
    protected internal abstract object? ObjectValue { get; }
    //--------------------------------------------------------------------------------

    #region Helper Properties

    //--------------------------------------------------------------------------------
    /// <summary>
    ///   Gets a cached instance representing the boolean value <c>true</c>.
    /// </summary>
    /// <value>
    ///   A <see cref="DynamicKey"/> instance that represents the boolean value <c>true</c>.
    /// </value>
    /// <remarks>
    ///   This property returns a thread-static cached instance for optimal performance
    ///   when working with boolean true values as keys.
    /// </remarks>
    public static DynamicKey True => DynamicBoolKey.True;
    //--------------------------------------------------------------------------------
    /// <summary>
    ///   Gets a cached instance representing the boolean value <c>false</c>.
    /// </summary>
    /// <value>
    ///   A <see cref="DynamicKey"/> instance that represents the boolean value <c>false</c>.
    /// </value>
    /// <remarks>
    ///   This property returns a thread-static cached instance for optimal performance
    ///   when working with boolean false values as keys.
    /// </remarks>
    public static DynamicKey False => DynamicBoolKey.False;
    //--------------------------------------------------------------------------------
    /// <summary>
    ///   Gets a cached instance representing the integer value <c>0</c>.
    /// </summary>
    /// <value>
    ///   A <see cref="DynamicKey"/> instance that represents the integer value <c>0</c>.
    /// </value>
    /// <remarks>
    ///   This property returns a thread-static cached instance for optimal performance
    ///   when working with zero integer values as keys.
    /// </remarks>
    public static DynamicKey Zero => DynamicIntKey.Zero;
    //--------------------------------------------------------------------------------
    /// <summary>
    ///   Gets a cached instance representing the long value <c>0L</c>.
    /// </summary>
    /// <value>
    ///   A <see cref="DynamicKey"/> instance that represents the long value <c>0L</c>.
    /// </value>
    /// <remarks>
    ///   This property returns a thread-static cached instance for optimal performance
    ///   when working with zero long values as keys.
    /// </remarks>
    public static DynamicKey ZeroLong => DynamicLongKey.Zero;
    //--------------------------------------------------------------------------------
    /// <summary>
    ///   Gets a cached instance representing the unsigned integer value <c>0U</c>.
    /// </summary>
    /// <value>
    ///   A <see cref="DynamicKey"/> instance that represents the unsigned integer value <c>0U</c>.
    /// </value>
    /// <remarks>
    ///   This property returns a thread-static cached instance for optimal performance
    ///   when working with zero unsigned integer values as keys.
    /// </remarks>
    public static DynamicKey ZeroUInt => DynamicUIntKey.Zero;
    //--------------------------------------------------------------------------------
    /// <summary>
    ///   Gets a cached instance representing the unsigned long value <c>0UL</c>.
    /// </summary>
    /// <value>
    ///   A <see cref="DynamicKey"/> instance that represents the unsigned long value <c>0UL</c>.
    /// </value>
    /// <remarks>
    ///   This property returns a thread-static cached instance for optimal performance
    ///   when working with zero unsigned long values as keys.
    /// </remarks>
    public static DynamicKey ZeroULong => DynamicULongKey.Zero;
    //--------------------------------------------------------------------------------
    /// <summary>
    ///   Gets a cached instance representing an empty string.
    /// </summary>
    /// <value>
    ///   A <see cref="DynamicKey"/> instance that represents an empty string.
    /// </value>
    /// <remarks>
    ///   This property returns a thread-static cached instance for optimal performance
    ///   when working with empty string values as keys. Null strings are also treated
    ///   as empty strings.
    /// </remarks>
    public static DynamicKey EmptyString => DynamicStringKey.Empty;
    //--------------------------------------------------------------------------------
    /// <summary>
    ///   Gets a cached instance representing the default/empty GUID value.
    /// </summary>
    /// <value>
    ///   A <see cref="DynamicKey"/> instance that represents <see cref="Guid.Empty"/>.
    /// </value>
    /// <remarks>
    ///   This property returns a thread-static cached instance for optimal performance
    ///   when working with empty GUID values as keys.
    /// </remarks>
    public static DynamicKey EmptyGuid => DynamicGuidKey.Empty;
    //--------------------------------------------------------------------------------
    /// <summary>
    ///   Gets a cached instance representing a null object reference.
    /// </summary>
    /// <value>
    ///   A <see cref="DynamicKey"/> instance that represents a null object reference.
    /// </value>
    /// <remarks>
    ///   This property returns a thread-static cached instance for optimal performance
    ///   when working with null object references as keys.
    /// </remarks>
    public static DynamicKey Null => DynamicObjectKey.Null;
    //--------------------------------------------------------------------------------

    #endregion Helper Properties

    #region Acquire Methods

    //--------------------------------------------------------------------------------
    /// <summary>
    ///   Creates a <see cref="DynamicKey"/> instance for the specified integer value.
    /// </summary>
    /// <param name="value">
    ///   The integer value to use as the key.
    /// </param>
    /// <returns>
    ///   A <see cref="DynamicKey"/> instance that represents the specified integer value.
    ///   The same instance may be returned for identical values due to thread-static caching.
    /// </returns>
    /// <remarks>
    ///   <para>
    ///     This method creates a type-safe key for integer values with optimized performance
    ///     through thread-static caching. The returned key can be used in dictionaries,
    ///     hash sets, and other collection types that require keys.
    ///   </para>
    ///   <para>
    ///     Example usage:
    ///   </para>
    ///   <code>
    ///     DynamicKey key = DynamicKey.GetKey(42);
    ///     var cache = new Dictionary&lt;DynamicKey, string&gt;();
    ///     cache[key] = "value_for_42";
    ///   </code>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DynamicKey GetKey (int value) => DynamicIntKey.GetKey(value);
    //--------------------------------------------------------------------------------
    /// <summary>
    ///   Creates a <see cref="DynamicKey"/> instance for the specified long value.
    /// </summary>
    /// <param name="value">
    ///   The long value to use as the key.
    /// </param>
    /// <returns>
    ///   A <see cref="DynamicKey"/> instance that represents the specified long value.
    ///   The same instance may be returned for identical values due to thread-static caching.
    /// </returns>
    /// <remarks>
    ///   This method creates a type-safe key for 64-bit integer values with optimized
    ///   performance through thread-static caching.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DynamicKey GetKey (long value) => DynamicLongKey.GetKey(value);
    //--------------------------------------------------------------------------------
    /// <summary>
    ///   Creates a <see cref="DynamicKey"/> instance for the specified unsigned integer value.
    /// </summary>
    /// <param name="value">
    ///   The unsigned integer value to use as the key.
    /// </param>
    /// <returns>
    ///   A <see cref="DynamicKey"/> instance that represents the specified unsigned integer value.
    ///   The same instance may be returned for identical values due to thread-static caching.
    /// </returns>
    /// <remarks>
    ///   This method creates a type-safe key for 32-bit unsigned integer values with optimized
    ///   performance through thread-static caching.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DynamicKey GetKey (uint value) => DynamicUIntKey.GetKey(value);
    //--------------------------------------------------------------------------------
    /// <summary>
    ///   Creates a <see cref="DynamicKey"/> instance for the specified unsigned long value.
    /// </summary>
    /// <param name="value">
    ///   The unsigned long value to use as the key.
    /// </param>
    /// <returns>
    ///   A <see cref="DynamicKey"/> instance that represents the specified unsigned long value.
    ///   The same instance may be returned for identical values due to thread-static caching.
    /// </returns>
    /// <remarks>
    ///   This method creates a type-safe key for 64-bit unsigned integer values with optimized
    ///   performance through thread-static caching.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DynamicKey GetKey (ulong value) => DynamicULongKey.GetKey(value);
    //--------------------------------------------------------------------------------
    /// <summary>
    ///   Creates a <see cref="DynamicKey"/> instance for the specified boolean value.
    /// </summary>
    /// <param name="value">
    ///   The boolean value to use as the key.
    /// </param>
    /// <returns>
    ///   A <see cref="DynamicKey"/> instance that represents the specified boolean value.
    ///   The same instance may be returned for identical values due to thread-static caching.
    /// </returns>
    /// <remarks>
    ///   This method creates a type-safe key for boolean values with optimized performance
    ///   through thread-static caching. Consider using <see cref="True"/> or <see cref="False"/>
    ///   properties for better performance when working with constant boolean values.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DynamicKey GetKey (bool value) => DynamicBoolKey.GetKey(value);
    //--------------------------------------------------------------------------------
    /// <summary>
    ///   Creates a <see cref="DynamicKey"/> instance for the specified GUID value.
    /// </summary>
    /// <param name="value">
    ///   The GUID value to use as the key.
    /// </param>
    /// <returns>
    ///   A <see cref="DynamicKey"/> instance that represents the specified GUID value.
    ///   The same instance may be returned for identical values due to thread-static caching.
    /// </returns>
    /// <remarks>
    ///   This method creates a type-safe key for GUID values with optimized performance
    ///   through thread-static caching. Consider using <see cref="EmptyGuid"/> for better
    ///   performance when working with <see cref="Guid.Empty"/>.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DynamicKey GetKey (Guid value) => DynamicGuidKey.GetKey(value);
    //--------------------------------------------------------------------------------
    /// <summary>
    ///   Creates a <see cref="DynamicKey"/> instance for the specified string value.
    /// </summary>
    /// <param name="value">
    ///   The string value to use as the key. If null, it will be treated as an empty string.
    /// </param>
    /// <returns>
    ///   A <see cref="DynamicKey"/> instance that represents the specified string value.
    ///   The same instance may be returned for identical values due to thread-static caching.
    /// </returns>
    /// <remarks>
    ///   <para>
    ///     This method creates a type-safe key for string values with optimized performance
    ///     through thread-static caching. Null strings are automatically converted to empty
    ///     strings for consistent behavior.
    ///   </para>
    ///   <para>
    ///     Consider using <see cref="EmptyString"/> for better performance when working
    ///     with empty or null strings.
    ///   </para>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DynamicKey GetKey (string value) => DynamicStringKey.GetKey(value);
    //--------------------------------------------------------------------------------
    /// <summary>
    ///   Creates a <see cref="DynamicKey"/> instance for the specified <see cref="Type"/> value.
    /// </summary>
    /// <param name="value">
    ///   The <see cref="Type"/> value to use as the key.
    /// </param>
    /// <returns>
    ///   A <see cref="DynamicKey"/> instance that represents the specified <see cref="Type"/> value.
    ///   The same instance may be returned for identical values due to thread-static caching.
    /// </returns>
    /// <remarks>
    ///   This method creates a type-safe key for <see cref="Type"/> instances with optimized
    ///   performance through thread-static caching. Useful for scenarios where you need to
    ///   cache or index data by type information.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DynamicKey GetKey (Type value) => DynamicTypeKey.GetKey(value);
    //--------------------------------------------------------------------------------
    /// <summary>
    ///   Creates a <see cref="DynamicKey"/> instance for the specified reference type value.
    /// </summary>
    /// <typeparam name="T">
    ///   The type of the value to be used as the key. Must be a reference type.
    /// </typeparam>
    /// <param name="value">
    ///   The reference type value to use as the key. May be null.
    /// </param>
    /// <returns>
    ///   A <see cref="DynamicKey"/> instance that represents the specified reference type value.
    ///   If <paramref name="value"/> is null, returns <see cref="Null"/>.
    /// </returns>
    /// <remarks>
    ///   <para>
    ///     This method creates a type-safe key for reference types with optimized performance
    ///     through thread-static caching. The key uses reference equality for comparison,
    ///     making it suitable for object instances where identity matters.
    ///   </para>
    ///   <para>
    ///     For better performance with generic types, consider using <see cref="DynamicKey{T}.GetKey(T)"/>
    ///     which provides additional optimizations through compiled lambda expressions.
    ///   </para>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DynamicKey GetKey<T> (T value) where T : class => DynamicRefKey<T>.GetKey(value);
    //--------------------------------------------------------------------------------
    /// <summary>
    ///   Creates a <see cref="DynamicKey"/> instance for the specified value type value.
    /// </summary>
    /// <typeparam name="T">
    ///   The type of the value to be used as the key. Must be a value type.
    /// </typeparam>
    /// <param name="value">
    ///   The value type value to use as the key.
    /// </param>
    /// <returns>
    ///   A <see cref="DynamicKey"/> instance that represents the specified value type value.
    /// </returns>
    /// <remarks>
    ///   <para>
    ///     This method creates a type-safe key for value types with optimized performance
    ///     through thread-static caching. The key uses value equality for comparison,
    ///     making it suitable for structs and other value types.
    ///   </para>
    ///   <para>
    ///     For better performance with generic types, consider using <see cref="DynamicKey{T}.GetKey(T)"/>
    ///     which provides additional optimizations through compiled lambda expressions.
    ///   </para>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DynamicKey GetValueKey<T> (T value) where T : struct => DynamicValKey<T>.GetKey(value);
    //--------------------------------------------------------------------------------
    /// <summary>
    ///   Creates a <see cref="DynamicKey"/> instance for the specified enum value.
    /// </summary>
    /// <typeparam name="TEnum">
    ///   The enum type to be used as the key.
    /// </typeparam>
    /// <param name="value">
    ///   The enum value to use as the key.
    /// </param>
    /// <returns>
    ///   A <see cref="DynamicKey"/> instance that represents the specified enum value.
    ///   The same instance may be returned for identical values due to thread-static caching.
    /// </returns>
    /// <remarks>
    ///   <para>
    ///     This method creates a type-safe key for enum values with optimized performance
    ///     through thread-static caching. The key uses the enum's underlying value for
    ///     comparison and hash code generation.
    ///   </para>
    ///   <para>
    ///     Example usage:
    ///   </para>
    ///   <code>
    ///     DynamicKey key = DynamicKey.GetEnumKey(StringComparison.OrdinalIgnoreCase);
    ///     var cache = new Dictionary&lt;DynamicKey, string&gt;();
    ///     cache[key] = "case_insensitive_value";
    ///   </code>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DynamicKey GetEnumKey<TEnum> (TEnum value) where TEnum : struct, Enum => DynamicEnumKey<TEnum>.GetKey(value);
    //--------------------------------------------------------------------------------
    /// <summary>
    ///   Returns the specified <see cref="DynamicKey"/> instance unchanged.
    /// </summary>
    /// <param name="value">
    ///   The <see cref="DynamicKey"/> instance to return.
    /// </param>
    /// <returns>
    ///   The passed <see cref="DynamicKey"/> instance.
    /// </returns>
    /// <remarks>
    ///   This method is a helper for generic templated key overloads. When a
    ///   <see cref="DynamicKey"/> instance is passed to a generic method, this overload
    ///   ensures the instance is returned unchanged rather than being wrapped in another key.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DynamicKey GetKey (DynamicKey value) => value;
    //--------------------------------------------------------------------------------
    /// <summary>
    ///   Creates a <see cref="DynamicKey"/> instance for the specified object value.
    /// </summary>
    /// <param name="value">
    ///   The object value to use as the key. May be null.
    /// </param>
    /// <returns>
    ///   A <see cref="DynamicKey"/> instance that represents the specified object value.
    ///   If <paramref name="value"/> is already a <see cref="DynamicKey"/>, it is returned unchanged.
    ///   If <paramref name="value"/> is null, returns <see cref="Null"/>.
    /// </returns>
    /// <remarks>
    ///   <para>
    ///     This method creates a key for any object type with optimized performance
    ///     through thread-static caching. The key uses the object's <see cref="object.Equals(object)"/>
    ///     and <see cref="object.GetHashCode"/> methods for comparison.
    ///   </para>
    ///   <para>
    ///     For better performance with known types, consider using the specific
    ///     <see cref="GetKey(int)"/>, <see cref="GetKey(string)"/>, etc. methods.
    ///   </para>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DynamicKey GetKey (object? value) =>
        value is DynamicKey dynamicKey
            ? dynamicKey
            : DynamicObjectKey.GetKey(value);
    //--------------------------------------------------------------------------------
    /// <summary>
    ///   Combines multiple <see cref="DynamicKey"/> instances into a single composite key.
    /// </summary>
    /// <param name="keys">
    ///   The <see cref="DynamicKey"/> instances to combine. Must contain at least one key.
    /// </param>
    /// <returns>
    ///   A composite key containing all the specified keys. If only one key is provided,
    ///   that key is returned unchanged.
    /// </returns>
    /// <exception cref="ArgumentException">
    ///   Thrown when no keys are provided (empty span).
    /// </exception>
    /// <remarks>
    ///   <para>
    ///     This method creates a composite key that combines multiple individual keys
    ///     into a single key instance. The composite key maintains the order of the
    ///     constituent keys and provides optimized hash code generation and comparison
    ///     operations.
    ///   </para>
    ///   <para>
    ///     Example usage:
    ///   </para>
    ///   <code>
    ///     DynamicKey key1 = DynamicKey.GetKey(42);
    ///     DynamicKey key2 = DynamicKey.GetKey("user");
    ///     DynamicKey composite = DynamicKey.Combine(key1, key2);
    ///     
    ///     // Or using the + operator
    ///     DynamicKey composite2 = key1 + key2;
    ///   </code>
    ///   <para>
    ///     For better performance with 2-12 parameters, consider using the generic
    ///     <see cref="GetKey{T0, T1}(T0, T1)"/> methods which provide compile-time
    ///     type safety and additional optimizations.
    ///   </para>
    /// </remarks>
    public static DynamicKey Combine (params ReadOnlySpan<DynamicKey> keys) => DynamicCompositeKey.GetKey(keys);
    //--------------------------------------------------------------------------------
    /// <summary>
    ///   Handles comparing two <see cref="DynamicKey"/> instances of (potentially) different types.
    /// </summary>
    /// <param name="other">
    ///   The other <see cref="DynamicKey"/> instance to compare against this instance.
    /// </param>
    /// <returns>
    ///   An integer that indicates the relative order of the objects being compared.
    ///   Returns a negative value if this instance is less than <paramref name="other"/>,
    ///   zero if they are equal, or a positive value if this instance is greater than
    ///   <paramref name="other"/>.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    ///   Thrown if two instances of the same derived type are being compared. Derived types
    ///   should handle comparison of instances of the same type in their own <see cref="CompareTo(DynamicKey?)"/>
    ///   implementation.
    /// </exception>
    /// <remarks>
    ///   <para>
    ///     This method provides a fallback comparison mechanism for <see cref="DynamicKey"/>
    ///     instances of different types. It uses the type names to provide a consistent
    ///     ordering when the specific types cannot be compared directly.
    ///   </para>
    ///   <para>
    ///     Derived types should override <see cref="CompareTo(DynamicKey?)"/> to handle
    ///     comparison with instances of the same type, and call this method only for
    ///     cross-type comparisons when other means are not available.
    ///   </para>
    /// </remarks>
    protected int CompareKey (DynamicKey other)
    {
        if (ReferenceEquals(other?.ObjectValue, ObjectValue))
            return 0;
        if (other?.ObjectValue is null)
            return 1;
        switch (ObjectValue)
        {
            case null:
                return -1;

            case DynamicCompositeKey compositeKey:
                if (other is DynamicCompositeKey otherComposite)
                    return compositeKey.CompareTo(otherComposite);
                break;

            case string strValue when other.ObjectValue is string stringKey:
                return string.CompareOrdinal(strValue, stringKey);

            case IComparable comparable when ObjectValue.GetType() == other.ObjectValue.GetType():
                return comparable.CompareTo(other.ObjectValue);
        }

        Type type = GetType();
        Type otherType = other.GetType();
        if (type == otherType)
            throw new InvalidOperationException($"Cannot compare two instances of the same type: {type.FullName} -- derived types of {nameof(DynamicKey)} should handle IComparable operations for keys of the same type.");
        // In lieu of using any specific ordering, we will just use the type name to
        // provide a consistent ordering, but it won't have any real meaning, and
        // we will throw if two instances of the same type (simple name) are being compared.
        string typeName = type.Name;
        string otherTypeName = otherType.Name;
        if (typeName == otherTypeName)
            throw new InvalidOperationException($"Cannot compare two instances of the same type: {typeName} -- derived types of {nameof(DynamicKey)} should handle IComparable operations for keys of the same type.");
        return string.Compare(typeName, otherTypeName, StringComparison.Ordinal);
    }
    //--------------------------------------------------------------------------------

    #endregion

    #region Equality members

    //--------------------------------------------------------------------------------
    /// <summary>
    ///   Determines whether the specified <see cref="DynamicKey"/> is equal to this instance.
    /// </summary>
    /// <param name="other">
    ///   The <see cref="DynamicKey"/> to compare with this instance.
    /// </param>
    /// <returns>
    ///   <c>true</c> if the specified <see cref="DynamicKey"/> is equal to this instance;
    ///   otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>
    ///   <para>
    ///     This method should be implemented by derived classes to provide type-specific
    ///     equality comparison logic. The implementation should consider both reference
    ///     equality (for cached instances) and value equality (for the underlying key data).
    ///   </para>
    ///   <para>
    ///     Two <see cref="DynamicKey"/> instances are considered equal if they represent
    ///     the same key value, regardless of whether they are the same object instance.
    ///   </para>
    /// </remarks>
    public abstract bool Equals (DynamicKey? other);
    //--------------------------------------------------------------------------------
    /// <summary>
    ///   Compares this instance with another <see cref="DynamicKey"/> and returns an integer
    ///   that indicates whether this instance precedes, follows, or occurs in the same position
    ///   in the sort order as the other <see cref="DynamicKey"/>.
    /// </summary>
    /// <param name="other">
    ///   The <see cref="DynamicKey"/> to compare with this instance.
    /// </param>
    /// <returns>
    ///   A value that indicates the relative order of the objects being compared.
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
    ///   <para>
    ///     This method should be implemented by derived classes to provide type-specific
    ///     comparison logic. For instances of the same type, the implementation should
    ///     compare the underlying key values. For instances of different types, the
    ///     implementation should call <see cref="CompareKey(DynamicKey)"/>.
    ///   </para>
    /// </remarks>
    public abstract int CompareTo (DynamicKey? other);
    //--------------------------------------------------------------------------------
    /// <summary>
    ///   Determines whether the specified object is equal to this instance.
    /// </summary>
    /// <param name="obj">
    ///   The object to compare with this instance.
    /// </param>
    /// <returns>
    ///   <c>true</c> if the specified object is equal to this instance; otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>
    ///   This method provides the standard <see cref="object.Equals(object)"/> implementation
    ///   and delegates to the type-safe <see cref="Equals(DynamicKey?)"/> method.
    /// </remarks>
#pragma warning disable CS0659
    public override bool Equals (object? obj)
#pragma warning restore CS0659
    {
        if (ReferenceEquals(null, obj))
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
        {
            return true;
        }
        return (obj is DynamicKey other) && Equals(other);
    }
    //--------------------------------------------------------------------------------
    /// <inheritdoc />
    public int CompareTo (object? obj)
    {
        if (ReferenceEquals(null, obj))
            return 1;
        if (ReferenceEquals(this, obj))
            return 0;
        if (obj is not DynamicKey key)
            throw new ArgumentException($"Passed object to compare must be of type {nameof(DynamicKey)}");
        return CompareTo(key);
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    ///   Determines whether two specified <see cref="DynamicKey"/> instances are equal.
    /// </summary>
    /// <param name="left">
    ///   The first <see cref="DynamicKey"/> to compare.
    /// </param>
    /// <param name="right">
    ///   The second <see cref="DynamicKey"/> to compare.
    /// </param>
    /// <returns>
    ///   <c>true</c> if <paramref name="left"/> and <paramref name="right"/> are equal;
    ///   otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>
    ///   <para>
    ///     This operator provides a convenient way to compare two <see cref="DynamicKey"/>
    ///     instances for equality. It handles null values correctly and delegates to the
    ///     <see cref="Equals(DynamicKey?)"/> method for the actual comparison logic.
    ///   </para>
    ///   <para>
    ///     Two <see cref="DynamicKey"/> instances are considered equal if they represent
    ///     the same key value, regardless of whether they are the same object instance.
    ///   </para>
    /// </remarks>
    public static bool operator == (DynamicKey? left, DynamicKey? right)
    {
        if (ReferenceEquals(left, right))
            return true;
        if (left is null || right is null)
            return false;
        return left.Equals(right);
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    ///   Determines whether two specified <see cref="DynamicKey"/> instances are not equal.
    /// </summary>
    /// <param name="left">
    ///   The first <see cref="DynamicKey"/> to compare.
    /// </param>
    /// <param name="right">
    ///   The second <see cref="DynamicKey"/> to compare.
    /// </param>
    /// <returns>
    ///   <c>true</c> if <paramref name="left"/> and <paramref name="right"/> are not equal;
    ///   otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>
    ///   <para>
    ///     This operator provides a convenient way to compare two <see cref="DynamicKey"/>
    ///     instances for inequality. It is the logical complement of the equality operator
    ///     and handles null values correctly.
    ///   </para>
    /// </remarks>
    public static bool operator != (DynamicKey? left, DynamicKey? right)
    {
        if (ReferenceEquals(left, right))
            return false;
        if (left is null || right is null)
            return true;
        return !left.Equals(right);
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    ///   Combines two <see cref="DynamicKey"/> instances into a composite key.
    /// </summary>
    /// <param name="left">
    ///   The first <see cref="DynamicKey"/> to combine.
    /// </param>
    /// <param name="right">
    ///   The second <see cref="DynamicKey"/> to combine.
    /// </param>
    /// <returns>
    ///   A composite key containing both <paramref name="left"/> and <paramref name="right"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///   Thrown when <paramref name="left"/> or <paramref name="right"/> is null.
    /// </exception>
    /// <remarks>
    ///   <para>
    ///     This operator provides a convenient way to combine two <see cref="DynamicKey"/>
    ///     instances into a single composite key. The resulting key maintains the order
    ///     of the constituent keys and provides optimized hash code generation and comparison
    ///     operations.
    ///   </para>
    ///   <para>
    ///     Example usage:
    ///   </para>
    ///   <code>
    ///     DynamicKey key1 = DynamicKey.GetKey(42);
    ///     DynamicKey key2 = DynamicKey.GetKey("user");
    ///     DynamicKey composite = key1 + key2;
    ///     
    ///     // Equivalent to:
    ///     DynamicKey composite2 = DynamicKey.Combine(key1, key2);
    ///   </code>
    ///   <para>
    ///     For better performance with multiple parameters, consider using the generic
    ///     <see cref="GetKey{T0, T1}(T0, T1)"/> methods which provide compile-time
    ///     type safety and additional optimizations.
    ///   </para>
    /// </remarks>
    public static DynamicKey operator + (DynamicKey? left, DynamicKey? right)
    {
        if (left is null)
            throw new ArgumentNullException(nameof(left));
        if (right is null)
            throw new ArgumentNullException(nameof(right));
        return DynamicCompositeKey.GetKey(left, right);
    }
    //--------------------------------------------------------------------------------

    #endregion

    #region Implicit Conversions

    /// <summary>
    ///   Implicitly converts an <see cref="int"/> value to a <see cref="DynamicKey"/>.
    /// </summary>
    /// <param name="value">
    ///   The integer value to convert.
    /// </param>
    /// <returns>
    ///   A <see cref="DynamicKey"/> instance representing the specified integer value.
    /// </returns>
    /// <remarks>
    ///   This implicit conversion allows integer values to be used directly as <see cref="DynamicKey"/>
    ///   instances in contexts where a <see cref="DynamicKey"/> is expected, such as dictionary keys.
    /// </remarks>
    public static implicit operator DynamicKey (int value) => GetKey(value);

    /// <summary>
    ///   Implicitly converts a <see cref="long"/> value to a <see cref="DynamicKey"/>.
    /// </summary>
    /// <param name="value">
    ///   The long value to convert.
    /// </param>
    /// <returns>
    ///   A <see cref="DynamicKey"/> instance representing the specified long value.
    /// </returns>
    /// <remarks>
    ///   This implicit conversion allows long values to be used directly as <see cref="DynamicKey"/>
    ///   instances in contexts where a <see cref="DynamicKey"/> is expected, such as dictionary keys.
    /// </remarks>
    public static implicit operator DynamicKey (long value) => GetKey(value);

    /// <summary>
    ///   Implicitly converts a <see cref="uint"/> value to a <see cref="DynamicKey"/>.
    /// </summary>
    /// <param name="value">
    ///   The unsigned integer value to convert.
    /// </param>
    /// <returns>
    ///   A <see cref="DynamicKey"/> instance representing the specified unsigned integer value.
    /// </returns>
    /// <remarks>
    ///   This implicit conversion allows unsigned integer values to be used directly as <see cref="DynamicKey"/>
    ///   instances in contexts where a <see cref="DynamicKey"/> is expected, such as dictionary keys.
    /// </remarks>
    public static implicit operator DynamicKey (uint value) => GetKey(value);

    /// <summary>
    ///   Implicitly converts a <see cref="ulong"/> value to a <see cref="DynamicKey"/>.
    /// </summary>
    /// <param name="value">
    ///   The unsigned long value to convert.
    /// </param>
    /// <returns>
    ///   A <see cref="DynamicKey"/> instance representing the specified unsigned long value.
    /// </returns>
    /// <remarks>
    ///   This implicit conversion allows unsigned long values to be used directly as <see cref="DynamicKey"/>
    ///   instances in contexts where a <see cref="DynamicKey"/> is expected, such as dictionary keys.
    /// </remarks>
    public static implicit operator DynamicKey (ulong value) => GetKey(value);

    /// <summary>
    ///   Implicitly converts a <see cref="bool"/> value to a <see cref="DynamicKey"/>.
    /// </summary>
    /// <param name="value">
    ///   The boolean value to convert.
    /// </param>
    /// <returns>
    ///   A <see cref="DynamicKey"/> instance representing the specified boolean value.
    /// </returns>
    /// <remarks>
    ///   This implicit conversion allows boolean values to be used directly as <see cref="DynamicKey"/>
    ///   instances in contexts where a <see cref="DynamicKey"/> is expected, such as dictionary keys.
    /// </remarks>
    public static implicit operator DynamicKey (bool value) => GetKey(value);

    /// <summary>
    ///   Implicitly converts a <see cref="Guid"/> value to a <see cref="DynamicKey"/>.
    /// </summary>
    /// <param name="value">
    ///   The GUID value to convert.
    /// </param>
    /// <returns>
    ///   A <see cref="DynamicKey"/> instance representing the specified GUID value.
    /// </returns>
    /// <remarks>
    ///   This implicit conversion allows GUID values to be used directly as <see cref="DynamicKey"/>
    ///   instances in contexts where a <see cref="DynamicKey"/> is expected, such as dictionary keys.
    /// </remarks>
    public static implicit operator DynamicKey (Guid value) => GetKey(value);

    /// <summary>
    ///   Implicitly converts a <see cref="string"/> value to a <see cref="DynamicKey"/>.
    /// </summary>
    /// <param name="value">
    ///   The string value to convert. If null, it will be treated as an empty string.
    /// </param>
    /// <returns>
    ///   A <see cref="DynamicKey"/> instance representing the specified string value.
    /// </returns>
    /// <remarks>
    ///   This implicit conversion allows string values to be used directly as <see cref="DynamicKey"/>
    ///   instances in contexts where a <see cref="DynamicKey"/> is expected, such as dictionary keys.
    ///   Null strings are automatically converted to empty strings for consistent behavior.
    /// </remarks>
    public static implicit operator DynamicKey (string value) => GetKey(value);

    /// <summary>
    ///   Implicitly converts a <see cref="Type"/> value to a <see cref="DynamicKey"/>.
    /// </summary>
    /// <param name="value">
    ///   The Type value to convert.
    /// </param>
    /// <returns>
    ///   A <see cref="DynamicKey"/> instance representing the specified Type value.
    /// </returns>
    /// <remarks>
    ///   This implicit conversion allows Type values to be used directly as <see cref="DynamicKey"/>
    ///   instances in contexts where a <see cref="DynamicKey"/> is expected, such as dictionary keys.
    ///   Useful for scenarios where you need to cache or index data by type information.
    /// </remarks>
    public static implicit operator DynamicKey (Type value) => GetKey(value);

    #endregion
}
#pragma warning restore CS0659, CS0660, CS0661
//################################################################################