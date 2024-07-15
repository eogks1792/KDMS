using System.Linq;
using System.Windows.Controls;
using System.Windows;
using Microsoft.Xaml.Behaviors;

namespace KDMSViewer.Model
{
    public class TreeViewSelectedItemBehavior : Behavior<TreeView>
    {
        #region SelectedItem Property

        public object SelectedItem
        {
            get { return (object)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(object), typeof(TreeViewSelectedItemBehavior), new UIPropertyMetadata(null, OnSelectedItemChanged));

        private static void OnSelectedItemChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var behavior = sender as TreeViewSelectedItemBehavior;

            if (behavior == null) return;

            var tree = behavior.AssociatedObject;

            if (tree == null) return;

            if (e.NewValue == null)
                foreach (var item in tree.Items.OfType<TreeViewItem>())
                    item.SetValue(TreeViewItem.IsSelectedProperty, false);

            var treeViewItem = e.NewValue as TreeViewItem;
            if (treeViewItem != null)
            {
                treeViewItem.SetValue(TreeViewItem.IsSelectedProperty, true);
            }
            else
            {
                var itemsHostProperty = tree.GetType().GetProperty("ItemsHost", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

                if (itemsHostProperty == null) return;

                var itemsHost = itemsHostProperty.GetValue(tree, null) as Panel;

                if (itemsHost == null) return;

                foreach (var item in itemsHost.Children.OfType<TreeViewItem>())
                    if (WalkTreeViewItem(item, e.NewValue)) break;
            }
        }

        public static bool WalkTreeViewItem(TreeViewItem treeViewItem, object selectedValue)
        {
            if (treeViewItem.DataContext == selectedValue)
            {
                treeViewItem.SetValue(TreeViewItem.IsSelectedProperty, true);
                //treeViewItem.IsExpanded = true;
                treeViewItem.Focus();
                return true;
            }

            foreach (var item in treeViewItem.Items)
            {
                if(item == selectedValue)
                {
                    treeViewItem.IsExpanded = true;
                    return true;
                }
            }

            foreach (var item in treeViewItem.Items.OfType<TreeViewItem>())
            {
                if (WalkTreeViewItem(item, selectedValue)) 
                    return true;
            }

            return false;
        }

        #endregion

        protected override void OnAttached()
        {
            base.OnAttached();

            this.AssociatedObject.SelectedItemChanged += OnTreeViewSelectedItemChanged;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            if (this.AssociatedObject != null)
            {
                this.AssociatedObject.SelectedItemChanged -= OnTreeViewSelectedItemChanged;
            }
        }

        private void OnTreeViewSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            this.SelectedItem = e.NewValue;
        }

        
    }
}
