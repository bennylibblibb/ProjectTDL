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
#pragma warning disable CS0618 // 'ConfigurationSettings.AppSettings' is obsolete: 'This method is obsolete, it has been replaced by System.Configuration!System.Configuration.ConfigurationManager.AppSettings'
        public static string ScoutsDBConn = ConfigurationSettings.AppSettings["ScoutsDBConn"];
#pragma warning restore CS0618 // 'ConfigurationSettings.AppSettings' is obsolete: 'This method is obsolete, it has been replaced by System.Configuration!System.Configuration.ConfigurationManager.AppSettings'
#pragma warning disable CS0618 // 'ConfigurationSettings.AppSettings' is obsolete: 'This method is obsolete, it has been replaced by System.Configuration!System.Configuration.ConfigurationManager.AppSettings'
        public static string HkjcDBConn = ConfigurationSettings.AppSettings["HkjcDBConn"];
#pragma warning restore CS0618 // 'ConfigurationSettings.AppSettings' is obsolete: 'This method is obsolete, it has been replaced by System.Configuration!System.Configuration.ConfigurationManager.AppSettings'
#pragma warning disable CS0618 // 'ConfigurationSettings.AppSettings' is obsolete: 'This method is obsolete, it has been replaced by System.Configuration!System.Configuration.ConfigurationManager.AppSettings'
        public static string Client_id = ConfigurationSettings.AppSettings["Client_id"];
#pragma warning restore CS0618 // 'ConfigurationSettings.AppSettings' is obsolete: 'This method is obsolete, it has been replaced by System.Configuration!System.Configuration.ConfigurationManager.AppSettings'
#pragma warning disable CS0618 // 'ConfigurationSettings.AppSettings' is obsolete: 'This method is obsolete, it has been replaced by System.Configuration!System.Configuration.ConfigurationManager.AppSettings'
        public static string Secret_key = ConfigurationSettings.AppSettings["Secret_key"];
#pragma warning restore CS0618 // 'ConfigurationSettings.AppSettings' is obsolete: 'This method is obsolete, it has been replaced by System.Configuration!System.Configuration.ConfigurationManager.AppSettings'
#pragma warning disable CS0618 // 'ConfigurationSettings.AppSettings' is obsolete: 'This method is obsolete, it has been replaced by System.Configuration!System.Configuration.ConfigurationManager.AppSettings'
        public static string ScoutsUrl = ConfigurationSettings.AppSettings["ScoutsUrl"];
#pragma warning restore CS0618 // 'ConfigurationSettings.AppSettings' is obsolete: 'This method is obsolete, it has been replaced by System.Configuration!System.Configuration.ConfigurationManager.AppSettings'
#pragma warning disable CS0618 // 'ConfigurationSettings.AppSettings' is obsolete: 'This method is obsolete, it has been replaced by System.Configuration!System.Configuration.ConfigurationManager.AppSettings'
        public static int iPageSize = Convert.ToInt32(ConfigurationSettings.AppSettings["PageSize"]);
#pragma warning restore CS0618 // 'ConfigurationSettings.AppSettings' is obsolete: 'This method is obsolete, it has been replaced by System.Configuration!System.Configuration.ConfigurationManager.AppSettings'
#pragma warning disable CS0618 // 'ConfigurationSettings.AppSettings' is obsolete: 'This method is obsolete, it has been replaced by System.Configuration!System.Configuration.ConfigurationManager.AppSettings'
        public static int iQueryDays = Convert.ToInt32(ConfigurationSettings.AppSettings["QueryDays"]);
#pragma warning restore CS0618 // 'ConfigurationSettings.AppSettings' is obsolete: 'This method is obsolete, it has been replaced by System.Configuration!System.Configuration.ConfigurationManager.AppSettings'
#pragma warning disable CS0618 // 'ConfigurationSettings.AppSettings' is obsolete: 'This method is obsolete, it has been replaced by System.Configuration!System.Configuration.ConfigurationManager.AppSettings'
        public static string SyncHkjcDateTime = ConfigurationSettings.AppSettings["SyncHkjcDateTime"];
#pragma warning restore CS0618 // 'ConfigurationSettings.AppSettings' is obsolete: 'This method is obsolete, it has been replaced by System.Configuration!System.Configuration.ConfigurationManager.AppSettings'
#pragma warning disable CS0618 // 'ConfigurationSettings.AppSettings' is obsolete: 'This method is obsolete, it has been replaced by System.Configuration!System.Configuration.ConfigurationManager.AppSettings'
        public static bool JsonType = ConfigurationSettings.AppSettings["JsonType"].ToLower() == "all" ? false : true;
