using System;
using System.Runtime.InteropServices;

namespace KdmsTcpSocket.KdmsTcpStruct;


[Serializable]
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct TcpPacketHeader
{
    public byte ucNodeCode;
    public byte ucActCode;
    public UInt16 usTotPktCnt;
    public UInt16 usPktIdx;
    public byte ucSeq;
    public byte ucComp;
    public UInt16 usLength;

    public int GetSize()
    {
        return Marshal.SizeOf(this);
    }
}

[Serializable]
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct TcpDataHeader
{
    public UInt32 uiTime;
    public UInt16 sReqFc;
    public UInt16 sRepFc;
    public UInt32 usCount;

    public int GetSize()
    {
        return Marshal.SizeOf(this);
    }
}



