namespace JC_SoccerWeb.UserControl
{
    using JC_SoccerWeb;
    using JC_SoccerWeb.BLL; 
    using JC_SoccerWeb.DAL;
    using JC_SoccerWeb.Common;
    using System;
    using System.Collections;
    using System.Web.Security;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using SportsUtil;
    using System.Configuration;

    public class SignIn : UserControl
    {
        protected Button BtnSignin;
        protected Label Message;
        private string msg = "";
        protected TextBox Password;
        protected CheckBox RememberCheckbox;
        protected RequiredFieldValidator RequiredFieldValidator1;
        protected RequiredFieldValidator RequiredFieldValidator2;
        protected RequiredFieldValidator RequiredFieldValidator3;
        protected ResourceHelper res;
        protected TextBox txtValidate;
        protected TextBox UserID;

        int CustomAuthenticate(string username, string password)
        {
            //string[] arrUserInfo;
            //Authenticator au = new Authenticator(ConfigurationManager.AppSettings["SoccerDBConnectionString"]);
            //try
            //{
                //arrUserInfo = au.UserAuthenticate(username, password);
                //if (au.AccessCode > 0)
                //{
                    Session["user_id"] = "3";// arrUserInfo[0];
                    Session["user_role"] = "999";// arrUserInfo[1];
                    Session["user_name"] = username;
                    Session["user_sortType"] = "0";
                //}

            //    return au.AccessCode;
            //}
            //catch (Exception)
            //{
               return -99;
            //}
        }
            private void Btnsignin_Click(object sender, EventArgs e)
        {
            if (base.Request.Cookies["CheckCode"] == null)
            {
                this.Message.Text = "<br>Cookie Is Block！<br>";
                this.Message.Visible = true;
            }
            //////else if (string.Compare(base.Request.Cookies["CheckCode"].Value, this.txtValidate.Text, true) != 0)
            //////{
            //////    this.Message.Text = "<br>驗證碼錯誤！<br>";
            //////    this.Message.Visible = true;
            //////}
            else
            {
                string str = "";
                UserData data = new Users().GetUserByUSER_ID(this.UserID.Text.ToUpper(), this.Password.Text.Trim());
                if (data != null)
                {
                    CustomAuthenticate(this.UserID.Text.ToUpper(), "");
                    str = data.Tables[0].Rows[0]["USER_ID"].ToString().ToUpper();
                    Files.CicsWriteLog(DateTime.Now.ToString("HH:mm:ss") + " " + str + " login success.");
                    FormsAuthentication.SetAuthCookie(str.ToUpper(), this.RememberCheckbox.Checked);
                    FormsAuthentication.RedirectFromLoginPage(str.ToUpper(), false);
                    Hashtable hashtable = (Hashtable) base.Application["Online"];
                    if (hashtable != null)
                    {
                        for (int i = 0; i < hashtable.Count; i++)
                        {
                            IDictionaryEnumerator enumerator = hashtable.GetEnumerator();
                            string str2 = "";
                            while (enumerator.MoveNext())
                            {
                                if ((enumerator.Value != null) && enumerator.Value.ToString().Equals(str.ToUpper()))
                                {
                                    str2 = enumerator.Key.ToString();
                                    hashtable[str2] = "XXXXXX";
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        hashtable = new Hashtable();
                    }
                    hashtable[base.Session.SessionID] = str.ToUpper();
                    base.Application.Lock();
                    base.Application["Online"] = hashtable;
                    base.Application.UnLock();
                }
                else
                {
                    this.Message.Text = "<br>Login Failure！<br>";
                    Files.CicsWriteLog(DateTime.Now.ToString("HH:mm:ss") + " " + str + "login failure.");
                }
            }
        }

        private void InitializeComponent()
        {
            this.BtnSignin.Click += new EventHandler(this.Btnsignin_Click);
            base.Load += new EventHandler(this.Page_Load);
        }

        protected override void OnInit(EventArgs e)
        {
            this.InitializeComponent();
            base.OnInit(e);
        }

        private void Page_Load(object sender, EventArgs e)
        {
            this.res = new ResourceHelper(this.Page);
            base.Response.Cookies["userroles"].Value = "";
            base.Response.Cookies["userroles"].Path = "/";
            base.Response.Cookies["userroles"].Expires = new DateTime(0x76c, 10, 12);
            this.Context.User = null;
            this.Message.Text = this.Msg;
        }

        public string Msg
        {
            get
            {
                return this.msg;
            }
            set
            {
                this.msg = value;
            }
        }
    }
}