#pragma warning restore CS0618 // 'ConfigurationSettings.AppSettings' is obsolete: 'This method is obsolete, it has been replaced by System.Configuration!System.Configuration.ConfigurationManager.AppSettings'
#pragma warning disable CS0618 // 'ConfigurationSettings.AppSettings' is obsolete: 'This method is obsolete, it has been replaced by System.Configuration!System.Configuration.ConfigurationManager.AppSettings'
        public static string GetTime = ConfigurationSettings.AppSettings["GetTime"];
#pragma warning restore CS0618 // 'ConfigurationSettings.AppSettings' is obsolete: 'This method is obsolete, it has been replaced by System.Configuration!System.Configuration.ConfigurationManager.AppSettings'
#pragma warning disable CS0618 // 'ConfigurationSettings.AppSettings' is obsolete: 'This method is obsolete, it has been replaced by System.Configuration!System.Configuration.ConfigurationManager.AppSettings'
        public static bool AutoBooked = ConfigurationSettings.AppSettings["AutoBooked"].ToLower() == "no" ? false : true;
#pragma warning restore CS0618 // 'ConfigurationSettings.AppSettings' is obsolete: 'This method is obsolete, it has been replaced by System.Configuration!System.Configuration.ConfigurationManager.AppSettings'
#pragma warning disable CS0618 // 'ConfigurationSettings.AppSettings' is obsolete: 'This method is obsolete, it has been replaced by System.Configuration!System.Configuration.ConfigurationManager.AppSettings'
        internal static readonly int MarginOfDeviation = Convert.ToInt32(ConfigurationSettings.AppSettings["MarginOfDeviation"]);
#pragma warning restore CS0618 // 'ConfigurationSettings.AppSettings' is obsolete: 'This method is obsolete, it has been replaced by System.Configuration!System.Configuration.ConfigurationManager.AppSettings'
#pragma warning disable CS0618 // 'ConfigurationSettings.AppSettings' is obsolete: 'This method is obsolete, it has been replaced by System.Configuration!System.Configuration.ConfigurationManager.AppSettings'
        internal static string lpString = ConfigurationSettings.AppSettings["lpString"];
#pragma warning restore CS0618 // 'ConfigurationSettings.AppSettings' is obsolete: 'This method is obsolete, it has been replaced by System.Configuration!System.Configuration.ConfigurationManager.AppSettings'
#pragma warning disable CS0618 // 'ConfigurationSettings.AppSettings' is obsolete: 'This method is obsolete, it has been replaced by System.Configuration!System.Configuration.ConfigurationManager.AppSettings'
        internal static readonly int LIVEGOALS = Convert.ToInt32(ConfigurationSettings.AppSettings["LIVEGOALS"]);
#pragma warning restore CS0618 // 'ConfigurationSettings.AppSettings' is obsolete: 'This method is obsolete, it has been replaced by System.Configuration!System.Configuration.ConfigurationManager.AppSettings'
#pragma warning disable CS0618 // 'ConfigurationSettings.AppSettings' is obsolete: 'This method is obsolete, it has been replaced by System.Configuration!System.Configuration.ConfigurationManager.AppSettings'
        internal static readonly int GOALDETAILS = Convert.ToInt32(ConfigurationSettings.AppSettings["GOALDETAILS"]);
#pragma warning restore CS0618 // 'ConfigurationSettings.AppSettings' is obsolete: 'This method is obsolete, it has been replaced by System.Configuration!System.Configuration.ConfigurationManager.AppSettings'
#pragma warning disable CS0618 // 'ConfigurationSettings.AppSettings' is obsolete: 'This method is obsolete, it has been replaced by System.Configuration!System.Configuration.ConfigurationManager.AppSettings'
        internal static readonly int IntervalSync = Convert.ToInt32(ConfigurationSettings.AppSettings["IntervalSync"]);
#pragma warning restore CS0618 // 'ConfigurationSettings.AppSettings' is obsolete: 'This method is obsolete, it has been replaced by System.Configuration!System.Configuration.ConfigurationManager.AppSettings'
#pragma warning disable CS0618 // 'ConfigurationSettings.AppSettings' is obsolete: 'This method is obsolete, it has been replaced by System.Configuration!System.Configuration.ConfigurationManager.AppSettings'
        public static string UnbookFiles = ConfigurationSettings.AppSettings["UnbookFiles"];
