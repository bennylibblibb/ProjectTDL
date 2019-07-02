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
using GOSTS;
using GOSTS.Common;
using System.Data;
using System.Threading;
using System.Collections.ObjectModel;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using WPF.MDI;

namespace GOSTS
{
    /// <summary>
    /// AccountInfo20130402.xaml 的交互逻辑
    /// </summary>
    public partial class AccountInfo : IDelearUser,IChangeLang
    {
        delegate void BingUserAccountInfoData(DataTable dtAccountInfo, string _userid, string _acc, DateTime? dt);
        BingUserAccountInfoData AccountInfoData;

        delegate void BingCashInfoData(DataTable dtCashInfo, string _userid, string _acc, DateTime? dt);
        BingCashInfoData CashInfoData;

        UserData userAccountID = new UserData();
        public MdiChild mdiChild { get; set; }
      // position mk pl
        delegate void BingPositionInfoData(DataTable dtPosition, string UserID, string Acc, DateTime? dt);    
        delegate void BindListItemMarketPrice(DataTable dataTable);

        public AccountInfo(MessageDistribute _msgDistribute,MdiChild _mdiChild)
        {           
            InitializeComponent();           
            InitCurUser(this.cbbUsers, _msgDistribute);
         
            PosBus = PositionBus.GetSinglePositionBus(_msgDistribute);
            if (PosBus != null)
            {
                PosDataChange = new PosInfoChange(PositionBusDataChange);
                PosBus.RegisterOnPosInfoChange(new PositionBus.PosInfoChange(PosBus_DataChangeInvoke));
            }

            AccountInfoData = new BingUserAccountInfoData(BindAccountInfoItemMethod);
            CashInfoData = new BingCashInfoData(BindCashInfoItemMethod);
            if (this.distributeMsg != null)
            {              
                distributeMsg.DisUserAccountInfo += new MessageDistribute.OnDisUserAccountInfo(distributeMsg_DisAccountInfo);
                distributeMsg.DisCashInfo += new MessageDistribute.OnDisCashInfo(distributeMsg_DisCashInfo);           
               // distributeMsg.DisNotificationNotify += distributeMsg_DisNotificationNotify;
            }

            mdiChild = _mdiChild;
          //  SetMdITitle(_msgDistribute, mdiChild, "Account Info");
            SetFormTitle(0, _msgDistribute, mdiChild,new string[]{"",""});
            if (mdiChild != null)
            {
                mdiChild.OnDragThumbClick += new MdiChild.deleDragThumbClick(this.SetSelect);
            }

            mthTPL = new MThData<decimal?>();
            if (GOSTradeStation.isDealer == false)
            {
                tabClientInfo.Visibility = Visibility.Collapsed;
            }
        
        }
      

        bool IsSameAcc(string PushAcc)
        {
            if (PushAcc == this.CurrentID)
            {
                return true;
            }
            return false;
        }
        #region position mk price PL变化计算


        delegate void PosInfoChange(ObservableCollection<OrderPosition> posCL, PosChangeType sType,string _Acc);//object sender, ObservableCollection<OrderPosition> posCL, PosChangeType sType);
        PosInfoChange PosDataChange;
        protected void PosBus_DataChangeInvoke(object sender, ObservableCollection<OrderPosition> posCL, PosChangeType sType,string _Acc)
        {
           // Application.Current.Dispatcher.BeginInvoke(PosDataChange, new Object[] { posCL, sType,_Acc });
            PosDataChange(posCL, sType,_Acc);//
        }

        public override void initCtrlData()
        {
            base.initCtrlData();
            if (PosBus != null)
            {
                ObservableCollection<OrderPosition> posCL = PosBus.getPosCL();
                PositionBusDataChange(posCL, PosChangeType.PosChange, PosBus.AccID);
            }
        } 
       
        public void PositionBusDataChange(ObservableCollection<OrderPosition> posCL, PosChangeType sType,string _Acc)
        {           
            if (!IsSameAcc(_Acc))
            { 
                return;
            }
           
            Decimal? TotalPL = 0.00M;
           // Decimal intPL = 0;
            double usexch = GosBzTool.getExChange(DefalutCYY);
            if (sType == PosChangeType.MKChange)
            {
                if (posCL == null) return;
                if (posCL.Count < 1)
                {
                    return;
                }
            }


            if (posCL.Count < 1)
            {
                TotalPL = null;               
            }
            else
            {
                foreach (OrderPosition order in posCL)
                {
                    //2013-07-22
                    decimal decPL = order.DecPL;
                    if (order.CCY != null)
                    {
                        if (order.CCY.Trim() != "")
                        {
                            if (order.CCY.ToUpper() != DefalutCYY)
                            {
                                double exch = GosBzTool.getExChange(DefalutCYY);
                                decPL = decPL * ((decimal)exch);
                                //decPL = decPL * ((decimal)(1 / usexch));
                            }
                        }
                    }
                    TotalPL += decPL;
                }                
            }
            if (_Acc == this.CurrentID)
            {
                mthTPL.SetData(_Acc, DateTime.Now, TotalPL);
            }
            //2014-03-28 cancel
            //UpdateAccounInfo(TotalPL.ToString("F2"),NotcieDtAcc.DT);
            //SetAccDecimalFormat(NotcieDtAcc.DT);
            //NotcieDtAcc.OnPropertyChanged(new PropertyChangedEventArgs("DT"));    
            if (Monitor.TryEnter(objLockAccUpdate))
            {
                try
                {
                    UpdateAccountInfo(TotalPL);
                }
                finally
                {
                    Monitor.Exit(objLockAccUpdate);
                }
            }
            else
            {
                TradeStationLog.WriteLogPerformance("accinfo.PosChange,can not get lock to update account info");
                if (PosBus != null)
                {
                    PosBus.sendGetSVPosition(this.CurrentID);
                }               
            }
        }
        string DefalutCYY = "HKD";
        string Cyy = "HKD";
        string USCyy = "USD";
        string RMBCcy = "RMB";

