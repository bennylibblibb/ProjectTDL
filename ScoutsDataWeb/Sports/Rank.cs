/*
Objective:
Update rank for each league

Last updated:
10 Feb 2004 by HenryCi

C#.NET complier statement:
csc /t:library /out:..\bin\Rank.dll /r:..\bin\DBManager.dll;..\bin\Files.dll;..\bin\MessageClient.dll;..\bin\SportsMessage.dll Rank.cs
*/

using System;
using System.Configuration;
using System.Data.OleDb;
using System.Reflection;
using System.Text;
using System.Web;
using TDL.DB;
using TDL.IO;
using TDL.Message;

namespace SportsUtil
{
    public class Rank
    {
        const string LOGFILESUFFIX = "log";
        string m_Alias;
        DBManager m_SportsDBMgr;
        Files m_SportsLog;
        OleDbDataReader m_SportsOleReader;
        Encoding m_Big5Encoded;
        StringBuilder SQLString;

        public Rank(string Connection)
        {
            m_SportsDBMgr = new DBManager();
            m_SportsDBMgr.ConnectionString = Connection;
            m_SportsLog = new Files();
            m_Big5Encoded = Encoding.GetEncoding(950);
            SQLString = new StringBuilder();
            m_Alias = "";
        }

        public string Alias
        {
            get
            {
                return m_Alias;
            }
        }

