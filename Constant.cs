using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace WPF.MDI
{
    internal struct AppFlag
    {
        public static string IPOMT = "";// ConfigurationManager.AppSettings["IP"];
        public static int PortOMT = Convert.ToInt32(ConfigurationManager.AppSettings["Port"]);
        public static string IPOMP = "";// ConfigurationManager.AppSettings["IPOMP"];
        public static int PortOMP = Convert.ToInt32(ConfigurationManager.AppSettings["PortOMP"]);
        internal static readonly string ErrorFolder = ConfigurationManager.AppSettings["ErrorFolder"];
        internal static readonly string EventFolder = ConfigurationManager.AppSettings["EventFolder"];
        internal static readonly string Log = ConfigurationManager.AppSettings["Log"];
        internal static readonly string Error = ConfigurationManager.AppSettings["Error"];
        internal static readonly int TimeOutDelay = Convert.ToInt32(ConfigurationManager.AppSettings["TimeOutDelay"]) * 1000;
        internal static readonly int HeartBeatDelay = Convert.ToInt32(ConfigurationManager.AppSettings["HeartBeatDelay"]) * 1000;

        internal static readonly int ReconnectDelay = Convert.ToInt32(ConfigurationManager.AppSettings["ReconnectDelay"]) * 1000;
        internal static readonly int ConnectRetries = Convert.ToInt32(ConfigurationManager.AppSettings["ConnectRetries"]);

        internal static readonly string UserData = ConfigurationManager.AppSettings["UserData"];
        internal static string ConfigFile = ConfigurationManager.AppSettings["ConfigFile"];
        internal static readonly string Url = UserData + "GOSTS.XML";
        internal static string DefaultUser = ConfigurationManager.AppSettings["DefaultUser"];
        internal static readonly int MCLReload = Convert.ToInt32(ConfigurationManager.AppSettings["MCLReload"]) * 1000;

        internal static readonly string BuildDate = ConfigurationManager.AppSettings["BuildDate"];

        internal static readonly string CustomizeFile = ConfigurationManager.AppSettings["CustomizeFile"];

        internal static readonly bool Loged = (ConfigurationManager.AppSettings["Loged"].ToUpper() == "FALSE") ? false : true;

        internal static readonly string InvalidNum = ConfigurationManager.AppSettings["InvalidNum"];

        internal static readonly decimal PriceUpBound = Convert.ToInt32(ConfigurationManager.AppSettings["PriceUpBound"]);
        internal static readonly decimal PriceDownBound = Convert.ToInt32(ConfigurationManager.AppSettings["PriceDownBound"]);

        internal static readonly decimal QtyUpBound = Convert.ToInt32(ConfigurationManager.AppSettings["QtyUpBound"]);
        internal static readonly decimal QtyDownBound = Convert.ToInt32(ConfigurationManager.AppSettings["QtyDownBound"]);

        internal static readonly int SyncDuration = Convert.ToInt32(ConfigurationManager.AppSettings["SyncDuration"]);
        internal static readonly int AccountInfoBGFlag = Convert.ToInt32(ConfigurationManager.AppSettings["AccountInfoBGFlag"]);
        internal static readonly string Servers = ConfigurationManager.AppSettings["Servers"];
        internal static readonly string RandomTest = ConfigurationManager.AppSettings["RandomTest"];
        internal static bool EnabledRandomTest()
        {
            if (RandomTest.ToLower() == "enabled")
            {
                return true;
            }
            return false;
        }
        internal static readonly string RandomTestProds = ConfigurationManager.AppSettings["RandomTestProdList"];    
        internal static readonly Int32 intRandomMin = Convert.ToInt32(ConfigurationManager.AppSettings["RandomMin"]);
        internal static readonly Int32 intRandomMax = Convert.ToInt32(ConfigurationManager.AppSettings["RandomMax"]);
        internal static readonly Int32 intDgPageSize = Convert.ToInt32(ConfigurationManager.AppSettings["dgPageSize"]);
    }

    public enum cmdClient
    {
        invalidation,
        getMarketPrice,
        registerMarketPrice,
        getPriceDepth,
        registerPriceDepth,
        getLongPriceDepth,
        registerLongPriceDepth,
        getProductList,
        getInstrumentList,
        getTicker = 10,
        registerTicker,
        getTradeStatistics,

        getAccountInfo,
        getOrderBookInfo,
        getPositionInfo,
        getPositionSummary,
        getCashInfo,
        getDoneTradeInfo,
        getClearTradeInfo,
        getAccList,

        getMarginCheck,
        getMarginCallList,
        reqCashApproval,
        tableNotificationAck = 253,
        notificationAck = 254,

        addOrder,
        changeOrder,
        deleteOrder,
        activateOrder,
        inactivateOrder,

        getMsgAccountMaster,
        getMsgLogin,
        getMsgLogout,
        getTradeConfOrders,
        getTradeConfTrades,
        reportTradeConf,
        getTradeConfOrderDetail,
        getMsgHeartBeat,
        getMsgHeartBeatOMP,
        getChart,
        changePassword

    }

    public enum WindowTypes
    {
        MarketPriceControl,
        PriceDepth,
        LongPriceDepth,
        Ticker,
        AccountInfo,
        UserOrderInfo,
        OrderEntry,
        OrderConfirm,
        MarginCallList,
        PosSumary,
        MarginCheck,
        AppPreference,
        ClosePosition,
        ChangeOrder,
        TradeConfirmation,
        TradeConfirmDetails,
        Clock,
        SCIChartAnalysis,
        OptionMaster,
        PriceAlert
    }

    public enum NotifyType
    {
        Reserved,
        OrderBookUpdated,
        DoneTradeUpdated,
        OrderAddSuccess,
        OrderAddFailure,
        OrderChangeSuccess,
        OrderChangeFailure,
        OrderDeleteSuccess,
        OrderDeleteFailure,
        OrderActivateSuccess,
        OrderActivateFailure,
        OrderInactivateSuccess,
        OrderInactivateFailure,
        OMServerDisconnect,
        OMServerConnected
    }

    public enum xmlType
    {
        Desktop,
        Customize,
        Server
    }

    public enum HandleType
    {
        Run,
        Stop,
        Alerted,
        Delete,

        Add,
        Remove,
        Update
    }

}
