using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KDMS.EF.Core.Contexts;
using KDMSServer.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace KDMSServer.ViewModel
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly ILogger _logger;
        private readonly DataWorker _worker;
        private readonly IConfiguration _configuration;

        [ObservableProperty]
        private string _ipAddress = "127.0.0.1";
        [ObservableProperty]
        private string _port = "3356";
        [ObservableProperty]
        private string _name = "kdms";
        [ObservableProperty]
        private string _userId = "kdms";
        [ObservableProperty]
        private string _userPwd = "";
        [ObservableProperty]
        private string _isDBConnetionText = "";
        [ObservableProperty]
        private string _isDBConnetionState = "";
        [ObservableProperty]
        private bool _isDBConnetion = false;

        [ObservableProperty]
        private string _loginId = "id";
        [ObservableProperty]
        private string _loginPwd = "password";
        [ObservableProperty]
        private string _scanPort = "12345";
        [ObservableProperty]
        private string _controlPort = "12345";
        [ObservableProperty]
        private string _eventPort = "12345";
        [ObservableProperty]
        private string _primeIP = "";
        [ObservableProperty]
        private string _backupIP = "";

        public string connectionString { get; set; }

        [ObservableProperty]
        private DateTime _manualtime;

        [ObservableProperty]
        private int _selectItem = 2;

        [ObservableProperty]
        private bool _isInput = true;

        [ObservableProperty]
        private Visibility _isVisibility = Visibility.Hidden;

        [ObservableProperty]
        public string _timeEditMask = CultureInfo.CurrentCulture.DateTimeFormat.LongTimePattern;

        [ObservableProperty]
        private ObservableCollection<LogInfo> _serverLogView = new ObservableCollection<LogInfo>();

        [ObservableProperty]
        private ObservableCollection<LogInfo> _dbLogView = new ObservableCollection<LogInfo>();

        public MainViewModel(IConfiguration configuration, ILogger logger, DataWorker worker)
        {
            _configuration = configuration;
            _logger = logger;
            _worker = worker;

            connectionString = _configuration.GetConnectionString("Server") ?? string.Empty;
            if (!string.IsNullOrEmpty(connectionString))
            {
                IpAddress = connectionString.Split(';')[0].Replace("Server=", "");
                Port = connectionString.Split(';')[1].Replace("Port=", "");
                Name = connectionString.Split(';')[2].Replace("Database=", "");
                UserId = connectionString.Split(';')[3].Replace("User=", "");
                UserPwd = connectionString.Split(';')[4].Replace("Password=", "");
            }

            var LogintInfoSection = _configuration.GetSection("LoginInfo");
            LoginId = LogintInfoSection.GetSection("LoginId").Value!;
            LoginPwd = LogintInfoSection.GetSection("LoginPwd").Value!;

            var ChannelInfoSection = _configuration.GetSection("ChannelInfo");
            ScanPort = ChannelInfoSection.GetSection("Scan").Value!;
            ControlPort = ChannelInfoSection.GetSection("Control").Value!;
            EventPort = ChannelInfoSection.GetSection("Event").Value!;

            var ServerIpInfoSection = _configuration.GetSection("ServerInfo");
            PrimeIP = ServerIpInfoSection.GetSection("PrimeServer").Value!;
            BackupIP = ServerIpInfoSection.GetSection("BackupServer").Value!;

            _worker.Init();
            Manualtime = DateTime.Now;
        }

        [RelayCommand]
        private void ConnnetionCheck()
        {
            var connectionString = $"Server={IpAddress};Port={Port};Database={Name};User={UserId};Password={UserPwd};";
            try
            {
                bool retValue = MySqlMapper.IsConnection(connectionString);
                if (retValue)
                    IsDBConnetionText = $"{Name} 데이터베이스 연결 성공"; 
                else
                    IsDBConnetionText = $"{Name} 데이터베이스 연결 실패";
            }
            catch (Exception ex)
            {
                IsDBConnetionText = $"{Name} 데이터베이스 연결 실패 ex:{ex.Message}"; 
            }
        }

        [RelayCommand]
        private void Save()
        {
            try
            {
                string filePath = AppDomain.CurrentDomain.BaseDirectory + "appsettings.json";
                connectionString = $"Server={IpAddress};Port={Port};Database={Name};User={UserId};Password={UserPwd};";
                JsonHelpers.AddOrUpdateAppSetting(filePath, "ConnectionStrings:Server", connectionString);

                var root = (IConfigurationRoot)_configuration;
                root.Reload();

                IsDBConnetionText = $"{Name} 데이터베이스 정보 저장 성공";
            }
            catch (Exception ex)
            {
                IsDBConnetionText = $"{Name} 데이터베이스 정보 저장 예외발생 ex:{ex.Message}";
            }
        }

        public void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectItem == (int)ProcTypeCode.STATISTICSHOUR)
                IsVisibility = Visibility.Visible;
            else
                IsVisibility = Visibility.Hidden;
        }

        [RelayCommand]
        private void ManualStatistics()
        {
            //_logger.Log($"{Manualtime.ToString("yyyy-MM-dd")} 통계 데이터 입력 시작");
            Task.Run(() =>
            {
                IsInput = false;
                _worker.GetProcData(SelectItem, Manualtime, true);
            });
        }
    }
}
