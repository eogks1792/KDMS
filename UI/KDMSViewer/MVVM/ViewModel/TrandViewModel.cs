using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DevExpress.Mvvm.Native;
using KDMSViewer.Model;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using KDMSViewer.View;
using System.IO;
using Microsoft.Win32;
using DevExpress.XtraPrinting;
using System.Drawing.Imaging;
using System.Windows.Media;

namespace KDMSViewer.ViewModel
{
    public partial class TrandViewModel : ObservableObject
    {
        private readonly DataWorker _worker;
        [ObservableProperty]
        private ObservableCollection<TreeDataModel> _treeItems;

        [ObservableProperty]
        private TreeDataModel _treeSelected;

        [ObservableProperty]
        private int _itemCount;

        [ObservableProperty]
        private List<ChartPointDataModel> _pointItems;

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
        private bool _minDataCheck = true;
        [ObservableProperty]
        private bool _dayStatCheck = false;
        [ObservableProperty]
        private bool _statisticsMinCheck = false;
        [ObservableProperty]
        private bool _statisticsHourCheck = false;
        [ObservableProperty]
        private bool _statisticsDayCheck = false;
        [ObservableProperty]
        private bool _statisticsMonthCheck = false;
        [ObservableProperty]
        private bool _statisticsYearCheck = false;

        [ObservableProperty]
        private Visibility _realVisibility = Visibility.Visible;

        [ObservableProperty]
        private Visibility _statisticalVisibility = Visibility.Collapsed;

        [ObservableProperty]
        private DateTime _fromDate;

        [ObservableProperty]
        private DateTime _toDate;

        [ObservableProperty]
        public string _timeEditMask = CultureInfo.CurrentCulture.DateTimeFormat.LongTimePattern;

        [ObservableProperty]
        private bool _isInquiry = true;

        public TrandViewModel(DataWorker worker)
        {
            _worker = worker;
            DataInit();
        }
        private void DataInit()
        {
            FromDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:00:00"));
            ToDate = Convert.ToDateTime(DateTime.Now.AddHours(1).ToString("yyyy-MM-dd HH:00:00"));

            TreeItems = new ObservableCollection<TreeDataModel>(_worker.TreeDatas);
        }

