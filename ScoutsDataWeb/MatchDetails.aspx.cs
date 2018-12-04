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
    public class MatchDetails : CommonPage
    {
        protected DataGrid eventDetails;
        protected DataGrid dgGoalInfo;
        protected Button btnSave;
        protected Anthem.Label lbMsg;
        private void Page_Load(object sender, EventArgs e)
        {
            if (Request.IsAuthenticated)
            {
                if (!Page.IsPostBack)
                {
                    string sID = this.Request.QueryString["ID"].ToString().Trim();
                    string sType = this.Request.QueryString["Type"].ToString().Trim();
                    if (sType != "HKJC")
                    { btnSave.Visible = false; }
                    BindResults(sType, sID==""?"-1":sID);
                    BindGoalInfo(sType, sID == "" ? "-1" : sID);
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
            this.btnSave.Click += new EventHandler(this.btnSave_Click);
            this.Load += new System.EventHandler(this.Page_Load);

        }

        #endregion

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (btnSave.Text == "Edit")
            {
                for (int i = 0; i < this.dgGoalInfo.Items.Count; i++)
                {
                    ((System.Web.UI.WebControls.TextBox)this.dgGoalInfo.Items[i].FindControl("txtCNName")).Enabled = true;
                }
                this.btnSave.Text = "Save";
                this.lbMsg.Text = "";
            }
            else if (btnSave.Text == "Save")
            {
                bool results = false;
                int id = -1;
                string strResults = "";
                for (int i = 0; i < this.dgGoalInfo.Items.Count; i++)
                {
                    try
                    {
                        using (FbConnection connection = new FbConnection(AppFlag.ScoutsDBConn))
                        {
                            string strName = ((System.Web.UI.WebControls.TextBox)this.dgGoalInfo.Items[i].FindControl("txtCNName")).Text;
                            string strEventid = ((System.Web.UI.WebControls.Label)this.dgGoalInfo.Items[i].FindControl("lbEventid")).Text;
                            string strPlayerid = ((System.Web.UI.WebControls.Label)this.dgGoalInfo.Items[i].FindControl("lbPlayerid")).Text;
                            string strPlayer = ((System.Web.UI.WebControls.Label)this.dgGoalInfo.Items[i].FindControl("lbPlayerName")).Text;
                            string strTeamid = ((System.Web.UI.WebControls.Label)this.dgGoalInfo.Items[i].FindControl("lbTeamid")).Text;

                            if (strPlayerid != "" && strName != ((System.Web.UI.WebControls.Label)this.dgGoalInfo.Items[i].FindControl("lbCNName")).Text)
                            {
                                id = -1;

                                using (FbCommand cmd = new FbCommand())
                                {
                                    connection.Open();
                                    cmd.CommandText = "Update_CNName_goalinfo_Players";
                                    cmd.CommandType = CommandType.StoredProcedure;
                                    cmd.Connection = connection;
                                    cmd.Parameters.Add("@EMATCHID", strEventid);
                                    cmd.Parameters.Add("@TEAMID", strTeamid == "" ? "-1" : strTeamid);
                                    cmd.Parameters.Add("@PARTICIPANTID", strPlayerid);
                                    cmd.Parameters.Add("@ENName", strPlayer);
                                    cmd.Parameters.Add("@CNName", strName);
                                    cmd.Parameters.Add("@CTIMESTAMP", DateTime.Now);
                                    id = Convert.ToInt32(cmd.ExecuteScalar());
                                    Files.CicsWriteLog((id > 0 ? DateTime.Now.ToString("HH:mm:ss ") + "[Success] " : "[Failure] ") + "Update player name:" + strName + "/" + strPlayer + " [" + strEventid + "] " + strPlayerid);

                                    // this.lbMsg.Text += (id > 0 ? "[Success] " : "[Failure] ") + "Update player name:" + strName + "/" + strPlayer + " [" + strEventid + "] " + strPlayerid +"<br/>";
                                    if (id > 0)
                                    {
                                        results = true;
                                        ((System.Web.UI.WebControls.Label)this.dgGoalInfo.Items[i].FindControl("lbCNName")).Text = strName;
                                    }
                                    else
                                    {
                                        strResults += strPlayerid + "/" + strPlayer + "/" + strName;
                                    }
                                }
                            }
                            connection.Close();
                        }
                    }
                    catch (Exception exp)
                    {
                        this.lbMsg.Text = "[Failure]";
                        string exps = exp.ToString();
                        Files.CicsWriteError(DateTime.Now.ToString("HH:mm:ss") + " btnSave_Click(),error: " + exps);
                        continue;
                    }
                }
                if(results)
                {
                    for (int i = 0; i < this.dgGoalInfo.Items.Count; i++)
                    {
                        ((System.Web.UI.WebControls.TextBox)this.dgGoalInfo.Items[i].FindControl("txtCNName")).Enabled = false;
                    }
                }
                this.btnSave.Text = results ? "Edit" : "Save";
                this.lbMsg.Text = results && strResults == "" ? "[Success]" : results && strResults != "" ? "[Success] " + "but " + strResults + " [Failure]" : !results && strResults == "" ? "" : "[Failure]";
            }
            this.lbMsg.UpdateAfterCallBack = true;
            this.btnSave.UpdateAfterCallBack = true;
            this.dgGoalInfo.UpdateAfterCallBack = true;
        }

        private void BindResults(string Type, string id)
        {
            //id = "2459871";
            try
            {
                using (FbConnection connection = new FbConnection(AppFlag.ScoutsDBConn))
                {
                    //(id == "-1") ? "SELECT x.id, x.name ,x.htname ||':'|| x1.gtname ,x.HOME_ID,x.GUEST_ID,   x.START_DATE, x.HKJCDAYCODE,x.HKJCMATCHNO, x.CTIMESTAMP FROM(select e.id,e.NAME,e.HOME_ID,e.GUEST_ID,t.NAME htname,  e.START_DATE,e.CTIMESTAMP ,m.HKJCDAYCODE,m.HKJCMATCHNO from events e   " +
                    //     "inner join teams t on t.id = e.HOME_ID " + "left JOIN EMATCHES M ON M.EMATCHID = e.ID where e.id= '" + id + "' ) x " + "INNER JOIN(SELECT * FROM( " +
                    //     "select e.id, e.NAME, e.HOME_ID, e.GUEST_ID, t.NAME gtname, e.START_DATE, e.CTIMESTAMP from events e  inner join teams t  on t.id = e.GUEST_ID  where e.id == '" + id + "'  )) x1 " +
                    //     "on x.id = x1.id" :
                    string queryString = 
                        "SELECT  x.ID, X.RNAME, X.TNAME HNAME, X.TNAMECN HNAMECN, X1.NAME GNAME,x1.HKJC_NAME_CN GNAMECN, X.HKJCHOSTNAME,x.HKJCGUESTNAME ,X.FHSD_19, X.RESULT_2 ||':'||  X1.RESULT_2 RESULT,X.HKJCDAYCODE,X.HKJCMATCHNO ,X.HKJCHOSTNAME,x.HKJCGUESTNAME " +
                        "FROM(SELECT R.ID, R.NAME RNAME, T.NAME TNAME, T.HKJC_NAME_CN TNAMECN, E.FHSD_19, P.TEAMTYPE, P.RESULT_2, M.HKJCDAYCODE, M.HKJCMATCHNO, m.HKJCHOSTNAME, m.HKJCGUESTNAME " +
                        "FROM EVENTS R LEFT  JOIN EMATCHES M ON M.EMATCHID = R.ID INNER JOIN  EVENT_DETAILS E ON  R.ID = E.EVENTID " +
                        "INNER JOIN PARTICIPANT_RESULTS P ON P.EVENTID = R.ID INNER JOIN TEAMS T ON T.ID = P.PARTICIPANTID WHERE R.ID = '"+ id + "' and p.TEAMTYPE = 'H') X " +
                        "INNER JOIN(SELECT * FROM(SELECT P2.EVENTID, P2.RESULT_2, T2.NAME, t2.HKJC_NAME_CN FROM PARTICIPANT_RESULTS P2 " +
                        "INNER JOIN TEAMS T2 ON P2.PARTICIPANTID = T2.ID  WHERE  EVENTID = '"+ id + "'  and  TEAMTYPE = 'G')) X1 ON X1.EVENTID = X.ID ";
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
                                if(data.Tables["EVENT_DETAILS"].Rows.Count ==0)
                                {
                                    queryString = " SELECT x.id, x.name ,''  RESULT,x.HOME_ID,x.GUEST_ID,x.hname, x1.gname,x.hnameCN,x1.gnameCN , x.FHSD_19, x.HKJCDAYCODE,x.HKJCMATCHNO, x.HKJCHOSTNAME,x.HKJCGUESTNAME, x.CTIMESTAMP FROM( " +
"select e.id,e.NAME,e.HOME_ID,e.GUEST_ID,t.NAME hname, t.HKJC_NAME_CN hnameCN, e.START_DATE,e.CTIMESTAMP ,m.HKJCDAYCODE,m.HKJCMATCHNO,m.HKJCHOSTNAME,m.HKJCGUESTNAME,b.FHSD_19 from events e " +
"inner join teams t on t.id = e.HOME_ID left JOIN EMATCHES M ON M.EMATCHID = e.ID  left JOIN  EVENT_DETAILS b ON  e.ID = b.EVENTID  where e.id = '" + id+"' ) x " +
"INNER JOIN(SELECT * FROM(select e.id, e.NAME, e.HOME_ID, e.GUEST_ID, t.NAME gname, T.HKJC_NAME_CN gnameCN, e.START_DATE, e.CTIMESTAMP from events e " +
"inner join teams t  on t.id = e.GUEST_ID  where e.id = '"+id+"'  )) x1 on x.id = x1.id";  
                                     cmd.CommandText = queryString;
                                    fda.SelectCommand = cmd;
                                    fda.Fill(data.Tables["EVENT_DETAILS"]);
                                }

                                eventDetails.DataSource = data.Tables[0].DefaultView;
                                eventDetails.DataBind();
                                this.eventDetails.UpdateAfterCallBack = true;
                                this.Page.Title = data.Tables[0].Rows[0]["rname"].ToString() + "   Details";
                            }
                        }
                    }
                    connection.Close();
                }
            }
            catch (Exception exp)
            {
                string exps = exp.ToString();
                Files.CicsWriteError(DateTime.Now.ToString("HH:mm:ss") + " BindResults(),error: " + exps);
            }
        }

        private void BindGoalInfo(string Type, string id)
        {
           //    id = "2459871";
            try
            {
                using (FbConnection connection = new FbConnection(AppFlag.ScoutsDBConn))
                {
                    // string queryString = "select i.TEAMTYPE,  i.PARTICIPANT_NAME,  i.INCIDENT_NAME, i.SUBPARTICIPANT_ID, i.SUBPARTICIPANT_nAME ,t.NAME_CN , i.EVENT_TIME,i.CTIMESTAMP from INCIDENTS  I inner JOIN  players T  ON T.ID=I.SUBPARTICIPANT_ID  WHERE    i.CACTION!='delete' AND   i.CACTION!='insert'  and  (i.INCIDENT_ID='413' or i.INCIDENT_ID='418' or i.INCIDENT_ID='419' ) and " +
                    //  "   EVENTID = '" + id + "' order by i.EVENT_TIME asc";
                    string queryString = "select i.hg, I.EMATCHID, i.player, i.CTYPE,  i.PARTICIPANTID, i.PLAYERCHI ,t.NAME_CN , i.ELAPSED,i.LASTTIME ,i.team_id from MATCHDETAILS I left JOIN  players T  ON T.ID = I.PARTICIPANTID  WHERE i.EMATCHID = '" + id + "' order by i.ELAPSED asc";
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
                                Session["ematchDetials"] = data;
                                dgGoalInfo.DataSource = data.Tables[0].DefaultView;
                                dgGoalInfo.DataBind();
                                dgGoalInfo.UpdateAfterCallBack = true;
                            }
                        }
                    }
                    connection.Close();
                }
            }
            catch (Exception exp)
            {
                string exps = exp.ToString();
                Files.CicsWriteError(DateTime.Now.ToString("HH:mm:ss") + " BindGoalInfo(),error: " + exps);
            } 
        }
    }
}