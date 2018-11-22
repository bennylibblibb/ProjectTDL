namespace JC_SoccerWeb.UserControl
{
    using Anthem;
    using System;
    using System.Collections;
    using System.Web.UI;

    public class UserTabs : UserControl
    {
        protected DataList tabs;

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
            int num = (base.Request["csIndex"] == null) ? 0 : Convert.ToInt32(base.Request["csIndex"]);
            ArrayList list = new ArrayList();
            list.Add(new TabItem("所有用戶", string.Concat(new object[] { "UserMaintenance.aspx?csindex=", num, "&sIndex=", list.Count })));
            list.Add(new TabItem("新增用戶", string.Concat(new object[] { "UserMaintenance.aspx?csindex=", num, "&sIndex=", list.Count })));
            list.Add(new TabItem("更改用戶", string.Concat(new object[] { "UserMaintenance.aspx?csindex=", num, "&sIndex=", list.Count })));
            list.Add(new TabItem("刪除用戶", string.Concat(new object[] { "UserMaintenance.aspx?csindex=", num, "&sIndex=", list.Count })));
            this.tabs.DataSource = list;
            this.tabs.SelectedIndex = (base.Request["sIndex"] == null) ? 0 : Convert.ToInt32(base.Request["sIndex"]);
            this.tabs.DataBind();
        }
    }
}

