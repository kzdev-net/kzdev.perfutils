// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using FluentAssertions;

namespace KZDev.PerfUtils.Tests
{
    //################################################################################
    /// <summary>
    /// Unit tests for the <see cref="InterlockedOps"/> SetBits operations.
    /// </summary>
    [Trait(TestConstants.TestTrait.Category, TestConstants.TestCategory.Concurrency)]
    public partial class UsingInterlockedOps : UnitTestBase
    {
        #region SetBits Tests

        //--------------------------------------------------------------------------------
        /// <summary>
        /// Tests the <see cref="InterlockedOps.SetBits(ref int, int)"/> method with no contention
        /// and variety of values.
        /// </summary>
        [Fact]
        public void UsingInterlockedOps_SetBitsInteger_SavesProperResult ()
        {
            int variable = GetTestInteger();

            // Run the test a bunch of times
            for (int loop = 0; loop < 1_000_000; loop++)
            {
                int originalValue = variable;
                int operationValue = GetTestInteger();
                (int operationOriginalValue, int newValue) = InterlockedOps.SetBits(ref variable, operationValue);
                operationOriginalValue.Should().Be(originalValue);
                variable.Should().Be(originalValue | operationValue);
                newValue.Should().Be(variable);
            }
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Tests the <see cref="InterlockedOps.SetBits(ref uint, uint)"/> method with no contention
        /// and variety of values.
        /// </summary>
        [Fact]
        public void UsingInterlockedOps_SetBitsUnsignedInteger_SavesProperResult ()
        {
            uint variable = GetTestUnsignedInteger();

            // Run the test a bunch of times
            for (int loop = 0; loop < 1_000_000; loop++)
            {
                uint originalValue = variable;
                uint operationValue = GetTestUnsignedInteger();
                (uint operationOriginalValue, uint newValue) = InterlockedOps.SetBits(ref variable, operationValue);
                operationOriginalValue.Should().Be(originalValue);
                variable.Should().Be(originalValue | operationValue);
                newValue.Should().Be(variable);
            }
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Tests the <see cref="InterlockedOps.SetBits(ref long, long)"/> method with no contention
        /// and variety of values.
        /// </summary>
        [Fact]
        public void UsingInterlockedOps_SetBitsLongInteger_SavesProperResult ()
        {
            long variable = GetTestLongInteger();

            // Run the test a bunch of times
            for (long loop = 0; loop < 1_000_000; loop++)
            {
                long originalValue = variable;
                long operationValue = GetTestLongInteger();
                (long operationOriginalValue, long newValue) = InterlockedOps.SetBits(ref variable, operationValue);
                operationOriginalValue.Should().Be(originalValue);
                variable.Should().Be(originalValue | operationValue);
                newValue.Should().Be(variable);
            }
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Tests the <see cref="InterlockedOps.SetBits(ref ulong, ulong)"/> method with no contention
        /// and variety of values.
        /// </summary>
        [Fact]
        public void UsingInterlockedOps_SetBitsUnsignedLongInteger_SavesProperResult ()
        {
            ulong variable = GetTestUnsignedLongInteger();

            // Run the test a bunch of times
            for (long loop = 0; loop < 1_000_000; loop++)
            {
                ulong originalValue = variable;
                ulong operationValue = GetTestUnsignedLongInteger();
                (ulong operationOriginalValue, ulong newValue) = InterlockedOps.SetBits(ref variable, operationValue);
                operationOriginalValue.Should().Be(originalValue);
                variable.Should().Be(originalValue | operationValue);
                newValue.Should().Be(variable);
            }
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Tests the <see cref="InterlockedOps.SetBits(ref int, int)"/> method with contention
        /// and variety of values.
        /// </summary>
        [Fact]
        public void UsingInterlockedOps_SetBitsInteger_WithContention_SavesProperResult ()
        {
            const int setBitsValue = 0x4000_0000;
            UsingInterlockedOps_IntegerOperation_WithContention_SavesProperResult(() =>
                    InterlockedOps.SetBits(ref _testContentionInteger, setBitsValue),
                setBitsValue, setBitsValue - 1, incrementValue => (incrementValue & setBitsValue) != 0,
                (incrementValue, compareValue) => incrementValue.Should().Be(compareValue | setBitsValue));
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Tests the <see cref="InterlockedOps.SetBits(ref uint, uint)"/> method with contention
        /// and variety of values.
        /// </summary>
        [Fact]
        public void UsingInterlockedOps_SetBitsUnsignedInteger_WithContention_SavesProperResult ()
        {
            const uint setBitsValue = 0x8000_0000;
            UsingInterlockedOps_UnsignedIntegerOperation_WithContention_SavesProperResult(() =>
                    InterlockedOps.SetBits(ref _testContentionUnsignedInteger, setBitsValue),
                setBitsValue, setBitsValue - 1, incrementValue => (incrementValue & setBitsValue) != 0,
                (incrementValue, compareValue) => incrementValue.Should().Be(compareValue | setBitsValue));
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Tests the <see cref="InterlockedOps.SetBits(ref long, long)"/> method with contention
        /// and variety of values.
        /// </summary>
        [Fact]
        public void UsingInterlockedOps_SetBitsLongInteger_WithContention_SavesProperResult ()
        {
            const long setBitsValue = 0x4000_0000_0000_0000;
            UsingInterlockedOps_LongIntegerOperation_WithContention_SavesProperResult(() =>
                    InterlockedOps.SetBits(ref _testContentionLongInteger, setBitsValue),
                setBitsValue, setBitsValue - 1, incrementValue => (incrementValue & setBitsValue) != 0,
                (incrementValue, compareValue) => incrementValue.Should().Be(compareValue | setBitsValue));
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Tests the <see cref="InterlockedOps.SetBits(ref ulong, ulong)"/> method with contention
        /// and variety of values.
        /// </summary>
        [Fact]
        public void UsingInterlockedOps_SetBitsUnsignedLongInteger_WithContention_SavesProperResult ()
        {
            const ulong setBitsValue = 0x8000_0000_0000_0000;
            UsingInterlockedOps_UnsignedLongIntegerOperation_WithContention_SavesProperResult(() =>
                    InterlockedOps.SetBits(ref _testContentionUnsignedLongInteger, setBitsValue),
                setBitsValue, setBitsValue - 1, incrementValue => (incrementValue & setBitsValue) != 0,
                (incrementValue, compareValue) => incrementValue.Should().Be(compareValue | setBitsValue));
        }
        //--------------------------------------------------------------------------------    

        #endregion SetBits Tests
    }
    //################################################################################
}
