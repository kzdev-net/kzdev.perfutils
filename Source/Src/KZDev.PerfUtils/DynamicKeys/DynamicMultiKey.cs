

using System.Runtime.CompilerServices;

#pragma warning disable CS0659, CS0660, CS0661
namespace KZDev.PerfUtils;

//################################################################################
/// <summary>
/// A general type that can be used as a key for scenarios where a dynamic number and
/// type of keys are needed, such as caching scenarios.
/// </summary>
public partial class DynamicKey 
{
	
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Gets an aggregate dynamic key instance from the passed constituent key values.
    /// </summary>
    /// <typeparam name="T0">The type of the first key value.</typeparam>
    /// <typeparam name="T1">The type of the second key value.</typeparam>
    /// <param name="arg0">The first key value.</param>
    /// <param name="arg1">The second key value.</param>  
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DynamicKey 
        GetKey<T0, T1> (T0 arg0, T1 arg1) =>
            Combine(DynamicKey<T0>.GetKey(arg0), DynamicKey<T1>.GetKey(arg1));
    	
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Gets an aggregate dynamic key instance from the passed constituent key values.
    /// </summary>
    /// <typeparam name="T0">The type of the first key value.</typeparam>
    /// <typeparam name="T1">The type of the second key value.</typeparam>
    /// <typeparam name="T2">The type of the third key value.</typeparam>
    /// <param name="arg0">The first key value.</param>
    /// <param name="arg1">The second key value.</param>
    /// <param name="arg2">The third key value.</param>  
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DynamicKey 
        GetKey<T0, T1, T2> (T0 arg0, T1 arg1, T2 arg2) =>
            Combine(DynamicKey<T0>.GetKey(arg0), DynamicKey<T1>.GetKey(arg1), DynamicKey<T2>.GetKey(arg2));
    	
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Gets an aggregate dynamic key instance from the passed constituent key values.
    /// </summary>
    /// <typeparam name="T0">The type of the first key value.</typeparam>
    /// <typeparam name="T1">The type of the second key value.</typeparam>
    /// <typeparam name="T2">The type of the third key value.</typeparam>
    /// <typeparam name="T3">The type of the fourth key value.</typeparam>
    /// <param name="arg0">The first key value.</param>
    /// <param name="arg1">The second key value.</param>
    /// <param name="arg2">The third key value.</param>
    /// <param name="arg3">The fourth key value.</param>  
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DynamicKey 
        GetKey<T0, T1, T2, T3> (T0 arg0, T1 arg1, T2 arg2, T3 arg3) =>
            Combine(DynamicKey<T0>.GetKey(arg0), DynamicKey<T1>.GetKey(arg1), DynamicKey<T2>.GetKey(arg2), DynamicKey<T3>.GetKey(arg3));
    	
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Gets an aggregate dynamic key instance from the passed constituent key values.
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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DynamicKey 
        GetKey<T0, T1, T2, T3, T4> (T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4) =>
            Combine(DynamicKey<T0>.GetKey(arg0), DynamicKey<T1>.GetKey(arg1), DynamicKey<T2>.GetKey(arg2), DynamicKey<T3>.GetKey(arg3), DynamicKey<T4>.GetKey(arg4));
    	
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Gets an aggregate dynamic key instance from the passed constituent key values.
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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DynamicKey 
        GetKey<T0, T1, T2, T3, T4, T5> (T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5) =>
            Combine(DynamicKey<T0>.GetKey(arg0), DynamicKey<T1>.GetKey(arg1), DynamicKey<T2>.GetKey(arg2), DynamicKey<T3>.GetKey(arg3), DynamicKey<T4>.GetKey(arg4), DynamicKey<T5>.GetKey(arg5));
    	
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Gets an aggregate dynamic key instance from the passed constituent key values.
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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DynamicKey 
        GetKey<T0, T1, T2, T3, T4, T5, T6> (T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6) =>
            Combine(DynamicKey<T0>.GetKey(arg0), DynamicKey<T1>.GetKey(arg1), DynamicKey<T2>.GetKey(arg2), DynamicKey<T3>.GetKey(arg3), DynamicKey<T4>.GetKey(arg4), DynamicKey<T5>.GetKey(arg5), DynamicKey<T6>.GetKey(arg6));
    	
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Gets an aggregate dynamic key instance from the passed constituent key values.
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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DynamicKey 
        GetKey<T0, T1, T2, T3, T4, T5, T6, T7> (T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7) =>
            Combine(DynamicKey<T0>.GetKey(arg0), DynamicKey<T1>.GetKey(arg1), DynamicKey<T2>.GetKey(arg2), DynamicKey<T3>.GetKey(arg3), DynamicKey<T4>.GetKey(arg4), DynamicKey<T5>.GetKey(arg5), DynamicKey<T6>.GetKey(arg6), DynamicKey<T7>.GetKey(arg7));
    	
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Gets an aggregate dynamic key instance from the passed constituent key values.
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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DynamicKey 
        GetKey<T0, T1, T2, T3, T4, T5, T6, T7, T8> (T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8) =>
            Combine(DynamicKey<T0>.GetKey(arg0), DynamicKey<T1>.GetKey(arg1), DynamicKey<T2>.GetKey(arg2), DynamicKey<T3>.GetKey(arg3), DynamicKey<T4>.GetKey(arg4), DynamicKey<T5>.GetKey(arg5), DynamicKey<T6>.GetKey(arg6), DynamicKey<T7>.GetKey(arg7), DynamicKey<T8>.GetKey(arg8));
    	
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Gets an aggregate dynamic key instance from the passed constituent key values.
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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DynamicKey 
        GetKey<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9> (T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9) =>
            Combine(DynamicKey<T0>.GetKey(arg0), DynamicKey<T1>.GetKey(arg1), DynamicKey<T2>.GetKey(arg2), DynamicKey<T3>.GetKey(arg3), DynamicKey<T4>.GetKey(arg4), DynamicKey<T5>.GetKey(arg5), DynamicKey<T6>.GetKey(arg6), DynamicKey<T7>.GetKey(arg7), DynamicKey<T8>.GetKey(arg8), DynamicKey<T9>.GetKey(arg9));
    	
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Gets an aggregate dynamic key instance from the passed constituent key values.
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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DynamicKey 
        GetKey<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> (T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10) =>
            Combine(DynamicKey<T0>.GetKey(arg0), DynamicKey<T1>.GetKey(arg1), DynamicKey<T2>.GetKey(arg2), DynamicKey<T3>.GetKey(arg3), DynamicKey<T4>.GetKey(arg4), DynamicKey<T5>.GetKey(arg5), DynamicKey<T6>.GetKey(arg6), DynamicKey<T7>.GetKey(arg7), DynamicKey<T8>.GetKey(arg8), DynamicKey<T9>.GetKey(arg9), DynamicKey<T10>.GetKey(arg10));
    	
    //--------------------------------------------------------------------------------
    /// <summary>
    /// Gets an aggregate dynamic key instance from the passed constituent key values.
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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DynamicKey 
        GetKey<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> (T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11) =>
            Combine(DynamicKey<T0>.GetKey(arg0), DynamicKey<T1>.GetKey(arg1), DynamicKey<T2>.GetKey(arg2), DynamicKey<T3>.GetKey(arg3), DynamicKey<T4>.GetKey(arg4), DynamicKey<T5>.GetKey(arg5), DynamicKey<T6>.GetKey(arg6), DynamicKey<T7>.GetKey(arg7), DynamicKey<T8>.GetKey(arg8), DynamicKey<T9>.GetKey(arg9), DynamicKey<T10>.GetKey(arg10), DynamicKey<T11>.GetKey(arg11));
    //--------------------------------------------------------------------------------
}
//################################################################################
#pragma warning restore CS0659, CS0660, CS0661
