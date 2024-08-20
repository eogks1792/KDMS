using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDMSViewer.Model
{
    public enum ViewTypeCode
    {
        None = 0,
        DataView,
        TrandView
    }

    public enum SearchTypeCode
    {
        MINDATA = 1,
        DAYSTATDATA,
        STATISTICSMIN,
        STATISTICSHOUR,
        STATISTICSDAY,
        STATISTICSMONTH,
        STATISTICSYEAR,
        FIALARM,
        COMMSTATE,
        COMMSTATELOG,
    }

    public enum TreeTypeCode
    {
        SUBS = 1,
        MTR,
        DL,
        COMPOSITE,
        EQUIPMENT,
    }

    public enum PointTypeCode
    {
        BI = 1,
        BO,
        AI,
        AO,
        COUNTER,
    }

    public enum ProcDataCode
    {
        HISTORY_MIN_DATA = 1,
        HISTORY_DAYSTAT_DATA,
        STATISTICS_15MIN,
        STATISTICS_HOUR,
        STATISTICS_DAY,
        STATISTICS_MONTH,
        STATISTICS_YEAR,
        HISTORY_FI_ALARM,
        HISTORY_COMM_STATE,
        HISTORY_COMM_STATE_LOG,
    }


}
