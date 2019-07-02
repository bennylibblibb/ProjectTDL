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
using WPF.MDI;
using System.Data;
using System.ComponentModel;
using System.Collections;
using System.Collections.ObjectModel;
using System.Windows.Interop;

namespace GOSTS
{
    /// <summary>
    /// Interaction logic for OrderEntry.xaml
    /// </summary>
    /// 

    public partial class OrderEntry : IDelearUser
    {
        //add by ben
        public MdiChild mdiChild { get; set; }
        private string _gosProdCode = "";
        public string ProdCode { 
            get{
                if (_gosProdCode == null) { _gosProdCode = ""; }
                return _gosProdCode;
            }
            set
            {
                _gosProdCode = value; 
                if (this.txt_Price_NT != null)
                {
                    this.txt_Price_NT.SetProdCode(ProdCode);
                }
                if (this.txt_Price_CT != null)
                {
                    txt_Price_CT.SetProdCode(ProdCode);
                }
                if (txt_StopPrice_CT != null)
                {
                    txt_StopPrice_CT.SetProdCode(ProdCode);
                }
                if (txt_StopPrice_OCO != null)
                {
                    txt_StopPrice_OCO.SetProdCode(ProdCode);
                }
                if (txt_AttractPrice_OCO != null)
                {
                    txt_AttractPrice_OCO.SetProdCode(ProdCode);
                }
                if (this.btnQTradeSell != null)
                {
                    this.btnQTradeSell.IsEnabled = true;
                }
                if (this.btnQuickBuy != null)
                {
                    this.btnQuickBuy.IsEnabled = true;
                }
            }
        }
        //delegate void BindListItemTicker(DataTable dataTable);
        //BindListItemTicker TickerDelegate;
        delegate void BindListItemPriceDepth(DataTable dataTable);
        BindListItemPriceDepth PriceDepthDelegate;

        delegate void BindListItemMarketPrice(ObservableCollection<MarketPriceItem> MktItems);
        BindListItemMarketPrice MarketPriceDelegate;

        delegate void NotificationItem(string str);
        NotificationItem NotificationDelegate;

        delegate void DisOrderConfirm(string str);
        DisOrderConfirm DisOrderConfirmDelegate;

        delegate void BindListItemAccout(DataTable dataTable);
        BindListItemAccout AccoutDelegate;

        private MessageDistribute distributeMsg = null;
      
        MdiChild orderChild;
        UserData userAccountID = new UserData();
        string strAccountID;

        OrderModel orderModel = new OrderModel();
        OrderModel quickOrderModel = new OrderModel();

        PriceDataViewModel viewModel;
        delegate void BindListData(ObservableCollection<LongPriceDepthData> data);
        BindListData PriceDepthDataDelegate;

        delegate void BindTickerItem(ObservableCollection<TickerData> TickerDataes);
        BindTickerItem TickerDataDelegate;
        TickerViewModel viewModelTicker;
        bool isRegistered = false;

        #region Resource Text
        string strRs_Prod_NotExist
        {
            get { return GOSTS.GosCulture.CultureHelper.GetString("EN_Prod_NotExist"); }
        }      
        public static string strRs_IncActive
        {
            get { return GOSTS.GosCulture.CultureHelper.GetString("ET_TbN_Inactive"); }
        }
        #endregion

        //public delegate void OnDisEnterOrderControl(object sender, string prodCode, string Price);
        //public event OnDisEnterOrderControl DisEnterOrderControl;
        public OrderEntry(MessageDistribute _msgDistribute, MdiChild mdi)
        { 
            InitializeComponent();
            NotificationDelegate = new NotificationItem(NotificationItemMethod);
            DisOrderConfirmDelegate = new DisOrderConfirm(OpenOrderConfrim);

            orderChild = mdi;

            //2014-10-11 cancel
           // SetMdITitle(_msgDistribute, orderChild, "Enter Order");
         
            SetContext();
            if (_msgDistribute != null)
            {
                distributeMsg = _msgDistribute;
                distributeMsg.DisEnterOrderControl+=distributeMsg_DisEnterOrderControl;
                //add by ben
               // distributeMsg.DisControlOrder += new MessageDistribute.OnDisControlOrder(distributeMsg_DisControlOrder); 
                distributeMsg.DisControlOrderItem += new MessageDistribute.OnDisControlOrderItem(distributeMsg_DisControlOrderItem); 
                distributeMsg.DisPMPriceDepthData += new MessageDistribute.OnDisPMPriceDepthData(distributeMsg_MPPriceDepthObs); 
                distributeMsg.DisPMTickerItemData += new MessageDistribute.OnDisPMTickerItemData(distributeMsg_MPTickerData);
                
                //distributeMsg.DisMPControlOrder += new MessageDistribute.OnDisMPControlOrder(distributeMsg_DisMarketPrice);
                distributeMsg.DisMarketPrice += new MessageDistribute.OnDisMarketPrice(distributeMsg_DisMarketPrice);
                
                PriceDepthDelegate = new BindListItemPriceDepth(BindListItemMethodPriceDepth); 
                MarketPriceDelegate = new BindListItemMarketPrice(BindListItemMethodMarketPrice);
               
                PriceDepthDataDelegate = new BindListData(BindListItemDataMethod);
                TickerDataDelegate = new BindTickerItem(BindTickerDataMethod);

                viewModelTicker = new TickerViewModel();
                this.dgTicker.ItemsSource = viewModelTicker.TickerList5;
 
                viewModel = new PriceDataViewModel();
                this.dgPriceDepth.ItemsSource = viewModel.DataList2;
              
                #region add by harry                
                InitCurUser(this.cbbUsers, _msgDistribute);
                MKReqType = this.GetHashCode().ToString();
                #endregion  

                //2014-10-11              
                SetFormTitle(0, _msgDistribute, orderChild, null);

                string strU = IDelearStatus.ACCUoAc;
                if (strU != null)
                {
                    if (strU.Trim() != "")
                        SetCobBoxSelectedUser(strU);
                }
            
                if (orderChild != null)
                {
                    orderChild.OnDragThumbClick += new MdiChild.deleDragThumbClick(this.SetSelect);
                }

                if (GOSTradeStation.isDealer == false)
                {
                    gdAccInfo.Visibility = Visibility.Collapsed;
                }
            }
        }
              
        
        //add by ben
        protected void distributeMsg_MPTickerData(object sender, ObservableCollection<TickerData> TickerDataes)
        {
            try
            {
                Application.Current.Dispatcher.BeginInvoke(this.TickerDataDelegate, new Object[] { TickerDataes });

            }
            catch(Exception exp)
            {
                TradeStationLog.WriteParserResult(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + "Type: distributeMsg_MPTickerData,Error:" + exp.Message);
                          
            }
        }

        public void BindTickerDataMethod(ObservableCollection<TickerData> TickerDataes)
        {
            IEnumerable<TickerData> data = TickerDataes.TakeWhile(c => c.productCode == this.ProdCode).Reverse().Take(5).Reverse ();
            if (data.Count()>0)
            {
                foreach (TickerData item in data)
                {
                    viewModelTicker.TickerList5.Insert(0, item);
                    if (viewModelTicker.TickerList5.Count > 5)
                    {
                        viewModelTicker.TickerList5.RemoveAt(viewModelTicker.TickerList5.Count - 1);
                    }
                }
            } 
        }

        protected void distributeMsg_MPPriceDepthObs(object sender, ObservableCollection<LongPriceDepthData> priceDepthData)
        {
            try
            {
                Application.Current.Dispatcher.BeginInvoke(this.PriceDepthDataDelegate, new Object[] { priceDepthData });
            }
            catch (Exception exp)
            {
                TradeStationLog.WriteParserResult(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + "Type: distributeMsg_MPPriceDepthObs,Error:" + exp.Message);

            }
        }

        public void BindListItemDataMethod(ObservableCollection<LongPriceDepthData> data)
        {
            if (data.Count != 0 && data[0].ProdCode == this.ProdCode)
            {
                if (viewModel.DataList2.Count == 0)
                {
                    foreach (LongPriceDepthData item in data)
                    {
                        viewModel.DataList2.Add(item);
                    }
                }
                else
                {
                    foreach (LongPriceDepthData item in data)
                    {
                        var ls = viewModel.DataList2.FirstOrDefault(x => (x.ID == item.ID));
                        if (ls != null)
                        {
                            ls.Bid = item.Bid;
                            ls.BQty = item.BQty;
                            ls.Ask = item.Ask;
                            ls.AQty = item.AQty;
                        }
                    }
                }
            }
        }


        MKReqManage MKReqMaitenance;
        string MKReqType;
        
        bool bReceiveMK=true;//标志从上次输入产品代码号后，是否已经获得服务器的返回信息；输入product id并回车后设false，如果收到服务器返回对应product code信息，设true；如果双击了市价，设true;
        public bool CanTrade()
        {
            if (bReceiveMK == false)
            {
                string strRs_EN_Prod_InQuery = GOSTS.GosCulture.CultureHelper.GetString("EN_Prod_InQuery");
                MessageBox.Show(strRs_EN_Prod_InQuery);//"waiting for enquiry of product info...,or you can select another product");
                return false ;
            }

            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        public void LoadLastProduct()
        {
            return;
            #region cancel temp
            //string pid = IDelearStatus.GetOrderEntryProduct();
            //if (pid != "")
            //{
            //    string strCode = pid;
            //    if (strCode == "") return;
            //    bool exists = MarketPriceData.ExistInProduct(strCode);
            //    if (exists == false)
            //    {                    
            //        this.txt_OrderID.Text.Trim();
            //        return;
            //    }               
            //    this.ProdCode = strCode;                
            //    List<string> lsnew = new List<string>();
            //    lsnew.Add(strCode);
            //    bReceiveMK = false;

            //    TradeStationSend.Get(lsnew, cmdClient.getMarketPrice);
            //    TradeStationSend.Send(null, lsnew, cmdClient.registerMarketPrice);

            //    this.tbOpen.Text = "";
            //    this.tbClose.Text = "";
            //    this.tbChange.Text = "";
            //    this.tbHigh.Text = "";
            //    this.tbLow.Text = "";
            //    this.tbVolQty.Text = "";
            //    this.tbVolume.Text = "";

            //    this.txt_OrderID.Text = this.ProdCode;
            //    this.lbProdName.Content = "";
            //    //this.txt_Price_NT.Text = "0";
            //    Set_Price_TextBox("0");

            //    this.orderModel.ProductCode = strCode;
            //  //this.orderModel.ProductName = "";
            //    this.quickOrderModel.Ask = 0;
            //    this.quickOrderModel.Bid = 0;
            //    SetQty("1", this.ProdCode);
            //    string Pname = TradeStationSetting.ReturnWindowName(WindowTypes.OrderEntry, ProdCode);
            //    string Pname1 = Pname.Trim();
            //    int pos = Pname1.IndexOf("(");
            //    if (pos > 0)
            //    {
            //        Pname1 = Pname1.Substring(pos);
            //    }
            //    pos = Pname1.IndexOf("(");
            //    if (pos > 0)
            //    {
            //        Pname1 = Pname1.Substring(0, pos + 1);
            //    }
            //    this.orderModel.ProductName = Pname1;
            //    this.lbProdName.Content = Pname1;
            //    if (this.mdiChild != null)
            //    {
            //        this.mdiChild.Title = Pname;
            //    }
            //}
            #endregion
        }

        void txt_OrderID_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)// private void txt_OrderID_LostFocus_1(object sender, RoutedEventArgs e)       
        {
            string strCode = this.txt_OrderID.Text.Trim();
            if (this.ProdCode.Trim() == strCode)
            {
                return;
            }
            List<string> lsold = new List<string>();
            if (strCode == ""){
                if (this.ProdCode != "")
                {
                    lsold.Add(this.ProdCode);
                    TradeStationSend.Send(lsold, null, cmdClient.registerMarketPrice);
                    this.ProdCode = "";
                    if (this.ProdCode.Trim() != "")
                    {
                        distributeMsg.DistributeControl(this.ProdCode);
                    }
                    //if (this.ProdCode.Trim() != "")
                    //{
                    //    distributeMsg.DistributeControl(this.ProdCode);
                    //}
                }
                clearProdInfo();//check if anything to clear?
                return;
            }

            bool exists = MarketPriceData.ExistInProduct(strCode);
            if (exists == false)
            {               
                string _msg = string.Format(strRs_Prod_NotExist, strCode);
                MessageBox.Show(_msg);
                this.txt_OrderID.Text=this.ProdCode;
                return;
            }

            string oldCode = "";
            if (this.ProdCode != null)
            {
                oldCode = this.ProdCode.Trim();
            }
            isDiEndterOrder = false;          
            this.ProdCode = strCode;
            if (this.ProdCode.Trim() != "")
            {
                distributeMsg.DistributeControl(this.ProdCode);
            }
          
            if (oldCode != "")
            {
                lsold.Add(oldCode);
            }
            List<string> lsnew = new List<string>();
            lsnew.Add(strCode);
            bReceiveMK = false;
                      
            viewModelTicker.TickerList5.Clear();
            viewModel.DataList2.Clear();

            TradeStationSend.Get(lsnew, cmdClient.getMarketPrice);
            TradeStationSend.Send(lsold, lsnew, cmdClient.registerMarketPrice);

            this.tbOpen.Text = "";
            this.tbClose.Text = "";
            this.tbChange.Text = "";
            this.tbHigh.Text = "";
            this.tbLow.Text = "";
            this.tbVolQty.Text = "";
            this.tbVolume.Text = "";

         //   this.txt_OrderID.Text = this.ProdCode;
            this.lbProdName.Content = "";
            //this.txt_Price_NT.Text = "0"; 
            Set_Price_TextBox("0");
            SetQty("1", this.ProdCode);
            this.txtQTradeTicks.Text = "0";

            this.orderModel.ProductCode = strCode;
            this.orderModel.ProductName = "";
            this.quickOrderModel.Ask = 0;
            this.quickOrderModel.Bid = 0;

            #region 2014-10-11 cancel
            //string Pname = TradeStationSetting.ReturnWindowName(WindowTypes.OrderEntry, ProdCode);
            //string Pname1 = Pname.Trim();
            //int pos = Pname1.IndexOf("(");
            //if (pos > 0)
            //{
            //    Pname1 = Pname1.Substring(pos);
            //}
            //pos = Pname1.IndexOf("(");
            //if (pos > 0)
            //{
            //    Pname1 = Pname1.Substring(0, pos + 1);
            //}
            //this.orderModel.ProductName = Pname1;
            //this.lbProdName.Content = Pname1;
            //if (this.mdiChild != null)
            //{
            //    SetMdITitle(this.distributeMsg, this.mdiChild, Pname);
            //}
            #endregion
            string _prodName = MarketPriceData.GetProductName(ProdCode);
            this.orderModel.ProductName = _prodName;
            this.lbProdName.Content = _prodName;
            if (this.mdiChild != null)
            {
                SetFormTitle(1, this.distributeMsg, this.mdiChild, new string[] { ProdCode, _prodName });
            }           

            // SetUPDownBtnStep();
            IDelearStatus.SetOrderEntryProduct(ProdCode);
        }
        /// <summary>
        /// invoke only after product textbox changed and set to public variable ->this.ProdCode.
        /// </summary>
        void clearProdInfo()
        {
           // SetMdITitle(this.distributeMsg, this.mdiChild, "Enter Order");
            SetFormTitle(0, this.distributeMsg, this.mdiChild, null);
            bReceiveMK = true;
            CurrentProdMktItem = null;
            this.orderModel.ProductCode = "";
            this.orderModel.ProductName = "";
            this.quickOrderModel.Ask = 0;
            this.quickOrderModel.Bid = 0;

            this.txt_Count_NT.Text ="0";
            this.txt_Count_CT.Text = "0";
            this.txt_Price_CT.Text = "0";
            this.txt_Price_NT.Text = "0";
            this.txtQTradeTicks.Text = "0";
            this.txt_StopPrice_CT.Text = "0";
            this.lbProdName.Content = "";

            this.tbOpen.Text = "";
            this.tbClose.Text = "";
            this.tbChange.Text = "";
            this.tbHigh.Text = "";
            this.tbLow.Text = "";
            this.tbVolQty.Text = "";
            this.tbVolume.Text = "";
        }
        private void txt_OrderID_KeyDown_1(object sender, KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                this.txt_OrderID.LostKeyboardFocus -= txt_OrderID_LostKeyboardFocus;
                string strCode = this.txt_OrderID.Text.Trim();
                List<string> lsold = new List<string>();
                if (strCode == "")
                {
                    if (this.ProdCode != "")
                    {
                        lsold.Add(this.ProdCode);
                        TradeStationSend.Send(lsold, null, cmdClient.registerMarketPrice);
                        this.ProdCode = "";
                    }
                    clearProdInfo();//check if anything to clear?
                    this.txt_OrderID.LostKeyboardFocus += txt_OrderID_LostKeyboardFocus;
                    return;
                }

                bool exists = MarketPriceData.ExistInProduct(strCode);
                if (exists == false)
                {
                    string _msg = string.Format(strRs_Prod_NotExist, strCode);
                    MessageBox.Show(_msg);//"ID:" + strCode + " not exists");
                    this.txt_OrderID.Text = this.ProdCode;
                    this.txt_OrderID.LostKeyboardFocus += txt_OrderID_LostKeyboardFocus;
                    return;
                }
                string oldCode = "";
                if (this.ProdCode != null)
                {
                    oldCode = this.ProdCode.Trim();
                }
                isDiEndterOrder = false;
                if (oldCode == strCode)
                {
                 //   return;
                }
                this.ProdCode = strCode;
                if (this.ProdCode.Trim() != "")
                {
                    distributeMsg.DistributeControl(this.ProdCode);
                }
               
                if (oldCode != "")
                {
                    lsold.Add(oldCode);
                }               
                List<string> lsnew = new List<string>();
                lsnew.Add(strCode);
                bReceiveMK = false;

                //MKReqMaitenance = MKReqManage.GetSingleInstance();
                //if (MKReqMaitenance != null)
                //{
                //    MKReqMaitenance.SendReq(lsnew, oldCode == "" ? null : lsold, MKReqType);
                //}
                viewModelTicker.TickerList5.Clear();
                viewModel.DataList2.Clear();

                TradeStationSend.Get(lsnew, cmdClient.getMarketPrice );
                TradeStationSend.Send(lsold, lsnew, cmdClient.registerMarketPrice); 

                this.tbOpen.Text = "";
                this.tbClose.Text = "";
                this.tbChange.Text = "";
                this.tbHigh.Text = "";
                this.tbLow.Text = "";
                this.tbVolQty.Text = "";
                this.tbVolume.Text = "";

                this.txt_OrderID.Text = this.ProdCode;
                this.lbProdName.Content = "";
                Set_Price_TextBox("0");
                SetQty("1", this.ProdCode);
                this.txtQTradeTicks.Text = "0";

                this.orderModel.ProductCode = strCode;
                this.orderModel.ProductName = "";
                this.quickOrderModel.Ask = 0;
                this.quickOrderModel.Bid = 0;
               
                string Pname = MarketPriceData.GetProductName(ProdCode);
                this.orderModel.ProductName = Pname;
                this.lbProdName.Content = Pname;
                if (this.mdiChild != null)
                {                   
                    SetFormTitle(1, this.distributeMsg, this.mdiChild, new string[] { ProdCode, Pname });
                }

                // SetUPDownBtnStep();
                IDelearStatus.SetOrderEntryProduct(ProdCode);
                this.txt_OrderID.LostKeyboardFocus += txt_OrderID_LostKeyboardFocus;
            }
        }

