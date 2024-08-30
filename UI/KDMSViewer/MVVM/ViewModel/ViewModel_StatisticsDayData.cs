using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KDMS.EF.Core.Infrastructure.Reverse.Models;
using KDMSViewer.Model;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
namespace KDMSViewer.ViewModel
{
    public partial class ViewModel_StatisticsDayData : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<TabData> _tabItems;

        [ObservableProperty]
        private TabData _selectItem;

        [ObservableProperty]
        private int _totalPage;

        [ObservableProperty]
        private int _totalCount;

        [ObservableProperty]
        private int _userPage;

        [ObservableProperty]
        private ObservableCollection<StatisticsDay> _pointItems;

        public ViewModel_StatisticsDayData()
        {
        }

        [RelayCommand]
        private void FirstItem()
        {
            if (TabItems != null && TabItems.Count > 0)
                SelectItem = TabItems.FirstOrDefault()!;
        }

        [RelayCommand]
        private void LeftItem()
        {
            if (TabItems != null && TabItems.Count > 0)
            {
                var minHeader = TabItems.Min(p => p.Header);
                if (minHeader == SelectItem?.Header)
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

        public void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TabControl tab = sender as TabControl;
            if (tab != null)
            {
                TabItem item = (TabItem)(tab.ItemContainerGenerator.ContainerFromItem(tab.SelectedItem));
                if (item != null)
                    item.Focus();
            }
        }
    }
}
