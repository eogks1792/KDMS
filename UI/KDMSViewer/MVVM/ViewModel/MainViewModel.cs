using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KDMSViewer.Model;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System.ComponentModel;
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

        [ObservableProperty]
        private string _mainEnabled = "1";

        [ObservableProperty]
        private string _operEnabled = "0";

        [ObservableProperty]
        private string _dbEnabled = "0";

        [ObservableProperty]
        private string _trandEnabled = "0";

        [ObservableProperty]
        private Visibility _dataViewVisible;

        [ObservableProperty]
        private Visibility _trandViewVisible;

        [ObservableProperty]
        private Visibility _operationViewVisible;

        [ObservableProperty]
        private INotifyPropertyChanged _dataCurrentViewModel;

        [ObservableProperty]
        private INotifyPropertyChanged _trandCurrentViewModel;

        [ObservableProperty]
        private INotifyPropertyChanged _operationCurrentViewModel;


        public MainViewModel(ILogger logger, DataWorker worker)
        {
            _logger = logger;
            _worker = worker;
            _worker.Init();
            DataCurrentViewModel = (INotifyPropertyChanged)App.Current.Services.GetService<DataViewModel>()!;
        }

        [RelayCommand]
        private void DataViewer()
        {
            MainEnabled = "1";
            OperEnabled = "0";
            DbEnabled = "0";
            TrandEnabled = "0";

            DataViewVisible = Visibility.Visible;
            TrandViewVisible = Visibility.Hidden;
            OperationViewVisible = Visibility.Hidden;

            DataCurrentViewModel = (INotifyPropertyChanged)App.Current.Services.GetService<DataViewModel>()!;
        }

        [RelayCommand]
        private void OperationConfig()
        {
            MainEnabled = "0";
            OperEnabled = "1";
            DbEnabled = "0";
            TrandEnabled = "0";

            DataViewVisible = Visibility.Hidden;
            TrandViewVisible = Visibility.Hidden;
            OperationViewVisible = Visibility.Visible;

            OperationCurrentViewModel = (INotifyPropertyChanged)App.Current.Services.GetService<OperationViewModel>()!;
        }

        [RelayCommand]
        private void TrandInfo()
        {
            MainEnabled = "0";
            OperEnabled = "0";
            DbEnabled = "0";
            TrandEnabled = "1";

            DataViewVisible = Visibility.Hidden;
            TrandViewVisible = Visibility.Visible;
            OperationViewVisible = Visibility.Hidden;

            TrandCurrentViewModel = (INotifyPropertyChanged)App.Current.Services.GetService<TrandViewModel>()!;
        }
    }
}
