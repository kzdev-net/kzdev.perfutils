// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Collections.Immutable;
using System.Diagnostics;

using KZDev.PerfUtils.Helpers;

namespace KZDev.PerfUtils;

//################################################################################
/// <summary>
///   A type of <see cref="DynamicKey"/> that combines multiple <see cref="DynamicKey"/> instances
///   into a single composite key with optimized performance characteristics.
/// </summary>
/// <remarks>
///   <para>
///     <see cref="DynamicCompositeKey"/> represents a composite key that contains multiple
///     individual keys. It provides efficient hash code generation, equality comparison,
///     and ordering operations while maintaining the order of constituent keys.
///   </para>
///   <para>
///     Key features include:
///   </para>
///   <list type="bullet">
///     <item>
///       <description>Immutable key collection using <see cref="ImmutableArray{T}"/></description>
///     </item>
///     <item>
///       <description>Thread-static caching for 2-3 element composites</description>
///     </item>
///     <item>
///       <description>Automatic flattening of nested composite keys</description>
///     </item>
///     <item>
///       <description>Optimized hash code generation for up to 5 elements</description>
///     </item>
///     <item>
///       <description>Efficient comparison operations</description>
///     </item>
///   </list>
///   <para>
///     This class is used internally by the <see cref="DynamicKey"/> system and is not
///     typically instantiated directly by user code. Instead, use <see cref="DynamicKey.Combine(ReadOnlySpan{DynamicKey})"/>
///     or the <c>+</c> operator to create composite keys.
///   </para>
/// </remarks>
/// <seealso cref="DynamicKey"/>
[DebuggerDisplay("{" + nameof(DisplayValue) + "}")]
internal sealed class DynamicCompositeKey : DynamicKey, IComparable<DynamicCompositeKey>
{
    /// <summary>
    /// When not null, this is a cached instance used to avoid creating multiple
    /// instances of this class for the same composite pattern on the same thread.
    /// </summary>
    [ThreadStatic] private static DynamicCompositeKey? _cachedInstance;

    /// <summary>
    /// Gets the debugger display value.
    /// </summary>
    /// <value>
    /// The debugger display value.
    /// </value>
    private string DisplayValue => ToString();

    /// <summary>
    /// The cached hash code for this composite key, or null if not yet computed.
    /// </summary>
    private int? _hashCode;

    /// <summary>
    ///   Gets the immutable array of <see cref="DynamicKey"/> instances that make up this composite key.
    /// </summary>
    /// <value>
    ///   An <see cref="ImmutableArray{T}"/> containing the individual keys that comprise this composite key.
    ///   The keys are maintained in the order they were added.
    /// </value>
    /// <remarks>
    ///   This property provides access to the constituent keys of the composite. The array is
    ///   immutable, ensuring that the composite key cannot be modified after creation.
    /// </remarks>
    public ImmutableArray<DynamicKey> Keys { [DebuggerStepThrough] get; }

    /// <summary>
    ///   Gets the number of keys in this composite.
    /// </summary>
    /// <value>
    ///   The number of individual keys that make up this composite key.
    /// </value>
    /// <remarks>
    ///   This property provides a quick way to determine how many keys are contained in
    ///   the composite without accessing the <see cref="Keys"/> collection.
    /// </remarks>
    public int Count { [DebuggerStepThrough] get; }

