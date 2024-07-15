using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KDMS.EF.Core.Infrastructure.Reverse;
using KDMS.EF.Core.Infrastructure.Reverse.Models;
using KDMSViewer.Model;
using Microsoft.Extensions.Configuration;
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
        private int _biTime = 1;

        [ObservableProperty]
        private int _boTime = 1;

        [ObservableProperty]
        private int _aiTime = 1;

        [ObservableProperty]
        private int _aoTime = 1;

        [ObservableProperty]
        private int _counterTime = 1;

        [ObservableProperty]
        public string _timeEditMask = CultureInfo.CurrentCulture.DateTimeFormat.LongTimePattern;

        [ObservableProperty]
        private DateTime _statisticalTime = Convert.ToDateTime(DateTime.Now.ToString("01:00:00"));

        //[ObservableProperty]
        //private int _deleteTime = 365;

        public int biTime { get; set; }
        public int boTime { get; set; }
        public int aiTime { get; set; }
        public int aoTime { get; set; }
        public int counterTime { get; set; }
        //public int deleteTime { get; set; }
        public DateTime statisticalTime { get; set; }
        public OperationSchduleViewModel(CommonDataModel commonData)
        {
            _commonData = commonData;

            var schduleList = _commonData.GetSchduleInfos();
            foreach(var schdule in schduleList)
            {
                switch(schdule.SchduleId)
                {
                    case (int)SchduleCode.BI:
                        BiTime = Convert.ToInt32(schdule.SchduleValue);
                        break;
                    case (int)SchduleCode.BO:
                        BoTime = Convert.ToInt32(schdule.SchduleValue);
                        break;
                    case (int)SchduleCode.AI:
                        AiTime = Convert.ToInt32(schdule.SchduleValue);
                        break;
                    case (int)SchduleCode.AO:
                        AoTime = Convert.ToInt32(schdule.SchduleValue);
                        break;
                    case (int)SchduleCode.COUNTER:
                        CounterTime = Convert.ToInt32(schdule.SchduleValue);
                        break;
                    case (int)SchduleCode.STATISTICS:
                        StatisticalTime = Convert.ToDateTime(schdule.SchduleValue);
                        break;
                }
            }

            biTime = BiTime;
            boTime = BoTime;
            aiTime = AiTime;
            aoTime = AoTime;
            counterTime = CounterTime;
            statisticalTime = StatisticalTime;
            //deleteTime = DeleteTime;
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
            biTime = BiTime;
            boTime = BoTime;
            aiTime = AiTime;
            aoTime = AoTime;
            counterTime = CounterTime;
            statisticalTime = StatisticalTime;
            //deleteTime = DeleteTime;

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

            //var bi = _kdmsContext.SchduleTables.FirstOrDefault(p => p.SchduleId == (int)SchduleCode.BI);
            //if (bi != null)
            //    bi.SchduleValue = BiTime.ToString();
            //var bo = _kdmsContext.SchduleTables.FirstOrDefault(p => p.SchduleId == (int)SchduleCode.BO);
            //if (bo != null)
            //    bo.SchduleValue = BoTime.ToString();
            //var ai = _kdmsContext.SchduleTables.FirstOrDefault(p => p.SchduleId == (int)SchduleCode.AI);
            //if (ai != null)
            //    ai.SchduleValue = AiTime.ToString();
            //var ao = _kdmsContext.SchduleTables.FirstOrDefault(p => p.SchduleId == (int)SchduleCode.AO);
            //if (ao != null)
            //    ao.SchduleValue = AoTime.ToString();
            //var cnt = _kdmsContext.SchduleTables.FirstOrDefault(p => p.SchduleId == (int)SchduleCode.COUNTER);
            //if (cnt != null)
            //    cnt.SchduleValue = CounterTime.ToString();
            //var statis = _kdmsContext.SchduleTables.FirstOrDefault(p => p.SchduleId == (int)SchduleCode.STATISTICS);
            //if (statis != null)
            //    statis.SchduleValue = Convert.ToDateTime(StatisticalTime).ToString("HH:mm:ss");

            //_commonData.DayStatCalCreate(statisticalTime.ToString("HH:mm:ss"));
            //await _kdmsContext.SaveChangesAsync();
        }
    }
}
