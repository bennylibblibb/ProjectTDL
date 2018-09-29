using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text;
using System.IO;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using FileLog;
using System.Net.Http.Headers;
using System.Data;
using FirebirdSql.Data.FirebirdClient;

namespace DataOfScouts
{
    public partial class DataOfScouts : Form
    {
        string strToken = "";
        OAuthClient clientTest = new OAuthClient();

        public DataOfScouts()
        {
            InitializeComponent();
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

                this.tsbGet_Click(sender, e);
            }
            else if (tabControl1.SelectedTab == tpPlayer || tabControl1.SelectedTab == tpTeam)
            {
                this.bnAreas2.Visible = true;
                this.bnAreas.Visible = true;

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

                this.tsbGet_Click(sender, e);

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
        }

        private void tabControl1_Selected(object sender, TabControlEventArgs e)
        {
            string str = "";
        }

        private void ClientAuthorize()
        {
            var responseValue = clientTest.GetAccessToken();
            var strResponseValue = responseValue.Result;
            var api = JObject.Parse(strResponseValue)["api"];
            var data = JObject.Parse(api.ToString())["data"];
            strToken = JObject.Parse(data.ToString())["token"].Value<string>();
            this.lbAuthorization.Text = "Authorized";
            this.lbToken.Text = strToken;

            Files.WriteLog("[Success] Authorized: " + strToken);
        }
        // private DataSet InsertData(int iPage, string responsValue, string type)
        //private DataSet InsertData(string type)
        private DataSet InsertData(string type, params string[] arr)
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
                            queryString = "select * from " + type + (arr[0] == "all" ? "" : " where PARENT_AREA_ID=" + arr[0]);
                            FbDataAdapter adapter = new FbDataAdapter();
                            adapter.SelectCommand = new FbCommand(queryString, connection);
                            FbCommandBuilder builder = new FbCommandBuilder(adapter);
                            connection.Open();
                            DataSet areasDs = new DataSet();
                            adapter.Fill(areasDs);

