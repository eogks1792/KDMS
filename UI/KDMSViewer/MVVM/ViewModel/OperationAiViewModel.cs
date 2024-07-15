using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DevExpress.ClipboardSource.SpreadsheetML;
using DevExpress.Mvvm.Native;
using KDMS.EF.Core.Infrastructure.Reverse.Models;
using KDMSViewer.Model;
using System.Collections.ObjectModel;
using System.Windows;

namespace KDMSViewer.ViewModel
{
    public partial class OperationAiViewModel : ObservableObject
    {
        private readonly CommonDataModel _commonData;

        [ObservableProperty]
        private ObservableCollection<AiInfo> _aiItems;

        public int DataCount { get; set; } = 0;

        //[ObservableProperty]
        //private List<PointUseynModel> _pointUseyns;

        public OperationAiViewModel(CommonDataModel commonData)
        {
            _commonData = commonData;

            //PointUseyns = new List<PointUseynModel>()
            //{
            //    new PointUseynModel { IsUseyn = true, Name = "사용" },
            //    new PointUseynModel { IsUseyn = false, Name = "미사용" },
            //};

            AiItems = new ObservableCollection<AiInfo>(_commonData.GetAiInfo());
            //var aiDatas = _commonData.GetAiInfo();
            //if (aiDatas.Count > 0)
            //{
            //    var data = aiDatas.Select(p => new PointDataModel
            //    {
            //        PointId = p.PointId,
            //        PointName = p.PointName,
            //        Alarmcategoryfk = p.Alarmcategoryfk,
            //        UseYn = p.UseYn,
            //        PointUseyns = PointUseyns
            //    }).ToList();
            //    AiItems = new ObservableCollection<PointDataModel>(data);
            //    DataCount = AiItems.Where(p => p.UseYn).Count();
            //}
        }

        //[RelayCommand]
        //private void AllCheck()
        //{
        //    AiItems.ForEach(p => p.UseYn = true);
        //}

        //[RelayCommand]
        //private void AllUnCheck()
        //{
        //    AiItems.ForEach(p => p.UseYn = false);
        //}

        [RelayCommand]
        private void Save()
        {
            try
            {
                DataCount = AiItems.Count();
                //var data = AiItems.Select(p => new AiInfo
                //{
                //    PointId = p.PointId,
                //    PointName = p.PointName,
                //    Alarmcategoryfk = p.Alarmcategoryfk,
                //    UseYn = p.UseYn
                //}).ToList();

                _commonData.SetAiInfo(AiItems.ToList());
                MessageBox.Show($"AI 목록 사용항목 CNT:{DataCount} 설정 성공", "AI 목록 설정", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"AI 목록 설정 예외발생 ex:{ex.Message}", "AI 목록 설정", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
