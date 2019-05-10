namespace JC_SoccerWeb
{
   using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Configuration;
    using System.IO;
using System.Runtime.Remoting;
using System.Web;
    using JC_SoccerWeb.DAL;
    using SportsItemHandler; 


    public class Global : HttpApplication
    {
        private IContainer components = null;

        public Global()
        {
            this.InitializeComponent();
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
        }

        protected void Application_End(object sender, EventArgs e)
        {
        }

        protected void Application_Error(object sender, EventArgs e)
        {
        }

        protected void Application_Start(object sender, EventArgs e)
        {
            int iIndex = 0;
            string[] weatherItems;
            string[] fieldItems;
            string[] msgType;
            string[] RemotingItemsArray;
            string[]   positionItems; ;
            string[] QueueItemsArray; 
            string[] NotifyMessageArray;
            string[] NotifyUpdateTypeArray;

            ArrayList configSetting = new ArrayList();

            //Load positionItems Tag
            configSetting = (ArrayList)ConfigurationSettings.GetConfig("positionItems");
            positionItems = new string[configSetting.Count];
            if (configSetting != null)
            {
                iIndex = 0;
                foreach (string s in configSetting)
                {
                    positionItems[iIndex] = s;
                    iIndex++;
                }
            }
            Application["positionItemsArray"] = positionItems;
            configSetting.Clear();

            //Load weatherItems Tag
            configSetting = (ArrayList)ConfigurationManager.GetSection("weatherItems");
            weatherItems = new string[configSetting.Count];
            if (configSetting != null)
            {
                iIndex = 0;
                foreach (string s in configSetting)
                {
                    weatherItems[iIndex] = s;
                    iIndex++;
                }
            }
            Application["weatherItemsArray"] = weatherItems;
            configSetting.Clear();

            //Load fieldItems Tag
            configSetting = (ArrayList)ConfigurationSettings.GetConfig("fieldItems");
            fieldItems = new string[configSetting.Count];
            if (configSetting != null)
            {
                iIndex = 0;
                foreach (string s in configSetting)
                {
                    fieldItems[iIndex] = s;
                    iIndex++;
                }
            }
            Application["fieldItemsArray"] = fieldItems;
            configSetting.Clear();

            //Load messageType Tag
            configSetting = (ArrayList)ConfigurationSettings.GetConfig("messageType");
            msgType = new string[configSetting.Count];
            if (configSetting != null)
            {
                iIndex = 0;
                foreach (string s in configSetting)
                {
                    msgType[iIndex] = s;
                    iIndex++;
                }
            }
            Application["messageType"] = msgType;
            configSetting.Clear();


            //Load RemotingInfo Tag
            configSetting = (ArrayList)ConfigurationSettings.GetConfig("RemotingInfo");
            RemotingItemsArray = new string[configSetting.Count];
            if (configSetting != null)
            {
                iIndex = 0;
                foreach (string s in configSetting)
                {
                    RemotingItemsArray[iIndex] = s;
                    iIndex++;
                }
            }
            Application["RemotingItems"] = RemotingItemsArray;
            configSetting.Clear();

            //Load QueueInfo Tag
            configSetting = (ArrayList)ConfigurationSettings.GetConfig("QueueInfo");
            QueueItemsArray = new string[configSetting.Count];
            if (configSetting != null)
            {
                iIndex = 0;
                foreach (string s in configSetting)
                {
                    QueueItemsArray[iIndex] = s;
                    iIndex++;
                }
            }
            Application["QueueItems"] = QueueItemsArray;
            configSetting.Clear();


            //Load NotifyMessageType Tag
            configSetting = (ArrayList)ConfigurationSettings.GetConfig("NotifyMessageType");
            NotifyMessageArray = new string[configSetting.Count];
            if (configSetting != null)
            {
                iIndex = 0;
                foreach (string s in configSetting)
                {
                    NotifyMessageArray[iIndex] = s;
                    iIndex++;
                }
            }
            Application["NotifyMessageTypes"] = NotifyMessageArray;
            configSetting.Clear();

            configSetting = (ArrayList)ConfigurationSettings.GetConfig("NotifyUpdateType");
            NotifyUpdateTypeArray = new string[configSetting.Count];
            if (configSetting != null)
            {
                iIndex = 0;
                foreach (string s in configSetting)
                {
                    NotifyUpdateTypeArray[iIndex] = s;
                    iIndex++;
                }
            }
            Application["NotifyUpdateType"] = NotifyUpdateTypeArray;
            configSetting.Clear();

            //string str = base.Context.Server.MapPath(base.Context.Request.ApplicationPath);
            //string path = Path.Combine(str, "remotingclient.cfg");
            //if (File.Exists(path))
            //{
            //    RemotingConfiguration.Configure(path, false);
            //}
            //ConfigManager.OnApplicationStart(str);
        }

        public static string GetApplicationPath(HttpRequest request)
        {
            string applicationPath = string.Empty;
            try
            {
                if (request.ApplicationPath != "/")
                {
                    applicationPath = request.ApplicationPath;
                }
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return applicationPath;
        }

        private void InitializeComponent()
        {
            this.components = new Container();
        }

        protected void Session_End(object sender, EventArgs e)
        {
            Application["weatherItemsArray"] = null;
            Application["fieldItemsArray"] = null;
              Application["RemotingItems"] = null;
            Application["positionItemsArray"] = null;
            Application["messageType"] = null;
            Application["QueueItems"] = null; 
            Application["NotifyMessageTypes"] = null;



            Hashtable hOnline = (Hashtable)Application["OnlineScouts"];
            if (hOnline!=null&&hOnline[Session.SessionID] != null)
            {
                hOnline.Remove(Session.SessionID);
                Application.Lock();
                Application["OnlineScouts"] = hOnline;
                Application.UnLock();
            }

        }

        protected void Session_Start(object sender, EventArgs e)
        {
        }
    }
}

