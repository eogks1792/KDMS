using CommunityToolkit.Mvvm.ComponentModel;
using KDMS.EF.Core.Infrastructure.Reverse.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace KDMSViewer.Model
{
    public class AiItem : ObservableObject
    {
        /// <summary>
        /// 번호
        /// </summary>
        public int No { get; set; }

        /// <summary>
        /// MIN_DATA 컬럼 이름
        /// </summary>
        public string Columnname { get; set; } = null!;

        /// <summary>
        /// DATAPOINT 아이디
        /// </summary>
        public int? Datapointid { get; set; }

        /// <summary>
        /// DATAPOINT 이름
        /// </summary>
        private string? Datapointname_;
        public string? Datapointname
        {
            get { return Datapointname_; }
            set
            {
                Datapointname_ = value;
                OnPropertyChanged(nameof(Datapointname));
            }
        }

        public List<Datapointinfo> Datapointinfos { get; set; }

        public void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var find = Datapointinfos.FirstOrDefault(p => p.Datapointid == Datapointid);
            if(find != null)
                Datapointname = find.Name;
        }

    }
}
