/*
Objective:
Update Scorers for each league

Last updated:
12 July 2004, Chapman Choi

C#.NET complier statement:
csc /t:library /out:..\bin\Scorers.dll /r:..\bin\DBManager.dll;..\bin\Files.dll;..\bin\MessageClient.dll;..\bin\SportsMessage.dll Scorers.cs
*/

using System;
using System.Collections;
using System.Configuration;
using System.Data;
//using System.Data.OleDb;
using System.Reflection;
using System.Text;
using System.Web;
using TDL.DB;
using TDL.IO;
using TDL.Message;
using FirebirdSql.Data.FirebirdClient;

namespace SportsUtil
{
    public class Scorers
    {
        const int TOTALRECORDS = 15;
        const string LOGFILESUFFIX = "log";
        string m_Alias;
        string m_Team;
        string m_LeagID;
        // DBManager m_SportsDBMgr;
        DBManagerFB m_SportsDBMgrFb;
         Files m_SportsLog;
        // OleDbDataReader m_SportsOleReader;
        FbDataReader m_SportsOleReaderFb;
        StringBuilder SQLString;
        SortedList m_TeamList;

        public Scorers(string Connection)
        {
            //m_SportsDBMgr = new DBManager();
            //m_SportsDBMgrFb.ConnectionString = Connection;
            m_SportsDBMgrFb = new DBManagerFB();
            m_SportsDBMgrFb.ConnectionString = JC_SoccerWeb.Common.AppFlag.ScoutsDBConn;
            m_SportsLog = new Files();
            SQLString = new StringBuilder();
            m_Alias = "";
            m_Team = "";
            m_LeagID = "";
            m_TeamList = new SortedList();
        }

        public string Alias
        {
            get
            {
                return m_Alias;
            }
        }

        public string Team
        {
            get
            {
                return m_Team;
            }
        }

        public string LeagID
        {
            get
            {
                return m_LeagID;
            }
        }

