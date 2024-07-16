using KDMS.EF.Core.Contexts;
using KDMS.EF.Core.Infrastructure.Reverse.Models;
using KDMSServer.Features;
using KDMSServer.MVVM.Model;
using KDMSServer.ViewModel;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Interop;

namespace KDMSServer.Model
{
    public class DataWorker
    {
        private readonly ILogger _logger;
        private readonly IMediator _mediator;
        private readonly IConfiguration _configuration;
        private readonly CommonDataModel _commonData;


        private bool ThreadFlag { get; set; } = true;
        public DataWorker(ILogger logger, IMediator mediator, IConfiguration configuration, CommonDataModel commonData)
        {
            _logger = logger;
            _mediator = mediator;
            _configuration = configuration;
            _commonData = commonData;
        }

        public void Init()
        {
            Task.Run(() =>
            {
                DataBaseStatue();
            });

            Task.Run(() =>
            {
                TableCreateWorker();
            });

            Task.Run(() =>
            {
                RealTimeDataWorker();
            });

            Task.Run(() =>
            {
                StatisticsDataWorker();
            });
        }

        public async void GetProcData(int type, DateTime time, bool manual = false)
        {
            switch (type)
            {
                case (int)ProcTypeCode.MINDATA:
                    {

                    }
                    break;
                case (int)ProcTypeCode.DAYSTATDATA:
                    {
                        _logger.DbLog($"[1일 통계(1분활용)] {time.ToString("yyyy-MM-dd")} 데이터 입력 시작{(manual ? " (수동)" : " (자동)")}");

                        DaystatDataCalculation.Command request = new DaystatDataCalculation.Command
                        {
                            Time = time,
                        };

                        var response = await _mediator.Send(request);
                        if (response != null && response.Result)
                        {
                            _logger.DbLog($"[1일 통계(1분활용)] {time.ToString("yyyy-MM-dd")} 데이터 입력 성공{(manual ? " (수동)" : " (자동)")}");
                        }
                        else
                        {
                            _logger.DbLog($"[1일 통계(1분활용)] {time.ToString("yyyy-MM-dd")} 데이터 입력 실패 {(manual ? " (수동)" : " (자동)")} CODE:{response.Error.Code} MSG:{response.Error.Message}");
                        }
                    }
                    break;
                case (int)ProcTypeCode.STATISTICSMIN:
                    {
                        
                    }
                    break;
                case (int)ProcTypeCode.STATISTICSHOUR:
                    {
                        _logger.DbLog($"[시간 통계(15분활용)] {time.ToString("yyyy-MM-dd HH:00:00")} 데이터 입력 시작{(manual ? " (수동)" : " (자동)")}");

                        StatisticsHourCalculation.Command request = new StatisticsHourCalculation.Command
                        {
                            Time = time,
                        };

                        var response = await _mediator.Send(request);
                        if (response != null && response.Result)
                        {
                            _logger.DbLog($"[시간 통계(15분활용)] {time.ToString("yyyy-MM-dd HH:00:00")} 데이터 입력 성공{(manual ? " (수동)" : " (자동)")}");
                        }
                        else
                        {
                            _logger.DbLog($"[시간 통계(15분활용)] {time.ToString("yyyy-MM-dd HH:00:00")} 데이터 입력 실패{(manual ? " (수동)" : " (자동)")} CODE:{response.Error.Code} MSG:{response.Error.Message}");
                        }
                    }
                    break;
                case (int)ProcTypeCode.STATISTICSDAY:
                    {
                        _logger.DbLog($"[일 통계(시활용)] {time.ToString("yyyy-MM-dd")} 데이터 입력 시작{(manual ? " (수동)" : " (자동)")}");

                        StatisticsDayCalculation.Command request = new StatisticsDayCalculation.Command
                        {
                            Time = time,
                        };

                        var response = await _mediator.Send(request);
                        if (response != null && response.Result)
                        {
                            _logger.DbLog($"[일 통계(시활용)] {time.ToString("yyyy-MM-dd")} 데이터 입력 성공{(manual ? " (수동)" : " (자동)")}");
                        }
                        else
                        {
                            _logger.DbLog($"[일 통계(시활용)] {time.ToString("yyyy-MM-dd")} 데이터 입력 실패{(manual ? " (수동)" : " (자동)")} CODE:{response.Error.Code} MSG:{response.Error.Message}");
                        }
                    }
                    break;
                case (int)ProcTypeCode.STATISTICSMONTH:
                    {
                        _logger.DbLog($"[월 통계(일활용)] {time.ToString("yyyy-MM")} 데이터 입력 시작{(manual ? " (수동)" : " (자동)")}");

                        StatisticsMonthCalculation.Command request = new StatisticsMonthCalculation.Command
                        {
                            Time = time,
                        };

                        var response = await _mediator.Send(request);
                        if (response != null && response.Result)
                        {
                            _logger.DbLog($"[월 통계(일활용)] {time.ToString("yyyy-MM")} 데이터 입력 성공{(manual ? " (수동)" : " (자동)")}");
                        }
                        else
                        {
                            _logger.DbLog($"[월 통계(일활용)] {time.ToString("yyyy-MM")} 데이터 입력 실패{(manual ? " (수동)" : " (자동)")} CODE:{response.Error.Code} MSG:{response.Error.Message}");
                        }
                    }
                    break;
                case (int)ProcTypeCode.STATISTICSYEAR:
                    {
                        _logger.DbLog($"[년 통계(월활용)] {time.ToString("yyyy")} 데이터 입력 시작{(manual ? " (수동)" : " (자동)")}");

                        StatisticsYearCalculation.Command request = new StatisticsYearCalculation.Command
                        {
                            Time = time,
                        };

                        var response = await _mediator.Send(request);
                        if (response != null && response.Result)
                        {
                            _logger.DbLog($"[년 통계(월활용)] {time.ToString("yyyy")} 데이터 입력 성공{(manual ? " (수동)" : " (자동)")}");
                        }
                        else
                        {
                            _logger.DbLog($"[년 통계(월활용)] {time.ToString("yyyy")} 데이터 입력 실패{(manual ? " (수동)" : " (자동)")} CODE:{response.Error.Code} MSG:{response.Error.Message}");
                        }
                    }
                    break;
                case (int)ProcTypeCode.FIALARM:
                    {
                       
                    }
                    break;
                case (int)ProcTypeCode.COMMSTATE:
                    {
                      
                    }
                    break;
                case (int)ProcTypeCode.COMMSTATELOG:
                    {
                        
                    }
                    break;
            }

            var model = App.Current.Services.GetService<MainViewModel>()!;
            if (model != null)
                model.IsInput = true;
        }

