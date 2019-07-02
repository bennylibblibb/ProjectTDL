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
using System.Text.RegularExpressions;

namespace GOSTS
{
    /// <summary>
    /// ChangeOrder.xaml 的交互逻辑
    /// </summary>
    public partial class ChangeOrder : UserControl, IChangeLang
    {

        DataRowView dgv_changeOrder = null;
        public MessageDistribute distributeMsg = null;
        public decimal ChangePrice;
        public uint ChangeCount,oldCount;
        public string ChangeRef;
        public string OrderNo,prodCode,Valid;
        TradeStationComm.Attribute.ValidType sEnumValid = TradeStationComm.Attribute.ValidType.today;
        string strMessage, StrBuySell;

        DataGrid m_dgSrc = null; // MatthewSin
        string m_sOrderNo = ""; // MatthewSin

        public MdiChild ChangeOrderChild;

        delegate void NotificationItem(string str);
        NotificationItem NotificationDelegate;

        delegate void DisOrderConfirm(string str);
        DisOrderConfirm DisOrderConfirmDelegate;

        delegate void BindListItemMarketPrice(ObservableCollection<MarketPriceItem> MktItems);//DataTable dataTable);
        BindListItemMarketPrice MarketPriceDelegate;

        public string Acc = "";
        string TOne = "0";

        static ChangeOrder chOrder;

        DecimalTextBz decTbCheck = new DecimalTextBz();

        #region Resoure Text
        string strRs_NoChange {
            get{ return  GOSTS.GosCulture.CultureHelper.GetString("CHOD_noChange");}
        }
        string strRs_invalidInput
        {
            get { return GOSTS.GosCulture.CultureHelper.GetString("CHOD_invalidInput"); }
        }
        string strRs_plesaeInput
        {
            get { return GOSTS.GosCulture.CultureHelper.GetString("CHOD_plesaeInput"); }
        }
        string strRs_CHOD_Pls_Price
        {
            get { return GOSTS.GosCulture.CultureHelper.GetString("CHOD_Pls_Price"); }
        }
        string strRs_CHOD_PLS_Qty
        {
            get { return GOSTS.GosCulture.CultureHelper.GetString("CHOD_PLS_Qty"); }
        }
        string strRs_CHOD_Invalid_Qty
        {
            get { return GOSTS.GosCulture.CultureHelper.GetString("CHOD_Invalid_Qty"); }
        }
        string strRs_CHOD_Invalid_price
        {
            get { return GOSTS.GosCulture.CultureHelper.GetString("CHOD_Invalid_price"); }
        }
        string strRs_CHOD_PLS_Val_Digit
        {
            get { return GOSTS.GosCulture.CultureHelper.GetString("CHOD_PLS_Val_Digit"); }
        }


        string strRs_BuyConfTitle
        {
            get { return GOSTS.GosCulture.CultureHelper.GetString("CHOD_BuyConfTitle"); }
        }
        string strRs_Ch_Err
        {
            get { return GOSTS.GosCulture.CultureHelper.GetString("CHOD_Err"); }
        }


        string strRs_CHOD_sellConf
        {
            get { return GOSTS.GosCulture.CultureHelper.GetString("CHOD_sellConfirm"); }
        }

        string strRs_CHOD_sellConfirmTitle
        {
            get { return GOSTS.GosCulture.CultureHelper.GetString("CHOD_sellConfirmTitle"); }
        }
        string strRs_CHOD_CHSellConfTitle
        {
            get { return GOSTS.GosCulture.CultureHelper.GetString("CHOD_CHSellConfTitle"); }
        }
        string strRs_CHOD_ChSellConf
        {
            get { return GOSTS.GosCulture.CultureHelper.GetString("CHOD_ChSellConfirm"); }
        }
        string strRs_CHOD_buyConfirm
        {
            get { return GOSTS.GosCulture.CultureHelper.GetString("CHOD_buyConfirm"); }
        }

        string strRs_CHOD_CHbuyConfirm
        {
            get { return GOSTS.GosCulture.CultureHelper.GetString("CHOD_CHbuyConfirm"); }
        }
        string strRs_CHBuyConfTitle
        {
            get { return GOSTS.GosCulture.CultureHelper.GetString("CHOD_CHBuyConfTitle"); }
        }

        string strRs_CHOD_DelConf
        {
            get { return GOSTS.GosCulture.CultureHelper.GetString("CHOD_DelConf"); }
        }
        string strRs_CH_DelConfTitle
        {
            get { return GOSTS.GosCulture.CultureHelper.GetString("CHOD_DelConfTitle"); }
        }

        string strRs_Qty_exceed_Ulimit
        {
            get { return GOSTS.GosCulture.CultureHelper.GetString("Qty_UP_Limit"); }
        }
        string strRs_Qty_shl_Less_Lowlimit
        {
            get { return GOSTS.GosCulture.CultureHelper.GetString("Qty_Shd_LessThan_LowLimit"); }
        }
        string strRs_CHOD_InValid_Stop_Price
        {
            get { return GOSTS.GosCulture.CultureHelper.GetString("CHOD_InValid_Stop_Price"); }
        }
        //string strRs_CHOD_Pls_Tr_Price
        //{
        //    get { return GOSTS.GosCulture.CultureHelper.GetString("input_trgStop_Pls"); }
        //}

        string strRs_Price_Shlnot_ULimit
        {
            get { return GOSTS.GosCulture.CultureHelper.GetString("Price_Shlnot_ULimit"); }
        }
        string strRs_Price_Shlnot_LowLimit
        {
            get { return GOSTS.GosCulture.CultureHelper.GetString("Price_Shlnot_LowLimit"); }
        }
        string strRs_CHOD_TrPrice_Must_Low_MktPrice
        {
            get { return GOSTS.GosCulture.CultureHelper.GetString("CHOD_TrPrice_Must_Low_MktPrice"); }
        }
        string strRs_CHOD_TrPrice_Must_High_MktPrice
        {
            get { return GOSTS.GosCulture.CultureHelper.GetString("CHOD_TrPrice_Must_High_MktPrice"); }
        }
        string strRs_CHOD_SL_Sell_Price_Must_EqOrLow_TrPrice
        {
            get { return GOSTS.GosCulture.CultureHelper.GetString("CHOD_SL_Sell_Price_Must_EqOrLow_TrPrice"); }
        }
        string strRs_CHOD_StopLoss_BuyPrice_Must_EqOrMore_MktPrice
        {
            get { return GOSTS.GosCulture.CultureHelper.GetString("CHOD_StopLoss_BuyPrice_Must_EqOrMore_MktPrice"); }
        }
        string strRs_CHOD_SL_Sell_TrPrie_MustLow_MktPrice
        {
            get { return GOSTS.GosCulture.CultureHelper.GetString("CHOD_SL_Sell_TrPrie_MustLow_MktPrice"); }
        }

        #endregion

        public static ChangeOrder getChangeOrder(MessageDistribute _msgDistribute, DataRowView drv, string buysell, uint count, MdiChild mdi, string acc, string tone,ref bool  bInitMdi, 
                                                 DataGrid p_dgSrc, string p_sOrderNo) // MatthewSin
        {
            bool bInstantiate = false;
            if(chOrder==null)
            {
                bInstantiate = true;               
            }
            else if(chOrder.distributeMsg!=_msgDistribute)
            {
                bInstantiate = true;               
            }           
           
            if (bInstantiate)
            {
                chOrder = new ChangeOrder(_msgDistribute, drv, buysell, count, mdi, acc, tone, p_dgSrc, p_sOrderNo); // MatthewSin
            }
            else
            {             
                chOrder.SetOrderData(drv, buysell, count, acc, tone, p_dgSrc, p_sOrderNo); // MatthewSin
            }

            chOrder.m_dgSrc = p_dgSrc; // MatthewSin
            chOrder.m_sOrderNo = p_sOrderNo; // MatthewSin   

            bInitMdi = bInstantiate;
            return chOrder;
        }

