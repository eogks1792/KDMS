﻿using DevExpress.Xpf.Core;
using KDMSViewer.Model;
using System.Diagnostics;
using System.Reflection.Metadata;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static KDMSViewer.Model.Win32API;

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
            MessageBoxResult result = MessageBox.Show($"현재 시스템 동작 중입니다. \n\r프로그램을 종료하시겠습니까?", "프로그램 종료", MessageBoxButton.YesNo, MessageBoxImage.Information);
            if (result == MessageBoxResult.No)
            {
                e.Cancel = true;
                return;
            }

            _worker.ThreadClose();
        }

        //protected override void OnSourceInitialized(EventArgs e)
        //{
        //    base.OnSourceInitialized(e);

        //    Win32API.handle = new WindowInteropHelper(this).Handle;
        //    Win32API.message = RegisterWindowMessage("User Message");
        //    ComponentDispatcher.ThreadFilterMessage += new ThreadMessageEventHandler(_dataWorker.ThreadFilterMessage);
        //}

        //private void Button_Click(object sender, RoutedEventArgs e)
        //{
        //    _dataWorker.PostMessageSend(100);
        //}

        //private void ThreadFilterMessage(ref MSG msg, ref bool handled)
        //{
        //    if (msg.message == message && msg.wParam != handle)
        //    {
        //        MessageBox.Show("Message : " + msg.lParam.ToString());
        //    }
        //}

        //private void Button_Click(object sender, RoutedEventArgs e)
        //{
        //    PostMessage((IntPtr)HWND_BROADCAST, message, (uint)handle, 100);
        //}
    }
}