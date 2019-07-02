using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using GOSTS.Common;
using GOSTS.ViewModel;
using WPF.MDI;
using System.Collections;
using System.Collections.ObjectModel;

namespace GOSTS.WPFControls
{
    /// <summary>
    /// MarketPriceControl.xaml
    /// </summary>
    public partial class MarketPriceControl : UserControl
    {
        private MessageDistribute distributeMsg = null; 
        public MdiChild mdiChild { get; set; }
        public string countSymbol { get; set; }
        delegate void BindListItem(MarketPriceData data);
        BindListItem InitDelegate;
        delegate void BindProdItem(ObservableCollection<MarketPriceItem> data);
        BindProdItem ProdDelegate;

        List<bool> m_pageFlag = new List<bool>();

        public MarketPriceControl(MessageDistribute _msgDistribute)
        {
            InitializeComponent();
            if (_msgDistribute != null)
            {
                distributeMsg = _msgDistribute;
                distributeMsg.DisPriceManager += new MessageDistribute.OnDisPriceManager(distributeMsg_DisPriceManager);
                distributeMsg.DisMarketPrice += new MessageDistribute.OnDisMarketPrice(distributeMsg_DisMarketPrice);
                distributeMsg.DisLoadProduct += new MessageDistribute.OnDisLoadProduct(distributeMsg_DisLoadProduct);
                InitDelegate = new BindListItem(BindInitItemMethod);
                ProdDelegate = new BindProdItem(BindProdListMethod);
                //distributeMsg.ClearEvent += new MessageDistribute.ClearEventHandler(GOSTradeStation_ClearEvent);
            }
        }

        string ProdListAlertTitle
        {
            get
            {
                return GOSTS.GosCulture.CultureHelper.GetString("MkRemoveProdListAlertTitle");
            }
        }

        string ProdListAlertMsg
        {
            get
            {
                return GOSTS.GosCulture.CultureHelper.GetString("MkRemoveProdListAlertMsg");
            }
        }

        //void GOSTradeStation_ClearEvent(object sender)
        //{
        //    this.TabGrid.SelectionChanged -= TabGrid_SelectionChanged;
        //    this.Unloaded -= UserControl_Unloaded;
        //    distributeMsg.DisPriceManager -= new MessageDistribute.OnDisPriceManager(distributeMsg_DisPriceManager);
        //}

        //HANDLE RECEIVED PRODUCT 
        protected void distributeMsg_DisMarketPrice(object sender, ObservableCollection<MarketPriceItem> data)
        {
            Application.Current.Dispatcher.BeginInvoke(this.ProdDelegate, new Object[] { data });
        }

        public void BindProdListMethod(ObservableCollection<MarketPriceItem> data)
        {
            TabControlViewModel tab = this.TabGrid.DataContext as TabControlViewModel;
            if (tab == null || tab.SelectedPage.MarketPriceItems == null || data == null) return;
            ObservableCollection<MarketPriceItem> marketPriceItems = tab.SelectedPage.MarketPriceItems;

            string[] codeArr = marketPriceItems.Select(d => d.ProductCode).ToArray<string>();
            string[] newCodeArr = data.Select(d => d.ProductCode).ToArray<string>();
             
            var updateData = from c in data
                             where codeArr.Contains(c.ProductCode)
                             select c;
            
            foreach (MarketPriceItem item in updateData)
            {
                MarketPriceItem dr = marketPriceItems.FirstOrDefault(c => c.ProductCode == item.ProductCode) as MarketPriceItem;
                if (dr != null)
                { 
                    dr.AQty = item.AQty;
                    dr.Ask = item.Ask;
                    dr.Bid = item.Bid;
                    dr.BQty = item.BQty;
                    dr.Change = item.Change;
                    dr.ChangePer = item.ChangePer;
                    dr.CloseDate = item.CloseDate;
                    dr.Datetime = item.Datetime;
                    dr.EP = item.EP;
                    dr.Expiry = item.Expiry;
                    dr.High = item.High;
                    dr.Last = item.Last;
                    dr.Low = item.Low;
                    dr.LQty = item.LQty;
                    dr.Open = item.Open;
                    dr.PreClose = item.PreClose;
                    dr.ProductStatus = item.ProductStatus;
                    dr.Strike = item.Strike;
                    dr.ProductCode = item.ProductCode;
                    dr.ProductName = (dr.ProductName == "") ? item.ProductName : dr.ProductName;
                    dr.Volume = item.Volume;
                }
            }
        }

