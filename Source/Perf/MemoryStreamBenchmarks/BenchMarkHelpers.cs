// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Microsoft.IO;

namespace MemoryStreamBenchmarks
{
    //################################################################################
    /// <summary>
    /// Helpers for the benchmarks  
    /// </summary>
    public static class BenchMarkHelpers
    {
        /// <summary>
        /// The maximum buffer size to use for the recyclable memory stream manager
        /// </summary>
        private const int UseRecyclableMaximumBufferSize = 512 * 1024 * 1024;

        /// <summary>
        /// The zero buffer stream manager for the benchmarks that zeros out buffers when released
        /// </summary>
        private static RecyclableMemoryStreamManager? _zeroBufferStreamManager;

        /// <summary>
        /// The zero buffer stream manager for the benchmarks that does not zero out buffers when released
        /// </summary>
        private static RecyclableMemoryStreamManager? _nonZeroBufferStreamManager;

        /// <summary>
        /// The zero buffer stream manager for the benchmarks that zeros out buffers when released using the
        /// exponential buffer size growth
        /// </summary>
        private static RecyclableMemoryStreamManager? _exponentialZeroBufferStreamManager;

        /// <summary>
        /// The zero buffer stream manager for the benchmarks that does not zero out buffers when released
        /// using the exponential buffer size growth
        /// </summary>
        private static RecyclableMemoryStreamManager? _exponentialNonZeroBufferStreamManager;

        /// <summary>
        /// The zero buffer stream manager for the benchmarks that zeros out buffers when released
        /// </summary>
        private static RecyclableMemoryStreamManager ZeroBufferStreamManager
        {
            get
            {
                RecyclableMemoryStreamManager? returnManager = _zeroBufferStreamManager;
                if (returnManager is not null)
                    return returnManager;
                returnManager = new RecyclableMemoryStreamManager(new RecyclableMemoryStreamManager.Options 
                { 
                    ZeroOutBuffer = true,
                    UseExponentialLargeBuffer = false,
                    MaximumBufferSize = UseRecyclableMaximumBufferSize
                });
                // Be sure the other stream managers are released (though they should not have ever been allocated)
                _nonZeroBufferStreamManager = null;
                _exponentialZeroBufferStreamManager = null;
                _exponentialNonZeroBufferStreamManager = null;
                Interlocked.CompareExchange(ref _zeroBufferStreamManager, returnManager, null);
                return _zeroBufferStreamManager;
            }
        }

        /// <summary>
        /// The zero buffer stream manager for the benchmarks that does not zero out buffers when released
        /// </summary>
        private static RecyclableMemoryStreamManager NonZeroBufferStreamManager
        {
            get
            {
                RecyclableMemoryStreamManager? returnManager = _nonZeroBufferStreamManager;
                if (returnManager is not null)
                    return returnManager;
                returnManager = new RecyclableMemoryStreamManager(new RecyclableMemoryStreamManager.Options 
                { 
                    ZeroOutBuffer = false,
                    UseExponentialLargeBuffer = false,
                    MaximumBufferSize = UseRecyclableMaximumBufferSize
                });
                // Be sure the other stream managers are released (though they should not have ever been allocated)
                _zeroBufferStreamManager = null;
                _exponentialZeroBufferStreamManager = null;
                _exponentialNonZeroBufferStreamManager = null;
                Interlocked.CompareExchange(ref _nonZeroBufferStreamManager, returnManager, null);
                return _nonZeroBufferStreamManager;
            }
        }

        /// <summary>
        /// The zero buffer stream manager for the benchmarks that zeros out buffers when released
        /// </summary>
        private static RecyclableMemoryStreamManager ExponentialZeroBufferStreamManager
        {
            get
            {
                RecyclableMemoryStreamManager? returnManager = _exponentialZeroBufferStreamManager;
                if (returnManager is not null)
                    return returnManager;
                returnManager = new RecyclableMemoryStreamManager(new RecyclableMemoryStreamManager.Options
                {
                    ZeroOutBuffer = true,
                    UseExponentialLargeBuffer = true,
                    MaximumBufferSize = UseRecyclableMaximumBufferSize
                });
                // Be sure the other stream managers are released (though they should not have ever been allocated)
                _nonZeroBufferStreamManager = null;
                _zeroBufferStreamManager = null;
                _exponentialNonZeroBufferStreamManager = null;
                Interlocked.CompareExchange(ref _exponentialZeroBufferStreamManager, returnManager, null);
                return _exponentialZeroBufferStreamManager;
            }
        }

        /// <summary>
        /// The zero buffer stream manager for the benchmarks that does not zero out buffers when released
        /// </summary>
        private static RecyclableMemoryStreamManager ExponentialNonZeroBufferStreamManager
        {
            get
            {
                RecyclableMemoryStreamManager? returnManager = _exponentialNonZeroBufferStreamManager;
                if (returnManager is not null)
                    return returnManager;
                returnManager = new RecyclableMemoryStreamManager(new RecyclableMemoryStreamManager.Options
                {
                    ZeroOutBuffer = false,
                    UseExponentialLargeBuffer = true,
                    MaximumBufferSize = UseRecyclableMaximumBufferSize
                });
                // Be sure the other stream managers are released (though they should not have ever been allocated)
                _nonZeroBufferStreamManager = null;
                _zeroBufferStreamManager = null;
                _exponentialZeroBufferStreamManager = null;
                Interlocked.CompareExchange(ref _exponentialNonZeroBufferStreamManager, returnManager, null);
                return _exponentialNonZeroBufferStreamManager;
            }
        }

        //--------------------------------------------------------------------------------
        /// <summary>
        /// The memory stream manager for the benchmarks
        /// </summary>
        public static RecyclableMemoryStreamManager GetMemoryStreamManager (bool zeroBuffers,
            bool useExponentialLargeBufferGrowth) =>
            useExponentialLargeBufferGrowth ?
            (zeroBuffers ? ExponentialZeroBufferStreamManager : ExponentialNonZeroBufferStreamManager) :
            (zeroBuffers ? ZeroBufferStreamManager : NonZeroBufferStreamManager);
        //--------------------------------------------------------------------------------
    }
    //################################################################################
}