        public string ShowRank()
        {
            int iIndex = 0;
            int iTabIndex = 0;
            int iRecordCount = 0;
            string sLeagID;
            StringBuilder HTMLString = new StringBuilder();

            //sLeagID = HttpContext.Current.Request.QueryString["leagID"].Trim();
            sLeagID = (HttpContext.Current.Request.QueryString["leagID"] == null) ? "2191" : HttpContext.Current.Request.QueryString["leagID"].Trim();

            try
            {
                SQLString.Remove(0, SQLString.Length);
                // SQLString.Append("SELECT ALIAS FROM LEAGINFO WHERE LEAG_ID='");
                //SQLString.Append("SELECT  aLIAS FROM COMPETITIONS where id= ");
                SQLString.Append("SELECT  l.LEAGUE_CHI_NAME FROM COMPETITIONS c  inner join  LEAGUE_INFO l on l.CLEAGUE_ALIAS_NAME=c.aLIAS  where c.id= ");
                SQLString.Append(sLeagID);
                 SQLString.Append("");
                m_Alias = m_SportsDBMgrFb.ExecuteQueryString(SQLString.ToString());
                m_SportsDBMgrFb.Close();

                SQLString.Remove(0, SQLString.Length);
                //SQLString.Append("SELECT team.TEAM_ID, team.TEAMNAME FROM TEAMINFO team, ID_INFO id WHERE id.TEAM_ID=team.TEAM_ID AND id.LEAG_ID='");
                SQLString.Append(" select distinct   t.id,t.HKJC_NAME_CN from events e inner join  teams t on e.HOME_ID = t.id or e.GUEST_ID = t.id where e.COMPETITION_ID =");
                SQLString.Append(sLeagID);
               // SQLString.Append("' ORDER BY team.TEAM_ID");
                 m_SportsOleReaderFb = m_SportsDBMgrFb.ExecuteQuery(SQLString.ToString());
                while (m_SportsOleReaderFb.Read())
                {
                    m_TeamList.Add(m_SportsOleReaderFb.GetInt32(0).ToString(), m_SportsOleReaderFb.GetString(1).Trim());
                }
                m_SportsOleReaderFb.Close();
                m_SportsDBMgrFb.Close();

                SQLString.Remove(0, SQLString.Length);
                //SQLString.Append("SELECT TEAM_ID, CTEAM_ABBR, CPLAYER_NAME, IRANK, IGOALS FROM SCORERS_INFO WHERE CLEAG_ID='");
                //SQLString.Append(sLeagID);
                //SQLString.Append("' AND CACT='U' ORDER BY IRANK, IRID, CPLAYER_NAME");
                //SQLString.Append("SELECT  distinct SCORERS_INFO.TEAM_ID, CTEAM_ABBR,SCORERS_INFO. CPLAYER_NAME , IRANK, IGOALS ,PLAYERS_INFO.CPLAYER_NAME b FROM SCORERS_INFO inner join PLAYERS_INFO on SCORERS_INFO.PLAYER_ID=PLAYERS_INFO.PLAYER_ID  WHERE CLEAG_ID=");
                //SQLString.Append(sLeagID);
                //SQLString.Append(" AND CACT='U'    ORDER BY IRANK, IRID, SCORERS_INFO. CPLAYER_NAME");
                //SQLString.Append(" select first 15 s.TEAM_ID,  s.CTEAM_ABBR, s.CPLAYER_NAME ,  s.IRANK,  s.IGOALS ,p.CPLAYER_NAME b  from  SCORERS_INFO s  inner join   PLAYERS_INFO p on p.PLAYER_ID = s.PLAYER_ID  and p.TEAM_ID = s.TEAM_ID  WHERE s.CLEAG_ID=");
                //SQLString.Append(sLeagID);
                //SQLString.Append(" AND CACT='U' ORDER BY s.IRANK, s.IRID, s.CPLAYER_NAME");
                SQLString.Append(" select first 15 s.TEAM_ID,  s.CTEAM_ABBR, s.CPLAYER_NAME ,  s.IRANK,  s.IGOALS ,p.CPLAYER_NAME b  from  SCORERS_INFO s  inner join   PLAYERS_INFO p on p.PLAYER_ID = s.PLAYER_ID  and p.TEAM_ID = s.TEAM_ID  WHERE s.CLEAG_ID=");
                SQLString.Append(sLeagID);
                SQLString.Append(" AND CACT='U' ORDER BY s.IRANK, s.IRID, s.CPLAYER_NAME");
                m_SportsOleReaderFb = m_SportsDBMgrFb.ExecuteQuery(SQLString.ToString());
                while (m_SportsOleReaderFb.Read())
                {
                    //Team Reference
                    HTMLString.Append("<tr align=\"center\"><td><select name=\"teamref\" onchange=\"GetPlayer(ScorersForm.LeagID.value, ScorersForm.teamref[");
                    HTMLString.Append(iRecordCount.ToString());
                    HTMLString.Append("].value, ");
                    HTMLString.Append(iRecordCount.ToString());
                    HTMLString.Append(")\"><option value=\"0\">請選擇");
                    for (iIndex = 0; iIndex < m_TeamList.Count; iIndex++)
                    {
                        HTMLString.Append("<option value=\"");
                        HTMLString.Append(m_TeamList.GetKey(iIndex));
                        HTMLString.Append("\">");
                        HTMLString.Append(m_TeamList[m_TeamList.GetKey(iIndex)]);
                    }
                    HTMLString.Append("</select></td>");

                    //Team and its ID
                    HTMLString.Append("<td><input type=\"text\" style=\"background:#778899; color:#FFFFFF\" name=\"team\" maxlength=\"3\" size=\"8\" value=\"");
                    HTMLString.Append(m_TeamList[m_SportsOleReaderFb.GetInt32(0).ToString()]);
                    HTMLString.Append("\" readonly=\"true\"><input type=\"hidden\" name=\"teamid\" value=\"");
                    HTMLString.Append(m_SportsOleReaderFb.GetInt32(0).ToString());
                    HTMLString.Append("\">");

                    //Team abbr.  style=\"displayer:none\" 
                    //  HTMLString.Append("(<input  style=\"display:none\"  name=\"abbr\" maxlength=\"4\" size=\"2\" value=\"");
                    HTMLString.Append("<input  style=\"display:none\"  name=\"abbr\" maxlength=\"4\" size=\"2\" value=\"");
                    if (!m_SportsOleReaderFb.IsDBNull(1))
                    {
                        HTMLString.Append(m_SportsOleReaderFb.GetString(1).Trim());
                    }
                    HTMLString.Append("\" onchange=\"AbbrChange(");
                    HTMLString.Append(iRecordCount.ToString());
                    //HTMLString.Append(")\">)</td>");
                    HTMLString.Append(")\"></td>");
                    //Eng name
                    ///  HTMLString.Append("</td>"); 
                    HTMLString.Append("<td><input type=\"text\" style=\"background:#778899; color:#FFFFFF\" name=\"playerenname\" maxlength=\"50\" size=\"12\" value=\"");
                     HTMLString.Append(m_SportsOleReaderFb.GetString(2).Trim());
                    HTMLString.Append("\" readonly=\"true\"></td>");


                    //Player Label
                    HTMLString.Append("<td><input type=\"text\" style=\"background:#778899; color:#FFFFFF\" name=\"player\" maxlength=\"5\" size=\"12\" value=\"");
                    // HTMLString.Append(m_SportsOleReaderFb.GetString(5).Trim()==""? m_SportsOleReaderFb.GetString(2).Trim(): m_SportsOleReaderFb.GetString(5).Trim());
                    HTMLString.Append(m_SportsOleReaderFb.GetString(5).Trim());
                    HTMLString.Append("\" readonly=\"true\"></td>");

                    iTabIndex++;
                    //Rank
                    HTMLString.Append("<td><input name=\"rank\" maxlength=\"2\" size=\"1\" value=\"");
                    if (!m_SportsOleReaderFb.IsDBNull(3))
                    {
                        HTMLString.Append(m_SportsOleReaderFb.GetInt32(3).ToString());
                    }
                    HTMLString.Append("\" onchange=\"RankValidity(");
                    HTMLString.Append(iRecordCount.ToString());
                    HTMLString.Append(")\" tabindex=\"");
                    HTMLString.Append(iTabIndex.ToString());
                    HTMLString.Append("\"></td>");

                    iTabIndex++;
                    //Goals
                    HTMLString.Append("<td><input name=\"goals\" maxlength=\"2\" size=\"1\" value=\"");
                    if (!m_SportsOleReaderFb.IsDBNull(4))
                    {
                        HTMLString.Append(m_SportsOleReaderFb.GetInt32(4).ToString());
                    }
                    HTMLString.Append("\" onchange=\"GoalsValidity(");
                    HTMLString.Append(iRecordCount.ToString());
                    HTMLString.Append(")\" tabindex=\"");
                    HTMLString.Append(iTabIndex.ToString());
                    HTMLString.Append("\"></td></tr>");

                    iRecordCount++;
                }
                m_SportsOleReaderFb.Close();
                m_SportsDBMgrFb.Close();

                while (iRecordCount < TOTALRECORDS)
                {
                    //Team Reference
                    HTMLString.Append("<tr align=\"center\"><td><select name=\"teamref\" onchange=\"GetPlayer(ScorersForm.LeagID.value, ScorersForm.teamref[");
                    HTMLString.Append(iRecordCount.ToString());
                    HTMLString.Append("].value, ");
                    HTMLString.Append(iRecordCount.ToString());
                    HTMLString.Append(")\"><option value=\"0\">請選擇");
                    for (iIndex = 0; iIndex < m_TeamList.Count; iIndex++)
                    {
                        HTMLString.Append("<option value=\"");
                        HTMLString.Append(m_TeamList.GetKey(iIndex));
                        HTMLString.Append("\">");
                        HTMLString.Append(m_TeamList[m_TeamList.GetKey(iIndex)]);
                    }
                    HTMLString.Append("</select></td>");

                    //Team and its ID
                    HTMLString.Append("<td><input type=\"text\" style=\"background:#778899; color:#FFFFFF\" name=\"team\" maxlength=\"3\" size=\"8\" value=\"\" readonly=\"true\"><input type=\"hidden\" name=\"teamid\" value=\"\">");

                    ////Team abbr.
                    //HTMLString.Append("(<input name=\"abbr\" maxlength=\"4\" size=\"2\" value=\"\" onchange=\"AbbrChange(");
                    //HTMLString.Append(iRecordCount.ToString());
                    //HTMLString.Append(")\">)</td>");

                    //Eng name
                    HTMLString.Append("</td>");
                    HTMLString.Append("<td><input type=\"text\" style=\"background:#778899; color:#FFFFFF\" name=\"playerenname\" maxlength=\"50\" size=\"12\" value=\"\" readonly=\"true\"></td>");


                    //Player
                    HTMLString.Append("<td><input type=\"text\" style=\"background:#778899; color:#FFFFFF\" name=\"player\" maxlength=\"5\" size=\"12\" value=\"\" readonly=\"true\"></td>");

                    iTabIndex++;
                    //Rank
                    HTMLString.Append("<td><input name=\"rank\" maxlength=\"2\" size=\"1\" value=\"\" onchange=\"RankValidity(");
                    HTMLString.Append(iRecordCount.ToString());
                    HTMLString.Append(")\" tabindex=\"");
                    HTMLString.Append(iTabIndex.ToString());
                    HTMLString.Append("\"></td>");

                    iTabIndex++;
                    //Goals
                    HTMLString.Append("<td><input name=\"goals\" maxlength=\"2\" size=\"1\" value=\"\" onchange=\"GoalsValidity(");
                    HTMLString.Append(iRecordCount.ToString());
                    HTMLString.Append(")\" tabindex=\"");
                    HTMLString.Append(iTabIndex.ToString());
                    HTMLString.Append("\"></td></tr>");

                    iRecordCount++;
                }

                HTMLString.Append("<input type=\"hidden\" name=\"Alias\" value=\"");
                HTMLString.Append(m_Alias);
                HTMLString.Append("\"><input type=\"hidden\" name=\"LeagID\" value=\"");
                HTMLString.Append(sLeagID);
                HTMLString.Append("\">");
            }
            catch (Exception ex)
            {
                m_SportsLog.FilePath = ConfigurationManager.AppSettings["errlog"];
                m_SportsLog.SetFileName(0, LOGFILESUFFIX);
                m_SportsLog.Open();
                m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Scorers.cs.ShowRank(): " + ex.ToString());
                m_SportsLog.Close();
                HTMLString.Remove(0, HTMLString.Length);
                HTMLString.Append((string)HttpContext.Current.Application["accessErrorMsg"]);
            }

            return HTMLString.ToString();
        }

