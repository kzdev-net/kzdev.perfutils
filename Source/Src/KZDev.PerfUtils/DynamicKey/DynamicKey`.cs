// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

namespace KZDev.PerfUtils;

//################################################################################
/// <summary>
/// A generic type that can be used as a key for scenarios where a dynamic number and
/// type of keys are needed, such as caching scenarios, where one of the existing
/// <see cref="DynamicKey"/> types is not sufficient.
/// </summary>
/// <remarks>
/// Each derived class should implement <see cref="object.GetHashCode"/>
/// </remarks>
public static class DynamicKey<T>
{
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Gets a <see cref="DynamicKey"/> instance for the given value.
    /// </summary>
    /// <param name="value">
    /// The value to use as a key. This may be null if T is a reference type.
    /// </param>
    /// <returns>
    /// An instance of <see cref="DynamicKey"/> that uses the specified value to
    /// represent the key or partial key.
    /// </returns>
    public static DynamicKey GetKey (T value)
    {
        if (value is null)
            return DynamicKey.Null;
        if (value is DynamicKey dynamicKey)
            return dynamicKey;
        switch (value)
        {
            case int intValue:
                return DynamicIntKey.GetKey (intValue);

            case bool boolValue:
                return DynamicBoolKey.GetKey (boolValue);

            case string stringValue:
                return DynamicStringKey.GetKey (stringValue);

            case Guid guidValue:
                return DynamicGuidKey.GetKey (guidValue);

            case Type typeValue:
                return DynamicTypeKey.GetKey (typeValue);

            default:
                return typeof(T) == typeof(object) ?
                    DynamicObjectKey.GetKey (value) :
                    value.GetType().IsValueType ?
                        DynamicValKey<T>.GetKey (value) :
                        DynamicRefKey<T>.GetKey (value);
        }
    }
    //--------------------------------------------------------------------------------
}
//################################################################################