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

    public class Matches : CommonPage
    {
        private IFormatProvider culture = new CultureInfo("zh-HK", true);
        // var culture = new System.Globalization.CultureInfo("zh-HK");
        protected Anthem.DataGrid dgRankDetails;
        protected System.Web.UI.WebControls.Label lbUser;
        protected Anthem.DropDownList dplLeague;
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
        protected Anthem.RadioButtonList cbDay;

        protected void cbDay_SelectedIndexChanged(object sender, EventArgs e)
        {
            dgRankDetails.CurrentPageIndex = 0;
            BindMatchesByDateDay(dplLeague.SelectedValue);
            //try
            //{
            //    dgRankDetails.CurrentPageIndex = 0;
            //    DataSet data = (DataSet)this.Session["matches"];
            //    DataTable table = data.Tables[0];
            //    if (table.Rows.Count > 0)
            //    {
            //        table.DefaultView.RowFilter = (cbDay.SelectedItem.Text == "All" ? "" : "HKJCDAYCODE='" + cbDay.SelectedItem.Text + "' ");
            //        table.DefaultView.Sort = cbDay.SelectedItem.Text == "All" ? " CMATCHDATETIME asc " : " HKJCMATCHNO asc";
            //        table = table.DefaultView.ToTable();
            //        dgRankDetails.DataSource = data.Tables[0].DefaultView;
            //        dgRankDetails.DataBind();

            //        dgRankDetails.UpdateAfterCallBack = true;
            //    }
            //}
            //catch (Exception exception)
            //{
            //    string str = "cbDay_SelectedIndexChanged(),error:" + exception.ToString();
            //}
        }

        private void dgSchedule_CancelCommand(object source, DataGridCommandEventArgs e)
        { 
        }

        private void dgSchedule_EditCommand(object source, DataGridCommandEventArgs e)
        { 
        } 

        private void dgSchedule_ItemCreated(object sender, DataGridItemEventArgs e)
        {
            //try
            //{
            //    if (e.Item.ItemType == ListItemType.EditItem)
            //    {
            //        ((System.Web.UI.WebControls.TextBox)e.Item.FindControl("txtMATCHNO")).Attributes.Add("onChange", "javascript:return CheckNum(this)");
            //    }
            //    else if (e.Item.ItemType == ListItemType.Item)
            //    {
            //        ((System.Web.UI.WebControls.TextBox)e.Item.FindControl("txtMATCHNO")).Attributes.Add("onChange", "javascript:return CheckNum(this)");
            //    }
            //}
            //catch (Exception exception)
            //{
            //    string str = exception.ToString();
            //}
        }

        private void dgSchedule_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
        {
            try
            {
                this.dgRankDetails.CurrentPageIndex = e.NewPageIndex;
                DataSet data = (DataSet)this.Session["matches"];
                this.dgRankDetails.DataSource = data.Tables["SocoutMatch"].DefaultView;
                this.dgRankDetails.PageSize = AppFlag.iPageSize;
                this.dgRankDetails.DataBind();
            }
            catch (Exception exp)
            {
                this.dgRankDetails.CurrentPageIndex = 0;
                string exps = exp.ToString();
                Files.CicsWriteError(DateTime.Now.ToString("HH:mm:ss") + "   dgSchedule_PageIndexChanged()  " + exps);
            }
            this.dgRankDetails.UpdateAfterCallBack = true;
            lbMsg.Text = "";
            lbMsg.UpdateAfterCallBack = true;
        }
        private void dgSchedule_UpdateCommand(object source, DataGridCommandEventArgs e)
        {

        }

        private void dplLeague_SelectedIndexChanged(object sender, EventArgs e)
        {
            dgRankDetails.CurrentPageIndex = 0;
            BindMatchesByDate(dplLeague.SelectedValue); 
            lbMsg.Text = "";
            lbMsg.UpdateAfterCallBack = true;
            this.cbDay.SelectedValue = "-1";
            this.cbDay.AutoUpdateAfterCallBack = true;
        }
        private void btnEdit_Click(object sender, EventArgs e)
        {
            dgRankDetails.CurrentPageIndex = 0;
            this.cbDay.SelectedValue = "-1";
            this.cbDay.AutoUpdateAfterCallBack = true;
            BindMatchesByDate(dplLeague.SelectedValue);
        }

        private void InitializeComponent()
        {
            this.dgRankDetails.PageIndexChanged += new DataGridPageChangedEventHandler(this.dgSchedule_PageIndexChanged);
            this.dgRankDetails.UpdateCommand += new DataGridCommandEventHandler(this.dgSchedule_UpdateCommand);
            this.dgRankDetails.CancelCommand += new DataGridCommandEventHandler(this.dgSchedule_CancelCommand);
            this.dgRankDetails.EditCommand += new DataGridCommandEventHandler(this.dgSchedule_EditCommand);
            this.dplLeague.SelectedIndexChanged += new EventHandler(this.dplLeague_SelectedIndexChanged);
          //  this.dgRankDetails.ItemCreated += new DataGridItemEventHandler(this.dgSchedule_ItemCreated);
            this.cbDay.SelectedIndexChanged += cbDay_SelectedIndexChanged;
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
                this.txtFrom.Text = DateTime.Now.ToString("yyyy-MM-dd", culture);
                this.txtTo.Text = DateTime.Now.ToString("yyyy-MM-dd", culture);
                this.lbUser.Text = this.Context.User.Identity.Name;
                //   BindStatuses();
                BindMatches(dplLeague.SelectedValue);

            }
        }

        private void BindStatuses()
        {
            try
            {
                using (FbConnection connection = new FbConnection(AppFlag.ScoutsDBConn))
                {
                    string queryString = "SELECT a.id,a.name FROM  statuses a;";
                    using (FbCommand cmd = new FbCommand(queryString))
                    {
                        using (FbDataAdapter fda = new FbDataAdapter())
                        {
                            connection.Open();
                            cmd.Connection = connection;
                            fda.SelectCommand = cmd;
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
                Files.CicsWriteError(DateTime.Now.ToString("HH:mm:ss") + "   BindStatuses()  " + exps);
            }
        }

        private void BindMatches(string id)
        {
            //select R.ID ,R.NAME  ,R.START_DATE,G.H_GOAL,G.G_GOAL,G.H_YELLOW,G.G_YELLOW,G.H_RED,G.G_RED,E.HKJCHOSTNAME,E.HKJCGUESTNAME,E.HKJCDAYCODE,E.HKJCMATCHNO,r.CTIMESTAMP from events r  LEFT join goalinfo g  on r.id = g.EMATCHID   LEFT join EMATCHES e on e.EMATCHID = r.id
            //where r.START_DATE >= '22.11.2018, 00:00:00.000' and r.START_DATE <= '22.11.2018, 23:59:59.000'   order by r.START_DATE ASC
        //    if(id=="All")
            try
            {
                DateTime maxTime = DateTime.MinValue;
                DateTime minTime = DateTime.MaxValue;
                using (FbConnection connection = new FbConnection(AppFlag.HkjcDBConn))
                {
                    string queryString = "SELECT e.CMATCHDATETIME FROM matchlist e   order by e.CMATCHDATETIME desc";
                    using (FbCommand cmd = new FbCommand(queryString, connection))
                    {
                        using (FbCommandBuilder fcb = new FbCommandBuilder())
                        {
                            using (FbDataAdapter fda = new FbDataAdapter())
                            {
                                fda.SelectCommand = cmd;
                                fcb.DataAdapter = fda;
                                using (DataSet data = new DataSet())
                                {
                                    data.Tables.Add(new DataTable("HKjcMatch"));
                                    fda.Fill(data.Tables["HKjcMatch"]);
                                    if (data.Tables["HKjcMatch"].Rows.Count > 0)
                                    {
                                        maxTime = Convert.ToDateTime(data.Tables[0].Rows[0]["CMATCHDATETIME"]);
                                        minTime = Convert.ToDateTime(data.Tables[0].Rows[data.Tables[0].Rows.Count - 1]["CMATCHDATETIME"]);
                                        this.txtFrom.Text = minTime.ToString("yyyy-MM-dd", culture);
                                        this.txtTo.Text = maxTime.ToString("yyyy-MM-dd", culture);
                                    }
                                }
                            }
                        }
                    }
                    connection.Close();
                }

                using (FbConnection connection = new FbConnection(AppFlag.ScoutsDBConn))
                {
                    //string queryString = "SELECT e.* FROM EMATCHES e where  '" + minTime.ToString("yyyy-MM-dd HH:mm:ss.fff", null) + "'<=  e.CMATCHDATETIME and  e.CMATCHDATETIME <='" + maxTime.ToString("yyyy-MM-dd HH:mm:ss.fff", null) + "' order by  e.EMATCHID desc ";
                    string queryString = "select  V.ID ,V.NAME  ,V.START_DATE,G.H_GOAL,G.G_GOAL,G.H_YELLOW,G.G_YELLOW,G.H_RED,G.G_RED,E.HKJCHOSTNAME,E.HKJCGUESTNAME,E.HKJCDAYCODE,E.HKJCMATCHNO,E.STATUS,V.CTIMESTAMP, E.CMATCHDATETIME,e.MAPPINGSTATUS  from EMATCHES E " +
                         " LEFT JOIN GOALINFO G ON E.EMATCHID = G.EMATCHID LEFT JOIN EVENTS V ON E.EMATCHID = V.ID " +
                           " WHERE E.CMATCHDATETIME >= '" + minTime.ToString("yyyy-MM-dd HH:mm:ss.fff", null) + "' and E.CMATCHDATETIME <= '" + maxTime.ToString("yyyy-MM-dd HH:mm:ss.fff", null) + "'" + (id.ToString() == "-1" ? "" : " and STATUS = '" + dplLeague.SelectedValue+"'") + "   order by  E.CMATCHDATETIME asc";
                    using (FbCommand cmd = new FbCommand(queryString, connection))
                    {
                        using (FbDataAdapter fda = new FbDataAdapter())
                        {
                            fda.SelectCommand = cmd;
                            using (DataSet data = new DataSet())
                            {
                                data.Tables.Add(new DataTable("SocoutMatch"));
                                fda.Fill(data.Tables["SocoutMatch"]);
                                dgRankDetails.PageSize = AppFlag.iPageSize;
                                dgRankDetails.DataSource = data.Tables[0].DefaultView;
                                dgRankDetails.DataBind();
                                dgRankDetails.UpdateAfterCallBack = true;
                                Session["matches"] = data;
                            }
                        }
                    }
                    connection.Close();
                }
            }
            catch (Exception exp)
            {
                string exps = exp.ToString();
                Files.CicsWriteError(DateTime.Now.ToString("HH:mm:ss") + "   BindMatches()  " + exps);
            }
        }


        private void BindMatchesByDate(string id)
        {
            try { 
                using (FbConnection connection = new FbConnection(AppFlag.ScoutsDBConn))
                {
                    //string queryString = "SELECT e.* FROM EMATCHES e where  '" + minTime.ToString("yyyy-MM-dd HH:mm:ss.fff", null) + "'<=  e.CMATCHDATETIME and  e.CMATCHDATETIME <='" + maxTime.ToString("yyyy-MM-dd HH:mm:ss.fff", null) + "' order by  e.EMATCHID desc ";
                    string queryString = "select  V.ID ,V.NAME  ,V.START_DATE,G.H_GOAL,G.G_GOAL,G.H_YELLOW,G.G_YELLOW,G.H_RED,G.G_RED,E.HKJCHOSTNAME,E.HKJCGUESTNAME,E.HKJCDAYCODE,E.HKJCMATCHNO,E.STATUS,V.CTIMESTAMP, E.CMATCHDATETIME,e.MAPPINGSTATUS  from EMATCHES E " +
                         " LEFT JOIN GOALINFO G ON E.EMATCHID = G.EMATCHID LEFT JOIN EVENTS V ON E.EMATCHID = V.ID " +
                           " WHERE E.CMATCHDATETIME >= '" + this.txtFrom.Text.Trim ()+ ", 00:00:00.000" + "' and E.CMATCHDATETIME <= '" + this.txtTo.Text.Trim() + ", 23:59:59.000" + "" + "'" + (id.ToString() == "-1" ? "" : " and E.STATUS = '" + dplLeague.SelectedValue+"'") + "   order by  E.CMATCHDATETIME asc";
                    using (FbCommand cmd = new FbCommand(queryString, connection))
                    {
                        using (FbDataAdapter fda = new FbDataAdapter())
                        {
                            fda.SelectCommand = cmd;
                            using (DataSet data = new DataSet())
                            {
                                data.Tables.Add(new DataTable("SocoutMatch"));
                                fda.Fill(data.Tables["SocoutMatch"]);
                                dgRankDetails.PageSize = AppFlag.iPageSize;
                                dgRankDetails.DataSource = data.Tables[0].DefaultView;
                                dgRankDetails.DataBind();
                                dgRankDetails.UpdateAfterCallBack = true;
                                Session["matches"] = data;
                            }
                        }
                    }
                    connection.Close();
                }
            }
            catch (Exception exp)
            {
                string exps = exp.ToString();
                Files.CicsWriteError(DateTime.Now.ToString("HH:mm:ss") + "   BindMatchesByDate()  " + exps);
            }
        }

        private void BindMatchesByDateDay(string id)
        {
            try
            {
                using (FbConnection connection = new FbConnection(AppFlag.ScoutsDBConn))
                {
                    //string queryString = "SELECT e.* FROM EMATCHES e where  '" + minTime.ToString("yyyy-MM-dd HH:mm:ss.fff", null) + "'<=  e.CMATCHDATETIME and  e.CMATCHDATETIME <='" + maxTime.ToString("yyyy-MM-dd HH:mm:ss.fff", null) + "' order by  e.EMATCHID desc ";
                    string queryString = "select  V.ID ,V.NAME  ,V.START_DATE,G.H_GOAL,G.G_GOAL,G.H_YELLOW,G.G_YELLOW,G.H_RED,G.G_RED,E.HKJCHOSTNAME,E.HKJCGUESTNAME,E.HKJCDAYCODE,E.HKJCMATCHNO,E.STATUS,V.CTIMESTAMP, E.CMATCHDATETIME,e.MAPPINGSTATUS  from EMATCHES E " +
                         " LEFT JOIN GOALINFO G ON E.EMATCHID = G.EMATCHID LEFT JOIN EVENTS V ON E.EMATCHID = V.ID " +
                           " WHERE E.CMATCHDATETIME >= '" + this.txtFrom.Text.Trim() + ", 00:00:00.000" + "' and E.CMATCHDATETIME <= '" + this.txtTo.Text.Trim() + ", 23:59:59.000" + "" + "'" + (id.ToString() == "-1" ? "" : " and E.STATUS = '" + dplLeague.SelectedValue+"'") + (cbDay.Text == "-1"?"": " AND E.HKJCDAYCODE='" + cbDay.Text + "'")+ " order by  E.HKJCMATCHNO asc";
                    using (FbCommand cmd = new FbCommand(queryString, connection))
                    {
                        using (FbDataAdapter fda = new FbDataAdapter())
                        {
                            fda.SelectCommand = cmd;
                            using (DataSet data = new DataSet())
                            {
                                data.Tables.Add(new DataTable("SocoutMatch"));
                                fda.Fill(data.Tables["SocoutMatch"]);
                                dgRankDetails.PageSize = AppFlag.iPageSize;
                                dgRankDetails.DataSource = data.Tables[0].DefaultView;
                                dgRankDetails.DataBind();
                                dgRankDetails.UpdateAfterCallBack = true;
                                Session["matches"] = data;
                            }
                        }
                    }
                    connection.Close();
                }
            }
            catch (Exception exp)
            {
                string exps = exp.ToString();
                Files.CicsWriteError(DateTime.Now.ToString("HH:mm:ss") + "   BindMatchesByDateDay()  " + exps);
            }
        }
    }
}
