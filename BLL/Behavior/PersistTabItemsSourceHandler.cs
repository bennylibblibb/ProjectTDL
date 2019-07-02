using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using System.Collections.Specialized;
using System.Windows;
using System.Collections; 

namespace GOSTS.Behavior
{
    public class PersistTabItemsSourceHandler
    {
        public TabControl Tab { get; private set; }

        public PersistTabItemsSourceHandler(TabControl tab)
        {
            Tab = tab;
            Tab.Loaded += TabLoaded;
            AttachCollectionChangedEvent();
        }

        #region Load
        private void TabLoaded(object sender, RoutedEventArgs e)
        {
            AttachCollectionChangedEvent();
            
            LoadItemsSource();
        }

        private void LoadItemsSource()
        {
            var sourceItems = Tab.GetValue(PersistTabBehavior.ItemsSourceProperty) as IEnumerable;

            if (sourceItems == null)
                return;

            Load(sourceItems);
        }

        private void AttachCollectionChangedEvent()
        {
            var source = Tab.GetValue(PersistTabBehavior.ItemsSourceProperty) as INotifyCollectionChanged;

            // This property is not necessary to implement INotifyCollectionChanged.
            // Everything else will still work.  We just can't add or remove tab.
            if (source == null)
                return;

            source.CollectionChanged += SourceCollectionChanged;
        }

        public void Load()
        {
            var source = Tab.GetValue(PersistTabBehavior.ItemsSourceProperty) as IEnumerable;

            if (source == null)
                return;

            Load(source);
        }
        
        private void Load(IEnumerable sourceItems)
        {
            Tab.Items.Clear();

            foreach (var page in sourceItems)
                AddTabItem(page);

            // If there is selected item, select it after setting the initial tabitem collection
            SelectItem();
        }

        //private void TabUnloaded(object sender, RoutedEventArgs e)
        //{
        //    var source = Tab.GetValue(PersistTabBehavior.ItemsSourceProperty) as INotifyCollectionChanged;

        //    if (source != null)
        //        source.CollectionChanged -= SourceCollectionChanged;
        //}
        #endregion

        #region Collection Changed
        private void SourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var view in e.NewItems)
                        AddTabItem(view);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (var view in e.OldItems)
                        RemoveTabItem(view);
                    break;
                //case NotifyCollectionChangedAction.Replace :
                //    foreach (var view in e.OldItems)
                //        ReplaceTabItem.(view);
                //    break;
            }
        }
        
        //private void ReplaceTabItem(object view)
        //{
        //    var contentControl = new ContentControl();
        //    contentControl.SetBinding(ContentControl.ContentProperty, new Binding());
        //    var item = new TabItem { DataContext = view, Content = contentControl };

        //    Tab.Items.Add(item);

        //    // When there is only 1 Item, the tab can't be rendered without have it selected
        //    // Don't do Refresh().  This may clear the Selected item, causing issue in the ViewModel
        //    if (Tab.SelectedItem == null)
        //        item.IsSelected = true;
        //}


        private void SelectItem()
        {
            var selectedObject = Tab.GetValue(PersistTabBehavior.SelectedItemProperty);

            if (selectedObject == null)
                return;

            foreach (TabItem item in Tab.Items)
            {
                if (item.DataContext != selectedObject)
                    continue;

                item.IsSelected = true;
                return;
            }
        }

        private void RemoveTabItem(object view)
        {
            var foundItem = Tab.Items.Cast<TabItem>().FirstOrDefault(t => t.DataContext == view);

            if (foundItem != null)
                Tab.Items.Remove(foundItem);
        }

        private void AddTabItem(object view)
        {
            var contentControl = new ContentControl();
            contentControl.SetBinding(ContentControl.ContentProperty, new Binding());
            var item = new TabItem { DataContext = view, Content = contentControl };

            Tab.Items.Add(item);

            // When there is only 1 Item, the tab can't be rendered without have it selected
            // Don't do Refresh().  This may clear the Selected item, causing issue in the ViewModel
            if (Tab.SelectedItem == null)
                item.IsSelected = true;
        }
        #endregion
        
        public void Dispose()
        {
            var source = Tab.GetValue(PersistTabBehavior.ItemsSourceProperty) as INotifyCollectionChanged;

            if (source != null)
                source.CollectionChanged -= SourceCollectionChanged;

            Tab = null;
        }
    }
}