        public string ShowPlayers()
        {
            int iExisted = 0;
            string sTeamID;
            string sRecordIndex;
            StringBuilder HTMLString = new StringBuilder();

            // m_LeagID = HttpContext.Current.Request.QueryString["leagID"].Trim();
            m_LeagID = (HttpContext.Current.Request.QueryString["leagID"] == null) ? "2191" : HttpContext.Current.Request.QueryString["leagID"].Trim();

            ////sTeamID = HttpContext.Current.Request.QueryString["teamID"].Trim();
            sTeamID = (HttpContext.Current.Request.QueryString["teamID"] == null) ? "000" : HttpContext.Current.Request.QueryString["teamID"].Trim();

            sRecordIndex = HttpContext.Current.Request.QueryString["RecordIndex"].Trim();
            try
            {
                SQLString.Remove(0, SQLString.Length);
                //SQLString.Append("SELECT team.TEAMNAME, player.CPLAYER_NAME FROM PLAYERS_INFO player, TEAMINFO team WHERE team.TEAM_ID=player.TEAM_ID and player.TEAM_ID=");
                //SQLString.Append(sTeamID);
                //SQLString.Append(" order by player.IPOS, player.IPLAYER_NO");
                SQLString.Append("SELECT team.HKJC_NAME_CN, player.CPLAYER_NAME||'|'||player.CENGNAME  FROM PLAYERS_INFO player, TEAMS team WHERE team.ID=player.TEAM_ID and player.TEAM_ID=");
                SQLString.Append(sTeamID);
                SQLString.Append(" order by player.IPOS, player.IPLAYER_NO");

                m_SportsOleReaderFb = m_SportsDBMgrFb.ExecuteQuery(SQLString.ToString());
                HTMLString.Append("<tr align=\"center\"><td><select name=\"player\">");
                while (m_SportsOleReaderFb.Read())
                {
                    if (!m_Team.Equals(m_SportsOleReaderFb.GetString(0).Trim()))
                    {
                        m_Team = m_SportsOleReaderFb.GetString(0).Trim();
                    }
                    HTMLString.Append("<option value=\"");
                    HTMLString.Append(m_SportsOleReaderFb.GetString(1).Trim());
                    HTMLString.Append("\">");
                    HTMLString.Append(m_SportsOleReaderFb.GetString(1).Trim().Substring(0, m_SportsOleReaderFb.GetString(1).Trim().IndexOf("|")));
                    iExisted++;
                }
                m_SportsOleReaderFb.Close();
                m_SportsDBMgrFb.Close();

                if (iExisted == 0)
                {
                    HTMLString.Append("<option value=\"\">沒有球員");
                }
                HTMLString.Append("</select></td></tr>");

                HTMLString.Append("<input type=\"hidden\" name=\"RecordIndex\" value=\"");
                HTMLString.Append(sRecordIndex);
                HTMLString.Append("\"><input type=\"hidden\" name=\"LeagID\" value=\"");
                HTMLString.Append(m_LeagID);
                HTMLString.Append("\"><input type=\"hidden\" name=\"teamID\" value=\"");
                HTMLString.Append(sTeamID);
                HTMLString.Append("\"><input type=\"hidden\" name=\"teamname\" value=\"");
                HTMLString.Append(m_Team);
                HTMLString.Append("\">");
            }
            catch (Exception ex)
            {
                m_SportsLog.FilePath = ConfigurationManager.AppSettings["errlog"];
                m_SportsLog.SetFileName(0, LOGFILESUFFIX);
                m_SportsLog.Open();
                m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Scorers.cs.ShowPlayers(): " + ex.ToString());
                m_SportsLog.Close();
                HTMLString.Remove(0, HTMLString.Length);
                HTMLString.Append((string)HttpContext.Current.Application["accessErrorMsg"]);
            }

            return HTMLString.ToString();
        }

