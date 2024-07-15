using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KDMS.EF.Core.Contexts;
using KDMSViewer.Model;
using Microsoft.Extensions.Configuration;
using System.Windows;
using System.Windows.Controls;

namespace KDMSViewer.ViewModel
{
    public partial class ConfigViewModel : ObservableObject
    {
        private readonly IConfiguration _configuration;

        [ObservableProperty]
        private string _serverName = string.Empty;

        [ObservableProperty]
        private string _ipAddress = string.Empty;

        [ObservableProperty]
        private string _port = string.Empty;

        [ObservableProperty]
        private string _dBName = string.Empty;

        [ObservableProperty]
        private string _userName = string.Empty;

        [ObservableProperty]
        private string _userPassword = string.Empty;

        public string connectionString { get; set; }
        public string serverName { get; set; }

        public ConfigViewModel(IConfiguration configuration)
        {
            _configuration = configuration;

            ServerName = _configuration.GetSection("ServerName")?.Value ?? string.Empty;
            connectionString = _configuration.GetConnectionString("Server") ?? string.Empty;
            if (!string.IsNullOrEmpty(connectionString))
            {
                IpAddress = connectionString.Split(';')[0].Replace("Server=", "");
                Port = connectionString.Split(';')[1].Replace("Port=", "");
                DBName = connectionString.Split(';')[2].Replace("Database=", "");
                UserName = connectionString.Split(';')[3].Replace("User=", "");
                UserPassword = connectionString.Split(';')[4].Replace("Password=", "");
            }
        }

        [RelayCommand]
        private void ConnnetionCheck()
        {
            var connectionString = $"Server={IpAddress};Port={Port};Database={DBName};User={UserName};Password={UserPassword};";
            try
            {
                bool retValue = MySqlMapper.IsConnection(connectionString);
                if (retValue)
                    MessageBox.Show($"DB 연결 성공", "DB 연결 확인", MessageBoxButton.OK, MessageBoxImage.Information);
                else
                    MessageBox.Show($"DB 연결 실패", "DB 연결 확인", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"DB 연결 실패 ex:{ex.Message}", "DB 연결 확인", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            //string filePath = AppDomain.CurrentDomain.BaseDirectory + "appsettings.json";
            //JsonHelpers.AddOrUpdateAppSetting(filePath, "ConnectionStrings:Check", connectionString);
            //var root = (IConfigurationRoot)_configuration;
            //root.Reload();
            // connectionString 확인
            //var data = _configuration.GetConnectionString("Server");
            //using (var serverContext = DbContextConfigurationExtensions.CreateServerContext(_configuration))
            //{
            //}
        }

        [RelayCommand]
        private void Save()
        {
            try
            {
                string filePath = AppDomain.CurrentDomain.BaseDirectory + "appsettings.json";

                serverName = ServerName;
                JsonHelpers.AddOrUpdateAppSetting(filePath, "ServerName", serverName);
                connectionString = $"Server={IpAddress};Port={Port};Database={DBName};User={UserName};Password={UserPassword};";
                JsonHelpers.AddOrUpdateAppSetting(filePath, "ConnectionStrings:Server", connectionString);

                var root = (IConfigurationRoot)_configuration;
                root.Reload();

                MessageBox.Show($"연결 정보 저장 성공", "연결 정보 저장", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"연결 정보 저장중 예외발생 ex:{ex.Message}", "연결 정보 저장", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            var passwordBox = (PasswordBox)e.Source;
            if (passwordBox != null)
            {
                var textBox = passwordBox.Template.FindName("RevealedPassword", passwordBox) as TextBox;
                if (textBox != null)
                    textBox.Text = passwordBox.Password;
            }
        }


    }
}
