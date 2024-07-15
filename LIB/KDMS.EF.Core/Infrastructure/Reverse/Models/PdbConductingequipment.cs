using System;
using System.Collections.Generic;

namespace KDMS.EF.Core.Infrastructure.Reverse.Models;

/// <summary>
/// 설비 정보 저장 테이블
/// </summary>
public partial class PdbConductingequipment
{
    /// <summary>
    /// ConductionEquipment  ID 
    /// </summary>
    public long Ceqid { get; set; }

    /// <summary>
    /// Equipment ID
    /// </summary>
    public long? EqFk { get; set; }

    /// <summary>
    /// Equipment Container ID
    /// </summary>
    public long? EcFk { get; set; }

    /// <summary>
    /// SubStation ID
    /// </summary>
    public long? StFk { get; set; }

    /// <summary>
    /// Distribution Line ID 
    /// </summary>
    public long? DlFk { get; set; }

    /// <summary>
    /// Link SubStation ID
    /// </summary>
    public long? LinkStFk { get; set; }

    /// <summary>
    /// Link Distribution Line ID
    /// </summary>
    public long? LinkDlFk { get; set; }

    /// <summary>
    /// PowerSystemResource Type ID
    /// </summary>
    public int? Psrtype { get; set; }

    /// <summary>
    /// Equipment Name 
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Equipment Descrption
    /// </summary>
    public string? Desc { get; set; }

    /// <summary>
    /// Equipment Container Name
    /// </summary>
    public string? EcName { get; set; }

    /// <summary>
    /// Equipment Mesh Number
    /// </summary>
    public string? MeshNo { get; set; }

    /// <summary>
    /// 단말/상시연계 상태 표시
    /// </summary>
    public long? SwType { get; set; }

    /// <summary>
    /// Equipment RTU Type
    /// </summary>
    public int? RtuType { get; set; }

    /// <summary>
    /// Equipment Device Number
    /// </summary>
    public string? DevNo { get; set; }

    /// <summary>
    /// distributionTransformer ID
    /// </summary>
    public long? DtrFk { get; set; }

    /// <summary>
    /// PowerTransformer ID
    /// </summary>
    public long? PtrFk { get; set; }

    /// <summary>
    /// Link Breaker ID
    /// </summary>
    public long? LinkCbFk { get; set; }

    /// <summary>
    /// Link BusBarSection ID
    /// </summary>
    public long? LinkBbsFk { get; set; }

    /// <summary>
    /// 기준 전압
    /// </summary>
    public float? BaseVoltage { get; set; }

    /// <summary>
    /// 상연결정보
    /// </summary>
    public int? Phases { get; set; }

    /// <summary>
    /// Busbarsection Order
    /// </summary>
    public int? BusbarOrder { get; set; }

    /// <summary>
    /// Analog Input Point Count
    /// </summary>
    public int? AiCnt { get; set; }

    /// <summary>
    /// Start Analog Point ID
    /// </summary>
    public long? AiPid { get; set; }

    /// <summary>
    /// Analog Output Point Count
    /// </summary>
    public int? AoCnt { get; set; }

    /// <summary>
    /// Start SetPoint ID
    /// </summary>
    public long? AoPid { get; set; }

    /// <summary>
    /// Binary Input Point Count
    /// </summary>
    public int? BiCnt { get; set; }

    /// <summary>
    /// Start Discrete Point ID
    /// </summary>
    public long? BiPid { get; set; }

    /// <summary>
    /// Counter Point Count
    /// </summary>
    public int? PiCnt { get; set; }

    /// <summary>
    /// Start Accumulator Point ID
    /// </summary>
    public long? PiPid { get; set; }
}
