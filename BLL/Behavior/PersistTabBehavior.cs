using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Collections;

namespace GOSTS.Behavior
{
    /// <summary>
    /// The TabControl will create the visual tree everytime a tab is switch if ItemsSource is used
    /// This class is adding a extra layer between the ObservableCollection and the TabItems.  Tab will be add and removed manually in this class so visual tree 
    /// won't be created again and again
    /// </summary>
    public static class PersistTabBehavior
    {
        #region ItemsSource
        private static readonly Dictionary<TabControl, PersistTabItemsSourceHandler> ItemSourceHandlers = new Dictionary<TabControl, PersistTabItemsSourceHandler>();

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.RegisterAttached(
            "ItemsSource", typeof(IEnumerable), typeof(PersistTabBehavior),
            new UIPropertyMetadata(null, OnItemsSourcePropertyChanged)
        );

        #region Getter / Setter
        public static void SetItemsSource(DependencyObject tab, IEnumerable source)
        {
            tab.SetValue(ItemsSourceProperty, source);
        }

        public static object GetItemsSource(DependencyObject tab)
        {
            return tab.GetValue(ItemsSourceProperty);
        }
        #endregion

        private static void OnItemsSourcePropertyChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs e)
        {
            var tabControl = dpo as TabControl;

            if (tabControl == null)
                return;

            if (tabControl.ItemsSource != null)
                return;

            PersistTabItemsSourceHandler handler;

            if (!ItemSourceHandlers.ContainsKey(tabControl))
            {
                handler = new PersistTabItemsSourceHandler(tabControl);
                ItemSourceHandlers.Add(tabControl, handler);
                tabControl.Unloaded += ItemsSourceTabControlUnloaded;
            }
            else
            {
                handler = ItemSourceHandlers[tabControl];
            }

            handler.Load();
        }

        private static void ItemsSourceTabControlUnloaded(object sender, RoutedEventArgs e)
        {
            var tabControl = sender as TabControl;

            if (tabControl == null)
                return;

            RemoveFromItemSourceHandlers(tabControl);
            tabControl.Unloaded -= ItemsSourceTabControlUnloaded;
        }

        private static void RemoveFromItemSourceHandlers(TabControl tabControl)
        {
            if (!ItemSourceHandlers.ContainsKey(tabControl))
                return;

            ItemSourceHandlers[tabControl].Dispose();
            ItemSourceHandlers.Remove(tabControl);
        }
        #endregion

        #region SelectedItem
        private static readonly Dictionary<TabControl, PersistTabSelectedItemHandler> SelectedItemHandlers = new Dictionary<TabControl, PersistTabSelectedItemHandler>();

        // FrameworkPropertyMetadata is required for TwoWay Binding to work
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.RegisterAttached(
            "SelectedItem", typeof(object), typeof(PersistTabBehavior),
            new FrameworkPropertyMetadata(
                null,
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                OnSelectedItemPropertyChanged));

        #region Getter / Setter
        public static void SetSelectedItem(DependencyObject tab, object source)
        {
            tab.SetValue(SelectedItemProperty, source);
        }

        public static object GetSelectedItem(DependencyObject tab)
        {
            return tab.GetValue(SelectedItemProperty);
        }
        #endregion

        private static void OnSelectedItemPropertyChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs e)
        {
            var tabControl = dpo as TabControl;

            if (tabControl == null)
                return;

            if (tabControl.ItemsSource != null)
                return;

            PersistTabSelectedItemHandler handler; 
         
            if (!SelectedItemHandlers.ContainsKey(tabControl))
            {
                handler = new PersistTabSelectedItemHandler(tabControl);
                SelectedItemHandlers.Add(tabControl, handler);
                tabControl.Unloaded += SelectedItemTabControlUnloaded;
            }
            else
            {
                handler = SelectedItemHandlers[tabControl];
                //Instmnt[] instmnts = Database.GetInstmnts(GOSTradeStation.marketPriceData.MarketTreeTable.Select("marketCode='" + GOSTradeStation.marketPriceData.MarketCodeList[tabControl.SelectedIndex].ToString() + "'"));
                //WPFControls.MarketPriceWPF.viewModel = new GOSTS.ViewModel.MarketCodeViewModel(instmnts);

            }

            handler.ChangeSelectionFromProperty();
        }

        private static void SelectedItemTabControlUnloaded(object sender, RoutedEventArgs e)
        {
            var tabControl = sender as TabControl;

            if (tabControl == null)
                return;

            RemoveFromSelectedItemHandlers(tabControl);

            tabControl.Unloaded -= SelectedItemTabControlUnloaded;
        }

        private static void RemoveFromSelectedItemHandlers(TabControl tabControl)
        {
            if (!SelectedItemHandlers.ContainsKey(tabControl))
                return;

            SelectedItemHandlers[tabControl].Dispose();
            SelectedItemHandlers.Remove(tabControl);
        }
        #endregion
    }
}
