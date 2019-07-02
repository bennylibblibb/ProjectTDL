using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using GOSTS.Common;
using System.Collections.ObjectModel;
using GOSTS;
using GOSTS.Preference;
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using System.Text.RegularExpressions;
using WPF.MDI;

namespace GOSTS
{
    public interface IProduceDirective
    {
        void Send();
       // int No { get; }
    }

    #region producer list
    class PdtGetAccMaster : IProduceDirective
    {
        public void Send()
        {
           //  if(GOSTradeStation.IsWindowInitialized)
            string acc = RamdomData.getRamdomAcc();
            if (!GOSTradeStation.isDealer)
            {
                TradeStationSend.Send(cmdClient.getMsgAccountMaster);
            }
            else
            {
                TradeStationSend.Send(cmdClient.getMsgAccountMaster, acc);
            }

            //send other instructment according to acc changed
            if (GOSTradeStation.isDealer)
            {              
                TradeStationSend.Send(cmdClient.getCashInfo, acc);
                TradeStationSend.Send(cmdClient.getAccountInfo, acc);
            }
            else
            {
                TradeStationSend.Send(cmdClient.getCashInfo);
                TradeStationSend.Send(cmdClient.getAccountInfo);

            }
            
            if (!GOSTradeStation.isDealer)
            {
                TradeStationSend.Send(cmdClient.getOrderBookInfo);              
                TradeStationSend.Send(cmdClient.getDoneTradeInfo);              
               TradeStationSend.Send(cmdClient.getClearTradeInfo);
            }
            else
            {
                TradeStationSend.Send(cmdClient.getOrderBookInfo, acc);           
                TradeStationSend.Send(cmdClient.getDoneTradeInfo, acc);            
              TradeStationSend.Send(cmdClient.getClearTradeInfo, acc);
            }
        }
    }

    class PdtGetCash:IProduceDirective
    {
        public void Send()
        {
            string acc = RamdomData.getRamdomAcc();
            if (GOSTradeStation.isDealer)
            {              
                TradeStationSend.Send(cmdClient.getCashInfo, acc);
                TradeStationLog.WriteTestDirectiveLog("getCashInfo,acc->"+acc);
               // TradeStationSend.Send(cmdClient.getAccountInfo, acc);
            }
            else
            {
                TradeStationSend.Send(cmdClient.getCashInfo);
                 TradeStationLog.WriteTestDirectiveLog("getCashInfo");
                //TradeStationSend.Send(cmdClient.getAccountInfo);               
            }
        }
    }

    //class PdtGetAccount : IProduceDirective
    //{
    //    public void Send()
    //    {
    //        string acc = ProduceFactory.getRamdomAcc();
    //        if (GOSTradeStation.isDealer)
    //        {               
    //            TradeStationSend.Send(cmdClient.getAccountInfo, acc);
    //        }
    //        else
    //        {              
    //            TradeStationSend.Send(cmdClient.getAccountInfo);
    //        }
    //    }
    //}

    class PdtGetAccount : IProduceDirective
    {
        public void Send()
        {
            string acc = RamdomData.getRamdomAcc();
            if (acc == null) return;
            if (acc.Trim() == "")
            {
                return;
            }
            if (GOSTradeStation.isDealer)
            {
                TradeStationSend.Send(cmdClient.getAccountInfo, acc);
                TradeStationLog.WriteTestDirectiveLog("getAccountInfo,acc->"+acc);
            }
            else
            {
                TradeStationSend.Send(cmdClient.getAccountInfo);
                TradeStationLog.WriteTestDirectiveLog("getAccountInfo");
            }
        }
    }

    class PdtGetOrderBook : IProduceDirective
    {
        public void Send()
        {
            string acc = RamdomData.getRamdomAcc();
            if (GOSTradeStation.isDealer)
            {
                TradeStationSend.Send(cmdClient.getOrderBookInfo,acc); 
                 TradeStationLog.WriteTestDirectiveLog("getOrderBookInfo,acc->"+acc);
            }
            else
            {
                TradeStationSend.Send(cmdClient.getOrderBookInfo); 
                TradeStationLog.WriteTestDirectiveLog("getOrderBookInfo");
            }
        }
    }

    class PdtGetDoneTrade : IProduceDirective
    {
        public void Send()
        {
            string acc = RamdomData.getRamdomAcc();
            if (GOSTradeStation.isDealer)
            {                
                TradeStationSend.Send(cmdClient.getDoneTradeInfo, acc); 
                TradeStationLog.WriteTestDirectiveLog("getDoneTradeInfo,acc->"+acc);
            }
            else
            {               
                TradeStationSend.Send(cmdClient.getDoneTradeInfo);        
                TradeStationLog.WriteTestDirectiveLog("getDoneTradeInfo");
            }
        }
    }


    class PdtGetPos : IProduceDirective
    {
        public void Send()
        {
            string acc = RamdomData.getRamdomAcc();
            if (GOSTradeStation.isDealer)
            {
                TradeStationSend.Send(cmdClient.getPositionInfo, acc);
                TradeStationLog.WriteTestDirectiveLog("getPositionInfo,acc->"+acc);
            }
            else
            {
                TradeStationSend.Send(cmdClient.getPositionInfo);
                TradeStationLog.WriteTestDirectiveLog("getPositionInfo");
            }
        }
    }
   
    class PdtGetClearTrade : IProduceDirective
    {
        public void Send()
        {
            string acc = RamdomData.getRamdomAcc();
            if (!GOSTradeStation.isDealer)
            {                
                TradeStationSend.Send(cmdClient.getClearTradeInfo);
                TradeStationLog.WriteTestDirectiveLog("getClearTradeInfo");
            }
            else
            {          
                TradeStationLog.WriteTestDirectiveLog("getClearTradeInfo,acc->"+acc);
                TradeStationSend.Send(cmdClient.getClearTradeInfo, acc);
            }
        }
    }
    