        bool isDiEndterOrder = false;
        //register to  delegate of double click price depth
        private void distributeMsg_DisEnterOrderControl(object sender, string newProdCode, float price, int Qty,bool  _bIsAo)
        {
            if (newProdCode == null) return;
            if (newProdCode.Trim() == "") return;
            //bool exists = MarketPriceData.ExistInProduct(newProdCode);
            // if (exists == false)
            // {
            //     MessageBox.Show("ID:" + newProdCode + " not exists");
            //     return;
            // }
            
            if (this.tbuCode.IsChecked == true)
            {
                if (newProdCode != this.ProdCode)
                {
                    return;
                }
            }

            SetPrice(price, newProdCode, _bIsAo);
            SetQty(Qty.ToString(), newProdCode);
            
            string oldCode = "";
            if (this.ProdCode != null)
            {              
                oldCode = this.ProdCode.Trim();
            }            
            
            bReceiveMK = true;
            isDiEndterOrder = true;

            if (oldCode != newProdCode)
            {
                this.ProdCode = newProdCode.Trim();
                if (this.ProdCode.Trim() != "")
                {
                    distributeMsg.DistributeControl(this.ProdCode);
                }
                List<string> lsold = new List<string>();
                if (oldCode != "")
                {
                    lsold.Add(oldCode);
                }
                List<string> lsnew = new List<string>();
                lsnew.Add(newProdCode);

                viewModelTicker.TickerList5.Clear();
                viewModel.DataList2.Clear();

                TradeStationSend.Get(lsnew, cmdClient.getMarketPrice);
                TradeStationSend.Send(lsold, lsnew, cmdClient.registerMarketPrice);

                this.tbOpen.Text = "";
                this.tbClose.Text = "";
                this.tbChange.Text = "";
                this.tbHigh.Text = "";
                this.tbLow.Text = "";
                this.tbVolQty.Text = "";
                this.tbVolume.Text = "";

                this.txt_OrderID.Text = this.ProdCode;
                this.orderModel.ProductCode = newProdCode;
                this.orderModel.ProductName = "";
                this.quickOrderModel.Ask = 0;
                this.quickOrderModel.Bid = 0;             

                string Pname = MarketPriceData.GetProductName(ProdCode);
                this.orderModel.ProductName = Pname;
                this.lbProdName.Content = Pname;
                if (this.mdiChild != null)
                {                  
                    SetFormTitle(1, this.distributeMsg, this.mdiChild, new string[] { ProdCode, Pname });
                }


                this.txtQTradeTicks.Text = "0";                      
                IDelearStatus.SetOrderEntryProduct(ProdCode);
            }

           
        }
        MarketPriceItem CurrentProdMktItem = null;
        public void BindListItemMethodMarketPrice(ObservableCollection<MarketPriceItem> MktItems) //DataTable dataTable)
        {
            if (this.tbuCode.IsChecked == true) return;
      
            if (MktItems == null) return;           
            
            var results =MktItems.FirstOrDefault(x=> x.ProductCode== this.ProdCode );//               
            if (results!=null)
            {  
                if (bReceiveMK == false)
                {
                    this.tbOpen.Text = results.Open.ToString();
                    this.tbClose.Text = results.PreClose.ToString();
                    this.tbChange.Text = results.Change.ToString() + "(" + results.ChangePer + "%)";
                    this.tbHigh.Text = results.High.ToString();
                    this.tbLow.Text = results.Low.ToString();
                    this.tbVolQty.Text = results.LQty.ToString();
                    this.tbVolume.Text = results.Volume.ToString();

                    Set_Price_TextBox(results.Last.ToString().Trim(), this.ProdCode);
                    bReceiveMK = true;
                }
                CurrentProdMktItem = results;
                //harry get ask,bid for QuickTrade
                if (quickOrderModel != null)
                {
                    string str = results.Ask.ToString().Trim();
                    if(GosBzTool.IsAOValue(str))                   
                    {
                        this.btnQuickBuy.IsEnabled = false;
                        quickOrderModel.Ask = GosBzTool.SetIntAoValue(); 
                    }
                    else
                    {
                        try
                        {
                            quickOrderModel.Ask = GosBzTool.adjustDecLength(this.ProdCode, Convert.ToDecimal(str));// GosBzTool.getDecimalPrice(this.ProdCode, str);// 2013-07-22; Convert.ToInt32(str);
                        }
                        catch { }
                        this.btnQuickBuy.IsEnabled = true ;
                    }

                    str = results.Bid.ToString();
                    if (GosBzTool.IsAOValue(str))         
                    {
                        this.btnQTradeSell.IsEnabled = false;
                        quickOrderModel.Bid = GosBzTool.SetIntAoValue(); 
                    }
                    else
                    {
                        try
                        {
                            quickOrderModel.Bid = GosBzTool.adjustDecLength(this.ProdCode, Convert.ToDecimal(str));// GosBzTool.getDecimalPrice(this.ProdCode, str);// Convert.ToInt32(str);
                        }
                        catch { }
                        this.btnQTradeSell.IsEnabled = true;
                    }
                }                
            }           
        }

        protected void distributeMsg_DisMarketPrice(object sender,ObservableCollection<MarketPriceItem> MktItems)// DataTable dataTable)
        {
            try
            {
                Application.Current.Dispatcher.BeginInvoke(this.MarketPriceDelegate, new Object[] { MktItems });
            }
            catch (Exception exp)
            {
                TradeStationLog.WriteParserResult(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + "Type: distributeMsg_DisMarketPrice,Error:" + exp.Message);
            }
        }

        // add by ben 2013/08/27  trigger by clicked MarketPrice 
        protected void distributeMsg_DisControlOrderItem(object sender, MarketPriceItem  mpi,string CellType)
        {
            if (this.tbuCode.IsChecked == true) return;
            if (mpi == null) return;
            //2013-11-12 harry add check product code 
            if (mpi.ProductCode == null) return;
            if (mpi.ProductCode.Trim() == "")
            {
                return;
            }
            bReceiveMK = true;//harry
            string defaultTitle = this.mdiChild.Title;
            string OldProdCode = this.ProdCode;

            this.ProdCode = mpi.ProductCode ;
            if (this.ProdCode.Trim() != "")
            {
                distributeMsg.DistributeControl(this.ProdCode);
            }
            this.txt_OrderID.Text = this.ProdCode;

            this.orderModel.ProductCode =mpi.ProductCode;

            string celltype = CellType.ToLower();
            switch(celltype)
            {
                case "name":
                case "last":
                      Set_Price_TextBox(mpi.Last , this.ProdCode);
                      SetQty("1", this.ProdCode);
                    break;
                case "bqty":
                    Set_Price_TextBox(mpi.Bid , this.ProdCode);
                    SetQty(mpi.BQty, this.ProdCode);
                    break;
                case "bid":
                    Set_Price_TextBox(mpi.Bid, this.ProdCode);
                    SetQty("1", this.ProdCode);
                    break;
                case "ask":
                    Set_Price_TextBox(mpi.Ask, this.ProdCode);
                    SetQty("1", this.ProdCode);
                    break;
                case "aqty":
                    Set_Price_TextBox(mpi.Ask, this.ProdCode);
                    SetQty(mpi.AQty, this.ProdCode);
                    break;
                default :
                    Set_Price_TextBox(mpi.Last, this.ProdCode);
                    SetQty("1", this.ProdCode);
                    break;
                
             }
            string strAsk = "";
            try
            {
                //for fear mpi change to null at the very moment
                strAsk = mpi.Ask;
            }
            catch{}
            if(GosBzTool.IsAOValue(strAsk) )         
            {
                this.quickOrderModel.Ask = GosBzTool.SetIntAoValue();
                this.btnQuickBuy.IsEnabled = false;
            }
            else
            {
                this.quickOrderModel.Ask = GosBzTool.adjustDecLength(this.ProdCode, Utility.ConvertToDecimal(strAsk));// GosBzTool.getDecimalPrice(this.ProdCode, drv.Row.getColValue("ask"));// 2013-07-22 Utility.ConvertToInt(Utility.getColumnValue(drv, "ask"));
                this.btnQuickBuy.IsEnabled = true;
            }
            string strBid = "";
            try
            {
                strBid = mpi.Bid;
            }
            catch (Exception ex)
            {}
            if(GosBzTool.IsAOValue(strBid))          
            {
                this.quickOrderModel.Bid = GosBzTool.SetIntAoValue();
                this.btnQTradeSell.IsEnabled = false;
            }
            else
            {
                this.quickOrderModel.Bid = GosBzTool.adjustDecLength(this.ProdCode, Utility.ConvertToDecimal(strBid));//  GosBzTool.getDecimalPrice(this.ProdCode, drv.Row.getColValue("bid"));//  Utility.ConvertToInt(Utility.getColumnValue(drv, "bid"));
                this.btnQTradeSell.IsEnabled = true;
            }
            string strProductName = mpi.ProductName;
            string strProductCode = mpi.ProductCode;
            isDiEndterOrder = false;      

            string Pname = MarketPriceData.GetProductName(ProdCode);
            this.orderModel.ProductName = Pname;
            this.lbProdName.Content = Pname;
            if (this.mdiChild != null)
            {
                SetFormTitle(1, this.distributeMsg, this.mdiChild, new string[] { ProdCode, Pname });
            }

            this.tbOpen.Text = mpi.Open;// drv["open"].ToString();
            this.tbClose.Text = mpi.PreClose;// drv["preClose"].ToString();
            this.tbChange.Text = mpi.Change  + "(" + mpi.ChangePer + "%)";
            this.tbHigh.Text = mpi.High;// drv["high"].ToString();
            this.tbLow.Text = mpi.Low;// drv["low"].ToString();
            this.tbVolQty.Text = mpi.Volume;// drv["volume"].ToString();
            this.tbVolume.Text = "";//["volume"].ToString();           

            if (this.ProdCode != OldProdCode)
            {
                isRegistered = false;
                viewModelTicker.TickerList5.Clear();
                viewModel.DataList2.Clear();
               
                this.txtQTradeTicks.Text = "0";
                // SetUPDownBtnStep();
                IDelearStatus.SetOrderEntryProduct(ProdCode);
            }
            if (isRegistered == false)
            {
                TradeStationSend.Send(OldProdCode, this.ProdCode, cmdClient.registerMarketPrice);
                //TradeStationSend.Send(OldProdCode, this.ProdCode, cmdClient.registerPriceDepth);
                //TradeStationSend.Send(OldProdCode, this.ProdCode, cmdClient.registerTicker);
                isRegistered = true;
            }
            distributeMsg.DistributeControlFocus(defaultTitle, this.mdiChild);
        }

        void SetQty(string sValue,string _ProdCode)
        {
            int Qty = 1;
            try
            {
                Qty = Convert.ToInt32(sValue);
            }
            catch {}
            Qty = GosBzTool.Verify_Order_Qty_ByBound(Qty, _ProdCode);
            this.txt_Count_NT.Text = Qty.ToString();
            this.txt_Count_CT.Text = Qty.ToString();
        }

