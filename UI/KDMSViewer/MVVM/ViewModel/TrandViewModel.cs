using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DevExpress.Mvvm.Native;
using DevExpress.Office.Utils;
using KDMS.EF.Core.Infrastructure.Reverse.Models;
using KDMSViewer.Model;
using KDMSViewer.View;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace KDMSViewer.ViewModel
{
    public partial class TrandViewModel : ObservableObject
    {
        private readonly CommonDataModel _commonData;
        private readonly DataWorker _worker;
        [ObservableProperty]
        private ObservableCollection<TreeDataModel> _treeItems;

        [ObservableProperty]
        private TreeDataModel _DLSelected;

        [ObservableProperty]
        private ObservableCollection<HistoryMinDatum> _pointItems;

        [ObservableProperty]
        private ObservableCollection<ChartModel> _seriesItems;

        [ObservableProperty]
        private bool _currentCheck = false;
        [ObservableProperty]
        private bool _currentA = false;
        [ObservableProperty]
        private bool _currentB = false;
        [ObservableProperty]
        private bool _currentC = false;
        [ObservableProperty]
        private bool _currentN = false;

        [ObservableProperty]
        private bool _voltageCheck = false;
        [ObservableProperty]
        private bool _voltageA = false;
        [ObservableProperty]
        private bool _voltageB = false;
        [ObservableProperty]
        private bool _voltageC = false;

        [ObservableProperty]
        private bool _powerCheck = false;
        //[ObservableProperty]
        //private bool _powerAll = false;
        [ObservableProperty]
        private bool _powerA = false;
        [ObservableProperty]
        private bool _powerB = false;
        [ObservableProperty]
        private bool _powerC = false;
        //[ObservableProperty]
        //private bool _apparentPower = false;

        [ObservableProperty]
        private bool _powerFactorCheck = false;
        [ObservableProperty]
        private bool _powerFactor3p = false;
        [ObservableProperty]
        private bool _powerFactorA = false;
        [ObservableProperty]
        private bool _powerFactorB = false;
        [ObservableProperty]
        private bool _powerFactorC = false;

        [ObservableProperty]
        private bool _faultCurrentCheck = false;
        [ObservableProperty]
        private bool _faultCurrentA = false;
        [ObservableProperty]
        private bool _faultCurrentB = false;
        [ObservableProperty]
        private bool _faultCurrentC = false;
        [ObservableProperty]
        private bool _faultCurrentN = false;

        [ObservableProperty]
        private DateTime _fromDate;

        [ObservableProperty]
        private DateTime _toDate;

        public LoadingView view { get; set; }

        [ObservableProperty]
        private bool _isInquiry = true;

        public TrandViewModel(DataWorker worker)
        {
            _worker = worker;

            DataInit();
            //PointItems = new ObservableCollection<PointDataModel>()
            //{
            //    new PointDataModel() {PID = 5, SubsName="변전소", DlName="DL#3", SwName="Sw#1", CeqID = 12340005, DataType = "실시간", PointType = 3, PointName = "voltage_a", PointValue = 10, PointTlq = 0, UpdateTime = DateTime.Now.AddMinutes(4)},
            //    new PointDataModel() {PID = 6, SubsName="변전소", DlName="DL#6", SwName="Sw#1", CeqID = 12340006, DataType = "실시간", PointType = 3, PointName = "voltage_b", PointValue = 20, PointTlq = 0, UpdateTime = DateTime.Now.AddMinutes(5)},
            //    new PointDataModel() {PID = 7, SubsName="변전소", DlName="DL#1", SwName="Sw#1", CeqID = 12340007, DataType = "실시간", PointType = 3, PointName = "voltage_c", PointValue = 30, PointTlq = 0, UpdateTime = DateTime.Now.AddMinutes(6)},

            //    new PointDataModel() {PID = 5, SubsName="변전소", DlName="DL#3", SwName="Sw#1", CeqID = 12340005, DataType = "실시간", PointType = 3, PointName = "power_factor_a", PointValue = 10, PointTlq = 0, UpdateTime = DateTime.Now.AddHours(4)},
            //    new PointDataModel() {PID = 6, SubsName="변전소", DlName="DL#6", SwName="Sw#1", CeqID = 12340006, DataType = "실시간", PointType = 3, PointName = "power_factor_b", PointValue = 20, PointTlq = 0, UpdateTime = DateTime.Now.AddHours(8)},
            //    new PointDataModel() {PID = 7, SubsName="변전소", DlName="DL#1", SwName="Sw#1", CeqID = 12340007, DataType = "실시간", PointType = 3, PointName = "power_factor_c", PointValue = 30, PointTlq = 0, UpdateTime = DateTime.Now.AddHours(9)},

            //    new PointDataModel() {PID = 5, SubsName="변전소", DlName="DL#3", SwName="Sw#1", CeqID = 12340005, DataType = "실시간", PointType = 3, PointName = "fi_current_a", PointValue = 10, PointTlq = 0, UpdateTime = DateTime.Now.AddHours(2)},
            //    new PointDataModel() {PID = 6, SubsName="변전소", DlName="DL#6", SwName="Sw#1", CeqID = 12340006, DataType = "실시간", PointType = 3, PointName = "fi_current_b", PointValue = 20, PointTlq = 0, UpdateTime = DateTime.Now.AddHours(3)},
            //    new PointDataModel() {PID = 7, SubsName="변전소", DlName="DL#1", SwName="Sw#1", CeqID = 12340007, DataType = "실시간", PointType = 3, PointName = "fi_current_c", PointValue = 30, PointTlq = 0, UpdateTime = DateTime.Now.AddHours(6)},
            //    new PointDataModel() {PID = 7, SubsName="변전소", DlName="DL#1", SwName="Sw#1", CeqID = 12340007, DataType = "실시간", PointType = 3, PointName = "fi_current_n", PointValue = 30, PointTlq = 0, UpdateTime = DateTime.Now.AddHours(9)},

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
       

        private void DataInit()
        {
            FromDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd 00:00:00"));
            ToDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd 23:59:59"));

            TreeItems = new ObservableCollection<TreeDataModel>(_worker.TreeDatas);

            //for (int i = 0; i < 3; i++)
            //{
            //    TreeDataModel subs = new TreeDataModel();
            //    subs.Name = $"변전소#{i + 1}";

            //    for (int j = 0; j < 10; j++)
            //    {
            //        TreeDataModel dl = new TreeDataModel();
            //        dl.Name = $"DL#{j + 1}";

            //        if (j % 2 == 0)
            //        {
            //            TreeDataModel sw = new TreeDataModel();
            //            sw.Name = $"Switch#{j}";
            //            dl.DataModels.Add(sw);

            //            sw = new TreeDataModel();
            //            sw.Name = $"Switch#{j + 1}";
            //            dl.DataModels.Add(sw);
            //        }

            //        subs.DataModels.Add(dl);
            //    }

            //    DLItems.Add(subs);
            //}
        }

        private List<ChartPointDataModel> CheckItemList()
        {
            //var chartDatas = PointItems.Where(p => p.SaveTime > FromDate && p.SaveTime < ToDate).ToList();
            List<ChartPointDataModel> retList = new List<ChartPointDataModel>();

            string name = string.Empty;
            string check = string.Empty;
            // 체크된 항목된 가져오도록 처리
            name = "current";
            if (CurrentA)
            {
                check = $"{name}_a";
                //retList.AddRange(chartDatas.Where(p => p.PointName.Contains(check)).ToList());
                retList.AddRange(PointItems.Select(p => new ChartPointDataModel
                {
                    CeqId = p.Ceqid,
                    Name = p.Name ?? string.Empty,
                    PointName = check,
                    PointValue = p.CurrentA ?? 0.0f,
                    UpdateTime = p.SaveTime

                }).ToList());
            }
            if (CurrentB)
            {
                check = $"{name}_b";
                //retList.AddRange(chartDatas.Where(p => p.PointName.Contains(check)).ToList());
                retList.AddRange(PointItems.Select(p => new ChartPointDataModel
                {
                    CeqId = p.Ceqid,
                    Name = p.Name ?? string.Empty,
                    PointName = check,
                    PointValue = p.CurrentB ?? 0.0f,
                    UpdateTime = p.SaveTime

                }).ToList());
            }
            if (CurrentC)
            {
                check = $"{name}_c";
                //retList.AddRange(chartDatas.Where(p => p.PointName.Contains(check)).ToList());
                retList.AddRange(PointItems.Select(p => new ChartPointDataModel
                {
                    CeqId = p.Ceqid,
                    Name = p.Name ?? string.Empty,
                    PointName = check,
                    PointValue = p.CurrentC ?? 0.0f,
                    UpdateTime = p.SaveTime

                }).ToList());
            }
            if (CurrentN)
            {
                check = $"{name}_n";
                //retList.AddRange(chartDatas.Where(p => p.PointName.Contains(check)).ToList());
                retList.AddRange(PointItems.Select(p => new ChartPointDataModel
                {
                    CeqId = p.Ceqid,
                    Name = p.Name ?? string.Empty,
                    PointName = check,
                    PointValue = p.CurrentN ?? 0.0f,
                    UpdateTime = p.SaveTime

                }).ToList());
            }

            name = "voltage";
            if (VoltageA)
            {
                check = $"{name}_a";
                //retList.AddRange(chartDatas.Where(p => p.PointName.Contains(check)).ToList());
                retList.AddRange(PointItems.Select(p => new ChartPointDataModel
                {
                    CeqId = p.Ceqid,
                    Name = p.Name ?? string.Empty,
                    PointName = check,
                    PointValue = p.VoltageA ?? 0.0f,
                    UpdateTime = p.SaveTime

                }).ToList());
            }
            if (VoltageB)
            {
                check = $"{name}_b";
                //retList.AddRange(chartDatas.Where(p => p.PointName.Contains(check)).ToList());
                retList.AddRange(PointItems.Select(p => new ChartPointDataModel
                {
                    CeqId = p.Ceqid,
                    Name = p.Name ?? string.Empty,
                    PointName = check,
                    PointValue = p.VoltageB ?? 0.0f,
                    UpdateTime = p.SaveTime

                }).ToList());
            }
            if (VoltageC)
            {
                check = $"{name}_c";
                //retList.AddRange(chartDatas.Where(p => p.PointName.Contains(check)).ToList());
                retList.AddRange(PointItems.Select(p => new ChartPointDataModel
                {
                    CeqId = p.Ceqid,
                    Name = p.Name ?? string.Empty,
                    PointName = check,
                    PointValue = p.VoltageC ?? 0.0f,
                    UpdateTime = p.SaveTime

                }).ToList());
            }

            name = "apparent_power";
            if (PowerA)
            {
                check = $"{name}_a";
                //retList.AddRange(chartDatas.Where(p => p.PointName.Contains(check)).ToList());
                retList.AddRange(PointItems.Select(p => new ChartPointDataModel
                {
                    CeqId = p.Ceqid,
                    Name = p.Name ?? string.Empty,
                    PointName = check,
                    PointValue = p.ApparentPowerA ?? 0.0f,
                    UpdateTime = p.SaveTime

                }).ToList());
            }
            if (PowerB)
            {
                check = $"{name}_b";
                //retList.AddRange(chartDatas.Where(p => p.PointName.Contains(check)).ToList());
                retList.AddRange(PointItems.Select(p => new ChartPointDataModel
                {
                    CeqId = p.Ceqid,
                    Name = p.Name ?? string.Empty,
                    PointName = check,
                    PointValue = p.ApparentPowerB ?? 0.0f,
                    UpdateTime = p.SaveTime

                }).ToList());
            }
            if (PowerC)
            {
                check = $"{name}_c";
                //retList.AddRange(chartDatas.Where(p => p.PointName.Contains(check)).ToList());
                retList.AddRange(PointItems.Select(p => new ChartPointDataModel
                {
                    CeqId = p.Ceqid,
                    Name = p.Name ?? string.Empty,
                    PointName = check,
                    PointValue = p.ApparentPowerC ?? 0.0f,
                    UpdateTime = p.SaveTime

                }).ToList());
            }
            //if (ApparentPower)
            //{
            //    name += "_c";
            //    chartDatas = chartDatas.Where(p => p.PointName.Contains(name)).ToList();
            //}

            name = "power_factor";
            if (PowerFactor3p)
            {
                check = $"{name}_3p";
                //retList.AddRange(chartDatas.Where(p => p.PointName.Contains(check)).ToList());
                retList.AddRange(PointItems.Select(p => new ChartPointDataModel
                {
                    CeqId = p.Ceqid,
                    Name = p.Name ?? string.Empty,
                    PointName = check,
                    PointValue = p.PowerFactor3p ?? 0.0f,
                    UpdateTime = p.SaveTime

                }).ToList());
            }
            if (PowerFactorA)
            {
                check = $"{name}_a";
                //retList.AddRange(chartDatas.Where(p => p.PointName.Contains(check)).ToList());
                retList.AddRange(PointItems.Select(p => new ChartPointDataModel
                {
                    CeqId = p.Ceqid,
                    Name = p.Name ?? string.Empty,
                    PointName = check,
                    PointValue = p.PowerFactorA ?? 0.0f,
                    UpdateTime = p.SaveTime

                }).ToList());
            }
            if (PowerFactorB)
            {
                check = $"{name}_b";
                //retList.AddRange(chartDatas.Where(p => p.PointName.Contains(check)).ToList());
                retList.AddRange(PointItems.Select(p => new ChartPointDataModel
                {
                    CeqId = p.Ceqid,
                    Name = p.Name ?? string.Empty,
                    PointName = check,
                    PointValue = p.PowerFactorB ?? 0.0f,
                    UpdateTime = p.SaveTime

                }).ToList());
            }
            if (PowerFactorC)
            {
                check = $"{name}_c";
                //retList.AddRange(chartDatas.Where(p => p.PointName.Contains(check)).ToList());
                retList.AddRange(PointItems.Select(p => new ChartPointDataModel
                {
                    PointName = check,
                    Name = p.Name ?? string.Empty,
                    CeqId = p.Ceqid,
                    PointValue = p.PowerFactorC ?? 0.0f,
                    UpdateTime = p.SaveTime

                }).ToList());
            }

            name = "fi_current";
            if (FaultCurrentA)
            {
                check = $"{name}_a";
                //retList.AddRange(chartDatas.Where(p => p.PointName.Contains(check)).ToList());
                retList.AddRange(PointItems.Select(p => new ChartPointDataModel
                {
                    PointName = check,
                    Name = p.Name ?? string.Empty,
                    CeqId = p.Ceqid,
                    PointValue = p.FaultCurrentA ?? 0.0f,
                    UpdateTime = p.SaveTime

                }).ToList());
            }
            if (FaultCurrentB)
            {
                check = $"{name}_b";
                //retList.AddRange(chartDatas.Where(p => p.PointName.Contains(check)).ToList());
                retList.AddRange(PointItems.Select(p => new ChartPointDataModel
                {
                    PointName = check,
                    Name = p.Name ?? string.Empty,
                    CeqId = p.Ceqid,
                    PointValue = p.FaultCurrentB ?? 0.0f,
                    UpdateTime = p.SaveTime

                }).ToList());
            }
            if (FaultCurrentC)
            {
                check = $"{name}_c";
                //retList.AddRange(chartDatas.Where(p => p.PointName.Contains(check)).ToList());
                retList.AddRange(PointItems.Select(p => new ChartPointDataModel
                {
                    CeqId = p.Ceqid,
                    Name = p.Name ?? string.Empty,
                    PointName = check,
                    PointValue = p.FaultCurrentC ?? 0.0f,
                    UpdateTime = p.SaveTime

                }).ToList());
            }
            if (FaultCurrentN)
            {
                check = $"{name}_n";
                //retList.AddRange(chartDatas.Where(p => p.PointName.Contains(check)).ToList());
                retList.AddRange(PointItems.Select(p => new ChartPointDataModel
                {
                    CeqId = p.Ceqid,
                    Name = p.Name ?? string.Empty,
                    PointName = check,
                    PointValue = p.FaultCurrentN ?? 0.0f,
                    UpdateTime = p.SaveTime

                }).ToList());
            }

            return retList;
        }

        public void OnCurrentChecked(object sender, RoutedEventArgs e)
        {
            CurrentA = true;
            CurrentB = true;
            CurrentC = true;
            CurrentN = true;
        }

        public void OnCurrentUnChecked(object sender, RoutedEventArgs e)
        {
            CurrentA = false;
            CurrentB = false;
            CurrentC = false;
            CurrentN = false;
        }

        public void OnVoltageChecked(object sender, RoutedEventArgs e)
        {
            VoltageA = true;
            VoltageB = true;
            VoltageC = true;
        }

        public void OnVoltageUnChecked(object sender, RoutedEventArgs e)
        {
            VoltageA = false;
            VoltageB = false;
            VoltageC = false;
        }

        public void OnPowerChecked(object sender, RoutedEventArgs e)
        {
            //PowerAll = true;
            PowerA = true;
            PowerB = true;
            PowerC = true;
            //ApparentPower = true;
        }

        public void OnPowerUnChecked(object sender, RoutedEventArgs e)
        {
            //PowerAll = false;
            PowerA = false;
            PowerB = false;
            PowerC = false;
            //ApparentPower = false;
        }

        public void OnPowerFactorChecked(object sender, RoutedEventArgs e)
        {
            PowerFactor3p = true;
            PowerFactorA = true;
            PowerFactorB = true;
            PowerFactorC = true;
        }

        public void OnPowerFactorUnChecked(object sender, RoutedEventArgs e)
        {
            PowerFactor3p = false;
            PowerFactorA = false;
            PowerFactorB = false;
            PowerFactorC = false;
        }

        public void OnFaultCurrentChecked(object sender, RoutedEventArgs e)
        {
            FaultCurrentA = true;
            FaultCurrentB = true;
            FaultCurrentC = true;
            FaultCurrentN = true;
        }

        public void OnFaultCurrentUnChecked(object sender, RoutedEventArgs e)
        {
            FaultCurrentA = false;
            FaultCurrentB = false;
            FaultCurrentC = false;
            FaultCurrentN = false;
        }

        private void GetData()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                view = new LoadingView();
                view.Owner = Application.Current.MainWindow;
                view.Show();
            });

            _worker.GetTrandData(FromDate, ToDate);
            if (PointItems == null)
                return;

            // 날짜 데이터 범위 
            // 체크된 변전소, DL, 스위치 
            // 데이터 가져와 화면에 표시
            SeriesItems = new ObservableCollection<ChartModel>();
            var chartDatas = CheckItemList();
            if (chartDatas.Count > 0)
            {
                var distintPoints = chartDatas.Select(p => new { Name = $"{p.CeqId}({p.PointName})", DisPlayName = $"{p.Name} ({p.PointName})" }).Distinct().ToList();
                foreach (var data in distintPoints)
                {
                    var findDatas = chartDatas.Where(p => $"{p.CeqId}({p.PointName})" == data.Name).ToList();
                    if (findDatas.Count > 0)
                    {
                        var dataList = findDatas.Select(p => new ChartDateModel
                        {
                            Date = p.UpdateTime,
                            Value = p.PointValue
                        }).ToList();

                        SeriesItems.Add(new ChartModel()
                        {
                            Name = data.DisPlayName,
                            Datas = dataList
                        });
                    }
                }
            }

            Application.Current.Dispatcher.Invoke(() =>
            {
                view.Close();
                IsInquiry = true;
            });
        }

        [RelayCommand]
        private async void Inquiry()
        {
            await Task.Run(() =>
            {
                IsInquiry = false;
                GetData();
            });
        }
    }
}