        //Inital window
        protected void distributeMsg_DisPriceManager(object sender, MarketPriceData data)
        {
            Application.Current.Dispatcher.BeginInvoke(this.InitDelegate, new Object[] { data });
        }

        public void BindInitItemMethod(MarketPriceData data)
        {
            DataContext = MainWindowViewModel.Create(data);
            this.TabGrid.IsEnabled = true;
            if (GOSTradeStation.deskFileName != null) return;
       //     SendMarketPrice();
        }

        #region Handle Tab
        private void Rename_Click(object sender, RoutedEventArgs e)
        {
            distributeMsg.HandleTabName();
            this.renameItem.IsEnabled = false;
            this.moveItem.IsEnabled = false;
            this.addItem.IsEnabled = false;
            //HandleMarketPrice child;
            //childWindow = new ChildWindow();
            //childWindow.Parent = this;
            //child = new HandleMarketPrice();
            //child.Close += new EventHandler(child_Close);
            //child.HandleProduct += new HandleMarketPrice.OnHandleProduct(child_HandleProduct);
            //childWindow.Content = child;
            //child.HandleType = "Rename";
            //childWindow.Show();
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            TabControlViewModel tab = this.TabGrid.DataContext as TabControlViewModel;
            if (tab == null || tab.SelectedPage.MarketPriceItems == null) return;
            int index = TabGrid.SelectedIndex;

            if (MessageBox.Show(ProdListAlertMsg,
                              ProdListAlertTitle, MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                ObservableCollection<MarketPriceItem> marketPriceItems = tab.SelectedPage.MarketPriceItems;
                List<string> codeList = marketPriceItems.Select(d => d.ProductCode).ToList<string>();

                if (tab.Pages.Count > 1)
                {
                    TradeStationSend.Send(codeList, null, cmdClient.registerMarketPrice);
                    //Write xml
                    TradeStationDesktop.WriteCDATACodes("MOVE", TradeStationSetting.GetMarketPriceCode(mdiChild.Title), tab.SelectedPage.Header, null, codeList);
                }
                m_pageFlag.RemoveAt(index); 
            }
            else
            {
                tab.Pages.Insert(((tab.Pages.IndexOf(tab.SelectedPage) == 0) ? 1 : tab.Pages.IndexOf(tab.SelectedPage) - 1), tab.SelectedPage);
            }
        }
         
        protected void child_HandleProduct(object sender, List<string> lsGridProd, List<string> lsInsertProd, string strName)
        {
            this.distributeMsg_RegGridProduct(sender,"Rename", null, null, strName);
        }

        /// <summary>
        /// Delegate register
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="lsGridProd">Datagrid data</param>
        /// <param name="lsAddProd">Need to add</param>
        /// <param name="strName">Rename Tabname</param>
        protected void distributeMsg_RegGridProduct(object sender, string handleMode, List<string> lsGridProd, List<string> lsAddProd, string strName)
        {
            TabControlViewModel tab = this.TabGrid.DataContext as TabControlViewModel;
            ObservableCollection<MarketPriceItem> marketPriceItems = tab.SelectedPage.MarketPriceItems;

            string header = tab.SelectedPage.Header;
            int index = tab.Pages.IndexOf(tab.SelectedPage);

            if (handleMode == "Sort")
            {
                TradeStationDesktop.WriteCDATACodes("SORT", TradeStationSetting.GetMarketPriceCode(mdiChild.Title), header, null, lsGridProd);
            }
            else if (handleMode == "Cancel")
            {
                this.renameItem.IsEnabled = true;
                this.moveItem.IsEnabled = true;
                this.addItem.IsEnabled = true;
            }
            else if (handleMode == "Disable")
            {
                this.renameItem.IsEnabled = false;
                this.moveItem.IsEnabled = false;
                this.addItem.IsEnabled = false;
            }
            else if (handleMode == "Rename")
            {
                this.renameItem.IsEnabled = true;
                this.moveItem.IsEnabled = true;
                this.addItem.IsEnabled = true;

                if (strName == "") return;
                tab.SelectedPage.Header = strName;
                //Write xml
                TradeStationDesktop.WriteCDATACodes("RENAME", TradeStationSetting.GetMarketPriceCode(mdiChild.Title), header, strName, null);
            }
            else if (lsAddProd == null)
            {
                TradeStationSend.Send(lsGridProd, null, cmdClient.registerMarketPrice);

                this.renameItem.IsEnabled = true;
                this.moveItem.IsEnabled = true;
                this.addItem.IsEnabled = true;

                //Write xml
                TradeStationDesktop.WriteCDATACodes("MOVECODE", TradeStationSetting.GetMarketPriceCode(mdiChild.Title), header, null, lsGridProd);

                //remove product in datagrid  
                List<string> codeArr = marketPriceItems.Select(d => d.ProductCode).ToList<string>().Except(lsGridProd).ToList();
                IEnumerable<MarketPriceItem> updateData = marketPriceItems.Where(c => codeArr.Contains(c.ProductCode));
                tab.SelectedPage.MarketPriceItems = new ObservableCollection<MarketPriceItem>(updateData);
            }
            else
            {
                this.renameItem.IsEnabled = true;
                this.moveItem.IsEnabled = true;
                this.addItem.IsEnabled = true;

                //unexisted in datagrid
                List<string> lsUnExistProd = new List<string>();
                lsUnExistProd = lsAddProd.Except(lsGridProd).ToList();
                if (lsUnExistProd.Count == 0) return;

                //Write xml
                TradeStationDesktop.WriteCDATACodes("ADD", TradeStationSetting.GetMarketPriceCode(mdiChild.Title), header, null, lsUnExistProd);

                //add product in datagrid  
                if (marketPriceItems == null)
                {
                    marketPriceItems = new ObservableCollection<MarketPriceItem>();
                }

                MarketPriceItem dr;

                foreach (string code in lsUnExistProd)
                {
                    dr = new MarketPriceItem();
                    dr.ProductCode = code;
                    dr.ProductName = MarketPriceData.GetProductName(code);
                    marketPriceItems.Add(dr);
                }

                tab.SelectedPage.Header = header;
                tab.SelectedPage.MarketPriceItems = marketPriceItems;

                TradeStationSend.Get(lsAddProd, cmdClient.getMarketPrice);
                TradeStationSend.Send(null, lsUnExistProd, cmdClient.registerMarketPrice);
            }
        }

        protected void distributeMsg_DisLoadProduct(object sender, List<string> arr, string title)
        {
            if (this.mdiChild.Title != title||arr==null||arr.Count ==0) return;
            int i = 0;
            TabControlViewModel tabAdd = new TabControlViewModel();

            foreach (string str in arr)
            {
                string[] strArr = str.Split(new Char[] { ',' });
                List<string> lsAddProd = new List<string>();
                string header = strArr[0];
                if (strArr[1] != "")
                {
                    lsAddProd = (strArr[1]).Trim().Split(new Char[] { ' ' }).ToList();
                }
                ObservableCollection<MarketPriceItem> marketPriceItems = new ObservableCollection<MarketPriceItem>();
                MarketPriceItem dr;

                foreach (string code in lsAddProd)
                {
                    dr = new MarketPriceItem();
                    dr.ProductCode = code;
                    dr.ProductName = MarketPriceData.GetProductName(code);
                    marketPriceItems.Add(dr);
                }
                tabAdd.Pages.Insert(i, (new TabPageViewModel() { Header = header, MarketPriceItems = marketPriceItems }));
                i++;
            }
            tabAdd.SelectedPage = tabAdd.Pages.First();
            this.TabGrid.DataContext = tabAdd;
            this.addItem.DataContext = tabAdd;
            this.moveItem.DataContext = tabAdd;
        }

        #endregion

        #region delegate control
        private void DataGridControl_Loaded(object sender, RoutedEventArgs e)
        {
            //resize control W/H
            this.mdiChild.Width = this.mdiChild.ActualWidth;
            this.mdiChild.Height = this.mdiChild.ActualHeight;

            ProductsControl dataGridControl1 = FindVisualChild<ProductsControl>(this.TabGrid);
            if (dataGridControl1 != null)
            {
                dataGridControl1.RegGridProduct += new ProductsControl.OnRegGridProduct(distributeMsg_RegGridProduct);
            }
        }

        private void dataGridControl1_Unloaded(object sender, RoutedEventArgs e)
        {
            ProductsControl dataGridControl1 = FindVisualChild<ProductsControl>(this.TabGrid);
            if (dataGridControl1 != null)
            {
                dataGridControl1.RegGridProduct -= new ProductsControl.OnRegGridProduct(distributeMsg_RegGridProduct);
            }
        }

        #endregion

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (GOSTradeStation.marketPriceData != null && GOSTradeStation.marketPriceData.ProdListTable != null && GOSTradeStation.marketPriceData.InstListTable != null)
            {
                BindInitItemMethod(GOSTradeStation.marketPriceData);
            }
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            TabControlViewModel tab = this.TabGrid.DataContext as TabControlViewModel;
            if (tab == null) return;

            List<string> lsAllProd = new List<string>();
            if (tab != null && tab.Pages.Count > 0)
            {
                foreach (TabPageViewModel page in tab.Pages)
                {
                    if (page.MarketPriceItems != null)
                    {
                        lsAllProd.AddRange(page.MarketPriceItems.Select(d => d.ProductCode).ToList<string>());
                    }
                }
            }
            distributeMsg.DisPriceManager -= new MessageDistribute.OnDisPriceManager(distributeMsg_DisPriceManager);
            distributeMsg.DisMarketPrice -= new MessageDistribute.OnDisMarketPrice(distributeMsg_DisMarketPrice);
            distributeMsg.DisLoadProduct -= new MessageDistribute.OnDisLoadProduct(distributeMsg_DisLoadProduct);
            //distributeMsg.ClearEvent -= new MessageDistribute.ClearEventHandler(GOSTradeStation_ClearEvent);

            if (!GOSTradeStation.IsWindowInitialized) return;
            TradeStationSend.Send(lsAllProd, null, cmdClient.registerMarketPrice);
        }

