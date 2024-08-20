using CommunityToolkit.Mvvm.ComponentModel;
using KDMSViewer.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace KDMSViewer.Model
{
    public class TreeDataModel : ObservableObject
    {
        private ViewTypeCode ViewType_;
        public ViewTypeCode ViewType
        {
            get { return ViewType_; }
            set
            {
                ViewType_ = value;
                OnPropertyChanged(nameof(ViewType));
            }
        }

        public TreeTypeCode Type { get; set; }
        //public long SubsId { get; set; } 
        //public string SubsName { get; set; } = string.Empty;
        //public long DlId { get; set; } 
        //public string DlName { get; set; } = string.Empty;
        //public long SwId { get; set; }
        //public string SwName { get; set; } = string.Empty;
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Tooltip { get; set; } = string.Empty;
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
        public bool IsExpanded { get; set; }
        public bool IsSelected { get; set; }

        public Visibility IsVisible { get; set; }

        public ObservableCollection<TreeDataModel> DataModels { get; set; } = new ObservableCollection<TreeDataModel>();


        public void OnChecked(object sender, MouseButtonEventArgs e)
        {
            if (Type != TreeTypeCode.EQUIPMENT)
                return;

            if (e.ClickCount == 2)
            {
                if(ViewType == ViewTypeCode.DataView)
                {
                    if (IsChecked)
                        IsChecked = false;
                    else
                        IsChecked = true;
                }
                else
                {
                    var model = App.Current.Services.GetService<TrandViewModel>();
                    if(model != null)
                    {
                        model.ChartClear();
                    }
                }
                
            }
        }

        //체크박스 이벤트 처리
        public void OnCheckBoxChecked(object sender, RoutedEventArgs e)
        {
            foreach (var item in DataModels)
            {
                item.IsChecked = true;
            }
        }

        public void OnCheckBoxUnChecked(object sender, RoutedEventArgs e)
        {
            foreach (var item in DataModels)
            {
                item.IsChecked = false;
            }
        }
    }
    //public class TreeCheckItemModel
    //{
    //    public TreeTypeCode Type { get; set; }
    //    public long ID { get; set; }
    //    public string Name { get; set; } = string.Empty;
    //    public long CpsID { get; set; }
    //    public long CeqID { get; set; }
    //    public bool IsEnd { get; set; }
    //}

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
