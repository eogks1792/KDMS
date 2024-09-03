using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors.Helpers;
using KDMS.EF.Core.Infrastructure.Reverse;
using KDMS.EF.Core.Infrastructure.Reverse.Models;
using KDMSServer.Model;
using MediatR;
using Serilog;
using System.Linq;
using System.ServiceModel.Channels;
using System.Windows;
using System.Windows.Media;

namespace KDMSServer.Features
{
    public class DaystatDataCalculation
    {
        public class Command : BaseRequest, IRequest<Response>
        {
        }

        public class Response : BaseResponse
        {

        }

        public class CommandHandler : IRequestHandler<Command, Response>
        {
            private readonly CommonDataModel _commonData;
            private readonly ILogger _logger;

            public CommandHandler(CommonDataModel commonData, ILogger logger)
            {
                _commonData = commonData;
                _logger = logger;
            }

            public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
            {
                var response = new Response { Result = false };
                try
                {
                    var datas = _commonData.MinDataLoad(request.Time);
                    if (datas != null && datas.Count > 0)
                    {
                        List<HistoryDaystatDatum> dataList = new List<HistoryDaystatDatum>();
                        var ceqList = datas.Select(p => p.Ceqid).Distinct().ToList();
                        foreach (var ceq in ceqList)
                        {
                            var findDatas = datas.Where(p => p.Ceqid == ceq).ToList();
                            if (findDatas.Count <= 0)
                                continue;

                            var avg = findDatas.GroupBy(p => p.Ceqid).Select(p => new
                            {
                                CurrentA = p.Average(x => x.CurrentA),
                                CurrentB = p.Average(x => x.CurrentB),
                                CurrentC = p.Average(x => x.CurrentC),
                                CurrentN = p.Average(x => x.CurrentN)
                            });

                            var sum = findDatas.GroupBy(g => g.CommTime).Select(p => new
                            {
                                key = p.Key,
                                sum = p.Sum(x => x.CurrentA + x.CurrentB + x.CurrentC),
                                items = p.ToList()
                            }).ToList();

                            var inputData = findDatas.Select(p => new HistoryDaystatDatum
                            {
                                SaveTime = p.SaveTime,
                                Ceqid = p.Ceqid,
                                CommTime = p.CommTime,
                                Cpsid = p.Cpsid,
                                Circuitno = p.Circuitno,
                                Name = p.Name,
                                Dl = p.Dl,
                                Diagnostics = p.Diagnostics,
                                VoltageUnbalance = p.VoltageUnbalance,
                                CurrentUnbalance = p.CurrentUnbalance,
                                Frequency = p.Frequency,
                                AverageCurrentA = avg.FirstOrDefault()!.CurrentA,
                                AverageCurrentB = avg.FirstOrDefault()!.CurrentB,
                                AverageCurrentC = avg.FirstOrDefault()!.CurrentC,
                                AverageCurrentN = avg.FirstOrDefault()!.CurrentN,
                                //MaxCurrentA = sum.MaxBy(p => p.sum).items.LastOrDefault()!.CurrentA,
                                //MaxCurrentB = sum.MaxBy(p => p.sum).items.LastOrDefault()!.CurrentB,
                                //MaxCurrentC = sum.MaxBy(p => p.sum).items.LastOrDefault()!.CurrentC,
                                //MaxCurrentN = sum.MaxBy(p => p.sum).items.LastOrDefault()!.CurrentN,
                                //MaxCommTime = sum.MaxBy(p => p.sum).items.LastOrDefault()!.CommTime,
                                MaxCurrentA = sum.OrderByDescending(p => p.key).MaxBy(p => p.sum).items.First().CurrentA,
                                MaxCurrentB = sum.OrderByDescending(p => p.key).MaxBy(p => p.sum).items.First().CurrentB,
                                MaxCurrentC = sum.OrderByDescending(p => p.key).MaxBy(p => p.sum).items.First().CurrentC,
                                MaxCurrentN = sum.OrderByDescending(p => p.key).MaxBy(p => p.sum).items.First().CurrentN,
                                MaxCommTime = sum.OrderByDescending(p => p.key).MaxBy(p => p.sum).items.First().CommTime,
                                //MinCurrentA = sum.MinBy(p => p.sum).items.FirstOrDefault()!.CurrentA,
                                //MinCurrentB = sum.MinBy(p => p.sum).items.FirstOrDefault()!.CurrentB,
                                //MinCurrentC = sum.MinBy(p => p.sum).items.FirstOrDefault()!.CurrentC,
                                //MinCurrentN = sum.MinBy(p => p.sum).items.FirstOrDefault()!.CurrentN,
                                //MinCommTime = sum.MinBy(p => p.sum).items.FirstOrDefault()!.CommTime,
                                MinCurrentA = sum.OrderByDescending(p => p.key).MinBy(p => p.sum).items.First().CurrentA,
                                MinCurrentB = sum.OrderByDescending(p => p.key).MinBy(p => p.sum).items.First().CurrentB,
                                MinCurrentC = sum.OrderByDescending(p => p.key).MinBy(p => p.sum).items.First().CurrentC,
                                MinCurrentN = sum.OrderByDescending(p => p.key).MinBy(p => p.sum).items.First().CurrentN,
                                MinCommTime = sum.OrderByDescending(p => p.key).MinBy(p => p.sum).items.First().CommTime
                            }).LastOrDefault();

                            dataList.Add(inputData);
                        }

                        if(dataList.Count > 0)
                            response.Result = _commonData.DaystatDataInput(dataList, request.Time);
                    }
                    else
                    {
                        response.Result = false;
                        response.Error = new Error
                        {
                            Code = "01",
                            Message = "DATA 없음"
                        };
                    }
                }
                catch (Exception ex)
                {
                    response.Error = new Error
                    {
                        Code = "02",
                        Message = ex.Message
                    };
                }

                return await Task.FromResult(response);
            }
        }
    }
}
