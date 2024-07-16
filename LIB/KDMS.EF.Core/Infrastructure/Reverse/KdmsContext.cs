using System;
using System.Collections.Generic;
using KDMS.EF.Core.Infrastructure.Reverse.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace KDMS.EF.Core.Infrastructure.Reverse;

public partial class KdmsContext : DbContext
{

    protected readonly IConfiguration _configuration;

    public KdmsContext(IConfiguration configuration)
    {
        _configuration = configuration;
    }


    public virtual DbSet<AiInfo> AiInfos { get; set; }

    public virtual DbSet<AlarmInfo> AlarmInfos { get; set; }

    public virtual DbSet<BiInfo> BiInfos { get; set; }

    public virtual DbSet<Compositeswitch> Compositeswitches { get; set; }

    public virtual DbSet<Datapointinfo> Datapointinfos { get; set; }

    public virtual DbSet<Distributionline> Distributionlines { get; set; }

    public virtual DbSet<Geographicalregion> Geographicalregions { get; set; }

    public virtual DbSet<HistoryCommState> HistoryCommStates { get; set; }

    public virtual DbSet<HistoryCommStateLog> HistoryCommStateLogs { get; set; }

    public virtual DbSet<HistoryDaystatDatum> HistoryDaystatData { get; set; }

    public virtual DbSet<HistoryFiAlarm> HistoryFiAlarms { get; set; }

    public virtual DbSet<HistoryMinDatum> HistoryMinData { get; set; }

    public virtual DbSet<PdbConductingequipment> PdbConductingequipments { get; set; }

    public virtual DbSet<PdbDistributionlinesegment> PdbDistributionlinesegments { get; set; }

    public virtual DbSet<Powertransformer> Powertransformers { get; set; }

    public virtual DbSet<SchduleInfo> SchduleInfos { get; set; }

    public virtual DbSet<SchduleType> SchduleTypes { get; set; }

    public virtual DbSet<Statistics15min> Statistics15mins { get; set; }

    public virtual DbSet<StatisticsDay> StatisticsDays { get; set; }

    public virtual DbSet<StatisticsHour> StatisticsHours { get; set; }

    public virtual DbSet<StatisticsMonth> StatisticsMonths { get; set; }

    public virtual DbSet<StatisticsYear> StatisticsYears { get; set; }

    public virtual DbSet<StorageInfo> StorageInfos { get; set; }

    public virtual DbSet<StorageType> StorageTypes { get; set; }

    public virtual DbSet<Subgeographicalregion> Subgeographicalregions { get; set; }

