using System;
using System.Collections.Generic;

namespace KDMS.EF.Core.Infrastructure.Reverse.Models;

/// <summary>
/// BI 포인트 필터 테이블
/// </summary>
public partial class BiInfo
{
    /// <summary>
    /// 포인트 아이디
    /// </summary>
    public int PointId { get; set; }

    /// <summary>
    /// 포인트 이름
    /// </summary>
    public string PointName { get; set; } = null!;

    /// <summary>
    /// Alarm Catagory ID
    /// </summary>
    public int? Alarmcategoryfk { get; set; }

    /// <summary>
    /// 포인트 사용여부
    /// </summary>
    public bool UseYn { get; set; }
}
