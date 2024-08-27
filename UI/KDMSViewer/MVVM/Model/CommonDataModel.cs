using DevExpress.Xpf.Grid;
using KDMS.EF.Core.Contexts;
using KDMS.EF.Core.Extensions;
using KDMS.EF.Core.Infrastructure.Reverse;
using KDMS.EF.Core.Infrastructure.Reverse.Models;
using KDMSViewer.Extensions;
using KDMSViewer.ViewModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Client.Extensions.Msal;
using System.Data;
using System.Text;
using System.Windows;

namespace KDMSViewer.Model
{
    public class CommonDataModel
    {
        private readonly KdmsContext _kdmsContext;
        private readonly IConfiguration _configuration;

        private List<Substation> Substations;
        private List<Distributionline> Distributionlines;
        private List<Compositeswitch> Compositeswitchs;
        private List<PdbConductingequipment> ConductingEquipments;
        private List<PdbRemoteunit> Remoteunits;
        private List<Datapointinfo> Datapointinfos;

        public CommonDataModel(KdmsContext kdmsContext, IConfiguration configuration)
        {
            _kdmsContext = kdmsContext;
            _configuration = configuration;
            DataLoad();
        }

        private void DataLoad()
        {
            // 트리 목록 생성 처리
            // 변전소 -> DL -> 개폐기
            try
            {
                Substations = _kdmsContext.Substations.ToList();
                Distributionlines = _kdmsContext.Distributionlines.ToList();
                Compositeswitchs = _kdmsContext.Compositeswitches.ToList();
                Remoteunits = _kdmsContext.PdbRemoteunits.ToList();
                Datapointinfos = _kdmsContext.Datapointinfos.Where(p => p.Pointtype == 3).ToList();

                ConductingEquipments = new List<PdbConductingequipment>();
                //var conductingEquipmentList = _kdmsContext.PdbConductingequipments.Where(p => (p.Psrtype >= 58 && p.Psrtype <= 88) || p.Psrtype == 105).ToList();
                var conductingEquipmentList = _kdmsContext.PdbConductingequipments.Where(p => p.RtuType == 2).ToList();
                foreach (var equipment in conductingEquipmentList)
                {
                    var findRemoteunit = Remoteunits.FirstOrDefault(p => p.EqType == 1 && p.EqFk == equipment.Ceqid);
                    if (findRemoteunit != null)
                    {
                        if (findRemoteunit.ProtocolFk == 0 && findRemoteunit.CommType == 0)
                            continue;
                    }

                    findRemoteunit = Remoteunits.FirstOrDefault(p => p.EqType == 2 && p.EqFk == equipment.EcFk);
                    if (findRemoteunit != null)
                    {
                        if (findRemoteunit.ProtocolFk == 0 && findRemoteunit.CommType == 0)
                            continue;
                    }

                    ConductingEquipments.Add(equipment);
                }
                TreeListInit();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"데이터베이스 로딩 실패 ex:{ex.Message}\n\n\r프로그램을 종료합니다.", "데이터베이스 로딩 실패", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
            }
        }

