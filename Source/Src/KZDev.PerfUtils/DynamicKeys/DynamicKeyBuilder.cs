// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

namespace KZDev.PerfUtils;

//################################################################################
/// <summary>
/// A builder class for creating composite DynamicKey instances using a fluent API
/// </summary>
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
    /// Creates a new DynamicKeyBuilder instance.
    /// </summary>
    /// <returns>
    /// A new DynamicKeyBuilder instance.
    /// </returns>
    public static DynamicKeyBuilder Create () => new();
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Adds a DynamicKey to the builder.
    /// If the key is a DynamicCompositeKey, its individual keys will be flattened and added separately.
    /// </summary>
    /// <param name="key">
    /// The DynamicKey to add.
    /// </param>
    /// <returns>
    /// This builder instance for method chaining.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when key is null.
    /// </exception>
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
    /// Adds a value as a DynamicKey to the builder.
    /// If the resulting key is a DynamicCompositeKey, its individual keys will be flattened and added separately.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the value to add.
    /// </typeparam>
    /// <param name="value">
    /// The value to add as a DynamicKey.
    /// </param>
    /// <returns>
    /// This builder instance for method chaining.
    /// </returns>
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
    /// Builds the final composite DynamicKey.
    /// </summary>
    /// <returns>
    /// A composite DynamicKey containing all added keys.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when no keys have been added.
    /// </exception>
    public DynamicKey Build ()
    {
        if (_keys.Count == 0)
            throw new InvalidOperationException("At least one key must be added before building");
        return DynamicKey.Combine([.. _keys]);
    }
    //--------------------------------------------------------------------------------
}
//################################################################################
