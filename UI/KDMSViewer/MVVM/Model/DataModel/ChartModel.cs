using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDMSViewer.Model
{
    public class ChartModel
    {
        public string Name { get; set; }
        public List<ChartDateModel> Datas { get; set; } = new List<ChartDateModel>();
    }

    public class ChartDateModel
    {
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public float Value { get; set; }
    }

    public class ChartPointDataModel : ObservableObject
    {
        public long CeqId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string PointName { get; set; } = string.Empty;
        public float PointValue { get; set; }
        public DateTime UpdateTime { get; set; }
    }
}
