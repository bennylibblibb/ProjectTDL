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
using System.Windows.Shapes;
using GOSTS;
using GOSTS.Common;
using System.Data;
using System.Threading;
using WPF.MDI;
using System.ComponentModel;
using System.Collections;
using System.Collections.ObjectModel;
using System.Globalization;


namespace GOSTS
{
    /// <summary>
    /// UserOrderInfo.xaml 的交互逻辑
    /// </summary>
    /// 

    public partial class UserOrderInfo : IDelearUser, GOSTS.IChangeLang 
    {

        public MdiChild mdiChild { get; set; }
        delegate void NotificationItem(string str);
        NotificationItem NotificationDelegate;

        delegate void BingUserOrderInfoData(DataTable dtOrderBook, string UserID, string Acc, DateTime? dt);
        BingUserOrderInfoData UserOrderInfoData;

        delegate void BingClearTradeInfoData(DataTable dtClearTrade, string _userID, string _Acc, DateTime? _dt);
        BingClearTradeInfoData ClearTradeInfoData;

        delegate void BingDoneTradeInfoData(DataTable dtDoneTrade, string _userID, string _Acc, DateTime? _dt);
        BingDoneTradeInfoData DoneTradeInfoData;

        delegate void BingPositionInfoData(DataTable dtPosition, string UserID, string Acc, DateTime? dt);
        BingPositionInfoData PositionInfoData;

        delegate void DisOrderConfirm(string str);
        DisOrderConfirm DisOrderConfirmDelegate;

        //  delegate void BindListItemMarketPrice(DataTable dataTable);
        //  BindListItemMarketPrice MarketPriceDelegate;

        string OrderSelectIndex, OrderProductCode = "";
        int OrderPrice = 0;
        string strRealOrderPrice ="0";
        uint OrderCount = 0;
        string strMessage = "";
        string strBuySell = "";
        TradeStationComm.Attribute.BuySell buySell;
       
        UserData userAccountID = new UserData();

        #region rs text
        string strRs_UOI_Pls_selectRow
        {
            get { return GOSTS.GosCulture.CultureHelper.GetString("UOI_Pls_selectRow"); }
        }
        #endregion

        public UserOrderInfo(MessageDistribute _msgDistribute,MdiChild _mdiChild)
        {
            mdiChild = _mdiChild;
            DisOrderConfirmDelegate = new DisOrderConfirm(OpenOrderConfrim);
            UserOrderInfoData = new BingUserOrderInfoData(BindUserOrderInfoItemMethod);
            ClearTradeInfoData = new BingClearTradeInfoData(BindClearTradeInfoItemMethod);
            DoneTradeInfoData = new BingDoneTradeInfoData(BindDoneTradeItemMethod);          
            NotificationDelegate = new NotificationItem(NotificationItemMethod);          

            InitializeComponent();
           
            this.tabControl1.SelectionChanged += tabControl1_SelectionChanged;           
            dgPosition.SelectionChanged += dgPosition_SelectionChanged_1;
            if (GOSTradeStation.isDealer)
            {              
                AddsRightContextMenu();
            }
            InitCurUser(this.cbbUsers, _msgDistribute);
            SetFormTitle(0, _msgDistribute, mdiChild, null);
            enableOrderButton(false);
            btnPosClose.IsEnabled = false;

            if (_msgDistribute != null)
            {
                distributeMsg = _msgDistribute;
                distributeMsg.DisUserOrderInfo += new MessageDistribute.onDisUserOrderInfo(distributeMsg_UserOrderInfoItem);
                distributeMsg.DisClearTradeInfo += new MessageDistribute.onDisClearTradeInfo(distributeMsg_ClearTradeInfoItem);
                distributeMsg.DisDoneTradeInfo += new MessageDistribute.onDisDoneTradeInfo(distributeMsg_DisDoneTradeInfo);              
            
                #region mk-pos
                PosBus = PositionBus.GetSinglePositionBus(_msgDistribute);
                if (PosBus != null)
                {
                    PosDataChange = new PosInfoChange(PositionBusDataChange);
                    PosBus.RegisterOnPosInfoChange(new PositionBus.PosInfoChange(PosBus_DataChangeInvoke));
                    dgPosition.ItemsSource = PosBus.posCollection;                    
                }
                //dgPosition.Sorting += dgPosition_Sorting;
                #endregion
              }
           
            if (GOSTradeStation.UserID != null)
            {
                if (GOSTradeStation.UserID.Trim() != "")
                {
                    PersonTransactionOrderHeight orderHConfig = new PersonTransactionOrderHeight();
                    PersonTransactionOrderHeight.Height = orderHConfig.ReadHeight(GOSTradeStation.UserID);
                    if (PersonTransactionOrderHeight.Height.HasValue)
                    {
                        rowOrder.Height = new GridLength(PersonTransactionOrderHeight.Height.Value);
                    }
                }
            }
            if (mdiChild != null)
            {
                mdiChild.OnDragThumbClick += new MdiChild.deleDragThumbClick(this.SetSelect);
            }
           // PersonTransactionOrderHeight.userOrderInfo = this;
        }

        #region position data receive
        delegate void PosInfoChange(ObservableCollection<OrderPosition> posCL, PosChangeType sType,string _Acc);
        PosInfoChange PosDataChange;
        protected void PosBus_DataChangeInvoke(object sender, ObservableCollection<OrderPosition> posCL, PosChangeType sType,string _Acc)
        {
            //Application.Current.Dispatcher.BeginInvoke(PosDataChange, new Object[] { posCL, sType,_Acc });
            PosDataChange(posCL, sType, _Acc);
        }

        public void PositionBusDataChange(ObservableCollection<OrderPosition> posCL, PosChangeType sType,string _Acc)
        {
            if (this.CurrentID != _Acc)
            {
                Dispatcher.Invoke((Action)(() =>
                {
                    this.dgPosition.ItemsSource = null;
                }),
                null);
                return;
            }
            if (sType == PosChangeType.PosChange)
            {
                Dispatcher.Invoke((Action)(() =>
                {
                    this.btnPosClose.IsEnabled = false;
                }),
                null);
            }
            string str = "";
            foreach (OrderPosition o in posCL)
            {
                str += ";" + o.ProductCode;
            }
          //  TradeStationLog.WriteQCLog(DateTime.Now.ToString("yyMMdd-HHmmss ") + "position in userorderinfo set itemsource,product: " + str);
            Dispatcher.Invoke((Action)(() =>
            {
                if (this.dgPosition.ItemsSource == null)
                {
                    if (this.PosBus != null)
                    {
                        this.dgPosition.ItemsSource = PosBus.posCollection;
                    }
                }
                this.posCollection = posCL;

                // this.dgPosition.ItemsSource = this.posCollection;
                if (this.posCollection.Count < 1)
                {
                    this.btnPosClose.IsEnabled = false;
                }
            }),
            null);
            // else if (sType == PosChangeType.MKChange)
           
        }
        #endregion


        public override void ClearControlData()
        {
            base.ClearControlData();
            //清空旧的数据
            this.dg_ClearTrade.ItemsSource = null;
            this.dg_tabOrder_ClearTrade.ItemsSource = null;
            this.dg_DoneTrade.ItemsSource = null;
           // this.dg_OrderDoneTrade.ItemsSource = null;
            this.dg_OrderBook.ItemsSource = null;
            this.dgPosition.ItemsSource = null;
            posCollection.Clear();
            if (mdiChild != null)
            {              
                SetFormTitle(0, distributeMsg, mdiChild, null);
            }
          
        }
        #region AccontMaster
        void clearACMast()
        {
            if (mdiChild != null)
            {
                SetFormTitle(0, distributeMsg, mdiChild, null);
            }
        }
        