    //--------------------------------------------------------------------------------
    /// <summary>
    /// Initializes a new instance of the <see cref="DynamicCompositeKey"/> class.
    /// </summary>
    /// <param name="keys">
    /// The span of DynamicKey instances to combine. This span of elements will be copied.
    /// </param>
    private DynamicCompositeKey (in ReadOnlySpan<DynamicKey> keys)
    {
        ImmutableArray<DynamicKey>.Builder arrayBuilder =
            ImmutableArray.CreateBuilder<DynamicKey>(keys.Length);
        for (int keyIndex = 0; keyIndex < keys.Length; keyIndex++)
        {
            DynamicKey key = keys[keyIndex];
            Debug.Assert(key is not null, "Null key in composite key");

            if (key is DynamicCompositeKey compositeKey)
                // Flatten nested composites
                arrayBuilder.AddRange(compositeKey.Keys);
            else
                arrayBuilder.Add(key);
        }
        Keys = arrayBuilder.ToImmutable();
        Count = keys.Length;
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    ///   Creates a <see cref="DynamicKey"/> instance that combines the specified keys.
    /// </summary>
    /// <param name="keys">
    ///   The span of <see cref="DynamicKey"/> instances to combine. Must contain at least one key.
    /// </param>
    /// <returns>
    ///   A <see cref="DynamicKey"/> instance that combines the specified keys.
    ///   If only one key is provided, that key is returned unchanged.
    ///   If multiple keys are provided, a <see cref="DynamicCompositeKey"/> is returned.
    /// </returns>
    /// <exception cref="ArgumentException">
    ///   Thrown when no keys are provided (empty span).
    /// </exception>
    /// <remarks>
    ///   <para>
    ///     This method creates a composite key from the specified keys with the following optimizations:
    ///   </para>
    ///   <list type="bullet">
    ///     <item>
    ///       <description>Single keys are returned unchanged to avoid unnecessary wrapping</description>
    ///     </item>
    ///     <item>
    ///       <description>Thread-static caching is used for 2-3 element composites</description>
    ///     </item>
    ///     <item>
    ///       <description>Nested composite keys are automatically flattened</description>
    ///     </item>
    ///   </list>
    ///   <para>
    ///     The method maintains the order of the constituent keys and provides optimized
    ///     hash code generation and comparison operations.
    ///   </para>
    /// </remarks>
    public static DynamicKey GetKey (params ReadOnlySpan<DynamicKey> keys)
    {
        if (keys.Length == 0)
            ThrowHelper.ThrowArgumentException_AtLeastOneKeyRequired(nameof(keys));
        if (keys.Length == 1)
            return keys[0];

        // Check for cached instance (only for 2-3 element composites for performance)
        if (keys.Length <= 3)
        {
            DynamicCompositeKey? cachedInstance = _cachedInstance;
            if ((cachedInstance is not null) && (cachedInstance.Count == keys.Length) &&
                AreKeysEqual(cachedInstance.Keys, keys))
                return cachedInstance;
        }

        DynamicCompositeKey returnInstance = new(keys);

        // Cache only 2-3 element composites
        if (keys.Length <= 3)
            _cachedInstance = returnInstance;

        return returnInstance;
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Helper method to compare two collections of keys for equality.
    /// </summary>
    /// <param name="keys1">First immutable array of keys</param>
    /// <param name="keys2">Second span of keys</param>
    /// <returns>True if collections are equal, false otherwise</returns>
    private static bool AreKeysEqual (ImmutableArray<DynamicKey> keys1, ReadOnlySpan<DynamicKey> keys2)
    {
        if (keys1.Length != keys2.Length)
            return false;

        for (int keyIndex = 0; keyIndex < keys1.Length; keyIndex++)
        {
            if (!keys1[keyIndex].Equals(keys2[keyIndex]))
                return false;
        }
        return true;
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Helper method to compare two immutable arrays of keys for equality.
    /// </summary>
    /// <param name="keys1">First immutable array of keys</param>
    /// <param name="keys2">Second immutable array of keys</param>
    /// <returns>True if arrays are equal, false otherwise</returns>
    private static bool AreKeysEqual (ImmutableArray<DynamicKey> keys1, ImmutableArray<DynamicKey> keys2)
    {
        if (keys1.Length != keys2.Length)
            return false;

        for (int keyIndex = 0; keyIndex < keys1.Length; keyIndex++)
        {
            if (!keys1[keyIndex].Equals(keys2[keyIndex]))
                return false;
        }
        return true;
    }
    //--------------------------------------------------------------------------------

    #region Overrides of Object

    //--------------------------------------------------------------------------------
    /// <inheritdoc />
    public override int GetHashCode ()
    {
        // We optimize for the common case of 1-5 elements, and we don't need to include 
        // all elements in the hash code for larger composites; this is a trade-off between
        // hash code quality and performance.
        // ReSharper disable once NonReadonlyMemberInGetHashCode
        return _hashCode ??= Count switch
        {
            1 => Keys[0].GetHashCode(),
            2 => HashCode.Combine(Keys[0].GetHashCode(), Keys[1].GetHashCode()),
            3 => HashCode.Combine(Keys[0].GetHashCode(), Keys[1].GetHashCode(), Keys[2].GetHashCode()),
            4 => HashCode.Combine(Keys[0].GetHashCode(), Keys[1].GetHashCode(), Keys[2].GetHashCode(), Keys[3].GetHashCode()),
            5 => HashCode.Combine(Keys[0].GetHashCode(), Keys[1].GetHashCode(), Keys[2].GetHashCode(), Keys[3].GetHashCode(), Keys[4].GetHashCode()),
            _ => (Count / 2) switch
            {
                3 => HashCode.Combine(Keys[0].GetHashCode(), Keys[1].GetHashCode(), Keys[2].GetHashCode(), Keys[3].GetHashCode()),
                4 => HashCode.Combine(Keys[0].GetHashCode(), Keys[1].GetHashCode(), Keys[2].GetHashCode(), Keys[3].GetHashCode()),
                5 => HashCode.Combine(Keys[0].GetHashCode(), Keys[1].GetHashCode(), Keys[2].GetHashCode(), Keys[3].GetHashCode(), Keys[4].GetHashCode()),
                _ => HashCode.Combine(Keys[0].GetHashCode(), Keys[1].GetHashCode(), Keys[2].GetHashCode(), Keys[3].GetHashCode(), Keys[4].GetHashCode(), Keys[5].GetHashCode())
            }
        };
    }
    //--------------------------------------------------------------------------------
    /// <inheritdoc />
    public override string ToString ()
    {
        return $"[{string.Join(", ", Keys.Select(key => key.ToString()))}]";
    }
    //--------------------------------------------------------------------------------

    #endregion Overrides of Object

    #region Overrides of DynamicKey

    //--------------------------------------------------------------------------------
    /// <inheritdoc />
    public override bool Equals (DynamicKey? other) =>
        (other is DynamicCompositeKey compositeKey) &&
        (ReferenceEquals(this, compositeKey) || AreKeysEqual(Keys, compositeKey.Keys));
    //--------------------------------------------------------------------------------
    /// <inheritdoc />
    public override int CompareTo (DynamicKey? other) =>
        other switch
        {
            null => 1,
            DynamicCompositeKey compositeKey => CompareTo(compositeKey),
            _ => CompareKey(other)
        };
    //--------------------------------------------------------------------------------

    #endregion

    #region IComparable<DynamicCompositeKey> Members

    //--------------------------------------------------------------------------------
    /// <inheritdoc />
    public int CompareTo (DynamicCompositeKey? other)
    {
        if (other is null)
            return 1;
        if (ReferenceEquals(this, other))
            return 0;

        // Compare by length first
        int lengthComparison = Count.CompareTo(other.Count);
        if (lengthComparison != 0)
            return lengthComparison;

        // Compare elements in order
        for (int index = 0; index < Count; index++)
        {
            int elementComparison = Keys[index].CompareTo(other.Keys[index]);
            if (elementComparison != 0)
                return elementComparison;
        }

        return 0;
    }
    //--------------------------------------------------------------------------------

    #endregion IComparable<DynamicCompositeKey> Members
}
//################################################################################
