using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Text;
using RemoteService.Win32;
using FirebirdSql.Data.FirebirdClient;


namespace JC_SoccerWeb.Common
{
	/// <summary>
	/// ConfigManager ªººK­n´y­z¡C
	/// </summary>
	internal class ConfigManager : MarshalByRefObject
    {
        internal static string  SendWinMsg(string eid,string type)
        {
            try
            {
                Win32Message message = (Win32Message)Activator.GetObject(typeof(Win32Message), AppFlag.ServiceURL);
                if (message != null)
                {
                    //string [] arrFields = (string[])HttpContext.Current.Application["NotifyUpdateTypeArray"];
                    //string sType= arrFields.Where(s => s.ToLower().Contains(type)).ToString();
                    //string id = sType.Substring(sType.IndexOf("-") + 1, sType.Length - sType.IndexOf("-"));
                    // string id = type.Substring(type.IndexOf("-") + 1, type.Length - type.IndexOf("-") - 1);
                    if (type == "GotEvent")
                    {
                        message.Broadcast("D" + eid + "-" + type);
                    }
                    else
                    {
                        message.Broadcast("S" + eid + "-" + type);
                    }
                    Files.CicsWriteLog(DateTime.Now.ToString("HH:mm:ss") + " Sent " + eid + " " + type);

                    return "Done";
                }
                return "PLEASE CHECK REMOTING HOST!";
            }
            catch (Exception exception)
            {
                Files.CicsWriteLog(DateTime.Now.ToString("HH:mm:ss ") + "[Failure] Send " + eid + " " + type + " ,PLEASE CHECK REMOTING HOST!");

                Files.CicsWriteError(DateTime.Now.ToString("HH:mm:ss") + " SendWinMsg(),error: " + exception);
                // this.RegisterStartupScript("key", " <script language='javascript'> alert('PLEASE CHECK REMOTING HOST!'); </script> ");
                return "PLEASE CHECK REMOTING HOST!";
            }
        } 
    }
}