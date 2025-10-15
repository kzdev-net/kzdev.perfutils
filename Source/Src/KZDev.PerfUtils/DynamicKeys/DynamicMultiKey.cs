

using System.Runtime.CompilerServices;

#pragma warning disable CS0659, CS0660, CS0661
namespace KZDev.PerfUtils;

//################################################################################
/// <summary>
///   Multi-key generation methods for creating composite dynamic keys from multiple
///   values of different types. These methods provide a convenient way to create
///   complex keys without manually combining individual keys.
/// </summary>
/// <remarks>
///   <para>
///     This partial class extension provides overloaded <see cref="GetKey{T0, T1}"/> methods
///     that accept 2 to 12 parameters of different types and automatically combine them
///     into a single <see cref="DynamicKey"/> instance.
///   </para>
///   <para>
///     Each method uses <see cref="MethodImplOptions.AggressiveInlining"/> for optimal
///     performance and delegates to the appropriate <see cref="DynamicKey{T}.GetKey(T)"/>
///     methods before combining the results using <see cref="DynamicKey.Combine(ReadOnlySpan{DynamicKey})"/>.
///   </para>
///   <para>
///     These methods are particularly useful for caching scenarios where you need to
///     create keys from multiple parameters, such as method parameters, database
///     query parameters, or any combination of values that uniquely identify a result.
///   </para>
/// </remarks>
/// <example>
///   <code>
///     // Create a key from two values
///     var key2 = DynamicKey.GetKey(userId, sessionId);
///     
///     // Create a key from multiple values of different types
///     var key5 = DynamicKey.GetKey(userId, "admin", DateTime.Now.Date, true, queryParams);
///     
///     // Use in caching scenarios
///     var cacheKey = DynamicKey.GetKey(methodName, parameters, userId, timestamp);
///   </code>
/// </example>
public partial class DynamicKey
{

    //--------------------------------------------------------------------------------
    /// <summary>
    ///   Creates a composite dynamic key from two values of different types.
    /// </summary>
    /// <typeparam name="T0">The type of the first key value.</typeparam>
    /// <typeparam name="T1">The type of the second key value.</typeparam>
    /// <param name="arg0">The first key value.</param>
    /// <param name="arg1">The second key value.</param>
    /// <returns>
    ///   A <see cref="DynamicKey"/> instance that represents the combination of both values.
    /// </returns>
    /// <remarks>
    ///   This method creates individual keys for each parameter and combines them into
    ///   a single composite key. The resulting key can be used for caching, dictionary
    ///   lookups, or any scenario requiring a unique identifier based on multiple values.
    /// </remarks>
    /// <example>
    ///   <code>
    ///     var key = DynamicKey.GetKey(userId, sessionId);
    ///     cache[key] = userData;
    ///   </code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DynamicKey
        GetKey<T0, T1> (T0 arg0, T1 arg1) =>
            Combine(DynamicKey<T0>.GetKey(arg0), DynamicKey<T1>.GetKey(arg1));

    //--------------------------------------------------------------------------------
    /// <summary>
    ///   Creates a composite dynamic key from three values of different types.
    /// </summary>
    /// <typeparam name="T0">The type of the first key value.</typeparam>
    /// <typeparam name="T1">The type of the second key value.</typeparam>
    /// <typeparam name="T2">The type of the third key value.</typeparam>
    /// <param name="arg0">The first key value.</param>
    /// <param name="arg1">The second key value.</param>
    /// <param name="arg2">The third key value.</param>
    /// <returns>
    ///   A <see cref="DynamicKey"/> instance that represents the combination of all three values.
    /// </returns>
    /// <remarks>
    ///   This method creates individual keys for each parameter and combines them into
    ///   a single composite key. The resulting key can be used for caching, dictionary
    ///   lookups, or any scenario requiring a unique identifier based on multiple values.
    /// </remarks>
    /// <example>
    ///   <code>
    ///     var key = DynamicKey.GetKey(userId, "admin", DateTime.Now.Date);
    ///     cache[key] = adminData;
    ///   </code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DynamicKey
        GetKey<T0, T1, T2> (T0 arg0, T1 arg1, T2 arg2) =>
            Combine(DynamicKey<T0>.GetKey(arg0), DynamicKey<T1>.GetKey(arg1), DynamicKey<T2>.GetKey(arg2));

