// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using FluentAssertions;

namespace KZDev.PerfUtils.Tests
{
    //################################################################################
    /// <summary>
    /// Unit tests for the <see cref="InterlockedOps"/> Xor operations.
    /// </summary>
    public partial class UsingInterlockedOps
    {
        #region Conditional Xor Tests

        //--------------------------------------------------------------------------------
        /// <summary>
        /// Tests the <see cref="InterlockedOps.ConditionXor(ref int, Predicate{int}, int)"/> method with no contention
        /// and variety of values.
        /// </summary>
        [Fact]
        public void UsingInterlockedOps_ConditionXorInteger_SavesProperResult ()
        {
            // Run the test a bunch of times
            for (int loop = 0; loop < ConditionTestLoopCount; loop++)
            {
                int variable = GetTestInteger(0, int.MaxValue / 2);
                int compareValue = variable + 5;
                int operationValue = GetTestInteger();
                (int originalValue, int newValue) = InterlockedOps.ConditionXor(ref variable,
                    current =>
                    {
                        // Cause the variable to change a number of times before allowing the operation
                        if (current != compareValue)
                            Interlocked.Increment(ref variable);
                        return true;
                    }, operationValue);
                originalValue.Should().Be(compareValue);
                newValue.Should().Be(originalValue ^ operationValue);
                variable.Should().Be(newValue);
            }
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Tests the <see cref="InterlockedOps.ConditionXor{T}(ref int, Func{int, T, bool}, T, int)"/> method 
        /// with no contention and variety of values
        /// </summary>
        [Fact]
        public void UsingInterlockedOps_ArgumentConditionXorInteger_SavesProperResult ()
        {
            // Run the test a bunch of times
            for (int loop = 0; loop < ConditionTestLoopCount; loop++)
            {
                int variable = GetTestInteger(0, int.MaxValue / 2);
                int compareValue = variable + 5;
                int operationValue = GetTestInteger();
                (int originalValue, int newValue) = InterlockedOps.ConditionXor(ref variable,
                    (current, compareTo) =>
                    {
                        // Cause the variable to change a number of times before allowing the operation
                        if (current != compareTo)
                            Interlocked.Increment(ref variable);
                        return true;
                    }, compareValue, operationValue);
                originalValue.Should().Be(compareValue);
                newValue.Should().Be(originalValue ^ operationValue);
                variable.Should().Be(newValue);
            }
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Tests the <see cref="InterlockedOps.ConditionXor(ref uint, Predicate{uint}, uint)"/> method with no contention
        /// and variety of values.
        /// </summary>
        [Fact]
        public void UsingInterlockedOps_ConditionXorUnsignedInteger_SavesProperResult ()
        {
            // Run the test a bunch of times
            for (int loop = 0; loop < ConditionTestLoopCount; loop++)
            {
                uint variable = GetTestUnsignedInteger();
                uint compareValue = variable + 5;
                uint operationValue = GetTestUnsignedInteger();
                (uint originalValue, uint newValue) = InterlockedOps.ConditionXor(ref variable,
                    current =>
                    {
                        // Cause the variable to change a number of times before allowing the operation
                        if (current != compareValue)
                            Interlocked.Increment(ref variable);
                        return true;
                    }, operationValue);
                originalValue.Should().Be(compareValue);
                newValue.Should().Be(originalValue ^ operationValue);
                variable.Should().Be(newValue);
            }
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Tests the <see cref="InterlockedOps.ConditionXor{T}(ref uint, Func{uint, T, bool}, T, uint)"/> method 
        /// with no contention and variety of values
        /// </summary>
        [Fact]
        public void UsingInterlockedOps_ArgumentConditionXorUnsignedInteger_SavesProperResult ()
        {
            // Run the test a bunch of times
            for (int loop = 0; loop < ConditionTestLoopCount; loop++)
            {
                uint variable = GetTestUnsignedInteger();
                uint compareValue = variable + 5;
                uint operationValue = GetTestUnsignedInteger();
                (uint originalValue, uint newValue) = InterlockedOps.ConditionXor(ref variable,
                    (current, compareTo) =>
                    {
                        // Cause the variable to change a number of times before allowing the operation
                        if (current != compareTo)
                            Interlocked.Increment(ref variable);
                        return true;
                    }, compareValue, operationValue);
                originalValue.Should().Be(compareValue);
                newValue.Should().Be(originalValue ^ operationValue);
                variable.Should().Be(newValue);
            }
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Tests the <see cref="InterlockedOps.ConditionXor(ref long, Predicate{long}, long)"/> method with no contention
        /// and variety of values.
        /// </summary>
        [Fact]
        public void UsingInterlockedOps_ConditionXorLongInteger_SavesProperResult ()
        {
            // Run the test a bunch of times
            for (int loop = 0; loop < ConditionTestLoopCount; loop++)
            {
                long variable = GetTestLongInteger();
                long compareValue = variable + 5;
                long operationValue = GetTestLongInteger();
                (long originalValue, long newValue) = InterlockedOps.ConditionXor(ref variable,
                    current =>
                    {
                        // Cause the variable to change a number of times before allowing the operation
                        if (current != compareValue)
                            Interlocked.Increment(ref variable);
                        return true;
                    }, operationValue);
                originalValue.Should().Be(compareValue);
                newValue.Should().Be(originalValue ^ operationValue);
                variable.Should().Be(newValue);
            }
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Tests the <see cref="InterlockedOps.ConditionXor{T}(ref long, Func{long, T, bool}, T, long)"/> method 
        /// with no contention and variety of values
        /// </summary>
        [Fact]
        public void UsingInterlockedOps_ArgumentConditionXorLongInteger_SavesProperResult ()
        {
            // Run the test a bunch of times
            for (int loop = 0; loop < ConditionTestLoopCount; loop++)
            {
                long variable = GetTestLongInteger();
                long compareValue = variable + 5;
                long operationValue = GetTestLongInteger();
                (long originalValue, long newValue) = InterlockedOps.ConditionXor(ref variable,
                    (current, compareTo) =>
                    {
                        // Cause the variable to change a number of times before allowing the operation
                        if (current != compareTo)
                            Interlocked.Increment(ref variable);
                        return true;
                    }, compareValue, operationValue);
                originalValue.Should().Be(compareValue);
                newValue.Should().Be(originalValue ^ operationValue);
                variable.Should().Be(newValue);
            }
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Tests the <see cref="InterlockedOps.ConditionXor(ref ulong, Predicate{ulong}, ulong)"/> method with no contention
        /// and variety of values.
        /// </summary>
        [Fact]
        public void UsingInterlockedOps_ConditionXorUnsignedLongInteger_SavesProperResult ()
        {
            // Run the test a bunch of times
            for (int loop = 0; loop < ConditionTestLoopCount; loop++)
            {
                ulong variable = GetTestUnsignedLongInteger();
                ulong compareValue = variable + 5;
                ulong operationValue = GetTestUnsignedLongInteger();
                (ulong originalValue, ulong newValue) = InterlockedOps.ConditionXor(ref variable,
                    current =>
                    {
                        // Cause the variable to change a number of times before allowing the operation
                        if (current != compareValue)
                            Interlocked.Increment(ref variable);
                        return true;
                    }, operationValue);
                originalValue.Should().Be(compareValue);
                newValue.Should().Be(originalValue ^ operationValue);
                variable.Should().Be(newValue);
            }
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Tests the <see cref="InterlockedOps.ConditionXor{T}(ref ulong, Func{ulong, T, bool}, T, ulong)"/> method 
        /// with no contention and variety of values
        /// </summary>
        [Fact]
        public void UsingInterlockedOps_ArgumentConditionXorUnsignedLongInteger_SavesProperResult ()
        {
            // Run the test a bunch of times
            for (int loop = 0; loop < ConditionTestLoopCount; loop++)
            {
                ulong variable = GetTestUnsignedLongInteger();
                ulong compareValue = variable + 5;
                ulong operationValue = GetTestUnsignedLongInteger();
                (ulong originalValue, ulong newValue) = InterlockedOps.ConditionXor(ref variable,
                    (current, compareTo) =>
                    {
                        // Cause the variable to change a number of times before allowing the operation
                        if (current != compareTo)
                            Interlocked.Increment(ref variable);
                        return true;
                    }, compareValue, operationValue);
                originalValue.Should().Be(compareValue);
                newValue.Should().Be(originalValue ^ operationValue);
                variable.Should().Be(newValue);
            }
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Tests the <see cref="InterlockedOps.ConditionXor(ref int, Predicate{int}, int)"/> method 
        /// with no contention and variety of values where the condition returns false, and the
        /// variable should remain unchanged and the returned values should match the variable.
        /// </summary>
        [Fact]
        public void UsingInterlockedOps_ConditionXorInteger_ConditionReturnsFalse_VariableIsUnchanged()
        {
            // Run the test a bunch of times
            for (int loop = 0; loop < ConditionTestLoopCount; loop++)
            {
                int variable = GetTestInteger(0, int.MaxValue / 2);
                int initialValue = variable;
                (int originalValue, int newValue) = InterlockedOps.ConditionXor(ref variable,
                    _ => false, GetTestInteger());
                variable.Should().Be(initialValue);
                originalValue.Should().Be(initialValue);
                newValue.Should().Be(initialValue);
            }
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Tests the <see cref="InterlockedOps.ConditionXor{T}(ref int, Func{int, T, bool}, T, int)"/> method
        /// with no contention and variety of values where the condition returns false, and the
        /// variable should remain unchanged and the returned values should match the variable.
        /// </summary>
        [Fact]
        public void UsingInterlockedOps_ArgumentConditionXorInteger_ConditionReturnsFalse_VariableIsUnchanged ()
        {
            // Run the test a bunch of times
            for (int loop = 0; loop < ConditionTestLoopCount; loop++)
            {
                int variable = GetTestInteger(0, int.MaxValue / 2);
                int initialValue = variable;
                (int originalValue, int newValue) = InterlockedOps.ConditionXor(ref variable,
                    (_, returnValue) => returnValue, false, GetTestInteger());
                variable.Should().Be(initialValue);
                originalValue.Should().Be(initialValue);
                newValue.Should().Be(initialValue);
            }
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Tests the <see cref="InterlockedOps.ConditionXor(ref uint, Predicate{uint}, uint)"/> method 
        /// with no contention and variety of values where the condition returns false, and the
        /// variable should remain unchanged and the returned values should match the variable.
        /// </summary>
        [Fact]
        public void UsingInterlockedOps_ConditionXorUnsignedInteger_ConditionReturnsFalse_VariableIsUnchanged ()
        {
            // Run the test a bunch of times
            for (int loop = 0; loop < ConditionTestLoopCount; loop++)
            {
                uint variable = GetTestUnsignedInteger(0, uint.MaxValue / 2);
                uint initialValue = variable;
                (uint originalValue, uint newValue) = InterlockedOps.ConditionXor(ref variable,
                    _ => false, GetTestUnsignedInteger());
                variable.Should().Be(initialValue);
                originalValue.Should().Be(initialValue);
                newValue.Should().Be(initialValue);
            }
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Tests the <see cref="InterlockedOps.ConditionXor{T}(ref uint, Func{uint, T, bool}, T, uint)"/> method
        /// with no contention and variety of values where the condition returns false, and the
        /// variable should remain unchanged and the returned values should match the variable.
        /// </summary>
        [Fact]
        public void UsingInterlockedOps_ArgumentConditionXorUnsignedInteger_ConditionReturnsFalse_VariableIsUnchanged ()
        {
            // Run the test a bunch of times
            for (int loop = 0; loop < ConditionTestLoopCount; loop++)
            {
                uint variable = GetTestUnsignedInteger(0, uint.MaxValue / 2);
                uint initialValue = variable;
                (uint originalValue, uint newValue) = InterlockedOps.ConditionXor(ref variable,
                    (_, returnValue) => returnValue, false, GetTestUnsignedInteger());
                variable.Should().Be(initialValue);
                originalValue.Should().Be(initialValue);
                newValue.Should().Be(initialValue);
            }
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Tests the <see cref="InterlockedOps.ConditionXor(ref long, Predicate{long}, long)"/> method 
        /// with no contention and variety of values where the condition returns false, and the
        /// variable should remain unchanged and the returned values should match the variable.
        /// </summary>
        [Fact]
        public void UsingInterlockedOps_ConditionXorLongInteger_ConditionReturnsFalse_VariableIsUnchanged ()
        {
            // Run the test a bunch of times
            for (int loop = 0; loop < ConditionTestLoopCount; loop++)
            {
                long variable = GetTestLongInteger(0, long.MaxValue / 2);
                long initialValue = variable;
                (long originalValue, long newValue) = InterlockedOps.ConditionXor(ref variable,
                    _ => false, GetTestLongInteger());
                variable.Should().Be(initialValue);
                originalValue.Should().Be(initialValue);
                newValue.Should().Be(initialValue);
            }
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Tests the <see cref="InterlockedOps.ConditionXor{T}(ref long, Func{long, T, bool}, T, long)"/> method
        /// with no contention and variety of values where the condition returns false, and the
        /// variable should remain unchanged and the returned values should match the variable.
        /// </summary>
        [Fact]
        public void UsingInterlockedOps_ArgumentConditionXorLongInteger_ConditionReturnsFalse_VariableIsUnchanged ()
        {
            // Run the test a bunch of times
            for (int loop = 0; loop < ConditionTestLoopCount; loop++)
            {
                long variable = GetTestLongInteger(0, long.MaxValue / 2);
                long initialValue = variable;
                (long originalValue, long newValue) = InterlockedOps.ConditionXor(ref variable,
                    (_, returnValue) => returnValue, false, GetTestLongInteger());
                variable.Should().Be(initialValue);
                originalValue.Should().Be(initialValue);
                newValue.Should().Be(initialValue);
            }
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Tests the <see cref="InterlockedOps.ConditionXor(ref ulong, Predicate{ulong}, ulong)"/> method 
        /// with no contention and variety of values where the condition returns false, and the
        /// variable should remain unchanged and the returned values should match the variable.
        /// </summary>
        [Fact]
        public void UsingInterlockedOps_ConditionXorUnsignedLongInteger_ConditionReturnsFalse_VariableIsUnchanged ()
        {
            // Run the test a bunch of times
            for (int loop = 0; loop < ConditionTestLoopCount; loop++)
            {
                ulong variable = GetTestUnsignedLongInteger(0, ulong.MaxValue / 2);
                ulong initialValue = variable;
                (ulong originalValue, ulong newValue) = InterlockedOps.ConditionXor(ref variable,
                    _ => false, GetTestUnsignedLongInteger());
                variable.Should().Be(initialValue);
                originalValue.Should().Be(initialValue);
                newValue.Should().Be(initialValue);
            }
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Tests the <see cref="InterlockedOps.ConditionXor{T}(ref ulong, Func{ulong, T, bool}, T, ulong)"/> method
        /// with no contention and variety of values where the condition returns false, and the
        /// variable should remain unchanged and the returned values should match the variable.
        /// </summary>
        [Fact]
        public void UsingInterlockedOps_ArgumentConditionXorUnsignedLongInteger_ConditionReturnsFalse_VariableIsUnchanged ()
        {
            // Run the test a bunch of times
            for (int loop = 0; loop < ConditionTestLoopCount; loop++)
            {
                ulong variable = GetTestUnsignedLongInteger(0, ulong.MaxValue / 2);
                ulong initialValue = variable;
                (ulong originalValue, ulong newValue) = InterlockedOps.ConditionXor(ref variable,
                    (_, returnValue) => returnValue, false, GetTestUnsignedLongInteger());
                variable.Should().Be(initialValue);
                originalValue.Should().Be(initialValue);
                newValue.Should().Be(initialValue);
            }
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Tests the <see cref="InterlockedOps.ConditionXor(ref int, Predicate{int}, int)"/> method with contention
        /// and variety of values.
        /// </summary>
        [Fact]
        public void UsingInterlockedOps_ConditionXorInteger_WithContention_SavesProperResult ()
        {
            const int xorValue = 0x4000_0000;
            int conditionRanCount = 0;
            UsingInterlockedOps_IntegerOperation_WithContention_SavesProperResult(() =>
                    InterlockedOps.ConditionXor(ref _testContentionInteger,
                        _ =>
                        {
                            // Track how many times the condition is checked.
                            Interlocked.Increment(ref conditionRanCount);
                            return true;
                        }, xorValue),
                0, xorValue - 1, incrementValue => (incrementValue & xorValue) != 0,
                (incrementValue, compareValue) =>
                {
                    conditionRanCount.Should().BeGreaterThan(0);
                    incrementValue.Should().Be(compareValue | xorValue);
                });
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Tests the <see cref="InterlockedOps.ConditionXor{T}(ref int, Func{int, T, bool}, T, int)"/> method with contention
        /// and variety of values.
        /// </summary>
        [Fact]
        public void UsingInterlockedOps_ArgumentConditionXorInteger_WithContention_SavesProperResult ()
        {
            const int xorValue = 0x4000_0000;
            int conditionRanCount = 0;
            UsingInterlockedOps_IntegerOperation_WithContention_SavesProperResult(() =>
                    InterlockedOps.ConditionXor(ref _testContentionInteger,
                        (_, returnValue) =>
                        {
                            // Track how many times the condition is checked.
                            Interlocked.Increment(ref conditionRanCount);
                            return returnValue;
                        }, true, xorValue),
                0, xorValue - 1, incrementValue => (incrementValue & xorValue) != 0,
                (incrementValue, compareValue) =>
                {
                    conditionRanCount.Should().BeGreaterThan(0);
                    incrementValue.Should().Be(compareValue | xorValue);
                });
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Tests the <see cref="InterlockedOps.ConditionXor(ref uint, Predicate{uint}, uint)"/> method with contention
        /// and variety of values.
        /// </summary>
        [Fact]
        public void UsingInterlockedOps_ConditionXorUnsignedInteger_WithContention_SavesProperResult ()
        {
            const uint xorValue = 0x8000_0000;
            int conditionRanCount = 0;
            UsingInterlockedOps_UnsignedIntegerOperation_WithContention_SavesProperResult(() =>
                    InterlockedOps.ConditionXor(ref _testContentionUnsignedInteger,
                        _ =>
                        {
                            // Track how many times the condition is checked.
                            Interlocked.Increment(ref conditionRanCount);
                            return true;
                        }, xorValue),
                0, xorValue - 1, incrementValue => (incrementValue & xorValue) != 0,
                (incrementValue, compareValue) =>
                {
                    conditionRanCount.Should().BeGreaterThan(0);
                    incrementValue.Should().Be(compareValue | xorValue);
                });
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Tests the <see cref="InterlockedOps.ConditionXor{T}(ref uint, Func{uint, T, bool}, T, uint)"/> method with contention
        /// and variety of values.
        /// </summary>
        [Fact]
        public void UsingInterlockedOps_ArgumentConditionXorUnsignedInteger_WithContention_SavesProperResult ()
        {
            const uint xorValue = 0x8000_0000;
            int conditionRanCount = 0;
            UsingInterlockedOps_UnsignedIntegerOperation_WithContention_SavesProperResult(() =>
                    InterlockedOps.ConditionXor(ref _testContentionUnsignedInteger,
                        (_, returnValue) =>
                        {
                            // Track how many times the condition is checked.
                            Interlocked.Increment(ref conditionRanCount);
                            return returnValue;
                        }, true, xorValue),
                0, xorValue - 1, incrementValue => (incrementValue & xorValue) != 0,
                (incrementValue, compareValue) =>
                {
                    conditionRanCount.Should().BeGreaterThan(0);
                    incrementValue.Should().Be(compareValue | xorValue);
                });
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Tests the <see cref="InterlockedOps.ConditionXor(ref long, Predicate{long}, long)"/> method with contention
        /// and variety of values.
        /// </summary>
        [Fact]
        public void UsingInterlockedOps_ConditionXorLongInteger_WithContention_SavesProperResult ()
        {
            const long xorValue = 0x4000_0000_0000_0000;
            int conditionRanCount = 0;
            UsingInterlockedOps_LongIntegerOperation_WithContention_SavesProperResult(() =>
                    InterlockedOps.ConditionXor(ref _testContentionLongInteger,
                        _ =>
                        {
                            // Track how many times the condition is checked.
                            Interlocked.Increment(ref conditionRanCount);
                            return true;
                        }, xorValue),
                0, xorValue - 1, incrementValue => (incrementValue & xorValue) != 0,
                (incrementValue, compareValue) =>
                {
                    conditionRanCount.Should().BeGreaterThan(0);
                    incrementValue.Should().Be(compareValue | xorValue);
                });
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Tests the <see cref="InterlockedOps.ConditionXor{T}(ref long, Func{long, T, bool}, T, long)"/> method with contention
        /// and variety of values.
        /// </summary>
        [Fact]
        public void UsingInterlockedOps_ArgumentConditionXorLongInteger_WithContention_SavesProperResult ()
        {
            const long xorValue = 0x4000_0000_0000_0000;
            int conditionRanCount = 0;
            UsingInterlockedOps_LongIntegerOperation_WithContention_SavesProperResult(() =>
                    InterlockedOps.ConditionXor(ref _testContentionLongInteger,
                        (_, returnValue) =>
                        {
                            // Track how many times the condition is checked.
                            Interlocked.Increment(ref conditionRanCount);
                            return returnValue;
                        }, true, xorValue),
                0, xorValue - 1, incrementValue => (incrementValue & xorValue) != 0,
                (incrementValue, compareValue) =>
                {
                    conditionRanCount.Should().BeGreaterThan(0);
                    incrementValue.Should().Be(compareValue | xorValue);
                });
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Tests the <see cref="InterlockedOps.ConditionXor(ref ulong, Predicate{ulong}, ulong)"/> method with contention
        /// and variety of values.
        /// </summary>
        [Fact]
        public void UsingInterlockedOps_ConditionXorUnsignedLongInteger_WithContention_SavesProperResult ()
        {
            const ulong xorValue = 0x8000_0000_0000_0000;
            int conditionRanCount = 0;
            UsingInterlockedOps_UnsignedLongIntegerOperation_WithContention_SavesProperResult(() =>
                    InterlockedOps.ConditionXor(ref _testContentionUnsignedLongInteger,
                        _ =>
                        {
                            // Track how many times the condition is checked.
                            Interlocked.Increment(ref conditionRanCount);
                            return true;
                        }, xorValue),
                0, xorValue - 1, incrementValue => (incrementValue & xorValue) != 0,
                (incrementValue, compareValue) =>
                {
                    conditionRanCount.Should().BeGreaterThan(0);
                    incrementValue.Should().Be(compareValue | xorValue);
                });
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Tests the <see cref="InterlockedOps.ConditionXor{T}(ref ulong, Func{ulong, T, bool}, T, ulong)"/> method with contention
        /// and variety of values.
        /// </summary>
        [Fact]
        public void UsingInterlockedOps_ArgumentConditionXorUnsignedLongInteger_WithContention_SavesProperResult ()
        {
            const ulong xorValue = 0x8000_0000_0000_0000;
            int conditionRanCount = 0;
            UsingInterlockedOps_UnsignedLongIntegerOperation_WithContention_SavesProperResult(() =>
                    InterlockedOps.ConditionXor(ref _testContentionUnsignedLongInteger,
                        (_, returnValue) =>
                        {
                            // Track how many times the condition is checked.
                            Interlocked.Increment(ref conditionRanCount);
                            return returnValue;
                        }, true, xorValue),
                0, xorValue - 1, incrementValue => (incrementValue & xorValue) != 0,
                (incrementValue, compareValue) =>
                {
                    conditionRanCount.Should().BeGreaterThan(0);
                    incrementValue.Should().Be(compareValue | xorValue);
                });
        }
        //--------------------------------------------------------------------------------    
        #endregion Conditional Xor Tests
    }
    //################################################################################
}
