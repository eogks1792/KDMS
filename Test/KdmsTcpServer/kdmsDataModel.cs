using System.Runtime.InteropServices;

namespace KdmsTcpServer;


[Serializable]
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct TcpNoData
{
    public int GetSize()
    {
        return Marshal.SizeOf(this);
    }
}


[Serializable]
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct OperLogReq
{
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
    public string szUserId;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
    public string szUserPw;

    public int GetSize()
    {
        return Marshal.SizeOf(this);
    }
}

[Serializable]
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct OperLogRes
{
    public UInt16 usSt;
    public UInt16 usRes;

    public int GetSize()
    {
        return Marshal.SizeOf(this);
    }
}



[Serializable]
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PdbListRes
{
    public Int32 iPdbId;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
    public string szPdbName;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 36)]
    public string szPdbMd5;

    public int GetSize()
    {
        return Marshal.SizeOf(this);
    }
}

[Serializable]
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PdbDataReqs
{
    public Int32 iPdbId;

    public int GetSize()
    {
        return Marshal.SizeOf(this);
    }
}
