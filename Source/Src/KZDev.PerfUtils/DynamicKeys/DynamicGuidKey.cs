// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Diagnostics;

namespace KZDev.PerfUtils;

//################################################################################
/// <summary>
///   A type of <see cref="DynamicKey"/> that uses a <see cref="Guid"/> as the key.
/// </summary>
/// <remarks>
///   <para>
///     <see cref="DynamicGuidKey"/> provides an optimized implementation for GUID-based keys
///     with thread-static caching for improved performance. It uses the GUID value directly
///     for hash code generation and comparison operations.
///   </para>
///   <para>
///     Key features include:
///   </para>
///   <list type="bullet">
///     <item>
///       <description>Thread-static caching for frequently used GUID values</description>
///     </item>
///     <item>
///       <description>Direct GUID value for hash codes (no computation overhead)</description>
///     </item>
///     <item>
///       <description>Optimized equality and comparison operations</description>
///     </item>
///     <item>
///       <description>Special handling for empty GUID values with cached instance</description>
///     </item>
///   </list>
///   <para>
///     This class is used internally by the <see cref="DynamicKey"/> system and is not
///     typically instantiated directly by user code. Instead, use <see cref="DynamicKey.GetKey(Guid)"/>
///     to create GUID-based keys.
///   </para>
/// </remarks>
/// <seealso cref="DynamicKey"/>
[DebuggerDisplay("{" + nameof(DisplayValue) + "}")]
internal sealed class DynamicGuidKey : DynamicKey, IComparable<DynamicGuidKey>
{
    /// <summary>
    /// When not null, this is a cached instance used to avoid creating multiple
    /// instances of this class for the same integer value on the same thread.
    /// </summary>
    [ThreadStatic] private static DynamicGuidKey? _cachedInstance;

    /// <summary>
    /// Gets the debugger display value.
    /// </summary>
    /// <value>
    /// The debugger display value.
    /// </value>
    private string DisplayValue => ToString();

    /// <summary>
    /// The object representation of the Guid value used for this instance.
    /// </summary>
    private object? _objValue;

    /// <summary>
    /// The Guid value used as the key value for this instance.
    /// </summary>
    public Guid Value { [DebuggerStepThrough] get; }

    //--------------------------------------------------------------------------------
    /// <summary>
    /// Returns a common instance used for empty Guid values.
    /// </summary>
    public static DynamicGuidKey Empty { [DebuggerStepThrough] get; } = new(Guid.Empty);
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Initializes a new instance of the <see cref="DynamicGuidKey"/> class.
    /// </summary>
    /// <param name="value">
    /// The Guid value to use as a key for this instance.
    /// </param>
    private DynamicGuidKey (in Guid value)
    {
        Value = value;
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Gets a <see cref="DynamicKey"/> instance for the given Guid value.
    /// </summary>
    /// <param name="value">
    /// The Guid value to use
    /// </param>
    /// <returns>
    /// An instance of <see cref="DynamicKey"/> that uses the specified Guid value to
    /// represent the key or partial key.
    /// </returns>
    public new static DynamicKey GetKey (in Guid value)
    {
        if (value.Equals(Guid.Empty))
            return Empty;
        DynamicGuidKey? cachedInstance = _cachedInstance;
        if ((cachedInstance is not null) && (cachedInstance.Value == value))
            return cachedInstance;
        DynamicGuidKey returnInstance = new(value);
        _cachedInstance = returnInstance;
        return returnInstance;
    }
    //--------------------------------------------------------------------------------

    #region Overrides of Object

    //--------------------------------------------------------------------------------
    /// <inheritdoc />
    public override int GetHashCode () => Value.GetHashCode();
    //--------------------------------------------------------------------------------
    /// <inheritdoc />
    public override string ToString () => Value.ToString();
    //--------------------------------------------------------------------------------

    #endregion Overrides of Object

    #region Overrides of DynamicKey

    //--------------------------------------------------------------------------------
    /// <inheritdoc />
    public override bool Equals (DynamicKey? other) =>
        ((other is DynamicGuidKey guidKey) &&
         (ReferenceEquals(this, guidKey) || (guidKey.Value == Value))) ||
        ((other is DynamicCompositeKey compositeKey) && compositeKey.Equals(this)) ||
        ((other?.ObjectValue is DynamicCompositeKey otherCompositeKey) && otherCompositeKey.Equals(this)) ||
        Equals(other?.ObjectValue, ObjectValue);
    //--------------------------------------------------------------------------------
    /// <inheritdoc />
    public override int CompareTo (DynamicKey? other) =>
        other switch
        {
            null => 1,
            DynamicGuidKey guidCacheKey => CompareTo(guidCacheKey),
            _ => CompareKey(other)
        };
    //--------------------------------------------------------------------------------

    #endregion

    #region IComparable<DynamicGuidKey> Members

    //--------------------------------------------------------------------------------
    /// <inheritdoc />
    public int CompareTo (DynamicGuidKey? other) => other is null ? 1 :
        Value.CompareTo(other.Value);
    //--------------------------------------------------------------------------------

    #endregion IComparable<DynamicGuidKey> Members

    #region Overrides of DynamicRefKey

    //--------------------------------------------------------------------------------
    /// <inheritdoc />
    protected internal override object ObjectValue { [DebuggerStepThrough] get => _objValue ??= Value; }
    //--------------------------------------------------------------------------------

    #endregion
}
//################################################################################