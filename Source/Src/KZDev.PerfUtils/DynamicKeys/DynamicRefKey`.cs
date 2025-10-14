// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Diagnostics;

namespace KZDev.PerfUtils;

//################################################################################
/// <summary>
/// An instance of <see cref="DynamicKey"/> that uses <typeparamref name="T"/>
/// as the key
/// </summary>
[DebuggerDisplay("{" + nameof(DisplayValue) + "}")]
internal sealed class DynamicRefKey<T> : DynamicKey, IComparable<DynamicRefKey<T>>
    where T : class
{
    /// <summary>
    /// When not null, this is a cached instance used to avoid creating multiple
    /// instances of this class for the same uint value on the same thread.
    /// </summary>
    [ThreadStatic] private static DynamicRefKey<T>? _cachedInstance;

    //--------------------------------------------------------------------------------
    /// <summary>
    /// Gets the debugger display value.
    /// </summary>
    /// <value>
    /// The debugger display value.
    /// </value>
    private string DisplayValue => ToString();

    /// <summary>
    /// The value used as the key value for this instance.
    /// </summary>
    public T Value { [DebuggerStepThrough] get; }

    //--------------------------------------------------------------------------------
    /// <summary>
    /// Initializes a new instance of the <see cref="DynamicRefKey{T}"/> class.
    /// </summary>
    /// <param name="value">
    /// The value to use as a key for this instance.
    /// </param>
    private DynamicRefKey (T value)
    {
        Value = value;
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Gets a <see cref="DynamicKey"/> instance for the given integer value.
    /// </summary>
    /// <param name="value">
    /// The integer value to use as a key.
    /// </param>
    /// <returns>
    /// An instance of <see cref="DynamicKey"/> that uses the specified integer value to
    /// represent the key or partial key.
    /// </returns>
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
    /// Gets a <see cref="DynamicKey"/> instance for the given value.
    /// </summary>
    /// <param name="value">
    /// The value to use as a key.
    /// </param>
    /// <returns>
    /// An instance of <see cref="DynamicKey"/> that uses the specified value to 
    /// represent the key or partial key.
    /// </returns>
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
    /// <inheritdoc />
    public override int GetHashCode () => Value!.GetHashCode();
    //--------------------------------------------------------------------------------
    /// <inheritdoc />
    public override string ToString () => Value?.ToString() ?? string.Empty;
    //--------------------------------------------------------------------------------

    #endregion Overrides of Object

    #region Overrides of DynamicKey

    //--------------------------------------------------------------------------------
    /// <inheritdoc />
    public override bool Equals (DynamicKey? other) =>
        (other is DynamicRefKey<T> objectCacheKey) &&
        EqualityComparer<T>.Default.Equals(Value!, objectCacheKey.Value!);
    //--------------------------------------------------------------------------------
    /// <inheritdoc />
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
    /// <inheritdoc />
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