    //--------------------------------------------------------------------------------
    /// <summary>
    ///   Creates a composite dynamic key from four values of different types.
    /// </summary>
    /// <typeparam name="T0">The type of the first key value.</typeparam>
    /// <typeparam name="T1">The type of the second key value.</typeparam>
    /// <typeparam name="T2">The type of the third key value.</typeparam>
    /// <typeparam name="T3">The type of the fourth key value.</typeparam>
    /// <param name="arg0">The first key value.</param>
    /// <param name="arg1">The second key value.</param>
    /// <param name="arg2">The third key value.</param>
    /// <param name="arg3">The fourth key value.</param>
    /// <returns>
    ///   A <see cref="DynamicKey"/> instance that represents the combination of all four values.
    /// </returns>
    /// <remarks>
    ///   This method creates individual keys for each parameter and combines them into
    ///   a single composite key. The resulting key can be used for caching, dictionary
    ///   lookups, or any scenario requiring a unique identifier based on multiple values.
    /// </remarks>
    /// <example>
    ///   <code>
    ///     var key = DynamicKey.GetKey(userId, "admin", DateTime.Now.Date, true);
    ///     cache[key] = userPermissions;
    ///   </code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DynamicKey
        GetKey<T0, T1, T2, T3> (T0 arg0, T1 arg1, T2 arg2, T3 arg3) =>
            Combine(DynamicKey<T0>.GetKey(arg0), DynamicKey<T1>.GetKey(arg1), DynamicKey<T2>.GetKey(arg2), DynamicKey<T3>.GetKey(arg3));

    //--------------------------------------------------------------------------------
    /// <summary>
    ///   Creates a composite dynamic key from five values of different types.
    /// </summary>
    /// <typeparam name="T0">The type of the first key value.</typeparam>
    /// <typeparam name="T1">The type of the second key value.</typeparam>
    /// <typeparam name="T2">The type of the third key value.</typeparam>
    /// <typeparam name="T3">The type of the fourth key value.</typeparam>
    /// <typeparam name="T4">The type of the fifth key value.</typeparam>
    /// <param name="arg0">The first key value.</param>
    /// <param name="arg1">The second key value.</param>
    /// <param name="arg2">The third key value.</param>
    /// <param name="arg3">The fourth key value.</param>
    /// <param name="arg4">The fifth key value.</param>
    /// <returns>
    ///   A <see cref="DynamicKey"/> instance that represents the combination of all five values.
    /// </returns>
    /// <remarks>
    ///   This method creates individual keys for each parameter and combines them into
    ///   a single composite key. The resulting key can be used for caching, dictionary
    ///   lookups, or any scenario requiring a unique identifier based on multiple values.
    /// </remarks>
    /// <example>
    ///   <code>
    ///     var key = DynamicKey.GetKey(userId, "admin", DateTime.Now.Date, true, queryParams);
    ///     cache[key] = searchResults;
    ///   </code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DynamicKey
        GetKey<T0, T1, T2, T3, T4> (T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4) =>
            Combine(DynamicKey<T0>.GetKey(arg0), DynamicKey<T1>.GetKey(arg1), DynamicKey<T2>.GetKey(arg2), DynamicKey<T3>.GetKey(arg3), DynamicKey<T4>.GetKey(arg4));

