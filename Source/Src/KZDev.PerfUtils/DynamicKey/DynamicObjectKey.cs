using System.Diagnostics;

using KZDev.PerfUtils.Helpers;

namespace KZDev.PerfUtils;

//################################################################################
/// <summary>
/// A type of <see cref="DynamicKey"/> that uses an <see cref="object"/>
/// instance as the key
/// </summary>
[DebuggerDisplay("{" + nameof(DisplayValue) + "}")]
internal class DynamicObjectKey : DynamicKey, IComparable<DynamicObjectKey>
{
    /// <summary>
    /// When not null, this is a cached instance used to avoid creating multiple
    /// instances of this class for the same integer value on the same thread.
    /// </summary>
    [ThreadStatic] private static DynamicObjectKey? _cachedInstance;

    /// <summary>
    /// Gets the debugger display value.
    /// </summary>
    /// <value>
    /// The debugger display value.
    /// </value>
    private string DisplayValue => ToString();

    /// <summary>
    /// The nullable object value used as the key value for this instance.
    /// </summary>
    public object? Value { [DebuggerStepThrough] get; }

    //--------------------------------------------------------------------------------
    /// <summary>
    /// Returns a common instance used for null object references.
    /// </summary>
    public new static DynamicObjectKey Null { [DebuggerStepThrough] get; } = new(null);
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Initializes a new instance of the <see cref="DynamicObjectKey"/> class.
    /// </summary>
    /// <param name="value">
    /// The object value to use as a key for this instance. This may be null.
    /// </param>
    private DynamicObjectKey (object? value)
    {
        Value = value;
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Gets a <see cref="DynamicKey"/> instance for the given object value.
    /// </summary>
    /// <param name="value">
    /// The object value to use as a key. This may be null.
    /// </param>
    /// <returns>
    /// An instance of <see cref="DynamicKey"/> that uses the specified object value to
    /// represent the key or partial key.
    /// </returns>
    public new static DynamicObjectKey GetKey (object? value)
    {
        if (value is null)
            return Null;
        DynamicObjectKey? cachedInstance = _cachedInstance;
        if ((cachedInstance is not null) && (cachedInstance.Value?.Equals(value) ?? false))
            return cachedInstance;
        DynamicObjectKey returnInstance = new(value);
        _cachedInstance = returnInstance;
        return returnInstance;
    }
    //--------------------------------------------------------------------------------

    #region Overrides of Object

    //--------------------------------------------------------------------------------
    /// <inheritdoc />
    public override int GetHashCode () => Value?.GetHashCode() ?? 0;
    //--------------------------------------------------------------------------------
    /// <inheritdoc />
    public override string ToString () => Value?.ToString() ?? "{null}";
    //--------------------------------------------------------------------------------

    #endregion Overrides of Object

    #region Overrides of DynamicKey

    //--------------------------------------------------------------------------------
    /// <inheritdoc />
    public override bool Equals (DynamicKey? other) =>
        (other is DynamicObjectKey objectCacheKey) &&
        (ReferenceEquals(objectCacheKey.Value, Value) || Equals(objectCacheKey.Value, Value));
    //--------------------------------------------------------------------------------
    /// <inheritdoc />
    public override int CompareTo (DynamicKey? other) =>
        other switch
        {
            null => 1,
            DynamicObjectKey boolCacheKey => CompareTo(boolCacheKey),
            _ => CompareKey(other)
        };
    //--------------------------------------------------------------------------------

    #endregion

    #region IComparable<DynamicObjectKey> Members

    //--------------------------------------------------------------------------------
    /// <inheritdoc />
    public int CompareTo (DynamicObjectKey? other)
    {
        if (ReferenceEquals(other?.Value, Value))
            return 0;
        if (other?.Value is null)
            return 1;
        switch (Value)
        {
            case null:
                return -1;

            case IComparable comparable when (other?.Value is IComparable otherComparable):
                return comparable.CompareTo (otherComparable);
        };
        ThrowHelper.ThrowArgumentException_ContainedValueIsNotComparable(nameof(other));
        return 0; // Will never get here
    }
    //--------------------------------------------------------------------------------

    #endregion IComparable<DynamicObjectKey> Members
}
//################################################################################