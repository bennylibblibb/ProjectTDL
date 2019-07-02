using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using WPF.MDI;
using System.Windows.Input;
using GOSTS.Common;
using GOSTS.ViewModel;
using System.Windows.Data;
using System.Globalization;
using System.Windows.Markup;
using System.Collections.ObjectModel;
using GOSTS.Behavior;
using System.Windows.Controls.Primitives;
using GOSTS.WPFControls.Chart.SCI;

namespace GOSTS.WPFControls
{
    /// <summary>
    /// Interaction logic for ProductsControl.xaml
    /// </summary>
    public partial class ProductsControl : UserControl
    {
        public delegate void OnRegGridProduct(object sender, string handleMode, List<string> lsGridProd, List<string> lsAddProd, string strNewName);
        public event OnRegGridProduct RegGridProduct;
        private ChildWindow childWindow;
        HandleMarketPrice child;
        private MessageDistribute distributeMsg = null;

        public static readonly DependencyProperty isDealerHiddenProperty = DependencyProperty.Register("isDealerHidden",
         typeof(Visibility), typeof(ProductsControl), new FrameworkPropertyMetadata(Visibility.Collapsed));
        public Visibility isDealerHidden
        {
            get { return (Visibility)GetValue(isDealerHiddenProperty); }
            set { SetValue(isDealerHiddenProperty, value); }
        }

        // Declare a Delegate which will return the position of the 
        // DragDropEventArgs and the MouseButtonEventArgs event object
        public delegate Point GetDragDropPosition(IInputElement theElement);
        int prevRowIndex = -1;

        public ProductsControl()
        {
            if (distributeMsg == null)
            {
                distributeMsg = GOSTradeStation.distributeMsg;
                distributeMsg.HandleTab += distributeMsg_HandleTabName;
            }

            isDealerHidden = (GOSTradeStation.isDealer == true) ? Visibility.Visible : Visibility.Hidden;

            InitializeComponent();
            childWindow = new ChildWindow();
            childWindow.Parent = this;
            child = new HandleMarketPrice();
            child.Close += new EventHandler(child_Close);
            child.HandleProduct += new HandleMarketPrice.OnHandleProduct(child_HandleProduct);
            childWindow.Content = child;

            this.dgProducts.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(dgProducts_PreviewMouseLeftButtonDown);
            this.PreviewKeyDown += new KeyEventHandler(PreviewKeyControlDownHandler);
        }

        private void PreviewKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (Key.Delete == e.Key)
            {
                child.HandleType = "Remove";
                if (this.dgProducts.SelectedCells.Count > 0 && child.ProdCode != null && child.ProdCode.Count > 0)
                {
                    childWindow.Show();
                }
                RegGridProduct(this, "Disable", null, null, null);

                e.Handled = true;
            }
        }

        private void PreviewKeyControlDownHandler(object sender, KeyEventArgs e)
        { 
            if (Key.Insert == e.Key)
            {
                child.HandleType = "Insert";
                ObservableCollection<MarketPriceItem> data = dgProducts.ItemsSource as ObservableCollection<MarketPriceItem>;
                if (data != null)
                {
                    child.ProdCode = data.Select(d => d.ProductCode).ToList<string>();
                }
                childWindow.Show();
                RegGridProduct(this, "Disable", null, null, null);
              
                e.Handled = true;
             }
        }

        #region Drag marketcode
        void dgProducts_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if ((Mouse.GetPosition(dgProducts).X > dgProducts.ActualWidth - 24) || (Mouse.GetPosition(dgProducts).Y > dgProducts.ActualHeight - 24) || Mouse.GetPosition(dgProducts).Y < 24) return;
            dgProducts.UnselectAllCells();

            prevRowIndex = GetDataGridItemCurrentRowIndex(e.GetPosition);

            if (prevRowIndex < 0)
                return;
            dgProducts.SelectedIndex = prevRowIndex;

            MarketPriceItem selectedEmp = dgProducts.Items[prevRowIndex] as MarketPriceItem;

            if (selectedEmp == null)
                return; DragDropEffects dragdropeffects = DragDropEffects.Move;