        MThData<decimal?> mthTPL;
        decimal? TPL = 0M;

        class MThData<T>
        {
            string acc="";
            DateTime dt;
            T Value;
            bool bUpdate = false;

            public void SetData(string _acc, DateTime _dt, T _value)
            {
                //if (dt < _dt)
                //{
                //    return;
                //}
                
                acc = _acc;
                dt = _dt;
                Value = _value;
            }

            public T getData(string CurAccID)
            {
                if (acc == null) return default(T);
                if (acc.Trim() == "") return default(T);
                if (CurAccID == acc)
                {
                    return Value;
                }
                return default(T);
            }
        }


        void UpdateAccountInfo(decimal? TPL)
        {
            DataTable dt = null;
            if(NotcieDtAcc.DT!=null)
            {
                dt=NotcieDtAcc.DT.Copy();
            }
           
            if (TPL.HasValue)
            {
                UpdateAccounInfo(TPL.Value.ToString("F2"),dt);// NotcieDtAcc.DT);
            }
            else
            {
                UpdateAccounInfo("",dt);//NotcieDtAcc.DT);
            }
            SetAccDecimalFormat(dt);// (NotcieDtAcc.DT);
            if (dt != null)
            {
                Dispatcher.Invoke((Action)(() =>
                {
                    dt.DefaultView.RowFilter = ("name<>'Credit Limit' and name<>'Period' and name<>'Max Margin'");
                    this.dg_AccountInfo.DataContext = dt.DefaultView;
                }), null);
            }
            else
            {
                Dispatcher.Invoke((Action)(() =>
                {
                    this.dg_AccountInfo.DataContext = null;
                }), null);
                
            }
          //  NotcieDtAcc.OnPropertyChanged(new PropertyChangedEventArgs("DT"));   
        }

        //计算account info 一些数据
        
        void UpdateAccounInfo(string TotalPL,DataTable dtAccount)
        {
            //System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace(true);
            if (dtAccount == null) return;
            if (dtAccount.Rows.Count < 1) return;
            string strCashBalance = "0", strIMargin = "0";
            decimal DCashBalance = 0.00M, DIMargin = 0.00M, DTotalPL = 0.00M, DNav = 0.00M, BP = 0.00M;

            DataRow drE;
            DataRow[] drs;// = dtAccount.Select("name='NAV'");
            if (dtAccount.Columns.Contains("name") == false || dtAccount.Columns.Contains("value") == false)
            {
                return;
            }
            foreach (DataRow dr in dtAccount.Rows)
            {
                string sName = Utility.getColumnValue(dr, "name");
                if (sName.ToLower() == "commodity p/l")
                {
                    if (TotalPL == "")
                    {
                        TotalPL = dr["value"].ToString().Trim().Replace(Cyy, "").Replace(USCyy, "").Replace(RMBCcy, "").Replace(",", "");
                        DTotalPL = Utility.ConvertToDecimal(TotalPL);
                    }
                    else
                    {
                        DTotalPL = Utility.ConvertToDecimal(TotalPL);
                    }
                }
                if (sName.ToLower() == "cash balance")
                {
                    strCashBalance = Utility.getColumnValue(dr, "value").Replace(Cyy, "").Replace(USCyy, "").Replace(RMBCcy, "").Replace(",", "");
                    DCashBalance = Utility.ConvertToDecimal(strCashBalance);
                }
                if (sName.ToLower() == "i.margin")
                {
                    strIMargin = Utility.getColumnValue(dr, "value").Replace(Cyy, "").Replace(USCyy, "").Replace(RMBCcy, "").Replace(",", "");
                    DIMargin = Utility.ConvertToDecimal(strIMargin);
                }
            }
            //update
            foreach (DataRow dr in dtAccount.Rows)
            {
                string sName = Utility.getColumnValue(dr, "name");
                if (sName.ToLower() == "commodity p/l")
                {
                    dr.BeginEdit();
                    dr["value"] = DTotalPL.ToString("F2") + " " + Cyy; //TotalPL
                    dr.EndEdit();
                    break;
                }
            }

            drs = dtAccount.Select("name='NAV'");
            if (drs.Length > 0)
            {
                drE = drs[0];
                drE.BeginEdit();
                DNav = DTotalPL + DCashBalance;
                drE["value"] = DNav.ToString() + " " + Cyy;
                drE.EndEdit();
            }           
           
            decimal DBuyPower=0.0M;
            drs = dtAccount.Select("name='Buying Power'");
            if (drs.Length > 0)
            {
                drE = drs[0];
                drE.BeginEdit();
                decimal d = DNav - DIMargin;
                DBuyPower=d;
                drE["value"] = d.ToString() + " " + Cyy;
                drE.EndEdit();
               // TradeStationLog.WriteQCHarry(DateTime.Now.ToString("yyMMdd-HHmmss ") + "Nav> " + DNav + ",DIMargin->" + DIMargin.ToString() + "," + ",DBuyPower->" + DBuyPower.ToString() + st.GetFrame(1).GetMethod().Name.ToString());
           
            }
            //TradeStationLog.WriteQCHarry(DateTime.Now.ToString("yyMMdd-HHmmss ") + "Nav> " + DNav + ",TotalPL->" + DTotalPL.ToString() + "," + ",DCashBalance->" + DCashBalance.ToString() + st.GetFrame(1).GetMethod().Name.ToString());
            //TradeStationLog.WriteQCHarry(DateTime.Now.ToString("yyMMdd-HHmmss ") + "iMARGIN> " + DIMargin + " , " + st.GetFrame(1).GetMethod().Name.ToString());
           
            drs = dtAccount.Select("name='M.Level'");
            if (drs.Length > 0)
            {
                drE = drs[0];
                drE.BeginEdit();
                if (DIMargin == 0.00M)
                {
                    drE["value"] = "0 ";
                }
                else
                {
                    decimal d = (DNav / DIMargin) * 100;
                    drE["value"] = d.ToString("F2") + " %";
                }
                drE.EndEdit();
            }

            decimal DMMargin = 0.0M;
           
            drs = dtAccount.Select("name='M.Margin'");
            if (drs.Length > 0)
            {
                drE = drs[0];
                string strMMargin = drE.getColValue("value");                
                if (strMMargin != "")
                {
                    try
                    {
                        strMMargin = System.Text.RegularExpressions.Regex.Replace(strMMargin, @"[^\-\d\.]", "");
                        try
                        {
                            DMMargin = Convert.ToDecimal(strMMargin);
                        }
                        catch(Exception ex) {
                            TradeStationLog.WriteError(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "  UpdateAccounInfo, convert to decimal mmargin,source DMMargin number:" + strMMargin + ",  " + ex);
                        }
                        bool bBpToMC = false;
                        if (DMMargin > DNav)
                        {
                            if (DBuyPower < 0)
                            {
                                bBpToMC = true;                               
                            }
                        }
                        drs = dtAccount.Select("name='Margin Call'");
                        if (drs.Length > 0)
                        {
                            drE = drs[0];
                            drE.BeginEdit();
                            if (bBpToMC)
                            {
                                drE["value"] = Math.Abs(DBuyPower);
                            }
                            else
                            {
                                drE["value"] =0;
                            }
                            drE.EndEdit();
                        }                       
                    }
                    catch(Exception ex) {
                        TradeStationLog.WriteError(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "  UpdateAccounInfo," + ex);
                    }
                }
            }
            decimal DMarginCall = 0.0M;
            drs = dtAccount.Select("name='Margin Call'");
            if (drs.Length > 0)
            {
                drE = drs[0];
                string strDMCall = drE.getColValue("value");
                strDMCall = System.Text.RegularExpressions.Regex.Replace(strDMCall, @"[^\-\d\.]", "");
                try
                {
                    DMarginCall = Convert.ToDecimal(strDMCall);
                }
                catch (Exception ex)
                {
                    TradeStationLog.WriteError(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "  UpdateAccountInfo, convert to decimal,source number:" +strDMCall.ToString()+"  "+ex );
                }
            }
            switchBGColor(DBuyPower.ToString(), DMarginCall);
        }