        public List<TreeDataModel> TreeListInit()
        {
            // 트리 목록 생성 처리
            var treeDatas = new List<TreeDataModel>();
            foreach (var substation in Substations)
            {
                TreeDataModel subs = new TreeDataModel();
                subs.Type = TreeTypeCode.SUBS;
                subs.Id = substation.Stid;
                subs.Name = substation.Name!.Trim() ?? string.Empty;
                subs.Tooltip = $"SUBS ID: {subs.Id}";
                subs.IsExpanded = true;

                foreach (var distributionline in Distributionlines.Where(p => p.StFk == substation.Stid).OrderBy(p => p.DlNo))
                {
                    TreeDataModel dl = new TreeDataModel();
                    dl.Type = TreeTypeCode.DL;
                    dl.Id = distributionline.Dlid;
                    dl.Name = distributionline.Name!.Trim() ?? string.Empty;
                    dl.Tooltip = $"D/L ID: {dl.Id}";
                    subs.DataModels.Add(dl);

                    var multiSwList = ConductingEquipments.Where(p => p.DlFk == distributionline.Dlid && p.EcFk != 0);
                    if (multiSwList.Count() > 0)
                    {
                        var compositList = multiSwList.Select(p => p.EcFk).ToList();
                        foreach (var compositeswitch in Compositeswitchs.Where(p => p.DlFk == distributionline.Dlid && compositList.Any(x => x == p.Pid)))
                        {
                            //TreeDataModel composit = new TreeDataModel();
                            //composit.Type = TreeTypeCode.COMPOSITE;
                            //composit.Id = compositeswitch.Pid;
                            //composit.Name = compositeswitch.Name!.Trim() ?? string.Empty;
                            //composit.IsVisible = Visibility.Collapsed;
                            //dl.DataModels.Add(composit);

                            foreach (var conductingEquipment in ConductingEquipments.Where(p => p.EcFk == compositeswitch.Pid))
                            {
                                TreeDataModel equipment = new TreeDataModel();
                                equipment.Type = TreeTypeCode.EQUIPMENT;
                                equipment.Id = conductingEquipment.Ceqid;
                                equipment.Name = conductingEquipment.Name!.Trim() ?? string.Empty;
                                equipment.Tooltip = $"CEQ ID: {equipment.Id}";
                                //composit.DataModels.Add(equipment);
                                dl.DataModels.Add(equipment);
                            }
                        }
                    }

                    var baseSwList = ConductingEquipments.Where(p => p.DlFk == distributionline.Dlid && p.EcFk == 0);
                    if (baseSwList.Count() > 0)
                    {
                        foreach (var sw in baseSwList)
                        {
                            TreeDataModel equipment = new TreeDataModel();
                            equipment.Type = TreeTypeCode.EQUIPMENT;
                            equipment.Id = sw.Ceqid;
                            equipment.Name = sw.Name!.Trim() ?? string.Empty;
                            dl.DataModels.Add(equipment);
                        }
                    }

                    //foreach (var conductingEquipment in ConductingEquipments.Where(p => p.DlFk == distributionline.Dlid && p.EcFk == 0))
                    //{
                    //    TreeDataModel equipment = new TreeDataModel();
                    //    equipment.Type = TreeTypeCode.EQUIPMENT;
                    //    equipment.Id = conductingEquipment.Ceqid;
                    //    equipment.Name = conductingEquipment.Name ?? string.Empty;

                    //    dl.DataModels.Add(equipment);
                    //}

                    //foreach (var compositeswitch in Compositeswitchs.Where(p => p.DlFk == distributionline.Dlid))
                    //{
                    //    TreeDataModel composit = new TreeDataModel();
                    //    composit.Type = TreeTypeCode.COMPOSITE;
                    //    composit.Id = compositeswitch.Pid;
                    //    composit.Name = compositeswitch.Name ?? string.Empty;
                    //    dl.DataModels.Add(composit);

                    //    foreach (var conductingEquipment in ConductingEquipments.Where(p => p.EcFk == compositeswitch.Pid))
                    //    {
                    //        TreeDataModel equipment = new TreeDataModel();
                    //        equipment.Type = TreeTypeCode.EQUIPMENT;
                    //        equipment.Id = conductingEquipment.Ceqid;
                    //        equipment.Name = conductingEquipment.Name ?? string.Empty;

                    //        composit.IsVisible = Visibility.Collapsed;
                    //        composit.DataModels.Add(equipment);
                    //    }
                    //}

                    //if (conductingEquipment.EcFk != 0)
                    //{
                    //    foreach (var compositeswitch in Compositeswitchs.Where(p => p.Pid == conductingEquipment.EcFk))
                    //    {
                    //        TreeDataModel composit = new TreeDataModel();
                    //        composit.Type = TreeTypeCode.COMPOSITE;
                    //        composit.Id = compositeswitch.Pid;
                    //        composit.Name = compositeswitch.Name ?? string.Empty;
                    //        composit.IsVisible = Visibility.Collapsed;

                    //        dl.DataModels.Add(composit);
                    //    }
                    //}
                    //else
                    //{
                    //    TreeDataModel equipment = new TreeDataModel();
                    //    equipment.Type = TreeTypeCode.EQUIPMENT;
                    //    equipment.Id = conductingEquipment.Ceqid;
                    //    equipment.Name = conductingEquipment.Name ?? string.Empty;

                    //    dl.DataModels.Add(equipment);
                    //}

                    //foreach (var compositeswitch in Compositeswitchs.Where(p => p.DlFk == distributionline.Dlid))
                    //{
                    //    TreeDataModel composit = new TreeDataModel();
                    //    composit.Type = TreeTypeCode.COMPOSITE;
                    //    composit.Id = compositeswitch.Pid;
                    //    composit.SubsId = subs.Id;
                    //    composit.SubsName = subs.Name;
                    //    composit.DlId = dl.Id;
                    //    composit.DlName = dl.Name;
                    //    composit.Name = compositeswitch.Name ?? string.Empty;
                    //    dl.DataModels.Add(composit);

                    //    foreach (var conductingEquipment in ConductingEquipments.Where(p => p.EcFk == compositeswitch.Pid))
                    //    {
                    //        TreeDataModel equipment = new TreeDataModel();
                    //        equipment.Type = TreeTypeCode.EQUIPMENT;
                    //        equipment.Id = conductingEquipment.Ceqid;
                    //        equipment.SubsId = subs.Id;
                    //        equipment.SubsName = subs.Name;
                    //        equipment.DlId = dl.Id;
                    //        equipment.DlName = dl.Name;
                    //        equipment.SwId = composit.Id;
                    //        equipment.SwName = composit.Name;
                    //        equipment.Name = conductingEquipment.Name ?? string.Empty;

                    //        composit.IsVisible = Visibility.Collapsed;
                    //        composit.DataModels.Add(equipment);
                    //    }
                    //}
                }
                treeDatas.Add(subs);
            }
            return treeDatas;
        }

