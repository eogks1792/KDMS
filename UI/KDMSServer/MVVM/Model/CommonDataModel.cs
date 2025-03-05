using KDMS.EF.Core.Contexts;
using KDMS.EF.Core.Extensions;
using KDMS.EF.Core.Infrastructure.Reverse;
using KDMS.EF.Core.Infrastructure.Reverse.Models;
using KdmsTcpSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System.Data;
using System.IO;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace KDMSServer.Model
{
    public class CommonDataModel
    {
        private readonly KdmsContext _kdmsContext;
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;

        public List<SchduleInfo> SchduleInfos { get; set; }
        public List<StorageInfo> StorageInfos { get; set; }
        public List<StorageType> StorageTypes { get; set; }
        public List<PdbList> pdbLists { get; set; }
        private List<AiInfo> AiInfos { get; set; }
        private List<AlarmInfo> AlarmInfos { get; set; }
        //private List<PdbRemoteunit> Remoteunits { get; set; }

        // PDB 데이터
        public List<pdb_Discrete> pdbDiscretes = new List<pdb_Discrete>();
        public List<pdb_Command> pdbCommands = new List<pdb_Command>();
        public List<pdb_Analog> pdbAnalogs = new List<pdb_Analog>();
        public List<pdb_SetPoint> pdbSetPoints = new List<pdb_SetPoint>();
        public List<pdb_Accumulator> pdbAccumulators = new List<pdb_Accumulator>();
        public List<pdb_Dmc> pdbDmcs = new List<pdb_Dmc>();
        public List<rtdb_Analog> rtdbAnalogs = new List<rtdb_Analog>();
        public List<rtdb_Dmc> rtdbDmcs = new List<rtdb_Dmc>();

        // PDB 데이터 => 데이터베이스 저장필요
        public List<pdb_RemoteUnit> pdbRemoteUnits = new List<pdb_RemoteUnit>();
        public List<pdb_CompositeSwitch> pdbCompositeSwitchs = new List<pdb_CompositeSwitch>();
        public List<pdb_ConductingEquipment> pdbConductingequipments = new List<pdb_ConductingEquipment>();
        public List<pdb_DistributionLineSegment> pdbDistributionLineSegments = new List<pdb_DistributionLineSegment>();
        public List<pdb_GeographicalRegion> pdbGeographicalRegions = new List<pdb_GeographicalRegion>();
        public List<pdb_SubGeographicalRegion> pdbSubGeographicalRegions = new List<pdb_SubGeographicalRegion>();
        public List<pdb_SubStation> pdbSubStations = new List<pdb_SubStation>();
        public List<pdb_DistributionLine> pdbDistributionLines = new List<pdb_DistributionLine>();
        public List<pdb_PowerTransformer> pdbPowerTransformers = new List<pdb_PowerTransformer>();

        private string BackupPath { get; set; }

        private DataWorker worker => App.Current.Services.GetService<DataWorker>()!;

        public CommonDataModel(KdmsContext kdmsContext, IConfiguration configuration, ILogger logger)
        {
            _kdmsContext = kdmsContext;
            _configuration = configuration;
            _logger = logger;

            DataLoad();
        }

        private void DataLoad()
        {
            SchduleInfos = _kdmsContext.SchduleInfos.ToList();
            StorageInfos = _kdmsContext.StorageInfos.ToList();
            StorageTypes = _kdmsContext.StorageTypes.ToList();
            pdbLists = _kdmsContext.PdbLists.ToList();

            AiInfos = _kdmsContext.AiInfos.ToList();
            AlarmInfos = _kdmsContext.AlarmInfos.ToList();
            //Remoteunits = _kdmsContext.PdbRemoteunits.Where(p => p.DmcFk != 0).ToList();

            BackupPath = _configuration.GetSection("FileBackupPath").Value ?? $"{AppDomain.CurrentDomain.BaseDirectory}FileSave";
            if (!Directory.Exists(BackupPath))
                Directory.CreateDirectory(BackupPath);
        }

        public string PdbFileName(int pdbId)
        {
            var find = pdbLists.FirstOrDefault(p => p.Pid == pdbId);
            if (find == null)
                return $"unknown_{pdbId}";
            else
                return find.Name ?? string.Empty;
        }

        public void PdbListSave(List<PdbListModel> pdbInfos)
        {
            using (KdmsContext context = DbContextConfigurationExtensions.CreateServerContext(_configuration))
            {
                foreach (PdbListModel pdb in pdbInfos)
                {
                    var find = context.PdbLists.Where(p => p.UseYn == true).FirstOrDefault(p => p.Pid == pdb.PdbId);
                    if (find != null)
                    {
                        if (find.Md5 != pdb.PdbMd5)
                            find.Md5 = pdb.PdbMd5;
                    }
                }
                context.SaveChanges();
            }
        }

        private DateTime GetMinDataTime(List<pdb_Analog> analogList, int dataPointId, DateTime now)
        {
            if (rtdbAnalogs.Count <= 0)
                return now;

            var find = analogList.FirstOrDefault(p => p.dp_fk == dataPointId);
            if (find.pid <= 0)
                return now;

            var findrtdb = rtdbAnalogs.FirstOrDefault(p => p.pid == find.pid);
            if (findrtdb.pid <= 0)
                return now;

            return KdmsValueConverter.TimeTToDateTime(findrtdb.last_update);
        }

        private int GetMinDataCircuitno(List<pdb_Analog> analogList, int dataPointId)
        {
            var find = analogList.FirstOrDefault(p => p.dp_fk == dataPointId);
            if (find.pid <= 0)
                return 0;

            return (int)find.circuit_no;
        }

        private float GetMinDataValue(List<pdb_Analog> analogList, int dataPointId)
        {
            if (rtdbAnalogs.Count <= 0)
                return 0;

            var find = analogList.FirstOrDefault(p => p.dp_fk == dataPointId);
            if (find.pid <= 0)
                return 0;

            var findrtdb = rtdbAnalogs.FirstOrDefault(p => p.pid == find.pid);
            if (findrtdb.pid <= 0)
                return 0;

            return (float)findrtdb.value;
        }

        private float GetAlarmDataValue(List<pdb_Analog> analogList, int dataPointId)
        {
            if (rtdbAnalogs.Count <= 0)
                return 0;

            var find = analogList.FirstOrDefault(p => p.dp_fk == dataPointId);
            if (find.pid <= 0)
                return 0;

            var findrtdb = rtdbAnalogs.FirstOrDefault(p => p.pid == find.pid);
            if (findrtdb.pid <= 0)
                return 0;

            return (float)findrtdb.value;
        }

        public void MinDataSave(DateTime date/*, List<rtdb_Analog> rtList, List<pdb_Analog> analogList, List<pdb_ConductingEquipment> equipmentList*/)
        {
            var mappingInfos = _configuration.GetSection("MappingInfo");
            var circuitnoId = Convert.ToInt32(mappingInfos.GetSection("Circuitno").Value ?? "38");
            var commTimeId = Convert.ToInt32(mappingInfos.GetSection("CommTime").Value ?? "38");

            List<HistoryMinDatum> dataList = new List<HistoryMinDatum>();
            foreach (var equipment in pdbConductingequipments)
            {
                if (equipment.ceqid <= 0)
                    continue;

                if (equipment.rtu_type != 2)    // RTU 타입이2 자동
                    continue;

                var findRemoteunit = pdbRemoteUnits.FirstOrDefault(p => p.eq_type == 1 && p.eq_fk == equipment.ceqid);
                if (findRemoteunit.pid > 0)
                {
                    if (findRemoteunit.protocol_fk == 0 && findRemoteunit.comm_type == 0)
                        continue;
                }

                findRemoteunit = pdbRemoteUnits.FirstOrDefault(p => p.eq_type == 2 && p.eq_fk == equipment.ec_fk);
                if (findRemoteunit.pid > 0)
                {
                    if (findRemoteunit.protocol_fk == 0 && findRemoteunit.comm_type == 0)
                        continue;
                }

                var findDl = pdbDistributionLines.FirstOrDefault(p => p.dlid == equipment.dl_fk);
                if (findDl.dlid <= 0)
                {
                    _logger.Debug($"[1분 실시간] DistributionLine DlId:0 / Equipment DlFk:{equipment.dl_fk}");
                    continue;
                }

                var ceqAnalogList = pdbAnalogs.Where(p => p.ceq_fk == equipment.ceqid).ToList();
                HistoryMinDatum data = new HistoryMinDatum();
                foreach (var ai in AiInfos)
                {
                    switch (ai.No)
                    {
                        case 1:
                            data.SaveTime = date;
                            break;
                        case 2:
                            data.Ceqid = (int)equipment.ceqid;
                            break;
                        case 3:
                            data.CommTime = GetMinDataTime(ceqAnalogList, commTimeId, date);
                            break;
                        case 4:
                            data.Cpsid = (int)equipment.ec_fk;
                            break;
                        case 5:
                            data.Circuitno = GetMinDataCircuitno(ceqAnalogList, circuitnoId);
                            break;
                        case 6:
                            data.Name = GetStringData(equipment.name);
                            break;
                        case 7:
                            data.Dl = GetStringData(findDl.name);
                            break;
                        case 8:
                            data.Diagnostics = (int)GetMinDataValue(ceqAnalogList, ai.Datapointid ?? 0);
                            break;
                        case 9:
                            data.VoltageUnbalance = (int)GetMinDataValue(ceqAnalogList, ai.Datapointid ?? 0);
                            break;
                        case 10:
                            data.CurrentUnbalance = (int)GetMinDataValue(ceqAnalogList, ai.Datapointid ?? 0);
                            break;
                        case 11:
                            data.Frequency = GetMinDataValue(ceqAnalogList, ai.Datapointid ?? 0);
                            break;
                        case 12:
                            data.CurrentA = GetMinDataValue(ceqAnalogList, ai.Datapointid ?? 0);
                            break;
                        case 13:
                            data.CurrentB = GetMinDataValue(ceqAnalogList, ai.Datapointid ?? 0);
                            break;
                        case 14:
                            data.CurrentC = GetMinDataValue(ceqAnalogList, ai.Datapointid ?? 0);
                            break;
                        case 15:
                            data.CurrentN = GetMinDataValue(ceqAnalogList, ai.Datapointid ?? 0);
                            break;
                        case 16:
                            data.VoltageA = GetMinDataValue(ceqAnalogList, ai.Datapointid ?? 0);
                            break;
                        case 17:
                            data.VoltageB = GetMinDataValue(ceqAnalogList, ai.Datapointid ?? 0);
                            break;
                        case 18:
                            data.VoltageC = GetMinDataValue(ceqAnalogList, ai.Datapointid ?? 0);
                            break;
                        case 19:
                            data.ApparentPowerA = GetMinDataValue(ceqAnalogList, ai.Datapointid ?? 0);
                            break;
                        case 20:
                            data.ApparentPowerB = GetMinDataValue(ceqAnalogList, ai.Datapointid ?? 0);
                            break;
                        case 21:
                            data.ApparentPowerC = GetMinDataValue(ceqAnalogList, ai.Datapointid ?? 0);
                            break;
                        case 22:
                            data.PowerFactor3p = (int)GetMinDataValue(ceqAnalogList, ai.Datapointid ?? 0);
                            break;
                        case 23:
                            data.PowerFactorA = (int)GetMinDataValue(ceqAnalogList, ai.Datapointid ?? 0);
                            break;
                        case 24:
                            data.PowerFactorB = (int)GetMinDataValue(ceqAnalogList, ai.Datapointid ?? 0);
                            break;
                        case 25:
                            data.PowerFactorC = (int)GetMinDataValue(ceqAnalogList, ai.Datapointid ?? 0);
                            break;
                        case 26:
                            data.FaultCurrentA = GetMinDataValue(ceqAnalogList, ai.Datapointid ?? 0);
                            break;
                        case 27:
                            data.FaultCurrentB = GetMinDataValue(ceqAnalogList, ai.Datapointid ?? 0);
                            break;
                        case 28:
                            data.FaultCurrentC = GetMinDataValue(ceqAnalogList, ai.Datapointid ?? 0);
                            break;
                        case 29:
                            data.FaultCurrentN = GetMinDataValue(ceqAnalogList, ai.Datapointid ?? 0);
                            break;
                        case 30:
                            data.CurrentPhaseA = GetMinDataValue(ceqAnalogList, ai.Datapointid ?? 0);
                            break;
                        case 31:
                            data.CurrentPhaseB = GetMinDataValue(ceqAnalogList, ai.Datapointid ?? 0);
                            break;
                        case 32:
                            data.CurrentPhaseC = GetMinDataValue(ceqAnalogList, ai.Datapointid ?? 0);
                            break;
                        case 33:
                            data.CurrentPhaseN = GetMinDataValue(ceqAnalogList, ai.Datapointid ?? 0);
                            break;
                        case 34:
                            data.VoltagePhaseA = GetMinDataValue(ceqAnalogList, ai.Datapointid ?? 0);
                            break;
                        case 35:
                            data.VoltagePhaseB = GetMinDataValue(ceqAnalogList, ai.Datapointid ?? 0);
                            break;
                        case 36:
                            data.VoltagePhaseC = GetMinDataValue(ceqAnalogList, ai.Datapointid ?? 0);
                            break;
                    }
                }

                dataList.Add(data);
                //try
                //{
                //    string query = $"insert into history_min_data_{date.AddDays(1).ToString("yyyyMMdd")}" +
                //        $" values('{data.SaveTime.ToString("yyyy-MM-dd HH:mm:ss")}', {data.Ceqid}, '{data.CommTime?.ToString("yyyy-MM-dd HH:mm:ss")}', {data.Cpsid}, {data.Circuitno}, '{data.Name?.Trim()}', '{data.Dl?.Trim()}'," +
                //        $" {data.Diagnostics}, {data.VoltageUnbalance}, {data.CurrentUnbalance}, {data.Frequency}," +
                //        $" {data.CurrentA}, {data.CurrentB}, {data.CurrentC}, {data.CurrentN}, {data.VoltageA}, {data.VoltageB}, {data.VoltageC}," +
                //        $" {data.ApparentPowerA}, {data.ApparentPowerB}, {data.ApparentPowerC}, {data.PowerFactor3p}, {data.PowerFactorA}, {data.PowerFactorB}, {data.PowerFactorC}," +
                //        $" {data.FaultCurrentA}, {data.FaultCurrentB}, {data.FaultCurrentC}, {data.FaultCurrentN}," +
                //        $" {data.CurrentPhaseA}, {data.CurrentPhaseB}, {data.CurrentPhaseC}, {data.CurrentPhaseN}," +
                //        $" {data.VoltagePhaseA}, {data.VoltagePhaseB}, {data.VoltagePhaseC});";

                //    using (MySqlMapper mapper = new MySqlMapper(_configuration))
                //    {
                //        mapper.RunQuery(query);
                //    }
                //}
                //catch (Exception ex)
                //{
                //    _logger.ServerLog($"[1분 실시간] {date.ToString("yyyy-MM-dd HH:mm:ss")} 데이터 입력중 예외 발생 ex:{ex.Message}");
                //}
            }

            if (dataList.Count > 0)
            {
                try
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine($"insert into history_min_data_{date.ToString("yyyyMMdd")} values ");
                    int cnt = 0;
                    foreach (var data in dataList)
                    {
                        cnt++;
                        sb.Append($"('{data.SaveTime.ToString("yyyy-MM-dd HH:mm:ss")}', {data.Ceqid}, '{data.CommTime?.ToString("yyyy-MM-dd HH:mm:ss")}', {data.Cpsid}, {data.Circuitno}, '{data.Name?.Trim()}', '{data.Dl?.Trim()}'," +
                           $" {data.Diagnostics}, {data.VoltageUnbalance}, {data.CurrentUnbalance}, {data.Frequency}," +
                           $" {data.CurrentA}, {data.CurrentB}, {data.CurrentC}, {data.CurrentN}, {data.VoltageA}, {data.VoltageB}, {data.VoltageC}," +
                           $" {data.ApparentPowerA}, {data.ApparentPowerB}, {data.ApparentPowerC}, {data.PowerFactor3p}, {data.PowerFactorA}, {data.PowerFactorB}, {data.PowerFactorC}," +
                           $" {data.FaultCurrentA}, {data.FaultCurrentB}, {data.FaultCurrentC}, {data.FaultCurrentN}," +
                           $" {data.CurrentPhaseA}, {data.CurrentPhaseB}, {data.CurrentPhaseC}, {data.CurrentPhaseN}," +
                           $" {data.VoltagePhaseA}, {data.VoltagePhaseB}, {data.VoltagePhaseC})");

                        if (cnt != dataList.Count)
                            sb.AppendLine(",");
                        else
                            sb.AppendLine(";");
                    }

                    using (MySqlMapper mapper = new MySqlMapper(_configuration))
                    {
                        mapper.RunQuery(sb.ToString());
                        _logger.ServerLog($"[1분 실시간] {date.ToString("yyyy-MM-dd HH:mm:ss")} 데이터 입력 완료 CNT:{dataList.Count} ");
                    }
                }
                catch (Exception ex)
                {
                    _logger.ServerLog($"[1분 실시간] {date.ToString("yyyy-MM-dd HH:mm:ss")} 데이터 입력중 예외 발생 ex:{ex.Message}");
                }
            }
        }

        public void StatisticsMinDataSave(DateTime date/*, List<rtdb_Analog> rtList, List<pdb_Analog> analogList, List<pdb_ConductingEquipment> equipmentList*/)
        {
            var mappingInfos = _configuration.GetSection("MappingInfo");
            var circuitnoId = Convert.ToInt32(mappingInfos.GetSection("Circuitno").Value ?? "38");
            var commTimeId = Convert.ToInt32(mappingInfos.GetSection("CommTime").Value ?? "38");
            var currentAId = Convert.ToInt32(mappingInfos.GetSection("AverageCurrentA").Value ?? "45");
            var currentBId = Convert.ToInt32(mappingInfos.GetSection("AverageCurrentB").Value ?? "46");
            var currentCId = Convert.ToInt32(mappingInfos.GetSection("AverageCurrentC").Value ?? "47");
            var currentNId = Convert.ToInt32(mappingInfos.GetSection("AverageCurrentN").Value ?? "65");

            List<Statistics15min> dataList = new List<Statistics15min>();
            foreach (var equipment in pdbConductingequipments)
            {
                if (equipment.ceqid <= 0)
                    continue;

                if (equipment.rtu_type != 2)    // RTU 타입이2 자동
                    continue;

                var findRemoteunit = pdbRemoteUnits.FirstOrDefault(p => p.eq_type == 1 && p.eq_fk == equipment.ceqid);
                if (findRemoteunit.pid > 0)
                {
                    if (findRemoteunit.protocol_fk == 0 && findRemoteunit.comm_type == 0)
                        continue;
                }

                findRemoteunit = pdbRemoteUnits.FirstOrDefault(p => p.eq_type == 2 && p.eq_fk == equipment.ec_fk);
                if (findRemoteunit.pid > 0)
                {
                    if (findRemoteunit.protocol_fk == 0 && findRemoteunit.comm_type == 0)
                        continue;
                }

                var findDl = pdbDistributionLines.FirstOrDefault(p => p.dlid == equipment.dl_fk);
                if (findDl.dlid <= 0)
                {
                    _logger.Debug($"[15분 실시간(평균부하전류)] DistributionLine DlId:0 / Equipment DlFk:{equipment.dl_fk}");
                    continue;
                }

                var ceqAnalogList = pdbAnalogs.Where(p => p.ceq_fk == equipment.ceqid).ToList();

                Statistics15min data = new Statistics15min();
                data.SaveTime = date;
                data.Ceqid = (int)equipment.ceqid;
                data.Cpsid = (int)equipment.ec_fk;
                data.Circuitno = GetMinDataCircuitno(ceqAnalogList, circuitnoId);
                data.Name = GetStringData(equipment.name);
                data.Dl = GetStringData(findDl.name);
                data.AverageCurrentA = GetMinDataValue(ceqAnalogList, currentAId);
                data.AverageCurrentB = GetMinDataValue(ceqAnalogList, currentBId);
                data.AverageCurrentC = GetMinDataValue(ceqAnalogList, currentCId);
                data.AverageCurrentN = GetMinDataValue(ceqAnalogList, currentNId);
                data.CommTime = GetMinDataTime(ceqAnalogList, commTimeId, date);

                dataList.Add(data);

                //try
                //{
                //    string query = $"insert into statistics_15min" +
                //        $" values('{data.SaveTime.ToString("yyyy-MM-dd HH:mm:ss")}', {data.Ceqid}, {data.Cpsid}, {data.Circuitno}, '{data.Name?.Trim()}', '{data.Dl?.Trim()}'," +
                //        $" {data.AverageCurrentA}, {data.AverageCurrentB}, {data.AverageCurrentC}, {data.AverageCurrentN}, '{data.CommTime.ToString("yyyy-MM-dd HH:mm:ss")}');";

                //    using (MySqlMapper mapper = new MySqlMapper(_configuration))
                //    {
                //        mapper.RunQuery(query);
                //    }
                //}
                //catch (Exception ex)
                //{
                //    _logger.ServerLog($"[15분 실시간(평균부하전류)] {date.ToString("yyyy-MM-dd HH:mm:ss")} 데이터 입력중 예외 발생 ex:{ex.Message}");
                //}
            }

            if (dataList.Count > 0)
            {
                try
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine($"insert into statistics_15min values ");
                    int cnt = 0;
                    foreach (var data in dataList)
                    {
                        cnt++;
                        sb.Append($"('{data.SaveTime.ToString("yyyy-MM-dd HH:mm:ss")}', {data.Ceqid}, {data.Cpsid}, {data.Circuitno}, '{data.Name?.Trim()}', '{data.Dl?.Trim()}'," +
                            $" {data.AverageCurrentA}, {data.AverageCurrentB}, {data.AverageCurrentC}, {data.AverageCurrentN}, '{data.CommTime.ToString("yyyy-MM-dd HH:mm:ss")}')");

                        if (cnt != dataList.Count)
                            sb.AppendLine(",");
                        else
                            sb.AppendLine(";");
                    }

                    using (MySqlMapper mapper = new MySqlMapper(_configuration))
                    {
                        mapper.RunQuery(sb.ToString());
                        _logger.ServerLog($"[15분 실시간(평균부하전류)] {date.ToString("yyyy-MM-dd HH:mm:ss")} 데이터 입력 완료 CNT:{dataList.Count}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.ServerLog($"[15분 실시간(평균부하전류)] {date.ToString("yyyy-MM-dd HH:mm:ss")} 데이터 입력중 예외 발생 ex:{ex.Message}");
                }
            }

            //if (dataList.Count > 0)
            //{
            //    try
            //    {
            //        StringBuilder sb = new StringBuilder();
            //        foreach (var data in dataList)
            //        {
            //            sb.AppendLine($"insert into statistics_15min" +
            //                $" values('{data.SaveTime.ToString("yyyy-MM-dd HH:mm:ss")}', {data.Ceqid}, {data.Cpsid}, {data.Circuitno}, '{data.Name?.Trim()}', '{data.Dl?.Trim()}'," +
            //                $" {data.AverageCurrentA}, {data.AverageCurrentB}, {data.AverageCurrentC}, {data.AverageCurrentN}, '{data.CommTime.ToString("yyyy-MM-dd HH:mm:ss")}');");
            //        }

            //        using (MySqlMapper mapper = new MySqlMapper(_configuration))
            //        {
            //            mapper.RunQuery(sb.ToString());
            //            _logger.ServerLog($"[15분 실시간(평균부하전류)] {date.ToString("yyyy-MM-dd HH:mm:ss")} 데이터 CNT:{dataList.Count} 입력 완료");
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        _logger.ServerLog($"[15분 실시간(평균부하전류)] {date.ToString("yyyy-MM-dd HH:mm:ss")} 데이터 입력중 예외 발생 ex:{ex.Message}");
            //    }
            //}
        }

        private bool GetCommStateValue(int dmcFk)
        {
            if (rtdbDmcs.Count <= 0)
                return false;

            // 0: Offie Line
            // 1: Fail
            // 2: Disable
            // 3: Fail Back
            // 4: OnLine

            var find = rtdbDmcs.FirstOrDefault(p => p.pid == dmcFk);
            if (find.pid <= 0)
                return false;

            //_logger.ServerLog($"[통신상태 이력] DMCFK: {dmcFk} VALUE:{findState}");
            if (find.value == 4)
                return true;
            else
                return false;
        }

        private float GetCommDataValue(int commDmcFk)
        {
            if (rtdbDmcs.Count <= 0)
                return 0;

            var find = rtdbDmcs.FirstOrDefault(p => p.pid == commDmcFk);
            if (find.pid <= 0)
                return 0;

            return (float)find.value;
        }

        private DateTime GetCommDataTime(int dmcFk, DateTime now)
        {
            if (rtdbDmcs.Count <= 0)
                return now;

            var find = rtdbDmcs.FirstOrDefault(p => p.pid == dmcFk);
            if (find.pid <= 0)
                return now;

            return KdmsValueConverter.TimeTToDateTime(find.last_update);
        }

        public void CommStateDataSave(DateTime date/*, List<rtdb_Dmc> rtList, List<pdb_ConductingEquipment> equipmentList*/)
        {
            List<HistoryCommState> dataList = new List<HistoryCommState>();
            foreach (var remote in pdbRemoteUnits)
            {
                if (remote.protocol_fk == 0 && remote.comm_type == 0)
                    continue;

                if (remote.eq_type == 1)
                {
                    var equipment = pdbConductingequipments.FirstOrDefault(p => p.ceqid != 0 && p.ceqid == remote.eq_fk);
                    if (equipment.ceqid <= 0)
                    {
                        _logger.Debug($"[통신 성공률] Conductingequipment CEQID:0 / Remote EqFk:{remote.eq_fk}");
                        continue;
                    }

                    if (equipment.rtu_type != 2)    // RTU 타입이2 자동
                        continue;

                    var findDl = pdbDistributionLines.FirstOrDefault(p => p.dlid == equipment.dl_fk);
                    if (findDl.dlid <= 0)
                    {
                        _logger.Debug($"[통신 성공률] DistributionLine DlId:0 / Equipment DlFk:{equipment.dl_fk}");
                        continue;
                    }

                    HistoryCommState data = new HistoryCommState();
                    data.SaveTime = date;
                    data.Dl = GetStringData(findDl.name);
                    data.Name = GetStringData(equipment.name);

                    data.EqType = (int)remote.eq_fk;
                    data.Ceqid = (int)equipment.ceqid;
                    data.Cpsid = 0;

                    data.CommSucessCount = (int)GetCommDataValue(Convert.ToInt32(remote.comm_dmc_fk + 3000));
                    data.CommFailCount = (int)GetCommDataValue(Convert.ToInt32(remote.comm_dmc_fk + 6000));
                    data.CommTotalCount = data.CommSucessCount + data.CommFailCount;
                    data.CommSucessRate = GetCommDataValue(Convert.ToInt32(remote.comm_dmc_fk));
                    data.CommTime = GetCommDataTime(Convert.ToInt32(remote.comm_dmc_fk), date);

                    dataList.Add(data);
                }
                else
                {
                    var findList = pdbConductingequipments.Where(p => p.ceqid != 0 && p.ec_fk == remote.eq_fk).OrderBy(p => p.ceqid).ToList();
                    if (findList.Count <= 0)
                    {
                        _logger.Debug($"[통신 성공률] Conductingequipment CEQID:0 / Remote EqFk:{remote.eq_fk}");
                        continue;
                    }

                    var equipment = findList.FirstOrDefault();
                    if (equipment.ceqid <= 0)
                    {
                        _logger.Debug($"[통신 성공률] Conductingequipment CEQID:0 / Remote EqFk:{remote.eq_fk} First");
                        continue;
                    }

                    //foreach (var equipment in findList)
                    //{
                    var findDl = pdbDistributionLines.FirstOrDefault(p => p.dlid == equipment.dl_fk);
                    if (findDl.dlid <= 0)
                    {
                        _logger.Debug($"[통신 성공률] DistributionLine DlId:0 / Equipment DlFk:{equipment.dl_fk}");
                        continue;
                    }

                    if (equipment.rtu_type != 2)    // RTU 타입이2 자동
                        continue;

                    HistoryCommState data = new HistoryCommState();
                    data.SaveTime = date;
                    data.Dl = GetStringData(findDl.name);
                    data.Name = GetStringData(equipment.name);

                    data.EqType = (int)remote.eq_fk;
                    data.Ceqid = (int)equipment.ceqid;
                    data.Cpsid = (int)equipment.ec_fk;

                    data.CommSucessCount = (int)GetCommDataValue(Convert.ToInt32(remote.comm_dmc_fk + 3000));
                    data.CommFailCount = (int)GetCommDataValue(Convert.ToInt32(remote.comm_dmc_fk + 6000));
                    data.CommTotalCount = data.CommSucessCount + data.CommFailCount;
                    data.CommSucessRate = GetCommDataValue(Convert.ToInt32(remote.comm_dmc_fk));
                    data.CommTime = GetCommDataTime(Convert.ToInt32(remote.comm_dmc_fk), date);

                    dataList.Add(data);
                    //}
                }
            }

            //foreach (var equipment in equipmentList)
            //{
            //    if (equipment.ceqid <= 0)
            //        continue;

            //    var find = Remoteunits.FirstOrDefault(p => p.EqFk == equipment.eq_fk);
            //    if (find == null)
            //        continue;

            //    HistoryCommState data = new HistoryCommState();
            //    data.SaveTime = date;
            //    data.Name = equipment.name;
            //    data.Dl = Distributionlines.FirstOrDefault(p => p.Dlid == equipment.dl_fk)?.Name;

            //    data.EqType = find.EqType ?? 0;
            //    if (find.EqType == 1)   // CEQ ID
            //    {
            //        data.Ceqid = (int)equipment.ceqid;
            //        data.Cpsid = 0;
            //    }
            //    else // CPSID
            //    {
            //        data.Ceqid = 0;
            //        data.Cpsid = (int)equipment.ec_fk;
            //    }

            //    var commValue = GetCommDataValue(rtList, Convert.ToInt32(find.CommDmcFk ?? 0));

            //    data.CommSucessCount = (int)commValue + 3000;
            //    data.CommFailCount = (int)commValue + 6000;
            //    data.CommTotalCount = data.CommSucessCount + data.CommFailCount;


            //    data.CommSucessRate = GetCommDataValue(rtList, Convert.ToInt32(find.CommDmcFk ?? 0));
            //    data.CommTime = GetCommDataTime(rtList, Convert.ToInt32(find.CommDmcFk ?? 0), date);

            //    dataList.Add(data);
            //}

            if (dataList.Count > 0)
            {
                try
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine($"insert into history_comm_state values ");
                    int cnt = 0;
                    foreach (var data in dataList)
                    {
                        cnt++;
                        sb.Append($"('{data.SaveTime.ToString("yyyy-MM-dd HH:mm:ss")}', {data.EqType}, {data.Ceqid}, {data.Cpsid}, '{data.Name?.Trim()}', '{data.Dl?.Trim()}'," +
                            $" {data.CommTotalCount}, {data.CommSucessCount}, {data.CommFailCount}, {data.CommSucessRate:N2}, '{data.CommTime?.ToString("yyyy-MM-dd HH:mm:ss")}')");

                        if (cnt != dataList.Count)
                            sb.AppendLine(",");
                        else
                            sb.AppendLine(";");
                    }

                    using (MySqlMapper mapper = new MySqlMapper(_configuration))
                    {
                        mapper.RunQuery(sb.ToString());
                        _logger.ServerLog($"[통신 성공률] {date.ToString("yyyy-MM-dd HH:mm:ss")} 데이터 입력 완료 CNT:{dataList.Count}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.ServerLog($"[통신 성공률] {date.ToString("yyyy-MM-dd HH:mm:ss")} 데이터 입력중 예외 발생 ex:{ex.Message}");
                }
            }
        }

        //public void DMCDataView()
        //{
        //    foreach (var remote in pdbRemoteUnits)
        //    {
        //        var findDmc = rtdbDmcs.FirstOrDefault(p => p.pid == remote.dmc_fk);
        //        if (findDmc.pid > 0)
        //        {
        //            _logger.ServerLog($"PID:{findDmc.pid} VAL:{findDmc.value}");
        //        }
        //    }
        //}

        public void CommStateLogDataSave(List<rtdb_Alarm> alarmList)
        {
            var date = DateTime.Now;
            foreach (var alarm in alarmList)
            {
                //var findRemoteunit = pdbRemoteUnits.FirstOrDefault(p => p.pid == alarm.uiRtuid);
                //if (findRemoteunit.pid <= 0)
                //    _logger.Debug($"[통신상태 이력] Remoteunit RTUID:{alarm.uiRtuid} 없음 ");

                var findRemoteunit = pdbRemoteUnits.FirstOrDefault(p => p.dmc_fk == alarm.uiPid);
                if (findRemoteunit.pid <= 0)
                {
                    _logger.Debug($"[통신상태 이력] Remoteunit DMCFK:{alarm.uiPid} 없음 ");
                    continue;
                }

                var findDmc = rtdbDmcs.FirstOrDefault(p => p.pid == alarm.uiPid);
                if (findDmc.pid > 0)
                {
                    _logger.Debug($"[통신상태 이력] ALARM PID:{alarm.uiPid} DMCFK:{findRemoteunit.dmc_fk} Alarm Value:{alarm.fVal} → DMC Value:{findDmc.value} 변경 전 확인");
                    var index = rtdbDmcs.IndexOf(findDmc);
                    if (index >= 0)
                    {
                        findDmc.value = alarm.fVal;
                        rtdbDmcs[index] = findDmc;
                        _logger.Debug($"[통신상태 이력] ALARM PID:{alarm.uiPid} DMCFK:{findRemoteunit.dmc_fk} Alarm Value:{alarm.fVal} → DMC Value:{findDmc.value} 변경 후 확인");
                    }
                    else
                        _logger.Debug($"[통신상태 이력] ALARM PID:{alarm.uiPid} DMCFK:{findRemoteunit.dmc_fk} Alarm Value:{alarm.fVal} → DMC Value:{findDmc.value} 변경 실패 (찾기 실패)");
                }

                if (findRemoteunit.dmc_fk == 0 && findRemoteunit.comm_dmc_fk == 0)
                {
                    _logger.Debug($"[통신상태 이력] Remoteunit DMCFK:0 / COMMDMC_FK:0 → RTDM DMC PID NOT FOUND");
                    continue;
                }

                if (findRemoteunit.protocol_fk == 0 && findRemoteunit.comm_type == 0)
                    continue;

                pdb_ConductingEquipment equipment = new pdb_ConductingEquipment();
                if (findRemoteunit.eq_type == 1)
                    equipment = pdbConductingequipments.FirstOrDefault(p => findRemoteunit.eq_fk == p.ceqid);
                else
                    equipment = pdbConductingequipments.FirstOrDefault(p => findRemoteunit.eq_fk == p.ec_fk);

                if (equipment.ceqid <= 0)
                    continue;

                if (equipment.rtu_type != 2)    // RTU 타입이2 자동
                    continue;

                var findDl = pdbDistributionLines.FirstOrDefault(p => p.dlid == equipment.dl_fk);
                if (findDl.dlid <= 0)
                {
                    _logger.Debug($"[통신상태 이력] DistributionLine DlId:0 / Alarm DlId:{alarm.uiDL}");
                    continue;
                }

                HistoryCommStateLog data = new HistoryCommStateLog();
                data.SaveTime = date;
                data.Dl = GetStringData(findDl.name);
                data.Name = GetStringData(equipment.name);

                data.EqType = (int)findRemoteunit.eq_fk;
                data.Ceqid = (int)equipment.ceqid;
                data.Cpsid = (int)equipment.ec_fk;
                data.CommState = GetCommStateValue(Convert.ToInt32(findRemoteunit.dmc_fk));
                data.CommSucessCount = (int)GetCommDataValue(Convert.ToInt32(findRemoteunit.comm_dmc_fk + 3000));
                data.CommFailCount = (int)GetCommDataValue(Convert.ToInt32(findRemoteunit.comm_dmc_fk + 6000));
                data.CommTotalCount = data.CommSucessCount + data.CommFailCount;
                data.CommSucessRate = GetCommDataValue(Convert.ToInt32(findRemoteunit.comm_dmc_fk));
                data.CommTime = GetCommDataTime(Convert.ToInt32(findRemoteunit.dmc_fk), date);

                try
                {
                    string query = $"insert into history_comm_state_log values ('{data.SaveTime.ToString("yyyy-MM-dd HH:mm:ss")}', {data.EqType}, {data.Ceqid}, {data.Cpsid}, '{data.Name?.Trim()}', '{data.Dl?.Trim()}', {data.CommState}," +
                        $" {data.CommTotalCount}, {data.CommSucessCount}, {data.CommFailCount}, {data.CommSucessRate:N2}, '{data.CommTime?.ToString("yyyy-MM-dd HH:mm:ss")}')";

                    using (MySqlMapper mapper = new MySqlMapper(_configuration))
                    {
                        mapper.RunQuery(query);
                        _logger.ServerLog($"[통신상태 이력] CEQ:{data.Ceqid} 통신:{(data.CommState == true ? "성공" : "실패")} 데이터 입력 완료 ");
                    }
                }
                catch (Exception ex)
                {
                    _logger.ServerLog($"[통신상태 이력] 데이터 입력중 예외 발생 ex:{ex.Message}");
                }

            }
        }

        private bool AlarmFilterCheck(int pointType, int pointId)
        {
            bool retValue = false;
            int dataPointId = 0;
            switch (pointType)
            {
                case (int)PointTypeCode.BI:
                    {
                        var find = pdbDiscretes.FirstOrDefault(p => p.pid == pointId);
                        if (find.pid > 0)
                            dataPointId = (int)find.dp_fk;
                    }
                    break;
                case (int)PointTypeCode.BO:
                    {
                        var find = pdbCommands.FirstOrDefault(p => p.pid == pointId);
                        if (find.pid > 0)
                            dataPointId = (int)find.dp_fk;
                    }
                    break;
                case (int)PointTypeCode.AI:
                    {
                        var find = pdbAnalogs.FirstOrDefault(p => p.pid == pointId);
                        if (find.pid > 0)
                            dataPointId = (int)find.dp_fk;
                    }
                    break;
                case (int)PointTypeCode.AO:
                    {
                        var find = pdbSetPoints.FirstOrDefault(p => p.pid == pointId);
                        if (find.pid > 0)
                            dataPointId = (int)find.dp_fk;
                    }
                    break;
                case (int)PointTypeCode.COUNTER:
                    {
                        var find = pdbAccumulators.FirstOrDefault(p => p.pid == pointId);
                        if (find.pid > 0)
                            dataPointId = (int)find.dp_fk;
                    }
                    break;
            }

            if (dataPointId > 0)
            {
                var alarmFilter = AlarmInfos.FirstOrDefault(p => p.PointId == dataPointId);
                if (alarmFilter != null)
                {
                    if (!alarmFilter.UseYn)
                        _logger.ServerLog($"[알람 실시간] ALARM PID:{pointId} DATAPOINT ID:{alarmFilter.PointId} PN:{alarmFilter.PointName} 항목은 알람 필터링 대상.");
                    else
                        retValue = true;
                }
                else
                    _logger.Debug($"[알람 실시간] ALARM PID:{pointId} DATAPOINT ID:{dataPointId} 알람 필터링 데이터가 없음.");
            }
            else
                _logger.Debug($"[알람 실시간] ALARM PID:{pointId} DATAPOINT ID:0 알람 필터링 데이터가 없음.");

            return retValue;
        }

        public void FiAlarmDataSave(List<rtdb_Alarm> alarmList)
        {
            var mappingInfos = _configuration.GetSection("MappingInfo");
            var currentAId = Convert.ToInt32(mappingInfos.GetSection("FaultCurrentA").Value ?? "51");
            var currentBId = Convert.ToInt32(mappingInfos.GetSection("FaultCurrentB").Value ?? "52");
            var currentCId = Convert.ToInt32(mappingInfos.GetSection("FaultCurrentC").Value ?? "53");
            var currentNId = Convert.ToInt32(mappingInfos.GetSection("FaultCurrentN").Value ?? "54");
            try
            {
                var date = DateTime.Now;
                foreach (var alarm in alarmList)
                {
                    bool retValue = AlarmFilterCheck((int)alarm.uiPtType, (int)alarm.uiPid);
                    if (!retValue)
                        continue;

                    var findRemoteunit = pdbRemoteUnits.FirstOrDefault(p => p.pid == alarm.uiRtuid);
                    if (findRemoteunit.pid <= 0)
                    {
                        _logger.Debug($"[알람 실시간] Remoteunit RTUID:{alarm.uiRtuid} 없음 ");
                        continue;
                    }

                    if (findRemoteunit.protocol_fk == 0 && findRemoteunit.comm_type == 0)
                        continue;

                    var findDl = pdbDistributionLines.FirstOrDefault(p => p.dlid == alarm.uiDL);
                    if (findDl.dlid <= 0)
                    {
                        _logger.Debug($"[알람 실시간] DistributionLine DlId:0 / Alarm DlId:{alarm.uiDL}");
                        continue;
                    }

                    var equipment = pdbConductingequipments.FirstOrDefault(p => p.ceqid == (int)alarm.uiEqid);
                    if (equipment.ceqid <= 0)
                    {
                        _logger.Debug($"[알람 실시간] Conductingequipment CEQID:0 / Alarm EqId:{alarm.uiEqid}");
                        continue;
                    }

                    if (equipment.rtu_type != 2)    // RTU 타입이2 자동
                        continue;

                    HistoryFiAlarm data = new HistoryFiAlarm();
                    data.SaveTime = date;
                    data.Ceqid = (int)alarm.uiEqid;
                    data.LogTime = KdmsValueConverter.TimeTToDateTime(alarm.uiSVRTime).AddMilliseconds(alarm.uiSVRMics);
                    data.FrtuTime = KdmsValueConverter.TimeTToDateTime(alarm.uiRTUTime).AddMilliseconds(alarm.uiRTUMics);
                    data.Cpsid = (int)equipment.ec_fk;
                    data.Name = GetStringData(equipment.name);
                    data.Dl = GetStringData(findDl.name);
                    data.Value = (float)alarm.fVal;
                    data.LogDesc = GetStringData(alarm.szDesc);




                    switch (alarm.uiPtType)
                    {
                        case (int)PointTypeCode.BI:
                            {
                                var findDiscrete = pdbDiscretes.FirstOrDefault(p => p.pid == alarm.uiPid);
                                if (findDiscrete.pid > 0)
                                {
                                    data.AlarmName = GetStringData(findDiscrete.name);
                                    data.Circuitno = (int)findDiscrete.circuit_no;
                                }
                                else
                                {
                                    data.AlarmName = "None";
                                    data.Circuitno = 0;
                                }

                                //data.AlarmName = GetStringData(pdbDiscretes.FirstOrDefault(p => p.pid == alarm.uiPid).name);
                                //data.Circuitno = (int)pdbDiscretes.FirstOrDefault(p => p.pid == alarm.uiPid).circuit_no;
                            }
                            break;
                        case (int)PointTypeCode.BO:
                            {
                                var findDiscrete = pdbCommands.FirstOrDefault(p => p.pid == alarm.uiPid);
                                if (findDiscrete.pid > 0)
                                {
                                    data.AlarmName = GetStringData(findDiscrete.name);
                                    data.Circuitno = (int)findDiscrete.circuit_no;
                                }
                                else
                                {
                                    data.AlarmName = "None";
                                    data.Circuitno = 0;
                                }

                                //data.AlarmName = GetStringData(pdbCommands.FirstOrDefault(p => p.pid == alarm.uiPid).name);
                                //data.Circuitno = (int)pdbCommands.FirstOrDefault(p => p.pid == alarm.uiPid).circuit_no;
                            }
                            break;
                        case (int)PointTypeCode.AI:
                            {
                                var findDiscrete = pdbAnalogs.FirstOrDefault(p => p.pid == alarm.uiPid);
                                if (findDiscrete.pid > 0)
                                {
                                    data.AlarmName = GetStringData(findDiscrete.name);
                                    data.Circuitno = (int)findDiscrete.circuit_no;
                                }
                                else
                                {
                                    data.AlarmName = "None";
                                    data.Circuitno = 0;
                                }

                                //data.AlarmName = GetStringData(pdbAnalogs.FirstOrDefault(p => p.pid == alarm.uiPid).name);
                                //data.Circuitno = (int)pdbAnalogs.FirstOrDefault(p => p.pid == alarm.uiPid).circuit_no;
                            }
                            break;
                        case (int)PointTypeCode.AO:
                            {
                                var findDiscrete = pdbSetPoints.FirstOrDefault(p => p.pid == alarm.uiPid);
                                if (findDiscrete.pid > 0)
                                {
                                    data.AlarmName = GetStringData(findDiscrete.name);
                                    data.Circuitno = (int)findDiscrete.circuit_no;
                                }
                                else
                                {
                                    data.AlarmName = "None";
                                    data.Circuitno = 0;
                                }

                                //data.AlarmName = GetStringData(pdbSetPoints.FirstOrDefault(p => p.pid == alarm.uiPid).name);
                                //data.Circuitno = (int)pdbSetPoints.FirstOrDefault(p => p.pid == alarm.uiPid).circuit_no;
                            }
                            break;
                        case (int)PointTypeCode.COUNTER:
                            {
                                var findDiscrete = pdbAccumulators.FirstOrDefault(p => p.pid == alarm.uiPid);
                                if (findDiscrete.pid > 0)
                                {
                                    data.AlarmName = GetStringData(findDiscrete.name);
                                    data.Circuitno = (int)findDiscrete.circuit_no;
                                }
                                else
                                {
                                    data.AlarmName = "None";
                                    data.Circuitno = 0;
                                }

                                //data.AlarmName = GetStringData(pdbAccumulators.FirstOrDefault(p => p.pid == alarm.uiPid).name);
                                //data.Circuitno = (int)pdbAccumulators.FirstOrDefault(p => p.pid == alarm.uiPid).circuit_no;
                            }
                            break;
                    }

                    var ceqAnalogList = pdbAnalogs.Where(p => p.ceq_fk == (int)alarm.uiEqid).ToList(); // && p.pid == alarm.uiPid).ToList();
                    var find = ceqAnalogList.FirstOrDefault(p => p.pid == alarm.uiPid);
                    if (find.pid > 0)
                    {
                        if (find.dp_fk == currentAId)
                        {
                            var findrtdb = rtdbAnalogs.FirstOrDefault(p => p.pid == find.pid);
                            if (findrtdb.pid > 0)
                            {
                                _logger.Debug($"[알람 실시간] PID;{find.pid} DPID:{currentAId} Alarm Value:{alarm.fVal} → Analogs Value:{findrtdb.value} 변경 전 확인");
                                var index = rtdbAnalogs.IndexOf(findrtdb);
                                if (index >= 0)
                                {
                                    findrtdb.value = alarm.fVal;
                                    rtdbAnalogs[index] = findrtdb;
                                    _logger.Debug($"[알람 실시간] PID;{find.pid} DPID:{currentAId} Alarm Value:{alarm.fVal} → Analogs Value:{findrtdb.value} 변경 후 확인");
                                }
                                else
                                    _logger.Debug($"[알람 실시간] PID;{find.pid} DPID:{currentAId} Alarm Value:{alarm.fVal} → Analogs Value:{findrtdb.value} 변경 실패 (찾기 실패)");
                            }
                        }
                        else if (find.dp_fk == currentBId)
                        {
                            var findrtdb = rtdbAnalogs.FirstOrDefault(p => p.pid == find.pid);
                            if (findrtdb.pid > 0)
                            {
                                _logger.Debug($"[알람 실시간] PID;{find.pid} DPID:{currentBId} Alarm Value:{alarm.fVal} → Analogs Value:{findrtdb.value} 변경 전 확인");
                                var index = rtdbAnalogs.IndexOf(findrtdb);
                                if (index >= 0)
                                {
                                    findrtdb.value = alarm.fVal;
                                    rtdbAnalogs[index] = findrtdb;
                                    _logger.Debug($"[알람 실시간] PID;{find.pid} DPID:{currentBId} Alarm Value:{alarm.fVal} → Analogs Value:{findrtdb.value} 변경 후 확인");
                                }
                                else
                                    _logger.Debug($"[알람 실시간] PID;{find.pid} DPID:{currentBId} Alarm Value:{alarm.fVal} → Analogs Value:{findrtdb.value} 변경 실패 (찾기 실패)");
                            }
                        }
                        else if (find.dp_fk == currentCId)
                        {
                            var findrtdb = rtdbAnalogs.FirstOrDefault(p => p.pid == find.pid);
                            if (findrtdb.pid > 0)
                            {
                                _logger.Debug($"[알람 실시간] PID;{find.pid} DPID:{currentCId} Alarm Value:{alarm.fVal} → Analogs Value:{findrtdb.value} 변경 전 확인");
                                var index = rtdbAnalogs.IndexOf(findrtdb);
                                if (index >= 0)
                                {
                                    findrtdb.value = alarm.fVal;
                                    rtdbAnalogs[index] = findrtdb;
                                    _logger.Debug($"[알람 실시간] PID;{find.pid} DPID:{currentCId} Alarm Value:{alarm.fVal} → Analogs Value:{findrtdb.value} 변경 후 확인");
                                }
                                else
                                    _logger.Debug($"[알람 실시간] PID;{find.pid} DPID:{currentCId} Alarm Value:{alarm.fVal} → Analogs Value:{findrtdb.value} 변경 실패 (찾기 실패)");
                            }
                        }
                        else if (find.dp_fk == currentNId)
                        {
                            var findrtdb = rtdbAnalogs.FirstOrDefault(p => p.pid == find.pid);
                            if (findrtdb.pid > 0)
                            {
                                _logger.Debug($"[알람 실시간] PID;{find.pid} DPID:{currentNId} Alarm Value:{alarm.fVal} → Analogs Value:{findrtdb.value} 변경 전 확인");
                                var index = rtdbAnalogs.IndexOf(findrtdb);
                                if (index >= 0)
                                {
                                    findrtdb.value = alarm.fVal;
                                    rtdbAnalogs[index] = findrtdb;
                                    _logger.Debug($"[알람 실시간] PID;{find.pid} DPID:{currentNId} Alarm Value:{alarm.fVal} → Analogs Value:{findrtdb.value} 변경 후 확인");
                                }
                                else
                                    _logger.Debug($"[알람 실시간] PID;{find.pid} DPID:{currentNId} Alarm Value:{alarm.fVal} → Analogs Value:{findrtdb.value} 변경 실패 (찾기 실패)");
                            }
                        }
                    }

                    data.FaultCurrentA = GetAlarmDataValue(ceqAnalogList, currentAId);
                    data.FaultCurrentB = GetAlarmDataValue(ceqAnalogList, currentBId);
                    data.FaultCurrentC = GetAlarmDataValue(ceqAnalogList, currentCId);
                    data.FaultCurrentN = GetAlarmDataValue(ceqAnalogList, currentNId);

                    // 고장검출 알람이고 Value 값이 1이면 Fault Current A,B,C,N 검사
                    if (data.AlarmName.Contains("고장") && data.Value == 1)
                    {
                        // 고장전류A 상 값이 0이면 다시 한번 RTDB PDB 다운로드
                        if (data.FaultCurrentA == 0)
                        {
                            _logger.Debug($"[알람 실시간] PID;{find.pid} DPID:{find.dp_fk} FaultCurrentA:0 RTDB PDB 재다운로드 진행 시작");
                            worker.KdmsRealTimePdbFile();
                            _logger.Debug($"[알람 실시간] PID;{find.pid} DPID:{find.dp_fk} FaultCurrentA:0 RTDB PDB 재다운로드 진행 완료");
                            data.FaultCurrentA = GetAlarmDataValue(ceqAnalogList, currentAId);
                            data.FaultCurrentB = GetAlarmDataValue(ceqAnalogList, currentBId);
                            data.FaultCurrentC = GetAlarmDataValue(ceqAnalogList, currentCId);
                            data.FaultCurrentN = GetAlarmDataValue(ceqAnalogList, currentNId);
                        }
                    }
                    else if (data.AlarmName.Contains("고장") && data.Value == 0)
                    {
                        data.FaultCurrentA = 0;
                        data.FaultCurrentB = 0;
                        data.FaultCurrentC = 0;
                        data.FaultCurrentN = 0;
                    }

                    string query = $"insert into history_fi_alarm values ('{data.SaveTime.ToString("yyyy-MM-dd HH:mm:ss")}', {data.Ceqid}, '{data.LogTime?.ToString("yyyy-MM-dd HH:mm:ss.fff")}', '{data.FrtuTime?.ToString("yyyy-MM-dd HH:mm:ss.fff")}', " +
                        $" {data.Cpsid}, {data.Circuitno}, '{data.Name?.Trim()}', '{data.Dl?.Trim()}', '{data.AlarmName}', {data.Value}, '{data.LogDesc}', {data.FaultCurrentA}, {data.FaultCurrentB}, {data.FaultCurrentC}, {data.FaultCurrentN})";

                    //_logger.Debug("확인 로그 시작=====================================================================================");
                    //_logger.Debug("");
                    //_logger.Debug($"[알람 실시간] SAVETIME, CEQID, LOGTIME, FRTUTIME, CPSID, CIRCUITNO, NAME, DL, ALARMNAME, VALUE, LOGDESC, FaultCurrentA, FaultCurrentB, FaultCurrentC, FaultCurrentN");
                    //_logger.Debug($"[알람 실시간] '{data.SaveTime.ToString("yyyy-MM-dd HH:mm:ss")}', {data.Ceqid}, '{data.LogTime?.ToString("yyyy-MM-dd HH:mm:ss.fff")}', '{data.FrtuTime?.ToString("yyyy-MM-dd HH:mm:ss.fff")}', " +
                    //    $" {data.Cpsid}, {data.Circuitno}, '{data.Name?.Trim()}', '{data.Dl?.Trim()}', '{data.AlarmName}', {data.Value}, '{data.LogDesc}', {data.FaultCurrentA}, {data.FaultCurrentB}, {data.FaultCurrentC}, {data.FaultCurrentN}");
                    //_logger.Debug("");
                    //_logger.Debug("확인 로그 종료=====================================================================================");

                    using (MySqlMapper mapper = new MySqlMapper(_configuration))
                    {
                        mapper.RunQuery(query);
                        _logger.ServerLog($"[알람 실시간] CEQ:{data.Ceqid} NAME:{data.AlarmName} VALUE:{data.Value} 데이터 입력 완료 ");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.ServerLog($"[알람 실시간] 데이터 입력중 예외 발생 ex:{ex.Message}");
            }
        }

        public string GetStringData(byte[] bytes)
        {
            var retValue = Encoding.UTF8.GetString(bytes).Trim('\0');
            retValue = retValue.TrimStart();
            retValue = retValue.TrimEnd();

            return retValue;
        }

        public void RemoteUnitSave()
        {
            if (pdbRemoteUnits.Where(p => p.pid != 0).Count() <= 0)
            {
                _logger.ServerLog($"[PDB 파일] pdb_Remoteunit 데이터가 없습니다.");
                return;
            }

            var saveDatas = pdbRemoteUnits.Where(p => p.pid != 0).Select(p => new PdbRemoteunit
            {
                Pid = p.pid,
                DmcFk = p.dmc_fk,
                CommDmcFk = p.comm_dmc_fk,
                CidFk = p.cid_fk,
                ChannelPrimary = p.channel_primary,
                ChannelAlternate = p.channel_alternate,
                DcpPrimeFk = p.dcp_prime_fk,
                DcpBackupFk = p.dcp_backup_fk,
                SbFk = p.sb_fk,
                ProtocolFk = p.protocol_fk,
                ProtocolName = GetStringData(p.protocol_name),
                CommType = p.comm_type,
                CommInfo = GetStringData(p.comm_info),
                RtuMaker = p.rtu_maker,
                RtuCompany = GetStringData(p.rtu_company),
                EqMaker = p.eq_maker,
                EqCompany = GetStringData(p.eq_company),
                EqFk = (int)p.eq_fk,
                EqType = (int)p.eq_type,
                RtuMapFk = (int)p.rtu_map_fk,
                RtuType = (int)p.rtu_type,
                Name = GetStringData(p.name),
                MasterAddr = (int)p.master_addr,
                SlaveAddr = (int)p.slave_addr,
                Confirm = (int)p.confirm,
                DlTimeout = (int)p.dl_timeout,
                AppTimeout = (int)p.app_timeout,
                Retry = (int)p.retry,
                RtuSeralno = GetStringData(p.rtu_seralno),
                RtuMakeDate = GetStringData(p.rtu_make_date),
                RtuInstallDate = GetStringData(p.rtu_install_date),
                RtuVersion = GetStringData(p.rtu_version),
                EqSerialno = GetStringData(p.eq_serialno),
                EqMakeDate = GetStringData(p.eq_make_date),
                EqInstallDate = GetStringData(p.eq_install_date),
                EqInstallManager = GetStringData(p.eq_install_manager),
                FiedName = GetStringData(p.fied_name),
                UseAoper = (int)p.use_aoper,
                LinkAddrSize = (int)p.link_addr_size,
                CotSize = (int)p.cot_size,
                AsduAddrSize = (int)p.asdu_addr_size,
                ObjectAddrSize = (int)p.object_addr_size,
                WaveCommTypeFk = (int)p.wave_comm_type_fk
            }).ToList();

            using (KdmsContext context = DbContextConfigurationExtensions.CreateServerContext(_configuration))
            {
                context.Database.ExecuteSqlRaw("TRUNCATE TABLE pdb_remoteunit");
                context.PdbRemoteunits.AddRange(saveDatas);
                context.SaveChanges();

                _logger.ServerLog($"[PDB 파일] pdb_Remoteunit 데이터 데이터베이스 입력 성공 CNT: {saveDatas.Count}");
            }
        }

        public void CompositSwitchSave()
        {
            if (pdbCompositeSwitchs.Where(p => p.pid != 0).Count() <= 0)
            {
                _logger.ServerLog($"[PDB 파일] CompositSwitch 데이터가 없습니다.");
                return;
            }

            var saveDatas = pdbCompositeSwitchs.Where(p => p.pid != 0).Select(p => new Compositeswitch
            {
                Pid = p.pid,
                DlFk = p.dl_fk,
                Psrtype = p.psrtype,
                Name = GetStringData(p.name),
                MeshNo = GetStringData(p.mesh_no),
                Aliasname = GetStringData(p.aliasname)
            }).ToList();

            using (KdmsContext context = DbContextConfigurationExtensions.CreateServerContext(_configuration))
            {
                context.Database.ExecuteSqlRaw("TRUNCATE TABLE compositeswitch");
                context.Compositeswitches.AddRange(saveDatas);
                context.SaveChanges();

                _logger.ServerLog($"[PDB 파일] CompositSwitch 데이터 데이터베이스 입력 성공 CNT: {saveDatas.Count}");
            }
        }

        public void ConductingequipmentSave()
        {
            if (pdbConductingequipments.Where(p => p.ceqid != 0).Count() <= 0)
            {
                _logger.ServerLog($"[PDB 파일] pdb_Conductingequipment 데이터가 없습니다.");
                return;
            }

            var saveDatas = pdbConductingequipments.Where(p => p.ceqid != 0).Select(p => new PdbConductingequipment
            {
                Ceqid = p.ceqid,
                EqFk = p.eq_fk,
                EcFk = p.ec_fk,
                StFk = p.st_fk,
                DlFk = p.dl_fk,
                LinkStFk = p.link_st_fk,
                LinkDlFk = p.link_dl_fk,
                Psrtype = (int)p.psrtype,
                Name = GetStringData(p.name),
                Desc = GetStringData(p.desc),
                EcName = GetStringData(p.ec_name),
                MeshNo = GetStringData(p.mesh_no),
                SwType = p.sw_type,
                RtuType = (int)p.rtu_type,
                DevNo = GetStringData(p.dev_no),
                DtrFk = p.dtr_fk,
                PtrFk = p.ptr_fk,
                LinkCbFk = p.link_cb_fk,
                LinkBbsFk = p.link_bbs_fk,
                BaseVoltage = (int)p.base_voltage,
                Phases = (int)p.phases,
                BusbarOrder = (int)p.busbar_order,
                AiCnt = (int)p.ai_cnt,
                AiPid = p.ai_pid,
                AoCnt = (int)p.ao_cnt,
                AoPid = p.ao_pid,
                BiCnt = (int)p.bi_cnt,
                BiPid = p.bi_pid,
                PiCnt = (int)p.pi_cnt,
                PiPid = p.pi_pid
            }).ToList();

            using (KdmsContext context = DbContextConfigurationExtensions.CreateServerContext(_configuration))
            {
                context.Database.ExecuteSqlRaw("TRUNCATE TABLE pdb_conductingequipment");
                context.PdbConductingequipments.AddRange(saveDatas);
                context.SaveChanges();

                _logger.ServerLog($"[PDB 파일] pdb_Conductingequipment 데이터 데이터베이스 입력 성공 CNT: {saveDatas.Count}");
            }
        }

        public void DistributionLineSegmentSave()
        {
            if (pdbDistributionLineSegments.Where(p => p.pid != 0).Count() <= 0)
            {
                _logger.ServerLog($"[PDB 파일] pdb_DistributionLineSegment 데이터가 없습니다.");
                return;
            }

            var saveDatas = pdbDistributionLineSegments.Where(p => p.pid != 0).Select(p => new PdbDistributionlinesegment
            {
                Dlsid = p.pid,
                CeqFk = p.ceq_fk,
                DlFk = p.dl_fk,
                PlsiAFk = p.plsiid_a_fk,
                PlsiBFk = p.plsiid_b_fk,
                PlsiCFk = p.plsiid_c_fk,
                PlsiNFk = p.plsiid_n_fk,
                Name = GetStringData(p.name),
                Aliasname = GetStringData(p.aliasname),
                Length = (float)p.length,
                LengthUsFk = p.length_us_fk,
                CeqFFk = p.ceq_f_fk,
                SgrFFk = p.sgr_f_fk,
                CeqBFk = p.ceq_b_fk,
                SgrBFk = p.sgr_b_fk,
                SecLoad = (float)p.secload,
                SecLoadUsFk = p.secload_us_fk
            }).ToList();

            using (KdmsContext context = DbContextConfigurationExtensions.CreateServerContext(_configuration))
            {
                context.Database.ExecuteSqlRaw("TRUNCATE TABLE pdb_distributionlinesegment");
                context.PdbDistributionlinesegments.AddRange(saveDatas);
                context.SaveChanges();

                _logger.ServerLog($"[PDB 파일] pdb_DistributionLineSegment 데이터 데이터베이스 입력 성공 CNT: {saveDatas.Count}");
            }
        }

        public void GeographicalRegionSave()
        {
            if (pdbGeographicalRegions.Where(p => p.ggrid != 0).Count() <= 0)
            {
                _logger.ServerLog($"[PDB 파일] GeographicalRegion 데이터가 없습니다.");
                return;
            }

            var saveDatas = pdbGeographicalRegions.Select(p => new Geographicalregion
            {
                Ggrid = (int)p.ggrid,
                Name = GetStringData(p.name)
            }).ToList();

            using (KdmsContext context = DbContextConfigurationExtensions.CreateServerContext(_configuration))
            {
                context.Database.ExecuteSqlRaw("TRUNCATE TABLE geographicalregion");
                context.Geographicalregions.AddRange(saveDatas);
                context.SaveChanges();

                _logger.ServerLog($"[PDB 파일] GeographicalRegion 데이터 데이터베이스 입력 성공 CNT: {saveDatas.Count}");
            }
        }

        public void SubGeographicalRegionSave()
        {
            if (pdbSubGeographicalRegions.Where(p => p.sgrid != 0).Count() <= 0)
            {
                _logger.ServerLog($"[PDB 파일] SubGeographicalRegion 데이터가 없습니다.");
                return;
            }

            var saveDatas = pdbSubGeographicalRegions.Select(p => new Subgeographicalregion
            {
                Sgrid = (int)p.sgrid,
                GgrFk = (int)p.ggr_fk,
                Name = GetStringData(p.name)
            }).ToList();

            using (KdmsContext context = DbContextConfigurationExtensions.CreateServerContext(_configuration))
            {
                context.Database.ExecuteSqlRaw("TRUNCATE TABLE subgeographicalregion");
                context.Subgeographicalregions.AddRange(saveDatas);
                context.SaveChanges();

                _logger.ServerLog($"[PDB 파일] SubGeographicalRegion 데이터 데이터베이스 입력 성공 CNT: {saveDatas.Count}");
            }
        }

        public void SubStationSave()
        {
            if (pdbSubStations.Where(p => p.stid != 0).Count() <= 0)
            {
                _logger.ServerLog($"[PDB 파일] SubStation 데이터가 없습니다.");
                return;
            }

            var saveDatas = pdbSubStations.Select(p => new Substation
            {
                Stid = (int)p.stid,
                SgrFk = p.sgr_fk,
                Name = GetStringData(p.name)
            }).ToList();

            using (KdmsContext context = DbContextConfigurationExtensions.CreateServerContext(_configuration))
            {
                context.Database.ExecuteSqlRaw("TRUNCATE TABLE substation");
                context.Substations.AddRange(saveDatas);
                context.SaveChanges();

                _logger.ServerLog($"[PDB 파일] SubStation 데이터 데이터베이스 입력 성공 CNT: {saveDatas.Count}");
            }
        }

        public void DistributionLineSave()
        {
            if (pdbDistributionLines.Where(p => p.dlid != 0).Count() <= 0)
            {
                _logger.ServerLog($"[PDB 파일] DistributionLine 데이터가 없습니다.");
                return;
            }

            var saveDatas = pdbDistributionLines.Where(p => p.dlid != 0).Select(p => new Distributionline
            {
                Dlid = p.dlid,
                StFk = p.st_fk,
                PtrFk = p.ptr_fk,
                SwFk = p.sw_fk,
                Name = GetStringData(p.name),
                DlNo = (int)p.dlno,
                Reliability = (int)p.reliability,
                Priority = (int)p.priority,
                RatedS = (int)p.rated_s,
                RatedSUsFk = p.rated_s_usfk
            }).ToList();

            using (KdmsContext context = DbContextConfigurationExtensions.CreateServerContext(_configuration))
            {
                context.Database.ExecuteSqlRaw("TRUNCATE TABLE distributionline");
                context.Distributionlines.AddRange(saveDatas);
                context.SaveChanges();

                _logger.ServerLog($"[PDB 파일] DistributionLine 데이터 데이터베이스 입력 성공 CNT: {saveDatas.Count}");
            }
        }

        public void PowerTransformerSave()
        {
            if (pdbPowerTransformers.Where(p => p.ptrid != 0).Count() <= 0)
            {
                _logger.ServerLog($"[PDB 파일] PowerTransformer 데이터가 없습니다.");
                return;
            }

            var saveDatas = pdbPowerTransformers.Where(p => p.ptrid != 0).Select(p => new Powertransformer
            {
                Pid = p.ptrid,
                StFk = p.st_fk,
                TapFk = p.tap_fk,
                Trw1stFk = p.trw_1st_fk,
                Trw2stFk = p.trw_2nd_fk,
                Bbs1stFk = p.bbs_1st_fk,
                Bbs2stFk = p.bbs_2nd_fk,
                Name = GetStringData(p.name),
                BankNo = (int)p.bank_no,
                MtrImp = (int)p.mtr_imp,
                MtrImpUsfk = p.mtr_imp_us_fk,
                DispPos = (int)p.disp_pos,
                StType = (int)p.st_type
            }).ToList();

            using (KdmsContext context = DbContextConfigurationExtensions.CreateServerContext(_configuration))
            {
                context.Database.ExecuteSqlRaw("TRUNCATE TABLE powertransformer");
                context.Powertransformers.AddRange(saveDatas);
                context.SaveChanges();

                _logger.ServerLog($"[PDB 파일] PowerTransformer 데이터 데이터베이스 입력 성공 CNT: {saveDatas.Count}");
            }
        }

        public List<HistoryMinDatum> MinDataLoad(DateTime date)
        {
            List<HistoryMinDatum> retList = new List<HistoryMinDatum>();
            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"select * from history_min_data_{date.ToString("yyyyMMdd")} where save_time >= '{date.ToString("yyyy-MM-dd 00:00:00")}' and save_time <= '{date.ToString("yyyy-MM-dd 23:59:59")}' order by ceqid, save_time");
            using (MySqlMapper mapper = new MySqlMapper(_configuration))
            {
                retList = mapper.ExecuteQuery<HistoryMinDatum>(sb.ToString());
            }
            return retList;
        }

        public List<Statistics15min> StatisticsMinDataLoad(DateTime date)
        {
            List<Statistics15min> retList = new List<Statistics15min>();
            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"select * from statistics_15min where save_time >= '{date.ToString("yyyy-MM-dd HH:00:00")}' and save_time <= '{date.ToString("yyyy-MM-dd HH:59:59")}' order by ceqid, save_time");
            using (MySqlMapper mapper = new MySqlMapper(_configuration))
            {
                retList = mapper.ExecuteQuery<Statistics15min>(sb.ToString());
            }
            return retList;
        }

        public List<StatisticsHour> StatisticsHourDataLoad(DateTime date)
        {
            List<StatisticsHour> retList = new List<StatisticsHour>();
            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"select * from statistics_hour where save_time >= '{date.ToString("yyyy-MM-dd 00:00:00")}' and save_time <= '{date.ToString("yyyy-MM-dd 23:59:59")}' order by ceqid, save_time");
            using (MySqlMapper mapper = new MySqlMapper(_configuration))
            {
                retList = mapper.ExecuteQuery<StatisticsHour>(sb.ToString());
            }
            return retList;
        }

        public List<StatisticsDay> StatisticsDayDataLoad(DateTime date)
        {
            List<StatisticsDay> retList = new List<StatisticsDay>();
            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"select * from statistics_day where save_time >= '{date.ToString("yyyy-MM-01 00:00:00")}' and save_time < '{date.AddMonths(1).ToString("yyyy-MM-01 00:00:00")}' order by ceqid, save_time");
            using (MySqlMapper mapper = new MySqlMapper(_configuration))
            {
                retList = mapper.ExecuteQuery<StatisticsDay>(sb.ToString());
            }
            return retList;
        }

        public List<StatisticsMonth> StatisticsMonthDataLoad(DateTime date)
        {
            List<StatisticsMonth> retList = new List<StatisticsMonth>();
            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"select * from statistics_month where save_time >= '{date.ToString("yyyy-01-01 00:00:00")}' and save_time < '{date.AddYears(1).ToString("yyyy-01-01 00:00:00")}' order by ceqid, save_time");
            using (MySqlMapper mapper = new MySqlMapper(_configuration))
            {
                retList = mapper.ExecuteQuery<StatisticsMonth>(sb.ToString());
            }
            return retList;
        }


        public bool DaystatDataInput(List<HistoryDaystatDatum> dataList, DateTime date)
        {
            bool retValue = false;

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"insert into history_daystat_data_{date.ToString("yyyy")} values ");
            int cnt = 0;
            foreach (var data in dataList)
            {
                cnt++;
                sb.Append($"('{data.SaveTime.ToString("yyyy-MM-dd HH:mm:ss")}', {data.Ceqid}, '{data.CommTime?.ToString("yyyy-MM-dd HH:mm:ss")}', {data.Cpsid}, {data.Circuitno}, '{data.Name}', '{data.Dl}'," +
                    $" {data.Diagnostics}, {data.VoltageUnbalance}, {data.CurrentUnbalance}, {data.Frequency}," +
                    $" {data.AverageCurrentA}, {data.AverageCurrentB}, {data.AverageCurrentC}, {data.AverageCurrentN}," +
                    $" {data.MaxCurrentA}, {data.MaxCurrentB}, {data.MaxCurrentC}, {data.MaxCurrentN}, '{data.MaxCommTime?.ToString("yyyy-MM-dd HH:mm:ss")}'," +
                    $" {data.MinCurrentA}, {data.MinCurrentB}, {data.MinCurrentC}, {data.MinCurrentN}, '{data.MinCommTime?.ToString("yyyy-MM-dd HH:mm:ss")}')");

                if (cnt != dataList.Count)
                    sb.AppendLine(",");
                else
                    sb.AppendLine(";");
            }
            using (MySqlMapper mapper = new MySqlMapper(_configuration))
            {
                mapper.RunQuery(sb.ToString());
                retValue = true;
            }

            return retValue;
        }

        public void StatisticsHourDataInput(List<StatisticsHour> dataList)
        {
            using (KdmsContext context = DbContextConfigurationExtensions.CreateServerContext(_configuration))
            {
                context.StatisticsHours.AddRange(dataList);
                context.SaveChanges();
            }
        }

        public void StatisticsDayDataInput(List<StatisticsDay> dataList)
        {
            using (KdmsContext context = DbContextConfigurationExtensions.CreateServerContext(_configuration))
            {
                context.StatisticsDays.AddRange(dataList);
                context.SaveChanges();
            }
        }

        public void StatisticsMonthDataInput(List<StatisticsMonth> dataList)
        {
            using (KdmsContext context = DbContextConfigurationExtensions.CreateServerContext(_configuration))
            {
                context.StatisticsMonths.AddRange(dataList);
                context.SaveChanges();
            }
        }

        public void StatisticsYearDataInput(List<StatisticsYear> dataList)
        {
            using (KdmsContext context = DbContextConfigurationExtensions.CreateServerContext(_configuration))
            {
                context.StatisticsYears.AddRange(dataList);
                context.SaveChanges();
            }
        }

        public bool SingleMinDataTableCreate(DateTime date)
        {
            bool retval = false;
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"call min_data_create('history_min_data_{date.ToString("yyyyMMdd")}');");

                using (MySqlMapper mapper = new MySqlMapper(_configuration))
                {
                    retval = mapper.RunQuery(sb.ToString());
                }
            }
            catch (Exception ex)
            {
                _logger.DbLog($"[1분 실시간] history_min_data_{date.ToString("yyyyMMdd")} 테이블 생성 실패 ex:{ex.Message}");
            }

            return retval;
        }

        public bool MinDataTableCreate(DateTime date)
        {
            bool retval = false;
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"call min_data_create('history_min_data_{date.AddDays(1).ToString("yyyyMMdd")}');");
                sb.AppendLine($"call min_data_create('history_min_data_{date.AddDays(2).ToString("yyyyMMdd")}');");

                using (MySqlMapper mapper = new MySqlMapper(_configuration))
                {
                    retval = mapper.RunQuery(sb.ToString());
                }
            }
            catch (Exception ex)
            {
                _logger.DbLog($"[1분 실시간] history_min_data_{date.AddDays(1).ToString("yyyyMMdd")} 테이블 생성 실패 ex:{ex.Message}");
                _logger.DbLog($"[1분 실시간] history_min_data_{date.AddDays(2).ToString("yyyyMMdd")} 테이블 생성 실패 ex:{ex.Message}");
            }

            return retval;
        }

        public bool SingleDayStatTableCreate(DateTime date)
        {
            bool retval = false;
            try
            {
                string query = $"call daystat_data_create('history_daystat_data_{date.ToString("yyyy")}');";
                using (MySqlMapper mapper = new MySqlMapper(_configuration))
                {
                    retval = mapper.RunQuery(query);
                }
            }
            catch (Exception ex)
            {
                _logger.DbLog($"[1일 통계(1분실시간전류)] history_daystat_data_{date.ToString("yyyy")} 테이블 생성 실패 ex:{ex.Message}");
            }
            return retval;
        }

        public bool DayStatTableCreate(DateTime date)
        {
            bool retval = false;
            try
            {
                string query = $"call daystat_data_create('history_daystat_data_{date.AddYears(1).ToString("yyyy")}');";
                using (MySqlMapper mapper = new MySqlMapper(_configuration))
                {
                    retval = mapper.RunQuery(query);
                }
            }
            catch (Exception ex)
            {
                _logger.DbLog($"[1일 통계(1분실시간전류)] history_daystat_data_{date.AddYears(1).ToString("yyyy")} 테이블 생성 실패 ex:{ex.Message}");
            }
            return retval;
        }

        public bool MinDataTableDrop(DateTime date, int day)
        {
            bool retval = false;

            retval = MinDataTableBackup(date, day);
            if (!retval)
                return false;

            try
            {
                string query = $"drop Table IF exists history_min_data_{date.AddDays(-day).ToString("yyyyMMdd")}";
                using (MySqlMapper mapper = new MySqlMapper(_configuration))
                {
                    retval = mapper.RunQuery(query);
                    if (retval)
                        _logger.DbLog($"[1분 실시간] history_min_data_{date.AddDays(-day).ToString("yyyyMMdd")} 테이블 삭제");
                }
            }
            catch (Exception ex)
            {
                _logger.DbLog($"[1분 실시간] history_min_data_{date.AddDays(-day).ToString("yyyyMMdd")} 테이블 삭제 실패 ex:{ex.Message}");
            }
            return retval;
        }

        public bool MinDataTableBackup(DateTime date, int day)
        {
            bool retval = false;

            var dayStr = date.AddDays(-day).ToString("yyyyMMdd");
            var dateStr = date.ToString("yyyyMMddHHmm");
            var tableName = $"history_min_data_{dayStr}";

            var forderPath = $"{BackupPath}//{date.ToString("yyyyMMdd")}";
            if (!Directory.Exists(forderPath))
                Directory.CreateDirectory(forderPath);

            string query = "SELECT 'SAVE_TIME','CEQID','COMM_TIME','CPSID','CIRCUITNO','NAME','DL','diagnostics','VOLTAGE_UNBALANCE','CURRENT_UNBALANCE','FREQUENCY'"
                    + ",'CURRENT_A','CURRENT_B','CURRENT_C','CURRENT_N','VOLTAGE_A','VOLTAGE_B','VOLTAGE_C','APPARENT_POWER_A','APPARENT_POWER_B','APPARENT_POWER_C'"
                    + ",'POWER_FACTOR_3P','POWER_FACTOR_A','POWER_FACTOR_B','POWER_FACTOR_C','FAULT_CURRENT_A','FAULT_CURRENT_B','FAULT_CURRENT_C','FAULT_CURRENT_N'"
                    + ",'CURRENT_PHASE_A','CURRENT_PHASE_B','CURRENT_PHASE_C','CURRENT_PHASE_N','VOLTAGE_PHASE_A','VOLTAGE_PHASE_B','VOLTAGE_PHASE_C'"
                    + " UNION ALL"
                    + $" SELECT * from {tableName}"
                    + $" INTO OUTFILE '{forderPath}//{tableName}_{dateStr}.csv'"
                    + " FIELDS"
                    + " TERMINATED BY ','"
                    + " ENCLOSED BY '\"'"
                    + " LINES"
                    + " TERMINATED BY '\\n'";

            try
            {
                using (MySqlMapper mapper = new MySqlMapper(_configuration))
                {
                    retval = mapper.RunQuery(query);
                    if (retval)
                        _logger.DbLog($"[1분 실시간] {tableName}_{dateStr} 파일 백업 완료");
                }
            }
            catch (Exception ex)
            {
                _logger.DbLog($"[1분 실시간] {tableName}_{dateStr} 파일 백업 실패 ex:{ex.Message}");
            }
            return retval;
        }

        public bool DayStatTableDrop(DateTime date, int day)
        {
            bool retval = false;

            retval = DayStatTableBackup(date, day);
            if (!retval)
                return false;

            try
            {
                string query = $"drop Table IF exists history_daystat_data_{date.AddDays(-day).ToString("yyyy")}";
                using (MySqlMapper mapper = new MySqlMapper(_configuration))
                {
                    retval = mapper.RunQuery(query);
                    if (retval)
                        _logger.DbLog($"[1일 통계(1분실시간전류)] history_daystat_data_{date.AddDays(-day).ToString("yyyy")} 테이블 삭제");
                }
            }
            catch (Exception ex)
            {
                _logger.DbLog($"[1일 통계(1분실시간전류)] history_daystat_data_{date.AddDays(-day).ToString("yyyy")} 테이블 삭제 실패 ex:{ex.Message}");
            }
            return retval;
        }

        public bool DayStatTableBackup(DateTime date, int day)
        {
            bool retval = false;

            var dayStr = date.AddDays(-day).ToString("yyyy");
            var dateStr = date.ToString("yyyyMMddHHmm");
            var tableName = $"history_daystat_data_{dayStr}";

            var forderPath = $"{BackupPath}//{date.ToString("yyyyMMdd")}";
            if (!Directory.Exists(forderPath))
                Directory.CreateDirectory(forderPath);

            string query = "SELECT 'SAVE_TIME','CEQID','COMM_TIME','CPSID','CIRCUITNO','NAME','DL','DIAGNOSTICS','VOLTAGE_UNBALANCE','CURRENT_UNBALANCE','FREQUENCY'"
                    + " ,'AVERAGE_CURRENT_A','AVERAGE_CURRENT_B','AVERAGE_CURRENT_C','AVERAGE_CURRENT_N'"
                    + " ,'MAX_CURRENT_A','MAX_CURRENT_B','MAX_CURRENT_C','MAX_CURRENT_N','MAX_COMM_TIME'"
                    + " ,'MIN_CURRENT_A','MIN_CURRENT_B','MIN_CURRENT_C','MIN_CURRENT_N','MIN_COMM_TIME'"
                    + " UNION ALL"
                    + $" SELECT * from {tableName}"
                    + $" INTO OUTFILE '{forderPath}//{tableName}_{dateStr}.csv'"
                    + " FIELDS"
                    + " TERMINATED BY ','"
                    + " ENCLOSED BY '\"'"
                    + " LINES"
                    + " TERMINATED BY '\\n'";

            try
            {
                using (MySqlMapper mapper = new MySqlMapper(_configuration))
                {
                    retval = mapper.RunQuery(query);
                    if (retval)
                        _logger.DbLog($"[1일 통계(1분실시간전류)] {tableName}_{dateStr} 파일 백업 완료");
                }
            }
            catch (Exception ex)
            {
                _logger.DbLog($"[1일 통계(1분실시간전류)] {tableName}_{dateStr} 파일 백업 실패 ex:{ex.Message}");
            }
            return retval;
        }


        public bool StatisticsMinTableDelete(DateTime date, int day)
        {
            bool retval = false;

            retval = StatisticsMinTableBackup(date, day);
            if (!retval)
                return false;
            try
            {
                string query = $"delete from statistics_15min where save_time < '{date.AddDays(-day).ToString("yyyy-MM-dd 00:00:00")}'";
                using (MySqlMapper mapper = new MySqlMapper(_configuration))
                {
                    retval = mapper.RunQuery(query);
                    if (retval)
                        _logger.DbLog($"[15분 실시간(평균부하전류)] statistics_15min 테이블 {date.AddDays(-day).ToString("yyyy-MM-dd 00:00:00")} 이전 데이터 삭제");
                }
            }
            catch (Exception ex)
            {
                _logger.DbLog($"[15분 실시간(평균부하전류)] statistics_15min 데이터 삭제 실패 ex:{ex.Message}");
            }
            return retval;
        }

        public bool StatisticsMinTableBackup(DateTime date, int day)
        {
            bool retval = false;

            var dayStr = date.AddDays(-day).ToString("yyyy-MM-dd 00:00:00");
            var dateStr = date.ToString("yyyyMMddHHmm");
            var tableName = $"statistics_15min";

            var forderPath = $"{BackupPath}//{date.ToString("yyyyMMdd")}";
            if (!Directory.Exists(forderPath))
                Directory.CreateDirectory(forderPath);

            string query = "SELECT 'SAVE_TIME','CEQID','CPSID','CIRCUITNO','NAME','DL'"
                    + " ,'AVERAGE_CURRENT_A','AVERAGE_CURRENT_B','AVERAGE_CURRENT_C','AVERAGE_CURRENT_N','COMM_TIME'"
                    + " UNION ALL"
                    + $" SELECT * from {tableName} where save_time < '{dayStr}'"
                    + $" INTO OUTFILE '{forderPath}//{tableName}_{dateStr}.csv'"
                    + " FIELDS"
                    + " TERMINATED BY ','"
                    + " ENCLOSED BY '\"'"
                    + " LINES"
                    + " TERMINATED BY '\\n'";

            try
            {
                using (MySqlMapper mapper = new MySqlMapper(_configuration))
                {
                    retval = mapper.RunQuery(query);
                    if (retval)
                        _logger.DbLog($"[15분 실시간(평균부하전류)] {tableName}_{dateStr} 파일 백업 완료");
                }
            }
            catch (Exception ex)
            {
                _logger.DbLog($"[15분 실시간(평균부하전류)] {tableName}_{dateStr} 파일 백업 실패 ex:{ex.Message}");
            }
            return retval;
        }

        public bool StatisticsHourDataDelete(DateTime date, int day)
        {
            bool retval = false;
            retval = StatisticsHourDataBackup(date, day);
            if (!retval)
                return false;

            try
            {
                string query = $"delete from statistics_hour where save_time < '{date.AddDays(-day).ToString("yyyy-MM-dd 00:00:00")}'";
                using (MySqlMapper mapper = new MySqlMapper(_configuration))
                {
                    retval = mapper.RunQuery(query);
                    if (retval)
                        _logger.DbLog($"[시간 통계(평균부하전류)] statistics_hour 테이블 {date.AddDays(-day).ToString("yyyy-MM-dd 00:00:00")} 이전 데이터 삭제");
                }
            }
            catch (Exception ex)
            {
                _logger.DbLog($"[시간 통계(평균부하전류)] statistics_hour 테이블 데이터 삭제 실패 ex:{ex.Message}");
            }
            return retval;
        }

        public bool StatisticsHourDataBackup(DateTime date, int day)
        {
            bool retval = false;

            var dayStr = date.AddDays(-day).ToString("yyyy-MM-dd 00:00:00");
            var dateStr = date.ToString("yyyyMMddHHmm");
            var tableName = $"statistics_hour";

            var forderPath = $"{BackupPath}//{date.ToString("yyyyMMdd")}";
            if (!Directory.Exists(forderPath))
                Directory.CreateDirectory(forderPath);

            string query = "SELECT 'SAVE_TIME','CEQID','CPSID','CIRCUITNO','NAME','DL'"
                    + " ,'AVERAGE_CURRENT_A','AVERAGE_CURRENT_B','AVERAGE_CURRENT_C','AVERAGE_CURRENT_N'"
                    + " ,'MAX_CURRENT_A','MAX_CURRENT_B','MAX_CURRENT_C','MAX_CURRENT_N','MAX_COMM_TIME'"
                    + " ,'MIN_CURRENT_A','MIN_CURRENT_B','MIN_CURRENT_C','MIN_CURRENT_N','MIN_COMM_TIME'"
                    + " UNION ALL"
                    + $" SELECT * from {tableName} where save_time < '{dayStr}'"
                    + $" INTO OUTFILE '{forderPath}//{tableName}_{dateStr}.csv'"
                    + " FIELDS"
                    + " TERMINATED BY ','"
                    + " ENCLOSED BY '\"'"
                    + " LINES"
                    + " TERMINATED BY '\\n'";

            try
            {
                using (MySqlMapper mapper = new MySqlMapper(_configuration))
                {
                    retval = mapper.RunQuery(query);
                    if (retval)
                        _logger.DbLog($"[시간 통계(평균부하전류)] {tableName}_{dateStr} 파일 백업 완료");
                }
            }
            catch (Exception ex)
            {
                _logger.DbLog($"[시간 통계(평균부하전류)] {tableName}_{dateStr} 파일 백업 실패 ex:{ex.Message}");
            }
            return retval;
        }

        public bool StatisticsDayDataDelete(DateTime date, int day)
        {
            bool retval = false;
            retval = StatisticsDayDataBakcup(date, day);
            if (!retval)
                return false;

            try
            {
                string query = $"delete from statistics_day where save_time < '{date.AddDays(-day).ToString("yyyy-MM-dd 00:00:00")}'";
                using (MySqlMapper mapper = new MySqlMapper(_configuration))
                {
                    retval = mapper.RunQuery(query);
                    if (retval)
                        _logger.DbLog($"[일 통계(평균부하전류)] statistics_day 테이블 {date.AddDays(-day).ToString("yyyy-MM-dd 00:00:00")} 이전 데이터 삭제");
                }
            }
            catch (Exception ex)
            {
                _logger.DbLog($"[일 통계(평균부하전류)] statistics_day 테이블 데이터 삭제 실패 ex:{ex.Message}");
            }
            return retval;
        }

        public bool StatisticsDayDataBakcup(DateTime date, int day)
        {
            bool retval = false;

            var dayStr = date.AddDays(-day).ToString("yyyy-MM-dd 00:00:00");
            var dateStr = date.ToString("yyyyMMddHHmm");
            var tableName = $"statistics_day";

            var forderPath = $"{BackupPath}//{date.ToString("yyyyMMdd")}";
            if (!Directory.Exists(forderPath))
                Directory.CreateDirectory(forderPath);

            string query = "SELECT 'SAVE_TIME','CEQID','CPSID','CIRCUITNO','NAME','DL'"
                    + " ,'AVERAGE_CURRENT_A','AVERAGE_CURRENT_B','AVERAGE_CURRENT_C','AVERAGE_CURRENT_N'"
                    + " ,'MAX_CURRENT_A','MAX_CURRENT_B','MAX_CURRENT_C','MAX_CURRENT_N','MAX_COMM_TIME'"
                    + " ,'MIN_CURRENT_A','MIN_CURRENT_B','MIN_CURRENT_C','MIN_CURRENT_N','MIN_COMM_TIME'"
                    + " UNION ALL"
                    + $" SELECT * from {tableName} where save_time < '{dayStr}'"
                    + $" INTO OUTFILE '{forderPath}//{tableName}_{dateStr}.csv'"
                    + " FIELDS"
                    + " TERMINATED BY ','"
                    + " ENCLOSED BY '\"'"
                    + " LINES"
                    + " TERMINATED BY '\\n'";

            try
            {
                using (MySqlMapper mapper = new MySqlMapper(_configuration))
                {
                    retval = mapper.RunQuery(query);
                    if (retval)
                        _logger.DbLog($"[일 통계(평균부하전류)] {tableName}_{dateStr} 파일 백업 완료");
                }
            }
            catch (Exception ex)
            {
                _logger.DbLog($"[일 통계(평균부하전류)] {tableName}_{dateStr} 파일 백업 실패 ex:{ex.Message}");
            }
            return retval;
        }

        public bool StatisticsMonthDataDelete(DateTime date, int day)
        {
            bool retval = false;
            retval = StatisticsMonthDataBackup(date, day);
            if (!retval)
                return false;

            try
            {
                string query = $"delete from statistics_month where save_time < '{date.AddDays(-day).ToString("yyyy-MM-dd 00:00:00")}'";
                using (MySqlMapper mapper = new MySqlMapper(_configuration))
                {
                    retval = mapper.RunQuery(query);
                    if (retval)
                        _logger.DbLog($"[월 통계(평균부하전류)] statistics_month 테이블 {date.AddDays(-day).ToString("yyyy-MM-dd 00:00:00")} 이전 데이터 삭제");
                }
            }
            catch (Exception ex)
            {
                _logger.DbLog($"[월 통계(평균부하전류)] statistics_month 테이블 데이터 삭제 실패 ex:{ex.Message}");
            }
            return retval;
        }

        public bool StatisticsMonthDataBackup(DateTime date, int day)
        {
            bool retval = false;

            var dayStr = date.AddDays(-day).ToString("yyyy-MM-dd 00:00:00");
            var dateStr = date.ToString("yyyyMMddHHmm");
            var tableName = $"statistics_month";

            var forderPath = $"{BackupPath}//{date.ToString("yyyyMMdd")}";
            if (!Directory.Exists(forderPath))
                Directory.CreateDirectory(forderPath);

            string query = "SELECT 'SAVE_TIME','CEQID','CPSID','CIRCUITNO','NAME','DL'"
                    + " ,'AVERAGE_CURRENT_A','AVERAGE_CURRENT_B','AVERAGE_CURRENT_C','AVERAGE_CURRENT_N'"
                    + " ,'MAX_CURRENT_A','MAX_CURRENT_B','MAX_CURRENT_C','MAX_CURRENT_N','MAX_COMM_TIME'"
                    + " ,'MIN_CURRENT_A','MIN_CURRENT_B','MIN_CURRENT_C','MIN_CURRENT_N','MIN_COMM_TIME'"
                    + " UNION ALL"
                    + $" SELECT * from {tableName} where save_time < '{dayStr}'"
                    + $" INTO OUTFILE '{forderPath}//{tableName}_{dateStr}.csv'"
                    + " FIELDS"
                    + " TERMINATED BY ','"
                    + " ENCLOSED BY '\"'"
                    + " LINES"
                    + " TERMINATED BY '\\n'";

            try
            {
                using (MySqlMapper mapper = new MySqlMapper(_configuration))
                {
                    retval = mapper.RunQuery(query);
                    if (retval)
                        _logger.DbLog($"[월 통계(평균부하전류)] {tableName}_{dateStr} 파일 백업 완료");
                }
            }
            catch (Exception ex)
            {
                _logger.DbLog($"[월 통계(평균부하전류)] {tableName}_{dateStr} 파일 백업 실패 ex:{ex.Message}");
            }
            return retval;
        }

        public bool StatisticsYearDataDelete(DateTime date, int day)
        {
            bool retval = false;
            retval = StatisticsYearDataBackup(date, day);
            if (!retval)
                return false;

            try
            {
                string query = $"delete from statistics_year where save_time < '{date.AddDays(-day).ToString("yyyy-MM-dd 00:00:00")}'";
                using (MySqlMapper mapper = new MySqlMapper(_configuration))
                {
                    retval = mapper.RunQuery(query);
                    if (retval)
                        _logger.DbLog($"[년 통계(평균부하전류)] statistics_year 테이블 {date.AddDays(-day).ToString("yyyy-MM-dd 00:00:00")} 이전 데이터 삭제");
                }
            }
            catch (Exception ex)
            {
                _logger.DbLog($"[년 통계(평균부하전류)] statistics_year 테이블 데이터 삭제 실패 ex:{ex.Message}");
            }
            return retval;
        }

        public bool StatisticsYearDataBackup(DateTime date, int day)
        {
            bool retval = false;

            var dayStr = date.AddDays(-day).ToString("yyyy-MM-dd 00:00:00");
            var dateStr = date.ToString("yyyyMMddHHmm");
            var tableName = $"statistics_year";

            var forderPath = $"{BackupPath}//{date.ToString("yyyyMMdd")}";
            if (!Directory.Exists(forderPath))
                Directory.CreateDirectory(forderPath);

            string query = "SELECT 'SAVE_TIME','CEQID','CPSID','CIRCUITNO','NAME','DL'"
                    + " ,'AVERAGE_CURRENT_A','AVERAGE_CURRENT_B','AVERAGE_CURRENT_C','AVERAGE_CURRENT_N'"
                    + " ,'MAX_CURRENT_A','MAX_CURRENT_B','MAX_CURRENT_C','MAX_CURRENT_N','MAX_COMM_TIME'"
                    + " ,'MIN_CURRENT_A','MIN_CURRENT_B','MIN_CURRENT_C','MIN_CURRENT_N','MIN_COMM_TIME'"
                    + " UNION ALL"
                    + $" SELECT * from {tableName} where save_time < '{dayStr}'"
                    + $" INTO OUTFILE '{forderPath}//{tableName}_{dateStr}.csv'"
                    + " FIELDS"
                    + " TERMINATED BY ','"
                    + " ENCLOSED BY '\"'"
                    + " LINES"
                    + " TERMINATED BY '\\n'";

            try
            {
                using (MySqlMapper mapper = new MySqlMapper(_configuration))
                {
                    retval = mapper.RunQuery(query);
                    if (retval)
                        _logger.DbLog($"[년 통계(평균부하전류)] {tableName}_{dateStr} 파일 백업 완료");
                }
            }
            catch (Exception ex)
            {
                _logger.DbLog($"[년 통계(평균부하전류)] {tableName}_{dateStr} 파일 백업 실패 ex:{ex.Message}");
            }
            return retval;
        }

        public bool FiAlarmDataDelete(DateTime date, int day)
        {
            bool retval = false;
            retval = FiAlarmDataBackup(date, day);
            if (!retval)
                return false;

            try
            {
                string query = $"delete from history_fi_alarm where save_time < '{date.AddDays(-day).ToString("yyyy-MM-dd 00:00:00")}'";
                using (MySqlMapper mapper = new MySqlMapper(_configuration))
                {
                    retval = mapper.RunQuery(query);
                    if (retval)
                        _logger.DbLog($"[알람 실시간] history_fi_alarm 테이블 {date.AddDays(-day).ToString("yyyy-MM-dd 00:00:00")} 이전 데이터 삭제");
                }
            }
            catch (Exception ex)
            {
                _logger.DbLog($"[알람 실시간] history_fi_alarm 테이블 데이터 삭제 실패 ex:{ex.Message}");
            }
            return retval;
        }

        public bool FiAlarmDataBackup(DateTime date, int day)
        {
            bool retval = false;

            var dayStr = date.AddDays(-day).ToString("yyyy-MM-dd 00:00:00");
            var dateStr = date.ToString("yyyyMMddHHmm");
            var tableName = $"history_fi_alarm";

            var forderPath = $"{BackupPath}//{date.ToString("yyyyMMdd")}";
            if (!Directory.Exists(forderPath))
                Directory.CreateDirectory(forderPath);

            string query = "SELECT 'SAVE_TIME','CEQID','LOG_TIME','FRTU_TIME','CPSID','CIRCUITNO','NAME','DL','ALARM_NAME'"
                    + " ,'VALUE','LOG_DESC','FAULT_CURRENT_A','FAULT_CURRENT_B','FAULT_CURRENT_C','FAULT_CURRENT_N'"
                    + " UNION ALL"
                    + $" SELECT * from {tableName} where save_time < '{dayStr}'"
                    + $" INTO OUTFILE '{forderPath}//{tableName}_{dateStr}.csv'"
                    + " FIELDS"
                    + " TERMINATED BY ','"
                    + " ENCLOSED BY '\"'"
                    + " LINES"
                    + " TERMINATED BY '\\n'";

            try
            {
                using (MySqlMapper mapper = new MySqlMapper(_configuration))
                {
                    retval = mapper.RunQuery(query);
                    if (retval)
                        _logger.DbLog($"[알람 실시간] {tableName}_{dateStr} 파일 백업 완료");
                }
            }
            catch (Exception ex)
            {
                _logger.DbLog($"[알람 실시간] {tableName}_{dateStr} 파일 백업 실패 ex:{ex.Message}");
            }
            return retval;
        }

        public bool CommStateDataDelete(DateTime date, int day)
        {
            bool retval = false;
            retval = CommStateDataBackup(date, day);
            if (!retval)
                return false;

            try
            {
                string query = $"delete from history_comm_state where save_time < '{date.AddDays(-day).ToString("yyyy-MM-dd 00:00:00")}'";
                using (MySqlMapper mapper = new MySqlMapper(_configuration))
                {
                    retval = mapper.RunQuery(query);
                    if (retval)
                        _logger.DbLog($"[통신 성공률] history_comm_state 테이블 {date.AddDays(-day).ToString("yyyy-MM-dd 00:00:00")} 이전 데이터 삭제");
                }
            }
            catch (Exception ex)
            {
                _logger.DbLog($"[통신 성공률] history_comm_state 테이블 데이터 삭제 실패 ex:{ex.Message}");
            }
            return retval;
        }

        public bool CommStateDataBackup(DateTime date, int day)
        {
            bool retval = false;

            var dayStr = date.AddDays(-day).ToString("yyyy-MM-dd 00:00:00");
            var dateStr = date.ToString("yyyyMMddHHmm");
            var tableName = $"history_comm_state";

            var forderPath = $"{BackupPath}//{date.ToString("yyyyMMdd")}";
            if (!Directory.Exists(forderPath))
                Directory.CreateDirectory(forderPath);

            string query = "SELECT 'SAVE_TIME','EQ_TYPE','CEQID','CPSID','NAME','DL'"
                    + " ,'COMM_TOTAL_COUNT','COMM_SUCESS_COUNT','COMM_FAIL_COUNT','COMM_SUCESS_RATE','COMM_TIME'"
                    + " UNION ALL"
                    + $" SELECT * from {tableName} where save_time < '{dayStr}'"
                    + $" INTO OUTFILE '{forderPath}//{tableName}_{dateStr}.csv'"
                    + " FIELDS"
                    + " TERMINATED BY ','"
                    + " ENCLOSED BY '\"'"
                    + " LINES"
                    + " TERMINATED BY '\\n'";

            try
            {
                using (MySqlMapper mapper = new MySqlMapper(_configuration))
                {
                    retval = mapper.RunQuery(query);
                    if (retval)
                        _logger.DbLog($"[통신 성공률] {tableName}_{dateStr} 파일 백업 완료");
                }
            }
            catch (Exception ex)
            {
                _logger.DbLog($"[통신 성공률] {tableName}_{dateStr} 파일 백업 실패 ex:{ex.Message}");
            }
            return retval;
        }

        public bool CommStateLogDataDelete(DateTime date, int day)
        {
            bool retval = false;
            retval = CommStateLogDataBackup(date, day);
            if (!retval)
                return false;

            try
            {
                string query = $"delete from history_comm_state_log where save_time < '{date.AddDays(-day).ToString("yyyy-MM-dd 00:00:00")}'";
                using (MySqlMapper mapper = new MySqlMapper(_configuration))
                {
                    retval = mapper.RunQuery(query);
                    if (retval)
                        _logger.DbLog($"[통신 상태 이력] history_comm_state_log 테이블 {date.AddDays(-day).ToString("yyyy-MM-dd 00:00:00")} 이전 데이터 삭제");
                }
            }
            catch (Exception ex)
            {
                _logger.DbLog($"[통신 상태 이력] history_comm_state_log 테이블 데이터 삭제 실패 ex:{ex.Message}");
            }
            return retval;
        }

        public bool CommStateLogDataBackup(DateTime date, int day)
        {
            bool retval = false;

            var dayStr = date.AddDays(-day).ToString("yyyy-MM-dd 00:00:00");
            var dateStr = date.ToString("yyyyMMddHHmm");
            var tableName = $"history_comm_state_log";

            var forderPath = $"{BackupPath}//{date.ToString("yyyyMMdd")}";
            if (!Directory.Exists(forderPath))
                Directory.CreateDirectory(forderPath);

            string query = "SELECT 'SAVE_TIME','EQ_TYPE','CEQID','CPSID','NAME','DL','COMM_STATE'"
                    + " ,'COMM_TOTAL_COUNT','COMM_SUCESS_COUNT','COMM_FAIL_COUNT','COMM_SUCESS_RATE','COMM_TIME'"
                    + " UNION ALL"
                    + $" SELECT * from {tableName} where save_time < '{dayStr}'"
                    + $" INTO OUTFILE '{forderPath}//{tableName}_{dateStr}.csv'"
                    + " FIELDS"
                    + " TERMINATED BY ','"
                    + " ENCLOSED BY '\"'"
                    + " LINES"
                    + " TERMINATED BY '\\n'";

            try
            {
                using (MySqlMapper mapper = new MySqlMapper(_configuration))
                {
                    retval = mapper.RunQuery(query);
                    if (retval)
                        _logger.DbLog($"[통신 상태 이력] {tableName}_{dateStr} 파일 백업 완료");
                }
            }
            catch (Exception ex)
            {
                _logger.DbLog($"[통신 상태 이력] {tableName}_{dateStr} 파일 백업 실패 ex:{ex.Message}");
            }
            return retval;
        }
    }
}

