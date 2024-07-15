using System;
using System.Collections.Generic;

namespace KDMS.EF.Core.Infrastructure.Reverse.Models;

/// <summary>
/// 실시간 15분 평균부하 테이블
/// </summary>
public partial class Statistics15min
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
    /// 전류A - 평균
    /// </summary>
    public float? AverageCurrentA { get; set; }

    /// <summary>
    /// 전류B - 평균
    /// </summary>
    public float? AverageCurrentB { get; set; }

    /// <summary>
    /// 전류C - 평균
    /// </summary>
    public float? AverageCurrentC { get; set; }

    /// <summary>
    /// 전류N - 평균
    /// </summary>
    public float? AverageCurrentN { get; set; }

    /// <summary>
    /// 정보 수집시간
    /// </summary>
    public DateTime CommTime { get; set; }
}
