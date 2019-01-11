/*
Objective:
Preview and send matches analysis information

Last updated:
12 July 2004, Chapman Choi

C#.NET complier statement:
csc /t:library /out:..\bin\AnalysisSinglePreview.dll /r:..\bin\DBManager.dll;..\bin\Files.dll;..\bin\MessageClient.dll;..\bin\SportsMessage.dll AnalysisSinglePreview.cs
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
	public class AnalysisSinglePreview {
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

        public AnalysisSinglePreview(string Connection) {
			m_SportsDBMgr = new DBManager();
			m_SportsDBMgr.ConnectionString = Connection;
			m_SportsLog = new Files();
			m_Big5Encoded = Encoding.GetEncoding(950);
			SQLString = new StringBuilder();
			arrFields = (string[])HttpContext.Current.Application["fieldItemsArray"];

            m_SportsDBMgrFb = new DBManagerFB();
            m_SportsDBMgrFb.ConnectionString = JC_SoccerWeb.Common.AppFlag.ScoutsDBConn;
        }

		public int NumberOfRecords {
			get {
				return iRecordsInPage;
			}
		}

		public string PreviewMatches() {
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

                SQLString.Remove(0, SQLString.Length);
                ////SQLString.Append("select gameinfo.matchdate, gameinfo.matchtime, gameinfo.leaglong, gameinfo.league, gameinfo.host, gameinfo.guest, gameinfo.match_cnt, leaginfo.LEAGUETYPE from gameinfo, leaginfo, analysis_bg_info where gameinfo.match_cnt=analysis_bg_info.imatch_cnt and gameinfo.league=leaginfo.alias and analysis_bg_info.cact='U' and leaginfo.leag_id in (select cleag_id from userprofile_info where iuser_id=");
                ////SQLString.Append(uid);
                ////SQLString.Append(") order by leaginfo.leag_order, gameinfo.matchdate, gameinfo.matchtime");
                SQLString.Append("select cast(cast(r.CMATCHDATETIME as date) as varchar(10)), cast(r.CMATCHDATETIME as time),r.CLEAGUEALIAS_OUTPUT_NAME , r.CLEAGUE_OUTPUT_NAME,   r.HKJCHOSTNAME_CN, r.HKJCGUESTNAME_CN,  e.id , '' sLeagType , R.HKJCDAYCODE, ");
                SQLString.Append(" (select a.NAME from TEAMS t inner join  AREAS a on t.AREA_ID=a.id  where t.ID=E.HOME_ID) , (select a.name from TEAMS t inner join  AREAS a on t.AREA_ID=a.id  where t.ID=E.HOME_ID) ,");
                SQLString.Append(" (select b2.NAME from AREAS B inner join AREAS b2 on b2.id=b.PARENT_AREA_ID where B.ID =(select AREA_ID from TEAMS where ID=e.HOME_ID)),");
                SQLString.Append(" (select  a.NAME from TEAMS t inner join  AREAS a on t.AREA_ID=a.id  where t.ID=E.GUEST_ID) ,  (select  a.NAME from TEAMS t inner join  AREAS a on t.AREA_ID=a.id  where t.ID=E.GUEST_ID) ,");
                SQLString.Append(" (select b2.NAME from AREAS B inner join AREAS b2 on b2.id=b.PARENT_AREA_ID  where B.ID =(select AREA_ID from TEAMS where ID=e.GUEST_ID)), x.name from  EMATCHES r inner join  events e on e.id =r.EMATCHID  inner join  AREAS x on x.id =e.AREA_ID  ");
                // SQLString.Append(" WHERE r.HKJCDAYCODE = (SELECT HKJCDAYCODE FROM EMATCHES WHERE EMATCHID ="+sEventID+ "  )  AND cast(cast(r.CMATCHDATETIME as date) as varchar(10))= (SELECT cast(cast(CMATCHDATETIME as date) as varchar(10)) FROM EMATCHES WHERE EMATCHID = " + sEventID + "   )");
                //  --and r.CMATCHDATETIME + 1 > (SELECT CMATCHDATETIME FROM EMATCHES WHERE EMATCHID = 2455014   ) and r.CMATCHDATETIME - 1 < (SELECT CMATCHDATETIME FROM EMATCHES WHERE EMATCHID = 2455014   )  
                //  SQLString.Append(" WHERE r.HKJCDAYCODE = (SELECT HKJCDAYCODE FROM EMATCHES WHERE EMATCHID =" + sEventID + "  ) and r.CMATCHDATETIME < (SELECT CMATCHDATETIME FROM EMATCHES WHERE EMATCHID =  " + sEventID + " )+1 and r.CMATCHDATETIME > (SELECT CMATCHDATETIME FROM EMATCHES WHERE EMATCHID =  " + sEventID + " ) -1 ");
                if (sEventID != "")
                {
                    SQLString.Append(" WHERE r.HKJCDAYCODE = (SELECT HKJCDAYCODE FROM EMATCHES WHERE EMATCHID =" + sEventID + "  ) and r.CMATCHDATETIME < (SELECT CMATCHDATETIME FROM EMATCHES WHERE EMATCHID =  " + sEventID + " )+1 and r.CMATCHDATETIME > (SELECT CMATCHDATETIME FROM EMATCHES WHERE EMATCHID =  " + sEventID + " ) -1 ");
                }
                else
                {
                    SQLString.Append("   WHERE r.HKJCDAYCODE = (SELECT first 1 HKJCDAYCODE FROM EMATCHES WHERE  cast(cast(CMATCHDATETIME as date) as varchar(10)) = cast(cast(current_timestamp as date) as varchar(10))   ) ");
                    SQLString.Append("  and r.CMATCHDATETIME < (SELECT first 1 CMATCHDATETIME FROM EMATCHES WHERE  cast(cast(CMATCHDATETIME as date) as varchar(10)) = cast(cast(current_timestamp as date) as varchar(10))   )+1 and r.CMATCHDATETIME > (SELECT  first 1 CMATCHDATETIME FROM EMATCHES WHERE  cast(cast(CMATCHDATETIME as date) as varchar(10)) = cast(cast(current_timestamp as date) as varchar(10))   ) -1 ");
                }
                //uid = HttpContext.Current.Session["user_id"].ToString();
                //SQLString.Remove(0,SQLString.Length);
                //SQLString.Append("select distinct gameinfo.matchdate, gameinfo.matchtime, gameinfo.leaglong, gameinfo.league, gameinfo.host, gameinfo.guest, gameinfo.match_cnt, leaginfo.leaguetype from gameinfo, leaginfo, analysis_recent_info where gameinfo.match_cnt=analysis_recent_info.imatch_cnt and analysis_recent_info.cact='U' and gameinfo.league=leaginfo.alias and leaginfo.leag_id in (select cleag_id from userprofile_info where iuser_id=");
                //SQLString.Append(uid);
                //SQLString.Append(") order by leaginfo.leag_order, gameinfo.matchdate, gameinfo.matchtime");
                m_SportsOleReaderFb = m_SportsDBMgrFb.ExecuteQuery(SQLString.ToString());
				while(m_SportsOleReaderFb.Read()) {
                    sDayCode = m_SportsOleReaderFb.GetString(8).Trim();
                    m_Title = "(" + sDayCode + ")" + "發送近績";
                    SQLString.Remove(0, SQLString.Length);
                    SQLString.Append("select count(IMATCH_CNT) from analysis_recent_info where CACT='U' and IMATCH_CNT=");
                    SQLString.Append(m_SportsOleReaderFb.GetString(6).Trim());
                    int iCount = m_SportsDBMgr.ExecuteScalar(SQLString.ToString());
                    m_SportsDBMgr.Close();
                    if (iCount == 0) { continue; }
                   // sDayCode = m_SportsOleReaderFb.GetString(8).Trim();
                    sMatchDate = m_SportsOleReaderFb.GetString(0).Trim();
					sMatchTime = m_SportsOleReaderFb.GetString(1).Trim();
					//sMatchDate = sMatchDate.Insert(4,"/");
					//sMatchDate = sMatchDate.Insert(7,"/");
					//sMatchTime = sMatchTime.Insert(2,":");

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
/*
					HTMLString.Append("\"><input type=\"checkbox\" name=\"analysis_bg\" value=\"");
					HTMLString.Append(iMatchCnt.ToString());
					HTMLString.Append("\"></td><td><input type=\"checkbox\" name=\"analysis_history\" value=\"");
					HTMLString.Append(iMatchCnt.ToString());
					HTMLString.Append("\"></td><td><input type=\"checkbox\" name=\"analysis_players\" value=\"");
					HTMLString.Append(iMatchCnt.ToString());
					HTMLString.Append("\"></td><td><input type=\"checkbox\" name=\"analysis_remarks\" value=\"");
					HTMLString.Append(iMatchCnt.ToString());
					HTMLString.Append("\"></td><td><input type=\"checkbox\" name=\"analysis_recent\" value=\"");
*/
					HTMLString.Append("\"><input type=\"checkbox\" name=\"analysis_recent\" value=\"");
					HTMLString.Append(iMatchCnt.ToString());
					HTMLString.Append("\"></td><td><input type=\"checkbox\" name=\"allitems\" value=\"1\"  onClick=\"selectAllItems(");
					HTMLString.Append(iRecordsInPage.ToString());
					HTMLString.Append(")\">所有項目</td></tr>");
					iRecordsInPage++;
				}
                m_SportsOleReaderFb.Close();
				m_SportsDBMgrFb.Close();
            
            } catch(Exception ex) {
				m_SportsLog.FilePath = ConfigurationManager.AppSettings["errlog"];
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " AnalysisSinglePreview.cs.PreviewMatches(): " + ex.ToString());
				m_SportsLog.Close();
				HTMLString.Remove(0,HTMLString.Length);
				HTMLString.Append((string)HttpContext.Current.Application["accessErrorMsg"]);
			}

			return HTMLString.ToString();
		}

		public int Send() {
			int iRecUpd = 0, iIdx = 0, iRecordCount = 0, iRecordIdx = 0;
			int iBgCnt = 0;
			int iHistoryCnt = 0;
			int iPlayerCnt = 0;
			int iRemarksCnt = 0;
			int iRecentCnt = 0;
			string sAction = "";
			string sCurrentTimestamp = null;
			string sBatchJob = null;
			char[] delimiter = new char[] {','};
			string[] arrLeague, arrHost, arrGuest, arrMatchCnt, arrAction, arrLeagType, arrRecentChecked;
			string[] arrSendToPager;
			string[] arrRemotingPath;
			ArrayList matchCnt_AL = new ArrayList();
			//SportsMessage object message
			SportsMessage sptMsg = new SportsMessage();
			StringBuilder LogSQLString = new StringBuilder();
			DBManager logDBMgr = new DBManager();
			logDBMgr.ConnectionString = m_SportsDBMgr.ConnectionString;
			arrRemotingPath = (string[])HttpContext.Current.Application["RemotingItems"];

			try {
				arrSendToPager = HttpContext.Current.Request.Form["SendToPager"].Split(delimiter);
			} catch(Exception) {
				arrSendToPager = new string[0];
			}
			arrLeague = HttpContext.Current.Request.Form["league"].Split(delimiter);
			arrHost = HttpContext.Current.Request.Form["host"].Split(delimiter);
			arrGuest = HttpContext.Current.Request.Form["guest"].Split(delimiter);
			arrMatchCnt = HttpContext.Current.Request.Form["matchcount"].Split(delimiter);
			arrAction = HttpContext.Current.Request.Form["Action"].Split(delimiter);
			arrLeagType = HttpContext.Current.Request.Form["leaguetype"].Split(delimiter);

			try {
				arrRecentChecked = HttpContext.Current.Request.Form["analysis_recent"].Split(delimiter);
			}	catch(Exception) {
				arrRecentChecked = new string[0];
			}

			try {
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
				//string sINIValue = "";
				string[] arrMsgType;
				arrMsgType = (string[])HttpContext.Current.Application["messageType"];

				//copy array element into ArrayList
				iRecordCount = arrMatchCnt.Length;
				for(iIdx=0;iIdx<iRecordCount;iIdx++) {
					matchCnt_AL.Add(arrMatchCnt[iIdx]);
				}
				matchCnt_AL.TrimToSize();

				/**************************************
					Send analysis recent information
				 **************************************/
				if(arrSendToPager.Length>0 && arrRecentChecked.Length>0) {
					sCurrentTimestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
					sBatchJob = DateTime.Now.ToString("yyMMddHHmmss_fffffff") + "." + HttpContext.Current.Session["user_name"].ToString() + "." + arrMsgType[17] + ".ini";
					for(iIdx=0;iIdx<arrRecentChecked.Length;iIdx++) {
						iRecordIdx = matchCnt_AL.IndexOf(arrRecentChecked[iIdx]);
						//check for Action
						sAction = arrAction[iRecordIdx];
						
						//delete from ANALYSIS_RECENT_INFO if required
						if(sAction.Equals("D")) {
							SQLString.Remove(0,SQLString.Length);
							SQLString.Append("update ANALYSIS_RECENT_INFO set CACT='D' where imatch_cnt=");
							SQLString.Append(arrRecentChecked[iIdx]);
							m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
							m_SportsDBMgr.Close();
						}

						SQLString.Remove(0,SQLString.Length);
						SQLString.Append("select i.CACT, g.LEAGLONG, g.HOST, g.GUEST, g.MATCHDATE, g.MATCHTIME, g.FIELD, g.HOST_HANDI, i.CTEAMFLAG, i.CLEAGUEALIAS, i.CCHALLENGER, i.IMATCHSTATUS, i.IHOSTSCORE, i.IGUESTSCORE from ANALYSIS_RECENT_INFO i, GAMEINFO g where i.IMATCH_CNT=g.MATCH_CNT and i.IMATCH_CNT=");
						SQLString.Append(arrRecentChecked[iIdx]);
						SQLString.Append(" order by IREC");
						m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
						int iItemSeq_no = 0;
						bool bChangedSeqNo = false;
						while (m_SportsOleReader.Read()) {
							if(!m_SportsOleReader.IsDBNull(0)) {
								if(!m_SportsOleReader.GetString(9).Trim().Equals("") && !m_SportsOleReader.GetString(10).Trim().Equals("")) {
									LogSQLString.Remove(0,LogSQLString.Length);
									LogSQLString.Append("insert into LOG_ANALYSISRECENT (TIMEFLAG, IITEMSEQ_NO, SECTION, ACT, LEAGUE, HOST, GUEST, MATCHDATE, MATCHTIME, MATCHFIELD, HOSTHANDICAP, RECENTTYPE, RECENTALIAS, RECENTCHALLENGER, RECENTFIELD, RECENTHOSTSCORE, RECENTGUESTSCORE, BATCHJOB) values ('");
									LogSQLString.Append(sCurrentTimestamp);									
									if (m_SportsOleReader.GetString(8).Trim().Equals("G") && !bChangedSeqNo) {
										iItemSeq_no = 1;
										bChangedSeqNo = true;
									} else {
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

					if(iRecentCnt > 0) {
						//Send Notify Message
						//Modified by Henry, 10 Feb 2004
						sptMsg.Body = sBatchJob;
						sptMsg.Timestamp = DateTime.Now.ToString("yyMMddHHmmss");
						sptMsg.AppID = "07";
						sptMsg.MsgID = "12";
						sptMsg.DeviceID = new string[0];						
						for(int i = 0; i < arrSendToPager.Length; i++) {
							sptMsg.AddDeviceID((string)arrSendToPager[i]);
						}					
						try {
							//Notify via MSMQ
							msgClt.MessageType = arrMessageTypes[0];
							msgClt.MessagePath = arrQueueNames[0];
							msgClt.SendMessage(sptMsg);
                        }   // catch (System.Messaging.MessageQueueException mqEx)
                        catch (InvalidOperationException mqEx)
                        {
							try {
								m_SportsLog.FilePath = ConfigurationManager.AppSettings["errlog"];
								m_SportsLog.SetFileName(0,LOGFILESUFFIX);
								m_SportsLog.Open();
								m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " MSMQ ERROR: Analysis Preview");
								m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " AnalysisSinglePreview.cs.Send(): Notify via MSMQ throws MessageQueueException:  " + mqEx.ToString());
								m_SportsLog.Close();
	
								//If MSMQ fail, notify via .NET Remoting
								msgClt.MessageType = arrMessageTypes[1];
								msgClt.MessagePath = arrRemotingPath[0];
								if(!msgClt.SendMessage((object)sptMsg)) {
									m_SportsLog.FilePath = ConfigurationManager.AppSettings["errlog"];
									m_SportsLog.SetFileName(0,LOGFILESUFFIX);
									m_SportsLog.Open();
									m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Remoting ERROR: Analysis Preview");
									m_SportsLog.Close();
								}
							}	catch(Exception ex) {
								m_SportsLog.FilePath = ConfigurationManager.AppSettings["errlog"];
								m_SportsLog.SetFileName(0,LOGFILESUFFIX);
								m_SportsLog.Open();
								m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Remoting ERROR: Analysis Preview");
								m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " AnalysisSinglePreview.cs.Send(): Notify via .NET Remoting throws exception: " + ex.ToString());
								m_SportsLog.Close();
							}							
						}	catch(Exception ex) {
							m_SportsLog.FilePath = ConfigurationManager.AppSettings["errlog"];
							m_SportsLog.SetFileName(0,LOGFILESUFFIX);
							m_SportsLog.Open();
							m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " AnalysisSinglePreview.cs.Send(): Notify via MSMQ throws exception: " + ex.ToString());
							m_SportsLog.Close();
						}					
						//Modified end						
					}

					//write log
					m_SportsLog.FilePath = ConfigurationManager.AppSettings["eventlog"];
					m_SportsLog.SetFileName(0,LOGFILESUFFIX);
					m_SportsLog.Open();
					m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " AnalysisSinglePreview.cs: Send " + iRecentCnt.ToString() + " recent (" + HttpContext.Current.Session["user_name"] + ")");
					m_SportsLog.Close();
				}
			} catch (IndexOutOfRangeException IndexEx) {
				iRecUpd = -1;
				m_SportsLog.FilePath = ConfigurationManager.AppSettings["errlog"];
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " AnalysisSinglePreview.cs.Send().IndexOutOfRangeException: " + IndexEx.ToString());
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " AnalysisSinglePreview.cs.Send().IndexOutOfRangeException: \nMessage ---\n" + IndexEx.Message);
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " AnalysisSinglePreview.cs.Send().IndexOutOfRangeException: \nSource ---\n" + IndexEx.Source);
				m_SportsLog.Close();
			} catch (Exception ex) {
				iRecUpd = -1;
				m_SportsLog.FilePath = ConfigurationManager.AppSettings["errlog"];
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " AnalysisSinglePreview.cs.Send().Exception: " + ex.ToString());
				m_SportsLog.Close();
			}
			iRecUpd = iBgCnt + iHistoryCnt + iPlayerCnt + iRemarksCnt + iRecentCnt;
			return iRecUpd;
		}
	}
}