        void CloneDv(DataRowView drv)
        {
            DataTable dt=drv.Row.Table.Clone();
            DataRow newRow = dt.NewRow();
            newRow.ItemArray = drv.Row.ItemArray;
            dt.Rows.Add(newRow);
            dt.AcceptChanges();
            dgv_changeOrder = dt.DefaultView[0];
        }

        void  SetOrderData(DataRowView drv, string buysell, uint count, string acc, string tone, DataGrid p_dgSrc, string p_sOrderNo)
        {
            this.m_dgSrc = p_dgSrc;
            this.m_sOrderNo = p_sOrderNo;

            if (tone == "1")
            {
                chkT1.IsChecked = true;
            }
            else
            {
                chkT1.IsChecked = false;
            }

            string ao = drv.Row.getColValue("cond");
           
            if ( ao.ToUpper().Trim() == "AO")//ao == "3" ||
            {
                this.chkAO.IsChecked = true;
                decTbCheck.bChkAO = true;
            }
            else
            {
                this.chkAO.IsChecked = false;
                decTbCheck.bChkAO = false;
            }
            switchCtrlStatuByAo();
            if (drv != null)
            {
                CloneDv(drv);           
            }
            Acc = acc;
            this.lbl_Account.Content = acc;
            oldCount = count;
            if (buysell == "buy")
            {
                LinearGradientBrush myLinearGradientBrush = new LinearGradientBrush();
                myLinearGradientBrush.StartPoint = new Point(0.504, 0.03);
                myLinearGradientBrush.EndPoint = new Point(0.504, 1.5);
                myLinearGradientBrush.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#FFD7D9F0"), 0.042));
                myLinearGradientBrush.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#FF5187D4"), 0.925));

                b_ChangeOrder.Background = myLinearGradientBrush;
                UG_buy.Visibility = Visibility.Visible;
                UG_sell.Visibility = Visibility.Hidden;

                LinearGradientBrush myLinearGradientBrush1 = new LinearGradientBrush();
                myLinearGradientBrush1.StartPoint = new Point(0.504, 0.03);
                myLinearGradientBrush1.EndPoint = new Point(0.504, 1.5);
                myLinearGradientBrush1.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#FFF"), 0));
                myLinearGradientBrush1.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#FFD7D9F0"), 0.5));
                rectAccInfo.Fill = myLinearGradientBrush1;
            }
            else
            {
                LinearGradientBrush myLinearGradientBrush = new LinearGradientBrush();
                myLinearGradientBrush.StartPoint = new Point(0.504, 0.03);
                myLinearGradientBrush.EndPoint = new Point(0.504, 1.5);
                myLinearGradientBrush.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#FFF3D0F3"), 0.042));
                myLinearGradientBrush.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#FFFF84EE"), 0.925));

                b_ChangeOrder.Background = myLinearGradientBrush;
                UG_buy.Visibility = Visibility.Hidden;
                UG_sell.Visibility = Visibility.Visible;

                LinearGradientBrush myLinearGradientBrush1 = new LinearGradientBrush();
                myLinearGradientBrush1.StartPoint = new Point(0.504, 0.03);
                myLinearGradientBrush1.EndPoint = new Point(0.504, 1.5);
                myLinearGradientBrush1.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#FFF"), 0));
                myLinearGradientBrush1.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#FFF3D0F3"), 0.5));
                rectAccInfo.Fill = myLinearGradientBrush1;
            }

            StrBuySell = buysell;
            prodCode = dgv_changeOrder.Row.getColValue("productCode"); 
            decTbCheck.SetProdCode(prodCode);//.prodCode = prodCode;
            setTxtPrice(dgv_changeOrder.Row.getColValue("price"));
            BKOriginalPrice();

            this.txt_Count.Text = count.ToString();
            this.txt_Ref.Text = dgv_changeOrder.Row.getColValue("refNo"); 
            this.lbl_Order.Content = dgv_changeOrder.Row.getColValue("productCode"); 
            OrderNo = dgv_changeOrder.Row.getColValue("internalOrderNo");        
            tbOrder.Text ="#" +OrderNo;

            ChangePrice = getTxtPrice();
            SetUPDownBtnStep();
            ChangeCount = Convert.ToUInt32(this.txt_Count.Text);
            ChangeRef = this.txt_Ref.Text;

            string _strValid = dgv_changeOrder.Row.getColValue("valid");
            Valid = _strValid;
            try
            {
                sEnumValid = (TradeStationComm.Attribute.ValidType)Enum.Parse(typeof(TradeStationComm.Attribute.ValidType), _strValid, true);
            }
            catch
            {
                try
                {
                    sEnumValid = CommonRsText.getEnumValidType(_strValid);
                }
                catch (Exception ex)
                { }
            }

            Trigger(dgv_changeOrder.Row);
            BKInputSource();
            List<string> ls = new List<string>();
            ls.Add(prodCode);
            TradeStationSend.Get(ls, cmdClient.getMarketPrice);

            if (ChangeOrderChild != null)
            {
                TitleStatus.SetFormTitle(1,null,this.ChangeOrderChild,new string[]{ OrderNo, prodCode.ToUpper()});
                //ChangeOrderChild.Title = @"Change/Delete Order Order #" + OrderNo + "-" + prodCode.ToUpper();
            }
        }
       
        MarketPriceItem CurrentProdMktItem;
        void ReceiveMktPrice(ObservableCollection<MarketPriceItem> MktItems)
        {
            if (MktItems == null) return;
            if (prodCode == null) return;
            if (prodCode.Trim() == "")
            {
                return;
            }
            var results = MktItems.FirstOrDefault(x => x.ProductCode == prodCode.Trim());//               
            if (results != null)
            {               
                CurrentProdMktItem = results;
            }
        }

        void switchCtrlStatuByAo()
        {
            if (this.chkAO.IsChecked ?? false)
            {
                btnDow.IsEnabled = false;
                btnUp.IsEnabled = false;
                this.txt_Price.Text = "";
                this.txt_Price.IsEnabled = false;
            }
            else
            {
                btnDow.IsEnabled = true;
                btnUp.IsEnabled = true;
              //  this.txt_Price.Text = "";
                this.txt_Price.IsEnabled = true;
            }
        }

        public void DisposeOldChangeOrder()
        {

        }

        public bool TriggerChange(DataRow dr)
        {
            if (dr == null) return false ;
            string _Cond = dr.getColValue("cond").Trim();
            if (GosBzTool.CanChangeOrder(_Cond))
            {
                return false;
            }
            string _strPrice = txt_StopPrice_CT.Text;
            if (_strPrice != strBkTriggerPrice)
            {
                return true;
            }
            return false;
        }

        string strBkTriggerPrice = "";
        public void Trigger(DataRow dr)
        {
            if (dr == null) return;
            string _Cond = dr.getColValue("cond").Trim();
            string _Prod=dr.getColValue("productCode"); 
            decTbCheck.SetProdCode(_Prod);
            decTbCheck.SetMdiChild(this.ChangeOrderChild);
            if (GosBzTool.CanChangeOrder(_Cond))
            {
                this.gdSTPrice.Visibility = Visibility.Collapsed;
                chkAO.Visibility = Visibility.Visible;
                tbOrder.Visibility = Visibility.Visible;
            }
            else
            {
                this.gdSTPrice.Visibility = Visibility.Visible;
                chkAO.Visibility = Visibility.Collapsed;
                tbOrder.Visibility = Visibility.Collapsed;
                if(_Cond.Length<3)return;
                string TGType = _Cond.Substring(0, 2).ToUpper();
                string strTrPrice = _Cond.Substring(2);
                strTrPrice = Regex.Replace(strTrPrice, @"[^\d\.]", "");
                switch (TGType)
                {
                    case "SL":
                        this.cbbstop.SelectedIndex=0;//.SelectedValue = "Stop Loss";
                        break;
                    case "UP":
                        this.cbbstop.SelectedIndex=1;//.SelectedValue = "Up Trigger";
                        break;
                    case "DN":
                        this.cbbstop.SelectedIndex=2;//.SelectedValue = "Down Trigger";
                        break;
                }
                txt_StopPrice_CT.SetProdCode(_Prod);
                decimal d = Utility.ConvertToDecimal(strTrPrice);
                txt_StopPrice_CT.Text = GosBzTool.adjustDecLength(_Prod, d).ToString();
                strBkTriggerPrice = txt_StopPrice_CT.Text;
                txt_StopPrice_CT.SetMdiChild(this.ChangeOrderChild);
            }
        }