        void SetAccDecimalFormat(DataTable dtAccount)
        {
            // return;
            if (dtAccount == null) return;
            if (dtAccount.Rows.Count < 1) return;
            string strName = "", strValue, strF;
            Decimal D = 0.00M;
            string strList = ",buying power,nav,margin call,commodity p/l,i.margin,m.margin,cash balance,credit limit,";
            if (dtAccount.Columns.Contains("name") && dtAccount.Columns.Contains("value"))
            foreach (DataRow dr in dtAccount.Rows)
            {
                dr.BeginEdit();
                strName = dr["name"].ToString().Trim().ToLower();
                if (strList.IndexOf("," + strName + ",") >= 0)
                {
                    if (strName.IndexOf("buying power") > -1)
                    {

                    }
                    strValue = Utility.getColumnValue(dr, "value").Replace(Cyy, "").Replace(USCyy, "").Replace(RMBCcy, "").Replace(",", "").Trim();
                    try
                    {
                        D = Utility.ConvertToDecimal(strValue);
                    }
                    catch (Exception ex)
                    {
                        TradeStationLog.WriteError(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "  UpdateAccountInfo, convert to decimal,source number:" + strValue + "  " + ex);
                    }

                    strF = string.Format("{0:N}", D);
                    dr["value"] = strF + " " + Cyy;
                }
                dr.EndEdit();
            }
        }

        //NameValueCollection CashFileds = new NameValueCollection();
        void SetCashFormat()
        {
            //return;
            if (dtCash == null) return;
            if (dtCash.Rows.Count < 1) return;
            string strName = "", strValue, strF;
            Decimal D = 0.00M;
            string strList = ",cash bf.,unsettle,today in/out,withdrawal req,cash,unpresented,cash(base ccy),";
            if (dtCash.Columns.Contains("name") && dtCash.Columns.Contains("value"))
            {
                foreach (DataRow dr in dtCash.Rows)
                {
                    dr.BeginEdit();
                    strName = dr["name"].ToString().Trim().ToLower();
                    if (strList.IndexOf("," + strName + ",") >= 0)
                    {
                        strValue = Utility.getColumnValue(dr, "value").Replace(Cyy, "").Replace(USCyy, "").Replace(RMBCcy, "").Replace(",", "").Trim();
                        D = Utility.ConvertToDecimal(strValue);
                        strF = string.Format("{0:N}", D);
                        dr["value"] = strF;// +" " + Cyy;
                    }
                    dr.EndEdit();
                }
            }
        }

        #endregion

