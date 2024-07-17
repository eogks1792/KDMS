namespace KdmsTcpSocket.Interfaces
{
    /// <summary>
    ///     Modbus master device.
    /// </summary>
    public interface ITcpSocketMaster : IDisposable
    {
        /// <summary>
        ///     Transport used by this master.
        /// </summary>
        ITcpSocketTransport Transport { get; }


        ITcpSocketMessage SendData<T>(ushort requestCode, ushort responseCode, T? data)
            where T : struct;

        ITcpSocketMessage SendListData<T>(ushort requestCode, ushort responseCode, IEnumerable<T>? data)
            where T : struct;

        ITcpSocketMessage Send(ITcpSocketMessage request);
        ITcpSocketMessage Recv();
    }
}
