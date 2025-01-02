# Monitoring Events

For detailed monitoring of `StringBuilderCache` cache management, you can use the `EventSource` events that are provided by the `PerfUtils` library with tools such as [`PerfView`](#perfview-example). The event source name is `KZDev.PerfUtils`. The following events are available.

## StringBuilderCreate event

This event is raised when a new `StringBuilderCache.Acquire` method is called and a new `StringBuilder` instance is created because either the requested size is larger than the maximum size allowed in the cache or no `StringBuilder` instances are available in the cache to satisfy the request.

The following table shows the task, keyword, level, and opcode.

| Task | Keyword | Level | Opcode |
| --- | --- | --- | --- |
| StringBuilderCache (0x0003) | Create (0x0001) | Informational (4) | Create (11) |

The following table shows the event information.

| Event | Event ID | Raised when |
| --- | --- | --- |
StringBuilderCreate | 21 | A new StringBuilder instance is created. |

The following table shows the event data.

| Name | Type | Description |
| --- | --- | --- |
| requestedCapacity | Int32 | The requested StringBuilder capacity. |
| builderCapacity | Int32 | The actual capacity of the created StringBuilder instance. |

---

## StringBuilderCacheMiss event

This event is raised when a `StringBuilderCache.Acquire` method is called and no `StringBuilder` instances are available in the cache to satisfy the request.

The following table shows the task, keyword, level, and opcode.

| Task | Keyword | Level | Opcode |
| --- | --- | --- | --- |
| StringBuilderCache (0x0003) | Cache (0x0020) | Informational (4) | StringBuilderCacheMiss (20) |

The following table shows the event information.

| Event | Event ID | Raised when |
| --- | --- | --- |
StringBuilderCacheMiss | 22 | A StringBuilder instance is not available in the cache. |

The following table shows the event data.

| Name | Type | Description |
| --- | --- | --- |
| requestedCapacity | Int32 | The requested StringBuilder capacity. |

---

## StringBuilderCacheHit event

This event is raised when a `StringBuilderCache.Acquire` method is called and a `StringBuilder` instance is available in the cache and returned to satisfy the request.

The following table shows the task, keyword, level, and opcode.

| Task | Keyword | Level | Opcode |
| --- | --- | --- | --- |
| StringBuilderCache (0x0003) | Cache (0x0020) | Warning (3) | StringBuilderCacheHit (21) |

The following table shows the event information.

| Event | Event ID | Raised when |
| --- | --- | --- |
StringBuilderCacheHit | 23 | A StringBuilder is returned from the cache. |

The following table shows the event data.

| Name | Type | Description |
| --- | --- | --- |
| requestedCapacity | Int32 | The requested StringBuilder capacity. |
| builderCapacity | Int32 | The actual capacity of the returned StringBuilder instance. |
| cacheType | UnicodeString | The type of cache the returned StringBuilder was pulled from. One of ['Global', 'Thread Local'] |

---

## StringBuilderCacheStore event

This event is raised when a `StringBuilder` instance is stored in the cache.

The following table shows the task, keyword, level, and opcode.

| Task | Keyword | Level | Opcode |
| --- | --- | --- | --- |
| StringBuilderCache (0x0003) | Cache (0x0020) | Informational (4) | StringBuilderCacheStore (22) |

The following table shows the event information.

| Event | Event ID | Raised when |
| --- | --- | --- |
StringBuilderCacheStore | 24 | A StringBuilder instance is stored in the cache. |

The following table shows the event data.

| Name | Type | Description |
| --- | --- | --- |
| builderCapacity | Int32 | The actual capacity of the stored StringBuilder instance. |
| cacheType | UnicodeString | The type of cache the stored StringBuilder was placed in. One of ['Global', 'Thread Local'] |

---

### PerfView example

To capture all the events in [PerfView](https://github.com/microsoft/perfview) from the `KZDev.PerfUtils` library, you can use the string `*KZDev.PerfUtils` as an argument to the `-providers` option of the `perfview` command line tool. To also capture stack traces for the events, you can use the `StacksEnabled` command (`*KZDev.PerfUtils:@StacksEnabled=true`). See the [`perfview`](https://github.com/microsoft/perfview) documentation for more information on how to use the tool.

