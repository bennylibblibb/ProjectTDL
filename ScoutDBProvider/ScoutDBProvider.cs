 
using FileLog;
using FirebirdSql.Data.FirebirdClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms; 

namespace ScoutDBProvider
{
    public partial class ScoutDBProvider : Form
    {

        [DllImport("user32.dll", EntryPoint = "FindWindow")]
        public static extern int FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        public static extern uint RegisterWindowMessage(string lpString);

        [DllImport("user32.dll")]
        public static extern IntPtr PostMessage(IntPtr hWnd, uint msg, int wParam, int lParam);

        private static readonly IntPtr HWND_BROADCAST = new IntPtr(0xFFFF);
        private const int SUCCESS_CODE = 100000;
         private static readonly uint uiSkSvrNotify = RegisterWindowMessage(AppFlag.SkSvrNotify);

        private static System.Threading.Timer tTimer;

        // private readonly int m_ExternID = RegisterWindowMessage(Directory.GetCurrentDirectory() + @"\ScoutDBProvider");
        private readonly uint m_ExternID = RegisterWindowMessage(AppFlag.SkSvrNotify);
        public ScoutDBProvider()
        {
            InitializeComponent();
            try
            {
                DateTime dueTimes = Convert.ToDateTime((DateTime.Now.Date.ToString("yyyy/MM/dd ") + AppFlag.GetTime));
                var timerState = new TimerState { Counter = 0 };
                tTimer = new System.Threading.Timer(
               callback: new TimerCallback(TimerTask),
               state: timerState,
               dueTime: new TimeSpan(1000),// (dueTimes < DateTime.Now ? dueTimes.AddDays(1).Subtract(DateTime.Now) : dueTimes.Subtract(DateTime.Now)),
               period: dueTimes.AddDays(1).Subtract(dueTimes));

            }
            catch (Exception exp)
            {
                Files.WriteError("ScoutDBProvider(),error: " + exp.Message);
            }
        }

