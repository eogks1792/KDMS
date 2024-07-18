using KdmsTcpSocket.Extensions;
using KdmsTcpSocket.Interfaces;
using KdmsTcpSocket.KdmsTcpStruct;
using KdmsTcpSocket.Message;
using KdmsTcpSocket.Unme.Common;
using System.Data;
using System.Net.Sockets;

namespace KdmsTcpSocket.IO;

public abstract class TcpSocketTransport : ITcpSocketTransport
{
    private readonly object _syncLock = new object();
    private int _waitToRetryMilliseconds = TcpSocket.DefaultWaitToRetryMilliseconds;
    private IStreamResource _streamResource;

    //protected TcpSocketTransport()
    //{
    //}

    public TcpSocketTransport(IStreamResource streamResource)
    {
        _streamResource = streamResource ?? throw new ArgumentNullException(nameof(streamResource));
    }

    public int WaitToRetryMilliseconds
    {
        get => _waitToRetryMilliseconds;
        set
        {
            if (value < 0)
            {
                throw new ArgumentException(Resources.WaitRetryGreaterThanZero);
            }

            _waitToRetryMilliseconds = value;
        }
    }
    public int ReadTimeout
    {
        get => StreamResource.ReadTimeout;
        set => StreamResource.ReadTimeout = value;
    }
    public int WriteTimeout
    {
        get => StreamResource.WriteTimeout;
        set => StreamResource.WriteTimeout = value;
    }

    public IStreamResource StreamResource => _streamResource;

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private byte[] CreateAckPacket(ushort requestCode, byte transactionId)
    {
        DateTime now = DateTime.Now;
        UInt16 packetLength = KdmsCodeInfo.HmiPacketHeaderSize + KdmsCodeInfo.HmiDataHeaderSize;
        TcpPacketHeader tcpPacketHeader = new TcpPacketHeader
        {
            ucNodeCode = (byte)eNodeCode.nc_hmi_comm,
            ucActCode = (byte)eActionCode.ack_packet,
            usTotPktCnt = 1,
            usPktIdx = 1,
            ucSeq = transactionId,
            ucComp = (byte)eCompress.uncompress,
            usLength = packetLength
        };
        TcpDataHeader dataHeader = new TcpDataHeader
        {
            uiTime = (uint)KdmsValueConverter.ConvertToUnixTimestamp(now),
            sReqFc = requestCode,
            sRepFc = requestCode,
            usCount = 0,
        };
        Console.WriteLine($"PKT SND ======> NC:{tcpPacketHeader.ucNodeCode} AC:{tcpPacketHeader.ucActCode} TOT:{tcpPacketHeader.usTotPktCnt} IDX:{tcpPacketHeader.usPktIdx} SEQ:{tcpPacketHeader.ucSeq} COMP:{tcpPacketHeader.ucComp} LEN:{tcpPacketHeader.usLength}");
        Console.WriteLine($"DATA SND - (time:{now.ToString("yyyy-MM-dd HH:mm:ss")} req:0x{requestCode.ToString("X2")} res:0x{requestCode.ToString("X2")} cnt:{0})");
        //Console.WriteLine($"ACK SND => time:{dataHeader.uiTime} req:{dataHeader.sReqFc.ToString("X2")} res:{dataHeader.sRepFc.ToString("X2")} cnt:{dataHeader.usCount}");
        var packetHeader = KdmsValueConverter.StructToByte(tcpPacketHeader);
        var packetData = KdmsValueConverter.StructToByte(tcpPacketHeader);
        var packetMessage = new MemoryStream(packetLength);
        packetMessage.Write(packetHeader, 0, packetHeader.Length);
        packetMessage.Write(packetData, 0, packetData.Length);
        return packetMessage.ToArray();
    }

