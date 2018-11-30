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

    public class Update : CommonPage
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
        protected Anthem.CheckBoxList cblIP;
        //protected System.Web.UI.WebControls.RequiredFieldValidator rfv1;
        //protected Anthem.RequiredFieldValidator rfv12;
        //protected Anthem.RangeValidator RangeValidator1;

        private void btnEdit_Click(object sender, EventArgs e)
        {
            lbMsg.Text = "";
            lbMsg.UpdateAfterCallBack = true;
            dgRankDetails.CurrentPageIndex = 0;
            BindEvents(dplLeague.SelectedValue);
            //string[,] strs = new string[this.dgRankDetails.Items.Count, 2];

            //if (btnEdit.Text == "Edit")
            //{
            //    //string[,] strs = new string[this.dgRankDetails.Items.Count, 2];
            //    if (this.dgRankDetails.Items.Count == 0) return;
            //    for (int i = 0; i < this.dgRankDetails.Items.Count; i++)
            //    {
            //        strs[i, 0] = ((Anthem.Label)this.dgRankDetails.Items[i].FindControl("lbIHOSTORYRANK")).Text;
            //        strs[i, 1] = ((Anthem.Label)this.dgRankDetails.Items[i].FindControl("lbISCORE")).Text;
            //        //((Anthem.RangeValidator)this.dgRankDetails.Items[i].FindControl("RangeValidator1")).Visible = true;
            //        //((Anthem.RangeValidator)this.dgRankDetails.Items[i].FindControl("RangeValidator1")).UpdateAfterCallBack = true;
            //        ((Anthem.TextBox)this.dgRankDetails.Items[i].FindControl("txtIHOSTORYRANK2")).Visible = true;
            //        ((Anthem.TextBox)this.dgRankDetails.Items[i].FindControl("txtISCORE2")).Visible = true;
            //        ((Anthem.Label)this.dgRankDetails.Items[i].FindControl("lbIHOSTORYRANK")).Visible = false;
            //        ((Anthem.Label)this.dgRankDetails.Items[i].FindControl("lbISCORE")).Visible = false;
            //        ((Anthem.TextBox)this.dgRankDetails.Items[i].FindControl("txtIHOSTORYRANK2")).UpdateAfterCallBack = true;
            //        ((Anthem.TextBox)this.dgRankDetails.Items[i].FindControl("txtISCORE2")).UpdateAfterCallBack = true;
            //        ((Anthem.Label)this.dgRankDetails.Items[i].FindControl("lbIHOSTORYRANK")).UpdateAfterCallBack = true;
            //        ((Anthem.Label)this.dgRankDetails.Items[i].FindControl("lbISCORE")).UpdateAfterCallBack = true;
            //    }
            //    Session["data"] = strs;
            //    btnEdit.Text = "Update";
            //    lbMsg.Text = "";
            //}
            //else if (btnEdit.Text == "Update")
            //{

            //    ArrayList al = new ArrayList();
            //    //string sLog = "";
            //    //string sUpdateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff");

            //    //string[,] ss = (string[,])Session["data"];
            //    //DataSet dataSet = new DataSet();
            //    //DataSet dataSet2 = new DataSet();
            //    //FbDataAdapter adapter, adapter2;
            //    //FbConnection connection;
            //    //using (connection = new FbConnection(AppFlag.HkjcDBConn))
            //    //{
            //    //    adapter = new FbDataAdapter();
            //    //    connection.Open();
            //    //    FbCommand command = new FbCommand("SELECT * FROM MISC_RANK_HISTORY_DETAILS  where CLEAGUEALIAS= '" + dplLeague.SelectedValue + "'", connection);
            //    //    FbCommandBuilder cb = new FbCommandBuilder(adapter);
            //    //    adapter.SelectCommand = command;
            //    //    adapter.Fill(dataSet, "MISC_RANK_HISTORY_DETAILS");

            //    //    adapter2 = new FbDataAdapter();
            //    //    FbCommand command2 = new FbCommand("SELECT * FROM MISC_RANK_DETAILS  where CLEAGUEALIAS='" + dplLeague.SelectedValue + "'", connection);
            //    //    FbCommandBuilder cb2 = new FbCommandBuilder(adapter2);
            //    //    adapter2.SelectCommand = command2;
            //    //    adapter2.Fill(dataSet2, "MISC_RANK_DETAILS");


            //    for (int i = 0; i < this.dgRankDetails.Items.Count; i++)
            //    {
            //        if (((Anthem.Label)this.dgRankDetails.Items[i].FindControl("lbIHOSTORYRANK")).Text !=
            //            ((Anthem.TextBox)this.dgRankDetails.Items[i].FindControl("txtIHOSTORYRANK2")).Text ||
            //            ((Anthem.Label)this.dgRankDetails.Items[i].FindControl("lbISCORE")).Text !=
            //            ((Anthem.TextBox)this.dgRankDetails.Items[i].FindControl("txtISCORE2")).Text)
            //        {
            //            //((Anthem.TextBox)this.dgRankDetails.Items[i].FindControl("txtISCORE2")).Visible = false;
            //            //((Anthem.TextBox)this.dgRankDetails.Items[i].FindControl("txtIHOSTORYRANK2")).Visible = false;
            //            //((Anthem.Label)this.dgRankDetails.Items[i].FindControl("lbIHOSTORYRANK")).Visible = true;
            //            //((Anthem.Label)this.dgRankDetails.Items[i].FindControl("lbISCORE")).Visible = true;
            //            //((Anthem.TextBox)this.dgRankDetails.Items[i].FindControl("txtISCORE2")).UpdateAfterCallBack = true;
            //            //((Anthem.TextBox)this.dgRankDetails.Items[i].FindControl("txtIHOSTORYRANK2")).UpdateAfterCallBack = true;
            //            //((Anthem.Label)this.dgRankDetails.Items[i].FindControl("lbIHOSTORYRANK")).UpdateAfterCallBack = true;
            //            //((Anthem.Label)this.dgRankDetails.Items[i].FindControl("lbISCORE")).UpdateAfterCallBack = true;

            //            var iSCORE = ((Anthem.TextBox)this.dgRankDetails.Items[i].FindControl("txtISCORE2")).Text;
            //            var iHOSTORYRANK = ((Anthem.TextBox)this.dgRankDetails.Items[i].FindControl("txtIHOSTORYRANK2")).Text;
            //            var cTeam = ((Anthem.Label)this.dgRankDetails.Items[i].FindControl("lbCTEAM")).Text;
            //            var cLeagueAlias = ((Anthem.Label)this.dgRankDetails.Items[i].FindControl("lbCLEAGUEALIAS")).Text;
            //            string[] sr = { iSCORE, iHOSTORYRANK, cTeam, cLeagueAlias };
            //            al.Add(sr);

            //            //DataRow[] rows = dataSet.Tables["MISC_RANK_HISTORY_DETAILS"].Select("CTEAM='" + cTeam + "' and CLEAGUEALIAS='" + cLeagueAlias + "'");
            //            //rows[0].BeginEdit();
            //            //rows[0]["IHOSTORYSCORE"] = iSCORE;
            //            //rows[0]["IHOSTORYRANK"] = iHOSTORYRANK;
            //            //rows[0]["CTIMESTAMP"] = sUpdateTime;
            //            //rows[0].EndEdit();

            //            //DataRow[] rows2 = dataSet2.Tables["MISC_RANK_DETAILS"].Select("CTEAM='" + cTeam + "' and CLEAGUEALIAS='" + cLeagueAlias + "'");
            //            //rows2[0].BeginEdit();
            //            //rows2[0]["IREC_NO"] = iHOSTORYRANK;
            //            //rows2[0].EndEdit();

            //            //sLog += "         Update IHOSTORYRANK=" + iHOSTORYRANK + ", IHOSTORYSCORE=" + iSCORE + "  CTIMESTAMP=" + sUpdateTime + " where CLEAGUEALIAS=" + cLeagueAlias + " and  CTeam =" + cTeam + " and IHOSTORYRANK=" + ss[i, 0] + ", IHOSTORYSCORE=" + ss[i, 1] + "\r\n";
            //            //sLog.Remove(sLog.Length - 2, 2);

            //            //adapter.Update(dataSet.Tables["MISC_RANK_HISTORY_DETAILS"]);
            //            //dataSet.Tables["MISC_RANK_HISTORY_DETAILS"].AcceptChanges();
            //            //sLog += "         on MISC_RANK_HISTORY_DETAILS";
            //            //adapter2.Update(dataSet2.Tables["MISC_RANK_DETAILS"]);
            //            //dataSet2.Tables["MISC_RANK_DETAILS"].AcceptChanges();
            //            //sLog += " and MISC_RANK_DETAILS." + "\r\n";
            //            //connection.Close();
            //            //BindRankDetails(dplLeague.SelectedValue);
            //            //lbMsg.Text = "Success";
            //            //Files.CicsWriteLog(DateTime.Now.ToString("HH:mm:ss ") + "\r\n" + sLog); 
            //        }

            //        ((Anthem.TextBox)this.dgRankDetails.Items[i].FindControl("txtISCORE2")).Visible = false;
            //        ((Anthem.TextBox)this.dgRankDetails.Items[i].FindControl("txtIHOSTORYRANK2")).Visible = false;
            //        ((Anthem.Label)this.dgRankDetails.Items[i].FindControl("lbIHOSTORYRANK")).Visible = true;
            //        ((Anthem.Label)this.dgRankDetails.Items[i].FindControl("lbISCORE")).Visible = true;
            //        ((Anthem.TextBox)this.dgRankDetails.Items[i].FindControl("txtISCORE2")).UpdateAfterCallBack = true;
            //        ((Anthem.TextBox)this.dgRankDetails.Items[i].FindControl("txtIHOSTORYRANK2")).UpdateAfterCallBack = true;
            //        ((Anthem.Label)this.dgRankDetails.Items[i].FindControl("lbIHOSTORYRANK")).UpdateAfterCallBack = true;
            //        ((Anthem.Label)this.dgRankDetails.Items[i].FindControl("lbISCORE")).UpdateAfterCallBack = true;
            //    }

            //    btnEdit.Text = "Edit"; btnEdit.UpdateAfterCallBack = true;

            //    if (al.Count == 0) { return; }

            //    string[,] ss = (string[,])Session["data"];
            //    string strConn = "";
            //    string sLog = "";
            //    string sUpdateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff");

            //    if (cblIP.Items.Count > 0 && cblIP.Items[0].Selected)
            //        try
            //        {
            //            using (FbConnection connection = new FbConnection(AppFlag.HkjcDBConn))
            //            // using (FbCommand command = connection.CreateCommand())
            //            {
            //                strConn = connection.DataSource;
            //                DataSet dataSet = new DataSet();
            //                DataSet dataSet2 = new DataSet();
            //                connection.Open();

            //                FbDataAdapter adapter = new FbDataAdapter();
            //                FbCommand command = new FbCommand("SELECT * FROM MISC_RANK_HISTORY_DETAILS  where CLEAGUEALIAS= '" + dplLeague.SelectedValue + "'", connection);
            //                FbCommandBuilder cb = new FbCommandBuilder(adapter);
            //                adapter.SelectCommand = command;
            //                adapter.Fill(dataSet, "MISC_RANK_HISTORY_DETAILS");

            //                FbDataAdapter adapter2 = new FbDataAdapter();
            //                command = new FbCommand("SELECT * FROM MISC_RANK_DETAILS  where CLEAGUEALIAS='" + dplLeague.SelectedValue + "'", connection);
            //                FbCommandBuilder cb2 = new FbCommandBuilder(adapter2);
            //                adapter2.SelectCommand = command;
            //                adapter2.Fill(dataSet2, "MISC_RANK_DETAILS");

            //                for (int i = 0; i < al.Count; i++)
            //                {
            //                    string[] sdr = (string[])al[i];
            //                    DataRow[] rows = dataSet.Tables["MISC_RANK_HISTORY_DETAILS"].Select("CTEAM='" + sdr[2] + "' and CLEAGUEALIAS='" + sdr[3] + "'");
            //                    sLog += "         Update IHOSTORYRANK=" + sdr[1] + ", IHOSTORYSCORE=" + sdr[0] + "  CTIMESTAMP=" + sUpdateTime + " where CTeam=" + sdr[2] + " and  CLEAGUEALIAS =" + sdr[3] + " and IHOSTORYRANK=" + rows[0]["IHOSTORYRANK"] + ", IHOSTORYSCORE=" + rows[0]["IHOSTORYSCORE"] + "\r\n";
            //                    rows[0].BeginEdit();
            //                    rows[0]["IHOSTORYSCORE"] = sdr[0];
            //                    rows[0]["IHOSTORYRANK"] = sdr[1];
            //                    rows[0]["CTIMESTAMP"] = sUpdateTime;
            //                    rows[0].EndEdit();

            //                    DataRow[] rows2 = dataSet2.Tables["MISC_RANK_DETAILS"].Select("CTEAM='" + sdr[2] + "' and CLEAGUEALIAS='" + sdr[3] + "'");
            //                    if (rows2.Length == 0)
            //                    {
            //                        sLog += "         CTeam=" + sdr[2] + " and  CLEAGUEALIAS =" + sdr[3] + " not found on MISC_RANK_DETAILS" + "\r\n";
            //                        //    continue;
            //                    }
            //                    else
            //                    {
            //                        rows2[0].BeginEdit();
            //                        rows2[0]["IREC_NO"] = sdr[1];
            //                        rows2[0].EndEdit();
            //                    }
            //                    //      sLog += "         Update IHOSTORYRANK=" + iHOSTORYRANK + ", IHOSTORYSCORE=" + iSCORE + "  CTIMESTAMP=" + sUpdateTime + " where CTeam=" + sdr[2] + " and  CLEAGUEALIAS =" + sdr[3] + " and IHOSTORYRANK=" + ss[i, 0] + ", IHOSTORYSCORE=" + ss[i, 1] + "\r\n";
            //                    //sLog.Remove(sLog.Length - 2, 2);
            //                }

            //                if (sLog != "") { sLog = sLog.Remove(sLog.Length - 2, 2); }

            //                adapter.Update(dataSet.Tables["MISC_RANK_HISTORY_DETAILS"]);
            //                dataSet.Tables["MISC_RANK_HISTORY_DETAILS"].AcceptChanges();
            //                //   sLog += "         on MISC_RANK_HISTORY_DETAILS";
            //                adapter2.Update(dataSet2.Tables["MISC_RANK_DETAILS"]);
            //                dataSet2.Tables["MISC_RANK_DETAILS"].AcceptChanges();
            //                // sLog += " and MISC_RANK_DETAILS.";// +"\r\n";
            //                connection.Close();
            //                BindEvents(dplLeague.SelectedValue);
            //                lbMsg.Text = connection.DataSource + " Success ;  ";
            //                Files.CicsWriteLog(DateTime.Now.ToString("HH:mm:ss ") + strConn + ":\r\n" + sLog);//  " and MISC_RANK_DETAILS.");
            //            }
            //            btnEdit.Text = "Edit";
            //        }
            //        catch (Exception exp)
            //        {
            //            Files.CicsWriteLog(DateTime.Now.ToString("HH:mm:ss") + " " + strConn + ":[Failure], Update.");

            //            btnEdit.Text = "Edit";
            //            lbMsg.Text = strConn + " Failure ;  ";
            //            string exps = exp.ToString();
            //            Files.CicsWriteError(DateTime.Now.ToString("HH:mm:ss ") + strConn + ":  btnEdit_Click()  " + exps);
            //        }

            //    //others 
            //    sLog = "";
            //    if (cblIP.Items.Count > 1 && cblIP.Items[1].Selected)
            //        try
            //        {
            //            using (FbConnection connection = new FbConnection(AppFlag.ScoutsDBConn))
            //            // using (FbCommand command = connection.CreateCommand())
            //            {
            //                strConn = connection.DataSource;
            //                DataSet dataSet = new DataSet();
            //                DataSet dataSet2 = new DataSet();
            //                connection.Open();

            //                FbDataAdapter adapter = new FbDataAdapter();
            //                FbCommand command = new FbCommand("SELECT * FROM MISC_RANK_HISTORY_DETAILS  where CLEAGUEALIAS= '" + dplLeague.SelectedValue + "'", connection);
            //                FbCommandBuilder cb = new FbCommandBuilder(adapter);
            //                adapter.SelectCommand = command;
            //                adapter.Fill(dataSet, "MISC_RANK_HISTORY_DETAILS");

            //                FbDataAdapter adapter2 = new FbDataAdapter();
            //                command = new FbCommand("SELECT * FROM MISC_RANK_DETAILS  where CLEAGUEALIAS='" + dplLeague.SelectedValue + "'", connection);
            //                FbCommandBuilder cb2 = new FbCommandBuilder(adapter2);
            //                adapter2.SelectCommand = command;
            //                adapter2.Fill(dataSet2, "MISC_RANK_DETAILS");

            //                for (int i = 0; i < al.Count; i++)
            //                {
            //                    string[] sdr = (string[])al[i];
            //                    DataRow[] rows = dataSet.Tables["MISC_RANK_HISTORY_DETAILS"].Select("CTEAM='" + sdr[2] + "' and CLEAGUEALIAS='" + sdr[3] + "'");
            //                    if (rows.Length == 0)
            //                    {
            //                        sLog += "         CTeam=" + sdr[2] + " and  CLEAGUEALIAS =" + sdr[3] + " not found on MISC_RANK_HISTORY_DETAILS" + "\r\n";
            //                        //     continue;
            //                    }
            //                    else
            //                    {
            //                        sLog += "         Update IHOSTORYRANK=" + sdr[1] + ", IHOSTORYSCORE=" + sdr[0] + "  CTIMESTAMP=" + sUpdateTime + " where CTeam=" + sdr[2] + " and  CLEAGUEALIAS =" + sdr[3] + " and IHOSTORYRANK=" + rows[0]["IHOSTORYRANK"] + ", IHOSTORYSCORE=" + rows[0]["IHOSTORYSCORE"] + "\r\n";
            //                        rows[0].BeginEdit();
            //                        rows[0]["IHOSTORYSCORE"] = sdr[0];
            //                        rows[0]["IHOSTORYRANK"] = sdr[1];
            //                        rows[0]["CTIMESTAMP"] = sUpdateTime;
            //                        rows[0].EndEdit();
            //                    }

            //                    DataRow[] rows2 = dataSet2.Tables["MISC_RANK_DETAILS"].Select("CTEAM='" + sdr[2] + "' and CLEAGUEALIAS='" + sdr[3] + "'");
            //                    if (rows2.Length == 0)
            //                    {
            //                        sLog += "         CTeam=" + sdr[2] + " and  CLEAGUEALIAS =" + sdr[3] + " not found on MISC_RANK_DETAILS" + "\r\n";
            //                        //  continue;
            //                    }
            //                    else
            //                    {
            //                        rows2[0].BeginEdit();
            //                        rows2[0]["IREC_NO"] = sdr[1];
            //                        rows2[0].EndEdit();
            //                    }
            //                    //      sLog += "         Update IHOSTORYRANK=" + iHOSTORYRANK + ", IHOSTORYSCORE=" + iSCORE + "  CTIMESTAMP=" + sUpdateTime + " where CTeam=" + sdr[2] + " and  CLEAGUEALIAS =" + sdr[3] + " and IHOSTORYRANK=" + ss[i, 0] + ", IHOSTORYSCORE=" + ss[i, 1] + "\r\n";
            //                    //  sLog = sLog.Remove(sLog.Length - 2, 2);
            //                }

            //                if (sLog != "") { sLog = sLog.Remove(sLog.Length - 2, 2); }

            //                adapter.Update(dataSet.Tables["MISC_RANK_HISTORY_DETAILS"]);
            //                dataSet.Tables["MISC_RANK_HISTORY_DETAILS"].AcceptChanges();
            //                // sLog += "         on MISC_RANK_HISTORY_DETAILS";
            //                adapter2.Update(dataSet2.Tables["MISC_RANK_DETAILS"]);
            //                dataSet2.Tables["MISC_RANK_DETAILS"].AcceptChanges();
            //                // sLog += " and MISC_RANK_DETAILS." + "\r\n";
            //                connection.Close();
            //                BindEvents(dplLeague.SelectedValue);
            //                if (lbMsg.Text == "") { lbMsg.Text = connection.DataSource + " Success ;  "; } else { lbMsg.Text += connection.DataSource + " Success ;  "; }
            //                Files.CicsWriteLog(DateTime.Now.ToString("HH:mm:ss ") + strConn + ":\r\n" + sLog + "\r\n");// " and MISC_RANK_DETAILS." + "\r\n");
            //            }
            //            btnEdit.Text = "Edit";
            //        }
            //        catch (Exception exp)
            //        {
            //            Files.CicsWriteLog(DateTime.Now.ToString("HH:mm:ss") + " " + strConn + ":[Failure], Update." + "\r\n");

            //            btnEdit.Text = "Edit";
            //            if (lbMsg.Text == "") { lbMsg.Text = strConn + " Failure ;  "; } else { lbMsg.Text += strConn + " Failure ;  "; }
            //            string exps = exp.ToString();
            //            Files.CicsWriteError(DateTime.Now.ToString("HH:mm:ss ") + strConn + ":  btnEdit_Click()  " + exps + "\r\n");
            //        }

            //}


            //lbMsg.UpdateAfterCallBack = true;
            //btnEdit.UpdateAfterCallBack = true;


            //string[,] strs = new string[this.dgRankDetails.Items.Count, 2];

            //if (btnEdit.Text == "Edit")
            //{
            //    //string[,] strs = new string[this.dgRankDetails.Items.Count, 2];

            //    for (int i = 0; i < this.dgRankDetails.Items.Count; i++)
            //    {
            //        strs[i, 0] = ((Anthem.Label)this.dgRankDetails.Items[i].FindControl("lbIHOSTORYRANK")).Text;
            //        strs[i, 1] = ((Anthem.Label)this.dgRankDetails.Items[i].FindControl("lbISCORE")).Text; 
            //        //((Anthem.RangeValidator)this.dgRankDetails.Items[i].FindControl("RangeValidator1")).Visible = true;
            //        //((Anthem.RangeValidator)this.dgRankDetails.Items[i].FindControl("RangeValidator1")).UpdateAfterCallBack = true;
            //        ((Anthem.TextBox) this.dgRankDetails.Items[i].FindControl("txtIHOSTORYRANK2")).Visible = true;
            //        ((Anthem.TextBox)this.dgRankDetails.Items[i].FindControl("txtISCORE2")).Visible = true;
            //        ((Anthem.Label)this.dgRankDetails.Items[i].FindControl("lbIHOSTORYRANK")).Visible = false;
            //        ((Anthem.Label)this.dgRankDetails.Items[i].FindControl("lbISCORE")).Visible = false;
            //        ((Anthem.TextBox) this.dgRankDetails.Items[i].FindControl("txtIHOSTORYRANK2")).UpdateAfterCallBack =true;
            //        ((Anthem.TextBox)this.dgRankDetails.Items[i].FindControl("txtISCORE2")).UpdateAfterCallBack =true;
            //        ((Anthem.Label) this.dgRankDetails.Items[i].FindControl("lbIHOSTORYRANK")).UpdateAfterCallBack =true;
            //        ((Anthem.Label)this.dgRankDetails.Items[i].FindControl("lbISCORE")).UpdateAfterCallBack = true;
            //    }
            //    Session["data"] = strs;
            //    btnEdit.Text = "Update";
            //    lbMsg.Text = "";
            //}
            //else if (btnEdit.Text == "Update")
            //{
            //    try
            //    {
            //        string[,] ss = (string[,])Session["data"];
            //        DataSet dataSet = new DataSet();
            //        DataSet dataSet2 = new DataSet();
            //        FbDataAdapter adapter, adapter2;
            //        FbConnection connection;
            //        using (connection = new FbConnection(AppFlag.HkjcDBConn))
            //        {
            //            adapter = new FbDataAdapter(); 
            //            connection.Open();
            //            FbCommand command = new FbCommand("SELECT * FROM MISC_RANK_HISTORY_DETAILS  where CLEAGUEALIAS= '" + dplLeague.SelectedValue + "'", connection);
            //            FbCommandBuilder cb = new FbCommandBuilder(adapter);
            //            adapter.SelectCommand = command;
            //            adapter.Fill(dataSet, "MISC_RANK_HISTORY_DETAILS");

            //            adapter2 = new FbDataAdapter();
            //            FbCommand command2 = new FbCommand("SELECT * FROM MISC_RANK_DETAILS  where CLEAGUEALIAS='" + dplLeague.SelectedValue+"'", connection);
            //            FbCommandBuilder cb2 = new FbCommandBuilder(adapter2);
            //            adapter2.SelectCommand = command2;
            //            adapter2.Fill(dataSet2, "MISC_RANK_DETAILS"); 

            //            string sLog = "";
            //            string sUpdateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff");
            //            for (int i = 0; i < this.dgRankDetails.Items.Count; i++)
            //            {
            //                if (((Anthem.Label) this.dgRankDetails.Items[i].FindControl("lbIHOSTORYRANK")).Text !=
            //                    ((Anthem.TextBox) this.dgRankDetails.Items[i].FindControl("txtIHOSTORYRANK2")).Text ||
            //                    ((Anthem.Label)this.dgRankDetails.Items[i].FindControl("lbISCORE")).Text !=
            //                    ((Anthem.TextBox)this.dgRankDetails.Items[i].FindControl("txtISCORE2")).Text)
            //                {
            //                    var iSCORE = ((Anthem.TextBox)this.dgRankDetails.Items[i].FindControl("txtISCORE2")).Text;
            //                    var iHOSTORYRANK =((Anthem.TextBox) this.dgRankDetails.Items[i].FindControl("txtIHOSTORYRANK2")).Text;
            //                    var cTeam = ((Anthem.Label) this.dgRankDetails.Items[i].FindControl("lbCTEAM")).Text;
            //                    var cLeagueAlias =((Anthem.Label) this.dgRankDetails.Items[i].FindControl("lbCLEAGUEALIAS")).Text;

            //                    DataRow[] rows = dataSet.Tables["MISC_RANK_HISTORY_DETAILS"]
            //                        .Select("CTEAM='" + cTeam + "' and CLEAGUEALIAS='" + cLeagueAlias + "'");
            //                    rows[0].BeginEdit();
            //                    rows[0]["IHOSTORYSCORE"] = iSCORE;
            //                    rows[0]["IHOSTORYRANK"] = iHOSTORYRANK;
            //                    rows[0]["CTIMESTAMP"] = sUpdateTime;
            //                    rows[0].EndEdit();

            //                    DataRow[] rows2 = dataSet2.Tables["MISC_RANK_DETAILS"].Select("CTEAM='" + cTeam + "' and CLEAGUEALIAS='" + cLeagueAlias + "'");
            //                    rows2[0].BeginEdit();
            //                    rows2[0]["IREC_NO"] = iHOSTORYRANK;
            //                    rows2[0].EndEdit();

            //                    sLog += "         Update IHOSTORYRANK=" + iHOSTORYRANK + ", IHOSTORYSCORE=" + iSCORE + "  CTIMESTAMP=" + sUpdateTime + " where CLEAGUEALIAS=" + cLeagueAlias + " and  CTeam =" + cTeam + " and IHOSTORYRANK=" + ss[i, 0] + ", IHOSTORYSCORE=" + ss[i, 1] + "\r\n";
            //                }
            //                ((Anthem.TextBox)this.dgRankDetails.Items[i].FindControl("txtISCORE2")).Visible = false;
            //                ((Anthem.TextBox) this.dgRankDetails.Items[i].FindControl("txtIHOSTORYRANK2")).Visible =
            //                    false;
            //                ((Anthem.Label) this.dgRankDetails.Items[i].FindControl("lbIHOSTORYRANK")).Visible = true;
            //                ((Anthem.Label)this.dgRankDetails.Items[i].FindControl("lbISCORE")).Visible = true;
            //                ((Anthem.TextBox)this.dgRankDetails.Items[i].FindControl("txtISCORE2"))
            //                    .UpdateAfterCallBack = true;
            //                ((Anthem.TextBox) this.dgRankDetails.Items[i].FindControl("txtIHOSTORYRANK2"))
            //                    .UpdateAfterCallBack = true;
            //                ((Anthem.Label) this.dgRankDetails.Items[i].FindControl("lbIHOSTORYRANK"))
            //                    .UpdateAfterCallBack = true;
            //                ((Anthem.Label)this.dgRankDetails.Items[i].FindControl("lbISCORE")).UpdateAfterCallBack =
            //                    true;
            //            }
            //            if (sLog != "")
            //            {
            //                sLog.Remove(sLog.Length - 2, 2);
            //                adapter.Update(dataSet.Tables["MISC_RANK_HISTORY_DETAILS"]);
            //                dataSet.Tables["MISC_RANK_HISTORY_DETAILS"].AcceptChanges();
            //                sLog += "         on MISC_RANK_HISTORY_DETAILS";
            //                adapter2.Update(dataSet2.Tables["MISC_RANK_DETAILS"]);
            //                dataSet2.Tables["MISC_RANK_DETAILS"].AcceptChanges();
            //                sLog += " and MISC_RANK_DETAILS." + "\r\n";
            //                connection.Close(); 
            //                BindRankDetails(dplLeague.SelectedValue);
            //                lbMsg.Text = "Success"; 
            //                Files.CicsWriteLog(DateTime.Now.ToString("HH:mm:ss ") + "\r\n" +sLog);
            //            }
            //            btnEdit.Text = "Edit";
            //        }
            //    }
            //    catch (Exception exp)
            //    {
            //        btnEdit.Text = "Edit";
            //        lbMsg.Text = "Failure";
            //        string exps = exp.ToString();
            //        Files.CicsWriteError(DateTime.Now.ToString("HH:mm:ss") + "   BindLeague()  " + exps);
            //    }
            //}
            //lbMsg.UpdateAfterCallBack = true;
            //btnEdit.UpdateAfterCallBack = true; 
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
                        Files.CicsWriteLog((id > 0 ? DateTime.Now.ToString("HH:mm:ss") + " [Success] " : DateTime.Now.ToString("HH:mm:ss") + " [Failure] ") + "Cancel Sync [" + eventId + "] EMATCHES[" + dayCODE + " " + matchNo + "] " + " " + eventName);
                        this.dgRankDetails.EditItemIndex = -1;
                    }
                    connection.Close();
                }
                if (id > 0)
                {
                    this.lbMsg.Visible = true;
                    this.lbMsg.Text = "[Success] Cancel [" + eventId + "][" + dayCODE + " " + matchNo + "] " + " " + eventName;
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
                if (((Anthem.Label)e.Item.Cells[13].Controls[3]).Text != "")
                {
                    ((Anthem.DropDownList)e.Item.Cells[13].Controls[1]).SelectedValue = ((Anthem.Label)e.Item.Cells[13].Controls[3]).Text;
                }
            }
        }

        private void dgSchedule_ItemCreated(object sender, DataGridItemEventArgs e)
        {
            try
            {
                if (e.Item.ItemType == ListItemType.EditItem)
                {
                   // string df = "";
             //  ((Anthem.DropDownList)e.Item.FindControl("dplDayCode")).SelectedValue = ((Anthem.Label)e.Item.FindControl("lbHKJCDAYCODE")).Text;

                    //((System.Web.UI.WebControls.TextBox)e.Item.FindControl("txtIHOSTORYRANK")).Attributes.Add("onChange", "javascript:return CheckNum(this)");
                    //((System.Web.UI.WebControls.TextBox)e.Item.FindControl("txtISCORE")).Attributes.Add("onChange", "javascript:return CheckNum(this)");
                }
                else if (e.Item.ItemType == ListItemType.Item)
                {
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
            this.dgRankDetails.CurrentPageIndex = e.NewPageIndex;
            DataSet data = (DataSet)this.Session["rankData"];
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
            int id = 0;
            try
            {
                using (FbConnection connection = new FbConnection(AppFlag.ScoutsDBConn))
                {
                    connection.Open();
                    using (FbCommand cmd2 = new FbCommand())
                    {  //maybe return booked or no
                        cmd2.CommandText = "SYNC_MANUAL_HKJCDATA_WEB";
                        cmd2.CommandType = CommandType.StoredProcedure;
                        cmd2.Connection = connection;
                        cmd2.Parameters.Add("@EMATCHID", eventId);
                        cmd2.Parameters.Add("@HKJCDAYCODE", dayCODE);
                        cmd2.Parameters.Add("@HKJCMATCHNO", matchNo);
                        cmd2.Parameters.Add("@CMATCHDATETIME1", Convert.ToDateTime(start_date).AddHours (-AppFlag.MarginOfDeviation).ToString("yyyy-MM-dd HH:mm:ss.fff"));
                        cmd2.Parameters.Add("@CMATCHDATETIME2", Convert.ToDateTime(start_date).AddDays(AppFlag.MarginOfDeviation).ToString("yyyy-MM-dd HH:mm:ss.fff"));
                          id = Convert.ToInt32(cmd2.ExecuteScalar());
                        Files.CicsWriteLog((id > 0 ? DateTime.Now.ToString("HH:mm:ss") + " [Success] " : DateTime.Now.ToString("HH:mm:ss") + " [Failure] ") + "Sync [" + eventId + "] EMATCHES[" + dayCODE + " " + matchNo + "] " + " " + eventName);
                     this.dgRankDetails.EditItemIndex = -1;
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
            BindEvents(dplLeague.SelectedValue);
            btnEdit.Text = "Get";
            btnEdit.UpdateAfterCallBack = true;
            lbMsg.Text = "";
            lbMsg.UpdateAfterCallBack = true;
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
           // this.dgRankDetails.ItemCreated += new DataGridItemEventHandler(this.dgSchedule_ItemCreated);
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
                BindEvents(dplLeague.SelectedValue);
            }
        }

        private void BindLeague()
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
                Files.CicsWriteError(DateTime.Now.ToString("HH:mm:ss") + "   BindLeague()  " + exps);
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
                    string queryString = "select R.ID ,R.NAME ,r.STATUS_NAME ,R.START_DATE,G.H_GOAL,G.G_GOAL,G.H_YELLOW,G.G_YELLOW,G.H_RED,G.G_RED,E.HKJCHOSTNAME,E.HKJCGUESTNAME,E.HKJCDAYCODE,E.HKJCMATCHNO,r.CTIMESTAMP, r.booked,e.CMATCHDATETIME "
                       + "from events r  LEFT join goalinfo g  on r.id = g.EMATCHID   LEFT join EMATCHES e on e.EMATCHID = r.id"
                       + " where (r.caction !='delete' or r.caction is null) and  r.START_DATE >= '" + txtFrom.Text.Trim() + ", 00:00:00.000' and r.START_DATE <= '" + txtTo.Text.Trim() + ", 23:59:59.000'"+ (id.ToString ()=="-1"?"": " and STATUS_ID ="+dplLeague.SelectedValue) +  " order by r.START_DATE ASC  ";
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
                                Session["rankData"] = data;
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

    }
}