    public class pdtSendBuyNormalProduct : IProduceDirective
    {
        static int state =1;//0 get mkt,1 random from product list
        public void Send()
        {
            string acc = RamdomData.getRamdomAcc();
            if (GOSTradeStation.distributeMsg != null)
            {
                if (state == 0)
                {
                    state = 1;
                }
                else
                {
                    state = 0;
                }
                ProduceFactory FC = ProduceFactory.getProduceFactory(GOSTradeStation.distributeMsg);
                MarketPriceItem mktItem = null;
                if (state == 0)
                {
                    mktItem = FC.GetRamdomMktItem();
                }
                int intOrderPrice=0;
                string ProdCode ="";
                string strLast = "1"; 
                if (mktItem == null)
                {
                    List<string> prodList = RamdomData.getRamdomProductList();
                    if (prodList == null)
                    {
                        TradeStationLog.WriteTestDirectiveLog("buy sell,neither mktprice and product list had product");
                        return;
                    }
                    if (prodList.Count < 1)
                    {
                        TradeStationLog.WriteTestDirectiveLog("buy sell,neither mktprice and product list had product");
                        return;
                    }
                    try
                    {
                        Random r = new Random(unchecked((int)DateTime.Now.Ticks));
                        int i = r.Next(0, prodList.Count);
                        string _Prod = prodList[i];
                        if (_Prod == "")
                        {
                            return;
                        }
                        ProdCode = _Prod;
                        intOrderPrice = RamdomData.getIntPrice(ProdCode);
                    }
                    catch (Exception ex)
                    {
                        return;
                    }
                }
                if(mktItem!=null)
                {
                    ProdCode = mktItem.ProductCode;
                    strLast  = mktItem.Last;
                    decimal DecPrice = 0M;
                    try
                    {
                        DecPrice = Convert.ToDecimal(strLast);
                    }
                    catch
                    {
                        return;
                    }
                    intOrderPrice = GosBzTool.ChangeDecToIn(ProdCode, DecPrice);//(int)DecPrice; //
                }
              
                TradeStationComm.Attribute.BuySell BS=RamdomData.getBuySell();
                uint Qty=(uint)RamdomData.getRamdomQty();
                bool bAo=RamdomData.getRamdomAo();
                TradeStationComm.Attribute.ValidType validType=RamdomData.getValidType();
                long longSpecTime=0;
                if(validType==TradeStationComm.Attribute.ValidType.specTime)
                {
                   longSpecTime =RamdomData.getSpecTime();
                }
                TradeStationComm.Attribute.CondType condType=RamdomData.getCondType(bAo);
                TradeStationComm.Attribute.Active Status=RamdomData.getActiveStatus();               
                
                TradeStationComm.Attribute.StopType stopType = TradeStationComm.Attribute.StopType.normalOrder;
                int intStopPrice=0;

                TradeStationComm.Attribute.TOne TPlus=RamdomData.getTOne();
                //if (mktItem != null)
                {
                    if (GOSTradeStation.isDealer)
                    {
                        GOSTradeStation.msgChannel.Send(TradeStationComm.MsgRequest.getMsgAddOrder(GOSTradeStation.UserID, GOSTradeStation.Pwd, acc,
                            new TradeStationComm.MsgRequest.OrderClass.AddOrderClass(ProdCode,
                               BS,
                                Qty,
                                (bAo) ? 0 : intOrderPrice,
                                validType,
                                condType,
                                stopType,
                                intStopPrice,
                                Status,
                                longSpecTime,
                                TradeStationComm.Attribute.AE.AE,
                                acc,
                                "",
                                TPlus
                                )
                           ));
                        TradeStationSend.Send(cmdClient.getOrderBookInfo, acc);
                        TradeStationSend.Send(cmdClient.getAccountInfo, acc);
                        TradeStationSend.Send(cmdClient.getPositionInfo,acc);
                       if (TradeStationSend.ExistInContainer(WindowTypes.TradeConfirmation))
                        {
                            TradeStationSend.Send(cmdClient.getTradeConfOrders);
                        } 
                        string strLog = "Buy Sell,Product:" + ProdCode + ",BuySell:" + BS.ToString() + ",Qty:" + Qty.ToString()
                            +",int price:"+( (bAo) ? 0 : intOrderPrice).ToString()
                            +",validType:"+validType.ToString()
                            +",specTime:"+longSpecTime.ToString()
                            +",condType:"+condType.ToString()
                            +",StopType:"+stopType.ToString()                            
                            +",intStopPrice:"+intStopPrice.ToString()
                            +",status:"+Status.ToString()
                            +",AE:AE"
                            +",acc:"+acc.ToString()
                            +",Ref:''"
                            +",TPlus:"+TPlus.ToString();
                        TradeStationLog.WriteTestDirectiveLog(strLog);
                    }
                    else
                    {
                        GOSTradeStation.msgChannel.Send(TradeStationComm.MsgRequest.getMsgAddOrder(GOSTradeStation.UserID,
                            GOSTradeStation.Pwd,
                            acc,
                                                    new TradeStationComm.MsgRequest.OrderClass.AddOrderClass(ProdCode,
                                                            BS,
                                                            Qty,
                                                            (bAo) ? 0 : intOrderPrice,
                                                            validType,
                                                            condType,
                                                            stopType,
                                                            intStopPrice,
                                                            Status,
                                                            longSpecTime,
                                                            TradeStationComm.Attribute.AE.normalUser,
                                                            "",
                                                            "",
                                                            TPlus
                                                            )
                                                  ));
                        TradeStationSend.Send(cmdClient.getOrderBookInfo);
                        TradeStationSend.Send(cmdClient.getAccountInfo);
                        TradeStationSend.Send(cmdClient.getPositionInfo);

                          string strLog="buy sell,Product:"+ProdCode +",BuySell:"+BS.ToString()
                            +",Qty:"+Qty.ToString()
                            +",int price"+( (bAo) ? 0 : intOrderPrice).ToString()
                            +",validType:"+validType.ToString()
                            +",specTime:"+longSpecTime.ToString()
                            +",condType:"+condType.ToString()
                            +",StopType:"+stopType.ToString()                            
                            +",intStopPrice:"+intStopPrice.ToString()
                            +",status:"+Status.ToString()
                            +",AE:normalUser"
                            +",acc:''"
                            +",Ref:''"
                            +",TPlus:"+TPlus.ToString();
                        TradeStationLog.WriteTestDirectiveLog(strLog);
                    }
                    // orderChild.Close();    
                
                }
            }
            //if (!GOSTradeStation.isDealer)
            //{
            //    TradeStationSend.Send(cmdClient.getPositionInfo);
            //}
            //else
            //{
            //    TradeStationSend.Send(cmdClient.getPositionInfo, _acc);
            //}
        }
    }


    public class PdtSendMkt : IProduceDirective
    {
        static List<string> lsProdSent=new List<string>();
        static string MKReqType = "testInst";
        public void Send()
        {
            List<string> prodList = RamdomData.getRamdomProductList();

            MKReqManage MKReqMaitenance = MKReqManage.GetSingleInstance();
            if (MKReqMaitenance != null)
            {
                MKReqMaitenance.SendReq(prodList.Count < 1 ? null : prodList, lsProdSent.Count < 1 ? null : lsProdSent, MKReqType);
            }
            TradeStationLog.WriteTestDirectiveLog("Send get mkt price:"+String.Join(",",prodList.ToArray()));
           // TradeStationSend.Get(prodList, cmdClient.getMarketPrice);
            lsProdSent = prodList;
        }
    }
    //public enum EnumDirective
    //{
    //    getAcc,

    //}
    #endregion

