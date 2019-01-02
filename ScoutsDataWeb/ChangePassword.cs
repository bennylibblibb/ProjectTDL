namespace JC_SoccerWeb
{
    using Anthem;
    using JC_SoccerWeb.BLL;
    using JC_SoccerWeb.DAL;
    using JC_SoccerWeb.Common;
    using System;
    using System.Data;
    using System.Web.UI.WebControls;

    public class ChangePassword : System.Web.UI.Page
    {
        protected System.Web.UI.WebControls.CompareValidator cvPW;
        protected System.Web.UI.WebControls.Label lbConfirm;
        protected Anthem.Label lbMsg;
        protected System.Web.UI.WebControls.Label lbpw;
        protected System.Web.UI.WebControls.Label lbUser;
        protected Anthem.Panel plSendMsg;
        protected System.Web.UI.WebControls.RequiredFieldValidator rfvPW;
        protected System.Web.UI.WebControls.RequiredFieldValidator rfvPW2;
        protected Anthem.Button Save;
        protected System.Web.UI.WebControls.TextBox txtPW;
        protected System.Web.UI.WebControls.TextBox txtPW2;

        private void InitializeComponent()
        {
            this.Save.Click += new EventHandler(this.Save_Click);
            base.Load += new EventHandler(this.Page_Load);
        }

        protected override void OnInit(EventArgs e)
        {
            this.InitializeComponent();
            base.OnInit(e);
        }

        private void Page_Load(object sender, EventArgs e)
        {
            if (base.Request.IsAuthenticated && !this.Page.IsPostBack)
            {
                this.lbUser.Text = this.Context.User.Identity.Name;
            }
        }

        private void Save_Click(object sender, EventArgs e)
        {
            UserData userdata = new UserData();
            userdata = new Users().LoadUserByUSER_ID(this.Context.User.Identity.Name.Trim().ToUpper());
            DataRow row = userdata.Tables[0].Rows[0];
            string str = row["USER_PWD"].ToString();
            row["USER_PWD"] = this.txtPW.Text.Trim();
            row["CREATE_DATE"] = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            if (new Users().UpdateUser(userdata))
            {
                this.lbMsg.Text = "更新密碼成功!";
                Files.CicsWriteLog(DateTime.Now.ToString("HH:mm:ss ") + this.Context.User.Identity.Name.Trim().ToUpper() + " 更新密碼 " + str + " to " + this.txtPW.Text.Trim() + ".\r\n");
            }
            else
            {
                this.lbMsg.Text = "更新密碼失敗!";
            }
            this.lbMsg.UpdateAfterCallBack = true;
        }
    }
}

