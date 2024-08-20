using CommunityToolkit.Mvvm.ComponentModel;
using KDMS.EF.Core.Infrastructure.Reverse.Models;
using KDMSViewer.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDMSViewer.ViewModel
{
    public partial class ViewModel_SwitchData2 : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<HistoryMinDatum> _pointItems = new ObservableCollection<HistoryMinDatum>();

        public ViewModel_SwitchData2()
        {
        }
    }
}
