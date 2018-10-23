using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using FileLog;
using System.Data;
using FirebirdSql.Data.FirebirdClient;
using System.Drawing;
using System.Collections;
using Microsoft.VisualBasic.CompilerServices;
using System.Xml.Linq;

namespace DataOfScouts
{
    public partial class DataOfScouts : Form
    {
        string strToken = "";
        OAuthClient clientTest = new OAuthClient();

        public DataOfScouts()
        {
            InitializeComponent();
            var label = new ToolStripLabel();
            label.Text = "                        From ";
            bnAreas.Items.Insert(16, label);

            var picker = new DateTimePicker();
            picker.Name = "dtpStartTime";
            picker.Format = DateTimePickerFormat.Custom;
         //   picker.CustomFormat = "yyyy/MM/dd";
            picker.Value = DateTime.Now.AddDays(0 - AppFlag.iQueryDays-10);
            picker.Width = 80;
            var host = new ToolStripControlHost(picker);
            host.Name = "dtpTime";
            bnAreas.Items.Insert(17, host);

            label = new ToolStripLabel();
            label.Text = " To ";
            bnAreas.Items.Insert(18, label);

            picker = new DateTimePicker();
            picker.Name = "dtpEndTime";
            picker.Format = DateTimePickerFormat.Custom;
            picker.Value = DateTime.Now.AddDays(0 -10);
            // picker.CustomFormat = "yyyy/MM/dd";
            picker.Width = 80;
            host = new ToolStripControlHost(picker);
            bnAreas.Items.Insert(19, host);

            label = new ToolStripLabel();
            label.Text = "  ";
            bnAreas.Items.Insert(20, label);

            var dplStatus = new ToolStripDropDownButton();
            string[] status = { "Not started", "Cancelled", "Abandoned", "Halftime", "Finished", "Extratime", "Interrupted" };
            for (int i = 0; i < status.Length; i++)
            {
                dplStatus.DropDownItems.Add(status[i]);
            }
            dplStatus.Text = "All";
            dplStatus.AutoSize  = false;
            dplStatus.Width = 100; 
            bnAreas.Items.Insert(21, dplStatus);

            label = new ToolStripLabel();
            label.Text = "  ";
            bnAreas.Items.Insert(22, label);

            ClientAuthorize();

            this.bnAreas2.Visible = false;
            this.bnAreas.Visible = false;

            this.tsdAreaParentId.DropDownItems.Clear();
            DataSet ds1 = InsertData("areas", "all");
            DataTable dtnew = ds1.Tables[0].DefaultView.ToTable("PARENTID", true, new string[] { "PARENT_AREA_ID" });
            foreach (DataRow dr in dtnew.Rows)
            {
                this.tsdAreaParentId.DropDownItems.Add(dr[0].ToString());
            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedTab == tpAuthorization)
            {
                this.bnAreas2.Visible = false;
                this.bnAreas.Visible = false;
            }
            else if (tabControl1.SelectedTab == tpAreas)
            {
                this.bnAreas2.Visible = true;
                this.bnAreas.Visible = true;

                tsbGet.Visible = true;
                tsdAreaParentId.Visible = false;
                tslArea.Visible = true;
                tsbArea.Visible = true;
                tsdArea.Visible = false;
                tsdComp.Visible = false;
                tsdSeason.Visible = false;
                tsdStage.Visible = false;
                tsdGroup.Visible = false;
                tsdPartic.Visible = false;
                tsdEvent.Visible = false;

                //ToolStripItem [] s=  bnAreas2.Items.Find("dtpTime", false);
                //s[0].Visible =false;
                ////bnAreas2.Items[11].Visible = false;
                ////bnAreas2.Items[12].Visible = false;
                ////bnAreas2.Items[13].Visible = false;
                ////bnAreas2.Items[14].Visible = false;
                ////bnAreas2.Items[16].Visible = false;
                ////bnAreas2.Items[17].Visible = false;
                //var responseValue = clientTest.GetAccessData(strToken, "areas");
                //var strResponseValue = responseValue.Result;
                /*   DataSet ds = InsertData("areas");
                   //this.dgvAreas.DataSource = ds.Tables[0].DefaultView;
                   tbData = ds.Tables[0];

                   BindingSource bs = new BindingSource();
                   bs.DataSource = tbData.DefaultView;
                   bnAreas.BindingSource = bs;
                   this.dgvAreas.DataSource = bs;
                   */
                //total = ds.Tables[0].Rows.Count;
                //pageCount = (total / AppFlag.iPageSize);
                //if ((total % AppFlag.iPageSize > 0))
                //{
                //    pageCount++;
                //}
                //pageCurrent = 1;
                //currentRow = 0;

                //this.LoadData();

                //this.tsdAreaParentId.DropDownItems.Clear();
                //DataSet ds1 = InsertData("areas", "all");
                //DataTable dtnew = ds1.Tables[0].DefaultView.ToTable("PARENTID", true, new string[] { "PARENT_AREA_ID" });
                //foreach (DataRow dr in dtnew.Rows)
                //{
                //    this.tsdAreaParentId.DropDownItems.Add(dr[0].ToString());
                //} 

                this.tsbGet_Click(sender, e);
            }
            else if (tabControl1.SelectedTab == tpCompetitions)
            {
                tsbGet.Visible = false;
                tsdAreaParentId.Visible = true;
                tsbArea.Visible = false;
                tslArea.Visible = false;
                tsdArea.Visible = true;
                tsdComp.Visible = false;
                tsdSeason.Visible = false;
                tsdStage.Visible = false;
                tsdGroup.Visible = false;
                tsdPartic.Visible = false;
                tsdEvent.Visible = false;
                ////bnAreas2.Items[11].Visible = false;
                ////bnAreas2.Items[12].Visible = false;
                ////bnAreas2.Items[13].Visible = false;
                ////bnAreas2.Items[14].Visible = false;
                ////bnAreas2.Items[16].Visible = false;
                ////bnAreas2.Items[17].Visible = false;
                this.bnAreas2.Visible = true;
                this.bnAreas.Visible = true;

                //DataSet ds= InsertData("areas","all");
                //foreach (DataRow dr in ds.Tables[0].Rows)
                //{
                //    this.tsdAreaParentId.DropDownItems.Add(dr["PARENT_AREA_ID"]);
                //}


                /* DataSet ds = new DataSet();

                 //var responseValue = clientTest.GetAccessData(strToken, "competitions/");
                 //var strResponseValue = responseValue.Result;
                 ds = InsertData("competitions");
                 // this.dgvComp.DataSource = ds.Tables[0].DefaultView;
                 tbData = ds.Tables[0];

                 BindingSource bs = new BindingSource();
                 bs.DataSource = tbData.DefaultView;
                 bnAreas.BindingSource = bs;
                 this.dgvComp.DataSource = bs; */

                this.tsbGet_Click(sender, e);
            }
            else if (tabControl1.SelectedTab == tpSeasons)
            {
                this.bnAreas2.Visible = true;
                this.bnAreas.Visible = true;
                tsbGet.Visible = false;
                tsdAreaParentId.Visible = true;
                tsbArea.Visible = false;
                tslArea.Visible = false;
                tsdArea.Visible = true;
                tsdComp.Visible = true;
                tsdSeason.Visible = false;
                tsdStage.Visible = false;
                tsdGroup.Visible = false;
                tsdPartic.Visible = false;
                tsdEvent.Visible = false;
                ////bnAreas2.Items[11].Visible = false;
                ////bnAreas2.Items[12].Visible = false;
                ////bnAreas2.Items[13].Visible = false;
                ////bnAreas2.Items[14].Visible = false;
                ////bnAreas2.Items[16].Visible = false;
                ////bnAreas2.Items[17].Visible = false;
                this.tsbGet_Click(sender, e);

                /*
                                DataSet ds = new DataSet();

                                //var responseValue = clientTest.GetAccessData(strToken, "seasons/");
                                //var strResponseValue = responseValue.Result;
                                ds = InsertData("seasons");
                                //  this.dgvComp.DataSource = ds.Tables[0].DefaultView;
                                tbData = ds.Tables[0];

                                BindingSource bs = new BindingSource();
                                bs.DataSource = tbData.DefaultView;
                                bnAreas.BindingSource = bs;
                                this.dgvSeasons.DataSource = bs;
                                */
            }
            else if (tabControl1.SelectedTab == tpStages)
            {
                this.bnAreas2.Visible = true;
                this.bnAreas.Visible = true;
                tsbGet.Visible = false;
                tsdAreaParentId.Visible = true;
                tsbArea.Visible = false;
                tslArea.Visible = false;
                tsdArea.Visible = true;
                tsdComp.Visible = true;
                tsdSeason.Visible = true;
                tsdStage.Visible = false;
                tsdGroup.Visible = false;
                tsdPartic.Visible = false;
                tsdEvent.Visible = false;
                ////bnAreas2.Items[11].Visible = false;
                ////bnAreas2.Items[12].Visible = false;
                ////bnAreas2.Items[13].Visible = false;
                ////bnAreas2.Items[14].Visible = false;
                ////bnAreas2.Items[16].Visible = false;
                ////bnAreas2.Items[17].Visible = false;
                //var responseValue = clientTest.GetAccessData(strToken, "stages/" + "1", "29543");
                //var strResponseValue = responseValue.Result;

                //TableGenerator.TableGenerators(typeof(DOSStages.apiDataCompetitionSeasonStage));

                //  responseValue = clientTest.GetAccessData(strToken, "groups/" + "1", "86215");
                //  strResponseValue = responseValue.Result;

                //TableGenerator.TableGenerators(typeof(DOSGroups.apiDataCompetitionSeasonStageGroup));

                //return;


                this.tsbGet_Click(sender, e);
            }
            else if (tabControl1.SelectedTab == tpGroups)
            {
                this.bnAreas2.Visible = true;
                this.bnAreas.Visible = true;
                tsbGet.Visible = false;
                tsdAreaParentId.Visible = true;
                tsbArea.Visible = false;
                tslArea.Visible = false;
                tsdArea.Visible = true;
                tsdComp.Visible = true;
                tsdSeason.Visible = true;
                tsdStage.Visible = true;
                tsdGroup.Visible = false;
                tsdPartic.Visible = false;
                tsdEvent.Visible = false;
                ////bnAreas2.Items[11].Visible = false;
                ////bnAreas2.Items[12].Visible = false;
                ////bnAreas2.Items[13].Visible = false;
                ////bnAreas2.Items[14].Visible = false;
                ////bnAreas2.Items[16].Visible = false;
                ////bnAreas2.Items[17].Visible = false;
                this.tsbGet_Click(sender, e);
            }
            else if (tabControl1.SelectedTab == tpPlayer || tabControl1.SelectedTab == tpTeam)
            {
                this.bnAreas2.Visible = true;
                this.bnAreas.Visible = true;
                tsbGet.Visible = false;
                tsdAreaParentId.Visible = true;
                tsbArea.Visible = false;
                tslArea.Visible = false;
                tsdArea.Visible = true;
                tsdComp.Visible = true;
                tsdSeason.Visible = true;
                tsdStage.Visible = false;
                tsdGroup.Visible = false;
                tsdPartic.Visible = false;
                tsdEvent.Visible = false;

                ////bnAreas2.Items[11].Visible = false;
                ////bnAreas2.Items[12].Visible = false;
                ////bnAreas2.Items[13].Visible = false;
                ////bnAreas2.Items[14].Visible = false;
                ////bnAreas2.Items[16].Visible = false;
                ////bnAreas2.Items[17].Visible = false;

                this.tsbGet_Click(sender, e);
            }

            //else if (tabControl1.SelectedTab == tpParticipants)
            //{ 
            //    tsdAreaParentId.Visible = true;
            //    tsbArea.Visible = false;
            //    tslArea.Visible = false;
            //    tsdArea.Visible = true;
            //    tsdComp.Visible = true;
            //    tsdSeason.Visible = true;
            //    tsdStage.Visible = true;
            //    tsdGroup.Visible = true;
            //    tsdPartic.Visible = false;
            //    tsdEvent.Visible = false;

            //    this.bnAreas2.Visible = true;
            //    this.bnAreas.Visible = true;

            //    DataSet ds = new DataSet();
            //    ds = InsertData("participants");
            //    //  this.dgvPart.DataSource = ds.Tables[0].DefaultView;
            //    tbData = ds.Tables[0];

            //    BindingSource bs = new BindingSource();
            //    bs.DataSource = tbData.DefaultView;
            //    bnAreas.BindingSource = bs;
            //    this.dgvPart.DataSource = bs;
            //}
            else if (tabControl1.SelectedTab == tpEvent)
            {
                this.bnAreas2.Visible = true;
                this.bnAreas.Visible = true;
                tsbGet.Visible = false;
                tsdAreaParentId.Visible = true;
                tsbArea.Visible = false;
                tslArea.Visible = false;
                tsdArea.Visible = true;
                tsdComp.Visible = true;
                tsdSeason.Visible = true;
                tsdStage.Visible = true;
                tsdGroup.Visible = true;
                tsdPartic.Visible = false;
                tsdEvent.Visible = false;
                ////bnAreas2.Items[11].Visible = true;
                ////bnAreas2.Items[12].Visible = true;
                ////bnAreas2.Items[13].Visible = true;
                ////bnAreas2.Items[14].Visible = true;
                ////bnAreas2.Items[16].Visible = true;
                ////bnAreas2.Items[17].Visible = true;
                //tsbGet.Visible = false;
                //tsdArea.Visible = true;
                //tsdComp.Visible = true;
                //tsdAreaParentId.Visible = true;
                //  this.tsbGet_Click(sender, e);
                this.tsbGet_Click(null , null);

                //  //var responseValue = clientTest.GetAccessData(strToken, "events/" + "1");
                //  //var strResponseValue = responseValue.Result;

                //  //TableGenerator.TableGenerators(typeof(DOSEvents.apiDataCompetitionSeasonStageGroupEvent));
                //// return;

                //  //string strName = "events" + "-" + "1" + " " + DateTime.Now.ToString("HHmmss");
                //  //Files.WriteXml(strName, strResponseValue);
                //  //this.bnAreas.Visible = true;
                //  DataSet ds = new DataSet();

                //  ds = InsertData("events");
                //  //  this.dgvEvent.DataSource = ds.Tables[0].DefaultView;
                //  tbData = ds.Tables[0];

                //  BindingSource bs = new BindingSource();
                //  bs.DataSource = tbData.DefaultView;
                //  bnAreas.BindingSource = bs;
                //  this.dgvEvent.DataSource = bs;
            }
            else if (tabControl1.SelectedTab == tpBook )
            {
                this.bnAreas2.Visible = true;
                this.bnAreas.Visible = true;
                tsbGet.Visible = false;
                tsdAreaParentId.Visible = true;
                tsbArea.Visible = false;
                tslArea.Visible = false;
                tsdArea.Visible = true;
                tsdComp.Visible = true;
                tsdSeason.Visible = true;
                tsdStage.Visible = true;
                tsdGroup.Visible = true;
                tsdPartic.Visible = false;
                tsdEvent.Visible = false;

                this.tsbGet_Click(null, null);
            }
            /*
            total = tbData.Rows.Count;
            pageCount = (total / AppFlag.iPageSize);
            if ((total % AppFlag.iPageSize > 0))
            {
                pageCount++;
            }
            pageCurrent = 1;
            currentRow = 0;

            this.LoadData(tabControl1.SelectedTab.Text);*/
        }

        int total = 0;
        int pageCount = 0;//总页数 
        int pageCurrent = 0;
        int currentRow = 0;//当前记录数从0开始 
        int nStartPos = 0;
        int nEndPos = 0;

        DataTable tbData = null;
        private void LoadData(string type)
        {
            int nStartPos = 0;
            int nEndPos = 0;
            DataTable dtTemp = tbData.Clone();
            if (pageCurrent == pageCount)
            {
                nEndPos = total;
            }
            else
            {
                nEndPos = AppFlag.iPageSize * pageCurrent;
            }
            nStartPos = currentRow;

            nbAreasPage.Text = "of " + pageCount.ToString();
            // bindingNavigatorCountItem.Text = "of " + pageCount.ToString();
            if (tbData.Rows.Count == 0)
            {
                nbAreasCurrent.Text = "0";
            }
            else
            {
                nbAreasCurrent.Text = Convert.ToString(pageCurrent);
            }
            this.nbAreasTotal.Text = total.ToString();

            if (tbData.Rows.Count != 0)
            {
                for (int i = nStartPos; i < nEndPos; i++)
                {
                    dtTemp.ImportRow(tbData.Rows[i]);
                    currentRow++;
                }
            }

            bindingSource1.DataSource = dtTemp;
            bnAreas.BindingSource = bindingSource1;
            if (type == "areas")
            {
                dgvAreas.DataSource = bindingSource1;
            }
            else if (type == "competitions")
            {
                dgvComp.DataSource = bindingSource1;
            }
            else if (type == "seasons")
            {
                dgvSeasons.DataSource = bindingSource1;
            }
            else if (type == "events")
            {
                dgvEvent.DataSource = bindingSource1;
            }
            //else if (type == "participants")
            //{
            //    dgvPart.DataSource = bindingSource1;
            //}
            else if (type == "team")
            {
                dgvTeam.DataSource = bindingSource1;
            }
            else if (type == "player")
            {
                dgvPlayer.DataSource = bindingSource1;
            }
            else if (type == "booked-events")
            {
                dgvBookedEvent.DataSource = bindingSource1;
            }
        }

