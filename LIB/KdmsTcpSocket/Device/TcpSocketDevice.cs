using KdmsTcpSocket.Interfaces;
using KdmsTcpSocket.Unme.Common;

namespace KdmsTcpSocket.Device;

public abstract class TcpSocketDevice : IDisposable
{
    private ITcpSocketTransport _transport;

    protected TcpSocketDevice(ITcpSocketTransport transport)
    {
        _transport = transport;
    }

    /// <summary>
    ///     Gets the Modbus Transport.
    /// </summary>
    public ITcpSocketTransport Transport => _transport;

    /// <summary>
    ///     Releases unmanaged and - optionally - managed resources.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    ///     Releases unmanaged and - optionally - managed resources.
    /// </summary>
    /// <param name="disposing">
    ///     <c>true</c> to release both managed and unmanaged resources;
    ///     <c>false</c> to release only unmanaged resources.
    /// </param>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            DisposableUtility.Dispose(ref _transport);
        }
    }
}
