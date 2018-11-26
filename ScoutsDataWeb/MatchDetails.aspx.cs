using System;
using System.Data;
using System.Web.UI;
using Anthem;
using FirebirdSql.Data.FirebirdClient;
using JC_SoccerWeb.Common;

namespace JC_SoccerWeb
{
	/// <summary>
	/// MemberofSent ���K�n�y�z�C
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

		#region Web Form �]�p�u�㲣�ͪ��{���X

		protected override void OnInit(EventArgs e)
		{
			//
			// CODEGEN: ���� ASP.NET Web Form �]�p�u��һݪ��I�s�C
			//
			InitializeComponent();
			base.OnInit(e);
		}

		/// <summary>
		/// �����]�p�u��䴩�ҥ�������k - �ФŨϥε{���X�s�边�ק�
		/// �o�Ӥ�k�����e�C
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