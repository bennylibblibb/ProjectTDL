using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace GOSTS
{
    public struct AppFlag
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

        internal static readonly string UserData = AppDomain.CurrentDomain.BaseDirectory.ToString() + ConfigurationManager.AppSettings["UserData"];
        internal static string ConfigFile = ConfigurationManager.AppSettings["ConfigFile"];
        internal static readonly string Url = UserData + "GOSTS.XML";
        internal static string DefaultUser = ConfigurationManager.AppSettings["DefaultUser"];
        internal static string DefaultLanguage = ConfigurationManager.AppSettings["DefaultLanguage"].Trim();
        internal static readonly int MCLReload = Convert.ToInt32(ConfigurationManager.AppSettings["MCLReload"]) * 1000;

        internal static readonly string BuildDate = ConfigurationManager.AppSettings["BuildDate"];

        internal static readonly string CustomizeFile = ConfigurationManager.AppSettings["CustomizeFile"];

        internal static readonly bool Loged = (ConfigurationManager.AppSettings["Loged"].ToUpper() == "FALSE") ? false : true;
        //internal static readonly bool DebugLoged = (ConfigurationManager.AppSettings["DebugLoged"].ToUpper() == "FALSE") ? false : true;
        //internal static readonly bool DealerLoged = (ConfigurationManager.AppSettings["DealerLoged"].ToUpper() == "FALSE") ? false : true;
        internal static readonly bool isDebugNonDelaer = false ;

        internal static readonly int InvalidNum = Convert.ToInt32(ConfigurationManager.AppSettings["InvalidNum"]);
        internal static readonly int AONum = Convert.ToInt32(ConfigurationManager.AppSettings["AONum"]);

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

        static string MenuOption = ConfigurationManager.AppSettings["bRobbinShow"].Trim();
        /// <summary>
        /// whether show robbin menu
        /// </summary>
        internal static bool bRobbin
        {
            get
            {
                if (MenuOption == "1") return true;
                return false;
            }
        }

        internal static readonly string strAOConst = "AO";
        internal static readonly int intAOConst = InvalidNum;

        // internal static readonly string UpdateXmlUrl = ConfigurationManager.AppSettings["UpdateXmlUrl"];
        public static readonly string VersionInfoFile = ConfigurationManager.AppSettings["VersionInfoFile"];
        public static readonly int MaximaTicker = Convert.ToInt32(ConfigurationManager.AppSettings["MaximaTicker"]);
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
        changePassword,
        changeLanguage

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
        PriceAlert,
        AddPriceAlert,
        Login
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

    public enum NotifyTypeCode
    {
        Reserved = 0,
        OrderBookUpdated = 1,
        DoneTradeUpdated = 2,
        OrderAddSuccess = 3,
        OrderAddFailure = 4,
        OrderChangeSuccess = 5,
        OrderChangeFailure = 6,
        OrderDeleteSuccess = 7,
        OrderDeleteFailure = 8,
        OrderActivateSuccess = 9,
        OrderActivateFailure = 10,
        OrderInactivateSuccess = 11,
        OrderInactivateFailure = 12,
        OMserverDisconnect = 13,
        OMserverConnected = 14,
        ForceUserLogout = 15,
    }

    public enum TableNotifyCode
    { 
        Reserved = 0,
        OrderBookUpdated = 1,
        DoneTradeUpdated = 2,
        PositionUpdated = 3,
        CashInfoUpdate = 4,
        AccountInfoUpdated = 5,
        MarginCAllListUpdated = 6,
        SystemPparametersUpdated = 7, 
    }

    public enum StatusCode
    {
        Reserved = 0,
        AccessToOMT = 1,
        AccessToOMP =2,
        OMTConnecting = 3,
        OMPConnecting = 4,
        OMTConnected = 5,
        OMPConnected = 6, 
        Error=255,
    }  
}
