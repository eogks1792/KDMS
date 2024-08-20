using System;
using System.Collections.Generic;

namespace KDMS.EF.Core.Infrastructure.Reverse.Models;

/// <summary>
/// 보관주기 정보 테이블
/// </summary>
public partial class StorageInfo
{
    /// <summary>
    /// 보관주기 아이디
    /// </summary>
    public int StorageId { get; set; }

    /// <summary>
    /// 보관주기 설정 타입
    /// </summary>
    public int StorageType { get; set; }

    /// <summary>
    /// 보관주기 설정 주기
    /// </summary>
    public string StorageValue { get; set; } = null!;

    /// <summary>
    /// 보관주기 설명
    /// </summary>
    public string Desc { get; set; } = null!;
}
