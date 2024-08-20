using CommunityToolkit.Mvvm.ComponentModel;
using KDMS.EF.Core.Infrastructure.Reverse.Models;
using KDMSViewer.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDMSViewer.ViewModel
{
    public partial class ViewModel_StatisticsDayData : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<StatisticsDayData> _pointItems = new ObservableCollection<StatisticsDayData>();
    }
}