        void distributeMsg_DisNotificationNotify(object sender, Notification notification)
        {
            switch (notification.Notify_Code)
            {
                case 2:
                    TradeStationSend.Send(cmdClient.getCashInfo,  notification.Acc_no);
                    TradeStationSend.Send(cmdClient.getAccountInfo,   notification.Acc_no);
                    break;
            }
        }


     

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }

        protected void distributeMsg_DisAccountInfo(object sender, DataTable dtAccount, string _userid, string _acc, DateTime? _dt)
        {
            //Application.Current.Dispatcher.Invoke(AccountInfoData, new object[] { dtAccount, _userid, _acc, _dt });
            AccountInfoData(dtAccount, _userid, _acc, _dt);
        }

        protected void distributeMsg_DisCashInfo(object sender, DataTable dtCashInfo, string _userid, string _acc, DateTime? dt)
        {
            Application.Current.Dispatcher.Invoke(CashInfoData, new object[] { dtCashInfo, _userid, _acc, dt });
        }

        public void BindAccountInfoItemMethod(DataTable dtAccountInfo, string _userid, string _acc, DateTime? dt)
        {
            bingAccount(dtAccountInfo, _userid, _acc, dt);
        }

        public void BindCashInfoItemMethod(DataTable dtCashInfo, string _userid, string _acc, DateTime? _dt)
        {
            bingCash(dtCashInfo, _userid, _acc, _dt);
        }
     
