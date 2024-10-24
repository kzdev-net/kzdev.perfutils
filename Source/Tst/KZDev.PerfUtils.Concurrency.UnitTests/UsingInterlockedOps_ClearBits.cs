// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using FluentAssertions;

namespace KZDev.PerfUtils.Tests
{
    //################################################################################
    /// <summary>
    /// Unit tests for the <see cref="InterlockedOps"/> ClearBits operations.
    /// </summary>
    public partial class UsingInterlockedOps
    {
        #region ClearBits Tests

        //--------------------------------------------------------------------------------
        /// <summary>
        /// Tests the <see cref="InterlockedOps.ClearBits(ref int, int)"/> method with no contention
        /// and variety of values.
        /// </summary>
        [Fact]
        public void UsingInterlockedOps_ClearBitsInteger_SavesProperResult ()
        {
            int variable = GetTestInteger();

            // Run the test a bunch of times
            for (int loop = 0; loop < 1_000_000; loop++)
            {
                int originalValue = variable;
                int operationValue = GetTestInteger();
                (int operationOriginalValue, int newValue) = InterlockedOps.ClearBits(ref variable, operationValue);
                operationOriginalValue.Should().Be(originalValue);
                variable.Should().Be(originalValue & ~operationValue);
                newValue.Should().Be(variable);
            }
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Tests the <see cref="InterlockedOps.ClearBits(ref uint, uint)"/> method with no contention
        /// and variety of values.
        /// </summary>
        [Fact]
        public void UsingInterlockedOps_ClearBitsUnsignedInteger_SavesProperResult ()
        {
            uint variable = GetTestUnsignedInteger();

            // Run the test a bunch of times
            for (int loop = 0; loop < 1_000_000; loop++)
            {
                uint originalValue = variable;
                uint operationValue = GetTestUnsignedInteger();
                (uint operationOriginalValue, uint newValue) = InterlockedOps.ClearBits(ref variable, operationValue);
                operationOriginalValue.Should().Be(originalValue);
                variable.Should().Be(originalValue & ~operationValue);
                newValue.Should().Be(variable);
            }
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Tests the <see cref="InterlockedOps.ClearBits(ref long, long)"/> method with no contention
        /// and variety of values.
        /// </summary>
        [Fact]
        public void UsingInterlockedOps_ClearBitsLongInteger_SavesProperResult ()
        {
            long variable = GetTestLongInteger();

            // Run the test a bunch of times
            for (long loop = 0; loop < 1_000_000; loop++)
            {
                long originalValue = variable;
                long operationValue = GetTestLongInteger();
                (long operationOriginalValue, long newValue) = InterlockedOps.ClearBits(ref variable, operationValue);
                operationOriginalValue.Should().Be(originalValue);
                variable.Should().Be(originalValue & ~operationValue);
                newValue.Should().Be(variable);
            }
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Tests the <see cref="InterlockedOps.ClearBits(ref ulong, ulong)"/> method with no contention
        /// and variety of values.
        /// </summary>
        [Fact]
        public void UsingInterlockedOps_ClearBitsUnsignedLongInteger_SavesProperResult ()
        {
            ulong variable = GetTestUnsignedLongInteger();

            // Run the test a bunch of times
            for (long loop = 0; loop < 1_000_000; loop++)
            {
                ulong originalValue = variable;
                ulong operationValue = GetTestUnsignedLongInteger();
                (ulong operationOriginalValue, ulong newValue) = InterlockedOps.ClearBits(ref variable, operationValue);
                operationOriginalValue.Should().Be(originalValue);
                variable.Should().Be(originalValue & ~operationValue);
                newValue.Should().Be(variable);
            }
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Tests the <see cref="InterlockedOps.ClearBits(ref int, int)"/> method with contention
        /// and variety of values.
        /// </summary>
        [Fact]
        public void UsingInterlockedOps_ClearBitsInteger_WithContention_SavesProperResult ()
        {
            const int clearBitsValue = 0x4000_0000;
            UsingInterlockedOps_IntegerOperation_WithContention_SavesProperResult(() =>
                    InterlockedOps.ClearBits(ref _testContentionInteger, clearBitsValue),
                clearBitsValue, clearBitsValue - 1, incrementValue => (incrementValue & clearBitsValue) == 0,
                (incrementValue, compareValue) => incrementValue.Should().Be(compareValue & ~clearBitsValue));
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Tests the <see cref="InterlockedOps.ClearBits(ref uint, uint)"/> method with contention
        /// and variety of values.
        /// </summary>
        [Fact]
        public void UsingInterlockedOps_ClearBitsUnsignedInteger_WithContention_SavesProperResult ()
        {
            const uint clearBitsValue = 0x8000_0000;
            UsingInterlockedOps_UnsignedIntegerOperation_WithContention_SavesProperResult(() =>
                    InterlockedOps.ClearBits(ref _testContentionUnsignedInteger, clearBitsValue),
                clearBitsValue, clearBitsValue - 1, incrementValue => (incrementValue & clearBitsValue) == 0,
                (incrementValue, compareValue) => incrementValue.Should().Be(compareValue & ~clearBitsValue));
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Tests the <see cref="InterlockedOps.ClearBits(ref long, long)"/> method with contention
        /// and variety of values.
        /// </summary>
        [Fact]
        public void UsingInterlockedOps_ClearBitsLongInteger_WithContention_SavesProperResult ()
        {
            const long clearBitsValue = 0x4000_0000_0000_0000;
            UsingInterlockedOps_LongIntegerOperation_WithContention_SavesProperResult(() =>
                    InterlockedOps.ClearBits(ref _testContentionLongInteger, clearBitsValue),
                clearBitsValue, clearBitsValue - 1, incrementValue => (incrementValue & clearBitsValue) == 0,
                (incrementValue, compareValue) => incrementValue.Should().Be(compareValue & ~clearBitsValue));
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Tests the <see cref="InterlockedOps.ClearBits(ref ulong, ulong)"/> method with contention
        /// and variety of values.
        /// </summary>
        [Fact]
        public void UsingInterlockedOps_ClearBitsUnsignedLongInteger_WithContention_SavesProperResult ()
        {
            const ulong clearBitsValue = 0x8000_0000_0000_0000;
            UsingInterlockedOps_UnsignedLongIntegerOperation_WithContention_SavesProperResult(() =>
                    InterlockedOps.ClearBits(ref _testContentionUnsignedLongInteger, clearBitsValue),
                clearBitsValue, clearBitsValue - 1, incrementValue => (incrementValue & clearBitsValue) == 0,
                (incrementValue, compareValue) => incrementValue.Should().Be(compareValue & ~clearBitsValue));
        }
        //--------------------------------------------------------------------------------    

        #endregion ClearBits Tests
    }
    //################################################################################
}