        public List<HistoryMinDatum> SwitchDataLoad(List<long> ceqList, DateTime fromDate, DateTime toDate)
        {
            var lst = string.Join(", ", ceqList.ToArray());
            List<HistoryMinDatum> retList = new List<HistoryMinDatum>();

            StringBuilder sb = new StringBuilder();

            if (fromDate.Day == toDate.Day)
            {
                sb.AppendLine($"select * from history_min_data_{fromDate.ToString("yyyyMMdd")} where ceqid in ({lst}) and save_time >= '{fromDate.ToString($"yyyy-MM-dd HH:mm:ss")}' and save_time <= '{toDate.ToString($"yyyy-MM-dd HH:mm:ss")}'");
            }
            else
            {
                for (DateTime st = fromDate; st < toDate; st = st.AddDays(1))
                {
                    if (st.Day == fromDate.Day)
                        sb.AppendLine($"select * from history_min_data_{st.ToString("yyyyMMdd")} where ceqid in ({lst}) and save_time >= '{st.ToString($"yyyy-MM-dd HH:mm:ss")}'");
                    else if (st.Day == toDate.Day)
                        sb.AppendLine($"select * from history_min_data_{st.ToString("yyyyMMdd")} where ceqid in ({lst}) and save_time <= '{toDate.ToString($"yyyy-MM-dd HH:mm:ss")}'");
                    else
                        sb.AppendLine($"select * from history_min_data_{st.ToString("yyyyMMdd")} where ceqid in ({lst}) and save_time >= '{st.ToString("yyyy-MM-dd 00:00:00")}' and save_time <= '{st.ToString("yyyy-MM-dd 23:59:59")}'");

                    if (st.Day != toDate.Day)
                        sb.AppendLine("union all");
                }
            }

            using (MySqlMapper mapper = new MySqlMapper(_configuration))
            {
                retList = (List<HistoryMinDatum>)mapper.ExecuteQuery<HistoryMinDatum>(sb.ToString());
            }
            return retList;
        }

