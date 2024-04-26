// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Diagnostics.Tracing;

using KZDev.PerfUtils.Internals;
// ReSharper disable InconsistentNaming

namespace KZDev.PerfUtils.Observability
{
    //################################################################################
    /// <summary>
    /// The event source log for the performance utilities library.
    /// </summary>
    [EventSource(Name = "KZDev.PerfUtils")]
    class UtilsEventSource : EventSource
    {
        /// <summary>
        /// The name to use for the GC Heap memory type.
        /// </summary>
        private const string GcHeapMemoryTypeName = "GC Heap";

        /// <summary>
        /// The name to use for the native heap memory type.
        /// </summary>
        private const string NativeHeapMemoryTypeName = "Native";

        //--------------------------------------------------------------------------------
        /// <summary>
        /// Optimized WriteEvent signature for event ID, long, and <see cref="MemoryStreamSlimSettings"/>
        /// </summary>
        /// <param name="eventId">
        /// The event ID to write
        /// </param>
        /// <param name="arg1">
        /// The first string argument
        /// </param>
        /// <param name="arg2">
        /// The long argument
        /// </param>
        /// <param name="arg3">
        /// The second string argument
        /// </param>
        /// <param name="arg4">
        /// The third string argument
        /// </param>
        [NonEvent]
        private unsafe void WriteEvent (int eventId, string arg1, long arg2, string arg3, string arg4)
        {
            if (!IsEnabled())
                return;

            fixed (char* arg1Bytes = arg1)
                fixed (char* arg3Bytes = arg3)
                    fixed (char* arg4Bytes = arg4)
                    {
                        EventData* descriptors = stackalloc EventData[4];
                        descriptors[0].DataPointer = (IntPtr)arg1Bytes;
                        descriptors[0].Size = (arg1.Length + 1) * sizeof(char);
                        descriptors[1].DataPointer = (IntPtr)(&arg2);
                        descriptors[1].Size = sizeof(long);
                        descriptors[2].DataPointer = (IntPtr)arg3Bytes;
                        descriptors[2].Size = (arg3.Length + 1) * sizeof(char);
                        descriptors[3].DataPointer = (IntPtr)arg4Bytes;
                        descriptors[3].Size = (arg4.Length + 1) * sizeof(char);
                        WriteEventCore(eventId, 3, descriptors);
                    }
        }
        //--------------------------------------------------------------------------------

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>
        /// The Event Source specific Keywords for events
        /// </summary>
        public class Keywords
        {
            /// <summary>
            /// A category of events for the creation of an object, etc.
            /// </summary>
            public const EventKeywords Create = (EventKeywords)0x0001;

            /// <summary>
            /// A category of events for objects being disposed
            /// </summary>
            public const EventKeywords Dispose = (EventKeywords)0x0002;

            /// <summary>
            /// A category of events for objects being finalized
            /// </summary>
            public const EventKeywords Finalize = (EventKeywords)0x0004;

            /// <summary>
            /// A category of events for memory allocation management.
            /// </summary>
            public const EventKeywords Memory = (EventKeywords)0x0008;

            /// <summary>
            /// A category of events for capacity management.
            /// </summary>
            public const EventKeywords Capacity = (EventKeywords)0x0010;
        }
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>
        /// The Event Source specific Event Tasks for events
        /// </summary>
        public class Tasks
        {
            /// <summary>
            /// MemoryStreamSlim task
            /// </summary>
            public const EventTask MemoryStreamSlim = (EventTask)0x0001;

            /// <summary>
            /// Buffer Memory task
            /// </summary>
            public const EventTask BufferMemory = (EventTask)0x0002;
        }
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>
        /// The Event Source specific OpCodes for events
        /// </summary>
        public static class Opcodes
        {
            /// <summary>
            /// OpCode for items that are initially created
            /// </summary>
            public const EventOpcode Create = (EventOpcode)11;

            /// <summary>
            /// OpCode for items that are disposed
            /// </summary>
            public const EventOpcode Dispose = (EventOpcode)12;

            /// <summary>
            /// OpCode for items that are finalized
            /// </summary>
            public const EventOpcode Finalize = (EventOpcode)13;

            /// <summary>
            /// OpCode for capacity growing events
            /// </summary>
            public const EventOpcode CapacityExpand = (EventOpcode)14;

