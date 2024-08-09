using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DevExpress.Pdf.Native;
using DevExpress.Pdf.Native.BouncyCastle.Ocsp;
using KDMS.EF.Core.Contexts;
using KDMSServer.Model;
using KdmsTcpSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using Serilog;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Xml.Linq;

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
        private string _isSocketConnetionText = "";
        [ObservableProperty]
        private string _isSocketConnetionState = "";
        [ObservableProperty]
        private bool _isSocketConnetion = false;

        [ObservableProperty]
        private string _isDBConnetionText = "";
        [ObservableProperty]
        private string _isDBConnetionState = "";
        [ObservableProperty]
        private bool _isDBConnetion = false;

        [ObservableProperty]
        private bool _scanState = false;
        [ObservableProperty]
        private bool _controlState = false;
        [ObservableProperty]
        private bool _eventState = false;

        [ObservableProperty]
        private string _loginId = "id";
        [ObservableProperty]
        private string _loginPwd = "password";
        [ObservableProperty]
        private int _scanPort = 12345;
        [ObservableProperty]
        private int _controlPort = 12345;
        [ObservableProperty]
        private int _eventPort = 12345;
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
            ScanPort = Convert.ToInt32(ChannelInfoSection.GetSection("Scan").Value!);
            ControlPort = Convert.ToInt32(ChannelInfoSection.GetSection("Control").Value!);
            EventPort = Convert.ToInt32(ChannelInfoSection.GetSection("Alarm").Value!);

            var ServerIpInfoSection = _configuration.GetSection("ServerInfo");
            PrimeIP = ServerIpInfoSection.GetSection("PrimeServer").Value!;
            BackupIP = ServerIpInfoSection.GetSection("BackupServer").Value!;

            _worker.Init();
            Manualtime = DateTime.Now;
        }

        [RelayCommand]
        private async void SocketConnnetionCheck()
        {
            await Task.Run(() =>
            {
                bool firstCheck = false;
                try
                {
                    using (var client = new TcpClient(PrimeIP, Convert.ToInt32(ControlPort)))
                    firstCheck = true;

                    IsSocketConnetionText = $"{PrimeIP} KDMS 서버 연결 성공 (CONTORL Session)";
                }
                catch 
                {
                    if (!firstCheck)
                    {
                        try
                        {
                            using (var client = new TcpClient(BackupIP, Convert.ToInt32(ControlPort)))
                            IsSocketConnetionText = $"{BackupIP} KDMS 서버 연결 성공 (CONTORL Session)";
                        }
                        catch (Exception ex)
                        {
                            IsSocketConnetionText = $"{BackupIP} KDMS 서버 연결 실패 (CONTORL Session) ex:{ex.Message}";
                        }
                    }
                }
            });
        }

        [RelayCommand]
        private void SocketSave()
        {
            try
            {
                string filePath = AppDomain.CurrentDomain.BaseDirectory + "appsettings.json";

                JsonHelpers.AddOrUpdateAppSetting(filePath, "ServerInfo:PrimeServer", PrimeIP);
                JsonHelpers.AddOrUpdateAppSetting(filePath, "ServerInfo:BackupServer", BackupIP);

                JsonHelpers.AddOrUpdateAppSetting(filePath, "LoginInfo:LoginId", LoginId);
                JsonHelpers.AddOrUpdateAppSetting(filePath, "LoginInfo:LoginPwd", LoginPwd);

                JsonHelpers.AddOrUpdateAppSetting(filePath, "ChannelInfo:Scan", ScanPort);
                //JsonHelpers.AddOrUpdateAppSetting(filePath, "ChannelInfo:Control", ControlPort);
                JsonHelpers.AddOrUpdateAppSetting(filePath, "ChannelInfo:Alarm", EventPort);
                
                var root = (IConfigurationRoot)_configuration;
                root.Reload();

                IsSocketConnetionText = $"KDMS 서버 연결 정보 저장 성공";

                _worker.SocketClose();
            }
            catch (Exception ex)
            {
                IsDBConnetionText = $"KDMS 서버 연결 정보 저장 실패 예외발생 ex:{ex.Message}";
            }
        }

        [RelayCommand]
        private void DBConnnetionCheck()
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
        private void DBSave()
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
