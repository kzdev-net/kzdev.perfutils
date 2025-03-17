# Compression

Compression is often used with memory streams to reduce the size of the data being stored or transmitted. This can be particularly useful when dealing with large amounts of data or when bandwidth is limited.

.NET provides several classes for compression, including `GZipStream` and `DeflateStream`, which can be used in conjunction with memory streams. To simplify the process of compressing and decompressing data, helper classes exist in this library, such as `MemoryStreamGZip` and `MemoryStreamDeflate`. These classes provide a convenient way to handle compression without having to manually manage the underlying streams.

## Helper Classes

The full list of helper classes available for compression are:

- `MemoryStreamGZip`: A helper class that uses GZip compression with a memory stream.
- `MemoryStreamDeflate`: A helper class that uses Deflate compression with a memory stream.
- `MemoryStreamBrotli`: A helper class that uses Brotli compression with a memory stream.
- `MemoryStreamZLib`: A helper class that uses ZLib compression with a memory stream.