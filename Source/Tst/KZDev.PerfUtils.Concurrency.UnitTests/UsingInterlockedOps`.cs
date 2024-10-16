﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;

using FluentAssertions;

namespace KZDev.PerfUtils.Tests
{
    //################################################################################
    /// <summary>
    /// Internal helpers for unit tests for the <see cref="InterlockedOps"/> class.
    /// </summary>
    public partial class UsingInterlockedOps
    {
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Tests the <see cref="InterlockedOps.ClearBits(ref int, int)"/> method with contention
        /// and variety of values.
        /// </summary>
        private void UsingInterlockedOps_IntegerOperation_WithContention_SavesProperResult
            (Action operation, int startValue, int maxIncrement, Predicate<int> operationRunCheckCondition,
            Action<int, int> operationRunVerifier)
        {
            TimeSpan testCycleWaitTime = TimeSpan.FromSeconds(3);
            // A signal to the test threads that they should abort
            ManualResetEventSlim abortSignal = new ManualResetEventSlim(false);
            // A signal to the increment value test thread that it should run
            ManualResetEventSlim runIncrementSignal = new ManualResetEventSlim(false);
            // A signal to the bit operation value test thread that it should run
            ManualResetEventSlim runTestSignal = new ManualResetEventSlim(false);
            // A signal for resetting the tests for the next loop.
            ManualResetEventSlim resetSignal = new ManualResetEventSlim(false);
            // A signal for the increment test to indicate it is done
            ManualResetEventSlim incrementTestDoneSignal = new ManualResetEventSlim(false);
            // Capture any exceptions that occur in the test threads
            ExceptionDispatchInfo? exceptionDispatchInfo = null;

            // The value containing the bit we will use to test for contention
            List<double> hitLoopCounts = new(ContentionTestLoopCount);

            // Set up two threads to run the test
            Thread interlockedOperation = new Thread(() =>
            {
                try
                {
                    while (!abortSignal.IsSet)
                    {
                        // Now, wait for the test signal
                        if (!runTestSignal.Wait(testCycleWaitTime))
                            throw new TimeoutException("Timed out waiting for the test signal");
                        runTestSignal.Reset();
                        operation();
                    }
                }
                catch (Exception error)
                {
                    exceptionDispatchInfo ??= ExceptionDispatchInfo.Capture(error);
                }
            })
            {
                // We want the bit operation thread to run at a lower priority than the increment thread to cause
                // possible context switching during the bit operation, but it should succeed at some point.
                Priority = ThreadPriority.Lowest,
                Name = "Operation Thread",
                IsBackground = true
            };
            interlockedOperation.Start();

            // Set up a thread to increment the value repeatedly looking for a bit change
            // in the high order bit
            Thread incrementThread = new Thread(() =>
            {
                try
                {
                    while (!abortSignal.IsSet)
                    {
                        // Now, wait for the test signal
                        if (!runIncrementSignal.Wait(testCycleWaitTime))
                            throw new TimeoutException("Timed out waiting for the test increment signal");
                        runIncrementSignal.Reset();
                        resetSignal.Reset();

                        Interlocked.Exchange(ref _testContentionInteger, startValue);
                        int lastValue = _testContentionInteger;
                        bool bitOperationFoundAsRun = false;
                        // We will try incrementing the value a number of times
                        for (int rangeLoop = 0; rangeLoop < 5; rangeLoop++)
                        {
                            // Go into a loop incrementing the value and checking if we ever see the
                            // high order bit change
                            for (int incrementLoop = 0; incrementLoop < maxIncrement; incrementLoop++)
                            {
                                int value = Interlocked.Increment(ref _testContentionInteger);
                                if (operationRunCheckCondition(value))
                                {
                                    // We found the bit change, the value without the bit should be the
                                    // last value we saw
                                    operationRunVerifier(value, ++lastValue);
                                    bitOperationFoundAsRun = true;
                                    hitLoopCounts.Add(incrementLoop);
                                    break;
                                }
                                // The value should be the last value plus the increment
                                value.Should().Be(++lastValue);
                                // Signal the bit operation test thread to run - wait for a few loops to signal this
                                if (incrementLoop == 10)
                                {
                                    runTestSignal.Set();
                                }
                                if ((0 == incrementLoop % 1_000_000) && (exceptionDispatchInfo is not null))
                                {
                                    break;
                                }
                            }
                            if (bitOperationFoundAsRun)
                                break;
                            if (exceptionDispatchInfo is not null)
                                break;
                        }
                        if (!bitOperationFoundAsRun)
                            throw new InvalidOperationException("The bit operation was not found");
                        incrementTestDoneSignal.Set();
                        if (!resetSignal.Wait(testCycleWaitTime))
                            throw new TimeoutException("Timed out waiting for the reset signal");
                    }
                }
                catch (Exception error)
                {
                    exceptionDispatchInfo ??= ExceptionDispatchInfo.Capture(error);
                    incrementTestDoneSignal.Set();
                }
            })
            {
                IsBackground = true,
                Name = "Increment Thread"
            };
            incrementThread.Start();

            // Now, run the test a bunch of times
            for (int loop = 0; loop < ContentionTestLoopCount; loop++)
            {
                // Signal the increment thread to run
                runIncrementSignal.Set();
                // Wait for the increment thread to finish
                bool testWaitReturned = incrementTestDoneSignal.Wait(5000);
                if (exceptionDispatchInfo is not null)
                {
                    TestWriteLine($"Exception found on loop #{loop}");
                    break;
                }
                testWaitReturned.Should().BeTrue();

                incrementTestDoneSignal.Reset();
                resetSignal.Set();
            }

            // Shut down the test threads
            abortSignal.Set();
            // Set the signals to run the threads so that they loop around to the abort signal
            runIncrementSignal.Set();
            runTestSignal.Set();
            // Wait for the threads to finish
            incrementThread.Join(5000);
            interlockedOperation.Join(5000);

            exceptionDispatchInfo?.Throw();
            TestWriteLine($"Average hit loop count: {hitLoopCounts.Average()}");
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Tests the <see cref="InterlockedOps.ClearBits(ref uint, uint)"/> method with contention
        /// and variety of values.
        /// </summary>
        private void UsingInterlockedOps_UnsignedIntegerOperation_WithContention_SavesProperResult
            (Action operation, uint startValue, uint maxIncrement, Predicate<uint> operationRunCheckCondition,
            Action<uint, uint> operationRunVerifier)
        {
            TimeSpan testCycleWaitTime = TimeSpan.FromSeconds(3);
            // A signal to the test threads that they should abort
            ManualResetEventSlim abortSignal = new ManualResetEventSlim(false);
            // A signal to the increment value test thread that it should run
            ManualResetEventSlim runIncrementSignal = new ManualResetEventSlim(false);
            // A signal to the bit operation value test thread that it should run
            ManualResetEventSlim runTestSignal = new ManualResetEventSlim(false);
            // A signal for resetting the tests for the next loop.
            ManualResetEventSlim resetSignal = new ManualResetEventSlim(false);
            // A signal for the increment test to indicate it is done
            ManualResetEventSlim incrementTestDoneSignal = new ManualResetEventSlim(false);
            // Capture any exceptions that occur in the test threads
            ExceptionDispatchInfo? exceptionDispatchInfo = null;

            // The value containing the bit we will use to test for contention
            List<double> hitLoopCounts = new(ContentionTestLoopCount);

            // Set up two threads to run the test
            Thread interlockedOperation = new Thread(() =>
            {
                try
                {
                    while (!abortSignal.IsSet)
                    {
                        // Now, wait for the test signal
                        if (!runTestSignal.Wait(testCycleWaitTime))
                            throw new TimeoutException("Timed out waiting for the test signal");
                        runTestSignal.Reset();
                        operation();
                    }
                }
                catch (Exception error)
                {
                    exceptionDispatchInfo ??= ExceptionDispatchInfo.Capture(error);
                }
            })
            {
                // We want the bit operation thread to run at a lower priority than the increment thread to cause
                // possible context switching during the bit operation, but it should succeed at some point.
                Priority = ThreadPriority.Lowest,
                Name = "Operation Thread",
                IsBackground = true
            };
            interlockedOperation.Start();

            // Set up a thread to increment the value repeatedly looking for a bit change
            // in the high order bit
            Thread incrementThread = new Thread(() =>
            {
                try
                {
                    while (!abortSignal.IsSet)
                    {
                        // Now, wait for the test signal
                        if (!runIncrementSignal.Wait(testCycleWaitTime))
                            throw new TimeoutException("Timed out waiting for the test increment signal");
                        runIncrementSignal.Reset();
                        resetSignal.Reset();

                        Interlocked.Exchange(ref _testContentionUnsignedInteger, startValue);
                        uint lastValue = _testContentionUnsignedInteger;
                        bool bitOperationFoundAsRun = false;
                        // We will try incrementing the value a number of times
                        for (int rangeLoop = 0; rangeLoop < 5; rangeLoop++)
                        {
                            // Go into a loop incrementing the value and checking if we ever see the
                            // high order bit change
                            for (int incrementLoop = 0; incrementLoop < maxIncrement; incrementLoop++)
                            {
                                uint value = Interlocked.Increment(ref _testContentionUnsignedInteger);
                                if (operationRunCheckCondition(value))
                                {
                                    // We found the bit change, the value without the bit should be the
                                    // last value we saw
                                    operationRunVerifier(value, ++lastValue);
                                    bitOperationFoundAsRun = true;
                                    hitLoopCounts.Add(incrementLoop);
                                    break;
                                }
                                // The value should be the last value plus the increment
                                value.Should().Be(++lastValue);
                                // Signal the bit operation test thread to run - wait for a few loops to signal this
                                if (incrementLoop == 10)
                                {
                                    runTestSignal.Set();
                                }
                                if ((0 == incrementLoop % 1_000_000) && (exceptionDispatchInfo is not null))
                                {
                                    break;
                                }
                            }
                            if (bitOperationFoundAsRun)
                                break;
                            if (exceptionDispatchInfo is not null)
                                break;
                        }
                        if (!bitOperationFoundAsRun)
                            throw new InvalidOperationException("The bit operation was not found");
                        incrementTestDoneSignal.Set();
                        if (!resetSignal.Wait(testCycleWaitTime))
                            throw new TimeoutException("Timed out waiting for the reset signal");
                    }
                }
                catch (Exception error)
                {
                    exceptionDispatchInfo ??= ExceptionDispatchInfo.Capture(error);
                    incrementTestDoneSignal.Set();
                }
            })
            {
                IsBackground = true,
                Name = "Increment Thread"
            };
            incrementThread.Start();

            // Now, run the test a bunch of times
            for (int loop = 0; loop < ContentionTestLoopCount; loop++)
            {
                // Signal the increment thread to run
                runIncrementSignal.Set();
                // Wait for the increment thread to finish
                bool testWaitReturned = incrementTestDoneSignal.Wait(5000);
                if (exceptionDispatchInfo is not null)
                {
                    TestWriteLine($"Exception found on loop #{loop}");
                    break;
                }
                testWaitReturned.Should().BeTrue();

                incrementTestDoneSignal.Reset();
                resetSignal.Set();
            }

            // Shut down the test threads
            abortSignal.Set();
            // Set the signals to run the threads so that they loop around to the abort signal
            runIncrementSignal.Set();
            runTestSignal.Set();
            // Wait for the threads to finish
            incrementThread.Join(5000);
            interlockedOperation.Join(5000);

            exceptionDispatchInfo?.Throw();
            TestWriteLine($"Average hit loop count: {hitLoopCounts.Average()}");
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Tests the <see cref="InterlockedOps.ClearBits(ref long, long)"/> method with contention
        /// and variety of values.
        /// </summary>
        private void UsingInterlockedOps_LongIntegerOperation_WithContention_SavesProperResult
            (Action operation, long startValue, long maxIncrement, Predicate<long> operationRunCheckCondition,
            Action<long, long> operationRunVerifier)
        {
            TimeSpan testCycleWaitTime = TimeSpan.FromSeconds(3);
            // A signal to the test threads that they should abort
            ManualResetEventSlim abortSignal = new ManualResetEventSlim(false);
            // A signal to the increment value test thread that it should run
            ManualResetEventSlim runIncrementSignal = new ManualResetEventSlim(false);
            // A signal to the bit operation value test thread that it should run
            ManualResetEventSlim runTestSignal = new ManualResetEventSlim(false);
            // A signal for resetting the tests for the next loop.
            ManualResetEventSlim resetSignal = new ManualResetEventSlim(false);
            // A signal for the increment test to indicate it is done
            ManualResetEventSlim incrementTestDoneSignal = new ManualResetEventSlim(false);
            // Capture any exceptions that occur in the test threads
            ExceptionDispatchInfo? exceptionDispatchInfo = null;

            // The value containing the bit we will use to test for contention
            List<double> hitLoopCounts = new(ContentionTestLoopCount);

            // Set up two threads to run the test
            Thread interlockedOperation = new Thread(() =>
            {
                try
                {
                    while (!abortSignal.IsSet)
                    {
                        // Now, wait for the test signal
                        if (!runTestSignal.Wait(testCycleWaitTime))
                            throw new TimeoutException("Timed out waiting for the test signal");
                        runTestSignal.Reset();
                        operation();
                    }
                }
                catch (Exception error)
                {
                    exceptionDispatchInfo ??= ExceptionDispatchInfo.Capture(error);
                }
            })
            {
                // We want the bit operation thread to run at a lower priority than the increment thread to cause
                // possible context switching during the bit operation, but it should succeed at some point.
                Priority = ThreadPriority.Lowest,
                Name = "Operation Thread",
                IsBackground = true
            };
            interlockedOperation.Start();

            // Set up a thread to increment the value repeatedly looking for a bit change
            // in the high order bit
            Thread incrementThread = new Thread(() =>
            {
                try
                {
                    while (!abortSignal.IsSet)
                    {
                        // Now, wait for the test signal
                        if (!runIncrementSignal.Wait(testCycleWaitTime))
                            throw new TimeoutException("Timed out waiting for the test increment signal");
                        runIncrementSignal.Reset();
                        resetSignal.Reset();

                        Interlocked.Exchange(ref _testContentionLongInteger, startValue);
                        long lastValue = _testContentionLongInteger;
                        bool bitOperationFoundAsRun = false;
                        // Go into a loop incrementing the value and checking if we ever see the
                        // high order bit change
                        for (long incrementLoop = 0; incrementLoop < maxIncrement; incrementLoop++)
                        {
                            long value = Interlocked.Increment(ref _testContentionLongInteger);
                            if (operationRunCheckCondition(value))
                            {
                                // We found the bit change, the value without the bit should be the
                                // last value we saw
                                operationRunVerifier(value, ++lastValue);
                                bitOperationFoundAsRun = true;
                                hitLoopCounts.Add(incrementLoop);
                                break;
                            }
                            // The value should be the last value plus the increment
                            value.Should().Be(++lastValue);
                            // Signal the bit operation test thread to run - wait for a few loops to signal this
                            if (incrementLoop == 10)
                            {
                                runTestSignal.Set();
                            }
                            if ((0 == incrementLoop % 1_000_000) && (exceptionDispatchInfo is not null))
                            {
                                break;
                            }
                        }
                        if (exceptionDispatchInfo is not null)
                            break;
                        if (!bitOperationFoundAsRun)
                            throw new InvalidOperationException("The bit operation was not found");
                        incrementTestDoneSignal.Set();
                        if (!resetSignal.Wait(testCycleWaitTime))
                            throw new TimeoutException("Timed out waiting for the reset signal");
                    }
                }
                catch (Exception error)
                {
                    exceptionDispatchInfo ??= ExceptionDispatchInfo.Capture(error);
                    incrementTestDoneSignal.Set();
                }
            })
            {
                IsBackground = true,
                Name = "Increment Thread"
            };
            incrementThread.Start();

            // Now, run the test a bunch of times
            for (long loop = 0; loop < ContentionTestLoopCount; loop++)
            {
                // Signal the increment thread to run
                runIncrementSignal.Set();
                // Wait for the increment thread to finish
                bool testWaitReturned = incrementTestDoneSignal.Wait(5000);
                if (exceptionDispatchInfo is not null)
                {
                    TestWriteLine($"Exception found on loop #{loop}");
                    break;
                }
                testWaitReturned.Should().BeTrue();

                incrementTestDoneSignal.Reset();
                resetSignal.Set();
            }

            // Shut down the test threads
            abortSignal.Set();
            // Set the signals to run the threads so that they loop around to the abort signal
            runIncrementSignal.Set();
            runTestSignal.Set();
            // Wait for the threads to finish
            incrementThread.Join(5000);
            interlockedOperation.Join(5000);

            exceptionDispatchInfo?.Throw();
            TestWriteLine($"Average hit loop count: {hitLoopCounts.Average()}");
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// General helper to run common test operations for the unsigned long integer operations.
        /// </summary>
        /// <param name="operation">
        /// The operation to run in the 'test operation' thread.
        /// </param>
        /// <param name="startValue">
        /// The starting value for the 'increment' thread loop.
        /// </param>
        /// <param name="maxIncrement">
        /// The maximum increment value for the 'increment' thread loop.
        /// </param>
        /// <param name="operationRunCheckCondition">
        /// The condition to check for the operation run.
        /// </param>
        /// <param name="operationRunVerifier">
        /// The verifier to run when the operation is found to have run.
        /// </param>
        private void UsingInterlockedOps_UnsignedLongIntegerOperation_WithContention_SavesProperResult 
            (Action operation, ulong startValue, ulong maxIncrement, Predicate<ulong> operationRunCheckCondition,
            Action<ulong, ulong> operationRunVerifier)
        {
            TimeSpan testCycleWaitTime = TimeSpan.FromSeconds(3);
            // A signal to the test threads that they should abort
            ManualResetEventSlim abortSignal = new ManualResetEventSlim(false);
            // A signal to the increment value test thread that it should run
            ManualResetEventSlim runIncrementSignal = new ManualResetEventSlim(false);
            // A signal to the bit operation value test thread that it should run
            ManualResetEventSlim runTestSignal = new ManualResetEventSlim(false);
            // A signal for resetting the tests for the next loop.
            ManualResetEventSlim resetSignal = new ManualResetEventSlim(false);
            // A signal for the increment test to indicate it is done
            ManualResetEventSlim incrementTestDoneSignal = new ManualResetEventSlim(false);
            // Capture any exceptions that occur in the test threads
            ExceptionDispatchInfo? exceptionDispatchInfo = null;

            // The value containing the bit we will use to test for contention
            List<double> hitLoopCounts = new(ContentionTestLoopCount);

            // Set up two threads to run the test
            Thread interlockedOperation = new Thread(() =>
            {
                try
                {
                    while (!abortSignal.IsSet)
                    {
                        // Now, wait for the test signal
                        if (!runTestSignal.Wait(testCycleWaitTime))
                            throw new TimeoutException("Timed out waiting for the test signal");
                        runTestSignal.Reset();
                        operation();
                    }
                }
                catch (Exception error)
                {
                    exceptionDispatchInfo ??= ExceptionDispatchInfo.Capture(error);
                }
            })
            {
                // We want the bit operation thread to run at a lower priority than the increment thread to cause
                // possible context switching during the bit operation, but it should succeed at some point.
                Priority = ThreadPriority.Lowest,
                Name = "Operation Thread",
                IsBackground = true
            };
            interlockedOperation.Start();

            // Set up a thread to increment the value repeatedly looking for a bit change
            // in the high order bit
            Thread incrementThread = new Thread(() =>
            {
                try
                {
                    while (!abortSignal.IsSet)
                    {
                        // Now, wait for the test signal
                        if (!runIncrementSignal.Wait(testCycleWaitTime))
                            throw new TimeoutException("Timed out waiting for the test increment signal");
                        runIncrementSignal.Reset();
                        resetSignal.Reset();

                        Interlocked.Exchange(ref _testContentionUnsignedLongInteger, startValue);
                        ulong lastValue = _testContentionUnsignedLongInteger;
                        bool bitOperationFoundAsRun = false;
                        // Go into a loop incrementing the value and checking if we ever see the
                        // high order bit change
                        for (ulong incrementLoop = 0; incrementLoop < maxIncrement; incrementLoop++)
                        {
                            ulong value = Interlocked.Increment(ref _testContentionUnsignedLongInteger);
                            if (operationRunCheckCondition(value))
                            {
                                // We found the bit change, the value without the bit should be the
                                // last value we saw
                                operationRunVerifier(value, ++lastValue);
                                bitOperationFoundAsRun = true;
                                hitLoopCounts.Add(incrementLoop);
                                break;
                            }
                            // The value should be the last value plus the increment
                            value.Should().Be(++lastValue);
                            // Signal the bit operation test thread to run - wait for a few loops to signal this
                            if (incrementLoop == 10)
                            {
                                runTestSignal.Set();
                            }
                            if ((0 == incrementLoop % 1_000_000) && (exceptionDispatchInfo is not null))
                            {
                                break;
                            }
                        }
                        if (exceptionDispatchInfo is not null)
                            break;
                        if (!bitOperationFoundAsRun)
                            throw new InvalidOperationException("The bit operation was not found");
                        incrementTestDoneSignal.Set();
                        if (!resetSignal.Wait(testCycleWaitTime))
                            throw new TimeoutException("Timed out waiting for the reset signal");
                    }
                }
                catch (Exception error)
                {
                    exceptionDispatchInfo ??= ExceptionDispatchInfo.Capture(error);
                    incrementTestDoneSignal.Set();
                }
            })
            {
                IsBackground = true,
                Name = "Increment Thread"
            };
            incrementThread.Start();

            // Now, run the test a bunch of times
            for (long loop = 0; loop < ContentionTestLoopCount; loop++)
            {
                // Signal the increment thread to run
                runIncrementSignal.Set();
                // Wait for the increment thread to finish
                bool testWaitReturned = incrementTestDoneSignal.Wait(5000);
                if (exceptionDispatchInfo is not null)
                {
                    TestWriteLine($"Exception found on loop #{loop}");
                    break;
                }
                testWaitReturned.Should().BeTrue();

                incrementTestDoneSignal.Reset();
                resetSignal.Set();
            }

            // Shut down the test threads
            abortSignal.Set();
            // Set the signals to run the threads so that they loop around to the abort signal
            runIncrementSignal.Set();
            runTestSignal.Set();
            // Wait for the threads to finish
            incrementThread.Join(5000);
            interlockedOperation.Join(5000);

            exceptionDispatchInfo?.Throw();
            TestWriteLine($"Average hit loop count: {hitLoopCounts.Average()}");
        }
        //--------------------------------------------------------------------------------    
    }
    //################################################################################
}