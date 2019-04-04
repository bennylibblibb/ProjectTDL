using System;
using System.Configuration;
using System.Runtime.InteropServices;
using System.Net.NetworkInformation;
using System.Net;

namespace ScoutDBProvider
{
    internal struct AppFlag
    {
        internal static readonly string ScoutsDBConn = ConfigurationManager.AppSettings["ScoutsDBConn"];
        internal static readonly string MangoDBConn = ConfigurationManager.AppSettings["MangoDBConn"]; 
        internal static string GetTime = ConfigurationManager.AppSettings["GetTime"];
        internal static string SkSvrNotify = ConfigurationManager.AppSettings["SkSvrNotify"];
         
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