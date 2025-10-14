// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Runtime.CompilerServices;

namespace KZDev.PerfUtils;

//################################################################################
/// <summary>
/// A general type that can be used as a key for scenarios where a dynamic number and
/// type of keys are needed, such as caching scenarios.
/// </summary>
/// <remarks>
/// <para>
/// Derived types of this class handle specific types of keys, such as
/// <see cref="DynamicBoolKey"/>, <see cref="DynamicIntKey"/>, 
/// <see cref="DynamicStringKey"/>, <see cref="DynamicGuidKey"/>, etc.
/// </para>
/// <para>
/// Each derived class should implement <see cref="object.GetHashCode"/>
/// </para>
/// </remarks>
#pragma warning disable CS0659, CS0660, CS0661
public abstract partial class DynamicKey : IEquatable<DynamicKey>, IComparable<DynamicKey>, IComparable
{
    #region Helper Properties

    //--------------------------------------------------------------------------------
    /// <summary>
    /// Returns a common instance used for <c>true</c> boolean values
    /// </summary>
    public static DynamicKey True => DynamicBoolKey.True;
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Returns a common instance used for <c>false</c> boolean values
    /// </summary>
    public static DynamicKey False => DynamicBoolKey.False;
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Returns a common instance used for <c>zero</c> integer values
    /// </summary>
    public static DynamicKey Zero => DynamicIntKey.Zero;
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Returns a common instance used for <c>empty</c> string values
    /// </summary>
    public static DynamicKey EmptyString => DynamicStringKey.Empty;
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Returns a common instance used for default/empty GUID values
    /// </summary>
    public static DynamicKey EmptyGuid => DynamicGuidKey.Empty;
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Returns a common instance used for null object reference values
    /// </summary>
    public static DynamicKey Null => DynamicObjectKey.Null;
    //--------------------------------------------------------------------------------

    #endregion Helper Properties

    #region Acquire Methods

    //--------------------------------------------------------------------------------
    /// <summary>
    /// Acquires an integer based dynamic key with the specified value.
    /// </summary>
    /// <param name="value">
    /// The integer value assigned to the returned key instance.
    /// </param>
    /// <returns>
    /// An instance of <see cref="DynamicKey"/> that uses the specified integer value to
    /// represent the key or partial key.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DynamicKey GetKey (int value) => DynamicIntKey.GetKey(value);
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Acquires a boolean based dynamic key with the specified value.
    /// </summary>
    /// <param name="value">
    /// The boolean value assigned to the returned key instance.
    /// </param>
    /// <returns>
    /// An instance of <see cref="DynamicKey"/> that uses the specified boolean value to
    /// represent the key or partial key.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DynamicKey GetKey (bool value) => DynamicBoolKey.GetKey(value);
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Acquires a Guid based dynamic key with the specified value.
    /// </summary>
    /// <param name="value">
    /// The Guid value assigned to the returned key instance.
    /// </param>
    /// <returns>
    /// An instance of <see cref="DynamicKey"/> that uses the specified Guid value to
    /// represent the key or partial key.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DynamicKey GetKey (Guid value) => DynamicGuidKey.GetKey(value);
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Acquires a string based dynamic key with the specified value.
    /// </summary>
    /// <param name="value">
    /// The string value assigned to the returned key instance.
    /// </param>
    /// <returns>
    /// An instance of <see cref="DynamicKey"/> that uses the specified string value to
    /// represent the key or partial key.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DynamicKey GetKey (string value) => DynamicStringKey.GetKey(value);
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Acquires a string based dynamic key with the specified value.
    /// </summary>
    /// <param name="value">
    /// The <see cref="Type"/> value assigned to the returned key instance.
    /// </param>
    /// <returns>
    /// An instance of <see cref="DynamicKey"/> that uses the specified <see cref="Type"/> value to
    /// represent the key or partial key.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DynamicKey GetKey (Type value) => DynamicTypeKey.GetKey(value);
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Acquires a generic type based dynamic key with the specified reference type value.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the value to be used as the key. Must be a reference type.
    /// </typeparam>
    /// <param name="value">
    /// The <see cref="value"/> value assigned to the returned key instance.
    /// </param>
    /// <returns>
    /// An instance of <see cref="DynamicKey"/> that uses the specified <typeparamref name="T"/> value to
    /// represent the key or partial key.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DynamicKey GetKey<T> (T value) where T : class => DynamicRefKey<T>.GetKey(value);
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Acquires a generic type based dynamic key with the specified value type value.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the value to be used as the key. Must be a value type.
    /// </typeparam>
    /// <param name="value">
    /// The <see cref="value"/> value assigned to the returned key instance.
    /// </param>
    /// <returns>
    /// An instance of <see cref="DynamicKey"/> that uses the specified <typeparamref name="T"/> value to
    /// represent the key or partial key.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DynamicKey GetValueKey<T> (T value) where T : struct => DynamicValKey<T>.GetKey(value);
    //--------------------------------------------------------------------------------
    /// <summary>
    /// A helper for the generic templated key overloads when passed an instance of the
    /// <see cref="DynamicKey"/> will call this single parameter overload, knowing that 
    /// the passed <paramref name="value"/> is a <see cref="DynamicKey"/> instance; we 
    /// can therefore just return it.
    /// </summary>
    /// <param name="value">
    /// The <see cref="DynamicKey"/> instance to return.
    /// </param>
    /// <returns>
    /// The passed <see cref="DynamicKey"/> instance.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DynamicKey GetKey (DynamicKey value) => value;
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Acquires an object based dynamic key.
    /// </summary>
    /// <param name="value">
    /// The object value assigned to the returned key instance.
    /// </param>
    /// <returns>
    /// An instance of <see cref="DynamicKey"/> that uses the specified object value to
    /// represent the key or partial key.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DynamicKey GetKey (object? value) =>
        value is DynamicKey dynamicKey
            ? dynamicKey
            : DynamicObjectKey.GetKey(value);
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Handles comparing two <see cref="DynamicKey"/> instances of (potentially) different types.
    /// </summary>
    /// <param name="other">
    /// The other <see cref="DynamicKey"/> instance to compare against this instance.
    /// </param>
    /// <returns>
    /// An integer that indicates the relative order of the objects being compared.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if two instances of the same derived type are being compared.
    /// </exception>
    protected int CompareKey (DynamicKey other)
    {
        // In lieu of using any specific ordering, we will just use the type name to
        // provide a consistent ordering.
        string typeName = GetType().Name;
        string otherTypeName = other.GetType().Name;
        if (typeName == otherTypeName)
            throw new InvalidOperationException($"Cannot compare two instances of the same type: {typeName} -- derived types of {nameof(DynamicKey)} should handle IComparable operations for keys of the same type.");
        return string.Compare(typeName, otherTypeName, StringComparison.Ordinal);
    }
    //--------------------------------------------------------------------------------

