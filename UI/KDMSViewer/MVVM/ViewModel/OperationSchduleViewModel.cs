using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KDMS.EF.Core.Infrastructure.Reverse;
using KDMS.EF.Core.Infrastructure.Reverse.Models;
using KDMSViewer.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client.Extensions.Msal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace KDMSViewer.ViewModel
{
    public partial class OperationSchduleViewModel : ObservableObject
    {
        private readonly CommonDataModel _commonData;

        [ObservableProperty]
        private List<SchduleType> _schduleTypes;

        [ObservableProperty]
        private string _hisMinTime = string.Empty;
        [ObservableProperty]
        private int _hisMinInterval = 0;
        [ObservableProperty]
        private string _hisMinDesc = string.Empty;

        [ObservableProperty]
        private string _hisStatTime = string.Empty;
        [ObservableProperty]
        private int _hisStatInterval = 0;
        [ObservableProperty]
        private string _hisStatDesc = string.Empty;

        [ObservableProperty]
        private string _statMinTime = string.Empty;
        [ObservableProperty]
        private int _statMinInterval = 0;
        [ObservableProperty]
        private string _statMinDesc = string.Empty;

        [ObservableProperty]
        private string _statHourTime = string.Empty;
        [ObservableProperty]
        private int _statHourInterval = 0;
        [ObservableProperty]
        private string _statHourDesc = string.Empty;

        [ObservableProperty]
        private string _statDayTime = string.Empty;
        [ObservableProperty]
        private int _statDayInterval = 0;
        [ObservableProperty]
        private string _statDayDesc = string.Empty;

        [ObservableProperty]
        private string _statMonthTime = string.Empty;
        [ObservableProperty]
        private int _statMonthInterval = 0;
        [ObservableProperty]
        private string _statMonthDesc = string.Empty;

        [ObservableProperty]
        private string _statYearTime = string.Empty;
        [ObservableProperty]
        private int _statYearInterval = 0;
        [ObservableProperty]
        private string _statYearDesc = string.Empty;

        [ObservableProperty]
        private string _hisFiTime = string.Empty;
        [ObservableProperty]
        private int _hisFiInterval = 0;
        [ObservableProperty]
        private string _hisFiDesc = string.Empty;

        [ObservableProperty]
        private string _hisCommTime = string.Empty;
        [ObservableProperty]
        private int _hisCommInterval = 0;
        [ObservableProperty]
        private string _hisCommDesc = string.Empty;

        [ObservableProperty]
        private string _hisCommLogTime = string.Empty;
        [ObservableProperty]
        private int _hisCommLogInterval = 0;
        [ObservableProperty]
        private string _hisCommLogDesc = string.Empty;


        public DateTime statisticalTime { get; set; }
        public OperationSchduleViewModel(CommonDataModel commonData)
        {
            _commonData = commonData;

            SchduleTypes = _commonData.GetSchduleTypes();
            var schduleInfos = _commonData.GetSchduleInfos();
            foreach(var schdule in schduleInfos)
            {
                switch (schdule.SchduleId)
                {
                    case (int)ProcDataCode.HISTORY_MIN_DATA:
                        {
                            HisMinInterval = schdule.SchduleType;
                            HisMinTime = Convert.ToDateTime(DateTime.Now.ToString(schdule.SchduleValue)).ToString("mm:ss");
                            HisMinDesc = schdule.Desc;
                        }
                        break;
                    case (int)ProcDataCode.HISTORY_DAYSTAT_DATA:
                        {
                            HisStatInterval = schdule.SchduleType;
                            HisStatTime = Convert.ToDateTime(DateTime.Now.ToString(schdule.SchduleValue)).ToString("HH:mm:ss");
                            HisStatDesc = schdule.Desc;
                        }
                        break;
                    case (int)ProcDataCode.STATISTICS_15MIN:
                        {
                            StatMinInterval = schdule.SchduleType;
                            StatMinTime = schdule.SchduleValue; 
                            StatMinDesc = schdule.Desc;
                        }
                        break;
                    case (int)ProcDataCode.STATISTICS_HOUR:
                        {
                            StatHourInterval = schdule.SchduleType;
                            StatHourTime = Convert.ToDateTime(DateTime.Now.ToString(schdule.SchduleValue)).ToString("mm:ss");
                            StatHourDesc = schdule.Desc;
                        }
                        break;
                    case (int)ProcDataCode.STATISTICS_DAY:
                        {
                            StatDayInterval = schdule.SchduleType;
                            StatDayTime = Convert.ToDateTime(DateTime.Now.ToString(schdule.SchduleValue)).ToString("HH:mm:ss");
                            StatDayDesc = schdule.Desc;
                        }
                        break;
                    case (int)ProcDataCode.STATISTICS_MONTH:
                        {
                            StatMonthInterval = schdule.SchduleType;
                            StatMonthTime = Convert.ToDateTime(DateTime.Now.ToString(schdule.SchduleValue)).ToString("dd HH:mm:ss");
                            StatMonthDesc = schdule.Desc;
                        }
                        break;
                    case (int)ProcDataCode.STATISTICS_YEAR:
                        {
                            StatYearInterval = schdule.SchduleType;
                            StatYearTime = Convert.ToDateTime(DateTime.Now.ToString(schdule.SchduleValue)).ToString("MM-dd HH:mm:ss");
                            StatYearDesc = schdule.Desc;
                        }
                        break;
                    case (int)ProcDataCode.HISTORY_FI_ALARM:
                        {
                            HisFiInterval = schdule.SchduleType;
                            HisFiTime = schdule.SchduleValue; 
                            HisFiDesc = schdule.Desc;
                        }
                        break;
                    case (int)ProcDataCode.HISTORY_COMM_STATE:
                        {
                            HisCommInterval = schdule.SchduleType;
                            HisCommTime = Convert.ToDateTime(DateTime.Now.ToString(schdule.SchduleValue)).ToString("HH:mm:ss");
                            HisCommDesc = schdule.Desc;

                        }
                        break;
                    case (int)ProcDataCode.HISTORY_COMM_STATE_LOG:
                        {
                            HisCommLogInterval = schdule.SchduleType;
                            HisCommLogTime = schdule.SchduleValue; 
                            HisCommLogDesc = schdule.Desc;
                        }
                        break;
                }
            }
        }

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

        [RelayCommand]
        private void Save()
        {
            try
            {
                // 데이터베이스 업데이트 처리
                _commonData.SetSchduleInfo();
                MessageBox.Show($"스케줄 정보 저장 성공", "스케줄 저장", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"스케줄 정보 실패 예외발생 ex:{ex.Message}", "스케줄 저장", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
