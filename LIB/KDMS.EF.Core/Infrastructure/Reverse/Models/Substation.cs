using System;
using System.Collections.Generic;

namespace KDMS.EF.Core.Infrastructure.Reverse.Models;

/// <summary>
/// 변전소 정보 테이블
/// </summary>
public partial class Substation
{
    /// <summary>
    /// 변전소 ID
    /// </summary>
    public long Stid { get; set; }

    /// <summary>
    /// 소속 지점 ID
    /// </summary>
    public long? SgrFk { get; set; }

    /// <summary>
    /// 변전소 명
    /// </summary>
    public string? Name { get; set; }
}
