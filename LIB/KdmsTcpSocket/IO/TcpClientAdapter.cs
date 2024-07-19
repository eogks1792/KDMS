using KdmsTcpSocket.Unme.Common;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;

namespace KdmsTcpSocket.IO;

public class TcpClientAdapter : IStreamResource
{
    private TcpClient _tcpClient;

    public TcpClientAdapter(TcpClient tcpClient)
    {
        Debug.Assert(tcpClient != null, "Argument tcpClient cannot be null.");
        //tcpClient.Client.LocalEndPoint.ToString();
        //var aaa = ((IPEndPoint)tcpClient.Client.RemoteEndPoint).Port.ToString();
        _tcpClient = tcpClient;
    }

    public int InfiniteTimeout => Timeout.Infinite;

    public int ReadTimeout
    {
        get => _tcpClient.GetStream().ReadTimeout;
        set => _tcpClient.GetStream().ReadTimeout = value;
    }

    public int WriteTimeout
    {
        get => _tcpClient.GetStream().WriteTimeout;
        set => _tcpClient.GetStream().WriteTimeout = value;
    }

    public string GetConnPort => ((IPEndPoint)_tcpClient.Client.RemoteEndPoint).Port.ToString();

    public void Write(byte[] buffer, int offset, int size)
    {
        _tcpClient.GetStream().Write(buffer, offset, size);
    }

    public int Read(byte[] buffer, int offset, int size)
    {
        return _tcpClient.GetStream().Read(buffer, offset, size);
    }

    public void DiscardInBuffer()
    {
        _tcpClient.GetStream().Flush();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            DisposableUtility.Dispose(ref _tcpClient);
        }
    }
}
