using System;
using System.Runtime.InteropServices;

namespace KdmsTcpSocket.KdmsTcpStruct;


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
public struct RtdbAnalog
{
    public Int32 pid;
    public float ucActCode;
    public Int32 usTotPktCnt;
    public Int32 usPktIdx;

    public int GetSize()
    {
        return Marshal.SizeOf(this);
    }
}


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


[Serializable]
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PdbAccessarea
{
    public UInt32 pid;              // AccessArea ID  
    public UInt32 user_fk;                       // USER ID  
    public UInt32 eq_fk;                         // Equipment ID  
    public Int32 eq_type;						// Equipment Type
    public int GetSize()
    {
        return Marshal.SizeOf(this);
    }
}

//[Serializable]
//[StructLayout(LayoutKind.Sequential, Pack = 1)]
//public struct PfdbHdr
//{
//    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
//    public string chPfName;                         /**< file name */
//    public Int32 iStatus;                            /**< file status */
//    public UInt32 stCreated;                          /**< created time */
//    public UInt32 stUpdated;                          /**< updated time */
//    public UInt32 uiPfNum;                            /**< record number */
//    public UInt32 uiPfLast;                           /**< last record */
//    public Int32 iPfLock;                            /**< file lock */
//    public Int32 ipfSize;                            /**< file size */
//    public int GetSize()
//    {
//        return Marshal.SizeOf(this);
//    }
//}


[Serializable]
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PdbInfoAnalog
{
    public UInt32 pid;                               // Discrete ID            
    public UInt32 datalink_fk;                       // DataLink ID          
    public UInt32 st_fk;                             // �Ҽ� ������ ID                
    public UInt32 rtu_fk;                        // RTU ID           
    public UInt32 ceq_fk;                            // ConductingEquipment ID
    public UInt32 dl_fk;                             // �Ҽ� D/L ID         
    public UInt32 tm_fk;                             // Terminal ID        
    public UInt32 us_fk;                         // Unit Symbol ID  
    public UInt32 dp_fk;                             // DataPointInfo ID   
    public UInt32 ac_fk;                             // Alarm Catagory ID              
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
    public string name;              // Point Name            
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
    public string aliasname;         // Point AliasName        
    public UInt32 position;                       // DNP Index     
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
    public string object_variation;    // DNP ObjectVariation
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 4)]
    public string dnp_class;            // DNP Use Class
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
    public string measurement_type;    // IED LN Code  
    public UInt32 asdu_addr;                      // ASDU Address(60870)    
    public UInt32 group_addr;                     // GROUP Address(60870)  
    public UInt32 circuit_no;                     // Circuit No
    public UInt32 use_ems;                        // Application Use         
    public UInt32 sf_fk;                         // ScaleFactor ID             
    public double sf_val;                            // ���� ���             
    public double sf_offset;                     // ���� ���  
    public UInt32 maxmin_limitset_fk;                // Limit Set ID  
    public double max_value;                     // Limit �ִ� ��         
    public UInt32 max_alarmclass_fk;             // Max AlarmClass ID
    public double min_value;                     // Limit �ּ� ��         
    public UInt32 min_alarmclass_fk;             // Min  AlarmClass ID 
    public UInt32 highlow_limitset_fk;           // Limit Set ID            
    public double high_value;                        // Limit �ִ� ��         
    public UInt32 high_alarmclass_fk;                // Max AlarmClass ID
    public double low_value;                     // Limit �ּ� ��         
    public UInt32 low_alarmclass_fk;             // Min  AlarmClass ID
    public double init_value;                        // �ʱ� ��            
    public UInt32 data_type;                      // Value Type(1:Integer, 2:float)            
    public UInt32 sw_group;                       // MSW�� LSW�� grouping        
    public UInt32 sw_type;                        // (1:MSW,2:LSW)            
    public UInt32 link_ai_fk;						// MSW��LSW ��ȣ���� POINT ID  	

    public int GetSize()
    {
        return Marshal.SizeOf(this);
    }
}

[Serializable]
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PdbAnalog
{
    public UInt32 pid;                               // Discrete ID            
    public UInt32 datalink_fk;                       // DataLink ID          
    public UInt32 st_fk;                             // �Ҽ� ������ ID                
    public UInt32 rtu_fk;                        // RTU ID           
    public UInt32 ceq_fk;                            // ConductingEquipment ID
    public UInt32 dl_fk;                             // �Ҽ� D/L ID         
    public UInt32 tm_fk;                             // Terminal ID        
    public UInt32 us_fk;                         // Unit Symbol ID  
    public UInt32 dp_fk;                             // DataPointInfo ID   
    public UInt32 ac_fk;                             // Alarm Catagory ID              
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
    public string name;              // Point Name            
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
    public string aliasname;         // Point AliasName        
    public UInt32 position;                       // DNP Index     
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
    public string object_variation;    // DNP ObjectVariation
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 4)]
    public string dnp_class;            // DNP Use Class
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
    public string measurement_type;    // IED LN Code  
    public UInt32 asdu_addr;                      // ASDU Address(60870)    
    public UInt32 group_addr;                     // GROUP Address(60870)  
    public UInt32 circuit_no;                     // Circuit No
    public UInt32 use_ems;                        // Application Use         
    public UInt32 sf_fk;                         // ScaleFactor ID             
    public double sf_val;                            // ���� ���             
    public double sf_offset;                     // ���� ���  
    public UInt32 maxmin_limitset_fk;                // Limit Set ID  
    public double max_value;                     // Limit �ִ� ��         
    public UInt32 max_alarmclass_fk;             // Max AlarmClass ID
    public double min_value;                     // Limit �ּ� ��         
    public UInt32 min_alarmclass_fk;             // Min  AlarmClass ID 
    public UInt32 highlow_limitset_fk;           // Limit Set ID            
    public double high_value;                        // Limit �ִ� ��         
    public UInt32 high_alarmclass_fk;                // Max AlarmClass ID
    public double low_value;                     // Limit �ּ� ��         
    public UInt32 low_alarmclass_fk;             // Min  AlarmClass ID
    public double init_value;                        // �ʱ� ��            
    public UInt32 data_type;                      // Value Type(1:Integer, 2:float)            
    public int GetSize()
    {
        return Marshal.SizeOf(this);
    }
}



