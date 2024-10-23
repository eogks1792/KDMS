using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System.Configuration;
using System.Data;
using System.Windows;
using KDMS.EF.Core.Extensions;
using KDMSViewer.Extensions;
using KDMSServer.ViewModel;
using System.Diagnostics;
using DevExpress.Xpf.Core;
using KDMSServer.Model;

namespace KDMSServer
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

            //Views
            services.AddSingleton<MainWindow>();

            //ViewsModel
            services.AddSingleton<MainViewModel>();

            //Data
            services.AddSingleton<DataWorker>();
            services.AddSingleton<CommonDataModel>();

            //DB
            string provider = configurationBuilder.GetSection("DbProvider")?.Value ?? "MySql";
            string connStr = configurationBuilder.GetConnectionString("Server")!;
            services.AddServerAccessServices(connStr, provider);
            //Log.Logger.Information($"provider:{provider} conn:{connStr}");

            //DATA
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(ServiceExtenions.GetAllAssemblies().ToArray()));


            return services.BuildServiceProvider();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            ThemedWindow.RoundCorners = true;

            DuplicateExecution();

            Log.Logger.Information("KDMS 서버 프로그램 시작...");

            var startupForm = Services.GetService<MainWindow>();
            this.MainWindow = startupForm;
            startupForm!.DataContext = Services.GetService<MainViewModel>();
            startupForm!.Show();
            
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
