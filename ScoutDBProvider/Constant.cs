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
        internal static string GetTime = ConfigurationManager.AppSettings["GetTime"];
        internal static string SkSvrNotify = ConfigurationManager.AppSettings["SkSvrNotify"];
        internal static ArrayList configSetting = (ArrayList)ConfigurationManager.GetSection("SyncItems2");
        internal static string SkSvrNodtify = ConfigurationManager.AppSettings["SkSvrNotify"];
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