// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Linq.Expressions;
using System.Reflection;

namespace KZDev.PerfUtils;

//################################################################################
/// <summary>
/// A generic type that can be used as a key for scenarios where a dynamic number and
/// type of keys are needed, such as caching scenarios, where one of the existing
/// <see cref="DynamicKey"/> types is not sufficient.
/// </summary>
public static class DynamicKey<T>
{
    /// <summary>
    /// Cached delegate for efficient key creation without boxing.
    /// This factory is created once per type T using reflection and compiled lambda expressions,
    /// then reused for all subsequent calls to avoid both boxing and reflection overhead.
    /// </summary>
    private static readonly Func<T, DynamicKey> KeyFactory = CreateKeyFactory();

    //--------------------------------------------------------------------------------
    /// <summary>
    /// Creates a compiled lambda expression for efficient key creation based on the type T.
    /// </summary>
    /// <returns>A compiled delegate that creates the appropriate DynamicKey without boxing.</returns>
    private static Func<T, DynamicKey> CreateKeyFactory ()
    {
        Type type = typeof(T);

        // Handle special cases first
        if (type == typeof(int))
            return (Func<T, DynamicKey>)(object)new Func<int, DynamicKey>(DynamicIntKey.GetKey);
        if (type == typeof(long))
            return (Func<T, DynamicKey>)(object)new Func<long, DynamicKey>(DynamicLongKey.GetKey);
        if (type == typeof(uint))
            return (Func<T, DynamicKey>)(object)new Func<uint, DynamicKey>(DynamicUIntKey.GetKey);
        if (type == typeof(ulong))
            return (Func<T, DynamicKey>)(object)new Func<ulong, DynamicKey>(DynamicULongKey.GetKey);
        if (type == typeof(bool))
            return (Func<T, DynamicKey>)(object)new Func<bool, DynamicKey>(DynamicBoolKey.GetKey);
        if (type == typeof(string))
            return (Func<T, DynamicKey>)(object)new Func<string, DynamicKey>(DynamicStringKey.GetKey);
        if (type == typeof(Guid))
            return (Func<T, DynamicKey>)(object)new Func<Guid, DynamicKey>(DynamicGuidKey.GetKey);
        if (type == typeof(Type))
            return (Func<T, DynamicKey>)(object)new Func<Type, DynamicKey>(DynamicTypeKey.GetKey);

        // Handle enums
        if (type.IsEnum)
        {
            return CreateEnumKeyFactory(type);
        }

        // Handle other value types
        if (type.IsValueType)
        {
            return CreateValueTypeKeyFactory(type);
        }

        // Handle reference types
        return CreateReferenceTypeKeyFactory(type);
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Creates a factory for enum types using DynamicEnumKey.
    /// </summary>
    private static Func<T, DynamicKey> CreateEnumKeyFactory (Type enumType)
    {
        try
        {
            // Get the generic DynamicEnumKey.GetKey method
            MethodInfo getKeyMethod = typeof(DynamicEnumKey<>)
                .MakeGenericType(enumType)
                .GetMethod(nameof(DynamicEnumKey<BindingFlags>.GetKey), BindingFlags.Public | BindingFlags.Static)!;

            // Create lambda: value => DynamicEnumKey<TEnum>.GetKey(value)
            ParameterExpression valueParam = Expression.Parameter(typeof(T), "value");
            MethodCallExpression call = Expression.Call(getKeyMethod, valueParam);

            return Expression.Lambda<Func<T, DynamicKey>>(call, valueParam).Compile();
        }
        catch
        {
            // Fallback to DynamicObjectKey if reflection fails
            return value => DynamicObjectKey.GetKey(value!);
        }
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Creates a factory for value types using DynamicValKey.
    /// </summary>
    private static Func<T, DynamicKey> CreateValueTypeKeyFactory (Type valueType)
    {
        try
        {
            // Get the generic DynamicValKey.GetKey method
            MethodInfo getKeyMethod = typeof(DynamicValKey<>)
                .MakeGenericType(valueType)
                .GetMethod(nameof(DynamicValKey<int>.GetKey), BindingFlags.Public | BindingFlags.Static)!;

            // Create lambda: value => DynamicValKey<T>.GetKey(value)
            ParameterExpression valueParam = Expression.Parameter(typeof(T), "value");
            MethodCallExpression call = Expression.Call(getKeyMethod, valueParam);

            return Expression.Lambda<Func<T, DynamicKey>>(call, valueParam).Compile();
        }
        catch
        {
            // Fallback to DynamicObjectKey if reflection fails
            return value => DynamicObjectKey.GetKey(value!);
        }
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Creates a factory for reference types using DynamicRefKey.
    /// </summary>
    private static Func<T, DynamicKey> CreateReferenceTypeKeyFactory (Type referenceType)
    {
        try
        {
            // Get the generic DynamicRefKey.GetKey method
            MethodInfo getKeyMethod = typeof(DynamicRefKey<>)
                .MakeGenericType(referenceType)
                .GetMethod(nameof(DynamicRefKey<object>.GetKey), BindingFlags.Public | BindingFlags.Static)!;

            // Create lambda: value => DynamicRefKey<T>.GetKey(value)
            ParameterExpression valueParam = Expression.Parameter(typeof(T), "value");
            MethodCallExpression call = Expression.Call(getKeyMethod, valueParam);

            return Expression.Lambda<Func<T, DynamicKey>>(call, valueParam).Compile();
        }
        catch
        {
            // Fallback to DynamicObjectKey if reflection fails
            return value => DynamicObjectKey.GetKey(value!);
        }
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Gets a <see cref="DynamicKey"/> instance for the given value.
    /// </summary>
    /// <param name="value">
    /// The value to use as a key. This may be null if T is a reference type.
    /// </param>
    /// <returns>
    /// An instance of <see cref="DynamicKey"/> that uses the specified value to
    /// represent the key or partial key.
    /// </returns>
    public static DynamicKey GetKey (T value)
    {
        if (value is null)
            return DynamicKey.Null;
        if (value is DynamicKey dynamicKey)
            return dynamicKey;

        return KeyFactory(value);
    }
    //--------------------------------------------------------------------------------
}
//################################################################################
