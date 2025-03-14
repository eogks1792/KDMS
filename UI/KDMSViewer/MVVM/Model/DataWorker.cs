﻿using DevExpress.ClipboardSource.SpreadsheetML;
using DevExpress.Xpf.Editors.Helpers;
using KDMS.EF.Core.Contexts;
using KDMS.EF.Core.Infrastructure.Reverse.Models;
using KDMSViewer.Extensions;
using KDMSViewer.Features;
using KDMSViewer.View;
using KDMSViewer.ViewModel;
using MediatR;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using System.Xml.Linq;
using static DevExpress.Charts.Designer.Native.BarThicknessEditViewModel;
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

        //public void ThreadFilterMessage(ref MSG msg, ref bool handled)
        //{
        //    if (msg.message == Win32API.message && msg.wParam != Win32API.handle)
        //    {
        //        MessageBox.Show("Message : " + msg.lParam.ToString());
        //    }
        //}

        //public void PostMessageSend(int value)
        //{
        //    var retValue = Win32API.PostMessage((IntPtr)Win32API.HWND_BROADCAST, Win32API.message, (uint)Win32API.handle, (uint)value);
        //    if(retValue)
        //    {

        //    }
        //    else
        //    {
        //        MessageBox.Show("111111111111");
        //    }
        //}

        private void TreeViewListInit()
        {
            // 트리 목록 생성 처리
            TreeDatas = _commonData.TreeListInit();
        }

        public async void GetTrandData(List<long> ceqList, int type, DateTime fromDate, DateTime toDate)
        {
            var model = App.Current.Services.GetService<TrandViewModel>()!;
            if (model == null)
                return;

            try
            {
                switch (type)
                {
                    case (int)SearchTypeCode.MINDATA:
                        {
                            GetSwitchData.Command request = new GetSwitchData.Command
                            {
                                CeqList = ceqList,
                                FromDate = fromDate,
                                ToDate = toDate,
                            };

                            var response = await _mediator.Send(request);
                            if (response != null && response.Result)
                            {
                                model.PointItems.AddRange(response.datas.Select(p => new ChartPointDataModel
                                {
                                    CeqId = p.Ceqid,
                                    Name = p.Name ?? string.Empty,
                                    PointName = "CURRENT_A",
                                    PointValue = p.CurrentA ?? 0.0f,
                                    UpdateTime = p.SaveTime
                                }).ToList());
                                model.PointItems.AddRange(response.datas.Select(p => new ChartPointDataModel
                                {
                                    CeqId = p.Ceqid,
                                    Name = p.Name ?? string.Empty,
                                    PointName = "CURRENT_B",
                                    PointValue = p.CurrentB ?? 0.0f,
                                    UpdateTime = p.SaveTime
                                }).ToList());
                                model.PointItems.AddRange(response.datas.Select(p => new ChartPointDataModel
                                {
                                    CeqId = p.Ceqid,
                                    Name = p.Name ?? string.Empty,
                                    PointName = "CURRENT_C",
                                    PointValue = p.CurrentC ?? 0.0f,
                                    UpdateTime = p.SaveTime
                                }).ToList());
                                model.PointItems.AddRange(response.datas.Select(p => new ChartPointDataModel
                                {
                                    CeqId = p.Ceqid,
                                    Name = p.Name ?? string.Empty,
                                    PointName = "CURRENT_N",
                                    PointValue = p.CurrentN ?? 0.0f,
                                    UpdateTime = p.SaveTime
                                }).ToList());

                                model.PointItems.AddRange(response.datas.Select(p => new ChartPointDataModel
                                {
                                    CeqId = p.Ceqid,
                                    Name = p.Name ?? string.Empty,
                                    PointName = "VOLTAGE_A",
                                    PointValue = p.VoltageA ?? 0.0f,
                                    UpdateTime = p.SaveTime
                                }).ToList());
                                model.PointItems.AddRange(response.datas.Select(p => new ChartPointDataModel
                                {
                                    CeqId = p.Ceqid,
                                    Name = p.Name ?? string.Empty,
                                    PointName = "VOLTAGE_B",
                                    PointValue = p.VoltageB ?? 0.0f,
                                    UpdateTime = p.SaveTime
                                }).ToList());
                                model.PointItems.AddRange(response.datas.Select(p => new ChartPointDataModel
                                {
                                    CeqId = p.Ceqid,
                                    Name = p.Name ?? string.Empty,
                                    PointName = "VOLTAGE_C",
                                    PointValue = p.VoltageC ?? 0.0f,
                                    UpdateTime = p.SaveTime
                                }).ToList());
                            }
                            else
                            {
                                DataResultView($"HISTORY_MIN_DATA 테이블 MSG:{response.Error.Message}");
                                //MessageBox.Show($"HISTORY_MIN_DATA 테이블 \n\rCODE:{response.Error.Code} MSG:{response.Error.Message}", "데이터 조회", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                        }
                        break;
                    case (int)SearchTypeCode.DAYSTATDATA:
                        {
                            GetDayStatData.Command request = new GetDayStatData.Command
                            {
                                CeqList = ceqList,
                                FromDate = fromDate,
                                ToDate = toDate
                            };

                            var response = await _mediator.Send(request);
                            if (response != null && response.Result)
                            {
                                model.PointItems.AddRange(response.datas.Select(p => new ChartPointDataModel
                                {
                                    CeqId = p.Ceqid,
                                    Name = p.Name ?? string.Empty,
                                    PointName = "AVERAGE_CURRENT_A",
                                    PointValue = p.AverageCurrentA ?? 0.0f,
                                    UpdateTime = p.SaveTime
                                }).ToList());
                                model.PointItems.AddRange(response.datas.Select(p => new ChartPointDataModel
                                {
                                    CeqId = p.Ceqid,
                                    Name = p.Name ?? string.Empty,
                                    PointName = "AVERAGE_CURRENT_B",
                                    PointValue = p.AverageCurrentB ?? 0.0f,
                                    UpdateTime = p.SaveTime
                                }).ToList());
                                model.PointItems.AddRange(response.datas.Select(p => new ChartPointDataModel
                                {
                                    CeqId = p.Ceqid,
                                    Name = p.Name ?? string.Empty,
                                    PointName = "AVERAGE_CURRENT_C",
                                    PointValue = p.AverageCurrentC ?? 0.0f,
                                    UpdateTime = p.SaveTime
                                }).ToList());
                                model.PointItems.AddRange(response.datas.Select(p => new ChartPointDataModel
                                {
                                    CeqId = p.Ceqid,
                                    Name = p.Name ?? string.Empty,
                                    PointName = "AVERAGE_CURRENT_N",
                                    PointValue = p.AverageCurrentN ?? 0.0f,
                                    UpdateTime = p.SaveTime
                                }).ToList());

                                model.PointItems.AddRange(response.datas.Select(p => new ChartPointDataModel
                                {
                                    CeqId = p.Ceqid,
                                    Name = p.Name ?? string.Empty,
                                    PointName = "MAX_CURRENT_A",
                                    PointValue = p.MaxCurrentA ?? 0.0f,
                                    UpdateTime = p.SaveTime
                                }).ToList());
                                model.PointItems.AddRange(response.datas.Select(p => new ChartPointDataModel
                                {
                                    CeqId = p.Ceqid,
                                    Name = p.Name ?? string.Empty,
                                    PointName = "MAX_CURRENT_B",
                                    PointValue = p.MaxCurrentB ?? 0.0f,
                                    UpdateTime = p.SaveTime
                                }).ToList());
                                model.PointItems.AddRange(response.datas.Select(p => new ChartPointDataModel
                                {
                                    CeqId = p.Ceqid,
                                    Name = p.Name ?? string.Empty,
                                    PointName = "MAX_CURRENT_C",
                                    PointValue = p.MaxCurrentC ?? 0.0f,
                                    UpdateTime = p.SaveTime
                                }).ToList());
                                model.PointItems.AddRange(response.datas.Select(p => new ChartPointDataModel
                                {
                                    CeqId = p.Ceqid,
                                    Name = p.Name ?? string.Empty,
                                    PointName = "MAX_CURRENT_N",
                                    PointValue = p.MaxCurrentN ?? 0.0f,
                                    UpdateTime = p.SaveTime
                                }).ToList());

                                model.PointItems.AddRange(response.datas.Select(p => new ChartPointDataModel
                                {
                                    CeqId = p.Ceqid,
                                    Name = p.Name ?? string.Empty,
                                    PointName = "MIN_CURRENT_A",
                                    PointValue = p.MinCurrentA ?? 0.0f,
                                    UpdateTime = p.SaveTime
                                }).ToList());
                                model.PointItems.AddRange(response.datas.Select(p => new ChartPointDataModel
                                {
                                    CeqId = p.Ceqid,
                                    Name = p.Name ?? string.Empty,
                                    PointName = "MIN_CURRENT_B",
                                    PointValue = p.MinCurrentB ?? 0.0f,
                                    UpdateTime = p.SaveTime
                                }).ToList());
                                model.PointItems.AddRange(response.datas.Select(p => new ChartPointDataModel
                                {
                                    CeqId = p.Ceqid,
                                    Name = p.Name ?? string.Empty,
                                    PointName = "MIN_CURRENT_C",
                                    PointValue = p.MinCurrentC ?? 0.0f,
                                    UpdateTime = p.SaveTime
                                }).ToList());
                                model.PointItems.AddRange(response.datas.Select(p => new ChartPointDataModel
                                {
                                    CeqId = p.Ceqid,
                                    Name = p.Name ?? string.Empty,
                                    PointName = "MIN_CURRENT_N",
                                    PointValue = p.MinCurrentN ?? 0.0f,
                                    UpdateTime = p.SaveTime
                                }).ToList());
                            }
                            else
                            {
                                DataResultView($"HISTORY_DAYSTAT_DATA 테이블 MSG:{response.Error.Message}");
                                //MessageBox.Show($"HISTORY_DAYSTAT_DATA 테이블 \n\rCODE:{response.Error.Code} MSG:{response.Error.Message}", "데이터 조회", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                        }
                        break;
                    case (int)SearchTypeCode.STATISTICSMIN:
                        {
                            GetStatisticsMinData.Command request = new GetStatisticsMinData.Command
                            {
                                CeqList = ceqList,
                                FromDate = fromDate,
                                ToDate = toDate
                            };

                            var response = await _mediator.Send(request);
                            if (response != null && response.Result)
                            {
                                model.PointItems.AddRange(response.datas.Select(p => new ChartPointDataModel
                                {
                                    CeqId = p.Ceqid,
                                    Name = p.Name ?? string.Empty,
                                    PointName = "AVERAGE_CURRENT_A",
                                    PointValue = p.AverageCurrentA ?? 0.0f,
                                    UpdateTime = p.SaveTime
                                }).ToList());
                                model.PointItems.AddRange(response.datas.Select(p => new ChartPointDataModel
                                {
                                    CeqId = p.Ceqid,
                                    Name = p.Name ?? string.Empty,
                                    PointName = "AVERAGE_CURRENT_B",
                                    PointValue = p.AverageCurrentB ?? 0.0f,
                                    UpdateTime = p.SaveTime
                                }).ToList());
                                model.PointItems.AddRange(response.datas.Select(p => new ChartPointDataModel
                                {
                                    CeqId = p.Ceqid,
                                    Name = p.Name ?? string.Empty,
                                    PointName = "AVERAGE_CURRENT_C",
                                    PointValue = p.AverageCurrentC ?? 0.0f,
                                    UpdateTime = p.SaveTime
                                }).ToList());
                                model.PointItems.AddRange(response.datas.Select(p => new ChartPointDataModel
                                {
                                    CeqId = p.Ceqid,
                                    Name = p.Name ?? string.Empty,
                                    PointName = "AVERAGE_CURRENT_N",
                                    PointValue = p.AverageCurrentN ?? 0.0f,
                                    UpdateTime = p.SaveTime
                                }).ToList());
                            }
                            else
                            {
                                DataResultView($"STATISTICS_15MIN 테이블 MSG:{response.Error.Message}");
                                //MessageBox.Show($"STATISTICS_15MIN 테이블 \n\rCODE:{response.Error.Code} MSG:{response.Error.Message}", "데이터 조회", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                        }
                        break;
                    case (int)SearchTypeCode.STATISTICSHOUR:
                        {
                            GetStatisticsHourData.Command request = new GetStatisticsHourData.Command
                            {
                                CeqList = ceqList,
                                FromDate = fromDate,
                                ToDate = toDate
                            };

                            var response = await _mediator.Send(request);
                            if (response != null && response.Result)
                            {
                                model.PointItems.AddRange(response.datas.Select(p => new ChartPointDataModel
                                {
                                    CeqId = p.Ceqid,
                                    Name = p.Name ?? string.Empty,
                                    PointName = "AVERAGE_CURRENT_A",
                                    PointValue = p.AverageCurrentA ?? 0.0f,
                                    UpdateTime = p.SaveTime
                                }).ToList());
                                model.PointItems.AddRange(response.datas.Select(p => new ChartPointDataModel
                                {
                                    CeqId = p.Ceqid,
                                    Name = p.Name ?? string.Empty,
                                    PointName = "AVERAGE_CURRENT_B",
                                    PointValue = p.AverageCurrentB ?? 0.0f,
                                    UpdateTime = p.SaveTime
                                }).ToList());
                                model.PointItems.AddRange(response.datas.Select(p => new ChartPointDataModel
                                {
                                    CeqId = p.Ceqid,
                                    Name = p.Name ?? string.Empty,
                                    PointName = "AVERAGE_CURRENT_C",
                                    PointValue = p.AverageCurrentC ?? 0.0f,
                                    UpdateTime = p.SaveTime
                                }).ToList());
                                model.PointItems.AddRange(response.datas.Select(p => new ChartPointDataModel
                                {
                                    CeqId = p.Ceqid,
                                    Name = p.Name ?? string.Empty,
                                    PointName = "AVERAGE_CURRENT_N",
                                    PointValue = p.AverageCurrentN ?? 0.0f,
                                    UpdateTime = p.SaveTime
                                }).ToList());

                                model.PointItems.AddRange(response.datas.Select(p => new ChartPointDataModel
                                {
                                    CeqId = p.Ceqid,
                                    Name = p.Name ?? string.Empty,
                                    PointName = "MAX_CURRENT_A",
                                    PointValue = p.MaxCurrentA ?? 0.0f,
                                    UpdateTime = p.SaveTime
                                }).ToList());
                                model.PointItems.AddRange(response.datas.Select(p => new ChartPointDataModel
                                {
                                    CeqId = p.Ceqid,
                                    Name = p.Name ?? string.Empty,
                                    PointName = "MAX_CURRENT_B",
                                    PointValue = p.MaxCurrentB ?? 0.0f,
                                    UpdateTime = p.SaveTime
                                }).ToList());
                                model.PointItems.AddRange(response.datas.Select(p => new ChartPointDataModel
                                {
                                    CeqId = p.Ceqid,
                                    Name = p.Name ?? string.Empty,
                                    PointName = "MAX_CURRENT_C",
                                    PointValue = p.MaxCurrentC ?? 0.0f,
                                    UpdateTime = p.SaveTime
                                }).ToList());
                                model.PointItems.AddRange(response.datas.Select(p => new ChartPointDataModel
                                {
                                    CeqId = p.Ceqid,
                                    Name = p.Name ?? string.Empty,
                                    PointName = "MAX_CURRENT_N",
                                    PointValue = p.MaxCurrentN ?? 0.0f,
                                    UpdateTime = p.SaveTime
                                }).ToList());

                                model.PointItems.AddRange(response.datas.Select(p => new ChartPointDataModel
                                {
                                    CeqId = p.Ceqid,
                                    Name = p.Name ?? string.Empty,
                                    PointName = "MIN_CURRENT_A",
                                    PointValue = p.MinCurrentA ?? 0.0f,
                                    UpdateTime = p.SaveTime
                                }).ToList());
                                model.PointItems.AddRange(response.datas.Select(p => new ChartPointDataModel
                                {
                                    CeqId = p.Ceqid,
                                    Name = p.Name ?? string.Empty,
                                    PointName = "MIN_CURRENT_B",
                                    PointValue = p.MinCurrentB ?? 0.0f,
                                    UpdateTime = p.SaveTime
                                }).ToList());
                                model.PointItems.AddRange(response.datas.Select(p => new ChartPointDataModel
                                {
                                    CeqId = p.Ceqid,
                                    Name = p.Name ?? string.Empty,
                                    PointName = "MIN_CURRENT_C",
                                    PointValue = p.MinCurrentC ?? 0.0f,
                                    UpdateTime = p.SaveTime
                                }).ToList());
                                model.PointItems.AddRange(response.datas.Select(p => new ChartPointDataModel
                                {
                                    CeqId = p.Ceqid,
                                    Name = p.Name ?? string.Empty,
                                    PointName = "MIN_CURRENT_N",
                                    PointValue = p.MinCurrentN ?? 0.0f,
                                    UpdateTime = p.SaveTime
                                }).ToList());

                            }
                            else
                            {
                                DataResultView($"STATISTICS_HOUR 테이블 MSG:{response.Error.Message}");
                                //MessageBox.Show($"STATISTICS_HOUR 테이블 \n\rCODE:{response.Error.Code} MSG:{response.Error.Message}", "데이터 조회", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                        }
                        break;
                    case (int)SearchTypeCode.STATISTICSDAY:
                        {
                            GetStatisticsDayData.Command request = new GetStatisticsDayData.Command
                            {
                                CeqList = ceqList,
                                FromDate = fromDate,
                                ToDate = toDate
                            };

                            var response = await _mediator.Send(request);
                            if (response != null && response.Result)
                            {
                                model.PointItems.AddRange(response.datas.Select(p => new ChartPointDataModel
                                {
                                    CeqId = p.Ceqid,
                                    Name = p.Name ?? string.Empty,
                                    PointName = "AVERAGE_CURRENT_A",
                                    PointValue = p.AverageCurrentA ?? 0.0f,
                                    UpdateTime = p.SaveTime
                                }).ToList());
                                model.PointItems.AddRange(response.datas.Select(p => new ChartPointDataModel
                                {
                                    CeqId = p.Ceqid,
                                    Name = p.Name ?? string.Empty,
                                    PointName = "AVERAGE_CURRENT_B",
                                    PointValue = p.AverageCurrentB ?? 0.0f,
                                    UpdateTime = p.SaveTime
                                }).ToList());
                                model.PointItems.AddRange(response.datas.Select(p => new ChartPointDataModel
                                {
                                    CeqId = p.Ceqid,
                                    Name = p.Name ?? string.Empty,
                                    PointName = "AVERAGE_CURRENT_C",
                                    PointValue = p.AverageCurrentC ?? 0.0f,
                                    UpdateTime = p.SaveTime
                                }).ToList());
                                model.PointItems.AddRange(response.datas.Select(p => new ChartPointDataModel
                                {
                                    CeqId = p.Ceqid,
                                    Name = p.Name ?? string.Empty,
                                    PointName = "AVERAGE_CURRENT_N",
                                    PointValue = p.AverageCurrentN ?? 0.0f,
                                    UpdateTime = p.SaveTime
                                }).ToList());

                                model.PointItems.AddRange(response.datas.Select(p => new ChartPointDataModel
                                {
                                    CeqId = p.Ceqid,
                                    Name = p.Name ?? string.Empty,
                                    PointName = "MAX_CURRENT_A",
                                    PointValue = p.MaxCurrentA ?? 0.0f,
                                    UpdateTime = p.SaveTime
                                }).ToList());
                                model.PointItems.AddRange(response.datas.Select(p => new ChartPointDataModel
                                {
                                    CeqId = p.Ceqid,
                                    Name = p.Name ?? string.Empty,
                                    PointName = "MAX_CURRENT_B",
                                    PointValue = p.MaxCurrentB ?? 0.0f,
                                    UpdateTime = p.SaveTime
                                }).ToList());
                                model.PointItems.AddRange(response.datas.Select(p => new ChartPointDataModel
                                {
                                    CeqId = p.Ceqid,
                                    Name = p.Name ?? string.Empty,
                                    PointName = "MAX_CURRENT_C",
                                    PointValue = p.MaxCurrentC ?? 0.0f,
                                    UpdateTime = p.SaveTime
                                }).ToList());
                                model.PointItems.AddRange(response.datas.Select(p => new ChartPointDataModel
                                {
                                    CeqId = p.Ceqid,
                                    Name = p.Name ?? string.Empty,
                                    PointName = "MAX_CURRENT_N",
                                    PointValue = p.MaxCurrentN ?? 0.0f,
                                    UpdateTime = p.SaveTime
                                }).ToList());

                                model.PointItems.AddRange(response.datas.Select(p => new ChartPointDataModel
                                {
                                    CeqId = p.Ceqid,
                                    Name = p.Name ?? string.Empty,
                                    PointName = "MIN_CURRENT_A",
                                    PointValue = p.MinCurrentA ?? 0.0f,
                                    UpdateTime = p.SaveTime
                                }).ToList());
                                model.PointItems.AddRange(response.datas.Select(p => new ChartPointDataModel
                                {
                                    CeqId = p.Ceqid,
                                    Name = p.Name ?? string.Empty,
                                    PointName = "MIN_CURRENT_B",
                                    PointValue = p.MinCurrentB ?? 0.0f,
                                    UpdateTime = p.SaveTime
                                }).ToList());
                                model.PointItems.AddRange(response.datas.Select(p => new ChartPointDataModel
                                {
                                    CeqId = p.Ceqid,
                                    Name = p.Name ?? string.Empty,
                                    PointName = "MIN_CURRENT_C",
                                    PointValue = p.MinCurrentC ?? 0.0f,
                                    UpdateTime = p.SaveTime
                                }).ToList());
                                model.PointItems.AddRange(response.datas.Select(p => new ChartPointDataModel
                                {
                                    CeqId = p.Ceqid,
                                    Name = p.Name ?? string.Empty,
                                    PointName = "MIN_CURRENT_N",
                                    PointValue = p.MinCurrentN ?? 0.0f,
                                    UpdateTime = p.SaveTime
                                }).ToList());
                            }
                            else
                            {
                                DataResultView($"STATISTICS_DAY 테이블 MSG:{response.Error.Message}");
                                //MessageBox.Show($"STATISTICS_DAY 테이블 \n\rCODE:{response.Error.Code} MSG:{response.Error.Message}", "데이터 조회", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                        }
                        break;
                    case (int)SearchTypeCode.STATISTICSMONTH:
                        {
                            GetStatisticsMonthData.Command request = new GetStatisticsMonthData.Command
                            {
                                CeqList = ceqList,
                                FromDate = fromDate,
                                ToDate = toDate
                            };

                            var response = await _mediator.Send(request);
                            if (response != null && response.Result)
                            {
                                model.PointItems.AddRange(response.datas.Select(p => new ChartPointDataModel
                                {
                                    CeqId = p.Ceqid,
                                    Name = p.Name ?? string.Empty,
                                    PointName = "AVERAGE_CURRENT_A",
                                    PointValue = p.AverageCurrentA ?? 0.0f,
                                    UpdateTime = p.SaveTime
                                }).ToList());
                                model.PointItems.AddRange(response.datas.Select(p => new ChartPointDataModel
                                {
                                    CeqId = p.Ceqid,
                                    Name = p.Name ?? string.Empty,
                                    PointName = "AVERAGE_CURRENT_B",
                                    PointValue = p.AverageCurrentB ?? 0.0f,
                                    UpdateTime = p.SaveTime
                                }).ToList());
                                model.PointItems.AddRange(response.datas.Select(p => new ChartPointDataModel
                                {
                                    CeqId = p.Ceqid,
                                    Name = p.Name ?? string.Empty,
                                    PointName = "AVERAGE_CURRENT_C",
                                    PointValue = p.AverageCurrentC ?? 0.0f,
                                    UpdateTime = p.SaveTime
                                }).ToList());
                                model.PointItems.AddRange(response.datas.Select(p => new ChartPointDataModel
                                {
                                    CeqId = p.Ceqid,
                                    Name = p.Name ?? string.Empty,
                                    PointName = "AVERAGE_CURRENT_N",
                                    PointValue = p.AverageCurrentN ?? 0.0f,
                                    UpdateTime = p.SaveTime
                                }).ToList());

                                model.PointItems.AddRange(response.datas.Select(p => new ChartPointDataModel
                                {
                                    CeqId = p.Ceqid,
                                    Name = p.Name ?? string.Empty,
                                    PointName = "MAX_CURRENT_A",
                                    PointValue = p.MaxCurrentA ?? 0.0f,
                                    UpdateTime = p.SaveTime
                                }).ToList());
                                model.PointItems.AddRange(response.datas.Select(p => new ChartPointDataModel
                                {
                                    CeqId = p.Ceqid,
                                    Name = p.Name ?? string.Empty,
                                    PointName = "MAX_CURRENT_B",
                                    PointValue = p.MaxCurrentB ?? 0.0f,
                                    UpdateTime = p.SaveTime
                                }).ToList());
                                model.PointItems.AddRange(response.datas.Select(p => new ChartPointDataModel
                                {
                                    CeqId = p.Ceqid,
                                    Name = p.Name ?? string.Empty,
                                    PointName = "MAX_CURRENT_C",
                                    PointValue = p.MaxCurrentC ?? 0.0f,
                                    UpdateTime = p.SaveTime
                                }).ToList());
                                model.PointItems.AddRange(response.datas.Select(p => new ChartPointDataModel
                                {
                                    CeqId = p.Ceqid,
                                    Name = p.Name ?? string.Empty,
                                    PointName = "MAX_CURRENT_N",
                                    PointValue = p.MaxCurrentN ?? 0.0f,
                                    UpdateTime = p.SaveTime
                                }).ToList());

                                model.PointItems.AddRange(response.datas.Select(p => new ChartPointDataModel
                                {
                                    CeqId = p.Ceqid,
                                    Name = p.Name ?? string.Empty,
                                    PointName = "MIN_CURRENT_A",
                                    PointValue = p.MinCurrentA ?? 0.0f,
                                    UpdateTime = p.SaveTime
                                }).ToList());
                                model.PointItems.AddRange(response.datas.Select(p => new ChartPointDataModel
                                {
                                    CeqId = p.Ceqid,
                                    Name = p.Name ?? string.Empty,
                                    PointName = "MIN_CURRENT_B",
                                    PointValue = p.MinCurrentB ?? 0.0f,
                                    UpdateTime = p.SaveTime
                                }).ToList());
                                model.PointItems.AddRange(response.datas.Select(p => new ChartPointDataModel
                                {
                                    CeqId = p.Ceqid,
                                    Name = p.Name ?? string.Empty,
                                    PointName = "MIN_CURRENT_C",
                                    PointValue = p.MinCurrentC ?? 0.0f,
                                    UpdateTime = p.SaveTime
                                }).ToList());
                                model.PointItems.AddRange(response.datas.Select(p => new ChartPointDataModel
                                {
                                    CeqId = p.Ceqid,
                                    Name = p.Name ?? string.Empty,
                                    PointName = "MIN_CURRENT_N",
                                    PointValue = p.MinCurrentN ?? 0.0f,
                                    UpdateTime = p.SaveTime
                                }).ToList());

                            }
                            else
                            {
                                DataResultView($"STATISTICS_MONTH 테이블 MSG:{response.Error.Message}");
                                //MessageBox.Show($"STATISTICS_MONTH 테이블 \n\rCODE:{response.Error.Code} MSG:{response.Error.Message}", "데이터 조회", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                        }
                        break;
                    case (int)SearchTypeCode.STATISTICSYEAR:
                        {
                            GetStatisticsYearData.Command request = new GetStatisticsYearData.Command
                            {
                                CeqList = ceqList,
                                FromDate = fromDate,
                                ToDate = toDate
                            };

                            var response = await _mediator.Send(request);
                            if (response != null && response.Result)
                            {
                                model.PointItems.AddRange(response.datas.Select(p => new ChartPointDataModel
                                {
                                    CeqId = p.Ceqid,
                                    Name = p.Name ?? string.Empty,
                                    PointName = "AVERAGE_CURRENT_A",
                                    PointValue = p.AverageCurrentA ?? 0.0f,
                                    UpdateTime = p.SaveTime
                                }).ToList());
                                model.PointItems.AddRange(response.datas.Select(p => new ChartPointDataModel
                                {
                                    CeqId = p.Ceqid,
                                    Name = p.Name ?? string.Empty,
                                    PointName = "AVERAGE_CURRENT_B",
                                    PointValue = p.AverageCurrentB ?? 0.0f,
                                    UpdateTime = p.SaveTime
                                }).ToList());
                                model.PointItems.AddRange(response.datas.Select(p => new ChartPointDataModel
                                {
                                    CeqId = p.Ceqid,
                                    Name = p.Name ?? string.Empty,
                                    PointName = "AVERAGE_CURRENT_C",
                                    PointValue = p.AverageCurrentC ?? 0.0f,
                                    UpdateTime = p.SaveTime
                                }).ToList());
                                model.PointItems.AddRange(response.datas.Select(p => new ChartPointDataModel
                                {
                                    CeqId = p.Ceqid,
                                    Name = p.Name ?? string.Empty,
                                    PointName = "AVERAGE_CURRENT_N",
                                    PointValue = p.AverageCurrentN ?? 0.0f,
                                    UpdateTime = p.SaveTime
                                }).ToList());

                                model.PointItems.AddRange(response.datas.Select(p => new ChartPointDataModel
                                {
                                    CeqId = p.Ceqid,
                                    Name = p.Name ?? string.Empty,
                                    PointName = "MAX_CURRENT_A",
                                    PointValue = p.MaxCurrentA ?? 0.0f,
                                    UpdateTime = p.SaveTime
                                }).ToList());
                                model.PointItems.AddRange(response.datas.Select(p => new ChartPointDataModel
                                {
                                    CeqId = p.Ceqid,
                                    Name = p.Name ?? string.Empty,
                                    PointName = "MAX_CURRENT_B",
                                    PointValue = p.MaxCurrentB ?? 0.0f,
                                    UpdateTime = p.SaveTime
                                }).ToList());
                                model.PointItems.AddRange(response.datas.Select(p => new ChartPointDataModel
                                {
                                    CeqId = p.Ceqid,
                                    Name = p.Name ?? string.Empty,
                                    PointName = "MAX_CURRENT_C",
                                    PointValue = p.MaxCurrentC ?? 0.0f,
                                    UpdateTime = p.SaveTime
                                }).ToList());
                                model.PointItems.AddRange(response.datas.Select(p => new ChartPointDataModel
                                {
                                    CeqId = p.Ceqid,
                                    Name = p.Name ?? string.Empty,
                                    PointName = "MAX_CURRENT_N",
                                    PointValue = p.MaxCurrentN ?? 0.0f,
                                    UpdateTime = p.SaveTime
                                }).ToList());

                                model.PointItems.AddRange(response.datas.Select(p => new ChartPointDataModel
                                {
                                    CeqId = p.Ceqid,
                                    Name = p.Name ?? string.Empty,
                                    PointName = "MIN_CURRENT_A",
                                    PointValue = p.MinCurrentA ?? 0.0f,
                                    UpdateTime = p.SaveTime
                                }).ToList());
                                model.PointItems.AddRange(response.datas.Select(p => new ChartPointDataModel
                                {
                                    CeqId = p.Ceqid,
                                    Name = p.Name ?? string.Empty,
                                    PointName = "MIN_CURRENT_B",
                                    PointValue = p.MinCurrentB ?? 0.0f,
                                    UpdateTime = p.SaveTime
                                }).ToList());
                                model.PointItems.AddRange(response.datas.Select(p => new ChartPointDataModel
                                {
                                    CeqId = p.Ceqid,
                                    Name = p.Name ?? string.Empty,
                                    PointName = "MIN_CURRENT_C",
                                    PointValue = p.MinCurrentC ?? 0.0f,
                                    UpdateTime = p.SaveTime
                                }).ToList());
                                model.PointItems.AddRange(response.datas.Select(p => new ChartPointDataModel
                                {
                                    CeqId = p.Ceqid,
                                    Name = p.Name ?? string.Empty,
                                    PointName = "MIN_CURRENT_N",
                                    PointValue = p.MinCurrentN ?? 0.0f,
                                    UpdateTime = p.SaveTime
                                }).ToList());
                            }
                            else
                            {
                                DataResultView($"STATISTICS_YEAR 테이블 MSG:{response.Error.Message}");
                                //MessageBox.Show($"STATISTICS_YEAR 테이블 \n\rCODE:{response.Error.Code} MSG:{response.Error.Message}", "데이터 조회", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                        }
                        break;
                    case (int)SearchTypeCode.FIALARM:
                        {
                           
                        }
                        break;
                    case (int)SearchTypeCode.COMMSTATE:
                        {
                            
                        }
                        break;
                    case (int)SearchTypeCode.COMMSTATELOG:
                        {
                           
                        }
                        break;

                }
            }
            catch (Exception ex)
            {
                DataResultView($"차트 데이터 조회 예외발생 ex:{ex.Message}");
            }
        }

        public async void GetSearchData(List<long> ceqList, int type, DateTime fromDate, DateTime toDate)
        {
            var dataModel = App.Current.Services.GetService<DataViewModel>()!;
            if (dataModel == null)
                return;

            try
            {
                switch (type)
                {
                    case (int)SearchTypeCode.MINDATA:
                        {
                            GetSwitchData.Command request = new GetSwitchData.Command
                            {
                                CeqList = ceqList,
                                FromDate = fromDate,
                                ToDate = toDate,
                            };

                            var model = App.Current.Services.GetService<ViewModel_SwitchData>()!;
                            if (model != null)
                            {
                                //model.PointItems = new ObservableCollection<HistoryMinDatum>();
                                model.TabItems = new ObservableCollection<TabData>();
                                model.TotalCount = 0;
                                model.TotalPage = 0;
                                model.UserPage = 0;
                                if (ceqList.Count <= 0)
                                    return;

                                var response = await _mediator.Send(request);
                                if (response != null && response.Result)
                                {
                                    int count = 200;
                                    int index = 1;

                                    model.PointItems = new ObservableCollection<HistoryMinDatum>(response.datas);
                                    model.TotalCount = model.PointItems.Count;
                                    model.TotalPage = (model.PointItems.Count / count) + 1;
                                    model.UserPage = 1;
                                    for (int idx = 0; idx < model.TotalPage; idx++)
                                    {
                                        TabData item = new TabData();
                                        item.Header = idx + 1;

                                        var datas = model.PointItems.Skip(idx * count).Take(count).ToList();
                                        foreach(var data in datas)
                                        {
                                            item.PointItems.Add(new HistoryMinData
                                            {
                                                No = index++,
                                                SaveTime = data.SaveTime,
                                                Ceqid = data.Ceqid,
                                                CommTime = data.CommTime,
                                                Cpsid = data.Cpsid,
                                                Circuitno = data.Circuitno,
                                                Name = data.Name,
                                                Dl = data.Dl,
                                                Diagnostics = data.Diagnostics,
                                                VoltageUnbalance = data.VoltageUnbalance,
                                                CurrentUnbalance = data.CurrentUnbalance,
                                                Frequency = data.Frequency,
                                                CurrentA = data.CurrentA,
                                                CurrentB = data.CurrentB,
                                                CurrentC = data.CurrentC,
                                                CurrentN = data.CurrentN,
                                                VoltageA = data.VoltageA,
                                                VoltageB = data.VoltageB,
                                                VoltageC = data.VoltageC,
                                                ApparentPowerA = data.ApparentPowerA,
                                                ApparentPowerB = data.ApparentPowerB,
                                                ApparentPowerC = data.ApparentPowerC,
                                                PowerFactor3p = data.PowerFactor3p,
                                                PowerFactorA = data.PowerFactorA,
                                                PowerFactorB = data.PowerFactorB,
                                                PowerFactorC = data.PowerFactorC,
                                                FaultCurrentA = data.FaultCurrentA,
                                                FaultCurrentB = data.FaultCurrentB,
                                                FaultCurrentC = data.FaultCurrentC,
                                                FaultCurrentN = data.FaultCurrentN,
                                                CurrentPhaseA = data.CurrentPhaseA,
                                                CurrentPhaseB = data.CurrentPhaseB,
                                                CurrentPhaseC = data.CurrentPhaseC,
                                                CurrentPhaseN = data.CurrentPhaseN,
                                                VoltagePhaseA = data.VoltagePhaseA,
                                                VoltagePhaseB = data.VoltagePhaseB,
                                                VoltagePhaseC = data.VoltagePhaseC
                                            });
                                            //item.PointItems.Add(data); 
                                        }
                                        DispatcherService.Invoke((System.Action)(() => { model.TabItems.Add(item); }));
                                    }

                                    if(model.TabItems.Count > 0)
                                        model.SelectItem = model.TabItems.FirstOrDefault()!;
                                }
                                else
                                {
                                    DataResultView($"HISTORY_MIN_DATA 테이블 MSG:{response.Error.Message}");
                                    //MessageBox.Show($"HISTORY_MIN_DATA 테이블 \n\rCODE:{response.Error.Code} MSG:{response.Error.Message}", "데이터 조회", MessageBoxButton.OK, MessageBoxImage.Information);
                                }
                            }
                        }
                        break;
                    case (int)SearchTypeCode.DAYSTATDATA:
                        {
                            GetDayStatData.Command request = new GetDayStatData.Command
                            {
                                CeqList = ceqList,
                                FromDate = fromDate,
                                ToDate = toDate,
                            };

                            var model = App.Current.Services.GetService<ViewModel_DayStatData>()!;
                            if (model != null)
                            {
                                //model.PointItems = new ObservableCollection<HistoryDaystatData>();
                                model.TabItems = new ObservableCollection<TabData>();
                                model.TotalCount = 0;
                                model.TotalPage = 0;
                                model.UserPage = 0;

                                if (ceqList.Count <= 0)
                                    return;

                                var response = await _mediator.Send(request);
                                if (response != null && response.Result)
                                {
                                    int count = 200;
                                    int index = 1;

                                    model.PointItems = new ObservableCollection<HistoryDaystatDatum>(response.datas);
                                    model.TotalCount = model.PointItems.Count;
                                    model.TotalPage = (model.PointItems.Count / count) + 1;
                                    model.UserPage = 1;

                                    for (int idx = 0; idx < model.TotalPage; idx++)
                                    {
                                        TabData item = new TabData();
                                        item.Header = idx + 1;

                                        var datas = model.PointItems.Skip(idx * count).Take(count).ToList();
                                        foreach (var data in datas)
                                        {
                                            item.PointItems.Add(new HistoryDaystatData
                                            {
                                                No = index++,
                                                SaveTime = data.SaveTime,
                                                Ceqid = data.Ceqid,
                                                CommTime = data.CommTime,
                                                Cpsid = data.Cpsid,
                                                Circuitno = data.Circuitno,
                                                Name = data.Name,
                                                Dl = data.Dl,
                                                Diagnostics = data.Diagnostics,
                                                VoltageUnbalance = data.VoltageUnbalance,
                                                CurrentUnbalance = data.CurrentUnbalance,
                                                Frequency = data.Frequency,
                                                AverageCurrentA = data.AverageCurrentA,
                                                AverageCurrentB = data.AverageCurrentB,
                                                AverageCurrentC = data.AverageCurrentC,
                                                AverageCurrentN = data.AverageCurrentN,
                                                MaxCurrentA = data.MaxCurrentA,
                                                MaxCurrentB = data.MaxCurrentB,
                                                MaxCurrentC = data.MaxCurrentC,
                                                MaxCurrentN = data.MaxCurrentN,
                                                MaxCommTime = data.MaxCommTime,
                                                MinCurrentA = data.MinCurrentA,
                                                MinCurrentB = data.MinCurrentB,
                                                MinCurrentC = data.MinCurrentC,
                                                MinCurrentN = data.MinCurrentN,
                                                MinCommTime = data.MinCommTime
                                            });
                                        }
                                        DispatcherService.Invoke((System.Action)(() => { model.TabItems.Add(item); }));
                                    }

                                    if (model.TabItems.Count > 0)
                                        model.SelectItem = model.TabItems.FirstOrDefault()!;
                                }
                                else
                                {
                                    DataResultView($"HISTORY_DAYSTAT_DATA 테이블 MSG:{response.Error.Message}");
                                    //MessageBox.Show($"HISTORY_DAYSTAT_DATA 테이블 \n\rCODE:{response.Error.Code} MSG:{response.Error.Message}", "데이터 조회", MessageBoxButton.OK, MessageBoxImage.Information);
                                }
                            }
                        }
                        break;
                    case (int)SearchTypeCode.STATISTICSMIN:
                        {
                            GetStatisticsMinData.Command request = new GetStatisticsMinData.Command
                            {
                                CeqList = ceqList,
                                FromDate = fromDate,
                                ToDate = toDate,
                            };

                            var model = App.Current.Services.GetService<ViewModel_StatisticsMinData>()!;
                            if (model != null)
                            {
                                //model.PointItems = new ObservableCollection<Statistics15minData>();
                                model.TabItems = new ObservableCollection<TabData>();
                                model.TotalCount = 0;
                                model.TotalPage = 0;
                                model.UserPage = 0;
                                if (ceqList.Count <= 0)
                                    return;

                                var response = await _mediator.Send(request);
                                if (response != null && response.Result)
                                {
                                    int count = 200;
                                    int index = 1;

                                    model.PointItems = new ObservableCollection<Statistics15min>(response.datas);
                                    model.TotalCount = model.PointItems.Count;
                                    model.TotalPage = (model.PointItems.Count / count) + 1;
                                    model.UserPage = 1;
                                    for (int idx = 0; idx < model.TotalPage; idx++)
                                    {
                                        TabData item = new TabData();
                                        item.Header = idx + 1;

                                        var datas = model.PointItems.Skip(idx * count).Take(count).ToList();
                                        foreach (var data in datas)
                                        {
                                            item.PointItems.Add(new Statistics15minData
                                            {
                                                No = index++,
                                                SaveTime = data.SaveTime,
                                                Ceqid = data.Ceqid,
                                                CommTime = data.CommTime,
                                                Cpsid = data.Cpsid,
                                                Circuitno = data.Circuitno,
                                                Name = data.Name,
                                                Dl = data.Dl,
                                                AverageCurrentA = data.AverageCurrentA,
                                                AverageCurrentB = data.AverageCurrentB,
                                                AverageCurrentC = data.AverageCurrentC,
                                                AverageCurrentN = data.AverageCurrentN
                                            });
                                        }
                                        DispatcherService.Invoke((System.Action)(() => { model.TabItems.Add(item); }));
                                    }

                                    if (model.TabItems.Count > 0)
                                        model.SelectItem = model.TabItems.FirstOrDefault()!;
                                }
                                else
                                {
                                    DataResultView($"STATISTICS_15MIN 테이블 MSG:{response.Error.Message}");
                                    //MessageBox.Show($"STATISTICS_15MIN 테이블 \n\rCODE:{response.Error.Code} MSG:{response.Error.Message}", "데이터 조회", MessageBoxButton.OK, MessageBoxImage.Information);
                                }
                            }
                        }
                        break;
                    case (int)SearchTypeCode.STATISTICSHOUR:
                        {
                            GetStatisticsHourData.Command request = new GetStatisticsHourData.Command
                            {
                                CeqList = ceqList,
                                FromDate = fromDate,
                                ToDate = toDate,
                            };

                            var model = App.Current.Services.GetService<ViewModel_StatisticsHourData>()!;
                            if (model != null)
                            {
                                //model.PointItems = new ObservableCollection<StatisticsHourData>();
                                model.TabItems = new ObservableCollection<TabData>();
                                model.TotalCount = 0;
                                model.TotalPage = 0;
                                model.UserPage = 0;
                                if (ceqList.Count <= 0)
                                    return;

                                var response = await _mediator.Send(request);
                                if (response != null && response.Result)
                                {
                                    int count = 200;
                                    int index = 1;

                                    model.PointItems = new ObservableCollection<StatisticsHour>(response.datas);
                                    model.TotalCount = model.PointItems.Count;
                                    model.TotalPage = (model.PointItems.Count / count) + 1;
                                    model.UserPage = 1;
                                    for (int idx = 0; idx < model.TotalPage; idx++)
                                    {
                                        TabData item = new TabData();
                                        item.Header = idx + 1;

                                        var datas = model.PointItems.Skip(idx * count).Take(count).ToList();
                                        foreach (var data in datas)
                                        {
                                            item.PointItems.Add(new StatisticsHourData
                                            {
                                                No = index++,
                                                SaveTime = data.SaveTime,
                                                Ceqid = data.Ceqid,
                                                Cpsid = data.Cpsid,
                                                Circuitno = data.Circuitno,
                                                Name = data.Name,
                                                Dl = data.Dl,
                                                AverageCurrentA = data.AverageCurrentA,
                                                AverageCurrentB = data.AverageCurrentB,
                                                AverageCurrentC = data.AverageCurrentC,
                                                AverageCurrentN = data.AverageCurrentN,
                                                MaxCurrentA = data.MaxCurrentA,
                                                MaxCurrentB = data.MaxCurrentB,
                                                MaxCurrentC = data.MaxCurrentC,
                                                MaxCurrentN = data.MaxCurrentN,
                                                MaxCommTime = data.MaxCommTime,
                                                MinCurrentA = data.MinCurrentA,
                                                MinCurrentB = data.MinCurrentB,
                                                MinCurrentC = data.MinCurrentC,
                                                MinCurrentN = data.MinCurrentN,
                                                MinCommTime = data.MinCommTime
                                            });
                                            //item.PointItems.Add(data); 
                                        }
                                        DispatcherService.Invoke((System.Action)(() => { model.TabItems.Add(item); }));
                                    }

                                    if (model.TabItems.Count > 0)
                                        model.SelectItem = model.TabItems.FirstOrDefault()!;
                                }
                                else
                                {
                                    DataResultView($"STATISTICS_HOUR 테이블 MSG:{response.Error.Message}");
                                    //MessageBox.Show($"STATISTICS_HOUR 테이블 \n\rCODE:{response.Error.Code} MSG:{response.Error.Message}", "데이터 조회", MessageBoxButton.OK, MessageBoxImage.Information);
                                }
                            }
                        }
                        break;
                    case (int)SearchTypeCode.STATISTICSDAY:
                        {
                            GetStatisticsDayData.Command request = new GetStatisticsDayData.Command
                            {
                                CeqList = ceqList,
                                FromDate = fromDate,
                                ToDate = toDate,
                            };

                            var model = App.Current.Services.GetService<ViewModel_StatisticsDayData>()!;
                            if (model != null)
                            {
                                //model.PointItems = new ObservableCollection<StatisticsDayData>();
                                model.TabItems = new ObservableCollection<TabData>();
                                model.TotalCount = 0;
                                model.TotalPage = 0;
                                model.UserPage = 0;
                                if (ceqList.Count <= 0)
                                    return;

                                var response = await _mediator.Send(request);
                                if (response != null && response.Result)
                                {
                                    int count = 200;
                                    int index = 1;

                                    model.PointItems = new ObservableCollection<StatisticsDay>(response.datas);
                                    model.TotalCount = model.PointItems.Count;
                                    model.TotalPage = (model.PointItems.Count / count) + 1;
                                    model.UserPage = 1;
                                    for (int idx = 0; idx < model.TotalPage; idx++)
                                    {
                                        TabData item = new TabData();
                                        item.Header = idx + 1;

                                        var datas = model.PointItems.Skip(idx * count).Take(count).ToList();
                                        foreach (var data in datas)
                                        {
                                            item.PointItems.Add(new StatisticsDayData
                                            {
                                                No = index++,
                                                SaveTime = data.SaveTime,
                                                Ceqid = data.Ceqid,
                                                Cpsid = data.Cpsid,
                                                Circuitno = data.Circuitno,
                                                Name = data.Name,
                                                Dl = data.Dl,
                                                AverageCurrentA = data.AverageCurrentA,
                                                AverageCurrentB = data.AverageCurrentB,
                                                AverageCurrentC = data.AverageCurrentC,
                                                AverageCurrentN = data.AverageCurrentN,
                                                MaxCurrentA = data.MaxCurrentA,
                                                MaxCurrentB = data.MaxCurrentB,
                                                MaxCurrentC = data.MaxCurrentC,
                                                MaxCurrentN = data.MaxCurrentN,
                                                MaxCommTime = data.MaxCommTime,
                                                MinCurrentA = data.MinCurrentA,
                                                MinCurrentB = data.MinCurrentB,
                                                MinCurrentC = data.MinCurrentC,
                                                MinCurrentN = data.MinCurrentN,
                                                MinCommTime = data.MinCommTime
                                            });
                                            //item.PointItems.Add(data); 
                                        }
                                        DispatcherService.Invoke((System.Action)(() => { model.TabItems.Add(item); }));
                                    }

                                    if (model.TabItems.Count > 0)
                                        model.SelectItem = model.TabItems.FirstOrDefault()!;
                                }
                                else
                                {
                                    DataResultView($"STATISTICS_DAY 테이블 MSG:{response.Error.Message}");
                                    //MessageBox.Show($"STATISTICS_DAY 테이블 \n\rCODE:{response.Error.Code} MSG:{response.Error.Message}", "데이터 조회", MessageBoxButton.OK, MessageBoxImage.Information);
                                }
                            }
                        }
                        break;
                    case (int)SearchTypeCode.STATISTICSMONTH:
                        {
                            GetStatisticsMonthData.Command request = new GetStatisticsMonthData.Command
                            {
                                CeqList = ceqList,
                                FromDate = fromDate,
                                ToDate = toDate,
                            };

                            var model = App.Current.Services.GetService<ViewModel_StatisticsMonthData>()!;
                            if (model != null)
                            {
                                //model.PointItems = new ObservableCollection<StatisticsMonthData>();
                                model.TabItems = new ObservableCollection<TabData>();
                                model.TotalCount = 0;
                                model.TotalPage = 0;
                                model.UserPage = 0;
                                if (ceqList.Count <= 0)
                                    return;

                                var response = await _mediator.Send(request);
                                if (response != null && response.Result)
                                {
                                    int count = 200;
                                    int index = 1;

                                    model.PointItems = new ObservableCollection<StatisticsMonth>(response.datas);
                                    model.TotalCount = model.PointItems.Count;
                                    model.TotalPage = (model.PointItems.Count / count) + 1;
                                    model.UserPage = 1;
                                    for (int idx = 0; idx < model.TotalPage; idx++)
                                    {
                                        TabData item = new TabData();
                                        item.Header = idx + 1;

                                        var datas = model.PointItems.Skip(idx * count).Take(count).ToList();
                                        foreach (var data in datas)
                                        {
                                            item.PointItems.Add(new StatisticsMonthData
                                            {
                                                No = index++,
                                                SaveTime = data.SaveTime,
                                                Ceqid = data.Ceqid,
                                                Cpsid = data.Cpsid,
                                                Circuitno = data.Circuitno,
                                                Name = data.Name,
                                                Dl = data.Dl,
                                                AverageCurrentA = data.AverageCurrentA,
                                                AverageCurrentB = data.AverageCurrentB,
                                                AverageCurrentC = data.AverageCurrentC,
                                                AverageCurrentN = data.AverageCurrentN,
                                                MaxCurrentA = data.MaxCurrentA,
                                                MaxCurrentB = data.MaxCurrentB,
                                                MaxCurrentC = data.MaxCurrentC,
                                                MaxCurrentN = data.MaxCurrentN,
                                                MaxCommTime = data.MaxCommTime,
                                                MinCurrentA = data.MinCurrentA,
                                                MinCurrentB = data.MinCurrentB,
                                                MinCurrentC = data.MinCurrentC,
                                                MinCurrentN = data.MinCurrentN,
                                                MinCommTime = data.MinCommTime
                                            });
                                            //item.PointItems.Add(data); 
                                        }
                                        DispatcherService.Invoke((System.Action)(() => { model.TabItems.Add(item); }));
                                    }

                                    if (model.TabItems.Count > 0)
                                        model.SelectItem = model.TabItems.FirstOrDefault()!;
                                }
                                else
                                {
                                    DataResultView($"STATISTICS_MONTH 테이블 MSG:{response.Error.Message}");
                                    //MessageBox.Show($"STATISTICS_MONTH 테이블 \n\rCODE:{response.Error.Code} MSG:{response.Error.Message}", "데이터 조회", MessageBoxButton.OK, MessageBoxImage.Information);
                                }
                            }
                        }
                        break;
                    case (int)SearchTypeCode.STATISTICSYEAR:
                        {
                            GetStatisticsYearData.Command request = new GetStatisticsYearData.Command
                            {
                                CeqList = ceqList,
                                FromDate = fromDate,
                                ToDate = toDate,
                            };

                            var model = App.Current.Services.GetService<ViewModel_StatisticsYearData>()!;
                            if (model != null)
                            {
                                //model.PointItems = new ObservableCollection<StatisticsYearData>();
                                model.TabItems = new ObservableCollection<TabData>();
                                model.TotalCount = 0;
                                model.TotalPage = 0;
                                model.UserPage = 0;
                                if (ceqList.Count <= 0)
                                    return;

                                var response = await _mediator.Send(request);
                                if (response != null && response.Result)
                                {
                                    int count = 200;
                                    int index = 1;

                                    model.PointItems = new ObservableCollection<StatisticsYear>(response.datas);
                                    model.TotalCount = model.PointItems.Count;
                                    model.TotalPage = (model.PointItems.Count / count) + 1;
                                    model.UserPage = 1;
                                    for (int idx = 0; idx < model.TotalPage; idx++)
                                    {
                                        TabData item = new TabData();
                                        item.Header = idx + 1;

                                        var datas = model.PointItems.Skip(idx * count).Take(count).ToList();
                                        foreach (var data in datas)
                                        {
                                            item.PointItems.Add(new StatisticsYearData
                                            {
                                                No = index++,
                                                SaveTime = data.SaveTime,
                                                Ceqid = data.Ceqid,
                                                Cpsid = data.Cpsid,
                                                Circuitno = data.Circuitno,
                                                Name = data.Name,
                                                Dl = data.Dl,
                                                AverageCurrentA = data.AverageCurrentA,
                                                AverageCurrentB = data.AverageCurrentB,
                                                AverageCurrentC = data.AverageCurrentC,
                                                AverageCurrentN = data.AverageCurrentN,
                                                MaxCurrentA = data.MaxCurrentA,
                                                MaxCurrentB = data.MaxCurrentB,
                                                MaxCurrentC = data.MaxCurrentC,
                                                MaxCurrentN = data.MaxCurrentN,
                                                MaxCommTime = data.MaxCommTime,
                                                MinCurrentA = data.MinCurrentA,
                                                MinCurrentB = data.MinCurrentB,
                                                MinCurrentC = data.MinCurrentC,
                                                MinCurrentN = data.MinCurrentN,
                                                MinCommTime = data.MinCommTime
                                            });
                                            //item.PointItems.Add(data); 
                                        }
                                        DispatcherService.Invoke((System.Action)(() => { model.TabItems.Add(item); }));
                                    }

                                    if (model.TabItems.Count > 0)
                                        model.SelectItem = model.TabItems.FirstOrDefault()!;
                                }
                                else
                                {
                                    DataResultView($"STATISTICS_YEAR 테이블 MSG:{response.Error.Message}");
                                    //MessageBox.Show($"STATISTICS_YEAR 테이블 \n\rCODE:{response.Error.Code} MSG:{response.Error.Message}", "데이터 조회", MessageBoxButton.OK, MessageBoxImage.Information);
                                }
                            }
                        }
                        break;
                    case (int)SearchTypeCode.FIALARM:
                        {
                            GetFiAlarmData.Command request = new GetFiAlarmData.Command
                            {
                                CeqList = ceqList,
                                FromDate = fromDate,
                                ToDate = toDate,
                            };

                            var model = App.Current.Services.GetService<ViewModel_FiAlarmData>()!;
                            if (model != null)
                            {
                                //model.PointItems = new ObservableCollection<HistoryFiAlarmData>();
                                model.TabItems = new ObservableCollection<TabData>();
                                model.TotalCount = 0;
                                model.TotalPage = 0;
                                model.UserPage = 0;
                                if (ceqList.Count <= 0)
                                    return;

                                var response = await _mediator.Send(request);
                                if (response != null && response.Result)
                                {
                                    int count = 200;
                                    int index = 1;

                                    model.PointItems = new ObservableCollection<HistoryFiAlarm>(response.datas);
                                    model.TotalCount = model.PointItems.Count;
                                    model.TotalPage = (model.PointItems.Count / count) + 1;
                                    model.UserPage = 1;
                                    for (int idx = 0; idx < model.TotalPage; idx++)
                                    {
                                        TabData item = new TabData();
                                        item.Header = idx + 1;

                                        var datas = model.PointItems.Skip(idx * count).Take(count).ToList();
                                        foreach (var data in datas)
                                        {
                                            item.PointItems.Add(new HistoryFiAlarmData
                                            {
                                                No = index++,
                                                SaveTime = data.SaveTime,
                                                Ceqid = data.Ceqid,
                                                LogTime = data.LogTime,
                                                FrtuTime = data.FrtuTime,
                                                Cpsid = data.Cpsid,
                                                Circuitno = data.Circuitno,
                                                Name = data.Name,
                                                Dl = data.Dl,
                                                AlarmName = data.AlarmName,
                                                Value = data.Value,
                                                LogDesc = data.LogDesc,
                                                FaultCurrentA = data.FaultCurrentA,
                                                FaultCurrentB = data.FaultCurrentB,
                                                FaultCurrentC = data.FaultCurrentC,
                                                FaultCurrentN = data.FaultCurrentN
                                            });
                                            //item.PointItems.Add(data); 
                                        }
                                        DispatcherService.Invoke((System.Action)(() => { model.TabItems.Add(item); }));
                                    }

                                    if (model.TabItems.Count > 0)
                                        model.SelectItem = model.TabItems.FirstOrDefault()!;
                                }
                                else
                                {
                                    DataResultView($"HISTORY_FI_ALARM 테이블 MSG:{response.Error.Message}");
                                    //MessageBox.Show($"HISTORY_FI_ALARM 테이블 \n\rCODE:{response.Error.Code} MSG:{response.Error.Message}", "데이터 조회", MessageBoxButton.OK, MessageBoxImage.Information);
                                }
                            }
                        }
                        break;
                    case (int)SearchTypeCode.COMMSTATE:
                        {
                            GetCommStateData.Command request = new GetCommStateData.Command
                            {
                                CeqList = ceqList,
                                FromDate = fromDate,
                                ToDate = toDate,
                            };

                            var model = App.Current.Services.GetService<ViewModel_CommDayData>()!;
                            if (model != null)
                            {
                                //model.PointItems = new ObservableCollection<HistoryCommStateData>();
                                model.TabItems = new ObservableCollection<TabData>();
                                model.TotalCount = 0;
                                model.TotalPage = 0;
                                model.UserPage = 0;
                                if (ceqList.Count <= 0)
                                    return;

                                var response = await _mediator.Send(request);
                                if (response != null && response.Result)
                                {
                                    int count = 200;
                                    int index = 1;

                                    model.PointItems = new ObservableCollection<HistoryCommState>(response.datas);
                                    model.TotalCount = model.PointItems.Count;
                                    model.TotalPage = (model.PointItems.Count / count) + 1;
                                    model.UserPage = 1;
                                    for (int idx = 0; idx < model.TotalPage; idx++)
                                    {
                                        TabData item = new TabData();
                                        item.Header = idx + 1;

                                        var datas = model.PointItems.Skip(idx * count).Take(count).ToList();
                                        foreach (var data in datas)
                                        {
                                            item.PointItems.Add(new HistoryCommStateData
                                            {
                                                No = index++,
                                                SaveTime = data.SaveTime,
                                                EqType = data.EqType,
                                                Ceqid = data.Ceqid,
                                                Cpsid = data.Cpsid,
                                                Name = data.Name,
                                                Dl = data.Dl,
                                                CommTotalCount = data.CommTotalCount,
                                                CommSucessCount = data.CommSucessCount,
                                                CommFailCount = data.CommFailCount,
                                                CommSucessRate = data.CommSucessRate,
                                                CommTime = data.CommTime
                                            });
                                            //item.PointItems.Add(data); 
                                        }
                                        DispatcherService.Invoke((System.Action)(() => { model.TabItems.Add(item); }));
                                    }

                                    if (model.TabItems.Count > 0)
                                        model.SelectItem = model.TabItems.FirstOrDefault()!;
                                }
                                else
                                {
                                    DataResultView($"HISTORY_COMM_STATE 테이블 MSG:{response.Error.Message}");
                                    //MessageBox.Show($"HISTORY_COMM_STATE 테이블 \n\rCODE:{response.Error.Code} MSG:{response.Error.Message}", "데이터 조회", MessageBoxButton.OK, MessageBoxImage.Information);
                                }
                            }
                        }
                        break;
                    case (int)SearchTypeCode.COMMSTATELOG:
                        {
                            GetCommStateLogData.Command request = new GetCommStateLogData.Command
                            {
                                CeqList = ceqList,
                                FromDate = fromDate,
                                ToDate = toDate,
                            };

                            var model = App.Current.Services.GetService<ViewModel_CommLogData>()!;
                            if (model != null)
                            {
                                //model.PointItems = new ObservableCollection<HistoryCommStateLogData>();
                                model.TabItems = new ObservableCollection<TabData>();
                                model.TotalCount = 0;
                                model.TotalPage = 0;
                                model.UserPage = 0;
                                if (ceqList.Count <= 0)
                                    return;

                                var response = await _mediator.Send(request);
                                if (response != null && response.Result)
                                {
                                    int count = 200;
                                    int index = 1;

                                    model.PointItems = new ObservableCollection<HistoryCommStateLog>(response.datas);
                                    model.TotalCount = model.PointItems.Count;
                                    model.TotalPage = (model.PointItems.Count / count) + 1;
                                    model.UserPage = 1;
                                    for (int idx = 0; idx < model.TotalPage; idx++)
                                    {
                                        TabData item = new TabData();
                                        item.Header = idx + 1;

                                        var datas = model.PointItems.Skip(idx * count).Take(count).ToList();
                                        foreach (var data in datas)
                                        {
                                            item.PointItems.Add(new HistoryCommStateLogData
                                            {
                                                No = index++,
                                                SaveTime = data.SaveTime,
                                                EqType = data.EqType,
                                                Ceqid = data.Ceqid,
                                                Cpsid = data.Cpsid,
                                                Name = data.Name,
                                                Dl = data.Dl,
                                                CommState = data.CommState,
                                                CommTotalCount = data.CommTotalCount,
                                                CommSucessCount = data.CommSucessCount,
                                                CommFailCount = data.CommFailCount,
                                                CommSucessRate = data.CommSucessRate,
                                                CommTime = data.CommTime
                                            });
                                            //item.PointItems.Add(data); 
                                        }
                                        DispatcherService.Invoke((System.Action)(() => { model.TabItems.Add(item); }));
                                    }

                                    if (model.TabItems.Count > 0)
                                        model.SelectItem = model.TabItems.FirstOrDefault()!;
                                }
                                else
                                {
                                    DataResultView($"HISTORY_COMM_STATE_LOG 테이블 MSG:{response.Error.Message}");
                                    //MessageBox.Show($"HISTORY_COMM_STATE_LOG 테이블 \n\rCODE:{response.Error.Code} MSG:{response.Error.Message}", "데이터 조회", MessageBoxButton.OK, MessageBoxImage.Information);
                                }
                            }
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                DataResultView($"실시간/통계 데이터 조회 예외발생 ex:{ex.Message}");
                //MessageBox.Show(ex.Message);
            }
        }

        public void DataResultView(string msg, string title = "데이터 조회")
        {
            DispatcherService.Invoke((System.Action)(() => 
            {
                Mouse.OverrideCursor = Cursors.Arrow;

                MessageClickView view = new MessageClickView();
                view.Title = $" {title}";
                view.Result.Text = msg;
                view.ShowDialog();
            }));
        }

        public MessageBoxResult DataYesNoView(string msg, string title = "데이터 조회")
        {
            MessageBoxResult mbr = MessageBoxResult.No;
            DispatcherService.Invoke((System.Action)(() =>
            {
                Mouse.OverrideCursor = Cursors.Arrow;

                MessageYesNoView view = new MessageYesNoView();
                view.Title = $" {title}";
                view.Result.Text = msg;
                if (view.ShowDialog() == true)
                    mbr = MessageBoxResult.Yes;
                else
                    mbr = MessageBoxResult.No;
            }));
            return mbr;
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