            /// <summary>
            /// OpCode for capacity reducing events
            /// </summary>
            public const EventOpcode CapacityReduced = (EventOpcode)15;

            /// <summary>
            /// OpCode for buffer allocation events
            /// </summary>
            public const EventOpcode BufferAllocate = (EventOpcode)16;

            /// <summary>
            /// OpCode for buffer release events
            /// </summary>
            public const EventOpcode BufferRelease = (EventOpcode)17;

            /// <summary>
            /// OpCode for array heap memory allocation events
            /// </summary>
            public const EventOpcode ArrayAllocate = (EventOpcode)18;
        }
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>
        /// The singleton instance of the event source.
        /// </summary>
        public static UtilsEventSource Log { get; } = new();

        //--------------------------------------------------------------------------------
        private const int EventId_MemoryStreamSlimCreated = 1;
        /// <summary>
        /// Event to report the creation of a memory stream slim instance.
        /// </summary>
        [Event(EventId_MemoryStreamSlimCreated,
            Keywords = Keywords.Create,
            Task = Tasks.MemoryStreamSlim,
            Opcode = Opcodes.Create,
            Level = EventLevel.Informational)]
        private void MemoryStreamSlimCreatedInternal (string streamId, long maximumCapacity, string mode, string zeroBehavior)
        {
            // Event monitoring enabled is checked in the caller
            WriteEvent(EventId_MemoryStreamSlimCreated, streamId, maximumCapacity, mode, zeroBehavior);
        }
        //--------------------------------------------------------------------------------
        private const int EventId_MemoryStreamSlimDisposed = 2;
        /// <summary>
        /// Event to report the disposal of a memory stream slim instance.
        /// </summary>
        [Event(EventId_MemoryStreamSlimDisposed,
            Keywords = Keywords.Dispose,
            Task = Tasks.MemoryStreamSlim,
            Opcode = Opcodes.Dispose,
            Level = EventLevel.Informational)]
        public void MemoryStreamSlimDisposed (string streamId)
        {
            if (IsEnabled(EventLevel.Informational, Keywords.Dispose))
                WriteEvent(EventId_MemoryStreamSlimDisposed, streamId);
        }
        //--------------------------------------------------------------------------------
        private const int EventId_MemoryStreamSlimFinalized = 3;
        /// <summary>
        /// Event to report a memory stream slim instance being finalized.
        /// </summary>
        [Event(EventId_MemoryStreamSlimFinalized,
            Keywords = Keywords.Finalize,
            Task = Tasks.MemoryStreamSlim,
            Opcode = Opcodes.Finalize,
            Level = EventLevel.Warning)]
        public void MemoryStreamSlimFinalized (string streamId)
        {
            if (IsEnabled(EventLevel.Warning, Keywords.Finalize))
                WriteEvent(EventId_MemoryStreamSlimFinalized, streamId);
        }
        //--------------------------------------------------------------------------------
        private const int EventId_MemoryStreamSlimCapacityExpanded = 4;
        /// <summary>
        /// Event to report a memory stream slim instance is expanding its capacity.
        /// </summary>
        [Event(EventId_MemoryStreamSlimCapacityExpanded,
            Keywords = Keywords.Capacity,
            Task = Tasks.MemoryStreamSlim,
            Opcode = Opcodes.CapacityExpand,
            Level = EventLevel.Informational)]
        public void MemoryStreamSlimCapacityExpanded (string streamId, int oldCapacity, int newCapacity)
        {
            if (IsEnabled(EventLevel.Informational, Keywords.Capacity))
                WriteEvent(EventId_MemoryStreamSlimCapacityExpanded, streamId, oldCapacity, newCapacity);
        }
        //--------------------------------------------------------------------------------
        private const int EventId_MemoryStreamSlimCapacityReduced = 5;
        /// <summary>
        /// Event to report a memory stream slim instance is reducing its capacity.
        /// </summary>
        [Event(EventId_MemoryStreamSlimCapacityReduced,
            Keywords = Keywords.Capacity,
            Task = Tasks.MemoryStreamSlim,
            Opcode = Opcodes.CapacityReduced,
            Level = EventLevel.Informational)]
        public void MemoryStreamSlimCapacityReduced (string streamId, int oldCapacity, int newCapacity)
        {
            if (IsEnabled(EventLevel.Informational, Keywords.Capacity))
                WriteEvent(EventId_MemoryStreamSlimCapacityReduced, streamId, oldCapacity, newCapacity);
        }
        //--------------------------------------------------------------------------------
        private const int EventId_MemoryBufferAllocate = 6;
        /// <summary>
        /// Event to report a buffer memory allocation from the Gc Heap.
        /// </summary>
        [Event(EventId_MemoryBufferAllocate,
            Keywords = Keywords.Memory,
            Task = Tasks.BufferMemory,
            Opcode = Opcodes.BufferAllocate,
            Level = EventLevel.Informational)]
        private void BufferMemoryAllocateInternal (int allocationSize, string bufferType)
        {
            // Event monitoring enabled is checked in the caller
            WriteEvent(EventId_MemoryBufferAllocate, allocationSize, bufferType);
        }
        //--------------------------------------------------------------------------------
        private const int EventId_BufferMemoryRelease = 7;
        /// <summary>
        /// Event to report a memory buffer released
        /// </summary>
        [Event(EventId_BufferMemoryRelease,
            Keywords = Keywords.Memory,
            Task = Tasks.BufferMemory,
            Opcode = Opcodes.BufferRelease,
            Level = EventLevel.Informational)]
        private void BufferMemoryRelease (int releaseSize, string bufferType)
        {
            if (IsEnabled(EventLevel.Informational, Keywords.Memory))
                WriteEvent(EventId_BufferMemoryRelease, releaseSize, bufferType);
        }
        //--------------------------------------------------------------------------------
        private const int EventId_MemoryStreamSlimToArray = 8;
        /// <summary>
        /// Event to report a ToArray call on a memory stream slim instance.
        /// </summary>
        [Event(EventId_MemoryStreamSlimToArray,
            Keywords = Keywords.Memory,
            Task = Tasks.MemoryStreamSlim,
            Opcode = Opcodes.ArrayAllocate,
            Level = EventLevel.Warning)]
        public void MemoryStreamSlimToArray (string streamId, int arraySize)
        {
            if (IsEnabled(EventLevel.Warning, Keywords.Memory))
                WriteEvent(EventId_MemoryStreamSlimToArray, streamId, arraySize);
        }
        //--------------------------------------------------------------------------------