    private byte[] CreatePacketData(UInt16 packetCount, UInt16 packetIndex, byte transactionId, byte[] packetData)
    {
        UInt16 packetLength = (UInt16)(packetData.Length + KdmsCodeInfo.HmiPacketHeaderSize);
        TcpPacketHeader tcpPacketHeader = new TcpPacketHeader
        {
            ucNodeCode = (byte)eNodeCode.nc_hmi_comm,
            ucActCode = (byte)eActionCode.rt_req,
            usTotPktCnt = packetCount,
            usPktIdx = packetIndex,
            ucSeq = transactionId,
            ucComp = (byte)eCompress.uncompress,
            usLength = packetLength
        };
        Console.WriteLine($"PKT SND ======> NC:{tcpPacketHeader.ucNodeCode} AC:{tcpPacketHeader.ucActCode} TOT:{tcpPacketHeader.usTotPktCnt} IDX:{tcpPacketHeader.usPktIdx} SEQ:{tcpPacketHeader.ucSeq} COMP:{tcpPacketHeader.ucComp} LEN:{tcpPacketHeader.usLength}");
        var packetHeader = KdmsValueConverter.StructToByte(tcpPacketHeader);
        var packetMessage = new MemoryStream(packetLength);
        packetMessage.Write(packetHeader, 0, packetHeader.Length);
        packetMessage.Write(packetData, 0, packetData.Length);
        return packetMessage.ToArray();
    }

    public virtual void Write(ITcpSocketMessage message)
    {
        try
        {
            lock (_syncLock)
            {
                byte[] sendData = message.SendDatas;

                var sendDatas = sendData.Split(KdmsCodeInfo.HmiPacketDataSize);
                UInt16 packetIndex = 0;
                UInt16 packetCount = (UInt16)sendDatas.Count();
                int sendCount = 0;
                foreach (var data in sendDatas)
                {                    
                    byte transactionId = GetNewTransactionId();
                    byte[] frame = CreatePacketData(packetCount, ++packetIndex, transactionId, sendData);
                    StreamResource.Write(frame, 0, frame.Length);
                    if(sendCount++ == 0)
                    {
                        Console.WriteLine($"DATA SND - (time:{message.SendTime.ToString("yyyy-MM-dd HH:mm:ss")} req:0x{message.RequestCode.ToString("X2")} res:0x{message.ResponseCode.ToString("X2")} cnt:{message.DataCount})");
                    }
                    // ACK 수신
                    //Sleep(WaitToRetryMilliseconds);
                    byte[] ackPacket = Read(out bool isCompress);
                    var packetHeader = KdmsValueConverter.ByteToStruct<TcpDataHeader>(ackPacket);
                    //if (packetHeader.ucSeq != transactionId || packetHeader.ucActCode != (byte)eActionCode.ack_packet)
                    {
                        // ACK 예외발생(종료해야할듯)
                    }
                }
            }
        }
        catch (Exception e)
        {
            if ((e is SocketException socketException && socketException.SocketErrorCode != SocketError.TimedOut)
                || (e.InnerException is SocketException innerSocketException && innerSocketException.SocketErrorCode != SocketError.TimedOut))
            {
                throw;
            }

            Sleep(WaitToRetryMilliseconds);
            throw;
        }
    }


