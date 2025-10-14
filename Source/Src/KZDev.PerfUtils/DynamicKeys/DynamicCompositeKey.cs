// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Diagnostics;

namespace KZDev.PerfUtils;

//################################################################################
/// <summary>
/// A type of <see cref="DynamicKey"/> that combines multiple DynamicKey instances into a single composite key
/// </summary>
[DebuggerDisplay("{" + nameof(DisplayValue) + "}")]
internal class DynamicCompositeKey : DynamicKey, IComparable<DynamicCompositeKey>
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
    /// The array of DynamicKey instances that make up this composite key.
    /// </summary>
    public DynamicKey[] Keys { [DebuggerStepThrough] get; }

    /// <summary>
    /// The number of keys in this composite.
    /// </summary>
    public int Count { [DebuggerStepThrough] get; }

    //--------------------------------------------------------------------------------
    /// <summary>
    /// Initializes a new instance of the <see cref="DynamicCompositeKey"/> class.
    /// </summary>
    /// <param name="keys">
    /// The array of DynamicKey instances to combine. This array will be copied.
    /// </param>
    private DynamicCompositeKey (DynamicKey[] keys)
    {
        Keys = new DynamicKey[keys.Length];
        Array.Copy(keys, Keys, keys.Length);
        Count = keys.Length;
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Gets a <see cref="DynamicKey"/> instance for the given array of keys.
    /// </summary>
    /// <param name="keys">
    /// The array of DynamicKey instances to combine.
    /// </param>
    /// <returns>
    /// An instance of <see cref="DynamicKey"/> that combines the specified keys.
    /// </returns>
    public static DynamicKey GetKey (params DynamicKey[] keys)
    {
        if (keys.Length == 0)
            throw new ArgumentException("At least one key must be provided", nameof(keys));
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
    /// Helper method to compare two arrays of keys for equality.
    /// </summary>
    /// <param name="keys1">First array of keys</param>
    /// <param name="keys2">Second array of keys</param>
    /// <returns>True if arrays are equal, false otherwise</returns>
    private static bool AreKeysEqual (DynamicKey[] keys1, DynamicKey[] keys2)
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
        return Count switch
        {
            1 => Keys[0].GetHashCode(),
            2 => HashCode.Combine(Keys[0].GetHashCode(), Keys[1].GetHashCode()),
            3 => HashCode.Combine(Keys[0].GetHashCode(), Keys[1].GetHashCode(), Keys[2].GetHashCode()),
            4 => HashCode.Combine(Keys[0].GetHashCode(), Keys[1].GetHashCode(), Keys[2].GetHashCode(), Keys[3].GetHashCode()),
            5 => HashCode.Combine(Keys[0].GetHashCode(), Keys[1].GetHashCode(), Keys[2].GetHashCode(), Keys[3].GetHashCode(), Keys[4].GetHashCode()),
            _ => GetHashCodeForMany()
        };
    }
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Calculates hash code for composites with more than 5 elements.
    /// </summary>
    /// <returns>Combined hash code</returns>
    private int GetHashCodeForMany ()
    {
        HashCode hashCode = new();
        foreach (DynamicKey key in Keys)
        {
            hashCode.Add(key.GetHashCode());
        }
        return hashCode.ToHashCode();
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
