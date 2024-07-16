using System;
using System.Collections.Generic;

namespace KDMS.EF.Core.Infrastructure.Reverse.Models;

/// <summary>
/// 설비별 일일 통신 성공 로그 테이블
/// </summary>
public partial class HistoryCommStateLog
{
    /// <summary>
    /// DB 기록시간
    /// </summary>
    public DateTime SaveTime { get; set; }

    /// <summary>
    /// 장치 타입
    /// </summary>
    public int EqType { get; set; }

    /// <summary>
    /// 단말장치 ID
    /// </summary>
    public int Ceqid { get; set; }

    /// <summary>
    /// CompositeSwitch ID
    /// </summary>
    public int Cpsid { get; set; }

    /// <summary>
    /// 단말장치 명
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// 소속DL
    /// </summary>
    public string? Dl { get; set; }

    /// <summary>
    /// 성공/실패
    /// </summary>
    public bool? CommState { get; set; }

    /// <summary>
    /// 전체횟수
    /// </summary>
    public int? CommTotalCount { get; set; }

    /// <summary>
    /// 성공횟수
    /// </summary>
    public int? CommSucessCount { get; set; }

    /// <summary>
    /// 실패횟수
    /// </summary>
    public int? CommFailCount { get; set; }

    /// <summary>
    /// 통신 성공률
    /// </summary>
    public float? CommSucessRate { get; set; }

    /// <summary>
    /// 정보 수집시간
    /// </summary>
    public DateTime? CommTime { get; set; }
}
