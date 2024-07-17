namespace KdmsTcpSocket;

public static class KdmsCodeInfo
{
    public const ushort kdmsOperLoginReqs = (0x0101); // FC_OPER_LOGIN_REQS
    public const ushort KdmsOperLoginReps = (0x0102); // FC_OPER_LOGIN_REPS

    public const ushort KdmsOperLogoutReqs = (0x0103); // FC_OPER_LOGOUT_REQS
    public const ushort KdmsOperLogoutReps = (0x0104); // FC_OPER_LOGOUT_REPS

    public const ushort KdmsPdbListReqs = (0x2001);     // FC_PDB_LIST_REQS
    public const ushort KdmsPdbListReps = (0x2002);     // FC_PDB_LIST_REPS
    public const ushort KdmsPdbSyncReqs = (0x2003);     // FC_PDB_SYNC_REQS
    public const ushort KdmsPdbSyncStart = (0x2004);    // FC_PDB_SYNC_START
    public const ushort KdmsPdbSyncEnd = (0x2005);      // FC_PDB_SYNC_END
    public const ushort KdmsPdbSyncComp = (0x2006);     // FC_PDB_SYNC_COMPLET

    public const ushort KdmsRtdbListReqs = (0x2101);  // FC_RTDB_LIST_REQS
    public const ushort KdmsRtdbListReps = (0x2102);  // FC_RTDB_LIST_REPS

    public const ushort KdmsEventDataReps = (0x9001);  // FC_EVT_SYNC

    //public const ushort KdmsRtDMCReqs = (0x2201);  // FC_RTDMC_GET_REQ
    //public const ushort KdmsRtDMCReps = (0x2202);  // FC_RTDMC_GET_RES

    public const ushort KdmsRtBIReqs = (0x8001);  // FC_RT_BI_REQS
    public const ushort KdmsRtBOReqs = (0x8002);  // FC_RT_BO_REQS
    public const ushort KdmsRtAIReqs = (0x8003);  // FC_RT_AI_REQS
    public const ushort KdmsRtAOReqs = (0x8004);  // FC_RT_AO_REQS
    public const ushort KdmsRtPIReqs = (0x8005);  // FC_RT_PI_REQS
    public const ushort KdmsRtDMCReqs = (0x8006);  // FC_RT_DMC_REQS
    public const ushort KdmsRtRTUReqs = (0x8007);  // FC_RT_RTU_REQS

    public const ushort KdmsRtBIReps = (0x8011);  // FC_RT_DATA_BI
    public const ushort KdmsRtBOReps = (0x8012);  // FC_RT_DATA_BO
    public const ushort KdmsRtAIReps = (0x8013);  // FC_RT_DATA_AI
    public const ushort KdmsRtAOReps = (0x8014);  // FC_RT_DATA_AO
    public const ushort KdmsRtPIReps = (0x8015);  // FC_RT_DATA_PI
    public const ushort KdmsRtDMCReps = (0x8016);  // FC_RT_DATA_DMC
    public const ushort KdmsRtRTUReps = (0x8019);  // FC_RT_DATA_RTU

    public const ushort KdmsAlarmEvent = (0x8101); // FC_AE_UPDATE

    //public const ushort KdmsScanCmdBIReqs = (0x4001);  // FC_SCAN_BI
    //public const ushort KdmsScanCmdBOReqs = (0x4002);  // FC_SCAN_BO
    //public const ushort KdmsScanCmdAIReqs = (0x4003);  // FC_SCAN_AI
    //public const ushort KdmsScanCmdAOReqs = (0x4004);  // FC_SCAN_AO
    //public const ushort KdmsScanCmdPIReqs = (0x4005);  // FC_SCAN_PI
    //public const ushort KdmsScanCmdReqs = (0x4101);    // FC_SCAN_ALL





    public const int HmiPacketSize = 4096;
    public const int HmiPacketHeaderSize = 10;
    public const int HmiDataHeaderSize = 12;
    public const int HmiPacketDataSize = HmiPacketSize - HmiPacketHeaderSize;
}
