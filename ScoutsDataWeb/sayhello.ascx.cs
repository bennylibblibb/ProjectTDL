namespace testascx
{
    using System.Web.UI.HtmlControls;
    using System;
    using System.Collections;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    public class sayhello : System.Web.UI.UserControl
    {
        protected System.Web.UI.WebControls.TextBox TextBox1;
        protected System.Web.UI.WebControls.TextBox TextBox2;
        protected System.Web.UI.WebControls.Label Label1;
        private string mysp = "";
        protected System.Web.UI.WebControls.Label Label2;
        private string labtext = "";
        public string sss = "";

        public string LabText
        {
            set { labtext = value; }
            get { return labtext; }
        }
        public string MySp
        {
            set
            {
                mysp = value;
            }

            get
            {
                return mysp;
            }
        }

        private void Page_Load(object sender, System.EventArgs e)
        {
            this.TextBox1.Text = MySp;
            this.Label1.Text = LabText;
            this.Label2.Text = sss;
        }

        protected override void OnInit(EventArgs e)
        {
            this.InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        ///设计器支持所需的方法 - 不要使用代码编辑器
        ///修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.Load += new System.EventHandler(this.Page_Load);

        }
    }
}