        public void ThreadClose()
        {
            ThreadFlag = false;
        }

        private void RealTimeDataWorker()
        {
            Thread.Sleep(1500);

            var value = Convert.ToDateTime(_commonData.SchduleInfos.FirstOrDefault(p => p.SchduleId == (int)SchduleCode.STATISTICS)?.SchduleValue);
            DateTime dayInitialTime = DateTimeHelper.GetNextDateTime(DateTime.Now, TimeDivisionCode.Day, hour: value.Hour, min: value.Minute, sec: value.Second);

            _logger.DbLog($"[1일 통계(1분활용)] 데이터 생성 시간: {dayInitialTime.ToString("yyyy-MM-dd HH:mm:ss")}");
            while (ThreadFlag)
            {
                try
                {
                    if (dayInitialTime <= DateTime.Now)
                    {
                        var model = App.Current.Services.GetService<MainViewModel>()!;
                        if (model != null)
                        {
                            if (model.IsDBConnetion)
                                GetProcData((int)ProcTypeCode.DAYSTATDATA, dayInitialTime.AddDays(-1));
                            else
                                _logger.DbLog($"{dayInitialTime.AddDays(-1).ToString("yyyy-MM-dd HH:mm:ss")} [1일 통계(1분활용)] 데이터 생성 실패 (DB 접속 실패) ");
                        }
                        dayInitialTime = dayInitialTime.AddDays(1);
                        _logger.DbLog($"NEXT [1일 통계(1분활용)] 데이터 생성 시간: {dayInitialTime.ToString("yyyy-MM-dd HH:mm:ss")}");
                    }
                }
                catch
                {

                }
                Thread.Sleep(1000);
            }
        }

