using KdmsTcpSocket.Interfaces;
using KdmsTcpSocket.KdmsTcpStruct;
using System;
using System.IO;
using System.Net;

namespace KdmsTcpSocket.Message
{
    public class KdmsDataRequest<T> : ITcpSocketRequest where T : struct
    {
        public KdmsDataRequest(ushort requestCode, ushort responseCode, T? data)
        {
            RequestCode = requestCode;
            ResponseCode = responseCode;
            DataCount = (uint)(data == null ? 0 : 1);
            SendTime = DateTime.Now;
            SendData = data;
        }

        public T? SendData { get; set; }
        public DateTime SendTime { get; set; }
        public ushort RequestCode { get; set; }
        public ushort ResponseCode { get; set; }
        public uint DataCount { get; set; }
        public byte[]? RecvDatas { get; set; } = null;

        // public override int MinimumFrameSize => 12;

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
                byte[]? data = null;
                int dataLen = 0;
                if(SendData is not null)
                {
                    data = KdmsValueConverter.StructToByte(SendData.Value);
                    dataLen = data.Length;
                }
                //var data = KdmsValueConverter.StructToByte(loginInfo);
                var frame = new MemoryStream(dataHeader.Length + dataLen);
                frame.Write(dataHeader, 0, dataHeader.Length);
                
                if(data is not null)
                    frame.Write(data, 0, data.Length);

                return frame.ToArray();
            }
        }


        public override string ToString()
        {
            string msg = $"REQ:{RequestCode} RES:{ResponseCode} LOGIN MESSAGE";
            return msg;
        }
    }
}
