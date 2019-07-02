using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data;
using GOSTS.Common;
using WPF.MDI;
using System.Collections.ObjectModel;

namespace GOSTS
{
    /// <summary>
    /// ClosePosition.xaml 的交互逻辑
    /// </summary>
    public partial class ClosePosition : UserControl, IChangeLang
    {

        static ClosePosition closePosition = null;
        public MessageDistribute distributeMsg = null;
        public int ChangePrice, AddPrice;
        public uint ChangeCount;
        public string ChangeRef,str_OrderID;
        public string strMessage, StrBuySell;
        public MdiChild mdiClosePosition;

        delegate void DisOrderConfirm(string str);
        DisOrderConfirm DisOrderConfirmDelegate;

        delegate void NotificationItem(string str);
        NotificationItem NotificationDelegate;

        string ACC="";
        decimal bkPrice=0;
        TradeStationComm.Attribute.BuySell BuyOrSell = TradeStationComm.Attribute.BuySell.sell;      

        OrderModel orderModel = new OrderModel();
        DecimalTextBz decTbCheck = new DecimalTextBz();

        #region resource text
        string strRs_CLPos_Err_initErr
        {
            get { return GOSTS.GosCulture.CultureHelper.GetString("CLPos_Err_initErr"); }
        }
       
        string strRs_Conf_SellOrder_Title
        {
            get { return GOSTS.GosCulture.CultureHelper.GetString("confirm_SellOrder_Title"); }
        }
        string strRs_SellOrder_Fail { 
            get { return GOSTS.GosCulture.CultureHelper.GetString("SellOrder_Fail"); } 
        }
        string strRs_confirm_BuyOrder_Title { 
            get { return GOSTS.GosCulture.CultureHelper.GetString("confirm_BuyOrder_Title"); }
        }
        string strRs_BuyOrder_Fail
        {
            get { return GOSTS.GosCulture.CultureHelper.GetString("BuyOrder_Fail"); }
        }
        #endregion

        private ClosePosition(MessageDistribute _msgDistribute, OrderPosition ClosePosition, MdiChild mdi,string acc)
        {          
            InitializeComponent();
            DisOrderConfirmDelegate = new DisOrderConfirm(OpenOrderConfrim);
            ACC = acc;
            txtAcc.Text = ACC;
            mdiClosePosition = mdi;
            this.DataContext = orderModel;

            #region
            orderModel.ProductCode = ClosePosition.OrderID;
            try
            {
                setTxtPrice(ClosePosition.strMktPrice.Trim());
                orderModel.Price = Utility.ConvertToDecimal(ClosePosition.strMktPrice.Trim(), 0);

                bkPrice = orderModel.Price;
                orderModel.Bid = ClosePosition.DecBid;
                if(GosBzTool.IsAOValue(orderModel.Bid.ToString()))
                {
                    this.btn_QuickSell.IsEnabled=false; 
                }
                else{
                    this.btn_QuickSell.IsEnabled = true; 
                }
                orderModel.Ask = ClosePosition.DecAsk;
                if (GosBzTool.IsAOValue(orderModel.Ask.ToString()))
                {
                    this.btnQuickBuy .IsEnabled = false;
                }
                else
                {
                    this.btnQuickBuy.IsEnabled = true;
                }
                orderModel.ProductCode = ClosePosition.OrderID;
                int intTemp = Convert.ToInt32(ClosePosition.Net);
                orderModel.Count = Convert.ToUInt32(intTemp < 0 ? -intTemp : intTemp);
            }
            catch { orderModel.Price = 0; MessageBox.Show(strRs_CLPos_Err_initErr); mdi.Close(); }            
          
            orderModel.TPlus = TradeStationComm.Attribute.TOne.TOnly;           
            MarketPriceDelegate = new BindListItemMarketPrice(BindListItemMethodMarketPrice);
            #endregion

            buySell(ClosePosition.Col7);
            str_OrderID = ClosePosition.OrderID;        
            if (ClosePosition.OrderID != null)
            {               
                TradeStationSend.Send(null, ClosePosition.OrderID, cmdClient.registerMarketPrice);
                List<string> lsProd = new List<string>();
                lsProd.Add(ClosePosition.OrderID);
                TradeStationSend.Get(lsProd, cmdClient.getMarketPrice);
            }
            if (_msgDistribute != null)
            {
                distributeMsg = _msgDistribute;
                distributeMsg.DisMarketPrice += new MessageDistribute.OnDisMarketPrice(distributeMsg_DisMarketPrice);          
            }
            decTbCheck.SetDecText(this.txt_Price);
            decTbCheck.SetProdCode(orderModel.ProductCode);
            SetUPDownBtnStep();
            ActiveBuySell();
            this.txt_Add.Text = "0";
            bindControlEvent();

            if (ClosePosition != null)
            {
                if (ClosePosition.OrderID != null)
                {
                    //string NewTitle = "Close Position - " + ClosePosition.OrderID;
                    //GosBzTool.setTitle(mdiClosePosition, _msgDistribute, NewTitle);
                    setFormTitle(1,_msgDistribute,mdiClosePosition,new string[]{ClosePosition.OrderID});
                }
            }
           
        }