        private void StatisticsDataWorker()
        {
            Thread.Sleep(2500);

            DateTime hourInitialTime = DateTimeHelper.GetNextDateTime(DateTime.Now, TimeDivisionCode.Hour, min:5);
            _logger.DbLog($"[시간 통계(15분활용)] 데이터 생성 시간: {hourInitialTime.ToString("yyyy-MM-dd HH:mm:ss")}");

            DateTime dayInitialTime = DateTimeHelper.GetNextDateTime(DateTime.Now, TimeDivisionCode.Day, hour:1);
            _logger.DbLog($"[일 통계(시간활용)] 데이터 생성 시간: {dayInitialTime.ToString("yyyy-MM-dd HH:mm:ss")}");

            DateTime monthInitialTime = DateTimeHelper.GetNextDateTime(DateTime.Now, TimeDivisionCode.Month, hour: 1);
            _logger.DbLog($"[월 통계(일활용)] 데이터 생성 시간: {monthInitialTime.ToString("yyyy-MM-dd HH:mm:ss")}");
            
            DateTime yearhInitialTime = DateTimeHelper.GetNextDateTime(DateTime.Now, TimeDivisionCode.Year, hour: 1);
            _logger.DbLog($"[년 통계(월활용)] 데이터 생성 시간: {yearhInitialTime.ToString("yyyy-MM-dd HH:mm:ss")}");

            while (ThreadFlag)
            {
                try
                {
                    if (hourInitialTime <= DateTime.Now)
                    {
                        var model = App.Current.Services.GetService<MainViewModel>()!;
                        if (model != null)
                        {
                            if (model.IsDBConnetion)
                                GetProcData((int)ProcTypeCode.STATISTICSHOUR, hourInitialTime.AddHours(-1));
                            else
                                _logger.DbLog($"{hourInitialTime.AddHours(-1).ToString("yyyy-MM-dd HH:00:00")} [시간 통계(15분활용)] 데이터 생성 실패 (DB 접속 실패) ");
                        }
                        hourInitialTime = hourInitialTime.AddDays(1);
                        _logger.DbLog($"NEXT [시간 통계(15분활용)] 데이터 생성 시간: {hourInitialTime.ToString("yyyy-MM-dd HH:mm:ss")}");
                    }

                    if (dayInitialTime <= DateTime.Now)
                    {
                        var model = App.Current.Services.GetService<MainViewModel>()!;
                        if (model != null)
                        {
                            if (model.IsDBConnetion)
                                GetProcData((int)ProcTypeCode.STATISTICSDAY, dayInitialTime.AddDays(-1));
                            else
                                _logger.DbLog($"{dayInitialTime.AddDays(-1).ToString("yyyy-MM-dd")} [일 통계(시간활용)] 데이터 생성 실패 (DB 접속 실패) ");
                        }
                        dayInitialTime = dayInitialTime.AddDays(1);
                        _logger.DbLog($"NEXT [일 통계(시간활용)] 데이터 생성 시간: {dayInitialTime.ToString("yyyy-MM-dd HH:mm:ss")}");
                    }

                    if (monthInitialTime <= DateTime.Now)
                    {
                        var model = App.Current.Services.GetService<MainViewModel>()!;
                        if (model != null)
                        {
                            if (model.IsDBConnetion)
                                GetProcData((int)ProcTypeCode.STATISTICSMONTH, monthInitialTime.AddMonths(-1));
                            else
                                _logger.DbLog($"{monthInitialTime.AddMonths(-1).ToString("yyyy-MM")} [월 통계(일활용)] 데이터 생성 실패 (DB 접속 실패) ");
                        }
                        monthInitialTime = monthInitialTime.AddDays(1);
                        _logger.DbLog($"NEXT [월 통계(일활용)] 데이터 생성 시간: {monthInitialTime.ToString("yyyy-MM-dd HH:mm:ss")}");
                    }

                    if (yearhInitialTime <= DateTime.Now)
                    {
                        var model = App.Current.Services.GetService<MainViewModel>()!;
                        if (model != null)
                        {
                            if (model.IsDBConnetion)
                                GetProcData((int)ProcTypeCode.STATISTICSYEAR, yearhInitialTime.AddYears(-1));
                            else
                                _logger.DbLog($"{yearhInitialTime.AddYears(-1).ToString("yyyy")} [년 통계(월활용)] 데이터 생성 실패 (DB 접속 실패) ");
                        }
                        yearhInitialTime = yearhInitialTime.AddDays(1);
                        _logger.DbLog($"NEXT [년 통계(월활용)] 데이터 생성 시간: {yearhInitialTime.ToString("yyyy-MM-dd HH:mm:ss")}");
                    }
                }
                catch
                {

                }
                Thread.Sleep(1000);
            }
        }

