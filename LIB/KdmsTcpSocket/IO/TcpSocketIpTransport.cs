﻿using KdmsTcpSocket.Interfaces;
using KdmsTcpSocket.Message;
using KdmsTcpSocket.Unme.Common;

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
            //WriteTimeout = TcpSocket.WriteTimeOut;
        }

        /// <summary>
        ///     Create a new transaction ID.
        /// </summary>
        public override byte GetNewTransactionId()
        {
            lock (_transactionIdLock)
            {
                _transactionId = _transactionId == byte.MaxValue ? (byte)1 : ++_transactionId;
                //_transactionId = ++_transactionId;
            }

            return _transactionId;
        }

        public override void NoResponseData(byte actionCode, ushort requestCode)
        {
            NoResponsePacketData(actionCode, requestCode);
        }

        public override ITcpSocketMessage ResponseData(ITcpSocketMessage message)
        {
            Write(message);
            return ResponseData();
        }


        public override ITcpSocketMessage ResponseData()
        {
            byte[] recvDatas = Read(out bool isCompress);
            ITcpSocketResponse responseData = null!;

            if (recvDatas.Length >= KdmsCodeInfo.HmiDataHeaderSize)
            {
                UInt32 uiTime = BitConverter.ToUInt32(recvDatas, 0);
                UInt16 sReqFc = BitConverter.ToUInt16(recvDatas, 4);
                UInt16 sRepFc = BitConverter.ToUInt16(recvDatas, 6);
                UInt32 usCount = BitConverter.ToUInt32(recvDatas, 8);

                if (usCount > 0)
                {
                    if (isCompress)
                    {
                        int sliceSize = KdmsCodeInfo.HmiDataHeaderSize + 4;
                        if (sRepFc == KdmsCodeInfo.KdmsPdbSyncStart)
                        {
                            sliceSize += 4;
                            responseData = new KdmsPdbDataResponse(sReqFc, sRepFc, usCount, uiTime);

                            int pdbId = BitConverter.ToInt32(recvDatas, 12);
                            (responseData as KdmsPdbDataResponse).PdbId = pdbId;

                            byte[] compressedData = recvDatas.Slice(sliceSize
                                , recvDatas.Length - sliceSize).ToArray();

                            responseData.RecvDatas = CompressUtility.DecompressUsingZlib(compressedData);
                        }
                        else
                        {
                            responseData = new KdmsDataResponse(sReqFc, sRepFc, usCount, uiTime);
                            byte[] compressedData = recvDatas.Slice(sliceSize
                                    , recvDatas.Length - sliceSize).ToArray();

                            responseData.RecvDatas = CompressUtility.DecompressUsingZlib(compressedData);
                        }
                    }
                    else
                    {
                        responseData = new KdmsDataResponse(sReqFc, sRepFc, usCount, uiTime);
                        responseData.RecvDatas = recvDatas.Slice(KdmsCodeInfo.HmiDataHeaderSize
                        , recvDatas.Length - KdmsCodeInfo.HmiDataHeaderSize).ToArray();
                    }
                }
                else
                {
                    responseData = new KdmsDataResponse(sReqFc, sRepFc, usCount, uiTime);
                }
            }

            return responseData;
        }

        public override byte NodeCode { get; set; } = (byte)eNodeCode.nc_hmi_comm;

    }
}
