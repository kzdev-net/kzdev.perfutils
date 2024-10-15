namespace KZDev.PerfUtils
{
    //################################################################################
    /// <summary>
    /// A collection of helper methods for atomic operations.
    /// </summary>
    public static class InterlockedOps
    {
        #region Xor

        //--------------------------------------------------------------------------------
        /// <summary>
        /// A thread-safe exclusive OR operation on a variable value.
        /// </summary>
        /// <param name="location1">
        /// A variable containing the first value to perform the XOR operation on. The result
        /// is stored in this variable.
        /// </param>
        /// <param name="value">
        /// The value to be combined with the value in <paramref name="location1"/>
        /// </param>
        /// <returns>
        /// A tuple containing the original value and the new value after the operation.
        /// </returns>
        public static (int originalValue, int newValue) Xor (ref int location1, int value)
        {
            int original, result;
            do
            {
                original = location1;
                result = original ^ value;
            }
            while (Interlocked.CompareExchange(ref location1, result, original) != original);
            return (original, result);
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// A thread-safe exclusive OR operation on a variable value.
        /// </summary>
        /// <param name="location1">
        /// A variable containing the first value to perform the XOR operation on. The result
        /// is stored in this variable.
        /// </param>
        /// <param name="value">
        /// The value to be combined with the value in <paramref name="location1"/>
        /// </param>
        /// <returns>
        /// A tuple containing the original value and the new value after the operation.
        /// </returns>
        public static (uint originalValue, uint newValue) Xor (ref uint location1, uint value)
        {
            uint original, result;
            do
            {
                original = location1;
                result = original ^ value;
            }
            while (Interlocked.CompareExchange(ref location1, result, original) != original);
            return (original, result);
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// A thread-safe exclusive OR operation on a variable value.
        /// </summary>
        /// <param name="location1">
        /// A variable containing the first value to perform the XOR operation on. The result
        /// is stored in this variable.
        /// </param>
        /// <param name="value">
        /// The value to be combined with the value in <paramref name="location1"/>
        /// </param>
        /// <returns>
        /// A tuple containing the original value and the new value after the operation.
        /// </returns>
        public static (long originalValue, long newValue) Xor (ref long location1, long value)
        {
            long original, result;
            do
            {
                original = location1;
                result = original ^ value;
            }
            while (Interlocked.CompareExchange(ref location1, result, original) != original);
            return (original, result);
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// A thread-safe exclusive OR operation on a variable value.
        /// </summary>
        /// <param name="location1">
        /// A variable containing the first value to perform the XOR operation on. The result
        /// is stored in this variable.
        /// </param>
        /// <param name="value">
        /// The value to be combined with the value in <paramref name="location1"/>
        /// </param>
        /// <returns>
        /// A tuple containing the original value and the new value after the operation.
        /// </returns>
        public static (ulong originalValue, ulong newValue) Xor (ref ulong location1, ulong value)
        {
            ulong original, result;
            do
            {
                original = location1;
                result = original ^ value;
            }
            while (Interlocked.CompareExchange(ref location1, result, original) != original);
            return (original, result);
        }
        //--------------------------------------------------------------------------------

        #endregion Xor

        #region SetBits

        //--------------------------------------------------------------------------------
        /// <summary>
        /// A thread-safe method to set a specific set of bits in a variable.
        /// </summary>
        /// <param name="location1">
        /// A variable containing the bits to set. The result is stored in this variable.
        /// </param>
        /// <param name="setBitsValue">
        /// The value that holds the bits to set and store in <paramref name="location1"/>
        /// </param>
        /// <returns>
        /// A tuple containing the original value and the new value after the operation.
        /// </returns>
        /// <remarks>
        /// This is just a convenience method that uses the <see cref="Interlocked.Or(ref int, int)"/>
        /// operation to set the bits, and returns the original and new values.
        /// </remarks>
        public static (int originalValue, int newValue) SetBits (ref int location1, int setBitsValue)
        {
            int original = Interlocked.Or(ref location1, setBitsValue);
            return (original, original | setBitsValue);
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// A thread-safe method to set a specific set of bits in a variable.
        /// </summary>
        /// <param name="location1">
        /// A variable containing the bits to set. The result is stored in this variable.
        /// </param>
        /// <param name="setBitsValue">
        /// The value that holds the bits to set and store in <paramref name="location1"/>
        /// </param>
        /// <returns>
        /// A tuple containing the original value and the new value after the operation.
        /// </returns>
        /// <remarks>
        /// This is just a convenience method that uses the <see cref="Interlocked.Or(ref int, int)"/>
        /// operation to set the bits, and returns the original and new values.
        /// </remarks>
        public static (uint originalValue, uint newValue) SetBits (ref uint location1, uint setBitsValue)
        {
            uint original = Interlocked.Or(ref location1, setBitsValue);
            return (original, original | setBitsValue);
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// A thread-safe method to set a specific set of bits in a variable.
        /// </summary>
        /// <param name="location1">
        /// A variable containing the bits to set. The result is stored in this variable.
        /// </param>
        /// <param name="setBitsValue">
        /// The value that holds the bits to set and store in <paramref name="location1"/>
        /// </param>
        /// <returns>
        /// A tuple containing the original value and the new value after the operation.
        /// </returns>
        /// <remarks>
        /// This is just a convenience method that uses the <see cref="Interlocked.Or(ref int, int)"/>
        /// operation to set the bits, and returns the original and new values.
        /// </remarks>
        public static (long originalValue, long newValue) SetBits (ref long location1, long setBitsValue)
        {
            long original = Interlocked.Or(ref location1, setBitsValue);
            return (original, original | setBitsValue);
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// A thread-safe method to set a specific set of bits in a variable.
        /// </summary>
        /// <param name="location1">
        /// A variable containing the bits to set. The result is stored in this variable.
        /// </param>
        /// <param name="setBitsValue">
        /// The value that holds the bits to set and store in <paramref name="location1"/>
        /// </param>
        /// <returns>
        /// A tuple containing the original value and the new value after the operation.
        /// </returns>
        /// <remarks>
        /// This is just a convenience method that uses the <see cref="Interlocked.Or(ref int, int)"/>
        /// operation to set the bits, and returns the original and new values.
        /// </remarks>
        public static (ulong originalValue, ulong newValue) SetBits (ref ulong location1, ulong setBitsValue)
        {
            ulong original = Interlocked.Or(ref location1, setBitsValue);
            return (original, original | setBitsValue);
        }
        //--------------------------------------------------------------------------------

        #endregion SetBits

        #region ClearBits

        //--------------------------------------------------------------------------------
        /// <summary>
        /// A thread-safe method to clear a specific set of bits in a variable.
        /// </summary>
        /// <param name="location1">
        /// A variable containing the bits to clear. The result is stored in this variable.
        /// </param>
        /// <param name="clearBitsValue">
        /// The value that holds the bits to clear in <paramref name="location1"/>
        /// </param>
        /// <returns>
        /// A tuple containing the original value and the new value after the operation.
        /// </returns>
        /// <remarks>
        /// This is just a convenience method that could be done with <see cref="Interlocked.And(ref int, int)"/>
        /// passing the bitwise complement of the <paramref name="clearBitsValue"/>, but this 
        /// method returns the original and new values in a tuple.
        /// </remarks>
        public static (int originalValue, int newValue) ClearBits (ref int location1, int clearBitsValue)
        {
            int original, result;
            do
            {
                original = location1;
                result = original & ~clearBitsValue;
            }
            while (Interlocked.CompareExchange(ref location1, result, original) != original);
            return (original, result);
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// A thread-safe method to clear a specific set of bits in a variable.
        /// </summary>
        /// <param name="location1">
        /// A variable containing the bits to clear. The result is stored in this variable.
        /// </param>
        /// <param name="clearBitsValue">
        /// The value that holds the bits to clear in <paramref name="location1"/>
        /// </param>
        /// <returns>
        /// A tuple containing the original value and the new value after the operation.
        /// </returns>
        /// <remarks>
        /// This is just a convenience method that could be done with <see cref="Interlocked.And(ref int, int)"/>
        /// passing the bitwise complement of the <paramref name="clearBitsValue"/>, but this 
        /// method returns the original and new values in a tuple.
        /// </remarks>
        public static (uint originalValue, uint newValue) ClearBits (ref uint location1, uint clearBitsValue)
        {
            uint original, result;
            do
            {
                original = location1;
                result = original & ~clearBitsValue;
            }
            while (Interlocked.CompareExchange(ref location1, result, original) != original);
            return (original, result);
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// A thread-safe method to clear a specific set of bits in a variable.
        /// </summary>
        /// <param name="location1">
        /// A variable containing the bits to clear. The result is stored in this variable.
        /// </param>
        /// <param name="clearBitsValue">
        /// The value that holds the bits to clear in <paramref name="location1"/>
        /// </param>
        /// <returns>
        /// A tuple containing the original value and the new value after the operation.
        /// </returns>
        /// <remarks>
        /// This is just a convenience method that could be done with <see cref="Interlocked.And(ref int, int)"/>
        /// passing the bitwise complement of the <paramref name="clearBitsValue"/>, but this 
        /// method returns the original and new values in a tuple.
        /// </remarks>
        public static (long originalValue, long newValue) ClearBits (ref long location1, long clearBitsValue)
        {
            long original, result;
            do
            {
                original = location1;
                result = original & ~clearBitsValue;
            }
            while (Interlocked.CompareExchange(ref location1, result, original) != original);
            return (original, result);
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// A thread-safe method to clear a specific set of bits in a variable.
        /// </summary>
        /// <param name="location1">
        /// A variable containing the bits to clear. The result is stored in this variable.
        /// </param>
        /// <param name="clearBitsValue">
        /// The value that holds the bits to clear in <paramref name="location1"/>
        /// </param>
        /// <returns>
        /// A tuple containing the original value and the new value after the operation.
        /// </returns>
        /// <remarks>
        /// This is just a convenience method that could be done with <see cref="Interlocked.And(ref int, int)"/>
        /// passing the bitwise complement of the <paramref name="clearBitsValue"/>, but this 
        /// method returns the original and new values in a tuple.
        /// </remarks>
        public static (ulong originalValue, ulong newValue) ClearBits (ref ulong location1, ulong clearBitsValue)
        {
            ulong original, result;
            do
            {
                original = location1;
                result = original & ~clearBitsValue;
            }
            while (Interlocked.CompareExchange(ref location1, result, original) != original);
            return (original, result);
        }
        //--------------------------------------------------------------------------------

        #endregion ClearBits

        #region Conditional Xor

        //--------------------------------------------------------------------------------
        /// <summary>
        /// A thread-safe conditional exclusive OR operation on a variable value.
        /// </summary>
        /// <param name="location1">
        /// A variable containing the first value to perform the XOR operation on. The result
        /// is stored in this variable.
        /// </param>
        /// <param name="condition">
        /// The condition that must be met for the operation to proceed.
        /// </param>
        /// <param name="value">
        /// The value to be combined with the value in <paramref name="location1"/>
        /// </param>
        /// <returns>
        /// A tuple containing the original value and the new value after the operation.
        /// If the condition is not met, the original value is returned in both fields.
        /// </returns>
        /// <remarks>
        /// This method uses a <see cref="Predicate{T}"/> to determine if the operation 
        /// should proceed based on the current value of <paramref name="location1"/>. If
        /// <paramref name="condition"/> returns <see langword="true"/>, then the XOR
        /// result is stored in <paramref name="location1"/> if the current value is the
        /// same as the value passed to the condition. If the value has changed in the time
        /// between being read, and processed by <paramref name="condition"/>, the operation
        /// is repeated until either the condition is not met, or the operation is successful.
        /// </remarks>
        public static (int originalValue, int newValue) ConditionXor (ref int location1,
            Predicate<int> condition, int value)
        {
            int original, result;
            do
            {
                original = location1;
                if (!condition(original))
                {
                    return (original, original);
                }
                result = original ^ value;
            }
            while (Interlocked.CompareExchange(ref location1, result, original) != original);
            return (original, result);
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// A thread-safe conditional exclusive OR operation on a variable value with the
        /// condition based on a function that takes the current value and an argument.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the argument to pass to the condition function.
        /// </typeparam>
        /// <param name="location1">
        /// A variable containing the first value to perform the XOR operation on. The result
        /// is stored in this variable.
        /// </param>
        /// <param name="condition">
        /// The condition that must be met for the operation to proceed.
        /// </param>
        /// <param name="argument">
        /// The argument to pass to the condition function <paramref name="condition"/>
        /// </param>
        /// <param name="value">
        /// The value to be combined with the value in <paramref name="location1"/>
        /// </param>
        /// <returns>
        /// A tuple containing the original value and the new value after the operation.
        /// If the condition is not met, the original value is returned in both fields.
        /// </returns>
        /// <remarks>
        /// This method uses a <see cref="Predicate{T}"/> to determine if the operation 
        /// should proceed based on the current value of <paramref name="location1"/>. If
        /// <paramref name="condition"/> returns <see langword="true"/>, then the XOR
        /// result is stored in <paramref name="location1"/> if the current value is the
        /// same as the value passed to the condition. If the value has changed in the time
        /// between being read, and processed by <paramref name="condition"/>, the operation
        /// is repeated until either the condition is not met, or the operation is successful.
        /// </remarks>
        public static (int originalValue, int newValue) ConditionXor<T> (ref int location1,
            Func<int, T, bool> condition, T argument, int value)
        {
            int original, result;
            do
            {
                original = location1;
                if (!condition(original, argument))
                {
                    return (original, original);
                }
                result = original ^ value;
            }
            while (Interlocked.CompareExchange(ref location1, result, original) != original);
            return (original, result);
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// A thread-safe conditional exclusive OR operation on a variable value.
        /// </summary>
        /// <param name="location1">
        /// A variable containing the first value to perform the XOR operation on. The result
        /// is stored in this variable.
        /// </param>
        /// <param name="condition">
        /// The condition that must be met for the operation to proceed.
        /// </param>
        /// <param name="value">
        /// The value to be combined with the value in <paramref name="location1"/>
        /// </param>
        /// <returns>
        /// A tuple containing the original value and the new value after the operation.
        /// If the condition is not met, the original value is returned in both fields.
        /// </returns>
        /// <remarks>
        /// This method uses a <see cref="Predicate{T}"/> to determine if the operation 
        /// should proceed based on the current value of <paramref name="location1"/>. If
        /// <paramref name="condition"/> returns <see langword="true"/>, then the XOR
        /// result is stored in <paramref name="location1"/> if the current value is the
        /// same as the value passed to the condition. If the value has changed in the time
        /// between being read, and processed by <paramref name="condition"/>, the operation
        /// is repeated until either the condition is not met, or the operation is successful.
        /// </remarks>
        public static (uint originalValue, uint newValue) ConditionXor (ref uint location1,
            Predicate<uint> condition, uint value)
        {
            uint original, result;
            do
            {
                original = location1;
                if (!condition(original))
                {
                    return (original, original);
                }
                result = original ^ value;
            }
            while (Interlocked.CompareExchange(ref location1, result, original) != original);
            return (original, result);
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// A thread-safe conditional exclusive OR operation on a variable value with the
        /// condition based on a function that takes the current value and an argument.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the argument to pass to the condition function.
        /// </typeparam>
        /// <param name="location1">
        /// A variable containing the first value to perform the XOR operation on. The result
        /// is stored in this variable.
        /// </param>
        /// <param name="condition">
        /// The condition that must be met for the operation to proceed.
        /// </param>
        /// <param name="argument">
        /// The argument to pass to the condition function <paramref name="condition"/>
        /// </param>
        /// <param name="value">
        /// The value to be combined with the value in <paramref name="location1"/>
        /// </param>
        /// <returns>
        /// A tuple containing the original value and the new value after the operation.
        /// If the condition is not met, the original value is returned in both fields.
        /// </returns>
        /// <remarks>
        /// This method uses a <see cref="Predicate{T}"/> to determine if the operation 
        /// should proceed based on the current value of <paramref name="location1"/>. If
        /// <paramref name="condition"/> returns <see langword="true"/>, then the XOR
        /// result is stored in <paramref name="location1"/> if the current value is the
        /// same as the value passed to the condition. If the value has changed in the time
        /// between being read, and processed by <paramref name="condition"/>, the operation
        /// is repeated until either the condition is not met, or the operation is successful.
        /// </remarks>
        public static (uint originalValue, uint newValue) ConditionXor<T> (ref uint location1,
            Func<uint, T, bool> condition, T argument, uint value)
        {
            uint original, result;
            do
            {
                original = location1;
                if (!condition(original, argument))
                {
                    return (original, original);
                }
                result = original ^ value;
            }
            while (Interlocked.CompareExchange(ref location1, result, original) != original);
            return (original, result);
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// A thread-safe conditional exclusive OR operation on a variable value.
        /// </summary>
        /// <param name="location1">
        /// A variable containing the first value to perform the XOR operation on. The result
        /// is stored in this variable.
        /// </param>
        /// <param name="condition">
        /// The condition that must be met for the operation to proceed.
        /// </param>
        /// <param name="value">
        /// The value to be combined with the value in <paramref name="location1"/>
        /// </param>
        /// <returns>
        /// A tuple containing the original value and the new value after the operation.
        /// If the condition is not met, the original value is returned in both fields.
        /// </returns>
        /// <remarks>
        /// This method uses a <see cref="Predicate{T}"/> to determine if the operation 
        /// should proceed based on the current value of <paramref name="location1"/>. If
        /// <paramref name="condition"/> returns <see langword="true"/>, then the XOR
        /// result is stored in <paramref name="location1"/> if the current value is the
        /// same as the value passed to the condition. If the value has changed in the time
        /// between being read, and processed by <paramref name="condition"/>, the operation
        /// is repeated until either the condition is not met, or the operation is successful.
        /// </remarks>
        public static (long originalValue, long newValue) ConditionXor (ref long location1,
            Predicate<long> condition, long value)
        {
            long original, result;
            do
            {
                original = location1;
                if (!condition(original))
                {
                    return (original, original);
                }
                result = original ^ value;
            }
            while (Interlocked.CompareExchange(ref location1, result, original) != original);
            return (original, result);
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// A thread-safe conditional exclusive OR operation on a variable value with the
        /// condition based on a function that takes the current value and an argument.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the argument to pass to the condition function.
        /// </typeparam>
        /// <param name="location1">
        /// A variable containing the first value to perform the XOR operation on. The result
        /// is stored in this variable.
        /// </param>
        /// <param name="condition">
        /// The condition that must be met for the operation to proceed.
        /// </param>
        /// <param name="argument">
        /// The argument to pass to the condition function <paramref name="condition"/>
        /// </param>
        /// <param name="value">
        /// The value to be combined with the value in <paramref name="location1"/>
        /// </param>
        /// <returns>
        /// A tuple containing the original value and the new value after the operation.
        /// If the condition is not met, the original value is returned in both fields.
        /// </returns>
        /// <remarks>
        /// This method uses a <see cref="Predicate{T}"/> to determine if the operation 
        /// should proceed based on the current value of <paramref name="location1"/>. If
        /// <paramref name="condition"/> returns <see langword="true"/>, then the XOR
        /// result is stored in <paramref name="location1"/> if the current value is the
        /// same as the value passed to the condition. If the value has changed in the time
        /// between being read, and processed by <paramref name="condition"/>, the operation
        /// is repeated until either the condition is not met, or the operation is successful.
        /// </remarks>
        public static (long originalValue, long newValue) ConditionXor<T> (ref long location1,
            Func<long, T, bool> condition, T argument, long value)
        {
            long original, result;
            do
            {
                original = location1;
                if (!condition(original, argument))
                {
                    return (original, original);
                }
                result = original ^ value;
            }
            while (Interlocked.CompareExchange(ref location1, result, original) != original);
            return (original, result);
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// A thread-safe conditional exclusive OR operation on a variable value.
        /// </summary>
        /// <param name="location1">
        /// A variable containing the first value to perform the XOR operation on. The result
        /// is stored in this variable.
        /// </param>
        /// <param name="condition">
        /// The condition that must be met for the operation to proceed.
        /// </param>
        /// <param name="value">
        /// The value to be combined with the value in <paramref name="location1"/>
        /// </param>
        /// <returns>
        /// A tuple containing the original value and the new value after the operation.
        /// If the condition is not met, the original value is returned in both fields.
        /// </returns>
        /// <remarks>
        /// This method uses a <see cref="Predicate{T}"/> to determine if the operation 
        /// should proceed based on the current value of <paramref name="location1"/>. If
        /// <paramref name="condition"/> returns <see langword="true"/>, then the XOR
        /// result is stored in <paramref name="location1"/> if the current value is the
        /// same as the value passed to the condition. If the value has changed in the time
        /// between being read, and processed by <paramref name="condition"/>, the operation
        /// is repeated until either the condition is not met, or the operation is successful.
        /// </remarks>
        public static (ulong originalValue, ulong newValue) ConditionXor (ref ulong location1,
            Predicate<ulong> condition, ulong value)
        {
            ulong original, result;
            do
            {
                original = location1;
                if (!condition(original))
                {
                    return (original, original);
                }
                result = original ^ value;
            }
            while (Interlocked.CompareExchange(ref location1, result, original) != original);
            return (original, result);
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// A thread-safe conditional exclusive OR operation on a variable value with the
        /// condition based on a function that takes the current value and an argument.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the argument to pass to the condition function.
        /// </typeparam>
        /// <param name="location1">
        /// A variable containing the first value to perform the XOR operation on. The result
        /// is stored in this variable.
        /// </param>
        /// <param name="condition">
        /// The condition that must be met for the operation to proceed.
        /// </param>
        /// <param name="argument">
        /// The argument to pass to the condition function <paramref name="condition"/>
        /// </param>
        /// <param name="value">
        /// The value to be combined with the value in <paramref name="location1"/>
        /// </param>
        /// <returns>
        /// A tuple containing the original value and the new value after the operation.
        /// If the condition is not met, the original value is returned in both fields.
        /// </returns>
        /// <remarks>
        /// This method uses a <see cref="Predicate{T}"/> to determine if the operation 
        /// should proceed based on the current value of <paramref name="location1"/>. If
        /// <paramref name="condition"/> returns <see langword="true"/>, then the XOR
        /// result is stored in <paramref name="location1"/> if the current value is the
        /// same as the value passed to the condition. If the value has changed in the time
        /// between being read, and processed by <paramref name="condition"/>, the operation
        /// is repeated until either the condition is not met, or the operation is successful.
        /// </remarks>
        public static (ulong originalValue, ulong newValue) ConditionXor<T> (ref ulong location1,
            Func<ulong, T, bool> condition, T argument, ulong value)
        {
            ulong original, result;
            do
            {
                original = location1;
                if (!condition(original, argument))
                {
                    return (original, original);
                }
                result = original ^ value;
            }
            while (Interlocked.CompareExchange(ref location1, result, original) != original);
            return (original, result);
        }
        //--------------------------------------------------------------------------------

        #endregion Conditional Xor

        #region Conditional And

        //--------------------------------------------------------------------------------
        /// <summary>
        /// A thread-safe conditional AND operation on a variable value.
        /// </summary>
        /// <param name="location1">
        /// A variable containing the first value to perform the XOR operation on. The result
        /// is stored in this variable.
        /// </param>
        /// <param name="condition">
        /// The condition that must be met for the operation to proceed.
        /// </param>
        /// <param name="value">
        /// The value to be combined with the value in <paramref name="location1"/>
        /// </param>
        /// <returns>
        /// A tuple containing the original value and the new value after the operation.
        /// If the condition is not met, the original value is returned in both fields.
        /// </returns>
        /// <remarks>
        /// This method uses a <see cref="Predicate{T}"/> to determine if the operation 
        /// should proceed based on the current value of <paramref name="location1"/>. If
        /// <paramref name="condition"/> returns <see langword="true"/>, then the AND
        /// result is stored in <paramref name="location1"/> if the current value is the
        /// same as the value passed to the condition. If the value has changed in the time
        /// between being read, and processed by <paramref name="condition"/>, the operation
        /// is repeated until either the condition is not met, or the operation is successful.
        /// </remarks>
        public static (int originalValue, int newValue) ConditionAnd (ref int location1,
            Predicate<int> condition, int value)
        {
            int original, result;
            do
            {
                original = location1;
                if (!condition(original))
                {
                    return (original, original);
                }
                result = original ^ value;
            }
            while (Interlocked.CompareExchange(ref location1, result, original) != original);
            return (original, result);
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// A thread-safe conditional AND operation on a variable value with the
        /// condition based on a function that takes the current value and an argument.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the argument to pass to the condition function.
        /// </typeparam>
        /// <param name="location1">
        /// A variable containing the first value to perform the XOR operation on. The result
        /// is stored in this variable.
        /// </param>
        /// <param name="condition">
        /// The condition that must be met for the operation to proceed.
        /// </param>
        /// <param name="argument">
        /// The argument to pass to the condition function <paramref name="condition"/>
        /// </param>
        /// <param name="value">
        /// The value to be combined with the value in <paramref name="location1"/>
        /// </param>
        /// <returns>
        /// A tuple containing the original value and the new value after the operation.
        /// If the condition is not met, the original value is returned in both fields.
        /// </returns>
        /// <remarks>
        /// This method uses a <see cref="Predicate{T}"/> to determine if the operation 
        /// should proceed based on the current value of <paramref name="location1"/>. If
        /// <paramref name="condition"/> returns <see langword="true"/>, then the AND
        /// result is stored in <paramref name="location1"/> if the current value is the
        /// same as the value passed to the condition. If the value has changed in the time
        /// between being read, and processed by <paramref name="condition"/>, the operation
        /// is repeated until either the condition is not met, or the operation is successful.
        /// </remarks>
        public static (int originalValue, int newValue) ConditionAnd<T> (ref int location1,
            Func<int, T, bool> condition, T argument, int value)
        {
            int original, result;
            do
            {
                original = location1;
                if (!condition(original, argument))
                {
                    return (original, original);
                }
                result = original ^ value;
            }
            while (Interlocked.CompareExchange(ref location1, result, original) != original);
            return (original, result);
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// A thread-safe conditional AND operation on a variable value.
        /// </summary>
        /// <param name="location1">
        /// A variable containing the first value to perform the XOR operation on. The result
        /// is stored in this variable.
        /// </param>
        /// <param name="condition">
        /// The condition that must be met for the operation to proceed.
        /// </param>
        /// <param name="value">
        /// The value to be combined with the value in <paramref name="location1"/>
        /// </param>
        /// <returns>
        /// A tuple containing the original value and the new value after the operation.
        /// If the condition is not met, the original value is returned in both fields.
        /// </returns>
        /// <remarks>
        /// This method uses a <see cref="Predicate{T}"/> to determine if the operation 
        /// should proceed based on the current value of <paramref name="location1"/>. If
        /// <paramref name="condition"/> returns <see langword="true"/>, then the AND
        /// result is stored in <paramref name="location1"/> if the current value is the
        /// same as the value passed to the condition. If the value has changed in the time
        /// between being read, and processed by <paramref name="condition"/>, the operation
        /// is repeated until either the condition is not met, or the operation is successful.
        /// </remarks>
        public static (uint originalValue, uint newValue) ConditionAnd (ref uint location1,
            Predicate<uint> condition, uint value)
        {
            uint original, result;
            do
            {
                original = location1;
                if (!condition(original))
                {
                    return (original, original);
                }
                result = original & value;
            }
            while (Interlocked.CompareExchange(ref location1, result, original) != original);
            return (original, result);
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// A thread-safe conditional AND operation on a variable value with the
        /// condition based on a function that takes the current value and an argument.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the argument to pass to the condition function.
        /// </typeparam>
        /// <param name="location1">
        /// A variable containing the first value to perform the XOR operation on. The result
        /// is stored in this variable.
        /// </param>
        /// <param name="condition">
        /// The condition that must be met for the operation to proceed.
        /// </param>
        /// <param name="argument">
        /// The argument to pass to the condition function <paramref name="condition"/>
        /// </param>
        /// <param name="value">
        /// The value to be combined with the value in <paramref name="location1"/>
        /// </param>
        /// <returns>
        /// A tuple containing the original value and the new value after the operation.
        /// If the condition is not met, the original value is returned in both fields.
        /// </returns>
        /// <remarks>
        /// This method uses a <see cref="Predicate{T}"/> to determine if the operation 
        /// should proceed based on the current value of <paramref name="location1"/>. If
        /// <paramref name="condition"/> returns <see langword="true"/>, then the AND
        /// result is stored in <paramref name="location1"/> if the current value is the
        /// same as the value passed to the condition. If the value has changed in the time
        /// between being read, and processed by <paramref name="condition"/>, the operation
        /// is repeated until either the condition is not met, or the operation is successful.
        /// </remarks>
        public static (uint originalValue, uint newValue) ConditionAnd<T> (ref uint location1,
            Func<uint, T, bool> condition, T argument, uint value)
        {
            uint original, result;
            do
            {
                original = location1;
                if (!condition(original, argument))
                {
                    return (original, original);
                }
                result = original & value;
            }
            while (Interlocked.CompareExchange(ref location1, result, original) != original);
            return (original, result);
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// A thread-safe conditional AND operation on a variable value.
        /// </summary>
        /// <param name="location1">
        /// A variable containing the first value to perform the XOR operation on. The result
        /// is stored in this variable.
        /// </param>
        /// <param name="condition">
        /// The condition that must be met for the operation to proceed.
        /// </param>
        /// <param name="value">
        /// The value to be combined with the value in <paramref name="location1"/>
        /// </param>
        /// <returns>
        /// A tuple containing the original value and the new value after the operation.
        /// If the condition is not met, the original value is returned in both fields.
        /// </returns>
        /// <remarks>
        /// This method uses a <see cref="Predicate{T}"/> to determine if the operation 
        /// should proceed based on the current value of <paramref name="location1"/>. If
        /// <paramref name="condition"/> returns <see langword="true"/>, then the AND
        /// result is stored in <paramref name="location1"/> if the current value is the
        /// same as the value passed to the condition. If the value has changed in the time
        /// between being read, and processed by <paramref name="condition"/>, the operation
        /// is repeated until either the condition is not met, or the operation is successful.
        /// </remarks>
        public static (long originalValue, long newValue) ConditionAnd (ref long location1,
            Predicate<long> condition, long value)
        {
            long original, result;
            do
            {
                original = location1;
                if (!condition(original))
                {
                    return (original, original);
                }
                result = original & value;
            }
            while (Interlocked.CompareExchange(ref location1, result, original) != original);
            return (original, result);
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// A thread-safe conditional AND operation on a variable value with the
        /// condition based on a function that takes the current value and an argument.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the argument to pass to the condition function.
        /// </typeparam>
        /// <param name="location1">
        /// A variable containing the first value to perform the XOR operation on. The result
        /// is stored in this variable.
        /// </param>
        /// <param name="condition">
        /// The condition that must be met for the operation to proceed.
        /// </param>
        /// <param name="argument">
        /// The argument to pass to the condition function <paramref name="condition"/>
        /// </param>
        /// <param name="value">
        /// The value to be combined with the value in <paramref name="location1"/>
        /// </param>
        /// <returns>
        /// A tuple containing the original value and the new value after the operation.
        /// If the condition is not met, the original value is returned in both fields.
        /// </returns>
        /// <remarks>
        /// This method uses a <see cref="Predicate{T}"/> to determine if the operation 
        /// should proceed based on the current value of <paramref name="location1"/>. If
        /// <paramref name="condition"/> returns <see langword="true"/>, then the AND
        /// result is stored in <paramref name="location1"/> if the current value is the
        /// same as the value passed to the condition. If the value has changed in the time
        /// between being read, and processed by <paramref name="condition"/>, the operation
        /// is repeated until either the condition is not met, or the operation is successful.
        /// </remarks>
        public static (long originalValue, long newValue) ConditionAnd<T> (ref long location1,
            Func<long, T, bool> condition, T argument, long value)
        {
            long original, result;
            do
            {
                original = location1;
                if (!condition(original, argument))
                {
                    return (original, original);
                }
                result = original & value;
            }
            while (Interlocked.CompareExchange(ref location1, result, original) != original);
            return (original, result);
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// A thread-safe conditional AND operation on a variable value.
        /// </summary>
        /// <param name="location1">
        /// A variable containing the first value to perform the XOR operation on. The result
        /// is stored in this variable.
        /// </param>
        /// <param name="condition">
        /// The condition that must be met for the operation to proceed.
        /// </param>
        /// <param name="value">
        /// The value to be combined with the value in <paramref name="location1"/>
        /// </param>
        /// <returns>
        /// A tuple containing the original value and the new value after the operation.
        /// If the condition is not met, the original value is returned in both fields.
        /// </returns>
        /// <remarks>
        /// This method uses a <see cref="Predicate{T}"/> to determine if the operation 
        /// should proceed based on the current value of <paramref name="location1"/>. If
        /// <paramref name="condition"/> returns <see langword="true"/>, then the AND
        /// result is stored in <paramref name="location1"/> if the current value is the
        /// same as the value passed to the condition. If the value has changed in the time
        /// between being read, and processed by <paramref name="condition"/>, the operation
        /// is repeated until either the condition is not met, or the operation is successful.
        /// </remarks>
        public static (ulong originalValue, ulong newValue) ConditionAnd (ref ulong location1,
            Predicate<ulong> condition, ulong value)
        {
            ulong original, result;
            do
            {
                original = location1;
                if (!condition(original))
                {
                    return (original, original);
                }
                result = original & value;
            }
            while (Interlocked.CompareExchange(ref location1, result, original) != original);
            return (original, result);
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// A thread-safe conditional AND operation on a variable value with the
        /// condition based on a function that takes the current value and an argument.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the argument to pass to the condition function.
        /// </typeparam>
        /// <param name="location1">
        /// A variable containing the first value to perform the XOR operation on. The result
        /// is stored in this variable.
        /// </param>
        /// <param name="condition">
        /// The condition that must be met for the operation to proceed.
        /// </param>
        /// <param name="argument">
        /// The argument to pass to the condition function <paramref name="condition"/>
        /// </param>
        /// <param name="value">
        /// The value to be combined with the value in <paramref name="location1"/>
        /// </param>
        /// <returns>
        /// A tuple containing the original value and the new value after the operation.
        /// If the condition is not met, the original value is returned in both fields.
        /// </returns>
        /// <remarks>
        /// This method uses a <see cref="Predicate{T}"/> to determine if the operation 
        /// should proceed based on the current value of <paramref name="location1"/>. If
        /// <paramref name="condition"/> returns <see langword="true"/>, then the AND
        /// result is stored in <paramref name="location1"/> if the current value is the
        /// same as the value passed to the condition. If the value has changed in the time
        /// between being read, and processed by <paramref name="condition"/>, the operation
        /// is repeated until either the condition is not met, or the operation is successful.
        /// </remarks>
        public static (ulong originalValue, ulong newValue) ConditionAnd<T> (ref ulong location1,
            Func<ulong, T, bool> condition, T argument, ulong value)
        {
            ulong original, result;
            do
            {
                original = location1;
                if (!condition(original, argument))
                {
                    return (original, original);
                }
                result = original & value;
            }
            while (Interlocked.CompareExchange(ref location1, result, original) != original);
            return (original, result);
        }
        //--------------------------------------------------------------------------------

        #endregion Conditional And

        #region Conditional Or

        //--------------------------------------------------------------------------------
        /// <summary>
        /// A thread-safe conditional OR operation on a variable value.
        /// </summary>
        /// <param name="location1">
        /// A variable containing the first value to perform the XOR operation on. The result
        /// is stored in this variable.
        /// </param>
        /// <param name="condition">
        /// The condition that must be met for the operation to proceed.
        /// </param>
        /// <param name="value">
        /// The value to be combined with the value in <paramref name="location1"/>
        /// </param>
        /// <returns>
        /// A tuple containing the original value and the new value after the operation.
        /// If the condition is not met, the original value is returned in both fields.
        /// </returns>
        /// <remarks>
        /// This method uses a <see cref="Predicate{T}"/> to determine if the operation 
        /// should proceed based on the current value of <paramref name="location1"/>. If
        /// <paramref name="condition"/> returns <see langword="true"/>, then the OR
        /// result is stored in <paramref name="location1"/> if the current value is the
        /// same as the value passed to the condition. If the value has changed in the time
        /// between being read, and processed by <paramref name="condition"/>, the operation
        /// is repeated until either the condition is not met, or the operation is successful.
        /// </remarks>
        public static (int originalValue, int newValue) ConditionOr (ref int location1,
            Predicate<int> condition, int value)
        {
            int original, result;
            do
            {
                original = location1;
                if (!condition(original))
                {
                    return (original, original);
                }
                result = original | value;
            }
            while (Interlocked.CompareExchange(ref location1, result, original) != original);
            return (original, result);
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// A thread-safe conditional OR operation on a variable value with the
        /// condition based on a function that takes the current value and an argument.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the argument to pass to the condition function.
        /// </typeparam>
        /// <param name="location1">
        /// A variable containing the first value to perform the XOR operation on. The result
        /// is stored in this variable.
        /// </param>
        /// <param name="condition">
        /// The condition that must be met for the operation to proceed.
        /// </param>
        /// <param name="argument">
        /// The argument to pass to the condition function <paramref name="condition"/>
        /// </param>
        /// <param name="value">
        /// The value to be combined with the value in <paramref name="location1"/>
        /// </param>
        /// <returns>
        /// A tuple containing the original value and the new value after the operation.
        /// If the condition is not met, the original value is returned in both fields.
        /// </returns>
        /// <remarks>
        /// This method uses a <see cref="Predicate{T}"/> to determine if the operation 
        /// should proceed based on the current value of <paramref name="location1"/>. If
        /// <paramref name="condition"/> returns <see langword="true"/>, then the OR
        /// result is stored in <paramref name="location1"/> if the current value is the
        /// same as the value passed to the condition. If the value has changed in the time
        /// between being read, and processed by <paramref name="condition"/>, the operation
        /// is repeated until either the condition is not met, or the operation is successful.
        /// </remarks>
        public static (int originalValue, int newValue) ConditionOr<T> (ref int location1,
            Func<int, T, bool> condition, T argument, int value)
        {
            int original, result;
            do
            {
                original = location1;
                if (!condition(original, argument))
                {
                    return (original, original);
                }
                result = original | value;
            }
            while (Interlocked.CompareExchange(ref location1, result, original) != original);
            return (original, result);
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// A thread-safe conditional OR operation on a variable value.
        /// </summary>
        /// <param name="location1">
        /// A variable containing the first value to perform the XOR operation on. The result
        /// is stored in this variable.
        /// </param>
        /// <param name="condition">
        /// The condition that must be met for the operation to proceed.
        /// </param>
        /// <param name="value">
        /// The value to be combined with the value in <paramref name="location1"/>
        /// </param>
        /// <returns>
        /// A tuple containing the original value and the new value after the operation.
        /// If the condition is not met, the original value is returned in both fields.
        /// </returns>
        /// <remarks>
        /// This method uses a <see cref="Predicate{T}"/> to determine if the operation 
        /// should proceed based on the current value of <paramref name="location1"/>. If
        /// <paramref name="condition"/> returns <see langword="true"/>, then the OR
        /// result is stored in <paramref name="location1"/> if the current value is the
        /// same as the value passed to the condition. If the value has changed in the time
        /// between being read, and processed by <paramref name="condition"/>, the operation
        /// is repeated until either the condition is not met, or the operation is successful.
        /// </remarks>
        public static (uint originalValue, uint newValue) ConditionOr (ref uint location1,
            Predicate<uint> condition, uint value)
        {
            uint original, result;
            do
            {
                original = location1;
                if (!condition(original))
                {
                    return (original, original);
                }
                result = original | value;
            }
            while (Interlocked.CompareExchange(ref location1, result, original) != original);
            return (original, result);
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// A thread-safe conditional OR operation on a variable value with the
        /// condition based on a function that takes the current value and an argument.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the argument to pass to the condition function.
        /// </typeparam>
        /// <param name="location1">
        /// A variable containing the first value to perform the XOR operation on. The result
        /// is stored in this variable.
        /// </param>
        /// <param name="condition">
        /// The condition that must be met for the operation to proceed.
        /// </param>
        /// <param name="argument">
        /// The argument to pass to the condition function <paramref name="condition"/>
        /// </param>
        /// <param name="value">
        /// The value to be combined with the value in <paramref name="location1"/>
        /// </param>
        /// <returns>
        /// A tuple containing the original value and the new value after the operation.
        /// If the condition is not met, the original value is returned in both fields.
        /// </returns>
        /// <remarks>
        /// This method uses a <see cref="Predicate{T}"/> to determine if the operation 
        /// should proceed based on the current value of <paramref name="location1"/>. If
        /// <paramref name="condition"/> returns <see langword="true"/>, then the OR
        /// result is stored in <paramref name="location1"/> if the current value is the
        /// same as the value passed to the condition. If the value has changed in the time
        /// between being read, and processed by <paramref name="condition"/>, the operation
        /// is repeated until either the condition is not met, or the operation is successful.
        /// </remarks>
        public static (uint originalValue, uint newValue) ConditionOr<T> (ref uint location1,
            Func<uint, T, bool> condition, T argument, uint value)
        {
            uint original, result;
            do
            {
                original = location1;
                if (!condition(original, argument))
                {
                    return (original, original);
                }
                result = original | value;
            }
            while (Interlocked.CompareExchange(ref location1, result, original) != original);
            return (original, result);
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// A thread-safe conditional OR operation on a variable value.
        /// </summary>
        /// <param name="location1">
        /// A variable containing the first value to perform the XOR operation on. The result
        /// is stored in this variable.
        /// </param>
        /// <param name="condition">
        /// The condition that must be met for the operation to proceed.
        /// </param>
        /// <param name="value">
        /// The value to be combined with the value in <paramref name="location1"/>
        /// </param>
        /// <returns>
        /// A tuple containing the original value and the new value after the operation.
        /// If the condition is not met, the original value is returned in both fields.
        /// </returns>
        /// <remarks>
        /// This method uses a <see cref="Predicate{T}"/> to determine if the operation 
        /// should proceed based on the current value of <paramref name="location1"/>. If
        /// <paramref name="condition"/> returns <see langword="true"/>, then the OR
        /// result is stored in <paramref name="location1"/> if the current value is the
        /// same as the value passed to the condition. If the value has changed in the time
        /// between being read, and processed by <paramref name="condition"/>, the operation
        /// is repeated until either the condition is not met, or the operation is successful.
        /// </remarks>
        public static (long originalValue, long newValue) ConditionOr (ref long location1,
            Predicate<long> condition, long value)
        {
            long original, result;
            do
            {
                original = location1;
                if (!condition(original))
                {
                    return (original, original);
                }
                result = original | value;
            }
            while (Interlocked.CompareExchange(ref location1, result, original) != original);
            return (original, result);
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// A thread-safe conditional OR operation on a variable value with the
        /// condition based on a function that takes the current value and an argument.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the argument to pass to the condition function.
        /// </typeparam>
        /// <param name="location1">
        /// A variable containing the first value to perform the XOR operation on. The result
        /// is stored in this variable.
        /// </param>
        /// <param name="condition">
        /// The condition that must be met for the operation to proceed.
        /// </param>
        /// <param name="argument">
        /// The argument to pass to the condition function <paramref name="condition"/>
        /// </param>
        /// <param name="value">
        /// The value to be combined with the value in <paramref name="location1"/>
        /// </param>
        /// <returns>
        /// A tuple containing the original value and the new value after the operation.
        /// If the condition is not met, the original value is returned in both fields.
        /// </returns>
        /// <remarks>
        /// This method uses a <see cref="Predicate{T}"/> to determine if the operation 
        /// should proceed based on the current value of <paramref name="location1"/>. If
        /// <paramref name="condition"/> returns <see langword="true"/>, then the OR
        /// result is stored in <paramref name="location1"/> if the current value is the
        /// same as the value passed to the condition. If the value has changed in the time
        /// between being read, and processed by <paramref name="condition"/>, the operation
        /// is repeated until either the condition is not met, or the operation is successful.
        /// </remarks>
        public static (long originalValue, long newValue) ConditionOr<T> (ref long location1,
            Func<long, T, bool> condition, T argument, long value)
        {
            long original, result;
            do
            {
                original = location1;
                if (!condition(original, argument))
                {
                    return (original, original);
                }
                result = original | value;
            }
            while (Interlocked.CompareExchange(ref location1, result, original) != original);
            return (original, result);
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// A thread-safe conditional OR operation on a variable value.
        /// </summary>
        /// <param name="location1">
        /// A variable containing the first value to perform the XOR operation on. The result
        /// is stored in this variable.
        /// </param>
        /// <param name="condition">
        /// The condition that must be met for the operation to proceed.
        /// </param>
        /// <param name="value">
        /// The value to be combined with the value in <paramref name="location1"/>
        /// </param>
        /// <returns>
        /// A tuple containing the original value and the new value after the operation.
        /// If the condition is not met, the original value is returned in both fields.
        /// </returns>
        /// <remarks>
        /// This method uses a <see cref="Predicate{T}"/> to determine if the operation 
        /// should proceed based on the current value of <paramref name="location1"/>. If
        /// <paramref name="condition"/> returns <see langword="true"/>, then the OR
        /// result is stored in <paramref name="location1"/> if the current value is the
        /// same as the value passed to the condition. If the value has changed in the time
        /// between being read, and processed by <paramref name="condition"/>, the operation
        /// is repeated until either the condition is not met, or the operation is successful.
        /// </remarks>
        public static (ulong originalValue, ulong newValue) ConditionOr (ref ulong location1,
            Predicate<ulong> condition, ulong value)
        {
            ulong original, result;
            do
            {
                original = location1;
                if (!condition(original))
                {
                    return (original, original);
                }
                result = original | value;
            }
            while (Interlocked.CompareExchange(ref location1, result, original) != original);
            return (original, result);
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// A thread-safe conditional OR operation on a variable value with the
        /// condition based on a function that takes the current value and an argument.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the argument to pass to the condition function.
        /// </typeparam>
        /// <param name="location1">
        /// A variable containing the first value to perform the XOR operation on. The result
        /// is stored in this variable.
        /// </param>
        /// <param name="condition">
        /// The condition that must be met for the operation to proceed.
        /// </param>
        /// <param name="argument">
        /// The argument to pass to the condition function <paramref name="condition"/>
        /// </param>
        /// <param name="value">
        /// The value to be combined with the value in <paramref name="location1"/>
        /// </param>
        /// <returns>
        /// A tuple containing the original value and the new value after the operation.
        /// If the condition is not met, the original value is returned in both fields.
        /// </returns>
        /// <remarks>
        /// This method uses a <see cref="Predicate{T}"/> to determine if the operation 
        /// should proceed based on the current value of <paramref name="location1"/>. If
        /// <paramref name="condition"/> returns <see langword="true"/>, then the OR
        /// result is stored in <paramref name="location1"/> if the current value is the
        /// same as the value passed to the condition. If the value has changed in the time
        /// between being read, and processed by <paramref name="condition"/>, the operation
        /// is repeated until either the condition is not met, or the operation is successful.
        /// </remarks>
        public static (ulong originalValue, ulong newValue) ConditionOr<T> (ref ulong location1,
            Func<ulong, T, bool> condition, T argument, ulong value)
        {
            ulong original, result;
            do
            {
                original = location1;
                if (!condition(original, argument))
                {
                    return (original, original);
                }
                result = original | value;
            }
            while (Interlocked.CompareExchange(ref location1, result, original) != original);
            return (original, result);
        }
        //--------------------------------------------------------------------------------

        #endregion Conditional Or

        #region Conditional SetBits

        //--------------------------------------------------------------------------------
        /// <summary>
        /// A thread-safe method to conditionally set a specific set of bits in a variable.
        /// </summary>
        /// <param name="location1">
        /// A variable containing the bits to set. The result is stored in this variable.
        /// </param>
        /// <param name="condition">
        /// The condition that must be met for the operation to proceed.
        /// </param>
        /// <param name="setBitsValue">
        /// The value that holds the bits to set and store in <paramref name="location1"/>
        /// If the condition is not met, the original value is returned in both fields.
        /// </param>
        /// <returns>
        /// A tuple containing the original value and the new value after the operation.
        /// </returns>
        /// <remarks>
        /// This method is just a convenience method that is a semantic alias for 
        /// <see cref="ConditionOr(ref int, Predicate{int}, int)"/>
        /// </remarks>
        public static (int originalValue, int newValue) ConditionSetBits (ref int location1,
            Predicate<int> condition, int setBitsValue) => ConditionOr(ref location1, condition, setBitsValue);
        //--------------------------------------------------------------------------------
        /// <summary>
        /// A thread-safe method to conditionally set a specific set of bits in a variable
        /// with the condition based on a function that takes the current value and an argument.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the argument to pass to the condition function.
        /// </typeparam>
        /// <param name="location1">
        /// A variable containing the bits to set. The result is stored in this variable.
        /// </param>
        /// <param name="condition">
        /// The condition that must be met for the operation to proceed.
        /// </param>
        /// <param name="argument">
        /// The argument to pass to the condition function <paramref name="condition"/>
        /// </param>
        /// <param name="setBitsValue">
        /// The value that holds the bits to set and store in <paramref name="location1"/>
        /// If the condition is not met, the original value is returned in both fields.
        /// </param>
        /// <returns>
        /// A tuple containing the original value and the new value after the operation.
        /// </returns>
        /// <remarks>
        /// This method is just a convenience method that is a semantic alias for 
        /// <see cref="ConditionOr(ref int, Predicate{int}, int)"/>
        /// </remarks>
        public static (int originalValue, int newValue) ConditionSetBits<T> (ref int location1,
            Func<int, T, bool> condition, T argument, int setBitsValue) =>
            ConditionOr(ref location1, condition, argument, setBitsValue);
        //--------------------------------------------------------------------------------
        /// <summary>
        /// A thread-safe method to conditionally set a specific set of bits in a variable.
        /// </summary>
        /// <param name="location1">
        /// A variable containing the bits to set. The result is stored in this variable.
        /// </param>
        /// <param name="condition">
        /// The condition that must be met for the operation to proceed.
        /// </param>
        /// <param name="setBitsValue">
        /// The value that holds the bits to set and store in <paramref name="location1"/>
        /// If the condition is not met, the original value is returned in both fields.
        /// </param>
        /// <returns>
        /// A tuple containing the original value and the new value after the operation.
        /// </returns>
        /// <remarks>
        /// This method is just a convenience method that is a semantic alias for 
        /// <see cref="ConditionOr(ref uint, Predicate{uint}, uint)"/>
        /// </remarks>
        public static (uint originalValue, uint newValue) ConditionSetBits (ref uint location1,
            Predicate<uint> condition, uint setBitsValue) => ConditionOr(ref location1, condition, setBitsValue);
        //--------------------------------------------------------------------------------
        /// <summary>
        /// A thread-safe method to conditionally set a specific set of bits in a variable
        /// with the condition based on a function that takes the current value and an argument.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the argument to pass to the condition function.
        /// </typeparam>
        /// <param name="location1">
        /// A variable containing the bits to set. The result is stored in this variable.
        /// </param>
        /// <param name="condition">
        /// The condition that must be met for the operation to proceed.
        /// </param>
        /// <param name="argument">
        /// The argument to pass to the condition function <paramref name="condition"/>
        /// </param>
        /// <param name="setBitsValue">
        /// The value that holds the bits to set and store in <paramref name="location1"/>
        /// If the condition is not met, the original value is returned in both fields.
        /// </param>
        /// <returns>
        /// A tuple containing the original value and the new value after the operation.
        /// </returns>
        /// <remarks>
        /// This method is just a convenience method that is a semantic alias for 
        /// <see cref="ConditionOr(ref uint, Predicate{uint}, uint)"/>
        /// </remarks>
        public static (uint originalValue, uint newValue) ConditionSetBits<T> (ref uint location1,
            Func<uint, T, bool> condition, T argument, uint setBitsValue) =>
            ConditionOr(ref location1, condition, argument, setBitsValue);
        //--------------------------------------------------------------------------------
        /// <summary>
        /// A thread-safe method to conditionally set a specific set of bits in a variable.
        /// </summary>
        /// <param name="location1">
        /// A variable containing the bits to set. The result is stored in this variable.
        /// </param>
        /// <param name="condition">
        /// The condition that must be met for the operation to proceed.
        /// </param>
        /// <param name="setBitsValue">
        /// The value that holds the bits to set and store in <paramref name="location1"/>
        /// </param>
        /// <returns>
        /// A tuple containing the original value and the new value after the operation.
        /// If the condition is not met, the original value is returned in both fields.
        /// </returns>
        /// <remarks>
        /// This method is just a convenience method that is a semantic alias for 
        /// <see cref="ConditionOr(ref long, Predicate{long}, long)"/>
        /// </remarks>
        public static (long originalValue, long newValue) ConditionSetBits (ref long location1,
            Predicate<long> condition, long setBitsValue) => ConditionOr(ref location1, condition, setBitsValue);
        //--------------------------------------------------------------------------------
        /// <summary>
        /// A thread-safe method to conditionally set a specific set of bits in a variable
        /// with the condition based on a function that takes the current value and an argument.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the argument to pass to the condition function.
        /// </typeparam>
        /// <param name="location1">
        /// A variable containing the bits to set. The result is stored in this variable.
        /// </param>
        /// <param name="condition">
        /// The condition that must be met for the operation to proceed.
        /// </param>
        /// <param name="argument">
        /// The argument to pass to the condition function <paramref name="condition"/>
        /// </param>
        /// <param name="setBitsValue">
        /// The value that holds the bits to set and store in <paramref name="location1"/>
        /// </param>
        /// <returns>
        /// A tuple containing the original value and the new value after the operation.
        /// If the condition is not met, the original value is returned in both fields.
        /// </returns>
        /// <remarks>
        /// This method is just a convenience method that is a semantic alias for 
        /// <see cref="ConditionOr(ref long, Predicate{long}, long)"/>
        /// </remarks>
        public static (long originalValue, long newValue) ConditionSetBits<T> (ref long location1,
            Func<long, T, bool> condition, T argument, long setBitsValue) =>
            ConditionOr(ref location1, condition, argument, setBitsValue);
        //--------------------------------------------------------------------------------
        /// <summary>
        /// A thread-safe method to conditionally set a specific set of bits in a variable.
        /// </summary>
        /// <param name="location1">
        /// A variable containing the bits to set. The result is stored in this variable.
        /// </param>
        /// <param name="condition">
        /// The condition that must be met for the operation to proceed.
        /// </param>
        /// <param name="setBitsValue">
        /// The value that holds the bits to set and store in <paramref name="location1"/>
        /// </param>
        /// <returns>
        /// A tuple containing the original value and the new value after the operation.
        /// If the condition is not met, the original value is returned in both fields.
        /// </returns>
        /// <remarks>
        /// This method is just a convenience method that is a semantic alias for 
        /// <see cref="ConditionOr(ref ulong, Predicate{ulong}, ulong)"/>
        /// </remarks>
        public static (ulong originalValue, ulong newValue) ConditionSetBits (ref ulong location1,
            Predicate<ulong> condition, ulong setBitsValue) => ConditionOr(ref location1, condition, setBitsValue);
        //--------------------------------------------------------------------------------
        /// <summary>
        /// A thread-safe method to conditionally set a specific set of bits in a variable
        /// with the condition based on a function that takes the current value and an argument.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the argument to pass to the condition function.
        /// </typeparam>
        /// <param name="location1">
        /// A variable containing the bits to set. The result is stored in this variable.
        /// </param>
        /// <param name="condition">
        /// The condition that must be met for the operation to proceed.
        /// </param>
        /// <param name="argument">
        /// The argument to pass to the condition function <paramref name="condition"/>
        /// </param>
        /// <param name="setBitsValue">
        /// The value that holds the bits to set and store in <paramref name="location1"/>
        /// </param>
        /// <returns>
        /// A tuple containing the original value and the new value after the operation.
        /// If the condition is not met, the original value is returned in both fields.
        /// </returns>
        /// <remarks>
        /// This method is just a convenience method that is a semantic alias for 
        /// <see cref="ConditionOr(ref ulong, Predicate{ulong}, ulong)"/>
        /// </remarks>
        public static (ulong originalValue, ulong newValue) ConditionSetBits<T> (ref ulong location1,
            Func<ulong, T, bool> condition, T argument, ulong setBitsValue) =>
            ConditionOr(ref location1, condition, argument, setBitsValue);
        //--------------------------------------------------------------------------------

        #endregion Conditional SetBits

        #region Conditional ClearBits

        //--------------------------------------------------------------------------------
        /// <summary>
        /// A thread-safe method to conditionally clear a specific set of bits in a variable.
        /// </summary>
        /// <param name="location1">
        /// A variable containing the bits to clear. The result is stored in this variable.
        /// </param>
        /// <param name="condition">
        /// The condition that must be met for the operation to proceed.
        /// </param>
        /// <param name="clearBitsValue">
        /// The value that holds the bits to clear in <paramref name="location1"/>
        /// </param>
        /// <returns>
        /// A tuple containing the original value and the new value after the operation.
        /// If the condition is not met, the original value is returned in both fields.
        /// </returns>
        /// <remarks>
        /// This method is just a convenience method that is a semantic alias for 
        /// <see cref="ConditionAnd(ref int, Predicate{int}, int)"/> with the
        /// bitwise complement of <paramref name="clearBitsValue"/>
        /// </remarks>
        public static (int originalValue, int newValue) ConditionClearBits (ref int location1,
            Predicate<int> condition, int clearBitsValue) => ConditionAnd(ref location1, condition, ~clearBitsValue);
        //--------------------------------------------------------------------------------
        /// <summary>
        /// A thread-safe method to conditionally clear a specific set of bits in a variable
        /// with the condition based on a function that takes the current value and an argument.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the argument to pass to the condition function.
        /// </typeparam>
        /// <param name="location1">
        /// A variable containing the bits to clear. The result is stored in this variable.
        /// </param>
        /// <param name="condition">
        /// The condition that must be met for the operation to proceed.
        /// </param>
        /// <param name="argument">
        /// The argument to pass to the condition function <paramref name="condition"/>
        /// </param>
        /// <param name="clearBitsValue">
        /// The value that holds the bits to clear in <paramref name="location1"/>
        /// </param>
        /// <returns>
        /// A tuple containing the original value and the new value after the operation.
        /// If the condition is not met, the original value is returned in both fields.
        /// </returns>
        /// <remarks>
        /// This method is just a convenience method that is a semantic alias for 
        /// <see cref="ConditionAnd(ref int, Predicate{int}, int)"/> with the
        /// bitwise complement of <paramref name="clearBitsValue"/>
        /// </remarks>
        public static (int originalValue, int newValue) ConditionClearBits<T> (ref int location1,
            Func<int, T, bool> condition, T argument, int clearBitsValue) =>
            ConditionAnd(ref location1, condition, argument, ~clearBitsValue);
        //--------------------------------------------------------------------------------
        /// <summary>
        /// A thread-safe method to conditionally clear a specific set of bits in a variable.
        /// </summary>
        /// <param name="location1">
        /// A variable containing the bits to clear. The result is stored in this variable.
        /// </param>
        /// <param name="condition">
        /// The condition that must be met for the operation to proceed.
        /// </param>
        /// <param name="clearBitsValue">
        /// The value that holds the bits to clear in <paramref name="location1"/>
        /// </param>
        /// <returns>
        /// A tuple containing the original value and the new value after the operation.
        /// If the condition is not met, the original value is returned in both fields.
        /// </returns>
        /// <remarks>
        /// This method is just a convenience method that is a semantic alias for 
        /// <see cref="ConditionAnd(ref uint, Predicate{uint}, uint)"/> with the
        /// bitwise complement of <paramref name="clearBitsValue"/>
        /// </remarks>
        public static (uint originalValue, uint newValue) ConditionClearBits (ref uint location1,
            Predicate<uint> condition, uint clearBitsValue) => ConditionAnd(ref location1, condition, ~clearBitsValue);
        //--------------------------------------------------------------------------------
        /// <summary>
        /// A thread-safe method to conditionally clear a specific set of bits in a variable
        /// with the condition based on a function that takes the current value and an argument.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the argument to pass to the condition function.
        /// </typeparam>
        /// <param name="location1">
        /// A variable containing the bits to clear. The result is stored in this variable.
        /// </param>
        /// <param name="condition">
        /// The condition that must be met for the operation to proceed.
        /// </param>
        /// <param name="argument">
        /// The argument to pass to the condition function <paramref name="condition"/>
        /// </param>
        /// <param name="clearBitsValue">
        /// The value that holds the bits to clear in <paramref name="location1"/>
        /// </param>
        /// <returns>
        /// A tuple containing the original value and the new value after the operation.
        /// If the condition is not met, the original value is returned in both fields.
        /// </returns>
        /// <remarks>
        /// This method is just a convenience method that is a semantic alias for 
        /// <see cref="ConditionAnd(ref uint, Predicate{uint}, uint)"/> with the
        /// bitwise complement of <paramref name="clearBitsValue"/>
        /// </remarks>
        public static (uint originalValue, uint newValue) ConditionClearBits<T> (ref uint location1,
            Func<uint, T, bool> condition, T argument, uint clearBitsValue) =>
            ConditionAnd(ref location1, condition, argument, ~clearBitsValue);
        //--------------------------------------------------------------------------------
        /// <summary>
        /// A thread-safe method to conditionally clear a specific set of bits in a variable.
        /// </summary>
        /// <param name="location1">
        /// A variable containing the bits to clear. The result is stored in this variable.
        /// </param>
        /// <param name="condition">
        /// The condition that must be met for the operation to proceed.
        /// </param>
        /// <param name="clearBitsValue">
        /// The value that holds the bits to clear in <paramref name="location1"/>
        /// </param>
        /// <returns>
        /// A tuple containing the original value and the new value after the operation.
        /// If the condition is not met, the original value is returned in both fields.
        /// </returns>
        /// <remarks>
        /// This method is just a convenience method that is a semantic alias for 
        /// <see cref="ConditionAnd(ref long, Predicate{long}, long)"/> with the
        /// bitwise complement of <paramref name="clearBitsValue"/>
        /// </remarks>
        public static (long originalValue, long newValue) ConditionClearBits (ref long location1,
            Predicate<long> condition, long clearBitsValue) => ConditionAnd(ref location1, condition, ~clearBitsValue);
        //--------------------------------------------------------------------------------
        /// <summary>
        /// A thread-safe method to conditionally clear a specific set of bits in a variable
        /// with the condition based on a function that takes the current value and an argument.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the argument to pass to the condition function.
        /// </typeparam>
        /// <param name="location1">
        /// A variable containing the bits to clear. The result is stored in this variable.
        /// </param>
        /// <param name="condition">
        /// The condition that must be met for the operation to proceed.
        /// </param>
        /// <param name="argument">
        /// The argument to pass to the condition function <paramref name="condition"/>
        /// </param>
        /// <param name="clearBitsValue">
        /// The value that holds the bits to clear in <paramref name="location1"/>
        /// </param>
        /// <returns>
        /// A tuple containing the original value and the new value after the operation.
        /// If the condition is not met, the original value is returned in both fields.
        /// </returns>
        /// <remarks>
        /// This method is just a convenience method that is a semantic alias for 
        /// <see cref="ConditionAnd(ref long, Predicate{long}, long)"/> with the
        /// bitwise complement of <paramref name="clearBitsValue"/>
        /// </remarks>
        public static (long originalValue, long newValue) ConditionClearBits<T> (ref long location1,
            Func<long, T, bool> condition, T argument, long clearBitsValue) =>
            ConditionAnd(ref location1, condition, argument, ~clearBitsValue);
        //--------------------------------------------------------------------------------
        /// <summary>
        /// A thread-safe method to conditionally clear a specific set of bits in a variable.
        /// </summary>
        /// <param name="location1">
        /// A variable containing the bits to clear. The result is stored in this variable.
        /// </param>
        /// <param name="condition">
        /// The condition that must be met for the operation to proceed.
        /// </param>
        /// <param name="clearBitsValue">
        /// The value that holds the bits to clear in <paramref name="location1"/>
        /// If the condition is not met, the original value is returned in both fields.
        /// </param>
        /// <returns>
        /// A tuple containing the original value and the new value after the operation.
        /// </returns>
        /// <remarks>
        /// This method is just a convenience method that is a semantic alias for 
        /// <see cref="ConditionAnd(ref ulong, Predicate{ulong}, ulong)"/> with the
        /// bitwise complement of <paramref name="clearBitsValue"/>
        /// </remarks>
        public static (ulong originalValue, ulong newValue) ConditionClearBits (ref ulong location1,
            Predicate<ulong> condition, ulong clearBitsValue) => ConditionAnd(ref location1, condition, ~clearBitsValue);
        //--------------------------------------------------------------------------------
        /// <summary>
        /// A thread-safe method to conditionally clear a specific set of bits in a variable
        /// with the condition based on a function that takes the current value and an argument.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the argument to pass to the condition function.
        /// </typeparam>
        /// <param name="location1">
        /// A variable containing the bits to clear. The result is stored in this variable.
        /// </param>
        /// <param name="condition">
        /// The condition that must be met for the operation to proceed.
        /// </param>
        /// <param name="argument">
        /// The argument to pass to the condition function <paramref name="condition"/>
        /// </param>
        /// <param name="clearBitsValue">
        /// The value that holds the bits to clear in <paramref name="location1"/>
        /// If the condition is not met, the original value is returned in both fields.
        /// </param>
        /// <returns>
        /// A tuple containing the original value and the new value after the operation.
        /// </returns>
        /// <remarks>
        /// This method is just a convenience method that is a semantic alias for 
        /// <see cref="ConditionAnd(ref ulong, Predicate{ulong}, ulong)"/> with the
        /// bitwise complement of <paramref name="clearBitsValue"/>
        /// </remarks>
        public static (ulong originalValue, ulong newValue) ConditionClearBits<T> (ref ulong location1,
            Func<ulong, T, bool> condition, T argument, ulong clearBitsValue) =>
            ConditionAnd(ref location1, condition, argument, ~clearBitsValue);
        //--------------------------------------------------------------------------------

        #endregion Conditional ClearBits
    }
    //################################################################################
}
