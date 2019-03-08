namespace JC_SoccerWeb
{
    using Anthem;
    using JC_SoccerWeb.BLL;
    using JC_SoccerWeb.DAL;
    using JC_SoccerWeb.Common;
    using System;
    using System.Globalization;
    using System.Collections;
    using System.Data;
    using System.Drawing;
    using System.Text.RegularExpressions;
    using System.Web;
    using System.Web.UI.HtmlControls;
    using System.Web.UI.WebControls;
    using System.Net;
    using System.Text;
    using System.IO;
    using FirebirdSql.Data.FirebirdClient;

    public class Fixtures : System.Web.UI.Page
    {
        private IFormatProvider culture = new CultureInfo("zh-HK", true);
       // var culture = new System.Globalization.CultureInfo("zh-HK");
        protected Anthem.DataGrid dgRankDetails;
        protected System.Web.UI.WebControls.Label lbUser;
        protected Anthem.DropDownList dplLeague;
        protected Anthem.DropDownList dplLeagues;
        protected Anthem.DropDownList dplTeams;
        protected Anthem.Panel plRankDetails;
        protected Anthem.Label lbIHOSTORYRANK;
        protected Anthem.Label lbCUPDATEDATE;
        protected Anthem.Label lbIRANK;
        protected Anthem.Label lbCTEAM;
        protected Anthem.Label lbCLEAGUEALIAS;
        protected Anthem.TextBox txtlbIRANK;
        protected Anthem.TextBox txtIHOSTORYRANK;
        protected Anthem.TextBox txtFrom;
        protected Anthem.TextBox txtTo;
        protected Anthem.Label lbIHEADER_ID;
        protected Anthem.Button btnEdit;
        protected Anthem.Label lbMsg;
        protected Anthem.CheckBoxList cblIP; 

        protected void dgRankDetails_SelectedIndexChanged(object sender, EventArgs e)
        {
            string d = dgRankDetails.SelectedIndex.ToString();
         //   dgRankDetails.Items[dgRankDetails.SelectedIndex].Attributes.Add("onclick", "this.style.backgroundColor='red';");
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            lbMsg.Text = "";
            lbMsg.UpdateAfterCallBack = true;
            dgRankDetails.CurrentPageIndex = 0;
            BindEvents(dplLeague.SelectedValue);
           
        }

        private void dgSchedule_CancelCommand(object source, DataGridCommandEventArgs e)
        {
            this.dgRankDetails.EditItemIndex = -1;
            BindEvents(dplLeague.SelectedValue);
        }

        private void dgSchedule_DeleteCommand(object source, DataGridCommandEventArgs e)
        {
            string eventId = ((Anthem.Label)e.Item.FindControl("lbID")).Text;
            string dayCODE = ((Anthem.Label)e.Item.FindControl("lbHKJCDAYCODE")).Text;
            string matchNo = ((Anthem.Label)e.Item.FindControl("lbHKJCMATCHNO")).Text;
           // string start_date = ((Anthem.Label)e.Item.FindControl("lbSTART_DATE")).Text;
           // string sUpdateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff");
            string eventName = ((Anthem.Label)e.Item.FindControl("lbNAME")).Text;
            string lbHKJCHOSTNAME = ((Anthem.Label)e.Item.FindControl("lbHKJCHOSTNAME")).Text;
            string lbHKJCGUESTNAME = ((Anthem.Label)e.Item.FindControl("lbHKJCGUESTNAME")).Text;
            int id = 0;
            try
            {
                using (FbConnection connection = new FbConnection(AppFlag.ScoutsDBConn))
                {
                    connection.Open();
                    using (FbCommand cmd2 = new FbCommand())
                    {  
                        cmd2.CommandText = "SYNC_MANUAL_HKJCDATA_WEB_Cancel";
                        cmd2.CommandType = CommandType.StoredProcedure;
                        cmd2.Connection = connection;
                        cmd2.Parameters.Add("@EMATCHID", eventId);
                        cmd2.Parameters.Add("@HKJCDAYCODE", dayCODE);
                        cmd2.Parameters.Add("@HKJCMATCHNO", matchNo);
                       // cmd2.Parameters.Add("@CMATCHDATETIME1", Convert.ToDateTime(start_date).AddDays(-1).ToString("yyyy-MM-dd HH:mm:ss.fff"));
                     //   cmd2.Parameters.Add("@CMATCHDATETIME2", Convert.ToDateTime(start_date).AddDays(1).ToString("yyyy-MM-dd HH:mm:ss.fff"));
                        id = Convert.ToInt32(cmd2.ExecuteScalar());
                        Files.CicsWriteLog((id > 0 ? DateTime.Now.ToString("HH:mm:ss") + " [Success] " : DateTime.Now.ToString("HH:mm:ss") + " [Failure] ") + "Cancel Sync [" + eventId + "] EMATCHES[" + dayCODE + " " + matchNo + "] " + " (" + eventName+") "  +lbHKJCHOSTNAME + "/" + lbHKJCGUESTNAME);
                        this.dgRankDetails.EditItemIndex = -1;
                    }
                    connection.Close();
                }
                if (id > 0)
                {
                    this.lbMsg.Visible = true;
                    this.lbMsg.Text = "[Success] Cancel [" + eventId + "][" + dayCODE + " " + matchNo + "] " + " " + eventName;
                    this.lbMsg.Text = "[Success] Cancel [" + eventId + "][" + dayCODE + " " + matchNo + "] "  + lbHKJCHOSTNAME + "/" + lbHKJCGUESTNAME;

                    this.lbMsg.UpdateAfterCallBack = true;

                }
                BindEvents(dplLeague.SelectedValue);
            }
            catch (Exception exp)
            {
                Files.CicsWriteError(DateTime.Now.ToString("HH:mm:ss ") + "Cancel sync, error:" + exp.ToString());
            }
        }

        private void dgSchedule_EditCommand(object source, DataGridCommandEventArgs e)
        {
            this.dgRankDetails.EditItemIndex = e.Item.ItemIndex;
            BindEvents(dplLeague.SelectedValue);
          //  btnEdit.Text = "Get";
            lbMsg.Text = "";
           // btnEdit.UpdateAfterCallBack = true;
            lbMsg.UpdateAfterCallBack = true;
        }

        private void dgSchedule_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            // ((((e.Item.Cells).Items[13])).Controls).Items[3]
            /// for (int i = 0; i < this.dgRankDetails.Items.Count; i++)
            {
                //if (((Anthem.Label)this.dgRankDetails.Items[i].Cells[13].Controls[3]).Text != "")
                //{
                //    ((Anthem.DropDownList)this.dgRankDetails.Items[i].Cells[13].Controls[3]).SelectedValue = ((Anthem.Label)this.dgRankDetails.Items[i].Cells[13].Controls[3]).Text;
                //}
            }
            if (e.Item.ItemType == ListItemType.EditItem)
            {
                //if (((Anthem.Label)e.Item.Cells[13].Controls[3]).Text != "")
                //{
                //    ((Anthem.DropDownList)e.Item.Cells[13].Controls[1]).SelectedValue = ((Anthem.Label)e.Item.Cells[13].Controls[3]).Text;
                //}
                // ((System.Web.UI.WebControls.TextBox)e.Item.FindControl("txtMATCHNO")).Attributes.Add("onChange", "javascript:return CheckNum(this)");
                ((Anthem.DropDownList)e.Item.FindControl("dplDayCode")).SelectedValue = ((Anthem.Label)e.Item.FindControl("lbDAYCODE")).Text;
                System.Web.UI.WebControls.LinkButton lbtnEdit = (System.Web.UI.WebControls.LinkButton)e.Item.Cells[19].Controls[0];
                string sID = ((System.Web.UI.WebControls.Label)e.Item.FindControl("lbID")).Text;
                lbtnEdit.CausesValidation = true;
                //  lbtnEdit.Attributes["onclick"] = "return confirm('Are your sure？');";
                lbtnEdit.Attributes.Add("onclick", "javascript:OpenMapping(" + sID + ")");
            }
            else if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                e.Item.Attributes["style"] = "cursor:hand";
                System.Web.UI.WebControls.LinkButton link = (System.Web.UI.WebControls.LinkButton)e.Item.Cells[1].Controls[3]; 
                //e.Item.Attributes.Add("onclick", "this.style.backgroundColor='Gray';");
                //href: "javascript:__doPostBack('dgRankDetails$ctl04$btnSelect','')"
                e.Item.Attributes.Add("onclick", "javascript:return ClickLinkBtn(" + link.ClientID + ")");
                if (((System.Web.UI.WebControls.Label)e.Item.FindControl("lbHKJCDAYCODE")).Text == "")
                {
                    System.Web.UI.WebControls.LinkButton lbtnEdit = (System.Web.UI.WebControls.LinkButton)e.Item.Cells[20].Controls[0];
                    lbtnEdit.Visible = false;
                }
            }
        }

        private void dgSchedule_ItemCreated(object sender, DataGridItemEventArgs e)
        {
            try
            {
                if (e.Item.ItemType == ListItemType.EditItem)
                {
                  //   ((System.Web.UI.WebControls.Button)e.Item.FindControl("btnAlert")).Attributes.Add("onclick", "return confirm('Are you sure?');");

                    // string df = "";
                    //  ((Anthem.DropDownList)e.Item.FindControl("dplDayCode")).SelectedValue = ((Anthem.Label)e.Item.FindControl("lbDAYCODE")).Text;
                    ((System.Web.UI.WebControls.TextBox)e.Item.FindControl("txtMATCHNO")).Attributes.Add("onChange", "javascript:return CheckNum(this)");
                    //((System.Web.UI.WebControls.TextBox)e.Item.FindControl("txtISCORE")).Attributes.Add("onChange", "javascript:return CheckNum(this)");
                }
                else if (e.Item.ItemType == ListItemType.Item)
                { 
                    //  ((System.Web.UI.WebControls.TextBox)e.Item.FindControl("txtMATCHNO")).Attributes.Add("onChange", "javascript:return CheckNum(this)");
                    //((System.Web.UI.WebControls.TextBox)e.Item.FindControl("txtIHOSTORYRANK2")).Attributes.Add("onChange", "javascript:return CheckNum(this)");
                    //((System.Web.UI.WebControls.TextBox)e.Item.FindControl("txtISCORE2")).Attributes.Add("onChange", "javascript:return CheckNum(this)");
                }
            }
            catch (Exception exception)
            {
                string str = exception.ToString();
            }
        }

        private void dgSchedule_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
        {
            dgRankDetails.SelectedIndex = -1;
            this.dgRankDetails.CurrentPageIndex = e.NewPageIndex;
            DataSet data = (DataSet)this.Session["frankData"];
            this.dgRankDetails.DataSource = data.Tables["events"].DefaultView;
            this.dgRankDetails.PageSize = AppFlag.iPageSize;
            this.dgRankDetails.DataBind();
            this.dgRankDetails.UpdateAfterCallBack = true;
           // btnEdit.Text = "Get";
           // btnEdit.UpdateAfterCallBack = true;
            lbMsg.Text = "";
            lbMsg.UpdateAfterCallBack = true;
        }

        private void dgSchedule_UpdateCommand(object source, DataGridCommandEventArgs e)
        {
            string eventId = ((Anthem.Label)e.Item.FindControl("lbID")).Text;
            string dayCODE = ((Anthem.DropDownList)e.Item.FindControl("dplDayCode")).SelectedValue;
            string matchNo= ((Anthem.TextBox)e.Item.FindControl("txtMATCHNO")).Text;
            string start_date= ((Anthem.Label)e.Item.FindControl("lbSTART_DATE")).Text;
            string sUpdateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff");
            string eventName= ((Anthem.Label)e.Item.FindControl("lbNAME")).Text;
            //string oldDayCODE = ((Anthem.Label)e.Item.FindControl("lbDAYCODE")).Text;
            //string oldMatchNo = ((Anthem.Label)e.Item.FindControl("lbMATCHNO")).Text;
            int id = 0;
            try
            {
                using (FbConnection connection = new FbConnection(AppFlag.ScoutsDBConn))
                {
                    connection.Open();
                    using (FbCommand cmd2 = new FbCommand())
                    {  //maybe return booked or no
                        cmd2.CommandText = "WEB_SYNC_MANUAL_HKJCDATA";
                        cmd2.CommandType = CommandType.StoredProcedure;
                        cmd2.Connection = connection;
                        cmd2.Parameters.Add("@EMATCHID", eventId);
                        //cmd2.Parameters.Add("@OLDHKJCDAYCODE", oldDayCODE);
                        //cmd2.Parameters.Add("@OLDHKJCMATCHNO", oldMatchNo);
                        cmd2.Parameters.Add("@HKJCDAYCODE", dayCODE);
                        cmd2.Parameters.Add("@HKJCMATCHNO", matchNo);
                        cmd2.Parameters.Add("@CMATCHDATETIME1", Convert.ToDateTime(start_date).AddHours (-AppFlag.MarginOfDeviation).ToString("yyyy-MM-dd HH:mm:ss.fff"));
                        cmd2.Parameters.Add("@CMATCHDATETIME2", Convert.ToDateTime(start_date).AddHours(AppFlag.MarginOfDeviation).ToString("yyyy-MM-dd HH:mm:ss.fff"));
                          id = Convert.ToInt32(cmd2.ExecuteScalar());
                        Files.CicsWriteLog((id > 0 ? DateTime.Now.ToString("HH:mm:ss") + " [Success] " : DateTime.Now.ToString("HH:mm:ss") + " [Failure] ") + "Sync [" + eventId + "] EMATCHES[" + dayCODE + " " + matchNo + "] " + " " + eventName);
                        this.dgRankDetails.EditItemIndex = -1;
                        if(id>0&&AppFlag.AutoMapping)
                        {
                            TeamMapping(eventId, dayCODE, matchNo, start_date, eventName);
                        }
                    }
                    connection.Close();
                }
                if(id>0)
                {
                    this.lbMsg.Visible = true;
                    // this.lbMsg.Text = "[Success] Sync "  +dayCODE + " " + matchNo;
                    this.lbMsg.Text = "[Success] Sync [" + eventId + "][" + dayCODE + " " + matchNo + "] " + " " + eventName;
                    this.lbMsg.UpdateAfterCallBack = true; 
                }
                else
                {
                    this.lbMsg.Visible = true;
                    this.lbMsg.Text = "[Failure] Sync [" + eventId + "][" + dayCODE + " " + matchNo + "] " + " " + eventName;
                    this.lbMsg.UpdateAfterCallBack = true;
                }
                BindEvents(dplLeague.SelectedValue);
            }
            catch (Exception exp)
            {
                Files.CicsWriteError(DateTime.Now.ToString("HH:mm:ss ") + "Sync, error:" + exp.ToString());
            } 
        }

        private void dplLeague_SelectedIndexChanged(object sender, EventArgs e)
        {
            dgRankDetails.CurrentPageIndex = 0;
            BindEvents(dplTeams.SelectedValue);
            btnEdit.Text = "Get";
            btnEdit.UpdateAfterCallBack = true;
            lbMsg.Text = "";
            lbMsg.UpdateAfterCallBack = true;
        }
        private void dplLeagues_SelectedIndexChanged(object sender, EventArgs e)
        {
            dgRankDetails.CurrentPageIndex = 0;
            BindTeams();
            BindEvents(dplTeams.SelectedValue);
        }
        private void dplTeams_SelectedIndexChanged(object sender, EventArgs e)
        {
            dgRankDetails.CurrentPageIndex = 0;
            BindEvents(dplTeams.SelectedValue);
        }
        private void InitializeComponent()
        {
            this.dgRankDetails.DeleteCommand += new DataGridCommandEventHandler(this.dgSchedule_DeleteCommand);
            this.dgRankDetails.PageIndexChanged += new DataGridPageChangedEventHandler(this.dgSchedule_PageIndexChanged);
            this.dgRankDetails.UpdateCommand += new DataGridCommandEventHandler(this.dgSchedule_UpdateCommand);
            this.dgRankDetails.CancelCommand += new DataGridCommandEventHandler(this.dgSchedule_CancelCommand);
            this.dgRankDetails.ItemDataBound += new DataGridItemEventHandler(this.dgSchedule_ItemDataBound);
            this.dgRankDetails.EditCommand += new DataGridCommandEventHandler(this.dgSchedule_EditCommand);
            this.dplLeague.SelectedIndexChanged += new EventHandler(this.dplLeague_SelectedIndexChanged);
            this.dplLeagues.SelectedIndexChanged += new EventHandler(this.dplLeagues_SelectedIndexChanged);
            this.dplTeams.SelectedIndexChanged += new EventHandler(this.dplTeams_SelectedIndexChanged);
            this.dgRankDetails.ItemCreated += new DataGridItemEventHandler(this.dgSchedule_ItemCreated);
            this.btnEdit.Click += new EventHandler(this.btnEdit_Click);

            base.Load += new EventHandler(this.Page_Load);
        }

        protected override void OnInit(EventArgs e)
        {
            this.InitializeComponent();
            base.OnInit(e);
        }

        private void Page_Load(object sender, EventArgs e)
        {
            if (!base.Request.IsAuthenticated)
            {
                base.Response.Redirect("Default.aspx", false);
            }
            else if (base.Request.IsAuthenticated && !this.Page.IsPostBack)
            {
                //int index1 = AppFlag.HkjcDBConn.IndexOf("DataSource=");
                //int index2 = AppFlag.HkjcDBConn.IndexOf(";", index1);
                //string ip1 = "";
                //if (index1 > 0)
                //{
                //     ip1 = AppFlag.HkjcDBConn.Substring(index1 + "DataSource=".Length, index2 - index1 - +"DataSource=".Length);
                //    this.cblIP.Items.Add(ip1);
                //    this.cblIP.Items[0].Selected = true;
                //}
                //index1 = AppFlag.ScoutsDBConn.IndexOf("DataSource=");
                //if (index1 > 0)
                //{
                //    index2 = AppFlag.ScoutsDBConn.IndexOf(";", index1);
                //    ip1 = AppFlag.ScoutsDBConn.Substring(index1 + "DataSource=".Length, index2 - index1 - +"DataSource=".Length);
                //    this.cblIP.Items.Add(ip1);
                //    this.cblIP.Items[1].Selected = true;
                //}
                this.txtFrom.Text =DateTime.Now.ToString("yyyy-MM-dd", culture);
                this.txtTo.Text = DateTime.Now.ToString("yyyy-MM-dd", culture);
                this.lbUser.Text = this.Context.User.Identity.Name;

                BindLeague();
                BindLeagues();
                BindTeams();
                BindEvents(dplTeams.SelectedValue); 
            }
        }

        private void TeamMapping(string eventID,string dayCode,string MatchNo,string startDate ,string eventName)
        {
            try
            {
                using (FbConnection connection = new FbConnection(AppFlag.ScoutsDBConn))
                {
                    connection.Open();
                    using (FbCommand cmd2 = new FbCommand())
                    {
                        cmd2.CommandText = "WEB_SYNC_MANUAL_HKJCTEAM";
                        cmd2.CommandType = CommandType.StoredProcedure;
                        cmd2.Connection = connection;
                        cmd2.Parameters.Add("@EMATCHID", eventID);
                        cmd2.Parameters.Add("@HKJCDAYCODE", dayCode);
                        cmd2.Parameters.Add("@HKJCMATCHNO", MatchNo);
                        cmd2.Parameters.Add("@CMATCHDATETIME1", Convert.ToDateTime(startDate).AddHours(-AppFlag.MarginOfDeviation).ToString("yyyy-MM-dd HH:mm:ss.fff"));
                        cmd2.Parameters.Add("@CMATCHDATETIME2", Convert.ToDateTime(startDate).AddHours(AppFlag.MarginOfDeviation).ToString("yyyy-MM-dd HH:mm:ss.fff"));
                        int id = Convert.ToInt32(cmd2.ExecuteScalar());
                        //  Files.CicsWriteLog((id > 0 ? DateTime.Now.ToString("HH:mm:ss") + " [Success] " : DateTime.Now.ToString("HH:mm:ss") + " [Failure] ") + "Sync [" + eventName + "] on Teams");
                        Files.CicsWriteLog((id > 0 ? DateTime.Now.ToString("HH:mm:ss") + " [Success] " : DateTime.Now.ToString("HH:mm:ss") + "") + " Sync [" + eventName + "] on Teams");
                    }
                    connection.Close();
                }
            }
            catch (Exception exp)
            {
                string exps = exp.ToString();
                Files.CicsWriteError(DateTime.Now.ToString("HH:mm:ss") + "   TeamMapping()  " + exps);
            }
        }
        private void BindLeagues()
        {
            try
            {
                using (FbConnection connection = new FbConnection(AppFlag.ScoutsDBConn))
                {
                    connection.Open();

                    string queryString = "SELECT r.ID,  L.LEAGUE_CHI_NAME FROM COMPETITIONS r INNER JOIN  LEAGUE_INFO L ON R.ALIAS=L.CLEAGUE_ALIAS_NAME where " +
                        " r.ALIAS='意甲' OR r.ALIAS='英超' OR   r.ALIAS='法甲' OR  r.ALIAS='德甲' OR  r.ALIAS='蘇超' OR  r.ALIAS='西甲'" +
                        " OR  r.ALIAS='荷甲' OR  r.ALIAS='日聯' OR  r.ALIAS='澳A' OR  r.ALIAS='歐冠' ORDER BY  R.ID   ";
                    using (FbDataAdapter fda = new FbDataAdapter(queryString, connection))
                    {
                        using (FbDataAdapter fda2 = new FbDataAdapter())
                        {
                            using (DataSet data = new DataSet())
                            {
                                data.Tables.Add(new DataTable("COMPETITIONS"));
                                fda.Fill(data.Tables["COMPETITIONS"]);
                                dplLeagues.DataSource = data.Tables[0].DefaultView;
                                dplLeagues.DataTextField = "LEAGUE_CHI_NAME";
                                dplLeagues.DataValueField = "id";
                                dplLeagues.DataBind();
                                //  dplLeagues.Items.Insert(0, new ListItem("All", "-1"));
                                if (dplLeagues.Items.Count > 0) dplLeagues.SelectedIndex = 0;
                                this.dplLeagues.UpdateAfterCallBack = true;
                            }
                        }
                    }

                    connection.Close();
                }
            }
            catch (Exception exp)
            {
                string exps = exp.ToString();
                Files.CicsWriteError(DateTime.Now.ToString("HH:mm:ss") + "   BindLeagues()  " + exps);
            }
        }

        private void BindLeague()
        {
            try
            {
                using (FbConnection connection = new FbConnection(AppFlag.ScoutsDBConn))
                {
                    connection.Open();
                    string queryString = "SELECT a.id,a.name FROM  statuses a;";
                    using (FbCommand cmd = new FbCommand(queryString))
                    {
                        using (FbDataAdapter fda = new FbDataAdapter(queryString, connection))
                        {
                            //connection.Open();
                            // cmd.Connection =  ;
                            //fda.SelectCommand = cmd;
                            using (DataSet data = new DataSet())
                            {
                                data.Tables.Add(new DataTable("statuses"));
                                fda.Fill(data.Tables["statuses"]);
                                dplLeague.DataSource = data.Tables[0].DefaultView;
                                dplLeague.DataTextField = "name";
                                dplLeague.DataValueField = "id";
                                dplLeague.DataBind();
                                dplLeague.Items.Insert(0, new ListItem("All", "-1"));
                                this.dplLeague.UpdateAfterCallBack = true;
                            }
                        }
                    } 
                    connection.Close();
                }
            }
            catch (Exception exp)
            {
                string exps = exp.ToString();
                Files.CicsWriteError(DateTime.Now.ToString("HH:mm:ss") + "   BindLeague()  " + exps);
            }
        }
        private void BindTeams()
        {
            try
            {
                using (FbConnection connection = new FbConnection(AppFlag.ScoutsDBConn))
                {
                    connection.Open();
                    //  string  queryString = "SELECT t.ID,  t.HKJC_NAME_CN  FROM teams t   where t.SEASON_ID=(select first 1 s.id from SEASONS s where s.COMPETITION_ID=" + dplLeagues .SelectedValue+ " order by s.SYEAR desc )";
                    //  string queryString = "SELECT t.ID,  t.HKJC_NAME_CN  FROM teams t   where t.SEASON_ID="+ dplLeagues.SelectedValue;
                    string queryString = " select distinct   t.id,t.HKJC_NAME_CN from events e inner join  teams t on e.HOME_ID = t.id or e.GUEST_ID = t.id where e.COMPETITION_ID =" + dplLeagues.SelectedValue;
                    using (FbDataAdapter fda = new FbDataAdapter(queryString, connection))
                    {
                        using (FbDataAdapter fda3 = new FbDataAdapter())
                        {
                            using (DataSet data = new DataSet())
                            {
                                data.Tables.Add(new DataTable("teams"));
                                fda.Fill(data.Tables["teams"]);
                                dplTeams.DataSource = data.Tables[0].DefaultView;
                                dplTeams.DataTextField = "HKJC_NAME_CN";
                                dplTeams.DataValueField = "id";
                                dplTeams.DataBind();
                                // dplTeams.Items.Insert(0, new ListItem("All", "-1"));
                                if(dplTeams.Items.Count>0) dplTeams.SelectedIndex = 0;
                                this.dplTeams.UpdateAfterCallBack = true;
                            }
                        }
                    } 
                    connection.Close();
                }
            }
            catch (Exception exp)
            {
                string exps = exp.ToString();
                Files.CicsWriteError(DateTime.Now.ToString("HH:mm:ss") + "   BindStatus()  " + exps);
            }
        }

        private void BindEvents(string id)
        {
            //select R.ID ,R.NAME  ,R.START_DATE,G.H_GOAL,G.G_GOAL,G.H_YELLOW,G.G_YELLOW,G.H_RED,G.G_RED,E.HKJCHOSTNAME,E.HKJCGUESTNAME,E.HKJCDAYCODE,E.HKJCMATCHNO,r.CTIMESTAMP from events r  LEFT join goalinfo g  on r.id = g.EMATCHID   LEFT join EMATCHES e on e.EMATCHID = r.id
            //where r.START_DATE >= '22.11.2018, 00:00:00.000' and r.START_DATE <= '22.11.2018, 23:59:59.000'   order by r.START_DATE ASC
            try
            {
                using (FbConnection connection = new FbConnection(AppFlag.ScoutsDBConn))
                {//(r.caction !='delete' or r.caction is null) and 
                    string queryString = "select R.ID ,R.NAME ,C.ALIAS,(select hkjc_name_cn from teams where id=home_id)||'-'||(select hkjc_name_cn from teams where id=guest_id) cNAME ,r.STATUS_NAME ,R.START_DATE,G.H_GOAL,G.G_GOAL,G.H_YELLOW,G.G_YELLOW,G.H_RED,G.G_RED,E.HKJCHOSTNAME,E.HKJCGUESTNAME,E.HKJCDAYCODE,E.HKJCMATCHNO,r.CTIMESTAMP, r.booked,e.CMATCHDATETIME,t.NAME, t.MAPPING_STATUS ,e.HKJCHOSTNAME_CN,e.HKJCGUESTNAME_CN "
                       + "from events r  LEFT join goalinfo g  on r.id = g.EMATCHID   LEFT join EMATCHES e on e.EMATCHID = r.id  inner join  teams t on t.id =r.HOME_ID  inner join  COMPETITIONS C on C.id =R.COMPETITION_ID"
                       + " where (r.caction !='delete' or r.caction is null) "
                     // "and  r.START_DATE >= '" + txtFrom.Text.Trim() + ", 00:00:00.000' and r.START_DATE <= '" + txtTo.Text.Trim() + ", 23:59:59.000'"
                     + "and ( r.HOME_ID = " + id + "  or r.GUEST_ID = " + id + ")"// and r.COMPETITION_ID= "  +dplLeagues.SelectedValue
                    //+ (dplLeague.SelectedValue == "-1"?"": " and STATUS_ID ="+dplLeague.SelectedValue) +  " order by r.START_DATE ASC  ";
                   +  " order by r.START_DATE ASC  ";
                    using (FbCommand cmd = new FbCommand(queryString))
                    {
                        using (FbDataAdapter fda = new FbDataAdapter())
                        {
                            connection.Open();
                            cmd.Connection = connection;
                            fda.SelectCommand = cmd;
                            using (DataSet data = new DataSet())
                            {
                                data.Tables.Add(new DataTable("events"));
                                fda.Fill(data.Tables["events"]);
                                dgRankDetails.PageSize = AppFlag.iPageSize;
                                dgRankDetails.DataSource = data.Tables[0].DefaultView;
                                dgRankDetails.DataBind();
                                dgRankDetails.UpdateAfterCallBack = true;
                                Session["frankData"] = data;
                            }
                        }
                    }
                    connection.Close();
                }
            }
            catch (Exception exp)
            {
                string exps = exp.ToString();
                Files.CicsWriteError(DateTime.Now.ToString("HH:mm:ss") + "   BindEvents()  " + exps);
            }

        }

    }
}

