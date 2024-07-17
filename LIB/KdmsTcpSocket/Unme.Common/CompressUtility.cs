using ICSharpCode.SharpZipLib.Zip.Compression;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using KdmsTcpSocket.KdmsTcpStruct;
using System.IO.Compression;

namespace KdmsTcpSocket.Unme.Common;

public static class CompressUtility
{
    public static byte[] DecompressUsingZlib(byte[] compressedData)
    {
        using (MemoryStream compressedStream = new MemoryStream(compressedData))
        using (MemoryStream decompressedStream = new MemoryStream())
        using (InflaterInputStream zlibStream = new InflaterInputStream(compressedStream))
        {
            zlibStream.CopyTo(decompressedStream);
            return decompressedStream.ToArray();
        }
    }
}
