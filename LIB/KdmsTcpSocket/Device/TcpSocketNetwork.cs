using KdmsTcpSocket.Interfaces;

namespace KdmsTcpSocket.Device;

public class TcpSocketNetwork : TcpSocketDevice//, ITcpSocketNetwork
{
    public TcpSocketNetwork(ITcpSocketTransport transport) : base(transport)
    {
    }

    //public abstract Task ListenAsync(CancellationToken cancellationToken = new CancellationToken());
}
