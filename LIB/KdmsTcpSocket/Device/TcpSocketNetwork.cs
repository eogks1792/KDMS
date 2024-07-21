using KdmsTcpSocket.Interfaces;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Net.Sockets;

namespace KdmsTcpSocket.Device;

public class TcpSocketNetwork : ITcpSocketNetwork
{
    private const int TimeWaitResponse = 1000;
    private readonly object _serverLock = new object();

    private readonly ConcurrentDictionary<string, TcpSocketConnection> _tcpClients =
        new ConcurrentDictionary<string, TcpSocketConnection>();

    private TcpListener _listener;
    //public TcpSocketNetwork(ITcpSocketTransport transport)
    //{
    //}

    public TcpSocketNetwork(TcpListener tcpListener)
    {
        if (tcpListener == null)
        {
            throw new ArgumentNullException(nameof(tcpListener));
        }

        _listener = tcpListener;
    }


    public ReadOnlyCollection<TcpClient> TcpClients
    {
        get
        {
            return new ReadOnlyCollection<TcpClient>(_tcpClients.Values.Select(mc => mc.TcpClient).ToList());
        }
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }

    public Task ListenAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        throw new NotImplementedException();
    }

    private TcpListener Server
    {
        get
        {
            if (_listener == null)
            {
                throw new ObjectDisposedException("Server");
            }

            return _listener;
        }
    }
    //public abstract Task ListenAsync(CancellationToken cancellationToken = new CancellationToken());
}
