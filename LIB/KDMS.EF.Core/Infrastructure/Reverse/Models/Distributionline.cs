using System;
using System.Collections.Generic;

namespace KDMS.EF.Core.Infrastructure.Reverse.Models;

/// <summary>
/// DL 정보 테이블
/// </summary>
public partial class Distributionline
{
    /// <summary>
    /// DL ID
    /// </summary>
    public long Dlid { get; set; }

    /// <summary>
    /// 변전소 ID
    /// </summary>
    public long? StFk { get; set; }

    /// <summary>
    /// MTR ID
    /// </summary>
    public long? PtrFk { get; set; }

    /// <summary>
    /// OCB ID
    /// </summary>
    public long? SwFk { get; set; }

    /// <summary>
    /// DL 이름
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// 회선번호
    /// </summary>
    public int? DlNo { get; set; }

    /// <summary>
    /// 회선 가용량
    /// </summary>
    public int? Reliability { get; set; }

    /// <summary>
    /// 회선 우선순위
    /// </summary>
    public int? Priority { get; set; }

    /// <summary>
    /// 회선 기준용량
    /// </summary>
    public int? RatedS { get; set; }

    /// <summary>
    /// UnitSymbol FK
    /// </summary>
    public long? RatedSUsFk { get; set; }
}
