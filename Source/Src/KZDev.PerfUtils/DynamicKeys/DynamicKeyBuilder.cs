// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

namespace KZDev.PerfUtils;

//################################################################################
/// <summary>
///   A fluent builder class for creating composite <see cref="DynamicKey"/> instances
///   from multiple values and keys.
/// </summary>
/// <remarks>
///   <para>
///     <see cref="DynamicKeyBuilder"/> provides a convenient fluent API for constructing
///     composite keys from multiple individual values. It automatically handles type
///     conversion and flattening of nested composite keys to prevent unnecessary nesting.
///   </para>
///   <para>
///     Key features include:
///   </para>
///   <list type="bullet">
///     <item>
///       <description>Fluent API for method chaining</description>
///     </item>
///     <item>
///       <description>Automatic flattening of composite keys</description>
///     </item>
///     <item>
///       <description>Type-safe generic methods</description>
///     </item>
///     <item>
///       <description>Specialized methods for common types</description>
///     </item>
///   </list>
///   <para>
///     Example usage:
///   </para>
///   <code>
///     DynamicKey composite = DynamicKeyBuilder.Create()
///         .AddInt(42)
///         .AddString("user")
///         .AddBool(true)
///         .Build();
///     
///     // Or using generic methods
///     DynamicKey composite2 = DynamicKeyBuilder.Create()
///         .Add(42)
///         .Add("user")
///         .Add(true)
///         .Build();
///   </code>
/// </remarks>
/// <seealso cref="DynamicKey"/>
public sealed class DynamicKeyBuilder
{
    /// <summary>
    /// The list of DynamicKey instances added to the builder.
    /// </summary>
    private readonly List<DynamicKey> _keys = [];

