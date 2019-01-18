using System;
using System.Configuration;
using System.Runtime.InteropServices; 
using System.Net;
using System.Net.Sockets; 
using System.Net.NetworkInformation;

namespace DataOfScouts
{
    internal struct AppFlag
    { 
        public static string ScoutsDBConn = ConfigurationSettings.AppSettings["ScoutsDBConn"];
        public static string HkjcDBConn = ConfigurationSettings.AppSettings["HkjcDBConn"];
        public static string Client_id = ConfigurationSettings.AppSettings["Client_id"];
        public static string Secret_key = ConfigurationSettings.AppSettings["Secret_key"];
        public static string ScoutsUrl = ConfigurationSettings.AppSettings["ScoutsUrl"];
        public static int iPageSize =Convert .ToInt32(ConfigurationSettings.AppSettings["PageSize"]);
        public static int iQueryDays = Convert.ToInt32(ConfigurationSettings.AppSettings["QueryDays"]);
        public static string SyncHkjcDateTime = ConfigurationSettings.AppSettings["SyncHkjcDateTime"];
        public static bool JsonType = ConfigurationSettings.AppSettings["JsonType"].ToLower ()=="all"?false:true;
        public static string GetTime = ConfigurationSettings.AppSettings["GetTime"];
        public static bool AutoBooked = ConfigurationSettings.AppSettings["AutoBooked"].ToLower() == "yes" ? false : true;
        internal static readonly int MarginOfDeviation = Convert.ToInt32(ConfigurationSettings.AppSettings["MarginOfDeviation"]);
        internal static string lpString = ConfigurationSettings.AppSettings["lpString"];   
        internal static readonly int LIVEGOALS = Convert.ToInt32(ConfigurationSettings.AppSettings["LIVEGOALS"]);
        internal static readonly int GOALDETAILS = Convert.ToInt32(ConfigurationSettings.AppSettings["GOALDETAILS"]);  
        internal static readonly int IntervalSync = Convert.ToInt32(ConfigurationSettings.AppSettings["IntervalSync"]);
        public static string UnbookFiles = ConfigurationSettings.AppSettings["UnbookFiles"];
        internal static bool AutoMapping = ConfigurationSettings.AppSettings["AutoMapping"].ToLower() == "yes" ? true : false;
        internal static bool TestMode = ConfigurationSettings.AppSettings["TestMode"].ToLower() == "yes" ? true : false;
        internal static bool Important_for_trader = ConfigurationSettings.AppSettings["Important_for_trader"].ToLower() == "yes" ? true : false;
        internal static bool ManualGet = ConfigurationSettings.AppSettings["ManualGet"].ToLower() == "yes" ? true : false;
    }

    public enum InformationType
    { 
        HeartBeat = 98,
        LastPacketBeforeServerShutdown = 99
    }

    public enum xmlType
    {
        FBInfoChange,
        ActiveMatch,
        ActiveTournament,
        ActivePariMutuelWagering
    }


}