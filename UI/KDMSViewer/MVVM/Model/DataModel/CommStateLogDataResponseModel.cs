using KDMS.EF.Core.Infrastructure.Reverse.Models;
using KDMSViewer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDMSViewer.Model
{
    public class CommStateLogDataResponseModel : BaseResponse
    {
        public List<HistoryCommStateLog> datas { get; set; }
    }
}
