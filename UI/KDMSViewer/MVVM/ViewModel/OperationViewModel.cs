using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDMSViewer.ViewModel
{
    public partial class OperationViewModel : ObservableObject
    {
        [ObservableProperty]
        private bool _bICheck = true;

        [ObservableProperty]
        private bool _aICheck;

        [ObservableProperty]
        private bool _alarmCheck;

        [ObservableProperty]
        private bool _schduleCheck;

        [ObservableProperty]
        private bool _storagePeriodCheck;

        [ObservableProperty]
        private bool _dataBaseCheck;


        [ObservableProperty]
        private INotifyPropertyChanged _currentViewModel;

        public OperationViewModel()
        {
            CurrentViewModel = (INotifyPropertyChanged)App.Current.Services.GetService<OperationAiViewModel>()!;
        }

        [RelayCommand]
        private void BiInfo()
        {
            AICheck = false;
            AlarmCheck = false;
            SchduleCheck = false;
            StoragePeriodCheck = false;
            DataBaseCheck = false;

            CurrentViewModel = (INotifyPropertyChanged)App.Current.Services.GetService<OperationBiViewModel>()!;
        }

        [RelayCommand]
        private void AiInfo()
        {
            BICheck = false;
            AlarmCheck = false;
            SchduleCheck = false;
            StoragePeriodCheck = false;
            DataBaseCheck = false;

            CurrentViewModel = (INotifyPropertyChanged)App.Current.Services.GetService<OperationAiViewModel>()!;
        }

        [RelayCommand]
        private void AlarmInfo()
        {
            BICheck = false;
            AICheck = false;
            SchduleCheck = false;
            StoragePeriodCheck = false;
            DataBaseCheck = false;

            CurrentViewModel = (INotifyPropertyChanged)App.Current.Services.GetService<OperationAlarmViewModel>()!;
        }

        [RelayCommand]
        private void SchduleInfo()
        {
            BICheck = false;
            AICheck = false;
            AlarmCheck = false;
            StoragePeriodCheck = false;
            DataBaseCheck = false;

            CurrentViewModel = (INotifyPropertyChanged)App.Current.Services.GetService<OperationSchduleViewModel>()!;
        }

        [RelayCommand]
        private void StoragePeriodInfo()
        {
            BICheck = false;
            AICheck = false;
            AlarmCheck = false;
            SchduleCheck = false;
            DataBaseCheck = false;

            CurrentViewModel = (INotifyPropertyChanged)App.Current.Services.GetService<OperationStorageViewModel>()!;
        }

        [RelayCommand]
        private void DataBaseInfo()
        {
            BICheck = false;
            AICheck = false;
            AlarmCheck = false;
            SchduleCheck = false;
            StoragePeriodCheck = false;

            CurrentViewModel = (INotifyPropertyChanged)App.Current.Services.GetService<ConfigViewModel>()!;
        }
    }
}
