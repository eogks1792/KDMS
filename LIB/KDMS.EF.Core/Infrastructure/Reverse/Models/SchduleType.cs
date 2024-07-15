using System;
using System.Collections.Generic;

namespace KDMS.EF.Core.Infrastructure.Reverse.Models;

/// <summary>
/// 스케줄 설정 타입 테이블
/// </summary>
public partial class SchduleType
{
    /// <summary>
    /// 스케줄 타입
    /// </summary>
    public int SchduleType1 { get; set; }

    /// <summary>
    /// 타입 이름
    /// </summary>
    public string? Name { get; set; }
}
