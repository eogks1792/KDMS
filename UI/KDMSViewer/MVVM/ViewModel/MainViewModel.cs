using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KDMSViewer.Model;
using KDMSViewer.View;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace KDMSViewer.ViewModel
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly ILogger _logger;
        private readonly DataWorker _worker;

        [ObservableProperty]
        private string _serverState = string.Empty;

        [ObservableProperty]
        private string _dBState = string.Empty;

        //[ObservableProperty]
        //private int _biCount = 0;

        //[ObservableProperty]
        //private int _aiCount = 0;

        //[ObservableProperty]
        //private int _alarmCount = 0;

        //[ObservableProperty]
        //private string _scheduleMeasureBIBO = string.Empty;

        //[ObservableProperty]
        //private string _scheduleMeasureBO = string.Empty;

        //[ObservableProperty]
        //private string _scheduleMeasureAIAO = string.Empty;

        ////[ObservableProperty]
        ////private string _scheduleMeasureAO = string.Empty;

        //[ObservableProperty]
        //private string _scheduleMeasureCounter = string.Empty;

        //[ObservableProperty]
        //private string _scheduleDelete = string.Empty;

        //[ObservableProperty]
        //private string _storagePeriodData = string.Empty;

        //[ObservableProperty]
        //private string _sPRealMinData = string.Empty;
        //[ObservableProperty]
        //private string _sPDayStatData = string.Empty;
        //[ObservableProperty]
        //private string _sPStatMinData = string.Empty;
        //[ObservableProperty]
        //private string _sPStatHourData = string.Empty;
        //[ObservableProperty]
        //private string _sPStatDayData = string.Empty;
        //[ObservableProperty]
        //private string _sPStatMonthData = string.Empty;
        //[ObservableProperty]
        //private string _sPStatYearData = string.Empty;
        //[ObservableProperty]
        //private string _sPFiData = string.Empty;
        //[ObservableProperty]
        //private string _sPCommData = string.Empty;
        //[ObservableProperty]
        //private string _sPCommLogData = string.Empty;

        //[ObservableProperty]
        //private string _storagePeriodStatistics = string.Empty;

        [ObservableProperty]
        private string _mainEnabled = "1";

        [ObservableProperty]
        private string _operEnabled = "0";

        [ObservableProperty]
        private string _dbEnabled = "0";

        [ObservableProperty]
        private string _trandEnabled = "0";

        [ObservableProperty]
        private INotifyPropertyChanged _currentViewModel;


        public MainViewModel(ILogger logger, DataWorker worker) 
        {
            _logger = logger;
            _worker = worker;
            _worker.Init();
            CurrentViewModel = (INotifyPropertyChanged)App.Current.Services.GetService<DataViewModel>()!;
        }

        [RelayCommand]
        private void DataViewer()
        {
            MainEnabled = "1";
            OperEnabled = "0";
            DbEnabled = "0";
            TrandEnabled = "0";

            CurrentViewModel = (INotifyPropertyChanged)App.Current.Services.GetService<DataViewModel>()!;
        }

        [RelayCommand]
        private void OperationConfig()
        {
            MainEnabled = "0";
            OperEnabled = "1";
            DbEnabled = "0";
            TrandEnabled = "0";

            CurrentViewModel = (INotifyPropertyChanged)App.Current.Services.GetService<OperationViewModel>()!;
        }

        //[RelayCommand]
        //private void DatabaseConfig()
        //{
        //    MainEnabled = "0";
        //    OperEnabled = "0";
        //    DbEnabled = "1";
        //    TrandEnabled = "0";

        //    CurrentViewModel = (INotifyPropertyChanged)App.Current.Services.GetService<DBConfigViewModel>()!;
        //}

        [RelayCommand]
        private void TrandInfo()
        {
            MainEnabled = "0";
            OperEnabled = "0";
            DbEnabled = "0";
            TrandEnabled = "1";

            CurrentViewModel = (INotifyPropertyChanged)App.Current.Services.GetService<TrandViewModel>()!;
        }

        //[RelayCommand]
        //private void FileSave()
        //{
        //    var viewModel = (INotifyPropertyChanged)App.Current.Services.GetService<DataViewModel>()!;
        //    if (CurrentViewModel != viewModel)
        //        return;


        //}
    }
}
