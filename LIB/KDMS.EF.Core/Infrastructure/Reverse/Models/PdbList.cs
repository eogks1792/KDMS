﻿using System;
using System.Collections.Generic;

namespace KDMS.EF.Core.Infrastructure.Reverse.Models;

/// <summary>
/// PDB 목록 테이블
/// </summary>
public partial class PdbList
{
    /// <summary>
    /// PDB 아이디
    /// </summary>
    public int Pid { get; set; }

    /// <summary>
    /// PDB 이름
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// PDB 사용여부
    /// </summary>
    public bool? UseYn { get; set; }

    /// <summary>
    /// PDB MD5
    /// </summary>
    public string? Md5 { get; set; }

    /// <summary>
    /// PDB 설명
    /// </summary>
    public string? Desc { get; set; }
}