    public class pdtOrderChange:IProduceDirective
    {
        public void Send()
        {
            TestOrderMaintainance pdtOrder = TestOrderMaintainance.getPosMaintainance(GOSTradeStation.distributeMsg);
            pdtOrder.Send();
        }
    }
    public class pdtOrderActive:IProduceDirective
    {
        public void Send()
        {
            TestOrderMaintainance pdtOrder = TestOrderMaintainance.getPosMaintainance(GOSTradeStation.distributeMsg);
            string Acc = "";
            DataRow dr = pdtOrder.getRandomOrder(ref Acc);
            if (dr == null || Acc == null)
            {
                return;
            }
            if (Acc.Trim() == "")
            {
                return;
            }

            uint osBQty = Convert.ToUInt32(dr.getColValue("osBQty"));
            uint osSQty = Convert.ToUInt32(dr.getColValue("osSQty"));
            string OrderSelectIndex = dr.getColValue("internalOrderNo");
            string OrderProductCode = dr.getColValue("productCode");          
            int OrderPrice = Convert.ToInt32(dr["price"]);//price
            uint OrderCount = 0;
            TradeStationComm.Attribute.BuySell buySell = TradeStationComm.Attribute.BuySell.buy;

            if (osBQty > 0)
            {
                OrderCount = osBQty;
                buySell = TradeStationComm.Attribute.BuySell.buy;
            }
            else
            {
                OrderCount = osSQty;
                buySell = TradeStationComm.Attribute.BuySell.sell;
            }
            

            if (GOSTradeStation.isDealer)
            {
                string Msg = "Acc:" + Acc
                    + ",internalOrderNo:" + OrderSelectIndex
                    + ",Prod code:" + OrderProductCode
                    + ",bs:" + buySell.ToString()
                    + ",qty:" + OrderCount.ToString()
                    + ",int Price" + OrderPrice.ToString()
                    + ",ae:ae";
                TradeStationLog.WriteTestDirectiveLog("Send Order Active-->" + Msg);

                GOSTradeStation.msgChannel.Send(TradeStationComm.MsgRequest.getMsgActivateOrder(GOSTradeStation.UserID, GOSTradeStation.Pwd, Acc,
                                                new TradeStationComm.MsgRequest.OrderClass.ActivateOrderClass(OrderSelectIndex, OrderProductCode, buySell, OrderCount, OrderPrice, TradeStationComm.Attribute.AE.AE, Acc)));
                TradeStationSend.Send(cmdClient.getOrderBookInfo, Acc);
                TradeStationSend.Send(cmdClient.getAccountInfo, Acc);
                TradeStationSend.Send(cmdClient.getPositionInfo, Acc);
            }
            else
            {
                GOSTradeStation.msgChannel.Send(TradeStationComm.MsgRequest.getMsgActivateOrder(GOSTradeStation.UserID, GOSTradeStation.Pwd, GOSTradeStation.AccountID,
                                                new TradeStationComm.MsgRequest.OrderClass.ActivateOrderClass(OrderSelectIndex, OrderProductCode, buySell, OrderCount, OrderPrice, TradeStationComm.Attribute.AE.normalUser, "")));
                string Msg = "Acc:''"  
                    + ",internalOrderNo:" + OrderSelectIndex
                    + ",Prod code:" + OrderProductCode
                    + ",bs:" + buySell.ToString()
                    + ",qty:" + OrderCount.ToString()
                    + ",int Price" + OrderPrice.ToString()
                    + ",ae:NormalUser";
                TradeStationLog.WriteTestDirectiveLog("Send Order Active-->" + Msg);
                TradeStationSend.Send(cmdClient.getOrderBookInfo);
                TradeStationSend.Send(cmdClient.getAccountInfo);
                TradeStationSend.Send(cmdClient.getPositionInfo);
            }
        }
    }

    public class pdtOrderInActive : IProduceDirective
    {
        public void Send()
        {
            TestOrderMaintainance pdtOrder = TestOrderMaintainance.getPosMaintainance(GOSTradeStation.distributeMsg);
            string Acc = "";
            DataRow dr = pdtOrder.getRandomOrder(ref Acc);
            if (dr == null || Acc == null)
            {
                return;
            }
            if (Acc.Trim() == "")
            {
                return;
            }

            uint osBQty = Convert.ToUInt32(dr.getColValue("osBQty"));
            uint osSQty = Convert.ToUInt32(dr.getColValue("osSQty"));
            string OrderSelectIndex = dr.getColValue("internalOrderNo");
            string OrderProductCode = dr.getColValue("productCode");
            int OrderPrice = Convert.ToInt32(dr["price"]);//basePrice
            uint OrderCount = 0;
            TradeStationComm.Attribute.BuySell buySell = TradeStationComm.Attribute.BuySell.buy;

            if (osBQty > 0)
            {
                OrderCount = osBQty;
                buySell = TradeStationComm.Attribute.BuySell.buy;
            }
            else
            {
                OrderCount = osSQty;
                buySell = TradeStationComm.Attribute.BuySell.sell;
            }


            if (GOSTradeStation.isDealer)
            {               
                GOSTradeStation.msgChannel.Send(TradeStationComm.MsgRequest.getMsgInactivateOrder(GOSTradeStation.UserID, GOSTradeStation.Pwd, Acc,
                                                           new TradeStationComm.MsgRequest.OrderClass.ActivateOrderClass(OrderSelectIndex, OrderProductCode, buySell, OrderCount, OrderPrice, TradeStationComm.Attribute.AE.AE, Acc)));
                string Msg = "Acc:" + Acc
                    + ",internalOrderNo:" + OrderSelectIndex
                    + ",Prod code:" + OrderProductCode
                    + ",bs:" + buySell.ToString()
                    + ",qty:" + OrderCount.ToString()
                    + ",int Price" + OrderPrice.ToString()
                    + ",ae:ae";
                TradeStationLog.WriteTestDirectiveLog("Send Order Inactive-->" + Msg);
                TradeStationSend.Send(cmdClient.getOrderBookInfo, Acc);
                TradeStationSend.Send(cmdClient.getAccountInfo, Acc);
                TradeStationSend.Send(cmdClient.getPositionInfo, Acc);
            }
            else
            {
                GOSTradeStation.msgChannel.Send(TradeStationComm.MsgRequest.getMsgInactivateOrder(GOSTradeStation.UserID, GOSTradeStation.Pwd, GOSTradeStation.AccountID,
                                                        new TradeStationComm.MsgRequest.OrderClass.ActivateOrderClass(OrderSelectIndex, OrderProductCode, buySell, OrderCount, OrderPrice, TradeStationComm.Attribute.AE.normalUser, "")));
                string Msg = "Acc:''"
                  + ",internalOrderNo:" + OrderSelectIndex
                  + ",Prod code:" + OrderProductCode
                  + ",bs:" + buySell.ToString()
                  + ",qty:" + OrderCount.ToString()
                  + ",int Price" + OrderPrice.ToString()
                  + ",ae:Normal";
                TradeStationLog.WriteTestDirectiveLog("Send Order InActive-->" + Msg);
                TradeStationSend.Send(cmdClient.getOrderBookInfo);
                TradeStationSend.Send(cmdClient.getAccountInfo);
                TradeStationSend.Send(cmdClient.getPositionInfo);
            }
        }
    }

