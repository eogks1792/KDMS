using DevExpress.Xpf.Core;
using KDMSServer.Model;
using System.Security.Claims;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace KDMSServer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : ThemedWindow
    {
        private readonly DataWorker _worker;

        public MainWindow(DataWorker worker)
        {
            InitializeComponent();
            _worker = worker;

            //TEST();
        }

        //public void TEST()
        //{
        //    List<rtdb_Dmc> rtdbDmcs = new List<rtdb_Dmc>();

        //    rtdb_Dmc dmc = new rtdb_Dmc();
        //    dmc.pid = 1;
        //    dmc.value = 99;
        //    dmc.tlq = 0;
        //    rtdbDmcs.Add(dmc);

        //    var findDmc = rtdbDmcs.FirstOrDefault(p => p.pid == 1);
        //    if (findDmc.pid > 0)
        //    {
        //        //var index = rtdbDmcs.IndexOf(findDmc);
        //        findDmc.value = 50;
        //        //rtdbDmcs[index] = findDmc;
        //    }

        //    foreach(var data in rtdbDmcs)
        //    {
        //        MessageBox.Show(data.value.ToString());
        //    }
        //}


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show($"현재 시스템 동작 중입니다. \n\r프로그램을 종료하시겠습니까?", "프로그램 종료", MessageBoxButton.YesNo, MessageBoxImage.Information);
            if (result == MessageBoxResult.No)
            {
                e.Cancel = true;
                return;
            }

            _worker.ThreadClose();
            _worker.SocketClose();

            Thread.Sleep(1000);
        }

        
    }
}