        private ChangeOrder(MessageDistribute _msgDistribute, DataRowView drv, string buysell, uint count,MdiChild mdi,string acc,string tone, DataGrid p_dgSrc, string p_sOrderNo)
        {
            InitializeComponent();

            this.m_dgSrc = p_dgSrc;
            this.m_sOrderNo = p_sOrderNo;

            Acc = acc;
            this.lbl_Account.Content = acc;
            DisOrderConfirmDelegate = new DisOrderConfirm(OpenOrderConfrim);
            NotificationDelegate = new NotificationItem(NotificationItemMethod);
            MarketPriceDelegate = new BindListItemMarketPrice(ReceiveMktPrice);
            ChangeOrderChild = mdi;
            
            string ao = drv.Row.getColValue("cond");
            
            if (ao.ToUpper().Trim() == "AO")//ao == "3" || 
            {
                this.chkAO.IsChecked = true;
                decTbCheck.bChkAO = true;
            }
            else
            {
                this.chkAO.IsChecked = false;
                decTbCheck.bChkAO = false;
            }
            switchCtrlStatuByAo();

            if (tone == "1")
            {
                chkT1.IsChecked = true;
            }
            else
            {
                chkT1.IsChecked = false;
            }
           
            if (drv != null)
            {
                CloneDv(drv);             
            }
            oldCount = count;
            if (buysell == "buy")
            {
                LinearGradientBrush myLinearGradientBrush = new LinearGradientBrush();
                myLinearGradientBrush.StartPoint = new Point(0.504, 0.03);
                myLinearGradientBrush.EndPoint = new Point(0.504, 1.5);
                myLinearGradientBrush.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#FFD7D9F0"), 0.042));
                myLinearGradientBrush.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#FF5187D4"), 0.925));

                b_ChangeOrder.Background = myLinearGradientBrush;
                UG_buy.Visibility = Visibility.Visible;
                UG_sell.Visibility = Visibility.Hidden;

                LinearGradientBrush myLinearGradientBrush1 = new LinearGradientBrush();
                myLinearGradientBrush1.StartPoint = new Point(0.504, 0.03);
                myLinearGradientBrush1.EndPoint = new Point(0.504, 1.5);
                myLinearGradientBrush1.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#FFF"), 0));
                myLinearGradientBrush1.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#FFD7D9F0"), 0.5));
                rectAccInfo.Fill = myLinearGradientBrush1;
            }
            StrBuySell = buysell;
            prodCode = dgv_changeOrder.Row.getColValue("productCode");
            decTbCheck.SetProdCode(prodCode);
            setTxtPrice(dgv_changeOrder.Row.getColValue("price"));
            BKOriginalPrice();
            this.txt_Count.Text = count.ToString();
            SetUPDownBtnStep();
            this.txt_Ref.Text = dgv_changeOrder.Row.getColValue("refNo");
            this.lbl_Order.Content = dgv_changeOrder.Row.getColValue("productCode");
            OrderNo = dgv_changeOrder.Row.getColValue("internalOrderNo");
            tbOrder.Text ="#"+ OrderNo;
            Trigger(dgv_changeOrder.Row);
           
            ChangePrice = getTxtPrice() ;
            ChangeCount = Convert.ToUInt32(this.txt_Count.Text);
            ChangeRef = this.txt_Ref.Text;
            
            string _strValid = dgv_changeOrder.Row.getColValue("valid");
            Valid = _strValid;
            try
            {
                sEnumValid = (TradeStationComm.Attribute.ValidType)Enum.Parse(typeof(TradeStationComm.Attribute.ValidType), _strValid, true);              
            }
            catch {
                    try
                    {
                        sEnumValid=CommonRsText.getEnumValidType(_strValid);                        
                    }
                    catch (Exception ex)
                    {}
             }

            decTbCheck.SetDecText(this.txt_Price);

            List<string> ls = new List<string>();
            ls.Add(prodCode);
            if (_msgDistribute != null)
            {
                distributeMsg = _msgDistribute;
                distributeMsg.DisMarketPrice += distributeMsg_DisMarketPrice;
            }
            BKInputSource();
            TradeStationSend.Get(ls, cmdClient.getMarketPrice);
            decTbCheck.SetMdiChild(ChangeOrderChild);
            decTbCheck.BindWheel();
            if (ChangeOrderChild != null)
            {
                //ChangeOrderChild.Title = @"Change/delete Order Order #" + OrderNo + "-" + prodCode.ToUpper();
                TitleStatus.SetFormTitle(1, null, this.ChangeOrderChild, new string[] { OrderNo, prodCode.ToUpper() });
               
            }
        }

        void distributeMsg_DisMarketPrice(object sender, System.Collections.ObjectModel.ObservableCollection<MarketPriceItem> MarketPriceItems)
        {
            try
            {
                Application.Current.Dispatcher.BeginInvoke(this.MarketPriceDelegate, new Object[] { MarketPriceItems });
            }
            catch (Exception exp)
            {
                TradeStationLog.WriteParserResult(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + ",on change order,Type: distributeMsg_DisMarketPrice,Error:" + exp.Message);
            }
        }

        void setTxtPrice(string strPric)
        {
            if (this.chkAO.IsChecked ?? false)
            {
                this.txt_Price.Text = "";
            }
            else
            {
                decimal decNum = Utility.ConvertToDecimal(strPric);
               // decNum = GosBzTool.CheckPriceBound(decNum);
                this.txt_Price.Text = GosBzTool.adjustDecLength(this.prodCode, decNum.ToString()).ToString();//GosBzTool.getDecimalPrice(prodCode,strPric).ToString();
            }
        }
        string StrOriginalTxtPrice = "",strSourceQty="",strSourceRef="";
        void BKOriginalPrice()
        {
            StrOriginalTxtPrice = this.txt_Price.Text.Trim();
        }
        
        bool CheckIfAddOrder()
        {
            if (this.chkAO.IsChecked == true)
            {
                return false;
            }
            if (StrOriginalTxtPrice == "")
            {
                return false;
            }
            string str = this.txt_Price.Text;
            if (StrOriginalTxtPrice == str)
            {
                return true;
            }
            return false;
        }

        void BKInputSource()
        {
            BKOriginalPrice();
            strSourceQty = this.txt_Count.Text.Trim();
            strSourceRef = this.txt_Ref.Text.Trim();
        }

        bool bChange()
        {
            string strQty = this.txt_Count.Text.Trim();
            string strRef = this.txt_Ref.Text.Trim();
            if (strQty == strSourceQty && strRef == strSourceRef)
            {
                if (this.chkAO.IsChecked==true)
                {
                    return false;
                }
                string str = this.txt_Price.Text;
                if (StrOriginalTxtPrice == str)
                {
                    return false;
                }
            }
            return true;
        }

        decimal getTxtPrice()
        {
            if (this.chkAO.IsChecked ?? false)
            {
                return 0;
            }
            else
            {
                try
                {
                    decimal i= Convert.ToDecimal(this.txt_Price.Text);
                    return i;
                }
                catch{
                    return 0;
                }
            }
        }

