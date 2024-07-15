using System;
using System.Collections.Generic;

namespace KDMS.EF.Core.Infrastructure.Reverse.Models;

/// <summary>
/// Composite 스위치 정보 테이블
/// </summary>
public partial class Compositeswitch
{
    /// <summary>
    /// CompositSwitch ID	
    /// </summary>
    public long Pid { get; set; }

    /// <summary>
    /// DL ID
    /// </summary>
    public long? DlFk { get; set; }

    /// <summary>
    /// PSR TYPE
    /// </summary>
    public long? Psrtype { get; set; }

    /// <summary>
    /// CompositSwitch 이름
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// 전산화번호
    /// </summary>
    public string? MeshNo { get; set; }

    /// <summary>
    /// CompositSwitch AliasName
    /// </summary>
    public string? Aliasname { get; set; }
}
