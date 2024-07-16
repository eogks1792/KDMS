using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DevExpress.ClipboardSource.SpreadsheetML;
using DevExpress.Mvvm.POCO;
using DevExpress.Mvvm;
using DevExpress.Xpf.Editors;
using DevExpress.XtraRichEdit.Commands;
using KDMS.EF.Core.Infrastructure.Reverse.Models;
using KDMSViewer.Model;
using KDMSViewer.View;
using Microsoft.Extensions.DependencyInjection;

//using LiveCharts;
//using LiveCharts.Defaults;
//using LiveCharts.Wpf;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using DevExpress.Mvvm.UI;
using System;
using System.Globalization;
using System.Text;
using DevExpress.Utils.CommonDialogs;

namespace KDMSViewer.ViewModel
{
    public partial class DataViewModel : ObservableObject
    {
        private readonly DataWorker _worker;
        [ObservableProperty]
        private ObservableCollection<TreeDataModel> _treeItems;
        [ObservableProperty]
        private TreeDataModel _DLSelected;
        //[ObservableProperty]
        //private ObservableCollection<PointDataModel> _pointItems;

        [ObservableProperty]
        private bool _realDataCheck = true;
        [ObservableProperty]
        private bool _switchCheck = true;
        [ObservableProperty]
        private bool _fiCheck;

        [ObservableProperty]
        private bool _statisticalCheck;
        [ObservableProperty]
        private bool _dayCheck;
        [ObservableProperty]
        private bool _statisticMinCheck; // STATISTIC
        [ObservableProperty]
        private bool _StatisticHourCheck;
        [ObservableProperty]
        private bool _statisticDayCheck;
        [ObservableProperty]
        private bool _statisticMonthCheck;
        [ObservableProperty]
        private bool _statisticYearCheck;

        [ObservableProperty]
        private bool _commCheck;
        [ObservableProperty]
        private bool _commDayCheck;
        [ObservableProperty]
        private bool _commLogCheck;

        [ObservableProperty]
        private DateTime _fromDate;

        [ObservableProperty]
        private DateTime _toDate;

        [ObservableProperty]
        public string _timeEditMask = CultureInfo.CurrentCulture.DateTimeFormat.LongTimePattern;

        [ObservableProperty]
        private DateTime _fromTime = Convert.ToDateTime(DateTime.Now.ToString("HH:00:00"));

        [ObservableProperty]
        private DateTime _toTime = Convert.ToDateTime(DateTime.Now.AddHours(1).ToString("HH:00:00"));

        [ObservableProperty]
        private Visibility _switchVisibility = Visibility.Visible;

        [ObservableProperty]
        private Visibility _realVisibility = Visibility.Visible;

        [ObservableProperty]
        private Visibility _statisticalVisibility = Visibility.Hidden;

        [ObservableProperty]
        private Visibility _commVisibility = Visibility.Hidden;

        [ObservableProperty]
        private ObservableCollection<ChartModel> _seriesItems;


        [ObservableProperty]
        private INotifyPropertyChanged _childViewModel;

        private List<long> CheckItems { get; set; }

        //public LoadingView view { get; set; }

        [ObservableProperty]
        private bool _isInquiry = true;

        public DataViewModel(DataWorker worker)
        {
            _worker = worker;
            DataInit();

            ChildViewModel = (INotifyPropertyChanged)App.Current.Services.GetService<ViewModel_SwitchData>()!;
        }

        private void DataInit()
        {
            FromDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd 00:00:00"));
            ToDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd 23:59:59"));

            TreeItems = new ObservableCollection<TreeDataModel>(_worker.TreeDatas);
        }

        private void GetTreeItemCheckData(ObservableCollection<TreeDataModel> TreeItems)
        {
            foreach (var item in TreeItems)
            {
                if (item.IsChecked)
                {
                    if (item.Type == TreeTypeCode.EQUIPMENT)
                    {
                        CheckItems.Add(item.Id);
                    }
                }
                else
                {
                    if (item.DataModels.Count > 0)
                        GetTreeItemCheckData(item.DataModels);
                }
            }
        }

