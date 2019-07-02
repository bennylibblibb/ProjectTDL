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

namespace GOSTS
{
    public enum PosChangeType
    {
        PosChange,
        MKChange
    }
    /// <summary>
    /// 
    /// 用到外部变量GOSTradeStation.isDealer
    /// </summary>
    public class PositionBus
    {

        public ObservableCollection<OrderPosition> posCollection = new ObservableCollection<OrderPosition>();
        public ObservableCollection<OrderPosition> getPosCL()
        {
            return posCollection;
        }

        public OrderPosition FindPosotionByProd(string prod)
        {
            if (posCollection == null)
            {
                return null;
            }
            if (posCollection.Count < 0)
            {
                return null;
            }
            OrderPosition position=this.posCollection.FirstOrDefault(x => x.ProductCode.ToLower() == prod.ToLower());
            return position;
        }

        public void OpenClosePos(string prod, string _acc, MessageDistribute _distributeMsg)
        {
            OrderPosition pos = FindPosotionByProd(prod);
            if (pos == null)
            {
                return;
            }
            if (_acc == null)
            {
                return;
            }
            if (this.AccID != null)
            {
                if (_acc == this.AccID)
                {
                    if (pos.bCanClose)
                    {
                        MdiContainer Container = App.Current.MainWindow.FindName("Container") as MdiContainer;
                        MdiChild ClosePositionChild = new MdiChild();
                        bool bFirstInit = false;
                        ClosePosition ClosePosition = ClosePosition.GetClosePosition(distributeMsg, pos, ClosePositionChild, this.AccID, ref bFirstInit);
                        if (ClosePosition == null) return;
                        if (bFirstInit)
                        {                           
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
                }
            }
        }

        public static DataTable dtPosition = new DataTable();
        public string _AccID;
        public string AccID
        {
            get { return _AccID; }
            set
            {
                traceAccChange.Log(enumItemType.uiChangeAcc.ToString(), @"(PositionBus.[AccID].set),Value:" + value + "'");                              
                if (_AccID == value) return;
                _AccID = value;
                if (_AccID != null)
                {
                   // if (_AccID.Trim() != "")
                        AccChange();
                }                
            }
        }
       
        MessageDistribute distributeMsg;

        #region 开放给调用方,通知数据已更改
        public delegate void PosInfoChange(object sender, ObservableCollection<OrderPosition> posCL, PosChangeType sType,string Acc);
        public event PosInfoChange OnPosInfoChange;

        public void RegisterOnPosInfoChange(PosInfoChange InfoChangeHandler)
        {
            OnPosInfoChange += InfoChangeHandler;
        }

        public void UnRegisterOnPosInfoChange(PosInfoChange InfoChangeHandler)
        {
            OnPosInfoChange -= InfoChangeHandler;
            if (OnPosInfoChange == null)
            {
                UnGetServerMKPrice();//如果没有使用方，对服务器发送取消mkprice get
                clearInstance();
            }
        }
        #endregion

        //delegate 

        private PositionBus(MessageDistribute _distributeMsg)
        {
            if (_distributeMsg == null)
            {
                //MessageBox.Show("");
                //throw new Exception("messgeDis error");
                return;
            }
            distributeMsg = _distributeMsg;
            RegisterSVDelegate();
            ///2013-06-21 ，when relogin　to initial a acc.
            string acid = IDelearStatus.ACCUoAc;
            if (acid != "")
            {
                AccID = acid;
            }
        }

      


        private static PositionBus instance;
        private static object _lock = new object();
        public static PositionBus GetSinglePositionBus(MessageDistribute _distributeMsg)
        {
            if (_distributeMsg == null) return null;
            if (instance == null)
            {
                lock (_lock)
                {
                    if (instance == null)
                    {
                        instance = new PositionBus(_distributeMsg);
                    }
                }
            }
            return instance;
        }

        /// <summary>
        /// 1登出再登入后，持有的DistributeMsg跟系统主界面不一致，须要在登出后调用此函数设当前类实例为空,
        /// 2应该在程序退出或者登入时调用，以免系统异常退出时没有调用到此方法。
        /// </summary>
        void clearInstance()
        {
           // UnGetServerMKPrice();
            if (MKReqMaitenance != null)
            {
                MKReqMaitenance.CloseType(MKReqType);
            }
            distributeMsg = null;
            instance = null;
        }

        public static void clearBusInstance()
        {
            instance = null;
        }

        /// <summary>
        /// 通知服务器去掉产品市价请求
        /// </summary>
        ~PositionBus()
        {
            if (MKReqMaitenance != null)
            {
                MKReqMaitenance.CloseType(MKReqType);
            }
        }

        void RegisterSVDelegate()
        {
            OndeleRecPos = new deleRecPos(ReceivePosition);
            distributeMsg.DisPositionInfo += new MessageDistribute.onDisPositionInfo(distributeMsg_DisPositionInfo);
            distributeMsg.DisMarketPrice  +=new MessageDistribute.OnDisMarketPrice(distributeMsg_DisMarketPrice);
            //distributeMsg.DisMPControlOrder += new MessageDistribute.OnDisMPControlOrder(distributeMsg_DisMarketPrice);
        }

        void UnRegisterSVDelegate()
        {
            distributeMsg.DisPositionInfo -= new MessageDistribute.onDisPositionInfo(distributeMsg_DisPositionInfo);
            distributeMsg.DisMarketPrice  -= new MessageDistribute.OnDisMarketPrice(distributeMsg_DisMarketPrice);
           // distributeMsg.DisMPControlOrder -= new MessageDistribute.OnDisMPControlOrder(distributeMsg_DisMarketPrice);
        }


        bool IsNeedSVRecData()
        {
            if (OnPosInfoChange == null) return false;
            return true;
        }

        delegate void deleRecPos(DataTable dt, string userid, string acc, DateTime? dtime);
        deleRecPos OndeleRecPos;
        protected void distributeMsg_DisPositionInfo(object sender, DataTable dtPos, string UserID, string Acc, DateTime? dtime)
        {
            if (!IsNeedSVRecData())
            {
            //    GosTradeTest.TestExcelLog.WriteLog(enumItemType.Position.ToString(), @"(PositionBus.distributeMsg_DisPositionInfo),recive position,no delegate return,rec acc" + (Acc == null ? "" : Acc) + " ,datatable:" + GosTradeTestHelper.RecTableToString(dtPos));
               
                return;//没有委托任务时无须处理
            }

           // Application.Current.Dispatcher.Invoke(OndeleRecPos, new object[] { dtPos, UserID, Acc, dtime });
            if (GosTestGlobal.bInTest)
            {
                GosTradeTest.TestExcelLog.WriteLog(enumItemType.Position.ToString(), @"(PositionBus.distributeMsg_DisPositionInfo),recive position,rec acc" + (Acc == null ? "" : Acc) + " ,datatable:" + GosTradeTestHelper.RecTableToString(dtPos));
                GosTradeTest.TestExcelLog.WriteLog(enumItemType.Position.ToString(), @"(PositionBus.distributeMsg_DisPositionInfo),before invoke ReceivePosition");
            }
            ReceivePosition(dtPos, UserID, Acc, dtime);
           
        }

        protected void distributeMsg_DisMarketPrice(object sender, ObservableCollection<MarketPriceItem> MktItems)
        {
            if (!IsNeedSVRecData()) return;//没有委托任务时无须处理

            RecMarketPrice(MktItems);         
        }


        //处理从服务器接收到position数据
        bool IsPosCLChangeing = false;
        object objLockUpdatePos = new object();
        void ReceivePosition(DataTable dt, string _userID, string _Acc, DateTime? _dtime)
        {
            if (dt == null)
            {
                return;
            }
            if (_userID == null || _userID.Trim() == "" || GOSTradeStation.UserID == "" || _userID.Trim() != GOSTradeStation.UserID)
            {
                return;
            }
            if (GOSTradeStation.isDealer)
            {
                if (this.AccID == null || this.AccID == "") return;
                if (_Acc == null || _Acc.Trim() == "" || _Acc.Trim() != this.AccID)
                {
                    return;
                }
            }
            
            
            Application.Current.Dispatcher.Invoke((Action)(() =>
            {
                IsPosCLChangeing = true;
                InitPosCollection(dt); 
                IsPosCLChangeing = false;
            }),
           null);           
           
            if (OnPosInfoChange != null)
            {                
                OnPosInfoChange(this, posCollection, PosChangeType.PosChange,this.AccID);
            }

            reSendMKPriceReq(dt);
            dtPosition = dt;
        }

        void InitPosCollection(DataTable dt)
        {
            if (dt == null) return;
          //  if (dt.Rows.Count < 1) return;
            string FGuidID = GosTestGlobal.GetBlockGuid();
            if (GosTestGlobal.bInTest)
            {
                GosTradeTest.TestExcelLog.WriteLog(enumItemType.Position.ToString(), @"(PositionBus.InitPosCollection_"+FGuidID+") begin");
            }
           
            if (dt.Columns.Contains("productCode") == false)
            {
                return;
            }
            if (Monitor.TryEnter(objLockUpdatePos))
            {
                try
                {
                    posCollection.Clear();

                    //Decimal DPL = 0.00M;

                    foreach (DataRow dr in dt.Rows)
                    {
                        if (dr["productCode"] == null) continue;
                        string prCode = dr.getColValue("productCode");
                        if (prCode == null) continue;
                        if (prCode == "") continue;
                        int radix = MarketPriceData.contraceSize(prCode);

                        string strNetAver = GosBzTool.getAndAdjustAveragePrice(prCode, dr.getColValue("netAverage"));

                        string strPrice = dr.getColValue("net") + "@" + strNetAver;
                        if (strNetAver.Trim() == "" || strNetAver.Trim() == "0" || Convert.ToDecimal(strNetAver) == 0)
                        {
                            strPrice = dr.getColValue("net");
                        }

                        string StrPositionHead = string.Format("{0}", strPrice);
                        DataRow drPos = dr;
                        OrderPosition pos = new OrderPosition();
                        pos.ProductCode = prCode;
                        pos.CCY = GosBzTool.getCurrency(prCode);
                        pos.Header = StrPositionHead;
                        pos.productName = drPos.getColValue(1);//productname
                        pos.Prev = drPos.getColValue(2);
                        pos.InOut = drPos.getColValue(3);
                        pos.DayLong = drPos.getColValue(4);
                        pos.DayShort = drPos.getColValue(5);
                        pos.DayNet = Utility.ConvertToInt(drPos.getColValue(6)).ToString();// GosBzTool.getDecimalPrice(prCode, drPos[6].ToString()).ToString();
                        pos.Daynetaverage = GosBzTool.getAndAdjustAveragePrice(prCode, drPos.getColValue("Daynetaverage")).ToString();  // Utility.getColumnValue(drPos, "Daynetaverage");

                        string strMktPrice = dr.getColValue("mktPrice");
                        pos.MktPrice = GosBzTool.getDecimalPrice(pos.ProductCode, strMktPrice).ToString();
                        pos.BaseMktPrice = pos.MktPrice.ToString();// strMktPrice;

                        pos.Ref = drPos.getColValue(10);
                        // drPos[11].ToString();
                        string strPL = dr.getColValue("pl");
                        decimal DecPL = Utility.ConvertToDecimal(strPL);// GosBzTool.getDecimalPrice(prCode, strPL); //
                        pos.decBasePL = DecPL;
                        if (strPL != "")
                        {
                            pos.PL = DecPL.ToString();
                        }
                        else
                        {
                            pos.PL = "";
                        }
                        pos.BasePL = strPL;
                        pos.Contract = drPos[12].ToString();
                        pos.OrderID = drPos[0].ToString().Trim();
                        pos.Col7 = drPos[7].ToString().Trim();
                        pos.Price = drPos[8].ToString().Trim();
                        pos.Net = drPos.getColValue("net");

                        pos.netAverage = strNetAver;
                        pos.Prevaverage = GosBzTool.getAndAdjustAveragePrice(pos.ProductCode, Utility.getColumnValue(drPos, "Prevaverage")).ToString();
                        pos.Daylongaverage = GosBzTool.getAndAdjustAveragePrice(pos.ProductCode, Utility.getColumnValue(drPos, "Daylongaverage")).ToString();

                        pos.Dayshortaverage = GosBzTool.getAndAdjustAveragePrice(pos.ProductCode, Utility.getColumnValue(drPos, "Dayshortaverage")).ToString();
                        pos.InOutaverage = GosBzTool.getAndAdjustAveragePrice(pos.ProductCode, Utility.getColumnValue(drPos, "InOutaverage")).ToString();

                        pos.Bid = 0;// Utility.ConvertToInt(Utility.getColumnValue(drPos, "mk_bid"));
                        pos.Ask = 0;// Utility.ConvertToInt(Utility.getColumnValue(drPos, "mk_ask"));

                        this.posCollection.Add(pos);
                    }
 //                   TradeStationLog.WriteLogPerformance("position initPosCollection ,success get lock to operate pos collection");
                }
                catch (Exception ex)
                {
                    TradeStationLog.WriteError("position initPosCollection ," + ex);
                }
                finally
                {
                    Monitor.Exit(objLockUpdatePos);
                }

            }
            else
            {
                sendGetSVPosition(AccID);
                TradeStationLog.WriteLogPerformance("position initPosCl ,can not get lock to operate pos collection");
            }          
        }



        /// <summary>
        /// 据收到position数据取消旧的getmkprice注册,重新注册新mkprice
        /// </summary>
        /// <param name="dt"></param>
        void reSendMKPriceReq(DataTable dt)
        {
            List<string> lsOldProduct = new List<string>();
            List<string> lsNewProduct = new List<string>();
            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        string str = dt.Rows[i][0].ToString().Trim();
                        if (str != "")
                        {
                            lsNewProduct.Add(str);
                        }
                    }
                }
            }