        NoticeChangeTable NotcieDtAcc = new NoticeChangeTable();//set to itemsSource of account info
        object objLockAccUpdate = new object();
        private void bingAccount(DataTable dtAccountInfo, string _userid, string _acc, DateTime? _dt)
        {
            if (_userid == null || _userid.Trim() == "" || GOSTradeStation.UserID == "" || _userid.Trim() != GOSTradeStation.UserID)
            {
                return;
            }
            if (GOSTradeStation.isDealer)
            {
                if (this.CurrentID == null || this.CurrentID == "") return;
                if (_acc == null || _acc.Trim() == "" || _acc.Trim() != this.CurrentID)
                {
                    return;
                }
            }
            if (dtAccountInfo != null)
            {
                if (dtAccountInfo.Columns.Contains("name"))
                {
                    DataRow[] drs = dtAccountInfo.Select("name='ae'");
                    string AEName = "";
                    if (drs.Length > 0)
                    {
                        DataRow dr = drs[0];
                        if (dr.Table.Columns.Contains("value"))
                        {
                            AEName = dr["value"].ToString().Trim();
                        }
                    }
                    if (AEName != "" && AEName != "-")
                    {                       
                       // SetMdITitle(distributeMsg, mdiChild, "Account Info  - " + this.CurrentID.ToString() + " AE:" + AEName);
                        SetFormTitle(2, distributeMsg, mdiChild,new string[]{this.CurrentID.ToString(), AEName});
                    }
                    drs = dtAccountInfo.Select("name='M.Level'");
                    if (drs.Length > 0)
                    {
                        DataRow dr = drs[0];
                        if (dr.Table.Columns.Contains("value"))
                        {
                            string strML=dr.getColValue("value");
                            if (strML == "")
                            {
                                strML="0";
                            }
                            strML=System.Text.RegularExpressions.Regex.Replace(strML,@"[^\-\d\.]","");
                            decimal decML=0M;
                            try{
                                decML = Convert.ToDecimal(strML);
                            }
                            catch(Exception ex){
                                TradeStationLog.WriteError(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "  bingAccount, convert to decimal,source number:" + strML + "  " + ex);
                            }
                            strML = decML.ToString("F2") + " %";
                            dr.BeginEdit();
                            dr["value"] = strML;
                            dr.EndEdit();
                        }
                    }

                     drs = dtAccountInfo.Select("name='ctrl level'");
                     if (drs.Length > 0)
                     {
                         DataRow dr = drs[0];
                         if (dr.Table.Columns.Contains("value"))
                         {
                             string ctrl = dr.getColValue("value").Trim();
                             if (ctrl != "")
                             {
                                 if (ctrl.Length % 4 == 0)
                                 {
                                     try
                                     {
                                         ctrl = TradeStationTools.Base64StringToString(ctrl);
                                         dr.BeginEdit();
                                         dr["value"] = ctrl;
                                         dr.EndEdit();
                                     }
                                     catch {  }
                                     
                                 }
                             }
                         }
                     }
                   // dtAccountInfo.DefaultView.RowFilter = ("name<>'Credit Limit' and name<>'Period' and name<>'Max Margin'");
                }
            }

            dtAccountInfo = GosDataHelper.dtAcc_AddLangCol(dtAccountInfo);

            #region writelog
             //if (dtAccountInfo != null)
             //{
             //    if(dtAccountInfo.Columns.Contains("name") && dtAccountInfo.Columns.Contains("value"))
             //    {
             //        TradeStationLog.WriteQCHarry(DateTime.Now.ToString("yyMMdd-HHmmss Receive Data:---------------")) ;
             //        foreach(DataRow dr in dtAccountInfo.Rows)
             //        {
             //            TradeStationLog.WriteQCHarry("name="+dr.getColValue("name")+",value="+dr.getColValue("value"));
           
             //        }
             //         TradeStationLog.WriteQCHarry(DateTime.Now.ToString("yyMMdd-HHmmss Receive Data End---------------")) ;
             //    }
             //}
            
            

            #endregion

            if (Monitor.TryEnter(objLockAccUpdate))
            {
                try
                {
                    NotcieDtAcc.DT = dtAccountInfo.Copy();
                    decimal? TPL = mthTPL.getData(this.CurrentID);
                    UpdateAccountInfo(TPL);
                }
                catch (Exception ex)
                {
                    TradeStationLog.WriteError("accinfo bingAcc ,update account info," + ex);
                }
                finally
                {
                    Monitor.Exit(objLockAccUpdate);
                }
 //               TradeStationLog.WriteLogPerformance("acountinfo.bingAccount,success get lock to update account info");
            }
            else
            {
                TradeStationLog.WriteLogPerformance("accinfo bingAcc,can not get lock to update accinfo");
                TradeStationSend.Send(cmdClient.getAccountInfo, this.CurrentID);
            }
  #region 2014-03-28 
          //  bool bUpdateFromPos = false;
          //  if (this.PosBus != null)
          //  {
          //      if (PosBus.AccID == this.CurrentID)
          //      {
          //          if (PosBus.posCollection != null)
          //          {
          //              if (PosBus.posCollection.Count > 0)
          //              {
          //                  PositionBusDataChange(PosBus.posCollection, PosChangeType.PosChange, this.CurrentID);
          //                  bUpdateFromPos = true;
          //              }
          //          }
          //      }
          //  }
          //  if (!bUpdateFromPos)
          //  {
              
          //      //cancel
          //      SetAccDecimalFormat(NotcieDtAcc.DT);
          //      if (NotcieDtAcc != null)
          //      {
          //          if (NotcieDtAcc.DT != null)
          //          {
          //              if (NotcieDtAcc.DT.Columns.Contains("name"))
          //              {
          //                  decimal DMMargin = 0M, DNav = 0M;
          //                  DataRow[] drs = dtAccountInfo.Select("name='M.Margin'");
          //                  if (drs.Length > 0)
          //                  {
          //                      DataRow drE = drs[0];
          //                      string strMMargin = drE.getColValue("value");
          //                      strMMargin = System.Text.RegularExpressions.Regex.Replace(strMMargin, @"[^\-\d\.]", "");
          //                      try
          //                      {
          //                          DMMargin = Convert.ToDecimal(strMMargin);
          //                      }
          //                      catch (Exception ex)
          //                      {
          //                          TradeStationLog.WriteError(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "  bingAccount, convert to decimal,source mmargin number:" + strMMargin + "  " + ex);
          //                      }
          //                  }
          //                  drs = dtAccountInfo.Select("name='NAV'");
          //                  if (drs.Length > 0)
          //                  {
          //                      DataRow drE = drs[0];
          //                      string strNav = drE.getColValue("value");
          //                      strNav = System.Text.RegularExpressions.Regex.Replace(strNav, @"[^\-\d\.]", "");
          //                      try
          //                      {
          //                          DNav = Convert.ToDecimal(strNav);
          //                      }
          //                      catch (Exception ex)
          //                      {
          //                          TradeStationLog.WriteError(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "  bingAccount, convert to decimal,source nav number:" + strNav + "  " + ex);
          //                      }
          //                  }
          //                  decimal DBuyPower = 0.0M;
          //                  string bp = "0";
          //                  drs = dtAccountInfo.Select("name='Buying Power'");
          //                  if (drs.Length > 0)
          //                  {
          //                      DataRow drE = drs[0];
          //                      bp = drE.getColValue("value");
          //                      bp = System.Text.RegularExpressions.Regex.Replace(bp, @"[^\-\d\.]", "");
          //                      try
          //                      {
          //                          DBuyPower = Convert.ToDecimal(bp);
          //                      }
          //                      catch (Exception ex)
          //                      {
          //                          TradeStationLog.WriteError(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "  bingAccount, convert to decimal,source bp number:" + bp + "  " + ex);
          //                      }
          //                  }


          //                  decimal DMarginCall = 0M;
          //                  drs = NotcieDtAcc.DT.Select("name='Margin Call'");
          //                  if (drs.Length > 0)
          //                  {
          //                      DataRow drE = drs[0];
          //                      drE.BeginEdit();
          //                      if (DMMargin > DNav)
          //                      {
          //                          DMarginCall = Math.Abs(DBuyPower); ;

          //                      }

          //                      drE["value"] = DMarginCall;
          //                      drE.EndEdit();
          //                  }




          //                  //string bp = "0";
          //                  //drs = NotcieDtAcc.DT.Select("name='Buying Power'");
          //                  //if (drs.Length > 0)
          //                  //{
          //                  //    DataRow drE = drs[0];
          //                  //    bp = Utility.getColumnValue(drE, "value");

          //                  //}
          //                  //decimal DMarginCall = 0.0M;
          //                  //drs = NotcieDtAcc.DT.Select("name='Margin Call'");
          //                  //if (drs.Length > 0)
          //                  //{
          //                  //    DataRow drE = drs[0];
          //                  //    string strDMCall = drE.getColValue("value");
          //                  //    strDMCall = System.Text.RegularExpressions.Regex.Replace(strDMCall, @"[^\d\.]", "");
          //                  //    DMarginCall = Convert.ToDecimal(strDMCall);                             
          //                  //}
          //                  switchBGColor(bp, DMarginCall);
          //              }
          //          }
          //      }



             
          //  }
           
          ////  NotcieDtAcc.OnPropertyChanged(new PropertyChangedEventArgs("DT")); 
  #endregion
           
        }