    #endregion

    #region Equality members

    //--------------------------------------------------------------------------------
    /// <inheritdoc />
    public abstract bool Equals (DynamicKey? other);
    //--------------------------------------------------------------------------------
    /// <inheritdoc />
    public abstract int CompareTo (DynamicKey? other);
    //--------------------------------------------------------------------------------
    /// <inheritdoc />
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
    /// Equality operator for two <see cref="DynamicKey"/> instances
    /// </summary>
    /// <param name="left">
    /// The left side of the equality operator.
    /// </param>
    /// <param name="right">
    /// The right side of the equality operator.
    /// </param>
    /// <returns>
    /// <c>true</c> if the two instances are equal; otherwise, <c>false</c>.
    /// </returns>
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
    /// Inequality operator for two <see cref="DynamicKey"/> instances
    /// </summary>
    /// <param name="left">
    /// The left side of the inequality operator.
    /// </param>
    /// <param name="right">
    /// The right side of the inequality operator.
    /// </param>
    /// <returns>
    /// <c>true</c> if the two instances are not equal; otherwise, <c>false</c>.
    /// </returns>
    public static bool operator != (DynamicKey? left, DynamicKey? right)
    {
        if (ReferenceEquals(left, right))
            return false;
        if (left is null || right is null)
            return true;
        return !left.Equals(right);
    }
    //--------------------------------------------------------------------------------

    #endregion

    #region Implicit Conversions

    /// <summary>
    /// Implicit conversion operator for <see cref="int"/> to <see cref="DynamicKey"/>
    /// </summary>
    /// <param name="value"></param>
    public static implicit operator DynamicKey (int value) => GetKey(value);

    /// <summary>
    /// Implicit conversion operator for <see cref="bool"/> to <see cref="DynamicKey"/>
    /// </summary>
    /// <param name="value"></param>
    public static implicit operator DynamicKey (bool value) => GetKey(value);

    /// <summary>
    /// Implicit conversion operator for <see cref="Guid"/> to <see cref="DynamicKey"/>
    /// </summary>
    /// <param name="value"></param>
    public static implicit operator DynamicKey (Guid value) => GetKey(value);

    /// <summary>
    /// Implicit conversion operator for <see cref="string"/> to <see cref="DynamicKey"/>
    /// </summary>
    /// <param name="value"></param>
    public static implicit operator DynamicKey (string value) => GetKey(value);

    /// <summary>
    /// Implicit conversion operator for <see cref="Type"/> to <see cref="DynamicKey"/>
    /// </summary>
    /// <param name="value"></param>
    public static implicit operator DynamicKey (Type value) => GetKey(value);

    #endregion
}
#pragma warning restore CS0659, CS0660, CS0661
//################################################################################