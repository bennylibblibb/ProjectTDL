/*
Objective:
Retrieval and send all matches statistics information for odds

Last updated:
19 Feb 2004, Chapman Choi
Add four additional field into LOG_ANALYSISSTAT (MATCHDATE, MATCHTIME, MATCHFIELD, HOSTHANDICAP)

C#.NET complier statement:
csc /t:library /out:..\bin\AnalysisStat.dll /r:..\bin\DBManager.dll;..\bin\Files.dll;..\bin\MessageClient.dll;..\bin\SportsMessage.dll AnalysisStat.cs
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

 
namespace SportsUtil {
	public class AnalysisStat {
		const string LOGFILESUFFIX = "log";
		int m_RecordsInPage = 0;
		OleDbDataReader m_SportsOleReader;
		DBManager m_SportsDBMgr;
		Files m_SportsLog;
		Encoding m_Big5Encoded;
		StringBuilder SQLString;

        public string m_Title = "";
        DBManagerFB m_SportsDBMgrFb;
        FbDataReader m_SportsOleReaderFb;

        public AnalysisStat(string Connection) {
			m_SportsDBMgr = new DBManager();
			m_SportsDBMgr.ConnectionString = Connection;
			m_SportsLog = new Files();
			m_Big5Encoded = Encoding.GetEncoding(950);
			SQLString = new StringBuilder();
            m_SportsDBMgrFb = new DBManagerFB();
            m_SportsDBMgrFb.ConnectionString = JC_SoccerWeb.Common.AppFlag.ScoutsDBConn;
        }

		public int NumberOfRecords {
			get {
				return m_RecordsInPage;
			}
		}

		public string GetStat() {
			string uid;
            string sEventID;
            string sDayCode="";
            StringBuilder HTMLString = new StringBuilder();

			try {
                sEventID = (HttpContext.Current.Request.QueryString["eventid"] == null) ? "" : HttpContext.Current.Request.QueryString["eventid"].Trim();

                bool bExisted = false;
				int iIdx = 1, iSubIdx = 0, iRecordCount = 0;
				ArrayList matchCntAL, leagueAL, hostAL, guestAL, anlyMatchCntAL, hostWinAL, hostDrawAL, hostLossAL, guestWinAL, guestDrawAL, guestLossAL;
				matchCntAL = new ArrayList();
				leagueAL = new ArrayList();
				hostAL = new ArrayList();
				guestAL = new ArrayList();
				hostWinAL = new ArrayList();
				hostDrawAL = new ArrayList();
				hostLossAL = new ArrayList();
				guestWinAL = new ArrayList();
				guestDrawAL = new ArrayList();
				guestLossAL = new ArrayList();
				anlyMatchCntAL = new ArrayList();

				uid = HttpContext.Current.Session["user_id"]==null?"3":HttpContext.Current.Session["user_id"].ToString();
				//retrieve league, host, guest and match count
				SQLString.Remove(0,SQLString.Length);
                ////SQLString.Append("select gameinfo.leaglong, gameinfo.host, gameinfo.guest, gameinfo.match_cnt from gameinfo, leaginfo where gameinfo.league=leaginfo.alias and gameinfo.act='U' and leaginfo.leag_id in (select cleag_id from userprofile_info where iuser_id=");
                ////SQLString.Append(uid);
                ////SQLString.Append(") order by leaginfo.leag_order, gameinfo.MATCHDATE, gameinfo.MATCHTIME");
                SQLString.Append("select E.CLEAGUE_OUTPUT_NAME,  E.HKJCHOSTNAME_CN, E.HKJCGUESTNAME_CN, E.EMATCHID,E.HKJCDAYCODE,E.HKJCMATCHNO from EMATCHES E ");
                //SQLString.Append(" WHERE E.HKJCDAYCODE = (SELECT HKJCDAYCODE FROM EMATCHES WHERE EMATCHID = '"+ sEventID + "'  ) ");
                //SQLString.Append(" AND cast(cast(e.CMATCHDATETIME as date) as varchar(10))= (SELECT cast(cast(CMATCHDATETIME as date) as varchar(10)) FROM EMATCHES WHERE EMATCHID = '"+ sEventID + "'  ) ");
                if (sEventID != ""&& sEventID!="-1" && sEventID != "0")
                {
                    SQLString.Append(" WHERE e.HKJCDAYCODE = (SELECT HKJCDAYCODE FROM EMATCHES WHERE EMATCHID =" + sEventID + "  ) and e.CMATCHDATETIME < (SELECT CMATCHDATETIME FROM EMATCHES WHERE EMATCHID =  " + sEventID + " )+1 and e.CMATCHDATETIME > (SELECT CMATCHDATETIME FROM EMATCHES WHERE EMATCHID =  " + sEventID + " ) -1 ");
                }
                else
                {
                    SQLString.Append("   WHERE e.HKJCDAYCODE = (SELECT first 1 HKJCDAYCODE FROM EMATCHES WHERE  cast(cast(CMATCHDATETIME as date) as varchar(10)) = cast(cast(current_timestamp as date) as varchar(10))   ) ");
                    SQLString.Append("  and e.CMATCHDATETIME < (SELECT first 1 CMATCHDATETIME FROM EMATCHES WHERE  cast(cast(CMATCHDATETIME as date) as varchar(10)) = cast(cast(current_timestamp as date) as varchar(10))   )+1 and e.CMATCHDATETIME > (SELECT  first 1 CMATCHDATETIME FROM EMATCHES WHERE  cast(cast(CMATCHDATETIME as date) as varchar(10)) = cast(cast(current_timestamp as date) as varchar(10))   ) -1 ");
                }
                m_SportsOleReaderFb = m_SportsDBMgrFb.ExecuteQuery(SQLString.ToString());
				while(m_SportsOleReaderFb.Read()) {
					leagueAL.Add(m_SportsOleReaderFb.GetString(0).Trim());
					hostAL.Add(m_SportsOleReaderFb.GetString(1).Trim());
					guestAL.Add(m_SportsOleReaderFb.GetString(2).Trim());
					matchCntAL.Add(m_SportsOleReaderFb.GetInt32(3));
                    sDayCode = m_SportsOleReaderFb.GetString(4);
                    iRecordCount++;
				}
                m_SportsDBMgrFb.Close();
                m_SportsOleReaderFb.Close();
				leagueAL.TrimToSize();
				hostAL.TrimToSize();
				guestAL.TrimToSize();
				matchCntAL.TrimToSize();
                m_Title ="("+ sDayCode + ")"+ "數據";
                //retrieve analysis stat
                if (iRecordCount>0) {
					SQLString.Remove(0,SQLString.Length);
					SQLString.Append("select IMATCH_CNT,IHOSTWIN,IHOSTDRAW,IHOSTLOSS,IGUESTWIN,IGUESTDRAW,IGUESTLOSS from ANALYSIS_STAT_INFO where CACT='U' and IMATCH_CNT in (");
					SQLString.Append(matchCntAL[0]);
					for(iIdx=1;iIdx<iRecordCount;iIdx++) {
						SQLString.Append(",");
						SQLString.Append(matchCntAL[iIdx]);
					}
					SQLString.Append(") order by IMATCH_CNT");
				}
				m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
				while(m_SportsOleReader.Read()) {
					anlyMatchCntAL.Add(m_SportsOleReader.GetInt32(0));
					hostWinAL.Add(m_SportsOleReader.GetInt32(1));
					hostDrawAL.Add(m_SportsOleReader.GetInt32(2));
					hostLossAL.Add(m_SportsOleReader.GetInt32(3));
					guestWinAL.Add(m_SportsOleReader.GetInt32(4));
					guestDrawAL.Add(m_SportsOleReader.GetInt32(5));
					guestLossAL.Add(m_SportsOleReader.GetInt32(6));
				}
				m_SportsDBMgr.Close();
				m_SportsOleReader.Close();
				anlyMatchCntAL.TrimToSize();
				hostWinAL.TrimToSize();
				hostDrawAL.TrimToSize();
				hostLossAL.TrimToSize();
				guestWinAL.TrimToSize();
				guestDrawAL.TrimToSize();
				guestLossAL.TrimToSize();

				m_RecordsInPage = iRecordCount;
				for(iIdx=0;iIdx<iRecordCount;iIdx++) {
					iSubIdx = anlyMatchCntAL.IndexOf(matchCntAL[iIdx]);
					if(iSubIdx > -1) {
						bExisted = true;
					}

					if(bExisted) HTMLString.Append("<tr align=\"center\" bgcolor=\"#FFB573\">");
					else HTMLString.Append("<tr align=\"center\">");
					HTMLString.Append("<th><input type=\"hidden\" name=\"matchcount\" value=\"");
					HTMLString.Append(matchCntAL[iIdx]);
					HTMLString.Append("\"><input type=\"hidden\" name=\"league\" value=\"");
					HTMLString.Append(leagueAL[iIdx]);
					HTMLString.Append("\"><b>");
					HTMLString.Append(leagueAL[iIdx]);
					HTMLString.Append("</b></th>");

					HTMLString.Append("<td><input type=\"hidden\" name=\"host\" value=\"");
					HTMLString.Append(hostAL[iIdx]);
					HTMLString.Append("\">");
					HTMLString.Append(hostAL[iIdx]);
					HTMLString.Append("</td><td><input type=\"text\" name=\"hostwin\" value=\"");
					if(bExisted) HTMLString.Append(hostWinAL[iSubIdx]);
					else HTMLString.Append("0");
					HTMLString.Append("\" size=\"1\" maxlength=\"3\" onChange=\"onHostWinChange(");
					HTMLString.Append(iIdx.ToString());
					HTMLString.Append(")\">%</td>");

					HTMLString.Append("<td><input type=\"text\" name=\"hostdraw\" value=\"");
					if(bExisted) HTMLString.Append(hostDrawAL[iSubIdx]);
					else HTMLString.Append("0");
					HTMLString.Append("\" size=\"1\" maxlength=\"3\" onChange=\"onHostDrawChange(");
					HTMLString.Append(iIdx.ToString());
					HTMLString.Append(")\">%</td>");

					HTMLString.Append("<td><input type=\"text\" name=\"hostloss\" value=\"");
					if(bExisted) HTMLString.Append(hostLossAL[iSubIdx]);
					else HTMLString.Append("0");
					HTMLString.Append("\" size=\"1\" maxlength=\"3\" onChange=\"onHostLossChange(");
					HTMLString.Append(iIdx.ToString());
					HTMLString.Append(")\">%</td>");

					HTMLString.Append("<td><input type=\"hidden\" name=\"guest\" value=\"");
					HTMLString.Append(guestAL[iIdx]);
					HTMLString.Append("\">");
					HTMLString.Append(guestAL[iIdx]);
					HTMLString.Append("</td><td><input type=\"text\" name=\"guestwin\" value=\"");
					if(bExisted) HTMLString.Append(guestWinAL[iSubIdx]);
					else HTMLString.Append("0");
					HTMLString.Append("\" size=\"1\" maxlength=\"3\" onChange=\"onGuestWinChange(");
					HTMLString.Append(iIdx.ToString());
					HTMLString.Append(")\">%</td>");

					HTMLString.Append("<td><input type=\"text\" name=\"guestdraw\" value=\"");
					if(bExisted) HTMLString.Append(guestDrawAL[iSubIdx]);
					else HTMLString.Append("0");
					HTMLString.Append("\" size=\"1\" maxlength=\"3\" onChange=\"onGuestDrawChange(");
					HTMLString.Append(iIdx.ToString());
					HTMLString.Append(")\">%</td>");

					HTMLString.Append("<td><input type=\"text\" name=\"guestloss\" value=\"");
					if(bExisted) HTMLString.Append(guestLossAL[iSubIdx]);
					else HTMLString.Append("0");
					HTMLString.Append("\" size=\"1\" maxlength=\"3\" onChange=\"onGuestLossChange(");
					HTMLString.Append(iIdx.ToString());
					HTMLString.Append(")\">%</td>");

					HTMLString.Append("<td><select name=\"Action\" onChange=\"actionChange(");
					HTMLString.Append(iIdx.ToString());
					HTMLString.Append(")\"><option value=\"U\">修改<option value=\"D\">刪除</select></td><td><input type=\"checkbox\" name=\"send\" value=\"");
					HTMLString.Append(matchCntAL[iIdx]);
					HTMLString.Append("\"></td></tr>");
					bExisted = false;
				}
			}	catch(Exception ex) {
				m_SportsLog.FilePath = ConfigurationManager.AppSettings["errlog"];
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " AnalysisStat.cs.GetStat(): " + ex.ToString());
				m_SportsLog.Close();
				HTMLString.Remove(0,HTMLString.Length);
				HTMLString.Append(ConfigurationManager.AppSettings["accessErrorMsg"]);
			} finally {
				m_SportsDBMgr.Dispose();
			}
			return HTMLString.ToString();
		}
		
		public int[] SaveRecord() {
			int iUpdCnt = 0, iExisted = 0, iIdx = 0, iRecordCount = 0;
			int[] arrRtn = new int[1];
			char[] delimiter = new char[] {','};
			string[] arrMatchCnt, arrLeague, arrHost, arrGuest, arrHostWin, arrHostDraw, arrHostLoss, arrGuestWin, arrGuestDraw, arrGuestLoss, arrAction;
			StringBuilder LogSQLString = new StringBuilder();
			DBManager logDBMgr = new DBManager();
			logDBMgr.ConnectionString = m_SportsDBMgr.ConnectionString;

			arrMatchCnt = HttpContext.Current.Request.Form["matchcount"].Split(delimiter);
			arrLeague = HttpContext.Current.Request.Form["league"].Split(delimiter);
			arrHost = HttpContext.Current.Request.Form["host"].Split(delimiter);
			arrGuest = HttpContext.Current.Request.Form["guest"].Split(delimiter);
			arrHostWin = HttpContext.Current.Request.Form["hostwin"].Split(delimiter);
			arrHostDraw = HttpContext.Current.Request.Form["hostdraw"].Split(delimiter);
			arrHostLoss = HttpContext.Current.Request.Form["hostloss"].Split(delimiter);
			arrGuestWin = HttpContext.Current.Request.Form["guestwin"].Split(delimiter);
			arrGuestDraw = HttpContext.Current.Request.Form["guestdraw"].Split(delimiter);
			arrGuestLoss = HttpContext.Current.Request.Form["guestloss"].Split(delimiter);
			arrAction = HttpContext.Current.Request.Form["Action"].Split(delimiter);

			try {
				iRecordCount = arrMatchCnt.Length;
				if (iRecordCount > 0) {
					for(iIdx=0; iIdx<iRecordCount; iIdx++) {
						//Update analysis stat info
						SQLString.Remove(0,SQLString.Length);
						SQLString.Append("select count(IMATCH_CNT) from ANALYSIS_STAT_INFO where IMATCH_CNT=");
						SQLString.Append(arrMatchCnt[iIdx]);
						iExisted = m_SportsDBMgr.ExecuteScalar(SQLString.ToString());
						m_SportsDBMgr.Close();
						SQLString.Remove(0,SQLString.Length);
						if (iExisted > 0) {	//update existing record
							SQLString.Append("update ANALYSIS_STAT_INFO set CACT='U',IHOSTWIN=");
							SQLString.Append(arrHostWin[iIdx]);
							SQLString.Append(",IHOSTDRAW=");
							SQLString.Append(arrHostDraw[iIdx]);
							SQLString.Append(",IHOSTLOSS=");
							SQLString.Append(arrHostLoss[iIdx]);
							SQLString.Append(",IGUESTWIN=");
							SQLString.Append(arrGuestWin[iIdx]);
							SQLString.Append(",IGUESTDRAW=");
							SQLString.Append(arrGuestDraw[iIdx]);
							SQLString.Append(",IGUESTLOSS=");
							SQLString.Append(arrGuestLoss[iIdx]);
							SQLString.Append(" where IMATCH_CNT=");
							SQLString.Append(arrMatchCnt[iIdx]);
						} else {	//insert a new record
							SQLString.Append("insert into ANALYSIS_STAT_INFO values(");
							SQLString.Append(arrMatchCnt[iIdx]);
							SQLString.Append(",'U',");
							SQLString.Append(arrHostWin[iIdx]);
							SQLString.Append(",");
							SQLString.Append(arrHostDraw[iIdx]);
							SQLString.Append(",");
							SQLString.Append(arrHostLoss[iIdx]);
							SQLString.Append(",");
							SQLString.Append(arrGuestWin[iIdx]);
							SQLString.Append(",");
							SQLString.Append(arrGuestDraw[iIdx]);
							SQLString.Append(",");
							SQLString.Append(arrGuestLoss[iIdx]);
							SQLString.Append(")");
						}
						if (arrHostWin[iIdx].Trim() != "0" || arrHostDraw[iIdx].Trim() != "0" || arrHostLoss[iIdx].Trim() != "0" || arrGuestWin[iIdx].Trim() != "0" || arrGuestDraw[iIdx].Trim() != "0" || arrGuestLoss[iIdx].Trim() != "0") {
							m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
							m_SportsDBMgr.Close();
							iUpdCnt++;
						}
					}
				}
				//write log
				m_SportsLog.FilePath = ConfigurationManager.AppSettings["eventlog"];
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " AnalysisStat.cs: Save Record " + iUpdCnt.ToString() + " statistical data (" + HttpContext.Current.Session["user_name"] + ")");
				m_SportsLog.Close();
			} catch(OleDbException ex) {
				m_SportsLog.FilePath = ConfigurationManager.AppSettings["errlog"];
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " AnalysisStat.cs.SaveRecord(): " + ex.ToString());
				m_SportsLog.Close();
				iUpdCnt = -1;
			}
			arrRtn[0] = iUpdCnt;
			return arrRtn;
		}

		public int[] Update() {
			int iUpdCnt = 0, iDelCnt = 0, iExisted = 0, iIdx = 0, iRecordCount = 0, iRecordIdx = 0;
			string sCurrentTimestamp = null;
			string sBatchJob = null;
			string sMatchDate = "";
			string sMatchTime = "";
			string sMatchField = "";
			string sHostHandicap = "";
			int[] arrRtn = new int[2];
			char[] delimiter = new char[] {','};
			string[] arrMatchCnt, arrLeague, arrHost, arrGuest, arrHostWin, arrHostDraw, arrHostLoss, arrGuestWin, arrGuestDraw, arrGuestLoss, arrSend, arrAction;
			string[] arrSendToPager;
			string[] arrRemotingPath;
			ArrayList matchCnt_AL = new ArrayList();
			//SportsMessage object message
			SportsMessage sptMsg = new SportsMessage();
			StringBuilder LogSQLString = new StringBuilder();
			DBManager logDBMgr = new DBManager();
			logDBMgr.ConnectionString = m_SportsDBMgr.ConnectionString;
			arrRemotingPath = (string[])HttpContext.Current.Application["RemotingItems"];

			arrMatchCnt = HttpContext.Current.Request.Form["matchcount"].Split(delimiter);
			arrLeague = HttpContext.Current.Request.Form["league"].Split(delimiter);
			arrHost = HttpContext.Current.Request.Form["host"].Split(delimiter);
			arrGuest = HttpContext.Current.Request.Form["guest"].Split(delimiter);
			arrHostWin = HttpContext.Current.Request.Form["hostwin"].Split(delimiter);
			arrHostDraw = HttpContext.Current.Request.Form["hostdraw"].Split(delimiter);
			arrHostLoss = HttpContext.Current.Request.Form["hostloss"].Split(delimiter);
			arrGuestWin = HttpContext.Current.Request.Form["guestwin"].Split(delimiter);
			arrGuestDraw = HttpContext.Current.Request.Form["guestdraw"].Split(delimiter);
			arrGuestLoss = HttpContext.Current.Request.Form["guestloss"].Split(delimiter);
			arrAction = HttpContext.Current.Request.Form["Action"].Split(delimiter);
			try {
				arrSend = HttpContext.Current.Request.Form["send"].Split(delimiter);
			} catch(Exception) {
				arrSend = new string[0];
			}
			try {
				arrSendToPager = HttpContext.Current.Request.Form["SendToPager"].Split(delimiter);
			} catch(Exception) {
				arrSendToPager = new string[0];
			}

            try
            {
                //copy array element into ArrayList
                iRecordCount = arrMatchCnt.Length;
                for (iIdx = 0; iIdx < iRecordCount; iIdx++)
                {
                    matchCnt_AL.Add(arrMatchCnt[iIdx]);
                }
                matchCnt_AL.TrimToSize();

                if (arrSend.Length > 0)
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


                    string sAction = "";
                    string[] arrMsgType;
                    arrMsgType = (string[])HttpContext.Current.Application["messageType"];

                    sCurrentTimestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                    sBatchJob = DateTime.Now.ToString("yyMMddHHmmss_fffffff") + "." + HttpContext.Current.Session["user_name"].ToString() + "." + arrMsgType[11] + ".ini";
                    for (iIdx = 0; iIdx < arrSend.Length; iIdx++)
                    {
                        //get checked item
                        iRecordIdx = matchCnt_AL.IndexOf(arrSend[iIdx]);

                        //get MatchDate, MatchTime, MatchField, HostHandicap from GAMEINFO
                        SQLString.Remove(0, SQLString.Length);
                        SQLString.Append("select MATCHDATE, MATCHTIME, FIELD, HOST_HANDI from GAMEINFO where MATCH_CNT=");
                        SQLString.Append(arrMatchCnt[iRecordIdx]);
                        m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
                        if (m_SportsOleReader.Read())
                        {
                            sMatchDate = m_SportsOleReader.GetString(0).Trim();
                            sMatchTime = m_SportsOleReader.GetString(1).Trim();
                            sMatchField = m_SportsOleReader.GetString(2).Trim();
                            sHostHandicap = m_SportsOleReader.GetString(3).Trim();
                        }
                        else
                        {
                            sMatchDate = "-1";
                            sMatchTime = "-1";
                            sMatchField = "-1";
                            sHostHandicap = "-1";
                        }
                        m_SportsOleReader.Close();
                        m_SportsDBMgr.Close();

                        //Update analysis stat info
                        SQLString.Remove(0, SQLString.Length);
                        SQLString.Append("select count(IMATCH_CNT) from ANALYSIS_STAT_INFO where IMATCH_CNT=");
                        SQLString.Append(arrMatchCnt[iRecordIdx]);
                        iExisted = m_SportsDBMgr.ExecuteScalar(SQLString.ToString());
                        m_SportsDBMgr.Close();
                        sAction = arrAction[iRecordIdx];
                        if (sAction.Equals("U"))
                        {
                            SQLString.Remove(0, SQLString.Length);
                            if (iExisted > 0)
                            {   //update existing record
                                SQLString.Append("update ANALYSIS_STAT_INFO set CACT='U',IHOSTWIN=");
                                SQLString.Append(arrHostWin[iRecordIdx]);
                                SQLString.Append(",IHOSTDRAW=");
                                SQLString.Append(arrHostDraw[iRecordIdx]);
                                SQLString.Append(",IHOSTLOSS=");
                                SQLString.Append(arrHostLoss[iRecordIdx]);
                                SQLString.Append(",IGUESTWIN=");
                                SQLString.Append(arrGuestWin[iRecordIdx]);
                                SQLString.Append(",IGUESTDRAW=");
                                SQLString.Append(arrGuestDraw[iRecordIdx]);
                                SQLString.Append(",IGUESTLOSS=");
                                SQLString.Append(arrGuestLoss[iRecordIdx]);
                                SQLString.Append(" where IMATCH_CNT=");
                                SQLString.Append(arrMatchCnt[iRecordIdx]);
                            }
                            else
                            {   //insert a new record
                                SQLString.Append("insert into ANALYSIS_STAT_INFO values(");
                                SQLString.Append(arrMatchCnt[iRecordIdx]);
                                SQLString.Append(",'U',");
                                SQLString.Append(arrHostWin[iRecordIdx]);
                                SQLString.Append(",");
                                SQLString.Append(arrHostDraw[iRecordIdx]);
                                SQLString.Append(",");
                                SQLString.Append(arrHostLoss[iRecordIdx]);
                                SQLString.Append(",");
                                SQLString.Append(arrGuestWin[iRecordIdx]);
                                SQLString.Append(",");
                                SQLString.Append(arrGuestDraw[iRecordIdx]);
                                SQLString.Append(",");
                                SQLString.Append(arrGuestLoss[iRecordIdx]);
                                SQLString.Append(")");
                            }
                            m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
                            m_SportsDBMgr.Close();

                            if (arrSendToPager.Length > 0)
                            {
                                //Insert log into LOG_ANALYSISSTAT
                                LogSQLString.Remove(0, LogSQLString.Length);
                                LogSQLString.Append("insert into LOG_ANALYSISSTAT (TIMEFLAG, SECTION, ACT, LEAGUE, HOST, GUEST, MATCHDATE, MATCHTIME, MATCHFIELD, HOSTHANDICAP, HOSTWIN, HOSTDRAW, HOSTLOSS, GUESTWIN, GUESTDRAW, GUESTLOSS, BATCHJOB) values ('");
                                LogSQLString.Append(sCurrentTimestamp);
                                LogSQLString.Append("','ANALYSISSTAT_','U','");
                                LogSQLString.Append(arrLeague[iRecordIdx]);
                                LogSQLString.Append("','");
                                LogSQLString.Append(arrHost[iRecordIdx]);
                                LogSQLString.Append("','");
                                LogSQLString.Append(arrGuest[iRecordIdx]);
                                LogSQLString.Append("','");
                                LogSQLString.Append(sMatchDate);
                                LogSQLString.Append("','");
                                LogSQLString.Append(sMatchTime);
                                LogSQLString.Append("','");
                                LogSQLString.Append(sMatchField);
                                LogSQLString.Append("','");
                                LogSQLString.Append(sHostHandicap);
                                LogSQLString.Append("',");
                                if (arrHostWin[iRecordIdx] == null)
                                {
                                    LogSQLString.Append("-1");
                                }
                                else if (arrHostWin[iRecordIdx].Trim().Equals(""))
                                {
                                    LogSQLString.Append("-1");
                                }
                                else
                                {
                                    LogSQLString.Append(arrHostWin[iRecordIdx]);
                                }
                                LogSQLString.Append(",");
                                if (arrHostDraw[iRecordIdx] == null)
                                {
                                    LogSQLString.Append("-1");
                                }
                                else if (arrHostDraw[iRecordIdx].Trim().Equals(""))
                                {
                                    LogSQLString.Append("-1");
                                }
                                else
                                {
                                    LogSQLString.Append(arrHostDraw[iRecordIdx]);
                                }
                                LogSQLString.Append(",");
                                if (arrHostLoss[iRecordIdx] == null)
                                {
                                    LogSQLString.Append("-1");
                                }
                                else if (arrHostLoss[iRecordIdx].Trim().Equals(""))
                                {
                                    LogSQLString.Append("-1");
                                }
                                else
                                {
                                    LogSQLString.Append(arrHostLoss[iRecordIdx]);
                                }
                                LogSQLString.Append(",");
                                if (arrGuestWin[iRecordIdx] == null)
                                {
                                    LogSQLString.Append("-1");
                                }
                                else if (arrGuestWin[iRecordIdx].Trim().Equals(""))
                                {
                                    LogSQLString.Append("-1");
                                }
                                else
                                {
                                    LogSQLString.Append(arrGuestWin[iRecordIdx]);
                                }
                                LogSQLString.Append(",");
                                if (arrGuestDraw[iRecordIdx] == null)
                                {
                                    LogSQLString.Append("-1");
                                }
                                else if (arrGuestDraw[iRecordIdx].Trim().Equals(""))
                                {
                                    LogSQLString.Append("-1");
                                }
                                else
                                {
                                    LogSQLString.Append(arrGuestDraw[iRecordIdx]);
                                }
                                LogSQLString.Append(",");
                                if (arrGuestLoss[iRecordIdx] == null)
                                {
                                    LogSQLString.Append("-1");
                                }
                                else if (arrGuestLoss[iRecordIdx].Trim().Equals(""))
                                {
                                    LogSQLString.Append("-1");
                                }
                                else
                                {
                                    LogSQLString.Append(arrGuestLoss[iRecordIdx]);
                                }
                                LogSQLString.Append(",'");
                                LogSQLString.Append(sBatchJob);
                                LogSQLString.Append("')");
                                logDBMgr.ExecuteNonQuery(LogSQLString.ToString());
                                logDBMgr.Close();
                            }

                            iUpdCnt++;
                        }
                        else
                        {
                            if (iExisted > 0)
                            {   //mark delete existing record
                                SQLString.Remove(0, SQLString.Length);
                                SQLString.Append("update ANALYSIS_STAT_INFO set CACT='D' where IMATCH_CNT=");
                                SQLString.Append(arrMatchCnt[iRecordIdx]);
                                m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
                                m_SportsDBMgr.Close();

                                if (arrSendToPager.Length > 0)
                                {
                                    //Insert log into LOG_ANALYSISSTAT
                                    LogSQLString.Remove(0, LogSQLString.Length);
                                    LogSQLString.Append("insert into LOG_ANALYSISSTAT (TIMEFLAG, SECTION, ACT, LEAGUE, HOST, GUEST, MATCHDATE, MATCHTIME, MATCHFIELD, HOSTHANDICAP, HOSTWIN, HOSTDRAW, HOSTLOSS, GUESTWIN, GUESTDRAW, GUESTLOSS, BATCHJOB) values ('");
                                    LogSQLString.Append(sCurrentTimestamp);
                                    LogSQLString.Append("','ANALYSISSTAT_','D','");
                                    LogSQLString.Append(arrLeague[iRecordIdx]);
                                    LogSQLString.Append("','");
                                    LogSQLString.Append(arrHost[iRecordIdx]);
                                    LogSQLString.Append("','");
                                    LogSQLString.Append(arrGuest[iRecordIdx]);
                                    LogSQLString.Append("','");
                                    LogSQLString.Append(sMatchDate);
                                    LogSQLString.Append("','");
                                    LogSQLString.Append(sMatchTime);
                                    LogSQLString.Append("','");
                                    LogSQLString.Append(sMatchField);
                                    LogSQLString.Append("','");
                                    LogSQLString.Append(sHostHandicap);
                                    LogSQLString.Append("',");
                                    if (arrHostWin[iRecordIdx] == null)
                                    {
                                        LogSQLString.Append("-1");
                                    }
                                    else if (arrHostWin[iRecordIdx].Trim().Equals(""))
                                    {
                                        LogSQLString.Append("-1");
                                    }
                                    else
                                    {
                                        LogSQLString.Append(arrHostWin[iRecordIdx]);
                                    }
                                    LogSQLString.Append(",");
                                    if (arrHostDraw[iRecordIdx] == null)
                                    {
                                        LogSQLString.Append("-1");
                                    }
                                    else if (arrHostDraw[iRecordIdx].Trim().Equals(""))
                                    {
                                        LogSQLString.Append("-1");
                                    }
                                    else
                                    {
                                        LogSQLString.Append(arrHostDraw[iRecordIdx]);
                                    }
                                    LogSQLString.Append(",");
                                    if (arrHostLoss[iRecordIdx] == null)
                                    {
                                        LogSQLString.Append("-1");
                                    }
                                    else if (arrHostLoss[iRecordIdx].Trim().Equals(""))
                                    {
                                        LogSQLString.Append("-1");
                                    }
                                    else
                                    {
                                        LogSQLString.Append(arrHostLoss[iRecordIdx]);
                                    }
                                    LogSQLString.Append(",");
                                    if (arrGuestWin[iRecordIdx] == null)
                                    {
                                        LogSQLString.Append("-1");
                                    }
                                    else if (arrGuestWin[iRecordIdx].Trim().Equals(""))
                                    {
                                        LogSQLString.Append("-1");
                                    }
                                    else
                                    {
                                        LogSQLString.Append(arrGuestWin[iRecordIdx]);
                                    }
                                    LogSQLString.Append(",");
                                    if (arrGuestDraw[iRecordIdx] == null)
                                    {
                                        LogSQLString.Append("-1");
                                    }
                                    else if (arrGuestDraw[iRecordIdx].Trim().Equals(""))
                                    {
                                        LogSQLString.Append("-1");
                                    }
                                    else
                                    {
                                        LogSQLString.Append(arrGuestDraw[iRecordIdx]);
                                    }
                                    LogSQLString.Append(",");
                                    if (arrGuestLoss[iRecordIdx] == null)
                                    {
                                        LogSQLString.Append("-1");
                                    }
                                    else if (arrGuestLoss[iRecordIdx].Trim().Equals(""))
                                    {
                                        LogSQLString.Append("-1");
                                    }
                                    else
                                    {
                                        LogSQLString.Append(arrGuestLoss[iRecordIdx]);
                                    }
                                    LogSQLString.Append(",'");
                                    LogSQLString.Append(sBatchJob);
                                    LogSQLString.Append("')");
                                    logDBMgr.ExecuteNonQuery(LogSQLString.ToString());
                                    logDBMgr.Close();
                                }

                                iDelCnt++;
                            }
                        }
                    }

                    if ((iDelCnt + iUpdCnt) > 0 && arrSendToPager.Length > 0)
                    {
                        //Modified by Chapman, 19 Feb 2004
                        //Send Notify Message
                        sptMsg.Body = sBatchJob;
                        sptMsg.Timestamp = DateTime.Now.ToString("yyMMddHHmmss");
                        sptMsg.AppID = "07";
                        sptMsg.MsgID = "11";
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
                                m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " MSMQ ERROR: Analysis Stat");
                                m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " AnalysisStat.cs.Update(): Notify via MSMQ throws MessageQueueException:  " + mqEx.ToString());
                                m_SportsLog.Close();

                                //If MSMQ fail, notify via .NET Remoting
                                msgClt.MessageType = arrMessageTypes[1];
                                msgClt.MessagePath = arrRemotingPath[0];
                                if (!msgClt.SendMessage((object)sptMsg))
                                {
                                    m_SportsLog.FilePath = ConfigurationManager.AppSettings["errlog"];
                                    m_SportsLog.SetFileName(0, LOGFILESUFFIX);
                                    m_SportsLog.Open();
                                    m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Remoting ERROR: Analysis Stat");
                                    m_SportsLog.Close();
                                }
                            }
                            catch (Exception ex)
                            {
                                m_SportsLog.FilePath = ConfigurationManager.AppSettings["errlog"];
                                m_SportsLog.SetFileName(0, LOGFILESUFFIX);
                                m_SportsLog.Open();
                                m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Remoting ERROR: Analysis Stat");
                                m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " AnalysisStat.cs.Update(): Notify via .NET Remoting throws exception: " + ex.ToString());
                                m_SportsLog.Close();
                            }
                        }
                        catch (Exception ex)
                        {
                            m_SportsLog.FilePath = ConfigurationManager.AppSettings["errlog"];
                            m_SportsLog.SetFileName(0, LOGFILESUFFIX);
                            m_SportsLog.Open();
                            m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " AnalysisStat.cs.Update(): Notify via MSMQ throws exception: " + ex.ToString());
                            m_SportsLog.Close();
                        }
                    }
                    m_SportsDBMgr.Dispose();
                }

                //write log
                m_SportsLog.FilePath = ConfigurationManager.AppSettings["eventlog"];
                m_SportsLog.SetFileName(0, LOGFILESUFFIX);
                m_SportsLog.Open();
                m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " AnalysisStat.cs: Update " + iUpdCnt.ToString() + " and delete " + iDelCnt.ToString() + " statistical data (" + HttpContext.Current.Session["user_name"] + ")");
                m_SportsLog.Close();
            }
            catch (OleDbException ex)
            {
                m_SportsLog.FilePath = ConfigurationManager.AppSettings["errlog"];
                m_SportsLog.SetFileName(0, LOGFILESUFFIX);
                m_SportsLog.Open();
                m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " AnalysisStat.cs.Update(): " + ex.ToString());
                m_SportsLog.Close();
                iUpdCnt = -1;
                iDelCnt = -1;
            }
            arrRtn[0] = iUpdCnt;
			arrRtn[1] = iDelCnt;
			return arrRtn;
		}
	}
}