        DataTable dtCash;
        private void bingCash(DataTable dtCashInfo, string _userid, string _acc, DateTime? _dt)
        {
            if (dtCashInfo == null)
            {
                this.dg_CashInfo.DataContext = null;
                return;
            }
            if (_userid == null || _userid.Trim() == "" || GOSTradeStation.UserID == "" || _userid.Trim() != GOSTradeStation.UserID)
            {
                return;
            }
            if (GOSTradeStation.isDealer)
            {
                if (this.CurrentID == null || this.CurrentID == "") return;
                if (_acc == null || _acc.Trim() == "" || _acc.Trim() != this.CurrentID)
                {
                    return;
                }
            }
            if (dtCashInfo != null)
            {
                dtCash = dtCashInfo;
                SetCashFormat();
                dtCash = GosDataHelper.dtCash_AddLangCol(dtCash);
                this.dg_CashInfo.DataContext = dtCash.DefaultView;
            }
        }

        public override void IDelearUser_OnUserChange(string _CurUserID)
        {
            if (GOSTradeStation.isDealer)
            {              
                TradeStationSend.Send(cmdClient.getCashInfo, _CurUserID);
                TradeStationSend.Send(cmdClient.getAccountInfo, _CurUserID);
            }
            else
            {
                TradeStationSend.Send(cmdClient.getCashInfo);
                TradeStationSend.Send(cmdClient.getAccountInfo);

            }
        }


        bool? bAccInfoColourRed = null; //flag variable
        void switchBGColor(string bp,decimal marginCall )
        {
            Dispatcher.Invoke((Action)(() =>
            {
                bp = bp.Replace(Cyy, "").Replace(",", "").Replace(RMBCcy,"").Replace(USCyy,"");
                decimal buypower = 0.0M;
                try
                {
                    buypower = Convert.ToDecimal(bp);
                }
                catch(Exception ex) {
                    TradeStationLog.WriteError(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "  switchBGColor, convert to decimal buypower,source bp number:" + bp + "  " + ex);
                }
                bool bRed = false;
                if (buypower < 0)
                {
                    if (GOSTS.AppFlag.AccountInfoBGFlag == 1)
                    {
                        if (marginCall > 0)
                        {
                            bRed = true;
                        }
                    }
                    else
                    {
                        bRed = true;
                    }
                }
               
                if (bRed)
                {
                    if (bAccInfoColourRed==null || bAccInfoColourRed == false)
                    {
                       dg_AccountInfo.Background = Brushes.Red;
                       bAccInfoColourRed = true;
                       setDgCellStyle(dg_AccountInfo, true);      
                    }
                }
                else
                {
                    if (bAccInfoColourRed == null || bAccInfoColourRed==true)
                    {
                       dg_AccountInfo.Background = Brushes.White;
                       bAccInfoColourRed = false;
                       setDgCellStyle(dg_AccountInfo, false);                   
                    }
                }
            }),
            null);
            //System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace(true);
            //TradeStationLog.WriteQCHarry(DateTime.Now.ToString("yyMMdd-HHmmss ") + "red?:" + (bRed ? "Y" : "N") + "   BP-> " + bp + "MARGIN->" + marginCall.ToString() + "," + st.GetFrame(1).GetMethod().Name.ToString());
           
        }      

        public void setDgCellStyle(DataGrid dg,bool bRed)
        {
            if (bRed)
            {
                object o = dg.FindResource("dgcellStyle_withBgTransparent");
                Style s = o as Style;
                if (s != null)
                {
                    dg.CellStyle = s;
                }
            }
            else
            {
                object o = dg.FindResource("dgCellStyle_WithBgGrandient");
                Style s = o as Style;
                if (s != null)
                {
                    dg.CellStyle = s;
                }
            }

        }
       

        public override void ClearControlData()
        {
            this.dg_CashInfo.DataContext = null;
           // GosTradeTest.TestExcelLog.WriteLog("form accountinfo","clear ui cash info at clearControlData,cur select id"+ this.CurrentID + "");           
             
            this.dg_acMast.ItemsSource = null;
          //  GosTradeTest.TestExcelLog.WriteLog("form accountinfo","clear ui client info at clearControlData,cur select id" + this.CurrentID + "");           
            
            NotcieDtAcc.DT = null;    
            NotcieDtAcc.OnPropertyChanged(new PropertyChangedEventArgs("DT"));
            dg_AccountInfo.Background = Brushes.White;
            bAccInfoColourRed = false;  // kenlo20150422 fix bug when changing ac from red to red, by harry
        //    GosTradeTest.TestExcelLog.WriteLog("form accountinfo","clear ui account info at clearControlData, cur select id" + this.CurrentID + "");           
                
            this.dg_AccountInfo.DataContext = null;
            if (mdiChild != null)
            {               
               // SetMdITitle(distributeMsg, mdiChild,  "Account Info   ");
                SetFormTitle(0, distributeMsg, mdiChild,new string[]{"",""});
                base.ClearControlData();
            }
        }

        #region get acc mast
       
