namespace JC_SoccerWeb.UserControl
{
    using Anthem;
    using JC_SoccerWeb.Common;
    using System;
    using System.Collections;
    using System.Web.UI;

    public class SendTabs : UserControl
    {
        protected DataList tabs;

        private void InitializeComponent()
        {
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
            ArrayList list = new ArrayList();
            int num = (base.Request["csIndex"] == null) ? 0 : Convert.ToInt32(base.Request["csIndex"]);
            list.Add(new TabItem("Update Rank", string.Concat(new object[] { "Update.aspx?csindex=", num, "&sIndex=", list.Count })));
            // if (AppFlag.DefineMessage)
            //{
            //    list.Add(new TabItem("定義預設短訊內容", string.Concat(new object[] { "SendByGroup.aspx?csindex=", num, "&sIndex=", list.Count })));
            //}
            this.tabs.DataSource = list;
            this.tabs.SelectedIndex = (base.Request["sIndex"] == null) ? 0 : Convert.ToInt32(base.Request["sIndex"]);
            this.tabs.DataBind();
        }
    }
}

