using DevExpress.Xpf.Core;
using KDMSViewer.Model;
using System.Windows;
using System.Windows.Input;

namespace KDMSViewer
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

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            // Begin dragging the window
            this.DragMove();
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult result = _worker.DataYesNoView($"현재 시스템 동작 중입니다. \n프로그램을 종료하시겠습니까?", "프로그램 종료");
            if (result == MessageBoxResult.No)
            {
                e.Cancel = true;
                return;
            }

            _worker.ThreadClose();
        }

        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                if (this.WindowState == WindowState.Maximized)
                    this.WindowState = WindowState.Normal;
                else
                    this.WindowState = WindowState.Maximized;
            }
        }


    }
}