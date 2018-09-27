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
            //  ClientAuthorize(); 

            this.bnAreas2.Visible = false;
            this.bnAreas.Visible = false;
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


                this.bnAreas2.Visible = true;
                this.bnAreas.Visible = true;

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
            }
            else if (tabControl1.SelectedTab == tpParticipants)
            {
                //var responseValue = clientTest.GetAccessData(strToken, "participants/" + "1");
                //var strResponseValue = responseValue.Result; 
                //string strName = "participants" + "-" + "1" + " " + DateTime.Now.ToString("HHmmss"); 
                //Files.WriteXml(strName, strResponseValue);
                ////   TableGenerator.TableGenerators(typeof(DOSEvents.apiDataCompetitionSeasonStageGroupEvent ));
                //DOSParticipants.api apis = XmlUtil.Deserialize(typeof(DOSParticipants.api), strResponseValue) as DOSParticipants.api;
                //DOSParticipants.apiDataParticipant[] participants = (apis.data.Length == 0) ? null : apis.data[0];

                //TableGenerator.TableGenerators(typeof(DOSParticipants.apiDataParticipant));

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

                this.bnAreas2.Visible = true;
                this.bnAreas.Visible = true;

                DataSet ds = new DataSet();
                ds = InsertData("participants");
                //  this.dgvPart.DataSource = ds.Tables[0].DefaultView;
                tbData = ds.Tables[0];

                BindingSource bs = new BindingSource();
                bs.DataSource = tbData.DefaultView;
                bnAreas.BindingSource = bs;
                this.dgvPart.DataSource = bs;
            }
            else if (tabControl1.SelectedTab == tpEvent)
            {
                tsdAreaParentId.Visible = true;
                tsbArea.Visible = false;
                tslArea.Visible = false;
                tsdArea.Visible = true;
                tsdComp.Visible = true;
                tsdSeason.Visible = true;
                tsdStage.Visible = true;
                tsdGroup.Visible = true;
                tsdPartic.Visible = true;
                tsdEvent.Visible = false;

                this.bnAreas2.Visible = true;
                this.bnAreas.Visible = true;

                var responseValue = clientTest.GetAccessData(strToken, "events/" + "1");
                var strResponseValue = responseValue.Result;

                TableGenerator.TableGenerators(typeof(DOSEvents.apiDataCompetitionSeasonStageGroupEvent));
                return;

                string strName = "events" + "-" + "1" + " " + DateTime.Now.ToString("HHmmss");
                Files.WriteXml(strName, strResponseValue);
                this.bnAreas.Visible = true;
                DataSet ds = new DataSet();

                ds = InsertData("events");
                //  this.dgvEvent.DataSource = ds.Tables[0].DefaultView;
                tbData = ds.Tables[0];

                BindingSource bs = new BindingSource();
                bs.DataSource = tbData.DefaultView;
                bnAreas.BindingSource = bs;
                this.dgvEvent.DataSource = bs;
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
            else if (type == "participants")
            {
                dgvPart.DataSource = bindingSource1;
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
        private DataSet InsertData(string type)
        {
            //string strName = type + "-" + iPage + " " + DateTime.Now.ToString("HHmmss");
            //Files.WriteXml(strName, responsValue);

            string queryString = "select FIRST 1 * from " + type;
            DateTime cTimestamp = DateTime.Now;
            DataSet ds = new DataSet();
            int count = 0;

            switch (type)
            {
                case "areas":
                    {
                        using (FbConnection connection = new FbConnection(AppFlag.ScoutsDBConn))
                        {
                            FbDataAdapter adapter = new FbDataAdapter();
                            adapter.SelectCommand = new FbCommand(queryString, connection);
                            FbCommandBuilder builder = new FbCommandBuilder(adapter);
                            connection.Open();
                            DataSet areasDs = new DataSet();
                            adapter.Fill(areasDs);
                            if (areasDs.Tables[0].Rows.Count == 0)
                            {
                                var responseValue = clientTest.GetAccessData(strToken, "areas");
                                var strResponseValue = responseValue.Result;
                                DOSAreas.api apis = XmlUtil.Deserialize(typeof(DOSAreas.api), strResponseValue) as DOSAreas.api;
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
                                count = -1;
                                areasDs.Clear();
                                queryString = "select   * from " + type;
                                adapter.SelectCommand = new FbCommand(queryString, connection);
                                adapter.Fill(areasDs);
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
                            FbDataAdapter adapter = new FbDataAdapter();
                            adapter.SelectCommand = new FbCommand(queryString, connection);
                            FbCommandBuilder builder = new FbCommandBuilder(adapter);
                            connection.Open();
                            DataSet competitionDs = new DataSet();
                            adapter.Fill(competitionDs);
                            for (int i = 1; i < 4; i++)
                            {
                                if (competitionDs.Tables[0].Rows.Count == 0)// && iPage == 1)
                                {
                                    var responseValue = clientTest.GetAccessData(strToken, "competitions/" + i);
                                    var strResponseValue = responseValue.Result;
                                    DOSCompetitions.api apis = XmlUtil.Deserialize(typeof(DOSCompetitions.api), strResponseValue) as DOSCompetitions.api;
                                    DOSCompetitions.apiDataCompetitionsCompetition[] competitions = (apis.data.Length == 0) ? null : apis.data[0];
                                    if (competitions == null) return ds;

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
                                }
                                else
                                {
                                    count = -1;
                                    queryString = "select   * from " + type;
                                    adapter.SelectCommand = new FbCommand(queryString, connection);
                                    adapter.Fill(competitionDs);
                                    ds = competitionDs;
                                    break;
                                }
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
                                    var responseValue = clientTest.GetAccessData(strToken, "seasons/" + i);
                                    var strResponseValue = responseValue.Result;
                                    //DOSSeasons.api apis = XmlUtil.Deserialize(typeof(DOSSeasons.api), strResponseValue) as DOSSeasons.api;
                                    //DOSSeasons.apiDataCompetitionsCompetition[] competitions = (apis.data.Length == 0) ? null : apis.data[0];
                                    // if (competitions == null) return ds;
                                    DOSSeasons.api apis = XmlUtil.Deserialize(typeof(DOSSeasons.api), strResponseValue) as DOSSeasons.api;
                                    DOSSeasons.apiDataCompetitionsCompetition[] competitions = (apis.data.Length == 0) ? null : apis.data[0];
                                    if (competitions == null) return ds;
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
                                count = -1;
                                queryString = "select   * from " + type;
                                adapter.SelectCommand = new FbCommand(queryString, connection);
                                adapter.Fill(seasonsDs);
                                ds = seasonsDs;
                            }
                            connection.Close();
                        }
                        break;
                    }
                case "events":
                    {
                        using (FbConnection connection = new FbConnection(AppFlag.ScoutsDBConn))
                        {
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
                                    var responseValue = clientTest.GetAccessData(strToken, "events/" + i);
                                    var strResponseValue = responseValue.Result;

                                    DOSSeasons.api apis = XmlUtil.Deserialize(typeof(DOSSeasons.api), strResponseValue) as DOSSeasons.api;
                                    DOSSeasons.apiDataCompetitionsCompetition[] competitions = (apis.data.Length == 0) ? null : apis.data[0];
                                    if (competitions == null) return ds;
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
                                adapter.Fill(seasonsDs);
                                ds = seasonsDs;
                            }
                            connection.Close();
                        }
                        break;
                    }

                case "participants":
                    {
                        using (FbConnection connection = new FbConnection(AppFlag.ScoutsDBConn))
                        {
                            string query = "SELECT FIRST 1 *  FROM teams";
                            using (FbCommand cmd = new FbCommand(query))
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
                                            adapter1.SelectCommand = new FbCommand(" SELECT FIRST 1 * FROM players", connection);
                                            FbCommandBuilder builder3 = new FbCommandBuilder(adapter1);
                                            adapter1.Fill(data.Tables["players"]);

                                            if (data.Tables["teams"].Rows.Count == 0 && data.Tables["players"].Rows.Count == 0)
                                            {
                                                for (int i = 1; i < 2869; i++)
                                                {
                                                    var responseValue = clientTest.GetAccessData(strToken, "participants/" + i);
                                                    var strResponseValue = responseValue.Result;
                                                    DOSParticipants.api apis = XmlUtil.Deserialize(typeof(DOSParticipants.api), strResponseValue) as DOSParticipants.api;
                                                    DOSParticipants.apiDataParticipant[] participants = (apis.data.Length == 0) ? null : apis.data[0];
                                                    if (participants == null) return ds;
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
                                                            dr[10] = cTimestamp;
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
                                                            dr2[12] = cTimestamp;
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
                                                    if (i < 2)
                                                    {
                                                        ds.Merge(data, true, MissingSchemaAction.AddWithKey);
                                                    }
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
            if (tabControl1.SelectedTab == tpAreas && tsbArea.Text.Trim().ToLower() == "all" && tsbArea.Text.Trim().ToLower() != "")
            {
                done = true;
                DataSet ds = InsertData("areas");
                //this.dgvAreas.DataSource = ds.Tables[0].DefaultView;
                tbData = ds.Tables[0];

                BindingSource bs = new BindingSource();
                bs.DataSource = tbData.DefaultView;
                bnAreas.BindingSource = bs;
                this.dgvAreas.DataSource = bs;
            }
            else if (tabControl1.SelectedTab == tpAreas && tsbArea.Text.Trim().ToLower() != "all" && tsbArea.Text.Trim().ToLower() != "")
            {
                done = true;
            }
            else if (tabControl1.SelectedTab == tpCompetitions )
            {
                done = true;
                DataSet ds = new DataSet(); 
                ds = InsertData("competitions"); 
                tbData = ds.Tables[0]; 
                BindingSource bs = new BindingSource();
                bs.DataSource = tbData.DefaultView;
                bnAreas.BindingSource = bs;
                this.dgvComp.DataSource = bs;
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

        public async Task<string> GetAccessData(string token, string type)
        {
            string strUrl = "";
            if (type.IndexOf("areas") > -1)
            {
                strUrl = $"/v2/" + ((type.IndexOf("/") > -1) ? "areas.xml?parent_area_id=" + type.Substring(type.IndexOf("/") + 1, type.Length - type.IndexOf("/") - 1) : type + ".xml");
                Console.WriteLine("GET areas " + strUrl);
                Files.WriteLog("GET areas " + strUrl);
            }
            else if (type.IndexOf("competitions") > -1)
            {
                strUrl = $"/v2/competitions.xml?token=" + token + "&sport_id=5&page=" + type.Substring(type.IndexOf("/") + 1, type.Length - type.IndexOf("/") - 1);

                Console.WriteLine("GET competitions " + strUrl);
                Files.WriteLog("GET competitions " + strUrl);
            }
            else if (type.IndexOf("seasons") > -1)
            {
                strUrl = $"/v2/seasons.xml?token=" + token + "&sport_id=5&page=" + type.Substring(type.IndexOf("/") + 1, type.Length - type.IndexOf("/") - 1);
                Console.WriteLine("GET seasons " + strUrl);
                Files.WriteLog("GET seasons " + strUrl);
            }
            else if (type.IndexOf("events") > -1)
            {
                strUrl = $"/v2/events.xml?token=" + token + "&sport_id=5&page=" + type.Substring(type.IndexOf("/") + 1, type.Length - type.IndexOf("/") - 1);
                Console.WriteLine("GET events " + strUrl);
                Files.WriteLog("GET events " + strUrl);
            }
            else if (type.IndexOf("participants") > -1)
            {
                strUrl = $"/v2/participants.xml?token=" + token + "&sport_id=5&page=" + type.Substring(type.IndexOf("/") + 1, type.Length - type.IndexOf("/") - 1);
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
