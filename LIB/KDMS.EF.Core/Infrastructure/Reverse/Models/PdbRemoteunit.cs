using System;
using System.Collections.Generic;

namespace KDMS.EF.Core.Infrastructure.Reverse.Models;

/// <summary>
/// RTU 정보 테이블
/// </summary>
public partial class PdbRemoteunit
{
    /// <summary>
    /// FRTU/FIED ID
    /// </summary>
    public long Pid { get; set; }

    /// <summary>
    /// DMC Point ID 
    /// </summary>
    public long? DmcFk { get; set; }

    /// <summary>
    /// DMC Point ID
    /// </summary>
    public long? CommDmcFk { get; set; }

    /// <summary>
    /// CID FILE ID
    /// </summary>
    public long? CidFk { get; set; }

    /// <summary>
    /// Prime Channel ID
    /// </summary>
    public long? ChannelPrimary { get; set; }

    /// <summary>
    /// Backup Channel ID
    /// </summary>
    public long? ChannelAlternate { get; set; }

    /// <summary>
    /// Prime DCP ID
    /// </summary>
    public long? DcpPrimeFk { get; set; }

    /// <summary>
    /// Backup DCP ID
    /// </summary>
    public long? DcpBackupFk { get; set; }

    /// <summary>
    /// RTU Scan Intrval(AI,AO,BI,BO,Counter,EMS)
    /// </summary>
    public long? SbFk { get; set; }

    /// <summary>
    /// Use RTU Protocol ID
    /// </summary>
    public long? ProtocolFk { get; set; }

    /// <summary>
    /// Protocol Name
    /// </summary>
    public string? ProtocolName { get; set; }

    /// <summary>
    /// Use RTU Coummunication Type
    /// </summary>
    public long? CommType { get; set; }

    /// <summary>
    /// Coummunication Name
    /// </summary>
    public string? CommInfo { get; set; }

    /// <summary>
    /// RTU Maker ID
    /// </summary>
    public long? RtuMaker { get; set; }

    /// <summary>
    /// RTU Maker Name
    /// </summary>
    public string? RtuCompany { get; set; }

    /// <summary>
    /// Equipment Maker ID
    /// </summary>
    public long? EqMaker { get; set; }

    /// <summary>
    /// Equipment Maker Name
    /// </summary>
    public string? EqCompany { get; set; }

    /// <summary>
    /// Equipment ID(CEQID, CPSID, MTRID, MTR Bank ID)
    /// </summary>
    public long? EqFk { get; set; }

    /// <summary>
    /// Equipment Type CEQ, CPS, MTR)
    /// </summary>
    public int? EqType { get; set; }

    /// <summary>
    /// Use RTU Map ID
    /// </summary>
    public int? RtuMapFk { get; set; }

    /// <summary>
    /// RTU Type(Not Connected RTU, FRTU, FIED)
    /// </summary>
    public int? RtuType { get; set; }

    /// <summary>
    /// RTU Name 
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// DCP Communication Master Address(DCP)
    /// </summary>
    public int? MasterAddr { get; set; }

    /// <summary>
    /// RTU Communication Slave Address(단말장치)
    /// </summary>
    public int? SlaveAddr { get; set; }

    /// <summary>
    /// Use Comnication Confirm 
    /// </summary>
    public int? Confirm { get; set; }

    /// <summary>
    /// Data Link TimeOut
    /// </summary>
    public int? DlTimeout { get; set; }

    /// <summary>
    /// Application Link TimeOut
    /// </summary>
    public int? AppTimeout { get; set; }

    /// <summary>
    /// Failed Retry Count
    /// </summary>
    public int? Retry { get; set; }

    /// <summary>
    /// RTU SeralNo
    /// </summary>
    public string? RtuSeralno { get; set; }

    /// <summary>
    /// RTU 제작일자 
    /// </summary>
    public string? RtuMakeDate { get; set; }

    /// <summary>
    /// RTU 설치일자
    /// </summary>
    public string? RtuInstallDate { get; set; }

    /// <summary>
    /// RTU Version
    /// </summary>
    public string? RtuVersion { get; set; }

    /// <summary>
    /// Equipment SeralNo
    /// </summary>
    public string? EqSerialno { get; set; }

    /// <summary>
    /// Equipment 제작일자
    /// </summary>
    public string? EqMakeDate { get; set; }

    /// <summary>
    /// Equipment 설치일자
    /// </summary>
    public string? EqInstallDate { get; set; }

    /// <summary>
    /// Equipment 설치자
    /// </summary>
    public string? EqInstallManager { get; set; }

    /// <summary>
    /// FIED NAME
    /// </summary>
    public string? FiedName { get; set; }

    /// <summary>
    /// Operration Use
    /// </summary>
    public int? UseAoper { get; set; }

    /// <summary>
    /// 60870_1
    /// </summary>
    public int? LinkAddrSize { get; set; }

    /// <summary>
    /// 60870_2
    /// </summary>
    public int? CotSize { get; set; }

    /// <summary>
    /// 60870_3
    /// </summary>
    public int? AsduAddrSize { get; set; }

    /// <summary>
    /// 60870_4
    /// </summary>
    public int? ObjectAddrSize { get; set; }

    /// <summary>
    /// Wave 처리 유형(DNP, FTP)	
    /// </summary>
    public long? WaveCommTypeFk { get; set; }
}