        private void GetData()
        {
            //Application.Current.Dispatcher.Invoke(() =>
            //{
            //    view = new LoadingView();
            //    view.Owner = Application.Current.MainWindow;
            //    view.Show();
            //});
            CheckItems = new List<long>();
            GetTreeItemCheckData(TreeItems);
            if (CheckItems.Count <= 0)
            {
                MessageBox.Show("선택된 데이터가 없습니다.", "데이터 조회", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // 데이터 취득 처리
            if (RealDataCheck)
            {
                // 실시간 데이터
                if (SwitchCheck)
                {
                    _worker.GetSearchData(CheckItems, (int)SearchTypeCode.MINDATA, FromDate, ToDate, FromTime, ToTime);
                }
                else if (FiCheck)
                {
                    _worker.GetSearchData(CheckItems, (int)SearchTypeCode.FIALARM, FromDate, ToDate, FromTime, ToTime);
                }
            }
            else if (StatisticalCheck)
            {
                if (DayCheck)
                {
                    _worker.GetSearchData(CheckItems, (int)SearchTypeCode.DAYSTATDATA, FromDate, ToDate);
                }
                else
                {
                    // 통계 데이터
                    if (StatisticMinCheck)
                    {
                        _worker.GetSearchData(CheckItems, (int)SearchTypeCode.STATISTICSMIN, FromDate, ToDate);
                    }
                    else if (StatisticHourCheck)
                    {
                        _worker.GetSearchData(CheckItems, (int)SearchTypeCode.STATISTICSHOUR, FromDate, ToDate);
                    }
                    else if (StatisticDayCheck)
                    {
                        _worker.GetSearchData(CheckItems, (int)SearchTypeCode.STATISTICSDAY, FromDate, ToDate);
                    }
                    else if (StatisticMonthCheck)
                    {
                        _worker.GetSearchData(CheckItems, (int)SearchTypeCode.STATISTICSMONTH, FromDate, ToDate);
                    }
                    else if (StatisticYearCheck)
                    {
                        _worker.GetSearchData(CheckItems, (int)SearchTypeCode.STATISTICSYEAR, FromDate, ToDate);
                    }
                }
            }
            else if (CommCheck)
            {
                if (CommDayCheck)
                {
                    _worker.GetSearchData(CheckItems, (int)SearchTypeCode.COMMSTATE, FromDate, ToDate);
                }
                else if (CommLogCheck)
                {
                    _worker.GetSearchData(CheckItems, (int)SearchTypeCode.COMMSTATELOG, FromDate, ToDate);
                }
            }

            if (SwitchCheck)
            {
                ChartDataInit();
            }

            //Application.Current.Dispatcher.Invoke(() =>
            //{
            //    view.Close();
            //    IsInquiry = true;
            //});

            IsInquiry = true;
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

        private void ChartDataInit()
        {
            SeriesItems = new ObservableCollection<ChartModel>();
            var chartDatas = (List<HistoryMinDatum>)_worker.GetPointItems((int)SearchTypeCode.MINDATA);
            if (chartDatas != null && chartDatas.Count > 0)
            {
                var distintPoints = chartDatas.Select(p => new { CeqId = p.Ceqid, DisplayName = p.Name } ).Distinct().ToList();
                foreach (var data in distintPoints)
                {
                    var lstColumns = GetColumnsNames<HistoryMinDatum>();
                    foreach (var column in lstColumns)
                    {
                        switch (column)
                        {
                            case "currenta":
                                {
                                    var dataList = chartDatas.Where(p => p.Ceqid == data.CeqId).Select(p => new ChartDateModel
                                    {
                                        Date = p.SaveTime,
                                        Value = p.CurrentA ?? 0.0f
                                    }).ToList();

                                    SeriesItems.Add(new ChartModel()
                                    {
                                        Name = $"{data.DisplayName}(current_a)",
                                        Datas = dataList
                                    });
                                }
                                break;
                            case "currentb":
                                {
                                    var dataList = chartDatas.Where(p => p.Ceqid == data.CeqId).Select(p => new ChartDateModel
                                    {
                                        Date = p.SaveTime,
                                        Value = p.CurrentB ?? 0.0f
                                    }).ToList();

                                    SeriesItems.Add(new ChartModel()
                                    {
                                        Name = $"{data.DisplayName}(current_b)",
                                        Datas = dataList
                                    });
                                }
                                break;
                            case "currentc":
                                {
                                    var dataList = chartDatas.Where(p => p.Ceqid == data.CeqId).Select(p => new ChartDateModel
                                    {
                                        Date = p.SaveTime,
                                        Value = p.CurrentC ?? 0.0f
                                    }).ToList();

                                    SeriesItems.Add(new ChartModel()
                                    {
                                        Name = $"{data.DisplayName}(current_c)",
                                        Datas = dataList
                                    });
                                }
                                break;
                            case "currentn":
                                {
                                    var dataList = chartDatas.Where(p => p.Ceqid == data.CeqId).Select(p => new ChartDateModel
                                    {
                                        Date = p.SaveTime,
                                        Value = p.CurrentN ?? 0.0f
                                    }).ToList();

                                    SeriesItems.Add(new ChartModel()
                                    {
                                        Name = $"{data.DisplayName}(current_n)",
                                        Datas = dataList
                                    });
                                }
                                break;
                        }
                    }
                }
            }
        }

        private List<string> GetColumnsNames<T>() where T : class, new()
        {
            var lst = new List<string>();
            var lstColumns = new T().GetType().GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).ToList();

            foreach (var column in lstColumns)
            {
                var name = column.Name.ToLower();
                if(name == "savetime" || name == "commtime" || name == "ceqid" || name == "cpsid" || name == "dl" || name == "circuitno" 
                    || name == "name" || name == "dl" || name == "diagnostics" || name == "voltageunbalance" || name == "currentunbalance" 
                    || name == "frequency")
                {
                    continue;
                }
                lst.Add(name);
            }
            return lst;
        }

        [RelayCommand]
        private void FileSave()
        {
            string folder = AppDomain.CurrentDomain.BaseDirectory + "save";
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.InitialDirectory = folder;
            //saveDialog.FileName = 
            saveDialog.Filter = "Excel files (*.csv)|*.csv|All files (*.*)|*.*";
            saveDialog.FilterIndex = 1;

            try
            {
                if (saveDialog.ShowDialog() == true)
                {
                    string header = string.Empty;
                    string writeData = string.Empty;
                    using (StreamWriter outputFile = new StreamWriter(new FileStream(saveDialog.FileName, FileMode.Create, FileAccess.ReadWrite), Encoding.UTF8))
                    {
                        if (RealDataCheck)
                        {
                            // 실시간 데이터
                            if (SwitchCheck)
                            {
                                var dataList = (List<HistoryMinDatum>)_worker.GetPointItems((int)SearchTypeCode.MINDATA);
                                if (dataList == null || dataList?.Count <= 0)
                                {
                                    outputFile.Close();
                                    File.Delete(saveDialog.FileName);
                                    MessageBox.Show($"데이터가 없습니다.", "파일 저장", MessageBoxButton.OK, MessageBoxImage.Information);
                                    return;
                                }

                                header = "D/L 이름, 단말장치명, CEQ ID, 단자번호, 단말장치 상태, 전압 불평형률, 전류 불평형률, 주파수"
                                    + ", 전류A, 전류B, 전류C, 전류N, 전압A, 전압B, 전압C, 피상전력A, 피상전력B, 피상전력C, 역률3상, 역률A, 역률B, 역률C"
                                    + ", 고장전류A, 고장전류B, 고장전류C, 고장전류N, 전류위상A, 전류위상B, 전류위상C, 전류위상N, 전압위상A, 전압위상B, 전압위상C, 정보수집시간, DB기록시간";
                                outputFile.WriteLine(header);
                                foreach (var data in dataList)
                                {
                                    writeData = $"{data.Dl}, {data.Name}, {data.Ceqid}, {data.Circuitno}, {data.Diagnostics}, {data.VoltageUnbalance}, {data.CurrentUnbalance}, {data.Frequency}"
                                        + $", {data.CurrentA}, {data.CurrentB}, {data.CurrentC}, {data.CurrentN}, {data.VoltageA}, {data.VoltageB}, {data.VoltageC}, {data.ApparentPowerA}, {data.ApparentPowerB}, {data.ApparentPowerC}, {data.PowerFactor3p}, {data.PowerFactorA}, {data.PowerFactorB}, {data.PowerFactorC}"
                                        + $", {data.FaultCurrentA}, {data.FaultCurrentB}, {data.FaultCurrentC}, {data.FaultCurrentN}, {data.CurrentPhaseA}, {data.CurrentPhaseB}, {data.CurrentPhaseC}, {data.CurrentPhaseN}, {data.VoltagePhaseA}, {data.VoltagePhaseB}, {data.VoltagePhaseC}, {data.CommTime?.ToString("yyyy-MM-dd HH:mm:ss")}, {data.SaveTime.ToString("yyyy-MM-dd HH:mm:ss")}";
                                    outputFile.WriteLine(writeData);
                                }
                            }
                            else if (FiCheck)
                            {
                                var dataList = (List<HistoryFiAlarm>)_worker.GetPointItems((int)SearchTypeCode.FIALARM);
                                if (dataList == null || dataList?.Count <= 0)
                                {
                                    outputFile.Close();
                                    File.Delete(saveDialog.FileName);
                                    MessageBox.Show($"데이터가 없습니다.", "파일 저장", MessageBoxButton.OK, MessageBoxImage.Information);
                                    return;
                                }

                                header = "D/L 이름, 단말장치명, CEQ ID, 단자번호, 알람 이름, 알람 값, 알람 내용, 고장전류A, 고장전류B, 고장전류C, 고장전류N, 서버기록시간, 단말발생시간, DB기록시간";
                                outputFile.WriteLine(header);
                                foreach (var data in dataList)
                                {
                                    writeData = $"{data.Dl}, {data.Name}, {data.Ceqid}, {data.Circuitno}, {data.AlarmName}, {data.Value}, {data.LogDesc}, {data.FaultCurrentA}, {data.FaultCurrentB}, {data.FaultCurrentC}, {data.FaultCurrentN}, {data.LogTime?.ToString("yyyy-MM-dd HH:mm:ss")}, {data.FrtuTime?.ToString("yyyy-MM-dd HH:mm:ss")}, {data.SaveTime.ToString("yyyy-MM-dd HH:mm:ss")}";
                                    outputFile.WriteLine(writeData);
                                }
                            }
                        }
                        else if (StatisticalCheck)
                        {
                            if (DayCheck)
                            {
                                var dataList = (List<HistoryDaystatDatum>)_worker.GetPointItems((int)SearchTypeCode.DAYSTATDATA);
                                if (dataList == null || dataList?.Count <= 0)
                                {
                                    outputFile.Close();
                                    File.Delete(saveDialog.FileName);
                                    MessageBox.Show($"데이터가 없습니다.", "파일 저장", MessageBoxButton.OK, MessageBoxImage.Information);
                                    return;
                                }

                                header = "D/L 이름, 단말장치명, CEQ ID, 단자번호, 단말장치 상태, 전압 불평형률, 전류 불평형률, 주파수"
                                    + ", 전류A(평균), 전류B(평균), 전류C(평균), 전류N(평균), 전류A(최대), 전류B(최대), 전류C(최대), 전류N(최대), 최대 수집시간"
                                    + ", 전류A(최소), 전류B(최소), 전류C(최소), 전류N(최소), 최소 수집시간, 정보수집시간, DB기록시간";
                                outputFile.WriteLine(header);
                                foreach (var data in dataList)
                                {
                                    writeData = $"{data.Dl}, {data.Name}, {data.Ceqid}, {data.Circuitno}, {data.Diagnostics}, {data.VoltageUnbalance}, {data.CurrentUnbalance}, {data.Frequency}"
                                         + $", {data.AverageCurrentA}, {data.AverageCurrentB}, {data.AverageCurrentC}, {data.AverageCurrentN}, {data.MaxCurrentA}, {data.MaxCurrentB}, {data.MaxCurrentC}, {data.MaxCurrentN}, {data.MaxCommTime?.ToString("yyyy-MM-dd HH:mm:ss")}"
                                         + $", {data.MinCurrentA}, {data.MinCurrentB}, {data.MinCurrentC}, {data.MinCurrentN}, {data.MinCommTime?.ToString("yyyy-MM-dd HH:mm:ss")}"
                                         + $", {data.CommTime?.ToString("yyyy-MM-dd HH:mm:ss")}, {data.SaveTime.ToString("yyyy-MM-dd HH:mm:ss")}";
                                    outputFile.WriteLine(writeData);
                                }
                            }
                            else
                            {
                                // 통계 데이터
                                if (StatisticMinCheck)
                                {
                                    var dataList = (List<Statistics15min>)_worker.GetPointItems((int)SearchTypeCode.STATISTICSMIN);
                                    if (dataList == null || dataList?.Count <= 0)
                                    {
                                        outputFile.Close();
                                        File.Delete(saveDialog.FileName);
                                        MessageBox.Show($"데이터가 없습니다.", "파일 저장", MessageBoxButton.OK, MessageBoxImage.Information);
                                        return;
                                    }

                                    header = "D/L 이름, 단말장치명, CEQ ID, 단자번호, 단말장치명,  전류A(평균), 전류B(평균), 전류C(평균), 전류N(평균), 정보수집시간, DB기록시간";
                                    outputFile.WriteLine(header);
                                    foreach (var data in dataList)
                                    {
                                        writeData = $"{data.Dl}, {data.Name}, {data.Ceqid}, {data.AverageCurrentA}, {data.AverageCurrentB}, {data.AverageCurrentC}, {data.AverageCurrentN}"
                                            + $", {data.SaveTime.ToString("yyyy-MM-dd HH:mm:ss")}, {data.SaveTime.ToString("yyyy-MM-dd HH:mm:ss")}";
                                        outputFile.WriteLine(writeData);
                                    }
                                }
                                else if (StatisticHourCheck)
                                {
                                    var dataList = (List<StatisticsHour>)_worker.GetPointItems((int)SearchTypeCode.STATISTICSHOUR);
                                    if (dataList == null || dataList?.Count <= 0)
                                    {
                                        outputFile.Close();
                                        File.Delete(saveDialog.FileName);
                                        MessageBox.Show($"데이터가 없습니다.", "파일 저장", MessageBoxButton.OK, MessageBoxImage.Information);
                                        return;
                                    }

                                    header = "D/L 이름, 단말장치명, CEQ ID, 단자번호, "
                                        + ", 전류A(평균), 전류B(평균), 전류C(평균), 전류N(평균), 전류A(최대), 전류B(최대), 전류C(최대), 전류N(최대), 최대 수집시간"
                                        + ", 전류A(최소), 전류B(최소), 전류C(최소), 전류N(최소), 최소 수집시간, DB 기록시간";
                                    outputFile.WriteLine(header);
                                    foreach (var data in dataList)
                                    {
                                        writeData = $"{data.Dl}, {data.Name}, {data.Ceqid}, {data.AverageCurrentA}, {data.AverageCurrentB}, {data.AverageCurrentC}, {data.AverageCurrentN}"
                                            + $", {data.MaxCurrentA}, {data.MaxCurrentB}, {data.MaxCurrentC}, {data.MaxCurrentN}, {data.MaxCommTime?.ToString("yyyy-MM-dd HH:mm:ss")}"
                                            + $", {data.MinCurrentA}, {data.MinCurrentB}, {data.MinCurrentC}, {data.MinCurrentN}, {data.MinCommTime?.ToString("yyyy-MM-dd HH:mm:ss")}"
                                            + $", {data.SaveTime.ToString("yyyy-MM-dd HH:mm:ss")}";
                                        outputFile.WriteLine(writeData);
                                    }
                                }
                                else if (StatisticDayCheck)
                                {
                                    var dataList = (List<StatisticsDay>)_worker.GetPointItems((int)SearchTypeCode.STATISTICSDAY);
                                    if (dataList == null || dataList?.Count <= 0)
                                    {
                                        outputFile.Close();
                                        File.Delete(saveDialog.FileName);
                                        MessageBox.Show($"데이터가 없습니다.", "파일 저장", MessageBoxButton.OK, MessageBoxImage.Information);
                                        return;
                                    }

                                    header = "D/L 이름, 단말장치명, CEQ ID, 단자번호, "
                                        + ", 전류A(평균), 전류B(평균), 전류C(평균), 전류N(평균), 전류A(최대), 전류B(최대), 전류C(최대), 전류N(최대), 최대 수집시간"
                                        + ", 전류A(최소), 전류B(최소), 전류C(최소), 전류N(최소), 최소 수집시간, DB 기록시간";
                                    outputFile.WriteLine(header);
                                    foreach (var data in dataList)
                                    {
                                        writeData = $"{data.Dl}, {data.Name}, {data.Ceqid}, {data.AverageCurrentA}, {data.AverageCurrentB}, {data.AverageCurrentC}, {data.AverageCurrentN}"
                                            + $", {data.MaxCurrentA}, {data.MaxCurrentB}, {data.MaxCurrentC}, {data.MaxCurrentN}, {data.MaxCommTime?.ToString("yyyy-MM-dd HH:mm:ss")}"
                                            + $", {data.MinCurrentA}, {data.MinCurrentB}, {data.MinCurrentC}, {data.MinCurrentN}, {data.MinCommTime?.ToString("yyyy-MM-dd HH:mm:ss")}"
                                            + $", {data.SaveTime.ToString("yyyy-MM-dd HH:mm:ss")}";
                                        outputFile.WriteLine(writeData);
                                    }
                                }
                                else if (StatisticMonthCheck)
                                {
                                    var dataList = (List<StatisticsMonth>)_worker.GetPointItems((int)SearchTypeCode.STATISTICSMONTH);
                                    if (dataList == null || dataList?.Count <= 0)
                                    {
                                        outputFile.Close();
                                        File.Delete(saveDialog.FileName);
                                        MessageBox.Show($"데이터가 없습니다.", "파일 저장", MessageBoxButton.OK, MessageBoxImage.Information);
                                        return;
                                    }

                                    header = "D/L 이름, 단말장치명, CEQ ID, 단자번호, "
                                        + ", 전류A(평균), 전류B(평균), 전류C(평균), 전류N(평균), 전류A(최대), 전류B(최대), 전류C(최대), 전류N(최대), 최대 수집시간"
                                        + ", 전류A(최소), 전류B(최소), 전류C(최소), 전류N(최소), 최소 수집시간, DB 기록시간";
                                    outputFile.WriteLine(header);
                                    foreach (var data in dataList)
                                    {
                                        writeData = $"{data.Dl}, {data.Name}, {data.Ceqid}, {data.AverageCurrentA}, {data.AverageCurrentB}, {data.AverageCurrentC}, {data.AverageCurrentN}"
                                            + $", {data.MaxCurrentA}, {data.MaxCurrentB}, {data.MaxCurrentC}, {data.MaxCurrentN}, {data.MaxCommTime?.ToString("yyyy-MM-dd HH:mm:ss")}"
                                            + $", {data.MinCurrentA}, {data.MinCurrentB}, {data.MinCurrentC}, {data.MinCurrentN}, {data.MinCommTime?.ToString("yyyy-MM-dd HH:mm:ss")}"
                                            + $", {data.SaveTime.ToString("yyyy-MM-dd HH:mm:ss")}";
                                        outputFile.WriteLine(writeData);
                                    }
                                }
                                else if (StatisticYearCheck)
                                {
                                    var dataList = (List<StatisticsYear>)_worker.GetPointItems((int)SearchTypeCode.STATISTICSYEAR);
                                    if (dataList == null || dataList?.Count <= 0)
                                    {
                                        outputFile.Close();
                                        File.Delete(saveDialog.FileName);
                                        MessageBox.Show($"데이터가 없습니다.", "파일 저장", MessageBoxButton.OK, MessageBoxImage.Information);
                                        return;
                                    }

                                    header = "D/L 이름, 단말장치명, CEQ ID, 단자번호, "
                                        + ", 전류A(평균), 전류B(평균), 전류C(평균), 전류N(평균), 전류A(최대), 전류B(최대), 전류C(최대), 전류N(최대), 최대 수집시간"
                                        + ", 전류A(최소), 전류B(최소), 전류C(최소), 전류N(최소), 최소 수집시간, DB 기록시간";
                                    outputFile.WriteLine(header);
                                    foreach (var data in dataList)
                                    {
                                        writeData = $"{data.Dl}, {data.Name}, {data.Ceqid}, {data.AverageCurrentA}, {data.AverageCurrentB}, {data.AverageCurrentC}, {data.AverageCurrentN}"
                                            + $", {data.MaxCurrentA}, {data.MaxCurrentB}, {data.MaxCurrentC}, {data.MaxCurrentN}, {data.MaxCommTime?.ToString("yyyy-MM-dd HH:mm:ss")}"
                                            + $", {data.MinCurrentA}, {data.MinCurrentB}, {data.MinCurrentC}, {data.MinCurrentN}, {data.MinCommTime?.ToString("yyyy-MM-dd HH:mm:ss")}"
                                            + $", {data.SaveTime.ToString("yyyy-MM-dd HH:mm:ss")}";
                                        outputFile.WriteLine(writeData);
                                    }
                                }
                            }
                        }
                        else if (CommCheck)
                        {
                            if (CommDayCheck)
                            {
                                var dataList = (List<HistoryCommState>)_worker.GetPointItems((int)SearchTypeCode.COMMSTATE);
                                if (dataList == null || dataList?.Count <= 0)
                                {
                                    outputFile.Close();
                                    File.Delete(saveDialog.FileName);
                                    MessageBox.Show($"데이터가 없습니다.", "파일 저장", MessageBoxButton.OK, MessageBoxImage.Information);
                                    return;
                                }

                                header = "D/L 이름, 단말장치명, 전체횟수, 성공횟수, 실패횟수, 통신 성공률, 정보수집시간, DB기록시간";
                                outputFile.WriteLine(header);
                                foreach (var data in dataList)
                                {
                                    writeData = $"{data.Dl}, {data.Name}, {data.CommTotalCount}, {data.CommSucessCount}, {data.CommFailCount}, {data.CommSucessRate}"
                                            + $", {data.CommTime?.ToString("yyyy-MM-dd HH:mm:ss")}, {data.SaveTime.ToString("yyyy-MM-dd HH:mm:ss")}";
                                    outputFile.WriteLine(writeData);
                                }
                            }
                            else if (CommLogCheck)
                            {
                                var dataList = (List<HistoryCommStateLog>)_worker.GetPointItems((int)SearchTypeCode.COMMSTATELOG);
                                if (dataList == null || dataList?.Count <= 0)
                                {
                                    outputFile.Close();
                                    File.Delete(saveDialog.FileName);
                                    MessageBox.Show($"데이터가 없습니다.", "파일 저장", MessageBoxButton.OK, MessageBoxImage.Information);
                                    return;
                                }

                                header = "D/L 이름, 단말장치명, 성공여부, 전체횟수, 성공횟수, 실패횟수, 통신 성공률, 정보수집시간, DB기록시간";
                                outputFile.WriteLine(header);
                                foreach (var data in dataList)
                                {
                                    writeData = $"{data.Dl}, {data.Name}, {data.CommState} {data.CommTotalCount}, {data.CommSucessCount}, {data.CommFailCount}, {data.CommSucessRate}"
                                            + $", {data.CommTime?.ToString("yyyy-MM-dd HH:mm:ss")}, {data.SaveTime.ToString("yyyy-MM-dd HH:mm:ss")}";
                                    outputFile.WriteLine(writeData);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"파일:{saveDialog.SafeFileName} 저장중 예외 발생 ex:{ex.Message}", "데이터 조회", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void OnRealDataChecked(object sender, RoutedEventArgs e)
        {
            StatisticalCheck = false;
            CommCheck = false;
            StatisticalVisibility = Visibility.Hidden;
            RealVisibility = Visibility.Visible;
            CommVisibility = Visibility.Hidden;
            SwitchVisibility = Visibility.Visible;
            //DayCheck = false;
            //ElectricMinCheck = false;
            //ElectricHourCheck = false;
            //ElectricDayCheck = false;
            //ElectricMonthCheck = false;
            //ElectricYearCheck = false;
            SwitchCheck = true;
            FiCheck = false;
            ChildViewModel = (INotifyPropertyChanged)App.Current.Services.GetService<ViewModel_SwitchData>()!;
        }

        public void OnStatisticalChecked(object sender, RoutedEventArgs e)
        {
            RealDataCheck = false;
            CommCheck = false;
            RealVisibility = Visibility.Collapsed;
            StatisticalVisibility = Visibility.Visible;
            CommVisibility = Visibility.Hidden;
            SwitchVisibility = Visibility.Collapsed;
            //SwitchCheck = false;
            //FiCheck = false;
            DayCheck = true;
            StatisticMinCheck = false;
            StatisticHourCheck = false;
            StatisticDayCheck = false;
            StatisticMonthCheck = false;
            StatisticYearCheck = false;

            ChildViewModel = (INotifyPropertyChanged)App.Current.Services.GetService<ViewModel_DayStatData>()!;
        }

        public void OnCommChecked(object sender, RoutedEventArgs e)
        {
            RealDataCheck = false;
            StatisticalCheck = false;
            RealVisibility = Visibility.Collapsed;
            StatisticalVisibility = Visibility.Hidden;
            CommVisibility = Visibility.Visible;
            SwitchVisibility = Visibility.Collapsed;

            CommDayCheck = true;
            CommLogCheck = false;

            ChildViewModel = (INotifyPropertyChanged)App.Current.Services.GetService<ViewModel_CommDayData>()!;
        }

        public void OnSwitchChecked(object sender, RoutedEventArgs e)
        {
            SwitchCheck = true;
            FiCheck = false;
            SwitchVisibility = Visibility.Visible;

            ChildViewModel = (INotifyPropertyChanged)App.Current.Services.GetService<ViewModel_SwitchData>()!;
        }

        public void OnFiChecked(object sender, RoutedEventArgs e)
        {
            SwitchCheck = false;
            FiCheck = true;
            SwitchVisibility = Visibility.Collapsed;

            ChildViewModel = (INotifyPropertyChanged)App.Current.Services.GetService<ViewModel_FiAlarmData>()!;
        }

        public void OnDayStatChecked(object sender, RoutedEventArgs e)
        {
            DayCheck = true;
            StatisticMinCheck = false;
            StatisticHourCheck = false;
            StatisticDayCheck = false;
            StatisticMonthCheck = false;
            StatisticYearCheck = false;

            ChildViewModel = (INotifyPropertyChanged)App.Current.Services.GetService<ViewModel_DayStatData>()!;
            SwitchVisibility = Visibility.Collapsed;
        }

        public void OnMinChecked(object sender, RoutedEventArgs e)
        {
            DayCheck = false;
            StatisticMinCheck = true;
            StatisticHourCheck = false;
            StatisticDayCheck = false;
            StatisticMonthCheck = false;
            StatisticYearCheck = false;

            ChildViewModel = (INotifyPropertyChanged)App.Current.Services.GetService<ViewModel_StatisticsMinData>()!;
            SwitchVisibility = Visibility.Collapsed;
        }

        public void OnHourChecked(object sender, RoutedEventArgs e)
        {
            DayCheck = false;
            StatisticMinCheck = false;
            StatisticHourCheck = true;
            StatisticDayCheck = false;
            StatisticMonthCheck = false;
            StatisticYearCheck = false;

            ChildViewModel = (INotifyPropertyChanged)App.Current.Services.GetService<ViewModel_StatisticsHourData>()!;
            SwitchVisibility = Visibility.Collapsed;
        }

        public void OnDayChecked(object sender, RoutedEventArgs e)
        {
            DayCheck = false;
            StatisticMinCheck = false;
            StatisticHourCheck = false;
            StatisticDayCheck = true;
            StatisticMonthCheck = false;
            StatisticYearCheck = false;

            ChildViewModel = (INotifyPropertyChanged)App.Current.Services.GetService<ViewModel_StatisticsDayData>()!;
            SwitchVisibility = Visibility.Collapsed;
        }

        public void OnMonthChecked(object sender, RoutedEventArgs e)
        {
            DayCheck = false;
            StatisticMinCheck = false;
            StatisticHourCheck = false;
            StatisticDayCheck = false;
            StatisticMonthCheck = true;
            StatisticYearCheck = false;

            ChildViewModel = (INotifyPropertyChanged)App.Current.Services.GetService<ViewModel_StatisticsMonthData>()!;
            SwitchVisibility = Visibility.Collapsed;
        }

        public void OnYearChecked(object sender, RoutedEventArgs e)
        {
            DayCheck = false;
            StatisticMinCheck = false;
            StatisticHourCheck = false;
            StatisticDayCheck = false;
            StatisticMonthCheck = false;
            StatisticYearCheck = true;

            ChildViewModel = (INotifyPropertyChanged)App.Current.Services.GetService<ViewModel_StatisticsYearData>()!;
            SwitchVisibility = Visibility.Collapsed;
        }

        public void OnCommDayChecked(object sender, RoutedEventArgs e)
        {
            CommDayCheck = true;
            CommLogCheck = false;

            ChildViewModel = (INotifyPropertyChanged)App.Current.Services.GetService<ViewModel_CommDayData>()!;
            SwitchVisibility = Visibility.Collapsed;
        }

        public void OnCommLogChecked(object sender, RoutedEventArgs e)
        {
            CommDayCheck = false;
            CommLogCheck = true;

            ChildViewModel = (INotifyPropertyChanged)App.Current.Services.GetService<ViewModel_CommLogData>()!;
            SwitchVisibility = Visibility.Collapsed;
        }

        public void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            CheckBox? check = sender as CheckBox;
            if (check != null)
            {
                if (check.IsChecked == true)
                {
                    check.IsChecked = true;
                    check.Focus();
                }
            }
        }

        //public void OnUnChecked(object sender, RoutedEventArgs e)
        //{
        //    CheckBox? check = sender as CheckBox;
        //    if(check != null) 
        //    {
        //        if (check.IsChecked == false)
        //            check.IsChecked = true;
        //    }
        //}

        //public void OnEditValueChanged(object sender, DevExpress.Xpf.Editors.EditValueChangedEventArgs e)
        //{
        //    DateEdit? edit = sender as DateEdit;
        //    if (edit != null)
        //    {
        //        if (Convert.ToDateTime(Convert.ToDateTime(edit.EditValue).ToString("yyyy-MM-dd 23:59:59")) > ToDate)
        //        {
        //            var nowDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd 00:00:00"));

        //            edit.EditValue = nowDate;
        //            edit.DateTime = nowDate;
        //            edit.SelectedText = nowDate.ToString("yyyy-MM-dd 00:00:00");
        //            edit.Text = nowDate.ToString("yyyy-MM-dd 00:00:00");
        //            edit.Focus();

        //            FromDate = nowDate;
        //            MessageBox.Show($"시작날짜가 종료날짜보다 클 수 없습니다.", "날짜 검사", MessageBoxButton.OK, MessageBoxImage.Information);
        //            return;
        //        }
        //        else if (Convert.ToDateTime(Convert.ToDateTime(edit.EditValue).ToString("yyyy-MM-dd 23:59:59")) < FromDate)
        //        {
        //            var nowDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd 23:59:59"));

        //            edit.EditValue = nowDate;
        //            edit.DateTime = nowDate;
        //            edit.SelectedText = nowDate.ToString("yyyy-MM-dd 23:59:59");
        //            edit.Text = nowDate.ToString("yyyy-MM-dd 23:59:59");
        //            edit.Focus();

        //            ToDate = nowDate;
        //            MessageBox.Show($"종료날짜가 시작날짜보다 작을 수 없습니다.", "날짜 검사", MessageBoxButton.OK, MessageBoxImage.Information);
        //            return;
        //        }
        //    }
        //}

    }
}
