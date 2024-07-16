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
        private TreeDataModel _treeSelected;

        [ObservableProperty]
        private ObservableCollection<HistoryMinDatum> _pointItems;

        [ObservableProperty]
        private ObservableCollection<ChartModel> _seriesItems;

        [ObservableProperty]
        private bool _typeCheck = false;
        [ObservableProperty]
        private bool _average = false;
        [ObservableProperty]
        private bool _max = false;
        [ObservableProperty]
        private bool _min = false;

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
        private DateTime _fromDate;

        [ObservableProperty]
        private DateTime _toDate;

        [ObservableProperty]
        private DateTime _fromTime;

        [ObservableProperty]
        private DateTime _toTime;

      
        public TrandViewModel(DataWorker worker)
        {
            _worker = worker;
            DataInit();
        }
        private void DataInit()
        {
            FromDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd 00:00:00"));
            ToDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd 23:59:59"));

            TreeItems = new ObservableCollection<TreeDataModel>(_worker.TreeDatas);
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
            
        }

        public void OnPowerUnChecked(object sender, RoutedEventArgs e)
        {
          
        }

        public void OnPowerFactorChecked(object sender, RoutedEventArgs e)
        {
           
        }

        public void OnPowerFactorUnChecked(object sender, RoutedEventArgs e)
        {
           
        }

        public void OnFaultCurrentChecked(object sender, RoutedEventArgs e)
        {
            
        }

        public void OnFaultCurrentUnChecked(object sender, RoutedEventArgs e)
        {
           
        }

        private void GetData()
        {
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
