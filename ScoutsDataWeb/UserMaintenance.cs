namespace JC_SoccerWeb
{
    using Anthem;
    using JC_SoccerWeb.BLL;
    using JC_SoccerWeb.DAL;
    using JC_SoccerWeb.Common;
    using System;
    using System.Data;
    using System.Web.UI.WebControls;
using Button = Anthem .Button;
using DataGrid = Anthem.DataGrid;
using DropDownList = Anthem.DropDownList;
using Label = Anthem.Label;
using Panel = Anthem.Panel;
 using TextBox = Anthem.TextBox;
using System .Globalization ;
    public class UserMaintenance : CommonPage
    {
//        protected Button btnDelete;
//        protected Button btnNewUser;
//        protected Button btnNewUserU;
//        protected Button btnSearch;
//        protected Button btnSearchU;
//        private IFormatProvider culture = new CultureInfo("en-GB", true);
//        protected DataGrid dgData;
//        protected DropDownList dplRight;
//        protected DropDownList dplRightU;
//        protected DropDownList dplUsers;
//        protected DropDownList dplUsersD;
//        protected DropDownList dplUsersU;
//        protected Label Label1;
//        protected Label Label2;
//        protected Label Label3;
//        protected Label lbDepart;
//        protected Label lbEmail;
//        protected Label lbEmailU;
//        protected Label lbMsg;
//        protected Label lbMsgD;
//        protected Label lbMsgU;
//        protected Label lbPwd;
//        protected Label lbPwdU;
//        protected Label lbRight;
//        protected Label lbRightU;
//        protected Label lbUser;
//        protected Label lbUserID;
//        protected Label lbUserIDU;
//        protected Label lbUserName;
//        protected Label lbUserNameU;
//        protected Panel panel1;
//        protected Panel panel2;
//        protected Panel panel3;
//        protected Panel panel4;
//        protected RequiredFieldValidator Requiredfieldvalidator11;
//        protected RequiredFieldValidator Requiredfieldvalidator12;
//        protected RequiredFieldValidator Requiredfieldvalidator2;
//        protected RequiredFieldValidator RequiredFieldValidator3;
//        protected RequiredFieldValidator Requiredfieldvalidator4;
//        protected RequiredFieldValidator RequiredFieldValidator4;
//        protected RequiredFieldValidator Requiredfieldvalidator5;
//        protected RequiredFieldValidator RequiredFieldValidator5;
//        protected RequiredFieldValidator Requiredfieldvalidator7;
//        protected RequiredFieldValidator RequiredFieldValidator7;
//        protected RequiredFieldValidator Requiredfieldvalidator8;
//        protected RequiredFieldValidator rfv;
//        protected RequiredFieldValidator rfvNum;
//        protected RequiredFieldValidator rfvPwdU;
//        protected RegularExpressionValidator rvEmail;
//        protected RegularExpressionValidator rvEmailU;
//        protected TextBox txtDepart;
//        protected TextBox txtDepartU;
//        protected TextBox txtEmail;
//        protected TextBox txtEmailU;
//        protected TextBox txtEndDate;
//        protected TextBox txtPwd;
//        protected TextBox txtPwdU;
//        protected TextBox txtStartDate;
//        protected TextBox txtUserID;
//        protected TextBox txtUserIDU;
//        protected TextBox txtUserName;
//        protected TextBox txtUserNameU;
protected System.Web.UI.WebControls.Label lbUser;
		protected DropDownList dplUsers;
		protected System.Web.UI.WebControls.TextBox txtStartDate;
		protected System.Web.UI.WebControls.TextBox txtEndDate;
		protected Button btnSearch;
		protected Panel panel1;
		protected Panel panel2;
		protected Panel panel3;
		protected Panel panel4;
		protected DataGrid dgData;
		protected System.Web.UI.WebControls.RequiredFieldValidator rfvNum;
		protected System.Web.UI.WebControls.Label lbUserName;
		protected System.Web.UI.WebControls.Label lbRight;
		protected System.Web.UI.WebControls.Label lbUserID;
		protected System.Web.UI.WebControls.Label lbPwd;
		protected System.Web.UI.WebControls.Label lbEmail;
		protected System.Web.UI.WebControls.Label lbDepart;
		protected TextBox txtUserID1;
		protected TextBox txtPwd1;
		protected TextBox txtUserName1;
		protected TextBox txtEmail1;
		protected TextBox txtDepart1;
		protected System.Web.UI.WebControls.RequiredFieldValidator RequiredFieldValidator3;
		protected System.Web.UI.WebControls.RequiredFieldValidator RequiredFieldValidator4;
		protected System.Web.UI.WebControls.RequiredFieldValidator RequiredFieldValidator5; 
        protected System.Web.UI.WebControls.RegularExpressionValidator rvEmail;
		protected Button btnNewUser;
		protected Label lbMsg;
		protected System.Web.UI.WebControls.Label lbUserIDU;
		protected TextBox txtUserIDU;
		protected System.Web.UI.WebControls.Label lbUserNameU;
		protected System.Web.UI.WebControls.RequiredFieldValidator Requiredfieldvalidator8;
		protected TextBox txtUserNameU;
		protected System.Web.UI.WebControls.Label lbPwdU;
		protected TextBox txtPwdU;
		protected System.Web.UI.WebControls.Label lbRightU;
		protected System.Web.UI.WebControls.Label lbEmailU;
        protected System.Web.UI.WebControls.RegularExpressionValidator rvEmailU;
		protected TextBox txtEmailU;
		protected System.Web.UI.WebControls.Label Label1;
		protected TextBox txtDepartU;
		protected Button btnNewUserU;
		protected Label lbMsgU;
		protected DropDownList dplUsersU;
		protected Button btnSearchU;
		protected System.Web.UI.WebControls.RequiredFieldValidator Requiredfieldvalidator12;
		protected System.Web.UI.WebControls.RequiredFieldValidator Requiredfieldvalidator11;
		protected Button btnDelete;
		protected DropDownList dplUsersD;
		protected Label lbMsgD;
		protected DropDownList dplRight1;
		protected DropDownList dplRightU;
        protected System.Web.UI.WebControls.RequiredFieldValidator Requiredfieldvalidator2;
        protected System.Web.UI.WebControls.RequiredFieldValidator Requiredfieldvalidator5;
        protected System.Web.UI.WebControls.RequiredFieldValidator Requiredfieldvalidator4;
        protected System .Web .UI.WebControls . RequiredFieldValidator Requiredfieldvalidator7;
		protected System.Web.UI.WebControls.Label Label2;
		protected System.Web.UI.WebControls.Label Label3; 
		protected System.Web.UI.WebControls.RequiredFieldValidator rfv;
		protected System.Web.UI.WebControls.RequiredFieldValidator rfvPwdU;
		private IFormatProvider culture = new CultureInfo("en-GB", true);
		
        private void btnDelete_Click(object sender, EventArgs e)
        {
            Users users = new Users();
            UserData userdata = new UserData();
            userdata = users.LoadUserByUSER_ID(this.dplUsersD.SelectedItem.Text.Trim());
            userdata.Tables[0].Rows[0].Delete();
            if (users.DeleteUser(userdata))
            {
                this.lbMsgD.Text = "刪除成功！";
                Files.CicsWriteLog(DateTime.Now.ToString("HH:mm:ss") + "  (" + this.Context.User.Identity.Name + ")  delete user  " + this.dplUsersD.SelectedItem.Text.Trim() + ".\r\n");
            }
            else
            {
                this.lbMsgD.Text = "刪除失敗！";
            }
            this.GetdplUserData(this.dplUsersD);
            this.dplUsersD.SelectedIndex = this.dplUsersD.Items.Count - 1;
            this.dplUsersD.UpdateAfterCallBack = true;
            this.lbMsgD.UpdateAfterCallBack = true;
        }

        private void btnNewUser_Click(object sender, EventArgs e)
        {
            UserData userdata = new UserData();
            DataRow row = userdata.Tables["USER_PROFILE"].NewRow();
            row["USER_ID"] = this.txtUserID1.Text.Trim().ToUpper();
            row["USER_NAME"] = this.txtUserName1.Text.Trim();
            row["USER_PWD"] = this.txtPwd1.Text.Trim();
            row["USER_RIGHT"] = "0";
            row["SHAREDGP_RIGHT"] = this.dplRight1.SelectedItem.Value;
            row["USER_EMAIL"] = this.txtEmail1.Text.Trim();
            row["DEPARTMENT"] = this.txtDepart1.Text.Trim();
            row["CREATE_DATE"] = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            userdata.Tables["USER_PROFILE"].Rows.Add(row);
            if (new Users().InsertUser(userdata))
            {
                this.lbMsg.Text = "新增用戶成功!";
                Files.CicsWriteLog(DateTime.Now.ToString("HH:mm:ss") + "  (" + this.Context.User.Identity.Name + ")  add user,USER_ID: " + this.txtUserID1.Text.Trim().ToUpper() + ",USER_NAME:" + this.txtUserName1.Text + ",PWD:" + this.txtPwd1.Text + ",SHAREDGP_RIGHT:" + this.dplRight1.SelectedItem.Text + ",USER_EMAIL:" + this.txtEmail1.Text + ",DEPARTMENT:" + this.txtDepart1.Text + ".\r\n");
                Manager.AddScriptForClientSideEval("Focuss( 'txtUserID1','All');");
            }
            else
            {
                this.lbMsg.Text = "新增用戶失敗，用戶ＩＤ已存在!";
            }
            this.lbMsg.UpdateAfterCallBack = true;
            Manager.AddScriptForClientSideEval("Focuss( 'txtUserID1','');");
        }

        private void btnNewUserU_Click(object sender, EventArgs e)
        {
            UserData userdata = new UserData();
            userdata = new Users().LoadUserByUSER_ID(this.txtUserIDU.Text.Trim());
            DataRow row = userdata.Tables[0].Rows[0];
            string str = "";
            if (row["SHAREDGP_RIGHT"].Equals("N"))
            {
                str = "無";
            }
            else
            {
                str = "有";
            }
            Files.CicsWriteLog(string.Concat(new object[] { DateTime.Now.ToString("HH:mm:ss"), "  (", this.Context.User.Identity.Name, ")  update user,USER_ID: ", row["USER_ID"], ",USER_NAME:", row["USER_NAME"], ",PWD:", row["USER_PWD"], ",SHAREDGP_RIGHT:", str, ",USER_EMAIL:", row["USER_EMAIL"], ",DEPARTMENT:", row["DEPARTMENT"] }));
            row["USER_ID"] = this.txtUserIDU.Text.Trim();
            row["USER_NAME"] = this.txtUserNameU.Text.Trim();
            row["USER_PWD"] = this.txtPwdU.Text.Trim();
            row["SHAREDGP_RIGHT"] = this.dplRightU.SelectedItem.Value;
            row["USER_EMAIL"] = this.txtEmailU.Text.Trim();
            row["DEPARTMENT"] = this.txtDepartU.Text.Trim();
            row["CREATE_DATE"] = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            if (new Users().UpdateUser(userdata))
            {
                this.lbMsgU.Text = "更新用戶成功!";
                Files.CicsWriteLog("to USER_ID: " + this.txtUserIDU.Text.Trim().ToUpper() + ",USER_NAME:" + this.txtUserNameU.Text + ",PWD:" + this.txtPwd1.Text + ",SHAREDGP_RIGHT:" + this.dplRightU.SelectedItem.Text + ",USER_EMAIL:" + this.txtEmailU.Text + ",DEPARTMENT:" + this.txtDepartU.Text + ".\r\n");
            }
            else
            {
                this.lbMsgU.Text = "更新用戶失敗!";
            }
            this.lbMsgU.UpdateAfterCallBack = true;
            this.rfvPwdU.Enabled = false;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string filterExpression = "";
            string str2 = this.dplUsers.SelectedItem.Text.Trim();
            string str3 = this.txtStartDate.Text.Trim();
            string str4 = this.txtEndDate.Text.Trim();
            UserData userDataBy = new Users().GetUserDataBy("");
            if (str2 != "All")
            {
                filterExpression = filterExpression + "and  USER_ID ='" + str2 + "' ";
            }
            if (str3 != "")
            {
                filterExpression = string.Concat(new object[] { filterExpression, "and   create_date >='", Convert.ToDateTime(str3 + " 00:00:01", this.culture), "' " });
            }
            if (str4 != "")
            {
                filterExpression = string.Concat(new object[] { filterExpression, "and  create_date <='", Convert.ToDateTime(str4 + " 23:59:59", this.culture), "' " });
            }
            if (filterExpression == "")
            {
                this.dgData.DataSource = userDataBy.Tables["USER_PROFILE"].DefaultView;
                this.Session["userdata"] = userDataBy;
            }
            else
            {
                filterExpression = filterExpression.Substring(4, filterExpression.Length - 4);
                try
                {
                    DataRow[] rowArray = userDataBy.Tables["USER_PROFILE"].Select(filterExpression, "CREATE_DATE DESC");
                    DataTable table = userDataBy.Tables["USER_PROFILE"].Copy();
                    table.Rows.Clear();
                    UserData data2 = new UserData();
                    foreach (DataRow row in rowArray)
                    {
                        object[] itemArray = row.ItemArray;
                        table.Rows.Add(itemArray);
                        data2.Tables["USER_PROFILE"].Rows.Add(itemArray);
                    }
                    this.dgData.DataSource = table.DefaultView;
                    this.Session["userdata"] = data2;
                }
                catch (Exception exception)
                {
                    string str5 = exception.ToString();
                }
            }
            this.dgData.CurrentPageIndex = 0;
            this.dgData.PageSize = AppFlag.iPageSize;
            this.dgData.DataBind();
            this.dgData.UpdateAfterCallBack = true;
        }

        private void btnSearchU_Click(object sender, EventArgs e)
        {
            DataRow row = new Users().LoadUserByUSER_ID(this.dplUsersU.SelectedItem.Text.Trim()).Tables["USER_PROFILE"].Rows[0];
            this.txtUserIDU.Text = row["USER_ID"].ToString();
            this.txtUserNameU.Text = row["USER_NAME"].ToString();
            ListItem item = this.dplRightU.Items.FindByValue(row["SHAREDGP_RIGHT"].ToString());
            this.dplRightU.ClearSelection();
            item.Selected = true;
            this.txtEmailU.Text = row["USER_EMAIL"].ToString();
            this.txtDepartU.Text = row["DEPARTMENT"].ToString();
            this.txtUserIDU.UpdateAfterCallBack = true;
            this.txtUserNameU.UpdateAfterCallBack = true;
            this.txtPwdU.Text = row["USER_PWD"].ToString();
            this.txtPwdU.UpdateAfterCallBack = true;
            this.dplRightU.UpdateAfterCallBack = true;
            this.txtEmailU.UpdateAfterCallBack = true;
            this.txtDepartU.UpdateAfterCallBack = true;
            this.Requiredfieldvalidator8.Enabled = true;
            this.Requiredfieldvalidator11.Enabled = true;
            this.Requiredfieldvalidator12.Enabled = true;
            this.lbMsgU.Text = "";
            this.lbMsgU.UpdateAfterCallBack = true;
        }

        private void dgData_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
        {
            this.dgData.CurrentPageIndex = e.NewPageIndex;
            this.GetAllUsers();
        }

        private void dplUsers_SelectedIndexChanged(object sender, EventArgs e)
        {
            UserData userDataBy;
            string str = this.dplUsers.SelectedItem.Text.Trim();
            if (str == "All")
            {
                userDataBy = new Users().GetUserDataBy("");
            }
            else
            {
                userDataBy = new Users().GetUserDataBy("where User_ID='" + str + "'");
            }
            this.dgData.DataSource = userDataBy.Tables["USER_PROFILE"].DefaultView;
            this.dgData.PageSize = AppFlag.iPageSize;
            this.dgData.DataBind();
            this.dgData.UpdateAfterCallBack = true;
        }

        private void dplUsersU_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.LoadUserData(this.dplUsersU.SelectedItem.Text.Trim());
        }

        private void GetAllUsers()
        {
            UserData userDataBy = new Users().GetUserDataBy("");
            this.dgData.DataSource = userDataBy.Tables["USER_PROFILE"].DefaultView;
            this.dgData.PageSize = AppFlag.iPageSize;
            this.dgData.DataBind();
            this.dplUsers.DataSource = userDataBy.Tables["USER_PROFILE"].DefaultView;
            this.dplUsers.DataTextField = "USER_ID";
            this.dplUsers.DataBind();
            this.dplUsers.Items.Insert(0, "All");
            this.Session["userdata"] = userDataBy;
        }

        private void GetdplUserData(DropDownList dplname)
        {
            UserData userDataBy = new Users().GetUserDataBy("");
            dplname.DataSource = userDataBy.Tables[0].DefaultView;
            dplname.DataTextField = "USER_ID";
            dplname.DataBind();
            dplname.SelectedIndex = 0;
        }

        private void InitializeComponent()
        {
            this.dplUsers.SelectedIndexChanged += new EventHandler(this.dplUsers_SelectedIndexChanged);
            this.btnSearch.Click += new EventHandler(this.btnSearch_Click);
            this.dgData.PageIndexChanged += new DataGridPageChangedEventHandler(this.dgData_PageIndexChanged);
            this.btnNewUser.Click += new EventHandler(this.btnNewUser_Click);
            this.dplUsersU.SelectedIndexChanged += new EventHandler(this.dplUsersU_SelectedIndexChanged);
            this.btnSearchU.Click += new EventHandler(this.btnSearchU_Click);
            this.btnNewUserU.Click += new EventHandler(this.btnNewUserU_Click);
            this.btnDelete.Click += new EventHandler(this.btnDelete_Click);
            base.Load += new EventHandler(this.Page_Load);
        }

        private void LoadUserData(string userid)
        {
            DataRow row = new Users().LoadUserByUSER_ID(userid).Tables["USER_PROFILE"].Rows[0];
            this.txtUserIDU.Text = row["USER_ID"].ToString();
            this.txtUserNameU.Text = row["USER_NAME"].ToString();
            ListItem item = this.dplRightU.Items.FindByValue(row["SHAREDGP_RIGHT"].ToString());
            this.dplRightU.ClearSelection();
            item.Selected = true;
            this.txtEmailU.Text = row["USER_EMAIL"].ToString();
            this.txtDepartU.Text = row["DEPARTMENT"].ToString();
            this.txtPwdU.Text = row["USER_PWD"].ToString();
            this.txtUserIDU.UpdateAfterCallBack = true;
            this.txtUserNameU.UpdateAfterCallBack = true;
            this.txtPwdU.UpdateAfterCallBack = true;
            this.dplRightU.UpdateAfterCallBack = true;
            this.txtEmailU.UpdateAfterCallBack = true;
            this.txtDepartU.UpdateAfterCallBack = true;
            this.lbMsgU.Text = "";
            this.lbMsgU.UpdateAfterCallBack = true;
        }

        protected override void OnInit(EventArgs e)
        {
            this.InitializeComponent();
            base.OnInit(e);
        }

        private void Page_Load(object sender, EventArgs e)
        {
            if (base.Request.IsAuthenticated)
            {
                if (!this.Page.IsPostBack)
                {
                    this.lbUser.Text = this.Context.User.Identity.Name;
                    string str = (base.Request["sIndex"] == null) ? "0" : base.Request["sIndex"];
                    switch (str)
                    {
                        case "0":
                            this.txtEndDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
                            this.txtStartDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
                            this.panel1.Visible = true;
                            this.panel2.Visible = false;
                            this.panel3.Visible = false;
                            this.panel4.Visible = false;
                            this.GetAllUsers();
                            return;

                        case "1":
                            this.panel1.Visible = false;
                            this.panel2.Visible = true;
                            this.panel3.Visible = false;
                            this.panel4.Visible = false;
                            return;

                        case "2":
                            this.panel1.Visible = false;
                            this.panel2.Visible = false;
                            this.panel3.Visible = true;
                            this.panel4.Visible = false;
                            this.GetdplUserData(this.dplUsersU);
                            this.LoadUserData(this.dplUsersU.SelectedItem.Text.Trim());
                            return;
                    }
                    if (str == "3")
                    {
                        ((Button) this.FindControl("btnDelete")).Attributes.Add("onclick", "return confirm('Are you sure?');");
                        this.panel1.Visible = false;
                        this.panel2.Visible = false;
                        this.panel3.Visible = false;
                        this.panel4.Visible = true;
                        this.GetdplUserData(this.dplUsersD);
                    }
                }
            }
            else
            {
                base.Response.Redirect("Default.aspx", false);
            }
        }
    }
}

