// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Linq.Expressions;
using System.Reflection;

namespace KZDev.PerfUtils;

//################################################################################
/// <summary>
///   A generic static class that provides optimized key creation for any type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">
///   The type for which to create <see cref="DynamicKey"/> instances.
/// </typeparam>
/// <remarks>
///   <para>
///     <see cref="DynamicKey{T}"/> provides a high-performance, type-safe way to create
///     <see cref="DynamicKey"/> instances for any type. It uses compiled lambda expressions
///     and reflection to avoid boxing overhead for value types and enums, while providing
///     fallback support for all other types.
///   </para>
///   <para>
///     Key features include:
///   </para>
///   <list type="bullet">
///     <item>
///       <description>Zero-boxing performance for value types and enums</description>
///     </item>
///     <item>
///       <description>Compiled lambda expressions for optimal performance</description>
///     </item>
///     <item>
///       <description>Automatic type detection and optimization</description>
///     </item>
///     <item>
///       <description>Fallback support for all types</description>
///     </item>
///   </list>
///   <para>
///     Example usage:
///   </para>
///   <code>
///     // For value types (no boxing)
///     DynamicKey key1 = DynamicKey&lt;int&gt;.GetKey(42);
///     DynamicKey key2 = DynamicKey&lt;DateTime&gt;.GetKey(DateTime.Now);
///     
///     // For reference types
///     DynamicKey key3 = DynamicKey&lt;string&gt;.GetKey("hello");
///     DynamicKey key4 = DynamicKey&lt;MyClass&gt;.GetKey(myInstance);
///     
///     // For enums (no boxing)
///     DynamicKey key5 = DynamicKey&lt;StringComparison&gt;.GetKey(StringComparison.Ordinal);
///   </code>
///   <para>
///     This class is particularly useful when you need to create keys from generic types
///     or when you want to ensure optimal performance for value types and enums.
///   </para>
/// </remarks>
/// <seealso cref="DynamicKey"/>
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
    ///   Creates a <see cref="DynamicKey"/> instance for the specified value of type <typeparamref name="T"/>.
    /// </summary>
    /// <param name="value">
    ///   The value to use as a key. This may be null if <typeparamref name="T"/> is a reference type.
    /// </param>
    /// <returns>
    ///   A <see cref="DynamicKey"/> instance that represents the specified value.
    ///   If <paramref name="value"/> is null, returns <see cref="DynamicKey.Null"/>.
    ///   If <paramref name="value"/> is already a <see cref="DynamicKey"/>, it is returned unchanged.
    /// </returns>
    /// <remarks>
    ///   <para>
    ///     This method provides the most efficient way to create a <see cref="DynamicKey"/> for
    ///     any type <typeparamref name="T"/>. It uses a pre-compiled lambda expression that
    ///     is created once per type and reused for all subsequent calls, providing optimal
    ///     performance without boxing overhead for value types and enums.
    ///   </para>
    ///   <para>
    ///     The method automatically detects the type and routes to the appropriate specialized
    ///     key creation method:
    ///   </para>
    ///   <list type="bullet">
    ///     <item>
    ///       <description>Primitive types (int, long, bool, string, Guid, Type) use direct method calls</description>
    ///     </item>
    ///     <item>
    ///       <description>Enum types use <see cref="DynamicEnumKey{TEnum}"/> without boxing</description>
    ///     </item>
    ///     <item>
    ///       <description>Other value types use <see cref="DynamicValKey{T}"/> without boxing</description>
    ///     </item>
    ///     <item>
    ///       <description>Reference types use <see cref="DynamicRefKey{T}"/> with reference equality</description>
    ///     </item>
    ///   </list>
    ///   <para>
    ///     If reflection fails for any reason, the method falls back to using
    ///     <see cref="DynamicObjectKey"/> for universal compatibility.
    ///   </para>
    /// </remarks>
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
