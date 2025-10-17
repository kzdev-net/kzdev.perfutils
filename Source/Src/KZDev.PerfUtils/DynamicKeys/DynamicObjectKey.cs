// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Diagnostics;

using KZDev.PerfUtils.Helpers;

namespace KZDev.PerfUtils;

//################################################################################
/// <summary>
///   A type of <see cref="DynamicKey"/> that uses an <see cref="object"/> instance as the key.
/// </summary>
/// <remarks>
///   <para>
///     <see cref="DynamicObjectKey"/> provides a general-purpose implementation for object-based keys
///     with thread-static caching for improved performance. It handles null objects and uses
///     the object's <see cref="object.Equals(object)"/> and <see cref="object.GetHashCode"/> methods
///     for comparison operations.
///   </para>
///   <para>
///     Key features include:
///   </para>
///   <list type="bullet">
///     <item>
///       <description>Thread-static caching for frequently used object references</description>
///     </item>
///     <item>
///       <description>Support for null object references</description>
///     </item>
///     <item>
///       <description>Uses object's equality and hash code methods</description>
///     </item>
///     <item>
///       <description>Special handling for null objects with cached instance</description>
///     </item>
///   </list>
///   <para>
///     This class is used internally by the <see cref="DynamicKey"/> system as a fallback
///     for types that don't have specialized implementations. It's also used when creating
///     keys from generic object references.
///   </para>
///   <para>
///     For better performance with known types, consider using the specific
///     <see cref="DynamicKey.GetKey(int)"/>, <see cref="DynamicKey.GetKey(string)"/>, etc. methods.
///   </para>
/// </remarks>
/// <seealso cref="DynamicKey"/>
[DebuggerDisplay("{" + nameof(DisplayValue) + "}")]
internal sealed class DynamicObjectKey : DynamicKey, IComparable<DynamicObjectKey>
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
        // For the cached instance of an object reference type, we use ReferenceEquals just in case the object         
        // overrides Equals to do a value comparison instead of a reference comparison, and
        // we want to be sure we are only returning the cached instance if it is actually
        // the same instance.
        if ((cachedInstance is not null) && ReferenceEquals(cachedInstance.Value, value))
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
        ((other is DynamicObjectKey objectCacheKey) &&
         (ReferenceEquals(objectCacheKey.Value, Value) || Equals(objectCacheKey.Value, Value))) ||
        ((other is DynamicCompositeKey compositeKey) && compositeKey.Equals(Value)) ||
        ((other?.ObjectValue is DynamicCompositeKey otherCompositeKey) && otherCompositeKey.Equals(Value)) ||
        Equals (other?.ObjectValue, Value);
    //--------------------------------------------------------------------------------
    /// <inheritdoc />
    public override int CompareTo (DynamicKey? other)
    {
        if (ReferenceEquals(null, other))
            return 1;
        if (ReferenceEquals(this, other))
            return 0;
        if (ReferenceEquals(other.ObjectValue, Value))
            return 0;
        if (other.ObjectValue is null)
            return 1;
        return Value switch
        {
            null => -1,
            string strValue when (other?.ObjectValue is string refStringValue) => string.CompareOrdinal (strValue, refStringValue),
            IComparable comparable => comparable.CompareTo (other?.ObjectValue),
            _ => other.ObjectValue is IComparable otherComparable ?  otherComparable.CompareTo(Value) * (-1) : CompareKey (other)
        };
    }
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

            case string stringValue when (other.Value is string otherStringValue):
                return string.CompareOrdinal(stringValue, otherStringValue);

            case IComparable comparable:
                return comparable.CompareTo(other.Value);
        }
        if (other.Value is IComparable otherComparable)
            return otherComparable.CompareTo(Value) * (-1);
        ThrowHelper.ThrowArgumentException_ContainedValueIsNotComparable(nameof(other));
        return 0; // Will never get here
    }
    //--------------------------------------------------------------------------------

    #endregion IComparable<DynamicObjectKey> Members

    #region Overrides of DynamicRefKey

    //--------------------------------------------------------------------------------
    /// <inheritdoc />
    protected internal override object ObjectValue { [DebuggerStepThrough] get => Value; }
    //--------------------------------------------------------------------------------

    #endregion
}
//################################################################################