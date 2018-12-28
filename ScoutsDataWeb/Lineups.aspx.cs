using FirebirdSql.Data.FirebirdClient;
using JC_SoccerWeb.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace JC_SoccerWeb
{
    public partial class Lineups : System.Web.UI.Page
    {
        private IFormatProvider culture = new CultureInfo("zh-HK", true);
        protected Anthem.DataGrid dgLineup;
        protected System.Web.UI.WebControls.Label lbUser;
        protected void Page_Load(object sender, EventArgs e)
        { 
            if (!base.Request.IsAuthenticated)
            {
                base.Response.Redirect("Default.aspx", false);
            }
            else if (base.Request.IsAuthenticated && !this.Page.IsPostBack)
            {
                this.txtFrom.Text = DateTime.Now.ToString("yyyy-MM-dd", culture);
                this.txtTo.Text = DateTime.Now.ToString("yyyy-MM-dd", culture);
                this.lbUser.Text = this.Context.User.Identity.Name;
                //   BindStatuses();
                string id = Request["eventid"];
                BindPlayers(id);

            }
        }

        private void BindPlayers(string id)
        {
            try
            {
                using (FbConnection connection = new FbConnection(AppFlag.ScoutsDBConn))
                {
                    string queryString = "SELECT r.ID, r.NAME, r.SHORT_NAME, r.ACRONYM, r.GENDER, r.BIRTHDATE, r.POSITION_NAME, r.AREA_ID," +
                       " r.BNATIONAL, r.UT, r.OLD_PARTICIPANT_ID, r.SLUG,r.TEAM_ID, r.BENCH, r.SHIRT_NR, r.SEASON_ID, r.CTIMESTAMP, r.NAME_CN " +
                       "FROM PLAYERS r inner join   events e on e.HOME_ID=r.TEAM_ID or e.GUEST_ID =r.TEAM_ID where e.id = '2450238' order by r.TEAM_ID";    
                    using (FbCommand cmd = new FbCommand(queryString, connection))
                    {
                        using (FbDataAdapter fda = new FbDataAdapter())
                        {
                            fda.SelectCommand = cmd;
                            using (DataSet data = new DataSet())
                            {
                                data.Tables.Add(new DataTable("players"));
                                fda.Fill(data.Tables["players"]);
                                dgLineup.DataSource = data.Tables[0].DefaultView;
                                dgLineup.PageSize = AppFlag.iPageSize;
                                dgLineup.DataBind();
                                dgLineup.UpdateAfterCallBack = true; 
                            }
                        }
                    }
                    connection.Close();
                }
            }
            catch (Exception exp)
            {
                string exps = exp.ToString();
                Files.CicsWriteError(DateTime.Now.ToString("HH:mm:ss") + "   BindPlayers()  " + exps);
            }
        }

        protected void dgLineup_SelectedIndexChanged(object sender, EventArgs e)
        {
            string d = dgLineup.SelectedIndex.ToString();

        }


    }
}