using KdmsTcpSocket.Interfaces;
using KdmsTcpSocket.IO;
using KdmsTcpSocket.Message;
using System.IO;
using System.Net.Sockets;

namespace KdmsTcpSocket.Device
{
    /// <summary>
    ///     Modbus master device.
    /// </summary>
    public class TcpSocketConnection : TcpSocketDevice, IDisposable
	{
        private readonly TcpClient _client;
        private readonly ITcpSocketNetwork _socketNetwork;
        private readonly string _endPoint;
        private readonly Stream _stream;
        //private readonly Task _requestHandlerTask;
        public TcpSocketConnection(TcpClient client, ITcpSocketNetwork socketNetwork)
				: base(new TcpSocketIpTransport(new TcpClientAdapter(client)))
		{
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _endPoint = client.Client.RemoteEndPoint.ToString();
            _stream = client.GetStream();
            _socketNetwork = socketNetwork;
            //_requestHandlerTask = Task.Run((Func<Task>)HandleRequestAsync);
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

        public string EndPoint => _endPoint;

        public Stream Stream => _stream;

        public TcpClient TcpClient => _client;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _stream.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
