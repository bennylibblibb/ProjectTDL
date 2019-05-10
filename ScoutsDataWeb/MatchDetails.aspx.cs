using System;
using System.Data;
using System.Web.UI;
using Anthem;
using FirebirdSql.Data.FirebirdClient;
using JC_SoccerWeb.Common;
using System.Collections;
using RemoteService.Win32;
using System.Web;
using System.Linq;

namespace JC_SoccerWeb
{
    /// <summary>
    /// MemberofSent 的摘要描述。
    /// </summary>
    public class MatchDetails : System.Web.UI.Page
    {
        protected DataGrid eventDetails;
        protected DataGrid dgGoalInfo, totalDetails;
        protected Button btnSave;
        protected Anthem.Label lbMsg;
        protected Anthem.Label lbAction;
        protected Anthem.Label lbHomeid;
        protected Anthem.Label lbGuestid;
        protected Anthem.Label lbEventid;
        protected Button btnLiveEdit;
        private void Page_Load(object sender, EventArgs e)
        {
            if (Request.IsAuthenticated)
            {
                if (!Page.IsPostBack)
                {
                    string sID = this.Request.QueryString["ID"].ToString().Trim();
                    string sType = this.Request.QueryString["Type"].ToString().Trim();
                    this.lbEventid.Text = sID;
                    if (sType != "HKJC")
                    {
                        btnSave.Visible = false;
                    }

                    if (sType == "HKJC")
                    {
                        BindResults(sType, sID == "" ? "-1" : sID);
                        BindGoalInfo(sType, sID == "" ? "-1" : sID);
                    }
                    else if(sType == "Live")
                    {
                        //btnSave.Visible = false;
                        //btnSave.UpdateAfterCallBack = true;
                        BindGoalInfo("Live", sID == "" ? "-1" : sID);
                        //lbAction.Text = "上次行動:擷取即場數據("+ DateTime.Now.ToString ("HH: mm:ss")+")";
                        //lbAction.UpdateAfterCallBack = true;
                    }

                    //if (this.Request.QueryString["csIndex"] == null)
                    //{
                    //    BindResults(sType, sID == "" ? "-1" : sID);
                    //    BindGoalInfo(sType, sID == "" ? "-1" : sID);
                    //}
                    //else
                    //{
                    //    //btnSave.Visible = false;
                    //    //btnSave.UpdateAfterCallBack = true;
                    //    BindGoalInfo("Live", sID == "" ? "-1" : sID);
                    //    //lbAction.Text = "上次行動:擷取即場數據("+ DateTime.Now.ToString ("HH: mm:ss")+")";
                    //    //lbAction.UpdateAfterCallBack = true;
                    //}

                    lbAction.Text = "上次行動:擷取即場數據(" + DateTime.Now.ToString("HH: mm:ss") + ")";
                    lbAction.UpdateAfterCallBack = true;
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
            this.btnLiveEdit.Click += new EventHandler(this.btnLiveEdit_Click);
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
                string strEventid = "";
                for (int i = 0; i < this.dgGoalInfo.Items.Count; i++)
                {
                    try
                    {
                        using (FbConnection connection = new FbConnection(AppFlag.ScoutsDBConn))
                        {
                            string strName = ((System.Web.UI.WebControls.TextBox)this.dgGoalInfo.Items[i].FindControl("txtCNName")).Text;
                              strEventid = ((System.Web.UI.WebControls.Label)this.dgGoalInfo.Items[i].FindControl("lbEventid")).Text;
                            string strPlayerid = ((System.Web.UI.WebControls.Label)this.dgGoalInfo.Items[i].FindControl("lbPlayerid")).Text;
                            string strPlayer = ((System.Web.UI.WebControls.Label)this.dgGoalInfo.Items[i].FindControl("lbPlayerName")).Text;
                            string strTeamid = ((System.Web.UI.WebControls.Label)this.dgGoalInfo.Items[i].FindControl("lbTeamid")).Text;
                            string strELAPSED = ((System.Web.UI.WebControls.Label)this.dgGoalInfo.Items[i].FindControl("lbELAPSED")).Text;
                            string strHG = ((System.Web.UI.WebControls.Label)this.dgGoalInfo.Items[i].FindControl("lbHG")).Text;
                            // if (strPlayerid != "" && strName != ((System.Web.UI.WebControls.Label)this.dgGoalInfo.Items[i].FindControl("lbCNName")).Text)
                            if (strName != ((System.Web.UI.WebControls.Label)this.dgGoalInfo.Items[i].FindControl("lbCNName")).Text)
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
                                    cmd.Parameters.Add("@HG", strHG);
                                    cmd.Parameters.Add("@ELAPSED", strELAPSED);
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
                if (results)
                {
                    for (int i = 0; i < this.dgGoalInfo.Items.Count; i++)
                    {
                        ((System.Web.UI.WebControls.TextBox)this.dgGoalInfo.Items[i].FindControl("txtCNName")).Enabled = false;
                    }
                     if(strEventid!="") SendWinMsg("GoalDetails-"+ strEventid);
                }
                this.btnSave.Text = results ? "Edit" : "Save";
                this.lbMsg.Text = results && strResults == "" ? "[Success]" : results && strResults != "" ? "[Success] " + "but " + strResults + " [Failure]" : !results && strResults == "" ? "" : "[Failure]";
            }
            this.lbMsg.UpdateAfterCallBack = true;
            this.btnSave.UpdateAfterCallBack = true;
            this.dgGoalInfo.UpdateAfterCallBack = true;
        }

        private void SendWinMsg(string type)
        { 
            try
            {
                Win32Message message = (Win32Message)Activator.GetObject(typeof(Win32Message), AppFlag.ServiceURL);
                if (message != null)
                {
                    //string [] arrFields = (string[])HttpContext.Current.Application["NotifyUpdateTypeArray"];
                    //string sType= arrFields.Where(s => s.ToLower().Contains(type)).ToString();
                    //string id = sType.Substring(sType.IndexOf("-") + 1, sType.Length - sType.IndexOf("-"));
                    string id= type.Substring(type.IndexOf("-") + 1, type.Length - type.IndexOf("-")-1);
                    message.Broadcast("S"+id);
                    Files.CicsWriteLog(DateTime.Now.ToString("HH:mm:ss") + " Sent "+ type);

                }
            }
            catch (Exception exception)
            {
                Files.CicsWriteError(DateTime.Now.ToString("HH:mm:ss") + " SendWinMsg(),error: " + exception);
                this.RegisterStartupScript("key", " <script language='javascript'> alert('PLEASE CHECK REMOTING HOST!'); </script> ");
               }
        }

        private void btnLiveEdit_Click(object sender, EventArgs e)
        {
            if (btnLiveEdit.Text == "Edit")
            {
                for (int i = 0; i < this.totalDetails.Items.Count; i++)
                {
                    ((System.Web.UI.WebControls.TextBox)this.totalDetails.Items[i].FindControl("txtHValue")).Enabled = true;
                    ((System.Web.UI.WebControls.TextBox)this.totalDetails.Items[i].Cells[2].Controls[1]).Enabled = true;
                }
                this.btnLiveEdit.Text = "Save";
                this.lbMsg.Text = "";
            }
            else if (btnLiveEdit.Text == "Save")
            {
                bool results = false;
                ArrayList ar = new ArrayList();
                ArrayList aI = new ArrayList();
                ArrayList aH = new ArrayList();
                ArrayList aG = new ArrayList();
                int id = -1;
                string strResults = "";
                string sType = "";
                string sH = "", sH2 = "";
                string sG = "", sG2 = "";
                for (int i = 0; i < this.totalDetails.Items.Count; i++)
                {
                    sType = ((System.Web.UI.WebControls.Label)this.totalDetails.Items[i].Cells[0].Controls[1]).Text;
                    sH = ((System.Web.UI.WebControls.TextBox)this.totalDetails.Items[i].FindControl("txtHValue")).Text;
                    sG = ((System.Web.UI.WebControls.TextBox)this.totalDetails.Items[i].Cells[2].Controls[1]).Text;
                    sH2 = ((System.Web.UI.WebControls.Label)this.totalDetails.Items[i].FindControl("lbHValue")).Text;
                    sG2 = ((System.Web.UI.WebControls.Label)this.totalDetails.Items[i].Cells[2].Controls[3]).Text;
                    if (sH != sH2 || sG != sG2)
                    {
                        aI.Add(i);
                        aH.Add(sH);
                        aG.Add(sG);
                        ar.Add(sType + "," + sH + "," + sG);
                        results = true;
                        strResults += sType + "," + sH + "," + sG+" / ";
                    }

                }
                if (results)
                {
                    try
                    {
                        using (FbConnection connection = new FbConnection(AppFlag.ScoutsDBConn))
                        {
                            connection.Open();
                            string queryString = "SELECT  r.SOT_20  \"Shot On\", r.SOT_21 \"Shot Off\", r.ATTACKS_10 \"Attacks\",    r.DA_11 \"Dangerous attacks\", r.CORNERS_13 \"Corners\", r.YELLOW_CARDS_8 \"Yellow cards\", r.RED_CARDS_9 \"Red cards\", r.TOTAL_SHOTS_19 \"Total shots\" ,    r.FOULS_22 \"Fouls\", r.OFFSIDES_24 \"Offsides\", r.PS_14 \"Penalties scored\", r.PM_15 \"Penalties missed\", r.PG_16 \"Penalties given\", r.FK_25 \"Free kicks\", r.DFK_26 \"Dangerous free kicks\"," +
                                        "r.FKG_18 \"Free kick goals\" , r.SW_27 \"Shots woodwork\", r.SB_28 \"Shots blocked\" , r.GS_29 \"Goalkeeper saves\", r.GK_30 \"Goal kicks\", r.TI_32 \"Throw-ins\" , r.SUBSTITUTIONS_31 \"Substitutions\",    r.GOALS_40, r.MP_34, r.OWN_GOALS_17, r.ADW_33, r.FORM_716, r.SKIN_718,    r.PS_639, r.PU_697, r.GOALS115_772, r.GOALS1630_773, r.GOALS3145_774,    r.GOALS4660_775, r.GOALS6175_776, r.GOALS7690_777, r.MPG_778, r.MPS_779," +
                                         " r.CTIMESTAMP, r.CACTION, r.TEAMTYPE, r.PARTICIPANTID   FROM PARTICIPANT_STATS r  where r.EVENTID=" + this.lbEventid.Text + " ORDER BY R.TEAMTYPE DESC ";
                            using (FbCommand cmd = new FbCommand(queryString, connection))
                            {
                                using (FbDataAdapter fda = new FbDataAdapter(cmd))
                                {
                                    using (FbCommandBuilder fcb = new FbCommandBuilder(fda))
                                    {
                                        using (DataSet data = new DataSet())
                                        {
                                            data.Tables.Add(new DataTable("liveOdds"));
                                            fda.Fill(data.Tables["liveOdds"]);
                                            for (int i = 0; i < aI.Count; i++)
                                            {
                                                data.Tables[0].Rows[0][Convert.ToInt32(aI[i])] = aH[i];
                                                data.Tables[0].Rows[1][Convert.ToInt32(aI[i])] = aG[i];
                                            }
                                            fda.Update(data.Tables["liveOdds"]);
                                        }
                                    }
                                }
                            }
                            connection.Close();
                        }

                        Files.CicsWriteLog(DateTime.Now.ToString("HH:mm:ss") + " Update ["+ this.lbEventid.Text+"]"+this.totalDetails.Columns[1].HeaderText+"/"+this.totalDetails.Columns[2].HeaderText + " : " + strResults);

                        for (int i = 0; i < this.totalDetails.Items.Count; i++)
                        {
                            ((System.Web.UI.WebControls.TextBox)this.totalDetails.Items[i].Cells[1].Controls[1]).Enabled = false;
                            ((System.Web.UI.WebControls.Label)this.totalDetails.Items[i].FindControl("lbHValue")).Text = ((System.Web.UI.WebControls.TextBox)this.totalDetails.Items[i].Cells[1].Controls[1]).Text;
                             ((System.Web.UI.WebControls.TextBox)this.totalDetails.Items[i].Cells[2].Controls[1]).Enabled = false;
                            ((System.Web.UI.WebControls.Label)this.totalDetails.Items[i].Cells[2].Controls[3]).Text = ((System.Web.UI.WebControls.TextBox)this.totalDetails.Items[i].Cells[2].Controls[1]).Text;
                        }
                    }
                    catch (Exception exp)
                    {
                        results = false;
                        string exps = exp.ToString();
                        Files.CicsWriteError(DateTime.Now.ToString("HH:mm:ss") + " btnLiveEdit_Click(),error: " + exps);
                    } 
                }
                else
                {

                }
                this.btnLiveEdit.Text = results&& strResults != "" ? "Edit" : "Save";
                this.lbMsg.Text = results && strResults != "" ? "[Success]" : strResults == ""?"": "[Failure]";
            }
            this.lbMsg.UpdateAfterCallBack = true;
            this.btnLiveEdit.UpdateAfterCallBack = true;
            this.totalDetails.UpdateAfterCallBack = true;
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
                        "SELECT  x.ID, X.RNAME, X.TNAME HNAME, X.TNAMECN HNAMECN, X1.NAME GNAME,x1.HKJC_NAME_CN GNAMECN, X.HKJCHOSTNAME,x.HKJCGUESTNAME , x.hmnameCN,x.gmnameCN,X.FHSD_19, X.RESULT_2 ||':'||  X1.RESULT_2 RESULT,X.HKJCDAYCODE,X.HKJCMATCHNO ,X.HKJCHOSTNAME,x.HKJCGUESTNAME ,x.STATUS_NAME " +
                        "FROM(SELECT R.ID, R.NAME RNAME, T.NAME TNAME, T.HKJC_NAME_CN TNAMECN, E.FHSD_19, P.TEAMTYPE, P.RESULT_2, M.HKJCDAYCODE, M.HKJCMATCHNO, m.HKJCHOSTNAME, m.HKJCGUESTNAME,m.HKJCHOSTNAME_CN hmnameCN, m.HKJCGUESTNAME_CN gmnameCN ,r.STATUS_NAME " +
                        "FROM EVENTS R LEFT  JOIN EMATCHES M ON M.EMATCHID = R.ID and m.CMATCHDATETIME=r.START_DATE INNER JOIN  EVENT_DETAILS E ON  R.ID = E.EVENTID " +
                        "INNER JOIN PARTICIPANT_RESULTS P ON P.EVENTID = R.ID INNER JOIN TEAMS T ON T.ID = P.PARTICIPANTID WHERE R.ID = '" + id + "' and p.TEAMTYPE = 'H') X " +
                        "INNER JOIN(SELECT * FROM(SELECT P2.EVENTID, P2.RESULT_2, T2.NAME, t2.HKJC_NAME_CN FROM PARTICIPANT_RESULTS P2 " +
                        "INNER JOIN TEAMS T2 ON P2.PARTICIPANTID = T2.ID  WHERE  EVENTID = '" + id + "'  and  TEAMTYPE = 'G')) X1 ON X1.EVENTID = X.ID ";
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
                                if (data.Tables["EVENT_DETAILS"].Rows.Count == 0)
                                {
                                    queryString = " SELECT x.id, x.name rname ,''  RESULT,x.HOME_ID,x.GUEST_ID,x.hname, x1.gname,x.hnameCN,x1.gnameCN , x.hmnameCN,x.gmnameCN ,x.FHSD_19, x.HKJCDAYCODE,x.HKJCMATCHNO, x.HKJCHOSTNAME,x.HKJCGUESTNAME, x.CTIMESTAMP ,X.STATUS_NAME FROM( " +
"select e.id,e.NAME,e.HOME_ID,e.GUEST_ID,t.NAME hname, t.HKJC_NAME_CN hnameCN, e.START_DATE,e.CTIMESTAMP ,m.HKJCDAYCODE,m.HKJCMATCHNO,m.HKJCHOSTNAME,m.HKJCGUESTNAME ,m.HKJCHOSTNAME_CN hmnameCN, m.HKJCGUESTNAME_CN gmnameCN,b.FHSD_19,E.STATUS_NAME from events e " +
"inner join teams t on t.id = e.HOME_ID left JOIN EMATCHES M ON M.EMATCHID = e.ID left JOIN  EVENT_DETAILS b ON  e.ID = b.EVENTID  where e.id = '" + id + "' ) x " +
"INNER JOIN(SELECT * FROM(select e.id, e.NAME, e.HOME_ID, e.GUEST_ID, t.NAME gname, T.HKJC_NAME_CN gnameCN, e.START_DATE, e.CTIMESTAMP from events e " +
"inner join teams t  on t.id = e.GUEST_ID  where e.id = '" + id + "'  )) x1 on x.id = x1.id";
                                    cmd.CommandText = queryString;
                                    fda.SelectCommand = cmd;
                                    fda.Fill(data.Tables["EVENT_DETAILS"]);
                                }

                                eventDetails.DataSource = data.Tables[0].DefaultView;
                                eventDetails.DataBind();
                                this.eventDetails.UpdateAfterCallBack = true;
                                this.Page.Title = (data.Tables[0].Rows.Count > 0) ? data.Tables[0].Rows[0]["rname"].ToString() : "" + "   Details";
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
        {  try
            {
                using (FbConnection connection = new FbConnection(AppFlag.ScoutsDBConn))
                {   string queryString = (Type == "Live") ?
                                "SELECT a.TEAMTYPE HG, a.EVENTID EMATCHID, a.SUBPARTICIPANT_NAME PLAYER, a.INCIDENT_NAME CTYPE, a.SUBPARTICIPANT_ID PARTICIPANTID, '' PLAYERCHI, t.NAME_CN  , cast(SUBSTRING(a.EVENT_TIME FROM 1 FOR 2) as integer)+1 ELAPSED,  a.CTIMESTAMP LASTTIME," +
                               "a.PARTICIPANT_ID TEAM_ID FROM INCIDENTS a left JOIN players T ON CAST(T.ID AS varchar(12)) = a.PARTICIPANT_ID  where a.EVENTID = '" + id + "'  and a.PARTICIPANT_ID !='' order by a.EVENT_TIME asc" :
                    //"select i.hg, I.EMATCHID, i.player, i.CTYPE,  i.PARTICIPANTID, i.PLAYERCHI ,t.NAME_CN , i.ELAPSED,i.LASTTIME ,i.team_id from MATCHDETAILS I left JOIN  players T  ON CAST(T.ID AS varchar(12)) = I.PARTICIPANTID  WHERE i.EMATCHID = '" + id + "' and (i.ctype='ycard' or i.ctype='ycard'  or i.ctype='goal' ) order by i.ELAPSED asc";
                    "select i.hg, I.EMATCHID, i.player, i.CTYPE,  i.PARTICIPANTID, i.PLAYERCHI ,t.CPLAYER_NAME , i.ELAPSED,i.LASTTIME ,i.team_id from MATCHDETAILS I left JOIN  PLAYERS_INFO T  ON CAST(T.PLAYER_ID AS varchar(12)) = I.PARTICIPANTID AND I.team_id=T.TEAM_ID  and t.EVENT_ID=i.EMATCHID WHERE i.EMATCHID = '" + id + "' and (i.ctype='rcard' or i.ctype='ycard'  or i.ctype='goal' ) and (i.N !=0 or i.n is null) order by i.ELAPSED asc";// and I.PLAYER !='' order by i.ELAPSED asc";
                    if (Type == "Live")
                    {
                        this.dgGoalInfo.Visible = false;
                        this.btnSave.Visible = false;
                        
                        //queryString = "SELECT  r.EVENTID,r.PARTICIPANT_ID,e.HKJC_NAME_CN, r.TEAMTYPE," +
                        //    "sum(case when r.INCIDENT_NAME='Goal' or r.INCIDENT_ID=421 then 1 else 0 end ) as Goal ," +
                        //    "sum(case when r.INCIDENT_NAME='Yellow card' then 1 else 0 end ) as Yellowcard ," +
                        //    "sum(case when r.INCIDENT_NAME='Red card' then 1 else 0 end ) as Redcard ," +
                        //    "sum(case when r.INCIDENT_NAME='Substitution in' then 1 else 0 end ) as Substitution " +
                        //    " FROM INCIDENTS r inner join teams e on e.id= cast (r.PARTICIPANT_ID as integer) where r.EVENTID=" + id + " and  r.TEAMTYPE !=''  GROUP BY r.EVENTID, r.TEAMTYPE,r.PARTICIPANT_ID,e.HKJC_NAME_CN";
                        ///  queryString = "SELECT  * FROM ANALYSIS_OTHERS r WHERE R.EVENTID=" + id+ " ORDER BY R.CTEAMTYPE DESC ";
                         queryString = "SELECT t.HKJC_NAME_CN, r.ID, r.EVENTID, r.PARTICIPANTID, r.SOT_20  \"Shot On\", r.SOT_21 \"Shot Off\", r.ATTACKS_10 \"Attacks\",    r.DA_11 \"Dangerous attacks\", r.CORNERS_13 \"Corners\", r.YELLOW_CARDS_8 \"Yellow cards\", r.RED_CARDS_9 \"Red cards\", r.TOTAL_SHOTS_19 \"Total shots\" ,    r.FOULS_22 \"Fouls\", r.OFFSIDES_24 \"Offsides\", r.PS_14 \"Penalties scored\", r.PM_15 \"Penalties missed\", r.PG_16 \"Penalties given\", r.FK_25 \"Free kicks\", r.DFK_26 \"Dangerous free kicks\"," +
                                     "r.FKG_18 \"Free kick goals\" , r.SW_27 \"Shots woodwork\", r.SB_28 \"Shots blocked\" , r.GS_29 \"Goalkeeper saves\", r.GK_30 \"Goal kicks\", r.TI_32 \"Throw-ins\" , r.SUBSTITUTIONS_31 \"Substitutions\",    r.GOALS_40, r.MP_34, r.OWN_GOALS_17, r.ADW_33, r.FORM_716, r.SKIN_718,    r.PS_639, r.PU_697, r.GOALS115_772, r.GOALS1630_773, r.GOALS3145_774,    r.GOALS4660_775, r.GOALS6175_776, r.GOALS7690_777, r.MPG_778, r.MPS_779," +
                                      " r.CTIMESTAMP, r.CACTION, r.TEAMTYPE   FROM PARTICIPANT_STATS r inner join  teams t on t.id =r.PARTICIPANTID where r.EVENTID=" + id + " ORDER BY R.TEAMTYPE DESC ";
                        using (FbCommand cmd = new FbCommand(queryString))
                        {
                            using (FbDataAdapter fda = new FbDataAdapter())
                            {
                                connection.Open();
                                cmd.Connection = connection;
                                fda.SelectCommand = cmd;
                                using (DataSet data = new DataSet())
                                {
                                    data.Tables.Add(new DataTable("lEVENT_DETAILS"));
                                    fda.Fill(data.Tables["lEVENT_DETAILS"]);

                                    DataTable tb = new DataTable();
                                    DataColumn[] cols = new DataColumn[3];
                                    cols[0] = new DataColumn("Type", typeof(String));
                                    cols[1] = new DataColumn("H", typeof(String));
                                    cols[2] = new DataColumn("G", typeof(String));
                                    tb.Columns.AddRange(cols);
                                    DataRow tbDr ;
                                    //foreach (DataRow dr in data.Tables[0].Rows)
                                    //{
                                    for (int i = 4; i < 26; i++)
                                    {
                                        tbDr = tb.NewRow();
                                        tbDr[0] = data.Tables[0].Columns[i].ColumnName;
                                        tbDr[1] = data.Tables[0].Rows[0][i];
                                        tbDr[2] = data.Tables[0].Rows[1][i];
                                        tb.Rows.Add(tbDr);
                                    }
                                    //}
                                    // totalDetails.DataSource = data.Tables[0].DefaultView; 
                                    totalDetails.DataSource =tb.DefaultView;
                                    totalDetails.Columns[0].HeaderText = "Type";
                                    totalDetails.Columns[1].HeaderText = data.Tables[0].Rows[0]["HKJC_NAME_CN"].ToString()+"(主)";
                                    totalDetails.Columns[2].HeaderText = data.Tables[0].Rows[1]["HKJC_NAME_CN"].ToString() + "(客)";
                                    totalDetails.DataBind(); 
                                    totalDetails.UpdateAfterCallBack = true;
                                    btnSave.Visible = false;
                                    btnSave.UpdateAfterCallBack = true;
                                    dgGoalInfo.UpdateAfterCallBack = true;
                                     if (data.Tables[0].Rows.Count==2)
                                    {
                                        this.Page.Title = ((data.Tables[0].Rows[0]["TEAMTYPE"].ToString() == "H") ? data.Tables[0].Rows[0]["HKJC_NAME_CN"].ToString()+"-"+ data.Tables[0].Rows[1]["HKJC_NAME_CN"].ToString() : data.Tables[0].Rows[1]["HKJC_NAME_CN"].ToString() + "-" + data.Tables[0].Rows[0]["HKJC_NAME_CN"].ToString() )+ "  即場";
                                    }
                                } 
                            }
                        }
                    }
                    else
                    {
                        this.totalDetails.Visible = false;
                        this.btnLiveEdit.Visible = false;
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
                                    if (data.Tables["EVENT_DETAILS"].Rows.Count == 0)
                                    {
                                        btnSave.Visible = false;
                                        btnSave.UpdateAfterCallBack = true;
                                    }
                                    totalDetails.UpdateAfterCallBack = true;
                                }
                            }
                        }
                    }
                    connection.Close();
                }
                //if (Type == "Live")
                //{
                //    this.Title = id + "即場";
                //}
            }
            catch (Exception exp)
            {
                string exps = exp.ToString();
                Files.CicsWriteError(DateTime.Now.ToString("HH:mm:ss") + " BindGoalInfo(),error: " + exps);
            }

            ////    id = "2459871";
            //try
            //{
            //    using (FbConnection connection = new FbConnection(AppFlag.ScoutsDBConn))
            //    {
            //        // string queryString = "select i.TEAMTYPE,  i.PARTICIPANT_NAME,  i.INCIDENT_NAME, i.SUBPARTICIPANT_ID, i.SUBPARTICIPANT_nAME ,t.NAME_CN , i.EVENT_TIME,i.CTIMESTAMP from INCIDENTS  I inner JOIN  players T  ON T.ID=I.SUBPARTICIPANT_ID  WHERE    i.CACTION!='delete' AND   i.CACTION!='insert'  and  (i.INCIDENT_ID='413' or i.INCIDENT_ID='418' or i.INCIDENT_ID='419' ) and " +
            //        //  "   EVENTID = '" + id + "' order by i.EVENT_TIME asc";
            //        string queryString = (Type == "Live") ?
            //                   // "select i.hg, I.EMATCHID, i.player, i.CTYPE,  i.PARTICIPANTID, i.PLAYERCHI ,t.NAME_CN , i.ELAPSED,i.LASTTIME ,i.team_id from MATCHDETAILS I left JOIN  players T  ON   CAST(T.ID AS varchar(12)) = I.PARTICIPANTID  WHERE i.EMATCHID = '" + id + "' and (i.ctype!='goal') order by i.ELAPSED asc" :
            //                   "SELECT a.TEAMTYPE HG, a.EVENTID EMATCHID, a.SUBPARTICIPANT_NAME PLAYER, a.INCIDENT_NAME CTYPE, a.SUBPARTICIPANT_ID PARTICIPANTID, '' PLAYERCHI, t.NAME_CN  , cast(SUBSTRING(a.EVENT_TIME FROM 1 FOR 2) as integer)+1 ELAPSED,  a.CTIMESTAMP LASTTIME," +
            //                   "a.PARTICIPANT_ID TEAM_ID FROM INCIDENTS a left JOIN players T ON CAST(T.ID AS varchar(12)) = a.PARTICIPANT_ID  where a.EVENTID = '" + id + "'  and a.PARTICIPANT_ID !='' order by a.EVENT_TIME asc" :
            //                 "select i.hg, I.EMATCHID, i.player, i.CTYPE,  i.PARTICIPANTID, i.PLAYERCHI ,t.NAME_CN , i.ELAPSED,i.LASTTIME ,i.team_id from MATCHDETAILS I left JOIN  players T  ON CAST(T.ID AS varchar(12)) = I.PARTICIPANTID  WHERE i.EMATCHID = '" + id + "' and (i.ctype='ycard' or i.ctype='ycard'  or i.ctype='goal' ) order by i.ELAPSED asc";
            //          using (FbCommand cmd = new FbCommand(queryString))
            //        {
            //            using (FbDataAdapter fda = new FbDataAdapter())
            //            {
            //                connection.Open();
            //                cmd.Connection = connection;
            //                fda.SelectCommand = cmd;
            //                using (DataSet data = new DataSet())
            //                {
            //                    data.Tables.Add(new DataTable("EVENT_DETAILS"));
            //                    fda.Fill(data.Tables["EVENT_DETAILS"]);
            //                    Session["ematchDetials"] = data;
            //                    dgGoalInfo.DataSource = data.Tables[0].DefaultView;
            //                    dgGoalInfo.DataBind();
            //                    dgGoalInfo.UpdateAfterCallBack = true;
            //                    if (data.Tables["EVENT_DETAILS"].Rows.Count == 0)
            //                    {
            //                        btnSave.Visible = false;
            //                        btnSave.UpdateAfterCallBack = true;
            //                    }
            //                }
            //            }
            //        }
            //        connection.Close();
            //    }
            //    if(Type == "Live")
            //    {
            //        this.Title = id+ "即場";
            //    }
            //}
            //catch (Exception exp)
            //{
            //    string exps = exp.ToString();
            //    Files.CicsWriteError(DateTime.Now.ToString("HH:mm:ss") + " BindGoalInfo(),error: " + exps);
            //}
        }
    }
}