    //--------------------------------------------------------------------------------
    /// <summary>
    ///   Creates a composite dynamic key from six values of different types.
    /// </summary>
    /// <typeparam name="T0">The type of the first key value.</typeparam>
    /// <typeparam name="T1">The type of the second key value.</typeparam>
    /// <typeparam name="T2">The type of the third key value.</typeparam>
    /// <typeparam name="T3">The type of the fourth key value.</typeparam>
    /// <typeparam name="T4">The type of the fifth key value.</typeparam>
    /// <typeparam name="T5">The type of the sixth key value.</typeparam>
    /// <param name="arg0">The first key value.</param>
    /// <param name="arg1">The second key value.</param>
    /// <param name="arg2">The third key value.</param>
    /// <param name="arg3">The fourth key value.</param>
    /// <param name="arg4">The fifth key value.</param>
    /// <param name="arg5">The sixth key value.</param>
    /// <returns>
    ///   A <see cref="DynamicKey"/> instance that represents the combination of all six values.
    /// </returns>
    /// <remarks>
    ///   This method creates individual keys for each parameter and combines them into
    ///   a single composite key. The resulting key can be used for caching, dictionary
    ///   lookups, or any scenario requiring a unique identifier based on multiple values.
    /// </remarks>
    /// <example>
    ///   <code>
    ///     var key = DynamicKey.GetKey(userId, "admin", DateTime.Now.Date, true, queryParams, pageSize);
    ///     cache[key] = paginatedResults;
    ///   </code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DynamicKey
        GetKey<T0, T1, T2, T3, T4, T5> (T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5) =>
            Combine(DynamicKey<T0>.GetKey(arg0), DynamicKey<T1>.GetKey(arg1), DynamicKey<T2>.GetKey(arg2), DynamicKey<T3>.GetKey(arg3), DynamicKey<T4>.GetKey(arg4), DynamicKey<T5>.GetKey(arg5));

    //--------------------------------------------------------------------------------
    /// <summary>
    ///   Creates a composite dynamic key from seven values of different types.
    /// </summary>
    /// <typeparam name="T0">The type of the first key value.</typeparam>
    /// <typeparam name="T1">The type of the second key value.</typeparam>
    /// <typeparam name="T2">The type of the third key value.</typeparam>
    /// <typeparam name="T3">The type of the fourth key value.</typeparam>
    /// <typeparam name="T4">The type of the fifth key value.</typeparam>
    /// <typeparam name="T5">The type of the sixth key value.</typeparam>
    /// <typeparam name="T6">The type of the seventh key value.</typeparam>
    /// <param name="arg0">The first key value.</param>
    /// <param name="arg1">The second key value.</param>
    /// <param name="arg2">The third key value.</param>
    /// <param name="arg3">The fourth key value.</param>
    /// <param name="arg4">The fifth key value.</param>
    /// <param name="arg5">The sixth key value.</param>
    /// <param name="arg6">The seventh key value.</param>
    /// <returns>
    ///   A <see cref="DynamicKey"/> instance that represents the combination of all seven values.
    /// </returns>
    /// <remarks>
    ///   This method creates individual keys for each parameter and combines them into
    ///   a single composite key. The resulting key can be used for caching, dictionary
    ///   lookups, or any scenario requiring a unique identifier based on multiple values.
    /// </remarks>
    /// <example>
    ///   <code>
    ///     var key = DynamicKey.GetKey(userId, "admin", DateTime.Now.Date, true, queryParams, pageSize, sortOrder);
    ///     cache[key] = sortedResults;
    ///   </code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DynamicKey
        GetKey<T0, T1, T2, T3, T4, T5, T6> (T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6) =>
            Combine(DynamicKey<T0>.GetKey(arg0), DynamicKey<T1>.GetKey(arg1), DynamicKey<T2>.GetKey(arg2), DynamicKey<T3>.GetKey(arg3), DynamicKey<T4>.GetKey(arg4), DynamicKey<T5>.GetKey(arg5), DynamicKey<T6>.GetKey(arg6));

