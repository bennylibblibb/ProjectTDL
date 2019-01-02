using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls; 

namespace testascx
{
    public class WebForm1 : System.Web.UI.Page
    {
        private void Page_Load(object sender, System.EventArgs e)
        {
            sayhello myuc = FindControl("Sayhello1") as sayhello;
            myuc.MySp = "我是在cs中用属性赋值的！ssdfsdfsdfsdafsdafsdfsdfsd";

            //可以在这里利用FindControl找到用户自定义控件中的控件
            //例如可以将我们定义的控件中的TextBox2找出并在这里给他赋值

            TextBox mybox = myuc.FindControl("TextBox2") as TextBox;
            mybox.Text = "我是在cs中找到控件后添加的值！";
            myuc.sss = "我是用用户控件中的公共属性赋值的！";
        }

        #region Web 窗体设计器生成的代码
        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN: 该调用是 ASP.NET Web 窗体设计器所必需的。
            //
            InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        /// 设计器支持所需的方法 - 不要使用代码编辑器修改
        /// 此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.Load += new System.EventHandler(this.Page_Load);

        }
        #endregion
    }
}