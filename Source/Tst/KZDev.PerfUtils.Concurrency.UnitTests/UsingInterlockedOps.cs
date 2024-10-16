// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Runtime.ExceptionServices;

using FluentAssertions;

using Xunit.Abstractions;

namespace KZDev.PerfUtils.Tests
{
    //################################################################################
    /// <summary>
    /// Unit tests for the <see cref="InterlockedOps"/> class.
    /// </summary>
    [Trait(TestConstants.TestTrait.Category, TestConstants.TestCategory.Concurrency)]
    public partial class UsingInterlockedOps : UnitTestBase
    {
        /// <summary>
        /// The number of times to loop through the contention tests
        /// </summary>
        private const int ContentionTestLoopCount = 100_000;
        /// <summary>
        /// A test integer value that is used to test the <see cref="InterlockedOps"/> methods
        /// with contention. This value is shared between threads, and we are counting on the
        /// xunit test runner to treat all tests in this class as a collection so that there
        /// is only one instance of this value since no two tests will run at the same time.
        /// </summary>
        private static int _testContentionInteger = 0;

        /// <summary>
        /// A test unsigned integer value that is used to test the <see cref="InterlockedOps"/> methods
        /// with contention. This value is shared between threads, and we are counting on the
        /// xunit test runner to treat all tests in this class as a collection so that there
        /// is only one instance of this value since no two tests will run at the same time.
        /// </summary>
        private static uint _testContentionUnsignedInteger = 0;

        /// <summary>
        /// A test long integer value that is used to test the <see cref="InterlockedOps"/> methods
        /// with contention. This value is shared between threads, and we are counting on the
        /// xunit test runner to treat all tests in this class as a collection so that there
        /// is only one instance of this value since no two tests will run at the same time.
        /// </summary>
        private static long _testContentionLongInteger = 0;

        /// <summary>
        /// A test unsigned long integer value that is used to test the <see cref="InterlockedOps"/> methods
        /// with contention. This value is shared between threads, and we are counting on the
        /// xunit test runner to treat all tests in this class as a collection so that there
        /// is only one instance of this value since no two tests will run at the same time.
        /// </summary>
        private static ulong _testContentionUnsignedLongInteger = 0;

        //--------------------------------------------------------------------------------
        /// <summary>
        /// Initializes a new instance of the <see cref="UsingInterlockedOps"/> class.
        /// </summary>
        /// <param name="xUnitTestOutputHelper">
        /// The Xunit test output helper that can be used to output test messages
        /// </param>
        public UsingInterlockedOps (ITestOutputHelper xUnitTestOutputHelper) : base(xUnitTestOutputHelper)
        {
        }
        //--------------------------------------------------------------------------------

        #region Test Methods

        //================================================================================

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
                    InterlockedOps.ClearBits(ref _testContentionInteger, xorValue),
                xorValue, xorValue - 1, incrementValue => (incrementValue & xorValue) != 0,
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
                    InterlockedOps.ClearBits(ref _testContentionUnsignedInteger, xorValue),
                xorValue, xorValue - 1, incrementValue => (incrementValue & xorValue) != 0,
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
                    InterlockedOps.ClearBits(ref _testContentionLongInteger, xorValue),
                xorValue, xorValue - 1, incrementValue => (incrementValue & xorValue) != 0,
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
                xorValue, xorValue - 1, incrementValue => (incrementValue & xorValue) != 0,
                (incrementValue, compareValue) => incrementValue.Should().Be(compareValue | xorValue));
        }
        //--------------------------------------------------------------------------------    

        #endregion Xor Tests

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

        //================================================================================

#endregion Test Methods
    }
    //################################################################################
}
