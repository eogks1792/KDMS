using DevExpress.ClipboardSource.SpreadsheetML;
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
using System.Windows.Media.Animation;
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

        public async void GetTrandData(List<long> ceqList, int type, DateTime fromDate, DateTime toDate, DateTime fromTime = new DateTime(), DateTime toTime = new DateTime())
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
                                FromTime = fromTime,
                                ToTime = toTime
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
                                MessageBox.Show($"HISTORY_MIN_DATA 테이블 \n\rCODE:{response.Error.Code} MSG:{response.Error.Message}", "데이터 조회", MessageBoxButton.OK, MessageBoxImage.Information);
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
                                MessageBox.Show($"HISTORY_DAYSTAT_DATA 테이블 \n\rCODE:{response.Error.Code} MSG:{response.Error.Message}", "데이터 조회", MessageBoxButton.OK, MessageBoxImage.Information);
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
                                MessageBox.Show($"STATISTICS_15MIN 테이블 \n\rCODE:{response.Error.Code} MSG:{response.Error.Message}", "데이터 조회", MessageBoxButton.OK, MessageBoxImage.Information);
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
                                MessageBox.Show($"STATISTICS_HOUR 테이블 \n\rCODE:{response.Error.Code} MSG:{response.Error.Message}", "데이터 조회", MessageBoxButton.OK, MessageBoxImage.Information);
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
                                MessageBox.Show($"STATISTICS_DAY 테이블 \n\rCODE:{response.Error.Code} MSG:{response.Error.Message}", "데이터 조회", MessageBoxButton.OK, MessageBoxImage.Information);
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
                                MessageBox.Show($"STATISTICS_MONTH 테이블 \n\rCODE:{response.Error.Code} MSG:{response.Error.Message}", "데이터 조회", MessageBoxButton.OK, MessageBoxImage.Information);
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
                                MessageBox.Show($"STATISTICS_YEAR 테이블 \n\rCODE:{response.Error.Code} MSG:{response.Error.Message}", "데이터 조회", MessageBoxButton.OK, MessageBoxImage.Information);
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
                MessageBox.Show(ex.Message);
            }
        }

        public async void GetSearchData(List<long> ceqList, int type, DateTime fromDate, DateTime toDate, DateTime fromTime = new DateTime(), DateTime toTime = new DateTime())
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
                                FromTime = fromTime,
                                ToTime = toTime
                            };

                            var model = App.Current.Services.GetService<ViewModel_SwitchData>()!;
                            if (model != null)
                            {
                                model.PointItems.Clear();
                                var response = await _mediator.Send(request);
                                if (response != null && response.Result)
                                {
                                    DispatcherService.Invoke((System.Action)(() =>
                                    {
                                        model.PointItems = response.datas;
                                    }));
                                }
                                else
                                {
                                    MessageBox.Show($"HISTORY_MIN_DATA 테이블 \n\rCODE:{response.Error.Code} MSG:{response.Error.Message}", "데이터 조회", MessageBoxButton.OK, MessageBoxImage.Information);
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
                                ToDate = toDate
                            };

                            var model = App.Current.Services.GetService<ViewModel_DayStatData>()!;
                            if (model != null)
                            {
                                model.PointItems.Clear();
                                var response = await _mediator.Send(request);
                                if (response != null && response.Result)
                                {
                                    Application.Current.Dispatcher.Invoke(() => { model.PointItems = response.datas; });
                                }
                                else
                                {
                                    MessageBox.Show($"HISTORY_DAYSTAT_DATA 테이블 \n\rCODE:{response.Error.Code} MSG:{response.Error.Message}", "데이터 조회", MessageBoxButton.OK, MessageBoxImage.Information);
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
                                ToDate = toDate
                            };

                            var model = App.Current.Services.GetService<ViewModel_StatisticsMinData>()!;
                            if (model != null)
                            {
                                model.PointItems.Clear();
                                var response = await _mediator.Send(request);
                                if (response != null && response.Result)
                                {
                                    Application.Current.Dispatcher.Invoke(() => { model.PointItems = response.datas; });
                                }
                                else
                                {
                                    MessageBox.Show($"STATISTICS_15MIN 테이블 \n\rCODE:{response.Error.Code} MSG:{response.Error.Message}", "데이터 조회", MessageBoxButton.OK, MessageBoxImage.Information);
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
                                ToDate = toDate
                            };

                            var model = App.Current.Services.GetService<ViewModel_StatisticsHourData>()!;
                            if (model != null)
                            {
                                model.PointItems.Clear();
                                var response = await _mediator.Send(request);
                                if (response != null && response.Result)
                                {
                                    Application.Current.Dispatcher.Invoke(() => { model.PointItems = response.datas; });
                                }
                                else
                                {
                                    MessageBox.Show($"STATISTICS_HOUR 테이블 \n\rCODE:{response.Error.Code} MSG:{response.Error.Message}", "데이터 조회", MessageBoxButton.OK, MessageBoxImage.Information);
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
                                ToDate = toDate
                            };

                            var model = App.Current.Services.GetService<ViewModel_StatisticsDayData>()!;
                            if (model != null)
                            {
                                model.PointItems.Clear();
                                var response = await _mediator.Send(request);
                                if (response != null && response.Result)
                                {
                                    Application.Current.Dispatcher.Invoke(() => { model.PointItems = response.datas; });
                                }
                                else
                                {
                                    MessageBox.Show($"STATISTICS_DAY 테이블 \n\rCODE:{response.Error.Code} MSG:{response.Error.Message}", "데이터 조회", MessageBoxButton.OK, MessageBoxImage.Information);
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
                                ToDate = toDate
                            };

                            var model = App.Current.Services.GetService<ViewModel_StatisticsMonthData>()!;
                            if (model != null)
                            {
                                model.PointItems.Clear();
                                var response = await _mediator.Send(request);
                                if (response != null && response.Result)
                                {
                                    Application.Current.Dispatcher.Invoke(() => { model.PointItems = response.datas; });
                                }
                                else
                                {
                                    MessageBox.Show($"STATISTICS_MONTH 테이블 \n\rCODE:{response.Error.Code} MSG:{response.Error.Message}", "데이터 조회", MessageBoxButton.OK, MessageBoxImage.Information);
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
                                ToDate = toDate
                            };

                            var model = App.Current.Services.GetService<ViewModel_StatisticsYearData>()!;
                            if (model != null)
                            {
                                model.PointItems.Clear();
                                var response = await _mediator.Send(request);
                                if (response != null && response.Result)
                                {
                                    Application.Current.Dispatcher.Invoke(() => { model.PointItems = response.datas; });
                                }
                                else
                                {
                                    MessageBox.Show($"STATISTICS_YEAR 테이블 \n\rCODE:{response.Error.Code} MSG:{response.Error.Message}", "데이터 조회", MessageBoxButton.OK, MessageBoxImage.Information);
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
                                ToDate = toDate
                            };

                            var model = App.Current.Services.GetService<ViewModel_FiAlarmData>()!;
                            if (model != null)
                            {
                                model.PointItems.Clear();
                                var response = await _mediator.Send(request);
                                if (response != null && response.Result)
                                {
                                    Application.Current.Dispatcher.Invoke(() => { model.PointItems = response.datas; });
                                }
                                else
                                {
                                    MessageBox.Show($"HISTORY_MIN_DATA 테이블 \n\rCODE:{response.Error.Code} MSG:{response.Error.Message}", "데이터 조회", MessageBoxButton.OK, MessageBoxImage.Information);
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
                                ToDate = toDate
                            };

                            var model = App.Current.Services.GetService<ViewModel_CommDayData>()!;
                            if (model != null)
                            {
                                model.PointItems.Clear();
                                var response = await _mediator.Send(request);
                                if (response != null && response.Result)
                                {
                                    Application.Current.Dispatcher.Invoke(() => { model.PointItems = response.datas; });
                                }
                                else
                                {
                                    MessageBox.Show($"HISTORY_COMM_STATE 테이블 \n\rCODE:{response.Error.Code} MSG:{response.Error.Message}", "데이터 조회", MessageBoxButton.OK, MessageBoxImage.Information);
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
                                ToDate = toDate
                            };

                            var model = App.Current.Services.GetService<ViewModel_CommLogData>()!;
                            if (model != null)
                            {
                                model.PointItems.Clear();
                                var response = await _mediator.Send(request);
                                if (response != null && response.Result)
                                {
                                    Application.Current.Dispatcher.Invoke(() => { model.PointItems = response.datas; });
                                }
                                else
                                {
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
                        //var biModel = App.Current.Services.GetService<OperationBiViewModel>()!;
                        //if (biModel != null)
                        //{
                        //    mainModel.BiCount = biModel.DataCount;
                        //}

                        //// AI 연계 데이터
                        //var aiModel = App.Current.Services.GetService<OperationAiViewModel>()!;
                        //if (aiModel != null)
                        //{
                        //    mainModel.AiCount = aiModel.DataCount;
                        //}

                        //// 알람 연계 데이터
                        //var alarmModel = App.Current.Services.GetService<OperationAlarmViewModel>()!;
                        //if (alarmModel != null)
                        //{
                        //    mainModel.AlarmCount = alarmModel.DataCount;
                        //}

                        // 스케줄 정보
                        //var schduleModel = App.Current.Services.GetService<OperationSchduleViewModel>()!;
                        //if (schduleModel != null)
                        //{
                        //    mainModel.ScheduleMeasureBIBO = $" - BI 계측: {schduleModel.biTime} / BO 계측: {schduleModel.boTime}";
                        //    //mainModel.ScheduleMeasureBO = $" - BO 계측: {schduleModel.boTime} (분)";
                        //    mainModel.ScheduleMeasureAIAO = $" - AI 계측: {schduleModel.aiTime} / AO 계측: {schduleModel.aoTime}";
                        //    //mainModel.ScheduleMeasureAO = $" - AO 계측: {schduleModel.aoTime} (분)";
                        //    mainModel.ScheduleMeasureCounter = $" - CNT 계측: {schduleModel.counterTime}";
                        //    mainModel.StoragePeriodStatistics = $" - 통계생성: {schduleModel.statisticalTime.ToString("HH:mm:ss")}";
                        //}

                        // 보관주기 정보
                        //var storageModel = App.Current.Services.GetService<OperationStorageViewModel>()!;
                        //if (storageModel != null)
                        //{
                        //    //mainModel.StoragePeriodData = $" - 데이터: {storageModel.GetStorage()}";
                        //    mainModel.SPRealMinData = $" - MIN_DATA : {storageModel.hisMinTime}";
                        //    mainModel.SPDayStatData = $" - DAYSTAT_DATA : {storageModel.hisStatTime}";
                        //    mainModel.SPStatMinData = $" - STATISTICS_15MIN : {storageModel.statMinTime}";
                        //    mainModel.SPStatHourData = $" - STATISTICS_HOUR : {storageModel.statHourTime}";
                        //    mainModel.SPStatDayData = $" - STATISTICS_DAY : {storageModel.statDayTime}";
                        //    mainModel.SPStatMonthData = $" - STATISTICS_MONTH : {storageModel.statMonthTime}";
                        //    mainModel.SPStatYearData = $" - STATISTICS_YEAR : {storageModel.statYearTime}";
                        //    mainModel.SPFiData = $" - FI_ALARM : {storageModel.hisFiTime}";
                        //    mainModel.SPCommData = $" - COMM_STATE : {storageModel.hisCommTime}";
                        //    mainModel.SPCommLogData = $" - COMM_STATE_LOG : {storageModel.hisCommLogTime}";

                        //    //mainModel.ScheduleDelete = $" - 삭제주기: {storageModel.deleteTime}";
                        //}
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
