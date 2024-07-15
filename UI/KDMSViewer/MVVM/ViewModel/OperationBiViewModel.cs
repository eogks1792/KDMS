using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Grid.Hierarchy;
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
    public partial class OperationBiViewModel : ObservableObject
    {
        private readonly CommonDataModel _commonData;

        [ObservableProperty]
        private ObservableCollection<PointDataModel> _biItems;

        public int DataCount { get; set; } = 0;

        [ObservableProperty]
        private List<PointUseynModel> _pointUseyns;

        public OperationBiViewModel(CommonDataModel commonData)
        {
            _commonData = commonData;

            PointUseyns = new List<PointUseynModel>()
            {
                new PointUseynModel { IsUseyn = true, Name = "사용" },
                new PointUseynModel { IsUseyn = false, Name = "미사용" },
            };

            var biDatas = _commonData.GetBiInfo();
            if (biDatas.Count > 0)
            {
                var data = biDatas.Select(p => new PointDataModel
                {
                    PointId = p.PointId,
                    PointName = p.PointName,
                    Alarmcategoryfk = p.Alarmcategoryfk,
                    UseYn = p.UseYn,
                    PointUseyns = PointUseyns
                }).ToList();
                BiItems = new ObservableCollection<PointDataModel>(data);
                DataCount = BiItems.Where(p => p.UseYn).Count();
            }
        }

        [RelayCommand]
        private void AllCheck()
        {
            BiItems.ForEach(p => p.UseYn = true);
        }

        [RelayCommand]
        private void AllUnCheck()
        {
            BiItems.ForEach(p => p.UseYn = false);
        }

        [RelayCommand]
        private void Save()
        {
            try
            {
                DataCount = BiItems.Where(p => p.UseYn).Count();
                var data = BiItems.Select(p => new BiInfo
                {
                    PointId = p.PointId,
                    PointName = p.PointName,
                    Alarmcategoryfk = p.Alarmcategoryfk,
                    UseYn = p.UseYn
                }).ToList();

                _commonData.SetBiInfo(data);
                MessageBox.Show($"BI 목록 사용항목 CNT:{DataCount} 설정 성공", "BI 목록 설정", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"BI 목록 설정 예외발생 ex:{ex.Message}", "BI 목록 설정", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
    }
}
