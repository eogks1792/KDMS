using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DevExpress.ClipboardSource.SpreadsheetML;
using DevExpress.Office.Utils;
using KDMS.EF.Core.Infrastructure.Reverse.Models;
using KDMSViewer.Extensions;
using KDMSViewer.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace KDMSViewer.ViewModel
{
    public partial class ViewModel_SwitchData : ObservableObject
    {
        private readonly DataWorker _worker;

        [ObservableProperty]
        private ObservableCollection<TabData> _tabItems;

        [ObservableProperty]
        private TabData _selectItem;

        [ObservableProperty]
        private int _totalPage;

        [ObservableProperty]
        private int _userPage;

        public ObservableCollection<HistoryMinDatum> PointItems { get; set; }

        public ViewModel_SwitchData(DataWorker worker)
        {
            _worker = worker;
        }

        //public void Init()
        //{
        //    TabItems = new ObservableCollection<TabData>();

        //    int count = 200;
        //    var pageTatal = (_worker.MinDatas.Count / count) + 1;
        //    for (int idx = 0; idx < pageTatal; idx++)
        //    {
        //        TabData item = new TabData();
        //        item.Header = idx + 1;

        //        var datas = _worker.MinDatas.Skip(idx * count).Take(count).ToList();
        //        item.PointItems = new ObservableCollection<HistoryMinDatum>(datas);
        //        DispatcherService.Invoke((System.Action)(() => { TabItems.Add(item); }));
        //    }
        //}

        [RelayCommand]
        private void FirstItem()
        {
            if(TabItems != null && TabItems.Count > 0)
                SelectItem = TabItems.FirstOrDefault()!;
        }

        [RelayCommand]
        private void LeftItem()
        {
            if (TabItems != null && TabItems.Count > 0)
            {
                var minHeader = TabItems.Min(p => p.Header);
                if(minHeader == SelectItem?.Header)
                {
                    MessageBox.Show($"맨 처음 페이지 입니다.", "1분 실시간 데이터", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
                SelectItem = TabItems.FirstOrDefault(p => p.Header == SelectItem.Header - 1)!;
            }
                
        }

        [RelayCommand]
        private void RightItem()
        {
            if (TabItems != null && TabItems.Count > 0)
            {
                var maxHeader = TabItems.Max(p => p.Header);
                if (maxHeader == SelectItem?.Header)
                {
                    MessageBox.Show($"맨 마지막 페이지 입니다.", "1분 실시간 데이터", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
                SelectItem = TabItems.FirstOrDefault(p => p.Header == SelectItem.Header + 1)!;
            }
                
        }

        [RelayCommand]
        private void LastItem()
        {
            if (TabItems != null && TabItems.Count > 0)
                SelectItem = TabItems.LastOrDefault()!;
        }

        [RelayCommand]
        private void HeaderInput()
        {
            if (TabItems != null && TabItems.Count > 0)
            {
                SelectItem = TabItems.FirstOrDefault(p => p.Header == UserPage)!;
            }    
        }
    }
}
