using KdmsTcpSocket.Interfaces;
using KdmsTcpSocket.KdmsTcpStruct;
using KdmsTcpSocket.Message;
using KdmsTcpSocket.Unme.Common;
using System.IO.Compression;

namespace KdmsTcpSocket.IO
{
    public class TcpSocketIpTransport : TcpSocketTransport
    {
        private static readonly object _transactionIdLock = new object();
        private byte _transactionId;

        public TcpSocketIpTransport(IStreamResource streamResource)
            : base(streamResource)
        {
            if (streamResource == null) throw new ArgumentNullException(nameof(streamResource));

            // 타임아웃 설정함.
            //ReadTimeout = TcpSocket.ReadTimeOut;
            //ReadTimeout = TcpSocket.WriteTimeOut;
        }

        /// <summary>
        ///     Create a new transaction ID.
        /// </summary>
        public override byte GetNewTransactionId()
        {
            lock (_transactionIdLock)
            {
                _transactionId = _transactionId == byte.MaxValue ? (byte)1 : ++_transactionId;
            }

            return _transactionId;
        }

        public override ITcpSocketMessage ResponseData(ITcpSocketMessage message)
        {
            Write(message);            
            return ResponseData();
        }

        public override ITcpSocketMessage ResponseData()
        {
            byte[] recvDatas = Read(out bool isCompress);
            ITcpSocketResponse tcpSocketResponse = null!;

            if (recvDatas.Length >= KdmsCodeInfo.HmiDataHeaderSize)
            {
                UInt32 uiTime = BitConverter.ToUInt32(recvDatas, 0);
                UInt16 sReqFc = BitConverter.ToUInt16(recvDatas, 4);
                UInt16 sRepFc = BitConverter.ToUInt16(recvDatas, 6);
                UInt32 usCount = BitConverter.ToUInt32(recvDatas, 8);
                tcpSocketResponse = new KdmsDataResponse(sReqFc, sRepFc, usCount, uiTime);
                if (isCompress)
                {
                    int sliceSize = KdmsCodeInfo.HmiDataHeaderSize + 4;
                    byte[] compressedData = recvDatas.Slice(sliceSize
                    , recvDatas.Length - sliceSize).ToArray();

                    tcpSocketResponse.RecvDatas = CompressUtility.DecompressUsingZlib(compressedData);
                }
                else
                {
                    tcpSocketResponse.RecvDatas = recvDatas.Slice(KdmsCodeInfo.HmiDataHeaderSize
                    , recvDatas.Length - KdmsCodeInfo.HmiDataHeaderSize).ToArray();
                }
            }

            return tcpSocketResponse;
        }
    }
}
