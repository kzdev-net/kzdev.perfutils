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
        /// 
        /// </summary>
        private const int UseLargeBlockSize = 1024 * 1024;

        /// <summary>
        /// The large buffer multiple value to use for large streams.
        /// </summary>
        private const int UseLargeStreamLargeBufferMultiple = 16 * 1024 * 1024;

        /// <summary>
        /// The maximum buffer size to use for the recyclable memory stream manager for large streams
        /// with exponential growth buffers.
        /// </summary>
        private static readonly int UseLargeExponentialRecyclableMaximumBufferSize = ComputeLargeExponentialMaximumBufferSize();

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
        /// for large streams
        /// </summary>
        private static RecyclableMemoryStreamManager? _largeZeroBufferStreamManager;

        /// <summary>
        /// The zero buffer stream manager for the benchmarks that does not zero out buffers when released
        /// for large streams
        /// </summary>
        private static RecyclableMemoryStreamManager? _largeNonZeroBufferStreamManager;

        /// <summary>
        /// The zero buffer stream manager for the benchmarks that zeros out buffers when released using the
        /// exponential buffer size growth for large streams
        /// </summary>
        private static RecyclableMemoryStreamManager? _largeExponentialZeroBufferStreamManager;

        /// <summary>
        /// The zero buffer stream manager for the benchmarks that does not zero out buffers when released
        /// using the exponential buffer size growth for large streams
        /// </summary>
        private static RecyclableMemoryStreamManager? _largeExponentialNonZeroBufferStreamManager;

        /// <summary>
        /// Helper method to compute the maximum buffer size for large streams with exponential growth
        /// </summary>
        /// <returns>
        /// The maximum buffer size for large streams with exponential growth
        /// </returns>
        private static int ComputeLargeExponentialMaximumBufferSize ()
        {
            int maxBufferSize = 0;
            long checkMaxBufferSize;
            long pow = 1;
            while ((checkMaxBufferSize = UseLargeStreamLargeBufferMultiple * pow) < int.MaxValue)
            {
                pow <<= 1;
                maxBufferSize = (int)checkMaxBufferSize;
            }
            return maxBufferSize;
        }
        //================================================================================
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
                _largeZeroBufferStreamManager = null;
                _largeNonZeroBufferStreamManager = null;
                _largeExponentialZeroBufferStreamManager = null;
                _largeExponentialNonZeroBufferStreamManager = null;
                Interlocked.CompareExchange(ref _zeroBufferStreamManager, returnManager, null);
                return _zeroBufferStreamManager;
            }
        }
        //================================================================================
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
                _largeZeroBufferStreamManager = null;
                _largeNonZeroBufferStreamManager = null;
                _largeExponentialZeroBufferStreamManager = null;
                _largeExponentialNonZeroBufferStreamManager = null;
                Interlocked.CompareExchange(ref _nonZeroBufferStreamManager, returnManager, null);
                return _nonZeroBufferStreamManager;
            }
        }
        //================================================================================
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
                _largeZeroBufferStreamManager = null;
                _largeNonZeroBufferStreamManager = null;
                _largeExponentialZeroBufferStreamManager = null;
                _largeExponentialNonZeroBufferStreamManager = null;
                Interlocked.CompareExchange(ref _exponentialZeroBufferStreamManager, returnManager, null);
                return _exponentialZeroBufferStreamManager;
            }
        }
        //================================================================================
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
                _largeZeroBufferStreamManager = null;
                _largeNonZeroBufferStreamManager = null;
                _largeExponentialZeroBufferStreamManager = null;
                _largeExponentialNonZeroBufferStreamManager = null;
                Interlocked.CompareExchange(ref _exponentialNonZeroBufferStreamManager, returnManager, null);
                return _exponentialNonZeroBufferStreamManager;
            }
        }
        //================================================================================
        /// <summary>
        /// The zero buffer stream manager for the benchmarks that zeros out buffers when released
        /// </summary>
        private static RecyclableMemoryStreamManager LargeZeroBufferStreamManager
        {
            get
            {
                RecyclableMemoryStreamManager? returnManager = _largeZeroBufferStreamManager;
                if (returnManager is not null)
                    return returnManager;
                returnManager = new RecyclableMemoryStreamManager(new RecyclableMemoryStreamManager.Options
                {
                    ZeroOutBuffer = true,
                    UseExponentialLargeBuffer = false,
                    BlockSize = UseLargeBlockSize,
                    LargeBufferMultiple = UseLargeStreamLargeBufferMultiple,
                    MaximumBufferSize = (int.MaxValue / UseLargeStreamLargeBufferMultiple) * UseLargeStreamLargeBufferMultiple
                });
                // Be sure the other stream managers are released (though they should not have ever been allocated)
                _nonZeroBufferStreamManager = null;
                _zeroBufferStreamManager = null;
                _exponentialZeroBufferStreamManager = null;
                _exponentialNonZeroBufferStreamManager = null;
                _largeNonZeroBufferStreamManager = null;
                _largeExponentialZeroBufferStreamManager = null;
                _largeExponentialNonZeroBufferStreamManager = null;
                Interlocked.CompareExchange(ref _zeroBufferStreamManager, returnManager, null);
                return _zeroBufferStreamManager;
            }
        }
        //================================================================================
        /// <summary>
        /// The zero buffer stream manager for the benchmarks that does not zero out buffers when released
        /// </summary>
        private static RecyclableMemoryStreamManager LargeNonZeroBufferStreamManager
        {
            get
            {
                RecyclableMemoryStreamManager? returnManager = _largeNonZeroBufferStreamManager;
                if (returnManager is not null)
                    return returnManager;
                returnManager = new RecyclableMemoryStreamManager(new RecyclableMemoryStreamManager.Options
                {
                    ZeroOutBuffer = false,
                    UseExponentialLargeBuffer = false,
                    BlockSize = UseLargeBlockSize,
                    LargeBufferMultiple = UseLargeStreamLargeBufferMultiple,
                    MaximumBufferSize = (int.MaxValue / UseLargeStreamLargeBufferMultiple) * UseLargeStreamLargeBufferMultiple
                });
                // Be sure the other stream managers are released (though they should not have ever been allocated)
                _nonZeroBufferStreamManager = null;
                _zeroBufferStreamManager = null;
                _exponentialZeroBufferStreamManager = null;
                _exponentialNonZeroBufferStreamManager = null;
                _largeZeroBufferStreamManager = null;
                _largeExponentialZeroBufferStreamManager = null;
                _largeExponentialNonZeroBufferStreamManager = null;
                Interlocked.CompareExchange(ref _largeNonZeroBufferStreamManager, returnManager, null);
                return _largeNonZeroBufferStreamManager;
            }
        }
        //================================================================================
        /// <summary>
        /// The zero buffer stream manager for the benchmarks that zeros out buffers when released
        /// </summary>
        private static RecyclableMemoryStreamManager LargeExponentialZeroBufferStreamManager
        {
            get
            {
                RecyclableMemoryStreamManager? returnManager = _largeExponentialZeroBufferStreamManager;
                if (returnManager is not null)
                    return returnManager;
                returnManager = new RecyclableMemoryStreamManager(new RecyclableMemoryStreamManager.Options
                {
                    ZeroOutBuffer = true,
                    UseExponentialLargeBuffer = true,
                    BlockSize = UseLargeBlockSize,
                    LargeBufferMultiple = UseLargeStreamLargeBufferMultiple,
                    MaximumBufferSize = UseLargeExponentialRecyclableMaximumBufferSize
                });
                // Be sure the other stream managers are released (though they should not have ever been allocated)
                _nonZeroBufferStreamManager = null;
                _zeroBufferStreamManager = null;
                _exponentialZeroBufferStreamManager = null;
                _exponentialNonZeroBufferStreamManager = null;
                _largeZeroBufferStreamManager = null;
                _largeNonZeroBufferStreamManager = null;
                _largeExponentialNonZeroBufferStreamManager = null;
                Interlocked.CompareExchange(ref _exponentialZeroBufferStreamManager, returnManager, null);
                return _exponentialZeroBufferStreamManager;
            }
        }
        //================================================================================
        /// <summary>
        /// The zero buffer stream manager for the benchmarks that does not zero out buffers when released
        /// </summary>
        private static RecyclableMemoryStreamManager LargeExponentialNonZeroBufferStreamManager
        {
            get
            {
                RecyclableMemoryStreamManager? returnManager = _largeExponentialNonZeroBufferStreamManager;
                if (returnManager is not null)
                    return returnManager;
                returnManager = new RecyclableMemoryStreamManager(new RecyclableMemoryStreamManager.Options
                {
                    ZeroOutBuffer = false,
                    UseExponentialLargeBuffer = true,
                    BlockSize = UseLargeBlockSize,
                    LargeBufferMultiple = UseLargeStreamLargeBufferMultiple,
                    MaximumBufferSize = UseLargeExponentialRecyclableMaximumBufferSize
                });
                // Be sure the other stream managers are released (though they should not have ever been allocated)
                _nonZeroBufferStreamManager = null;
                _zeroBufferStreamManager = null;
                _exponentialZeroBufferStreamManager = null;
                _exponentialNonZeroBufferStreamManager = null;
                _largeZeroBufferStreamManager = null;
                _largeNonZeroBufferStreamManager = null;
                _largeExponentialZeroBufferStreamManager = null;
                Interlocked.CompareExchange(ref _largeExponentialNonZeroBufferStreamManager, returnManager, null);
                return _largeExponentialNonZeroBufferStreamManager;
            }
        }
        //================================================================================

        //--------------------------------------------------------------------------------
        /// <summary>
        /// Gets a specific memory stream manager for the benchmarks
        /// </summary>
        public static RecyclableMemoryStreamManager GetMemoryStreamManager (bool zeroBuffers,
            bool useExponentialLargeBufferGrowth) =>
            useExponentialLargeBufferGrowth ?
            (zeroBuffers ? ExponentialZeroBufferStreamManager : ExponentialNonZeroBufferStreamManager) :
            (zeroBuffers ? ZeroBufferStreamManager : NonZeroBufferStreamManager);
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Gets a specific large memory stream manager for the benchmarks
        /// </summary>
        public static RecyclableMemoryStreamManager GetLargeMemoryStreamManager (bool zeroBuffers,
            bool useExponentialLargeBufferGrowth) =>
            useExponentialLargeBufferGrowth ?
                (zeroBuffers ? LargeExponentialZeroBufferStreamManager : LargeExponentialNonZeroBufferStreamManager) :
                (zeroBuffers ? LargeZeroBufferStreamManager : LargeNonZeroBufferStreamManager);
        //--------------------------------------------------------------------------------
    }
    //################################################################################
}