        public void NotificationItemMethod(string str)
        {
            if (str != null)
            {
                Application.Current.Dispatcher.Invoke(DisOrderConfirmDelegate, new object[] { str });
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

        protected void distributeMsg_DeleteOrder(object sender, String str)
        {
            Application.Current.Dispatcher.Invoke(DisOrderConfirmDelegate, new object[] { str });
        }

        protected void distributeMsg_ChangeOrder(object sender, String str)
        {

            Application.Current.Dispatcher.Invoke(DisOrderConfirmDelegate, new object[] { str });

        }

    private bool CheckIfSrcGrdQtyHasChanged() // MatthewSin
    {
      if (m_dgSrc == null || m_sOrderNo == "") return false;
      foreach (System.Data.DataRowView drv in m_dgSrc.ItemsSource)
      {
        if (drv[0].ToString() == m_sOrderNo)
        {
          uint iQty = 0;
          if (StrBuySell == "buy")
            iQty = drv.Row.getColUIntValue("osBQty", 0);
          else
            iQty = drv.Row.getColUIntValue("osSQty", 0);
          MessageBox.Show("Order "+m_sOrderNo + " [" + StrBuySell + "] (OnSrcGridDoubleClick=" + oldCount + ",OnChangeConfirm=" + iQty + ")");
          if (iQty != oldCount)
          {
            MessageBox.Show("Rejected (Order "+m_sOrderNo + " Source Grid Quantity Changed)");
            return false;
          }
        }
      }
      return true;
    }

    private void btn_Sell_Change_Click(object sender, RoutedEventArgs e)
        {

            if (CheckIfSrcGrdQtyHasChanged() == false) return; // MatthewSin

            if (bChange() == false && TriggerChange(dgv_changeOrder.Row) == false)
            {
                MessageBox.Show(strRs_NoChange);
                tbPriceFocus();
                return;
            }
            if (!ValidateInput())
            {
                tbPriceFocus();
                return;
            }
            if (!CheckTrigglePrice("S"))
            {
                tbPriceFocus();
                return;
            }
            ChangeRef=txt_Ref.Text.Trim();
            try
            {
                if (dgv_changeOrder != null&&this.txt_Count.Text!=null&&this.txt_Price.Text!=null&&this.txt_Ref.Text!=null)
                {
                    ChangeCount = Convert.ToUInt32(this.txt_Count.Text);
                    ChangePrice = getTxtPrice();
                    int IntPrice = GosBzTool.ChangeDecToIn(prodCode, ChangePrice);
                    string _prodCode = dgv_changeOrder.Row.getColValue("productCode");                 
                    //string strValid = dgv_changeOrder.Row.getColValue("valid");

                    TradeStationComm.Attribute.ValidType EnumValid = sEnumValid;// TradeStationComm.Attribute.ValidType.FAK;
                    //if (strValid.ToUpper() == "SPECDATE")
                    //{
                    //    EnumValid = TradeStationComm.Attribute.ValidType.specTime;
                    //}
                    //else
                    //{
                    //    try
                    //    {
                    //        EnumValid = (TradeStationComm.Attribute.ValidType)Enum.Parse(typeof(TradeStationComm.Attribute.ValidType), strValid, true);
                    //    }
                    //    catch { }
                    //}
                    
                    TradeStationComm.Attribute.CondType condType = TradeStationComm.Attribute.CondType.normal;
                    try
                    {
                        condType = (this.chkAO.IsChecked ?? false) ? TradeStationComm.Attribute.CondType.AO : TradeStationComm.Attribute.CondType.normal;
                    }
                    catch { }

                    int StopPrice = 0;
                    TradeStationComm.Attribute.StopType stopType = TradeStationComm.Attribute.StopType.normalOrder;
                    if(this.chkAO.IsChecked !=true)
                    {
                         string str=dgv_changeOrder.Row.getColValue("cond");
                         stopType=GosBzTool.getStopTypeByOrderBookStatus(str);
                         if (stopType != TradeStationComm.Attribute.StopType.normalOrder)
                         {
                             decimal dStopPrice = txt_StopPrice_CT.get_txt_Price_NT();
                             StopPrice = GosBzTool.ChangeDecToIn(_prodCode, dStopPrice);
                         }
                    }                    

                    string strActive = dgv_changeOrder.Row.getColValue("status");
                    strActive = GosBzTool.getBase64StatusInfo(strActive, 1);
                    TradeStationComm.Attribute.Active Status = TradeStationComm.Attribute.Active.active;
                    if (strActive == "2")
                    {
                        Status = TradeStationComm.Attribute.Active.inactive;
                    }
                    
                    //try
                    //{
                    //    Status = (TradeStationComm.Attribute.Active)Enum.Parse(typeof(TradeStationComm.Attribute.Active), strActive, true);
                    //}
                    //catch { }
                    string strTimeStamp = dgv_changeOrder.Row.getColValue("specTime");
                    long lTimeStamp = 0;
                    try
                    {
                        lTimeStamp = Convert.ToInt64(strTimeStamp);
                    }
                    catch { }

                    string strRefNo = dgv_changeOrder.Row.getColValue("refNo");
                    string strTOne = dgv_changeOrder.Row.getColValue("tOne");
                    TradeStationComm.Attribute.TOne tone = TradeStationComm.Attribute.TOne.TOnly;
                    try
                    {
                        tone = (TradeStationComm.Attribute.TOne)Enum.Parse(typeof(TradeStationComm.Attribute.TOne), strTOne, true);
                    }
                    catch { }
                    

                    if (CheckIfAddOrder() && oldCount < ChangeCount)
                    {
                        uint intAddCount = (ChangeCount - oldCount);                        
                       // string strMessage = string.Format("Acc：{1}\nCmd:Buy\nID：{0}\nPrice：{2}\nQty：{3}\nValidity:{4}", _prodCode, Acc, (this.chkAO.IsChecked ?? false) ? "AO" : ChangePrice.ToString(),
                        string strMessage = string.Format(strRs_CHOD_sellConf, 
                            _prodCode, 
                            Acc, 
                            (this.chkAO.IsChecked ?? false) ? "AO" : ChangePrice.ToString(),                    
                            intAddCount,
                            CommonRsText.getValidTypeText(EnumValid) );

                        if (EnumValid == TradeStationComm.Attribute.ValidType.specTime && lTimeStamp != 0)
                        {
                            DateTime dtSpecTime = Utility.UnixToWinTime(lTimeStamp);
                            strMessage += "," + dtSpecTime.ToString("yyyy-MM-dd");
                        }
                        strMessage += ((chkT1.IsChecked ?? false) ? ",T+1" : "");

                        MsgForm msgbox = new MsgForm(strRs_CHOD_sellConfirmTitle, strMessage, 15);
                        Window w = Window.GetWindow(this);
                        if (w != null)
                        {
                            msgbox.Owner = w;
                        }
                        bool? Yes = msgbox.ShowDialog();

                        if (Yes == true)
                        {
                            TradeStationLog.WriteLogPerformance(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + " Change order-add,sell:" + strMessage);
                            if (GOSTradeStation.isDealer)
                            {
                                GOSTradeStation.msgChannel.Send(TradeStationComm.MsgRequest.getMsgAddOrder(GOSTradeStation.UserID, GOSTradeStation.Pwd, Acc,
                                    new TradeStationComm.MsgRequest.OrderClass.AddOrderClass(_prodCode,
                                        TradeStationComm.Attribute.BuySell.sell,
                                        intAddCount,
                                        (this.chkAO.IsChecked ?? false) ? 0 : IntPrice,
                                        EnumValid,
                                        condType,
                                        stopType,
                                        StopPrice,
                                        Status,
                                        lTimeStamp,
                                        TradeStationComm.Attribute.AE.AE,
                                        Acc,
                                        ChangeRef,//strRefNo,
                                        tone
                                        )
                                   ));
                                TradeStationSend.Send(cmdClient.getOrderBookInfo, Acc);
                                TradeStationSend.Send(cmdClient.getAccountInfo, Acc);
                                //kenlo20150312 TradeStationSend.Send(cmdClient.getPositionInfo, Acc);
                                if (TradeStationSend.ExistInContainer(WindowTypes.TradeConfirmation))
                                {
                                    TradeStationSend.Send(cmdClient.getTradeConfOrders);
                                }
                            }
                            else
                            {
                                GOSTradeStation.msgChannel.Send(TradeStationComm.MsgRequest.getMsgAddOrder(GOSTradeStation.UserID,
                                    GOSTradeStation.Pwd,
                                    Acc,
                                                            new TradeStationComm.MsgRequest.OrderClass.AddOrderClass(dgv_changeOrder.Row.getColValue("productCode"),
                                                                    TradeStationComm.Attribute.BuySell.sell,
                                                                     intAddCount,
                                                                    (this.chkAO.IsChecked ?? false) ? 0 : IntPrice,
                                                                    EnumValid,
                                                                    condType,
                                                                    stopType,
                                                                    StopPrice,
                                                                    Status,
                                                                    lTimeStamp,
                                                                    TradeStationComm.Attribute.AE.normalUser,
                                                                    "",
                                                                    ChangeRef,
                                                                    tone
                                                                    )
                                                          ));
                                TradeStationSend.Send(cmdClient.getOrderBookInfo);
                                TradeStationSend.Send(cmdClient.getAccountInfo);
                                //kenlo20150312 TradeStationSend.Send(cmdClient.getPositionInfo);
                            }
                        }
                    }
                    else
                    {
                        //"Change Order Confirm\n\nAccount:[{0}]\nOrder#:{1}\nCmd:{2}\nId:{3}\nPrice:{4}\nQty:{5}\n\nValidity:{6}"
                        strMessage = string.Format(strRs_CHOD_ChSellConf,
                            (GOSTradeStation.isDealer ? Acc : GOSTradeStation.UserID),
                               OrderNo,
                                //StrBuySell,
                                prodCode,
                                (this.chkAO.IsChecked ?? false) ? "AO" : ChangePrice.ToString(),
                                ChangeCount,
                                CommonRsText.getValidTypeText(sEnumValid) + ((chkT1.IsChecked ?? false) ? ",T+1" : ""));
                        MsgForm msgbox = new MsgForm(strRs_CHOD_CHSellConfTitle, strMessage, 15);
                        Window w = Window.GetWindow(this);
                        if (w != null)
                        {
                            msgbox.Owner = w;
                        }

                        bool? Yes = msgbox.ShowDialog();
                        //if (MessageBox.Show(strMessage, "Change Order Confirm ", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                        if (Yes == true)
                        {
                            if (GOSTradeStation.msgChannel == null)
                            {
                                return;
                            }
                            TradeStationLog.WriteLogPerformance(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + " Change order,sell:" + strMessage);                            
                            if (GOSTradeStation.isDealer)
                            {                               
                                GOSTradeStation.msgChannel.Send(TradeStationComm.MsgRequest.getMsgChangeOrder(GOSTradeStation.UserID, GOSTradeStation.Pwd, Acc,
                                       new TradeStationComm.MsgRequest.OrderClass.ChangeOrderClass(dgv_changeOrder.Row.getColValue("internalOrderNo"),
                                           dgv_changeOrder.Row.getColValue("productCode"),
                                           TradeStationComm.Attribute.BuySell.sell,
                                           ChangeCount,
                                           IntPrice,
                                           TradeStationComm.Attribute.AE.AE,
                                           Acc,
                                           ChangeRef,
                                           (chkT1.IsChecked ?? false) ? TradeStationComm.Attribute.TOne.TPluseOne : TradeStationComm.Attribute.TOne.TOnly,StopPrice)
                                      ));
                                TradeStationSend.Send(cmdClient.getOrderBookInfo, Acc);
                                TradeStationSend.Send(cmdClient.getAccountInfo, Acc);
                                //kenlo20150312 TradeStationSend.Send(cmdClient.getPositionInfo, Acc);
                               if (TradeStationSend.ExistInContainer(WindowTypes.TradeConfirmation))
                                {
                                    TradeStationSend.Send(cmdClient.getTradeConfOrders);
                                }
                            }
                            else
                            {
                                GOSTradeStation.msgChannel.Send(TradeStationComm.MsgRequest.getMsgChangeOrder(GOSTradeStation.UserID, GOSTradeStation.Pwd,
                                     new TradeStationComm.MsgRequest.OrderClass.ChangeOrderClass(dgv_changeOrder.Row.getColValue("internalOrderNo"),
                                         dgv_changeOrder.Row.getColValue("productCode"),
                                         TradeStationComm.Attribute.BuySell.sell,
                                         ChangeCount,
                                         IntPrice,
                                         TradeStationComm.Attribute.AE.normalUser,
                                         "",
                                       ChangeRef,
                                       (chkT1.IsChecked ?? false) ? TradeStationComm.Attribute.TOne.TPluseOne : TradeStationComm.Attribute.TOne.TOnly,StopPrice)
                                     ));
                                TradeStationSend.Send(cmdClient.getOrderBookInfo);
                                TradeStationSend.Send(cmdClient.getAccountInfo);
                                //kenlo20150312 TradeStationSend.Send(cmdClient.getPositionInfo);
                            }

                        }
                       
                    }
                    ChangeOrderChild.Close();
                    return;
                }
                else
                {
                   MessageBox.Show(strRs_plesaeInput);
                }
            }
            catch (Exception) 
            {
                MessageBox.Show(strRs_invalidInput);
            }
            tbPriceFocus();
        }

        private void btn_buy_change_Click(object sender, RoutedEventArgs e)
        {
            if (CheckIfSrcGrdQtyHasChanged() == false) return; // MatthewSin

            if (bChange() == false && TriggerChange(dgv_changeOrder.Row) == false)
            {
                MessageBox.Show(strRs_NoChange);
                tbPriceFocus();
                return;
            }
            if (!ValidateInput())
            {
                tbPriceFocus();
                return;
            }
            if (!CheckTrigglePrice("B"))
            {
                tbPriceFocus();
                return;
            }
            ChangeRef = txt_Ref.Text.Trim();
            try
            {
                if (dgv_changeOrder != null && this.txt_Count.Text != null && this.txt_Price.Text != null && this.txt_Ref.Text != null)
                {
                    ChangeCount = Convert.ToUInt32(this.txt_Count.Text);
                    ChangePrice = getTxtPrice();
                    int IntPrice = GosBzTool.ChangeDecToIn(prodCode, ChangePrice);
                    string _prodCode = dgv_changeOrder.Row.getColValue("productCode");                
                    //string strValid = dgv_changeOrder.Row.getColValue("valid");

                    TradeStationComm.Attribute.ValidType EnumValid = sEnumValid;// TradeStationComm.Attribute.ValidType.FAK;
                    //if (strValid.ToUpper() == "SPECDATE")
                    //{
                    //    EnumValid = TradeStationComm.Attribute.ValidType.specTime ;
                    //}
                    //else
                    //{
                    //    try
                    //    {
                    //        EnumValid = (TradeStationComm.Attribute.ValidType)Enum.Parse(typeof(TradeStationComm.Attribute.ValidType), strValid, true);
                    //    }
                    //    catch { }
                    //}
                    TradeStationComm.Attribute.CondType condType = TradeStationComm.Attribute.CondType.normal;
                    try
                    {
                        condType = (this.chkAO.IsChecked ?? false) ? TradeStationComm.Attribute.CondType.AO : TradeStationComm.Attribute.CondType.normal;
                    }
                    catch { }
                   
                    int StopPrice =0;
                    TradeStationComm.Attribute.StopType stopType = TradeStationComm.Attribute.StopType.normalOrder;                    
                    if (this.chkAO.IsChecked != true)
                    {
                        string str = dgv_changeOrder.Row.getColValue("cond");
                        stopType = GosBzTool.getStopTypeByOrderBookStatus(str);
                        if (stopType != TradeStationComm.Attribute.StopType.normalOrder)
                        {
                            decimal dStopPrice = txt_StopPrice_CT.get_txt_Price_NT();
                            StopPrice = GosBzTool.ChangeDecToIn(_prodCode, dStopPrice);
                        }
                    }
                    
                    string strActive=dgv_changeOrder.Row.getColValue("status");
                    strActive = GosBzTool.getBase64StatusInfo(strActive, 1).Trim();
                    TradeStationComm.Attribute.Active Status = TradeStationComm.Attribute.Active.active;
                    if (strActive == "2")
                    {
                        Status = TradeStationComm.Attribute.Active.inactive;
                    }                    
                    string strTimeStamp = dgv_changeOrder.Row.getColValue("specTime");
                    long lTimeStamp = 0;
                    try
                    {
                        lTimeStamp = Convert.ToInt64(strTimeStamp);
                    }
                    catch { }

                    string strRefNo = dgv_changeOrder.Row.getColValue("refNo");
                    string strTOne = dgv_changeOrder.Row.getColValue("tOne");

                    TradeStationComm.Attribute.TOne tone = TradeStationComm.Attribute.TOne.TOnly;
                    try
                    {
                        tone = (TradeStationComm.Attribute.TOne)Enum.Parse(typeof(TradeStationComm.Attribute.TOne), strTOne, true);
                    }
                    catch { }

                    if (CheckIfAddOrder() && oldCount < ChangeCount)
                    {
                        uint intAddCount = (ChangeCount - oldCount);
                        string strMessage = string.Format(strRs_CHOD_buyConfirm, _prodCode, Acc, (this.chkAO.IsChecked ?? false) ? "AO" : ChangePrice.ToString(),
                    intAddCount,
                    EnumValid.ToString());

                        if (EnumValid == TradeStationComm.Attribute.ValidType.specTime && lTimeStamp!=0)
                        {
                            DateTime dtSpecTime = Utility.UnixToWinTime(lTimeStamp);
                            strMessage += "," + dtSpecTime.ToString("yyyy-MM-dd");
                        }
                        strMessage +=((chkT1.IsChecked ?? false) ? ",T+1" : "");
                        MsgForm msgbox = new MsgForm(strRs_BuyConfTitle, strMessage, 15);
                        Window w = Window.GetWindow(this);
                        if (w != null)
                        {
                            msgbox.Owner = w;
                        }
                        bool? Yes = msgbox.ShowDialog();
                     
                        if (Yes == true)
                        {
                            TradeStationLog.WriteLogPerformance(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + " Change order-add,buy:" + strMessage);
                            if (GOSTradeStation.isDealer)
                            {
                                GOSTradeStation.msgChannel.Send(TradeStationComm.MsgRequest.getMsgAddOrder(GOSTradeStation.UserID, GOSTradeStation.Pwd, Acc,
                                    new TradeStationComm.MsgRequest.OrderClass.AddOrderClass(_prodCode,
                                        TradeStationComm.Attribute.BuySell.buy,
                                        intAddCount,
                                        (this.chkAO.IsChecked ?? false) ? 0 : IntPrice,
                                        EnumValid,
                                        condType,
                                        stopType,
                                        StopPrice,
                                        Status,
                                        lTimeStamp,
                                        TradeStationComm.Attribute.AE.AE,
                                        Acc,
                                        ChangeRef,
                                        tone
                                        )
                                   ));
                                TradeStationSend.Send(cmdClient.getOrderBookInfo, Acc);
                                TradeStationSend.Send(cmdClient.getAccountInfo, Acc);
                                //kenlo20150312 TradeStationSend.Send(cmdClient.getPositionInfo, Acc);
                               if (TradeStationSend.ExistInContainer(WindowTypes.TradeConfirmation))
                                {
                                    TradeStationSend.Send(cmdClient.getTradeConfOrders);
                                }
                            }
                            else
                            {
                                GOSTradeStation.msgChannel.Send(TradeStationComm.MsgRequest.getMsgAddOrder(GOSTradeStation.UserID,
                                    GOSTradeStation.Pwd,
                                    Acc,
                                                            new TradeStationComm.MsgRequest.OrderClass.AddOrderClass(dgv_changeOrder.Row.getColValue("productCode"),
                                                                    TradeStationComm.Attribute.BuySell.buy,
                                                                     intAddCount,
                                                                    (this.chkAO.IsChecked ?? false) ? 0 : IntPrice,
                                                                    EnumValid,
                                                                    condType,
                                                                    stopType,
                                                                    StopPrice,
                                                                    Status,
                                                                    lTimeStamp,
                                                                    TradeStationComm.Attribute.AE.normalUser,
                                                                    "",
                                                                    ChangeRef,
                                                                    tone
                                                                    )
                                                          ));
                                TradeStationSend.Send(cmdClient.getOrderBookInfo);
                                TradeStationSend.Send(cmdClient.getAccountInfo);
                                //kenlo20150312 TradeStationSend.Send(cmdClient.getPositionInfo);
                            }
                        }
                    }               
                    else
                    {       
                        // Change Order Confirm\n\nAccount:[{0}]\nOrder#:{1}\nCmd:{2}\nId:{3}\nPrice:{4}\nQty:{5}\n\n Validity:{6}
                       
                        strMessage = string.Format(strRs_CHOD_CHbuyConfirm,
                            (GOSTradeStation.isDealer ? Acc : GOSTradeStation.UserID),
                            OrderNo,
                         //   StrBuySell,
                           prodCode,
                           (this.chkAO.IsChecked ?? false) ? "AO" : ChangePrice.ToString(),
                            ChangeCount,
                            CommonRsText.getValidTypeText(sEnumValid)+ ((chkT1.IsChecked ?? false) ? ",T+1" : ""));

                        MsgForm msgbox = new MsgForm(strRs_CHBuyConfTitle, strMessage, 15);
                        Window w = Window.GetWindow(this);
                        if (w != null)
                        {
                            msgbox.Owner = w;
                        }

                        bool? Yes = msgbox.ShowDialog();
                        // if (MessageBox.Show(strMessage, "Change Order Confirm ", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                        if (Yes == true)
                        {
                            if (GOSTradeStation.msgChannel == null)
                            {
                                return;
                            }
                            TradeStationLog.WriteLogPerformance(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + " Change order,buy:" + strMessage);
                            if (GOSTradeStation.isDealer)
                            {
                                GOSTradeStation.msgChannel.Send(TradeStationComm.MsgRequest.getMsgChangeOrder(GOSTradeStation.UserID, GOSTradeStation.Pwd, Acc,
                                    new TradeStationComm.MsgRequest.OrderClass.ChangeOrderClass(dgv_changeOrder.Row.getColValue("internalOrderNo"),
                                        dgv_changeOrder.Row.getColValue("productCode"),
                                        TradeStationComm.Attribute.BuySell.buy,
                                        ChangeCount,
                                        IntPrice,
                                        TradeStationComm.Attribute.AE.AE,
                                        Acc,
                                        ChangeRef,
                                       (chkT1.IsChecked ?? false) ? TradeStationComm.Attribute.TOne.TPluseOne : TradeStationComm.Attribute.TOne.TOnly, StopPrice)
                                    ));
                                TradeStationSend.Send(cmdClient.getOrderBookInfo, Acc);
                                TradeStationSend.Send(cmdClient.getAccountInfo, Acc);
                                //kenlo20150312 TradeStationSend.Send(cmdClient.getPositionInfo, Acc);
                               if (TradeStationSend.ExistInContainer(WindowTypes.TradeConfirmation))
                                {
                                    TradeStationSend.Send(cmdClient.getTradeConfOrders);
                                }
                            }
                            else
                            {
                                GOSTradeStation.msgChannel.Send(TradeStationComm.MsgRequest.getMsgChangeOrder(GOSTradeStation.UserID, GOSTradeStation.Pwd,
                                    new TradeStationComm.MsgRequest.OrderClass.ChangeOrderClass(dgv_changeOrder.Row.getColValue("internalOrderNo"),
                                        dgv_changeOrder.Row.getColValue("productCode"),
                                        TradeStationComm.Attribute.BuySell.buy,
                                        ChangeCount,
                                        IntPrice,
                                        TradeStationComm.Attribute.AE.normalUser,
                                        "",
                                        ChangeRef,
                                       (chkT1.IsChecked ?? false) ? TradeStationComm.Attribute.TOne.TPluseOne : TradeStationComm.Attribute.TOne.TOnly, StopPrice)
                                      ));

                                TradeStationSend.Send(cmdClient.getOrderBookInfo);
                                TradeStationSend.Send(cmdClient.getAccountInfo);
                                //kenlo20150312 TradeStationSend.Send(cmdClient.getPositionInfo);
                            }
                        }
                        
                    }
                    ChangeOrderChild.Close();
                }
                else
                {
                    MessageBox.Show(this.strRs_plesaeInput);
                }
            }
            catch (Exception)
            {
                MessageBox.Show(this.strRs_invalidInput);
            }
            tbPriceFocus();
        }

        private void btn_Delete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dgv_changeOrder != null)
                {                   
                    //"Delete Order Confirm\n\nAccount:[{0}]\nOrder#:{1}\nCmd:{2}\nId:{3}\nPrice:{4}\nQty:{5}\n\nValidity:{6}"
                    strMessage = string.Format(strRs_CHOD_DelConf, 
                        (GOSTradeStation.isDealer?Acc: GOSTradeStation.UserID),
                        dgv_changeOrder.Row.getColValue("internalOrderNo"),
                        CommonRsText.GetBSText(StrBuySell),
                        prodCode,
                         (this.chkAO.IsChecked ?? false) ? "AO" : dgv_changeOrder.Row.getColValue("price"),
                         oldCount.ToString(), CommonRsText.getValidTypeText(sEnumValid) + ((chkT1.IsChecked ?? false) ? ",T+1" : ""));

                    MsgForm msgbox = new MsgForm(strRs_CH_DelConfTitle, strMessage, 15);
                    Window w = Window.GetWindow(this);
                    if (w != null)
                    {
                        msgbox.Owner = w;
                    }

                    bool? Yes = msgbox.ShowDialog();
                    //if (MessageBox.Show(strMessage, "Delete Order Confirm ", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    if(Yes ==true)
                    {
                        if (GOSTradeStation.msgChannel == null)
                        {
                            tbPriceFocus();
                            return;
                        }
                        TradeStationLog.WriteLogPerformance(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + " Change order,delete:" + dgv_changeOrder.Row.getColValue("internalOrderNo"));
                        if (GOSTradeStation.isDealer)
                        {
                            GOSTradeStation.msgChannel.Send(TradeStationComm.MsgRequest.getMsgDeleteOrder(GOSTradeStation.UserID, GOSTradeStation.Pwd,Acc,
                                   new TradeStationComm.MsgRequest.OrderClass.DeleteOrderClass(dgv_changeOrder.Row.getColValue("internalOrderNo"), 
                                       dgv_changeOrder.Row.getColValue("productCode"), 
                                       TradeStationComm.Attribute.AE.AE, 
                                       Acc, 
                                       ChangeRef,
                                      (chkT1.IsChecked ?? false) ? TradeStationComm.Attribute.TOne.TPluseOne : TradeStationComm.Attribute.TOne.TOnly)
                                  ));
                            TradeStationSend.Send(cmdClient.getOrderBookInfo, Acc);
                            TradeStationSend.Send(cmdClient.getAccountInfo, Acc);
                            //kenlo20150312 TradeStationSend.Send(cmdClient.getPositionInfo, Acc);
                           if (TradeStationSend.ExistInContainer(WindowTypes.TradeConfirmation))
                            {
                                TradeStationSend.Send(cmdClient.getTradeConfOrders); 
                            }
                        }
                        else
                        {
                            GOSTradeStation.msgChannel.Send(TradeStationComm.MsgRequest.getMsgDeleteOrder(GOSTradeStation.UserID, GOSTradeStation.Pwd,
                                new TradeStationComm.MsgRequest.OrderClass.DeleteOrderClass(dgv_changeOrder.Row.getColValue("internalOrderNo"), 
                                    dgv_changeOrder.Row.getColValue("productCode"), 
                                    TradeStationComm.Attribute.AE.normalUser, 
                                    "", 
                                    ChangeRef,
                                  (chkT1.IsChecked ?? false) ? TradeStationComm.Attribute.TOne.TPluseOne : TradeStationComm.Attribute.TOne.TOnly)
                               ));

                            TradeStationSend.Send(cmdClient.getOrderBookInfo);
                            TradeStationSend.Send(cmdClient.getAccountInfo);
                            //kenlo20150312 TradeStationSend.Send(cmdClient.getPositionInfo);
                        }
                        ChangeOrderChild.Close();
                        return;                          
                    }
                }
                else
                {
                    MessageBox.Show(strRs_Ch_Err);
                }
            }
            catch (Exception) { }
            tbPriceFocus();
        }



