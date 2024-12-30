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
    public class StatisticsHourCalculation
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
                    var datas = _commonData.StatisticsMinDataLoad(request.Time);
                    if (datas != null && datas.Count > 0)
                    {
                        List<StatisticsHour> dataList = new List<StatisticsHour>();
                        var ceqList = datas.Select(p => p.Ceqid).Distinct().ToList();
                        foreach (var ceq in ceqList)
                        {
                            var findDatas = datas.Where(p => p.Ceqid == ceq).ToList();
                            if (findDatas.Count <= 0)
                                continue;

                            var avg = findDatas.GroupBy(p => p.Ceqid).Select(p => new
                            {
                                AverageCurrentA = p.Average(x => x.AverageCurrentA),
                                AverageCurrentB = p.Average(x => x.AverageCurrentB),
                                AverageCurrentC = p.Average(x => x.AverageCurrentC),
                                AverageCurrentN = p.Average(x => x.AverageCurrentN)
                            });

                            var sum = findDatas.GroupBy(g => g.CommTime).Select(p => new
                            {
                                key = p.Key,
                                sum = p.Sum(x => x.AverageCurrentA + x.AverageCurrentB + x.AverageCurrentC),
                                items = p.ToList()
                            }).ToList();

                            var inputData = findDatas.Select(p => new StatisticsHour
                            {
                                SaveTime = request.Time.AddHours(1),
                                Ceqid = p.Ceqid,
                                Cpsid = p.Cpsid,
                                Circuitno = p.Circuitno,
                                Name = p.Name,
                                Dl = p.Dl,
                                AverageCurrentA = avg.FirstOrDefault()!.AverageCurrentA,
                                AverageCurrentB = avg.FirstOrDefault()!.AverageCurrentB,
                                AverageCurrentC = avg.FirstOrDefault()!.AverageCurrentC,
                                AverageCurrentN = avg.FirstOrDefault()!.AverageCurrentN,
                                MaxCurrentA = sum.OrderByDescending(p => p.key).MaxBy(p => p.sum).items.First().AverageCurrentA,
                                MaxCurrentB = sum.OrderByDescending(p => p.key).MaxBy(p => p.sum).items.First().AverageCurrentB,
                                MaxCurrentC = sum.OrderByDescending(p => p.key).MaxBy(p => p.sum).items.First().AverageCurrentC,
                                MaxCurrentN = sum.OrderByDescending(p => p.key).MaxBy(p => p.sum).items.First().AverageCurrentN,
                                MaxCommTime = sum.OrderByDescending(p => p.key).MaxBy(p => p.sum).items.First().CommTime,
                                MinCurrentA = sum.OrderByDescending(p => p.key).MinBy(p => p.sum).items.First().AverageCurrentA,
                                MinCurrentB = sum.OrderByDescending(p => p.key).MinBy(p => p.sum).items.First().AverageCurrentB,
                                MinCurrentC = sum.OrderByDescending(p => p.key).MinBy(p => p.sum).items.First().AverageCurrentC,
                                MinCurrentN = sum.OrderByDescending(p => p.key).MinBy(p => p.sum).items.First().AverageCurrentN,
                                MinCommTime = sum.OrderByDescending(p => p.key).MinBy(p => p.sum).items.First().CommTime
                            }).LastOrDefault();

                            dataList.Add(inputData);
                        }

                        if (dataList.Count > 0)
                            _commonData.StatisticsHourDataInput(dataList);

                        response.Result = true;
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
