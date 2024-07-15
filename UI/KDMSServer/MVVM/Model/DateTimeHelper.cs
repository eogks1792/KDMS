using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDMSServer.MVVM.Model
{
    public enum TimeDivisionCode
    {
        None = 0,
        Minute,     // 분별
        Hour,       // 분별 + 시별
        Day,        // 분별 + 시별 + 일별
        Week,
        Month,      // 분별 + 시별 + 일별 + 월별
        Year,
    }

    class DateTimeHelper
    {
        public static DateTime GetNextDateTime(DateTime now, TimeDivisionCode timeCode, int month = 1, int day = 1, int hour = 0, int min = 0, int sec = 0, int weekday = 0)

        {
            DateTime date = DateTime.Now;
            switch (timeCode)
            {
                case TimeDivisionCode.None:

                    break;
                case TimeDivisionCode.Minute:
                    {
                        if (min <= 0)
                            min = 1;

                        int addMin = min - (now.Minute % min);
                        DateTime mindate = now.AddMinutes(addMin);
                        date = new DateTime(mindate.Year, mindate.Month, mindate.Day, mindate.Hour, mindate.Minute, sec);
                    }
                    break;
                case TimeDivisionCode.Hour:
                    {
                        date = new DateTime(now.Year, now.Month, now.Day, now.Hour, min, sec);
                        if (now > date)
                            date = date.AddHours(1);
                    }
                    break;
                case TimeDivisionCode.Day:
                    {
                        date = new DateTime(now.Year, now.Month, now.Day, hour, min, sec);
                        if (now > date)
                            date = date.AddDays(1);
                    }
                    break;
                case TimeDivisionCode.Week:
                    {
                        date = new DateTime(now.Year, now.Month, now.Day, hour, min, sec);
                        if (now > date)
                            date = date.AddDays(weekday + 7 - Convert.ToInt32(date.DayOfWeek));
                    }
                    break;
                case TimeDivisionCode.Month:
                    {
                        date = new DateTime(now.Year, now.Month, day, hour, min, sec);
                        if (now > date)
                            date = date.AddMonths(1);
                    }
                    break;
                case TimeDivisionCode.Year:
                    {
                        date = new DateTime(now.Year + 1, month, day, hour, min, sec);
                        if (now > date)
                            date = date.AddMonths(1);
                    }
                    break;
                default:
                    break;
            }

            return date;
        }
    }
}