        public string ShowRank()
        {
            int iIndex = 0;
            string sLeagID;
            StringBuilder HTMLString = new StringBuilder();

            ////sLeagID = HttpContext.Current.Request.QueryString["leagID"].Trim();
            sLeagID = (HttpContext.Current.Request.QueryString["leagID"] == null) ? "001" : HttpContext.Current.Request.QueryString["leagID"].Trim();

            SQLString.Remove(0, SQLString.Length);
            SQLString.Append("SELECT leag.ALIAS, rank.RANK, team.TEAMNAME, rank.GAMES, rank.SCORE, team.CHKJCSHORTNAME FROM LEAGINFO leag ");
            SQLString.Append("LEFT OUTER JOIN TEAMINFO team ON team.TEAM_ID in (select TEAM_ID from ID_INFO where LEAG_ID='");
            SQLString.Append(sLeagID);
            SQLString.Append("') LEFT OUTER JOIN LEAGRANKINFO rank ON rank.LEAG_ID=leag.LEAG_ID and rank.LEAG_ID='");
            SQLString.Append(sLeagID);
            SQLString.Append("' and rank.TEAM=team.TEAMNAME ORDER BY rank.RANK");
            try
            {
                m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
                while (m_SportsOleReader.Read())
                {
                    if (!m_SportsOleReader.IsDBNull(0)) m_Alias = m_SportsOleReader.GetString(0).Trim();

                    //rank
                    HTMLString.Append("<tr align=\"center\"><td><input name=\"rank\" maxlength=\"2\" size=\"1\" value=\"");
                    if (!m_SportsOleReader.IsDBNull(1)) HTMLString.Append(m_SportsOleReader.GetInt32(1).ToString());
                    HTMLString.Append("\" onChange=\"RankValidity(");
                    HTMLString.Append(iIndex.ToString());
                    HTMLString.Append(")\"></td>");

                    //team name
                    HTMLString.Append("<td><input type=\"hidden\" name=\"team\" value=\"");
                    if (!m_SportsOleReader.IsDBNull(2)) HTMLString.Append(m_SportsOleReader.GetString(2).Trim());
                    HTMLString.Append("\">");
                    HTMLString.Append(m_SportsOleReader.GetString(2).Trim());
                    HTMLString.Append("</td>");

                    //HKJC team name
                    HTMLString.Append("<td><input type=\"hidden\" name=\"hkjcteam\" value=\"");
                    if (!m_SportsOleReader.IsDBNull(5)) HTMLString.Append(m_SportsOleReader.GetString(5).Trim());
                    HTMLString.Append("\">");
                    HTMLString.Append(m_SportsOleReader.GetString(5).Trim());
                    HTMLString.Append("</td>");

                    //no. of games
                    HTMLString.Append("<td><input name=\"games\" maxlength=\"2\" size=\"1\" value=\"");
                    if (!m_SportsOleReader.IsDBNull(3)) HTMLString.Append(m_SportsOleReader.GetInt32(3).ToString());
                    HTMLString.Append("\" onChange=\"GamesValidity(");
                    HTMLString.Append(iIndex.ToString());
                    HTMLString.Append(")\"></td>");

                    //score
                    HTMLString.Append("<td><input name=\"score\" maxlength=\"3\" size=\"1\" value=\"");
                    if (!m_SportsOleReader.IsDBNull(4)) HTMLString.Append(m_SportsOleReader.GetInt32(4).ToString());
                    HTMLString.Append("\" onChange=\"ScoreValidity(");
                    HTMLString.Append(iIndex.ToString());
                    HTMLString.Append(")\"></td></tr>");
                    iIndex++;
                }
                m_SportsOleReader.Close();
                m_SportsDBMgr.Close();

                if (iIndex == 0)
                {
                    SQLString.Remove(0, SQLString.Length);
                    SQLString.Append("select leaginfo.alias, teaminfo.teamname, teaminfo.chkjcshortname from leaginfo, teaminfo, id_info where leaginfo.leag_id=id_info.leag_id and teaminfo.team_id=id_info.team_id and id_info.leag_id='");
                    SQLString.Append(sLeagID);
                    SQLString.Append("' order by teaminfo.teamname");

                    m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
                    while (m_SportsOleReader.Read())
                    {
                        if (!m_SportsOleReader.IsDBNull(0)) m_Alias = m_SportsOleReader.GetString(0).Trim();

                        //rank
                        HTMLString.Append("<tr align=\"center\"><td><input name=\"rank\" maxlength=\"2\" size=\"1\" value=\"\" onChange=\"RankValidity(");
                        HTMLString.Append(iIndex.ToString());
                        HTMLString.Append(")\"></td>");

                        //team name
                        HTMLString.Append("<td><input type=\"hidden\" name=\"team\" value=\"");
                        if (!m_SportsOleReader.IsDBNull(1)) HTMLString.Append(m_SportsOleReader.GetString(1).Trim());
                        HTMLString.Append("\">");
                        HTMLString.Append(m_SportsOleReader.GetString(1).Trim());
                        HTMLString.Append("</td>");

                        //HKJC team name
                        HTMLString.Append("<td><input type=\"hidden\" name=\"hkjcteam\" value=\"");
                        if (!m_SportsOleReader.IsDBNull(2)) HTMLString.Append(m_SportsOleReader.GetString(2).Trim());
                        HTMLString.Append("\">");
                        HTMLString.Append(m_SportsOleReader.GetString(2).Trim());
                        HTMLString.Append("</td>");

                        //no. of games
                        HTMLString.Append("<td><input name=\"games\" maxlength=\"2\" size=\"1\" value=\"\" onChange=\"GamesValidity(");
                        HTMLString.Append(iIndex.ToString());
                        HTMLString.Append(")\"></td>");

                        //score
                        HTMLString.Append("<td><input name=\"score\" maxlength=\"3\" size=\"1\" value=\"\" onChange=\"ScoreValidity(");
                        HTMLString.Append(iIndex.ToString());
                        HTMLString.Append(")\"></td></tr>");
                        iIndex++;
                    }
                    m_SportsOleReader.Close();
                    m_SportsDBMgr.Close();
                }
                m_SportsDBMgr.Dispose();

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
                m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Rank.cs.ShowRank(): " + ex.ToString());
                m_SportsLog.Close();
                HTMLString.Remove(0, HTMLString.Length);
                HTMLString.Append((string)HttpContext.Current.Application["accessErrorMsg"]);
            }
            return HTMLString.ToString();
        }