        public static  ClosePosition GetClosePosition(MessageDistribute _msgDistribute, OrderPosition ClosePosition,MdiChild mdi,string acc,ref bool bFirstInit)
        {
            bool bNeedInstantiate = false;
            if (closePosition == null)
            {
                bNeedInstantiate = true;
            }
            else if (closePosition.distributeMsg != _msgDistribute)
            {
                bNeedInstantiate = true;
            }
           
            bFirstInit = bNeedInstantiate;
            if (bNeedInstantiate)
            {
                closePosition = new ClosePosition(_msgDistribute, ClosePosition, mdi, acc);
            }
            else
            {
                closePosition.SetIntialData(ClosePosition, acc);
            }
           
            return closePosition;
        }

        void SetIntialData(OrderPosition ClosePosition, string acc)
        {
            ACC = acc;
            txtAcc.Text = ACC;
            string OldProduct = "";
            if (orderModel != null)
            {
                if (orderModel.ProductCode != null)
                {
                    OldProduct = orderModel.ProductCode.Trim();
                }
            }
            #region
            try
            {
                orderModel.ProductCode = ClosePosition.OrderID;
                setTxtPrice(ClosePosition.strMktPrice.Trim());
                orderModel.Price = Utility.ConvertToDecimal(ClosePosition.strMktPrice.Trim(), 0);

                bkPrice = orderModel.Price;
                orderModel.Bid = ClosePosition.DecBid;
                if (GosBzTool.IsAOValue(orderModel.Bid.ToString()))
                {
                    this.btn_QuickSell.IsEnabled = false;
                }
                else
                {
                    this.btn_QuickSell.IsEnabled = true;
                }
                orderModel.Ask = ClosePosition.DecAsk;
                if (GosBzTool.IsAOValue(orderModel.Ask.ToString()))
                {
                    this.btnQuickBuy.IsEnabled = false;
                }
                else
                {
                    this.btnQuickBuy.IsEnabled = true;
                }
                int intTemp = Convert.ToInt32(ClosePosition.Net);
                orderModel.Count = Convert.ToUInt32(intTemp < 0 ? -intTemp : intTemp);
            }
            catch { orderModel.Price = 0; MessageBox.Show(this.strRs_CLPos_Err_initErr); if (mdiClosePosition != null) { mdiClosePosition.Close(); } }
            orderModel.ProductCode = ClosePosition.OrderID;
            orderModel.TPlus = TradeStationComm.Attribute.TOne.TOnly;          
            #endregion

            buySell(ClosePosition.Col7);
            str_OrderID = ClosePosition.OrderID;
            if (ClosePosition.OrderID != null )
            {
                if (OldProduct != ClosePosition.OrderID)
                {
                    TradeStationSend.Send(OldProduct == "" ? null : OldProduct, ClosePosition.OrderID, cmdClient.registerMarketPrice);
                    List<string> lsProd = new List<string>();
                    lsProd.Add(ClosePosition.OrderID);
                    TradeStationSend.Get(lsProd, cmdClient.getMarketPrice);
                }
            }
            decTbCheck.SetDecText(this.txt_Price);
            decTbCheck.SetProdCode(orderModel.ProductCode);
            ActiveBuySell();
            this.txt_Add.Text = "0";
            SetUPDownBtnStep();
        
            if (ClosePosition != null)
            {
                if (ClosePosition.OrderID != null)
                {
                    //string NewTitle = "Close Position - " + ClosePosition.OrderID;
                    //GosBzTool.setTitle(mdiClosePosition, distributeMsg, NewTitle);
                    setFormTitle(1, distributeMsg, mdiClosePosition, new string[] { ClosePosition.OrderID });
                }
            }
           
        }

        void bindControlEvent()
        {
            this.btn_QuickSell.Click += btnQuickSell_Click;
            this.btnQuickBuy.Click+=btnQuickBuy_Click;
            chkT1.Checked += chkT1_Checked;
            chkT1.Unchecked += chkT1_Unchecked;
           
            decTbCheck.SetMdiChild(mdiClosePosition);
            decTbCheck.BindWheel();
        }

        void OnPriceError(object sender, ValidationErrorEventArgs vea)
        {
            checkPrice();
        }

        void OnCountError(object sender, ValidationErrorEventArgs e)
        {
            checkQtyInput();
        }

        #region register market price
        delegate void BindListItemMarketPrice(ObservableCollection<MarketPriceItem> MktItems);
        BindListItemMarketPrice MarketPriceDelegate;

        protected void distributeMsg_DisMarketPrice(object sender, ObservableCollection<MarketPriceItem> MktItems)
        {
            Application.Current.Dispatcher.BeginInvoke(this.MarketPriceDelegate, new Object[] { MktItems });
        }

