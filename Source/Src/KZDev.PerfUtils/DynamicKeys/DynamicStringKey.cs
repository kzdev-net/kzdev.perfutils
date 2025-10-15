// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace KZDev.PerfUtils;

//################################################################################
/// <summary>
///   A type of <see cref="DynamicKey"/> that uses a string as the key.
/// </summary>
/// <remarks>
///   <para>
///     <see cref="DynamicStringKey"/> provides an optimized implementation for string-based keys
///     with thread-static caching for improved performance. It handles null strings by treating
///     them as empty strings for consistent behavior.
///   </para>
///   <para>
///     Key features include:
///   </para>
///   <list type="bullet">
///     <item>
///       <description>Thread-static caching for frequently used strings</description>
///     </item>
///     <item>
///       <description>Automatic null-to-empty string conversion</description>
///     </item>
///     <item>
///       <description>Optimized equality and comparison operations</description>
///     </item>
///     <item>
///       <description>Special handling for empty strings with cached instance</description>
///     </item>
///   </list>
///   <para>
///     This class is used internally by the <see cref="DynamicKey"/> system and is not
///     typically instantiated directly by user code. Instead, use <see cref="DynamicKey.GetKey(string)"/>
///     to create string-based keys.
///   </para>
/// </remarks>
/// <seealso cref="DynamicKey"/>
[DebuggerDisplay("{" + nameof(DisplayValue) + "}")]
internal sealed class DynamicStringKey : DynamicKey, IComparable<DynamicStringKey>
{
    /// <summary>
    /// When not null, this is a cached instance used to avoid creating multiple
    /// instances of this class for the same integer value on the same thread.
    /// </summary>
    [ThreadStatic] private static DynamicStringKey? _cachedInstance;

    /// <summary>
    /// Gets the debugger display value.
    /// </summary>
    /// <value>
    /// The debugger display value.
    /// </value>
    private string DisplayValue => StringKey;

    /// <summary>
    /// The string value used as the key value for this instance.
    /// </summary>
    public string StringKey { [DebuggerStepThrough] get; }

    //--------------------------------------------------------------------------------
    /// <summary>
    /// Returns a common cached instance used for empty strings.
    /// </summary>
    public static DynamicStringKey Empty { [DebuggerStepThrough] get; } = new(string.Empty);
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Helper to get a common usable string for comparison while accounting for nulls.
    /// </summary>
    /// <param name="key">
    /// The key string to use or null if you want to use an empty string.
    /// </param>
    /// <returns>
    /// A usable string for comparison. If <paramref name="key"/> is null,
    /// this will return <see cref="string.Empty"/>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static string GetUsableKeyString (string? key) => key ?? string.Empty;
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Initializes a new instance of the <see cref="DynamicStringKey"/> class.
    /// </summary>
    /// <param name="key">
    /// The string value to use as a key for this instance. This must not be null.
    /// </param>
    private DynamicStringKey (string key)
    {
        ArgumentNullException.ThrowIfNull(key);
        StringKey = key;
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Gets a <see cref="DynamicKey"/> instance for the given string value.
    /// </summary>
    /// <param name="key">
    /// The key string to use for this instance. If this is null, it will be treated
    /// as an empty string.
    /// </param>
    /// <returns></returns>
    public new static DynamicKey GetKey (string? key)
    {
        string useKey = GetUsableKeyString(key);
        if (useKey == string.Empty)
            return Empty;
        DynamicStringKey? cachedInstance = _cachedInstance;
        if ((cachedInstance is not null) && (cachedInstance.StringKey == useKey))
            return cachedInstance;
        DynamicStringKey returnInstance = new(useKey);
        _cachedInstance = returnInstance;
        return returnInstance;
    }
    //--------------------------------------------------------------------------------

    #region Overrides of Object

    //--------------------------------------------------------------------------------
    /// <inheritdoc />
    public override int GetHashCode () => StringKey.GetHashCode();
    //--------------------------------------------------------------------------------
    /// <inheritdoc />
    public override string ToString () => StringKey;
    //--------------------------------------------------------------------------------

    #endregion Overrides of Object

    #region Overrides of DynamicKey

    //--------------------------------------------------------------------------------
    /// <inheritdoc />
    public override bool Equals (DynamicKey? other) =>
        (other is DynamicStringKey stringCacheKey) &&
        (ReferenceEquals(this, stringCacheKey) || (stringCacheKey.StringKey == StringKey));
    //--------------------------------------------------------------------------------
    /// <inheritdoc />
    public override int CompareTo (DynamicKey? other)
    {
        return other switch
        {
            null => 1,
            DynamicStringKey stringCacheKey => CompareTo(stringCacheKey),
            _ => CompareKey(other)
        };
    }
    //--------------------------------------------------------------------------------

    #endregion

    #region IComparable<DynamicStringKey> Members

    //--------------------------------------------------------------------------------
    /// <inheritdoc />
    public int CompareTo (DynamicStringKey? other) => other is null ? 1 :
        ReferenceEquals(StringKey, other.StringKey) ? 0 :
        string.Compare(StringKey, other.StringKey, StringComparison.Ordinal);
    //--------------------------------------------------------------------------------

    #endregion IComparable<DynamicStringKey> Members
}
//################################################################################