    public class pdtOrderDelete : IProduceDirective
    {
        public void Send()
        {
            TestOrderMaintainance pdtOrder = TestOrderMaintainance.getPosMaintainance(GOSTradeStation.distributeMsg);
            string Acc = "";
            DataRow dr = pdtOrder.getRandomOrder(ref Acc);
            if (dr == null || Acc == null)
            {
                return;
            }
            if (Acc.Trim() == "")
            {
                return;
            }
            if (GOSTradeStation.isDealer)
            {
                //   TradeStationSend.Send(cmdClient.
                GOSTradeStation.msgChannel.Send(TradeStationComm.MsgRequest.getMsgDeleteOrder(GOSTradeStation.UserID, GOSTradeStation.Pwd, Acc,
                                                    new TradeStationComm.MsgRequest.OrderClass.DeleteOrderClass(dr["internalOrderNo"].ToString(), dr["productCode"].ToString(), TradeStationComm.Attribute.AE.AE, Acc)));
                string Msg = "Acc:" + Acc
                     + ",internalOrderNo:" + dr["internalOrderNo"].ToString()
                     + ",Prod code:" + dr["productCode"].ToString()
                     + ",ae:ae";
                TradeStationLog.WriteTestDirectiveLog("Send Order Delete-->" + Msg);
                TradeStationSend.Send(cmdClient.getOrderBookInfo);
                TradeStationSend.Send(cmdClient.getAccountInfo);
                TradeStationSend.Send(cmdClient.getPositionInfo);

            }
            else
            {
                GOSTradeStation.msgChannel.Send(TradeStationComm.MsgRequest.getMsgDeleteOrder(GOSTradeStation.UserID, GOSTradeStation.Pwd, GOSTradeStation.AccountID,
                                                new TradeStationComm.MsgRequest.OrderClass.DeleteOrderClass(dr["internalOrderNo"].ToString(), dr["productCode"].ToString(), TradeStationComm.Attribute.AE.normalUser, "")));

                string Msg = "Acc:''"  
                   + ",internalOrderNo:" + dr["internalOrderNo"].ToString()
                   + ",Prod code:" + dr["productCode"].ToString()
                   + ",ae:NormalUser";
                TradeStationLog.WriteTestDirectiveLog("Send Order Delete-->" + Msg);
                TradeStationSend.Send(cmdClient.getOrderBookInfo);
                TradeStationSend.Send(cmdClient.getAccountInfo);
                TradeStationSend.Send(cmdClient.getPositionInfo);
            }
        }
    }

    public class TestOrderMaintainance 
    {
        static TestOrderMaintainance _instance;
        static MessageDistribute msgDistribute;
        private TestOrderMaintainance(MessageDistribute _msgDist)
        {
            if (_msgDist != null)
            {
                if (msgDistribute != _msgDist)
                {
                    msgDistribute = _msgDist;
                    _msgDist.DisUserOrderInfo += new MessageDistribute.onDisUserOrderInfo(distributeMsg_UserOrderInfoItem);
                  
                }
            }          
        }

        public static TestOrderMaintainance getPosMaintainance(MessageDistribute _msgDist)
        {
            if (_instance == null || _msgDist != msgDistribute)
            {
                _instance = new TestOrderMaintainance(_msgDist);
            }
            return _instance;
        }
      
        DataRow drRam;
        string AccCurrent;

        protected void distributeMsg_UserOrderInfoItem(object sender, DataTable _dtOrderBook, string _UserID, string _Acc, DateTime? _dt)
        {
            //when receive from server,randomly copy  a row store to drRam
            try
            {
                if (_dtOrderBook != null)
                {
                    DataTable dtOrderBook = _dtOrderBook;
                    drRam = AsignRandomOrderDr(dtOrderBook);
                    AccCurrent = _Acc;
                }
                // Application.Current.Dispatcher.Invoke(UserOrderInfoData, new object[] { dtOrderBook, _UserID, _Acc, _dt });
            }
            catch(Exception ex)
            {
                
            }
        }

        public DataRow AsignRandomOrderDr(DataTable dtOrderBook)
        {
            if (dtOrderBook == null) return null;
            if (dtOrderBook.Rows.Count < 1) return null;
            Random r=RamdomData.getRamdom();
            int i=r.Next(0,dtOrderBook.Rows.Count);
            DataRow dr=TestToolHelper.CloneDataRow(dtOrderBook.Rows[i]);
            return dr;
        }

        public DataRow getRandomOrder(ref string Acc)
        {
            DataRow dr = TestToolHelper.CloneDataRow(drRam);
            Acc = AccCurrent;
            return dr;
        }

