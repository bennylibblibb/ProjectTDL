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
         public static string Client_id = ConfigurationSettings.AppSettings["Client_id"];
        public static string Secret_key = ConfigurationSettings.AppSettings["Secret_key"];
        public static string ScoutsUrl = ConfigurationSettings.AppSettings["ScoutsUrl"];
        public static int iPageSize =Convert .ToInt32(ConfigurationSettings.AppSettings["PageSize"]);
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