        MarketPriceItem CurrentProdMktItem = null;   
        public void BindListItemMethodMarketPrice(ObservableCollection<MarketPriceItem> MktItems)
        {          
            var results = MktItems.FirstOrDefault(x => x.ProductCode == str_OrderID); 
            if (results!=null)
            {              
                if (orderModel != null)
                {
                    string str = results.Ask.ToString();
                    try
                    {
                        if (GosBzTool.IsAOValue(str))
                        {
                            orderModel.Ask = GosBzTool.SetIntAoValue();
                            this.btnQuickBuy.IsEnabled = false;
                        }
                        else
                        {
                            orderModel.Ask = GosBzTool.adjustDecLength(str_OrderID, str);
                            this.btnQuickBuy.IsEnabled = true;
                        }
                    }
                    catch { }
                    str = results.Bid.ToString();
                    try
                    {
                        if (GosBzTool.IsAOValue(str))
                        {
                            orderModel.Bid = GosBzTool.SetIntAoValue();
                            this.btn_QuickSell.IsEnabled = false;
                        }
                        else
                        {
                            orderModel.Bid = GosBzTool.adjustDecLength(str_OrderID, str);
                            this.btn_QuickSell.IsEnabled = true;
                        }
                    }
                    catch { }
                }
                CurrentProdMktItem = results;
            }
          //  ActiveBuySell();
        }
        #endregion       

        protected void distributeMsg_AddOrderInfo(object sender, String str)
        {
            Application.Current.Dispatcher.Invoke(DisOrderConfirmDelegate, new object[] { str });
            
        }

        protected void distributeMsg_GetNotification(object sender, String str)
        {
            Application.Current.Dispatcher.BeginInvoke(this.NotificationDelegate, new Object[] { str });
        }
       
        public void buySell(string strCount)
        {
            int count = Convert.ToInt32(strCount);
            if (count > 0)
            {  
                LinearGradientBrush myLinearGradientBrush = new LinearGradientBrush();
                myLinearGradientBrush.StartPoint = new Point(0.504, 0.03);
                myLinearGradientBrush.EndPoint = new Point(0.504, 1.5);
                myLinearGradientBrush.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#FFF3D0F3"), 0.042));
                myLinearGradientBrush.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#FFFF84EE"), 0.925));
                b_ClosePosition.Background = myLinearGradientBrush;

                this.btn_sell.Visibility = Visibility.Visible;                 
                this.btn_QuickSell.Visibility = Visibility.Visible;
                this.btn_buy.Visibility = Visibility.Hidden; 
                this.btnQuickBuy.Visibility = Visibility.Hidden;
                BuyOrSell = TradeStationComm.Attribute.BuySell.sell;
            }
            else if (count < 0)
            {             
                LinearGradientBrush myLinearGradientBrush = new LinearGradientBrush();
                myLinearGradientBrush.StartPoint = new Point(0.504, 0.03);
                myLinearGradientBrush.EndPoint = new Point(0.504, 1.5);
                myLinearGradientBrush.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#FFD7D9F0"), 0.042));
                myLinearGradientBrush.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#FF5187D4"), 0.925));

                b_ClosePosition.Background = myLinearGradientBrush;
                this.btn_sell.Visibility = Visibility.Hidden;
                this.btn_QuickSell.Visibility = Visibility.Hidden;
                this.btn_buy.Visibility = Visibility.Visible;
                this.btnQuickBuy.Visibility = Visibility.Visible;
                BuyOrSell = TradeStationComm.Attribute.BuySell.buy;
            }
        }