        public List<HistoryDaystatDatum> DayStatDataLoad(List<long> ceqList, DateTime fromDate, DateTime toDate)
        {
            var lst = string.Join(", ", ceqList.ToArray());
            List<HistoryDaystatDatum> retList = new List<HistoryDaystatDatum>();

            StringBuilder sb = new StringBuilder();
            if (fromDate.Year == toDate.Year)
            {
                sb.AppendLine($"select * from history_daystat_data_{fromDate.ToString("yyyy")} where ceqid in ({lst}) and save_time >= '{fromDate.ToString("yyyy-MM-dd HH:mm:ss")}' and save_time <= '{toDate.ToString("yyyy-MM-dd HH:mm:ss")}'");
            }
            else
            {
                for (DateTime st = fromDate; st < toDate; st = st.AddYears(1))
                {
                    if (st.Year == fromDate.Year)
                        sb.AppendLine($"select * from history_daystat_data_{st.ToString("yyyy")} where ceqid in ({lst}) and save_time >= '{st.ToString("yyyy-MM-dd HH:mm:ss")}'");
                    else if (st.Year == toDate.Year)
                        sb.AppendLine($"select * from history_daystat_data_{st.ToString("yyyy")} where ceqid in ({lst}) and save_time <= '{toDate.ToString("yyyy-MM-dd HH:mm:ss")}'");
                    else
                        sb.AppendLine($"select * from history_daystat_data_{st.ToString("yyyy")} where ceqid in ({lst}) and save_time >= '{st.ToString("yyyy-01-01 00:00:00")}' and save_time <= '{st.ToString("yyyy-12-31 23:59:59")}'");

                    if (st.Year != toDate.Year)
                        sb.AppendLine("union all");
                }
            }

            using (MySqlMapper mapper = new MySqlMapper(_configuration))
            {
                retList = (List<HistoryDaystatDatum>)mapper.ExecuteQuery<HistoryDaystatDatum>(sb.ToString());
            }
            return retList;
        }

        public List<HistoryFiAlarm> FiDataLoad(List<long> ceqList, DateTime fromDate, DateTime toDate)
        {
            using (var context = DbContextConfigurationExtensions.CreateServerContext(_configuration))
            {
                return context.HistoryFiAlarms.Where(p => ceqList.Any(x => x == p.Ceqid) && p.SaveTime >= fromDate && p.SaveTime <= toDate).ToList();
            }
        }

        public List<Statistics15min> StatisticsMinDataLoad(List<long> ceqList, DateTime fromDate, DateTime toDate)
        {
            using (var context = DbContextConfigurationExtensions.CreateServerContext(_configuration))
            {
                return context.Statistics15mins.Where(p => ceqList.Any(x => x == p.Ceqid) && p.SaveTime >= fromDate && p.SaveTime <= toDate).ToList();
            }
        }

        public List<StatisticsHour> StatisticsHourDataLoad(List<long> ceqList, DateTime fromDate, DateTime toDate)
        {
            using (var context = DbContextConfigurationExtensions.CreateServerContext(_configuration))
            {
                return context.StatisticsHours.Where(p => ceqList.Any(x => x == p.Ceqid) && p.SaveTime >= fromDate && p.SaveTime <= toDate).ToList();
            }
        }

        public List<StatisticsDay> StatisticsDayDataLoad(List<long> ceqList, DateTime fromDate, DateTime toDate)
        {
            using (var context = DbContextConfigurationExtensions.CreateServerContext(_configuration))
            {
                return context.StatisticsDays.Where(p => ceqList.Any(x => x == p.Ceqid) && p.SaveTime >= fromDate && p.SaveTime <= toDate).ToList();
            }
        }
            
        public List<StatisticsMonth> StatisticsMonthDataLoad(List<long> ceqList, DateTime fromDate, DateTime toDate)
        {
            using (var context = DbContextConfigurationExtensions.CreateServerContext(_configuration))
            {
                return context.StatisticsMonths.Where(p => ceqList.Any(x => x == p.Ceqid) && p.SaveTime >= fromDate && p.SaveTime <= toDate).ToList();
            }
        }

        public List<StatisticsYear> StatisticsYearDataLoad(List<long> ceqList, DateTime fromDate, DateTime toDate)
        {
            using (var context = DbContextConfigurationExtensions.CreateServerContext(_configuration))
            {
                return context.StatisticsYears.Where(p => ceqList.Any(x => x == p.Ceqid) && p.SaveTime >= fromDate && p.SaveTime <= toDate).ToList();
            }
        }

        public List<HistoryCommState> CommStateDataLoad(List<long> ceqList, DateTime fromDate, DateTime toDate)
        {
            using (var context = DbContextConfigurationExtensions.CreateServerContext(_configuration))
            {
                return context.HistoryCommStates.Where(p => ceqList.Any(x => x == p.Ceqid) && p.SaveTime >= fromDate && p.SaveTime <= toDate).ToList();
            }
        }