        public override void GetAccountMaster(object sender, DataTable tableMaster,string RecAcc)
        {
            string FGuidID = GosTestGlobal.GetBlockGuid();
         
            base.GetAccountMaster(sender, tableMaster, RecAcc);           
            if (RecAcc != this.CurrentID)
            {             
                return;
            }
           
            if (tableMaster == null)
            {
                this.dg_acMast.ItemsSource = null;
                this.dg_acMast.Items.Clear();
                if (GosTestGlobal.bInTest)
                {
                    GosTradeTest.TestExcelLog.WriteLog(enumItemType.getMast.ToString(), @"(AccountInfo.GetAccountMaster_" + FGuidID + "),receive null date table,clear ui client info and return, invoke base.GetAccountMaster,rec Acc:" + (RecAcc != null ? RecAcc : "") + ",current select Id:" + this.CurrentID);
                }
                return;
            }
            if (tableMaster.Rows.Count < 1)
            {
                if (GosTestGlobal.bInTest)
                {
                    GosTradeTest.TestExcelLog.WriteLog(enumItemType.getMast.ToString(), @"(AccountInfo.GetAccountMaster_" + FGuidID + "),receive 0 rows count date table,clear ui client info and return,(AccountInfo.GetAccountMaster_" + FGuidID + "), invoke base.GetAccountMaster,rec Acc:" + (RecAcc != null ? RecAcc : "") + ",current select Id:" + this.CurrentID);
                }
                this.dg_acMast.ItemsSource = null;
                this.dg_acMast.Items.Clear();
                return;
            }
            DataRow[] drs=tableMaster.Select("accNO='"+this.CurrentID+"'");
            if (drs.Length < 1)
            {
               // this.dg_acMast.Items.Clear();
                if (GosTestGlobal.bInTest)
                {
                    GosTradeTest.TestExcelLog.WriteLog(enumItemType.getMast.ToString(), @"(AccountInfo.GetAccountMaster_" + FGuidID + "),can not find  current select Id:" + this.CurrentID);
                }
                this.dg_acMast.ItemsSource = null;                
                return;
            }
            DataTable dt = acMastTable(tableMaster);
            if (dt == null)
            {
                this.dg_acMast.ItemsSource = null;
                return;
            }
            if (mdiChild != null)
            // if (GOSTradeStation.isDealer)
            {
                      
                //if (mdiChild.Title.IndexOf("Account Info  - " + this.CurrentID.ToString() + " AE:") < 0)
                //{                    
                //    SetMdITitle(distributeMsg, mdiChild, "Account Info  - " + this.CurrentID.ToString());
                //}
                SetFormTitle( 1, distributeMsg, mdiChild,new string[]{this.CurrentID.ToString(), ""});
            }
            dt = GosDataHelper.dtAccClientInfo_AddLangCol(dt);
            dt.DefaultView.RowFilter = "name<>'creditLimit' and name<>'maxMargin'";
            this.dg_acMast.ItemsSource = dt.DefaultView;
        }

       
        DataTable acMastTable(DataTable tableMaster)
        {
            if (tableMaster == null) return null;
            if (tableMaster.Rows.Count < 1) return null;
            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("name"));
            dt.Columns.Add(new DataColumn("value"));

