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
        internal static string GetTime = ConfigurationManager.AppSettings["InitialTime"];
        internal static string SkSvrNotify = ConfigurationManager.AppSettings["SkSvrNotify"];
             internal static string SkSvrNotify2 = ConfigurationManager.AppSettings["SkSvrNotify2"];
        internal static ArrayList configSetting = (ArrayList)ConfigurationManager.GetSection("SyncItems2");
        internal static string SkSvrNodtify = ConfigurationManager.AppSettings["SkSvrNotify"];
        internal static bool Alert = ConfigurationManager.AppSettings["Alert"].ToLower() == "yes" ? true : false;
        internal static string DailyAnalysisTime = ConfigurationManager.AppSettings["DailyAnalysisTime"];
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