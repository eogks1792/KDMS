using KDMS.EF.Core.Contexts;
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
        //private List<PdbDistributionlinesegment> distributionLinesegments;

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
                ConductingEquipments = _kdmsContext.PdbConductingequipments.ToList();
                //distributionLinesegments = _kdmsContext.PdbDistributionlinesegments.ToList();
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
                subs.Name = substation.Name ?? string.Empty;
                subs.IsVisible = Visibility.Collapsed;

                foreach (var distributionline in Distributionlines.Where(p => p.StFk == substation.Stid))
                {
                    TreeDataModel dl = new TreeDataModel();
                    dl.Type = TreeTypeCode.DL;
                    dl.Id = distributionline.Dlid;
                    //dl.SubsId = subs.Id;
                    //dl.SubsName = subs.Name;
                    dl.Name = distributionline.Name ?? string.Empty;
                    dl.IsVisible = Visibility.Collapsed;
                    subs.DataModels.Add(dl);


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

                    var multiSwList = ConductingEquipments.Where(p => p.DlFk == distributionline.Dlid && p.EcFk != 0);
                    if (multiSwList.Count() > 0)
                    {
                        var compositList = multiSwList.Select(p => p.EcFk).ToList();
                        foreach (var compositeswitch in Compositeswitchs.Where(p => compositList.Any(x => x == p.Pid)))
                        {
                            TreeDataModel composit = new TreeDataModel();
                            composit.Type = TreeTypeCode.COMPOSITE;
                            composit.Id = compositeswitch.Pid;
                            composit.Name = compositeswitch.Name!.Trim() ?? string.Empty;
                            composit.IsVisible = Visibility.Collapsed;
                            dl.DataModels.Add(composit);

                            foreach (var conductingEquipment in ConductingEquipments.Where(p => p.EcFk == compositeswitch.Pid))
                            {
                                TreeDataModel equipment = new TreeDataModel();
                                equipment.Type = TreeTypeCode.EQUIPMENT;
                                equipment.Id = conductingEquipment.Ceqid;
                                equipment.Name = conductingEquipment.Name!.Trim() ?? string.Empty;
                                composit.DataModels.Add(equipment);
                            }
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
            for (DateTime st = fromDate; st < toDate; st = st.AddDays(1))
            {
                sb.AppendLine($"select * from history_min_data_{st.ToString("yyyyMMdd")} where ceqid in ({lst}) and save_time >= '{st.ToString("yyyy-MM-dd 00:00:00")}' and save_time <= '{st.ToString("yyyy-MM-dd 23:59:59")}'");

                if (st.Day != toDate.Day)
                    sb.AppendLine("union all");
            }

            //string qeury = $"select * from history_min_data_{DateTime.Now.ToString("yyyyMM")}01 where save_time >= '{fromDate.ToString("yyyy-MM-dd HH:mm:ss")}' and save_time <= '{toDate.ToString("yyyy-MM-dd HH:mm:ss")}'";
            //DataTable dt = null;
            using (MySqlMapper mapper = new MySqlMapper(_configuration))
            {
                retList = (List<HistoryMinDatum>)mapper.ExecuteQuery<HistoryMinDatum>(sb.ToString());
                //dt = mapper.SqlQuery<HistoryMinDatum>(sb.ToString());
                //if (dt != null)
                //    retList = dt.ConvertDataTable<HistoryMinDatum>();
            }
            return retList;
        }

        public List<HistoryDaystatDatum> DayStatDataLoad(List<long> ceqList, DateTime fromDate, DateTime toDate)
        {
            var lst = string.Join(", ", ceqList.ToArray());
            List<HistoryDaystatDatum> retList = new List<HistoryDaystatDatum>();

            StringBuilder sb = new StringBuilder();
            if(fromDate.Year == toDate.Year)
            {
                sb.AppendLine($"select * from history_daystat_data_{fromDate.ToString("yyyy")} where ceqid in ({lst}) and save_time >= '{fromDate.ToString("yyyy-MM-dd 00:00:00")}' and save_time <= '{toDate.ToString("yyyy-MM-dd 23:59:59")}'");
            }
            else
            {
                for (DateTime st = fromDate; st < toDate; st = st.AddYears(1))
                {
                    if(st.Year == fromDate.Year)
                        sb.AppendLine($"select * from history_daystat_data_{st.ToString("yyyy")} where ceqid in ({lst}) and save_time >= '{st.ToString("yyyy-MM-dd 00:00:00")}'");
                    else if (st.Year == toDate.Year)
                        sb.AppendLine($"select * from history_daystat_data_{st.ToString("yyyy")} where ceqid in ({lst}) and save_time <= '{toDate.ToString("yyyy-MM-dd 23:00:00")}'");
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
            var lst = string.Join(", ", ceqList.ToArray());
            return _kdmsContext.HistoryFiAlarms.Where(p => lst.Any(x => x == p.Ceqid) && p.SaveTime >= fromDate && p.SaveTime <= toDate).ToList();
        }

        public List<Statistics15min> StatisticsMinDataLoad(List<long> ceqList, DateTime fromDate, DateTime toDate)
        {
            var lst = string.Join(", ", ceqList.ToArray());
            return _kdmsContext.Statistics15mins.Where(p => lst.Any(x => x == p.Ceqid) && p.SaveTime >= fromDate && p.SaveTime <= toDate).ToList();
        }

        public List<StatisticsHour> StatisticsHourDataLoad(List<long> ceqList, DateTime fromDate, DateTime toDate)
        {
            var lst = string.Join(", ", ceqList.ToArray());
            return _kdmsContext.StatisticsHours.Where(p => lst.Any(x => x == p.Ceqid) && p.SaveTime >= fromDate && p.SaveTime <= toDate).ToList();
        }

        public List<StatisticsDay> StatisticsDayDataLoad(List<long> ceqList, DateTime fromDate, DateTime toDate)
        {
            var lst = string.Join(", ", ceqList.ToArray());
            return _kdmsContext.StatisticsDays.Where(p => lst.Any(x => x == p.Ceqid) && p.SaveTime >= fromDate && p.SaveTime <= toDate).ToList();
        }

        public List<StatisticsMonth> StatisticsMonthDataLoad(List<long> ceqList, DateTime fromDate, DateTime toDate)
        {
            var lst = string.Join(", ", ceqList.ToArray());
            return _kdmsContext.StatisticsMonths.Where(p => lst.Any(x => x == p.Ceqid) && p.SaveTime >= fromDate && p.SaveTime <= toDate).ToList();
        }

        public List<StatisticsYear> StatisticsYearDataLoad(List<long> ceqList, DateTime fromDate, DateTime toDate)
        {
            var lst = string.Join(", ", ceqList.ToArray());
            return _kdmsContext.StatisticsYears.Where(p => lst.Any(x => x == p.Ceqid) && p.SaveTime >= fromDate && p.SaveTime <= toDate).ToList();
        }

        public List<HistoryCommState> CommStateDataLoad(List<long> ceqList, DateTime fromDate, DateTime toDate)
        {
            var lst = string.Join(", ", ceqList.ToArray());
            return _kdmsContext.HistoryCommStates.Where(p => p.SaveTime >= fromDate && p.SaveTime <= toDate).ToList();
        }

        public List<HistoryCommStateLog> CommStateLogDataLoad(List<long> ceqList, DateTime fromDate, DateTime toDate)
        {
            var lst = string.Join(", ", ceqList.ToArray());
            return _kdmsContext.HistoryCommStateLogs.Where(p => p.SaveTime >= fromDate && p.SaveTime <= toDate).ToList();
        }

        public List<AiInfo> GetAiInfo()
        {
            return _kdmsContext.AiInfos.ToList();

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

        public async void SetAiInfo(List<AiInfo> aiDatas)
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
                    _kdmsContext.AiInfos.Add(ai);
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
                        case (int)SchduleCode.BI:
                            schdule.SchduleValue = model.BiTime.ToString();
                            break;
                        case (int)SchduleCode.BO:
                            schdule.SchduleValue = model.BoTime.ToString();
                            break;
                        case (int)SchduleCode.AI:
                            schdule.SchduleValue = model.AiTime.ToString();
                            break;
                        case (int)SchduleCode.AO:
                            schdule.SchduleValue = model.AoTime.ToString();
                            break;
                        case (int)SchduleCode.COUNTER:
                            schdule.SchduleValue = model.CounterTime.ToString();
                            break;
                        case (int)SchduleCode.STATISTICS:
                            schdule.SchduleValue = model.StatisticalTime.ToString("HH:mm:ss");
                            break;
                    }
                }
                DayStatCalCreate(model.StatisticalTime.ToString("HH:mm:ss"));
            }
            
            await _kdmsContext.SaveChangesAsync();
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
                        case (int)StorageCode.HISTORY_MIN_DATA:
                            storage.StorageValue = model.HisMinTime.ToString(); 
                            break;
                        case (int)StorageCode.HISTORY_DAYSTAT_DATA:
                            storage.StorageValue = model.HisStatTime.ToString();
                            break;
                        case (int)StorageCode.STATISTICS_15MIN:
                            storage.StorageValue = model.StatMinTime.ToString();
                            break;
                        case (int)StorageCode.STATISTICS_HOUR:
                            storage.StorageValue = model.StatHourTime.ToString();
                            break;
                        case (int)StorageCode.STATISTICS_DAY:
                            storage.StorageValue = model.StatDayTime.ToString();
                            break;
                        case (int)StorageCode.STATISTICS_MONTH:
                            storage.StorageValue = model.StatMonthTime.ToString();
                            break;
                        case (int)StorageCode.STATISTICS_YEAR:
                            storage.StorageValue = model.StatYearTime.ToString();
                            break;
                        case (int)StorageCode.HISTORY_FI_ALARM:
                            storage.StorageValue = model.HisFiTime.ToString();
                            break;
                        case (int)StorageCode.HISTORY_COMM_STATE:
                            storage.StorageValue = model.HisCommTime.ToString();
                            break;
                        case (int)StorageCode.HISTORY_COMM_STATE_LOG:
                            storage.StorageValue = model.HisCommLogTime.ToString();
                            break;
                    }
                }
            }

            await _kdmsContext.SaveChangesAsync();
        }

        public void DayStatCalCreate(string date)
        {
            string query = "drop EVENT IF exists min_data_calculation_event;";

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("CREATE EVENT IF NOT EXISTS min_data_calculation_event");
            sb.AppendLine("ON SCHEDULE EVERY '1' day");
            sb.AppendLine($"STARTS concat(curdate(), ' {date}')");
            sb.AppendLine("COMMENT '매일 1회 지정된 시간에 실행하는 프로시저'");
            sb.AppendLine("DO");
            sb.AppendLine("call kdms.min_data_calculation(curdate());");

            using (MySqlMapper mapper = new MySqlMapper(_configuration))
            {
                bool retval = mapper.RunQuery(query);
                if (retval)
                {
                    mapper.RunQuery(sb.ToString());
                }
            }
        }
    }
}
