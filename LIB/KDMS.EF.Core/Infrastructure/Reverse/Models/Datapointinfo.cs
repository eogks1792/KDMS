using System;
using System.Collections.Generic;

namespace KDMS.EF.Core.Infrastructure.Reverse.Models;

/// <summary>
/// 데이터 포인트 테이블
/// </summary>
public partial class Datapointinfo
{
    /// <summary>
    /// 포인트 아이디
    /// </summary>
    public int? Datapointid { get; set; }

    /// <summary>
    /// 포인트 타입
    /// </summary>
    public int? Pointtype { get; set; }

    /// <summary>
    /// Alarm Catagory ID
    /// </summary>
    public int? Alarmcategoryfk { get; set; }

    /// <summary>
    /// 포인트 이름
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// 이력 여부
    /// </summary>
    public short? Usehistory { get; set; }

    /// <summary>
    /// 제어 명
    /// </summary>
    public int? Ccdfk { get; set; }
}
