﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDMSServer.Model
{
    public enum ProcTypeCode
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

    public enum SchduleCode
    {
        BI = 1,
        BO,
        AI,
        AO,
        COUNTER,
        STATISTICS
    }

    public enum StorageCode
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
