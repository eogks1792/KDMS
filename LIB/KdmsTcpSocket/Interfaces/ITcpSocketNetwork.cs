using System.Collections.ObjectModel;
using System.Net.Sockets;

namespace KdmsTcpSocket.Interfaces;

public interface ITcpSocketNetwork : IDisposable
{
    /// <summary>
    /// Listen for incoming requests.
    /// </summary>
    /// <returns></returns>
    Task ListenAsync(CancellationToken cancellationToken = new CancellationToken());

    ReadOnlyCollection<TcpClient> TcpClients { get; }
}
