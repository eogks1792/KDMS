using KDMS.EF.Core.Infrastructure.Reverse.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDMSViewer.Model
{
    public class TabData
    {
        public int Header { get; set; }
        public ObservableCollection<HistoryMinData> PointItems { get; set; } = new ObservableCollection<HistoryMinData>();
    }


}