        private void tabControl1_Selected(object sender, TabControlEventArgs e)
        {
         //   string str = "";
        }

        private void ClientAuthorize()
        {
            var responseValue = clientTest.GetAccessToken();
            var strResponseValue = responseValue.Result;
            var api = JObject.Parse(strResponseValue)["api"];
            var data = JObject.Parse(api.ToString())["data"];
            if (data == null)
            {
                this.lbAuthorization.Text = "Unauthorized";
                this.label1.Text = strResponseValue;
                return;
            }
            strToken = JObject.Parse(data.ToString())["token"].Value<string>();
            this.lbAuthorization.Text = "Authorized";
            this.lbToken.Text = strToken;
            clientTest.token = strToken;

            Files.WriteLog("[Success] Authorized: " + strToken);

        }
        // private DataSet InsertData(int iPage, string responsValue, string type)
        //private DataSet InsertData(string type)
        private DataSet InsertData(string type, params object[] arr)
        {
            //string strName = type + "-" + iPage + " " + DateTime.Now.ToString("HHmmss");
            //Files.WriteXml(strName, responsValue);
            //string queryString = "select FIRST 1 * from " + type;
            string queryString = "";
            DateTime cTimestamp = DateTime.Now;
            DataSet ds = new DataSet();
            int count = 0;

            switch (type)
            {
                case "areas":
                    {  
                        using (FbConnection connection = new FbConnection(AppFlag.ScoutsDBConn))
                        {
                            queryString = "select * from " + type + (arr[0].ToString().ToLower() == "all" ? "" : " where PARENT_AREA_ID=" + arr[0]);
                            FbDataAdapter adapter = new FbDataAdapter();
                            adapter.SelectCommand = new FbCommand(queryString, connection);
                            FbCommandBuilder builder = new FbCommandBuilder(adapter);
                            connection.Open();
                            DataSet areasDs = new DataSet();
                            adapter.Fill(areasDs);

                            //using (FbCommand cmd = new FbCommand())
                            //{
                            //    cmd.CommandText = "ADD_AREA7";
                            //    cmd.CommandType = CommandType.StoredProcedure;
                            //    cmd.Connection = connection;
                            //    cmd.Parameters.Add("@ID", "904");
                            //    cmd.Parameters.Add("@AREA_CODE", "CODE1");
                            //    cmd.Parameters.Add("@NAME", "NAME1");
                            //    cmd.Parameters.Add("@PARENT_AREA_ID", "905");
                            //    cmd.Parameters.Add("@UT", "1537427316");
                            //    cmd.Parameters.Add("@CTIMESTAMP", cTimestamp);
                            //    int id = Convert.ToInt32(cmd.ExecuteScalar());
                            //}

                            if (areasDs.Tables[0].Rows.Count == 0)
                            {
                                var responseValue = clientTest.GetAccessData(strToken, "areas", arr[0]);
                                var strResponseValue = responseValue.Result;
                                if (strResponseValue == "Unauthorized") { MessageBox.Show("Unauthorized!"); break; }

                                string strName = type + "-" + DateTime.Now.ToString("HHmmss");
                                Files.WriteXml(strName, strResponseValue);

                                DOSAreas.api apis = XmlUtil.Deserialize(typeof(DOSAreas.api), strResponseValue) as DOSAreas.api;
                                if (apis.data.Length == 0) return ds;
                                DOSAreas.apiDataAreasArea[] areas = apis.data[0];
                                if (areas == null) return ds;


                                //code to modify data in dataset here
                                foreach (DOSAreas.apiDataAreasArea area in areas)
                                {
                                    DataRow dr = areasDs.Tables[0].NewRow();
                                    dr[0] = area.id;
                                    dr[1] = area.area_code;
                                    dr[2] = area.name;
                                    dr[3] = area.parent_area_id;
                                    dr[4] = area.ut;
                                    dr[5] = cTimestamp;
                                    areasDs.Tables[0].Rows.Add(dr);
                                }

                                count = adapter.Update(areasDs);

                                if (count > -1)
                                {
                                    Console.WriteLine("[Success] Insert areas [" + count + "/" + areas.Length + "]");
                                    Files.WriteLog("[Success] Insert areas[" + count + " / " + areas.Length + "] " + strName + ".xml");
                                }
                                else
                                {
                                    Console.WriteLine("[Failure] Insert areas [" + areas.Length + "]");
                                    Files.WriteLog("[Failure] Insert areas [" + areas.Length + "] " + strName + ".xml");
                                }
                            }
                            else
                            {
                                //count = 0;
                                //areasDs.Clear();
                                //queryString = "select   * from " + type;
                                //adapter.SelectCommand = new FbCommand(queryString, connection);
                                //adapter.Fill(areasDs); 
                                //ds = areasDs;
                            }
                            ds = areasDs;
                            connection.Close();
                        }
                        break;
                    }
                case "competitions":
                    {
                        //var responseValue = clientTest.GetAccessData(strToken, "competitions/" + i);
                        //var strResponseValue = responseValue.Result;

                        //DOSCompetitions.api apis = XmlUtil.Deserialize(typeof(DOSCompetitions.api), responsValue) as DOSCompetitions.api;
                        //DOSCompetitions.apiDataCompetitionsCompetition[] competitions = (apis.data.Length == 0) ? null : apis.data[0];
                        //if (competitions == null) return ds;
                        using (FbConnection connection = new FbConnection(AppFlag.ScoutsDBConn))
                        {
                            queryString = "select * from " + type + " where AREA_ID=" + arr[0];

                            FbDataAdapter adapter = new FbDataAdapter();
                            adapter.SelectCommand = new FbCommand(queryString, connection);
                            FbCommandBuilder builder = new FbCommandBuilder(adapter);
                            connection.Open();
                            DataSet competitionDs = new DataSet();
                            adapter.Fill(competitionDs);

                            if (competitionDs.Tables[0].Rows.Count == 0)// && iPage == 1)
                            {
                                for (int i = 1; i < 4; i++)
                                {
                                    //if (competitionDs.Tables[0].Rows.Count == 0)// && iPage == 1)
                                    //{
                                    var responseValue = clientTest.GetAccessData(strToken, "competitions/" + i, arr);
                                    var strResponseValue = responseValue.Result;
                                    if (strResponseValue == "Unauthorized") { MessageBox.Show("Unauthorized!"); break; }

                                    string strName = type + "-" + i + " " + DateTime.Now.ToString("HHmmss");
                                    Files.WriteXml(strName, strResponseValue);

                                    DOSCompetitions.api apis = XmlUtil.Deserialize(typeof(DOSCompetitions.api), strResponseValue) as DOSCompetitions.api;
                                    if (apis == null) break;
                                    DOSCompetitions.apiDataCompetitionsCompetition[] competitions = (apis.data.Length == 0) ? null : apis.data[0];
                                    if (competitions == null) break;

                                    foreach (DOSCompetitions.apiDataCompetitionsCompetition competition in competitions)
                                    {
                                        DataRow dr = competitionDs.Tables[0].NewRow();
                                        dr[0] = competition.id;
                                        dr[1] = competition.name;
                                        dr[2] = competition.short_name;
                                        dr[3] = competition.mini_name;
                                        dr[4] = (competition.gender.ToLower() == "male") ? true : false;
                                        dr[5] = competition.type;
                                        dr[6] = competition.area_id;
                                        dr[7] = competition.area_name;
                                        dr[8] = competition.area_type;
                                        dr[9] = competition.area_sort;
                                        dr[10] = competition.area_code;
                                        dr[11] = competition.overall_sort;
                                        dr[12] = competition.sport_id;
                                        dr[13] = competition.sport_name;
                                        dr[14] = (competition.tour_id == "") ? "-1" : competition.tour_id;
                                        dr[15] = competition.tour_name;
                                        dr[16] = competition.ut;
                                        dr[17] = (competition.old_competition_id == "") ? "-1" : competition.old_competition_id;
                                        dr[18] = competition.slug;
                                        dr[19] = cTimestamp;
                                        competitionDs.Tables[0].Rows.Add(dr);
                                    }
                                    count = adapter.Update(competitionDs);
                                    ds.Merge(competitionDs, true, MissingSchemaAction.AddWithKey);
                                    competitionDs.Clear();

                                    if (count > -1)
                                    {
                                        Console.WriteLine("[Success] Insert competitions [" + count + "/" + competitions.Length + "]");
                                        Files.WriteLog("[Success] Insert competitions [" + competitions.Length + "] " + strName + ".xml");
                                    }
                                    else
                                    {
                                        Console.WriteLine("[Failure] Insert competitions [" + competitions.Length + "]");
                                        Files.WriteLog("[Failure] Insert competitions [" + competitions.Length + "] " + strName + ".xml");
                                    }
                                    //}
                                    //else
                                    //{
                                    //    count = -1;
                                    //    queryString = "select   * from " + type;
                                    //    adapter.SelectCommand = new FbCommand(queryString, connection);
                                    //    adapter.Fill(competitionDs);
                                    //    ds = competitionDs;
                                    //    break;
                                    //}
                                }
                            }
                            else
                            {
                                //count = -1;
                                //queryString = "select   * from " + type;
                                //adapter.SelectCommand = new FbCommand(queryString, connection);
                                //adapter.Fill(competitionDs);
                                ds = competitionDs;
                            }
                            connection.Close();
                        }
                        break;
                    }
                case "seasons":
                    {
                        //var responseValue = clientTest.GetAccessData(strToken, "seasons");
                        //var strResponseValue = responseValue.Result; 
                        //DOSSeasons.api apis = XmlUtil.Deserialize(typeof(DOSSeasons.api), strResponseValue) as DOSSeasons.api;
                        //DOSSeasons.apiDataCompetitionsCompetition[] competitions = (apis.data.Length == 0) ? null : apis.data[0];
                        //if (competitions == null) return ds;

                        //string strName = type + "-" + iPage + " " + DateTime.Now.ToString("HHmmss");
                        //Files.WriteXml(strName, responsValue);

                        using (FbConnection connection = new FbConnection(AppFlag.ScoutsDBConn))
                        {
                            queryString = "select * from " + type + " where COMPETITION_ID=" + arr[0];

                            FbDataAdapter adapter = new FbDataAdapter();
                            adapter.SelectCommand = new FbCommand(queryString, connection);
                            FbCommandBuilder builder = new FbCommandBuilder(adapter);
                            connection.Open();
                            DataSet seasonsDs = new DataSet();
                            adapter.Fill(seasonsDs);

                            if (seasonsDs.Tables[0].Rows.Count == 0)
                            {
                                for (int i = 1; i < 4; i++)
                                {
                                    var responseValue = clientTest.GetAccessData(strToken, "seasons/" + i, arr);
                                    var strResponseValue = responseValue.Result;
                                    //DOSSeasons.api apis = XmlUtil.Deserialize(typeof(DOSSeasons.api), strResponseValue) as DOSSeasons.api;
                                    //DOSSeasons.apiDataCompetitionsCompetition[] competitions = (apis.data.Length == 0) ? null : apis.data[0];
                                    // if (competitions == null) return ds;
                                    if (strResponseValue == "Unauthorized") { MessageBox.Show("Unauthorized!"); break; }
                                    string strName = type + "-" + i + " " + DateTime.Now.ToString("HHmmss");
                                    Files.WriteXml(strName, strResponseValue);

                                    DOSSeasons.api apis = XmlUtil.Deserialize(typeof(DOSSeasons.api), strResponseValue) as DOSSeasons.api;
                                    DOSSeasons.apiDataCompetitionsCompetition[] competitions = (apis.data.Length == 0) ? null : apis.data[0];
                                    if (competitions == null) break;

                                    foreach (DOSSeasons.apiDataCompetitionsCompetition competition in competitions)
                                    {
                                        string strCompetition_id = competition.id;
                                        DOSSeasons.apiDataCompetitionSeason[] seasons = competition.seasons;
                                        if (seasons == null) continue;
                                        foreach (DOSSeasons.apiDataCompetitionSeason season in seasons)
                                        {
                                            DataRow dr = seasonsDs.Tables[0].NewRow();
                                            dr[0] = season.id;
                                            dr[1] = season.name;
                                            dr[2] = strCompetition_id;
                                            dr[3] = season.year;
                                            dr[4] = season.actual;
                                            dr[5] = season.ut;
                                            dr[6] = (season.old_season_id == "") ? "-1" : season.old_season_id;
                                            dr[7] = season.range;
                                            dr[8] = cTimestamp;
                                            seasonsDs.Tables[0].Rows.Add(dr);
                                        }
                                    }
                                    count = adapter.Update(seasonsDs);
                                    ds.Merge(seasonsDs, true, MissingSchemaAction.AddWithKey);
                                    seasonsDs.Clear();

                                    if (count > -1)
                                    {
                                        Console.WriteLine("[Success] Insert seasons [" + count + " " + "]");
                                        Files.WriteLog("[Success] Insert seasons[" + count + "  " + "] " + " " + strName + ".xml");
                                    }
                                    else
                                    {
                                        Console.WriteLine("[Failure] Insert seasons [  ]");
                                        Files.WriteLog("[Failure] Insert seasons [  ]" + " " + strName + ".xml");
                                    }
                                }
                            }
                            else
                            {
                                //count = -1;
                                //queryString = "select   * from " + type;
                                //adapter.SelectCommand = new FbCommand(queryString, connection);
                                //adapter.Fill(seasonsDs);
                                ds = seasonsDs;
                            }
                            connection.Close();
                        }
                        break;
                    }

                case "stages":
                    {
                        using (FbConnection connection = new FbConnection(AppFlag.ScoutsDBConn))
                        {
                            queryString = "select * from " + type + " where season_id=" + arr[0];

                            FbDataAdapter adapter = new FbDataAdapter();
                            adapter.SelectCommand = new FbCommand(queryString, connection);
                            FbCommandBuilder builder = new FbCommandBuilder(adapter);
                            connection.Open();
                            DataSet stagesDs = new DataSet();
                            adapter.Fill(stagesDs);

                            if (stagesDs.Tables[0].Rows.Count == 0)
                            {
                                for (int i = 1; i < 4; i++)
                                {
                                    var responseValue = clientTest.GetAccessData(strToken, "stages/" + i, arr);
                                    var strResponseValue = responseValue.Result;
                                    if (strResponseValue == "Unauthorized") { MessageBox.Show("Unauthorized!"); break; }
                                    string strName = type + "-" + i + " " + DateTime.Now.ToString("HHmmss");
                                    Files.WriteXml(strName, strResponseValue);

                                    DOSStages.api apis = XmlUtil.Deserialize(typeof(DOSStages.api), strResponseValue) as DOSStages.api;
                                    if (apis == null) break;
                                    DOSStages.apiDataCompetition[] competitions = (apis.data.Length == 0) ? null : apis.data;
                                    if (competitions == null) break;

                                    foreach (DOSStages.apiDataCompetition competition in competitions)
                                    {
                                        string strCompetition_id = competition.id;
                                        string strArea_id = competition.area_id;
                                        DOSStages.apiDataCompetitionSeason[] seasons = competition.season;
                                        if (seasons == null) continue;
                                        foreach (DOSStages.apiDataCompetitionSeason season in seasons)
                                        {
                                            string strSeasons_id = season.id;
                                            DOSStages.apiDataCompetitionSeasonStage[] stages = season.stages;
                                            if (stages == null) continue;
                                            foreach (DOSStages.apiDataCompetitionSeasonStage stage in stages)
                                            {
                                                DataRow dr = stagesDs.Tables[0].NewRow();
                                                dr[0] = stage.id;
                                                dr[1] = stage.stage_name_id;
                                                dr[2] = stage.name;
                                                dr[3] = stage.start_date;
                                                dr[4] = stage.end_date;
                                                dr[5] = stage.show_standings == "yes" ? true : false;
                                                dr[6] = stage.groups_nr;
                                                dr[7] = stage.sort;
                                                dr[8] = stage.is_current == "yes" ? true : false;
                                                dr[9] = stage.ut;
                                                dr[10] = (stage.old_stage_id == "") ? "-1" : stage.old_stage_id;
                                                dr[11] = strSeasons_id;
                                                dr[12] = strCompetition_id;
                                                dr[13] = strArea_id;
                                                dr[14] = cTimestamp;
                                                stagesDs.Tables[0].Rows.Add(dr);
                                            }
                                        }
                                    }
                                    count = adapter.Update(stagesDs);
                                    ds.Merge(stagesDs, true, MissingSchemaAction.AddWithKey);
                                    stagesDs.Clear();

                                    if (count > -1)
                                    {
                                        Console.WriteLine("[Success] Insert stages [" + count + " " + "]");
                                        Files.WriteLog("[Success] Insert stages[" + count + "  " + "] " + " " + strName + ".xml");
                                    }
                                    else
                                    {
                                        Console.WriteLine("[Failure] Insert stages [  ]");
                                        Files.WriteLog("[Failure] Insert stages [  ]" + " " + strName + ".xml");
                                    }
                                }
                            }
                            else
                            {
                                ds = stagesDs;
                            }
                            connection.Close();
                        }
                        break;
                    }

                case "groups":
                    {
                        using (FbConnection connection = new FbConnection(AppFlag.ScoutsDBConn))
                        {
                            queryString = "select * from " + type + " where stage_id=" + arr[0];

                            FbDataAdapter adapter = new FbDataAdapter();
                            adapter.SelectCommand = new FbCommand(queryString, connection);
                            FbCommandBuilder builder = new FbCommandBuilder(adapter);
                            connection.Open();
                            DataSet stagesDs = new DataSet();
                            adapter.Fill(stagesDs);

                            if (stagesDs.Tables[0].Rows.Count == 0)
                            {
                                for (int i = 1; i < 4; i++)
                                {
                                    var responseValue = clientTest.GetAccessData(strToken, "groups/" + i, arr);
                                    var strResponseValue = responseValue.Result;
                                    if (strResponseValue == "Unauthorized") { MessageBox.Show("Unauthorized!"); break; }

                                    string strName = type + "-" + i + " " + DateTime.Now.ToString("HHmmss");
                                    Files.WriteXml(strName, strResponseValue);

                                    DOSGroups.api apis = XmlUtil.Deserialize(typeof(DOSGroups.api), strResponseValue) as DOSGroups.api;
                                    // if (apis == null) return ds;
                                    if (apis == null) break;
                                    DOSGroups.apiDataCompetition[] competitions = (apis.data.Length == 0) ? null : apis.data;
                                    if (competitions == null) break;
                                    //string strName = type + "-" + i + " " + DateTime.Now.ToString("HHmmss");
                                    //Files.WriteXml(strName, strResponseValue);

                                    foreach (DOSGroups.apiDataCompetition competition in competitions)
                                    {
                                        string strCompetition_id = competition.id;
                                        string strArea_id = competition.area_id;
                                        DOSGroups.apiDataCompetitionSeason[] seasons = competition.season;
                                        if (seasons == null) continue;
                                        foreach (DOSGroups.apiDataCompetitionSeason season in seasons)
                                        {
                                            string strSeasons_id = season.id;
                                            DOSGroups.apiDataCompetitionSeasonStage[] stages = season.stage;
                                            if (stages == null) continue;
                                            foreach (DOSGroups.apiDataCompetitionSeasonStage stage in stages)
                                            {
                                                string strStage_id = stage.id;
                                                DOSGroups.apiDataCompetitionSeasonStageGroup[] groups = stage.groups;
                                                if (groups == null) continue;
                                                foreach (DOSGroups.apiDataCompetitionSeasonStageGroup group in groups)
                                                {
                                                    DataRow dr = stagesDs.Tables[0].NewRow();
                                                    dr[0] = group.id;
                                                    dr[1] = group.name;
                                                    dr[2] = group.ut;
                                                    dr[3] = strStage_id;
                                                    dr[4] = strSeasons_id;
                                                    dr[5] = strCompetition_id;
                                                    dr[6] = strArea_id;
                                                    dr[7] = cTimestamp;
                                                    stagesDs.Tables[0].Rows.Add(dr);
                                                }
                                            }
                                        }
                                    }
                                    count = adapter.Update(stagesDs);
                                    ds.Merge(stagesDs, true, MissingSchemaAction.AddWithKey);
                                    stagesDs.Clear();

                                    if (count > -1)
                                    {
                                        Console.WriteLine("[Success] Insert groups [" + count + " " + "]");
                                        Files.WriteLog("[Success] Insert groups[" + count + "  " + "] " + " " + strName + ".xml");
                                    }
                                    else
                                    {
                                        Console.WriteLine("[Failure] Insert groups [  ]");
                                        Files.WriteLog("[Failure] Insert groups [  ]" + " " + strName + ".xml");
                                    }
                                }
                            }
                            else
                            {
                                ds = stagesDs;
                            }
                            connection.Close();
                        }
                        break;
                    }
                case "participants":
                    {
                        using (FbConnection connection = new FbConnection(AppFlag.ScoutsDBConn))
                        {
                            queryString = "select * from  teams where season_id=" + arr[1];

                            using (FbCommand cmd = new FbCommand(queryString))
                            {
                                using (FbCommandBuilder fcb = new FbCommandBuilder())
                                {
                                    using (FbDataAdapter fda = new FbDataAdapter())
                                    {
                                        cmd.Connection = connection;
                                        fda.SelectCommand = cmd;
                                        fcb.DataAdapter = fda;
                                        using (DataSet data = new DataSet())
                                        {
                                            data.Tables.Add(new DataTable("teams"));
                                            // fda.TableMappings.Add("teams", "teams");
                                            fda.Fill(data.Tables["teams"]);

                                            data.Tables.Add(new DataTable("players"));
                                            FbDataAdapter adapter1 = new FbDataAdapter();
                                            adapter1.SelectCommand = new FbCommand(" SELECT   * FROM players" + " where season_id=" + arr[1], connection);
                                            FbCommandBuilder builder3 = new FbCommandBuilder(adapter1);
                                            adapter1.Fill(data.Tables["players"]);

                                            if (data.Tables["teams"].Rows.Count == 0 && data.Tables["players"].Rows.Count == 0)
                                            {
                                                for (int i = 1; i < 2869; i++)
                                                {
                                                    var responseValue = clientTest.GetAccessData(strToken, "participants/" + i, arr[0], arr[1]);
                                                    var strResponseValue = responseValue.Result;
                                                    // if (strResponseValue == "") { break; }
                                                    if (strResponseValue == "Unauthorized") { MessageBox.Show("Unauthorized!"); break; }

                                                    DOSParticipants.api apis = XmlUtil.Deserialize(typeof(DOSParticipants.api), strResponseValue) as DOSParticipants.api;
                                                    DOSParticipants.apiDataParticipant[] participants = (apis.data.Length == 0) ? null : apis.data[0];
                                                    //  if (participants == null) return ds;
                                                    if (participants == null) break;
                                                    string strName = type + "-" + i + " " + DateTime.Now.ToString("HHmmss");
                                                    Files.WriteXml(strName, strResponseValue);

                                                    foreach (DOSParticipants.apiDataParticipant participant in participants)
                                                    {
                                                        if (participant.type == "team")
                                                        {
                                                            DataRow dr = data.Tables["teams"].NewRow();
                                                            dr[0] = participant.id;
                                                            dr[1] = participant.name;
                                                            dr[2] = participant.short_name;
                                                            dr[3] = participant.acronym;
                                                            dr[4] = (participant.gender.ToLower() == "male") ? true : false;
                                                            dr[5] = participant.area_id;

                                                            dr[6] = (participant.national.ToLower() == "yes") ? true : false;
                                                            dr[7] = participant.ut;
                                                            dr[8] = (participant.old_participant_id == "") ? "-1" : participant.old_participant_id;
                                                            dr[9] = participant.slug;
                                                            dr[10] = arr[0];
                                                            dr[11] = cTimestamp;
                                                            data.Tables["teams"].Rows.Add(dr);
                                                        }
                                                        else if (participant.type == "person")
                                                        {
                                                            DataRow dr2 = data.Tables["players"].NewRow();
                                                            dr2[0] = participant.id;
                                                            dr2[1] = participant.name;
                                                            dr2[2] = participant.short_name;
                                                            dr2[3] = participant.acronym;
                                                            dr2[4] = (participant.gender.ToLower() == "male") ? true : false;
                                                            dr2[5] = participant.details[0].birthdate;
                                                            dr2[6] = participant.details[0].position_name;
                                                            dr2[7] = participant.area_id;

                                                            dr2[8] = (participant.national.ToLower() == "yes") ? true : false;
                                                            dr2[9] = participant.ut;
                                                            dr2[10] = (participant.old_participant_id == "") ? "-1" : participant.old_participant_id;
                                                            dr2[11] = participant.slug;
                                                            dr2[12] = arr[0];
                                                            dr2[13] = cTimestamp;
                                                            data.Tables["players"].Rows.Add(dr2);
                                                        }
                                                    }

                                                    if (data.Tables["teams"].Rows.Count > 0)
                                                    {
                                                        count = fda.Update(data.Tables["teams"]);
                                                        Files.WriteLog("[Success] Insert teams[" + count + "  " + "] " + " " + strName + ".xml");
                                                    }
                                                    if (data.Tables["players"].Rows.Count > 0)
                                                    {
                                                        count = adapter1.Update(data.Tables["players"]);
                                                        Files.WriteLog("[Success] Insert players[" + count + "  " + "] " + " " + strName + ".xml");
                                                    }

                                                    ds.Merge(data, true, MissingSchemaAction.AddWithKey);
                                                    data.Clear();
                                                    //if (count > -1)
                                                    //{
                                                    //    Files.WriteLog("[Success] Insert seasons[" + count + "  " + "] " + " " + strName + ".xml");
                                                    //    Files.WriteLog("[Success] Insert seasons[" + count + "  " + "] " + " " + strName + ".xml");
                                                    //}
                                                    //else
                                                    //{
                                                    //    Files.WriteLog("[Failure] Insert seasons [  ]" + " " + strName + ".xml");
                                                    //}
                                                }

                                            }
                                            else
                                            {
                                                ds = data;
                                            }
                                            connection.Close();
                                        }
                                    }
                                }
                            }

                            //    DataSet participantsDs = new DataSet();
                            //queryString = " SELECT FIRST 1 *  FROM seasons;";
                            //FbDataAdapter adapter = new FbDataAdapter();
                            //adapter.SelectCommand = new FbCommand(queryString, connection);
                            //FbCommandBuilder builder = new FbCommandBuilder(adapter);
                            //connection.Open(); 
                            //participantsDs.Tables.Add(new DataTable());
                            //adapter.Fill(participantsDs.Tables[0]);
                            //adapter.SelectCommand = new FbCommand(" SELECT FIRST 1 * FROM competitions", connection);
                            //FbCommandBuilder builder2 = new FbCommandBuilder(adapter);
                            //participantsDs.Tables.Add(new DataTable());
                            //adapter.Fill(participantsDs.Tables[1]);

                            //if (participantsDs.Tables[0].Rows.Count == 0)
                            //{
                            //    for (int i = 1; i < 4; i++)
                            //    {
                            //        var responseValue = clientTest.GetAccessData(strToken, "participants/" + i);
                            //        var strResponseValue = responseValue.Result;

                            //        DOSParticipants.api apis = XmlUtil.Deserialize(typeof(DOSParticipants.api), strResponseValue) as DOSParticipants.api;
                            //        DOSParticipants.apiDataParticipant [] participants = (apis.data.Length == 0) ? null : apis.data[0];
                            //        if (participants == null) return ds;
                            //        string strName = type + "-" + i + " " + DateTime.Now.ToString("HHmmss");
                            //        Files.WriteXml(strName, strResponseValue);

                            //        foreach (DOSParticipants.apiDataParticipant participant in participants)
                            //        {
                            //            //string strCompetition_id = competition.id;
                            //            //DOSParticipants.apiDataCompetitionSeason[] seasons = DOSParticipants.seasons;
                            //            //if (seasons == null) continue;
                            //            //foreach (DOSSeasons.apiDataCompetitionSeason season in seasons)
                            //            //{
                            //            //    DataRow dr = seasonsDs.Tables[0].NewRow();
                            //            //    dr[0] = season.id;
                            //            //    dr[1] = season.name;
                            //            //    dr[2] = strCompetition_id;
                            //            //    dr[3] = season.year;
                            //            //    dr[4] = season.actual;
                            //            //    dr[5] = season.ut;
                            //            //    dr[6] = (season.old_season_id == "") ? "-1" : season.old_season_id;
                            //            //    dr[7] = season.range;
                            //            //    dr[8] = cTimestamp;
                            //            //    seasonsDs.Tables[0].Rows.Add(dr);
                            //            //}
                            //        }
                            //        count = adapter.Update(participantsDs);
                            //        ds.Merge(participantsDs, true, MissingSchemaAction.AddWithKey);
                            //        participantsDs.Clear();

                            //        if (count > -1)
                            //        {
                            //            Console.WriteLine("[Success] Insert participants [" + count + " " + "]");
                            //            Files.WriteLog("[Success] Insert participants[" + count + "  " + "] " + " " + strName + ".xml");
                            //        }
                            //        else
                            //        {
                            //            Console.WriteLine("[Failure] Insert participants [  ]");
                            //            Files.WriteLog("[Failure] Insert participants [  ]" + " " + strName + ".xml");
                            //        }
                            //    }
                            //}
                            //else
                            //{
                            //    count = -1;
                            //    queryString = "select   * from " + type;
                            //    adapter.SelectCommand = new FbCommand(queryString, connection);
                            //    adapter.Fill(participantsDs);
                            //    ds = participantsDs;
                            //}
                            connection.Close();
                        }
                        break;
                    }
                case "events.participants":
                    {
                        using (FbConnection connection = new FbConnection(AppFlag.ScoutsDBConn))
                        {
                            // connection.Open();

                            for (int i = 1; i < 10 && Convert.ToBoolean(arr[0]) == true; i++)
                            {
                                var responseValue = clientTest.GetAccessData(strToken, "participants/" + i, arr[0], arr[1]);
                                var strResponseValue = responseValue.Result;

                                if (strResponseValue == "Unauthorized") { MessageBox.Show("Unauthorized!"); break; }
                                // if (strResponseValue == "Unauthorized") { MessageBox.Show("Unauthorized!");   ds = data;break; }

                                DOSParticipants.api apis = XmlUtil.Deserialize(typeof(DOSParticipants.api), strResponseValue) as DOSParticipants.api;
                                DOSParticipants.apiDataParticipant[] participants = (apis.data.Length == 0) ? null : apis.data[0];
                                //  if (participants == null) return ds;
                                if (participants == null) break;
                                string strName = type + "-" + i + " " + DateTime.Now.ToString("HHmmss");
                                Files.WriteXml(strName, strResponseValue);

                                foreach (DOSParticipants.apiDataParticipant participant in participants)
                                {
                                    if (participant.type == "team")
                                    {
                                        using (FbCommand cmd2 = new FbCommand())
                                        {
                                            cmd2.CommandText = "ADD_TEAM";
                                            cmd2.CommandType = CommandType.StoredProcedure;
                                            cmd2.Connection = connection;
                                            cmd2.Parameters.Add("@ID", participant.id);
                                            cmd2.Parameters.Add("@NAME", participant.name);
                                            cmd2.Parameters.Add("@SHORT_NAME", participant.short_name);
                                            cmd2.Parameters.Add("@ACRONYM", participant.acronym);
                                            cmd2.Parameters.Add("@GENDER", (participant.gender.ToLower() == "male") ? true : false);
                                            cmd2.Parameters.Add("@AREA_ID", participant.area_id);
                                            cmd2.Parameters.Add("@BNATIONAL", (participant.national.ToLower() == "male") ? true : false);
                                            cmd2.Parameters.Add("@UT", participant.ut);
                                            cmd2.Parameters.Add("@OLD_PARTICIPANT_ID", participant.old_participant_id == "" ? "-1" : participant.old_participant_id);
                                            cmd2.Parameters.Add("@SLUG", participant.slug);
                                            cmd2.Parameters.Add("@SEASON_ID", "-1");
                                            cmd2.Parameters.Add("@CTIMESTAMP", cTimestamp);
                                            int id = Convert.ToInt32(cmd2.ExecuteScalar());
                                            Files.WriteLog(id > 0 ? " [Success] Insert teams[" + count + "  " + "] " + " " + strName + ".xml" : " team exist.");
                                        }
                                    }
                                    else if (participant.type == "person")
                                    {
                                        using (FbCommand cmd2 = new FbCommand())
                                        {
                                            cmd2.CommandText = "ADD_Player";
                                            cmd2.CommandType = CommandType.StoredProcedure;
                                            cmd2.Connection = connection;
                                            cmd2.Parameters.Add("@ID", participant.id);
                                            cmd2.Parameters.Add("@NAME", participant.name);
                                            cmd2.Parameters.Add("@SHORT_NAME", participant.short_name);
                                            cmd2.Parameters.Add("@ACRONYM", participant.acronym);
                                            cmd2.Parameters.Add("@GENDER", (participant.gender.ToLower() == "male") ? true : false);
                                            cmd2.Parameters.Add("@BIRTHDATE", participant.area_id);
                                            cmd2.Parameters.Add("@POSITION_NAME", participant.area_id);
                                            cmd2.Parameters.Add("@AREA_ID", participant.area_id);
                                            cmd2.Parameters.Add("@BNATIONAL", (participant.national.ToLower() == "male") ? true : false);
                                            cmd2.Parameters.Add("@UT", participant.ut);
                                            cmd2.Parameters.Add("@OLD_PARTICIPANT_ID", participant.old_participant_id == "" ? "-1" : participant.old_participant_id);
                                            cmd2.Parameters.Add("@SLUG", participant.slug);
                                            cmd2.Parameters.Add("@TEAM_ID", arr[0]);
                                            cmd2.Parameters.Add("@SEASON_ID", "-1");
                                            cmd2.Parameters.Add("@CTIMESTAMP", cTimestamp);
                                            int id = Convert.ToInt32(cmd2.ExecuteScalar());
                                            Files.WriteLog(id > 0 ? " [Success] Insert players[" + count + "  " + "] " + " " + strName + ".xml" : " player exist.");
                                        }
                                    }
                                }
                            }

                            queryString = "SELECT t.* FROM teams t  inner  join  events e on   t.id= e.HOME_ID or t.id=e.GUEST_ID   where e.id='" + arr[1] + "' order by id asc";
                            using (FbCommand cmd = new FbCommand(queryString))
                            {
                                using (FbCommandBuilder fcb = new FbCommandBuilder())
                                {
                                    using (FbDataAdapter fda = new FbDataAdapter())
                                    {
                                        cmd.Connection = connection;
                                        fda.SelectCommand = cmd;
                                        fcb.DataAdapter = fda;
                                        using (DataSet data = new DataSet())
                                        {
                                            data.Tables.Add(new DataTable("teams"));
                                            fda.Fill(data.Tables["teams"]);

                                            // queryString = "SELECT p.* FROM teams t  inner join  events e on t.id = e.HOME_ID or t.id = e.GUEST_ID    inner join players p on t.id = p.TEAM_ID   where e.id = '" + arr[1] + "' order by p.id asc";
                                            queryString = "SELECT p.*FROM players p inner join  events e on p.TEAM_ID = e.HOME_ID or p.TEAM_ID = e.GUEST_ID where e.id='" + arr[1] + "' order by p.id asc";
                                            data.Tables.Add(new DataTable("players"));
                                            FbDataAdapter adapter1 = new FbDataAdapter();
                                            adapter1.SelectCommand = new FbCommand(queryString, connection);
                                            FbCommandBuilder builder3 = new FbCommandBuilder(adapter1);
                                            adapter1.Fill(data.Tables["players"]);
                                            //if (data.Tables["teams"].Rows.Count == 0 && data.Tables["players"].Rows.Count == 0)
                                            //{ 
                                            //}
                                            //else
                                            //{
                                            ds = data;
                                            //}
                                            connection.Close();
                                        }
                                    }
                                }
                            }
                            connection.Close();
                        }
                        break;
                    }
                 case "events":
                    {
                        using (FbConnection connection = new FbConnection(AppFlag.ScoutsDBConn))
                        {
                            //  queryString = "select * from " + type + " where  COMPETITION_ID=" + arr[0] +" and booked=false";
                            //queryString = "select * from " + type;// + " where booked = false";
                            //start_date  BETWEEN  '9/19/2018 00:00:00' and'9/20/2018  10:59:59'
                            if (Convert.ToBoolean(arr[0]))
                            {
                                queryString = "select * from " + type + " where '" + arr[1] + "'<= start_date and start_date <='" + arr[2] + "'";
                            }
                            else
                            {
                                if (arr[1].ToString().Length < 12)
                                {
                                    queryString = "select * from " + type + " where " + (arr[1].ToString() == "Group" ? "GROUP_ID" : (arr[1].ToString() == "Stage") ? "STAGE_ID" : (arr[1].ToString() == "Season") ? "SEASON_ID" : (arr[1].ToString() == "Comp") ? "COMPETITION_ID" : "") + "='" + arr[2] + "'";
                                }
                                else
                                {
                                    queryString = "select * from " + type + " where '" + arr[1] + "'<= start_date and start_date <='" + arr[2] + "'";
                                }
                            }
                            FbDataAdapter adapter = new FbDataAdapter();
                            adapter.SelectCommand = new FbCommand(queryString, connection);
                            FbCommandBuilder builder = new FbCommandBuilder(adapter);
                            connection.Open();
                            DataSet eventsDs = new DataSet();
                            adapter.Fill(eventsDs);
                             if (Convert.ToBoolean(arr[0]) && DateTime.Now <= Convert .ToDateTime ( arr[2]))
                           // if (Convert.ToBoolean(arr[0]))
                            //if (DateTime.Now <= Convert.ToDateTime(arr[2]))
                            {
                                //if (eventsDs.Tables[0].Rows.Count == 0)
                                //{
                                //DataSet newEventsDs= eventsDs.Clone ();
                                for (int i = 1; i < 10; i++)
                                {
                                    //if (Convert.ToBoolean(arr[0]) == false) { ds = eventsDs; break; }

                                    //var responseValue = clientTest.GetAccessData(strToken, "events/" + i, arr[1], arr[2]);
                                    //var strResponseValue = responseValue.Result;
                                    XDocument document = XDocument.Load("E:\\Project\\AppProject\\DataOfScouts\\DataOfScouts\\bin\\Debug\\New folder\\events-1153436.xml");
                                    var strResponseValue = document.ToString();

                                    if (strResponseValue == "Unauthorized") { MessageBox.Show("Unauthorized!"); break; }
                                    string strName = type + "-" + i + " " + DateTime.Now.ToString("HHmmss");
                                    Files.WriteXml(strName, strResponseValue);

                                    DOSEvents.api apis = XmlUtil.Deserialize(typeof(DOSEvents.api), strResponseValue) as DOSEvents.api;
                                    if (apis == null) break;
                                    DOSEvents.apiDataCompetition[] competitions = (apis.data.Length == 0) ? null : apis.data[0];
                                    if (competitions == null) break;

                                    foreach (DOSEvents.apiDataCompetition competition in competitions)
                                    {
                                        string strCompetition_id = competition.id;
                                        string strArea_id = competition.area_id;

                                        using (FbCommand cmd = new FbCommand())
                                        {
    //                                        'ID', 'NAME', 'SHORT_NAME', 'MINI_NAME', 'GENDER',
    //'CTYPE', 'AREA_ID', 'AREA_TYPE', 'AREA_SORT', 'OVERALL_SORT', 'SPORT_ID',
    //'SPORT_NAME', 'TOUR_ID', 'TOUR_NAME', 'UT', 'OLD_COMPETITION_ID', 'SLUG', 'CTIMESTAMP'
                                            cmd.CommandText = "ADD_COMPETITION";
                                            cmd.CommandType = CommandType.StoredProcedure;
                                            cmd.Connection = connection;
                                            cmd.Parameters.Add("@ID", strCompetition_id);
                                            cmd.Parameters.Add("@NAME", competition.name  );
                                            cmd.Parameters.Add("@SHORT_NAME", competition.short_name  );
                                            cmd.Parameters.Add("@MINI_NAME", competition.mini_name );
                                            cmd.Parameters.Add("@GENDER", (competition.gender.ToLower() == "male") ? true : false);
                                            cmd.Parameters.Add("@CTYPE", competition.type );
                                            cmd.Parameters.Add("@AREA_ID", competition.area_id );
                                            cmd.Parameters.Add("@AREA_TYPE", competition.type );
                                            cmd.Parameters.Add("@AREA_SORT", competition.area_sort );
                                            cmd.Parameters.Add("@OVERALL_SORT", competition.overall_sort );
                                            cmd.Parameters.Add("@TOUR_ID", competition.tour_id ==""?"-1": competition.tour_id);
                                            cmd.Parameters.Add("@TOUR_NAME", competition.tour_name);
                                            cmd.Parameters.Add("@UT", competition.ut );
                                            cmd.Parameters.Add("@OLD_COMPETITION_ID", competition.old_competition_id == "" ? "-1" : competition.old_competition_id );
                                            cmd.Parameters.Add("@SLUG", competition.slug );
                                            cmd.Parameters.Add("@CTIMESTAMP", cTimestamp); 
                                            int id = Convert.ToInt32(cmd.ExecuteScalar());
                                        }


                                        DOSEvents.apiDataCompetitionSeason[] seasons = competition.seasons;
                                        if (seasons == null) continue;
                                        foreach (DOSEvents.apiDataCompetitionSeason season in seasons)
                                        {
                                            string strSeasons_id = season.id; 
                                            using (FbCommand cmd = new FbCommand())
                                            {
                                                //r.ID, r.NAME, r.COMPETITION_ID, r.SYEAR, r.ACTUAL, r.UT, r.OLD_SEASON_ID,
                                                //r.RANGE, r.CTIMESTAMP
                                                cmd.CommandText = "ADD_SEASON";
                                                cmd.CommandType = CommandType.StoredProcedure;
                                                cmd.Connection = connection;
                                                cmd.Parameters.Add("@ID", season.id);
                                                cmd.Parameters.Add("@NAME", season.name);
                                                cmd.Parameters.Add("@COMPETITION_ID", strCompetition_id);
                                                cmd.Parameters.Add("@SYEAR", season.year);
                                                cmd.Parameters.Add("@ACTUAL", season.actual);
                                                cmd.Parameters.Add("@UT", season.ut);
                                                cmd.Parameters.Add("@OLD_SEASON_ID", season.old_season_id == "" ? "-1" : season.old_season_id);
                                                cmd.Parameters.Add("@RANGE", season.range);
                                                cmd.Parameters.Add("@CTIMESTAMP", cTimestamp);
                                                int id = Convert.ToInt32(cmd.ExecuteScalar());
                                            }

                                            DOSEvents.apiDataCompetitionSeasonStage[] stages = season.stages;
                                            if (stages == null) continue;
                                            
                                            foreach (DOSEvents.apiDataCompetitionSeasonStage stage in stages)
                                            {
                                                string strStage_id = stage.id == "" ? "-1" : stage.id;
                                                if (strStage_id != "-1")
                                                {
                                                    using (FbCommand cmd = new FbCommand())
                                                    {
                                                        //r.ID, r.STAGE_NAME_ID, r.NAME, r.START_DATE, r.END_DATE,
                                                        //r.SHOW_STANDINGS, r.GROUPS_NR, r.ISORT, r.IS_CURRENT, r.UT, r.OLD_STAGE_ID,    r.SEASON_ID, r.COMPETITION_ID, r.AREA_ID, r.CTIMESTAMP
                                                          cmd.CommandText = "ADD_STAGE";
                                                        cmd.CommandType = CommandType.StoredProcedure;
                                                        cmd.Connection = connection;
                                                        cmd.Parameters.Add("@ID", stage.id);
                                                        cmd.Parameters.Add("@STAGE_NAME_ID", stage.stage_name_id);
                                                        cmd.Parameters.Add("@NAME", stage.name);
                                                        cmd.Parameters.Add("@START_DATE", stage.start_date);
                                                        cmd.Parameters.Add("@END_DATE", stage.end_date);
                                                        cmd.Parameters.Add("@SHOW_STANDINGS", stage.show_standings.ToLower() == "yes" ? true : false);
                                                        cmd.Parameters.Add("@GROUPS_NR", stage.groups_nr == "" ? "-1" : stage.groups_nr);
                                                        cmd.Parameters.Add("@ISORT", stage.sort == "" ? "-1" : stage.sort);
                                                        cmd.Parameters.Add("@IS_CURRENT", stage.is_current.ToLower() == "yes" ? true : false);
                                                        cmd.Parameters.Add("@UT", stage.ut);
                                                        cmd.Parameters.Add("@OLD_STAGE_ID", stage.old_stage_id == "" ? "-1" : stage.old_stage_id);
                                                        cmd.Parameters.Add("@SEASON_ID", strSeasons_id);
                                                        cmd.Parameters.Add("@COMPETITION_ID", strCompetition_id);
                                                        cmd.Parameters.Add("@AREA_ID", strArea_id);
                                                        cmd.Parameters.Add("@CTIMESTAMP", cTimestamp);
                                                        int id = Convert.ToInt32(cmd.ExecuteScalar());
                                                    }
                                                }
                                                DOSEvents.apiDataCompetitionSeasonStageGroup[] groups = stage.groups;
                                                if (groups == null) continue;
                                                foreach (DOSEvents.apiDataCompetitionSeasonStageGroup group in groups)
                                                {
                                                    string strGroup_id = group.id == "" ? "-1" : group.id;
                                                    if (strGroup_id != "-1")
                                                    {
                                                        using (FbCommand cmd = new FbCommand())
                                                        {
                                                            // r.ID, r.NAME, r.UT, r.STAGE_ID, r.SEASON_ID, r.COMPETITION_ID, r.AREA_ID,     r.CTIMESTAMP
                                                            cmd.CommandText = "ADD_GROUP";
                                                            cmd.CommandType = CommandType.StoredProcedure;
                                                            cmd.Connection = connection;
                                                            cmd.Parameters.Add("@ID", group.id);
                                                            cmd.Parameters.Add("@NAME", group.name);
                                                            cmd.Parameters.Add("@UT", group.ut );
                                                            cmd.Parameters.Add("@STAGE_ID", strStage_id);
                                                            cmd.Parameters.Add("@SEASON_ID", strSeasons_id);
                                                            cmd.Parameters.Add("@COMPETITION_ID", strCompetition_id);
                                                            cmd.Parameters.Add("@AREA_ID", strArea_id); 
                                                            cmd.Parameters.Add("@CTIMESTAMP", cTimestamp);
                                                            int id = Convert.ToInt32(cmd.ExecuteScalar());
                                                        }
                                                    }

                                                    foreach (DOSEvents.apiDataCompetitionSeasonStageGroupEvent sevent in group.events)
                                                    {
                                                        if (sevent == null) continue;
                                                        DataRow[] drs = eventsDs.Tables[0].Select("id=" + sevent.id);
                                                        if (eventsDs.Tables[0].Select("id=" + sevent.id).Length == 0)
                                                        {
                                                            DOSEvents.apiDataCompetitionSeasonStageGroupEventParticipant[] participants = sevent.participants;

                                                            DataRow dr = eventsDs.Tables[0].NewRow();
                                                            dr[0] = sevent.id;
                                                            dr[1] = sevent.name;
                                                            dr[2] = (participants[0].counter == "1") ? participants[0].id : participants[1].id;
                                                            dr[3] = (participants[1].counter == "2") ? participants[1].id : participants[0].id;
                                                            dr[4] = sevent.source;
                                                            dr[5] = sevent.source_dc == "yes" ? true : false;
                                                            dr[6] = sevent.source_super;
                                                            dr[7] = sevent.relation_status;
                                                            dr[8] = Convert.ToDateTime(sevent.start_date);
                                                            dr[9] = sevent.ft_only == "yes" ? true : false;
                                                            dr[10] = sevent.coverage_type;
                                                            dr[11] = sevent.channel_id;
                                                            dr[12] = sevent.channel_name;
                                                            dr[13] = sevent.scoutsfeed == "yes" ? true : false;
                                                            dr[14] = sevent.status_id;
                                                            dr[15] = sevent.status_name;
                                                            dr[16] = sevent.status_type;
                                                            dr[17] = sevent.day;
                                                            dr[18] = sevent.clock_time;
                                                            dr[19] = sevent.clock_status;
                                                            dr[20] = sevent.winner_id;
                                                            dr[21] = sevent.progress_id;
                                                            dr[22] = sevent.bet_status;
                                                            dr[23] = sevent.neutral_venue == "yes" ? true : false;
                                                            dr[24] = sevent.item_status;
                                                            dr[25] = sevent.ut;
                                                            dr[26] = sevent.old_event_id == "" ? "-1" : sevent.old_event_id;
                                                            dr[27] = sevent.slug;
                                                            dr[28] = sevent.verified_result == "yes" ? true : false;
                                                            dr[29] = sevent.is_protocol_verified == "yes" ? true : false;
                                                            dr[30] = sevent.protocol_verified_by;
                                                            dr[31] = sevent.protocol_verified_at;
                                                            dr[32] = sevent.round_id;
                                                            dr[33] = sevent.round_name;
                                                            dr[34] = sevent.client_event_id == "" ? "-1" : sevent.client_event_id;
                                                            dr[35] = sevent.booked == "yes" ? true : false;
                                                            dr[36] = sevent.booked_by;
                                                            dr[37] = sevent.inverted_participants == "yes" ? true : false;
                                                            dr[38] = sevent.venue_id;
                                                            dr[39] = group.id == "" ? "-1" : group.id;
                                                            dr[40] = strStage_id;
                                                            dr[41] = strSeasons_id;
                                                            dr[42] = strCompetition_id;
                                                            dr[43] = strArea_id;
                                                            dr[44] = cTimestamp;
                                                            eventsDs.Tables[0].Rows.Add(dr);
                                                        }
                                                        else
                                                        {
                                                            Files.WriteLog("[" + drs[0]["id"] + "]   events existed.");
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    count = adapter.Update(eventsDs);
                                    ds.Merge(eventsDs, true, MissingSchemaAction.AddWithKey);
                                    eventsDs.Clear();

                                    if (count > -1)
                                    {
                                        Files.WriteLog("[Success] Insert events[" + count + "  " + "] " + " " + strName + ".xml");
                                    }
                                    else
                                    {
                                        Files.WriteLog("[Failure] Insert events [  ]" + " " + strName + ".xml");
                                    }
                                }
                                //}
                                //else
                                //{
                                //    //count = -1;
                                //    //queryString = "select   * from " + type;
                                //    //adapter.SelectCommand = new FbCommand(queryString, connection);
                                //    //adapter.Fill(eventsDs);
                                //count = adapter.Update(eventsDs);
                                //ds = eventsDs;
                                //}
                            }
                            else
                            {
                                ds = eventsDs;
                            }
                            connection.Close();
                        }
                        break;
                    }
                case "booked-events":
                    {
                        using (FbConnection connection = new FbConnection(AppFlag.ScoutsDBConn))
                        {
                            //  queryString = "select * from " + type + " where  COMPETITION_ID=" + arr[0] +" and booked=false";
                            //queryString = "select * from " + type;// + " where booked = false";
                            //start_date  BETWEEN  '9/19/2018 00:00:00' and'9/20/2018  10:59:59'
                            if (Convert.ToBoolean(arr[0]))
                            {
                                queryString = "select * from " + "events" + " where '" + arr[1] + "'<= start_date and start_date <='" + arr[2] + "' and booked =true";
                            }
                            else
                            {
                                if (arr[1].ToString().Length < 12)
                                {
                                    queryString = "select * from " + "events" + " where " + (arr[1].ToString() == "Group" ? "GROUP_ID" : (arr[1].ToString() == "Stage") ? "STAGE_ID" : (arr[1].ToString() == "Season") ? "SEASON_ID" : (arr[1].ToString() == "Comp") ? "COMPETITION_ID" : "") + "='" + arr[2] + "' and booked =true";
                                }
                                else
                                {
                                    queryString = "select * from " + "events" + " where '" + arr[1] + "'<= start_date and start_date <='" + arr[2] + "' and booked =true";
                                }
                            }
                            FbDataAdapter adapter = new FbDataAdapter();
                            adapter.SelectCommand = new FbCommand(queryString, connection);
                            FbCommandBuilder builder = new FbCommandBuilder(adapter);
                            connection.Open();
                            DataSet eventsDs = new DataSet();
                            adapter.Fill(eventsDs);
                            if (Convert.ToBoolean(arr[0]) && DateTime.Now <= Convert.ToDateTime(arr[2]))
                            //if (DateTime.Now <= Convert.ToDateTime(arr[2]))
                            {
                                //if (eventsDs.Tables[0].Rows.Count == 0)
                                //{
                                //DataSet newEventsDs= eventsDs.Clone ();
                                for (int i = 1; i < 10; i++)
                                {
                                    //if (Convert.ToBoolean(arr[0]) == false) { ds = eventsDs; break; }

                                    var responseValue = clientTest.GetAccessData(strToken, "events/" + i, arr[1], arr[2]);
                                    var strResponseValue = responseValue.Result;
                                    //XDocument document = XDocument.Load("E:\\Project\\AppProject\\DataOfScouts\\DataOfScouts\\bin\\Debug\\New folder\\events-1101914.xml");
                                    //var strResponseValue = document.ToString();

                                    if (strResponseValue == "Unauthorized") { MessageBox.Show("Unauthorized!"); break; }
                                    string strName = type + "-" + i + " " + DateTime.Now.ToString("HHmmss");
                                    Files.WriteXml(strName, strResponseValue);

                                    DOSEvents.api apis = XmlUtil.Deserialize(typeof(DOSEvents.api), strResponseValue) as DOSEvents.api;
                                    if (apis == null) break;
                                    DOSEvents.apiDataCompetition[] competitions = (apis.data.Length == 0) ? null : apis.data[0];
                                    if (competitions == null) break;

                                    foreach (DOSEvents.apiDataCompetition competition in competitions)
                                    {
                                        string strCompetition_id = competition.id;
                                        string strArea_id = competition.area_id;

                                        using (FbCommand cmd = new FbCommand())
                                        {
                                            //                                        'ID', 'NAME', 'SHORT_NAME', 'MINI_NAME', 'GENDER',
                                            //'CTYPE', 'AREA_ID', 'AREA_TYPE', 'AREA_SORT', 'OVERALL_SORT', 'SPORT_ID',
                                            //'SPORT_NAME', 'TOUR_ID', 'TOUR_NAME', 'UT', 'OLD_COMPETITION_ID', 'SLUG', 'CTIMESTAMP'
                                            cmd.CommandText = "ADD_COMPETITION";
                                            cmd.CommandType = CommandType.StoredProcedure;
                                            cmd.Connection = connection;
                                            cmd.Parameters.Add("@ID", strCompetition_id);
                                            cmd.Parameters.Add("@NAME", competition.name);
                                            cmd.Parameters.Add("@SHORT_NAME", competition.short_name);
                                            cmd.Parameters.Add("@MINI_NAME", competition.mini_name);
                                            cmd.Parameters.Add("@GENDER", (competition.gender.ToLower() == "male") ? true : false);
                                            cmd.Parameters.Add("@CTYPE", competition.type);
                                            cmd.Parameters.Add("@AREA_ID", competition.area_id);
                                            cmd.Parameters.Add("@AREA_TYPE", competition.type);
                                            cmd.Parameters.Add("@AREA_SORT", competition.area_sort);
                                            cmd.Parameters.Add("@OVERALL_SORT", competition.overall_sort);
                                            cmd.Parameters.Add("@TOUR_ID", competition.tour_id == "" ? "-1" : competition.tour_id);
                                            cmd.Parameters.Add("@TOUR_NAME", competition.tour_name);
                                            cmd.Parameters.Add("@UT", competition.ut);
                                            cmd.Parameters.Add("@OLD_COMPETITION_ID", competition.old_competition_id == "" ? "-1" : competition.old_competition_id);
                                            cmd.Parameters.Add("@SLUG", competition.slug);
                                            cmd.Parameters.Add("@CTIMESTAMP", cTimestamp);
                                            int id = Convert.ToInt32(cmd.ExecuteScalar());
                                        }


                                        DOSEvents.apiDataCompetitionSeason[] seasons = competition.seasons;
                                        if (seasons == null) continue;
                                        foreach (DOSEvents.apiDataCompetitionSeason season in seasons)
                                        {
                                            string strSeasons_id = season.id;
                                            using (FbCommand cmd = new FbCommand())
                                            {
                                                //r.ID, r.NAME, r.COMPETITION_ID, r.SYEAR, r.ACTUAL, r.UT, r.OLD_SEASON_ID,
                                                //r.RANGE, r.CTIMESTAMP
                                                cmd.CommandText = "ADD_SEASON";
                                                cmd.CommandType = CommandType.StoredProcedure;
                                                cmd.Connection = connection;
                                                cmd.Parameters.Add("@ID", season.id);
                                                cmd.Parameters.Add("@NAME", season.name);
                                                cmd.Parameters.Add("@COMPETITION_ID", strCompetition_id);
                                                cmd.Parameters.Add("@SYEAR", season.year);
                                                cmd.Parameters.Add("@ACTUAL", season.actual);
                                                cmd.Parameters.Add("@UT", season.ut);
                                                cmd.Parameters.Add("@OLD_SEASON_ID", season.old_season_id == "" ? "-1" : season.old_season_id);
                                                cmd.Parameters.Add("@RANGE", season.range);
                                                cmd.Parameters.Add("@CTIMESTAMP", cTimestamp);
                                                int id = Convert.ToInt32(cmd.ExecuteScalar());
                                            }

                                            DOSEvents.apiDataCompetitionSeasonStage[] stages = season.stages;
                                            if (stages == null) continue;

                                            foreach (DOSEvents.apiDataCompetitionSeasonStage stage in stages)
                                            {
                                                string strStage_id = stage.id == "" ? "-1" : stage.id;
                                                if (strStage_id != "-1")
                                                {
                                                    using (FbCommand cmd = new FbCommand())
                                                    {
                                                        //r.ID, r.STAGE_NAME_ID, r.NAME, r.START_DATE, r.END_DATE,
                                                        //r.SHOW_STANDINGS, r.GROUPS_NR, r.ISORT, r.IS_CURRENT, r.UT, r.OLD_STAGE_ID,    r.SEASON_ID, r.COMPETITION_ID, r.AREA_ID, r.CTIMESTAMP
                                                        cmd.CommandText = "ADD_STAGE";
                                                        cmd.CommandType = CommandType.StoredProcedure;
                                                        cmd.Connection = connection;
                                                        cmd.Parameters.Add("@ID", stage.id);
                                                        cmd.Parameters.Add("@STAGE_NAME_ID", stage.stage_name_id);
                                                        cmd.Parameters.Add("@NAME", stage.name);
                                                        cmd.Parameters.Add("@START_DATE", stage.start_date);
                                                        cmd.Parameters.Add("@END_DATE", stage.end_date);
                                                        cmd.Parameters.Add("@SHOW_STANDINGS", stage.show_standings.ToLower() == "yes" ? true : false);
                                                        cmd.Parameters.Add("@GROUPS_NR", stage.groups_nr == "" ? "-1" : stage.groups_nr);
                                                        cmd.Parameters.Add("@ISORT", stage.sort == "" ? "-1" : stage.sort);
                                                        cmd.Parameters.Add("@IS_CURRENT", stage.is_current.ToLower() == "yes" ? true : false);
                                                        cmd.Parameters.Add("@UT", stage.ut);
                                                        cmd.Parameters.Add("@OLD_STAGE_ID", stage.old_stage_id == "" ? "-1" : stage.old_stage_id);
                                                        cmd.Parameters.Add("@SEASON_ID", strSeasons_id);
                                                        cmd.Parameters.Add("@COMPETITION_ID", strCompetition_id);
                                                        cmd.Parameters.Add("@AREA_ID", strArea_id);
                                                        cmd.Parameters.Add("@CTIMESTAMP", cTimestamp);
                                                        int id = Convert.ToInt32(cmd.ExecuteScalar());
                                                    }
                                                }
                                                DOSEvents.apiDataCompetitionSeasonStageGroup[] groups = stage.groups;
                                                if (groups == null) continue;
                                                foreach (DOSEvents.apiDataCompetitionSeasonStageGroup group in groups)
                                                {
                                                    string strGroup_id = group.id == "" ? "-1" : group.id;
                                                    if (strGroup_id != "-1")
                                                    {
                                                        using (FbCommand cmd = new FbCommand())
                                                        {
                                                            // r.ID, r.NAME, r.UT, r.STAGE_ID, r.SEASON_ID, r.COMPETITION_ID, r.AREA_ID,     r.CTIMESTAMP
                                                            cmd.CommandText = "ADD_GROUP";
                                                            cmd.CommandType = CommandType.StoredProcedure;
                                                            cmd.Connection = connection;
                                                            cmd.Parameters.Add("@ID", group.id);
                                                            cmd.Parameters.Add("@NAME", group.name);
                                                            cmd.Parameters.Add("@UT", group.ut);
                                                            cmd.Parameters.Add("@STAGE_ID", strStage_id);
                                                            cmd.Parameters.Add("@SEASON_ID", strSeasons_id);
                                                            cmd.Parameters.Add("@COMPETITION_ID", strCompetition_id);
                                                            cmd.Parameters.Add("@AREA_ID", strArea_id);
                                                            cmd.Parameters.Add("@CTIMESTAMP", cTimestamp);
                                                            int id = Convert.ToInt32(cmd.ExecuteScalar());
                                                        }
                                                    }

                                                    foreach (DOSEvents.apiDataCompetitionSeasonStageGroupEvent sevent in group.events)
                                                    {
                                                        if (sevent == null) continue;
                                                        DataRow[] drs = eventsDs.Tables[0].Select("id=" + sevent.id);
                                                        if (eventsDs.Tables[0].Select("id=" + sevent.id).Length == 0)
                                                        {
                                                            DOSEvents.apiDataCompetitionSeasonStageGroupEventParticipant[] participants = sevent.participants;

                                                            DataRow dr = eventsDs.Tables[0].NewRow();
                                                            dr[0] = sevent.id;
                                                            dr[1] = sevent.name;
                                                            dr[2] = (participants[0].counter == "1") ? participants[0].id : participants[1].id;
                                                            dr[3] = (participants[1].counter == "2") ? participants[1].id : participants[0].id;
                                                            dr[4] = sevent.source;
                                                            dr[5] = sevent.source_dc == "yes" ? true : false;
                                                            dr[6] = sevent.source_super;
                                                            dr[7] = sevent.relation_status;
                                                            dr[8] = Convert.ToDateTime(sevent.start_date);
                                                            dr[9] = sevent.ft_only == "yes" ? true : false;
                                                            dr[10] = sevent.coverage_type;
                                                            dr[11] = sevent.channel_id;
                                                            dr[12] = sevent.channel_name;
                                                            dr[13] = sevent.scoutsfeed == "yes" ? true : false;
                                                            dr[14] = sevent.status_id;
                                                            dr[15] = sevent.status_name;
                                                            dr[16] = sevent.status_type;
                                                            dr[17] = sevent.day;
                                                            dr[18] = sevent.clock_time;
                                                            dr[19] = sevent.clock_status;
                                                            dr[20] = sevent.winner_id;
                                                            dr[21] = sevent.progress_id;
                                                            dr[22] = sevent.bet_status;
                                                            dr[23] = sevent.neutral_venue == "yes" ? true : false;
                                                            dr[24] = sevent.item_status;
                                                            dr[25] = sevent.ut;
                                                            dr[26] = sevent.old_event_id == "" ? "-1" : sevent.old_event_id;
                                                            dr[27] = sevent.slug;
                                                            dr[28] = sevent.verified_result == "yes" ? true : false;
                                                            dr[29] = sevent.is_protocol_verified == "yes" ? true : false;
                                                            dr[30] = sevent.protocol_verified_by;
                                                            dr[31] = sevent.protocol_verified_at;
                                                            dr[32] = sevent.round_id;
                                                            dr[33] = sevent.round_name;
                                                            dr[34] = sevent.client_event_id == "" ? "-1" : sevent.client_event_id;
                                                            dr[35] = sevent.booked == "yes" ? true : false;
                                                            dr[36] = sevent.booked_by;
                                                            dr[37] = sevent.inverted_participants == "yes" ? true : false;
                                                            dr[38] = sevent.venue_id;
                                                            dr[39] = group.id == "" ? "-1" : group.id;
                                                            dr[40] = strStage_id;
                                                            dr[41] = strSeasons_id;
                                                            dr[42] = strCompetition_id;
                                                            dr[43] = strArea_id;
                                                            dr[44] = cTimestamp;
                                                            eventsDs.Tables[0].Rows.Add(dr);
                                                        }
                                                        else
                                                        {
                                                            Files.WriteLog("[" + drs[0]["id"] + "]   events existed.");
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    count = adapter.Update(eventsDs);
                                    ds.Merge(eventsDs, true, MissingSchemaAction.AddWithKey);
                                    eventsDs.Clear();

                                    if (count > -1)
                                    {
                                        Files.WriteLog("[Success] Insert events[" + count + "  " + "] " + " " + strName + ".xml");
                                    }
                                    else
                                    {
                                        Files.WriteLog("[Failure] Insert events [  ]" + " " + strName + ".xml");
                                    }
                                }
                                //}
                                //else
                                //{
                                //    //count = -1;
                                //    //queryString = "select   * from " + type;
                                //    //adapter.SelectCommand = new FbCommand(queryString, connection);
                                //    //adapter.Fill(eventsDs);
                                //count = adapter.Update(eventsDs);
                                //ds = eventsDs;
                                //}
                            }
                            else
                            {
                                ds = eventsDs;
                            }
                            connection.Close();
                        }
                        break;
                    }
                case "booked-events2":
                    {
                        using (FbConnection connection = new FbConnection(AppFlag.ScoutsDBConn))
                        {
                            //queryString = "select * from " + "events where  booked = true";
                            queryString = "select * from " + "events" + " where  booked = true and '" + arr[0] + "'<= start_date and start_date <='" + arr[1] + "'";

                            FbDataAdapter adapter = new FbDataAdapter();
                            adapter.SelectCommand = new FbCommand(queryString, connection);
                            FbCommandBuilder builder = new FbCommandBuilder(adapter);
                            connection.Open();
                            DataSet bookedEventsDs = new DataSet();
                            adapter.Fill(bookedEventsDs);
                            int l = 0;
                            //if (bookedEventsDs.Tables[0].Rows.Count == 0)
                            //{
                            for (int i = 1; i < 4; i++)
                            {
                                if (Convert.ToBoolean(arr[2]) == false) break;

                                var responseValue = clientTest.GetAccessData(strToken, "booked-events/" + i);
                                var strResponseValue = responseValue.Result;

                                if (strResponseValue == "") { break; }
                                string strName = type + "-" + i + " " + DateTime.Now.ToString("HHmmss");
                                Files.WriteXml(strName, strResponseValue);

                                DOSBookedEvents.api apis = XmlUtil.Deserialize(typeof(DOSBookedEvents.api), strResponseValue) as DOSBookedEvents.api;
                                if (apis == null) break;
                                DOSBookedEvents.apiDataBooked_eventsEvent[] bookedEvents = (apis.data.Length == 0) ? null : apis.data[0];
                                if (bookedEvents == null) break;

                                foreach (DOSBookedEvents.apiDataBooked_eventsEvent sevent in bookedEvents)
                                {
                                    DataRow[] drs = bookedEventsDs.Tables[0].Select("id=" + sevent.id);
                                    // if (bookedEventsDs.Tables[0].Select("id=" + sevent.id).Length == 0)
                                    if (drs.Length == 0)
                                    {
                                        DOSBookedEvents.apiDataBooked_eventsEventParticipant[] participants = sevent.participants;

                                        DataRow dr = bookedEventsDs.Tables[0].NewRow();
                                        dr[0] = sevent.id;
                                        dr[1] = sevent.name;
                                        dr[2] = (participants[0].counter == "1") ? participants[0].id : participants[1].id;
                                        dr[3] = (participants[1].counter == "2") ? participants[1].id : participants[0].id;
                                        dr[4] = sevent.source;
                                        dr[5] = false;//sevent.source_dc == "yes" ? true : false;
                                        dr[6] = "";// sevent.source_super;
                                        dr[7] = sevent.relation_status;
                                        dr[8] = sevent.start_date;
                                        dr[9] = sevent.ft_only == "yes" ? true : false;
                                        dr[10] = sevent.coverage_type;
                                        dr[11] = "";// sevent.channel_id;
                                        dr[12] = "";// sevent.channel_name;
                                        dr[13] = sevent.scoutsfeed == "yes" ? true : false;
                                        dr[14] = sevent.status_id;
                                        dr[15] = sevent.status_name;
                                        dr[16] = sevent.status_type;
                                        dr[17] = sevent.day;
                                        dr[18] = sevent.clock_time;
                                        dr[19] = sevent.clock_status;
                                        dr[20] = sevent.winner_id;
                                        dr[21] = sevent.progress_id;
                                        dr[22] = sevent.bet_status;
                                        dr[23] = sevent.neutral_venue == "yes" ? true : false;
                                        dr[24] = sevent.item_status;
                                        dr[25] = sevent.ut;
                                        dr[26] = sevent.old_event_id == "" ? "-1" : sevent.old_event_id;
                                        dr[27] = sevent.slug;
                                        dr[28] = sevent.verified_result == "yes" ? true : false;
                                        dr[29] = false;// sevent.is_protocol_verified == "yes" ? true : false;
                                        dr[30] = "";// sevent.protocol_verified_by;
                                        dr[31] = "";// sevent.protocol_verified_at;
                                        dr[32] = sevent.round_id;
                                        dr[33] = sevent.round_name;
                                        dr[34] = sevent.client_event_id == "" ? "-1" : sevent.client_event_id;
                                        dr[35] = true;// sevent.booked == "yes" ? true : false;
                                        dr[36] = "";// sevent.booked_by;
                                        dr[37] = false;// sevent.inverted_participants == "yes" ? true : false;
                                        dr[38] = "";// sevent.venue_id;
                                        dr[39] = -1;// group.id == "" ? "-1" : group.id;
                                        dr[40] = -1;// sevent.stage_id;// strStage_id;
                                        dr[41] = -1;// sevent.season_id; //strSeasons_id;
                                        dr[42] = sevent.competition_id;// strCompetition_id;
                                        dr[43] = -1;// sevent.area_id;// strArea_id;
                                        dr[44] = cTimestamp;
                                        bookedEventsDs.Tables[0].Rows.Add(dr);
                                        l++;
                                    }
                                    else
                                    {
                                        if (Convert.ToBoolean(drs[0][35]) == false)
                                        {
                                            drs[0][35] = true;
                                            l++;
                                        }
                                    }
                                }
                                // bookedEventsDs.Tables[0].Merge(dsBooked.Tables[0], true, MissingSchemaAction.AddWithKey);

                                //count = adapter.Update(bookedEventsDs);
                                //ds.Merge(bookedEventsDs, true, MissingSchemaAction.AddWithKey);
                                //bookedEventsDs.Clear();

                                //if (count > -1)
                                //{
                                //    Console.WriteLine("[Success] Insert booked-events [" + count + "/" + bookedEvents.Length + "]");
                                //    Files.WriteLog("[Success] Insert booked-events [" + bookedEvents.Length + "] " + strName + ".xml");
                                //}
                                //else
                                //{
                                //    Console.WriteLine("[Failure] Insert booked-events [" + bookedEvents.Length + "]");
                                //    Files.WriteLog("[Failure] Insert booked-events [" + bookedEvents.Length + "] " + strName + ".xml");
                                //}
                            }

                            count = adapter.Update(bookedEventsDs);
                            ds = bookedEventsDs;
                            if (count > -1)
                            {
                                Files.WriteLog("[Success] Insert booked-events [" + l.ToString() + "] " + "" + ".xml");
                            }
                            else
                            {
                                Files.WriteLog("[Failure] Insert booked-events [" + l.ToString() + "] " + "" + ".xml");
                            }

                            //}
                            //else
                            //{
                            //    ds = bookedEventsDs;
                            //}
                            connection.Close();
                        }
                        break;
                    }
                //case "booked-events":
                //    {
                //        using (FbConnection connection = new FbConnection(AppFlag.ScoutsDBConn))
                //        { 
                //            queryString = "select * from " + "events";

                //            FbDataAdapter adapter = new FbDataAdapter();
                //            adapter.SelectCommand = new FbCommand(queryString, connection);
                //            FbCommandBuilder builder = new FbCommandBuilder(adapter);
                //            connection.Open();
                //             DataSet bookedEventsDs = new DataSet();
                //            adapter.Fill(bookedEventsDs);
                //            int l = bookedEventsDs.Tables[0].Rows.Count;

                //            DataSet dsBooked = bookedEventsDs.Clone();
                //            //if (bookedEventsDs.Tables[0].Rows.Count == 0)
                //            //{
                //            for (int i = 1; i < 4; i++)
                //            {
                //                var responseValue = clientTest.GetAccessData(strToken, "booked-events/" + i);
                //                var strResponseValue = responseValue.Result;

                //                string strName = type + "-" + i + " " + DateTime.Now.ToString("HHmmss");
                //                Files.WriteXml(strName, strResponseValue);

                //                DOSBookedEvents.api apis = XmlUtil.Deserialize(typeof(DOSBookedEvents.api), strResponseValue) as DOSBookedEvents.api;
                //                if (apis == null) break;
                //                DOSBookedEvents.apiDataBooked_eventsEvent[] bookedEvents = (apis.data.Length == 0) ? null : apis.data[0];
                //                if (bookedEvents == null) break;

                //                foreach (DOSBookedEvents.apiDataBooked_eventsEvent sevent in bookedEvents)
                //                {
                //                    DataRow dr = dsBooked.Tables[0].NewRow();
                //                    dr[0] = sevent.id;
                //                    dr[1] = sevent.name;
                //                    dr[2] = -1; //(participants[0].counter == "1") ? participants[0].id : participants[1].id;
                //                    dr[3] = -1; //(participants[1].counter == "2") ? participants[1].id : participants[0].id;
                //                    dr[4] = sevent.source;
                //                    dr[5] = false;//sevent.source_dc == "yes" ? true : false;
                //                    dr[6] = "";// sevent.source_super;
                //                    dr[7] = sevent.relation_status;
                //                    dr[8] = sevent.start_date;
                //                    dr[9] = sevent.ft_only == "yes" ? true : false;
                //                    dr[10] = sevent.coverage_type;
                //                    dr[11] = "";// sevent.channel_id;
                //                    dr[12] = "";// sevent.channel_name;
                //                    dr[13] = sevent.scoutsfeed == "yes" ? true : false;
                //                    dr[14] = sevent.status_id;
                //                    dr[15] = sevent.status_name;
                //                    dr[16] = sevent.status_type;
                //                    dr[17] = sevent.day;
                //                    dr[18] = sevent.clock_time;
                //                    dr[19] = sevent.clock_status;
                //                    dr[20] = sevent.winner_id;
                //                    dr[21] = sevent.progress_id;
                //                    dr[22] = sevent.bet_status;
                //                    dr[23] = sevent.neutral_venue == "yes" ? true : false;
                //                    dr[24] = sevent.item_status;
                //                    dr[25] = sevent.ut;
                //                    dr[26] = sevent.old_event_id == "" ? "-1" : sevent.old_event_id;
                //                    dr[27] = sevent.slug;
                //                    dr[28] = sevent.verified_result == "yes" ? true : false;
                //                    dr[29] = false;// sevent.is_protocol_verified == "yes" ? true : false;
                //                    dr[30] = "";// sevent.protocol_verified_by;
                //                    dr[31] = "";// sevent.protocol_verified_at;
                //                    dr[32] = sevent.round_id;
                //                    dr[33] = sevent.round_name;
                //                    dr[34] = sevent.client_event_id == "" ? "-1" : sevent.client_event_id;
                //                    dr[35] = true;// sevent.booked == "yes" ? true : false;
                //                    dr[36] = sevent.booked_by;
                //                    dr[37] = sevent.inverted_participants == "yes" ? true : false;
                //                    dr[38] = "";// sevent.venue_id;
                //                    dr[39] = -1;// group.id == "" ? "-1" : group.id;
                //                    dr[40] = sevent.stage_id;// strStage_id;
                //                    dr[41] = sevent.season_id; //strSeasons_id;
                //                    dr[42] = sevent.competition_id;// strCompetition_id;
                //                    dr[43] = sevent.area_id;// strArea_id;
                //                    dr[44] = cTimestamp;
                //                    dsBooked.Tables[0].Rows.Add(dr);
                //                    dsBooked.Tables[0].AcceptChanges();
                //                }
                //                bookedEventsDs.Tables [0].Merge(dsBooked.Tables[0], true, MissingSchemaAction.AddWithKey);

                //                //count = adapter.Update(bookedEventsDs);
                //                //ds.Merge(bookedEventsDs, true, MissingSchemaAction.AddWithKey);
                //                //bookedEventsDs.Clear();

                //                //if (count > -1)
                //                //{
                //                //    Console.WriteLine("[Success] Insert booked-events [" + count + "/" + bookedEvents.Length + "]");
                //                //    Files.WriteLog("[Success] Insert booked-events [" + bookedEvents.Length + "] " + strName + ".xml");
                //                //}
                //                //else
                //                //{
                //                //    Console.WriteLine("[Failure] Insert booked-events [" + bookedEvents.Length + "]");
                //                //    Files.WriteLog("[Failure] Insert booked-events [" + bookedEvents.Length + "] " + strName + ".xml");
                //                //}
                //            }
                //            int l2 = bookedEventsDs.Tables[0].Rows.Count;
                //            count = adapter.Update(bookedEventsDs);
                //            ds = bookedEventsDs;
                //            if (count > -1)
                //            {
                //                 Files.WriteLog("[Success] Insert booked-events [" + (l2 - l) + "] " + "" + ".xml");
                //            }
                //            else
                //            {
                //                 Files.WriteLog("[Failure] Insert booked-events [" + "" + "] " + "" + ".xml");
                //            }

                //            //}
                //            //else
                //            //{
                //            //    ds = bookedEventsDs;
                //            //}
                //            connection.Close();
                //        }
                //        break;
                //    }
                default:
                    break;
            }
            return ds;
        }

        private void bnAreas_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.Text == "Previous")
            {
                if (pageCurrent >= 0)
                {
                    pageCurrent--;
                    this.dgvEvent.currentPage--;
                    this.dgvBookedEvent .currentPage--;
                }
                if (pageCurrent <= 0)
                {
                    pageCurrent++;
                    this.dgvEvent.currentPage++ ;
                    this.dgvBookedEvent.currentPage++;
                    // MessageBox.Show("已经是第一页");
                    return;
                }
                else
                {
                    currentRow = AppFlag.iPageSize * (pageCurrent - 1);
                }
                //
                LoadData(tabControl1.SelectedTab.Text);
                // 
            }

            if (e.ClickedItem.Text == "Next")
            {
                if (pageCurrent <= pageCount)
                {
                    pageCurrent++;
                    this.dgvEvent.currentPage++;
                    this.dgvBookedEvent.currentPage++;
                }
                if (pageCurrent > pageCount)
                {
                    pageCurrent--;
                    this.dgvEvent.currentPage--;
                    this.dgvBookedEvent.currentPage--;
                    // MessageBox.Show("已经是最后一页");
                    return;
                }
                else
                {
                    currentRow = AppFlag.iPageSize * (pageCurrent - 1);
                }
                //
                nStartPos = 0;
                nEndPos = 0;
                DataTable dtTemp = tbData.Clone();
                if (pageCurrent == pageCount)
                {
                    nEndPos = total;
                }
                else
                {
                    nEndPos = AppFlag.iPageSize * pageCurrent;
                }
                nStartPos = currentRow;

                nbAreasPage.Text = "of " + pageCount.ToString();
                //bindingNavigatorCountItem.Text =  "of " + pageCount.ToString();
                if (tbData.Rows.Count == 0)
                {
                    nbAreasCurrent.Text = "0";
                }
                else
                {
                    nbAreasCurrent.Text = Convert.ToString(pageCurrent);
                }
                this.nbAreasTotal.Text = total.ToString();

                if (tbData.Rows.Count != 0)
                {
                    for (int i = nStartPos; i < nEndPos; i++)
                    {
                        dtTemp.ImportRow(tbData.Rows[i]);
                        currentRow++;
                    }
                }
                bindingSource1.DataSource = dtTemp;
                bnAreas.BindingSource = bindingSource1;
                this.dgvAreas.DataSource = bindingSource1;

            }
        }

        private void tsbGet_Click(object sender, EventArgs e)
        {
            bool done = false;
            if (tabControl1.SelectedTab == tpAreas && tsdArea.Tag == null && tsbArea.Text.Trim() == "")
            {
                DataSet ds = InsertData("areas", "all");
                //foreach (DataRow dr in ds.Tables[0].Rows)
                //{
                //    this.tsdAreaParentId.DropDownItems.Add(dr["PARENT_AREA_ID"].ToString());
                //} 
                if (tsbArea.Text.Trim().ToLower() == "") return;

                tbData = ds.Tables[0];

                BindingSource bs = new BindingSource();
                bs.DataSource = tbData.DefaultView;
                bnAreas.BindingSource = bs;
                this.dgvAreas.DataSource = bs;
                done = true;
            }
            else if ((tabControl1.SelectedTab == tpAreas && tsbArea.Text.Trim() != "") || (tabControl1.SelectedTab == tpAreas && tsdArea.Tag != null))
            {
                //DataSet ds = InsertData("areas", (tsdArea.Tag != null ? this.tsdAreaParentId.Text : tsbArea.Text.Trim()));
                DataSet ds = InsertData("areas", (tsbArea.Text.Trim() != "" ? tsbArea.Text.Trim() : this.tsdAreaParentId.Text));
                if (ds.Tables.Count == 0) return;

                tbData = ds.Tables[0];

                BindingSource bs = new BindingSource();
                bs.DataSource = tbData.DefaultView;
                bnAreas.BindingSource = bs;
                this.dgvAreas.DataSource = bs;

                //  this.tsdAreaParentId.Text = tsbArea.Text.Trim();
                done = true;
            }
            else if (tabControl1.SelectedTab == tpCompetitions)
            {
                if (tsdArea.Tag == null)
                {
                    //this.tsdAreaParentId.DropDownItems.Clear();
                    //DataSet ds1 = InsertData("areas", "all");
                    //DataTable dtnew = ds1.Tables[0].DefaultView.ToTable("PARENTID", true, new string[] { "PARENT_AREA_ID" });
                    //foreach (DataRow dr in dtnew.Rows)
                    //{
                    //    this.tsdAreaParentId.DropDownItems.Add(dr[0].ToString());
                    //}

                    return;
                }

                DataSet ds = new DataSet();
                ds = InsertData("competitions", tsdArea.Tag.ToString());
                if (ds.Tables.Count == 0) { this.dgvComp.DataSource = null; return; }
                tbData = ds.Tables[0];

                BindingSource bs = new BindingSource();
                bs.DataSource = tbData.DefaultView;
                bnAreas.BindingSource = bs;
                this.dgvComp.DataSource = bs;

                //    tsbArea.Text = this.tsdAreaParentId.Text;
                done = true;
            }
            else if (tabControl1.SelectedTab == tpSeasons)
            {
                //if (tsdArea.Tag != null && tsdComp.Tag == null)
                //{
                //    this.tsdComp.DropDownItems.Clear();
                //    DataSet ds1 = InsertData("competitions", tsdArea.Tag.ToString());
                //    if (ds1.Tables.Count != 0)
                //    {
                //        foreach (DataRow dr in ds1.Tables[0].Rows)
                //        {
                //            ToolStripMenuItem item = new ToolStripMenuItem();
                //            item.Tag = dr["ID"].ToString();
                //            item.Text = dr["NAME"].ToString();
                //            this.tsdComp.DropDownItems.Add(item);
                //        }
                //    }
                //}

                if (tsdComp.Tag == null) { this.dgvSeasons.DataSource = null; return; }

                DataSet ds = new DataSet();
                ds = InsertData("seasons", tsdComp.Tag.ToString());
                if (ds.Tables.Count == 0) { this.dgvSeasons.DataSource = null; return; }
                tbData = ds.Tables[0];

                BindingSource bs = new BindingSource();
                bs.DataSource = tbData.DefaultView;
                bnAreas.BindingSource = bs;
                this.dgvSeasons.DataSource = bs;

                done = true;
            }
            else if (tabControl1.SelectedTab == tpStages)
            {
                //if (tsdArea.Tag != null && tsdComp.Tag == null && tsdSeason.Tag == null)
                //{ 
                //    this.tsdComp.DropDownItems.Clear();
                //    DataSet ds1 = InsertData("competitions", tsdArea.Tag.ToString());
                //    if (ds1.Tables.Count != 0)
                //    {
                //        foreach (DataRow dr in ds1.Tables[0].Rows)
                //        {
                //            ToolStripMenuItem item = new ToolStripMenuItem();
                //            item.Tag = dr["ID"].ToString();
                //            item.Text = dr["NAME"].ToString();
                //            this.tsdComp.DropDownItems.Add(item);
                //        }
                //    }
                //}

                //if ( tsdComp.Tag != null && tsdSeason.Tag == null)
                //{  
                //    this.tsdSeason.DropDownItems.Clear();
                //    DataSet ds2 = InsertData("seasons", tsdComp.Tag.ToString());
                //    if (ds2.Tables.Count != 0)
                //    {
                //        foreach (DataRow dr in ds2.Tables[0].Rows)
                //        {
                //            ToolStripMenuItem item = new ToolStripMenuItem();
                //            item.Tag = dr["ID"].ToString();
                //            item.Text = dr["NAME"].ToString();
                //            this.tsdSeason.DropDownItems.Add(item);
                //        }
                //    }
                //}

                if (tsdSeason.Tag == null) { this.dgvStages.DataSource = null; return; }

                DataSet ds = new DataSet();
                ds = InsertData("stages", tsdSeason.Tag.ToString());
                if (ds.Tables.Count == 0) { this.dgvStages.DataSource = null; return; }
                tbData = ds.Tables[0];

                BindingSource bs = new BindingSource();
                bs.DataSource = tbData.DefaultView;
                bnAreas.BindingSource = bs;
                this.dgvStages.DataSource = bs;

                done = true;

            }
            else if (tabControl1.SelectedTab == tpGroups)
            {

                if (tsdStage.Tag == null) { this.dgvGroups.DataSource = null; return; }

                DataSet ds = new DataSet();
                ds = InsertData("groups", tsdStage.Tag.ToString());
                if (ds.Tables.Count == 0) { this.dgvGroups.DataSource = null; return; }
                tbData = ds.Tables[0];

                BindingSource bs = new BindingSource();
                bs.DataSource = tbData.DefaultView;
                bnAreas.BindingSource = bs;
                this.dgvGroups.DataSource = bs;

                done = true;

            }
            else if (tabControl1.SelectedTab == tpPlayer)
            {
                if (tsdSeason.Tag == null) { this.dgvPlayer.DataSource = null; return; }

                DataSet ds = new DataSet();
                ds = InsertData("participants", true, tsdSeason.Tag.ToString());
                if (ds.Tables.Count == 0) { this.dgvPlayer.DataSource = null; return; }
                tbData = ds.Tables[1];

                BindingSource bs = new BindingSource();
                bs.DataSource = tbData.DefaultView;
                bnAreas.BindingSource = bs;
                this.dgvPlayer.DataSource = bs;

                done = true;

            }
            else if (tabControl1.SelectedTab == tpTeam)
            {
                if (tsdSeason.Tag == null) { this.dgvTeam.DataSource = null; return; }

                DataSet ds = new DataSet();
                ds = InsertData("participants", true, tsdSeason.Tag.ToString());
                if (ds.Tables.Count == 0) { this.dgvTeam.DataSource = null; return; }
                tbData = ds.Tables[0];

                BindingSource bs = new BindingSource();
                bs.DataSource = tbData.DefaultView;
                bnAreas.BindingSource = bs;
                this.dgvTeam.DataSource = bs;

                done = true;

            }
            else if (tabControl1.SelectedTab == tpEvent)
            {
                ////if (tsdComp.Tag == null) { this.dgvEvent.DataSource = null; return; }

                //if (Convert.ToDateTime(this.bnAreas.Items[19].Text).Subtract(Convert.ToDateTime(this.bnAreas.Items[17].Text)).Days > 30 &&  DateTime.Now <= Convert.ToDateTime(this.bnAreas.Items[19].Text+ " 23:59:59"))
                //{
                //    MessageBox.Show("Maximum period is 30 days!"); return;
                //}
                //DataSet ds = new DataSet();
                //// ds = InsertData("events", tsdComp.Tag.ToString());
                //ds = InsertData("events", this.bnAreas.Items[17].Text+ " 00:00:00", this.bnAreas.Items[19].Text+ " 23:59:59", ((sender == null) ? false : true));
                DataSet ds = new DataSet();
                //if (sender != null && ((ToolStripButton)sender)!=null&&((ToolStripButton)sender).Name == "tsbGet2")
                //{
                if (sender != null && sender.GetType() != typeof(ToolStripButton))
                {
                    if (sender != null && ((ToolStripDropDownButton)sender).Name == "tsdGroup")
                    {
                        ds = InsertData("events", false, "Group", tsdGroup.Tag.ToString());
                    }
                    else if (sender != null && ((ToolStripDropDownButton)sender).Name == "tsdStage")
                    {
                        ds = InsertData("events", false, "Stage", tsdStage.Tag.ToString());
                    }
                    else if (sender != null && ((ToolStripDropDownButton)sender).Name == "tsdSeason")
                    {
                        ds = InsertData("events", false, "Season", tsdSeason.Tag.ToString());
                    }
                    else if (sender != null && ((ToolStripDropDownButton)sender).Name == "tsdComp")
                    {
                        ds = InsertData("events", false, "Comp", tsdComp.Tag.ToString());
                    }
                    else if (sender != null && ((ToolStripDropDownButton)sender).Name == "tsdArea")
                    { }
                }
                else if (sender != null && ((ToolStripButton)sender) != null && ((ToolStripButton)sender).Name == "tsbGet2")
                {

                    if (Convert.ToDateTime(this.bnAreas.Items[19].Text).Subtract(Convert.ToDateTime(this.bnAreas.Items[17].Text)).Days > 30 && DateTime.Now <= Convert.ToDateTime(this.bnAreas.Items[19].Text + " 23:59:59"))
                    {
                        MessageBox.Show("Maximum period is 30 days!"); return;
                    }
                    //    ds = InsertData("events", this.bnAreas.Items[17].Text + " 00:00:00", this.bnAreas.Items[19].Text + " 23:59:59", ((sender == null) ? false : true));
                    ds = InsertData("events", true, this.bnAreas.Items[17].Text + " 00:00:00", this.bnAreas.Items[19].Text + " 23:59:59");
                }
                else if (sender == null)
                {
                    ds = InsertData("events", false, this.bnAreas.Items[17].Text + " 00:00:00", this.bnAreas.Items[19].Text + " 23:59:59");
                }
                if (ds.Tables.Count == 0) { this.dgvEvent.DataSource = null; return; }
                tbData = ds.Tables[0];

                BindingSource bs = new BindingSource();
                bs.DataSource = tbData.DefaultView;
                bnAreas.BindingSource = bs;
                // this.dgvEvent.DataSource = bs;
                this.dgvEvent.MasterControls(ref ds);
                this.dgvEvent.setParentSource(ds.Tables[0].TableName, "ID");
                //  dgvEvent.childView.Add(ds.Tables[1].TableName, "TEAMS");
                //dgvEvent.childView.Add("", "TEAMS");
                done = true;
            }
            else if (tabControl1.SelectedTab == tpBook)
            {
                DataSet ds = new DataSet();
                if (sender != null && sender.GetType() != typeof(ToolStripButton))
                {
                    if (sender != null && ((ToolStripDropDownButton)sender).Name == "tsdGroup")
                    {
                        ds = InsertData("booked-events", false, "Group", tsdGroup.Tag.ToString());
                    }
                    else if (sender != null && ((ToolStripDropDownButton)sender).Name == "tsdStage")
                    {
                        ds = InsertData("booked-events", false, "Stage", tsdStage.Tag.ToString());
                    }
                    else if (sender != null && ((ToolStripDropDownButton)sender).Name == "tsdSeason")
                    {
                        ds = InsertData("booked-events", false, "Season", tsdSeason.Tag.ToString());
                    }
                    else if (sender != null && ((ToolStripDropDownButton)sender).Name == "tsdComp")
                    {
                        ds = InsertData("booked-events", false, "Comp", tsdComp.Tag.ToString());
                    }
                    else if (sender != null && ((ToolStripDropDownButton)sender).Name == "tsdArea")
                    { }
                }
                else if (sender != null && ((ToolStripButton)sender) != null && ((ToolStripButton)sender).Name == "tsbGet2")
                {

                    if (Convert.ToDateTime(this.bnAreas.Items[19].Text).Subtract(Convert.ToDateTime(this.bnAreas.Items[17].Text)).Days > 30 && DateTime.Now <= Convert.ToDateTime(this.bnAreas.Items[19].Text + " 23:59:59"))
                    {
                        MessageBox.Show("Maximum period is 30 days!"); return;
                    }
                    ds = InsertData("booked-events", true, this.bnAreas.Items[17].Text + " 00:00:00", this.bnAreas.Items[19].Text + " 23:59:59");
                }
                else if (sender == null)
                {
                    ds = InsertData("booked-events", false, this.bnAreas.Items[17].Text + " 00:00:00", this.bnAreas.Items[19].Text + " 23:59:59");
                }
                if (ds.Tables.Count == 0) { this.dgvBookedEvent.DataSource = null; return; }
                tbData = ds.Tables[0];

                BindingSource bs = new BindingSource();
                bs.DataSource = tbData.DefaultView;
                bnAreas.BindingSource = bs;
                //  this.dgvBookedEvent .DataSource = bs;
                this.dgvBookedEvent.MasterControls(ref ds);
                this.dgvBookedEvent.setParentSource(ds.Tables[0].TableName, "ID");

                done = true;
                //DataSet ds = new DataSet();
                ////  ds = InsertData("booked-events");
                //ds = InsertData("booked-events", this.bnAreas.Items[17].Text + " 00:00:00", this.bnAreas.Items[19].Text + " 23:59:59", ((sender == null) ? false : true));
                //if (ds.Tables.Count == 0) { this.dgvBookedEvent.DataSource = null; return; }
                //tbData = ds.Tables[0];

                //BindingSource bs = new BindingSource();
                //bs.DataSource = tbData.DefaultView;
                //bnAreas.BindingSource = bs;
                //this.dgvEvent.DataSource = bs;

                //done = true;
            }

            if (!done) return;
            total = tbData.Rows.Count;
            pageCount = (total / AppFlag.iPageSize);
            if ((total % AppFlag.iPageSize > 0))
            {
                pageCount++;
            }
            pageCurrent = 1;
            this.dgvEvent.currentPage = 1;
            this.dgvBookedEvent.currentPage = 1;
            currentRow = 0;

            this.LoadData(tabControl1.SelectedTab.Text);
        }

        private void tsdAreaParentId_Click(object sender, EventArgs e)
        {

        }

        private void tsdAreaParentId_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            this.tsdAreaParentId.Text = e.ClickedItem.Text;

            this.tsdArea.DropDownItems.Clear();
            DataSet ds1 = InsertData("areas", e.ClickedItem.Text);
            foreach (DataRow dr in ds1.Tables[0].Rows)
            {
                ToolStripMenuItem item = new ToolStripMenuItem();
                item.Tag = dr["ID"].ToString();
                item.Text = dr["NAME"].ToString();
                this.tsdArea.DropDownItems.Add(item);
            }
            this.tsdArea.Text = "areas";

            tsbArea.Text = this.tsdAreaParentId.Text;
        }

        private void tsdArea_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            this.tsdArea.Text = e.ClickedItem.Text;
            this.tsdArea.Tag = e.ClickedItem.Tag;

             this.tsdComp.DropDownItems.Clear();
            DataSet ds1 = InsertData("competitions", e.ClickedItem.Tag .ToString());
            if (ds1.Tables.Count != 0)
            {
                foreach (DataRow dr in ds1.Tables[0].Rows)
                {
                    ToolStripMenuItem item = new ToolStripMenuItem();
                    item.Tag = dr["ID"].ToString();
                    item.Text = dr["NAME"].ToString();
                    this.tsdComp.DropDownItems.Add(item);
                }
            }
            this.tsdComp.Text = "competitions";
            this.tsdComp.Tag  = null;

            this.tsbGet_Click(sender, e);
        }

        private void tsdSeason_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            this.tsdSeason.Text = e.ClickedItem.Text;
            this.tsdSeason.Tag = e.ClickedItem.Tag;

            this.tsdStage.DropDownItems.Clear();
            DataSet ds2 = InsertData("stages", tsdSeason.Tag.ToString());
            if (ds2.Tables.Count != 0)
            {
                foreach (DataRow dr in ds2.Tables[0].Rows)
                {
                    ToolStripMenuItem item = new ToolStripMenuItem();
                    item.Tag = dr["ID"].ToString();
                    item.Text = dr["NAME"].ToString();
                    this.tsdStage.DropDownItems.Add(item);
                }
            } 

            this.tsdStage.Text = "stages";
            this.tsdStage.Tag = null;

            this.tsbGet_Click(sender, e);
        }

        private void tsdComp_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            this.tsdComp.Text = e.ClickedItem.Text;
            this.tsdComp.Tag = e.ClickedItem.Tag;

            this.tsdSeason.DropDownItems.Clear();
            DataSet ds2 = InsertData("seasons", tsdComp.Tag.ToString());
            if (ds2.Tables.Count != 0)
            {
                foreach (DataRow dr in ds2.Tables[0].Rows)
                {
                    ToolStripMenuItem item = new ToolStripMenuItem();
                    item.Tag = dr["ID"].ToString();
                    item.Text = dr["NAME"].ToString();
                    this.tsdSeason.DropDownItems.Add(item);
                }
            }

            this.tsdSeason.Text = "seasons";
            this.tsdSeason.Tag = null;

            this.tsbGet_Click(sender, e);
        }

        private void tsdStage_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            this.tsdStage.Text = e.ClickedItem.Text;
            this.tsdStage.Tag = e.ClickedItem.Tag;

            this.tsdGroup.DropDownItems.Clear();
            DataSet ds2 = InsertData("groups", tsdStage .Tag.ToString());
            if (ds2.Tables.Count != 0)
            {
                foreach (DataRow dr in ds2.Tables[0].Rows)
                {
                    ToolStripMenuItem item = new ToolStripMenuItem();
                    item.Tag = dr["ID"].ToString();
                    item.Text = dr["NAME"].ToString();
                    this.tsdGroup.DropDownItems.Add(item);
                }
            }

            this.tsdGroup.Text = "groups";
            this.tsdGroup.Tag = null;

            this.tsbGet_Click(sender, e);
        }


        private void tsdGroup_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            this.tsdGroup.Tag = e.ClickedItem.Tag;
            this.tsdGroup.Text = e.ClickedItem.Text;
            this.tsbGet_Click(sender, e);
        }

        private void button1_Click(object sender, EventArgs e)
        { 
          //var  responseValue = clientTest.PostAccessData( this.textBox1 .Text ,"");
          //var  strResponseValue = responseValue.Result;
          //  this.label1.Text = strResponseValue.ToString();
        }

        private void tsbGet2_Click(object sender, EventArgs e)
        {
            this.tsbGet_Click(sender, e);
        }
      //  Hashtable ht;
        private void dgvEvent_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            ////  this.dgvEvent.Rows[e.RowIndex].HeaderCell.Value = "+  ";
            //if (dgvEvent.Rows[e.RowIndex]!=null&& dgvEvent.Rows[e.RowIndex].Cells[1]!= null && dgvEvent.Rows[e.RowIndex].Cells[1].Value!=null && (int)dgvEvent.Rows[e.RowIndex].Cells[1].Value == 0) 
            //{
            //    Rectangle rectangle = new Rectangle(e.RowBounds.Location.X + dgvEvent.RowHeadersWidth - 20,
            //    e.RowBounds.Location.Y + 4, 14, 14);
            //    Image img = (bool)ht[(int)dgvEvent.Rows[e.RowIndex].Cells[0].Value]
            //    ? Properties.Resources.minus : Properties.Resources.plus;
            //    e.Graphics.DrawImage(img, rectangle);
            //}
            //else
            //{
            ////    dgvEvent.Rows[e.RowIndex].Visible = (bool)ht[(int)dgvEvent.Rows[e.RowIndex].Cells[1].Value];
            //} 
        }

        private void MasterControl_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            Rectangle rectangle = new Rectangle(Conversions.ToInteger(Microsoft.VisualBasic.CompilerServices.Operators.DivideObject(Microsoft.VisualBasic.CompilerServices.Operators.SubtractObject(this.dgvEvent .rowDefaultHeight, 0x10), 2)), Conversions.ToInteger(Microsoft.VisualBasic.CompilerServices.Operators.DivideObject(Microsoft.VisualBasic.CompilerServices.Operators.SubtractObject(this.dgvEvent .rowDefaultHeight, 0x10), 2)), 0x10, 0x10);
            if (rectangle.Contains(e.Location))
            {
                if ( this.dgvEvent.rowCurrent.Contains(e.RowIndex))
                {
                    this.dgvEvent.rowCurrent.Clear();
                    this.dgvEvent.Rows[e.RowIndex].Height = Conversions.ToInteger(this.dgvEvent.rowDefaultHeight);
                    this.dgvEvent.Rows[e.RowIndex].DividerHeight = Conversions.ToInteger(this.dgvEvent.rowDefaultDivider);
                }
                else
                {

                    //ADD TEAM INFO  
                    string team_id = this.dgvEvent.Rows[e.RowIndex].Cells[0].Value.ToString(); 
                    DataSet data = new DataSet();
                    data = InsertData("events.participants", false, team_id);

                    //using (FbConnection connection = new FbConnection("User=SYSDBA;Password=masterkey;Database=E:\\Project\\Database\\SCOUTS_DB.FDB;DataSource=127.0.0.1;Port=3050;Dialect=3;Charset=NONE;Role=;Connection lifetime=15;Pooling=true;MinPoolSize=0;MaxPoolSize=50;Packet Size=8192;ServerType=0"))
                    //{
                    //    connection.Open();
                    //    string queryString = "SELECT t.* FROM teams t  inner  join  events e on   t.id= e.HOME_ID or t.id=e.GUEST_ID   where e.id='" + team_id + "' order by id asc";
                    //    using (FbCommand cmd = new FbCommand(queryString))
                    //    {
                    //        using (FbCommandBuilder fcb = new FbCommandBuilder())
                    //        {
                    //            using (FbDataAdapter fda = new FbDataAdapter())
                    //            {
                    //                cmd.Connection = connection;
                    //                fda.SelectCommand = cmd;
                    //                //using (DataSet data = new DataSet())
                    //                //{
                    //                DataSet data = new DataSet();
                    //                data.Tables.Add(new DataTable("teams"));
                    //                fda.Fill(data.Tables["teams"]);

                    //                queryString = "select   * from  players2 where team_id=" + team_id;// 174516 ";
                    //                data.Tables.Add(new DataTable("players"));
                    //                FbDataAdapter adapter1 = new FbDataAdapter();
                    //                adapter1.SelectCommand = new FbCommand(queryString, connection);
                    //                // FbCommandBuilder builder3 = new FbCommandBuilder(adapter1);
                    //                adapter1.Fill(data.Tables["players"]);

                    //                if (data.Tables[0].Rows.Count == 0)
                    //                {
                    //                    data = InsertData("participants", false, team_id);
                    //                }

                    //                if (dgvEvent.childView.TabPages.Count == 0)
                    //                {
                    //                    dgvEvent.childView.AddData(data.Tables[0], "Teams");
                    //                    dgvEvent.childView.AddData(data.Tables[1], "Players");
                    //                }
                    //                else if (dgvEvent.childView.TabPages.Count > 0 && dgvEvent.childView.TabPages[0].Text == "Teams")
                    //                {
                    //                    dgvEvent.childView.BindData(data.Tables[0], "Teams");
                    //                    dgvEvent.childView.BindData(data.Tables[1], "Players");
                    //                }
                    //                //}
                    //            }
                    //        }
                    //    }
                    //    connection.Close();
                    //}

                    if (data.Tables[0].Rows.Count == 0)
                    {
                        data = InsertData("events.participants", true, team_id);
                    }

                    if (dgvEvent.childView.TabPages.Count == 0)
                    {
                        dgvEvent.childView.AddData(data.Tables[0], "Teams");
                        dgvEvent.childView.AddData(data.Tables[1], "Players");
                    }
                    else if (dgvEvent.childView.TabPages.Count > 0 && dgvEvent.childView.TabPages[0].Text == "Teams")
                    {
                        //dgvEvent.childView.TabPages[0].Select();
                        dgvEvent.childView.BindData(data.Tables[0], "Teams");
                        // dgvEvent.childView.BindData(data.Tables[1], "Players");
                        if (dgvEvent.childView.TabPages.Count > 1 && dgvEvent.childView.TabPages[1].Text == "Players")
                        {
                            //DataGridView dgv = ((DataGridView)dgvEvent.childView.TabPages[1].Controls[0]);
                            //while (dgv.Rows.Count != 0)
                            //{
                            //    dgv.Rows.RemoveAt(0);
                            //} 
                            dgvEvent.childView.BindData(data.Tables[1], "Players");
                        }
                    }
                    

                    if (this.dgvEvent.rowCurrent.Count > 0)
                    {
                        int num = this.dgvEvent.rowCurrent[0];
                        this.dgvEvent.rowCurrent.Clear();
                        this.dgvEvent.Rows[num].Height = Conversions.ToInteger(this.dgvEvent.rowDefaultHeight);
                        this.dgvEvent.Rows[num].DividerHeight = Conversions.ToInteger(this.dgvEvent.rowDefaultDivider);
                        this.dgvEvent.ClearSelection();
                        this.dgvEvent.collapseRow = true;
                        this.dgvEvent.Rows[num].Selected = true;
                    }
                    this.dgvEvent.rowCurrent.Add(e.RowIndex);
                    this.dgvEvent.Rows[e.RowIndex].Height = Conversions.ToInteger(this.dgvEvent.rowExpandedHeight);
                    this.dgvEvent.Rows[e.RowIndex].DividerHeight = Conversions.ToInteger(this.dgvEvent.rowExpandedDivider);
                }
                this.dgvEvent.ClearSelection();
                this.dgvEvent.collapseRow = true;
                this.dgvEvent.Rows[e.RowIndex].Selected = true;
            }
            else
            {
                this.dgvEvent.collapseRow = false;
            }
        }
        private void MasterControl_RowHeaderMouseClick2(object sender, DataGridViewCellMouseEventArgs e)
        {
            Rectangle rectangle = new Rectangle(Conversions.ToInteger(Microsoft.VisualBasic.CompilerServices.Operators.DivideObject(Microsoft.VisualBasic.CompilerServices.Operators.SubtractObject(this.dgvBookedEvent.rowDefaultHeight, 0x10), 2)), Conversions.ToInteger(Microsoft.VisualBasic.CompilerServices.Operators.DivideObject(Microsoft.VisualBasic.CompilerServices.Operators.SubtractObject(this.dgvBookedEvent.rowDefaultHeight, 0x10), 2)), 0x10, 0x10);
            if (rectangle.Contains(e.Location))
            {
                if (this.dgvBookedEvent.rowCurrent.Contains(e.RowIndex))
                {
                    this.dgvBookedEvent.rowCurrent.Clear();
                    this.dgvBookedEvent.Rows[e.RowIndex].Height = Conversions.ToInteger(this.dgvBookedEvent.rowDefaultHeight);
                    this.dgvBookedEvent.Rows[e.RowIndex].DividerHeight = Conversions.ToInteger(this.dgvBookedEvent.rowDefaultDivider);
                }
                else
                { 
                    //ADD TEAM INFO  
                    string team_id = this.dgvBookedEvent.Rows[e.RowIndex].Cells[0].Value.ToString();
                    DataSet data = new DataSet();
                    data = InsertData("events.participants", false, team_id);
                     
                    if (data.Tables[0].Rows.Count == 0)
                    {
                        data = InsertData("events.participants", true, team_id);
                    }

                    if (dgvBookedEvent.childView.TabPages.Count == 0)
                    {
                        dgvBookedEvent.childView.AddData(data.Tables[0], "Teams");
                        dgvBookedEvent.childView.AddData(data.Tables[1], "Players");
                    }
                    else if (dgvBookedEvent.childView.TabPages.Count > 0 && dgvBookedEvent.childView.TabPages[0].Text == "Teams")
                    {
                          dgvBookedEvent.childView.BindData(data.Tables[0], "Teams");
                         if (dgvBookedEvent.childView.TabPages.Count > 1 && dgvBookedEvent.childView.TabPages[1].Text == "Players")
                        { 
                            dgvBookedEvent.childView.BindData(data.Tables[1], "Players");
                        }
                    }


                    if (this.dgvBookedEvent.rowCurrent.Count > 0)
                    {
                        int num = this.dgvBookedEvent.rowCurrent[0];
                        this.dgvBookedEvent.rowCurrent.Clear();
                        this.dgvBookedEvent.Rows[num].Height = Conversions.ToInteger(this.dgvBookedEvent.rowDefaultHeight);
                        this.dgvBookedEvent.Rows[num].DividerHeight = Conversions.ToInteger(this.dgvBookedEvent.rowDefaultDivider);
                        this.dgvBookedEvent.ClearSelection();
                        this.dgvBookedEvent.collapseRow = true;
                        this.dgvBookedEvent.Rows[num].Selected = true;
                    }
                    this.dgvBookedEvent.rowCurrent.Add(e.RowIndex);
                    this.dgvBookedEvent.Rows[e.RowIndex].Height = Conversions.ToInteger(this.dgvBookedEvent.rowExpandedHeight);
                    this.dgvBookedEvent.Rows[e.RowIndex].DividerHeight = Conversions.ToInteger(this.dgvBookedEvent.rowExpandedDivider);
                }
                this.dgvBookedEvent.ClearSelection();
                this.dgvBookedEvent.collapseRow = true;
                this.dgvBookedEvent.Rows[e.RowIndex].Selected = true;
            }
            else
            {
                this.dgvBookedEvent.collapseRow = false;
            }
        }

    }



    class utc

    {

        public static int ConvertDateTimeInt(System.DateTime time)

        {

            double intResult = 0;

            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));

            intResult = (time - startTime).TotalSeconds;

            return (int)intResult;

        }