                            if (areasDs.Tables[0].Rows.Count == 0)
                            {
                                var responseValue = clientTest.GetAccessData(strToken, "areas", arr[0]);
                                var strResponseValue = responseValue.Result;
                                DOSAreas.api apis = XmlUtil.Deserialize(typeof(DOSAreas.api), strResponseValue) as DOSAreas.api;
                                if (apis.data.Length == 0) return ds;
                                DOSAreas.apiDataAreasArea[] areas = apis.data[0];
                                if (areas == null) return ds;

                                string strName = type + "-" + DateTime.Now.ToString("HHmmss");
                                Files.WriteXml(strName, strResponseValue);
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
                                    DOSCompetitions.api apis = XmlUtil.Deserialize(typeof(DOSCompetitions.api), strResponseValue) as DOSCompetitions.api;
                                    if (apis == null) break;  
                                    DOSCompetitions.apiDataCompetitionsCompetition[] competitions = (apis.data.Length == 0) ? null : apis.data[0];
                                    if (competitions == null) break;

                                    string strName = type + "-" + i + " " + DateTime.Now.ToString("HHmmss");
                                    Files.WriteXml(strName, strResponseValue);

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
                                    DOSSeasons.api apis = XmlUtil.Deserialize(typeof(DOSSeasons.api), strResponseValue) as DOSSeasons.api;
                                    DOSSeasons.apiDataCompetitionsCompetition[] competitions = (apis.data.Length == 0) ? null : apis.data[0];
                                    if (competitions == null) break;
                                    string strName = type + "-" + i + " " + DateTime.Now.ToString("HHmmss");
                                    Files.WriteXml(strName, strResponseValue);

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
                                    
                                    DOSStages.api apis = XmlUtil.Deserialize(typeof(DOSStages.api), strResponseValue) as DOSStages.api;
                                      if (apis == null) break;  
                                    DOSStages.apiDataCompetition [] competitions = (apis.data.Length == 0) ? null : apis.data;
                                    if (competitions == null) break;
                                    string strName = type + "-" + i + " " + DateTime.Now.ToString("HHmmss");
                                    Files.WriteXml(strName, strResponseValue);

                                    foreach (DOSStages.apiDataCompetition  competition in competitions)
                                    {
                                        string strCompetition_id = competition.id;
                                        string strArea_id = competition.area_id;
                                        DOSStages.apiDataCompetitionSeason [] seasons = competition.season ;
                                        if (seasons == null) continue;
                                        foreach (DOSStages.apiDataCompetitionSeason season in seasons)
                                        {
                                            string strSeasons_id = season.id;
                                            DOSStages.apiDataCompetitionSeasonStage  [] stages = season.stages  ;
                                            if (stages == null) continue;
                                            foreach (DOSStages.apiDataCompetitionSeasonStage  stage in stages)
                                            {
                                                DataRow dr = stagesDs.Tables[0].NewRow();
                                                dr[0] = stage.id;
                                                dr[1] = stage.stage_name_id ; 
                                                dr[2] = stage.name;
                                                dr[3] = stage.start_date;
                                                dr[4] = stage.end_date ;
                                                dr[5] = stage.show_standings =="yes" ? true:false;
                                                dr[6] = stage.groups_nr ;
                                                dr[7] = stage.sort ;
                                                dr[8] = stage.is_current  == "yes" ? true : false;
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

                                    string strName = type + "-" + i + " " + DateTime.Now.ToString("HHmmss");
                                    Files.WriteXml(strName, strResponseValue);

                                    DOSGroups.api apis = XmlUtil.Deserialize(typeof(DOSGroups.api), strResponseValue) as DOSGroups.api;
                                    // if (apis == null) return ds;
                                    if (apis == null) break;
                                    DOSGroups.apiDataCompetition [] competitions = (apis.data.Length == 0) ? null : apis.data;
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
                                            DOSGroups.apiDataCompetitionSeasonStage[] stages = season.stage ;
                                            if (stages == null) continue;
                                            foreach (DOSGroups.apiDataCompetitionSeasonStage stage in stages)
                                            {
                                                string strStage_id = stage.id;
                                                DOSGroups.apiDataCompetitionSeasonStageGroup[] groups = stage.groups ;
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

                    //https://api.statscore.com/v2/events.xml?token=c48ad02061e4610726188d8df4b7eea0&sport_id=5&area_id=149&lang=zh&page=1&competition_id=2183 today
                case "events":
                    {
                        using (FbConnection connection = new FbConnection(AppFlag.ScoutsDBConn))
                        {
                            queryString = "select * from " + type + " where  COMPETITION_ID=" + arr[0];

                            FbDataAdapter adapter = new FbDataAdapter();
                            adapter.SelectCommand = new FbCommand(queryString, connection);
                            FbCommandBuilder builder = new FbCommandBuilder(adapter);
                            connection.Open();
                            DataSet eventsDs = new DataSet();
                            adapter.Fill(eventsDs);

                            if (eventsDs.Tables[0].Rows.Count == 0)
                            {
                                for (int i = 1; i < 4; i++)
                                {
                                    var  responseValue = clientTest.GetAccessData(strToken, "events/" + i,arr[0]);
                                    var  strResponseValue = responseValue.Result; 
                                    string strName = type + "-" + i + " " + DateTime.Now.ToString("HHmmss");
                                    Files.WriteXml(strName, strResponseValue);

                                    DOSEvents.api apis = XmlUtil.Deserialize(typeof(DOSEvents.api), strResponseValue) as DOSEvents.api;
                                    DOSEvents.apiDataCompetition[] competitions = (apis.data.Length == 0) ? null : apis.data[0];
                                    if (competitions == null) break;

                                    foreach (DOSEvents.apiDataCompetition competition in competitions)
                                    {
                                        string strCompetition_id = competition.id;
                                        string strArea_id = competition.area_id;
                                        DOSEvents.apiDataCompetitionSeason[] seasons = competition.seasons;
                                        if (seasons == null) continue;
                                        foreach (DOSEvents.apiDataCompetitionSeason season in seasons)
                                        {
                                            string strSeasons_id = season.id;
                                            DOSEvents.apiDataCompetitionSeasonStage[] stages = season.stages;
                                            if (stages == null) continue;
                                            foreach (DOSEvents.apiDataCompetitionSeasonStage stage in stages)
                                            {
                                                string strStage_id = stage.id;
                                                DOSEvents.apiDataCompetitionSeasonStageGroup[] groups = stage.groups;
                                                if (groups == null) continue;
                                                foreach (DOSEvents.apiDataCompetitionSeasonStageGroup group in groups)
                                                {
                                                    foreach (DOSEvents.apiDataCompetitionSeasonStageGroupEvent sevent in group.events)
                                                    {
                                                        if (sevent == null) continue;
                                                        if (eventsDs.Tables[0].Select("id="+sevent.id).Length == 0)
                                                        {
                                                            DataRow dr = eventsDs.Tables[0].NewRow();
                                                            dr[0] = sevent.id;
                                                            dr[1] = sevent.name ;
                                                            dr[2] = sevent.id;
                                                            dr[3] = sevent.name;
                                                            dr[2] = strStage_id;
                                                            dr[3] = strSeasons_id;
                                                            dr[5] = strCompetition_id;
                                                            dr[6] = strArea_id;
                                                            dr[7] = cTimestamp;
                                                            dr[8] = cTimestamp;
                                                            eventsDs.Tables[0].Rows.Add(dr);
                                                        }
                                                        else
                                                        {

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
                                        Console.WriteLine("[Success] Insert events [" + count + " " + "]");
                                        Files.WriteLog("[Success] Insert events[" + count + "  " + "] " + " " + strName + ".xml");
                                    }
                                    else
                                    {
                                        Console.WriteLine("[Failure] Insert events [  ]");
                                        Files.WriteLog("[Failure] Insert events [  ]" + " " + strName + ".xml");
                                    }
                                }
                            }
                            else
                            {
                                count = -1;
                                queryString = "select   * from " + type;
                                adapter.SelectCommand = new FbCommand(queryString, connection);
                                adapter.Fill(eventsDs);
                                ds = eventsDs;
                            }
                            connection.Close();
                        }
                        break;
                    }

                case "participants":
                    {
                        using (FbConnection connection = new FbConnection(AppFlag.ScoutsDBConn))
                        {
                           queryString = "select * from  teams where season_id=" + arr[0];

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
                                            adapter1.SelectCommand = new FbCommand(" SELECT   * FROM players" + " where season_id=" + arr[0], connection);
                                            FbCommandBuilder builder3 = new FbCommandBuilder(adapter1);
                                            adapter1.Fill(data.Tables["players"]);

                                            if (data.Tables["teams"].Rows.Count == 0 && data.Tables["players"].Rows.Count == 0)
                                            {
                                                for (int i = 1; i < 2869; i++)
                                                {
                                                    var responseValue = clientTest.GetAccessData(strToken, "participants/" + i, arr[0]);
                                                    var strResponseValue = responseValue.Result;
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
                                                            dr[6] = arr[0];
                                                            dr[7] = (participant.national.ToLower() == "yes") ? true : false;
                                                            dr[8] = participant.ut;
                                                            dr[9] = (participant.old_participant_id == "") ? "-1" : participant.old_participant_id;
                                                            dr[10] = participant.slug;
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
                                                            dr2[8] = arr[0];
                                                            dr2[9] = (participant.national.ToLower() == "yes") ? true : false;
                                                            dr2[10] = participant.ut;
                                                            dr2[11] = (participant.old_participant_id == "") ? "-1" : participant.old_participant_id;
                                                            dr2[12] = participant.slug;
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

                default:
                    break;
            }
            //if (count > -1)
            //{
            //    Files.WriteLog("[Success] Insert " + type + " [" + count + "] " + strName + ".xml");
            //}
            //else
            //{
            //    Files.WriteLog("[Failure] Insert " + type + " [" + count + "] " + strName + ".xml");
            //}
            return ds;
        }

        private void bnAreas_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.Text == "Previous")
            {
                if (pageCurrent >= 0)
                {
                    pageCurrent--;
                }
                if (pageCurrent <= 0)
                {
                    pageCurrent++;
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
                }
                if (pageCurrent > pageCount)
                {
                    pageCurrent--;
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
                DataSet ds = InsertData("areas", (tsdArea.Tag != null ? this.tsdAreaParentId.Text : tsbArea.Text.Trim()));

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
                ds = InsertData("stages", tsdSeason .Tag.ToString());
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
            else if (tabControl1.SelectedTab == tpPlayer )
            {
                if (tsdSeason.Tag == null) { this.dgvPlayer.DataSource = null; return; }

                DataSet ds = new DataSet();
                ds = InsertData("participants", tsdSeason.Tag.ToString());
                if (ds.Tables.Count == 0) { this.dgvPlayer.DataSource = null; return; }
                tbData = ds.Tables[1];

                BindingSource bs = new BindingSource();
                bs.DataSource = tbData.DefaultView;
                bnAreas.BindingSource = bs;
                this.dgvPlayer.DataSource = bs;

                done = true;

            }
            else if (tabControl1.SelectedTab == tpTeam )
            {
                if (tsdSeason.Tag == null) { this.dgvTeam.DataSource = null; return; }

                DataSet ds = new DataSet();
                ds = InsertData("participants", tsdSeason.Tag.ToString());
                if (ds.Tables.Count == 0) { this.dgvTeam.DataSource = null; return; }
                tbData = ds.Tables[0];

                BindingSource bs = new BindingSource();
                bs.DataSource = tbData.DefaultView;
                bnAreas.BindingSource = bs;
                this.dgvTeam.DataSource = bs;

                done = true;

            }
            else if (tabControl1.SelectedTab == tpEvent )
            { 
                if (tsdComp.Tag == null) { this.dgvEvent.DataSource = null; return; }

                DataSet ds = new DataSet();
                ds = InsertData("events", tsdComp.Tag.ToString());
                if (ds.Tables.Count == 0) { this.dgvEvent.DataSource = null; return; }
                tbData = ds.Tables[0];

                BindingSource bs = new BindingSource();
                bs.DataSource = tbData.DefaultView;
                bnAreas.BindingSource = bs;
                this.dgvEvent.DataSource = bs;

                done = true;
            }

            if (!done) return;
            total = tbData.Rows.Count;
            pageCount = (total / AppFlag.iPageSize);
            if ((total % AppFlag.iPageSize > 0))
            {
                pageCount++;
            }
            pageCurrent = 1;
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
            DataSet ds2 = InsertData("seasons", tsdStage .Tag.ToString());
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
    }

    class OAuthClient
    {
        private static  HttpClient _httpClient;
       // private string token;

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
                return string.Empty;
            }
        }

        public async Task<string> GetAccessData(string token, string type, params string[] arr)
        {
            string strUrl = "";
            if (type.IndexOf("areas") > -1)
            {
                //(arr[0] == "all" ? "" : " where PARENT_AREA_ID=" + arr[0])
                //strUrl = $"/v2/" + ((type.IndexOf("/") > -1) ? "areas.xml?parent_area_id=" + type.Substring(type.IndexOf("/") + 1, type.Length - type.IndexOf("/") - 1) : type + ".xml");
                 strUrl = $"/v2/" +((arr[0]!="all") ? "areas.xml?parent_area_id=" + arr[0] : type + ".xml");
                //Console.WriteLine("GET areas " + strUrl);
                Files.WriteLog("GET areas " + strUrl);
            }
            else if (type.IndexOf("competitions") > -1)
            {
                strUrl = $"/v2/competitions.xml?token=" + token + "&sport_id=5&area_id=" + arr [0]+"&page=" + type.Substring(type.IndexOf("/") + 1, type.Length - type.IndexOf("/") - 1);
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

            else if (type.IndexOf("events") > -1)
            {
                strUrl = $"/v2/events.xml?token=" + token + "&sport_id=5&page=" + type.Substring(type.IndexOf("/") + 1, type.Length - type.IndexOf("/") - 1);
                Console.WriteLine("GET events " + strUrl);
                Files.WriteLog("GET events " + strUrl);
            }
            else if (type.IndexOf("participants") > -1)
            {
                strUrl = $"/v2/participants.xml?token=" + token + "&sport_id=5&season_id=" + arr[0] + "&page=" + type.Substring(type.IndexOf("/") + 1, type.Length - type.IndexOf("/") - 1);
                Console.WriteLine("GET participants " + strUrl);
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
                Console.WriteLine("Empty: " + responseValue);
                return string.Empty;
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