            if (DragDrop.DoDragDrop(dgProducts, selectedEmp, dragdropeffects)
                                != DragDropEffects.None)
            {
                //Now This Item will be dropped at new location and so the new Selected Item
                dgProducts.SelectedItem = selectedEmp;
            } 
        }

        /// <param name="theTarget"></param>
        /// <param name="pos"></param>
        /// <returns>The "Rect" Information for specific Position</returns>
        private bool IsTheMouseOnTargetRow(Visual theTarget, GetDragDropPosition pos)
        {
            //try
            //{
            if (theTarget == null) return false;
            Rect posBounds = VisualTreeHelper.GetDescendantBounds(theTarget);
            Point theMousePos = pos((IInputElement)theTarget);
            return posBounds.Contains(theMousePos);
            //}
            //catch
            //{
            //    return false;
            //}
        }

        /// <summary>
        /// Returns the selected DataGridRow
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private DataGridRow GetDataGridRowItem(int index)
        {
            if (dgProducts.ItemContainerGenerator.Status
                    != GeneratorStatus.ContainersGenerated)
                return null;

            return dgProducts.ItemContainerGenerator.ContainerFromIndex(index)
                                                            as DataGridRow;
        }

        /// <summary>
        /// Returns the Index of the Current Row.
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        private int GetDataGridItemCurrentRowIndex(GetDragDropPosition pos)
        {
            int curIndex = -1;
            for (int i = 0; i < dgProducts.Items.Count; i++)
            {
                DataGridRow itm = GetDataGridRowItem(i);
                if (IsTheMouseOnTargetRow(itm, pos))
                {
                    curIndex = i;
                    break;
                }
            }
            return curIndex;
        }
        #endregion

        #region child control

        private void MenuItemClick(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = (MenuItem)e.OriginalSource;
            MdiContainer Container1;
            Container1 = App.Current.MainWindow.FindName("Container") as MdiContainer;

            ObservableCollection<MarketPriceItem> data = dgProducts.ItemsSource as ObservableCollection<MarketPriceItem>;

            switch ((string)menuItem.Tag)
            {
                case "Insert":
                    child.HandleType = "Insert";
                    if (data != null)
                    {
                        child.ProdCode = data.Select(d => d.ProductCode).ToList<string>();
                    }
                    childWindow.Show();
                    RegGridProduct(this, "Disable", null, null, null);
                    break;
                case "Remove":
                    child.HandleType = "Remove";
                    if (this.dgProducts .SelectedCells.Count > 0&& child.ProdCode != null && child.ProdCode.Count > 0)
                    {
                        childWindow.Show();
                    }
                    RegGridProduct(this, "Disable", null, null, null);
                    break;
                case "Clear":
                    child.HandleType = "Clear";
                    if (data == null) return;
                    child.ProdCode = data.Select(d => d.ProductCode).ToList<string>();
                    if (child.ProdCode.Count > 0)
                    {
                        childWindow.Show();
                    }
                    RegGridProduct(this, "Disable", null, null, null);
                    break;
                case "Rename":
                    child.HandleType = "Rename";
                    childWindow.Show();
                    RegGridProduct(this, "Disable", null, null, null);
                    break;
                case "PriceDepth":
                    if (child.ProdCode != null && child.ProdCode.Count > 0)
                    {
                        if (Container1 != null)
                        {
                            MdiChild mcPriceDepth = new MdiChild();
                            PriceDepth priceDepth = new PriceDepth(distributeMsg);
                            priceDepth.ProdCode = child.ProdCode[0];
                            priceDepth.Locked = true;
                            priceDepth.mdiChild = mcPriceDepth;
                            mcPriceDepth.Title = TradeStationSetting.ReturnWindowName(WindowTypes.PriceDepth, child.ProdCode[0]) + " * ";// child.ProdCode[0] + "- " + WindowTypes.PriceDepth + " * ";
                            mcPriceDepth.Content = priceDepth;
                            mcPriceDepth.Width = priceDepth .Width ;
                            mcPriceDepth.Height = priceDepth.Height ;

                            if (!mcPriceDepth.ExistInContainer(mcPriceDepth, Container1))
                            {
                                Container1.Children.Add(mcPriceDepth);
                            }
                        }
                    }
                    break;
                case "LongPriceDepth":
                    if (child.ProdCode != null && child.ProdCode.Count > 0)
                    {
                        if (Container1 != null)
                        {
                            MdiChild mcLongPriceDepth = new MdiChild();
                            LongPriceDepth longPriceDepth = new LongPriceDepth(distributeMsg);
                            longPriceDepth.ProdCode = child.ProdCode[0];
                            longPriceDepth.Locked = true;
                            longPriceDepth.mdiChild = mcLongPriceDepth;
                            mcLongPriceDepth.Title = child.ProdCode[0] + "- " + WindowTypes.LongPriceDepth + " * ";
                            mcLongPriceDepth.Content = longPriceDepth;
                            mcLongPriceDepth.Width = 300;
                            mcLongPriceDepth.Height = 180;

                            if (!mcLongPriceDepth.ExistInContainer(mcLongPriceDepth, Container1))
                            {
                                Container1.Children.Add(mcLongPriceDepth);
                            }
                        }
                    }
                    break;
                case "Ticker":
                    if (child.ProdCode != null && child.ProdCode.Count > 0)
                    {
                        Container1 = App.Current.MainWindow.FindName("Container") as MdiContainer;
                        MdiChild mcTicker = new MdiChild();
                        Ticker ticker = new Ticker(GOSTradeStation.distributeMsg);
                        ticker.mdiChild = mcTicker;
                        ticker.ProdCode = child.ProdCode[0];
                        ticker.Locked = true;
                        mcTicker.Title = TradeStationSetting.ReturnWindowName(WindowTypes.Ticker , child.ProdCode[0])+" * "; // child.ProdCode[0] + "- Ticker * ";
                        mcTicker.Content = ticker;
                        mcTicker.Width = ticker.Width;
                        mcTicker.Height = ticker.Height;

                        if (!mcTicker.ExistInContainer(mcTicker, Container1))
                        {
                            Container1.Children.Add(mcTicker);
                        }
                    }
                    break;
                case "EnterOrder":
                    if (child.ProdCode != null)
                    {
                        MdiChild orderChild = new MdiChild();
                        OrderEntry orderEntry = new OrderEntry(GOSTradeStation.distributeMsg, orderChild);
                        orderEntry.mdiChild = orderChild;
                        orderChild.Content = orderEntry;
                        orderChild.Width = orderEntry.Width;
                        orderChild.Height = orderEntry.Height;
                        if (distributeMsg != null)
                        {
                            foreach (MarketPriceItem dr in dgProducts.Items)
                            {
                                if (dr.ProductCode == child.ProdCode[0])
                                {
                                    distributeMsg.DistributeControlOrder(dr, "Last");
                                    break;
                                }
                            }

                        }
                        orderEntry.tbuCode.IsChecked = true;
                        Container1.Children.Add(orderChild);
                    }
                    break;
                case "Chart":
                    if (child.ProdCode != null && child.ProdCode.Count > 0)
                    {
                        MdiChild sciMDIChild = new MdiChild();
                        SCIChartAnalysis sciChart = new SCIChartAnalysis(GOSTradeStation.distributeMsg, sciMDIChild, child.ProdCode[0]);
                        sciMDIChild.Content = sciChart;
                        sciMDIChild.Width = 1000;
                        sciMDIChild.Height = 500;
                        sciMDIChild.Position = new System.Windows.Point(50, 50);
                        Container1.Children.Add(sciMDIChild);
                    }
                    break;
                case "OptionMaster":
                    if (child.ProdCode != null && child.ProdCode.Count > 0)
                    {
                        Container1 = GOSTradeStation.container;
                        MdiChild mcOptionMasterr = new MdiChild();
                        OptionMaster optionMasterr = new OptionMaster(GOSTradeStation.distributeMsg);
                        optionMasterr.mdiChild = mcOptionMasterr;
                        optionMasterr.ProdCode = child.ProdCode[0];
                       // mcOptionMasterr.Title = TradeStationSetting.ReturnWindowName(WindowTypes.OptionMaster, child.ProdCode[0]) + " * ";// child.ProdCode[0] + "- Option Matrix * ";
                        mcOptionMasterr.Content = optionMasterr;
                        mcOptionMasterr.Width = optionMasterr.Width;
                        mcOptionMasterr.Height = optionMasterr.Height;

                        if (!mcOptionMasterr.ExistInContainer(mcOptionMasterr, Container1))
                        {
                            Container1.Children.Add(mcOptionMasterr);
                        }
                    }
                    break;
                case "AddPriceAlert":
                    if (child.ProdCode != null && child.ProdCode.Count > 0)
                    {
                        MdiChild mcAddPriceAlert = new MdiChild();
                        AddPriceAlert addPriceAlert = new AddPriceAlert(GOSTradeStation.distributeMsg);

                        addPriceAlert.tbID.Text = child.ProdCode[0];
                        addPriceAlert.lbName.Content = MarketPriceData.GetProductName(child.ProdCode[0]);

                        addPriceAlert.mdiChild = mcAddPriceAlert;
                        addPriceAlert.ProdCode = child.ProdCode[0];
                        mcAddPriceAlert.Title = TradeStationSetting.ReturnWindowName(WindowTypes.AddPriceAlert, child.ProdCode[0]); // child.ProdCode[0] + "- Price Alert Settings";
                        mcAddPriceAlert.Content = addPriceAlert;
                        mcAddPriceAlert.Width = addPriceAlert.Width;
                        mcAddPriceAlert.Height = addPriceAlert.Height;

                        if (!mcAddPriceAlert.ExistInContainer(mcAddPriceAlert, Container1))
                        {
                            Container1.Children.Add(mcAddPriceAlert);
                        }
                    }
                    break;
            }
        }

        protected void child_HandleProduct(object sender, string handleMode, List<string> lsGridProd, List<string> lsInsertProd, string strName)
        {
            //ADD REGISTER PRODUCT
            if (RegGridProduct != null)
            {
                RegGridProduct(this, handleMode, lsGridProd, lsInsertProd, strName);
            }

            this.dgProducts.Focus();
        }

        public void child_Close(object sender, EventArgs e)
        {
            childWindow.Close();
        }

        #endregion

        #region 

        protected void distributeMsg_HandleTabName(object sender)
        {
            child.HandleType = "Rename";
            childWindow.Show();
        }

        private void DataGridRowHeader_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DataGridRowHeader rowHeader = sender as DataGridRowHeader;
            if (rowHeader == null) return;

            MarketPriceItem mpi = rowHeader.DataContext as MarketPriceItem;
            if (mpi == null) return;

            if (GOSTradeStation.customizeData.AlertData.isDoubleClick == true)
            {
                if (e.ClickCount > 1)
                {
                    //DoubleClick   
                    distributeMsg.DistributeControl(mpi.ProductCode);
                    distributeMsg.DistributeControlOrder(mpi, "Last");
                }
            }
            else
            {
                //SingleClick  
                distributeMsg.DistributeControl(mpi.ProductCode);
                distributeMsg.DistributeControlOrder(mpi, "Last");
            }
        }

        private void DataGridCell_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DataGridCell cell = sender as DataGridCell;
            if (cell == null) return;

            string strHaeder = cell.Column.Header.ToString();
            //strHaeder = (strHaeder == "Bid" || strHaeder == "買入價" || strHaeder == "买入价") ? "Bid" :
            //    (strHaeder == "BQty" || strHaeder == "买入量" || strHaeder == "買入量") ? "BQty" :
            //    (strHaeder == "Ask" || strHaeder == "沽出價" || strHaeder == "沽出价") ? "Ask" :
            //    (strHaeder == "AQty" || strHaeder == "沽出量" || strHaeder == "沽出量") ? "AQty" :
            //    (strHaeder == "Name" || strHaeder == "名称" || strHaeder == "名稱") ? "Name" :
            //    (strHaeder == "ID") ? "ID" : "";

            strHaeder = (strHaeder == GOSTS.GosCulture.CultureHelper.GetString("MkBid")) ? "Bid" :
               (strHaeder == GOSTS.GosCulture.CultureHelper.GetString("MkBQty")) ? "BQty" :
               (strHaeder == GOSTS.GosCulture.CultureHelper.GetString("MkAsk")) ? "Ask" :
               (strHaeder == GOSTS.GosCulture.CultureHelper.GetString("MkAQty")) ? "AQty" :
                (strHaeder == GOSTS.GosCulture.CultureHelper.GetString("MkName")) ? "Name" : "";

            if (strHaeder != "Bid" && strHaeder != "Ask" && strHaeder != "BQty" && strHaeder != "AQty" && strHaeder != "ID" && strHaeder != "Name") return;
            MarketPriceItem mpi = cell.DataContext as MarketPriceItem;
            if (mpi == null) return;

            if (GOSTradeStation.customizeData.AlertData.isDoubleClick == true)
            {
                if (e.ClickCount > 1)
                {
                    //DoubleClick   
                    distributeMsg.DistributeControl(mpi.ProductCode);
                    distributeMsg.DistributeControlOrder(mpi, strHaeder);
                }
            }
            else
            {
                //SingleClick  
                distributeMsg.DistributeControl(mpi.ProductCode);
                distributeMsg.DistributeControlOrder(mpi, strHaeder);
            }
        }

        private void dgProducts_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            List<string> ls = new List<string>();
            MarketPriceItem mpi;

            IList<DataGridCellInfo> selectedcells = e.AddedCells;
            foreach (DataGridCellInfo dci in selectedcells)
            {
                mpi = dci.Item as MarketPriceItem;
                if (mpi == null) return;
                if (ls.Contains(mpi.ProductCode)) return;
                ls.Add(mpi.ProductCode);
                child.ProdCode = ls;
            } 
        }

        //private void resultDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        //{
        //    if (e.ClickCount > 1)
        //    {
        //        if (GOSTradeStation.customizeData.AlertData.isDoubleClick == false) return;

        //        if (sender != null)
        //        {
        //            DataGridRow dgr = sender as DataGridRow;
        //            if (dgr == null) return;
        //            MarketPriceItem mpi = dgr.Item as MarketPriceItem;
        //            if (mpi == null) return;

        //            if (distributeMsg != null)
        //            {
        //                distributeMsg.DistributeControl(mpi.ProductCode);
        //                //distributeMsg.DistributeControlOrder(mpi);
        //            }
        //        }
        //        e.Handled = true;
        //    }
        //}

        private void dgProducts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //if (sender != null)
            //{
            //    if (dgProducts.SelectedItems.Count > 0)
            //    {
            //        List<string> ls = new List<string>();
            //        MarketPriceItem mpi = new MarketPriceItem();

            //        for (int i = 0; i < dgProducts.SelectedItems.Count; i++)
            //        {
            //            mpi = dgProducts.SelectedItems[i] as MarketPriceItem;
            //            if (mpi == null) return;
            //            ls.Add(mpi.ProductCode);
            //            child.ProdCode = ls;
            //        }

            //        if (GOSTradeStation.customizeData.AlertData.isSingleClick == false) return;

            //        if (distributeMsg != null && mpi != null && child.HandleType != "Remove")
            //        {
            //            distributeMsg.DistributeControl(mpi.ProductCode);
            //            //distributeMsg.DistributeControlOrder(mpi);
            //        }
            //    }
            //}
            e.Handled = true; //disable TabGrid_SelectionChanged
        }
       
        #endregion

        #region drop handle
        private void OnDragOver(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.None;

            Point pos = e.GetPosition(dgProducts);
            HitTestResult result = VisualTreeHelper.HitTest(dgProducts, pos);
            if (result == null)
                return;

            e.Effects = DragDropEffects.Copy;
        }

        private void OnDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetData(typeof(MarketPriceItem)) != null && e.Data.GetData(typeof(MarketPriceItem)).GetType() == typeof(MarketPriceItem))
            {
                if (prevRowIndex < 0)
                    return;

                int index = this.GetDataGridItemCurrentRowIndex(e.GetPosition);

                //The current Rowindex is -1 (No selected)
                if (index < 0)
                    return;
                //If Drag-Drop Location are same
                if (index == prevRowIndex)
                    return;
                //If the Drop Index is the last Row of DataGrid(
                // Note: This Row is typically used for performing Insert operation)
                if (index == dgProducts.Items.Count - 1)
                {
                    // MessageBox.Show("This row-index cannot be used for Drop Operations");
                    return;
                }

                ObservableCollection<MarketPriceItem> data = dgProducts.ItemsSource as ObservableCollection<MarketPriceItem>;

                MarketPriceItem marketPriceItem = data[prevRowIndex];
                data.RemoveAt(prevRowIndex);

                data.Insert(index, marketPriceItem);

                //Sort marketcode
                RegGridProduct(this, "Sort", data.Select(d => d.ProductCode).ToList<string>(), null, null);
            }
            else
            {
                Point pos = e.GetPosition(dgProducts);
                HitTestResult result = VisualTreeHelper.HitTest(dgProducts, pos);
                if (result == null)
                    return;

                //drop product
                List<string> lsReceivedProd = e.Data.GetData(typeof(List<string>)) as List<string>;

                // all product in datagrid
                List<string> lsGridProd = new List<string>();
                if (this.dgProducts.Items.Count > 0)
                {
                    ObservableCollection<MarketPriceItem> marketPriceItems = dgProducts.ItemsSource as ObservableCollection<MarketPriceItem>;
                    if (marketPriceItems == null) return;
                    foreach (MarketPriceItem dr in marketPriceItems)
                    {
                        lsGridProd.Add(dr.ProductCode);
                    }
                }
                //unexisted in datagrid
                RegGridProduct(this, "Add", lsGridProd, lsReceivedProd, null);
            }
        }
        #endregion

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            child.HandleProduct -= new HandleMarketPrice.OnHandleProduct(child_HandleProduct);
            distributeMsg.HandleTab -= distributeMsg_HandleTabName; 
            this.dgProducts.PreviewMouseLeftButtonDown -= new MouseButtonEventHandler(dgProducts_PreviewMouseLeftButtonDown);
            this.PreviewKeyDown -= new KeyEventHandler(PreviewKeyControlDownHandler);
        } 
    }
}