        private void btn_sell_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput())
            {
                tbPriceFocus();
                return;
            }
            try
            {
                ComboBoxItem itemQTradeValidType = cmbValidType.SelectedItem as ComboBoxItem;

                TradeStationComm.Attribute.ValidType valid = TradeStationComm.Attribute.ValidType.today;
                if (itemQTradeValidType != null)
                {
                    bool b = Enum.TryParse<TradeStationComm.Attribute.ValidType>(itemQTradeValidType.Tag.ToString(), true, out valid);
                }
                decimal dPrice = getTxtPrice();
               
                string strMessage = string.Format(CommonRsText.strRs_confirm_SellOrder,//"Account:{0}\nCmd:Sell\nId：{1}\nPrice：{2}\nQty：{3} \nValidity:{4}",
                    ACC,
                    orderModel.ProductCode, dPrice, orderModel.Count,
                    CommonRsText.getValidTypeText(valid) + ((chkT1.IsChecked ?? false) == true ? ",T+1" : ""));
               
                int intOrderPrice = GosBzTool.ChangeDecToIn(orderModel.ProductCode, dPrice);           
              
                MsgForm msgbox = new MsgForm(strRs_Conf_SellOrder_Title, strMessage, 15);
                Window w = Window.GetWindow(this);
                if (w != null)
                {
                    msgbox.Owner = w;
                }
                bool? Yes = msgbox.ShowDialog();
                if (Yes == true)
                {
                    if (GOSTradeStation.msgChannel == null)
                    {
                        return;
                    }
                    TradeStationLog.WriteLogPerformance(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + " close position, sell:" + strMessage);
                    if (GOSTradeStation.isDealer)
                    {
                        GOSTradeStation.msgChannel.Send(TradeStationComm.MsgRequest.getMsgAddOrder(GOSTradeStation.UserID, GOSTradeStation.Pwd, ACC,
                                                   new TradeStationComm.MsgRequest.OrderClass.AddOrderClass(orderModel.ProductCode, 
                                                       TradeStationComm.Attribute.BuySell.sell,
                                                       orderModel.Count,
                                                       intOrderPrice, 
                                                       valid, 
                                                       TradeStationComm.Attribute.CondType.normal, 
                                                       TradeStationComm.Attribute.StopType.normalOrder,
                                                       0, 
                                                       TradeStationComm.Attribute.Active.active, 
                                                       0, 
                                                       TradeStationComm.Attribute.AE.AE, 
                                                       ACC,
                                                          "",
                            orderModel.TPlus)
                                                 ));

                        TradeStationSend.Send(cmdClient.getOrderBookInfo, ACC);
                        TradeStationSend.Send(cmdClient.getAccountInfo, ACC);
                        TradeStationSend.Send(cmdClient.getPositionInfo, ACC);
                    }
                    else
                    {
                        GOSTradeStation.msgChannel.Send(TradeStationComm.MsgRequest.getMsgAddOrder(GOSTradeStation.UserID, GOSTradeStation.Pwd, "",
                                                   new TradeStationComm.MsgRequest.OrderClass.AddOrderClass(orderModel.ProductCode, 
                                                       TradeStationComm.Attribute.BuySell.sell,
                                                       orderModel.Count,
                                                       intOrderPrice, 
                                                       valid, 
                                                       TradeStationComm.Attribute.CondType.normal,
                                                       TradeStationComm.Attribute.StopType.normalOrder, 
                                                       0, 
                                                       TradeStationComm.Attribute.Active.active, 
                                                       0, 
                                                       TradeStationComm.Attribute.AE.normalUser, 
                                                       "",
                                                          "",
                            orderModel.TPlus
                                                       )
                                                 ));

                        TradeStationSend.Send(cmdClient.getOrderBookInfo);
                        TradeStationSend.Send(cmdClient.getAccountInfo);
                        TradeStationSend.Send(cmdClient.getPositionInfo);
                    }
                    mdiClosePosition.Close();
                    return;
                }
            }
            catch (Exception)
            {               
                MessageBox.Show(strRs_SellOrder_Fail);
            }
            tbPriceFocus();
        }

        private void btn_buy_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput())
            {
                tbPriceFocus();
                return;
            }
            try
            {
                ComboBoxItem itemQTradeValidType = cmbValidType.SelectedItem as ComboBoxItem;
                TradeStationComm.Attribute.ValidType valid = TradeStationComm.Attribute.ValidType.today;
                if (itemQTradeValidType != null)
                {
                    bool b = Enum.TryParse<TradeStationComm.Attribute.ValidType>(itemQTradeValidType.Tag.ToString(), true, out valid);
                }
                decimal dPrice = getTxtPrice();
                string strMessage = string.Format(CommonRsText.strRs_confirm_BuyOrder,//"Accout:{0}\nCmd:Buy\nId：{1}\nPrice：{2}\nQty：{3}\nValidity:{4}",
                    ACC,
                    str_OrderID, dPrice, orderModel.Count,CommonRsText.getValidTypeText(valid) + ((chkT1.IsChecked ?? false) == true ? ",T+1" : ""));


                MsgForm msgbox = new MsgForm(strRs_confirm_BuyOrder_Title, strMessage, 15);
                Window w = Window.GetWindow(this);
                if (w != null)
                {
                    msgbox.Owner = w;
                }
                bool? Yes = msgbox.ShowDialog();               
                if (Yes == true)
                {
                    if (GOSTradeStation.msgChannel == null)
                    {
                        return;
                    }
                    TradeStationLog.WriteLogPerformance(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + " close position, buy:" + strMessage);                    
                    int intOrderPrice = GosBzTool.ChangeDecToIn(orderModel.ProductCode, dPrice);
                    if (GOSTradeStation.isDealer)
                    { 
                        GOSTradeStation.msgChannel.Send(TradeStationComm.MsgRequest.getMsgAddOrder(GOSTradeStation.UserID, GOSTradeStation.Pwd, ACC,
                                                    new TradeStationComm.MsgRequest.OrderClass.AddOrderClass(orderModel.ProductCode,
                                                        TradeStationComm.Attribute.BuySell.buy,
                                                        orderModel.Count,
                                                        intOrderPrice,
                                                        valid,
                                                        TradeStationComm.Attribute.CondType.normal,
                                                        TradeStationComm.Attribute.StopType.normalOrder,
                                                        0,
                                                        TradeStationComm.Attribute.Active.active,
                                                        0,
                                                        TradeStationComm.Attribute.AE.normalUser,
                                                        ACC,
                                                        "",
                                                        orderModel.TPlus
                                                        )
                                                    ));

                        TradeStationSend.Send(cmdClient.getOrderBookInfo, ACC);
                        TradeStationSend.Send(cmdClient.getAccountInfo, ACC);
                        TradeStationSend.Send(cmdClient.getPositionInfo, ACC);                        
                    }
                    else
                    {
                        GOSTradeStation.msgChannel.Send(TradeStationComm.MsgRequest.getMsgAddOrder(GOSTradeStation.UserID, GOSTradeStation.Pwd, "",
                                                    new TradeStationComm.MsgRequest.OrderClass.AddOrderClass(orderModel.ProductCode, 
                                                        TradeStationComm.Attribute.BuySell.buy, 
                                                        orderModel.Count,
                                                        intOrderPrice,
                                                        valid, 
                                                        TradeStationComm.Attribute.CondType.normal,
                                                        TradeStationComm.Attribute.StopType.normalOrder, 
                                                        0, 
                                                        TradeStationComm.Attribute.Active.active, 
                                                        0, 
                                                        TradeStationComm.Attribute.AE.normalUser, 
                                                        "",
                                                         "",
                                                        orderModel.TPlus
                                                        )
                                                  ));

                        TradeStationSend.Send(cmdClient.getOrderBookInfo);
                        TradeStationSend.Send(cmdClient.getAccountInfo);
                        TradeStationSend.Send(cmdClient.getPositionInfo);
                    }
                }
                mdiClosePosition.Close();
                return;
            }
            catch (Exception)
            {
                MessageBox.Show(strRs_BuyOrder_Fail);
            }
            tbPriceFocus();
        }

        private void btnQuickBuy_Click(object sender, RoutedEventArgs e)
        {
            if (!(checkQuickPrice(orderModel.AskTo)&&  checkQtyInput())) //ValidateInput())
            {
                tbQuickTrickFocus();
                return;
            }
            ComboBoxItem itemQTradeValidType = cmbValidType.SelectedItem as ComboBoxItem;
            TradeStationComm.Attribute.ValidType valid = TradeStationComm.Attribute.ValidType.today;
            if (itemQTradeValidType != null)
            {
                bool b = Enum.TryParse<TradeStationComm.Attribute.ValidType>(itemQTradeValidType.Tag.ToString(), true, out valid);
            }
            string strMessage = string.Format(CommonRsText.strRs_confirm_BuyOrder,// "Acc:{0}\nCmd:Buy\nId：{1}\nPrice：{2}\nQty：{3}\nValidity:{4}",
                 ACC,
                orderModel.ProductCode, 
                orderModel.AskTo, 
                orderModel.Count,
                CommonRsText.getValidTypeText(valid) + ((chkT1.IsChecked ?? false) == true ? ",T+1" : ""));
            try
            {
                MsgForm msgbox = new MsgForm(this.strRs_confirm_BuyOrder_Title, strMessage, 15);
                Window w = Window.GetWindow(this);
                if (w != null)
                {
                    msgbox.Owner = w;
                }
                bool? Yes = msgbox.ShowDialog();
               
                //if (MessageBox.Show(strMessage, "Buy Confirm ", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                if (Yes == true)
                {
                    int intAskTo = GosBzTool.ChangeDecToIn(orderModel.ProductCode, orderModel.AskTo);
                    if (GOSTradeStation.msgChannel == null)
                    {                      
                        return;
                    }
                    TradeStationLog.WriteLogPerformance(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + " close position, quick buy:" + strMessage);
                    if (GOSTradeStation.isDealer)
                    {
                        GOSTradeStation.msgChannel.Send(TradeStationComm.MsgRequest.getMsgAddOrder(GOSTradeStation.UserID, GOSTradeStation.Pwd, ACC,
                            new TradeStationComm.MsgRequest.OrderClass.AddOrderClass(orderModel.ProductCode,
                                TradeStationComm.Attribute.BuySell.buy,
                                orderModel.Count,
                                intAskTo,
                                valid,
                                TradeStationComm.Attribute.CondType.normal,
                                orderModel.StopType,
                                orderModel.StopPrice,
                                TradeStationComm.Attribute.Active.active,
                                0,
                                TradeStationComm.Attribute.AE.AE,
                                ACC,
                                "",
                                orderModel.TPlus                           
                                )
                        ));
                        TradeStationSend.Send(cmdClient.getOrderBookInfo, ACC);
                        TradeStationSend.Send(cmdClient.getAccountInfo, ACC);
                        TradeStationSend.Send(cmdClient.getPositionInfo, ACC);
                    }
                    else
                    {                       
                        GOSTradeStation.msgChannel.Send(TradeStationComm.MsgRequest.getMsgAddOrder(GOSTradeStation.UserID, GOSTradeStation.Pwd, ACC,
                            new TradeStationComm.MsgRequest.OrderClass.AddOrderClass(orderModel.ProductCode,
                                TradeStationComm.Attribute.BuySell.buy,
                                orderModel.Count,
                               intAskTo,
                                valid,
                                TradeStationComm.Attribute.CondType.normal,
                                orderModel.StopType,
                                orderModel.StopPrice,
                                TradeStationComm.Attribute.Active.active,
                                0,
                                TradeStationComm.Attribute.AE.normalUser,
                                "",
                                  "",
                               orderModel.TPlus                          
                                )));

                        TradeStationSend.Send(cmdClient.getOrderBookInfo);
                        TradeStationSend.Send(cmdClient.getAccountInfo);
                        TradeStationSend.Send(cmdClient.getPositionInfo);
                    }

                    mdiClosePosition.Close();
                    return;
                }
            }
            catch { }
            tbQuickTrickFocus();
        }


        private void btnQuickSell_Click(object sender, RoutedEventArgs e)
        {
            if (!(checkQuickPrice(orderModel.BidTo) && checkQtyInput()))//ValidateInput())
            {
                tbQuickTrickFocus();
                return;
            }
            ComboBoxItem itemQTradeValidType = cmbValidType.SelectedItem as ComboBoxItem;
            TradeStationComm.Attribute.ValidType valid = TradeStationComm.Attribute.ValidType.today;
            if (itemQTradeValidType != null)
            {
                bool b = Enum.TryParse<TradeStationComm.Attribute.ValidType>(itemQTradeValidType.Tag.ToString(), true, out valid);
            }

            string strMessage = string.Format(CommonRsText.strRs_confirm_SellOrder,// "Acc:{0}\nCmd:Sell\nId：{1}\nPrice：{2}\nQty：{3}\nValidity:{4}",
                ACC,
                orderModel.ProductCode,
                orderModel.BidTo, 
                orderModel.Count,
                CommonRsText.getValidTypeText(valid) + ((chkT1.IsChecked ?? false) == true ? ",T+1" : ""));

            MsgForm msgbox = new MsgForm(this.strRs_Conf_SellOrder_Title,strMessage, 15);
            Window w = Window.GetWindow(this);
            if (w != null)
            {
                msgbox.Owner = w;
            }

            bool? Yes = msgbox.ShowDialog();           
            if(Yes==true)
            {
                if (GOSTradeStation.msgChannel == null)
                {                  
                    tbQuickTrickFocus();
                    return;
                }
                try
                {
                    int intBidTo = GosBzTool.ChangeDecToIn(orderModel.ProductCode, orderModel.BidTo);                    
                    TradeStationLog.WriteLogPerformance(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + " close position, quick sell:" + strMessage);
                    if (GOSTradeStation.isDealer)
                    {
                        GOSTradeStation.msgChannel.Send(TradeStationComm.MsgRequest.getMsgAddOrder(GOSTradeStation.UserID,
                            GOSTradeStation.Pwd,
                            ACC,
                            new TradeStationComm.MsgRequest.OrderClass.AddOrderClass(orderModel.ProductCode,
                                TradeStationComm.Attribute.BuySell.sell,
                                orderModel.Count,
                                intBidTo,
                                valid,
                                TradeStationComm.Attribute.CondType.normal,
                                orderModel.StopType,
                                orderModel.StopPrice,
                                TradeStationComm.Attribute.Active.active,
                                0,
                                TradeStationComm.Attribute.AE.AE,
                                ACC,
                                "",
                                orderModel.TPlus                          
                                )
                        ));
                        TradeStationSend.Send(cmdClient.getOrderBookInfo, ACC);
                        TradeStationSend.Send(cmdClient.getAccountInfo, ACC);
                        TradeStationSend.Send(cmdClient.getPositionInfo, ACC);
                    }
                    else
                    {
                        GOSTradeStation.msgChannel.Send(TradeStationComm.MsgRequest.getMsgAddOrder(GOSTradeStation.UserID, GOSTradeStation.Pwd, ACC,
                            new TradeStationComm.MsgRequest.OrderClass.AddOrderClass(orderModel.ProductCode,
                                TradeStationComm.Attribute.BuySell.sell,
                                orderModel.Count,
                                intBidTo,
                                valid,
                                TradeStationComm.Attribute.CondType.normal,
                                orderModel.StopType,
                                orderModel.StopPrice,
                                TradeStationComm.Attribute.Active.active,
                                0,
                                TradeStationComm.Attribute.AE.normalUser,
                                "",
                                "",
                                orderModel.TPlus                           
                                )));

                        TradeStationSend.Send(cmdClient.getOrderBookInfo);
                        TradeStationSend.Send(cmdClient.getAccountInfo);
                        TradeStationSend.Send(cmdClient.getPositionInfo);
                    }

                    mdiClosePosition.Close();
                    return;
                }
                catch { }
            }
            tbQuickTrickFocus();
        }      

        public void OpenOrderConfrim(string str)
        {
            MdiContainer Container = App.Current.MainWindow.FindName("Container") as MdiContainer;
            MdiChild orderChild = new MdiChild();
            OrderConfirm OrderConfirm = new OrderConfirm(orderChild);
            OrderConfirm.lbl_Message.Text = str;
            OrderConfirm.lbl_Message.Foreground = Brushes.Red;
            
            orderChild.Title = "Order Confirm";
            orderChild.Content = OrderConfirm;
            orderChild.Width = OrderConfirm.Width;
            orderChild.Height = OrderConfirm.Height;
            orderChild.Position = new System.Windows.Point(0, 0);
            Container.Children.Add(orderChild);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }      

        void switchButton(bool bAppend)
        {
            //if (bAppend)
            {
                if (BuyOrSell == TradeStationComm.Attribute.BuySell.sell)
                {
                    btn_sell.Visibility = bAppend ? Visibility.Hidden : Visibility.Visible;
                    btn_QuickSell.Visibility = bAppend ? Visibility.Visible : Visibility.Hidden;

                    btn_buy.Visibility = Visibility.Hidden;
                    btnQuickBuy.Visibility = Visibility.Hidden;
                }
                else if (BuyOrSell == TradeStationComm.Attribute.BuySell.buy)
                {
                    btn_buy.Visibility = bAppend ? Visibility.Hidden : Visibility.Visible;
                    btnQuickBuy.Visibility = bAppend ? Visibility.Visible : Visibility.Hidden;

                    btn_sell.Visibility = Visibility.Hidden;
                    btn_QuickSell.Visibility = Visibility.Hidden;
                }
            }
          
        }

        void ActiveBuySell()
        {          
      
            if (BuyOrSell == TradeStationComm.Attribute.BuySell.sell)
            {
                if (orderModel.Bid == 0)
                    btn_QuickSell.IsEnabled = false;
                else
                {
                    btn_QuickSell.IsEnabled = true;
                }
            }
            else if (BuyOrSell == TradeStationComm.Attribute.BuySell.buy)
            {
                if (orderModel.Ask == 0)
                    btnQuickBuy.IsEnabled = false;
                else
                {
                    btnQuickBuy.IsEnabled = true;
                }
            }
        }

        bool ValidateInput()
        {
            return (checkPrice() && checkQtyInput());           
        }

        bool checkPrice()
        {
            if (this.txt_Price.Text.Trim() == "")
            {
                MessageBox.Show(CommonRsText.StrRs_Order_Pls_Price);
                return false;
            }
            decimal d = 0M;
            try
            {
                d=Convert.ToDecimal(this.txt_Price.Text);
            }
            catch (Exception)
            {

                MessageBox.Show(CommonRsText.StrRs_Order_Invalid_Price );
                return false;
            }

            ProductBoundInfo BoundInfo = GosBzTool.getProdBoundInfo(orderModel.ProductCode);
            if (BoundInfo.DecPriceUp.HasValue)
            {
                if (BoundInfo.DecPriceUp.Value < d)
                {
                    MessageBox.Show(string.Format(CommonRsText.strRs_Price_Shlnot_ULimit,BoundInfo.DecPriceUp.Value.ToString()));//"price should not exceeded upper limit " + BoundInfo.DecPriceUp.Value.ToString());
                    return false;
                }
            }

            if (BoundInfo.DecPriceLower.HasValue)
            {
                if (BoundInfo.DecPriceLower.Value > d)
                {
                    MessageBox.Show(string.Format(CommonRsText.strRs_Price_Shlnot_LowLimit, BoundInfo.DecPriceLower.Value.ToString()));
                    //MessageBox.Show( "price should not less than lower limit " + BoundInfo.DecPriceLower.Value.ToString());
                    return false;
                }
            }

            if (GosBzTool.CheckDeviationPrice(d, CurrentProdMktItem) == false)
            {
                return false;
            }

           // str_OrderID
            if (!GosBzTool.CheckDecLength(orderModel.ProductCode, d))
            {
                return false;
            }

            return true;
        }

        //2014-10-15
        bool checkQuickPrice(decimal dPrice)
        {
            decimal d = dPrice;
            ProductBoundInfo BoundInfo = GosBzTool.getProdBoundInfo(orderModel.ProductCode);
            if (BoundInfo.DecPriceUp.HasValue)
            {
                if (BoundInfo.DecPriceUp.Value < d)
                {
                    MessageBox.Show(string.Format(CommonRsText.strRs_Price_Shlnot_ULimit, BoundInfo.DecPriceUp.Value.ToString()));
                    return false;
                }
            }

            if (BoundInfo.DecPriceLower.HasValue)
            {
                if (BoundInfo.DecPriceLower.Value > d)
                {
                    MessageBox.Show(string.Format(CommonRsText.strRs_Price_Shlnot_LowLimit, BoundInfo.DecPriceLower.Value.ToString()));                   
                    return false;
                }
            }

            if (GosBzTool.CheckDeviationPrice(d, CurrentProdMktItem) == false)
            {
                return false;
            }

            // str_OrderID
            if (!GosBzTool.CheckDecLength(orderModel.ProductCode, d))
            {
                return false;
            }

            return true;
        }

        bool checkQtyInput()
        {
            string strQty=this.txt_Count.Text.Trim();
            if (strQty == "")
            {
                MessageBox.Show(CommonRsText.strRs_Order_Qty_Pls);
                return false;
            }
            try
            {
                Convert.ToUInt32(this.txt_Count.Text);
            }
            catch (Exception)
            {
                MessageBox.Show(CommonRsText.strRs_Order_Qty_Invalid);
                return false;
            }

            ProductBoundInfo BoundInfo = GosBzTool.getProdBoundInfo(orderModel.ProductCode);
            int intQty = Convert.ToInt32(strQty);
            if (intQty == 0)
            {
                MessageBox.Show(CommonRsText.strRs_Order_Qty_Invalid);
                return false;
            }
            if (BoundInfo.IntQtyUp.HasValue)
            {
                if (BoundInfo.IntQtyUp.Value < intQty)
                {
                    MessageBox.Show(string.Format(CommonRsText.strRs_Qty_UP_Limit, BoundInfo.IntQtyUp.Value.ToString()));//  "qty exceeded Upper limit " + BoundInfo.IntQtyUp.Value.ToString());
                    return false;
                }
            }
            if (BoundInfo.IntQtyLower.HasValue)
            {
                if (BoundInfo.IntQtyLower.Value > intQty)
                {
                    //MessageBox.Show("qty should be less than lower limit " + BoundInfo.IntQtyLower.Value.ToString());
                    MessageBox.Show(string.Format(CommonRsText.strRs_Qty_ShdNot_LessThan_LowLimit, BoundInfo.IntQtyLower.Value.ToString()));//
                    return false;
                }
            }
            if (GosBzTool.CheckDeviationQty(intQty) == false)
            {
                return false;
            }
           
            return true;
        }

        private void btn_QuickSell_MouseEnter_1(object sender, MouseEventArgs e)
        {
            btn_QuickSell.Foreground = Brushes.Black;
        }

        private void UserControl_Unloaded_1(object sender, RoutedEventArgs e)
        {
            distributeMsg.DisMarketPrice -= new MessageDistribute.OnDisMarketPrice(distributeMsg_DisMarketPrice);
            decTbCheck.UnBindWheel();
            if (orderModel != null)
            {
                if (orderModel.ProductCode != null)
                {
                    if (GOSTradeStation.IsWindowInitialized)
                    TradeStationSend.Send(orderModel.ProductCode, null, cmdClient.registerMarketPrice);
                }
            }
            closePosition = null;
        }


        void chkT1_Unchecked(object sender, RoutedEventArgs e)
        {
            if (orderModel != null) 
            orderModel.TPlus = TradeStationComm.Attribute.TOne.TOnly;
        }

        void chkT1_Checked(object sender, RoutedEventArgs e)
        {
            if (orderModel != null) 
            orderModel.TPlus = TradeStationComm.Attribute.TOne.TPluseOne;
        }       

        void setTxtPrice(string strPric)
        {
            decimal decNum = Utility.ConvertToDecimal(strPric);
            decNum=GosBzTool.CheckPriceBound(decNum);
            this.txt_Price.Text = GosBzTool.adjustDecLength(orderModel.ProductCode, decNum.ToString()).ToString();       
        }

        decimal getTxtPrice()
        {           
             
            {
                try
                {
                    decimal i = Convert.ToDecimal(this.txt_Price.Text);
                    return i;
                }
                catch
                {
                    return 0;
                }
            }
        }



        void SetUPDownBtnStep()
        {

            if (orderModel.ProductCode == null) return;
            string _prodCode = orderModel.ProductCode.Trim();
            if (_prodCode == null) return;
            if (_prodCode == "")
            {
                this.btnPriceDown.Step = 1;
                this.btnPriceUp.Step = 1;
                this.btnTickDown.Step = 1;
                this.btnTickUp.Step = 1;
                return;
            }
            int len = GosBzTool.getDecLen(_prodCode);
            if (len > 0)
            {
                decimal dec = Convert.ToDecimal(Math.Pow(10, len));
                dec = 1 / dec;
                this.btnPriceDown.Step = dec;
                this.btnPriceUp.Step = dec;
                this.btnTickDown.Step = dec;
                this.btnTickUp.Step = dec;
             
            }
            else
            {
                this.btnPriceDown.Step = 1;
                this.btnPriceUp.Step = 1;
                this.btnTickDown.Step = 1;
                this.btnTickUp.Step = 1;
            }
            return;

        }

        void tbPriceFocus()
        {
            if (txt_Price != null)
            {
                txt_Price.Focus();
            }
        }

        void tbQuickTrickFocus()
        {
            if (txt_Add != null)
            {
                txt_Add.Focus();
            }
        }

        public void ChangLangInRuntime()
        {
            string strProdCode = "";
            if (this.orderModel != null)
            {
                if (orderModel.ProductCode != null)
                {
                    strProdCode = orderModel.ProductCode;
                }
            }
            if (strProdCode == "")
            {
                setFormTitle(0, this.distributeMsg, this.mdiClosePosition,null);
            }
            else
            {
                setFormTitle(1, this.distributeMsg, this.mdiClosePosition, new string[]{strProdCode});
            }
        }
        public string TitleFormat
        {
            get {
                return GOSTS.GosCulture.CultureHelper.GetString(this.GetType().Name.ToString().ToLower() + "Title");
            }
        }
        public void setFormTitle(int _iType, MessageDistribute _msgDistribute,MdiChild _mdiChild,params string[] _params)
        {
            string title = "";
            switch (_iType)
            {
                case 0:
                    title= TitleFormat.Split('|')[0];
                    break;
                case 1:
                    title= string.Format(TitleFormat, Utility.getArrayItemValue(_params, 0));
                    title = title.Replace("|", "");
                    break;
            }
            GosBzTool.setTitle(mdiClosePosition, _msgDistribute, title);
        }
    }
}
