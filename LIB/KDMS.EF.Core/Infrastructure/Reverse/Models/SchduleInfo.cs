using System;
using System.Collections.Generic;

namespace KDMS.EF.Core.Infrastructure.Reverse.Models;

/// <summary>
/// 스케줄 정보 테이블
/// </summary>
public partial class SchduleInfo
{
    /// <summary>
    /// 스케줄 아이디
    /// </summary>
    public int SchduleId { get; set; }

    /// <summary>
    /// 스케줄 설정 타입
    /// </summary>
    public int SchduleType { get; set; }

    /// <summary>
    /// 스케줄 설정 주기
    /// </summary>
    public string SchduleValue { get; set; } = null!;

    /// <summary>
    /// 스케줄 설명
    /// </summary>
    public string Desc { get; set; } = null!;
}
