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
using RabbitMQ.Client;
using RabbitMQ.Client.Content;
using System.ComponentModel;
using RabbitMQ.Client.Events;
using System.Text;
using Newtonsoft.Json;
using System.IO;
using System.Threading;
using System.Runtime.InteropServices;
using Ecostar.MQConsumer.Core;

namespace DataOfScouts
{
    public partial class DataOfScouts : Form
    {
        string strToken = "";
        OAuthClient clientTest = new OAuthClient();
        [DllImport("user32.dll")]
        public static extern uint RegisterWindowMessage(string lpString);
        [DllImport("user32.dll")]
        public static extern IntPtr PostMessage(IntPtr hWnd, uint msg, int wParam, int lParam);
        [DllImport("kernel32.dll")]
        private static extern int GetLastError();
        private static readonly uint uiSkSvrNotify = RegisterWindowMessage(AppFlag.lpString);
        private static readonly IntPtr HWND_BROADCAST = new IntPtr(0xFFFF);
        public IReceiver Receiver { get; set; }
        public DataOfScouts()
        {
            InitializeComponent();
            try
            {
                //SendAlertMsg(AppFlag.LIVEGOALS);
                // SendAlertMsg(AppFlag.GOALDETAILS);
                var label = new ToolStripLabel();
                label.Text = "                        From ";
                bnAreas.Items.Insert(16, label);

                var picker = new DateTimePicker();
                picker.Name = "dtpStartTime";
                picker.Format = DateTimePickerFormat.Custom;
                //   picker.CustomFormat = "yyyy/MM/dd";
                picker.Value = DateTime.Now.AddDays(-1);
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
                picker.Value = DateTime.Now.AddDays(AppFlag.iQueryDays);
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
                dplStatus.AutoSize = false;
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

                this.btnSelectFile.Visible = false;
                this.btnXmlParser.Visible = false;
                this.txtXmlFileUrl.Visible = false;

                //  RunAMQPService();
                //  RunAMQPService2();
                bgwAMQPService.DoWork += new DoWorkEventHandler(bgwAMQPService_DoWork);
                bgwAMQPService.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgwAMQPService_Completed);
                lstStatus.SelectedValueChanged += new EventHandler(lstStatus_SelectedValueChanged);

                //Timer for amqp
                aTimer = new System.Timers.Timer(AppFlag.IntervalSync * 60 * 1000);
                aTimer.Elapsed += OnTimedEvent;
                aTimer.AutoReset = true;
                aTimer.Enabled = true;

                DateTime dueTimes = Convert.ToDateTime((DateTime.Now.Date.ToString("yyyy/MM/dd ") + AppFlag.GetTime));
                //Timer for Get events
                var timerState = new TimerState { Counter = 0 };
                tTimer = new System.Threading.Timer(
               callback: new TimerCallback(TimerTask),
               state: timerState,
               dueTime: (dueTimes < DateTime.Now ? dueTimes.AddDays(1).Subtract(DateTime.Now) : dueTimes.Subtract(DateTime.Now)),
               period: dueTimes.AddDays(1).Subtract(dueTimes));
                //  RunGetEventCompare("2726703");
                // InsertData("events.show3", true, "2737999", true);
                //InsertData("events", true, this.bnAreas.Items[17].Text + "2019-03-19 00:00:00", this.bnAreas.Items[19].Text + "2019-03-31 23:59:59");
                 InsertData("standings", true);
                this.Receiver = new Receiver();
            }
            catch (Exception exp)
            {
                Files.WriteError("DataOfScouts(),error: " + exp.Message);
            }

            //try
            //{
            //    bool done = false;
            //    string strName = "";
            //    string message = (File.ReadAllText("D:\\Users\\Administrator\\Desktop\\2504787_33_034604750.json"));
            //    using (FbConnection connection = new FbConnection(AppFlag.ScoutsDBConn))
            //    {
            //        connection.Open();
            //        DOSEventJson.EventJson api = JsonUtil.Deserialize(typeof(DOSEventJson.EventJson), message) as DOSEventJson.EventJson;
            //        if (api != null && api.type == "event" && (AppFlag.JsonType ? api.data.@event.sport_id == 5 : api.data.@event.sport_id != -1))
            //        { 
            //            int id = -2;
            //            strName = api.data.@event.id + "_" + (api.data.@event.status_id) + "_" + DateTime.Now.ToString("HHmmssfff");
            //            Files.WriteJson(strName, message); 
            //            if (api.data.@event.details.Count() > 0)
            //            {
            //                using (FbCommand cmd2 = new FbCommand())
            //                {
            //                    cmd2.CommandText = "PR_event_details";
            //                    cmd2.CommandType = CommandType.StoredProcedure;
            //                    cmd2.Connection = connection;
            //                    cmd2.Parameters.Add("@EVENTID", api.data.@event.id);
            //                    cmd2.Parameters.Add("@WC_8", api.data.@event.details.FirstOrDefault(c => c.id == 8).value);
            //                    cmd2.Parameters.Add("@PC_36", api.data.@event.details.FirstOrDefault(c => c.id == 36).value);
            //                    cmd2.Parameters.Add("@PL_16", api.data.@event.details.FirstOrDefault(c => c.id == 16).value);
            //                    cmd2.Parameters.Add("@EPL_50", api.data.@event.details.FirstOrDefault(c => c.id == 50).value);
            //                    cmd2.Parameters.Add("@NOP_17", api.data.@event.details.FirstOrDefault(c => c.id == 17).value);
            //                    cmd2.Parameters.Add("@EPTC_58", api.data.@event.details.FirstOrDefault(c => c.id == 58).value);
            //                    cmd2.Parameters.Add("@IT_151", api.data.@event.details.FirstOrDefault(c => c.id == 151).value);
            //                    cmd2.Parameters.Add("@ATT_141", api.data.@event.details.FirstOrDefault(c => c.id == 141).value);
            //                    cmd2.Parameters.Add("@FHSD_19", api.data.@event.details.FirstOrDefault(c => c.id == 19).value == null ? DateTime.MinValue : Convert.ToDateTime(api.data.@event.details.FirstOrDefault(c => c.id == 19).value).AddHours(8));
            //                    cmd2.Parameters.Add("@SHSD_20", api.data.@event.details.FirstOrDefault(c => c.id == 20).value == null ? DateTime.MinValue : Convert.ToDateTime(api.data.@event.details.FirstOrDefault(c => c.id == 20).value).AddHours(8));
            //                    cmd2.Parameters.Add("@FEHSD_44", api.data.@event.details.FirstOrDefault(c => c.id == 44).value);
            //                    cmd2.Parameters.Add("@SEHSD_45", api.data.@event.details.FirstOrDefault(c => c.id == 45).value);
            //                    cmd2.Parameters.Add("@PSSD_150", api.data.@event.details.FirstOrDefault(c => c.id == 150).value);
            //                    cmd2.Parameters.Add("@FHIT_201", api.data.@event.details.FirstOrDefault(c => c.id == 201).value);
            //                    cmd2.Parameters.Add("@SHIT_202", api.data.@event.details.FirstOrDefault(c => c.id == 202).value);
            //                    cmd2.Parameters.Add("@FEHIT_203", api.data.@event.details.FirstOrDefault(c => c.id == 203).value);
            //                    cmd2.Parameters.Add("@SEHIT_204", api.data.@event.details.FirstOrDefault(c => c.id == 204).value);
            //                    cmd2.Parameters.Add("@HL_205", api.data.@event.details.FirstOrDefault(c => c.id == 205).value);
            //                    cmd2.Parameters.Add("@TD_124", api.data.@event.details.FirstOrDefault(c => c.id == 124).value);
            //                    cmd2.Parameters.Add("@BM_160", api.data.@event.details.FirstOrDefault(c => c.id == 160).value);
            //                    cmd2.Parameters.Add("@HF_178", api.data.@event.details.FirstOrDefault(c => c.id == 178).value);
            //                    cmd2.Parameters.Add("@UT", api.ut);
            //                    cmd2.Parameters.Add("@CTIMESTAMP", DateTime.Now);
            //                    id = Convert.ToInt32(cmd2.ExecuteScalar());
            //                    Files.WriteLog((id > 0 ? " [Success] Insert event_details " : id == 0 ? " Old details " : " [Failure] Insert event_details ") + "[" + api.data.@event.id + "]," + strName + ".json.");
            //                }
            //            }

            //            if (api.data.@event.participants.Count() == 2 && api.data.@event.participants[0].stats.Length > 0 && api.data.@event.participants[0].results.Length > 0)
            //            {
            //                for (int i = 0; i < api.data.@event.participants.Length && api.data.@event.participants.Count() == 2; i++)
            //                {
            //                    using (FbCommand cmd2 = new FbCommand())
            //                    {
            //                        cmd2.CommandText = "PR_participant_results";
            //                        cmd2.CommandType = CommandType.StoredProcedure;
            //                        cmd2.Connection = connection;
            //                        // cmd2.Parameters.Add("@ID", 0);
            //                        cmd2.Parameters.Add("@EVENTID", api.data.@event.id);
            //                        cmd2.Parameters.Add("@PARTICIPANTID", api.data.@event.participants[i].id);
            //                        cmd2.Parameters.Add("@PROGRESS_412", api.data.@event.participants[i].results.FirstOrDefault(c => c.id == 412).value);
            //                        cmd2.Parameters.Add("@WINNER_411", api.data.@event.participants[i].results.FirstOrDefault(c => c.id == 411).value);
            //                        cmd2.Parameters.Add("@RESULT_2", api.data.@event.participants[i].results.FirstOrDefault(c => c.id == 2).value);
            //                        cmd2.Parameters.Add("@RT_3", api.data.@event.participants[i].results.FirstOrDefault(c => c.id == 3).value);
            //                        cmd2.Parameters.Add("@FH_4", api.data.@event.participants[i].results.FirstOrDefault(c => c.id == 4).value);
            //                        cmd2.Parameters.Add("@SH_5", api.data.@event.participants[i].results.FirstOrDefault(c => c.id == 5).value);
            //                        cmd2.Parameters.Add("@E1H_133", api.data.@event.participants[i].results.FirstOrDefault(c => c.id == 133).value);
            //                        cmd2.Parameters.Add("@E2H_134", api.data.@event.participants[i].results.FirstOrDefault(c => c.id == 134).value);
            //                        cmd2.Parameters.Add("@PENALTY_7", api.data.@event.participants[i].results.FirstOrDefault(c => c.id == 7).value);
            //                        cmd2.Parameters.Add("@OVERTIME_104", api.data.@event.participants[i].results.FirstOrDefault(c => c.id == 104).value);
            //                        cmd2.Parameters.Add("@CTIMESTAMP", DateTime.Now);
            //                        cmd2.Parameters.Add("@CACTION", api.data.@event.action);
            //                        cmd2.Parameters.Add("@TEAMTYPE", api.data.@event.participants[i].counter == 1 ? "H" : "G");
            //                        id = Convert.ToInt32(cmd2.ExecuteScalar());
            //                        Files.WriteLog((id > 0 ? " [Success] Insert participant_results" : " [Failure] Insert participant_results") + "[" + api.data.@event.id + "/" + api.data.@event.participants[0].id + "].");
            //                    }

            //                    using (FbCommand cmd2 = new FbCommand())
            //                    {
            //                        cmd2.CommandText = "PR_participant_stats";
            //                        cmd2.CommandType = CommandType.StoredProcedure;
            //                        cmd2.Connection = connection;
            //                        // cmd2.Parameters.Add("@ID", 0);
            //                        cmd2.Parameters.Add("@EVENTID", api.data.@event.id);
            //                        cmd2.Parameters.Add("@PARTICIPANTID", api.data.@event.participants[i].id);
            //                        cmd2.Parameters.Add("@SOT_20", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 20).value);
            //                        cmd2.Parameters.Add("@SOT_21", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 21).value);
            //                        cmd2.Parameters.Add("@ATTACKS_10", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 10).value);
            //                        cmd2.Parameters.Add("@DA_11", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 11).value);
            //                        cmd2.Parameters.Add("@CORNERS_13", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 13).value);
            //                        cmd2.Parameters.Add("@YELLOW_CARDS_8", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 8).value);
            //                        cmd2.Parameters.Add("@RED_CARDS_9", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 9).value);
            //                        cmd2.Parameters.Add("@TOTAL_SHOTS_19", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 19).value);
            //                        cmd2.Parameters.Add("@FOULS_22", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 22).value);
            //                        cmd2.Parameters.Add("@OFFSIDES_24", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 24).value);
            //                        cmd2.Parameters.Add("@PS_14", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 14).value);
            //                        cmd2.Parameters.Add("@PM_15", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 15).value);
            //                        cmd2.Parameters.Add("@PG_16", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 16).value);
            //                        cmd2.Parameters.Add("@FK_25", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 25).value);
            //                        cmd2.Parameters.Add("@DFK_26", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 26).value);
            //                        cmd2.Parameters.Add("@FKG_18", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 18).value);
            //                        cmd2.Parameters.Add("@SW_27", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 27).value);
            //                        cmd2.Parameters.Add("@SB_28", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 28).value);
            //                        cmd2.Parameters.Add("@GS_29", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 29).value);
            //                        cmd2.Parameters.Add("@GK_30", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 30).value);
            //                        cmd2.Parameters.Add("@TI_32", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 32).value);
            //                        cmd2.Parameters.Add("@SUBSTITUTIONS_31", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 31).value);
            //                        cmd2.Parameters.Add("@GOALS_40", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 40).value);
            //                        cmd2.Parameters.Add("@MP_34", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 34).value);
            //                        cmd2.Parameters.Add("@OWN_GOALS_17", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 17).value);
            //                        cmd2.Parameters.Add("@ADW_33", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 33).value);
            //                        cmd2.Parameters.Add("@FORM_716", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 716).value);
            //                        cmd2.Parameters.Add("@SKIN_718", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 718).value);
            //                        cmd2.Parameters.Add("@PS_639", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 639).value);
            //                        cmd2.Parameters.Add("@PU_697", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 697).value);
            //                        cmd2.Parameters.Add("@GOALS115_772", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 772).value);
            //                        cmd2.Parameters.Add("@GOALS1630_773", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 773).value);
            //                        cmd2.Parameters.Add("@GOALS3145_774", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 774).value);
            //                        cmd2.Parameters.Add("@GOALS4660_775", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 775).value);
            //                        cmd2.Parameters.Add("@GOALS6175_776", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 776).value);
            //                        cmd2.Parameters.Add("@GOALS7690_777", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 777).value);
            //                        cmd2.Parameters.Add("@MPG_778", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 778).value);
            //                        cmd2.Parameters.Add("@MPS_779", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 779).value);
            //                        cmd2.Parameters.Add("@CTIMESTAMP", DateTime.Now);
            //                        cmd2.Parameters.Add("@CACTION", api.data.@event.action);
            //                        cmd2.Parameters.Add("@TEAMTYPE", api.data.@event.participants[i].counter == 2 ? "G" : "H");
            //                        id = Convert.ToInt32(cmd2.ExecuteScalar());
            //                        Files.WriteLog((id > 0 ? " [Success] Insert participant_stats" : " [Failure] Insert participant_results") + "[" + api.data.@event.id + "/" + api.data.@event.participants[0].id + "].");
            //                        //  Thread.Sleep(20);
            //                    }
            //                }

            //                if (api.data.@event.action != "delete")
            //                {
            //                    using (FbCommand cmd2 = new FbCommand())
            //                    {
            //                        cmd2.CommandText = "PR_Stats_Result_GoalInfo";
            //                        cmd2.CommandType = CommandType.StoredProcedure;
            //                        cmd2.Connection = connection;
            //                        cmd2.Parameters.Add("@EMATCHID", api.data.@event.id);
            //                        cmd2.Parameters.Add("@H_YELLOW", api.data.@event.participants[0].stats.FirstOrDefault(c => c.id == 8).value);
            //                        cmd2.Parameters.Add("@H_RED", api.data.@event.participants[0].stats.FirstOrDefault(c => c.id == 9).value);
            //                        cmd2.Parameters.Add("@G_YELLOW", api.data.@event.participants[1].stats.FirstOrDefault(c => c.id == 8).value);
            //                        cmd2.Parameters.Add("@G_RED", api.data.@event.participants[1].stats.FirstOrDefault(c => c.id == 9).value);
            //                        cmd2.Parameters.Add("@H_GOAL", api.data.@event.participants[0].results.FirstOrDefault(c => c.id == 2).value);
            //                        cmd2.Parameters.Add("@G_GOAL", api.data.@event.participants[1].results.FirstOrDefault(c => c.id == 2).value);
            //                        cmd2.Parameters.Add("@STATUSID", api.data.@event.status_id);
            //                        cmd2.Parameters.Add("@LASTTIME", DateTime.Now);
            //                        id = Convert.ToInt32(cmd2.ExecuteScalar());
            //                        if (id > -1)
            //                        {
            //                            Files.WriteLog(" [Success] Update GoalInfo " + "[" + api.data.@event.id + "] " + api.data.@event.participants[0].id + "/" + api.data.@event.participants[1].id + " " + id + ".");
            //                            if (id > 0) done = true;
            //                        }
            //                    }
            //                }
            //                using (FbCommand cmd2 = new FbCommand())
            //                {
            //                    cmd2.CommandText = "PR_JSON_event";
            //                    cmd2.CommandType = CommandType.StoredProcedure;
            //                    cmd2.Connection = connection;
            //                    cmd2.Parameters.Add("@ID", api.data.@event.id);
            //                    cmd2.Parameters.Add("@NAME", api.data.@event.name);
            //                    cmd2.Parameters.Add("@HOME_ID", api.data.@event.participants.Count() > 0 ? api.data.@event.participants[0].id : -1);
            //                    cmd2.Parameters.Add("@GUEST_ID", api.data.@event.participants.Count() > 0 ? api.data.@event.participants[1].id : -1);
            //                    // cmd2.Parameters.Add("@SOURCE", api.data.@evenT.S);
            //                    // cmd2.Parameters.Add("@SOURCE_DC", api.data.@event.S);
            //                    //cmd2.Parameters.Add("@SOURCE_SUPER", api.data.@event.s);
            //                    cmd2.Parameters.Add("@RELATION_STATUS", api.data.@event.relation_status);
            //                    cmd2.Parameters.Add("@START_DATE", Convert.ToDateTime(api.data.@event.start_date).AddHours(8));
            //                    cmd2.Parameters.Add("@FT_ONLY", api.data.@event.ft_only == "yes" ? true : false);
            //                    cmd2.Parameters.Add("@COVERAGE_TYPE", api.data.@event.coverage_type);
            //                    //cmd2.Parameters.Add("@CHANNEL_ID", api.data.@event.CH);
            //                    //cmd2.Parameters.Add("@CHANNEL_NAME", api.data.@event.C);
            //                    cmd2.Parameters.Add("@SCOUTSFEED", api.data.@event.scoutsfeed == "yes" ? true : false);
            //                    cmd2.Parameters.Add("@STATUS_ID", api.data.@event.status_id);
            //                    //cmd2.Parameters.Add("@STATUS_NAME", api.data.@event.STA);
            //                    cmd2.Parameters.Add("@STATUS_TYPE", api.data.@event.status_type);
            //                    cmd2.Parameters.Add("@CDAY", api.data.@event.day);
            //                    cmd2.Parameters.Add("@CLOCK_TIME", api.data.@event.clock_time);
            //                    cmd2.Parameters.Add("@CLOCK_STATUS", api.data.@event.clock_status);
            //                    // cmd2.Parameters.Add("@WINNER_ID", api.data.@event.W);
            //                    //cmd2.Parameters.Add("@PROGRESS_ID", api.data.@event.PR);
            //                    cmd2.Parameters.Add("@BET_STATUS", api.data.@event.bet_status);
            //                    cmd2.Parameters.Add("@NEUTRAL_VENUE", api.data.@event.neutral_venue == "yes" ? true : false);
            //                    cmd2.Parameters.Add("@ITEM_STATUS", api.data.@event.item_status);
            //                    cmd2.Parameters.Add("@UT", api.ut);
            //                    // cmd2.Parameters.Add("@OLD_EVENT_ID", api.data.@event.OL);
            //                    // cmd2.Parameters.Add("@SLUG", api.data.@event.S);
            //                    // cmd2.Parameters.Add("@VERIFIED_RESULT", api.data.@event.VE);
            //                    // cmd2.Parameters.Add("@IS_PROTOCOL_VERIFIED", api.data.@event.IS);
            //                    //  cmd2.Parameters.Add("@PROTOCOL_VERIFIED_BY", api.data.@event.PRO);
            //                    //cmd2.Parameters.Add("@PROTOCOL_VERIFIED_AT", api.data.@event.PRO);
            //                    cmd2.Parameters.Add("@ROUND_ID", api.data.@event.round_id);
            //                    cmd2.Parameters.Add("@ROUND_NAME", api.data.@event.round_name);
            //                    //cmd2.Parameters.Add("@CLIENT_EVENT_ID", api.data.@event.C);
            //                    // cmd2.Parameters.Add("@BOOKED", null);
            //                    // cmd2.Parameters.Add("@BOOKED_BY", api.data.@event.);
            //                    // cmd2.Parameters.Add("@INVERTED_PARTICIPANTS", api.data.@event.iv);
            //                    cmd2.Parameters.Add("@VENUE_ID", api.data.@event.tour_id == null ? "-1" : api.data.@event.tour_id);
            //                    //  cmd2.Parameters.Add("@GROUP_ID", api.data.@event.gr);
            //                    cmd2.Parameters.Add("@STAGE_ID", api.data.@event.stage_id);
            //                    cmd2.Parameters.Add("@SEASON_ID", api.data.@event.season_id);
            //                    cmd2.Parameters.Add("@COMPETITION_ID", api.data.@event.competition_id);
            //                    cmd2.Parameters.Add("@AREA_ID", api.data.@event.area_id);
            //                    cmd2.Parameters.Add("@CTIMESTAMP", DateTime.Now);
            //                    cmd2.Parameters.Add("@CACTION", api.data.@event.action);
            //                    id = Convert.ToInt32(cmd2.ExecuteScalar());
            //                    // Files.WriteLog((id == 0 ? " [Success] Insert event " : id == 1 ? " Update event " : "") + "[" + api.data.@event.id + "]," + strName + ".json.");
            //                    Files.WriteLog((id > 0 ? " [Success] Insert event " : id == 0 ? " Old data " : " [Failure] Insert event ") + "[" + api.data.@event.id + "]," + strName + ".json." + api.data.@event.status_id);
            //                    //1312019
            //                    if (id == 0) done = false;
            //                }
            //            }

            //            if (done)
            //            {
            //                if (api.data.@event.status_id == 6 || api.data.@event.status_id == 9 || api.data.@event.status_id == 11)
            //                {
            //                    if (api.data.@event.id > 0)
            //                    {
            //                        /// await AyncHandleData("events.show3", true, api.data.@event.id.ToString());
            //                        InsertData("events.show3", true, api.data.@event.id.ToString(), true);
            //                        Files.WriteLog(" Housekeep [" + api.data.@event.id.ToString() + "].." + api.data.@event.status_id);
            //                    }
            //                }
            //                else
            //                {
            //                    InsertData("events.show3", true, api.data.@event.id.ToString(), true);
            //                    Files.WriteLog(" Get event [" + api.data.@event.id.ToString() + "].." + api.data.@event.status_id);
            //                }

            //                SendAlertMsg(AppFlag.LIVEGOALS);
            //                done = false;
            //            } 
            //        }
            //    }
            //}
            //catch (Exception exp)
            //{
            //    Files.WriteError("DataOfScouts(),error: " + exp.Message);
            //}



            //try
            //{
            //    int sID = -1;
            //    string strName = "";
            //    string message = (File.ReadAllText("D:\\Users\\Administrator\\Desktop\\Incid_2520214_33_402-5-70791802-165855430.json"));
            //    DOSIncidentJson.IncidentJson incidentJson = JsonUtil.Deserialize(typeof(DOSIncidentJson.IncidentJson), message) as DOSIncidentJson.IncidentJson;
            //    iCount2++;
            //    if (incidentJson != null && incidentJson.data.@event.sport_id == 5 && incidentJson.data.incident.important_for_trader == "yes")
            //    {
            //        //20190121 RUN DELETE ACTION BY TIGGER
            //        ////if (incidentJson.data.incident.action != "delete")
            //        ////{
            //        strName = "Incid_" + incidentJson.data.@event.id + "_" + incidentJson.data.incident.event_status_id + "_" + incidentJson.data.incident.incident_id + "-" + incidentJson.data.incident.id + "-" + DateTime.Now.ToString("HHmmssfff");
            //        Files.WriteJson(strName, message);
            //        DateTime time = DateTime.Now;
            //        using (FbConnection connection = new FbConnection(AppFlag.ScoutsDBConn))
            //        {
            //            connection.Open();
            //            using (FbCommand cmd2 = new FbCommand())
            //            {
            //                cmd2.CommandText = "PR_INCIDENTS";
            //                cmd2.CommandType = CommandType.StoredProcedure;
            //                cmd2.Connection = connection;
            //                cmd2.Parameters.Add("@ID", incidentJson.data.incident.id);
            //                cmd2.Parameters.Add("@EVENTID", incidentJson.data.@event.id);
            //                cmd2.Parameters.Add("@CACTION", incidentJson.data.incident.action);
            //                cmd2.Parameters.Add("@INCIDENT_ID", incidentJson.data.incident.incident_id);
            //                cmd2.Parameters.Add("@INCIDENT_NAME", incidentJson.data.incident.incident_name);
            //                cmd2.Parameters.Add("@PARTICIPANT_ID", incidentJson.data.incident.participant_id);
            //                cmd2.Parameters.Add("@PARTICIPANT_NAME", incidentJson.data.incident.participant_name);
            //                cmd2.Parameters.Add("@SUBPARTICIPANT_ID", incidentJson.data.incident.subparticipant_id);
            //                cmd2.Parameters.Add("@SUBPARTICIPANT_NAME", incidentJson.data.incident.subparticipant_name);
            //                cmd2.Parameters.Add("@IMPORTANT_FOR_TRADER", true);// incidentJson.data.incident.important_for_trader == "yes" ? true : false);
            //                cmd2.Parameters.Add("@EVENT_TIME", incidentJson.data.incident.event_time);
            //                cmd2.Parameters.Add("@EVENT_STATUS_ID", incidentJson.data.incident.event_status_id);
            //                cmd2.Parameters.Add("@EVENT_STATUS_NAME", incidentJson.data.incident.event_status_name);
            //                cmd2.Parameters.Add("@UT", incidentJson.ut);
            //                cmd2.Parameters.Add("@CTIMESTAMP", DateTime.Now);
            //                cmd2.Parameters.Add("@TEAMTYPE",
            //                (incidentJson.data.incident.participant_id == null ? "" : incidentJson.data.incident.participant_id.ToString() == incidentJson.data.@event.participants[0].id.ToString() ? "H" : incidentJson.data.incident.participant_id.ToString() == incidentJson.data.@event.participants[1].id.ToString() ? "G" : "H"));
            //                sID = Convert.ToInt32(cmd2.ExecuteScalar());
            //                Files.WriteLog((sID > -1 ? " [Success] Insert INCIDENTS " : " [Failure] Insert INCIDENTS ") + "[" + incidentJson.data.@event.id + "]," + strName + ".json");

            //                if (sID > 0) { SendAlertMsg(AppFlag.GOALDETAILS); }

            //                //if (!backgroundWorker.IsBusy && sID > 0)
            //                //{
            //                //      backgroundWorker.RunWorkerAsync(incidentJson.data.@event.name + " " + strName + ".json");
            //                //}
            //                if (sID > 0)
            //                {
            //                    this.lstStatus.Invoke(new Action(() =>
            //                    {
            //                        {
            //                            this.lstStatus.Items.Insert(0, DateTime.Now.ToString("HH:mm:ss.fff   ") + incidentJson.data.@event.name + " " + strName + ".json");
            //                        }
            //                    }));
            //                }
            //            }

            //            if (incidentJson.data.incident.incident_id == 429 || incidentJson.data.@event.status_id == 9 || incidentJson.data.@event.status_id == 11)
            //            {
            //                if (incidentJson.data.@event.id > 0)
            //                {
            //                    //  await AyncHandleData("events.show3", true, incidentJson.data.@event.id.ToString());
            //                    InsertData("events.show3", true, incidentJson.data.@event.id.ToString());
            //                    Files.WriteLog(" Housekeep incident [" + incidentJson.data.@event.id.ToString() + "].");
            //                    if (incidentJson.data.incident.incident_id == 429)
            //                    {
            //                        InsertData("events.participants", true, incidentJson.data.@event.id.ToString());
            //                        Files.WriteLog(" Housekeep event participant [" + incidentJson.data.@event.id.ToString() + "]." + incidentJson.data.incident.incident_id);

            //                    }
            //                }
            //            }

            //            //402 Dangerous attack 
            //          if (incidentJson.data.incident.incident_id == 413 || incidentJson.data.incident.incident_id == 418 || incidentJson.data.incident.incident_id == 419 ||
            //                      (incidentJson.data.incident.subparticipant_name != null 
            //                      && (incidentJson.data.incident.incident_id == 408
            //                      || incidentJson.data.incident.incident_id == 404 || incidentJson.data.incident.incident_id == 402
            //                      || incidentJson.data.incident.incident_id == 414 || incidentJson.data.incident.incident_id == 415)))
            //                    {
            //                string sName = "";
            //                using (FbCommand cmd2 = new FbCommand("SELECT  r.EMATCHID , r.HKJCDAYCODE, r.HKJCMATCHNO FROM EMATCHES r where r.EMATCHID='" + incidentJson.data.@event.id + "'", connection))
            //                {
            //                    FbDataReader reader = cmd2.ExecuteReader();
            //                    while (reader.Read())
            //                    {
            //                        var data = (IDataRecord)reader;
            //                        sName = data[1] + "-" + data[2] + "-" + data[0];
            //                    }
            //                    reader.Close();
            //                }
            //                string sData = time + "    " + incidentJson.data.incident.event_time + "  " + (incidentJson.data.incident.subparticipant_id==null?"": incidentJson.data.incident.subparticipant_id.ToString()) + "  " + (incidentJson.data.incident.subparticipant_name==null?"":incidentJson.data.incident.subparticipant_name) + "  " + incidentJson.data.incident.incident_name;

            //                Files.WriteLog(sName, false, sData);
            //            }

            //            //if (incidentJson.data.incident.incident_id == 429 || incidentJson.data.@event.status_id == 9 || incidentJson.data.@event.status_id == 11)
            //            //{
            //            //    if (incidentJson.data.@event.id > 0)
            //            //    {
            //            //        //  await AyncHandleData("events.show3", true, incidentJson.data.@event.id.ToString());
            //            //        InsertData("events.show3", true, incidentJson.data.@event.id.ToString());
            //            //        Files.WriteLog(" Housekeep incident [" + incidentJson.data.@event.id.ToString() + "].");
            //            //        if (incidentJson.data.incident.incident_id == 429)
            //            //        {
            //            //            InsertData("events.participants", true, incidentJson.data.@event.id.ToString());
            //            //            Files.WriteLog(" Housekeep event participant [" + incidentJson.data.@event.id.ToString() + "]." + incidentJson.data.incident.incident_id);

            //            //        }
            //            //    }
            //            //}
            //        }
            //    }
            //}
            //catch (Exception exp)
            //{
            //    Files.WriteError("DataOfScouts(),error: " + exp.Message);
            //} 
        }
        private void btnSelectFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory + "XmlFolder" + DateTime.Now.ToString("yyyyMMdd") + "\\";
            openFileDialog.Filter = "Json|*.json";
            openFileDialog.RestoreDirectory = true;
            openFileDialog.FilterIndex = 1;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                this.txtXmlFileUrl.Text = openFileDialog.FileName;
            }
        }

        private void btnXmlParser_Click(object sender, EventArgs e)
        {
            this.DoEvent(this.txtXmlFileUrl.Text.Trim());
            this.lstStatus.Items.Insert(0, DateTime.Now.ToString("HH:mm:ss.fff   ") + "Do " + this.txtXmlFileUrl.Text.Trim());
            Files.WriteLog("Do " + this.txtXmlFileUrl.Text.Trim());
        }

        // this.Text = Directory.GetCurrentDirectory() + @"\DataOfScouts";
        void DoEvent(string file)
        {
            try
            {
                string strName = "";
                string message = (File.ReadAllText(file));
                using (FbConnection connection = new FbConnection(AppFlag.ScoutsDBConn))
                {
                    connection.Open();
                    DOSEventJson.EventJson api = JsonUtil.Deserialize(typeof(DOSEventJson.EventJson), message) as DOSEventJson.EventJson;
                    if (api != null && api.type == "event" && (AppFlag.JsonType ? api.data.@event.sport_id == 5 : api.data.@event.sport_id != -1))
                    {
                        int id = -2;
                        strName = api.data.@event.id + "_" + (api.data.@event.status_id) + "_" + DateTime.Now.ToString("HHmmssfff");
                        Files.WriteJson(strName, message);
                        if (api.data.@event.details.Count() > 0)
                        {
                            using (FbCommand cmd2 = new FbCommand())
                            {
                                cmd2.CommandText = "PR_event_details";
                                cmd2.CommandType = CommandType.StoredProcedure;
                                cmd2.Connection = connection;
                                cmd2.Parameters.Add("@EVENTID", api.data.@event.id);
                                cmd2.Parameters.Add("@WC_8", api.data.@event.details.FirstOrDefault(c => c.id == 8).value);
                                cmd2.Parameters.Add("@PC_36", api.data.@event.details.FirstOrDefault(c => c.id == 36).value);
                                cmd2.Parameters.Add("@PL_16", api.data.@event.details.FirstOrDefault(c => c.id == 16).value);
                                cmd2.Parameters.Add("@EPL_50", api.data.@event.details.FirstOrDefault(c => c.id == 50).value);
                                cmd2.Parameters.Add("@NOP_17", api.data.@event.details.FirstOrDefault(c => c.id == 17).value);
                                cmd2.Parameters.Add("@EPTC_58", api.data.@event.details.FirstOrDefault(c => c.id == 58).value);
                                cmd2.Parameters.Add("@IT_151", api.data.@event.details.FirstOrDefault(c => c.id == 151).value);
                                cmd2.Parameters.Add("@ATT_141", api.data.@event.details.FirstOrDefault(c => c.id == 141).value);
                                cmd2.Parameters.Add("@FHSD_19", api.data.@event.details.FirstOrDefault(c => c.id == 19).value == null ? DateTime.MinValue : Convert.ToDateTime(api.data.@event.details.FirstOrDefault(c => c.id == 19).value).AddHours(8));
                                cmd2.Parameters.Add("@SHSD_20", api.data.@event.details.FirstOrDefault(c => c.id == 20).value == null ? DateTime.MinValue : Convert.ToDateTime(api.data.@event.details.FirstOrDefault(c => c.id == 20).value).AddHours(8));
                                cmd2.Parameters.Add("@FEHSD_44", api.data.@event.details.FirstOrDefault(c => c.id == 44).value);
                                cmd2.Parameters.Add("@SEHSD_45", api.data.@event.details.FirstOrDefault(c => c.id == 45).value);
                                cmd2.Parameters.Add("@PSSD_150", api.data.@event.details.FirstOrDefault(c => c.id == 150).value);
                                cmd2.Parameters.Add("@FHIT_201", api.data.@event.details.FirstOrDefault(c => c.id == 201).value);
                                cmd2.Parameters.Add("@SHIT_202", api.data.@event.details.FirstOrDefault(c => c.id == 202).value);
                                cmd2.Parameters.Add("@FEHIT_203", api.data.@event.details.FirstOrDefault(c => c.id == 203).value);
                                cmd2.Parameters.Add("@SEHIT_204", api.data.@event.details.FirstOrDefault(c => c.id == 204).value);
                                cmd2.Parameters.Add("@HL_205", api.data.@event.details.FirstOrDefault(c => c.id == 205).value);
                                cmd2.Parameters.Add("@TD_124", api.data.@event.details.FirstOrDefault(c => c.id == 124).value);
                                cmd2.Parameters.Add("@BM_160", api.data.@event.details.FirstOrDefault(c => c.id == 160).value);
                                cmd2.Parameters.Add("@HF_178", api.data.@event.details.FirstOrDefault(c => c.id == 178).value);
                                cmd2.Parameters.Add("@UT", api.ut);
                                cmd2.Parameters.Add("@CTIMESTAMP", DateTime.Now);
                                id = Convert.ToInt32(cmd2.ExecuteScalar());
                                Files.WriteLog((id > 0 ? " [Success] Insert event_details " : id == -1 ? " Old data " : " [Failure] Insert event_details ") + "[" + api.data.@event.id + "]," + strName + ".json.");
                            }
                        }
                        if (api.data.@event.participants.Count() == 2 && api.data.@event.participants[0].stats.Length > 0 && api.data.@event.participants[0].results.Length > 0)
                        {
                            if (api.data.@event.action != "delete")
                            {

                                using (FbCommand cmd2 = new FbCommand())
                                {
                                    cmd2.CommandText = "PR_Stats_Result_GoalInfo";
                                    cmd2.CommandType = CommandType.StoredProcedure;
                                    cmd2.Connection = connection;
                                    cmd2.Parameters.Add("@EMATCHID", api.data.@event.id);
                                    cmd2.Parameters.Add("@H_YELLOW", api.data.@event.participants[0].stats.FirstOrDefault(c => c.id == 8).value);
                                    cmd2.Parameters.Add("@H_RED", api.data.@event.participants[0].stats.FirstOrDefault(c => c.id == 9).value);
                                    cmd2.Parameters.Add("@G_YELLOW", api.data.@event.participants[1].stats.FirstOrDefault(c => c.id == 8).value);
                                    cmd2.Parameters.Add("@G_RED", api.data.@event.participants[1].stats.FirstOrDefault(c => c.id == 9).value);
                                    cmd2.Parameters.Add("@H_GOAL", api.data.@event.participants[0].results.FirstOrDefault(c => c.id == 2).value);
                                    cmd2.Parameters.Add("@G_GOAL", api.data.@event.participants[1].results.FirstOrDefault(c => c.id == 2).value);
                                    cmd2.Parameters.Add("@STATUSID", api.data.@event.status_id);
                                    cmd2.Parameters.Add("@LASTTIME", DateTime.Now);
                                    id = Convert.ToInt32(cmd2.ExecuteScalar());
                                    if (id > -1)
                                    {
                                        Files.WriteLog(" [Success] Update GoalInfo " + "[" + api.data.@event.id + "/" + api.data.@event.participants[0].id + "]" + id + ".");
                                        //   if (id > 0) done = true;
                                    }
                                }
                            }

                            using (FbCommand cmd2 = new FbCommand())
                            {
                                cmd2.CommandText = "PR_JSON_event";
                                cmd2.CommandType = CommandType.StoredProcedure;
                                cmd2.Connection = connection;
                                cmd2.Parameters.Add("@ID", api.data.@event.id);
                                cmd2.Parameters.Add("@NAME", api.data.@event.name);
                                cmd2.Parameters.Add("@HOME_ID", api.data.@event.participants.Count() > 0 ? api.data.@event.participants[0].id : -1);
                                cmd2.Parameters.Add("@GUEST_ID", api.data.@event.participants.Count() > 0 ? api.data.@event.participants[1].id : -1);
                                // cmd2.Parameters.Add("@SOURCE", api.data.@evenT.S);
                                // cmd2.Parameters.Add("@SOURCE_DC", api.data.@event.S);
                                //cmd2.Parameters.Add("@SOURCE_SUPER", api.data.@event.s);
                                cmd2.Parameters.Add("@RELATION_STATUS", api.data.@event.relation_status);
                                cmd2.Parameters.Add("@START_DATE", Convert.ToDateTime(api.data.@event.start_date).AddHours(8));
                                cmd2.Parameters.Add("@FT_ONLY", api.data.@event.ft_only == "yes" ? true : false);
                                cmd2.Parameters.Add("@COVERAGE_TYPE", api.data.@event.coverage_type);
                                //cmd2.Parameters.Add("@CHANNEL_ID", api.data.@event.CH);
                                //cmd2.Parameters.Add("@CHANNEL_NAME", api.data.@event.C);
                                cmd2.Parameters.Add("@SCOUTSFEED", api.data.@event.scoutsfeed == "yes" ? true : false);
                                cmd2.Parameters.Add("@STATUS_ID", api.data.@event.status_id);
                                //cmd2.Parameters.Add("@STATUS_NAME", api.data.@event.STA);
                                cmd2.Parameters.Add("@STATUS_TYPE", api.data.@event.status_type);
                                cmd2.Parameters.Add("@CDAY", api.data.@event.day);
                                cmd2.Parameters.Add("@CLOCK_TIME", api.data.@event.clock_time);
                                cmd2.Parameters.Add("@CLOCK_STATUS", api.data.@event.clock_status);
                                // cmd2.Parameters.Add("@WINNER_ID", api.data.@event.W);
                                //cmd2.Parameters.Add("@PROGRESS_ID", api.data.@event.PR);
                                cmd2.Parameters.Add("@BET_STATUS", api.data.@event.bet_status);
                                cmd2.Parameters.Add("@NEUTRAL_VENUE", api.data.@event.neutral_venue == "yes" ? true : false);
                                cmd2.Parameters.Add("@ITEM_STATUS", api.data.@event.item_status);
                                // cmd2.Parameters.Add("@UT", api.data.@event.U);
                                // cmd2.Parameters.Add("@OLD_EVENT_ID", api.data.@event.OL);
                                // cmd2.Parameters.Add("@SLUG", api.data.@event.S);
                                // cmd2.Parameters.Add("@VERIFIED_RESULT", api.data.@event.VE);
                                // cmd2.Parameters.Add("@IS_PROTOCOL_VERIFIED", api.data.@event.IS);
                                //  cmd2.Parameters.Add("@PROTOCOL_VERIFIED_BY", api.data.@event.PRO);
                                //cmd2.Parameters.Add("@PROTOCOL_VERIFIED_AT", api.data.@event.PRO);
                                cmd2.Parameters.Add("@ROUND_ID", api.data.@event.round_id);
                                cmd2.Parameters.Add("@ROUND_NAME", api.data.@event.round_name);
                                //cmd2.Parameters.Add("@CLIENT_EVENT_ID", api.data.@event.C);
                                // cmd2.Parameters.Add("@BOOKED", null);
                                // cmd2.Parameters.Add("@BOOKED_BY", api.data.@event.);
                                // cmd2.Parameters.Add("@INVERTED_PARTICIPANTS", api.data.@event.iv);
                                cmd2.Parameters.Add("@VENUE_ID", api.data.@event.tour_id == null ? "-1" : api.data.@event.tour_id);
                                //  cmd2.Parameters.Add("@GROUP_ID", api.data.@event.gr);
                                cmd2.Parameters.Add("@STAGE_ID", api.data.@event.stage_id);
                                cmd2.Parameters.Add("@SEASON_ID", api.data.@event.season_id);
                                cmd2.Parameters.Add("@COMPETITION_ID", api.data.@event.competition_id);
                                cmd2.Parameters.Add("@AREA_ID", api.data.@event.area_id);
                                cmd2.Parameters.Add("@CTIMESTAMP", DateTime.Now);
                                cmd2.Parameters.Add("@CACTION", api.data.@event.action);
                                id = Convert.ToInt32(cmd2.ExecuteScalar());
                                Files.WriteLog((id == 0 ? " [Success] Insert event " : id == 1 ? " Update event " : "") + "[" + api.data.@event.id + "]," + strName + ".json.");
                                // Files.WriteLog((id == 0 ? " [Success] Insert event [" + api.data.@event.id + "]," + strName + ".json":"");
                            }
                            for (int i = 0; i < api.data.@event.participants.Length && api.data.@event.participants.Count() == 2; i++)
                            {
                                using (FbCommand cmd2 = new FbCommand())
                                {
                                    cmd2.CommandText = "PR_participant_results";
                                    cmd2.CommandType = CommandType.StoredProcedure;
                                    cmd2.Connection = connection;
                                    // cmd2.Parameters.Add("@ID", 0);
                                    cmd2.Parameters.Add("@EVENTID", api.data.@event.id);
                                    cmd2.Parameters.Add("@PARTICIPANTID", api.data.@event.participants[i].id);
                                    cmd2.Parameters.Add("@PROGRESS_412", api.data.@event.participants[i].results.FirstOrDefault(c => c.id == 412).value);
                                    cmd2.Parameters.Add("@WINNER_411", api.data.@event.participants[i].results.FirstOrDefault(c => c.id == 411).value);
                                    cmd2.Parameters.Add("@RESULT_2", api.data.@event.participants[i].results.FirstOrDefault(c => c.id == 2).value);
                                    cmd2.Parameters.Add("@RT_3", api.data.@event.participants[i].results.FirstOrDefault(c => c.id == 3).value);
                                    cmd2.Parameters.Add("@FH_4", api.data.@event.participants[i].results.FirstOrDefault(c => c.id == 4).value);
                                    cmd2.Parameters.Add("@SH_5", api.data.@event.participants[i].results.FirstOrDefault(c => c.id == 5).value);
                                    cmd2.Parameters.Add("@E1H_133", api.data.@event.participants[i].results.FirstOrDefault(c => c.id == 133).value);
                                    cmd2.Parameters.Add("@E2H_134", api.data.@event.participants[i].results.FirstOrDefault(c => c.id == 134).value);
                                    cmd2.Parameters.Add("@PENALTY_7", api.data.@event.participants[i].results.FirstOrDefault(c => c.id == 7).value);
                                    cmd2.Parameters.Add("@OVERTIME_104", api.data.@event.participants[i].results.FirstOrDefault(c => c.id == 104).value);
                                    cmd2.Parameters.Add("@CTIMESTAMP", DateTime.Now);
                                    cmd2.Parameters.Add("@CACTION", api.data.@event.action);
                                    cmd2.Parameters.Add("@TEAMTYPE", api.data.@event.participants[i].counter == 1 ? "H" : "G");
                                    id = Convert.ToInt32(cmd2.ExecuteScalar());
                                    Files.WriteLog((id > 0 ? " [Success] Insert participant_results" : " [Failure] Insert participant_results") + "[" + api.data.@event.id + "/" + api.data.@event.participants[0].id + "].");
                                }

                                using (FbCommand cmd2 = new FbCommand())
                                {
                                    cmd2.CommandText = "PR_participant_stats";
                                    cmd2.CommandType = CommandType.StoredProcedure;
                                    cmd2.Connection = connection;
                                    // cmd2.Parameters.Add("@ID", 0);
                                    cmd2.Parameters.Add("@EVENTID", api.data.@event.id);
                                    cmd2.Parameters.Add("@PARTICIPANTID", api.data.@event.participants[i].id);
                                    cmd2.Parameters.Add("@SOT_20", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 20).value);
                                    cmd2.Parameters.Add("@SOT_21", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 21).value);
                                    cmd2.Parameters.Add("@ATTACKS_10", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 10).value);
                                    cmd2.Parameters.Add("@DA_11", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 11).value);
                                    cmd2.Parameters.Add("@CORNERS_13", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 13).value);
                                    cmd2.Parameters.Add("@YELLOW_CARDS_8", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 8).value);
                                    cmd2.Parameters.Add("@RED_CARDS_9", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 9).value);
                                    cmd2.Parameters.Add("@TOTAL_SHOTS_19", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 19).value);
                                    cmd2.Parameters.Add("@FOULS_22", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 22).value);
                                    cmd2.Parameters.Add("@OFFSIDES_24", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 24).value);
                                    cmd2.Parameters.Add("@PS_14", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 14).value);
                                    cmd2.Parameters.Add("@PM_15", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 15).value);
                                    cmd2.Parameters.Add("@PG_16", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 16).value);
                                    cmd2.Parameters.Add("@FK_25", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 25).value);
                                    cmd2.Parameters.Add("@DFK_26", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 26).value);
                                    cmd2.Parameters.Add("@FKG_18", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 18).value);
                                    cmd2.Parameters.Add("@SW_27", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 27).value);
                                    cmd2.Parameters.Add("@SB_28", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 28).value);
                                    cmd2.Parameters.Add("@GS_29", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 29).value);
                                    cmd2.Parameters.Add("@GK_30", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 30).value);
                                    cmd2.Parameters.Add("@TI_32", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 32).value);
                                    cmd2.Parameters.Add("@SUBSTITUTIONS_31", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 31).value);
                                    cmd2.Parameters.Add("@GOALS_40", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 40).value);
                                    cmd2.Parameters.Add("@MP_34", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 34).value);
                                    cmd2.Parameters.Add("@OWN_GOALS_17", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 17).value);
                                    cmd2.Parameters.Add("@ADW_33", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 33).value);
                                    cmd2.Parameters.Add("@FORM_716", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 716).value);
                                    cmd2.Parameters.Add("@SKIN_718", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 718).value);
                                    cmd2.Parameters.Add("@PS_639", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 639).value);
                                    cmd2.Parameters.Add("@PU_697", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 697).value);
                                    cmd2.Parameters.Add("@GOALS115_772", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 772).value);
                                    cmd2.Parameters.Add("@GOALS1630_773", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 773).value);
                                    cmd2.Parameters.Add("@GOALS3145_774", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 774).value);
                                    cmd2.Parameters.Add("@GOALS4660_775", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 775).value);
                                    cmd2.Parameters.Add("@GOALS6175_776", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 776).value);
                                    cmd2.Parameters.Add("@GOALS7690_777", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 777).value);
                                    cmd2.Parameters.Add("@MPG_778", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 778).value);
                                    cmd2.Parameters.Add("@MPS_779", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 779).value);
                                    cmd2.Parameters.Add("@CTIMESTAMP", DateTime.Now);
                                    cmd2.Parameters.Add("@CACTION", api.data.@event.action);
                                    cmd2.Parameters.Add("@TEAMTYPE", api.data.@event.participants[i].counter == 2 ? "G" : "H");
                                    id = Convert.ToInt32(cmd2.ExecuteScalar());
                                    Files.WriteLog((id > 0 ? " [Success] Insert participant_stats" : " [Failure] Insert participant_results") + "[" + api.data.@event.id + "/" + api.data.@event.participants[0].id + "].");
                                    //  Thread.Sleep(20);
                                }
                            }
                        }

                        if (api.data.@event.status_id == 9 || api.data.@event.status_id == 11)
                        {
                            if (api.data.@event.id > 0)
                            {
                                /// await AyncHandleData("events.show3", true, api.data.@event.id.ToString());
                                InsertData("events.show3", true, api.data.@event.id.ToString(), false);
                                Files.WriteLog(" Housekeep [" + api.data.@event.id.ToString() + "].." + api.data.@event.status_id);
                            }
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                Files.WriteError("DataOfScouts(),error: " + exp.Message);
            }

        }
        void InitialListener()
        {
            try
            {
                string serverAddress = "amqp://queue.statscore.com:5672/";
                Uri uri = new Uri(serverAddress);

                MQConnectionFactory connectionFactoryParam = new MQConnectionFactory();
                connectionFactoryParam.HostName = "queue.statscore.com";
                connectionFactoryParam.UserName = "telecom-digital-data-limited";
                connectionFactoryParam.Password = "Eb76sQDkn9oZEki5L9QreKpaPmD3GbuuW2I";
                connectionFactoryParam.VirtualHost = "statscore";
                connectionFactoryParam.Port = 5672;

                MQReceiverParam mq = new MQReceiverParam();
                mq._queueName = "telecom-digital-data-limited";
                mq._isAutoAck = false;
                mq._mqUrls = new List<System.Uri>() { uri };
                mq._processFunction = (message) =>
                {
                    //string receiveMsg = Encoding.UTF8.GetString(buffer);
                    //if (AppFlag.TestMode) this.lstStatus.Invoke(new Action(() => { { this.lstStatus.Items.Insert(0, "ABC"); } }));

                    using (FbConnection connection = new FbConnection(AppFlag.ScoutsDBConn))
                    {
                        connection.Open();
                        string strName = "";
                        bool done = false;
                        try
                        {
                            DOSEventJson.EventJson api = JsonUtil.Deserialize(typeof(DOSEventJson.EventJson), message) as DOSEventJson.EventJson;
                            if (api != null && api.type == "event" && (AppFlag.JsonType ? api.data.@event.sport_id == 5 : api.data.@event.sport_id != -1))
                            {
                                iCount++;
                                ////   if (AppFlag.TestMode) Files.WriteTestLog("Queue", iCount.ToString() + " Event " + api.id);

                                int id = -2;
                                strName = api.data.@event.id + "_" + (api.data.@event.status_id) + "_" + DateTime.Now.ToString("HHmmssfff");
                                Files.WriteJson(strName, message);
                                //if (api.data.@event.participants.Count() == 2)
                                //{
                                //    using (FbCommand cmd2 = new FbCommand())
                                //    {
                                //        cmd2.CommandText = "PR_JSON_event";
                                //        cmd2.CommandType = CommandType.StoredProcedure;
                                //        cmd2.Connection = connection;
                                //        cmd2.Parameters.Add("@ID", api.data.@event.id);
                                //        cmd2.Parameters.Add("@NAME", api.data.@event.name);
                                //        cmd2.Parameters.Add("@HOME_ID", api.data.@event.participants.Count() > 0 ? api.data.@event.participants[0].id : -1);
                                //        cmd2.Parameters.Add("@GUEST_ID", api.data.@event.participants.Count() > 0 ? api.data.@event.participants[1].id : -1);
                                //        // cmd2.Parameters.Add("@SOURCE", api.data.@evenT.S);
                                //        // cmd2.Parameters.Add("@SOURCE_DC", api.data.@event.S);
                                //        //cmd2.Parameters.Add("@SOURCE_SUPER", api.data.@event.s);
                                //        cmd2.Parameters.Add("@RELATION_STATUS", api.data.@event.relation_status);
                                //        cmd2.Parameters.Add("@START_DATE", Convert.ToDateTime(api.data.@event.start_date).AddHours(8));
                                //        cmd2.Parameters.Add("@FT_ONLY", api.data.@event.ft_only == "yes" ? true : false);
                                //        cmd2.Parameters.Add("@COVERAGE_TYPE", api.data.@event.coverage_type);
                                //        //cmd2.Parameters.Add("@CHANNEL_ID", api.data.@event.CH);
                                //        //cmd2.Parameters.Add("@CHANNEL_NAME", api.data.@event.C);
                                //        cmd2.Parameters.Add("@SCOUTSFEED", api.data.@event.scoutsfeed == "yes" ? true : false);
                                //        cmd2.Parameters.Add("@STATUS_ID", api.data.@event.status_id);
                                //        //cmd2.Parameters.Add("@STATUS_NAME", api.data.@event.STA);
                                //        cmd2.Parameters.Add("@STATUS_TYPE", api.data.@event.status_type);
                                //        cmd2.Parameters.Add("@CDAY", api.data.@event.day);
                                //        cmd2.Parameters.Add("@CLOCK_TIME", api.data.@event.clock_time);
                                //        cmd2.Parameters.Add("@CLOCK_STATUS", api.data.@event.clock_status);
                                //        // cmd2.Parameters.Add("@WINNER_ID", api.data.@event.W);
                                //        //cmd2.Parameters.Add("@PROGRESS_ID", api.data.@event.PR);
                                //        cmd2.Parameters.Add("@BET_STATUS", api.data.@event.bet_status);
                                //        cmd2.Parameters.Add("@NEUTRAL_VENUE", api.data.@event.neutral_venue == "yes" ? true : false);
                                //        cmd2.Parameters.Add("@ITEM_STATUS", api.data.@event.item_status);
                                //        cmd2.Parameters.Add("@UT", api.ut);
                                //        // cmd2.Parameters.Add("@OLD_EVENT_ID", api.data.@event.OL);
                                //        // cmd2.Parameters.Add("@SLUG", api.data.@event.S);
                                //        // cmd2.Parameters.Add("@VERIFIED_RESULT", api.data.@event.VE);
                                //        // cmd2.Parameters.Add("@IS_PROTOCOL_VERIFIED", api.data.@event.IS);
                                //        //  cmd2.Parameters.Add("@PROTOCOL_VERIFIED_BY", api.data.@event.PRO);
                                //        //cmd2.Parameters.Add("@PROTOCOL_VERIFIED_AT", api.data.@event.PRO);
                                //        cmd2.Parameters.Add("@ROUND_ID", api.data.@event.round_id);
                                //        cmd2.Parameters.Add("@ROUND_NAME", api.data.@event.round_name);
                                //        //cmd2.Parameters.Add("@CLIENT_EVENT_ID", api.data.@event.C);
                                //        // cmd2.Parameters.Add("@BOOKED", null);
                                //        // cmd2.Parameters.Add("@BOOKED_BY", api.data.@event.);
                                //        // cmd2.Parameters.Add("@INVERTED_PARTICIPANTS", api.data.@event.iv);
                                //        cmd2.Parameters.Add("@VENUE_ID", api.data.@event.tour_id == null ? "-1" : api.data.@event.tour_id);
                                //        //  cmd2.Parameters.Add("@GROUP_ID", api.data.@event.gr);
                                //        cmd2.Parameters.Add("@STAGE_ID", api.data.@event.stage_id);
                                //        cmd2.Parameters.Add("@SEASON_ID", api.data.@event.season_id);
                                //        cmd2.Parameters.Add("@COMPETITION_ID", api.data.@event.competition_id);
                                //        cmd2.Parameters.Add("@AREA_ID", api.data.@event.area_id);
                                //        cmd2.Parameters.Add("@CTIMESTAMP", DateTime.Now);
                                //        cmd2.Parameters.Add("@CACTION", api.data.@event.action);
                                //        id = Convert.ToInt32(cmd2.ExecuteScalar());
                                //        // Files.WriteLog((id == 0 ? " [Success] Insert event " : id == 1 ? " Update event " : "") + "[" + api.data.@event.id + "]," + strName + ".json.");
                                //        Files.WriteLog((id > 0 ? " [Success] Insert event " : id == 0 ? " Old data " : " [Failure] Insert event ") + "[" + api.data.@event.id + "]," + strName + ".json.");
                                //    }
                                //}
                                if (api.data.@event.details.Count() > 0)
                                {
                                    using (FbCommand cmd2 = new FbCommand())
                                    {
                                        cmd2.CommandText = "PR_event_details";
                                        cmd2.CommandType = CommandType.StoredProcedure;
                                        cmd2.Connection = connection;
                                        cmd2.Parameters.Add("@EVENTID", api.data.@event.id);
                                        cmd2.Parameters.Add("@WC_8", api.data.@event.details.FirstOrDefault(c => c.id == 8).value);
                                        cmd2.Parameters.Add("@PC_36", api.data.@event.details.FirstOrDefault(c => c.id == 36).value);
                                        cmd2.Parameters.Add("@PL_16", api.data.@event.details.FirstOrDefault(c => c.id == 16).value);
                                        cmd2.Parameters.Add("@EPL_50", api.data.@event.details.FirstOrDefault(c => c.id == 50).value);
                                        cmd2.Parameters.Add("@NOP_17", api.data.@event.details.FirstOrDefault(c => c.id == 17).value);
                                        cmd2.Parameters.Add("@EPTC_58", api.data.@event.details.FirstOrDefault(c => c.id == 58).value);
                                        cmd2.Parameters.Add("@IT_151", api.data.@event.details.FirstOrDefault(c => c.id == 151).value);
                                        cmd2.Parameters.Add("@ATT_141", api.data.@event.details.FirstOrDefault(c => c.id == 141).value);
                                        cmd2.Parameters.Add("@FHSD_19", api.data.@event.details.FirstOrDefault(c => c.id == 19).value == null ? DateTime.MinValue : Convert.ToDateTime(api.data.@event.details.FirstOrDefault(c => c.id == 19).value).AddHours(8));
                                        cmd2.Parameters.Add("@SHSD_20", api.data.@event.details.FirstOrDefault(c => c.id == 20).value == null ? DateTime.MinValue : Convert.ToDateTime(api.data.@event.details.FirstOrDefault(c => c.id == 20).value).AddHours(8));
                                        cmd2.Parameters.Add("@FEHSD_44", api.data.@event.details.FirstOrDefault(c => c.id == 44).value);
                                        cmd2.Parameters.Add("@SEHSD_45", api.data.@event.details.FirstOrDefault(c => c.id == 45).value);
                                        cmd2.Parameters.Add("@PSSD_150", api.data.@event.details.FirstOrDefault(c => c.id == 150).value);
                                        cmd2.Parameters.Add("@FHIT_201", api.data.@event.details.FirstOrDefault(c => c.id == 201).value);
                                        cmd2.Parameters.Add("@SHIT_202", api.data.@event.details.FirstOrDefault(c => c.id == 202).value);
                                        cmd2.Parameters.Add("@FEHIT_203", api.data.@event.details.FirstOrDefault(c => c.id == 203).value);
                                        cmd2.Parameters.Add("@SEHIT_204", api.data.@event.details.FirstOrDefault(c => c.id == 204).value);
                                        cmd2.Parameters.Add("@HL_205", api.data.@event.details.FirstOrDefault(c => c.id == 205).value);
                                        cmd2.Parameters.Add("@TD_124", api.data.@event.details.FirstOrDefault(c => c.id == 124).value);
                                        cmd2.Parameters.Add("@BM_160", api.data.@event.details.FirstOrDefault(c => c.id == 160).value);
                                        cmd2.Parameters.Add("@HF_178", api.data.@event.details.FirstOrDefault(c => c.id == 178).value);
                                        cmd2.Parameters.Add("@UT", api.ut);
                                        cmd2.Parameters.Add("@CTIMESTAMP", DateTime.Now);
                                        id = Convert.ToInt32(cmd2.ExecuteScalar());
                                        Files.WriteLog((id > 0 ? " [Success] Insert event_details " : id == 0 ? " Old details " : " [Failure] Insert event_details ") + "[" + api.data.@event.id + "]," + strName + ".json.");
                                    }
                                }

                                if (api.data.@event.participants.Count() == 2 && api.data.@event.participants[0].stats.Length > 0 && api.data.@event.participants[0].results.Length > 0)
                                {
                                    for (int i = 0; i < api.data.@event.participants.Length && api.data.@event.participants.Count() == 2; i++)
                                    {
                                        using (FbCommand cmd2 = new FbCommand())
                                        {
                                            cmd2.CommandText = "PR_participant_results";
                                            cmd2.CommandType = CommandType.StoredProcedure;
                                            cmd2.Connection = connection;
                                            // cmd2.Parameters.Add("@ID", 0);
                                            cmd2.Parameters.Add("@EVENTID", api.data.@event.id);
                                            cmd2.Parameters.Add("@PARTICIPANTID", api.data.@event.participants[i].id);
                                            cmd2.Parameters.Add("@PROGRESS_412", api.data.@event.participants[i].results.FirstOrDefault(c => c.id == 412).value);
                                            cmd2.Parameters.Add("@WINNER_411", api.data.@event.participants[i].results.FirstOrDefault(c => c.id == 411).value);
                                            cmd2.Parameters.Add("@RESULT_2", api.data.@event.participants[i].results.FirstOrDefault(c => c.id == 2).value);
                                            cmd2.Parameters.Add("@RT_3", api.data.@event.participants[i].results.FirstOrDefault(c => c.id == 3).value);
                                            cmd2.Parameters.Add("@FH_4", api.data.@event.participants[i].results.FirstOrDefault(c => c.id == 4).value);
                                            cmd2.Parameters.Add("@SH_5", api.data.@event.participants[i].results.FirstOrDefault(c => c.id == 5).value);
                                            cmd2.Parameters.Add("@E1H_133", api.data.@event.participants[i].results.FirstOrDefault(c => c.id == 133).value);
                                            cmd2.Parameters.Add("@E2H_134", api.data.@event.participants[i].results.FirstOrDefault(c => c.id == 134).value);
                                            cmd2.Parameters.Add("@PENALTY_7", api.data.@event.participants[i].results.FirstOrDefault(c => c.id == 7).value);
                                            cmd2.Parameters.Add("@OVERTIME_104", api.data.@event.participants[i].results.FirstOrDefault(c => c.id == 104).value);
                                            cmd2.Parameters.Add("@CTIMESTAMP", DateTime.Now);
                                            cmd2.Parameters.Add("@CACTION", api.data.@event.action);
                                            cmd2.Parameters.Add("@TEAMTYPE", api.data.@event.participants[i].counter == 1 ? "H" : "G");
                                            id = Convert.ToInt32(cmd2.ExecuteScalar());
                                            Files.WriteLog((id > 0 ? " [Success] Insert participant_results" : " [Failure] Insert participant_results") + "[" + api.data.@event.id + "/" + api.data.@event.participants[0].id + "].");
                                        }

                                        using (FbCommand cmd2 = new FbCommand())
                                        {
                                            cmd2.CommandText = "PR_participant_stats";
                                            cmd2.CommandType = CommandType.StoredProcedure;
                                            cmd2.Connection = connection;
                                            // cmd2.Parameters.Add("@ID", 0);
                                            cmd2.Parameters.Add("@EVENTID", api.data.@event.id);
                                            cmd2.Parameters.Add("@PARTICIPANTID", api.data.@event.participants[i].id);
                                            cmd2.Parameters.Add("@SOT_20", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 20).value);
                                            cmd2.Parameters.Add("@SOT_21", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 21).value);
                                            cmd2.Parameters.Add("@ATTACKS_10", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 10).value);
                                            cmd2.Parameters.Add("@DA_11", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 11).value);
                                            cmd2.Parameters.Add("@CORNERS_13", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 13).value);
                                            cmd2.Parameters.Add("@YELLOW_CARDS_8", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 8).value);
                                            cmd2.Parameters.Add("@RED_CARDS_9", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 9).value);
                                            cmd2.Parameters.Add("@TOTAL_SHOTS_19", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 19).value);
                                            cmd2.Parameters.Add("@FOULS_22", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 22).value);
                                            cmd2.Parameters.Add("@OFFSIDES_24", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 24).value);
                                            cmd2.Parameters.Add("@PS_14", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 14).value);
                                            cmd2.Parameters.Add("@PM_15", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 15).value);
                                            cmd2.Parameters.Add("@PG_16", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 16).value);
                                            cmd2.Parameters.Add("@FK_25", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 25).value);
                                            cmd2.Parameters.Add("@DFK_26", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 26).value);
                                            cmd2.Parameters.Add("@FKG_18", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 18).value);
                                            cmd2.Parameters.Add("@SW_27", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 27).value);
                                            cmd2.Parameters.Add("@SB_28", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 28).value);
                                            cmd2.Parameters.Add("@GS_29", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 29).value);
                                            cmd2.Parameters.Add("@GK_30", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 30).value);
                                            cmd2.Parameters.Add("@TI_32", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 32).value);
                                            cmd2.Parameters.Add("@SUBSTITUTIONS_31", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 31).value);
                                            cmd2.Parameters.Add("@GOALS_40", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 40).value);
                                            cmd2.Parameters.Add("@MP_34", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 34).value);
                                            cmd2.Parameters.Add("@OWN_GOALS_17", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 17).value);
                                            cmd2.Parameters.Add("@ADW_33", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 33).value);
                                            cmd2.Parameters.Add("@FORM_716", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 716).value);
                                            cmd2.Parameters.Add("@SKIN_718", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 718).value);
                                            cmd2.Parameters.Add("@PS_639", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 639).value);
                                            cmd2.Parameters.Add("@PU_697", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 697).value);
                                            cmd2.Parameters.Add("@GOALS115_772", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 772).value);
                                            cmd2.Parameters.Add("@GOALS1630_773", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 773).value);
                                            cmd2.Parameters.Add("@GOALS3145_774", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 774).value);
                                            cmd2.Parameters.Add("@GOALS4660_775", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 775).value);
                                            cmd2.Parameters.Add("@GOALS6175_776", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 776).value);
                                            cmd2.Parameters.Add("@GOALS7690_777", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 777).value);
                                            cmd2.Parameters.Add("@MPG_778", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 778).value);
                                            cmd2.Parameters.Add("@MPS_779", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 779).value);
                                            cmd2.Parameters.Add("@CTIMESTAMP", DateTime.Now);
                                            cmd2.Parameters.Add("@CACTION", api.data.@event.action);
                                            cmd2.Parameters.Add("@TEAMTYPE", api.data.@event.participants[i].counter == 2 ? "G" : "H");
                                            id = Convert.ToInt32(cmd2.ExecuteScalar());
                                            Files.WriteLog((id > 0 ? " [Success] Insert participant_stats" : " [Failure] Insert participant_results") + "[" + api.data.@event.id + "/" + api.data.@event.participants[0].id + "].");
                                            //  Thread.Sleep(20);
                                        }
                                    }

                                    if (api.data.@event.action != "delete")
                                    {
                                        using (FbCommand cmd2 = new FbCommand())
                                        {
                                            cmd2.CommandText = "PR_Stats_Result_GoalInfo";
                                            cmd2.CommandType = CommandType.StoredProcedure;
                                            cmd2.Connection = connection;
                                            cmd2.Parameters.Add("@EMATCHID", api.data.@event.id);
                                            cmd2.Parameters.Add("@H_YELLOW", api.data.@event.participants[0].stats.FirstOrDefault(c => c.id == 8).value);
                                            cmd2.Parameters.Add("@H_RED", api.data.@event.participants[0].stats.FirstOrDefault(c => c.id == 9).value);
                                            cmd2.Parameters.Add("@G_YELLOW", api.data.@event.participants[1].stats.FirstOrDefault(c => c.id == 8).value);
                                            cmd2.Parameters.Add("@G_RED", api.data.@event.participants[1].stats.FirstOrDefault(c => c.id == 9).value);
                                            cmd2.Parameters.Add("@H_GOAL", api.data.@event.participants[0].results.FirstOrDefault(c => c.id == 2).value);
                                            cmd2.Parameters.Add("@G_GOAL", api.data.@event.participants[1].results.FirstOrDefault(c => c.id == 2).value);
                                            cmd2.Parameters.Add("@STATUSID", api.data.@event.status_id);
                                            cmd2.Parameters.Add("@LASTTIME", DateTime.Now);
                                            id = Convert.ToInt32(cmd2.ExecuteScalar());
                                            if (id > -1)
                                            {
                                                Files.WriteLog(" [Success] Update GoalInfo " + "[" + api.data.@event.id + "] " + api.data.@event.participants[0].id + "/" + api.data.@event.participants[1].id + " " + id + ".");
                                                if (id > 0) done = true;
                                            }
                                        }
                                    }
                                    using (FbCommand cmd2 = new FbCommand())
                                    {
                                        cmd2.CommandText = "PR_JSON_event";
                                        cmd2.CommandType = CommandType.StoredProcedure;
                                        cmd2.Connection = connection;
                                        cmd2.Parameters.Add("@ID", api.data.@event.id);
                                        cmd2.Parameters.Add("@NAME", api.data.@event.name);
                                        cmd2.Parameters.Add("@HOME_ID", api.data.@event.participants.Count() > 0 ? api.data.@event.participants[0].id : -1);
                                        cmd2.Parameters.Add("@GUEST_ID", api.data.@event.participants.Count() > 0 ? api.data.@event.participants[1].id : -1);
                                        // cmd2.Parameters.Add("@SOURCE", api.data.@evenT.S);
                                        // cmd2.Parameters.Add("@SOURCE_DC", api.data.@event.S);
                                        //cmd2.Parameters.Add("@SOURCE_SUPER", api.data.@event.s);
                                        cmd2.Parameters.Add("@RELATION_STATUS", api.data.@event.relation_status);
                                        cmd2.Parameters.Add("@START_DATE", Convert.ToDateTime(api.data.@event.start_date).AddHours(8));
                                        cmd2.Parameters.Add("@FT_ONLY", api.data.@event.ft_only == "yes" ? true : false);
                                        cmd2.Parameters.Add("@COVERAGE_TYPE", api.data.@event.coverage_type);
                                        //cmd2.Parameters.Add("@CHANNEL_ID", api.data.@event.CH);
                                        //cmd2.Parameters.Add("@CHANNEL_NAME", api.data.@event.C);
                                        cmd2.Parameters.Add("@SCOUTSFEED", api.data.@event.scoutsfeed == "yes" ? true : false);
                                        cmd2.Parameters.Add("@STATUS_ID", api.data.@event.status_id);
                                        //cmd2.Parameters.Add("@STATUS_NAME", api.data.@event.STA);
                                        cmd2.Parameters.Add("@STATUS_TYPE", api.data.@event.status_type);
                                        cmd2.Parameters.Add("@CDAY", api.data.@event.day);
                                        cmd2.Parameters.Add("@CLOCK_TIME", api.data.@event.clock_time);
                                        cmd2.Parameters.Add("@CLOCK_STATUS", api.data.@event.clock_status);
                                        // cmd2.Parameters.Add("@WINNER_ID", api.data.@event.W);
                                        //cmd2.Parameters.Add("@PROGRESS_ID", api.data.@event.PR);
                                        cmd2.Parameters.Add("@BET_STATUS", api.data.@event.bet_status);
                                        cmd2.Parameters.Add("@NEUTRAL_VENUE", api.data.@event.neutral_venue == "yes" ? true : false);
                                        cmd2.Parameters.Add("@ITEM_STATUS", api.data.@event.item_status);
                                        cmd2.Parameters.Add("@UT", api.ut);
                                        // cmd2.Parameters.Add("@OLD_EVENT_ID", api.data.@event.OL);
                                        // cmd2.Parameters.Add("@SLUG", api.data.@event.S);
                                        // cmd2.Parameters.Add("@VERIFIED_RESULT", api.data.@event.VE);
                                        // cmd2.Parameters.Add("@IS_PROTOCOL_VERIFIED", api.data.@event.IS);
                                        //  cmd2.Parameters.Add("@PROTOCOL_VERIFIED_BY", api.data.@event.PRO);
                                        //cmd2.Parameters.Add("@PROTOCOL_VERIFIED_AT", api.data.@event.PRO);
                                        cmd2.Parameters.Add("@ROUND_ID", api.data.@event.round_id);
                                        cmd2.Parameters.Add("@ROUND_NAME", api.data.@event.round_name);
                                        //cmd2.Parameters.Add("@CLIENT_EVENT_ID", api.data.@event.C);
                                        // cmd2.Parameters.Add("@BOOKED", null);
                                        // cmd2.Parameters.Add("@BOOKED_BY", api.data.@event.);
                                        // cmd2.Parameters.Add("@INVERTED_PARTICIPANTS", api.data.@event.iv);
                                        cmd2.Parameters.Add("@VENUE_ID", api.data.@event.tour_id == null ? "-1" : api.data.@event.tour_id);
                                        //  cmd2.Parameters.Add("@GROUP_ID", api.data.@event.gr);
                                        cmd2.Parameters.Add("@STAGE_ID", api.data.@event.stage_id);
                                        cmd2.Parameters.Add("@SEASON_ID", api.data.@event.season_id);
                                        cmd2.Parameters.Add("@COMPETITION_ID", api.data.@event.competition_id);
                                        cmd2.Parameters.Add("@AREA_ID", api.data.@event.area_id);
                                        cmd2.Parameters.Add("@CTIMESTAMP", DateTime.Now);
                                        cmd2.Parameters.Add("@CACTION", api.data.@event.action);
                                        id = Convert.ToInt32(cmd2.ExecuteScalar());
                                        // Files.WriteLog((id == 0 ? " [Success] Insert event " : id == 1 ? " Update event " : "") + "[" + api.data.@event.id + "]," + strName + ".json.");
                                        Files.WriteLog((id > 0 ? " [Success] Insert event " : id == 0 ? " Old data " : " [Failure] Insert event ") + "[" + api.data.@event.id + "]," + strName + ".json." + api.data.@event.status_id);
                                        //1312019
                                        if (id == 0) done = false;
                                    }
                                }

                                if (done)
                                {
                                    if (api.data.@event.status_id == 6 || api.data.@event.status_id == 9 || api.data.@event.status_id == 11)
                                    {
                                        if (api.data.@event.id > 0)
                                        {
                                            /// await AyncHandleData("events.show3", true, api.data.@event.id.ToString());
                                            InsertData("events.show3", true, api.data.@event.id.ToString(), true);
                                            Files.WriteLog(" Housekeep [" + api.data.@event.id.ToString() + "].." + api.data.@event.status_id);
                                        }
                                    }
                                    else
                                    {
                                        InsertData("events.show3", true, api.data.@event.id.ToString(), true);
                                        Files.WriteLog(" Get event [" + api.data.@event.id.ToString() + "].." + api.data.@event.status_id);
                                    }

                                    SendAlertMsg(AppFlag.LIVEGOALS);
                                    done = false;
                                }

                                if (id > 0)
                                {
                                    this.lstStatus.Invoke(new Action(() => { { this.lstStatus.Items.Insert(0, DateTime.Now.ToString("HH:mm:ss.fff   ") + api.data.@event.name + " " + strName + ".json"); } }));
                                }

                                //if (api.data.@event.status_id == 6 || api.data.@event.status_id == 9 || api.data.@event.status_id == 11)
                                //{
                                //    if (api.data.@event.id > 0)
                                //    {
                                //        /// await AyncHandleData("events.show3", true, api.data.@event.id.ToString());
                                //        InsertData("events.show3", true, api.data.@event.id.ToString(), false);
                                //        Files.WriteLog(" Housekeep [" + api.data.@event.id.ToString() + "].." + api.data.@event.status_id);
                                //    }
                                //}

                                //old2
                                //if (api.data.@event.details.Count() > 0)
                                //{
                                //    using (FbCommand cmd2 = new FbCommand())
                                //    {
                                //        cmd2.CommandText = "PR_event_details";
                                //        cmd2.CommandType = CommandType.StoredProcedure;
                                //        cmd2.Connection = connection;
                                //        cmd2.Parameters.Add("@EVENTID", api.data.@event.id);
                                //        cmd2.Parameters.Add("@WC_8", api.data.@event.details.FirstOrDefault(c => c.id == 8).value);
                                //        cmd2.Parameters.Add("@PC_36", api.data.@event.details.FirstOrDefault(c => c.id == 36).value);
                                //        cmd2.Parameters.Add("@PL_16", api.data.@event.details.FirstOrDefault(c => c.id == 16).value);
                                //        cmd2.Parameters.Add("@EPL_50", api.data.@event.details.FirstOrDefault(c => c.id == 50).value);
                                //        cmd2.Parameters.Add("@NOP_17", api.data.@event.details.FirstOrDefault(c => c.id == 17).value);
                                //        cmd2.Parameters.Add("@EPTC_58", api.data.@event.details.FirstOrDefault(c => c.id == 58).value);
                                //        cmd2.Parameters.Add("@IT_151", api.data.@event.details.FirstOrDefault(c => c.id == 151).value);
                                //        cmd2.Parameters.Add("@ATT_141", api.data.@event.details.FirstOrDefault(c => c.id == 141).value);
                                //        cmd2.Parameters.Add("@FHSD_19", api.data.@event.details.FirstOrDefault(c => c.id == 19).value == null ? DateTime.MinValue : Convert.ToDateTime(api.data.@event.details.FirstOrDefault(c => c.id == 19).value).AddHours(8));
                                //        cmd2.Parameters.Add("@SHSD_20", api.data.@event.details.FirstOrDefault(c => c.id == 20).value == null ? DateTime.MinValue : Convert.ToDateTime(api.data.@event.details.FirstOrDefault(c => c.id == 20).value).AddHours(8));
                                //        cmd2.Parameters.Add("@FEHSD_44", api.data.@event.details.FirstOrDefault(c => c.id == 44).value);
                                //        cmd2.Parameters.Add("@SEHSD_45", api.data.@event.details.FirstOrDefault(c => c.id == 45).value);
                                //        cmd2.Parameters.Add("@PSSD_150", api.data.@event.details.FirstOrDefault(c => c.id == 150).value);
                                //        cmd2.Parameters.Add("@FHIT_201", api.data.@event.details.FirstOrDefault(c => c.id == 201).value);
                                //        cmd2.Parameters.Add("@SHIT_202", api.data.@event.details.FirstOrDefault(c => c.id == 202).value);
                                //        cmd2.Parameters.Add("@FEHIT_203", api.data.@event.details.FirstOrDefault(c => c.id == 203).value);
                                //        cmd2.Parameters.Add("@SEHIT_204", api.data.@event.details.FirstOrDefault(c => c.id == 204).value);
                                //        cmd2.Parameters.Add("@HL_205", api.data.@event.details.FirstOrDefault(c => c.id == 205).value);
                                //        cmd2.Parameters.Add("@TD_124", api.data.@event.details.FirstOrDefault(c => c.id == 124).value);
                                //        cmd2.Parameters.Add("@BM_160", api.data.@event.details.FirstOrDefault(c => c.id == 160).value);
                                //        cmd2.Parameters.Add("@HF_178", api.data.@event.details.FirstOrDefault(c => c.id == 178).value);
                                //        cmd2.Parameters.Add("@UT", api.ut);
                                //        cmd2.Parameters.Add("@CTIMESTAMP", DateTime.Now);
                                //        id = Convert.ToInt32(cmd2.ExecuteScalar());
                                //        Files.WriteLog((id > 0 ? " [Success] Insert event_details " : id == -1 ? " Old data " : " [Failure] Insert event_details ") + "[" + api.data.@event.id + "]," + strName + ".json.");
                                //    }
                                //}
                                //if (api.data.@event.participants.Count() == 2&& api.data.@event.participants[0].stats.Length>0&& api.data.@event.participants[0].results.Length  >0)
                                //{
                                //    if (api.data.@event.action != "delete")
                                //    {

                                //        using (FbCommand cmd2 = new FbCommand())
                                //        {
                                //            cmd2.CommandText = "PR_Stats_Result_GoalInfo";
                                //            cmd2.CommandType = CommandType.StoredProcedure;
                                //            cmd2.Connection = connection;
                                //            cmd2.Parameters.Add("@EMATCHID", api.data.@event.id);
                                //            cmd2.Parameters.Add("@H_YELLOW", api.data.@event.participants[0].stats.FirstOrDefault(c => c.id == 8).value);
                                //            cmd2.Parameters.Add("@H_RED", api.data.@event.participants[0].stats.FirstOrDefault(c => c.id == 9).value);
                                //            cmd2.Parameters.Add("@G_YELLOW", api.data.@event.participants[1].stats.FirstOrDefault(c => c.id == 8).value);
                                //            cmd2.Parameters.Add("@G_RED", api.data.@event.participants[1].stats.FirstOrDefault(c => c.id == 9).value);
                                //            cmd2.Parameters.Add("@H_GOAL", api.data.@event.participants[0].results.FirstOrDefault(c => c.id == 2).value);
                                //            cmd2.Parameters.Add("@G_GOAL", api.data.@event.participants[1].results.FirstOrDefault(c => c.id == 2).value);
                                //            cmd2.Parameters.Add("@STATUSID", api.data.@event.status_id);
                                //            cmd2.Parameters.Add("@LASTTIME", DateTime.Now);
                                //            id = Convert.ToInt32(cmd2.ExecuteScalar());
                                //            if (id > -1)
                                //            {
                                //                Files.WriteLog(" [Success] Update GoalInfo " + "[" + api.data.@event.id + "/" + api.data.@event.participants[0].id + "]" + id + ".");
                                //                if (id > 0) done = true;
                                //            }
                                //        }
                                //    }

                                //    using (FbCommand cmd2 = new FbCommand())
                                //    {
                                //        cmd2.CommandText = "PR_JSON_event";
                                //        cmd2.CommandType = CommandType.StoredProcedure;
                                //        cmd2.Connection = connection;
                                //        cmd2.Parameters.Add("@ID", api.data.@event.id);
                                //        cmd2.Parameters.Add("@NAME", api.data.@event.name);
                                //        cmd2.Parameters.Add("@HOME_ID", api.data.@event.participants.Count() > 0 ? api.data.@event.participants[0].id : -1);
                                //        cmd2.Parameters.Add("@GUEST_ID", api.data.@event.participants.Count() > 0 ? api.data.@event.participants[1].id : -1);
                                //        // cmd2.Parameters.Add("@SOURCE", api.data.@evenT.S);
                                //        // cmd2.Parameters.Add("@SOURCE_DC", api.data.@event.S);
                                //        //cmd2.Parameters.Add("@SOURCE_SUPER", api.data.@event.s);
                                //        cmd2.Parameters.Add("@RELATION_STATUS", api.data.@event.relation_status);
                                //        cmd2.Parameters.Add("@START_DATE", Convert.ToDateTime(api.data.@event.start_date).AddHours(8));
                                //        cmd2.Parameters.Add("@FT_ONLY", api.data.@event.ft_only == "yes" ? true : false);
                                //        cmd2.Parameters.Add("@COVERAGE_TYPE", api.data.@event.coverage_type);
                                //        //cmd2.Parameters.Add("@CHANNEL_ID", api.data.@event.CH);
                                //        //cmd2.Parameters.Add("@CHANNEL_NAME", api.data.@event.C);
                                //        cmd2.Parameters.Add("@SCOUTSFEED", api.data.@event.scoutsfeed == "yes" ? true : false);
                                //        cmd2.Parameters.Add("@STATUS_ID", api.data.@event.status_id);
                                //        //cmd2.Parameters.Add("@STATUS_NAME", api.data.@event.STA);
                                //        cmd2.Parameters.Add("@STATUS_TYPE", api.data.@event.status_type);
                                //        cmd2.Parameters.Add("@CDAY", api.data.@event.day);
                                //        cmd2.Parameters.Add("@CLOCK_TIME", api.data.@event.clock_time);
                                //        cmd2.Parameters.Add("@CLOCK_STATUS", api.data.@event.clock_status);
                                //        // cmd2.Parameters.Add("@WINNER_ID", api.data.@event.W);
                                //        //cmd2.Parameters.Add("@PROGRESS_ID", api.data.@event.PR);
                                //        cmd2.Parameters.Add("@BET_STATUS", api.data.@event.bet_status);
                                //        cmd2.Parameters.Add("@NEUTRAL_VENUE", api.data.@event.neutral_venue == "yes" ? true : false);
                                //        cmd2.Parameters.Add("@ITEM_STATUS", api.data.@event.item_status);
                                //        cmd2.Parameters.Add("@UT", api.ut);
                                //        // cmd2.Parameters.Add("@OLD_EVENT_ID", api.data.@event.OL);
                                //        // cmd2.Parameters.Add("@SLUG", api.data.@event.S);
                                //        // cmd2.Parameters.Add("@VERIFIED_RESULT", api.data.@event.VE);
                                //        // cmd2.Parameters.Add("@IS_PROTOCOL_VERIFIED", api.data.@event.IS);
                                //        //  cmd2.Parameters.Add("@PROTOCOL_VERIFIED_BY", api.data.@event.PRO);
                                //        //cmd2.Parameters.Add("@PROTOCOL_VERIFIED_AT", api.data.@event.PRO);
                                //        cmd2.Parameters.Add("@ROUND_ID", api.data.@event.round_id);
                                //        cmd2.Parameters.Add("@ROUND_NAME", api.data.@event.round_name);
                                //        //cmd2.Parameters.Add("@CLIENT_EVENT_ID", api.data.@event.C);
                                //        // cmd2.Parameters.Add("@BOOKED", null);
                                //        // cmd2.Parameters.Add("@BOOKED_BY", api.data.@event.);
                                //        // cmd2.Parameters.Add("@INVERTED_PARTICIPANTS", api.data.@event.iv);
                                //        cmd2.Parameters.Add("@VENUE_ID", api.data.@event.tour_id == null ? "-1" : api.data.@event.tour_id);
                                //        //  cmd2.Parameters.Add("@GROUP_ID", api.data.@event.gr);
                                //        cmd2.Parameters.Add("@STAGE_ID", api.data.@event.stage_id);
                                //        cmd2.Parameters.Add("@SEASON_ID", api.data.@event.season_id);
                                //        cmd2.Parameters.Add("@COMPETITION_ID", api.data.@event.competition_id);
                                //        cmd2.Parameters.Add("@AREA_ID", api.data.@event.area_id);
                                //        cmd2.Parameters.Add("@CTIMESTAMP", DateTime.Now);
                                //        cmd2.Parameters.Add("@CACTION", api.data.@event.action);
                                //        id = Convert.ToInt32(cmd2.ExecuteScalar());
                                //        Files.WriteLog((id == 0 ? " [Success] Insert event " : id == 1 ? " Update event " : "") + "[" + api.data.@event.id + "]," + strName + ".json.");
                                //        // Files.WriteLog((id == 0 ? " [Success] Insert event [" + api.data.@event.id + "]," + strName + ".json":"");
                                //    }
                                //    for (int i = 0; i < api.data.@event.participants.Length && api.data.@event.participants.Count() == 2; i++)
                                //    {
                                //        using (FbCommand cmd2 = new FbCommand())
                                //        {
                                //            cmd2.CommandText = "PR_participant_results";
                                //            cmd2.CommandType = CommandType.StoredProcedure;
                                //            cmd2.Connection = connection;
                                //            // cmd2.Parameters.Add("@ID", 0);
                                //            cmd2.Parameters.Add("@EVENTID", api.data.@event.id);
                                //            cmd2.Parameters.Add("@PARTICIPANTID", api.data.@event.participants[i].id);
                                //            cmd2.Parameters.Add("@PROGRESS_412", api.data.@event.participants[i].results.FirstOrDefault(c => c.id == 412).value);
                                //            cmd2.Parameters.Add("@WINNER_411", api.data.@event.participants[i].results.FirstOrDefault(c => c.id == 411).value);
                                //            cmd2.Parameters.Add("@RESULT_2", api.data.@event.participants[i].results.FirstOrDefault(c => c.id == 2).value);
                                //            cmd2.Parameters.Add("@RT_3", api.data.@event.participants[i].results.FirstOrDefault(c => c.id == 3).value);
                                //            cmd2.Parameters.Add("@FH_4", api.data.@event.participants[i].results.FirstOrDefault(c => c.id == 4).value);
                                //            cmd2.Parameters.Add("@SH_5", api.data.@event.participants[i].results.FirstOrDefault(c => c.id == 5).value);
                                //            cmd2.Parameters.Add("@E1H_133", api.data.@event.participants[i].results.FirstOrDefault(c => c.id == 133).value);
                                //            cmd2.Parameters.Add("@E2H_134", api.data.@event.participants[i].results.FirstOrDefault(c => c.id == 134).value);
                                //            cmd2.Parameters.Add("@PENALTY_7", api.data.@event.participants[i].results.FirstOrDefault(c => c.id == 7).value);
                                //            cmd2.Parameters.Add("@OVERTIME_104", api.data.@event.participants[i].results.FirstOrDefault(c => c.id == 104).value);
                                //            cmd2.Parameters.Add("@CTIMESTAMP", DateTime.Now);
                                //            cmd2.Parameters.Add("@CACTION", api.data.@event.action);
                                //            cmd2.Parameters.Add("@TEAMTYPE", api.data.@event.participants[i].counter == 1 ? "H" : "G");
                                //            id = Convert.ToInt32(cmd2.ExecuteScalar());
                                //            Files.WriteLog((id > 0 ? " [Success] Insert participant_results" : " [Failure] Insert participant_results") + "[" + api.data.@event.id + "/" + api.data.@event.participants[0].id + "].");
                                //        }

                                //        using (FbCommand cmd2 = new FbCommand())
                                //        {
                                //            cmd2.CommandText = "PR_participant_stats";
                                //            cmd2.CommandType = CommandType.StoredProcedure;
                                //            cmd2.Connection = connection;
                                //            // cmd2.Parameters.Add("@ID", 0);
                                //            cmd2.Parameters.Add("@EVENTID", api.data.@event.id);
                                //            cmd2.Parameters.Add("@PARTICIPANTID", api.data.@event.participants[i].id);
                                //            cmd2.Parameters.Add("@SOT_20", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 20).value);
                                //            cmd2.Parameters.Add("@SOT_21", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 21).value);
                                //            cmd2.Parameters.Add("@ATTACKS_10", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 10).value);
                                //            cmd2.Parameters.Add("@DA_11", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 11).value);
                                //            cmd2.Parameters.Add("@CORNERS_13", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 13).value);
                                //            cmd2.Parameters.Add("@YELLOW_CARDS_8", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 8).value);
                                //            cmd2.Parameters.Add("@RED_CARDS_9", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 9).value);
                                //            cmd2.Parameters.Add("@TOTAL_SHOTS_19", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 19).value);
                                //            cmd2.Parameters.Add("@FOULS_22", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 22).value);
                                //            cmd2.Parameters.Add("@OFFSIDES_24", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 24).value);
                                //            cmd2.Parameters.Add("@PS_14", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 14).value);
                                //            cmd2.Parameters.Add("@PM_15", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 15).value);
                                //            cmd2.Parameters.Add("@PG_16", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 16).value);
                                //            cmd2.Parameters.Add("@FK_25", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 25).value);
                                //            cmd2.Parameters.Add("@DFK_26", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 26).value);
                                //            cmd2.Parameters.Add("@FKG_18", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 18).value);
                                //            cmd2.Parameters.Add("@SW_27", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 27).value);
                                //            cmd2.Parameters.Add("@SB_28", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 28).value);
                                //            cmd2.Parameters.Add("@GS_29", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 29).value);
                                //            cmd2.Parameters.Add("@GK_30", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 30).value);
                                //            cmd2.Parameters.Add("@TI_32", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 32).value);
                                //            cmd2.Parameters.Add("@SUBSTITUTIONS_31", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 31).value);
                                //            cmd2.Parameters.Add("@GOALS_40", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 40).value);
                                //            cmd2.Parameters.Add("@MP_34", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 34).value);
                                //            cmd2.Parameters.Add("@OWN_GOALS_17", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 17).value);
                                //            cmd2.Parameters.Add("@ADW_33", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 33).value);
                                //            cmd2.Parameters.Add("@FORM_716", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 716).value);
                                //            cmd2.Parameters.Add("@SKIN_718", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 718).value);
                                //            cmd2.Parameters.Add("@PS_639", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 639).value);
                                //            cmd2.Parameters.Add("@PU_697", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 697).value);
                                //            cmd2.Parameters.Add("@GOALS115_772", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 772).value);
                                //            cmd2.Parameters.Add("@GOALS1630_773", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 773).value);
                                //            cmd2.Parameters.Add("@GOALS3145_774", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 774).value);
                                //            cmd2.Parameters.Add("@GOALS4660_775", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 775).value);
                                //            cmd2.Parameters.Add("@GOALS6175_776", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 776).value);
                                //            cmd2.Parameters.Add("@GOALS7690_777", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 777).value);
                                //            cmd2.Parameters.Add("@MPG_778", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 778).value);
                                //            cmd2.Parameters.Add("@MPS_779", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 779).value);
                                //            cmd2.Parameters.Add("@CTIMESTAMP", DateTime.Now);
                                //            cmd2.Parameters.Add("@CACTION", api.data.@event.action);
                                //            cmd2.Parameters.Add("@TEAMTYPE", api.data.@event.participants[i].counter == 2 ? "G" : "H");
                                //            id = Convert.ToInt32(cmd2.ExecuteScalar());
                                //            Files.WriteLog((id > 0 ? " [Success] Insert participant_stats" : " [Failure] Insert participant_results") + "[" + api.data.@event.id + "/" + api.data.@event.participants[0].id + "].");
                                //            //  Thread.Sleep(20);
                                //        }
                                //    }
                                //}

                                //old
                                //if (api.data.@event.details.Count() > 0)
                                //{
                                //    using (FbCommand cmd2 = new FbCommand())
                                //    {
                                //        cmd2.CommandText = "PR_event_details";
                                //        cmd2.CommandType = CommandType.StoredProcedure;
                                //        cmd2.Connection = connection;
                                //        cmd2.Parameters.Add("@EVENTID", api.data.@event.id);
                                //        cmd2.Parameters.Add("@WC_8", api.data.@event.details.FirstOrDefault(c => c.id == 8).value);
                                //        cmd2.Parameters.Add("@PC_36", api.data.@event.details.FirstOrDefault(c => c.id == 36).value);
                                //        cmd2.Parameters.Add("@PL_16", api.data.@event.details.FirstOrDefault(c => c.id == 16).value);
                                //        cmd2.Parameters.Add("@EPL_50", api.data.@event.details.FirstOrDefault(c => c.id == 50).value);
                                //        cmd2.Parameters.Add("@NOP_17", api.data.@event.details.FirstOrDefault(c => c.id == 17).value);
                                //        cmd2.Parameters.Add("@EPTC_58", api.data.@event.details.FirstOrDefault(c => c.id == 58).value);
                                //        cmd2.Parameters.Add("@IT_151", api.data.@event.details.FirstOrDefault(c => c.id == 151).value);
                                //        cmd2.Parameters.Add("@ATT_141", api.data.@event.details.FirstOrDefault(c => c.id == 141).value);
                                //        cmd2.Parameters.Add("@FHSD_19", api.data.@event.details.FirstOrDefault(c => c.id == 19).value == null ? DateTime.MinValue : Convert.ToDateTime(api.data.@event.details.FirstOrDefault(c => c.id == 19).value).AddHours(8));
                                //        cmd2.Parameters.Add("@SHSD_20", api.data.@event.details.FirstOrDefault(c => c.id == 20).value == null ? DateTime.MinValue : Convert.ToDateTime(api.data.@event.details.FirstOrDefault(c => c.id == 20).value).AddHours(8));
                                //        cmd2.Parameters.Add("@FEHSD_44", api.data.@event.details.FirstOrDefault(c => c.id == 44).value);
                                //        cmd2.Parameters.Add("@SEHSD_45", api.data.@event.details.FirstOrDefault(c => c.id == 45).value);
                                //        cmd2.Parameters.Add("@PSSD_150", api.data.@event.details.FirstOrDefault(c => c.id == 150).value);
                                //        cmd2.Parameters.Add("@FHIT_201", api.data.@event.details.FirstOrDefault(c => c.id == 201).value);
                                //        cmd2.Parameters.Add("@SHIT_202", api.data.@event.details.FirstOrDefault(c => c.id == 202).value);
                                //        cmd2.Parameters.Add("@FEHIT_203", api.data.@event.details.FirstOrDefault(c => c.id == 203).value);
                                //        cmd2.Parameters.Add("@SEHIT_204", api.data.@event.details.FirstOrDefault(c => c.id == 204).value);
                                //        cmd2.Parameters.Add("@HL_205", api.data.@event.details.FirstOrDefault(c => c.id == 205).value);
                                //        cmd2.Parameters.Add("@TD_124", api.data.@event.details.FirstOrDefault(c => c.id == 124).value);
                                //        cmd2.Parameters.Add("@BM_160", api.data.@event.details.FirstOrDefault(c => c.id == 160).value);
                                //        cmd2.Parameters.Add("@HF_178", api.data.@event.details.FirstOrDefault(c => c.id == 178).value);
                                //        cmd2.Parameters.Add("@UT", api.ut);
                                //        cmd2.Parameters.Add("@CTIMESTAMP", DateTime.Now);
                                //        id = Convert.ToInt32(cmd2.ExecuteScalar());
                                //        Files.WriteLog((id > 0 ? " [Success] Insert event_details " : id == -1 ? " Old data " : " [Failure] Insert event_details ") + "[" + api.data.@event.id + "]," + strName + ".json.");
                                //    }

                                //if (id != -1)
                                //{
                                //for (int i = 0; i < api.data.@event.participants.Length && api.data.@event.participants.Count() == 2; i++)
                                //{
                                //    using (FbCommand cmd2 = new FbCommand())
                                //    {
                                //        cmd2.CommandText = "PR_participant_results";
                                //        cmd2.CommandType = CommandType.StoredProcedure;
                                //        cmd2.Connection = connection;
                                //        // cmd2.Parameters.Add("@ID", 0);
                                //        cmd2.Parameters.Add("@EVENTID", api.data.@event.id);
                                //        cmd2.Parameters.Add("@PARTICIPANTID", api.data.@event.participants[i].id);
                                //        cmd2.Parameters.Add("@PROGRESS_412", api.data.@event.participants[i].results.FirstOrDefault(c => c.id == 412).value);
                                //        cmd2.Parameters.Add("@WINNER_411", api.data.@event.participants[i].results.FirstOrDefault(c => c.id == 411).value);
                                //        cmd2.Parameters.Add("@RESULT_2", api.data.@event.participants[i].results.FirstOrDefault(c => c.id == 2).value);
                                //        cmd2.Parameters.Add("@RT_3", api.data.@event.participants[i].results.FirstOrDefault(c => c.id == 3).value);
                                //        cmd2.Parameters.Add("@FH_4", api.data.@event.participants[i].results.FirstOrDefault(c => c.id == 4).value);
                                //        cmd2.Parameters.Add("@SH_5", api.data.@event.participants[i].results.FirstOrDefault(c => c.id == 5).value);
                                //        cmd2.Parameters.Add("@E1H_133", api.data.@event.participants[i].results.FirstOrDefault(c => c.id == 133).value);
                                //        cmd2.Parameters.Add("@E2H_134", api.data.@event.participants[i].results.FirstOrDefault(c => c.id == 134).value);
                                //        cmd2.Parameters.Add("@PENALTY_7", api.data.@event.participants[i].results.FirstOrDefault(c => c.id == 7).value);
                                //        cmd2.Parameters.Add("@OVERTIME_104", api.data.@event.participants[i].results.FirstOrDefault(c => c.id == 104).value);
                                //        cmd2.Parameters.Add("@CTIMESTAMP", DateTime.Now);
                                //        cmd2.Parameters.Add("@CACTION", api.data.@event.action);
                                //        cmd2.Parameters.Add("@TEAMTYPE", api.data.@event.participants[i].counter == 1 ? "H" : "G");
                                //        id = Convert.ToInt32(cmd2.ExecuteScalar());
                                //        Files.WriteLog((id > 0 ? " [Success] Insert participant_results" : " [Failure] Insert participant_results") + "[" + api.data.@event.id + "/" + api.data.@event.participants[0].id + "].");
                                //    }

                                //    using (FbCommand cmd2 = new FbCommand())
                                //    {
                                //        cmd2.CommandText = "PR_participant_stats";
                                //        cmd2.CommandType = CommandType.StoredProcedure;
                                //        cmd2.Connection = connection;
                                //        // cmd2.Parameters.Add("@ID", 0);
                                //        cmd2.Parameters.Add("@EVENTID", api.data.@event.id);
                                //        cmd2.Parameters.Add("@PARTICIPANTID", api.data.@event.participants[i].id);
                                //        cmd2.Parameters.Add("@SOT_20", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 20).value);
                                //        cmd2.Parameters.Add("@SOT_21", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 21).value);
                                //        cmd2.Parameters.Add("@ATTACKS_10", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 10).value);
                                //        cmd2.Parameters.Add("@DA_11", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 11).value);
                                //        cmd2.Parameters.Add("@CORNERS_13", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 13).value);
                                //        cmd2.Parameters.Add("@YELLOW_CARDS_8", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 8).value);
                                //        cmd2.Parameters.Add("@RED_CARDS_9", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 9).value);
                                //        cmd2.Parameters.Add("@TOTAL_SHOTS_19", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 19).value);
                                //        cmd2.Parameters.Add("@FOULS_22", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 22).value);
                                //        cmd2.Parameters.Add("@OFFSIDES_24", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 24).value);
                                //        cmd2.Parameters.Add("@PS_14", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 14).value);
                                //        cmd2.Parameters.Add("@PM_15", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 15).value);
                                //        cmd2.Parameters.Add("@PG_16", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 16).value);
                                //        cmd2.Parameters.Add("@FK_25", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 25).value);
                                //        cmd2.Parameters.Add("@DFK_26", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 26).value);
                                //        cmd2.Parameters.Add("@FKG_18", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 18).value);
                                //        cmd2.Parameters.Add("@SW_27", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 27).value);
                                //        cmd2.Parameters.Add("@SB_28", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 28).value);
                                //        cmd2.Parameters.Add("@GS_29", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 29).value);
                                //        cmd2.Parameters.Add("@GK_30", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 30).value);
                                //        cmd2.Parameters.Add("@TI_32", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 32).value);
                                //        cmd2.Parameters.Add("@SUBSTITUTIONS_31", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 31).value);
                                //        cmd2.Parameters.Add("@GOALS_40", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 40).value);
                                //        cmd2.Parameters.Add("@MP_34", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 34).value);
                                //        cmd2.Parameters.Add("@OWN_GOALS_17", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 17).value);
                                //        cmd2.Parameters.Add("@ADW_33", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 33).value);
                                //        cmd2.Parameters.Add("@FORM_716", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 716).value);
                                //        cmd2.Parameters.Add("@SKIN_718", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 718).value);
                                //        cmd2.Parameters.Add("@PS_639", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 639).value);
                                //        cmd2.Parameters.Add("@PU_697", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 697).value);
                                //        cmd2.Parameters.Add("@GOALS115_772", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 772).value);
                                //        cmd2.Parameters.Add("@GOALS1630_773", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 773).value);
                                //        cmd2.Parameters.Add("@GOALS3145_774", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 774).value);
                                //        cmd2.Parameters.Add("@GOALS4660_775", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 775).value);
                                //        cmd2.Parameters.Add("@GOALS6175_776", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 776).value);
                                //        cmd2.Parameters.Add("@GOALS7690_777", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 777).value);
                                //        cmd2.Parameters.Add("@MPG_778", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 778).value);
                                //        cmd2.Parameters.Add("@MPS_779", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 779).value);
                                //        cmd2.Parameters.Add("@CTIMESTAMP", DateTime.Now);
                                //        cmd2.Parameters.Add("@CACTION", api.data.@event.action);
                                //        cmd2.Parameters.Add("@TEAMTYPE", api.data.@event.participants[i].counter == 2 ? "G" : "H");
                                //        id = Convert.ToInt32(cmd2.ExecuteScalar());
                                //        Files.WriteLog((id > 0 ? " [Success] Insert participant_stats" : " [Failure] Insert participant_results") + "[" + api.data.@event.id + "/" + api.data.@event.participants[0].id + "].");
                                //        //  Thread.Sleep(20);
                                //    }
                                //}
                                //if (api.data.@event.participants.Length == 2 && api.data.@event.action != "delete")
                                //{

                                //    using (FbCommand cmd2 = new FbCommand())
                                //    {
                                //        cmd2.CommandText = "PR_Stats_Result_GoalInfo";
                                //        cmd2.CommandType = CommandType.StoredProcedure;
                                //        cmd2.Connection = connection;
                                //        cmd2.Parameters.Add("@EMATCHID", api.data.@event.id);
                                //        cmd2.Parameters.Add("@H_YELLOW", api.data.@event.participants[0].stats.FirstOrDefault(c => c.id == 8).value);
                                //        cmd2.Parameters.Add("@H_RED", api.data.@event.participants[0].stats.FirstOrDefault(c => c.id == 9).value);
                                //        cmd2.Parameters.Add("@G_YELLOW", api.data.@event.participants[1].stats.FirstOrDefault(c => c.id == 8).value);
                                //        cmd2.Parameters.Add("@G_RED", api.data.@event.participants[1].stats.FirstOrDefault(c => c.id == 9).value);
                                //        cmd2.Parameters.Add("@H_GOAL", api.data.@event.participants[0].results.FirstOrDefault(c => c.id == 2).value);
                                //        cmd2.Parameters.Add("@G_GOAL", api.data.@event.participants[1].results.FirstOrDefault(c => c.id == 2).value);
                                //        cmd2.Parameters.Add("@STATUSID", api.data.@event.status_id);
                                //        cmd2.Parameters.Add("@LASTTIME", DateTime.Now);
                                //        id = Convert.ToInt32(cmd2.ExecuteScalar());
                                //        if (id > -1) Files.WriteLog(" [Success] Update GoalInfo " + "[" + api.data.@event.id + "/" + api.data.@event.participants[0].id + "]" + id+".");
                                //    }
                                //    if (id > 0) { SendAlertMsg(AppFlag.LIVEGOALS); }

                                //    //if (!backgroundWorker.IsBusy)
                                //    ////if (!backgroundWorker.IsBusy && id > 0)
                                //    //{
                                //    //    backgroundWorker.RunWorkerAsync(api.data.@event.name + " " + strName + ".json");
                                //    //}

                                //    if (id > 0)
                                //    {
                                //        this.lstStatus.Invoke(new Action(() => { { this.lstStatus.Items.Insert(0, DateTime.Now.ToString("HH:mm:ss.fff   ") + api.data.@event.name + " " + strName + ".json"); } }));
                                //    }
                                //}
                                //}
                                //} 

                            }
                            else if (api != null && api.type == "incident")
                            {
                                int sID = -1;
                                DOSIncidentJson.IncidentJson incidentJson = JsonUtil.Deserialize(typeof(DOSIncidentJson.IncidentJson), message) as DOSIncidentJson.IncidentJson;
                                iCount2++;
                                if (incidentJson != null && incidentJson.data.@event.sport_id == 5 && incidentJson.data.incident.important_for_trader == "yes")
                                {
                                    //20190121 RUN DELETE ACTION BY TIGGER
                                    ////if (incidentJson.data.incident.action != "delete")
                                    ////{
                                    strName = "Incid_" + incidentJson.data.@event.id + "_" + incidentJson.data.incident.event_status_id + "_" + incidentJson.data.incident.incident_id + "-" + incidentJson.data.incident.id + "-" + DateTime.Now.ToString("HHmmssfff");
                                    Files.WriteJson(strName, message);
                                    DateTime time = DateTime.Now;
                                    using (FbCommand cmd2 = new FbCommand())
                                    {
                                        cmd2.CommandText = "PR_INCIDENTS";
                                        cmd2.CommandType = CommandType.StoredProcedure;
                                        cmd2.Connection = connection;
                                        cmd2.Parameters.Add("@ID", incidentJson.data.incident.id);
                                        cmd2.Parameters.Add("@EVENTID", incidentJson.data.@event.id);
                                        cmd2.Parameters.Add("@CACTION", incidentJson.data.incident.action);
                                        cmd2.Parameters.Add("@INCIDENT_ID", incidentJson.data.incident.incident_id);
                                        cmd2.Parameters.Add("@INCIDENT_NAME", incidentJson.data.incident.incident_name);
                                        cmd2.Parameters.Add("@PARTICIPANT_ID", incidentJson.data.incident.participant_id);
                                        cmd2.Parameters.Add("@PARTICIPANT_NAME", incidentJson.data.incident.participant_name);
                                        cmd2.Parameters.Add("@SUBPARTICIPANT_ID", incidentJson.data.incident.subparticipant_id);
                                        cmd2.Parameters.Add("@SUBPARTICIPANT_NAME", incidentJson.data.incident.subparticipant_name);
                                        cmd2.Parameters.Add("@IMPORTANT_FOR_TRADER", true);// incidentJson.data.incident.important_for_trader == "yes" ? true : false);
                                        cmd2.Parameters.Add("@EVENT_TIME", incidentJson.data.incident.event_time);
                                        cmd2.Parameters.Add("@EVENT_STATUS_ID", incidentJson.data.incident.event_status_id);
                                        cmd2.Parameters.Add("@EVENT_STATUS_NAME", incidentJson.data.incident.event_status_name);
                                        cmd2.Parameters.Add("@UT", incidentJson.ut);
                                        cmd2.Parameters.Add("@CTIMESTAMP", DateTime.Now);
                                        cmd2.Parameters.Add("@TEAMTYPE",
                                        (incidentJson.data.incident.participant_id == null || incidentJson.data.incident.participant_id.ToString() == "" ? "" : incidentJson.data.incident.participant_id.ToString() == incidentJson.data.@event.participants[0].id.ToString() ? "H" : incidentJson.data.incident.participant_id.ToString() == incidentJson.data.@event.participants[1].id.ToString() ? "G" : "H"));
                                        sID = Convert.ToInt32(cmd2.ExecuteScalar());
                                        Files.WriteLog((sID > -1 ? " [Success] Insert INCIDENTS " : " [Failure] Insert INCIDENTS ") + "[" + incidentJson.data.@event.id + "]," + strName + ".json");

                                        //  if (sID > 0) { SendAlertMsg(AppFlag.GOALDETAILS); }

                                        // if (!backgroundWorker.IsBusy)
                                        //// if (!backgroundWorker.IsBusy && sID > 0)
                                        // {
                                        //     backgroundWorker.RunWorkerAsync(incidentJson.data.@event.name + " " + strName + ".json");
                                        // }
                                        if (sID > 0)
                                        {
                                            this.lstStatus.Invoke(new Action(() =>
                                            {
                                                {
                                                    this.lstStatus.Items.Insert(0, DateTime.Now.ToString("HH:mm:ss.fff   ") + incidentJson.data.@event.name + " " + strName + ".json");
                                                }
                                            }));
                                        }
                                    }

                                    if (incidentJson.data.incident.incident_id == 429)// || incidentJson.data.@event.status_id == 9 || incidentJson.data.@event.status_id == 11)
                                    {
                                        if (incidentJson.data.@event.id > 0)
                                        {
                                            //  await AyncHandleData("events.show3", true, incidentJson.data.@event.id.ToString());
                                            InsertData("events.show3", true, incidentJson.data.@event.id.ToString(), true);
                                            Files.WriteLog(" Housekeep incident [" + incidentJson.data.@event.id.ToString() + "].");
                                            if (incidentJson.data.incident.incident_id == 429)
                                            {
                                                InsertData("events.participants", true, incidentJson.data.@event.id.ToString());
                                                Files.WriteLog(" Housekeep event participant [" + incidentJson.data.@event.id.ToString() + "]." + incidentJson.data.incident.incident_id);

                                            }
                                        }
                                    }

                                    //////402 Dangerous attack 
                                    ////if (incidentJson.data.incident.incident_id == 413 || incidentJson.data.incident.incident_id == 418 || incidentJson.data.incident.incident_id == 419 ||
                                    ////  (incidentJson.data.incident.subparticipant_name != null
                                    ////  && (incidentJson.data.incident.incident_id == 408
                                    ////  || incidentJson.data.incident.incident_id == 404// || incidentJson.data.incident.incident_id == 402
                                    ////  || incidentJson.data.incident.incident_id == 414 || incidentJson.data.incident.incident_id == 415)))
                                    ////{
                                    ////    string sName = "";
                                    ////    using (FbCommand cmd2 = new FbCommand("SELECT  r.EMATCHID , r.HKJCDAYCODE, r.HKJCMATCHNO FROM EMATCHES r where r.EMATCHID='" + incidentJson.data.@event.id + "'", connection))
                                    ////    {
                                    ////        FbDataReader reader = cmd2.ExecuteReader();
                                    ////        while (reader.Read())
                                    ////        {
                                    ////            var data = (IDataRecord)reader;
                                    ////            sName = data[1] + "-" + data[2] + "-" + data[0];
                                    ////        }
                                    ////        reader.Close();
                                    ////    }
                                    ////    string sData = time + "    " + incidentJson.data.incident.event_time + "  " + (incidentJson.data.incident.subparticipant_id == null ? "" : incidentJson.data.incident.subparticipant_id.ToString()) + "  " + (incidentJson.data.incident.subparticipant_name == null ? "" : incidentJson.data.incident.subparticipant_name) + "  " + incidentJson.data.incident.incident_name;

                                    ////    Files.WriteLog(sName, false, sData);
                                    ////}

                                    if (sID > 0 && (incidentJson.data.incident.incident_id == 413 || incidentJson.data.incident.incident_id == 418 ||
                                        incidentJson.data.incident.incident_id == 419 || incidentJson.data.incident.incident_id == 421 || incidentJson.data.incident.incident_id == 429 ||
                                         incidentJson.data.incident.incident_id == 404// || incidentJson.data.incident.incident_id == 402
                                         || incidentJson.data.incident.incident_id == 414 || incidentJson.data.incident.incident_id == 415 ||
                                         (429 < incidentJson.data.@event.status_id && incidentJson.data.@event.status_id < 452)))
                                    { SendAlertMsg(AppFlag.GOALDETAILS); }
                                }
                                //    else
                                //    {
                                //        //delete action

                                //    }
                                //}
                                else if (incidentJson != null && incidentJson.data.@event.sport_id == 5 && incidentJson.data.incident.important_for_trader == "no")
                                {
                                    if (!AppFlag.Important_for_trader)
                                    {
                                        strName = "Incid_" + incidentJson.data.@event.id + "_" + incidentJson.data.@event.id + "_" + DateTime.Now.ToString("HHmmssfff") + "_no";
                                        Files.WriteJson(strName, message);
                                    }
                                }
                                else
                                {
                                    strName = "Incid_" + incidentJson.data.@event.id + "_" + incidentJson.data.@event.id + "_" + DateTime.Now.ToString("HHmmssfff") + "_other";
                                    Files.WriteJson(strName, message);
                                }

                            }
                            else if (api != null && api.type == "event_keep_alive")
                            {
                                Files.WriteLog("event_keep_alive");
                            }
                            else if (api != null && api.data.@event.sport_id != 5)
                            {
                            }
                            else
                            {
                                strName = "other2_" + DateTime.Now.ToString("HHmmssfff");
                                Files.WriteJson(strName, message);
                            }
                            if (bBreak)
                            {
                                // Files.WriteLog("Stop AMQPService");
                                // break;
                            }
                            Thread.Sleep(TimeSpan.FromMilliseconds(2));
                            ///    return true;
                        }
                        catch (EndOfStreamException endOfStreamException)
                        {
                            Files.WriteLog("No message." + (strName != "" ? strName + ".json," : ""));
                            Files.WriteError("InitialListener()," + (strName != "" ? strName + ".json," : "") + "StreamException: " + endOfStreamException);
                            connection.Close();
                            bBreak = true;
                            // e.Result = "No message,break."; 
                            return false;
                        }
                        catch (Exception exp)
                        {
                            if (strName != "") Files.WriteLog("Error: " + strName + ".json");
                            Files.WriteError("InitialListener(), " + (strName != "" ? strName + ".json," : "") + "Exception: " + exp);
                            bBreak = true;
                            //  e.Result = "No AMQPService.";
                            //  continue;
                            return false;
                        }
                        finally
                        {
                            connection.Close();
                        }
                        //  connection.Close();
                    }

                    return true;
                };

                mq._mqActionLogFunc = (msg) =>
                {
                    this.lstStatus.Invoke(new Action(() =>
                    {
                        this.lstStatus.Invoke(new Action(() => { { this.lstStatus.Items.Insert(0, msg); } }));
                    }));
                };
                mq.ConnectionFactoryParam = connectionFactoryParam;

                this.Receiver.InitialReceive(mq);
            }
            catch (Exception exp)
            {
                Files.WriteError("InitialListener(),error: " + exp.Message);
                bBreak = true;
            }
        }

        public static bool SendAlertMsg(int sBroadcast)
        {
            bool SUCCESS_CODE = false;
            int iResultCode = 0;
            try
            {
                IntPtr handle = PostMessage(HWND_BROADCAST, uiSkSvrNotify, new Random().Next(), sBroadcast);
                iResultCode = (int)handle;

                if (iResultCode != 0)
                {
                    SUCCESS_CODE = true;
                    Files.WriteLog(" [Success] Send " + sBroadcast.ToString());
                }
                else
                {
                    iResultCode = GetLastError();
                    throw (new Exception("GetLastError()"));
                }
            }
            catch (Exception ex)
            {
                SUCCESS_CODE = false;
                Files.WriteError("SendAlertMsg(),error:" + ex.ToString());
            }
            return SUCCESS_CODE;
        }

        private Task<bool> SyncHkjcAndBook(DataSet ds)
        {
            return Task.Run(() =>
            {
                bool result = true;
                try
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        using (FbConnection connection = new FbConnection(AppFlag.ScoutsDBConn))
                        {
                            foreach (DataRow dr1 in ds.Tables[0].Rows)
                            {
                                string strHkjcHostName = dr1["CHOME_TEAM_ENG_NAME"].ToString();
                                string strHkjcGeustName = dr1["CAWAY_TEAM_ENG_NAME"].ToString();

                                connection.Open();
                                DataSet evtTeams = new DataSet();
                                string queryString = "SELECT distinct t.id,t.name FROM teams t where t.NAME='" + strHkjcHostName + "' OR T.NAME='" + strHkjcGeustName + "'";
                                using (FbCommand cmd = new FbCommand(queryString, connection))
                                {
                                    using (FbDataAdapter fda = new FbDataAdapter())
                                    {
                                        fda.SelectCommand = cmd;
                                        using (DataSet data = new DataSet())
                                        {
                                            data.Tables.Add(new DataTable("eventTeams"));
                                            fda.Fill(data.Tables["eventTeams"]);
                                            evtTeams = data;
                                        }
                                    }
                                }
                                if (evtTeams.Tables[0].Rows.Count >= 2)
                                {
                                    string id1 = evtTeams.Tables[0].Select("NAME='" + strHkjcHostName + "'")[0]["ID"].ToString();
                                    string id2 = evtTeams.Tables[0].Select("NAME='" + strHkjcGeustName + "'")[0]["ID"].ToString();

                                    using (FbCommand cmd2 = new FbCommand())
                                    {
                                        cmd2.CommandText = "Sync_HkjcData";
                                        cmd2.CommandType = CommandType.StoredProcedure;
                                        cmd2.Connection = connection;
                                        cmd2.Parameters.Add("@HOME_ID", id1);
                                        cmd2.Parameters.Add("@GUEST_ID", id2);
                                        cmd2.Parameters.Add("@HKJCHOSTNAME", strHkjcHostName);
                                        cmd2.Parameters.Add("@HKJCGUESTNAME", strHkjcGeustName);
                                        int id = Convert.ToInt32(cmd2.ExecuteScalar());
                                        Files.WriteLog((id > 0 ? " [Success] " : (id == -2) ? " [Failure] event not exist " : " [Failure] ") + "Sync [" + id + "] EMATCHES[" + dr1["IMATCH_NO"] + " " + dr1["CMATCH_DAY_CODE"] + "] " + " " + strHkjcHostName + "/" + strHkjcGeustName);
                                    }
                                }
                                connection.Close();
                            }
                        }
                    }

                }
#pragma warning disable CS0168 // The variable 'exp' is declared but never used
                catch (Exception exp)
#pragma warning restore CS0168 // The variable 'exp' is declared but never used
                {
                    result = false;
                }
                return result;
            }
                );
        }

        private async void RunGetEventCompare(string eventID)
        {
            // Files.WriteLog("GetEventCompare1 " + eventID);
            bool results = await GetEventCompare(eventID);

        }


        private Task<bool> GetEventCompare(string id)
        {
            return Task.Run(() =>
            {
                bool result = true;
                try
                {
                    //  Files.WriteLog("GetEventCompare3 " + id);
                    InsertData("events.compare", false, id);
                }
                catch (Exception exp)
                {
                    Files.WriteError("GetEventCompare " + id + " error:" + exp.Message);
                    result = false;
                }
                return result;
            }
                );
        }

        private async void RunSyncHkjcAndBook(DataSet ds)
        {
            bool results = await SyncHkjcAndBook(ds);
            //this.lbResults.Text = "AMQP:" + results;
        }
        private bool RunSyncHkjcAndBook3(DataTable table)
        {
            bool result = true;
            try
            {
                if (table.Rows.Count > 0)
                {
                    using (FbConnection connection = new FbConnection(AppFlag.ScoutsDBConn))
                    {
                        connection.Open();
                        foreach (DataRow dr1 in table.Rows)
                        {
                            try
                            {
                                int id = dr1["EMATCHID"] == DBNull.Value ? -1 : Convert.ToInt32(dr1["EMATCHID"]);
                                if (id == -1 || id == 0)
                                {
                                    string strHkjcHostName = dr1["HKJCHOSTNAME"].ToString();
                                    string strHkjcGeustName = dr1["HKJCGUESTNAME"].ToString();
                                    string dayCODE = dr1["HKJCDAYCODE"].ToString();
                                    string matchNo = dr1["HKJCMATCHNO"].ToString();
                                    string alias = dr1["CLEAGUEALIAS_OUTPUT_NAME"].ToString();
                                    DateTime dMATCHDATETIME = Convert.ToDateTime(dr1["CMATCHDATETIME"]);
                                    try
                                    {
                                        using (FbCommand cmd2 = new FbCommand())
                                        {//maybe return booked or no
                                            cmd2.CommandText = "Sync_HkjcData_Auto_byName";
                                            cmd2.CommandType = CommandType.StoredProcedure;
                                            cmd2.Connection = connection;
                                            //cmd2.Parameters.Add("@HOME_ID", id1);
                                            //cmd2.Parameters.Add("@GUEST_ID", id2);
                                            cmd2.Parameters.Add("@HKJCDAYCODE", dayCODE);
                                            cmd2.Parameters.Add("@HKJCMATCHNO", matchNo);
                                            cmd2.Parameters.Add("@HKJCHOSTNAME", strHkjcHostName);
                                            cmd2.Parameters.Add("@HKJCGUESTNAME", strHkjcGeustName);
                                            cmd2.Parameters.Add("@CMATCHDATETIME1", dMATCHDATETIME.AddHours(-AppFlag.MarginOfDeviation));
                                            cmd2.Parameters.Add("@CMATCHDATETIME2", dMATCHDATETIME.AddHours(AppFlag.MarginOfDeviation));
                                            cmd2.Parameters.Add("@ALIAS", alias);
                                            id = Convert.ToInt32(cmd2.ExecuteScalar());
                                            Files.WriteLog((id > 0 ? " [Success] " : (id == 0) ? " [Failure] event not exist " : " [Failure] ") + "Sync [" + id + "] EMATCHES[" + dr1["HKJCMATCHNO"] + " " + dr1["HKJCDAYCODE"] + "] " + " " + strHkjcHostName + "/" + strHkjcGeustName);
                                        }
                                        if (id > 0)
                                        {
                                            if (AppFlag.GetEventShow) InsertData("events.show3", true, id, true);
                                            if (!AppFlag.GetEventShow) RunGetEventCompare(id.ToString());
                                            Files.WriteLog("GetEventCompare " + id);
                                        }
                                    }
                                    catch (Exception exp)
                                    {
                                        Files.WriteError("[Failure] Sync [" + id + "] EMATCHES[" + dr1["HKJCMATCHNO"] + " " + dr1["HKJCDAYCODE"] + "] " + " " + strHkjcHostName + "/" + strHkjcGeustName + ",error:" + exp.ToString());
                                        continue;
                                    }
                                    // string strHkjcHostName = dr1["HKJCHOSTNAME"].ToString();
                                    // string strHkjcGeustName = dr1["HKJCGUESTNAME"].ToString();
                                    // string dayCODE = dr1["HKJCDAYCODE"].ToString();
                                    // string matchNo = dr1["HKJCMATCHNO"].ToString();
                                    // DateTime dMATCHDATETIME = Convert.ToDateTime(dr1["CMATCHDATETIME"]);
                                    // //string strHkjcHostName = dr1[35].ToString();
                                    // //string strHkjcGeustName = dr1[37].ToString();

                                    // //if (strHkjcHostName == "Valenciennes" || strHkjcGeustName == "Valenciennes") 
                                    // //{
                                    // //    string str = "";
                                    // //}
                                    // //connection.Open();
                                    // DataSet evtTeams = new DataSet();
                                    // //  string queryString = "SELECT distinct t.id,t.name FROM teams t where t.NAME='" + strHkjcHostName + "' OR T.NAME='" + strHkjcGeustName + "'";
                                    // string queryString = "SELECT t.id,t.name,t.SHORT_NAME,t.HKJC_NAME FROM teams t where "
                                    // ///+ "t.HKJC_NAME='" + strHkjcHostName + "' OR T.HKJC_NAME='" + strHkjcGeustName + "' "  
                                    // //+ "or t.SHORT_NAME='" + strHkjcHostName + "' OR T.SHORT_NAME='" + strHkjcGeustName + "' "
                                    // // + "or t.NAME='" + strHkjcHostName + "' OR T.NAME='" + strHkjcGeustName + "' ";
                                    //+ " t.SHORT_NAME='" + strHkjcHostName + "' OR T.SHORT_NAME='" + strHkjcGeustName + "' "
                                    //+ " or t.NAME = '" + strHkjcHostName + "' OR T.NAME = '" + strHkjcGeustName + "' "
                                    //+ " or (" + (AppFlag.AutoMapping ? "" : "t. MAPPING_STATUS=true and ") + "( t.HKJC_NAME='" + strHkjcHostName + "' OR T.HKJC_NAME='" + strHkjcGeustName + "')) ";
                                    // if (AppFlag.TestMode) Files.WriteTestLog("Test", "HKjcMatch2 " + queryString);
                                    // using (FbCommand cmd = new FbCommand(queryString, connection))
                                    // {
                                    //     using (FbDataAdapter fda = new FbDataAdapter())
                                    //     {
                                    //         fda.SelectCommand = cmd;
                                    //         using (DataSet data = new DataSet())
                                    //         {
                                    //             data.Tables.Add(new DataTable("eventTeams"));
                                    //             fda.Fill(data.Tables["eventTeams"]);
                                    //             evtTeams = data;
                                    //         }
                                    //     }
                                    // }

                                    // if (AppFlag.TestMode) Files.WriteTestLog("Test", "HKjcMatch2 " + evtTeams.Tables[0].Rows.Count);

                                    // if ((evtTeams.Tables[0].Rows.Count == 0))
                                    // {
                                    //     Files.WriteLogNR("");
                                    //     Files.WriteLog("Team not exist on scoutsfeed " + strHkjcHostName + "/" + strHkjcGeustName);
                                    // }
                                    // else if ((evtTeams.Tables[0].Rows.Count == 1))
                                    // {
                                    //     Files.WriteLogNR("");
                                    //     Files.WriteLog("Team only one(" + evtTeams.Tables[0].Rows[0]["Name"].ToString() + ") exist on scoutsfeed " + strHkjcHostName + "/" + strHkjcGeustName);
                                    // }
                                    // else if (evtTeams.Tables[0].Rows.Count == 2)
                                    // {
                                    //     Files.WriteLogNR("");
                                    //     // int id = -1;
                                    //     string id1 = evtTeams.Tables[0].Select("NAME='" + strHkjcHostName + "' or  SHORT_NAME='" + strHkjcHostName + "' or  HKJC_NAME='" + strHkjcHostName + "'")[0]["ID"].ToString();
                                    //     string id2 = evtTeams.Tables[0].Select("NAME='" + strHkjcGeustName + "' or  SHORT_NAME='" + strHkjcGeustName + "' or  HKJC_NAME='" + strHkjcGeustName + "'")[0]["ID"].ToString();
                                    //     try
                                    //     {
                                    //         using (FbCommand cmd2 = new FbCommand())
                                    //         {//maybe return booked or no
                                    //             cmd2.CommandText = "Sync_HkjcData_Auto";
                                    //             cmd2.CommandType = CommandType.StoredProcedure;
                                    //             cmd2.Connection = connection;
                                    //             cmd2.Parameters.Add("@HOME_ID", id1);
                                    //             cmd2.Parameters.Add("@GUEST_ID", id2);
                                    //             cmd2.Parameters.Add("@HKJCDAYCODE", dayCODE);
                                    //             cmd2.Parameters.Add("@HKJCMATCHNO", matchNo);
                                    //             cmd2.Parameters.Add("@HKJCHOSTNAME", strHkjcHostName);
                                    //             cmd2.Parameters.Add("@HKJCGUESTNAME", strHkjcGeustName);
                                    //             cmd2.Parameters.Add("@CMATCHDATETIME1", dMATCHDATETIME.AddHours(-AppFlag.MarginOfDeviation));
                                    //             cmd2.Parameters.Add("@CMATCHDATETIME2", dMATCHDATETIME.AddHours(AppFlag.MarginOfDeviation));
                                    //             id = Convert.ToInt32(cmd2.ExecuteScalar());
                                    //             Files.WriteLog((id > 0 ? " [Success] " : (id == 0) ? " [Failure] event not exist " : " [Failure] ") + "Sync [" + id + "] EMATCHES[" + dr1["HKJCMATCHNO"] + " " + dr1["HKJCDAYCODE"] + "] " + " " + strHkjcHostName + "/" + strHkjcGeustName);
                                    //         }
                                    //     }
                                    //     catch (Exception exp)
                                    //     {
                                    //         Files.WriteError("[Failure] Sync [" + id + "] EMATCHES[" + dr1["HKJCMATCHNO"] + " " + dr1["HKJCDAYCODE"] + "] " + " " + strHkjcHostName + "/" + strHkjcGeustName + ",error:" + exp.ToString());
                                    //         continue;
                                    //     }
                                    //     //  if (id > 0) BookEventAction(id.ToString(), false);
                                    // }
                                    // else if (evtTeams.Tables[0].Rows.Count > 2)
                                    // {
                                    //     Files.WriteLogNR("");
                                    //     Files.WriteLog("Team more than two exist on scoutsfeed " + strHkjcHostName + "/" + strHkjcGeustName);
                                    //     Files.WriteLog(queryString);
                                    // }
                                }
                                // if (id > 0) BookEventAction(id.ToString(), dr1["NAME"].ToString(), dr1["START_DATE"].ToString(), dr1["HKJCHOSTNAME"].ToString (), dr1["HKJCGUESTNAME"].ToString(), dr1["HKJCDAYCODE"].ToString(), dr1["HKJCMATCHNO"].ToString(), dr1["MAPPINGSTATUS"].ToString(), false);
                                ///20190213 20190218 no book
                                ///  if (id > 0 &&AppFlag.AutoBooked) BookEventAction(id.ToString(), dr1["NAME"].ToString(), dr1["CMATCHDATETIME"].ToString(), dr1["HKJCHOSTNAME"].ToString(), dr1["HKJCGUESTNAME"].ToString(), dr1["HKJCDAYCODE"].ToString(), dr1["HKJCMATCHNO"].ToString(), false);
                                //Files.WriteLog("AutoGetPlayer"); 
                                //RunGetEventCompare(id.ToString());

                                if (AppFlag.IntervalGetPlayer > 0 && id > 0)
                                {
                                    AutoGetPlayer(id.ToString(), dr1["NAME"].ToString(), dr1["CMATCHDATETIME"].ToString(), dr1["HKJCHOSTNAME"].ToString(), dr1["HKJCGUESTNAME"].ToString(), dr1["HKJCDAYCODE"].ToString(), dr1["HKJCMATCHNO"].ToString(), false);
                                    //  Files.WriteLog("AutoGetPlayer " + id);
                                }
                            }
                            catch (Exception exp)
                            {
                                Files.WriteError("RunSyncHkjcAndBook3(), foreach " + dr1["EMATCHID"] + "-" + dr1["HKJCHOSTNAME"] + "/" + dr1["HKJCGUESTNAME"] + " error:" + exp);
                                continue;
                            }
                        }
                        connection.Close();
                    }
                }
            }
            catch (Exception exp)
            {
                Files.WriteError("RunSyncHkjcAndBook3(),error:" + exp.ToString());
                result = false;
            }
            return result;
        }

        private bool RunSyncHkjcAndBook2(DataTable table)
        {
            bool result = true;
            try
            {
                if (table.Rows.Count > 0)
                {
                    using (FbConnection connection = new FbConnection(AppFlag.ScoutsDBConn))
                    {
                        connection.Open();
                        foreach (DataRow dr1 in table.Rows)
                        {
                            string strHkjcHostName = dr1["CHOME_TEAM_ENG_NAME"].ToString();
                            string strHkjcGeustName = dr1["CAWAY_TEAM_ENG_NAME"].ToString();
                            DateTime dMATCHDATETIME = Convert.ToDateTime(dr1["CMATCHDATETIME"]);
                            //string strHkjcHostName = dr1[35].ToString();
                            //string strHkjcGeustName = dr1[37].ToString();

                            //if (strHkjcHostName == "Valenciennes" || strHkjcGeustName == "Valenciennes") 
                            //{
                            //    string str = "";
                            //}
                            //connection.Open();
                            DataSet evtTeams = new DataSet();
                            //  string queryString = "SELECT distinct t.id,t.name FROM teams t where t.NAME='" + strHkjcHostName + "' OR T.NAME='" + strHkjcGeustName + "'";
                            string queryString = "SELECT   t.id,t.name,t.SHORT_NAME FROM teams t where t.NAME='" + strHkjcHostName + "' OR T.NAME='" + strHkjcGeustName + "' or  t.SHORT_NAME='" + strHkjcHostName + "' OR T.SHORT_NAME='" + strHkjcGeustName + "'";
                            using (FbCommand cmd = new FbCommand(queryString, connection))
                            {
                                using (FbDataAdapter fda = new FbDataAdapter())
                                {
                                    fda.SelectCommand = cmd;
                                    using (DataSet data = new DataSet())
                                    {
                                        data.Tables.Add(new DataTable("eventTeams"));
                                        fda.Fill(data.Tables["eventTeams"]);
                                        evtTeams = data;
                                    }
                                }
                            }

                            if ((evtTeams.Tables[0].Rows.Count == 0))
                            {
                                Files.WriteLogNR("");
                                Files.WriteLog("Team not exist on scoutsfeed " + strHkjcHostName + "/" + strHkjcGeustName);
                            }
                            else if ((evtTeams.Tables[0].Rows.Count == 1))
                            {
                                Files.WriteLogNR("");
                                Files.WriteLog("Team only one(" + evtTeams.Tables[0].Rows[0]["Name"].ToString() + ") exist on scoutsfeed " + strHkjcHostName + "/" + strHkjcGeustName);
                            }
                            else if (evtTeams.Tables[0].Rows.Count == 2)
                            {
                                Files.WriteLogNR("");
                                int id = -1;
                                string id1 = evtTeams.Tables[0].Select("NAME='" + strHkjcHostName + "' or  SHORT_NAME='" + strHkjcHostName + "'")[0]["ID"].ToString();
                                string id2 = evtTeams.Tables[0].Select("NAME='" + strHkjcGeustName + "' or  SHORT_NAME='" + strHkjcGeustName + "'")[0]["ID"].ToString();
                                try
                                {
                                    using (FbCommand cmd2 = new FbCommand())
                                    {//maybe return booked or no
                                        cmd2.CommandText = "Sync_HkjcData_Auto";
                                        cmd2.CommandType = CommandType.StoredProcedure;
                                        cmd2.Connection = connection;
                                        cmd2.Parameters.Add("@HOME_ID", id1);
                                        cmd2.Parameters.Add("@GUEST_ID", id2);
                                        cmd2.Parameters.Add("@HKJCHOSTNAME", strHkjcHostName);
                                        cmd2.Parameters.Add("@HKJCGUESTNAME", strHkjcGeustName);
                                        cmd2.Parameters.Add("@CMATCHDATETIME1", dMATCHDATETIME.AddHours(-AppFlag.MarginOfDeviation));
                                        cmd2.Parameters.Add("@CMATCHDATETIME2", dMATCHDATETIME.AddHours(AppFlag.MarginOfDeviation));
                                        id = Convert.ToInt32(cmd2.ExecuteScalar());
                                        Files.WriteLog((id > 0 ? " [Success] " : (id == 0) ? " [Failure] event not exist " : " [Failure] ") + "Sync [" + id + "] EMATCHES[" + dr1["IMATCH_NO"] + " " + dr1["CMATCH_DAY_CODE"] + "] " + " " + strHkjcHostName + "/" + strHkjcGeustName);
                                    }
                                }
                                catch (Exception exp)
                                {
                                    Files.WriteError("error:" + exp.ToString());
                                }
                                ///no need book when app -bookedevent clicked
                                ///  if (id > 0) BookEventAction(id.ToString(), false);
                            }
                            // connection.Close();
                        }
                        connection.Close();
                    }
                }
            }
            catch (Exception exp)
            {
                Files.WriteError("RunSyncHkjcAndBook2(),error:" + exp.ToString());
                result = false;
            }
            return result;
        }

        private Task<DataSet> AyncHandleData(string type, params object[] arr)
        {
            return Task.Run(() =>
            {
                DataSet result = InsertData(type, arr);
                return result;
            }
                );
        }

        private Task<bool> AyncHandleMsg(string message)
        {
            return Task.Run(() =>
            {
                bool result = false;
                string strName = "";
                try
                {
                    using (FbConnection connection = new FbConnection(AppFlag.ScoutsDBConn))
                    {
                        connection.Open();
                        DOSEventJson.EventJson api = JsonUtil.Deserialize(typeof(DOSEventJson.EventJson), message) as DOSEventJson.EventJson;
                        if (api != null && api.type == "event" && (AppFlag.JsonType ? api.data.@event.sport_id == 5 : api.data.@event.sport_id != -1))
                        {
                            int id = -2;
                            strName = api.data.@event.id + "_" + DateTime.Now.ToString("HHmmssfff");
                            Files.WriteJson(strName, message);
                            //using (FbConnection connection = new FbConnection(AppFlag.ScoutsDBConn))
                            //{
                            //    connection.Open();
                            using (FbCommand cmd2 = new FbCommand())
                            {
                                cmd2.CommandText = "PR_JSON_event";
                                cmd2.CommandType = CommandType.StoredProcedure;
                                cmd2.Connection = connection;
                                cmd2.Parameters.Add("@ID", api.data.@event.id);
                                cmd2.Parameters.Add("@NAME", api.data.@event.name);
                                cmd2.Parameters.Add("@HOME_ID", api.data.@event.participants.Count() > 0 ? api.data.@event.participants[0].id : -1);
                                cmd2.Parameters.Add("@GUEST_ID", api.data.@event.participants.Count() > 0 ? api.data.@event.participants[1].id : -1);
                                // cmd2.Parameters.Add("@SOURCE", api.data.@evenT.S);
                                // cmd2.Parameters.Add("@SOURCE_DC", api.data.@event.S);
                                //cmd2.Parameters.Add("@SOURCE_SUPER", api.data.@event.s);
                                cmd2.Parameters.Add("@RELATION_STATUS", api.data.@event.relation_status);
                                cmd2.Parameters.Add("@START_DATE", Convert.ToDateTime(api.data.@event.start_date).AddHours(8));
                                cmd2.Parameters.Add("@FT_ONLY", api.data.@event.ft_only == "yes" ? true : false);
                                cmd2.Parameters.Add("@COVERAGE_TYPE", api.data.@event.coverage_type);
                                //cmd2.Parameters.Add("@CHANNEL_ID", api.data.@event.CH);
                                //cmd2.Parameters.Add("@CHANNEL_NAME", api.data.@event.C);
                                cmd2.Parameters.Add("@SCOUTSFEED", api.data.@event.scoutsfeed == "yes" ? true : false);
                                cmd2.Parameters.Add("@STATUS_ID", api.data.@event.status_id);
                                //cmd2.Parameters.Add("@STATUS_NAME", api.data.@event.STA);
                                cmd2.Parameters.Add("@STATUS_TYPE", api.data.@event.status_type);
                                cmd2.Parameters.Add("@CDAY", api.data.@event.day);
                                cmd2.Parameters.Add("@CLOCK_TIME", api.data.@event.clock_time);
                                cmd2.Parameters.Add("@CLOCK_STATUS", api.data.@event.clock_status);
                                // cmd2.Parameters.Add("@WINNER_ID", api.data.@event.W);
                                //cmd2.Parameters.Add("@PROGRESS_ID", api.data.@event.PR);
                                cmd2.Parameters.Add("@BET_STATUS", api.data.@event.bet_status);
                                cmd2.Parameters.Add("@NEUTRAL_VENUE", api.data.@event.neutral_venue == "yes" ? true : false);
                                cmd2.Parameters.Add("@ITEM_STATUS", api.data.@event.item_status);
                                // cmd2.Parameters.Add("@UT", api.data.@event.U);
                                // cmd2.Parameters.Add("@OLD_EVENT_ID", api.data.@event.OL);
                                // cmd2.Parameters.Add("@SLUG", api.data.@event.S);
                                // cmd2.Parameters.Add("@VERIFIED_RESULT", api.data.@event.VE);
                                // cmd2.Parameters.Add("@IS_PROTOCOL_VERIFIED", api.data.@event.IS);
                                //  cmd2.Parameters.Add("@PROTOCOL_VERIFIED_BY", api.data.@event.PRO);
                                //cmd2.Parameters.Add("@PROTOCOL_VERIFIED_AT", api.data.@event.PRO);
                                cmd2.Parameters.Add("@ROUND_ID", api.data.@event.round_id);
                                cmd2.Parameters.Add("@ROUND_NAME", api.data.@event.round_name);
                                //cmd2.Parameters.Add("@CLIENT_EVENT_ID", api.data.@event.C);
                                // cmd2.Parameters.Add("@BOOKED", null);
                                // cmd2.Parameters.Add("@BOOKED_BY", api.data.@event.);
                                // cmd2.Parameters.Add("@INVERTED_PARTICIPANTS", api.data.@event.iv);
                                cmd2.Parameters.Add("@VENUE_ID", api.data.@event.tour_id == null ? "-1" : api.data.@event.tour_id);
                                //  cmd2.Parameters.Add("@GROUP_ID", api.data.@event.gr);
                                cmd2.Parameters.Add("@STAGE_ID", api.data.@event.stage_id);
                                cmd2.Parameters.Add("@SEASON_ID", api.data.@event.season_id);
                                cmd2.Parameters.Add("@COMPETITION_ID", api.data.@event.competition_id);
                                cmd2.Parameters.Add("@AREA_ID", api.data.@event.area_id);
                                cmd2.Parameters.Add("@CTIMESTAMP", DateTime.Now);
                                cmd2.Parameters.Add("@CACTION", api.data.@event.action);
                                id = Convert.ToInt32(cmd2.ExecuteScalar());
                                Files.WriteLog((id == 0 ? " [Success] Insert event " : id == 1 ? " Update event " : "") + "[" + api.data.@event.id + "]," + strName + ".json");
                                // Files.WriteLog((id == 0 ? " [Success] Insert event [" + api.data.@event.id + "]," + strName + ".json":"");
                            }


                            if (api.data.@event.details.Count() > 0)
                            {
                                using (FbCommand cmd2 = new FbCommand())
                                {
                                    cmd2.CommandText = "PR_event_details";
                                    cmd2.CommandType = CommandType.StoredProcedure;
                                    cmd2.Connection = connection;
                                    cmd2.Parameters.Add("@EVENTID", api.data.@event.id);
                                    cmd2.Parameters.Add("@WC_8", api.data.@event.details.FirstOrDefault(c => c.id == 8).value);
                                    cmd2.Parameters.Add("@PC_36", api.data.@event.details.FirstOrDefault(c => c.id == 36).value);
                                    cmd2.Parameters.Add("@PL_16", api.data.@event.details.FirstOrDefault(c => c.id == 16).value);
                                    cmd2.Parameters.Add("@EPL_50", api.data.@event.details.FirstOrDefault(c => c.id == 50).value);
                                    cmd2.Parameters.Add("@NOP_17", api.data.@event.details.FirstOrDefault(c => c.id == 17).value);
                                    cmd2.Parameters.Add("@EPTC_58", api.data.@event.details.FirstOrDefault(c => c.id == 58).value);
                                    cmd2.Parameters.Add("@IT_151", api.data.@event.details.FirstOrDefault(c => c.id == 151).value);
                                    cmd2.Parameters.Add("@ATT_141", api.data.@event.details.FirstOrDefault(c => c.id == 141).value);
                                    cmd2.Parameters.Add("@FHSD_19", api.data.@event.details.FirstOrDefault(c => c.id == 19).value == null ? DateTime.MinValue : Convert.ToDateTime(api.data.@event.details.FirstOrDefault(c => c.id == 19).value).AddHours(8));
                                    cmd2.Parameters.Add("@SHSD_20", api.data.@event.details.FirstOrDefault(c => c.id == 20).value == null ? DateTime.MinValue : Convert.ToDateTime(api.data.@event.details.FirstOrDefault(c => c.id == 20).value).AddHours(8));
                                    cmd2.Parameters.Add("@FEHSD_44", api.data.@event.details.FirstOrDefault(c => c.id == 44).value);
                                    cmd2.Parameters.Add("@SEHSD_45", api.data.@event.details.FirstOrDefault(c => c.id == 45).value);
                                    cmd2.Parameters.Add("@PSSD_150", api.data.@event.details.FirstOrDefault(c => c.id == 150).value);
                                    cmd2.Parameters.Add("@FHIT_201", api.data.@event.details.FirstOrDefault(c => c.id == 201).value);
                                    cmd2.Parameters.Add("@SHIT_202", api.data.@event.details.FirstOrDefault(c => c.id == 202).value);
                                    cmd2.Parameters.Add("@FEHIT_203", api.data.@event.details.FirstOrDefault(c => c.id == 203).value);
                                    cmd2.Parameters.Add("@SEHIT_204", api.data.@event.details.FirstOrDefault(c => c.id == 204).value);
                                    cmd2.Parameters.Add("@HL_205", api.data.@event.details.FirstOrDefault(c => c.id == 205).value);
                                    cmd2.Parameters.Add("@TD_124", api.data.@event.details.FirstOrDefault(c => c.id == 124).value);
                                    cmd2.Parameters.Add("@BM_160", api.data.@event.details.FirstOrDefault(c => c.id == 160).value);
                                    cmd2.Parameters.Add("@HF_178", api.data.@event.details.FirstOrDefault(c => c.id == 178).value);
                                    cmd2.Parameters.Add("@UT", api.ut);
                                    cmd2.Parameters.Add("@CTIMESTAMP", DateTime.Now);
                                    id = Convert.ToInt32(cmd2.ExecuteScalar());
                                    Files.WriteLog((id > 0 ? " [Success] Insert event_details " : id == -1 ? " Old data " : " [Failure] Insert event_details ") + "[" + api.data.@event.id + "]," + strName + ".json");
                                }

                                if (id != -1)
                                {
                                    for (int i = 0; i < api.data.@event.participants.Length; i++)
                                    {
                                        using (FbCommand cmd2 = new FbCommand())
                                        {
                                            cmd2.CommandText = "PR_participant_results";
                                            cmd2.CommandType = CommandType.StoredProcedure;
                                            cmd2.Connection = connection;
                                            // cmd2.Parameters.Add("@ID", 0);
                                            cmd2.Parameters.Add("@EVENTID", api.data.@event.id);
                                            cmd2.Parameters.Add("@PARTICIPANTID", api.data.@event.participants[i].id);
                                            cmd2.Parameters.Add("@PROGRESS_412", api.data.@event.participants[i].results.FirstOrDefault(c => c.id == 412).value);
                                            cmd2.Parameters.Add("@WINNER_411", api.data.@event.participants[i].results.FirstOrDefault(c => c.id == 411).value);
                                            cmd2.Parameters.Add("@RESULT_2", api.data.@event.participants[i].results.FirstOrDefault(c => c.id == 2).value);
                                            cmd2.Parameters.Add("@RT_3", api.data.@event.participants[i].results.FirstOrDefault(c => c.id == 3).value);
                                            cmd2.Parameters.Add("@FH_4", api.data.@event.participants[i].results.FirstOrDefault(c => c.id == 4).value);
                                            cmd2.Parameters.Add("@SH_5", api.data.@event.participants[i].results.FirstOrDefault(c => c.id == 5).value);
                                            cmd2.Parameters.Add("@E1H_133", api.data.@event.participants[i].results.FirstOrDefault(c => c.id == 133).value);
                                            cmd2.Parameters.Add("@E2H_134", api.data.@event.participants[i].results.FirstOrDefault(c => c.id == 134).value);
                                            cmd2.Parameters.Add("@PENALTY_7", api.data.@event.participants[i].results.FirstOrDefault(c => c.id == 7).value);
                                            cmd2.Parameters.Add("@OVERTIME_104", api.data.@event.participants[i].results.FirstOrDefault(c => c.id == 104).value);
                                            cmd2.Parameters.Add("@CTIMESTAMP", DateTime.Now);
                                            cmd2.Parameters.Add("@CACTION", api.data.@event.action);
                                            cmd2.Parameters.Add("@TEAMTYPE", api.data.@event.participants[i].counter == 1 ? "H" : "G");
                                            id = Convert.ToInt32(cmd2.ExecuteScalar());
                                            Files.WriteLog((id > 0 ? " [Success] Insert participant_results" : " [Failure] Insert participant_results") + "[" + api.data.@event.id + "/" + api.data.@event.participants[0].id + "]");
                                        }

                                        using (FbCommand cmd2 = new FbCommand())
                                        {
                                            cmd2.CommandText = "PR_participant_stats";
                                            cmd2.CommandType = CommandType.StoredProcedure;
                                            cmd2.Connection = connection;
                                            // cmd2.Parameters.Add("@ID", 0);
                                            cmd2.Parameters.Add("@EVENTID", api.data.@event.id);
                                            cmd2.Parameters.Add("@PARTICIPANTID", api.data.@event.participants[i].id);
                                            cmd2.Parameters.Add("@SOT_20", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 20).value);
                                            cmd2.Parameters.Add("@SOT_21", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 21).value);
                                            cmd2.Parameters.Add("@ATTACKS_10", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 10).value);
                                            cmd2.Parameters.Add("@DA_11", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 11).value);
                                            cmd2.Parameters.Add("@CORNERS_13", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 13).value);
                                            cmd2.Parameters.Add("@YELLOW_CARDS_8", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 8).value);
                                            cmd2.Parameters.Add("@RED_CARDS_9", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 9).value);
                                            cmd2.Parameters.Add("@TOTAL_SHOTS_19", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 19).value);
                                            cmd2.Parameters.Add("@FOULS_22", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 22).value);
                                            cmd2.Parameters.Add("@OFFSIDES_24", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 24).value);
                                            cmd2.Parameters.Add("@PS_14", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 14).value);
                                            cmd2.Parameters.Add("@PM_15", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 15).value);
                                            cmd2.Parameters.Add("@PG_16", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 16).value);
                                            cmd2.Parameters.Add("@FK_25", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 25).value);
                                            cmd2.Parameters.Add("@DFK_26", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 26).value);
                                            cmd2.Parameters.Add("@FKG_18", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 18).value);
                                            cmd2.Parameters.Add("@SW_27", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 27).value);
                                            cmd2.Parameters.Add("@SB_28", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 28).value);
                                            cmd2.Parameters.Add("@GS_29", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 29).value);
                                            cmd2.Parameters.Add("@GK_30", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 30).value);
                                            cmd2.Parameters.Add("@TI_32", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 32).value);
                                            cmd2.Parameters.Add("@SUBSTITUTIONS_31", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 31).value);
                                            cmd2.Parameters.Add("@GOALS_40", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 40).value);
                                            cmd2.Parameters.Add("@MP_34", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 34).value);
                                            cmd2.Parameters.Add("@OWN_GOALS_17", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 17).value);
                                            cmd2.Parameters.Add("@ADW_33", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 33).value);
                                            cmd2.Parameters.Add("@FORM_716", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 716).value);
                                            cmd2.Parameters.Add("@SKIN_718", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 718).value);
                                            cmd2.Parameters.Add("@PS_639", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 639).value);
                                            cmd2.Parameters.Add("@PU_697", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 697).value);
                                            cmd2.Parameters.Add("@GOALS115_772", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 772).value);
                                            cmd2.Parameters.Add("@GOALS1630_773", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 773).value);
                                            cmd2.Parameters.Add("@GOALS3145_774", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 774).value);
                                            cmd2.Parameters.Add("@GOALS4660_775", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 775).value);
                                            cmd2.Parameters.Add("@GOALS6175_776", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 776).value);
                                            cmd2.Parameters.Add("@GOALS7690_777", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 777).value);
                                            cmd2.Parameters.Add("@MPG_778", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 778).value);
                                            cmd2.Parameters.Add("@MPS_779", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 779).value);
                                            cmd2.Parameters.Add("@CTIMESTAMP", DateTime.Now);
                                            cmd2.Parameters.Add("@CACTION", api.data.@event.action);
                                            cmd2.Parameters.Add("@TEAMTYPE", api.data.@event.participants[i].counter == 2 ? "G" : "H");
                                            id = Convert.ToInt32(cmd2.ExecuteScalar());
                                            Files.WriteLog((id > 0 ? " [Success] Insert participant_stats" : " [Failure] Insert participant_results") + "[" + api.data.@event.id + "/" + api.data.@event.participants[0].id + "]");
                                            //  Thread.Sleep(20);
                                        }

                                        //using (FbCommand cmd2 = new FbCommand())
                                        //{

                                        //    cmd2.CommandText = "PR_stats_GoalInfo";
                                        //    cmd2.CommandType = CommandType.StoredProcedure;
                                        //    cmd2.Connection = connection; 
                                        //    cmd2.Parameters.Add("@EMATCHID", api.data.@event.id); 
                                        //    cmd2.Parameters.Add("@YELLOW_CARDS_8", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 8).value);
                                        //    cmd2.Parameters.Add("@RED_CARDS_9", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 9).value);
                                        //    cmd2.Parameters.Add("@CTIMESTAMP", DateTime.Now);
                                        //    id = Convert.ToInt32(cmd2.ExecuteScalar());
                                        //    Files.WriteLog((id > 0 ? " [Success] Update GoalInfo " : " [Failure]  Update GoalInfo ") + "[" + api.data.@event.id + "/" + api.data.@event.participants[0].id + "]");
                                        //    Thread.Sleep(20);
                                        //}
                                    }
                                    if (api.data.@event.participants.Length == 2 && api.data.@event.action != "delete")
                                    {
                                        using (FbCommand cmd2 = new FbCommand())
                                        {
                                            cmd2.CommandText = "PR_stats_GoalInfo";
                                            cmd2.CommandType = CommandType.StoredProcedure;
                                            cmd2.Connection = connection;
                                            cmd2.Parameters.Add("@EMATCHID", api.data.@event.id);
                                            cmd2.Parameters.Add("@H_YELLOW", api.data.@event.participants[0].stats.FirstOrDefault(c => c.id == 8).value);
                                            cmd2.Parameters.Add("@H_RED", api.data.@event.participants[0].stats.FirstOrDefault(c => c.id == 9).value);
                                            cmd2.Parameters.Add("@G_YELLOW", api.data.@event.participants[1].stats.FirstOrDefault(c => c.id == 8).value);
                                            cmd2.Parameters.Add("@G_RED", api.data.@event.participants[1].stats.FirstOrDefault(c => c.id == 9).value);
                                            cmd2.Parameters.Add("@LASTTIME", DateTime.Now);
                                            id = Convert.ToInt32(cmd2.ExecuteScalar());
                                            ////  Files.WriteLog((id > 0 ? " [Success] Update GoalInfo " : " [Failure]  Update GoalInfo ") + "[" + api.data.@event.id + "/" + api.data.@event.participants[0].id + "]");
                                            Files.WriteLog((id > 0 ? " [Success] Update GoalInfo " : " [Failure]  Update GoalInfo ") + "[" + api.data.@event.id + "] " + api.data.@event.participants[0].id + "/" + api.data.@event.participants[1].id);
                                        }

                                        using (FbCommand cmd2 = new FbCommand())
                                        {
                                            cmd2.CommandText = "PR_Result_GoalInfo";
                                            cmd2.CommandType = CommandType.StoredProcedure;
                                            cmd2.Connection = connection;
                                            cmd2.Parameters.Add("@EMATCHID", api.data.@event.id);
                                            cmd2.Parameters.Add("@H_GOAL", api.data.@event.participants[0].results.FirstOrDefault(c => c.id == 2).value);
                                            cmd2.Parameters.Add("@G_GOAL", api.data.@event.participants[1].results.FirstOrDefault(c => c.id == 2).value);
                                            cmd2.Parameters.Add("@LASTTIME", DateTime.Now);
                                            id = Convert.ToInt32(cmd2.ExecuteScalar());
                                            ////  Files.WriteLog((id > 0 ? " [Success] Update GoalInfo Goal " : " [Failure]  Update GoalInfo Goal") + "[" + api.data.@event.id + "/" + api.data.@event.participants[0].id + "]");
                                            Files.WriteLog((id > 0 ? " [Success] Update GoalInfo Goal " : " [Failure]  Update GoalInfo Goal") + "[" + api.data.@event.id + "] " + api.data.@event.participants[0].id + "/" + api.data.@event.participants[1].id);
                                        }

                                    }
                                }
                            }
                            if (!backgroundWorker.IsBusy)
                            {
                                backgroundWorker.RunWorkerAsync("[" + api.data.@event.id + "] " + strName + ".json");
                            }
                        }
                        else if (api != null && api.type == "incident")
                        {
                            string sID = "";
                            DOSIncidentJson.IncidentJson incidentJson = JsonUtil.Deserialize(typeof(DOSIncidentJson.IncidentJson), message) as DOSIncidentJson.IncidentJson;
                            if (incidentJson != null && incidentJson.data.@event.sport_id == 5 && incidentJson.data.incident.important_for_trader == "yes")
                            {
                                strName = "Incid_" + incidentJson.data.@event.id + "_" + DateTime.Now.ToString("HHmmssfff");
                                Files.WriteJson(strName, message);
                                //if(incidentJson.data.incident.incident_id== 429)
                                //{ 
                                //}
                                //using (FbConnection connection = new FbConnection(AppFlag.ScoutsDBConn))
                                //{
                                //connection.Open();
                                using (FbCommand cmd2 = new FbCommand())
                                {
                                    cmd2.CommandText = "PR_INCIDENTS";
                                    cmd2.CommandType = CommandType.StoredProcedure;
                                    cmd2.Connection = connection;
                                    cmd2.Parameters.Add("@ID", incidentJson.data.incident.id);
                                    cmd2.Parameters.Add("@EVENTID", incidentJson.data.@event.id);
                                    cmd2.Parameters.Add("@CACTION", incidentJson.data.incident.action);
                                    cmd2.Parameters.Add("@INCIDENT_ID", incidentJson.data.incident.incident_id);
                                    cmd2.Parameters.Add("@INCIDENT_NAME", incidentJson.data.incident.incident_name);
                                    cmd2.Parameters.Add("@PARTICIPANT_ID", incidentJson.data.incident.participant_id);
                                    cmd2.Parameters.Add("@PARTICIPANT_NAME", incidentJson.data.incident.participant_name);
                                    cmd2.Parameters.Add("@SUBPARTICIPANT_ID", incidentJson.data.incident.subparticipant_id);
                                    cmd2.Parameters.Add("@SUBPARTICIPANT_NAME", incidentJson.data.incident.subparticipant_name);
                                    cmd2.Parameters.Add("@IMPORTANT_FOR_TRADER", true);// incidentJson.data.incident.important_for_trader == "yes" ? true : false);
                                    cmd2.Parameters.Add("@EVENT_TIME", incidentJson.data.incident.event_time);
                                    cmd2.Parameters.Add("@EVENT_STATUS_ID", incidentJson.data.incident.event_status_id);
                                    cmd2.Parameters.Add("@EVENT_STATUS_NAME", incidentJson.data.incident.event_status_name);
                                    cmd2.Parameters.Add("@UT", incidentJson.ut);
                                    cmd2.Parameters.Add("@CTIMESTAMP", DateTime.Now);
                                    cmd2.Parameters.Add("@TEAMTYPE",
                                    (incidentJson.data.incident.participant_id == null || incidentJson.data.incident.participant_id.ToString() == "" ? "" : incidentJson.data.incident.participant_id.ToString() == incidentJson.data.@event.participants[0].id.ToString() ? "H" : incidentJson.data.incident.participant_id.ToString() == incidentJson.data.@event.participants[1].id.ToString() ? "G" : "H"));
                                    sID = (cmd2.ExecuteScalar()).ToString(); sID = (cmd2.ExecuteScalar()).ToString();
                                    Files.WriteLog((sID != "" ? " [Success] Insert INCIDENTS " : " [Failure] Insert INCIDENTS ") + "[" + incidentJson.data.@event.id + "]," + strName + ".json");
                                }
                                //    connection.Close();
                                //}
                                if (!backgroundWorker.IsBusy)
                                {
                                    backgroundWorker.RunWorkerAsync("[" + incidentJson.data.@event.id + "] " + strName + ".json");
                                }
                            }
                            else if (incidentJson != null && incidentJson.data.@event.sport_id == 5 && incidentJson.data.incident.important_for_trader == "no")
                            {
                                strName = "Incid_" + incidentJson.data.@event.id + "_" + DateTime.Now.ToString("HHmmssfff") + "_no";
                                Files.WriteJson(strName, message);
                            }
                            else
                            {
                                strName = "Incid_" + incidentJson.data.@event.id + "_" + DateTime.Now.ToString("HHmmssfff") + "_other";
                                Files.WriteJson(strName, message);
                            }

                        }
                        else if (api != null && api.type == "event_keep_alive")
                        {
                            Files.WriteLog("event_keep_alive");
                        }
                        else if (api != null && api.data.@event.sport_id != 5)
                        {
                        }
                        else
                        {
                            strName = "other2_" + DateTime.Now.ToString("HHmmssfff");
                            Files.WriteJson(strName, message);
                        }
                        connection.Close();
                    }
                }
                catch (Exception exp)
                {
                    Files.WriteError("AyncHandleMsg(),error:" + (strName != "" ? strName + ".json," : "") + "error: " + exp.Message);
                }
                return result;
            }
                );
        }

        private async void RunAyncHandleData(string type, params object[] arr)
        {
            DataSet results = await AyncHandleData(type, arr);

        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //datatime picker booked min/max time, the others app.config QueryDays
            //if (tabControl1.SelectedTab != tpBook)
            //{
            //    var culture = new System.Globalization.CultureInfo("zh-HK");
            //    DateTime now = DateTime.Now;
            //    this.bnAreas.Items[17].Text = now.AddDays(-1).ToString("yyyy-MM-dd");
            //    this.bnAreas.Items[19].Text = now.AddDays(AppFlag.iQueryDays).ToString("yyyy-MM-dd");
            //}

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
                this.tsbGet_Click(null, null);
                //  MasterControl_RowHeaderMouseClick(null, null);
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
            else if (tabControl1.SelectedTab == tpBook)
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
                // MasterControl_RowHeaderMouseClick2(null, null);
                if (iExpand != -1)
                {
                    //this.dgvBookedEvent.childView.Visible = false;
                    //this.dgvBookedEvent.rowCurrent.Clear();
                    //this.dgvBookedEvent.Rows[iExpand].Height = Conversions.ToInteger(this.dgvBookedEvent.rowDefaultHeight);
                    //this.dgvBookedEvent.Rows[iExpand].DividerHeight = Conversions.ToInteger(this.dgvBookedEvent.rowDefaultDivider);
                }
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

        int iExpand = -1;
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
                this.lbResults.Text = strResponseValue;
                this.lstStatus.Items.Insert(0, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff   ") + lbAuthorization.Text);
                return;
            }
            strToken = JObject.Parse(data.ToString())["token"].Value<string>();
            this.lbAuthorization.Text = "Authorized";
            this.lbToken.Text = strToken;
            clientTest.token = strToken;
            this.lstStatus.Items.Insert(0, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff   ") + "Authorized " + strToken);
            Files.WriteLog("[Success] Authorized: " + strToken);

        }

        /*private DataSet InsertData2(string type, params object[] arr)
        {
            string queryString = "";
            DateTime cTimestamp = DateTime.Now;
            DataSet ds = new DataSet();
            switch (type)
            {
                case "events.show2":
                    {
                        using (FbConnection connection = new FbConnection(AppFlag.ScoutsDBConn))
                        {
                            connection.Open();
                            for (int i = 1; i < 2 && Convert.ToBoolean(arr[0]) == true; i++)
                            {
                                var responseValue = clientTest.GetAccessData(strToken, type, arr[1]);
                                var strResponseValue = responseValue.Result;
                                if (strResponseValue == "Unauthorized") { MessageBox.Show("Unauthorized!"); break; }
                                string strName = arr[1].ToString() + type + "-" + i + " " + DateTime.Now.ToString("HHmmss");

                                DOSEvents2.api apis = XmlUtil.Deserialize(typeof(DOSEvents2.api), strResponseValue) as DOSEvents2.api;
                                if (apis == null) break;

                                Files.WriteXml(strName, strResponseValue);
                                Files.WriteLog("Get " + strName + ".xml.");

                                DOSEvents2.apiDataCompetition competition = (apis.data.Length == 0) ? null : apis.data[0];
                                if (competition == null) break;

                                string strCompetition_id = competition.id;
                                string strArea_id = competition.area_id;

                                DOSEvents2.apiDataCompetitionSeason[] seasons = competition.seasons;
                                if (seasons == null) continue;
                                foreach (DOSEvents2.apiDataCompetitionSeason season in seasons)
                                {
                                    DOSEvents2.apiDataCompetitionSeasonStage[] stages = season.stages;
                                    if (stages == null) continue;

                                    foreach (DOSEvents2.apiDataCompetitionSeasonStage stage in stages)
                                    {
                                        DOSEvents2.apiDataCompetitionSeasonStageGroup[] groups = stage.groups;
                                        if (groups == null) continue;
                                        foreach (DOSEvents2.apiDataCompetitionSeasonStageGroup group in groups)
                                        {
                                            DOSEvents2.apiDataCompetitionSeasonStageGroupEvent[] events = group.events;
                                            if (events == null) continue;
                                            foreach (DOSEvents2.apiDataCompetitionSeasonStageGroupEvent sevent in events)
                                            {
                                                if (sevent == null) continue;

                                                foreach (DOSEvents2.apiDataCompetitionSeasonStageGroupEventParticipant participant in sevent.participants)
                                                {
                                                    if (participant == null) continue;
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
                                                        Files.WriteLog(id > 0 ? " [Success] Insert teams[" + participant.id + "] " + participant.name : "[" + participant.id + "] " + participant.name + " team exist.");
                                                    }
                                                    foreach (DOSEvents2.apiDataCompetitionSeasonStageGroupEventParticipantLineup lineup in participant.lineups)
                                                    {
                                                        if (lineup == null) continue;

                                                        using (FbCommand cmd2 = new FbCommand())
                                                        {
                                                            cmd2.CommandText = "ADD_Player2";
                                                            cmd2.CommandType = CommandType.StoredProcedure;
                                                            cmd2.Connection = connection;
                                                            cmd2.Parameters.Add("@ID", lineup.participant_id);
                                                            cmd2.Parameters.Add("@NAME", lineup.participant_name);
                                                            cmd2.Parameters.Add("@AREA_ID", lineup.participant_area_id);
                                                            cmd2.Parameters.Add("@SLUG", lineup.participant_slug);
                                                            cmd2.Parameters.Add("@BENCH", lineup.bench.ToLower() == "yes" ? true : false);
                                                            cmd2.Parameters.Add("@SHIRT_NR", lineup.shirt_nr == "" ? "-1" : lineup.shirt_nr);
                                                            cmd2.Parameters.Add("@TEAM_ID", participant.id);
                                                            cmd2.Parameters.Add("@SEASON_ID", season.id);
                                                            cmd2.Parameters.Add("@CTIMESTAMP", cTimestamp);
                                                            int id = Convert.ToInt32(cmd2.ExecuteScalar());
                                                            // Files.WriteLog(id > 0 ? " [Success] Insert players2 [" + lineup.participant_id+ "] " + lineup.participant_name + " " + strName + ".xml" : " player exist.");
                                                            Files.WriteLog(id > 0 ? " [Success] Insert players [" + lineup.participant_id + "] " + lineup.participant_name : "[" + lineup.participant_id + "] " + lineup.participant_name + " player exist.");
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            queryString = "SELECT t.* FROM teams t  inner  join  events e on   t.id= e.HOME_ID or t.id=e.GUEST_ID   where e.id='" + arr[1] + "' order by id asc";
                            using (FbCommand cmdA = new FbCommand(queryString))
                            {
                                using (FbCommandBuilder fcb = new FbCommandBuilder())
                                {
                                    using (FbDataAdapter fda = new FbDataAdapter())
                                    {
                                        cmdA.Connection = connection;
                                        fda.SelectCommand = cmdA;
                                        fcb.DataAdapter = fda;
                                        using (DataSet data = new DataSet())
                                        {
                                            data.Tables.Add(new DataTable("teams"));
                                            fda.Fill(data.Tables["teams"]);
                                            queryString = "SELECT  p.*   FROM players p inner join  events e on p.TEAM_ID = e.HOME_ID where e.id='" + arr[1] + "' order by p.id asc";
                                            data.Tables.Add(new DataTable("hplayers"));
                                            FbDataAdapter adapter1 = new FbDataAdapter();
                                            adapter1.SelectCommand = new FbCommand(queryString, connection);
                                            FbCommandBuilder builder2 = new FbCommandBuilder(adapter1);
                                            adapter1.Fill(data.Tables["hplayers"]);

                                            queryString = "SELECT  p.* FROM players p inner join  events e on  p.TEAM_ID = e.GUEST_ID where e.id='" + arr[1] + "' order by p.id asc";
                                            data.Tables.Add(new DataTable("gplayers"));
                                            FbDataAdapter adapter2 = new FbDataAdapter();
                                            adapter2.SelectCommand = new FbCommand(queryString, connection);
                                            FbCommandBuilder builder3 = new FbCommandBuilder(adapter1);
                                            adapter2.Fill(data.Tables["gplayers"]);

                                            ds = data;
                                        }
                                    }
                                }
                            }
                            connection.Close();
                        }
                        break;
                    }
                case "events.show":
                    {
                        using (FbConnection connection = new FbConnection(AppFlag.ScoutsDBConn))
                        {
                            connection.Open();
                            //for (int i = 1; i < 10 && Convert.ToBoolean(arr[0]) == true; i++)
                            //{
                            //    var responseValue = clientTest.GetAccessData(strToken, "participants/" + i, arr[0], arr[1]);
                            //    var strResponseValue = responseValue.Result;

                            //    if (strResponseValue == "Unauthorized") { MessageBox.Show("Unauthorized!"); break; }
                            //    // if (strResponseValue == "Unauthorized") { MessageBox.Show("Unauthorized!");   ds = data;break; }

                            //    DOSParticipants.api apis = XmlUtil.Deserialize(typeof(DOSParticipants.api), strResponseValue) as DOSParticipants.api;
                            //    if (apis == null) break;
                            //    DOSParticipants.apiDataParticipant[] participants = (apis.data.Length == 0) ? null : apis.data[0];
                            //    //  if (participants == null) return ds;
                            //    if (participants == null) break;
                            //    string strName = type + "-" + i + " " + DateTime.Now.ToString("HHmmss");
                            //    Files.WriteXml(strName, strResponseValue);

                            //    foreach (DOSParticipants.apiDataParticipant participant in participants)
                            //    {
                            //        if (participant.type == "team")
                            //        {
                            //            using (FbCommand cmd2 = new FbCommand())
                            //            {
                            //                cmd2.CommandText = "ADD_TEAM";
                            //                cmd2.CommandType = CommandType.StoredProcedure;
                            //                cmd2.Connection = connection;
                            //                cmd2.Parameters.Add("@ID", participant.id);
                            //                cmd2.Parameters.Add("@NAME", participant.name);
                            //                cmd2.Parameters.Add("@SHORT_NAME", participant.short_name);
                            //                cmd2.Parameters.Add("@ACRONYM", participant.acronym);
                            //                cmd2.Parameters.Add("@GENDER", (participant.gender.ToLower() == "male") ? true : false);
                            //                cmd2.Parameters.Add("@AREA_ID", participant.area_id);
                            //                cmd2.Parameters.Add("@BNATIONAL", (participant.national.ToLower() == "male") ? true : false);
                            //                cmd2.Parameters.Add("@UT", participant.ut);
                            //                cmd2.Parameters.Add("@OLD_PARTICIPANT_ID", participant.old_participant_id == "" ? "-1" : participant.old_participant_id);
                            //                cmd2.Parameters.Add("@SLUG", participant.slug);
                            //                cmd2.Parameters.Add("@SEASON_ID", "-1");
                            //                cmd2.Parameters.Add("@CTIMESTAMP", cTimestamp);
                            //                int id = Convert.ToInt32(cmd2.ExecuteScalar());
                            //                Files.WriteLog(id > 0 ? " [Success] Insert teams[" + count + "  " + "] " + " " + strName + ".xml" : " team exist.");
                            //            }
                            //        }
                            //        else if (participant.type == "person")
                            //        {
                            //            using (FbCommand cmd2 = new FbCommand())
                            //            {
                            //                cmd2.CommandText = "ADD_Player";
                            //                cmd2.CommandType = CommandType.StoredProcedure;
                            //                cmd2.Connection = connection;
                            //                cmd2.Parameters.Add("@ID", participant.id);
                            //                cmd2.Parameters.Add("@NAME", participant.name);
                            //                cmd2.Parameters.Add("@SHORT_NAME", participant.short_name);
                            //                cmd2.Parameters.Add("@ACRONYM", participant.acronym);
                            //                cmd2.Parameters.Add("@GENDER", (participant.gender.ToLower() == "male") ? true : false);
                            //                cmd2.Parameters.Add("@BIRTHDATE", participant.area_id);
                            //                cmd2.Parameters.Add("@POSITION_NAME", participant.area_id);
                            //                cmd2.Parameters.Add("@AREA_ID", participant.area_id);
                            //                cmd2.Parameters.Add("@BNATIONAL", (participant.national.ToLower() == "male") ? true : false);
                            //                cmd2.Parameters.Add("@UT", participant.ut);
                            //                cmd2.Parameters.Add("@OLD_PARTICIPANT_ID", participant.old_participant_id == "" ? "-1" : participant.old_participant_id);
                            //                cmd2.Parameters.Add("@SLUG", participant.slug);
                            //                cmd2.Parameters.Add("@TEAM_ID", arr[0]);
                            //                cmd2.Parameters.Add("@SEASON_ID", "-1");
                            //                cmd2.Parameters.Add("@CTIMESTAMP", cTimestamp);
                            //                int id = Convert.ToInt32(cmd2.ExecuteScalar());
                            //                Files.WriteLog(id > 0 ? " [Success] Insert players[" + count + "  " + "] " + " " + strName + ".xml" : " player exist.");
                            //            }
                            //        }
                            //    }
                            //}

                            for (int i = 1; i < 2 && Convert.ToBoolean(arr[0]) == true; i++)
                            {
                                //if (Convert.ToBoolean(arr[0]) == false) { ds = eventsDs; break; }
                                var responseValue = clientTest.GetAccessData(strToken, type, arr[1]);
                                var strResponseValue = responseValue.Result;
                                //XDocument document = XDocument.Load("E:\\Project\\AppProject\\DataOfScouts\\DataOfScouts\\bin\\Debug\\New folder\\events-1153436.xml");
                                //var strResponseValue = document.ToString(); 
                                if (strResponseValue == "Unauthorized") { MessageBox.Show("Unauthorized!"); break; }
                                string strName = arr[1].ToString() + type + "-" + i + " " + DateTime.Now.ToString("HHmmss");

                                DOSEvents2.api apis = XmlUtil.Deserialize(typeof(DOSEvents2.api), strResponseValue) as DOSEvents2.api;
                                if (apis == null) break;

                                Files.WriteXml(strName, strResponseValue);
                                Files.WriteLog("Get " + strName + ".xml.");

                                DOSEvents2.apiDataCompetition competition = (apis.data.Length == 0) ? null : apis.data[0];
                                if (competition == null) break;

                                //foreach (DOSEvents2.apiDataCompetition competition in competitions)
                                //{
                                string strCompetition_id = competition.id;
                                string strArea_id = competition.area_id;
                                //connection.Open();
                                //using (FbCommand cmd = new FbCommand())
                                //{
                                //    //                                        'ID', 'NAME', 'SHORT_NAME', 'MINI_NAME', 'GENDER',
                                //    //'CTYPE', 'AREA_ID', 'AREA_TYPE', 'AREA_SORT', 'OVERALL_SORT', 'SPORT_ID',
                                //    //'SPORT_NAME', 'TOUR_ID', 'TOUR_NAME', 'UT', 'OLD_COMPETITION_ID', 'SLUG', 'CTIMESTAMP'
                                //    cmd.CommandText = "ADD_COMPETITION";
                                //    cmd.CommandType = CommandType.StoredProcedure;
                                //    cmd.Connection = connection;
                                //    cmd.Parameters.Add("@ID", strCompetition_id);
                                //    cmd.Parameters.Add("@NAME", competition.name);
                                //    cmd.Parameters.Add("@SHORT_NAME", competition.short_name);
                                //    cmd.Parameters.Add("@MINI_NAME", competition.mini_name);
                                //    cmd.Parameters.Add("@GENDER", (competition.gender.ToLower() == "male") ? true : false);
                                //    cmd.Parameters.Add("@CTYPE", competition.type);
                                //    cmd.Parameters.Add("@AREA_ID", competition.area_id);
                                //    cmd.Parameters.Add("@AREA_TYPE", competition.type);
                                //    cmd.Parameters.Add("@AREA_SORT", competition.area_sort);
                                //    cmd.Parameters.Add("@OVERALL_SORT", competition.overall_sort);
                                //    cmd.Parameters.Add("@TOUR_ID", competition.tour_id == "" ? "-1" : competition.tour_id);
                                //    cmd.Parameters.Add("@TOUR_NAME", competition.tour_name);
                                //    cmd.Parameters.Add("@UT", competition.ut);
                                //    cmd.Parameters.Add("@OLD_COMPETITION_ID", competition.old_competition_id == "" ? "-1" : competition.old_competition_id);
                                //    cmd.Parameters.Add("@SLUG", competition.slug);
                                //    cmd.Parameters.Add("@CTIMESTAMP", cTimestamp);
                                //    int id = Convert.ToInt32(cmd.ExecuteScalar());
                                //    Files.WriteLog(id > 0 ? " [Success] Insert competition[" + competition.id + "] " + competition.name : "[" + competition.id + "] " + competition.name + " competition exist.");
                                //} 
                                DOSEvents2.apiDataCompetitionSeason[] seasons = competition.seasons;
                                if (seasons == null) continue;
                                foreach (DOSEvents2.apiDataCompetitionSeason season in seasons)
                                {
                                    DOSEvents2.apiDataCompetitionSeasonStage[] stages = season.stages;
                                    if (stages == null) continue;

                                    foreach (DOSEvents2.apiDataCompetitionSeasonStage stage in stages)
                                    {
                                        DOSEvents2.apiDataCompetitionSeasonStageGroup[] groups = stage.groups;
                                        if (groups == null) continue;
                                        foreach (DOSEvents2.apiDataCompetitionSeasonStageGroup group in groups)
                                        {
                                            DOSEvents2.apiDataCompetitionSeasonStageGroupEvent[] events = group.events;
                                            if (events == null) continue;
                                            foreach (DOSEvents2.apiDataCompetitionSeasonStageGroupEvent sevent in events)
                                            {
                                                if (sevent == null) continue;

                                                foreach (DOSEvents2.apiDataCompetitionSeasonStageGroupEventParticipant participant in sevent.participants)
                                                {
                                                    if (participant == null) continue;
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
                                                        //  Files.WriteLog(id > 0 ? " [Success] Insert teams[" + participant.id + "] " + participant.name + " " + strName + ".xml" : " team exist.");
                                                        Files.WriteLog(id > 0 ? " [Success] Insert teams[" + participant.id + "] " + participant.name : "[" + participant.id + "] " + participant.name + " team exist.");
                                                    }
                                                    foreach (DOSEvents2.apiDataCompetitionSeasonStageGroupEventParticipantLineup lineup in participant.lineups)
                                                    {
                                                        if (lineup == null) continue;

                                                        using (FbCommand cmd2 = new FbCommand())
                                                        {
                                                            cmd2.CommandText = "ADD_Player2";
                                                            cmd2.CommandType = CommandType.StoredProcedure;
                                                            cmd2.Connection = connection;
                                                            cmd2.Parameters.Add("@ID", lineup.participant_id == "" ? "-1" : lineup.participant_id);
                                                            cmd2.Parameters.Add("@NAME", lineup.participant_name);
                                                            cmd2.Parameters.Add("@AREA_ID", lineup.participant_area_id == "" ? "-1" : lineup.participant_area_id);
                                                            cmd2.Parameters.Add("@SLUG", lineup.participant_slug);
                                                            cmd2.Parameters.Add("@BENCH", lineup.bench.ToLower() == "yes" ? true : false);
                                                            cmd2.Parameters.Add("@SHIRT_NR", lineup.shirt_nr == "" ? "-1" : lineup.shirt_nr);
                                                            cmd2.Parameters.Add("@TEAM_ID", participant.id);
                                                            cmd2.Parameters.Add("@SEASON_ID", season.id);
                                                            cmd2.Parameters.Add("@CTIMESTAMP", cTimestamp);
                                                            int id = Convert.ToInt32(cmd2.ExecuteScalar());
                                                            // Files.WriteLog(id > 0 ? " [Success] Insert players2 [" + lineup.participant_id+ "] " + lineup.participant_name + " " + strName + ".xml" : " player exist.");
                                                            Files.WriteLog(id > 0 ? " [Success] Insert players [" + lineup.participant_id + "] " + lineup.participant_name : "[" + lineup.participant_id + "] " + lineup.participant_name + " player exist.");
                                                        }
                                                    }
                                                }
                                                // DataRow[] drs = eventsDs.Tables[0].Select("id=" + sevent.id);
                                                //{ 
                                                //}
                                                //else
                                                //{
                                                //    Files.WriteLog("[" + drs[0]["id"] + "]   events existed.");
                                                //}
                                            }
                                        }
                                    }
                                }

                            }

                            queryString = "SELECT t.* FROM teams t  inner  join  events e on   t.id= e.HOME_ID or t.id=e.GUEST_ID   where e.id='" + arr[1] + "' order by id asc";
                            //queryString = "SELECT t.ID ,t.NAME ,t.SHORT_NAME ,t.ACRONYM,t.season_id ,t.AREA_ID ,t.ctimestamp FROM teams t  inner  join  events e on   t.id= e.HOME_ID or t.id=e.GUEST_ID   where e.id='" + arr[1] + "' order by id asc";
                            using (FbCommand cmdA = new FbCommand(queryString))
                            {
                                using (FbCommandBuilder fcb = new FbCommandBuilder())
                                {
                                    using (FbDataAdapter fda = new FbDataAdapter())
                                    {
                                        cmdA.Connection = connection;
                                        fda.SelectCommand = cmdA;
                                        fcb.DataAdapter = fda;
                                        using (DataSet data = new DataSet())
                                        {
                                            data.Tables.Add(new DataTable("teams"));
                                            fda.Fill(data.Tables["teams"]);

                                            // queryString = "SELECT p.* FROM teams t  inner join  events e on t.id = e.HOME_ID or t.id = e.GUEST_ID    inner join players p on t.id = p.TEAM_ID   where e.id = '" + arr[1] + "' order by p.id asc";
                                            //queryString = "SELECT p.*FROM players p inner join  events e on p.TEAM_ID = e.HOME_ID or p.TEAM_ID = e.GUEST_ID where e.id='" + arr[1] + "' order by p.id asc";
                                            queryString = "SELECT  p.*   FROM players p inner join  events e on p.TEAM_ID = e.HOME_ID where e.id='" + arr[1] + "' order by p.id asc";
                                            data.Tables.Add(new DataTable("hplayers"));
                                            FbDataAdapter adapter1 = new FbDataAdapter();
                                            adapter1.SelectCommand = new FbCommand(queryString, connection);
                                            FbCommandBuilder builder2 = new FbCommandBuilder(adapter1);
                                            adapter1.Fill(data.Tables["hplayers"]);

                                            queryString = "SELECT  p.* FROM players p inner join  events e on  p.TEAM_ID = e.GUEST_ID where e.id='" + arr[1] + "' order by p.id asc";
                                            data.Tables.Add(new DataTable("gplayers"));
                                            FbDataAdapter adapter2 = new FbDataAdapter();
                                            adapter2.SelectCommand = new FbCommand(queryString, connection);
                                            FbCommandBuilder builder3 = new FbCommandBuilder(adapter1);
                                            adapter2.Fill(data.Tables["gplayers"]);

                                            //if ((Convert .ToBoolean (arr[0])==false) ||data.Tables["teams"].Rows.Count == 0 ||( data.Tables["hplayers"].Rows.Count == 0 && data.Tables["gplayers"].Rows.Count == 0))
                                            //{
                                            //for (int i = 1; i < 2; i++)
                                            //{
                                            //    //if (Convert.ToBoolean(arr[0]) == false) { ds = eventsDs; break; }

                                            //    var responseValue = clientTest.GetAccessData(strToken, "events/" + i, arr[0], arr[1]);
                                            //    var strResponseValue = responseValue.Result;
                                            //    //XDocument document = XDocument.Load("E:\\Project\\AppProject\\DataOfScouts\\DataOfScouts\\bin\\Debug\\New folder\\events-1153436.xml");
                                            //    //var strResponseValue = document.ToString();

                                            //    if (strResponseValue == "Unauthorized") { MessageBox.Show("Unauthorized!"); break; }
                                            //    string strName =arr[1].ToString ()+ type + "-" + i + " " + DateTime.Now.ToString("HHmmss");
                                            //    Files.WriteXml(strName, strResponseValue);

                                            //    DOSEvents2.api apis = XmlUtil.Deserialize(typeof(DOSEvents2.api), strResponseValue) as DOSEvents2.api;
                                            //    if (apis == null) break;
                                            //    DOSEvents2.apiDataCompetition  competition = (apis.data.Length == 0) ? null : apis.data[0];
                                            //    if (competition == null) break;

                                            //    //foreach (DOSEvents2.apiDataCompetition competition in competitions)
                                            //    //{
                                            //        string strCompetition_id = competition.id;
                                            //        string strArea_id = competition.area_id;
                                            //    connection.Open();
                                            //    using (FbCommand cmd = new FbCommand())
                                            //        {
                                            //            //                                        'ID', 'NAME', 'SHORT_NAME', 'MINI_NAME', 'GENDER',
                                            //            //'CTYPE', 'AREA_ID', 'AREA_TYPE', 'AREA_SORT', 'OVERALL_SORT', 'SPORT_ID',
                                            //            //'SPORT_NAME', 'TOUR_ID', 'TOUR_NAME', 'UT', 'OLD_COMPETITION_ID', 'SLUG', 'CTIMESTAMP'
                                            //            cmd.CommandText = "ADD_COMPETITION";
                                            //            cmd.CommandType = CommandType.StoredProcedure;
                                            //            cmd.Connection = connection;
                                            //            cmd.Parameters.Add("@ID", strCompetition_id);
                                            //            cmd.Parameters.Add("@NAME", competition.name);
                                            //            cmd.Parameters.Add("@SHORT_NAME", competition.short_name);
                                            //            cmd.Parameters.Add("@MINI_NAME", competition.mini_name);
                                            //            cmd.Parameters.Add("@GENDER", (competition.gender.ToLower() == "male") ? true : false);
                                            //            cmd.Parameters.Add("@CTYPE", competition.type);
                                            //            cmd.Parameters.Add("@AREA_ID", competition.area_id);
                                            //            cmd.Parameters.Add("@AREA_TYPE", competition.type);
                                            //            cmd.Parameters.Add("@AREA_SORT", competition.area_sort);
                                            //            cmd.Parameters.Add("@OVERALL_SORT", competition.overall_sort);
                                            //            cmd.Parameters.Add("@TOUR_ID", competition.tour_id == "" ? "-1" : competition.tour_id);
                                            //            cmd.Parameters.Add("@TOUR_NAME", competition.tour_name);
                                            //            cmd.Parameters.Add("@UT", competition.ut);
                                            //            cmd.Parameters.Add("@OLD_COMPETITION_ID", competition.old_competition_id == "" ? "-1" : competition.old_competition_id);
                                            //            cmd.Parameters.Add("@SLUG", competition.slug);
                                            //            cmd.Parameters.Add("@CTIMESTAMP", cTimestamp);
                                            //            int id = Convert.ToInt32(cmd.ExecuteScalar());
                                            //        }


                                            //        DOSEvents2.apiDataCompetitionSeason[] seasons = competition.seasons;
                                            //        if (seasons == null) continue;
                                            //        foreach (DOSEvents2.apiDataCompetitionSeason season in seasons)
                                            //        {
                                            //            DOSEvents2.apiDataCompetitionSeasonStage[] stages = season.stages;
                                            //            if (stages == null) continue;

                                            //            foreach (DOSEvents2.apiDataCompetitionSeasonStage stage in stages)
                                            //            {
                                            //                DOSEvents2.apiDataCompetitionSeasonStageGroup[] groups = stage.groups;
                                            //                if (groups == null) continue;
                                            //                foreach (DOSEvents2.apiDataCompetitionSeasonStageGroup group in groups)
                                            //                {
                                            //                    DOSEvents2.apiDataCompetitionSeasonStageGroupEvent[] events = group.events;
                                            //                    if (events == null) continue;
                                            //                    foreach (DOSEvents2.apiDataCompetitionSeasonStageGroupEvent sevent in events)
                                            //                    {
                                            //                        if (sevent == null) continue;

                                            //                        foreach (DOSEvents2.apiDataCompetitionSeasonStageGroupEventParticipant participant in sevent.participants )
                                            //                        {
                                            //                            if (participant == null) continue;
                                            //                        using (FbCommand cmd2 = new FbCommand())
                                            //                        {
                                            //                            cmd2.CommandText = "ADD_TEAM";
                                            //                            cmd2.CommandType = CommandType.StoredProcedure;
                                            //                            cmd2.Connection = connection;
                                            //                            cmd2.Parameters.Add("@ID", participant.id);
                                            //                            cmd2.Parameters.Add("@NAME", participant.name);
                                            //                            cmd2.Parameters.Add("@SHORT_NAME", participant.short_name);
                                            //                            cmd2.Parameters.Add("@ACRONYM", participant.acronym);
                                            //                            cmd2.Parameters.Add("@GENDER", (participant.gender.ToLower() == "male") ? true : false);
                                            //                            cmd2.Parameters.Add("@AREA_ID", participant.area_id);
                                            //                            cmd2.Parameters.Add("@BNATIONAL", (participant.national.ToLower() == "male") ? true : false);
                                            //                            cmd2.Parameters.Add("@UT", participant.ut);
                                            //                            cmd2.Parameters.Add("@OLD_PARTICIPANT_ID", participant.old_participant_id == "" ? "-1" : participant.old_participant_id);
                                            //                            cmd2.Parameters.Add("@SLUG", participant.slug);
                                            //                            cmd2.Parameters.Add("@SEASON_ID", "-1");
                                            //                            cmd2.Parameters.Add("@CTIMESTAMP", cTimestamp);
                                            //                            int id = Convert.ToInt32(cmd2.ExecuteScalar());
                                            //                            Files.WriteLog(id > 0 ? " [Success] Insert teams[" + count + "  " + "] " + " " + strName + ".xml" : " team exist.");
                                            //                        }
                                            //                        foreach (DOSEvents2.apiDataCompetitionSeasonStageGroupEventParticipantLineup lineup in participant.lineups)
                                            //                            {
                                            //                                if (lineup == null) continue;

                                            //                                using (FbCommand cmd2 = new FbCommand())
                                            //                                {
                                            //                                    cmd2.CommandText = "ADD_Player2";
                                            //                                    cmd2.CommandType = CommandType.StoredProcedure;
                                            //                                    cmd2.Connection = connection;
                                            //                                    cmd2.Parameters.Add("@ID", lineup.participant_id );
                                            //                                    cmd2.Parameters.Add("@NAME", lineup.participant_name);
                                            //                                    cmd2.Parameters.Add("@AREA_ID", lineup.participant_area_id);
                                            //                                    cmd2.Parameters.Add("@SLUG", lineup.participant_slug);
                                            //                                    cmd2.Parameters.Add("@BENCH", lineup.bench.ToLower ()=="yes"? true:false);
                                            //                                    cmd2.Parameters.Add("@SHIRT_NR", lineup.shirt_nr==""?"-1": lineup.shirt_nr); 
                                            //                                    cmd2.Parameters.Add("@TEAM_ID", participant.id);
                                            //                                    cmd2.Parameters.Add("@SEASON_ID", season.id);
                                            //                                    cmd2.Parameters.Add("@CTIMESTAMP", cTimestamp);
                                            //                                    int id = Convert.ToInt32(cmd2.ExecuteScalar());
                                            //                                    Files.WriteLog(id > 0 ? " [Success] Insert players2 [" + count + "  " + "] " + " " + strName + ".xml" : " player exist.");
                                            //                                }
                                            //                            }
                                            //                        }
                                            //                        // DataRow[] drs = eventsDs.Tables[0].Select("id=" + sevent.id);
                                            //                        //{ 
                                            //                        //}
                                            //                        //else
                                            //                        //{
                                            //                        //    Files.WriteLog("[" + drs[0]["id"] + "]   events existed.");
                                            //                        //}
                                            //                    }
                                            //                }
                                            //            }
                                            //        }
                                            //    }

                                            //count = adapter.Update(eventsDs);
                                            //ds.Merge(eventsDs, true, MissingSchemaAction.AddWithKey);
                                            //eventsDs.Clear();

                                            //if (count > -1)
                                            //{
                                            //    Files.WriteLog("[Success] Insert events[" + count + "  " + "] " + " " + strName + ".xml");
                                            //}
                                            //else
                                            //{
                                            //    Files.WriteLog("[Failure] Insert events [  ]" + " " + strName + ".xml");
                                            //}
                                            //    }
                                            ////}
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

                        //using (FbConnection connection = new FbConnection(AppFlag.ScoutsDBConn))
                        //{
                        //    //  queryString = "select * from " + type + " where  COMPETITION_ID=" + arr[0] +" and booked=false";
                        //    //queryString = "select * from " + type;// + " where booked = false";
                        //    //start_date  BETWEEN  '9/19/2018 00:00:00' and'9/20/2018  10:59:59'
                        //    if (Convert.ToBoolean(arr[0]))
                        //    {
                        //        // queryString = "select e.ID, e.NAME, e.HOME_ID, e.GUEST_ID, e.START_DATE,  e.STATUS_NAME,    e.STATUS_TYPE,    e.ROUND_NAME,     e.BOOKED,  e.GROUP_ID, e.STAGE_ID, e.SEASON_ID, e.COMPETITION_ID,    e.AREA_ID, e.CTIMESTAMP from events e where '" + arr[1] + "'<=  e.start_date and  e.start_date <='" + arr[2] + "'";
                        //        queryString = "select e.* from events e where '" + arr[1] + "'<=  e.start_date and  e.start_date <='" + arr[2] + "' order by  e.start_date asc";
                        //    }
                        //    else
                        //    {
                        //        if (arr[1].ToString().Length < 12)
                        //        {
                        //            //  queryString = "select e.ID, e.NAME, e.HOME_ID, e.GUEST_ID, e.START_DATE,  e.STATUS_NAME,    e.STATUS_TYPE,    e.ROUND_NAME,     e.BOOKED,  e.GROUP_ID, e.STAGE_ID, e.SEASON_ID, e.COMPETITION_ID,    e.AREA_ID, e.CTIMESTAMP from events e where " + (arr[1].ToString() == "Group" ? " e.GROUP_ID" : (arr[1].ToString() == "Stage") ? " e.STAGE_ID" : (arr[1].ToString() == "Season") ? " e.SEASON_ID" : (arr[1].ToString() == "Comp") ? " e.COMPETITION_ID" : "") + "='" + arr[2] + "'";
                        //            queryString = "select e.* from events e where " + (arr[1].ToString() == "Group" ? " e.GROUP_ID" : (arr[1].ToString() == "Stage") ? " e.STAGE_ID" : (arr[1].ToString() == "Season") ? " e.SEASON_ID" : (arr[1].ToString() == "Comp") ? " e.COMPETITION_ID" : "") + "='" + arr[2] + "'  order by  e.start_date asc";
                        //        }
                        //        else
                        //        {
                        //            // queryString = "select e.ID, e.NAME, e.HOME_ID, e.GUEST_ID, e.START_DATE,  e.STATUS_NAME,    e.STATUS_TYPE,    e.ROUND_NAME,     e.BOOKED,  e.GROUP_ID, e.STAGE_ID, e.SEASON_ID, e.COMPETITION_ID,    e.AREA_ID, e.CTIMESTAMP from events e where '" + arr[1] + "'<= e.start_date and  e.start_date <='" + arr[2] + "'";
                        //            queryString = "select e.* from events e where '" + arr[1] + "'<= e.start_date and  e.start_date <='" + arr[2] + "'  order by  e.start_date asc";
                        //        }
                        //    }
                        //    FbDataAdapter adapter = new FbDataAdapter();
                        //    adapter.SelectCommand = new FbCommand(queryString, connection);
                        //    FbCommandBuilder builder = new FbCommandBuilder(adapter);
                        //    connection.Open();
                        //    DataSet eventsDs = new DataSet();
                        //    adapter.Fill(eventsDs);
                        //    if (Convert.ToBoolean(arr[0]) && DateTime.Now <= Convert.ToDateTime(arr[2]) && eventsDs.Tables[0].Rows.Count == 0)
                        //    // if (Convert.ToBoolean(arr[0]))
                        //    //if (DateTime.Now <= Convert.ToDateTime(arr[2]))
                        //    {
                        //        //if (eventsDs.Tables[0].Rows.Count == 0)
                        //        //{
                        //        //DataSet newEventsDs= eventsDs.Clone ();
                        //        for (int i = 1; i < 10; i++)
                        //        {
                        //            //if (Convert.ToBoolean(arr[0]) == false) { ds = eventsDs; break; }

                        //            var responseValue = clientTest.GetAccessData(strToken, "events/" + i, arr[1], arr[2]);
                        //            var strResponseValue = responseValue.Result;
                        //            //XDocument document = XDocument.Load("E:\\Project\\AppProject\\DataOfScouts\\DataOfScouts\\bin\\Debug\\New folder\\events-1153436.xml");
                        //            //var strResponseValue = document.ToString();

                        //            if (strResponseValue == "Unauthorized") { MessageBox.Show("Unauthorized!"); break; }
                        //            string strName = type + "-" + i + " " + DateTime.Now.ToString("HHmmss");
                        //            Files.WriteXml(strName, strResponseValue);

                        //            DOSEvents.api apis = XmlUtil.Deserialize(typeof(DOSEvents.api), strResponseValue) as DOSEvents.api;
                        //            if (apis == null) break;
                        //            DOSEvents.apiDataCompetition[] competitions = (apis.data.Length == 0) ? null : apis.data[0];
                        //            if (competitions == null) break;

                        //            foreach (DOSEvents.apiDataCompetition competition in competitions)
                        //            {
                        //                string strCompetition_id = competition.id;
                        //                string strArea_id = competition.area_id;

                        //                using (FbCommand cmd = new FbCommand())
                        //                {
                        //                    //                                        'ID', 'NAME', 'SHORT_NAME', 'MINI_NAME', 'GENDER',
                        //                    //'CTYPE', 'AREA_ID', 'AREA_TYPE', 'AREA_SORT', 'OVERALL_SORT', 'SPORT_ID',
                        //                    //'SPORT_NAME', 'TOUR_ID', 'TOUR_NAME', 'UT', 'OLD_COMPETITION_ID', 'SLUG', 'CTIMESTAMP'
                        //                    cmd.CommandText = "ADD_COMPETITION";
                        //                    cmd.CommandType = CommandType.StoredProcedure;
                        //                    cmd.Connection = connection;
                        //                    cmd.Parameters.Add("@ID", strCompetition_id);
                        //                    cmd.Parameters.Add("@NAME", competition.name);
                        //                    cmd.Parameters.Add("@SHORT_NAME", competition.short_name);
                        //                    cmd.Parameters.Add("@MINI_NAME", competition.mini_name);
                        //                    cmd.Parameters.Add("@GENDER", (competition.gender.ToLower() == "male") ? true : false);
                        //                    cmd.Parameters.Add("@CTYPE", competition.type);
                        //                    cmd.Parameters.Add("@AREA_ID", competition.area_id);
                        //                    cmd.Parameters.Add("@AREA_TYPE", competition.type);
                        //                    cmd.Parameters.Add("@AREA_SORT", competition.area_sort);
                        //                    cmd.Parameters.Add("@OVERALL_SORT", competition.overall_sort);
                        //                    cmd.Parameters.Add("@TOUR_ID", competition.tour_id == "" ? "-1" : competition.tour_id);
                        //                    cmd.Parameters.Add("@TOUR_NAME", competition.tour_name);
                        //                    cmd.Parameters.Add("@UT", competition.ut);
                        //                    cmd.Parameters.Add("@OLD_COMPETITION_ID", competition.old_competition_id == "" ? "-1" : competition.old_competition_id);
                        //                    cmd.Parameters.Add("@SLUG", competition.slug);
                        //                    cmd.Parameters.Add("@CTIMESTAMP", cTimestamp);
                        //                    int id = Convert.ToInt32(cmd.ExecuteScalar());
                        //                }


                        //                DOSEvents.apiDataCompetitionSeason[] seasons = competition.seasons;
                        //                if (seasons == null) continue;
                        //                foreach (DOSEvents.apiDataCompetitionSeason season in seasons)
                        //                {
                        //                    string strSeasons_id = season.id;
                        //                    using (FbCommand cmd = new FbCommand())
                        //                    {
                        //                        //r.ID, r.NAME, r.COMPETITION_ID, r.SYEAR, r.ACTUAL, r.UT, r.OLD_SEASON_ID,
                        //                        //r.RANGE, r.CTIMESTAMP
                        //                        cmd.CommandText = "ADD_SEASON";
                        //                        cmd.CommandType = CommandType.StoredProcedure;
                        //                        cmd.Connection = connection;
                        //                        cmd.Parameters.Add("@ID", season.id);
                        //                        cmd.Parameters.Add("@NAME", season.name);
                        //                        cmd.Parameters.Add("@COMPETITION_ID", strCompetition_id);
                        //                        cmd.Parameters.Add("@SYEAR", season.year);
                        //                        cmd.Parameters.Add("@ACTUAL", season.actual);
                        //                        cmd.Parameters.Add("@UT", season.ut);
                        //                        cmd.Parameters.Add("@OLD_SEASON_ID", season.old_season_id == "" ? "-1" : season.old_season_id);
                        //                        cmd.Parameters.Add("@RANGE", season.range);
                        //                        cmd.Parameters.Add("@CTIMESTAMP", cTimestamp);
                        //                        int id = Convert.ToInt32(cmd.ExecuteScalar());
                        //                    }

                        //                    DOSEvents.apiDataCompetitionSeasonStage[] stages = season.stages;
                        //                    if (stages == null) continue;

                        //                    foreach (DOSEvents.apiDataCompetitionSeasonStage stage in stages)
                        //                    {
                        //                        string strStage_id = stage.id == "" ? "-1" : stage.id;
                        //                        if (strStage_id != "-1")
                        //                        {
                        //                            using (FbCommand cmd = new FbCommand())
                        //                            {
                        //                                //r.ID, r.STAGE_NAME_ID, r.NAME, r.START_DATE, r.END_DATE,
                        //                                //r.SHOW_STANDINGS, r.GROUPS_NR, r.ISORT, r.IS_CURRENT, r.UT, r.OLD_STAGE_ID,    r.SEASON_ID, r.COMPETITION_ID, r.AREA_ID, r.CTIMESTAMP
                        //                                cmd.CommandText = "ADD_STAGE";
                        //                                cmd.CommandType = CommandType.StoredProcedure;
                        //                                cmd.Connection = connection;
                        //                                cmd.Parameters.Add("@ID", stage.id);
                        //                                cmd.Parameters.Add("@STAGE_NAME_ID", stage.stage_name_id);
                        //                                cmd.Parameters.Add("@NAME", stage.name);
                        //                                cmd.Parameters.Add("@START_DATE", stage.start_date);
                        //                                cmd.Parameters.Add("@END_DATE", stage.end_date);
                        //                                cmd.Parameters.Add("@SHOW_STANDINGS", stage.show_standings.ToLower() == "yes" ? true : false);
                        //                                cmd.Parameters.Add("@GROUPS_NR", stage.groups_nr == "" ? "-1" : stage.groups_nr);
                        //                                cmd.Parameters.Add("@ISORT", stage.sort == "" ? "-1" : stage.sort);
                        //                                cmd.Parameters.Add("@IS_CURRENT", stage.is_current.ToLower() == "yes" ? true : false);
                        //                                cmd.Parameters.Add("@UT", stage.ut);
                        //                                cmd.Parameters.Add("@OLD_STAGE_ID", stage.old_stage_id == "" ? "-1" : stage.old_stage_id);
                        //                                cmd.Parameters.Add("@SEASON_ID", strSeasons_id);
                        //                                cmd.Parameters.Add("@COMPETITION_ID", strCompetition_id);
                        //                                cmd.Parameters.Add("@AREA_ID", strArea_id);
                        //                                cmd.Parameters.Add("@CTIMESTAMP", cTimestamp);
                        //                                int id = Convert.ToInt32(cmd.ExecuteScalar());
                        //                            }
                        //                        }
                        //                        DOSEvents.apiDataCompetitionSeasonStageGroup[] groups = stage.groups;
                        //                        if (groups == null) continue;
                        //                        foreach (DOSEvents.apiDataCompetitionSeasonStageGroup group in groups)
                        //                        {
                        //                            string strGroup_id = group.id == "" ? "-1" : group.id;
                        //                            if (strGroup_id != "-1")
                        //                            {
                        //                                using (FbCommand cmd = new FbCommand())
                        //                                {
                        //                                    // r.ID, r.NAME, r.UT, r.STAGE_ID, r.SEASON_ID, r.COMPETITION_ID, r.AREA_ID,     r.CTIMESTAMP
                        //                                    cmd.CommandText = "ADD_GROUP";
                        //                                    cmd.CommandType = CommandType.StoredProcedure;
                        //                                    cmd.Connection = connection;
                        //                                    cmd.Parameters.Add("@ID", group.id);
                        //                                    cmd.Parameters.Add("@NAME", group.name);
                        //                                    cmd.Parameters.Add("@UT", group.ut);
                        //                                    cmd.Parameters.Add("@STAGE_ID", strStage_id);
                        //                                    cmd.Parameters.Add("@SEASON_ID", strSeasons_id);
                        //                                    cmd.Parameters.Add("@COMPETITION_ID", strCompetition_id);
                        //                                    cmd.Parameters.Add("@AREA_ID", strArea_id);
                        //                                    cmd.Parameters.Add("@CTIMESTAMP", cTimestamp);
                        //                                    int id = Convert.ToInt32(cmd.ExecuteScalar());
                        //                                }
                        //                            }

                        //                            foreach (DOSEvents.apiDataCompetitionSeasonStageGroupEvent sevent in group.events)
                        //                            {
                        //                                if (sevent == null) continue;
                        //                                DataRow[] drs = eventsDs.Tables[0].Select("id=" + sevent.id);
                        //                                if (eventsDs.Tables[0].Select("id=" + sevent.id).Length == 0)
                        //                                {
                        //                                    DOSEvents.apiDataCompetitionSeasonStageGroupEventParticipant[] participants = sevent.participants;

                        //                                    DataRow dr = eventsDs.Tables[0].NewRow();
                        //                                    dr[0] = sevent.id;
                        //                                    dr[1] = sevent.name;
                        //                                    dr[2] = (participants[0].counter == "1") ? participants[0].id : participants[1].id;
                        //                                    dr[3] = (participants[1].counter == "2") ? participants[1].id : participants[0].id;
                        //                                    dr[4] = sevent.source;
                        //                                    dr[5] = sevent.source_dc == "yes" ? true : false;
                        //                                    dr[6] = sevent.source_super;
                        //                                    dr[7] = sevent.relation_status;
                        //                                    dr[8] = Convert.ToDateTime(sevent.start_date);
                        //                                    dr[9] = sevent.ft_only == "yes" ? true : false;
                        //                                    dr[10] = sevent.coverage_type;
                        //                                    dr[11] = sevent.channel_id;
                        //                                    dr[12] = sevent.channel_name;
                        //                                    dr[13] = sevent.scoutsfeed == "yes" ? true : false;
                        //                                    dr[14] = sevent.status_id;
                        //                                    dr[15] = sevent.status_name;
                        //                                    dr[16] = sevent.status_type;
                        //                                    dr[17] = sevent.day;
                        //                                    dr[18] = sevent.clock_time;
                        //                                    dr[19] = sevent.clock_status;
                        //                                    dr[20] = sevent.winner_id;
                        //                                    dr[21] = sevent.progress_id;
                        //                                    dr[22] = sevent.bet_status;
                        //                                    dr[23] = sevent.neutral_venue == "yes" ? true : false;
                        //                                    dr[24] = sevent.item_status;
                        //                                    dr[25] = sevent.ut;
                        //                                    dr[26] = sevent.old_event_id == "" ? "-1" : sevent.old_event_id;
                        //                                    dr[27] = sevent.slug;
                        //                                    dr[28] = sevent.verified_result == "yes" ? true : false;
                        //                                    dr[29] = sevent.is_protocol_verified == "yes" ? true : false;
                        //                                    dr[30] = sevent.protocol_verified_by;
                        //                                    dr[31] = sevent.protocol_verified_at;
                        //                                    dr[32] = sevent.round_id;
                        //                                    dr[33] = sevent.round_name;
                        //                                    dr[34] = sevent.client_event_id == "" ? "-1" : sevent.client_event_id;
                        //                                    dr[35] = sevent.booked == "yes" ? true : false;
                        //                                    dr[36] = sevent.booked_by;
                        //                                    dr[37] = sevent.inverted_participants == "yes" ? true : false;
                        //                                    dr[38] = sevent.venue_id;
                        //                                    dr[39] = group.id == "" ? "-1" : group.id;
                        //                                    dr[40] = strStage_id;
                        //                                    dr[41] = strSeasons_id;
                        //                                    dr[42] = strCompetition_id;
                        //                                    dr[43] = strArea_id;
                        //                                    dr[44] = cTimestamp;
                        //                                    eventsDs.Tables[0].Rows.Add(dr);
                        //                                }
                        //                                else
                        //                                {
                        //                                    Files.WriteLog("[" + drs[0]["id"] + "]   events existed.");
                        //                                }
                        //                            }
                        //                        }
                        //                    }
                        //                }
                        //            }

                        //            count = adapter.Update(eventsDs);
                        //            ds.Merge(eventsDs, true, MissingSchemaAction.AddWithKey);
                        //            eventsDs.Clear();

                        //            if (count > -1)
                        //            {
                        //                Files.WriteLog("[Success] Insert events[" + count + "  " + "] " + " " + strName + ".xml");
                        //            }
                        //            else
                        //            {
                        //                Files.WriteLog("[Failure] Insert events [  ]" + " " + strName + ".xml");
                        //            }
                        //        }
                        //        //}
                        //        //else
                        //        //{
                        //        //    //count = -1;
                        //        //    //queryString = "select   * from " + type;
                        //        //    //adapter.SelectCommand = new FbCommand(queryString, connection);
                        //        //    //adapter.Fill(eventsDs);
                        //        //count = adapter.Update(eventsDs);
                        //        //ds = eventsDs;
                        //        //}
                        //    }
                        //    else
                        //    {
                        //        ds = eventsDs;
                        //    }
                        //    connection.Close();
                        //}
                        break;
                    }
                default:
                    break;
            }
            return ds;
        }
        */

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
                            queryString = "select * from " + type + (arr[0].ToString().ToLower() == "all" ? "" : " where PARENT_AREA_ID=" + arr[0] + " order by name asc");
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
                            queryString = "select * from " + type + " where AREA_ID=" + arr[1];

                            FbDataAdapter adapter = new FbDataAdapter();
                            adapter.SelectCommand = new FbCommand(queryString, connection);
                            FbCommandBuilder builder = new FbCommandBuilder(adapter);
                            connection.Open();
                            DataSet competitionDs = new DataSet();
                            adapter.Fill(competitionDs);

                            if (competitionDs.Tables[0].Rows.Count == 0 && Convert.ToBoolean(arr[0]))
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
                                        //dr[12] = competition.sport_id;
                                        //dr[13] = competition.sport_name;
                                        dr[12] = (competition.tour_id == "") ? "-1" : competition.tour_id;
                                        dr[13] = competition.tour_name;
                                        dr[14] = competition.ut;
                                        dr[15] = (competition.old_competition_id == "") ? "-1" : competition.old_competition_id;
                                        dr[16] = competition.slug;
                                        dr[17] = cTimestamp;
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
                            queryString = "select * from " + type + " where COMPETITION_ID=" + arr[1];

                            FbDataAdapter adapter = new FbDataAdapter();
                            adapter.SelectCommand = new FbCommand(queryString, connection);
                            FbCommandBuilder builder = new FbCommandBuilder(adapter);
                            connection.Open();
                            DataSet seasonsDs = new DataSet();
                            adapter.Fill(seasonsDs);

                            if (seasonsDs.Tables[0].Rows.Count == 0 && Convert.ToBoolean(arr[0]))
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
                            queryString = "select * from " + type + " where season_id=" + arr[1] + " and COMPETITION_ID =" + arr[2];

                            FbDataAdapter adapter = new FbDataAdapter();
                            adapter.SelectCommand = new FbCommand(queryString, connection);
                            FbCommandBuilder builder = new FbCommandBuilder(adapter);
                            connection.Open();
                            DataSet stagesDs = new DataSet();
                            adapter.Fill(stagesDs);

                            if (stagesDs.Tables[0].Rows.Count == 0 && Convert.ToBoolean(arr[0]))
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
                            queryString = "select * from " + type + " where stage_id=" + arr[1] + " and season_id=" + arr[2] + " and COMPETITION_ID =" + arr[3]; ;

                            FbDataAdapter adapter = new FbDataAdapter();
                            adapter.SelectCommand = new FbCommand(queryString, connection);
                            FbCommandBuilder builder = new FbCommandBuilder(adapter);
                            connection.Open();
                            DataSet stagesDs = new DataSet();
                            adapter.Fill(stagesDs);

                            if (stagesDs.Tables[0].Rows.Count == 0 && Convert.ToBoolean(arr[0]))
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
                            queryString = "select t.ID ,t.NAME ,t.SHORT_NAME ,t.season_id ,t.AREA_ID ,t.ctimestamp   from  teams t where season_id=" + arr[1];

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
                                            adapter1.SelectCommand = new FbCommand(" SELECT   p.ID ,p.NAME ,p.SHORT_NAME ,p.position_name ,p.season_id ,p.AREA_ID ,p.ctimestamp   FROM players p" + " where p.season_id=" + arr[1], connection);
                                            FbCommandBuilder builder3 = new FbCommandBuilder(adapter1);
                                            adapter1.Fill(data.Tables["players"]);

                                            if (data.Tables["teams"].Rows.Count == 0 && data.Tables["players"].Rows.Count == 0 && Convert.ToBoolean(arr[0]))
                                            {
                                                for (int i = 1; i < 2; i++)
                                                {
                                                    //XDocument document = XDocument.Load("E:\\Project\\AppProject\\DataOfScouts\\DataOfScouts\\bin\\Debug\\New folder\\participantst-1164510.xml");
                                                    //var strResponseValue = document.ToString();

                                                    var responseValue = clientTest.GetAccessData(strToken, "participants/" + i, arr[0], arr[1]);
                                                    var strResponseValue = responseValue.Result;

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
                                                            dr2[12] = "-1";
                                                            dr2[13] = arr[0];
                                                            dr2[14] = cTimestamp;
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
                        //var responseValue1 = clientTest.PostAccessData(strToken, "participants/" + "booked-events", arr[1].ToString() );
                        //var strResponseValue12 = responseValue1.Result;

                        using (FbConnection connection = new FbConnection(AppFlag.ScoutsDBConn))
                        {
                            // connection.Open();

                            for (int i = 1; i < 2 && Convert.ToBoolean(arr[0]) == true; i++)
                            {
                                var responseValue = clientTest.GetAccessData(strToken, "participants/" + i, arr[0], arr[1]);
                                var strResponseValue = responseValue.Result;

                                if (strResponseValue == "Unauthorized") { MessageBox.Show("Unauthorized!"); break; }
                                string strName = type + "-" + i + " " + DateTime.Now.ToString("HHmmss");

                                DOSParticipants.api apis = XmlUtil.Deserialize(typeof(DOSParticipants.api), strResponseValue) as DOSParticipants.api;
                                if (apis == null) break;

                                Files.WriteXml(strName, strResponseValue);
                                Files.WriteLog("Get " + strName + ".xml.");

                                DOSParticipants.apiDataParticipant[] participants = (apis.data == null || apis.data.Length == 0) ? null : apis.data[0];
                                if (participants == null) break;
                                //string strName = type + "-" + i + " " + DateTime.Now.ToString("HHmmss");
                                //Files.WriteXml(strName, strResponseValue);

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

                            //  queryString = "SELECT t.* FROM teams t  inner  join  events e on   t.id= e.HOME_ID or t.id=e.GUEST_ID   where e.id='" + arr[1] + "' order by id asc";
                            queryString = "SELECT t.ID ,t.NAME ,t.SHORT_NAME ,t.ACRONYM,t.season_id ,t.AREA_ID ,t.ctimestamp FROM teams t  inner  join  events e on   t.id= e.HOME_ID or t.id=e.GUEST_ID   where e.id='" + arr[1] + "' order by id asc";
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
                                            //queryString = "SELECT p.*FROM players p inner join  events e on p.TEAM_ID = e.HOME_ID or p.TEAM_ID = e.GUEST_ID where e.id='" + arr[1] + "' order by p.id asc";
                                            queryString = "SELECT  p.ID ,p.NAME ,p.SHORT_NAME,p.ACRONYM,p.position_name ,p.team_ID ,p.ctimestamp   FROM players p inner join  events e on p.TEAM_ID = e.HOME_ID where e.id='" + arr[1] + "' order by p.id asc";
                                            data.Tables.Add(new DataTable("hplayers"));
                                            FbDataAdapter adapter1 = new FbDataAdapter();
                                            adapter1.SelectCommand = new FbCommand(queryString, connection);
                                            FbCommandBuilder builder2 = new FbCommandBuilder(adapter1);
                                            adapter1.Fill(data.Tables["hplayers"]);

                                            queryString = "SELECT p.ID ,p.NAME ,p.SHORT_NAME ,p.ACRONYM,p.position_name ,p.team_ID ,p.ctimestamp   FROM players p inner join  events e on  p.TEAM_ID = e.GUEST_ID where e.id='" + arr[1] + "' order by p.id asc";
                                            data.Tables.Add(new DataTable("gplayers"));
                                            FbDataAdapter adapter2 = new FbDataAdapter();
                                            adapter2.SelectCommand = new FbCommand(queryString, connection);
                                            FbCommandBuilder builder3 = new FbCommandBuilder(adapter1);
                                            adapter2.Fill(data.Tables["gplayers"]);
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
                case "events.show2":
                    {
                        using (FbConnection connection = new FbConnection(AppFlag.ScoutsDBConn))
                        {
                            connection.Open();
                            for (int i = 1; i < 2 && Convert.ToBoolean(arr[0]) == true && arr[1].ToString() != ""; i++)
                            {
                                var responseValue = clientTest.GetAccessData(strToken, "events.show", arr[1]);
                                var strResponseValue = responseValue.Result;
                                if (strResponseValue == "Unauthorized") { MessageBox.Show("Unauthorized!"); break; }
                                string strName = arr[1].ToString() + "_" + type + "-" + i + "_" + DateTime.Now.ToString("HHmmss");

                                DOSEvents2.api apis = XmlUtil.Deserialize(typeof(DOSEvents2.api), strResponseValue) as DOSEvents2.api;
                                if (apis == null) break;

                                Files.WriteXml(strName, strResponseValue);
                                Files.WriteLog("Get " + strName + ".xml.");

                                DOSEvents2.apiDataCompetition competition = (apis.data == null || apis.data.Length == 0) ? null : apis.data[0];
                                if (competition == null) break;

                                string strCompetition_id = competition.id;
                                string strArea_id = competition.area_id;

                                DOSEvents2.apiDataCompetitionSeason[] seasons = competition.seasons;
                                if (seasons == null) continue;
                                foreach (DOSEvents2.apiDataCompetitionSeason season in seasons)
                                {
                                    DOSEvents2.apiDataCompetitionSeasonStage[] stages = season.stages;
                                    if (stages == null) continue;

                                    foreach (DOSEvents2.apiDataCompetitionSeasonStage stage in stages)
                                    {
                                        DOSEvents2.apiDataCompetitionSeasonStageGroup[] groups = stage.groups;
                                        if (groups == null) continue;
                                        foreach (DOSEvents2.apiDataCompetitionSeasonStageGroup group in groups)
                                        {
                                            DOSEvents2.apiDataCompetitionSeasonStageGroupEvent[] events = group.events;
                                            if (events == null) continue;
                                            foreach (DOSEvents2.apiDataCompetitionSeasonStageGroupEvent sevent in events)
                                            {
                                                if (sevent == null) continue;

                                                string sName = "";
                                                string sPlayers = "";
                                                using (FbCommand cmd2 = new FbCommand("SELECT  r.EMATCHID , r.HKJCDAYCODE, r.HKJCMATCHNO FROM EMATCHES r where r.EMATCHID='" + sevent.id + "'", connection))
                                                {
                                                    FbDataReader reader = cmd2.ExecuteReader();
                                                    while (reader.Read())
                                                    {
                                                        var data = (IDataRecord)reader;
                                                        sName = data[1] + "-" + data[2] + "-" + data[0];
                                                    }
                                                    reader.Close();
                                                }
                                                foreach (DOSEvents2.apiDataCompetitionSeasonStageGroupEventParticipant participant in sevent.participants)
                                                {
                                                    if (participant == null) continue;
                                                    sPlayers += "\r\n" + (participant.counter == "1" ? "Host\r\n" : "Guest\r\n");
                                                    foreach (DOSEvents2.apiDataCompetitionSeasonStageGroupEventParticipantLineup lineup in participant.lineups)
                                                    {
                                                        if (lineup == null) continue;

                                                        sPlayers += (lineup.shirt_nr == "" ? "-1" : lineup.shirt_nr) + "  " + lineup.participant_name + "  " + (lineup.bench.ToLower() == "yes" ? "/Bench" : "     ") + "  " + (lineup.type.ToLower() == "coach" ? "/Coach" : "") + "\r\n";

                                                        using (FbCommand cmd2 = new FbCommand())
                                                        {
                                                            cmd2.CommandText = "ADD_Player2";
                                                            cmd2.CommandType = CommandType.StoredProcedure;
                                                            cmd2.Connection = connection;
                                                            cmd2.Parameters.Add("@ID", lineup.participant_id == "" ? "-1" : lineup.participant_id);
                                                            cmd2.Parameters.Add("@NAME", lineup.participant_name);
                                                            cmd2.Parameters.Add("@AREA_ID", lineup.participant_area_id == "" ? "-1" : lineup.participant_area_id);
                                                            cmd2.Parameters.Add("@SLUG", lineup.participant_slug);
                                                            cmd2.Parameters.Add("@BENCH", lineup.bench.ToLower() == "yes" ? true : false);
                                                            cmd2.Parameters.Add("@SHIRT_NR", lineup.shirt_nr == "" || lineup.shirt_nr == "-" ? "-1" : lineup.shirt_nr.Replace("-", ""));
                                                            cmd2.Parameters.Add("@TEAM_ID", participant.id);
                                                            cmd2.Parameters.Add("@SEASON_ID", season.id);
                                                            cmd2.Parameters.Add("@CTIMESTAMP", cTimestamp);
                                                            int id = Convert.ToInt32(cmd2.ExecuteScalar());
                                                            Files.WriteLog(id > 0 ? " [Success] Insert players [" + lineup.participant_id + "] " + lineup.participant_name : "[" + lineup.participant_id + "] " + lineup.participant_name + " player exist.");
                                                        }
                                                    }

                                                    ////  Files.WriteLog(sName,true, "\r\n"+(participant.counter == "1" ? "Host\r\n" : "Guest\r\n") + sPlayers);


                                                    using (FbCommand cmd2 = new FbCommand())
                                                    {
                                                        cmd2.CommandText = "ADD_TEAM2";
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
                                                        Files.WriteLog(id > 0 ? " [Success] Insert teams[" + participant.id + "] " + participant.name : "[" + participant.id + "] " + participant.name + " team exist.");
                                                    }
                                                    //foreach (DOSEvents2.apiDataCompetitionSeasonStageGroupEventParticipantLineup lineup in participant.lineups)
                                                    //{
                                                    //    if (lineup == null) continue;

                                                    //    using (FbCommand cmd2 = new FbCommand())
                                                    //    {
                                                    //        cmd2.CommandText = "ADD_Player2";
                                                    //        cmd2.CommandType = CommandType.StoredProcedure;
                                                    //        cmd2.Connection = connection;
                                                    //        cmd2.Parameters.Add("@ID", lineup.participant_id == "" ? "-1" : lineup.participant_id);
                                                    //        cmd2.Parameters.Add("@NAME", lineup.participant_name);
                                                    //        cmd2.Parameters.Add("@AREA_ID", lineup.participant_area_id == "" ? "-1" : lineup.participant_area_id);
                                                    //        cmd2.Parameters.Add("@SLUG", lineup.participant_slug);
                                                    //        cmd2.Parameters.Add("@BENCH", lineup.bench.ToLower() == "yes" ? true : false);
                                                    //        cmd2.Parameters.Add("@SHIRT_NR", lineup.shirt_nr == "" ? "-1" : lineup.shirt_nr);
                                                    //        cmd2.Parameters.Add("@TEAM_ID", participant.id);
                                                    //        cmd2.Parameters.Add("@SEASON_ID", season.id);
                                                    //        cmd2.Parameters.Add("@CTIMESTAMP", cTimestamp);
                                                    //        int id = Convert.ToInt32(cmd2.ExecuteScalar());
                                                    //        // Files.WriteLog(id > 0 ? " [Success] Insert players2 [" + lineup.participant_id+ "] " + lineup.participant_name + " " + strName + ".xml" : " player exist.");
                                                    //        Files.WriteLog(id > 0 ? " [Success] Insert players [" + lineup.participant_id + "] " + lineup.participant_name : "[" + lineup.participant_id + "] " + lineup.participant_name + " player exist.");
                                                    //    }
                                                    //} 
                                                }

                                                // if (sName != "") Files.WriteLog(sName, true, sPlayers);

                                                if (sevent.participants.Length == 2 && sevent.participants[0].stats.Length > 0 && sevent.participants[0].results.Length > 0)
                                                {
                                                    using (FbCommand cmd2 = new FbCommand())
                                                    {
                                                        cmd2.CommandText = "PR_stats_GoalInfo";
                                                        cmd2.CommandType = CommandType.StoredProcedure;
                                                        cmd2.Connection = connection;
                                                        cmd2.Parameters.Add("@EMATCHID", sevent.id);
                                                        cmd2.Parameters.Add("@H_YELLOW", sevent.participants[0].counter == "1" ? sevent.participants[0].stats.FirstOrDefault(c => c.id == "8").value == "" ? null : sevent.participants[0].stats.FirstOrDefault(c => c.id == "8").value : sevent.participants[1].stats.FirstOrDefault(c => c.id == "8").value == "" ? null : sevent.participants[1].stats.FirstOrDefault(c => c.id == "8").value);
                                                        cmd2.Parameters.Add("@H_RED", sevent.participants[0].counter == "1" ? sevent.participants[0].stats.FirstOrDefault(c => c.id == "9").value == "" ? null : sevent.participants[0].stats.FirstOrDefault(c => c.id == "9").value : sevent.participants[1].stats.FirstOrDefault(c => c.id == "9").value == "" ? null : sevent.participants[1].stats.FirstOrDefault(c => c.id == "9").value);
                                                        cmd2.Parameters.Add("@G_YELLOW", sevent.participants[1].counter == "2" ? sevent.participants[1].stats.FirstOrDefault(c => c.id == "8").value == "" ? null : sevent.participants[1].stats.FirstOrDefault(c => c.id == "8").value : sevent.participants[0].stats.FirstOrDefault(c => c.id == "8").value == "" ? null : sevent.participants[0].stats.FirstOrDefault(c => c.id == "8").value);
                                                        cmd2.Parameters.Add("@G_RED", sevent.participants[1].counter == "2" ? sevent.participants[1].stats.FirstOrDefault(c => c.id == "9").value == "" ? null : sevent.participants[1].stats.FirstOrDefault(c => c.id == "9").value : sevent.participants[0].stats.FirstOrDefault(c => c.id == "9").value == "" ? null : sevent.participants[0].stats.FirstOrDefault(c => c.id == "9").value);
                                                        cmd2.Parameters.Add("@LASTTIME", DateTime.Now);
                                                        int id = Convert.ToInt32(cmd2.ExecuteScalar());
                                                        Files.WriteLog((id > 0 ? " [Success] Update GoalInfo " : " [Failure]  Update GoalInfo ") + "[" + sevent.id + "] " + sevent.participants[0].id + "/" + sevent.participants[1].id + "..");
                                                    }

                                                    using (FbCommand cmd2 = new FbCommand())
                                                    {
                                                        cmd2.CommandText = "PR_Result_GoalInfo";
                                                        cmd2.CommandType = CommandType.StoredProcedure;
                                                        cmd2.Connection = connection;
                                                        cmd2.Parameters.Add("@EMATCHID", sevent.id);
                                                        cmd2.Parameters.Add("@H_GOAL", sevent.participants[0].counter == "1" ? sevent.participants[0].results.FirstOrDefault(c => c.id == "2").value == "" ? null : sevent.participants[0].results.FirstOrDefault(c => c.id == "2").value : sevent.participants[1].stats.FirstOrDefault(c => c.id == "2").value == "" ? null : sevent.participants[1].stats.FirstOrDefault(c => c.id == "2").value);
                                                        cmd2.Parameters.Add("@G_GOAL", sevent.participants[1].counter == "2" ? sevent.participants[1].results.FirstOrDefault(c => c.id == "2").value == "" ? null : sevent.participants[1].results.FirstOrDefault(c => c.id == "2").value : sevent.participants[0].stats.FirstOrDefault(c => c.id == "2").value == "" ? null : sevent.participants[0].stats.FirstOrDefault(c => c.id == "2").value);
                                                        cmd2.Parameters.Add("@LASTTIME", DateTime.Now);
                                                        int id = Convert.ToInt32(cmd2.ExecuteScalar());
                                                        Files.WriteLog((id > 0 ? " [Success] Update GoalInfo Goal " : " [Failure]  Update GoalInfo Goal") + "[" + sevent.id + "] " + sevent.participants[0].id + "/" + sevent.participants[1].id + "..");
                                                    }
                                                }
                                                //1142019
                                                DOSEvents2.apiDataCompetitionSeasonStageGroupEventEvent_incident[] events_incidents = sevent.events_incidents;
                                                if (events_incidents == null || events_incidents.Count() == 0) continue;
                                                foreach (DOSEvents2.apiDataCompetitionSeasonStageGroupEventEvent_incident Event_incident in sevent.events_incidents)
                                                {
                                                    if (Event_incident == null) continue;

                                                    using (FbCommand cmd2 = new FbCommand())
                                                    {
                                                        cmd2.CommandText = "PR_INCIDENTS";
                                                        cmd2.CommandType = CommandType.StoredProcedure;
                                                        cmd2.Connection = connection;
                                                        cmd2.Parameters.Add("@ID", Event_incident.id);
                                                        cmd2.Parameters.Add("@EVENTID", sevent.id);
                                                        cmd2.Parameters.Add("@CACTION", "update");
                                                        cmd2.Parameters.Add("@INCIDENT_ID", Event_incident.incident_id);
                                                        cmd2.Parameters.Add("@INCIDENT_NAME", Event_incident.incident_name);
                                                        cmd2.Parameters.Add("@PARTICIPANT_ID", Event_incident.participant_id);
                                                        cmd2.Parameters.Add("@PARTICIPANT_NAME", Event_incident.participant_name);
                                                        cmd2.Parameters.Add("@SUBPARTICIPANT_ID", Event_incident.subparticipant_id);
                                                        cmd2.Parameters.Add("@SUBPARTICIPANT_NAME", Event_incident.subparticipant_name);
                                                        cmd2.Parameters.Add("@IMPORTANT_FOR_TRADER", true);// incidentJson.data.incident.important_for_trader == "yes" ? true : false);
                                                        cmd2.Parameters.Add("@EVENT_TIME", Event_incident.event_time);
                                                        cmd2.Parameters.Add("@EVENT_STATUS_ID", Event_incident.event_status_id);
                                                        cmd2.Parameters.Add("@EVENT_STATUS_NAME", Event_incident.event_status_name);
                                                        cmd2.Parameters.Add("@UT", Event_incident.ut);
                                                        cmd2.Parameters.Add("@CTIMESTAMP", DateTime.Now);
                                                        cmd2.Parameters.Add("@TEAMTYPE",
                                                        (Event_incident.participant_id == null || Event_incident.participant_id == "" ? "" : Event_incident.participant_id.ToString() == sevent.participants[0].id.ToString() ? "H" : Event_incident.participant_id.ToString() == sevent.participants[1].id.ToString() ? "G" : "H"));
                                                        int sID = Convert.ToInt32(cmd2.ExecuteScalar());
                                                        Files.WriteLog((sID > -1 ? " [Success] Update INCIDENTS " : " [Failure] Update INCIDENTS ") + "[" + sevent.id + "] " + Event_incident.id + " " + Event_incident.incident_id + " " + Event_incident.incident_name);
                                                    }
                                                }
                                                Files.WriteLog(" Update INCIDENTS " + "[" + sevent.id + "]," + strName + ".xml..");
                                            }
                                        }
                                    }
                                }
                            }
                            queryString = "SELECT t.* FROM teams t  inner  join  events e on   t.id= e.HOME_ID or t.id=e.GUEST_ID   where e.id='" + arr[1] + "' ";// order by id asc";
                            using (FbCommand cmdA = new FbCommand(queryString))
                            {
                                using (FbCommandBuilder fcb = new FbCommandBuilder())
                                {
                                    using (FbDataAdapter fda = new FbDataAdapter())
                                    {
                                        cmdA.Connection = connection;
                                        fda.SelectCommand = cmdA;
                                        fcb.DataAdapter = fda;
                                        using (DataSet data = new DataSet())
                                        {
                                            data.Tables.Add(new DataTable("teams"));
                                            fda.Fill(data.Tables["teams"]);

                                            queryString = "SELECT  p.*   FROM players p inner join  events e on p.TEAM_ID = e.HOME_ID where e.id='" + arr[1] + "' order by p.id asc";
                                            data.Tables.Add(new DataTable("hplayers"));
                                            FbDataAdapter adapter1 = new FbDataAdapter();
                                            adapter1.SelectCommand = new FbCommand(queryString, connection);
                                            FbCommandBuilder builder2 = new FbCommandBuilder(adapter1);
                                            adapter1.Fill(data.Tables["hplayers"]);

                                            queryString = "SELECT  p.* FROM players p inner join  events e on  p.TEAM_ID = e.GUEST_ID where e.id='" + arr[1] + "' order by p.id asc";
                                            data.Tables.Add(new DataTable("gplayers"));
                                            FbDataAdapter adapter2 = new FbDataAdapter();
                                            adapter2.SelectCommand = new FbCommand(queryString, connection);
                                            FbCommandBuilder builder3 = new FbCommandBuilder(adapter1);
                                            adapter2.Fill(data.Tables["gplayers"]);

                                            queryString = "SELECT  i.* FROM EVENT_DETAILS i where i.EVENTID='" + arr[1] + "'";
                                            data.Tables.Add(new DataTable("Details"));
                                            FbDataAdapter adapter3 = new FbDataAdapter();
                                            adapter3.SelectCommand = new FbCommand(queryString, connection);
                                            //   FbCommandBuilder builder3 = new FbCommandBuilder(adapter1);
                                            adapter3.Fill(data.Tables["Details"]);

                                            queryString = "SELECT  i.* FROM PARTICIPANT_RESULTS i where i.EVENTID='" + arr[1] + "' order by i.id asc";
                                            data.Tables.Add(new DataTable("Results"));
                                            FbDataAdapter adapter4 = new FbDataAdapter();
                                            adapter4.SelectCommand = new FbCommand(queryString, connection);
                                            //   FbCommandBuilder builder3 = new FbCommandBuilder(adapter1);
                                            adapter4.Fill(data.Tables["Results"]);


                                            queryString = "SELECT  i.* FROM PARTICIPANT_STATS i where i.EVENTID='" + arr[1] + "' order by i.id asc";
                                            data.Tables.Add(new DataTable("STATS"));
                                            FbDataAdapter adapter6 = new FbDataAdapter();
                                            adapter6.SelectCommand = new FbCommand(queryString, connection);
                                            //   FbCommandBuilder builder3 = new FbCommandBuilder(adapter1);
                                            adapter6.Fill(data.Tables["STATS"]);


                                            queryString = "SELECT  i.* FROM INCIDENTS i where i.EVENTID='" + arr[1] + "' order by i.id asc";
                                            data.Tables.Add(new DataTable("Incids"));
                                            FbDataAdapter adapter5 = new FbDataAdapter();
                                            adapter5.SelectCommand = new FbCommand(queryString, connection);
                                            //   FbCommandBuilder builder3 = new FbCommandBuilder(adapter1);
                                            adapter5.Fill(data.Tables["Incids"]);

                                            queryString = "SELECT  i.* FROM GoalInfo i where i.EMATCHID='" + arr[1] + "'";
                                            data.Tables.Add(new DataTable("GoalInfo"));
                                            FbDataAdapter adapter7 = new FbDataAdapter();
                                            adapter7.SelectCommand = new FbCommand(queryString, connection);
                                            //   FbCommandBuilder builder3 = new FbCommandBuilder(adapter1);
                                            adapter7.Fill(data.Tables["GoalInfo"]);


                                            queryString = "SELECT  i.* FROM MatchDetails i where i.EMATCHID='" + arr[1] + "' order by  i.INCIDENTS_ID asc ";
                                            data.Tables.Add(new DataTable("MatchDetails"));
                                            FbDataAdapter adapter8 = new FbDataAdapter();
                                            adapter8.SelectCommand = new FbCommand(queryString, connection);
                                            //   FbCommandBuilder builder3 = new FbCommandBuilder(adapter1);
                                            adapter8.Fill(data.Tables["MatchDetails"]);


                                            ds = data;
                                        }
                                    }
                                }
                            }
                            connection.Close();
                        }
                        break;
                    }
                case "events.show":
                    {
                        using (FbConnection connection = new FbConnection(AppFlag.ScoutsDBConn))
                        {
                            connection.Open();
                            //for (int i = 1; i < 10 && Convert.ToBoolean(arr[0]) == true; i++)
                            //{
                            //    var responseValue = clientTest.GetAccessData(strToken, "participants/" + i, arr[0], arr[1]);
                            //    var strResponseValue = responseValue.Result;

                            //    if (strResponseValue == "Unauthorized") { MessageBox.Show("Unauthorized!"); break; }
                            //    // if (strResponseValue == "Unauthorized") { MessageBox.Show("Unauthorized!");   ds = data;break; }

                            //    DOSParticipants.api apis = XmlUtil.Deserialize(typeof(DOSParticipants.api), strResponseValue) as DOSParticipants.api;
                            //    if (apis == null) break;
                            //    DOSParticipants.apiDataParticipant[] participants = (apis.data.Length == 0) ? null : apis.data[0];
                            //    //  if (participants == null) return ds;
                            //    if (participants == null) break;
                            //    string strName = type + "-" + i + " " + DateTime.Now.ToString("HHmmss");
                            //    Files.WriteXml(strName, strResponseValue);

                            //    foreach (DOSParticipants.apiDataParticipant participant in participants)
                            //    {
                            //        if (participant.type == "team")
                            //        {
                            //            using (FbCommand cmd2 = new FbCommand())
                            //            {
                            //                cmd2.CommandText = "ADD_TEAM";
                            //                cmd2.CommandType = CommandType.StoredProcedure;
                            //                cmd2.Connection = connection;
                            //                cmd2.Parameters.Add("@ID", participant.id);
                            //                cmd2.Parameters.Add("@NAME", participant.name);
                            //                cmd2.Parameters.Add("@SHORT_NAME", participant.short_name);
                            //                cmd2.Parameters.Add("@ACRONYM", participant.acronym);
                            //                cmd2.Parameters.Add("@GENDER", (participant.gender.ToLower() == "male") ? true : false);
                            //                cmd2.Parameters.Add("@AREA_ID", participant.area_id);
                            //                cmd2.Parameters.Add("@BNATIONAL", (participant.national.ToLower() == "male") ? true : false);
                            //                cmd2.Parameters.Add("@UT", participant.ut);
                            //                cmd2.Parameters.Add("@OLD_PARTICIPANT_ID", participant.old_participant_id == "" ? "-1" : participant.old_participant_id);
                            //                cmd2.Parameters.Add("@SLUG", participant.slug);
                            //                cmd2.Parameters.Add("@SEASON_ID", "-1");
                            //                cmd2.Parameters.Add("@CTIMESTAMP", cTimestamp);
                            //                int id = Convert.ToInt32(cmd2.ExecuteScalar());
                            //                Files.WriteLog(id > 0 ? " [Success] Insert teams[" + count + "  " + "] " + " " + strName + ".xml" : " team exist.");
                            //            }
                            //        }
                            //        else if (participant.type == "person")
                            //        {
                            //            using (FbCommand cmd2 = new FbCommand())
                            //            {
                            //                cmd2.CommandText = "ADD_Player";
                            //                cmd2.CommandType = CommandType.StoredProcedure;
                            //                cmd2.Connection = connection;
                            //                cmd2.Parameters.Add("@ID", participant.id);
                            //                cmd2.Parameters.Add("@NAME", participant.name);
                            //                cmd2.Parameters.Add("@SHORT_NAME", participant.short_name);
                            //                cmd2.Parameters.Add("@ACRONYM", participant.acronym);
                            //                cmd2.Parameters.Add("@GENDER", (participant.gender.ToLower() == "male") ? true : false);
                            //                cmd2.Parameters.Add("@BIRTHDATE", participant.area_id);
                            //                cmd2.Parameters.Add("@POSITION_NAME", participant.area_id);
                            //                cmd2.Parameters.Add("@AREA_ID", participant.area_id);
                            //                cmd2.Parameters.Add("@BNATIONAL", (participant.national.ToLower() == "male") ? true : false);
                            //                cmd2.Parameters.Add("@UT", participant.ut);
                            //                cmd2.Parameters.Add("@OLD_PARTICIPANT_ID", participant.old_participant_id == "" ? "-1" : participant.old_participant_id);
                            //                cmd2.Parameters.Add("@SLUG", participant.slug);
                            //                cmd2.Parameters.Add("@TEAM_ID", arr[0]);
                            //                cmd2.Parameters.Add("@SEASON_ID", "-1");
                            //                cmd2.Parameters.Add("@CTIMESTAMP", cTimestamp);
                            //                int id = Convert.ToInt32(cmd2.ExecuteScalar());
                            //                Files.WriteLog(id > 0 ? " [Success] Insert players[" + count + "  " + "] " + " " + strName + ".xml" : " player exist.");
                            //            }
                            //        }
                            //    }
                            //}

                            for (int i = 1; i < 2 && Convert.ToBoolean(arr[0]) == true; i++)
                            {
                                //if (Convert.ToBoolean(arr[0]) == false) { ds = eventsDs; break; }
                                var responseValue = clientTest.GetAccessData(strToken, type, arr[1]);
                                var strResponseValue = responseValue.Result;
                                //XDocument document = XDocument.Load("E:\\Project\\AppProject\\DataOfScouts\\DataOfScouts\\bin\\Debug\\New folder\\events-1153436.xml");
                                //var strResponseValue = document.ToString(); 
                                if (strResponseValue == "Unauthorized") { MessageBox.Show("Unauthorized!"); break; }
                                string strName = arr[1].ToString() + type + "-" + i + " " + DateTime.Now.ToString("HHmmss");

                                DOSEvents2.api apis = XmlUtil.Deserialize(typeof(DOSEvents2.api), strResponseValue) as DOSEvents2.api;
                                if (apis == null) break;

                                Files.WriteXml(strName, strResponseValue);
                                Files.WriteLog("Get " + strName + ".xml.");

                                DOSEvents2.apiDataCompetition competition = (apis.data.Length == 0) ? null : apis.data[0];
                                if (competition == null) break;

                                //foreach (DOSEvents2.apiDataCompetition competition in competitions)
                                //{
                                string strCompetition_id = competition.id;
                                string strArea_id = competition.area_id;
                                //connection.Open();
                                //using (FbCommand cmd = new FbCommand())
                                //{
                                //    //                                        'ID', 'NAME', 'SHORT_NAME', 'MINI_NAME', 'GENDER',
                                //    //'CTYPE', 'AREA_ID', 'AREA_TYPE', 'AREA_SORT', 'OVERALL_SORT', 'SPORT_ID',
                                //    //'SPORT_NAME', 'TOUR_ID', 'TOUR_NAME', 'UT', 'OLD_COMPETITION_ID', 'SLUG', 'CTIMESTAMP'
                                //    cmd.CommandText = "ADD_COMPETITION";
                                //    cmd.CommandType = CommandType.StoredProcedure;
                                //    cmd.Connection = connection;
                                //    cmd.Parameters.Add("@ID", strCompetition_id);
                                //    cmd.Parameters.Add("@NAME", competition.name);
                                //    cmd.Parameters.Add("@SHORT_NAME", competition.short_name);
                                //    cmd.Parameters.Add("@MINI_NAME", competition.mini_name);
                                //    cmd.Parameters.Add("@GENDER", (competition.gender.ToLower() == "male") ? true : false);
                                //    cmd.Parameters.Add("@CTYPE", competition.type);
                                //    cmd.Parameters.Add("@AREA_ID", competition.area_id);
                                //    cmd.Parameters.Add("@AREA_TYPE", competition.type);
                                //    cmd.Parameters.Add("@AREA_SORT", competition.area_sort);
                                //    cmd.Parameters.Add("@OVERALL_SORT", competition.overall_sort);
                                //    cmd.Parameters.Add("@TOUR_ID", competition.tour_id == "" ? "-1" : competition.tour_id);
                                //    cmd.Parameters.Add("@TOUR_NAME", competition.tour_name);
                                //    cmd.Parameters.Add("@UT", competition.ut);
                                //    cmd.Parameters.Add("@OLD_COMPETITION_ID", competition.old_competition_id == "" ? "-1" : competition.old_competition_id);
                                //    cmd.Parameters.Add("@SLUG", competition.slug);
                                //    cmd.Parameters.Add("@CTIMESTAMP", cTimestamp);
                                //    int id = Convert.ToInt32(cmd.ExecuteScalar());
                                //    Files.WriteLog(id > 0 ? " [Success] Insert competition[" + competition.id + "] " + competition.name : "[" + competition.id + "] " + competition.name + " competition exist.");
                                //} 
                                DOSEvents2.apiDataCompetitionSeason[] seasons = competition.seasons;
                                if (seasons == null) continue;
                                foreach (DOSEvents2.apiDataCompetitionSeason season in seasons)
                                {
                                    DOSEvents2.apiDataCompetitionSeasonStage[] stages = season.stages;
                                    if (stages == null) continue;

                                    foreach (DOSEvents2.apiDataCompetitionSeasonStage stage in stages)
                                    {
                                        DOSEvents2.apiDataCompetitionSeasonStageGroup[] groups = stage.groups;
                                        if (groups == null) continue;
                                        foreach (DOSEvents2.apiDataCompetitionSeasonStageGroup group in groups)
                                        {
                                            DOSEvents2.apiDataCompetitionSeasonStageGroupEvent[] events = group.events;
                                            if (events == null) continue;
                                            foreach (DOSEvents2.apiDataCompetitionSeasonStageGroupEvent sevent in events)
                                            {
                                                if (sevent == null) continue;

                                                foreach (DOSEvents2.apiDataCompetitionSeasonStageGroupEventParticipant participant in sevent.participants)
                                                {
                                                    if (participant == null) continue;

                                                    foreach (DOSEvents2.apiDataCompetitionSeasonStageGroupEventParticipantLineup lineup in participant.lineups)
                                                    {
                                                        if (lineup == null) continue;
                                                        using (FbCommand cmd2 = new FbCommand())
                                                        {
                                                            cmd2.CommandText = "ADD_Player2";
                                                            cmd2.CommandType = CommandType.StoredProcedure;
                                                            cmd2.Connection = connection;
                                                            cmd2.Parameters.Add("@ID", lineup.participant_id == "" ? "-1" : lineup.participant_id);
                                                            cmd2.Parameters.Add("@NAME", lineup.participant_name);
                                                            cmd2.Parameters.Add("@AREA_ID", lineup.participant_area_id == "" ? "-1" : lineup.participant_area_id);
                                                            cmd2.Parameters.Add("@SLUG", lineup.participant_slug);
                                                            cmd2.Parameters.Add("@BENCH", lineup.bench.ToLower() == "yes" ? true : false);
                                                            cmd2.Parameters.Add("@SHIRT_NR", lineup.shirt_nr == "" || lineup.shirt_nr == "-" ? "-1" : lineup.shirt_nr.Replace("-", ""));
                                                            cmd2.Parameters.Add("@TEAM_ID", participant.id);
                                                            cmd2.Parameters.Add("@SEASON_ID", season.id);
                                                            cmd2.Parameters.Add("@CTIMESTAMP", cTimestamp);
                                                            int id = Convert.ToInt32(cmd2.ExecuteScalar());
                                                            Files.WriteLog(id > 0 ? " [Success] Insert players [" + lineup.participant_id + "] " + lineup.participant_name : "[" + lineup.participant_id + "] " + lineup.participant_name + " player exist.");
                                                        }
                                                    }

                                                    using (FbCommand cmd2 = new FbCommand())
                                                    {
                                                        cmd2.CommandText = "ADD_TEAM2";
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
                                                        //  Files.WriteLog(id > 0 ? " [Success] Insert teams[" + participant.id + "] " + participant.name + " " + strName + ".xml" : " team exist.");
                                                        Files.WriteLog(id > 0 ? " [Success] Insert teams[" + participant.id + "] " + participant.name : "[" + participant.id + "] " + participant.name + " team exist.");
                                                    }
                                                    //foreach (DOSEvents2.apiDataCompetitionSeasonStageGroupEventParticipantLineup lineup in participant.lineups)
                                                    //{
                                                    //    if (lineup == null) continue;

                                                    //    using (FbCommand cmd2 = new FbCommand())
                                                    //    {
                                                    //        cmd2.CommandText = "ADD_Player2";
                                                    //        cmd2.CommandType = CommandType.StoredProcedure;
                                                    //        cmd2.Connection = connection;
                                                    //        cmd2.Parameters.Add("@ID", lineup.participant_id);
                                                    //        cmd2.Parameters.Add("@NAME", lineup.participant_name);
                                                    //        cmd2.Parameters.Add("@AREA_ID", lineup.participant_area_id);
                                                    //        cmd2.Parameters.Add("@SLUG", lineup.participant_slug);
                                                    //        cmd2.Parameters.Add("@BENCH", lineup.bench.ToLower() == "yes" ? true : false);
                                                    //        cmd2.Parameters.Add("@SHIRT_NR", lineup.shirt_nr == "" ? "-1" : lineup.shirt_nr);
                                                    //        cmd2.Parameters.Add("@TEAM_ID", participant.id);
                                                    //        cmd2.Parameters.Add("@SEASON_ID", season.id);
                                                    //        cmd2.Parameters.Add("@CTIMESTAMP", cTimestamp);
                                                    //        int id = Convert.ToInt32(cmd2.ExecuteScalar());
                                                    //        // Files.WriteLog(id > 0 ? " [Success] Insert players2 [" + lineup.participant_id+ "] " + lineup.participant_name + " " + strName + ".xml" : " player exist.");
                                                    //        Files.WriteLog(id > 0 ? " [Success] Insert players [" + lineup.participant_id + "] " + lineup.participant_name : "[" + lineup.participant_id + "] " + lineup.participant_name + " player exist.");
                                                    //    }
                                                    //} 
                                                }
                                                //1142019
                                                foreach (DOSEvents2.apiDataCompetitionSeasonStageGroupEventEvent_incident Event_incident in sevent.events_incidents)
                                                {
                                                    if (Event_incident == null) continue;

                                                    using (FbCommand cmd2 = new FbCommand())
                                                    {
                                                        cmd2.CommandText = "PR_INCIDENTS";
                                                        cmd2.CommandType = CommandType.StoredProcedure;
                                                        cmd2.Connection = connection;
                                                        cmd2.Parameters.Add("@ID", Event_incident.id);
                                                        cmd2.Parameters.Add("@EVENTID", sevent.id);
                                                        cmd2.Parameters.Add("@CACTION", "update");
                                                        cmd2.Parameters.Add("@INCIDENT_ID", Event_incident.incident_id);
                                                        cmd2.Parameters.Add("@INCIDENT_NAME", Event_incident.incident_name);
                                                        cmd2.Parameters.Add("@PARTICIPANT_ID", Event_incident.participant_id);
                                                        cmd2.Parameters.Add("@PARTICIPANT_NAME", Event_incident.participant_name);
                                                        cmd2.Parameters.Add("@SUBPARTICIPANT_ID", Event_incident.subparticipant_id);
                                                        cmd2.Parameters.Add("@SUBPARTICIPANT_NAME", Event_incident.subparticipant_name);
                                                        cmd2.Parameters.Add("@IMPORTANT_FOR_TRADER", true);// incidentJson.data.incident.important_for_trader == "yes" ? true : false);
                                                        cmd2.Parameters.Add("@EVENT_TIME", Event_incident.event_time);
                                                        cmd2.Parameters.Add("@EVENT_STATUS_ID", Event_incident.event_status_id);
                                                        cmd2.Parameters.Add("@EVENT_STATUS_NAME", Event_incident.event_status_name);
                                                        cmd2.Parameters.Add("@UT", Event_incident.ut);
                                                        cmd2.Parameters.Add("@CTIMESTAMP", DateTime.Now);
                                                        cmd2.Parameters.Add("@TEAMTYPE",
                                                        (Event_incident.participant_id == null || Event_incident.participant_id == "" ? "" : Event_incident.participant_id.ToString() == sevent.participants[0].id.ToString() ? "H" : Event_incident.participant_id.ToString() == sevent.participants[1].id.ToString() ? "G" : "H"));
                                                        int sID = Convert.ToInt32(cmd2.ExecuteScalar());
                                                        Files.WriteLog((sID > -1 ? " [Success] Update INCIDENTS " : " [Failure] Update INCIDENTS ") + "[" + sevent.id + "] " + Event_incident.id + " " + Event_incident.incident_id + " " + Event_incident.incident_name);
                                                    }
                                                }
                                                Files.WriteLog(" Update INCIDENTS " + "[" + sevent.id + "]," + strName + ".xml.");

                                                // DataRow[] drs = eventsDs.Tables[0].Select("id=" + sevent.id);
                                                //{ 
                                                //}
                                                //else
                                                //{
                                                //    Files.WriteLog("[" + drs[0]["id"] + "]   events existed.");
                                                //}
                                            }
                                        }
                                    }
                                }

                            }

                            queryString = "SELECT t.* FROM teams t  inner  join  events e on   t.id= e.HOME_ID or t.id=e.GUEST_ID   where e.id='" + arr[1] + "' order by id asc";
                            //queryString = "SELECT t.ID ,t.NAME ,t.SHORT_NAME ,t.ACRONYM,t.season_id ,t.AREA_ID ,t.ctimestamp FROM teams t  inner  join  events e on   t.id= e.HOME_ID or t.id=e.GUEST_ID   where e.id='" + arr[1] + "' order by id asc";
                            using (FbCommand cmdA = new FbCommand(queryString))
                            {
                                using (FbCommandBuilder fcb = new FbCommandBuilder())
                                {
                                    using (FbDataAdapter fda = new FbDataAdapter())
                                    {
                                        cmdA.Connection = connection;
                                        fda.SelectCommand = cmdA;
                                        fcb.DataAdapter = fda;
                                        using (DataSet data = new DataSet())
                                        {
                                            data.Tables.Add(new DataTable("teams"));
                                            fda.Fill(data.Tables["teams"]);

                                            // queryString = "SELECT p.* FROM teams t  inner join  events e on t.id = e.HOME_ID or t.id = e.GUEST_ID    inner join players p on t.id = p.TEAM_ID   where e.id = '" + arr[1] + "' order by p.id asc";
                                            //queryString = "SELECT p.*FROM players p inner join  events e on p.TEAM_ID = e.HOME_ID or p.TEAM_ID = e.GUEST_ID where e.id='" + arr[1] + "' order by p.id asc";
                                            queryString = "SELECT  p.*   FROM players p inner join  events e on p.TEAM_ID = e.HOME_ID where e.id='" + arr[1] + "' order by p.id asc";
                                            data.Tables.Add(new DataTable("hplayers"));
                                            FbDataAdapter adapter1 = new FbDataAdapter();
                                            adapter1.SelectCommand = new FbCommand(queryString, connection);
                                            FbCommandBuilder builder2 = new FbCommandBuilder(adapter1);
                                            adapter1.Fill(data.Tables["hplayers"]);

                                            queryString = "SELECT  p.* FROM players p inner join  events e on  p.TEAM_ID = e.GUEST_ID where e.id='" + arr[1] + "' order by p.id asc";
                                            data.Tables.Add(new DataTable("gplayers"));
                                            FbDataAdapter adapter2 = new FbDataAdapter();
                                            adapter2.SelectCommand = new FbCommand(queryString, connection);
                                            FbCommandBuilder builder3 = new FbCommandBuilder(adapter1);
                                            adapter2.Fill(data.Tables["gplayers"]);

                                            //if ((Convert .ToBoolean (arr[0])==false) ||data.Tables["teams"].Rows.Count == 0 ||( data.Tables["hplayers"].Rows.Count == 0 && data.Tables["gplayers"].Rows.Count == 0))
                                            //{
                                            //for (int i = 1; i < 2; i++)
                                            //{
                                            //    //if (Convert.ToBoolean(arr[0]) == false) { ds = eventsDs; break; }

                                            //    var responseValue = clientTest.GetAccessData(strToken, "events/" + i, arr[0], arr[1]);
                                            //    var strResponseValue = responseValue.Result;
                                            //    //XDocument document = XDocument.Load("E:\\Project\\AppProject\\DataOfScouts\\DataOfScouts\\bin\\Debug\\New folder\\events-1153436.xml");
                                            //    //var strResponseValue = document.ToString();

                                            //    if (strResponseValue == "Unauthorized") { MessageBox.Show("Unauthorized!"); break; }
                                            //    string strName =arr[1].ToString ()+ type + "-" + i + " " + DateTime.Now.ToString("HHmmss");
                                            //    Files.WriteXml(strName, strResponseValue);

                                            //    DOSEvents2.api apis = XmlUtil.Deserialize(typeof(DOSEvents2.api), strResponseValue) as DOSEvents2.api;
                                            //    if (apis == null) break;
                                            //    DOSEvents2.apiDataCompetition  competition = (apis.data.Length == 0) ? null : apis.data[0];
                                            //    if (competition == null) break;

                                            //    //foreach (DOSEvents2.apiDataCompetition competition in competitions)
                                            //    //{
                                            //        string strCompetition_id = competition.id;
                                            //        string strArea_id = competition.area_id;
                                            //    connection.Open();
                                            //    using (FbCommand cmd = new FbCommand())
                                            //        {
                                            //            //                                        'ID', 'NAME', 'SHORT_NAME', 'MINI_NAME', 'GENDER',
                                            //            //'CTYPE', 'AREA_ID', 'AREA_TYPE', 'AREA_SORT', 'OVERALL_SORT', 'SPORT_ID',
                                            //            //'SPORT_NAME', 'TOUR_ID', 'TOUR_NAME', 'UT', 'OLD_COMPETITION_ID', 'SLUG', 'CTIMESTAMP'
                                            //            cmd.CommandText = "ADD_COMPETITION";
                                            //            cmd.CommandType = CommandType.StoredProcedure;
                                            //            cmd.Connection = connection;
                                            //            cmd.Parameters.Add("@ID", strCompetition_id);
                                            //            cmd.Parameters.Add("@NAME", competition.name);
                                            //            cmd.Parameters.Add("@SHORT_NAME", competition.short_name);
                                            //            cmd.Parameters.Add("@MINI_NAME", competition.mini_name);
                                            //            cmd.Parameters.Add("@GENDER", (competition.gender.ToLower() == "male") ? true : false);
                                            //            cmd.Parameters.Add("@CTYPE", competition.type);
                                            //            cmd.Parameters.Add("@AREA_ID", competition.area_id);
                                            //            cmd.Parameters.Add("@AREA_TYPE", competition.type);
                                            //            cmd.Parameters.Add("@AREA_SORT", competition.area_sort);
                                            //            cmd.Parameters.Add("@OVERALL_SORT", competition.overall_sort);
                                            //            cmd.Parameters.Add("@TOUR_ID", competition.tour_id == "" ? "-1" : competition.tour_id);
                                            //            cmd.Parameters.Add("@TOUR_NAME", competition.tour_name);
                                            //            cmd.Parameters.Add("@UT", competition.ut);
                                            //            cmd.Parameters.Add("@OLD_COMPETITION_ID", competition.old_competition_id == "" ? "-1" : competition.old_competition_id);
                                            //            cmd.Parameters.Add("@SLUG", competition.slug);
                                            //            cmd.Parameters.Add("@CTIMESTAMP", cTimestamp);
                                            //            int id = Convert.ToInt32(cmd.ExecuteScalar());
                                            //        }


                                            //        DOSEvents2.apiDataCompetitionSeason[] seasons = competition.seasons;
                                            //        if (seasons == null) continue;
                                            //        foreach (DOSEvents2.apiDataCompetitionSeason season in seasons)
                                            //        {
                                            //            DOSEvents2.apiDataCompetitionSeasonStage[] stages = season.stages;
                                            //            if (stages == null) continue;

                                            //            foreach (DOSEvents2.apiDataCompetitionSeasonStage stage in stages)
                                            //            {
                                            //                DOSEvents2.apiDataCompetitionSeasonStageGroup[] groups = stage.groups;
                                            //                if (groups == null) continue;
                                            //                foreach (DOSEvents2.apiDataCompetitionSeasonStageGroup group in groups)
                                            //                {
                                            //                    DOSEvents2.apiDataCompetitionSeasonStageGroupEvent[] events = group.events;
                                            //                    if (events == null) continue;
                                            //                    foreach (DOSEvents2.apiDataCompetitionSeasonStageGroupEvent sevent in events)
                                            //                    {
                                            //                        if (sevent == null) continue;

                                            //                        foreach (DOSEvents2.apiDataCompetitionSeasonStageGroupEventParticipant participant in sevent.participants )
                                            //                        {
                                            //                            if (participant == null) continue;
                                            //                        using (FbCommand cmd2 = new FbCommand())
                                            //                        {
                                            //                            cmd2.CommandText = "ADD_TEAM";
                                            //                            cmd2.CommandType = CommandType.StoredProcedure;
                                            //                            cmd2.Connection = connection;
                                            //                            cmd2.Parameters.Add("@ID", participant.id);
                                            //                            cmd2.Parameters.Add("@NAME", participant.name);
                                            //                            cmd2.Parameters.Add("@SHORT_NAME", participant.short_name);
                                            //                            cmd2.Parameters.Add("@ACRONYM", participant.acronym);
                                            //                            cmd2.Parameters.Add("@GENDER", (participant.gender.ToLower() == "male") ? true : false);
                                            //                            cmd2.Parameters.Add("@AREA_ID", participant.area_id);
                                            //                            cmd2.Parameters.Add("@BNATIONAL", (participant.national.ToLower() == "male") ? true : false);
                                            //                            cmd2.Parameters.Add("@UT", participant.ut);
                                            //                            cmd2.Parameters.Add("@OLD_PARTICIPANT_ID", participant.old_participant_id == "" ? "-1" : participant.old_participant_id);
                                            //                            cmd2.Parameters.Add("@SLUG", participant.slug);
                                            //                            cmd2.Parameters.Add("@SEASON_ID", "-1");
                                            //                            cmd2.Parameters.Add("@CTIMESTAMP", cTimestamp);
                                            //                            int id = Convert.ToInt32(cmd2.ExecuteScalar());
                                            //                            Files.WriteLog(id > 0 ? " [Success] Insert teams[" + count + "  " + "] " + " " + strName + ".xml" : " team exist.");
                                            //                        }
                                            //                        foreach (DOSEvents2.apiDataCompetitionSeasonStageGroupEventParticipantLineup lineup in participant.lineups)
                                            //                            {
                                            //                                if (lineup == null) continue;

                                            //                                using (FbCommand cmd2 = new FbCommand())
                                            //                                {
                                            //                                    cmd2.CommandText = "ADD_Player2";
                                            //                                    cmd2.CommandType = CommandType.StoredProcedure;
                                            //                                    cmd2.Connection = connection;
                                            //                                    cmd2.Parameters.Add("@ID", lineup.participant_id );
                                            //                                    cmd2.Parameters.Add("@NAME", lineup.participant_name);
                                            //                                    cmd2.Parameters.Add("@AREA_ID", lineup.participant_area_id);
                                            //                                    cmd2.Parameters.Add("@SLUG", lineup.participant_slug);
                                            //                                    cmd2.Parameters.Add("@BENCH", lineup.bench.ToLower ()=="yes"? true:false);
                                            //                                    cmd2.Parameters.Add("@SHIRT_NR", lineup.shirt_nr==""?"-1": lineup.shirt_nr); 
                                            //                                    cmd2.Parameters.Add("@TEAM_ID", participant.id);
                                            //                                    cmd2.Parameters.Add("@SEASON_ID", season.id);
                                            //                                    cmd2.Parameters.Add("@CTIMESTAMP", cTimestamp);
                                            //                                    int id = Convert.ToInt32(cmd2.ExecuteScalar());
                                            //                                    Files.WriteLog(id > 0 ? " [Success] Insert players2 [" + count + "  " + "] " + " " + strName + ".xml" : " player exist.");
                                            //                                }
                                            //                            }
                                            //                        }
                                            //                        // DataRow[] drs = eventsDs.Tables[0].Select("id=" + sevent.id);
                                            //                        //{ 
                                            //                        //}
                                            //                        //else
                                            //                        //{
                                            //                        //    Files.WriteLog("[" + drs[0]["id"] + "]   events existed.");
                                            //                        //}
                                            //                    }
                                            //                }
                                            //            }
                                            //        }
                                            //    }

                                            //count = adapter.Update(eventsDs);
                                            //ds.Merge(eventsDs, true, MissingSchemaAction.AddWithKey);
                                            //eventsDs.Clear();

                                            //if (count > -1)
                                            //{
                                            //    Files.WriteLog("[Success] Insert events[" + count + "  " + "] " + " " + strName + ".xml");
                                            //}
                                            //else
                                            //{
                                            //    Files.WriteLog("[Failure] Insert events [  ]" + " " + strName + ".xml");
                                            //}
                                            //    }
                                            ////}
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

                        //using (FbConnection connection = new FbConnection(AppFlag.ScoutsDBConn))
                        //{
                        //    //  queryString = "select * from " + type + " where  COMPETITION_ID=" + arr[0] +" and booked=false";
                        //    //queryString = "select * from " + type;// + " where booked = false";
                        //    //start_date  BETWEEN  '9/19/2018 00:00:00' and'9/20/2018  10:59:59'
                        //    if (Convert.ToBoolean(arr[0]))
                        //    {
                        //        // queryString = "select e.ID, e.NAME, e.HOME_ID, e.GUEST_ID, e.START_DATE,  e.STATUS_NAME,    e.STATUS_TYPE,    e.ROUND_NAME,     e.BOOKED,  e.GROUP_ID, e.STAGE_ID, e.SEASON_ID, e.COMPETITION_ID,    e.AREA_ID, e.CTIMESTAMP from events e where '" + arr[1] + "'<=  e.start_date and  e.start_date <='" + arr[2] + "'";
                        //        queryString = "select e.* from events e where '" + arr[1] + "'<=  e.start_date and  e.start_date <='" + arr[2] + "' order by  e.start_date asc";
                        //    }
                        //    else
                        //    {
                        //        if (arr[1].ToString().Length < 12)
                        //        {
                        //            //  queryString = "select e.ID, e.NAME, e.HOME_ID, e.GUEST_ID, e.START_DATE,  e.STATUS_NAME,    e.STATUS_TYPE,    e.ROUND_NAME,     e.BOOKED,  e.GROUP_ID, e.STAGE_ID, e.SEASON_ID, e.COMPETITION_ID,    e.AREA_ID, e.CTIMESTAMP from events e where " + (arr[1].ToString() == "Group" ? " e.GROUP_ID" : (arr[1].ToString() == "Stage") ? " e.STAGE_ID" : (arr[1].ToString() == "Season") ? " e.SEASON_ID" : (arr[1].ToString() == "Comp") ? " e.COMPETITION_ID" : "") + "='" + arr[2] + "'";
                        //            queryString = "select e.* from events e where " + (arr[1].ToString() == "Group" ? " e.GROUP_ID" : (arr[1].ToString() == "Stage") ? " e.STAGE_ID" : (arr[1].ToString() == "Season") ? " e.SEASON_ID" : (arr[1].ToString() == "Comp") ? " e.COMPETITION_ID" : "") + "='" + arr[2] + "'  order by  e.start_date asc";
                        //        }
                        //        else
                        //        {
                        //            // queryString = "select e.ID, e.NAME, e.HOME_ID, e.GUEST_ID, e.START_DATE,  e.STATUS_NAME,    e.STATUS_TYPE,    e.ROUND_NAME,     e.BOOKED,  e.GROUP_ID, e.STAGE_ID, e.SEASON_ID, e.COMPETITION_ID,    e.AREA_ID, e.CTIMESTAMP from events e where '" + arr[1] + "'<= e.start_date and  e.start_date <='" + arr[2] + "'";
                        //            queryString = "select e.* from events e where '" + arr[1] + "'<= e.start_date and  e.start_date <='" + arr[2] + "'  order by  e.start_date asc";
                        //        }
                        //    }
                        //    FbDataAdapter adapter = new FbDataAdapter();
                        //    adapter.SelectCommand = new FbCommand(queryString, connection);
                        //    FbCommandBuilder builder = new FbCommandBuilder(adapter);
                        //    connection.Open();
                        //    DataSet eventsDs = new DataSet();
                        //    adapter.Fill(eventsDs);
                        //    if (Convert.ToBoolean(arr[0]) && DateTime.Now <= Convert.ToDateTime(arr[2]) && eventsDs.Tables[0].Rows.Count == 0)
                        //    // if (Convert.ToBoolean(arr[0]))
                        //    //if (DateTime.Now <= Convert.ToDateTime(arr[2]))
                        //    {
                        //        //if (eventsDs.Tables[0].Rows.Count == 0)
                        //        //{
                        //        //DataSet newEventsDs= eventsDs.Clone ();
                        //        for (int i = 1; i < 10; i++)
                        //        {
                        //            //if (Convert.ToBoolean(arr[0]) == false) { ds = eventsDs; break; }

                        //            var responseValue = clientTest.GetAccessData(strToken, "events/" + i, arr[1], arr[2]);
                        //            var strResponseValue = responseValue.Result;
                        //            //XDocument document = XDocument.Load("E:\\Project\\AppProject\\DataOfScouts\\DataOfScouts\\bin\\Debug\\New folder\\events-1153436.xml");
                        //            //var strResponseValue = document.ToString();

                        //            if (strResponseValue == "Unauthorized") { MessageBox.Show("Unauthorized!"); break; }
                        //            string strName = type + "-" + i + " " + DateTime.Now.ToString("HHmmss");
                        //            Files.WriteXml(strName, strResponseValue);

                        //            DOSEvents.api apis = XmlUtil.Deserialize(typeof(DOSEvents.api), strResponseValue) as DOSEvents.api;
                        //            if (apis == null) break;
                        //            DOSEvents.apiDataCompetition[] competitions = (apis.data.Length == 0) ? null : apis.data[0];
                        //            if (competitions == null) break;

                        //            foreach (DOSEvents.apiDataCompetition competition in competitions)
                        //            {
                        //                string strCompetition_id = competition.id;
                        //                string strArea_id = competition.area_id;

                        //                using (FbCommand cmd = new FbCommand())
                        //                {
                        //                    //                                        'ID', 'NAME', 'SHORT_NAME', 'MINI_NAME', 'GENDER',
                        //                    //'CTYPE', 'AREA_ID', 'AREA_TYPE', 'AREA_SORT', 'OVERALL_SORT', 'SPORT_ID',
                        //                    //'SPORT_NAME', 'TOUR_ID', 'TOUR_NAME', 'UT', 'OLD_COMPETITION_ID', 'SLUG', 'CTIMESTAMP'
                        //                    cmd.CommandText = "ADD_COMPETITION";
                        //                    cmd.CommandType = CommandType.StoredProcedure;
                        //                    cmd.Connection = connection;
                        //                    cmd.Parameters.Add("@ID", strCompetition_id);
                        //                    cmd.Parameters.Add("@NAME", competition.name);
                        //                    cmd.Parameters.Add("@SHORT_NAME", competition.short_name);
                        //                    cmd.Parameters.Add("@MINI_NAME", competition.mini_name);
                        //                    cmd.Parameters.Add("@GENDER", (competition.gender.ToLower() == "male") ? true : false);
                        //                    cmd.Parameters.Add("@CTYPE", competition.type);
                        //                    cmd.Parameters.Add("@AREA_ID", competition.area_id);
                        //                    cmd.Parameters.Add("@AREA_TYPE", competition.type);
                        //                    cmd.Parameters.Add("@AREA_SORT", competition.area_sort);
                        //                    cmd.Parameters.Add("@OVERALL_SORT", competition.overall_sort);
                        //                    cmd.Parameters.Add("@TOUR_ID", competition.tour_id == "" ? "-1" : competition.tour_id);
                        //                    cmd.Parameters.Add("@TOUR_NAME", competition.tour_name);
                        //                    cmd.Parameters.Add("@UT", competition.ut);
                        //                    cmd.Parameters.Add("@OLD_COMPETITION_ID", competition.old_competition_id == "" ? "-1" : competition.old_competition_id);
                        //                    cmd.Parameters.Add("@SLUG", competition.slug);
                        //                    cmd.Parameters.Add("@CTIMESTAMP", cTimestamp);
                        //                    int id = Convert.ToInt32(cmd.ExecuteScalar());
                        //                }


                        //                DOSEvents.apiDataCompetitionSeason[] seasons = competition.seasons;
                        //                if (seasons == null) continue;
                        //                foreach (DOSEvents.apiDataCompetitionSeason season in seasons)
                        //                {
                        //                    string strSeasons_id = season.id;
                        //                    using (FbCommand cmd = new FbCommand())
                        //                    {
                        //                        //r.ID, r.NAME, r.COMPETITION_ID, r.SYEAR, r.ACTUAL, r.UT, r.OLD_SEASON_ID,
                        //                        //r.RANGE, r.CTIMESTAMP
                        //                        cmd.CommandText = "ADD_SEASON";
                        //                        cmd.CommandType = CommandType.StoredProcedure;
                        //                        cmd.Connection = connection;
                        //                        cmd.Parameters.Add("@ID", season.id);
                        //                        cmd.Parameters.Add("@NAME", season.name);
                        //                        cmd.Parameters.Add("@COMPETITION_ID", strCompetition_id);
                        //                        cmd.Parameters.Add("@SYEAR", season.year);
                        //                        cmd.Parameters.Add("@ACTUAL", season.actual);
                        //                        cmd.Parameters.Add("@UT", season.ut);
                        //                        cmd.Parameters.Add("@OLD_SEASON_ID", season.old_season_id == "" ? "-1" : season.old_season_id);
                        //                        cmd.Parameters.Add("@RANGE", season.range);
                        //                        cmd.Parameters.Add("@CTIMESTAMP", cTimestamp);
                        //                        int id = Convert.ToInt32(cmd.ExecuteScalar());
                        //                    }

                        //                    DOSEvents.apiDataCompetitionSeasonStage[] stages = season.stages;
                        //                    if (stages == null) continue;

                        //                    foreach (DOSEvents.apiDataCompetitionSeasonStage stage in stages)
                        //                    {
                        //                        string strStage_id = stage.id == "" ? "-1" : stage.id;
                        //                        if (strStage_id != "-1")
                        //                        {
                        //                            using (FbCommand cmd = new FbCommand())
                        //                            {
                        //                                //r.ID, r.STAGE_NAME_ID, r.NAME, r.START_DATE, r.END_DATE,
                        //                                //r.SHOW_STANDINGS, r.GROUPS_NR, r.ISORT, r.IS_CURRENT, r.UT, r.OLD_STAGE_ID,    r.SEASON_ID, r.COMPETITION_ID, r.AREA_ID, r.CTIMESTAMP
                        //                                cmd.CommandText = "ADD_STAGE";
                        //                                cmd.CommandType = CommandType.StoredProcedure;
                        //                                cmd.Connection = connection;
                        //                                cmd.Parameters.Add("@ID", stage.id);
                        //                                cmd.Parameters.Add("@STAGE_NAME_ID", stage.stage_name_id);
                        //                                cmd.Parameters.Add("@NAME", stage.name);
                        //                                cmd.Parameters.Add("@START_DATE", stage.start_date);
                        //                                cmd.Parameters.Add("@END_DATE", stage.end_date);
                        //                                cmd.Parameters.Add("@SHOW_STANDINGS", stage.show_standings.ToLower() == "yes" ? true : false);
                        //                                cmd.Parameters.Add("@GROUPS_NR", stage.groups_nr == "" ? "-1" : stage.groups_nr);
                        //                                cmd.Parameters.Add("@ISORT", stage.sort == "" ? "-1" : stage.sort);
                        //                                cmd.Parameters.Add("@IS_CURRENT", stage.is_current.ToLower() == "yes" ? true : false);
                        //                                cmd.Parameters.Add("@UT", stage.ut);
                        //                                cmd.Parameters.Add("@OLD_STAGE_ID", stage.old_stage_id == "" ? "-1" : stage.old_stage_id);
                        //                                cmd.Parameters.Add("@SEASON_ID", strSeasons_id);
                        //                                cmd.Parameters.Add("@COMPETITION_ID", strCompetition_id);
                        //                                cmd.Parameters.Add("@AREA_ID", strArea_id);
                        //                                cmd.Parameters.Add("@CTIMESTAMP", cTimestamp);
                        //                                int id = Convert.ToInt32(cmd.ExecuteScalar());
                        //                            }
                        //                        }
                        //                        DOSEvents.apiDataCompetitionSeasonStageGroup[] groups = stage.groups;
                        //                        if (groups == null) continue;
                        //                        foreach (DOSEvents.apiDataCompetitionSeasonStageGroup group in groups)
                        //                        {
                        //                            string strGroup_id = group.id == "" ? "-1" : group.id;
                        //                            if (strGroup_id != "-1")
                        //                            {
                        //                                using (FbCommand cmd = new FbCommand())
                        //                                {
                        //                                    // r.ID, r.NAME, r.UT, r.STAGE_ID, r.SEASON_ID, r.COMPETITION_ID, r.AREA_ID,     r.CTIMESTAMP
                        //                                    cmd.CommandText = "ADD_GROUP";
                        //                                    cmd.CommandType = CommandType.StoredProcedure;
                        //                                    cmd.Connection = connection;
                        //                                    cmd.Parameters.Add("@ID", group.id);
                        //                                    cmd.Parameters.Add("@NAME", group.name);
                        //                                    cmd.Parameters.Add("@UT", group.ut);
                        //                                    cmd.Parameters.Add("@STAGE_ID", strStage_id);
                        //                                    cmd.Parameters.Add("@SEASON_ID", strSeasons_id);
                        //                                    cmd.Parameters.Add("@COMPETITION_ID", strCompetition_id);
                        //                                    cmd.Parameters.Add("@AREA_ID", strArea_id);
                        //                                    cmd.Parameters.Add("@CTIMESTAMP", cTimestamp);
                        //                                    int id = Convert.ToInt32(cmd.ExecuteScalar());
                        //                                }
                        //                            }

                        //                            foreach (DOSEvents.apiDataCompetitionSeasonStageGroupEvent sevent in group.events)
                        //                            {
                        //                                if (sevent == null) continue;
                        //                                DataRow[] drs = eventsDs.Tables[0].Select("id=" + sevent.id);
                        //                                if (eventsDs.Tables[0].Select("id=" + sevent.id).Length == 0)
                        //                                {
                        //                                    DOSEvents.apiDataCompetitionSeasonStageGroupEventParticipant[] participants = sevent.participants;

                        //                                    DataRow dr = eventsDs.Tables[0].NewRow();
                        //                                    dr[0] = sevent.id;
                        //                                    dr[1] = sevent.name;
                        //                                    dr[2] = (participants[0].counter == "1") ? participants[0].id : participants[1].id;
                        //                                    dr[3] = (participants[1].counter == "2") ? participants[1].id : participants[0].id;
                        //                                    dr[4] = sevent.source;
                        //                                    dr[5] = sevent.source_dc == "yes" ? true : false;
                        //                                    dr[6] = sevent.source_super;
                        //                                    dr[7] = sevent.relation_status;
                        //                                    dr[8] = Convert.ToDateTime(sevent.start_date);
                        //                                    dr[9] = sevent.ft_only == "yes" ? true : false;
                        //                                    dr[10] = sevent.coverage_type;
                        //                                    dr[11] = sevent.channel_id;
                        //                                    dr[12] = sevent.channel_name;
                        //                                    dr[13] = sevent.scoutsfeed == "yes" ? true : false;
                        //                                    dr[14] = sevent.status_id;
                        //                                    dr[15] = sevent.status_name;
                        //                                    dr[16] = sevent.status_type;
                        //                                    dr[17] = sevent.day;
                        //                                    dr[18] = sevent.clock_time;
                        //                                    dr[19] = sevent.clock_status;
                        //                                    dr[20] = sevent.winner_id;
                        //                                    dr[21] = sevent.progress_id;
                        //                                    dr[22] = sevent.bet_status;
                        //                                    dr[23] = sevent.neutral_venue == "yes" ? true : false;
                        //                                    dr[24] = sevent.item_status;
                        //                                    dr[25] = sevent.ut;
                        //                                    dr[26] = sevent.old_event_id == "" ? "-1" : sevent.old_event_id;
                        //                                    dr[27] = sevent.slug;
                        //                                    dr[28] = sevent.verified_result == "yes" ? true : false;
                        //                                    dr[29] = sevent.is_protocol_verified == "yes" ? true : false;
                        //                                    dr[30] = sevent.protocol_verified_by;
                        //                                    dr[31] = sevent.protocol_verified_at;
                        //                                    dr[32] = sevent.round_id;
                        //                                    dr[33] = sevent.round_name;
                        //                                    dr[34] = sevent.client_event_id == "" ? "-1" : sevent.client_event_id;
                        //                                    dr[35] = sevent.booked == "yes" ? true : false;
                        //                                    dr[36] = sevent.booked_by;
                        //                                    dr[37] = sevent.inverted_participants == "yes" ? true : false;
                        //                                    dr[38] = sevent.venue_id;
                        //                                    dr[39] = group.id == "" ? "-1" : group.id;
                        //                                    dr[40] = strStage_id;
                        //                                    dr[41] = strSeasons_id;
                        //                                    dr[42] = strCompetition_id;
                        //                                    dr[43] = strArea_id;
                        //                                    dr[44] = cTimestamp;
                        //                                    eventsDs.Tables[0].Rows.Add(dr);
                        //                                }
                        //                                else
                        //                                {
                        //                                    Files.WriteLog("[" + drs[0]["id"] + "]   events existed.");
                        //                                }
                        //                            }
                        //                        }
                        //                    }
                        //                }
                        //            }

                        //            count = adapter.Update(eventsDs);
                        //            ds.Merge(eventsDs, true, MissingSchemaAction.AddWithKey);
                        //            eventsDs.Clear();

                        //            if (count > -1)
                        //            {
                        //                Files.WriteLog("[Success] Insert events[" + count + "  " + "] " + " " + strName + ".xml");
                        //            }
                        //            else
                        //            {
                        //                Files.WriteLog("[Failure] Insert events [  ]" + " " + strName + ".xml");
                        //            }
                        //        }
                        //        //}
                        //        //else
                        //        //{
                        //        //    //count = -1;
                        //        //    //queryString = "select   * from " + type;
                        //        //    //adapter.SelectCommand = new FbCommand(queryString, connection);
                        //        //    //adapter.Fill(eventsDs);
                        //        //count = adapter.Update(eventsDs);
                        //        //ds = eventsDs;
                        //        //}
                        //    }
                        //    else
                        //    {
                        //        ds = eventsDs;
                        //    }
                        //    connection.Close();
                        //}
                        break;
                    }
                case "events.show3":
                    {
                        using (FbConnection connection = new FbConnection(AppFlag.ScoutsDBConn))
                        {
                            connection.Open();
                            for (int i = 1; i < 2 && Convert.ToBoolean(arr[0]) == true && arr[1].ToString() != ""; i++)
                            {
                                var responseValue = clientTest.GetAccessData(strToken, "events.show", arr[1]);
                                var strResponseValue = responseValue.Result;
                                //XDocument document = XDocument.Load("E:\\Project\\AppProject\\DataOfScouts\\DataOfScouts\\bin\\Debug\\New folder\\2492172_events.show3-1_075138.xml");
                                //var strResponseValue = document.ToString();
                                if (strResponseValue == "Unauthorized") { MessageBox.Show("Unauthorized!"); break; }
                                string strName = arr[1].ToString() + "_" + type + "-" + i + "_" + DateTime.Now.ToString("HHmmss");

                                DOSEvents2.api apis = XmlUtil.Deserialize(typeof(DOSEvents2.api), strResponseValue) as DOSEvents2.api;
                                if (apis == null) break;

                                Files.WriteXml(strName, strResponseValue);
                                Files.WriteLog("Get " + strName + ".xml.");

                                DOSEvents2.apiDataCompetition competition = (apis.data == null || apis.data.Length == 0) ? null : apis.data[0];
                                if (competition == null) break;

                                string strCompetition_id = competition.id;
                                string strArea_id = competition.area_id;

                                DOSEvents2.apiDataCompetitionSeason[] seasons = competition.seasons;
                                if (seasons == null) continue;
                                foreach (DOSEvents2.apiDataCompetitionSeason season in seasons)
                                {
                                    DOSEvents2.apiDataCompetitionSeasonStage[] stages = season.stages;
                                    if (stages == null) continue;

                                    foreach (DOSEvents2.apiDataCompetitionSeasonStage stage in stages)
                                    {
                                        DOSEvents2.apiDataCompetitionSeasonStageGroup[] groups = stage.groups;
                                        if (groups == null) continue;
                                        foreach (DOSEvents2.apiDataCompetitionSeasonStageGroup group in groups)
                                        {
                                            DOSEvents2.apiDataCompetitionSeasonStageGroupEvent[] events = group.events;
                                            if (events == null) continue;
                                            foreach (DOSEvents2.apiDataCompetitionSeasonStageGroupEvent sevent in events)
                                            {
                                                if (sevent == null) continue;

                                                string sName = "";
                                                string sPlayers = "";

                                                using (FbCommand cmd2 = new FbCommand("SELECT  r.EMATCHID , r.HKJCDAYCODE, r.HKJCMATCHNO,r.CMATCHDATETIME FROM EMATCHES r where r.EMATCHID='" + sevent.id + "'", connection))
                                                {
                                                    FbDataReader reader = cmd2.ExecuteReader();
                                                    while (reader.Read())
                                                    {
                                                        var data = (IDataRecord)reader;
                                                        sName = data[1] + "-" + data[2] + "-" + Convert.ToDateTime(data[3]).ToString("yyyyMMdd") + "-" + data[0];
                                                    }
                                                    reader.Close();
                                                }
                                                foreach (DOSEvents2.apiDataCompetitionSeasonStageGroupEventParticipant participant in sevent.participants)
                                                {
                                                    if (participant == null) continue;
                                                    //using (FbCommand cmd2 = new FbCommand())
                                                    //{
                                                    //    cmd2.CommandText = "ADD_TEAM";
                                                    //    cmd2.CommandType = CommandType.StoredProcedure;
                                                    //    cmd2.Connection = connection;
                                                    //    cmd2.Parameters.Add("@ID", participant.id);
                                                    //    cmd2.Parameters.Add("@NAME", participant.name);
                                                    //    cmd2.Parameters.Add("@SHORT_NAME", participant.short_name);
                                                    //    cmd2.Parameters.Add("@ACRONYM", participant.acronym);
                                                    //    cmd2.Parameters.Add("@GENDER", (participant.gender.ToLower() == "male") ? true : false);
                                                    //    cmd2.Parameters.Add("@AREA_ID", participant.area_id);
                                                    //    cmd2.Parameters.Add("@BNATIONAL", (participant.national.ToLower() == "male") ? true : false);
                                                    //    cmd2.Parameters.Add("@UT", participant.ut);
                                                    //    cmd2.Parameters.Add("@OLD_PARTICIPANT_ID", participant.old_participant_id == "" ? "-1" : participant.old_participant_id);
                                                    //    cmd2.Parameters.Add("@SLUG", participant.slug);
                                                    //    cmd2.Parameters.Add("@SEASON_ID", "-1");
                                                    //    cmd2.Parameters.Add("@CTIMESTAMP", cTimestamp);
                                                    //    int id = Convert.ToInt32(cmd2.ExecuteScalar());
                                                    //    //  Files.WriteLog(id > 0 ? " [Success] Insert teams[" + participant.id + "] " + participant.name + " " + strName + ".xml" : " team exist.");
                                                    //    Files.WriteLog(id > 0 ? " [Success] Insert teams[" + participant.id + "] " + participant.name : "[" + participant.id + "] " + participant.name + " team exist.");
                                                    //}

                                                    sPlayers += (participant.lineups == null || participant.lineups.Count() == 0) ? "" : "\r\n" + (participant.counter == "1" ? "Host\r\n" : "Guest\r\n");

                                                    foreach (DOSEvents2.apiDataCompetitionSeasonStageGroupEventParticipantLineup lineup in participant.lineups)
                                                    {
                                                        if (lineup == null) continue;

                                                        sPlayers += (lineup.shirt_nr == "" ? "-1" : lineup.shirt_nr) + "  " + lineup.participant_name + "  " + (lineup.bench.ToLower() == "yes" ? "/Bench" : "     ") + "  " + (lineup.type.ToLower() == "coach" ? "/Coach" : "") + "\r\n";

                                                        using (FbCommand cmd2 = new FbCommand())
                                                        {
                                                            cmd2.CommandText = "ADD_Player2";
                                                            cmd2.CommandType = CommandType.StoredProcedure;
                                                            cmd2.Connection = connection;
                                                            cmd2.Parameters.Add("@ID", lineup.participant_id == "" ? "-1" : lineup.participant_id);
                                                            cmd2.Parameters.Add("@NAME", lineup.participant_name);
                                                            cmd2.Parameters.Add("@AREA_ID", lineup.participant_area_id == "" ? "-1" : lineup.participant_area_id);
                                                            cmd2.Parameters.Add("@SLUG", lineup.participant_slug);
                                                            cmd2.Parameters.Add("@BENCH", lineup.bench.ToLower() == "yes" ? true : false);
                                                            cmd2.Parameters.Add("@SHIRT_NR", lineup.shirt_nr == "" || lineup.shirt_nr == "-" ? "-1" : lineup.shirt_nr.Replace("-", ""));
                                                            cmd2.Parameters.Add("@TEAM_ID", participant.id);
                                                            cmd2.Parameters.Add("@SEASON_ID", season.id);
                                                            cmd2.Parameters.Add("@CTIMESTAMP", cTimestamp);
                                                            int id = Convert.ToInt32(cmd2.ExecuteScalar());
                                                            Files.WriteLog(id > 0 ? " [Success] Insert players [" + lineup.participant_id + "] " + lineup.participant_name : "[" + lineup.participant_id + "] " + lineup.participant_name + " player exist.");
                                                        }
                                                    }
                                                }

                                                // if(sName!="" && Convert.ToBoolean(arr[2])&& sPlayers!=""&&DateTime.Now<Convert.ToDateTime(sevent.start_date))  Files.WriteLog(sName, true, sPlayers);
                                                if (sName != "" && Convert.ToBoolean(arr[2]) && sPlayers != "")
                                                {
                                                    // if (DateTime.Now < Convert.ToDateTime(sevent.start_date).AddHours(1))
                                                    if (sevent.status_id == "1" || sevent.status_id == "6" || DateTime.Now < Convert.ToDateTime(sevent.start_date).AddHours(1) || sevent.status_id == "33")
                                                    {
                                                        Files.WriteLog(sName, true, sevent.start_date + "\r\n" + sPlayers);
                                                    }
                                                    else
                                                    {
                                                        Files.WriteLog(sName + "_old", true, sevent.start_date + "\r\n" + sPlayers);
                                                    }
                                                }

                                                if (sevent.participants.Length == 2 && sevent.participants[0].stats.Length > 0 && sevent.participants[0].results.Length > 0)
                                                {
                                                    using (FbCommand cmd2 = new FbCommand())
                                                    {
                                                        cmd2.CommandText = "PR_stats_GoalInfo";
                                                        cmd2.CommandType = CommandType.StoredProcedure;
                                                        cmd2.Connection = connection;
                                                        cmd2.Parameters.Add("@EMATCHID", sevent.id);
                                                        cmd2.Parameters.Add("@H_YELLOW", sevent.participants[0].counter == "1" ? sevent.participants[0].stats.FirstOrDefault(c => c.id == "8").value == "" ? null : sevent.participants[0].stats.FirstOrDefault(c => c.id == "8").value : sevent.participants[1].stats.FirstOrDefault(c => c.id == "8").value == "" ? null : sevent.participants[1].stats.FirstOrDefault(c => c.id == "8").value);
                                                        cmd2.Parameters.Add("@H_RED", sevent.participants[0].counter == "1" ? sevent.participants[0].stats.FirstOrDefault(c => c.id == "9").value == "" ? null : sevent.participants[0].stats.FirstOrDefault(c => c.id == "9").value : sevent.participants[1].stats.FirstOrDefault(c => c.id == "9").value == "" ? null : sevent.participants[1].stats.FirstOrDefault(c => c.id == "9").value);
                                                        cmd2.Parameters.Add("@G_YELLOW", sevent.participants[1].counter == "2" ? sevent.participants[1].stats.FirstOrDefault(c => c.id == "8").value == "" ? null : sevent.participants[1].stats.FirstOrDefault(c => c.id == "8").value : sevent.participants[0].stats.FirstOrDefault(c => c.id == "8").value == "" ? null : sevent.participants[0].stats.FirstOrDefault(c => c.id == "8").value);
                                                        cmd2.Parameters.Add("@G_RED", sevent.participants[1].counter == "2" ? sevent.participants[1].stats.FirstOrDefault(c => c.id == "9").value == "" ? null : sevent.participants[1].stats.FirstOrDefault(c => c.id == "9").value : sevent.participants[0].stats.FirstOrDefault(c => c.id == "9").value == "" ? null : sevent.participants[0].stats.FirstOrDefault(c => c.id == "9").value);
                                                        cmd2.Parameters.Add("@LASTTIME", DateTime.Now);
                                                        int id = Convert.ToInt32(cmd2.ExecuteScalar());
                                                        Files.WriteLog((id > 0 ? " [Success] Update GoalInfo " : " [Failure]  Update GoalInfo ") + "[" + sevent.id + "] " + sevent.participants[0].id + "/" + sevent.participants[1].id + "..");
                                                    }

                                                    using (FbCommand cmd2 = new FbCommand())
                                                    {
                                                        cmd2.CommandText = "PR_Result_GoalInfo";
                                                        cmd2.CommandType = CommandType.StoredProcedure;
                                                        cmd2.Connection = connection;
                                                        cmd2.Parameters.Add("@EMATCHID", sevent.id);
                                                        cmd2.Parameters.Add("@H_GOAL", sevent.participants[0].counter == "1" ? sevent.participants[0].results.FirstOrDefault(c => c.id == "2").value == "" ? null : sevent.participants[0].results.FirstOrDefault(c => c.id == "2").value : sevent.participants[1].stats.FirstOrDefault(c => c.id == "2").value == "" ? null : sevent.participants[1].stats.FirstOrDefault(c => c.id == "2").value);
                                                        cmd2.Parameters.Add("@G_GOAL", sevent.participants[1].counter == "2" ? sevent.participants[1].results.FirstOrDefault(c => c.id == "2").value == "" ? null : sevent.participants[1].results.FirstOrDefault(c => c.id == "2").value : sevent.participants[0].stats.FirstOrDefault(c => c.id == "2").value == "" ? null : sevent.participants[0].stats.FirstOrDefault(c => c.id == "2").value);
                                                        cmd2.Parameters.Add("@LASTTIME", DateTime.Now);
                                                        int id = Convert.ToInt32(cmd2.ExecuteScalar());
                                                        Files.WriteLog((id > 0 ? " [Success] Update GoalInfo Goal " : " [Failure]  Update GoalInfo Goal") + "[" + sevent.id + "] " + sevent.participants[0].id + "/" + sevent.participants[1].id + "..");
                                                    }
                                                }
                                                DOSEvents2.apiDataCompetitionSeasonStageGroupEventEvent_incident[] events_incidents = sevent.events_incidents;
                                                if (events_incidents == null || events_incidents.Count() == 0) continue;
                                                foreach (DOSEvents2.apiDataCompetitionSeasonStageGroupEventEvent_incident Event_incident in sevent.events_incidents)
                                                {
                                                    if (Event_incident == null) continue;

                                                    using (FbCommand cmd2 = new FbCommand())
                                                    {
                                                        cmd2.CommandText = "PR_INCIDENTS";
                                                        cmd2.CommandType = CommandType.StoredProcedure;
                                                        cmd2.Connection = connection;
                                                        cmd2.Parameters.Add("@ID", Event_incident.id);
                                                        cmd2.Parameters.Add("@EVENTID", sevent.id);
                                                        cmd2.Parameters.Add("@CACTION", "update");
                                                        cmd2.Parameters.Add("@INCIDENT_ID", Event_incident.incident_id);
                                                        cmd2.Parameters.Add("@INCIDENT_NAME", Event_incident.incident_name);
                                                        cmd2.Parameters.Add("@PARTICIPANT_ID", Event_incident.participant_id);
                                                        cmd2.Parameters.Add("@PARTICIPANT_NAME", Event_incident.participant_name);
                                                        cmd2.Parameters.Add("@SUBPARTICIPANT_ID", Event_incident.subparticipant_id);
                                                        cmd2.Parameters.Add("@SUBPARTICIPANT_NAME", Event_incident.subparticipant_name);
                                                        cmd2.Parameters.Add("@IMPORTANT_FOR_TRADER", true);// incidentJson.data.incident.important_for_trader == "yes" ? true : false);
                                                        cmd2.Parameters.Add("@EVENT_TIME", Event_incident.event_time);
                                                        cmd2.Parameters.Add("@EVENT_STATUS_ID", Event_incident.event_status_id);
                                                        cmd2.Parameters.Add("@EVENT_STATUS_NAME", Event_incident.event_status_name);
                                                        cmd2.Parameters.Add("@UT", Event_incident.ut);
                                                        cmd2.Parameters.Add("@CTIMESTAMP", DateTime.Now);
                                                        cmd2.Parameters.Add("@TEAMTYPE",
                                                        (Event_incident.participant_id == null || Event_incident.participant_id == "" ? "" : Event_incident.participant_id.ToString() == sevent.participants[0].id.ToString() ? "H" : Event_incident.participant_id.ToString() == sevent.participants[1].id.ToString() ? "G" : "H"));
                                                        int sID = Convert.ToInt32(cmd2.ExecuteScalar());
                                                        Files.WriteLog((sID > -1 ? " [Success] Update INCIDENTS " : " [Failure] Update INCIDENTS ") + "[" + sevent.id + "] " + Event_incident.id + " " + Event_incident.incident_id + " " + Event_incident.incident_name);
                                                    }
                                                }
                                                Files.WriteLog(" Update INCIDENTS " + "[" + sevent.id + "]," + strName + ".xml");

                                                //if (sevent.participants.Length == 2)
                                                //{
                                                //    using (FbCommand cmd2 = new FbCommand())
                                                //    {
                                                //        cmd2.CommandText = "PR_stats_GoalInfo";
                                                //        cmd2.CommandType = CommandType.StoredProcedure;
                                                //        cmd2.Connection = connection;
                                                //        cmd2.Parameters.Add("@EMATCHID", sevent.id);
                                                //        cmd2.Parameters.Add("@H_YELLOW", sevent.participants[0].counter == "1" ? sevent.participants[0].stats.FirstOrDefault(c => c.id == "8").value : sevent.participants[1].stats.FirstOrDefault(c => c.id == "8").value);
                                                //        cmd2.Parameters.Add("@H_RED", sevent.participants[0].counter == "1" ? sevent.participants[0].stats.FirstOrDefault(c => c.id == "9").value : sevent.participants[1].stats.FirstOrDefault(c => c.id == "9").value);
                                                //        cmd2.Parameters.Add("@G_YELLOW", sevent.participants[1].counter == "2" ? sevent.participants[1].stats.FirstOrDefault(c => c.id == "8").value : sevent.participants[0].stats.FirstOrDefault(c => c.id == "8").value);
                                                //        cmd2.Parameters.Add("@G_RED", sevent.participants[1].counter == "2" ? sevent.participants[1].stats.FirstOrDefault(c => c.id == "9").value : sevent.participants[0].stats.FirstOrDefault(c => c.id == "9").value);
                                                //        cmd2.Parameters.Add("@LASTTIME", DateTime.Now);
                                                //        int id = Convert.ToInt32(cmd2.ExecuteScalar());
                                                //        Files.WriteLog((id > 0 ? " [Success] Update GoalInfo " : " [Failure]  Update GoalInfo ") + "[" + sevent.id + "] " + sevent.participants[0].id + "/" + sevent.participants[1].id);
                                                //    }

                                                //    using (FbCommand cmd2 = new FbCommand())
                                                //    {
                                                //        cmd2.CommandText = "PR_Result_GoalInfo";
                                                //        cmd2.CommandType = CommandType.StoredProcedure;
                                                //        cmd2.Connection = connection;
                                                //        cmd2.Parameters.Add("@EMATCHID", sevent.id);
                                                //        cmd2.Parameters.Add("@H_GOAL", sevent.participants[0].counter == "1" ? sevent.participants[0].results.FirstOrDefault(c => c.id == "2").value : sevent.participants[1].stats.FirstOrDefault(c => c.id == "2").value);
                                                //        cmd2.Parameters.Add("@G_GOAL", sevent.participants[1].counter == "2" ? sevent.participants[1].results.FirstOrDefault(c => c.id == "2").value : sevent.participants[0].stats.FirstOrDefault(c => c.id == "2").value);
                                                //        cmd2.Parameters.Add("@LASTTIME", DateTime.Now);
                                                //        int id = Convert.ToInt32(cmd2.ExecuteScalar());
                                                //        Files.WriteLog((id > 0 ? " [Success] Update GoalInfo Goal " : " [Failure]  Update GoalInfo Goal") + "[" + sevent.id + "] " + sevent.participants[0].id + "/" + sevent.participants[1].id);
                                                //    }
                                                //}

                                                //    foreach (DOSEvents2.apiDataCompetitionSeasonStageGroupEventParticipant participant in sevent.participants)
                                                //{
                                                //    if (participant == null) continue;
                                                //    using (FbCommand cmd2 = new FbCommand())
                                                //    {
                                                //        cmd2.CommandText = "ADD_TEAM";
                                                //        cmd2.CommandType = CommandType.StoredProcedure;
                                                //        cmd2.Connection = connection;
                                                //        cmd2.Parameters.Add("@ID", participant.id);
                                                //        cmd2.Parameters.Add("@NAME", participant.name);
                                                //        cmd2.Parameters.Add("@SHORT_NAME", participant.short_name);
                                                //        cmd2.Parameters.Add("@ACRONYM", participant.acronym);
                                                //        cmd2.Parameters.Add("@GENDER", (participant.gender.ToLower() == "male") ? true : false);
                                                //        cmd2.Parameters.Add("@AREA_ID", participant.area_id);
                                                //        cmd2.Parameters.Add("@BNATIONAL", (participant.national.ToLower() == "male") ? true : false);
                                                //        cmd2.Parameters.Add("@UT", participant.ut);
                                                //        cmd2.Parameters.Add("@OLD_PARTICIPANT_ID", participant.old_participant_id == "" ? "-1" : participant.old_participant_id);
                                                //        cmd2.Parameters.Add("@SLUG", participant.slug);
                                                //        cmd2.Parameters.Add("@SEASON_ID", "-1");
                                                //        cmd2.Parameters.Add("@CTIMESTAMP", cTimestamp);
                                                //        int id = Convert.ToInt32(cmd2.ExecuteScalar());
                                                //        Files.WriteLog(id > 0 ? " [Success] Insert teams[" + participant.id + "] " + participant.name : "[" + participant.id + "] " + participant.name + " team exist.");
                                                //    }
                                                //    foreach (DOSEvents2.apiDataCompetitionSeasonStageGroupEventParticipantLineup lineup in participant.lineups)
                                                //    {
                                                //        if (lineup == null) continue;

                                                //        using (FbCommand cmd2 = new FbCommand())
                                                //        {
                                                //            cmd2.CommandText = "ADD_Player2";
                                                //            cmd2.CommandType = CommandType.StoredProcedure;
                                                //            cmd2.Connection = connection;
                                                //            cmd2.Parameters.Add("@ID", lineup.participant_id == "" ? "-1" : lineup.participant_id);
                                                //            cmd2.Parameters.Add("@NAME", lineup.participant_name);
                                                //            cmd2.Parameters.Add("@AREA_ID", lineup.participant_area_id == "" ? "-1" : lineup.participant_area_id);
                                                //            cmd2.Parameters.Add("@SLUG", lineup.participant_slug);
                                                //            cmd2.Parameters.Add("@BENCH", lineup.bench.ToLower() == "yes" ? true : false);
                                                //            cmd2.Parameters.Add("@SHIRT_NR", lineup.shirt_nr == "" ? "-1" : lineup.shirt_nr);
                                                //            cmd2.Parameters.Add("@TEAM_ID", participant.id);
                                                //            cmd2.Parameters.Add("@SEASON_ID", season.id);
                                                //            cmd2.Parameters.Add("@CTIMESTAMP", cTimestamp);
                                                //            int id = Convert.ToInt32(cmd2.ExecuteScalar());
                                                //            // Files.WriteLog(id > 0 ? " [Success] Insert players2 [" + lineup.participant_id+ "] " + lineup.participant_name + " " + strName + ".xml" : " player exist.");
                                                //            Files.WriteLog(id > 0 ? " [Success] Insert players [" + lineup.participant_id + "] " + lineup.participant_name : "[" + lineup.participant_id + "] " + lineup.participant_name + " player exist.");
                                                //        }
                                                //    }
                                                //}
                                            }
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
                                // queryString = "select e.ID, e.NAME, e.HOME_ID, e.GUEST_ID, e.START_DATE,  e.STATUS_NAME,    e.STATUS_TYPE,    e.ROUND_NAME,     e.BOOKED,  e.GROUP_ID, e.STAGE_ID, e.SEASON_ID, e.COMPETITION_ID,    e.AREA_ID, e.CTIMESTAMP from events e where '" + arr[1] + "'<=  e.start_date and  e.start_date <='" + arr[2] + "'";
                                // queryString = "select e.* from events e where '" + arr[1] + "'<=  e.start_date and  e.start_date <='" + arr[2] + "' order by  e.start_date asc";
                                // queryString = "select e.* from events e where '" + arr[1] + "'<= e.start_date and  e.start_date <='" + arr[2] + "'  order by  e.start_date asc";
                                ///queryString = "SELECT * FROM EVENTS AS G1 JOIN (SELECT id, max(ut) as mostrecent    FROM EVENTS group by id) AS G2 ON G2.id = G1.id and g2.mostrecent = g1.ut where '" + arr[1] + "'<= G1.start_date and  G1.start_date <='" + arr[2] + "'  order by  G1.start_date desc";
                                queryString = "SELECT * FROM EVENTS AS G1 where '" + arr[1] + "'<= G1.start_date and  G1.start_date <='" + arr[2] + "'  and (g1.HOME_ID!=-1 and g1.GUEST_ID !=-1) order by  G1.start_date desc";

                            }
                            else
                            {
                                if (arr[1].ToString().Length < 12)
                                {
                                    //  queryString = "select e.ID, e.NAME, e.HOME_ID, e.GUEST_ID, e.START_DATE,  e.STATUS_NAME,    e.STATUS_TYPE,    e.ROUND_NAME,     e.BOOKED,  e.GROUP_ID, e.STAGE_ID, e.SEASON_ID, e.COMPETITION_ID,    e.AREA_ID, e.CTIMESTAMP from events e where " + (arr[1].ToString() == "Group" ? " e.GROUP_ID" : (arr[1].ToString() == "Stage") ? " e.STAGE_ID" : (arr[1].ToString() == "Season") ? " e.SEASON_ID" : (arr[1].ToString() == "Comp") ? " e.COMPETITION_ID" : "") + "='" + arr[2] + "'";
                                    //queryString = "select e.* from events e where " + (arr[1].ToString() == "Group" ? " e.GROUP_ID" : (arr[1].ToString() == "Stage") ? " e.STAGE_ID" : (arr[1].ToString() == "Season") ? " e.SEASON_ID" : (arr[1].ToString() == "Comp") ? " e.COMPETITION_ID" : "") + "='" + arr[2] + "'  order by  e.start_date asc";
                                    // queryString = "SELECT * FROM EVENTS AS G1 JOIN (SELECT id, max(ut) as mostrecent    FROM EVENTS group by id) AS G2 ON G2.id = G1.id and g2.mostrecent = g1.ut   where " + (arr[1].ToString() == "Group" ? " G1.GROUP_ID" : (arr[1].ToString() == "Stage") ? " G1.STAGE_ID" : (arr[1].ToString() == "Season") ? " G1.SEASON_ID" : (arr[1].ToString() == "Comp") ? " G1.COMPETITION_ID" : "") + "='" + arr[2] + "'  order by  G1.start_date asc";
                                    //queryString = "SELECT * FROM EVENTS AS G1 JOIN (SELECT id, max(ut) as mostrecent    FROM EVENTS group by id) AS G2 ON G2.id = G1.id and g2.mostrecent = g1.ut   where "
                                    //    + (arr[1].ToString() == "Group" ? " G1.GROUP_ID" : (arr[1].ToString() == "Stage") ? " G1.STAGE_ID" : (arr[1].ToString() == "Season") ? " G1.SEASON_ID" : (arr[1].ToString() == "Comp") ? " G1.COMPETITION_ID" : "")
                                    //    + "='" + arr[2] + "'  order by  G1.start_date asc";
                                    ////queryString = "SELECT * FROM EVENTS AS G1 JOIN (SELECT id, max(ut) as mostrecent    FROM EVENTS group by id) AS G2 ON G2.id = G1.id and g2.mostrecent = g1.ut   where "
                                    ////    + (arr[1].ToString() == "Comp" ? " G1.COMPETITION_ID='" + arr[2] + "'"
                                    ////    : (arr[1].ToString() == "Season") ? "G1.COMPETITION_ID='" + arr[3] + "' and  G1.SEASON_ID='" + arr[2] + "'"
                                    ////    : (arr[1].ToString() == "Stage") ? "G1.COMPETITION_ID='" + arr[4] + "' and  G1. SEASON_ID ='" + arr[3] + "' and G1.STAGE_ID='" + arr[2] + "'"
                                    ////    : (arr[1].ToString() == "Group") ? "G1.COMPETITION_ID='" + arr[5] + "' and  G1. SEASON_ID ='" + arr[4] + "' and G1.STAGE_ID='" + arr[3] + "' and G1.GROUP_ID ='" + arr[2] + "'" : "")
                                    ////      + "  order by  G1.start_date desc";

                                    queryString = "SELECT * FROM EVENTS AS G1  where "
                                       + (arr[1].ToString() == "Comp" ? " G1.COMPETITION_ID='" + arr[2] + "' and "
                                       : (arr[1].ToString() == "Season") ? "G1.COMPETITION_ID='" + arr[3] + "' and  G1.SEASON_ID='" + arr[2] + "' and "
                                       : (arr[1].ToString() == "Stage") ? "G1.COMPETITION_ID='" + arr[4] + "' and  G1. SEASON_ID ='" + arr[3] + "' and G1.STAGE_ID='" + arr[2] + "' and "
                                       : (arr[1].ToString() == "Group") ? "G1.COMPETITION_ID='" + arr[5] + "' and  G1. SEASON_ID ='" + arr[4] + "' and G1.STAGE_ID='" + arr[3] + "' and G1.GROUP_ID ='" + arr[2] + "' and " : "")
                                         + " (g1.HOME_ID!=-1 and g1.GUEST_ID !=-1)  order by  G1.start_date desc";
                                }
                                else
                                {
                                    // queryString = "select e.ID, e.NAME, e.HOME_ID, e.GUEST_ID, e.START_DATE,  e.STATUS_NAME,    e.STATUS_TYPE,    e.ROUND_NAME,     e.BOOKED,  e.GROUP_ID, e.STAGE_ID, e.SEASON_ID, e.COMPETITION_ID,    e.AREA_ID, e.CTIMESTAMP from events e where '" + arr[1] + "'<= e.start_date and  e.start_date <='" + arr[2] + "'";
                                    //  queryString = "select e.* from events e where '" + arr[1] + "'<= e.start_date and  e.start_date <='" + arr[2] + "'  order by  e.start_date asc";
                                    ///queryString = "SELECT * FROM EVENTS AS G1 JOIN (SELECT id, max(ut) as mostrecent    FROM EVENTS group by id) AS G2 ON G2.id = G1.id and g2.mostrecent = g1.ut where '" + arr[1] + "'<= G1.start_date and  G1.start_date <='" + arr[2] + "'  order by  G1.start_date desc";
                                    queryString = "SELECT * FROM EVENTS AS G1  where '" + arr[1] + "'<= G1.start_date and  G1.start_date <='" + arr[2] + "' and  (g1.HOME_ID!=-1 and g1.GUEST_ID !=-1)   order by  G1.start_date desc";
                                }
                            }
                            FbDataAdapter adapter = new FbDataAdapter();
                            adapter.SelectCommand = new FbCommand(queryString, connection);
                            FbCommandBuilder builder = new FbCommandBuilder(adapter);
                            connection.Open();
                            DataSet eventsDs = new DataSet();
                            adapter.Fill(eventsDs);
                            if ((Convert.ToBoolean(arr[0]) && DateTime.Now <= Convert.ToDateTime(arr[2]))||AppFlag.GetEventShow)// && eventsDs.Tables[0].Rows.Count == 0)
                            //  if (Convert.ToBoolean(arr[0]))
                            //if (DateTime.Now <= Convert.ToDateTime(arr[2]))
                            {
                                //if (eventsDs.Tables[0].Rows.Count == 0)
                                //{
                                //DataSet newEventsDs= eventsDs.Clone ();
                                for (int i = 1; i < 500; i++)
                                {
                                    //if (Convert.ToBoolean(arr[0]) == false) { ds = eventsDs; break; }
                                    count = 0;
                                    var responseValue = clientTest.GetAccessData(strToken, "events/" + i, arr[1], arr[2]);
                                    var strResponseValue = responseValue.Result;
                                    //XDocument document = XDocument.Load("E:\\Project\\AppProject\\DataOfScouts\\DataOfScouts\\bin\\Debug\\New folder\\events-1 105644.xml");
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
                                            cmd.Parameters.Add("@AREA_TYPE", competition.area_type);
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
                                                    //  if (ds.Tables.Count == 0) { ds = eventsDs; } 
                                                    foreach (DOSEvents.apiDataCompetitionSeasonStageGroupEvent sevent in group.events)
                                                    {
                                                        if (sevent == null) continue;
                                                        //if (ds.Tables.Count == 0) { ds=eventsDs; }
                                                        DataRow[] drs = (ds.Tables.Count == 0 ? eventsDs : ds).Tables[0].Select("id=" + sevent.id + " AND start_date='" + sevent.start_date + "'");
                                                        //if(ds.Tables.Count == 0)
                                                        //{
                                                        //    Files.WriteLog("eventsDs " + eventsDs.Tables[0].Rows.Count.ToString());
                                                        //}
                                                        //else
                                                        //{
                                                        //    Files.WriteLog("ds " + ds.Tables[0].Rows.Count.ToString());
                                                        //}
                                                        DOSEvents.apiDataCompetitionSeasonStageGroupEventParticipant[] participants = sevent.participants;
                                                        if (participants == null|| participants.Count()<2) continue;
                                                        if (drs.Length == 0)
                                                        {
                                                            /// DOSEvents.apiDataCompetitionSeasonStageGroupEventParticipant[] participants = sevent.participants;
                                                            //if(sevent.id== "2455015")
                                                            //{
                                                            //    string sdt = "";
                                                            //}
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
                                                            dr[35] = DBNull.Value; // sevent.booked == "yes" ? true : false;
                                                            dr[36] = DBNull.Value;// sevent.booked_by;
                                                            dr[37] = sevent.inverted_participants == "yes" ? true : false;
                                                            dr[38] = sevent.venue_id;
                                                            dr[39] = group.id == "" ? "-1" : group.id;
                                                            dr[40] = strStage_id;
                                                            dr[41] = strSeasons_id;
                                                            dr[42] = strCompetition_id;
                                                            dr[43] = strArea_id;
                                                            dr[44] = cTimestamp;
                                                            eventsDs.Tables[0].Rows.Add(dr);
                                                            /// 20190111  Files.WriteLog("[" + sevent.id + "] " + (participants[0].counter == "1" ? participants[0].name : participants[1].name) + "/" + (participants[0].counter == "2" ? participants[0].name : participants[1].name));
                                                            //20190227
                                                            /// Files.WriteLog("[" + sevent.id +"  "+ sevent.name + "] " + (participants[0].counter == "1" ? participants[0].name : participants[1].name) + "/" + (participants[0].counter == "2" ? participants[0].name : participants[1].name));
                                                            Files.WriteLog("[" + sevent.id + " " + sevent.name + "] " + (participants[0].counter == "1" ? participants[0].id + " " + participants[0].name : participants[1].id + " " + participants[1].name) + "/" + (participants[0].counter == "2" ? participants[0].id + " " + participants[0].name : participants[1].id + " " + participants[1].name));

                                                            //  Files.WriteLog("ABDCE "+ sevent.id);
                                                            //foreach (DOSEvents.apiDataCompetitionSeasonStageGroupEventParticipant participant in participants)
                                                            //{
                                                            //    if (participant == null) continue;
                                                            //    using (FbCommand cmd2 = new FbCommand())
                                                            //    {
                                                            //        cmd2.CommandText = "ADD_TEAM";
                                                            //        cmd2.CommandType = CommandType.StoredProcedure;
                                                            //        cmd2.Connection = connection;
                                                            //        cmd2.Parameters.Add("@ID", participant.id);
                                                            //        cmd2.Parameters.Add("@NAME", participant.name);
                                                            //        cmd2.Parameters.Add("@SHORT_NAME", participant.short_name);
                                                            //        cmd2.Parameters.Add("@ACRONYM", participant.acronym);
                                                            //        cmd2.Parameters.Add("@GENDER", (participant.gender.ToLower() == "male") ? true : false);
                                                            //        cmd2.Parameters.Add("@AREA_ID", participant.area_id);
                                                            //        cmd2.Parameters.Add("@BNATIONAL", (participant.national.ToLower() == "male") ? true : false);
                                                            //        cmd2.Parameters.Add("@UT", participant.ut);
                                                            //        cmd2.Parameters.Add("@OLD_PARTICIPANT_ID", participant.old_participant_id == "" ? "-1" : participant.old_participant_id);
                                                            //        cmd2.Parameters.Add("@SLUG", participant.slug);
                                                            //        cmd2.Parameters.Add("@SEASON_ID", "-1");
                                                            //        cmd2.Parameters.Add("@CTIMESTAMP", cTimestamp);
                                                            //        int id = Convert.ToInt32(cmd2.ExecuteScalar());
                                                            //        Files.WriteLog(id > 0 ? " [Success] Insert teams[" + participant.id + "] " + participant.name : "[" + participant.id + "] " + participant.name + " team exist.");
                                                            //    }
                                                            //}
                                                            ////    InsertData2("events.show", true, sevent.id);
                                                        }
                                                        else
                                                        {
                                                            //drs[0].Delete();
                                                            //(ds.Tables.Count == 0 ? eventsDs : ds).Tables[0].AcceptChanges();
                                                            //DataRow dr = eventsDs.Tables[0].NewRow();
                                                            //dr[0] = sevent.id;
                                                            //dr[1] = sevent.name;
                                                            //dr[2] = (participants[0].counter == "1") ? participants[0].id : participants[1].id;
                                                            //dr[3] = (participants[1].counter == "2") ? participants[1].id : participants[0].id;
                                                            //dr[4] = sevent.source;
                                                            //dr[5] = sevent.source_dc == "yes" ? true : false;
                                                            //dr[6] = sevent.source_super;
                                                            //dr[7] = sevent.relation_status;
                                                            //dr[8] = Convert.ToDateTime(sevent.start_date);
                                                            //dr[9] = sevent.ft_only == "yes" ? true : false;
                                                            //dr[10] = sevent.coverage_type;
                                                            //dr[11] = sevent.channel_id;
                                                            //dr[12] = sevent.channel_name;
                                                            //dr[13] = sevent.scoutsfeed == "yes" ? true : false;
                                                            //dr[14] = sevent.status_id;
                                                            //dr[15] = sevent.status_name;
                                                            //dr[16] = sevent.status_type;
                                                            //dr[17] = sevent.day;
                                                            //dr[18] = sevent.clock_time;
                                                            //dr[19] = sevent.clock_status;
                                                            //dr[20] = sevent.winner_id;
                                                            //dr[21] = sevent.progress_id;
                                                            //dr[22] = sevent.bet_status;
                                                            //dr[23] = sevent.neutral_venue == "yes" ? true : false;
                                                            //dr[24] = sevent.item_status;
                                                            //dr[25] = sevent.ut;
                                                            //dr[26] = sevent.old_event_id == "" ? "-1" : sevent.old_event_id;
                                                            //dr[27] = sevent.slug;
                                                            //dr[28] = sevent.verified_result == "yes" ? true : false;
                                                            //dr[29] = sevent.is_protocol_verified == "yes" ? true : false;
                                                            //dr[30] = sevent.protocol_verified_by;
                                                            //dr[31] = sevent.protocol_verified_at;
                                                            //dr[32] = sevent.round_id;
                                                            //dr[33] = sevent.round_name;
                                                            //dr[34] = sevent.client_event_id == "" ? "-1" : sevent.client_event_id;
                                                            //dr[35] = DBNull.Value; // sevent.booked == "yes" ? true : false;
                                                            //dr[36] = DBNull.Value;// sevent.booked_by;
                                                            //dr[37] = sevent.inverted_participants == "yes" ? true : false;
                                                            //dr[38] = sevent.venue_id;
                                                            //dr[39] = group.id == "" ? "-1" : group.id;
                                                            //dr[40] = strStage_id;
                                                            //dr[41] = strSeasons_id;
                                                            //dr[42] = strCompetition_id;
                                                            //dr[43] = strArea_id;
                                                            //dr[44] = cTimestamp;
                                                            //eventsDs.Tables[0].Rows.Add(dr);

                                                            ///Files.WriteLog("[" + sevent.id + "] " + (participants[0].counter == "1" ? participants[0].name : participants[1].name) + "/" + (participants[0].counter == "2" ? participants[0].name : participants[1].name)+".");
                                                            Files.WriteLog("[" + sevent.id + " " + sevent.name + "] " + (participants[0].counter == "1" ? participants[0].id + " " + participants[0].name : participants[1].id + " " + participants[1].name) + "/" + (participants[0].counter == "2" ? participants[0].id + " " + participants[0].name : participants[1].id + " " + participants[1].name) + ".");

                                                        }
                                                        foreach (DOSEvents.apiDataCompetitionSeasonStageGroupEventParticipant participant in participants)
                                                        {
                                                            if (participant == null) continue;
                                                            using (FbCommand cmd2 = new FbCommand())
                                                            {
                                                                cmd2.CommandText = "ADD_TEAM2";
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
                                                                Files.WriteLog(id == 1 ? " [Success] Insert teams[" + participant.id + "] " + participant.name : "[" + participant.id + "] " + participant.name + " team exist.");
                                                            }
                                                        }
                                                        //    InsertData2("events.show", true, sevent.id);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    string eid = "";
                                    int retry = 0;
                                    while (true && eventsDs.Tables[0].Rows.Count > 0 && retry < eventsDs.Tables[0].Rows.Count + 1)
                                    {
                                        try
                                        {
                                            retry++;
                                            //Files.WriteLog("eventsDs 3 " + eventsDs.Tables[0].Rows.Count.ToString()+"    "+ retry.ToString ());
                                            count = adapter.Update(eventsDs);
                                            // Files.WriteLog("eventsDs 4 " + eventsDs.Tables[0].Rows.Count.ToString() + "    " + retry.ToString());
                                            Files.WriteLog("[Success] Insert events [" + count + "]" + " " + strName + ".xml");
                                            break;
                                        }
                                        catch (FbException fex)
                                        {
                                            if (fex.InnerException.Message.IndexOf(" = ") > 0)
                                            {

                                                eid = fex.InnerException.Message.Substring(fex.InnerException.Message.IndexOf(" = ") + 3, fex.InnerException.Message.Length - fex.InnerException.Message.IndexOf(" = ") - 4);
                                                //  eventsDs.AcceptChanges();
                                                DataRow dr = eventsDs.Tables[0].Select("id=" + eid)[0];
                                                using (FbCommand cmd2 = new FbCommand())
                                                {
                                                    cmd2.CommandText = "DeleteEvent_ByID";
                                                    cmd2.CommandType = CommandType.StoredProcedure;
                                                    cmd2.Connection = connection;
                                                    cmd2.Parameters.Add("@ID", eid);
                                                    int id = Convert.ToInt32(cmd2.ExecuteScalar());
                                                }
                                                Files.WriteLog("[" + dr["id"] + "] " + dr["name"] + "  events existed.");
                                                //  /dr.Delete();
                                                ////  eventsDs.Tables[0].Rows.Remove(dr);
                                                //  eventsDs.AcceptChanges();
                                            }
                                        }
#pragma warning disable CS0168 // The variable 'exp' is declared but never used
                                        catch (Exception exp)
#pragma warning restore CS0168 // The variable 'exp' is declared but never used
                                        {
                                            break;
                                        }
                                    }
                                    // Files.WriteLog("eventsDs 2 " + eventsDs.Tables[0].Rows.Count.ToString());
                                    // Files.WriteLog((ds.Tables.Count!=0)?"ds 2 " + ds.Tables[0].Rows.Count.ToString():"DS 0");
                                    ds.Merge(eventsDs, true, MissingSchemaAction.AddWithKey);
                                    // Files.WriteLog("ds 3 " + ds.Tables[0].Rows.Count.ToString());
                                    eventsDs.Clear();
                                    //20190227
                                    ////if (count > -1)
                                    ////{
                                    ////    Files.WriteLog("[Success] Insert events[" + count + "  " + "] " + " " + strName + ".xml");
                                    ////}
                                    ////else
                                    ////{
                                    ////    Files.WriteLog("[Failure] Insert events" + " " + strName + ".xml");
                                    ////}

                                    if (count == 0)
                                    {
                                        Files.WriteLog("Insert events[" + count + "  " + "] " + " " + strName + ".xml");
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
                                queryString = "select e.ID, e.NAME, e.HOME_ID, e.GUEST_ID, e.START_DATE,  e.STATUS_NAME,    e.STATUS_TYPE,    e.ROUND_NAME,     e.BOOKED,  e.GROUP_ID, e.STAGE_ID, e.SEASON_ID, e.COMPETITION_ID,    e.AREA_ID, e.CTIMESTAMP from " + "events e" + " where '" + arr[1] + "'<= e.start_date and e.start_date <='" + arr[2] + "' and e.booked =true";
                            }
                            else
                            {
                                if (arr[1].ToString().Length < 12)
                                {
                                    queryString = "select e.ID, e.NAME, e.HOME_ID, e.GUEST_ID, e.START_DATE,  e.STATUS_NAME,    e.STATUS_TYPE,    e.ROUND_NAME,     e.BOOKED,  e.GROUP_ID, e.STAGE_ID, e.SEASON_ID, e.COMPETITION_ID,    e.AREA_ID, e.CTIMESTAMP from " + "events e" + " where " + (arr[1].ToString() == "Group" ? "e.GROUP_ID" : (arr[1].ToString() == "Stage") ? "e.STAGE_ID" : (arr[1].ToString() == "Season") ? "e.SEASON_ID" : (arr[1].ToString() == "Comp") ? "e.COMPETITION_ID" : "") + "='" + arr[2] + "' and e.booked =true";
                                }
                                else
                                {
                                    queryString = "select e.ID, e.NAME, e.HOME_ID, e.GUEST_ID, e.START_DATE,  e.STATUS_NAME,    e.STATUS_TYPE,    e.ROUND_NAME,     e.BOOKED,  e.GROUP_ID, e.STAGE_ID, e.SEASON_ID, e.COMPETITION_ID,    e.AREA_ID, e.CTIMESTAMP from " + "events e" + " where '" + arr[1] + "'<= e.start_date and e.start_date <='" + arr[2] + "' and e.booked =true";
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
                case "events.compare":
                    {
                        string m_HOME_ID = "", m_GUEST_ID = "";
                        using (FbConnection connection = new FbConnection(AppFlag.ScoutsDBConn))
                        {
                            // Files.WriteLog("GetEventCompare4 " + arr[1]);
                            connection.Open();
                            queryString = "SELECT e.HOME_ID, e.GUEST_ID from events e where e.id=" + arr[1];
                            using (FbCommand cmd = new FbCommand(queryString, connection))
                            {
                                using (FbDataReader dataReader = cmd.ExecuteReader())
                                {
                                    while (dataReader.Read())
                                    {
                                        m_HOME_ID = dataReader.GetInt32(0).ToString();
                                        m_GUEST_ID = dataReader.GetInt32(1).ToString();
                                    }
                                    dataReader.Close();
                                }
                            }
                            //    connection.Close();
                            //}
                            //Files.WriteLog("GetEventCompare5 " + arr[1]);
                            var responseValue = clientTest.GetAccessData(strToken, "events.compare", arr[1], m_HOME_ID, m_GUEST_ID);
                            var strResponseValue = responseValue.Result;
                            //XDocument document = XDocument.Load("E:\\Project\\AppProject\\DataOfScouts\\DataOfScouts\\bin\\Debug\\New folder\\2684815-events.compare173720.xml");
                            //var strResponseValue = document.ToString();

                            if (strResponseValue == "Unauthorized") { MessageBox.Show("Unauthorized!"); break; }
                            string strName = arr[1].ToString() + "-" + type + DateTime.Now.ToString("HHmmss");

                            DOSCompare.api apis = XmlUtil.Deserialize(typeof(DOSCompare.api), strResponseValue) as DOSCompare.api;
                            if (apis == null) break;

                            Files.WriteXml(strName, strResponseValue);
                            // Files.WriteLog("GetEventCompare6 " + arr[1] +" "+ m_HOME_ID+" "+ m_GUEST_ID);

                            int irec = 0;
                            DOSCompare.apiData data = (apis.data == null || apis.data.Length == 0) ? null : apis.data[0];
                            if (data == null) break;
                            //string dp = "";
                            DOSCompare.participantsParticipant[] participants = (data.participants == null || data.participants.Length == 0) ? null : data.participants;
                            //if (participants == null) break; 
                            //foreach(DOSCompare.participantsParticipant p in participants)
                            //{
                            //    dp=
                            //}

                            DOSCompare.apiDataHead2head dataHead = (data.head2head == null || data.head2head.Length == 0) ? null : data.head2head[0];
                            if (dataHead == null) break;

                            DOSCompare.apiDataHead2headAll_matches_stats allMatches = (dataHead.all_matches_stats == null || dataHead.all_matches_stats.Length == 0) ? null : dataHead.all_matches_stats[0];
                            if (allMatches != null && allMatches.total_events_played != "0")
                            {
                                int total_events_played = -1, participant1_won = -1, participant1_draw = -1, participant1_lost = -1, participant2_won = -1, participant2_draw = -1, participant2_lost = -1;
                                total_events_played = Convert.ToInt32(allMatches.total_events_played);
                                participant1_won = Convert.ToInt32(allMatches.participant1_won);
                                participant1_draw = Convert.ToInt32(allMatches.participant1_draw);
                                participant1_lost = Convert.ToInt32(allMatches.participant1_lost);
                                participant2_won = Convert.ToInt32(allMatches.participant2_won);
                                participant2_draw = Convert.ToInt32(allMatches.participant2_draw);
                                participant2_lost = Convert.ToInt32(allMatches.participant2_lost);

                                using (FbCommand cmd = new FbCommand())
                                {
                                    cmd.CommandText = "ADD_ANALYSIS_STAT_INFO";
                                    cmd.CommandType = CommandType.StoredProcedure;
                                    cmd.Connection = connection;
                                    cmd.Parameters.Add("@IMATCH_CNT", arr[1]);
                                    cmd.Parameters.Add("@CACT", 'U');
                                    cmd.Parameters.Add("@IHOSTWIN", ((double)participant1_won / total_events_played) * 100);
                                    cmd.Parameters.Add("@IHOSTDRAW", ((double)participant1_draw / total_events_played) * 100);
                                    cmd.Parameters.Add("@IHOSTLOSS", ((double)participant1_lost / total_events_played) * 100);
                                    cmd.Parameters.Add("@IGUESTWIN", ((double)participant2_won / total_events_played) * 100);
                                    cmd.Parameters.Add("@IGUESTDRAW", ((double)participant2_draw / total_events_played) * 100);
                                    cmd.Parameters.Add("@IGUESTLOSS", ((double)participant2_lost / total_events_played) * 100);
                                    cmd.Parameters.Add("@CTIMESTAMP", DateTime.Now);
                                    int id = Convert.ToInt32(cmd.ExecuteScalar());
                                }
                            }

                            DOSCompare.apiDataHead2headLast_10_matches_stats tenMatches = (dataHead.last_10_matches_stats == null || dataHead.last_10_matches_stats.Length == 0) ? null : dataHead.last_10_matches_stats[0];
                            // if (tenMatches == null) break;

                            DOSCompare.apiDataHead2headH2h_eventsCompetitionsCompetition[] competition = (dataHead.h2h_events == null || dataHead.h2h_events.Length == 0) ? null : dataHead.h2h_events[0];
                            // if (competition == null) break;
                            if (competition != null)
                            {
                                foreach (DOSCompare.apiDataHead2headH2h_eventsCompetitionsCompetition c in competition)
                                {
                                    if (c == null) break;
                                    DOSCompare.apiDataHead2headH2h_eventsCompetitionsCompetitionSeasonsSeason[] seasons = (c.seasons == null || c.seasons.Length == 0) ? null : c.seasons;
                                    foreach (DOSCompare.apiDataHead2headH2h_eventsCompetitionsCompetitionSeasonsSeason season in seasons)
                                    {
                                        if (season == null) break;
                                        DOSCompare.apiDataHead2headH2h_eventsCompetitionsCompetitionSeasonsSeasonStagesStage[] stages = (season.stages == null || season.stages.Length == 0) ? null : season.stages;
                                        foreach (DOSCompare.apiDataHead2headH2h_eventsCompetitionsCompetitionSeasonsSeasonStagesStage stage in stages)
                                        {
                                            if (stage == null) break;
                                            DOSCompare.apiDataHead2headH2h_eventsCompetitionsCompetitionSeasonsSeasonStagesStageGroupsGroup[] groups = (stage.groups == null || stage.groups.Length == 0) ? null : stage.groups;
                                            foreach (DOSCompare.apiDataHead2headH2h_eventsCompetitionsCompetitionSeasonsSeasonStagesStageGroupsGroup group in groups)
                                            {
                                                if (group == null) break;
                                                DOSCompare.apiDataHead2headH2h_eventsCompetitionsCompetitionSeasonsSeasonStagesStageGroupsGroupEventsEvent[] events = (group.events == null || group.events.Length == 0) ? null : group.events;
                                                //   int irec = 0;
                                                foreach (DOSCompare.apiDataHead2headH2h_eventsCompetitionsCompetitionSeasonsSeasonStagesStageGroupsGroupEventsEvent e in events)
                                                {
                                                    if (e == null) break;
                                                    string p1 = "";
                                                    string p2 = "";
                                                    string m_IHOSTSCORE = "";
                                                    string m_IGUESTSCORE = "";
                                                    DOSCompare.participantsParticipant[] eparticipants = (e.participants == null || e.participants.Length == 0) ? null : e.participants;
                                                    foreach (DOSCompare.participantsParticipant p in eparticipants)
                                                    {
                                                        if (p == null) break;
                                                        if (p.counter == "1")
                                                        {
                                                            p1 = p.id;
                                                            m_IHOSTSCORE = p.results[3].value;
                                                        }
                                                        else if (p.counter == "2")
                                                        {
                                                            p2 = p.id;
                                                            m_IGUESTSCORE = p.results[3].value;
                                                        }
                                                    }
                                                    using (FbCommand cmd = new FbCommand())
                                                    {
                                                        cmd.CommandText = "ADD_ANALYSIS_HISTORY_INFO";
                                                        cmd.CommandType = CommandType.StoredProcedure;
                                                        cmd.Connection = connection;
                                                        cmd.Parameters.Add("@IMATCH_CNT", arr[1]);
                                                        cmd.Parameters.Add("@IREC", irec);
                                                        cmd.Parameters.Add("@CACT", 'U');
                                                        cmd.Parameters.Add("@EVENTID", e.id);
                                                        cmd.Parameters.Add("@HOME_ID", eparticipants[0].counter == "1" ? eparticipants[0].id : eparticipants[1].id);
                                                        cmd.Parameters.Add("@GUEST_ID", eparticipants[1].counter == "2" ? eparticipants[1].id : eparticipants[0].id);
                                                        cmd.Parameters.Add("@START_DATE", e.start_date);
                                                        cmd.Parameters.Add("@CLEAGUEALIASID", c.id);
                                                        cmd.Parameters.Add("@IMATCHMONTH", Convert.ToDateTime(e.start_date).Year);
                                                        cmd.Parameters.Add("@IMATCHYEAR", Convert.ToDateTime(e.start_date).Month);
                                                        cmd.Parameters.Add("@IMATCHSTATUS", participants[0].id == p1 ? 1 : 0);
                                                        cmd.Parameters.Add("@IHOSTSCORE", m_IHOSTSCORE);
                                                        cmd.Parameters.Add("@IGUESTSCORE", m_IGUESTSCORE);
                                                        cmd.Parameters.Add("@CTIMESTAMP", DateTime.Now);
                                                        int id = Convert.ToInt32(cmd.ExecuteScalar());
                                                        irec++;
                                                    }
                                                    DOSCompare.apiDataHead2headH2h_eventsCompetitionsCompetitionSeasonsSeasonStagesStageGroupsGroupEventsEventDetailsDetail[] details = (e.details == null || e.details.Length == 0) ? null : e.details;
                                                    foreach (DOSCompare.apiDataHead2headH2h_eventsCompetitionsCompetitionSeasonsSeasonStagesStageGroupsGroupEventsEventDetailsDetail detail in details)
                                                    {
                                                        if (details == null) break;

                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            queryString = "SELECT a.IMATCH_CNT, a.IREC, a.CACT, a.EVENTID,a.home_id,a.guest_id,a.start_date,a.LEAGUEALIAS, a.IMATCHMONTH, a.IMATCHYEAR, a.IMATCHSTATUS, a.IHOSTSCORE, a.IGUESTSCORE, a.CTIMESTAMP FROM ANALYSIS_HISTORY_INFO  A " +
                                " WHERE  A.HOME_ID=" + m_HOME_ID + " OR A.GUEST_ID=" + m_GUEST_ID +
                                " OR  A.HOME_ID=" + m_GUEST_ID + " OR A.GUEST_ID=" + m_HOME_ID + " ORDER BY A.START_DATE DESC ";
                            using (FbCommand cmd = new FbCommand(queryString, connection))
                            {
                                using (FbDataAdapter fda = new FbDataAdapter())
                                {
                                    fda.SelectCommand = cmd;
                                    using (DataSet hisData = new DataSet())
                                    {
                                        hisData.Tables.Add(new DataTable("hisData"));
                                        fda.Fill(hisData.Tables["hisData"]);
                                        irec = 1;
                                        for (int i = 0; i < 2; i++)
                                        {
                                            string m_CTEAMFLAG = "";
                                            string m_Team = "";
                                            int iretry = 0;
                                            m_Team = (i == 0) ? m_HOME_ID : m_GUEST_ID;
                                            foreach (DataRow dr in hisData.Tables[0].Select("HOME_ID =" + m_Team + " or GUEST_ID= " + m_Team, "  START_DATE DESC "))
                                            {
                                                if (m_HOME_ID == m_Team)
                                                {
                                                    m_CTEAMFLAG = "H";
                                                }
                                                else
                                                {
                                                    m_CTEAMFLAG = "G";
                                                }
                                                using (FbCommand cmd1 = new FbCommand())
                                                {
                                                    cmd1.CommandText = "ADD_ANALYSIS_RECENT_INFO";
                                                    cmd1.CommandType = CommandType.StoredProcedure;
                                                    cmd1.Connection = connection;
                                                    cmd1.Parameters.Add("@IMATCH_CNT", arr[1]);
                                                    cmd1.Parameters.Add("@IREC", irec);
                                                    cmd1.Parameters.Add("@CACT", 'U');
                                                    cmd1.Parameters.Add("@EVENTID", dr["EVENTID"]);
                                                    cmd1.Parameters.Add("@START_DATE", dr["START_DATE"]);
                                                    cmd1.Parameters.Add("@LEAGUEALIAS", dr["LEAGUEALIAS"]);
                                                    cmd1.Parameters.Add("@CTEAMFLAG", m_CTEAMFLAG);
                                                    cmd1.Parameters.Add("@CCHALLENGER", m_HOME_ID == m_Team ? m_GUEST_ID : m_HOME_ID);
                                                    cmd1.Parameters.Add("@IMATCHSTATUS", m_HOME_ID == m_Team ? dr["home_id"].ToString() == m_HOME_ID ? "0" : "1" : dr["home_id"].ToString() == m_GUEST_ID ? "0" : "1");//  dr["IMATCHSTATUS"]);
                                                                                                                                                                                                                       //cmd1.Parameters.Add("@IHOSTSCORE", m_HOME_ID == m_Team ? (dr["home_id"].ToString() == m_HOME_ID ? dr["IHOSTSCORE"] : dr["IGUESTSCORE"]) : dr["home_id"].ToString() == m_GUEST_ID ? dr["IGUESTSCORE"] : dr["IHOSTSCORE"]);// dr["IHOSTSCORE"]);
                                                                                                                                                                                                                       //cmd1.Parameters.Add("@IGUESTSCORE", m_HOME_ID == m_Team ? dr["home_id"].ToString() == m_HOME_ID ? dr["IGUESTSCORE"] : dr["IHOSTSCORE"] : dr["home_id"].ToString() == m_GUEST_ID ? dr["IHOSTSCORE"] : dr["IGUESTSCORE"]);// dr["IGUESTSCORE"]);
                                                    cmd1.Parameters.Add("@IHOSTSCORE", dr["IHOSTSCORE"]);
                                                    cmd1.Parameters.Add("@IGUESTSCORE", dr["IGUESTSCORE"]);
                                                    cmd1.Parameters.Add("@CTIMESTAMP", DateTime.Now);
                                                    int id = Convert.ToInt32(cmd1.ExecuteScalar());
                                                    irec++;
                                                    iretry++;
                                                }
                                                if (iretry > 4) break;
                                            }
                                        }
                                    }
                                }
                            }
                            connection.Close();
                        }
                        break;
                    }
                case "standings":
                    {
                        //string m_Comp_ID = "", m_Comp_Name = "", m_Comp_Name_cn = "";
                        ds = new DataSet();
                        using (FbConnection connection = new FbConnection(AppFlag.ScoutsDBConn))
                        {
                            connection.Open();
                            // queryString = "SELECT r.ID, r.COMPETITION_ID,   r.UT FROM STANDINGS_INFO r";
                            // queryString = "SELECT c.id cid,c.alias, s2.id  sid, s.ID ssid,  r.* from STANDINGS_INFO r " +
                            //queryString = "select   c.id cid,c.alias, s2.id  sid, s.ID ssid ,r.id from (SELECT    r.* from STANDINGS_INFO   r where    R.TYPE_NAME='Top scorers') r " +
                            //    "inner join   STAGES s   on r.OBJECT_ID =s.id  " +
                            //    "inner join   SEASONS s2 on s2.ID=s.SEASON_ID " +
                            //    "right join  COMPETITIONS c on c.id =s2.COMPETITION_ID " +
                            //    "where c.ALIAS is not null  order by r.id desc";
                            queryString = "select c.id cid,c.alias, s2.id  sid, r.id  from (SELECT r.id,r.OBJECT_TYPE,r.SEASON_ID from STANDINGS_INFO   r where    R.TYPE_NAME='Top scorers') r  "+
                            " inner join   SEASONS s2 on s2.ID = r.SEASON_ID and(s2.SYEAR = '2019' or s2.SYEAR = '2018/19')   right join  COMPETITIONS c on c.id = s2.COMPETITION_ID "+
                            " where c.ALIAS is not null order by c.ALIAS desc";
                            using (FbCommand cmd = new FbCommand(queryString))
                            {
                                using (FbDataAdapter fda = new FbDataAdapter())
                                {
                                    cmd.Connection = connection;
                                    fda.SelectCommand = cmd;
                                    using (DataSet data = new DataSet())
                                    {
                                        data.Tables.Add(new DataTable("standings"));
                                        fda.Fill(data.Tables["standings"]);
                                        ds = data;
                                    }
                                }
                            }

                            foreach (DataRow dr in ds.Tables[0].Rows)
                            {
                                if (dr["id"] is DBNull)
                                {
                                    var responseValue = clientTest.GetAccessData(strToken, "standings", "TopSoccer", dr["cid"]);
                                    var strResponseValue = responseValue.Result;
                                    //XDocument document = XDocument.Load("E:\\Project\\AppProject\\DataOfScouts\\DataOfScouts\\bin\\Debug\\XmlFolder20180920\\standings.xml");
                                    //var strResponseValue = document.ToString();

                                    if (strResponseValue == "Unauthorized") { MessageBox.Show("Unauthorized!"); break; }
                                    string strName = "standingSorer-" + dr["ALIAS"] + DateTime.Now.ToString("HHmmssfff");
                                    Files.WriteXml(strName, strResponseValue);

                                    DOSStandings.api apis = XmlUtil.Deserialize(typeof(DOSStandings.api), strResponseValue) as DOSStandings.api;
                                    if (apis == null) break;
                                    DOSStandings.apiDataStandings_listStandings[] list = (apis.data == null || apis.data.Length == 0) ? null : apis.data[0];
                                    if (list != null)
                                    {
                                        foreach (DOSStandings.apiDataStandings_listStandings c in list)
                                        {
                                            if (c.item_status == "deleted") continue;
                                            using (FbCommand cmd1 = new FbCommand())
                                            {
                                                cmd1.CommandText = "ADD_STANDINGS";
                                                cmd1.CommandType = CommandType.StoredProcedure;
                                                cmd1.Connection = connection;
                                                cmd1.Parameters.Add("@ID", c.id);
                                                cmd1.Parameters.Add("@Competition_ID", dr["cid"].ToString());
                                                cmd1.Parameters.Add("@CCompName", c.name.Length > 99 ? c.name.Substring(0, 99) : c.name);
                                                cmd1.Parameters.Add("@object_id", c.object_id);
                                                cmd1.Parameters.Add("@object_name", c.object_name);
                                                cmd1.Parameters.Add("@object_type", c.object_type);
                                                cmd1.Parameters.Add("@subtype", c.subtype);
                                                cmd1.Parameters.Add("@type_name", c.type_name);
                                                cmd1.Parameters.Add("@ut", c.ut);
                                                cmd1.Parameters.Add("@CTIMESTAMP", DateTime.Now);
                                                int id = Convert.ToInt32(cmd1.ExecuteScalar());
                                            }
                                        }

                                    }
                                }
                                if (dr["id"] is DBNull) continue;
                                var responseValue2 = clientTest.GetAccessData(strToken, "standings.TopSorers", dr["id"]);
                                var strResponseValue2 = responseValue2.Result;
                                //XDocument document = XDocument.Load("E:\\Project\\AppProject\\DataOfScouts\\DataOfScouts\\bin\\Debug\\New folder\\36571.xml");
                                //var strResponseValue2 = document.ToString();

                                if (strResponseValue2 == "Unauthorized") { MessageBox.Show("Unauthorized!"); break; }
                                string strName2 = "TopSorers-" + dr["ALIAS"].ToString() + DateTime.Now.ToString("HHmmss");
                                Files.WriteXml(strName2, strResponseValue2);

                                DOSTopScorers.api apis2 = XmlUtil.Deserialize(typeof(DOSTopScorers.api), strResponseValue2) as DOSTopScorers.api;
                                if (apis2 == null) break;
                                DOSTopScorers.apiDataStandings apistanding = (apis2.data == null || apis2.data.Length == 0) ? null : apis2.data[0];
                                if (apistanding == null) break;
                                DOSTopScorers.apiDataStandingsGroupsGroup[] apiGroups = apistanding.groups;
                                if (apiGroups == null) break;
                                foreach (DOSTopScorers.apiDataStandingsGroupsGroup c in apiGroups)
                                {
                                    if (c == null) continue;
                                    DOSTopScorers.apiDataStandingsGroupsGroupParticipantsParticipant[] apiPants = c.participants;
                                    if (apiPants == null) break;
                                    foreach (DOSTopScorers.apiDataStandingsGroupsGroupParticipantsParticipant p in apiPants)
                                    {
                                        if (p == null) continue;
                                        DOSTopScorers.apiDataStandingsGroupsGroupParticipantsParticipantColumnsColumn[] apim = p.columns;
                                        using (FbCommand cmd1 = new FbCommand())
                                        {
                                            cmd1.CommandText = "ADD_SCORERS_INFO";
                                            cmd1.CommandType = CommandType.StoredProcedure;
                                            cmd1.Connection = connection;
                                            cmd1.Parameters.Add("@CLEAG_ID", dr["cid"].ToString());
                                            cmd1.Parameters.Add("@CLEAG_ALIAS", dr["ALIAS"].ToString());
                                            cmd1.Parameters.Add("@SEASON_ID", dr["sid"].ToString());
                                            cmd1.Parameters.Add("@TEAM_ID", p.subparticipant_id);
                                            cmd1.Parameters.Add("@PLAYER_ID", p.id);
                                            cmd1.Parameters.Add("@CPLAYER_NAME", p.name);
                                            cmd1.Parameters.Add("@CTEAM_ABBR", "");
                                            cmd1.Parameters.Add("@CACT", "U");
                                            cmd1.Parameters.Add("@IRID", "-1");
                                            cmd1.Parameters.Add("@IRANK", p.rank);
                                            cmd1.Parameters.Add("@IGOALS", apim.Length > 1 ? apim[0].value : "0");
                                            cmd1.Parameters.Add("@ut", c.ut);
                                            cmd1.Parameters.Add("@CTIMESTAMP", DateTime.Now);
                                            int id = Convert.ToInt32(cmd1.ExecuteScalar());
                                        }

                                    }
                                }

                            }

                            // RANK
                            //queryString = "select   c.id cid,c.alias, s2.id  sid, s.ID ssid ,r.id from (SELECT    r.* from STANDINGS_INFO   r where    R.TYPE_NAME='Wide standings') r " +
                            //"inner join   STAGES s   on r.OBJECT_ID =s.id  " +
                            //"inner join   SEASONS s2 on s2.ID=s.SEASON_ID " +
                            //"right join  COMPETITIONS c on c.id =s2.COMPETITION_ID " +
                            //"where c.ALIAS is not null order by r.id desc";
                            queryString = "select c.id cid,c.alias, s2.id  sid, r.id  from (SELECT r.id,r.OBJECT_TYPE,r.SEASON_ID from STANDINGS_INFO   r where    R.TYPE_NAME='Wide standings') r  " +
                          " inner join   SEASONS s2 on s2.ID = r.SEASON_ID and(s2.SYEAR = '2019' or s2.SYEAR = '2018/19')   right join  COMPETITIONS c on c.id = s2.COMPETITION_ID " +
                          " where c.ALIAS is not null order by c.ALIAS desc";
                            using (FbCommand cmd = new FbCommand(queryString))
                            {
                                using (FbDataAdapter fda = new FbDataAdapter())
                                {
                                    cmd.Connection = connection;
                                    fda.SelectCommand = cmd;
                                    using (DataSet data = new DataSet())
                                    {
                                        data.Tables.Add(new DataTable("standings"));
                                        fda.Fill(data.Tables["standings"]);
                                        ds = data;
                                    }
                                }
                            }

                            foreach (DataRow dr in ds.Tables[0].Rows)
                            {
                                if (dr["id"] is DBNull)
                                {
                                    var responseValue = clientTest.GetAccessData(strToken, "standings", "Rank", dr["cid"]);
                                    var strResponseValue = responseValue.Result;
                                    //XDocument document = XDocument.Load("E:\\Project\\AppProject\\DataOfScouts\\DataOfScouts\\bin\\Debug\\XmlFolder20180920\\standings.xml");
                                    //var strResponseValue = document.ToString();

                                    if (strResponseValue == "Unauthorized") { MessageBox.Show("Unauthorized!"); break; }
                                    string strName = "standingRank-" + dr["ALIAS"] + DateTime.Now.ToString("HHmmssfff");
                                    Files.WriteXml(strName, strResponseValue);

                                    DOSStandings.api apis = XmlUtil.Deserialize(typeof(DOSStandings.api), strResponseValue) as DOSStandings.api;
                                    if (apis == null) break;
                                    DOSStandings.apiDataStandings_listStandings[] list = (apis.data == null || apis.data.Length == 0) ? null : apis.data[0];
                                    if (list != null)
                                    {
                                        foreach (DOSStandings.apiDataStandings_listStandings c in list)
                                        {
                                            if (c.item_status == "deleted") continue;
                                            using (FbCommand cmd1 = new FbCommand())
                                            {
                                                cmd1.CommandText = "ADD_STANDINGS";
                                                cmd1.CommandType = CommandType.StoredProcedure;
                                                cmd1.Connection = connection;
                                                cmd1.Parameters.Add("@ID", c.id);
                                                cmd1.Parameters.Add("@Competition_ID", dr["cid"].ToString());
                                                cmd1.Parameters.Add("@CCompName", c.name.Length > 99 ? c.name.Substring(0, 99) : c.name);
                                                cmd1.Parameters.Add("@object_id", c.object_id);
                                                cmd1.Parameters.Add("@object_name", c.object_name);
                                                cmd1.Parameters.Add("@object_type", c.object_type);
                                                cmd1.Parameters.Add("@subtype", c.subtype);
                                                cmd1.Parameters.Add("@type_name", c.type_name);
                                                cmd1.Parameters.Add("@ut", c.ut);
                                                cmd1.Parameters.Add("@CTIMESTAMP", DateTime.Now);
                                                int id = Convert.ToInt32(cmd1.ExecuteScalar());
                                            }
                                        }

                                    }
                                }
                                if (dr["id"] is DBNull) continue;
                                var responseValue2 = clientTest.GetAccessData(strToken, "standings.Rank", dr["id"]);
                                var strResponseValue2 = responseValue2.Result;
                                //XDocument document = XDocument.Load("E:\\Project\\AppProject\\DataOfScouts\\DataOfScouts\\bin\\Debug\\New folder\\47798.xml");
                                //var strResponseValue3 = document.ToString();

                                if (strResponseValue2 == "Unauthorized") { MessageBox.Show("Unauthorized!"); break; }
                                string strName2 = "Rank-" + dr["ALIAS"].ToString() + DateTime.Now.ToString("HHmmss");
                                Files.WriteXml(strName2, strResponseValue2);

                                DOSRank.api apis2 = XmlUtil.Deserialize(typeof(DOSRank.api), strResponseValue2) as DOSRank.api;
                                if (apis2 == null) break;
                                DOSRank.apiDataStandings apistanding = (apis2.data == null || apis2.data.Length == 0) ? null : apis2.data[0];
                                if (apistanding == null) break;
                                DOSRank.apiDataStandingsGroupsGroup[] apiGroups = apistanding.groups;
                                if (apiGroups == null) break;
                                foreach (DOSRank.apiDataStandingsGroupsGroup c in apiGroups)
                                {
                                    if (c == null) continue;
                                    DOSRank.apiDataStandingsGroupsGroupParticipantsParticipant[] apiPants = c.participants;
                                    if (apiPants == null) break;
                                    foreach (DOSRank.apiDataStandingsGroupsGroupParticipantsParticipant p in apiPants)
                                    {
                                        if (p == null) continue;
                                        DOSRank.apiDataStandingsGroupsGroupParticipantsParticipantColumnsColumn[] apim = p.columns;
                                        using (FbCommand cmd1 = new FbCommand())
                                        {
                                            cmd1.CommandText = "ADD_RANK_INFO";
                                            cmd1.CommandType = CommandType.StoredProcedure;
                                            cmd1.Connection = connection;
                                            cmd1.Parameters.Add("@LEAG_ID", dr["cid"].ToString());
                                            cmd1.Parameters.Add("@CLEAG_ALIAS", dr["ALIAS"].ToString());
                                            cmd1.Parameters.Add("@SEASON_ID", dr["sid"].ToString());
                                            cmd1.Parameters.Add("@TEAM_ID", p.id);
                                            cmd1.Parameters.Add("@TEAM", p.name);
                                            cmd1.Parameters.Add("@HKJC_TEAM", "");
                                            cmd1.Parameters.Add("@SCORE", p.columns.FirstOrDefault(x => x.id == "8").value);
                                            cmd1.Parameters.Add("@RANK", p.rank);
                                            cmd1.Parameters.Add("@FLAG", "0");
                                            cmd1.Parameters.Add("@GAMES", p.columns.FirstOrDefault(x => x.id == "1").value);// apim.Length > 1 ? apim[0].value : "0");
                                            cmd1.Parameters.Add("@IWON", p.columns.FirstOrDefault(x => x.id == "2").value);
                                            cmd1.Parameters.Add("@IDRAW", p.columns.FirstOrDefault(x => x.id == "3").value);
                                            cmd1.Parameters.Add("@ILOST", p.columns.FirstOrDefault(x => x.id == "4").value);
                                            cmd1.Parameters.Add("@CTIMESTAMP", DateTime.Now);
                                            int id = Convert.ToInt32(cmd1.ExecuteScalar());
                                        }

                                    }
                                }

                            }

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
                    this.dgvBookedEvent.currentPage--;
                }
                if (pageCurrent <= 0)
                {
                    pageCurrent++;
                    this.dgvEvent.currentPage++;
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

        private async void tsbGet_Click(object sender, EventArgs e)
        {
            try
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
                    ds = InsertData("competitions", false, tsdArea.Tag.ToString());
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
                    ds = InsertData("seasons", false, tsdComp.Tag.ToString());
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

                    if (tsdSeason.Tag == null || tsdComp.Tag == null) { this.dgvStages.DataSource = null; return; }

                    DataSet ds = new DataSet();
                    ds = InsertData("stages", false, tsdSeason.Tag.ToString(), tsdComp.Tag.ToString());
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

                    if (tsdStage.Tag == null || tsdSeason.Tag == null || tsdComp.Tag == null) { this.dgvGroups.DataSource = null; return; }

                    DataSet ds = new DataSet();
                    ds = InsertData("groups", false, tsdStage.Tag.ToString(), tsdSeason.Tag.ToString(), tsdComp.Tag.ToString());
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
                    ds = InsertData("participants", false, tsdSeason.Tag.ToString());
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
                    ds = InsertData("participants", false, tsdSeason.Tag.ToString());
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
                            ds = InsertData("events", false, "Group", tsdGroup.Tag.ToString(), tsdStage.Tag.ToString(), tsdSeason.Tag.ToString(), tsdComp.Tag.ToString());
                        }
                        else if (sender != null && ((ToolStripDropDownButton)sender).Name == "tsdStage")
                        {
                            ds = InsertData("events", false, "Stage", tsdStage.Tag.ToString(), tsdSeason.Tag.ToString(), tsdComp.Tag.ToString());
                        }
                        else if (sender != null && ((ToolStripDropDownButton)sender).Name == "tsdSeason")
                        {
                            ds = InsertData("events", false, "Season", tsdSeason.Tag.ToString(), tsdComp.Tag.ToString());
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
                        ///   ds = InsertData("events", true, this.bnAreas.Items[17].Text + " 00:00:00", this.bnAreas.Items[19].Text + " 23:59:59");
                        this.dgvEvent.DataSource = null;
                        this.tsbGet2.Enabled = false;
                        this.tsbGet2.Text = "Doing";
                        ds = await AyncHandleData("events", true, this.bnAreas.Items[17].Text + " 00:00:00", this.bnAreas.Items[19].Text + " 23:59:59");
                        this.tsbGet2.Enabled = true;
                        this.tsbGet2.Text = "Get";
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
                    ////this.dgvEvent.MasterControls(ref ds);
                    ////this.dgvEvent.setParentSource(ds.Tables[0].TableName, "ID");
                    //  dgvEvent.childView.Add(ds.Tables[1].TableName, "TEAMS");
                    //dgvEvent.childView.Add("", "TEAMS");
                    done = true;
                }
                else if (tabControl1.SelectedTab == tpBook)
                {
                    DataSet ds = new DataSet();
                    if (sender == null)
                    {
                        DataSet ds1 = new DataSet();
                        DataSet ds2 = new DataSet();
                        using (FbConnection connection = new FbConnection(AppFlag.HkjcDBConn))
                        {
                            //   string queryString = "SELECT t.* FROM teams t  inner  join  events e on   t.id= e.HOME_ID or t.id=e.GUEST_ID   where e.id='" + arr[1] + "' order by id asc";
                            //  string queryString = "SELECT e.* FROM matchlist e " + (AppFlag.SyncHkjcDateTime ==""?"": "where e.CTIMESTAMP > '"+ AppFlag.SyncHkjcDateTime+"'") + " order by e.CTIMESTAMP desc";
                            string queryString = "SELECT e.* FROM matchlist e   order by e.CMATCHDATETIME desc";

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
                                            ds1 = data;
                                        }
                                    }
                                }
                            }
                            connection.Close();
                        }

                        var culture = new System.Globalization.CultureInfo("zh-HK");
                        DateTime maxTime = DateTime.MinValue;
                        DateTime minTime = DateTime.MaxValue;

                        /// Files.WriteTestLog("Test", "HKjcMatch " + ds1.Tables["HKjcMatch"].Rows.Count);
                        if (ds1.Tables["HKjcMatch"].Rows.Count > 0)
                        {
                            maxTime = Convert.ToDateTime(ds1.Tables[0].Rows[0]["CMATCHDATETIME"]);
                            minTime = Convert.ToDateTime(ds1.Tables[0].Rows[ds1.Tables[0].Rows.Count - 1]["CMATCHDATETIME"]);
                            this.bnAreas.Items[17].Text = minTime.ToString("yyyy-MM-dd", culture);
                            this.bnAreas.Items[19].Text = maxTime.ToString("yyyy-MM-dd", culture);
                        }

                        // DataRow[] rows = ds1.Tables[0].Select((AppFlag.SyncHkjcDateTime == "" ? "" : "CTIMESTAMP >'" + AppFlag.SyncHkjcDateTime + "'"), "CTIMESTAMP DESC");
                        DataTable table = ds1.Tables[0];
                        if (table.Rows.Count > 0)
                        {
                            table.DefaultView.RowFilter = (AppFlag.SyncHkjcDateTime == "" ? "" : "CTIMESTAMP >'" + AppFlag.SyncHkjcDateTime + "'");
                            table.DefaultView.Sort = "CTIMESTAMP DESC";
                            table = table.DefaultView.ToTable();
                            /// Files.WriteTestLog("Test", "HKjcMatch 2 " + table.Rows.Count);
                        }
                        if (table.Rows.Count > 0)
                        {
                            Files.UpdateConfig("SyncHkjcDateTime", Convert.ToDateTime(table.Rows[0]["CTIMESTAMP"]).ToString("yyyy-MM-dd HH:mm:ss.fff", null));
                            Files.WriteLog("Update SyncHkjcDateTime " + Convert.ToDateTime(table.Rows[0]["CTIMESTAMP"]).ToString("yyyy-MM-dd HH:mm:ss.fff", null));
                            AppFlag.SyncHkjcDateTime = Convert.ToDateTime(table.Rows[0]["CTIMESTAMP"]).ToString("yyyy-MM-dd HH:mm:ss.fff", null);

                            using (FbConnection connection = new FbConnection(AppFlag.ScoutsDBConn))
                            {
                                connection.Open();
                                int iIndiex = 0;
                                List<int> indexs = new List<int>();
                                foreach (DataRow dr1 in table.Rows)
                                {
                                    using (FbCommand cmd2 = new FbCommand())
                                    {
                                        cmd2.CommandText = "ADD_HKJCMATCH";
                                        cmd2.CommandType = CommandType.StoredProcedure;
                                        cmd2.Connection = connection;
                                        // cmd2.Parameters.Add("@EMATCHID", null);
                                        cmd2.Parameters.Add("@HKJCMATCHNO", dr1["IMATCH_NO"]);
                                        cmd2.Parameters.Add("@HKJCDAYCODE", dr1["CMATCH_DAY_CODE"]);
                                        cmd2.Parameters.Add("@CMATCHDATETIME", dr1["CMATCHDATETIME"]);
                                        cmd2.Parameters.Add("@HKJCHOSTID", dr1["IHOME_TEAM_CODE"]);
                                        cmd2.Parameters.Add("@HKJCGUESTID", dr1["IAWAY_TEAM_CODE"]);
                                        cmd2.Parameters.Add("@HKJCHOSTNAME", dr1["CHOME_TEAM_ENG_NAME"]);
                                        cmd2.Parameters.Add("@HKJCGUESTNAME", dr1["CAWAY_TEAM_ENG_NAME"]);
                                        cmd2.Parameters.Add("@HKJCHOSTNAME_CN", dr1["CHOME_TEAM_OUTPUT_NAME"]);
                                        cmd2.Parameters.Add("@HKJCGUESTNAME_CN", dr1["CAWAY_TEAM_OUTPUT_NAME"]);
                                        cmd2.Parameters.Add("@CLEAGUE_HKJC_NAME", dr1["CLEAGUE_HKJC_NAME"]);
                                        cmd2.Parameters.Add("@CLEAGUE_OUTPUT_NAME", dr1["CLEAGUE_OUTPUT_NAME"]);
                                        cmd2.Parameters.Add("@CLEAGUEALIAS_OUTPUT_NAME", dr1["CLEAGUEALIAS_OUTPUT_NAME"]);
                                        cmd2.Parameters.Add("@CLEAGUE_SHT_ENG_NAME", dr1["CLEAGUE_SHT_ENG_NAME"]);
                                        cmd2.Parameters.Add("@CLEAGUE_ENG_NAME", dr1["CLEAGUE_ENG_NAME"]);
                                        cmd2.Parameters.Add("@STATUS", dr1["ISTATUS"]);
                                        cmd2.Parameters.Add("@MAPPINGSTATUS", null);
                                        cmd2.Parameters.Add("@CTIMESTAMP", Convert.ToDateTime(dr1["CTIMESTAMP"]).ToString("yyyy-MM-dd HH:mm:ss.fff", null));
                                        int id = Convert.ToInt32(cmd2.ExecuteScalar());
                                        // Files.WriteLog((id == 0 ? " [Success] Insert EMATCHES " : "Match exist "+id+" ") + "[" + dr1["IMATCH_NO"] + " " + dr1["CMATCH_DAY_CODE"] + "] " + " " + dr1["CHOME_TEAM_ENG_NAME"] + "/" + dr1["CAWAY_TEAM_ENG_NAME"]);
                                        Files.WriteLog((id == 0 ? " [Success] Insert EMATCHES " : "Match exist ") + "[" + dr1["IMATCH_NO"] + " " + dr1["CMATCH_DAY_CODE"] + "] " + " " + dr1["CHOME_TEAM_ENG_NAME"] + "/" + dr1["CAWAY_TEAM_ENG_NAME"]);
                                        if (id > 1)
                                        {
                                            //   Files.WriteTestLog("Test", "table [" + id + "] "+ table.Rows[iIndiex]["CHOME_TEAM_ENG_NAME"].ToString()+"/"+ table.Rows[iIndiex]["CAWAY_TEAM_ENG_NAME"].ToString());
                                            ///    Files.WriteTestLog("Test", "table " + table.Rows[iIndiex]["CHOME_TEAM_ENG_NAME"].ToString() + "/" + table.Rows[iIndiex]["CAWAY_TEAM_ENG_NAME"].ToString());
                                            //table.Rows[iIndiex].Delete();
                                            indexs.Add(iIndiex);
                                        }
                                    }
                                    iIndiex++;
                                }
                                table.AcceptChanges();
                                foreach (int i in indexs)
                                {
                                    table.Rows[i].Delete();
                                }
                                table.AcceptChanges();
                                connection.Close();

                                //Files.UpdateConfig("SyncHkjcDateTime", Convert.ToDateTime(table.Rows[0]["CTIMESTAMP"]).ToString("yyyy-MM-dd HH:mm:ss.fff", null));
                                //AppFlag.SyncHkjcDateTime = Convert.ToDateTime(table.Rows[0]["CTIMESTAMP"]).ToString("yyyy-MM-dd HH:mm:ss.fff", null);

                                ///Files.WriteTestLog("Test", "table2  " + table.Rows.Count);
                                //RunSyncHkjcAndBook(ds1);
                                // await SyncHkjcAndBook(ds);
                                // await RunBookEventAction();

                                ///no need book when app -bookedevent clicked
                                ///     RunSyncHkjcAndBook2(table);
                            }
                        }

                        using (FbConnection connection = new FbConnection(AppFlag.ScoutsDBConn))
                        {
                            // string queryString = "SELECT t.* FROM teams t  inner  join  events e on   t.id= e.HOME_ID or t.id=e.GUEST_ID   where e.id='" + arr[1] + "' order by id asc";
                            // string queryString = "SELECT e.*,r.start_date ,r.STATUS_ID,r.STATUS_NAME  FROM EMATCHES e left join events r on e.ematchid= r.id  where  '" + minTime.ToString("yyyy-MM-dd HH:mm:ss.fff", null) + "'<=  e.CMATCHDATETIME and  e.CMATCHDATETIME <='" + maxTime.ToString("yyyy-MM-dd HH:mm:ss.fff", null) + "' order by  e.EMATCHID desc ";
                            string queryString = "SELECT e.*,r.start_date ,r.STATUS_ID,r.STATUS_NAME  FROM EMATCHES e left join events r on e.ematchid= r.id  where  '" + minTime.ToString("yyyy-MM-dd HH:mm:ss.fff", null) + "'<=  e.CMATCHDATETIME and  e.CMATCHDATETIME <='" + maxTime.ToString("yyyy-MM-dd HH:mm:ss.fff", null) + "' order by E.CMATCHDATETIME ASC ";
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
                                            data.Tables.Add(new DataTable("SocoutMatch"));
                                            fda.Fill(data.Tables["SocoutMatch"]);
                                            ds2 = data;
                                        }
                                        ds = ds2;
                                    }
                                }
                            }
                            connection.Close();
                        }

                        using (FbConnection connection = new FbConnection(AppFlag.ScoutsDBConn))
                        {
                            string queryString = "";
                            /// 20190213  if (!AppFlag.AutoBooked)
                            ////if(true)
                            ////{
                            queryString = "select a.id,a.name,a.booked_by from(" +
                                       "select* from events e  where '" + minTime.ToString("yyyy-MM-dd", null) + "'<=  e.start_date and  e.start_date <='" + maxTime.AddDays(1).ToString("yyyy-MM-dd", null) + "' and e.booked = true) a " +
                                       "where not exists(" +
                                       "select r.EMATCHID from EMATCHES r   where    r.EMATCHID is not  null and a.id = r.EMATCHID and '" + minTime.ToString("yyyy-MM-dd HH:mm:ss.fff", null) + "'<=  r.CMATCHDATETIME  and r.CMATCHDATETIME<='" + maxTime.ToString("yyyy-MM-dd HH:mm:ss.fff", null) + "'  ) order by a.name  ";
                            ////}
                            ////else
                            ////{
                            ////    queryString = "select a.id,a.name,a.booked_by from(" +
                            ////                "select* from events e  where '" + this.bnAreas.Items[17].Text + minTime.ToString(" HH:mm:ss.fff", null) + "'<=  e.start_date and  e.start_date <='" + this.bnAreas.Items[19].Text + maxTime.ToString(" HH:mm:ss.fff", null) + "' ) a " +
                            ////                "where not exists(" +
                            ////                "select r.EMATCHID from EMATCHES r   where    r.EMATCHID is not  null and a.id = r.EMATCHID and '" + minTime.ToString("yyyy-MM-dd HH:mm:ss.fff", null) + "'<=  r.CMATCHDATETIME  and r.CMATCHDATETIME<='" + maxTime.ToString("yyyy-MM-dd HH:mm:ss.fff", null) + "' ) order by a.name  ";
                            ////}
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
                                            fda.Fill(data);
                                            cmsBooked.Items.Clear();
                                            cmsBooked.Items.Add(new ToolStripMenuItem()
                                            {
                                                Text = "Cancel Sync",
                                                Tag = ""
                                            });
                                            foreach (DataRow dr in data.Tables[0].Rows)
                                            {
                                                cmsBooked.Items.Add(new ToolStripMenuItem()
                                                {
                                                    Text = dr["id"].ToString() + " " + dr["Name"].ToString() + " " + dr["booked_by"].ToString(),
                                                    Tag = dr["id"].ToString()
                                                });
                                                Thread.Sleep(10);
                                            }
                                        }
                                    }
                                }
                                connection.Close();
                            }
                        }


                        //DataSet ds = new DataSet();
                        //var ls1 = ds1.Tables["matches1"].AsEnumerable().ToList();
                        //var ls2 = ds2.Tables["matches2"].AsEnumerable().ToList();

                        ////var result = from x in ls1 join y in ls2
                        //// .Where(y => y["HKJCMATCHNO"] == x["IMATCH_NO"] && y["HKJCDAYCODE"] == x["CMATCH_DAY_CODE"]);

                        //var result2 = from x in ls1
                        //              join y in ls2
                        //              on new { X1 = x["HKJCMATCHNO"], X2 = x["HKJCDAYCODE"] } equals new { X1 = y["IMATCH_NO"], X2 = y["CMATCH_DAY_CODE"] }
                        //              //into lj
                        //              // from r in lj.DefaultIfEmpty()
                        //              // select ds2.Tables["matches2"].LoadDataRow(new object[]
                        //              //{
                        //              //  y.Field<string>("IMATCH_NO"),
                        //              //  y.Field<string>("CMATCH_DAY_CODE"),
                        //              // y.Field<string>("CMATCHDATETIME"),
                        //              //  y.Field<string>("IHOME_TEAM_CODE"),
                        //              //  y.Field<string>("IAWAY_TEAM_CODE"),
                        //              //  y.Field<string>("CHOME_TEAM_ENG_NAME"),
                        //              //  y.Field<string>("CAWAY_TEAM_ENG_NAME"),
                        //              //  y.Field<string>("ISTATUS")
                        //              // }, false);

                        //              select new
                        //              {
                        //                  HKJCMATCHNO = y["IMATCH_NO"],
                        //                  HKJCDAYCODE = y["CMATCH_DAY_CODE"],
                        //                  CMATCHDATETIME = y["CMATCHDATETIME"],
                        //                  HKJCHOSTID = y["IHOME_TEAM_CODE"],
                        //                  HKJCGUESTID = y["IAWAY_TEAM_CODE"],
                        //                  HKJCHOSTNAME = y["CHOME_TEAM_ENG_NAME"],
                        //                  HKJCGUESTNAME = y["CAWAY_TEAM_ENG_NAME"],
                        //                  STATUS = y["ISTATUS"]
                        //              };

                        //var result = from dataRows1 in ls2
                        //             join dataRows2 in ls1
                        //             on
                        //             new { X1 = dataRows1["IMATCH_NO"], X2 = dataRows1["CMATCH_DAY_CODE"] } equals new { X1 = dataRows2["HKJCMATCHNO"], X2 = dataRows2["HKJCDAYCODE"] }
                        //             //dataRows1.Field<string>("HKJCMATCHNO") equals dataRows2.Field<string>("IMATCH_NO") 
                        //             into lj
                        //             from r in lj.DefaultIfEmpty()
                        //             select new
                        //             {
                        //                 HKJCMATCHNO = dataRows1["IMATCH_NO"],
                        //                 HKJCDAYCODE = dataRows1["CMATCH_DAY_CODE"],
                        //                 CMATCHDATETIME = dataRows1["CMATCHDATETIME"],
                        //                 HKJCHOSTID = dataRows1["IHOME_TEAM_CODE"],
                        //                 HKJCGUESTID = dataRows1["IAWAY_TEAM_CODE"],
                        //                 HKJCHOSTNAME = dataRows1["CHOME_TEAM_ENG_NAME"],
                        //                 HKJCGUESTNAME = dataRows1["CAWAY_TEAM_ENG_NAME"],
                        //                 STATUS = dataRows1["ISTATUS"]
                        //             };
                        //                select ds2.Tables["matches2"].LoadDataRow(new object[]
                        //{
                        //   dataRows1.Field<int>("IMATCH_NO"),
                        //   dataRows1.Field<string>("CMATCH_DAY_CODE"),
                        //   dataRows1.Field<DateTime>("CMATCHDATETIME"),
                        //   dataRows1.Field<int>("IHOME_TEAM_CODE"),
                        //   dataRows1.Field<int>("IAWAY_TEAM_CODE"),
                        //   dataRows1.Field<string>("CHOME_TEAM_ENG_NAME"),
                        //   dataRows1.Field<string>("CAWAY_TEAM_ENG_NAME"),
                        //   dataRows1.Field<int>("ISTATUS")
                        // }
                        // , false);

                        //var results = from x in ls1 join y in ls2   on
                        //              where x["HKJCMATCHNO"] != y["HKJCDAYCODE"] && x["HKJCDAYCODE"] == y["CMATCH_DAY_CODE"]
                        //              select x;

                        // var result3 = ls2.SelectMany(a => ls1.Where(xi => xi["HKJCMATCHNO"] == a["IMATCH_NO"] && xi["HKJCDAYCODE"] == a["CMATCH_DAY_CODE"])).ToList();
                        //(a, b) => new ResultItem
                        //{
                        //    id = a["HKJCMATCHNO"],
                        //    name = a["HKJCDAYCODE"]
                        //}).ToList();
#pragma warning disable CS0219 // The variable 'sd' is assigned but its value is never used
                        string sd = "";
#pragma warning restore CS0219 // The variable 'sd' is assigned but its value is never used
                        // var query = (from x in ls1 select new { A = x["HKJCMATCHNO"], B = x["HKJCDAYCODE"] }).Concat(from y in ls2 select new { A = y["IMATCH_NO"], B = y["CMATCH_DAY_CODE"] });

                        //var db1 = (from a in ds1.Tables["matches1"] select a).ToList();
                        //var db2 = (from a in ds1.Tables["matches1"] select a).ToList();

                        //var query = (from a in db1
                        //             join b in db2 on a.EnteredBy equals b.UserId
                        //             where a.LHManifestNum == LHManifestNum
                        //             select new { LHManifestId = a.LHManifestId, LHManifestNum = a.LHManifestNum, LHManifestDate = a.LHManifestDate, StnCode = a.StnCode, Operatr = b.UserName }).FirstOrDefault();


                        //DataSet ds = new DataSet();
                        //if (sender != null && sender.GetType() != typeof(ToolStripButton))
                        //{
                        //    if (sender != null && ((ToolStripDropDownButton)sender).Name == "tsdGroup")
                        //    {
                        //        ds = InsertData("booked-events", false, "Group", tsdGroup.Tag.ToString());
                        //    }
                        //    else if (sender != null && ((ToolStripDropDownButton)sender).Name == "tsdStage")
                        //    {
                        //        ds = InsertData("booked-events", false, "Stage", tsdStage.Tag.ToString());
                        //    }
                        //    else if (sender != null && ((ToolStripDropDownButton)sender).Name == "tsdSeason")
                        //    {
                        //        ds = InsertData("booked-events", false, "Season", tsdSeason.Tag.ToString());
                        //    }
                        //    else if (sender != null && ((ToolStripDropDownButton)sender).Name == "tsdComp")
                        //    {
                        //        ds = InsertData("booked-events", false, "Comp", tsdComp.Tag.ToString());
                        //    }
                        //    else if (sender != null && ((ToolStripDropDownButton)sender).Name == "tsdArea")
                        //    { }
                        //}
                        //else if (sender != null && ((ToolStripButton)sender) != null && ((ToolStripButton)sender).Name == "tsbGet2")
                        //{

                        //    if (Convert.ToDateTime(this.bnAreas.Items[19].Text).Subtract(Convert.ToDateTime(this.bnAreas.Items[17].Text)).Days > 30 && DateTime.Now <= Convert.ToDateTime(this.bnAreas.Items[19].Text + " 23:59:59"))
                        //    {
                        //        MessageBox.Show("Maximum period is 30 days!"); return;
                        //    }
                        //    ds = InsertData("booked-events", true, this.bnAreas.Items[17].Text + " 00:00:00", this.bnAreas.Items[19].Text + " 23:59:59");
                        //}
                        //else if (sender == null)
                        //{
                        //    ds = InsertData("booked-events", false, this.bnAreas.Items[17].Text + " 00:00:00", this.bnAreas.Items[19].Text + " 23:59:59");
                        //}
                    }
                    else
                    {
                        using (FbConnection connection = new FbConnection(AppFlag.ScoutsDBConn))
                        {
                            // string queryString = "SELECT t.* FROM teams t  inner  join  events e on   t.id= e.HOME_ID or t.id=e.GUEST_ID   where e.id='" + arr[1] + "' order by id asc";
                            string queryString = "SELECT e.* FROM EMATCHES e where  '" + this.bnAreas.Items[17].Text + " 00:00:00" + "'<=  e.CMATCHDATETIME and  e.CMATCHDATETIME <='" + this.bnAreas.Items[19].Text + " 23:59:59" + "' order by  e.CMATCHDATETIME ASC ";
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
                                            data.Tables.Add(new DataTable("SocoutMatch"));
                                            fda.Fill(data.Tables["SocoutMatch"]);
                                            ds = data;
                                        }

                                    }
                                }
                            }
                            connection.Close();
                        }
                    }

                    if (ds.Tables.Count == 0) { this.dgvBookedEvent.DataSource = null; return; }
                    tbData = ds.Tables[0];

                    BindingSource bs = new BindingSource();
                    bs.DataSource = tbData.DefaultView;
                    bnAreas.BindingSource = bs;

                    this.dgvBookedEvent.MasterControls(ref ds);
                    this.dgvBookedEvent.setParentSource(ds.Tables[0].TableName, "EMATCHID");

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
                this.dgvEvent.currentPage = 1;
                this.dgvBookedEvent.currentPage = 1;
                currentRow = 0;

                this.LoadData(tabControl1.SelectedTab.Text);
            }
            catch (Exception exp)
            {
                Files.WriteError("tsbGet_Click(),error: " + exp.Message);
            }
        }
        private bool AutoRunSync()
        {
            try
            {
                //return Task.Run(() =>
                //   {
                week = Day[Convert.ToInt32(DateTime.Now.DayOfWeek.ToString("d"))].ToString();

                DataSet ds1 = new DataSet();
                using (FbConnection connection = new FbConnection(AppFlag.HkjcDBConn))
                {
                    string queryString = "SELECT e.* FROM matchlist e   order by e.CMATCHDATETIME desc";
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
                                    ds1 = data;
                                }
                            }
                        }
                    }
                    connection.Close();
                }

                var culture = new System.Globalization.CultureInfo("zh-HK");
                DateTime maxTime = DateTime.MinValue;
                DateTime minTime = DateTime.MaxValue;

                if (ds1.Tables["HKjcMatch"].Rows.Count > 0)
                {
                    maxTime = Convert.ToDateTime(ds1.Tables[0].Rows[0]["CMATCHDATETIME"]);
                    minTime = Convert.ToDateTime(ds1.Tables[0].Rows[ds1.Tables[0].Rows.Count - 1]["CMATCHDATETIME"]);
                    if (this.bnAreas.InvokeRequired)
                    {
                        //textbox1.Invoke(new MethodInvoker(delegate { name = textbox1.text; }));
                        this.bnAreas.Invoke(new MethodInvoker(delegate { this.bnAreas.Items[17].Text = minTime.ToString("yyyy-MM-dd", culture); }));
                        this.bnAreas.Invoke(new MethodInvoker(delegate { this.bnAreas.Items[19].Text = maxTime.ToString("yyyy-MM-dd", culture); }));
                    }
                }

                DataTable table = ds1.Tables[0];
                if (table.Rows.Count > 0)
                {
                    table.DefaultView.RowFilter = (AppFlag.SyncHkjcDateTime == "" ? "" : "CTIMESTAMP >'" + AppFlag.SyncHkjcDateTime + "'");
                    table.DefaultView.Sort = "CTIMESTAMP DESC";
                    table = table.DefaultView.ToTable();
                }
                //20190218 run get player every 5 
                ////if (table.Rows.Count > 0)
                ////{
                ////    Files.UpdateConfig("SyncHkjcDateTime", Convert.ToDateTime(table.Rows[0]["CTIMESTAMP"]).ToString("yyyy-MM-dd HH:mm:ss.fff", null));
                ////    Files.WriteLog("Update SyncHkjcDateTime " + Convert.ToDateTime(table.Rows[0]["CTIMESTAMP"]).ToString("yyyy-MM-dd HH:mm:ss.fff", null));
                ////    AppFlag.SyncHkjcDateTime = Convert.ToDateTime(table.Rows[0]["CTIMESTAMP"]).ToString("yyyy-MM-dd HH:mm:ss.fff", null);

                ////    using (FbConnection connection = new FbConnection(AppFlag.ScoutsDBConn))
                ////    {
                ////        connection.Open();
                ////        foreach (DataRow dr1 in table.Rows)
                ////        {
                ////            using (FbCommand cmd2 = new FbCommand())
                ////            {
                ////                cmd2.CommandText = "ADD_HKJCMATCH";
                ////                cmd2.CommandType = CommandType.StoredProcedure;
                ////                cmd2.Connection = connection;
                ////                // cmd2.Parameters.Add("@EMATCHID", null);
                ////                cmd2.Parameters.Add("@HKJCMATCHNO", dr1["IMATCH_NO"]);
                ////                cmd2.Parameters.Add("@HKJCDAYCODE", dr1["CMATCH_DAY_CODE"]);
                ////                cmd2.Parameters.Add("@CMATCHDATETIME", dr1["CMATCHDATETIME"]);
                ////                cmd2.Parameters.Add("@HKJCHOSTID", dr1["IHOME_TEAM_CODE"]);
                ////                cmd2.Parameters.Add("@HKJCGUESTID", dr1["IAWAY_TEAM_CODE"]);
                ////                cmd2.Parameters.Add("@HKJCHOSTNAME", dr1["CHOME_TEAM_ENG_NAME"]);
                ////                cmd2.Parameters.Add("@HKJCGUESTNAME", dr1["CAWAY_TEAM_ENG_NAME"]);
                ////                  cmd2.Parameters.Add("@HKJCHOSTNAME_CN", dr1["CHOME_TEAM_OUTPUT_NAME"]);
                ////                        cmd2.Parameters.Add("@HKJCGUESTNAME_CN", dr1["CAWAY_TEAM_OUTPUT_NAME"]);
                ////                cmd2.Parameters.Add("@CLEAGUE_OUTPUT_NAME", dr1["CLEAGUE_OUTPUT_NAME"]);
                ////                cmd2.Parameters.Add("@CLEAGUEALIAS_OUTPUT_NAME", dr1["CLEAGUEALIAS_OUTPUT_NAME"]);
                ////                cmd2.Parameters.Add("@STATUS", dr1["ISTATUS"]);
                ////                cmd2.Parameters.Add("@MAPPINGSTATUS", null);
                ////                cmd2.Parameters.Add("@CTIMESTAMP", Convert.ToDateTime(dr1["CTIMESTAMP"]).ToString("yyyy-MM-dd HH:mm:ss.fff", null));
                ////                int id = Convert.ToInt32(cmd2.ExecuteScalar());
                ////                Files.WriteLog((id == 0 ? " [Success] Insert EMATCHES " : "Match exist ") + "[" + dr1["IMATCH_NO"] + " " + dr1["CMATCH_DAY_CODE"] + "] " + " " + dr1["CHOME_TEAM_ENG_NAME"] + "/" + dr1["CAWAY_TEAM_ENG_NAME"]);
                ////            }
                ////        }
                ////        // connection.Close(); 
                ////        string queryString = "SELECT r.id, r.NAME, r.START_DATE,e.ematchid, e.HKJCDAYCODE,e.HKJCMATCHNO,e.HKJCHOSTNAME,e.HKJCGUESTNAME,e.MAPPINGSTATUS,e.CMATCHDATETIME FROM ematches e left join events r on r.id=e.ematchid  where '" + minTime.ToString("yyyy-MM-dd HH:mm:ss.fff", null) + "'<=  e.CMATCHDATETIME  and e.CMATCHDATETIME<='" + maxTime.ToString("yyyy-MM-dd HH:mm:ss.fff", null) + "'  and ((e.EMATCHID<1 or e.EMATCHID is null) "+ (AppFlag.AutoBooked?" or ( e.MAPPINGSTATUS is null AND e.EMATCHID>0 ) ":"")+" ) order by e.CMATCHDATETIME desc";
                ////        if (AppFlag.TestMode) Files.WriteTestLog("Test", "HKjcMatch " + queryString);
                ////        using (FbCommand cmd = new FbCommand(queryString, connection))
                ////        {
                ////            using (FbCommandBuilder fcb = new FbCommandBuilder())
                ////            {
                ////                using (FbDataAdapter fda = new FbDataAdapter())
                ////                {
                ////                    fda.SelectCommand = cmd;
                ////                    fcb.DataAdapter = fda;
                ////                    using (DataSet data = new DataSet())
                ////                    {
                ////                        data.Tables.Add(new DataTable("HKjcMatch"));
                ////                        fda.Fill(data.Tables["HKjcMatch"]);
                ////                        table = data.Tables["HKjcMatch"];
                ////                    }
                ////                }
                ////            }
                ////        }
                ////        connection.Close();

                ////        //Files.UpdateConfig("SyncHkjcDateTime", Convert.ToDateTime(table.Rows[0]["CTIMESTAMP"]).ToString("yyyy-MM-dd HH:mm:ss.fff", null));
                ////        //AppFlag.SyncHkjcDateTime = Convert.ToDateTime(table.Rows[0]["CTIMESTAMP"]).ToString("yyyy-MM-dd HH:mm:ss.fff", null);
                ////       if(AppFlag.TestMode) Files.WriteTestLog("Test", "HKjcMatch " + table.Rows.Count); 
                ////        RunSyncHkjcAndBook3(table);
                ////    }
                ////}

                //if (table.Rows.Count > 0)
                //{
                //Files.UpdateConfig("SyncHkjcDateTime", Convert.ToDateTime(table.Rows[0]["CTIMESTAMP"]).ToString("yyyy-MM-dd HH:mm:ss.fff", null));
                //Files.WriteLog("Update SyncHkjcDateTime " + Convert.ToDateTime(table.Rows[0]["CTIMESTAMP"]).ToString("yyyy-MM-dd HH:mm:ss.fff", null));
                //AppFlag.SyncHkjcDateTime = Convert.ToDateTime(table.Rows[0]["CTIMESTAMP"]).ToString("yyyy-MM-dd HH:mm:ss.fff", null);

                using (FbConnection connection = new FbConnection(AppFlag.ScoutsDBConn))
                {
                    connection.Open();
                    if (table.Rows.Count > 0)
                    {
                        Files.UpdateConfig("SyncHkjcDateTime", Convert.ToDateTime(table.Rows[0]["CTIMESTAMP"]).ToString("yyyy-MM-dd HH:mm:ss.fff", null));
                        Files.WriteLog("Update SyncHkjcDateTime " + Convert.ToDateTime(table.Rows[0]["CTIMESTAMP"]).ToString("yyyy-MM-dd HH:mm:ss.fff", null));
                        AppFlag.SyncHkjcDateTime = Convert.ToDateTime(table.Rows[0]["CTIMESTAMP"]).ToString("yyyy-MM-dd HH:mm:ss.fff", null);

                        foreach (DataRow dr1 in table.Rows)
                        {
                            using (FbCommand cmd2 = new FbCommand())
                            {
                                cmd2.CommandText = "ADD_HKJCMATCH";
                                cmd2.CommandType = CommandType.StoredProcedure;
                                cmd2.Connection = connection;
                                // cmd2.Parameters.Add("@EMATCHID", null);
                                cmd2.Parameters.Add("@HKJCMATCHNO", dr1["IMATCH_NO"]);
                                cmd2.Parameters.Add("@HKJCDAYCODE", dr1["CMATCH_DAY_CODE"]);
                                cmd2.Parameters.Add("@CMATCHDATETIME", dr1["CMATCHDATETIME"]);
                                cmd2.Parameters.Add("@HKJCHOSTID", dr1["IHOME_TEAM_CODE"]);
                                cmd2.Parameters.Add("@HKJCGUESTID", dr1["IAWAY_TEAM_CODE"]);
                                cmd2.Parameters.Add("@HKJCHOSTNAME", dr1["CHOME_TEAM_ENG_NAME"]);
                                cmd2.Parameters.Add("@HKJCGUESTNAME", dr1["CAWAY_TEAM_ENG_NAME"]);
                                //cmd2.Parameters.Add("@HKJCHOSTNAME_CN", dr1["CHOME_TEAM_OUTPUT_NAME"]);
                                //cmd2.Parameters.Add("@HKJCGUESTNAME_CN", dr1["CAWAY_TEAM_OUTPUT_NAME"]);
                                cmd2.Parameters.Add("@HKJCHOSTNAME_CN", dr1["CHOME_TEAM_HKJC_NAME"].ToString ().Substring(0, dr1["CHOME_TEAM_HKJC_NAME"].ToString().IndexOf("/")));
                                cmd2.Parameters.Add("@HKJCGUESTNAME_CN", dr1["CAWAY_TEAM_HKJC_NAME"].ToString().Substring(0, dr1["CAWAY_TEAM_HKJC_NAME"].ToString().IndexOf("/")));
                                cmd2.Parameters.Add("@CLEAGUE_HKJC_NAME", dr1["CLEAGUE_HKJC_NAME"]);
                                cmd2.Parameters.Add("@CLEAGUE_OUTPUT_NAME", dr1["CLEAGUE_OUTPUT_NAME"]);
                                cmd2.Parameters.Add("@CLEAGUEALIAS_OUTPUT_NAME", dr1["CLEAGUEALIAS_OUTPUT_NAME"]);
                                cmd2.Parameters.Add("@CLEAGUE_SHT_ENG_NAME", dr1["CLEAGUE_SHT_ENG_NAME"]);
                                cmd2.Parameters.Add("@CLEAGUE_ENG_NAME", dr1["CLEAGUE_ENG_NAME"]);
                                cmd2.Parameters.Add("@STATUS", dr1["ISTATUS"]);
                                cmd2.Parameters.Add("@MAPPINGSTATUS", null);
                                cmd2.Parameters.Add("@CTIMESTAMP", Convert.ToDateTime(dr1["CTIMESTAMP"]).ToString("yyyy-MM-dd HH:mm:ss.fff", null));
                                int id = Convert.ToInt32(cmd2.ExecuteScalar());
                                Files.WriteLog((id == 0 ? " [Success] Insert EMATCHES " : "Match exist ") + "[" + dr1["IMATCH_NO"] + " " + dr1["CMATCH_DAY_CODE"] + "] " + " " + dr1["CHOME_TEAM_ENG_NAME"] + "/" + dr1["CAWAY_TEAM_ENG_NAME"]);
                            }
                        }
                    }
                    // connection.Close(); 
                    string queryString = "SELECT r.id, r.NAME, r.START_DATE,e.ematchid, e.HKJCDAYCODE,e.HKJCMATCHNO,e.HKJCHOSTNAME,e.HKJCGUESTNAME,e.MAPPINGSTATUS,e.CMATCHDATETIME,e.CLEAGUEALIAS_OUTPUT_NAME FROM ematches e left join events r on r.id=e.ematchid  where '" + minTime.ToString("yyyy-MM-dd HH:mm:ss.fff", null) + "'<=  e.CMATCHDATETIME  and e.CMATCHDATETIME<='" + maxTime.ToString("yyyy-MM-dd HH:mm:ss.fff", null) + "'  and ((e.EMATCHID<1 or e.EMATCHID is null) " + (AppFlag.AutoBooked ? " or ( e.MAPPINGSTATUS is null AND e.EMATCHID>0 ) " : "") + " ) order by e.CMATCHDATETIME desc";
                    if (AppFlag.TestMode) Files.WriteTestLog("Test", "HKjcMatch " + queryString);
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
                                    table = data.Tables["HKjcMatch"];
                                }
                            }
                        }
                    }
                    connection.Close();

                    //Files.UpdateConfig("SyncHkjcDateTime", Convert.ToDateTime(table.Rows[0]["CTIMESTAMP"]).ToString("yyyy-MM-dd HH:mm:ss.fff", null));
                    //AppFlag.SyncHkjcDateTime = Convert.ToDateTime(table.Rows[0]["CTIMESTAMP"]).ToString("yyyy-MM-dd HH:mm:ss.fff", null);
                    if (AppFlag.TestMode) Files.WriteTestLog("Test", "HKjcMatch " + table.Rows.Count);
                    RunSyncHkjcAndBook3(table);
                }
                //}

                return true;
                //});
            }
            catch (Exception exp)
            {
                Files.WriteError("AutoRunSync(),error: " + exp.Message);
                return false;
            }
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
            DataSet ds1 = InsertData("competitions", false, e.ClickedItem.Tag.ToString());
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
            this.tsdComp.Tag = null;

            this.tsbGet_Click(sender, e);
        }

        private void tsdSeason_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            this.tsdSeason.Text = e.ClickedItem.Text;
            this.tsdSeason.Tag = e.ClickedItem.Tag;

            this.tsdStage.DropDownItems.Clear();
            DataSet ds2 = InsertData("stages", false, tsdSeason.Tag.ToString(), tsdComp.Tag.ToString());
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
            DataSet ds2 = InsertData("seasons", false, tsdComp.Tag.ToString());
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
            DataSet ds2 = InsertData("groups", false, tsdStage.Tag.ToString(), tsdSeason.Tag.ToString(), tsdComp.Tag.ToString());
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
            try
            {
                Rectangle rectangle = new Rectangle(Conversions.ToInteger(Microsoft.VisualBasic.CompilerServices.Operators.DivideObject(Microsoft.VisualBasic.CompilerServices.Operators.SubtractObject(this.dgvEvent.rowDefaultHeight, 0x10), 2)), Conversions.ToInteger(Microsoft.VisualBasic.CompilerServices.Operators.DivideObject(Microsoft.VisualBasic.CompilerServices.Operators.SubtractObject(this.dgvEvent.rowDefaultHeight, 0x10), 2)), 0x10, 0x10);
                if (rectangle.Contains(e.Location))
                {
                    if (this.dgvEvent.rowCurrent.Contains(e.RowIndex))
                    {
                        this.dgvEvent.rowCurrent.Clear();
                        this.dgvEvent.Rows[e.RowIndex].Height = Conversions.ToInteger(this.dgvEvent.rowDefaultHeight);
                        this.dgvEvent.Rows[e.RowIndex].DividerHeight = Conversions.ToInteger(this.dgvEvent.rowDefaultDivider);
                    }
                    else
                    {
                        ////ADD TEAM INFO   
                        //string team_id = this.dgvEvent.Rows[e.RowIndex].Cells[0].Value.ToString();
                        //DataSet data = new DataSet();
                        //data = InsertData("events.participants", false, team_id); 
                        //if (data.Tables[0].Rows.Count == 0)
                        //{
                        //    data = InsertData("events.participants", true, team_id);
                        //}

                        //ADD TEAM INFO   
                        string event_id = this.dgvEvent.Rows[e.RowIndex].Cells[0].Value.ToString();
                        DataSet data = new DataSet();
                        data = InsertData("events.show", false, event_id);
                        // if (data.Tables[0].Rows.Count == 0)
                        if (data.Tables[0].Rows.Count < 2)
                        {
                            data = InsertData("events.show", true, event_id);
                        }

                        if (dgvEvent.childView.TabPages.Count == 0)
                        {
                            dgvEvent.childView.AddData(data.Tables[0], "Teams");
                            dgvEvent.childView.AddData(data.Tables[1], "HPlayers");
                            dgvEvent.childView.AddData(data.Tables[2], "GPlayers");
                        }
                        else if (dgvEvent.childView.TabPages.Count > 0 && dgvEvent.childView.TabPages[0].Text == "Teams")
                        {
                            dgvEvent.childView.BindData(data.Tables[0], "Teams");

                            dgvEvent.childView.BindData(data.Tables[1], "HPlayers");
                            dgvEvent.childView.BindData(data.Tables[2], "GPlayers");
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
                    dgvEvent.Rows[e.RowIndex].HeaderCell.ContextMenuStrip = cmsBook;
                    cmsBook.Items[0].Text = "Book " + dgvEvent.Rows[e.RowIndex].Cells[0].Value;
                    cmsBook.Items[0].Tag = dgvEvent.Rows[e.RowIndex].Cells[0].Value;
                    this.dgvEvent.collapseRow = false;
                }
                /*
                                           using (FbConnection connection = new FbConnection("User=SYSDBA;Password=masterkey;Database=E:\\Project\\Database\\SCOUTS_DB.FDB;DataSource=127.0.0.1;Port=3050;Dialect=3;Charset=NONE;Role=;Connection lifetime=15;Pooling=true;MinPoolSize=0;MaxPoolSize=50;Packet Size=8192;ServerType=0"))
                                           {
                                               connection.Open();
                                               string queryString = "SELECT t.* FROM teams t  inner  join  events e on   t.id= e.HOME_ID or t.id=e.GUEST_ID   where e.id='" + team_id + "' order by id asc";
                                               using (FbCommand cmd = new FbCommand(queryString))
                                               {
                                                   using (FbCommandBuilder fcb = new FbCommandBuilder())
                                                   {
                                                       using (FbDataAdapter fda = new FbDataAdapter())
                                                       {
                                                           cmd.Connection = connection;
                                                           fda.SelectCommand = cmd;
                                                           //using (DataSet data = new DataSet())
                                                           //{
                                                           DataSet data = new DataSet();
                                                           data.Tables.Add(new DataTable("teams"));
                                                           fda.Fill(data.Tables["teams"]);
                                                           queryString = "select   * from  players2 where team_id=" + team_id;// 174516 ";
                                                           data.Tables.Add(new DataTable("players"));
                                                           FbDataAdapter adapter1 = new FbDataAdapter();
                                                           adapter1.SelectCommand = new FbCommand(queryString, connection);
                                                           // FbCommandBuilder builder3 = new FbCommandBuilder(adapter1);
                                                           adapter1.Fill(data.Tables["players"]);
                                                           if (data.Tables[0].Rows.Count == 0)
                                                           {
                                                               data = InsertData("participants", false, team_id);
                                                           }
                                                           if (dgvEvent.childView.TabPages.Count == 0)
                                                           {
                                                               dgvEvent.childView.AddData(data.Tables[0], "Teams");
                                                               dgvEvent.childView.AddData(data.Tables[1], "Players");
                                                           }
                                                           else if (dgvEvent.childView.TabPages.Count > 0 && dgvEvent.childView.TabPages[0].Text == "Teams")
                                                           {
                                                               dgvEvent.childView.BindData(data.Tables[0], "Teams");
                                                               dgvEvent.childView.BindData(data.Tables[1], "Players");
                                                           }
                                                           //}
                                                       }
                                                   }
                                               }
                                               connection.Close();
                                           }
                       */
            }
            catch (Exception exp)
            {
                Files.WriteError("MasterControl_RowHeaderMouseClic(),error:" + exp.Message);
            }
        }

        private void MasterControl_RowHeaderMouseClick2(object sender, DataGridViewCellMouseEventArgs e)
        {
            try
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

                        string event_id = this.dgvBookedEvent.Rows[e.RowIndex].Cells[0].Value.ToString();
                        string status_id = this.dgvBookedEvent.Rows[e.RowIndex].Cells[12].Value.ToString();
                        if (event_id == "") return;
                        DataSet data = new DataSet();
                        ///20190114 get incident where not run app
                        data = InsertData("events.show2", false, event_id == "" ? "-1" : event_id);
                        if (AppFlag.ManualGet || (event_id != "" && event_id != "0" && (data.Tables[0].Rows.Count < 2 || (status_id != "1" && data.Tables[1].Rows.Count == 0))))
                        {
                            data = InsertData("events.show2", true, event_id == "" ? "-1" : event_id);
                        }

                        if (dgvBookedEvent.childView.TabPages.Count == 0)
                        {
                            dgvBookedEvent.childView.AddData(data.Tables[0], "Teams");
                            dgvBookedEvent.childView.AddData(data.Tables[1], "HPlayers");
                            dgvBookedEvent.childView.AddData(data.Tables[2], "GPlayers");
                            dgvBookedEvent.childView.AddData(data.Tables[3], "Details");
                            dgvBookedEvent.childView.AddData(data.Tables[4], "Results");
                            dgvBookedEvent.childView.AddData(data.Tables[5], "Stats");
                            dgvBookedEvent.childView.AddData(data.Tables[6], "Incidents");
                            dgvBookedEvent.childView.AddData(data.Tables[7], "GoalInfo");
                            dgvBookedEvent.childView.AddData(data.Tables[8], "MatchDetails");
                        }
                        else if (dgvBookedEvent.childView.TabPages.Count > 0 && dgvBookedEvent.childView.TabPages[0].Text == "Teams")
                        {
                            dgvBookedEvent.childView.BindData(data.Tables[0], "Teams");
                            dgvBookedEvent.childView.BindData(data.Tables[1], "HPlayers");
                            dgvBookedEvent.childView.BindData(data.Tables[2], "GPlayers");
                            dgvBookedEvent.childView.BindData(data.Tables[3], "Details");
                            dgvBookedEvent.childView.BindData(data.Tables[4], "Results");
                            dgvBookedEvent.childView.BindData(data.Tables[5], "Stats");
                            dgvBookedEvent.childView.BindData(data.Tables[6], "Incidents");
                            dgvBookedEvent.childView.BindData(data.Tables[7], "GoalInfo");
                            dgvBookedEvent.childView.BindData(data.Tables[8], "MatchDetails");
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
                    dgvBookedEvent.Rows[e.RowIndex].HeaderCell.ContextMenuStrip = cmsBooked;

                    this.dgvBookedEvent.collapseRow = false;
                }
            }
            catch (Exception exp)
            {
                Files.WriteError("MasterControl_RowHeaderMouseClick2(),error:" + exp.Message);
            }
        }

        private void dgvBookedEvent_Leave(object sender, EventArgs e)
        {
            if (dgvBookedEvent.childView != null)
            {
                dgvBookedEvent.childView.Dispose();
            }
            //    dgvBookedEvent.MasterControlsDispose();
            // Rectangle rect = new Rectangle(Conversions.ToInteger(Microsoft.VisualBasic.CompilerServices.Operators.AddObject(e.RowBounds.X, Microsoft.VisualBasic.CompilerServices.Operators.DivideObject(Microsoft.VisualBasic.CompilerServices.Operators.SubtractObject(this.rowDefaultHeight, 0x10), 2))), Conversions.ToInteger(Microsoft.VisualBasic.CompilerServices.Operators.AddObject(e.RowBounds.Y, Microsoft.VisualBasic.CompilerServices.Operators.DivideObject(Microsoft.VisualBasic.CompilerServices.Operators.SubtractObject(this.rowDefaultHeight, 0x10), 2))), 0x10, 0x10);
            //if (dgvBookedEvent.collapseRow)
            //{
            //    if (dgvBookedEvent.rowCurrent.Contains(iExpand))
            //    {
            //    }
            //    else
            //    {
            //        dgvBookedEvent.childView.Visible = false;
            //     //   e.Graphics.DrawImage(dgvBookedEvent.RowHeaderIconList.Images[0], rect);
            //    }
            //    dgvBookedEvent.collapseRow = false;
            //}
        }

        private void dgvEvent_Leave(object sender, EventArgs e)
        {
            if (dgvEvent.childView != null)
            {
                dgvEvent.childView.Dispose();
            }
        }

        private void dgvEvent_RowContextMenuStripNeeded(object sender, DataGridViewRowContextMenuStripNeededEventArgs e)
        {
            //e.HeaderCell.ContextMenuStrip = cmsBook;

            // dgvEvent.Rows[0].HeaderCell.ContextMenuStrip = cmsBook;
        }

        private void cmsBooked_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

            string eventid = e.ClickedItem.Tag.ToString();
            string eventsName = e.ClickedItem.Text;
            int id = -1;
            if (MessageBox.Show("Are you sure " + (e.ClickedItem.Tag.ToString() != "" ? "" : " CANCEL ") + "sync " + dgvBookedEvent.SelectedRows[0].Cells[3].Value + "/" + dgvBookedEvent.SelectedRows[0].Cells[4].Value + (e.ClickedItem.Tag.ToString() == "" ? "" : " to " + eventsName) + "?", "Alert", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                using (FbConnection connection = new FbConnection(AppFlag.ScoutsDBConn))
                {
                    using (FbCommand cmd = new FbCommand())
                    {
                        connection.Open();
                        cmd.CommandText = "SYNC_MANUAL_HKJCDATA";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = connection;
                        cmd.Parameters.Add("@Event_id", e.ClickedItem.Tag.ToString() == "" ? null : e.ClickedItem.Tag.ToString());
                        cmd.Parameters.Add("@HKJCHOSTNAME", dgvBookedEvent.SelectedRows[0].Cells[3].Value);
                        cmd.Parameters.Add("@HKJCGUESTNAME", dgvBookedEvent.SelectedRows[0].Cells[4].Value);
                        id = Convert.ToInt32(cmd.ExecuteScalar());
                        Files.WriteLog((id > 0 ? "[Success] Manual sync " + e.ClickedItem.Text + " To " : id == 0 ? "Cancel sync " + dgvBookedEvent.SelectedRows[0].Cells[0].Value + " " : "[Failure] Manual sync  " + e.ClickedItem.Text + " To ") + dgvBookedEvent.SelectedRows[0].Cells[3].Value + "/" + dgvBookedEvent.SelectedRows[0].Cells[4].Value);
                    }
                    connection.Close();
                }
                if (id > 0)
                {
                    dgvBookedEvent.SelectedRows[0].Cells[0].Value = id;
                    cmsBooked.Items.Remove(e.ClickedItem);
                }
                else if (id == 0 && e.ClickedItem.Tag.ToString() == "")
                {
                    cmsBooked.Items.Add(new ToolStripMenuItem()
                    {
                        Text = dgvBookedEvent.SelectedRows[0].Cells[0].Value + " " + dgvBookedEvent.SelectedRows[0].Cells[3].Value + "/" + dgvBookedEvent.SelectedRows[0].Cells[4].Value,// + " " + dr["booked_by"].ToString(),
                        Tag = dgvBookedEvent.SelectedRows[0].Cells[0].Value
                    });

                    dgvBookedEvent.SelectedRows[0].Cells[0].Value = DBNull.Value;
                }
            }
        }

        static string[] Day = new string[] { "SUN", "MON", "TUE", "WED", "THU", "FRI", "SAT" };
        static string week = "";
        private bool BookEventAction(string eventid, string EName, string start_date, string hkjcHname, string hkjcGname, string hkjcDaycode, string hkjcMatchNo, bool show)
        {
            try
            {
                ///20190213 only book currently day match
                /// if (Convert.ToDateTime(start_date).ToString("yyyy-MM-dd") != DateTime.Now.ToString("yyyy-MM-dd")) return false;
                if (hkjcDaycode != week) return false;

                bool b_booked = false;
                string strName = "Booked" + eventid + "-" + DateTime.Now.ToString("HHmmss");
                var responseValue = clientTest.PostAccessData(strToken, "booked-events", eventid);
                var strResponseValue = responseValue.Result;

                Files.WriteLog(strName + ".xml");
                Files.WriteXml(strName, strResponseValue);

                DOSBookedResults.api apis = XmlUtil.Deserialize(typeof(DOSBookedResults.api), strResponseValue) as DOSBookedResults.api;
                DOSBookedResults.apiError errors = apis.error;
                if (errors != null)
                {
                    if (show) MessageBox.Show(errors.message);
                    Files.WriteLog("[Failure] " + "Booked " + eventid + ",Error: " + errors.message);

                    if (errors.message == "Booking already Exist")
                    {
                        b_booked = true;
                        using (FbConnection connection = new FbConnection(AppFlag.ScoutsDBConn))
                        {
                            using (FbCommand cmd = new FbCommand())
                            {
                                connection.Open();
                                cmd.CommandText = "Set_BookedEventBy";
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Connection = connection;
                                cmd.Parameters.Add("@ID", eventid);
                                cmd.Parameters.Add("@BOOKED", true);
                                cmd.Parameters.Add("@BOOKED_BY", null);
                                cmd.Parameters.Add("@CTIMESTAMP", DateTime.Now);
                                int id = Convert.ToInt32(cmd.ExecuteScalar());
                                /// Files.WriteLog(id > 0 ? "[Success] Update Booked " + eventid : "[Failure] Update Booked " + eventid);
                                //if (id > 0)
                                //{ 
                                //    if (show) MessageBox.Show(eventid + " Booking already Exist." );
                                //}
                            }
                            connection.Close();
                        }
                        if (show) MessageBox.Show(eventid + " Booking already Exist.");
                    }
                    else if (errors.message == "You cannot book this event, it is not covered in this product" || errors.message == "Your membership package does not include requested method or access to this method is expired" || errors.status == "403" || errors.message == "You can order/refuse only events with the statuses: scheduled")
                    {
                        //id.ToString(), dr1["NAME"].ToString(), dr1["START_DATE"], dr1["HKJCHOSTNAME"], dr1["HKJCGUESTNAME"].ToString(), dr1["HKJCDAYCODE"].ToString(), dr1["HKJCMATCHNO"].ToString(), dr1["MAPPINGSTATUS"].ToString(), false
                        //  Files.WriteUnBookLog("["+eventid+"]"+EName+" "+ start_date +"  "+hkjcHname+"/"+hkjcGname +" - [" +hkjcDaycode+"]" +hkjcMatchNo+" "+ status);
                        Files.WriteUnBookLog("[" + eventid + "]" + (EName == "" ? "" : EName + "      ") + hkjcHname + "/" + hkjcGname + " - [" + hkjcDaycode + "]" + hkjcMatchNo + "      " + start_date);
                        using (FbConnection connection = new FbConnection(AppFlag.ScoutsDBConn))
                        {
                            using (FbCommand cmd = new FbCommand())
                            {
                                connection.Open();
                                cmd.CommandText = "Set_BookedEventBy";
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Connection = connection;
                                cmd.Parameters.Add("@ID", eventid);
                                cmd.Parameters.Add("@BOOKED", false);
                                cmd.Parameters.Add("@BOOKED_BY", "(UnBooked)");
                                cmd.Parameters.Add("@CTIMESTAMP", DateTime.Now);
                                int id = Convert.ToInt32(cmd.ExecuteScalar());
                                Files.WriteLog(id > 0 ? "[Success] Update Booked " + eventid : "[Failure] Update Booked " + eventid);
                            }
                            connection.Close();
                        }
                    }
                }
                else
                {
                    DOSBookedResults.apiDataBooked_eventsEvent[] bookedEvents = (apis.data.Length == 0) ? null : apis.data[0];
                    if (bookedEvents == null) return false;
                    foreach (DOSBookedResults.apiDataBooked_eventsEvent sevent in bookedEvents)
                    {
                        using (FbConnection connection = new FbConnection(AppFlag.ScoutsDBConn))
                        {
                            using (FbCommand cmd = new FbCommand())
                            {
                                connection.Open();
                                cmd.CommandText = "Set_BookedEventBy";
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Connection = connection;
                                cmd.Parameters.Add("@ID", sevent.id);
                                cmd.Parameters.Add("@BOOKED", true);
                                cmd.Parameters.Add("@BOOKED_BY", null);
                                cmd.Parameters.Add("@CTIMESTAMP", DateTime.Now);
                                int id = Convert.ToInt32(cmd.ExecuteScalar());
                                Files.WriteLog(id > 0 ? "[Success] Booked " + eventid : "[Failure] Booked " + eventid);
                                if (id > 0)
                                {
                                    b_booked = true;
                                    if (show) MessageBox.Show("[Success] Booked " + sevent.id + ".");
                                }
                            }
                            connection.Close();
                        }
                    }
                }
                return b_booked;
            }
            catch (Exception exp)
            {
                Files.WriteError("BookEventAction(),error:" + exp.Message);
                return false;
            }
        }
        private bool AutoGetPlayer(string eventid, string EName, string start_date, string hkjcHname, string hkjcGname, string hkjcDaycode, string hkjcMatchNo, bool show)
        {
            try
            {
                // Files.WriteLog("AutoGetPlayer2");
                DateTime time = Convert.ToDateTime(start_date).AddHours(-AppFlag.IntervalGetPlayer);
                if (time.AddMinutes(-AppFlag.IntervalSync / 2.0) < DateTime.Now && DateTime.Now <= time.AddMinutes(AppFlag.IntervalSync / 2.0))
                {
                    InsertData("events.show3", true, eventid, true);
                    Files.WriteLog(" Get event player [" + eventid + "]");
                    /// Files.WriteLog("AutoGetPlayer(),DO " + time.AddMinutes(-AppFlag.IntervalSync / 2.0) + " < " + DateTime.Now + " < " + time.AddMinutes(AppFlag.IntervalSync / 2.0));
                }
                else
                {
                    ///Files.WriteLog("AutoGetPlayer(),NO " + time.AddMinutes(-AppFlag.IntervalSync / 2.0) + "---" + DateTime.Now + "---" + time.AddMinutes(AppFlag.IntervalSync / 2.0));
                }
                return true;
            }
            catch (Exception exp)
            {
                Files.WriteError("AutoGetPlayer(),error:" + exp.Message);
                return false;
            }
        }

        private void cmsBook_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            try
            {
                if (e.ClickedItem.Text.IndexOf("Book") == 0)
                {
                    string strName = "Book" + e.ClickedItem.Tag.ToString() + "-" + DateTime.Now.ToString("HHmmss");
                    Files.WriteLog(e.ClickedItem.Text + ",File: " + strName + ".xml");

                    var responseValue = clientTest.PostAccessData(strToken, "booked-events", e.ClickedItem.Tag.ToString());
                    var strResponseValue = responseValue.Result;

                    //XDocument document = XDocument.Load("E:\\Project\\AppProject\\DataOfScouts\\DataOfScouts\\bin\\Debug\\New folder\\Book2556471132507.xml");
                    //var strResponseValue = document.ToString();

                    Files.WriteXml(strName, strResponseValue);

                    DOSBookedResults.api apis = XmlUtil.Deserialize(typeof(DOSBookedResults.api), strResponseValue) as DOSBookedResults.api;
                    DOSBookedResults.apiError errors = apis.error;
                    if (errors != null)
                    {
                        MessageBox.Show(errors.message);
                        Files.WriteLog("[Failure] " + e.ClickedItem.Text + ",Error: " + errors.message);

                        if (errors.message == "Booking already Exist")
                        {
                            using (FbConnection connection = new FbConnection(AppFlag.ScoutsDBConn))
                            {
                                using (FbCommand cmd = new FbCommand())
                                {
                                    connection.Open();
                                    cmd.CommandText = "Set_BookedEventBy";
                                    cmd.CommandType = CommandType.StoredProcedure;
                                    cmd.Connection = connection;
                                    cmd.Parameters.Add("@ID", e.ClickedItem.Tag.ToString());
                                    cmd.Parameters.Add("@BOOKED", true);
                                    cmd.Parameters.Add("@BOOKED_BY", null);
                                    cmd.Parameters.Add("@CTIMESTAMP", DateTime.Now);
                                    int id = Convert.ToInt32(cmd.ExecuteScalar());
                                    //Files.WriteLog(id > 0 ? "[Success] Update " + e.ClickedItem.Text : "[Failure] Update " + e.ClickedItem.Text);
                                    //if (id > 0)
                                    //{
                                    //    MessageBox.Show("[Success] Booked " + e.ClickedItem.Tag.ToString() + ".");
                                    //}
                                }
                                connection.Close();
                            }
                        }
                        else if (errors.message == "You cannot book this event, it is not covered in this product" || errors.message == "Your membership package does not include requested method or access to this method is expired" || errors.status == "403" || errors.message == "You can order/refuse only events with the statuses: scheduled")
                        {
                            using (FbConnection connection = new FbConnection(AppFlag.ScoutsDBConn))
                            {
                                using (FbCommand cmd = new FbCommand())
                                {
                                    connection.Open();
                                    cmd.CommandText = "Set_BookedEventBy";
                                    cmd.CommandType = CommandType.StoredProcedure;
                                    cmd.Connection = connection;
                                    cmd.Parameters.Add("@ID", e.ClickedItem.Tag.ToString());
                                    cmd.Parameters.Add("@BOOKED", false);
                                    cmd.Parameters.Add("@BOOKED_BY", "(UnBooked)");
                                    cmd.Parameters.Add("@CTIMESTAMP", DateTime.Now);
                                    int id = Convert.ToInt32(cmd.ExecuteScalar());
                                    Files.WriteLog(id > 0 ? "[Success] Update " + e.ClickedItem.Text : "[Failure] Update " + e.ClickedItem.Text);
                                }
                                connection.Close();
                            }
                        }
                    }

                    else
                    {
                        DOSBookedResults.api apiEvents = XmlUtil.Deserialize(typeof(DOSBookedResults.api), strResponseValue) as DOSBookedResults.api;
                        if (apiEvents == null) return;
                        DOSBookedResults.apiDataBooked_eventsEvent[] bookedEvents = (apiEvents.data.Length == 0) ? null : apiEvents.data[0];
                        if (bookedEvents == null) return;

                        foreach (DOSBookedResults.apiDataBooked_eventsEvent sevent in bookedEvents)
                        {
                            using (FbConnection connection = new FbConnection(AppFlag.ScoutsDBConn))
                            {
                                using (FbCommand cmd = new FbCommand())
                                {
                                    connection.Open();
                                    cmd.CommandText = "Set_BookedEventBy";
                                    cmd.CommandType = CommandType.StoredProcedure;
                                    cmd.Connection = connection;
                                    cmd.Parameters.Add("@ID", sevent.id);
                                    cmd.Parameters.Add("@BOOKED", true);
                                    cmd.Parameters.Add("@BOOKED_BY", null);
                                    cmd.Parameters.Add("@CTIMESTAMP", DateTime.Now);
                                    int id = Convert.ToInt32(cmd.ExecuteScalar());
                                    Files.WriteLog(id > 0 ? "[Success] Booked " + e.ClickedItem.Text : "[Failure] Booked " + e.ClickedItem.Text);
                                    if (id > 0)
                                    {
                                        MessageBox.Show("[Success] Booked " + sevent.id + ".");
                                    }
                                }
                                connection.Close();
                            }
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                Files.WriteError("cmsBook_ItemClicked(),error:" + exp.Message);
            }
        }



        private void DataOfScouts_Load(object sender, EventArgs e)
        {
            ///  InitialListener(); 
            Files.WriteLog(" Start AMQPService.");
            this.lstStatus.Items.Insert(0, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff   ") + "Start AMQPService.");
            bgwAMQPService.RunWorkerAsync();
        }

        private void DataOfScouts_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (bBreak == false)
            {
                bBreak = true;
                ///   Files.WriteLog(" Stop AMQPService.");
            }
            //else
            //{
            //    bBreak = false;
            //    Files.WriteLog(" Run AMQPService.");
            //    bgwAMQPService.RunWorkerAsync();
            //}
            Thread.Sleep(TimeSpan.FromMilliseconds(10));
            if (bgwAMQPService.IsBusy && bgwAMQPService.WorkerSupportsCancellation == true)
            {
                bBreak = true;
                Files.WriteLog(" Stop AMQPService.");
                bgwAMQPService.CancelAsync();
            }

            bgwAMQPService.Dispose();
        }

        BackgroundWorker bgwAMQPService = new BackgroundWorker();
        static bool bBreak = false;
        BackgroundWorker backgroundWorker = new BackgroundWorker();


        //private void bgwAMQPService_DoWorkBAK(object sender, DoWorkEventArgs e)
        //// private async void bgwAMQPService_DoWork(object sender, DoWorkEventArgs e)
        //{
        //    try
        //    {
        //        bgwAMQPService.WorkerSupportsCancellation = true;
        //        backgroundWorker.DoWork += new DoWorkEventHandler(backgroundWorker_DoWork);
        //        string serverAddress = "amqp://queue.statscore.com:5672/";
        //        Uri uri = new Uri(serverAddress);
        //        ConnectionFactory cf = new ConnectionFactory();
        //        cf.Uri = uri;
        //        cf.HostName = "queue.statscore.com";
        //        cf.Port = 5672;
        //        cf.UserName = "telecom-digital-data-limited";
        //        cf.Password = "Eb76sQDkn9oZEki5L9QreKpaPmD3GbuuW2I";
        //        cf.VirtualHost = "statscore";
        //        cf.RequestedHeartbeat = 60;
        //        using (IConnection conn = cf.CreateConnection())
        //        {
        //            using (IModel channel = conn.CreateModel())
        //            {
        //                //在MQ上定义一个持久化队列，如果名称相同不会重复创建
        //                //  channel.QueueDeclare("telecom-digital-data-limited", true, false, false, null);

        //                ////输入1，那如果接收一个消息，但是没有应答，则客户端不会收到下一个消息
        //                ////channel.BasicQos(0, 1, false);
        //                bool ack = false;
        //                using (FbConnection connection = new FbConnection(AppFlag.ScoutsDBConn))
        //                {
        //                    connection.Open();
        //                    var consumer = new QueueingBasicConsumer(channel);
        //                    //// channel.BasicConsume("telecom-digital-data-limited", true, consumer); disable autoack 20190111
        //                    var consumerTag = channel.BasicConsume("telecom-digital-data-limited", false, consumer);
        //                    ////if (AppFlag.TestMode) Files.WriteTestLog("Queue", " Listening--------"+ consumerTag);
        //                    ////QueueDeclareOk result = channel.QueueDeclarePassive("telecom-digital-data-limited");
        //                    ////uint countA = result != null ? result.MessageCount : 0;
        //                    ////if (AppFlag.TestMode) Files.WriteTestLog("Queue", iCount3.ToString() + "  [" + channel.MessageCount("telecom-digital-data-limited") + "/" + countA + "]  ID -a ");

        //                    while (true)
        //                    {
        //                        string strName = "";
        //                        try
        //                        {
        //                            //  result = channel.QueueDeclarePassive("telecom-digital-data-limited");
        //                            //  countA = result != null ? result.MessageCount : 0;
        //                            //if (AppFlag.TestMode) Files.WriteTestLog("Queue", iCount3.ToString() + "  [" + channel.MessageCount("telecom-digital-data-limited") + "/" + countA + "]  ID -b ");


        //                            //// if (!consumer.Queue.Any()) { Files.WriteLog("no message"); continue; }////20190111 ////1142019
        //                            if (!consumer.Queue.Any())
        //                            {
        //                                Thread.Sleep(100); continue;
        //                            }
        //                            else
        //                            {
        //                                QueueDeclareOk result = channel.QueueDeclarePassive("telecom-digital-data-limited");
        //                                uint countA = result != null ? result.MessageCount : 0;
        //                                if (AppFlag.TestMode) Files.WriteTestLog("Queue", iCount3.ToString() + "  [" + channel.MessageCount("telecom-digital-data-limited") + "/" + countA + "]");

        //                            }
        //                            ////if (AppFlag.TestMode) Files.WriteTestLog("Queue", " Listening...1");
        //                            var ea = (BasicDeliverEventArgs)consumer.Queue.Dequeue();
        //                            ////  result = channel.QueueDeclarePassive("telecom-digital-data-limited");
        //                            ////countA = result != null ? result.MessageCount : 0;
        //                            ////if (AppFlag.TestMode) Files.WriteTestLog("Queue", iCount3.ToString() + "  [" + channel.MessageCount("telecom-digital-data-limited") +"//"+countA+ "]  ID -c ");
        //                            ////  if (AppFlag.TestMode) Files.WriteTestLog("Queue", " Listening...2");
        //                            iCount3++;
        //                            if (ea == null)
        //                            {
        //                                Files.WriteError("ea == null");
        //                                break;
        //                            } ////20190111
        //                            var body = ea.Body;
        //                            var message = Encoding.UTF8.GetString(body);
        //                            if (AppFlag.TestMode) Files.WriteTestLog("msg", message);
        //                            ///  manaul  ack 20190111
        //                            channel.BasicAck(ea.DeliveryTag, false);
        //                            if (!ack) { channel.BasicQos(0, 1, false); }
        //                            ////result = channel.QueueDeclarePassive("telecom-digital-data-limited");
        //                            ////countA = result != null ? result.MessageCount : 0;
        //                            ////if (AppFlag.TestMode) Files.WriteTestLog("Queue", iCount3.ToString() + "  [" + channel.MessageCount("telecom-digital-data-limited") + "///" + countA + "]  ID -d ");

        //                            //continue;
        //                            DOSEventJson.EventJson api = JsonUtil.Deserialize(typeof(DOSEventJson.EventJson), message) as DOSEventJson.EventJson;

        //                            ////if (api != null)
        //                            ////{
        //                            ////    if (AppFlag.TestMode) Files.WriteTestLog("Queue", iCount3.ToString() + " api " + api.id);
        //                            ////}
        //                            ////else
        //                            ////{
        //                            ////    if (AppFlag.TestMode) Files.WriteTestLog("Queue", iCount3.ToString() + " api -2");
        //                            ////}
        //                            ////if (AppFlag.TestMode) Files.WriteTestLog("Queue", "+++++++++++++++++++++++");
        //                            if (api != null && api.type == "event" && (AppFlag.JsonType ? api.data.@event.sport_id == 5 : api.data.@event.sport_id != -1))
        //                            {
        //                                iCount++;
        //                                ////   if (AppFlag.TestMode) Files.WriteTestLog("Queue", iCount.ToString() + " Event " + api.id);

        //                                int id = -2;
        //                                strName = api.data.@event.id + "_" + (api.data.@event.status_id) + "_" + DateTime.Now.ToString("HHmmssfff");
        //                                Files.WriteJson(strName, message);
        //                                using (FbCommand cmd2 = new FbCommand())
        //                                {
        //                                    cmd2.CommandText = "PR_JSON_event";
        //                                    cmd2.CommandType = CommandType.StoredProcedure;
        //                                    cmd2.Connection = connection;
        //                                    cmd2.Parameters.Add("@ID", api.data.@event.id);
        //                                    cmd2.Parameters.Add("@NAME", api.data.@event.name);
        //                                    cmd2.Parameters.Add("@HOME_ID", api.data.@event.participants.Count() > 0 ? api.data.@event.participants[0].id : -1);
        //                                    cmd2.Parameters.Add("@GUEST_ID", api.data.@event.participants.Count() > 0 ? api.data.@event.participants[1].id : -1);
        //                                    // cmd2.Parameters.Add("@SOURCE", api.data.@evenT.S);
        //                                    // cmd2.Parameters.Add("@SOURCE_DC", api.data.@event.S);
        //                                    //cmd2.Parameters.Add("@SOURCE_SUPER", api.data.@event.s);
        //                                    cmd2.Parameters.Add("@RELATION_STATUS", api.data.@event.relation_status);
        //                                    cmd2.Parameters.Add("@START_DATE", Convert.ToDateTime(api.data.@event.start_date).AddHours(8));
        //                                    cmd2.Parameters.Add("@FT_ONLY", api.data.@event.ft_only == "yes" ? true : false);
        //                                    cmd2.Parameters.Add("@COVERAGE_TYPE", api.data.@event.coverage_type);
        //                                    //cmd2.Parameters.Add("@CHANNEL_ID", api.data.@event.CH);
        //                                    //cmd2.Parameters.Add("@CHANNEL_NAME", api.data.@event.C);
        //                                    cmd2.Parameters.Add("@SCOUTSFEED", api.data.@event.scoutsfeed == "yes" ? true : false);
        //                                    cmd2.Parameters.Add("@STATUS_ID", api.data.@event.status_id);
        //                                    //cmd2.Parameters.Add("@STATUS_NAME", api.data.@event.STA);
        //                                    cmd2.Parameters.Add("@STATUS_TYPE", api.data.@event.status_type);
        //                                    cmd2.Parameters.Add("@CDAY", api.data.@event.day);
        //                                    cmd2.Parameters.Add("@CLOCK_TIME", api.data.@event.clock_time);
        //                                    cmd2.Parameters.Add("@CLOCK_STATUS", api.data.@event.clock_status);
        //                                    // cmd2.Parameters.Add("@WINNER_ID", api.data.@event.W);
        //                                    //cmd2.Parameters.Add("@PROGRESS_ID", api.data.@event.PR);
        //                                    cmd2.Parameters.Add("@BET_STATUS", api.data.@event.bet_status);
        //                                    cmd2.Parameters.Add("@NEUTRAL_VENUE", api.data.@event.neutral_venue == "yes" ? true : false);
        //                                    cmd2.Parameters.Add("@ITEM_STATUS", api.data.@event.item_status);
        //                                    // cmd2.Parameters.Add("@UT", api.data.@event.U);
        //                                    // cmd2.Parameters.Add("@OLD_EVENT_ID", api.data.@event.OL);
        //                                    // cmd2.Parameters.Add("@SLUG", api.data.@event.S);
        //                                    // cmd2.Parameters.Add("@VERIFIED_RESULT", api.data.@event.VE);
        //                                    // cmd2.Parameters.Add("@IS_PROTOCOL_VERIFIED", api.data.@event.IS);
        //                                    //  cmd2.Parameters.Add("@PROTOCOL_VERIFIED_BY", api.data.@event.PRO);
        //                                    //cmd2.Parameters.Add("@PROTOCOL_VERIFIED_AT", api.data.@event.PRO);
        //                                    cmd2.Parameters.Add("@ROUND_ID", api.data.@event.round_id);
        //                                    cmd2.Parameters.Add("@ROUND_NAME", api.data.@event.round_name);
        //                                    //cmd2.Parameters.Add("@CLIENT_EVENT_ID", api.data.@event.C);
        //                                    // cmd2.Parameters.Add("@BOOKED", null);
        //                                    // cmd2.Parameters.Add("@BOOKED_BY", api.data.@event.);
        //                                    // cmd2.Parameters.Add("@INVERTED_PARTICIPANTS", api.data.@event.iv);
        //                                    cmd2.Parameters.Add("@VENUE_ID", api.data.@event.tour_id == null ? "-1" : api.data.@event.tour_id);
        //                                    //  cmd2.Parameters.Add("@GROUP_ID", api.data.@event.gr);
        //                                    cmd2.Parameters.Add("@STAGE_ID", api.data.@event.stage_id);
        //                                    cmd2.Parameters.Add("@SEASON_ID", api.data.@event.season_id);
        //                                    cmd2.Parameters.Add("@COMPETITION_ID", api.data.@event.competition_id);
        //                                    cmd2.Parameters.Add("@AREA_ID", api.data.@event.area_id);
        //                                    cmd2.Parameters.Add("@CTIMESTAMP", DateTime.Now);
        //                                    cmd2.Parameters.Add("@CACTION", api.data.@event.action);
        //                                    id = Convert.ToInt32(cmd2.ExecuteScalar());
        //                                    Files.WriteLog((id == 0 ? " [Success] Insert event " : id == 1 ? " Update event " : "") + "[" + api.data.@event.id + "]," + strName + ".json");
        //                                    // Files.WriteLog((id == 0 ? " [Success] Insert event [" + api.data.@event.id + "]," + strName + ".json":"");
        //                                }


        //                                if (api.data.@event.details.Count() > 0)
        //                                {
        //                                    using (FbCommand cmd2 = new FbCommand())
        //                                    {
        //                                        cmd2.CommandText = "PR_event_details";
        //                                        cmd2.CommandType = CommandType.StoredProcedure;
        //                                        cmd2.Connection = connection;
        //                                        cmd2.Parameters.Add("@EVENTID", api.data.@event.id);
        //                                        cmd2.Parameters.Add("@WC_8", api.data.@event.details.FirstOrDefault(c => c.id == 8).value);
        //                                        cmd2.Parameters.Add("@PC_36", api.data.@event.details.FirstOrDefault(c => c.id == 36).value);
        //                                        cmd2.Parameters.Add("@PL_16", api.data.@event.details.FirstOrDefault(c => c.id == 16).value);
        //                                        cmd2.Parameters.Add("@EPL_50", api.data.@event.details.FirstOrDefault(c => c.id == 50).value);
        //                                        cmd2.Parameters.Add("@NOP_17", api.data.@event.details.FirstOrDefault(c => c.id == 17).value);
        //                                        cmd2.Parameters.Add("@EPTC_58", api.data.@event.details.FirstOrDefault(c => c.id == 58).value);
        //                                        cmd2.Parameters.Add("@IT_151", api.data.@event.details.FirstOrDefault(c => c.id == 151).value);
        //                                        cmd2.Parameters.Add("@ATT_141", api.data.@event.details.FirstOrDefault(c => c.id == 141).value);
        //                                        cmd2.Parameters.Add("@FHSD_19", api.data.@event.details.FirstOrDefault(c => c.id == 19).value == null ? DateTime.MinValue : Convert.ToDateTime(api.data.@event.details.FirstOrDefault(c => c.id == 19).value).AddHours(8));
        //                                        cmd2.Parameters.Add("@SHSD_20", api.data.@event.details.FirstOrDefault(c => c.id == 20).value == null ? DateTime.MinValue : Convert.ToDateTime(api.data.@event.details.FirstOrDefault(c => c.id == 20).value).AddHours(8));
        //                                        cmd2.Parameters.Add("@FEHSD_44", api.data.@event.details.FirstOrDefault(c => c.id == 44).value);
        //                                        cmd2.Parameters.Add("@SEHSD_45", api.data.@event.details.FirstOrDefault(c => c.id == 45).value);
        //                                        cmd2.Parameters.Add("@PSSD_150", api.data.@event.details.FirstOrDefault(c => c.id == 150).value);
        //                                        cmd2.Parameters.Add("@FHIT_201", api.data.@event.details.FirstOrDefault(c => c.id == 201).value);
        //                                        cmd2.Parameters.Add("@SHIT_202", api.data.@event.details.FirstOrDefault(c => c.id == 202).value);
        //                                        cmd2.Parameters.Add("@FEHIT_203", api.data.@event.details.FirstOrDefault(c => c.id == 203).value);
        //                                        cmd2.Parameters.Add("@SEHIT_204", api.data.@event.details.FirstOrDefault(c => c.id == 204).value);
        //                                        cmd2.Parameters.Add("@HL_205", api.data.@event.details.FirstOrDefault(c => c.id == 205).value);
        //                                        cmd2.Parameters.Add("@TD_124", api.data.@event.details.FirstOrDefault(c => c.id == 124).value);
        //                                        cmd2.Parameters.Add("@BM_160", api.data.@event.details.FirstOrDefault(c => c.id == 160).value);
        //                                        cmd2.Parameters.Add("@HF_178", api.data.@event.details.FirstOrDefault(c => c.id == 178).value);
        //                                        cmd2.Parameters.Add("@UT", api.ut);
        //                                        cmd2.Parameters.Add("@CTIMESTAMP", DateTime.Now);
        //                                        id = Convert.ToInt32(cmd2.ExecuteScalar());
        //                                        Files.WriteLog((id > 0 ? " [Success] Insert event_details " : id == -1 ? " Old data " : " [Failure] Insert event_details ") + "[" + api.data.@event.id + "]," + strName + ".json");
        //                                    }

        //                                    if (id != -1)
        //                                    {
        //                                        for (int i = 0; i < api.data.@event.participants.Length; i++)
        //                                        {
        //                                            using (FbCommand cmd2 = new FbCommand())
        //                                            {
        //                                                cmd2.CommandText = "PR_participant_results";
        //                                                cmd2.CommandType = CommandType.StoredProcedure;
        //                                                cmd2.Connection = connection;
        //                                                // cmd2.Parameters.Add("@ID", 0);
        //                                                cmd2.Parameters.Add("@EVENTID", api.data.@event.id);
        //                                                cmd2.Parameters.Add("@PARTICIPANTID", api.data.@event.participants[i].id);
        //                                                cmd2.Parameters.Add("@PROGRESS_412", api.data.@event.participants[i].results.FirstOrDefault(c => c.id == 412).value);
        //                                                cmd2.Parameters.Add("@WINNER_411", api.data.@event.participants[i].results.FirstOrDefault(c => c.id == 411).value);
        //                                                cmd2.Parameters.Add("@RESULT_2", api.data.@event.participants[i].results.FirstOrDefault(c => c.id == 2).value);
        //                                                cmd2.Parameters.Add("@RT_3", api.data.@event.participants[i].results.FirstOrDefault(c => c.id == 3).value);
        //                                                cmd2.Parameters.Add("@FH_4", api.data.@event.participants[i].results.FirstOrDefault(c => c.id == 4).value);
        //                                                cmd2.Parameters.Add("@SH_5", api.data.@event.participants[i].results.FirstOrDefault(c => c.id == 5).value);
        //                                                cmd2.Parameters.Add("@E1H_133", api.data.@event.participants[i].results.FirstOrDefault(c => c.id == 133).value);
        //                                                cmd2.Parameters.Add("@E2H_134", api.data.@event.participants[i].results.FirstOrDefault(c => c.id == 134).value);
        //                                                cmd2.Parameters.Add("@PENALTY_7", api.data.@event.participants[i].results.FirstOrDefault(c => c.id == 7).value);
        //                                                cmd2.Parameters.Add("@OVERTIME_104", api.data.@event.participants[i].results.FirstOrDefault(c => c.id == 104).value);
        //                                                cmd2.Parameters.Add("@CTIMESTAMP", DateTime.Now);
        //                                                cmd2.Parameters.Add("@CACTION", api.data.@event.action);
        //                                                cmd2.Parameters.Add("@TEAMTYPE", api.data.@event.participants[i].counter == 1 ? "H" : "G");
        //                                                id = Convert.ToInt32(cmd2.ExecuteScalar());
        //                                                Files.WriteLog((id > 0 ? " [Success] Insert participant_results" : " [Failure] Insert participant_results") + "[" + api.data.@event.id + "/" + api.data.@event.participants[0].id + "]");
        //                                            }

        //                                            using (FbCommand cmd2 = new FbCommand())
        //                                            {
        //                                                cmd2.CommandText = "PR_participant_stats";
        //                                                cmd2.CommandType = CommandType.StoredProcedure;
        //                                                cmd2.Connection = connection;
        //                                                // cmd2.Parameters.Add("@ID", 0);
        //                                                cmd2.Parameters.Add("@EVENTID", api.data.@event.id);
        //                                                cmd2.Parameters.Add("@PARTICIPANTID", api.data.@event.participants[i].id);
        //                                                cmd2.Parameters.Add("@SOT_20", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 20).value);
        //                                                cmd2.Parameters.Add("@SOT_21", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 21).value);
        //                                                cmd2.Parameters.Add("@ATTACKS_10", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 10).value);
        //                                                cmd2.Parameters.Add("@DA_11", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 11).value);
        //                                                cmd2.Parameters.Add("@CORNERS_13", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 13).value);
        //                                                cmd2.Parameters.Add("@YELLOW_CARDS_8", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 8).value);
        //                                                cmd2.Parameters.Add("@RED_CARDS_9", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 9).value);
        //                                                cmd2.Parameters.Add("@TOTAL_SHOTS_19", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 19).value);
        //                                                cmd2.Parameters.Add("@FOULS_22", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 22).value);
        //                                                cmd2.Parameters.Add("@OFFSIDES_24", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 24).value);
        //                                                cmd2.Parameters.Add("@PS_14", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 14).value);
        //                                                cmd2.Parameters.Add("@PM_15", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 15).value);
        //                                                cmd2.Parameters.Add("@PG_16", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 16).value);
        //                                                cmd2.Parameters.Add("@FK_25", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 25).value);
        //                                                cmd2.Parameters.Add("@DFK_26", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 26).value);
        //                                                cmd2.Parameters.Add("@FKG_18", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 18).value);
        //                                                cmd2.Parameters.Add("@SW_27", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 27).value);
        //                                                cmd2.Parameters.Add("@SB_28", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 28).value);
        //                                                cmd2.Parameters.Add("@GS_29", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 29).value);
        //                                                cmd2.Parameters.Add("@GK_30", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 30).value);
        //                                                cmd2.Parameters.Add("@TI_32", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 32).value);
        //                                                cmd2.Parameters.Add("@SUBSTITUTIONS_31", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 31).value);
        //                                                cmd2.Parameters.Add("@GOALS_40", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 40).value);
        //                                                cmd2.Parameters.Add("@MP_34", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 34).value);
        //                                                cmd2.Parameters.Add("@OWN_GOALS_17", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 17).value);
        //                                                cmd2.Parameters.Add("@ADW_33", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 33).value);
        //                                                cmd2.Parameters.Add("@FORM_716", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 716).value);
        //                                                cmd2.Parameters.Add("@SKIN_718", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 718).value);
        //                                                cmd2.Parameters.Add("@PS_639", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 639).value);
        //                                                cmd2.Parameters.Add("@PU_697", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 697).value);
        //                                                cmd2.Parameters.Add("@GOALS115_772", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 772).value);
        //                                                cmd2.Parameters.Add("@GOALS1630_773", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 773).value);
        //                                                cmd2.Parameters.Add("@GOALS3145_774", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 774).value);
        //                                                cmd2.Parameters.Add("@GOALS4660_775", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 775).value);
        //                                                cmd2.Parameters.Add("@GOALS6175_776", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 776).value);
        //                                                cmd2.Parameters.Add("@GOALS7690_777", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 777).value);
        //                                                cmd2.Parameters.Add("@MPG_778", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 778).value);
        //                                                cmd2.Parameters.Add("@MPS_779", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 779).value);
        //                                                cmd2.Parameters.Add("@CTIMESTAMP", DateTime.Now);
        //                                                cmd2.Parameters.Add("@CACTION", api.data.@event.action);
        //                                                cmd2.Parameters.Add("@TEAMTYPE", api.data.@event.participants[i].counter == 2 ? "G" : "H");
        //                                                id = Convert.ToInt32(cmd2.ExecuteScalar());
        //                                                Files.WriteLog((id > 0 ? " [Success] Insert participant_stats" : " [Failure] Insert participant_results") + "[" + api.data.@event.id + "/" + api.data.@event.participants[0].id + "]");
        //                                                //  Thread.Sleep(20);
        //                                            }

        //                                            //using (FbCommand cmd2 = new FbCommand())
        //                                            //{

        //                                            //    cmd2.CommandText = "PR_stats_GoalInfo";
        //                                            //    cmd2.CommandType = CommandType.StoredProcedure;
        //                                            //    cmd2.Connection = connection; 
        //                                            //    cmd2.Parameters.Add("@EMATCHID", api.data.@event.id); 
        //                                            //    cmd2.Parameters.Add("@YELLOW_CARDS_8", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 8).value);
        //                                            //    cmd2.Parameters.Add("@RED_CARDS_9", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 9).value);
        //                                            //    cmd2.Parameters.Add("@CTIMESTAMP", DateTime.Now);
        //                                            //    id = Convert.ToInt32(cmd2.ExecuteScalar());
        //                                            //    Files.WriteLog((id > 0 ? " [Success] Update GoalInfo " : " [Failure]  Update GoalInfo ") + "[" + api.data.@event.id + "/" + api.data.@event.participants[0].id + "]");
        //                                            //    Thread.Sleep(20);
        //                                            //}
        //                                        }
        //                                        if (api.data.@event.participants.Length == 2 && api.data.@event.action != "delete")
        //                                        {

        //                                            using (FbCommand cmd2 = new FbCommand())
        //                                            {
        //                                                cmd2.CommandText = "PR_Stats_Result_GoalInfo";
        //                                                cmd2.CommandType = CommandType.StoredProcedure;
        //                                                cmd2.Connection = connection;
        //                                                cmd2.Parameters.Add("@EMATCHID", api.data.@event.id);
        //                                                cmd2.Parameters.Add("@H_YELLOW", api.data.@event.participants[0].stats.FirstOrDefault(c => c.id == 8).value);
        //                                                cmd2.Parameters.Add("@H_RED", api.data.@event.participants[0].stats.FirstOrDefault(c => c.id == 9).value);
        //                                                cmd2.Parameters.Add("@G_YELLOW", api.data.@event.participants[1].stats.FirstOrDefault(c => c.id == 8).value);
        //                                                cmd2.Parameters.Add("@G_RED", api.data.@event.participants[1].stats.FirstOrDefault(c => c.id == 9).value);
        //                                                cmd2.Parameters.Add("@H_GOAL", api.data.@event.participants[0].results.FirstOrDefault(c => c.id == 2).value);
        //                                                cmd2.Parameters.Add("@G_GOAL", api.data.@event.participants[1].results.FirstOrDefault(c => c.id == 2).value);
        //                                                cmd2.Parameters.Add("@LASTTIME", DateTime.Now);
        //                                                id = Convert.ToInt32(cmd2.ExecuteScalar());
        //                                                // Files.WriteLog((id > 0 ? " [Success] Update GoalInfo " : " [Failure]  Update GoalInfo ") + "[" + api.data.@event.id + "/" + api.data.@event.participants[0].id + "]" + id);
        //                                                if (id > -1) Files.WriteLog(" [Success] Update GoalInfo " + "[" + api.data.@event.id + "/" + api.data.@event.participants[0].id + "]" + id);
        //                                            }

        //                                            //using (FbCommand cmd2 = new FbCommand())
        //                                            //{
        //                                            //    cmd2.CommandText = "PR_stats_GoalInfo";
        //                                            //    cmd2.CommandType = CommandType.StoredProcedure;
        //                                            //    cmd2.Connection = connection;
        //                                            //    cmd2.Parameters.Add("@EMATCHID", api.data.@event.id);
        //                                            //    cmd2.Parameters.Add("@H_YELLOW", api.data.@event.participants[0].stats.FirstOrDefault(c => c.id == 8).value);
        //                                            //    cmd2.Parameters.Add("@H_RED", api.data.@event.participants[0].stats.FirstOrDefault(c => c.id == 9).value);
        //                                            //    cmd2.Parameters.Add("@G_YELLOW", api.data.@event.participants[1].stats.FirstOrDefault(c => c.id == 8).value);
        //                                            //    cmd2.Parameters.Add("@G_RED", api.data.@event.participants[1].stats.FirstOrDefault(c => c.id == 9).value);
        //                                            //    cmd2.Parameters.Add("@LASTTIME", DateTime.Now);
        //                                            //    id = Convert.ToInt32(cmd2.ExecuteScalar());
        //                                            //    Files.WriteLog((id > 0 ? " [Success] Update GoalInfo " : " [Failure]  Update GoalInfo ") + "[" + api.data.@event.id + "/" + api.data.@event.participants[0].id + "]");
        //                                            //}

        //                                            //using (FbCommand cmd2 = new FbCommand())
        //                                            //{
        //                                            //    cmd2.CommandText = "PR_Result_GoalInfo";
        //                                            //    cmd2.CommandType = CommandType.StoredProcedure;
        //                                            //    cmd2.Connection = connection;
        //                                            //    cmd2.Parameters.Add("@EMATCHID", api.data.@event.id);
        //                                            //    cmd2.Parameters.Add("@H_GOAL", api.data.@event.participants[0].results.FirstOrDefault(c => c.id == 2).value);
        //                                            //    cmd2.Parameters.Add("@G_GOAL", api.data.@event.participants[1].results.FirstOrDefault(c => c.id == 2).value);
        //                                            //    cmd2.Parameters.Add("@LASTTIME", DateTime.Now);
        //                                            //    id = Convert.ToInt32(cmd2.ExecuteScalar());
        //                                            //    Files.WriteLog((id > -1 ? " [Success] Update GoalInfo Goal " : " [Failure]  Update GoalInfo Goal") + "[" + api.data.@event.id + "/" + api.data.@event.participants[0].id + "]" + id);
        //                                            //}

        //                                            if (id > 0)
        //                                            {
        //                                                SendAlertMsg(AppFlag.LIVEGOALS);
        //                                            }

        //                                            if (!backgroundWorker.IsBusy && id > 0)
        //                                            {
        //                                                //  backgroundWorker.RunWorkerAsync("[" + api.data.@event.id + "] "+ api.data.@event.name + " " + strName + ".json");
        //                                                backgroundWorker.RunWorkerAsync(api.data.@event.name + " " + strName + ".json");
        //                                            }
        //                                        }
        //                                    }
        //                                }

        //                                if (api.data.@event.status_id == 9 || api.data.@event.status_id == 11)
        //                                {
        //                                    if (api.data.@event.id > 0)
        //                                    {
        //                                        /// await AyncHandleData("events.show3", true, api.data.@event.id.ToString());
        //                                        InsertData("events.show3", true, api.data.@event.id.ToString());
        //                                        Files.WriteLog(" Housekeep [" + api.data.@event.id.ToString() + "].." + api.data.@event.status_id);
        //                                    }
        //                                }
        //                                //    connection.Close();
        //                                //}

        //                                ////if (!backgroundWorker.IsBusy&& id > 0)
        //                                ////{
        //                                ////    backgroundWorker.RunWorkerAsync("[" + api.data.@event.id + "] " + strName + ".json");
        //                                ////}
        //                            }
        //                            //else if (api != null)
        //                            //{
        //                            //    //strName = "other_" + DateTime.Now.ToString("HHmmssfff");
        //                            //    //Files.WriteJson("other_" + DateTime.Now.ToString("HHmm"), message);
        //                            //}
        //                            //}
        //                            // else if (message.IndexOf("\"type\": \"incident\"") > 0)
        //                            else if (api != null && api.type == "incident")
        //                            {
        //                                int sID = -1;
        //                                DOSIncidentJson.IncidentJson incidentJson = JsonUtil.Deserialize(typeof(DOSIncidentJson.IncidentJson), message) as DOSIncidentJson.IncidentJson;
        //                                iCount2++;
        //                                ////if (AppFlag.TestMode) Files.WriteTestLog("Queue", iCount2.ToString() + " Incid " + api.id);

        //                                if (incidentJson != null && incidentJson.data.@event.sport_id == 5 && incidentJson.data.incident.important_for_trader == "yes")
        //                                {
        //                                    //  strName = "Incid_" + incidentJson.data.@event.id +"_"+ incidentJson.data.incident.event_status_id + "_"+ (incidentJson.data.incident.incident_id == 413 || incidentJson.data.incident.incident_id == 418 || incidentJson.data.incident.incident_id == 419 ? "_" + incidentJson.data.incident.incident_id : "") + incidentJson.data.incident.id + "-" + DateTime.Now.ToString("HHmmssfff");
        //                                    strName = "Incid_" + incidentJson.data.@event.id + "_" + incidentJson.data.incident.event_status_id + "_" + incidentJson.data.incident.incident_id + "-" + incidentJson.data.incident.id + "-" + DateTime.Now.ToString("HHmmssfff");
        //                                    Files.WriteJson(strName, message);
        //                                    //if(incidentJson.data.incident.incident_id== 429)
        //                                    //{ 
        //                                    //}
        //                                    //using (FbConnection connection = new FbConnection(AppFlag.ScoutsDBConn))
        //                                    //{
        //                                    //connection.Open();
        //                                    using (FbCommand cmd2 = new FbCommand())
        //                                    {
        //                                        cmd2.CommandText = "PR_INCIDENTS";
        //                                        cmd2.CommandType = CommandType.StoredProcedure;
        //                                        cmd2.Connection = connection;
        //                                        cmd2.Parameters.Add("@ID", incidentJson.data.incident.id);
        //                                        cmd2.Parameters.Add("@EVENTID", incidentJson.data.@event.id);
        //                                        cmd2.Parameters.Add("@CACTION", incidentJson.data.incident.action);
        //                                        cmd2.Parameters.Add("@INCIDENT_ID", incidentJson.data.incident.incident_id);
        //                                        cmd2.Parameters.Add("@INCIDENT_NAME", incidentJson.data.incident.incident_name);
        //                                        cmd2.Parameters.Add("@PARTICIPANT_ID", incidentJson.data.incident.participant_id);
        //                                        cmd2.Parameters.Add("@PARTICIPANT_NAME", incidentJson.data.incident.participant_name);
        //                                        cmd2.Parameters.Add("@SUBPARTICIPANT_ID", incidentJson.data.incident.subparticipant_id);
        //                                        cmd2.Parameters.Add("@SUBPARTICIPANT_NAME", incidentJson.data.incident.subparticipant_name);
        //                                        cmd2.Parameters.Add("@IMPORTANT_FOR_TRADER", true);// incidentJson.data.incident.important_for_trader == "yes" ? true : false);
        //                                        cmd2.Parameters.Add("@EVENT_TIME", incidentJson.data.incident.event_time);
        //                                        cmd2.Parameters.Add("@EVENT_STATUS_ID", incidentJson.data.incident.event_status_id);
        //                                        cmd2.Parameters.Add("@EVENT_STATUS_NAME", incidentJson.data.incident.event_status_name);
        //                                        cmd2.Parameters.Add("@UT", incidentJson.ut);
        //                                        cmd2.Parameters.Add("@CTIMESTAMP", DateTime.Now);
        //                                        cmd2.Parameters.Add("@TEAMTYPE",
        //                                        (incidentJson.data.incident.participant_id == null ? "" : incidentJson.data.incident.participant_id.ToString() == incidentJson.data.@event.participants[0].id.ToString() ? "H" : incidentJson.data.incident.participant_id.ToString() == incidentJson.data.@event.participants[1].id.ToString() ? "G" : "H"));
        //                                        sID = Convert.ToInt32(cmd2.ExecuteScalar());
        //                                        Files.WriteLog((sID > -1 ? " [Success] Insert INCIDENTS " : " [Failure] Insert INCIDENTS ") + "[" + incidentJson.data.@event.id + "]," + strName + ".json");

        //                                        if (sID > 0) { SendAlertMsg(AppFlag.GOALDETAILS); }

        //                                        if (!backgroundWorker.IsBusy && sID > 0)
        //                                        {
        //                                            // backgroundWorker.RunWorkerAsync("[" + incidentJson.data.@event.id + "] " + incidentJson.data.@event.name + " " + strName + ".json");
        //                                            backgroundWorker.RunWorkerAsync(incidentJson.data.@event.name + " " + strName + ".json");
        //                                        }
        //                                    }

        //                                    if (incidentJson.data.incident.incident_id == 429 || incidentJson.data.@event.status_id == 9 || incidentJson.data.@event.status_id == 11)
        //                                    {
        //                                        if (incidentJson.data.@event.id > 0)
        //                                        {
        //                                            //  await AyncHandleData("events.show3", true, incidentJson.data.@event.id.ToString());
        //                                            InsertData("events.show3", true, incidentJson.data.@event.id.ToString());
        //                                            Files.WriteLog(" Housekeep incident [" + incidentJson.data.@event.id.ToString() + "].");
        //                                            if (incidentJson.data.incident.incident_id == 429)
        //                                            {
        //                                                InsertData("events.participants", true, incidentJson.data.@event.id.ToString());
        //                                                Files.WriteLog(" Housekeep event participant [" + incidentJson.data.@event.id.ToString() + "]." + incidentJson.data.incident.incident_id);

        //                                            }
        //                                        }
        //                                    }
        //                                    //    connection.Close();
        //                                    //}
        //                                    ////if (!backgroundWorker.IsBusy && sID > 0)
        //                                    ////{
        //                                    ////    backgroundWorker.RunWorkerAsync("[" + incidentJson.data.@event.id + "] " + strName + ".json");
        //                                    ////}
        //                                }
        //                                else if (incidentJson != null && incidentJson.data.@event.sport_id == 5 && incidentJson.data.incident.important_for_trader == "no")
        //                                {
        //                                    if (!AppFlag.Important_for_trader)
        //                                    {
        //                                        strName = "Incid_" + incidentJson.data.@event.id + "_" + incidentJson.data.@event.id + "_" + DateTime.Now.ToString("HHmmssfff") + "_no";
        //                                        Files.WriteJson(strName, message);
        //                                    }
        //                                }
        //                                else
        //                                {
        //                                    strName = "Incid_" + incidentJson.data.@event.id + "_" + incidentJson.data.@event.id + "_" + DateTime.Now.ToString("HHmmssfff") + "_other";
        //                                    Files.WriteJson(strName, message);
        //                                }

        //                            }
        //                            else if (api != null && api.type == "event_keep_alive")
        //                            {
        //                                Files.WriteLog("event_keep_alive");
        //                            }
        //                            else if (api != null && api.data.@event.sport_id != 5)
        //                            {
        //                            }
        //                            else
        //                            {
        //                                strName = "other2_" + DateTime.Now.ToString("HHmmssfff");
        //                                Files.WriteJson(strName, message);
        //                            }
        //                            if (bBreak)
        //                            {
        //                                // Files.WriteLog("Stop AMQPService");
        //                                break;
        //                            }
        //                            Thread.Sleep(TimeSpan.FromMilliseconds(2));
        //                        }
        //                        catch (EndOfStreamException endOfStreamException)
        //                        {
        //                            Files.WriteLog("No message." + (strName != "" ? strName + ".json," : ""));
        //                            Files.WriteError("bgwAMQPService_DoWork(while true)," + (strName != "" ? strName + ".json," : "") + "error: " + endOfStreamException);
        //                            connection.Close();
        //                            bBreak = true;
        //                            e.Result = "No message,break.";
        //                            break;
        //                        }
        //                        catch (Exception exp)
        //                        {
        //                            if (strName != "") Files.WriteLog("Error: " + strName + ".json");
        //                            Files.WriteError("bgwAMQPService_DoWork(while true),error: " + (strName != "" ? strName + ".json," : "") + "error: " + exp);
        //                            e.Result = "No AMQPService.";
        //                            continue;
        //                        }
        //                    }
        //                    connection.Close();
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception exp)
        //    {
        //        Files.WriteError("bgwAMQPService_DoWork(),error: " + exp);
        //        e.Result = "No AMQPService.";
        //        bBreak = true;
        //        ///  bgwAMQPService.RunWorkerAsync();
        //    }
        //} 
        int iCount = 0;
        int iCount2 = 0;
        int iCount3 = 0;
        private void bgwAMQPService_DoWork2(object sender, DoWorkEventArgs e)
        {
            try
            {
                if (AppFlag.TestMode) Files.WriteTestLog("Queue", "------------------Start 1-----------------");
                /// bgwAMQPService.WorkerSupportsCancellation = true;
                backgroundWorker.DoWork += new DoWorkEventHandler(backgroundWorker_DoWork);
                string serverAddress = "amqp://queue.statscore.com:5672/";
                Uri uri = new Uri(serverAddress);
                ConnectionFactory cf = new ConnectionFactory();
                cf.Uri = uri;
                cf.HostName = "queue.statscore.com";
                cf.Port = 5672;
                cf.UserName = "telecom-digital-data-limited";
                cf.Password = "Eb76sQDkn9oZEki5L9QreKpaPmD3GbuuW2I";
                cf.VirtualHost = "statscore";
                cf.RequestedHeartbeat = 60;
                using (IConnection conn = cf.CreateConnection())
                {
                    if (AppFlag.TestMode) Files.WriteTestLog("Queue", "------------------Start 2-----------------");
                    using (IModel channel = conn.CreateModel())
                    {
                        if (AppFlag.TestMode) Files.WriteTestLog("Queue", "------------------Start 3-----------------");
                        using (FbConnection connection = new FbConnection(AppFlag.ScoutsDBConn))
                        {
                            connection.Open();

                            //var consumer = new QueueingBasicConsumer(channel);
                            //var consumerTag = channel.BasicConsume("telecom-digital-data-limited", false, consumer);
                            ////////while (true)
                            ////////{
                            //string strName = "";
                            //try
                            //{

                            //if (!consumer.Queue.Any())
                            //{
                            //    Thread.Sleep(100); continue;
                            //}
                            //else
                            //{
                            //    QueueDeclareOk result = channel.QueueDeclarePassive("telecom-digital-data-limited");
                            //    uint countA = result != null ? result.MessageCount : 0;
                            //    if (AppFlag.TestMode) Files.WriteTestLog("Queue", iCount3.ToString() + "  [" + channel.MessageCount("telecom-digital-data-limited") + "/" + countA + "]");

                            //}
                            //var ea = (BasicDeliverEventArgs)consumer.Queue.Dequeue();
                            //iCount3++;
                            //if (ea == null)
                            //{
                            //    Files.WriteError("ea == null");
                            //    break;
                            //}
                            //var body = ea.Body;
                            //var message = Encoding.UTF8.GetString(body);
                            //if (AppFlag.TestMode) Files.WriteTestLog("msg", message);
                            //channel.BasicAck(ea.DeliveryTag, false);
                            //if (!ack) { channel.BasicQos(0, 1, false); }

                            //channel.QueueDeclare(queue: "task_queue",
                            //     durable: true,
                            //     exclusive: false,
                            //     autoDelete: false,
                            //     arguments: null);

                            /// channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
                            if (AppFlag.TestMode) Files.WriteTestLog("Queue", "------------------Start 4----------------");
                            //   Console.WriteLine(" [*] Waiting for messages.");

                            var consumer = new EventingBasicConsumer(channel);

                            QueueDeclareOk result = channel.QueueDeclarePassive("telecom-digital-data-limited");
                            uint countA = result != null ? result.MessageCount : 0;
                            if (AppFlag.TestMode) Files.WriteTestLog("Queue", iCount3.ToString() + "  [" + channel.MessageCount("telecom-digital-data-limited") + "/" + countA + "]  ---Count ");

                            consumer.Received += (model, ea) =>
                            {
                                iCount3++;
                                if (AppFlag.TestMode) Files.WriteTestLog("Queue", "------------------Start 5----------------");
                                var body = ea.Body;
                                var message = Encoding.UTF8.GetString(body);
                                if (AppFlag.TestMode) Files.WriteTestLog("msg", message);
                                if (AppFlag.TestMode) Files.WriteTestLog("Queue", "------------------Start 6----------------");
                                string strName = "";
                                try
                                {
                                    DOSEventJson.EventJson api = JsonUtil.Deserialize(typeof(DOSEventJson.EventJson), message) as DOSEventJson.EventJson;

                                    if (api != null && api.type == "event" && (AppFlag.JsonType ? api.data.@event.sport_id == 5 : api.data.@event.sport_id != -1))
                                    {
                                        iCount++;
                                        int id = -2;
                                        strName = api.data.@event.id + "_" + (api.data.@event.status_id) + "_" + DateTime.Now.ToString("HHmmssfff");
                                        Files.WriteJson(strName, message);
                                        using (FbCommand cmd2 = new FbCommand())
                                        {
                                            cmd2.CommandText = "PR_JSON_event";
                                            cmd2.CommandType = CommandType.StoredProcedure;
                                            cmd2.Connection = connection;
                                            cmd2.Parameters.Add("@ID", api.data.@event.id);
                                            cmd2.Parameters.Add("@NAME", api.data.@event.name);
                                            cmd2.Parameters.Add("@HOME_ID", api.data.@event.participants.Count() > 0 ? api.data.@event.participants[0].id : -1);
                                            cmd2.Parameters.Add("@GUEST_ID", api.data.@event.participants.Count() > 0 ? api.data.@event.participants[1].id : -1);
                                            // cmd2.Parameters.Add("@SOURCE", api.data.@evenT.S);
                                            // cmd2.Parameters.Add("@SOURCE_DC", api.data.@event.S);
                                            //cmd2.Parameters.Add("@SOURCE_SUPER", api.data.@event.s);
                                            cmd2.Parameters.Add("@RELATION_STATUS", api.data.@event.relation_status);
                                            cmd2.Parameters.Add("@START_DATE", Convert.ToDateTime(api.data.@event.start_date).AddHours(8));
                                            cmd2.Parameters.Add("@FT_ONLY", api.data.@event.ft_only == "yes" ? true : false);
                                            cmd2.Parameters.Add("@COVERAGE_TYPE", api.data.@event.coverage_type);
                                            //cmd2.Parameters.Add("@CHANNEL_ID", api.data.@event.CH);
                                            //cmd2.Parameters.Add("@CHANNEL_NAME", api.data.@event.C);
                                            cmd2.Parameters.Add("@SCOUTSFEED", api.data.@event.scoutsfeed == "yes" ? true : false);
                                            cmd2.Parameters.Add("@STATUS_ID", api.data.@event.status_id);
                                            //cmd2.Parameters.Add("@STATUS_NAME", api.data.@event.STA);
                                            cmd2.Parameters.Add("@STATUS_TYPE", api.data.@event.status_type);
                                            cmd2.Parameters.Add("@CDAY", api.data.@event.day);
                                            cmd2.Parameters.Add("@CLOCK_TIME", api.data.@event.clock_time);
                                            cmd2.Parameters.Add("@CLOCK_STATUS", api.data.@event.clock_status);
                                            // cmd2.Parameters.Add("@WINNER_ID", api.data.@event.W);
                                            //cmd2.Parameters.Add("@PROGRESS_ID", api.data.@event.PR);
                                            cmd2.Parameters.Add("@BET_STATUS", api.data.@event.bet_status);
                                            cmd2.Parameters.Add("@NEUTRAL_VENUE", api.data.@event.neutral_venue == "yes" ? true : false);
                                            cmd2.Parameters.Add("@ITEM_STATUS", api.data.@event.item_status);
                                            // cmd2.Parameters.Add("@UT", api.data.@event.U);
                                            // cmd2.Parameters.Add("@OLD_EVENT_ID", api.data.@event.OL);
                                            // cmd2.Parameters.Add("@SLUG", api.data.@event.S);
                                            // cmd2.Parameters.Add("@VERIFIED_RESULT", api.data.@event.VE);
                                            // cmd2.Parameters.Add("@IS_PROTOCOL_VERIFIED", api.data.@event.IS);
                                            //  cmd2.Parameters.Add("@PROTOCOL_VERIFIED_BY", api.data.@event.PRO);
                                            //cmd2.Parameters.Add("@PROTOCOL_VERIFIED_AT", api.data.@event.PRO);
                                            cmd2.Parameters.Add("@ROUND_ID", api.data.@event.round_id);
                                            cmd2.Parameters.Add("@ROUND_NAME", api.data.@event.round_name);
                                            //cmd2.Parameters.Add("@CLIENT_EVENT_ID", api.data.@event.C);
                                            // cmd2.Parameters.Add("@BOOKED", null);
                                            // cmd2.Parameters.Add("@BOOKED_BY", api.data.@event.);
                                            // cmd2.Parameters.Add("@INVERTED_PARTICIPANTS", api.data.@event.iv);
                                            cmd2.Parameters.Add("@VENUE_ID", api.data.@event.tour_id == null ? "-1" : api.data.@event.tour_id);
                                            //  cmd2.Parameters.Add("@GROUP_ID", api.data.@event.gr);
                                            cmd2.Parameters.Add("@STAGE_ID", api.data.@event.stage_id);
                                            cmd2.Parameters.Add("@SEASON_ID", api.data.@event.season_id);
                                            cmd2.Parameters.Add("@COMPETITION_ID", api.data.@event.competition_id);
                                            cmd2.Parameters.Add("@AREA_ID", api.data.@event.area_id);
                                            cmd2.Parameters.Add("@CTIMESTAMP", DateTime.Now);
                                            cmd2.Parameters.Add("@CACTION", api.data.@event.action);
                                            id = Convert.ToInt32(cmd2.ExecuteScalar());
                                            Files.WriteLog((id == 0 ? " [Success] Insert event " : id == 1 ? " Update event " : "") + "[" + api.data.@event.id + "]," + strName + ".json");
                                            // Files.WriteLog((id == 0 ? " [Success] Insert event [" + api.data.@event.id + "]," + strName + ".json":"");
                                        }


                                        if (api.data.@event.details.Count() > 0)
                                        {
                                            using (FbCommand cmd2 = new FbCommand())
                                            {
                                                cmd2.CommandText = "PR_event_details";
                                                cmd2.CommandType = CommandType.StoredProcedure;
                                                cmd2.Connection = connection;
                                                cmd2.Parameters.Add("@EVENTID", api.data.@event.id);
                                                cmd2.Parameters.Add("@WC_8", api.data.@event.details.FirstOrDefault(c => c.id == 8).value);
                                                cmd2.Parameters.Add("@PC_36", api.data.@event.details.FirstOrDefault(c => c.id == 36).value);
                                                cmd2.Parameters.Add("@PL_16", api.data.@event.details.FirstOrDefault(c => c.id == 16).value);
                                                cmd2.Parameters.Add("@EPL_50", api.data.@event.details.FirstOrDefault(c => c.id == 50).value);
                                                cmd2.Parameters.Add("@NOP_17", api.data.@event.details.FirstOrDefault(c => c.id == 17).value);
                                                cmd2.Parameters.Add("@EPTC_58", api.data.@event.details.FirstOrDefault(c => c.id == 58).value);
                                                cmd2.Parameters.Add("@IT_151", api.data.@event.details.FirstOrDefault(c => c.id == 151).value);
                                                cmd2.Parameters.Add("@ATT_141", api.data.@event.details.FirstOrDefault(c => c.id == 141).value);
                                                cmd2.Parameters.Add("@FHSD_19", api.data.@event.details.FirstOrDefault(c => c.id == 19).value == null ? DateTime.MinValue : Convert.ToDateTime(api.data.@event.details.FirstOrDefault(c => c.id == 19).value).AddHours(8));
                                                cmd2.Parameters.Add("@SHSD_20", api.data.@event.details.FirstOrDefault(c => c.id == 20).value == null ? DateTime.MinValue : Convert.ToDateTime(api.data.@event.details.FirstOrDefault(c => c.id == 20).value).AddHours(8));
                                                cmd2.Parameters.Add("@FEHSD_44", api.data.@event.details.FirstOrDefault(c => c.id == 44).value);
                                                cmd2.Parameters.Add("@SEHSD_45", api.data.@event.details.FirstOrDefault(c => c.id == 45).value);
                                                cmd2.Parameters.Add("@PSSD_150", api.data.@event.details.FirstOrDefault(c => c.id == 150).value);
                                                cmd2.Parameters.Add("@FHIT_201", api.data.@event.details.FirstOrDefault(c => c.id == 201).value);
                                                cmd2.Parameters.Add("@SHIT_202", api.data.@event.details.FirstOrDefault(c => c.id == 202).value);
                                                cmd2.Parameters.Add("@FEHIT_203", api.data.@event.details.FirstOrDefault(c => c.id == 203).value);
                                                cmd2.Parameters.Add("@SEHIT_204", api.data.@event.details.FirstOrDefault(c => c.id == 204).value);
                                                cmd2.Parameters.Add("@HL_205", api.data.@event.details.FirstOrDefault(c => c.id == 205).value);
                                                cmd2.Parameters.Add("@TD_124", api.data.@event.details.FirstOrDefault(c => c.id == 124).value);
                                                cmd2.Parameters.Add("@BM_160", api.data.@event.details.FirstOrDefault(c => c.id == 160).value);
                                                cmd2.Parameters.Add("@HF_178", api.data.@event.details.FirstOrDefault(c => c.id == 178).value);
                                                cmd2.Parameters.Add("@UT", api.ut);
                                                cmd2.Parameters.Add("@CTIMESTAMP", DateTime.Now);
                                                id = Convert.ToInt32(cmd2.ExecuteScalar());
                                                Files.WriteLog((id > 0 ? " [Success] Insert event_details " : id == -1 ? " Old data " : " [Failure] Insert event_details ") + "[" + api.data.@event.id + "]," + strName + ".json");
                                            }

                                            if (id != -1)
                                            {
                                                for (int i = 0; i < api.data.@event.participants.Length; i++)
                                                {
                                                    using (FbCommand cmd2 = new FbCommand())
                                                    {
                                                        cmd2.CommandText = "PR_participant_results";
                                                        cmd2.CommandType = CommandType.StoredProcedure;
                                                        cmd2.Connection = connection;
                                                        // cmd2.Parameters.Add("@ID", 0);
                                                        cmd2.Parameters.Add("@EVENTID", api.data.@event.id);
                                                        cmd2.Parameters.Add("@PARTICIPANTID", api.data.@event.participants[i].id);
                                                        cmd2.Parameters.Add("@PROGRESS_412", api.data.@event.participants[i].results.FirstOrDefault(c => c.id == 412).value);
                                                        cmd2.Parameters.Add("@WINNER_411", api.data.@event.participants[i].results.FirstOrDefault(c => c.id == 411).value);
                                                        cmd2.Parameters.Add("@RESULT_2", api.data.@event.participants[i].results.FirstOrDefault(c => c.id == 2).value);
                                                        cmd2.Parameters.Add("@RT_3", api.data.@event.participants[i].results.FirstOrDefault(c => c.id == 3).value);
                                                        cmd2.Parameters.Add("@FH_4", api.data.@event.participants[i].results.FirstOrDefault(c => c.id == 4).value);
                                                        cmd2.Parameters.Add("@SH_5", api.data.@event.participants[i].results.FirstOrDefault(c => c.id == 5).value);
                                                        cmd2.Parameters.Add("@E1H_133", api.data.@event.participants[i].results.FirstOrDefault(c => c.id == 133).value);
                                                        cmd2.Parameters.Add("@E2H_134", api.data.@event.participants[i].results.FirstOrDefault(c => c.id == 134).value);
                                                        cmd2.Parameters.Add("@PENALTY_7", api.data.@event.participants[i].results.FirstOrDefault(c => c.id == 7).value);
                                                        cmd2.Parameters.Add("@OVERTIME_104", api.data.@event.participants[i].results.FirstOrDefault(c => c.id == 104).value);
                                                        cmd2.Parameters.Add("@CTIMESTAMP", DateTime.Now);
                                                        cmd2.Parameters.Add("@CACTION", api.data.@event.action);
                                                        cmd2.Parameters.Add("@TEAMTYPE", api.data.@event.participants[i].counter == 1 ? "H" : "G");
                                                        id = Convert.ToInt32(cmd2.ExecuteScalar());
                                                        Files.WriteLog((id > 0 ? " [Success] Insert participant_results" : " [Failure] Insert participant_results") + "[" + api.data.@event.id + "/" + api.data.@event.participants[0].id + "]");
                                                    }

                                                    using (FbCommand cmd2 = new FbCommand())
                                                    {
                                                        cmd2.CommandText = "PR_participant_stats";
                                                        cmd2.CommandType = CommandType.StoredProcedure;
                                                        cmd2.Connection = connection;
                                                        // cmd2.Parameters.Add("@ID", 0);
                                                        cmd2.Parameters.Add("@EVENTID", api.data.@event.id);
                                                        cmd2.Parameters.Add("@PARTICIPANTID", api.data.@event.participants[i].id);
                                                        cmd2.Parameters.Add("@SOT_20", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 20).value);
                                                        cmd2.Parameters.Add("@SOT_21", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 21).value);
                                                        cmd2.Parameters.Add("@ATTACKS_10", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 10).value);
                                                        cmd2.Parameters.Add("@DA_11", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 11).value);
                                                        cmd2.Parameters.Add("@CORNERS_13", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 13).value);
                                                        cmd2.Parameters.Add("@YELLOW_CARDS_8", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 8).value);
                                                        cmd2.Parameters.Add("@RED_CARDS_9", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 9).value);
                                                        cmd2.Parameters.Add("@TOTAL_SHOTS_19", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 19).value);
                                                        cmd2.Parameters.Add("@FOULS_22", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 22).value);
                                                        cmd2.Parameters.Add("@OFFSIDES_24", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 24).value);
                                                        cmd2.Parameters.Add("@PS_14", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 14).value);
                                                        cmd2.Parameters.Add("@PM_15", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 15).value);
                                                        cmd2.Parameters.Add("@PG_16", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 16).value);
                                                        cmd2.Parameters.Add("@FK_25", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 25).value);
                                                        cmd2.Parameters.Add("@DFK_26", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 26).value);
                                                        cmd2.Parameters.Add("@FKG_18", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 18).value);
                                                        cmd2.Parameters.Add("@SW_27", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 27).value);
                                                        cmd2.Parameters.Add("@SB_28", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 28).value);
                                                        cmd2.Parameters.Add("@GS_29", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 29).value);
                                                        cmd2.Parameters.Add("@GK_30", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 30).value);
                                                        cmd2.Parameters.Add("@TI_32", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 32).value);
                                                        cmd2.Parameters.Add("@SUBSTITUTIONS_31", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 31).value);
                                                        cmd2.Parameters.Add("@GOALS_40", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 40).value);
                                                        cmd2.Parameters.Add("@MP_34", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 34).value);
                                                        cmd2.Parameters.Add("@OWN_GOALS_17", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 17).value);
                                                        cmd2.Parameters.Add("@ADW_33", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 33).value);
                                                        cmd2.Parameters.Add("@FORM_716", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 716).value);
                                                        cmd2.Parameters.Add("@SKIN_718", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 718).value);
                                                        cmd2.Parameters.Add("@PS_639", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 639).value);
                                                        cmd2.Parameters.Add("@PU_697", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 697).value);
                                                        cmd2.Parameters.Add("@GOALS115_772", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 772).value);
                                                        cmd2.Parameters.Add("@GOALS1630_773", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 773).value);
                                                        cmd2.Parameters.Add("@GOALS3145_774", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 774).value);
                                                        cmd2.Parameters.Add("@GOALS4660_775", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 775).value);
                                                        cmd2.Parameters.Add("@GOALS6175_776", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 776).value);
                                                        cmd2.Parameters.Add("@GOALS7690_777", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 777).value);
                                                        cmd2.Parameters.Add("@MPG_778", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 778).value);
                                                        cmd2.Parameters.Add("@MPS_779", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 779).value);
                                                        cmd2.Parameters.Add("@CTIMESTAMP", DateTime.Now);
                                                        cmd2.Parameters.Add("@CACTION", api.data.@event.action);
                                                        cmd2.Parameters.Add("@TEAMTYPE", api.data.@event.participants[i].counter == 2 ? "G" : "H");
                                                        id = Convert.ToInt32(cmd2.ExecuteScalar());
                                                        Files.WriteLog((id > 0 ? " [Success] Insert participant_stats" : " [Failure] Insert participant_results") + "[" + api.data.@event.id + "/" + api.data.@event.participants[0].id + "]");
                                                    }
                                                }
                                                if (api.data.@event.participants.Length == 2 && api.data.@event.action != "delete")
                                                {

                                                    using (FbCommand cmd2 = new FbCommand())
                                                    {
                                                        cmd2.CommandText = "PR_Stats_Result_GoalInfo";
                                                        cmd2.CommandType = CommandType.StoredProcedure;
                                                        cmd2.Connection = connection;
                                                        cmd2.Parameters.Add("@EMATCHID", api.data.@event.id);
                                                        cmd2.Parameters.Add("@H_YELLOW", api.data.@event.participants[0].stats.FirstOrDefault(c => c.id == 8).value);
                                                        cmd2.Parameters.Add("@H_RED", api.data.@event.participants[0].stats.FirstOrDefault(c => c.id == 9).value);
                                                        cmd2.Parameters.Add("@G_YELLOW", api.data.@event.participants[1].stats.FirstOrDefault(c => c.id == 8).value);
                                                        cmd2.Parameters.Add("@G_RED", api.data.@event.participants[1].stats.FirstOrDefault(c => c.id == 9).value);
                                                        cmd2.Parameters.Add("@H_GOAL", api.data.@event.participants[0].results.FirstOrDefault(c => c.id == 2).value);
                                                        cmd2.Parameters.Add("@G_GOAL", api.data.@event.participants[1].results.FirstOrDefault(c => c.id == 2).value);
                                                        cmd2.Parameters.Add("@LASTTIME", DateTime.Now);
                                                        id = Convert.ToInt32(cmd2.ExecuteScalar());
                                                        // Files.WriteLog((id > 0 ? " [Success] Update GoalInfo " : " [Failure]  Update GoalInfo ") + "[" + api.data.@event.id + "/" + api.data.@event.participants[0].id + "]" + id);
                                                        if (id > -1) Files.WriteLog(" [Success] Update GoalInfo " + "[" + api.data.@event.id + "/" + api.data.@event.participants[0].id + "]" + id);
                                                    }


                                                    if (id > 0)
                                                    {
                                                        SendAlertMsg(AppFlag.LIVEGOALS);
                                                    }

                                                    if (!backgroundWorker.IsBusy && id > 0)
                                                    {
                                                        //  backgroundWorker.RunWorkerAsync("[" + api.data.@event.id + "] "+ api.data.@event.name + " " + strName + ".json");
                                                        backgroundWorker.RunWorkerAsync(api.data.@event.name + " " + strName + ".json");
                                                    }
                                                }
                                            }
                                        }

                                        if (api.data.@event.status_id == 9 || api.data.@event.status_id == 11)
                                        {
                                            if (api.data.@event.id > 0)
                                            {
                                                InsertData("events.show3", true, api.data.@event.id.ToString());
                                                Files.WriteLog(" Housekeep [" + api.data.@event.id.ToString() + "].." + api.data.@event.status_id);
                                            }
                                        }
                                    }
                                    else if (api != null && api.type == "incident")
                                    {
                                        int sID = -1;
                                        DOSIncidentJson.IncidentJson incidentJson = JsonUtil.Deserialize(typeof(DOSIncidentJson.IncidentJson), message) as DOSIncidentJson.IncidentJson;
                                        iCount2++;

                                        if (incidentJson != null && incidentJson.data.@event.sport_id == 5 && incidentJson.data.incident.important_for_trader == "yes")
                                        {
                                            strName = "Incid_" + incidentJson.data.@event.id + "_" + incidentJson.data.incident.event_status_id + "_" + incidentJson.data.incident.incident_id + "-" + incidentJson.data.incident.id + "-" + DateTime.Now.ToString("HHmmssfff");
                                            Files.WriteJson(strName, message);
                                            using (FbCommand cmd2 = new FbCommand())
                                            {
                                                cmd2.CommandText = "PR_INCIDENTS";
                                                cmd2.CommandType = CommandType.StoredProcedure;
                                                cmd2.Connection = connection;
                                                cmd2.Parameters.Add("@ID", incidentJson.data.incident.id);
                                                cmd2.Parameters.Add("@EVENTID", incidentJson.data.@event.id);
                                                cmd2.Parameters.Add("@CACTION", incidentJson.data.incident.action);
                                                cmd2.Parameters.Add("@INCIDENT_ID", incidentJson.data.incident.incident_id);
                                                cmd2.Parameters.Add("@INCIDENT_NAME", incidentJson.data.incident.incident_name);
                                                cmd2.Parameters.Add("@PARTICIPANT_ID", incidentJson.data.incident.participant_id);
                                                cmd2.Parameters.Add("@PARTICIPANT_NAME", incidentJson.data.incident.participant_name);
                                                cmd2.Parameters.Add("@SUBPARTICIPANT_ID", incidentJson.data.incident.subparticipant_id);
                                                cmd2.Parameters.Add("@SUBPARTICIPANT_NAME", incidentJson.data.incident.subparticipant_name);
                                                cmd2.Parameters.Add("@IMPORTANT_FOR_TRADER", true);// incidentJson.data.incident.important_for_trader == "yes" ? true : false);
                                                cmd2.Parameters.Add("@EVENT_TIME", incidentJson.data.incident.event_time);
                                                cmd2.Parameters.Add("@EVENT_STATUS_ID", incidentJson.data.incident.event_status_id);
                                                cmd2.Parameters.Add("@EVENT_STATUS_NAME", incidentJson.data.incident.event_status_name);
                                                cmd2.Parameters.Add("@UT", incidentJson.ut);
                                                cmd2.Parameters.Add("@CTIMESTAMP", DateTime.Now);
                                                cmd2.Parameters.Add("@TEAMTYPE",
                                                        (incidentJson.data.incident.participant_id == null ? "" : incidentJson.data.incident.participant_id.ToString() == incidentJson.data.@event.participants[0].id.ToString() ? "H" : incidentJson.data.incident.participant_id.ToString() == incidentJson.data.@event.participants[1].id.ToString() ? "G" : "H"));
                                                sID = Convert.ToInt32(cmd2.ExecuteScalar());
                                                Files.WriteLog((sID > -1 ? " [Success] Insert INCIDENTS " : " [Failure] Insert INCIDENTS ") + "[" + incidentJson.data.@event.id + "]," + strName + ".json");

                                                if (sID > 0) { SendAlertMsg(AppFlag.GOALDETAILS); }

                                                if (!backgroundWorker.IsBusy && sID > 0)
                                                {
                                                    backgroundWorker.RunWorkerAsync(incidentJson.data.@event.name + " " + strName + ".json");
                                                }
                                            }

                                            if (incidentJson.data.incident.incident_id == 429 || incidentJson.data.@event.status_id == 9 || incidentJson.data.@event.status_id == 11)
                                            {
                                                if (incidentJson.data.@event.id > 0)
                                                {
                                                    //  await AyncHandleData("events.show3", true, incidentJson.data.@event.id.ToString());
                                                    InsertData("events.show3", true, incidentJson.data.@event.id.ToString());
                                                    Files.WriteLog(" Housekeep incident [" + incidentJson.data.@event.id.ToString() + "].");
                                                    if (incidentJson.data.incident.incident_id == 429)
                                                    {
                                                        InsertData("events.participants", true, incidentJson.data.@event.id.ToString());
                                                        Files.WriteLog(" Housekeep event participant [" + incidentJson.data.@event.id.ToString() + "]." + incidentJson.data.incident.incident_id);

                                                    }
                                                }
                                            }
                                        }
                                        else if (incidentJson != null && incidentJson.data.@event.sport_id == 5 && incidentJson.data.incident.important_for_trader == "no")
                                        {
                                            if (!AppFlag.Important_for_trader)
                                            {
                                                strName = "Incid_" + incidentJson.data.@event.id + "_" + incidentJson.data.@event.id + "_" + DateTime.Now.ToString("HHmmssfff") + "_no";
                                                Files.WriteJson(strName, message);
                                            }
                                        }
                                        else
                                        {
                                            strName = "Incid_" + incidentJson.data.@event.id + "_" + incidentJson.data.@event.id + "_" + DateTime.Now.ToString("HHmmssfff") + "_other";
                                            Files.WriteJson(strName, message);
                                        }

                                    }
                                    else if (api != null && api.type == "event_keep_alive")
                                    {
                                        Files.WriteLog("event_keep_alive");
                                    }
                                    else if (api != null && api.data.@event.sport_id != 5)
                                    {
                                    }
                                    else
                                    {
                                        strName = "other2_" + DateTime.Now.ToString("HHmmssfff");
                                        Files.WriteJson(strName, message);
                                    }
                                    if (bBreak)
                                    {
                                        Files.WriteLog("Stop AMQPService bBreak");
                                        ////////break;
                                    }
                                    Thread.Sleep(TimeSpan.FromMilliseconds(2));
                                }
                                catch (EndOfStreamException endOfStreamException)
                                {
                                    Files.WriteLog("No message." + (strName != "" ? strName + ".json," : ""));
                                    Files.WriteError("bgwAMQPService_DoWork(while true)," + (strName != "" ? strName + ".json," : "") + "error: " + endOfStreamException);
                                    connection.Close();
                                    bBreak = true;
                                    e.Result = "No message,break.";
                                    //////break;
                                }
                                catch (Exception exp)
                                {
                                    if (strName != "") Files.WriteLog("Error: " + strName + ".json");
                                    Files.WriteError("bgwAMQPService_DoWork(while true),error: " + (strName != "" ? strName + ".json," : "") + "error: " + exp);
                                    e.Result = "No AMQPService.";
                                    //////continue;
                                }
                                //     Console.WriteLine(" [x] Received {0}", message);

                                //     int dots = message.Split('.').Length - 1;
                                //     Thread.Sleep(dots * 1000);

                                // Console.WriteLine(" [x] Done");
                                if (AppFlag.TestMode) Files.WriteTestLog("Queue", "------------------Start 7----------------");
                                channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                                if (AppFlag.TestMode) Files.WriteTestLog("Queue", "------------------Start 8----------------");
                            };
                            if (AppFlag.TestMode) Files.WriteTestLog("Queue", "------------------Start 9----------------");
                            channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
                            channel.BasicConsume(queue: "telecom-digital-data-limited",
                                                 autoAck: false,
                                                 consumer: consumer);
                            if (AppFlag.TestMode) Files.WriteTestLog("Queue", "------------------Start 10----------------");
                            //  Console.WriteLine(" Press [enter] to exit.");
                            //  Console.ReadLine();


                            //////////}
                            connection.Close();
                        }
                        if (AppFlag.TestMode) Files.WriteTestLog("Queue", "------------------Start 11----------------");
                    }
                    if (AppFlag.TestMode) Files.WriteTestLog("Queue", "------------------Start 12----------------");
                }
                if (AppFlag.TestMode) Files.WriteTestLog("Queue", "------------------Start 13----------------");
            }
            catch (Exception exp)
            {
                if (AppFlag.TestMode) Files.WriteTestLog("Queue", "------------------Start 14----------------");
                Files.WriteError("bgwAMQPService_DoWork(),error: " + exp);
                e.Result = "No AMQPService.";
                bBreak = true;
            }
        }

        public void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            //int id = -2;
            //String message = e.Argument.ToString();
            ////string strName = "amqp" + DateTime.Now.ToString("HHmmssfffff");
            ////Files.WriteJson(strName, message);
            //DOSEventJson.api api = JsonUtil.Deserialize(message) as DOSEventJson.api;
            // using (FbConnection connection = new FbConnection(AppFlag.ScoutsDBConn))
            //{
            //    connection.Open();
            //    using (FbCommand cmd2 = new FbCommand())
            //    {
            //        cmd2.CommandText = "PR_event_details";
            //        cmd2.CommandType = CommandType.StoredProcedure;
            //        cmd2.Connection = connection;
            //        //  cmd2.Parameters.Add("@ID", 0);
            //        cmd2.Parameters.Add("@EVENTID", api.data.@event.id);
            //        cmd2.Parameters.Add("@WC_8", api.data.@event.details.FirstOrDefault(c => c.id == 8).value);
            //        cmd2.Parameters.Add("@PC_36", api.data.@event.details.FirstOrDefault(c => c.id == 36).value);
            //        cmd2.Parameters.Add("@PL_16", api.data.@event.details.FirstOrDefault(c => c.id == 16).value);
            //        cmd2.Parameters.Add("@EPL_50", api.data.@event.details.FirstOrDefault(c => c.id == 50).value);
            //        cmd2.Parameters.Add("@NOP_17", api.data.@event.details.FirstOrDefault(c => c.id == 17).value);
            //        cmd2.Parameters.Add("@EPTC_58", api.data.@event.details.FirstOrDefault(c => c.id == 58).value);
            //        cmd2.Parameters.Add("@IT_151", api.data.@event.details.FirstOrDefault(c => c.id == 151).value);
            //        cmd2.Parameters.Add("@ATT_141", api.data.@event.details.FirstOrDefault(c => c.id == 141).value);
            //        cmd2.Parameters.Add("@FHSD_19", api.data.@event.details.FirstOrDefault(c => c.id == 19).value);
            //        cmd2.Parameters.Add("@SHSD_20", api.data.@event.details.FirstOrDefault(c => c.id == 20).value);
            //        cmd2.Parameters.Add("@FEHSD_44", api.data.@event.details.FirstOrDefault(c => c.id == 44).value);
            //        cmd2.Parameters.Add("@SEHSD_45", api.data.@event.details.FirstOrDefault(c => c.id == 45).value);
            //        cmd2.Parameters.Add("@PSSD_150", api.data.@event.details.FirstOrDefault(c => c.id == 150).value);
            //        cmd2.Parameters.Add("@FHIT_201", api.data.@event.details.FirstOrDefault(c => c.id == 201).value);
            //        cmd2.Parameters.Add("@SHIT_202", api.data.@event.details.FirstOrDefault(c => c.id == 202).value);
            //        cmd2.Parameters.Add("@FEHIT_203", api.data.@event.details.FirstOrDefault(c => c.id == 203).value);
            //        cmd2.Parameters.Add("@SEHIT_204", api.data.@event.details.FirstOrDefault(c => c.id == 204).value);
            //        cmd2.Parameters.Add("@HL_205", api.data.@event.details.FirstOrDefault(c => c.id == 205).value);
            //        cmd2.Parameters.Add("@TD_124", api.data.@event.details.FirstOrDefault(c => c.id == 124).value);
            //        cmd2.Parameters.Add("@BM_160", api.data.@event.details.FirstOrDefault(c => c.id == 160).value);
            //        cmd2.Parameters.Add("@HF_178", api.data.@event.details.FirstOrDefault(c => c.id == 178).value);
            //        cmd2.Parameters.Add("@UT", api.ut);
            //        cmd2.Parameters.Add("@CTIMESTAMP", DateTime.Now);
            //        id = Convert.ToInt32(cmd2.ExecuteScalar());
            //        Files.WriteLog((id < 0 ? " [Success] Insert event_details " : id == -1 ? " Old data " : " [Failure] Insert event_details ") + "[" + api.data.@event.id + "]");
            //    }

            //    if (id != -1)
            //    {
            //        for (int i = 0; i < api.data.@event.participants.Length; i++)
            //        {
            //            using (FbCommand cmd2 = new FbCommand())
            //            {
            //                cmd2.CommandText = "PR_participant_results";
            //                cmd2.CommandType = CommandType.StoredProcedure;
            //                cmd2.Connection = connection;
            //                // cmd2.Parameters.Add("@ID", 0);
            //                cmd2.Parameters.Add("@EVENTID", api.data.@event.id);
            //                cmd2.Parameters.Add("@PARTICIPANTID", api.data.@event.participants[i].id);
            //                cmd2.Parameters.Add("@PROGRESS_412", api.data.@event.participants[i].results.FirstOrDefault(c => c.id == 412).value);
            //                cmd2.Parameters.Add("@WINNER_411", api.data.@event.participants[i].results.FirstOrDefault(c => c.id == 411).value);
            //                cmd2.Parameters.Add("@RESULT_2", api.data.@event.participants[i].results.FirstOrDefault(c => c.id == 2).value);
            //                cmd2.Parameters.Add("@RT_3", api.data.@event.participants[i].results.FirstOrDefault(c => c.id == 3).value);
            //                cmd2.Parameters.Add("@FH_4", api.data.@event.participants[i].results.FirstOrDefault(c => c.id == 4).value);
            //                cmd2.Parameters.Add("@SH_5", api.data.@event.participants[i].results.FirstOrDefault(c => c.id == 5).value);
            //                cmd2.Parameters.Add("@E1H_133", api.data.@event.participants[i].results.FirstOrDefault(c => c.id == 133).value);
            //                cmd2.Parameters.Add("@E2H_134", api.data.@event.participants[i].results.FirstOrDefault(c => c.id == 134).value);
            //                cmd2.Parameters.Add("@PENALTY_7", api.data.@event.participants[i].results.FirstOrDefault(c => c.id == 7).value);
            //                cmd2.Parameters.Add("@OVERTIME_104", api.data.@event.participants[i].results.FirstOrDefault(c => c.id == 104).value);
            //                cmd2.Parameters.Add("@CTIMESTAMP", DateTime.Now);
            //                id = Convert.ToInt32(cmd2.ExecuteScalar());
            //                Files.WriteLog((id < 0 ? " [Success] Insert participant_results" : " [Failure] Insert participant_results") + "[" + api.data.@event.id + "/" + api.data.@event.participants[0].id + "]");
            //            }

            //            using (FbCommand cmd2 = new FbCommand())
            //            {
            //                cmd2.CommandText = "PR_participant_stats";
            //                cmd2.CommandType = CommandType.StoredProcedure;
            //                cmd2.Connection = connection;
            //                // cmd2.Parameters.Add("@ID", 0);
            //                cmd2.Parameters.Add("@EVENTID", api.data.@event.id);
            //                cmd2.Parameters.Add("@PARTICIPANTID", api.data.@event.participants[i].id);
            //                cmd2.Parameters.Add("@SOT_20", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 20).value);
            //                cmd2.Parameters.Add("@SOT_21", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 21).value);
            //                cmd2.Parameters.Add("@ATTACKS_10", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 10).value);
            //                cmd2.Parameters.Add("@DA_11", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 11).value);
            //                cmd2.Parameters.Add("@CORNERS_13", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 13).value);
            //                cmd2.Parameters.Add("@YELLOW_CARDS_8", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 8).value);
            //                cmd2.Parameters.Add("@RED_CARDS_9", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 9).value);
            //                cmd2.Parameters.Add("@TOTAL_SHOTS_19", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 19).value);
            //                cmd2.Parameters.Add("@FOULS_22", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 22).value);
            //                cmd2.Parameters.Add("@OFFSIDES_24", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 24).value);
            //                cmd2.Parameters.Add("@PS_14", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 14).value);
            //                cmd2.Parameters.Add("@PM_15", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 15).value);
            //                cmd2.Parameters.Add("@PG_16", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 16).value);
            //                cmd2.Parameters.Add("@FK_25", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 25).value);
            //                cmd2.Parameters.Add("@DFK_26", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 26).value);
            //                cmd2.Parameters.Add("@FKG_18", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 18).value);
            //                cmd2.Parameters.Add("@SW_27", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 27).value);
            //                cmd2.Parameters.Add("@SB_28", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 28).value);
            //                cmd2.Parameters.Add("@GS_29", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 29).value);
            //                cmd2.Parameters.Add("@GK_30", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 30).value);
            //                cmd2.Parameters.Add("@TI_32", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 32).value);
            //                cmd2.Parameters.Add("@SUBSTITUTIONS_31", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 31).value);
            //                cmd2.Parameters.Add("@GOALS_40", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 40).value);
            //                cmd2.Parameters.Add("@MP_34", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 34).value);
            //                cmd2.Parameters.Add("@OWN_GOALS_17", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 17).value);
            //                cmd2.Parameters.Add("@ADW_33", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 33).value);
            //                cmd2.Parameters.Add("@FORM_716", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 716).value);
            //                cmd2.Parameters.Add("@SKIN_718", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 718).value);
            //                cmd2.Parameters.Add("@PS_639", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 639).value);
            //                cmd2.Parameters.Add("@PU_697", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 697).value);
            //                cmd2.Parameters.Add("@GOALS115_772", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 772).value);
            //                cmd2.Parameters.Add("@GOALS1630_773", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 773).value);
            //                cmd2.Parameters.Add("@GOALS3145_774", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 774).value);
            //                cmd2.Parameters.Add("@GOALS4660_775", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 775).value);
            //                cmd2.Parameters.Add("@GOALS6175_776", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 776).value);
            //                cmd2.Parameters.Add("@GOALS7690_777", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 777).value);
            //                cmd2.Parameters.Add("@MPG_778", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 778).value);
            //                cmd2.Parameters.Add("@MPS_779", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 779).value);
            //                cmd2.Parameters.Add("@CTIMESTAMP", DateTime.Now);
            //                id = Convert.ToInt32(cmd2.ExecuteScalar());
            //                Files.WriteLog((id < 0 ? " [Success] Insert participant_results" : " [Failure] Insert participant_results") + "[" + api.data.@event.id + "/" + api.data.@event.participants[0].id + "]");
            //            }
            //        }
            //    }
            //    connection.Close();
            //}

            //this.lstStatus.Items.Insert(0, DateTime.Now.ToString("HH:mm:ss.fff   ") + "[" + api.data.@event.id + "] " + strName + ".json");
            this.lstStatus.Items.Insert(0, DateTime.Now.ToString("HH:mm:ss.fff   ") + e.Argument.ToString());

        }
        private void bgwAMQPService_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                this.lstStatus.Invoke(new MethodInvoker(delegate
                {
                    if (e != null && e.Result != null)
                    {
                        this.lstStatus.Items.Insert(0, DateTime.Now.ToString("HH:mm:ss.fff   ") + e.Result.ToString());
                    }
                }));
            }
            catch (Exception exp)
            {
                Files.WriteError("bgwAMQPService_Completed(),error: " + exp.Message);
            }
        }

        private void lstStatus_SelectedValueChanged(object sender, EventArgs e)
        {
            if (lstStatus.Items.Count == 100)
            {
                for (int i = lstStatus.Items.Count; i < 3; i--)
                {
                    lstStatus.Items.RemoveAt(i);
                }
            }
        }
        private void DataOfScouts_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (bgwAMQPService.IsBusy && bgwAMQPService.WorkerSupportsCancellation == true)
            {
                bBreak = true;
                // Files.WriteLog(" Stop AMQPService..");
                bgwAMQPService.CancelAsync();
            }

            bgwAMQPService.Dispose();
        }

        private static System.Timers.Timer aTimer;
        private void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
            if (bBreak == true)
            {
                bBreak = false;
                Files.WriteLog(" Start AMQPService..");
                if (!bgwAMQPService.IsBusy)
                {
                    bgwAMQPService.RunWorkerAsync();
                }
            }

            Files.WriteLog(" Auto run sync HKJC data.");
            AutoRunSync();
        }

        private static System.Threading.Timer tTimer;
        private void TimerTask(object timerState)
        {
            try
            {
                //Files.WriteLog(" Auto task get events," + (this.bnAreas.Items[19].Text ==""? DateTime.Now.AddDays(AppFlag.iQueryDays).ToString("yyyy-MM-dd") : this.bnAreas.Items[19].Text) + " 00:00:00" + "-" + (this.bnAreas.Items[19].Text == "" ? DateTime.Now.AddDays(AppFlag.iQueryDays).ToString("yyyy-MM-dd") : this.bnAreas.Items[19].Text) + " 23:59:59");
                //InsertData("events", true, (this.bnAreas.Items[19].Text == "" ? DateTime.Now.AddDays(AppFlag.iQueryDays).ToString("yyyy-MM-dd") : this.bnAreas.Items[19].Text) + " 00:00:00", (this.bnAreas.Items[19].Text == "" ? DateTime.Now.AddDays(AppFlag.iQueryDays).ToString("yyyy-MM-dd") : this.bnAreas.Items[19].Text) + " 23:59:59");
                //Files.WriteLog(" Auto task get events," + this.bnAreas.Items[19].Text+ " 00:00:00" + "-" +  this.bnAreas.Items[19].Text  + " 23:59:59");
                //InsertData("events", true,   this.bnAreas.Items[19].Text + " 00:00:00",   this.bnAreas.Items[19].Text+ " 23:59:59");
                Files.WriteLog(" Auto task get events," + DateTime.Now.AddDays(AppFlag.iQueryDays).ToString("yyyy-MM-dd") + " 00:00:00" + "-" + DateTime.Now.AddDays(AppFlag.iQueryDays).ToString("yyyy-MM-dd") + " 23:59:59");

                ClientAuthorize();
                InsertData("events", true, DateTime.Now.AddDays(AppFlag.iQueryDays).ToString("yyyy-MM-dd") + " 00:00:00", DateTime.Now.AddDays(AppFlag.iQueryDays).ToString("yyyy-MM-dd") + " 23:59:59");
                 InsertData("standings", true);

                ///    AutoRunSync();
                //var state = timerState as TimerState;
            }
            catch (Exception exp)
            {
                Files.WriteError("TimerTask(),error: " + exp.Message);
            }      //Interlocked.Increment(ref state.Counter);
        }
        class TimerState
        {
            public int Counter;
            //   public bool Result;
        }
        private void bgwAMQPService_DoWork(object sender, DoWorkEventArgs e)
        {
            InitialListener();
            e.Result = "AMQP Done.";
            //  this.lstStatus.Items.Insert(0, DateTime.Now.ToString("HH:mm:ss.fff   " +"AMQP Done." ));
        }

        private void bgwAMQPService_DoWorkOLD(object sender, DoWorkEventArgs e)
        // private async void bgwAMQPService_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                ////bgwAMQPService.WorkerSupportsCancellation = true;
                ////backgroundWorker.DoWork += new DoWorkEventHandler(backgroundWorker_DoWork);
                string serverAddress = "amqp://queue.statscore.com:5672/";
                Uri uri = new Uri(serverAddress);
                ConnectionFactory cf = new ConnectionFactory();
                cf.Uri = uri;
                cf.HostName = "queue.statscore.com";
                cf.Port = 5672;
                cf.UserName = "telecom-digital-data-limited";
                cf.Password = "Eb76sQDkn9oZEki5L9QreKpaPmD3GbuuW2I";
                cf.VirtualHost = "statscore";
                cf.RequestedHeartbeat = 60;
                using (IConnection conn = cf.CreateConnection())
                {
                    using (IModel channel = conn.CreateModel())
                    {
                        //在MQ上定义一个持久化队列，如果名称相同不会重复创建
                        //  channel.QueueDeclare("telecom-digital-data-limited", true, false, false, null);

                        ////输入1，那如果接收一个消息，但是没有应答，则客户端不会收到下一个消息
#pragma warning disable CS0219 // The variable 'ack' is assigned but its value is never used
                        channel.BasicQos(0, 3, false);
#pragma warning restore CS0219 // The variable 'ack' is assigned but its value is never used
                        bool ack = false;
                        using (FbConnection connection = new FbConnection(AppFlag.ScoutsDBConn))
                        {
                            connection.Open();
#pragma warning disable CS0618 // 'QueueingBasicConsumer' is obsolete: 'Deprecated. Use EventingBasicConsumer or a different consumer interface implementation instead'
                            var consumer = new QueueingBasicConsumer(channel);
#pragma warning restore CS0618 // 'QueueingBasicConsumer' is obsolete: 'Deprecated. Use EventingBasicConsumer or a different consumer interface implementation instead'
                            //// channel.BasicConsume("telecom-digital-data-limited", true, consumer); disable autoack 20190111
                            var consumerTag = channel.BasicConsume("telecom-digital-data-limited", false, consumer);
                            if (AppFlag.TestMode) Files.WriteTestLog("Queue", " Listening--------" + consumerTag);
                            QueueDeclareOk result = channel.QueueDeclarePassive("telecom-digital-data-limited");
                            uint countA = result != null ? result.MessageCount : 0;
                            if (AppFlag.TestMode) Files.WriteTestLog("Queue", iCount3.ToString() + "  [" + channel.MessageCount("telecom-digital-data-limited") + "/" + countA + "]  ID -a ");

                            while (true)
                            {
                                string strName = "";
                                try
                                {
                                    //  result = channel.QueueDeclarePassive("telecom-digital-data-limited");
                                    //  countA = result != null ? result.MessageCount : 0;
                                    //if (AppFlag.TestMode) Files.WriteTestLog("Queue", iCount3.ToString() + "  [" + channel.MessageCount("telecom-digital-data-limited") + "/" + countA + "]  ID -b ");


                                    //// if (!consumer.Queue.Any()) { Files.WriteLog("no message"); continue; }////20190111 ////1142019
                                    if (!consumer.Queue.Any())
                                    {
                                        Thread.Sleep(100); continue;
                                    }
                                    else
                                    {
                                        //QueueDeclareOk result = channel.QueueDeclarePassive("telecom-digital-data-limited");
                                        //uint countA = result != null ? result.MessageCount : 0;
                                        //if (AppFlag.TestMode) Files.WriteTestLog("Queue", iCount3.ToString() + "  [" + channel.MessageCount("telecom-digital-data-limited") + "/" + countA + "]");

                                    }
                                    ////if (AppFlag.TestMode) Files.WriteTestLog("Queue", " Listening...1");
                                    var ea = (BasicDeliverEventArgs)consumer.Queue.Dequeue();
                                    ////  result = channel.QueueDeclarePassive("telecom-digital-data-limited");
                                    ////countA = result != null ? result.MessageCount : 0;
                                    ////if (AppFlag.TestMode) Files.WriteTestLog("Queue", iCount3.ToString() + "  [" + channel.MessageCount("telecom-digital-data-limited") +"//"+countA+ "]  ID -c ");
                                    ////  if (AppFlag.TestMode) Files.WriteTestLog("Queue", " Listening...2");
                                    iCount3++;
                                    if (ea == null)
                                    {
                                        Files.WriteError("ea == null");
                                        break;
                                    } ////20190111
                                    var body = ea.Body;
                                    var message = Encoding.UTF8.GetString(body);
                                    if (AppFlag.TestMode) Files.WriteTestLog("msg", message);
                                    if (AppFlag.TestMode) Files.WriteTestLog("Queue", iCount3.ToString() + "          RECEIVED  " + ea.DeliveryTag + "   " + message.Substring(0, 20));
                                    ///  manaul  ack 20190111
                                    channel.BasicAck(ea.DeliveryTag, false);
                                    if (AppFlag.TestMode) Files.WriteTestLog("Queue", iCount3.ToString() + "           SEND   " + ea.DeliveryTag);
                                    //if (!ack) {
                                    //    channel.BasicQos(0, 1, false);
                                    //    ack = true;
                                    //    if (AppFlag.TestMode) Files.WriteTestLog("Queue", iCount3.ToString() + " -----------Set ACK "  );
                                    //}
                                    ////result = channel.QueueDeclarePassive("telecom-digital-data-limited");
                                    ////countA = result != null ? result.MessageCount : 0;
                                    ////if (AppFlag.TestMode) Files.WriteTestLog("Queue", iCount3.ToString() + "  [" + channel.MessageCount("telecom-digital-data-limited") + "///" + countA + "]  ID -d ");


                                    DOSEventJson.EventJson api = JsonUtil.Deserialize(typeof(DOSEventJson.EventJson), message) as DOSEventJson.EventJson;

                                    ////if (api != null)
                                    ////{
                                    ////    if (AppFlag.TestMode) Files.WriteTestLog("Queue", iCount3.ToString() + " api " + api.id);
                                    ////}
                                    ////else
                                    ////{
                                    ////    if (AppFlag.TestMode) Files.WriteTestLog("Queue", iCount3.ToString() + " api -2");
                                    ////}
                                    ////if (AppFlag.TestMode) Files.WriteTestLog("Queue", "+++++++++++++++++++++++");
                                    if (api != null && api.type == "event" && (AppFlag.JsonType ? api.data.@event.sport_id == 5 : api.data.@event.sport_id != -1))
                                    {
                                        iCount++;
                                        ////   if (AppFlag.TestMode) Files.WriteTestLog("Queue", iCount.ToString() + " Event " + api.id);

                                        int id = -2;
                                        strName = api.data.@event.id + "_" + (api.data.@event.status_id) + "_" + DateTime.Now.ToString("HHmmssfff");
                                        Files.WriteJson(strName, message);
                                        using (FbCommand cmd2 = new FbCommand())
                                        {
                                            cmd2.CommandText = "PR_JSON_event";
                                            cmd2.CommandType = CommandType.StoredProcedure;
                                            cmd2.Connection = connection;
                                            cmd2.Parameters.Add("@ID", api.data.@event.id);
                                            cmd2.Parameters.Add("@NAME", api.data.@event.name);
                                            cmd2.Parameters.Add("@HOME_ID", api.data.@event.participants.Count() > 0 ? api.data.@event.participants[0].id : -1);
                                            cmd2.Parameters.Add("@GUEST_ID", api.data.@event.participants.Count() > 0 ? api.data.@event.participants[1].id : -1);
                                            // cmd2.Parameters.Add("@SOURCE", api.data.@evenT.S);
                                            // cmd2.Parameters.Add("@SOURCE_DC", api.data.@event.S);
                                            //cmd2.Parameters.Add("@SOURCE_SUPER", api.data.@event.s);
                                            cmd2.Parameters.Add("@RELATION_STATUS", api.data.@event.relation_status);
                                            cmd2.Parameters.Add("@START_DATE", Convert.ToDateTime(api.data.@event.start_date).AddHours(8));
                                            cmd2.Parameters.Add("@FT_ONLY", api.data.@event.ft_only == "yes" ? true : false);
                                            cmd2.Parameters.Add("@COVERAGE_TYPE", api.data.@event.coverage_type);
                                            //cmd2.Parameters.Add("@CHANNEL_ID", api.data.@event.CH);
                                            //cmd2.Parameters.Add("@CHANNEL_NAME", api.data.@event.C);
                                            cmd2.Parameters.Add("@SCOUTSFEED", api.data.@event.scoutsfeed == "yes" ? true : false);
                                            cmd2.Parameters.Add("@STATUS_ID", api.data.@event.status_id);
                                            //cmd2.Parameters.Add("@STATUS_NAME", api.data.@event.STA);
                                            cmd2.Parameters.Add("@STATUS_TYPE", api.data.@event.status_type);
                                            cmd2.Parameters.Add("@CDAY", api.data.@event.day);
                                            cmd2.Parameters.Add("@CLOCK_TIME", api.data.@event.clock_time);
                                            cmd2.Parameters.Add("@CLOCK_STATUS", api.data.@event.clock_status);
                                            // cmd2.Parameters.Add("@WINNER_ID", api.data.@event.W);
                                            //cmd2.Parameters.Add("@PROGRESS_ID", api.data.@event.PR);
                                            cmd2.Parameters.Add("@BET_STATUS", api.data.@event.bet_status);
                                            cmd2.Parameters.Add("@NEUTRAL_VENUE", api.data.@event.neutral_venue == "yes" ? true : false);
                                            cmd2.Parameters.Add("@ITEM_STATUS", api.data.@event.item_status);
                                            // cmd2.Parameters.Add("@UT", api.data.@event.U);
                                            // cmd2.Parameters.Add("@OLD_EVENT_ID", api.data.@event.OL);
                                            // cmd2.Parameters.Add("@SLUG", api.data.@event.S);
                                            // cmd2.Parameters.Add("@VERIFIED_RESULT", api.data.@event.VE);
                                            // cmd2.Parameters.Add("@IS_PROTOCOL_VERIFIED", api.data.@event.IS);
                                            //  cmd2.Parameters.Add("@PROTOCOL_VERIFIED_BY", api.data.@event.PRO);
                                            //cmd2.Parameters.Add("@PROTOCOL_VERIFIED_AT", api.data.@event.PRO);
                                            cmd2.Parameters.Add("@ROUND_ID", api.data.@event.round_id);
                                            cmd2.Parameters.Add("@ROUND_NAME", api.data.@event.round_name);
                                            //cmd2.Parameters.Add("@CLIENT_EVENT_ID", api.data.@event.C);
                                            // cmd2.Parameters.Add("@BOOKED", null);
                                            // cmd2.Parameters.Add("@BOOKED_BY", api.data.@event.);
                                            // cmd2.Parameters.Add("@INVERTED_PARTICIPANTS", api.data.@event.iv);
                                            cmd2.Parameters.Add("@VENUE_ID", api.data.@event.tour_id == null ? "-1" : api.data.@event.tour_id);
                                            //  cmd2.Parameters.Add("@GROUP_ID", api.data.@event.gr);
                                            cmd2.Parameters.Add("@STAGE_ID", api.data.@event.stage_id);
                                            cmd2.Parameters.Add("@SEASON_ID", api.data.@event.season_id);
                                            cmd2.Parameters.Add("@COMPETITION_ID", api.data.@event.competition_id);
                                            cmd2.Parameters.Add("@AREA_ID", api.data.@event.area_id);
                                            cmd2.Parameters.Add("@CTIMESTAMP", DateTime.Now);
                                            cmd2.Parameters.Add("@CACTION", api.data.@event.action);
                                            id = Convert.ToInt32(cmd2.ExecuteScalar());
                                            Files.WriteLog((id == 0 ? " [Success] Insert event " : id == 1 ? " Update event " : "") + "[" + api.data.@event.id + "]," + strName + ".json");
                                            // Files.WriteLog((id == 0 ? " [Success] Insert event [" + api.data.@event.id + "]," + strName + ".json":"");
                                        }


                                        if (api.data.@event.details.Count() > 0)
                                        {
                                            using (FbCommand cmd2 = new FbCommand())
                                            {
                                                cmd2.CommandText = "PR_event_details";
                                                cmd2.CommandType = CommandType.StoredProcedure;
                                                cmd2.Connection = connection;
                                                cmd2.Parameters.Add("@EVENTID", api.data.@event.id);
                                                cmd2.Parameters.Add("@WC_8", api.data.@event.details.FirstOrDefault(c => c.id == 8).value);
                                                cmd2.Parameters.Add("@PC_36", api.data.@event.details.FirstOrDefault(c => c.id == 36).value);
                                                cmd2.Parameters.Add("@PL_16", api.data.@event.details.FirstOrDefault(c => c.id == 16).value);
                                                cmd2.Parameters.Add("@EPL_50", api.data.@event.details.FirstOrDefault(c => c.id == 50).value);
                                                cmd2.Parameters.Add("@NOP_17", api.data.@event.details.FirstOrDefault(c => c.id == 17).value);
                                                cmd2.Parameters.Add("@EPTC_58", api.data.@event.details.FirstOrDefault(c => c.id == 58).value);
                                                cmd2.Parameters.Add("@IT_151", api.data.@event.details.FirstOrDefault(c => c.id == 151).value);
                                                cmd2.Parameters.Add("@ATT_141", api.data.@event.details.FirstOrDefault(c => c.id == 141).value);
                                                cmd2.Parameters.Add("@FHSD_19", api.data.@event.details.FirstOrDefault(c => c.id == 19).value == null ? DateTime.MinValue : Convert.ToDateTime(api.data.@event.details.FirstOrDefault(c => c.id == 19).value).AddHours(8));
                                                cmd2.Parameters.Add("@SHSD_20", api.data.@event.details.FirstOrDefault(c => c.id == 20).value == null ? DateTime.MinValue : Convert.ToDateTime(api.data.@event.details.FirstOrDefault(c => c.id == 20).value).AddHours(8));
                                                cmd2.Parameters.Add("@FEHSD_44", api.data.@event.details.FirstOrDefault(c => c.id == 44).value);
                                                cmd2.Parameters.Add("@SEHSD_45", api.data.@event.details.FirstOrDefault(c => c.id == 45).value);
                                                cmd2.Parameters.Add("@PSSD_150", api.data.@event.details.FirstOrDefault(c => c.id == 150).value);
                                                cmd2.Parameters.Add("@FHIT_201", api.data.@event.details.FirstOrDefault(c => c.id == 201).value);
                                                cmd2.Parameters.Add("@SHIT_202", api.data.@event.details.FirstOrDefault(c => c.id == 202).value);
                                                cmd2.Parameters.Add("@FEHIT_203", api.data.@event.details.FirstOrDefault(c => c.id == 203).value);
                                                cmd2.Parameters.Add("@SEHIT_204", api.data.@event.details.FirstOrDefault(c => c.id == 204).value);
                                                cmd2.Parameters.Add("@HL_205", api.data.@event.details.FirstOrDefault(c => c.id == 205).value);
                                                cmd2.Parameters.Add("@TD_124", api.data.@event.details.FirstOrDefault(c => c.id == 124).value);
                                                cmd2.Parameters.Add("@BM_160", api.data.@event.details.FirstOrDefault(c => c.id == 160).value);
                                                cmd2.Parameters.Add("@HF_178", api.data.@event.details.FirstOrDefault(c => c.id == 178).value);
                                                cmd2.Parameters.Add("@UT", api.ut);
                                                cmd2.Parameters.Add("@CTIMESTAMP", DateTime.Now);
                                                id = Convert.ToInt32(cmd2.ExecuteScalar());
                                                Files.WriteLog((id > 0 ? " [Success] Insert event_details " : id == -1 ? " Old data " : " [Failure] Insert event_details ") + "[" + api.data.@event.id + "]," + strName + ".json");
                                            }

                                            if (id != -1)
                                            {
                                                for (int i = 0; i < api.data.@event.participants.Length; i++)
                                                {
                                                    using (FbCommand cmd2 = new FbCommand())
                                                    {
                                                        cmd2.CommandText = "PR_participant_results";
                                                        cmd2.CommandType = CommandType.StoredProcedure;
                                                        cmd2.Connection = connection;
                                                        // cmd2.Parameters.Add("@ID", 0);
                                                        cmd2.Parameters.Add("@EVENTID", api.data.@event.id);
                                                        cmd2.Parameters.Add("@PARTICIPANTID", api.data.@event.participants[i].id);
                                                        cmd2.Parameters.Add("@PROGRESS_412", api.data.@event.participants[i].results.FirstOrDefault(c => c.id == 412).value);
                                                        cmd2.Parameters.Add("@WINNER_411", api.data.@event.participants[i].results.FirstOrDefault(c => c.id == 411).value);
                                                        cmd2.Parameters.Add("@RESULT_2", api.data.@event.participants[i].results.FirstOrDefault(c => c.id == 2).value);
                                                        cmd2.Parameters.Add("@RT_3", api.data.@event.participants[i].results.FirstOrDefault(c => c.id == 3).value);
                                                        cmd2.Parameters.Add("@FH_4", api.data.@event.participants[i].results.FirstOrDefault(c => c.id == 4).value);
                                                        cmd2.Parameters.Add("@SH_5", api.data.@event.participants[i].results.FirstOrDefault(c => c.id == 5).value);
                                                        cmd2.Parameters.Add("@E1H_133", api.data.@event.participants[i].results.FirstOrDefault(c => c.id == 133).value);
                                                        cmd2.Parameters.Add("@E2H_134", api.data.@event.participants[i].results.FirstOrDefault(c => c.id == 134).value);
                                                        cmd2.Parameters.Add("@PENALTY_7", api.data.@event.participants[i].results.FirstOrDefault(c => c.id == 7).value);
                                                        cmd2.Parameters.Add("@OVERTIME_104", api.data.@event.participants[i].results.FirstOrDefault(c => c.id == 104).value);
                                                        cmd2.Parameters.Add("@CTIMESTAMP", DateTime.Now);
                                                        cmd2.Parameters.Add("@CACTION", api.data.@event.action);
                                                        cmd2.Parameters.Add("@TEAMTYPE", api.data.@event.participants[i].counter == 1 ? "H" : "G");
                                                        id = Convert.ToInt32(cmd2.ExecuteScalar());
                                                        Files.WriteLog((id > 0 ? " [Success] Insert participant_results" : " [Failure] Insert participant_results") + "[" + api.data.@event.id + "/" + api.data.@event.participants[0].id + "]");
                                                    }

                                                    using (FbCommand cmd2 = new FbCommand())
                                                    {
                                                        cmd2.CommandText = "PR_participant_stats";
                                                        cmd2.CommandType = CommandType.StoredProcedure;
                                                        cmd2.Connection = connection;
                                                        // cmd2.Parameters.Add("@ID", 0);
                                                        cmd2.Parameters.Add("@EVENTID", api.data.@event.id);
                                                        cmd2.Parameters.Add("@PARTICIPANTID", api.data.@event.participants[i].id);
                                                        cmd2.Parameters.Add("@SOT_20", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 20).value);
                                                        cmd2.Parameters.Add("@SOT_21", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 21).value);
                                                        cmd2.Parameters.Add("@ATTACKS_10", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 10).value);
                                                        cmd2.Parameters.Add("@DA_11", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 11).value);
                                                        cmd2.Parameters.Add("@CORNERS_13", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 13).value);
                                                        cmd2.Parameters.Add("@YELLOW_CARDS_8", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 8).value);
                                                        cmd2.Parameters.Add("@RED_CARDS_9", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 9).value);
                                                        cmd2.Parameters.Add("@TOTAL_SHOTS_19", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 19).value);
                                                        cmd2.Parameters.Add("@FOULS_22", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 22).value);
                                                        cmd2.Parameters.Add("@OFFSIDES_24", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 24).value);
                                                        cmd2.Parameters.Add("@PS_14", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 14).value);
                                                        cmd2.Parameters.Add("@PM_15", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 15).value);
                                                        cmd2.Parameters.Add("@PG_16", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 16).value);
                                                        cmd2.Parameters.Add("@FK_25", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 25).value);
                                                        cmd2.Parameters.Add("@DFK_26", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 26).value);
                                                        cmd2.Parameters.Add("@FKG_18", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 18).value);
                                                        cmd2.Parameters.Add("@SW_27", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 27).value);
                                                        cmd2.Parameters.Add("@SB_28", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 28).value);
                                                        cmd2.Parameters.Add("@GS_29", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 29).value);
                                                        cmd2.Parameters.Add("@GK_30", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 30).value);
                                                        cmd2.Parameters.Add("@TI_32", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 32).value);
                                                        cmd2.Parameters.Add("@SUBSTITUTIONS_31", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 31).value);
                                                        cmd2.Parameters.Add("@GOALS_40", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 40).value);
                                                        cmd2.Parameters.Add("@MP_34", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 34).value);
                                                        cmd2.Parameters.Add("@OWN_GOALS_17", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 17).value);
                                                        cmd2.Parameters.Add("@ADW_33", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 33).value);
                                                        cmd2.Parameters.Add("@FORM_716", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 716).value);
                                                        cmd2.Parameters.Add("@SKIN_718", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 718).value);
                                                        cmd2.Parameters.Add("@PS_639", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 639).value);
                                                        cmd2.Parameters.Add("@PU_697", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 697).value);
                                                        cmd2.Parameters.Add("@GOALS115_772", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 772).value);
                                                        cmd2.Parameters.Add("@GOALS1630_773", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 773).value);
                                                        cmd2.Parameters.Add("@GOALS3145_774", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 774).value);
                                                        cmd2.Parameters.Add("@GOALS4660_775", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 775).value);
                                                        cmd2.Parameters.Add("@GOALS6175_776", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 776).value);
                                                        cmd2.Parameters.Add("@GOALS7690_777", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 777).value);
                                                        cmd2.Parameters.Add("@MPG_778", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 778).value);
                                                        cmd2.Parameters.Add("@MPS_779", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 779).value);
                                                        cmd2.Parameters.Add("@CTIMESTAMP", DateTime.Now);
                                                        cmd2.Parameters.Add("@CACTION", api.data.@event.action);
                                                        cmd2.Parameters.Add("@TEAMTYPE", api.data.@event.participants[i].counter == 2 ? "G" : "H");
                                                        id = Convert.ToInt32(cmd2.ExecuteScalar());
                                                        Files.WriteLog((id > 0 ? " [Success] Insert participant_stats" : " [Failure] Insert participant_results") + "[" + api.data.@event.id + "/" + api.data.@event.participants[0].id + "]");
                                                        //  Thread.Sleep(20);
                                                    }

                                                    //using (FbCommand cmd2 = new FbCommand())
                                                    //{

                                                    //    cmd2.CommandText = "PR_stats_GoalInfo";
                                                    //    cmd2.CommandType = CommandType.StoredProcedure;
                                                    //    cmd2.Connection = connection; 
                                                    //    cmd2.Parameters.Add("@EMATCHID", api.data.@event.id); 
                                                    //    cmd2.Parameters.Add("@YELLOW_CARDS_8", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 8).value);
                                                    //    cmd2.Parameters.Add("@RED_CARDS_9", api.data.@event.participants[i].stats.FirstOrDefault(c => c.id == 9).value);
                                                    //    cmd2.Parameters.Add("@CTIMESTAMP", DateTime.Now);
                                                    //    id = Convert.ToInt32(cmd2.ExecuteScalar());
                                                    //    Files.WriteLog((id > 0 ? " [Success] Update GoalInfo " : " [Failure]  Update GoalInfo ") + "[" + api.data.@event.id + "/" + api.data.@event.participants[0].id + "]");
                                                    //    Thread.Sleep(20);
                                                    //}
                                                }
                                                if (api.data.@event.participants.Length == 2 && api.data.@event.action != "delete")
                                                {

                                                    using (FbCommand cmd2 = new FbCommand())
                                                    {
                                                        cmd2.CommandText = "PR_Stats_Result_GoalInfo";
                                                        cmd2.CommandType = CommandType.StoredProcedure;
                                                        cmd2.Connection = connection;
                                                        cmd2.Parameters.Add("@EMATCHID", api.data.@event.id);
                                                        cmd2.Parameters.Add("@H_YELLOW", api.data.@event.participants[0].stats.FirstOrDefault(c => c.id == 8).value);
                                                        cmd2.Parameters.Add("@H_RED", api.data.@event.participants[0].stats.FirstOrDefault(c => c.id == 9).value);
                                                        cmd2.Parameters.Add("@G_YELLOW", api.data.@event.participants[1].stats.FirstOrDefault(c => c.id == 8).value);
                                                        cmd2.Parameters.Add("@G_RED", api.data.@event.participants[1].stats.FirstOrDefault(c => c.id == 9).value);
                                                        cmd2.Parameters.Add("@H_GOAL", api.data.@event.participants[0].results.FirstOrDefault(c => c.id == 2).value);
                                                        cmd2.Parameters.Add("@G_GOAL", api.data.@event.participants[1].results.FirstOrDefault(c => c.id == 2).value);
                                                        cmd2.Parameters.Add("@LASTTIME", DateTime.Now);
                                                        id = Convert.ToInt32(cmd2.ExecuteScalar());
                                                        // Files.WriteLog((id > 0 ? " [Success] Update GoalInfo " : " [Failure]  Update GoalInfo ") + "[" + api.data.@event.id + "/" + api.data.@event.participants[0].id + "]" + id);
                                                        if (id > -1) Files.WriteLog(" [Success] Update GoalInfo " + "[" + api.data.@event.id + "/" + api.data.@event.participants[0].id + "]" + id);
                                                    }

                                                    //using (FbCommand cmd2 = new FbCommand())
                                                    //{
                                                    //    cmd2.CommandText = "PR_stats_GoalInfo";
                                                    //    cmd2.CommandType = CommandType.StoredProcedure;
                                                    //    cmd2.Connection = connection;
                                                    //    cmd2.Parameters.Add("@EMATCHID", api.data.@event.id);
                                                    //    cmd2.Parameters.Add("@H_YELLOW", api.data.@event.participants[0].stats.FirstOrDefault(c => c.id == 8).value);
                                                    //    cmd2.Parameters.Add("@H_RED", api.data.@event.participants[0].stats.FirstOrDefault(c => c.id == 9).value);
                                                    //    cmd2.Parameters.Add("@G_YELLOW", api.data.@event.participants[1].stats.FirstOrDefault(c => c.id == 8).value);
                                                    //    cmd2.Parameters.Add("@G_RED", api.data.@event.participants[1].stats.FirstOrDefault(c => c.id == 9).value);
                                                    //    cmd2.Parameters.Add("@LASTTIME", DateTime.Now);
                                                    //    id = Convert.ToInt32(cmd2.ExecuteScalar());
                                                    //    Files.WriteLog((id > 0 ? " [Success] Update GoalInfo " : " [Failure]  Update GoalInfo ") + "[" + api.data.@event.id + "/" + api.data.@event.participants[0].id + "]");
                                                    //}

                                                    //using (FbCommand cmd2 = new FbCommand())
                                                    //{
                                                    //    cmd2.CommandText = "PR_Result_GoalInfo";
                                                    //    cmd2.CommandType = CommandType.StoredProcedure;
                                                    //    cmd2.Connection = connection;
                                                    //    cmd2.Parameters.Add("@EMATCHID", api.data.@event.id);
                                                    //    cmd2.Parameters.Add("@H_GOAL", api.data.@event.participants[0].results.FirstOrDefault(c => c.id == 2).value);
                                                    //    cmd2.Parameters.Add("@G_GOAL", api.data.@event.participants[1].results.FirstOrDefault(c => c.id == 2).value);
                                                    //    cmd2.Parameters.Add("@LASTTIME", DateTime.Now);
                                                    //    id = Convert.ToInt32(cmd2.ExecuteScalar());
                                                    //    Files.WriteLog((id > -1 ? " [Success] Update GoalInfo Goal " : " [Failure]  Update GoalInfo Goal") + "[" + api.data.@event.id + "/" + api.data.@event.participants[0].id + "]" + id);
                                                    //}

                                                    if (id > 0)
                                                    {
                                                        SendAlertMsg(AppFlag.LIVEGOALS);
                                                    }

                                                    if (!backgroundWorker.IsBusy && id > 0)
                                                    {
                                                        //  backgroundWorker.RunWorkerAsync("[" + api.data.@event.id + "] "+ api.data.@event.name + " " + strName + ".json");
                                                        backgroundWorker.RunWorkerAsync(api.data.@event.name + " " + strName + ".json");
                                                    }
                                                }
                                            }
                                        }

                                        if (api.data.@event.status_id == 9 || api.data.@event.status_id == 11)
                                        {
                                            if (api.data.@event.id > 0)
                                            {
                                                /// await AyncHandleData("events.show3", true, api.data.@event.id.ToString());
                                                InsertData("events.show3", true, api.data.@event.id.ToString());
                                                Files.WriteLog(" Housekeep [" + api.data.@event.id.ToString() + "].." + api.data.@event.status_id);
                                            }
                                        }
                                        //    connection.Close();
                                        //}

                                        ////if (!backgroundWorker.IsBusy&& id > 0)
                                        ////{
                                        ////    backgroundWorker.RunWorkerAsync("[" + api.data.@event.id + "] " + strName + ".json");
                                        ////}
                                    }
                                    //else if (api != null)
                                    //{
                                    //    //strName = "other_" + DateTime.Now.ToString("HHmmssfff");
                                    //    //Files.WriteJson("other_" + DateTime.Now.ToString("HHmm"), message);
                                    //}
                                    //}
                                    // else if (message.IndexOf("\"type\": \"incident\"") > 0)
                                    else if (api != null && api.type == "incident")
                                    {
                                        int sID = -1;
                                        DOSIncidentJson.IncidentJson incidentJson = JsonUtil.Deserialize(typeof(DOSIncidentJson.IncidentJson), message) as DOSIncidentJson.IncidentJson;
                                        iCount2++;
                                        ////if (AppFlag.TestMode) Files.WriteTestLog("Queue", iCount2.ToString() + " Incid " + api.id);

                                        if (incidentJson != null && incidentJson.data.@event.sport_id == 5 && incidentJson.data.incident.important_for_trader == "yes")
                                        {
                                            //  strName = "Incid_" + incidentJson.data.@event.id +"_"+ incidentJson.data.incident.event_status_id + "_"+ (incidentJson.data.incident.incident_id == 413 || incidentJson.data.incident.incident_id == 418 || incidentJson.data.incident.incident_id == 419 ? "_" + incidentJson.data.incident.incident_id : "") + incidentJson.data.incident.id + "-" + DateTime.Now.ToString("HHmmssfff");
                                            strName = "Incid_" + incidentJson.data.@event.id + "_" + incidentJson.data.incident.event_status_id + "_" + incidentJson.data.incident.incident_id + "-" + incidentJson.data.incident.id + "-" + DateTime.Now.ToString("HHmmssfff");
                                            Files.WriteJson(strName, message);
                                            //if(incidentJson.data.incident.incident_id== 429)
                                            //{ 
                                            //}
                                            //using (FbConnection connection = new FbConnection(AppFlag.ScoutsDBConn))
                                            //{
                                            //connection.Open();
                                            using (FbCommand cmd2 = new FbCommand())
                                            {
                                                cmd2.CommandText = "PR_INCIDENTS";
                                                cmd2.CommandType = CommandType.StoredProcedure;
                                                cmd2.Connection = connection;
                                                cmd2.Parameters.Add("@ID", incidentJson.data.incident.id);
                                                cmd2.Parameters.Add("@EVENTID", incidentJson.data.@event.id);
                                                cmd2.Parameters.Add("@CACTION", incidentJson.data.incident.action);
                                                cmd2.Parameters.Add("@INCIDENT_ID", incidentJson.data.incident.incident_id);
                                                cmd2.Parameters.Add("@INCIDENT_NAME", incidentJson.data.incident.incident_name);
                                                cmd2.Parameters.Add("@PARTICIPANT_ID", incidentJson.data.incident.participant_id);
                                                cmd2.Parameters.Add("@PARTICIPANT_NAME", incidentJson.data.incident.participant_name);
                                                cmd2.Parameters.Add("@SUBPARTICIPANT_ID", incidentJson.data.incident.subparticipant_id);
                                                cmd2.Parameters.Add("@SUBPARTICIPANT_NAME", incidentJson.data.incident.subparticipant_name);
                                                cmd2.Parameters.Add("@IMPORTANT_FOR_TRADER", true);// incidentJson.data.incident.important_for_trader == "yes" ? true : false);
                                                cmd2.Parameters.Add("@EVENT_TIME", incidentJson.data.incident.event_time);
                                                cmd2.Parameters.Add("@EVENT_STATUS_ID", incidentJson.data.incident.event_status_id);
                                                cmd2.Parameters.Add("@EVENT_STATUS_NAME", incidentJson.data.incident.event_status_name);
                                                cmd2.Parameters.Add("@UT", incidentJson.ut);
                                                cmd2.Parameters.Add("@CTIMESTAMP", DateTime.Now);
                                                cmd2.Parameters.Add("@TEAMTYPE",
                                                (incidentJson.data.incident.participant_id == null || incidentJson.data.incident.participant_id.ToString() == "" ? "" : incidentJson.data.incident.participant_id.ToString() == incidentJson.data.@event.participants[0].id.ToString() ? "H" : incidentJson.data.incident.participant_id.ToString() == incidentJson.data.@event.participants[1].id.ToString() ? "G" : "H"));
                                                sID = Convert.ToInt32(cmd2.ExecuteScalar());
                                                Files.WriteLog((sID > -1 ? " [Success] Insert INCIDENTS " : " [Failure] Insert INCIDENTS ") + "[" + incidentJson.data.@event.id + "]," + strName + ".json");

                                                if (sID > 0) { SendAlertMsg(AppFlag.GOALDETAILS); }

                                                if (!backgroundWorker.IsBusy && sID > 0)
                                                {
                                                    // backgroundWorker.RunWorkerAsync("[" + incidentJson.data.@event.id + "] " + incidentJson.data.@event.name + " " + strName + ".json");
                                                    backgroundWorker.RunWorkerAsync(incidentJson.data.@event.name + " " + strName + ".json");
                                                }
                                            }

                                            if (incidentJson.data.incident.incident_id == 429 || incidentJson.data.@event.status_id == 9 || incidentJson.data.@event.status_id == 11)
                                            {
                                                if (incidentJson.data.@event.id > 0)
                                                {
                                                    //  await AyncHandleData("events.show3", true, incidentJson.data.@event.id.ToString());
                                                    InsertData("events.show3", true, incidentJson.data.@event.id.ToString());
                                                    Files.WriteLog(" Housekeep incident [" + incidentJson.data.@event.id.ToString() + "].");
                                                    if (incidentJson.data.incident.incident_id == 429)
                                                    {
                                                        InsertData("events.participants", true, incidentJson.data.@event.id.ToString());
                                                        Files.WriteLog(" Housekeep event participant [" + incidentJson.data.@event.id.ToString() + "]." + incidentJson.data.incident.incident_id);

                                                    }
                                                }
                                            }
                                            //    connection.Close();
                                            //}
                                            ////if (!backgroundWorker.IsBusy && sID > 0)
                                            ////{
                                            ////    backgroundWorker.RunWorkerAsync("[" + incidentJson.data.@event.id + "] " + strName + ".json");
                                            ////}
                                        }
                                        else if (incidentJson != null && incidentJson.data.@event.sport_id == 5 && incidentJson.data.incident.important_for_trader == "no")
                                        {
                                            if (!AppFlag.Important_for_trader)
                                            {
                                                strName = "Incid_" + incidentJson.data.@event.id + "_" + incidentJson.data.@event.id + "_" + DateTime.Now.ToString("HHmmssfff") + "_no";
                                                Files.WriteJson(strName, message);
                                            }
                                        }
                                        else
                                        {
                                            strName = "Incid_" + incidentJson.data.@event.id + "_" + incidentJson.data.@event.id + "_" + DateTime.Now.ToString("HHmmssfff") + "_other";
                                            Files.WriteJson(strName, message);
                                        }

                                    }
                                    else if (api != null && api.type == "event_keep_alive")
                                    {
                                        Files.WriteLog("event_keep_alive");
                                    }
                                    else if (api != null && api.data.@event.sport_id != 5)
                                    {
                                    }
                                    else
                                    {
                                        strName = "other2_" + DateTime.Now.ToString("HHmmssfff");
                                        Files.WriteJson(strName, message);
                                    }
                                    if (bBreak)
                                    {
                                        // Files.WriteLog("Stop AMQPService");
                                        break;
                                    }
                                    Thread.Sleep(TimeSpan.FromMilliseconds(2));
                                }
                                catch (EndOfStreamException endOfStreamException)
                                {
                                    Files.WriteLog("No message." + (strName != "" ? strName + ".json," : ""));
                                    Files.WriteError("bgwAMQPService_DoWork(while true)," + (strName != "" ? strName + ".json," : "") + "error: " + endOfStreamException);
                                    connection.Close();
                                    bBreak = true;
                                    e.Result = "No message,break.";
                                    break;
                                }
                                catch (Exception exp)
                                {
                                    if (strName != "") Files.WriteLog("Error: " + strName + ".json");
                                    Files.WriteError("bgwAMQPService_DoWork(while true),error: " + (strName != "" ? strName + ".json," : "") + "error: " + exp);
                                    e.Result = "No AMQPService.";
                                    continue;
                                }
                            }
                            connection.Close();
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                Files.WriteError("bgwAMQPService_DoWork(),error: " + exp);
                e.Result = "No AMQPService.";
                bBreak = true;
                ///  bgwAMQPService.RunWorkerAsync();
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
        private static HttpClient _httpClient;
        public string token;

        public OAuthClient()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(AppFlag.ScoutsUrl);
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

            if (type == "booked-events")
            {
                strUrl = $"/v2/booked-events.xml";
                parameters.Add("event_id", arr[0].ToString());
                parameters.Add("product", "scoutsfeed");
                parameters.Add("token", token);
                Files.WriteLog("Post " + strUrl + " " + token + " scoutsfeed event_id " + arr[0].ToString());
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
                return responseValue;
                // return string.Empty;
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
            else if (type == "events.show")
            {
                strUrl = $"/v2/events/" + arr[0].ToString() + ".xml?token=" + token + "&tz=Asia/Hong_Kong";// &page=" + type.Substring(type.IndexOf("/") + 1, type.Length - type.IndexOf("/") - 1);

                Files.WriteLog("GET events.show " + strUrl);
            }
            else if (type == "events.compare")
            {
                //https://api.statscore.com/v2/participants/136396/compare.xml?token=54a288d988a2ae0756e076e946bdf1b2&sport_id=5&compare_participant_id=136415
                strUrl = $"/v2/participants/" + arr[1].ToString() + "/compare.xml?token=" + token + "&&tz=Asia/Hong_Kong&sport_id=5&compare_participant_id=" + arr[2].ToString();
                Files.WriteLog("GET events.compare " + arr[0].ToString() + " " + strUrl);
            }
            else if (type.IndexOf("events") > -1)
            {
                // strUrl = $"/v2/events.xml?token=" + token + "&sport_id=5&tz=Asia/Hong_Kong&competition_id=" + arr[0] + "&page=" + type.Substring(type.IndexOf("/") + 1, type.Length - type.IndexOf("/") - 1);
                // "/v2/events.xml?token=&sport_id=5&tz=Asia/Hong_Kong&date_from=9/19/2018 00:00:00&date_to=10/11/2018 23:59:59&page=1"
                //if (Convert.ToBoolean(arr[0]) == true)
                //{
                strUrl = $"/v2/events.xml?token=" + token + "&sport_id=5&tz=Asia/Hong_Kong&date_from=" + Convert.ToDateTime(arr[0]).ToString("yyyy-MM-dd HH:mm:ss") + "&date_to=" + Convert.ToDateTime(arr[1]).ToString("yyyy-MM-dd HH:mm:ss") + "&page=" + type.Substring(type.IndexOf("/") + 1, type.Length - type.IndexOf("/") - 1);
                //}
                //else
                //{
                //    strUrl = $"/v2/events/" + arr[1].ToString() + ".xml?token=" + token + "&tz=Asia/Hong_Kong&page=" + type.Substring(type.IndexOf("/") + 1, type.Length - type.IndexOf("/") - 1);
                //}
                Files.WriteLog("GET events " + strUrl);
            }

            else if (type.IndexOf("participants") > -1)
            {
                if (Convert.ToBoolean(arr[0]) == true)
                {
                    strUrl = $"/v2/events/" + arr[1] + "/participants.xml?token=" + token + "&page=" + type.Substring(type.IndexOf("/") + 1, type.Length - type.IndexOf("/") - 1);
                }
                else
                {
                    strUrl = $"/v2/participants.xml?token=" + token + "&sport_id=5&season_id=" + arr[1] + "&page=" + type.Substring(type.IndexOf("/") + 1, type.Length - type.IndexOf("/") - 1);
                }
                Files.WriteLog("GET participants " + strUrl);
            }
            else if (type=="standings")
            {   //https://api.statscore.com/v2/standings.xml?token=2664ad198dce4b5007648f071cbb433b&sport_id=5&subtype=standings&type_id=6&client_id=262&%20object_type=competition&object_id=1610
                string typeid= (arr[0].ToString() == "TopSoccer")? "6" : (arr[0].ToString() == "Rank")?"1": ""; 
                strUrl = $"/v2/standings.xml?token=" + token + "&sport_id=5&subtype=standings&type_id="+ typeid + "&client_id=262&%20object_type=competition&object_id="+ arr[1].ToString();
                Files.WriteLog("GET standings " + strUrl);
            }
            else if (type == "standings.TopSorers")
            {
                strUrl = $"/v2/standings/"+ arr[0].ToString() + ".xml?token=" + token;
                Files.WriteLog("GET "+ arr[0].ToString() + " TopSoccer standings  " + strUrl);
            }
            else if (type == "standings.Rank")
            {
                strUrl = $"/v2/standings/" + arr[0].ToString() + ".xml?token=" + token;
                Files.WriteLog("GET " + arr[0].ToString() + " Rank standings  " + strUrl);
            }
            var response = await _httpClient.GetAsync(strUrl).ConfigureAwait(false);
            var responseValue = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            if (response.StatusCode == System.Net.HttpStatusCode.OK || response.StatusCode == System.Net.HttpStatusCode.Forbidden)
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