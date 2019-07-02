using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GOSTS;
using System.Diagnostics;

namespace GOSTS.Common
{
    /// <summary>
    /// Get and register 
    /// </summary>
    public class MarketPriceSentData
    {
        private cmdClient _type;
        private List<String> _prodCodeList;
        private String _prodCode;
        private int _count;

        public cmdClient Type
        {
            get
            {
                return _type;
            }
            set
            {
                _type = value;
            }
        }

        public List<String> ProdCodeList
        {
            get
            {
                return _prodCodeList;
            }
            set
            {
                _prodCodeList = value;
            }
        }

        public String ProdCode
        {
            get
            {
                return _prodCode;
            }
            set
            {
                _prodCode = value;
            }
        }

        public int Count
        {
            get
            {
                return _count;
            }
            set
            {
                _count = value;
            }
        }
    }

    public class TradeStationSend
    {
        public static void Get(string prodCode, cmdClient cmd)
        {
            if (prodCode == null || prodCode == "") return;
            List<string> ls = new List<string>();
            ls.Add(prodCode);
            Get(ls, cmd);
        }

        public static void Get(List<string> prodCodeList, cmdClient cmd)
        {
            if (prodCodeList == null || GOSTradeStation.UserID == null || GOSTradeStation.msgChannel == null) return;

            if (prodCodeList.Count > 0 && GOSTradeStation.msgChannelOMP != null)
            {
                switch (cmd)
                {
                    case cmdClient.getTicker:
                        GOSTradeStation.msgChannelOMP.Send(TradeStationComm.MsgRequest.getMsgGetTicker(GOSTradeStation.UserID, prodCodeList.ToArray()));
                        break;
                    case cmdClient.getPriceDepth:
                        GOSTradeStation.msgChannelOMP.Send(TradeStationComm.MsgRequest.getMsgGetPriceDepth(GOSTradeStation.UserID, prodCodeList.ToArray()));
                        break;
                    case cmdClient.getLongPriceDepth:
                        GOSTradeStation.msgChannelOMP.Send(TradeStationComm.MsgRequest.getMsgGetLongPriceDepth(GOSTradeStation.UserID, prodCodeList.ToArray()));
                        break;
                    case cmdClient.getMarketPrice:
                        GOSTradeStation.msgChannelOMP.Send(TradeStationComm.MsgRequest.getMsgGetMarketPrice(GOSTradeStation.UserID, prodCodeList.ToArray()));
                        break;
                };
                if (cmd == cmdClient.invalidation) return;

                // TradeStationLog.WriteForRegister(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + " " + cmd.ToString() + "---" + string.Concat("'", string.Join("','", prodCodeList), "'"));
            }
        }

        /// <param name="UnprodCodeList">Unregister prodcode List</param>
        /// <param name="prodCodeList">Register prodcode List</param>
        /// <param name="cmd">cmdClient</param>
        public static void Send(List<string> UnprodCodeList, List<string> prodCodeList, cmdClient cmd)
        {
            if (prodCodeList != null && prodCodeList.Count > 0)
            {
                cmdClient cmdGet = cmdClient.invalidation;
                switch (cmd)
                {
                    case cmdClient.registerLongPriceDepth:
                        cmdGet = cmdClient.getLongPriceDepth;
                        break;
                    case cmdClient.registerPriceDepth:
                        cmdGet = cmdClient.getPriceDepth;
                        break;
                    case cmdClient.registerMarketPrice:
                        // cmdGet = cmdClient.getMarketPrice; //20130605 update by ben  without get when register
                        break;
                    case cmdClient.registerTicker:
                        // cmdGet = cmdClient.getTicker;
                        break;
                }
                Get(prodCodeList, cmdGet);
            }
            Register(UnprodCodeList, prodCodeList, cmd);

            //if (cmd != cmdClient.registerMarketPrice) return;
            //StackTrace st = new StackTrace(true);
            //string str = st.GetFrame(1).GetMethod().Name.ToString();
            //TradeStationLog.WriteForRegister(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + " ---SendList " + str);

        }