        public int Update()
        {
            int iUpdIndex;
            int iRecUpd = 0;
            int iSubKeyIndex = 0;
            string sAction;
            string sAlias;
            string sLeagID;
            string sCurrentTimestamp = null;
            string sBatchJob = null;
            char[] delimiter = new char[] { ',' };
            string[] arrRank;
            string[] arrTeamID;
            string[] arrAbbr;
            string[] arrPlayer;
            string[] arrGoals;
            string[] arrMsgType;
            string[] arrSendToPager;
            string[] arrRemotingPath;

            //SportsMessage object message
            SportsMessage sptMsg = new SportsMessage();
            StringBuilder LogSQLString = new StringBuilder();
            DBManager logDBMgr = new DBManager();
            logDBMgr.ConnectionString = m_SportsDBMgrFb.ConnectionString;

            try
            {
                arrSendToPager = HttpContext.Current.Request.Form["SendToPager"].Split(delimiter);
            }
            catch (Exception)
            {
                arrSendToPager = new string[0];
            }
            arrMsgType = (string[])HttpContext.Current.Application["messageType"];
            arrRemotingPath = (string[])HttpContext.Current.Application["RemotingItems"];
            sAction = HttpContext.Current.Request.Form["action"];
            sAlias = HttpContext.Current.Request.Form["Alias"];
            sLeagID = HttpContext.Current.Request.Form["LeagID"];
            arrRank = HttpContext.Current.Request.Form["rank"].Split(delimiter);
            arrTeamID = HttpContext.Current.Request.Form["teamid"].Split(delimiter);
            arrAbbr = HttpContext.Current.Request.Form["abbr"].Split(delimiter);
            arrPlayer = HttpContext.Current.Request.Form["player"].Split(delimiter);
            arrGoals = HttpContext.Current.Request.Form["goals"].Split(delimiter);

            try
            {
                /*****************************
				 * GoGo Pager2 alert message *
				 *****************************/
                string[] arrQueueNames;
                string[] arrMessageTypes;
                arrQueueNames = (string[])HttpContext.Current.Application["QueueItems"];
                arrMessageTypes = (string[])HttpContext.Current.Application["NotifyMessageTypes"];
                MessageClient msgClt = new MessageClient();
                msgClt.MessageType = arrMessageTypes[0];
                msgClt.MessagePath = arrQueueNames[0];

                if (!sAction.Equals("P"))
                {
                    //Clear all records first
                    SQLString.Remove(0, SQLString.Length);
                    SQLString.Append("DELETE FROM SCORERS_INFO where CLEAG_ID='");
                    SQLString.Append(sLeagID);
                    SQLString.Append("'");
                    m_SportsDBMgrFb.ExecuteNonQuery(SQLString.ToString());
                    m_SportsDBMgrFb.Close();
                }

                if (sAction.Equals("U"))
                {
                    for (iUpdIndex = 0; iUpdIndex < arrTeamID.Length; iUpdIndex++)
                    {
                        if (!(arrTeamID[iUpdIndex].Trim().Equals("0") || arrTeamID[iUpdIndex].Trim().Equals("") || arrPlayer[iUpdIndex].Trim().Equals("0") || arrPlayer[iUpdIndex].Trim().Equals("")))
                        {
                            SQLString.Remove(0, SQLString.Length);
                            SQLString.Append("insert into SCORERS_INFO values('");
                            SQLString.Append(sLeagID);
                            SQLString.Append("',");
                            SQLString.Append(arrTeamID[iUpdIndex]);
                            SQLString.Append(",'");
                            SQLString.Append(arrPlayer[iUpdIndex]);
                            SQLString.Append("','");
                            if (arrAbbr[iUpdIndex].Trim().Equals("")) SQLString.Append("null");
                            else SQLString.Append(arrAbbr[iUpdIndex].Trim());
                            SQLString.Append("','U',");
                            SQLString.Append(iRecUpd.ToString());
                            SQLString.Append(",");
                            if (arrRank[iUpdIndex].Trim().Equals("")) SQLString.Append("null");
                            else SQLString.Append(arrRank[iUpdIndex].Trim());
                            SQLString.Append(",");
                            if (arrGoals[iUpdIndex].Trim().Equals("")) SQLString.Append("null");
                            else SQLString.Append(arrGoals[iUpdIndex].Trim());
                            SQLString.Append(")");
                            m_SportsDBMgrFb.ExecuteNonQuery(SQLString.ToString());
                            m_SportsDBMgrFb.Close();
                            iRecUpd++;
                        }
                    }
                }
                else
                {
                    iRecUpd++;
                }

                sCurrentTimestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                if (iRecUpd > 0 && arrSendToPager.Length > 0)
                {
                    if (sAction.Equals("U"))
                    {
                        sBatchJob = DateTime.Now.ToString("yyMMddHHmmss_fffffff") + "." + HttpContext.Current.Session["user_name"].ToString() + "." + arrMsgType[35] + ".ini";

                        SQLString.Remove(0, SQLString.Length);
                        SQLString.Append("SELECT IRANK, CTEAM_ABBR, CPLAYER_NAME, IGOALS FROM SCORERS_INFO WHERE CLEAG_ID='");
                        SQLString.Append(sLeagID);
                        SQLString.Append("' AND CACT='U' ORDER BY IRANK, IRID, CPLAYER_NAME");
                        m_SportsOleReaderFb = m_SportsDBMgrFb.ExecuteQuery(SQLString.ToString());
                        while (m_SportsOleReaderFb.Read())
                        {
                            iSubKeyIndex++;
                            LogSQLString.Remove(0, LogSQLString.Length);
                            LogSQLString.Append("insert into LOG_SCORERS (TIMEFLAG, SECTION, ALIAS, ACT, IITEMSEQ_NO, RANK, TEAMABBR, PLAYER, GOALS, BATCHJOB) values ('");
                            LogSQLString.Append(sCurrentTimestamp);
                            LogSQLString.Append("','SCORERS_','");
                            LogSQLString.Append(sAlias);
                            LogSQLString.Append("','");
                            LogSQLString.Append(sAction);
                            LogSQLString.Append("',");
                            LogSQLString.Append(iSubKeyIndex.ToString());
                            LogSQLString.Append(",");
                            if (!m_SportsOleReaderFb.IsDBNull(0)) LogSQLString.Append(m_SportsOleReaderFb.GetInt32(0).ToString());
                            else LogSQLString.Append("-1");
                            LogSQLString.Append(",'");
                            if (!m_SportsOleReaderFb.IsDBNull(1))
                            {
                                if (m_SportsOleReaderFb.GetString(1).Trim().Equals("")) LogSQLString.Append("-1");
                                else LogSQLString.Append(m_SportsOleReaderFb.GetString(1).Trim());
                            }
                            else
                            {
                                LogSQLString.Append("-1");
                            }
                            LogSQLString.Append("','");
                            if (!m_SportsOleReaderFb.IsDBNull(2)) LogSQLString.Append(m_SportsOleReaderFb.GetString(2).Trim());
                            else LogSQLString.Append("-1");
                            LogSQLString.Append("',");
                            if (!m_SportsOleReaderFb.IsDBNull(3)) LogSQLString.Append(m_SportsOleReaderFb.GetInt32(3).ToString());
                            else LogSQLString.Append("-1");
                            LogSQLString.Append(",'");
                            LogSQLString.Append(sBatchJob);
                            LogSQLString.Append("')");
                            logDBMgr.ExecuteNonQuery(LogSQLString.ToString());
                            logDBMgr.Close();
                        }
                        m_SportsDBMgrFb.Close();
                        m_SportsOleReaderFb.Close();
                    }
                    else
                    {
                        sBatchJob = DateTime.Now.ToString("yyMMddHHmmss_fffffff") + "." + HttpContext.Current.Session["user_name"].ToString() + "." + arrMsgType[35] + ".del.ini";

                        LogSQLString.Remove(0, LogSQLString.Length);
                        LogSQLString.Append("insert into LOG_SCORERS (TIMEFLAG, SECTION, ALIAS, ACT, IITEMSEQ_NO, RANK, TEAMABBR, PLAYER, GOALS, BATCHJOB) values ('");
                        LogSQLString.Append(sCurrentTimestamp);
                        LogSQLString.Append("','SCORERS_','");
                        LogSQLString.Append(sAlias);
                        LogSQLString.Append("','");
                        LogSQLString.Append(sAction);
                        LogSQLString.Append("',null,null,null,null,null,'");
                        LogSQLString.Append(sBatchJob);
                        LogSQLString.Append("')");
                        logDBMgr.ExecuteNonQuery(LogSQLString.ToString());
                        logDBMgr.Close();
                    }

                    //Send Notify Message
                    sptMsg.Body = sBatchJob;
                    sptMsg.Timestamp = DateTime.Now.ToString("yyMMddHHmmss");
                    sptMsg.AppID = "07";
                    sptMsg.MsgID = "31";
                    sptMsg.DeviceID = new string[0];
                    for (int i = 0; i < arrSendToPager.Length; i++)
                    {
                        sptMsg.AddDeviceID((string)arrSendToPager[i]);
                    }
                    try
                    {
                        //Notify via MSMQ
                        msgClt.MessageType = arrMessageTypes[0];
                        msgClt.MessagePath = arrQueueNames[0];
                        msgClt.SendMessage(sptMsg);
                    }
                    //// catch (System.Messaging.MessageQueueException mqEx)
                    catch (InvalidOperationException mqEx)
                    {
                        try
                        {
                            m_SportsLog.FilePath = ConfigurationManager.AppSettings["errlog"];
                            m_SportsLog.SetFileName(0, LOGFILESUFFIX);
                            m_SportsLog.Open();
                            m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " MSMQ ERROR: Scorers");
                            m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Scorers.cs.Update(): Notify via MSMQ throws MessageQueueException:  " + mqEx.ToString());
                            m_SportsLog.Close();

                            //If MSMQ fail, notify via .NET Remoting
                            msgClt.MessageType = arrMessageTypes[1];
                            msgClt.MessagePath = arrRemotingPath[0];
                            if (!msgClt.SendMessage((object)sptMsg))
                            {
                                m_SportsLog.FilePath = ConfigurationManager.AppSettings["errlog"];
                                m_SportsLog.SetFileName(0, LOGFILESUFFIX);
                                m_SportsLog.Open();
                                m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Remoting ERROR: Scorers");
                                m_SportsLog.Close();
                            }
                        }
                        catch (Exception ex)
                        {
                            m_SportsLog.FilePath = ConfigurationManager.AppSettings["errlog"];
                            m_SportsLog.SetFileName(0, LOGFILESUFFIX);
                            m_SportsLog.Open();
                            m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Remoting ERROR: Scorers");
                            m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Scorers.cs.Update(): Notify via .NET Remoting throws exception: " + ex.ToString());
                            m_SportsLog.Close();
                        }
                    }
                    catch (Exception ex)
                    {
                        m_SportsLog.FilePath = ConfigurationManager.AppSettings["errlog"];
                        m_SportsLog.SetFileName(0, LOGFILESUFFIX);
                        m_SportsLog.Open();
                        m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Scorers.cs.Update(): Notify via MSMQ throws exception: " + ex.ToString());
                        m_SportsLog.Close();
                    }
                }   //Insert log into LOG_RANK

                //write log
                m_SportsLog.FilePath = ConfigurationManager.AppSettings["eventlog"];
                m_SportsLog.SetFileName(0, LOGFILESUFFIX);
                m_SportsLog.Open();
                m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Scorers.cs: Update(" + sAction + ") " + iRecUpd.ToString() + " scorers (" + HttpContext.Current.Session["user_name"] + ")");
                m_SportsLog.Close();
            }
            catch (Exception ex)
            {
                iRecUpd = -1;
                m_SportsLog.FilePath = ConfigurationManager.AppSettings["errlog"];
                m_SportsLog.SetFileName(0, LOGFILESUFFIX);
                m_SportsLog.Open();
                m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Scorers.cs.Update(): " + ex.ToString());
                m_SportsLog.Close();
            }

            return iRecUpd;
        }
    }
}