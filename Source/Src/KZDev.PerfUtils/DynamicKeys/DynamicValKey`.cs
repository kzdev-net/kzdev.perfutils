using System.Diagnostics;

namespace KZDev.PerfUtils;

//################################################################################
/// <summary>
/// An instance of <see cref="DynamicKey"/> that uses <typeparamref name="T"/>
/// as the key
/// </summary>
[DebuggerDisplay("{" + nameof(DisplayValue) + "}")]
internal class DynamicValKey<T> : DynamicKey, IComparable<DynamicValKey<T>>
    where T : struct
{
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
    /// Initializes a new instance of the <see cref="DynamicValKey{T}"/> class.
    /// </summary>
    /// <param name="value">
    /// The value to use as a key for this instance.
    /// </param>
    private DynamicValKey (in T value)
    {
        Value = value;
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
    public static DynamicKey GetKey (in T value)
    {
        return value switch
        {
            bool boolValue => DynamicBoolKey.GetKey(boolValue),
            int intValue => DynamicIntKey.GetKey (intValue),
            long longValue => DynamicLongKey.GetKey (longValue),
            uint uintValue => DynamicUIntKey.GetKey (uintValue),
            ulong ulongValue => DynamicULongKey.GetKey (ulongValue),
            Guid guidValue => DynamicGuidKey.GetKey (guidValue),
            _ => new DynamicValKey<T> (value)
        };
    }
    //--------------------------------------------------------------------------------

    #region Overrides of Object

    //--------------------------------------------------------------------------------
    /// <inheritdoc />
    public override int GetHashCode () => Value.GetHashCode();
    //--------------------------------------------------------------------------------
    /// <inheritdoc />
    public override string ToString () => Value.ToString() ?? string.Empty;
    //--------------------------------------------------------------------------------

    #endregion Overrides of Object

    #region Overrides of DynamicKey

    //--------------------------------------------------------------------------------
    /// <inheritdoc />
    public override bool Equals (DynamicKey? other) =>
        (other is DynamicValKey<T> objectCacheKey) &&
        EqualityComparer<T>.Default.Equals(Value, objectCacheKey.Value!);
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
    /// <inheritdoc />
    public int CompareTo (DynamicValKey<T>? other) =>
        Value switch
        {
            IComparable<T> comparableT => comparableT.CompareTo (other?.Value ?? default),
            IComparable comparable => comparable.CompareTo (other?.Value ?? default),
            _ => other?.Value switch
            {
                IComparable<T> otherComparableT => (otherComparableT.CompareTo (Value) * (-1)),
                IComparable otherComparable => (otherComparable.CompareTo (Value) * (-1)),
                _ => throw new InvalidOperationException (
                    $"DynamicValKey<{typeof(T)}> can't be compared because the generic type {typeof(T)} is not comparable")
            }
        };
    //--------------------------------------------------------------------------------

    #endregion IComparable<DynamicValKey<T>> Members
}
//################################################################################