        private void TabGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            while (m_pageFlag.Count < TabGrid.Items.Count)
            {
                m_pageFlag.Add(false);
            }
            if (((TabControl)sender).SelectedIndex != -1 && m_pageFlag[((TabControl)sender).SelectedIndex] == true)
            {
                TabControlViewModel tab = this.TabGrid.DataContext as TabControlViewModel;
                if (tab != null)
                {
                    List<String> lsProdCode = (tab.SelectedPage.MarketPriceItems == null) ? new List<string>() : tab.SelectedPage.MarketPriceItems.Select(d => d.ProductCode).ToList<string>();
                    if (lsProdCode.Count == 0) return;
                    TradeStationSend.Get(lsProdCode, cmdClient.getMarketPrice);
                }
                return;
            }
            if (((TabControl)sender).SelectedIndex != -1)
                m_pageFlag[((TabControl)sender).SelectedIndex] = true;

            SendMarketPrice();
        }

        private void SendMarketPrice()
        {
            TabControlViewModel tab = this.TabGrid.DataContext as TabControlViewModel;
            if (tab == null) return;
            if (tab.SelectedPage != null && GOSTradeStation.m_BeforeGet == true)
            {
                if (tab.SelectedPage.MarketPriceItems != null)
                {
                    foreach (MarketPriceItem mpi in tab.SelectedPage.MarketPriceItems)
                    {
                        if (mpi == null) return;
                        mpi.ProductName = MarketPriceData.GetProductName(mpi.ProductCode);
                    }
                }

                List<String> lsProdCode = (tab.SelectedPage.MarketPriceItems == null) ? new List<string>() : tab.SelectedPage.MarketPriceItems.Select(d => d.ProductCode).ToList<string>();
                if (lsProdCode.Count == 0) return;
                TradeStationSend.Get(lsProdCode, cmdClient.getMarketPrice);
                TradeStationSend.Send(null, lsProdCode, cmdClient.registerMarketPrice);
            }
        }

        #region private
        private childItem FindVisualChild<childItem>(DependencyObject obj) where childItem : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is childItem)
                    return (childItem)child;
                else
                {
                    childItem childOfChild = FindVisualChild<childItem>(child);
                    if (childOfChild != null)
                        return childOfChild;
                }
            }
            return null;
        }
        #endregion

        private void TabTree_Loaded(object sender, RoutedEventArgs e)
        {
            if (GOSTradeStation.marketPriceData == null || GOSTradeStation.marketPriceData.ProdListTable == null || GOSTradeStation.marketPriceData.InstListTable == null)
            {
                TradeStationSend.Send(cmdClient.getInstrumentList);
                TradeStationSend.Send(cmdClient.getProductList);
            }
        }

        private void DataExp_Expanded(object sender, RoutedEventArgs e)
        {
            this.columnTree.Width =new GridLength (220);
        }

        private void DataExp_Collapsed(object sender, RoutedEventArgs e)
        {
            this.columnTree.Width = new GridLength (20);
        }
    }
}
