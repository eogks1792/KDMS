using KdmsTcpSocket.KdmsTcpStruct;

namespace KdmsTcpSocket.Message;

public class KdmsPdbDataResponse : KdmsDataResponse
{
    public KdmsPdbDataResponse(ushort requestCode, ushort responseCode, uint dataCount, uint sendTime)
        : base(requestCode, responseCode, dataCount, sendTime)
    {
        RequestCode = requestCode;
        ResponseCode = responseCode;
        DataCount = dataCount;
        SendTime = KdmsValueConverter.TimeTToDateTime(sendTime);
    }

    public int PdbId { get; set; }
}

public class KdmsDataResponse : ITcpSocketResponse
{
    public KdmsDataResponse(ushort requestCode, ushort responseCode, uint dataCount, uint sendTime)
    {
        RequestCode = requestCode;
        ResponseCode = responseCode;
        DataCount = dataCount;
        SendTime = KdmsValueConverter.TimeTToDateTime(sendTime);
    }

    public DateTime SendTime { get; set; }
    public ushort RequestCode { get; set; }
    public ushort ResponseCode { get; set; }
    public uint DataCount { get; set; }
    public byte[]? RecvDatas { get; set; } = null;

    public byte[] DataHeader
    {
        get
        {
            TcpDataHeader tcpDataHeader = new TcpDataHeader
            {
                uiTime = (uint)KdmsValueConverter.ConvertToUnixTimestamp(SendTime),
                sReqFc = RequestCode,
                sRepFc = RequestCode,
                usCount = DataCount,
            };

            return KdmsValueConverter.StructToByte(tcpDataHeader);
        }
    }

    public byte[] SendDatas
    {
        get
        {
            var dataHeader = DataHeader;
            var frame = new MemoryStream(dataHeader.Length);
            frame.Write(dataHeader, 0, dataHeader.Length);
            return frame.ToArray();
        }
    }


    public override string ToString()
    {
        string msg = $"REQ:{RequestCode} RES:{ResponseCode} MESSAGE";
        return msg;
    }
}
