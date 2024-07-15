using System;
using System.Collections.Generic;

namespace KDMS.EF.Core.Infrastructure.Reverse.Models;

/// <summary>
/// AI 포인트 필터 테이블
/// </summary>
public partial class AiInfo
{
    /// <summary>
    /// 번호
    /// </summary>
    public int No { get; set; }

    /// <summary>
    /// MIN_DATA 컬럼 이름
    /// </summary>
    public string Columnname { get; set; } = null!;

    /// <summary>
    /// DATAPOINT 아이디
    /// </summary>
    public int? Datapointid { get; set; }

    /// <summary>
    /// DATAPOINT 이름
    /// </summary>
    public string? Datapointname { get; set; }
}
