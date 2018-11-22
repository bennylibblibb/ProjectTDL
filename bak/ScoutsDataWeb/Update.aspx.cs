﻿namespace JC_SoccerWeb
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
        private IFormatProvider culture = new CultureInfo("en-GB", true);
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
        protected Anthem.Label lbIHEADER_ID;
        protected Anthem.Button btnEdit;
        protected Anthem.Label lbMsg;
        protected Anthem.CheckBoxList cblIP;
        //protected System.Web.UI.WebControls.RequiredFieldValidator rfv1;
        //protected Anthem.RequiredFieldValidator rfv12;
        //protected Anthem.RangeValidator RangeValidator1;

        private void btnEdit_Click(object sender, EventArgs e)
        {
            string[,] strs = new string[this.dgRankDetails.Items.Count, 2];

            if (btnEdit.Text == "Edit")
            {
                //string[,] strs = new string[this.dgRankDetails.Items.Count, 2];
                if (this.dgRankDetails.Items.Count == 0) return;
                for (int i = 0; i < this.dgRankDetails.Items.Count; i++)
                {
                    strs[i, 0] = ((Anthem.Label)this.dgRankDetails.Items[i].FindControl("lbIHOSTORYRANK")).Text;
                    strs[i, 1] = ((Anthem.Label)this.dgRankDetails.Items[i].FindControl("lbISCORE")).Text;
                    //((Anthem.RangeValidator)this.dgRankDetails.Items[i].FindControl("RangeValidator1")).Visible = true;
                    //((Anthem.RangeValidator)this.dgRankDetails.Items[i].FindControl("RangeValidator1")).UpdateAfterCallBack = true;
                    ((Anthem.TextBox)this.dgRankDetails.Items[i].FindControl("txtIHOSTORYRANK2")).Visible = true;
                    ((Anthem.TextBox)this.dgRankDetails.Items[i].FindControl("txtISCORE2")).Visible = true;
                    ((Anthem.Label)this.dgRankDetails.Items[i].FindControl("lbIHOSTORYRANK")).Visible = false;
                    ((Anthem.Label)this.dgRankDetails.Items[i].FindControl("lbISCORE")).Visible = false;
                    ((Anthem.TextBox)this.dgRankDetails.Items[i].FindControl("txtIHOSTORYRANK2")).UpdateAfterCallBack = true;
                    ((Anthem.TextBox)this.dgRankDetails.Items[i].FindControl("txtISCORE2")).UpdateAfterCallBack = true;
                    ((Anthem.Label)this.dgRankDetails.Items[i].FindControl("lbIHOSTORYRANK")).UpdateAfterCallBack = true;
                    ((Anthem.Label)this.dgRankDetails.Items[i].FindControl("lbISCORE")).UpdateAfterCallBack = true;
                }
                Session["data"] = strs;
                btnEdit.Text = "Update";
                lbMsg.Text = "";
            }
            else if (btnEdit.Text == "Update")
            {

                ArrayList al = new ArrayList();
                //string sLog = "";
                //string sUpdateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff");

                //string[,] ss = (string[,])Session["data"];
                //DataSet dataSet = new DataSet();
                //DataSet dataSet2 = new DataSet();
                //FbDataAdapter adapter, adapter2;
                //FbConnection connection;
                //using (connection = new FbConnection(AppFlag.CENTASMSINTEConn))
                //{
                //    adapter = new FbDataAdapter();
                //    connection.Open();
                //    FbCommand command = new FbCommand("SELECT * FROM MISC_RANK_HISTORY_DETAILS  where CLEAGUEALIAS= '" + dplLeague.SelectedValue + "'", connection);
                //    FbCommandBuilder cb = new FbCommandBuilder(adapter);
                //    adapter.SelectCommand = command;
                //    adapter.Fill(dataSet, "MISC_RANK_HISTORY_DETAILS");

                //    adapter2 = new FbDataAdapter();
                //    FbCommand command2 = new FbCommand("SELECT * FROM MISC_RANK_DETAILS  where CLEAGUEALIAS='" + dplLeague.SelectedValue + "'", connection);
                //    FbCommandBuilder cb2 = new FbCommandBuilder(adapter2);
                //    adapter2.SelectCommand = command2;
                //    adapter2.Fill(dataSet2, "MISC_RANK_DETAILS");


                for (int i = 0; i < this.dgRankDetails.Items.Count; i++)
                {
                    if (((Anthem.Label)this.dgRankDetails.Items[i].FindControl("lbIHOSTORYRANK")).Text !=
                        ((Anthem.TextBox)this.dgRankDetails.Items[i].FindControl("txtIHOSTORYRANK2")).Text ||
                        ((Anthem.Label)this.dgRankDetails.Items[i].FindControl("lbISCORE")).Text !=
                        ((Anthem.TextBox)this.dgRankDetails.Items[i].FindControl("txtISCORE2")).Text)
                    {
                        //((Anthem.TextBox)this.dgRankDetails.Items[i].FindControl("txtISCORE2")).Visible = false;
                        //((Anthem.TextBox)this.dgRankDetails.Items[i].FindControl("txtIHOSTORYRANK2")).Visible = false;
                        //((Anthem.Label)this.dgRankDetails.Items[i].FindControl("lbIHOSTORYRANK")).Visible = true;
                        //((Anthem.Label)this.dgRankDetails.Items[i].FindControl("lbISCORE")).Visible = true;
                        //((Anthem.TextBox)this.dgRankDetails.Items[i].FindControl("txtISCORE2")).UpdateAfterCallBack = true;
                        //((Anthem.TextBox)this.dgRankDetails.Items[i].FindControl("txtIHOSTORYRANK2")).UpdateAfterCallBack = true;
                        //((Anthem.Label)this.dgRankDetails.Items[i].FindControl("lbIHOSTORYRANK")).UpdateAfterCallBack = true;
                        //((Anthem.Label)this.dgRankDetails.Items[i].FindControl("lbISCORE")).UpdateAfterCallBack = true;

                        var iSCORE = ((Anthem.TextBox)this.dgRankDetails.Items[i].FindControl("txtISCORE2")).Text;
                        var iHOSTORYRANK = ((Anthem.TextBox)this.dgRankDetails.Items[i].FindControl("txtIHOSTORYRANK2")).Text;
                        var cTeam = ((Anthem.Label)this.dgRankDetails.Items[i].FindControl("lbCTEAM")).Text;
                        var cLeagueAlias = ((Anthem.Label)this.dgRankDetails.Items[i].FindControl("lbCLEAGUEALIAS")).Text;
                        string[] sr = { iSCORE, iHOSTORYRANK, cTeam, cLeagueAlias };
                        al.Add(sr);

                        //DataRow[] rows = dataSet.Tables["MISC_RANK_HISTORY_DETAILS"].Select("CTEAM='" + cTeam + "' and CLEAGUEALIAS='" + cLeagueAlias + "'");
                        //rows[0].BeginEdit();
                        //rows[0]["IHOSTORYSCORE"] = iSCORE;
                        //rows[0]["IHOSTORYRANK"] = iHOSTORYRANK;
                        //rows[0]["CTIMESTAMP"] = sUpdateTime;
                        //rows[0].EndEdit();

                        //DataRow[] rows2 = dataSet2.Tables["MISC_RANK_DETAILS"].Select("CTEAM='" + cTeam + "' and CLEAGUEALIAS='" + cLeagueAlias + "'");
                        //rows2[0].BeginEdit();
                        //rows2[0]["IREC_NO"] = iHOSTORYRANK;
                        //rows2[0].EndEdit();

                        //sLog += "         Update IHOSTORYRANK=" + iHOSTORYRANK + ", IHOSTORYSCORE=" + iSCORE + "  CTIMESTAMP=" + sUpdateTime + " where CLEAGUEALIAS=" + cLeagueAlias + " and  CTeam =" + cTeam + " and IHOSTORYRANK=" + ss[i, 0] + ", IHOSTORYSCORE=" + ss[i, 1] + "\r\n";
                        //sLog.Remove(sLog.Length - 2, 2);

                        //adapter.Update(dataSet.Tables["MISC_RANK_HISTORY_DETAILS"]);
                        //dataSet.Tables["MISC_RANK_HISTORY_DETAILS"].AcceptChanges();
                        //sLog += "         on MISC_RANK_HISTORY_DETAILS";
                        //adapter2.Update(dataSet2.Tables["MISC_RANK_DETAILS"]);
                        //dataSet2.Tables["MISC_RANK_DETAILS"].AcceptChanges();
                        //sLog += " and MISC_RANK_DETAILS." + "\r\n";
                        //connection.Close();
                        //BindRankDetails(dplLeague.SelectedValue);
                        //lbMsg.Text = "Success";
                        //Files.CicsWriteLog(DateTime.Now.ToString("HH:mm:ss ") + "\r\n" + sLog); 
                    }

                    ((Anthem.TextBox)this.dgRankDetails.Items[i].FindControl("txtISCORE2")).Visible = false;
                    ((Anthem.TextBox)this.dgRankDetails.Items[i].FindControl("txtIHOSTORYRANK2")).Visible = false;
                    ((Anthem.Label)this.dgRankDetails.Items[i].FindControl("lbIHOSTORYRANK")).Visible = true;
                    ((Anthem.Label)this.dgRankDetails.Items[i].FindControl("lbISCORE")).Visible = true;
                    ((Anthem.TextBox)this.dgRankDetails.Items[i].FindControl("txtISCORE2")).UpdateAfterCallBack = true;
                    ((Anthem.TextBox)this.dgRankDetails.Items[i].FindControl("txtIHOSTORYRANK2")).UpdateAfterCallBack = true;
                    ((Anthem.Label)this.dgRankDetails.Items[i].FindControl("lbIHOSTORYRANK")).UpdateAfterCallBack = true;
                    ((Anthem.Label)this.dgRankDetails.Items[i].FindControl("lbISCORE")).UpdateAfterCallBack = true; 
                }

                btnEdit.Text = "Edit"; btnEdit.UpdateAfterCallBack = true;

                if (al.Count == 0) {   return; }
                 
                string[,] ss = (string[,])Session["data"];
                string strConn = "";
                string sLog = "";
                string sUpdateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff");
                
                if (cblIP.Items.Count > 0 && cblIP.Items[0].Selected)
                try
                {
                    using (FbConnection connection = new FbConnection(AppFlag.CENTASMSINTEConn))
                    // using (FbCommand command = connection.CreateCommand())
                    {
                        strConn = connection.DataSource;
                        DataSet dataSet = new DataSet();
                        DataSet dataSet2 = new DataSet();
                        connection.Open();

                        FbDataAdapter adapter = new FbDataAdapter();
                        FbCommand command = new FbCommand("SELECT * FROM MISC_RANK_HISTORY_DETAILS  where CLEAGUEALIAS= '" + dplLeague.SelectedValue + "'", connection);
                        FbCommandBuilder cb = new FbCommandBuilder(adapter);
                        adapter.SelectCommand = command;
                        adapter.Fill(dataSet, "MISC_RANK_HISTORY_DETAILS");

                        FbDataAdapter adapter2 = new FbDataAdapter();
                        command = new FbCommand("SELECT * FROM MISC_RANK_DETAILS  where CLEAGUEALIAS='" + dplLeague.SelectedValue + "'", connection);
                        FbCommandBuilder cb2 = new FbCommandBuilder(adapter2);
                        adapter2.SelectCommand = command;
                        adapter2.Fill(dataSet2, "MISC_RANK_DETAILS");

                        for (int i = 0; i < al.Count; i++)
                        {
                            string[] sdr = (string[])al[i];
                            DataRow[] rows = dataSet.Tables["MISC_RANK_HISTORY_DETAILS"].Select("CTEAM='" + sdr[2] + "' and CLEAGUEALIAS='" + sdr[3] + "'");
                            sLog += "         Update IHOSTORYRANK=" + sdr[1] + ", IHOSTORYSCORE=" + sdr[0] + "  CTIMESTAMP=" + sUpdateTime + " where CTeam=" + sdr[2] + " and  CLEAGUEALIAS =" + sdr[3] + " and IHOSTORYRANK=" + rows[0]["IHOSTORYRANK"] + ", IHOSTORYSCORE=" + rows[0]["IHOSTORYSCORE"] + "\r\n";
                            rows[0].BeginEdit();
                            rows[0]["IHOSTORYSCORE"] = sdr[0];
                            rows[0]["IHOSTORYRANK"] = sdr[1];
                            rows[0]["CTIMESTAMP"] = sUpdateTime;
                            rows[0].EndEdit();

                            DataRow[] rows2 = dataSet2.Tables["MISC_RANK_DETAILS"].Select("CTEAM='" + sdr[2] + "' and CLEAGUEALIAS='" + sdr[3] + "'");
                            if (rows2.Length == 0)
                            {
                                sLog += "         CTeam=" + sdr[2] + " and  CLEAGUEALIAS =" + sdr[3] + " not found on MISC_RANK_DETAILS" + "\r\n";
                                //    continue;
                            }
                            else
                            {
                                rows2[0].BeginEdit();
                                rows2[0]["IREC_NO"] = sdr[1];
                                rows2[0].EndEdit();
                            }
                            //      sLog += "         Update IHOSTORYRANK=" + iHOSTORYRANK + ", IHOSTORYSCORE=" + iSCORE + "  CTIMESTAMP=" + sUpdateTime + " where CTeam=" + sdr[2] + " and  CLEAGUEALIAS =" + sdr[3] + " and IHOSTORYRANK=" + ss[i, 0] + ", IHOSTORYSCORE=" + ss[i, 1] + "\r\n";
                            //sLog.Remove(sLog.Length - 2, 2);
                        }

                        if (sLog != "") { sLog=sLog.Remove(sLog.Length - 2, 2); }

                        adapter.Update(dataSet.Tables["MISC_RANK_HISTORY_DETAILS"]);
                        dataSet.Tables["MISC_RANK_HISTORY_DETAILS"].AcceptChanges();
                     //   sLog += "         on MISC_RANK_HISTORY_DETAILS";
                        adapter2.Update(dataSet2.Tables["MISC_RANK_DETAILS"]);
                        dataSet2.Tables["MISC_RANK_DETAILS"].AcceptChanges();
                       // sLog += " and MISC_RANK_DETAILS.";// +"\r\n";
                        connection.Close();
                        BindRankDetails(dplLeague.SelectedValue);
                        lbMsg.Text = connection.DataSource + " Success ;  ";
                        Files.CicsWriteLog(DateTime.Now.ToString("HH:mm:ss ") + strConn + ":\r\n" + sLog );//  " and MISC_RANK_DETAILS.");
                    }
                   btnEdit.Text = "Edit";
                }
                catch (Exception exp)
                {
                    Files.CicsWriteLog(DateTime.Now.ToString("HH:mm:ss") + " " + strConn + ":[Failure], Update.");

                    btnEdit.Text = "Edit";
                    lbMsg.Text = strConn + " Failure ;  ";
                    string exps = exp.ToString();
                    Files.CicsWriteError(DateTime.Now.ToString("HH:mm:ss ") + strConn + ":  btnEdit_Click()  " + exps);
                }

                //others 
                sLog = "";
                if(cblIP.Items .Count >1 && cblIP .Items[1].Selected) 
                try
                {
                    using (FbConnection connection = new FbConnection(AppFlag.CENTASMSMAININTEConn))
                    // using (FbCommand command = connection.CreateCommand())
                    {
                        strConn = connection.DataSource;
                        DataSet dataSet = new DataSet();
                        DataSet dataSet2 = new DataSet();
                        connection.Open();

                        FbDataAdapter adapter = new FbDataAdapter();
                        FbCommand command = new FbCommand("SELECT * FROM MISC_RANK_HISTORY_DETAILS  where CLEAGUEALIAS= '" + dplLeague.SelectedValue + "'", connection);
                        FbCommandBuilder cb = new FbCommandBuilder(adapter);
                        adapter.SelectCommand = command;
                        adapter.Fill(dataSet, "MISC_RANK_HISTORY_DETAILS");

                        FbDataAdapter adapter2 = new FbDataAdapter();
                        command = new FbCommand("SELECT * FROM MISC_RANK_DETAILS  where CLEAGUEALIAS='" + dplLeague.SelectedValue + "'", connection);
                        FbCommandBuilder cb2 = new FbCommandBuilder(adapter2);
                        adapter2.SelectCommand = command;
                        adapter2.Fill(dataSet2, "MISC_RANK_DETAILS");

                        for (int i = 0; i < al.Count; i++)
                        {
                            string[] sdr = (string[])al[i];
                            DataRow[] rows = dataSet.Tables["MISC_RANK_HISTORY_DETAILS"].Select("CTEAM='" + sdr[2] + "' and CLEAGUEALIAS='" + sdr[3] + "'");
                            if (rows.Length == 0)
                            {
                                sLog += "         CTeam=" + sdr[2] + " and  CLEAGUEALIAS =" + sdr[3] + " not found on MISC_RANK_HISTORY_DETAILS" + "\r\n";
                                //     continue;
                            }
                            else
                            {
                                sLog += "         Update IHOSTORYRANK=" + sdr[1] + ", IHOSTORYSCORE=" + sdr[0] + "  CTIMESTAMP=" + sUpdateTime + " where CTeam=" + sdr[2] + " and  CLEAGUEALIAS =" + sdr[3] + " and IHOSTORYRANK=" + rows[0]["IHOSTORYRANK"] + ", IHOSTORYSCORE=" + rows[0]["IHOSTORYSCORE"] + "\r\n";
                                rows[0].BeginEdit();
                                rows[0]["IHOSTORYSCORE"] = sdr[0];
                                rows[0]["IHOSTORYRANK"] = sdr[1];
                                rows[0]["CTIMESTAMP"] = sUpdateTime;
                                rows[0].EndEdit();
                            }

                            DataRow[] rows2 = dataSet2.Tables["MISC_RANK_DETAILS"].Select("CTEAM='" + sdr[2] + "' and CLEAGUEALIAS='" + sdr[3] + "'");
                            if (rows2.Length == 0)
                            {
                                sLog += "         CTeam=" + sdr[2] + " and  CLEAGUEALIAS =" + sdr[3] + " not found on MISC_RANK_DETAILS" + "\r\n";
                                //  continue;
                            }
                            else
                            {
                                rows2[0].BeginEdit();
                                rows2[0]["IREC_NO"] = sdr[1];
                                rows2[0].EndEdit();
                            }
                            //      sLog += "         Update IHOSTORYRANK=" + iHOSTORYRANK + ", IHOSTORYSCORE=" + iSCORE + "  CTIMESTAMP=" + sUpdateTime + " where CTeam=" + sdr[2] + " and  CLEAGUEALIAS =" + sdr[3] + " and IHOSTORYRANK=" + ss[i, 0] + ", IHOSTORYSCORE=" + ss[i, 1] + "\r\n";
                          //  sLog = sLog.Remove(sLog.Length - 2, 2);
                        }

                        if (sLog != "") { sLog = sLog.Remove(sLog.Length - 2, 2); }

                        adapter.Update(dataSet.Tables["MISC_RANK_HISTORY_DETAILS"]);
                        dataSet.Tables["MISC_RANK_HISTORY_DETAILS"].AcceptChanges();
                       // sLog += "         on MISC_RANK_HISTORY_DETAILS";
                        adapter2.Update(dataSet2.Tables["MISC_RANK_DETAILS"]);
                        dataSet2.Tables["MISC_RANK_DETAILS"].AcceptChanges();
                       // sLog += " and MISC_RANK_DETAILS." + "\r\n";
                        connection.Close();
                        BindRankDetails(dplLeague.SelectedValue);
                        if (lbMsg.Text == "") { lbMsg.Text = connection.DataSource + " Success ;  "; } else { lbMsg.Text += connection.DataSource + " Success ;  "; }
                        Files.CicsWriteLog(DateTime.Now.ToString("HH:mm:ss ") + strConn + ":\r\n" + sLog  + "\r\n");// " and MISC_RANK_DETAILS." + "\r\n");
                    }
                  btnEdit.Text = "Edit";
                }
                catch (Exception exp)
                {
                    Files.CicsWriteLog(DateTime.Now.ToString("HH:mm:ss") + " " + strConn + ":[Failure], Update." + "\r\n");

                    btnEdit.Text = "Edit";
                    if (lbMsg.Text == "") { lbMsg.Text = strConn + " Failure ;  "; } else { lbMsg.Text += strConn + " Failure ;  "; }
                    string exps = exp.ToString();
                    Files.CicsWriteError(DateTime.Now.ToString("HH:mm:ss ") + strConn + ":  btnEdit_Click()  " + exps + "\r\n");
                }

            }


            lbMsg.UpdateAfterCallBack = true;
            btnEdit.UpdateAfterCallBack = true;


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
            //        using (connection = new FbConnection(AppFlag.CENTASMSINTEConn))
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
            BindRankDetails(dplLeague.SelectedValue);
        }

        private void dgSchedule_DeleteCommand(object source, DataGridCommandEventArgs e)
        {
            lbMsg.Text = "";
            string strConn = "";
            string sTeam = ((Anthem.Label)e.Item.Cells[3].Controls[1]).Text;
            string sLeagueAlias = ((Anthem.Label)e.Item.Cells[2].Controls[1]).Text;

            if (cblIP.Items.Count > 0 && cblIP.Items[0].Selected)
            try
            { 
                using (FbConnection connection = new FbConnection(AppFlag.CENTASMSINTEConn))
                using (FbCommand command = connection.CreateCommand())
                {
                    strConn = connection.DataSource;
                    command.CommandText =
                        "delete from MISC_RANK_HISTORY_DETAILS Where CTeam = @cTeam and  CLEAGUEALIAS=@cLEAGUEALIAS ";
                    command.Parameters.AddWithValue("@cTeam", sTeam);
                    command.Parameters.AddWithValue("@cLEAGUEALIAS", sLeagueAlias);
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                    BindRankDetails(dplLeague.SelectedValue);
                    lbMsg.Text = connection.DataSource + " Success ;  ";
                    lbMsg.UpdateAfterCallBack = true;
                    Files.CicsWriteLog(DateTime.Now.ToString("HH:mm:ss") + " " + connection.DataSource + ": Delete CLEAGUEALIAS=" + sLeagueAlias + " and  CTeam =" + sTeam);
                }
            }
            catch (Exception exp)
            {
                Files.CicsWriteLog(DateTime.Now.ToString("HH:mm:ss") + " " + strConn + ":[Failure], Delete CLEAGUEALIAS=" + sLeagueAlias + " and  CTeam =" + sTeam);

                lbMsg.Text = strConn + " Failure ;  ";
                // lbMsg.Text = "Failure";
                lbMsg.UpdateAfterCallBack = true;
                string exps = exp.ToString();
                Files.CicsWriteError(DateTime.Now.ToString("HH:mm:ss") + " " + strConn + ":  dgSchedule_DeleteCommand()  " + exps);
            }

            if (cblIP.Items.Count > 1 && cblIP.Items[1].Selected) 
            try
            {
                int count = 0;
                bool done = false;
                using (FbConnection connection = new FbConnection(AppFlag.CENTASMSMAININTEConn))
                using (FbCommand command = connection.CreateCommand())
                {
                    strConn = connection.DataSource;
                    command.CommandText =
                        "delete from MISC_RANK_HISTORY_DETAILS Where CTeam = @cTeam and  CLEAGUEALIAS=@cLEAGUEALIAS ";
                    command.Parameters.AddWithValue("@cTeam", sTeam);
                    command.Parameters.AddWithValue("@cLEAGUEALIAS", sLeagueAlias);
                    connection.Open();
                   count= command.ExecuteNonQuery();
                   done = (count > 0 ? true : false);
                    connection.Close();
                    BindRankDetails(dplLeague.SelectedValue);
                   lbMsg.Text += connection.DataSource + " Success ;  ";
                  //  if (lbMsg.Text == "") { lbMsg.Text = connection.DataSource + " Success ;  "; } else { lbMsg.Text += connection.DataSource + " Success ;  "; }
                      
                    lbMsg.UpdateAfterCallBack = true;
                    Files.CicsWriteLog(DateTime.Now.ToString("HH:mm:ss") + " " + connection.DataSource + ": Delete CLEAGUEALIAS=" + sLeagueAlias + " and  CTeam =" + sTeam + (done ? "." : ",but not found on MISC_RANK_HISTORY_DETAILS.") + "\r\n");
                }
            }
            catch (Exception exp)
            {
                Files.CicsWriteLog(DateTime.Now.ToString("HH:mm:ss") + " " + strConn + ":[Failure], Delete CLEAGUEALIAS=" + sLeagueAlias + " and  CTeam =" + sTeam + "\r\n");
              //  if (lbMsg.Text == "") { lbMsg.Text = strConn + " Failure ;  "; } else { lbMsg.Text += strConn + " Failure ;  "; }
                      
               lbMsg.Text += strConn + " Failure ;  ";
                // lbMsg.Text = "Failure";
                lbMsg.UpdateAfterCallBack = true;
                string exps = exp.ToString();
                Files.CicsWriteError(DateTime.Now.ToString("HH:mm:ss") + " " + strConn + ":  dgSchedule_DeleteCommand()  " + exps + "\r\n");
            }
        }

        private void dgSchedule_EditCommand(object source, DataGridCommandEventArgs e)
        {
            this.dgRankDetails.EditItemIndex = e.Item.ItemIndex;
            BindRankDetails(dplLeague.SelectedValue);
            btnEdit.Text = "Edit";
            lbMsg.Text = "";
            btnEdit.UpdateAfterCallBack = true;
            lbMsg.UpdateAfterCallBack = true;
        }

        private void dgSchedule_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            //for (int i = 0; i < this.dgRankDetails.Items.Count; i++)
            //{
            //    if ((((Anthem.Label)this.dgRankDetails.Items[i].FindControl("lbdgSTATUS")).Text == "C") || (((Anthem.Label)this.dgRankDetails.Items[i].FindControl("lbdgSTATUS")).Text == "D"))
            //    {
            //        this.dgRankDetails.Items[i].Cells[6].Visible = false;
            //        this.dgRankDetails.Items[i].Cells[7].Visible = false;
            //    }
            //}
        }

        private void dgSchedule_ItemCreated(object sender, DataGridItemEventArgs e)
        {
            try
            {
                if (e.Item.ItemType == ListItemType.EditItem)
                {
                    ((System.Web.UI.WebControls.TextBox)e.Item.FindControl("txtIHOSTORYRANK")).Attributes.Add("onChange", "javascript:return CheckNum(this)");
                    ((System.Web.UI.WebControls.TextBox)e.Item.FindControl("txtISCORE")).Attributes.Add("onChange", "javascript:return CheckNum(this)");
                }
                else if (e.Item.ItemType == ListItemType.Item)
                {
                    ((System.Web.UI.WebControls.TextBox)e.Item.FindControl("txtIHOSTORYRANK2")).Attributes.Add("onChange", "javascript:return CheckNum(this)");
                    ((System.Web.UI.WebControls.TextBox)e.Item.FindControl("txtISCORE2")).Attributes.Add("onChange", "javascript:return CheckNum(this)");
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
            this.dgRankDetails.DataSource = data.Tables["RankDetails"].DefaultView;
            this.dgRankDetails.PageSize = AppFlag.iPageSize;
            this.dgRankDetails.DataBind();
            this.dgRankDetails.UpdateAfterCallBack = true;
            btnEdit.Text = "Edit";
            btnEdit.UpdateAfterCallBack = true;
            lbMsg.Text = "";
            lbMsg.UpdateAfterCallBack = true;
        }

        private void dgSchedule_UpdateCommand(object source, DataGridCommandEventArgs e)
        {
            string strConn = "";
            string sUpdateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff");
            string sSCORE = ((Anthem.TextBox)e.Item.Cells[6].Controls[1]).Text;
            string sHOSTORYRANK = ((Anthem.TextBox)e.Item.Cells[5].Controls[1]).Text;
            string sTeam = ((Anthem.Label)e.Item.Cells[3].Controls[1]).Text;
            string sLeagueAlias = ((Anthem.Label)e.Item.Cells[2].Controls[1]).Text;
            string strRank = "";
            string strScore = "";

            if (cblIP.Items.Count > 0 && cblIP.Items[0].Selected)
                try
                {

                    if (((Anthem.Label)e.Item.FindControl("lbIHOSTORYRANK3")).Text !=
                        ((Anthem.TextBox)e.Item.Cells[5].Controls[1]).Text ||
                        ((Anthem.Label)e.Item.FindControl("lbISCORE3")).Text !=
                        ((Anthem.TextBox)e.Item.Cells[6].Controls[1]).Text)
                    {
                        strRank = ((Anthem.Label)e.Item.FindControl("lbIHOSTORYRANK3")).Text;
                        strScore = ((Anthem.Label)e.Item.FindControl("lbISCORE3")).Text;
                        bool done = false;
                        int count = 0;
                        using (FbConnection connection = new FbConnection(AppFlag.CENTASMSINTEConn))
                        using (FbCommand command = connection.CreateCommand())
                        {
                            strConn = connection.DataSource;
                            command.CommandText =
                                "UPDATE MISC_RANK_HISTORY_DETAILS SET CTIMESTAMP= @TIMESTAMP ,IHOSTORYRANK= @iHOSTORYRANK  ,IHOSTORYSCORE= @iSCORE Where CTeam = @cTeam and CLEAGUEALIAS=@cLEAGUEALIAS";
                            command.Parameters.AddWithValue("@TIMESTAMP", sUpdateTime);
                            command.Parameters.AddWithValue("@iHOSTORYRANK", sHOSTORYRANK);
                            command.Parameters.AddWithValue("@iSCORE", sSCORE);
                            command.Parameters.AddWithValue("@cTeam", sTeam);
                            command.Parameters.AddWithValue("@cLEAGUEALIAS", sLeagueAlias);
                            connection.Open();
                            command.ExecuteNonQuery();
                            command.CommandText =
                                "UPDATE MISC_RANK_DETAILS SET IREC_NO= @iIREC_NO ,ISEQ_NO= @iISEQ_NO  Where CTeam = @cTeam and CLEAGUEALIAS=@cLEAGUEALIAS";
                            command.Parameters.AddWithValue("@iIREC_NO", sHOSTORYRANK);
                            command.Parameters.AddWithValue("@iISEQ_NO", sSCORE);
                            command.Parameters.AddWithValue("@cTeam", sTeam);
                            command.Parameters.AddWithValue("@cLEAGUEALIAS", sLeagueAlias);
                              count = command.ExecuteNonQuery();
                            done = (count > 0) ? true : false;
                            connection.Close();
                            this.dgRankDetails.EditItemIndex = -1;
                            BindRankDetails(dplLeague.SelectedValue);
                            //   lbMsg.Text = "Success";
                            lbMsg.Text = strConn + " Success ;  ";
                            lbMsg.UpdateAfterCallBack = true;

                            Files.CicsWriteLog(DateTime.Now.ToString("HH:mm:ss") + " " + strConn + ": Update IHOSTORYRANK=" + sHOSTORYRANK +
                                               ", ISEQ_NO =" + sSCORE + " and CTIMESTAMP=" + sUpdateTime + " where CLEAGUEALIAS=" + sLeagueAlias +
                                               " and  CTeam =" + sTeam + " and IHOSTORYRANK=" + strRank + ", IHOSTORYSCORE=" + strScore
                                               + " on MISC_RANK_HISTORY_DETAILS and MISC_RANK_DETAILS"+(done?".":",but not found on MISC_RANK_DETAILS."));//+ "\r\n");//" on 2 tables.");
                        }
                    }
                }
                catch (Exception exp)
                {
                    Files.CicsWriteLog(DateTime.Now.ToString("HH:mm:ss") + " " + strConn + ":[Failure] Update IHOSTORYRANK=" + sHOSTORYRANK +
                                              ", ISEQ_NO =" + sSCORE + " and CTIMESTAMP=" + sUpdateTime + " where CLEAGUEALIAS=" + sLeagueAlias +
                                              " and  CTeam =" + sTeam + " and IHOSTORYRANK=" + strRank + ", IHOSTORYSCORE=" + strScore
                                              + " on MISC_RANK_HISTORY_DETAILS and MISC_RANK_DETAILS.");//+ "\r\n");//" on 2 tables.");
                    lbMsg.Text = strConn + " Failure ;  ";
                    // lbMsg.Text = "Failure";
                    lbMsg.UpdateAfterCallBack = true;
                    string exps = exp.ToString();
                    Files.CicsWriteError(DateTime.Now.ToString("HH:mm:ss") + " " + strConn + ":  dgSchedule_UpdateCommand()  " + exps);
                }

            if (cblIP.Items.Count > 1 && cblIP.Items[1].Selected) 
            try
            {
                if (((Anthem.Label)e.Item.FindControl("lbIHOSTORYRANK3")).Text !=
                    ((Anthem.TextBox)e.Item.Cells[5].Controls[1]).Text ||
                    ((Anthem.Label)e.Item.FindControl("lbISCORE3")).Text !=
                    ((Anthem.TextBox)e.Item.Cells[6].Controls[1]).Text)
                {
                    strRank = ((Anthem.Label)e.Item.FindControl("lbIHOSTORYRANK3")).Text;
                    strScore = ((Anthem.Label)e.Item.FindControl("lbISCORE3")).Text;
                    bool done1 = false;
                    bool done2 = false;
                    int count = 0; 
                    using (FbConnection connection = new FbConnection(AppFlag.CENTASMSMAININTEConn))
                    using (FbCommand command = connection.CreateCommand())
                    {
                        strConn = connection.DataSource;
                        command.CommandText =
                            "UPDATE MISC_RANK_HISTORY_DETAILS SET CTIMESTAMP= @TIMESTAMP ,IHOSTORYRANK= @iHOSTORYRANK  ,IHOSTORYSCORE= @iSCORE Where CTeam = @cTeam and CLEAGUEALIAS=@cLEAGUEALIAS";
                        command.Parameters.AddWithValue("@TIMESTAMP", sUpdateTime);
                        command.Parameters.AddWithValue("@iHOSTORYRANK", sHOSTORYRANK);
                        command.Parameters.AddWithValue("@iSCORE", sSCORE);
                        command.Parameters.AddWithValue("@cTeam", sTeam);
                        command.Parameters.AddWithValue("@cLEAGUEALIAS", sLeagueAlias);
                        connection.Open();
                         count= command.ExecuteNonQuery();
                        done1 = (count > 0) ? true : false;
                        command.CommandText =
                            "UPDATE MISC_RANK_DETAILS SET IREC_NO= @iIREC_NO ,ISEQ_NO= @iISEQ_NO  Where CTeam = @cTeam and CLEAGUEALIAS=@cLEAGUEALIAS";
                        command.Parameters.AddWithValue("@iIREC_NO", sHOSTORYRANK);
                        command.Parameters.AddWithValue("@iISEQ_NO", sSCORE);
                        command.Parameters.AddWithValue("@cTeam", sTeam);
                        command.Parameters.AddWithValue("@cLEAGUEALIAS", sLeagueAlias);
                       count= command.ExecuteNonQuery();
                        done2 = (count > 0) ? true : false;
                        connection.Close();
                        this.dgRankDetails.EditItemIndex = -1;
                        BindRankDetails(dplLeague.SelectedValue);
                        //  lbMsg.Text = "Success";
                        if (lbMsg.Text == "") { lbMsg.Text = strConn + " Success ;  "; } else { lbMsg.Text += strConn + " Success ;  "; }
               
                        lbMsg.UpdateAfterCallBack = true;

                        Files.CicsWriteLog(DateTime.Now.ToString("HH:mm:ss") + " " + strConn + ": Update IHOSTORYRANK=" + sHOSTORYRANK +
                                           ", ISEQ_NO =" + sSCORE + " and CTIMESTAMP=" + sUpdateTime + " where CLEAGUEALIAS=" + sLeagueAlias +
                                           " and  CTeam =" + sTeam + " and IHOSTORYRANK=" + strRank + ", IHOSTORYSCORE=" + strScore
                                           + " on MISC_RANK_HISTORY_DETAILS and MISC_RANK_DETAILS " + (done1 ? " " : ",but not found on MISC_RANK_HISTORY_DETAILS,") + (done2 ? " " : ",but not found on MISC_RANK_DETAILS.") + "\r\n");//" on 2 tables.");
                    }
                }
            }
            catch (Exception exp)
            {
                Files.CicsWriteLog(DateTime.Now.ToString("HH:mm:ss") + " " + strConn + ":[Failure], Update IHOSTORYRANK=" + sHOSTORYRANK +
                                           ", ISEQ_NO =" + sSCORE + " and CTIMESTAMP=" + sUpdateTime + " where CLEAGUEALIAS=" + sLeagueAlias +
                                           " and  CTeam =" + sTeam + " and IHOSTORYRANK=" + strRank + ", IHOSTORYSCORE=" + strScore
                                           + " on MISC_RANK_HISTORY_DETAILS and MISC_RANK_DETAILS." + "\r\n");//" on 2 tables.");
                //  lbMsg.Text = "Failure";
                if (lbMsg.Text == "") { lbMsg.Text = strConn + " Failure ;  "; } else { lbMsg.Text += strConn + " Failure ;  " + "\r\n"; }
               
                lbMsg.UpdateAfterCallBack = true;
                string exps = exp.ToString();
                Files.CicsWriteError(DateTime.Now.ToString("HH:mm:ss") + " " + strConn + ":  dgSchedule_UpdateCommand()  " + exps + "\r\n");
            }
        }

        private void dplLeague_SelectedIndexChanged(object sender, EventArgs e)
        {
            dgRankDetails.CurrentPageIndex = 0;
            BindRankDetails(dplLeague.SelectedValue);
            btnEdit.Text = "Edit";
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
                int index1 = AppFlag.CENTASMSINTEConn.IndexOf("DataSource=");
                int index2 = AppFlag.CENTASMSINTEConn.IndexOf(";", index1);
                string ip1 = "";
                if (index1 > 0)
                {
                     ip1 = AppFlag.CENTASMSINTEConn.Substring(index1 + "DataSource=".Length, index2 - index1 - +"DataSource=".Length);
                    this.cblIP.Items.Add(ip1);
                    this.cblIP.Items[0].Selected = true;
                }
                index1 = AppFlag.CENTASMSMAININTEConn.IndexOf("DataSource=");
                if (index1 > 0)
                {
                    index2 = AppFlag.CENTASMSMAININTEConn.IndexOf(";", index1);
                    ip1 = AppFlag.CENTASMSMAININTEConn.Substring(index1 + "DataSource=".Length, index2 - index1 - +"DataSource=".Length);
                    this.cblIP.Items.Add(ip1);
                    this.cblIP.Items[1].Selected = true;
                }

                this.lbUser.Text = this.Context.User.Identity.Name;
                BindLeague();
                BindRankDetails(dplLeague.SelectedValue);
            }
        }

        private void BindLeague()
        {
            try
            {
                using (FbConnection connection =
                    new FbConnection(AppFlag.CENTASMSINTEConn))
                {
                    FbDataAdapter adapter = new FbDataAdapter();
                    adapter.TableMappings.Add("Table", "allLeauge");
                    connection.Open();
                    FbCommand command = new FbCommand(
                        "SELECT distinct a.CLEAGUEALIAS FROM MISC_RANK_HISTORY_DETAILS a;",
                        connection);
                    command.CommandType = CommandType.Text;
                    adapter.SelectCommand = command;
                    DataSet dataSet = new DataSet("allLeauge");
                    adapter.Fill(dataSet);
                    connection.Close();
                    dplLeague.DataSource = dataSet.Tables[0].DefaultView;
                    dplLeague.DataTextField = "CLEAGUEALIAS";
                    dplLeague.DataValueField = "CLEAGUEALIAS";
                    dplLeague.DataBind();
                    dplLeague.Items.Insert(0, new ListItem("--Select--", "-1"));
                    this.dplLeague.UpdateAfterCallBack = true;
                }
            }
            catch (Exception exp)
            {
                string exps = exp.ToString();
                Files.CicsWriteError(DateTime.Now.ToString("HH:mm:ss") + "   BindLeague()  " + exps);
            }
        }

        private void BindRankDetails(string id)
        {
            //if (this.Session["rankData"] != null)
            //{
            //    DataSet data = (DataSet)this.Session["rankData"];
            //    this.dgRankDetails.DataSource = data.Tables["RankDetails"].DefaultView;
            //    this.dgRankDetails.PageSize = AppFlag.iPageSize;
            //    this.dgRankDetails.DataBind();
            //    this.dgRankDetails.UpdateAfterCallBack = true; 
            //}
            //else
            //{ 
            try
            {
                string connectionString = AppFlag.CENTASMSINTEConn;

                using (FbConnection connection =
                    new FbConnection(connectionString))
                {
                    FbDataAdapter adapter = new FbDataAdapter();
                    adapter.TableMappings.Add("Table", "RankDetails");
                    connection.Open();
                    FbCommand command = new FbCommand(
                        "SELECT * FROM MISC_RANK_HISTORY_DETAILS where CLEAGUEALIAS='" + id + "' order by IRANK asc ",
                        connection);
                    command.CommandType = CommandType.Text;
                    adapter.SelectCommand = command;
                    DataSet dataSet = new DataSet("RankDetails");
                    adapter.Fill(dataSet);
                    dgRankDetails.DataSource = dataSet.Tables[0].DefaultView;
                    dgRankDetails.DataBind();
                    dgRankDetails.UpdateAfterCallBack = true;
                    connection.Close();
                    this.Session["rankData"] = dataSet;
                }
            }
            catch (Exception exp)
            {
                string exps = exp.ToString();
                Files.CicsWriteError(DateTime.Now.ToString("HH:mm:ss") + "   BindRankDetails(" + id + ")  " + exps);
            }
            //}
        }
    }
}

