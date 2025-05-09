// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KZDev.PerfUtils;

using System;

// TODO - Add support in InterlockedOps for this type

/// <summary>
/// A reference type wrapper for value types. Using this wrapper can
/// be especially useful when you need to manage atomic operations
/// such as exchanging values in a thread-safe manner, or interlocked
/// operations.
/// </summary>
/// <typeparam name="T">
/// The type of the value to wrap. This type must be a value type 
/// and implement IComparable&lt;T&gt; and IEquatable&lt;T&gt;.
/// </typeparam>
public class ValueRef<T> : IComparable<T>, IEquatable<T>
    where T : struct, IComparable<T>, IEquatable<T>
{
    /// <summary>
    /// The value being wrapped.
    /// </summary>
    public T Value { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValueRef{T}"/> class
    /// </summary>
    /// <param name="value">
    /// The value to wrap.
    /// </param>
    public ValueRef(T value) => Value = value;

    #region IComparable<T> implementation

    /// <inheritdoc />
    public int CompareTo(T other) => Value.CompareTo(other);

    #endregion IComparable<T> implementation

    #region IEquatable<T> implementation

    /// <inheritdoc />
    public bool Equals(T other) => Value.Equals(other);

    #endregion IEquatable<T> implementation

    #region Object Overrides

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is ValueRef<T> wrapper && Equals(wrapper.Value);

#pragma warning disable HAA0102
#pragma warning disable CS8603 // Possible null reference return.
    /// <inheritdoc />
    public override int GetHashCode() => Value.GetHashCode();

    /// <inheritdoc />
    public override string ToString() => Value.ToString();
#pragma warning restore CS8603 // Possible null reference return.
#pragma warning restore HAA0102

    #endregion Object Overrides

    /// <summary>
    /// Implicit conversion operator to convert from <see cref="ValueRef{T}"/> to <typeparamref name="T"/>.
    /// </summary>
    /// <param name="wrapper"></param>
    public static implicit operator T(ValueRef<T> wrapper) => wrapper.Value;

    /// <summary>
    /// Implicit conversion operator to convert from <typeparamref name="T"/> to <see cref="ValueRef{T}"/>.
    /// </summary>
    /// <param name="value"></param>
    public static implicit operator ValueRef<T>(in T value) => new(value);

    /// <summary>
    /// Equality operator overload for <see cref="ValueRef{T}"/>.
    /// </summary>
    /// <param name="left">
    /// The left operand of the equality operator.
    /// </param>
    /// <param name="right">
    /// The right operand of the equality operator.
    /// </param>
    /// <returns>
    /// True if the left and right operands are equal; otherwise, false.
    /// </returns>
    public static bool operator ==(ValueRef<T> left, ValueRef<T> right) => left.Equals(right.Value);

    /// <summary>
    /// Inequality operator overload for <see cref="ValueRef{T}"/>.
    /// </summary>
    /// <param name="left">
    /// The left operand of the inequality operator.
    /// </param>
    /// <param name="right">
    /// The right operand of the inequality operator.
    /// </param>
    /// <returns>
    /// True if the left and right operands are not equal; otherwise, false.
    /// </returns>
    public static bool operator !=(ValueRef<T> left, ValueRef<T> right) => !left.Equals(right.Value);

    /// <summary>
    /// Greater than operator overload for <see cref="ValueRef{T}"/>.
    /// </summary>
    /// <param name="left">
    /// The left operand of the greater than operator.
    /// </param>
    /// <param name="right">
    /// The right operand of the greater than operator.
    /// </param>
    /// <returns>
    /// True if the left operand is greater than the right operand; otherwise, false.
    /// </returns>
    public static bool operator >(ValueRef<T> left, ValueRef<T> right) => left.Value.CompareTo(right.Value) > 0;

    /// <summary>
    /// Less than operator overload for <see cref="ValueRef{T}"/>.
    /// </summary>
    /// <param name="left">
    /// The left operand of the less than operator.
    /// </param>
    /// <param name="right">
    /// The right operand of the less than operator.
    /// </param>
    /// <returns>
    /// True if the left operand is less than the right operand; otherwise, false.
    /// </returns>
    public static bool operator >=(ValueRef<T> left, ValueRef<T> right) => left.Value.CompareTo(right.Value) >= 0;

    /// <summary>
    /// Less than or equal to operator overload for <see cref="ValueRef{T}"/>.
    /// </summary>
    /// <param name="left">
    /// The left operand of the less than or equal to operator.
    /// </param>
    /// <param name="right">
    /// The right operand of the less than or equal to operator.
    /// </param>
    /// <returns>
    /// True if the left operand is less than or equal to the right operand; otherwise, false.
    /// </returns>
    public static bool operator <(ValueRef<T> left, ValueRef<T> right) => left.Value.CompareTo(right.Value) < 0;

    /// <summary>
    /// Less than or equal to operator overload for <see cref="ValueRef{T}"/>.
    /// </summary>
    /// <param name="left">
    /// The left operand of the less than or equal to operator.
    /// </param>
    /// <param name="right">
    /// The right operand of the less than or equal to operator.
    /// </param>
    /// <returns>
    /// True if the left operand is less than or equal to the right operand; otherwise, false.
    /// </returns>
    public static bool operator <=(ValueRef<T> left, ValueRef<T> right) => left.Value.CompareTo(right.Value) <= 0;


}

