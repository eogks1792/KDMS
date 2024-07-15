using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace KDMSViewer.Converters
{
    class GridIndexConverter : BaseValueConverter<GridIndexConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DataGridRow item = (DataGridRow)value;
            DataGrid dataGrid = ItemsControl.ItemsControlFromItemContainer(item) as DataGrid;
            int index = dataGrid.ItemContainerGenerator.IndexFromContainer(item) + 1;
            return index.ToString();
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
