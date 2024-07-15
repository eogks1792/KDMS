using CommunityToolkit.Mvvm.ComponentModel;
using KDMS.EF.Core.Infrastructure.Reverse.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDMSViewer.Model
{
    public class PointDataModel : ObservableObject
    {
        /// <summary>
        /// 포인트 아이디
        /// </summary>
        public int PointId { get; set; }

        /// <summary>
        /// 포인트 이름
        /// </summary>
        public string PointName { get; set; } = null!;

        /// <summary>
        /// Alarm Catagory ID
        /// </summary>
        public int? Alarmcategoryfk { get; set; }

        /// <summary>
        /// 포인트 사용여부
        /// </summary>
        private bool UseYn_;
        public bool UseYn
        {
            get { return UseYn_; }
            set
            {
                UseYn_ = value;
                OnPropertyChanged(nameof(UseYn));
            }
        }
        public List<PointUseynModel> PointUseyns { get; set; }
    }

    public class PointUseynModel : ObservableObject
    {
        public bool IsUseyn { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
