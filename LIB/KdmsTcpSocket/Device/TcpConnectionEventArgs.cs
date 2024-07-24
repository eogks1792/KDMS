using KdmsTcpSocket.Interfaces;
using System.Net;

namespace KdmsTcpSocket.Device;

public class TcpConnectionEventArgs : EventArgs
{
    public TcpConnectionEventArgs(string endPoint)
    {
        if (endPoint == null)
        {
            throw new ArgumentNullException(nameof(endPoint));
        }

        if (endPoint == string.Empty)
        {
            throw new ArgumentException(Resources.EmptyEndPoint);
        }

        EndPoint = endPoint;
    }

    public string EndPoint { get; set; }
}

public class TcpRequestEventArgs : EventArgs
{
    public TcpRequestEventArgs(ITcpSocketTransport clientTransport, ITcpSocketMessage request)
    {
        if (clientTransport == null)
        {
            throw new ArgumentNullException(nameof(clientTransport));
        }

        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        ClientTransport = clientTransport;
        Request = request;
    }

    public ITcpSocketMessage Request { get; set; }
    public ITcpSocketTransport ClientTransport { get; set; }

}
