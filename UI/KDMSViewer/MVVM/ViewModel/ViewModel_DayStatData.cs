﻿using CommunityToolkit.Mvvm.ComponentModel;
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
    public partial class ViewModel_DayStatData : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<HistoryDaystatData> _pointItems = new ObservableCollection<HistoryDaystatData>();
    }
}
