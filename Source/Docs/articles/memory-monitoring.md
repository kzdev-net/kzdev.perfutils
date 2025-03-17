# Monitoring

`MemoryStreamSlim` provides a couple of ways that you can monitor usage, using the [`Metrics`](https://learn.microsoft.com/en-us/dotnet/core/diagnostics/metrics) and `Events` features of the .NET runtime.

## Metrics

`MemoryStreamSlim` provides a few metrics that you can use with tools such as [`dotnet-counters`](https://learn.microsoft.com/en-us/dotnet/core/diagnostics/dotnet-counters) to monitor the memory usage of `MemoryStreamSlim` instances in your application. 

`"kzdev.perfutils.memory"` is the counter name for the metrics that are provided by `MemoryStreamSlim`. 

For example, you can use the following command to monitor the `MemoryStreamSlim` metrics for a specific process by name.

```cmd
dotnet-counters monitor -n <process-name> --counters "kzdev.perfutils.memory"
```

Or you can use the following command to monitor the `MemoryStreamSlim` metrics for a specific process by process id.

```cmd
dotnet-counters monitor -p <process-id> --counters "kzdev.perfutils.memory"
```

---

The following metric counters are available.

---

### MemoryStreamSlim.Count Counter

This counter provides the number of `MemoryStreamSlim` instances that are currently active in the application. The counter name is `memory_stream_slim.count`.

| Units | Description |
| --- | --- |
| \{instances\} | The number of active MemoryStreamSlim instances. |

---

### SegmentMemory.GCAllocated Counter

This counter provides the number of buffer segments currently allocated from the GC Large Object Heap, if GC Heap memory is being used for the memory buffers. The counter name is `segment_memory.gc_allocated`.

| Units | Description |
| --- | --- |
| \{segments\} | The total number of GC heap segments (of 64K bytes) allocated for the segmented memory buffers. |

---

### SegmentMemory.NativeAllocated Counter

This counter provides the number of buffer segments currently allocated from the native OS heap if native memory is being used for the memory buffers. The counter name is `segment_memory.native_allocated`.

| Units | Description |
| --- | --- |
| \{segments\} | The total number of native memory segments (of 64K bytes) allocated for the segmented memory buffers. |

---

## Events

For detailed monitoring of `MemoryStreamSlim` instances and memory management, you can use the `EventSource` events that are provided by the `PerfUtils` library with tools such as [`PerfView`](#perfview-example). The event source name is `KZDev.PerfUtils`. The following events are available.

### MemoryStreamSlimCreated event

This event is raised when a new `MemoryStreamSlim` instance is created.

The following table shows the task, keyword, level, and opcode.

| Task | Keyword | Level | Opcode |
| --- | --- | --- | --- |
| MemoryStreamSlim (0x0001) | Create (0x0001) | Informational (4) | Create (11) |

The following table shows the event information.

| Event | Event ID | Raised when |
| --- | --- | --- |
MemoryStreamSlimCreated | 1 | A new MemoryStreamSlim instance is created. |

The following table shows the event data.

| Name | Type | Description |
| --- | --- | --- |
| StreamId | Guid | The unique identifier for the MemoryStreamSlim instance. |
| MaximumCapacity | Int32 | The maximum capacity set for the MemoryStreamSlim instance. |
| ZeroBehavior | UnicodeString | The type of buffer clearing used for the memory buffer segments in this MemoryStreamSlim instance. One of ['None', 'OutOfBand', 'OnRelease'] |

---

### MemoryStreamSlimDisposed event

This event is raised when a `MemoryStreamSlim` instance is disposed.

The following table shows the task, keyword, level, and opcode.

| Task | Keyword | Level | Opcode |
| --- | --- | --- | --- |
| MemoryStreamSlim (0x0001) | Dispose (0x0002) | Informational (4) | Dispose (12) |

The following table shows the event information.

| Event | Event ID | Raised when |
| --- | --- | --- |
MemoryStreamSlimDisposed | 2 | A MemoryStreamSlim instance is diposed. |

The following table shows the event data.

| Name | Type | Description |
| --- | --- | --- |
| StreamId | Guid | The unique identifier for the MemoryStreamSlim instance. |

---

### MemoryStreamSlimFinalized event

This event is raised when a `MemoryStreamSlim` instance finalizer is executed. This event will not be raised if the `MemoryStreamSlim` instance is disposed properly.

The following table shows the task, keyword, level, and opcode.

| Task | Keyword | Level | Opcode |
| --- | --- | --- | --- |
| MemoryStreamSlim (0x0001) | Finalize (0x0004) | Warning (3) | Finalize (13) |

The following table shows the event information.

| Event | Event ID | Raised when |
| --- | --- | --- |
MemoryStreamSlimFinalized | 3 | A MemoryStreamSlim instance finalizer is executed. |

The following table shows the event data.

| Name | Type | Description |
| --- | --- | --- |
| StreamId | Guid | The unique identifier for the MemoryStreamSlim instance. |

---

### MemoryStreamSlimCapacityExpanded event

This event is raised when a `MemoryStreamSlim` instance capacity is expanded.

The following table shows the task, keyword, level, and opcode.

| Task | Keyword | Level | Opcode |
| --- | --- | --- | --- |
| MemoryStreamSlim (0x0001) | Capacity (0x0010) | Informational (4) | CapacityExpand (14) |

The following table shows the event information.

| Event | Event ID | Raised when |
| --- | --- | --- |
MemoryStreamSlimCapacityExpanded | 4 | A MemoryStreamSlim instance capacity is expanded. |

The following table shows the event data.

| Name | Type | Description |
| --- | --- | --- |
| StreamId | Guid | The unique identifier for the MemoryStreamSlim instance. |
| OldCapacity | Int64 | The capacity of the MemoryStreamSlim instance before the capacity expansion. |
| NewCapacity | Int64 | The new expanded capacity of the MemoryStreamSlim instance. |

---

### MemoryStreamSlimCapacityReduced event

This event is raised when a `MemoryStreamSlim` instance capacity is reduced.

The following table shows the task, keyword, level, and opcode.

| Task | Keyword | Level | Opcode |
| --- | --- | --- | --- |
| MemoryStreamSlim (0x0001) | Capacity (0x0010) | Informational (4) | CapacityReduced (15) |

The following table shows the event information.

| Event | Event ID | Raised when |
| --- | --- | --- |
MemoryStreamSlimCapacityReduced | 5 | A MemoryStreamSlim instance capacity is reduced. |

The following table shows the event data.

| Name | Type | Description |
| --- | --- | --- |
| StreamId | Guid | The unique identifier for the MemoryStreamSlim instance. |
| OldCapacity | Int64 | The capacity of the MemoryStreamSlim instance before the capacity reduction. |
| NewCapacity | Int64 | The new reduced capacity of the MemoryStreamSlim instance. |

---

### BufferMemoryAllocated event

This event is raised when a memory allocation is made for needed memory buffers.

The following table shows the task, keyword, level, and opcode.

| Task | Keyword | Level | Opcode |
| --- | --- | --- | --- |
| BufferMemory (0x0002) | Memory (0x0008) | Informational (4) | BufferAllocate (16) |

The following table shows the event information.

| Event | Event ID | Raised when |
| --- | --- | --- |
BufferMemoryAllocated | 6 | A memory allocation was made for needed memory buffers. |

The following table shows the event data.

| Name | Type | Description |
| --- | --- | --- |
| AllocationSize | Int32 | The size (in bytes) of the newly allocated memory. |
| BufferType | UnicodeString | The type of memory that was allocated. One of ['GC Heap', 'Native'] |

---

### BufferMemoryRelease event

This event is raised when a previous memory allocation for memory buffers is released.

The following table shows the task, keyword, level, and opcode.

| Task | Keyword | Level | Opcode |
| --- | --- | --- | --- |
| BufferMemory (0x0002) | Memory (0x0008) | Informational (4) | BufferRelease (17) |

The following table shows the event information.

| Event | Event ID | Raised when |
| --- | --- | --- |
BufferMemoryRelease | 7 | A previous memory allocation for memory buffers is released. |

The following table shows the event data.

| Name | Type | Description |
| --- | --- | --- |
| ReleaseSize | Int32 | The size (in bytes) of the released memory. |
| BufferType | UnicodeString | The type of memory that was released. One of ['GC Heap', 'Native'] |

---

### MemoryStreamSlimToArray event

This event is raised when the ToArray() method is called on a `MemoryStreamSlim` instance and returns a non-zero length array. This is useful for tracking extra heap memory allocations caused by calling the ToArray() method. This also records the internal operation of putting all of the
  bytes into a contiguous array for decoding the bytes into a string.

The following table shows the task, keyword, level, and opcode.

| Task | Keyword | Level | Opcode |
| --- | --- | --- | --- |
| MemoryStreamSlim (0x0001) | Memory (0x0008) | Warning (3) | ArrayAllocate (18) |

The following table shows the event information.

| Event | Event ID | Raised when |
| --- | --- | --- |
MemoryStreamSlimToArray | 8 | ToArray() is called on a MemoryStreamSlim instance and returns a non-zero length array result. |

The following table shows the event data.

| Name | Type | Description |
| --- | --- | --- |
| StreamId | Guid | The unique identifier for the MemoryStreamSlim instance. |
| ArraySize | Int32 | The size (in bytes) of the returned heap allocated array. |
| StringDecode | Int32 | Indicates if the contiguous array was used for string decoding. 0 = ToArray() called, 1 = Decode() called. |

---

### PerfView example

To capture all the events in [PerfView](https://github.com/microsoft/perfview) from the `KZDev.PerfUtils` library, you can use the string `*KZDev.PerfUtils` as an argument to the `-providers` option of the `perfview` command line tool. To also capture stack traces for the events, you can use the `StacksEnabled` command (`*KZDev.PerfUtils:@StacksEnabled=true`). See the [`perfview`](https://github.com/microsoft/perfview) documentation for more information on how to use the tool.

