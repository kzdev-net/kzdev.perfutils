# Compression

Compression is often used with memory streams to reduce the size of data being stored or transmitted. This is particularly useful when dealing with large amounts of data or when bandwidth is limited.

.NET provides several classes for compression, including `GZipStream` and `DeflateStream`, which can be used in conjunction with memory streams. To simplify the process of compressing and decompressing data, this library provides helper classes such as `MemoryStreamGZip` and `MemoryStreamDeflate`. These classes offer a convenient way to handle compression without requiring manual management of the underlying streams.

## Helper Classes

The full list of helper classes available for compression are:

- [MemoryStreamDeflate](xref:KZDev.PerfUtils.MemoryStreamDeflate): A helper class that uses Deflate compression with a **MemoryStreamSlim**.
- [MemoryStreamGZip](xref:KZDev.PerfUtils.MemoryStreamGZip): A helper class that uses GZip compression with a **MemoryStreamSlim**.
- [MemoryStreamZLib](xref:KZDev.PerfUtils.MemoryStreamZLib): A helper class that uses ZLib compression with a **MemoryStreamSlim**.
- [MemoryStreamBrotli](xref:KZDev.PerfUtils.MemoryStreamBrotli): A helper class that uses Brotli compression with a **MemoryStreamSlim**.


## Examples

Using these helper classes is straightforward, as the usage pattern is consistent across all of them. Additionally, there is no need to manage intermediate streams manually.

## Simple Data Compression

Here's an example of how to use `MemoryStreamDeflate` to compress data in the simplest case:

```csharp
using KZDev.PerfUtils;

public class CompressionExample
{
    public MemoryStreamSlim CompressData(byte[] data)
    {
        return MemoryStreamDeflate.Compress(data);
    }
}
```

## Compression With Options

Here's an example of how to use `MemoryStreamDeflate` to compress data using the best compression level by utilizing the options setup delegate overload:

```csharp
using KZDev.PerfUtils;

public class CompressionExample
{
    public MemoryStreamSlim CompressDataWithSmallestSize(byte[] data)
    {
        return MemoryStreamDeflate.Compress(data, 
            options => options.WithCompressionLevel(CompressionLevel.SmallestSize));
    }
}
```

All the different compression classes have overloads to compress source data provided in a `byte[]`, `ReadOnlySpan<byte>`, `string`, or `Stream`.