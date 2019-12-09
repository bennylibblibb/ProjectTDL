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
using System.Web.UI.WebControls;

namespace JC_SoccerWeb
{
    /// <summary>
    /// MemberofSent 的摘要描述。
    /// </summary>
    public class MatchDetails : System.Web.UI.Page
    {
        protected Anthem.DataGrid eventDetails;
        protected Anthem.DataGrid dgGoalInfo, totalDetails;
        protected Anthem.Button btnSave;
        protected Anthem.Label lbMsg;
        protected Anthem.Label lbAction;
        protected Anthem.Label lbHomeid;
        protected Anthem.Label lbGuestid;
        protected Anthem.Label lbEventid;
        protected Anthem.Button btnLiveEdit;
        protected Anthem.Button btnCancel;
        protected Anthem.CheckBox chkToLive;
        protected System.Web.UI.WebControls.CheckBox chkAlert;
        protected Anthem.Button btnSavetoLive;
        protected Anthem.Label lbMsgResult;
        protected Anthem.Button btnAddRow;
        private void Page_Load(object sender, EventArgs e)
        {
            if (Request.IsAuthenticated)
            {
                if (!Page.IsPostBack)
                {
                    this.chkToLive.Attributes.Add("onclick", "if (!confirm('Are you sure ?')) return false;"); 
                    string sID = this.Request.QueryString["ID"].ToString().Trim();
                    string sCode = this.Request.QueryString["code"]==null?"": this.Request.QueryString["code"].ToString().Trim();
                    string sType = this.Request.QueryString["Type"].ToString().Trim();
                    this.lbEventid.Text = sID;
                    //if (sType != "HKJC")
                    //{
                    //    btnSave.Visible = false;
                    //    btnLiveEdit.Visible = false;
                    //}

                    if (sType == "HKJC" || sType == "STATSCORE")
                    {
                        btnLiveEdit.Visible = false;
                        btnLiveEdit.UpdateAfterCallBack = true;
                        btnSave.Visible = true;
                        btnSave.UpdateAfterCallBack = true;
                        // BindResults(sType, sID == "" ? "-1" : sID);
                        BindResults(sCode, sID == "" ? "-1" : sID);
                        BindGoalInfo(sType, sID == "" ? "-1" : sID);
                    }
                    else if (sType == "Live")
                    {
                        btnLiveEdit.Visible = true;
                        btnLiveEdit.UpdateAfterCallBack = true;
                        btnSave.Visible = false;
                        btnSave.UpdateAfterCallBack = true;
                        btnSavetoLive.Visible = false;
                        btnSavetoLive.UpdateAfterCallBack = true;
                        btnAddRow.Visible = false;
                        btnAddRow.UpdateAfterCallBack = true;
                        chkToLive.Visible = false;
                        chkAlert.Visible = false;
                        btnCancel.Visible = false;
                        btnCancel.UpdateAfterCallBack = true;
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
            this.btnAddRow.Click += new EventHandler(this.btnAddRow_Click);
            this.btnSave.Click += new EventHandler(this.btnSave_Click);
            this.btnSavetoLive.Click += new EventHandler(this.btnSavetoLive_Click);
            this.btnCancel.Click += new EventHandler(this.btnCancel_Click);
            this.btnLiveEdit.Click += new EventHandler(this.btnLiveEdit_Click);
            this.Load += new System.EventHandler(this.Page_Load);
            this.eventDetails.ItemDataBound += new DataGridItemEventHandler(this.eventDetails_ItemDataBound);
            this.dgGoalInfo.ItemDataBound += new DataGridItemEventHandler(this.dgGoalInfo_ItemDataBound);
            this.chkToLive.CheckedChanged += new EventHandler(this.chkToLive_CheckedChanged);
        }

        #endregion

        private void chkToLive_CheckedChanged(object sender, EventArgs e)
        {
            string strEventid = this.lbEventid.Text;
            if (strEventid != "")
            {
                try
                {
                    using (FbConnection connection = new FbConnection(AppFlag.ScoutsDBConn))
                    {
                        string sql = "update  GOALINFO g set g.TOLIVE=" + (this.chkToLive.Checked ? "1" : "0") + "  where  g.EMATCHID='" + strEventid + "'";
                        using (FbCommand cmd = new FbCommand( sql, connection))
                        {
                            connection.Open();
                            Files.CicsWriteLog(DateTime.Now.ToString("HH:mm:ss ") +" ToLive sql:"+ sql);
                            int id = Convert.ToInt32(cmd.ExecuteNonQuery());
                            if (id > 0)
                            {
                                this.lbMsgResult.Text = "[Success]";
                                Files.CicsWriteLog(DateTime.Now.ToString("HH:mm:ss ") + "[" + this.Context.User.Identity.Name + "] " + (this.chkToLive.Checked ? "Do" : "Cancel ") + " ToLive.");
                            }
                        }
                        connection.Close();
                    }
                    if (strEventid != "")
                    {
                        Files.CicsWriteLog(DateTime.Now.ToString("HH:mm:ss ") + "Send " + strEventid + " LIVEGOALS/30 ");
                        string sReslut = ConfigManager.SendWinMsg(strEventid == "" ? "-1" : strEventid, "LIVEGOALS/30");
                    }
                }
                catch (Exception exp)
                {
                    this.lbMsgResult.Text = "[Failure]";
                    string exps = exp.ToString();
                    Files.CicsWriteError(DateTime.Now.ToString("HH:mm:ss") + " chkToLive_CheckedChanged(),error: " + exps);
                }
                lbMsgResult.UpdateAfterCallBack = true;
            }
        }

        private void btnAddRow_Click(object sender, EventArgs e)
        {
            DataSet ds = (DataSet)Session["ematchDetials"];
            DataRow dr;
            if (dgGoalInfo.Items.Count > 0)
            {
                DataGridItem dgi = dgGoalInfo.Items[dgGoalInfo.Items.Count - 1] as DataGridItem;

                if (((System.Web.UI.WebControls.Label)dgi.Cells[2].Controls[1]).Text == "")
                {
                    dr = ds.Tables[0].Rows[ds.Tables[0].Rows.Count - 1];
                    dr["hg"] = ((System.Web.UI.WebControls.DropDownList)dgi.FindControl("dplHG")).SelectedItem.Text;
                    dr["CTYPE"] = ((System.Web.UI.WebControls.DropDownList)dgi.FindControl("dplType")).SelectedItem.Text;
                    dr["PLAYERCHI"] = ((System.Web.UI.WebControls.TextBox)dgi.FindControl("txtCNName")).Text;
                    dr["ELAPSED"] = ((System.Web.UI.WebControls.TextBox)dgi.FindControl("txtElapsed")).Text == "" ? "0" : ((System.Web.UI.WebControls.TextBox)dgi.FindControl("txtElapsed")).Text;
                    dr["MatchStatus"] = ((System.Web.UI.WebControls.DropDownList)dgi.FindControl("dplAt")).SelectedItem.Text;
                }
            }
            dr = ds.Tables[0].NewRow();
            ds.Tables[0].Rows.Add(dr);
            dgGoalInfo.DataSource = ds.Tables[0];
            dgGoalInfo.DataBind();
            Session["ematchDetials"] = ds;

            dgGoalInfo.UpdateAfterCallBack = true;
            lbMsg.Text = "";
            lbMsg.UpdateAfterCallBack = true;
            //btnAddRow.Enabled = false;
            //btnAddRow.UpdateAfterCallBack = true;
        }

            private void dgGoalInfo_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            //if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            //{
            try
            {
                string sHG = ((System.Web.UI.WebControls.Label)e.Item.FindControl("lbHG")).Text;
                if (sHG == "")
                {
                    ((System.Web.UI.WebControls.DropDownList)e.Item.FindControl("dplHG")).SelectedItem.Text = "H";
                }
                else
                {
                    ((System.Web.UI.WebControls.DropDownList)e.Item.FindControl("dplHG")).SelectedItem.Text = sHG;
                }
                string sMatchStatus = ((System.Web.UI.WebControls.Label)e.Item.FindControl("lbMatchStatus")).Text;
                if (sMatchStatus.IndexOf("1st half") > -1)
                {
                    ((System.Web.UI.WebControls.DropDownList)e.Item.FindControl("dplAt")).SelectedItem.Text = "1st half";
                }
                else if (sMatchStatus.IndexOf("2nd half") > -1)
                {
                    ((System.Web.UI.WebControls.DropDownList)e.Item.FindControl("dplAt")).SelectedItem.Text = "2nd half";
                }

                if (((System.Web.UI.WebControls.Label)e.Item.FindControl("lbPlayerName")).Text.IndexOf("*") > -1 || ((System.Web.UI.WebControls.Label)e.Item.FindControl("lbCNName")).Text.IndexOf("*") > -1)
                {
                    ((System.Web.UI.WebControls.CheckBox)e.Item.FindControl("chkOwnGoal")).Checked = true;
                }

                string sType = ((System.Web.UI.WebControls.Label)e.Item.FindControl("lbType")).Text;
                if (sType == "")
                {
                    ((System.Web.UI.WebControls.DropDownList)e.Item.FindControl("dplType")).SelectedItem.Text = "goal";
                }
                else
                {
                    ((System.Web.UI.WebControls.DropDownList)e.Item.FindControl("dplType")).SelectedItem.Text = sType;
                }
            }
            catch (Exception exp)
            {
            }
            //}
        }

            private void eventDetails_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item)
            {
                string Status = ((System.Web.UI.WebControls.Label)e.Item.FindControl("lbGoalInfoStatus2")).Text;
                string sGameStatus = "";
                // if (Status == "not_started" || Status == "Not started")
                 
                if (Status == "notstarted")
                { sGameStatus = "0"; }
                else if (Status == "gamestarted")
                { sGameStatus = "1"; }
                else if (Status == "firsthalfended")
                {
                    sGameStatus = "2";
                }
                else if (Status == "secondhalfstarted")
                {
                    sGameStatus = "3";
                }
                else if (Status == "secondhalfended" || Status == "gameended" || Status == "finishedawardedwin" ||
                    Status == "finishedafterextratime" || Status == "finishedafterpenalties")
                {
                    sGameStatus = "4";
                }
                else if (Status == "extratimestarted" || Status == "extratimefirsthalfended" || Status == "extratimesecondhalfstarted"
                    || Status == "penaltyshootout" || Status == "waitingforpenalty")
                {
                    sGameStatus = "5";
                }
                else if (Status == "cancelled"  || Status == "deleted")
                {
                    sGameStatus = "6";
                }
                else if (Status == "interrupted" || Status == "abandoned")
                {
                    sGameStatus = "7";
                }
                else if (Status == "changed")
                {
                    sGameStatus = "8";
                }
                else if (Status == "delayed" || Status == "postponed")
                {
                    sGameStatus = "9";
                }
                else if (Status == "unknown")
                {
                    sGameStatus = "10";
                }
                else if (Status == "notmapped")
                {
                    sGameStatus = "11";
                }
                ((System.Web.UI.WebControls.DropDownList)e.Item.FindControl("dplLeagues")).SelectedValue = sGameStatus;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (dgGoalInfo.Items.Count == 0) return;
            try
            {
                using (FbConnection connection2 = new FbConnection(AppFlag.ScoutsDBConn))
                {
                    connection2.Open();
                    string queryString = "delete from MATCHDETAILS m where m.EMATCHID='" + lbEventid.Text + "'";

                    using (FbCommand cmd2 = new FbCommand(queryString, connection2))
                    {
                        int id = Convert.ToInt32(cmd2.ExecuteScalar());
                        Files.CicsWriteLog((id > -1 ? DateTime.Now.ToString("HH:mm:ss ") + "[Success] " : "[Failure] ") + " Clear " + lbEventid.Text);
                        if (id > -1)
                        {
                            BindGoalInfo(lbEventid.Text);
                            this.lbMsg.Text = "[Success]"; 
                        }
                        else
                        {
                            this.lbMsg.Text = "[Failure]";
                        }
                    }
                }
            }
            catch(Exception exp)
            {
                this.lbMsg.Text = "[Failure]"; 
                Files.CicsWriteError(DateTime.Now.ToString("HH:mm:ss") + " btnClear_Click(),error: " + exp.ToString());
            }
            this.lbMsg.UpdateAfterCallBack = true;

                //if (this.totalDetails.Visible == true)
                //{
                //    for (int i = 0; i < this.totalDetails.Items.Count; i++)
                //    {
                //        ((System.Web.UI.WebControls.TextBox)this.totalDetails.Items[i].FindControl("txtHValue")).Enabled = false; 
                //        ((System.Web.UI.WebControls.TextBox)this.totalDetails.Items[i].Cells[2].Controls[1]).Enabled = false;
                //    }
                //    this.btnLiveEdit.Text = "Edit";
                //    this.totalDetails.UpdateAfterCallBack = true;
                //    this.btnLiveEdit.UpdateAfterCallBack = true;
                //}
                //else
                //{
                //    System.Web.UI.WebControls.DropDownList dpl = (System.Web.UI.WebControls.DropDownList)(this.eventDetails.Items[0].FindControl("dplLeagues"));
                //    ((System.Web.UI.WebControls.TextBox)this.eventDetails.Items[0].FindControl("txtResult1")).Enabled = false;
                //    ((System.Web.UI.WebControls.TextBox)this.eventDetails.Items[0].FindControl("txtResult2")).Enabled = false;
                //    ((System.Web.UI.WebControls.TextBox)this.eventDetails.Items[0].FindControl("txtElapsed")).Enabled = false;
                //    dpl.Enabled = false;
                //    for (int i = 0; i < this.dgGoalInfo.Items.Count; i++)
                //    {
                //        ((System.Web.UI.WebControls.TextBox)this.dgGoalInfo.Items[i].FindControl("txtCNName")).Enabled = false;
                //        ((System.Web.UI.WebControls.TextBox)this.dgGoalInfo.Items[i].FindControl("txtScoreNo")).Enabled = false;
                //        ((System.Web.UI.WebControls.TextBox)this.dgGoalInfo.Items[i].FindControl("txtResult1")).Enabled = false;
                //        ((System.Web.UI.WebControls.TextBox)this.dgGoalInfo.Items[i].FindControl("txtResult2")).Enabled = false;
                //        ((System.Web.UI.WebControls.TextBox)this.dgGoalInfo.Items[i].FindControl("txtElapsed")).Enabled = false;
                //        ((System.Web.UI.WebControls.DropDownList)this.dgGoalInfo.Items[i].FindControl("dplAt")).Enabled = false;
                //        ((System.Web.UI.WebControls.DropDownList)this.dgGoalInfo.Items[i].FindControl("dplHG")).Enabled = false;
                //        ((System.Web.UI.WebControls.CheckBox)this.dgGoalInfo.Items[i].FindControl("chkOwnGoal")).Enabled = false;
                //    }
                //    this.eventDetails.UpdateAfterCallBack = true;
                //    this.dgGoalInfo.UpdateAfterCallBack = true;
                //    this.btnSave.Text = "Edit";
                //    this.btnSave.UpdateAfterCallBack = true;
                //}
        }
        private void btnSavetoLive_Click(object sender, EventArgs e)
        {
            string  strEventid = HttpContext.Current.Request.QueryString["ID"];
            System.Web.UI.WebControls.DropDownList dpl = (System.Web.UI.WebControls.DropDownList)(this.eventDetails.Items[0].FindControl("dplLeagues"));
            string sStatus = dpl.SelectedItem.Value;
            System.Web.UI.WebControls.Label lbStatus = (System.Web.UI.WebControls.Label)(this.eventDetails.Items[0].FindControl("lbGoalInfoStatus"));
            string sResult1 = ((System.Web.UI.WebControls.TextBox)this.eventDetails.Items[0].FindControl("txtResult1")).Text.Trim();
            string sResult2 = ((System.Web.UI.WebControls.TextBox)this.eventDetails.Items[0].FindControl("txtResult2")).Text.Trim();
            string sTimeOfGame = ((System.Web.UI.WebControls.TextBox)this.eventDetails.Items[0].FindControl("txtElapsed")).Text.Trim();
            string sAlert = chkAlert.Checked ? "1" : "0";
            string sToLive = chkToLive.Checked ? "1" : "0";
            string sComments = ((System.Web.UI.WebControls.TextBox)this.eventDetails.Items[0].FindControl("txtComments")).Text.Trim();
            if (strEventid != "" && sStatus != "All")
            {
                try
                {
                    using (FbConnection connection = new FbConnection(AppFlag.ScoutsDBConn))
                    {
                        using (FbCommand cmd = new FbCommand())
                        {
                            connection.Open();
                            //cmd.CommandText = "Update_Status_Goalinfo";
                            cmd.CommandText = "Update_StatusAndOthers_Goalinfo";
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Connection = connection;
                            cmd.Parameters.Add("@EMATCHID", strEventid);
                            cmd.Parameters.Add("@Status", sStatus);
                            cmd.Parameters.Add("@Result1", sResult1==""?"0": sResult1);
                            cmd.Parameters.Add("@Result2", sResult2 == "" ? "0" : sResult2);
                            cmd.Parameters.Add("@TimeOfGame", sTimeOfGame == "" ? "-1" : sTimeOfGame);
                            cmd.Parameters.Add("@ALERT", sAlert);
                            cmd.Parameters.Add("@CCOMMENTS", sComments);
                            cmd.Parameters.Add("@TOLIVE", sToLive);
                            cmd.Parameters.Add("@CTIMESTAMP", DateTime.Now);
                            int id = Convert.ToInt32(cmd.ExecuteScalar());
                            Files.CicsWriteLog((id > 0 ? DateTime.Now.ToString("HH:mm:ss ") + "[Success] " : "[Failure] ") + "Update ["+ strEventid + "] Status: " + lbStatus.Text + " to " + dpl.SelectedItem.Text + "/" + sStatus+" "+ sResult1+":"+sResult2);
                            if (id > 0) this.lbMsgResult.Text = "[Success]";
                        }
                        connection.Close();
                    }
                    if (strEventid != "")
                    {
                        Files.CicsWriteLog(DateTime.Now.ToString("HH:mm:ss ") + "Send " + strEventid + " LIVEGOALS/30 ");
                        string sReslut = ConfigManager.SendWinMsg(strEventid == "" ? "-1" : strEventid, "LIVEGOALS/30");
                    }
                }
                catch (Exception exp)
                {
                    this.lbMsgResult.Text = "[Failure]";
                    string exps = exp.ToString();
                    Files.CicsWriteError(DateTime.Now.ToString("HH:mm:ss") + " btnSavetoLive_Click(),error: " + exps);
                }
                lbMsgResult.UpdateAfterCallBack = true;
            }
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            //System.Web.UI.WebControls.DropDownList dpl = (System.Web.UI.WebControls.DropDownList)(this.eventDetails.Items[0].FindControl("dplLeagues"));
            //System.Web.UI.WebControls.Label lbStatus = (System.Web.UI.WebControls.Label)(this.eventDetails.Items[0].FindControl("lbGoalInfoStatus"));
            //////((System.Web.UI.WebControls.TextBox)this.eventDetails.Items[0].FindControl("txtResult1")).Enabled = true;
            //////((System.Web.UI.WebControls.TextBox)this.eventDetails.Items[0].FindControl("txtResult2")).Enabled = true;
            //////((System.Web.UI.WebControls.TextBox)this.eventDetails.Items[0].FindControl("txtElapsed")).Enabled = true;
            //if (btnSave.Text == "Edit")
            //{
            //    for (int i = 0; i < this.dgGoalInfo.Items.Count; i++)
            //    {
            //        ////((System.Web.UI.WebControls.TextBox)this.dgGoalInfo.Items[i].FindControl("txtCNName")).Enabled = true;
            //        ////((System.Web.UI.WebControls.TextBox)this.dgGoalInfo.Items[i].FindControl("txtScoreNo")).Enabled = true;
            //        ////((System.Web.UI.WebControls.TextBox)this.dgGoalInfo.Items[i].FindControl("txtResult1")).Enabled = true;
            //        ////((System.Web.UI.WebControls.TextBox)this.dgGoalInfo.Items[i].FindControl("txtResult2")).Enabled = true;
            //        ////((System.Web.UI.WebControls.TextBox)this.dgGoalInfo.Items[i].FindControl("txtElapsed")).Enabled = true;
            //        ////((System.Web.UI.WebControls.DropDownList)this.dgGoalInfo.Items[i].FindControl("dplAt")).Enabled = true;
            //        ////((System.Web.UI.WebControls.DropDownList)this.dgGoalInfo.Items[i].FindControl("dplHG")).Enabled = true;
            //        ////((System.Web.UI.WebControls.CheckBox)this.dgGoalInfo.Items[i].FindControl("chkOwnGoal")).Enabled = true;
            //    }
            //    this.btnSave.Text = "Save";
            //    this.lbMsg.Text = "";
            //    dpl.Enabled = true;
            //}
            //else if (btnSave.Text == "Send")
            //{
                bool results = false;
                int id = -1;
                string strResults = "";
                 string strEventid = "";
                strEventid = HttpContext.Current.Request.QueryString["ID"];
                //string sStatus = dpl.SelectedItem.Value;
                //string sResult1 = ((System.Web.UI.WebControls.TextBox)this.eventDetails.Items[0].FindControl("txtResult1")).Text.Trim();
                //string sResult2 = ((System.Web.UI.WebControls.TextBox)this.eventDetails.Items[0].FindControl("txtResult2")).Text.Trim();
                //string sTimeOfGame = ((System.Web.UI.WebControls.TextBox)this.eventDetails.Items[0].FindControl("txtElapsed")).Text.Trim();
                //string sAlert = chkAlert.Checked?"1":"0";
                //string sToLive = chkToLive.Checked ? "1" : "0";
                //string sComments = ((System.Web.UI.WebControls.TextBox)this.eventDetails.Items[0].FindControl("txtComments")).Text.Trim();
                //if (strEventid != "" && sStatus != "All")
                //{
                //    results = true;
                //    try
                //    {
                //        using (FbConnection connection = new FbConnection(AppFlag.ScoutsDBConn))
                //        {
                //            using (FbCommand cmd = new FbCommand())
                //            {
                //                connection.Open();
                //                //cmd.CommandText = "Update_Status_Goalinfo";
                //                cmd.CommandText = "Update_StatusAndOthers_Goalinfo";
                //                cmd.CommandType = CommandType.StoredProcedure;
                //                cmd.Connection = connection;
                //                cmd.Parameters.Add("@EMATCHID", strEventid);
                //                cmd.Parameters.Add("@Status", sStatus);
                //                cmd.Parameters.Add("@Result1", sResult1);
                //                cmd.Parameters.Add("@Result2", sResult2);
                //                cmd.Parameters.Add("@TimeOfGame", sTimeOfGame==""?"-1": sTimeOfGame);
                //                cmd.Parameters.Add("@ALERT", sAlert);
                //                cmd.Parameters.Add("@CCOMMENTS", sComments);
                //                cmd.Parameters.Add("@TOLIVE", sToLive); 
                //                cmd.Parameters.Add("@CTIMESTAMP", DateTime.Now);
                //                id = Convert.ToInt32(cmd.ExecuteScalar());
                //                Files.CicsWriteLog((id > 0 ? DateTime.Now.ToString("HH:mm:ss ") + "[Success] " : "[Failure] ") + "Update Status: " + lbStatus.Text + " to " + dpl.SelectedItem.Text + "/" + sStatus);
                //                if (id > 0) { results = true; }
                //            }
                //            connection.Close();
                //        }
                //    }
                //    catch (Exception exp)
                //    {
                //        this.lbMsg.Text = "[Failure]";
                //        string exps = exp.ToString();
                //        Files.CicsWriteError(DateTime.Now.ToString("HH:mm:ss") + " btnSave_Click(),error: " + exps);
                //    }
                //}

                id = -1;
                for (int i = 0; i < this.dgGoalInfo.Items.Count; i++)
                {
                    try
                    {
                        using (FbConnection connection = new FbConnection(AppFlag.ScoutsDBConn))
                        {
                            string strName = ((System.Web.UI.WebControls.TextBox)this.dgGoalInfo.Items[i].FindControl("txtCNName")).Text;
                            strEventid = lbEventid.Text; //((System.Web.UI.WebControls.Label)this.dgGoalInfo.Items[i].FindControl("lbEventid")).Text;
                            string strIncid= ((System.Web.UI.WebControls.Label)this.dgGoalInfo.Items[i].FindControl("lbIncid")).Text;
                            string strPlayerid = ((System.Web.UI.WebControls.Label)this.dgGoalInfo.Items[i].FindControl("lbPlayerid")).Text;
                            string strPlayer = ((System.Web.UI.WebControls.TextBox)this.dgGoalInfo.Items[i].FindControl("txtPlayerName")).Text;
                            string strTeamid = ((System.Web.UI.WebControls.Label)this.dgGoalInfo.Items[i].FindControl("lbTeamid")).Text;
                            string strELAPSED = ((System.Web.UI.WebControls.TextBox)this.dgGoalInfo.Items[i].FindControl("txtElapsed")).Text;
                            string strHG = ((System.Web.UI.WebControls.DropDownList)(this.dgGoalInfo.Items[i].FindControl("dplHG"))).SelectedItem.Text;
                            // if (strPlayerid != "" && strName != ((System.Web.UI.WebControls.Label)this.dgGoalInfo.Items[i].FindControl("lbCNName")).Text)
                            string strType = ((System.Web.UI.WebControls.DropDownList)(this.dgGoalInfo.Items[i].FindControl("dplType"))).SelectedItem.Text;
                            string strStatus = ((System.Web.UI.WebControls.DropDownList)(this.dgGoalInfo.Items[i].FindControl("dplAt"))).SelectedItem.Text;
                            string strPlayerNO = ((System.Web.UI.WebControls.TextBox)this.dgGoalInfo.Items[i].FindControl("txtScoreNo")).Text;
                            string strOwanGoal= ((System.Web.UI.WebControls.CheckBox)this.dgGoalInfo.Items[i].FindControl("chkOwnGoal")).Checked==true?"1":"0";

                            //if (strIncid == "" 20191205
                            //    ||strName != ((System.Web.UI.WebControls.Label)this.dgGoalInfo.Items[i].FindControl("lbCNName")).Text
                            //    || strELAPSED != ((System.Web.UI.WebControls.Label)this.dgGoalInfo.Items[i].FindControl("lbELAPSED")).Text
                            //    || strStatus != ((System.Web.UI.WebControls.Label)this.dgGoalInfo.Items[i].FindControl("lbMatchStatus")).Text
                            //    || strHG != ((System.Web.UI.WebControls.Label)this.dgGoalInfo.Items[i].FindControl("lbHG")).Text
                            //    || strType != ((System.Web.UI.WebControls.Label)this.dgGoalInfo.Items[i].FindControl("lbType")).Text)
                            if(strEventid!="")
                            {
                            //Files.CicsWriteError( strEventid + "/"+ strIncid + "/"+ strHG + "/"+strTeamid+"/" +strType+ "/"+ strPlayerNO
                            //   + "/" + strPlayerid + "/"+ strELAPSED + "/" + strStatus + "/" + strPlayer + "/"+ strName + "/" + strOwanGoal );
                               id = -1; 
                                using (FbCommand cmd = new FbCommand())
                                {
                                    connection.Open();
                                    // cmd.CommandText = "Update_CNName_goalinfo_Players";
                                    cmd.CommandText = "PR_MATCHDETAILS_Players";
                                    cmd.CommandType = CommandType.StoredProcedure;
                                    cmd.Connection = connection;
                                    cmd.Parameters.Add("@EMATCHID", strEventid);
                                    cmd.Parameters.Add("@INCIDENTS_ID", strIncid);   
                                    cmd.Parameters.Add("@HG", strHG);
                                    cmd.Parameters.Add("@TEAMID", strTeamid != "" ?strTeamid: (strHG=="H"?lbHomeid.Text:lbGuestid.Text));
                                    cmd.Parameters.Add("@CTYPE", strType);
                                    cmd.Parameters.Add("@IPLAYER_NO", strPlayerNO); 
                                    cmd.Parameters.Add("@PARTICIPANTID", strPlayerid); 
                                    cmd.Parameters.Add("@ELAPSED", strELAPSED==""?"0": strELAPSED);
                                    cmd.Parameters.Add("@STATUS", strStatus);
                                    cmd.Parameters.Add("@ENName", strPlayer);
                                    cmd.Parameters.Add("@CNName", strName==""?strPlayer: strName);
                                    cmd.Parameters.Add("@OWNGOAL", strOwanGoal);
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
                    ////for (int i = 0; i < this.dgGoalInfo.Items.Count; i++)
                    ////{
                    ////    ((System.Web.UI.WebControls.TextBox)this.dgGoalInfo.Items[i].FindControl("txtCNName")).Enabled = false;
                    ////    ((System.Web.UI.WebControls.TextBox)this.dgGoalInfo.Items[i].FindControl("txtScoreNo")).Enabled = false;
                    ////    ((System.Web.UI.WebControls.TextBox)this.dgGoalInfo.Items[i].FindControl("txtResult1")).Enabled = false;
                    ////    ((System.Web.UI.WebControls.TextBox)this.dgGoalInfo.Items[i].FindControl("txtResult2")).Enabled = false;
                    ////    ((System.Web.UI.WebControls.TextBox)this.dgGoalInfo.Items[i].FindControl("txtElapsed")).Enabled = false;
                    ////    ((System.Web.UI.WebControls.DropDownList)this.dgGoalInfo.Items[i].FindControl("dplAt")).Enabled = false;
                    ////    ((System.Web.UI.WebControls.DropDownList)this.dgGoalInfo.Items[i].FindControl("dplHG")).Enabled = false;
                    ////    ((System.Web.UI.WebControls.CheckBox)this.dgGoalInfo.Items[i].FindControl("chkOwnGoal")).Enabled = false;
                    ////}
                    ////((System.Web.UI.WebControls.TextBox)this.eventDetails.Items[0].FindControl("txtResult1")).Enabled = false;
                    ////((System.Web.UI.WebControls.TextBox)this.eventDetails.Items[0].FindControl("txtResult2")).Enabled = false;
                    ////((System.Web.UI.WebControls.TextBox)this.eventDetails.Items[0].FindControl("txtElapsed")).Enabled = false;
                    ////dpl.Enabled = false;
                    if (strEventid != "")
                    {
                        ////JC_SoccerWeb.Common.Files.CicsWriteLog(DateTime.Now.ToString("HH:mm:ss ") + "Send " + strEventid + " LIVEGOALS/30 ");
                        ////string sReslut = ConfigManager.SendWinMsg(strEventid == "" ? "-1" : strEventid, "LIVEGOALS/30");

                        JC_SoccerWeb.Common.Files.CicsWriteLog(DateTime.Now.ToString("HH:mm:ss ") + "Send " + strEventid + " GoalDetails/31 ");
                        string sReslut = ConfigManager.SendWinMsg(strEventid, "GoalDetails/31");
                    }
                }
                ////  this.btnSave.Text = results ? "Edit" : "Save";
                BindGoalInfo(lbEventid.Text);
                this.lbMsg.Text = results && strResults == "" ? "[Success]" : results && strResults != "" ? "[Success] " + "but " + strResults + " [Failure]" : !results && strResults == "" ? "" : "[Failure]";
            //}
            //btnAddRow.Enabled = true;
            //btnAddRow.UpdateAfterCallBack = true;
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
                    string id = type.Substring(type.IndexOf("-") + 1, type.Length - type.IndexOf("-") - 1);
                    message.Broadcast("S" + id);
                    Files.CicsWriteLog(DateTime.Now.ToString("HH:mm:ss") + " Sent " + type);

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
                        strResults += sType + "," + sH + "," + sG + " / ";
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
                                        "r.FKG_18 \"Free kick goals\" , r.SW_27 \"Shots woodwork\", r.SB_28 \"Shots blocked\" , r.GS_29 \"Goalkeeper saves\", r.GK_30 \"Goal kicks\", r.TI_32 \"Throw-ins\" , r.SUBSTITUTIONS_31 \"Substitutions\", r.BPP_771 \"Possession(%)\",    r.GOALS_40, r.MP_34, r.OWN_GOALS_17, r.ADW_33, r.FORM_716, r.SKIN_718,    r.PS_639, r.PU_697, r.GOALS115_772, r.GOALS1630_773, r.GOALS3145_774,    r.GOALS4660_775, r.GOALS6175_776, r.GOALS7690_777, r.MPG_778, r.MPS_779," +
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

                        Files.CicsWriteLog(DateTime.Now.ToString("HH:mm:ss") + " Update [" + this.lbEventid.Text + "]" + this.totalDetails.Columns[1].HeaderText + "/" + this.totalDetails.Columns[2].HeaderText + " : " + strResults);

                        for (int i = 0; i < this.totalDetails.Items.Count; i++)
                        {
                            ((System.Web.UI.WebControls.TextBox)this.totalDetails.Items[i].Cells[1].Controls[1]).Enabled = false;
                            ((System.Web.UI.WebControls.Label)this.totalDetails.Items[i].FindControl("lbHValue")).Text = ((System.Web.UI.WebControls.TextBox)this.totalDetails.Items[i].Cells[1].Controls[1]).Text;
                            ((System.Web.UI.WebControls.TextBox)this.totalDetails.Items[i].Cells[2].Controls[1]).Enabled = false;
                            ((System.Web.UI.WebControls.Label)this.totalDetails.Items[i].Cells[2].Controls[3]).Text = ((System.Web.UI.WebControls.TextBox)this.totalDetails.Items[i].Cells[2].Controls[1]).Text;
                        }

                        //Files.CicsWriteLog(DateTime.Now.ToString("HH:mm:ss ") + "Sql: " + playerQuery);
                        if (this.lbEventid.Text != "")
                        {
                            JC_SoccerWeb.Common.Files.CicsWriteLog(DateTime.Now.ToString("HH:mm:ss ") + "Send " + this.lbEventid.Text + " ANALYSISOTHER/62");
                            string sReslut = ConfigManager.SendWinMsg(this.lbEventid.Text, "ANALYSISOTHER/62");
                            //  if (sReslut != "Done") JC_SoccerWeb.Common.Files.CicsWriteLog(DateTime.Now.ToString("HH:mm:ss ")+ "[Failure] Send " + sMatchCount + " ANALYSISBGREMARK-10, " + sReslut);
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
                this.btnLiveEdit.Text = results && strResults != "" ? "Edit" : "Save";
                this.lbMsg.Text = results && strResults != "" ? "[Success]" : strResults == "" ? "" : "[Failure]";
            }
            this.lbMsg.UpdateAfterCallBack = true;
            this.btnLiveEdit.UpdateAfterCallBack = true;
            this.totalDetails.UpdateAfterCallBack = true;
        }


        private void BindResults(string Code, string id)
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
                    //string queryString =
                    //    "SELECT x.GAMESTATUS, x.ID, X.RNAME, X.TNAME HNAME, X.TNAMECN HNAMECN, X1.NAME GNAME,x1.HKJC_NAME_CN GNAMECN, X.HKJCHOSTNAME,x.HKJCGUESTNAME , x.hmnameCN,x.gmnameCN,X.FHSD_19, X.RESULT_2 ||':'||  X1.RESULT_2 RESULT,X.HKJCDAYCODE,X.HKJCMATCHNO ,X.HKJCHOSTNAME,x.HKJCGUESTNAME ,x.STATUS_NAME " +
                    //    "FROM(SELECT  g.GAMESTATUS,R.ID, R.NAME RNAME, T.NAME TNAME, T.HKJC_NAME_CN TNAMECN, E.FHSD_19, P.TEAMTYPE, P.RESULT_2, M.HKJCDAYCODE, M.HKJCMATCHNO, m.HKJCHOSTNAME, m.HKJCGUESTNAME,m.HKJCHOSTNAME_CN hmnameCN, m.HKJCGUESTNAME_CN gmnameCN ,r.STATUS_NAME " +
                    //    "FROM EVENTS R LEFT  JOIN EMATCHES M ON M.EMATCHID = R.ID and m.CMATCHDATETIME=r.START_DATE  inner join GOALINFO g on g.EMATCHID=r.ID   INNER JOIN  EVENT_DETAILS E ON  R.ID = E.EVENTID " +
                    //    "INNER JOIN PARTICIPANT_RESULTS P ON P.EVENTID = R.ID INNER JOIN TEAMS T ON T.ID = P.PARTICIPANTID WHERE R.ID = '" + id + "' and p.TEAMTYPE = 'H') X " +
                    //    "INNER JOIN(SELECT * FROM(SELECT P2.EVENTID, P2.RESULT_2, T2.NAME, t2.HKJC_NAME_CN FROM PARTICIPANT_RESULTS P2 " +
                    //    "INNER JOIN TEAMS T2 ON P2.PARTICIPANTID = T2.ID  WHERE  EVENTID = '" + id + "'  and  TEAMTYPE = 'G')) X1 ON X1.EVENTID = X.ID ";

                    //string queryString = "select first 1 r.EMATCHID,r.HKJCDAYCODE,r.HKJCMATCHNO,r.HKJCHOSTNAME_CN,r.HKJCGUESTNAME_CN,r.HKJCHOSTNAME,r.HKJCGUESTNAME,  e.NAME,e.HOME_ID,(select  hkjc_name_cn from teams where  id =e.HOME_ID) HTName,   e.GUEST_ID,(select  hkjc_name_cn from teams   where  id =e.GUEST_ID) GTName,e.id,e.START_DATE,d.FHSD_19, e.STATUS_NAME,g.GAMESTATUS,g.H_GOAL||':'||g.G_GOAL, p.* from PARTICIPANT_RESULTS p  inner join EMATCHES r on r.EMATCHID=p.EVENTID inner join events e on e.id =p.EVENTID   INNER JOIN  EVENT_DETAILS d ON  d.EVENTID = p.EVENTID inner join GOALINFO g on g.EMATCHID=p.EVENTID where p.EVENTID=2893383    order by p.CTIMESTAMP desc ";
                    //string queryString = "select first 1 e.NAME, r.EMATCHID,r.HKJCDAYCODE,r.HKJCMATCHNO,r.HKJCHOSTNAME_CN,r.HKJCGUESTNAME_CN,r.HKJCHOSTNAME,r.HKJCGUESTNAME,  e.NAME,e.HOME_ID,(select  hkjc_name_cn from teams where  id =e.HOME_ID) HTName,   e.GUEST_ID,(select  hkjc_name_cn from teams   where  id =e.GUEST_ID) GTName,e.id, d.FHSD_19, e.STATUS_NAME,g.GAMESTATUS,g.H_GOAL||':'||g.G_GOAL RESULT from GOALINFO g " +
                    //    " inner join EMATCHES r on r.EMATCHID=g.EMATCHID   inner join events e on e.id =g.EMATCHID  INNER JOIN  EVENT_DETAILS d ON  d.EVENTID = g.EMATCHID " +
                    //    " where g.EMATCHID=" + id + " order by g.LASTTIME desc ";
                    //queryString = "select  e.id, e.NAME,e.HOME_ID,(select  hkjc_name_cn from teams where  id =e.HOME_ID) HTName,  e.GUEST_ID,(select  hkjc_name_cn from teams   where  id =e.GUEST_ID) GTName,r.EMATCHID,r.HKJCDAYCODE,r.HKJCMATCHNO,r.HKJCHOSTNAME_CN,r.HKJCGUESTNAME_CN,r.HKJCHOSTNAME,r.HKJCGUESTNAME,    d.FHSD_19, e.STATUS_NAME,g.GAMESTATUS,g.H_GOAL||':'||g.G_GOAL RESULT from events e" +
                    //    " inner join  EMATCHES R on e.id =R.EMATCHID inner join  GOALINFO g   on r.EMATCHID=g.EMATCHID INNER JOIN  EVENT_DETAILS d ON  d.EVENTID = g.EMATCHID " +
                    //    " where E.ID=2893389 order by g.LASTTIME desc ";
                    string queryString = (id == "-1") ? "select FIRST 1 g.tolive,g.alert,e.id, e.NAME,e.start_date, R.CMATCHDATETIME , e.HOME_ID,(select  hkjc_name_cn from teams where  id =e.HOME_ID) HTName,  e.GUEST_ID,(select  hkjc_name_cn from teams   where  id =e.GUEST_ID) GTName,r.EMATCHID,r.HKJCDAYCODE,r.HKJCMATCHNO,r.HKJCHOSTNAME_CN,r.HKJCGUESTNAME_CN,r.HKJCHOSTNAME,r.HKJCGUESTNAME,    d.FHSD_19, e.STATUS_NAME,g.GAMESTATUS,g.H_GOAL||':'||g.G_GOAL RESULT,G.ELAPSED,G.CCOMMENTS from EMATCHES R " +
                        "LEFT join  EVENTS E on e.id =R.EMATCHID LEFT join  GOALINFO g   on E.ID=g.EMATCHID LEFT JOIN  EVENT_DETAILS d ON  d.EVENTID = g.EMATCHID " +
                        "Where r.HKJCDAYCODE='" + Code.Substring(0, 3) + "' AND r.HKJCMATCHNO =" + Code.Substring(4, Code.Length - 4) + " order by  R.CMATCHDATETIME  desc "
                        : "select FIRST 1 g.tolive,g.alert, e.id, e.NAME,e.start_date, e.HOME_ID,(select  hkjc_name_cn from teams where  id =e.HOME_ID) HTName,  e.GUEST_ID,(select  hkjc_name_cn from teams   where  id =e.GUEST_ID) GTName,r.EMATCHID,r.HKJCDAYCODE,r.HKJCMATCHNO,r.HKJCHOSTNAME_CN,r.HKJCGUESTNAME_CN,r.HKJCHOSTNAME,r.HKJCGUESTNAME,    d.FHSD_19, s.name STATUS_NAME,g.GAMESTATUS,g.H_GOAL||':'||g.G_GOAL RESULT,G.ELAPSED,G.CCOMMENTS,g.startdate CMATCHDATETIME from events e " +
                        " LEFT join  EMATCHES R on e.id =R.EMATCHID LEFT join  GOALINFO g   on E.ID=g.EMATCHID inner join STATUSES s on s.id =g.HRUNSCOREID   LEFT JOIN  EVENT_DETAILS d ON  d.EVENTID = g.EMATCHID " +
                        "Where E.ID=" + id + " order by E.CTIMESTAMP desc ";
                    Files.CicsWriteLog(DateTime.Now.ToString("HH:mm:ss") + " Sql1: " + queryString);
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
                                if (data.Tables["EVENT_DETAILS"].Rows.Count > 0)
                                {
                                    //    //queryString = " SELECT  x.GAMESTATUS, x.id, x.name rname ,''  RESULT,x.HOME_ID,x.GUEST_ID,x.hname, x1.gname,x.hnameCN,x1.gnameCN , x.hmnameCN,x.gmnameCN ,x.FHSD_19, x.HKJCDAYCODE,x.HKJCMATCHNO, x.HKJCHOSTNAME,x.HKJCGUESTNAME, x.CTIMESTAMP ,X.STATUS_NAME FROM( " +
                                    //    //"select   g.GAMESTATUS,e.id,e.NAME,e.HOME_ID,e.GUEST_ID,t.NAME hname, t.HKJC_NAME_CN hnameCN, e.START_DATE,e.CTIMESTAMP ,m.HKJCDAYCODE,m.HKJCMATCHNO,m.HKJCHOSTNAME,m.HKJCGUESTNAME ,m.HKJCHOSTNAME_CN hmnameCN, m.HKJCGUESTNAME_CN gmnameCN,b.FHSD_19,E.STATUS_NAME from events e " +
                                    //    //"inner join teams t on t.id = e.HOME_ID left JOIN EMATCHES M ON M.EMATCHID = e.ID  inner join GOALINFO g on g.EMATCHID=r.ID  left JOIN  EVENT_DETAILS b ON  e.ID = b.EVENTID  where e.id = '" + id + "' ) x " +
                                    //    //"INNER JOIN(SELECT * FROM(select e.id, e.NAME, e.HOME_ID, e.GUEST_ID, t.NAME gname, T.HKJC_NAME_CN gnameCN, e.START_DATE, e.CTIMESTAMP from events e " +
                                    //    //"inner join teams t  on t.id = e.GUEST_ID  where e.id = '" + id + "'  )) x1 on x.id = x1.id";
                                    //    queryString = "";
                                    //    Files.CicsWriteLog(DateTime.Now.ToString("HH:mm:ss") + " Sql2: " + queryString);
                                    //    cmd.CommandText = queryString;
                                    //    fda.SelectCommand = cmd;
                                    //    fda.Fill(data.Tables["EVENT_DETAILS"]);
                                    ////this.btnSave.Visible = false;
                                    ////this.btnSave.AutoUpdateAfterCallBack = true; 
                                    this.lbHomeid.Text = data.Tables["EVENT_DETAILS"].Rows[0]["HOME_ID"].ToString();
                                    this.lbGuestid.Text = data.Tables["EVENT_DETAILS"].Rows[0]["GUEST_ID"].ToString();
                                    this.chkToLive.Checked = data.Tables["EVENT_DETAILS"].Rows[0]["TOLIVE"].ToString() == "1" ? true : false;
                                }

                                eventDetails.DataSource = data.Tables[0].DefaultView;
                                eventDetails.DataBind();
                                this.eventDetails.UpdateAfterCallBack = true;
                                this.Page.Title = (data.Tables[0].Rows.Count > 0) ? data.Tables[0].Rows[0]["name"].ToString() : "" + "   Details";
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
            try
            {
                using (FbConnection connection = new FbConnection(AppFlag.ScoutsDBConn))
                {
                    string queryString = (Type == "Live") ?
                                "SELECT a.TEAMTYPE HG, a.EVENTID EMATCHID, a.SUBPARTICIPANT_NAME PLAYER, a.INCIDENT_NAME CTYPE, a.SUBPARTICIPANT_ID PARTICIPANTID, '' PLAYERCHI, t.NAME_CN  , cast(SUBSTRING(a.EVENT_TIME FROM 1 FOR 2) as integer)+1 ELAPSED,  a.CTIMESTAMP LASTTIME," +
                               "a.PARTICIPANT_ID TEAM_ID FROM INCIDENTS a left JOIN players T ON CAST(T.ID AS varchar(12)) = a.PARTICIPANT_ID  where a.EVENTID = '" + id + "'  and a.PARTICIPANT_ID !='' order by a.EVENT_TIME asc" :
                     "select i.hg, I.EMATCHID, i.player, i.CTYPE,  i.PARTICIPANTID, i.PLAYERCHI,t.IPLAYER_NO ,t.CPLAYER_NAME , i.ELAPSED,i.LASTTIME ,i.team_id,i.Status  MatchStatus ,i.INCIDENTS_ID, " +
                    "(select count(*) from MATCHDETAILS e where e.EMATCHID = '"+id+"' and  e.ctype='goal'and e.HG='H' AND E.ELAPSED<=I.ELAPSED ) resultH,  (select count(*) from MATCHDETAILS e where e.EMATCHID = '"+ id + "' and  e.ctype='goal'and e.HG='G' AND E.ELAPSED<=I.ELAPSED ) resultG " +
                    " from MATCHDETAILS I left JOIN  PLAYERS_INFO T  ON CAST(T.PLAYER_ID AS varchar(12)) = I.PARTICIPANTID AND I.team_id=T.TEAM_ID  and t.EVENT_ID=i.EMATCHID WHERE i.EMATCHID = '" + id + "' and (i.ctype='rcard' or i.ctype='ycard'  or i.ctype='goal' ) and (i.N !=0 or i.n is null) order by i.ELAPSED asc";// and I.PLAYER !='' order by i.ELAPSED asc";
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
                                    "r.FKG_18 \"Free kick goals\" , r.SW_27 \"Shots woodwork\", r.SB_28 \"Shots blocked\" , r.GS_29 \"Goalkeeper saves\", r.GK_30 \"Goal kicks\", r.TI_32 \"Throw-ins\" , r.SUBSTITUTIONS_31 \"Substitutions\", r.BPP_771 \"Possession(%)\",   r.GOALS_40, r.MP_34, r.OWN_GOALS_17, r.ADW_33, r.FORM_716, r.SKIN_718,    r.PS_639, r.PU_697, r.GOALS115_772, r.GOALS1630_773, r.GOALS3145_774,    r.GOALS4660_775, r.GOALS6175_776, r.GOALS7690_777, r.MPG_778, r.MPS_779," +
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
                                    DataRow tbDr;
                                    //foreach (DataRow dr in data.Tables[0].Rows)
                                    //{
                                    for (int i = 4; i < 27; i++)
                                    {
                                        tbDr = tb.NewRow();
                                        tbDr[0] = data.Tables[0].Columns[i].ColumnName;
                                        tbDr[1] = data.Tables[0].Rows[0][i];
                                        tbDr[2] = data.Tables[0].Rows[1][i];
                                        tb.Rows.Add(tbDr);
                                    }
                                    //}
                                    // totalDetails.DataSource = data.Tables[0].DefaultView; 
                                    totalDetails.DataSource = tb.DefaultView;
                                    totalDetails.Columns[0].HeaderText = "Type";
                                    totalDetails.Columns[1].HeaderText = data.Tables[0].Rows[0]["HKJC_NAME_CN"].ToString() + "(主)";
                                    totalDetails.Columns[2].HeaderText = data.Tables[0].Rows[1]["HKJC_NAME_CN"].ToString() + "(客)";
                                    totalDetails.DataBind();
                                    totalDetails.UpdateAfterCallBack = true;
                                    btnSave.Visible = false;
                                    btnSave.UpdateAfterCallBack = true;
                                    dgGoalInfo.UpdateAfterCallBack = true;
                                    if (data.Tables[0].Rows.Count == 2)
                                    {
                                        this.Page.Title = ((data.Tables[0].Rows[0]["TEAMTYPE"].ToString() == "H") ? data.Tables[0].Rows[0]["HKJC_NAME_CN"].ToString() + "-" + data.Tables[0].Rows[1]["HKJC_NAME_CN"].ToString() : data.Tables[0].Rows[1]["HKJC_NAME_CN"].ToString() + "-" + data.Tables[0].Rows[0]["HKJC_NAME_CN"].ToString()) + "  即場";
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
                            Files.CicsWriteLog(DateTime.Now.ToString("HH:mm:ss") + " SqlGoal: " + queryString);
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
                                        //btnSave.Visible = false;
                                        //btnSave.UpdateAfterCallBack = true;
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

        private void BindGoalInfo(string id)
        {
            try
            {
                using (FbConnection connection = new FbConnection(AppFlag.ScoutsDBConn))
                {
                    string queryString = "select i.hg, I.EMATCHID, i.player, i.CTYPE,  i.PARTICIPANTID, i.PLAYERCHI,t.IPLAYER_NO ,t.CPLAYER_NAME , i.ELAPSED,i.LASTTIME ,i.team_id,i.Status  MatchStatus ,i.INCIDENTS_ID," +
                        " (select count(*) from MATCHDETAILS e where e.EMATCHID = '"+id+"' and  e.ctype = 'goal'and e.HG = 'H' AND E.ELAPSED <= I.ELAPSED) resultH,  (select count(*) from MATCHDETAILS e where e.EMATCHID = '"+ id + "' and e.ctype = 'goal'and e.HG = 'G' AND E.ELAPSED <= I.ELAPSED ) resultG " +
                        " from MATCHDETAILS I left JOIN  PLAYERS_INFO T  ON CAST(T.PLAYER_ID AS varchar(12)) = I.PARTICIPANTID AND I.team_id=T.TEAM_ID  and t.EVENT_ID=i.EMATCHID WHERE i.EMATCHID = '" + id + "' and (i.ctype='rcard' or i.ctype='ycard'  or i.ctype='goal' ) and (i.N !=0 or i.n is null) order by i.ELAPSED asc";// and I.PLAYER !='' order by i.ELAPSED asc";
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
                Files.CicsWriteError(DateTime.Now.ToString("HH:mm:ss") + " BindGoalInfo(id),error: " + exp.ToString());
            }
        }

    }
}