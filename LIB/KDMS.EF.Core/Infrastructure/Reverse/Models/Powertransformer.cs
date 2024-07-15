using System;
using System.Collections.Generic;

namespace KDMS.EF.Core.Infrastructure.Reverse.Models;

/// <summary>
/// MTR 테이블
/// </summary>
public partial class Powertransformer
{
    /// <summary>
    /// PowerTransformer  ID
    /// </summary>
    public long Pid { get; set; }

    /// <summary>
    /// SubStation ID
    /// </summary>
    public long? StFk { get; set; }

    /// <summary>
    /// TapChanger ID
    /// </summary>
    public long? TapFk { get; set; }

    /// <summary>
    /// TransformerWinding ID (1차측)
    /// </summary>
    public long? Trw1stFk { get; set; }

    /// <summary>
    /// TransformerWinding ID (2차측)
    /// </summary>
    public long? Trw2stFk { get; set; }

    /// <summary>
    /// Busbarsection ID (1차측)
    /// </summary>
    public long? Bbs1stFk { get; set; }

    /// <summary>
    /// Busbarsection ID (2차측)
    /// </summary>
    public long? Bbs2stFk { get; set; }

    /// <summary>
    /// PowerTransformer 명
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Bank Number
    /// </summary>
    public int? BankNo { get; set; }

    /// <summary>
    /// MTR 임피던스
    /// </summary>
    public int? MtrImp { get; set; }

    /// <summary>
    /// UnitSymbol
    /// </summary>
    public long? MtrImpUsfk { get; set; }

    /// <summary>
    /// Display Position (변전소 단선도 사용)
    /// </summary>
    public int? DispPos { get; set; }

    /// <summary>
    /// Substation Type (1:GIS, 2:MCSG, 3:GIS+MCSG, 4:Non Display Type)
    /// </summary>
    public int? StType { get; set; }
}
