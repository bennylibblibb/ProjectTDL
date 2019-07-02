/*
 * Created by SharpDevelop.
 * User: Benny
 * Date: 2012/11/7
 * Time: 下午 05:37
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.ComponentModel;
using System.Drawing;
using WPF.MDI;
using System.Collections;

namespace GOSTS.Common
{
    /// <summary>
    /// Description of DistributeMessage.
    /// </summary>
    public class MessageDistribute : IDisposable
    {
        private bool disposed = false;
        private MarketPriceData marketPriceData = new MarketPriceData();

        private TradeStationComm.MsgResponse.ResponseObject _responseObject = null;
        public TradeStationComm.MsgResponse.ResponseObject responseObject
        {
            get { return _responseObject; }
            set { _responseObject = value; }
        }

        private CultureInfo _culture;
        public CultureInfo Culture
        {
            get { return _culture; }
            set
            {
                _culture = value;
            }
        }

        private string _lanuage;
        public string Lanuage
        {
            get { return _lanuage; }
            set
            {
                _lanuage = value;
            }
        }

        /// <summary>
        /// change language
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="theObject"></param>
        public delegate void OnChangeLanguage(object sender, string lanuage);
        public event OnChangeLanguage DisChangeLanguage;

        public delegate void OnDisPriceManager(object sender, MarketPriceData data);
        public event OnDisPriceManager DisPriceManager;

        public delegate void OnDisMarketPrice(object sender, ObservableCollection<MarketPriceItem> MarketPriceItems);
        public event OnDisMarketPrice DisMarketPrice;

        public delegate void OnDisGotProductList(object sender);
        public event OnDisGotProductList DisGotProductList;

        public delegate void OnDisPMPriceDepth(object sender, DataTable priceDepthTable);
        public event OnDisPMPriceDepth DisPMPriceDepth;

        public delegate void OnDisPMPriceDepthData(object sender, ObservableCollection<LongPriceDepthData> priceDepthData);
        public event OnDisPMPriceDepthData DisPMPriceDepthData;

        public delegate void OnDisPMLongPriceDepth(object sender, List<LongPriceDepthData> data);
        public event OnDisPMLongPriceDepth DisPMLongPriceDepth;

        public delegate void OnDisPMTickerItemData(object sender, ObservableCollection<TickerData> tickerData);
        public event OnDisPMTickerItemData DisPMTickerItemData;

        public delegate void OnDisTradeStatistic(object sender, DataTable statistTable);
        public event OnDisTradeStatistic DisTradeStatistic;

        public delegate void OnDisControl(object sender, string prodCode);
        public event OnDisControl DisControl;

        public delegate void OnDisEnterOrderControl(object sender, string prodCode, float price, int Qty, bool isAO);
        public event OnDisEnterOrderControl DisEnterOrderControl;


        public delegate void OnDisControlOrderItem(object sender, MarketPriceItem mpi, string CellType);
        public event OnDisControlOrderItem DisControlOrderItem;

        public delegate void OnDisControlFocus(object sender, string defaultTitle, MdiChild child);
        public event OnDisControlFocus DisControlFocus;

        public delegate void OnDisControlChangeTitle(object sender, string defaultTitle, MdiChild child);
        public event OnDisControlChangeTitle DisControlChangeTitle;


        public delegate void OnDisNotification(object sender, Notification notification, TableNotification tableNotification);
        public event OnDisNotification DisNotification;


        public delegate void OnDisAccoutList(object sender, DataTable accoutList);
        public event OnDisAccoutList DisAccoutList;

        public delegate void OnDisMarginCheck(object sender, ObservableCollection<MarginCheckAccData> data);
        public event OnDisMarginCheck DisMarginCheck;

        public delegate void OnDisMarginCallList(object sender, ObservableCollection<MarginCheckAccData> data);
        public event OnDisMarginCallList DisMarginCallList;

        public delegate void OnDisGetError(object sender, MessageException exp);
        public event OnDisGetError DisGetError;

        public delegate void OnDisLoadProduct(object sender, List<string> arr, string title);
        public event OnDisLoadProduct DisLoadProduct;

        public delegate void OnDisAccountMaster(object sender, DataTable tableMaster, string AccNo);
        public event OnDisAccountMaster DisAccountMaster;

        public delegate void OnTradeConfOrder(object sender, DataTable table);
        public event OnTradeConfOrder DisTradeConfOrder;

        public delegate void OnTradeConfTrade(object sender, DataTable table);
        public event OnTradeConfTrade DisTradeConfTrade;

        public delegate void OnReportTradeConf(object sender, string str);
        public event OnReportTradeConf DisReportTradeConf;

        public delegate void OnTradeConfOrderDetail(object sender, DataTable table);
        public event OnTradeConfOrderDetail DisTradeConfOrderDetail;

        public delegate void OnHandleTab(object sender);
        public event OnHandleTab HandleTab;

        public delegate void OnSyncAlert(object sender, HandleType type, PriceAlertData data);
        public event OnSyncAlert SyncAlert;

        public void DisSyncAlert(HandleType type, PriceAlertData data)
        {
            if (this.SyncAlert != null)
            {
                this.SyncAlert(this, type, data);
            }
        }

        public void HandleTabName()
        {
            if (this.HandleTab != null)
            {
                this.HandleTab(this);
            }
        }

        public delegate void OnSyncTime(object sender, DateTime dateTime);
        public event OnSyncTime SyncTime;

        public void SyncServerTime(DateTime dateTime)
        {
            if (this.SyncTime != null)
            {
                this.SyncTime(this, dateTime);
            }
        } 

        /// <summary>
        /// coco
        /// </summary> 
        public delegate void OnDisUserAccountInfo(object sender, DataTable dtAccount, string _userid, string _acc, DateTime? _dt);
        public event OnDisUserAccountInfo DisUserAccountInfo;

        public delegate void OnDisCashInfo(object sender, DataTable dtCashInfo, string _userid, string _acc, DateTime? dt);
        public event OnDisCashInfo DisCashInfo;

        public delegate void OnDisAddOrder(object sender, OrderResponse orderResponse);
        public event OnDisAddOrder DisAddOrder;

        public delegate void OnDisChangeOrder(object sender, OrderResponse orderResponse);
        public event OnDisChangeOrder DisChangeOrder;

        public delegate void OnDisDeleteOrder(object sender, OrderResponse orderResponse);
        public event OnDisDeleteOrder DisDeleteOrder;

        public delegate void OnDisInactivateOrder(object sender, OrderResponse orderResponse);
        public event OnDisInactivateOrder DisInactivateOrder;

        public delegate void OnDisActivateOrder(object sender, OrderResponse orderResponse);
        public event OnDisActivateOrder DisActivateOrder;

        public delegate void onDisClearTradeInfo(object sender, DataTable dtClearTrade, string _userID, string _Acc, DateTime? _dt);
        public event onDisClearTradeInfo DisClearTradeInfo;

        public delegate void onDisUserOrderInfo(object sender, DataTable dtOrderBook, string _userID, string _Acc, DateTime? _dt);
        public event onDisUserOrderInfo DisUserOrderInfo;

        public delegate void onDisDoneTradeInfo(object sender, DataTable dtDoneTrade, string _userID, string _Acc, DateTime? _dt);
        public event onDisDoneTradeInfo DisDoneTradeInfo;

        public delegate void onDisPositionInfo(object sender, DataTable dtPosition, string _userID, string _Acc, DateTime? _dt);
        public event onDisPositionInfo DisPositionInfo;


        public delegate void OnSendReportTradeConf(object sender, string accNo, string orderID, string[] recArr, int type);
        public event OnSendReportTradeConf DisSendReportTradeConf;
        public void DistributeSendReportTradeConf(string accNo, string orderID, string[] recArr, int type)
        {
            if (this.DisSendReportTradeConf != null)
            {
                this.DisSendReportTradeConf(this, accNo, orderID, recArr, type);
            }
        }

        public delegate void OnSendReportTradeConfs(object sender, ObservableCollection<TradeSimply> data, int type);
        public event OnSendReportTradeConfs DisSendReportTradeConfs;
        public void DistributeSendReportTradeConf(ObservableCollection<TradeSimply> data, int type)
        {
            if (this.DisSendReportTradeConfs != null)
            {
                this.DisSendReportTradeConfs(this, data, type);
            }
        }

        //public delegate void ClearEventHandler(object sender);
        //public event ClearEventHandler ClearEvent;

        //public void DistributeClearEvent(object sender)
        //{
        //    if (this.ClearEvent != null)
        //    {
        //        this.ClearEvent(sender);
        //    }
        //}

        public delegate void onDisChartInfo(object sender, TradeStationComm.infoClass.Chart.ChartHistoryData dtChart);
        public event onDisChartInfo DisChartInfo;

        public delegate void onDisChangePassword(object sender, string strResult, bool bSuccess, string _UserID);
        public event onDisChangePassword DisChangePassword;

        public MessageDistribute()
        {

        }

        public void DistributeLoadProduct(List<string> arr, string title)
        {
            if (this.DisLoadProduct != null)
            {
                this.DisLoadProduct(this, arr, title);
            }
        }

        /// <summary>
        /// change language  
        /// </summary>
        public void DistributeChangeLanguage()
        {
            if (_lanuage != null)
            {
                if (this.DisChangeLanguage != null)
                {
                    this.DisChangeLanguage(this, _lanuage);
                }
            }
        }

        /// <summary>
        /// for MarketPrice click
        /// </summary>
        public void DistributeControl(string prodCode)
        {
            if (prodCode != null)
            {
                if (this.DisControl != null)
                {
                    this.DisControl(this, prodCode);
                }
            }
        }

        public void DistributeControl(string prodCode, float price, int qty, bool isAO)
        {
            if (prodCode != null)
            {
                if (this.DisEnterOrderControl != null)
                {
                    this.DisEnterOrderControl(this, prodCode, price, qty, isAO);
                }
            }
        }


        public void DistributeControlOrder(MarketPriceItem mpi, string CellType)
        {
            if (mpi != null)
            {
                if (this.DisControlOrderItem != null)
                {
                    this.DisControlOrderItem(this, mpi, CellType);
                }
            }
        }


        public void DistributeControlFocus(string defaultTitle, MdiChild child)
        {
            if (child != null)
            {
                if (this.DisControlFocus != null)
                {
                    this.DisControlFocus(this, defaultTitle, child);
                }
            }
        }

        public void DistributeControlChangeTitle(string defaultTitle, MdiChild child)
        {
            if (child != null)
            {
                if (this.DisControlChangeTitle != null)
                {
                    this.DisControlChangeTitle(this, defaultTitle, child);
                }
            }
        }

        public void DistributeMsg(TradeStationComm.MsgResponse.ResponseObject responseObject)
        {
            try
            {
                if (responseObject != null)
                {
                    switch (responseObject.ResponseType)
                    {
                        case TradeStationComm.MsgResponse.responseType.errorMsg:
                            TradeStationLog.WriteParserResult(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + "Type: errorMsg");

                            TradeStationComm.infoClass.infoBasic theInfo = responseObject.InfoObject as TradeStationComm.infoClass.infoBasic;
                            if (theInfo is TradeStationComm.infoClass.infoBasic && theInfo != null)
                            {
                                if (this.DisGetError != null)
                                {
                                    this.DisGetError(this, new MessageException(theInfo.ErrorCode, theInfo.ResponseString));
                                    TradeStationLog.WriteParserResult(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + "Error code:" + theInfo.ErrorCode + ",error msg:" + theInfo.ResponseString);
                                }
                            }
                            break;
                        case TradeStationComm.MsgResponse.responseType.getInstrumentList:
                            TradeStationLog.WriteParserResult(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + "Type: getInstrumentList");

                            TradeStationComm.infoClass.InstrumentList infoInstList = responseObject.InfoObject as TradeStationComm.infoClass.InstrumentList;
                            if (infoInstList != null)
                            {
                                marketPriceData.InstListTable = infoInstList.GetInstrumentListTable(infoInstList.InfoItems);
                                marketPriceData.MarketCodeList = (infoInstList.GetMarketCodeList()).OrderBy(q => q).ToList(); ;

                                GOSTradeStation.marketPriceData.MarketCodeList = marketPriceData.MarketCodeList;
                                GOSTradeStation.marketPriceData.InstListTable = marketPriceData.InstListTable;

                                if (this.DisPriceManager != null && marketPriceData.ProdListTable != null && marketPriceData.MarketCodeList != null)
                                {
                                    this.DisPriceManager(this, marketPriceData);
                                    TradeStationLog.WriteParserResult(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + "Type: getInstrumentList and triger binding. " + infoInstList.Datetime);
                                }
                            }
                            break;
                        case TradeStationComm.MsgResponse.responseType.getProductList:
                            TradeStationLog.WriteParserResult(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + "Type: getProductList");

                            TradeStationComm.infoClass.ProductList infoProdList = responseObject.InfoObject as TradeStationComm.infoClass.ProductList;
                            if (infoProdList != null)
                            {
                                marketPriceData.ProdListTable = infoProdList.GetProductListTable(infoProdList.InfoItems);
                                GOSTradeStation.marketPriceData.ProdListTable = marketPriceData.ProdListTable;

                                if (this.DisGotProductList != null && marketPriceData.ProdListTable != null)
                                {
                                    this.DisGotProductList(this);
                                  //TradeStationLog.WriteParserResult(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + "Type: getProductList and triger." + infoProdList.Datetime);
                                }

                                if (this.DisPriceManager != null && marketPriceData.ProdListTable != null && marketPriceData.MarketCodeList != null)
                                {
                                    this.DisPriceManager(this, marketPriceData);
                                  //TradeStationLog.WriteParserResult(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + "Type: getProductList and triger binding. " + infoProdList.Datetime);

                                }
                            }
                            break;
                        case TradeStationComm.MsgResponse.responseType.getMarketPrice:
                           // TradeStationLog.WriteParserResult(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + "Type: getMarketPrice");

                            TradeStationComm.infoClass.MarketPrice infoMarkPrice = responseObject.InfoObject as TradeStationComm.infoClass.MarketPrice;
                            if (infoMarkPrice != null)
                            {
                                ObservableCollection<MarketPriceItem> marketPriceItems = infoMarkPrice.GetMarketPriceItems(infoMarkPrice.InfoItems);
                                //if (GOSTradeStation.marketPriceData.MarktePriceItems == null)
                                //{
                                //    GOSTradeStation.marketPriceData.MarktePriceItems = marketPriceItems;
                                //}
                                //else
                                //{
                                //    GOSTradeStation.marketPriceData.MarktePriceItems = Merge(GOSTradeStation.marketPriceData.MarktePriceItems, marketPriceItems);
                                //} 
                                if (this.DisMarketPrice != null && marketPriceItems != null)
                                {
                                    this.DisMarketPrice(this, marketPriceItems);
                                    TradeStationLog.WriteParserResult(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + "Type: getMarketPrice," + marketPriceItems.Count + " " + infoMarkPrice.Datetime);
                                }
                            }
                            break;
                        case TradeStationComm.MsgResponse.responseType.getPriceDepth:
                           // TradeStationLog.WriteParserResult(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + "Type: getPriceDepth");

                            TradeStationComm.infoClass.PriceDepth infoPriceDepth = responseObject.InfoObject as TradeStationComm.infoClass.PriceDepth;
                            if (infoPriceDepth != null)
                            {
                                DataTable priceDepthTable = infoPriceDepth.GetPriceDepthTable(infoPriceDepth.InfoItems);
                                //if (GOSTradeStation.marketPriceData.PriceDepthTable == null)
                                //{
                                //    GOSTradeStation.marketPriceData.PriceDepthTable = marketPriceData.PriceDepthTable;
                                //}
                                //else
                                //{
                                //    GOSTradeStation.marketPriceData.PriceDepthTable.Merge(marketPriceData.PriceDepthTable, false, MissingSchemaAction.Add);
                                //}

                                if (this.DisPMPriceDepth != null)
                                {
                                    this.DisPMPriceDepth(this, priceDepthTable);
                                    TradeStationLog.WriteParserResult(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + "Type: getPriceDepth and triger binding." + infoPriceDepth.Datetime);
                                }

                                ObservableCollection<LongPriceDepthData> priceDepthDatas = infoPriceDepth.GetPriceDepthItemData(infoPriceDepth.InfoItems);
                                
                                if (this.DisPMPriceDepthData != null)
                                {
                                    this.DisPMPriceDepthData(this, priceDepthDatas);
                                  //  TradeStationLog.WriteParserResult(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + "Type: getPriceDepth and triger binding EnterOrder." + infoPriceDepth.Datetime);
                                }
                            }
                            break;
                        case TradeStationComm.MsgResponse.responseType.getLongPriceDepth:
                            TradeStationLog.WriteParserResult(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + "Type: getLongPriceDepth");

                            TradeStationComm.infoClass.LongPriceDepth infoLongPriceDepth = responseObject.InfoObject as TradeStationComm.infoClass.LongPriceDepth;
                            if (infoLongPriceDepth != null)
                            {
                                List<string> prodCodes = (infoLongPriceDepth.GetProductCodeList() == null) ? new List<string>() : infoLongPriceDepth.GetProductCodeList();
                                //GOSTradeStation.marketPriceData.LongPriceDepthList = (GOSTradeStation.marketPriceData.LongPriceDepthList == null) ? prodCodes : GOSTradeStation.marketPriceData.LongPriceDepthList.Union(prodCodes).ToList();
                                LongPriceDepthViewModel viewModel = new LongPriceDepthViewModel();
                                if (prodCodes != null && prodCodes.Count > 0)
                                {
                                    foreach (string str in prodCodes)
                                    {
                                        viewModel.longPriceDepthData = (viewModel.longPriceDepthData == null) ? viewModel.GetLongPriceDepthData(str, infoLongPriceDepth.GetLongPriceDepthItem(str)) : viewModel.longPriceDepthData.Union(viewModel.GetLongPriceDepthData(str, infoLongPriceDepth.GetLongPriceDepthItem(str))).ToList();
                                    }
                                }

                                if (this.DisPMLongPriceDepth != null)
                                {
                                    this.DisPMLongPriceDepth(this, viewModel.longPriceDepthData);
                                    TradeStationLog.WriteParserResult(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + "Type: getLongPriceDepth and triger binding." + infoLongPriceDepth.Datetime);
                                }
                            }
                            break;
                        case TradeStationComm.MsgResponse.responseType.getTicker:
                            TradeStationLog.WriteParserResult(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + "Type: getTicker");

                            TradeStationComm.infoClass.Ticker infoTicker = responseObject.InfoObject as TradeStationComm.infoClass.Ticker;
                            if (infoTicker != null)
                            {
                                ObservableCollection<TickerData> tickerData = infoTicker.GetTickerItemData(infoTicker.InfoItems);
                                if (this.DisPMTickerItemData != null)
                                {
                                    this.DisPMTickerItemData(this, tickerData);
                                    TradeStationLog.WriteParserResult(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + "Type: getTicker and triger binding. " + infoTicker.Datetime);
                                }
                            }
                            break;
                        case TradeStationComm.MsgResponse.responseType.registerMarketPrice:
                            break;
                        case TradeStationComm.MsgResponse.responseType.registerLongPriceDepth:
                            break;
                        case TradeStationComm.MsgResponse.responseType.registerPriceDepth:
                            break;
                        case TradeStationComm.MsgResponse.responseType.getTradeStatistics:
                            TradeStationLog.WriteParserResult(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + "Type: getTradeStatistics");

                            TradeStationComm.infoClass.TradeStatistics infoStats = responseObject.InfoObject as TradeStationComm.infoClass.TradeStatistics;
                            if (infoStats != null)
                            {
                                DataTable statisticTable = infoStats.GetStatisticTable((infoStats.InfoItems));
                                if (this.DisTradeStatistic != null)
                                {
                                    this.DisTradeStatistic(this, statisticTable);
                                    TradeStationLog.WriteParserResult(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + "Type: getTradeStatistics, " + infoStats.Datetime);
                                }
                            }
                            break;
                        case TradeStationComm.MsgResponse.responseType.notificationMsg:
                            TradeStationLog.WriteForNotificationsOpening(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + "Type: notification");

                            TradeStationComm.infoClass.NotificationInfo infoNotification = responseObject.InfoObject as TradeStationComm.infoClass.NotificationInfo;
                            if (infoNotification != null)
                            {
                                Notification notification = new TradeStationNotification().GetNotification(infoNotification.InfoItem.AllItem);
                                if (this.DisNotification != null)
                                {
                                    this.DisNotification(this, notification, null);
                                    TradeStationLog.WriteForNotificationsOpening(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff ") + "Type: Notification,Notify_Code:" + notification.Notify_Code + ",level:" + notification.Level + ",msg:" + notification.Message + ",datetime:" + notification.Datetime);
                                }
                            }
                            break;
                        case TradeStationComm.MsgResponse.responseType.tableNotification:
                            TradeStationLog.WriteForNotificationsOpening(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + "Type: tableNotification ");

                            TradeStationComm.infoClass.TableNotificationInfo infoTableNotification = responseObject.InfoObject as TradeStationComm.infoClass.TableNotificationInfo;
                            if (infoTableNotification != null)
                            {
                                TableNotification tableNotification = new TradeStationNotification().GetTableNotification(infoTableNotification.InfoItem.AllItem);

                                if (this.DisNotification != null)
                                {
                                    this.DisNotification(this, null, tableNotification);
                                    TradeStationLog.WriteForNotificationsOpening(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff ") + "Type: tableNotification,TableCode:" + tableNotification.TableCode + ",level:" + tableNotification.Level + ",msg:" + tableNotification.Message + ",datetime:" + tableNotification.Datetime);
                                }
                            }
                            break;

                        case TradeStationComm.MsgResponse.responseType.getAccList:
                            TradeStationLog.WriteParserResult(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + "Type: getAccList");

                            // removed by ben 2014/06/20, ignore received AccList,manually add Acc by user(Harry added)

                            //TradeStationComm.infoClass.AccList infoAccList = responseObject.InfoObject as TradeStationComm.infoClass.AccList;
                            //if (infoAccList != null)
                            //{
                            //    GOSTradeStation.userData.AccountTable = infoAccList.GetAccListItemTable(infoAccList.InfoItems); ;
                            //    if (GOSTradeStation.userData.AccountTable != null)
                            //    {
                            //        if (this.DisAccoutList != null)
                            //        {
                            //            this.DisAccoutList(this, GOSTradeStation.userData.AccountTable);
                            //            TradeStationLog.WriteParserResult(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + "Type: getAccList, " + infoAccList.Datetime);
                            //        }
                            //    }
                            //} 
                            break;
                        case TradeStationComm.MsgResponse.responseType.getMarginCheck:
                            TradeStationLog.WriteParserResult(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + "Type: getMarginCheck");

                            TradeStationComm.infoClass.MarginCheck infoMarginCheck = responseObject.InfoObject as TradeStationComm.infoClass.MarginCheck;
                            if (infoMarginCheck != null)
                            {
                                if (infoMarginCheck.RInfo.IndexOf("op=A") > -1)
                                {
                                    ObservableCollection<MarginCheckAccData> marginCheckData = infoMarginCheck.GetMarginCheckItemData(infoMarginCheck.InfoItems);
                                    if (this.DisMarginCheck != null)
                                    {
                                        this.DisMarginCheck(this, marginCheckData);
                                        TradeStationLog.WriteParserResult(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + "Type: getMarginCheck(op=A)");
                                    }
                                }
                                else if (infoMarginCheck.RInfo.IndexOf("op=C") > -1)
                                {
                                    ObservableCollection<MarginCheckAccData> marginCallListData = infoMarginCheck.GetMarginCheckItemData(infoMarginCheck.InfoItems);
                                    if (this.DisMarginCallList != null)
                                    {
                                        this.DisMarginCallList(this, marginCallListData);
                                        TradeStationLog.WriteParserResult(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + "Type: getMarginCheck(op=C)");
                                    }
                                }
                            }
                            break;
                        case TradeStationComm.MsgResponse.responseType.getAccountMaster:
                            TradeStationLog.WriteParserResult(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + "Type: getAccountMaster");

                            TradeStationComm.infoClass.AccountMaster accountMaster = responseObject.InfoObject as TradeStationComm.infoClass.AccountMaster;
                            //harry 2014-04-01,get receive acc from server at rinfo:sp=|&bd=\"123456789||\"
                            string RecAcc = "";
                            if (accountMaster != null && accountMaster.InfoItems != null)
                            {
                                if (accountMaster.RInfo != null)
                                {
                                    string rinfo = accountMaster.RInfo;
                                    int pos = rinfo.IndexOf("bd=\"");
                                    if (pos > 0)
                                    {
                                        rinfo = rinfo.Substring(pos + 4);
                                    }
                                    pos = rinfo.IndexOf("||\"");
                                    if (pos >= 0)
                                    {
                                        rinfo = rinfo.Substring(0, pos);
                                    }
                                    if (rinfo != "")
                                    {
                                        RecAcc = rinfo;
                                    }
                                }
                                //string rinfo=responseObject.InfoObject.r
                                //sp=|&bd=\"123456789||\"
                                DataTable accountMasterTable = accountMaster.GetAccountMasterItemTable(accountMaster.InfoItems);
                                if (accountMasterTable != null)
                                {
                                    if (this.DisAccountMaster != null)
                                    {
                                        this.DisAccountMaster(this, accountMasterTable, RecAcc == "" ? accountMaster.AccNo : RecAcc);
                                        TradeStationLog.WriteParserResult(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + "Type: getAccountMaster, " + accountMaster.Datetime);
                                    }
                                }
                            }
                            break;
                        case TradeStationComm.MsgResponse.responseType.getTradeConfOrders:
                            TradeStationLog.WriteParserResult(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + "Type: getTradeConfOrders");

                            TradeStationComm.infoClass.TradeConfOrder tradeConfOrder = responseObject.InfoObject as TradeStationComm.infoClass.TradeConfOrder;
                            if (tradeConfOrder != null && tradeConfOrder.InfoItems != null)
                            {
                                DataTable table = tradeConfOrder.GetTradeConfOrderItemTable(tradeConfOrder.InfoItems);
                                if (table != null)
                                {
                                    if (this.DisTradeConfOrder != null)
                                    {
                                        this.DisTradeConfOrder(this, table);
                                        TradeStationLog.WriteParserResult(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + "Type: getTradeConfOrders, " + table.Rows.Count.ToString());
                                    }
                                }
                            }
                            break;
                        case TradeStationComm.MsgResponse.responseType.getTradeConfTrades:
                            TradeStationLog.WriteParserResult(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + "Type: getTradeConfTrades");

                            TradeStationComm.infoClass.TradeConfTrade tradeConfTrade = responseObject.InfoObject as TradeStationComm.infoClass.TradeConfTrade;
                            if (tradeConfTrade != null && tradeConfTrade.InfoItems != null)
                            {
                                DataTable table = tradeConfTrade.GetTradeConfTradeItemTable(tradeConfTrade.InfoItems);
                                if (table != null)
                                {
                                    if (this.DisTradeConfTrade != null)
                                    {
                                        TradeStationLog.WriteParserResult(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + "Type: getTradeConfTrades, " + table.Rows.Count.ToString());
                                        this.DisTradeConfTrade(this, table);
                                    }
                                }
                            }
                            break;

                        case TradeStationComm.MsgResponse.responseType.reportTradeConf:
                            TradeStationLog.WriteParserResult(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + "Type: reportTradeConf");

                            TradeStationComm.infoClass.ReportTradeConfAck reportTradeConfAck = responseObject.InfoObject as TradeStationComm.infoClass.ReportTradeConfAck;
                            if (reportTradeConfAck != null && reportTradeConfAck.InfoItem != null)
                            {
                                string str = reportTradeConfAck.GetReportTradeConfAckItem(reportTradeConfAck.InfoItem);
                                if (str != null)
                                {
                                    if (this.DisReportTradeConf != null)
                                    {
                                        this.DisReportTradeConf(this, str);
                                        TradeStationLog.WriteParserResult(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + "Type: reportTradeConf," + str);
                                    }
                                }
                            }
                            break;
                        case TradeStationComm.MsgResponse.responseType.getTradeConfOrderDetail:
                            TradeStationLog.WriteParserResult(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + "Type: getTradeConfOrderDetail");

                            TradeStationComm.infoClass.TradeConfOrderDetail tradeConfOrderDetail = responseObject.InfoObject as TradeStationComm.infoClass.TradeConfOrderDetail;
                            if (tradeConfOrderDetail != null && tradeConfOrderDetail.InfoItems != null)
                            {
                                DataTable table = tradeConfOrderDetail.GetTradeConfOrderDetailTable(tradeConfOrderDetail.InfoItems);
                                if (table != null)
                                {
                                    if (this.DisTradeConfOrderDetail != null)
                                    {
                                        this.DisTradeConfOrderDetail(this, table);
                                        TradeStationLog.WriteParserResult(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + "Type: getTradeConfOrderDetail, " + table.Rows.Count.ToString());
                                    }
                                }
                            }
                            break;
                        case TradeStationComm.MsgResponse.responseType.getAccountInfo:
                            TradeStationLog.WriteParserResult(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + "Type: getAccountInfo");

                            TradeStationComm.infoClass.AccountInfo accountInfo = responseObject.InfoObject as TradeStationComm.infoClass.AccountInfo;
                            DataTable dtAccountInfo = accountInfo.GetAccountInfoItemTable(accountInfo.InfoItem);
                            string accinfo_user = "", accinfo_acc = "";
                            if (accountInfo.InfoItem.AllItem != null)
                            {
                                if (accountInfo.InfoItem.AllItem.Length > 2)
                                {
                                    accinfo_user = accountInfo.InfoItem.AllItem[1].Trim();
                                }
                                if (accountInfo.InfoItem.AllItem.Length > 3)
                                {
                                    accinfo_acc = accountInfo.InfoItem.AllItem[2].Trim();
                                }
                            }

                            if (this.DisUserAccountInfo != null)
                            {
                                this.DisUserAccountInfo(this, dtAccountInfo, accinfo_user, accinfo_acc, null);
                                TradeStationLog.WriteParserResult(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + "Type: getAccountInfo, " + TradeStationTools.getDateTimeFromUnixTime(accountInfo.InfoItem.AllItem[0].ToString()));
                            }
                            break;
                        case TradeStationComm.MsgResponse.responseType.getCashInfo:
                            TradeStationLog.WriteParserResult(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + "Type: getCashInfo");

                            TradeStationComm.infoClass.CashInfo CashInfo = responseObject.InfoObject as TradeStationComm.infoClass.CashInfo;
                            DataTable dtCashInfo = CashInfo.GetCashInfoItemTable(CashInfo.InfoItems);
                            if (this.DisCashInfo != null)
                            {
                                this.DisCashInfo(this, dtCashInfo, CashInfo.UserID, CashInfo.AccNo, CashInfo.Datetime);
                                TradeStationLog.WriteParserResult(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + "Type: getCashInfo, " + CashInfo.Datetime);
                            }
                            break;
                        case TradeStationComm.MsgResponse.responseType.activateOrder:
                            TradeStationLog.WriteError(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + "Type: activateOrder");
                            TradeStationLog.WriteForNotificationsOpening(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + "Type: activateOrder");

                            TradeStationComm.infoClass.infoBasic ActivateOrderInfo = responseObject.InfoObject as TradeStationComm.infoClass.infoBasic;
                            if (ActivateOrderInfo != null)
                            {
                                OrderResponse orderResponse = ActivateOrderInfo.GetOrderResponse(ActivateOrderInfo.InfoItem.AllItem);
                                if (this.DisActivateOrder != null)
                                {
                                    this.DisActivateOrder(this, orderResponse);
                                    TradeStationLog.WriteParserResult(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + "Type: activateOrder, " + TradeStationTools.getDateTimeFromUnixTime(ActivateOrderInfo.InfoItem.AllItem[0].ToString()));
                                    TradeStationLog.WriteForNotificationsOpening(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff ") + "Type: activateOrder,msg:" + orderResponse.Message);
                                }
                            }
                            break;
                        case TradeStationComm.MsgResponse.responseType.inactivateOrder:
                            TradeStationLog.WriteParserResult(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + "Type: inactivateOrder");
                            TradeStationLog.WriteForNotificationsOpening(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + "Type: inactivateOrder");

                            TradeStationComm.infoClass.infoBasic inactivateOrderInfo = responseObject.InfoObject as TradeStationComm.infoClass.infoBasic;
                            if (inactivateOrderInfo != null)
                            {
                                OrderResponse orderResponse = inactivateOrderInfo.GetOrderResponse(inactivateOrderInfo.InfoItem.AllItem);
                                if (this.DisInactivateOrder != null)
                                {
                                    this.DisInactivateOrder(this, orderResponse);
                                    TradeStationLog.WriteParserResult(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + "Type: inactivateOrder, " + TradeStationTools.getDateTimeFromUnixTime(inactivateOrderInfo.InfoItem.AllItem[0].ToString()));
                                    TradeStationLog.WriteForNotificationsOpening(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff ") + "Type: inactivateOrder,msg:" + orderResponse.Message);
                                }
                            }
                            break;
                        case TradeStationComm.MsgResponse.responseType.deleteOrder:
                            TradeStationLog.WriteParserResult(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + "Type: deleteOrder");
                            TradeStationLog.WriteForNotificationsOpening(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + "Type: deleteOrder");

                            TradeStationComm.infoClass.infoBasic deleteOrderInfo = responseObject.InfoObject as TradeStationComm.infoClass.infoBasic;
                            if (deleteOrderInfo != null)
                            {
                                OrderResponse orderResponse = deleteOrderInfo.GetOrderResponse(deleteOrderInfo.InfoItem.AllItem);
                                if (this.DisDeleteOrder != null)
                                {
                                    this.DisDeleteOrder(this, orderResponse);
                                    TradeStationLog.WriteParserResult(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + "Type: deleteOrder, " + TradeStationTools.getDateTimeFromUnixTime(deleteOrderInfo.InfoItem.AllItem[0].ToString()));
                                    TradeStationLog.WriteForNotificationsOpening(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff ") + "Type: deleteOrder,msg:" + orderResponse.Message);
                                }
                            }
                            break;
                        case TradeStationComm.MsgResponse.responseType.addOrder:
                            TradeStationLog.WriteParserResult(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + "Type: addOrder");
                            TradeStationLog.WriteForNotificationsOpening(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + "Type: addOrder");

                            TradeStationComm.infoClass.infoBasic addOrderInfo = responseObject.InfoObject as TradeStationComm.infoClass.infoBasic;
                            if (addOrderInfo != null)
                            {
                                OrderResponse orderResponse = addOrderInfo.GetOrderResponse(addOrderInfo.InfoItem.AllItem);
                                if (this.DisAddOrder != null)
                                {
                                    this.DisAddOrder(this, orderResponse);
                                    TradeStationLog.WriteParserResult(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + "Type: addOrder, " + TradeStationTools.getDateTimeFromUnixTime(addOrderInfo.InfoItem.AllItem[0].ToString()));
                                    TradeStationLog.WriteForNotificationsOpening(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff ") + "Type: addOrder,msg:" + orderResponse.Message);
                                }
                            }
                            break;
                        case TradeStationComm.MsgResponse.responseType.changeOrder:
                            TradeStationLog.WriteParserResult(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + "Type: changeOrder");
                            TradeStationLog.WriteForNotificationsOpening(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + "Type: changeOrder");

                            TradeStationComm.infoClass.infoBasic ChangeOrderInfo = responseObject.InfoObject as TradeStationComm.infoClass.infoBasic;
                            if (ChangeOrderInfo != null)
                            {
                                OrderResponse orderResponse = ChangeOrderInfo.GetOrderResponse(ChangeOrderInfo.InfoItem.AllItem);
                                if (this.DisChangeOrder != null)
                                {
                                    this.DisChangeOrder(this, orderResponse);
                                    TradeStationLog.WriteParserResult(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + "Type: changeOrder, " + TradeStationTools.getDateTimeFromUnixTime(ChangeOrderInfo.InfoItem.AllItem[0].ToString()));
                                    TradeStationLog.WriteForNotificationsOpening(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff ") + "Type: changeOrder,msg:" + orderResponse.Message);
                                }
                            }
                            break;
                        case TradeStationComm.MsgResponse.responseType.getOrderBookInfo:
                            TradeStationLog.WriteParserResult(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + "Type: getOrderBookInfo");

                            TradeStationComm.infoClass.OrderBook OrderBookInfo = responseObject.InfoObject as TradeStationComm.infoClass.OrderBook;
                            if (OrderBookInfo != null)
                            {
                                if (this.DisUserOrderInfo != null)
                                {
                                    DataTable dtOrderBook = OrderBookInfo.GetOrderBookItemTable(OrderBookInfo.InfoItems);
                                    this.DisUserOrderInfo(this, dtOrderBook, OrderBookInfo.UserID, OrderBookInfo.AccNo, OrderBookInfo.Datetime);

                                    TradeStationLog.WriteParserResult(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + "Type: getOrderBookInfo, " + dtOrderBook.Rows.Count.ToString() + "  " + OrderBookInfo.Datetime);
                                   // TradeStationLog.WriteLogPerformance(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + "Type: getOrderBookInfo, " + OrderBookInfo.AccNo + ", " + dtOrderBook.Rows.Count.ToString() + "  " + OrderBookInfo.Datetime);
                                }
                            }
                            break;

                        case TradeStationComm.MsgResponse.responseType.getClearTradeInfo:
                            TradeStationLog.WriteParserResult(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + "Type: getClearTradeInfo");

                            TradeStationComm.infoClass.ClearTrade ClearTradeInfo = responseObject.InfoObject as TradeStationComm.infoClass.ClearTrade;
                            DataTable dtClearTradeInfo = ClearTradeInfo.GetClearTradeInfoItemTable(ClearTradeInfo.InfoItems);
                            if (this.DisClearTradeInfo != null)
                            {
                                this.DisClearTradeInfo(this, dtClearTradeInfo, ClearTradeInfo.UserID, ClearTradeInfo.AccNo, ClearTradeInfo.Datetime);
                                TradeStationLog.WriteParserResult(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + "Type: getClearTradeInfo, " + ClearTradeInfo.Datetime);
                            }
                            break;
                        case TradeStationComm.MsgResponse.responseType.getDoneTradeInfo:
                            TradeStationLog.WriteParserResult(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff ") + "Type: getDoneTradeInfo");

                            TradeStationComm.infoClass.DoneTrade DoneTradeInfo = responseObject.InfoObject as TradeStationComm.infoClass.DoneTrade;
                            DataTable dtDoneTradeInfo = DoneTradeInfo.GetDoneTradeInfoItemTable(DoneTradeInfo.InfoItems);
                            if (this.DisDoneTradeInfo != null)
                            {
                                this.DisDoneTradeInfo(this, dtDoneTradeInfo, DoneTradeInfo.UserID, DoneTradeInfo.AccNo, DoneTradeInfo.Datetime);
                                TradeStationLog.WriteParserResult(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + "Type: getDoneTradeInfo, " + DoneTradeInfo.Datetime);
                            }
                            break;

                        case TradeStationComm.MsgResponse.responseType.getPositionInfo:
                            TradeStationLog.WriteParserResult(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + "Type: getPositionInfo");

                            TradeStationComm.infoClass.Position PositionInfo = responseObject.InfoObject as TradeStationComm.infoClass.Position;
                            DataTable dtPositionInfo = PositionInfo.GetPositionItemTable(PositionInfo.InfoItems);
                            if (this.DisPositionInfo != null)
                            {
                                this.DisPositionInfo(this, dtPositionInfo, PositionInfo.UserID, PositionInfo.AccNo, PositionInfo.Datetime);
                                TradeStationLog.WriteParserResult(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + "Type: getPositionInfo, " + PositionInfo.Datetime);
                            }
                            break;
                        case TradeStationComm.MsgResponse.responseType.getChart:
                            TradeStationLog.WriteParserResult(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + "Type: getChart");

                            TradeStationComm.infoClass.Chart chart = responseObject.InfoObject as TradeStationComm.infoClass.Chart;
                            TradeStationComm.infoClass.Chart.ChartHistoryData chartHistory = chart.GetChartInfoTable(chart);
                            if (this.DisChartInfo != null)
                            {
                                this.DisChartInfo(this, chartHistory);
                                TradeStationLog.WriteParserResult(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + "Type: getChart, " + chart.Datetime);
                            }
                            break;
                        case TradeStationComm.MsgResponse.responseType.changePassword:
                            TradeStationComm.infoClass.infoBasic respInfo = responseObject.InfoObject as TradeStationComm.infoClass.infoBasic;
                            if (this.DisChangePassword != null)
                            {
                                //HARRY 2014-01-22.    RECEIVE PASSWORD CHANGED MESSAGE FROM SERVER
                                if (respInfo.InfoItem != null)
                                {
                                    string chUser = "";
                                    if (respInfo.InfoItem.AllItem.Length > 1)
                                    {
                                        chUser = respInfo.InfoItem.AllItem[1];
                                    }
                                    this.DisChangePassword(this, respInfo.ResponseString, respInfo.IsSuccess, chUser);
                                }
                            }
                            break;
                        case TradeStationComm.MsgResponse.responseType.ChangeLanguage :
                            TradeStationLog.WriteParserResult(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + "Type: ChangeLanguage");

                            TradeStationComm.infoClass.infoBasic changeLanguage = responseObject.InfoObject as TradeStationComm.infoClass.infoBasic;
                            if (this.DisChangeLanguage != null)
                            { 
                                if (changeLanguage.InfoItem != null)
                                { 
                                    this.DisChangeLanguage(this, "");
                                }
                            }
                            break;
                    }
                }
            }
            catch (Exception exp)
            {
                TradeStationLog.WriteError(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "  MessageDistribute()" + exp);
            }
        }

        private static ObservableCollection<MarketPriceItem> Merge(ObservableCollection<MarketPriceItem> one, ObservableCollection<MarketPriceItem> two)
        {
            ObservableCollection<MarketPriceItem> newCollection = new ObservableCollection<MarketPriceItem>(one);

            foreach (MarketPriceItem e in two)
            {
                MarketPriceItem tmp = newCollection.FirstOrDefault(x => x.ProductCode == e.ProductCode);
                if (tmp == null)
                {
                    newCollection.Add(e);
                }
                else
                {
                    tmp.Datetime = e.Datetime;
                    tmp.ProductName = e.ProductName;
                    tmp.Expiry = e.Expiry;
                    tmp.ProductStatus = e.ProductStatus;
                    tmp.BQty = e.BQty;
                    tmp.Bid = e.Bid;
                    tmp.Ask = e.Ask;
                    tmp.AQty = e.AQty;
                    tmp.Last = e.Last;
                    tmp.EP = e.EP;
                    tmp.LQty = e.LQty;
                    tmp.Change = e.Change;
                    tmp.ChangePer = e.ChangePer;
                    tmp.Volume = e.Volume;
                    tmp.High = e.High;
                    tmp.Low = e.Low;
                    tmp.Open = e.Open;
                    tmp.PreClose = e.PreClose;
                    tmp.CloseDate = e.CloseDate;
                    tmp.Strike = e.Strike;
                }
            }
            return newCollection;
        }

        public void Dispose()
        {
            if (disposed) return;

            _responseObject = null;
            marketPriceData.InstListTable = null;
            marketPriceData.ProdListTable = null;
            marketPriceData.MarketCodeList = null;
            marketPriceData.MarketTreeTable = null;
            marketPriceData = null;

            disposed = true;
        }
    }
}