        public static void ClearRegister(cmdClient cmd)
        {
            if (GOSTradeStation.UserID == null || GOSTradeStation.msgChannelOMP == null) return;

            List<string> listProd = new List<string>();
            if (CheckSend(null, null, cmd, out listProd) == true)
            {
                switch (cmd)
                {
                    case cmdClient.registerMarketPrice:
                        GOSTradeStation.msgChannelOMP.Send(TradeStationComm.MsgRequest.getMsgRegisterMarketPrice(GOSTradeStation.UserID, listProd.ToArray()));
                        break;
                    case cmdClient.registerLongPriceDepth:
                        GOSTradeStation.msgChannelOMP.Send(TradeStationComm.MsgRequest.getMsgRegisterLongPriceDepth(GOSTradeStation.UserID, listProd.ToArray()));
                        break;
                    case cmdClient.registerPriceDepth:
                        GOSTradeStation.msgChannelOMP.Send(TradeStationComm.MsgRequest.getMsgRegisterPriceDepth(GOSTradeStation.UserID, listProd.ToArray()));
                        break;
                    case cmdClient.registerTicker:
                        GOSTradeStation.msgChannelOMP.Send(TradeStationComm.MsgRequest.getMsgRegisterTicker(GOSTradeStation.UserID, listProd.ToArray()));
                        break;
                }

                TradeStationLog.WriteForRegister(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff") + " ClearRegister---" + cmd.ToString());
            }
        }

        private static void Register(List<string> UnprodCodeList, List<string> prodCodeList, cmdClient cmd)
        {
            if (GOSTradeStation.UserID == null || GOSTradeStation.msgChannelOMP == null) return;

            List<string> listProd = new List<string>();
            if (CheckSend(UnprodCodeList, prodCodeList, cmd, out listProd) == true)
            {
                switch (cmd)
                {
                    case cmdClient.registerMarketPrice:
                        GOSTradeStation.msgChannelOMP.Send(TradeStationComm.MsgRequest.getMsgRegisterMarketPrice(GOSTradeStation.UserID, listProd.ToArray()));
                        break;
                    case cmdClient.registerLongPriceDepth:
                        GOSTradeStation.msgChannelOMP.Send(TradeStationComm.MsgRequest.getMsgRegisterLongPriceDepth(GOSTradeStation.UserID, listProd.ToArray()));
                        break;
                    case cmdClient.registerPriceDepth:
                        GOSTradeStation.msgChannelOMP.Send(TradeStationComm.MsgRequest.getMsgRegisterPriceDepth(GOSTradeStation.UserID, listProd.ToArray()));
                        break;
                    case cmdClient.registerTicker:
                        GOSTradeStation.msgChannelOMP.Send(TradeStationComm.MsgRequest.getMsgRegisterTicker(GOSTradeStation.UserID, listProd.ToArray()));
                        break;
                }

                TradeStationLog.WriteForRegister(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff") + " " + cmd.ToString() + "------" + string.Concat("'", string.Join("','", listProd), "'"));

                //if (cmd != cmdClient.registerMarketPrice) return;
                //StackTrace st = new StackTrace(true);
                //string str = st.GetFrame(1).GetMethod().Name.ToString();
                //TradeStationLog.WriteForRegister(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + " ---Register " + str);

            }
        }

        private static bool CheckSend(List<string> UnprodCodeList, List<string> prodCodeList, cmdClient cmd, out List<string> lsProd)
        {
            bool result = false;

            if (UnprodCodeList == null && prodCodeList == null)
            {
                if (GOSTradeStation.marketPriceSentData != null)
                {
                    var list = (from filter in GOSTradeStation.marketPriceSentData
                                where filter.Type.Equals(cmd)
                                select filter).ToList();

                    if (list.Count() == 0)
                    {
                        result = true;
                    }
                    else
                    {
                        //List<string> prodList = new List<string>();
                        //prodList.AddRange(list[0].ProdCodeList);

                        //foreach (string str in UnprodCodeList)
                        //{
                        //    prodList.Remove(str);
                        //}

                        //if (list[0].ProdCodeList.Except(prodList).Count() > 0)
                        //{
                        //    result = true;
                        //}

                        list[0].ProdCodeList.Clear();
                        result = true;
                    }
                }
            }
            else
            {
                if (UnprodCodeList != null && UnprodCodeList.Count > 0)  //unregister
                {
                    if (GOSTradeStation.marketPriceSentData != null)
                    {
                        var list = (from filter in GOSTradeStation.marketPriceSentData
                                    where filter.Type.Equals(cmd)
                                    select filter).ToList();

                        if (list.Count() == 0)
                        {
                            // result = true;
                        }
                        else
                        {
                            List<string> prodList = new List<string>();
                            prodList.AddRange(list[0].ProdCodeList);

                            foreach (string str in UnprodCodeList)
                            {
                                prodList.Remove(str);
                            }

                            if (list[0].ProdCodeList.Except(prodList).Count() > 0)
                            {
                                result = true;
                            }

                            list[0].ProdCodeList = prodList;
                        }
                    }
                }

                if (prodCodeList != null) //register
                {
                    if (GOSTradeStation.marketPriceSentData != null)
                    {
                        var list = (from filter in GOSTradeStation.marketPriceSentData
                                    where filter.Type.Equals(cmd)
                                    select filter).ToList(); 

                        //var item = GOSTradeStation.marketPriceSentData.Find(p => p.Type.Equals(cmd)); //can filter

                        if (list.Count() == 0)
                        {
                            GOSTradeStation.marketPriceSentData.Add(new MarketPriceSentData()
                            {
                                Type = cmd,
                                ProdCodeList = prodCodeList
                            });
                            result = true;
                        }
                        else
                        {
                            if (prodCodeList.Except(list[0].ProdCodeList).Count() > 0)
                            {
                                result = true;
                            }

                            List<string> codes = new List<string>();
                            codes.AddRange(list[0].ProdCodeList);
                            codes.AddRange(prodCodeList);
                            list[0].ProdCodeList = codes;
                        }
                    }
                }
            }

            lsProd = new List<string>();
            var ls = (from filter in GOSTradeStation.marketPriceSentData
                      where filter.Type.Equals(cmd)
                      select filter).ToList();

            if (ls.Count() > 0)
            {
                lsProd = ls[0].ProdCodeList.Distinct().ToList();
                if (lsProd.Count() == 0)
                {
                    //TradeStationLog.WriteForRegister(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff") + " " + cmd.ToString() + "------NULL");
                }
                else
                {
                    TradeStationLog.WriteForRegister(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff") + " " + cmd.ToString() + "-Alls-" + string.Concat("'", string.Join("','", ls[0].ProdCodeList), "'"));
                }
            }
            return result;
        }

        public static void Send(cmdClient cmd)
        {
            if (GOSTradeStation.UserID == null) return;

            switch (cmd)
            {
                case cmdClient.getMsgHeartBeatOMP:
                    if (GOSTradeStation.msgChannelOMP == null) return;
                    GOSTradeStation.msgChannelOMP.Send(TradeStationComm.MsgRequest.getMsgHeartBeat());
                    break;
                case cmdClient.getInstrumentList:
                    if (GOSTradeStation.msgChannelOMP == null) return;
                    GOSTradeStation.msgChannelOMP.Send(TradeStationComm.MsgRequest.getMsgGetInstrumentList(GOSTradeStation.UserID));
                    break;
                case cmdClient.getProductList:
                    if (GOSTradeStation.msgChannelOMP == null) return;
                    GOSTradeStation.msgChannelOMP.Send(TradeStationComm.MsgRequest.getMsgGetProductList(GOSTradeStation.UserID));
                    break;

                case cmdClient.getTradeStatistics:
                    if (GOSTradeStation.msgChannel == null) return;
                    GOSTradeStation.msgChannel.Send(TradeStationComm.MsgRequest.getMsgGetTradeStatistics(GOSTradeStation.UserID));
                    break;

                case cmdClient.getAccountInfo:
                    if (GOSTradeStation.msgChannel == null) return;
                    GOSTradeStation.msgChannel.Send(TradeStationComm.MsgRequest.getMsgGetAccountInfo(GOSTradeStation.UserID));
                    break;
                case cmdClient.getCashInfo:
                    if (GOSTradeStation.msgChannel == null) return;
                    GOSTradeStation.msgChannel.Send(TradeStationComm.MsgRequest.getMsgGetCashInfo(GOSTradeStation.UserID));
                    break;
                case cmdClient.getOrderBookInfo:
                    if (GOSTradeStation.msgChannel == null) return;
                    GOSTradeStation.msgChannel.Send(TradeStationComm.MsgRequest.getMsgGetOrderBookInfo(GOSTradeStation.UserID));
                    break;
                case cmdClient.getDoneTradeInfo:
                    if (GOSTradeStation.msgChannel == null) return;
                    GOSTradeStation.msgChannel.Send(TradeStationComm.MsgRequest.getMsgDoneTradeInfo(GOSTradeStation.UserID));
                    break;
                case cmdClient.getPositionInfo:
                    if (GOSTradeStation.msgChannel == null) return;
                    GOSTradeStation.msgChannel.Send(TradeStationComm.MsgRequest.getMsgGetPositionInfo(GOSTradeStation.UserID));
                    break;
                case cmdClient.getClearTradeInfo:
                    if (GOSTradeStation.msgChannel == null) return;
                    GOSTradeStation.msgChannel.Send(TradeStationComm.MsgRequest.getMsgClearTradeInfo(GOSTradeStation.UserID));
                    break;
                case cmdClient.getAccList:
                    if (GOSTradeStation.msgChannel == null) return;
                    GOSTradeStation.msgChannel.Send(TradeStationComm.MsgRequest.getMsgAccList(GOSTradeStation.UserID));
                    break;
                case cmdClient.getMarginCheck:
                    if (GOSTradeStation.msgChannel == null) return;
                    GOSTradeStation.msgChannel.Send(TradeStationComm.MsgRequest.getMsMarginCheck(GOSTradeStation.UserID));
                    break;
                case cmdClient.getMarginCallList:
                    if (GOSTradeStation.msgChannel == null) return;
                    GOSTradeStation.msgChannel.Send(TradeStationComm.MsgRequest.getMsgMarginCallList(GOSTradeStation.UserID));
                    break;
                case cmdClient.getMsgLogout:
                    if (GOSTradeStation.msgChannel == null) return;
                    GOSTradeStation.msgChannel.Send(TradeStationComm.MsgRequest.getMsgLogout(GOSTradeStation.UserID));
                    break;
                case cmdClient.getTradeConfOrders:
                    if (GOSTradeStation.msgChannel == null) return;
                    GOSTradeStation.msgChannel.Send(TradeStationComm.MsgRequest.getMsgTradeConfOrders(GOSTradeStation.UserID));
                    break;
                case cmdClient.getTradeConfTrades:
                    if (GOSTradeStation.msgChannel == null) return;
                    GOSTradeStation.msgChannel.Send(TradeStationComm.MsgRequest.getMsgTradeConfTrades(GOSTradeStation.UserID));
                    break;
                case cmdClient.getMsgHeartBeat:
                    if (GOSTradeStation.msgChannel == null) return;
                    GOSTradeStation.msgChannel.Send(TradeStationComm.MsgRequest.getMsgHeartBeat());
                    break;
            }

            if (cmd == cmdClient.getMsgHeartBeatOMP || cmd == cmdClient.getMsgHeartBeat || cmd == cmdClient.getMarginCallList) return;
            TradeStationLog.WriteForRegister(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + " " + cmd.ToString() + "---" + GOSTradeStation.UserID);
        }

        public static void Send(cmdClient cmd, string[] RecNo, int actionID)
        {
            if (GOSTradeStation.msgChannel == null) return;
            GOSTradeStation.msgChannel.Send(TradeStationComm.MsgRequest.getMsgReportTradeConf(GOSTradeStation.UserID, RecNo, actionID));
        }

        /// <summary>
        /// getTradeConfOrderDetail parameter1 accNo,parameter2 orderNo
        /// changePassword  parameter1 password ,parameter2 new password
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="parameter1"></param>
        /// <param name="parameter2"></param>
        public static void Send(cmdClient cmd, string parameter1, string parameter2)
        {
            if (GOSTradeStation.UserID == null || GOSTradeStation.msgChannel == null) return;
            switch (cmd)
            {
                case cmdClient.getTradeConfOrderDetail:
                    GOSTradeStation.msgChannel.Send(TradeStationComm.MsgRequest.getMsgTradeConfOrderDetail(GOSTradeStation.UserID, parameter1, parameter2));
                    break;
                case cmdClient.changePassword:
                    GOSTradeStation.msgChannel.Send(TradeStationComm.MsgRequest.getMsgChangePassword(GOSTradeStation.UserID, parameter1, parameter2));
                    break;
                default:
                    break;
            }
        }

        public static void Send(cmdClient cmd, TradeStationComm .Attribute .Language language)
        {
            if (GOSTradeStation.UserID == null || GOSTradeStation.msgChannel == null) return;
            switch (cmd)
            { 
                case cmdClient.changeLanguage :
                    GOSTradeStation.msgChannel.Send(TradeStationComm.MsgRequest.getMsgChangeLanguage(GOSTradeStation.UserID, language));
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// accoutID/productCode
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="accoutID"></param>
        public static void Send(cmdClient cmd, string accoutID)
        {
            if (GOSTradeStation.UserID == null) return;

            switch (cmd)
            {
                case cmdClient.getAccountInfo:
                    if (GOSTradeStation.msgChannel == null) return;
                    GOSTradeStation.msgChannel.Send(TradeStationComm.MsgRequest.getMsgGetAccountInfo(GOSTradeStation.UserID, accoutID));
                    break;
                case cmdClient.getCashInfo:
                    if (GOSTradeStation.msgChannel == null) return;
                    GOSTradeStation.msgChannel.Send(TradeStationComm.MsgRequest.getMsgGetCashInfo(GOSTradeStation.UserID, accoutID));
                    break;
                case cmdClient.getOrderBookInfo:
                    if (GOSTradeStation.msgChannel == null) return;
                    GOSTradeStation.msgChannel.Send(TradeStationComm.MsgRequest.getMsgGetOrderBookInfo(GOSTradeStation.UserID, accoutID));                   
                    break;
                case cmdClient.getDoneTradeInfo:
                    if (GOSTradeStation.msgChannel == null) return;
                    GOSTradeStation.msgChannel.Send(TradeStationComm.MsgRequest.getMsgDoneTradeInfo(GOSTradeStation.UserID, accoutID));
                    break;
                case cmdClient.getPositionInfo:
                    if (GOSTradeStation.msgChannel == null) return;
                    GOSTradeStation.msgChannel.Send(TradeStationComm.MsgRequest.getMsgGetPositionInfo(GOSTradeStation.UserID, accoutID));
                    break;
                case cmdClient.getClearTradeInfo:
                    if (GOSTradeStation.msgChannel == null) return;
                    GOSTradeStation.msgChannel.Send(TradeStationComm.MsgRequest.getMsgClearTradeInfo(GOSTradeStation.UserID, accoutID));
                    break;
                case cmdClient.getMsgAccountMaster:
                    if (GOSTradeStation.msgChannel == null) return;
                    GOSTradeStation.msgChannel.Send(TradeStationComm.MsgRequest.getMsgAccountMaster(GOSTradeStation.UserID, accoutID, "", ""));
                    break;
            }

            TradeStationLog.WriteForRegister(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + " " + cmd.ToString() + "---" + GOSTradeStation.UserID + "," + accoutID);
        }

        public static void Send(string UnprodCode, string prodCode, cmdClient cmd)
        {
            if (GOSTradeStation.m_BeforeGet == false) return;

            List<String> UnprodCodeList = null;
            List<String> prodCodeList = null;
            if (UnprodCode != null)
            {
                UnprodCodeList = new List<string> { UnprodCode };
            }
            if (prodCode != null)
            {
                prodCodeList = new List<string> { prodCode };
            }

            if (prodCodeList != null && prodCodeList.Count > 0)
            {
                cmdClient cmdGet = cmdClient.invalidation;
                switch (cmd)
                {
                    case cmdClient.registerLongPriceDepth:
                        cmdGet = cmdClient.getLongPriceDepth;
                        break;
                    case cmdClient.registerPriceDepth:
                        cmdGet = cmdClient.getPriceDepth;
                        break;
                    case cmdClient.registerMarketPrice:
                        cmdGet = cmdClient.getMarketPrice;
                        break;
                    case cmdClient.registerTicker:
                        //cmdGet = cmdClient.getTicker ;
                        break;
                }
                if (cmdGet != cmdClient.invalidation)
                {
                    Get(prodCodeList, cmdGet);
                }
            }
            Register(UnprodCodeList, prodCodeList, cmd);

            //if (cmd != cmdClient.registerMarketPrice) return;
            //StackTrace st = new StackTrace(true);
            //string str = st.GetFrame(1).GetMethod().Name.ToString();
            //TradeStationLog.WriteForRegister(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + " ---Send " + str);

        }

        /// <summary>
        /// Send command with list of strings
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="stringList"></param>
        public static void Send(cmdClient cmd, List<String> stringList)
        {
            if (GOSTradeStation.UserID == null) return;

            switch (cmd)
            {
                case cmdClient.getChart:
                    if (GOSTradeStation.msgChannelOMP != null)
                    {
                        String productCode = stringList[0];
                        String period = stringList[1];
                        String startTime = stringList[2];
                        GOSTradeStation.msgChannelOMP.Send(TradeStationComm.MsgRequest.getMsgGetChart(GOSTradeStation.UserID, productCode, period, startTime));
                    }
                    break;

                default:
                    break;
            }
        }

        public static void SendNotification(cmdClient cmd, string accID, int code, string no)
        {
            if (GOSTradeStation.UserID == null || GOSTradeStation.msgChannel == null) return;

            switch (cmd)
            {
                case cmdClient.notificationAck:
                    if (no != null)
                    {
                        GOSTradeStation.msgChannel.Send(TradeStationComm.MsgRequest.getMsgNotificationAck(GOSTradeStation.UserID, accID, code.ToString(), no));
                    }
                    break;
                case cmdClient.tableNotificationAck:
                    GOSTradeStation.msgChannel.Send(TradeStationComm.MsgRequest.getMsgTableNotificationAck(GOSTradeStation.UserID, accID, code.ToString(), no));
                    break;
            }

            TradeStationLog.WriteForRegister(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff") + " SendNotification:" + cmd.ToString() + "---" + GOSTradeStation.UserID + "," + accID + "," + code.ToString() + "," + no);

        }

        public static bool ExistInContainer(WindowTypes type)
        {
            //StackTrace st = new StackTrace(true);
            //string str = st.GetFrame(1).GetMethod().Name.ToString();
            //TradeStationLog.WriteError(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "ExistInContainer()---" + str);

            try
            {
                switch (type)
                {
                    case WindowTypes.TradeConfirmation:
                        for (int i = 0; i < GOSTradeStation.container.Children.Count; i++)
                        {
                            if (GOSTradeStation.container.Children[i] == null || GOSTradeStation.container.Children[i].Content == null) return false;
                            if (GOSTradeStation.container.Children[i].Content.GetType() == typeof(GOSTS.Dealer.TradeConfirmation))
                            {
                                return true;
                            }
                        }
                        return false;
                    default:
                        return false;
                }
            }
            catch(Exception exp)
            {
                TradeStationLog.WriteError(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff") + " TradeStationSend. ExistInContainer(WindowTypes type),error:" +  exp.ToString ());
                return false;
            }
        }
    }
}