        private void txt_Count_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (this.txt_Count.Text.Trim() != null)
                {
                    ChangeCount = Convert.ToUInt32(this.txt_Count.Text);
                }
                else
                {
                    MessageBox.Show(strRs_CHOD_PLS_Qty);
                }
            }
            catch (Exception)
            {
               MessageBox.Show(strRs_CHOD_Invalid_Qty);
            }
        }

        private void txt_Price_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(this.chkAO.IsChecked??false)
            {
                return;
            }
            try
            {
                string temp = this.txt_Price.Text.Trim();
                if (temp != null)
                {
                    if (temp.EndsWith("."))
                    {
                        temp = temp + "0";
                    }
                    ChangePrice = Convert.ToDecimal(this.txt_Price.Text);
                }
                else
                {
                   MessageBox.Show(strRs_CHOD_PLS_Val_Digit);
                }
            }
            catch (Exception)
            {
               MessageBox.Show(strRs_CHOD_Invalid_price);
            }
        }

        bool CheckPrice()
        {
            if(this.chkAO.IsChecked??false)
            {
                return true;
            }
            decimal  i= getTxtPrice();
            if (i == 0)
            {
                MessageBox.Show(strRs_CHOD_Pls_Price);
                return false;
            }
            return true;
        }

        private void txt_Ref_TextChanged(object sender, TextChangedEventArgs e)
        {           
            if (this.txt_Ref.Text.Trim() != null)
            {
                ChangeRef = this.txt_Ref.Text;
            }           
        }

        public void OpenOrderConfrim(string str)
        {
        //    MdiContainer Container = App.Current.MainWindow.FindName("Container") as MdiContainer;
        //    MdiChild orderChild = new MdiChild();
        //    OrderConfirm OrderConfirm = new OrderConfirm(orderChild);
        //    OrderConfirm.lbl_Message.Text = str;
        //    OrderConfirm.lbl_Message.Foreground = Brushes.Red;
            
        //    orderChild.Title = "Order Confirm";
        //    orderChild.Content = OrderConfirm;
        //    orderChild.Width = OrderConfirm.Width;
        //    orderChild.Height = OrderConfirm.Height;
        //    orderChild.Position = new System.Windows.Point(0, 0);
        //    Container.Children.Add(orderChild);

            
            //Container.Children.Remove(GOSTradeStation.MdiChind_Order);
        }

        bool ValidateInput()
        {          
            string strQty = this.txt_Count.Text.Trim();
            if (this.txt_Count.Text.Trim() == "")
            {
                MessageBox.Show(strRs_CHOD_PLS_Qty);
                return false;
            }
            try
            {
                Convert.ToUInt32(this.txt_Count.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(strRs_CHOD_Invalid_Qty);
                return false;
            }
            ProductBoundInfo BoundInfo = GosBzTool.getProdBoundInfo(this.prodCode);
            int intQty = Convert.ToInt32(strQty);
            if (intQty <= 0)
            {
                MessageBox.Show(strRs_CHOD_Invalid_Qty);
                return false;
            }
            if (BoundInfo.IntQtyUp.HasValue)
            {
                if (BoundInfo.IntQtyUp.Value < intQty)
                {
                    string _strMsg = string.Format(strRs_Qty_exceed_Ulimit, BoundInfo.IntQtyUp.Value.ToString());
                    MessageBox.Show(_strMsg);//"qty exceeded Upper limit " + BoundInfo.IntQtyUp.Value.ToString());
                    return false;
                }
            }
            if (BoundInfo.IntQtyLower.HasValue)
            {
                if (BoundInfo.IntQtyLower.Value > intQty)
                {                   
                    string _strMsg = string.Format(strRs_Qty_shl_Less_Lowlimit, BoundInfo.IntQtyLower.Value.ToString());
                    MessageBox.Show(_strMsg);//"qty should be less than lower limit " + BoundInfo.IntQtyLower.Value.ToString());
                    return false;
                }
            }
            if (GosBzTool.CheckDeviationQty(intQty) == false)
            {
                return false;
            }

            if (this.chkAO.IsChecked != true)
            {
                if (this.txt_Price.Text.Trim() == "")
                {
                    MessageBox.Show(this.strRs_CHOD_Pls_Price);
                    return false;
                }
                try
                {
                    Convert.ToDecimal(this.txt_Price.Text);
                }
                catch (Exception)
                {
                    MessageBox.Show(this.strRs_CHOD_Invalid_price);
                    return false;
                }
                decimal d = getTxtPrice();

                if (BoundInfo.DecPriceUp.HasValue)
                {
                    if (BoundInfo.DecPriceUp.Value < d)
                    {
                        string _msg = string.Format(strRs_Price_Shlnot_ULimit, BoundInfo.DecPriceUp.Value.ToString());
                        MessageBox.Show(_msg);
                        return false;
                    }
                }

                if (BoundInfo.DecPriceLower.HasValue)
                {
                    if (BoundInfo.DecPriceLower.Value > d)
                    {
                        string _msg = string.Format(strRs_Price_Shlnot_LowLimit, BoundInfo.DecPriceLower.Value.ToString());
                        MessageBox.Show(_msg);
                        return false;
                    }
                }

                if (GosBzTool.CheckDeviationPrice(d, CurrentProdMktItem) == false)
                {
                    return false;
                }
                if (!GosBzTool.CheckDecLength(this.prodCode, d))
                {
                    return false;
                }
            }
            return true;
        }

        bool CheckTrigglePrice(string BS)
        {
            //CHECK TrigglePrice 
            string str = dgv_changeOrder.Row.getColValue("cond");
            TradeStationComm.Attribute.StopType stopType = TradeStationComm.Attribute.StopType.normalOrder;
            stopType = GosBzTool.getStopTypeByOrderBookStatus(str);
            if (stopType == TradeStationComm.Attribute.StopType.normalOrder)
            {
                return true;
            }
          
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
                    MessageBox.Show(strRs_CHOD_InValid_Stop_Price);
                    return false;
                }
                dStopPrice = txt_StopPrice_CT.get_txt_Price_NT();              
            }

            decimal LastMKPrice = 0M;
            decimal OrderPrice = getTxtPrice();
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
                        MessageBox.Show(strRs_CHOD_TrPrice_Must_High_MktPrice);
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
                        MessageBox.Show(strRs_CHOD_TrPrice_Must_Low_MktPrice);
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
                            MessageBox.Show(strRs_CHOD_TrPrice_Must_High_MktPrice);
                            return false;
                        }
                    }
                    if ((!b) && OrderPrice < dStopPrice)
                    {
                        MessageBox.Show(strRs_CHOD_StopLoss_BuyPrice_Must_EqOrMore_MktPrice);
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
                            MessageBox.Show(strRs_CHOD_SL_Sell_TrPrie_MustLow_MktPrice);
                            return false;
                        }
                    }
                    if ((!b) && OrderPrice > dStopPrice)
                    {
                        MessageBox.Show(strRs_CHOD_SL_Sell_Price_Must_EqOrLow_TrPrice);//"Sell price must be equal to or lower than trigger price.");
                        return false;
                    }
                }
            }

            return true;
        }

        void tbPriceFocus()
        {
            if (txt_Price != null)
            {
                txt_Price.Focus();
            }
        }

        private void UserControl_Unloaded_1(object sender, RoutedEventArgs e)
        {
            decTbCheck.UnBindWheel();
            distributeMsg.DisMarketPrice -= distributeMsg_DisMarketPrice;
            chOrder = null;
        }


        void SetUPDownBtnStep()
        {
            if (prodCode == null) return;
            string _prodCode = prodCode.Trim();
            if (_prodCode == null) return;
            if (_prodCode == "")
            {
                this.btnDow .Step = 1;
                this.btnUp.Step = 1;
                
                return;
            }
            int len = GosBzTool.getDecLen(_prodCode);
            if (len > 0)
            {
                decimal dec = Convert.ToDecimal(Math.Pow(10, len));
                dec = 1 / dec;
                this.btnDow.Step = dec;
                this.btnUp.Step = dec;                 
            }
            else
            {
                this.btnDow.Step = 1;
                this.btnUp.Step = 1;
            }
            return;

        }




        public void ChangLangInRuntime()
        {
            TitleStatus.ChangeTitleLang(this.distributeMsg, this.ChangeOrderChild);
        }

        ChangeOrderTitleStatus TitleStatus = new ChangeOrderTitleStatus();
        
    }

    public class ChangeOrderTitleStatus 
    {
        public string orderNo="";
        public string ProdCode="";
        public int iType = 0;
        public GOSTS.GosCulture.LabelTextExtension labelExt=GOSTS.GosBzTool.getTitleLabel(typeof(ChangeOrder));

        public void RestoreTitle(int _itype, params string[] parms)
        {
            switch (_itype)
            {
                case 0:
                    iType = _itype;
                    orderNo = "";
                    ProdCode = "";                   
                    break;
                case 1:
                    iType = _itype;
                    orderNo = GOSTS.Utility.getArrayItemValue(parms, 0);
                    ProdCode = GOSTS.Utility.getArrayItemValue(parms, 1);                   
                    break;
                default:
                    iType = _itype;
                    orderNo = "";
                    ProdCode = "";                   
                    break;
            }
        }

        public string RetrieveTitle(string strTitleFormat)
        {
            string[] Arry = strTitleFormat.Split('|');
            string str = "";
            switch (iType)
            {
                case 0:
                    str = Arry[0];
                    break;
                case 1:
                    str = string.Format(strTitleFormat, orderNo, ProdCode);
                    str = str.Replace("|", "");
                    break;
            }
            return str;
        }

        UOITitleStatus TitleStatus = new UOITitleStatus();
        public void SetFormTitle(int itype, MessageDistribute _distributeMsg, MdiChild _mdiChild, params string[] _Params)
        {
            TitleStatus.restore(itype, _Params);
            string strFormat = labelExt.Value;
            string strTitle = TitleStatus.getTitle(strFormat);
            _mdiChild.Title = strTitle;
           // GOSTS.GosBzTool.setTitle(_mdiChild, _distributeMsg, strTitle);           
        }

        public void ChangeTitleLang(MessageDistribute _distributeMsg, MdiChild _mdiChild)
        {
            string strTitleFormat = labelExt.Value;
            string strTitle = TitleStatus.getTitle(strTitleFormat);
          //  GOSTS.GosBzTool.setTitle(_mdiChild, _distributeMsg, strTitle);
            _mdiChild.Title = strTitle;
        }
    }

    interface IChangeLang
    {
        void ChangLangInRuntime();
    }

    interface ITitleManage
    {
        void SetFormTitle(int itype, MessageDistribute _distributeMsg, MdiChild _mdiChild, params string[] _Params);
        void ChangeTitleLang();
        void RestoreTitle(int _itype, params string[] parms);
        string RetrieveTitle(string strTitleFormat);
    }
}
