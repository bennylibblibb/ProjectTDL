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
        protected DataList tabs;

        private void InitializeComponent()
        {
             this.tabs.SelectedIndexChanged += new EventHandler(this.Tabs_SelectedIndexChanged);
            // this.tabs.ItemCommand += new DataListCommandEventHandler( this.tabs_ItemCommand);
           // this.tabs.ItemCreated += new DataListItemEventHandler(this.tabs_ItemCreated);
             this.tabs.ItemDataBound += new DataListItemEventHandler(this.tabs_ItemDataBound);
            this.tabs.Load += new EventHandler(this.Page_Load);
            base.Load += new EventHandler(this.Page_Load);
        }


        protected void Tabs_SelectedIndexChanged(object sender, EventArgs e)
        {
            var ddlList = (DataList)sender;
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
            if (list.Count == 2 && (base.Request["csIndex"] != null && base.Request["csIndex"] != "0" && base.Request["csIndex"] != "8" && base.Request["csIndex"] != "9"))
            {
                list.Add(new TabItem("陣容", Global.GetApplicationPath(base.Request) + "/PlayersRetrieval.aspx?csIndex=" + list.Count+"&eventid="+eventID));
                list.Add(new TabItem("分析", Global.GetApplicationPath(base.Request) + "/AnalysisModify.aspx?csIndex=" + list.Count + "&eventid=" + eventID));
                list.Add(new TabItem("數據", Global.GetApplicationPath(base.Request) + "/AnalysisStat.aspx?csIndex=" + list.Count + "&eventid=" + eventID));
                list.Add(new TabItem("近績", Global.GetApplicationPath(base.Request) + "/AnalysisRecent.aspx?csIndex=" + list.Count + "&eventid=" + eventID));
                list.Add(new TabItem("發送分析", Global.GetApplicationPath(base.Request) + "/Lineups.aspx?csIndex=" + list.Count + "&eventid=" + eventID));
                list.Add(new TabItem("發送近績", Global.GetApplicationPath(base.Request) + "/Lineups.aspx?csIndex=" + list.Count + "&eventid=" + eventID));
            }
            if (list.Count == 2 && (base.Request["csIndex"] == null && base.Request["csIndex"] != "1" && base.Request["csIndex"] != "6" && base.Request["csIndex"] != "7"))
            {
                list.Add(new TabItem("球隊排名", Global.GetApplicationPath(base.Request) + "/Lineups.aspx?csIndex=" + list.Count + "&eventid=" + eventID));
                list.Add(new TabItem("射手榜", Global.GetApplicationPath(base.Request) + "/Lineups.aspx?csIndex=" + list.Count + "&eventid=" + eventID));
                list.Add(new TabItem("其它資訊", Global.GetApplicationPath(base.Request) + "/Lineups.aspx?csIndex=" + list.Count + "&eventid=" + eventID));
                list.Add(new TabItem("EnetPlus", Global.GetApplicationPath(base.Request) + "/Lineups.aspx?csIndex=" + list.Count + "&eventid=" + eventID));
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
                list.Add(new TabItem("更改密碼", Global.GetApplicationPath(base.Request) + "/ChangePassword.aspx?csIndex=" + list.Count));
            }
            else
            {
                /// list.Add(new TabItem("", ""));
                list.Add(new TabItem("更改密碼", Global.GetApplicationPath(base.Request) + "/ChangePassword.aspx?csIndex=" + list.Count));

            }
            list.Add(new TabItem("登出", Global.GetApplicationPath(base.Request) + "/Logout.aspx?csIndex=" + list.Count));
            this.tabs.DataSource = list;
            this.tabs.SelectedIndex = (base.Request["csIndex"] == null) ? 0 : Convert.ToInt32(base.Request["csIndex"]);
            this.tabs.DataBind();

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
                {
                    hl.Attributes.Add("onclick", "javascript:return CheckUrl('" + hl.Text + "')");
                }
           // }


            //if (tabs.SelectedIndex == 1)
            //{
            //    if (e.Item.ItemIndex == 0)
            //    {
            //        DataListItem item = e.Item as DataListItem;
            //        item.Enabled = false;
            //        item.ForeColor = Color.Black;
            //    }
            //}
           
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

        private string eventID = string.Empty;
        public string EventID
        {
            get { return eventID; }
            set { eventID = value; }
        }
    }
}

