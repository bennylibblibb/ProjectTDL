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
using System.Windows.Controls.Primitives;
using GOSTS.WPFControls.Chart.SCI;
using System.ComponentModel;
using System.Windows.Input;
using GOSTS;

namespace GOSTS.WPFControls
{
    /// <summary>
    /// OptionMaster.xaml 的交互逻辑
    /// </summary>
    public partial class OptionMaster : UserControl
    {
        public MdiChild mdiChild { get; set; }
        public string ProdCode { get; set; }
        public List<string> ProdCodes { get; private set; }
        private MessageDistribute distributeMsg = null;
        OptionMasterViewModel optionMasterViewModel;
        private string m_ProdCode;
        private bool m_Binded;
        private string m_currentPosAcc;
        private PositionBus PosBus;
        delegate void PosInfoChange(ObservableCollection<OrderPosition> posCL, PosChangeType sType, string _Acc);
        PosInfoChange PosDataChange;

        public OptionMaster(MessageDistribute _msgDistribute)
        {
            InitializeComponent();
            this.distributeMsg = _msgDistribute;
            distributeMsg.DisMarketPrice += new MessageDistribute.OnDisMarketPrice(distributeMsg_DisMarketPrice);
            distributeMsg.DisGotProductList += new MessageDistribute.OnDisGotProductList(distributeMsg_DisDisGotProductList);


            PosBus = PositionBus.GetSinglePositionBus(_msgDistribute);
            if (PosBus != null)
            {
                PosDataChange = new PosInfoChange(PositionBusDataChange);
                PosBus.RegisterOnPosInfoChange(new PositionBus.PosInfoChange(PosBus_DataChangeInvoke));

            }

            optionMasterViewModel = new OptionMasterViewModel();
            this.DataContext = optionMasterViewModel;
        }

        protected void PosBus_DataChangeInvoke(object sender, ObservableCollection<OrderPosition> posCL, PosChangeType sType, string _Acc)
        {
            Application.Current.Dispatcher.BeginInvoke(PosDataChange, new Object[] { posCL, sType, _Acc });
        }

