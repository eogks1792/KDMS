using DevExpress.XtraPrinting;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Extensions.DependencyInjection;

namespace KDMSViewer.View
{
    /// <summary>
    /// TrandView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class TrandView : UserControl
    {
        public TrandView()
        {
            InitializeComponent();
        }

        private void OnChartImageSave(object sender, RoutedEventArgs e)
        {
            var folder = $"{AppDomain.CurrentDomain.BaseDirectory}image";
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.InitialDirectory = folder;
            saveDialog.Filter = "Image files (*.png)|*.png|All files (*.*)|*.*";
            saveDialog.FilterIndex = 1;

            if (saveDialog.ShowDialog() == true)
            {
                var imageExportOptions = new ImageExportOptions
                {
                    Format = ImageFormat.Png,
                    ExportMode = ImageExportMode.SingleFile,
                    PageBorderColor = System.Drawing.Color.White,
                    PageBorderWidth = 0,
                    TextRenderingMode = DevExpress.XtraPrinting.TextRenderingMode.AntiAliasGridFit,
                    Resolution = 96
                };

                chart.ExportToImage(saveDialog.FileName, imageExportOptions, DevExpress.Xpf.Charts.PrintSizeMode.None);
            }


          
        }
    }
}
