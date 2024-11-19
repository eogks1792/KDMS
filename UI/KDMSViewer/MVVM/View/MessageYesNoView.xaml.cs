using DevExpress.Xpf.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace KDMSViewer.View
{
    /// <summary>
    /// MessageYesNoView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MessageYesNoView : ThemedWindow
    {
        public MessageYesNoView()
        {
            InitializeComponent();
        }

        private void OnYes(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void OnNo(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