    public virtual DbSet<Substation> Substations { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("server=192.168.1.205;port=3306;database=kdms;user=root;password=20wellsdb19!@", Microsoft.EntityFrameworkCore.ServerVersion.Parse("11.3.2-mariadb"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_general_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<AiInfo>(entity =>
        {
            entity.HasKey(e => e.No).HasName("PRIMARY");

            entity.ToTable("ai_info", tb => tb.HasComment("AI 포인트 필터 테이블"));

            entity.Property(e => e.No)
                .ValueGeneratedNever()
                .HasComment("번호")
                .HasColumnType("int(11)")
                .HasColumnName("NO");
            entity.Property(e => e.Columnname)
                .HasMaxLength(64)
                .HasComment("MIN_DATA 컬럼 이름")
                .HasColumnName("COLUMNNAME");
            entity.Property(e => e.Datapointid)
                .HasComment("DATAPOINT 아이디")
                .HasColumnType("int(11)")
                .HasColumnName("DATAPOINTID");
            entity.Property(e => e.Datapointname)
                .HasMaxLength(64)
                .HasComment("DATAPOINT 이름")
                .HasColumnName("DATAPOINTNAME");
        });

        modelBuilder.Entity<AlarmInfo>(entity =>
        {
            entity.HasKey(e => e.PointId).HasName("PRIMARY");

            entity.ToTable("alarm_info", tb => tb.HasComment("알람 포인트 필터 테이블"));

            entity.Property(e => e.PointId)
                .ValueGeneratedNever()
                .HasComment("포인트 아이디")
                .HasColumnType("int(11)")
                .HasColumnName("POINT_ID");
            entity.Property(e => e.Alarmcategoryfk)
                .HasComment("Alarm Catagory ID")
                .HasColumnType("int(11)")
                .HasColumnName("ALARMCATEGORYFK");
            entity.Property(e => e.PointName)
                .HasMaxLength(64)
                .HasComment("포인트 이름")
                .HasColumnName("POINT_NAME");
            entity.Property(e => e.UseYn)
                .HasComment("포인트 사용여부")
                .HasColumnName("USE_YN");
        });

        modelBuilder.Entity<BiInfo>(entity =>
        {
            entity.HasKey(e => e.PointId).HasName("PRIMARY");

            entity.ToTable("bi_info", tb => tb.HasComment("BI 포인트 필터 테이블"));

            entity.Property(e => e.PointId)
                .ValueGeneratedNever()
                .HasComment("포인트 아이디")
                .HasColumnType("int(11)")
                .HasColumnName("POINT_ID");
            entity.Property(e => e.Alarmcategoryfk)
                .HasComment("Alarm Catagory ID")
                .HasColumnType("int(11)")
                .HasColumnName("ALARMCATEGORYFK");
            entity.Property(e => e.PointName)
                .HasMaxLength(64)
                .HasComment("포인트 이름")
                .HasColumnName("POINT_NAME");
            entity.Property(e => e.UseYn)
                .HasComment("포인트 사용여부")
                .HasColumnName("USE_YN");
        });

        modelBuilder.Entity<Compositeswitch>(entity =>
        {
            entity.HasKey(e => e.Pid).HasName("PRIMARY");

            entity.ToTable("compositeswitch", tb => tb.HasComment("Composite 스위치 정보 테이블"));

            entity.Property(e => e.Pid)
                .ValueGeneratedNever()
                .HasComment("CompositSwitch ID	")
                .HasColumnType("bigint(20)")
                .HasColumnName("PID");
            entity.Property(e => e.Aliasname)
                .HasMaxLength(64)
                .HasComment("CompositSwitch AliasName")
                .HasColumnName("ALIASNAME");
            entity.Property(e => e.DlFk)
                .HasComment("DL ID")
                .HasColumnType("bigint(20)")
                .HasColumnName("DL_FK");
            entity.Property(e => e.MeshNo)
                .HasMaxLength(64)
                .HasComment("전산화번호")
                .HasColumnName("MESH_NO");
            entity.Property(e => e.Name)
                .HasMaxLength(64)
                .HasComment("CompositSwitch 이름")
                .HasColumnName("NAME");
            entity.Property(e => e.Psrtype)
                .HasComment("PSR TYPE")
                .HasColumnType("bigint(20)")
                .HasColumnName("PSRTYPE");
        });

        modelBuilder.Entity<Datapointinfo>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("datapointinfo", tb => tb.HasComment("데이터 포인트 테이블"));

            entity.Property(e => e.Alarmcategoryfk)
                .HasComment("Alarm Catagory ID")
                .HasColumnType("int(11)")
                .HasColumnName("ALARMCATEGORYFK");
            entity.Property(e => e.Ccdfk)
                .HasComment("제어 명")
                .HasColumnType("int(11)")
                .HasColumnName("CCDFK");
            entity.Property(e => e.Datapointid)
                .HasComment("포인트 아이디")
                .HasColumnType("int(11)")
                .HasColumnName("DATAPOINTID");
            entity.Property(e => e.Name)
                .HasMaxLength(64)
                .HasComment("포인트 이름")
                .HasColumnName("NAME");
            entity.Property(e => e.Pointtype)
                .HasComment("포인트 타입")
                .HasColumnType("int(11)")
                .HasColumnName("POINTTYPE");
            entity.Property(e => e.Usehistory)
                .HasComment("이력 여부")
                .HasColumnType("smallint(6)")
                .HasColumnName("USEHISTORY");
        });

        modelBuilder.Entity<Distributionline>(entity =>
        {
            entity.HasKey(e => e.Dlid).HasName("PRIMARY");

            entity.ToTable("distributionline", tb => tb.HasComment("DL 정보 테이블"));

            entity.Property(e => e.Dlid)
                .ValueGeneratedNever()
                .HasComment("DL ID")
                .HasColumnType("bigint(20)")
                .HasColumnName("DLID");
            entity.Property(e => e.DlNo)
                .HasComment("회선번호")
                .HasColumnType("int(11)")
                .HasColumnName("DL_NO");
            entity.Property(e => e.Name)
                .HasMaxLength(32)
                .HasComment("DL 이름")
                .HasColumnName("NAME");
            entity.Property(e => e.Priority)
                .HasComment("회선 우선순위")
                .HasColumnType("int(11)")
                .HasColumnName("PRIORITY");
            entity.Property(e => e.PtrFk)
                .HasComment("MTR ID")
                .HasColumnType("bigint(20)")
                .HasColumnName("PTR_FK");
            entity.Property(e => e.RatedS)
                .HasComment("회선 기준용량")
                .HasColumnType("int(11)")
                .HasColumnName("RATED_S");
            entity.Property(e => e.RatedSUsFk)
                .HasComment("UnitSymbol FK")
                .HasColumnType("bigint(20)")
                .HasColumnName("RATED_S_US_FK");
            entity.Property(e => e.Reliability)
                .HasComment("회선 가용량")
                .HasColumnType("int(11)")
                .HasColumnName("RELIABILITY");
            entity.Property(e => e.StFk)
                .HasComment("변전소 ID")
                .HasColumnType("bigint(20)")
                .HasColumnName("ST_FK");
            entity.Property(e => e.SwFk)
                .HasComment("OCB ID")
                .HasColumnType("bigint(20)")
                .HasColumnName("SW_FK");
        });

        modelBuilder.Entity<Geographicalregion>(entity =>
        {
            entity.HasKey(e => e.Ggrid).HasName("PRIMARY");

            entity.ToTable("geographicalregion", tb => tb.HasComment("지사 테이블"));

            entity.Property(e => e.Ggrid)
                .ValueGeneratedNever()
                .HasComment("지사 아이디")
                .HasColumnType("int(11)")
                .HasColumnName("GGRID");
            entity.Property(e => e.Name)
                .HasMaxLength(32)
                .HasComment("지사 이름")
                .HasColumnName("NAME");
        });

        modelBuilder.Entity<HistoryCommState>(entity =>
        {
            entity.HasKey(e => e.SaveTime).HasName("PRIMARY");

            entity.ToTable("history_comm_state", tb => tb.HasComment("설비별 일일 통신 성공 테이블"));

            entity.Property(e => e.SaveTime)
                .HasComment("DB 기록시간")
                .HasColumnType("datetime")
                .HasColumnName("SAVE_TIME");
            entity.Property(e => e.Ceqid)
                .HasComment("단말장치 ID")
                .HasColumnType("int(11)")
                .HasColumnName("CEQID");
            entity.Property(e => e.CommFailCount)
                .HasComment("실패횟수")
                .HasColumnType("int(11)")
                .HasColumnName("COMM_FAIL_COUNT");
            entity.Property(e => e.CommSucessCount)
                .HasComment("성공횟수")
                .HasColumnType("int(11)")
                .HasColumnName("COMM_SUCESS_COUNT");
            entity.Property(e => e.CommSucessRate)
                .HasComment("통신 성공률")
                .HasColumnName("COMM_SUCESS_RATE");
            entity.Property(e => e.CommTime)
                .HasComment("정보 수집시간")
                .HasColumnType("datetime")
                .HasColumnName("COMM_TIME");
            entity.Property(e => e.CommTotalCount)
                .HasComment("전체횟수")
                .HasColumnType("int(11)")
                .HasColumnName("COMM_TOTAL_COUNT");
            entity.Property(e => e.Cpsid)
                .HasComment("CompositeSwitch ID")
                .HasColumnType("int(11)")
                .HasColumnName("CPSID");
            entity.Property(e => e.Dl)
                .HasMaxLength(64)
                .HasComment("소속DL")
                .HasColumnName("DL");
            entity.Property(e => e.EqType)
                .HasComment("장치 타입")
                .HasColumnType("int(11)")
                .HasColumnName("EQ_TYPE");
            entity.Property(e => e.Name)
                .HasMaxLength(128)
                .HasComment("단말장치 명")
                .HasColumnName("NAME");
        });

        modelBuilder.Entity<HistoryCommStateLog>(entity =>
        {
            entity.HasKey(e => e.SaveTime).HasName("PRIMARY");

            entity.ToTable("history_comm_state_log", tb => tb.HasComment("설비별 일일 통신 성공 로그 테이블"));

            entity.Property(e => e.SaveTime)
                .HasComment("DB 기록시간")
                .HasColumnType("datetime")
                .HasColumnName("SAVE_TIME");
            entity.Property(e => e.Ceqid)
                .HasComment("단말장치 ID")
                .HasColumnType("int(11)")
                .HasColumnName("CEQID");
            entity.Property(e => e.CommFailCount)
                .HasComment("실패횟수")
                .HasColumnType("int(11)")
                .HasColumnName("COMM_FAIL_COUNT");
            entity.Property(e => e.CommState)
                .HasComment("성공/실패")
                .HasColumnName("COMM_STATE");
            entity.Property(e => e.CommSucessCount)
                .HasComment("성공횟수")
                .HasColumnType("int(11)")
                .HasColumnName("COMM_SUCESS_COUNT");
            entity.Property(e => e.CommSucessRate)
                .HasComment("통신 성공률")
                .HasColumnName("COMM_SUCESS_RATE");
            entity.Property(e => e.CommTime)
                .HasComment("정보 수집시간")
                .HasColumnType("datetime")
                .HasColumnName("COMM_TIME");
            entity.Property(e => e.CommTotalCount)
                .HasComment("전체횟수")
                .HasColumnType("int(11)")
                .HasColumnName("COMM_TOTAL_COUNT");
            entity.Property(e => e.Cpsid)
                .HasComment("CompositeSwitch ID")
                .HasColumnType("int(11)")
                .HasColumnName("CPSID");
            entity.Property(e => e.Dl)
                .HasMaxLength(64)
                .HasComment("소속DL")
                .HasColumnName("DL");
            entity.Property(e => e.EqType)
                .HasComment("장치 타입")
                .HasColumnType("int(11)")
                .HasColumnName("EQ_TYPE");
            entity.Property(e => e.Name)
                .HasMaxLength(128)
                .HasComment("단말장치 명")
                .HasColumnName("NAME");
        });

        modelBuilder.Entity<HistoryDaystatDatum>(entity =>
        {
            entity.HasKey(e => new { e.SaveTime, e.Ceqid })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity.ToTable("history_daystat_data", tb => tb.HasComment("시간단위 통계 테이블"));

            entity.Property(e => e.SaveTime)
                .HasComment("DB 기록시간")
                .HasColumnType("datetime")
                .HasColumnName("SAVE_TIME");
            entity.Property(e => e.Ceqid)
                .HasComment("단말장치 ID")
                .HasColumnType("int(11)")
                .HasColumnName("CEQID");
            entity.Property(e => e.AverageCurrentA)
                .HasComment("전류A-평균")
                .HasColumnName("AVERAGE_CURRENT_A");
            entity.Property(e => e.AverageCurrentB)
                .HasComment("전류B-평균")
                .HasColumnName("AVERAGE_CURRENT_B");
            entity.Property(e => e.AverageCurrentC)
                .HasComment("전류C-평균")
                .HasColumnName("AVERAGE_CURRENT_C");
            entity.Property(e => e.AverageCurrentN)
                .HasComment("전류N-평균")
                .HasColumnName("AVERAGE_CURRENT_N");
            entity.Property(e => e.Circuitno)
                .HasComment("회로번호")
                .HasColumnType("int(11)")
                .HasColumnName("CIRCUITNO");
            entity.Property(e => e.CommTime)
                .HasComment("정보 수집시간")
                .HasColumnType("datetime")
                .HasColumnName("COMM_TIME");
            entity.Property(e => e.Cpsid)
                .HasComment("Composite ID")
                .HasColumnType("int(11)")
                .HasColumnName("CPSID");
            entity.Property(e => e.CurrentUnbalance)
                .HasComment("전류 불평형률")
                .HasColumnType("int(11)")
                .HasColumnName("CURRENT_UNBALANCE");
            entity.Property(e => e.Diagnostics)
                .HasComment("단말장치 상태")
                .HasColumnType("int(11)")
                .HasColumnName("DIAGNOSTICS");
            entity.Property(e => e.Dl)
                .HasMaxLength(64)
                .HasComment("소속DL")
                .HasColumnName("DL");
            entity.Property(e => e.Frequency)
                .HasComment("주파수")
                .HasColumnName("FREQUENCY");
            entity.Property(e => e.MaxCommTime)
                .HasComment("최대 수집시간")
                .HasColumnType("datetime")
                .HasColumnName("MAX_COMM_TIME");
            entity.Property(e => e.MaxCurrentA)
                .HasComment("전류A-최대")
                .HasColumnName("MAX_CURRENT_A");
            entity.Property(e => e.MaxCurrentB)
                .HasComment("전류B-최대")
                .HasColumnName("MAX_CURRENT_B");
            entity.Property(e => e.MaxCurrentC)
                .HasComment("전류C-최대")
                .HasColumnName("MAX_CURRENT_C");
            entity.Property(e => e.MaxCurrentN)
                .HasComment("전류N-최대")
                .HasColumnName("MAX_CURRENT_N");
            entity.Property(e => e.MinCommTime)
                .HasComment("최소 수집시간")
                .HasColumnType("datetime")
                .HasColumnName("MIN_COMM_TIME");
            entity.Property(e => e.MinCurrentA)
                .HasComment("전류A - 최소")
                .HasColumnName("MIN_CURRENT_A");
            entity.Property(e => e.MinCurrentB)
                .HasComment("전류B - 최소")
                .HasColumnName("MIN_CURRENT_B");
            entity.Property(e => e.MinCurrentC)
                .HasComment("전류C - 최소")
                .HasColumnName("MIN_CURRENT_C");
            entity.Property(e => e.MinCurrentN)
                .HasComment("전류N - 최소")
                .HasColumnName("MIN_CURRENT_N");
            entity.Property(e => e.Name)
                .HasMaxLength(128)
                .HasComment("단말장치 명")
                .HasColumnName("NAME");
            entity.Property(e => e.VoltageUnbalance)
                .HasComment("전압 불평형률")
                .HasColumnType("int(11)")
                .HasColumnName("VOLTAGE_UNBALANCE");
        });

        modelBuilder.Entity<HistoryFiAlarm>(entity =>
        {
            entity.HasKey(e => new { e.SaveTime, e.Ceqid })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity.ToTable("history_fi_alarm", tb => tb.HasComment("FI 발생 저장 테이블"));

            entity.Property(e => e.SaveTime)
                .HasComment("DB 기록시간")
                .HasColumnType("datetime")
                .HasColumnName("SAVE_TIME");
            entity.Property(e => e.Ceqid)
                .HasComment("단말장치 ID")
                .HasColumnType("int(11)")
                .HasColumnName("CEQID");
            entity.Property(e => e.AlarmName)
                .HasMaxLength(64)
                .HasComment("알람 이름")
                .HasColumnName("ALARM_NAME");
            entity.Property(e => e.Circuitno)
                .HasComment("회로번호")
                .HasColumnType("int(11)")
                .HasColumnName("CIRCUITNO");
            entity.Property(e => e.Cpsid)
                .HasComment("Composite ID")
                .HasColumnType("int(11)")
                .HasColumnName("CPSID");
            entity.Property(e => e.Dl)
                .HasMaxLength(64)
                .HasComment("소속DL")
                .HasColumnName("DL");
            entity.Property(e => e.FaultCurrentA)
                .HasComment("고장전류A")
                .HasColumnName("FAULT_CURRENT_A");
            entity.Property(e => e.FaultCurrentB)
                .HasComment("고장전류B")
                .HasColumnName("FAULT_CURRENT_B");
            entity.Property(e => e.FaultCurrentC)
                .HasComment("고장전류C")
                .HasColumnName("FAULT_CURRENT_C");
            entity.Property(e => e.FaultCurrentN)
                .HasComment("고장전류N")
                .HasColumnName("FAULT_CURRENT_N");
            entity.Property(e => e.FrtuTime)
                .HasComment("단말장치 발생시간")
                .HasColumnType("datetime")
                .HasColumnName("FRTU_TIME");
            entity.Property(e => e.LogDesc)
                .HasMaxLength(128)
                .HasComment("알람 내용")
                .HasColumnName("LOG_DESC");
            entity.Property(e => e.LogTime)
                .HasComment("서버 기록시간")
                .HasColumnType("datetime")
                .HasColumnName("LOG_TIME");
            entity.Property(e => e.Name)
                .HasMaxLength(128)
                .HasComment("단말장치 명")
                .HasColumnName("NAME");
            entity.Property(e => e.Value)
                .HasComment("알람 값")
                .HasColumnName("VALUE");
        });

        modelBuilder.Entity<HistoryMinDatum>(entity =>
        {
            entity.HasKey(e => new { e.SaveTime, e.Ceqid })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity.ToTable("history_min_data", tb => tb.HasComment("실시간 저장 테이블"));

            entity.Property(e => e.SaveTime)
                .HasComment("DB 기록시간")
                .HasColumnType("datetime")
                .HasColumnName("SAVE_TIME");
            entity.Property(e => e.Ceqid)
                .HasComment("단말장치 ID")
                .HasColumnType("int(11)")
                .HasColumnName("CEQID");
            entity.Property(e => e.ApparentPowerA)
                .HasComment("피상전력A")
                .HasColumnName("APPARENT_POWER_A");
            entity.Property(e => e.ApparentPowerB)
                .HasComment("피상전력B")
                .HasColumnName("APPARENT_POWER_B");
            entity.Property(e => e.ApparentPowerC)
                .HasComment("피상전력C")
                .HasColumnName("APPARENT_POWER_C");
            entity.Property(e => e.Circuitno)
                .HasComment("회로번호")
                .HasColumnType("int(11)")
                .HasColumnName("CIRCUITNO");
            entity.Property(e => e.CommTime)
                .HasComment("정보 수집시간")
                .HasColumnType("datetime")
                .HasColumnName("COMM_TIME");
            entity.Property(e => e.Cpsid)
                .HasComment("Composite ID")
                .HasColumnType("int(11)")
                .HasColumnName("CPSID");
            entity.Property(e => e.CurrentA)
                .HasComment("전류A")
                .HasColumnName("CURRENT_A");
            entity.Property(e => e.CurrentB)
                .HasComment("전류B")
                .HasColumnName("CURRENT_B");
            entity.Property(e => e.CurrentC)
                .HasComment("전류C")
                .HasColumnName("CURRENT_C");
            entity.Property(e => e.CurrentN)
                .HasComment("전류N")
                .HasColumnName("CURRENT_N");
            entity.Property(e => e.CurrentPhaseA)
                .HasComment("전류위상A")
                .HasColumnName("CURRENT_PHASE_A");
            entity.Property(e => e.CurrentPhaseB)
                .HasComment("전류위상B")
                .HasColumnName("CURRENT_PHASE_B");
            entity.Property(e => e.CurrentPhaseC)
                .HasComment("전류위상C")
                .HasColumnName("CURRENT_PHASE_C");
            entity.Property(e => e.CurrentPhaseN)
                .HasComment("전류위상N")
                .HasColumnName("CURRENT_PHASE_N");
            entity.Property(e => e.CurrentUnbalance)
                .HasComment("전류 불평형률")
                .HasColumnType("int(11)")
                .HasColumnName("CURRENT_UNBALANCE");
            entity.Property(e => e.Diagnostics)
                .HasComment("단말장치 상태")
                .HasColumnType("int(11)")
                .HasColumnName("DIAGNOSTICS");
            entity.Property(e => e.Dl)
                .HasMaxLength(64)
                .HasComment("소속DL")
                .HasColumnName("DL");
            entity.Property(e => e.FaultCurrentA)
                .HasComment("고장전류A")
                .HasColumnName("FAULT_CURRENT_A");
            entity.Property(e => e.FaultCurrentB)
                .HasComment("고장전류B")
                .HasColumnName("FAULT_CURRENT_B");
            entity.Property(e => e.FaultCurrentC)
                .HasComment("고장전류C")
                .HasColumnName("FAULT_CURRENT_C");
            entity.Property(e => e.FaultCurrentN)
                .HasComment("고장전류N")
                .HasColumnName("FAULT_CURRENT_N");
            entity.Property(e => e.Frequency)
                .HasComment("주파수")
                .HasColumnName("FREQUENCY");
            entity.Property(e => e.Name)
                .HasMaxLength(128)
                .HasComment("단말장치 명")
                .HasColumnName("NAME");
            entity.Property(e => e.PowerFactor3p)
                .HasComment("역률3상")
                .HasColumnType("int(11)")
                .HasColumnName("POWER_FACTOR_3P");
            entity.Property(e => e.PowerFactorA)
                .HasComment("역률A")
                .HasColumnType("int(11)")
                .HasColumnName("POWER_FACTOR_A");
            entity.Property(e => e.PowerFactorB)
                .HasComment("역률B")
                .HasColumnType("int(11)")
                .HasColumnName("POWER_FACTOR_B");
            entity.Property(e => e.PowerFactorC)
                .HasComment("역률C")
                .HasColumnType("int(11)")
                .HasColumnName("POWER_FACTOR_C");
            entity.Property(e => e.VoltageA)
                .HasComment("전압A")
                .HasColumnName("VOLTAGE_A");
            entity.Property(e => e.VoltageB)
                .HasComment("전압B")
                .HasColumnName("VOLTAGE_B");
            entity.Property(e => e.VoltageC)
                .HasComment("전압C")
                .HasColumnName("VOLTAGE_C");
            entity.Property(e => e.VoltagePhaseA)
                .HasComment("전압위상A")
                .HasColumnName("VOLTAGE_PHASE_A");
            entity.Property(e => e.VoltagePhaseB)
                .HasComment("전압위상B")
                .HasColumnName("VOLTAGE_PHASE_B");
            entity.Property(e => e.VoltagePhaseC)
                .HasComment("전압위상C")
                .HasColumnName("VOLTAGE_PHASE_C");
            entity.Property(e => e.VoltageUnbalance)
                .HasComment("전압 불평형률")
                .HasColumnType("int(11)")
                .HasColumnName("VOLTAGE_UNBALANCE");
        });

        modelBuilder.Entity<PdbConductingequipment>(entity =>
        {
            entity.HasKey(e => e.Ceqid).HasName("PRIMARY");

            entity.ToTable("pdb_conductingequipment", tb => tb.HasComment("설비 정보 저장 테이블"));

            entity.Property(e => e.Ceqid)
                .ValueGeneratedNever()
                .HasComment("ConductionEquipment  ID ")
                .HasColumnType("bigint(20)")
                .HasColumnName("CEQID");
            entity.Property(e => e.AiCnt)
                .HasComment("Analog Input Point Count")
                .HasColumnType("int(11)")
                .HasColumnName("AI_CNT");
            entity.Property(e => e.AiPid)
                .HasComment("Start Analog Point ID")
                .HasColumnType("bigint(20)")
                .HasColumnName("AI_PID");
            entity.Property(e => e.AoCnt)
                .HasComment("Analog Output Point Count")
                .HasColumnType("int(11)")
                .HasColumnName("AO_CNT");
            entity.Property(e => e.AoPid)
                .HasComment("Start SetPoint ID")
                .HasColumnType("bigint(20)")
                .HasColumnName("AO_PID");
            entity.Property(e => e.BaseVoltage)
                .HasComment("기준 전압")
                .HasColumnName("BASE_VOLTAGE");
            entity.Property(e => e.BiCnt)
                .HasComment("Binary Input Point Count")
                .HasColumnType("int(11)")
                .HasColumnName("BI_CNT");
            entity.Property(e => e.BiPid)
                .HasComment("Start Discrete Point ID")
                .HasColumnType("bigint(20)")
                .HasColumnName("BI_PID");
            entity.Property(e => e.BusbarOrder)
                .HasComment("Busbarsection Order")
                .HasColumnType("int(11)")
                .HasColumnName("BUSBAR_ORDER");
            entity.Property(e => e.Desc)
                .HasMaxLength(128)
                .HasComment("Equipment Descrption")
                .HasColumnName("DESC");
            entity.Property(e => e.DevNo)
                .HasMaxLength(64)
                .HasComment("Equipment Device Number")
                .HasColumnName("DEV_NO");
            entity.Property(e => e.DlFk)
                .HasComment("Distribution Line ID ")
                .HasColumnType("bigint(20)")
                .HasColumnName("DL_FK");
            entity.Property(e => e.DtrFk)
                .HasComment("distributionTransformer ID")
                .HasColumnType("bigint(20)")
                .HasColumnName("DTR_FK");
            entity.Property(e => e.EcFk)
                .HasComment("Equipment Container ID")
                .HasColumnType("bigint(20)")
                .HasColumnName("EC_FK");
            entity.Property(e => e.EcName)
                .HasMaxLength(64)
                .HasComment("Equipment Container Name")
                .HasColumnName("EC_NAME");
            entity.Property(e => e.EqFk)
                .HasComment("Equipment ID")
                .HasColumnType("bigint(20)")
                .HasColumnName("EQ_FK");
            entity.Property(e => e.LinkBbsFk)
                .HasComment("Link BusBarSection ID")
                .HasColumnType("bigint(20)")
                .HasColumnName("LINK_BBS_FK");
            entity.Property(e => e.LinkCbFk)
                .HasComment("Link Breaker ID")
                .HasColumnType("bigint(20)")
                .HasColumnName("LINK_CB_FK");
            entity.Property(e => e.LinkDlFk)
                .HasComment("Link Distribution Line ID")
                .HasColumnType("bigint(20)")
                .HasColumnName("LINK_DL_FK");
            entity.Property(e => e.LinkStFk)
                .HasComment("Link SubStation ID")
                .HasColumnType("bigint(20)")
                .HasColumnName("LINK_ST_FK");
            entity.Property(e => e.MeshNo)
                .HasMaxLength(12)
                .HasComment("Equipment Mesh Number")
                .HasColumnName("MESH_NO");
            entity.Property(e => e.Name)
                .HasMaxLength(64)
                .HasComment("Equipment Name ")
                .HasColumnName("NAME");
            entity.Property(e => e.Phases)
                .HasComment("상연결정보")
                .HasColumnType("int(11)")
                .HasColumnName("PHASES");
            entity.Property(e => e.PiCnt)
                .HasComment("Counter Point Count")
                .HasColumnType("int(11)")
                .HasColumnName("PI_CNT");
            entity.Property(e => e.PiPid)
                .HasComment("Start Accumulator Point ID")
                .HasColumnType("bigint(20)")
                .HasColumnName("PI_PID");
            entity.Property(e => e.Psrtype)
                .HasComment("PowerSystemResource Type ID")
                .HasColumnType("int(11)")
                .HasColumnName("PSRTYPE");
            entity.Property(e => e.PtrFk)
                .HasComment("PowerTransformer ID")
                .HasColumnType("bigint(20)")
                .HasColumnName("PTR_FK");
            entity.Property(e => e.RtuType)
                .HasComment("Equipment RTU Type")
                .HasColumnType("int(11)")
                .HasColumnName("RTU_TYPE");
            entity.Property(e => e.StFk)
                .HasComment("SubStation ID")
                .HasColumnType("bigint(20)")
                .HasColumnName("ST_FK");
            entity.Property(e => e.SwType)
                .HasComment("단말/상시연계 상태 표시")
                .HasColumnType("bigint(20)")
                .HasColumnName("SW_TYPE");
        });

        modelBuilder.Entity<PdbDistributionlinesegment>(entity =>
        {
            entity.HasKey(e => e.Dlsid).HasName("PRIMARY");

            entity.ToTable("pdb_distributionlinesegment", tb => tb.HasComment("선로 정보 저장 테이블"));

            entity.Property(e => e.Dlsid)
                .ValueGeneratedNever()
                .HasComment("Distribution Line ID ")
                .HasColumnType("bigint(20)")
                .HasColumnName("DLSID");
            entity.Property(e => e.Aliasname)
                .HasMaxLength(64)
                .HasComment("DistributionLineSegment AliasName")
                .HasColumnName("ALIASNAME");
            entity.Property(e => e.CeqBFk)
                .HasComment("뒷단 CondutingEquipment ID")
                .HasColumnType("bigint(20)")
                .HasColumnName("CEQ_B_FK");
            entity.Property(e => e.CeqFFk)
                .HasComment("전단 CondutingEquipment ID")
                .HasColumnType("bigint(20)")
                .HasColumnName("CEQ_F_FK");
            entity.Property(e => e.CeqFk)
                .HasComment("CondutingEquipment ID")
                .HasColumnType("bigint(20)")
                .HasColumnName("CEQ_FK");
            entity.Property(e => e.DlFk)
                .HasComment("소속된 DistributionLine ID")
                .HasColumnType("bigint(20)")
                .HasColumnName("DL_FK");
            entity.Property(e => e.Length)
                .HasComment("Line 길이")
                .HasColumnName("LENGTH");
            entity.Property(e => e.LengthUsFk)
                .HasComment("UnitSymbol 길이")
                .HasColumnType("bigint(20)")
                .HasColumnName("LENGTH_US_FK");
            entity.Property(e => e.Name)
                .HasMaxLength(64)
                .HasComment("DistributionLineSegment Name")
                .HasColumnName("NAME");
            entity.Property(e => e.PlsiAFk)
                .HasComment("PerLengthSequenceImpedanceA ID")
                .HasColumnType("bigint(20)")
                .HasColumnName("PLSI_A_FK");
            entity.Property(e => e.PlsiBFk)
                .HasComment("PerLengthSequenceImpedanceB ID")
                .HasColumnType("bigint(20)")
                .HasColumnName("PLSI_B_FK");
            entity.Property(e => e.PlsiCFk)
                .HasComment("PerLengthSequenceImpedanceC ID")
                .HasColumnType("bigint(20)")
                .HasColumnName("PLSI_C_FK");
            entity.Property(e => e.PlsiNFk)
                .HasComment("PerLengthSequenceImpedanceN ID")
                .HasColumnType("bigint(20)")
                .HasColumnName("PLSI_N_FK");
            entity.Property(e => e.SecLoad)
                .HasComment("Section 부하")
                .HasColumnName("SEC_LOAD");
            entity.Property(e => e.SecLoadUsFk)
                .HasComment("Section 부하 Unit")
                .HasColumnType("bigint(20)")
                .HasColumnName("SEC_LOAD_US_FK");
            entity.Property(e => e.SgrBFk)
                .HasComment("뒷단 SGRegion ID")
                .HasColumnType("bigint(20)")
                .HasColumnName("SGR_B_FK");
            entity.Property(e => e.SgrFFk)
                .HasComment("전단 SGRegion ID")
                .HasColumnType("bigint(20)")
                .HasColumnName("SGR_F_FK");
        });

        modelBuilder.Entity<Powertransformer>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("powertransformer", tb => tb.HasComment("MTR 테이블"));

            entity.Property(e => e.BankNo)
                .HasComment("Bank Number")
                .HasColumnType("int(11)")
                .HasColumnName("BANK_NO");
            entity.Property(e => e.Bbs1stFk)
                .HasComment("Busbarsection ID (1차측)")
                .HasColumnType("bigint(20)")
                .HasColumnName("BBS_1ST_FK");
            entity.Property(e => e.Bbs2stFk)
                .HasComment("Busbarsection ID (2차측)")
                .HasColumnType("bigint(20)")
                .HasColumnName("BBS_2ST_FK");
            entity.Property(e => e.DispPos)
                .HasComment("Display Position (변전소 단선도 사용)")
                .HasColumnType("int(11)")
                .HasColumnName("DISP_POS");
            entity.Property(e => e.MtrImp)
                .HasComment("MTR 임피던스")
                .HasColumnType("int(11)")
                .HasColumnName("MTR_IMP");
            entity.Property(e => e.MtrImpUsfk)
                .HasComment("UnitSymbol")
                .HasColumnType("bigint(20)")
                .HasColumnName("MTR_IMP_USFK");
            entity.Property(e => e.Name)
                .HasMaxLength(64)
                .HasComment("PowerTransformer 명")
                .HasColumnName("NAME");
            entity.Property(e => e.Pid)
                .HasComment("PowerTransformer  ID")
                .HasColumnType("bigint(20)")
                .HasColumnName("PID");
            entity.Property(e => e.StFk)
                .HasComment("SubStation ID")
                .HasColumnType("bigint(20)")
                .HasColumnName("ST_FK");
            entity.Property(e => e.StType)
                .HasComment("Substation Type (1:GIS, 2:MCSG, 3:GIS+MCSG, 4:Non Display Type)")
                .HasColumnType("int(11)")
                .HasColumnName("ST_TYPE");
            entity.Property(e => e.TapFk)
                .HasComment("TapChanger ID")
                .HasColumnType("bigint(20)")
                .HasColumnName("TAP_FK");
            entity.Property(e => e.Trw1stFk)
                .HasComment("TransformerWinding ID (1차측)")
                .HasColumnType("bigint(20)")
                .HasColumnName("TRW_1ST_FK");
            entity.Property(e => e.Trw2stFk)
                .HasComment("TransformerWinding ID (2차측)")
                .HasColumnType("bigint(20)")
                .HasColumnName("TRW_2ST_FK");
        });

        modelBuilder.Entity<SchduleInfo>(entity =>
        {
            entity.HasKey(e => e.SchduleId).HasName("PRIMARY");

            entity.ToTable("schdule_info", tb => tb.HasComment("스케줄 정보 테이블"));

            entity.Property(e => e.SchduleId)
                .ValueGeneratedNever()
                .HasComment("스케줄 아이디")
                .HasColumnType("int(11)")
                .HasColumnName("SCHDULE_ID");
            entity.Property(e => e.Desc)
                .HasMaxLength(64)
                .HasComment("스케줄 설명")
                .HasColumnName("DESC");
            entity.Property(e => e.SchduleType)
                .HasComment("스케줄 설정 타입")
                .HasColumnType("int(11)")
                .HasColumnName("SCHDULE_TYPE");
            entity.Property(e => e.SchduleValue)
                .HasMaxLength(64)
                .HasComment("스케줄 설정 주기")
                .HasColumnName("SCHDULE_VALUE");
        });

        modelBuilder.Entity<SchduleType>(entity =>
        {
            entity.HasKey(e => e.SchduleType1).HasName("PRIMARY");

            entity.ToTable("schdule_type", tb => tb.HasComment("스케줄 설정 타입 테이블"));

            entity.Property(e => e.SchduleType1)
                .ValueGeneratedNever()
                .HasComment("스케줄 타입")
                .HasColumnType("int(11)")
                .HasColumnName("SCHDULE_TYPE");
            entity.Property(e => e.Name)
                .HasMaxLength(32)
                .HasComment("타입 이름")
                .HasColumnName("NAME");
        });

        modelBuilder.Entity<Statistics15min>(entity =>
        {
            entity.HasKey(e => new { e.SaveTime, e.Ceqid })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity.ToTable("statistics_15min", tb => tb.HasComment("실시간 15분 평균부하 테이블"));

            entity.Property(e => e.SaveTime)
                .HasComment("DB 기록시간")
                .HasColumnType("datetime")
                .HasColumnName("SAVE_TIME");
            entity.Property(e => e.Ceqid)
                .HasComment("단말장치 ID")
                .HasColumnType("int(11)")
                .HasColumnName("CEQID");
            entity.Property(e => e.AverageCurrentA)
                .HasComment("전류A - 평균")
                .HasColumnName("AVERAGE_CURRENT_A");
            entity.Property(e => e.AverageCurrentB)
                .HasComment("전류B - 평균")
                .HasColumnName("AVERAGE_CURRENT_B");
            entity.Property(e => e.AverageCurrentC)
                .HasComment("전류C - 평균")
                .HasColumnName("AVERAGE_CURRENT_C");
            entity.Property(e => e.AverageCurrentN)
                .HasComment("전류N - 평균")
                .HasColumnName("AVERAGE_CURRENT_N");
            entity.Property(e => e.Circuitno)
                .HasComment("회로번호")
                .HasColumnType("int(11)")
                .HasColumnName("CIRCUITNO");
            entity.Property(e => e.CommTime)
                .HasComment("정보 수집시간")
                .HasColumnType("datetime")
                .HasColumnName("COMM_TIME");
            entity.Property(e => e.Cpsid)
                .HasComment("Composite ID")
                .HasColumnType("int(11)")
                .HasColumnName("CPSID");
            entity.Property(e => e.Dl)
                .HasMaxLength(64)
                .HasComment("소속DL")
                .HasColumnName("DL");
            entity.Property(e => e.Name)
                .HasMaxLength(128)
                .HasComment("단말장치 명")
                .HasColumnName("NAME");
        });

        modelBuilder.Entity<StatisticsDay>(entity =>
        {
            entity.HasKey(e => new { e.SaveTime, e.Ceqid })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity.ToTable("statistics_day", tb => tb.HasComment("전류 일 통계 저장 테이블"));

            entity.Property(e => e.SaveTime)
                .HasComment("DB 기록시간")
                .HasColumnType("datetime")
                .HasColumnName("SAVE_TIME");
            entity.Property(e => e.Ceqid)
                .HasComment("단말장치 ID")
                .HasColumnType("int(11)")
                .HasColumnName("CEQID");
            entity.Property(e => e.AverageCurrentA)
                .HasComment("전류A-평균")
                .HasColumnName("AVERAGE_CURRENT_A");
            entity.Property(e => e.AverageCurrentB)
                .HasComment("전류B-평균")
                .HasColumnName("AVERAGE_CURRENT_B");
            entity.Property(e => e.AverageCurrentC)
                .HasComment("전류C-평균")
                .HasColumnName("AVERAGE_CURRENT_C");
            entity.Property(e => e.AverageCurrentN)
                .HasComment("전류N-평균")
                .HasColumnName("AVERAGE_CURRENT_N");
            entity.Property(e => e.Circuitno)
                .HasComment("회로번호")
                .HasColumnType("int(11)")
                .HasColumnName("CIRCUITNO");
            entity.Property(e => e.Cpsid)
                .HasComment("Composite ID")
                .HasColumnType("int(11)")
                .HasColumnName("CPSID");
            entity.Property(e => e.Dl)
                .HasMaxLength(64)
                .HasComment("소속DL")
                .HasColumnName("DL");
            entity.Property(e => e.MaxCommTime)
                .HasComment("최대 수집시간")
                .HasColumnType("datetime")
                .HasColumnName("MAX_COMM_TIME");
            entity.Property(e => e.MaxCurrentA)
                .HasComment("전류A-최대")
                .HasColumnName("MAX_CURRENT_A");
            entity.Property(e => e.MaxCurrentB)
                .HasComment("전류B-최대")
                .HasColumnName("MAX_CURRENT_B");
            entity.Property(e => e.MaxCurrentC)
                .HasComment("전류C-최대")
                .HasColumnName("MAX_CURRENT_C");
            entity.Property(e => e.MaxCurrentN)
                .HasComment("전류N-최대")
                .HasColumnName("MAX_CURRENT_N");
            entity.Property(e => e.MinCommTime)
                .HasComment("최소 수집시간")
                .HasColumnType("datetime")
                .HasColumnName("MIN_COMM_TIME");
            entity.Property(e => e.MinCurrentA)
                .HasComment("전류A - 최소")
                .HasColumnName("MIN_CURRENT_A");
            entity.Property(e => e.MinCurrentB)
                .HasComment("전류B - 최소")
                .HasColumnName("MIN_CURRENT_B");
            entity.Property(e => e.MinCurrentC)
                .HasComment("전류C - 최소")
                .HasColumnName("MIN_CURRENT_C");
            entity.Property(e => e.MinCurrentN)
                .HasComment("전류N - 최소")
                .HasColumnName("MIN_CURRENT_N");
            entity.Property(e => e.Name)
                .HasMaxLength(128)
                .HasComment("단말장치 명")
                .HasColumnName("NAME");
        });

        modelBuilder.Entity<StatisticsHour>(entity =>
        {
            entity.HasKey(e => new { e.SaveTime, e.Ceqid })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity.ToTable("statistics_hour", tb => tb.HasComment("전류 시간 통계 저장 테이블"));

            entity.Property(e => e.SaveTime)
                .HasComment("DB 기록시간")
                .HasColumnType("datetime")
                .HasColumnName("SAVE_TIME");
            entity.Property(e => e.Ceqid)
                .HasComment("단말장치 ID")
                .HasColumnType("int(11)")
                .HasColumnName("CEQID");
            entity.Property(e => e.AverageCurrentA)
                .HasComment("전류A-평균")
                .HasColumnName("AVERAGE_CURRENT_A");
            entity.Property(e => e.AverageCurrentB)
                .HasComment("전류B-평균")
                .HasColumnName("AVERAGE_CURRENT_B");
            entity.Property(e => e.AverageCurrentC)
                .HasComment("전류C-평균")
                .HasColumnName("AVERAGE_CURRENT_C");
            entity.Property(e => e.AverageCurrentN)
                .HasComment("전류N-평균")
                .HasColumnName("AVERAGE_CURRENT_N");
            entity.Property(e => e.Circuitno)
                .HasComment("회로번호")
                .HasColumnType("int(11)")
                .HasColumnName("CIRCUITNO");
            entity.Property(e => e.Cpsid)
                .HasComment("Composite ID")
                .HasColumnType("int(11)")
                .HasColumnName("CPSID");
            entity.Property(e => e.Dl)
                .HasMaxLength(64)
                .HasComment("소속DL")
                .HasColumnName("DL");
            entity.Property(e => e.MaxCommTime)
                .HasComment("최대 수집시간")
                .HasColumnType("datetime")
                .HasColumnName("MAX_COMM_TIME");
            entity.Property(e => e.MaxCurrentA)
                .HasComment("전류A-최대")
                .HasColumnName("MAX_CURRENT_A");
            entity.Property(e => e.MaxCurrentB)
                .HasComment("전류B-최대")
                .HasColumnName("MAX_CURRENT_B");
            entity.Property(e => e.MaxCurrentC)
                .HasComment("전류C-최대")
                .HasColumnName("MAX_CURRENT_C");
            entity.Property(e => e.MaxCurrentN)
                .HasComment("전류N-최대")
                .HasColumnName("MAX_CURRENT_N");
            entity.Property(e => e.MinCommTime)
                .HasComment("최소 수집시간")
                .HasColumnType("datetime")
                .HasColumnName("MIN_COMM_TIME");
            entity.Property(e => e.MinCurrentA)
                .HasComment("전류A - 최소")
                .HasColumnName("MIN_CURRENT_A");
            entity.Property(e => e.MinCurrentB)
                .HasComment("전류B - 최소")
                .HasColumnName("MIN_CURRENT_B");
            entity.Property(e => e.MinCurrentC)
                .HasComment("전류C - 최소")
                .HasColumnName("MIN_CURRENT_C");
            entity.Property(e => e.MinCurrentN)
                .HasComment("전류N - 최소")
                .HasColumnName("MIN_CURRENT_N");
            entity.Property(e => e.Name)
                .HasMaxLength(128)
                .HasComment("단말장치 명")
                .HasColumnName("NAME");
        });

        modelBuilder.Entity<StatisticsMonth>(entity =>
        {
            entity.HasKey(e => new { e.SaveTime, e.Ceqid })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity.ToTable("statistics_month", tb => tb.HasComment("전류 월 통계 저장 테이블"));

            entity.Property(e => e.SaveTime)
                .HasComment("DB 기록시간")
                .HasColumnType("datetime")
                .HasColumnName("SAVE_TIME");
            entity.Property(e => e.Ceqid)
                .HasComment("단말장치 ID")
                .HasColumnType("int(11)")
                .HasColumnName("CEQID");
            entity.Property(e => e.AverageCurrentA)
                .HasComment("전류A-평균")
                .HasColumnName("AVERAGE_CURRENT_A");
            entity.Property(e => e.AverageCurrentB)
                .HasComment("전류B-평균")
                .HasColumnName("AVERAGE_CURRENT_B");
            entity.Property(e => e.AverageCurrentC)
                .HasComment("전류C-평균")
                .HasColumnName("AVERAGE_CURRENT_C");
            entity.Property(e => e.AverageCurrentN)
                .HasComment("전류N-평균")
                .HasColumnName("AVERAGE_CURRENT_N");
            entity.Property(e => e.Circuitno)
                .HasComment("회로번호")
                .HasColumnType("int(11)")
                .HasColumnName("CIRCUITNO");
            entity.Property(e => e.Cpsid)
                .HasComment("Composite ID")
                .HasColumnType("int(11)")
                .HasColumnName("CPSID");
            entity.Property(e => e.Dl)
                .HasMaxLength(64)
                .HasComment("소속DL")
                .HasColumnName("DL");
            entity.Property(e => e.MaxCommTime)
                .HasComment("최대 수집시간")
                .HasColumnType("datetime")
                .HasColumnName("MAX_COMM_TIME");
            entity.Property(e => e.MaxCurrentA)
                .HasComment("전류A-최대")
                .HasColumnName("MAX_CURRENT_A");
            entity.Property(e => e.MaxCurrentB)
                .HasComment("전류B-최대")
                .HasColumnName("MAX_CURRENT_B");
            entity.Property(e => e.MaxCurrentC)
                .HasComment("전류C-최대")
                .HasColumnName("MAX_CURRENT_C");
            entity.Property(e => e.MaxCurrentN)
                .HasComment("전류N-최대")
                .HasColumnName("MAX_CURRENT_N");
            entity.Property(e => e.MinCommTime)
                .HasComment("최소 수집시간")
                .HasColumnType("datetime")
                .HasColumnName("MIN_COMM_TIME");
            entity.Property(e => e.MinCurrentA)
                .HasComment("전류A - 최소")
                .HasColumnName("MIN_CURRENT_A");
            entity.Property(e => e.MinCurrentB)
                .HasComment("전류B - 최소")
                .HasColumnName("MIN_CURRENT_B");
            entity.Property(e => e.MinCurrentC)
                .HasComment("전류C - 최소")
                .HasColumnName("MIN_CURRENT_C");
            entity.Property(e => e.MinCurrentN)
                .HasComment("전류N - 최소")
                .HasColumnName("MIN_CURRENT_N");
            entity.Property(e => e.Name)
                .HasMaxLength(128)
                .HasComment("단말장치 명")
                .HasColumnName("NAME");
        });

        modelBuilder.Entity<StatisticsYear>(entity =>
        {
            entity.HasKey(e => new { e.SaveTime, e.Ceqid })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity.ToTable("statistics_year", tb => tb.HasComment("전류 년 통계 저장 테이블"));

            entity.Property(e => e.SaveTime)
                .HasComment("DB 기록시간")
                .HasColumnType("datetime")
                .HasColumnName("SAVE_TIME");
            entity.Property(e => e.Ceqid)
                .HasComment("단말장치 ID")
                .HasColumnType("int(11)")
                .HasColumnName("CEQID");
            entity.Property(e => e.AverageCurrentA)
                .HasComment("전류A-평균")
                .HasColumnName("AVERAGE_CURRENT_A");
            entity.Property(e => e.AverageCurrentB)
                .HasComment("전류B-평균")
                .HasColumnName("AVERAGE_CURRENT_B");
            entity.Property(e => e.AverageCurrentC)
                .HasComment("전류C-평균")
                .HasColumnName("AVERAGE_CURRENT_C");
            entity.Property(e => e.AverageCurrentN)
                .HasComment("전류N-평균")
                .HasColumnName("AVERAGE_CURRENT_N");
            entity.Property(e => e.Circuitno)
                .HasComment("회로번호")
                .HasColumnType("int(11)")
                .HasColumnName("CIRCUITNO");
            entity.Property(e => e.Cpsid)
                .HasComment("Composite ID")
                .HasColumnType("int(11)")
                .HasColumnName("CPSID");
            entity.Property(e => e.Dl)
                .HasMaxLength(64)
                .HasComment("소속DL")
                .HasColumnName("DL");
            entity.Property(e => e.MaxCommTime)
                .HasComment("최대 수집시간")
                .HasColumnType("datetime")
                .HasColumnName("MAX_COMM_TIME");
            entity.Property(e => e.MaxCurrentA)
                .HasComment("전류A-최대")
                .HasColumnName("MAX_CURRENT_A");
            entity.Property(e => e.MaxCurrentB)
                .HasComment("전류B-최대")
                .HasColumnName("MAX_CURRENT_B");
            entity.Property(e => e.MaxCurrentC)
                .HasComment("전류C-최대")
                .HasColumnName("MAX_CURRENT_C");
            entity.Property(e => e.MaxCurrentN)
                .HasComment("전류N-최대")
                .HasColumnName("MAX_CURRENT_N");
            entity.Property(e => e.MinCommTime)
                .HasComment("최소 수집시간")
                .HasColumnType("datetime")
                .HasColumnName("MIN_COMM_TIME");
            entity.Property(e => e.MinCurrentA)
                .HasComment("전류A - 최소")
                .HasColumnName("MIN_CURRENT_A");
            entity.Property(e => e.MinCurrentB)
                .HasComment("전류B - 최소")
                .HasColumnName("MIN_CURRENT_B");
            entity.Property(e => e.MinCurrentC)
                .HasComment("전류C - 최소")
                .HasColumnName("MIN_CURRENT_C");
            entity.Property(e => e.MinCurrentN)
                .HasComment("전류N - 최소")
                .HasColumnName("MIN_CURRENT_N");
            entity.Property(e => e.Name)
                .HasMaxLength(128)
                .HasComment("단말장치 명")
                .HasColumnName("NAME");
        });

        modelBuilder.Entity<StorageInfo>(entity =>
        {
            entity.HasKey(e => e.StorageId).HasName("PRIMARY");

            entity.ToTable("storage_info", tb => tb.HasComment("보관주기 정보 테이블"));

            entity.Property(e => e.StorageId)
                .ValueGeneratedNever()
                .HasComment("보관주기 아이디")
                .HasColumnType("int(11)")
                .HasColumnName("STORAGE_ID");
            entity.Property(e => e.Desc)
                .HasMaxLength(64)
                .HasComment("보관주기 설명")
                .HasColumnName("DESC");
            entity.Property(e => e.SchduleType)
                .HasComment("보관주기 설정 타입")
                .HasColumnType("int(11)")
                .HasColumnName("SCHDULE_TYPE");
            entity.Property(e => e.StorageValue)
                .HasMaxLength(10)
                .HasComment("보관주기 설정 주기")
                .HasColumnName("STORAGE_VALUE");
        });

        modelBuilder.Entity<StorageType>(entity =>
        {
            entity.HasKey(e => e.StorageType1).HasName("PRIMARY");

            entity.ToTable("storage_type", tb => tb.HasComment("보관주기 타입 테이블"));

            entity.Property(e => e.StorageType1)
                .ValueGeneratedNever()
                .HasComment("보관주기 타입")
                .HasColumnType("int(11)")
                .HasColumnName("STORAGE_TYPE");
            entity.Property(e => e.Name)
                .HasMaxLength(32)
                .HasComment("타입 이름")
                .HasColumnName("NAME");
        });

        modelBuilder.Entity<Subgeographicalregion>(entity =>
        {
            entity.HasKey(e => e.Sgrid).HasName("PRIMARY");

            entity.ToTable("subgeographicalregion", tb => tb.HasComment("지점 테이블"));

            entity.Property(e => e.Sgrid)
                .ValueGeneratedNever()
                .HasComment("지점 아이디")
                .HasColumnType("bigint(20)")
                .HasColumnName("SGRID");
            entity.Property(e => e.GgrFk)
                .HasComment("GEOGRAPHICALREGION의 GGRID")
                .HasColumnType("int(11)")
                .HasColumnName("GGR_FK");
            entity.Property(e => e.Name)
                .HasMaxLength(32)
                .HasComment("지점 이름")
                .HasColumnName("NAME");
        });

        modelBuilder.Entity<Substation>(entity =>
        {
            entity.HasKey(e => e.Stid).HasName("PRIMARY");

            entity.ToTable("substation", tb => tb.HasComment("변전소 정보 테이블"));

            entity.Property(e => e.Stid)
                .ValueGeneratedNever()
                .HasComment("변전소 ID")
                .HasColumnType("bigint(20)")
                .HasColumnName("STID");
            entity.Property(e => e.Name)
                .HasMaxLength(32)
                .HasComment("변전소 명")
                .HasColumnName("NAME");
            entity.Property(e => e.SgrFk)
                .HasComment("소속 지점 ID")
                .HasColumnType("bigint(20)")
                .HasColumnName("SGR_FK");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
