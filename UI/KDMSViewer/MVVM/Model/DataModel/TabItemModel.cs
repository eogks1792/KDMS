using KDMS.EF.Core.Infrastructure.Reverse.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace KDMSViewer.Model
{
    public class TabData
    {
        public int Header { get; set; }
        public ObservableCollection<object> PointItems { get; set; } = new ObservableCollection<object>();
    }
}