    //--------------------------------------------------------------------------------
    /// <summary>
    /// Initializes a new instance of the <see cref="DynamicKeyBuilder"/> class.
    /// </summary>
    private DynamicKeyBuilder ()
    {
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    ///   Creates a new <see cref="DynamicKeyBuilder"/> instance.
    /// </summary>
    /// <returns>
    ///   A new <see cref="DynamicKeyBuilder"/> instance ready for building composite keys.
    /// </returns>
    /// <remarks>
    ///   This is the entry point for creating composite keys using the builder pattern.
    ///   After creating a builder, use the various <c>Add</c> methods to add keys and values,
    ///   then call <see cref="Build"/> to create the final composite key.
    /// </remarks>
    public static DynamicKeyBuilder Create () => new();
    //--------------------------------------------------------------------------------
    /// <summary>
    ///   Adds a <see cref="DynamicKey"/> to the builder.
    /// </summary>
    /// <param name="key">
    ///   The <see cref="DynamicKey"/> to add.
    /// </param>
    /// <returns>
    ///   This builder instance for method chaining.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///   Thrown when <paramref name="key"/> is null.
    /// </exception>
    /// <remarks>
    ///   <para>
    ///     This method adds a <see cref="DynamicKey"/> instance to the builder. If the key
    ///     is a <see cref="DynamicCompositeKey"/>, its individual keys will be flattened
    ///     and added separately to prevent unnecessary nesting.
    ///   </para>
    ///   <para>
    ///     This method is part of the fluent API and returns the builder instance
    ///     to allow method chaining.
    ///   </para>
    /// </remarks>
    public DynamicKeyBuilder Add (DynamicKey key)
    {
        ArgumentNullException.ThrowIfNull(key);

        // If the key is a DynamicCompositeKey, flatten it by adding its individual keys
        if (key is DynamicCompositeKey compositeKey)
        {
            foreach (DynamicKey individualKey in compositeKey.Keys)
            {
                _keys.Add(individualKey);
            }
        }
        else
        {
            _keys.Add(key);
        }

        return this;
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    ///   Adds a value as a <see cref="DynamicKey"/> to the builder.
    /// </summary>
    /// <typeparam name="T">
    ///   The type of the value to add.
    /// </typeparam>
    /// <param name="value">
    ///   The value to add as a <see cref="DynamicKey"/>.
    /// </param>
    /// <returns>
    ///   This builder instance for method chaining.
    /// </returns>
    /// <remarks>
    ///   <para>
    ///     This method creates a <see cref="DynamicKey"/> from the specified value using
    ///     <see cref="DynamicKey{T}.GetKey(T)"/> and adds it to the builder. If the resulting
    ///     key is a <see cref="DynamicCompositeKey"/>, its individual keys will be flattened
    ///     and added separately to prevent unnecessary nesting.
    ///   </para>
    ///   <para>
    ///     This method provides type-safe generic support and is part of the fluent API,
    ///     returning the builder instance to allow method chaining.
    ///   </para>
    ///   <para>
    ///     For better performance with specific types, consider using the specialized
    ///     methods like <see cref="AddInt(int)"/>, <see cref="AddString(string)"/>, etc.
    ///   </para>
    /// </remarks>
    public DynamicKeyBuilder Add<T> (T value) => Add(DynamicKey<T>.GetKey(value));
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Adds an integer value to the builder.
    /// </summary>
    /// <param name="value">
    /// The integer value to add.
    /// </param>
    /// <returns>
    /// This builder instance for method chaining.
    /// </returns>
    public DynamicKeyBuilder AddInt (int value)
    {
        _keys.Add(DynamicKey.GetKey(value));
        return this;
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Adds a long value to the builder.
    /// </summary>
    /// <param name="value">
    /// The long value to add.
    /// </param>
    /// <returns>
    /// This builder instance for method chaining.
    /// </returns>
    public DynamicKeyBuilder AddLong (long value)
    {
        _keys.Add(DynamicKey.GetKey(value));
        return this;
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Adds an unsigned integer value to the builder.
    /// </summary>
    /// <param name="value">
    /// The unsigned integer value to add.
    /// </param>
    /// <returns>
    /// This builder instance for method chaining.
    /// </returns>
    public DynamicKeyBuilder AddUInt (uint value)
    {
        _keys.Add(DynamicKey.GetKey(value));
        return this;
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Adds an unsigned long value to the builder.
    /// </summary>
    /// <param name="value">
    /// The unsigned long value to add.
    /// </param>
    /// <returns>
    /// This builder instance for method chaining.
    /// </returns>
    public DynamicKeyBuilder AddULong (ulong value)
    {
        _keys.Add(DynamicKey.GetKey(value));
        return this;
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Adds a boolean value to the builder.
    /// </summary>
    /// <param name="value">
    /// The boolean value to add.
    /// </param>
    /// <returns>
    /// This builder instance for method chaining.
    /// </returns>
    public DynamicKeyBuilder AddBool (bool value)
    {
        _keys.Add(DynamicKey.GetKey(value));
        return this;
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Adds a string value to the builder.
    /// </summary>
    /// <param name="value">
    /// The string value to add.
    /// </param>
    /// <returns>
    /// This builder instance for method chaining.
    /// </returns>
    public DynamicKeyBuilder AddString (string? value)
    {
        _keys.Add(DynamicKey.GetKey(value));
        return this;
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Adds a Guid value to the builder.
    /// </summary>
    /// <param name="value">
    /// The Guid value to add.
    /// </param>
    /// <returns>
    /// This builder instance for method chaining.
    /// </returns>
    public DynamicKeyBuilder AddGuid (Guid value)
    {
        _keys.Add(DynamicKey.GetKey(value));
        return this;
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Adds a Type value to the builder.
    /// </summary>
    /// <param name="value">
    /// The Type value to add.
    /// </param>
    /// <returns>
    /// This builder instance for method chaining.
    /// </returns>
    public DynamicKeyBuilder AddType (Type value)
    {
        _keys.Add(DynamicKey.GetKey(value));
        return this;
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Adds an enum value to the builder.
    /// </summary>
    /// <typeparam name="TEnum">
    /// The enum type.
    /// </typeparam>
    /// <param name="value">
    /// The enum value to add.
    /// </param>
    /// <returns>
    /// This builder instance for method chaining.
    /// </returns>
    public DynamicKeyBuilder AddEnum<TEnum> (TEnum value) where TEnum : struct, Enum
    {
        _keys.Add(DynamicKey.GetEnumKey(value));
        return this;
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    ///   Builds the final composite <see cref="DynamicKey"/> from all added keys.
    /// </summary>
    /// <returns>
    ///   A composite <see cref="DynamicKey"/> containing all keys that were added to the builder.
    ///   If only one key was added, that key is returned unchanged.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    ///   Thrown when no keys have been added to the builder.
    /// </exception>
    /// <remarks>
    ///   <para>
    ///     This method creates the final composite key from all keys that have been added
    ///     to the builder using the various <c>Add</c> methods. The keys are combined in
    ///     the order they were added.
    ///   </para>
    ///   <para>
    ///     The resulting composite key provides optimized hash code generation and comparison
    ///     operations, making it suitable for use as dictionary keys or in other scenarios
    ///     requiring efficient key operations.
    ///   </para>
    ///   <para>
    ///     After calling this method, the builder can be reused to create additional
    ///     composite keys by calling the <c>Add</c> methods again.
    ///   </para>
    /// </remarks>
    public DynamicKey Build ()
    {
        if (_keys.Count == 0)
            throw new InvalidOperationException("At least one key must be added before building");
        return DynamicKey.Combine([.. _keys]);
    }
    //--------------------------------------------------------------------------------
}
//################################################################################