        public void BindListItemMethodPriceDepth(DataTable dataTable)
        {
            // if (this.tbuCode.IsChecked == true) return;
            PriceDataViewModel viewModel =new  PriceDataViewModel();
            if (viewModel.GetPriceDepthData(this.ProdCode, dataTable) > 0)
            {
                this.dgPriceDepth.DataContext = viewModel;
            }
        }

        protected void distributeMsg_MPPriceDepth(object sender, DataTable dataTable)
        {
            Application.Current.Dispatcher.BeginInvoke(this.PriceDepthDelegate, new Object[] { dataTable });
        }

        private void Expander_Collapsed(object sender, RoutedEventArgs e)
        {

        }

        private void Expander_Expanded(object sender, RoutedEventArgs e)
        {
            //if (this.ProdCode == null || isRegistered) return;
            // TradeStationSend.Send (null, this.ProdCode, cmdClient.registerPriceDepth);
            // TradeStationSend.Send (null, this.ProdCode, cmdClient.registerTicker);
            // isRegistered = true;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.ProdCode = this.txt_OrderID.Text;
            //if (this.ProdCode.Trim() != "")
            //{
            //    distributeMsg.DistributeControl(this.ProdCode);
            //}
        }


        string getBTSourceInput()
        {
            StringBuilder builder=new StringBuilder("Normal Trade input,");
           
            string strPrice = txt_Price_NT.Text;
            builder.Append("price:" + strPrice + GosTestGlobal.itSeperate);
            string strQty = txt_Count_NT.Text;
            builder.Append("qty:" + strPrice + GosTestGlobal.itSeperate);
            object obj=ntValid.SelectedItem  ;
            string strValid="";
            ComboBoxItem item = obj as ComboBoxItem;
            if (item != null)
            {
                if (item.Tag != null)
                {
                    strValid = item.Tag.ToString();
                }
            }
            builder.Append("Valid:" + strValid + GosTestGlobal.itSeperate);
            string strValidDate = ntSecTime.SelectedDate == null ? "" : ntSecTime.SelectedDate.Value.ToShortDateString();
            builder.Append("ValidDate:" + strValidDate + GosTestGlobal.itSeperate);
            builder.Append("is ao:" + ((chkAO.IsChecked??false)?"N":"Y") + GosTestGlobal.itSeperate);
            builder.Append("is active:" + ((chkNTActive.IsChecked??false)?"N":"Y") + GosTestGlobal.itSeperate);
            builder.Append("ref:" + txt_NT_ref.Text + GosTestGlobal.itSeperate);
            return builder.ToString();
        }


