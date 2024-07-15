using KDMS.EF.Core.Infrastructure.Reverse.Models;

namespace KDMSServer.Model
{
    public class SwitchDataResponseModel : BaseResponse
    {
        public List<HistoryMinDatum> datas { get; set; }
    }
}
