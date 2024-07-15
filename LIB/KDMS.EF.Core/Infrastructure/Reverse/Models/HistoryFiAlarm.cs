using System;
using System.Collections.Generic;

namespace KDMS.EF.Core.Infrastructure.Reverse.Models;

/// <summary>
/// FI 발생 저장 테이블
/// </summary>
public partial class HistoryFiAlarm
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
    /// 서버 기록시간
    /// </summary>
    public DateTime? LogTime { get; set; }

    /// <summary>
    /// 단말장치 발생시간
    /// </summary>
    public DateTime? FrtuTime { get; set; }

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
    /// 알람 이름
    /// </summary>
    public string? AlarmName { get; set; }

    /// <summary>
    /// 알람 값
    /// </summary>
    public float? Value { get; set; }

    /// <summary>
    /// 알람 내용
    /// </summary>
    public string? LogDesc { get; set; }

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
}