            if (dtPosition != null)
            {
                string temp = "";
                for (int i = 0; i < dtPosition.Rows.Count; i++)
                {
                    temp = dtPosition.Rows[i][0].ToString().Trim();
                    if (temp != "")
                    {
                        lsOldProduct.Add(temp);
                    }
                }
            }

            if (lsOldProduct.Count > 0 || lsNewProduct.Count > 0)
            {
              //  TradeStationSend.Send(lsOldProduct.Count < 1 ? null : lsOldProduct, lsNewProduct.Count < 1 ? null : lsNewProduct, cmdClient.registerMarketPrice);
                //TradeStationSend.Send(null, lsNewProduct.Count < 1 ? null : lsNewProduct, cmdClient.registerMarketPrice);
                
                //2013-06-17
                MKReqMaitenance = MKReqManage.GetSingleInstance();
                if (MKReqMaitenance != null)
                {
                    MKReqMaitenance.SendReq(lsNewProduct.Count < 1 ? null : lsNewProduct, lsOldProduct.Count < 1 ? null : lsOldProduct, MKReqType);
                }
                TradeStationSend.Get(lsNewProduct, cmdClient.getMarketPrice);
            }
        }


        MKReqManage MKReqMaitenance;
        string MKReqType = "posbus";

        /// <summary>
        /// 取消旧的getmkprice注册
        /// </summary>
        void UnGetServerMKPrice()
        {
            GosTradeTest.TestExcelLog.WriteLog(enumItemType.Position.ToString(), @"(PositionBus.UnGetServerMKPrice),unRegister MKT Price according to old pos collection");
            if (dtPosition != null)
            {
                if (dtPosition.Rows.Count < 1) return;
                List<string> lsOldProduct = new List<string>();

                if (dtPosition != null)
                {
                    string temp = "";
                    for (int i = 0; i < dtPosition.Rows.Count; i++)
                    {
                        temp = dtPosition.Rows[i][0].ToString().Trim();
                        if (temp != "")
                        {
                            lsOldProduct.Add(temp);
                        }
                    }
                }

                if (lsOldProduct.Count > 0)
                {
                   // TradeStationSend.Send(lsOldProduct.Count < 1 ? null : lsOldProduct, null, cmdClient.registerMarketPrice);

                    MKReqMaitenance = MKReqManage.GetSingleInstance();
                    if (MKReqMaitenance != null)
                    { 
                        MKReqMaitenance.SendReq(null, lsOldProduct,MKReqType);
                    }
                }
            }
        }