        public override void GetAccountMaster(object sender, DataTable tableMaster,string RecAcc)
        {
            base.GetAccountMaster(sender, tableMaster, RecAcc);
            if (tableMaster == null)
            {
                clearACMast();
                return;
            }
            if (tableMaster.Rows.Count < 1)
            {
                clearACMast();
                return;
            }
            if (!tableMaster.Columns.Contains("accNo"))
            {
                clearACMast();
                return;
            }
            DataRow[] drs=tableMaster.Select("accNo='"+this.CurrentID+"'"); //.Rows[0];
            if (drs.Length < 1)
            {
                clearACMast();
                return;
            }
            DataRow dr = drs[0];
            string Sex = FormatSex(dr.getColValue("sex"));
            string accName = dr.getColValue("accName");
            string idBrNo = dr.getColValue("idBrNo");
            if (idBrNo != "")
            {
                idBrNo = ",HKID:" + idBrNo;
            }
            else
            {
                idBrNo = "";
            }
            if (mdiChild != null)
            {
               // mdiChild.Title = "Transactions - [" + this.CurrentID.ToString() + "<" + Sex + " " + TradeStationTools.Base64Utf16StringToString(accName) + idBrNo + @">]";
               // SetMdITitle(distributeMsg, mdiChild,"Transactions - [" + this.CurrentID.ToString() + "<" + Sex + " " + TradeStationTools.Base64Utf16StringToString(accName) + idBrNo + @">]");
                string[] _params=new string[]{this.CurrentID.ToString(),Sex,TradeStationTools.Base64Utf16StringToString(accName), idBrNo};
                SetFormTitle(1, distributeMsg, mdiChild, _params);
            }
        }

        public string FormatSex(string sex)
        {
            switch (sex)
            { 
                case "0":
                    return "Mr";
                case "1":
                    return "Miss";
                case "2":
                    return "Mrs";
            }
            return sex;
        }

        #endregion