    //--------------------------------------------------------------------------------
    /// <summary>
    ///   Creates a composite dynamic key from eight values of different types.
    /// </summary>
    /// <typeparam name="T0">The type of the first key value.</typeparam>
    /// <typeparam name="T1">The type of the second key value.</typeparam>
    /// <typeparam name="T2">The type of the third key value.</typeparam>
    /// <typeparam name="T3">The type of the fourth key value.</typeparam>
    /// <typeparam name="T4">The type of the fifth key value.</typeparam>
    /// <typeparam name="T5">The type of the sixth key value.</typeparam>
    /// <typeparam name="T6">The type of the seventh key value.</typeparam>
    /// <typeparam name="T7">The type of the eighth key value.</typeparam>
    /// <param name="arg0">The first key value.</param>
    /// <param name="arg1">The second key value.</param>
    /// <param name="arg2">The third key value.</param>
    /// <param name="arg3">The fourth key value.</param>
    /// <param name="arg4">The fifth key value.</param>
    /// <param name="arg5">The sixth key value.</param>
    /// <param name="arg6">The seventh key value.</param>
    /// <param name="arg7">The eighth key value.</param>
    /// <returns>
    ///   A <see cref="DynamicKey"/> instance that represents the combination of all eight values.
    /// </returns>
    /// <remarks>
    ///   This method creates individual keys for each parameter and combines them into
    ///   a single composite key. The resulting key can be used for caching, dictionary
    ///   lookups, or any scenario requiring a unique identifier based on multiple values.
    /// </remarks>
    /// <example>
    ///   <code>
    ///     var key = DynamicKey.GetKey(userId, "admin", DateTime.Now.Date, true, queryParams, pageSize, sortOrder, includeDeleted);
    ///     cache[key] = filteredResults;
    ///   </code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DynamicKey
        GetKey<T0, T1, T2, T3, T4, T5, T6, T7> (T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7) =>
            Combine(DynamicKey<T0>.GetKey(arg0), DynamicKey<T1>.GetKey(arg1), DynamicKey<T2>.GetKey(arg2), DynamicKey<T3>.GetKey(arg3), DynamicKey<T4>.GetKey(arg4), DynamicKey<T5>.GetKey(arg5), DynamicKey<T6>.GetKey(arg6), DynamicKey<T7>.GetKey(arg7));

    //--------------------------------------------------------------------------------
    /// <summary>
    ///   Creates a composite dynamic key from nine values of different types.
    /// </summary>
    /// <typeparam name="T0">The type of the first key value.</typeparam>
    /// <typeparam name="T1">The type of the second key value.</typeparam>
    /// <typeparam name="T2">The type of the third key value.</typeparam>
    /// <typeparam name="T3">The type of the fourth key value.</typeparam>
    /// <typeparam name="T4">The type of the fifth key value.</typeparam>
    /// <typeparam name="T5">The type of the sixth key value.</typeparam>
    /// <typeparam name="T6">The type of the seventh key value.</typeparam>
    /// <typeparam name="T7">The type of the eighth key value.</typeparam>
    /// <typeparam name="T8">The type of the ninth key value.</typeparam>
    /// <param name="arg0">The first key value.</param>
    /// <param name="arg1">The second key value.</param>
    /// <param name="arg2">The third key value.</param>
    /// <param name="arg3">The fourth key value.</param>
    /// <param name="arg4">The fifth key value.</param>
    /// <param name="arg5">The sixth key value.</param>
    /// <param name="arg6">The seventh key value.</param>
    /// <param name="arg7">The eighth key value.</param>
    /// <param name="arg8">The ninth key value.</param>
    /// <returns>
    ///   A <see cref="DynamicKey"/> instance that represents the combination of all nine values.
    /// </returns>
    /// <remarks>
    ///   This method creates individual keys for each parameter and combines them into
    ///   a single composite key. The resulting key can be used for caching, dictionary
    ///   lookups, or any scenario requiring a unique identifier based on multiple values.
    /// </remarks>
    /// <example>
    ///   <code>
    ///     var key = DynamicKey.GetKey(userId, "admin", DateTime.Now.Date, true, queryParams, pageSize, sortOrder, includeDeleted, maxResults);
    ///     cache[key] = limitedResults;
    ///   </code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DynamicKey
        GetKey<T0, T1, T2, T3, T4, T5, T6, T7, T8> (T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8) =>
            Combine(DynamicKey<T0>.GetKey(arg0), DynamicKey<T1>.GetKey(arg1), DynamicKey<T2>.GetKey(arg2), DynamicKey<T3>.GetKey(arg3), DynamicKey<T4>.GetKey(arg4), DynamicKey<T5>.GetKey(arg5), DynamicKey<T6>.GetKey(arg6), DynamicKey<T7>.GetKey(arg7), DynamicKey<T8>.GetKey(arg8));

