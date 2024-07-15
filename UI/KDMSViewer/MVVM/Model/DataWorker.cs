using DevExpress.ClipboardSource.SpreadsheetML;
using DevExpress.Xpf.Editors.Helpers;
using KDMS.EF.Core.Contexts;
using KDMS.EF.Core.Infrastructure.Reverse.Models;
using KDMSViewer.Features;
using KDMSViewer.View;
using KDMSViewer.ViewModel;
using MediatR;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using System.Xml.Linq;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace KDMSViewer.Model
{
    public class DataWorker
    {
        private readonly ILogger _logger;
        private readonly IMediator _mediator;
        private readonly IConfiguration _configuration;
        private readonly CommonDataModel _commonData;

        public List<TreeDataModel> TreeDatas;
        private List<long> CheckItems;
        private List<long> TrandCheckItems;
        //private List<TreeCheckItemModel> CheckItems;
        //private List<TreeCheckItemModel> TrandCheckItems;

        private bool ThreadFlag { get; set; } = true;
        public DataWorker(ILogger logger, IMediator mediator, IConfiguration configuration, CommonDataModel commonData) 
        {
            _logger = logger;
            _mediator = mediator;
            _configuration = configuration;
            _commonData = commonData;
        }

        public void Init()
        {
            TreeViewListInit();
            Task.Run(async () =>
            {
                await DataStatusBar();
            });
        }

        public void ThreadFilterMessage(ref MSG msg, ref bool handled)
        {
            if (msg.message == Win32API.message && msg.wParam != Win32API.handle)
            {
                MessageBox.Show("Message : " + msg.lParam.ToString());
            }
        }

        public void PostMessageSend(int value)
        {
            var retValue = Win32API.PostMessage((IntPtr)Win32API.HWND_BROADCAST, Win32API.message, (uint)Win32API.handle, (uint)value);
            if(retValue)
            {

            }
            else
            {
                MessageBox.Show("111111111111");
            }
        }

        private void TreeViewListInit()
        {
            // 트리 목록 생성 처리
            TreeDatas = _commonData.TreeListInit();
        }

        private void TreeItemCheckData()
        {
            CheckItems = new List<long>();
            var model = App.Current.Services.GetService<DataViewModel>()!;
            if (model == null)
                return;

            GetTreeItemCheckData(model.TreeItems, CheckItems);
        }

        //private void TrandTreeItemCheckData()
        //{
        //    TrandCheckItems = new List<TreeCheckItemModel>();
        //    var model = App.Current.Services.GetService<TrandViewModel>()!;
        //    if (model == null)
        //        return;

        //    GetTreeItemCheckData(model.TreeItems, TrandCheckItems);
        //}

        private void GetTreeItemCheckData(ObservableCollection<TreeDataModel> TreeItems, List<long> CheckItems)
        {
            foreach (var item in TreeItems)
            {
                if (item.IsChecked)
                {
                    if (item.Type == TreeTypeCode.EQUIPMENT)
                    { 
                        CheckItems.Add(item.Id);
                    }


                        //if (item.Type == TreeTypeCode.COMPOSITE)
                        //{
                        //    if (item.DataModels.Count > 0)
                        //    {
                        //        CheckItems.Add(new TreeCheckItemModel() { Type = item.Type, id = item.DlId, CpsID = item.Id, IsEnd = false });
                        //        GetTreeItemCheckData(item.DataModels, CheckItems);
                        //    }
                        //    else
                        //    {
                        //        CheckItems.Add(new TreeCheckItemModel() { Type = item.Type, DlID = item.DlId, CpsID = item.Id, IsEnd = true });
                        //    }
                        //}
                        //else if (item.Type == TreeTypeCode.EQUIPMENT)
                        //{
                        //    CheckItems.Add(new TreeCheckItemModel() { Type = item.Type, DlID = item.DlId, CpsID = item.SwId, CeqID = item.Id, IsEnd = true });
                        //}
                }
                else
                {
                    if (item.DataModels.Count > 0)
                        GetTreeItemCheckData(item.DataModels, CheckItems);
                }
            }
        }

        public async void GetTrandData(DateTime fromDate, DateTime toDate)
        {
            var model = App.Current.Services.GetService<TrandViewModel>()!;
            if (model == null)
                return;

            //TrandTreeItemCheckData();
            if (TrandCheckItems.Count <= 0)
            {
                Application.Current.Dispatcher.Invoke(() => { model.view.Close(); });
                MessageBox.Show($"체크된 개폐기가 없습니다.", "데이터 조회", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            GetSwitchData.Command request = new GetSwitchData.Command
            {
                CeqList = CheckItems,
                FromDate = fromDate,
                ToDate = toDate
            };

            var response = await _mediator.Send(request);
            if (response != null && response.Result)
            {
                model.PointItems = new ObservableCollection<HistoryMinDatum>();
                foreach (var data in response.datas)
                {
                    //foreach (var item in TrandCheckItems)
                    //{
                    //    if (item.Type == TreeTypeCode.COMPOSITE)
                    //    {
                    //        if (data.Cpsid == item.CpsID)
                    //        {
                    //            if (item.IsEnd)
                    //            {
                    //                Application.Current.Dispatcher.Invoke(() => { model.PointItems.Add(data); });
                    //            }
                    //        }
                    //    }
                    //    else
                    //    {
                    //        if (data.Cpsid == item.CpsID && data.Ceqid == item.CeqID)
                    //        {
                    //            if (item.IsEnd)
                    //            {
                    //                Application.Current.Dispatcher.Invoke(() => { model.PointItems.Add(data); });
                    //            }
                    //        }
                    //    }
                    //}
                }
            }
            else
            {
                Application.Current.Dispatcher.Invoke(() => { model.view.Close(); });
                MessageBox.Show($"HISTORY_MIN_DATA_{fromDate.ToString("yyMMdd")}~{fromDate.ToString("yyMMdd")} 테이블 \n\rCODE:{response.Error.Code} MSG:{response.Error.Message}", "데이터 조회", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        public async void GetSearchData(int type, DateTime fromDate, DateTime toDate)
        {
            var dataModel = App.Current.Services.GetService<DataViewModel>()!;
            if (dataModel == null)
                return;

            TreeItemCheckData();
            if (CheckItems.Count <= 0)
            {
                //Application.Current.Dispatcher.Invoke(() => { dataModel.view.Close(); });
                MessageBox.Show($"체크된 개폐기가 없습니다.", "데이터 조회", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            try
            {
                switch (type)
                {
                    case (int)SearchTypeCode.MINDATA:
                        {
                            GetSwitchData.Command request = new GetSwitchData.Command
                            {
                                CeqList = CheckItems,
                                FromDate = fromDate,
                                ToDate = toDate
                            };

                            var model = App.Current.Services.GetService<ViewModel_SwitchData>()!;
                            if (model != null)
                            {
                                //model.PointItems = new ObservableCollection<HistoryMinDatum>();
                                model.PointItems.Clear();

                                var response = await _mediator.Send(request);
                                if (response != null && response.Result)
                                {
                                    _logger.Information("데이터 표시 시작");
                                    //Application.Current.Dispatcher.Invoke(() => { model.PointItems = new ObservableCollection<HistoryMinDatum>(response.datas); });
                                    Application.Current.Dispatcher.Invoke(() => { model.PointItems = response.datas; });
                                    _logger.Information("데이터 표시 완료");

                                    //foreach (var data in response.datas)
                                    //{
                                    //    foreach (var item in CheckItems)
                                    //    {
                                    //        if (item.Type == TreeTypeCode.COMPOSITE)
                                    //        {
                                    //            if (data.Cpsid == item.CpsID)
                                    //            {
                                    //                if (item.IsEnd)
                                    //                {
                                    //                    Application.Current.Dispatcher.Invoke(() => { model.PointItems.Add(data); });
                                    //                }
                                    //            }
                                    //        }
                                    //        else
                                    //        {
                                    //            if (data.Cpsid == item.CpsID && data.Ceqid == item.CeqID)
                                    //            {
                                    //                if (item.IsEnd)
                                    //                {
                                    //                    Application.Current.Dispatcher.Invoke(() => { model.PointItems.Add(data); });
                                    //                }
                                    //            }
                                    //        }
                                    //    }
                                    //}
                                }
                                else
                                {
                                    //Application.Current.Dispatcher.Invoke(() => { dataModel.view.Close(); });
                                    MessageBox.Show($"HISTORY_MIN_DATA 테이블 \n\rCODE:{response.Error.Code} MSG:{response.Error.Message}", "데이터 조회", MessageBoxButton.OK, MessageBoxImage.Information);
                                }
                            }
                        }
                        break;
                    case (int)SearchTypeCode.DAYSTATDATA:
                        {
                            GetDayStatData.Command request = new GetDayStatData.Command
                            {
                                CeqList = CheckItems,
                                FromDate = fromDate,
                                ToDate = toDate
                            };

                            var model = App.Current.Services.GetService<ViewModel_DayStatData>()!;
                            if (model != null)
                            {
                                model.PointItems = new ObservableCollection<HistoryDaystatDatum>();
                                var response = await _mediator.Send(request);
                                if (response != null && response.Result)
                                {

                                    //foreach (var data in response.datas)
                                    //{
                                    //    foreach (var item in CheckItems)
                                    //    {
                                    //        if (item.Type == TreeTypeCode.COMPOSITE)
                                    //        {
                                    //            if (data.Cpsid == item.CpsID)
                                    //            {
                                    //                if (item.IsEnd)
                                    //                {
                                    //                    Application.Current.Dispatcher.Invoke(() => { model.PointItems.Add(data); });
                                    //                }
                                    //            }
                                    //        }
                                    //        else
                                    //        {
                                    //            if (data.Cpsid == item.CpsID && data.Ceqid == item.CeqID)
                                    //            {
                                    //                if (item.IsEnd)
                                    //                {
                                    //                    Application.Current.Dispatcher.Invoke(() => { model.PointItems.Add(data); });
                                    //                }
                                    //            }
                                    //        }
                                    //    }
                                    //}
                                }
                                else
                                {
                                    //Application.Current.Dispatcher.Invoke(() => { dataModel.view.Close(); });
                                    MessageBox.Show($"HISTORY_DAYSTAT_DATA 테이블 \n\rCODE:{response.Error.Code} MSG:{response.Error.Message}", "데이터 조회", MessageBoxButton.OK, MessageBoxImage.Information);
                                }
                            }
                        }
                        break;
                    case (int)SearchTypeCode.STATISTICSMIN:
                        {
                            GetStatisticsMinData.Command request = new GetStatisticsMinData.Command
                            {
                                CeqList = CheckItems,
                                FromDate = fromDate,
                                ToDate = toDate
                            };

                            var model = App.Current.Services.GetService<ViewModel_StatisticsMinData>()!;
                            if (model != null)
                            {
                                model.PointItems = new ObservableCollection<Statistics15min>();

                                var response = await _mediator.Send(request);
                                if (response != null && response.Result)
                                {
                                    //foreach (var data in response.datas)
                                    //{
                                    //    foreach (var item in CheckItems)
                                    //    {
                                    //        if (item.Type == TreeTypeCode.COMPOSITE)
                                    //        {
                                    //            if (data.Cpsid == item.CpsID)
                                    //            {
                                    //                if (item.IsEnd)
                                    //                {
                                    //                    Application.Current.Dispatcher.Invoke(() => { model.PointItems.Add(data); });
                                    //                }
                                    //            }
                                    //        }
                                    //        else
                                    //        {
                                    //            if (data.Cpsid == item.CpsID && data.Ceqid == item.CeqID)
                                    //            {
                                    //                if (item.IsEnd)
                                    //                {
                                    //                    Application.Current.Dispatcher.Invoke(() => { model.PointItems.Add(data); } );
                                    //                }
                                    //            }
                                    //        }
                                    //    }
                                    //}
                                }
                                else
                                {
                                    //Application.Current.Dispatcher.Invoke(() => { dataModel.view.Close(); });
                                    MessageBox.Show($"STATISTICS_15MIN 테이블 \n\rCODE:{response.Error.Code} MSG:{response.Error.Message}", "데이터 조회", MessageBoxButton.OK, MessageBoxImage.Information);
                                }
                            }
                        }
                        break;
                    case (int)SearchTypeCode.STATISTICSHOUR:
                        {
                            GetStatisticsHourData.Command request = new GetStatisticsHourData.Command
                            {
                                CeqList = CheckItems,
                                FromDate = fromDate,
                                ToDate = toDate
                            };

                            var model = App.Current.Services.GetService<ViewModel_StatisticsHourData>()!;
                            if (model != null)
                            {
                                model.PointItems = new ObservableCollection<StatisticsHour>();
                                var response = await _mediator.Send(request);
                                if (response != null && response.Result)
                                {

                                    //foreach (var data in response.datas)
                                    //{
                                    //    foreach (var item in CheckItems)
                                    //    {
                                    //        if (item.Type == TreeTypeCode.COMPOSITE)
                                    //        {
                                    //            if (data.Cpsid == item.CpsID)
                                    //            {
                                    //                if (item.IsEnd)
                                    //                {
                                    //                    Application.Current.Dispatcher.Invoke(() => { model.PointItems.Add(data); });
                                    //                }
                                    //            }
                                    //        }
                                    //        else
                                    //        {
                                    //            if (data.Cpsid == item.CpsID && data.Ceqid == item.CeqID)
                                    //            {
                                    //                if (item.IsEnd)
                                    //                {
                                    //                    Application.Current.Dispatcher.Invoke(() => { model.PointItems.Add(data); });
                                    //                }
                                    //            }
                                    //        }
                                    //    }
                                    //}
                                }
                                else
                                {
                                    //Application.Current.Dispatcher.Invoke(() => { dataModel.view.Close(); });
                                    MessageBox.Show($"STATISTICS_HOUR 테이블 \n\rCODE:{response.Error.Code} MSG:{response.Error.Message}", "데이터 조회", MessageBoxButton.OK, MessageBoxImage.Information);
                                }
                            }
                        }
                        break;
                    case (int)SearchTypeCode.STATISTICSDAY:
                        {
                            GetStatisticsDayData.Command request = new GetStatisticsDayData.Command
                            {
                                CeqList = CheckItems,
                                FromDate = fromDate,
                                ToDate = toDate
                            };

                            var model = App.Current.Services.GetService<ViewModel_StatisticsDayData>()!;
                            if (model != null)
                            {
                                model.PointItems = new ObservableCollection<StatisticsDay>();
                                var response = await _mediator.Send(request);
                                if (response != null && response.Result)
                                {

                                    //foreach (var data in response.datas)
                                    //{
                                    //    foreach (var item in CheckItems)
                                    //    {
                                    //        if (item.Type == TreeTypeCode.COMPOSITE)
                                    //        {
                                    //            if (data.Cpsid == item.CpsID)
                                    //            {
                                    //                if (item.IsEnd)
                                    //                {
                                    //                    Application.Current.Dispatcher.Invoke(() => { model.PointItems.Add(data); });
                                    //                }
                                    //            }
                                    //        }
                                    //        else
                                    //        {
                                    //            if (data.Cpsid == item.CpsID && data.Ceqid == item.CeqID)
                                    //            {
                                    //                if (item.IsEnd)
                                    //                {
                                    //                    Application.Current.Dispatcher.Invoke(() => { model.PointItems.Add(data); });
                                    //                }
                                    //            }
                                    //        }
                                    //    }
                                    //}
                                }
                                else
                                {
                                    //Application.Current.Dispatcher.Invoke(() => { dataModel.view.Close(); });
                                    MessageBox.Show($"STATISTICS_DAY 테이블 \n\rCODE:{response.Error.Code} MSG:{response.Error.Message}", "데이터 조회", MessageBoxButton.OK, MessageBoxImage.Information);
                                }
                            }
                        }
                        break;
                    case (int)SearchTypeCode.STATISTICSMONTH:
                        {
                            GetStatisticsMonthData.Command request = new GetStatisticsMonthData.Command
                            {
                                CeqList = CheckItems,
                                FromDate = fromDate,
                                ToDate = toDate
                            };

                            var model = App.Current.Services.GetService<ViewModel_StatisticsMonthData>()!;
                            if (model != null)
                            {
                                model.PointItems = new ObservableCollection<StatisticsMonth>();
                                var response = await _mediator.Send(request);
                                if (response != null && response.Result)
                                {

                                    //foreach (var data in response.datas)
                                    //{
                                    //    foreach (var item in CheckItems)
                                    //    {
                                    //        if (item.Type == TreeTypeCode.COMPOSITE)
                                    //        {
                                    //            if (data.Cpsid == item.CpsID)
                                    //            {
                                    //                if (item.IsEnd)
                                    //                {
                                    //                    Application.Current.Dispatcher.Invoke(() => { model.PointItems.Add(data); });
                                    //                }
                                    //            }
                                    //        }
                                    //        else
                                    //        {
                                    //            if (data.Cpsid == item.CpsID && data.Ceqid == item.CeqID)
                                    //            {
                                    //                if (item.IsEnd)
                                    //                {
                                    //                    Application.Current.Dispatcher.Invoke(() => { model.PointItems.Add(data); });
                                    //                }
                                    //            }
                                    //        }
                                    //    }
                                    //}
                                }
                                else
                                {
                                    //Application.Current.Dispatcher.Invoke(() => { dataModel.view.Close(); });
                                    MessageBox.Show($"STATISTICS_MONTH 테이블 \n\rCODE:{response.Error.Code} MSG:{response.Error.Message}", "데이터 조회", MessageBoxButton.OK, MessageBoxImage.Information);
                                }
                            }
                        }
                        break;
                    case (int)SearchTypeCode.STATISTICSYEAR:
                        {
                            GetStatisticsYearData.Command request = new GetStatisticsYearData.Command
                            {
                                CeqList = CheckItems,
                                FromDate = fromDate,
                                ToDate = toDate
                            };

                            var model = App.Current.Services.GetService<ViewModel_StatisticsYearData>()!;
                            if (model != null)
                            {
                                model.PointItems = new ObservableCollection<StatisticsYear>();
                                var response = await _mediator.Send(request);
                                if (response != null && response.Result)
                                {

                                    //foreach (var data in response.datas)
                                    //{
                                    //    foreach (var item in CheckItems)
                                    //    {
                                    //        if (item.Type == TreeTypeCode.COMPOSITE)
                                    //        {
                                    //            if (data.Cpsid == item.CpsID)
                                    //            {
                                    //                if (item.IsEnd)
                                    //                {
                                    //                    Application.Current.Dispatcher.Invoke(() => { model.PointItems.Add(data); });
                                    //                }
                                    //            }
                                    //        }
                                    //        else
                                    //        {
                                    //            if (data.Cpsid == item.CpsID && data.Ceqid == item.CeqID)
                                    //            {
                                    //                if (item.IsEnd)
                                    //                {
                                    //                    Application.Current.Dispatcher.Invoke(() => { model.PointItems.Add(data); });
                                    //                }
                                    //            }
                                    //        }
                                    //    }
                                    //}
                                }
                                else
                                {
                                    //Application.Current.Dispatcher.Invoke(() => { dataModel.view.Close(); });
                                    MessageBox.Show($"STATISTICS_YEAR 테이블 \n\rCODE:{response.Error.Code} MSG:{response.Error.Message}", "데이터 조회", MessageBoxButton.OK, MessageBoxImage.Information);
                                }
                            }
                        }
                        break;
                    case (int)SearchTypeCode.FIALARM:
                        {
                            GetFiAlarmData.Command request = new GetFiAlarmData.Command
                            {
                                CeqList = CheckItems,
                                FromDate = fromDate,
                                ToDate = toDate
                            };

                            var model = App.Current.Services.GetService<ViewModel_FiAlarmData>()!;
                            if (model != null)
                            {
                                model.PointItems = new ObservableCollection<HistoryFiAlarm>();
                                var response = await _mediator.Send(request);
                                if (response != null && response.Result)
                                {

                                    //foreach (var data in response.datas)
                                    //{
                                    //    foreach (var item in CheckItems)
                                    //    {
                                    //        if (item.Type == TreeTypeCode.COMPOSITE)
                                    //        {
                                    //            if (data.Cpsid == item.CpsID)
                                    //            {
                                    //                if (item.IsEnd)
                                    //                {
                                    //                    Application.Current.Dispatcher.Invoke(() => { model.PointItems.Add(data); });
                                    //                }
                                    //            }
                                    //        }
                                    //        else
                                    //        {
                                    //            if (data.Cpsid == item.CpsID && data.Ceqid == item.CeqID)
                                    //            {
                                    //                if (item.IsEnd)
                                    //                {
                                    //                    Application.Current.Dispatcher.Invoke(() => { model.PointItems.Add(data); });
                                    //                }
                                    //            }
                                    //        }
                                    //    }
                                    //}
                                }
                                else
                                {
                                    //Application.Current.Dispatcher.Invoke(() => { dataModel.view.Close(); });
                                    MessageBox.Show($"HISTORY_MIN_DATA 테이블 \n\rCODE:{response.Error.Code} MSG:{response.Error.Message}", "데이터 조회", MessageBoxButton.OK, MessageBoxImage.Information);
                                }
                            }
                        }
                        break;
                    case (int)SearchTypeCode.COMMSTATE:
                        {
                            GetCommStateData.Command request = new GetCommStateData.Command
                            {
                                CeqList = CheckItems,
                                FromDate = fromDate,
                                ToDate = toDate
                            };

                            var model = App.Current.Services.GetService<ViewModel_CommDayData>()!;
                            if (model != null)
                            {
                                model.PointItems = new ObservableCollection<HistoryCommState>();
                                var response = await _mediator.Send(request);
                                if (response != null && response.Result)
                                {

                                    //foreach (var data in response.datas)
                                    //{
                                    //    foreach (var item in CheckItems)
                                    //    {
                                    //        if (data.Cpsid == item.CpsID)
                                    //        {
                                    //            if (item.IsEnd)
                                    //            {
                                    //                Application.Current.Dispatcher.Invoke(() => { model.PointItems.Add(data); });
                                    //            }
                                    //        }
                                    //    }
                                    //}
                                }
                                else
                                {
                                    //Application.Current.Dispatcher.Invoke(() => { dataModel.view.Close(); });
                                    MessageBox.Show($"HISTORY_COMM_STATE 테이블 \n\rCODE:{response.Error.Code} MSG:{response.Error.Message}", "데이터 조회", MessageBoxButton.OK, MessageBoxImage.Information);
                                }
                            }
                        }
                        break;
                    case (int)SearchTypeCode.COMMSTATELOG:
                        {
                            GetCommStateLogData.Command request = new GetCommStateLogData.Command
                            {
                                CeqList = CheckItems,
                                FromDate = fromDate,
                                ToDate = toDate
                            };

                            var model = App.Current.Services.GetService<ViewModel_CommLogData>()!;
                            if (model != null)
                            {
                                model.PointItems = new ObservableCollection<HistoryCommStateLog>();
                                var response = await _mediator.Send(request);
                                if (response != null && response.Result)
                                {

                                    //foreach (var data in response.datas)
                                    //{
                                    //    foreach (var item in CheckItems)
                                    //    {
                                    //        if (data.Cpsid == item.CpsID)
                                    //        {
                                    //            if (item.IsEnd)
                                    //            {
                                    //                Application.Current.Dispatcher.Invoke(() => { model.PointItems.Add(data); });
                                    //            }
                                    //        }
                                    //    }
                                    //}
                                }
                                else
                                {
                                    //Application.Current.Dispatcher.Invoke(() => { dataModel.view.Close(); });
                                    MessageBox.Show($"HISTORY_COMM_STATE_LOG 테이블 \n\rCODE:{response.Error.Code} MSG:{response.Error.Message}", "데이터 조회", MessageBoxButton.OK, MessageBoxImage.Information);
                                }
                            }
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public object GetPointItems(int type)
        {
            object retValue = new object();
            switch (type)
            {
                case (int)SearchTypeCode.MINDATA:
                    {
                        var model = App.Current.Services.GetService<ViewModel_SwitchData>()!;
                        if (model != null)
                            retValue = model.PointItems;
                    }
                    break;
                case (int)SearchTypeCode.DAYSTATDATA:
                    {

                        var model = App.Current.Services.GetService<ViewModel_DayStatData>()!;
                        if (model != null)
                            retValue = model.PointItems;

                    }
                    break;
                case (int)SearchTypeCode.STATISTICSMIN:
                    {
                        var model = App.Current.Services.GetService<ViewModel_StatisticsMinData>()!;
                        if (model != null)
                            retValue = model.PointItems;
                    }
                    break;
                case (int)SearchTypeCode.STATISTICSHOUR:
                    {
                        var model = App.Current.Services.GetService<ViewModel_StatisticsHourData>()!;
                        if (model != null)
                            retValue = model.PointItems;
                    }
                    break;
                case (int)SearchTypeCode.STATISTICSDAY:
                    {
                        var model = App.Current.Services.GetService<ViewModel_StatisticsDayData>()!;
                        if (model != null)
                            retValue = model.PointItems;
                    }
                    break;
                case (int)SearchTypeCode.STATISTICSMONTH:
                    {
                        var model = App.Current.Services.GetService<ViewModel_StatisticsMonthData>()!;
                        if (model != null)
                            retValue = model.PointItems;
                    }
                    break;
                case (int)SearchTypeCode.STATISTICSYEAR:
                    {
                        var model = App.Current.Services.GetService<ViewModel_StatisticsYearData>()!;
                        if (model != null)
                            retValue = model.PointItems;
                    }
                    break;
                case (int)SearchTypeCode.FIALARM:
                    {

                        var model = App.Current.Services.GetService<ViewModel_FiAlarmData>()!;
                        if (model != null)
                            retValue = model.PointItems;
                    }
                    break;
                case (int)SearchTypeCode.COMMSTATE:
                    {

                        var model = App.Current.Services.GetService<ViewModel_CommDayData>()!;
                        if (model != null)
                            retValue = model.PointItems;
                    }
                    break;
                case (int)SearchTypeCode.COMMSTATELOG:
                    {
                        var model = App.Current.Services.GetService<ViewModel_CommLogData>()!;
                        if (model != null)
                            retValue = model.PointItems;
                    }
                    break;
            }

            return retValue;
        }

        public void ThreadClose()
        {
            ThreadFlag = false;
        }

        private async Task DataStatusBar()
        {
            while(ThreadFlag)
            {
                try
                {
                    var mainModel = App.Current.Services.GetService<MainViewModel>()!;
                    if (mainModel != null)
                    {
                        var configModel = App.Current.Services.GetService<ConfigViewModel>()!;
                        if (configModel != null)
                        {
                            // 서버 상태
                            // 서버 상태
                            var serverState = ProgramHandler.GetProcID(configModel.serverName);
                            if (serverState > 0)
                                mainModel.ServerState = $"서버 상태: 정상";
                            else
                                mainModel.ServerState = $"서버 상태: 비정상";

                            // DB 상태
                            try
                            {
                                bool retValue = new MySqlMapper(_configuration).IsConnection(); // MySqlMapper.IsConnection(configModel.connectionString);
                                if (retValue)
                                    mainModel.DBState = $"DB 상태: 정상";
                                else
                                    mainModel.DBState = $"DB 상태: 비정상";
                            }
                            catch
                            {
                                mainModel.DBState = $"DB 상태: 비정상";
                            }
                            //var connectionString = _configuration.GetConnectionString("Server");
                            //if (!string.IsNullOrEmpty(connectionString))
                            //{
                            //    var ipAdderss = connectionString.Split(';')[0].Replace("Server=", "");
                            //    var dBName = connectionString.Split(';')[1].Replace("Database=", "");
                            //    var userName = connectionString.Split(';')[2].Replace("User=", "");
                            //    var userPassword = connectionString.Split(';')[3].Replace("Password=", "");

                            //    try
                            //    {
                            //        bool retValue = ConnectCheck.IsConnection(configModel.ConnectionString);
                            //        if (retValue)
                            //            mainModel.DBState = $"DB 상태: 정상";
                            //        else
                            //            mainModel.DBState = $"DB 상태: 비정상";
                            //    }
                            //    catch
                            //    {
                            //        mainModel.DBState = $"DB 상태: 비정상";
                            //    }
                            //}
                        }

                        // BI 연계 데이터
                        var biModel = App.Current.Services.GetService<OperationBiViewModel>()!;
                        if (biModel != null)
                        {
                            mainModel.BiCount = biModel.DataCount;
                        }

                        // AI 연계 데이터
                        var aiModel = App.Current.Services.GetService<OperationAiViewModel>()!;
                        if (aiModel != null)
                        {
                            mainModel.AiCount = aiModel.DataCount;
                        }

                        // 알람 연계 데이터
                        var alarmModel = App.Current.Services.GetService<OperationAlarmViewModel>()!;
                        if (alarmModel != null)
                        {
                            mainModel.AlarmCount = alarmModel.DataCount;
                        }

                        // 스케줄 정보
                        var schduleModel = App.Current.Services.GetService<OperationSchduleViewModel>()!;
                        if (schduleModel != null)
                        {
                            mainModel.ScheduleMeasureBIBO = $" - BI 계측: {schduleModel.biTime} / BO 계측: {schduleModel.boTime}";
                            //mainModel.ScheduleMeasureBO = $" - BO 계측: {schduleModel.boTime} (분)";
                            mainModel.ScheduleMeasureAIAO = $" - AI 계측: {schduleModel.aiTime} / AO 계측: {schduleModel.aoTime}";
                            //mainModel.ScheduleMeasureAO = $" - AO 계측: {schduleModel.aoTime} (분)";
                            mainModel.ScheduleMeasureCounter = $" - CNT 계측: {schduleModel.counterTime}";
                            mainModel.StoragePeriodStatistics = $" - 통계생성: {schduleModel.statisticalTime.ToString("HH:mm:ss")}";
                        }

                        // 보관주기 정보
                        var storageModel = App.Current.Services.GetService<OperationStorageViewModel>()!;
                        if (storageModel != null)
                        {
                            //mainModel.StoragePeriodData = $" - 데이터: {storageModel.GetStorage()}";
                            mainModel.SPRealMinData = $" - MIN_DATA : {storageModel.hisMinTime}";
                            mainModel.SPDayStatData = $" - DAYSTAT_DATA : {storageModel.hisStatTime}";
                            mainModel.SPStatMinData = $" - STATISTICS_15MIN : {storageModel.statMinTime}";
                            mainModel.SPStatHourData = $" - STATISTICS_HOUR : {storageModel.statHourTime}";
                            mainModel.SPStatDayData = $" - STATISTICS_DAY : {storageModel.statDayTime}";
                            mainModel.SPStatMonthData = $" - STATISTICS_MONTH : {storageModel.statMonthTime}";
                            mainModel.SPStatYearData = $" - STATISTICS_YEAR : {storageModel.statYearTime}";
                            mainModel.SPFiData = $" - FI_ALARM : {storageModel.hisFiTime}";
                            mainModel.SPCommData = $" - COMM_STATE : {storageModel.hisCommTime}";
                            mainModel.SPCommLogData = $" - COMM_STATE_LOG : {storageModel.hisCommLogTime}";

                            //mainModel.ScheduleDelete = $" - 삭제주기: {storageModel.deleteTime}";
                        }
                    }
                    await Task.Delay(1000);
                }
                catch
                {

                }
            }
        }
    }
}