        public List<HistoryCommStateLog> CommStateLogDataLoad(List<long> ceqList, DateTime fromDate, DateTime toDate)
        {
            using (var context = DbContextConfigurationExtensions.CreateServerContext(_configuration))
            {
                return context.HistoryCommStateLogs.Where(p => ceqList.Any(x => x == p.Ceqid) && p.SaveTime >= fromDate && p.SaveTime <= toDate).ToList();
            }
        }

        //public List<Datapointinfo> GetDatapointinfo()
        //{
        //    // AI 항목만 표시 하도록
        //    return _kdmsContext.Datapointinfos.Where(p => p.Pointtype == 3).ToList();
        //}

        public List<AiItem> GetAiInfo()
        {
            var data = _kdmsContext.AiInfos.Select(p => new AiItem
            {
                No = p.No,
                Columnname = p.Columnname,
                Datapointid = p.Datapointid,
                Datapointname = p.Datapointname,
                Datapointinfos = Datapointinfos
            }).ToList();

            return data; // _kdmsContext.AiInfos.ToList();

            //var datas = _kdmsContext.Datapointinfos.Where(p => p.Pointtype == (int)PointTypeCode.AI).Select(p => new AiInfo
            //{
            //    PointId = p.Datapointid ?? 0,
            //    PointName = p.Name ?? string.Empty,
            //    Alarmcategoryfk = p.Alarmcategoryfk,
            //    UseYn = false
            //}).ToList();

            //var retvalList = _kdmsContext.AiInfos.ToList();
            //if (retvalList.Count <= 0)
            //{
            //    _kdmsContext.AiInfos.AddRange(datas);
            //    _kdmsContext.SaveChanges();
            //    return datas;
            //}
            //else
            //{
            //    foreach(var data in  datas)
            //    {
            //        var find = retvalList.FirstOrDefault(p => p.PointId == data.PointId);
            //        if (find == null)
            //            retvalList.Add(data);
            //    }
            //}
            //return retvalList;
        }

        public async void SetAiInfo(List<AiItem> aiDatas)
        {
            foreach (var ai in aiDatas)
            {
                var find = _kdmsContext.AiInfos.FirstOrDefault(p => p.No == ai.No);
                if (find != null)
                {
                    find.Columnname = ai.Columnname;
                    find.Datapointid = ai.Datapointid;
                    find.Datapointname = ai.Datapointname;
                }
                else
                {
                    var data = new AiInfo { No = ai.No, Columnname = ai.Columnname, Datapointid = ai.Datapointid, Datapointname = ai.Datapointname };
                    _kdmsContext.AiInfos.Add(data);
                }
            }
            await _kdmsContext.SaveChangesAsync();
        }

        public List<BiInfo> GetBiInfo()
        {
            var datas = _kdmsContext.Datapointinfos.Where(p => p.Pointtype == (int)PointTypeCode.BI).Select(p => new BiInfo
            {
                PointId = p.Datapointid ?? 0,
                PointName = p.Name ?? string.Empty,
                Alarmcategoryfk = p.Alarmcategoryfk,
                UseYn = false
            }).ToList();

            var retvalList = _kdmsContext.BiInfos.ToList();
            if (retvalList.Count <= 0)
            {
                _kdmsContext.BiInfos.AddRange(datas);
                _kdmsContext.SaveChanges();
                return datas;
            }
            else
            {
                foreach (var data in datas)
                {
                    var find = retvalList.FirstOrDefault(p => p.PointId == data.PointId);
                    if (find == null)
                        retvalList.Add(data);
                }
            }
            return retvalList;
        }

        public async void SetBiInfo(List<BiInfo> biDatas)
        {
            foreach (var bi in biDatas)
            {
                var find = _kdmsContext.BiInfos.FirstOrDefault(p => p.PointId == bi.PointId);
                if (find != null)
                {
                    find.PointName = bi.PointName;
                    find.Alarmcategoryfk = bi.Alarmcategoryfk;
                    find.UseYn = bi.UseYn;
                }
                else
                {
                    _kdmsContext.BiInfos.Add(bi);
                }
            }
            await _kdmsContext.SaveChangesAsync();
        }

