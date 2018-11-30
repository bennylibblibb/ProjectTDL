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
    public class MatchDetails : CommonPage
    {
        protected DataGrid eventDetails;
        protected DataGrid dgGoalInfo;
        protected Button btnSave;
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

        private void BindResults(string Type, string id)
        {
            //id = "2459871";
            try
            {
                using (FbConnection connection = new FbConnection(AppFlag.ScoutsDBConn))
                {
                    string queryString =  "SELECT  x.ID, X.RNAME, X.TNAME HNAME, X.TNAMECN HNAMECN, X1.NAME GNAME,x1.HKJC_NAME_CN GNAMECN, X.HKJCHOSTNAME,x.HKJCGUESTNAME ,X.FHSD_19, X.RESULT_2 ||':'||  X1.RESULT_2 RESULT,X.HKJCDAYCODE,X.HKJCMATCHNO ,X.HKJCHOSTNAME,x.HKJCGUESTNAME " +
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
                    string queryString = queryString = "select i.TEAMTYPE,  i.PARTICIPANT_NAME,  i.INCIDENT_NAME, i.SUBPARTICIPANT_ID, i.SUBPARTICIPANT_nAME ,T.HKJC_NAME_CN ,i.EVENT_TIME,i.CTIMESTAMP from INCIDENTS  I inner JOIN  TEAMS T  ON T.ID=I.PARTICIPANT_ID  WHERE    i.CACTION!='delete' AND   i.CACTION!='insert'  and  (i.INCIDENT_ID='413' or i.INCIDENT_ID='418' or i.INCIDENT_ID='419' ) and " +
                    "   EVENTID = '" + id + "' order by i.EVENT_TIME asc";
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