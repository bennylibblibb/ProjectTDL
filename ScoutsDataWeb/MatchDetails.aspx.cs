using System;
using System.Data;
using System.Web.UI;
using Anthem;
using FirebirdSql.Data.FirebirdClient;
using JC_SoccerWeb.Common;

namespace JC_SoccerWeb
{
	/// <summary>
	/// MemberofSent 的摘要描述。
	/// </summary>
	public class MatchDetails : Page
	{
        protected DataGrid eventDetails;

        private void Page_Load(object sender, EventArgs e)
		{
			if (Request.IsAuthenticated)
			{
				if (!Page.IsPostBack)
				{
                    try
                    {
                        string sID = this.Request.QueryString["ID"].ToString().Trim();
                        using (FbConnection connection = new FbConnection(AppFlag.ScoutsDBConn))
                        {
                            string queryString = "SELECT a.*  FROM  EVENT_DETAILS a where a.eventid='"+ sID+"'";
                            using (FbCommand cmd = new FbCommand(queryString))
                            {
                                using (FbDataAdapter fda = new FbDataAdapter())
                                {
                                    connection.Open();
                                    cmd.Connection = connection;
                                    fda.SelectCommand = cmd;
                                    using (DataSet data = new DataSet())
                                    {
                                        data.Tables.Add(new DataTable("EVENT_DETAILS"));
                                        fda.Fill(data.Tables["EVENT_DETAILS"]);
                                        eventDetails.DataSource = data.Tables[0].DefaultView; 
                                        eventDetails.DataBind(); 
                                        this.eventDetails.UpdateAfterCallBack = true;
                                    }
                                }
                            }
                            connection.Close();
                        }
                    }
                    catch (Exception exp)
                    {
                        string exps = exp.ToString();
                        Files.CicsWriteError(DateTime.Now.ToString("HH:mm:ss") + "   BindStatuses()  " + exps);
                    }
                }
			}
		}

		#region Web Form 設計工具產生的程式碼

		protected override void OnInit(EventArgs e)
		{
			//
			// CODEGEN: 此為 ASP.NET Web Form 設計工具所需的呼叫。
			//
			InitializeComponent();
			base.OnInit(e);
		}

		/// <summary>
		/// 此為設計工具支援所必須的方法 - 請勿使用程式碼編輯器修改
		/// 這個方法的內容。
		/// </summary>
		private void InitializeComponent()
		{
			 this.Load += new System.EventHandler(this.Page_Load);

		}

		#endregion

		private void btnUpdate_Click(object sender, EventArgs e)
		{ 
		}


	}
}