    //--------------------------------------------------------------------------------
    /// <summary>
    ///   Creates a composite dynamic key from ten values of different types.
    /// </summary>
    /// <typeparam name="T0">The type of the first key value.</typeparam>
    /// <typeparam name="T1">The type of the second key value.</typeparam>
    /// <typeparam name="T2">The type of the third key value.</typeparam>
    /// <typeparam name="T3">The type of the fourth key value.</typeparam>
    /// <typeparam name="T4">The type of the fifth key value.</typeparam>
    /// <typeparam name="T5">The type of the sixth key value.</typeparam>
    /// <typeparam name="T6">The type of the seventh key value.</typeparam>
    /// <typeparam name="T7">The type of the eighth key value.</typeparam>
    /// <typeparam name="T8">The type of the ninth key value.</typeparam>
    /// <typeparam name="T9">The type of the tenth key value.</typeparam>
    /// <param name="arg0">The first key value.</param>
    /// <param name="arg1">The second key value.</param>
    /// <param name="arg2">The third key value.</param>
    /// <param name="arg3">The fourth key value.</param>
    /// <param name="arg4">The fifth key value.</param>
    /// <param name="arg5">The sixth key value.</param>
    /// <param name="arg6">The seventh key value.</param>
    /// <param name="arg7">The eighth key value.</param>
    /// <param name="arg8">The ninth key value.</param>
    /// <param name="arg9">The tenth key value.</param>
    /// <returns>
    ///   A <see cref="DynamicKey"/> instance that represents the combination of all ten values.
    /// </returns>
    /// <remarks>
    ///   This method creates individual keys for each parameter and combines them into
    ///   a single composite key. The resulting key can be used for caching, dictionary
    ///   lookups, or any scenario requiring a unique identifier based on multiple values.
    /// </remarks>
    /// <example>
    ///   <code>
    ///     var key = DynamicKey.GetKey(userId, "admin", DateTime.Now.Date, true, queryParams, pageSize, sortOrder, includeDeleted, maxResults, cacheTimeout);
    ///     cache[key] = cachedResults;
    ///   </code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DynamicKey
        GetKey<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9> (T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9) =>
            Combine(DynamicKey<T0>.GetKey(arg0), DynamicKey<T1>.GetKey(arg1), DynamicKey<T2>.GetKey(arg2), DynamicKey<T3>.GetKey(arg3), DynamicKey<T4>.GetKey(arg4), DynamicKey<T5>.GetKey(arg5), DynamicKey<T6>.GetKey(arg6), DynamicKey<T7>.GetKey(arg7), DynamicKey<T8>.GetKey(arg8), DynamicKey<T9>.GetKey(arg9));

    //--------------------------------------------------------------------------------
    /// <summary>
    ///   Creates a composite dynamic key from eleven values of different types.
    /// </summary>
    /// <typeparam name="T0">The type of the first key value.</typeparam>
    /// <typeparam name="T1">The type of the second key value.</typeparam>
    /// <typeparam name="T2">The type of the third key value.</typeparam>
    /// <typeparam name="T3">The type of the fourth key value.</typeparam>
    /// <typeparam name="T4">The type of the fifth key value.</typeparam>
    /// <typeparam name="T5">The type of the sixth key value.</typeparam>
    /// <typeparam name="T6">The type of the seventh key value.</typeparam>
    /// <typeparam name="T7">The type of the eighth key value.</typeparam>
    /// <typeparam name="T8">The type of the ninth key value.</typeparam>
    /// <typeparam name="T9">The type of the tenth key value.</typeparam>
    /// <typeparam name="T10">The type of the eleventh key value.</typeparam>
    /// <param name="arg0">The first key value.</param>
    /// <param name="arg1">The second key value.</param>
    /// <param name="arg2">The third key value.</param>
    /// <param name="arg3">The fourth key value.</param>
    /// <param name="arg4">The fifth key value.</param>
    /// <param name="arg5">The sixth key value.</param>
    /// <param name="arg6">The seventh key value.</param>
    /// <param name="arg7">The eighth key value.</param>
    /// <param name="arg8">The ninth key value.</param>
    /// <param name="arg9">The tenth key value.</param>
    /// <param name="arg10">The eleventh key value.</param>
    /// <returns>
    ///   A <see cref="DynamicKey"/> instance that represents the combination of all eleven values.
    /// </returns>
    /// <remarks>
    ///   This method creates individual keys for each parameter and combines them into
    ///   a single composite key. The resulting key can be used for caching, dictionary
    ///   lookups, or any scenario requiring a unique identifier based on multiple values.
    /// </remarks>
    /// <example>
    ///   <code>
    ///     var key = DynamicKey.GetKey(userId, "admin", DateTime.Now.Date, true, queryParams, pageSize, sortOrder, includeDeleted, maxResults, cacheTimeout, compressionLevel);
    ///     cache[key] = compressedResults;
    ///   </code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DynamicKey
        GetKey<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> (T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10) =>
            Combine(DynamicKey<T0>.GetKey(arg0), DynamicKey<T1>.GetKey(arg1), DynamicKey<T2>.GetKey(arg2), DynamicKey<T3>.GetKey(arg3), DynamicKey<T4>.GetKey(arg4), DynamicKey<T5>.GetKey(arg5), DynamicKey<T6>.GetKey(arg6), DynamicKey<T7>.GetKey(arg7), DynamicKey<T8>.GetKey(arg8), DynamicKey<T9>.GetKey(arg9), DynamicKey<T10>.GetKey(arg10));