        public void RecMarketPrice(ObservableCollection<MarketPriceItem> MktItem)  //(DataTable dtMarketPrice)
        {
            if (MktItem == null)
            {
                return;
            }

            //if (posCollection == null) return;
            //if (posCollection.Count < 1) return;
            //if (dtMarketPrice == null) return;
            //if (dtMarketPrice.Rows.Count < 1) return;
            bool b = false;
            ObservableCollection<OrderPosition> posCLCopy = null;
            if (Monitor.TryEnter(objLockUpdatePos))
            {
                try
                {
                    b=UpdateFromMKPrice(MktItem);
                    if (b)
                    {
                        if (OnPosInfoChange != null)
                        {
                            //2014-04-09 add and cancel next line not tested yet
                           posCLCopy = new ObservableCollection<OrderPosition>(posCollection);                           
                          // OnPosInfoChange(this, posCollection, PosChangeType.MKChange, this.AccID);
                        }
                    }
                }
                catch (Exception ex)
                {
                    TradeStationLog.WriteError("positionBus RecMktP," + ex);
                }
                finally
                {
                    Monitor.Exit(objLockUpdatePos);
                }
            }
            else
            {
                TradeStationLog.WriteLogPerformance("positionBus RecMktP,can not get lock to update pos collction");
            }

            //2014-04-09 add 
            try
            {
                if (b)
                {
                    if (OnPosInfoChange != null)
                    {
                        if (posCLCopy != null)
                        {
                            OnPosInfoChange(this, posCLCopy, PosChangeType.MKChange, this.AccID);
                            // OnPosInfoChange(this, posCollection, PosChangeType.MKChange, this.AccID);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TradeStationLog.WriteError("positionBus.css ,RecMarketPrice,invoke OnPosInfoChange error," + ex);
            }
        }

      

        bool UpdateFromMKPrice(ObservableCollection<MarketPriceItem> MktItems) //DataTable dtMarketPrice)
        {
            if (GosTestGlobal.bInTest)
            {
                GosTradeTest.TestExcelLog.WriteLog(enumItemType.Position.ToString(), @"(acountinfo.UpdateFromMKPrice) begin");
            }
         
            if (posCollection == null) return false;
            if (posCollection.Count < 1) return false ;
            if (MktItems == null) return false;
            if (MktItems.Count < 1) return false;
            if (IsPosCLChangeing == true) return false;
            bool bUpdatePos = false;         
               
            foreach (OrderPosition Order in posCollection)
            {
                if (Order == null) continue;
                string prCode = Order.ProductCode;
                if (prCode == "") continue;

                string strPrice = "", strPL = "", strMktPrice = "";
                int Net;
                //decimal PL=0.00M;
                decimal Bid = 0, Ask = 0;
                Decimal DPL = 0.00M, decMKPrice = 0.00M;

                int radix = MarketPriceData.contraceSize(prCode);

                var results = MktItems.FirstOrDefault((x => x.ProductCode == prCode));
                  
                if (results!=null)
                {
                    if (!bUpdatePos)
                    {
                        bUpdatePos = true;
                    }
                    string strBasePL = Order.BasePL;
                    Net = Utility.ConvertToInt(Order.Net);
                    strMktPrice = results.Last.ToString().Trim();
                    decMKPrice = Utility.ConvertToDecimal(strMktPrice);
                    if (strMktPrice == "" || decMKPrice == 0) //如果mkt price 为空或0,取上次的mktprice
                    {
                        strMktPrice = Order.MktPrice;
                        decMKPrice = Utility.ConvertToDecimal(strMktPrice); 
                    }
                    try
                    {                        
                        DPL = Order.decBasePL + ((Net == 0) ? 0 : ((decMKPrice - Convert.ToDecimal(Order.BaseMktPrice)) * Net * radix));
                        strPL = DPL.ToString();
                    }
                    catch
                    {
                        strPL = "";
                    }

                    string temp = results.Bid.ToString().Trim();
                    if (GosBzTool.IsAOValue(temp))
                    {
                        Bid = GosBzTool.SetIntAoValue();
                    }
                    else
                    {
                        Bid = Utility.ConvertToDecimal(temp, 0.0M);
                    }

                    temp = results.Ask.ToString().Trim();
                    if (GosBzTool.IsAOValue(temp))
                    {
                        Ask = GosBzTool.SetIntAoValue();
                    }
                    else
                    {
                        Ask = Utility.ConvertToDecimal(temp, 0.0M);
                    }

                    if (IsPosCLChangeing == true) return false ;
                    //update position data:                    
                    //if pl is null ,no change to order.PL
                    if (strPL != "")
                    {
                        if (Order.PL != strPL)
                        {
                            Order.PL = DPL.ToString();
                        }
                    }
                    if (strMktPrice != "")
                    {
                        if (Order.MktPrice != strMktPrice)
                        {
                            Order.MktPrice = strMktPrice;
                        }
                    } 

                    Order.Bid = Bid;
                    Order.Ask = Ask;

                    #region additional property
                    Order.MktOpen = results.Open.ToString().Trim();
                    Order.MktPreClose = results.PreClose.ToString().Trim();
                    Order.MktChange = results.Change.ToString().Trim();
                    Order.MktHigh = results.High.ToString().Trim();
                    Order.MktChangePer = results.ChangePer.ToString().Trim();
                    Order.MktLow = results.Low.ToString().Trim();
                    Order.MktVolume = results.Volume.ToString().Trim();
                    #endregion
                }
            }

            if (GosTestGlobal.bInTest)
            {
                GosTradeTest.TestExcelLog.WriteLog(enumItemType.Position.ToString(), @"(acountinfo.UpdateFromMKPrice) end");
            }
            return bUpdatePos; 
        }


        void AccChange()
        {           
            ClearControlData();
            sendGetSVPosition(AccID);
        }

        public void ClearControlData()
        {
            if (dtPosition != null)
            {
                UnGetServerMKPrice();
            }
            // IsPosCLChangeing = true;
            if (posCollection.Count > 0)
            {
                posCollection.Clear();
            }
            dtPosition = null;
            // IsPosCLChangeing = false;
            //string str = "";
            //foreach (OrderPosition o in posCollection)
            //{
            //    str += ";" + o.ProductCode;
            //}
            //TradeStationLog.WriteQCLog(DateTime.Now.ToString("yyMMdd-HHmmss ") + "before position clear  in posBus->AccChange;position product :" + str);  


            if (OnPosInfoChange != null)
            {
                OnPosInfoChange(this, posCollection, PosChangeType.PosChange, this.AccID);
            }
        }

        public void sendGetSVPosition(string _acc)
        {
            if (!GOSTradeStation.isDealer)
            {
                TradeStationSend.Send(cmdClient.getPositionInfo);
            }
            else
            {
                if (_acc != null)
                {
                    if (_acc.Trim() != "")
                    {
                        TradeStationSend.Send(cmdClient.getPositionInfo, _acc);
                    }
                }
            }
        }
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
              
    }


    public enum GetMKType
    {
        PositionBus,
        OrderEntry //for orderEntry,more than one OrderEntry may be open, consider this
    }


    /// <summary>
    /// 维护市价发送
    /// 
    /// </summary>
    public class MKReqManage
    {
        public bool bSysPreventSend
        {
            get { return  GOSTradeStation.IsWindowInitialized; }
        }

        private static MKReqManage instance;
        private static object _lockMKReq = new object();
        public static MKReqManage GetSingleInstance()
        {
            if (instance == null)
            {
                lock (_lockMKReq)
                {
                    if (instance == null)
                    {
                        instance = new MKReqManage();
                    }
                }
            }
            return instance;
        }

        private MKReqManage() { }



        Dictionary<string, List<string>> DcCodeList = new Dictionary<string, List<string>>();


        /// <summary>
        /// 维护发送列表，以便unRegister只发送一次，并且不影响其它类型
        /// 一，找出要unreg列表
        /// 1,找出unreglist中有的，reglist没有的元素 unexceptlist
        /// 2,在总的DcCodeList遍历非本类型的list,找出所有在unexceptlist里有的，各list没的元素集
        /// 3,在DcCodeList取得本类型的list中，查找和unreglist共有的code作为发出结果,这是为了不发送多余的unreg以防止影响其它mk price发送请求　
        /// 
        /// 二找出要发送reg请求的列表
        /// 1,reglist 遍历DcCodeList所有类型，查找出在DcCodeList没有的code元素
        /// 三，发送一的unreg结果，二的reg结果列表,
        /// 四，从对应相同类型CodeList除掉一的结果，添加二的结果　
        /// </summary>
        /// <param name="RegList"></param>
        /// <param name="UnRegList"></param>
        /// <param name="sType"></param>
        public void SendReq(List<string> RegList, List<string> UnRegList,string sType)
        {
            if (RegList == null && UnRegList == null) return;
            if (RegList == null) RegList = new List<string>();
            if (UnRegList == null) UnRegList = new List<string>();

            List<string> UnExceptList = UnRegList.Except(RegList).ToList();
            if (DcCodeList.ContainsKey(sType) == false)
            {
                DcCodeList.Add(sType, new List<string>());
            }
            List<string> CodeList = DcCodeList[sType];
           // List<string> UnExceptList1=new List<string>();// = UnExceptList.Except(CodeList).ToList();
            
            foreach (var i in DcCodeList)
            {
                RegList =RegList.Except(i.Value).ToList();
                if (i.Key.Trim() != sType.Trim())
                {
                    UnExceptList = UnExceptList.Except(i.Value).ToList();
                }              
            }
            UnExceptList = UnExceptList.Intersect(CodeList).ToList();//只有在原有的发送列表中，才对外发出解除指令，以免影响其它地方的mk price的请求
            UnExceptList = UnExceptList.Distinct().ToList();
            RegList = RegList.Distinct().ToList();
            if (RegList.Count > 0 || UnExceptList.Count > 0)
            {
                if(bSysPreventSend)
                TradeStationSend.Send(UnExceptList.Count < 1 ? null : UnExceptList, RegList.Count < 1 ? null : RegList, cmdClient.registerMarketPrice);
            }
            bool bChange = false;
            if (RegList.Count > 0)
            {
                TradeStationSend.Get(RegList, cmdClient.getMarketPrice);
                CodeList.AddRange(RegList);
                bChange = true;
            }
           
            if (UnExceptList.Count > 0)
            {
                CodeList = CodeList.Except(UnExceptList).ToList();
                bChange = true;
            }
            if (bChange)
            {
                DcCodeList[sType] = CodeList.Distinct().ToList();
            }
        }


        public void CloseType(string sType)
        {
            if (DcCodeList.ContainsKey(sType) == false)
            {
                return;
            }
            List<string> CodeList = DcCodeList[sType];
            foreach (var i in DcCodeList)
            {               
                if (i.Key.Trim() != sType.Trim())
                {
                    CodeList = CodeList.Except(i.Value).ToList();
                }
            }
            CodeList = CodeList.Distinct().ToList();
            if (CodeList.Count > 0)
            {
                if (bSysPreventSend)
                TradeStationSend.Send(CodeList, null, cmdClient.registerMarketPrice);
            }
            DcCodeList.Remove(sType);
        }

    }

    /// <summary>
    /// harry 2013-04-08
    /// This is to store the position data.
    /// </summary>
    public class OrderPosition : INotifyPropertyChanged
    {
        private string _Header;
        public string Header
        {
            get { return _Header; }
            set { _Header = value; OnPropertyChanged(new PropertyChangedEventArgs("Header")); }
        }

        private string _productCode;
        public string ProductCode
        {
            get { return _productCode; }
            set { _productCode = value; OnPropertyChanged(new PropertyChangedEventArgs("ProductCode")); }
        }

        private string _productName;
        public string productName
        {
            get { return _productName; }
            set { _productName = value; OnPropertyChanged(new PropertyChangedEventArgs("productName")); }
        }

        private string _Prev;
        public string Prev
        {
            get { return _Prev; }
            set { _Prev = value; OnPropertyChanged(new PropertyChangedEventArgs("Prev")); }
        }

        private string _InOut;
        public string InOut
        {
            get { return _InOut; }
            set { _InOut = value; OnPropertyChanged(new PropertyChangedEventArgs("InOut")); }
        }

        private string _DayLong;
        public string DayLong
        {
            get { return _DayLong; }
            set { _DayLong = value; OnPropertyChanged(new PropertyChangedEventArgs("DayLong")); }
        }

        private string _DayShort;
        public string DayShort
        {
            get { return _DayShort; }
            set { _DayShort = value; OnPropertyChanged(new PropertyChangedEventArgs("DayShort")); }
        }

        private string _DayNet;
        public string DayNet
        {
            get { return _DayNet; }
            set { _DayNet = value; OnPropertyChanged(new PropertyChangedEventArgs("DayNet")); }
        }

        private string _Net;
        public string Net
        {
            get { return _Net; }
            set { _Net = value; OnPropertyChanged(new PropertyChangedEventArgs("Net")); }
        }

        private string _MktPrice;
        public string MktPrice
        {
            get { return _MktPrice; }
            set { _MktPrice = value; OnPropertyChanged(new PropertyChangedEventArgs("MktPrice")); 
                    OnPropertyChanged(new PropertyChangedEventArgs("DecMktPrice"));
                    OnPropertyChanged(new PropertyChangedEventArgs("strMktPrice"));
            }
        }
       
        public decimal DecMktPrice
        {
            get { 
                if(MktPrice==null)
                {
                    return 0;
                }
                return GosBzTool.adjustDecLength(this.ProductCode, MktPrice);
              //    return Utility.ConvertToDecimal(MktPrice);
                //2013-07-22 
                //return GosBzTool.getDecimalPrice(this.ProductCode, MktPrice);
            }
            //set { 
            //    _MktPrice = value.ToString(); 
            //    OnPropertyChanged(new PropertyChangedEventArgs("MktPrice"));
            //    );
            //}
        }

        public bool bNagativePL
        {
            get {
                if (DecPL < 0)
                {
                    return true;
                }
                return false;
            }
        }

        public string strMktPrice
        {
            get { 
                if(MktPrice==null)
                {
                    return "0";
                }
                //if (this.CCY.ToUpper() == "USD")
                //{
                //    return GosBzTool.adjustDecLengthToString(this.ProductCode, MktPrice)+" USD";
                //}
                return GosBzTool.adjustDecLengthToString(this.ProductCode, MktPrice);             
            }          
        }

        public string strMktPrice1
        {
            get
            {
                decimal decValue = 0M;
                if (MktPrice != null)
                {
                    decValue = Utility.ConvertToDecimal(MktPrice, 0);
                }
                if (decValue == 0)
                {
                    if (BaseMktPrice != null)
                    {
                        decValue = Utility.ConvertToDecimal(BaseMktPrice, 0);
                    }
                }
                return GosBzTool.adjustDecLengthToString(this.ProductCode, decValue);
            }

        }       


        private string _BaseMktPrice;
        public string BaseMktPrice
        {
            get { return _BaseMktPrice; }
            set { _BaseMktPrice = value; OnPropertyChanged(new PropertyChangedEventArgs("BaseMktPrice")); }
        }


        private string _Ref;
        public string Ref
        {
            get { return _Ref; }
            set { _Ref = value; OnPropertyChanged(new PropertyChangedEventArgs("Ref")); }
        }

        /// <summary>
        /// col 9 column name is "pl"
        /// </summary>
        private string _PL;
        public string PL
        {
            get { return _PL; }
            set { _PL = value; OnPropertyChanged(new PropertyChangedEventArgs("PL"));
            OnPropertyChanged(new PropertyChangedEventArgs("DecPL"));
            OnPropertyChanged(new PropertyChangedEventArgs("strDecPL"));
            OnPropertyChanged(new PropertyChangedEventArgs("bNagativePL"));
            }
        }

        public decimal DecPL
        {
            get
            {
                if (PL == null)
                {
                    return 0;
                }
                //2013-07-22 
                //return GosBzTool.getDecimalPrice(this.ProductCode, PL);
                return GosBzTool.adjustDecLength(this.ProductCode, PL);
                //return Utility.ConvertToDecimal(PL);
            }          
        }

        public string strDecPL
        {
            get
            {
                if (PL == null)
                {
                    return "0";
                }
                //2013-07-22 
              
               // return GosBzTool.adjustDecLengthToString(this.ProductCode, PL);
               

                if (CCY != null)
                {
                    if (CCY.Trim() != "")
                    {
                        if (CCY != "HKD")
                        {
                           return GosBzTool.NFormatString(PL, 2) + "  " + CCY;
                        }
                    }
                }
                return GosBzTool.NFormatString(PL, 2);
            }
        }

        private string _Contract;
        public string Contract
        {
            get { return _Contract; }
            set { _Contract = value; OnPropertyChanged(new PropertyChangedEventArgs("Contract")); }
        }

        private string _OrderID;//column 0 according to closeposition
        public string OrderID
        {
            get { return _OrderID; }
            set { _OrderID = value; OnPropertyChanged(new PropertyChangedEventArgs("OrderID")); }
        }

        private string _Col7;
        /// <summary>
        /// net
        /// </summary>
        public string Col7
        {
            get { return _Col7; }
            set { _Col7 = value; OnPropertyChanged(new PropertyChangedEventArgs("Col7")); }
        }

        private string _Price;  //column 8 according to closeposition
        public string Price
        {
            get { return _Price; }
            set { _Price = value; OnPropertyChanged(new PropertyChangedEventArgs("Price")); }
        }


        //private string _pl;
        //public string pl
        //{
        //    get { return _pl; }
        //    set { _pl = value; OnPropertyChanged(new PropertyChangedEventArgs("pl")); }
        //}

        private string _BasePL;
        public string BasePL
        {
            get { return _BasePL; }
            set { _BasePL = value; OnPropertyChanged(new PropertyChangedEventArgs("BasePL")); }
        }

        private decimal _decBasePL;
        public decimal decBasePL
        {
            get { return _decBasePL; }
            set { _decBasePL = value; OnPropertyChanged(new PropertyChangedEventArgs("decBasePL")); }
        }

        /// <summary>
        /// col 10
        /// </summary>
        private string _refFxRate;
        public string refFxRate
        {
            get { return _refFxRate; }
            set { _refFxRate = value; OnPropertyChanged(new PropertyChangedEventArgs("refFxRate")); }
        }

        /// <summary>
        /// col 11
        /// </summary>
        private string _plBaseCcy;
        public string plBaseCcy
        {
            get { return _plBaseCcy; }
            set { _plBaseCcy = value; OnPropertyChanged(new PropertyChangedEventArgs("plBaseCcy")); }
        }

        /// <summary>
        /// col 12
        /// </summary>
        private string _contract;
        public string contract
        {
            get { return _contract; }
            set { _contract = value; OnPropertyChanged(new PropertyChangedEventArgs("contract")); }
        }

        /// <summary>
        /// col 13
        /// </summary>
        private string _netAverage;
        public string netAverage
        {
            get { return _netAverage; }
            set { _netAverage = value; OnPropertyChanged(new PropertyChangedEventArgs("netAverage")); }
        }

        /// <summary>
        /// col 14
        /// </summary>
        private string _Prevaverage;
        public string Prevaverage
        {
            get { return _Prevaverage; }
            set { _Prevaverage = value; OnPropertyChanged(new PropertyChangedEventArgs("Prevaverage")); }
        }

        /// <summary>
        /// col 15
        /// </summary>
        private string _InOutaverage;
        public string InOutaverage
        {
            get { return _InOutaverage; }
            set { _InOutaverage = value; OnPropertyChanged(new PropertyChangedEventArgs("InOutaverage")); }
        }

        /// <summary>
        /// col 16
        /// </summary>
        private string _Daylongaverage;
        public string Daylongaverage
        {
            get { return _Daylongaverage; }
            set { _Daylongaverage = value; OnPropertyChanged(new PropertyChangedEventArgs("Daylongaverage")); }
        }

        /// <summary>
        /// col 17
        /// </summary>
        private string _Dayshortaverage;
        public string Dayshortaverage
        {
            get { return _Dayshortaverage; }
            set { _Dayshortaverage = value; OnPropertyChanged(new PropertyChangedEventArgs("Dayshortaverage")); }
        }

        /// <summary>
        /// col 18
        /// </summary>
        private string _Daynetaverage;
        public string Daynetaverage
        {
            get { return _Daynetaverage; }
            set { _Daynetaverage = value; OnPropertyChanged(new PropertyChangedEventArgs("Daynetaverage")); }
        }

        public string PrevInfo
        {
            get
            {
                if (this.Prev == null) return "";
                if (this.Prev.Trim() == "" || this.Prev.Trim() == "0") return "";
                return (this.Prev) + @"@" + (this.Prevaverage == null ? "" : this.Prevaverage);
            }
        }
        public string NetInfo
        {
            get
            {
                if (this.Net == null) return "";
                if (this.Net.Trim() == "") return "";
                if (this.Net.Trim() == "0") return "0";
                return (this.Net) + (this.netAverage == null ? "" : (@"@" + this.netAverage));
            }
        }
        public string DayNetInfo
        {
            get
            {
                if (this.DayNet == null) return "";
                if (this.DayNet.Trim() == "" || this.DayNet.Trim() == "0") return "";
                return (this.DayNet) + (this.Daynetaverage == null ? "" : (@"@" + this.Daynetaverage));
            }
        }

        public string InOutInfo
        {
            get
            {
                if (this.InOut == null) return "";
                if (this.InOut.Trim() == "" || this.InOut.Trim() == "0") return "";
                return (this.InOut) + (this.InOutaverage == null ? "" : (@"@" + this.InOutaverage));
            }
        }

        public string DayLongInfo
        {
            get
            {
                if (this.DayLong == null) return "";
                if (this.DayLong.Trim() == "" || this.DayLong.Trim() == "0") return "";
                return (this.DayLong) + (this.Daylongaverage == null ? "" : (@"@" + this.Daylongaverage));
            }
        }

        public string DayShortInfo
        {
            get
            {
                if (this.DayShort == null) return "";
                if (this.DayShort.Trim() == "" || this.DayShort.Trim() == "0") return "";
                return (this.DayShort) + (this.Dayshortaverage == null ? "" : (@"@" + this.Dayshortaverage));
            }
        }

        private bool _bCanClose = true;
        public bool bCanClose
        {
            get
            {
                if (Net != null)
                {
                    int? temp = null;
                    try
                    {
                        temp = Convert.ToInt32(Net);
                    }
                    catch { }
                    if (temp.HasValue)
                    {
                        if (temp.Value == 0)
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
            set { _bCanClose = value; OnPropertyChanged(new PropertyChangedEventArgs("bCanClose")); }
        }

        private string _CCY;
        public string CCY
        {
            get { return _CCY; }
            set { _CCY = value; OnPropertyChanged(new PropertyChangedEventArgs("CCY")); }
        }

        #region Market Info
        private decimal _Ask;
        public decimal Ask
        {
            get { return _Ask; }
            set { _Ask = value; OnPropertyChanged(new PropertyChangedEventArgs("Ask")); }
        }

        private decimal _Bid;
        public decimal Bid
        {
            get { return _Bid; }
            set { _Bid = value; OnPropertyChanged(new PropertyChangedEventArgs("Bid")); }
        }

        
        //2013-07-22
        public decimal DecAsk
        {
            get {
                return GosBzTool.adjustDecLength(this.ProductCode, Ask);
                //return Ask;   
                //GosBzTool.getDecimalPrice(this.ProductCode, Ask.ToString());
            }            
        }

        public string strAsk
        {
            get
            {                
                return GosBzTool.adjustDecLengthToString(this.ProductCode, Ask);
            }

        }     

        public decimal DecBid
        {
            get {
                return GosBzTool.adjustDecLength(this.ProductCode, Bid);
               // return GosBzTool.getDecimalPrice(this.ProductCode, Bid.ToString());
              //  return Bid;
            }
        }

        public string strBid
        {
            get
            {
                return GosBzTool.adjustDecLengthToString(this.ProductCode, Bid);              
            }
        }
        // private int _decLen=0
        public int decLen
        {
            get { return GosBzTool.getDecLen(this.ProductCode); }
        } 

       private string _mktOpen="";
       public string MktOpen
       {
           get { return _mktOpen; }
           set { _mktOpen = value; OnPropertyChanged(new PropertyChangedEventArgs("MktOpen")); }
       }

       private string _MktPreClose = "";
       public string MktPreClose
       {
           get { return _MktPreClose; }
           set { _MktPreClose = value; OnPropertyChanged(new PropertyChangedEventArgs("MktPreClose")); }
       }

       private string _MktChangePer = "";
       public string MktChangePer
       {
           get { return _MktChangePer; }
           set { _MktChangePer = value; OnPropertyChanged(new PropertyChangedEventArgs("MktChangePer")); }
       }

       private string _MktChange = "";
       public string MktChange
       {
           get { return _MktChange; }
           set { _MktChange = value; OnPropertyChanged(new PropertyChangedEventArgs("MktChange")); }
       }

       private string _MktHigh = "";
       public string MktHigh
       {
           get { return _MktHigh; }
           set { _MktHigh = value; OnPropertyChanged(new PropertyChangedEventArgs("MktHigh")); }
       }

       private string _MktLow = "";
       public string MktLow
       {
           get { return _MktLow; }
           set { _MktLow = value; OnPropertyChanged(new PropertyChangedEventArgs("MktLow")); }
       }

       private string _MktVolume = "";
       public string MktVolume
       {
           get { return _MktVolume; }
           set { _MktVolume = value; OnPropertyChanged(new PropertyChangedEventArgs("MktVolume")); }
       }
       //private string _MktPreClose = "";
       //public string MktPreClose
       //{
       //    get { return _MktPreClose; }
       //    set { _MktPreClose = value; OnPropertyChanged(new PropertyChangedEventArgs("MktPreClose")); }
       //}

        #endregion
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

        //this.txt_Name.Text = dr[1].ToString();
        //        this.txt_Prev.Text = "Prev: " + dr[2].ToString();
        //        this.txt_InOut.Text = "In/Out: " + dr[3].ToString();
        //        this.txt_DayLong.Text = "Day Long: " + dr[4].ToString();
        //        this.txt_DayShort.Text = "Day Short: " + dr[5].ToString();
        //        this.txt_DayNet.Text = "Day Net: " + dr[6].ToString();
        //        this.txt_MktPrice.Text = "Mkt Price: " + mkt;
        //        this.txt_Ref.Text = "Ref: " + dr[10].ToString();
        //        this.txt_PL.Text = "P/L (Base Ccy): " + dr[11].ToString();
        //        this.txt_Contract.Text = "Contract: " + dr[12].ToString();
    }

    /// <summary>
    /// if bound value is null,not need to consider this limit
    /// </summary>
    public class ProductBoundInfo
    {
        public string Product { get; set; }
        public string strPriceUp { get; set; }
        public decimal? DecPriceUp
        {
            get {
                if (strPriceUp == null) return null;
                if (strPriceUp.Trim() == "") return null;
                if (strPriceUp.Trim() == "-2147483648") return null;
                try
                {
                    return Convert.ToDecimal(strPriceUp);
                }
                catch
                {
                    return null;
                }
            }
        }
        public string strPriceLower { get; set; }
        public decimal? DecPriceLower
        {
            get
            {
                if (strPriceLower == null) return null;
                if (strPriceLower.Trim() == "") return null;
                if (strPriceLower.Trim() == "-2147483648") return null;
                try
                {
                    return Convert.ToDecimal(strPriceLower);
                }
                catch
                {
                    return null;
                }
            }
        }
        public string strQtyUp { get; set; }
        public Int32? IntQtyUp
        {
            get
            {
                if (strQtyUp == null) return null;
                if (strQtyUp.Trim() == "") return null;
                if (strQtyUp.Trim() == "-2147483648") return null;
                try
                {
                    return Convert.ToInt32(strQtyUp);
                }
                catch
                {
                    return null;
                }
            }
        }
        public string strQtyLower { get; set; }
        public Int32? IntQtyLower
        {
            get
            {
                if (strQtyLower == null) return null;
                if (strQtyLower.Trim() == "") return null;
                if (strQtyLower.Trim() == "-2147483648") return null;
                try
                {
                    return Convert.ToInt32(strQtyLower);
                }
                catch
                {
                    return null;
                }
            }
        } 

        private int _decLen = 0;
        public int DecLen {
            get { return _decLen; }
            set { _decLen = value; }
        }
    }

   
    

       
   
}
