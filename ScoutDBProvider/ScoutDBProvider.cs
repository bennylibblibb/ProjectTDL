﻿
using FileLog;
using FirebirdSql.Data.FirebirdClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;

namespace ScoutDBProvider
{
    public partial class ScoutDBProvider : Form
    {

        [DllImport("kernel32.dll")]
        private static extern int GetLastError();

        [DllImport("user32.dll", EntryPoint = "FindWindow")]
        public static extern int FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        public static extern uint RegisterWindowMessage(string lpString);

        [DllImport("user32.dll")]
        public static extern IntPtr PostMessage(IntPtr hWnd, uint msg, int wParam, int lParam);

        private static readonly IntPtr HWND_BROADCAST = new IntPtr(0xFFFF);
        private const int SUCCESS_CODE = 100000;
        private static readonly uint uiSkSvrNotify = RegisterWindowMessage(AppFlag.SkSvrNotify);
        private static readonly uint uiSkSvrNotify2 = RegisterWindowMessage(AppFlag.SkSvrNotify2); 

        // private readonly int m_ExternID = RegisterWindowMessage(Directory.GetCurrentDirectory() + @"\ScoutDBProvider");
        private readonly uint m_ExternID = RegisterWindowMessage(AppFlag.SkSvrNotify);

        static string[] Day = new string[] { "SUN", "MON", "TUE", "WED", "THU", "FRI", "SAT" };

        private static System.Threading.Timer tTimer;
        private static System.Threading.Timer analysisTimer;
        private static System.Threading.Timer InitTimer2;

        private static System.Timers.Timer aTimer;
        private static FbConnection connInitMatch; 

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
                dueTime: (dueTimes < DateTime.Now ? dueTimes.AddDays(1).Subtract(DateTime.Now) : dueTimes.Subtract(DateTime.Now)),//  new TimeSpan(1000),//
                period:  dueTimes.AddDays(1).Subtract(dueTimes)  //new TimeSpan(24, 0, 1)  
                );

                DateTime analysisTimes = Convert.ToDateTime((DateTime.Now.Date.ToString("yyyy/MM/dd ") + AppFlag.DailyAnalysisTime));
                var analysisState = new TimerState { Counter = 0 };
                analysisTimer = new System.Threading.Timer(
                callback: new TimerCallback(analysisTimerTask),
                state: analysisState,
                dueTime: (analysisTimes < DateTime.Now ? analysisTimes.AddDays(1).Subtract(DateTime.Now) : analysisTimes.Subtract(DateTime.Now)),
                period:  analysisTimes.AddDays(1).Subtract(analysisTimes) //new TimeSpan(24, 0, 1)  
                );

                DateTime InitTimes2 = Convert.ToDateTime((DateTime.Now.Date.ToString("yyyy/MM/dd ") + AppFlag.InitialTime2));
                var InitState2 = new TimerState { Counter = 0 };
                InitTimer2 = new System.Threading.Timer(
                callback: new TimerCallback(InitTimes2Task),
                state: InitState2,
                dueTime: (InitTimes2 < DateTime.Now ? InitTimes2.AddDays(1).Subtract(DateTime.Now) : InitTimes2.Subtract(DateTime.Now)),
                period:  InitTimes2.AddDays(1).Subtract(InitTimes2) //new TimeSpan(24, 0, 1)  
                );

                //auto match ontime
                aTimer = new System.Timers.Timer(AppFlag.InitMatchInterval * 60 * 1000);
                aTimer.Elapsed += OnTimedEvent;
                aTimer.AutoReset = true;
                aTimer.Enabled = true;

                connInitMatch= new FbConnection(AppFlag.MangoDBConn);
            }
            catch (Exception exp)
            {
                Files.WriteError("ScoutDBProvider(),error: " + exp.Message);
            }
        }
         
        private void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                int count = 0;
                string queryString = "update LIVEGOALS a set a.CSTATUS = '1',A.TIMEFLAG=current_timestamp  where   a.CMATCHDATE ||a.CMATCHTIME < replace(cast(cast('Now' as date) as varchar(10)), '-', '')|| replace( substring( cast(cast('Now' as time) as varchar(24)) from 1 for 5),':', '')  AND   A.CSTATUS='0'  ";
                using (FbCommand cmd2 = new FbCommand(queryString, connInitMatch))
                {
                    if(connInitMatch.State== ConnectionState.Closed)
                    {
                        connInitMatch.Open();
                    }
                    count =  cmd2.ExecuteNonQuery();
                    Files.WriteLog("Sql: "+ count+" " + queryString);
                }
                if (count > 0 == true) { SendAlertMsg(30); }
            }
            catch (Exception exp)
            {
                Files.WriteError(DateTime.Now.ToString("HH:mm:ss ") + "OnTimedEvent,error: " + exp);
                connInitMatch.Dispose();
                connInitMatch = new FbConnection(AppFlag.MangoDBConn);
            }
        }

        private void InitTimes2Task(object timerState)
        {
            AlertEng();
            InitGoalInfo();

            var state = timerState as TimerState;
            Interlocked.Increment(ref state.Counter);
            Files.WriteLog(" InitTimes2Task " + state.Counter + " " + DateTime.Now);
        }

        private void AlertEng()
        {
            try
            {
                string queryString = "";
                DataSet ds = new DataSet();
                using (FbConnection connection = new FbConnection(AppFlag.MangoDBConn))
                {
                    connection.Open();
                    Files.WriteLog("Get Engname.");
                    //this.listBox1.Invoke(new Action(() =>
                    //{
                    //    this.listBox1.Invoke(new Action(() => { { this.listBox1.Items.Insert(0, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ") + "Start Init LIVEGOALS"); } }));
                    //})); 
                    // queryString = "SELECT a.CLEAGUEALIAS, a.CLEAGUE||' ' || a.CHOST||'/'|| a.CGUEST abc, cas  a.CSCORER, a.TIMEFLAG FROM GOALDETAILS a where  '" + DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd") + " 12:00:00.000'  <a.TIMEFLAG and a.TIMEFLAG<  '" + DateTime.Now.ToString("yyyy-MM-dd") + " 12:00:00.000'  order by a.TIMEFLAG desc";
                    queryString = "SELECT distinct a.CLEAGUEALIAS, a.CLEAGUE||'  ' || a.CHOST||'/'|| a.CGUEST A,  case when (a.CRECORDBELONG ='H'and  position('*'  in  a.CSCORER )<0) or (a.CRECORDBELONG ='G' and  position('*'  in  a.CSCORER )>0)   THEN A.CHOST ELSE A.CGUEST END ||'-'|| REPLACE(a.CSCORER,'*','')   B,  a.CSCORER, a.TIMEFLAG FROM GOALDETAILS a where  '" + DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd") + " 12:00:00.000'  <a.TIMEFLAG and a.TIMEFLAG<  '" + DateTime.Now.ToString("yyyy-MM-dd") + " 12:00:00.000'  order by a.CLEAGUEALIAS, a.CHOST, cast ( a.CMATCHTIME as integer) ASC ";
                     Files.WriteLog("Sql: " + queryString);

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
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        try
                        {
                            string strs = "League       Host/Guest     Team - Player eng name " + Environment.NewLine;
                            foreach (DataRow dr in ds.Tables[0].Rows)
                            {
                                if (!Regex.IsMatch(dr["CSCORER"].ToString(), "[\u4e00-\u9fbb]") && dr["CSCORER"].ToString() !="")
                                {
                                    strs += dr["A"].ToString() + "  " + dr["B"].ToString() + Environment.NewLine;
                                    Files.WriteLog(dr["A"].ToString() + "  " + dr["B"].ToString() +"  " + dr["TIMEFLAG"].ToString() + "\r\n");
                                }
                            }
                           
                            string title = "Scouts Alert  Engname " + DateTime.Now.ToString("yyyy-MM-dd") + ".";
                            MailMessage mail = new MailMessage();
                            MailAddress fromMail = new MailAddress(AppFlag.AlertEmailFrom);
                            mail.From = fromMail;
                            foreach (var address in AppFlag.AlertEmailTo.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                            {
                                mail.To.Add(address);
                            }
                            SmtpClient client = new SmtpClient();
                            client.Port = 25;
                            client.DeliveryMethod = SmtpDeliveryMethod.Network;
                            client.UseDefaultCredentials = false;
                            client.Host = AppFlag.AlertEmailSMTP;
                            mail.Subject = title;
                            mail.Body = "Scouts alert player engname " + DateTime.Now.ToString("yyyy-MM-dd") + "." + Environment.NewLine + Environment.NewLine + strs;
                            mail.Priority = MailPriority.High;
                            client.Send(mail);
                            Files.WriteLog("Alert Engname: " + DateTime.Now.ToString("yyyy-MM-dd"));
                        }
                        catch (Exception exp)
                        {
                            Files.WriteError(DateTime.Now.ToString("HH:mm:ss ") + "Alert Engname,error: " + exp);
                        }
                    }

                    connection.Close();
                }
            }
            catch (Exception exp)
            {
                Files.WriteError(DateTime.Now.ToString("HH:mm:ss ") + "Alert ,error: " + exp);
            }
        }

        private void analysisTimerTask(object timerState)
        {
            try
            {
                this.listBox1.Invoke(new Action(() =>
                {
                    this.listBox1.Invoke(new Action(() => { { this.listBox1.Items.Insert(0, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ") + "TimerTask Analysis."); } }));
                }));

                Files.WriteLog("TimerTask Analysis.");

                int iIndex = 0;
                DataSet ds = new DataSet();
                string[] syncItems;
                ArrayList configSetting = new ArrayList();
                string queryString = "";

                configSetting = AppFlag.AnalysisItems;
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
                // configSetting.Clear();

                if (syncItems.Count() > 0)
                {
                    int i = 0;
                    try
                    {
                        using (FbConnection connection = new FbConnection(AppFlag.ScoutsDBConn))
                        {
                            connection.Open();
                            for (i = 0; i < 3; i++)
                            {
                                Files.WriteLog("Update " + syncItems[i].ToString());
                                this.listBox1.Invoke(new Action(() =>
                                {
                                    this.listBox1.Invoke(new Action(() => { { this.listBox1.Items.Insert(0, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ") + "Start sync " + syncItems[i] + "."); } }));
                                }));

                                if (i == 0)
                                {
                                    //ANALYSIS_STAT_INFO
                                    //queryString = " select   'CLEAGUE='''|| x.CLEAGUE_HKJC_NAME||''' AND CHOST=''' || x.HKJCHOSTNAME_CN||''' AND CGUEST='''|| x.HKJCGUESTNAME_CN|| ''' and IMATCHDATE=''' || replace(cast(X.CMATCHDATETIME as date), '-', '') || ''' and IMATCHTIME=''' || SUBSTRING(replace(cast(X.CMATCHDATETIME as time), ':', '') FROM 1 FOR 6) || ''''  ABC, X.EMATCHID,X.HKJCDAYCODE,X.HKJCMATCHNO, x.CMATCHDATETIME from EMATCHES X WHERE X.HKJCDAYCODE  = '" + Day[Convert.ToInt32(DateTime.Now.DayOfWeek.ToString("d"))].ToString() + "' and cast(cast(X.CMATCHDATETIME as date) as varchar(10))>= cast(cast(current_timestamp as date) - 1 as varchar(10))   AND X.EMATCHID IS NOT NULL  AND X.EMATCHID > 0  ORDER BY x.HKJCMATCHNO ASC";
                                    queryString = "select  e.EMATCHID eid,   e.CLEAGUE_HKJC_NAME,  e.CLEAGUEALIAS_OUTPUT_NAME, e.HKJCHOSTNAME_CN, e.HKJCGUESTNAME_CN, e.CMATCHDATETIME,  a.*   from ANALYSIS_STAT_INFO  a inner join EMATCHES e on e.EMATCHID =a.IMATCH_CNT   where a.IMATCH_CNT in ( select X.EMATCHID from EMATCHES X WHERE X.HKJCDAYCODE = '" + Day[Convert.ToInt32(DateTime.Now.DayOfWeek.ToString("d"))].ToString() + "' and cast(cast(X.CMATCHDATETIME as date) as varchar(10))>= cast(cast( current_timestamp as date) - 1 as varchar(10))   AND X.EMATCHID IS NOT NULL  AND X.EMATCHID > 0)  ORDER BY  e.CMATCHDATETIME asc";
                                    //queryString = "select  e.EMATCHID eid,   e.CLEAGUE_HKJC_NAME,  e.CLEAGUEALIAS_OUTPUT_NAME, e.HKJCHOSTNAME_CN, e.HKJCGUESTNAME_CN, e.CMATCHDATETIME,  a.*   from ANALYSIS_STAT_INFO  a inner join EMATCHES e on e.EMATCHID =a.IMATCH_CNT   where a.IMATCH_CNT in ( select X.EMATCHID from EMATCHES X WHERE X.HKJCDAYCODE = '" + "WED" + "' and cast(cast(X.CMATCHDATETIME as date) as varchar(10))>= cast(cast( current_timestamp as date) - 1 as varchar(10))   AND X.EMATCHID IS NOT NULL  AND X.EMATCHID > 0)  ORDER BY  e.CMATCHDATETIME asc";
                                }
                                else if (i == 1)
                                {
                                    //ANALYSIS_HISTORY_INFO
                                    //queryString = "select    e.EMATCHID eid,    'CLEAGUEALIAS='''||  e.CLEAGUEALIAS_OUTPUT_NAME||''' AND CHOST=''' || e.HKJCHOSTNAME_CN||''' AND CGUEST='''|| e.HKJCGUESTNAME_CN||''' and cdate='''||substring( replace( cast(a.START_DATE as date),'-','') FROM 1 FOR 6)||'''' ABC, e.CLEAGUE_HKJC_NAME,  e.CLEAGUEALIAS_OUTPUT_NAME, e.HKJCHOSTNAME_CN, e.HKJCGUESTNAME_CN, e.CMATCHDATETIME,  a.*   from ANALYSIS_HISTORY_INFO  a inner join EMATCHES e on e.EMATCHID =a.IMATCH_CNT   where a.IMATCH_CNT in (select X.EMATCHID from EMATCHES X WHERE X.HKJCDAYCODE = '" + Day[Convert.ToInt32(DateTime.Now.DayOfWeek.ToString("d"))].ToString() + "' and cast(cast(X.CMATCHDATETIME as date) as varchar(10))>= cast(cast( current_timestamp as date) - 1 as varchar(10))   AND X.EMATCHID IS NOT NULL  AND X.EMATCHID > 0 ) ORDER BY  a.imatch_cnt ,a.irec asc   ";
                                    queryString = "select  c.id ,c.name cname, c.alias calias, l.LEAGUE_CHI_NAME lCLEAGUE_NAME,   e.EMATCHID eid,    'CLEAGUEALIAS='''|| CASE WHEN  c.alias IS NULL THEN  C.NAME ELSE C.ALIAS END  ||''' AND CHOST=''' || e.HKJCHOSTNAME_CN||''' AND CGUEST='''|| e.HKJCGUESTNAME_CN||''' and cdate='''||substring( replace( cast(a.START_DATE as date),'-','') FROM 1 FOR 6)||'''' ABC, e.CLEAGUE_HKJC_NAME,  e.CLEAGUEALIAS_OUTPUT_NAME, e.HKJCHOSTNAME_CN, e.HKJCGUESTNAME_CN, e.CMATCHDATETIME,  a.*   from ANALYSIS_HISTORY_INFO  a inner join EMATCHES e on e.EMATCHID =a.IMATCH_CNT  left join COMPETITIONS c on c.id =a.LEAGUEALIASID left join LEAGUE_INFO l on  c.ALIAS=l.CLEAGUE_ALIAS_NAME   where a.IMATCH_CNT in (select X.EMATCHID from EMATCHES X WHERE X.HKJCDAYCODE = '" + Day[Convert.ToInt32(DateTime.Now.DayOfWeek.ToString("d"))].ToString() + "' and cast(cast(X.CMATCHDATETIME as date) as varchar(10))>= cast(cast( current_timestamp as date) - 1 as varchar(10))   AND X.EMATCHID IS NOT NULL  AND X.EMATCHID > 0 ) ORDER BY  a.imatch_cnt ,a.irec asc   ";
                                    // queryString = "select  c.id ,c.name cname, c.alias calias, l.CLEAGUE_NAME lCLEAGUE_NAME,   e.EMATCHID eid,    'CLEAGUEALIAS='''||  e.CLEAGUEALIAS_OUTPUT_NAME||''' AND CHOST=''' || e.HKJCHOSTNAME_CN||''' AND CGUEST='''|| e.HKJCGUESTNAME_CN||''' and cdate='''||substring( replace( cast(a.START_DATE as date),'-','') FROM 1 FOR 6)||'''' ABC, e.CLEAGUE_HKJC_NAME,  e.CLEAGUEALIAS_OUTPUT_NAME, e.HKJCHOSTNAME_CN, e.HKJCGUESTNAME_CN, e.CMATCHDATETIME,  a.*   from ANALYSIS_HISTORY_INFO  a inner join EMATCHES e on e.EMATCHID =a.IMATCH_CNT  left join COMPETITIONS c on c.id =a.LEAGUEALIASID left join LEAGUE_INFO l on  c.ALIAS=l.CLEAGUE_ALIAS_NAME   where a.IMATCH_CNT in (select X.EMATCHID from EMATCHES X WHERE X.HKJCDAYCODE = '" + "WED" + "' and cast(cast(X.CMATCHDATETIME as date) as varchar(10))>= cast(cast( current_timestamp as date) - 1 as varchar(10))   AND X.EMATCHID IS NOT NULL  AND X.EMATCHID > 0 ) ORDER BY  a.imatch_cnt ,a.irec asc   ";
                                }
                                else if (i == 2)
                                {
                                    //ANALYSIS_RECENT_INFO
                                    queryString = "select  e.EMATCHID eid,   'CLEAGUE='''||  e.CLEAGUE_HKJC_NAME||''' AND CHOST=''' || e.HKJCHOSTNAME_CN||''' AND CGUEST='''|| e.HKJCGUESTNAME_CN||''' ' ABC, e.CLEAGUE_HKJC_NAME CLEAGUE,  e.CLEAGUEALIAS_OUTPUT_NAME , e.HKJCHOSTNAME_CN CHOST, e.HKJCGUESTNAME_CN CGUEST, e.CMATCHDATETIME,  a.* from ANALYSIS_RECENT_INFO a inner join EMATCHES e on e.EMATCHID = a.IMATCH_CNT   where a.IMATCH_CNT in  ( select X.EMATCHID from EMATCHES X WHERE X.HKJCDAYCODE = '" + Day[Convert.ToInt32(DateTime.Now.DayOfWeek.ToString("d"))].ToString() + "' and cast(cast(X.CMATCHDATETIME as date) as varchar(10))>= cast(cast( current_timestamp as date) - 1 as varchar(10))   AND X.EMATCHID IS NOT NULL  AND X.EMATCHID > 0 ) AND (A.IHOSTSCORE IS NOT NULL AND A.IGUESTSCORE IS NOT NULL ) AND (A.IHOSTSCORE IS NOT NULL AND A.IGUESTSCORE IS NOT NULL ) ORDER BY  a.IMATCH_CNT ,a.irec asc";
                                    //queryString = "select  e.EMATCHID eid,   'CLEAGUE='''||  e.CLEAGUE_HKJC_NAME||''' AND CHOST=''' || e.HKJCHOSTNAME_CN||''' AND CGUEST='''|| e.HKJCGUESTNAME_CN||''' ' ABC, e.CLEAGUE_HKJC_NAME CLEAGUE,  e.CLEAGUEALIAS_OUTPUT_NAME , e.HKJCHOSTNAME_CN CHOST, e.HKJCGUESTNAME_CN CGUEST, e.CMATCHDATETIME,  a.* from ANALYSIS_RECENT_INFO a inner join EMATCHES e on e.EMATCHID = a.IMATCH_CNT   where a.IMATCH_CNT in  ( select X.EMATCHID from EMATCHES X WHERE X.HKJCDAYCODE = '" + "WED" + "' and cast(cast(X.CMATCHDATETIME as date) as varchar(10))>= cast(cast( current_timestamp as date) - 1 as varchar(10))   AND X.EMATCHID IS NOT NULL  AND X.EMATCHID > 0 )  ORDER BY  a.IMATCH_CNT ,a.irec asc";
                                }

                                Files.WriteLog("Sql: " + queryString);

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

                                if (ds.Tables[0].Rows.Count > 0)
                                {
                                    using (FbConnection connection2 = new FbConnection(AppFlag.MangoDBConn))
                                    {
                                        connection2.Open();
                                        if (i == 0)
                                        {
                                            try
                                            {
                                                DateTime dt2 = DateTime.Now;
                                                foreach (DataRow dr in ds.Tables[0].Rows)
                                                {
                                                    using (FbCommand cmd2 = new FbCommand("PR_ANALYSIS_STAT", connection2))
                                                    {
                                                        cmd2.CommandType = CommandType.StoredProcedure;
                                                        cmd2.Parameters.Add("@CLEAGUE", dr["CLEAGUE_HKJC_NAME"]);
                                                        cmd2.Parameters.Add("@CHOST", dr["HKJCHOSTNAME_CN"]);
                                                        cmd2.Parameters.Add("@CGUEST", dr["HKJCGUESTNAME_CN"]);
                                                        cmd2.Parameters.Add("@IMATCHDATE", Convert.ToDateTime(dr["CMATCHDATETIME"]).ToString("yyyyMMdd"));
                                                        cmd2.Parameters.Add("@IMATCHTIME", Convert.ToDateTime(dr["CMATCHDATETIME"]).ToString("HHmmss"));
                                                        cmd2.Parameters.Add("@CHANDICAP", "1");
                                                        cmd2.Parameters.Add("@CMATCHFIELD", "H");
                                                        cmd2.Parameters.Add("@IHOSTWIN", dr["IHOSTWIN"]);
                                                        cmd2.Parameters.Add("@IHOSTDRAW", dr["IHOSTDRAW"]);
                                                        cmd2.Parameters.Add("@IHOSTLOSS", dr["IHOSTLOSS"]);
                                                        cmd2.Parameters.Add("@IGUESTWIN", dr["IGUESTWIN"]);
                                                        cmd2.Parameters.Add("@IGUESTDRAW", dr["IGUESTDRAW"]);
                                                        cmd2.Parameters.Add("@IGUESTLOSS", dr["IGUESTLOSS"]);
                                                        cmd2.Parameters.Add("@CREMARK", "");
                                                        cmd2.Parameters.Add("@TIMEFLAG", dt2.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                                                        cmd2.Parameters.Add("@IDEST", "0");
                                                        int id = Convert.ToInt32(cmd2.ExecuteScalar());
                                                        dt2 = dt2.AddMilliseconds(1);
                                                        if (id > -1)
                                                        {
                                                            Files.WriteLog(" [Success] Insert ANALYSIS_STAT " + " " + dr["CLEAGUEALIAS_OUTPUT_NAME"] + " " + dr["HKJCHOSTNAME_CN"] + "/" + dr["HKJCGUESTNAME_CN"]);
                                                        }
                                                    }
                                                }
                                            }
                                            catch (Exception exp)
                                            {
                                                Files.WriteError(DateTime.Now.ToString("HH:mm:ss ") + "Sync ANALYSIS_STAT,error1: " + exp);
                                            }

                                            //List<string> lsWhere = ds.Tables[0].AsEnumerable().Select(d => d.Field<string>("ABC")).ToList<string>().Distinct().ToList();
                                            //string strWhere = string.Concat("(", string.Join(") OR  (", lsWhere), ")");
                                            //using (FbCommand cmd2 = new FbCommand())
                                            //{
                                            //    cmd2.CommandText = "UPDATE ANALYSISTATS SET TIMEFLAG='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "' where " + strWhere;
                                            //    cmd2.Connection = connection2;
                                            //    int id = Convert.ToInt32(cmd2.ExecuteScalar());
                                            //    Files.WriteLog("Sql: "+ cmd2.CommandText);
                                            //}

                                        }
                                        else if (i == 1)
                                        {
                                            try
                                            {
                                                using (FbCommand cmd2 = new FbCommand())
                                                {
                                                    List<string> strL = ds.Tables[0].AsEnumerable().Select(d => d.Field<string>("ABC")).ToList<string>().Distinct().ToList();
                                                    string strs = string.Concat("(", string.Join(") OR  (", strL), ")");
                                                    cmd2.CommandText = "delete from ANALYSISHISTORYS where " + strs.Replace("OR  ()", "").Replace("() OR", "").Replace("()", "");
                                                    cmd2.Connection = connection2;
                                                    int id = Convert.ToInt32(cmd2.ExecuteScalar());
                                                    Files.WriteLog("delete from ANALYSISHISTORYS where " + strs.Replace("OR  ()", "").Replace("() OR", "").Replace("()", "") + "  id=" + id);
                                                }

                                                queryString = "select first   1 * from ANALYSISHISTORYS";
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
                                                                DateTime dt2 = DateTime.Now;
                                                                foreach (DataRow dr2 in ds.Tables[0].Rows)
                                                                {
                                                                    DataRow[] drs = data.Tables["data"].Select("CHOST='" + dr2["HKJCHOSTNAME_CN"] + "' AND CGUEST='" + dr2["HKJCGUESTNAME_CN"] + "' and cdate='" + (dr2["START_DATE"] is DBNull ? dr2["IMATCHYEAR"].ToString() + dr2["IMATCHMONTH"].ToString().PadLeft(2, '0') : Convert.ToDateTime(dr2["START_DATE"]).ToString("yyyyMM")) + "'");
                                                                    // DataRow[] drs2 = data.Tables["data"].Select("CLEAGUE='" + dr2["CLEAGUE_HKJC_NAME"] + "' AND CHOST='" + dr2["HKJCHOSTNAME_CN"] + "' AND CGUEST='" + dr2["HKJCGUESTNAME_CN"] + "'");
                                                                    DataRow[] drs2 = data.Tables["data"].Select("CHOST='" + dr2["HKJCHOSTNAME_CN"] + "' AND CGUEST='" + dr2["HKJCGUESTNAME_CN"] + "'");

                                                                    if (drs.Length > 0 || drs2.Length > 4) continue;
                                                                    // if ( drs2.Length > 4) continue;
                                                                    DataRow dr3 = data.Tables["data"].NewRow();
                                                                    // dr3[0] = dr2["CLEAGUE_HKJC_NAME"];
                                                                    dr3[0] = dr2["lCLEAGUE_NAME"] is DBNull ? dr2["cname"] : dr2["lCLEAGUE_NAME"];
                                                                    dr3[1] = dr2["HKJCHOSTNAME_CN"];
                                                                    dr3[2] = dr2["HKJCGUESTNAME_CN"];
                                                                    dr3[3] = dr2["START_DATE"] is DBNull ? dr2["IMATCHYEAR"].ToString() + dr2["IMATCHMONTH"].ToString().PadLeft(2, '0') : Convert.ToDateTime(dr2["START_DATE"]).ToString("yyyyMM");
                                                                    //dr3[4] = dr2["CLEAGUEALIAS_OUTPUT_NAME"];calias
                                                                    dr3[4] = dr2["lCLEAGUE_NAME"] is DBNull ? dr2["cname"] : dr2["calias"];
                                                                    dr3[5] = dr2["IMATCHSTATUS"].ToString() == "0" ? "主" : "客";
                                                                    dr3[6] = dr2["IHOSTSCORE"];
                                                                    dr3[7] = dr2["IGUESTSCORE"];
                                                                    dr3[8] = dt2.ToString("yyyy-MM-dd HH:mm:ss.fff");
                                                                    dr3[9] = "0";
                                                                    data.Tables["data"].Rows.Add(dr3);
                                                                    dt2 = dt2.AddMilliseconds(1);
                                                                    Files.WriteLog("Add ANALYSISHISTORYS " + dr3[0] + " " + dr3[1] + "/" + dr3[2]);
                                                                }
                                                                fda.Update(data.Tables["data"]);
                                                                Files.WriteLog(" Insert ANALYSISHISTORYS [" + data.Tables["data"].Rows.Count + "]");
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            catch (Exception exp)
                                            {
                                                Files.WriteError(DateTime.Now.ToString("HH:mm:ss ") + "Sync ANALYSISHISTORYS,error1: " + exp);
                                            }

                                            //try
                                            //{
                                            //    using (FbCommand cmd2 = new FbCommand())
                                            //    {
                                            //        List<string> strL = ds.Tables[0].AsEnumerable().Select(d => d.Field<string>("ABC")).ToList<string>().Distinct().ToList();
                                            //        string strs = string.Concat("(", string.Join(") OR  (", strL), ")");
                                            //        cmd2.CommandText = "update   ANALYSISHISTORYS set  TIMEFLAG='"+ DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "' where " + strs;
                                            //        Files.WriteLog("Sql: " + cmd2.CommandText);
                                            //        cmd2.Connection = connection2;
                                            //        int id = Convert.ToInt32(cmd2.ExecuteScalar());

                                            //    } 
                                            //}
                                            //catch (Exception exp)
                                            //{
                                            //    Files.WriteError(DateTime.Now.ToString("HH:mm:ss ") + "Sync ANALYSISHISTORYS,error1: " + exp);
                                            //}
                                        }
                                        else if (i == 2)
                                        {
                                            try
                                            {
                                                using (FbCommand cmd2 = new FbCommand())
                                                {
                                                    List<string> strL = ds.Tables[0].AsEnumerable().Select(d => d.Field<string>("ABC")).ToList<string>().Distinct().ToList();
                                                    string strs = string.Concat("(", string.Join(") OR  (", strL), ")");
                                                    cmd2.CommandText = "delete from ANALYSISRECENTS where " + strs.Replace("OR  ()", "").Replace("() OR", "").Replace("()", "");
                                                    cmd2.Connection = connection2;
                                                    int id = Convert.ToInt32(cmd2.ExecuteScalar());
                                                    Files.WriteLog("delete from ANALYSISRECENTS where " + strs.Replace("OR  ()", "").Replace("() OR", "").Replace("()", "") + "  id=" + id);
                                                }

                                                queryString = "select first   1 * from ANALYSISRECENTS";
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
                                                                List<string> strTeam = ds.Tables[0].AsEnumerable().Select(d => d.Field<string>("ABC")).ToList<string>().Distinct().ToList();
                                                                DateTime dt2 = DateTime.Now;
                                                                foreach (string s in strTeam)
                                                                {
                                                                    DataRow[] drs = ds.Tables[0].Select(s);
                                                                    List<DataRow> drH = (drs.AsEnumerable().ToList<DataRow>().Where(x => x.Field<string>("CTEAMFLAG") == "H").ToList()).ToList<DataRow>().Where(x => x.Field<int>("IHOSTSCORE") != -1).ToList();
                                                                    List<DataRow> drG = (drs.AsEnumerable().ToList<DataRow>().Where(x => x.Field<string>("CTEAMFLAG") == "G").ToList()).ToList<DataRow>().Where(x => x.Field<int>("IHOSTSCORE") != -1).ToList();
                                                                    List<DataRow> drHN = (drs.AsEnumerable().ToList<DataRow>().Where(x => x.Field<string>("CTEAMFLAG") == "H").ToList()).ToList<DataRow>().Where(x => x.Field<int>("IHOSTSCORE") == -1).ToList();
                                                                    List<DataRow> drGN = (drs.AsEnumerable().ToList<DataRow>().Where(x => x.Field<string>("CTEAMFLAG") == "G").ToList()).ToList<DataRow>().Where(x => x.Field<int>("IHOSTSCORE") == -1).ToList();

                                                                    DataRow dr3 = data.Tables["data"].NewRow();
                                                                    dr3[0] = drH.Count > 0 ? drH[0]["CLEAGUE"] : drG.Count > 0 ? drG[0][2] : "";
                                                                    dr3[1] = drH.Count > 0 ? drH[0]["CHOST"] : drG.Count > 0 ? drG[0][3] : "";
                                                                    dr3[2] = drH.Count > 0 ? drH[0]["CGUEST"] : drG.Count > 0 ? drG[0][4] : "";
                                                                    dr3[3] = "U";
                                                                    dr3[4] = drHN.Count > 0 ? drHN[0]["LEAGUEALIAS"].ToString() + "/" + drHN[0]["CCHALLENGER"].ToString() + "/" + (drHN[0]["IMATCHSTATUS"].ToString() == "0" ? "主" : "客") : "-1/-1/-1";
                                                                    dr3[5] = drH.Count > 0 ? drH[0]["LEAGUEALIAS"].ToString() + "/" + drH[0]["CCHALLENGER"].ToString() + "/" + (drH[0]["IMATCHSTATUS"].ToString() == "0" ? "主" : "客") + "/" + drH[0]["IHOSTSCORE"].ToString() + "/" + drH[0]["IGUESTSCORE"].ToString() : "-1";
                                                                    dr3[6] = drH.Count > 1 ? drH[0]["LEAGUEALIAS"].ToString() + "/" + drH[1]["CCHALLENGER"].ToString() + "/" + (drH[1]["IMATCHSTATUS"].ToString() == "0" ? "主" : "客") + "/" + drH[1]["IHOSTSCORE"].ToString() + "/" + drH[1]["IGUESTSCORE"].ToString() : "-1";
                                                                    dr3[7] = drH.Count > 2 ? drH[0]["LEAGUEALIAS"].ToString() + "/" + drH[2]["CCHALLENGER"].ToString() + "/" + (drH[2]["IMATCHSTATUS"].ToString() == "0" ? "主" : "客") + "/" + drH[2]["IHOSTSCORE"].ToString() + "/" + drH[2]["IGUESTSCORE"].ToString() : "-1";
                                                                    dr3[8] = drH.Count > 3 ? drH[0]["LEAGUEALIAS"].ToString() + "/" + drH[3]["CCHALLENGER"].ToString() + "/" + (drH[3]["IMATCHSTATUS"].ToString() == "0" ? "主" : "客") + "/" + drH[3]["IHOSTSCORE"].ToString() + "/" + drH[3]["IGUESTSCORE"].ToString() : "-1";
                                                                    dr3[9] = drH.Count > 4 ? drH[0]["LEAGUEALIAS"].ToString() + "/" + drH[4]["CCHALLENGER"].ToString() + "/" + (drH[4]["IMATCHSTATUS"].ToString() == "0" ? "主" : "客") + "/" + drH[4]["IHOSTSCORE"].ToString() + "/" + drH[4]["IGUESTSCORE"].ToString() : "-1";
                                                                    dr3[10] = drGN.Count > 0 ? drGN[0]["LEAGUEALIAS"].ToString() + "/" + drGN[0]["CCHALLENGER"].ToString() + "/" + (drGN[0]["IMATCHSTATUS"].ToString() == "0" ? "主" : "客") : "-1/-1/-1";
                                                                    dr3[11] = drG.Count > 0 ? drG[0]["LEAGUEALIAS"].ToString() + "/" + drG[0]["CCHALLENGER"].ToString() + "/" + (drG[0]["IMATCHSTATUS"].ToString() == "0" ? "主" : "客") + "/" + drG[0]["IHOSTSCORE"].ToString() + "/" + drG[0]["IGUESTSCORE"].ToString() : "-1";
                                                                    dr3[12] = drG.Count > 1 ? drG[0]["LEAGUEALIAS"].ToString() + "/" + drG[1]["CCHALLENGER"].ToString() + "/" + (drG[1]["IMATCHSTATUS"].ToString() == "0" ? "主" : "客") + "/" + drG[1]["IHOSTSCORE"].ToString() + "/" + drG[1]["IGUESTSCORE"].ToString() : "-1";
                                                                    dr3[13] = drG.Count > 2 ? drG[0]["LEAGUEALIAS"].ToString() + "/" + drG[2]["CCHALLENGER"].ToString() + "/" + (drG[2]["IMATCHSTATUS"].ToString() == "0" ? "主" : "客") + "/" + drG[2]["IHOSTSCORE"].ToString() + "/" + drG[2]["IGUESTSCORE"].ToString() : "-1";
                                                                    dr3[14] = drG.Count > 3 ? drG[0]["LEAGUEALIAS"].ToString() + "/" + drG[3]["CCHALLENGER"].ToString() + "/" + (drG[3]["IMATCHSTATUS"].ToString() == "0" ? "主" : "客") + "/" + drG[3]["IHOSTSCORE"].ToString() + "/" + drG[3]["IGUESTSCORE"].ToString() : "-1";
                                                                    dr3[15] = drG.Count > 4 ? drG[0]["LEAGUEALIAS"].ToString() + "/" + drG[4]["CCHALLENGER"].ToString() + "/" + (drG[4]["IMATCHSTATUS"].ToString() == "0" ? "主" : "客") + "/" + drG[4]["IHOSTSCORE"].ToString() + "/" + drG[4]["IGUESTSCORE"].ToString() : "-1";
                                                                    dr3[16] = dt2.ToString("yyyy-MM-dd HH:mm:ss.fff");
                                                                    dr3[17] = "0";
                                                                    data.Tables["data"].Rows.Add(dr3);
                                                                    dt2 = dt2.AddMilliseconds(1);
                                                                    Files.WriteLog("Add ANALYSISRECENTS " + dr3["CLEAGUE"] + " " + dr3["CHOST"] + "/" + dr3["CGUEST"]);
                                                                }

                                                                int count = fda.Update(data.Tables["data"]);
                                                                Files.WriteLog((count > 0 ? "[Success] " : "[Failure] ") + " Insert ANALYSISRECENTS [" + data.Tables["data"].Rows.Count + "]");
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            catch (Exception exp)
                                            {
                                                Files.WriteError(DateTime.Now.ToString("HH:mm:ss ") + "Sync ANALYSISRECENTS,error1: " + exp);
                                            }

                                            //try
                                            //{
                                            //    using (FbCommand cmd2 = new FbCommand())
                                            //    {
                                            //        List<string> strL = ds.Tables[0].AsEnumerable().Select(d => d.Field<string>("ABC")).ToList<string>().Distinct().ToList();
                                            //        string strs = string.Concat("(", string.Join(") OR  (", strL), ")");
                                            //        cmd2.CommandText = "update  ANALYSISRECENTS set  TIMEFLAG='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "' where " + strs;
                                            //        cmd2.Connection = connection2;
                                            //        int id = Convert.ToInt32(cmd2.ExecuteScalar());
                                            //        Files.WriteLog("Sql: " + cmd2.CommandText);
                                            //    }  
                                            //}
                                            //catch (Exception exp)
                                            //{
                                            //    Files.WriteError(" Sync ANALYSISRECENTS [" + ds.Tables[0].Rows.Count + "], error:" + exp.Message);
                                            //}
                                        }
                                        connection2.Close();
                                    }
                                }


                                int msgID = -1;
                                if (AppFlag.Alert)
                                {
                                    switch (syncItems[i])
                                    {
                                        case "ANALYSISTATS":
                                            {
                                                msgID = 14;
                                            }
                                            break;
                                        case "ANALYSISHISTORYS":
                                            {
                                                msgID = 11;
                                            }
                                            break;
                                        case "ANALYSISRECENTS":
                                            {
                                                msgID = 13;
                                            }
                                            break;

                                    }
                                    if (msgID != -1)
                                    {
                                        SendAlertMsg(msgID);
                                        Files.WriteLog(DateTime.Now.ToString("HH:mm:ss ") + "Send " + msgID);
                                    }
                                }
                            }
                            connection.Close();
                        }
                    }
                    catch (Exception exp)
                    {
                        Files.WriteError(DateTime.Now.ToString("HH:mm:ss ") + "analysisTimerTask()," + syncItems[i] + ",error: " + exp);
                    }
                }

                var state = timerState as TimerState;
                Interlocked.Increment(ref state.Counter);
                Files.WriteLog(" analysisTimerTask " + state.Counter + " " + DateTime.Now);

            }
            catch (Exception exp)
            {
                Files.WriteError("analysisTimerTask(),error: " + exp.Message);
            }
        }

        private void InitGoalInfo()
        {
            try
            {
                string queryString = "";
                DataSet ds = new DataSet();
                using (FbConnection connection = new FbConnection(AppFlag.ScoutsDBConn))
                {
                    connection.Open();
                    Files.WriteLog("Init LIVEGOALS");
                    this.listBox1.Invoke(new Action(() =>
                    {
                        this.listBox1.Invoke(new Action(() => { { this.listBox1.Items.Insert(0, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ") + "Start Init LIVEGOALS"); } }));
                    }));

                    //LIVEGOALS   
                    //queryString = "SELECT E.MAPPINGSTATUS, ( CASE WHEN E.MAPPINGSTATUS IS TRUE THEN E.HKJCGUESTNAME_CN ELSE E.HKJCHOSTNAME_CN END ) HH,  ( CASE WHEN E.MAPPINGSTATUS IS NOT TRUE THEN E.HKJCHOSTNAME_CN ELSE E.HKJCGUESTNAME_CN END ) GG,  (select first 1 t.HKJC_NAME_CN from teams t  where t.id= r.HOME_ID) H,(select first 1 t.HKJC_NAME_CN from teams t  where t.id=r.GUEST_ID) G, " +
                    //    "e.HKJCDAYCODE,e.HKJCMATCHNO,  E.CLEAGUEALIAS_OUTPUT_NAME,E.CLEAGUE_HKJC_NAME,E.HKJCHOSTNAME_CN, E.HKJCGUESTNAME_CN,E.CMATCHDATETIME CMATCHDATE,E.CMATCHDATETIME CMATCHTIME,'H' CMATCHFIELD,'U' " +
                    //             "CACTION,G.H_GOAL,G.G_GOAL,G.H_RED,G.G_RED,G.HH_GOAL,G.GH_GOAL,G.H_CONFIRM,G.G_CONFIRM,''CSONGID,''CALERT,G.GAMESTATUS,'' CCOMMENT,-1 " +
                    //             "CTIMEOFGAME,current_timestamp TIMEFLAG,'0'IDEST FROM EVENTS r INNER JOIN  EMATCHES E ON E.EMATCHID = R.ID left JOIN GOALINFO G ON G.EMATCHID = " +
                    //             "E.EMATCHID WHERE R.ID in(select E.EMATCHID from EMATCHES E   WHERE e.HKJCDAYCODE = '" + Day[Convert.ToInt32(DateTime.Now.DayOfWeek.ToString("d"))].ToString() + "' " +
                    //             " and cast(cast(CMATCHDATETIME as date) as varchar(10))>= cast(cast( current_timestamp as date) - 1 as varchar(10)) " +
                    //            //-- and cast(cast(CMATCHDATETIME as date) as varchar(10))<= cast(cast('29.04.2019' as date) + 1 as varchar(10)) ---  order by e.HKJCMATCHNO asc
                    //            ")  and cast(cast(e.CMATCHDATETIME as date) as varchar(10))>= cast(cast( current_timestamp as date) - 1 as varchar(10))  ORDER BY E.HKJCMATCHNO asc ";

                    queryString = "SELECT e.HKJCDAYCODE||'['||e.HKJCMATCHNO||']  '||e.CLEAGUE_HKJC_NAME||'  '||e.HKJCHOSTNAME_CN||'/'||e.HKJCGUESTNAME_CN ||'  '||replace(e.CMATCHDATETIME, ':00.0000', '') ABC, E.EMATCHID EEMATCHID, E.MAPPINGSTATUS, ( CASE WHEN E.MAPPINGSTATUS IS TRUE THEN E.HKJCGUESTNAME_CN ELSE E.HKJCHOSTNAME_CN END ) HH,  ( CASE WHEN E.MAPPINGSTATUS IS TRUE THEN E.HKJCHOSTNAME_CN ELSE E.HKJCGUESTNAME_CN END ) GG,  (select first 1 t.HKJC_NAME_CN from teams t  where t.id= r.HOME_ID) H,(select first 1 t.HKJC_NAME_CN from teams t  where t.id=r.GUEST_ID) G, " +
                                "e.HKJCDAYCODE,e.HKJCMATCHNO,  E.CLEAGUEALIAS_OUTPUT_NAME,E.CLEAGUE_HKJC_NAME,E.HKJCHOSTNAME_CN, E.HKJCGUESTNAME_CN,E.CMATCHDATETIME CMATCHDATE,E.CMATCHDATETIME CMATCHTIME,'H' CMATCHFIELD,'U' " +
                               "CACTION,G.H_GOAL,G.G_GOAL,G.H_RED,G.G_RED,G.HH_GOAL,G.GH_GOAL,G.H_CONFIRM,G.G_CONFIRM,''CSONGID,''CALERT,(CASE WHEN G.GAMESTATUS IS NULL THEN 'Not mapped' ELSE G.GAMESTATUS END ) GAMESTATUS,'' CCOMMENT,-1 " +
                               "CTIMEOFGAME,current_timestamp TIMEFLAG,'0'IDEST FROM EVENTS r RIGHT JOIN  EMATCHES E ON E.EMATCHID = R.ID left JOIN GOALINFO G ON G.EMATCHID = " +
                               "E.EMATCHID WHERE cast(cast(e.CMATCHDATETIME as date) as varchar(10))>= cast(cast( current_timestamp as date) - 1 as varchar(10)) " +
                               // "R.ID in(select E.EMATCHID from EMATCHES E   WHERE e.HKJCDAYCODE = '" + Day[Convert.ToInt32(DateTime.Now.DayOfWeek.ToString("d"))].ToString() + "' and cast(cast(CMATCHDATETIME as date) as varchar(10))>= cast(cast( current_timestamp as date) - 1 as varchar(10))) " +
                               " and e.HKJCDAYCODE = '" + Day[Convert.ToInt32(DateTime.Now.DayOfWeek.ToString("d"))].ToString() + "'   ORDER BY E.HKJCMATCHNO asc ";
                    Files.WriteLog("Init Sql: " + queryString);

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

                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        //alert email
                        try
                        {
                            string strs = "DayCode    League       Host/Guest     Start Date  " + Environment.NewLine;
                            DataRow[] drs = ds.Tables[0].Select("EEMATCHID IS NULL OR EEMATCHID<=0 ");
                            if (drs.Count() > 0)
                            {
                                List<string> strL = drs.AsEnumerable().Select(d => d.Field<string>("ABC")).ToList<string>().Distinct().ToList();
                                  strs = string.Concat("", string.Join("\r\n", strL), "");
                                Files.WriteLog("Alert: " + strs);
                                string title = "Scouts alert initial (響機) " + DateTime.Now.ToString("yyyy-MM-dd") + ".";

                                MailMessage mail = new MailMessage();
                                MailAddress fromMail = new MailAddress(AppFlag.AlertEmailFrom);
                                mail.From = fromMail;
                                foreach (var address in AppFlag.AlertEmailTo.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                                {
                                    mail.To.Add(address);
                                }
                                SmtpClient client = new SmtpClient();
                                client.Port = 25;
                                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                                client.UseDefaultCredentials = false;
                                client.Host = AppFlag.AlertEmailSMTP;
                                mail.Subject = title;
                                mail.Body = title + Environment.NewLine + Environment.NewLine + strs;
                                mail.Priority = MailPriority.High;
                                client.Send(mail);
                            }
                        }
                        catch (Exception exp)
                        {
                            Files.WriteError(DateTime.Now.ToString("HH:mm:ss ") + "Alert initial,error: " + exp);
                        }

                        using (FbConnection connection2 = new FbConnection(AppFlag.MangoDBConn))
                        {
                            connection2.Open();
                            foreach (DataRow drH in ds.Tables[0].Select("EEMATCHID IS NOT NULL AND EEMATCHID>0 "))
                            {
                                try
                                {
                                    //if (drH["CLEAGUE_HKJC_NAME"] is DBNull || drH["CLEAGUEALIAS_OUTPUT_NAME"] is DBNull
                                    //    || drH["HKJCHOSTNAME_CN"] is DBNull || drH["HKJCGUESTNAME_CN"] is DBNull
                                    //    || drH["H"] is DBNull || drH["G"] is DBNull)
                                    //{ continue; }
                                    using (FbCommand cmd2 = new FbCommand("PR_LIVEGOALS", connection2))
                                    {
                                        cmd2.CommandType = CommandType.StoredProcedure;
                                        cmd2.Parameters.Add("@CLEAGUEALIAS", drH["CLEAGUEALIAS_OUTPUT_NAME"]);
                                        cmd2.Parameters.Add("@CLEAGUE", drH["CLEAGUE_HKJC_NAME"]);
                                        //cmd2.Parameters.Add("@CHOST", drH["H"] is DBNull ? drH["HKJCHOSTNAME_CN"] : drH["H"]);
                                        //cmd2.Parameters.Add("@CGUEST", drH["G"] is DBNull ? drH["HKJCGUESTNAME_CN"] C);
                                        cmd2.Parameters.Add("@CHOST", drH["H"] is DBNull ? drH["HH"] : drH["H"]);
                                        cmd2.Parameters.Add("@CGUEST", drH["G"] is DBNull ? drH["GG"] : drH["G"]);
                                        cmd2.Parameters.Add("@CMATCHDATE", Convert.ToDateTime(drH["CMATCHDATE"]).ToString("yyyyMMdd"));
                                        cmd2.Parameters.Add("@CMATCHTIME", Convert.ToDateTime(drH["CMATCHTIME"]).ToString("HHmm"));
                                        cmd2.Parameters.Add("@CMATCHFIELD", drH["CMATCHFIELD"]);
                                        cmd2.Parameters.Add("@CACTION", drH["CACTION"]);
                                        cmd2.Parameters.Add("@IH_GOAL", drH["H_GOAL"].ToString() == "" ? "0" : drH["H_GOAL"].ToString());
                                        cmd2.Parameters.Add("@IG_GOAL", drH["G_GOAL"].ToString() == "" ? "0" : drH["G_GOAL"].ToString());
                                        cmd2.Parameters.Add("@IH_REDCARD", drH["H_RED"].ToString() == "" ? "0" : drH["H_RED"].ToString());
                                        cmd2.Parameters.Add("@IG_REDCARD", drH["G_RED"].ToString() == "" ? "0" : drH["G_RED"].ToString());
                                        cmd2.Parameters.Add("@IH_HT_GOAL", drH["HH_GOAL"].ToString() == "" ? "0" : drH["HH_GOAL"].ToString());
                                        cmd2.Parameters.Add("@IG_HT_GOAL", drH["GH_GOAL"].ToString() == "" ? "0" : drH["GH_GOAL"].ToString());
                                        //cmd2.Parameters.Add("@IH_CONFIRM", drH["H_CONFIRM"].ToString() == "" ? "-1" : drH["H_CONFIRM"].ToString());
                                        //cmd2.Parameters.Add("@IG_CONFIRM", drH["G_CONFIRM"].ToString() == "" ? "-1" : drH["G_CONFIRM"].ToString());
                                        cmd2.Parameters.Add("@CSONGID", drH["CSONGID"]);
                                        cmd2.Parameters.Add("@CALERT", drH["CALERT"]);
                                        cmd2.Parameters.Add("@CSTATUS", drH["GAMESTATUS"]);
                                        cmd2.Parameters.Add("@CCOMMENT", drH["CCOMMENT"]);
                                        // cmd2.Parameters.Add("@CTIMEOFGAME", drH["CTIMEOFGAME"]);
                                        cmd2.Parameters.Add("@CTIMEOFGAME", drH["CTIMEOFGAME"] == DBNull.Value ? "-1" : drH["CTIMEOFGAME"]);
                                        cmd2.Parameters.Add("@TIMEFLAG", Convert.ToDateTime(drH["TIMEFLAG"]).ToString("yyyy-MM-dd HH:mm:ss.fff"));
                                        cmd2.Parameters.Add("@IDEST", drH["IDEST"]);
                                        int id = Convert.ToInt32(cmd2.ExecuteScalar());
                                        // if (id > -1)
                                        // {
                                        Files.WriteLog(" [Success] " + (id > 0 ? "Insert" : "Update") + " LIVEGOALS " + " " + drH["CLEAGUEALIAS_OUTPUT_NAME"] + " " + drH["HKJCHOSTNAME_CN"] + "/" + drH["HKJCGUESTNAME_CN"]);
                                        // }
                                    }
                                }
                                catch (Exception exp)
                                {
                                    Files.WriteError(DateTime.Now.ToString("HH:mm:ss ") + "LIVEGOALS,error1: " + exp);
                                }
                            }
                            connection2.Close();
                        }
                    }
                    SendAlertMsg(30);
                    ////ANALYSISBGREMARK 
                    //Files.WriteLog("Update ANALYSISBGREMARK");
                    //queryString = "SELECT g.WC_8,e.HKJCDAYCODE,e.HKJCMATCHNO,  E.CLEAGUEALIAS_OUTPUT_NAME,E.CLEAGUE_HKJC_NAME,E.HKJCHOSTNAME_CN, E.HKJCGUESTNAME_CN,E.CMATCHDATETIME CMATCHDATE, E.CMATCHDATETIME CMATCHTIME," +
                    // " '' CHANDICAP,'H' CMATCHFIELD,'' CHOSTROOT,   ''CGUESTROOT,''CVENUE,'' CTEMPERATURE,'' CWEATHERSTATUS,''CREMARK,current_timestamp  TIMEFLAG,'0'IDEST FROM EVENTS r" +
                    //    " INNER JOIN  EMATCHES E ON E.EMATCHID = R.ID INNER JOIN EVENT_DETAILS G ON G.eventid = " +
                    //             "E.EMATCHID WHERE R.ID in(select E.EMATCHID from EMATCHES E   WHERE e.HKJCDAYCODE = '" + Day[Convert.ToInt32(DateTime.Now.DayOfWeek.ToString("d"))].ToString() + "' " +
                    //             " and cast(cast(CMATCHDATETIME as date) as varchar(10))>= cast(cast( current_timestamp as date) - 1 as varchar(10)) " +
                    //            //-- and cast(cast(CMATCHDATETIME as date) as varchar(10))<= cast(cast('29.04.2019' as date) + 1 as varchar(10))
                    //            " order by e.HKJCMATCHNO asc  ) ORDER BY E.HKJCMATCHNO asc ";
                    //Files.WriteLog("Sql: " + queryString);

                    //using (FbCommand cmd = new FbCommand(queryString, connection))
                    //{
                    //    using (FbDataAdapter fda = new FbDataAdapter(cmd))
                    //    {
                    //        using (DataSet data = new DataSet())
                    //        {
                    //            data.Tables.Add(new DataTable("data"));
                    //            fda.Fill(data.Tables["data"]);
                    //            ds = data;
                    //        }
                    //    }
                    //}

                    //if (ds.Tables[0].Rows.Count > 0)
                    //{
                    //    using (FbConnection connection2 = new FbConnection(AppFlag.MangoDBConn))
                    //    {
                    //        connection2.Open();
                    //        foreach (DataRow drH in ds.Tables[0].Rows)
                    //        {
                    //            try
                    //            {
                    //                using (FbCommand cmd2 = new FbCommand("PR_ANALYSISBGREMARK", connection2))
                    //                {
                    //                    cmd2.CommandType = CommandType.StoredProcedure;
                    //                    cmd2.Parameters.Add("@CLEAGUE", drH["CLEAGUE_HKJC_NAME"]);
                    //                    cmd2.Parameters.Add("@CHOST", drH["HKJCHOSTNAME_CN"]);
                    //                    cmd2.Parameters.Add("@CGUEST", drH["HKJCGUESTNAME_CN"]);
                    //                    cmd2.Parameters.Add("@CMATCHDATE", Convert.ToDateTime(drH["CMATCHDATE"]).ToString("yyyyMMdd"));
                    //                    cmd2.Parameters.Add("@CMATCHTIME", Convert.ToDateTime(drH["CMATCHTIME"]).ToString("HHmm"));
                    //                    cmd2.Parameters.Add("@CHANDICAP", drH["CHANDICAP"]);
                    //                    cmd2.Parameters.Add("@CMATCHFIELD", drH["CMATCHFIELD"]);
                    //                    cmd2.Parameters.Add("@CHOSTROOT", drH["CHOSTROOT"]);
                    //                    cmd2.Parameters.Add("@CGUESTROOT", drH["CGUESTROOT"]);
                    //                    cmd2.Parameters.Add("@CVENUE", drH["CVENUE"]);
                    //                    cmd2.Parameters.Add("@CTEMPERATURE", drH["CTEMPERATURE"]);
                    //                    cmd2.Parameters.Add("@CWEATHERSTATUS", drH["WC_8"]);
                    //                    cmd2.Parameters.Add("@CREMARK", drH["CREMARK"]);
                    //                    cmd2.Parameters.Add("@TIMEFLAG", Convert.ToDateTime(drH["TIMEFLAG"]).ToString("yyyy-MM-dd HH:mm:ss.fff"));
                    //                    cmd2.Parameters.Add("@IDEST", drH["IDEST"]);
                    //                    int id = Convert.ToInt32(cmd2.ExecuteScalar());
                    //                    if (id > -1)
                    //                    {
                    //                        Files.WriteLog(" [Success] Insert ANALYSISBGREMARK " + " " + drH["CLEAGUEALIAS_OUTPUT_NAME"] + " " + drH["HKJCHOSTNAME_CN"] + "/" + drH["HKJCGUESTNAME_CN"]);
                    //                    }
                    //                }
                    //            }
                    //            catch (Exception exp)
                    //            {
                    //                Files.WriteError(DateTime.Now.ToString("HH:mm:ss ") + "ANALYSISBGREMARK,error1: " + exp);
                    //            }
                    //        }
                    //        connection2.Close();
                    //    }
                    //} 
                    connection.Close();
                }
            }
            catch (Exception exp)
            {
                Files.WriteError(DateTime.Now.ToString("HH:mm:ss ") + "Init LIVEGOALS,error: " + exp);
            }
        }

        private void TimerTask(object timerState)
        {
            try
            {
                InitGoalInfo();

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
                // configSetting.Clear();

                if (syncItems.Count() > 0)
                {
                    int i = 0;
                    try
                    {
                        using (FbConnection connection = new FbConnection(AppFlag.ScoutsDBConn))
                        {
                            connection.Open();
                            //ANALYSISBGREMARK 
                            Files.WriteLog("Update ANALYSISBGREMARK");
                            queryString = "SELECT g.WC_8,e.HKJCDAYCODE,e.HKJCMATCHNO,  E.CLEAGUEALIAS_OUTPUT_NAME,E.CLEAGUE_HKJC_NAME,E.HKJCHOSTNAME_CN, E.HKJCGUESTNAME_CN,E.CMATCHDATETIME CMATCHDATE, E.CMATCHDATETIME CMATCHTIME," +
                            " '' CHANDICAP,'H' CMATCHFIELD,'' CHOSTROOT,   ''CGUESTROOT,''CVENUE,'' CTEMPERATURE,'' CWEATHERSTATUS,''CREMARK,current_timestamp  TIMEFLAG,'0'IDEST FROM EVENTS r" +
                            " INNER JOIN  EMATCHES E ON E.EMATCHID = R.ID INNER JOIN EVENT_DETAILS G ON G.eventid = " +
                            " E.EMATCHID WHERE R.ID in(select E.EMATCHID from EMATCHES E   WHERE e.HKJCDAYCODE = '" + Day[Convert.ToInt32(DateTime.Now.DayOfWeek.ToString("d"))].ToString() + "' " +
                            " and cast(cast(CMATCHDATETIME as date) as varchar(10))>= cast(cast( current_timestamp as date) - 1 as varchar(10)) " +
                            //-- and cast(cast(CMATCHDATETIME as date) as varchar(10))<= cast(cast('29.04.2019' as date) + 1 as varchar(10))
                            " order by e.HKJCMATCHNO asc  ) ORDER BY E.HKJCMATCHNO asc ";
                            Files.WriteLog("Sql: " + queryString);

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
                            if (ds.Tables[0].Rows.Count > 0)
                            {
                                using (FbConnection connection2 = new FbConnection(AppFlag.MangoDBConn))
                                {
                                    connection2.Open();
                                    foreach (DataRow drH in ds.Tables[0].Rows)
                                    {
                                        try
                                        {
                                            using (FbCommand cmd2 = new FbCommand("PR_ANALYSISBGREMARK", connection2))
                                            {
                                                cmd2.CommandType = CommandType.StoredProcedure;
                                                cmd2.Parameters.Add("@CLEAGUE", drH["CLEAGUE_HKJC_NAME"]);
                                                cmd2.Parameters.Add("@CHOST", drH["HKJCHOSTNAME_CN"]);
                                                cmd2.Parameters.Add("@CGUEST", drH["HKJCGUESTNAME_CN"]);
                                                cmd2.Parameters.Add("@CMATCHDATE", Convert.ToDateTime(drH["CMATCHDATE"]).ToString("yyyyMMdd"));
                                                cmd2.Parameters.Add("@CMATCHTIME", Convert.ToDateTime(drH["CMATCHTIME"]).ToString("HHmm"));
                                                cmd2.Parameters.Add("@CHANDICAP", drH["CHANDICAP"]);
                                                cmd2.Parameters.Add("@CMATCHFIELD", drH["CMATCHFIELD"]);
                                                cmd2.Parameters.Add("@CHOSTROOT", drH["CHOSTROOT"]);
                                                cmd2.Parameters.Add("@CGUESTROOT", drH["CGUESTROOT"]);
                                                cmd2.Parameters.Add("@CVENUE", drH["CVENUE"]);
                                                cmd2.Parameters.Add("@CTEMPERATURE", drH["CTEMPERATURE"]);
                                                cmd2.Parameters.Add("@CWEATHERSTATUS", drH["WC_8"]);
                                                cmd2.Parameters.Add("@CREMARK", drH["CREMARK"]);
                                                cmd2.Parameters.Add("@TIMEFLAG", Convert.ToDateTime(drH["TIMEFLAG"]).ToString("yyyy-MM-dd HH:mm:ss.fff"));
                                                cmd2.Parameters.Add("@IDEST", drH["IDEST"]);
                                                int id = Convert.ToInt32(cmd2.ExecuteScalar());
                                                if (id > -1)
                                                {
                                                    Files.WriteLog(" [Success] Insert ANALYSISBGREMARK " + " " + drH["CLEAGUEALIAS_OUTPUT_NAME"] + " " + drH["HKJCHOSTNAME_CN"] + "/" + drH["HKJCGUESTNAME_CN"]);
                                                }
                                            }
                                        }
                                        catch (Exception exp)
                                        {
                                            Files.WriteError(DateTime.Now.ToString("HH:mm:ss ") + "ANALYSISBGREMARK,error1: " + exp);
                                        }
                                    }
                                    connection2.Close();
                                }
                            }

                            for (i = 0; i < syncItems.Length; i++)
                            {
                                //< item > RANKS </ item >
                                //< item > SCORERS </ item >
                                //< item > LEAGINFO </ item >
                                Files.WriteLog("Update " + syncItems[i].ToString());
                                this.listBox1.Invoke(new Action(() =>
                                {
                                    this.listBox1.Invoke(new Action(() => { { this.listBox1.Items.Insert(0, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ") + "Update " + syncItems[i].ToString()); } }));
                                }));
                                if (i == 0)
                                {
                                    // LEAGUE_INFO
                                    using (FbConnection connection2 = new FbConnection(AppFlag.MangoDBConn))
                                    {
                                        connection2.Open();
                                        queryString = "SELECT r.LEAG_ID, r.LEAGNAME, r.ORG, r.ALIAS, r.LEAGUETYPE, r.LEAG_ORDER,  r.CENGNAME, r.CMACAUNAME, r.CHKJCNAME, r.CHKJCSHORTNAME, r.LASTUPDATE,    r.TIMEFLAG FROM LEAGINFO r";
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
                                                        List<string> strL = data.Tables[0].AsEnumerable().Select(d => d.Field<string>("LEAG_ID")).ToList<string>().Distinct().ToList();
                                                        string strs = strL.Count == 0 ? "-1" : string.Concat("", string.Join(" , ", strL), "");

                                                        DataSet ds2 = new DataSet();
                                                        queryString = "select a.* from LEAGUE_INFO a  where a.ILEAG_ID  not in (" + strs + ") order by a.ILEAG_ID ";
                                                        using (FbCommand cmd2 = new FbCommand(queryString, connection))
                                                        {
                                                            using (FbDataAdapter fda2 = new FbDataAdapter(cmd2))
                                                            {
                                                                using (DataSet data2 = new DataSet())
                                                                {
                                                                    data2.Tables.Add(new DataTable("data"));
                                                                    fda2.Fill(data2.Tables["data"]);
                                                                    ds2 = data2;
                                                                }
                                                            }
                                                        }
                                                        if (ds2.Tables[0].Rows.Count > 0)
                                                        {
                                                            foreach (DataRow dr2 in ds2.Tables[0].Rows)
                                                            {
                                                                DataRow dr3 = data.Tables["data"].NewRow();
                                                                dr3[0] = dr2["ILEAG_ID"];
                                                                dr3[1] = dr2["LEAGUE_CHI_NAME"];
                                                                dr3[2] = DBNull.Value;
                                                                dr3[3] = dr2["CLEAGUE_ALIAS_NAME"];
                                                                dr3[4] = DBNull.Value;
                                                                dr3[5] = DBNull.Value;
                                                                dr3[6] = dr2["LEAGUE_ENG_NAME"];
                                                                dr3[7] = dr2["CLEAGUE_NAME"];
                                                                dr3[8] = dr2["CLEAGUE_NAME"];
                                                                dr3[9] = dr2["CLEAGUE_NAME"];
                                                                dr3[10] = DateTime.Now;
                                                                dr3[11] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                                                                data.Tables["data"].Rows.Add(dr3);
                                                            }
                                                            fda.Update(data.Tables["data"]);
                                                            Files.WriteLog("Insert LEAGUE_INFO [" + data.Tables[0].Rows.Count + "] ");
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        connection2.Close();
                                    }
                                }
                                else if (i == 1)
                                {
                                    //queryString = "SELECT 'c.id=''' || r.ID||'''' cid, r.id eid,  L.LEAGUE_CHI_NAME FROM COMPETITIONS r INNER JOIN  LEAGUE_INFO L ON R.ALIAS=L.CLEAGUE_ALIAS_NAME where r.ALIAS = '意甲' OR r.ALIAS = '英超' OR r.ALIAS = '法甲' OR r.ALIAS = '德甲' OR r.ALIAS = '蘇超' OR r.ALIAS = '西甲'OR r.ALIAS = '荷甲' OR r.ALIAS = '日聯' OR r.ALIAS = '澳A' OR r.ALIAS = '歐冠' ORDER BY  R.ID ";
                                    //Files.WriteLog("Sql: " + queryString);
                                    //using (FbCommand cmd = new FbCommand(queryString, connection))
                                    //{
                                    //    using (FbDataAdapter fda = new FbDataAdapter(cmd))
                                    //    {
                                    //        using (DataSet data = new DataSet())
                                    //        {
                                    //            data.Tables.Add(new DataTable("data"));
                                    //            fda.Fill(data.Tables["data"]);
                                    //            ds = data;
                                    //        }
                                    //    }
                                    //}

                                    //if (ds.Tables[0].Rows.Count > 0)
                                    //{
                                    using (FbConnection connection2 = new FbConnection(AppFlag.MangoDBConn))
                                    {
                                        connection2.Open();
                                        //List<string> strL = ds.Tables[0].AsEnumerable().Select(d => d.Field<string>("cid")).ToList<string>().Distinct().ToList().ConvertAll<string>(x => x.ToString());
                                        //string strs = strL.Count == 0 ? "-1" : string.Concat("(", string.Join(" or ", strL), ")");
                                        string strs = "";
                                        queryString = "SELECT  count (*) FROM FIXTURES r";
                                        using (FbCommand cmd = new FbCommand(queryString, connection2))
                                        {
                                            int count = Convert.ToInt32(cmd.ExecuteScalar());
                                            // if (count > 0) strs = strs + " and e.id in (select  c.EMATCHID FROM EMATCHES c where c.CTIMESTAMP >= '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "' AND C.EMATCHID IS NOT NULL   AND C.EMATCHID > 0 ORDER BY C.CTIMESTAMP ASC)";
                                            if (count > 0) strs = strs + " where e.id in ( select  c.id  FROM events c inner join  COMPETITIONS r on r.id =c.COMPETITION_ID  where  c.start_date >= current_timestamp -1 and (r.ALIAS = '意甲' OR r.ALIAS = '英超' OR r.ALIAS = '法甲' OR r.ALIAS = '德甲' OR r.ALIAS = '蘇超' OR r.ALIAS = '西甲'OR r.ALIAS = '荷甲' OR r.ALIAS = '日聯' OR r.ALIAS = '澳A' OR r.ALIAS = '歐冠')  ORDER BY C.start_date ASC)";
                                        }

                                        queryString = "select e.id eid, e.COMPETITION_ID,c.ALIAS,l.LEAGUE_CHI_NAME, (select  hkjc_name_cn from teams where id= e.HOME_ID and hkjc_name_cn is not null ) HOME_ID,(select hkjc_name_cn from teams where id= e.GUEST_ID and hkjc_name_cn is not null) GUEST_ID,e.START_DATE, G.H_GOAL||':'||G.G_GOAL RESULT ,G.HH_GOAL||':'||G.GH_GOAL RESULT2 from events e " +
                                            " inner join COMPETITIONS c on c.id = e.COMPETITION_ID   inner join LEAGUE_INFO l on l.CLEAGUE_ALIAS_NAME = c.ALIAS   inner JOIN GOALINFO G ON G.EMATCHID = E.ID  and   (c.ALIAS = '意甲' OR c.ALIAS = '英超' OR c.ALIAS = '法甲' OR c.ALIAS = '德甲' OR c.ALIAS = '蘇超' OR c.ALIAS = '西甲'OR c.ALIAS = '荷甲' OR c.ALIAS = '日聯' OR c.ALIAS = '澳A' OR c.ALIAS = '歐冠') " +
                                               strs + " order by e.start_date asc"; //" (c.id = 1507 or c.id = 1519 or c.id = 1528 or c.id = 1556 or c.id = 1599 or c.id = 1625 or c.id = 1639 or c.id = 1658 or c.id = 2131 or c.id = 2202) ";
                                        Files.WriteLog("Sql: " + queryString);

                                        using (FbCommand cmd = new FbCommand(queryString, connection))
                                        {
                                            using (FbDataAdapter fda = new FbDataAdapter(cmd))
                                            {
                                                using (DataSet data = new DataSet())
                                                {
                                                    data.Tables.Add(new DataTable("data"));
                                                    fda.Fill(data.Tables["data"]);

                                                    string ids = string.Concat("(", string.Join(") OR  (", data.Tables[0].AsEnumerable().Select(d => d.Field<int>("eid")).ToList<int>().Distinct().ToList().ConvertAll<string>(x => x.ToString())), ")");
                                                    Files.WriteLog("Sync ids: " + ids);

                                                    foreach (DataRow dr in data.Tables[0].Rows)
                                                    {
                                                        using (FbCommand cmd2 = new FbCommand("PR_ADDFIXTURE", connection2))
                                                        {
                                                            cmd2.CommandType = CommandType.StoredProcedure;
                                                            cmd2.Parameters.Add("@CLEAGUEALIAS", dr["ALIAS"]);
                                                            cmd2.Parameters.Add("@CLEAGUE", dr["LEAGUE_CHI_NAME"]);
                                                            cmd2.Parameters.Add("@CHOST", dr["HOME_ID"] == DBNull.Value ? "" : dr["HOME_ID"]);
                                                            cmd2.Parameters.Add("@CJCHOST", dr["HOME_ID"] == DBNull.Value ? "" : dr["HOME_ID"]);
                                                            cmd2.Parameters.Add("@CGUEST", dr["GUEST_ID"] == DBNull.Value ? "" : dr["GUEST_ID"]);
                                                            cmd2.Parameters.Add("@CJCGUEST", dr["GUEST_ID"] == DBNull.Value ? "" : dr["GUEST_ID"]);
                                                            cmd2.Parameters.Add("@CMATCHFIELD", "H");
                                                            cmd2.Parameters.Add("@CMATCHDATE", Convert.ToDateTime(dr["START_DATE"]).ToString("yyyyMMdd"));
                                                            cmd2.Parameters.Add("@CMATCHTIME", Convert.ToDateTime(dr["START_DATE"]).ToString("HHmm"));
                                                            cmd2.Parameters.Add("@CHALF_TIME", dr["RESULT2"] == DBNull.Value || dr["RESULT2"].ToString() == "0:0" ? "-1" : dr["RESULT2"]);
                                                            cmd2.Parameters.Add("@CFULL_TIME", dr["RESULT"] == DBNull.Value ? "-1" : dr["RESULT"]);
                                                            cmd2.Parameters.Add("@CREMARK", "");
                                                            cmd2.Parameters.Add("@CACTION", "U");
                                                            cmd2.Parameters.Add("@CSCORER", "");
                                                            cmd2.Parameters.Add("@TIMEFLAG", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                                                            cmd2.Parameters.Add("@IDEST", "0");
                                                            int id = Convert.ToInt32(cmd2.ExecuteScalar());
                                                            if (id > -1)
                                                            {
                                                              //  Files.WriteLog(" [Success] Insert FIXTURE" + " " + dr["ALIAS"] + " " + dr["HOME_ID"] + "/" + dr["GUEST_ID"]);
                                                                Files.WriteLog(" [Success] Insert FIXTURE" + " " + dr["ALIAS"] + " " + dr["HOME_ID"] + "/" + dr["GUEST_ID"] + " " + Convert.ToDateTime(dr["START_DATE"]).ToString("yyyy-MM-dd HH:mm:ss"));
                                                            }
                                                        }
                                                    }

                                                }
                                            }
                                        }
                                        connection2.Close();
                                    }
                                    //}
                                }
                                else if (i == 2 || i == 3)
                                {
                                    // RANKS/SCORERS
                                    // queryString = "SELECT  c.id, c.ALIAS,l.ILEAG_ID  FROM COMPETITIONS c inner join LEAGUE_INFO l on l.CLEAGUE_ALIAS_NAME=c.alias where c.ALIAS is not null ";
                                    //  queryString = "SELECT  c.id, c.ALIAS   FROM COMPETITIONS c inner join LEAGUE_INFO r on r.CLEAGUE_ALIAS_NAME=c.alias   where c.ALIAS is not null ";
                                    queryString = "SELECT c.id, c.ALIAS cALIAS, r.LEAGUE_CHI_NAME ALIAS FROM COMPETITIONS c inner join LEAGUE_INFO r on r.CLEAGUE_ALIAS_NAME = c.alias   where c.ALIAS is not null ";
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
                                        if (i == 2)
                                        {
                                            queryString = "SELECT T.SHORT_NAME  , c.name, a.LEAG_ID, a.CLEAG_ALIAS,l.LEAGUE_CHI_NAME, a.SEASON_ID, a.TEAM_ID, a.TEAM, a.HKJC_TEAM, a.SCORE, a.RANK, a.FLAG, a.GAMES, a.IWON, a.IDRAW,a.ILOST," +
                                                "a.CTIMESTAMP,T.HKJC_NAME_CN FROM LEAGRANKINFO a inner join LEAGUE_INFO l on l.CLEAGUE_ALIAS_NAME = a.CLEAG_ALIAS  inner join SEASONS s on s.id = a.SEASON_ID and ( s.SYEAR = '" + DateTime.Now.ToString("yyyy") + "' or s.SYEAR = '" + DateTime.Now.ToString("yyyy") + "/" + DateTime.Now.AddYears(1).ToString("yy") + "') " +
                                                "inner join teams t on t.id = a.team_id   inner join areas c on c.id = t.area_id where  a.LEAG_ID=" + dr["id"].ToString() + " order by a.CLEAG_ALIAS ,a.rank asc ";
                                        }
                                        else if (i == 3)
                                        {
                                            queryString = "SELECT  first 15 distinct  t.SHORT_NAME tname,t.HKJC_NAME_CN tcname ,r.CLEAG_ID, r.CLEAG_ALIAS,l.LEAGUE_CHI_NAME,  r.SEASON_ID,  r.PLAYER_ID,r.CPLAYER_NAME, r.CTEAM_ABBR, r.CACT," +
                                            " r.IRID, r.IRANK, r.IGOALS, r.UT, r.CTIMESTAMP ,t.short_name tname, r.CPLAYER_NAME_CN pcname ,r.CPLAYER_NAME_cn FROM SCORERS_INFO r  inner join LEAGUE_INFO l on l.CLEAGUE_ALIAS_NAME = r.CLEAG_ALIAS   inner join SEASONS s on s.id = r.SEASON_ID and ( s.SYEAR = '" + DateTime.Now.ToString("yyyy") + "' or s.SYEAR = '" + DateTime.Now.ToString("yyyy") + "/" + DateTime.Now.AddYears(1).ToString("yy") + "')  " +
                                            " inner join teams t on t.id = r.TEAM_ID" +
                                            //20191115 "  left join  PLAYERS_INFO p  on p.PLAYER_ID = r.PLAYER_ID and p.TEAM_ID = r.TEAM_ID " +     // p.CPLAYER_NAME pcname
                                            " where r.CLEAG_ID = " + dr["id"].ToString() + "  order by r.IRANK asc";
                                        }
                                        //else if (i==2)
                                        //{
                                        //    queryString = "SELECT  r.ILEAG_ID, r.CLEAGUE_ALIAS_NAME, r.CLEAGUE_NAME,    r.LEAGUE_CHI_NAME, r.LEAGUE_ENG_NAME, r.CLEAGUE_SHT_ENG_NAME,    r.COUNTRY_CHI_NAME, r.CTIMESTAMP FROM LEAGUE_INFO r inner join COMPETITIONS c on c.alias =r.CLEAGUE_ALIAS_NAME where c.id='"+ dr["id"].ToString() + "' order by r.ILEAG_ID asc ";
                                        //}
                                        Files.WriteLog("Sql:RANKS/SCORERS " + queryString);
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
                                                if (i == 2)
                                                {
                                                    //ranks
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
                                                                        //if (dr2["SHORT_NAME"].ToString() == "Brisbane Roar")
                                                                        //{
                                                                        //    string ST = "";
                                                                        //}
                                                                        DataRow dr3 = data.Tables["data"].NewRow();
                                                                        dr3[0] = dr2["LEAGUE_CHI_NAME"];
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
                                                                        dr3[12] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
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
                                                else if (i == 3)
                                                {
                                                    //SCORERS
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
                                                                        dr3[0] = dr2["LEAGUE_CHI_NAME"];
                                                                        dr3[1] = DateTime.Now.ToString("yyyy/MM/dd");
                                                                        dr3[2] = dr2["IRANK"];
                                                                        dr3[3] = dr2["CPLAYER_NAME_cn"].Equals(DBNull.Value) || dr2["CPLAYER_NAME_cn"].Equals("") ? (dr2["pcname"].Equals(DBNull.Value) || dr2["pcname"].Equals("") ? dr2["CPLAYER_NAME"] : dr2["pcname"]) : dr2["CPLAYER_NAME_cn"];
                                                                        dr3[4] = dr2["tcname"] == DBNull.Value || dr2["tcname"].ToString() == "" ? dr2["tname"] : dr2["tcname"];
                                                                        dr3[5] = dr2["IGOALS"];
                                                                        dr3[6] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
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
                                //else if (i == 2)
                                //{
                                //    // LEAGUE_INFO
                                //    using (FbConnection connection2 = new FbConnection(AppFlag.MangoDBConn))
                                //    {
                                //        connection2.Open();
                                //        queryString = "SELECT r.LEAG_ID, r.LEAGNAME, r.ORG, r.ALIAS, r.LEAGUETYPE, r.LEAG_ORDER,  r.CENGNAME, r.CMACAUNAME, r.CHKJCNAME, r.CHKJCSHORTNAME, r.LASTUPDATE,    r.TIMEFLAG FROM LEAGINFO r";
                                //        using (FbCommand cmd = new FbCommand(queryString, connection2))
                                //        {
                                //            using (FbDataAdapter fda = new FbDataAdapter(cmd))
                                //            {
                                //                using (FbCommandBuilder fcb = new FbCommandBuilder(fda))
                                //                {
                                //                    using (DataSet data = new DataSet())
                                //                    {
                                //                        data.Tables.Add(new DataTable("data"));
                                //                        fda.Fill(data.Tables["data"]);
                                //                        List<string> strL = data.Tables[0].AsEnumerable().Select(d => d.Field<string>("LEAG_ID")).ToList<string>().Distinct().ToList();
                                //                        string strs = strL.Count == 0 ? "-1" : string.Concat("", string.Join(" , ", strL), "");

                                //                        DataSet ds2 = new DataSet();
                                //                        queryString = "select a.* from LEAGUE_INFO a  where a.ILEAG_ID  not in (" + strs + ") order by a.ILEAG_ID ";
                                //                        using (FbCommand cmd2 = new FbCommand(queryString, connection))
                                //                        {
                                //                            using (FbDataAdapter fda2 = new FbDataAdapter(cmd2))
                                //                            {
                                //                                using (DataSet data2 = new DataSet())
                                //                                {
                                //                                    data2.Tables.Add(new DataTable("data"));
                                //                                    fda2.Fill(data2.Tables["data"]);
                                //                                    ds2 = data2;
                                //                                }
                                //                            }
                                //                        }
                                //                        if (ds2.Tables[0].Rows.Count > 0)
                                //                        {
                                //                            foreach (DataRow dr2 in ds2.Tables[0].Rows)
                                //                            {
                                //                                DataRow dr3 = data.Tables["data"].NewRow();
                                //                                dr3[0] = dr2["ILEAG_ID"];
                                //                                dr3[1] = dr2["LEAGUE_CHI_NAME"];
                                //                                dr3[2] = DBNull.Value;
                                //                                dr3[3] = dr2["CLEAGUE_ALIAS_NAME"];
                                //                                dr3[4] = DBNull.Value;
                                //                                dr3[5] = DBNull.Value;
                                //                                dr3[6] = dr2["LEAGUE_ENG_NAME"];
                                //                                dr3[7] = dr2["CLEAGUE_NAME"];
                                //                                dr3[8] = dr2["CLEAGUE_NAME"];
                                //                                dr3[9] = dr2["CLEAGUE_NAME"];
                                //                                dr3[10] = DateTime.Now;
                                //                                dr3[11] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                                //                                data.Tables["data"].Rows.Add(dr3);
                                //                            }
                                //                            fda.Update(data.Tables["data"]);
                                //                            Files.WriteLog("Insert LEAGUE_INFO [" + data.Tables[0].Rows.Count + "] ");
                                //                        }
                                //                    }
                                //                }
                                //            }
                                //        }
                                //        connection2.Close();
                                //    }
                                //}
                                int msg = -1;
                                if (AppFlag.Alert)
                                {
                                    switch (syncItems[i])
                                    {
                                        case "LIVEGOALS":
                                            {
                                                msg = 30;
                                            }
                                            break;
                                        case "GOALDETAILS":
                                            {
                                                msg = 31;
                                            }
                                            break;
                                        case "ANALYSISOTHER":
                                            {
                                                msg = 62;
                                            }
                                            break;
                                        case "FIXTURES":
                                            {
                                                msg = 63;
                                            }
                                            break;
                                        case "ANALYSISTATS":
                                            {
                                                msg = 14;
                                            }
                                            break;
                                        case "ANALYSISHISTORYS":
                                            {
                                                msg = 11;
                                            }
                                            break;
                                        case "RANKS":
                                            {
                                                msg = 15;
                                            }
                                            break;
                                        case "SCORERS":
                                            {
                                                msg = 17;
                                            }
                                            break;
                                        case "ANALYSISRECENTS":
                                            {
                                                msg = 13;
                                            }
                                            break;
                                        case "HKGOAL":
                                            {
                                                msg = 60;
                                            }
                                            break;
                                        case "HKGOALDETAILS":
                                            {
                                                msg = 61;
                                            }
                                            break;
                                        case "ANALYSISPLAYERLIST":
                                            {
                                                msg = 12;
                                            }
                                            break;
                                    }
                                    if (msg != -1)
                                    {
                                        SendAlertMsg(msg);
                                        Files.WriteLog(DateTime.Now.ToString("HH:mm:ss ") + "Send " + msg);
                                    }
                                }
                            }
                            connection.Close();
                        }
                    }
                    catch (Exception exp)
                    {
                        Files.WriteError(DateTime.Now.ToString("HH:mm:ss ") + syncItems[i] + ",error: " + exp);
                    }
                }


                var state = timerState as TimerState;
                Interlocked.Increment(ref state.Counter);
                Files.WriteLog(" TimerTask " + state.Counter + " " + DateTime.Now);
            }
            catch (Exception exp)
            {
                Files.WriteError("TimerTask(),error: " + exp.Message);
            }
        }

        public struct COPYDATASTRUCT
        {
            public IntPtr dwData;
            public int cbData;
            [MarshalAs(UnmanagedType.LPStr)]
            public string lpData;

        }

        //private void SendToLiveDB(string ml)
        //{
        //    try
        //    {
        //        string queryString = "";
        //        DataSet ds = new DataSet();
        //        using (FbConnection connection = new FbConnection(AppFlag.ScoutsDBConn))
        //        {
        //            connection.Open();
        //            Files.WriteLog("Update LIVEGOALS/GOALDETAILS[ToLive] " + ml.ToString());
        //            this.listBox1.Invoke(new Action(() =>
        //            {
        //                this.listBox1.Invoke(new Action(() => { { this.listBox1.Items.Insert(0, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ") + "Start sync LIVEGOALS/GOALDETAILS." + ml.ToString()); } }));
        //            }));

        //            //LIVEGOALS
        //            queryString = "SELECT FIRST 1   (select  FIRST 1  t.HKJC_NAME_CN from teams t  where t.id= r.HOME_ID) H,(select  FIRST 1  t.HKJC_NAME_CN from teams t  where t.id=r.GUEST_ID) G,  E.CLEAGUEALIAS_OUTPUT_NAME,E.CLEAGUE_HKJC_NAME,E.HKJCHOSTNAME_CN, E.HKJCGUESTNAME_CN," +
        //              " E.MAPPINGSTATUS, ( CASE WHEN E.MAPPINGSTATUS IS TRUE THEN E.HKJCGUESTNAME_CN ELSE E.HKJCHOSTNAME_CN END ) HH,  ( CASE WHEN E.MAPPINGSTATUS IS TRUE THEN E.HKJCHOSTNAME_CN ELSE E.HKJCGUESTNAME_CN END ) GG, " +
        //              " E.CMATCHDATETIME CMATCHDATE,E.CMATCHDATETIME CMATCHTIME,'H' CMATCHFIELD, 'U' CACTION,G.H_GOAL,G.G_GOAL,G.H_RED,G.G_RED,G.HH_GOAL,G.GH_GOAL,G.H_CONFIRM,G.G_CONFIRM,''CSONGID,''CALERT,G.GAMESTATUS, " +
        //              " ((select count(*) from MATCHDETAILS e where e.EMATCHID= " + ml.ToString() + " and (e.STATUS='Extratime 1st half' or e.STATUS='Extratime 2nd half') and e.CTYPE='goal' and e.TEAM_ID=r.HOME_ID)||'-'||" +
        //              " (select count(*) from MATCHDETAILS e where e.EMATCHID = " + ml.ToString() + " and(e.STATUS = 'Extratime 1st half' or e.STATUS = 'Extratime 2nd half') and e.CTYPE = 'goal' and e.TEAM_ID = r.GUEST_ID) || ';' ||" +
        //              " (select count(*) from MATCHDETAILS e where e.EMATCHID = " + ml.ToString() + " and(e.STATUS = 'Penalty shootout') and e.CTYPE = 'goal' and e.TEAM_ID = r.HOME_ID)|| '-' ||" +
        //              " (select count(*) from MATCHDETAILS e where e.EMATCHID = " + ml.ToString() + " and(e.STATUS = 'Penalty shootout') and e.CTYPE = 'goal' and e.TEAM_ID = r.GUEST_ID) )" +
        //              " CCOMMENT,g.ELAPSED CTIMEOFGAME,current_timestamp TIMEFLAG,'0'IDEST FROM EVENTS r INNER JOIN " +
        //              " EMATCHES E ON E.EMATCHID = R.ID " +
        //              " left JOIN GOALINFO G ON G.EMATCHID = E.EMATCHID WHERE R.ID =" + ml.ToString() + " and r.START_DATE > dateadd (-" + AppFlag.LivePeriod + " day to current_timestamp)  ORDER BY E.CTIMESTAMP DESC ";

        //            Files.WriteLog("Sql: " + queryString);

        //            using (FbCommand cmd = new FbCommand(queryString, connection))
        //            {
        //                using (FbDataAdapter fda = new FbDataAdapter(cmd))
        //                {
        //                    using (DataSet data = new DataSet())
        //                    {
        //                        data.Tables.Add(new DataTable("data"));
        //                        fda.Fill(data.Tables["data"]);
        //                        ds = data;
        //                    }
        //                }
        //            }

        //            if (ds.Tables[0].Rows.Count > 0)
        //            {
        //                DataRow drH = ds.Tables[0].Rows[0];
        //                //if (drH["CLEAGUE_HKJC_NAME"] is DBNull || drH["CLEAGUEALIAS_OUTPUT_NAME"] is DBNull
        //                //    || drH["HKJCHOSTNAME_CN"] is DBNull || drH["HKJCGUESTNAME_CN"] is DBNull
        //                //    || drH["H"] is DBNull || drH["G"] is DBNull) { }
        //                //else
        //                //{
        //                try
        //                {
        //                    using (FbConnection connection2 = new FbConnection(AppFlag.ToLiveDB))
        //                    {
        //                        connection2.Open();
        //                        using (FbCommand cmd2 = new FbCommand("PR_LIVEGOALS", connection2))
        //                        {
        //                            cmd2.CommandType = CommandType.StoredProcedure;
        //                            cmd2.Parameters.Add("@CLEAGUEALIAS", drH["CLEAGUEALIAS_OUTPUT_NAME"]);
        //                            cmd2.Parameters.Add("@CLEAGUE", drH["CLEAGUE_HKJC_NAME"]);
        //                            cmd2.Parameters.Add("@CHOST", drH["H"] is DBNull ? drH["HH"] : drH["H"]);
        //                            cmd2.Parameters.Add("@CGUEST", drH["G"] is DBNull ? drH["GG"] : drH["G"]);
        //                            cmd2.Parameters.Add("@CMATCHDATE", Convert.ToDateTime(drH["CMATCHDATE"]).ToString("yyyyMMdd"));
        //                            cmd2.Parameters.Add("@CMATCHTIME", Convert.ToDateTime(drH["CMATCHTIME"]).ToString("HHmm"));
        //                            cmd2.Parameters.Add("@CMATCHFIELD", drH["CMATCHFIELD"]);
        //                            cmd2.Parameters.Add("@CACTION", drH["CACTION"]);
        //                            cmd2.Parameters.Add("@IH_GOAL", drH["H_GOAL"].ToString() == "" ? "0" : drH["H_GOAL"].ToString());
        //                            cmd2.Parameters.Add("@IG_GOAL", drH["G_GOAL"].ToString() == "" ? "0" : drH["G_GOAL"].ToString());
        //                            cmd2.Parameters.Add("@IH_REDCARD", drH["H_RED"].ToString() == "" ? "0" : drH["H_RED"].ToString());
        //                            cmd2.Parameters.Add("@IG_REDCARD", drH["G_RED"].ToString() == "" ? "0" : drH["G_RED"].ToString());
        //                            cmd2.Parameters.Add("@IH_HT_GOAL", drH["HH_GOAL"].ToString() == "" ? "0" : drH["HH_GOAL"].ToString());
        //                            cmd2.Parameters.Add("@IG_HT_GOAL", drH["GH_GOAL"].ToString() == "" ? "0" : drH["GH_GOAL"].ToString());
        //                            //cmd2.Parameters.Add("@IH_CONFIRM", drH["H_CONFIRM"].ToString() == "" ? "-1" : drH["H_CONFIRM"].ToString());
        //                            //cmd2.Parameters.Add("@IG_CONFIRM", drH["G_CONFIRM"].ToString() == "" ? "-1" : drH["G_CONFIRM"].ToString());
        //                            cmd2.Parameters.Add("@CSONGID", drH["CSONGID"]);
        //                            cmd2.Parameters.Add("@CALERT", drH["CALERT"]);
        //                            cmd2.Parameters.Add("@CSTATUS", drH["GAMESTATUS"]);
        //                            cmd2.Parameters.Add("@CCOMMENT", drH["CCOMMENT"].ToString().Replace("0-0;0-0", "").Replace(";", ";PK").Replace(";PK0-0", ""));
        //                            // cmd2.Parameters.Add("@CTIMEOFGAME", drH["CTIMEOFGAME"]); 
        //                            cmd2.Parameters.Add("@CTIMEOFGAME", drH["CTIMEOFGAME"] is DBNull ? "-1" : drH["CTIMEOFGAME"].ToString());
        //                            cmd2.Parameters.Add("@TIMEFLAG", Convert.ToDateTime(drH["TIMEFLAG"]).ToString("yyyy-MM-dd HH:mm:ss.fff"));
        //                            cmd2.Parameters.Add("@IDEST", drH["IDEST"]);
        //                            int id = Convert.ToInt32(cmd2.ExecuteScalar());
        //                            // if (id > 0)
        //                            //  {
        //                            Files.WriteLog(" [Success] " + (id > 0 ? "Insert" : "Update") + " LIVEGOALS " + " " + drH["CLEAGUEALIAS_OUTPUT_NAME"] + " " + drH["HKJCHOSTNAME_CN"] + "/" + drH["HKJCGUESTNAME_CN"]);
        //                            //  } 
        //                            Files.WriteLog("Sql: " + cmd2.CommandText + " " + drH["CLEAGUEALIAS_OUTPUT_NAME"] + " " + drH["CLEAGUE_HKJC_NAME"] + " " + drH["HKJCHOSTNAME_CN"] + " " + drH["HKJCGUESTNAME_CN"] + " " + Convert.ToDateTime(drH["CMATCHDATE"]).ToString("yyyyMMdd") + " " + Convert.ToDateTime(drH["CMATCHDATE"]).ToString("HHmm") + " " + drH["GAMESTATUS"]);
        //                        }
        //                        connection2.Close();
        //                    }
        //                }
        //                catch (Exception exp)
        //                {
        //                    Files.WriteError(DateTime.Now.ToString("HH:mm:ss ") + "LIVEGOALS[ToLive],error2: " + exp);
        //                }
        //                //}
        //            }
        //            SendAlertMsg(1030);

        //            //GOALDETAILS 
        //            Files.WriteLog("Update GOALDETAILS " + ml.ToString());
        //            queryString = " SELECT distinct  (select  FIRST 1  t.HKJC_NAME_CN from teams t  where t.id=s.HOME_ID) H,(select  FIRST 1  t.HKJC_NAME_CN from teams t  where t.id=s.GUEST_ID) G, E.CLEAGUEALIAS_OUTPUT_NAME,E.CLEAGUE_HKJC_NAME,E.HKJCHOSTNAME_CN, E.HKJCGUESTNAME_CN," +
        //                        " 'F' CCURRENTSTATUS, '' CPK, 'U' CACTION, '' CALERT, R.CTYPE CRECORDTYPE, R.HG CRECORDBELONG,  r.STATUS CMATCHSTATUS, r.ELAPSED , (select count(*) from MATCHDETAILS x where x.EMATCHID=g.EMATCHID and cast( x.ELAPSED as integer)<=cast( r.ELAPSED as integer) and x.CTYPE='goal' and x.HG='H'AND (x.STATUS!='Penalty shootout' AND x.ELAPSED!=105)  and (x.STATUS!='Extratime 1st half' and x.STATUS!='Extratime 2nd half'))  CSCOREHOST, (select count(*) from MATCHDETAILS x where x.EMATCHID=g.EMATCHID and cast( x.ELAPSED as integer)<=cast( r.ELAPSED as integer) and x.CTYPE='goal' and x.HG='G' AND (x.STATUS!='Penalty shootout' AND x.ELAPSED!=105)  and (x.STATUS!='Extratime 1st half' and x.STATUS!='Extratime 2nd half'))  CSCOREGUEST, '-1' CSCORENUM,   '0' CSCOREOWNGOAL, r.PLAYERCHI CSCORER,r.PLAYER CSCORER2 , current_timestamp TIMEFLAG, '0' IDEST " +
        //                        "FROM MATCHDETAILS r   INNER JOIN  EMATCHES E ON E.EMATCHID = r.EMATCHID   INNER JOIN  events s ON s.id = r.EMATCHID   INNER JOIN GOALINFO G ON G.EMATCHID= E.EMATCHID where r.EMATCHID =" + ml.ToString() + " AND (R.CTYPE='goal'  or r.CTYPE='rcard') AND (R.STATUS!='Penalty shootout' AND R.ELAPSED!=105) and ( r.STATUS!='Extratime 1st half' and r.STATUS!='Extratime 2nd half') AND   E.CMATCHDATETIME > dateadd (-" + AppFlag.LivePeriod + " day to current_timestamp)  order by r.ELAPSED asc";
        //            Files.WriteLog("Sql: " + queryString);

        //            using (FbCommand cmd = new FbCommand(queryString, connection))
        //            {
        //                using (FbDataAdapter fda = new FbDataAdapter(cmd))
        //                {
        //                    using (DataSet data = new DataSet())
        //                    {
        //                        data.Tables.Add(new DataTable("data"));
        //                        fda.Fill(data.Tables["data"]);
        //                        ds = data;
        //                    }
        //                }
        //            }

        //            if (ds.Tables[0].Rows.Count > 0)
        //            {
        //                using (FbConnection connection2 = new FbConnection(AppFlag.ToLiveDB))
        //                {
        //                    connection2.Open();
        //                    queryString = "delete from GOALDETAILS g where g.CLEAGUE='" + ds.Tables[0].Rows[0]["CLEAGUE_HKJC_NAME"].ToString() + "' and( (g.CHOST='" + ds.Tables[0].Rows[0]["HKJCHOSTNAME_CN"].ToString() + "'  and g.CGUEST='" + ds.Tables[0].Rows[0]["HKJCGUESTNAME_CN"].ToString() + "') OR (g.CHOST='" + ds.Tables[0].Rows[0]["HKJCGUESTNAME_CN"].ToString() + "'  and g.CGUEST='" + ds.Tables[0].Rows[0]["HKJCGUESTNAME_CN"].ToString() + "')) ";
        //                    using (FbCommand cmd2 = new FbCommand(queryString, connection2))
        //                    {
        //                        int id = Convert.ToInt32(cmd2.ExecuteScalar());
        //                        Files.WriteLog("Sql: " + queryString);
        //                    }

        //                    foreach (DataRow drH in ds.Tables[0].Rows)
        //                    {
        //                        try
        //                        {
        //                            if (drH["CLEAGUEALIAS_OUTPUT_NAME"] is DBNull || drH["CLEAGUE_HKJC_NAME"] is DBNull
        //                                || drH["HKJCHOSTNAME_CN"] is DBNull || drH["HKJCGUESTNAME_CN"] is DBNull)
        //                            //  || drH["H"] is DBNull || drH["G"] is DBNull)
        //                            { continue; }
        //                            using (FbCommand cmd2 = new FbCommand("PR_GOALDETAILS", connection2))
        //                            {
        //                                cmd2.CommandType = CommandType.StoredProcedure;
        //                                cmd2.Parameters.Add("@CLEAGUEALIAS", drH["CLEAGUEALIAS_OUTPUT_NAME"]);
        //                                cmd2.Parameters.Add("@CLEAGUE", drH["CLEAGUE_HKJC_NAME"]);
        //                                cmd2.Parameters.Add("@CHOST", drH["H"] is DBNull ? drH["HKJCHOSTNAME_CN"] : drH["H"]);
        //                                cmd2.Parameters.Add("@CGUEST", drH["G"] is DBNull ? drH["HKJCGUESTNAME_CN"] : drH["G"]);
        //                                cmd2.Parameters.Add("@CCURRENTSTATUS", drH["CCURRENTSTATUS"]);
        //                                cmd2.Parameters.Add("@CPK", drH["CPK"]);
        //                                cmd2.Parameters.Add("@CACTION", drH["CACTION"]);
        //                                cmd2.Parameters.Add("@CALERT", drH["CALERT"]);
        //                                cmd2.Parameters.Add("@CRECORDTYPE", drH["CRECORDTYPE"]);
        //                                cmd2.Parameters.Add("@CRECORDBELONG", drH["CRECORDBELONG"]);
        //                                cmd2.Parameters.Add("@CMATCHSTATUS", drH["CMATCHSTATUS"]);
        //                                cmd2.Parameters.Add("@CMATCHTIME", drH["ELAPSED"]);
        //                                cmd2.Parameters.Add("@CSCOREHOST", drH["CSCOREHOST"]);
        //                                cmd2.Parameters.Add("@CSCOREGUEST", drH["CSCOREGUEST"]);
        //                                cmd2.Parameters.Add("@CSCORENUM", drH["CSCORENUM"]);
        //                                cmd2.Parameters.Add("@CSCOREOWNGOAL", drH["CSCOREOWNGOAL"]);
        //                                cmd2.Parameters.Add("@CSCORER", drH["CSCORER"] is DBNull ? drH["CSCORER2"].ToString() : drH["CSCORER"].ToString());
        //                                cmd2.Parameters.Add("@TIMEFLAG", Convert.ToDateTime(drH["TIMEFLAG"]).ToString("yyyy-MM-dd HH:mm:ss.fff"));
        //                                cmd2.Parameters.Add("@IDEST", drH["IDEST"]);
        //                                int id = Convert.ToInt32(cmd2.ExecuteScalar());
        //                                if (id > -1)
        //                                {
        //                                    Files.WriteLog(" [Success] Insert/Update GOALDETAILS " + " " + drH["CLEAGUEALIAS_OUTPUT_NAME"] + " " + drH["HKJCHOSTNAME_CN"] + "/" + drH["HKJCGUESTNAME_CN"]);
        //                                }
        //                            }
        //                        }
        //                        catch (Exception exp)
        //                        {
        //                            Files.WriteError(DateTime.Now.ToString("HH:mm:ss ") + "GOALDETAILS[ToLive],error1: " + exp);
        //                            continue;
        //                        }
        //                    }
        //                    connection2.Close();
        //                }
        //            }
        //            else
        //            {
        //                queryString = "SELECt r.EMATCHID,  r.CMATCHDATETIME,    r.MAPPINGSTATUS,   r.HKJCHOSTNAME_CN, r.HKJCGUESTNAME_CN,r.CLEAGUE_HKJC_NAME, r.CLEAGUEALIAS_OUTPUT_NAME, " +
        //                      " ( CASE WHEN r.MAPPINGSTATUS IS TRUE THEN r.HKJCGUESTNAME_CN ELSE r.HKJCHOSTNAME_CN END ) HH,  ( CASE WHEN r.MAPPINGSTATUS IS TRUE THEN r.HKJCHOSTNAME_CN ELSE r.HKJCGUESTNAME_CN END ) GG," +
        //                      " (select  FIRST 1  t.HKJC_NAME_CN from teams t  where t.id= e.HOME_ID) H,(select  FIRST 1  t.HKJC_NAME_CN from teams t  where t.id=e.GUEST_ID) G FROM EMATCHES r inner join events e on e.id =r.EMATCHID " +
        //                      " where r.EMATCHID =" + ml.ToString();
        //                Files.WriteLog("Sql2: " + queryString);

        //                using (FbCommand cmd = new FbCommand(queryString, connection))
        //                {
        //                    using (FbDataAdapter fda = new FbDataAdapter(cmd))
        //                    {
        //                        using (DataSet data = new DataSet())
        //                        {
        //                            data.Tables.Add(new DataTable("data"));
        //                            fda.Fill(data.Tables["data"]);
        //                            ds = data;
        //                        }
        //                    }
        //                }
        //                if (ds.Tables[0].Rows.Count > 0)
        //                {
        //                    using (FbConnection connection2 = new FbConnection(AppFlag.ToLiveDB))
        //                    {
        //                        connection2.Open();
        //                        queryString = "delete from GOALDETAILS g where g.CLEAGUE='" + ds.Tables[0].Rows[0]["CLEAGUE_HKJC_NAME"].ToString() + "' and ((g.CHOST='" + ds.Tables[0].Rows[0]["HKJCHOSTNAME_CN"].ToString() + "'  and g.CGUEST='" + ds.Tables[0].Rows[0]["HKJCGUESTNAME_CN"].ToString() + "') OR (g.CHOST='" + ds.Tables[0].Rows[0]["HKJCGUESTNAME_CN"].ToString() + "'  and g.CGUEST='" + ds.Tables[0].Rows[0]["HKJCHOSTNAME_CN"].ToString() + "')) ";
        //                        using (FbCommand cmd2 = new FbCommand(queryString, connection2))
        //                        {
        //                            int id = Convert.ToInt32(cmd2.ExecuteScalar());
        //                            Files.WriteLog("Sql2: " + queryString);
        //                        }
        //                        connection2.Close();
        //                    }
        //                }
        //            }
        //            connection.Close();
        //            SendAlertMsg(1031);
        //        }
        //    }
        //    catch (Exception exp)
        //    {
        //        Files.WriteError(DateTime.Now.ToString("HH:mm:ss ") + "LIVEGOALS/GOALDETAILS[ToLive],error: " + exp);
        //    } 
        //}

        private void SendToLiveDB(string League, string Host, string Guest, string MatchDate, string MatchTime)
        {
            if (AppFlag.TestToLive)
            {

            }

            string queryString = "";
            DataSet ds = new DataSet();
            try
            {
                using (FbConnection connection = new FbConnection(AppFlag.MangoDBConn))
                {
                    connection.Open();
                    Files.WriteLog("Update LIVEGOALS/GOALDETAILS[ToLive] " + League + " " + Host + " " + Guest);
                    this.listBox1.Invoke(new Action(() =>
                    {
                        this.listBox1.Invoke(new Action(() => { { this.listBox1.Items.Insert(0, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ") + "Update LIVEGOALS / GOALDETAILS "+ Host + " " + Guest + " ToLive"); } }));
                    }));

                    //LIVEGOALS
                    queryString = "select * from livegoals g where g.CLEAGUE='" + League + "' and ((g.CHOST='" + Host + "'  and g.CGUEST='" + Guest + "') OR (g.CHOST='" + Guest + "'  and g.CGUEST='" + Host + "')) ";
                    Files.WriteLog("Sql: " + queryString);

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

                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        //if (AppFlag.TestToLive)
                        //{
                        //    queryString = "delete from livegoals g where g.CLEAGUE='" + League + "' and ((g.CHOST='" + Host + "'  and g.CGUEST='" + Guest + "') OR (g.CHOST='" + Guest + "'  and g.CGUEST='" + Host + "')) ";
                        //    using (FbCommand cmd2 = new FbCommand(queryString, connection))
                        //    {
                        //        int id = Convert.ToInt32(cmd2.ExecuteScalar());
                        //        Files.WriteLog("Sql1: " + queryString);
                        //    }
                        //}

                        using (FbConnection connection2 = new FbConnection(AppFlag.ToLiveDB))
                        {
                            connection2.Open();
                            //LIVEGOALS
                            queryString = "delete from livegoals g where g.CLEAGUE='" + League + "' and ((g.CHOST='" + Host + "'  and g.CGUEST='" + Guest + "') OR (g.CHOST='" + Guest + "'  and g.CGUEST='" + Host + "')) ";
                            using (FbCommand cmd2 = new FbCommand(queryString, connection2))
                            {
                                int id = Convert.ToInt32(cmd2.ExecuteScalar());
                                Files.WriteLog("Sql2: " + queryString);
                            }

                            //insert LIVEGOALS
                            queryString = "select * from livegoals g where g.CLEAGUE='" + League + "' and ((g.CHOST='" + Host + "'  and g.CGUEST='" + Guest + "') OR (g.CHOST='" + Guest + "'  and g.CGUEST='" + Host + "')) ";

                            using (FbCommand cmd = new FbCommand(queryString, connection2))
                            {
                                using (FbDataAdapter fda = new FbDataAdapter(cmd))
                                {
                                    using (FbCommandBuilder fcb = new FbCommandBuilder(fda))
                                    {
                                        // DataSet data = new DataSet();
                                        // //  using (DataSet data = new DataSet())
                                        // //  {
                                        // //data.Tables.Add(new DataTable("livegoals"));
                                        // //fda.Fill(data.Tables["livegoals"]); 

                                        // //fda.Update(data.Tables["livegoals"]); 
                                        // data.Tables.Add(new DataTable("data"));
                                        // fda.Fill(data.Tables["data"]);
                                        // data = ds;
                                        //// data.AcceptChanges();
                                        // fda.Update(data);
                                        // // }
                                        using (DataSet data = new DataSet())
                                        {
                                            data.Tables.Add(new DataTable("data"));
                                            fda.Fill(data.Tables["data"]);
                                            DataRow dr = ds.Tables[0].Rows[0];
                                            DataRow dr2 = data.Tables["data"].NewRow();
                                            for (int i = 0; i < ds.Tables[0].Columns.Count; i++)
                                            {
                                                dr2[i] = dr[i];
                                            }
                                            dr2[21] =  DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                                            data.Tables["data"].Rows.Add(dr2);
                                            fda.Update(data.Tables["data"]);
                                        }
                                    }
                                }
                            }
                            Files.WriteLog(" [Success] Insert/Update LIVEGOALS " + " " + League + " " + Host + "/" + Guest);
                            SendAlertMsg(1030);
                            //goaldetails
                            queryString = "select * from goaldetails g where g.CLEAGUE='" + League + "' and ((g.CHOST='" + Host + "'  and g.CGUEST='" + Guest + "') OR (g.CHOST='" + Guest + "'  and g.CGUEST='" + Host + "')) ";
                            Files.WriteLog("Sql: " + queryString);

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

                            if (ds.Tables[0].Rows.Count > 0)
                            {
                                queryString = "delete from GOALDETAILS g where g.CLEAGUE='" + League + "' and ((g.CHOST='" + Host + "'  and g.CGUEST='" + Guest + "') OR (g.CHOST='" + Guest + "'  and g.CGUEST='" + Host + "')) ";

                                //if (AppFlag.TestToLive)
                                //{
                                //    using (FbCommand cmd2 = new FbCommand(queryString, connection))
                                //    {
                                //        int id = Convert.ToInt32(cmd2.ExecuteScalar());
                                //        Files.WriteLog("Sql1: " + queryString);
                                //    }
                                //}

                                using (FbCommand cmd2 = new FbCommand(queryString, connection2))
                                {
                                    int id = Convert.ToInt32(cmd2.ExecuteScalar());
                                    Files.WriteLog("Sql2: " + queryString);
                                }

                                //insert GOALDETAILS
                                queryString = "select * from GOALDETAILS g where g.CLEAGUE='" + League + "' and ((g.CHOST='" + Host + "'  and g.CGUEST='" + Guest + "') OR (g.CHOST='" + Guest + "'  and g.CGUEST='" + Host + "')) ";

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
                                                foreach (DataRow dr in ds.Tables[0].Rows)
                                                { 
                                                    DataRow dr2 = data.Tables["data"].NewRow();
                                                    for (int i = 0; i < ds.Tables[0].Columns.Count; i++)
                                                    {
                                                        dr2[i] = dr[i];
                                                    }
                                                     dr2[17] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                                                    data.Tables["data"].Rows.Add(dr2);
                                                }
                                                fda.Update(data.Tables["data"]);
                                            }
                                        }
                                    }
                                } 
                                Files.WriteLog(" [Success] Insert/Update GOALDETAILS " + " " + League + " " + Host + "/" + Guest); 
                            }
                            connection2.Close();
                        }

                    }
                    connection.Close();
                }

                SendAlertMsg(1031);
            }
            catch (Exception exp)
            {
                Files.WriteError(DateTime.Now.ToString("HH:mm:ss ") + "LIVEGOALS/GOALDETAILS[ToLive],error: " + exp);
            }
        }

        protected override void WndProc(ref Message m)
        {
            try
            {
                if (m_ExternID == m.Msg)
                {
                    //COPYDATASTRUCT mystr = new COPYDATASTRUCT();
                    //Type mytype = mystr.GetType();
                    //mystr = (COPYDATASTRUCT)m.GetLParam(mytype);
                    //string ml = mystr.lpData;

                    ////// string len = mystr.cbData;
                    ////COPYDATASTRUCT cdata = new COPYDATASTRUCT();
                    ////Type mytype = cdata.GetType();
                    ////cdata = (COPYDATASTRUCT)Marshal.PtrToStructure(m.LParam, mytype);
                    ////string ml = cdata.lpData;
                    //COPYDATASTRUCT cdata = new COPYDATASTRUCT();
                    //Type mytype = cdata.GetType();
                    //cdata = (COPYDATASTRUCT)m.GetLParam(mytype);
                    //int ml = 1;
                    //string strCmdLine = string.Empty;
                    //COPYDATASTRUCT cds = new COPYDATASTRUCT();
                    //cds = (COPYDATASTRUCT)Marshal.PtrToStructure(m.LParam, typeof(COPYDATASTRUCT));
                    //if (cds.cbData > 0)
                    //{
                    //    strCmdLine = Marshal.PtrToStringAnsi(cds.lpData).Substring(0, cds.cbData);
                    //}

                    int mw = (int)m.WParam;
                    int ml = (int)m.LParam;

                    Files.WriteLog(" [Success] recevied " + m.Msg + "-" + mw.ToString() + "/" + ml.ToString());

                    if (mw == 1)
                    {
                        string queryString = "";
                        DataSet ds = new DataSet();
                        try
                        {
                            using (FbConnection connection = new FbConnection(AppFlag.ScoutsDBConn))
                            {
                                connection.Open();
                                Files.WriteLog("Update ADDANALYSISOTHER " + ml.ToString());
                                this.listBox1.Invoke(new Action(() =>
                                {
                                    this.listBox1.Invoke(new Action(() => { { this.listBox1.Items.Insert(0, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ") + "Start sync ANALYSISOTHERS." + ml.ToString()); } }));
                                }));

                                queryString = "SELECT a.EVENTID, a.CLEAGUE aCLEAGUE, l.LEAGUE_CHI_NAME CLEAGUE, a.CTEAM, a.CTEAMTYPE, a.CACTION, a.CSHOTS, a.CFOULS, a.CCORNER_KICKS, a.COFFSIDES, a.CPOSSESSION, a.CYELLOW_CARDS, a.CRED_CARDS, a.CATTACKS, a.CSUBSTITUTIONS, a.CTHROWINS, a.CGOALKICKS, a.CTIMESTAMP FROM ANALYSIS_OTHERS a" +
                                   " inner join  LEAGUE_INFO l on l.CLEAGUE_ALIAS_NAME  =a.CLEAGUE where a.EVENTID = " + ml.ToString() + " order by a.CTIMESTAMP desc ";

                                Files.WriteLog("Sql: " + queryString);


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

                                if (ds.Tables[0].Rows.Count == 2)
                                {
                                    using (FbConnection connection2 = new FbConnection(AppFlag.MangoDBConn))
                                    {
                                        List<DataRow> drH = ds.Tables[0].Select().AsEnumerable().ToList<DataRow>().Where(x => x.Field<string>("CTEAMTYPE") == "H").ToList();
                                        List<DataRow> drG = ds.Tables[0].Select().AsEnumerable().ToList<DataRow>().Where(x => x.Field<string>("CTEAMTYPE") == "G").ToList();

                                        connection2.Open();

                                        using (FbCommand cmd2 = new FbCommand("PR_ADDANALYSISOTHER", connection2))
                                        {
                                            cmd2.CommandType = CommandType.StoredProcedure;
                                            cmd2.Parameters.Add("@CLEAGUE", drH[0]["CLEAGUE"]);
                                            cmd2.Parameters.Add("@CHOST", drH[0]["CTEAM"]);
                                            cmd2.Parameters.Add("@CGUEST", drG[0]["CTEAM"]);
                                            cmd2.Parameters.Add("@CACTION", "U");
                                            cmd2.Parameters.Add("@CSHOTS", drH[0]["CSHOTS"] + "@" + drG[0]["CSHOTS"]);
                                            cmd2.Parameters.Add("@CFOULS", drH[0]["CFOULS"] + "@" + drG[0]["CFOULS"]);
                                            cmd2.Parameters.Add("@CCORNER_KICKS", drH[0]["CCORNER_KICKS"] + "@" + drG[0]["CCORNER_KICKS"]);
                                            cmd2.Parameters.Add("@COFFSIDES", drH[0]["COFFSIDES"] + "@" + drG[0]["COFFSIDES"]);
                                            cmd2.Parameters.Add("@CPOSSESSION", drH[0]["CPOSSESSION"] + "@" + drG[0]["CPOSSESSION"]);
                                            cmd2.Parameters.Add("@CYELLOW_CARDS", drH[0]["CYELLOW_CARDS"] + "@" + drG[0]["CYELLOW_CARDS"]);
                                            cmd2.Parameters.Add("@CRED_CARDS", drH[0]["CRED_CARDS"] + "@" + drG[0]["CRED_CARDS"]);
                                            cmd2.Parameters.Add("@TIMEFLAG", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                                            cmd2.Parameters.Add("@IDEST", "0");
                                            int id = Convert.ToInt32(cmd2.ExecuteScalar());
                                            if (id > -1)
                                            {
                                                Files.WriteLog(" [Success] Insert ADDANALYSISOTHER " + " " + drH[0]["CLEAGUE"] + " " + drH[0]["CTEAM"] + "/" + drG[0]["CTEAM"]);
                                            }
                                        }

                                        connection2.Close();
                                    }
                                }
                                connection.Close();
                            }
                            SendAlertMsg(62);
                        }
                        catch (Exception exp)
                        {
                            Files.WriteError(DateTime.Now.ToString("HH:mm:ss ") + "ANALYSISOTHERS,error: " + exp);
                        }

                        //ANALYSISBGREMARK 
                        try
                        {
                            using (FbConnection connection = new FbConnection(AppFlag.ScoutsDBConn))
                            {
                                connection.Open();
                                Files.WriteLog("Update ANALYSISBGREMARK " + ml.ToString());
                                this.listBox1.Invoke(new Action(() =>
                                {
                                    this.listBox1.Invoke(new Action(() => { { this.listBox1.Items.Insert(0, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ") + "Start sync ANALYSISBGREMARK." + ml.ToString()); } }));
                                }));

                                //queryString = "SELECT g.WC_8,e.HKJCDAYCODE,e.HKJCMATCHNO,  E.CLEAGUEALIAS_OUTPUT_NAME,E.CLEAGUE_HKJC_NAME,E.HKJCHOSTNAME_CN, E.HKJCGUESTNAME_CN,E.CMATCHDATETIME CMATCHDATE, E.CMATCHDATETIME CMATCHTIME," +
                                //    " '' CHANDICAP,'H' CMATCHFIELD,'' CHOSTROOT,   ''CGUESTROOT,''CVENUE,'' CTEMPERATURE,'' CWEATHERSTATUS,''CREMARK,current_timestamp  TIMEFLAG,'0'IDEST FROM EVENTS r" +
                                //    " INNER JOIN  EMATCHES E ON E.EMATCHID = R.ID INNER JOIN EVENT_DETAILS G ON G.eventid = " +
                                //             "E.EMATCHID WHERE R.ID ="+ ml.ToString() + " ORDER BY E.CTIMESTAMP desc ";
                                queryString = "SELECT e.HKJCDAYCODE,e.HKJCMATCHNO,  E.CLEAGUEALIAS_OUTPUT_NAME,E.CLEAGUE_HKJC_NAME,E.HKJCHOSTNAME_CN, E.HKJCGUESTNAME_CN,E.CMATCHDATETIME CMATCHDATE, E.CMATCHDATETIME CMATCHTIME,'' CHANDICAP,'H' CMATCHFIELD,'' CHOSTROOT,   ''CGUESTROOT, a.CMATCH_VENUE,  a.CTEMPERATURE, a.IWEATHER_STATUS,ar.CREMARKS,current_timestamp TIMEFLAG,'0'IDEST FROM EVENTS r " +
                                                     "INNER JOIN  EMATCHES E ON E.EMATCHID = R.ID INNER JOIN EVENT_DETAILS G ON G.eventid = E.EMATCHID left join   ANALYSIS_BG_INFO a on a.IMATCH_CNT = r.id left join   ANALYSIS_REMARK_INFO ar on ar.IMATCH_CNT = r.id " +
                                                     "WHERE R.ID =" + ml;
                                Files.WriteLog("Sql: " + queryString);


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

                                if (ds.Tables[0].Rows.Count > 0)
                                {
                                    using (FbConnection connection2 = new FbConnection(AppFlag.MangoDBConn))
                                    {
                                        connection2.Open();
                                        foreach (DataRow drH in ds.Tables[0].Rows)
                                        {
                                            if (drH["CLEAGUE_HKJC_NAME"] is DBNull || drH["HKJCHOSTNAME_CN"] is DBNull || drH["HKJCGUESTNAME_CN"] is DBNull) continue;

                                            string wStatus = drH["IWEATHER_STATUS"].ToString();
                                            if (wStatus == "0")
                                            { wStatus = "多雲"; }
                                            else if (wStatus == "1")
                                            { wStatus = "良好"; }
                                            else if (wStatus == "2")
                                            { wStatus = "暴雨"; }
                                            else if (wStatus == "3")
                                            { wStatus = "暴雲"; }
                                            else if (wStatus == "4")
                                            { wStatus = "熱"; }
                                            else if (wStatus == "5")
                                            { wStatus = "小雨"; }
                                            else if (wStatus == "6")
                                            { wStatus = "小雪"; }
                                            else if (wStatus == "7")
                                            { wStatus = "多雨"; }
                                            else if (wStatus == "8")
                                            { wStatus = "暴风雨"; }
                                            else if (wStatus == "9")
                                            { wStatus = "晴朗"; }
                                            else if (wStatus == "10")
                                            { wStatus = "炎熱"; }
                                            else if (wStatus == "11")
                                            { wStatus = "有风"; }
                                            else { wStatus = ""; }
                                            using (FbCommand cmd2 = new FbCommand("PR_ANALYSISBGREMARK", connection2))
                                            {
                                                cmd2.CommandType = CommandType.StoredProcedure;
                                                cmd2.Parameters.Add("@CLEAGUE", drH["CLEAGUE_HKJC_NAME"]);
                                                cmd2.Parameters.Add("@CHOST", drH["HKJCHOSTNAME_CN"]);
                                                cmd2.Parameters.Add("@CGUEST", drH["HKJCGUESTNAME_CN"]);
                                                cmd2.Parameters.Add("@CMATCHDATE", Convert.ToDateTime(drH["CMATCHDATE"]).ToString("yyyyMMdd"));
                                                cmd2.Parameters.Add("@CMATCHTIME", Convert.ToDateTime(drH["CMATCHTIME"]).ToString("HHmm"));
                                                cmd2.Parameters.Add("@CHANDICAP", drH["CHANDICAP"]);
                                                cmd2.Parameters.Add("@CMATCHFIELD", drH["CMATCHFIELD"]);
                                                cmd2.Parameters.Add("@CHOSTROOT", drH["CHOSTROOT"]);
                                                cmd2.Parameters.Add("@CGUESTROOT", drH["CGUESTROOT"]);
                                                cmd2.Parameters.Add("@CVENUE", drH["CMATCH_VENUE"]);
                                                cmd2.Parameters.Add("@CTEMPERATURE", drH["CTEMPERATURE"]);
                                                cmd2.Parameters.Add("@CWEATHERSTATUS", wStatus);
                                                cmd2.Parameters.Add("@CREMARK", drH["CREMARKS"]);
                                                cmd2.Parameters.Add("@TIMEFLAG", Convert.ToDateTime(drH["TIMEFLAG"]).ToString("yyyy-MM-dd HH:mm:ss.fff"));
                                                cmd2.Parameters.Add("@IDEST", drH["IDEST"]);
                                                int id = Convert.ToInt32(cmd2.ExecuteScalar());
                                                if (id > -1)
                                                {
                                                    Files.WriteLog(" [Success] Insert ANALYSISBGREMARK " + " " + drH["CLEAGUEALIAS_OUTPUT_NAME"] + " " + drH["HKJCHOSTNAME_CN"] + "/" + drH["HKJCGUESTNAME_CN"]);
                                                }
                                            }
                                        }
                                        connection2.Close();
                                    }
                                }
                                connection.Close();
                            }
                            SendAlertMsg(62);
                        }
                        catch (Exception exp)
                        {
                            Files.WriteError(DateTime.Now.ToString("HH:mm:ss ") + "ANALYSISBGREMARK,error: " + exp);
                        }
                    }
                    else if (mw == 2)
                    {
                        bool bLive = false;
                        string sLeague = "";
                        string sHost = "";
                        string sGuest = "";
                        try
                        {
                            string queryString = "";
                            DataSet ds = new DataSet();
                            using (FbConnection connection = new FbConnection(AppFlag.ScoutsDBConn))
                            {
                                connection.Open();
                                Files.WriteLog("Update LIVEGOALS/GOALDETAILS " + ml.ToString());
                                this.listBox1.Invoke(new Action(() =>
                                {
                                    this.listBox1.Invoke(new Action(() => { { this.listBox1.Items.Insert(0, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ") + "Start sync LIVEGOALS/GOALDETAILS." + ml.ToString()); } }));
                                }));

                                //LIVEGOALS
                                queryString = "SELECT FIRST 1 g.Tolive,  (select  FIRST 1  t.HKJC_NAME_CN from teams t  where t.id= r.HOME_ID) H,(select  FIRST 1  t.HKJC_NAME_CN from teams t  where t.id=r.GUEST_ID) G,  E.CLEAGUEALIAS_OUTPUT_NAME,E.CLEAGUE_HKJC_NAME,E.HKJCHOSTNAME_CN, E.HKJCGUESTNAME_CN," +
                                  " E.MAPPINGSTATUS, ( CASE WHEN E.MAPPINGSTATUS IS TRUE THEN E.HKJCGUESTNAME_CN ELSE E.HKJCHOSTNAME_CN END ) HH,  ( CASE WHEN E.MAPPINGSTATUS IS TRUE THEN E.HKJCHOSTNAME_CN ELSE E.HKJCGUESTNAME_CN END ) GG, " +
                                  " E.CMATCHDATETIME CMATCHDATE,E.CMATCHDATETIME CMATCHTIME,'H' CMATCHFIELD, 'U' CACTION,G.H_GOAL,G.G_GOAL,G.H_RED,G.G_RED,G.HH_GOAL,G.GH_GOAL,G.H_CONFIRM,G.G_CONFIRM,''CSONGID,''CALERT,G.GAMESTATUS, " +
                                  " ((select count(*) from MATCHDETAILS e where e.EMATCHID= " + ml.ToString() + " and (e.STATUS='Extratime 1st half' or e.STATUS='Extratime 2nd half') and e.CTYPE='goal' and e.TEAM_ID=r.HOME_ID)||'-'||" +
                                  " (select count(*) from MATCHDETAILS e where e.EMATCHID = " + ml.ToString() + " and(e.STATUS = 'Extratime 1st half' or e.STATUS = 'Extratime 2nd half') and e.CTYPE = 'goal' and e.TEAM_ID = r.GUEST_ID) || ';' ||" +
                                  " (select count(*) from MATCHDETAILS e where e.EMATCHID = " + ml.ToString() + " and(e.STATUS = 'Penalty shootout') and e.CTYPE = 'goal' and e.TEAM_ID = r.HOME_ID)|| '-' ||" +
                                  " (select count(*) from MATCHDETAILS e where e.EMATCHID = " + ml.ToString() + " and(e.STATUS = 'Penalty shootout') and e.CTYPE = 'goal' and e.TEAM_ID = r.GUEST_ID) )" +
                                  " CCOMMENT,g.ELAPSED CTIMEOFGAME,current_timestamp TIMEFLAG,'0'IDEST FROM EVENTS r INNER JOIN " +
                                  " EMATCHES E ON E.EMATCHID = R.ID " +
                                  " left JOIN GOALINFO G ON G.EMATCHID = E.EMATCHID WHERE R.ID =" + ml.ToString() + " and r.START_DATE > dateadd (-" + AppFlag.LivePeriod + " day to current_timestamp)  ORDER BY E.CTIMESTAMP DESC ";

                                Files.WriteLog("Sql: " + queryString);

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

                                if (ds.Tables[0].Rows.Count > 0)
                                { 

                                    DataRow drH = ds.Tables[0].Rows[0];
                                    bLive = drH["Tolive"].ToString() == "1" ? true : false;
                                    sLeague = drH["CLEAGUE_HKJC_NAME"].ToString();
                                    sHost = drH["H"] is DBNull ? drH["HH"].ToString() : drH["H"].ToString();
                                    sGuest = drH["G"] is DBNull ? drH["GG"].ToString() : drH["G"].ToString();
                                    //if (drH["CLEAGUE_HKJC_NAME"] is DBNull || drH["CLEAGUEALIAS_OUTPUT_NAME"] is DBNull
                                    //    || drH["HKJCHOSTNAME_CN"] is DBNull || drH["HKJCGUESTNAME_CN"] is DBNull
                                    //    || drH["H"] is DBNull || drH["G"] is DBNull) { }
                                    //else
                                    //{
                                    try
                                    {
                                        using (FbConnection connection2 = new FbConnection(AppFlag.MangoDBConn))
                                        {
                                            connection2.Open();
                                            using (FbCommand cmd2 = new FbCommand("PR_LIVEGOALS", connection2))
                                            {
                                                cmd2.CommandType = CommandType.StoredProcedure;
                                                cmd2.Parameters.Add("@CLEAGUEALIAS", drH["CLEAGUEALIAS_OUTPUT_NAME"]);
                                                cmd2.Parameters.Add("@CLEAGUE", drH["CLEAGUE_HKJC_NAME"]);
                                                cmd2.Parameters.Add("@CHOST", drH["H"] is DBNull ? drH["HH"] : drH["H"]);
                                                cmd2.Parameters.Add("@CGUEST", drH["G"] is DBNull ? drH["GG"] : drH["G"]);
                                                cmd2.Parameters.Add("@CMATCHDATE", Convert.ToDateTime(drH["CMATCHDATE"]).ToString("yyyyMMdd"));
                                                cmd2.Parameters.Add("@CMATCHTIME", Convert.ToDateTime(drH["CMATCHTIME"]).ToString("HHmm"));
                                                cmd2.Parameters.Add("@CMATCHFIELD", drH["CMATCHFIELD"]);
                                                cmd2.Parameters.Add("@CACTION", drH["CACTION"]);
                                                cmd2.Parameters.Add("@IH_GOAL", drH["H_GOAL"].ToString() == "" ? "0" : drH["H_GOAL"].ToString());
                                                cmd2.Parameters.Add("@IG_GOAL", drH["G_GOAL"].ToString() == "" ? "0" : drH["G_GOAL"].ToString());
                                                cmd2.Parameters.Add("@IH_REDCARD", drH["H_RED"].ToString() == "" ? "0" : drH["H_RED"].ToString());
                                                cmd2.Parameters.Add("@IG_REDCARD", drH["G_RED"].ToString() == "" ? "0" : drH["G_RED"].ToString());
                                                cmd2.Parameters.Add("@IH_HT_GOAL", drH["HH_GOAL"].ToString() == "" ? "0" : drH["HH_GOAL"].ToString());
                                                cmd2.Parameters.Add("@IG_HT_GOAL", drH["GH_GOAL"].ToString() == "" ? "0" : drH["GH_GOAL"].ToString());
                                                //cmd2.Parameters.Add("@IH_CONFIRM", drH["H_CONFIRM"].ToString() == "" ? "-1" : drH["H_CONFIRM"].ToString());
                                                //cmd2.Parameters.Add("@IG_CONFIRM", drH["G_CONFIRM"].ToString() == "" ? "-1" : drH["G_CONFIRM"].ToString());
                                                cmd2.Parameters.Add("@CSONGID", drH["CSONGID"]);
                                                cmd2.Parameters.Add("@CALERT", drH["CALERT"]);
                                                cmd2.Parameters.Add("@CSTATUS", drH["GAMESTATUS"]);
                                                cmd2.Parameters.Add("@CCOMMENT", drH["CCOMMENT"].ToString().Replace("0-0;0-0", "").Replace(";", ";PK").Replace(";PK0-0",""));
                                                // cmd2.Parameters.Add("@CTIMEOFGAME", drH["CTIMEOFGAME"]); 
                                                cmd2.Parameters.Add("@CTIMEOFGAME", drH["CTIMEOFGAME"] is DBNull ? "-1" : drH["CTIMEOFGAME"].ToString());
                                                cmd2.Parameters.Add("@TIMEFLAG", Convert.ToDateTime(drH["TIMEFLAG"]).ToString("yyyy-MM-dd HH:mm:ss.fff"));
                                                cmd2.Parameters.Add("@IDEST", drH["IDEST"]);
                                                int id = Convert.ToInt32(cmd2.ExecuteScalar());
                                                // if (id > 0)
                                                //  {
                                                Files.WriteLog(" [Success] " + (id > 0 ? "Insert" : "Update") + " LIVEGOALS " + " " + drH["CLEAGUEALIAS_OUTPUT_NAME"] + " " + drH["HKJCHOSTNAME_CN"] + "/" + drH["HKJCGUESTNAME_CN"]);
                                                //  } 
                                                Files.WriteLog("Sql: " + cmd2.CommandText + " " + drH["CLEAGUEALIAS_OUTPUT_NAME"] + " " + drH["CLEAGUE_HKJC_NAME"] + " " + drH["HKJCHOSTNAME_CN"] + " " + drH["HKJCGUESTNAME_CN"] + " " + Convert.ToDateTime(drH["CMATCHDATE"]).ToString("yyyyMMdd") + " " + Convert.ToDateTime(drH["CMATCHDATE"]).ToString("HHmm") + " " + drH["GAMESTATUS"]);
                                            }
                                            connection2.Close();
                                        }
                                    }
                                    catch (Exception exp)
                                    {
                                        Files.WriteError(DateTime.Now.ToString("HH:mm:ss ") + "LIVEGOALS,error2: " + exp);
                                    }
                                    //}
                                }
                                SendAlertMsg(30);

                                //GOALDETAILS 
                                Files.WriteLog("Update GOALDETAILS " + ml.ToString());
                                queryString = " SELECT distinct  (select  FIRST 1  t.HKJC_NAME_CN from teams t  where t.id=s.HOME_ID) H,(select  FIRST 1  t.HKJC_NAME_CN from teams t  where t.id=s.GUEST_ID) G, E.CLEAGUEALIAS_OUTPUT_NAME,E.CLEAGUE_HKJC_NAME,E.HKJCHOSTNAME_CN, E.HKJCGUESTNAME_CN," +
                                            " 'F' CCURRENTSTATUS, '' CPK, 'U' CACTION, '' CALERT, R.CTYPE CRECORDTYPE, R.HG CRECORDBELONG,  r.STATUS CMATCHSTATUS, r.ELAPSED , (select count(*) from MATCHDETAILS x where x.EMATCHID=g.EMATCHID and cast( x.ELAPSED as integer)<=cast( r.ELAPSED as integer) and x.CTYPE='goal' and x.HG='H' " +
                                          //  "AND (x.STATUS!='Penalty shootout' AND x.ELAPSED!=105)  and (x.STATUS!='Extratime 1st half' and x.STATUS!='Extratime 2nd half')" +
                                            ")  CSCOREHOST, (select count(*) from MATCHDETAILS x where x.EMATCHID=g.EMATCHID and cast( x.ELAPSED as integer)<=cast( r.ELAPSED as integer) and x.CTYPE='goal' and x.HG='G' " +
                                            //  "AND (x.STATUS!='Penalty shootout' AND x.ELAPSED!=105)  and (x.STATUS!='Extratime 1st half' and x.STATUS!='Extratime 2nd half')" +
                                            ")  CSCOREGUEST, '-1' CSCORENUM,   '0' CSCOREOWNGOAL, r.PLAYERCHI CSCORER,r.PLAYER CSCORER2 , r.LASTTIME TIMEFLAG, '0' IDEST " +
                                            "FROM MATCHDETAILS r   INNER JOIN  EMATCHES E ON E.EMATCHID = r.EMATCHID   INNER JOIN  events s ON s.id = r.EMATCHID   INNER JOIN GOALINFO G ON G.EMATCHID= E.EMATCHID where r.EMATCHID =" + ml.ToString() + " AND (R.CTYPE='goal'  or r.CTYPE='rcard')" +
                                            //  " AND (R.STATUS!='Penalty shootout' AND R.ELAPSED!=105) and ( r.STATUS!='Extratime 1st half' and r.STATUS!='Extratime 2nd half')" +
                                            //  " AND   E.CMATCHDATETIME > dateadd (-" + AppFlag.LivePeriod + " day to current_timestamp)" +
                                            "  order by r.LASTTIME asc";
                                Files.WriteLog("Sql GOALDETAILS2: " + queryString);

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

                                if (ds.Tables[0].Rows.Count > 0)
                                {
                                    using (FbConnection connection2 = new FbConnection(AppFlag.MangoDBConn))
                                    {
                                        connection2.Open();
                                        queryString = "delete from GOALDETAILS g where g.CLEAGUE='" + ds.Tables[0].Rows[0]["CLEAGUE_HKJC_NAME"].ToString() + "' and( (g.CHOST='" + ds.Tables[0].Rows[0]["HKJCHOSTNAME_CN"].ToString() + "'  and g.CGUEST='" + ds.Tables[0].Rows[0]["HKJCGUESTNAME_CN"].ToString() + "') OR (g.CHOST='" + ds.Tables[0].Rows[0]["HKJCGUESTNAME_CN"].ToString() + "'  and g.CGUEST='" + ds.Tables[0].Rows[0]["HKJCGUESTNAME_CN"].ToString() + "')) ";
                                        using (FbCommand cmd2 = new FbCommand(queryString, connection2))
                                        {
                                            int id = Convert.ToInt32(cmd2.ExecuteScalar());
                                            Files.WriteLog("Sql: " + queryString);
                                        } 

                                        foreach (DataRow drH in ds.Tables[0].Rows)
                                        {
                                            try
                                            {
                                                if (drH["CLEAGUEALIAS_OUTPUT_NAME"] is DBNull || drH["CLEAGUE_HKJC_NAME"] is DBNull
                                                    || drH["HKJCHOSTNAME_CN"] is DBNull || drH["HKJCGUESTNAME_CN"] is DBNull)
                                                //  || drH["H"] is DBNull || drH["G"] is DBNull)
                                                { continue; }
                                                using (FbCommand cmd2 = new FbCommand("PR_GOALDETAILS", connection2))
                                                {
                                                    cmd2.CommandType = CommandType.StoredProcedure;
                                                    cmd2.Parameters.Add("@CLEAGUEALIAS", drH["CLEAGUEALIAS_OUTPUT_NAME"]);
                                                    cmd2.Parameters.Add("@CLEAGUE", drH["CLEAGUE_HKJC_NAME"]);
                                                    cmd2.Parameters.Add("@CHOST", drH["H"] is DBNull ? drH["HKJCHOSTNAME_CN"] : drH["H"]);
                                                    cmd2.Parameters.Add("@CGUEST", drH["G"] is DBNull ? drH["HKJCGUESTNAME_CN"] : drH["G"]);
                                                    cmd2.Parameters.Add("@CCURRENTSTATUS", drH["CCURRENTSTATUS"]);
                                                    cmd2.Parameters.Add("@CPK", drH["CPK"]);
                                                    cmd2.Parameters.Add("@CACTION", drH["CACTION"]);
                                                    cmd2.Parameters.Add("@CALERT", drH["CALERT"]);
                                                    cmd2.Parameters.Add("@CRECORDTYPE", drH["CRECORDTYPE"]);
                                                    cmd2.Parameters.Add("@CRECORDBELONG", drH["CRECORDBELONG"]);
                                                    cmd2.Parameters.Add("@CMATCHSTATUS", drH["CMATCHSTATUS"]);
                                                    cmd2.Parameters.Add("@CMATCHTIME", drH["ELAPSED"]);
                                                    cmd2.Parameters.Add("@CSCOREHOST", drH["CSCOREHOST"]);
                                                    cmd2.Parameters.Add("@CSCOREGUEST", drH["CSCOREGUEST"]);
                                                    cmd2.Parameters.Add("@CSCORENUM", drH["CSCORENUM"]);
                                                    cmd2.Parameters.Add("@CSCOREOWNGOAL", drH["CSCOREOWNGOAL"]);
                                                    cmd2.Parameters.Add("@CSCORER", drH["CSCORER"] is DBNull ? drH["CSCORER2"].ToString() : drH["CSCORER"].ToString());
                                                    cmd2.Parameters.Add("@TIMEFLAG", Convert.ToDateTime(drH["TIMEFLAG"]).ToString("yyyy-MM-dd HH:mm:ss.fff"));
                                                    cmd2.Parameters.Add("@IDEST", drH["IDEST"]);
                                                    int id = Convert.ToInt32(cmd2.ExecuteScalar());
                                                    if (id > -1)
                                                    {
                                                        Files.WriteLog(" [Success] Insert/Update GOALDETAILS " + " " + drH["CLEAGUEALIAS_OUTPUT_NAME"] + " " + drH["HKJCHOSTNAME_CN"] + "/" + drH["HKJCGUESTNAME_CN"]);
                                                    }
                                                }
                                            }
                                            catch (Exception exp)
                                            {
                                                Files.WriteError(DateTime.Now.ToString("HH:mm:ss ") + "GOALDETAILS,error1: " + exp);
                                                continue;
                                            }
                                        } 
                                        connection2.Close();
                                    }
                                }
                                else
                                {
                                    queryString = "SELECt r.EMATCHID,  r.CMATCHDATETIME,    r.MAPPINGSTATUS,   r.HKJCHOSTNAME_CN, r.HKJCGUESTNAME_CN,r.CLEAGUE_HKJC_NAME, r.CLEAGUEALIAS_OUTPUT_NAME, " +
                                          " ( CASE WHEN r.MAPPINGSTATUS IS TRUE THEN r.HKJCGUESTNAME_CN ELSE r.HKJCHOSTNAME_CN END ) HH,  ( CASE WHEN r.MAPPINGSTATUS IS TRUE THEN r.HKJCHOSTNAME_CN ELSE r.HKJCGUESTNAME_CN END ) GG," +
                                          " (select  FIRST 1  t.HKJC_NAME_CN from teams t  where t.id= e.HOME_ID) H,(select  FIRST 1  t.HKJC_NAME_CN from teams t  where t.id=e.GUEST_ID) G FROM EMATCHES r inner join events e on e.id =r.EMATCHID " +
                                          " where r.EMATCHID =" + ml.ToString();
                                    Files.WriteLog("Sql2: " + queryString);

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
                                    if (ds.Tables[0].Rows.Count > 0)
                                    {
                                        using (FbConnection connection2 = new FbConnection(AppFlag.MangoDBConn))
                                        {
                                            connection2.Open();
                                            queryString = "delete from GOALDETAILS g where g.CLEAGUE='" + ds.Tables[0].Rows[0]["CLEAGUE_HKJC_NAME"].ToString() + "' and ((g.CHOST='" + ds.Tables[0].Rows[0]["HKJCHOSTNAME_CN"].ToString() + "'  and g.CGUEST='" + ds.Tables[0].Rows[0]["HKJCGUESTNAME_CN"].ToString() + "') OR (g.CHOST='" + ds.Tables[0].Rows[0]["HKJCGUESTNAME_CN"].ToString() + "'  and g.CGUEST='" + ds.Tables[0].Rows[0]["HKJCHOSTNAME_CN"].ToString() + "')) ";
                                            using (FbCommand cmd2 = new FbCommand(queryString, connection2))
                                            {
                                                int id = Convert.ToInt32(cmd2.ExecuteScalar());
                                                Files.WriteLog("Sql2: " + queryString);
                                            }
                                            connection2.Close();
                                        }
                                    }
                                }
                                connection.Close();
                                SendAlertMsg(31);
                            }

                            //   if(bLive) SendToLiveDB(ml.ToString());
                            if (bLive) SendToLiveDB(sLeague, sHost,sGuest,"","");
                        }
                        catch (Exception exp)
                        {
                            Files.WriteError(DateTime.Now.ToString("HH:mm:ss ") + "LIVEGOALS/GOALDETAILS,error: " + exp);
                        }

                        //Playlist 2019/0715
                        try
                        {
                            string queryString = "";
                            DataSet ds = new DataSet();
                            using (FbConnection connection = new FbConnection(AppFlag.ScoutsDBConn))
                            {
                                connection.Open();
                                this.listBox1.Invoke(new Action(() =>
                                {
                                    this.listBox1.Invoke(new Action(() => { { this.listBox1.Items.Insert(0, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ") + "Received " + ml + "-ANALYSISPLAYERLIST"); } }));
                                }));

                                Files.WriteLog("Update " + ml + "-ANALYSISPLAYERLIST auto.");
                                //queryString = "select e.id eid, e.id,c.ALIAS, t.HKJC_NAME,t.HKJC_NAME_CN, p.*  from  PLAYERS_INFO p inner join teams t on t.id = p.TEAM_ID inner join events e on e.id = p.EVENT_ID inner join  COMPETITIONS c on c.id = e.COMPETITION_ID where p.EVENT_ID=" + ml + "   and p.IROSTER = 1 order by p.EVENT_ID, p.TEAM_ID  ,p.IPLAYER_NO ";
                                queryString = "select (CASE WHEN T.ID=E.HOME_ID THEN 'H' ELSE 'G' END ) HG,l.LEAGUE_CHI_NAME,  e.id eid, e.id,c.ALIAS, t.HKJC_NAME,t.HKJC_NAME_CN, p.*  from  PLAYERS_INFO p inner join teams t on t.id = p.TEAM_ID inner join events e on e.id = p.EVENT_ID inner join  COMPETITIONS c on c.id = e.COMPETITION_ID inner join LEAGUE_INFO l on c.ALIAS =l.CLEAGUE_ALIAS_NAME where p.EVENT_ID=" + ml.ToString() + " and e.START_DATE -1 <  current_timestamp and   current_timestamp <e.START_DATE+1    and p.IROSTER = 1 order by HG DESC,p.CTIMESTAMP desc  ";
                                Files.WriteLog("Sql: " + queryString);

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
                                connection.Close();
                            }
                            if (ds.Tables[0].Rows.Count > 0)
                            {
                                using (FbConnection connection2 = new FbConnection(AppFlag.MangoDBConn))
                                {
                                    connection2.Open();
                                    List<int> strE = ds.Tables[0].AsEnumerable().Select(d => d.Field<int>("id")).ToList<int>().Distinct().ToList<int>();
                                    foreach (int s in strE)
                                    {
                                         DataRow[] drs = ds.Tables[0].Select("id=" + s);
                                        List<int> strT = drs.Select(d => d.Field<int>("TEAM_ID")).ToList<int>().Distinct().ToList<int>();
                                        if (strT.Count == 2)
                                        {
                                            //  Files.WriteLog(strT.Count.ToString() + " " + "t");
                                            List<DataRow> drH = drs.AsEnumerable().ToList<DataRow>().Where(x => x.Field<int>("TEAM_ID") == strT[0]).ToList();
                                            List<DataRow> drG = drs.AsEnumerable().ToList<DataRow>().Where(x => x.Field<int>("TEAM_ID") == strT[1]).ToList();

                                            List<DataRow> sGH = drH.Where(x => x.Field<int>("IPOS") == 3).ToList();
                                            List<DataRow> sGG = drG.Where(x => x.Field<int>("IPOS") == 3).ToList();
                                            List<DataRow> sBH = drH.Where(x => x.Field<int>("IPOS") == 0).ToList();
                                            List<DataRow> sBG = drG.Where(x => x.Field<int>("IPOS") == 0).ToList();
                                            List<DataRow> sFH = drH.Where(x => x.Field<int>("IPOS") == 2).ToList();
                                            List<DataRow> sFG = drG.Where(x => x.Field<int>("IPOS") == 2).ToList();
                                            List<DataRow> sMH = drH.Where(x => x.Field<int>("IPOS") == 1).ToList();
                                            List<DataRow> sMG = drG.Where(x => x.Field<int>("IPOS") == 1).ToList();
                                            List<DataRow> sUH = drH.Where(x => x.Field<int>("IPOS") == 4).ToList();
                                            List<DataRow> sUG = drG.Where(x => x.Field<int>("IPOS") == 4).ToList();

                                            string sH = "";
                                            string sG = "";
                                            string ssBH = "", ssBG = "", ssFH = "", ssFG = "", ssMH = "", ssMG = "", ssUH = "", ssUG = "";
                                            for (int j = 0; j < 5; j++)
                                            {
                                                if (j == 0)
                                                {
                                                    foreach (DataRow R in sBH)
                                                    {
                                                        ssBH += "B/" + R["IPLAYER_NO"].ToString() + "/" + (R["CPLAYER_NAME"].ToString() == "" ? R["CENGNAME"].ToString() : R["CPLAYER_NAME"].ToString()) + ",";
                                                    }
                                                    foreach (DataRow R in sBG)
                                                    {
                                                        ssBG += "B/" + R["IPLAYER_NO"].ToString() + "/" + (R["CPLAYER_NAME"].ToString() == "" ? R["CENGNAME"] : R["CPLAYER_NAME"].ToString()) + ",";
                                                    }

                                                }
                                                if (j == 1)
                                                {
                                                    foreach (DataRow R in sMH)
                                                    {
                                                        ssMH += "M/" + R["IPLAYER_NO"].ToString() + "/" + (R["CPLAYER_NAME"].ToString() == "" ? R["CENGNAME"] : R["CPLAYER_NAME"].ToString()) + ",";
                                                    }
                                                    foreach (DataRow R in sMG)
                                                    {
                                                        ssMG += "M/" + R["IPLAYER_NO"].ToString() + "/" + (R["CPLAYER_NAME"].ToString() == "" ? R["CENGNAME"] : R["CPLAYER_NAME"].ToString()) + ",";
                                                    }
                                                }
                                                if (j == 2)
                                                {
                                                    foreach (DataRow R in sFH)
                                                    {
                                                        ssFH += "F/" + R["IPLAYER_NO"].ToString() + "/" + (R["CPLAYER_NAME"].ToString() == "" ? R["CENGNAME"] : R["CPLAYER_NAME"].ToString()) + ",";
                                                    }
                                                    foreach (DataRow R in sFG)
                                                    {
                                                        ssFG += "F/" + R["IPLAYER_NO"].ToString() + "/" + (R["CPLAYER_NAME"].ToString() == "" ? R["CENGNAME"] : R["CPLAYER_NAME"].ToString()) + ",";
                                                    }
                                                }
                                                if (j == 3)
                                                {
                                                    sH = sGH.Count > 0 ? "G/" + sGH[0]["IPLAYER_NO"].ToString() + "/" + (sGH[0]["CPLAYER_NAME"].ToString() == "" ? sGH[0]["CENGNAME"] : sGH[0]["CPLAYER_NAME"]) + "," : "";
                                                    sG = sGG.Count > 0 ? "G/" + sGG[0]["IPLAYER_NO"].ToString() + "/" + (sGG[0]["CPLAYER_NAME"].ToString() == "" ? sGG[0]["CENGNAME"] : sGG[0]["CPLAYER_NAME"]) + "," : "";
                                                }
                                                if (j == 4)
                                                {
                                                    foreach (DataRow R in sUH)
                                                    {
                                                        if (R["IPLAYER_NO"].ToString() != "-2")
                                                        {
                                                            ssUH += "F/" + R["IPLAYER_NO"].ToString() + "/" + (R["CPLAYER_NAME"].ToString() == "" ? R["CENGNAME"] : R["CPLAYER_NAME"].ToString()) + ",";
                                                        }
                                                    }
                                                    foreach (DataRow R in sUG)
                                                    {
                                                        if (R["IPLAYER_NO"].ToString() != "-2")
                                                        {
                                                            ssUG += "F/" + R["IPLAYER_NO"].ToString() + "/" + (R["CPLAYER_NAME"].ToString() == "" ? R["CENGNAME"] : R["CPLAYER_NAME"].ToString()) + ",";
                                                        }
                                                    }
                                                }
                                            }

                                            using (FbCommand cmd2 = new FbCommand("PR_ADDPLAYERLIST", connection2))
                                            {
                                                cmd2.CommandType = CommandType.StoredProcedure;
                                                cmd2.Parameters.Add("@CLEAGUE", drs[0]["LEAGUE_CHI_NAME"]);
                                                cmd2.Parameters.Add("@CHOST", drH[0]["HKJC_NAME_CN"]);
                                                cmd2.Parameters.Add("@CGUEST", drG[0]["HKJC_NAME_CN"]);
                                                cmd2.Parameters.Add("@CACTION", "U");
                                                cmd2.Parameters.Add("@CH_PLAYER", (sH + "" + ssBH + "" + ssMH + "" + ssFH + ssUH).TrimEnd(','));
                                                cmd2.Parameters.Add("@CG_PLAYER", (sG + "" + ssBG + "" + ssMG + "" + ssFG + ssUG).TrimEnd(','));
                                                cmd2.Parameters.Add("@TIMEFLAG", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                                                cmd2.Parameters.Add("@IDEST", "0");
                                                int id = Convert.ToInt32(cmd2.ExecuteScalar());
                                                if (id > -1)
                                                {
                                                    Files.WriteLog(" [Success] Insert PLAYERLIST " + " " + drs[0]["ALIAS"] + " " + drH[0]["HKJC_NAME_CN"] + "/" + drG[0]["HKJC_NAME_CN"]);
                                                }
                                            }
                                        }
                                    }
                                    connection2.Close();
                                    SendAlertMsg(12);
                                }
                            }
                            // connection.Close();
                            //  }
                        }
                        catch (Exception exp)
                        {
                            Files.WriteError(DateTime.Now.ToString("HH:mm:ss ") + "Update ANALYSISPLAYERLIST auto,error: " + exp);
                        }
                    }
                    else if (mw == 3)
                    {
                        try
                        {
                            this.listBox1.Invoke(new Action(() =>
                            {
                                this.listBox1.Invoke(new Action(() => { { this.listBox1.Items.Insert(0, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ") + "Recevied msg,Start Sync."); } }));
                            }));

                            //DateTime dt = DateTime.ParseExact(((int)m.WParam).ToString() + ((int)m.LParam).ToString(), "yyyyMMddHHmmssfff", CultureInfo.InvariantCulture);
                            //Files.WriteLog(" [Success] recevied " + dt.ToString("yyyy-MM-dd HH:mm:ss.fff"));

                            int iIndex = 0;
                            DataSet ds = new DataSet();
                            string[] syncItems;
                            ArrayList configSetting = new ArrayList();
                            string queryString = "";

                            configSetting = AppFlag.configSetting;
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
                            // configSetting.Clear(); 

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
                                            this.listBox1.Invoke(new Action(() =>
                                            {
                                                this.listBox1.Invoke(new Action(() => { { this.listBox1.Items.Insert(0, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ") + "Start sync " + syncItems[i] + "."); } }));
                                            }));
                                            if (i == 0)
                                            {
                                                //ANALYSIS_STAT_INFO
                                                queryString = "select  e.EMATCHID eid,   e.CLEAGUE_HKJC_NAME,  e.CLEAGUEALIAS_OUTPUT_NAME, e.HKJCHOSTNAME_CN, e.HKJCGUESTNAME_CN, e.CMATCHDATETIME,  a.*   from ANALYSIS_STAT_INFO  a inner join EMATCHES e on e.EMATCHID =a.IMATCH_CNT   where a.IMATCH_CNT in ( " + ml.ToString() + ")";
                                            }
                                            else if (i == 1)
                                            {
                                                //ANALYSIS_HISTORY_INFO
                                                //     queryString = "select   e.CLEAGUE_OUTPUT_NAME,e.CLEAGUEALIAS_OUTPUT_NAME, e.HKJCHOSTNAME_CN, e.HKJCGUESTNAME_CN, e.CMATCHDATETIME,  a.*   from    ANALYSIS_HISTORY_INFO    a inner join EMATCHES e on e.EMATCHID =a.IMATCH_CNT where   c.CTIMESTAMP >= '" + dt.ToString("yyyy-MM-dd HH:mm:ss.fff") + "' AND C.EMATCHID IS NOT NULL   AND C.EMATCHID > 0 ORDER BY C.CTIMESTAMP ASC";
                                                queryString = "select  FIRST 4 e.EMATCHID eid,  'CLEAGUEALIAS='''||  e.CLEAGUEALIAS_OUTPUT_NAME||''' AND CHOST=''' || e.HKJCHOSTNAME_CN||''' AND CGUEST='''|| e.HKJCGUESTNAME_CN||''' and cdate='''||substring( replace( cast(a.START_DATE as date),'-','') FROM 1 FOR 6)||''' ' ABC, e.CLEAGUE_HKJC_NAME,  e.CLEAGUEALIAS_OUTPUT_NAME, e.HKJCHOSTNAME_CN, e.HKJCGUESTNAME_CN, e.CMATCHDATETIME,  a.*   from ANALYSIS_HISTORY_INFO  a inner join EMATCHES e on e.EMATCHID =a.IMATCH_CNT   where a.IMATCH_CNT in (" + ml.ToString() + ") ORDER BY  a.imatch_cnt ,a.irec asc   ";
                                            }
                                            else if (i == 2)
                                            {
                                                //ANALYSIS_RECENT_INFO
                                                // queryString = "select  e.CLEAGUE_OUTPUT_NAME,  e.CLEAGUEALIAS_OUTPUT_NAME, e.HKJCHOSTNAME_CN, e.HKJCGUESTNAME_CN,  e.CMATCHDATETIME, a.*   from    ANALYSIS_RECENT_INFO    a inner join EMATCHES e on e.EMATCHID =a.IMATCH_CNT     a where a.IMATCH_CNT in (select  c.EMATCHID  FROM EMATCHES c where   c.CTIMESTAMP >= '" + dt.ToString("yyyy-MM-dd HH:mm:ss.fff") + "' AND C.EMATCHID IS NOT NULL   AND C.EMATCHID > 0 ORDER BY C.CTIMESTAMP ASC)";
                                                // queryString = "select  e.EMATCHID eid,   'CLEAGUE='''||  e.CLEAGUEALIAS_OUTPUT_NAME||''' AND CHOST=''' || e.HKJCHOSTNAME_CN||''' AND CGUEST='''|| e.HKJCGUESTNAME_CN||''' ' ABC, e.CLEAGUE_HKJC_NAME,  e.CLEAGUEALIAS_OUTPUT_NAME CLEAGUE, e.HKJCHOSTNAME_CN CHOST, e.HKJCGUESTNAME_CN CGUEST, e.CMATCHDATETIME,  a.* from ANALYSIS_RECENT_INFO a inner join EMATCHES e on e.EMATCHID = a.IMATCH_CNT   where a.IMATCH_CNT in  (" + ml.ToString() + ")  ORDER BY  a.IMATCH_CNT ,a.irec asc";
                                                queryString = "select  e.EMATCHID eid,   'CLEAGUE='''||  e.CLEAGUE_HKJC_NAME||''' AND CHOST=''' || e.HKJCHOSTNAME_CN||''' AND CGUEST='''|| e.HKJCGUESTNAME_CN||''' ' ABC, e.CLEAGUE_HKJC_NAME CLEAGUE,  e.CLEAGUEALIAS_OUTPUT_NAME  , e.HKJCHOSTNAME_CN CHOST, e.HKJCGUESTNAME_CN CGUEST, e.CMATCHDATETIME,  a.* from ANALYSIS_RECENT_INFO a inner join EMATCHES e on e.EMATCHID = a.IMATCH_CNT   where a.IMATCH_CNT in  (" + ml.ToString() + ")  ORDER BY  a.IMATCH_CNT ,a.irec asc";
                                            }
                                            else if (i == 3)
                                            {
                                                //teams 2019-04-09 10:51:04.234
                                                queryString = "select e.id eid,  t.id tid, t.SHORT_NAME,t.hkjc_name_cn ,T.AREA_ID ,a.NAME COUNTRY ,c.COUNTRY_CHI_NAME from events e inner join teams t on t.id = e.HOME_ID or t.id = e.guest_id inner join  AREAS a on a.ID = t.AREA_ID LEFT join  INT_COUNTRY c on c.COUNTRY_ENG_NAME = a.NAME  where e.id  in (" + ml.ToString() + ")";
                                            }
                                            //else if (i == 4)
                                            //{
                                            //    //players  
                                            //    // queryString = "select e.id,c.ALIAS, t.HKJC_NAME,t.HKJC_NAME_CN, p.*  from  PLAYERS_INFO p inner join teams t on t.id = p.TEAM_ID inner join events e on e.id = p.EVENT_ID inner join  COMPETITIONS c on c.id = e.COMPETITION_ID where p.EVENT_ID  in (select  c.EMATCHID FROM EMATCHES c where c.CTIMESTAMP >= '" + dt.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'  AND C.EMATCHID IS NOT NULL   AND C.EMATCHID > 0 ORDER BY C.CTIMESTAMP ASC) and p.IROSTER = 1 order by p.EVENT_ID, p.TEAM_ID  ,p.IPLAYER_NO ";
                                            //    // queryString = "select e.id,c.ALIAS, t.HKJC_NAME,t.HKJC_NAME_CN, p.*  from  PLAYERS_INFO p inner join teams t on t.id = p.TEAM_ID inner join events e on e.id = p.EVENT_ID inner join  COMPETITIONS c on c.id = e.COMPETITION_ID where p.EVENT_ID = 2737951 and p.IROSTER = 1 order by p.EVENT_ID, p.TEAM_ID  ,p.IPLAYER_NO ";
                                            //    queryString = "select e.id eid, e.id,c.ALIAS, t.HKJC_NAME,t.HKJC_NAME_CN, p.*  from  PLAYERS_INFO p inner join teams t on t.id = p.TEAM_ID inner join events e on e.id = p.EVENT_ID inner join  COMPETITIONS c on c.id = e.COMPETITION_ID where p.EVENT_ID  in (" + ml.ToString() + ") and p.IROSTER = 1 order by p.EVENT_ID, p.TEAM_ID  ,p.IPLAYER_NO ";
                                            //}
                                            else if (i == 5)
                                            {
                                                //FIXTURES  
                                                //queryString = "select e.id,c.ALIAS, t.HKJC_NAME,t.HKJC_NAME_CN, p.*  from  PLAYERS_INFO p inner join teams t on t.id = p.TEAM_ID inner join events e on e.id = p.EVENT_ID inner join  COMPETITIONS c on c.id = e.COMPETITION_ID where p.EVENT_ID  in (select  c.EMATCHID FROM EMATCHES c where c.CTIMESTAMP >= '" + dt.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'  AND C.EMATCHID IS NOT NULL   AND C.EMATCHID > 0 ORDER BY C.CTIMESTAMP ASC) and p.IROSTER = 1 order by p.EVENT_ID, p.TEAM_ID  ,p.IPLAYER_NO ";
                                                queryString = "SELECT 'c.id=''' || r.ID||'''' cid, r.id eid,  L.LEAGUE_CHI_NAME FROM COMPETITIONS r INNER JOIN  LEAGUE_INFO L ON R.ALIAS=L.CLEAGUE_ALIAS_NAME where r.ALIAS = '意甲' OR r.ALIAS = '英超' OR r.ALIAS = '法甲' OR r.ALIAS = '德甲' OR r.ALIAS = '蘇超' OR r.ALIAS = '西甲'OR r.ALIAS = '荷甲' OR r.ALIAS = '日聯' OR r.ALIAS = '澳A' OR r.ALIAS = '歐冠' ORDER BY  R.ID ";
                                            }

                                            Files.WriteLog("Sql: " + queryString);


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

                                            if (ds.Tables[0].Rows.Count > 0)
                                            {
                                                if (i != 5)
                                                {
                                                    string ids = string.Concat("(", string.Join(") OR  (", ds.Tables[0].AsEnumerable().Select(d => d.Field<int>("eid")).ToList<int>().Distinct().ToList().ConvertAll<string>(x => x.ToString())), ")");
                                                    Files.WriteLog("Sync ids: " + ids);
                                                }
                                                using (FbConnection connection2 = new FbConnection(AppFlag.MangoDBConn))
                                                {
                                                    connection2.Open();
                                                    if (i == 0)
                                                    {
                                                        continue;
                                                        foreach (DataRow dr in ds.Tables[0].Rows)
                                                        {
                                                            using (FbCommand cmd2 = new FbCommand("PR_ANALYSIS_STAT", connection2))
                                                            {
                                                                cmd2.CommandType = CommandType.StoredProcedure;
                                                                cmd2.Parameters.Add("@CLEAGUE", dr["CLEAGUE_HKJC_NAME"]);
                                                                cmd2.Parameters.Add("@CHOST", dr["HKJCHOSTNAME_CN"]);
                                                                cmd2.Parameters.Add("@CGUEST", dr["HKJCGUESTNAME_CN"]);
                                                                cmd2.Parameters.Add("@IMATCHDATE", Convert.ToDateTime(dr["CMATCHDATETIME"]).ToString("yyyyMMdd"));
                                                                cmd2.Parameters.Add("@IMATCHTIME", Convert.ToDateTime(dr["CMATCHDATETIME"]).ToString("HHmmss"));
                                                                cmd2.Parameters.Add("@CHANDICAP", "1");
                                                                cmd2.Parameters.Add("@CMATCHFIELD", "H");
                                                                cmd2.Parameters.Add("@IHOSTWIN", dr["IHOSTWIN"]);
                                                                cmd2.Parameters.Add("@IHOSTDRAW", dr["IHOSTDRAW"]);
                                                                cmd2.Parameters.Add("@IHOSTLOSS", dr["IHOSTLOSS"]);
                                                                cmd2.Parameters.Add("@IGUESTWIN", dr["IGUESTWIN"]);
                                                                cmd2.Parameters.Add("@IGUESTDRAW", dr["IGUESTDRAW"]);
                                                                cmd2.Parameters.Add("@IGUESTLOSS", dr["IGUESTLOSS"]);
                                                                cmd2.Parameters.Add("@CREMARK", "");
                                                                cmd2.Parameters.Add("@TIMEFLAG", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                                                                cmd2.Parameters.Add("@IDEST", "0");
                                                                int id = Convert.ToInt32(cmd2.ExecuteScalar());
                                                                if (id > -1)
                                                                {
                                                                    Files.WriteLog(" [Success] Insert ANALYSIS_STAT " + " " + dr["CLEAGUEALIAS_OUTPUT_NAME"] + " " + dr["HKJCHOSTNAME_CN"] + "/" + dr["HKJCGUESTNAME_CN"]);
                                                                }
                                                            }
                                                        }
                                                    }
                                                    else if (i == 1)
                                                    {

                                                        try
                                                        {
                                                            using (FbCommand cmd2 = new FbCommand())
                                                            {
                                                                //List<string> strL= (ds.Tables[0].AsEnumerable().Select(d => d.Field<int>("IMATCH_CNT")).ToList<int>()).ConvertAll<string>(x => x.ToString());
                                                                //string strs = string.Concat("'", string.Join("','", strL), "'");
                                                                //cmd2.CommandText = "delete from ANALYSISHISTORYS where  IMATCH_CNT in (" + strs + ")";
                                                                List<string> strL = ds.Tables[0].AsEnumerable().Select(d => d.Field<string>("ABC")).ToList<string>().Distinct().ToList();
                                                                string strs = string.Concat("(", string.Join(") OR  (", strL), ")");
                                                                cmd2.CommandText = "delete from ANALYSISHISTORYS where " + strs.Replace("OR  ()", "").Replace("() OR", "").Replace("()", "");
                                                                cmd2.Connection = connection2;
                                                                int id = Convert.ToInt32(cmd2.ExecuteScalar());
                                                                Files.WriteLog("delete from ANALYSISHISTORYS where " + strs.Replace("OR  ()", "").Replace("() OR", "").Replace("()", "") + " id=" + id);
                                                            }

                                                            queryString = "select first   1 * from ANALYSISHISTORYS";
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
                                                                            DateTime dt2 = DateTime.MinValue;
                                                                            foreach (DataRow dr2 in ds.Tables[0].Rows)
                                                                            {
                                                                                //r.CLEAGUE, r.CHOST, r.CGUEST, r.CDATE, r.CLEAGUEALIAS, r.CDES,
                                                                                //  r.IHOSTSCORE, r.IGUESTSCORE, r.TIMEFLAG, r.IDEST
                                                                                DataRow dr3 = data.Tables["data"].NewRow();
                                                                                dr3[0] = dr2["CLEAGUE_HKJC_NAME"];
                                                                                dr3[1] = dr2["HKJCHOSTNAME_CN"];
                                                                                dr3[2] = dr2["HKJCGUESTNAME_CN"];
                                                                                //dr3[3] = Convert.ToDateTime(dr2["START_DATE"]).ToString("yyyyMMdd");
                                                                                dr3[3] = dr2["START_DATE"] is DBNull ? dr2["IMATCHYEAR"].ToString() + dr2["IMATCHMONTH"].ToString().PadLeft(2, '0') : Convert.ToDateTime(dr2["START_DATE"]).ToString("yyyyMM");
                                                                                dr3[4] = dr2["CLEAGUEALIAS_OUTPUT_NAME"];
                                                                                dr3[5] = dr2["IMATCHSTATUS"].ToString() == "0" ? "主" : "客";
                                                                                dr3[6] = dr2["IHOSTSCORE"];
                                                                                dr3[7] = dr2["IGUESTSCORE"];
                                                                                // dr3[8] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                                                                                dr3[8] = dt2.ToString("yyyy-MM-dd HH:mm:ss.fff");
                                                                                dr3[9] = "0";
                                                                                data.Tables["data"].Rows.Add(dr3);
                                                                                dt2 = dt2.AddMilliseconds(1);
                                                                            }
                                                                            int count = fda.Update(data.Tables["data"]);
                                                                            Files.WriteLog((count > 0 ? "[Success] " : "[Failure] ") + " Insert ANALYSISHISTORYS [" + data.Tables["data"].Rows.Count + "]");
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                        catch (Exception exp)
                                                        {
                                                            Files.WriteError(DateTime.Now.ToString("HH:mm:ss ") + "Sync ANALYSISHISTORYS,error2: " + exp);
                                                        }
                                                    }
                                                    else if (i == 2)
                                                    {
                                                        using (FbCommand cmd2 = new FbCommand())
                                                        {
                                                            List<string> strL = ds.Tables[0].AsEnumerable().Select(d => d.Field<string>("ABC")).ToList<string>().Distinct().ToList();
                                                            string strs = string.Concat("(", string.Join(") OR  (", strL), ")");
                                                            cmd2.CommandText = "delete from ANALYSISRECENTS where " + strs.Replace("OR  ()", "").Replace("() OR", "").Replace("()", "");
                                                            cmd2.Connection = connection2;
                                                            int id = Convert.ToInt32(cmd2.ExecuteScalar());
                                                            Files.WriteLog("delete from ANALYSISRECENTS where " + strs.Replace("OR  ()", "").Replace("() OR", "").Replace("()", "") + "  id=" + id);
                                                        }

                                                        queryString = "select first   1 * from ANALYSISRECENTS";
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
                                                                        List<string> strTeam = ds.Tables[0].AsEnumerable().Select(d => d.Field<string>("ABC")).ToList<string>().Distinct().ToList();

                                                                        foreach (string s in strTeam)
                                                                        {
                                                                            DataRow[] drs = ds.Tables[0].Select(s);
                                                                            List<DataRow> drH = (drs.AsEnumerable().ToList<DataRow>().Where(x => x.Field<string>("CTEAMFLAG") == "H").ToList()).ToList<DataRow>().Where(x => x.Field<int>("IHOSTSCORE") != -1).ToList();
                                                                            List<DataRow> drG = (drs.AsEnumerable().ToList<DataRow>().Where(x => x.Field<string>("CTEAMFLAG") == "G").ToList()).ToList<DataRow>().Where(x => x.Field<int>("IHOSTSCORE") != -1).ToList();
                                                                            List<DataRow> drHN = (drs.AsEnumerable().ToList<DataRow>().Where(x => x.Field<string>("CTEAMFLAG") == "H").ToList()).ToList<DataRow>().Where(x => x.Field<int>("IHOSTSCORE") == -1).ToList();
                                                                            List<DataRow> drGN = (drs.AsEnumerable().ToList<DataRow>().Where(x => x.Field<string>("CTEAMFLAG") == "G").ToList()).ToList<DataRow>().Where(x => x.Field<int>("IHOSTSCORE") == -1).ToList();

                                                                            DataRow dr3 = data.Tables["data"].NewRow();
                                                                            dr3[0] = drH.Count > 0 ? drH[0]["CLEAGUE"] : drG.Count > 0 ? drG[0][2] : "";
                                                                            dr3[1] = drH.Count > 0 ? drH[0]["CHOST"] : drG.Count > 0 ? drG[0][3] : "";
                                                                            dr3[2] = drH.Count > 0 ? drH[0]["CGUEST"] : drG.Count > 0 ? drG[0][4] : "";
                                                                            dr3[3] = "U";
                                                                            dr3[4] = drHN.Count > 0 ? drHN[0]["LEAGUEALIAS"].ToString() + "/" + drHN[0]["CCHALLENGER"].ToString() + "/" + (drHN[0]["IMATCHSTATUS"].ToString() == "0" ? "主" : "客") : "-1/-1/-1";
                                                                            dr3[5] = drH.Count > 0 ? drH[0]["LEAGUEALIAS"].ToString() + "/" + drH[0]["CCHALLENGER"].ToString() + "/" + (drH[0]["IMATCHSTATUS"].ToString() == "0" ? "主" : "客") + "/" + drH[0]["IHOSTSCORE"].ToString() + "/" + drH[0]["IGUESTSCORE"].ToString() : "-1";
                                                                            dr3[6] = drH.Count > 1 ? drH[0]["LEAGUEALIAS"].ToString() + "/" + drH[1]["CCHALLENGER"].ToString() + "/" + (drH[1]["IMATCHSTATUS"].ToString() == "0" ? "主" : "客") + "/" + drH[1]["IHOSTSCORE"].ToString() + "/" + drH[1]["IGUESTSCORE"].ToString() : "-1";
                                                                            dr3[7] = drH.Count > 2 ? drH[0]["LEAGUEALIAS"].ToString() + "/" + drH[2]["CCHALLENGER"].ToString() + "/" + (drH[2]["IMATCHSTATUS"].ToString() == "0" ? "主" : "客") + "/" + drH[2]["IHOSTSCORE"].ToString() + "/" + drH[2]["IGUESTSCORE"].ToString() : "-1";
                                                                            dr3[8] = drH.Count > 3 ? drH[0]["LEAGUEALIAS"].ToString() + "/" + drH[3]["CCHALLENGER"].ToString() + "/" + (drH[3]["IMATCHSTATUS"].ToString() == "0" ? "主" : "客") + "/" + drH[3]["IHOSTSCORE"].ToString() + "/" + drH[3]["IGUESTSCORE"].ToString() : "-1";
                                                                            dr3[9] = drH.Count > 4 ? drH[0]["LEAGUEALIAS"].ToString() + "/" + drH[4]["CCHALLENGER"].ToString() + "/" + (drH[4]["IMATCHSTATUS"].ToString() == "0" ? "主" : "客") + "/" + drH[4]["IHOSTSCORE"].ToString() + "/" + drH[4]["IGUESTSCORE"].ToString() : "-1";
                                                                            dr3[10] = drGN.Count > 0 ? drGN[0]["LEAGUEALIAS"].ToString() + "/" + drGN[0]["CCHALLENGER"].ToString() + "/" + (drGN[0]["IMATCHSTATUS"].ToString() == "0" ? "主" : "客") : "-1/-1/-1";
                                                                            dr3[11] = drG.Count > 0 ? drG[0]["LEAGUEALIAS"].ToString() + "/" + drG[0]["CCHALLENGER"].ToString() + "/" + (drG[0]["IMATCHSTATUS"].ToString() == "0" ? "主" : "客") + "/" + drG[0]["IHOSTSCORE"].ToString() + "/" + drG[0]["IGUESTSCORE"].ToString() : "-1";
                                                                            dr3[12] = drG.Count > 1 ? drG[0]["LEAGUEALIAS"].ToString() + "/" + drG[1]["CCHALLENGER"].ToString() + "/" + (drG[1]["IMATCHSTATUS"].ToString() == "0" ? "主" : "客") + "/" + drG[1]["IHOSTSCORE"].ToString() + "/" + drG[1]["IGUESTSCORE"].ToString() : "-1";
                                                                            dr3[13] = drG.Count > 2 ? drG[0]["LEAGUEALIAS"].ToString() + "/" + drG[2]["CCHALLENGER"].ToString() + "/" + (drG[2]["IMATCHSTATUS"].ToString() == "0" ? "主" : "客") + "/" + drG[2]["IHOSTSCORE"].ToString() + "/" + drG[2]["IGUESTSCORE"].ToString() : "-1";
                                                                            dr3[14] = drG.Count > 3 ? drG[0]["LEAGUEALIAS"].ToString() + "/" + drG[3]["CCHALLENGER"].ToString() + "/" + (drG[3]["IMATCHSTATUS"].ToString() == "0" ? "主" : "客") + "/" + drG[3]["IHOSTSCORE"].ToString() + "/" + drG[3]["IGUESTSCORE"].ToString() : "-1";
                                                                            dr3[15] = drG.Count > 4 ? drG[0]["LEAGUEALIAS"].ToString() + "/" + drG[4]["CCHALLENGER"].ToString() + "/" + (drG[4]["IMATCHSTATUS"].ToString() == "0" ? "主" : "客") + "/" + drG[4]["IHOSTSCORE"].ToString() + "/" + drG[4]["IGUESTSCORE"].ToString() : "-1";
                                                                            // dr3[16] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                                                                            dr3[16] = DateTime.MinValue.ToString("yyyy-MM-dd HH:mm:ss.fff");
                                                                            dr3[17] = "0";
                                                                            data.Tables["data"].Rows.Add(dr3);
                                                                        }

                                                                        int count = fda.Update(data.Tables["data"]);
                                                                        Files.WriteLog((count > 0 ? "[Success] " : "[Failure] ") + " Insert ANALYSISRECENTS [" + data.Tables["data"].Rows.Count + "]");
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                    else if (i == 3)
                                                    {
                                                        continue;
                                                        try
                                                        {
                                                            foreach (DataRow dr in ds.Tables[0].Rows)
                                                            {
                                                                using (FbCommand cmd2 = new FbCommand("PR_AddTeam", connection2))
                                                                {
                                                                    cmd2.CommandType = CommandType.StoredProcedure;
                                                                    cmd2.Parameters.Add("@TEAM_ID", dr["tid"]);
                                                                    cmd2.Parameters.Add("@TEAMNAME", dr["hkjc_name_cn"]);
                                                                    cmd2.Parameters.Add("@COUNTRY", dr["COUNTRY_CHI_NAME"] is DBNull ? dr["COUNTRY"] : dr["COUNTRY_CHI_NAME"]);
                                                                    cmd2.Parameters.Add("@CITY", DBNull.Value);
                                                                    cmd2.Parameters.Add("@VENUE", DBNull.Value);
                                                                    cmd2.Parameters.Add("@CONTINENT", DBNull.Value);
                                                                    cmd2.Parameters.Add("@CENGNAME", dr["SHORT_NAME"]);
                                                                    cmd2.Parameters.Add("@CMACAUNAME", dr["hkjc_name_cn"]);
                                                                    cmd2.Parameters.Add("@CHKJCNAME", dr["hkjc_name_cn"]);
                                                                    cmd2.Parameters.Add("@CHKJCSHORTNAME", dr["hkjc_name_cn"]);
                                                                    cmd2.Parameters.Add("@LASTUPDATE", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                                                                    cmd2.Parameters.Add("@TIMEFLAG", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                                                                    int id = Convert.ToInt32(cmd2.ExecuteScalar());
                                                                    if (id > -1)
                                                                    {
                                                                        Files.WriteLog(" [Success] Insert TEAMINFO " + " " + dr["tid"] + " " + dr["hkjc_name_cn"] + "/" + dr["SHORT_NAME"]);
                                                                    }
                                                                }
                                                            }
                                                        }
                                                        catch (Exception exp)
                                                        {
                                                            Files.WriteError(DateTime.Now.ToString("HH:mm:ss ") + "Sync TEAM,error: " + exp);
                                                        }
                                                    }
                                                    //else if (i == 4)
                                                    //{
                                                    //    List<int> strE = ds.Tables[0].AsEnumerable().Select(d => d.Field<int>("id")).ToList<int>().Distinct().ToList<int>();
                                                    //    Files.WriteLog(strE.Count.ToString());
                                                    //    foreach (int s in strE)
                                                    //    {
                                                    //        Files.WriteLog(strE.Count.ToString() + " " + s);
                                                    //        DataRow[] drs = ds.Tables[0].Select("id=" + s);
                                                    //        List<int> strT = drs.Select(d => d.Field<int>("TEAM_ID")).ToList<int>().Distinct().ToList<int>();
                                                    //        if (strT.Count == 2)
                                                    //        {
                                                    //            //  Files.WriteLog(strT.Count.ToString() + " " + "t");
                                                    //            List<DataRow> drH = drs.AsEnumerable().ToList<DataRow>().Where(x => x.Field<int>("TEAM_ID") == strT[0]).ToList();
                                                    //            List<DataRow> drG = drs.AsEnumerable().ToList<DataRow>().Where(x => x.Field<int>("TEAM_ID") == strT[1]).ToList();

                                                    //            List<DataRow> sGH = drH.Where(x => x.Field<int>("IPOS") == 3).ToList();
                                                    //            List<DataRow> sGG = drG.Where(x => x.Field<int>("IPOS") == 3).ToList();
                                                    //            List<DataRow> sBH = drH.Where(x => x.Field<int>("IPOS") == 0).ToList();
                                                    //            List<DataRow> sBG = drG.Where(x => x.Field<int>("IPOS") == 0).ToList();
                                                    //            List<DataRow> sFH = drH.Where(x => x.Field<int>("IPOS") == 2).ToList();
                                                    //            List<DataRow> sFG = drG.Where(x => x.Field<int>("IPOS") == 2).ToList();
                                                    //            List<DataRow> sMH = drH.Where(x => x.Field<int>("IPOS") == 1).ToList();
                                                    //            List<DataRow> sMG = drG.Where(x => x.Field<int>("IPOS") == 1).ToList();
                                                    //            List<DataRow> sUH = drH.Where(x => x.Field<int>("IPOS") == 4).ToList();
                                                    //            List<DataRow> sUG = drG.Where(x => x.Field<int>("IPOS") == 4).ToList();

                                                    //            string sH = "";
                                                    //            string sG = "";
                                                    //            string ssBH = "", ssBG = "", ssFH = "", ssFG = "", ssMH = "", ssMG = "", ssUH = "", ssUG = "";
                                                    //            for (int j = 0; j < 5; j++)
                                                    //            {
                                                    //                if (j == 0)
                                                    //                {
                                                    //                    foreach (DataRow R in sBH)
                                                    //                    {
                                                    //                        ssBH += "B/" + R["IPLAYER_NO"].ToString() + "/" + (R["CPLAYER_NAME"].ToString() == "" ? R["CENGNAME"].ToString() : R["CPLAYER_NAME"].ToString()) + ",";
                                                    //                    }
                                                    //                    foreach (DataRow R in sBG)
                                                    //                    {
                                                    //                        ssBG += "B/" + R["IPLAYER_NO"].ToString() + "/" + (R["CPLAYER_NAME"].ToString() == "" ? R["CENGNAME"] : R["CPLAYER_NAME"].ToString()) + ",";
                                                    //                    }

                                                    //                }
                                                    //                if (j == 1)
                                                    //                {
                                                    //                    foreach (DataRow R in sMH)
                                                    //                    {
                                                    //                        ssMH += "M/" + R["IPLAYER_NO"].ToString() + "/" + (R["CPLAYER_NAME"].ToString() == "" ? R["CENGNAME"] : R["CPLAYER_NAME"].ToString()) + ",";
                                                    //                    }
                                                    //                    foreach (DataRow R in sMG)
                                                    //                    {
                                                    //                        ssMG += "M/" + R["IPLAYER_NO"].ToString() + "/" + (R["CPLAYER_NAME"].ToString() == "" ? R["CENGNAME"] : R["CPLAYER_NAME"].ToString()) + ",";
                                                    //                    }
                                                    //                }
                                                    //                if (j == 2)
                                                    //                {
                                                    //                    foreach (DataRow R in sFH)
                                                    //                    {
                                                    //                        ssFH += "F/" + R["IPLAYER_NO"].ToString() + "/" + (R["CPLAYER_NAME"].ToString() == "" ? R["CENGNAME"] : R["CPLAYER_NAME"].ToString()) + ",";
                                                    //                    }
                                                    //                    foreach (DataRow R in sFG)
                                                    //                    {
                                                    //                        ssFG += "F/" + R["IPLAYER_NO"].ToString() + "/" + (R["CPLAYER_NAME"].ToString() == "" ? R["CENGNAME"] : R["CPLAYER_NAME"].ToString()) + ",";
                                                    //                    }
                                                    //                }
                                                    //                if (j == 3)
                                                    //                {
                                                    //                    sH = sGH.Count > 0 ? "G/" + sGH[0]["IPLAYER_NO"].ToString() + "/" + (sGH[0]["CPLAYER_NAME"].ToString() == "" ? sGH[0]["CENGNAME"] : sGH[0]["CPLAYER_NAME"]) + "," : "";
                                                    //                    sG = sGG.Count > 0 ? "G/" + sGG[0]["IPLAYER_NO"].ToString() + "/" + (sGG[0]["CPLAYER_NAME"].ToString() == "" ? sGG[0]["CENGNAME"] : sGG[0]["CPLAYER_NAME"]) + "," : "";
                                                    //                }
                                                    //                if (j == 4)
                                                    //                {
                                                    //                    foreach (DataRow R in sUH)
                                                    //                    {
                                                    //                        if (R["IPLAYER_NO"].ToString() != "-1")
                                                    //                        {
                                                    //                            ssUH += "F/" + R["IPLAYER_NO"].ToString() + "/" + (R["CPLAYER_NAME"].ToString() == "" ? R["CENGNAME"] : R["CPLAYER_NAME"].ToString()) + ",";
                                                    //                        }
                                                    //                    }
                                                    //                    foreach (DataRow R in sUG)
                                                    //                    {
                                                    //                        if (R["IPLAYER_NO"].ToString() != "-1")
                                                    //                        {
                                                    //                            ssUG += "F/" + R["IPLAYER_NO"].ToString() + "/" + (R["CPLAYER_NAME"].ToString() == "" ? R["CENGNAME"] : R["CPLAYER_NAME"].ToString()) + ",";
                                                    //                        }
                                                    //                    }
                                                    //                }
                                                    //            }

                                                    //            using (FbCommand cmd2 = new FbCommand("PR_ADDPLAYERLIST", connection2))
                                                    //            {
                                                    //                cmd2.CommandType = CommandType.StoredProcedure;
                                                    //                cmd2.Parameters.Add("@CLEAGUE", drs[0]["ALIAS"]);
                                                    //                cmd2.Parameters.Add("@CHOST", drH[0]["HKJC_NAME_CN"]);
                                                    //                cmd2.Parameters.Add("@CGUEST", drG[0]["HKJC_NAME_CN"]);
                                                    //                cmd2.Parameters.Add("@CACTION", "U");
                                                    //                cmd2.Parameters.Add("@CH_PLAYER", (sH + "" + ssBH + "" + ssMH + "" + ssFH + ssUH).TrimEnd(','));
                                                    //                cmd2.Parameters.Add("@CG_PLAYER", (sG + "" + ssBG + "" + ssMG + "" + ssFG + ssUG).TrimEnd(','));
                                                    //                cmd2.Parameters.Add("@TIMEFLAG", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                                                    //                cmd2.Parameters.Add("@IDEST", "0");
                                                    //                int id = Convert.ToInt32(cmd2.ExecuteScalar());
                                                    //                if (id > -1)
                                                    //                {
                                                    //                    Files.WriteLog(" [Success] Insert PLAYERLIST " + " " + drs[0]["ALIAS"] + " " + drH[0]["HKJC_NAME_CN"] + "/" + drG[0]["HKJC_NAME_CN"]);
                                                    //                }
                                                    //            }
                                                    //        }
                                                    //    }
                                                    //    //List<int> strE = ds.Tables[0].AsEnumerable().Select(d => d.Field<int>("id")).ToList<int>().Distinct().ToList<int>();
                                                    //    //Files.WriteLog(strE.Count.ToString());
                                                    //    //foreach (int s in strE)
                                                    //    //{
                                                    //    //    Files.WriteLog(strE.Count.ToString() + " " + s);
                                                    //    //    DataRow[] drs = ds.Tables[0].Select("id=" + s);
                                                    //    //    List<int> strT = drs.Select(d => d.Field<int>("TEAM_ID")).ToList<int>().Distinct().ToList<int>();
                                                    //    //    if (strT.Count == 2)
                                                    //    //    {
                                                    //    //        Files.WriteLog(strT.Count.ToString() + " " + "t");
                                                    //    //        List<DataRow> drH = drs.AsEnumerable().ToList<DataRow>().Where(x => x.Field<int>("TEAM_ID") == strT[0]).ToList();
                                                    //    //        List<DataRow> drG = drs.AsEnumerable().ToList<DataRow>().Where(x => x.Field<int>("TEAM_ID") == strT[1]).ToList();

                                                    //    //        List<DataRow> sGH = drH.Where(x => x.Field<int>("IPOS") == 3).ToList();
                                                    //    //        List<DataRow> sGG = drG.Where(x => x.Field<int>("IPOS") == 3).ToList();
                                                    //    //        List<DataRow> sBH = drH.Where(x => x.Field<int>("IPOS") == 0).ToList();
                                                    //    //        List<DataRow> sBG = drG.Where(x => x.Field<int>("IPOS") == 0).ToList();
                                                    //    //        List<DataRow> sFH = drH.Where(x => x.Field<int>("IPOS") == 2).ToList();
                                                    //    //        List<DataRow> sFG = drG.Where(x => x.Field<int>("IPOS") == 2).ToList();
                                                    //    //        List<DataRow> sMH = drH.Where(x => x.Field<int>("IPOS") == 1).ToList();
                                                    //    //        List<DataRow> sMG = drG.Where(x => x.Field<int>("IPOS") == 1).ToList();
                                                    //    //        List<DataRow> sUH = drH.Where(x => x.Field<int>("IPOS") == 4).ToList();
                                                    //    //        List<DataRow> sUG = drG.Where(x => x.Field<int>("IPOS") == 4).ToList();

                                                    //    //        string sH = "";
                                                    //    //        string sG = "";
                                                    //    //        string ssBH = "", ssBG = "", ssFH = "", ssFG = "", ssMH = "", ssMG = "", ssUH = "", ssUG = "";
                                                    //    //        for (int j = 0; j < 5; j++)
                                                    //    //        {
                                                    //    //            if (j == 0)
                                                    //    //            {
                                                    //    //                foreach (DataRow R in sBH)
                                                    //    //                {
                                                    //    //                    ssBH += "B/" + R["IPLAYER_NO"].ToString() + "/" + (R["CPLAYER_NAME"].ToString() == "" ? R["CENGNAME"].ToString() : R["CPLAYER_NAME"].ToString()) + ",";
                                                    //    //                }
                                                    //    //                foreach (DataRow R in sBG)
                                                    //    //                {
                                                    //    //                    ssBG += "B/" + R["IPLAYER_NO"].ToString() + "/" + (R["CPLAYER_NAME"].ToString() == "" ? R["CENGNAME"] : R["CPLAYER_NAME"].ToString()) + ",";
                                                    //    //                }

                                                    //    //            }
                                                    //    //            if (j == 1)
                                                    //    //            {
                                                    //    //                foreach (DataRow R in sMH)
                                                    //    //                {
                                                    //    //                    ssMH += "M/" + R["IPLAYER_NO"].ToString() + "/" + (R["CPLAYER_NAME"].ToString() == "" ? R["CENGNAME"] : R["CPLAYER_NAME"].ToString()) + ",";
                                                    //    //                }
                                                    //    //                foreach (DataRow R in sMG)
                                                    //    //                {
                                                    //    //                    ssMG += "M/" + R["IPLAYER_NO"].ToString() + "/" + (R["CPLAYER_NAME"].ToString() == "" ? R["CENGNAME"] : R["CPLAYER_NAME"].ToString()) + ",";
                                                    //    //                }
                                                    //    //            }
                                                    //    //            if (j == 2)
                                                    //    //            {
                                                    //    //                foreach (DataRow R in sFH)
                                                    //    //                {
                                                    //    //                    ssFH += "F/" + R["IPLAYER_NO"].ToString() + "/" + (R["CPLAYER_NAME"].ToString() == "" ? R["CENGNAME"] : R["CPLAYER_NAME"].ToString()) + ",";
                                                    //    //                }
                                                    //    //                foreach (DataRow R in sFG)
                                                    //    //                {
                                                    //    //                    ssFG += "F/" + R["IPLAYER_NO"].ToString() + "/" + (R["CPLAYER_NAME"].ToString() == "" ? R["CENGNAME"] : R["CPLAYER_NAME"].ToString()) + ",";
                                                    //    //                }
                                                    //    //            }
                                                    //    //            if (j == 3)
                                                    //    //            {
                                                    //    //                sH = sGH.Count > 0 ? "G/" + sGH[0]["IPLAYER_NO"].ToString() + "/" + (sGH[0]["CPLAYER_NAME"].ToString() == "" ? sGH[0]["CENGNAME"] : sGH[0]["CPLAYER_NAME"]) : "";
                                                    //    //                sG = sGG.Count > 0 ? "G/" + sGG[0]["IPLAYER_NO"].ToString() + "/" + (sGG[0]["CPLAYER_NAME"].ToString() == "" ? sGG[0]["CENGNAME"] : sGG[0]["CPLAYER_NAME"]) : "";
                                                    //    //            }
                                                    //    //            if (j == 4)
                                                    //    //            {
                                                    //    //                foreach (DataRow R in sUH)
                                                    //    //                {
                                                    //    //                    ssUH += "U/" + R["IPLAYER_NO"].ToString() + "/" + (R["CPLAYER_NAME"].ToString() == "" ? R["CENGNAME"] : R["CPLAYER_NAME"].ToString()) + ",";
                                                    //    //                }
                                                    //    //                foreach (DataRow R in sUG)
                                                    //    //                {
                                                    //    //                    ssUG += "U/" + R["IPLAYER_NO"].ToString() + "/" + (R["CPLAYER_NAME"].ToString() == "" ? R["CENGNAME"] : R["CPLAYER_NAME"].ToString()) + ",";
                                                    //    //                }
                                                    //    //            }
                                                    //    //        }

                                                    //    //        using (FbCommand cmd2 = new FbCommand("PR_ADDPLAYERLIST", connection2))
                                                    //    //        {
                                                    //    //            cmd2.CommandType = CommandType.StoredProcedure;
                                                    //    //            cmd2.Parameters.Add("@CLEAGUE", drs[0]["ALIAS"]);
                                                    //    //            cmd2.Parameters.Add("@CHOST", drH[0]["HKJC_NAME_CN"]);
                                                    //    //            cmd2.Parameters.Add("@CGUEST", drG[0]["HKJC_NAME_CN"]);
                                                    //    //            cmd2.Parameters.Add("@CACTION", "U");
                                                    //    //            cmd2.Parameters.Add("@CH_PLAYER", sH + "" + ssBH + "" + ssMH + "" + ssFH + ssUH);
                                                    //    //            cmd2.Parameters.Add("@CG_PLAYER", sG + "" + ssBG + "" + ssMG + "" + ssFG + ssUG);
                                                    //    //            cmd2.Parameters.Add("@TIMEFLAG", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                                                    //    //            cmd2.Parameters.Add("@IDEST", "0");
                                                    //    //            int id = Convert.ToInt32(cmd2.ExecuteScalar());
                                                    //    //            if (id > -1)
                                                    //    //            {
                                                    //    //                Files.WriteLog(" [Success] Insert PLAYERLIST " + " " + drs[0]["ALIAS"] + " " + drH[0]["HKJC_NAME_CN"] + "/" + drG[0]["HKJC_NAME_CN"]);
                                                    //    //            }
                                                    //    //        }
                                                    //    //    }
                                                    //    //}
                                                    //}
                                                    else if (i == 5)
                                                    {
                                                        continue;
                                                        List<string> strL = ds.Tables[0].AsEnumerable().Select(d => d.Field<string>("cid")).ToList<string>().Distinct().ToList().ConvertAll<string>(x => x.ToString());
                                                        string strs = strL.Count == 0 ? "-1" : string.Concat("(", string.Join(" or ", strL), ")");

                                                        queryString = "SELECT  count (*) FROM FIXTURES r";
                                                        using (FbCommand cmd = new FbCommand(queryString, connection2))
                                                        {
                                                            int count = Convert.ToInt32(cmd.ExecuteScalar());
                                                            if (count > 0) strs = strs + " and e.id in ( " + ml.ToString() + ")";
                                                        }

                                                        queryString = "select e.id eid, e.COMPETITION_ID,c.ALIAS,l.LEAGUE_CHI_NAME, (select  hkjc_name_cn from teams where id= e.HOME_ID and hkjc_name_cn is not null ) HOME_ID,(select hkjc_name_cn from teams where id= e.GUEST_ID and hkjc_name_cn is not null) GUEST_ID,e.START_DATE, G.H_GOAL||':'||G.G_GOAL RESULT ,G.HH_GOAL||':'||G.GH_GOAL RESULT2 from events e " +
                                                            " inner join COMPETITIONS c on c.id = e.COMPETITION_ID   LEFT join LEAGUE_INFO l on l.CLEAGUE_ALIAS_NAME = c.ALIAS   LEFT JOIN GOALINFO G ON G.EMATCHID = E.ID" +
                                                            "  where" + strs; //" (c.id = 1507 or c.id = 1519 or c.id = 1528 or c.id = 1556 or c.id = 1599 or c.id = 1625 or c.id = 1639 or c.id = 1658 or c.id = 2131 or c.id = 2202) ";
                                                        using (FbCommand cmd = new FbCommand(queryString, connection))
                                                        {
                                                            using (FbDataAdapter fda = new FbDataAdapter(cmd))
                                                            {
                                                                using (DataSet data = new DataSet())
                                                                {
                                                                    data.Tables.Add(new DataTable("data"));
                                                                    fda.Fill(data.Tables["data"]);

                                                                    string ids = string.Concat("(", string.Join(") OR  (", data.Tables[0].AsEnumerable().Select(d => d.Field<int>("eid")).ToList<int>().Distinct().ToList().ConvertAll<string>(x => x.ToString())), ")");
                                                                    Files.WriteLog("Sync ids: " + ids);

                                                                    foreach (DataRow dr in data.Tables[0].Rows)
                                                                    {
                                                                        using (FbCommand cmd2 = new FbCommand("PR_ADDFIXTURE", connection2))
                                                                        {
                                                                            cmd2.CommandType = CommandType.StoredProcedure;
                                                                            cmd2.Parameters.Add("@CLEAGUEALIAS", dr["ALIAS"]);
                                                                            cmd2.Parameters.Add("@CLEAGUE", dr["LEAGUE_CHI_NAME"]);
                                                                            cmd2.Parameters.Add("@CHOST", dr["HOME_ID"] == DBNull.Value ? "" : dr["HOME_ID"]);
                                                                            cmd2.Parameters.Add("@CJCHOST", dr["HOME_ID"] == DBNull.Value ? "" : dr["HOME_ID"]);
                                                                            cmd2.Parameters.Add("@CGUEST", dr["GUEST_ID"] == DBNull.Value ? "" : dr["GUEST_ID"]);
                                                                            cmd2.Parameters.Add("@CJCGUEST", dr["GUEST_ID"] == DBNull.Value ? "" : dr["GUEST_ID"]);
                                                                            cmd2.Parameters.Add("@CMATCHFIELD", "H");
                                                                            cmd2.Parameters.Add("@CMATCHDATE", Convert.ToDateTime(dr["START_DATE"]).ToString("yyyyMMdd"));
                                                                            cmd2.Parameters.Add("@CMATCHTIME", Convert.ToDateTime(dr["START_DATE"]).ToString("HHmm"));
                                                                            cmd2.Parameters.Add("@CHALF_TIME", dr["RESULT2"] == DBNull.Value ? "" : dr["RESULT2"]);
                                                                            cmd2.Parameters.Add("@CFULL_TIME", dr["RESULT"] == DBNull.Value ? "" : dr["RESULT"]);
                                                                            cmd2.Parameters.Add("@CREMARK", "");
                                                                            cmd2.Parameters.Add("@CACTION", "U");
                                                                            cmd2.Parameters.Add("@CSCORER", "");
                                                                            cmd2.Parameters.Add("@TIMEFLAG", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                                                                            cmd2.Parameters.Add("@IDEST", "0");
                                                                            int id = Convert.ToInt32(cmd2.ExecuteScalar());
                                                                            if (id > -1)
                                                                            {
                                                                                Files.WriteLog(" [Success] Insert FIXTURE" + " " + dr["ALIAS"] + " " + dr["HOME_ID"] + "/" + dr["GUEST_ID"]);
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                    connection2.Close();
                                                }
                                            }
                                            int msg = -1;
                                            if (AppFlag.Alert)
                                            {
                                                switch (syncItems[i])
                                                {
                                                    case "LIVEGOALS":
                                                        {
                                                            msg = 30;
                                                        }
                                                        break;
                                                    case "GOALDETAILS":
                                                        {
                                                            msg = 31;
                                                        }
                                                        break;
                                                    case "ANALYSISOTHER":
                                                        {
                                                            msg = 62;
                                                        }
                                                        break;
                                                    case "FIXTURES":
                                                        {
                                                            msg = 63;
                                                        }
                                                        break;
                                                    case "ANALYSISTATS":
                                                        {
                                                            msg = 14;
                                                        }
                                                        break;
                                                    case "ANALYSISHISTORYS":
                                                        {
                                                            msg = 11;
                                                        }
                                                        break;
                                                    case "RANKS":
                                                        {
                                                            msg = 15;
                                                        }
                                                        break;
                                                    case "SCORERS":
                                                        {
                                                            msg = 17;
                                                        }
                                                        break;
                                                    case "ANALYSISRECENTS":
                                                        {
                                                            msg = 13;
                                                        }
                                                        break;
                                                    case "HKGOAL":
                                                        {
                                                            msg = 60;
                                                        }
                                                        break;
                                                    case "HKGOALDETAILS":
                                                        {
                                                            msg = 61;
                                                        }
                                                        break;
                                                        //case "ANALYSISPLAYERLIST":
                                                        //    {
                                                        //        msg = 12;
                                                        //    }
                                                        //    break;
                                                }
                                                SendAlertMsg(msg);
                                                Files.WriteLog(DateTime.Now.ToString("HH:mm:ss ") + "Send " + msg);
                                            }
                                        }
                                        connection.Close();
                                    }
                                }
                                catch (Exception exp)
                                {
                                    Files.WriteError(DateTime.Now.ToString("HH:mm:ss ") + syncItems[i] + ",error: " + exp);
                                }
                            }

                            // Files.WriteLog(" TimerTask " + DateTime.Now);
                            //var state = timerState as TimerState;
                            //Interlocked.Increment(ref state.Counter);
                        }
                        catch (Exception exp)
                        {
                            Files.WriteError("WndProc1(),error: " + exp.Message);

                        }
                    }
                    else if (mw.ToString().Length < 3)
                    {
                        string queryString = "";
                        DataSet ds = new DataSet();
                        if (mw == 10)
                        {
                            try
                            {
                                //string queryString = "";
                                //DataSet ds = new DataSet();
                                using (FbConnection connection = new FbConnection(AppFlag.ScoutsDBConn))
                                {
                                    connection.Open();
                                    this.listBox1.Invoke(new Action(() =>
                                    {
                                        this.listBox1.Invoke(new Action(() => { { this.listBox1.Items.Insert(0, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ") + "Received " + ml + "-ANALYSISBGREMARK"); } }));
                                    }));

                                    if (mw == 10)//ANALYSISBGREMARK
                                    {
                                        Files.WriteLog("Update " + ml + "-ANALYSISBGREMARK");
                                        //queryString = "SELECT g.WC_8,e.HKJCDAYCODE,e.HKJCMATCHNO,  E.CLEAGUEALIAS_OUTPUT_NAME,E.CLEAGUE_HKJC_NAME,E.HKJCHOSTNAME_CN, E.HKJCGUESTNAME_CN,E.CMATCHDATETIME CMATCHDATE, E.CMATCHDATETIME CMATCHTIME," +
                                        //   " '' CHANDICAP,'H' CMATCHFIELD,'' CHOSTROOT,   ''CGUESTROOT,''CVENUE,'' CTEMPERATURE,'' CWEATHERSTATUS,''CREMARK,current_timestamp  TIMEFLAG,'0'IDEST FROM EVENTS r" +
                                        //   " INNER JOIN  EMATCHES E ON E.EMATCHID = R.ID INNER JOIN EVENT_DETAILS G ON G.eventid = " +
                                        //            "E.EMATCHID WHERE R.ID = "+ml;

                                        queryString = "SELECT e.HKJCDAYCODE,e.HKJCMATCHNO,  E.CLEAGUEALIAS_OUTPUT_NAME,E.CLEAGUE_HKJC_NAME,E.HKJCHOSTNAME_CN, E.HKJCGUESTNAME_CN,E.CMATCHDATETIME CMATCHDATE, E.CMATCHDATETIME CMATCHTIME,'' CHANDICAP,'H' CMATCHFIELD,'' CHOSTROOT,   ''CGUESTROOT, a.CMATCH_VENUE,  a.CTEMPERATURE, a.IWEATHER_STATUS,ar.CREMARKS,current_timestamp TIMEFLAG,'0'IDEST FROM EVENTS r " +
                                                      "INNER JOIN  EMATCHES E ON E.EMATCHID = R.ID INNER JOIN EVENT_DETAILS G ON G.eventid = E.EMATCHID left join   ANALYSIS_BG_INFO a on a.IMATCH_CNT = r.id left join   ANALYSIS_REMARK_INFO ar on ar.IMATCH_CNT = r.id " +
                                                      "WHERE R.ID =" + ml;

                                        Files.WriteLog("Sql: " + queryString);

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

                                        if (ds.Tables[0].Rows.Count > 0)
                                        {
                                            using (FbConnection connection2 = new FbConnection(AppFlag.MangoDBConn))
                                            {
                                                connection2.Open();
                                                foreach (DataRow drH in ds.Tables[0].Rows)
                                                {
                                                    string wStatus = drH["IWEATHER_STATUS"].ToString();
                                                    if (wStatus == "0")
                                                    { wStatus = "多雲"; }
                                                    else if (wStatus == "1")
                                                    { wStatus = "良好"; }
                                                    else if (wStatus == "2")
                                                    { wStatus = "暴雨"; }
                                                    else if (wStatus == "3")
                                                    { wStatus = "暴雲"; }
                                                    else if (wStatus == "4")
                                                    { wStatus = "熱"; }
                                                    else if (wStatus == "5")
                                                    { wStatus = "小雨"; }
                                                    else if (wStatus == "6")
                                                    { wStatus = "小雪"; }
                                                    else if (wStatus == "7")
                                                    { wStatus = "多雨"; }
                                                    else if (wStatus == "8")
                                                    { wStatus = "暴风雨"; }
                                                    else if (wStatus == "9")
                                                    { wStatus = "晴朗"; }
                                                    else if (wStatus == "10")
                                                    { wStatus = "炎熱"; }
                                                    else if (wStatus == "11")
                                                    { wStatus = "有风"; }
                                                    else { wStatus = ""; }
                                                    using (FbCommand cmd2 = new FbCommand("PR_ANALYSISBGREMARK", connection2))
                                                    {
                                                        cmd2.CommandType = CommandType.StoredProcedure;
                                                        cmd2.Parameters.Add("@CLEAGUE", drH["CLEAGUE_HKJC_NAME"]);
                                                        cmd2.Parameters.Add("@CHOST", drH["HKJCHOSTNAME_CN"]);
                                                        cmd2.Parameters.Add("@CGUEST", drH["HKJCGUESTNAME_CN"]);
                                                        cmd2.Parameters.Add("@CMATCHDATE", Convert.ToDateTime(drH["CMATCHDATE"]).ToString("yyyyMMdd"));
                                                        cmd2.Parameters.Add("@CMATCHTIME", Convert.ToDateTime(drH["CMATCHTIME"]).ToString("HHmm"));
                                                        cmd2.Parameters.Add("@CHANDICAP", drH["CHANDICAP"]);
                                                        cmd2.Parameters.Add("@CMATCHFIELD", drH["CMATCHFIELD"]);
                                                        cmd2.Parameters.Add("@CHOSTROOT", drH["CHOSTROOT"]);
                                                        cmd2.Parameters.Add("@CGUESTROOT", drH["CGUESTROOT"]);
                                                        cmd2.Parameters.Add("@CVENUE", drH["CMATCH_VENUE"]);
                                                        cmd2.Parameters.Add("@CTEMPERATURE", drH["CTEMPERATURE"]);
                                                        cmd2.Parameters.Add("@CWEATHERSTATUS", wStatus);
                                                        cmd2.Parameters.Add("@CREMARK", drH["CREMARKS"]);
                                                        cmd2.Parameters.Add("@TIMEFLAG", Convert.ToDateTime(drH["TIMEFLAG"]).ToString("yyyy-MM-dd HH:mm:ss.fff"));
                                                        cmd2.Parameters.Add("@IDEST", drH["IDEST"]);
                                                        int id = Convert.ToInt32(cmd2.ExecuteScalar());
                                                        if (id > -1)
                                                        {
                                                            Files.WriteLog(" [Success] Insert ANALYSISBGREMARK " + " " + drH["CLEAGUEALIAS_OUTPUT_NAME"] + " " + drH["HKJCHOSTNAME_CN"] + "/" + drH["HKJCGUESTNAME_CN"]);
                                                        }
                                                    }
                                                }
                                                connection2.Close();
                                            }
                                        }

                                        SendAlertMsg(10);
                                    }
                                    //else if (mw == 15)//RANKS
                                    //{ 
                                    //}
                                    //else if (mw == 17)//SCORERS
                                    //{ 
                                    //}
                                    connection.Close();
                                }
                            }
                            catch (Exception exp)
                            {
                                Files.WriteError(DateTime.Now.ToString("HH:mm:ss ") + "Update ANALYSISBGREMARK,error: " + exp);
                            }
                        }
                        else if (mw == 12)
                        {
                            try
                            {
                                using (FbConnection connection = new FbConnection(AppFlag.ScoutsDBConn))
                                {
                                    connection.Open();
                                    this.listBox1.Invoke(new Action(() =>
                                    {
                                        this.listBox1.Invoke(new Action(() => { { this.listBox1.Items.Insert(0, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ") + "Received " + ml + "-ANALYSISPLAYERLIST"); } }));
                                    }));

                                    Files.WriteLog("Update " + ml + "-ANALYSISPLAYERLIST");
                                    //queryString = "select e.id eid, e.id,c.ALIAS, t.HKJC_NAME,t.HKJC_NAME_CN, p.*  from  PLAYERS_INFO p inner join teams t on t.id = p.TEAM_ID inner join events e on e.id = p.EVENT_ID inner join  COMPETITIONS c on c.id = e.COMPETITION_ID where p.EVENT_ID=" + ml + "   and p.IROSTER = 1 order by p.EVENT_ID, p.TEAM_ID  ,p.IPLAYER_NO ";
                                    queryString = "select (CASE WHEN T.ID=E.HOME_ID THEN 'H' ELSE 'G' END ) HG,l.LEAGUE_CHI_NAME,  e.id eid, e.id,c.ALIAS, t.HKJC_NAME,t.HKJC_NAME_CN, p.*  from  PLAYERS_INFO p inner join teams t on t.id = p.TEAM_ID inner join events e on e.id = p.EVENT_ID inner join  COMPETITIONS c on c.id = e.COMPETITION_ID inner join LEAGUE_INFO l on c.ALIAS =l.CLEAGUE_ALIAS_NAME where p.EVENT_ID=" + ml + " and e.START_DATE -1 <  current_timestamp and   current_timestamp <e.START_DATE+1    and p.IROSTER = 1 order by HG DESC,p.CTIMESTAMP desc  ";
                                    Files.WriteLog("Sql: " + queryString);

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
                                    connection.Close();
                                }
                                if (ds.Tables[0].Rows.Count > 0)
                                {
                                    using (FbConnection connection2 = new FbConnection(AppFlag.MangoDBConn))
                                    {
                                        connection2.Open();
                                        List<int> strE = ds.Tables[0].AsEnumerable().Select(d => d.Field<int>("id")).ToList<int>().Distinct().ToList<int>();
                                         foreach (int s in strE)
                                        { 
                                            DataRow[] drs = ds.Tables[0].Select("id=" + s);
                                            List<int> strT = drs.Select(d => d.Field<int>("TEAM_ID")).ToList<int>().Distinct().ToList<int>();
                                            if (strT.Count == 2)
                                            {
                                                //  Files.WriteLog(strT.Count.ToString() + " " + "t");
                                                List<DataRow> drH = drs.AsEnumerable().ToList<DataRow>().Where(x => x.Field<int>("TEAM_ID") == strT[0]).ToList();
                                                List<DataRow> drG = drs.AsEnumerable().ToList<DataRow>().Where(x => x.Field<int>("TEAM_ID") == strT[1]).ToList();

                                                List<DataRow> sGH = drH.Where(x => x.Field<int>("IPOS") == 3).ToList();
                                                List<DataRow> sGG = drG.Where(x => x.Field<int>("IPOS") == 3).ToList();
                                                List<DataRow> sBH = drH.Where(x => x.Field<int>("IPOS") == 0).ToList();
                                                List<DataRow> sBG = drG.Where(x => x.Field<int>("IPOS") == 0).ToList();
                                                List<DataRow> sFH = drH.Where(x => x.Field<int>("IPOS") == 2).ToList();
                                                List<DataRow> sFG = drG.Where(x => x.Field<int>("IPOS") == 2).ToList();
                                                List<DataRow> sMH = drH.Where(x => x.Field<int>("IPOS") == 1).ToList();
                                                List<DataRow> sMG = drG.Where(x => x.Field<int>("IPOS") == 1).ToList();
                                                List<DataRow> sUH = drH.Where(x => x.Field<int>("IPOS") == 4).ToList();
                                                List<DataRow> sUG = drG.Where(x => x.Field<int>("IPOS") == 4).ToList();

                                                string sH = "";
                                                string sG = "";
                                                string ssBH = "", ssBG = "", ssFH = "", ssFG = "", ssMH = "", ssMG = "", ssUH = "", ssUG = "";
                                                for (int j = 0; j < 5; j++)
                                                {
                                                    if (j == 0)
                                                    {
                                                        foreach (DataRow R in sBH)
                                                        {
                                                            ssBH += "B/" + R["IPLAYER_NO"].ToString() + "/" + (R["CPLAYER_NAME"].ToString() == "" ? R["CENGNAME"].ToString() : R["CPLAYER_NAME"].ToString()) + ",";
                                                        }
                                                        foreach (DataRow R in sBG)
                                                        {
                                                            ssBG += "B/" + R["IPLAYER_NO"].ToString() + "/" + (R["CPLAYER_NAME"].ToString() == "" ? R["CENGNAME"] : R["CPLAYER_NAME"].ToString()) + ",";
                                                        }

                                                    }
                                                    if (j == 1)
                                                    {
                                                        foreach (DataRow R in sMH)
                                                        {
                                                            ssMH += "M/" + R["IPLAYER_NO"].ToString() + "/" + (R["CPLAYER_NAME"].ToString() == "" ? R["CENGNAME"] : R["CPLAYER_NAME"].ToString()) + ",";
                                                        }
                                                        foreach (DataRow R in sMG)
                                                        {
                                                            ssMG += "M/" + R["IPLAYER_NO"].ToString() + "/" + (R["CPLAYER_NAME"].ToString() == "" ? R["CENGNAME"] : R["CPLAYER_NAME"].ToString()) + ",";
                                                        }
                                                    }
                                                    if (j == 2)
                                                    {
                                                        foreach (DataRow R in sFH)
                                                        {
                                                            ssFH += "F/" + R["IPLAYER_NO"].ToString() + "/" + (R["CPLAYER_NAME"].ToString() == "" ? R["CENGNAME"] : R["CPLAYER_NAME"].ToString()) + ",";
                                                        }
                                                        foreach (DataRow R in sFG)
                                                        {
                                                            ssFG += "F/" + R["IPLAYER_NO"].ToString() + "/" + (R["CPLAYER_NAME"].ToString() == "" ? R["CENGNAME"] : R["CPLAYER_NAME"].ToString()) + ",";
                                                        }
                                                    }
                                                    if (j == 3)
                                                    {
                                                        sH = sGH.Count > 0 ? "G/" + sGH[0]["IPLAYER_NO"].ToString() + "/" + (sGH[0]["CPLAYER_NAME"].ToString() == "" ? sGH[0]["CENGNAME"] : sGH[0]["CPLAYER_NAME"]) + "," : "";
                                                        sG = sGG.Count > 0 ? "G/" + sGG[0]["IPLAYER_NO"].ToString() + "/" + (sGG[0]["CPLAYER_NAME"].ToString() == "" ? sGG[0]["CENGNAME"] : sGG[0]["CPLAYER_NAME"]) + "," : "";
                                                    }
                                                    if (j == 4)
                                                    {
                                                        foreach (DataRow R in sUH)
                                                        {
                                                            if (R["IPLAYER_NO"].ToString() != "-2")
                                                            {
                                                                ssUH += "F/" + R["IPLAYER_NO"].ToString() + "/" + (R["CPLAYER_NAME"].ToString() == "" ? R["CENGNAME"] : R["CPLAYER_NAME"].ToString()) + ",";
                                                            }
                                                        }
                                                        foreach (DataRow R in sUG)
                                                        {
                                                            if (R["IPLAYER_NO"].ToString() != "-2")
                                                            {
                                                                ssUG += "F/" + R["IPLAYER_NO"].ToString() + "/" + (R["CPLAYER_NAME"].ToString() == "" ? R["CENGNAME"] : R["CPLAYER_NAME"].ToString()) + ",";
                                                            }
                                                        }
                                                    }
                                                }

                                                using (FbCommand cmd2 = new FbCommand("PR_ADDPLAYERLIST", connection2))
                                                {
                                                    cmd2.CommandType = CommandType.StoredProcedure;
                                                    cmd2.Parameters.Add("@CLEAGUE", drs[0]["LEAGUE_CHI_NAME"]);
                                                    cmd2.Parameters.Add("@CHOST", drH[0]["HKJC_NAME_CN"]);
                                                    cmd2.Parameters.Add("@CGUEST", drG[0]["HKJC_NAME_CN"]);
                                                    cmd2.Parameters.Add("@CACTION", "U");
                                                    cmd2.Parameters.Add("@CH_PLAYER", (sH + "" + ssBH + "" + ssMH + "" + ssFH + ssUH).TrimEnd(','));
                                                    cmd2.Parameters.Add("@CG_PLAYER", (sG + "" + ssBG + "" + ssMG + "" + ssFG + ssUG).TrimEnd(','));
                                                    cmd2.Parameters.Add("@TIMEFLAG", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                                                    cmd2.Parameters.Add("@IDEST", "0");
                                                    int id = Convert.ToInt32(cmd2.ExecuteScalar());
                                                    if (id > -1)
                                                    {
                                                        Files.WriteLog(" [Success] Insert PLAYERLIST " + " " + drs[0]["ALIAS"] + " " + drH[0]["HKJC_NAME_CN"] + "/" + drG[0]["HKJC_NAME_CN"]);
                                                    }
                                                }
                                            }
                                        }
                                        connection2.Close();
                                        SendAlertMsg(12);
                                    }
                                }
                                // connection.Close();
                                //  }
                            }
                            catch (Exception exp)
                            {
                                Files.WriteError(DateTime.Now.ToString("HH:mm:ss ") + "Update ANALYSISPLAYERLIST,error: " + exp);
                            }
                        }
                        else if (mw == 30)
                        {

                            //try
                            //{
                            //    using (FbConnection connection = new FbConnection(AppFlag.ScoutsDBConn))
                            //    {
                            //        connection.Open();
                            //        Files.WriteLog("Update ADDANALYSISOTHER " + ml.ToString());
                            //        this.listBox1.Invoke(new Action(() =>
                            //        {
                            //            this.listBox1.Invoke(new Action(() => { { this.listBox1.Items.Insert(0, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ") + "Received " + ml + "-ANALYSISOTHERS"); } }));
                            //        }));

                            //        queryString = "SELECT a.EVENTID, a.CLEAGUE aCLEAGUE, l.LEAGUE_CHI_NAME CLEAGUE, a.CTEAM, a.CTEAMTYPE, a.CACTION, a.CSHOTS, a.CFOULS, a.CCORNER_KICKS, a.COFFSIDES, a.CPOSSESSION, a.CYELLOW_CARDS, a.CRED_CARDS, a.CATTACKS, a.CSUBSTITUTIONS, a.CTHROWINS, a.CGOALKICKS, a.CTIMESTAMP FROM ANALYSIS_OTHERS a" +
                            //           " inner join  LEAGUE_INFO l on l.CLEAGUE_ALIAS_NAME  =a.CLEAGUE where a.EVENTID = " + ml.ToString() + " order by a.CTIMESTAMP desc ";

                            //        Files.WriteLog("Sql: " + queryString);


                            //        using (FbCommand cmd = new FbCommand(queryString, connection))
                            //        {
                            //            using (FbDataAdapter fda = new FbDataAdapter(cmd))
                            //            {
                            //                using (DataSet data = new DataSet())
                            //                {
                            //                    data.Tables.Add(new DataTable("data"));
                            //                    fda.Fill(data.Tables["data"]);
                            //                    ds = data;
                            //                }
                            //            }
                            //        }

                            //        if (ds.Tables[0].Rows.Count == 2)
                            //        {
                            //            using (FbConnection connection2 = new FbConnection(AppFlag.MangoDBConn))
                            //            {
                            //                List<DataRow> drH = ds.Tables[0].Select().AsEnumerable().ToList<DataRow>().Where(x => x.Field<string>("CTEAMTYPE") == "H").ToList();
                            //                List<DataRow> drG = ds.Tables[0].Select().AsEnumerable().ToList<DataRow>().Where(x => x.Field<string>("CTEAMTYPE") == "G").ToList();

                            //                connection2.Open();

                            //                using (FbCommand cmd2 = new FbCommand("PR_ADDANALYSISOTHER", connection2))
                            //                {
                            //                    cmd2.CommandType = CommandType.StoredProcedure;
                            //                    cmd2.Parameters.Add("@CLEAGUE", drH[0]["CLEAGUE"]);
                            //                    cmd2.Parameters.Add("@CHOST", drH[0]["CTEAM"]);
                            //                    cmd2.Parameters.Add("@CGUEST", drG[0]["CTEAM"]);
                            //                    cmd2.Parameters.Add("@CACTION", "U");
                            //                    cmd2.Parameters.Add("@CSHOTS", drH[0]["CSHOTS"] + "@" + drG[0]["CSHOTS"]);
                            //                    cmd2.Parameters.Add("@CFOULS", drH[0]["CFOULS"] + "@" + drG[0]["CFOULS"]);
                            //                    cmd2.Parameters.Add("@CCORNER_KICKS", drH[0]["CCORNER_KICKS"] + "@" + drG[0]["CCORNER_KICKS"]);
                            //                    cmd2.Parameters.Add("@COFFSIDES", drH[0]["COFFSIDES"] + "@" + drG[0]["COFFSIDES"]);
                            //                    cmd2.Parameters.Add("@CPOSSESSION", drH[0]["CPOSSESSION"] + "@" + drG[0]["CPOSSESSION"]);
                            //                    cmd2.Parameters.Add("@CYELLOW_CARDS", drH[0]["CYELLOW_CARDS"] + "@" + drG[0]["CYELLOW_CARDS"]);
                            //                    cmd2.Parameters.Add("@CRED_CARDS", drH[0]["CRED_CARDS"] + "@" + drG[0]["CRED_CARDS"]);
                            //                    cmd2.Parameters.Add("@TIMEFLAG", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                            //                    cmd2.Parameters.Add("@IDEST", "0");
                            //                    int id = Convert.ToInt32(cmd2.ExecuteScalar());
                            //                    if (id > -1)
                            //                    {
                            //                        Files.WriteLog(" [Success] Insert ADDANALYSISOTHER " + " " + drH[0]["CLEAGUE"] + " " + drH[0]["CTEAM"] + "/" + drG[0]["CTEAM"]);
                            //                    }
                            //                }

                            //                connection2.Close();
                            //            }
                            //        }
                            //        connection.Close();
                            //    }
                            //    SendAlertMsg(62);
                            //}
                            //catch (Exception exp)
                            //{
                            //    Files.WriteError(DateTime.Now.ToString("HH:mm:ss ") + "ANALYSISOTHERS,error: " + exp);
                            //}

                            try
                            {
                                using (FbConnection connection = new FbConnection(AppFlag.ScoutsDBConn))
                                {
                                    connection.Open();
                                    this.listBox1.Invoke(new Action(() =>
                                    {
                                        this.listBox1.Invoke(new Action(() => { { this.listBox1.Items.Insert(0, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ") + "Received " + ml + "-LIVEGOALS"); } }));
                                    }));

                                    Files.WriteLog("Update LIVEGOALS " + ml.ToString());
                                    queryString = "SELECT FIRST 1 g.tolive, (select  FIRST 1  t.HKJC_NAME_CN from teams t  where t.id= r.HOME_ID) H,(select  FIRST 1  t.HKJC_NAME_CN from teams t  where t.id=r.GUEST_ID) G,  E.CLEAGUEALIAS_OUTPUT_NAME,E.CLEAGUE_HKJC_NAME,E.HKJCHOSTNAME_CN, E.HKJCGUESTNAME_CN," +
                                      " E.MAPPINGSTATUS, ( CASE WHEN E.MAPPINGSTATUS IS TRUE THEN E.HKJCGUESTNAME_CN ELSE E.HKJCHOSTNAME_CN END ) HH,  ( CASE WHEN E.MAPPINGSTATUS IS TRUE THEN E.HKJCHOSTNAME_CN ELSE E.HKJCGUESTNAME_CN END ) GG, " +
                                     "E.CMATCHDATETIME CMATCHDATE,E.CMATCHDATETIME CMATCHTIME,'H' CMATCHFIELD,'U' CACTION,G.H_GOAL,G.G_GOAL,G.H_RED,G.G_RED,G.HH_GOAL,G.GH_GOAL,G.H_CONFIRM,G.G_CONFIRM,''CSONGID,''CALERT,G.GAMESTATUS,  " +
                                      " ((select count(*) from MATCHDETAILS e where e.EMATCHID=" + ml.ToString() + " and (e.STATUS='Extratime 1st half' or e.STATUS='Extratime 2nd half') and e.CTYPE='goal' and e.TEAM_ID=r.HOME_ID)||'-'||" +
                                    " (select count(*) from MATCHDETAILS e where e.EMATCHID = " + ml.ToString() + " and(e.STATUS = 'Extratime 1st half' or e.STATUS = 'Extratime 2nd half') and e.CTYPE = 'goal' and e.TEAM_ID = r.GUEST_ID) || ';' ||" +
                                    " (select count(*) from MATCHDETAILS e where e.EMATCHID = " + ml.ToString() + " and(e.STATUS = 'Penalty shootout') and e.CTYPE = 'goal' and e.TEAM_ID = r.HOME_ID)|| '-' ||" +
                                    " (select count(*) from MATCHDETAILS e where e.EMATCHID = " + ml.ToString() + " and(e.STATUS = 'Penalty shootout') and e.CTYPE = 'goal' and e.TEAM_ID = r.GUEST_ID) )" +
                                    " CCOMMENT,g.ELAPSED CTIMEOFGAME,current_timestamp TIMEFLAG,'0'IDEST FROM EVENTS r INNER JOIN  EMATCHES E ON E.EMATCHID = R.ID " +
                                    "left JOIN GOALINFO G ON G.EMATCHID = E.EMATCHID WHERE R.ID =" + ml.ToString() + " ORDER BY E.CTIMESTAMP DESC ";
                                    Files.WriteLog("Sql: " + queryString);

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
                                    connection.Close();
                                }
                                if (ds.Tables[0].Rows.Count > 0)
                                {
                                    bool bToLive = false;
                                    using (FbConnection connection2 = new FbConnection(AppFlag.MangoDBConn))
                                    {
                                        connection2.Open();
                                        DataRow drH = ds.Tables[0].Rows[0];
                                        // if (drH["CLEAGUE_HKJC_NAME"] is DBNull || drH["CLEAGUEALIAS_OUTPUT_NAME"] is DBNull
                                        //     || drH["HKJCHOSTNAME_CN"] is DBNull || drH["HKJCGUESTNAME_CN"] is DBNull
                                        //|| drH["H"] is DBNull || drH["G"] is DBNull) { }
                                        // else
                                        // { 
                                        bToLive = drH["Tolive"].ToString() == "1" ? true : false;
                                      string   sLeague = drH["CLEAGUE_HKJC_NAME"].ToString();
                                        string sHost = drH["H"] is DBNull ? drH["HH"].ToString() : drH["H"].ToString();
                                        string sGuest = drH["G"] is DBNull ? drH["GG"].ToString() : drH["G"].ToString();
                                        using (FbCommand cmd2 = new FbCommand("PR_LIVEGOALS", connection2))
                                        {
                                            cmd2.CommandType = CommandType.StoredProcedure;
                                            cmd2.Parameters.Add("@CLEAGUEALIAS", drH["CLEAGUEALIAS_OUTPUT_NAME"]);
                                            cmd2.Parameters.Add("@CLEAGUE", drH["CLEAGUE_HKJC_NAME"]);
                                            cmd2.Parameters.Add("@CHOST", drH["H"] is DBNull ? drH["HH"] : drH["H"]);
                                            cmd2.Parameters.Add("@CGUEST", drH["G"] is DBNull ? drH["GG"] : drH["G"]);
                                            cmd2.Parameters.Add("@CMATCHDATE", Convert.ToDateTime(drH["CMATCHDATE"]).ToString("yyyyMMdd"));
                                            cmd2.Parameters.Add("@CMATCHTIME", Convert.ToDateTime(drH["CMATCHTIME"]).ToString("HHmm"));
                                            cmd2.Parameters.Add("@CMATCHFIELD", drH["CMATCHFIELD"]);
                                            cmd2.Parameters.Add("@CACTION", drH["CACTION"]);
                                            cmd2.Parameters.Add("@IH_GOAL", drH["H_GOAL"].ToString() == "" ? "0" : drH["H_GOAL"].ToString());
                                            cmd2.Parameters.Add("@IG_GOAL", drH["G_GOAL"].ToString() == "" ? "0" : drH["G_GOAL"].ToString());
                                            cmd2.Parameters.Add("@IH_REDCARD", drH["H_RED"].ToString() == "" ? "0" : drH["H_RED"].ToString());
                                            cmd2.Parameters.Add("@IG_REDCARD", drH["G_RED"].ToString() == "" ? "0" : drH["G_RED"].ToString());
                                            cmd2.Parameters.Add("@IH_HT_GOAL", drH["HH_GOAL"].ToString() == "" ? "0" : drH["HH_GOAL"].ToString());
                                            cmd2.Parameters.Add("@IG_HT_GOAL", drH["GH_GOAL"].ToString() == "" ? "0" : drH["GH_GOAL"].ToString());
                                            cmd2.Parameters.Add("@CSONGID", drH["CSONGID"]);
                                            cmd2.Parameters.Add("@CALERT", drH["CALERT"]);
                                            cmd2.Parameters.Add("@CSTATUS", drH["GAMESTATUS"]);
                                            // cmd2.Parameters.Add("@CCOMMENT", drH["CCOMMENT"]);
                                            cmd2.Parameters.Add("@CCOMMENT", drH["CCOMMENT"].ToString().Replace("0-0;0-0", "").Replace(";", ";PK").Replace(";PK0-0", ""));
                                            cmd2.Parameters.Add("@CTIMEOFGAME", drH["CTIMEOFGAME"] == DBNull.Value ? "-1" : drH["CTIMEOFGAME"]);
                                            cmd2.Parameters.Add("@TIMEFLAG", Convert.ToDateTime(drH["TIMEFLAG"]).ToString("yyyy-MM-dd HH:mm:ss.fff"));
                                            cmd2.Parameters.Add("@IDEST", drH["IDEST"]);
                                            int id = Convert.ToInt32(cmd2.ExecuteScalar());
                                            Files.WriteLog(" [Success] " + (id > 0 ? "Insert" : "Update") + " LIVEGOALS " + " " + drH["CLEAGUEALIAS_OUTPUT_NAME"] + " " + drH["HKJCHOSTNAME_CN"] + "/" + drH["HKJCGUESTNAME_CN"]);
                                            Files.WriteLog("Sql: " + cmd2.CommandText + " " + drH["CLEAGUEALIAS_OUTPUT_NAME"] + " " + drH["CLEAGUE_HKJC_NAME"] + " " + drH["HKJCHOSTNAME_CN"] + " " + drH["HKJCGUESTNAME_CN"] + " " + Convert.ToDateTime(drH["CMATCHDATE"]).ToString("yyyyMMdd") + " " + Convert.ToDateTime(drH["CMATCHDATE"]).ToString("HHmm") + " " + drH["GAMESTATUS"]);
                                        }
                                        connection2.Close();
                                        //}
                                        if (bToLive) SendToLiveDB(sLeague,sHost,sGuest,"","");
                                    }
                                }
                                SendAlertMsg(30);
                                SendAlertMsg(62);

                            }
                            catch (Exception exp)
                            {
                                Files.WriteError(DateTime.Now.ToString("HH:mm:ss ") + "Update LIVEGOAL,error: " + exp);
                            }

                        }
                        else if (mw == 31)
                        {
                            try
                            {
                                using (FbConnection connection = new FbConnection(AppFlag.ScoutsDBConn))
                                {
                                    connection.Open();
                                    this.listBox1.Invoke(new Action(() =>
                                    {
                                        this.listBox1.Invoke(new Action(() => { { this.listBox1.Items.Insert(0, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ") + "Received " + ml + "-GOALDETAILS"); } }));
                                    }));

                                    Files.WriteLog("Update GOALDETAILS " + ml.ToString());//distinct
                                    queryString = " SELECT G.TOLIVE,  (select  FIRST 1  t.HKJC_NAME_CN from teams t  where t.id= s.HOME_ID) H,(select  FIRST 1  t.HKJC_NAME_CN from teams t  where t.id=s.GUEST_ID) G, E.CLEAGUEALIAS_OUTPUT_NAME,E.CLEAGUE_HKJC_NAME,E.HKJCHOSTNAME_CN, E.HKJCGUESTNAME_CN," +
                                                " 'F' CCURRENTSTATUS, '' CPK, 'U' CACTION, '' CALERT, R.CTYPE CRECORDTYPE, R.HG CRECORDBELONG,  r.STATUS CMATCHSTATUS, r.ELAPSED , (select count(*) from MATCHDETAILS x where x.EMATCHID=g.EMATCHID and cast( x.ELAPSED as integer)<=cast( r.ELAPSED as integer) and x.CTYPE='goal' and x.HG='H' " +
                                            //    " AND (x.STATUS!='Penalty shootout' AND x.ELAPSED!=105) and ( x.STATUS!='Extratime 1st half' and x.STATUS!='Extratime 2nd half')" +
                                                ")  CSCOREHOST, (select count(*) from MATCHDETAILS x where x.EMATCHID=g.EMATCHID and cast( x.ELAPSED as integer)<=cast( r.ELAPSED as integer) and x.CTYPE='goal' and x.HG='G' " +
                                                //    " AND (x.STATUS!='Penalty shootout' AND x.ELAPSED!=105)  and ( x.STATUS!='Extratime 1st half' and x.STATUS!='Extratime 2nd half')" +
                                                ")  CSCOREGUEST, '-1' CSCORENUM,   '0' CSCOREOWNGOAL, r.PLAYERCHI CSCORER,r.PLAYER CSCORER2 , r.LASTTIME TIMEFLAG, '0' IDEST " +
                                                "FROM MATCHDETAILS r   INNER JOIN  EMATCHES E ON E.EMATCHID = r.EMATCHID   INNER JOIN  events s ON s.id = r.EMATCHID  INNER JOIN GOALINFO G ON G.EMATCHID= E.EMATCHID where r.EMATCHID =" + ml.ToString() + " AND (R.CTYPE='goal'  or r.CTYPE='rcard') " +
                                            //    " AND (R.STATUS!='Penalty shootout' AND R.ELAPSED!=105)  and ( r.STATUS!='Extratime 1st half' and r.STATUS!='Extratime 2nd half')" +
                                                " order by r.LASTTIME asc";
                                    Files.WriteLog("Sql GOALDETAILS: " + queryString);

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
                                    //    connection.Close();
                                    //}
                                    bool bTolive = false;
                                    string sLeague="",sHost="", sGuest="";
                                    using (FbConnection connection2 = new FbConnection(AppFlag.MangoDBConn))
                                    {
                                        connection2.Open();
                                        if (ds.Tables[0].Rows.Count > 0)
                                        {
                                            //using (FbConnection connection2 = new FbConnection(AppFlag.MangoDBConn))
                                            //{
                                            //    connection2.Open();
                                            queryString = "delete from GOALDETAILS g where g.CLEAGUE='" + ds.Tables[0].Rows[0]["CLEAGUE_HKJC_NAME"].ToString() + "' and ((g.CHOST='" + ds.Tables[0].Rows[0]["HKJCHOSTNAME_CN"].ToString() + "'  and g.CGUEST='" + ds.Tables[0].Rows[0]["HKJCGUESTNAME_CN"].ToString() + "') OR (g.CHOST='" + ds.Tables[0].Rows[0]["HKJCGUESTNAME_CN"].ToString() + "'  and g.CGUEST='" + ds.Tables[0].Rows[0]["HKJCHOSTNAME_CN"].ToString() + "')) ";
                                            using (FbCommand cmd2 = new FbCommand(queryString, connection2))
                                            {
                                                int id = Convert.ToInt32(cmd2.ExecuteScalar());
                                                Files.WriteLog("Sql: " + queryString);
                                            }

                                            bTolive = ds.Tables[0].Rows[0]["TOLIVE"].ToString() == "1" ? true : false;
                                            sLeague = ds.Tables[0].Rows[0]["CLEAGUE_HKJC_NAME"].ToString();
                                            sHost = ds.Tables[0].Rows[0]["HKJCHOSTNAME_CN"].ToString();
                                            sGuest = ds.Tables[0].Rows[0]["HKJCGUESTNAME_CN"].ToString();
                                            foreach (DataRow drH in ds.Tables[0].Rows)
                                            {
                                                try
                                                {
                                                    if (drH["CLEAGUEALIAS_OUTPUT_NAME"] is DBNull || drH["CLEAGUE_HKJC_NAME"] is DBNull
                                                        || drH["HKJCHOSTNAME_CN"] is DBNull || drH["HKJCGUESTNAME_CN"] is DBNull)
                                                    //  || drH["H"] is DBNull || drH["G"] is DBNull)
                                                    { continue; }
                                                    using (FbCommand cmd2 = new FbCommand("PR_GOALDETAILS", connection2))
                                                    {
                                                        cmd2.CommandType = CommandType.StoredProcedure;
                                                        cmd2.Parameters.Add("@CLEAGUEALIAS", drH["CLEAGUEALIAS_OUTPUT_NAME"]);
                                                        cmd2.Parameters.Add("@CLEAGUE", drH["CLEAGUE_HKJC_NAME"]);
                                                        cmd2.Parameters.Add("@CHOST", drH["H"] is DBNull ? drH["HKJCHOSTNAME_CN"] : drH["H"]);
                                                        cmd2.Parameters.Add("@CGUEST", drH["G"] is DBNull ? drH["HKJCGUESTNAME_CN"] : drH["G"]);
                                                        cmd2.Parameters.Add("@CCURRENTSTATUS", drH["CCURRENTSTATUS"]);
                                                        cmd2.Parameters.Add("@CPK", drH["CPK"]);
                                                        cmd2.Parameters.Add("@CACTION", drH["CACTION"]);
                                                        cmd2.Parameters.Add("@CALERT", drH["CALERT"]);
                                                        cmd2.Parameters.Add("@CRECORDTYPE", drH["CRECORDTYPE"]);
                                                        cmd2.Parameters.Add("@CRECORDBELONG", drH["CRECORDBELONG"]);
                                                        cmd2.Parameters.Add("@CMATCHSTATUS", drH["CMATCHSTATUS"]);
                                                        cmd2.Parameters.Add("@CMATCHTIME", drH["ELAPSED"]);
                                                        cmd2.Parameters.Add("@CSCOREHOST", drH["CSCOREHOST"]);
                                                        cmd2.Parameters.Add("@CSCOREGUEST", drH["CSCOREGUEST"]);
                                                        cmd2.Parameters.Add("@CSCORENUM", drH["CSCORENUM"]);
                                                        cmd2.Parameters.Add("@CSCOREOWNGOAL", drH["CSCOREOWNGOAL"]);
                                                        cmd2.Parameters.Add("@CSCORER", drH["CSCORER"] is DBNull ? drH["CSCORER2"].ToString() : drH["CSCORER"].ToString());
                                                        cmd2.Parameters.Add("@TIMEFLAG", Convert.ToDateTime(drH["TIMEFLAG"]).ToString("yyyy-MM-dd HH:mm:ss.fff"));
                                                        cmd2.Parameters.Add("@IDEST", drH["IDEST"]);
                                                        int id = Convert.ToInt32(cmd2.ExecuteScalar());
                                                        if (id > -1)
                                                        {
                                                            Files.WriteLog(" [Success] Insert/Update GOALDETAILS " + " " + drH["CLEAGUEALIAS_OUTPUT_NAME"] + " " + drH["HKJCHOSTNAME_CN"] + "/" + drH["HKJCGUESTNAME_CN"] 
                                                                +" "+ drH["CMATCHSTATUS"] +" "+ drH["ELAPSED"] + " " + drH["CRECORDBELONG"] + " " + drH["CRECORDTYPE"] + " " + drH["CSCOREHOST"] + " " + drH["CSCOREGUEST"] + " " + drH["CSCOREOWNGOAL"]
                                                                + " " + (drH["CSCORER"] is DBNull ? drH["CSCORER2"].ToString() : drH["CSCORER"].ToString()) + " " + Convert.ToDateTime(drH["TIMEFLAG"]).ToString("yyyy-MM-dd HH:mm:ss.fff")
                                                                );
                                                        }
                                                    }
                                                }
                                                catch (Exception exp)
                                                {
                                                    Files.WriteError(DateTime.Now.ToString("HH:mm:ss ") + "GOALDETAILS,error2: " + exp);
                                                    continue;
                                                }
                                            }
                                            //    connection2.Close();
                                            //}
                                            ///   SendToLiveDB(ds.Tables[0].Rows[0]["CLEAGUE_HKJC_NAME"].ToString(), ds.Tables[0].Rows[0]["H"] is DBNull ? ds.Tables[0].Rows[0]["HH"].ToString() : ds.Tables[0].Rows[0]["H"].ToString(), ds.Tables[0].Rows[0]["G"] is DBNull ? ds.Tables[0].Rows[0]["GG"].ToString() : ds.Tables[0].Rows[0]["G"].ToString(), "", "");
                                        }
                                        else
                                        {
                                            queryString = "SELECt r.EMATCHID,  r.CMATCHDATETIME,    r.MAPPINGSTATUS,   r.HKJCHOSTNAME_CN, r.HKJCGUESTNAME_CN,r.CLEAGUE_HKJC_NAME, r.CLEAGUEALIAS_OUTPUT_NAME, " +
                                                " ( CASE WHEN r.MAPPINGSTATUS IS TRUE THEN r.HKJCGUESTNAME_CN ELSE r.HKJCHOSTNAME_CN END ) HH,  ( CASE WHEN r.MAPPINGSTATUS IS TRUE THEN r.HKJCHOSTNAME_CN ELSE r.HKJCGUESTNAME_CN END ) GG," +
                                                " (select  FIRST 1  t.HKJC_NAME_CN from teams t  where t.id= e.HOME_ID) H,(select  FIRST 1  t.HKJC_NAME_CN from teams t  where t.id=e.GUEST_ID) G FROM EMATCHES r inner join events e on e.id =r.EMATCHID " +
                                                " where r.EMATCHID =" + ml.ToString();
                                            Files.WriteLog("Sql2: " + queryString);

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
                                            if (ds.Tables[0].Rows.Count > 0)
                                            {
                                                queryString = "delete from GOALDETAILS g where g.CLEAGUE='" + ds.Tables[0].Rows[0]["CLEAGUE_HKJC_NAME"].ToString() + "' and ((g.CHOST='" + ds.Tables[0].Rows[0]["HKJCHOSTNAME_CN"].ToString() + "'  and g.CGUEST='" + ds.Tables[0].Rows[0]["HKJCGUESTNAME_CN"].ToString() + "') OR (g.CHOST='" + ds.Tables[0].Rows[0]["HKJCGUESTNAME_CN"].ToString() + "'  and g.CGUEST='" + ds.Tables[0].Rows[0]["HKJCHOSTNAME_CN"].ToString() + "')) ";
                                                using (FbCommand cmd2 = new FbCommand(queryString, connection2))
                                                {
                                                    int id = Convert.ToInt32(cmd2.ExecuteScalar());
                                                    Files.WriteLog("Sql2: " + queryString);
                                                }
                                                //    DataRow drH = ds.Tables[0].Rows[0];
                                                //    if (drH["CLEAGUEALIAS_OUTPUT_NAME"] is DBNull || drH["CLEAGUE_HKJC_NAME"] is DBNull
                                                //                || drH["HKJCHOSTNAME_CN"] is DBNull || drH["HKJCGUESTNAME_CN"] is DBNull)
                                                //    { }
                                                //    else
                                                //    {
                                                //        using (FbCommand cmd2 = new FbCommand("PR_GOALDETAILS", connection2))
                                                //        {
                                                //            cmd2.CommandType = CommandType.StoredProcedure;
                                                //            cmd2.Parameters.Add("@CLEAGUEALIAS", drH["CLEAGUEALIAS_OUTPUT_NAME"]);
                                                //            cmd2.Parameters.Add("@CLEAGUE", drH["CLEAGUE_HKJC_NAME"]);
                                                //            cmd2.Parameters.Add("@CHOST", drH["H"] is DBNull ? drH["HH"] : drH["H"]);
                                                //            cmd2.Parameters.Add("@CGUEST", drH["G"] is DBNull ? drH["GG"] : drH["G"]);
                                                //            cmd2.Parameters.Add("@CCURRENTSTATUS", "F");
                                                //            cmd2.Parameters.Add("@CPK", "");
                                                //            cmd2.Parameters.Add("@CACTION", "U");
                                                //            cmd2.Parameters.Add("@CALERT", "");
                                                //            cmd2.Parameters.Add("@CRECORDTYPE", "goal");
                                                //            cmd2.Parameters.Add("@CRECORDBELONG", "H");
                                                //            cmd2.Parameters.Add("@CMATCHSTATUS", "1st half");
                                                //            cmd2.Parameters.Add("@CMATCHTIME", "0");
                                                //            cmd2.Parameters.Add("@CSCOREHOST", "0");
                                                //            cmd2.Parameters.Add("@CSCOREGUEST", "0");
                                                //            cmd2.Parameters.Add("@CSCORENUM", "-1");
                                                //            cmd2.Parameters.Add("@CSCOREOWNGOAL", "0");
                                                //            cmd2.Parameters.Add("@CSCORER", "");
                                                //            cmd2.Parameters.Add("@TIMEFLAG", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                                                //            cmd2.Parameters.Add("@IDEST", "0");
                                                //            int id = Convert.ToInt32(cmd2.ExecuteScalar());
                                                //            if (id > -1)
                                                //            {
                                                //                Files.WriteLog(" [Success] Init GOALDETAILS " + " " + drH["CLEAGUEALIAS_OUTPUT_NAME"] + " " + drH["HKJCHOSTNAME_CN"] + "/" + drH["HKJCGUESTNAME_CN"]);
                                                //            }
                                                //        }
                                                //    }
                                            }
                                        }
                                        connection2.Close();
                                    }
                                    connection.Close();

                                    SendAlertMsg(31);
                                    if (bTolive) SendToLiveDB(sLeague, sHost, sGuest, "", "");
                                }
                               // SendAlertMsg(31); 
                            }
                            catch (Exception exp)
                            {
                                Files.WriteError(DateTime.Now.ToString("HH:mm:ss ") + "Update GOALDETAILS,error: " + exp);
                            }
                            /*
                                try
                        {
                            using (FbConnection connection = new FbConnection(AppFlag.ScoutsDBConn))
                            {
                                connection.Open();
                                this.listBox1.Invoke(new Action(() =>
                                {
                                    this.listBox1.Invoke(new Action(() => { { this.listBox1.Items.Insert(0, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ") + "Received " + ml + "-GOALDETAILS"); } }));
                                }));

                                Files.WriteLog("Update GOALDETAILS " + ml.ToString());
                                queryString = " SELECT distinct  (select  FIRST 1  t.HKJC_NAME_CN from teams t  where t.id= s.HOME_ID) H,(select  FIRST 1  t.HKJC_NAME_CN from teams t  where t.id=s.GUEST_ID) G, E.CLEAGUEALIAS_OUTPUT_NAME,E.CLEAGUE_HKJC_NAME,E.HKJCHOSTNAME_CN, E.HKJCGUESTNAME_CN," +
                                            " 'F' CCURRENTSTATUS, '' CPK, 'U' CACTION, '' CALERT, R.CTYPE CRECORDTYPE, R.HG CRECORDBELONG,  r.STATUS CMATCHSTATUS, r.ELAPSED , (select count(*) from MATCHDETAILS x where x.EMATCHID=g.EMATCHID and cast( x.ELAPSED as integer)<=cast( r.ELAPSED as integer) and x.CTYPE='goal' and x.HG='H'AND (x.STATUS!='Penalty shootout' AND x.ELAPSED!=105) and ( x.STATUS!='Extratime 1st half' and x.STATUS!='Extratime 2nd half'))  CSCOREHOST, (select count(*) from MATCHDETAILS x where x.EMATCHID=g.EMATCHID and cast( x.ELAPSED as integer)<=cast( r.ELAPSED as integer) and x.CTYPE='goal' and x.HG='G' AND (x.STATUS!='Penalty shootout' AND x.ELAPSED!=105)  and ( x.STATUS!='Extratime 1st half' and x.STATUS!='Extratime 2nd half'))  CSCOREGUEST, '-1' CSCORENUM,   '0' CSCOREOWNGOAL, r.PLAYERCHI CSCORER,r.PLAYER CSCORER2 , current_timestamp TIMEFLAG, '0' IDEST " +
                                            "FROM MATCHDETAILS r   INNER JOIN  EMATCHES E ON E.EMATCHID = r.EMATCHID   INNER JOIN  events s ON s.id = r.EMATCHID  INNER JOIN GOALINFO G ON G.EMATCHID= E.EMATCHID where r.EMATCHID =" + ml.ToString() + " AND (R.CTYPE='goal'  or r.CTYPE='rcard') AND (R.STATUS!='Penalty shootout' AND R.ELAPSED!=105)  and ( r.STATUS!='Extratime 1st half' and r.STATUS!='Extratime 2nd half') order by r.ELAPSED asc";
                                Files.WriteLog("Sql: " + queryString);

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
                                connection.Close();
                            }
                            if (ds.Tables[0].Rows.Count > 0)
                            {
                                using (FbConnection connection2 = new FbConnection(AppFlag.MangoDBConn))
                                {
                                    connection2.Open();
                                    queryString = "delete from GOALDETAILS g where g.CLEAGUE='" + ds.Tables[0].Rows[0]["CLEAGUE_HKJC_NAME"].ToString() + "' and ((g.CHOST='" + ds.Tables[0].Rows[0]["HKJCHOSTNAME_CN"].ToString() + "'  and g.CGUEST='" + ds.Tables[0].Rows[0]["HKJCGUESTNAME_CN"].ToString() + "') OR (g.CHOST='" + ds.Tables[0].Rows[0]["HKJCGUESTNAME_CN"].ToString() + "'  and g.CGUEST='" + ds.Tables[0].Rows[0]["HKJCHOSTNAME_CN"].ToString() + "')) ";
                                    using (FbCommand cmd2 = new FbCommand(queryString, connection2))
                                    {
                                        int id = Convert.ToInt32(cmd2.ExecuteScalar());
                                        Files.WriteLog("Sql: " + id.ToString() + " " + queryString);
                                    }

                                    //if (ds.Tables[0].Rows.Count > 0)
                                    //{
                                    foreach (DataRow drH in ds.Tables[0].Rows)
                                    {
                                        try
                                        {
                                            if (drH["CLEAGUEALIAS_OUTPUT_NAME"] is DBNull || drH["CLEAGUE_HKJC_NAME"] is DBNull
                                                || drH["HKJCHOSTNAME_CN"] is DBNull || drH["HKJCGUESTNAME_CN"] is DBNull)
                                            //  || drH["H"] is DBNull || drH["G"] is DBNull)
                                            { continue; }
                                            using (FbCommand cmd2 = new FbCommand("PR_GOALDETAILS", connection2))
                                            {
                                                cmd2.CommandType = CommandType.StoredProcedure;
                                                cmd2.Parameters.Add("@CLEAGUEALIAS", drH["CLEAGUEALIAS_OUTPUT_NAME"]);
                                                cmd2.Parameters.Add("@CLEAGUE", drH["CLEAGUE_HKJC_NAME"]);
                                                cmd2.Parameters.Add("@CHOST", drH["H"] is DBNull ? drH["HKJCHOSTNAME_CN"] : drH["H"]);
                                                cmd2.Parameters.Add("@CGUEST", drH["G"] is DBNull ? drH["HKJCGUESTNAME_CN"] : drH["G"]);
                                                cmd2.Parameters.Add("@CCURRENTSTATUS", drH["CCURRENTSTATUS"]);
                                                cmd2.Parameters.Add("@CPK", drH["CPK"]);
                                                cmd2.Parameters.Add("@CACTION", drH["CACTION"]);
                                                cmd2.Parameters.Add("@CALERT", drH["CALERT"]);
                                                cmd2.Parameters.Add("@CRECORDTYPE", drH["CRECORDTYPE"]);
                                                cmd2.Parameters.Add("@CRECORDBELONG", drH["CRECORDBELONG"]);
                                                cmd2.Parameters.Add("@CMATCHSTATUS", drH["CMATCHSTATUS"]);
                                                cmd2.Parameters.Add("@CMATCHTIME", drH["ELAPSED"]);
                                                cmd2.Parameters.Add("@CSCOREHOST", drH["CSCOREHOST"]);
                                                cmd2.Parameters.Add("@CSCOREGUEST", drH["CSCOREGUEST"]);
                                                cmd2.Parameters.Add("@CSCORENUM", drH["CSCORENUM"]);
                                                cmd2.Parameters.Add("@CSCOREOWNGOAL", drH["CSCOREOWNGOAL"]);
                                                cmd2.Parameters.Add("@CSCORER", drH["CSCORER"] is DBNull ? drH["CSCORER2"].ToString() : drH["CSCORER"].ToString());
                                                cmd2.Parameters.Add("@TIMEFLAG", Convert.ToDateTime(drH["TIMEFLAG"]).ToString("yyyy-MM-dd HH:mm:ss.fff"));
                                                cmd2.Parameters.Add("@IDEST", drH["IDEST"]);
                                                int id = Convert.ToInt32(cmd2.ExecuteScalar());
                                                if (id > -1)
                                                {
                                                    Files.WriteLog(" [Success] Insert/Update GOALDETAILS " + " " + drH["CLEAGUEALIAS_OUTPUT_NAME"] + " " + drH["HKJCHOSTNAME_CN"] + "/" + drH["HKJCGUESTNAME_CN"]);
                                                }
                                            }
                                        }
                                        catch (Exception exp)
                                        {
                                            Files.WriteError(DateTime.Now.ToString("HH:mm:ss ") + "GOALDETAILS,error2: " + exp);
                                            continue;
                                        }
                                    }
                                    //}
                                    connection2.Close();
                                }
                            }
                            SendAlertMsg(31);
                        }
                        catch (Exception exp)
                        {
                            Files.WriteError(DateTime.Now.ToString("HH:mm:ss ") + "Update GOALDETAILS,error: " + exp);
                        }  
                      */
                        }
                        else if (mw == 25)
                        {
                            try
                            {
                                using (FbConnection connection = new FbConnection(AppFlag.ScoutsDBConn))
                                {
                                    connection.Open();
                                    this.listBox1.Invoke(new Action(() =>
                                    {
                                        this.listBox1.Invoke(new Action(() => { { this.listBox1.Items.Insert(0, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ") + "Received " + ml + "-ANALYSISOTHER"); } }));
                                    }));

                                    Files.WriteLog("Update ANALYSISOTHER " + ml.ToString());
                                    queryString = "SELECT a.EVENTID, a.CLEAGUE aCLEAGUE, l.LEAGUE_CHI_NAME CLEAGUE, a.CTEAM, a.CTEAMTYPE, a.CACTION, a.CSHOTS, a.CFOULS, a.CCORNER_KICKS, a.COFFSIDES, a.CPOSSESSION, a.CYELLOW_CARDS, a.CRED_CARDS, a.CATTACKS, a.CSUBSTITUTIONS, a.CTHROWINS, a.CGOALKICKS, a.CTIMESTAMP FROM ANALYSIS_OTHERS a" +
                                   " inner join  LEAGUE_INFO l on l.CLEAGUE_ALIAS_NAME  =a.CLEAGUE where a.EVENTID = " + ml.ToString() + " order by a.CTIMESTAMP desc ";
                                    Files.WriteLog("Sql: " + queryString);

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
                                    connection.Close();
                                }

                                if (ds.Tables[0].Rows.Count == 2)
                                {
                                    using (FbConnection connection2 = new FbConnection(AppFlag.MangoDBConn))
                                    {
                                        List<DataRow> drH = ds.Tables[0].Select().AsEnumerable().ToList<DataRow>().Where(x => x.Field<string>("CTEAMTYPE") == "H").ToList();
                                        List<DataRow> drG = ds.Tables[0].Select().AsEnumerable().ToList<DataRow>().Where(x => x.Field<string>("CTEAMTYPE") == "G").ToList();

                                        connection2.Open();

                                        using (FbCommand cmd2 = new FbCommand("PR_ADDANALYSISOTHER", connection2))
                                        {
                                            cmd2.CommandType = CommandType.StoredProcedure;
                                            cmd2.Parameters.Add("@CLEAGUE", drH[0]["CLEAGUE"]);
                                            cmd2.Parameters.Add("@CHOST", drH[0]["CTEAM"]);
                                            cmd2.Parameters.Add("@CGUEST", drG[0]["CTEAM"]);
                                            cmd2.Parameters.Add("@CACTION", "U");
                                            cmd2.Parameters.Add("@CSHOTS", drH[0]["CSHOTS"] + "@" + drG[0]["CSHOTS"]);
                                            cmd2.Parameters.Add("@CFOULS", drH[0]["CFOULS"] + "@" + drG[0]["CFOULS"]);
                                            cmd2.Parameters.Add("@CCORNER_KICKS", drH[0]["CCORNER_KICKS"] + "@" + drG[0]["CCORNER_KICKS"]);
                                            cmd2.Parameters.Add("@COFFSIDES", drH[0]["COFFSIDES"] + "@" + drG[0]["COFFSIDES"]);
                                            cmd2.Parameters.Add("@CPOSSESSION", drH[0]["CPOSSESSION"] + "@" + drG[0]["CPOSSESSION"]);
                                            cmd2.Parameters.Add("@CYELLOW_CARDS", drH[0]["CYELLOW_CARDS"] + "@" + drG[0]["CYELLOW_CARDS"]);
                                            cmd2.Parameters.Add("@CRED_CARDS", drH[0]["CRED_CARDS"] + "@" + drG[0]["CRED_CARDS"]);
                                            cmd2.Parameters.Add("@TIMEFLAG", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                                            cmd2.Parameters.Add("@IDEST", "0");
                                            int id = Convert.ToInt32(cmd2.ExecuteScalar());
                                            if (id > -1)
                                            {
                                                Files.WriteLog(" [Success] Insert ADDANALYSISOTHER " + " " + drH[0]["CLEAGUE"] + " " + drH[0]["CTEAM"] + "/" + drG[0]["CTEAM"]);
                                            }
                                        }

                                        connection2.Close();
                                    }
                                }
                                SendAlertMsg(25);
                            }
                            catch (Exception exp)
                            {
                                Files.WriteError(DateTime.Now.ToString("HH:mm:ss ") + "Update ANALYSISOTHER,error: " + exp);
                            }
                        }
                        else if (mw == 14)
                        {
                            try
                            {
                                using (FbConnection connection = new FbConnection(AppFlag.ScoutsDBConn))
                                {
                                    connection.Open();
                                    this.listBox1.Invoke(new Action(() =>
                                    {
                                        this.listBox1.Invoke(new Action(() => { { this.listBox1.Items.Insert(0, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ") + "Received " + ml + "-ANALYSISTATS"); } }));
                                    }));

                                    Files.WriteLog("Update ANALYSISTATS " + ml.ToString());
                                    if (ml == -1)
                                    {
                                        queryString = "select E.CLEAGUE_OUTPUT_NAME,  E.HKJCHOSTNAME_CN, E.HKJCGUESTNAME_CN, E.EMATCHID,E.HKJCDAYCODE,E.HKJCMATCHNO from EMATCHES E    WHERE e.HKJCDAYCODE = (SELECT first 1 HKJCDAYCODE FROM EMATCHES WHERE  cast(cast(CMATCHDATETIME as date) as varchar(10)) = cast(cast(current_timestamp as date) as varchar(10)) and STATUS!='finished'  order by CMATCHDATETIME desc    )   and e.CMATCHDATETIME >= (SELECT  first 1 CMATCHDATETIME FROM EMATCHES WHERE  cast(cast(CMATCHDATETIME as date) as varchar(10)) = cast(cast(current_timestamp as date) -1 as varchar(10)) )  order by e.HKJCMATCHNO asc ";
                                    }
                                    else
                                    {
                                        queryString = "select  e.EMATCHID eid,   e.CLEAGUE_HKJC_NAME,  e.CLEAGUEALIAS_OUTPUT_NAME, e.HKJCHOSTNAME_CN, e.HKJCGUESTNAME_CN, e.CMATCHDATETIME,  a.*   from ANALYSIS_STAT_INFO  a inner join EMATCHES e on e.EMATCHID =a.IMATCH_CNT    WHERE e.HKJCDAYCODE = (SELECT HKJCDAYCODE FROM EMATCHES WHERE EMATCHID =" + ml + "  ) and e.CMATCHDATETIME <= (SELECT CMATCHDATETIME FROM EMATCHES WHERE EMATCHID =  " + ml + " )+1 and e.CMATCHDATETIME >= (SELECT CMATCHDATETIME FROM EMATCHES WHERE EMATCHID =  " + ml + " ) -1  order by e.HKJCMATCHNO asc";
                                    }
                                    Files.WriteLog("Sql: " + queryString);

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
                                    connection.Close();
                                }
                                if (ds.Tables[0].Rows.Count > 0)
                                {
                                    using (FbConnection connection2 = new FbConnection(AppFlag.MangoDBConn))
                                    {
                                        connection2.Open();
                                        foreach (DataRow dr in ds.Tables[0].Rows)
                                        {
                                            using (FbCommand cmd2 = new FbCommand("PR_ANALYSIS_STAT", connection2))
                                            {
                                                cmd2.CommandType = CommandType.StoredProcedure;
                                                cmd2.Parameters.Add("@CLEAGUE", dr["CLEAGUE_HKJC_NAME"]);
                                                cmd2.Parameters.Add("@CHOST", dr["HKJCHOSTNAME_CN"]);
                                                cmd2.Parameters.Add("@CGUEST", dr["HKJCGUESTNAME_CN"]);
                                                cmd2.Parameters.Add("@IMATCHDATE", Convert.ToDateTime(dr["CMATCHDATETIME"]).ToString("yyyyMMdd"));
                                                cmd2.Parameters.Add("@IMATCHTIME", Convert.ToDateTime(dr["CMATCHDATETIME"]).ToString("HHmmss"));
                                                cmd2.Parameters.Add("@CHANDICAP", "1");
                                                cmd2.Parameters.Add("@CMATCHFIELD", "H");
                                                cmd2.Parameters.Add("@IHOSTWIN", dr["IHOSTWIN"]);
                                                cmd2.Parameters.Add("@IHOSTDRAW", dr["IHOSTDRAW"]);
                                                cmd2.Parameters.Add("@IHOSTLOSS", dr["IHOSTLOSS"]);
                                                cmd2.Parameters.Add("@IGUESTWIN", dr["IGUESTWIN"]);
                                                cmd2.Parameters.Add("@IGUESTDRAW", dr["IGUESTDRAW"]);
                                                cmd2.Parameters.Add("@IGUESTLOSS", dr["IGUESTLOSS"]);
                                                cmd2.Parameters.Add("@CREMARK", "");
                                                cmd2.Parameters.Add("@TIMEFLAG", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                                                cmd2.Parameters.Add("@IDEST", "0");
                                                int id = Convert.ToInt32(cmd2.ExecuteScalar());
                                                if (id > -1)
                                                {
                                                    Files.WriteLog(" [Success] Insert ANALYSIS_STAT " + " " + dr["CLEAGUEALIAS_OUTPUT_NAME"] + " " + dr["HKJCHOSTNAME_CN"] + "/" + dr["HKJCGUESTNAME_CN"]);
                                                }
                                            }
                                        }
                                        connection2.Close();
                                    }
                                }
                                SendAlertMsg(14);
                            }
                            catch (Exception exp)
                            {
                                Files.WriteError(DateTime.Now.ToString("HH:mm:ss ") + "Update ANALYSISTATS,error: " + exp);
                            }
                        }
                        else if (mw == 13)
                        {
                            try
                            {
                                using (FbConnection connection = new FbConnection(AppFlag.ScoutsDBConn))
                                {
                                    connection.Open();
                                    this.listBox1.Invoke(new Action(() =>
                                    {
                                        this.listBox1.Invoke(new Action(() => { { this.listBox1.Items.Insert(0, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ") + "Received " + ml + "-ANALYSISRECENTS"); } }));
                                    }));

                                    Files.WriteLog("Update ANALYSISRECENTS " + ml.ToString());
                                    queryString = "select e.EMATCHID eid,   'CLEAGUE=''' || e.CLEAGUE_HKJC_NAME || ''' AND CHOST=''' || e.HKJCHOSTNAME_CN || ''' AND CGUEST=''' || e.HKJCGUESTNAME_CN || ''' ' ABC, e.CLEAGUE_HKJC_NAME CLEAGUE, e.CLEAGUEALIAS_OUTPUT_NAME  , e.HKJCHOSTNAME_CN CHOST, e.HKJCGUESTNAME_CN CGUEST, e.CMATCHDATETIME,  a.* from ANALYSIS_RECENT_INFO a inner join EMATCHES e on e.EMATCHID = a.IMATCH_CNT   where a.IMATCH_CNT=" + ml.ToString() + "  ORDER BY  a.IMATCH_CNT ,a.irec asc ";
                                    Files.WriteLog("Sql: " + queryString);

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
                                    connection.Close();
                                }

                                if (ds.Tables[0].Rows.Count > 0)
                                {
                                    using (FbConnection connection2 = new FbConnection(AppFlag.MangoDBConn))
                                    {
                                        connection2.Open();
                                        using (FbCommand cmd2 = new FbCommand())
                                        {
                                            //List<string> strL = ds.Tables[0].AsEnumerable().Select(d => d.Field<string>("ABC")).ToList<string>().Distinct().ToList();
                                            //string strs = string.Concat("(", string.Join(") OR  (", strL), ")");
                                            cmd2.CommandText = "delete from ANALYSISRECENTS where CLEAGUE='" + ds.Tables[0].Rows[0]["CLEAGUE"].ToString() + "' and CHOST='" + ds.Tables[0].Rows[0]["CHOST"].ToString() + "' and CGUEST='" + ds.Tables[0].Rows[0]["CGUEST"].ToString() + "'";
                                            cmd2.Connection = connection2;
                                            int id = Convert.ToInt32(cmd2.ExecuteScalar());
                                            Files.WriteLog("Sql: " + "delete from ANALYSISRECENTS where CLEAGUE='" + ds.Tables[0].Rows[0]["CLEAGUE"].ToString() + "' and CHOST='" + ds.Tables[0].Rows[0]["CHOST"].ToString() + "' and CGUEST='" + ds.Tables[0].Rows[0]["CGUEST"].ToString() + "'");
                                        }

                                        queryString = "select first   1 * from ANALYSISRECENTS";
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
                                                        List<string> strTeam = ds.Tables[0].AsEnumerable().Select(d => d.Field<string>("ABC")).ToList<string>().Distinct().ToList();

                                                        foreach (string s in strTeam)
                                                        {
                                                            DataRow[] drs = ds.Tables[0].Select(s);
                                                            List<DataRow> drH = (drs.AsEnumerable().ToList<DataRow>().Where(x => x.Field<string>("CTEAMFLAG") == "H").ToList()).ToList<DataRow>().Where(x => x.Field<int>("IHOSTSCORE") != -1).ToList();
                                                            List<DataRow> drG = (drs.AsEnumerable().ToList<DataRow>().Where(x => x.Field<string>("CTEAMFLAG") == "G").ToList()).ToList<DataRow>().Where(x => x.Field<int>("IHOSTSCORE") != -1).ToList();
                                                            List<DataRow> drHN = (drs.AsEnumerable().ToList<DataRow>().Where(x => x.Field<string>("CTEAMFLAG") == "H").ToList()).ToList<DataRow>().Where(x => x.Field<int>("IHOSTSCORE") == -1).ToList();
                                                            List<DataRow> drGN = (drs.AsEnumerable().ToList<DataRow>().Where(x => x.Field<string>("CTEAMFLAG") == "G").ToList()).ToList<DataRow>().Where(x => x.Field<int>("IHOSTSCORE") == -1).ToList();

                                                            DataRow dr3 = data.Tables["data"].NewRow();
                                                            dr3[0] = drH.Count > 0 ? drH[0]["CLEAGUE"] : drG.Count > 0 ? drG[0][2] : "";
                                                            dr3[1] = drH.Count > 0 ? drH[0]["CHOST"] : drG.Count > 0 ? drG[0][3] : "";
                                                            dr3[2] = drH.Count > 0 ? drH[0]["CGUEST"] : drG.Count > 0 ? drG[0][4] : "";
                                                            dr3[3] = "U";
                                                            dr3[4] = drHN.Count > 0 ? drHN[0]["LEAGUEALIAS"].ToString() + "/" + drHN[0]["CCHALLENGER"].ToString() + "/" + (drHN[0]["IMATCHSTATUS"].ToString() == "0" ? "主" : "客") : "-1/-1/-1";
                                                            dr3[5] = drH.Count > 0 ? drH[0]["LEAGUEALIAS"].ToString() + "/" + drH[0]["CCHALLENGER"].ToString() + "/" + (drH[0]["IMATCHSTATUS"].ToString() == "0" ? "主" : "客") + "/" + drH[0]["IHOSTSCORE"].ToString() + "/" + drH[0]["IGUESTSCORE"].ToString() : "-1";
                                                            dr3[6] = drH.Count > 1 ? drH[0]["LEAGUEALIAS"].ToString() + "/" + drH[1]["CCHALLENGER"].ToString() + "/" + (drH[1]["IMATCHSTATUS"].ToString() == "0" ? "主" : "客") + "/" + drH[1]["IHOSTSCORE"].ToString() + "/" + drH[1]["IGUESTSCORE"].ToString() : "-1";
                                                            dr3[7] = drH.Count > 2 ? drH[0]["LEAGUEALIAS"].ToString() + "/" + drH[2]["CCHALLENGER"].ToString() + "/" + (drH[2]["IMATCHSTATUS"].ToString() == "0" ? "主" : "客") + "/" + drH[2]["IHOSTSCORE"].ToString() + "/" + drH[2]["IGUESTSCORE"].ToString() : "-1";
                                                            dr3[8] = drH.Count > 3 ? drH[0]["LEAGUEALIAS"].ToString() + "/" + drH[3]["CCHALLENGER"].ToString() + "/" + (drH[3]["IMATCHSTATUS"].ToString() == "0" ? "主" : "客") + "/" + drH[3]["IHOSTSCORE"].ToString() + "/" + drH[3]["IGUESTSCORE"].ToString() : "-1";
                                                            dr3[9] = drH.Count > 4 ? drH[0]["LEAGUEALIAS"].ToString() + "/" + drH[4]["CCHALLENGER"].ToString() + "/" + (drH[4]["IMATCHSTATUS"].ToString() == "0" ? "主" : "客") + "/" + drH[4]["IHOSTSCORE"].ToString() + "/" + drH[4]["IGUESTSCORE"].ToString() : "-1";
                                                            dr3[10] = drGN.Count > 0 ? drGN[0]["LEAGUEALIAS"].ToString() + "/" + drGN[0]["CCHALLENGER"].ToString() + "/" + (drGN[0]["IMATCHSTATUS"].ToString() == "0" ? "主" : "客") : "-1/-1/-1";
                                                            dr3[11] = drG.Count > 0 ? drG[0]["LEAGUEALIAS"].ToString() + "/" + drG[0]["CCHALLENGER"].ToString() + "/" + (drG[0]["IMATCHSTATUS"].ToString() == "0" ? "主" : "客") + "/" + drG[0]["IHOSTSCORE"].ToString() + "/" + drG[0]["IGUESTSCORE"].ToString() : "-1";
                                                            dr3[12] = drG.Count > 1 ? drG[0]["LEAGUEALIAS"].ToString() + "/" + drG[1]["CCHALLENGER"].ToString() + "/" + (drG[1]["IMATCHSTATUS"].ToString() == "0" ? "主" : "客") + "/" + drG[1]["IHOSTSCORE"].ToString() + "/" + drG[1]["IGUESTSCORE"].ToString() : "-1";
                                                            dr3[13] = drG.Count > 2 ? drG[0]["LEAGUEALIAS"].ToString() + "/" + drG[2]["CCHALLENGER"].ToString() + "/" + (drG[2]["IMATCHSTATUS"].ToString() == "0" ? "主" : "客") + "/" + drG[2]["IHOSTSCORE"].ToString() + "/" + drG[2]["IGUESTSCORE"].ToString() : "-1";
                                                            dr3[14] = drG.Count > 3 ? drG[0]["LEAGUEALIAS"].ToString() + "/" + drG[3]["CCHALLENGER"].ToString() + "/" + (drG[3]["IMATCHSTATUS"].ToString() == "0" ? "主" : "客") + "/" + drG[3]["IHOSTSCORE"].ToString() + "/" + drG[3]["IGUESTSCORE"].ToString() : "-1";
                                                            dr3[15] = drG.Count > 4 ? drG[0]["LEAGUEALIAS"].ToString() + "/" + drG[4]["CCHALLENGER"].ToString() + "/" + (drG[4]["IMATCHSTATUS"].ToString() == "0" ? "主" : "客") + "/" + drG[4]["IHOSTSCORE"].ToString() + "/" + drG[4]["IGUESTSCORE"].ToString() : "-1";
                                                            dr3[16] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                                                            dr3[17] = "0";
                                                            data.Tables["data"].Rows.Add(dr3);
                                                        }

                                                        int count = fda.Update(data.Tables["data"]);
                                                        Files.WriteLog((count > 0 ? "[Success] " : "[Failure] ") + " Insert ANALYSISRECENTS [" + data.Tables["data"].Rows.Count + "]");
                                                    }
                                                }
                                            }
                                        }
                                        connection2.Close();
                                    }
                                    SendAlertMsg(13);
                                }
                            }
                            catch (Exception exp)
                            {
                                Files.WriteError(DateTime.Now.ToString("HH:mm:ss ") + "Update ANALYSISRECENTS,error: " + exp);
                            }
                        }
                        else if (mw == 11)
                        {
                            try
                            {
                                using (FbConnection connection = new FbConnection(AppFlag.ScoutsDBConn))
                                {
                                    connection.Open();
                                    this.listBox1.Invoke(new Action(() =>
                                    {
                                        this.listBox1.Invoke(new Action(() => { { this.listBox1.Items.Insert(0, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ") + "Received " + ml + "-ANALYSISHISTORYS"); } }));
                                    }));

                                    Files.WriteLog("Update ANALYSISHISTORYS " + ml.ToString());
                                    // queryString = "select  FIRST 4  e.EMATCHID eid,  'CLEAGUEALIAS='''||  e.CLEAGUEALIAS_OUTPUT_NAME||''' AND CHOST=''' || e.HKJCHOSTNAME_CN||''' AND CGUEST='''|| e.HKJCGUESTNAME_CN||''' and cdate='''|| replace( cast(a.START_DATE as date),'-','')||''' ' ABC, e.CLEAGUE_HKJC_NAME,  e.CLEAGUEALIAS_OUTPUT_NAME, e.HKJCHOSTNAME_CN, e.HKJCGUESTNAME_CN, e.CMATCHDATETIME,  a.*   from ANALYSIS_HISTORY_INFO  a inner join EMATCHES e on e.EMATCHID =a.IMATCH_CNT   where a.IMATCH_CNT=" + ml.ToString() + " ORDER BY  a.imatch_cnt ,a.irec asc   ";
                                    queryString = "select  FIRST 4  e.EMATCHID eid,  'CLEAGUEALIAS='''||  e.CLEAGUEALIAS_OUTPUT_NAME||''' AND CHOST=''' || e.HKJCHOSTNAME_CN||''' AND CGUEST='''|| e.HKJCGUESTNAME_CN||''' and cdate='''||  a.IMATCHYEAR   ||   right( '0'|| CAST(a.IMATCHMONTH as VARCHAR(2)),2)||''' ' ABC, e.CLEAGUE_HKJC_NAME,  e.CLEAGUEALIAS_OUTPUT_NAME, e.HKJCHOSTNAME_CN, e.HKJCGUESTNAME_CN, e.CMATCHDATETIME,  a.IMATCHYEAR   ||   right( '0'|| CAST(a.IMATCHMONTH as VARCHAR(2)),2) startdate2,  a.*   from ANALYSIS_HISTORY_INFO  a inner join EMATCHES e on e.EMATCHID =a.IMATCH_CNT   where a.IMATCH_CNT=" + ml.ToString() + " ORDER BY  a.imatch_cnt ,a.irec asc   ";

                                    Files.WriteLog("Sql: " + queryString);

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

                                    connection.Close();
                                }

                                if (ds.Tables[0].Rows.Count > 0)
                                {
                                    using (FbConnection connection2 = new FbConnection(AppFlag.MangoDBConn))
                                    {
                                        connection2.Open();
                                        using (FbCommand cmd2 = new FbCommand())
                                        {
                                            List<string> strL = ds.Tables[0].AsEnumerable().Select(d => d.Field<string>("ABC")).ToList<string>().Distinct().ToList();
                                            string strs = string.Concat("(", string.Join(") OR  (", strL), ")");
                                            cmd2.CommandText = "delete from ANALYSISHISTORYS where " + strs.Replace("OR  ()", "").Replace("() OR", "").Replace("()", "");
                                            cmd2.Connection = connection2;
                                            int id = Convert.ToInt32(cmd2.ExecuteScalar());
                                            Files.WriteLog("Sql: " + "delete from ANALYSISHISTORYS where " + strs.Replace("OR  ()", "").Replace("() OR", "").Replace("()", "") + "  id =" + id);
                                        }

                                        queryString = "select first   1 * from ANALYSISHISTORYS";
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
                                                        foreach (DataRow dr2 in ds.Tables[0].Rows)
                                                        {
                                                            DataRow dr3 = data.Tables["data"].NewRow();
                                                            dr3[0] = dr2["CLEAGUE_HKJC_NAME"];
                                                            dr3[1] = dr2["HKJCHOSTNAME_CN"];
                                                            dr3[2] = dr2["HKJCGUESTNAME_CN"];
                                                            dr3[3] = dr2["startdate2"]; //Convert.ToDateTime(dr2["START_DATE"]).ToString("yyyyMMdd");
                                                            dr3[4] = dr2["CLEAGUEALIAS_OUTPUT_NAME"];
                                                            dr3[5] = dr2["IMATCHSTATUS"].ToString() == "0" ? "主" : "客";
                                                            dr3[6] = dr2["IHOSTSCORE"];
                                                            dr3[7] = dr2["IGUESTSCORE"];
                                                            dr3[8] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                                                            dr3[9] = "0";
                                                            data.Tables["data"].Rows.Add(dr3);
                                                        }
                                                        int count = fda.Update(data.Tables["data"]);
                                                        Files.WriteLog((count > 0 ? "[Success] " : "[Failure] ") + " Insert ANALYSISHISTORYS [" + data.Tables["data"].Rows.Count + "]");
                                                    }
                                                }
                                            }
                                        }
                                        connection2.Close();
                                    }
                                    SendAlertMsg(11);
                                }
                                // SendAlertMsg(11);
                            }
                            catch (Exception exp)
                            {
                                Files.WriteError(DateTime.Now.ToString("HH:mm:ss ") + "Update ANALYSISHISTORYS,error: " + exp);
                            }
                        }
                        else if (mw == 15)
                        {
                            try
                            {
                                using (FbConnection connection = new FbConnection(AppFlag.ScoutsDBConn))
                                {
                                    connection.Open();
                                    this.listBox1.Invoke(new Action(() =>
                                    {
                                        this.listBox1.Invoke(new Action(() => { { this.listBox1.Items.Insert(0, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ") + "Received " + ml + "-Ranks"); } }));
                                    }));

                                    Files.WriteLog("Update Ranks " + ml.ToString());
                                    queryString = "SELECT T.SHORT_NAME  , c.name, a.LEAG_ID, a.CLEAG_ALIAS,l.LEAGUE_CHI_NAME, a.SEASON_ID, a.TEAM_ID, a.TEAM, a.HKJC_TEAM, a.SCORE, a.RANK, a.FLAG, a.GAMES, a.IWON, a.IDRAW,a.ILOST," +
                                             "a.CTIMESTAMP,T.HKJC_NAME_CN FROM LEAGRANKINFO a inner join LEAGUE_INFO l on l.CLEAGUE_ALIAS_NAME = a.CLEAG_ALIAS  inner join SEASONS s on s.id = a.SEASON_ID and ( s.SYEAR = '" + DateTime.Now.ToString("yyyy") + "' or s.SYEAR = '" + DateTime.Now.ToString("yyyy") + "/" + DateTime.Now.AddYears(1).ToString("yy") + "') " +
                                             "inner join teams t on t.id = a.team_id   inner join areas c on c.id = t.area_id where  a.LEAG_ID=" + ml.ToString() + " order by a.CLEAG_ALIAS ,a.rank asc ";

                                    Files.WriteLog("Sql: " + queryString);

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

                                    connection.Close();
                                }

                                if (ds.Tables[0].Rows.Count > 0)
                                {
                                    using (FbConnection connection2 = new FbConnection(AppFlag.MangoDBConn))
                                    {
                                        connection2.Open();
                                        using (FbCommand cmd2 = new FbCommand())
                                        {
                                            cmd2.CommandText = "delete from ranks where CLEAGUENAME='" + ds.Tables[0].Rows[0]["LEAGUE_CHI_NAME"].ToString() + "'";
                                            cmd2.Connection = connection2;
                                            int id = Convert.ToInt32(cmd2.ExecuteScalar());
                                            Files.WriteLog("Sql: " + "delete from ranks where CLEAGUENAME='" + ds.Tables[0].Rows[0]["LEAGUE_CHI_NAME"].ToString() + "'");
                                        }

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
                                                        foreach (DataRow dr2 in ds.Tables[0].Rows)
                                                        {
                                                            DataRow dr3 = data.Tables["data"].NewRow();
                                                            dr3[0] = dr2["LEAGUE_CHI_NAME"];
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
                                                            dr3[12] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                                                            dr3[13] = "0";
                                                            data.Tables["data"].Rows.Add(dr3);
                                                        }
                                                        fda.Update(data.Tables["data"]);
                                                        Files.WriteLog("Insert ranks [" + ml.ToString() + "] " + ds.Tables[0].Rows[0]["LEAGUE_CHI_NAME"].ToString());
                                                    }
                                                }
                                            }
                                        }
                                        connection2.Close();
                                    }

                                    SendAlertMsg(15);
                                }
                            }
                            catch (Exception exp)
                            {
                                Files.WriteError(DateTime.Now.ToString("HH:mm:ss ") + "Update ANALYSISHISTORYS,error: " + exp);
                            }
                        }
                        else if (mw == 17)
                        {
                            try
                            {
                                using (FbConnection connection = new FbConnection(AppFlag.ScoutsDBConn))
                                {
                                    connection.Open();
                                    this.listBox1.Invoke(new Action(() =>
                                    {
                                        this.listBox1.Invoke(new Action(() => { { this.listBox1.Items.Insert(0, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ") + "Received " + ml + "-Scorers"); } }));
                                    }));

                                    Files.WriteLog("Update Scorers " + ml.ToString());
                                    queryString = "SELECT  first 15 distinct  t.SHORT_NAME tname,t.HKJC_NAME_CN tcname ,r.CLEAG_ID, r.CLEAG_ALIAS,l.LEAGUE_CHI_NAME,  r.SEASON_ID,  r.PLAYER_ID,r.CPLAYER_NAME, r.CTEAM_ABBR, r.CACT," +
                                            " r.IRID, r.IRANK, r.IGOALS, r.UT, r.CTIMESTAMP ,t.short_name tname, r.CPLAYER_NAME_cn   pcname,r.CPLAYER_NAME_cn FROM SCORERS_INFO r  inner join LEAGUE_INFO l on l.CLEAGUE_ALIAS_NAME = r.CLEAG_ALIAS   inner join SEASONS s on s.id = r.SEASON_ID and ( s.SYEAR = '" + DateTime.Now.ToString("yyyy") + "' or s.SYEAR = '" + DateTime.Now.ToString("yyyy") + "/" + DateTime.Now.ToString("yy") + "')  " +
                                            " inner join teams t on t.id = r.TEAM_ID" +
                                            //2019118 "  left join  PLAYERS_INFO p  on p.PLAYER_ID = r.PLAYER_ID and p.TEAM_ID = r.TEAM_ID " + //p.CPLAYER_NAME
                                            " where r.CLEAG_ID = " + ml.ToString() + "  order by r.IRANK asc";
                                    Files.WriteLog("Sql: " + queryString);

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

                                    connection.Close();
                                }

                                if (ds.Tables[0].Rows.Count > 0)
                                {
                                    using (FbConnection connection2 = new FbConnection(AppFlag.MangoDBConn))
                                    {
                                        connection2.Open();
                                        using (FbCommand cmd2 = new FbCommand())
                                        {
                                            cmd2.CommandText = "delete from Scorers where CLEAGUENAME='" + ds.Tables[0].Rows[0]["LEAGUE_CHI_NAME"].ToString() + "'";
                                            cmd2.Connection = connection2;
                                            int id = Convert.ToInt32(cmd2.ExecuteScalar());
                                            Files.WriteLog("Sql: " + "delete from Scorers where CLEAGUENAME='" + ds.Tables[0].Rows[0]["LEAGUE_CHI_NAME"].ToString() + "'");
                                        }

                                        queryString = "select first  1 * from Scorers";
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
                                                        foreach (DataRow dr2 in ds.Tables[0].Rows)
                                                        {
                                                            DataRow dr3 = data.Tables["data"].NewRow();
                                                            dr3[0] = dr2["LEAGUE_CHI_NAME"];
                                                            dr3[1] = DateTime.Now.ToString("yyyy/MM/dd");
                                                            dr3[2] = dr2["IRANK"];
                                                            dr3[3] = dr2["CPLAYER_NAME_cn"].Equals(DBNull.Value) || dr2["CPLAYER_NAME_cn"].Equals("") ? (dr2["pcname"].Equals(DBNull.Value) || dr2["pcname"].Equals("") ? dr2["CPLAYER_NAME"] : dr2["pcname"]) : dr2["CPLAYER_NAME_cn"];
                                                            dr3[4] = dr2["tcname"] == DBNull.Value || dr2["tcname"].ToString() == "" ? dr2["tname"] : dr2["tcname"];
                                                            dr3[5] = dr2["IGOALS"];
                                                            dr3[6] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                                                            dr3[7] = "0";
                                                            data.Tables["data"].Rows.Add(dr3);
                                                        }
                                                        fda.Update(data.Tables["data"]);
                                                        Files.WriteLog("Insert Scorers [" + ml.ToString() + "] " + ds.Tables[0].Rows[0]["LEAGUE_CHI_NAME"].ToString());
                                                    }
                                                }
                                            }
                                        }
                                        connection2.Close();
                                    }

                                    SendAlertMsg(17);
                                }
                            }
                            catch (Exception exp)
                            {
                                Files.WriteError(DateTime.Now.ToString("HH:mm:ss ") + "Update Scorers,error: " + exp);
                            }
                        }
                        else if (mw == 62)
                        {
                            try
                            {
                                using (FbConnection connection = new FbConnection(AppFlag.ScoutsDBConn))
                                {
                                    connection.Open();
                                    Files.WriteLog("Update ADDANALYSISOTHER " + ml.ToString());
                                    this.listBox1.Invoke(new Action(() =>
                                    {
                                        this.listBox1.Invoke(new Action(() => { { this.listBox1.Items.Insert(0, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ") + "Received " + ml + "-ANALYSISOTHERS"); } }));
                                    }));

                                    queryString = "SELECT a.EVENTID, a.CLEAGUE aCLEAGUE, l.LEAGUE_CHI_NAME CLEAGUE, a.CTEAM, a.CTEAMTYPE, a.CACTION, a.CSHOTS, a.CFOULS, a.CCORNER_KICKS, a.COFFSIDES, a.CPOSSESSION, a.CYELLOW_CARDS, a.CRED_CARDS, a.CATTACKS, a.CSUBSTITUTIONS, a.CTHROWINS, a.CGOALKICKS, a.CTIMESTAMP FROM ANALYSIS_OTHERS a" +
                                       " inner join  LEAGUE_INFO l on l.CLEAGUE_ALIAS_NAME  =a.CLEAGUE where a.EVENTID = " + ml.ToString() + " order by a.CTIMESTAMP desc ";

                                    Files.WriteLog("Sql: " + queryString);


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

                                    if (ds.Tables[0].Rows.Count == 2)
                                    {
                                        using (FbConnection connection2 = new FbConnection(AppFlag.MangoDBConn))
                                        {
                                            List<DataRow> drH = ds.Tables[0].Select().AsEnumerable().ToList<DataRow>().Where(x => x.Field<string>("CTEAMTYPE") == "H").ToList();
                                            List<DataRow> drG = ds.Tables[0].Select().AsEnumerable().ToList<DataRow>().Where(x => x.Field<string>("CTEAMTYPE") == "G").ToList();

                                            connection2.Open();

                                            using (FbCommand cmd2 = new FbCommand("PR_ADDANALYSISOTHER", connection2))
                                            {
                                                cmd2.CommandType = CommandType.StoredProcedure;
                                                cmd2.Parameters.Add("@CLEAGUE", drH[0]["CLEAGUE"]);
                                                cmd2.Parameters.Add("@CHOST", drH[0]["CTEAM"]);
                                                cmd2.Parameters.Add("@CGUEST", drG[0]["CTEAM"]);
                                                cmd2.Parameters.Add("@CACTION", "U");
                                                cmd2.Parameters.Add("@CSHOTS", drH[0]["CSHOTS"] + "@" + drG[0]["CSHOTS"]);
                                                cmd2.Parameters.Add("@CFOULS", drH[0]["CFOULS"] + "@" + drG[0]["CFOULS"]);
                                                cmd2.Parameters.Add("@CCORNER_KICKS", drH[0]["CCORNER_KICKS"] + "@" + drG[0]["CCORNER_KICKS"]);
                                                cmd2.Parameters.Add("@COFFSIDES", drH[0]["COFFSIDES"] + "@" + drG[0]["COFFSIDES"]);
                                                cmd2.Parameters.Add("@CPOSSESSION", drH[0]["CPOSSESSION"] + "@" + drG[0]["CPOSSESSION"]);
                                                cmd2.Parameters.Add("@CYELLOW_CARDS", drH[0]["CYELLOW_CARDS"] + "@" + drG[0]["CYELLOW_CARDS"]);
                                                cmd2.Parameters.Add("@CRED_CARDS", drH[0]["CRED_CARDS"] + "@" + drG[0]["CRED_CARDS"]);
                                                cmd2.Parameters.Add("@TIMEFLAG", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                                                cmd2.Parameters.Add("@IDEST", "0");
                                                int id = Convert.ToInt32(cmd2.ExecuteScalar());
                                                if (id > -1)
                                                {
                                                    Files.WriteLog(" [Success] Insert ADDANALYSISOTHER " + " " + drH[0]["CLEAGUE"] + " " + drH[0]["CTEAM"] + "/" + drG[0]["CTEAM"]);
                                                }
                                            }

                                            connection2.Close();
                                        }
                                    }
                                    connection.Close();
                                }
                                SendAlertMsg(62);
                            }
                            catch (Exception exp)
                            {
                                Files.WriteError(DateTime.Now.ToString("HH:mm:ss ") + "ANALYSISOTHERS,error: " + exp);
                            }
                        }
                    }
                    else
                    {
                        try
                        {      //COPYDATASTRUCT mystr2 = new COPYDATASTRUCT();
                               //Type mytype2 = mystr.GetType();
                               //mystr2 = (COPYDATASTRUCT)m.GetLParam(mytype2);
                               //string ml2 = mystr.lpData;
                               //// string len = mystr.cbData;
                               //int mw2 = (int)m.WParam;

                            //COPYDATASTRUCT cdata2 = new COPYDATASTRUCT();
                            //Type mytype2 = cdata2.GetType();
                            //cdata2 = (COPYDATASTRUCT)Marshal.PtrToStructure(m.LParam, mytype2);
                            //string ml2 = cdata.lpData;
                            // string ml2 = "";
                            this.listBox1.Invoke(new Action(() =>
                            {
                                this.listBox1.Invoke(new Action(() => { { this.listBox1.Items.Insert(0, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ") + "Recevied msg,Start Sync.."); } }));
                            }));

                            DateTime dt = DateTime.ParseExact(((int)m.WParam).ToString() + ((int)m.LParam).ToString(), "yyyyMMddHHmmssfff", CultureInfo.InvariantCulture);
                            Files.WriteLog(" [Success] recevied " + dt.ToString("yyyy-MM-dd HH:mm:ss.fff"));

                            Files.WriteLog("Update TeamInfo.");
                            string queryString = "";
                            DataSet ds = new DataSet();
                            try
                            {
                                using (FbConnection connection = new FbConnection(AppFlag.ScoutsDBConn))
                                {
                                    connection.Open();
                                    this.listBox1.Invoke(new Action(() =>
                                    {
                                        this.listBox1.Invoke(new Action(() => { { this.listBox1.Items.Insert(0, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ") + "Start sync TeamInfo."); } }));
                                    }));

                                    //queryString = "select e.id eid,  t.id tid, t.SHORT_NAME,t.hkjc_name_cn ,T.AREA_ID ,a.NAME COUNTRY ,c.COUNTRY_CHI_NAME from events e inner join teams t on t.id = e.HOME_ID or t.id = e.guest_id inner join  AREAS a on a.ID = t.AREA_ID LEFT join  INT_COUNTRY c on c.COUNTRY_ENG_NAME = a.NAME  where e.id  in (select  c.EMATCHID FROM EMATCHES c where c.CTIMESTAMP >= '" + dt.ToString("yyyy-MM-dd HH:mm:ss.fff") + "' AND C.EMATCHID IS NOT NULL   AND C.EMATCHID > 0 ORDER BY C.CTIMESTAMP ASC)";
                                    queryString = "select e.START_DATE, e.id eid,  t.id tid, t.SHORT_NAME,t.hkjc_name_cn ,T.AREA_ID ,a.NAME COUNTRY ,c.COUNTRY_CHI_NAME from events e " +
                                        "inner join teams t on t.id = e.HOME_ID or t.id = e.guest_id inner join  AREAS a on a.ID = t.AREA_ID LEFT join  INT_COUNTRY c on c.COUNTRY_ENG_NAME = a.NAME " +
                                        " where e.id  in (select  x.EMATCHID FROM EMATCHES x where  x.CTIMESTAMP >= '2019-05-30 21:03:31.581' AND x.EMATCHID IS NOT NULL   AND x.EMATCHID > 0 and x.CMATCHDATETIME> current_timestamp ORDER BY x.CTIMESTAMP ASC) " +
                                        "order by e.START_DATE asc ";
                                    Files.WriteLog("Sql: " + queryString);

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

                                    if (ds.Tables[0].Rows.Count > 0)
                                    {
                                        using (FbConnection connection2 = new FbConnection(AppFlag.MangoDBConn))
                                        {
                                            connection2.Open();
                                            foreach (DataRow dr in ds.Tables[0].Rows)
                                            {
                                                if (dr["hkjc_name_cn"] is DBNull) continue;
                                                try
                                                {
                                                    using (FbCommand cmd2 = new FbCommand("PR_AddTeam", connection2))
                                                    {
                                                        cmd2.CommandType = CommandType.StoredProcedure;
                                                        cmd2.Parameters.Add("@TEAM_ID", dr["tid"]);
                                                        cmd2.Parameters.Add("@TEAMNAME", dr["hkjc_name_cn"]);
                                                        cmd2.Parameters.Add("@COUNTRY", dr["COUNTRY_CHI_NAME"] is DBNull ? dr["COUNTRY"] : dr["COUNTRY_CHI_NAME"]);
                                                        cmd2.Parameters.Add("@CITY", DBNull.Value);
                                                        cmd2.Parameters.Add("@VENUE", DBNull.Value);
                                                        cmd2.Parameters.Add("@CONTINENT", DBNull.Value);
                                                        cmd2.Parameters.Add("@CENGNAME", dr["SHORT_NAME"]);
                                                        cmd2.Parameters.Add("@CMACAUNAME", dr["hkjc_name_cn"]);
                                                        cmd2.Parameters.Add("@CHKJCNAME", dr["hkjc_name_cn"]);
                                                        cmd2.Parameters.Add("@CHKJCSHORTNAME", dr["hkjc_name_cn"]);
                                                        cmd2.Parameters.Add("@LASTUPDATE", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                                                        cmd2.Parameters.Add("@TIMEFLAG", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                                                        int id = Convert.ToInt32(cmd2.ExecuteScalar());
                                                        if (id > -1)
                                                        {
                                                            Files.WriteLog(" [Success] Insert TEAMINFO " + " " + dr["tid"] + " " + dr["hkjc_name_cn"] + "/" + dr["SHORT_NAME"]);
                                                        }
                                                    }
                                                }
                                                catch (Exception exp)
                                                {
                                                    Files.WriteError(DateTime.Now.ToString("HH:mm:ss ") + "Sync TEAM " + dr["tid"] + " " + dr["hkjc_name_cn"] + "/" + dr["SHORT_NAME"] + "error: " + exp);
                                                    continue;
                                                }
                                            }
                                            connection2.Close();
                                        }
                                    }
                                    connection.Close();
                                }
                            }
                            catch (Exception exp)
                            {
                                Files.WriteError(DateTime.Now.ToString("HH:mm:ss ") + "TeamInfo,error: " + exp);
                            }

                            /* 
                           string[] syncItems;
                           ArrayList configSetting = new ArrayList(); 
                           configSetting = AppFlag.configSetting;
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
                           // configSetting.Clear();
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
                                           this.listBox1.Invoke(new Action(() =>
                                           {
                                               this.listBox1.Invoke(new Action(() => { { this.listBox1.Items.Insert(0, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ") + "Start sync " + syncItems[i] + "."); } }));
                                           }));
                                           if (i == 0)
                                           {
                                               //ANALYSIS_STAT_INFO
                                               queryString = "select  e.EMATCHID eid,   e.CLEAGUE_HKJC_NAME,  e.CLEAGUEALIAS_OUTPUT_NAME, e.HKJCHOSTNAME_CN, e.HKJCGUESTNAME_CN, e.CMATCHDATETIME,  a.*   from ANALYSIS_STAT_INFO  a inner join EMATCHES e on e.EMATCHID =a.IMATCH_CNT   where a.IMATCH_CNT in (select  c.EMATCHID  FROM EMATCHES c where   c.CTIMESTAMP >= '" + dt.ToString("yyyy-MM-dd HH:mm:ss.fff") + "' AND C.EMATCHID IS NOT NULL   AND C.EMATCHID > 0 ORDER BY C.CTIMESTAMP ASC)";
                                           }
                                           else if (i == 1)
                                           {
                                               //ANALYSIS_HISTORY_INFO
                                               //queryString = "select   e.CLEAGUE_OUTPUT_NAME,e.CLEAGUEALIAS_OUTPUT_NAME, e.HKJCHOSTNAME_CN, e.HKJCGUESTNAME_CN, e.CMATCHDATETIME,  a.*   from    ANALYSIS_HISTORY_INFO    a inner join EMATCHES e on e.EMATCHID =a.IMATCH_CNT where   c.CTIMESTAMP >= '" + dt.ToString("yyyy-MM-dd HH:mm:ss.fff") + "' AND C.EMATCHID IS NOT NULL   AND C.EMATCHID > 0 ORDER BY C.CTIMESTAMP ASC";
                                               //queryString = "select   e.EMATCHID eid,  'CLEAGUEALIAS='''||  e.CLEAGUEALIAS_OUTPUT_NAME||''' AND CHOST=''' || e.HKJCHOSTNAME_CN||''' AND CGUEST='''|| e.HKJCGUESTNAME_CN||''' and cdate='''|| replace( cast(a.START_DATE as date),'-','')||''' ' ABC, e.CLEAGUE_HKJC_NAME,  e.CLEAGUEALIAS_OUTPUT_NAME, e.HKJCHOSTNAME_CN, e.HKJCGUESTNAME_CN, e.CMATCHDATETIME,  a.*   from ANALYSIS_HISTORY_INFO  a inner join EMATCHES e on e.EMATCHID =a.IMATCH_CNT   where a.IMATCH_CNT in (select  c.EMATCHID  FROM EMATCHES c where   c.CTIMESTAMP >= '" + dt.ToString("yyyy-MM-dd HH:mm:ss.fff") + "' AND C.EMATCHID IS NOT NULL   AND C.EMATCHID > 0 ORDER BY C.CTIMESTAMP ASC) ORDER BY  a.imatch_cnt ,a.irec asc   ";
                                               queryString = "select   e.EMATCHID eid,  'CLEAGUEALIAS='''||  e.CLEAGUEALIAS_OUTPUT_NAME||''' AND CHOST=''' || e.HKJCHOSTNAME_CN||''' AND CGUEST='''|| e.HKJCGUESTNAME_CN||''' and cdate='''||substring( replace( cast(a.START_DATE as date),'-','') FROM 1 FOR 6)||''' ' ABC, e.CLEAGUE_HKJC_NAME,  e.CLEAGUEALIAS_OUTPUT_NAME, e.HKJCHOSTNAME_CN, e.HKJCGUESTNAME_CN, e.CMATCHDATETIME,  a.*   from ANALYSIS_HISTORY_INFO  a inner join EMATCHES e on e.EMATCHID =a.IMATCH_CNT   where a.IMATCH_CNT in (select  c.EMATCHID  FROM EMATCHES c where   c.CTIMESTAMP >= '" + dt.ToString("yyyy-MM-dd HH:mm:ss.fff") + "' AND C.EMATCHID IS NOT NULL   AND C.EMATCHID > 0 ORDER BY C.CTIMESTAMP ASC) ORDER BY  a.imatch_cnt ,a.irec asc   ";
                                           }
                                           else if (i == 2)
                                           {
                                               //ANALYSIS_RECENT_INFO
                                               // queryString = "select  e.CLEAGUE_OUTPUT_NAME,  e.CLEAGUEALIAS_OUTPUT_NAME, e.HKJCHOSTNAME_CN, e.HKJCGUESTNAME_CN,  e.CMATCHDATETIME, a.*   from    ANALYSIS_RECENT_INFO    a inner join EMATCHES e on e.EMATCHID =a.IMATCH_CNT     a where a.IMATCH_CNT in (select  c.EMATCHID  FROM EMATCHES c where   c.CTIMESTAMP >= '" + dt.ToString("yyyy-MM-dd HH:mm:ss.fff") + "' AND C.EMATCHID IS NOT NULL   AND C.EMATCHID > 0 ORDER BY C.CTIMESTAMP ASC)";
                                               queryString = "select  e.EMATCHID eid,   'CLEAGUE='''||  e.CLEAGUE_HKJC_NAME||''' AND CHOST=''' || e.HKJCHOSTNAME_CN||''' AND CGUEST='''|| e.HKJCGUESTNAME_CN||''' ' ABC, e.CLEAGUE_HKJC_NAME CLEAGUE,  e.CLEAGUEALIAS_OUTPUT_NAME , e.HKJCHOSTNAME_CN CHOST, e.HKJCGUESTNAME_CN CGUEST, e.CMATCHDATETIME,  a.* from ANALYSIS_RECENT_INFO a inner join EMATCHES e on e.EMATCHID = a.IMATCH_CNT   where a.IMATCH_CNT in  (select  c.EMATCHID FROM EMATCHES c where c.CTIMESTAMP >= '" + dt.ToString("yyyy-MM-dd HH:mm:ss.fff") + "' AND C.EMATCHID IS NOT NULL AND C.EMATCHID > 0 ORDER BY C.CTIMESTAMP ASC)  ORDER BY  a.IMATCH_CNT ,a.irec asc";
                                           }
                                           else if (i == 3)
                                           {
                                               //teams 2019-04-09 10:51:04.234
                                               queryString = "select e.id eid,  t.id tid, t.SHORT_NAME,t.hkjc_name_cn ,T.AREA_ID ,a.NAME COUNTRY ,c.COUNTRY_CHI_NAME from events e inner join teams t on t.id = e.HOME_ID or t.id = e.guest_id inner join  AREAS a on a.ID = t.AREA_ID LEFT join  INT_COUNTRY c on c.COUNTRY_ENG_NAME = a.NAME  where e.id  in (select  c.EMATCHID FROM EMATCHES c where c.CTIMESTAMP >= '" + dt.ToString("yyyy-MM-dd HH:mm:ss.fff") + "' AND C.EMATCHID IS NOT NULL   AND C.EMATCHID > 0 ORDER BY C.CTIMESTAMP ASC)";
                                           }
                                           //else if (i == 4)
                                           //{
                                           //    //players  
                                           //    // queryString = "select e.id,c.ALIAS, t.HKJC_NAME,t.HKJC_NAME_CN, p.*  from  PLAYERS_INFO p inner join teams t on t.id = p.TEAM_ID inner join events e on e.id = p.EVENT_ID inner join  COMPETITIONS c on c.id = e.COMPETITION_ID where p.EVENT_ID  in (select  c.EMATCHID FROM EMATCHES c where c.CTIMESTAMP >= '" + dt.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'  AND C.EMATCHID IS NOT NULL   AND C.EMATCHID > 0 ORDER BY C.CTIMESTAMP ASC) and p.IROSTER = 1 order by p.EVENT_ID, p.TEAM_ID  ,p.IPLAYER_NO ";
                                           //    // queryString = "select e.id,c.ALIAS, t.HKJC_NAME,t.HKJC_NAME_CN, p.*  from  PLAYERS_INFO p inner join teams t on t.id = p.TEAM_ID inner join events e on e.id = p.EVENT_ID inner join  COMPETITIONS c on c.id = e.COMPETITION_ID where p.EVENT_ID = 2737951 and p.IROSTER = 1 order by p.EVENT_ID, p.TEAM_ID  ,p.IPLAYER_NO ";
                                           //    queryString = "select e.id eid, e.id,c.ALIAS, t.HKJC_NAME,t.HKJC_NAME_CN, p.*  from  PLAYERS_INFO p inner join teams t on t.id = p.TEAM_ID inner join events e on e.id = p.EVENT_ID inner join  COMPETITIONS c on c.id = e.COMPETITION_ID where p.EVENT_ID  in (select  c.EMATCHID FROM EMATCHES c where c.CTIMESTAMP >= '" + dt.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'  AND C.EMATCHID IS NOT NULL   AND C.EMATCHID > 0 ORDER BY C.CTIMESTAMP ASC) and p.IROSTER = 1 order by p.EVENT_ID, p.TEAM_ID  ,p.IPLAYER_NO ";
                                           //}
                                           else if (i == 5)
                                           {
                                               //FIXTURES  
                                               //queryString = "select e.id,c.ALIAS, t.HKJC_NAME,t.HKJC_NAME_CN, p.*  from  PLAYERS_INFO p inner join teams t on t.id = p.TEAM_ID inner join events e on e.id = p.EVENT_ID inner join  COMPETITIONS c on c.id = e.COMPETITION_ID where p.EVENT_ID  in (select  c.EMATCHID FROM EMATCHES c where c.CTIMESTAMP >= '" + dt.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'  AND C.EMATCHID IS NOT NULL   AND C.EMATCHID > 0 ORDER BY C.CTIMESTAMP ASC) and p.IROSTER = 1 order by p.EVENT_ID, p.TEAM_ID  ,p.IPLAYER_NO ";
                                               queryString = "SELECT 'c.id=''' || r.ID||'''' cid, r.id eid,  L.LEAGUE_CHI_NAME FROM COMPETITIONS r INNER JOIN  LEAGUE_INFO L ON R.ALIAS=L.CLEAGUE_ALIAS_NAME where r.ALIAS = '意甲' OR r.ALIAS = '英超' OR r.ALIAS = '法甲' OR r.ALIAS = '德甲' OR r.ALIAS = '蘇超' OR r.ALIAS = '西甲'OR r.ALIAS = '荷甲' OR r.ALIAS = '日聯' OR r.ALIAS = '澳A' OR r.ALIAS = '歐冠' ORDER BY  R.ID ";
                                           }

                                           Files.WriteLog("Sql: " + queryString);


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

                                           if (ds.Tables[0].Rows.Count > 0)
                                           {
                                               if (i != 5)
                                               {
                                                   string ids = string.Concat("(", string.Join(") OR  (", ds.Tables[0].AsEnumerable().Select(d => d.Field<int>("eid")).ToList<int>().Distinct().ToList().ConvertAll<string>(x => x.ToString())), ")");
                                                   Files.WriteLog("Sync ids: " + ids);
                                               }
                                               using (FbConnection connection2 = new FbConnection(AppFlag.MangoDBConn))
                                               {
                                                   connection2.Open();
                                                   if (i == 0)
                                                   {
                                                       continue;
                                                       DateTime dt2 = DateTime.MinValue;
                                                       foreach (DataRow dr in ds.Tables[0].Rows)
                                                       {
                                                           using (FbCommand cmd2 = new FbCommand("PR_ANALYSIS_STAT", connection2))
                                                           {
                                                               cmd2.CommandType = CommandType.StoredProcedure;
                                                               cmd2.Parameters.Add("@CLEAGUE", dr["CLEAGUE_HKJC_NAME"]);
                                                               cmd2.Parameters.Add("@CHOST", dr["HKJCHOSTNAME_CN"]);
                                                               cmd2.Parameters.Add("@CGUEST", dr["HKJCGUESTNAME_CN"]);
                                                               cmd2.Parameters.Add("@IMATCHDATE", Convert.ToDateTime(dr["CMATCHDATETIME"]).ToString("yyyyMMdd"));
                                                               cmd2.Parameters.Add("@IMATCHTIME", Convert.ToDateTime(dr["CMATCHDATETIME"]).ToString("HHmmss"));
                                                               cmd2.Parameters.Add("@CHANDICAP", "1");
                                                               cmd2.Parameters.Add("@CMATCHFIELD", "H");
                                                               cmd2.Parameters.Add("@IHOSTWIN", dr["IHOSTWIN"]);
                                                               cmd2.Parameters.Add("@IHOSTDRAW", dr["IHOSTDRAW"]);
                                                               cmd2.Parameters.Add("@IHOSTLOSS", dr["IHOSTLOSS"]);
                                                               cmd2.Parameters.Add("@IGUESTWIN", dr["IGUESTWIN"]);
                                                               cmd2.Parameters.Add("@IGUESTDRAW", dr["IGUESTDRAW"]);
                                                               cmd2.Parameters.Add("@IGUESTLOSS", dr["IGUESTLOSS"]);
                                                               cmd2.Parameters.Add("@CREMARK", "");
                                                               // cmd2.Parameters.Add("@TIMEFLAG", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                                                               //cmd2.Parameters.Add("@TIMEFLAG", DateTime.MinValue.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                                                               cmd2.Parameters.Add("@TIMEFLAG", dt2.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                                                               cmd2.Parameters.Add("@IDEST", "0");
                                                               int id = Convert.ToInt32(cmd2.ExecuteScalar());
                                                               dt2 = dt2.AddMilliseconds(1);
                                                               if (id > -1)
                                                               {
                                                                   Files.WriteLog(" [Success] Insert ANALYSIS_STAT " + " " + dr["CLEAGUEALIAS_OUTPUT_NAME"] + " " + dr["HKJCHOSTNAME_CN"] + "/" + dr["HKJCGUESTNAME_CN"]);
                                                               }
                                                           }
                                                       }
                                                   }
                                                   else if (i == 1)
                                                   {
                                                       continue;
                                                       try
                                                       {
                                                           using (FbCommand cmd2 = new FbCommand())
                                                           {
                                                               //List<string> strL= (ds.Tables[0].AsEnumerable().Select(d => d.Field<int>("IMATCH_CNT")).ToList<int>()).ConvertAll<string>(x => x.ToString());
                                                               //string strs = string.Concat("'", string.Join("','", strL), "'");
                                                               //cmd2.CommandText = "delete from ANALYSISHISTORYS where  IMATCH_CNT in (" + strs + ")";
                                                               List<string> strL = ds.Tables[0].AsEnumerable().Select(d => d.Field<string>("ABC")).ToList<string>().Distinct().ToList();
                                                               string strs = string.Concat("(", string.Join(") OR  (", strL), ")");
                                                               cmd2.CommandText = "delete from ANALYSISHISTORYS where " + strs.Replace("OR  ()", "").Replace("() OR", "").Replace("()", "");
                                                               cmd2.Connection = connection2;
                                                               int id = Convert.ToInt32(cmd2.ExecuteScalar());
                                                               Files.WriteLog("delete from ANALYSISHISTORYS where " + strs.Replace("OR  ()", "").Replace("() OR", "").Replace("()", "") + "  id=" + id);
                                                           }

                                                           queryString = "select first   1 * from ANALYSISHISTORYS";
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
                                                                           //DateTime dt2 = DateTime.Now;
                                                                           DateTime dt2 = DateTime.MinValue;
                                                                           foreach (DataRow dr2 in ds.Tables[0].Rows)
                                                                           {
                                                                               //r.CLEAGUE, r.CHOST, r.CGUEST, r.CDATE, r.CLEAGUEALIAS, r.CDES,
                                                                               //  r.IHOSTSCORE, r.IGUESTSCORE, r.TIMEFLAG, r.IDEST
                                                                               DataRow[] drs = data.Tables["data"].Select("CLEAGUE='" + dr2["CLEAGUE_HKJC_NAME"] + "' AND CHOST='" + dr2["HKJCHOSTNAME_CN"] + "' AND CGUEST='" + dr2["HKJCGUESTNAME_CN"] + "' and cdate='" + (dr2["START_DATE"] is DBNull ? dr2["IMATCHYEAR"].ToString() + dr2["IMATCHMONTH"].ToString().PadLeft(2, '0') : Convert.ToDateTime(dr2["START_DATE"]).ToString("yyyyMM")) + "'");
                                                                               DataRow[] drs2 = data.Tables["data"].Select("CLEAGUE='" + dr2["CLEAGUE_HKJC_NAME"] + "' AND CHOST='" + dr2["HKJCHOSTNAME_CN"] + "' AND CGUEST='" + dr2["HKJCGUESTNAME_CN"] + "'");

                                                                               if (drs.Length > 0 || drs2.Length > 4) continue;
                                                                               DataRow dr3 = data.Tables["data"].NewRow();
                                                                               dr3[0] = dr2["CLEAGUE_HKJC_NAME"];
                                                                               dr3[1] = dr2["HKJCHOSTNAME_CN"];
                                                                               dr3[2] = dr2["HKJCGUESTNAME_CN"];
                                                                               dr3[3] = dr2["START_DATE"] is DBNull ? dr2["IMATCHYEAR"].ToString() + dr2["IMATCHMONTH"].ToString().PadLeft(2, '0') : Convert.ToDateTime(dr2["START_DATE"]).ToString("yyyyMM");
                                                                               dr3[4] = dr2["CLEAGUEALIAS_OUTPUT_NAME"];
                                                                               dr3[5] = dr2["IMATCHSTATUS"].ToString() == "0" ? "主" : "客";
                                                                               dr3[6] = dr2["IHOSTSCORE"];
                                                                               dr3[7] = dr2["IGUESTSCORE"];
                                                                               dr3[8] = dt2.ToString("yyyy-MM-dd HH:mm:ss.fff");
                                                                               dr3[9] = "0";
                                                                               data.Tables["data"].Rows.Add(dr3);
                                                                               dt2 = dt2.AddMilliseconds(1);
                                                                               Files.WriteLog("Add ANALYSISHISTORYS " + dr3[0] + " " + dr3[1] + "/" + dr3[2]);
                                                                           }
                                                                           fda.Update(data.Tables["data"]);
                                                                           Files.WriteLog(" Insert ANALYSISHISTORYS [" + data.Tables["data"].Rows.Count + "]");

                                                                           //int count = fda.Update(data.Tables["data"]);
                                                                           //Files.WriteLog((count > 0 ? "[Success] " : "[Failure] ") + " Insert ANALYSISHISTORYS [" + data.Tables["data"].Rows.Count + "]");
                                                                       }
                                                                   }
                                                               }
                                                           }
                                                       }
                                                       catch (Exception exp)
                                                       {
                                                           Files.WriteError(DateTime.Now.ToString("HH:mm:ss ") + "Sync ANALYSISHISTORYS,error3: " + exp);
                                                       }
                                                   }
                                                   else if (i == 2)
                                                   {
                                                       continue;
                                                       try
                                                       {
                                                           using (FbCommand cmd2 = new FbCommand())
                                                           {
                                                               List<string> strL = ds.Tables[0].AsEnumerable().Select(d => d.Field<string>("ABC")).ToList<string>().Distinct().ToList();
                                                               string strs = string.Concat("(", string.Join(") OR  (", strL), ")");
                                                               cmd2.CommandText = "delete from ANALYSISRECENTS where " + strs.Replace("OR  ()", "").Replace("() OR", "").Replace("()", "");
                                                               cmd2.Connection = connection2;
                                                               int id = Convert.ToInt32(cmd2.ExecuteScalar());
                                                               Files.WriteLog("delete from ANALYSISRECENTS where " + strs.Replace("OR  ()", "").Replace("() OR", "").Replace("()", "") + "  id=" + id);
                                                           }

                                                           //List<string> strTeam = ds.Tables[0].AsEnumerable().Select(d => d.Field<string>("ABC")).ToList<string>().Distinct().ToList();
                                                           // Files.WriteLog("Insert ANALYSISRECENTS[" + strTeam.Count () + "]");
                                                           // foreach (string s in strTeam)
                                                           // {
                                                           //     Files.WriteLog("1");
                                                           //     DataRow[] drs = ds.Tables[0].Select(s);
                                                           //     List<DataRow> drH = (drs.AsEnumerable().ToList<DataRow>().Where(x => x.Field<string>("CTEAMFLAG") == "H").ToList()).ToList<DataRow>().Where(x => x.Field<int>("IHOSTSCORE") != -1).ToList();
                                                           //     List<DataRow> drG = (drs.AsEnumerable().ToList<DataRow>().Where(x => x.Field<string>("CTEAMFLAG") == "G").ToList()).ToList<DataRow>().Where(x => x.Field<int>("IHOSTSCORE") != -1).ToList();
                                                           //     List<DataRow> drHN = (drs.AsEnumerable().ToList<DataRow>().Where(x => x.Field<string>("CTEAMFLAG") == "H").ToList()).ToList<DataRow>().Where(x => x.Field<int>("IHOSTSCORE") == -1).ToList();
                                                           //     List<DataRow> drGN = (drs.AsEnumerable().ToList<DataRow>().Where(x => x.Field<string>("CTEAMFLAG") == "G").ToList()).ToList<DataRow>().Where(x => x.Field<int>("IHOSTSCORE") == -1).ToList();
                                                           //     Files.WriteLog("2");
                                                           //     using (FbCommand cmd2 = new FbCommand("PR_AddANALYSISRECENT"))
                                                           //     {
                                                           //         Files.WriteLog("3");
                                                           //         cmd2.CommandType = CommandType.StoredProcedure;
                                                           //         cmd2.Connection = connection2;
                                                           //         cmd2.Parameters.Add("@CLEAGUE", drH.Count > 0 ? drH[0]["CLEAGUE"] : drG.Count > 0 ? drG[0][2] : "");
                                                           //         cmd2.Parameters.Add("@CHOST", drH.Count > 0 ? drH[0]["CHOST"] : drG.Count > 0 ? drG[0][3] : "");
                                                           //         cmd2.Parameters.Add("@CGUEST", drH.Count > 0 ? drH[0]["CGUEST"] : drG.Count > 0 ? drG[0][4] : "");
                                                           //         cmd2.Parameters.Add("@CACTION", "U");
                                                           //         cmd2.Parameters.Add("@CH_NEXTMATCH", drHN.Count > 0 ? drHN[0]["LEAGUEALIAS"].ToString() + "/" + drHN[0]["CCHALLENGER"].ToString() + "/" + (drHN[0]["IMATCHSTATUS"].ToString() == "0" ? "主" : "客") : "-1/-1/-1");
                                                           //         cmd2.Parameters.Add("@CH_MATCH1", drH.Count > 0 ? drH[0]["LEAGUEALIAS"].ToString() + "/" + drH[0]["CCHALLENGER"].ToString() + "/" + (drH[0]["IMATCHSTATUS"].ToString() == "0" ? "主" : "客") + "/" + drH[0]["IHOSTSCORE"].ToString() + "/" + drH[0]["IGUESTSCORE"].ToString() : "-1");
                                                           //         cmd2.Parameters.Add("@CH_MATCH2", drH.Count > 1 ? drH[0]["LEAGUEALIAS"].ToString() + "/" + drH[1]["CCHALLENGER"].ToString() + "/" + (drH[1]["IMATCHSTATUS"].ToString() == "0" ? "主" : "客") + "/" + drH[1]["IHOSTSCORE"].ToString() + "/" + drH[1]["IGUESTSCORE"].ToString() : "-1");
                                                           //         cmd2.Parameters.Add("@CH_MATCH3", drH.Count > 2 ? drH[0]["LEAGUEALIAS"].ToString() + "/" + drH[2]["CCHALLENGER"].ToString() + "/" + (drH[2]["IMATCHSTATUS"].ToString() == "0" ? "主" : "客") + "/" + drH[2]["IHOSTSCORE"].ToString() + "/" + drH[2]["IGUESTSCORE"].ToString() : "-1");
                                                           //         cmd2.Parameters.Add("@CH_MATCH4", drH.Count > 3 ? drH[0]["LEAGUEALIAS"].ToString() + "/" + drH[3]["CCHALLENGER"].ToString() + "/" + (drH[3]["IMATCHSTATUS"].ToString() == "0" ? "主" : "客") + "/" + drH[3]["IHOSTSCORE"].ToString() + "/" + drH[3]["IGUESTSCORE"].ToString() : "-1");
                                                           //         cmd2.Parameters.Add("@CH_MATCH5", drH.Count > 4 ? drH[0]["LEAGUEALIAS"].ToString() + "/" + drH[4]["CCHALLENGER"].ToString() + "/" + (drH[4]["IMATCHSTATUS"].ToString() == "0" ? "主" : "客") + "/" + drH[4]["IHOSTSCORE"].ToString() + "/" + drH[4]["IGUESTSCORE"].ToString() : "-1");
                                                           //         cmd2.Parameters.Add("@CG_NEXTMATCH", drGN.Count > 0 ? drGN[0]["LEAGUEALIAS"].ToString() + "/" + drGN[0]["CCHALLENGER"].ToString() + "/" + (drGN[0]["IMATCHSTATUS"].ToString() == "0" ? "主" : "客") : "-1/-1/-1");
                                                           //         cmd2.Parameters.Add("@CG_MATCH1", drG.Count > 0 ? drG[0]["LEAGUEALIAS"].ToString() + "/" + drG[0]["CCHALLENGER"].ToString() + "/" + (drG[0]["IMATCHSTATUS"].ToString() == "0" ? "主" : "客") + "/" + drG[0]["IHOSTSCORE"].ToString() + "/" + drG[0]["IGUESTSCORE"].ToString() : "-1");
                                                           //         cmd2.Parameters.Add("@CG_MATCH2", drG.Count > 1 ? drG[0]["LEAGUEALIAS"].ToString() + "/" + drG[1]["CCHALLENGER"].ToString() + "/" + (drG[1]["IMATCHSTATUS"].ToString() == "0" ? "主" : "客") + "/" + drG[1]["IHOSTSCORE"].ToString() + "/" + drG[1]["IGUESTSCORE"].ToString() : "-1");
                                                           //         cmd2.Parameters.Add("@CG_MATCH3", drG.Count > 2 ? drG[0]["LEAGUEALIAS"].ToString() + "/" + drG[2]["CCHALLENGER"].ToString() + "/" + (drG[2]["IMATCHSTATUS"].ToString() == "0" ? "主" : "客") + "/" + drG[2]["IHOSTSCORE"].ToString() + "/" + drG[2]["IGUESTSCORE"].ToString() : "-1");
                                                           //         cmd2.Parameters.Add("@CG_MATCH4", drG.Count > 3 ? drG[0]["LEAGUEALIAS"].ToString() + "/" + drG[3]["CCHALLENGER"].ToString() + "/" + (drG[3]["IMATCHSTATUS"].ToString() == "0" ? "主" : "客") + "/" + drG[3]["IHOSTSCORE"].ToString() + "/" + drG[3]["IGUESTSCORE"].ToString() : "-1");
                                                           //         cmd2.Parameters.Add("@CG_MATCH5", drG.Count > 4 ? drG[0]["LEAGUEALIAS"].ToString() + "/" + drG[4]["CCHALLENGER"].ToString() + "/" + (drG[4]["IMATCHSTATUS"].ToString() == "0" ? "主" : "客") + "/" + drG[4]["IHOSTSCORE"].ToString() + "/" + drG[4]["IGUESTSCORE"].ToString() : "-1");
                                                           //         cmd2.Parameters.Add("@TIMEFLAG", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                                                           //         cmd2.Parameters.Add("@IDEST", "0");
                                                           //         Files.WriteLog("4");
                                                           //         int id = Convert.ToInt32(cmd2.ExecuteScalar());
                                                           //         Files.WriteLog("5");
                                                           //         if (id > -1)
                                                           //         {
                                                           //             Files.WriteLog("6");
                                                           //             Files.WriteLog(" [Success] Insert ANALYSISRECENTS " + s);
                                                           //         }
                                                           //         Files.WriteLog("7");
                                                           //     }
                                                           // }

                                                           queryString = "select first   1 * from ANALYSISRECENTS";
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
                                                                           List<string> strTeam = ds.Tables[0].AsEnumerable().Select(d => d.Field<string>("ABC")).ToList<string>().Distinct().ToList();
                                                                           DateTime dt2 = DateTime.MinValue;
                                                                           foreach (string s in strTeam)
                                                                           {
                                                                               DataRow[] drs = ds.Tables[0].Select(s);
                                                                               List<DataRow> drH = (drs.AsEnumerable().ToList<DataRow>().Where(x => x.Field<string>("CTEAMFLAG") == "H").ToList()).ToList<DataRow>().Where(x => x.Field<int>("IHOSTSCORE") != -1).ToList();
                                                                               List<DataRow> drG = (drs.AsEnumerable().ToList<DataRow>().Where(x => x.Field<string>("CTEAMFLAG") == "G").ToList()).ToList<DataRow>().Where(x => x.Field<int>("IHOSTSCORE") != -1).ToList();
                                                                               List<DataRow> drHN = (drs.AsEnumerable().ToList<DataRow>().Where(x => x.Field<string>("CTEAMFLAG") == "H").ToList()).ToList<DataRow>().Where(x => x.Field<int>("IHOSTSCORE") == -1).ToList();
                                                                               List<DataRow> drGN = (drs.AsEnumerable().ToList<DataRow>().Where(x => x.Field<string>("CTEAMFLAG") == "G").ToList()).ToList<DataRow>().Where(x => x.Field<int>("IHOSTSCORE") == -1).ToList();

                                                                               DataRow dr3 = data.Tables["data"].NewRow();
                                                                               dr3[0] = drH.Count > 0 ? drH[0]["CLEAGUE"] : drG.Count > 0 ? drG[0][2] : "";
                                                                               dr3[1] = drH.Count > 0 ? drH[0]["CHOST"] : drG.Count > 0 ? drG[0][3] : "";
                                                                               dr3[2] = drH.Count > 0 ? drH[0]["CGUEST"] : drG.Count > 0 ? drG[0][4] : "";
                                                                               dr3[3] = "U";
                                                                               dr3[4] = drHN.Count > 0 ? drHN[0]["LEAGUEALIAS"].ToString() + "/" + drHN[0]["CCHALLENGER"].ToString() + "/" + (drHN[0]["IMATCHSTATUS"].ToString() == "0" ? "主" : "客") : "-1/-1/-1";
                                                                               dr3[5] = drH.Count > 0 ? drH[0]["LEAGUEALIAS"].ToString() + "/" + drH[0]["CCHALLENGER"].ToString() + "/" + (drH[0]["IMATCHSTATUS"].ToString() == "0" ? "主" : "客") + "/" + drH[0]["IHOSTSCORE"].ToString() + "/" + drH[0]["IGUESTSCORE"].ToString() : "-1";
                                                                               dr3[6] = drH.Count > 1 ? drH[0]["LEAGUEALIAS"].ToString() + "/" + drH[1]["CCHALLENGER"].ToString() + "/" + (drH[1]["IMATCHSTATUS"].ToString() == "0" ? "主" : "客") + "/" + drH[1]["IHOSTSCORE"].ToString() + "/" + drH[1]["IGUESTSCORE"].ToString() : "-1";
                                                                               dr3[7] = drH.Count > 2 ? drH[0]["LEAGUEALIAS"].ToString() + "/" + drH[2]["CCHALLENGER"].ToString() + "/" + (drH[2]["IMATCHSTATUS"].ToString() == "0" ? "主" : "客") + "/" + drH[2]["IHOSTSCORE"].ToString() + "/" + drH[2]["IGUESTSCORE"].ToString() : "-1";
                                                                               dr3[8] = drH.Count > 3 ? drH[0]["LEAGUEALIAS"].ToString() + "/" + drH[3]["CCHALLENGER"].ToString() + "/" + (drH[3]["IMATCHSTATUS"].ToString() == "0" ? "主" : "客") + "/" + drH[3]["IHOSTSCORE"].ToString() + "/" + drH[3]["IGUESTSCORE"].ToString() : "-1";
                                                                               dr3[9] = drH.Count > 4 ? drH[0]["LEAGUEALIAS"].ToString() + "/" + drH[4]["CCHALLENGER"].ToString() + "/" + (drH[4]["IMATCHSTATUS"].ToString() == "0" ? "主" : "客") + "/" + drH[4]["IHOSTSCORE"].ToString() + "/" + drH[4]["IGUESTSCORE"].ToString() : "-1";
                                                                               dr3[10] = drGN.Count > 0 ? drGN[0]["LEAGUEALIAS"].ToString() + "/" + drGN[0]["CCHALLENGER"].ToString() + "/" + (drGN[0]["IMATCHSTATUS"].ToString() == "0" ? "主" : "客") : "-1/-1/-1";
                                                                               dr3[11] = drG.Count > 0 ? drG[0]["LEAGUEALIAS"].ToString() + "/" + drG[0]["CCHALLENGER"].ToString() + "/" + (drG[0]["IMATCHSTATUS"].ToString() == "0" ? "主" : "客") + "/" + drG[0]["IHOSTSCORE"].ToString() + "/" + drG[0]["IGUESTSCORE"].ToString() : "-1";
                                                                               dr3[12] = drG.Count > 1 ? drG[0]["LEAGUEALIAS"].ToString() + "/" + drG[1]["CCHALLENGER"].ToString() + "/" + (drG[1]["IMATCHSTATUS"].ToString() == "0" ? "主" : "客") + "/" + drG[1]["IHOSTSCORE"].ToString() + "/" + drG[1]["IGUESTSCORE"].ToString() : "-1";
                                                                               dr3[13] = drG.Count > 2 ? drG[0]["LEAGUEALIAS"].ToString() + "/" + drG[2]["CCHALLENGER"].ToString() + "/" + (drG[2]["IMATCHSTATUS"].ToString() == "0" ? "主" : "客") + "/" + drG[2]["IHOSTSCORE"].ToString() + "/" + drG[2]["IGUESTSCORE"].ToString() : "-1";
                                                                               dr3[14] = drG.Count > 3 ? drG[0]["LEAGUEALIAS"].ToString() + "/" + drG[3]["CCHALLENGER"].ToString() + "/" + (drG[3]["IMATCHSTATUS"].ToString() == "0" ? "主" : "客") + "/" + drG[3]["IHOSTSCORE"].ToString() + "/" + drG[3]["IGUESTSCORE"].ToString() : "-1";
                                                                               dr3[15] = drG.Count > 4 ? drG[0]["LEAGUEALIAS"].ToString() + "/" + drG[4]["CCHALLENGER"].ToString() + "/" + (drG[4]["IMATCHSTATUS"].ToString() == "0" ? "主" : "客") + "/" + drG[4]["IHOSTSCORE"].ToString() + "/" + drG[4]["IGUESTSCORE"].ToString() : "-1";
                                                                               // dr3[16] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                                                                               // dr3[16] = DateTime.MinValue.ToString("yyyy-MM-dd HH:mm:ss.fff");
                                                                               dr3[16] = dt2.ToString("yyyy-MM-dd HH:mm:ss.fff");
                                                                               dr3[17] = "0";
                                                                               data.Tables["data"].Rows.Add(dr3);
                                                                               dt2 = dt2.AddMilliseconds(1);
                                                                               Files.WriteLog("Add ANALYSISRECENTS " + dr3["CLEAGUE"] + " " + dr3["CHOST"] + "/" + dr3["CGUEST"]);
                                                                           }

                                                                           int count = fda.Update(data.Tables["data"]);
                                                                           Files.WriteLog((count > 0 ? "[Success] " : "[Failure] ") + " Insert ANALYSISRECENTS [" + data.Tables["data"].Rows.Count + "]");
                                                                       }
                                                                       //using (DataSet data = new DataSet())
                                                                       //{
                                                                       //    data.Tables.Add(new DataTable("data"));
                                                                       //    fda.Fill(data.Tables["data"]);
                                                                       //    List<string> strTeam = ds.Tables[0].AsEnumerable().Select(d => d.Field<string>("ABC")).ToList<string>().Distinct().ToList();

                                                                       //    foreach (string s in strTeam)
                                                                       //    {
                                                                       //        DataRow[] drs = ds.Tables[0].Select(s);
                                                                       //        List<DataRow> drH = drs.AsEnumerable().ToList<DataRow>().Where(x => x.Field<string>("CTEAMFLAG") == "H").ToList();
                                                                       //        List<DataRow> drG = drs.AsEnumerable().ToList<DataRow>().Where(x => x.Field<string>("CTEAMFLAG") == "G").ToList();
                                                                       //        DataRow dr3 = data.Tables["data"].NewRow();
                                                                       //        dr3[0] = drH.Count > 0 ? drH[0]["CLEAGUE_HKJC_NAME"] : drG.Count > 0 ? drG[0][2] : "";
                                                                       //        dr3[1] = drH.Count > 0 ? drH[0]["CHOST"] : drG.Count > 0 ? drG[0][3] : "";
                                                                       //        dr3[2] = drH.Count > 0 ? drH[0]["CGUEST"] : drG.Count > 0 ? drG[0][4] : "";
                                                                       //        dr3[3] = "U";
                                                                       //        dr3[4] = "-1/-1/-1";
                                                                       //        dr3[5] = drH.Count > 0 ? drH[0]["LEAGUEALIAS"].ToString() + "/" + drH[0]["CCHALLENGER"].ToString() + "/" + (drH[0]["IMATCHSTATUS"].ToString() == "0" ? "主" : "客") + "/" + drH[0]["IHOSTSCORE"].ToString() + "/" + drH[0]["IGUESTSCORE"].ToString() : "-1";
                                                                       //        dr3[6] = drH.Count > 1 ? drH[0]["LEAGUEALIAS"].ToString() + "/" + drH[1]["CCHALLENGER"].ToString() + "/" + (drH[1]["IMATCHSTATUS"].ToString() == "0" ? "主" : "客") + "/" + drH[1]["IHOSTSCORE"].ToString() + "/" + drH[1]["IGUESTSCORE"].ToString() : "-1";
                                                                       //        dr3[7] = drH.Count > 2 ? drH[0]["LEAGUEALIAS"].ToString() + "/" + drH[2]["CCHALLENGER"].ToString() + "/" + (drH[2]["IMATCHSTATUS"].ToString() == "0" ? "主" : "客") + "/" + drH[2]["IHOSTSCORE"].ToString() + "/" + drH[2]["IGUESTSCORE"].ToString() : "-1";
                                                                       //        dr3[8] = drH.Count > 3 ? drH[0]["LEAGUEALIAS"].ToString() + "/" + drH[3]["CCHALLENGER"].ToString() + "/" + (drH[3]["IMATCHSTATUS"].ToString() == "0" ? "主" : "客") + "/" + drH[3]["IHOSTSCORE"].ToString() + "/" + drH[3]["IGUESTSCORE"].ToString() : "-1";
                                                                       //        dr3[9] = drH.Count > 4 ? drH[0]["LEAGUEALIAS"].ToString() + "/" + drH[4]["CCHALLENGER"].ToString() + "/" + (drH[4]["IMATCHSTATUS"].ToString() == "0" ? "主" : "客") + "/" + drH[4]["IHOSTSCORE"].ToString() + "/" + drH[4]["IGUESTSCORE"].ToString() : "-1";
                                                                       //        dr3[10] = "-1/-1/-1";
                                                                       //        dr3[11] = drG.Count > 0 ? drG[0]["LEAGUEALIAS"].ToString() + "/" + drG[0]["CCHALLENGER"].ToString() + "/" + (drG[0]["IMATCHSTATUS"].ToString() == "0" ? "主" : "客") + "/" + drG[0]["IHOSTSCORE"].ToString() + "/" + drG[0]["IGUESTSCORE"].ToString() : "-1";
                                                                       //        dr3[12] = drG.Count > 1 ? drG[0]["LEAGUEALIAS"].ToString() + "/" + drG[1]["CCHALLENGER"].ToString() + "/" + (drG[1]["IMATCHSTATUS"].ToString() == "0" ? "主" : "客") + "/" + drG[1]["IHOSTSCORE"].ToString() + "/" + drG[1]["IGUESTSCORE"].ToString() : "-1";
                                                                       //        dr3[13] = drG.Count > 2 ? drG[0]["LEAGUEALIAS"].ToString() + "/" + drG[2]["CCHALLENGER"].ToString() + "/" + (drG[2]["IMATCHSTATUS"].ToString() == "0" ? "主" : "客") + "/" + drG[2]["IHOSTSCORE"].ToString() + "/" + drG[2]["IGUESTSCORE"].ToString() : "-1";
                                                                       //        dr3[14] = drG.Count > 3 ? drG[0]["LEAGUEALIAS"].ToString() + "/" + drG[3]["CCHALLENGER"].ToString() + "/" + (drG[3]["IMATCHSTATUS"].ToString() == "0" ? "主" : "客") + "/" + drG[3]["IHOSTSCORE"].ToString() + "/" + drG[3]["IGUESTSCORE"].ToString() : "-1";
                                                                       //        dr3[15] = drG.Count > 4 ? drG[0]["LEAGUEALIAS"].ToString() + "/" + drG[4]["CCHALLENGER"].ToString() + "/" + (drG[4]["IMATCHSTATUS"].ToString() == "0" ? "主" : "客") + "/" + drG[4]["IHOSTSCORE"].ToString() + "/" + drG[4]["IGUESTSCORE"].ToString() : "-1";
                                                                       //        dr3[16] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                                                                       //        dr3[17] = "0";
                                                                       //        data.Tables["data"].Rows.Add(dr3);
                                                                       //    }

                                                                       //    int count = fda.Update(data.Tables["data"]);
                                                                       //    Files.WriteLog((count > 0 ? "[Success] " : "[Failure] ") + " Insert ANALYSISRECENTS [" + data.Tables["data"].Rows.Count + "]");
                                                                       //}
                                                                   }
                                                               }
                                                           }
                                                       }
                                                       catch (Exception exp)
                                                       {
                                                           Files.WriteError(" Insert ANALYSISRECENTS [" + ds.Tables[0].Rows.Count + "], error:" + exp.Message);
                                                       }
                                                   }
                                                   else if (i == 3)
                                                   {
                                                       try
                                                       {
                                                           foreach (DataRow dr in ds.Tables[0].Rows)
                                                           {
                                                               using (FbCommand cmd2 = new FbCommand("PR_AddTeam", connection2))
                                                               {
                                                                   cmd2.CommandType = CommandType.StoredProcedure;
                                                                   cmd2.Parameters.Add("@TEAM_ID", dr["tid"]);
                                                                   cmd2.Parameters.Add("@TEAMNAME", dr["hkjc_name_cn"]);
                                                                   cmd2.Parameters.Add("@COUNTRY", dr["COUNTRY_CHI_NAME"] is DBNull ? dr["COUNTRY"] : dr["COUNTRY_CHI_NAME"]);
                                                                   cmd2.Parameters.Add("@CITY", DBNull.Value);
                                                                   cmd2.Parameters.Add("@VENUE", DBNull.Value);
                                                                   cmd2.Parameters.Add("@CONTINENT", DBNull.Value);
                                                                   cmd2.Parameters.Add("@CENGNAME", dr["SHORT_NAME"]);
                                                                   cmd2.Parameters.Add("@CMACAUNAME", dr["hkjc_name_cn"]);
                                                                   cmd2.Parameters.Add("@CHKJCNAME", dr["hkjc_name_cn"]);
                                                                   cmd2.Parameters.Add("@CHKJCSHORTNAME", dr["hkjc_name_cn"]);
                                                                   cmd2.Parameters.Add("@LASTUPDATE", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                                                                   cmd2.Parameters.Add("@TIMEFLAG", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                                                                   int id = Convert.ToInt32(cmd2.ExecuteScalar());
                                                                   if (id > -1)
                                                                   {
                                                                       Files.WriteLog(" [Success] Insert TEAMINFO " + " " + dr["tid"] + " " + dr["hkjc_name_cn"] + "/" + dr["SHORT_NAME"]);
                                                                   }
                                                               }
                                                           }
                                                       }
                                                       catch (Exception exp)
                                                       {
                                                           Files.WriteError(DateTime.Now.ToString("HH:mm:ss ") + "Sync TEAM,error: " + exp);
                                                       }
                                                   }
                                                   else if (i == 4)
                                                   {
                                                       continue;
                                                       List<int> strE = ds.Tables[0].AsEnumerable().Select(d => d.Field<int>("id")).ToList<int>().Distinct().ToList<int>();
                                                       Files.WriteLog(strE.Count.ToString());
                                                       foreach (int s in strE)
                                                       {
                                                           Files.WriteLog(strE.Count.ToString() + " " + s);
                                                           DataRow[] drs = ds.Tables[0].Select("id=" + s);
                                                           List<int> strT = drs.Select(d => d.Field<int>("TEAM_ID")).ToList<int>().Distinct().ToList<int>();
                                                           if (strT.Count == 2)
                                                           {
                                                               //  Files.WriteLog(strT.Count.ToString() + " " + "t");
                                                               List<DataRow> drH = drs.AsEnumerable().ToList<DataRow>().Where(x => x.Field<int>("TEAM_ID") == strT[0]).ToList();
                                                               List<DataRow> drG = drs.AsEnumerable().ToList<DataRow>().Where(x => x.Field<int>("TEAM_ID") == strT[1]).ToList();

                                                               List<DataRow> sGH = drH.Where(x => x.Field<int>("IPOS") == 3).ToList();
                                                               List<DataRow> sGG = drG.Where(x => x.Field<int>("IPOS") == 3).ToList();
                                                               List<DataRow> sBH = drH.Where(x => x.Field<int>("IPOS") == 0).ToList();
                                                               List<DataRow> sBG = drG.Where(x => x.Field<int>("IPOS") == 0).ToList();
                                                               List<DataRow> sFH = drH.Where(x => x.Field<int>("IPOS") == 2).ToList();
                                                               List<DataRow> sFG = drG.Where(x => x.Field<int>("IPOS") == 2).ToList();
                                                               List<DataRow> sMH = drH.Where(x => x.Field<int>("IPOS") == 1).ToList();
                                                               List<DataRow> sMG = drG.Where(x => x.Field<int>("IPOS") == 1).ToList();
                                                               List<DataRow> sUH = drH.Where(x => x.Field<int>("IPOS") == 4).ToList();
                                                               List<DataRow> sUG = drG.Where(x => x.Field<int>("IPOS") == 4).ToList();

                                                               string sH = "";
                                                               string sG = "";
                                                               string ssBH = "", ssBG = "", ssFH = "", ssFG = "", ssMH = "", ssMG = "", ssUH = "", ssUG = "";
                                                               for (int j = 0; j < 5; j++)
                                                               {
                                                                   if (j == 0)
                                                                   {
                                                                       foreach (DataRow R in sBH)
                                                                       {
                                                                           ssBH += "B/" + R["IPLAYER_NO"].ToString() + "/" + (R["CPLAYER_NAME"].ToString() == "" ? R["CENGNAME"].ToString() : R["CPLAYER_NAME"].ToString()) + ",";
                                                                       }
                                                                       foreach (DataRow R in sBG)
                                                                       {
                                                                           ssBG += "B/" + R["IPLAYER_NO"].ToString() + "/" + (R["CPLAYER_NAME"].ToString() == "" ? R["CENGNAME"] : R["CPLAYER_NAME"].ToString()) + ",";
                                                                       }

                                                                   }
                                                                   if (j == 1)
                                                                   {
                                                                       foreach (DataRow R in sMH)
                                                                       {
                                                                           ssMH += "M/" + R["IPLAYER_NO"].ToString() + "/" + (R["CPLAYER_NAME"].ToString() == "" ? R["CENGNAME"] : R["CPLAYER_NAME"].ToString()) + ",";
                                                                       }
                                                                       foreach (DataRow R in sMG)
                                                                       {
                                                                           ssMG += "M/" + R["IPLAYER_NO"].ToString() + "/" + (R["CPLAYER_NAME"].ToString() == "" ? R["CENGNAME"] : R["CPLAYER_NAME"].ToString()) + ",";
                                                                       }
                                                                   }
                                                                   if (j == 2)
                                                                   {
                                                                       foreach (DataRow R in sFH)
                                                                       {
                                                                           ssFH += "F/" + R["IPLAYER_NO"].ToString() + "/" + (R["CPLAYER_NAME"].ToString() == "" ? R["CENGNAME"] : R["CPLAYER_NAME"].ToString()) + ",";
                                                                       }
                                                                       foreach (DataRow R in sFG)
                                                                       {
                                                                           ssFG += "F/" + R["IPLAYER_NO"].ToString() + "/" + (R["CPLAYER_NAME"].ToString() == "" ? R["CENGNAME"] : R["CPLAYER_NAME"].ToString()) + ",";
                                                                       }
                                                                   }
                                                                   if (j == 3)
                                                                   {
                                                                       sH = sGH.Count > 0 ? "G/" + sGH[0]["IPLAYER_NO"].ToString() + "/" + (sGH[0]["CPLAYER_NAME"].ToString() == "" ? sGH[0]["CENGNAME"] : sGH[0]["CPLAYER_NAME"]) + "," : "";
                                                                       sG = sGG.Count > 0 ? "G/" + sGG[0]["IPLAYER_NO"].ToString() + "/" + (sGG[0]["CPLAYER_NAME"].ToString() == "" ? sGG[0]["CENGNAME"] : sGG[0]["CPLAYER_NAME"]) + "," : "";
                                                                   }
                                                                   if (j == 4)
                                                                   {
                                                                       foreach (DataRow R in sUH)
                                                                       {
                                                                           if (R["IPLAYER_NO"].ToString() != "-1")
                                                                           {
                                                                               ssUH += "F/" + R["IPLAYER_NO"].ToString() + "/" + (R["CPLAYER_NAME"].ToString() == "" ? R["CENGNAME"] : R["CPLAYER_NAME"].ToString()) + ",";
                                                                           }
                                                                       }
                                                                       foreach (DataRow R in sUG)
                                                                       {
                                                                           if (R["IPLAYER_NO"].ToString() != "-1")
                                                                           {
                                                                               ssUG += "F/" + R["IPLAYER_NO"].ToString() + "/" + (R["CPLAYER_NAME"].ToString() == "" ? R["CENGNAME"] : R["CPLAYER_NAME"].ToString()) + ",";
                                                                           }
                                                                       }
                                                                   }
                                                               }

                                                               using (FbCommand cmd2 = new FbCommand("PR_ADDPLAYERLIST", connection2))
                                                               {
                                                                   cmd2.CommandType = CommandType.StoredProcedure;
                                                                   cmd2.Parameters.Add("@CLEAGUE", drs[0]["ALIAS"]);
                                                                   cmd2.Parameters.Add("@CHOST", drH[0]["HKJC_NAME_CN"]);
                                                                   cmd2.Parameters.Add("@CGUEST", drG[0]["HKJC_NAME_CN"]);
                                                                   cmd2.Parameters.Add("@CACTION", "U");
                                                                   cmd2.Parameters.Add("@CH_PLAYER", (sH + "" + ssBH + "" + ssMH + "" + ssFH + ssUH).TrimEnd(','));
                                                                   cmd2.Parameters.Add("@CG_PLAYER", (sG + "" + ssBG + "" + ssMG + "" + ssFG + ssUG).TrimEnd(','));
                                                                   cmd2.Parameters.Add("@TIMEFLAG", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                                                                   cmd2.Parameters.Add("@IDEST", "0");
                                                                   int id = Convert.ToInt32(cmd2.ExecuteScalar());
                                                                   if (id > -1)
                                                                   {
                                                                       Files.WriteLog(" [Success] Insert PLAYERLIST " + " " + drs[0]["ALIAS"] + " " + drH[0]["HKJC_NAME_CN"] + "/" + drG[0]["HKJC_NAME_CN"]);
                                                                   }
                                                               }
                                                           }
                                                       }
                                                   }
                                                   else if (i == 5)
                                                   {
                                                       continue;
                                                       List<string> strL = ds.Tables[0].AsEnumerable().Select(d => d.Field<string>("cid")).ToList<string>().Distinct().ToList().ConvertAll<string>(x => x.ToString());
                                                       string strs = strL.Count == 0 ? "-1" : string.Concat("(", string.Join(" or ", strL), ")");

                                                       queryString = "SELECT  count (*) FROM FIXTURES r";
                                                       using (FbCommand cmd = new FbCommand(queryString, connection2))
                                                       {
                                                           int count = Convert.ToInt32(cmd.ExecuteScalar());
                                                           if (count > 0) strs = strs + " and e.id in (select  c.EMATCHID FROM EMATCHES c where c.CTIMESTAMP >= '" + dt.ToString("yyyy-MM-dd HH:mm:ss.fff") + "' AND C.EMATCHID IS NOT NULL   AND C.EMATCHID > 0 ORDER BY C.CTIMESTAMP ASC)";
                                                       }

                                                       queryString = "select e.id eid, e.COMPETITION_ID,c.ALIAS,l.LEAGUE_CHI_NAME, (select  hkjc_name_cn from teams where id= e.HOME_ID and hkjc_name_cn is not null ) HOME_ID,(select hkjc_name_cn from teams where id= e.GUEST_ID and hkjc_name_cn is not null) GUEST_ID,e.START_DATE, G.H_GOAL||':'||G.G_GOAL RESULT ,G.HH_GOAL||':'||G.GH_GOAL RESULT2 from events e " +
                                                           " inner join COMPETITIONS c on c.id = e.COMPETITION_ID   LEFT join LEAGUE_INFO l on l.CLEAGUE_ALIAS_NAME = c.ALIAS   LEFT JOIN GOALINFO G ON G.EMATCHID = E.ID" +
                                                           "  where" + strs; //" (c.id = 1507 or c.id = 1519 or c.id = 1528 or c.id = 1556 or c.id = 1599 or c.id = 1625 or c.id = 1639 or c.id = 1658 or c.id = 2131 or c.id = 2202) ";
                                                       Files.WriteLog("Sql: " + queryString);
                                                       using (FbCommand cmd = new FbCommand(queryString, connection))
                                                       {
                                                           using (FbDataAdapter fda = new FbDataAdapter(cmd))
                                                           {
                                                               using (DataSet data = new DataSet())
                                                               {
                                                                   data.Tables.Add(new DataTable("data"));
                                                                   fda.Fill(data.Tables["data"]);

                                                                   string ids = string.Concat("(", string.Join(") OR  (", data.Tables[0].AsEnumerable().Select(d => d.Field<int>("eid")).ToList<int>().Distinct().ToList().ConvertAll<string>(x => x.ToString())), ")");
                                                                   Files.WriteLog("Sync ids: " + ids);

                                                                   foreach (DataRow dr in data.Tables[0].Rows)
                                                                   {
                                                                       using (FbCommand cmd2 = new FbCommand("PR_ADDFIXTURE", connection2))
                                                                       {
                                                                           cmd2.CommandType = CommandType.StoredProcedure;
                                                                           cmd2.Parameters.Add("@CLEAGUEALIAS", dr["ALIAS"]);
                                                                           cmd2.Parameters.Add("@CLEAGUE", dr["LEAGUE_CHI_NAME"]);
                                                                           cmd2.Parameters.Add("@CHOST", dr["HOME_ID"] == DBNull.Value ? "" : dr["HOME_ID"]);
                                                                           cmd2.Parameters.Add("@CJCHOST", dr["HOME_ID"] == DBNull.Value ? "" : dr["HOME_ID"]);
                                                                           cmd2.Parameters.Add("@CGUEST", dr["GUEST_ID"] == DBNull.Value ? "" : dr["GUEST_ID"]);
                                                                           cmd2.Parameters.Add("@CJCGUEST", dr["GUEST_ID"] == DBNull.Value ? "" : dr["GUEST_ID"]);
                                                                           cmd2.Parameters.Add("@CMATCHFIELD", "H");
                                                                           cmd2.Parameters.Add("@CMATCHDATE", Convert.ToDateTime(dr["START_DATE"]).ToString("yyyyMMdd"));
                                                                           cmd2.Parameters.Add("@CMATCHTIME", Convert.ToDateTime(dr["START_DATE"]).ToString("HHmm"));
                                                                           cmd2.Parameters.Add("@CHALF_TIME", dr["RESULT2"] == DBNull.Value || dr["RESULT2"].ToString() == "0:0" ? "-1" : dr["RESULT2"]);
                                                                           cmd2.Parameters.Add("@CFULL_TIME", dr["RESULT"] == DBNull.Value ? "-1" : dr["RESULT"]);
                                                                           cmd2.Parameters.Add("@CREMARK", "");
                                                                           cmd2.Parameters.Add("@CACTION", "U");
                                                                           cmd2.Parameters.Add("@CSCORER", "");
                                                                           cmd2.Parameters.Add("@TIMEFLAG", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                                                                           cmd2.Parameters.Add("@IDEST", "0");
                                                                           int id = Convert.ToInt32(cmd2.ExecuteScalar());
                                                                           if (id > -1)
                                                                           {
                                                                               Files.WriteLog(" [Success] Insert FIXTURE" + " " + dr["ALIAS"] + " " + dr["HOME_ID"] + "/" + dr["GUEST_ID"]);
                                                                           }
                                                                       }
                                                                   }
                                                               }
                                                           }
                                                       }
                                                   }
                                                   connection2.Close();
                                               }
                                           }

                                           int msgID = -1;
                                           if (AppFlag.Alert)
                                           {
                                               switch (syncItems[i])
                                               {
                                                   case "LIVEGOALS":
                                                       {
                                                           msgID = 30;
                                                       }
                                                       break;
                                                   case "GOALDETAILS":
                                                       {
                                                           msgID = 31;
                                                       }
                                                       break;
                                                   case "ANALYSISOTHER":
                                                       {
                                                           msgID = 62;
                                                       }
                                                       break;
                                                   case "FIXTURES":
                                                       {
                                                           msgID = 63;
                                                       }
                                                       break;
                                                   //case "ANALYSISTATS":
                                                   //    {
                                                   //        msgID = 14;
                                                   //    }
                                                   //    break;
                                                   //case "ANALYSISHISTORYS":
                                                   //    {
                                                   //        msgID = 11;
                                                   //    }
                                                   //    break; 
                                                   //case "ANALYSISRECENTS":
                                                   //    {
                                                   //        msgID = 13;
                                                   //    }
                                                   //    break;
                                                   case "RANKS":
                                                       {
                                                           msgID = 15;
                                                       }
                                                       break;
                                                   case "SCORERS":
                                                       {
                                                           msgID = 17;
                                                       }
                                                       break;
                                                   case "HKGOAL":
                                                       {
                                                           msgID = 60;
                                                       }
                                                       break;
                                                   case "HKGOALDETAILS":
                                                       {
                                                           msgID = 61;
                                                       }
                                                       break;
                                                       //case "ANALYSISPLAYERLIST":
                                                       //    {
                                                       //        msgID = 12;
                                                       //    }
                                                       //    break;
                                               }
                                               if (msgID != -1)
                                               {
                                                   SendAlertMsg(msgID);
                                                   Files.WriteLog(DateTime.Now.ToString("HH:mm:ss ") + "Send " + msgID);
                                               }
                                           } 
                                       }
                                       connection.Close();
                                   }

                               }
                               catch (Exception exp)
                               {
                                   Files.WriteError(DateTime.Now.ToString("HH:mm:ss ") + syncItems[i] + ",error: " + exp);
                               }
                           }
                         */
                            // Files.WriteLog(" TimerTask " + DateTime.Now);
                            //var state = timerState as TimerState;
                            //Interlocked.Increment(ref state.Counter);
                        }
                        catch (Exception exp)
                        {
                            Files.WriteError("WndProc2(),error: " + exp.Message);

                        }
                    }
                }
            }
            catch (Exception exp)
            {
                Files.WriteError(DateTime.Now.ToString("HH:mm:ss fff ") + " WndProc() ;  Error: " + exp.ToString());
            }
            base.WndProc(ref m);
        }

        public static bool SendAlertMsg(int sBroadcast)
        {
            bool SUCCESS_CODE = false;
            int iResultCode = 0;
            try
            {
                IntPtr handle = PostMessage(HWND_BROADCAST, uiSkSvrNotify2, new Random().Next(), sBroadcast);
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
        private void ScoutDBProvider_Load(object sender, EventArgs e)
        {
            this.listBox1.Invoke(new Action(() =>
            {
                this.listBox1.Invoke(new Action(() => { { this.listBox1.Items.Insert(0, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ") + "Start application."); } }));
            }));
        }
    }

    class TimerState
    {
        public int Counter;
        public bool Result;

    }
}