#pragma warning restore CS0618 // 'ConfigurationSettings.AppSettings' is obsolete: 'This method is obsolete, it has been replaced by System.Configuration!System.Configuration.ConfigurationManager.AppSettings'
#pragma warning disable CS0618 // 'ConfigurationSettings.AppSettings' is obsolete: 'This method is obsolete, it has been replaced by System.Configuration!System.Configuration.ConfigurationManager.AppSettings'
        internal static bool AutoMapping = ConfigurationSettings.AppSettings["AutoMapping"].ToLower() == "yes" ? true : false;
#pragma warning restore CS0618 // 'ConfigurationSettings.AppSettings' is obsolete: 'This method is obsolete, it has been replaced by System.Configuration!System.Configuration.ConfigurationManager.AppSettings'
#pragma warning disable CS0618 // 'ConfigurationSettings.AppSettings' is obsolete: 'This method is obsolete, it has been replaced by System.Configuration!System.Configuration.ConfigurationManager.AppSettings'
        internal static bool TestMode = ConfigurationSettings.AppSettings["TestMode"].ToLower() == "yes" ? true : false;
#pragma warning restore CS0618 // 'ConfigurationSettings.AppSettings' is obsolete: 'This method is obsolete, it has been replaced by System.Configuration!System.Configuration.ConfigurationManager.AppSettings'
#pragma warning disable CS0618 // 'ConfigurationSettings.AppSettings' is obsolete: 'This method is obsolete, it has been replaced by System.Configuration!System.Configuration.ConfigurationManager.AppSettings'
        internal static bool Important_for_trader = ConfigurationSettings.AppSettings["Important_for_trader"].ToLower() == "yes" ? true : false;
#pragma warning restore CS0618 // 'ConfigurationSettings.AppSettings' is obsolete: 'This method is obsolete, it has been replaced by System.Configuration!System.Configuration.ConfigurationManager.AppSettings'
#pragma warning disable CS0618 // 'ConfigurationSettings.AppSettings' is obsolete: 'This method is obsolete, it has been replaced by System.Configuration!System.Configuration.ConfigurationManager.AppSettings'
        internal static bool ManualGet = ConfigurationSettings.AppSettings["ManualGet"].ToLower() == "yes" ? true : false;
#pragma warning restore CS0618 // 'ConfigurationSettings.AppSettings' is obsolete: 'This method is obsolete, it has been replaced by System.Configuration!System.Configuration.ConfigurationManager.AppSettings'
#pragma warning disable CS0618 // 'ConfigurationSettings.AppSettings' is obsolete: 'This method is obsolete, it has been replaced by System.Configuration!System.Configuration.ConfigurationManager.AppSettings'
        /// public static int AutoBookTime = Convert.ToInt32(ConfigurationSettings.AppSettings["AutoBookTime"]);
        internal static readonly int IntervalGetPlayer = Convert.ToInt32(ConfigurationSettings.AppSettings["IntervalGetPlayer"]);
#pragma warning restore CS0618 // 'ConfigurationSettings.AppSettings' is obsolete: 'This method is obsolete, it has been replaced by System.Configuration!System.Configuration.ConfigurationManager.AppSettings'
#pragma warning disable CS0618 // 'ConfigurationSettings.AppSettings' is obsolete: 'This method is obsolete, it has been replaced by System.Configuration!System.Configuration.ConfigurationManager.AppSettings'
        internal static bool GetEventShow = ConfigurationSettings.AppSettings["GetEventShow"].ToLower() == "yes" ? true : false;
#pragma warning restore CS0618 // 'ConfigurationSettings.AppSettings' is obsolete: 'This method is obsolete, it has been replaced by System.Configuration!System.Configuration.ConfigurationManager.AppSettings'
        internal static string NotifyApplication = ConfigurationManager.AppSettings["NotifyApplication"];
        internal static string SkSvrNotify = ConfigurationManager.AppSettings["SkSvrNotify"];
        public static string AlertTime = ConfigurationSettings.AppSettings["AlertTime"];
        public static string AlertEmailFrom = ConfigurationSettings.AppSettings["AlertEmailFrom"];
        public static string AlertEmailPwd = ConfigurationSettings.AppSettings["AlertEmailPwd"];
        public static string AlertEmailTo = ConfigurationSettings.AppSettings["AlertEmailTo"];
        public static string AlertEmailSMTP = ConfigurationSettings.AppSettings["AlertEmailSMTP"];
         
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