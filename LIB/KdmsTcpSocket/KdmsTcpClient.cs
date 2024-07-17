using KdmsTcpSocket.Device;
using KdmsTcpSocket.Interfaces;
using KdmsTcpSocket.IO;
using System.Net.Sockets;

namespace KdmsTcpSocket;

public class KdmsTcpClient
{
    public static ITcpSocketMaster CreateKdmsSocketMaster(TcpClient client)
    {
        var adapter = new TcpClientAdapter(client);

        var transport = new TcpSocketIpTransport(adapter);

        return new TcpSocketMaster(transport);
    }
}