        public void Send()
        {
            if (AccCurrent == null)
            {
                return;
            }
            if (AccCurrent.Trim() == "")
            {
                return;
            }
            DataRow dr = TestToolHelper.CloneDataRow(drRam);// getRandomOrderDr();
            if (dr == null) return;
            uint osBQty = dr.getColUIntValue("osBQty", 0);
            uint osSQty = dr.getColUIntValue("osSQty", 0);
            uint qty = osSQty;
            Random r = RamdomData.getRamdom();
            int Price = dr.getColIntValue("price",10);
            int Deviation = r.Next(0,(int)(Price*0.2));
            if (RamdomData.getRamdomNegativeOP())
            {
                Price -= Deviation;
            }
            if (Price < 1)
            {
                Price = r.Next(1, Price);
            }
            if (Price < 1)
            {
                Price = 1;
            }
            Price = TestToolHelper.adjustPriceLen(Price);
            //r.Next(
            //int intPrice= dr.get
             TradeStationComm.Attribute.BuySell bs = TradeStationComm.Attribute.BuySell.sell;
             if (osBQty > 0)
             {
                 bs = TradeStationComm.Attribute.BuySell.buy;
                 qty = osBQty;
             }

            string strValid = dr.getColValue("valid");

            TradeStationComm.Attribute.ValidType EnumValid = TradeStationComm.Attribute.ValidType.FAK;
            long lTimeStamp = 0;
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
            if(EnumValid==TradeStationComm.Attribute.ValidType.specTime)
            {
                string strTimeStamp = dr.getColValue("specTime");                   
                try
                {
                    lTimeStamp = Convert.ToInt64(strTimeStamp);
                }
                catch { }
            }
            bool bTOne = false;
            string tone = dr.getColValue("tOne");
            if (tone == "1")
            {
                bTOne = true;
            }
            bool bAo = false;
            string strCond = dr.getColValue("cond");
            if (strCond.ToUpper().Trim() == "AO")//ao == "3" ||
            {
                bAo = true;
            }
            if (bAo)
            {
                Price = 0;
            }
            int StopPrice = 0;
            TradeStationComm.Attribute.StopType stopType = TradeStationComm.Attribute.StopType.normalOrder;
            if (!bAo)
            {
                stopType = GosBzTool.getStopTypeByOrderBookStatus(strCond);
                if (stopType != TradeStationComm.Attribute.StopType.normalOrder)
                {
                    if (GosBzTool.CanChangeOrder(strCond))
                    {
                        if (strCond.Length < 3) return;
                        string TGType = strCond.Substring(0, 2).ToUpper();
                        string strTrPrice = strCond.Substring(2);
                        strTrPrice = Regex.Replace(strTrPrice, @"[^\d\.]", "");
                        decimal dStopPrice = Utility.ConvertToDecimal(strTrPrice);// txt_StopPrice_CT.get_txt_Price_NT();
                        StopPrice = (int)dStopPrice;// GosBzTool.ChangeDecToIn(_prodCode, dStopPrice);
                    }
                }
            }           

             if (GOSTradeStation.isDealer)
             {
                 // TradeStationSend.Send(cmdClient.get
                 GOSTradeStation.msgChannel.Send(TradeStationComm.MsgRequest.getMsgChangeOrder(GOSTradeStation.UserID, GOSTradeStation.Pwd, AccCurrent,
                        new TradeStationComm.MsgRequest.OrderClass.ChangeOrderClass(dr.getColValue("internalOrderNo"),
                            dr.getColValue("productCode"),
                            bs,
                            qty,
                            Price,
                            TradeStationComm.Attribute.AE.AE,
                            AccCurrent,
                            dr.getColValue("refNo"),
                            (bTOne) ? TradeStationComm.Attribute.TOne.TPluseOne : TradeStationComm.Attribute.TOne.TOnly, StopPrice)
                       ));


                 string Msg = "Acc:" + AccCurrent
                   + ",internalOrderNo:" + dr.getColValue("internalOrderNo")
                   + ",Prod code:" + dr.getColValue("productCode")
                   + ",bs:" + bs.ToString()
                   + ",qty:" + qty.ToString()
                   + ",int Price" + Price.ToString()
                   + ",StopPrice" + StopPrice
                   + ",ae:ae";
                 TradeStationLog.WriteTestDirectiveLog("Send Order Change-->" + Msg);                 

                 TradeStationSend.Send(cmdClient.getOrderBookInfo, AccCurrent);
                 TradeStationSend.Send(cmdClient.getAccountInfo, AccCurrent);
                 TradeStationSend.Send(cmdClient.getPositionInfo, AccCurrent);
                if (TradeStationSend.ExistInContainer(WindowTypes.TradeConfirmation))
                 {
                     TradeStationSend.Send(cmdClient.getTradeConfOrders);
                 }
             }
             else
             {
                 GOSTradeStation.msgChannel.Send(TradeStationComm.MsgRequest.getMsgChangeOrder(GOSTradeStation.UserID, GOSTradeStation.Pwd,
                      new TradeStationComm.MsgRequest.OrderClass.ChangeOrderClass(dr.getColValue("internalOrderNo"),
                          dr.getColValue("productCode"),
                          bs,
                          qty,
                          Price,
                          TradeStationComm.Attribute.AE.normalUser,
                          "",
                         dr.getColValue("refNo"),
                        (bTOne) ? TradeStationComm.Attribute.TOne.TPluseOne : TradeStationComm.Attribute.TOne.TOnly, StopPrice)
                      ));

                 string Msg = "Acc:''" 
                 + ",internalOrderNo:" + dr.getColValue("internalOrderNo")
                 + ",Prod code:" + dr.getColValue("productCode")
                 + ",bs:" + bs.ToString()
                 + ",qty:" + qty.ToString()
                 + ",int Price" + Price.ToString()
                 + ",StopPrice" + StopPrice
                 + ",ae:Normal User";
                 TradeStationLog.WriteTestDirectiveLog("Send Order Change-->" + Msg);


                 TradeStationSend.Send(cmdClient.getOrderBookInfo);
                 TradeStationSend.Send(cmdClient.getAccountInfo);
                 TradeStationSend.Send(cmdClient.getPositionInfo);
             }
        }
    }

    public class pdtPosClose:IProduceDirective
    {
        static int state = 1;
        public void Send()
        {
            TestPosMaintainance pdtPos = TestPosMaintainance.getPosMaintainance(GOSTradeStation.distributeMsg);
            string Acc = "";
            DataRow dr = pdtPos.getRandomOrder(ref Acc);
            if (dr == null || Acc == null)
            {
                return;
            }
            if (Acc.Trim() == "")
            {
                return;
            }
            string prodCode=dr.getColValue("productCode");
            int intNet = dr.getColIntValue("net", 0);            
            if (intNet == 0)
            {
                return;
            }
            uint Qty = (uint)(intNet < 0 ? -intNet : intNet);
            TradeStationComm.Attribute.BuySell bs = TradeStationComm.Attribute.BuySell.sell;
            if (intNet < 0)
            {
                bs = TradeStationComm.Attribute.BuySell.buy;
            }

            TradeStationComm.Attribute.ValidType valid = RamdomData.getClosePosValidType();// TradeStationComm.Attribute.ValidType.today;
            long SpecTime = 0;
            TradeStationComm.Attribute.TOne tone = RamdomData.getTOne();
            TradeStationComm.Attribute.StopType stopType = TradeStationComm.Attribute.StopType.normalOrder;
            TradeStationComm.Attribute.Active activeState = TradeStationComm.Attribute.Active.active;
            TradeStationComm.Attribute.CondType condType = TradeStationComm.Attribute.CondType.normal;

            int intStopPrice = 0;

            int RandomPrice=RamdomData.getIntPrice(prodCode);
            decimal dPrice =dr.getDecimalValue("mktPrice",RandomPrice);
            int intOrderPrice =(int)dPrice;// GosBzTool.ChangeDecToIn(orderModel.ProductCode, dPrice);
            intOrderPrice = TestToolHelper.adjustPriceLen(intOrderPrice);
          
            if (GOSTradeStation.msgChannel == null)
            {
                return;
            }
            string Message = "";
            if (GOSTradeStation.isDealer)
            {
                GOSTradeStation.msgChannel.Send(TradeStationComm.MsgRequest.getMsgAddOrder(GOSTradeStation.UserID, GOSTradeStation.Pwd, Acc,
                                            new TradeStationComm.MsgRequest.OrderClass.AddOrderClass(prodCode,
                                                bs,
                                                Qty,
                                                intOrderPrice,
                                                valid,
                                                condType,
                                                stopType,
                                                intStopPrice,
                                                activeState,
                                                SpecTime, 
                                                TradeStationComm.Attribute.AE.AE, 
                                                Acc,
                                                    "",
                    tone)
                                            ));

                Message = string.Format("Account:{0},Cmd:{1},ProdCode：{2},Price：{3},Qty：{4} ,Validity:{5},active:{6},specTime:{7},AE:{8},Tone:{9}",
              Acc, bs.ToString(), prodCode, intOrderPrice, Qty, valid, activeState, SpecTime, "AE", tone);       
                TradeStationSend.Send(cmdClient.getOrderBookInfo, Acc);
                TradeStationSend.Send(cmdClient.getAccountInfo, Acc);
                TradeStationSend.Send(cmdClient.getPositionInfo, Acc);
            }
            else
            {
                GOSTradeStation.msgChannel.Send(TradeStationComm.MsgRequest.getMsgAddOrder(GOSTradeStation.UserID, GOSTradeStation.Pwd, "",
                                            new TradeStationComm.MsgRequest.OrderClass.AddOrderClass(prodCode,
                                               bs,
                                                Qty,
                                                intOrderPrice,// orderModel.Price, 
                                                valid,
                                                condType,
                                                stopType,
                                                intStopPrice,
                                                activeState,
                                                SpecTime, 
                                                TradeStationComm.Attribute.AE.normalUser, 
                                                "",
                                                    "",
                    tone
                                                )
                                            ));
                Message = string.Format("Account:{0},Cmd:{1},ProdCode：{2},Price：{3},Qty：{4} ,Validity:{5},active:{6},specTime:{7},AE:{8},Tone:{9}",
             GOSTradeStation.UserID, bs.ToString(), prodCode, intOrderPrice, Qty, valid, activeState, SpecTime, "normalUser", tone);       
                TradeStationSend.Send(cmdClient.getOrderBookInfo);
                TradeStationSend.Send(cmdClient.getAccountInfo);
                TradeStationSend.Send(cmdClient.getPositionInfo);
            }
            TradeStationLog.WriteTestDirectiveLog("Send Position Close-->" + Message);      
        }
    }

