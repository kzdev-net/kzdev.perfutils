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
            for (int loop = 0; loop < 1_000_000; loop++)
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
        /// Tests the <see cref="InterlockedOps.ConditionXor(ref uint, Predicate{uint}, uint)"/> method with no contention
        /// and variety of values.
        /// </summary>
        [Fact]
        public void UsingInterlockedOps_ConditionXorUnsignedInteger_SavesProperResult ()
        {
            // Run the test a bunch of times
            for (int loop = 0; loop < 1_000_000; loop++)
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
        /// Tests the <see cref="InterlockedOps.ConditionXor(ref long, Predicate{long}, long)"/> method with no contention
        /// and variety of values.
        /// </summary>
        [Fact]
        public void UsingInterlockedOps_ConditionXorLongInteger_SavesProperResult ()
        {
            // Run the test a bunch of times
            for (int loop = 0; loop < 1_000_000; loop++)
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
        /// Tests the <see cref="InterlockedOps.ConditionXor(ref ulong, Predicate{ulong}, ulong)"/> method with no contention
        /// and variety of values.
        /// </summary>
        [Fact]
        public void UsingInterlockedOps_ConditionXorUnsignedLongInteger_SavesProperResult ()
        {
            // Run the test a bunch of times
            for (int loop = 0; loop < 1_000_000; loop++)
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
        /// Tests the <see cref="InterlockedOps.ConditionXor(ref int, Predicate{int}, int)"/> method with contention
        /// and variety of values.
        /// </summary>
        [Fact]
        public void UsingInterlockedOpsConditionXorInteger_WithContention_SavesProperResult ()
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
        #endregion Conditional Xor Tests
    }
    //################################################################################
}