        void tabControl1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source == this.tabControl1)
            {
                showHideOrderOperationButton();
                showHidePositionButton();
            }
        }



      //  public MessageDistribute distributeMsg = null;  

        public void Window_Loaded(object sender, RoutedEventArgs e)
        {
        }

        protected void distributeMsg_DisDoneTradeInfo(object sender, DataTable dtDoneTrade, string _userID, string _Acc, DateTime? _dt)
        {
            Application.Current.Dispatcher.Invoke(DoneTradeInfoData, new object[] { dtDoneTrade, _userID, _Acc, _dt });
        }


        protected void distributeMsg_UserOrderInfoItem(object sender, DataTable dtOrderBook, string _UserID, string _Acc, DateTime? _dt)
        {            
            BindUserOrderInfoItemMethod(dtOrderBook, _UserID, _Acc, _dt);
        }

        protected void distributeMsg_ClearTradeInfoItem(object sender, DataTable dtClearTrade, string _userID, string _Acc, DateTime? _dt)
        {
            BindClearTradeInfoItemMethod(dtClearTrade, _userID, _Acc, _dt);
        }


        public void NotificationItemMethod(string str)
        {
            if (str != null)
            {
                if (GOSTradeStation.msgChannel != null)
                {
                    GOSTradeStation.msgChannel.Send(TradeStationComm.MsgRequest.getMsgGetOrderBookInfo(GOSTradeStation.UserID));
                }
            }
        }

        protected void distributeMsg_GetNotification(object sender, String str)
        {
            Application.Current.Dispatcher.BeginInvoke(this.NotificationDelegate, new Object[] { str });
        }



        public void BindDoneTradeItemMethod(DataTable dtDoneTrade, string _userID, string _Acc, DateTime? _dt)
        {
            bingDoneTradeData(dtDoneTrade, _userID, _Acc, _dt);
        }

        public void BindUserOrderInfoItemMethod(DataTable dtOrderBook, string _UserId, string Acc, DateTime? dt)
        {
            bingOrderData(dtOrderBook, _UserId, Acc, dt);
        }

        public void BindClearTradeInfoItemMethod(DataTable dtClearTrade, string _UserId, string _Acc, DateTime? _dt)
        {
            bingClearTradeData(dtClearTrade, _UserId, _Acc, _dt);
        }

        public void bingDoneTradeData(DataTable dtDoneTrade, string _userID, string _Acc, DateTime? dt)
        {
            if (_userID != GOSTradeStation.UserID)
            {
                return;
            }
            if (GOSTradeStation.isDealer)
            {
                if (this.CurrentID == null || this.CurrentID.Trim() == "" || _Acc == null || _Acc.Trim() == "" || this.CurrentID != _Acc.Trim())
                {
                    return;
                }
            }
            
            if (dtDoneTrade != null)
            {
                #region for sort
                SetBOrSColumnInfo(dtDoneTrade, "BQty", "SQty", "bors");

                DataTable dtClone = dtDoneTrade.Clone();
                if (dtClone.Columns.Contains("price"))
                {
                    dtClone.Columns["price"].DataType = typeof(decimal);
                }
                if (dtClone.Columns.Contains("bQty"))
                {
                    dtClone.Columns["bQty"].DataType = typeof(int);
                }
                if (dtClone.Columns.Contains("sQty"))
                {
                    dtClone.Columns["sQty"].DataType = typeof(int);
                }
                dtClone.Columns.Add(new DataColumn("strNFPrice"));
                foreach (DataRow dr in dtDoneTrade.Rows)
                {
                    DataRow drNew = dtClone.NewRow();
                    foreach (DataColumn col in dtDoneTrade.Columns)
                    {
                        if (col.ColumnName.ToLower().Trim() == "price")
                        {
                            string productCode = dr.getColValue("productCode");
                            //decimal dPrice = GosBzTool.getDecimalPrice(productCode, dr.getDecimalValue(col.ColumnName, 0));
                            string strprice = GosBzTool.CalAndAdjustDecimalPrice(productCode, dr.getColValue(col.ColumnName)); //getAndAdjustAveragePrice
                             // GosBzTool.getDecimalPrice(productCode, dr.getDecimalValue(col.ColumnName, 0));
                            drNew[col.ColumnName] = strprice;// dr.getDecimalValue(col.ColumnName, 0);
                            drNew["strNFPrice"] = strprice;
                            continue;
                        }
                        if (col.ColumnName.ToLower().Trim() == "bqty")
                        {
                            drNew[col.ColumnName] = dr.getColIntValue(col.ColumnName, 0);
                            continue;
                        }

                        if (col.ColumnName.ToLower().Trim() == "sqty")
                        {
                            drNew[col.ColumnName] = dr.getColIntValue(col.ColumnName, 0);
                            continue;
                        }

                        drNew[col.ColumnName] = dr[col.ColumnName];
                    }
                    dtClone.Rows.Add(drNew);
                       #endregion
                }
                if (dtClone.Rows.Count > 0)
                {
                    this.dg_DoneTrade.ItemsSource = dtClone.DefaultView;
                   // this.dg_OrderDoneTrade.ItemsSource = dtClone.DefaultView;
                }
                else
                {
                    this.dg_DoneTrade.ItemsSource =null;
                  //  this.dg_OrderDoneTrade.ItemsSource =null;
                }
               
            }
           
        }


        public class BoxComp : IComparer<DataRow>
        {
          
            public int Compare(DataRow x, DataRow y)
            {              
                return 0;
            }
        }

        public void bingOrderData(DataTable dtOrderBook, string _UserID, string _Acc, DateTime? _dt)
        { 
          
            if (bInOrderBookDeleting)
            {
                return;
            }
            if (bInOrderBookActiving)
            {
                return;
            }
            if (_UserID == null || _UserID.Trim() == "" || _UserID != GOSTradeStation.UserID)
            {
                return;
            }
           // TradeStationLog.WriteLogPerformance(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + "Type: getOrderBookInfo,bingOrderData, BIND TO UI BEGIN,ACC " + _Acc + " "); 

            if (GOSTradeStation.isDealer)
            {
                if (this.CurrentID == null || this.CurrentID.Trim() == "" || _Acc == null || _Acc.Trim() == "" || this.CurrentID != _Acc.Trim())
                {
                    return;
                }
            }
             try
            {

                if (dtOrderBook != null)
                {
                    SetBOrSColumnInfo(dtOrderBook, "osBQty", "osSQty", "bors");
                    DataTable dtClone = dtOrderBook.Clone();
                    if (dtClone.Columns.Contains("price"))
                    {
                        dtClone.Columns["price"].DataType = typeof(decimal);
                    }
                    if (dtClone.Columns.Contains("internalOrderNo"))
                    {
                        dtClone.Columns["internalOrderNo"].DataType = typeof(int);
                    }
                    if (dtClone.Columns.Contains("osBQty"))
                    {
                        dtClone.Columns["osBQty"].DataType = typeof(int);
                    }
                    if (dtClone.Columns.Contains("osSQty"))
                    {
                        dtClone.Columns["osSQty"].DataType = typeof(int);
                    }
                    dtClone.Columns.Add(new DataColumn("basePrice"));
                    dtClone.Columns.Add(new DataColumn("strNFPrice"));

                    foreach (DataRow dr in dtOrderBook.Rows)
                    {
                        DataRow drNew = dtClone.NewRow();
                        foreach (DataColumn col in dtOrderBook.Columns)
                        {
                            if (col.ColumnName.ToLower().Trim() == "price")
                            {
                                string proCode = dr.getColValue("productCode");
                                drNew["basePrice"] = dr.getColValue(col.ColumnName);
                                decimal dPrice = GosBzTool.getDecimalPrice(proCode, dr.getColValue(col.ColumnName));
                                drNew[col.ColumnName] = dPrice;  
                                drNew["strNFPrice"] = GosBzTool.adjustDecLengthToString(proCode, dPrice);
                                continue;
                            }
                            if (col.ColumnName.ToLower().Trim() == "osbqty")
                            {
                                drNew[col.ColumnName] = dr.getColIntValue(col.ColumnName, 0);
                                continue;
                            }

                            if (col.ColumnName.ToLower().Trim() == "ossqty")
                            {
                                drNew[col.ColumnName] = dr.getColIntValue(col.ColumnName, 0);
                                continue;
                            }
                            //if (col.ColumnName.ToLower().Trim() == "status")
                            //{
                            //    string strStatus = dr.getColValue("status");
                            //    strStatus=System.Text.RegularExpressions.Regex.Replace(strStatus,"^\d+","");
                            //   // TradeStationTools.Base64StringToString
                            //}

                            if (col.ColumnName.ToLower().Trim() == "valid")
                            {
                                string strValid= dr.getColValue(col.ColumnName);
                                if (strValid != "")
                                {
                                    if (strValid.Length % 4 == 0)
                                    {
                                        try
                                        {
                                            string strValid1 = TradeStationTools.Base64StringToString(strValid);
                                            drNew[col.ColumnName] = strValid1;
                                        }
                                        catch {
                                            drNew[col.ColumnName] = strValid;
                                            continue; }
                                    }
                                }
                                continue;
                            }
                            drNew[col.ColumnName] = dr[col.ColumnName];
                        }
                        dtClone.Rows.Add(drNew);
                    }

                    Application.Current.Dispatcher.Invoke((Action)delegate()
                    {
                        this.dg_OrderBook.ItemsSource = dtClone.DefaultView;                       
                    },
                     System.Windows.Threading.DispatcherPriority.Send,
                    null);

                }
                else
                {
                    Application.Current.Dispatcher.Invoke((Action)delegate()
                    {
                      
                        this.dg_OrderBook.ItemsSource = null;                       
                    },
                     System.Windows.Threading.DispatcherPriority.Send,
                    null);
                }
            }
             
            catch { }

        }




        public void bingClearTradeData(DataTable dtClearTrade, string _userID, string _Acc, DateTime? _dt)
        {
            if (dtClearTrade == null)
            {
                return;
            }
            if (_userID == null || _userID.Trim() == "" || GOSTradeStation.UserID == "" || _userID.Trim() != GOSTradeStation.UserID)
            {
                return;
            }
            if (GOSTradeStation.isDealer)
            {
                if (this.CurrentID == "") return;
                if (_Acc == null || _Acc.Trim() == "" || _Acc != this.CurrentID)
                {
                    return;
                }
            }
            if (dtClearTrade != null)
            {
                SetBOrSColumnInfo(dtClearTrade, "BQty", "SQty", "bors");

                DataTable dtClearClone = dtClearTrade.Clone();
                if (dtClearClone.Columns.Contains("tradePrice"))
                {
                    dtClearClone.Columns["tradePrice"].DataType = typeof(decimal);
                }

                dtClearClone.Columns.Add(new DataColumn("basePrice"));
                dtClearClone.Columns.Add(new DataColumn("strNFPrice"));

                foreach (DataRow dr in dtClearTrade.Rows)
                {
                    DataRow drNew = dtClearClone.NewRow();
                    foreach (DataColumn col in dtClearTrade.Columns)
                    {
                        if (col.ColumnName.ToLower().Trim() == "tradeprice")
                        {
                            string proCode = dr.getColValue("productCode");
                            decimal dPrice = GosBzTool.getDecimalPrice(proCode, dr.getColValue(col.ColumnName));
                            drNew[col.ColumnName] = dPrice;
                            drNew["basePrice"] = dr.getColValue(col.ColumnName);
                            drNew["strNFPrice"] = GosBzTool.adjustDecLengthToString(proCode, dPrice);
                            continue;
                        }

                        if (col.ColumnName.ToLower().Trim() == "orderprice")
                        {
                            string proCode = dr.getColValue("productCode");
                            decimal dPrice = GosBzTool.getDecimalPrice(proCode, dr.getColValue(col.ColumnName));
                            drNew[col.ColumnName] = GosBzTool.adjustDecLengthToString(proCode, dPrice);
                            continue;
                        }

                        drNew[col.ColumnName] = dr[col.ColumnName];
                    }
                    dtClearClone.Rows.Add(drNew);
                }

              
                Dispatcher.Invoke((Action)(() =>
                {
                    this.dg_ClearTrade.ItemsSource = dtClearClone.DefaultView;
                    this.dg_tabOrder_ClearTrade.ItemsSource = dtClearClone.DefaultView;              
                }),
                System.Windows.Threading.DispatcherPriority.Send,
                 null);
            }
            else
            {
                Dispatcher.Invoke((Action)(() =>
                {                   
                    this.dg_ClearTrade.ItemsSource = null;
                    this.dg_tabOrder_ClearTrade.ItemsSource = null;
                   // TradeStationLog.WriteLogPerformance(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + "ClearTrade bind  exec on dispatch at UOI,null Data,ACC " + _Acc + " ");

                }),
                 System.Windows.Threading.DispatcherPriority.Send,
              null);
            }
        }    

        /// <summary>
        /// 转换科学计算法的Ｅ表示的字串为数字

        /// </summary>
        /// <param name="Estr"></param>
        /// <returns></returns>
        int ConvertScientEToInt(string Estr)
        {
            int i;
            try
            {
                i = Convert.ToInt32(Estr);
                return i;
            }
            catch { }

            if (Estr == null) return 0;
            if (Estr.Trim() == "") return 0;
            Estr = Estr.ToLower();
            int pos = Estr.IndexOf("e");
            if (pos == -1)
            {
                return 0;
            }
            string temp = "";
            if (pos + 1 == Estr.Length)
            {
                temp = Estr.Substring(0, pos);
                try
                {
                    i = Convert.ToInt32(temp);
                    return i;
                }
                catch { return 0; }
            }
            try
            {
                string strNum = Estr.Substring(0, pos);
                string Len = Estr.Substring(pos + 1);
                double dbNum = Convert.ToDouble(strNum);
                int IntLen = Convert.ToInt32(Len);
                double Result = dbNum * Math.Pow(10, IntLen);
                return (int)Result;
            }
            catch { }
            return 0;
        }

        #region header button
        void showHideOrderOperationButton()
        {
            if (tabControl1 == null) return;
            if (tabControl1.SelectedIndex == 0)//dg_OrderBook.Items.Count>0 && 
            {
                gdOrderBtnContainer.Visibility = Visibility.Visible;
                return;

            }
            gdOrderBtnContainer.Visibility = Visibility.Collapsed;
        }

        void showHidePositionButton()
        {
            if (tabControl1 == null) return;
            if (tabControl1.SelectedIndex == 1)//&& dgPosition.Items.Count > 0)
            {
                gdPositionBtnContainer.Visibility = Visibility.Visible;
                return;
            }
            gdPositionBtnContainer.Visibility = Visibility.Collapsed;
        }
        #endregion

        #region tab Position
        ObservableCollection<OrderPosition> posCollection = new ObservableCollection<OrderPosition>();      

        public void OpenClosePosition(OrderPosition PositionData)
        {
            MdiContainer Container = App.Current.MainWindow.FindName("Container") as MdiContainer;
            MdiChild ClosePositionChild = new MdiChild();
            bool bFirstInit = false;
            ClosePosition ClosePosition = ClosePosition.GetClosePosition(distributeMsg, PositionData,ClosePositionChild, this.CurrentID, ref bFirstInit);
            if (ClosePosition == null) return;
            if (bFirstInit)
            {
               // ClosePositionChild.Title = "Close Position - " + PositionData.OrderID;ClosePositionChild
                //if (this.mdiChild != null)
                //{
                //    SetMdITitle(distributeMsg, ClosePositionChild, "Close Position - " + PositionData.OrderID);
                //}
                ClosePositionChild.Content = ClosePosition;
                ClosePositionChild.Width = 320;
                ClosePositionChild.Height = 200;
                ClosePositionChild.Position = new System.Windows.Point(0, 0);
                if (!Container.Children.Contains(ClosePositionChild))
                {
                    Container.Children.Add(ClosePositionChild);
                }
            }
            else
            {              
                Container.ActiveMdiChild = ClosePosition.mdiClosePosition;
            }
        }
        private void btn_ClosePosition_Click(object sender, RoutedEventArgs e)
        {
            if (dgPosition.SelectedIndex == -1) return;
            var item = this.dgPosition.SelectedItem;
            try
            {
                var b = item as OrderPosition;
                if (b != null)
                {
                    if (b.bCanClose)
                    {
                        OpenClosePosition(b);
                    }
                }
            }
            catch { }
        }

        private void dgPosition_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            if (dgPosition.CurrentItem != null && dgPosition.SelectedItems.Count > 0 && e.AddedItems.Count > 0)
            {
                if (dgPosition.SelectedIndex == -1) return;
                var item = this.dgPosition.SelectedItem;
                try
                {
                    var b = item as OrderPosition;
                    if (b != null)
                    {
                        if (b.bCanClose)
                        {
                            this.btnPosClose.IsEnabled = true;
                        }
                        else
                        {
                            this.btnPosClose.IsEnabled = false;
                        }
                    }
                }
                catch { }
            }
        }

        #endregion

        #region Tab Order

        //protected void distributeMsg_DeleteOrder(object sender, String str)
        //{
        //    Application.Current.Dispatcher.Invoke(DisOrderConfirmDelegate, new object[] { str });
        //}

        //protected void distributeMsg_ActivateOrder(object sender, String str)
        //{
        //    Application.Current.Dispatcher.Invoke(DisOrderConfirmDelegate, new object[] { str });
        //}

        //protected void distributeMsg_InactivateOrder(object sender, String str)
        //{
        //    Application.Current.Dispatcher.Invoke(DisOrderConfirmDelegate, new object[] { str });
        //}
        DataRowView drvSelected;
        private void dg_OrderBook_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (dg_OrderBook.CurrentItem != null && dg_OrderBook.SelectedItems.Count > 0 && e.AddedItems.Count > 0)
            {
                enableOrderButton(true);              
                try
                {
                    DataRowView dt = e.AddedItems[0] as DataRowView;
                    drvSelected = dt.CopyDrv();
                    OrderSelectIndex = dt["internalOrderNo"].ToString();
                    OrderProductCode = dt["productCode"].ToString();
                    uint osBQty = Convert.ToUInt32(dt["osBQty"]);
                    uint osSQty = Convert.ToUInt32(dt["osSQty"]);
                    if (osBQty > 0)
                    {
                        OrderCount = osBQty;
                        strBuySell = "Buy";
                    }
                    else
                    {
                        OrderCount = osSQty;
                        strBuySell = "Sell";
                    }
                    OrderPrice = Convert.ToInt32(dt["basePrice"]);
                    strRealOrderPrice = dt.Row.getColValue("strNFPrice");
                    if (strBuySell == "Buy")
                    {
                        buySell = TradeStationComm.Attribute.BuySell.buy;
                    }
                    else
                    {
                        buySell = TradeStationComm.Attribute.BuySell.sell;
                    }
                }
                catch {
                    OrderSelectIndex = "";
                    OrderProductCode = "";
                }
            }
            else
            {
                enableOrderButton(false);
            }
        }


        private void btn_Delete_Click(object sender, RoutedEventArgs e)
        {
            if (dg_OrderBook.SelectedIndex == -1)
            {               
                MessageBox.Show(strRs_UOI_Pls_selectRow);
                return;
            }
            try
            {
                if (OrderSelectIndex != "" && OrderProductCode != "")
                {
                    strMessage = string.Format(CommonRsText.strRs_Confirm_Order_Del,// "Delete Order Confirm\n\nAccount:[{0}]\nOrder#:{1}\nCmd:{2}\nId:{3}\nPrice:{4}\nQty:{5}", 
                        this.CurrentID,OrderSelectIndex,
                         CommonRsText.GetBSText(strBuySell), 
                        OrderProductCode, 
                        strRealOrderPrice , 
                        OrderCount);
                    if (MessageBox.Show(strMessage, 
                        CommonRsText.strRs_Confirm_OrderDel_Title,// "Delete Order Confirm ", 
                        MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    {
                        TradeStationLog.WriteLogPerformance(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + " Delete Order:" + strMessage);
                    
                        if (GOSTradeStation.isDealer)
                        {
                            GOSTradeStation.msgChannel.Send(TradeStationComm.MsgRequest.getMsgDeleteOrder(GOSTradeStation.UserID, GOSTradeStation.Pwd, this.CurrentID,
                                                           new TradeStationComm.MsgRequest.OrderClass.DeleteOrderClass(OrderSelectIndex, OrderProductCode, TradeStationComm.Attribute.AE.AE, this.CurrentID)));
                            TradeStationSend.Send(cmdClient.getOrderBookInfo,this.CurrentID);
                            TradeStationSend.Send(cmdClient.getAccountInfo,  this.CurrentID);
                            //kenlo20150312 TradeStationSend.Send(cmdClient.getPositionInfo, this.CurrentID);
                        }
                        else
                        {
                            GOSTradeStation.msgChannel.Send(TradeStationComm.MsgRequest.getMsgDeleteOrder(GOSTradeStation.UserID, GOSTradeStation.Pwd,
                                                            new TradeStationComm.MsgRequest.OrderClass.DeleteOrderClass(OrderSelectIndex, OrderProductCode, TradeStationComm.Attribute.AE.normalUser, "")));
                            TradeStationSend.Send(cmdClient.getOrderBookInfo);
                            TradeStationSend.Send(cmdClient.getAccountInfo);
                            //kenlo20150312 TradeStationSend.Send(cmdClient.getPositionInfo);
                        }
                    }                    
                }
                else
                {
                    MessageBox.Show("error product code");
                }
            }
            catch (Exception) { }
        }

        private void btn_Activate_Click(object sender, RoutedEventArgs e)
        {
            if (dg_OrderBook.SelectedIndex == -1)
            {
                MessageBox.Show(this.strRs_UOI_Pls_selectRow );// "please select row first");
                return;
            }
            try
            {
                strMessage = string.Format(CommonRsText.strRs_Confirm_Order_Active,// "Activate Order Confirm\n\nAccount:[{0}]\nOrder#:{1}\nCmd:{2}\nId:{3}\nPrice:{4}\nQty:{5}",
                    this.CurrentID, 
                    OrderSelectIndex,
                    CommonRsText.GetBSText(strBuySell),
                    OrderProductCode,
                    strRealOrderPrice, 
                    OrderCount);

                if (OrderSelectIndex != "" && OrderProductCode != "")
                {
                    if (MessageBox.Show(strMessage,
                        CommonRsText.strRs_Confirm_OrderActive_title,// "Activate Order Confirm ", 
                        MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    {
                        TradeStationLog.WriteLogPerformance(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + " Active Order:" + strMessage);
                    
                        if (GOSTradeStation.isDealer)
                        {
                            GOSTradeStation.msgChannel.Send(TradeStationComm.MsgRequest.getMsgActivateOrder(GOSTradeStation.UserID, GOSTradeStation.Pwd, this.CurrentID,
                                                            new TradeStationComm.MsgRequest.OrderClass.ActivateOrderClass(OrderSelectIndex, OrderProductCode, buySell, OrderCount, OrderPrice, TradeStationComm.Attribute.AE.AE, this.CurrentID)));
                            TradeStationSend.Send(cmdClient.getOrderBookInfo, this.CurrentID);
                            TradeStationSend.Send(cmdClient.getAccountInfo, this.CurrentID);
                            //kenlo20150312 TradeStationSend.Send(cmdClient.getPositionInfo, this.CurrentID);
                        }
                        else
                        {
                            GOSTradeStation.msgChannel.Send(TradeStationComm.MsgRequest.getMsgActivateOrder(GOSTradeStation.UserID, GOSTradeStation.Pwd, GOSTradeStation.AccountID,
                                                            new TradeStationComm.MsgRequest.OrderClass.ActivateOrderClass(OrderSelectIndex, OrderProductCode, buySell, OrderCount, OrderPrice, TradeStationComm.Attribute.AE.normalUser, "")));
                            TradeStationSend.Send(cmdClient.getOrderBookInfo);
                            TradeStationSend.Send(cmdClient.getAccountInfo);
                            //kenlo20150312 TradeStationSend.Send(cmdClient.getPositionInfo);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("error");
                }
            }
            catch (Exception) { }
        }
       
        private void btn_Inactivate_Click(object sender, RoutedEventArgs e)
        {
            if (dg_OrderBook.SelectedIndex == -1)
            {
                MessageBox.Show(this.strRs_UOI_Pls_selectRow );// "please select row first");
                return;
            }
            try
            {
                strMessage = string.Format(CommonRsText.strRs_Confirm_Order_Inactive,// "Inactivate Order Confirm\n\nAccount:[{0}]\nOrder#:{1}\nCmd:{2}\nId:{3}\nPrice:{4}\nQty:{5}",
                    this.CurrentID, 
                    OrderSelectIndex, 
                    CommonRsText.GetBSText(strBuySell), 
                    OrderProductCode, 
                    strRealOrderPrice , 
                    OrderCount);

                if (OrderSelectIndex != "" && OrderProductCode != "")
                {
                    if (MessageBox.Show(strMessage,
                        CommonRsText.strRs_Confirm_OrderInActive_title ,// "Inactivate Order Confirm ", 
                        MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    {
                        TradeStationLog.WriteLogPerformance(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + " Inactivate Order:" + strMessage);
                    
                        if (GOSTradeStation.isDealer)
                        {
                            GOSTradeStation.msgChannel.Send(TradeStationComm.MsgRequest.getMsgInactivateOrder(GOSTradeStation.UserID, GOSTradeStation.Pwd, this.CurrentID,
                                                               new TradeStationComm.MsgRequest.OrderClass.ActivateOrderClass(OrderSelectIndex, OrderProductCode, buySell, OrderCount, OrderPrice, TradeStationComm.Attribute.AE.AE, this.CurrentID)));
                            TradeStationSend.Send(cmdClient.getOrderBookInfo, this.CurrentID);
                            TradeStationSend.Send(cmdClient.getAccountInfo, this.CurrentID);
                            //kenlo20150312 TradeStationSend.Send(cmdClient.getPositionInfo, this.CurrentID);
                        }
                        else
                        {
                            GOSTradeStation.msgChannel.Send(TradeStationComm.MsgRequest.getMsgInactivateOrder(GOSTradeStation.UserID, GOSTradeStation.Pwd, GOSTradeStation.AccountID,
                                                            new TradeStationComm.MsgRequest.OrderClass.ActivateOrderClass(OrderSelectIndex, OrderProductCode, buySell, OrderCount, OrderPrice, TradeStationComm.Attribute.AE.normalUser, "")));
                            TradeStationSend.Send(cmdClient.getOrderBookInfo);
                            TradeStationSend.Send(cmdClient.getAccountInfo);
                            //kenlo20150312 TradeStationSend.Send(cmdClient.getPositionInfo);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("error");
                }
            }
            catch (Exception) { }
        }

        public static object objLockBookOrder = new object();
        bool bInOrderBookDeleting = false;
        private void btn_DeleteAll_Click(object sender, RoutedEventArgs e)
        {
            if (bInOrderBookDeleting)
            {
                return;
            }

            //try
            //{
                if (dg_OrderBook != null)
                {
                    if (dg_OrderBook.Items == null)
                    {
                        return;
                    }
                    if (dg_OrderBook.Items.Count < 1)
                    {
                        return;
                    }

                    string _accConf = "";
                    if (this.CurrentID != null)
                    {
                        _accConf = this.CurrentID;
                    }
                    string _msgConf = CommonRsText.strRs_Confirm_Sure_DelAll;
                    if (_accConf != "")
                    {
                        _msgConf = _msgConf + "\n" + GOSTS.CommonRsText.strRs_AccText + ":" + _accConf;
                    }

                    if (MessageBox.Show(_msgConf,
                        CommonRsText.strRs_Confirm_OrderDel_Title ,
                        MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    {
                        try
                        {
                            bInOrderBookDeleting = true;
                            // Thread.Sleep(5000);
                            #region
                            DataTable dt = dg_OrderBook.DataContext as DataTable;
                            if (dt == null)
                            {
                                var item = dg_OrderBook.Items[0] as DataRowView;
                                if (item == null) { bInOrderBookDeleting = false; return; }
                                dt = item.Row.Table;
                            }
                            #endregion
                            if (dt == null) { bInOrderBookDeleting = false; return; }
                            if (dt.Rows.Count < 1) { bInOrderBookDeleting = false; return; }

                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                TradeStationLog.WriteLogPerformance(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + " Delete Order:order No:" + dt.Rows[i].getColValue("internalOrderNo") + ",prodCode:" + dt.Rows[i].getColValue("productCode"));                      
                                if (GOSTradeStation.isDealer)
                                {                                    
                                    GOSTradeStation.msgChannel.Send(TradeStationComm.MsgRequest.getMsgDeleteOrder(GOSTradeStation.UserID, GOSTradeStation.Pwd, this.CurrentID,
                                                                        new TradeStationComm.MsgRequest.OrderClass.DeleteOrderClass(dt.Rows[i]["internalOrderNo"].ToString(), dt.Rows[i]["productCode"].ToString(), TradeStationComm.Attribute.AE.AE, this.CurrentID)));

                                }
                                else
                                {
                                    GOSTradeStation.msgChannel.Send(TradeStationComm.MsgRequest.getMsgDeleteOrder(GOSTradeStation.UserID, GOSTradeStation.Pwd, GOSTradeStation.AccountID,
                                                                    new TradeStationComm.MsgRequest.OrderClass.DeleteOrderClass(dt.Rows[i]["internalOrderNo"].ToString(), dt.Rows[i]["productCode"].ToString(), TradeStationComm.Attribute.AE.normalUser, "")));
                                }
                            }
                            bInOrderBookDeleting = false;
                        }
                        catch { }
                        finally {
                            bInOrderBookDeleting = false;
                        }
                        
                        if (GOSTradeStation.isDealer)
                        {
                            TradeStationSend.Send(cmdClient.getOrderBookInfo, this.CurrentID);
                            TradeStationSend.Send(cmdClient.getAccountInfo, this.CurrentID);
                            //kenlo20150312 TradeStationSend.Send(cmdClient.getPositionInfo, this.CurrentID);
                        }
                        else
                        {
                            TradeStationSend.Send(cmdClient.getOrderBookInfo);
                            TradeStationSend.Send(cmdClient.getAccountInfo);
                            //kenlo20150312 TradeStationSend.Send(cmdClient.getPositionInfo);
                        }

                    }
                }
                else
                {
                    MessageBox.Show("Order is null.");
                }

            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.ToString());
            //}

        }


        private void dg_OrderBook_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (this.dg_OrderBook.SelectedItem != null)
            {
                try
                {
                    DataRowView drv = this.dg_OrderBook.SelectedItem as DataRowView;
                    if (drv == null) return;

                    OrderSelectIndex = drv.Row.getColValue("internalOrderNo");
                    OrderProductCode = drv.Row.getColValue("productCode");
                    if (OrderSelectIndex == "")
                    {
                        MessageBox.Show("order no lost");
                        return;
                    }
                    if (OrderProductCode == "")
                    {
                        MessageBox.Show("order has not product code");
                        return;
                    }

                    uint osBQty = drv.Row.getColUIntValue("osBQty", 0);
                    uint osSQty = drv.Row.getColUIntValue("osSQty", 0); // Convert.ToUInt32(drv["osSQty"]);
                    string tone = Utility.getColumnValue(drv.Row, "tOne");
                    bool bInitMdi = false;
                    if (osBQty == 0 && osSQty == 0)
                    {
                        MessageBox.Show("order has not quantity");
                        return;
                    }

                    string cond = drv.Row.getColValue("cond");
                    TradeStationComm.Attribute.StopType stopType = TradeStationComm.Attribute.StopType.normalOrder;
                    stopType = GosBzTool.getStopTypeByOrderBookStatus(cond);
                    bool bTriggerPrice = false;
                    if(stopType != TradeStationComm.Attribute.StopType.normalOrder)
                    {
                        bTriggerPrice = true;
                    }

                    //if (!GosBzTool.CanChangeOrder(cond))
                    //{
                    //    MessageBox.Show("sorry,cond '" + cond + "' can't change now");
                    //}
                    //else
                    {
                        if (osBQty > 0)
                        {
                            MdiContainer Container = App.Current.MainWindow.FindName("Container") as MdiContainer;
                            MdiChild ChangeOrderChild = new MdiChild();

                            ChangeOrder changeOrder = ChangeOrder.getChangeOrder(distributeMsg, drv, "buy", osBQty, ChangeOrderChild, this.CurrentID, tone, ref bInitMdi, 
                                                                                 this.dg_OrderBook, OrderSelectIndex); // MatthewSin
                            if (changeOrder == null)
                            {
                                return;
                            }
                            if (bInitMdi)
                            {
                                ChangeOrderChild.Width = GosBzTool.ChOrderWidth;// 408;
                                ChangeOrderChild.Height = GosBzTool.ChOrderHeight;// 195;
                                
                                ChangeOrderChild.Position = new System.Windows.Point(50, 55);
                                ChangeOrderChild.Content = changeOrder;
                                if (!Container.Children.Contains(ChangeOrderChild))
                                {
                                    Container.Children.Add(ChangeOrderChild);
                                }
                            }                                  
                            e.Handled = true;
                            changeOrder.ChangeOrderChild.Focus();
                        }
                        else
                        {
                            MdiContainer Container = App.Current.MainWindow.FindName("Container") as MdiContainer;

                            MdiChild ChangeOrderChild = new MdiChild();
                            ChangeOrder ChangeOrderSell = ChangeOrder.getChangeOrder(distributeMsg, drv, "sell", osSQty, ChangeOrderChild, this.CurrentID, tone, ref bInitMdi, 
                                                                                     this.dg_OrderBook, OrderSelectIndex); // MatthewSin
                            //  ChangeOrderChild.Title = "ChangeOrder";
                            if (bInitMdi)
                            {
                                ChangeOrderChild.Width = GosBzTool.ChOrderWidth; ; // 408;
                                ChangeOrderChild.Height = GosBzTool.ChOrderHeight; // 195;
                                ChangeOrderChild.Position = new System.Windows.Point(50, 55);
                                ChangeOrderChild.Content = ChangeOrderSell;

                                if (!Container.Children.Contains(ChangeOrderChild))
                                {
                                    Container.Children.Add(ChangeOrderChild);
                                }
                            }                         
                          //  ChangeOrderChild.Height = bTriggerPrice ? 250 : 220; // 408;
                            e.Handled = true;
                            ChangeOrderSell.ChangeOrderChild.Focus();
                        }
                    }
                }
                catch(Exception ex) {
                    TradeStationLog.WriteError(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "  UserOrderInfo.dg_OrderBook_MouseDoubleClick(),error: " + ex.ToString());

                }


            }


        }


        bool bInOrderBookActiving = false;
        private void btn_ActivateAll_Click(object sender, RoutedEventArgs e)
        {
            if (bInOrderBookActiving)
            {
                return;
            }
            //try
            //{
                if (dg_OrderBook != null)
                {
                    if (dg_OrderBook.Items == null)
                    {
                        return;
                    } 
                   
                    if (dg_OrderBook.Items.Count < 1)
                    {
                        return;
                    }
                   //2014-10-28
                    string _accConf ="";
                    if (this.CurrentID != null)
                    {
                        _accConf = this.CurrentID;
                    }                                      
                    string _msgConf =CommonRsText.strRs_Confirm_sure_ActiveAllOrder;
                    if(_accConf !="")
                    {
                        _msgConf = _msgConf + "\n" + GOSTS.CommonRsText.strRs_AccText + ":" + _accConf;
                    }

                    if (MessageBox.Show(_msgConf,
                        CommonRsText.strRs_Confirm_OrderActive_title ,
                        MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    {
                        bInOrderBookActiving = true;
                        try
                        {
                            foreach (var item in dg_OrderBook.Items)
                            {
                                DataRowView dataview = item as DataRowView;
                                if (dataview == null) continue;
                                DataRow dr = dataview.Row;

                                uint osBQty = Convert.ToUInt32(dr.getColValue("osBQty"));
                                uint osSQty = Convert.ToUInt32(dr.getColValue("osSQty"));
                                OrderSelectIndex = dr["internalOrderNo"].ToString();
                                OrderProductCode = dr["productCode"].ToString();
                                OrderPrice = Convert.ToInt32(dr["basePrice"]);//price

                                if (osBQty > 0)
                                {
                                    OrderCount = osBQty;
                                    strBuySell = "Buy";
                                }
                                else
                                {
                                    OrderCount = osSQty;
                                    strBuySell = "Sell";
                                }
                                if (strBuySell == "Buy")
                                {
                                    buySell = TradeStationComm.Attribute.BuySell.buy;
                                }
                                else
                                {
                                    buySell = TradeStationComm.Attribute.BuySell.sell;
                                }
                                TradeStationLog.WriteLogPerformance(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + " Active Order:order No:" + OrderSelectIndex + ",prodCode:" + OrderProductCode);
                                if (GOSTradeStation.isDealer)
                                {
                                    GOSTradeStation.msgChannel.Send(TradeStationComm.MsgRequest.getMsgActivateOrder(GOSTradeStation.UserID, GOSTradeStation.Pwd, this.CurrentID,
                                                               new TradeStationComm.MsgRequest.OrderClass.ActivateOrderClass(OrderSelectIndex, OrderProductCode, buySell, OrderCount, OrderPrice, TradeStationComm.Attribute.AE.AE, this.CurrentID)));
                                }
                                else
                                {
                                    GOSTradeStation.msgChannel.Send(TradeStationComm.MsgRequest.getMsgActivateOrder(GOSTradeStation.UserID, GOSTradeStation.Pwd, GOSTradeStation.AccountID,
                                                                new TradeStationComm.MsgRequest.OrderClass.ActivateOrderClass(OrderSelectIndex, OrderProductCode, buySell, OrderCount, OrderPrice, TradeStationComm.Attribute.AE.normalUser, "")));
                                }
                            }
                        }
                        catch { }
                        finally { bInOrderBookActiving = false; }

                       // if (dg_OrderBook.Items.Count > 0)
                        {
                            if (GOSTradeStation.isDealer)
                            {
                                TradeStationSend.Send(cmdClient.getOrderBookInfo, this.CurrentID);
                                TradeStationSend.Send(cmdClient.getAccountInfo, this.CurrentID);
                                //kenlo20150312 TradeStationSend.Send(cmdClient.getPositionInfo, this.CurrentID);
                            }
                            else
                            {
                                TradeStationSend.Send(cmdClient.getOrderBookInfo);
                                TradeStationSend.Send(cmdClient.getAccountInfo);
                                //kenlo20150312 TradeStationSend.Send(cmdClient.getPositionInfo);
                            }
                        }
                    }
                }

            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.ToString());
            //}
        }

        private void btn_InactivateAll_Click(object sender, RoutedEventArgs e)
        {
            if (bInOrderBookActiving) return;
            //try
            //{
                if (dg_OrderBook != null)
                {
                    string _accConf = "";
                    if (this.CurrentID != null)
                    {
                        _accConf = this.CurrentID;
                    }
                    string _msgConf = CommonRsText.strRs_Confirm_sure_InActiveAllOrder;
                    if (_accConf != "")
                    {
                        _msgConf = _msgConf + "\n" + GOSTS.CommonRsText.strRs_AccText + ":" + _accConf;
                    }

                    if (MessageBox.Show(_msgConf,
                        CommonRsText.strRs_Confirm_OrderInActive_title,
                        MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    {
                        bInOrderBookActiving = true;
                        try
                        {
                            foreach (var item in dg_OrderBook.Items)
                            {
                                DataRowView dataview = item as DataRowView;
                                if (dataview == null) continue;
                                DataRow dr = dataview.Row;

                                uint osBQty = Convert.ToUInt32(dr["osBQty"]);
                                uint osSQty = Convert.ToUInt32(dr["osSQty"]);
                                OrderSelectIndex = dr["internalOrderNo"].ToString();
                                OrderProductCode = dr["productCode"].ToString();
                                OrderPrice = Convert.ToInt32(dr["basePrice"]);//price

                                if (osBQty > 0)
                                {
                                    OrderCount = osBQty;
                                    strBuySell = "Buy";
                                }
                                else
                                {
                                    OrderCount = osSQty;
                                    strBuySell = "Sell";
                                }
                                if (strBuySell == "Buy")
                                {
                                    buySell = TradeStationComm.Attribute.BuySell.buy;
                                }
                                else
                                {
                                    buySell = TradeStationComm.Attribute.BuySell.sell;
                                }
                                TradeStationLog.WriteLogPerformance(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + " Inactive Order:order No:" + OrderSelectIndex + ",prodCode:" + OrderProductCode);
                                
                                if (GOSTradeStation.isDealer)
                                {                                   
                                    GOSTradeStation.msgChannel.Send(TradeStationComm.MsgRequest.getMsgInactivateOrder(GOSTradeStation.UserID, GOSTradeStation.Pwd, this.CurrentID,
                                                                               new TradeStationComm.MsgRequest.OrderClass.ActivateOrderClass(OrderSelectIndex, OrderProductCode, buySell, OrderCount, OrderPrice, TradeStationComm.Attribute.AE.AE, this.CurrentID)));

                                }
                                else
                                {
                                    GOSTradeStation.msgChannel.Send(TradeStationComm.MsgRequest.getMsgInactivateOrder(GOSTradeStation.UserID, GOSTradeStation.Pwd, GOSTradeStation.AccountID,
                                                                            new TradeStationComm.MsgRequest.OrderClass.ActivateOrderClass(OrderSelectIndex, OrderProductCode, buySell, OrderCount, OrderPrice, TradeStationComm.Attribute.AE.normalUser, "")));
                                }
                            }
                        }
                        catch { }
                        finally
                        {
                            bInOrderBookActiving = false;
                        }
                        //if (dg_OrderBook.Items.Count > 0)
                        try
                        {
                            if (GOSTradeStation.isDealer)
                            {
                                TradeStationSend.Send(cmdClient.getOrderBookInfo, this.CurrentID);
                                TradeStationSend.Send(cmdClient.getAccountInfo, this.CurrentID);
                                //kenlo20150312 TradeStationSend.Send(cmdClient.getPositionInfo, this.CurrentID);
                            }
                            else
                            {
                                TradeStationSend.Send(cmdClient.getOrderBookInfo);
                                TradeStationSend.Send(cmdClient.getAccountInfo);
                                //kenlo20150312 TradeStationSend.Send(cmdClient.getPositionInfo);
                            }
                        }
                        catch { }
                    }
                }

            //}
            //catch (Exception) { }
        }

        void enableOrderButton(bool b)
        {
            btn_Delete.IsEnabled = b;
            btn_Activate.IsEnabled = b;
            btn_Inactivate.IsEnabled = b;
            btn_DeleteAll.IsEnabled = b;
            btn_ActivateAll.IsEnabled = b;
            btn_InactivateAll.IsEnabled = b;
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

        #endregion

        public override void initCtrlData()
        {
            base.initCtrlData();
            if (PosBus != null)
            {
                ObservableCollection<OrderPosition> posCL = PosBus.getPosCL();
                PositionBusDataChange(posCL, PosChangeType.PosChange, PosBus.AccID);
            }
        }

        public override void IDelearUser_OnUserChange(string _CurUserID)
        {           
            //重新发起请求数据
            if(GOSTradeStation.IsWindowInitialized)
            if (!GOSTradeStation.isDealer)
            {
                TradeStationSend.Send(cmdClient.getOrderBookInfo);              
                //kenlo20150312 TradeStationSend.Send(cmdClient.getDoneTradeInfo);              
               TradeStationSend.Send(cmdClient.getClearTradeInfo);
            }
            else
            {
                TradeStationSend.Send(cmdClient.getOrderBookInfo, _CurUserID);
                //kenlo20150312 TradeStationSend.Send(cmdClient.getDoneTradeInfo, _CurUserID);            
              TradeStationSend.Send(cmdClient.getClearTradeInfo, _CurUserID);
            }
        }

        private void ToggleButton_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void IDelearUser_Unloaded_1(object sender, RoutedEventArgs e)
        {
            if (distributeMsg != null)
            {
                distributeMsg.DisUserOrderInfo -= new MessageDistribute.onDisUserOrderInfo(distributeMsg_UserOrderInfoItem);
                distributeMsg.DisClearTradeInfo -= new MessageDistribute.onDisClearTradeInfo(distributeMsg_ClearTradeInfoItem);
                distributeMsg.DisDoneTradeInfo -= new MessageDistribute.onDisDoneTradeInfo(distributeMsg_DisDoneTradeInfo);
              //  distributeMsg.DisAccountMaster -= DispatcherInvokeGetAccountMaster; 
            }
           
            #region  95-29 cancel mk-pos
            //distributeMsg.DisMPControlOrder -= new MessageDistribute.OnDisMPControlOrder(distributeMsg_DisMarketPrice);
            //distributeMsg.DisPositionInfo -= new MessageDistribute.onDisPositionInfo(distributeMsg_DisPositionInfo);
            if (PosBus != null)
            {
                PosBus.UnRegisterOnPosInfoChange(new PositionBus.PosInfoChange(PosBus_DataChangeInvoke));
            }
            #endregion
            base.IDelearUser_Unloaded_1(sender, e);
        }

        private void ProcessRowBG(DataGrid dg, SolidColorBrush brush)
        {
            if (dg.Items.Count <= 1) return;

            for (int i = 0; i < dg.Items.Count; i++)
            {
                if (i % 2 == 1)
                {

                    DataGridRow row = dg.ItemContainerGenerator.ContainerFromItem(dg.Items[i]) as DataGridRow;
                    if (row == null)
                    {
                        //  row = dg.Items[i] as DataRow;
                    }

                }
            }
        }

        void SetBOrSColumnInfo(DataTable dt, string ColB, string ColS, string IndicatorName)
        {
            //dt.DefaultView.Sort(
            //dt.Columns[0].DataType 
            if (dt == null) return;
            if (dt.Rows.Count < 1) return;
            if (dt.Columns.Contains(IndicatorName) == false)
            {
                dt.Columns.Add(new DataColumn(IndicatorName));
            }

            string strBQty = "", strSQty = "";
            int IntBQty, intSQty;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow drB = dt.Rows[i];
                strBQty = Utility.getColumnValue(drB, ColB);
                strSQty = Utility.getColumnValue(drB, ColS);
                IntBQty = Utility.ConvertToInt(strBQty);
                intSQty = Utility.ConvertToInt(strSQty);
                drB.BeginEdit();
                if (IntBQty > 0)
                {
                    drB[IndicatorName] = "b";
                }
                else if (intSQty > 0)
                {
                    drB[IndicatorName] = "s";
                }
                else
                {
                    drB[IndicatorName] = "o";
                }
                //if (IntBQty == 0)      IDelearStatus.ChangeSelectedAcc(o, ss);
                //{
                //    drB[ColB] = "";
                //}
                //if (intSQty == 0)
                //{
                //    drB[ColS] = "";
                //}
                drB.EndEdit();
            }

        }

        private void bttest_Click_1(object sender, RoutedEventArgs e)
        {
            MdiContainer Container = App.Current.MainWindow.FindName("Container") as MdiContainer;
            MdiChild ClosePositionChild = new MdiChild();
            OrderConfirm ORDE = new OrderConfirm(ClosePositionChild);
            {
                ClosePositionChild.Title = "ORDER CONFIRM  ";
                ClosePositionChild.Content = ORDE;
                ClosePositionChild.Width = ORDE.Width;
                ClosePositionChild.Height = ORDE.Height;
                ClosePositionChild.Position = new System.Windows.Point(0, 0);
                if (!Container.Children.Contains(ClosePositionChild))
                {
                    Container.Children.Add(ClosePositionChild);
                }


                //string ss=this.tbTestAc.Text.Trim();
                //object o = new object();
                //IDelearStatus.ChangeSelectedAcc(o,ss);
                // IDelearUser.ChanerAccFromOtherForm(o, ss);
            }
        }
        
        #region Right Menu for Openning Order History

        void AddRightContextMenuOHist(DataGrid dg)
        {
            ContextMenu cm = new ContextMenu();
            MenuItem mi = new MenuItem(); 
            mi.Header = strRs_OHist_CMenuText;
            mi.Click +=(sender,e)=>{
                if (dg.SelectedItem != null)
                {
                    DataRowView drv =dg.SelectedItem as DataRowView;
                    if (drv == null) return;
                    string orderNo = drv.Row.getColValue("internalOrderNo");
                    if (orderNo.Trim() == "")
                    {
                        return;
                    }
                    OpenHistory(orderNo, distributeMsg);               
                }
                e.Handled = true; 
            };
            cm.Items.Add(mi);

            //2014-12-31 Done Trade
         /*
            MenuItem miDoneTrade = new MenuItem();
            miDoneTrade.Header = "Done Trade";
            miDoneTrade.Click += (sender, e) =>
            {
                if (dg.SelectedItem != null)
                {
                    DataRowView drv = dg.SelectedItem as DataRowView;
                    if (drv == null) return;
                    string orderNo = drv.Row.getColValue("internalOrderNo");
                    if (orderNo.Trim() == "")
                    {
                        return;
                    }
                    OpenDoneTrade(orderNo, distributeMsg);
                }
                e.Handled = true;
            };
            cm.Items.Add(miDoneTrade);
          */
            dg.ContextMenu = cm;           
        }

        void AddsRightContextMenu()
        {
            AddRightContextMenuOHist(this.dg_tabOrder_ClearTrade);
            AddRightContextMenuOHist(this.dg_OrderBook);  
        }

        string strRs_OHist_CMenuText
        {
            get { return GOSTS.GosCulture.CultureHelper.GetString("OHist_CMenuText_ToShow"); }
        }
        private void OpenHistory(string orderNo, MessageDistribute _Dist)
        {
            MdiContainer Container = App.Current.MainWindow.FindName("Container") as MdiContainer;
            MdiChild MdiChildOrHist = new MdiChild();
            GOSTS.Accounts.OrderHistory oh = new Accounts.OrderHistory(MdiChildOrHist, this.CurrentID, orderNo, _Dist);

            MdiChildOrHist.Width = 600;
            MdiChildOrHist.Height = 200;

            MdiChildOrHist.Position = new System.Windows.Point(50, 55);
            MdiChildOrHist.Content = oh;
            if (!Container.Children.Contains(MdiChildOrHist))
            {
                Container.Children.Add(MdiChildOrHist);
            }
            MdiChildOrHist.Focus();
        }

        //20141231
        private void OpenDoneTrade(string orderNo, MessageDistribute _Dist)
        {
            MdiContainer Container = App.Current.MainWindow.FindName("Container") as MdiContainer;
            MdiChild MdiChildOrHist = new MdiChild();
            GOSTS.Accounts.DoneTrade oh = new Accounts.DoneTrade(MdiChildOrHist,_Dist);

            MdiChildOrHist.Width = 600;
            MdiChildOrHist.Height = 200;

            MdiChildOrHist.Position = new System.Windows.Point(50, 55);
            MdiChildOrHist.Content = oh;
            if (!Container.Children.Contains(MdiChildOrHist))
            {
                Container.Children.Add(MdiChildOrHist);
            }
            MdiChildOrHist.Focus();
        }

        public void ChangLangInRuntime()
        {
            ChangeContextMenuOHist_HeaderLang(this.dg_OrderBook);
            ChangeContextMenuOHist_HeaderLang(this.dg_tabOrder_ClearTrade);
        }

        void ChangeContextMenuOHist_HeaderLang(DataGrid dg)
        {
            ContextMenu cm = dg.ContextMenu;
            if (cm == null) return;
            if (cm.Items == null) return;
            if (cm.Items[0] is MenuItem)
            {
                MenuItem mi = cm.Items[0] as MenuItem;
                if (mi == null) return;
                mi.Header = strRs_OHist_CMenuText;
            }
        }
        #endregion

        //2014-10-11 "Transactions| - [{0}<{1} {2}{3}>]
        UOITitleStatus TitleStatus = new UOITitleStatus();
        public override void SetFormTitle(int itype, MessageDistribute _distributeMsg, MdiChild _mdiChild, params string[] _Params)
        {
            TitleStatus.restore(itype, _Params);
            string strTitle = TitleStatus.getTitle(this.strTitleFormat);
            SetMdITitle(_distributeMsg, mdiChild, strTitle);// "Transactions - [" + this.CurrentID.ToString() + "<" + Sex + " " + TradeStationTools.Base64Utf16StringToString(accName) + idBrNo + @">]");
        }

        public override void SetFormTitle()
        {
            string strTitle = TitleStatus.getTitle(this.strTitleFormat);
            SetMdITitle(distributeMsg, mdiChild, strTitle);
        }
    }


    public class UOITitleStatus
    {
        public string CurID="";
        public string Sex="";
        public string accName="";
        public string idBrNo="";

        public int iType = 0;
        public string getTitle(string strTitleFormat)
        {
            string[] Arry = strTitleFormat.Split('|');
            string str = "";
            switch (iType)
            {
                case 0:
                    str = Arry[0];
                    break;
                case 1:
                    str = string.Format(strTitleFormat, CurID, Sex,accName,idBrNo);
                    str = str.Replace("|", "");
                    break;
            }
            return str;
        }

        public void restore(int _itype,params string[] parms)
        {
            switch (_itype)
            {
                case 0:
                    iType = _itype;
                    CurID = "";
                    Sex = "";
                    accName = "";
                    idBrNo = "";
                    break;
                case 1:
                    iType = _itype;
                    CurID =GOSTS.Utility.getArrayItemValue(parms,0);
                    Sex = GOSTS.Utility.getArrayItemValue(parms, 1);
                    accName = GOSTS.Utility.getArrayItemValue(parms, 2);
                    idBrNo = GOSTS.Utility.getArrayItemValue(parms, 3);
                    break;
                default:
                    iType = _itype;
                    CurID = "";
                    Sex = "";
                    accName = "";
                    idBrNo = "";
                    break;
            }
        }
         // SetMdITitle(distributeMsg, mdiChild,"Transactions - [" + this.CurrentID.ToString() + "<" + Sex + " " + TradeStationTools.Base64Utf16StringToString(accName) + idBrNo + @">]");
    }


    public class zeroToBlankConvertor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return "";
            string str = value.ToString();
            if (str.Trim() == "0")
            {
                return "";
            }
            //if (System.Text.RegularExpressions.Regex.IsMatch(str, @"^(\-)?\d+(\.(\d){1,})*$"))
            //{
            //    try
            //    {
            //        decimal d = System.Convert.ToDecimal(str);
            //        if (d == 0)
            //        {
            //            return "";
            //        }
            //    }
            //    catch { }
            //}
            return str;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string strValue = value as string;         
            return strValue;
        }
    }

    public class TrimBlankConvertor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return "";
            string str = value.ToString().Trim();            
            return str;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string strValue = value as string;
            return strValue;
        }
    }

    public class StatusBase64ToUIConvertor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return "";
            string str = value.ToString();
            int pos=str.IndexOf(";");
            if (pos < 0)
            {
                return value;
            }
            string strLeft = str.Substring(0, pos);
             str=System.Text.RegularExpressions.Regex.Replace(str,@"^\d+;","");             
             str=TradeStationTools.Base64StringToString(str);
             return str;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string strValue = value as string;
            return strValue;
        }
    }
}