            DataRow[] drs= tableMaster.Select("accNO='"+this.CurrentID+"'");
            if (drs.Length < 0)
            {               
                return null;
            }
            DataRow drMast = drs[0];        
            foreach (DataColumn col in tableMaster.Columns)
            {
                string strValue = drMast.getColValue(col.ColumnName);
            
                {
                    if (col.ColumnName.ToLower() == "sex")
                    {
                        strValue = FormatSex(strValue);
                    } 
                    if (col.ColumnName.ToLower() == "accname")
                    {
                        strValue = TradeStationTools.Base64Utf16StringToString(strValue);
                    }
                    if (col.ColumnName.ToLower() == "active")
                    {
                        strValue = getActiveText(strValue);
                    }
                    if (col.ColumnName.ToLower() == "address")
                    {
                        strValue = TradeStationTools.Base64Utf16StringToString(strValue);
                    }
                    if (col.ColumnName.ToLower() == "contactinfo")
                    {
                        strValue = TradeStationTools.Base64Utf16StringToString(strValue);
                    }
                    //if (col.ColumnName.ToLower() == "home" || col.ColumnName.ToLower() == "office" || col.ColumnName.ToLower() == "mobile" )
                    //{
                    //    strValue = TradeStationTools.Base64Utf16StringToString(strValue);
                    //}

                    DataRow drNew = dt.NewRow();
                    drNew["name"] = col.ColumnName;
                    drNew["value"] = strValue;
                    dt.Rows.Add(drNew);
                }
            }
            return dt;            
        }
        public string getActiveText(string a)
        {
            switch (a)
            {
                case "1":
                    return "active";
                case "0":
                    return "inactive";
            }
            return "";
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

        private void IDelearUser_Unloaded_1(object sender, RoutedEventArgs e)
        {
            if (distributeMsg != null)
            {              
                distributeMsg.DisUserAccountInfo -= new MessageDistribute.OnDisUserAccountInfo(distributeMsg_DisAccountInfo);
                distributeMsg.DisCashInfo -= new MessageDistribute.OnDisCashInfo(distributeMsg_DisCashInfo);
               // UnRegisterReceiveAccMaster();
              //  distributeMsg.DisNotificationNotify -= distributeMsg_DisNotificationNotify;
            }
            if (PosBus != null)
            {
                PosBus.UnRegisterOnPosInfoChange(new PositionBus.PosInfoChange(PosBus_DataChangeInvoke));
            }
            base.IDelearUser_Unloaded_1(sender, e);
        }

        private void btnTest_Click_1(object sender, RoutedEventArgs e)
        {
            IDelearStatus.CloseMsgBoxWindows();
        }


      //2014-10-10
        private int IType=0;
        private string[] ParamTitles;
        private MessageDistribute _TitleMsgDist;
        private MdiChild _TitleMdiChild;
        public override void SetFormTitle(int itype, MessageDistribute _distributeMsg,MdiChild _mdiChild,params string[] Params)
        {
          
            try
            {
                string str = "";
                string[] Arry = this.strTitleFormat.Split('|');
                switch (itype)
                {
                    case 0:
                        str = this.strTitleFormat.Split('|')[0];

                        ParamTitles = Params;
                        _TitleMsgDist = _distributeMsg;
                        _TitleMdiChild = _mdiChild;
                        IType = itype;

                        SetMdITitle(_distributeMsg, _mdiChild, str);
                        break;
                    case 1:
                        str = Arry[0] + string.Format(Arry[1], Params[0]);
                        if (this.mdiChild.Title.IndexOf(str) < 0)
                        {
                            ParamTitles = Params;
                            _TitleMsgDist = _distributeMsg;
                            _TitleMdiChild = _mdiChild;
                            IType = itype;

                            SetMdITitle(_distributeMsg, _mdiChild, str);                          
                        }                        
                        break;
                    case 2:
                        str = string.Format(this.strTitleFormat, Params[0], Params[1]);
                        str = str.Replace("|", "");

                        ParamTitles = Params;
                        _TitleMsgDist = _distributeMsg;
                        _TitleMdiChild = _mdiChild;
                        IType = itype;

                        SetMdITitle(_distributeMsg, _mdiChild, str);                                             

                        break;
                    default:
                        str = this.strTitleFormat.Split('|')[0];

                         ParamTitles = new string[]{"",""};
                        _TitleMsgDist =this.distributeMsg;
                        _TitleMdiChild = this.mdiChild;
                        IType = 0;

                        SetMdITitle(_TitleMsgDist, _TitleMdiChild, str);
                        break;

                }
            }
            catch (Exception ex)
            {
            }
        }

        public override void SetFormTitle()
        {
            SetFormTitle(IType, this.distributeMsg, this.mdiChild, ParamTitles);
        }


        public void ChangLangInRuntime()
        {
            bAccInfoColourRed = null;
        }
    }

    public class NoticeChangeTable : INotifyPropertyChanged
    {
        private DataTable _DT;
        public DataTable DT
        {
            get { return _DT; }
            set { _DT = value; OnPropertyChanged(new PropertyChangedEventArgs("DT")); }
        }

        private string _acc = "";
        public string Acc { 
            get { return _acc;} 
            set{_acc=value;} 
        }

        public void StoreTable(string _acc, DataTable _dt)
        {
            DT = _dt;
            Acc = _acc;
        }

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(PropertyChangedEventArgs propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, (propertyName));
            }
        }
        #endregion
    }

    public class N0Format : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return "";
            try
            {
                decimal D = System.Convert.ToDecimal(value.ToString());
                return string.Format("{0:N}", D);
            }
            catch { }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }

    public class DecCommaFormat : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return "";
            try
            {
                string str=value.ToString().Trim();
                decimal D = System.Convert.ToDecimal(str);
                int pos = str.IndexOf(".");
                if (pos < 0)
                {
                    return string.Format("{0:N0}", D);
                }
                else
                {
                    int len = str.Length - 1 - pos;
                    if (len >= 0)
                    {
                        return string.Format("{0:N"+len.ToString()+"}", D);
                    }
                }
            }
            catch { }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }

    public class GosDataHelper
    {
        public static DataTable dtAcc_AddLangCol(DataTable dt)
        {
            if (dt == null) return dt;
            if (dt.Columns.Count < 1) return dt;
             if (dt.Columns.Contains("name") == false)
             {
                 return dt;
             }
             if (dt.Columns.Contains("sname") == false)
            {
                dt.Columns.Add(new DataColumn("sname"));
            }
            else{
                return dt;
            }
            foreach (DataRow dr in dt.Rows)
            {   //AppFlag.DefaultLanguage
                string name = dr.getColValue("name");
                name = name.Replace(" ", "").Replace(".", "").Replace("/","").ToLower();
                name = "AccInfo_TabAcc_" + name;
                //GosCulture.LabelTextExtension lb = new GosCulture.LabelTextExtension(name);
                //string sValue = lb.Value;               
                dr.BeginEdit();
                dr["sname"] = GosCulture.CultureHelper.GetString(name);
                dr.EndEdit();
                continue;
               
            }
            return dt;
        }


        public static DataTable dtCash_AddLangCol(DataTable dt)
        {
            if (dt == null) return dt;
            if (dt.Columns.Count < 1) return dt;
            if (dt.Columns.Contains("name") == false)
            {
                return dt;
            }
            if (dt.Columns.Contains("sname") == false)
            {
                dt.Columns.Add(new DataColumn("sname"));
            }
            else
            {
                return dt;
            }
            foreach (DataRow dr in dt.Rows)
            {  
                string name = dr.getColValue("name");
                name = name.Replace(" ", "").Replace(".", "").Replace("/", "").ToLower();
                name = "AccInfo_TabCash_" + name;      
                dr.BeginEdit();
                dr["sname"] = GosCulture.CultureHelper.GetString(name);
                dr.EndEdit();
                continue;

            }
            return dt;
        }


        public static DataTable dtAccClientInfo_AddLangCol(DataTable dt)
        {
            if (dt == null) return dt;
            if (dt.Columns.Count < 1) return dt;
            if (dt.Columns.Contains("name") == false)
            {
                return dt;
            }
            if (dt.Columns.Contains("sname") == false)
            {
                dt.Columns.Add(new DataColumn("sname"));
            }
            else
            {
                return dt;
            }
            foreach (DataRow dr in dt.Rows)
            {
                string name = dr.getColValue("name");
                name = name.Replace(" ", "").Replace(".", "").Replace("/", "").ToLower();
                name = "AccInfo_TabCli_" + name;
                dr.BeginEdit();
                dr["sname"] = GosCulture.CultureHelper.GetString(name);
                dr.EndEdit();
                continue;

            }
            return dt;
        }
    }
}
