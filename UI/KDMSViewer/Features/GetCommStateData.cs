using DevExpress.Mvvm;
using DevExpress.Utils.Serializing;
using KDMSViewer.Model;
using MediatR;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Net;
using System.Security.Cryptography.Pkcs;
using System.Text;
using System.Threading.Tasks;

namespace KDMSViewer.Features
{
    public class GetCommStateData
    {
        public class Command : BaseRequest, IRequest<Response>
        {
        }

        public class Response : CommStateDataResponseModel
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
                    var datas = _commonData.CommStateDataLoad(request.CeqList, request.FromDate, request.ToDate);
                    if(datas != null && datas.Count > 0) 
                    {
                        response.datas = datas;
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