        public List<AlarmInfo> GetAlarmInfo()
        {
            var datas = _kdmsContext.Datapointinfos.Select(p => new AlarmInfo
            {
                PointId = p.Datapointid ?? 0,
                PointName = p.Name ?? string.Empty,
                Alarmcategoryfk = p.Alarmcategoryfk,
                UseYn = false
            }).ToList();

            var retvalList = _kdmsContext.AlarmInfos.ToList();
            if (retvalList.Count <= 0)
            {
                _kdmsContext.AlarmInfos.AddRange(datas);
                _kdmsContext.SaveChanges();
                return datas;
            }
            else
            {
                foreach (var data in datas)
                {
                    var find = retvalList.FirstOrDefault(p => p.PointId == data.PointId);
                    if (find == null)
                        retvalList.Add(data);
                }
            }
            return retvalList;
        }

        public async void SetAlarmInfo(List<AlarmInfo> alarmDatas)
        {
            foreach (var alarm in alarmDatas)
            {
                var find = _kdmsContext.AlarmInfos.FirstOrDefault(p => p.PointId == alarm.PointId);
                if (find != null)
                {
                    find.PointName = alarm.PointName;
                    find.Alarmcategoryfk = alarm.Alarmcategoryfk;
                    find.UseYn = alarm.UseYn;
                }
                else
                {
                    _kdmsContext.AlarmInfos.Add(alarm);
                }
            }
            await _kdmsContext.SaveChangesAsync();
        }

        public List<SchduleInfo> GetSchduleInfos()
        {
            return _kdmsContext.SchduleInfos.ToList();
        }

        public List<SchduleType> GetSchduleTypes()
        {
            return _kdmsContext.SchduleTypes.ToList();
        }

        public List<StorageInfo> GetStorageInfos()
        {
            return _kdmsContext.StorageInfos.ToList();
        }

        public async void SetSchduleInfo()
        {
            var model = App.Current.Services.GetService<OperationSchduleViewModel>()!;
            if (model != null)
            {
                foreach (var schdule in _kdmsContext.SchduleInfos)
                {
                    switch (schdule.SchduleId)
                    {
                        case (int)ProcDataCode.HISTORY_MIN_DATA:
                            {
                                schdule.SchduleType = model.HisMinInterval;
                                schdule.SchduleValue = GetDateTimeFormat(schdule.SchduleType, model.HisMinTime);
                                schdule.Desc = model.HisMinDesc;
                            }
                            break;
                        case (int)ProcDataCode.HISTORY_DAYSTAT_DATA:
                            {
                                schdule.SchduleType = model.HisStatInterval;
                                schdule.SchduleValue = GetDateTimeFormat(schdule.SchduleType, model.HisStatTime);
                                schdule.Desc = model.HisStatDesc;
                            }
                            break;
                        case (int)ProcDataCode.STATISTICS_15MIN:
                            {
                                schdule.SchduleType = model.StatMinInterval;
                                schdule.SchduleValue = GetDateTimeFormat(schdule.SchduleType, model.StatMinTime);
                                schdule.Desc = model.StatMinDesc;
                            }
                            break;
                        case (int)ProcDataCode.STATISTICS_HOUR:
                            {
                                schdule.SchduleType = model.StatHourInterval;
                                schdule.SchduleValue = GetDateTimeFormat(schdule.SchduleType, model.StatHourTime);
                                schdule.Desc = model.StatHourDesc;
                            }
                            break;
                        case (int)ProcDataCode.STATISTICS_DAY:
                            {
                                schdule.SchduleType = model.StatDayInterval;
                                schdule.SchduleValue = GetDateTimeFormat(schdule.SchduleType, model.StatDayTime);
                                schdule.Desc = model.StatDayDesc;
                            }
                            break;
                        case (int)ProcDataCode.STATISTICS_MONTH:
                            {
                                schdule.SchduleType = model.StatMonthInterval;
                                schdule.SchduleValue = GetDateTimeFormat(schdule.SchduleType, model.StatMonthTime);
                                schdule.Desc = model.StatMonthDesc;
                            }
                            break;
                        case (int)ProcDataCode.STATISTICS_YEAR:
                            {
                                schdule.SchduleType = model.StatYearInterval;
                                schdule.SchduleValue = GetDateTimeFormat(schdule.SchduleType, model.StatYearTime);
                                schdule.Desc = model.StatYearDesc;
                            }
                            break;
                        case (int)ProcDataCode.HISTORY_FI_ALARM:
                            {
                                
                            }
                            break;
                        case (int)ProcDataCode.HISTORY_COMM_STATE:
                            {
                                schdule.SchduleType = model.HisCommInterval;
                                schdule.SchduleValue = GetDateTimeFormat(schdule.SchduleType, model.HisCommTime);
                                schdule.Desc = model.HisCommDesc;
                            }
                            break;
                        case (int)ProcDataCode.HISTORY_COMM_STATE_LOG:
                            {
                                
                            }
                            break;
                    }
                }
                await _kdmsContext.SaveChangesAsync();
            }
        }

