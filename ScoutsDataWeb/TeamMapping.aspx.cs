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
               
        private void InitializeComponent()
        {
            this.btnSave.Click += new EventHandler(this.btnSave_Click);
            base.Load += new EventHandler(this.Page_Load);
        }
        protected override void OnInit(EventArgs e)
        {
            this.InitializeComponent();
            base.OnInit(e);
        }
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
                    string queryString = "Select  e.id eid ,t.id,e.NAME ,t.id,t.name TNAME,t.SHORT_NAME, t.HKJC_ID , t.HKJC_NAME, t.HKJC_NAME_CN,t.MAPPING_STATUS from teams t  inner  join  events e  on e.HOME_ID=t.ID or e.GUEST_ID=t.id  where e.id ='" + id + "'";
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
                                lbEvent.Text = data.Tables["Teams"].Rows[0]["eid"].ToString() + " " + data.Tables["Teams"].Rows[0]["name"].ToString();
                                this.Page.Title = data.Tables["Teams"].Rows[0]["name"].ToString()+" Team Mapping";

                                if (!(data.Tables["Teams"].Rows[0]["MAPPING_STATUS"] is DBNull)&& Convert.ToBoolean(data.Tables["Teams"].Rows[0]["MAPPING_STATUS"]) && !(data.Tables["Teams"].Rows[1]["MAPPING_STATUS"] is DBNull)&& Convert.ToBoolean(data.Tables["Teams"].Rows[1]["MAPPING_STATUS"]))
                                {
                                    btnSave.Visible = false;
                                    btnSave.UpdateAfterCallBack = true;
                                }
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

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                string eventID = this.lbEvent.Text.Substring(0, this.lbEvent.Text.IndexOf(" "));
                using (FbConnection connection = new FbConnection(AppFlag.ScoutsDBConn))
                {
                    connection.Open();
                    using (FbCommand cmd2 = new FbCommand())
                    {
                        cmd2.CommandText = "WEB_SYNC_MANUAL_HKJCTEAM_SURE";
                        cmd2.CommandType = CommandType.StoredProcedure;
                        cmd2.Connection = connection;
                        cmd2.Parameters.Add("@EVENT_ID", eventID);
                        int id = Convert.ToInt32(cmd2.ExecuteScalar());
                        Files.CicsWriteLog((id == 2 ? DateTime.Now.ToString("HH:mm:ss") + " [Success] " : DateTime.Now.ToString("HH:mm:ss") + " [Failure] ") + "Sure [" + lbEvent.Text + "] on Teams");
                        if(id==2)
                        {
                            btnSave.Enabled = false;
                            btnSave.UpdateAfterCallBack = true;
                        }
                    }
                    connection.Close();
                }
            }
            catch (Exception exp)
            {
                string exps = exp.ToString();
                Files.CicsWriteError(DateTime.Now.ToString("HH:mm:ss") + "  btnSave_Click()  " + exps);
            }
        }
        
    }
}