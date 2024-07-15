using CommunityToolkit.Mvvm.ComponentModel;
using KDMSViewer.Extensions;
using System.Windows;
using System.Windows.Controls;

namespace KDMSViewer.View
{
    /// <summary>
    /// TitleBar.xaml에 대한 상호 작용 논리
    /// </summary>
    [ObservableObject]
    public partial class TitleBar : UserControl
    {
        private Window? _parentWindow;
        private WindowState _winState;

        public WindowState WinState
        {
            get
            {
                return _winState;
            }
            set
            {
                SetProperty(ref _winState, value);
            }
        }

        public Window ParentWindow
        {
            get
            {
                if (_parentWindow == null)
                {
                    _parentWindow = this.FindParent<Window>()!;
                }
                return _parentWindow;
            }
            set
            {
                _parentWindow = value;
            }
        }

        public TitleBar()
        {
            InitializeComponent();
        }

        private void OnMinimizeClick(object sender, RoutedEventArgs e)
        {
            WinState = WindowState.Minimized;
            ParentWindow.WindowState = WinState;
        }

        private void OnMaximizeClick(object sender, RoutedEventArgs e)
        {
            WinState = ParentWindow.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
            ParentWindow.WindowState = WinState;
        }

        private void OnExitClick(object sender, RoutedEventArgs e)
        {
            ParentWindow.Close();
        }

        private void OnMouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if(ParentWindow.WindowState == WindowState.Maximized)
                ParentWindow.WindowState = WindowState.Normal;
            else
                ParentWindow.WindowState = WindowState.Maximized;
        }
    }
}
