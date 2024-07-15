using System;
using System.Collections.Generic;

namespace KDMS.EF.Core.Infrastructure.Reverse.Models;

/// <summary>
/// 지사 테이블
/// </summary>
public partial class Geographicalregion
{
    /// <summary>
    /// 지사 아이디
    /// </summary>
    public int Ggrid { get; set; }

    /// <summary>
    /// 지사 이름
    /// </summary>
    public string? Name { get; set; }
}
