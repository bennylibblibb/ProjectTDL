namespace JC_SoccerWeb
{
   using System;
    using System.Collections;
    using System.ComponentModel;
using System.IO;
using System.Runtime.Remoting;
using System.Web;
    using JC_SoccerWeb.DAL;

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
            Hashtable hOnline = (Hashtable)Application["Online"];
            if (hOnline[Session.SessionID] != null)
            {
                hOnline.Remove(Session.SessionID);
                Application.Lock();
                Application["Online"] = hOnline;
                Application.UnLock();
            }
        }

        protected void Session_Start(object sender, EventArgs e)
        {
        }
    }
}

