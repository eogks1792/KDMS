using DevExpress.Xpf.Core;
using KDMSServer.Model;
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
        }

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
        }

        
    }
}