        public void PositionBusDataChange(ObservableCollection<OrderPosition> orderPosition, PosChangeType sType, string _Acc)
        {
            m_currentPosAcc = (m_currentPosAcc == null) ? IDelearStatus.ACCUoAc : m_currentPosAcc;

            if (m_currentPosAcc == null) return;

            if (sType == PosChangeType.PosChange)
            {
                if (m_currentPosAcc == IDelearStatus.ACCUoAc)
                {

                    if (orderPosition != null && orderPosition.Count > 0)
                    {
                        //update product Pos
                        if (optionMasterViewModel != null && optionMasterViewModel.marketPriceItems != null && optionMasterViewModel.marketPriceItems.Count > 0)
                        {
                            if (orderPosition.FirstOrDefault(x => (x.ProductCode == optionMasterViewModel.marketPriceItems[0].ProductCode)) != null)
                            {
                                OrderPosition op = orderPosition.FirstOrDefault(x => (x.ProductCode == optionMasterViewModel.marketPriceItems[0].ProductCode));
                                optionMasterViewModel.marketPriceItems[0].Pos = op.Net;
                            }
                        }
                        //update option product Pos
                        if (optionMasterViewModel != null && optionMasterViewModel.masterOptionItems != null && optionMasterViewModel.masterOptionItems.Count > 0)
                        {
                            if (optionMasterViewModel.lsProdCode == null) return;
                            var updateData = from c in orderPosition
                                             where optionMasterViewModel.lsProdCode.Contains(c.ProductCode)
                                             select c;
                            foreach (OrderPosition item in updateData)
                            {
                                var ls = optionMasterViewModel.masterOptionItems.FirstOrDefault(x => (x.ProductCode == item.ProductCode || x.ProductCodeP == item.ProductCode));
                                if (ls != null)
                                {
                                    if (ls.ProductCode == item.ProductCode)
                                    {
                                        ls.Pos = item.Net;

                                    }
                                    else if (ls.ProductCodeP == item.ProductCode)
                                    {
                                        ls.PosP = item.Net;
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    m_currentPosAcc = IDelearStatus.ACCUoAc;
                    if (optionMasterViewModel != null && optionMasterViewModel.marketPriceItems != null && optionMasterViewModel.marketPriceItems.Count > 0)
                    {
                        optionMasterViewModel.marketPriceItems[0].Pos = "";
                    }
                    if (optionMasterViewModel != null && optionMasterViewModel.masterOptionItems != null && optionMasterViewModel.masterOptionItems.Count > 0)
                    {
                        foreach (MasterOptionItem item in optionMasterViewModel.masterOptionItems)
                        {
                            item.Pos = "";
                            item.PosP = "";
                        }
                    }
                }
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.mdiChild.Width = 800;
            this.mdiChild.Height = this.mdiChild.ActualHeight;

            this.BindInitData();

            if (this.ProdCode != null)
            {
                m_currentPosAcc = IDelearStatus.ACCUoAc;

                this.BindData();
                this.BindPosData();
            }
            else
            {
                this.sv1.ScrollToHorizontalOffset(210);
                this.sv2.ScrollToHorizontalOffset(210);
            }
        }

        private void ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            //if (sender == sv1)
            //{
            //    //sv2.ScrollToVerticalOffset(e.VerticalOffset);
            //    sv2.ScrollToHorizontalOffset(e.HorizontalOffset);
            //}
            //else
            //{ 
            sv1.ScrollToVerticalOffset(e.VerticalOffset);
            sv1.ScrollToHorizontalOffset(e.HorizontalOffset);
            //}
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            this.sv2.ScrollToHorizontalOffset(210);

            if (this.ProdCodes != null)
            {
                TradeStationSend.Send(this.ProdCodes, null, cmdClient.registerMarketPrice);
                this.ProdCodes = null;
                this.ProdCode = null;
            }

            if (cbOption.SelectedIndex == -1) return;

            //FGHJKMNQUVXZ
            string strOption = cbOption.SelectedValue.ToString();
            string[] arrFuturesSymbol = { "F", "G", "H", "J", "K", "M", "N", "Q", "U", "V", "X", "Z" };
            string productCode = cbInstrument.SelectedValue + arrFuturesSymbol[Convert.ToInt32(strOption.Substring(strOption.Length - 2, 2)) - 1] + strOption.Substring(3, 1);
            if (this.ProdCode == productCode) return;
            this.ProdCode = productCode;
            BindData();
            this.BindPosData();
        }

        private void BindInitData()
        {
            if (MarketPriceData.GetMarketCodes() == null) return;
            if (this.ProdCode == null)
            {
                this.cbMarketCode.ItemsSource = MarketPriceData.GetMarketCodes();
                this.cbMarketCode.SelectedIndex = 0;
            }
            else
            {
                DataRow dr = MarketPriceData.GetMoreProductInfo(this.ProdCode);
                if (dr == null) return;
                this.cbMarketCode.ItemsSource = MarketPriceData.GetMarketCodes();
                this.cbMarketCode.SelectedValue = dr["marketCode"].ToString();
            }
            m_Binded = true;
        }

        private void BindData()
        {
            optionMasterViewModel.GetOptionMasterData(this.ProdCode);
            this.ProdCode = optionMasterViewModel.productCode;
            this.ProdCodes = optionMasterViewModel.lsProdCode;

            DataRow dr = (this.ProdCode == "") ? (this.ProdCodes != null) ? MarketPriceData.GetProductInfo(this.ProdCodes[0]) : null : MarketPriceData.GetProductInfo(this.ProdCode);
            if (dr != null)
            {
                string defaultTitle = this.mdiChild.Title;
                this.mdiChild.Title = TradeStationSetting.ReturnWindowName(WindowTypes.OptionMaster, "") + " - " + dr["instmntCode"].ToString() + " " + Convert.ToDateTime(dr["expiryDate"].ToString()).ToString("yyyy/MM") + "(" + dr["instmntCode"].ToString() + " " + Convert.ToDateTime(dr["expiryDate"].ToString()).ToString("yyyy-MM") + ")" + " - Contract " + ((this.ProdCode == "") ? (this.ProdCodes != null) ? MarketPriceData.contraceSize(this.ProdCodes[0]).ToString() : "" : MarketPriceData.contraceSize(this.ProdCode).ToString());
                distributeMsg.DistributeControlFocus(defaultTitle, this.mdiChild);
            }

            // Get and Registr this ProdCode 
            if ((ProdCodes == null || ProdCodes.Count == 0) && (this.ProdCode == null || this.ProdCode == "")) return;
            if (this.ProdCodes != null && this.ProdCode != "") this.ProdCodes.Add(this.ProdCode);
            TradeStationSend.Get((this.ProdCodes != null) ? this.ProdCodes : new List<string> { this.ProdCode }, cmdClient.getMarketPrice);
            TradeStationSend.Send(null, (this.ProdCodes != null) ? this.ProdCodes : new List<string> { this.ProdCode }, cmdClient.registerMarketPrice);

        }

        private void BindPosData()
        {
            ObservableCollection<OrderPosition> orderPosition = PosBus.getPosCL() as ObservableCollection<OrderPosition>;
            if (orderPosition != null && orderPosition.Count > 0)
            {
                //update product Pos
                if (orderPosition.FirstOrDefault(x => (x.ProductCode == optionMasterViewModel.marketPriceItems[0].ProductCode)) != null)
                {
                    OrderPosition op = orderPosition.FirstOrDefault(x => (x.ProductCode == optionMasterViewModel.marketPriceItems[0].ProductCode));
                    optionMasterViewModel.marketPriceItems[0].Pos = op.Net;
                }

                //update option product Pos
                if (optionMasterViewModel.lsProdCode == null) return;
                var updateData = from c in orderPosition
                                 where optionMasterViewModel.lsProdCode.Contains(c.ProductCode)
                                 select c;
                foreach (OrderPosition item in updateData)
                {
                    var ls = optionMasterViewModel.masterOptionItems.FirstOrDefault(x => (x.ProductCode == item.ProductCode || x.ProductCodeP == item.ProductCode));
                    if (ls != null)
                    {
                        if (ls.ProductCode == item.ProductCode)
                        {
                            ls.Pos = item.Net;

                        }
                        else if (ls.ProductCodeP == item.ProductCode)
                        {
                            ls.PosP = item.Net;
                        }
                    }
                }
            }
        }

        protected void distributeMsg_DisDisGotProductList(object sender)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                BindInitData();
                if (this.ProdCode != null)
                {
                    this.BindData();
                }
            }));
        }

        protected void distributeMsg_DisMarketPrice(object sender, ObservableCollection<MarketPriceItem> data)
        {
            if ((this.ProdCode == null || this.ProdCode == "") && this.ProdCodes == null) return;
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                //update product     
                if (data.FirstOrDefault(x => (x.ProductCode == optionMasterViewModel.productCode)) != null)
                {
                    MarketPriceItem mpi = data.FirstOrDefault(x => (x.ProductCode == optionMasterViewModel.productCode));
                    foreach (MarketPriceItem item in optionMasterViewModel.marketPriceItems)
                    {
                        item.AQty = mpi.AQty;
                        item.Ask = mpi.Ask;
                        item.Bid = mpi.Bid;
                        item.BQty = mpi.BQty;
                        item.Last = mpi.Last;
                        item.High = mpi.High;
                        item.Low = mpi.Low;
                        item.Volume = mpi.Volume;
                        item.PreClose = mpi.PreClose;
                    }
                }

                //update option product   
                if (optionMasterViewModel.lsProdCode == null) return;
                var updateData = from c in data
                                 where optionMasterViewModel.lsProdCode.Contains(c.ProductCode)
                                 select c;
                foreach (MarketPriceItem item in updateData)
                {
                    var ls = optionMasterViewModel.masterOptionItems.FirstOrDefault(x => (x.ProductCode == item.ProductCode || x.ProductCodeP == item.ProductCode));
                    if (ls != null)
                    {
                        if (ls.ProductCode == item.ProductCode)
                        {
                            ls.Volume = item.Volume;
                            ls.Low = item.Low;
                            ls.High = item.High;
                            ls.PreClose = item.PreClose;
                            ls.Last = item.Last;
                            ls.BQty = item.BQty;
                            ls.Bid = item.Bid;
                            ls.Ask = item.Ask;
                            ls.AQty = item.AQty;
                            //ls.Strike = item.Strike;
                        }
                        else if (ls.ProductCodeP == item.ProductCode)
                        {
                            ls.VolumeP = item.Volume;
                            ls.LowP = item.Low;
                            ls.HighP = item.High;
                            ls.PreCloseP = item.PreClose;
                            ls.LastP = item.Last;
                            ls.BQtyP = item.BQty;
                            ls.BidP = item.Bid;
                            ls.AskP = item.Ask;
                            ls.AQtyP = item.AQty;
                            //ls.StrikeP = item.Strike;
                        }
                    }
                }
            }));
        }

        private void cbMarketCode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.cbMarketCode.SelectedIndex == -1) return;
            if (this.ProdCode == null || m_Binded)
            {
                this.cbInstrument.ItemsSource = MarketPriceData.GetInstrCoedes(cbMarketCode.SelectedValue.ToString());
                this.cbInstrument.SelectedIndex = 0;
            }
            else
            {
                DataRow dr = MarketPriceData.GetMoreProductInfo(this.ProdCode);
                this.cbInstrument.ItemsSource = MarketPriceData.GetInstrCoedes(dr["marketCode"].ToString());
                this.cbInstrument.SelectedValue = dr["instmntCode"].ToString();
            }

        }

        private void cbInstrument_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (m_Binded && this.ProdCode != null)
            {
                this.optionMasterViewModel.marketPriceItems.Clear();
                this.optionMasterViewModel.masterOptionItems.Clear();
                this.mdiChild.Title = TradeStationSetting.ReturnWindowName(WindowTypes.OptionMaster, "") + " - ";
                lbName.Content = "";
            }

            if (this.cbInstrument.SelectedIndex == -1) return;

            if (this.ProdCode == null || m_Binded)
            {
                ProdType prodType = new ProdType("2", cbInstrument.SelectedValue.ToString(), cbMarketCode.SelectedValue.ToString());
                ProdOption[] prodOptions = Database.GetProdOptions(prodType);
                if (prodOptions == null) return;
                cbOption.ItemsSource = prodOptions.Select(d => d.ProdOptionName.ToString("yyyy/MM")).ToList();
                this.cbOption.SelectedIndex = 0;
            }
            else
            {
                DataRow dr = MarketPriceData.GetMoreProductInfo(this.ProdCode);
                if (dr == null) return;
                ProdType prodType = new ProdType("2", dr["instmntCode"].ToString(), dr["marketCode"].ToString());
                ProdOption[] prodOptions = Database.GetProdOptions(prodType);
                cbOption.ItemsSource = prodOptions.Select(d => d.ProdOptionName.ToString("yyyy/MM")).ToList();

                this.cbOption.SelectedValue = Convert.ToDateTime(dr["expiryDate"].ToString()).ToString("yyyy/MM");
            }
        }

        private void cbOption_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.cbOption.SelectedIndex == -1) return;

            if (m_Binded && this.ProdCode != null)
            {
                this.optionMasterViewModel.marketPriceItems.Clear();
                this.optionMasterViewModel.masterOptionItems.Clear();
                this.mdiChild.Title = TradeStationSetting.ReturnWindowName(WindowTypes.OptionMaster, "") + " - ";
                lbName.Content = "";
            }
        }

        private void MenuItemClick(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = (MenuItem)e.OriginalSource;
            MdiContainer Container1;
            Container1 = App.Current.MainWindow.FindName("Container") as MdiContainer;
            switch ((string)menuItem.Tag)
            {
                case "PriceDepth":
                    if (Container1 != null)
                    {
                        if (m_ProdCode != null)
                        {
                            MdiChild mcPriceDepth = new MdiChild();
                            PriceDepth priceDepth = new PriceDepth(distributeMsg);
                            priceDepth.ProdCode = m_ProdCode;
                            priceDepth.Locked = true;
                            priceDepth.mdiChild = mcPriceDepth;
                            mcPriceDepth.Title = TradeStationSetting.ReturnWindowName(WindowTypes.PriceDepth, m_ProdCode) + " * ";// m_ProdCode + "- " + WindowTypes.PriceDepth + " * ";
                            mcPriceDepth.Content = priceDepth;
                            mcPriceDepth.Width = 600;
                            mcPriceDepth.Height = 180;

                            if (!mcPriceDepth.ExistInContainer(mcPriceDepth, Container1))
                            {
                                Container1.Children.Add(mcPriceDepth);
                            }
                        }
                    }
                    break;
                case "Chart":
                    if (Container1 != null)
                    {
                        if (m_ProdCode != null)
                        {
                            MdiChild sciMDIChild = new MdiChild();
                            SCIChartAnalysis sciChart = new SCIChartAnalysis(GOSTradeStation.distributeMsg, sciMDIChild, m_ProdCode);
                            sciMDIChild.Content = sciChart;
                            sciMDIChild.Width = 1000;
                            sciMDIChild.Height = 500;
                            sciMDIChild.Position = new System.Windows.Point(50, 50);
                            Container1.Children.Add(sciMDIChild);
                            if (!sciMDIChild.ExistInContainer(sciMDIChild, Container1))
                            {
                                Container1.Children.Add(sciMDIChild);
                            }
                        }
                    }
                    break;
                case "SimpleMode":
                    this.miBaseMode1.IsChecked = false;
                    this.miSimpleMode1.IsChecked = true;
                    this.miBaseMode2.IsChecked = false;
                    this.miSimpleMode2.IsChecked = true;
                    this.Column23.Visibility = Visibility.Hidden;
                    this.Column21.Visibility = Visibility.Hidden;
                    this.Column20.Visibility = Visibility.Hidden;
                    this.Column19.Visibility = Visibility.Hidden;
                    this.Column18.Visibility = Visibility.Hidden;
                    this.Column17.Visibility = Visibility.Hidden;
                    this.Column7.Visibility = Visibility.Hidden;
                    this.Column6.Visibility = Visibility.Hidden;
                    this.Column5.Visibility = Visibility.Hidden;
                    this.Column4.Visibility = Visibility.Hidden;
                    this.Column3.Visibility = Visibility.Hidden;
                    this.Column1.Visibility = Visibility.Hidden;
                    this.Column23P.Visibility = Visibility.Hidden;
                    this.Column21P.Visibility = Visibility.Hidden;
                    this.Column20P.Visibility = Visibility.Hidden;
                    this.Column19P.Visibility = Visibility.Hidden;
                    this.Column18P.Visibility = Visibility.Hidden;
                    this.Column17P.Visibility = Visibility.Hidden;
                    this.Column7P.Visibility = Visibility.Hidden;
                    this.Column6P.Visibility = Visibility.Hidden;
                    this.Column5P.Visibility = Visibility.Hidden;
                    this.Column4P.Visibility = Visibility.Hidden;
                    this.Column3P.Visibility = Visibility.Hidden;
                    this.Column1P.Visibility = Visibility.Hidden;

                    this.rowBase.Height = new GridLength(0);
                    this.rowSimple.Height = new GridLength(23);
                    break;
                case "BaseMode":
                    this.miBaseMode1.IsChecked = true;
                    this.miSimpleMode1.IsChecked = false;
                    this.miBaseMode2.IsChecked = true;
                    this.miSimpleMode2.IsChecked = false;
                    this.Column23.Visibility = Visibility.Visible;
                    this.Column21.Visibility = Visibility.Visible;
                    this.Column20.Visibility = Visibility.Visible;
                    this.Column19.Visibility = Visibility.Visible;
                    this.Column18.Visibility = Visibility.Visible;
                    this.Column17.Visibility = Visibility.Visible;
                    this.Column7.Visibility = Visibility.Visible;
                    this.Column6.Visibility = Visibility.Visible;
                    this.Column5.Visibility = Visibility.Visible;
                    this.Column4.Visibility = Visibility.Visible;
                    this.Column3.Visibility = Visibility.Visible;
                    this.Column1.Visibility = Visibility.Visible;
                    this.Column23P.Visibility = Visibility.Visible;
                    this.Column21P.Visibility = Visibility.Visible;
                    this.Column20P.Visibility = Visibility.Visible;
                    this.Column19P.Visibility = Visibility.Visible;
                    this.Column18P.Visibility = Visibility.Visible;
                    this.Column17P.Visibility = Visibility.Visible;
                    this.Column7P.Visibility = Visibility.Visible;
                    this.Column6P.Visibility = Visibility.Visible;
                    this.Column5P.Visibility = Visibility.Visible;
                    this.Column4P.Visibility = Visibility.Visible;
                    this.Column3P.Visibility = Visibility.Visible;
                    this.Column1P.Visibility = Visibility.Visible;

                    this.rowBase.Height = new GridLength(23);
                    this.rowSimple.Height = new GridLength(0);
                    break;
            }
        }

        ////Base Mode/Simple Mode
        //public static readonly DependencyProperty isSimpleHiddenProperty = DependencyProperty.Register("isSimpleHidden",
        // typeof(Visibility), typeof(ProductsControl), new FrameworkPropertyMetadata(Visibility.Collapsed));
        // public Visibility isSimpleHidden
        //{
        //    get { return (Visibility)GetValue(isSimpleHiddenProperty); }
        //    set { SetValue(isSimpleHiddenProperty, value); }
        //} 

        private void DataGridCell_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DataGridCell cell = sender as DataGridCell;
            if (cell == null) return;

            string strHaeder = cell.Column.Header.ToString();
            if (strHaeder != "Pos" && strHaeder != "PosP" && strHaeder != "LastP" && strHaeder != "BidP" && strHaeder != "AskP" && strHaeder != "BQtyP" && strHaeder != "AQtyP" && strHaeder != "Last" && strHaeder != "Bid" && strHaeder != "Ask" && strHaeder != "BQty" && strHaeder != "AQty") return;

            MasterOptionItem moi = cell.DataContext as MasterOptionItem;
            if (moi == null) return;

            MarketPriceItem mpi = new MasterOptionItem();
            if (strHaeder == "Pos" || strHaeder == "Last" || strHaeder == "Bid" || strHaeder == "Ask" || strHaeder == "BQty" || strHaeder == "AQty")
            {
                mpi.ProductCode = moi.ProductCode;
                mpi.ProductName = moi.ProductName;
                mpi.Bid = moi.Bid;
                mpi.Ask = moi.Ask;
                mpi.BQty = moi.BQty;
                mpi.AQty = moi.AQty;
                mpi.Last = moi.Last;
                mpi.Pos = moi.Pos;
            }
            else if (strHaeder == "PosP" || strHaeder == "LastP" || strHaeder == "BidP" || strHaeder == "AskP" || strHaeder == "BQtyP" || strHaeder == "AQtyP")
            {
                mpi.ProductCode = moi.ProductCodeP;
                mpi.ProductName = moi.ProductNameP;
                mpi.Bid = moi.BidP;
                mpi.Ask = moi.AskP;
                mpi.BQty = moi.BQtyP;
                mpi.AQty = moi.AQtyP;
                mpi.Last = moi.LastP;
                mpi.Pos = moi.PosP;
            }

            lbName.Content = mpi.ProductCode + " (" + mpi.ProductName + ")";
            m_ProdCode = mpi.ProductCode;

            if (strHaeder == "Pos" || strHaeder == "PosP")
            {
                if (mpi.Pos != null && mpi.Pos != "")
                {
                    //Close Option Position
                    if (PosBus != null)
                    {
                        PosBus.OpenClosePos(mpi.ProductCode, m_currentPosAcc, this.distributeMsg);
                    }
                }
            }
            else
            {
                if (GOSTradeStation.customizeData.AlertData.isDoubleClick == true)
                {
                    if (e.ClickCount > 1)
                    {
                        //DoubleClick   
                        distributeMsg.DistributeControl(mpi.ProductCode);
                        distributeMsg.DistributeControlOrder(mpi, (strHaeder.IndexOf("P") > -1) ? strHaeder.Substring(0, strHaeder.Length - 1) : strHaeder);
                    }
                }
                else
                {
                    //SingleClick  
                    distributeMsg.DistributeControl(mpi.ProductCode);
                    distributeMsg.DistributeControlOrder(mpi, (strHaeder.IndexOf("P") > -1) ? strHaeder.Substring(0, strHaeder.Length - 1) : strHaeder);
                }
            }
        }

        private void dgProdPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DataGridCell cell = sender as DataGridCell;
            if (cell == null) return;

            string strHaeder = cell.Column.Header.ToString();
            if (strHaeder != "Pos" && strHaeder != "Last" && strHaeder != "Bid" && strHaeder != "Ask" && strHaeder != "BQty" && strHaeder != "AQty") return;

            MarketPriceItem mpi = cell.DataContext as MarketPriceItem;
            if (mpi == null) return;

            lbName.Content = mpi.ProductCode + " (" + mpi.ProductName + ")";
            m_ProdCode = mpi.ProductCode;

            if (strHaeder == "Pos")
            {
                if (mpi.Pos != null && mpi.Pos != "")
                {
                    //Close Product Position
                    if (PosBus != null)
                    {
                        PosBus.OpenClosePos(mpi.ProductCode, m_currentPosAcc, this.distributeMsg);
                    }
                }
            }
            else
            {
                if (GOSTradeStation.customizeData.AlertData.isDoubleClick == true)
                {
                    if (e.ClickCount > 1)
                    {
                        //DoubleClick   
                        distributeMsg.DistributeControl(mpi.ProductCode);
                        distributeMsg.DistributeControlOrder(mpi, (strHaeder.IndexOf("P") > -1) ? strHaeder.Substring(0, strHaeder.Length - 1) : strHaeder);
                    }
                }
                else
                {
                    //SingleClick  
                    distributeMsg.DistributeControl(mpi.ProductCode);
                    distributeMsg.DistributeControlOrder(mpi, (strHaeder.IndexOf("P") > -1) ? strHaeder.Substring(0, strHaeder.Length - 1) : strHaeder);
                }
            }
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            distributeMsg.DisMarketPrice -= new MessageDistribute.OnDisMarketPrice(distributeMsg_DisMarketPrice);
            distributeMsg.DisGotProductList -= new MessageDistribute.OnDisGotProductList(distributeMsg_DisDisGotProductList);
            if (PosBus != null)
            {
                PosBus.UnRegisterOnPosInfoChange(new PositionBus.PosInfoChange(PosBus_DataChangeInvoke));
            }
            if ((!GOSTradeStation.IsWindowInitialized) || ((ProdCodes == null || ProdCodes.Count == 0) && (this.ProdCode == null || this.ProdCode == ""))) return;
            TradeStationSend.Send((this.ProdCodes != null) ? this.ProdCodes : new List<string> { this.ProdCode }, null, cmdClient.registerMarketPrice);
        }

        private void dgOptions2_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            IList<DataGridCellInfo> selectedcells = e.AddedCells;
            foreach (DataGridCellInfo dci in selectedcells)
            {
                MasterOptionItem moi = dci.Item as MasterOptionItem;
                if (moi == null) return;
                if (dci.Column.DisplayIndex > 11)
                {
                    lbName.Content = moi.ProductCodeP + " (" + moi.ProductNameP + ")";
                    m_ProdCode = moi.ProductCodeP;
                }
                else
                {
                    lbName.Content = moi.ProductCode + " (" + moi.ProductName + ")";
                    m_ProdCode = moi.ProductCode;
                }
            }
        }

        private void dgOptions1_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            IList<DataGridCellInfo> selectedcells = e.AddedCells;
            foreach (DataGridCellInfo dci in selectedcells)
            {
                MarketPriceItem mpi = dci.Item as MarketPriceItem;
                if (mpi == null) return;
                lbName.Content = mpi.ProductCode + " (" + mpi.ProductName + ")";
                m_ProdCode = mpi.ProductCode;
            }
        }

        private void DataGrid_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            this.sv2.ScrollToVerticalOffset(sv2.VerticalOffset - e.Delta / 4);

        }
    }
}