        private string GetDateTimeFormat(int schduleType, string time)
        {
            string retVaue = string.Empty;
            switch(schduleType)
            {
                case 1:
                    retVaue = $"yyyy-MM-dd HH:{time}";
                    break;
                case 2:
                    retVaue = $"yyyy-MM-dd HH:{time}";
                    break;
                case 3:
                    retVaue = $"yyyy-MM-dd {time}";
                    break;
                case 4:
                    retVaue = $"yyyy-MM-{time}";
                    break;
                case 5:
                    retVaue = $"yyyy-{time}";
                    break;
                case 6:
                    retVaue = time;
                    break;
                case 7:
                    retVaue = time;
                    break;
                case 8:
                    retVaue = $"yyyy-MM-dd {time}";
                    break;
            }
            return retVaue;
        }
        public async void SetStorageInfo()
        {
            var model = App.Current.Services.GetService<OperationStorageViewModel>()!;
            if (model != null)
            {
                foreach (var storage in _kdmsContext.StorageInfos)
                {
                    switch (storage.StorageId)
                    {
                        case (int)ProcDataCode.HISTORY_MIN_DATA:
                            storage.StorageValue = model.HisMinTime.ToString();
                            break;
                        case (int)ProcDataCode.HISTORY_DAYSTAT_DATA:
                            storage.StorageValue = model.HisStatTime.ToString();
                            break;
                        case (int)ProcDataCode.STATISTICS_15MIN:
                            storage.StorageValue = model.StatMinTime.ToString();
                            break;
                        case (int)ProcDataCode.STATISTICS_HOUR:
                            storage.StorageValue = model.StatHourTime.ToString();
                            break;
                        case (int)ProcDataCode.STATISTICS_DAY:
                            storage.StorageValue = model.StatDayTime.ToString();
                            break;
                        case (int)ProcDataCode.STATISTICS_MONTH:
                            storage.StorageValue = model.StatMonthTime.ToString();
                            break;
                        case (int)ProcDataCode.STATISTICS_YEAR:
                            storage.StorageValue = model.StatYearTime.ToString();
                            break;
                        case (int)ProcDataCode.HISTORY_FI_ALARM:
                            storage.StorageValue = model.HisFiTime.ToString();
                            break;
                        case (int)ProcDataCode.HISTORY_COMM_STATE:
                            storage.StorageValue = model.HisCommTime.ToString();
                            break;
                        case (int)ProcDataCode.HISTORY_COMM_STATE_LOG:
                            storage.StorageValue = model.HisCommLogTime.ToString();
                            break;
                    }
                }
            }

            await _kdmsContext.SaveChangesAsync();
        }

        //public void DayStatCalCreate(string date)
        //{
        //    string query = "drop EVENT IF exists min_data_calculation_event;";

        //    StringBuilder sb = new StringBuilder();
        //    sb.AppendLine("CREATE EVENT IF NOT EXISTS min_data_calculation_event");
        //    sb.AppendLine("ON SCHEDULE EVERY '1' day");
        //    sb.AppendLine($"STARTS concat(curdate(), ' {date}')");
        //    sb.AppendLine("COMMENT '매일 1회 지정된 시간에 실행하는 프로시저'");
        //    sb.AppendLine("DO");
        //    sb.AppendLine("call kdms.min_data_calculation(curdate());");

        //    using (MySqlMapper mapper = new MySqlMapper(_configuration))
        //    {
        //        bool retval = mapper.RunQuery(query);
        //        if (retval)
        //        {
        //            mapper.RunQuery(sb.ToString());
        //        }
        //    }
        //}
    }
}