        private static void TimerTask(object timerState)
        {
            try
            {
                int iIndex = 0;
                DataSet ds = new DataSet();
                string[] syncItems;
                ArrayList configSetting = new ArrayList();
                string queryString = "";

                configSetting = (ArrayList)ConfigurationManager.GetSection("SyncItems");
                syncItems = new string[configSetting.Count];
                if (configSetting != null)
                {
                    iIndex = 0;
                    foreach (string s in configSetting)
                    {
                        syncItems[iIndex] = s;
                        iIndex++;
                    }
                }
                configSetting.Clear();

                if (syncItems.Count() > 0)
                {
                    int i = 0;
                    try
                    {
                        using (FbConnection connection = new FbConnection(AppFlag.ScoutsDBConn))
                        {
                            connection.Open();
                            for (i = 0; i < syncItems.Length; i++)
                            {
                                Files.WriteLog("Update " + syncItems[i].ToString());
                                queryString = "SELECT  c.id, c.ALIAS FROM COMPETITIONS c where c.ALIAS is not null ";
                                using (FbCommand cmd = new FbCommand(queryString, connection))
                                {
                                    using (FbDataAdapter fda = new FbDataAdapter(cmd))
                                    {
                                        using (DataSet data = new DataSet())
                                        {
                                            data.Tables.Add(new DataTable("data"));
                                            fda.Fill(data.Tables["data"]);
                                            ds = data;
                                        }
                                    }
                                }
                                foreach (DataRow dr in ds.Tables[0].Rows)
                                {
                                    DataSet ds2 = new DataSet();
                                    if (i == 0)
                                    {
                                        queryString = "SELECT T.SHORT_NAME  , c.name, a.LEAG_ID, a.CLEAG_ALIAS, a.SEASON_ID, a.TEAM_ID, a.TEAM, a.HKJC_TEAM, a.SCORE, a.RANK, a.FLAG, a.GAMES, a.IWON, a.IDRAW,a.ILOST," +
                                            "a.CTIMESTAMP,T.HKJC_NAME_CN FROM LEAGRANKINFO a inner join SEASONS s on s.id = a.SEASON_ID and ( s.SYEAR = '2019' or s.SYEAR = '2018/19') " +
                                            "inner join teams t on t.id = a.team_id   inner join areas c on c.id = t.area_id where  a.LEAG_ID=" + dr["id"].ToString() + " order by a.CLEAG_ALIAS ,a.rank asc ";
                                    }
                                    else if (i == 1)
                                    {
                                        queryString = "SELECT  first 15  t.SHORT_NAME tname,t.HKJC_NAME_CN tcname ,r.CLEAG_ID, r.CLEAG_ALIAS, r.SEASON_ID,  r.PLAYER_ID,r.CPLAYER_NAME, r.CTEAM_ABBR, r.CACT," +
                                        " r.IRID, r.IRANK, r.IGOALS, r.UT, r.CTIMESTAMP ,t.short_name tname, p.CPLAYER_NAME pcname FROM SCORERS_INFO r " +
                                        " inner join teams t on t.id = r.TEAM_ID  left join  PLAYERS_INFO p  on p.PLAYER_ID = r.PLAYER_ID and p.TEAM_ID = r.TEAM_ID " +
                                        " where r.CLEAG_ID = " + dr["id"].ToString() + "  order by r.IRANK asc";
                                    }

                                    using (FbCommand cmd = new FbCommand(queryString, connection))
                                    {
                                        using (FbDataAdapter fda = new FbDataAdapter(cmd))
                                        {
                                            using (DataSet data = new DataSet())
                                            {
                                                data.Tables.Add(new DataTable("data"));
                                                fda.Fill(data.Tables["data"]);
                                                ds2 = data;
                                            }
                                        }
                                    }

                                    if (ds2.Tables[0].Rows.Count > 0)
                                    {
                                        using (FbConnection connection2 = new FbConnection(AppFlag.MangoDBConn))
                                        {
                                            connection2.Open();
                                            if (i == 0)
                                            {
                                                using (FbCommand cmd2 = new FbCommand())
                                                {
                                                    cmd2.CommandText = "delete from ranks where CLEAGUENAME='" + dr["ALIAS"] + "'";
                                                    cmd2.Connection = connection2;
                                                    int id = Convert.ToInt32(cmd2.ExecuteScalar());
                                                }
                                                Files.WriteLog("Delete ranks [" + dr["id"] + "] " + dr["ALIAS"]);

                                                queryString = "select first  1 * from RANKS";
                                                using (FbCommand cmd = new FbCommand(queryString, connection2))
                                                {
                                                    using (FbDataAdapter fda = new FbDataAdapter(cmd))
                                                    {
                                                        using (FbCommandBuilder fcb = new FbCommandBuilder(fda))
                                                        {
                                                            using (DataSet data = new DataSet())
                                                            {
                                                                data.Tables.Add(new DataTable("data"));
                                                                fda.Fill(data.Tables["data"]);
                                                                foreach (DataRow dr2 in ds2.Tables[0].Rows)
                                                                {
                                                                    if (dr2["SHORT_NAME"].ToString() == "Brisbane Roar")
                                                                    {
                                                                        string ST = "";
                                                                    }
                                                                    DataRow dr3 = data.Tables["data"].NewRow();
                                                                    dr3[0] = dr2["CLEAG_ALIAS"];
                                                                    dr3[1] = DateTime.Now.ToString("yyyy/MM/dd");
                                                                    dr3[2] = dr2["name"];
                                                                    dr3[3] = "U";
                                                                    dr3[4] = dr2["RANK"];
                                                                    dr3[5] = dr2["HKJC_NAME_CN"].Equals(DBNull.Value) ? dr2["TEAM"] : dr2["HKJC_NAME_CN"];
                                                                    dr3[6] = dr2["HKJC_NAME_CN"] == DBNull.Value ? dr2["TEAM"] : dr2["HKJC_NAME_CN"];
                                                                    dr3[7] = dr2["SCORE"];
                                                                    dr3[8] = "";
                                                                    dr3[9] = dr2["GAMES"];
                                                                    dr3[10] = dr2["IWON"] + "/" + dr2["IDRAW"] + "/" + dr2["ILOST"];
                                                                    dr3[11] = "1";
                                                                    dr3[12] = DateTime.Now;
                                                                    dr3[13] = "0";
                                                                    data.Tables["data"].Rows.Add(dr3);
                                                                }
                                                                fda.Update(data.Tables["data"]);
                                                                Files.WriteLog("Insert ranks [" + dr["id"] + "] " + dr["ALIAS"]);
                                                            }
                                                        }
                                                    }
                                                }

                                            }
                                            else if (i == 1)
                                            {
                                                using (FbCommand cmd2 = new FbCommand())
                                                {
                                                    cmd2.CommandText = "delete from SCORERS where CLEAGUENAME='" + dr["ALIAS"] + "'";
                                                    cmd2.Connection = connection2;
                                                    int id = Convert.ToInt32(cmd2.ExecuteScalar());
                                                }
                                                Files.WriteLog("Delete SCORERS [" + dr["id"] + "] " + dr["ALIAS"]);

                                                queryString = "select first  1 * from SCORERS";
                                                using (FbCommand cmd = new FbCommand(queryString, connection2))
                                                {
                                                    using (FbDataAdapter fda = new FbDataAdapter(cmd))
                                                    {
                                                        using (FbCommandBuilder fcb = new FbCommandBuilder(fda))
                                                        {
                                                            using (DataSet data = new DataSet())
                                                            {
                                                                data.Tables.Add(new DataTable("data"));
                                                                fda.Fill(data.Tables["data"]);
                                                                foreach (DataRow dr2 in ds2.Tables[0].Rows)
                                                                {
                                                                    DataRow dr3 = data.Tables["data"].NewRow();
                                                                    dr3[0] = dr2["CLEAG_ALIAS"];
                                                                    dr3[1] = DateTime.Now.ToString("yyyy/MM/dd");
                                                                    dr3[2] = dr2["IRANK"];
                                                                    dr3[3] = dr2["pcname"].Equals(DBNull.Value) || dr2["pcname"].Equals("") ? dr2["CPLAYER_NAME"] : dr2["pcname"];
                                                                    dr3[4] = dr2["tcname"] == DBNull.Value || dr2["tcname"].ToString() == "" ? dr2["tname"] : dr2["tcname"];
                                                                    dr3[5] = dr2["IGOALS"];
                                                                    dr3[6] = DateTime.Now;
                                                                    dr3[7] = "0";
                                                                    data.Tables["data"].Rows.Add(dr3);
                                                                }
                                                                fda.Update(data.Tables["data"]);
                                                                Files.WriteLog("Insert SCORERS [" + dr["id"] + "] " + dr["ALIAS"]);
                                                            }
                                                        }
                                                    }
                                                }
                                            }

                                            connection2.Close();
                                        }
                                    }

                                }
                            }
                            connection.Close();
                        }
                    }
                    catch (Exception exp)
                    {
                        Files.WriteError(DateTime.Now.ToString("HH:mm:ss") + syncItems[i] + ",error: " + exp);
                    }
                }

                Files.WriteLog(" TimerTask " + DateTime.Now);
                var state = timerState as TimerState;
                Interlocked.Increment(ref state.Counter);
            }
            catch (Exception exp)
            {
                Files.WriteError("TimerTask(),error: " + exp.Message);

            }
        }

        protected override void WndProc(ref Message m)
        {
            try
            {
                if (m_ExternID == m.Msg)
                {
                    int mw = (int)m.WParam;
                    int ml = (int)m.LParam;
                    Files.WriteLog(" [Success] recevied "+ m.Msg+"--" + ((int)m.WParam).ToString() + "/ " + ((int)m.LParam).ToString());
                }
            }
            catch (Exception exp)
            {
                Files.WriteError(DateTime.Now.ToString("HH:mm:ss fff ") + " WndProc ;  Error: " + exp.ToString());
            }
            base.WndProc(ref m);
        }
    }

    class TimerState
    {
        public int Counter;
        public bool Result;
    }
}
