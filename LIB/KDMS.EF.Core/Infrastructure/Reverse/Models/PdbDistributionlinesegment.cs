using System;
using System.Collections.Generic;

namespace KDMS.EF.Core.Infrastructure.Reverse.Models;

/// <summary>
/// 선로 정보 저장 테이블
/// </summary>
public partial class PdbDistributionlinesegment
{
    /// <summary>
    /// Distribution Line ID 
    /// </summary>
    public long Dlsid { get; set; }

    /// <summary>
    /// CondutingEquipment ID
    /// </summary>
    public long? CeqFk { get; set; }

    /// <summary>
    /// 소속된 DistributionLine ID
    /// </summary>
    public long? DlFk { get; set; }

    /// <summary>
    /// PerLengthSequenceImpedanceA ID
    /// </summary>
    public long? PlsiAFk { get; set; }

    /// <summary>
    /// PerLengthSequenceImpedanceB ID
    /// </summary>
    public long? PlsiBFk { get; set; }

    /// <summary>
    /// PerLengthSequenceImpedanceC ID
    /// </summary>
    public long? PlsiCFk { get; set; }

    /// <summary>
    /// PerLengthSequenceImpedanceN ID
    /// </summary>
    public long? PlsiNFk { get; set; }

    /// <summary>
    /// DistributionLineSegment Name
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// DistributionLineSegment AliasName
    /// </summary>
    public string? Aliasname { get; set; }

    /// <summary>
    /// Line 길이
    /// </summary>
    public float? Length { get; set; }

    /// <summary>
    /// UnitSymbol 길이
    /// </summary>
    public long? LengthUsFk { get; set; }

    /// <summary>
    /// 전단 CondutingEquipment ID
    /// </summary>
    public long? CeqFFk { get; set; }

    /// <summary>
    /// 전단 SGRegion ID
    /// </summary>
    public long? SgrFFk { get; set; }

    /// <summary>
    /// 뒷단 CondutingEquipment ID
    /// </summary>
    public long? CeqBFk { get; set; }

    /// <summary>
    /// 뒷단 SGRegion ID
    /// </summary>
    public long? SgrBFk { get; set; }

    /// <summary>
    /// Section 부하
    /// </summary>
    public float? SecLoad { get; set; }

    /// <summary>
    /// Section 부하 Unit
    /// </summary>
    public long? SecLoadUsFk { get; set; }
}