        //--------------------------------------------------------------------------------
        /// <summary>
        /// Event to report the creation of a memory stream slim instance.
        /// </summary>
        [NonEvent]
        public void MemoryStreamSlimCreated (string streamId, long maximumCapacity, 
            MemoryStreamSlimMode mode, MemoryStreamSlimSettings settings)
        {
            if (IsEnabled(EventLevel.Informational, Keywords.Create))
                MemoryStreamSlimCreatedInternal(streamId, maximumCapacity, mode.GetString(), settings.ZeroBufferBehavior.GetString());
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Event to report GC Heap memory allocation.
        /// </summary>
        [NonEvent]
        public void BufferGcMemoryAllocated (int allocationSize)
        {
            if (IsEnabled(EventLevel.Informational, Keywords.Memory))
                BufferMemoryAllocateInternal(allocationSize, GcHeapMemoryTypeName);
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Event to report Native Heap memory allocation.
        /// </summary>
        [NonEvent]
        public void BufferNativeMemoryAllocated (int allocationSize)
        {
            if (IsEnabled(EventLevel.Informational, Keywords.Memory))
                BufferMemoryAllocateInternal(allocationSize, NativeHeapMemoryTypeName);
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Event to report GC Heap memory release.
        /// </summary>
        [NonEvent]
        public void BufferGcMemoryReleased (int allocationSize)
        {
            if (IsEnabled(EventLevel.Informational, Keywords.Memory))
                BufferMemoryRelease(allocationSize, GcHeapMemoryTypeName);
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Event to report Native Heap memory release.
        /// </summary>
        [NonEvent]
        public void BufferNativeMemoryReleased (int allocationSize)
        {
            if (IsEnabled(EventLevel.Informational, Keywords.Memory))
                BufferMemoryRelease(allocationSize, NativeHeapMemoryTypeName);
        }
        //--------------------------------------------------------------------------------
    }
    //################################################################################
}
