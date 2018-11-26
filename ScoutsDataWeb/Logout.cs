namespace JC_SoccerWeb
{
    using System;
    using System.Web.Security;
    using System.Web.UI;
    using JC_SoccerWeb.Common;
    public class Logout : Page
    {
        private void InitializeComponent()
        {
            base.Load += new EventHandler(this.Page_Load);
        }

        protected override void OnInit(EventArgs e)
        {
            this.InitializeComponent();
            base.OnInit(e);
        }

        private void Page_Load(object sender, EventArgs e)
        {
            Files.CicsWriteLog(DateTime.Now.ToString("HH:mm:ss") + " " + this.Context.User.Identity.Name + " logout success.");
            FormsAuthentication.SignOut();
            base.Response.Cookies["userroles"].Value = "";
            base.Response.Cookies["userroles"].Path = "/";
            base.Response.Cookies["userroles"].Expires = new DateTime(0x76c, 10, 12);
            this.Context.User = null;
            this.Session.Clear();
            base.Response.Redirect("Default.aspx", false);
        }
    }
}

