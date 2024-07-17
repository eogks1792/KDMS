using KdmsTcpSocket.Interfaces;
using KdmsTcpSocket.Message;

namespace KdmsTcpSocket.Device
{
    /// <summary>
    ///     Modbus master device.
    /// </summary>
    public class TcpSocketMaster : TcpSocketDevice, ITcpSocketMaster
	{
		public TcpSocketMaster(ITcpSocketTransport transport)
				: base(transport)
		{
		}

        public ITcpSocketMessage Recv()
        {
            return Transport.ResponseData();
        }

        public ITcpSocketMessage Send(ITcpSocketMessage request)
        {
            return Transport.ResponseData(request);
        }

        public ITcpSocketMessage SendData<T>(ushort requestCode, ushort responseCode, T? data) where T : struct
        {
            var request = new KdmsDataRequest<T>(requestCode, responseCode, data);
            return Transport.ResponseData(request);
        }

        public ITcpSocketMessage SendListData<T>(ushort requestCode, ushort responseCode, IEnumerable<T>? data) where T : struct
        {
            var request = new KdmsDataListRequest<T>(requestCode, responseCode, data);
            return Transport.ResponseData(request);
        }


    }
}
