using System;
using System.Data;
using System.Web.UI;
using Anthem;
using FirebirdSql.Data.FirebirdClient;
using JC_SoccerWeb.Common;

namespace JC_SoccerWeb
{
    public partial class WebForm2 : System.Web.UI.Page
    {
        protected DataGrid dgTeams;
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                using (FbConnection connection = new FbConnection(AppFlag.ScoutsDBConn))
                {
                    string queryString = "SELECT r.RDB$DB_KEY, r.EMATCHID, r.LASTTIME, r.GAMESTATUS, r.ELAPSED,    r.LOG_TYPE,r.AID FROM GOALINFO_LOG r  where r.EMATCHID = '2655752' order by r.LASTTIME desc    ";
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