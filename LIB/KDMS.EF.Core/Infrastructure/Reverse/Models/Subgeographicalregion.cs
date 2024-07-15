using System;
using System.Collections.Generic;

namespace KDMS.EF.Core.Infrastructure.Reverse.Models;

/// <summary>
/// 지점 테이블
/// </summary>
public partial class Subgeographicalregion
{
    /// <summary>
    /// 지점 아이디
    /// </summary>
    public long Sgrid { get; set; }

    /// <summary>
    /// GEOGRAPHICALREGION의 GGRID
    /// </summary>
    public int? GgrFk { get; set; }

    /// <summary>
    /// 지점 이름
    /// </summary>
    public string? Name { get; set; }
}