        public int Update()
        {
            int iUpdIndex;
            //int iExisted = 0;
            int iRecUpd = 0;
            int iINIIndex = 0;
            string sAction;
            string sLeagID;
            string sAlias;
            string sTempValue = null;
            string sCurrentTimestamp = null;
            string sBatchJob = null;
            char[] delimiter = new char[] { ',' };
            string[] arrRank;
            string[] arrTeam;
            string[] arrHKJCTeam;
            string[] arrGames;
            string[] arrScore;
            string[] arrMsgType;
            string[] arrSendToPager;
            string[] arrRemotingPath;

            //SportsMessage object message
            SportsMessage sptMsg = new SportsMessage();
            StringBuilder LogSQLString = new StringBuilder();
            DBManager logDBMgr = new DBManager();
            logDBMgr.ConnectionString = m_SportsDBMgr.ConnectionString;

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
            sLeagID = HttpContext.Current.Request.Form["LeagID"];
            sAlias = HttpContext.Current.Request.Form["Alias"];
            arrRank = HttpContext.Current.Request.Form["rank"].Split(delimiter);
            arrTeam = HttpContext.Current.Request.Form["team"].Split(delimiter);
            arrHKJCTeam = HttpContext.Current.Request.Form["hkjcteam"].Split(delimiter);
            arrGames = HttpContext.Current.Request.Form["games"].Split(delimiter);
            arrScore = HttpContext.Current.Request.Form["score"].Split(delimiter);
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

                if (sAction.Equals("U"))
                {

                    //clear current league
                    SQLString.Remove(0, SQLString.Length);
                    SQLString.Append("delete from LEAGRANKINFO where LEAG_ID='");
                    SQLString.Append(sLeagID);
                    SQLString.Append("'");
                    m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
                    m_SportsDBMgr.Close();

                    for (iUpdIndex = 0; iUpdIndex < arrTeam.Length; iUpdIndex++)
                    {
                        if (!arrRank[iUpdIndex].Trim().Equals("") || !arrScore[iUpdIndex].Trim().Equals(""))
                        {
                            iRecUpd++;
                            //insert team record
                            SQLString.Remove(0, SQLString.Length);
                            SQLString.Append("insert into LEAGRANKINFO values('");
                            SQLString.Append(sLeagID);
                            SQLString.Append("',");
                            if (arrRank[iUpdIndex].Trim().Equals("")) SQLString.Append("null");
                            else SQLString.Append(arrRank[iUpdIndex].Trim());
                            SQLString.Append(",'");
                            SQLString.Append(arrTeam[iUpdIndex]);
                            SQLString.Append("','");
                            SQLString.Append(arrHKJCTeam[iUpdIndex]);
                            SQLString.Append("',");
                            if (arrScore[iUpdIndex].Trim().Equals("")) SQLString.Append("null");
                            else SQLString.Append(arrScore[iUpdIndex].Trim());
                            SQLString.Append(",'0',");
                            if (arrGames[iUpdIndex].Trim().Equals("")) SQLString.Append("null");
                            else SQLString.Append(arrGames[iUpdIndex].Trim());
                            SQLString.Append(")");

                            /*
							SQLString.Remove(0,SQLString.Length);
							SQLString.Append("select count(TEAM) from LEAGRANKINFO where TEAM='");
							SQLString.Append(arrTeam[iUpdIndex]);
							SQLString.Append("' AND LEAG_ID='");
							SQLString.Append(sLeagID);
							SQLString.Append("'");
							iExisted = m_SportsDBMgr.ExecuteScalar(SQLString.ToString());
							m_SportsDBMgr.Close();
							SQLString.Remove(0,SQLString.Length);
							if(iExisted > 0) {		//update
								SQLString.Append("update LEAGRANKINFO SET RANK=");
								if(arrRank[iUpdIndex].Trim().Equals("")) SQLString.Append("null");
								else SQLString.Append(arrRank[iUpdIndex].Trim());
								SQLString.Append(", SCORE=");
								if(arrScore[iUpdIndex].Trim().Equals("")) SQLString.Append("null");
								else SQLString.Append(arrScore[iUpdIndex].Trim());
								SQLString.Append(", GAMES=");
								if(arrGames[iUpdIndex].Trim().Equals("")) SQLString.Append("null");
								else SQLString.Append(arrGames[iUpdIndex].Trim());
								SQLString.Append(" where TEAM='");
								SQLString.Append(arrTeam[iUpdIndex]);
								SQLString.Append("' AND LEAG_ID='");
								SQLString.Append(sLeagID);
								SQLString.Append("'");
							} else {		//insert
								SQLString.Append("insert into LEAGRANKINFO values('");
								SQLString.Append(sLeagID);
								SQLString.Append("',");
								if(arrRank[iUpdIndex].Trim().Equals("")) SQLString.Append("null");
								else SQLString.Append(arrRank[iUpdIndex].Trim());
								SQLString.Append(",'");
								SQLString.Append(arrTeam[iUpdIndex]);
								SQLString.Append("',");
								if(arrScore[iUpdIndex].Trim().Equals("")) SQLString.Append("null");
								else SQLString.Append(arrScore[iUpdIndex].Trim());
								SQLString.Append(",'0',");
								if(arrGames[iUpdIndex].Trim().Equals("")) SQLString.Append("null");
								else SQLString.Append(arrGames[iUpdIndex].Trim());
								SQLString.Append(")");
							}
							*/
                            m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
                            m_SportsDBMgr.Close();
                        }
                    }
                }
                else
                {
                    iRecUpd++;
                    if (sAction.Equals("D"))
                    {
                        SQLString.Remove(0, SQLString.Length);
                        SQLString.Append("delete from LEAGRANKINFO where LEAG_ID='");
                        SQLString.Append(sLeagID);
                        SQLString.Append("'");
                        m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
                        m_SportsDBMgr.Close();
                    }
                }

                sCurrentTimestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                if (iRecUpd > 0 && arrSendToPager.Length > 0)
                {
                    if (sAction.Equals("U"))
                    {
                        sBatchJob = DateTime.Now.ToString("yyMMddHHmmss_fffffff") + "." + HttpContext.Current.Session["user_name"].ToString() + "." + arrMsgType[22] + ".ini";

                        SQLString.Remove(0, SQLString.Length);
                        SQLString.Append("select RANK, TEAM, SCORE, GAMES, HKJC_TEAM from LEAGRANKINFO where LEAG_ID='");
                        SQLString.Append(sLeagID);
                        SQLString.Append("' order by RANK");
                        m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
                        while (m_SportsOleReader.Read())
                        {
                            iINIIndex++;
                            LogSQLString.Remove(0, LogSQLString.Length);
                            LogSQLString.Append("insert into LOG_RANK (TIMEFLAG, SECTION, LEAGUEALIAS, ACT, IITEMSEQ_NO, RANK, TEAM, HKJC_TEAM, SCORE, EVENTCNT, BATCHJOB) values ('");
                            LogSQLString.Append(sCurrentTimestamp);
                            LogSQLString.Append("','LEAGUERANK_','");
                            LogSQLString.Append(sAlias);
                            LogSQLString.Append("','");
                            LogSQLString.Append(sAction);
                            LogSQLString.Append("',");
                            LogSQLString.Append(iINIIndex.ToString());
                            LogSQLString.Append(",");
                            if (!m_SportsOleReader.IsDBNull(0)) sTempValue = m_SportsOleReader.GetInt32(0).ToString();
                            else sTempValue = "-1";
                            LogSQLString.Append(sTempValue);
                            LogSQLString.Append(",'");
                            if (!m_SportsOleReader.IsDBNull(1))
                            {
                                if (m_SportsOleReader.GetString(1).Trim().Equals("")) sTempValue = "-1";
                                else sTempValue = m_SportsOleReader.GetString(1).Trim();
                            }
                            else
                            {
                                sTempValue = "-1";
                            }
                            LogSQLString.Append(sTempValue);
                            LogSQLString.Append("','");
                            if (!m_SportsOleReader.IsDBNull(4))
                            {
                                if (m_SportsOleReader.GetString(4).Trim().Equals("")) sTempValue = "-1";
                                else sTempValue = m_SportsOleReader.GetString(4).Trim();
                            }
                            else
                            {
                                sTempValue = "-1";
                            }
                            LogSQLString.Append(sTempValue);
                            LogSQLString.Append("',");
                            if (!m_SportsOleReader.IsDBNull(2)) sTempValue = m_SportsOleReader.GetInt32(2).ToString();
                            else sTempValue = "-1";
                            LogSQLString.Append(sTempValue);
                            LogSQLString.Append(",");
                            if (!m_SportsOleReader.IsDBNull(3)) sTempValue = m_SportsOleReader.GetInt32(3).ToString();
                            else sTempValue = "-1";
                            LogSQLString.Append(sTempValue);
                            LogSQLString.Append(",'");
                            LogSQLString.Append(sBatchJob);
                            LogSQLString.Append("')");
                            logDBMgr.ExecuteNonQuery(LogSQLString.ToString());
                            logDBMgr.Close();
                        }
                        m_SportsDBMgr.Close();
                        m_SportsOleReader.Close();
                    }
                    else
                    {
                        sBatchJob = DateTime.Now.ToString("yyMMddHHmmss_fffffff") + "." + HttpContext.Current.Session["user_name"].ToString() + "." + arrMsgType[22] + ".del.ini";

                        LogSQLString.Remove(0, LogSQLString.Length);
                        LogSQLString.Append("insert into LOG_RANK (TIMEFLAG, SECTION, LEAGUEALIAS, ACT, IITEMSEQ_NO, RANK, TEAM, HKJC_TEAM, SCORE, EVENTCNT, BATCHJOB) values ('");
                        LogSQLString.Append(sCurrentTimestamp);
                        LogSQLString.Append("','LEAGUERANK_','");
                        LogSQLString.Append(sAlias);
                        LogSQLString.Append("','");
                        LogSQLString.Append(sAction);
                        LogSQLString.Append("',null,null,null,null,null,null,'");
                        LogSQLString.Append(sBatchJob);
                        LogSQLString.Append("')");
                        logDBMgr.ExecuteNonQuery(LogSQLString.ToString());
                        logDBMgr.Close();
                    }
                    //modified by Henry, 10 Feb 2004
                    //Send Notify Message
                    sptMsg.Body = sBatchJob;
                    sptMsg.Timestamp = DateTime.Now.ToString("yyMMddHHmmss");
                    sptMsg.AppID = "07";
                    sptMsg.MsgID = "14";
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
                    ////catch (System.Messaging.MessageQueueException mqEx)
                    catch (InvalidOperationException mqEx)
                    {
                        try
                        {
                            m_SportsLog.FilePath = ConfigurationManager.AppSettings["errlog"];
                            m_SportsLog.SetFileName(0, LOGFILESUFFIX);
                            m_SportsLog.Open();
                            m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " MSMQ ERROR: Rank");
                            m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Rank.cs.Update(): Notify via MSMQ throws MessageQueueException:  " + mqEx.ToString());
                            m_SportsLog.Close();

                            //If MSMQ fail, notify via .NET Remoting
                            msgClt.MessageType = arrMessageTypes[1];
                            msgClt.MessagePath = arrRemotingPath[0];
                            if (!msgClt.SendMessage((object)sptMsg))
                            {
                                m_SportsLog.FilePath = ConfigurationManager.AppSettings["errlog"];
                                m_SportsLog.SetFileName(0, LOGFILESUFFIX);
                                m_SportsLog.Open();
                                m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Remoting ERROR: Rank");
                                m_SportsLog.Close();
                            }
                        }
                        catch (Exception ex)
                        {
                            m_SportsLog.FilePath = ConfigurationManager.AppSettings["errlog"];
                            m_SportsLog.SetFileName(0, LOGFILESUFFIX);
                            m_SportsLog.Open();
                            m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Remoting ERROR: Rank");
                            m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Rank.cs.Update(): Notify via .NET Remoting throws exception: " + ex.ToString());
                            m_SportsLog.Close();
                        }
                    }
                    catch (Exception ex)
                    {
                        m_SportsLog.FilePath = ConfigurationManager.AppSettings["errlog"];
                        m_SportsLog.SetFileName(0, LOGFILESUFFIX);
                        m_SportsLog.Open();
                        m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Rank.cs.Update(): Notify via MSMQ throws exception: " + ex.ToString());
                        m_SportsLog.Close();
                    }
                    //end modify
                }   //Insert log into LOG_RANK

                //write log
                m_SportsLog.FilePath = ConfigurationManager.AppSettings["eventlog"];
                m_SportsLog.SetFileName(0, LOGFILESUFFIX);
                m_SportsLog.Open();
                m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Rank.cs: Update(" + sAction + ") " + iRecUpd.ToString() + " rank (" + HttpContext.Current.Session["user_name"] + ")");
                m_SportsLog.Close();
            }
            catch (Exception ex)
            {
                iRecUpd = -1;
                m_SportsLog.FilePath = ConfigurationManager.AppSettings["errlog"];
                m_SportsLog.SetFileName(0, LOGFILESUFFIX);
                m_SportsLog.Open();
                m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Rank.cs.Update(): " + ex.ToString());
                m_SportsLog.Close();
            }
            return iRecUpd;
        }
    }
}