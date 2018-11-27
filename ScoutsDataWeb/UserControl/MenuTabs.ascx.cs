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
            // this.tabs.SelectedIndexChanged += new EventHandler(this.Tabs_SelectedIndexChanged);
            // this.tabs.ItemCommand += new DataListCommandEventHandler( this.tabs_ItemCommand);
            this.tabs.ItemCreated += new DataListItemEventHandler(this.tabs_ItemCreated);
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
            //HyperLink hl = e.Item.FindControl("hlMenu") as HyperLink;
            //if ((e.Item.ItemType == ListItemType.Item))
            //{ 
            HyperLink hl = e.Item.FindControl("hlMenu") as HyperLink;
            if (hl != null)
            {
                hl.Attributes.Add("onclick", "javascript:return CheckUrl('" + hl.Text + "')");
            }
            //}
            //HyperLink hl = e.Item.FindControl("hlMenu") as HyperLink;
            //if (tabs.SelectedIndex == 1)
            //{
            //    if (e.Item.ItemIndex == 0)
            //    {
            //        DataListItem item = e.Item as DataListItem;
            //        item.Enabled = false;
            //    }
            //}
            //if (((e.Item.ItemType == ListItemType.Item) || (e.Item.ItemType == ListItemType.AlternatingItem)) && (((e.Item.ItemIndex == 2) && !Users.UserCheck(this.Context.User.Identity.Name)) || ((e.Item.ItemIndex == 4) && !Users.UserCheck(this.Context.User.Identity.Name))))
            //{
            //    e.Item.Height = 0; 
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


    }
}

