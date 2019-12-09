using System;
using System.Configuration;
using System.Runtime.InteropServices;
using System.Net.NetworkInformation;
using System.Net;
using System.Collections;

namespace ScoutDBProvider
{
    internal struct AppFlag
    {
        internal static readonly string ScoutsDBConn = ConfigurationManager.AppSettings["ScoutsDBConn"];
        internal static readonly string MangoDBConn = ConfigurationManager.AppSettings["MangoDBConn"]; 
        
        internal static string SkSvrNotify = ConfigurationManager.AppSettings["SkSvrNotify"];
        internal static string SkSvrNotify2 = ConfigurationManager.AppSettings["SkSvrNotify2"];
        internal static ArrayList AnalysisItems = (ArrayList)ConfigurationManager.GetSection("AnalysisItems");
        internal static ArrayList configSetting = (ArrayList)ConfigurationManager.GetSection("SyncItems");
      //  internal static string SkSvrNodtify = ConfigurationManager.AppSettings["SkSvrNotify"];
        internal static bool Alert = ConfigurationManager.AppSettings["Alert"].ToLower() == "yes" ? true : false;
        internal static string LivePeriod = ConfigurationManager.AppSettings["LivePeriod"];

        public static string AlertEmailFrom = ConfigurationManager.AppSettings["AlertEmailFrom"];
        public static string AlertEmailPwd = ConfigurationManager.AppSettings["AlertEmailPwd"];
        public static string AlertEmailTo = ConfigurationManager.AppSettings["AlertEmailTo"];
        public static string AlertEmailSMTP = ConfigurationManager.AppSettings["AlertEmailSMTP"];
        internal static string GetTime = ConfigurationManager.AppSettings["InitialTime"]; 
        internal static string InitialTime2 = ConfigurationManager.AppSettings["InitialTime2"];
        internal static string DailyAnalysisTime = ConfigurationManager.AppSettings["DailyAnalysisTime"];
        internal static bool TestToLive = ConfigurationManager.AppSettings["TestToLive"].ToLower() == "yes" ? true : false;
        internal static readonly string ToLiveDB = ConfigurationManager.AppSettings["ToLiveDB"];
        internal static readonly int InitMatchInterval =Convert.ToInt32(ConfigurationManager.AppSettings["InitMatchInterval"]);
        
    }

    public enum MessageID
    {
        LIVEGOALS = 30,
        GOALDETAILS = 31,
        ANALYSISOTHER = 62,
        FIXTURES = 63,
        HKGOAL = 60,
        HKGOALDETAILS = 61,
        RANKS = 15,
        SCORERS = 17,
        ANALYSISRECENTS=13,
        ANALYSISHISTORYS=11,
        ANALYSISTATS=14,
        ANALYSISPLAYERLIST=12
    }


    public enum xmlType
    {
        FBInfoChange,
        ActiveMatch,
        ActiveTournament,
        ActivePariMutuelWagering
    }


}