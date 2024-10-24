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
        #region Xor Tests

        //--------------------------------------------------------------------------------
        /// <summary>
        /// Tests the <see cref="InterlockedOps.Xor(ref int, int)"/> method with no contention
        /// and variety of values.
        /// </summary>
        [Fact]
        public void UsingInterlockedOps_XorInteger_SavesProperResult ()
        {
            int variable = GetTestInteger();

            // Run the test a bunch of times
            for (int loop = 0; loop < 1_000_000; loop++)
            {
                int originalValue = variable;
                int operationValue = GetTestInteger();
                int result = InterlockedOps.Xor(ref variable, operationValue);
                result.Should().Be(originalValue);
                variable.Should().Be(originalValue ^ operationValue);
            }
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Tests the <see cref="InterlockedOps.Xor(ref uint, uint)"/> method with no contention
        /// and variety of values.
        /// </summary>
        [Fact]
        public void UsingInterlockedOps_XorUnsignedInteger_SavesProperResult ()
        {
            uint variable = GetTestUnsignedInteger();

            // Run the test a bunch of times
            for (int loop = 0; loop < 1_000_000; loop++)
            {
                uint originalValue = variable;
                uint operationValue = GetTestUnsignedInteger();
                uint result = InterlockedOps.Xor(ref variable, operationValue);
                result.Should().Be(originalValue);
                variable.Should().Be(originalValue ^ operationValue);
            }
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Tests the <see cref="InterlockedOps.Xor(ref long, long)"/> method with no contention
        /// and variety of values.
        /// </summary>
        [Fact]
        public void UsingInterlockedOps_XorLongInteger_SavesProperResult ()
        {
            long variable = GetTestLongInteger();

            // Run the test a bunch of times
            for (long loop = 0; loop < 1_000_000; loop++)
            {
                long originalValue = variable;
                long operationValue = GetTestLongInteger();
                long result = InterlockedOps.Xor(ref variable, operationValue);
                result.Should().Be(originalValue);
                variable.Should().Be(originalValue ^ operationValue);
            }
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Tests the <see cref="InterlockedOps.Xor(ref ulong, ulong)"/> method with no contention
        /// and variety of values.
        /// </summary>
        [Fact]
        public void UsingInterlockedOps_XorUnsignedLongInteger_SavesProperResult ()
        {
            ulong variable = GetTestUnsignedLongInteger();

            // Run the test a bunch of times
            for (long loop = 0; loop < 1_000_000; loop++)
            {
                ulong originalValue = variable;
                ulong operationValue = GetTestUnsignedLongInteger();
                ulong result = InterlockedOps.Xor(ref variable, operationValue);
                result.Should().Be(originalValue);
                variable.Should().Be(originalValue ^ operationValue);
            }
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Tests the <see cref="InterlockedOps.Xor(ref int, int)"/> method with contention
        /// and variety of values.
        /// </summary>
        [Fact]
        public void UsingInterlockedOps_XorInteger_WithContention_SavesProperResult ()
        {
            const int xorValue = 0x4000_0000;
            UsingInterlockedOps_IntegerOperation_WithContention_SavesProperResult(() =>
                    InterlockedOps.Xor(ref _testContentionInteger, xorValue),
                0, xorValue - 1, incrementValue => (incrementValue & xorValue) != 0,
                (incrementValue, compareValue) => incrementValue.Should().Be(compareValue | xorValue));
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Tests the <see cref="InterlockedOps.Xor(ref uint, uint)"/> method with contention
        /// and variety of values.
        /// </summary>
        [Fact]
        public void UsingInterlockedOps_XorUnsignedInteger_WithContention_SavesProperResult ()
        {
            const uint xorValue = 0x8000_0000;
            UsingInterlockedOps_UnsignedIntegerOperation_WithContention_SavesProperResult(() =>
                    InterlockedOps.Xor(ref _testContentionUnsignedInteger, xorValue),
                0, xorValue - 1, incrementValue => (incrementValue & xorValue) != 0,
                (incrementValue, compareValue) => incrementValue.Should().Be(compareValue | xorValue));
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Tests the <see cref="InterlockedOps.Xor(ref long, long)"/> method with contention
        /// and variety of values.
        /// </summary>
        [Fact]
        public void UsingInterlockedOps_XorLongInteger_WithContention_SavesProperResult ()
        {
            const long xorValue = 0x4000_0000_0000_0000;
            UsingInterlockedOps_LongIntegerOperation_WithContention_SavesProperResult(() =>
                    InterlockedOps.Xor(ref _testContentionLongInteger, xorValue),
                0, xorValue - 1, incrementValue => (incrementValue & xorValue) != 0,
                (incrementValue, compareValue) => incrementValue.Should().Be(compareValue | xorValue));
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Tests the <see cref="InterlockedOps.Xor(ref ulong, ulong)"/> method with contention
        /// and variety of values.
        /// </summary>
        [Fact]
        public void UsingInterlockedOps_XorUnsignedLongInteger_WithContention_SavesProperResult ()
        {
            const ulong xorValue = 0x8000_0000_0000_0000;
            UsingInterlockedOps_UnsignedLongIntegerOperation_WithContention_SavesProperResult(() =>
                    InterlockedOps.Xor(ref _testContentionUnsignedLongInteger, xorValue),
                0, xorValue - 1, incrementValue => (incrementValue & xorValue) != 0,
                (incrementValue, compareValue) => incrementValue.Should().Be(compareValue | xorValue));
        }
        //--------------------------------------------------------------------------------    

        #endregion Xor Tests
    }
    //################################################################################
}
