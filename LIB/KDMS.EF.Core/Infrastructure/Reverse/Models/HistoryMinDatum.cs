using System;
using System.Collections.Generic;

namespace KDMS.EF.Core.Infrastructure.Reverse.Models;

/// <summary>
/// 실시간 저장 테이블
/// </summary>
public partial class HistoryMinDatum
{
    /// <summary>
    /// DB 기록시간
    /// </summary>
    public DateTime SaveTime { get; set; }

    /// <summary>
    /// 단말장치 ID
    /// </summary>
    public int Ceqid { get; set; }

    /// <summary>
    /// 정보 수집시간
    /// </summary>
    public DateTime? CommTime { get; set; }

    /// <summary>
    /// Composite ID
    /// </summary>
    public int? Cpsid { get; set; }

    /// <summary>
    /// 회로번호
    /// </summary>
    public int? Circuitno { get; set; }

    /// <summary>
    /// 단말장치 명
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// 소속DL
    /// </summary>
    public string? Dl { get; set; }

    /// <summary>
    /// 단말장치 상태
    /// </summary>
    public int? Diagnostics { get; set; }

    /// <summary>
    /// 전압 불평형률
    /// </summary>
    public int? VoltageUnbalance { get; set; }

    /// <summary>
    /// 전류 불평형률
    /// </summary>
    public int? CurrentUnbalance { get; set; }

    /// <summary>
    /// 주파수
    /// </summary>
    public float? Frequency { get; set; }

    /// <summary>
    /// 전류A
    /// </summary>
    public float? CurrentA { get; set; }

    /// <summary>
    /// 전류B
    /// </summary>
    public float? CurrentB { get; set; }

    /// <summary>
    /// 전류C
    /// </summary>
    public float? CurrentC { get; set; }

    /// <summary>
    /// 전류N
    /// </summary>
    public float? CurrentN { get; set; }

    /// <summary>
    /// 전압A
    /// </summary>
    public float? VoltageA { get; set; }

    /// <summary>
    /// 전압B
    /// </summary>
    public float? VoltageB { get; set; }

    /// <summary>
    /// 전압C
    /// </summary>
    public float? VoltageC { get; set; }

    /// <summary>
    /// 피상전력A
    /// </summary>
    public float? ApparentPowerA { get; set; }

    /// <summary>
    /// 피상전력B
    /// </summary>
    public float? ApparentPowerB { get; set; }

    /// <summary>
    /// 피상전력C
    /// </summary>
    public float? ApparentPowerC { get; set; }

    /// <summary>
    /// 역률3상
    /// </summary>
    public int? PowerFactor3p { get; set; }

    /// <summary>
    /// 역률A
    /// </summary>
    public int? PowerFactorA { get; set; }

    /// <summary>
    /// 역률B
    /// </summary>
    public int? PowerFactorB { get; set; }

    /// <summary>
    /// 역률C
    /// </summary>
    public int? PowerFactorC { get; set; }

    /// <summary>
    /// 고장전류A
    /// </summary>
    public float? FaultCurrentA { get; set; }

    /// <summary>
    /// 고장전류B
    /// </summary>
    public float? FaultCurrentB { get; set; }

    /// <summary>
    /// 고장전류C
    /// </summary>
    public float? FaultCurrentC { get; set; }

    /// <summary>
    /// 고장전류N
    /// </summary>
    public float? FaultCurrentN { get; set; }

    /// <summary>
    /// 전류위상A
    /// </summary>
    public float? CurrentPhaseA { get; set; }

    /// <summary>
    /// 전류위상B
    /// </summary>
    public float? CurrentPhaseB { get; set; }

    /// <summary>
    /// 전류위상C
    /// </summary>
    public float? CurrentPhaseC { get; set; }

    /// <summary>
    /// 전류위상N
    /// </summary>
    public float? CurrentPhaseN { get; set; }

    /// <summary>
    /// 전압위상A
    /// </summary>
    public float? VoltagePhaseA { get; set; }

    /// <summary>
    /// 전압위상B
    /// </summary>
    public float? VoltagePhaseB { get; set; }

    /// <summary>
    /// 전압위상C
    /// </summary>
    public float? VoltagePhaseC { get; set; }
}
