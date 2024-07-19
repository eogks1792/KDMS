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

    public static byte[] CompressUsingZlib(byte[] data)
    {
        using (MemoryStream compressedStream = new MemoryStream())
        {
            using (DeflaterOutputStream zlibStream = new DeflaterOutputStream(compressedStream, new Deflater(Deflater.BEST_COMPRESSION)))
            {
                zlibStream.Write(data, 0, data.Length);
                zlibStream.Finish(); // 남아 있는 데이터를 플러시
            }
            return compressedStream.ToArray();
        }
    }
}