    public virtual byte[] Read(out bool isCompress)
    {
        isCompress = false;
        if (StreamResource == null) throw new ArgumentNullException(nameof(StreamResource));

        byte[] dataTotalFrame = null!;
        //KdmsDataResponse kdmsDataResponse = null!;
        lock (_syncLock)
        {
            ushort requestCode = 0;
            do
            {
                var tcpHeader = new byte[KdmsCodeInfo.HmiPacketHeaderSize];
                int numBytesRead = 0;
                while (numBytesRead != KdmsCodeInfo.HmiPacketHeaderSize) // 패킷 헤더 수신
                {
                    int bRead = StreamResource.Read(tcpHeader, numBytesRead, KdmsCodeInfo.HmiPacketHeaderSize - numBytesRead);

                    if (bRead == 0)
                    {
                        throw new IOException("Read resulted in 0 bytes returned.");
                    }

                    numBytesRead += bRead;
                }

                var packetHeader = KdmsValueConverter.ByteToStruct<TcpPacketHeader>(tcpHeader);
                Console.WriteLine($"PKT RCV <====== NC:{packetHeader.ucNodeCode} AC:{packetHeader.ucActCode} TOT:{packetHeader.usTotPktCnt} IDX:{packetHeader.usPktIdx} SEQ:{packetHeader.ucSeq} COMP:{packetHeader.ucComp} LEN:{packetHeader.usLength}");

                int DataReadSize = packetHeader.usLength - numBytesRead;

                if (packetHeader.usPktIdx == 1)
                {
                    isCompress = packetHeader.ucComp == 1? true : false;
                    numBytesRead = 0;
                    byte[] dataHeaderFrame = new byte[KdmsCodeInfo.HmiDataHeaderSize];
                    while (numBytesRead != KdmsCodeInfo.HmiDataHeaderSize)
                    {
                        int bRead = StreamResource.Read(dataHeaderFrame, numBytesRead, KdmsCodeInfo.HmiDataHeaderSize - numBytesRead);

                        if (bRead == 0)
                        {
                            throw new IOException("Read resulted in 0 bytes returned.");
                        }

                        numBytesRead += bRead;
                    }
                    dataTotalFrame = dataHeaderFrame;
                    DataReadSize = DataReadSize - numBytesRead;
                    var recvTime = BitConverter.ToUInt32(dataHeaderFrame, 0);
                    requestCode = BitConverter.ToUInt16(dataHeaderFrame, 4);
                    var responseCode = BitConverter.ToUInt16(dataHeaderFrame, 6);
                    var dataCount = BitConverter.ToUInt32(dataHeaderFrame, 8);
                    Console.WriteLine($"DATA RCV - (time:{KdmsValueConverter.TimeTToDateTime(recvTime).ToString("yyyy-MM-dd HH:mm:ss")} req:0x{requestCode.ToString("X2")} res:0x{responseCode.ToString("X2")} cnt:{dataCount})");

                }

                byte[] dataFrame = null!;
                if (DataReadSize > 0)
                {
                    numBytesRead = 0;
                    dataFrame = new byte[DataReadSize];
                    while (numBytesRead != DataReadSize)
                    {
                        int bRead = StreamResource.Read(dataFrame, numBytesRead, DataReadSize - numBytesRead);

                        if (bRead == 0)
                        {
                            throw new IOException("Read resulted in 0 bytes returned.");
                        }

                        numBytesRead += bRead;
                    }
                }

                if (packetHeader.ucActCode == (byte)eActionCode.ack_packet)
                {
                    // ACK 확인
                    var dataHeader = KdmsValueConverter.ByteToStruct<TcpDataHeader>(dataTotalFrame);
                    // Console.WriteLine($"ACK RCV <= time:{dataHeader.uiTime} req:{dataHeader.sReqFc.ToString("X2")} res:{dataHeader.sRepFc.ToString("X2")} cnt:{dataHeader.usCount}");
                }
                else
                {
                    // ACK PACKET 전송
                    var ackPacket = CreateAckPacket(requestCode, packetHeader.ucSeq);
                    StreamResource.Write(ackPacket, 0, ackPacket.Length);
                }
                if (dataFrame != null)
                    dataTotalFrame = dataTotalFrame.Concat(dataFrame).ToArray();
                if (packetHeader.usTotPktCnt <= packetHeader.usPktIdx)
                    break;
            } while (true);
        }

        return dataTotalFrame;
    }

    //public abstract void Write(ITcpSocketMessage message);
    public abstract byte GetNewTransactionId();

    //public abstract byte[] Read();
    //public abstract byte[] ResponseData(ITcpSocketMessage message);
    public abstract ITcpSocketMessage ResponseData(ITcpSocketMessage message);
    public abstract ITcpSocketMessage ResponseData();

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            DisposableUtility.Dispose(ref _streamResource);
        }
    }

    private static void Sleep(int millisecondsTimeout)
    {
        Task.Delay(millisecondsTimeout).Wait();
    }

}
