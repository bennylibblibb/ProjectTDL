namespace JC_SoccerWeb
{
    using JC_SoccerWeb.UserControl;
    using System;
    using System.Web.Security;
    using System.Web.UI;
    using JC_SoccerWeb.Common;

    public class _Default : Page
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
            if ((base.Request["redtype"] != "unonly") && (base.Request["redtype"] != "again"))
            {
                if (base.Request.IsAuthenticated)
                {
                    base.Response.Redirect("Update.aspx");
                }
            }
            else
            {
                SignIn controlSignIn = this.FindControl("SignIn1") as SignIn;
                if (base.Request["redtype"] == "again")
                {
                    controlSignIn.Msg = "<br>" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "  Time out ,please re-login.<br>";
                    Files.CicsWriteLog(DateTime.Now.ToString("HH:mm:ss") + " (" + this.Context.User.Identity.Name + ")   Time out ,please re-login.");
                }
                else
                {
                    controlSignIn.Msg = "<br>" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "  This login name is already in use by a different user,please re-login.<br>";
                    Files.CicsWriteLog(DateTime.Now.ToString("HH:mm:ss") + "  ( " + this.Context.User.Identity.Name + ")   This login name is already in use by a different user,please re-login.");
                }
                FormsAuthentication.SignOut();
                base.Response.Cookies["userroles"].Value = "";
                base.Response.Cookies["userroles"].Path = "/";
                base.Response.Cookies["userroles"].Expires = new DateTime(0x76c, 10, 12);
                this.Session.Clear();
                this.Context.User = null;
            }
        }
    }
}

