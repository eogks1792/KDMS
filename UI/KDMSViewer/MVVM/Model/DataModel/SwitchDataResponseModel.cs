using KDMS.EF.Core.Infrastructure.Reverse.Models;
using KDMSViewer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDMSViewer.Model
{
    public class SwitchDataResponseModel : BaseResponse
    {
        public List<HistoryMinDatum> datas { get; set; }
    }
}
