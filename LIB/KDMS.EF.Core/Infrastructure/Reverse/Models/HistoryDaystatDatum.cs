using System;
using System.Collections.Generic;

namespace KDMS.EF.Core.Infrastructure.Reverse.Models;

/// <summary>
/// 시간단위 통계 테이블
/// </summary>
public partial class HistoryDaystatDatum
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
    /// 전류A-평균
    /// </summary>
    public float? AverageCurrentA { get; set; }

    /// <summary>
    /// 전류B-평균
    /// </summary>
    public float? AverageCurrentB { get; set; }

    /// <summary>
    /// 전류C-평균
    /// </summary>
    public float? AverageCurrentC { get; set; }

    /// <summary>
    /// 전류N-평균
    /// </summary>
    public float? AverageCurrentN { get; set; }

    /// <summary>
    /// 전류A-최대
    /// </summary>
    public float? MaxCurrentA { get; set; }

    /// <summary>
    /// 전류B-최대
    /// </summary>
    public float? MaxCurrentB { get; set; }

    /// <summary>
    /// 전류C-최대
    /// </summary>
    public float? MaxCurrentC { get; set; }

    /// <summary>
    /// 전류N-최대
    /// </summary>
    public float? MaxCurrentN { get; set; }

    /// <summary>
    /// 최대 수집시간
    /// </summary>
    public DateTime? MaxCommTime { get; set; }

    /// <summary>
    /// 전류A - 최소
    /// </summary>
    public float? MinCurrentA { get; set; }

    /// <summary>
    /// 전류B - 최소
    /// </summary>
    public float? MinCurrentB { get; set; }

    /// <summary>
    /// 전류C - 최소
    /// </summary>
    public float? MinCurrentC { get; set; }

    /// <summary>
    /// 전류N - 최소
    /// </summary>
    public float? MinCurrentN { get; set; }

    /// <summary>
    /// 최소 수집시간
    /// </summary>
    public DateTime? MinCommTime { get; set; }
}
