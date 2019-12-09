namespace JC_SoccerWeb.UserControl
{
    using System.Globalization;
    using JC_SoccerWeb.BLL;
    using System;
    using System.Collections;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Drawing; 

    public class MenuTabs : UserControl
    {
        protected Anthem.DataList tabs;

        private void InitializeComponent()
        {
            this.tabs.SelectedIndexChanged += new EventHandler(this.Tabs_SelectedIndexChanged);
            // this.tabs.ItemCommand += new DataListCommandEventHandler( this.tabs_ItemCommand);
            // this.tabs.ItemCreated += new DataListItemEventHandler(this.tabs_ItemCreated);
            this.tabs.ItemDataBound += new DataListItemEventHandler(this.tabs_ItemDataBound);
            this.tabs.Load += new EventHandler(this.Page_Load);
            //base.Load += new EventHandler(this.Page_Load);
        }


        protected void Tabs_SelectedIndexChanged(object sender, EventArgs e)
        {
            var ddlList = (Anthem.DataList)sender;
        }

        protected override void OnInit(EventArgs e)
        {
            this.InitializeComponent();
            base.OnInit(e);
        }

        private void Page_Load(object sender, EventArgs e)
        {
            //if (!IsPostBack)
            //{ 
            string name = this.Context.User.Identity.Name;
            ArrayList list = new ArrayList();
            list.Add(new TabItem("STATSCORE", Global.GetApplicationPath(base.Request) + "/SyncMatches.aspx?csIndex=" + list.Count));
            list.Add(new TabItem("HKJC", Global.GetApplicationPath(base.Request) + "/Matches.aspx?csIndex=" + list.Count));
            //   if (list.Count==2&&(base.Request["csIndex"] !=null&&base.Request["csIndex"] != "0") ||(list.Count == 2&& base.Request["csIndex"] != "8" && base.Request["csIndex"] != "9"))
            if (list.Count == 2 && (base.Request["csIndex"] == null || base.Request["csIndex"] == "0"|| base.Request["csIndex"] == "2"))
            { 
               list.Add(new TabItem("賽程", Global.GetApplicationPath(base.Request) + "/Fixtures.aspx?csIndex=" + list.Count));
                list.Add(new TabItem("球隊排名", Global.GetApplicationPath(base.Request) + "/sports/Rank.aspx?csIndex=" + list.Count));
                list.Add(new TabItem("射手榜", Global.GetApplicationPath(base.Request) + "/sports/Scorers.aspx?csIndex=" + list.Count ));
                list.Add(new TabItem("其它資訊", Global.GetApplicationPath(base.Request) + "/sports/SportNews.aspx?csIndex=" + list.Count));
                list.Add(new TabItem("EnetPlus", "http://" + Common.AppFlag.EnetPlus));
            }
            else if (list.Count == 2 && (base.Request["csIndex"] != null && base.Request["csIndex"] != "0" && base.Request["csIndex"] != "6" && base.Request["csIndex"] != "7"))
            {
                string m_ID = Session["eventID"] == null ?"":Session["eventID"].ToString().Substring(0, Session["eventID"].ToString().IndexOf("/"));
                string m_Code = Session["eventID"] == null ? "" : Session["eventID"].ToString().Substring( Session["eventID"].ToString().IndexOf("/")+1, Session["eventID"].ToString().Trim().Length-m_ID.Length-1);

                list.Add(new TabItem("分析", Global.GetApplicationPath(base.Request) + "/sports/AnalysisModify.aspx?csIndex=" + list.Count + "&eventid=" + m_ID));
                list.Add(new TabItem("陣容", Global.GetApplicationPath(base.Request) + "/sports/PlayersRetrieval.aspx?csIndex=" + list.Count + "&eventid=" + m_ID));
                list.Add(new TabItem("即場", Global.GetApplicationPath(base.Request) + "/MatchDetails.aspx?csIndex=" + list.Count + "&Type=Live&ID=" + m_ID));
                list.Add(new TabItem("數據", Global.GetApplicationPath(base.Request) + "/sports/AnalysisStat.aspx?csIndex=" + list.Count + "&eventid=" + m_ID));
                list.Add(new TabItem("近績", Global.GetApplicationPath(base.Request) + "/sports/AnalysisRecent.aspx?csIndex=" + list.Count + "&eventid=" + m_ID));
                list.Add(new TabItem("比數詳情", Global.GetApplicationPath(base.Request) + "/MatchDetails.aspx?Type=HKJC&csIndex=" + list.Count + "&ID=" + m_ID + "&code=" + m_Code));
                ////list.Add(new TabItem("發送分析", Global.GetApplicationPath(base.Request) + "/sports/AnalysisPreview.aspx?csIndex=" + list.Count + "&eventid=" + EventID));
                ////list.Add(new TabItem("發送近績", Global.GetApplicationPath(base.Request) + "/sports/AnalysisSinglePreview.aspx?csIndex=" + list.Count + "&eventid=" + EventID));
            }
            //list.Add(new TabItem("組別管理", Global.GetApplicationPath(base.Request) + "/GroupAdmin.aspx?csIndex=" + list.Count));
            if (Users.UserCheck(name))
            {
                list.Add(new TabItem("用戶管理", Global.GetApplicationPath(base.Request) + "/UserMaintenance.aspx?csIndex=" + list.Count));
                // list.Add(new TabItem("Mms Tool", Global.GetApplicationPath(base.Request) + "/MmsTool.aspx?csIndex=" + list.Count));
            }
            //else
            //{
            //    list.Add(new TabItem("", ""));
            //}
            //list.Add(new TabItem("短訊統計", Global.GetApplicationPath(base.Request) + "/StatSms.aspx?csIndex=" + list.Count));
            if (Users.UserCheck(name))
            {
             ///   list.Add(new TabItem("更改密碼", Global.GetApplicationPath(base.Request) + "/ChangePassword.aspx?csIndex=" + list.Count));
            }
            else
            {
                /// list.Add(new TabItem("", ""));
             ///   list.Add(new TabItem("更改密碼", Global.GetApplicationPath(base.Request) + "/ChangePassword.aspx?csIndex=" + list.Count));

            }
            list.Add(new TabItem("登出 (" + this.Context.User.Identity.Name+")", Global.GetApplicationPath(base.Request) + "/Logout.aspx?csIndex=" + list.Count));
            //if (Session["eventID"] != null)
            //{
            //    list.Add(new TabItem("陣容2", Global.GetApplicationPath(base.Request) + "/sports/PlayersRetrieval.aspx?csIndex=" + list.Count + "&eventid=" + Session["eventID"].ToString()));

            //}
            this.tabs.DataSource = list;
            this.tabs.SelectedIndex = (base.Request["csIndex"] == null) ? 0 : Convert.ToInt32(base.Request["csIndex"]);
            this.tabs.DataBind();
            this.tabs.UpdateAfterCallBack = true;
            //}
        }

        protected void tabs_ItemCommand(object source, DataListCommandEventArgs e)
        {
            //if (e.CommandName.Equals("Answer"))
            //{
            //    int AnswerId = e.Item.ItemIndex; 
            //}
        }

        protected void LnkClicked(Object sender, EventArgs e)
        {
            //LinkButton btn = (LinkButton)sender;
            //string ID = btn.CommandArgument;
            //    Response.Redirect(btn.PostBackUrl);
            // btn.Enabled = false;
        }

        private void tabs_ItemDataBound(object sender, DataListItemEventArgs e)
        {
            //  if ((e.Item.ItemType == ListItemType.Item))
            //{
            HyperLink hl = e.Item.FindControl("hlMenu") as HyperLink;
            if (hl != null)
                //{
                //    hl.Attributes.Add("onclick", "javascript:return CheckUrl('" + hl.Text + "')");
                //}
                // }


                if (tabs.SelectedIndex == 1)
                {
                    if (hl != null && hl.Text != "STATSCORE")
                    {
                        if (hl.Text != "數據" && hl.Text != "登出 (" + this.Context.User.Identity.Name + ")")
                        {
                            hl.Attributes.Add("onclick", "javascript:" + (Session["eventID"] == null ? "alert('請選擇賽事!');return false;" : "return true;"));
                        }
                    }
                    else
                    {
                        hl.Attributes.Add("onclick", "javascript:return CheckUrl('" + hl.Text + "')");
                    }
                    //if (e.Item.ItemIndex == 0)
                    //{
                    //    DataListItem item = e.Item as DataListItem;
                    //    item.Enabled = false;
                    //    item.ForeColor = Color.Black;
                    //}
                }
                else
                {
                    if (hl != null)
                    {
                        hl.Attributes.Add("onclick", "javascript:return CheckUrl('" + hl.Text + "')");
                    }
                }

        }
        private void tabs_ItemCreated(object sender, DataListItemEventArgs e)
        {
            //HyperLink hl1 = e.Item.FindControl("hlMenu") as HyperLink;
            //if (e.Item.ItemType == ListItemType.Footer)
            //{
            //    HyperLink hl = e.Item.FindControl("hlMenu") as HyperLink;
            //    if (hl != null)
            //    {
            //        hl.Attributes.Add("onclick", "javascript:return CheckUrl(this)");
            //    }
            //}
        }


        private string EventID;
        public string eventID
        {
            get { return EventID; }
            set
            {
                EventID = value;
              //  EventID = Session["eventID"] == null ? "0" : Session["eventID"].ToString();
            }
        }
    }
}

