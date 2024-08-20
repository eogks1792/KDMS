using DevExpress.Mvvm;
using DevExpress.Mvvm.UI;
using DevExpress.Xpf.Core;
using KDMS.EF.Core.Extensions;
using KDMSViewer.Extensions;
using KDMSViewer.Model;
using KDMSViewer.View;
using KDMSViewer.ViewModel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System.Diagnostics;
using System.Windows;

namespace KDMSViewer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private Mutex mutex = null;
        public new static App Current => (App)Application.Current;
        public IServiceProvider Services { get; }

        public App()
        {
            Services = ConfigureServices();
        }

        private static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();
            // Config
            //var commonConfigFile = Environment.GetEnvironmentVariable("K_H2EMS_CONFIG") ?? @"C:\KDN\k_h2ems_config.json";
            var configurationBuilder = new ConfigurationBuilder()
                        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                        .AddJsonFile($"appsettings.Development.json", optional: true, reloadOnChange: true)
                        //.AddJsonFile(commonConfigFile, optional: true, reloadOnChange: true)
                        .Build();
            services.AddSingleton<IConfiguration>(config => configurationBuilder);

            Environment.SetEnvironmentVariable("BASEDIR", AppDomain.CurrentDomain.BaseDirectory);
            Log.Logger = new LoggerConfiguration()
                         .ReadFrom.Configuration(configurationBuilder)
                         .CreateLogger();
            services.AddSingleton(Log.Logger);

            //ViewModels
            services.AddSingleton<MainViewModel>();
            services.AddSingleton<DataViewModel>();
            services.AddSingleton<TrandViewModel>();
            services.AddSingleton<OperationViewModel>();
            services.AddSingleton<OperationBiViewModel>();
            services.AddSingleton<OperationAiViewModel>();
            services.AddSingleton<OperationAlarmViewModel>();
            services.AddSingleton<OperationSchduleViewModel>();
            services.AddSingleton<OperationStorageViewModel>();
            services.AddSingleton<ConfigViewModel>();

            services.AddSingleton<ViewModel_SwitchData>();
            services.AddSingleton<ViewModel_FiAlarmData>();
            services.AddSingleton<ViewModel_DayStatData>();
            services.AddSingleton<ViewModel_StatisticsMinData>();
            services.AddSingleton<ViewModel_StatisticsHourData>();
            services.AddSingleton<ViewModel_StatisticsDayData>();
            services.AddSingleton<ViewModel_StatisticsMonthData>();
            services.AddSingleton<ViewModel_StatisticsYearData>();
            services.AddSingleton<ViewModel_CommDayData>();
            services.AddSingleton<ViewModel_CommLogData>();

            //Views
            services.AddSingleton<MainWindow>();
            services.AddSingleton<DataView>();
            services.AddSingleton<TrandView>();
            services.AddSingleton<OperationView>();
            services.AddSingleton<OperationBiView>();
            services.AddSingleton<OperationAiView>();
            services.AddSingleton<OperationAlarmView>();
            services.AddSingleton<OperationSchduleView>();
            services.AddSingleton<OperationStorageView>();
            services.AddSingleton<ConfigView>();

            services.AddSingleton<View_SwitchData>();
            services.AddSingleton<View_FiAlarmData>();
            services.AddSingleton<View_DayStatData>();
            services.AddSingleton<View_StatisticsMinData>();
            services.AddSingleton<View_StatisticsHourData>();
            services.AddSingleton<View_StatisticsDayData>();
            services.AddSingleton<View_StatisticsMonthData>();
            services.AddSingleton<View_StatisticsYearData>();
            services.AddSingleton<View_CommDayData>();
            services.AddSingleton<View_CommLogData>();

            //ETC
            //services.AddSingleton<View_TestData>();
            //services.AddSingleton<ViewModel_SwitchData>();

            //DB
            string provider = configurationBuilder.GetSection("DbProvider")?.Value ?? "MySql";
            string connStr = configurationBuilder.GetConnectionString("Server")!;
            services.AddServerAccessServices(connStr, provider);
            Log.Logger.Information($"provider:{provider} conn:{connStr}");

            //DATA
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(ServiceExtenions.GetAllAssemblies().ToArray()));

            //Data
            services.AddSingleton<CommonDataModel>();
            services.AddSingleton<DataWorker>();

            return services.BuildServiceProvider();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            ThemedWindow.RoundCorners = true;

            DuplicateExecution();

            var startupForm = Services.GetService<MainWindow>();
            this.MainWindow = startupForm;
            startupForm!.DataContext = Services.GetService<MainViewModel>();
            startupForm!.Show();

            //Log.Logger.Log("프로그램 시작...");
            Log.Information("프로그램 시작...");
            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
        }

        /// <summary>
        /// 중복실행방지
        /// </summary>
        /// <param name="mutexName"></param>
        private void DuplicateExecution()
        {
            string processName = Process.GetCurrentProcess().ProcessName;
            mutex = new Mutex(false, processName);
            if (mutex.WaitOne(0, false))
            {
                InitializeComponent();
            }
            else
            {
                MessageBox.Show("이미 프로그램이 실행 중입니다.", "프로그램 중복 실행", MessageBoxButton.OK, MessageBoxImage.Information);
                Environment.Exit(0);
            }
        }
    }

}