        private void btn_NTbuy_Click(object sender, RoutedEventArgs e)
        {
            if (GOSTradeStation.isDealer)
            {
                if (CurrentID == null || CurrentID.Trim() == "")
                {
                    MessageBox.Show(CommonRsText.StrRs_input_Acc_pls);
                    ctrlAccFocus();                 
                    return;
                }
            }
            if (this.ProdCode == null || this.ProdCode.Trim() == "")
            {
                MessageBox.Show(CommonRsText.StrRs_input_prod_pls);
                tbProdFocus();             
                return;
            }
            if (!ValidBS())
            {
                NomalTbPriceFocus();
                return;
            }
            // Buy
           
            if (CanTrade() == false)
            {
                NomalTbPriceFocus();
                return;
            }

            if (GOSTradeStation.isDealer == true && this.CurrentID != this.cbbUsers.cbbUsers.Text.Trim())
            {
                string _msg = string.Format(CommonRsText.strRs_Order_AccNotEqual, this.cbbUsers.cbbUsers.Text.Trim(), this.CurrentID);
                //if (MessageBox.Show("'" + this.cbbUsers.cbbUsers.Text.Trim() + "' not equal current acc '" + this.CurrentID + "',continue place to order with current acc '" + this.CurrentID + "?  ", "Buy Confirm ", MessageBoxButton.OKCancel) != MessageBoxResult.OK)
                if (MessageBox.Show(_msg,CommonRsText.strRs_confirm_BuyOrder_Title, MessageBoxButton.OKCancel) != MessageBoxResult.OK)
                
                {
                    ctrlAccFocus();
                    return;
                }
            }
            try
            {
                decimal OrderPrice = get_txt_Price_NT();
                int intOrderPrice = GosBzTool.ChangeDecToIn(this.ProdCode, OrderPrice);
                strAccountID =CurrentID;
                string strMessage = string.Format(CommonRsText.strRs_confirm_BuyOrder,                   
                    strAccountID,
                    this.ProdCode, 
                    (this.chkAO.IsChecked ?? false) ? "AO" : OrderPrice.ToString(),
                    orderModel.Count,
                   CommonRsText.getValidTypeText( orderModel.Valid) );
                TradeStationComm.Attribute.Active Status = orderModel.Active == 1 ? TradeStationComm.Attribute.Active.inactive : TradeStationComm.Attribute.Active.active;
                TradeStationComm.Attribute.CondType condType = (this.chkAO.IsChecked ?? false) ? TradeStationComm.Attribute.CondType.AO : TradeStationComm.Attribute.CondType.normal; 
                
                long specTime = 0;                
               
                if (orderModel.Valid == TradeStationComm.Attribute.ValidType.specTime)   
                {
                    if (orderModel.dtSpecialTime.HasValue)
                    {
                        specTime = Utility.ConvertToUnixTime(orderModel.dtSpecialTime.Value);
                    }
                    if (specTime == 0)
                    {
                        MessageBox.Show(CommonRsText.strRs_Input_order_specDate_pls);
                        NomalTbPriceFocus();
                        return;
                    }
                    strMessage += "," + orderModel.dtSpecialTime.Value.ToString("yyyy-MM-dd");
                }

                string strMsgOther = "";
                if (chkNTActive.IsChecked == true)
                {
                    strMsgOther += strRs_IncActive;
                }
                strMsgOther += ((chkT1.IsChecked ?? false) ? "," + CommonRsText.strRs_TOne  : ""); 
                if (strMsgOther.StartsWith(","))
                {
                    strMsgOther = strMsgOther.Substring(1);
                }
                strMsgOther = "\n\n" + strMsgOther;// ((chkT1.IsChecked ?? false) ? "" + CommonRsText.strRs_TOne + "," : "");

                strMessage += strMsgOther;
                MsgForm msgbox = new MsgForm(CommonRsText.strRs_confirm_BuyOrder_Title, strMessage,15);
                Window w = Window.GetWindow(this);
                if (w != null)
                {
                    msgbox.Owner = w;
                }            
                bool? Yes = msgbox.ShowDialog();
           
                if(Yes==true)
                {
                    TradeStationLog.WriteLogPerformance(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")+" normal buy:" + strMessage);
                    if (GOSTradeStation.msgChannel == null)
                    {
                       // MessageBox.Show("order has not send");
                        NomalTbPriceFocus();
                        return;
                    }
                    if (GOSTradeStation.isDealer)
                    {
                        GOSTradeStation.msgChannel.Send(TradeStationComm.MsgRequest.getMsgAddOrder(GOSTradeStation.UserID, GOSTradeStation.Pwd, strAccountID,
                            new TradeStationComm.MsgRequest.OrderClass.AddOrderClass(this.ProdCode,
                                TradeStationComm.Attribute.BuySell.buy,
                                orderModel.Count,
                                (this.chkAO.IsChecked ?? false) ? 0 : intOrderPrice,
                                orderModel.Valid,
                                condType,
                                orderModel.StopType,
                                orderModel.StopPrice,
                                Status,
                                specTime,
                                TradeStationComm.Attribute.AE.AE,
                                strAccountID,
                                orderModel.Ref,
                                orderModel.TPlus
                                )
                           ));
                        TradeStationSend.Send(cmdClient.getOrderBookInfo, this.CurrentID);
                        TradeStationSend.Send(cmdClient.getAccountInfo, this.CurrentID);
                        //kenlo20150312 TradeStationSend.Send(cmdClient.getPositionInfo, this.CurrentID);
                       if (TradeStationSend.ExistInContainer(WindowTypes.TradeConfirmation))
                        {
                            TradeStationSend.Send(cmdClient.getTradeConfOrders);
                        }
                    }
                    else
                    {
                        GOSTradeStation.msgChannel.Send(TradeStationComm.MsgRequest.getMsgAddOrder(GOSTradeStation.UserID, 
                            GOSTradeStation.Pwd, 
                            strAccountID,
                                                    new TradeStationComm.MsgRequest.OrderClass.AddOrderClass(this.ProdCode, 
                                                            TradeStationComm.Attribute.BuySell.buy,
                                                            orderModel.Count,
                                                            (this.chkAO.IsChecked ?? false) ? 0 : intOrderPrice,
                                                            orderModel.Valid,
                                                           condType,
                                                            orderModel.StopType,
                                                            orderModel.StopPrice, 
                                                            Status,
                                                            specTime, 
                                                            TradeStationComm.Attribute.AE.normalUser,
                                                            "",
                                                            orderModel.Ref,
                                                            orderModel.TPlus
                                                            )
                                                  ));
                        TradeStationSend.Send(cmdClient.getOrderBookInfo);
                        TradeStationSend.Send(cmdClient.getAccountInfo);
                        //kenlo20150312 TradeStationSend.Send(cmdClient.getPositionInfo);
                    }
                        // orderChild.Close();
                }
            }
            catch (Exception)
            {

                MessageBox.Show(CommonRsText.strRs_BuyOrder_Fail);//"Bought failed");
            }
            NomalTbPriceFocus();

        }

        decimal convertPrice(string strPrice)
        {
            if (strPrice.EndsWith("."))
            {
                strPrice += "0";
            }
            return Convert.ToDecimal(strPrice);
        }

        bool ValidBS()
        {            
            string strQty = this.txt_Count_NT.Text.Trim();
            if (strQty == "")
            {
                MessageBox.Show(CommonRsText.strRs_Order_Qty_Pls);
                return false;
            }
            if (!System.Text.RegularExpressions.Regex.IsMatch(strQty,@"^\d+$"))
            {
                MessageBox.Show(CommonRsText.strRs_Order_Qty_Invalid);
                return false;
            }
            ProductBoundInfo BoundInfo = GosBzTool.getProdBoundInfo(this.ProdCode);
            int intQty=Convert.ToInt32(strQty);
            if (intQty <= 0)
            {
                MessageBox.Show(CommonRsText.strRs_Order_Qty_Invalid);
                return false;
            }
            if (BoundInfo.IntQtyUp.HasValue)
            {
                if(BoundInfo.IntQtyUp.Value<intQty)
                {
                    MessageBox.Show(string.Format(CommonRsText.strRs_Qty_UP_Limit, BoundInfo.IntQtyUp.Value.ToString()) ); //"qty exceeded Upper limit " + BoundInfo.IntQtyUp.Value.ToString());
                    return false;
                }
            }
            if (BoundInfo.IntQtyLower.HasValue)
            {
                if (BoundInfo.IntQtyLower.Value > intQty)
                {
                    string _msg = string.Format(CommonRsText.strRs_Qty_ShdNot_LessThan_LowLimit, BoundInfo.IntQtyLower.Value.ToString());
                    MessageBox.Show(_msg);//"qty should be less than lower limit " + BoundInfo.IntQtyLower.Value.ToString());
                    return false;
                }
            }
            if (GosBzTool.CheckDeviationQty(intQty) == false)
            {
                return false;
            }

            string strPrice = this.txt_Price_NT.Text.Trim();
            decimal d = 0.0M;
            if (this.chkAO.IsChecked != true)
            {
                if (strPrice == "")
                {
                    MessageBox.Show(CommonRsText.StrRs_Order_Pls_Price);// "Please input price");
                    return false;
                }
                
                try
                {
                    d=convertPrice(strPrice);                 
                }
                catch
                {
                    MessageBox.Show(CommonRsText.StrRs_Order_Invalid_Price);//"Invalid price");
                    return false;
                }

                if (BoundInfo.DecPriceUp.HasValue)
                {
                    if (BoundInfo.DecPriceUp.Value < d)
                    {
                        string _msg = string.Format(CommonRsText.strRs_Price_Shlnot_ULimit, BoundInfo.DecPriceUp.Value.ToString());//
                        MessageBox.Show(_msg);//"price should not exceeded upper limit " + BoundInfo.DecPriceUp.Value.ToString());
                        return false;
                    }
                }

                if (BoundInfo.DecPriceLower.HasValue)
                {
                    if (BoundInfo.DecPriceLower.Value > d)
                    {
                        string _msg = string.Format(CommonRsText.strRs_Price_Shlnot_LowLimit, BoundInfo.DecPriceLower.Value.ToString());//
                        MessageBox.Show(_msg);//"price should not less than lower limit " + BoundInfo.DecPriceLower.Value.ToString());
                        return false;
                    }
                }

                if (GosBzTool.CheckDeviationPrice(d, CurrentProdMktItem) == false)
                {
                    return false;
                }

                decimal decInt = Math.Truncate(d);
                if ((d - decInt) > 0)
                {
                    int l = GosBzTool.getDecLen(txt_OrderID.Text.Trim());
                    int pos = strPrice.IndexOf('.');
                    if (pos > 0)
                    {
                        if (l == 0)
                        {
                            string _msg = string.Format(CommonRsText.strRs_input_price_no_allow_dec, txt_OrderID.Text.Trim());
                            MessageBox.Show(_msg);// "no decimal part allowed for product '" + txt_OrderID.Text.Trim() + "' ");
                            return false;
                        }
                        int decLen = strPrice.Length - pos - 1;
                        if (decLen > l)
                        {
                            string _msg=string.Format(CommonRsText.strRs_input_priceDec_only_sLen_allowed, l.ToString(), txt_OrderID.Text.Trim());
                            MessageBox.Show(_msg);//"only " + l.ToString() + " number of decimal part allowed for product '" + txt_OrderID.Text.Trim() + "'");
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        private void btn_NTsell_Click(object sender, RoutedEventArgs e)
        {
            if (GOSTradeStation.isDealer)
            {
                if (CurrentID == null || CurrentID.Trim() == "")
                {
                    MessageBox.Show(CommonRsText.StrRs_input_Acc_pls);//"please selected an account");
                    ctrlAccFocus();
                    return;
                }
            }
            if (this.ProdCode == null || this.ProdCode.Trim() == "")
            {
                MessageBox.Show(CommonRsText.StrRs_input_prod_pls);// "please selected a product");
                tbProdFocus();
                return;
            }
            if (!ValidBS())
            {
                NomalTbPriceFocus();
                return;
            }
            
            // Sell;
           
            if (CanTrade() == false)
            {
                NomalTbPriceFocus();
                return;
            }

            if (GOSTradeStation.isDealer == true && this.CurrentID != this.cbbUsers.cbbUsers.Text.Trim())
            {
                string _msg = string.Format(CommonRsText.strRs_Order_AccNotEqual, this.cbbUsers.cbbUsers.Text.Trim(), this.CurrentID);
               // if (MessageBox.Show("'" + this.cbbUsers.cbbUsers.Text.Trim() + "' not equal current acc '" + this.CurrentID + "',continue to place order with current acc '" + this.CurrentID + "?  ", "Buy Confirm ", MessageBoxButton.OKCancel) != MessageBoxResult.OK)
                if (MessageBox.Show(_msg,CommonRsText.strRs_confirm_SellOrder_Title, MessageBoxButton.OKCancel) != MessageBoxResult.OK)
                {
                    ctrlAccFocus();
                    return;
                }
            }
            try
            {
                decimal OrderPrice = get_txt_Price_NT();
                int intOrderPrice = GosBzTool.ChangeDecToIn(this.ProdCode, OrderPrice);
                strAccountID = CurrentID;
                //string strMessage = string.Format("代號：{0}\n帳號：{1}\n價格：{2}\n數量：{3}", str_OrderID, GOSTradeStation.AccountID, Num_Price_Normal, Num_Count_Normal);
                string strMessage = string.Format(CommonRsText.strRs_confirm_SellOrder,// "Account：{1}\nCmd:Sell\nID：{0}\nPrice：{2}\nQty：{3}\nValidity:{4}",
                   strAccountID,
                    this.ProdCode, 
                    (this.chkAO.IsChecked ?? false) ? "AO" : OrderPrice.ToString(), 
                    orderModel.Count,
                    CommonRsText.getValidTypeText( orderModel.Valid) );
                long specTime = 0;

                if (orderModel.Valid == TradeStationComm.Attribute.ValidType.specTime)
                {
                    if (orderModel.dtSpecialTime.HasValue)
                    {
                        specTime = Utility.ConvertToUnixTime(orderModel.dtSpecialTime.Value);
                    }
                    if (specTime == 0)
                    {
                        MessageBox.Show(CommonRsText.strRs_Input_order_specDate_pls);
                        return;
                    }
                    strMessage += "," + orderModel.dtSpecialTime.Value.ToString("yyyy-MM-dd");
                }

                string strMsgOther = "";
                if (chkNTActive.IsChecked == true)
                {
                    strMsgOther += strRs_IncActive;
                }
                strMsgOther+= ((chkT1.IsChecked ?? false) ? "," + CommonRsText.strRs_TOne + "" : "");
                if(strMsgOther.StartsWith(","))
                {
                    strMsgOther=strMsgOther.Substring(1);
                }
                strMsgOther = "\n\n" + strMsgOther;

                strMessage += strMsgOther;

                TradeStationComm.Attribute.Active Status = orderModel.Active == 1 ? TradeStationComm.Attribute.Active.inactive : TradeStationComm.Attribute.Active.active;
                TradeStationComm.Attribute.CondType condType = (this.chkAO.IsChecked ?? false) ? TradeStationComm.Attribute.CondType.AO : TradeStationComm.Attribute.CondType.normal;

                MsgForm msgbox = new MsgForm(CommonRsText.strRs_confirm_SellOrder_Title, strMessage, 15);
                Window w = Window.GetWindow(this);
                if (w != null)
                {
                    msgbox.Owner = w;
                }                          
                bool? Yes = msgbox.ShowDialog();
              
                if (GOSTradeStation.msgChannel == null)
                {
                    // MessageBox.Show("order has not send");
                    NomalTbPriceFocus();
                    return;
                }

                if(Yes==true)
                {
                    TradeStationLog.WriteLogPerformance(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")+" normal sell:" + strMessage);
                    if (GOSTradeStation.isDealer)
                    {
                        GOSTradeStation.msgChannel.Send(TradeStationComm.MsgRequest.getMsgAddOrder(GOSTradeStation.UserID,
                            GOSTradeStation.Pwd,
                            strAccountID,
                            new TradeStationComm.MsgRequest.OrderClass.AddOrderClass(this.ProdCode,
                                TradeStationComm.Attribute.BuySell.sell,
                                orderModel.Count,
                                (this.chkAO.IsChecked ?? false) ? 0 : intOrderPrice,
                                orderModel.Valid,
                                condType,
                                orderModel.StopType,
                                orderModel.StopPrice,
                                Status,
                                specTime,
                                TradeStationComm.Attribute.AE.AE,
                                strAccountID,
                                orderModel.Ref,
                                orderModel.TPlus
                                )
                        ));
                        TradeStationSend.Send(cmdClient.getOrderBookInfo, this.CurrentID);
                        TradeStationSend.Send(cmdClient.getAccountInfo, this.CurrentID);
                        //kenlo20150312 TradeStationSend.Send(cmdClient.getPositionInfo, this.CurrentID);
                       if (TradeStationSend.ExistInContainer(WindowTypes.TradeConfirmation))
                        {
                            TradeStationSend.Send(cmdClient.getTradeConfOrders);
                        }
                    }
                    else
                    {
                        GOSTradeStation.msgChannel.Send(TradeStationComm.MsgRequest.getMsgAddOrder(GOSTradeStation.UserID, GOSTradeStation.Pwd, strAccountID,
                            new TradeStationComm.MsgRequest.OrderClass.AddOrderClass(this.ProdCode,
                                TradeStationComm.Attribute.BuySell.sell,
                                orderModel.Count,
                                (this.chkAO.IsChecked ?? false) ? 0 : intOrderPrice,
                                orderModel.Valid,
                                condType,
                                orderModel.StopType,
                                orderModel.StopPrice,
                                Status,
                                specTime,
                                TradeStationComm.Attribute.AE.normalUser,
                                "",
                                orderModel.Ref,
                                orderModel.TPlus)));
                        TradeStationSend.Send(cmdClient.getOrderBookInfo);
                        TradeStationSend.Send(cmdClient.getAccountInfo);
                        //kenlo20150312 TradeStationSend.Send(cmdClient.getPositionInfo);
                    }
                    // orderChild.Close();
                }
            }
            catch (Exception)
            {
                MessageBox.Show(CommonRsText.strRs_SellOrder_Fail);
            }
            NomalTbPriceFocus();
        }

        protected void distributeMsg_AddOrderInfo(object sender, String str)
        {
            Application.Current.Dispatcher.Invoke(DisOrderConfirmDelegate, new object[] { str });
        }

        protected void distributeMsg_GetNotification(object sender, String str)
        {
            Application.Current.Dispatcher.Invoke(DisOrderConfirmDelegate, new object[] { str });
            if (GOSTradeStation.boolWindowOrderBook == false)
            {
                Application.Current.Dispatcher.BeginInvoke(this.NotificationDelegate, new Object[] { str });
                GOSTradeStation.boolWindowOrderBook = true;
            }
            else
            {
               // GOSTradeStation.msgChannel.Send(TradeStationComm.MsgRequest.getMsgGetOrderBookInfo(GOSTradeStation.UserID));
                if (GOSTradeStation.msgChannel != null)
                {
                    GOSTradeStation.msgChannel.Send(TradeStationComm.MsgRequest.getMsgGetOrderBookInfo(GOSTradeStation.UserID, this.CurrentID));
                }
                GOSTradeStation.boolWindowOrderBook = true;
            }

        }

        public void NotificationItemMethod(string str)
        {
            if (str != null)
            {
                MdiContainer Container = App.Current.MainWindow.FindName("Container") as MdiContainer;

                if (Container != null)
                {
                    
                    MdiChild OrderWPF = new MdiChild();
                    UserOrderInfo ui=new UserOrderInfo(distributeMsg,OrderWPF);
                    OrderWPF.Content=ui;
                    
                       OrderWPF. Content =OrderWPF;// new UserOrderInfo(distributeMsg),
                       OrderWPF. Width = 820;
                       OrderWPF.Height = 640;
                       OrderWPF. Position = new System.Windows.Point(60, 60);
                   
                    Container.Children.Add(OrderWPF);
                }
            }


        }

  

        private void btn_Price_NT_down_Click(object sender, RoutedEventArgs e)
        {
            if (orderModel.Price <= 0)
            {
                return;
            }
            orderModel.Price -= 1;
        }

        private void btn_Price_NT_up_Click(object sender, RoutedEventArgs e)
        {           
            orderModel.Price += 1;
        }

        private void txt_OrderID_TextChanged(object sender, TextChangedEventArgs e)
        { }


        public void OpenOrderConfrim(string str)
        {
            MdiContainer Container = App.Current.MainWindow.FindName("Container") as MdiContainer;
            MdiChild orderChild = new MdiChild();
            OrderConfirm OrderConfirm = new OrderConfirm(orderChild);
            OrderConfirm.lbl_Message.Text = str;

            orderChild.Title = "Order Confirm";
            orderChild.Content = OrderConfirm;
            orderChild.Width = OrderConfirm.Width;
            orderChild.Height = OrderConfirm.Height;
            orderChild.Position = new System.Windows.Point(0, 0);
            Container.Children.Add(orderChild);
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            if (this.ProdCode != null && this.ProdCode != "")
            {
                TradeStationSend.Send(this.ProdCode, null, cmdClient.registerMarketPrice);
            }
            distributeMsg.DisPMPriceDepthData -= new MessageDistribute.OnDisPMPriceDepthData(distributeMsg_MPPriceDepthObs);
            distributeMsg.DisPMPriceDepth -= new MessageDistribute.OnDisPMPriceDepth(distributeMsg_MPPriceDepth);
            distributeMsg.DisPMTickerItemData -= new MessageDistribute.OnDisPMTickerItemData(distributeMsg_MPTickerData);
            distributeMsg.DisMarketPrice -= new MessageDistribute.OnDisMarketPrice(distributeMsg_DisMarketPrice);

            this.NotificationDelegate -= new NotificationItem(NotificationItemMethod);
            distributeMsg.DisEnterOrderControl -= distributeMsg_DisEnterOrderControl;

            distributeMsg.DisControlOrderItem -= new MessageDistribute.OnDisControlOrderItem(distributeMsg_DisControlOrderItem);

            base.IDelearUser_Unloaded_1(sender, e);
        }

        void SetContext()
        {
            //ValidTypeList validls = new ValidTypeList();
            //CbbValid.ItemsSource = validls;            
         
            cbbstop.SelectedIndex = 0;
            this.DataContext = orderModel;
            orderModel.Valid = TradeStationComm.Attribute.ValidType.today;
            this.dt_CT_GN_SpecTime.IsEnabled = false;
            this.orderModel.dtSpecialTime = null;

            BindControlEven();
            BindGeneralContrlEvent();
            this.tbQuickTrade.DataContext = quickOrderModel;
            txt_Price_NT.SetMdiChild(orderChild);
        }

        void BindControlEven()
        {
            tabc.SelectionChanged += tabc_SelectionChanged;
           // this.cbbstop.SelectionChanged += cbbstop_SelectionChanged;
            this.CbbValid.SelectionChanged += CbbValid_SelectionChanged;
            this.ntValid.SelectionChanged += ntValid_SelectionChanged;
            chkT1.Checked += chkT1_Checked;
            chkT1.Unchecked += chkT1_Unchecked;
            this.txt_OrderID.LostKeyboardFocus += txt_OrderID_LostKeyboardFocus;
            
        }      

        double BkWidth = 0;
        double BKHeight = 0;
        double Tab_GT_MinWidth = 390, Tab_GT_MinHeight=300;
        int last_Tab_Index = 0;
        void tabc_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.tabc == null) return;
            if (e.OriginalSource != tabc)
            {
                return;
            }
            int i = this.tabc.SelectedIndex;
            if (i == 2)
            {
                if (tabc.Items[2] != null)
                {
                    BkWidth=orderChild.Width;
                    BKHeight=orderChild.Height;
                    if (Tab_GT_MinWidth > orderChild.Width)
                    {
                        orderChild.Width = Tab_GT_MinWidth;
                    }
                    if (Tab_GT_MinHeight > orderChild.Height)
                    {
                        orderChild.Height = Tab_GT_MinHeight;
                    }
                }
            }
            else{
                if (last_Tab_Index==2)
                {
                    if (BkWidth != 0)
                    {
                        orderChild.Width = BkWidth;
                    }
                    if (BKHeight != 0)
                    {
                        orderChild.Height = BKHeight;
                    }
                }
                //else
                //{
                //    BkWidth = orderChild.Width;
                //    BKHeight = orderChild.Height;
                //}             
            }
            last_Tab_Index = i;
        }

        

        void CbbValid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem ctValidType = CbbValid.SelectedItem as ComboBoxItem;
            TradeStationComm.Attribute.ValidType valid = TradeStationComm.Attribute.ValidType.today;
            if (ctValidType != null)
            {
                bool b = false;
                try
                {
                    b = Enum.TryParse<TradeStationComm.Attribute.ValidType>(ctValidType.Tag.ToString(), true, out valid);
                }
                catch { }                
            }
            if (valid != TradeStationComm.Attribute.ValidType.specTime)// && valid != TradeStationComm.Attribute.ValidType.GTC)
            {
                this.dt_CT_GN_SpecTime.IsEnabled = false;               
            }
            else
            {
                this.dt_CT_GN_SpecTime.IsEnabled = true;
            }
        }

        void ntValid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ntValid == null) return;
            ComboBoxItem ntValidType = ntValid.SelectedItem as ComboBoxItem;
            TradeStationComm.Attribute.ValidType valid = TradeStationComm.Attribute.ValidType.today;
            if (ntValidType != null)
            {
                bool b = Enum.TryParse<TradeStationComm.Attribute.ValidType>(ntValidType.Tag.ToString(), true, out valid);
            }
            orderModel.Valid = valid;
            if (orderModel.Valid != TradeStationComm.Attribute.ValidType.specTime)// && orderModel.Valid != TradeStationComm.Attribute.ValidType.GTC)
            {
                this.ntSecTime.IsEnabled = false;
                this.orderModel.dtSpecialTime = null;
            }
            else
            {
                this.ntSecTime.IsEnabled = true;
            }
        }

        void chkT1_Unchecked(object sender, RoutedEventArgs e)
        {
            orderModel.TPlus = TradeStationComm.Attribute.TOne.TOnly;
        }

        void chkT1_Checked(object sender, RoutedEventArgs e)
        {
            orderModel.TPlus = TradeStationComm.Attribute.TOne.TPluseOne;
        }

       

        bool ValidQuickBS(string BS)
        {
            string strTicks = this.txtQTradeTicks.Text.Trim();
            if (strTicks != "")
            {
                if (!System.Text.RegularExpressions.Regex.IsMatch(strTicks, @"^\d+(\.\d+){0,1}$"))
                {
                    MessageBox.Show(CommonRsText.StrRs_input_ticks_invalid);
                    return false;
                }
            }
            decimal d = Convert.ToDecimal(strTicks);
            int l = GosBzTool.getDecLen(txt_OrderID.Text.Trim());
            int pos = strTicks.IndexOf('.');
            if (pos > 0)
            {
                decimal decInt = Math.Truncate(d);
                if (d - decInt > 0)
                {
                    if (l == 0)
                    {
                        string _msg = string.Format(CommonRsText.strRs_input_price_no_allow_dec, txt_OrderID.Text.Trim());
                        MessageBox.Show(_msg);
                        return false;
                    }
                    int decLen = strTicks.Length - pos - 1;
                    if (decLen > l)
                    {
                        string _msg = string.Format(CommonRsText.strRs_input_priceDec_only_sLen_allowed, l.ToString(), txt_OrderID.Text.Trim());
                        MessageBox.Show(_msg);
                        return false;
                    }
                }
            }

            string strQty = this.txtQTradeCount.Text.Trim(); 
            if (strQty == "")
            {
                MessageBox.Show(CommonRsText.strRs_Order_Qty_Pls);
                return false;
            }
            if (!System.Text.RegularExpressions.Regex.IsMatch(strQty, @"^\d+$"))
            {
                MessageBox.Show(CommonRsText.strRs_Order_Qty_Invalid);
                return false;
            }
            ProductBoundInfo BoundInfo = GosBzTool.getProdBoundInfo(this.ProdCode);
            int intQty = Convert.ToInt32(strQty);
            if (intQty <= 0)
            {
                MessageBox.Show(CommonRsText.strRs_Order_Qty_Invalid);
                return false;
            }
            if (BoundInfo.IntQtyUp.HasValue)
            {
                if (BoundInfo.IntQtyUp.Value < intQty)
                {
                    string _msg = string.Format(CommonRsText.strRs_Qty_UP_Limit, BoundInfo.IntQtyUp.Value.ToString());
                    MessageBox.Show(_msg);
                    return false;
                }
            }

            if (BoundInfo.IntQtyLower.HasValue)
            {
                if (BoundInfo.IntQtyLower.Value > intQty)
                {
                    string _msg = string.Format(CommonRsText.strRs_Qty_ShdNot_LessThan_LowLimit, BoundInfo.IntQtyUp.Value.ToString());
                    MessageBox.Show(_msg);//"qty should not less than lower limit " + BoundInfo.IntQtyLower.Value.ToString());
                    return false;
                }
            }

            decimal DecPrice = 0.0M;
            if (BS == "B")
            {
                DecPrice = quickOrderModel.AskTo;                
            }
            else if (BS == "S")
            {
                DecPrice = quickOrderModel.BidTo;              
            }
            if (BS.ToUpper() == "B" || BS.ToUpper() == "S")
            {
                if (BoundInfo.DecPriceUp.HasValue)
                {
                    if (BoundInfo.DecPriceUp.Value < DecPrice)
                    {
                        MessageBox.Show(string.Format( CommonRsText.strRs_Price_Shlnot_ULimit,BoundInfo.DecPriceUp.Value.ToString()));// "price should not exceeded upper limit " + BoundInfo.DecPriceUp.Value.ToString());
                        return false;
                    }
                }

                if (BoundInfo.DecPriceLower.HasValue)
                {
                    if (BoundInfo.DecPriceLower.Value > DecPrice)
                    {
                        MessageBox.Show( string.Format(CommonRsText.strRs_Price_Shlnot_LowLimit,  BoundInfo.DecPriceLower.Value.ToString()));
                        return false;
                    }
                }
            }
            
            return true;
        }

        private void btnQuickBuy_Click_1(object sender, RoutedEventArgs e)
        {
            if (this.ProdCode == null || this.ProdCode.Trim() == "")
            {
                MessageBox.Show(CommonRsText.StrRs_input_prod_pls);
                tbProdFocus();
                return;
            }
            if (CanTrade() == false)
            {
                return;
            }
            if (GOSTradeStation.isDealer)
            {
                if (CurrentID == null || CurrentID.Trim() == "")
                {
                    MessageBox.Show(CommonRsText.StrRs_input_Acc_pls); 
                    ctrlAccFocus();
                    return;
                }
            }
            if (!ValidQuickBS("B"))
            {
                txtQTradeTicksFocus();
                return;
            }
            if (GosBzTool.CheckDeviationQty((int)quickOrderModel.Count) == false)
            {
                txtQTradeTicksFocus();
                return;
            }

            if (GosBzTool.CheckDeviationPrice(quickOrderModel.AskTo, CurrentProdMktItem) == false)
            {
                txtQTradeTicksFocus();
                return ;
            }

            if (GOSTradeStation.isDealer == true && this.CurrentID != this.cbbUsers.cbbUsers.Text.Trim())
            {
                string _msg = string.Format(CommonRsText.strRs_Order_AccNotEqual, this.cbbUsers.cbbUsers.Text.Trim(), this.CurrentID);
                if (MessageBox.Show(_msg,//  "'" + this.cbbUsers.cbbUsers.Text.Trim() + "' not equal current acc '" + this.CurrentID + "',continue to place order with current acc '" + this.CurrentID + "?  ", 
                   CommonRsText.strRs_confirm_BuyOrder_Title,// "Buy Confirm ", 
                    MessageBoxButton.OKCancel) != MessageBoxResult.OK)
                {
                    txtQTradeTicksFocus();
                    return;
                }
            }
            try
            {
                strAccountID = CurrentID;
              
                // string strMessage = string.Format("代號：{0}\n帳號：{1}\n價格：{2}\n數量：{3}", str_OrderID, GOSTradeStation.AccountID, Num_Price_Normal, Num_Count_Normal);
              
                long specTime = 0;
                //if (orderModel.dtSpecialTime.HasValue)
                //{
                //    specTime = Utility.ConvertToUnixTime(orderModel.dtSpecialTime.Value);
                //}
                int intAskTo = GosBzTool.ChangeDecToIn(this.ProdCode, quickOrderModel.AskTo);
                ComboBoxItem itemQTradeValidType = cmbQuickValid.SelectedItem as ComboBoxItem;
                TradeStationComm.Attribute.ValidType valid = TradeStationComm.Attribute.ValidType.today;
                if (itemQTradeValidType != null)
                {
                    bool b = Enum.TryParse<TradeStationComm.Attribute.ValidType>(itemQTradeValidType.Tag.ToString(), true, out valid);
                }

                string strMessage = string.Format(CommonRsText.strRs_confirm_BuyOrder,// "Acc：{1}\nCmd:Buy\nID：{0}\nPrice：{2}\nQty：{3}\nValidity:{4}\n", 
                    strAccountID, 
                    this.ProdCode, 
                    this.quickOrderModel.AskTo, 
                    this.quickOrderModel.Count,
                    CommonRsText.getValidTypeText(valid) 
                    );
                strMessage += ((this.chkT1.IsChecked ?? false) ? "\n\n" + CommonRsText.strRs_TOne : "");
                MsgForm msgbox = new MsgForm(CommonRsText.strRs_confirm_BuyOrder_Title, strMessage, 15);
                Window w = Window.GetWindow(this);
                if (w != null)
                {
                    msgbox.Owner = w;
                }
              
                bool? Yes = msgbox.ShowDialog(); 
               // if (MessageBox.Show(strMessage, "Buy Confirm ", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                if(Yes==true)
                {
                    if (GOSTradeStation.msgChannel == null)
                    {                     
                        txtQTradeTicksFocus();
                        return;
                    }
                    TradeStationLog.WriteLogPerformance(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")+" quick buy:" + strMessage);
                    if (GOSTradeStation.isDealer)
                    {
                        GOSTradeStation.msgChannel.Send(TradeStationComm.MsgRequest.getMsgAddOrder(GOSTradeStation.UserID, GOSTradeStation.Pwd, strAccountID,
                            new TradeStationComm.MsgRequest.OrderClass.AddOrderClass(this.ProdCode,
                                TradeStationComm.Attribute.BuySell.buy,
                                quickOrderModel.Count,
                               intAskTo,
                                valid,
                                TradeStationComm.Attribute.CondType.normal,
                                orderModel.StopType,
                                orderModel.StopPrice,
                                TradeStationComm.Attribute.Active.active,
                                specTime,
                                TradeStationComm.Attribute.AE.AE,
                                strAccountID,
                                orderModel.Ref,
                                orderModel.TPlus
                                )
                           ));
                        TradeStationSend.Send(cmdClient.getOrderBookInfo, strAccountID);
                        TradeStationSend.Send(cmdClient.getAccountInfo, strAccountID);
                        //kenlo20150312 TradeStationSend.Send(cmdClient.getPositionInfo, strAccountID);
                       if (TradeStationSend.ExistInContainer(WindowTypes.TradeConfirmation))
                        {
                            TradeStationSend.Send(cmdClient.getTradeConfOrders);
                        }
                    }
                    else
                    {
                        GOSTradeStation.msgChannel.Send(TradeStationComm.MsgRequest.getMsgAddOrder(GOSTradeStation.UserID,
                            GOSTradeStation.Pwd,
                            strAccountID,
                                                    new TradeStationComm.MsgRequest.OrderClass.AddOrderClass(this.ProdCode,
                                                            TradeStationComm.Attribute.BuySell.buy,
                                                            quickOrderModel.Count,
                                                           intAskTo,
                                                            valid,
                                                            TradeStationComm.Attribute.CondType.normal,
                                                            orderModel.StopType,
                                                            orderModel.StopPrice,
                                                            TradeStationComm.Attribute.Active.active,
                                                            specTime,
                                                            TradeStationComm.Attribute.AE.normalUser,
                                                            "",
                                                            orderModel.Ref,
                                                            orderModel.TPlus
                                                            )
                                                  ));

                        TradeStationSend.Send(cmdClient.getOrderBookInfo);
                        TradeStationSend.Send(cmdClient.getAccountInfo);
                        //kenlo20150312 TradeStationSend.Send(cmdClient.getPositionInfo);
                    }
                    // orderChild.Close();
                }
            }
            catch (Exception)
            {
                
                MessageBox.Show(CommonRsText.strRs_BuyOrder_Fail);
            }
            txtQTradeTicksFocus();
        }

        private void btnQTradeSell_Click_1(object sender, RoutedEventArgs e)
        {
            if (this.ProdCode == null || this.ProdCode.Trim() == "")
            {
                MessageBox.Show(CommonRsText.StrRs_input_prod_pls);
                tbProdFocus();
                return;
            }
            if (GOSTradeStation.isDealer)
            {
                if (CurrentID == null || CurrentID.Trim() == "")
                {
                    ctrlAccFocus();
                    MessageBox.Show(CommonRsText.StrRs_input_Acc_pls);
                    return;
                }
            }
            if (CanTrade() == false)
            {
                txtQTradeTicksFocus();
                return;
            }
            if (!ValidQuickBS("S"))
            {
                txtQTradeTicksFocus();
                return;
            }

            if (GosBzTool.CheckDeviationQty((int)quickOrderModel.Count) == false)
            {
                txtQTradeTicksFocus();
                return;
            }
            if (GosBzTool.CheckDeviationPrice(quickOrderModel.BidTo, CurrentProdMktItem) == false)
            {
                txtQTradeTicksFocus();
                return;
            }

            if (GOSTradeStation.isDealer == true && this.CurrentID != this.cbbUsers.cbbUsers.Text.Trim())
            {
                string _msg = string.Format(CommonRsText.strRs_Order_AccNotEqual, this.cbbUsers.cbbUsers.Text.Trim(), this.CurrentID);
                if (MessageBox.Show(_msg,//"'" + this.cbbUsers.cbbUsers.Text.Trim() + "' not equal current acc '" + this.CurrentID + "',continue to place order with current acc '" + this.CurrentID + "?  "
                    CommonRsText.strRs_confirm_BuyOrder_Title,// "Buy Confirm ", 
                    MessageBoxButton.OKCancel) != MessageBoxResult.OK)
                {
                    ctrlAccFocus();
                    return;
                }
            }

            try
            {
                strAccountID = CurrentID;               
                long specTime = 0;

                int intBidTo = GosBzTool.ChangeDecToIn(this.ProdCode, quickOrderModel.BidTo);
                ComboBoxItem itemQTradeValidType = cmbQuickValid.SelectedItem as ComboBoxItem;
                TradeStationComm.Attribute.ValidType valid = TradeStationComm.Attribute.ValidType.today;
                if (itemQTradeValidType != null)
                {
                    bool b = Enum.TryParse<TradeStationComm.Attribute.ValidType>(itemQTradeValidType.Tag.ToString(), true, out valid);
                }
                string strMessage = string.Format(CommonRsText.strRs_confirm_SellOrder,// "Acc：{1}\nID：{0}\nPrice：{2}\nQty：{3}\nValidity:{4}", 
                     strAccountID, 
                     this.ProdCode,
                    quickOrderModel.BidTo, 
                    quickOrderModel.Count,
                    CommonRsText.getValidTypeText(valid) 
                   );

                strMessage += ((this.chkT1.IsChecked ?? false) ? "\n\n" + CommonRsText.strRs_TOne : "");
                MsgForm msgbox = new MsgForm(CommonRsText.strRs_confirm_SellOrder_Title, strMessage, 15);
                Window w = Window.GetWindow(this);
                if (w != null)
                {
                    msgbox.Owner = w;
                }
                bool? Yes = msgbox.ShowDialog(); 
              
                if(Yes==true)
                {
                    TradeStationLog.WriteLogPerformance(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")+" quick sell:" + strMessage);
                    if (GOSTradeStation.isDealer)
                    {
                        GOSTradeStation.msgChannel.Send(TradeStationComm.MsgRequest.getMsgAddOrder(GOSTradeStation.UserID,
                            GOSTradeStation.Pwd,
                            strAccountID,
                            new TradeStationComm.MsgRequest.OrderClass.AddOrderClass(this.ProdCode,
                                TradeStationComm.Attribute.BuySell.sell,
                                quickOrderModel.Count,
                                intBidTo ,//quickOrderModel.BidTo,
                                valid,
                                TradeStationComm.Attribute.CondType.normal,
                                orderModel.StopType,
                                orderModel.StopPrice,
                                TradeStationComm.Attribute.Active.active,
                                specTime,
                                TradeStationComm.Attribute.AE.AE,
                                strAccountID,
                                orderModel.Ref,
                                orderModel.TPlus
                                )
                        ));
                        TradeStationSend.Send(cmdClient.getOrderBookInfo, CurrentID);
                        TradeStationSend.Send(cmdClient.getAccountInfo, CurrentID);
                        //kenlo20150312 TradeStationSend.Send(cmdClient.getPositionInfo, CurrentID);
                       if (TradeStationSend.ExistInContainer(WindowTypes.TradeConfirmation))
                        {
                            TradeStationSend.Send(cmdClient.getTradeConfOrders);
                        }
                    }
                    else
                    {
                        GOSTradeStation.msgChannel.Send(TradeStationComm.MsgRequest.getMsgAddOrder(GOSTradeStation.UserID, GOSTradeStation.Pwd, strAccountID,
                            new TradeStationComm.MsgRequest.OrderClass.AddOrderClass(this.ProdCode,
                                TradeStationComm.Attribute.BuySell.sell,
                                quickOrderModel.Count,
                                intBidTo,
                                valid,
                                TradeStationComm.Attribute.CondType.normal,
                                orderModel.StopType,
                                orderModel.StopPrice,
                                TradeStationComm.Attribute.Active.active,
                                specTime,
                                TradeStationComm.Attribute.AE.normalUser,
                                "",
                                orderModel.Ref,
                                orderModel.TPlus)));

                        TradeStationSend.Send(cmdClient.getOrderBookInfo);
                        TradeStationSend.Send(cmdClient.getAccountInfo);
                        //kenlo20150312 TradeStationSend.Send(cmdClient.getPositionInfo);
                    }
                    // orderChild.Close();
                }
            }
            catch (Exception)
            {
                MessageBox.Show(CommonRsText.strRs_SellOrder_Fail);
            }
            txtQTradeTicksFocus();
        }
        
       
        int decInPrice = 0;
        decimal getProdPrice(int decLen, decimal ReceivePrice)
        {
            if (decLen == 0) return ReceivePrice;
            decimal a = 123;
            double bottomNum = 10;
            double divisor = Math.Pow(bottomNum, (double)decLen);
            decimal result = a / (decimal)divisor;
            return result;
        }        

        public void Set_Price_TextBox(string strDecimal,string _ProdCode = "", bool? isAO=null)
        {
            if (strDecimal == null)
            {
                strDecimal = "0";
            }
            //set CheckBox AO
            if (isAO.HasValue)
            {
                chkAO.IsChecked = isAO;
                ChkCTGeneralAO.IsChecked = isAO;
            }
            else if (strDecimal.ToUpper().Trim() == "AO")
            {
                chkAO.IsChecked = true;
                ChkCTGeneralAO.IsChecked = true;
            }
            else
            {
                ChkCTGeneralAO.IsChecked = false;
                chkAO.IsChecked = false;
            }

           
            if (chkAO.IsChecked ?? false)
            {
                txt_Price_NT.Text = "0";               
            }

            decimal d = 0;
            decimal decPrice = 0;
            if ((chkAO.IsChecked ?? false)==false)
            {
                d = Utility.ConvertToDecimal(strDecimal, 0);
                decPrice = GosBzTool.adjustDecLength(this.ProdCode, d);
                txt_Price_NT.Text = decPrice.ToString();   
            }           

            if (ChkCTGeneralAO.IsChecked ?? false)
            {
                this.txt_Price_CT.Text = "0";
            }
            else
            {
                txt_Price_CT.Text = GosBzTool.adjustDecLength(this.ProdCode, d).ToString();
                txt_StopPrice_CT.Text = GosBzTool.adjustDecLength(this.ProdCode, d).ToString();
                this.txt_AttractPrice_OCO.Text = decPrice.ToString();
                this.txt_StopPrice_OCO.Text = decPrice.ToString();
            }
        }

        public decimal get_txt_Price_NT()
        {
            return txt_Price_NT.GetInputPrice(chkAO.IsChecked);            
        }

        public decimal get_txt_Price_CT()
        {
            return txt_Price_CT.GetInputPrice(ChkCTGeneralAO.IsChecked);            
        }

        public decimal get_txt_StopPrice_CT()
        {
            return txt_StopPrice_CT.GetInputPrice(ChkCTGeneralAO.IsChecked);           
        }

        void SetPrice(float  price,string _prodCode,bool _isAo)
        {
            if (_isAo)//|| price ==-99999) 
            {
               // SetAO(true);
                Set_Price_TextBox("0", _prodCode,true);
            }
            else
            {
               // SetAO(false);
                Set_Price_TextBox(price.ToString(), _prodCode,false);
            }
        }
       
#region Ao Deal   
  
        //void SetAO(bool bCheck)
        //{
        //    this.chkAO.IsChecked = bCheck;
        //    this.ChkCTGeneralAO.IsChecked = bCheck;
        //}

        void BindGeneralContrlEvent()
        {
            this.ChkCTGeneralAO.Checked += ChkCTGeneralAO_Checked;
            this.ChkCTGeneralAO.Unchecked += ChkCTGeneralAO_Unchecked;
        }

        void ChkCTGeneralAO_Unchecked(object sender, RoutedEventArgs e)
        {
            switchStatusByCTGeneralAO();
        }

        void ChkCTGeneralAO_Checked(object sender, RoutedEventArgs e)
        {
            switchStatusByCTGeneralAO();
        }


        private void chkAO_Checked(object sender, RoutedEventArgs e)
        {
            ChangePriceEnableStatus();
        }

        private void chkAO_Unchecked(object sender, RoutedEventArgs e)
        {
            ChangePriceEnableStatus();
        }

        void ChangePriceEnableStatus()
        {
            if (this.chkAO.IsChecked ?? false == true)
            {
                this.txt_Price_NT.Text = "0";
                this.txt_Price_NT.IsEnabled = false;
                btnPrAdd.IsEnabled = false;
                btnPrDow.IsEnabled = false;
            }
            else
            {
                btnPrAdd.IsEnabled = true;
                btnPrDow.IsEnabled = true;
                this.txt_Price_NT.IsEnabled = true;
            }
        }

        void switchStatusByCTGeneralAO()
        {
            if (this.ChkCTGeneralAO.IsChecked ?? false == true)
            {
                this.txt_Price_CT.Text = "0";
                this.txt_Price_CT.IsEnabled = false;
                btn_Price_CT_up.IsEnabled = false;
                btn_Price_CT_down.IsEnabled = false;
            }
            else
            {
                btn_Price_CT_up.IsEnabled = true;
                btn_Price_CT_down.IsEnabled = true;
                this.txt_Price_CT.IsEnabled = true;
            }
        }
#endregion
        
        private void tbTick_KeyUp(object sender, KeyEventArgs e)
        {
            string str = this.txtQTradeTicks.Text.Trim();
            try
            {
                Convert.ToDecimal(str);
            }
            catch
            {
                int i = this.txtQTradeTicks.SelectionStart;
                if (i > str.Length)
                {
                    i = str.Length;
                }
                int curPo = 0;
                if (i > 0) //calculate right cursor position should be
                {
                    string strToCursor = str.Substring(0, i);
                    strToCursor = System.Text.RegularExpressions.Regex.Replace(strToCursor, @"[^\d\.]", "");
                    curPo = strToCursor.Length;
                }
                str = System.Text.RegularExpressions.Regex.Replace(str, @"[^\d\.]", "");
                this.txtQTradeTicks.Text = str;
                this.txtQTradeTicks.SelectionStart = curPo;
                return;
            }
            int l = GosBzTool.getDecLen(this.ProdCode);
            int pos = str.IndexOf('.');
            if (pos > 0)
            {
                int decLen = str.Length - pos - 1;
                if (decLen > l)
                {
                    str = str.Substring(0, str.Length - (decLen - l));
                    this.txtQTradeTicks.Text = str;
                    this.txtQTradeTicks.SelectionStart = str.Length;
                }
            }

            //  e.Handled = true;
        }
        
        bool ValidCTGeneralBuy(string BS)
        {
            string temp = this.txt_Count_CT.Text.Trim();            
            if (temp == "")
            {
                MessageBox.Show(CommonRsText.strRs_Order_Qty_Pls);
                return false;
            }
            try
            {
                Convert.ToUInt32(temp);
            }
            catch
            {
                MessageBox.Show(CommonRsText.strRs_Order_Qty_Invalid);
                return false;
            }
            string strPrice = this.txt_Price_CT.Text.Trim();
            decimal d = 0.0M;
            if (this.ChkCTGeneralAO.IsChecked != true)
            {
                if (strPrice == "")
                {
                    MessageBox.Show(CommonRsText.StrRs_Order_Pls_Price);
                    return false;
                }
                try
                {
                    d = convertPrice(strPrice);
                }
                catch
                {
                    MessageBox.Show(CommonRsText.StrRs_Order_Invalid_Price);
                    return false;
                }
            }
            if (CheckTrigglePrice(BS) == false)
            {
                return false;
            }

            ProductBoundInfo BoundInfo = GosBzTool.getProdBoundInfo(this.ProdCode);
            int intQty = Convert.ToInt32(temp);
            if (intQty <= 0)
            {
                MessageBox.Show(CommonRsText.strRs_Order_Qty_Invalid);
                return false;
            }
            if (BoundInfo.IntQtyUp.HasValue)
            {
                if (BoundInfo.IntQtyUp.Value < intQty)
                {
                    string _msg = string.Format(CommonRsText.strRs_Qty_UP_Limit, BoundInfo.IntQtyUp.Value.ToString());
                    MessageBox.Show(_msg);//"qty should not exceeded Upper limit " + BoundInfo.IntQtyUp.Value.ToString());
                    return false;
                }
            }

            if (BoundInfo.IntQtyLower.HasValue)
            {
                if (BoundInfo.IntQtyLower.Value > intQty)
                {
                    string _msg = string.Format(CommonRsText.strRs_Qty_ShdNot_LessThan_LowLimit, BoundInfo.IntQtyLower.Value.ToString());
                    MessageBox.Show(_msg);//"qty should not less than lower limit " + BoundInfo.IntQtyLower.Value.ToString());
                    return false;
                }
            }
            if (GosBzTool.CheckDeviationQty(intQty) == false)
            {
                return false;
            }
          //  string strPrice = this.txt_Price_CT.Text.Trim(); 
          
            if (this.ChkCTGeneralAO.IsChecked != true)
            {                
                if (BoundInfo.DecPriceUp.HasValue)
                {
                    if (BoundInfo.DecPriceUp.Value < d)
                    {
                        string _msg = string.Format(CommonRsText.strRs_Price_Shlnot_ULimit, BoundInfo.DecPriceUp.Value.ToString());
                        MessageBox.Show( _msg);//"price should not exceeded upper limit " + BoundInfo.DecPriceUp.Value.ToString());
                        return false;
                    }
                }

                if (BoundInfo.DecPriceLower.HasValue)
                {
                    if (BoundInfo.DecPriceLower.Value > d)
                    {
                        string _msg = string.Format(CommonRsText.strRs_Price_Shlnot_LowLimit, BoundInfo.DecPriceLower.Value.ToString());
                        MessageBox.Show(_msg);// "price should not less than lower limit " + BoundInfo.DecPriceLower.Value.ToString());
                        return false;
                    }
                }
                if (GosBzTool.CheckDeviationPrice(d, CurrentProdMktItem) == false)
                {
                    return false;
                }
                decimal decInt = Math.Truncate(d);
                if ((d - decInt) > 0)
                {
                    int l = GosBzTool.getDecLen(txt_OrderID.Text.Trim());
                    int pos = strPrice.IndexOf('.');
                    if (pos > 0)
                    {
                        if (l == 0)
                        {
                            string _msg = string.Format(CommonRsText.strRs_input_price_no_allow_dec, txt_OrderID.Text.Trim());
                            MessageBox.Show(_msg);// "no decimal digit allowed for product '" + txt_OrderID.Text.Trim() + "' ");
                            return false;
                        }
                        int decLen = strPrice.Length - pos - 1;
                        if (decLen > l)
                        {
                            string _msg = string.Format(CommonRsText.strRs_input_priceDec_only_sLen_allowed, "only " + l.ToString(), txt_OrderID.Text.Trim());
                            MessageBox.Show(_msg);//"only " + l.ToString() + " number of decimal digit allowed for product '" + txt_OrderID.Text.Trim() + "'");
                            return false;
                        }
                    }
                }
            }
            return true;
        }
        
        bool CheckTrigglePrice(string BS)
        {
            //CHECK TrigglePrice 
            TradeStationComm.Attribute.StopType stopType = TradeStationComm.Attribute.StopType.normalOrder;
            try
            {
                if (cbbstop.SelectedIndex != -1)
                {
                    ComboBoxItem item = cbbstop.SelectionBoxItem as ComboBoxItem;
                    if (item == null) { item = cbbstop.SelectedItem as ComboBoxItem; };
                    if (item != null)
                    {
                        string strTemp = item.Tag.ToString().Trim();
                        stopType = (TradeStationComm.Attribute.StopType)Enum.Parse(typeof(TradeStationComm.Attribute.StopType), strTemp);
                    }
                }
            }
            catch
            {
                MessageBox.Show(CommonRsText.strRs_input_stoptype_error);
                return false;
            }

           // int intStopPrice = 0;
            decimal dStopPrice = 0m;
            if (this.txt_StopPrice_CT.Text.Trim() == "")
            {
                MessageBox.Show(CommonRsText.strRs_input_trgStop_Pls);
                return false;
            }
            else
            {
                string strTemp = this.txt_StopPrice_CT.Text.Trim();
                try
                {
                    Convert.ToDecimal(strTemp);
                }
                catch
                {
                    MessageBox.Show(CommonRsText.strRs_input_SLPrice_invalid);
                    return false;
                }
                dStopPrice = get_txt_StopPrice_CT();              
            }



            decimal LastMKPrice = 0M;
            decimal OrderPrice = get_txt_Price_CT();
            bool hasMktPrice = false;
            if (CurrentProdMktItem != null)
            {
                if (CurrentProdMktItem.Last != null)
                {
                    try
                    {
                        LastMKPrice = Convert.ToDecimal(CurrentProdMktItem.Last);
                        hasMktPrice = true;
                    }
                    catch { }
                }
            }
            if (stopType == TradeStationComm.Attribute.StopType.upTrigger)
            {
                if (hasMktPrice)
                {
                    if (dStopPrice <= LastMKPrice)
                    {
                        MessageBox.Show(CommonRsText.strRs_TrStopPrice_Must_High_MktPrice);
                        return false;
                    }
                }
            }
            if (stopType == TradeStationComm.Attribute.StopType.downTrigger)
            {
                if (hasMktPrice)
                {
                    if (dStopPrice >= LastMKPrice)
                    {
                        MessageBox.Show(CommonRsText.strRs_TrStopPrice_Must_Low_MktPrice);// "Trigger price must be lower than market price." );
                        return false;
                    }
                }
            }
            if (stopType == TradeStationComm.Attribute.StopType.stopLoss)
            {
                bool b = false;
                if (BS == "B")
                {                    
                    if (hasMktPrice)
                    {
                        
                        if (dStopPrice <= LastMKPrice)
                        {
                            b = true;
                            MessageBox.Show(CommonRsText.strRs_TrStopPrice_Must_High_MktPrice);//"Trigger price must be higher than market price.");
                            return false;
                        }
                    }
                    if ((!b)&&OrderPrice < dStopPrice)
                    {
                        MessageBox.Show(CommonRsText.strRs_SL_BuyPrice_Must_EqOrMore_MktPrice);// "Buy price must be equal to or higher than trigger price.");
                        return false;
                    }
                }
                else if (BS == "S")
                {
                    if (hasMktPrice)
                    {
                        if (dStopPrice >= LastMKPrice)
                        {
                            b = true;
                            MessageBox.Show(CommonRsText.strRs_TrStopPrice_Must_Low_MktPrice);//"Trigger price must be lower than market price." );
                            return false;
                        }
                    }
                    if ((!b)&&OrderPrice > dStopPrice)
                    {
                        MessageBox.Show(CommonRsText.strRs_SL_SellPrice_Must_EqOrLow_TrPrice);// "Sell price must be equal to or lower than trigger price." );
                        return false;
                    }
                }
            }

            return true;
        }
        private void btn_CTbuy_Click_1(object sender, RoutedEventArgs e)
        {
            if (this.ProdCode == null || this.ProdCode.Trim() == "")
            {
                MessageBox.Show(CommonRsText.StrRs_input_prod_pls );// "please selected a product");
                tbProdFocus();
                return;
            }

            if (!ValidCTGeneralBuy("B"))
            {
                ConditionTbPriceFocus();
                return;
            }
            // Buy

            if (CanTrade() == false)
            {
                ConditionTbPriceFocus();
                return;
            }
            if (GOSTradeStation.isDealer)
            {
                if (CurrentID == null || CurrentID.Trim() == "")
                {
                    MessageBox.Show(CommonRsText.StrRs_input_Acc_pls );// "please selected an account");
                    ctrlAccFocus();
                    return;
                }
            }
            if (GOSTradeStation.isDealer == true && this.CurrentID != this.cbbUsers.cbbUsers.Text.Trim())
            {
                string _msg = string.Format(CommonRsText.strRs_Order_AccNotEqual, this.cbbUsers.cbbUsers.Text.Trim(), this.CurrentID);
                if (MessageBox.Show(_msg,//"'" + this.cbbUsers.cbbUsers.Text.Trim() + "' not equal current acc '" + this.CurrentID + "',continue place to order with current acc '" + this.CurrentID + "?  ",
                    CommonRsText.strRs_confirm_BuyOrder_Title,// "Buy Confirm ",
                    MessageBoxButton.OKCancel) != MessageBoxResult.OK)
                {
                    ctrlAccFocus();
                    return;
                }
            }
            try
            {
                decimal OrderPrice = get_txt_Price_CT();
                int intOrderPrice = GosBzTool.ChangeDecToIn(this.ProdCode, OrderPrice);
                strAccountID = CurrentID;
                uint qty = Convert.ToUInt32(this.txt_Count_CT.Text.Trim());

                CheckBox _chkAo = this.ChkCTGeneralAO;
                TradeStationComm.Attribute.ValidType EnumValid = TradeStationComm.Attribute.ValidType.today;
                ComboBoxItem objValid = CbbValid.SelectedItem as ComboBoxItem;
                if (objValid != null)
                {
                    string strValid = objValid.Tag.ToString().Trim();
                    if (strValid.ToUpper() == "SPECDATE")
                    {
                        EnumValid = TradeStationComm.Attribute.ValidType.specTime;
                    }
                    else
                    {
                        try
                        {
                            EnumValid = (TradeStationComm.Attribute.ValidType)Enum.Parse(typeof(TradeStationComm.Attribute.ValidType), strValid, true);
                        }
                        catch { }
                    }
                }
                 TradeStationComm.Attribute.Active Status =TradeStationComm.Attribute.Active.active ;
                 if (ChkCtGeneralInactive.IsChecked == true)
                 {
                     Status = TradeStationComm.Attribute.Active.inactive;
                 }

                 TradeStationComm.Attribute.StopType stopType = TradeStationComm.Attribute.StopType.normalOrder;
                 try
                 {
                     if (cbbstop.SelectedIndex != -1)
                     {
                         ComboBoxItem item = cbbstop.SelectionBoxItem as ComboBoxItem;
                         if (item == null) { item = cbbstop.SelectedItem as ComboBoxItem; };
                         if (item != null)
                         {
                             string strTemp = item.Tag.ToString().Trim();
                             stopType = (TradeStationComm.Attribute.StopType)Enum.Parse(typeof(TradeStationComm.Attribute.StopType), strTemp);
                         }
                     }
                 }
                 catch
                 {
                     MessageBox.Show(CommonRsText.strRs_input_stoptype_error );// "error stoptype convert");
                     ConditionTbPriceFocus();
                     return;
                 }

                 int intStopPrice = 0;
                 decimal dStopPrice = 0m;
                 if (this.txt_StopPrice_CT.Text.Trim() == "")
                 {
                     MessageBox.Show(CommonRsText.strRs_input_trgStop_Pls);// "please input stop price");
                     this.txt_StopPrice_CT.Focus();
                     return;
                 }
                 else
                 {
                     string strTemp = this.txt_StopPrice_CT.Text.Trim();
                     try
                     {
                         Convert.ToDecimal(strTemp);
                     }
                     catch
                     {
                         MessageBox.Show(CommonRsText.strRs_input_SLPrice_invalid);// "invalid stop price");
                         this.txt_StopPrice_CT.Focus();
                         return;
                     }
                     dStopPrice = get_txt_StopPrice_CT();
                     intStopPrice = GosBzTool.ChangeDecToIn(this.ProdCode, dStopPrice);
                 }


                 string Ref = this.cdGeneralRef.Text.Trim();
                 string strMessage = string.Format(CommonRsText.strRs_confirm_BuyOrder, //"Acc：{1}\nCmd:Buy\nID：{0}\nPrice：{2}\nQty：{3}\nValidity:{4}", 
                     strAccountID,
                     this.ProdCode, 
                     (_chkAo.IsChecked ?? false) ? "AO" : OrderPrice.ToString(),
                    qty,
                   CommonRsText.getValidTypeText( EnumValid));

                 TradeStationComm.Attribute.CondType condType = (_chkAo.IsChecked ?? false) ? TradeStationComm.Attribute.CondType.AO : TradeStationComm.Attribute.CondType.normal;
                 
                 long specTime = 0;

                if (EnumValid == TradeStationComm.Attribute.ValidType.specTime)// || EnumValid == TradeStationComm.Attribute.ValidType.GTC)
                {
                    if (dt_CT_GN_SpecTime.SelectedDate.HasValue)
                    {
                        specTime = Utility.ConvertToUnixTime(dt_CT_GN_SpecTime.SelectedDate.Value);
                    }
                    if (specTime == 0)
                    {
                        MessageBox.Show(CommonRsText.strRs_Input_order_specDate_pls);// "please select validity date");
                        ConditionTbPriceFocus();
                        return;
                    }
                    strMessage += "," + dt_CT_GN_SpecTime.SelectedDate.Value.ToString("yyyy-MM-dd");
                }
                string strMsgOther = "";
                if (Status == TradeStationComm.Attribute.Active.inactive)
                {
                    strMsgOther += CommonRsText.getActiveText(Status);
                }
                strMsgOther += (chkT1.IsChecked ?? false) ? "," + CommonRsText.strRs_TOne : "";
                if (strMsgOther.StartsWith(","))
                {
                    strMsgOther = strMsgOther.Substring(1);
                }
                strMsgOther = "\n" + strMsgOther;
                strMessage = string.Format(CommonRsText.strRs_confirm_StopTrgOrder,// "{0},\n{1},\nStop Trigger:{2}", 
                    strMessage,
                  // CommonRsText.getActiveText(Status) + ((chkT1.IsChecked ?? false) ? "," + CommonRsText.strRs_TOne : ""),
                  strMsgOther,
                   CommonRsText.getStopTypeText(stopType) + (intStopPrice != 0 ? " ," + dStopPrice.ToString() : ""));
                MsgForm msgbox = new MsgForm(CommonRsText.strRs_confirm_BuyOrder_Title,
                    strMessage, 15);
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
                        ConditionTbPriceFocus();
                        return;
                    }
                    TradeStationLog.WriteLogPerformance(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")+" ct buy:" + strMessage);
                    if (GOSTradeStation.isDealer)
                    {
                        GOSTradeStation.msgChannel.Send(TradeStationComm.MsgRequest.getMsgAddOrder(GOSTradeStation.UserID, GOSTradeStation.Pwd, strAccountID,
                            new TradeStationComm.MsgRequest.OrderClass.AddOrderClass(this.ProdCode,
                                TradeStationComm.Attribute.BuySell.buy,
                                qty,
                                (_chkAo.IsChecked ?? false) ? 0 : intOrderPrice,
                                EnumValid,
                                condType,
                                stopType,
                                intStopPrice,
                                Status,
                                specTime,
                                TradeStationComm.Attribute.AE.AE,
                                strAccountID,
                                Ref,
                                orderModel.TPlus
                                )
                           ));
                        TradeStationSend.Send(cmdClient.getOrderBookInfo, this.CurrentID);
                        TradeStationSend.Send(cmdClient.getAccountInfo, this.CurrentID);
                        //kenlo20150312 TradeStationSend.Send(cmdClient.getPositionInfo, this.CurrentID);
                       if (TradeStationSend.ExistInContainer(WindowTypes.TradeConfirmation))
                        {
                            TradeStationSend.Send(cmdClient.getTradeConfOrders);
                        }
                    }
                    else
                    {
                        GOSTradeStation.msgChannel.Send(TradeStationComm.MsgRequest.getMsgAddOrder(GOSTradeStation.UserID,
                            GOSTradeStation.Pwd,
                            strAccountID,
                                                    new TradeStationComm.MsgRequest.OrderClass.AddOrderClass(this.ProdCode,
                                                            TradeStationComm.Attribute.BuySell.buy,
                                                            qty,
                                                            (_chkAo.IsChecked ?? false) ? 0 : intOrderPrice,
                                                            EnumValid,
                                                           condType,
                                                           stopType,
                                                            intStopPrice,
                                                            Status,
                                                            specTime,
                                                            TradeStationComm.Attribute.AE.normalUser,
                                                            "",
                                                           Ref,
                                                            orderModel.TPlus
                                                            )
                                                  ));
                        TradeStationSend.Send(cmdClient.getOrderBookInfo);
                        TradeStationSend.Send(cmdClient.getAccountInfo);
                        //kenlo20150312 TradeStationSend.Send(cmdClient.getPositionInfo);
                    }
                    // orderChild.Close();
                }
            }
            catch (Exception)
            {

                MessageBox.Show(CommonRsText.strRs_BuyOrder_Fail );// "Bought failed");
            }
            ConditionTbPriceFocus();
        }

        private void btn_CTsell_Click_1(object sender, RoutedEventArgs e)
        {
            if (this.ProdCode == null || this.ProdCode.Trim() == "")
            {
                MessageBox.Show(CommonRsText.StrRs_input_prod_pls);// "please selected a product");
                tbProdFocus();
                return;
            }
            if (!ValidCTGeneralBuy("S"))
            {
                ConditionTbPriceFocus();
                return;
            }
            // Buy
           

            if (CanTrade() == false)
            {
                ConditionTbPriceFocus();
                return;
            }
            if (GOSTradeStation.isDealer)
            {
                if (CurrentID == null || CurrentID.Trim() == "")
                {
                    MessageBox.Show(CommonRsText.StrRs_input_Acc_pls);
                    ConditionTbPriceFocus();
                    return;
                }
            }
            if (GOSTradeStation.isDealer == true && this.CurrentID != this.cbbUsers.cbbUsers.Text.Trim())
            {
                string _msg = string.Format(CommonRsText.strRs_Order_AccNotEqual, this.cbbUsers.cbbUsers.Text.Trim(), this.CurrentID);
                if (MessageBox.Show(_msg,//"'" + this.cbbUsers.cbbUsers.Text.Trim() + "' not equal current acc '" + this.CurrentID + "',continue place to order with current acc '" + this.CurrentID + "?  ", 
                    CommonRsText.strRs_confirm_BuyOrder_Title,
                    MessageBoxButton.OKCancel) != MessageBoxResult.OK)
                {
                    ConditionTbPriceFocus();
                    return;
                }
            }
            try
            {

                decimal OrderPrice = get_txt_Price_CT();
                int intOrderPrice = GosBzTool.ChangeDecToIn(this.ProdCode, OrderPrice);
                strAccountID = CurrentID;
                uint qty = Convert.ToUInt32(this.txt_Count_CT.Text.Trim());//checked in ValidCTGeneralBuy();

                CheckBox _chkAo = this.ChkCTGeneralAO;
                TradeStationComm.Attribute.ValidType EnumValid = TradeStationComm.Attribute.ValidType.today ;
                ComboBoxItem objValid = CbbValid.SelectedItem as ComboBoxItem;
                if (objValid != null)
                {
                    string strValid = objValid.Tag.ToString().Trim();
                    if (strValid.ToUpper() == "SPECDATE")
                    {
                        EnumValid = TradeStationComm.Attribute.ValidType.specTime;
                    }
                    else
                    {
                        try
                        {
                            EnumValid = (TradeStationComm.Attribute.ValidType)Enum.Parse(typeof(TradeStationComm.Attribute.ValidType), strValid, true);
                        }
                        catch { }
                    }
                }
                TradeStationComm.Attribute.Active Status = TradeStationComm.Attribute.Active.active;
                if (ChkCtGeneralInactive.IsChecked == true)
                {
                    Status = TradeStationComm.Attribute.Active.inactive;
                }
                TradeStationComm.Attribute.StopType stopType = TradeStationComm.Attribute.StopType.normalOrder; ;
                if (cbbstop.SelectedIndex != -1)
                {
                    ComboBoxItem item = cbbstop.SelectionBoxItem as ComboBoxItem;
                    if (item == null) { item = cbbstop.SelectedItem as ComboBoxItem; };
                    if (item != null)
                    {
                        string strTemp = item.Tag.ToString().Trim();
                        stopType = (TradeStationComm.Attribute.StopType)Enum.Parse(typeof(TradeStationComm.Attribute.StopType), strTemp);
                    }
                }

                int intStopPrice = 0;
                decimal dStopPrice = 0m;
                if (this.txt_StopPrice_CT.Text.Trim() == "")
                {
                }
                else
                {
                    string strTemp = this.txt_StopPrice_CT.Text.Trim();
                    try
                    {
                        Convert.ToDecimal(strTemp);
                    }
                    catch
                    {
                        MessageBox.Show(CommonRsText.strRs_input_trgStop_Pls);
                        ConditionTbPriceFocus();
                        return;
                    }
                    dStopPrice = get_txt_StopPrice_CT();
                    intStopPrice = GosBzTool.ChangeDecToIn(this.ProdCode, dStopPrice);
                }               

                string Ref = this.cdGeneralRef.Text.Trim();
                string strMessage = string.Format(CommonRsText.strRs_confirm_SellOrder,// "Acc：{1}\nCmd:Buy\nID：{0}\nPrice：{2}\nQty：{3}\nValidity:{4}", 
                    strAccountID,
                     this.ProdCode, 
                    (_chkAo.IsChecked ?? false) ? "AO" : OrderPrice.ToString(),
                   qty,
                  CommonRsText.getValidTypeText( EnumValid));

                TradeStationComm.Attribute.CondType condType = (_chkAo.IsChecked ?? false) ? TradeStationComm.Attribute.CondType.AO : TradeStationComm.Attribute.CondType.normal;

                long specTime = 0;

                if (EnumValid == TradeStationComm.Attribute.ValidType.specTime)// || EnumValid == TradeStationComm.Attribute.ValidType.GTC)  //orderModel.Valid == TradeStationComm.Attribute.ValidType.GTC || 
                {
                    if (dt_CT_GN_SpecTime.SelectedDate.HasValue)
                    {
                        specTime = Utility.ConvertToUnixTime(dt_CT_GN_SpecTime.SelectedDate.Value);
                    }
                    if (specTime == 0)
                    {
                        MessageBox.Show(CommonRsText.strRs_Input_order_specDate_pls);//"please select validity date");
                        ConditionTbPriceFocus();
                        return;
                    }
                    strMessage += "," + dt_CT_GN_SpecTime.SelectedDate.Value.ToString("yyyy-MM-dd");
                }

                string strMsgOther = "";
                if (Status == TradeStationComm.Attribute.Active.inactive)
                {
                    strMsgOther += CommonRsText.getActiveText(Status);
                }
                strMsgOther += (chkT1.IsChecked ?? false) ? "," + CommonRsText.strRs_TOne : "";
                if (strMsgOther.StartsWith(","))
                {
                    strMsgOther = strMsgOther.Substring(1);
                }

                strMsgOther = "\n" + strMsgOther;
                strMessage = string.Format(CommonRsText.strRs_confirm_StopTrgOrder,
                    strMessage,
                   // CommonRsText.getActiveText(Status) + ((chkT1.IsChecked ?? false) ? "," + CommonRsText.strRs_TOne : ""),
                   strMsgOther,
                    CommonRsText.getStopTypeText( stopType) + (intStopPrice != 0 ? " ," + dStopPrice.ToString() : ""));
                MsgForm msgbox = new MsgForm(CommonRsText.strRs_confirm_SellOrder_Title , strMessage, 15);
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
                        ConditionTbPriceFocus();
                        return;
                    }
                    TradeStationLog.WriteLogPerformance(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")+" ct sell:" + strMessage);
                   
                    if (GOSTradeStation.isDealer)
                    {
                        GOSTradeStation.msgChannel.Send(TradeStationComm.MsgRequest.getMsgAddOrder(GOSTradeStation.UserID, GOSTradeStation.Pwd, strAccountID,
                            new TradeStationComm.MsgRequest.OrderClass.AddOrderClass(this.ProdCode,
                                TradeStationComm.Attribute.BuySell.sell,
                                qty,
                                (_chkAo.IsChecked ?? false) ? 0 : intOrderPrice,
                                EnumValid,
                                condType,
                                stopType,
                                intStopPrice,
                                Status,
                                specTime,
                                TradeStationComm.Attribute.AE.AE,
                                strAccountID,
                                Ref,
                                orderModel.TPlus
                                )
                           ));
                        TradeStationSend.Send(cmdClient.getOrderBookInfo, this.CurrentID);
                        TradeStationSend.Send(cmdClient.getAccountInfo, this.CurrentID);
                        //kenlo20150312 TradeStationSend.Send(cmdClient.getPositionInfo, this.CurrentID);
                       if (TradeStationSend.ExistInContainer(WindowTypes.TradeConfirmation))
                        {
                            TradeStationSend.Send(cmdClient.getTradeConfOrders);
                        }
                    }
                    else
                    {
                        GOSTradeStation.msgChannel.Send(TradeStationComm.MsgRequest.getMsgAddOrder(GOSTradeStation.UserID,
                            GOSTradeStation.Pwd,
                            strAccountID,
                                                    new TradeStationComm.MsgRequest.OrderClass.AddOrderClass(this.ProdCode,
                                                            TradeStationComm.Attribute.BuySell.sell,
                                                            qty,
                                                            (_chkAo.IsChecked ?? false) ? 0 : intOrderPrice,
                                                            EnumValid,
                                                           condType,
                                                           stopType,
                                                            intStopPrice,
                                                            Status,
                                                            specTime,
                                                            TradeStationComm.Attribute.AE.normalUser,
                                                            "",
                                                           Ref,
                                                            orderModel.TPlus
                                                            )
                                                  ));
                        TradeStationSend.Send(cmdClient.getOrderBookInfo);
                        TradeStationSend.Send(cmdClient.getAccountInfo);
                        //kenlo20150312 TradeStationSend.Send(cmdClient.getPositionInfo);
                    }
                    // orderChild.Close();
                }
            }
            catch (Exception)
            {

                MessageBox.Show(CommonRsText.strRs_SellOrder_Fail);// "Bought failed");
            }
            ConditionTbPriceFocus();
        }
        
        void NomalTbPriceFocus()
        {
            if (txt_Price_NT != null)
            {
                if (txt_Price_NT.txtAmount != null)
                {
                    txt_Price_NT.txtAmount.Focus();
                }
            }
        }

        void ConditionTbPriceFocus()
        {
            if (txt_Price_CT != null)
            {
                if (txt_Price_CT.txtAmount != null)
                {
                    txt_Price_CT.txtAmount.Focus();
                }
            }
        }
        void ctrlAccFocus()
        {
            if (cbbUsers != null)
            {
                if (cbbUsers.cbbUsers != null)
                {
                    cbbUsers.cbbUsers.Focus();
                }
            }            
        }

        void tbProdFocus()
        {
            if (this.txt_OrderID != null)
            {
                this.txt_OrderID.Focus();
            }
        }

        void txtQTradeTicksFocus()
        {
            if (this.txtQTradeTicks != null)
            {
                txtQTradeTicks.Focus();
            }
        }

        //2014-10-11
        ORDEntryTitleStatus titleStatus = new ORDEntryTitleStatus();
        public override void SetFormTitle(int itype, MessageDistribute _distributeMsg, WPF.MDI.MdiChild _mdiChild, params string[] _Params)
        {
            titleStatus.restore(itype, _Params);
            string strTitle = titleStatus.getTitle(this.strTitleFormat);
            SetMdITitle(distributeMsg, _mdiChild, strTitle);           
        }

        public override void SetFormTitle()
        {           
            string strTitle = titleStatus.getTitle(this.strTitleFormat);
            SetMdITitle(distributeMsg, orderChild, strTitle);
        }
    }

    public class ORDEntryTitleStatus
    {
        public int iType = 0;
        public string ProdName = "";
        public string ProdCode = "";

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
                    str = string.Format(strTitleFormat,ProdCode, ProdName);
                    str = str.Replace("|", "");
                    break;
            }
            return str;
        }

