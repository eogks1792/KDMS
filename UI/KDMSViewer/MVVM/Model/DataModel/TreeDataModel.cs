using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace KDMSViewer.Model
{
    public class TreeDataModel : ObservableObject
    {
        public TreeTypeCode Type { get; set; }
        //public long SubsId { get; set; } 
        //public string SubsName { get; set; } = string.Empty;
        //public long DlId { get; set; } 
        //public string DlName { get; set; } = string.Empty;
        //public long SwId { get; set; }
        //public string SwName { get; set; } = string.Empty;
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        private bool IsChecked_;
        public bool IsChecked
        {
            get { return IsChecked_; }
            set
            {
                IsChecked_ = value;
                OnPropertyChanged(nameof(IsChecked));
            }
        }

        public Visibility IsVisible { get; set; }

        public ObservableCollection<TreeDataModel> DataModels { get; set; } = new ObservableCollection<TreeDataModel>();

        // 체크박스 이벤트 처리
        public void OnChecked(object sender, RoutedEventArgs e)
        {
            foreach (var item in DataModels)
            {
                item.IsChecked = true;
            }
        }

        public void OnUnchecked(object sender, RoutedEventArgs e)
        {
            foreach (var item in DataModels)
            {
                item.IsChecked = false;
            }
        }
    }
    public class TreeCheckItemModel
    {
        public TreeTypeCode Type { get; set; }
        public long ID { get; set; }
        public string Name { get; set; } = string.Empty;
        public long CpsID { get; set; }
        public long CeqID { get; set; }
        public bool IsEnd { get; set; }
    }

    //public class TreeItemModel : ObservableObject
    //{
    //    public string Name { get; set; } = string.Empty;
    //    private bool IsChecked_;
    //    public bool IsChecked
    //    {
    //        get { return IsChecked_; }
    //        set
    //        {
    //            IsChecked_ = value;
    //            OnPropertyChanged(nameof(IsChecked));
    //        }
    //    }
    //    public ObservableCollection<TreeItemModel> ChildModels { get; set; } = new ObservableCollection<TreeItemModel>();
    //}
}
