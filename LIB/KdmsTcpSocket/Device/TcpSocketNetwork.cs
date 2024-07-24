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

    private TcpListener _server;
    private readonly EventHandler<TcpRequestEventArgs> _rcvProcFunc;

    //public TcpSocketNetwork(ITcpSocketTransport transport)
    //{
    //}

    public TcpSocketNetwork(TcpListener tcpListener, EventHandler<TcpRequestEventArgs> rcvProcFunc)
    {
        if (tcpListener == null)
        {
            throw new ArgumentNullException(nameof(tcpListener));
        }

        _server = tcpListener;
        _rcvProcFunc = rcvProcFunc;
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
        // double-check locking
        if (_server != null)
        {
            lock (_serverLock)
            {
                if (_server != null)
                {
                    _server.Stop();
                    _server = null;


                    foreach (var key in _tcpClients.Keys)
                    {
                        if (_tcpClients.TryRemove(key, out TcpSocketConnection connection))
                        {
                            connection.TcpClientConnectionClosed -= OnClientConnectionClosedHandler;
                            connection.Dispose();
                        }
                    }
                }
            }
        }
    }

    public async Task ListenAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        Server.Start();
        using (cancellationToken.Register(() => Server.Stop()))
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    TcpClient client = await Server.AcceptTcpClientAsync().ConfigureAwait(false);
                    var masterConnection = new TcpSocketConnection(client, this);
                    masterConnection.TcpClientConnectionClosed += OnClientConnectionClosedHandler;
                    masterConnection.DataProcHandler += _rcvProcFunc;
                    Console.WriteLine($"Client:{client.Client.RemoteEndPoint.ToString()} CONNECTED");
                    _tcpClients.TryAdd(client.Client.RemoteEndPoint.ToString(), masterConnection);
                }
            }
            catch (ObjectDisposedException) when (cancellationToken.IsCancellationRequested)
            {
                //Swallow this
            }
            catch (InvalidOperationException)
            {
                // Either Server.Start wasn't called (a bug!)
                // or the CancellationToken was cancelled before
                // we started accepting (giving an InvalidOperationException),
                // or the CancellationToken was cancelled after
                // we started accepting (giving an ObjectDisposedException).
                //
                // In the latter two cases we should surface the cancellation
                // exception, or otherwise rethrow the original exception.
                cancellationToken.ThrowIfCancellationRequested();
                throw;
            }
        }
    }

    private TcpListener Server
    {
        get
        {
            if (_server == null)
            {
                throw new ObjectDisposedException("Server");
            }

            return _server;
        }
    }

    private void OnClientConnectionClosedHandler(object sender, TcpConnectionEventArgs e)
    {
        if (!_tcpClients.TryRemove(e.EndPoint, out TcpSocketConnection connection))
        {
            string msg = $"EndPoint {e.EndPoint} cannot be removed, it does not exist.";
            throw new ArgumentException(msg);
        }

        connection.Dispose();
        Console.WriteLine($"Removed Master {e.EndPoint}");
    }
    //public abstract Task ListenAsync(CancellationToken cancellationToken = new CancellationToken());
}
