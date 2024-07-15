using DevExpress.Mvvm.Native;
using KDMS.EF.Core.Contexts;
using KDMS.EF.Core.Extensions;
using KDMS.EF.Core.Infrastructure.Reverse;
using KDMS.EF.Core.Infrastructure.Reverse.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System.Data;
using System.Text;
using System.Windows;

namespace KDMSServer.Model
{
    public class CommonDataModel
    {
        private readonly KdmsContext _kdmsContext;
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;

        public List<SchduleInfo> SchduleInfos { get; set; }
        public List<StorageInfo> StorageInfos { get; set; }

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
