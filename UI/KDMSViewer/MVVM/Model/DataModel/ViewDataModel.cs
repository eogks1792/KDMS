using KDMS.EF.Core.Infrastructure.Reverse.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDMSViewer.Model
{
    ///////////////////////// 실시간

    public class HistoryMinData : HistoryMinDatum
    {
        public int No { get; set; }
    }

    public class Statistics15minData : Statistics15min
    {
        public int No { get; set; }
    }

    public class HistoryFiAlarmData : HistoryFiAlarm
    {
        public int No { get; set; }
    }

    ///////////////////////// 통계
    public class HistoryDaystatData : HistoryDaystatDatum
    {
        public int No { get; set; }
    }

    public class StatisticsHourData : StatisticsHour
    {
        public int No { get; set; }
    }

    public class StatisticsDayData : StatisticsDay
    {
        public int No { get; set; }
    }

    public class StatisticsMonthData : StatisticsMonth
    {
        public int No { get; set; }
    }

    public class StatisticsYearData : StatisticsYear
    {
        public int No { get; set; }
    }


    ///////////////////////// 통계

    public class HistoryCommStateData : HistoryCommState
    {
        public int No { get; set; }
    }

    public class HistoryCommStateLogData : HistoryCommStateLog
    {
        public int No { get; set; }
    }



}
