using System;
using System.Data;
using System.Web.UI;
using Anthem;
using FirebirdSql.Data.FirebirdClient;
using JC_SoccerWeb.Common;

namespace JC_SoccerWeb
{
    public partial class TeamMapping : System.Web.UI.Page
    {
        protected DataGrid dgTeams;
        protected Button btnSave;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.IsAuthenticated)
            {
                if (!Page.IsPostBack)
                {
                    string sID = this.Request.QueryString["ID"].ToString().Trim();
                    // string sType = this.Request.QueryString["Type"].ToString().Trim();
                    BindTeams(sID);
                }
            }
        }

        private void BindTeams(string id)
        {
            if (id == null || id == "") return;
            try
            {
                using (FbConnection connection = new FbConnection(AppFlag.ScoutsDBConn))
                {
                    string queryString = "Select t.id,e.NAME ,t.id,t.name TNAME,t.SHORT_NAME, t.HKJC_ID , t.HKJC_NAME, t.HKJC_NAME_CN from teams t  inner  join  events e  on e.HOME_ID=t.ID or e.GUEST_ID=t.id  where e.id ='" + id+"'";
                    using (FbCommand cmd = new FbCommand(queryString))
                    {
                        using (FbDataAdapter fda = new FbDataAdapter())
                        {
                            connection.Open();
                            cmd.Connection = connection;
                            fda.SelectCommand = cmd;
                            using (DataSet data = new DataSet())
                            {
                                data.Tables.Add(new DataTable("Teams"));
                                fda.Fill(data.Tables["Teams"]);
                                dgTeams.DataSource = data.Tables[0].DefaultView;
                                dgTeams.DataBind();
                                dgTeams.UpdateAfterCallBack = true;
                                lbEvent.Text = data.Tables["Teams"].Rows[0]["id"].ToString() +" "+ data.Tables["Teams"].Rows[0]["name"].ToString();
                            }
                        }
                    }
                    connection.Close();
                }
            }
            catch (Exception exp)
            {
                string exps = exp.ToString();
                Files.CicsWriteError(DateTime.Now.ToString("HH:mm:ss") + " BindTeams(),error: " + exps);
            }
        }
    }
}