    public class TestPosMaintainance
    {
        static TestPosMaintainance _instance;
        static MessageDistribute msgDistribute;
        private TestPosMaintainance(MessageDistribute _msgDist)
        {
            if (_msgDist != null)
            {
                if (msgDistribute != _msgDist)
                {
                    msgDistribute = _msgDist;
                    _msgDist.DisPositionInfo += new MessageDistribute.onDisPositionInfo(distributeMsg_DisPositionInfo);

                }
            }
        }

        DataRow drRam;
        string AccCurrent;

        public static TestPosMaintainance getPosMaintainance(MessageDistribute _msgDist)
        {
            if (_instance == null || _msgDist != msgDistribute)
            {
                _instance = new TestPosMaintainance(_msgDist);
            }
            return _instance;
        }

        protected void distributeMsg_DisPositionInfo(object sender, DataTable dtPos, string UserID, string Acc, DateTime? dtime)
        {
            try
            {
                if (dtPos != null)
                {
                    DataTable _dtPos = dtPos;
                    drRam = AsignRandomOrderDr(_dtPos);
                    AccCurrent = Acc;
                }
            }
            catch (Exception ex)
            {

            }
        }

        public DataRow AsignRandomOrderDr(DataTable dtPos)
        {
            if (dtPos == null) return null;
            if (dtPos.Rows.Count < 1) return null;
            Random r = RamdomData.getRamdom();
            int i = r.Next(0, dtPos.Rows.Count);
            DataRow dr = TestToolHelper.CloneDataRow(dtPos.Rows[i]);
            return dr;
        }

        public DataRow getRandomOrder(ref string Acc)
        {
            DataRow dr = TestToolHelper.CloneDataRow(drRam);
            Acc = AccCurrent;
            return dr;
        }
    }

    public class ProduceFactory
    {
        static ProduceFactory _instance;        
        static MessageDistribute msgDistribute;
        PositionBus PosBus;
        private ProduceFactory(MessageDistribute _msgDist)
        {
            if (_msgDist != null)
            {
                if (msgDistribute != _msgDist)
                {
                    msgDistribute = _msgDist;
                    _msgDist.DisMarketPrice += new MessageDistribute.OnDisMarketPrice(distributeMsg_DisMarketPrice);
                    PosBus = PositionBus.GetSinglePositionBus(_msgDist);
                    TestPosMaintainance pdtPos = TestPosMaintainance.getPosMaintainance(_msgDist);
                }
            }          
        }

        public static ProduceFactory getProduceFactory(MessageDistribute _msgDist)
        {
            if (_instance == null || _msgDist != msgDistribute)
            {
                _instance = new ProduceFactory(_msgDist);
            }
            return _instance;
        }

        protected void distributeMsg_DisMarketPrice(object sender, ObservableCollection<MarketPriceItem> MktItems)
        {          
            RecMarketPrice(MktItems);
        }
        ObservableCollection<MarketPriceItem> MktPriceItems;

        public void RecMarketPrice(ObservableCollection<MarketPriceItem> MktItem)  //(DataTable dtMarketPrice)
        {
            MktPriceItems = MktItem;           
        }

        public MarketPriceItem GetRamdomMktItem()
        {
            if (MktPriceItems == null) return null;
            if (MktPriceItems.Count < 1) return null;
            Random r = new Random(unchecked((int)DateTime.Now.Ticks));
            int i=r.Next(0, MktPriceItems.Count);
            if (MktPriceItems[i] != null)
            {
                return MktPriceItems[i];
            }
            return null;
        }

        public static IProduceDirective Product(int i)//(cmdClient i)
        {
           
            switch (i)
            {
                case 1: //cmdClient.getMsgAccountMaster:
                    return new PdtGetAccMaster();
                    break;
                case 2://cmdClient.getCashInfo:
                    return new PdtGetCash();
                    break;
                case 3://cmdClient.getAccountInfo:
                    return new PdtGetAccount();
                    break;
                case 4://cmdClient.getOrderBookInfo:
                    return new PdtGetOrderBook();
                    break;
                case 5://cmdClient.getDoneTradeInfo:
                    return new PdtGetDoneTrade();
                    break;
                case 6://cmdClient.getPositionInfo:
                    return new PdtGetPos();
                    break;
                case 7://cmdClient.getClearTradeInfo:
                    return new PdtGetClearTrade();
                    break;
                case 8://cmdClient.getClearTradeInfo:
                    return new PdtGetClearTrade();
                    break;
                case 9:
                case 10:
                case 11:
                    return new pdtSendBuyNormalProduct();
                    break;
                case 12:
                    return new PdtSendMkt();
                    break;
                case 13:
                    return new pdtOrderChange();
                    break;
                case 14:
                    return new pdtOrderActive();
                    break;
                case 15:
                    return new pdtOrderInActive();
                    break;
                case 16:
                    return new pdtOrderDelete();
                    break;
                case 17:
                    return new pdtPosClose();
                    break;

            }
            return null;
        }

        

      
    }

   

    public class RamdomData
    {
        static Random r;
        static RamdomData()
        {
            r = new Random(new Guid().GetHashCode());
            getProdFromConfig();
        }

        
        public static Random getRamdom()
        {
          
            //new Random(unchecked((int)DateTime.Now.Ticks));
            //long tick = DateTime.Now.Ticks;
            //Random ran = new Random((int)(tick & 0xffffffffL) | (int)(tick >> 32));
            return r;
        }
        public static bool getRamdomNegativeOP()
        {
            Random r = getRamdom();// new Random(unchecked((int)DateTime.Now.Ticks));
            int i = r.Next(1, 3);
            switch (i)
            {
                case 1:
                    return true;
                    break;
            }
            return false;
        }

        #region Ramdom Acc
        public static string defalutAcc = "1001";
        public static string getRamdomAcc()
        {
            string CurAcc = IDelearStatus.ACCUoAc;

            Random r = getRamdom();// new Random(unchecked((int)DateTime.Now.Ticks));
            if (AccListConfigManage.accoutCL != null)
            {
                if (AccListConfigManage.accoutCL.Count > 1)
                {
                    int i = r.Next(0, AccListConfigManage.accoutCL.Count);
                    //i = i - 1;
                    if (i > -1)
                    {
                        try
                        {
                            if (AccListConfigManage.accoutCL[i] != null)
                            {
                                CurAcc = AccListConfigManage.accoutCL[i];
                            }
                        }
                        catch { }
                    }
                }
            }
            if (CurAcc == null)
            {
                CurAcc = defalutAcc;
            }
            if (CurAcc.Trim() == "")
            {
                CurAcc = defalutAcc;
            }
            return CurAcc;
        }
        #endregion

