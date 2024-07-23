using DevExpress.ClipboardSource.SpreadsheetML;
using DevExpress.Mvvm.Native;
using DevExpress.Pdf;
using DevExpress.Utils;
using DevExpress.Xpf.Bars;
using KDMS.EF.Core.Contexts;
using KDMS.EF.Core.Extensions;
using KDMS.EF.Core.Infrastructure.Reverse;
using KDMS.EF.Core.Infrastructure.Reverse.Models;
using KdmsTcpSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using Serilog;
using System.Data;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Media;
using static DevExpress.Utils.HashCodeHelper.Primitives;
using static DevExpress.XtraPrinting.Native.ExportOptionsPropertiesNames;
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
        public List<PdbList> pdbLists { get; set; }
        private List<AiInfo> AiInfos { get; set; }
        private List<Distributionline> Distributionlines { get; set; }
        private List<PdbRemoteunit> Remoteunits { get; set; }

        // PDB 데이터
        public List<pdb_Discrete> pdbDiscretes = new List<pdb_Discrete>();
        public List<pdb_Command> pdbCommands = new List<pdb_Command>();
        public List<pdb_Analog> pdbAnalogs = new List<pdb_Analog>();
        public List<pdb_SetPoint> pdbSetPoints = new List<pdb_SetPoint>();
        public List<pdb_Accumulator> pdbAccumulators = new List<pdb_Accumulator>();
        public List<pdb_Dmc> pdbDmcs = new List<pdb_Dmc>();
        public List<pdb_ConductingEquipment> PdbConductingequipments = new List<pdb_ConductingEquipment>();
        public List<rtdb_Analog> rtdbAnalogs = new List<rtdb_Analog>();
        public List<rtdb_Dmc> rtdbDmcs = new List<rtdb_Dmc>();

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
            pdbLists = _kdmsContext.PdbLists.ToList();

            AiInfos = _kdmsContext.AiInfos.ToList();
            Distributionlines = _kdmsContext.Distributionlines.ToList();
            Remoteunits = _kdmsContext.PdbRemoteunits.Where(p => p.DmcFk != 0).ToList();
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
            var pid = analogList.FirstOrDefault(p => p.dp_fk == dataPointId).pid;
            if(rtdbAnalogs.Count <= 0)
                return now;

            var value = rtdbAnalogs.FirstOrDefault(p => p.pid == pid).last_update;
            if(value == 0)
                return now;

            return KdmsValueConverter.TimeTToDateTime(value);
        }

        private int GetMinDataCircuitno(List<pdb_Analog> analogList, int dataPointId)
        {
            var pid = analogList.FirstOrDefault(p => p.dp_fk == dataPointId).pid;
            if (rtdbAnalogs.Count <= 0)
                return 0;

            var find = rtdbAnalogs.FirstOrDefault(p => p.pid == pid);
            if(find.pid > 0)
                return (int)analogList.FirstOrDefault(p => p.dp_fk == dataPointId).circuit_no;
            
            return 0;
        }

        private float GetMinDataValue(List<pdb_Analog> analogList, int dataPointId)
        {
            var pid = analogList.FirstOrDefault(p => p.dp_fk == dataPointId).pid;
            if (rtdbAnalogs.Count <= 0)
                return 0;

            return (float)rtdbAnalogs.FirstOrDefault(p => p.pid == pid).value;
        }

        public void MinDataSave(DateTime date/*, List<rtdb_Analog> rtList, List<pdb_Analog> analogList, List<pdb_ConductingEquipment> equipmentList*/)
        {
            List<HistoryMinDatum> dataList = new List<HistoryMinDatum>();
            foreach (var equipment in PdbConductingequipments)
            {
                if (equipment.ceqid <= 0)
                    continue;

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
                            data.CommTime = GetMinDataTime(ceqAnalogList, AiInfos.FirstOrDefault(p => p.No == 12)?.Datapointid ?? 0, date);
                            break;
                        case 4:
                            data.Cpsid = (int)equipment.ec_fk;
                            break;
                        case 5:
                            data.Circuitno = GetMinDataCircuitno(ceqAnalogList, AiInfos.FirstOrDefault(p => p.No == 12)?.Datapointid ?? 0);
                            break;
                        case 6:
                            data.Name = GetStringData(equipment.name);
                            break;
                        case 7:
                            data.Dl = Distributionlines.FirstOrDefault(p => p.Dlid == equipment.dl_fk)?.Name;
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
            List<Statistics15min> dataList = new List<Statistics15min>();
            foreach (var equipment in PdbConductingequipments)
            {
                if (equipment.ceqid <= 0)
                    continue;

                var ceqAnalogList = pdbAnalogs.Where(p => p.ceq_fk == equipment.ceqid).ToList();

                Statistics15min data = new Statistics15min();
                data.SaveTime = date;
                data.Ceqid = (int)equipment.ceqid;
                data.Cpsid = (int)equipment.ec_fk;
                data.Circuitno = GetMinDataCircuitno(ceqAnalogList, AiInfos.FirstOrDefault(p => p.No == 45)?.Datapointid ?? 0);
                data.Name = GetStringData(equipment.name);
                data.Dl = Distributionlines.FirstOrDefault(p => p.Dlid == equipment.dl_fk)?.Name;
                data.AverageCurrentA = GetMinDataValue(ceqAnalogList, 45);
                data.AverageCurrentB = GetMinDataValue(ceqAnalogList, 46);
                data.AverageCurrentC = GetMinDataValue(ceqAnalogList, 47);
                data.AverageCurrentN = GetMinDataValue(ceqAnalogList, 65);
                data.CommTime = GetMinDataTime(ceqAnalogList, AiInfos.FirstOrDefault(p => p.No == 45)?.Datapointid ?? 0, date);

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

            return rtdbDmcs.FirstOrDefault(p => p.pid == dmcFk).value == 1 ? true : false;
        }

        private float GetCommDataValue(int commDmcFk)
        {
            if (rtdbDmcs.Count <= 0)
                return 0;

            return (float)rtdbDmcs.FirstOrDefault(p => p.pid == commDmcFk).value;
        }

        private DateTime GetCommDataTime(int commDmcFk, DateTime now)
        {
            if (rtdbDmcs.Count <= 0)
                return now;

            var value = rtdbDmcs.FirstOrDefault(p => p.pid == commDmcFk).last_update;
            if (value == 0)
                return now;

            return KdmsValueConverter.TimeTToDateTime(value);
        }

        public void CommStateDataSave(DateTime date/*, List<rtdb_Dmc> rtList, List<pdb_ConductingEquipment> equipmentList*/)
        {
            List<HistoryCommState> dataList = new List<HistoryCommState>();

            foreach(var remote in Remoteunits)
            {
                if(remote.EqType == 1)
                {
                    var equipment = PdbConductingequipments.FirstOrDefault(p => p.ceqid != 0 && p.ceqid == remote.EqFk);
                    if (equipment.ceqid <= 0)
                        continue;

                        HistoryCommState data = new HistoryCommState();
                        data.SaveTime = date;
                        data.Dl = Distributionlines.FirstOrDefault(p => p.Dlid == equipment.dl_fk)?.Name;
                        data.Name = GetStringData(equipment.name);

                        data.EqType = remote.EqType ?? 0;
                        data.Ceqid = (int)equipment.ceqid;
                        data.Cpsid = 0;

                        data.CommSucessCount = (int)GetCommDataValue(Convert.ToInt32(remote.CommDmcFk + 3000 ?? 0));
                        data.CommFailCount = (int)GetCommDataValue(Convert.ToInt32(remote.CommDmcFk + 6000 ?? 0));
                        data.CommTotalCount = data.CommSucessCount + data.CommFailCount;
                        data.CommSucessRate = GetCommDataValue(Convert.ToInt32(remote.CommDmcFk ?? 0));
                        data.CommTime = GetCommDataTime(Convert.ToInt32(remote.CommDmcFk ?? 0), date);

                        dataList.Add(data);
                }
                else
                {
                    var findList = PdbConductingequipments.Where(p => p.ceqid != 0 && p.ec_fk == remote.EqFk).ToList();
                    if (findList.Count <= 0)
                        continue;

                    foreach (var equipment in findList)
                    {
                        HistoryCommState data = new HistoryCommState();
                        data.SaveTime = date;
                        data.Dl = Distributionlines.FirstOrDefault(p => p.Dlid == equipment.dl_fk)?.Name;
                        data.Name = GetStringData(equipment.name);

                        data.EqType = remote.EqType ?? 0;
                        data.Ceqid = (int)equipment.ceqid;
                        data.Cpsid = (int)equipment.ec_fk;

                        data.CommSucessCount = (int)GetCommDataValue(Convert.ToInt32(remote.CommDmcFk + 3000 ?? 0));
                        data.CommFailCount = (int)GetCommDataValue(Convert.ToInt32(remote.CommDmcFk + 6000 ?? 0));
                        data.CommTotalCount = data.CommSucessCount + data.CommFailCount;
                        data.CommSucessRate = GetCommDataValue(Convert.ToInt32(remote.CommDmcFk ?? 0));
                        data.CommTime = GetCommDataTime(Convert.ToInt32(remote.CommDmcFk ?? 0), date);

                        dataList.Add(data);
                    }
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

        public void CommStateLogDataSave(List<rtdb_Alarm> alarmList)
        {
            var date = DateTime.Now;
            List<HistoryCommStateLog> dataList = new List<HistoryCommStateLog>();
            foreach (var alarm in alarmList)
            {
                var remote = Remoteunits.FirstOrDefault(p => p.Pid == alarm.uiRtuid);
                if (remote == null)
                    continue;

                var dl = Distributionlines.FirstOrDefault(p => p.Dlid == alarm.uiDL);
                if (dl == null)
                    continue;

                var equipment = PdbConductingequipments.FirstOrDefault(p => p.ceqid == alarm.uiEqid);
                if (equipment.ceqid <= 0)
                    continue;

                HistoryCommStateLog data = new HistoryCommStateLog();
                data.SaveTime = date;
                data.Dl = dl.Name;
                data.Name = GetStringData(equipment.name);

                data.EqType = remote.EqType ?? 0;
                data.Ceqid = (int)equipment.ceqid;
                data.Cpsid = 0;
                data.CommState = GetCommStateValue(Convert.ToInt32(remote.DmcFk ?? 0));
                data.CommSucessCount = (int)GetCommDataValue(Convert.ToInt32(remote.CommDmcFk + 3000 ?? 0));
                data.CommFailCount = (int)GetCommDataValue(Convert.ToInt32(remote.CommDmcFk + 6000 ?? 0));
                data.CommTotalCount = data.CommSucessCount + data.CommFailCount;
                data.CommSucessRate = GetCommDataValue(Convert.ToInt32(remote.CommDmcFk ?? 0));
                data.CommTime = GetCommDataTime(Convert.ToInt32(remote.CommDmcFk ?? 0), date);

                dataList.Add(data);
            }
            
            if (dataList.Count > 0)
            {
                try
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine($"insert into history_comm_state_log values ");
                    int cnt = 0;
                    foreach (var data in dataList)
                    {
                        cnt++;
                        sb.Append($"('{data.SaveTime.ToString("yyyy-MM-dd HH:mm:ss")}', {data.EqType}, {data.Ceqid}, {data.Cpsid}, '{data.Name?.Trim()}', '{data.Dl?.Trim()}', {data.CommState}" +
                            $" {data.CommTotalCount}, {data.CommSucessCount}, {data.CommFailCount}, {data.CommSucessRate:N2}, '{data.CommTime?.ToString("yyyy-MM-dd HH:mm:ss")}')");

                        if (cnt != dataList.Count)
                            sb.AppendLine(",");
                        else
                            sb.AppendLine(";");
                    }

                    using (MySqlMapper mapper = new MySqlMapper(_configuration))
                    {
                        mapper.RunQuery(sb.ToString());
                    }
                }
                catch (Exception ex)
                {
                    _logger.ServerLog($"[통신상태 이력] 데이터 입력중 예외 발생 ex:{ex.Message}");
                }
            }
        }

        public void FiAlarmDataSave(List<rtdb_Alarm> alarmList)
        {
            try
            {
                var date = DateTime.Now;
                foreach (var alarm in  alarmList)
                {
                    var find = PdbConductingequipments.FirstOrDefault(p => p.ceqid == (int)alarm.uiEqid);
                    if (find.ceqid <= 0)
                        continue;

                    var dlName = Distributionlines.FirstOrDefault(p => p.Dlid == find.dl_fk)?.Name;
                    var ceqAnalogList = pdbAnalogs.Where(p => p.ceq_fk == (int)alarm.uiEqid).ToList();

                    HistoryFiAlarm data = new HistoryFiAlarm();
                    data.SaveTime = date;
                    data.Ceqid = (int)alarm.uiEqid;
                    data.LogTime = KdmsValueConverter.TimeTToDateTime(alarm.uiSVRTime).AddMilliseconds(alarm.uiSVRMics);
                    data.FrtuTime = KdmsValueConverter.TimeTToDateTime(alarm.uiRTUTime).AddMilliseconds(alarm.uiRTUMics);
                    data.Cpsid = (int)find.ec_fk;
                    data.Name = GetStringData(find.name);
                    data.Dl = dlName;
                    data.Value = (float)alarm.fVal;
                    data.LogDesc = GetStringData(alarm.szDesc);
                    switch (alarm.uiPtType)
                    {
                        case (int)PointTypeCode.BI: 
                            {
                                data.AlarmName = GetStringData(pdbDiscretes.FirstOrDefault(p => p.pid == alarm.uiPid).name);
                                data.Circuitno = (int)pdbDiscretes.FirstOrDefault(p => p.pid == alarm.uiPid).circuit_no;
                            }
                            break;
                        case (int)PointTypeCode.BO:
                            {
                                data.AlarmName = GetStringData(pdbCommands.FirstOrDefault(p => p.pid == alarm.uiPid).name);
                                data.Circuitno = (int)pdbCommands.FirstOrDefault(p => p.pid == alarm.uiPid).circuit_no;
                            }
                            break;
                        case (int)PointTypeCode.AI:
                            {
                                data.AlarmName = GetStringData(pdbAnalogs.FirstOrDefault(p => p.pid == alarm.uiPid).name);
                                data.Circuitno = (int)pdbAnalogs.FirstOrDefault(p => p.pid == alarm.uiPid).circuit_no;
                            }
                            break;
                        case (int)PointTypeCode.AO:
                            {
                                data.AlarmName = GetStringData(pdbSetPoints.FirstOrDefault(p => p.pid == alarm.uiPid).name);
                                data.Circuitno = (int)pdbSetPoints.FirstOrDefault(p => p.pid == alarm.uiPid).circuit_no;
                            }
                            break;
                        case (int)PointTypeCode.COUNTER:
                            {
                                data.AlarmName = GetStringData(pdbAccumulators.FirstOrDefault(p => p.pid == alarm.uiPid).name);
                                data.Circuitno = (int)pdbAccumulators.FirstOrDefault(p => p.pid == alarm.uiPid).circuit_no;
                            }
                            break;
                        case (int)PointTypeCode.DMC:
                            {
                                data.AlarmName = GetStringData(pdbDmcs.FirstOrDefault(p => p.pid == alarm.uiPid).name); 
                                data.Circuitno = 0;
                            }
                            break;
                    }

                    data.FaultCurrentA = GetMinDataValue(ceqAnalogList, 51);
                    data.FaultCurrentB = GetMinDataValue(ceqAnalogList, 52);
                    data.FaultCurrentC = GetMinDataValue(ceqAnalogList, 53);
                    data.FaultCurrentN = GetMinDataValue(ceqAnalogList, 54);

                    string query = $"insert into history_fi_alarm values ('{data.SaveTime.ToString("yyyy-MM-dd HH:mm:ss")}', {data.Ceqid}, '{data.LogTime?.ToString("yyyy-MM-dd HH:mm:ss.fff")}', '{data.FrtuTime?.ToString("yyyy-MM-dd HH:mm:ss.fff")}', " +
                        $" {data.Cpsid}, {data.Circuitno}, '{data.Name?.Trim()}', '{data.Dl?.Trim()}', '{data.AlarmName}', {data.Value}, '{data.LogDesc}', {data.FaultCurrentA}, {data.FaultCurrentB}, {data.FaultCurrentC}, {data.FaultCurrentN})";

                    using (MySqlMapper mapper = new MySqlMapper(_configuration))
                    {
                        mapper.RunQuery(query);
                        //_logger.ServerLog($"[알람 실시간] {date.ToString("yyyy-MM-dd HH:mm:ss")} 데이터 입력 완료 CNT:{dataList.Count}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.ServerLog($"[알람 실시간] 데이터 입력중 예외 발생 ex:{ex.Message}");
            }
        }

        private string GetStringData(byte[] bytes)
        {
            return Encoding.Default.GetString(bytes).Trim('\0');
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


        public void DaystatDataInput(List<HistoryDaystatDatum> dataList, DateTime date)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                foreach (var data in dataList)
                {
                    sb.AppendLine($"insert into history_daystat_data_{date.ToString("yyyy")} " +
                        $"values('{data.SaveTime.ToString("yyyy-MM-dd HH:mm:ss")}', {data.Ceqid}, '{data.CommTime?.ToString("yyyy-MM-dd HH:mm:ss")}', {data.Cpsid}, {data.Circuitno}, '{data.Name}', '{data.Dl}'," +
                        $" {data.Diagnostics}, {data.VoltageUnbalance}, {data.CurrentUnbalance}, {data.Frequency}," +
                        $" {data.AverageCurrentA}, {data.AverageCurrentB}, {data.AverageCurrentC}, {data.AverageCurrentN}," +
                        $" {data.MaxCurrentA}, {data.MaxCurrentB}, {data.MaxCurrentC}, {data.MaxCurrentN}, '{data.MaxCommTime?.ToString("yyyy-MM-dd HH:mm:ss")}'," +
                        $" {data.MinCurrentA}, {data.MinCurrentB}, {data.MinCurrentC}, {data.MinCurrentN}, '{data.MinCommTime?.ToString("yyyy-MM-dd HH:mm:ss")}');");
                }
                using (MySqlMapper mapper = new MySqlMapper(_configuration))
                {
                    mapper.RunQuery(sb.ToString());
                }
            }
            catch (Exception ex)
            {
                _logger.DbLog($"일일 통계 데이터 입력중 예외 발생 ex:{ex.Message}");
            }
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
                _logger.DbLog($"{date.AddDays(1).ToString("yyyyMMdd")} 이력 실시간 테이블 생성 실패 ex:{ex.Message}");
                _logger.DbLog($"{date.AddDays(2).ToString("yyyyMMdd")} 이력 실시간 테이블 생성 실패 ex:{ex.Message}");
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
                _logger.DbLog($"{date.AddYears(1).ToString("yyyy")} 이력 통계 테이블 생성 실패 ex:{ex.Message}");
            }
            return retval;
        }
    }
}