    //--------------------------------------------------------------------------------
    /// <summary>
    ///   Creates a composite dynamic key from twelve values of different types.
    /// </summary>
    /// <typeparam name="T0">The type of the first key value.</typeparam>
    /// <typeparam name="T1">The type of the second key value.</typeparam>
    /// <typeparam name="T2">The type of the third key value.</typeparam>
    /// <typeparam name="T3">The type of the fourth key value.</typeparam>
    /// <typeparam name="T4">The type of the fifth key value.</typeparam>
    /// <typeparam name="T5">The type of the sixth key value.</typeparam>
    /// <typeparam name="T6">The type of the seventh key value.</typeparam>
    /// <typeparam name="T7">The type of the eighth key value.</typeparam>
    /// <typeparam name="T8">The type of the ninth key value.</typeparam>
    /// <typeparam name="T9">The type of the tenth key value.</typeparam>
    /// <typeparam name="T10">The type of the eleventh key value.</typeparam>
    /// <typeparam name="T11">The type of the twelfth key value.</typeparam>
    /// <param name="arg0">The first key value.</param>
    /// <param name="arg1">The second key value.</param>
    /// <param name="arg2">The third key value.</param>
    /// <param name="arg3">The fourth key value.</param>
    /// <param name="arg4">The fifth key value.</param>
    /// <param name="arg5">The sixth key value.</param>
    /// <param name="arg6">The seventh key value.</param>
    /// <param name="arg7">The eighth key value.</param>
    /// <param name="arg8">The ninth key value.</param>
    /// <param name="arg9">The tenth key value.</param>
    /// <param name="arg10">The eleventh key value.</param>
    /// <param name="arg11">The twelfth key value.</param>
    /// <returns>
    ///   A <see cref="DynamicKey"/> instance that represents the combination of all twelve values.
    /// </returns>
    /// <remarks>
    ///   This method creates individual keys for each parameter and combines them into
    ///   a single composite key. The resulting key can be used for caching, dictionary
    ///   lookups, or any scenario requiring a unique identifier based on multiple values.
    /// </remarks>
    /// <example>
    ///   <code>
    ///     var key = DynamicKey.GetKey(userId, "admin", DateTime.Now.Date, true, queryParams, pageSize, sortOrder, includeDeleted, maxResults, cacheTimeout, compressionLevel, encryptionKey);
    ///     cache[key] = secureResults;
    ///   </code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DynamicKey
        GetKey<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> (T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11) =>
            Combine(DynamicKey<T0>.GetKey(arg0), DynamicKey<T1>.GetKey(arg1), DynamicKey<T2>.GetKey(arg2), DynamicKey<T3>.GetKey(arg3), DynamicKey<T4>.GetKey(arg4), DynamicKey<T5>.GetKey(arg5), DynamicKey<T6>.GetKey(arg6), DynamicKey<T7>.GetKey(arg7), DynamicKey<T8>.GetKey(arg8), DynamicKey<T9>.GetKey(arg9), DynamicKey<T10>.GetKey(arg10), DynamicKey<T11>.GetKey(arg11));
    //--------------------------------------------------------------------------------
}
//################################################################################
#pragma warning restore CS0659, CS0660, CS0661