        #region Ramdom qty
        public static int getRamdomQty()
        {
            Random r = getRamdom();// new Random(unchecked((int)DateTime.Now.Ticks));
            int i= r.Next(1,10);
            return i;
        }
        #endregion

         #region Ramdom ao
        public static bool getRamdomAo()
        {
            Random r = new Random(unchecked((int)DateTime.Now.Ticks));
            int i= r.Next(1,6);
            if(i==5)
            return true;
            else{
                return false;
            }
        }
        #endregion

        #region  TradeStationComm.Attribute.ValidType
        public static TradeStationComm.Attribute.ValidType getValidType()
        {
            Random r = getRamdom();// new Random(unchecked((int)DateTime.Now.Ticks));
            int i= r.Next(1,6);
            i=i-1;
            return (TradeStationComm.Attribute.ValidType)i;            
        }

        public static TradeStationComm.Attribute.ValidType getClosePosValidType()
        {
            Random r = getRamdom();// new Random(unchecked((int)DateTime.Now.Ticks));
            int i = r.Next(0, 3);             
            switch (i)
            {
                case 1:
                    return TradeStationComm.Attribute.ValidType.FAK;
                case 2:
                    return TradeStationComm.Attribute.ValidType.FOK;
                case 0:
                    return TradeStationComm.Attribute.ValidType.today;
            }
            return TradeStationComm.Attribute.ValidType.today;
        }
        #endregion

     public static  TradeStationComm.Attribute.StopType getStopType()
     {
         Random r = getRamdom();// new Random(unchecked((int)DateTime.Now.Ticks));
         int i = r.Next(1, 5);
         switch (i)
         {
             case 1:
                 return TradeStationComm.Attribute.StopType.normalOrder;
                 break;
             case 2:
                   return TradeStationComm.Attribute.StopType.stopLoss;
                 break;
             case 3:
                 return TradeStationComm.Attribute.StopType.upTrigger;
                 break;
             case 4:
                 return TradeStationComm.Attribute.StopType.downTrigger;
                 break;
         }
         return TradeStationComm.Attribute.StopType.normalOrder;
     }


    #region  TradeStationComm.Attribute.Active
        public static  TradeStationComm.Attribute.Active getActiveStatus()
        {
            Random r = getRamdom();// new Random(unchecked((int)DateTime.Now.Ticks));
            int i= r.Next(0,2);
            return (TradeStationComm.Attribute.Active)i;
        }
    #endregion

         #region  TradeStationComm.Attribute.TOne
        public static  TradeStationComm.Attribute.TOne getTOne()
        {
            Random r = getRamdom();// new Random(unchecked((int)DateTime.Now.Ticks));
            int i= r.Next(0,2);
            return (TradeStationComm.Attribute.TOne)i;
        }
    #endregion
        
    #region TradeStationComm.Attribute.CondType
        public static TradeStationComm.Attribute.CondType getCondType(bool bAo)
        {
            if(bAo)
            {
                return TradeStationComm.Attribute.CondType.AO ;
            }
            return TradeStationComm.Attribute.CondType.normal;
        }
    #endregion  

#region specTime for BuySell
        public static long getSpecTime()
        {
            Random r = getRamdom();// new Random(unchecked((int)DateTime.Now.Ticks));
            int i=r.Next(0,20);
            DateTime dt=DateTime.Now.AddDays(i);
            long specTime= Utility.ConvertToUnixTime(dt);
            return specTime;
        }
#endregion

#region Buy sell
        public static TradeStationComm.Attribute.BuySell getBuySell()
        {
            Random r = getRamdom();// new Random(unchecked((int)DateTime.Now.Ticks));
            int i= r.Next(0,2);
            switch(i)
            {
                case 0:
                    return TradeStationComm.Attribute.BuySell .buy;
            }
            return  TradeStationComm.Attribute.BuySell.sell;
        }
#endregion

        static List<string> ProdsFromConfig = new List<string>();
        public static void getProdFromConfig()
        {           
           string str = AppFlag.RandomTestProds;
           string[] arry=str.Split(',');
           ProdsFromConfig.Clear();
           foreach (string s in arry)
           {
               if (s != null)
               {
                   if(s.Trim()!="")
                   ProdsFromConfig.Add(s);
               }
           }            
        }

        public static List<string> getRamdomProductList()
        {
            bool bReadSysProdList = false;
            if (ProdsFromConfig == null)
            {
                bReadSysProdList = true;
            }
            if (!bReadSysProdList && ProdsFromConfig.Count < 1)
            {
                bReadSysProdList = true;
            }

            // if (ProdsFromConfig.Count < 1) return null;
            if (!bReadSysProdList)
            {
                Random r = new Random(unchecked((int)DateTime.Now.Ticks));
                List<string> ls = new List<string>();
                int count = r.Next(1, 5);
                try
                {
                    for (int j = 0; j < count; j++)
                    {
                        int i = r.Next(0, ProdsFromConfig.Count);
                        string Prods = ProdsFromConfig[i];
                        if (Prods.Trim() != "")
                        {
                            if (ls.Contains(Prods.Trim()) == false)
                            {
                                ls.Add(Prods.Trim());
                            }
                        }
                    }
                }
                catch (Exception ex)
                { }
                return ls;
            }
            else
            {
                #region for get some product info from mkt price info to do buy sell action, ramdom get product list from ProdListTable and send mkt price
                //string MKReqType = "TraderTest";
                //List<string> prodListForGetMKT = new List<string>();
                //void RomdomMktSend()
                {
                    if (GOSTradeStation.marketPriceData != null)
                    {
                        List<string> prodList = new List<string>();
                        if (GOSTradeStation.marketPriceData.ProdListTable != null)
                        {
                            Random r = getRamdom();// new Random(unchecked((int)DateTime.Now.Ticks));
                            DataRow[] drs = GOSTradeStation.marketPriceData.ProdListTable.Select("prodType='1'");
                            if (drs.Length < 1)
                            {
                                drs = GOSTradeStation.marketPriceData.ProdListTable.Select("1=1");
                            }
                            if (drs.Length > 1)
                            {
                                int RowsCount = drs.Length;
                                int Num = 0;
                                int index = r.Next(0, RowsCount);
                                int counts = 0;
                                while (counts<30 &&Num < 6 && (drs.Length - 1) >= Num)
                                {
                                    counts++;
                                    if (drs.Length <= index)
                                    {
                                        continue;
                                    }
                                    if (drs == null) break;
                                    DataRow dr = drs[index];
                                    if (dr == null) continue;
                                    string prod = dr.getColValue("productCode");
                                    if (prod != "")
                                    {
                                        if (prodList.Contains(prod) == false)
                                        {
                                            prodList.Add(prod);
                                            Num++;
                                        }
                                    }
                                    index = r.Next(0, RowsCount);
                                }
                            }
                        }
                        return prodList;
                    }
                }
                #endregion

            }
            return null;
        }

