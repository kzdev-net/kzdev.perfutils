﻿// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

namespace KZDev.PerfUtils.Tests
{
    //################################################################################
    /// <summary>
    /// Constants used throughout the test projects.
    /// </summary>
    public static class TestConstants
    {
        //================================================================================
        /// <summary>
        /// Constants for test trait names.
        /// </summary>
        public static class TestTrait
        {
            /// <summary>
            /// The category trait name.
            /// </summary>
            public const string Category = "Category";

            /// <summary>
            /// The time genre trait name.
            /// </summary>
            public const string TimeGenre = "TimeGenre";

            /// <summary>
            /// The model trait name.
            /// </summary>
            public const string Model = "Model";
        }
        //================================================================================
        /// <summary>
        /// Names of specific test time genres.
        /// </summary>
        public static class TimeGenreName
        {
            /// <summary>
            /// Name of the Tier Genre for long-running unit tests
            /// </summary>
            public const string MedRun = "Medium Run";

            /// <summary>
            /// Name of the Tier Genre for long-running unit tests
            /// </summary>
            public const string LongRun = "Long Run";
        }
        //================================================================================
        /// <summary>
        /// Constants for test model names.
        /// </summary>
        public static class TestModel
        {
            /// <summary>
            /// The unit test model name.
            /// </summary>
            public const string Unit = "Unit";

            /// <summary>
            /// The concurrency test model name.
            /// </summary>
            public const string Concurrency = "Concurrency";
        }
        //================================================================================
    }
    //################################################################################
}