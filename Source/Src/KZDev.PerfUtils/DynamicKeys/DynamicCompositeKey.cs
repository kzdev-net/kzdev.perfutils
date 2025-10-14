// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Collections.Immutable;
using System.Diagnostics;

using KZDev.PerfUtils.Helpers;

namespace KZDev.PerfUtils;

//################################################################################
/// <summary>
/// A type of <see cref="DynamicKey"/> that combines multiple DynamicKey instances into a single composite key
/// </summary>
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
    /// The immutable array of DynamicKey instances that make up this composite key.
    /// </summary>
    public ImmutableArray<DynamicKey> Keys { [DebuggerStepThrough] get; }

    /// <summary>
    /// The number of keys in this composite.
    /// </summary>
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
#if NET8_0_OR_GREATER
        Keys = [.. keys];
#else
        Keys = ImmutableArray.Create(keys.ToArray());
#endif
        Count = keys.Length;
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Gets a <see cref="DynamicKey"/> instance for the given span of keys.
    /// </summary>
    /// <param name="keys">
    /// The span of DynamicKey instances to combine.
    /// </param>
    /// <returns>
    /// An instance of <see cref="DynamicKey"/> that combines the specified keys.
    /// </returns>
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
        return Count switch
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
                _ => HashCode.Combine(Keys[0].GetHashCode(), Keys[1].GetHashCode(), Keys[2].GetHashCode(), Keys[3].GetHashCode(), Keys[4].GetHashCode())
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