        #region
        public static int getSleeps()
        {
            Random r = new Random(unchecked((int)DateTime.Now.Ticks));
            int i = r.Next(AppFlag.intRandomMin, AppFlag.intRandomMax);
            return i;
        }
        #endregion

        public static int getIntPrice(string prodCode)
        {
            int len = 0;
            try
            { len = GosBzTool.getDecLen(prodCode); }
            catch (Exception ex) { len = 0; }
            int power = (int)Math.Pow(10, len);
            int Lower = power;
            int Upper = 99999;
            try
            {
                DataRow dr = MarketPriceData.GetProductInfo(prodCode);
                if (dr != null)
                {
                    string strLower = dr.getColValue("priceLowerLimit");
                    if (strLower == "-2147483648")
                    {
                        strLower = "";
                    }
                    string strUpper = dr.getColValue("priceUpperLimit");
                    if (strUpper == "-2147483648")
                    {
                        strUpper = "";
                    }
                    if (strLower != "")
                    {
                        try
                        {
                            int intLower = Convert.ToInt32(strLower);
                            if (Lower < intLower)
                            {
                                Lower = intLower;
                            }
                        }
                        catch { }
                    }

                    if (strUpper != "")
                    {
                        try
                        {
                            int intUpper = Convert.ToInt32(strUpper);
                            if (Upper > intUpper)
                            {
                                Upper = intUpper;
                            }
                        }
                        catch { }
                    }

                }
            }
            catch { }

            Random R = getRamdom();
            int i = R.Next(Lower, Upper);
            return i;        
        }
    }
    
    public class TraderTest
    {
        static TraderTest _TraderTest;
        private TraderTest()
        {
            initData();
        }

        void initData()
        {
            TradeStationLog.WriteTestDirectiveLog("prepare ramdom products mkt info");
           //RomdomMktSend();
            PdtSendMkt pdt = new PdtSendMkt();
            pdt.Send();
        }

      
        public int State = 0;//0,un start，1,running;2,pause;
        AutoResetEvent are = new AutoResetEvent(false);
        public void Pause()
        {
            if (State==1)
            {
                TradeStationLog.WriteTestDirectiveLog("pause Test Thread");
                State = 2;               
            }
        }

        public void Resume()
        {
            if (State == 2)
            {               
                State = 1;
                are.Set();
                TradeStationLog.WriteTestDirectiveLog("Resume Test Thread");
            }
        }

        public void Reset()
        {
            try
            {
                if (ThreadTest != null)
                {
                    ThreadTest.Abort();
                }
            }
            catch { }
            _TraderTest = null;
        }

        public static TraderTest getTraderTest()
        {
            if (_TraderTest == null)
            {
                _TraderTest = new TraderTest();
            }
            return _TraderTest;
        }

         Thread ThreadTest;
         public void Test()
         {
             if (!GOSTS.AppFlag.EnabledRandomTest())
             {
                 return;
             }
             ThreadTest = new Thread(new ThreadStart(RunTest));
             ThreadTest.IsBackground = true;
             TradeStationLog.WriteTestDirectiveLog("Start Test Thread");
             ThreadTest.Start();
           
         }

         void RunTest()
         {
             State = 1;
             while (true)
             {
                 if (State == 2)
                 {
                     are.WaitOne();
                 }

                 Random r = new Random(unchecked((int)DateTime.Now.Ticks));
                 int i = r.Next(1, 20);
                 cmdClient cmd;
                 bool bRun = false;
                 try
                 {
                     //cmd = (cmdClient)i;// cmdClient(i);     
                     IProduceDirective IPD = ProduceFactory.Product(i);
                     if (IPD != null)
                     {
                         IPD.Send();
                         bRun = true;
                     }
                 }
                 catch(Exception ex) {
                     TradeStationLog.WriteTestDirectiveLog(ex.Source + "," + ex.TargetSite + "," + ex.Message);
                 }

                 if (bRun)
                 {
                     int sends = RamdomData.getSleeps();
                     Thread.Sleep(sends * 1000);
                 }
             }
         }
    }


    //class TestAtrr : Attribute
    //{
    //      string Type;
    //}

    public class TestToolHelper
    {
        public static DataRow CloneDataRow(DataRow drv)
        {
            if (drv == null) return null;
            try
            {
                DataTable dt = drv.Table.Clone();
                DataRow newRow = dt.NewRow();
                newRow.ItemArray = drv.ItemArray;
                dt.Rows.Add(newRow);
                dt.AcceptChanges();
                return dt.Rows[0];
            }
            catch { return null; }
        }     
 
        /// <summary>
        /// if len of price over 5,replace with 0
        /// </summary>
        /// <param name="Price"></param>
        /// <returns></returns>
        public static Int32 adjustPriceLen(int Price)
        {
            if (Price < 1)
            {
                Price = 1;
                return Price;
            }
            if (Price > 99999)
            {
                string str=Price.ToString();
                int len=str.Length;
                string str1=Price.ToString().Substring(0, 5) + "".PadLeft(len - 5, '0');
                try
                {
                    int i = Convert.ToInt32(str1);
                    return i;
                }
                catch { 
                     
                }
                return 99999;
            }
            return Price;
        }
    }







    abstract class GosTestItem<T>
    {
        public abstract void getInput(T input);
        public abstract void Test(T input);
    }



    public interface IGosTestLog
    {
         void WriteLog(string Type,string msg);
         void WriteLog<T>(T Input,string Type, string msg);   
    }

    public class GosTestGlobal
    {
        public static bool bInTest = false ;
        public static string itSeperate = ",";
        public static Random rd = new Random(new Guid().GetHashCode());
        public static string GetBlockGuid()
        {
            return rd.Next(0, 1000000).ToString();
        }

    }

    public class ExcelLog : IGosTestLog
    {
        public void WriteLog(string Type, string msg)
        {
            if (GOSTradeStation.isDealer)
            {
                TradeStationLog.WriteTestDriverLog("[" + Type + "]" + msg);
            }
        }
        public void WriteLog<T>(T Input, string Type, string msg)
        {

        }
    }

    public class GosTradeTest
    {
        public static IGosTestLog TestExcelLog=new ExcelLog();
    }

    public enum enumItemType
    {
        getMast,
        uiChangeAcc,
        Position
    }


    public class GosTradeTestHelper
    {
        public static string RecTableToString(DataTable dt)
        {
            if (dt == null) return @"\r\n--null datatable--\r\n";
            StringBuilder builder = new StringBuilder("");
            builder.AppendLine("");
            builder.AppendLine(@"-------------datatable-----------------");
            foreach (DataColumn col in dt.Columns )
            {
                builder.Append(col.ColumnName == null ? "no col name" : col.ColumnName + ":");
                int i = 1;
                foreach (DataRow dr in dt.Rows)
                {
                    builder.Append("(row"+i.ToString()+")"+dr.getColValue(col.ColumnName)+","+@"        ");
                    i++;
                }
                
            }
            builder.AppendLine(@"---------------datatable---------------");
            return builder.ToString();
        }
    }
}
