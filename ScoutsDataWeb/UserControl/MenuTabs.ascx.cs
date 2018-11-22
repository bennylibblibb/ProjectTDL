namespace JC_SoccerWeb.UserControl
{
    using System.Globalization;
    using JC_SoccerWeb.BLL;
    using System;
    using System.Collections;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    public class MenuTabs : UserControl
    {
        protected DataList tabs;

        private void InitializeComponent()
        {
            this.tabs.ItemDataBound += new DataListItemEventHandler(this.tabs_ItemDataBound);
            this.tabs.Load += new EventHandler(this.Page_Load);
            base.Load += new EventHandler(this.Page_Load);
        }

        protected override void OnInit(EventArgs e)
        {
            this.InitializeComponent();
            base.OnInit(e);
        }

        private void Page_Load(object sender, EventArgs e)
        {
            string name = this.Context.User.Identity.Name;
            ArrayList list = new ArrayList();
            list.Add(new TabItem("Update", Global.GetApplicationPath(base.Request) + "/Update.aspx?csIndex=" + list.Count));
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
        }

        private void tabs_ItemDataBound(object sender, DataListItemEventArgs e)
        {
            if (((e.Item.ItemType == ListItemType.Item) || (e.Item.ItemType == ListItemType.AlternatingItem)) && (((e.Item.ItemIndex == 2) && !Users.UserCheck(this.Context.User.Identity.Name)) || ((e.Item.ItemIndex == 4) && !Users.UserCheck(this.Context.User.Identity.Name))))
            {
                e.Item.Height = 0;
            }
        }
    }
}

