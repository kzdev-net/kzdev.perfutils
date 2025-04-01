// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.IO.Compression;

using KZDev.PerfUtils;

namespace Examples.Compression;

public class CompressionExample
{
    public MemoryStreamSlim CompressData(byte[] data)
    {
        return MemoryStreamDeflate.Compress(data);
    }

    public MemoryStreamSlim CompressDataWithSmallestSize(byte[] data)
    {
        return MemoryStreamDeflate.Compress(data, options => options.WithCompressionLevel(CompressionLevel.SmallestSize));
    }
}