        public static DateTime ConvertIntDatetime(double utc)

        {

            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));

            startTime = startTime.AddSeconds(utc);

            startTime = startTime.AddHours(8);//转化为北京时间(北京时间=UTC时间+8小时 )            

            return startTime;

        }
    }

        class OAuthClient
    {
        private static  HttpClient _httpClient;
        public string token;

        public OAuthClient()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(AppFlag .ScoutsUrl);
        }

        public async Task<string> GetAccessToken()
        {
            var client_id = AppFlag.Client_id;
            var secret_key = AppFlag.Secret_key;
            var response = await _httpClient.GetAsync($"/v2/oauth?client_id={client_id}&secret_key={secret_key}").ConfigureAwait(false);
            var responseValue = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return responseValue;
            }
            else
            {
                //    return string.Empty;
                return responseValue;
            }
        }

        public async Task<string> PostAccessData(string token, string type, params string[] arr)
        {
            string strUrl = "";
            var parameters = new Dictionary<string, string>();

            if (type.IndexOf("booked-events") > -1)
            {
                strUrl = $"/v2/booked-events/";
                parameters.Add("product", "scoutsfeed");
                parameters.Add("token", token);
                Files.WriteLog("GET booked-events " + strUrl);
            }

            //var client_id = AppFlag.Client_id;
            //var secret_key = AppFlag.Secret_key;

            //var parameters = new Dictionary<string, string>();
            // parameters.Add("event_id", id);
            //parameters.Add("product", "scoutsfeed");
            //parameters.Add("token", token); 

            //var response = await _httpClient.PostAsync($"/v2/booked-events/", new FormUrlEncodedContent(parameters)).ConfigureAwait(false);
            // var response = await _httpClient.PostAsync($"/v2/booked-events/"+ id , new FormUrlEncodedContent(parameters)).ConfigureAwait(false);
            var response = await _httpClient.PostAsync(strUrl, new FormUrlEncodedContent(parameters)).ConfigureAwait(false);
            var responseValue = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return responseValue;
            }
            else
            {
               // return responseValue;
                return string.Empty;
            }
        }

        public async Task<string> GetAccessData(string token, string type, params object[] arr)
        {
            string strUrl = "";
            if (type.IndexOf("areas") > -1)
            {
                //(arr[0] == "all" ? "" : " where PARENT_AREA_ID=" + arr[0])
                //strUrl = $"/v2/" + ((type.IndexOf("/") > -1) ? "areas.xml?parent_area_id=" + type.Substring(type.IndexOf("/") + 1, type.Length - type.IndexOf("/") - 1) : type + ".xml");
                strUrl = $"/v2/" + ((arr[0].ToString().ToLower() != "all") ? "areas.xml?parent_area_id=" + arr[0] : type + ".xml");
                //Console.WriteLine("GET areas " + strUrl);
                Files.WriteLog("GET areas " + strUrl);
            }
            else if (type.IndexOf("competitions") > -1)
            {
                strUrl = $"/v2/competitions.xml?token=" + token + "&sport_id=5&area_id=" + arr[0] + "&page=" + type.Substring(type.IndexOf("/") + 1, type.Length - type.IndexOf("/") - 1);
                // Console.WriteLine("GET competitions " + strUrl);
                Files.WriteLog("GET competitions " + strUrl);
            }
            else if (type.IndexOf("seasons") > -1)
            {
                strUrl = $"/v2/seasons.xml?token=" + token + "&sport_id=5&competition_id=" + arr[0] + "&page=" + type.Substring(type.IndexOf("/") + 1, type.Length - type.IndexOf("/") - 1);
                Console.WriteLine("GET seasons " + strUrl);
                Files.WriteLog("GET seasons " + strUrl);
            }

            else if (type.IndexOf("stages") > -1)
            {
                strUrl = $"/v2/stages.xml?token=" + token + "&sport_id=5&season_id=" + arr[0] + "&page=" + type.Substring(type.IndexOf("/") + 1, type.Length - type.IndexOf("/") - 1);
                Files.WriteLog("GET stages " + strUrl);
            }
            else if (type.IndexOf("groups") > -1)
            {
                strUrl = $"/v2/groups.xml?token=" + token + "&sport_id=5&stage_id=" + arr[0] + "&page=" + type.Substring(type.IndexOf("/") + 1, type.Length - type.IndexOf("/") - 1);
                Files.WriteLog("GET groups " + strUrl);
            }
            else if (type.IndexOf("booked-events") > -1)
            {
                strUrl = $"/v2/booked-events.xml?product=scoutsfeed&client_id=" + AppFlag.Client_id + "&page=" + type.Substring(type.IndexOf("/") + 1, type.Length - type.IndexOf("/") - 1);
                Files.WriteLog("GET booked-events " + strUrl);
            }
            else if (type.IndexOf("events") > -1)
            {
                // strUrl = $"/v2/events.xml?token=" + token + "&sport_id=5&tz=Asia/Hong_Kong&competition_id=" + arr[0] + "&page=" + type.Substring(type.IndexOf("/") + 1, type.Length - type.IndexOf("/") - 1);
                // "/v2/events.xml?token=&sport_id=5&tz=Asia/Hong_Kong&date_from=9/19/2018 00:00:00&date_to=10/11/2018 23:59:59&page=1"
                strUrl = $"/v2/events.xml?token=" + token + "&sport_id=5&tz=Asia/Hong_Kong&date_from=" + Convert.ToDateTime(arr[0]).ToString("yyyy-MM-dd HH:mm:ss") + "&date_to=" + Convert.ToDateTime(arr[1]).ToString("yyyy-MM-dd HH:mm:ss") + "&page=" + type.Substring(type.IndexOf("/") + 1, type.Length - type.IndexOf("/") - 1);
                Console.WriteLine("GET events " + strUrl);
                Files.WriteLog("GET events " + strUrl);
            }
            else if (type.IndexOf("participants") > -1)
            {
                if (Convert.ToBoolean(arr[0]) == false)
                {
                    strUrl = $"/v2/events/" + arr[1] + "/participants.xml?token=" + token + "&page=" + type.Substring(type.IndexOf("/") + 1, type.Length - type.IndexOf("/") - 1);
                }
                else
                {
                    strUrl = $"/v2/participants.xml?token=" + token + "&sport_id=5&season_id=" + arr[1] + "&page=" + type.Substring(type.IndexOf("/") + 1, type.Length - type.IndexOf("/") - 1);
                }
                Files.WriteLog("GET participants " + strUrl);
            }

            var response = await _httpClient.GetAsync(strUrl).ConfigureAwait(false);
            var responseValue = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return responseValue;
            }
            else
            {
                //  Console.WriteLine("Empty: " + responseValue);
                // return string.Empty;
                return response.StatusCode.ToString();
            }



            //switch (type.Substring(0, 5))
            //{
            //    case "areas":
            //        strUrl = $"/v2/" + type + ".xml?parent_area_id=209";
            //        break;
            //    default:
            //        break;
            //}

        }

        //public async Task Call_WebAPI_By_Resource_Owner_Password_Credentials_Grant()
        //{
        //    var client_id = "262";
        //    var secret_key = "ksUzQEuI5neLBOEgX6ZdqisPfC5Lv91u2TE";
        //    if (string.IsNullOrEmpty(token))
        //        token = await GetAccessToken();
        //    Console.WriteLine(token);
        //    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        //    Console.WriteLine(await (await _httpClient.GetAsync($"/v2/oauth?client_id={client_id}&secret_key={secret_key}")).Content.ReadAsStringAsync());
        //}

        //public async Task<string> Call_WebAPIByType(string token, string type)
        //{
        //    switch (type)
        //    {
        //        case "areas":
        //            //https://api.statscore.com/v2/areas.xml
        //            //https://api.statscore.com/v2/areas.xml?parent_area_id=209
        //            var returns = (await (await _httpClient.GetAsync($"/v2/areas.xml?parent_area_id=209")).Content.ReadAsStringAsync());
        //            Console.WriteLine(returns);

        //            Console.WriteLine("-----------------------------areas----------------------------");

        //            break;
        //        default:
        //            break;
        //    }
        //    return "";
        //}

    }
}
