using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace KDMSViewer.ViewModel
{
    public partial class OperationViewModel : ObservableObject
    {
        //[ObservableProperty]
        //private bool _bICheck = true;

        [ObservableProperty]
        private bool _aICheck = true;

        [ObservableProperty]
        private bool _alarmCheck;

        [ObservableProperty]
        private bool _schduleCheck;

        [ObservableProperty]
        private bool _storagePeriodCheck;

        [ObservableProperty]
        private bool _dataBaseCheck;

        [ObservableProperty]
        private Visibility _aiInfoVisible;

        [ObservableProperty]
        private Visibility _etcVisible;

        [ObservableProperty]
        private INotifyPropertyChanged _aiInfoViewModel;

        [ObservableProperty]
        private INotifyPropertyChanged _etcViewModel;

        public OperationViewModel()
        {
            AiInfoViewModel = (INotifyPropertyChanged)App.Current.Services.GetService<OperationAiViewModel>()!;
        }

        //[RelayCommand]
        //private void BiInfo()
        //{
        //    AICheck = false;
        //    AlarmCheck = false;
        //    SchduleCheck = false;
        //    StoragePeriodCheck = false;
        //    DataBaseCheck = false;

        //    CurrentViewModel = (INotifyPropertyChanged)App.Current.Services.GetService<OperationBiViewModel>()!;
        //}

        [RelayCommand]
        private void AiInfo()
        {
            //BICheck = false;
            AlarmCheck = false;
            SchduleCheck = false;
            StoragePeriodCheck = false;
            DataBaseCheck = false;

            AiInfoVisible = Visibility.Visible;
            EtcVisible = Visibility.Hidden;
            AiInfoViewModel = (INotifyPropertyChanged)App.Current.Services.GetService<OperationAiViewModel>()!;
        }

        [RelayCommand]
        private void AlarmInfo()
        {
            //BICheck = false;
            AICheck = false;
            SchduleCheck = false;
            StoragePeriodCheck = false;
            DataBaseCheck = false;

            AiInfoVisible = Visibility.Hidden;
            EtcVisible = Visibility.Visible;
            EtcViewModel = (INotifyPropertyChanged)App.Current.Services.GetService<OperationAlarmViewModel>()!;
        }

        [RelayCommand]
        private void SchduleInfo()
        {
            //BICheck = false;
            AICheck = false;
            AlarmCheck = false;
            StoragePeriodCheck = false;
            DataBaseCheck = false;

            AiInfoVisible = Visibility.Hidden;
            EtcVisible = Visibility.Visible;
            EtcViewModel = (INotifyPropertyChanged)App.Current.Services.GetService<OperationSchduleViewModel>()!;
        }

        [RelayCommand]
        private void StoragePeriodInfo()
        {
            //BICheck = false;
            AICheck = false;
            AlarmCheck = false;
            SchduleCheck = false;
            DataBaseCheck = false;

            AiInfoVisible = Visibility.Hidden;
            EtcVisible = Visibility.Visible;
            EtcViewModel = (INotifyPropertyChanged)App.Current.Services.GetService<OperationStorageViewModel>()!;
        }

        [RelayCommand]
        private void DataBaseInfo()
        {
            //BICheck = false;
            AICheck = false;
            AlarmCheck = false;
            SchduleCheck = false;
            StoragePeriodCheck = false;

            AiInfoVisible = Visibility.Hidden;
            EtcVisible = Visibility.Visible;
            EtcViewModel = (INotifyPropertyChanged)App.Current.Services.GetService<ConfigViewModel>()!;
        }
    }
}
