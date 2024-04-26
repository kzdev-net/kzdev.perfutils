// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

namespace KZDev.PerfUtils
{
    //################################################################################
    /// <summary>
    /// Options for configuring <see cref="MemoryStreamSlim"/> instances.
    /// </summary>
    public class MemoryStreamSlimOptions
    {
        /// <summary>
        /// The default value for the <see cref="ZeroBufferBehavior"/> property.
        /// </summary>
        internal const MemoryStreamSlimZeroBufferOption DefaultZeroBufferBehavior = MemoryStreamSlimZeroBufferOption.OutOfBand;

        //--------------------------------------------------------------------------------
        /// <summary>
        /// Setting for how buffers are zeroed out when memory is released.
        /// </summary>
        /// <remarks>
        /// <para>
        /// By default, this setting is set to <see cref="MemoryStreamSlimZeroBufferOption.OutOfBand"/>. 
        /// To protect sensitive data, unused buffers are cleared when disposing the stream, but
        /// done out-of-band in the thread pool to help performance. In cases where you know that the
        /// stream does not contain any sensitive data that should not reside in memory after
        /// the <see cref="MemoryStreamSlim"/> instance is no longer in use, you can set 
        /// this option to <see cref="MemoryStreamSlimZeroBufferOption.None"/>, or for especially sensitive
        /// data and to force the zeroing to happen immediately inline as part of the release process, set it to 
        /// <see cref="MemoryStreamSlimZeroBufferOption.OnRelease"/>.
        /// </para>
        /// <para>
        /// The zeroing of buffers can cause a performance overhead, especially when the stream
        /// is particularly large.
        /// </para>
        /// </remarks>
        public MemoryStreamSlimZeroBufferOption ZeroBufferBehavior { get; set; } = DefaultZeroBufferBehavior;
        //--------------------------------------------------------------------------------
    }
    //################################################################################
}
