using System;
using System.Configuration;
using System.Runtime.InteropServices;
using System.Net.NetworkInformation;
using System.Net;

namespace JC_SoccerWeb.Common
{
    internal struct AppFlag
    {
        //Application config
        
        internal static readonly string ScoutsDBConn = ConfigurationManager.AppSettings["ScoutsDBConn"]        ; 
        internal static readonly string HkjcDBConn = ConfigurationManager.AppSettings["HkjcDBConn"];

        //internal static readonly string SMS_MSG_RECEIVER_TABLE =
        //    ConfigurationManager.AppSettings["SMS_MSG_RECEIVER_TABLE"];

        //internal static readonly string SMS_MSG_TABLE = ConfigurationManager.AppSettings["SMS_MSG_TABLE"];
        //internal static readonly string WEBSITELABEL = ConfigurationManager.AppSettings["WEBSITELABEL"];

        internal static readonly int iPageSize = Convert.ToInt32(ConfigurationManager.AppSettings["PageSize"]);
        //internal static readonly string WinMsgID = ConfigurationManager.AppSettings["WinMsgID"];
        //internal static readonly string ServiceURL = ConfigurationManager.AppSettings["ServiceURL"];

        internal static string Error = ConfigurationManager.AppSettings["Error"];
        internal static string  ErrorFolder = ConfigurationManager.AppSettings["ErrorFolder"];
        internal static string EventFolder = ConfigurationManager.AppSettings["EventFolder"];
        internal static string  Log = ConfigurationManager.AppSettings["Log"];
        //internal static string CentaSmsUploadFolder = ConfigurationManager.AppSettings["CentaSmsUploadFolder"];
        internal static string CentaSmsAdmin = ConfigurationManager.AppSettings["CentaSmsAdmin"];
        internal static string CentaSmsExtra = ConfigurationManager.AppSettings["CentaSmsExtra"];
        internal static readonly int MarginOfDeviation = Convert.ToInt32(ConfigurationManager.AppSettings["MarginOfDeviation"]);
        internal static bool AutoMapping = ConfigurationManager.AppSettings["AutoMapping"].ToLower()=="yes"? true:false;
        internal static string EnetPlus = ConfigurationManager.AppSettings["EnetPlus"];
        internal static string ServiceURL = ConfigurationSettings.AppSettings["ServiceURL"];
        //internal static readonly int CentaSmsSendTime =
        //    Convert.ToInt32(ConfigurationManager.AppSettings["CentaSmsSendTime"]);

        //internal static readonly int UploadRefresh =
        //    Convert.ToInt32(ConfigurationManager.AppSettings["UploadRefresh"]) * 1000;

        //internal static readonly int SendRefresh =
        //    Convert.ToInt32(ConfigurationManager.AppSettings["SendRefresh"]) * 1000;

        //internal static readonly string RefreshMode = ConfigurationManager.AppSettings["RefreshMode"];
        //internal static readonly string ValidPrefixes = ConfigurationManager.AppSettings["ValidPrefixes"].Trim();

        //internal static readonly string DeteleGroupMode =
        //    ConfigurationManager.AppSettings["DeteleGroupMode"].ToUpper().Trim();

        //internal static readonly bool DefineMessage =
        //    (ConfigurationManager.AppSettings["DefineMessage"].ToUpper().Trim() == "TRUE") ? true : false;

        //internal static readonly string MmsUploadFolder = ConfigurationManager.AppSettings["MmsUploadFolder"];
        //internal static readonly string CheckDNCUrl = ConfigurationManager.AppSettings["CheckDNCUrl"];

        //internal static readonly int ImageMaximumSize =
        //    Convert.ToInt32(ConfigurationManager.AppSettings["ImageMaximumSize"]);

        //internal static readonly bool UploadFilter =
        //    (ConfigurationManager.AppSettings["UploadFilter"].ToUpper().Trim() == "TRUE") ? true : false;

        //internal static readonly string AppName = ConfigurationManager.AppSettings["AppName"];
        //internal static readonly string Password = ConfigurationManager.AppSettings["Password"];

        internal static string ApiIp()
        {
            string hostname = Dns.GetHostName();
            IPHostEntry localhost = Dns.GetHostEntry(hostname);
            IPAddress localaddr = localhost.AddressList[1];
            return localaddr.ToString();
        }
    }
}