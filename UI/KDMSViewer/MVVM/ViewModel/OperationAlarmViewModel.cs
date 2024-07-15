using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DevExpress.Xpf.Grid.Hierarchy;
using DevExpress.Mvvm.Native;
using KDMS.EF.Core.Infrastructure.Reverse.Models;
using KDMSViewer.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace KDMSViewer.ViewModel
{
    public partial class OperationAlarmViewModel : ObservableObject
    {
        private readonly CommonDataModel _commonData;

        [ObservableProperty]
        private ObservableCollection<PointDataModel> _alarmItems;

        public int DataCount { get; set; } = 0;
        
        [ObservableProperty]
        private List<PointUseynModel> _pointUseyns;

        public OperationAlarmViewModel(CommonDataModel commonData)
        {
            _commonData = commonData;

            PointUseyns = new List<PointUseynModel>()
            {
                new PointUseynModel { IsUseyn = true, Name = "사용" },
                new PointUseynModel { IsUseyn = false, Name = "미사용" },
            };

            var alarmDatas = _commonData.GetAlarmInfo();
            if (alarmDatas.Count > 0)
            {
                var data = alarmDatas.Select(p => new PointDataModel
                {
                    PointId = p.PointId,
                    PointName = p.PointName,
                    Alarmcategoryfk = p.Alarmcategoryfk,
                    UseYn = p.UseYn,
                    PointUseyns = PointUseyns
                }).ToList();
                AlarmItems = new ObservableCollection<PointDataModel>(data);
                DataCount = AlarmItems.Where(p => p.UseYn).Count();
            }
            
        }

        [RelayCommand]
        private void AllCheck()
        {
            AlarmItems.ForEach(p => p.UseYn = true);
        }

        [RelayCommand]
        private void AllUnCheck()
        {
            AlarmItems.ForEach(p => p.UseYn = false);
        }

        [RelayCommand]
        private void Save()
        {
            try
            {
                DataCount = AlarmItems.Where(p => p.UseYn).Count();
                var data = AlarmItems.Select(p => new AlarmInfo
                {
                    PointId = p.PointId,
                    PointName = p.PointName,
                    Alarmcategoryfk = p.Alarmcategoryfk,
                    UseYn = p.UseYn
                }).ToList();

                _commonData.SetAlarmInfo(data);
                MessageBox.Show($"알람 목록 사용항목 CNT:{DataCount} 설정 성공", "알람 목록 설정", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"알람 목록 설정 예외발생 ex:{ex.Message}", "알람 목록 설정", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
