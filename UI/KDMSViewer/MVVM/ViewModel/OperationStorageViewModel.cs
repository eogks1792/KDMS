using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using System.Windows.Media.Animation;
using KDMSViewer.Model;

namespace KDMSViewer.ViewModel
{
    public partial class OperationStorageViewModel : ObservableObject
    {
        private readonly CommonDataModel _commonData;

        [ObservableProperty]
        public string _timeEditMask = CultureInfo.CurrentCulture.DateTimeFormat.LongTimePattern;

        [ObservableProperty]
        private int _hisMinTime = 30;
        [ObservableProperty]
        private int _hisStatTime = 365;
        [ObservableProperty]
        private int _statMinTime = 0;
        [ObservableProperty]
        private int _statHourTime = 0;
        [ObservableProperty]
        private int _statDayTime = 0;
        [ObservableProperty]
        private int _statMonthTime = 0;
        [ObservableProperty]
        private int _statYearTime = 0;
        [ObservableProperty]
        private int _hisFiTime = 0;
        [ObservableProperty]
        private int _hisCommTime = 0;
        [ObservableProperty]
        private int _hisCommLogTime = 0;

        //[ObservableProperty]
        //private bool _realCheck = false;

        //[ObservableProperty]
        //private bool _minuteCheck = false;

        //[ObservableProperty]
        //private bool _hourCheck = false;

        //[ObservableProperty]
        //private bool _dayCheck = false;

        //[ObservableProperty]
        //private bool _monthCheck = false;

        //[ObservableProperty]
        //private bool _yearCheck = true;

        //[ObservableProperty]
        //private DateTime _statisticalTime = Convert.ToDateTime(DateTime.Now.ToString("01:00:00"));

        //[ObservableProperty]
        //private int _deleteTime = 365;

        //public int hisMinTime { get; set; }
        //public int hisStatTime { get; set; }
        //public int statMinTime { get; set; }
        //public int statHourTime { get; set; }
        //public int statDayTime { get; set; }
        //public int statMonthTime { get; set; }
        //public int statYearTime { get; set; }
        //public int hisFiTime { get; set; }
        //public int hisCommTime { get; set; }
        //public int hisCommLogTime { get; set; }

        //public bool realCheck { get; set; } = false;
        //public bool minuteCheck { get; set; } = false;
        //public bool hourCheck { get; set; } = false;
        //public bool dayCheck { get; set; } = false;
        //public bool monthCheck { get; set; } = false;
        //public bool yearCheck { get; set; } = true;
        //public DateTime statisticalTime { get; set; }
        //public int deleteTime { get; set; }

        public OperationStorageViewModel(CommonDataModel commonData)
        {
            _commonData = commonData;

            var storageList = _commonData.GetStorageInfos();
            foreach (var storage in storageList)
            {
                switch (storage.StorageId)
                {
                    case (int)ProcDataCode.HISTORY_MIN_DATA:
                        HisMinTime = Convert.ToInt32(storage.StorageValue);
                        break;
                    case (int)ProcDataCode.HISTORY_DAYSTAT_DATA:
                        HisStatTime = Convert.ToInt32(storage.StorageValue);
                        break;
                    case (int)ProcDataCode.STATISTICS_15MIN:
                        StatMinTime = Convert.ToInt32(storage.StorageValue);
                        break;
                    case (int)ProcDataCode.STATISTICS_HOUR:
                        StatHourTime = Convert.ToInt32(storage.StorageValue);
                        break;
                    case (int)ProcDataCode.STATISTICS_DAY:
                        StatDayTime = Convert.ToInt32(storage.StorageValue);
                        break;
                    case (int)ProcDataCode.STATISTICS_MONTH:
                        StatMonthTime = Convert.ToInt32(storage.StorageValue);
                        break;
                    case (int)ProcDataCode.STATISTICS_YEAR:
                        StatYearTime = Convert.ToInt32(storage.StorageValue);
                        break;
                    case (int)ProcDataCode.HISTORY_FI_ALARM:
                        HisFiTime = Convert.ToInt32(storage.StorageValue);
                        break;
                    case (int)ProcDataCode.HISTORY_COMM_STATE:
                        HisCommTime = Convert.ToInt32(storage.StorageValue);
                        break;
                    case (int)ProcDataCode.HISTORY_COMM_STATE_LOG:
                        HisCommLogTime = Convert.ToInt32(storage.StorageValue);
                        break;
                }
            }

            //hisMinTime = HisMinTime;
            //hisStatTime = HisStatTime;
            //statMinTime = StatMinTime;
            //statHourTime = StatHourTime;
            //statDayTime = StatDayTime;
            //statMonthTime = StatMonthTime;
            //statYearTime = StatYearTime;
            //hisFiTime = HisFiTime;
            //hisCommTime = HisCommTime;
            //hisCommLogTime = HisCommLogTime;
            //realCheck = RealCheck;
            //minuteCheck = MinuteCheck;
            //hourCheck = HourCheck;
            //dayCheck = DayCheck;
            //monthCheck = MonthCheck;
            //statisticalTime = StatisticalTime;
            //deleteTime = DeleteTime;
        }

        [RelayCommand]
        private void Save()
        {
            //realCheck = RealCheck;
            //minuteCheck = MinuteCheck;
            //hourCheck = HourCheck;
            //dayCheck = DayCheck;
            //monthCheck = MonthCheck;
            //hisMinTime = HisMinTime;
            //hisStatTime = HisStatTime;
            //statMinTime = StatMinTime;
            //statHourTime = StatHourTime;
            //statDayTime = StatDayTime;
            //statMonthTime = StatMonthTime;
            //statYearTime = StatYearTime;
            //hisFiTime = HisFiTime;
            //hisCommTime = HisCommTime;
            //hisCommLogTime = HisCommLogTime;
            //statisticalTime = StatisticalTime;
            //deleteTime = DeleteTime;

            try
            {
                // 데이터베이스 업데이트 처리
                _commonData.SetStorageInfo();
                MessageBox.Show($"보관기간 설정 성공", "보관기간 설정", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"보관 기간 설정 실패 예외발생 ex:{ex.Message}", "보관기간 설정", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            
        }

        //public string GetStorage()
        //{
        //    string retValue = string.Empty;

        //    if (realCheck)
        //        retValue = "실시간";
        //    else if (minuteCheck)
        //        retValue = "15분단위";
        //    else if (hourCheck)
        //        retValue = "시단위";
        //    else if (dayCheck)
        //        retValue = "일단위";
        //    else if (monthCheck)
        //        retValue = "월단위";
        //    else if (yearCheck)
        //        retValue = "년단위";

        //    return retValue;
        //}
        public void OnTextInput(object sender, TextCompositionEventArgs e)
        {
            var regex = new Regex("[^0-9]+"); // 숫자만 입력
            e.Handled = regex.IsMatch(e.Text);
            if (e.Handled)
            {
                if (Keyboard.IsKeyDown(Key.Enter))
                    return;

                MessageBox.Show(string.Format("숫자만 입력이 가능합니다."), "숫자 입력", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
        }
    }
}
