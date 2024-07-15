using System;
using System.Collections.Generic;

namespace KDMS.EF.Core.Infrastructure.Reverse.Models;

/// <summary>
/// 보관주기 타입 테이블
/// </summary>
public partial class StorageType
{
    /// <summary>
    /// 보관주기 타입
    /// </summary>
    public int StorageType1 { get; set; }

    /// <summary>
    /// 타입 이름
    /// </summary>
    public string? Name { get; set; }
}