        private List<ChartPointDataModel> CheckItemList()
        {
            //var chartDatas = PointItems.Where(p => p.SaveTime > FromDate && p.SaveTime < ToDate).ToList();
            List<ChartPointDataModel> retList = new List<ChartPointDataModel>();

            string name = string.Empty;
            string check = string.Empty;
            // 체크된 항목된 가져오도록 처리

            if (MinDataCheck)
            {
                name = "CURRENT";
                if (CurrentA)
                {
                    check = $"{name}_A";
                    retList.AddRange(PointItems.Where(p => p.PointName.Contains(check)).ToList());

                }
                if (CurrentB)
                {
                    check = $"{name}_B";
                    retList.AddRange(PointItems.Where(p => p.PointName.Contains(check)).ToList());
                }
                if (CurrentC)
                {
                    check = $"{name}_C";
                    retList.AddRange(PointItems.Where(p => p.PointName.Contains(check)).ToList());
                }
                if (CurrentN)
                {
                    check = $"{name}_N";
                    retList.AddRange(PointItems.Where(p => p.PointName.Contains(check)).ToList());
                }

                name = "VOLTAGE";
                if (VoltageA)
                {
                    check = $"{name}_A";
                    retList.AddRange(PointItems.Where(p => p.PointName.Contains(check)).ToList());
                }
                if (VoltageB)
                {
                    check = $"{name}_B";
                    retList.AddRange(PointItems.Where(p => p.PointName.Contains(check)).ToList());
                }
                if (VoltageC)
                {
                    check = $"{name}_C";
                    retList.AddRange(PointItems.Where(p => p.PointName.Contains(check)).ToList());
                }
            }
            else
            {
                if (StatisticsMinCheck)
                {
                    name = "AVERAGE_CURRENT";
                    if (CurrentA)
                    {
                        check = $"{name}_A";
                        retList.AddRange(PointItems.Where(p => p.PointName.Contains(check)).ToList());
                    }
                    if (CurrentB)
                    {
                        check = $"{name}_B";
                        retList.AddRange(PointItems.Where(p => p.PointName.Contains(check)).ToList());
                    }
                    if (CurrentC)
                    {
                        check = $"{name}_C";
                        retList.AddRange(PointItems.Where(p => p.PointName.Contains(check)).ToList());
                    }
                    if (CurrentN)
                    {
                        check = $"{name}_N";
                        retList.AddRange(PointItems.Where(p => p.PointName.Contains(check)).ToList());
                    }
                }
                else
                {
                    if (Average)
                    {
                        name = "AVERAGE_CURRENT";
                        if (CurrentA)
                        {
                            check = $"{name}_A";
                            retList.AddRange(PointItems.Where(p => p.PointName.Contains(check)).ToList());
                        }
                        if (CurrentB)
                        {
                            check = $"{name}_B";
                            retList.AddRange(PointItems.Where(p => p.PointName.Contains(check)).ToList());
                        }
                        if (CurrentC)
                        {
                            check = $"{name}_C";
                            retList.AddRange(PointItems.Where(p => p.PointName.Contains(check)).ToList());
                        }
                        if (CurrentN)
                        {
                            check = $"{name}_N";
                            retList.AddRange(PointItems.Where(p => p.PointName.Contains(check)).ToList());
                        }
                    }

                    if (Max)
                    {
                        name = "MAX_CURRENT";
                        if (CurrentA)
                        {
                            check = $"{name}_A";
                            retList.AddRange(PointItems.Where(p => p.PointName.Contains(check)).ToList());
                        }
                        if (CurrentB)
                        {
                            check = $"{name}_B";
                            retList.AddRange(PointItems.Where(p => p.PointName.Contains(check)).ToList());
                        }
                        if (CurrentC)
                        {
                            check = $"{name}_C";
                            retList.AddRange(PointItems.Where(p => p.PointName.Contains(check)).ToList());
                        }
                        if (CurrentN)
                        {
                            check = $"{name}_N";
                            retList.AddRange(PointItems.Where(p => p.PointName.Contains(check)).ToList());
                        }
                    }

                    if (Min)
                    {
                        name = "MIN_CURRENT";
                        if (CurrentA)
                        {
                            check = $"{name}_A";
                            retList.AddRange(PointItems.Where(p => p.PointName.Contains(check)).ToList());
                        }
                        if (CurrentB)
                        {
                            check = $"{name}_B";
                            retList.AddRange(PointItems.Where(p => p.PointName.Contains(check)).ToList());
                        }
                        if (CurrentC)
                        {
                            check = $"{name}_C";
                            retList.AddRange(PointItems.Where(p => p.PointName.Contains(check)).ToList());
                        }
                        if (CurrentN)
                        {
                            check = $"{name}_N";
                            retList.AddRange(PointItems.Where(p => p.PointName.Contains(check)).ToList());
                        }
                    }
                }
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
        public void OnCurrentAChecked(object sender, RoutedEventArgs e)
        {
            ChartInit();
        }
        public void OnCurrentAUnChecked(object sender, RoutedEventArgs e)
        {
            ChartInit();
        }
        public void OnCurrentBChecked(object sender, RoutedEventArgs e)
        {
            ChartInit();
        }
        public void OnCurrentBUnChecked(object sender, RoutedEventArgs e)
        {
            ChartInit();
        }
        public void OnCurrentCChecked(object sender, RoutedEventArgs e)
        {
            ChartInit();
        }
        public void OnCurrentCUnChecked(object sender, RoutedEventArgs e)
        {
            ChartInit();
        }
        public void OnCurrentNChecked(object sender, RoutedEventArgs e)
        {
            ChartInit();
        }
        public void OnCurrentNUnChecked(object sender, RoutedEventArgs e)
        {
            ChartInit();
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

        public void OnVoltageAChecked(object sender, RoutedEventArgs e)
        {
            ChartInit();
        }
        public void OnVoltageAUnChecked(object sender, RoutedEventArgs e)
        {
            ChartInit();
        }
        public void OnVoltageBChecked(object sender, RoutedEventArgs e)
        {
            ChartInit();
        }
        public void OnVoltageBUnChecked(object sender, RoutedEventArgs e)
        {
            ChartInit();
        }
        public void OnVoltageCChecked(object sender, RoutedEventArgs e)
        {
            ChartInit();
        }
        public void OnVoltageCUnChecked(object sender, RoutedEventArgs e)
        {
            ChartInit();
        }

        public void OnTypeChecked(object sender, RoutedEventArgs e)
        {
            Average = true;
            Max = true;
            Min = true;
        }

        public void OnTypeUnChecked(object sender, RoutedEventArgs e)
        {
            Average = false;
            Max = false;
            Min = false;
        }

        public void OnAverageChecked(object sender, RoutedEventArgs e)
        {
            ChartInit();
        }

        public void OnAverageUnChecked(object sender, RoutedEventArgs e)
        {
            ChartInit();
        }

        public void OnMaxChecked(object sender, RoutedEventArgs e)
        {
            ChartInit();
        }

        public void OnMaxUnChecked(object sender, RoutedEventArgs e)
        {
            ChartInit();
        }

        public void OnMinChecked(object sender, RoutedEventArgs e)
        {
            ChartInit();
        }

        public void OnMinUnChecked(object sender, RoutedEventArgs e)
        {
            ChartInit();
        }

        private void ChartInit()
        {
            if (PointItems == null)
                return;

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

        private void GetData()
        {
            if (TreeSelected == null)
            {
                MessageBox.Show("선택된 데이터가 없습니다. \n\r개페기 및 다회로 스위치를 선택하세요.", "데이터 조회", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (TreeSelected.Type != TreeTypeCode.EQUIPMENT)
            {
                MessageBox.Show("선택된 데이터는 개페기 및 다회로 스위치가 아닙니다.", "데이터 조회", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            PointItems = new List<ChartPointDataModel>();
            var ceqList = new List<long>
            {
                TreeSelected.Id
            };

            if (MinDataCheck)
            {
                _worker.GetTrandData(ceqList, (int)SearchTypeCode.MINDATA, FromDate, ToDate);
            }
            else if (DayStatCheck)
            {
                _worker.GetTrandData(ceqList, (int)SearchTypeCode.DAYSTATDATA, FromDate, ToDate);
            }
            else if (StatisticsMinCheck)
            {
                _worker.GetTrandData(ceqList, (int)SearchTypeCode.STATISTICSMIN, FromDate, ToDate);
            }
            else if (StatisticsHourCheck)
            {
                _worker.GetTrandData(ceqList, (int)SearchTypeCode.STATISTICSHOUR, FromDate, ToDate);
            }
            else if (StatisticsDayCheck)
            {
                _worker.GetTrandData(ceqList, (int)SearchTypeCode.STATISTICSDAY, FromDate, ToDate);
            }
            else if (StatisticsMonthCheck)
            {
                _worker.GetTrandData(ceqList, (int)SearchTypeCode.STATISTICSMONTH, FromDate, ToDate);
            }
            else if (StatisticsYearCheck)
            {
                _worker.GetTrandData(ceqList, (int)SearchTypeCode.STATISTICSYEAR, FromDate, ToDate);
            }

            ItemCount = PointItems.Count;
            TypeCheck = true;
            TypeCheck = false;
            CurrentCheck = true;
            CurrentCheck = false;
            VoltageCheck = true;
            VoltageCheck = false;
            Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Arrow; });
            IsInquiry = true;

            //// 날짜 데이터 범위 
            //// 체크된 변전소, DL, 스위치 
            //// 데이터 가져와 화면에 표시
            //SeriesItems = new ObservableCollection<ChartModel>();
            //var chartDatas = CheckItemList();
            //if (chartDatas.Count > 0)
            //{
            //    var distintPoints = chartDatas.Select(p => new { Name = $"{p.CeqId}({p.PointName})", DisPlayName = $"{p.Name} ({p.PointName})" }).Distinct().ToList();
            //    foreach (var data in distintPoints)
            //    {
            //        var findDatas = chartDatas.Where(p => $"{p.CeqId}({p.PointName})" == data.Name).ToList();
            //        if (findDatas.Count > 0)
            //        {
            //            var dataList = findDatas.Select(p => new ChartDateModel
            //            {
            //                Date = p.UpdateTime,
            //                Value = p.PointValue
            //            }).ToList();

            //            SeriesItems.Add(new ChartModel()
            //            {
            //                Name = data.DisPlayName,
            //                Datas = dataList
            //            });
            //        }
            //    }
            //}
        }

        [RelayCommand]
        private async void Inquiry()
        {
            Mouse.OverrideCursor = Cursors.Wait;
            await Task.Run(() =>
            {
                IsInquiry = false;
                GetData();
            });
        }

        //[RelayCommand]
        //private void ImageSave()
        //{
        //    var folder = $"{AppDomain.CurrentDomain.BaseDirectory}Image";
        //    if (!Directory.Exists(folder))
        //        Directory.CreateDirectory(folder);

        //    SaveFileDialog saveDialog = new SaveFileDialog();
        //    saveDialog.InitialDirectory = folder;
        //    saveDialog.Filter = "Image files (*.png)|*.png|All files (*.*)|*.*";
        //    saveDialog.FilterIndex = 1;

        //    if (saveDialog.ShowDialog() == true)
        //    {
        //        var view = App.Current.Services.GetService<TrandView>()!;
        //        if (view != null)
        //        {
        //            var imageExportOptions = new ImageExportOptions
        //            {
        //                Format = ImageFormat.Png,
        //                ExportMode = ImageExportMode.SingleFile,
        //                PageBorderColor = System.Drawing.Color.White,
        //                PageBorderWidth = 0,
        //                TextRenderingMode = DevExpress.XtraPrinting.TextRenderingMode.AntiAliasGridFit,
        //                Resolution = 96
        //            };

        //            view.chart.ExportToImage(saveDialog.FileName, imageExportOptions, DevExpress.Xpf.Charts.PrintSizeMode.None);
        //        }
        //    }
        //}

        [RelayCommand]
        private void MinData()
        {
            CurrentCheck = true;
            CurrentCheck = false;
            VoltageCheck = true;
            VoltageCheck = false;
            PointItems = new List<ChartPointDataModel>();
            ItemCount = 0;

            StatisticalVisibility = Visibility.Collapsed;
            RealVisibility = Visibility.Visible;
        }

        [RelayCommand]
        private void DayStat()
        {
            TypeCheck = true;
            TypeCheck = false;
            CurrentCheck = true;
            CurrentCheck = false;
            PointItems = new List<ChartPointDataModel>();
            ItemCount = 0;

            StatisticalVisibility = Visibility.Visible;
            RealVisibility = Visibility.Collapsed;
        }

        [RelayCommand]
        private void StatisticsMin()
        {
            TypeCheck = true;
            TypeCheck = false;
            CurrentCheck = true;
            CurrentCheck = false;
            PointItems = new List<ChartPointDataModel>();
            ItemCount = 0;

            StatisticalVisibility = Visibility.Collapsed;
            RealVisibility = Visibility.Collapsed;
        }

        [RelayCommand]
        private void StatisticsHour()
        {
            TypeCheck = true;
            TypeCheck = false;
            CurrentCheck = true;
            CurrentCheck = false;
            PointItems = new List<ChartPointDataModel>();
            ItemCount = 0;

            StatisticalVisibility = Visibility.Visible;
            RealVisibility = Visibility.Collapsed;
        }

        [RelayCommand]
        private void StatisticsDay()
        {
            TypeCheck = true;
            TypeCheck = false;
            CurrentCheck = true;
            CurrentCheck = false;
            PointItems = new List<ChartPointDataModel>();
            ItemCount = 0;

            StatisticalVisibility = Visibility.Visible;
            RealVisibility = Visibility.Collapsed;
        }

        [RelayCommand]
        private void StatisticsMonth()
        {
            TypeCheck = true;
            TypeCheck = false;
            CurrentCheck = true;
            CurrentCheck = false;
            PointItems = new List<ChartPointDataModel>();
            ItemCount = 0;

            StatisticalVisibility = Visibility.Visible;
            RealVisibility = Visibility.Collapsed;
        }

        [RelayCommand]
        private void StatisticsYear()
        {
            TypeCheck = true;
            TypeCheck = false;
            CurrentCheck = true;
            CurrentCheck = false;
            PointItems = new List<ChartPointDataModel>();
            ItemCount = 0;

            StatisticalVisibility = Visibility.Visible;
            RealVisibility = Visibility.Collapsed;
        }

    }
}