        private void TableCreateWorker()
        {
            Thread.Sleep(1000);

            DateTime yearInitialTime = DateTimeHelper.GetNextDateTime(DateTime.Now.AddYears(-1), TimeDivisionCode.Year, month:12, day:31, hour:1);
            _logger.DbLog($"[1일 통계(1분활용)] 테이블 생성 시간: {yearInitialTime.ToString("yyyy-MM-dd HH:mm:ss")}");
            DateTime dayInitialTime = DateTimeHelper.GetNextDateTime(DateTime.Now, TimeDivisionCode.Day, hour: 23, min:55);
            _logger.DbLog($"[1분 실시간] 테이블 생성 시간: {dayInitialTime.ToString("yyyy-MM-dd HH:mm:ss")}");

            while (ThreadFlag)
            {
                try
                {
                    if (dayInitialTime <= DateTime.Now)
                    {
                        var model = App.Current.Services.GetService<MainViewModel>()!;
                        if (model != null)
                        {
                            if(model.IsDBConnetion)
                            {
                                bool retval = _commonData.MinDataTableCreate(dayInitialTime);
                                if (retval)
                                {
                                    _logger.DbLog($"{dayInitialTime.AddDays(1).ToString("yyyyMMdd")} [1분 실시간] 테이블 생성 성공");
                                    _logger.DbLog($"{dayInitialTime.AddDays(2).ToString("yyyyMMdd")} [1분 실시간] 테이블 생성 성공");
                                }
                            }
                            else
                            {
                                _logger.DbLog($"{dayInitialTime.AddDays(1).ToString("yyyyMMdd")} [1분 실시간] 테이블 생성 실패 (DB 접속 실패) ");
                                _logger.DbLog($"{dayInitialTime.AddDays(2).ToString("yyyyMMdd")} [1분 실시간] 테이블 생성 실패 (DB 접속 실패) ");
                            }
                        }
                        dayInitialTime = dayInitialTime.AddDays(1);
                        _logger.DbLog($"NEXT [1분 실시간] 테이블 생성 시간: {dayInitialTime.ToString("yyyy-MM-dd HH:mm:ss")}");
                    }

                    if (yearInitialTime <= DateTime.Now)
                    {
                        var model = App.Current.Services.GetService<MainViewModel>()!;
                        if (model != null)
                        {
                            if (model.IsDBConnetion)
                            {
                                bool retval = _commonData.DayStatTableCreate(yearInitialTime);
                                if (retval)
                                {
                                    _logger.DbLog($"{yearInitialTime.AddYears(1).ToString("yyyy")} [1일 통계(1분활용)] 테이블 생성 성공");
                                }
                            }
                            else
                                _logger.DbLog($"{yearInitialTime.AddYears(1).ToString("yyyy")} [1일 통계(1분활용)] 테이블 생성 실패 (DB 접속 실패) ");
                        }
                        yearInitialTime = yearInitialTime.AddYears(1);
                        _logger.DbLog($"NEXT [1일 통계(1분활용)] 테이블 생성 시간: {yearInitialTime.ToString("yyyy-MM-dd HH:mm:ss")}");
                    }
                }
                catch
                {

                }
                Thread.Sleep(1000);
            }
        }

        private async void DataBaseStatue()
        {
            int checkCnt = 0;
            string checkText = string.Empty;
            while (ThreadFlag)
            {
                try
                {
                    var model = App.Current.Services.GetService<MainViewModel>()!;
                    if (model != null)
                    {
                        // DB 상태
                        try
                        {
                            if (!string.IsNullOrEmpty(model.IsDBConnetionText))
                                checkText = model.IsDBConnetionText;

                            if (checkText == model.IsDBConnetionText)
                                checkCnt++;

                            if (checkCnt == 11)
                            {
                                checkCnt = 0;
                                checkText = string.Empty;
                                model.IsDBConnetionText = string.Empty;
                            }

                            bool retValue = new MySqlMapper(_configuration).IsConnection();
                            if (retValue)
                                model.IsDBConnetionState = "성공";
                            else
                                model.IsDBConnetionState = "실패";

                            model.IsDBConnetion = retValue;
                        }
                        catch (Exception ex) 
                        {
                            model.IsDBConnetion = false;
                            model.IsDBConnetionState = "실패";
                        }
                    }
                    await Task.Delay(1000);
                }
                catch
                {

                }
            }
        }
    }
}