        public void restore(int _itype, params string[] parms)
        {
            switch (_itype)
            {
                case 0:
                    iType = _itype;
                    ProdName = "";
                    ProdCode = "";
                    break;
                case 1:
                    iType = _itype;
                    ProdName = GOSTS.Utility.getArrayItemValue(parms, 0);
                    ProdCode = GOSTS.Utility.getArrayItemValue(parms, 1);
                    break;
                default:                 
                    iType = _itype;
                    ProdName = "";
                    ProdCode = "";
                    break;
            }
        }
    }

    public class OrderModel:INotifyPropertyChanged
    {         
        private string _ProductCode;
        public string ProductCode {
            get { return _ProductCode; }
            set { _ProductCode = value; OnPropertyChanged(new PropertyChangedEventArgs("ProductCode")); }
        }

        private string _ProductName;
        public string ProductName {
            get { return _ProductName; }
            set { _ProductName = value; OnPropertyChanged(new PropertyChangedEventArgs("ProductName")); }
        }

        private  decimal   _Price=0;
        public decimal Price
        {
            get { return _Price; }
            set { _Price = value; OnPropertyChanged(new PropertyChangedEventArgs("Price")); }
        }

        private uint _Count = 1;
        public uint Count
        {
            get  { return _Count; }
            set{_Count=value;OnPropertyChanged(new PropertyChangedEventArgs("Count"));}
        }

        private string _mdiChildTitle;
        public string mdiChildTitle
        {
            get { return _mdiChildTitle; }
            set { _mdiChildTitle = value; OnPropertyChanged(new PropertyChangedEventArgs("mdiChildTitle")); }
        }

        private string _Open;
        public string Open
        {
            get { return _Open; }
            set { _Open = value; OnPropertyChanged(new PropertyChangedEventArgs("Open")); }
        }

        private string _PreClose;
        public string PreClose
        {
            get { return _PreClose; }
            set { _PreClose = value; OnPropertyChanged(new PropertyChangedEventArgs("PreClose")); }
        }

        private string _Change;
        public string Change
        {
            get { return _Change; }
            set { _Change = value; OnPropertyChanged(new PropertyChangedEventArgs("Change")); }
        }

        private string _Hight;
        public string Hight
        {
            get { return _Hight; }
            set { _Hight = value; OnPropertyChanged(new PropertyChangedEventArgs("Hight")); }
        }


        private string _Low;
        public string Low
        {
            get { return _Low; }
            set { _Low = value; OnPropertyChanged(new PropertyChangedEventArgs("Low")); }
        }


        private string _Volumn;
        public string Volumn
        {
            get { return _Volumn; }
            set { _Volumn = value; OnPropertyChanged(new PropertyChangedEventArgs("Volumn")); }
        }

        private TradeStationComm.Attribute.ValidType _Valid = TradeStationComm.Attribute.ValidType.today;
        public  TradeStationComm.Attribute.ValidType Valid
        {
            get { return _Valid; }
            set { _Valid = value; OnPropertyChanged(new PropertyChangedEventArgs("Valid")); }
        }
        
        private DateTime? _dtSpecialTime;
        public  DateTime? dtSpecialTime
        {
            get { return _dtSpecialTime; }
            set { _dtSpecialTime = value; OnPropertyChanged(new PropertyChangedEventArgs("dtSpecialTime")); }
        }

        private TradeStationComm.Attribute.StopType _StopType = TradeStationComm.Attribute.StopType.normalOrder; //.stopLoss;
        public TradeStationComm.Attribute.StopType StopType
        {
            get { return _StopType; }
            set { _StopType = value; OnPropertyChanged(new PropertyChangedEventArgs("StopType")); }
        }

        private int _StopPrice=0;
        public int StopPrice
        {
            get { return _StopPrice; }
            set { _StopPrice = value; OnPropertyChanged(new PropertyChangedEventArgs("StopPrice")); }
        }

        private int _active = 0;
        public int Active
        {
            get { return _active; }
            set { _active = value; OnPropertyChanged(new PropertyChangedEventArgs("Active")); }
        }

        private string _Ref;
        public string Ref
        {
            get { return _Ref; }
            set { _Ref = value; OnPropertyChanged(new PropertyChangedEventArgs("Ref")); }
        }

        private TradeStationComm.Attribute.TOne _TPlus;
        public TradeStationComm.Attribute.TOne TPlus
        {
            get { return _TPlus; }
            set { _TPlus = value; OnPropertyChanged(new PropertyChangedEventArgs("TPlus")); }
        }
        //  this.tbVolQty.Text = drv["volume"].ToString();
        //  this.tbVolume.Text = "";//["volume"].ToString();

        #region Quick trade
        private  decimal _Ask;
        public decimal Ask
        {
            get { return _Ask; }
            set { _Ask = value; OnPropertyChanged(new PropertyChangedEventArgs("Ask")); CalAskTo(); }
        }

        private decimal _Bid;
        public decimal Bid
        {
            get { return _Bid; }
            set { _Bid = value; OnPropertyChanged(new PropertyChangedEventArgs("Bid")); CalBidTo(); }
        }

        private decimal _Ticks;
        public decimal Ticks
        {
            get { return _Ticks; }
            set { _Ticks = value; OnPropertyChanged(new PropertyChangedEventArgs("Ticks")); CalAskTo(); CalBidTo(); }
        }

        private decimal _AskTo;
        public decimal AskTo
        {
            get { return _AskTo; }
            set { _AskTo = value; OnPropertyChanged(new PropertyChangedEventArgs("AskTo")); }
        }

        private int intAO = AppFlag.intAOConst ;
    //    private string strAO = "AO";

        private decimal _BidTo;
        public decimal BidTo
        {
            get { return _BidTo; }
            set { _BidTo = value; OnPropertyChanged(new PropertyChangedEventArgs("BidTo")); }
        }
        #endregion

        public bool BidAO
        {
            get
            {
                if (GosBzTool.IsAOValue(Bid.ToString()))
                {
                    return true;
                }
                return false;
            }
        }

        public bool AskAO
        {
            get
            {
                if (GosBzTool.IsAOValue(Ask.ToString()))
                {
                    return true;
                }
                return false;
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(PropertyChangedEventArgs propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, (propertyName));
            }
        }

        #endregion

        public void CalAskTo()
        {
           // if(GosBzTool.IsAOValue(
            if (intAO == this.Ask)
            {
                this.AskTo = intAO;
            }
            else
            {
                this.AskTo = Ask + Ticks;
            }
        }

        public void CalBidTo()
        {
            if (intAO == this.Bid)
            {
                this.BidTo = intAO;
            }
            else
            {
                decimal I = Bid - Ticks;
                //if (I < 0)
                //{
                //    I = 0;
                //}
                this.BidTo = I;
            }
        }

        
    }

    public class TextValueItem
    {
        public string ItemText { get; set; }
        public string ItemValue { get; set; }
    }

    public class CharValueItem
    {
        public string ItemText { get; set; }
        public char ItemValue { get; set; }
    }

    public class ValidTypeList : ObservableCollection<TextValueItem>
    {

        public ValidTypeList()
        {
          
            this.Clear();
            //this.Add(new TextValueItem { ItemValue = "0", ItemText = "即日" });
            //this.Add(new TextValueItem { ItemValue = "1", ItemText = "成交並取消" });
            //this.Add(new TextValueItem { ItemValue = "2", ItemText = "成交或取消" });
            //this.Add(new TextValueItem { ItemValue = "3", ItemText = "到期日" });
            //this.Add(new TextValueItem { ItemValue = "4", ItemText = "指定日期" });

            this.Add(new TextValueItem { ItemValue = "0", ItemText = "TODAY" });
            this.Add(new TextValueItem { ItemValue = "1", ItemText = "FAK" });
            this.Add(new TextValueItem { ItemValue = "2", ItemText = "FOK" });
            this.Add(new TextValueItem { ItemValue = "3", ItemText = "GTC" });
            this.Add(new TextValueItem { ItemValue = "4", ItemText = "SPEC TIME" });     
        }
    }

    public class ValidTypeListStrValue : ObservableCollection<TextValueItem>
    {

        public ValidTypeListStrValue()
        {

            this.Clear();
            this.Add(new TextValueItem { ItemValue = TradeStationComm.Attribute.ValidType.today.ToString().Trim(), ItemText = "即日" });
            this.Add(new TextValueItem { ItemValue = TradeStationComm.Attribute.ValidType.FAK.ToString().Trim(), ItemText = "成交並取消" });
            this.Add(new TextValueItem { ItemValue = TradeStationComm.Attribute.ValidType.FOK.ToString().Trim(), ItemText = "成交或取消" });
            this.Add(new TextValueItem { ItemValue = TradeStationComm.Attribute.ValidType.GTC.ToString().Trim(), ItemText = "到期日" });
            this.Add(new TextValueItem { ItemValue = TradeStationComm.Attribute.ValidType.specTime.ToString().Trim(), ItemText = "指定日期" });     
        }
    }

    public class CondTypeList : ObservableCollection<TextValueItem>  
    {
         public CondTypeList(){
         this.Clear();
            this.Add(new TextValueItem { ItemValue = "0", ItemText = "即日" });
            this.Add(new TextValueItem { ItemValue = "1", ItemText = "成交並取消" });
        }
    }

   


    public static class OrderUtil
    {
        public static void Analyse(OrderResponse orderResponse)
        {
             if (orderResponse == null)
                 return;
             if (orderResponse.Rinfo == null)
                 return;

             string RInfo = orderResponse.Rinfo;
            string spLeft = "sp=";
            string bdLeft="&bd=";
            int pos = RInfo.IndexOf(spLeft);
            if (pos <0)
            {
                return;
            }   
            int posEnd=RInfo.IndexOf(bdLeft);
            if(posEnd<0)return;
            if(pos>=posEnd)return;
            int len = posEnd - pos;
            string sp = RInfo.Substring(pos, len);
            sp = sp.Replace("sp=","").Trim();

            string str = RInfo.Substring(posEnd).Replace(bdLeft,"").Replace("\"","").Replace("\\","");
            string[] arr = str.Split(sp.ToCharArray());
            string ProdCode="";
            TradeStationComm.Attribute.BuySell bs= TradeStationComm.Attribute.BuySell.buy;
            if(arr.Length>=0)
            {
                ProdCode=arr[0];
            }
            if(arr.Length>=1)
            {
                if(arr[1].ToLower()=="s")
                {
                    bs=TradeStationComm.Attribute.BuySell.sell;
                }
            }
            int qty=Utility.ConvertToInt(arr[2]);
            int price=Utility.ConvertToInt(arr[3]);
            string refstr=arr[4];
          
        }      
    } 
}
