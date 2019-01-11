/*
Objective:
Preview and send matches analysis information

Last updated:
12 July 2004, Chapman Choi

C#.NET complier statement:
csc /t:library /out:..\bin\AnalysisPreview.dll /r:..\bin\DBManager.dll;..\bin\Files.dll;..\bin\MessageClient.dll;..\bin\SportsMessage.dll AnalysisPreview.cs
*/

using FirebirdSql.Data.FirebirdClient;
using System;
using System.Collections;
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
    public class AnalysisPreview
    {
        const string LOGFILESUFFIX = "log";
        int iRecordsInPage = 0;
        OleDbDataReader m_SportsOleReader;
        DBManager m_SportsDBMgr;
        Files m_SportsLog;
        Encoding m_Big5Encoded;
        StringBuilder SQLString;
        string[] arrFields;

        public string m_Title = "";
        DBManagerFB m_SportsDBMgrFb;
        FbDataReader m_SportsOleReaderFb;
        public AnalysisPreview(string Connection)
        {
            m_SportsDBMgr = new DBManager();
            m_SportsDBMgr.ConnectionString = Connection;
            m_SportsLog = new Files();
            m_Big5Encoded = Encoding.GetEncoding(950);
            SQLString = new StringBuilder();
            arrFields = (string[])HttpContext.Current.Application["fieldItemsArray"];

            m_SportsDBMgrFb = new DBManagerFB();
            m_SportsDBMgrFb.ConnectionString = JC_SoccerWeb.Common.AppFlag.ScoutsDBConn;
        }

        public int NumberOfRecords
        {
            get
            {
                return iRecordsInPage;
            }
        }

        public string PreviewMatches()
        {
            int iMatchCnt = 0;
            string sMatchDate = "";
            string sMatchTime = "";
            string uid;
            StringBuilder HTMLString = new StringBuilder();
            string sEventID;
            string sDayCode = "";

            try
            {
                sEventID = (HttpContext.Current.Request.QueryString["eventid"] == null) ? "" : HttpContext.Current.Request.QueryString["eventid"].Trim();

               //// uid = HttpContext.Current.Session["user_id"].ToString();
                SQLString.Remove(0, SQLString.Length);
                ////SQLString.Append("select gameinfo.matchdate, gameinfo.matchtime, gameinfo.leaglong, gameinfo.league, gameinfo.host, gameinfo.guest, gameinfo.match_cnt, leaginfo.LEAGUETYPE from gameinfo, leaginfo, analysis_bg_info where gameinfo.match_cnt=analysis_bg_info.imatch_cnt and gameinfo.league=leaginfo.alias and analysis_bg_info.cact='U' and leaginfo.leag_id in (select cleag_id from userprofile_info where iuser_id=");
                ////SQLString.Append(uid);
                ////SQLString.Append(") order by leaginfo.leag_order, gameinfo.matchdate, gameinfo.matchtime");
                SQLString.Append("select cast(cast(r.CMATCHDATETIME as date) as varchar(10)), cast(r.CMATCHDATETIME as time),r.CLEAGUEALIAS_OUTPUT_NAME , r.CLEAGUE_OUTPUT_NAME,   r.HKJCHOSTNAME_CN, r.HKJCGUESTNAME_CN,  e.id , '' sLeagType , R.HKJCDAYCODE,");
                SQLString.Append(" (select a.NAME from TEAMS t inner join  AREAS a on t.AREA_ID=a.id  where t.ID=E.HOME_ID) , (select a.name from TEAMS t inner join  AREAS a on t.AREA_ID=a.id  where t.ID=E.HOME_ID) ,");
                SQLString.Append(" (select b2.NAME from AREAS B inner join AREAS b2 on b2.id=b.PARENT_AREA_ID where B.ID =(select AREA_ID from TEAMS where ID=e.HOME_ID)),");
                SQLString.Append(" (select  a.NAME from TEAMS t inner join  AREAS a on t.AREA_ID=a.id  where t.ID=E.GUEST_ID) ,  (select  a.NAME from TEAMS t inner join  AREAS a on t.AREA_ID=a.id  where t.ID=E.GUEST_ID) ,");
                SQLString.Append(" (select b2.NAME from AREAS B inner join AREAS b2 on b2.id=b.PARENT_AREA_ID  where B.ID =(select AREA_ID from TEAMS where ID=e.GUEST_ID)), x.name from  EMATCHES r inner join  events e on e.id =r.EMATCHID  inner join  AREAS x on x.id =e.AREA_ID  ");
                // SQLString.Append(" WHERE r.HKJCDAYCODE = (SELECT HKJCDAYCODE FROM EMATCHES WHERE EMATCHID ="+sEventID+ "  )  AND cast(cast(r.CMATCHDATETIME as date) as varchar(10))= (SELECT cast(cast(CMATCHDATETIME as date) as varchar(10)) FROM EMATCHES WHERE EMATCHID = " + sEventID + "   )");
                //  --and r.CMATCHDATETIME + 1 > (SELECT CMATCHDATETIME FROM EMATCHES WHERE EMATCHID = 2455014   ) and r.CMATCHDATETIME - 1 < (SELECT CMATCHDATETIME FROM EMATCHES WHERE EMATCHID = 2455014   )  
                if (sEventID != "")
                {
                    SQLString.Append(" WHERE r.HKJCDAYCODE = (SELECT HKJCDAYCODE FROM EMATCHES WHERE EMATCHID =" + sEventID + "  ) and r.CMATCHDATETIME < (SELECT CMATCHDATETIME FROM EMATCHES WHERE EMATCHID =  " + sEventID + " )+1 and r.CMATCHDATETIME > (SELECT CMATCHDATETIME FROM EMATCHES WHERE EMATCHID =  " + sEventID + " ) -1 ");
                }else
                {
                    SQLString.Append("   WHERE r.HKJCDAYCODE = (SELECT first 1 HKJCDAYCODE FROM EMATCHES WHERE  cast(cast(CMATCHDATETIME as date) as varchar(10)) = cast(cast(current_timestamp as date) as varchar(10))   ) "); 
                  SQLString.Append("  and r.CMATCHDATETIME < (SELECT first 1 CMATCHDATETIME FROM EMATCHES WHERE  cast(cast(CMATCHDATETIME as date) as varchar(10)) = cast(cast(current_timestamp as date) as varchar(10))   )+1 and r.CMATCHDATETIME > (SELECT  first 1 CMATCHDATETIME FROM EMATCHES WHERE  cast(cast(CMATCHDATETIME as date) as varchar(10)) = cast(cast(current_timestamp as date) as varchar(10))   ) -1 ");
                }
                m_SportsOleReaderFb = m_SportsDBMgrFb.ExecuteQuery(SQLString.ToString());
                while (m_SportsOleReaderFb.Read())
                {
                    sDayCode = m_SportsOleReaderFb.GetString(8).Trim();
                    m_Title = "(" + sDayCode + ")" + "發送分析";
                    SQLString.Remove(0, SQLString.Length);
                    SQLString.Append("select count(IMATCH_CNT) from ANALYSIS_BG_INFO where CACT='U' and IMATCH_CNT=");
                    SQLString.Append(m_SportsOleReaderFb.GetString(6).Trim());
                    int iCount = m_SportsDBMgr.ExecuteScalar(SQLString.ToString());
                    m_SportsDBMgr.Close();
                    if(iCount == 0) { continue; }
                   // sDayCode = m_SportsOleReaderFb.GetString(8).Trim();
                    sMatchDate = m_SportsOleReaderFb.GetString(0).Trim();
                    sMatchTime = m_SportsOleReaderFb.GetString(1).Trim();
                    //sMatchDate = sMatchDate.Insert(4, "/");
                    //sMatchDate = sMatchDate.Insert(7, "/");
                    //sMatchTime = sMatchTime.Insert(2, ":");

                    HTMLString.Append("<tr align=\"center\"><td>");
                    HTMLString.Append(sMatchDate);
                    HTMLString.Append("&nbsp;");
                    HTMLString.Append(sMatchTime);
                    HTMLString.Append("</td><td><input type=\"hidden\" name=\"league\" value=\"");
                    HTMLString.Append(m_SportsOleReaderFb.GetString(2).Trim());
                    HTMLString.Append("\">");
                    HTMLString.Append(m_SportsOleReaderFb.GetString(3).Trim());
                    HTMLString.Append("</td><td><input type=\"hidden\" name=\"host\" value=\"");
                    HTMLString.Append(m_SportsOleReaderFb.GetString(4).Trim());
                    HTMLString.Append("\">");
                    HTMLString.Append(m_SportsOleReaderFb.GetString(4).Trim());
                    HTMLString.Append("</td><td><input type=\"hidden\" name=\"guest\" value=\"");
                    HTMLString.Append(m_SportsOleReaderFb.GetString(5).Trim());
                    HTMLString.Append("\">");
                    HTMLString.Append(m_SportsOleReaderFb.GetString(5).Trim());
                    HTMLString.Append("</td><td><select name=\"Action\" onChange=\"OnActionChanged(");
                    HTMLString.Append(iRecordsInPage.ToString());
                    HTMLString.Append(")\"><option value=\"U\">發送<option value=\"D\">刪除</select></td>");

                    iMatchCnt = m_SportsOleReaderFb.GetInt32(6);
                    HTMLString.Append("<td><input type=\"hidden\" name=\"matchcount\" value=\"");
                    HTMLString.Append(iMatchCnt.ToString());
                    HTMLString.Append("\"><input type=\"hidden\" name=\"leaguetype\" value=\"");
                    HTMLString.Append(m_SportsOleReaderFb.GetString(7).Trim());
                    HTMLString.Append("\"><input type=\"checkbox\" name=\"analysis_bg\" value=\"");
                    HTMLString.Append(iMatchCnt.ToString());
                    HTMLString.Append("\"></td><td><input type=\"checkbox\" name=\"analysis_history\" value=\"");
                    HTMLString.Append(iMatchCnt.ToString());
                    HTMLString.Append("\"></td><td><input type=\"checkbox\" name=\"analysis_players\" value=\"");
                    HTMLString.Append(iMatchCnt.ToString());
                    HTMLString.Append("\"></td><td><input type=\"checkbox\" name=\"analysis_remarks\" value=\"");
                    HTMLString.Append(iMatchCnt.ToString());
                    HTMLString.Append("\"></td><td><input type=\"checkbox\" name=\"analysis_recent\" value=\"");
                    HTMLString.Append(iMatchCnt.ToString());
                    HTMLString.Append("\"></td><td><input type=\"checkbox\" name=\"allitems\" value=\"1\"  onClick=\"selectAllItems(");
                    HTMLString.Append(iRecordsInPage.ToString());
                    HTMLString.Append(")\">所有項目</td></tr>");
                    iRecordsInPage++;
                }
                m_SportsOleReaderFb.Close();
                m_SportsDBMgrFb.Close();
                m_Title = "(" + sDayCode + ")" + "發送分析";
            }
            catch (Exception ex)
            {
                m_SportsLog.FilePath = ConfigurationManager.AppSettings["errlog"];
                m_SportsLog.SetFileName(0, LOGFILESUFFIX);
                m_SportsLog.Open();
                m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " AnalysisPreview.cs.PreviewMatches(): " + ex.ToString());
                m_SportsLog.Close();
                HTMLString.Remove(0, HTMLString.Length);
                HTMLString.Append((string)HttpContext.Current.Application["accessErrorMsg"]);
            }

            return HTMLString.ToString();
        }

        public int Send()
        {
            int iRecUpd = 0, iIdx = 0, iRecordCount = 0, iRecordIdx = 0;
            int iBgCnt = 0;
            int iHistoryCnt = 0;
            int iPlayerCnt = 0;
            int iRemarksCnt = 0;
            int iRecentCnt = 0;
            string sAction = "";
            string sCurrentTimestamp = null;
            string sBatchJob = null;
            char[] delimiter = new char[] { ',' };
            string[] arrLeague, arrHost, arrGuest, arrMatchCnt, arrAction, arrLeagType, arrBGChecked, arrHistoryChecked, arrPlayerChecked, arrRemarkChecked, arrRecentChecked;
            string[] arrSendToPager;
            string[] arrRemotingPath;
            ArrayList matchCnt_AL = new ArrayList();
            //SportsMessage object message
            SportsMessage sptMsg = new SportsMessage();
            StringBuilder LogSQLString = new StringBuilder();
            DBManager logDBMgr = new DBManager();
            logDBMgr.ConnectionString = m_SportsDBMgr.ConnectionString;
            arrRemotingPath = (string[])HttpContext.Current.Application["RemotingItems"];

            try
            {
                arrSendToPager = HttpContext.Current.Request.Form["SendToPager"].Split(delimiter);
            }
            catch (Exception)
            {
                arrSendToPager = new string[0];
            }
            arrLeague = HttpContext.Current.Request.Form["league"].Split(delimiter);
            arrHost = HttpContext.Current.Request.Form["host"].Split(delimiter);
            arrGuest = HttpContext.Current.Request.Form["guest"].Split(delimiter);
            arrMatchCnt = HttpContext.Current.Request.Form["matchcount"].Split(delimiter);
            arrAction = HttpContext.Current.Request.Form["Action"].Split(delimiter);
            arrLeagType = HttpContext.Current.Request.Form["leaguetype"].Split(delimiter);
            try
            {
                arrBGChecked = HttpContext.Current.Request.Form["analysis_bg"].Split(delimiter);
            }
            catch (Exception)
            {
                arrBGChecked = new string[0];
            }
            try
            {
                arrHistoryChecked = HttpContext.Current.Request.Form["analysis_history"].Split(delimiter);
            }
            catch (Exception)
            {
                arrHistoryChecked = new string[0];
            }
            try
            {
                arrPlayerChecked = HttpContext.Current.Request.Form["analysis_players"].Split(delimiter);
            }
            catch (Exception)
            {
                arrPlayerChecked = new string[0];
            }
            try
            {
                arrRemarkChecked = HttpContext.Current.Request.Form["analysis_remarks"].Split(delimiter);
            }
            catch (Exception)
            {
                arrRemarkChecked = new string[0];
            }
            try
            {
                arrRecentChecked = HttpContext.Current.Request.Form["analysis_recent"].Split(delimiter);
            }
            catch (Exception)
            {
                arrRecentChecked = new string[0];
            }

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


                //init INI file
                string sINIValue = "";
                string[] arrMsgType;
                arrMsgType = (string[])HttpContext.Current.Application["messageType"];

                //copy array element into ArrayList
                iRecordCount = arrMatchCnt.Length;
                for (iIdx = 0; iIdx < iRecordCount; iIdx++)
                {
                    matchCnt_AL.Add(arrMatchCnt[iIdx]);
                }
                matchCnt_AL.TrimToSize();

                /******************************
					Send background information
				 ******************************/
                if (arrSendToPager.Length > 0 && arrBGChecked.Length > 0)
                {
                    string sRoot = "city";
                    string[] arrWeather;
                    arrWeather = (string[])HttpContext.Current.Application["weatherItemsArray"];

                    sCurrentTimestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                    sBatchJob = DateTime.Now.ToString("yyMMddHHmmss_fffffff") + "." + HttpContext.Current.Session["user_name"].ToString() + "." + arrMsgType[7] + ".ini";
                    for (iIdx = 0; iIdx < arrBGChecked.Length; iIdx++)
                    {
                        iRecordIdx = matchCnt_AL.IndexOf(arrBGChecked[iIdx]);
                        sAction = arrAction[iRecordIdx];

                        if (arrLeagType[iRecordIdx].Equals("1")) sRoot = "city";
                        else if (arrLeagType[iRecordIdx].Equals("2")) sRoot = "country";
                        else sRoot = "continent";

                        SQLString.Remove(0, SQLString.Length);
                        SQLString.Append("select game.matchdate, game.field , game.host_handi, (select ");
                        SQLString.Append(sRoot);
                        SQLString.Append(" from teaminfo where teamname=game.host), (select ");
                        SQLString.Append(sRoot);
                        SQLString.Append(" from teaminfo where teamname=game.guest), anlybg.CMATCH_VENUE, anlybg.CTEMPERATURE, anlybg.IWEATHER_STATUS from gameinfo game, analysis_bg_info anlybg where game.match_cnt=anlybg.imatch_cnt and anlybg.imatch_cnt=");
                        SQLString.Append(arrBGChecked[iIdx]);
                        m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
                        if (m_SportsOleReader.Read())
                        {
                            //Insert log into LOG_ANALYSISBG
                            LogSQLString.Remove(0, LogSQLString.Length);
                            LogSQLString.Append("insert into LOG_ANALYSISBG (TIMEFLAG, SECTION, ACT, MATCHDATE, LEAGUE, HOST, GUEST, MATCHFIELD, HANDICAP, HOSTROOT, GUESTROOT, VENUE, TEMPERATURE, WEATHERSTATUS, BATCHJOB) values ('");
                            LogSQLString.Append(sCurrentTimestamp);
                            LogSQLString.Append("','ANALYSISBG_','");
                            LogSQLString.Append(sAction);
                            LogSQLString.Append("','");
                            LogSQLString.Append(m_SportsOleReader.GetString(0).Trim());
                            LogSQLString.Append("','");
                            LogSQLString.Append(arrLeague[iRecordIdx]);
                            LogSQLString.Append("','");
                            LogSQLString.Append(arrHost[iRecordIdx]);
                            LogSQLString.Append("','");
                            LogSQLString.Append(arrGuest[iRecordIdx]);
                            LogSQLString.Append("','");
                            LogSQLString.Append(m_SportsOleReader.GetString(1).Trim());
                            LogSQLString.Append("','");
                            LogSQLString.Append(m_SportsOleReader.GetString(2).Trim());
                            LogSQLString.Append("','");
                            if (m_SportsOleReader.IsDBNull(3))
                            {
                                sINIValue = "-1";
                            }
                            else
                            {
                                sINIValue = m_SportsOleReader.GetString(3).Trim();
                                if (sINIValue.Equals("")) sINIValue = "-1";
                            }
                            LogSQLString.Append(sINIValue);
                            LogSQLString.Append("','");
                            if (m_SportsOleReader.IsDBNull(4))
                            {
                                sINIValue = "-1";
                            }
                            else
                            {
                                sINIValue = m_SportsOleReader.GetString(4).Trim();
                                if (sINIValue.Equals("")) sINIValue = "-1";
                            }
                            LogSQLString.Append(sINIValue);
                            LogSQLString.Append("','");
                            if (m_SportsOleReader.IsDBNull(5))
                            {
                                sINIValue = "-1";
                            }
                            else
                            {
                                sINIValue = m_SportsOleReader.GetString(5).Trim();
                                if (sINIValue.Equals("")) sINIValue = "-1";
                            }
                            LogSQLString.Append(sINIValue);
                            LogSQLString.Append("','");
                            if (m_SportsOleReader.IsDBNull(6))
                            {
                                sINIValue = "-9999";
                            }
                            else
                            {
                                sINIValue = m_SportsOleReader.GetString(6).Trim();
                                if (sINIValue.Equals("")) sINIValue = "-9999";
                            }
                            LogSQLString.Append(sINIValue);
                            LogSQLString.Append("','");
                            if (sINIValue.Equals("-9999")) LogSQLString.Append("-1");
                            else LogSQLString.Append(arrWeather[m_SportsOleReader.GetInt32(7)]);
                            LogSQLString.Append("','");
                            LogSQLString.Append(sBatchJob);
                            LogSQLString.Append("')");
                            logDBMgr.ExecuteNonQuery(LogSQLString.ToString());
                            logDBMgr.Close();
                            iBgCnt++;
                        }
                        m_SportsOleReader.Close();
                        m_SportsDBMgr.Close();

                        //delete from ANALYSIS_BG_INFO, ANALYSIS_HISTORY_INFO, ANALYSIS_REMARK_INFO if required
                        if (sAction.Equals("D"))
                        {
                            SQLString.Remove(0, SQLString.Length);
                            SQLString.Append("update ANALYSIS_BG_INFO set CACT='D' where imatch_cnt=");
                            SQLString.Append(arrBGChecked[iIdx]);
                            m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
                            m_SportsDBMgr.Close();

                            SQLString.Remove(0, SQLString.Length);
                            SQLString.Append("update ANALYSIS_HISTORY_INFO set CACT='D' where imatch_cnt=");
                            SQLString.Append(arrBGChecked[iIdx]);
                            m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
                            m_SportsDBMgr.Close();

                            SQLString.Remove(0, SQLString.Length);
                            SQLString.Append("update ANALYSIS_REMARK_INFO set CACT='D' where imatch_cnt=");
                            SQLString.Append(arrBGChecked[iIdx]);
                            m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
                            m_SportsDBMgr.Close();
                        }
                    }


                    //Modified by Henry, 10 Feb 2004
                    //Send Notify Message
                    sptMsg.Body = sBatchJob;
                    sptMsg.Timestamp = DateTime.Now.ToString("yyMMddHHmmss");
                    sptMsg.AppID = "07";
                    sptMsg.MsgID = "07";
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
                    // catch (System.Messaging.MessageQueueException mqEx)
                    catch (InvalidOperationException mqEx)
                    {
                        try
                        {
                            m_SportsLog.FilePath = ConfigurationManager.AppSettings["errlog"];
                            m_SportsLog.SetFileName(0, LOGFILESUFFIX);
                            m_SportsLog.Open();
                            m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " MSMQ ERROR: Analysis Preview");
                            m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " AnalysisPreview.cs.Send(): Notify via MSMQ throws MessageQueueException:  " + mqEx.ToString());
                            m_SportsLog.Close();

                            //If MSMQ fail, notify via .NET Remoting
                            msgClt.MessageType = arrMessageTypes[1];
                            msgClt.MessagePath = arrRemotingPath[0];
                            if (!msgClt.SendMessage((object)sptMsg))
                            {
                                m_SportsLog.FilePath = ConfigurationManager.AppSettings["errlog"];
                                m_SportsLog.SetFileName(0, LOGFILESUFFIX);
                                m_SportsLog.Open();
                                m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Remoting ERROR: Analysis Preview");
                                m_SportsLog.Close();
                            }
                        }
                        catch (Exception ex)
                        {
                            m_SportsLog.FilePath = ConfigurationManager.AppSettings["errlog"];
                            m_SportsLog.SetFileName(0, LOGFILESUFFIX);
                            m_SportsLog.Open();
                            m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Remoting ERROR: Analysis Preview");
                            m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " AnalysisPreview.cs.Send(): Notify via .NET Remoting throws exception: " + ex.ToString());
                            m_SportsLog.Close();
                        }
                    }
                    catch (Exception ex)
                    {
                        m_SportsLog.FilePath = ConfigurationManager.AppSettings["errlog"];
                        m_SportsLog.SetFileName(0, LOGFILESUFFIX);
                        m_SportsLog.Open();
                        m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " AnalysisPreview.cs.Send(): Notify via MSMQ throws exception: " + ex.ToString());
                        m_SportsLog.Close();
                    }
                    //Modified end

                    //write log
                    m_SportsLog.FilePath = ConfigurationManager.AppSettings["eventlog"];
                    m_SportsLog.SetFileName(0, LOGFILESUFFIX);
                    m_SportsLog.Open();
                    m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " AnalysisPreview.cs: Send " + iBgCnt.ToString() + " background information (" + HttpContext.Current.Session["user_name"] + ")");
                    m_SportsLog.Close();
                }

                /***************************
					Send history information
				 ***************************/
                if (arrSendToPager.Length > 0 && arrHistoryChecked.Length > 0)
                {
                    int iMonth = 0, iYear = 0, iHistoryIdx = 0, iRecords = 0;
                    string sMatchDate = "";
                    string[] arrFields;
                    arrFields = (string[])HttpContext.Current.Application["fieldItemsArray"];

                    sCurrentTimestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                    sBatchJob = DateTime.Now.ToString("yyMMddHHmmss_fffffff") + "." + HttpContext.Current.Session["user_name"].ToString() + "." + arrMsgType[8] + ".ini";
                    for (iIdx = 0; iIdx < arrHistoryChecked.Length; iIdx++)
                    {
                        iHistoryIdx = 0;
                        iRecordIdx = matchCnt_AL.IndexOf(arrHistoryChecked[iIdx]);
                        sAction = arrAction[iRecordIdx];

                        SQLString.Remove(0, SQLString.Length);
                        SQLString.Append("select count(his.imatch_cnt) from GAMEINFO game, ANALYSIS_HISTORY_INFO his where game.match_cnt=his.imatch_cnt and game.match_cnt= ");
                        SQLString.Append(arrHistoryChecked[iIdx]);
                        iRecords = m_SportsDBMgr.ExecuteScalar(SQLString.ToString());
                        m_SportsDBMgr.Close();
                        if (iRecords > 0)
                        {   //Insert into LOG_ANALYSISHISTORY if related history existed
                            if (sAction.Equals("D"))
                            {
                                LogSQLString.Remove(0, LogSQLString.Length);
                                LogSQLString.Append("insert into LOG_ANALYSISHISTORY (TIMEFLAG, IITEMSEQ_NO, SECTION, ACT, LEAGUE, HOST, GUEST, MATCHDATE, LEAGUEALIAS, MATCHFIELD, HOSTSCORE, GUESTSCORE, BATCHJOB) values ('");
                                LogSQLString.Append(sCurrentTimestamp);
                                LogSQLString.Append("',");
                                LogSQLString.Append(iHistoryIdx.ToString());
                                LogSQLString.Append(",'ANALYSISHISTORY_','D','");
                                LogSQLString.Append(arrLeague[iRecordIdx]);
                                LogSQLString.Append("','");
                                LogSQLString.Append(arrHost[iRecordIdx]);
                                LogSQLString.Append("','");
                                LogSQLString.Append(arrGuest[iRecordIdx]);
                                LogSQLString.Append("',null,null,null,null,null,'");
                                LogSQLString.Append(sBatchJob);
                                LogSQLString.Append("')");
                                logDBMgr.ExecuteNonQuery(LogSQLString.ToString());
                                logDBMgr.Close();

                                SQLString.Remove(0, SQLString.Length);
                                SQLString.Append("update ANALYSIS_HISTORY_INFO set CACT='D' where imatch_cnt=");
                                SQLString.Append(arrHistoryChecked[iIdx]);
                                m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
                                m_SportsDBMgr.Close();
                            }
                            else
                            {
                                SQLString.Remove(0, SQLString.Length);
                                SQLString.Append("select his.IMATCHMONTH, his.IMATCHYEAR, his.CLEAGUEALIAS, his.IMATCHSTATUS, his.IHOSTSCORE, his.IGUESTSCORE from GAMEINFO game, ANALYSIS_HISTORY_INFO his where game.match_cnt=his.imatch_cnt and game.match_cnt= ");
                                SQLString.Append(arrHistoryChecked[iIdx]);
                                SQLString.Append(" order by his.IREC");
                                m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
                                while (m_SportsOleReader.Read())
                                {
                                    iHistoryIdx++;
                                    iMonth = m_SportsOleReader.GetInt32(0);
                                    iYear = m_SportsOleReader.GetInt32(1);
                                    sMatchDate = iYear.ToString();
                                    if (iMonth < 10) sMatchDate += "0" + iMonth.ToString();
                                    else sMatchDate += iMonth.ToString();

                                    LogSQLString.Remove(0, LogSQLString.Length);
                                    LogSQLString.Append("insert into LOG_ANALYSISHISTORY (TIMEFLAG, IITEMSEQ_NO, SECTION, ACT, LEAGUE, HOST, GUEST, MATCHDATE, LEAGUEALIAS, MATCHFIELD, HOSTSCORE, GUESTSCORE, BATCHJOB) values ('");
                                    LogSQLString.Append(sCurrentTimestamp);
                                    LogSQLString.Append("',");
                                    LogSQLString.Append(iHistoryIdx.ToString());
                                    LogSQLString.Append(",'ANALYSISHISTORY_','U','");
                                    LogSQLString.Append(arrLeague[iRecordIdx]);
                                    LogSQLString.Append("','");
                                    LogSQLString.Append(arrHost[iRecordIdx]);
                                    LogSQLString.Append("','");
                                    LogSQLString.Append(arrGuest[iRecordIdx]);
                                    LogSQLString.Append("','");
                                    LogSQLString.Append(sMatchDate);
                                    LogSQLString.Append("','");
                                    LogSQLString.Append(m_SportsOleReader.GetString(2).Trim());
                                    LogSQLString.Append("','");
                                    LogSQLString.Append(arrFields[m_SportsOleReader.GetInt32(3)]);
                                    LogSQLString.Append("',");
                                    LogSQLString.Append(m_SportsOleReader.GetInt32(4).ToString());
                                    LogSQLString.Append(",");
                                    LogSQLString.Append(m_SportsOleReader.GetInt32(5).ToString());
                                    LogSQLString.Append(",'");
                                    LogSQLString.Append(sBatchJob);
                                    LogSQLString.Append("')");
                                    logDBMgr.ExecuteNonQuery(LogSQLString.ToString());
                                    logDBMgr.Close();
                                }
                                m_SportsOleReader.Close();
                                m_SportsDBMgr.Close();
                            }
                            iHistoryCnt++;
                        }
                    }

                    if (iHistoryCnt > 0)
                    {
                        //Send Notify Message						
                        //Modified by Henry, 10 Feb 2004	
                        sptMsg.Body = sBatchJob;
                        sptMsg.Timestamp = DateTime.Now.ToString("yyMMddHHmmss");
                        sptMsg.AppID = "07";
                        sptMsg.MsgID = "08";
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
                        // catch (System.Messaging.MessageQueueException mqEx)
                        catch (InvalidOperationException mqEx)
                        {
                            try
                            {
                                m_SportsLog.FilePath = ConfigurationManager.AppSettings["errlog"];
                                m_SportsLog.SetFileName(0, LOGFILESUFFIX);
                                m_SportsLog.Open();
                                m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " MSMQ ERROR: Analysis Preview");
                                m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " AnalysisPreview.cs.Send(): Notify via MSMQ throws MessageQueueException:  " + mqEx.ToString());
                                m_SportsLog.Close();

                                //If MSMQ fail, notify via .NET Remoting
                                msgClt.MessageType = arrMessageTypes[1];
                                msgClt.MessagePath = arrRemotingPath[0];
                                if (!msgClt.SendMessage((object)sptMsg))
                                {
                                    m_SportsLog.FilePath = ConfigurationManager.AppSettings["errlog"];
                                    m_SportsLog.SetFileName(0, LOGFILESUFFIX);
                                    m_SportsLog.Open();
                                    m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Remoting ERROR: Analysis Preview");
                                    m_SportsLog.Close();
                                }
                            }
                            catch (Exception ex)
                            {
                                m_SportsLog.FilePath = ConfigurationManager.AppSettings["errlog"];
                                m_SportsLog.SetFileName(0, LOGFILESUFFIX);
                                m_SportsLog.Open();
                                m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Remoting ERROR: Analysis Preview");
                                m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " AnalysisPreview.cs.Send(): Notify via .NET Remoting throws exception: " + ex.ToString());
                                m_SportsLog.Close();
                            }
                        }
                        catch (Exception ex)
                        {
                            m_SportsLog.FilePath = ConfigurationManager.AppSettings["errlog"];
                            m_SportsLog.SetFileName(0, LOGFILESUFFIX);
                            m_SportsLog.Open();
                            m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " AnalysisPreview.cs.Send(): Notify via MSMQ throws exception: " + ex.ToString());
                            m_SportsLog.Close();
                        }
                        //Modified end					

                    }

                    //write log
                    m_SportsLog.FilePath = ConfigurationManager.AppSettings["eventlog"];
                    m_SportsLog.SetFileName(0, LOGFILESUFFIX);
                    m_SportsLog.Open();
                    m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " AnalysisPreview.cs: Send " + iHistoryCnt.ToString() + " history (" + HttpContext.Current.Session["user_name"] + ")");
                    m_SportsLog.Close();
                }

                /***************************
					Send players information
				 ***************************/
                if (arrSendToPager.Length > 0 && arrPlayerChecked.Length > 0)
                {
                    int iPlayerSubIdx = 0, iPlayerNo = 0, iHostCount = 0, iGuestCount = 0;
                    string sTeam = "";
                    string[] arrPos;
                    arrPos = (string[])HttpContext.Current.Application["positionItemsArray"];

                    sCurrentTimestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                    sBatchJob = DateTime.Now.ToString("yyMMddHHmmss_fffffff") + "." + HttpContext.Current.Session["user_name"].ToString() + "." + arrMsgType[9] + ".ini";
                    for (iIdx = 0; iIdx < arrPlayerChecked.Length; iIdx++)
                    {
                        iRecordIdx = matchCnt_AL.IndexOf(arrPlayerChecked[iIdx]);
                        //check for Action
                        sAction = arrAction[iRecordIdx];

                        sTeam = "HOST";
                        SQLString.Remove(0, SQLString.Length);
                        SQLString.Append("select count(play.CPLAYER_NAME) from GAMEINFO game, PLAYERS_INFO play where play.IROSTER=1 and game.match_cnt=");
                        SQLString.Append(arrPlayerChecked[iIdx]);
                        SQLString.Append(" and play.TEAM_ID=(select TEAM_ID from teaminfo where TEAMNAME=game.");
                        SQLString.Append(sTeam);
                        SQLString.Append(")");
                        iHostCount = m_SportsDBMgr.ExecuteScalar(SQLString.ToString());
                        m_SportsDBMgr.Close();

                        sTeam = "GUEST";
                        SQLString.Remove(0, SQLString.Length);
                        SQLString.Append("select count(play.CPLAYER_NAME) from GAMEINFO game, PLAYERS_INFO play where play.IROSTER=1 and game.match_cnt=");
                        SQLString.Append(arrPlayerChecked[iIdx]);
                        SQLString.Append(" and play.TEAM_ID=(select TEAM_ID from teaminfo where TEAMNAME=game.");
                        SQLString.Append(sTeam);
                        SQLString.Append(")");
                        iGuestCount = m_SportsDBMgr.ExecuteScalar(SQLString.ToString());
                        m_SportsDBMgr.Close();

                        iPlayerSubIdx = 0;
                        if (sAction.Equals("D"))
                        {
                            iPlayerCnt++;
                            iPlayerSubIdx++;
                            LogSQLString.Remove(0, LogSQLString.Length);
                            LogSQLString.Append("insert into LOG_ANALYSISPLAYERS (TIMEFLAG, IITEMSEQ_NO, SECTION, ACT, LEAGUE, HOST, GUEST, PLAYERTYPE, PLAYERNAME, PLAYERNO, PLAYERPOS, BATCHJOB) values ('");
                            LogSQLString.Append(sCurrentTimestamp);
                            LogSQLString.Append("',");
                            LogSQLString.Append(iPlayerSubIdx.ToString());
                            LogSQLString.Append(",'ANALYSISPLAYERS_','D','");
                            LogSQLString.Append(arrLeague[iRecordIdx]);
                            LogSQLString.Append("','");
                            LogSQLString.Append(arrHost[iRecordIdx]);
                            LogSQLString.Append("','");
                            LogSQLString.Append(arrGuest[iRecordIdx]);
                            LogSQLString.Append("','H',null,null,null,'");
                            LogSQLString.Append(sBatchJob);
                            LogSQLString.Append("')");
                            logDBMgr.ExecuteNonQuery(LogSQLString.ToString());
                            logDBMgr.Close();
                        }
                        else
                        {
                            if (iHostCount > 0 || iGuestCount > 0)
                            {
                                iPlayerCnt++;
                                //host player retrieval
                                sTeam = "HOST";
                                SQLString.Remove(0, SQLString.Length);
                                SQLString.Append("select play.CPLAYER_NAME, play.IPLAYER_NO, play.IPOS ");
                                SQLString.Append("from GAMEINFO game, PLAYERS_INFO play ");
                                SQLString.Append("where play.IROSTER=1 and game.match_cnt=");
                                SQLString.Append(arrPlayerChecked[iIdx]);
                                SQLString.Append(" and play.TEAM_ID=(select TEAM_ID from teaminfo where TEAMNAME=game.");
                                SQLString.Append(sTeam);
                                SQLString.Append(") order by IPOS");
                                m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
                                while (m_SportsOleReader.Read())
                                {
                                    iPlayerSubIdx++;
                                    LogSQLString.Remove(0, LogSQLString.Length);
                                    LogSQLString.Append("insert into LOG_ANALYSISPLAYERS (TIMEFLAG, IITEMSEQ_NO, SECTION, ACT, LEAGUE, HOST, GUEST, PLAYERTYPE, PLAYERNAME, PLAYERNO, PLAYERPOS, BATCHJOB) values ('");
                                    LogSQLString.Append(sCurrentTimestamp);
                                    LogSQLString.Append("',");
                                    LogSQLString.Append(iPlayerSubIdx.ToString());
                                    LogSQLString.Append(",'ANALYSISPLAYERS_','U','");
                                    LogSQLString.Append(arrLeague[iRecordIdx]);
                                    LogSQLString.Append("','");
                                    LogSQLString.Append(arrHost[iRecordIdx]);
                                    LogSQLString.Append("','");
                                    LogSQLString.Append(arrGuest[iRecordIdx]);
                                    LogSQLString.Append("','H','");
                                    LogSQLString.Append(m_SportsOleReader.GetString(0).Trim());
                                    LogSQLString.Append("',");
                                    if (m_SportsOleReader.IsDBNull(1)) iPlayerNo = -1;
                                    else iPlayerNo = m_SportsOleReader.GetInt32(1);
                                    LogSQLString.Append(iPlayerNo.ToString());
                                    LogSQLString.Append(",'");
                                    LogSQLString.Append(arrPos[m_SportsOleReader.GetInt32(2)]);
                                    LogSQLString.Append("','");
                                    LogSQLString.Append(sBatchJob);
                                    LogSQLString.Append("')");
                                    logDBMgr.ExecuteNonQuery(LogSQLString.ToString());
                                    logDBMgr.Close();
                                }
                                m_SportsOleReader.Close();
                                m_SportsDBMgr.Close();

                                //guest player retrieval
                                iPlayerSubIdx = 0;
                                sTeam = "GUEST";
                                SQLString.Remove(0, SQLString.Length);
                                SQLString.Append("select play.CPLAYER_NAME, play.IPLAYER_NO, play.IPOS ");
                                SQLString.Append("from GAMEINFO game, PLAYERS_INFO play ");
                                SQLString.Append("where play.IROSTER=1 and game.match_cnt=");
                                SQLString.Append(arrPlayerChecked[iIdx]);
                                SQLString.Append(" and play.TEAM_ID=(select TEAM_ID from teaminfo where TEAMNAME=game.");
                                SQLString.Append(sTeam);
                                SQLString.Append(") order by IPOS");
                                m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
                                while (m_SportsOleReader.Read())
                                {
                                    iPlayerSubIdx++;
                                    LogSQLString.Remove(0, LogSQLString.Length);
                                    LogSQLString.Append("insert into LOG_ANALYSISPLAYERS (TIMEFLAG, IITEMSEQ_NO, SECTION, ACT, LEAGUE, HOST, GUEST, PLAYERTYPE, PLAYERNAME, PLAYERNO, PLAYERPOS, BATCHJOB) values ('");
                                    LogSQLString.Append(sCurrentTimestamp);
                                    LogSQLString.Append("',");
                                    LogSQLString.Append(iPlayerSubIdx.ToString());
                                    LogSQLString.Append(",'ANALYSISPLAYERS_','U','");
                                    LogSQLString.Append(arrLeague[iRecordIdx]);
                                    LogSQLString.Append("','");
                                    LogSQLString.Append(arrHost[iRecordIdx]);
                                    LogSQLString.Append("','");
                                    LogSQLString.Append(arrGuest[iRecordIdx]);
                                    LogSQLString.Append("','G','");
                                    LogSQLString.Append(m_SportsOleReader.GetString(0).Trim());
                                    LogSQLString.Append("',");
                                    if (m_SportsOleReader.IsDBNull(1)) iPlayerNo = -1;
                                    else iPlayerNo = m_SportsOleReader.GetInt32(1);
                                    LogSQLString.Append(iPlayerNo.ToString());
                                    LogSQLString.Append(",'");
                                    LogSQLString.Append(arrPos[m_SportsOleReader.GetInt32(2)]);
                                    LogSQLString.Append("','");
                                    LogSQLString.Append(sBatchJob);
                                    LogSQLString.Append("')");
                                    logDBMgr.ExecuteNonQuery(LogSQLString.ToString());
                                    logDBMgr.Close();
                                }
                                m_SportsOleReader.Close();
                                m_SportsDBMgr.Close();
                            }
                        }
                    }

                    if (iPlayerCnt > 0)
                    {
                        //Send Notify Message
                        //Modified by Henry, 10 Feb 2004
                        sptMsg.Body = sBatchJob;
                        sptMsg.Timestamp = DateTime.Now.ToString("yyMMddHHmmss");
                        sptMsg.AppID = "07";
                        sptMsg.MsgID = "09";
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
                        // catch (System.Messaging.MessageQueueException mqEx)
                        catch (InvalidOperationException mqEx)
                        {
                            try
                            {
                                m_SportsLog.FilePath = ConfigurationManager.AppSettings["errlog"];
                                m_SportsLog.SetFileName(0, LOGFILESUFFIX);
                                m_SportsLog.Open();
                                m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " MSMQ ERROR: Analysis Preview");
                                m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " AnalysisPreview.cs.Send(): Notify via MSMQ throws MessageQueueException:  " + mqEx.ToString());
                                m_SportsLog.Close();

                                //If MSMQ fail, notify via .NET Remoting
                                msgClt.MessageType = arrMessageTypes[1];
                                msgClt.MessagePath = arrRemotingPath[0];
                                if (!msgClt.SendMessage((object)sptMsg))
                                {
                                    m_SportsLog.FilePath = ConfigurationManager.AppSettings["errlog"];
                                    m_SportsLog.SetFileName(0, LOGFILESUFFIX);
                                    m_SportsLog.Open();
                                    m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Remoting ERROR: Analysis Preview");
                                    m_SportsLog.Close();
                                }
                            }
                            catch (Exception ex)
                            {
                                m_SportsLog.FilePath = ConfigurationManager.AppSettings["errlog"];
                                m_SportsLog.SetFileName(0, LOGFILESUFFIX);
                                m_SportsLog.Open();
                                m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Remoting ERROR: Analysis Preview");
                                m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " AnalysisPreview.cs.Send(): Notify via .NET Remoting throws exception: " + ex.ToString());
                                m_SportsLog.Close();
                            }
                        }
                        catch (Exception ex)
                        {
                            m_SportsLog.FilePath = ConfigurationManager.AppSettings["errlog"];
                            m_SportsLog.SetFileName(0, LOGFILESUFFIX);
                            m_SportsLog.Open();
                            m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " AnalysisPreview.cs.Send(): Notify via MSMQ throws exception: " + ex.ToString());
                            m_SportsLog.Close();
                        }
                        //Modified end	

                    }

                    //write log
                    m_SportsLog.FilePath = ConfigurationManager.AppSettings["eventlog"];
                    m_SportsLog.SetFileName(0, LOGFILESUFFIX);
                    m_SportsLog.Open();
                    m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " AnalysisPreview.cs: Send " + iPlayerCnt.ToString() + " players (" + HttpContext.Current.Session["user_name"] + ")");
                    m_SportsLog.Close();
                }

                /**************************************
					Send additional remarks information
				 **************************************/
                if (arrSendToPager.Length > 0 && arrRemarkChecked.Length > 0)
                {
                    sCurrentTimestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                    sBatchJob = DateTime.Now.ToString("yyMMddHHmmss_fffffff") + "." + HttpContext.Current.Session["user_name"].ToString() + "." + arrMsgType[10] + ".ini";
                    for (iIdx = 0; iIdx < arrRemarkChecked.Length; iIdx++)
                    {
                        iRecordIdx = matchCnt_AL.IndexOf(arrRemarkChecked[iIdx]);
                        //check for Action
                        sAction = arrAction[iRecordIdx];

                        SQLString.Remove(0, SQLString.Length);
                        SQLString.Append("select CREMARKS from ANALYSIS_REMARK_INFO where IMATCH_CNT=");
                        SQLString.Append(arrRemarkChecked[iIdx]);
                        m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
                        if (m_SportsOleReader.Read())
                        {
                            if (!m_SportsOleReader.IsDBNull(0))
                            {
                                if (!m_SportsOleReader.GetString(0).Trim().Equals("-1") && !m_SportsOleReader.GetString(0).Trim().Equals(""))
                                {
                                    LogSQLString.Remove(0, LogSQLString.Length);
                                    LogSQLString.Append("insert into LOG_ANALYSISREMARKS (TIMEFLAG, SECTION, ACT, LEAGUE, HOST, GUEST, REMARKS, BATCHJOB) values ('");
                                    LogSQLString.Append(sCurrentTimestamp);
                                    LogSQLString.Append("','ANALYSISREMARKS_','");
                                    LogSQLString.Append(sAction);
                                    LogSQLString.Append("','");
                                    LogSQLString.Append(arrLeague[iRecordIdx]);
                                    LogSQLString.Append("','");
                                    LogSQLString.Append(arrHost[iRecordIdx]);
                                    LogSQLString.Append("','");
                                    LogSQLString.Append(arrGuest[iRecordIdx]);
                                    LogSQLString.Append("','");
                                    LogSQLString.Append(m_SportsOleReader.GetString(0).Trim());
                                    LogSQLString.Append("','");
                                    LogSQLString.Append(sBatchJob);
                                    LogSQLString.Append("')");
                                    logDBMgr.ExecuteNonQuery(LogSQLString.ToString());
                                    logDBMgr.Close();
                                    iRemarksCnt++;
                                }
                            }
                        }
                        m_SportsOleReader.Close();
                        m_SportsDBMgr.Close();

                        //delete from ANALYSIS_REMARK_INFO if required
                        if (sAction.Equals("D"))
                        {
                            SQLString.Remove(0, SQLString.Length);
                            SQLString.Append("update ANALYSIS_REMARK_INFO set CACT='D' where imatch_cnt=");
                            SQLString.Append(arrRemarkChecked[iIdx]);
                            m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
                            m_SportsDBMgr.Close();
                        }
                    }

                    if (iRemarksCnt > 0)
                    {
                        //Send Notify Message
                        //Modified by Henry, 10 Feb 2004
                        sptMsg.Body = sBatchJob;
                        sptMsg.Timestamp = DateTime.Now.ToString("yyMMddHHmmss");
                        sptMsg.AppID = "07";
                        sptMsg.MsgID = "10";
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
                        // catch (System.Messaging.MessageQueueException mqEx)
                        catch (InvalidOperationException mqEx)
                        {
                            try
                            {
                                m_SportsLog.FilePath = ConfigurationManager.AppSettings["errlog"];
                                m_SportsLog.SetFileName(0, LOGFILESUFFIX);
                                m_SportsLog.Open();
                                m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " MSMQ ERROR: Analysis Preview");
                                m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " AnalysisPreview.cs.Send(): Notify via MSMQ throws MessageQueueException:  " + mqEx.ToString());
                                m_SportsLog.Close();

                                //If MSMQ fail, notify via .NET Remoting
                                msgClt.MessageType = arrMessageTypes[1];
                                msgClt.MessagePath = arrRemotingPath[0];
                                if (!msgClt.SendMessage((object)sptMsg))
                                {
                                    m_SportsLog.FilePath = ConfigurationManager.AppSettings["errlog"];
                                    m_SportsLog.SetFileName(0, LOGFILESUFFIX);
                                    m_SportsLog.Open();
                                    m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Remoting ERROR: Analysis Preview");
                                    m_SportsLog.Close();
                                }
                            }
                            catch (Exception ex)
                            {
                                m_SportsLog.FilePath = ConfigurationManager.AppSettings["errlog"];
                                m_SportsLog.SetFileName(0, LOGFILESUFFIX);
                                m_SportsLog.Open();
                                m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Remoting ERROR: Analysis Preview");
                                m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " AnalysisPreview.cs.Send(): Notify via .NET Remoting throws exception: " + ex.ToString());
                                m_SportsLog.Close();
                            }
                        }
                        catch (Exception ex)
                        {
                            m_SportsLog.FilePath = ConfigurationManager.AppSettings["errlog"];
                            m_SportsLog.SetFileName(0, LOGFILESUFFIX);
                            m_SportsLog.Open();
                            m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " AnalysisPreview.cs.Send(): Notify via MSMQ throws exception: " + ex.ToString());
                            m_SportsLog.Close();
                        }
                        //Modified end						
                    }

                    //write log
                    m_SportsLog.FilePath = ConfigurationManager.AppSettings["eventlog"];
                    m_SportsLog.SetFileName(0, LOGFILESUFFIX);
                    m_SportsLog.Open();
                    m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " AnalysisPreview.cs: Send " + iRemarksCnt.ToString() + " remarks (" + HttpContext.Current.Session["user_name"] + ")");
                    m_SportsLog.Close();
                }

                /**************************************
					Send analysis recent information
				 **************************************/
                if (arrSendToPager.Length > 0 && arrRecentChecked.Length > 0)
                {
                    sCurrentTimestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                    sBatchJob = DateTime.Now.ToString("yyMMddHHmmss_fffffff") + "." + HttpContext.Current.Session["user_name"].ToString() + "." + arrMsgType[17] + ".ini";
                    for (iIdx = 0; iIdx < arrRecentChecked.Length; iIdx++)
                    {
                        iRecordIdx = matchCnt_AL.IndexOf(arrRecentChecked[iIdx]);
                        //check for Action
                        sAction = arrAction[iRecordIdx];

                        //delete from ANALYSIS_RECENT_INFO if required
                        if (sAction.Equals("D"))
                        {
                            SQLString.Remove(0, SQLString.Length);
                            SQLString.Append("update ANALYSIS_RECENT_INFO set CACT='D' where imatch_cnt=");
                            SQLString.Append(arrRecentChecked[iIdx]);
                            m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
                            m_SportsDBMgr.Close();
                        }

                        SQLString.Remove(0, SQLString.Length);
                        SQLString.Append("select i.CACT, g.LEAGLONG, g.HOST, g.GUEST, g.MATCHDATE, g.MATCHTIME, g.FIELD, g.HOST_HANDI, i.CTEAMFLAG, i.CLEAGUEALIAS, i.CCHALLENGER, i.IMATCHSTATUS, i.IHOSTSCORE, i.IGUESTSCORE from ANALYSIS_RECENT_INFO i, GAMEINFO g where i.IMATCH_CNT=g.MATCH_CNT and i.IMATCH_CNT=");
                        SQLString.Append(arrRecentChecked[iIdx]);
                        SQLString.Append(" order by IREC");
                        m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
                        int iItemSeq_no = 0;
                        bool bChangedSeqNo = false;
                        while (m_SportsOleReader.Read())
                        {
                            if (!m_SportsOleReader.IsDBNull(0))
                            {
                                if (!m_SportsOleReader.GetString(9).Trim().Equals("") && !m_SportsOleReader.GetString(10).Trim().Equals(""))
                                {
                                    LogSQLString.Remove(0, LogSQLString.Length);
                                    LogSQLString.Append("insert into LOG_ANALYSISRECENT (TIMEFLAG, IITEMSEQ_NO, SECTION, ACT, LEAGUE, HOST, GUEST, MATCHDATE, MATCHTIME, MATCHFIELD, HOSTHANDICAP, RECENTTYPE, RECENTALIAS, RECENTCHALLENGER, RECENTFIELD, RECENTHOSTSCORE, RECENTGUESTSCORE, BATCHJOB) values ('");
                                    LogSQLString.Append(sCurrentTimestamp);
                                    if (m_SportsOleReader.GetString(8).Trim().Equals("G") && !bChangedSeqNo)
                                    {
                                        iItemSeq_no = 1;
                                        bChangedSeqNo = true;
                                    }
                                    else
                                    {
                                        iItemSeq_no++;
                                    }
                                    LogSQLString.Append("',");
                                    LogSQLString.Append(iItemSeq_no.ToString());
                                    LogSQLString.Append(",'ANALYSISRECENT_','");
                                    LogSQLString.Append(m_SportsOleReader.GetString(0).Trim());
                                    LogSQLString.Append("','");
                                    LogSQLString.Append(m_SportsOleReader.GetString(1).Trim());
                                    LogSQLString.Append("','");
                                    LogSQLString.Append(m_SportsOleReader.GetString(2).Trim());
                                    LogSQLString.Append("','");
                                    LogSQLString.Append(m_SportsOleReader.GetString(3).Trim());
                                    LogSQLString.Append("','");
                                    LogSQLString.Append(m_SportsOleReader.GetString(4).Trim());
                                    LogSQLString.Append("','");
                                    LogSQLString.Append(m_SportsOleReader.GetString(5).Trim());
                                    LogSQLString.Append("','");
                                    LogSQLString.Append(m_SportsOleReader.GetString(6).Trim());
                                    LogSQLString.Append("','");
                                    LogSQLString.Append(m_SportsOleReader.GetString(7).Trim());
                                    LogSQLString.Append("','");
                                    LogSQLString.Append(m_SportsOleReader.GetString(8).Trim());
                                    LogSQLString.Append("','");
                                    LogSQLString.Append(m_SportsOleReader.GetString(9).Trim());
                                    LogSQLString.Append("','");
                                    LogSQLString.Append(m_SportsOleReader.GetString(10).Trim());
                                    LogSQLString.Append("','");
                                    LogSQLString.Append(arrFields[m_SportsOleReader.GetInt32(11)]);
                                    LogSQLString.Append("',");
                                    LogSQLString.Append(m_SportsOleReader.GetInt32(12).ToString());
                                    LogSQLString.Append(",");
                                    LogSQLString.Append(m_SportsOleReader.GetInt32(13).ToString());
                                    LogSQLString.Append(",'");
                                    LogSQLString.Append(sBatchJob);
                                    LogSQLString.Append("')");
                                    logDBMgr.ExecuteNonQuery(LogSQLString.ToString());
                                    logDBMgr.Close();
                                }
                            }
                        }
                        iRecentCnt++;
                        m_SportsOleReader.Close();
                        m_SportsDBMgr.Close();
                    }

                    if (iRecentCnt > 0)
                    {
                        //Send Notify Message
                        //Modified by Henry, 10 Feb 2004
                        sptMsg.Body = sBatchJob;
                        sptMsg.Timestamp = DateTime.Now.ToString("yyMMddHHmmss");
                        sptMsg.AppID = "07";
                        sptMsg.MsgID = "12";
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
                        // catch (System.Messaging.MessageQueueException mqEx)
                        catch (InvalidOperationException mqEx)
                        {
                            try
                            {
                                m_SportsLog.FilePath = ConfigurationManager.AppSettings["errlog"];
                                m_SportsLog.SetFileName(0, LOGFILESUFFIX);
                                m_SportsLog.Open();
                                m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " MSMQ ERROR: Analysis Preview");
                                m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " AnalysisPreview.cs.Send(): Notify via MSMQ throws MessageQueueException:  " + mqEx.ToString());
                                m_SportsLog.Close();

                                //If MSMQ fail, notify via .NET Remoting
                                msgClt.MessageType = arrMessageTypes[1];
                                msgClt.MessagePath = arrRemotingPath[0];
                                if (!msgClt.SendMessage((object)sptMsg))
                                {
                                    m_SportsLog.FilePath = ConfigurationManager.AppSettings["errlog"];
                                    m_SportsLog.SetFileName(0, LOGFILESUFFIX);
                                    m_SportsLog.Open();
                                    m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Remoting ERROR: Analysis Preview");
                                    m_SportsLog.Close();
                                }
                            }
                            catch (Exception ex)
                            {
                                m_SportsLog.FilePath = ConfigurationManager.AppSettings["errlog"];
                                m_SportsLog.SetFileName(0, LOGFILESUFFIX);
                                m_SportsLog.Open();
                                m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Remoting ERROR: Analysis Preview");
                                m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " AnalysisPreview.cs.Send(): Notify via .NET Remoting throws exception: " + ex.ToString());
                                m_SportsLog.Close();
                            }
                        }
                        catch (Exception ex)
                        {
                            m_SportsLog.FilePath = ConfigurationManager.AppSettings["errlog"];
                            m_SportsLog.SetFileName(0, LOGFILESUFFIX);
                            m_SportsLog.Open();
                            m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " AnalysisPreview.cs.Send(): Notify via MSMQ throws exception: " + ex.ToString());
                            m_SportsLog.Close();
                        }
                        //Modified end						
                    }

                    //write log
                    m_SportsLog.FilePath = ConfigurationManager.AppSettings["eventlog"];
                    m_SportsLog.SetFileName(0, LOGFILESUFFIX);
                    m_SportsLog.Open();
                    m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " AnalysisPreview.cs: Send " + iRecentCnt.ToString() + " recent (" + HttpContext.Current.Session["user_name"] + ")");
                    m_SportsLog.Close();
                }
            }
            catch (Exception ex)
            {
                iRecUpd = -1;
                m_SportsLog.FilePath = ConfigurationManager.AppSettings["errlog"];
                m_SportsLog.SetFileName(0, LOGFILESUFFIX);
                m_SportsLog.Open();
                m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " AnalysisPreview.cs.Send(): " + ex.ToString());
                m_SportsLog.Close();
            }
            iRecUpd = iBgCnt + iHistoryCnt + iPlayerCnt + iRemarksCnt + iRecentCnt;
            return iRecUpd;
        }
    }
}