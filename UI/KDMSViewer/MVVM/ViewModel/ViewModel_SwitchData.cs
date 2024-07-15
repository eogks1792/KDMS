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
    public partial class ViewModel_SwitchData : ObservableObject
    {
        [ObservableProperty]
        private List<HistoryMinDatum> _pointItems = new List<HistoryMinDatum>();

        public ViewModel_SwitchData(DataWorker worker)
        {
            //PointItems = new ObservableCollection<PointDataModel>()
            //{
            //    new PointDataModel() {PID = 5, SubsName="변전소", DlName="DL#3", SwName="Sw#1", CeqID = 12340005, DataType = "실시간", PointType = 3, PointName = "voltage_a", PointValue = 10, PointTlq = 0, UpdateTime = DateTime.Now.AddMinutes(4)},
            //    new PointDataModel() {PID = 6, SubsName="변전소", DlName="DL#6", SwName="Sw#1", CeqID = 12340006, DataType = "실시간", PointType = 3, PointName = "voltage_b", PointValue = 20, PointTlq = 0, UpdateTime = DateTime.Now.AddMinutes(5)},
            //    new PointDataModel() {PID = 7, SubsName="변전소", DlName="DL#1", SwName="Sw#1", CeqID = 12340007, DataType = "실시간", PointType = 3, PointName = "voltage_c", PointValue = 30, PointTlq = 0, UpdateTime = DateTime.Now.AddMinutes(6)},

            //    new PointDataModel() {PID = 1, SubsName="변전소", DlName="DL#1", SwName="Sw#1", CeqID = 12340007, DataType = "실시간", PointType = 3, PointName = "current_a", PointValue = 30, PointTlq = 0, UpdateTime = DateTime.Now.AddMinutes(10)},
            //    new PointDataModel() {PID = 1, SubsName="변전소", DlName="DL#1", SwName="Sw#1", CeqID = 12340007, DataType = "실시간", PointType = 3, PointName = "current_a", PointValue = 22.5, PointTlq = 0, UpdateTime = DateTime.Now.AddMinutes(13)},
            //    new PointDataModel() {PID = 1, SubsName="변전소", DlName="DL#1", SwName="Sw#1", CeqID = 12340007, DataType = "실시간", PointType = 3, PointName = "current_a", PointValue = 140, PointTlq = 0, UpdateTime = DateTime.Now.AddMinutes(16)},
            //    new PointDataModel() {PID = 1, SubsName="변전소", DlName="DL#1", SwName="Sw#1", CeqID = 12340007, DataType = "실시간", PointType = 3, PointName = "current_a", PointValue = 20, PointTlq = 0, UpdateTime = DateTime.Now.AddMinutes(25)},
            //    new PointDataModel() {PID = 1, SubsName="변전소", DlName="DL#1", SwName="Sw#1", CeqID = 12340007, DataType = "실시간", PointType = 3, PointName = "current_a", PointValue = 12, PointTlq = 0, UpdateTime = DateTime.Now.AddMinutes(30)},

            //    new PointDataModel() {PID = 1, SubsName="변전소", DlName="DL#1", SwName="Sw#1", CeqID = 12340007, DataType = "실시간", PointType = 3, PointName = "current_b", PointValue = 21.2, PointTlq = 0, UpdateTime = DateTime.Now.AddMinutes(10)},
            //    new PointDataModel() {PID = 1, SubsName="변전소", DlName="DL#1", SwName="Sw#1", CeqID = 12340007, DataType = "실시간", PointType = 3, PointName = "current_b", PointValue = 40, PointTlq = 0, UpdateTime = DateTime.Now.AddMinutes(16)},
            //    new PointDataModel() {PID = 1, SubsName="변전소", DlName="DL#1", SwName="Sw#1", CeqID = 12340007, DataType = "실시간", PointType = 3, PointName = "current_b", PointValue = 12, PointTlq = 0, UpdateTime = DateTime.Now.AddMinutes(25)},
            //    new PointDataModel() {PID = 1, SubsName="변전소", DlName="DL#1", SwName="Sw#1", CeqID = 12340007, DataType = "실시간", PointType = 3, PointName = "current_b", PointValue = 80, PointTlq = 0, UpdateTime = DateTime.Now.AddMinutes(30)},

            //    new PointDataModel() {PID = 1, SubsName="변전소", DlName="DL#1", SwName="Sw#1", CeqID = 12340007, DataType = "실시간", PointType = 3, PointName = "current_c", PointValue = 10, PointTlq = 0, UpdateTime = DateTime.Now.AddMinutes(10)},
            //    new PointDataModel() {PID = 1, SubsName="변전소", DlName="DL#1", SwName="Sw#1", CeqID = 12340007, DataType = "실시간", PointType = 3, PointName = "current_c", PointValue = 20, PointTlq = 0, UpdateTime = DateTime.Now.AddMinutes(16)},
            //    new PointDataModel() {PID = 1, SubsName="변전소", DlName="DL#1", SwName="Sw#1", CeqID = 12340007, DataType = "실시간", PointType = 3, PointName = "current_c", PointValue = 30, PointTlq = 0, UpdateTime = DateTime.Now.AddMinutes(25)},
            //    new PointDataModel() {PID = 1, SubsName="변전소", DlName="DL#1", SwName="Sw#1", CeqID = 12340007, DataType = "실시간", PointType = 3, PointName = "current_c", PointValue = 42, PointTlq = 0, UpdateTime = DateTime.Now.AddMinutes(30)},

            //    new PointDataModel() {PID = 1, SubsName="변전소", DlName="DL#1", SwName="Sw#1", CeqID = 12340007, DataType = "실시간", PointType = 3, PointName = "current_n", PointValue = 40, PointTlq = 0, UpdateTime = DateTime.Now.AddMinutes(10)},
            //    new PointDataModel() {PID = 1, SubsName="변전소", DlName="DL#1", SwName="Sw#1", CeqID = 12340007, DataType = "실시간", PointType = 3, PointName = "current_n", PointValue = 60, PointTlq = 0, UpdateTime = DateTime.Now.AddMinutes(16)},
            //    new PointDataModel() {PID = 1, SubsName="변전소", DlName="DL#1", SwName="Sw#1", CeqID = 12340007, DataType = "실시간", PointType = 3, PointName = "current_n", PointValue = 40, PointTlq = 0, UpdateTime = DateTime.Now.AddMinutes(25)},
            //    new PointDataModel() {PID = 1, SubsName="변전소", DlName="DL#1", SwName="Sw#1", CeqID = 12340007, DataType = "실시간", PointType = 3, PointName = "current_n", PointValue = 12, PointTlq = 0, UpdateTime = DateTime.Now.AddMinutes(30)},

            //    new PointDataModel() {PID = 1, SubsName="변전소", DlName="DL#1", SwName="Sw#1", CeqID = 12340007, DataType = "실시간", PointType = 3, PointName = "current_a", PointValue = 60, PointTlq = 0, UpdateTime = DateTime.Now.AddHours(4)},
            //    new PointDataModel() {PID = 1, SubsName="변전소", DlName="DL#1", SwName="Sw#1", CeqID = 12340007, DataType = "실시간", PointType = 3, PointName = "current_a", PointValue = 90, PointTlq = 0, UpdateTime = DateTime.Now.AddHours(7)},
            //    new PointDataModel() {PID = 1, SubsName="변전소", DlName="DL#1", SwName="Sw#1", CeqID = 12340007, DataType = "실시간", PointType = 3, PointName = "current_a", PointValue = 30, PointTlq = 0, UpdateTime = DateTime.Now.AddHours(8)},
            //    new PointDataModel() {PID = 1, SubsName="변전소", DlName="DL#1", SwName="Sw#1", CeqID = 12340007, DataType = "실시간", PointType = 3, PointName = "current_a", PointValue = 60, PointTlq = 0, UpdateTime = DateTime.Now.AddHours(10)},
            //    new PointDataModel() {PID = 1, SubsName="변전소", DlName="DL#1", SwName="Sw#1", CeqID = 12340007, DataType = "실시간", PointType = 3, PointName = "current_a", PointValue = 90, PointTlq = 0, UpdateTime = DateTime.Now.AddHours(11)},

            //    new PointDataModel() {PID = 1, SubsName="변전소", DlName="DL#1", SwName="Sw#1", CeqID = 12340007, DataType = "실시간", PointType = 3, PointName = "current_a", PointValue = 60, PointTlq = 0, UpdateTime = DateTime.Now.AddDays(4)},
            //    new PointDataModel() {PID = 1, SubsName="변전소", DlName="DL#1", SwName="Sw#1", CeqID = 12340007, DataType = "실시간", PointType = 3, PointName = "current_a", PointValue = 90, PointTlq = 0, UpdateTime = DateTime.Now.AddDays(13)},
            //    new PointDataModel() {PID = 1, SubsName="변전소", DlName="DL#1", SwName="Sw#1", CeqID = 12340007, DataType = "실시간", PointType = 3, PointName = "current_a", PointValue = 30, PointTlq = 0, UpdateTime = DateTime.Now.AddDays(14)},
            //    new PointDataModel() {PID = 1, SubsName="변전소", DlName="DL#1", SwName="Sw#1", CeqID = 12340007, DataType = "실시간", PointType = 3, PointName = "current_a", PointValue = 60, PointTlq = 0, UpdateTime = DateTime.Now.AddDays(15)},
            //    new PointDataModel() {PID = 1, SubsName="변전소", DlName="DL#1", SwName="Sw#1", CeqID = 12340007, DataType = "실시간", PointType = 3, PointName = "current_a", PointValue = 90, PointTlq = 0, UpdateTime = DateTime.Now.AddDays(16)},

            //    new PointDataModel() {PID = 1, SubsName="변전소", DlName="DL#1", SwName="Sw#1", CeqID = 12340007, DataType = "실시간", PointType = 3, PointName = "current_b", PointValue = 40, PointTlq = 0, UpdateTime = DateTime.Now.AddDays(10)},
            //    new PointDataModel() {PID = 1, SubsName="변전소", DlName="DL#1", SwName="Sw#1", CeqID = 12340007, DataType = "실시간", PointType = 3, PointName = "current_b", PointValue = 20, PointTlq = 0, UpdateTime = DateTime.Now.AddDays(13)},
            //    new PointDataModel() {PID = 1, SubsName="변전소", DlName="DL#1", SwName="Sw#1", CeqID = 12340007, DataType = "실시간", PointType = 3, PointName = "current_b", PointValue = 80, PointTlq = 0, UpdateTime = DateTime.Now.AddDays(16)},
            //    new PointDataModel() {PID = 1, SubsName="변전소", DlName="DL#1", SwName="Sw#1", CeqID = 12340007, DataType = "실시간", PointType = 3, PointName = "current_b", PointValue = 10, PointTlq = 0, UpdateTime = DateTime.Now.AddDays(25)},
            //    new PointDataModel() {PID = 1, SubsName="변전소", DlName="DL#1", SwName="Sw#1", CeqID = 12340007, DataType = "실시간", PointType = 3, PointName = "current_b", PointValue = 20, PointTlq = 0, UpdateTime = DateTime.Now.AddDays(30)}
            //};

        }
    }
}
