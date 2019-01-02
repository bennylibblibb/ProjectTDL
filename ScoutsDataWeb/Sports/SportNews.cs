/*
Objective:
Update sports news

Last updated:
17 Sep 2004 (Fanny) Replicator newsinfo data into db2
12 Feb 2004 by HenryCi

C#.NET complier statement (With Repplicator):
csc /t:library /out:..\bin\SportNews.dll /r:..\bin\DBManager.dll;..\bin\Files.dll;..\bin\MessageClient.dll;..\bin\SportsMessage.dll;..\bin\Replicator.dll SportNews.cs
*/

using System;
using System.Configuration;
using System.Data.OleDb;
using System.Reflection;
using System.Text;
using System.Web;
using TDL.DB;
using TDL.IO;
////using TDL.Util;
using TDL.Message;

 
namespace SportsUtil {
	public class SportNews {
		const string LOGFILESUFFIX = "log";
		const string DBCR = "(CR)";
		const string PAGECR = "\r\n";
		int m_MsgCnt;
		int m_MatchRecordCnt = 0;
		OleDbDataReader m_SportsOleReader;
		DBManager m_SportsDBMgr;
		Files m_SportsLog;
		Encoding m_Big5Encoded;

		public SportNews(string Connection) {
			m_SportsDBMgr = new DBManager();
			m_SportsDBMgr.ConnectionString = Connection;
			m_SportsLog = new Files();
			m_Big5Encoded = Encoding.GetEncoding(950);
		}

		public int MessageCount {
			get {
				return m_MsgCnt;
			}
		}

		public int MatchRecordCount {
			get {
				return m_MatchRecordCnt;
			}
		}

		public string GetMessage() {
			int iRecordCnt = 0;
			string sNewsQuery;
			string sAppID;
			string sRtn;
			string sMsgType = null;
			string sInfoType = null;
			string sRecordMsg = null;

			sAppID = HttpContext.Current.Request.QueryString["AppID"];
			m_MsgCnt = Convert.ToInt32(HttpContext.Current.Request.QueryString["MsgCnt"]);
			try {
				sNewsQuery = "select CAPPTYPE from NEWS_CFG where ISEQNO=" + sAppID;
				sMsgType = m_SportsDBMgr.ExecuteQueryString(sNewsQuery);
				sNewsQuery = "select CINFOTYPE from NEWS_CFG where ISEQNO=" + sAppID;
				sInfoType = m_SportsDBMgr.ExecuteQueryString(sNewsQuery);
				sRtn = "<tr><th colspan=\"3\" align=\"left\">" + sMsgType.Substring(0,sMsgType.IndexOf("[")) + "-" + sInfoType + "</th><th align=\"center\" width=\"15%\">發送</th></tr>";
				sNewsQuery = "select CMESSAGE,IMSG_ID from NEWSINFO where CACT='U' AND IAPP_ID=" + sAppID + " order by IMSG_ID";
				m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(sNewsQuery);
				while(m_SportsOleReader.Read()) {
					if(!m_SportsOleReader.IsDBNull(0)) sRecordMsg = m_SportsOleReader.GetString(0).Trim().Replace(DBCR,PAGECR);
					else sRecordMsg = "";
					if(sRecordMsg.Equals("-1")) sRecordMsg = "";
					if(iRecordCnt < m_MsgCnt) {
						sRtn += "<tr align=\"center\"><td width=\"6%\">" + (iRecordCnt+1).ToString() + "</td><td width=\"12%\"><select name=\"Action\" OnChange=\"ActionChange(" + iRecordCnt + ")\"><option value=\"U\">更新<option value=\"D\">刪除</select></td>";
						sRtn += "<td><textarea name=\"SportMsg" + iRecordCnt.ToString() + "\" rows=\"8\" cols=\"40\" OnChange=\"MsgChange(" + iRecordCnt + ")\">" + sRecordMsg + "</textarea></td>";
						if (Convert.ToInt32(sAppID)>=409 && Convert.ToInt32(sAppID) <=443)
							sRtn += "<td><input disabled=\"true\" type=\"checkbox\" name=\"mustSend\" value=\"" + iRecordCnt + "\" checked>";//disabled=\"true\"
						else
							sRtn += "<td><input type=\"checkbox\" name=\"mustSend\" value=\"" + iRecordCnt + "\">";
						sRtn += "<input type=\"hidden\" name=\"MsgID\" value=\"" + m_SportsOleReader.GetInt32(1).ToString() + "\"></td></tr>";
					} else {
						sRtn += "<tr align=\"center\"><td width=\"6%\">" + (iRecordCnt+1).ToString() + "</td><td width=\"12%\"></td>";
						sRtn += "<td>" + sRecordMsg + "</td><td>未能發送之訊息</td></tr>";
					}
					iRecordCnt++;
				}
				m_SportsDBMgr.Close();
				m_SportsOleReader.Close();

				while(iRecordCnt < m_MsgCnt) {
					sRtn += "<tr align=\"center\"><td width=\"6%\">" + (iRecordCnt+1).ToString() + "</td><td width=\"12%\"><select name=\"Action\" OnChange=\"ActionChange(" + iRecordCnt + ")\"><option value=\"U\">更新<option value=\"D\">刪除</select></td><td><textarea name=\"SportMsg" + iRecordCnt.ToString() + "\" rows=\"8\" cols=\"40\" OnChange=\"MsgChange(" + iRecordCnt + ")\"></textarea></td>";
					if (Convert.ToInt32(sAppID)>=409 && Convert.ToInt32(sAppID) <=443)
						sRtn += "<td><input disabled=\"true\" type=\"checkbox\" name=\"mustSend\" value=\"" + iRecordCnt + "\" checked>";
					else
						sRtn += "<td><input type=\"checkbox\" name=\"mustSend\" value=\"" + iRecordCnt + "\">";
					sRtn += "<input type=\"hidden\" name=\"MsgID\" value=\"-1\"></td></tr>";
					iRecordCnt++;
				}
				sRtn += "<input type=\"hidden\" name=\"AppID\" value=\"" + sAppID + "\">";

				/**********************************
				 * GoGo2 Pager: Display matchlist
				 **********************************/
				if((sMsgType.IndexOf("足球資訊") != -1) || (sMsgType.IndexOf("推介") != -1)) {
					if((sInfoType.IndexOf("推介") != -1) || (sInfoType.IndexOf("直播") != -1)) {
						sRtn += "<tr bgcolor=\"#F5FFFA\"><th colspan=\"3\" align=\"left\"><font color=\"red\">新GOGO機必要選項</font>，請選擇相關" + sInfoType + "賽事:</th>";
						sRtn += "<th>全選<input type=\"checkbox\" name=\"selectedAllMatches\" value=\"1\" onClick=\"selectAllMatches()\"></th></tr>";
						sNewsQuery = "select LEAGLONG,HOST,GUEST,MATCH_CNT,(select count(*) from NEWSINFO_MATCHLIST where IMATCH_CNT=GAMEINFO.MATCH_CNT AND IAPP_ID=" + sAppID + ") from GAMEINFO where ACT='U' order by LEAGUE, MATCHDATE, MATCHTIME";
						m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(sNewsQuery);
						while(m_SportsOleReader.Read()) {
							sRtn += "<tr bgcolor=\"#F5FFFA\" align=\"center\"><th colspan=\"2\"><input type=\"hidden\" name=\"League\" value=\"" + m_SportsOleReader.GetString(0).Trim() + "\">" + m_SportsOleReader.GetString(0).Trim() + "</th>";
							sRtn += "<td><input type=\"hidden\" name=\"Host\" value=\"" + m_SportsOleReader.GetString(1).Trim() + "\">" + m_SportsOleReader.GetString(1).Trim() + " vs ";
							sRtn += "<input type=\"hidden\" name=\"Guest\" value=\"" + m_SportsOleReader.GetString(2).Trim() + "\">" + m_SportsOleReader.GetString(2).Trim() + "</td>";
							sRtn += "<td><input type=\"hidden\" name=\"MatchCount\" value=\"" + m_SportsOleReader.GetInt32(3).ToString() + "\"><input type=\"checkbox\" name=\"selectedMatch\" value=\"" + m_MatchRecordCnt.ToString() + "\" ";
							if(m_SportsOleReader.GetInt32(4) > 0) sRtn += "checked";
							sRtn += "></td></tr>";
							m_MatchRecordCnt++;
						}
						m_SportsDBMgr.Close();
						m_SportsOleReader.Close();
					}
				}
				m_SportsDBMgr.Dispose();

				if(m_MatchRecordCnt == 0) sRtn += "<input type=\"hidden\" name=\"MatchCount\" value=\"\"><input type=\"hidden\" name=\"League\" value=\"\"><input type=\"hidden\" name=\"Host\" value=\"\"><input type=\"hidden\" name=\"Guest\" value=\"\">";
			} catch(Exception ex) {
				m_SportsLog.FilePath = ConfigurationManager.AppSettings["errlog"];
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " SportNews.cs.GetMessage(): " + ex.ToString());
				m_SportsLog.Close();
				sRtn = (string)HttpContext.Current.Application["accessErrorMsg"];
			}

			return sRtn;
		}

		public int SendNews() {
			char[] delimiter = new char[] {','};
			bool bNewMsg = false;
			bool bUpdLogList = false;
			int iMustSendLen = 0;
			int iMatchListLen = 0;
			int iArrayIndex = 0;
			int iNeedToUpdate = 0;
			int iMsgID = 0;
			int iLogUpd = 0;
			string sNewsQuery = null;
			string sMsgContent;
			string sCurrDate = DateTime.Now.ToString("yyyyMMdd");
			string sCurrTime = DateTime.Now.ToString("HHmm");
			string sAppID;
			string sCurrentTimestamp = null;
			string sBatchJob = null;
			string[] arrMustSend;
			string[] arrAction;
			string[] arrMsgID;
			string[] arrMsgType;
			string[] arrMatchList;
			string[] arrMatchCount;
			string[] arrLeague;
			string[] arrHost;
			string[] arrGuest;
			string[] arrSendToPager;
			string[] arrRemotingPath;

			//SportsMessage object message
			SportsMessage sptMsg = new SportsMessage();
			StringBuilder LogSQLString = new StringBuilder();
			DBManager logDBMgr = new DBManager();
			logDBMgr.ConnectionString = m_SportsDBMgr.ConnectionString;

			try {
				arrSendToPager = HttpContext.Current.Request.Form["SendToPager"].Split(delimiter);
			} catch(Exception) {
				arrSendToPager = new string[0];
			}
			arrMsgType = (string[])HttpContext.Current.Application["messageType"];
			try {
				arrMustSend = HttpContext.Current.Request.Form["mustSend"].Split(delimiter);
				iMustSendLen = arrMustSend.Length;
			} catch(Exception) {
				arrMustSend = new string[0];
			}
			try {
				arrMatchList = HttpContext.Current.Request.Form["selectedMatch"].Split(delimiter);
				iMatchListLen = arrMatchList.Length;
			} catch(Exception) {
				arrMatchList = new string[0];
			}

			arrRemotingPath = (string[])HttpContext.Current.Application["RemotingItems"];
			arrMatchCount = HttpContext.Current.Request.Form["MatchCount"].Split(delimiter);
			arrLeague = HttpContext.Current.Request.Form["League"].Split(delimiter);
			arrHost = HttpContext.Current.Request.Form["Host"].Split(delimiter);
			arrGuest = HttpContext.Current.Request.Form["Guest"].Split(delimiter);
			arrAction = HttpContext.Current.Request.Form["Action"].Split(delimiter);
			sAppID = HttpContext.Current.Request.Form["AppID"];
			arrMsgID = HttpContext.Current.Request.Form["MsgID"].Split(delimiter);

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

			try {
				sCurrentTimestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
				sBatchJob = DateTime.Now.ToString("yyMMddHHmmss_fffffff") + "." + HttpContext.Current.Session["user_name"].ToString() + "." + arrMsgType[18] + ".ini";
				for(iArrayIndex = 0; iArrayIndex < iMustSendLen; iArrayIndex++) {
					iNeedToUpdate = Convert.ToInt32(arrMustSend[iArrayIndex]);
					if(arrMsgID[iNeedToUpdate].Equals("-1")) {	//assign new MsgID
						bNewMsg = true;
						sNewsQuery = "select MAX(IMSG_ID) from NEWSINFO";
						m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(sNewsQuery);
						if(m_SportsOleReader.Read()) {
							if(!m_SportsOleReader.IsDBNull(0)) iMsgID = m_SportsOleReader.GetInt32(0) + 1;
							else iMsgID = 1;
						} else {
							iMsgID = 1;
						}
						m_SportsDBMgr.Close();
						m_SportsOleReader.Close();
					} else {	//update with current MsgID
						bNewMsg = false;
						iMsgID = Convert.ToInt32(arrMsgID[iNeedToUpdate]);
					}
					sMsgContent = HttpContext.Current.Request.Form["SportMsg" + iNeedToUpdate.ToString()];
					if(sMsgContent == null) sMsgContent = "";
					sMsgContent = sMsgContent.Trim().Replace("'","''");
					sMsgContent = sMsgContent.Trim().Replace(PAGECR,DBCR);
					if(sMsgContent.Equals("-1")) sMsgContent = "";

					//Write To DB
					if(bNewMsg) sNewsQuery = "insert into NEWSINFO values(" + iMsgID.ToString() + ",'" + arrAction[iNeedToUpdate] + "'," + sAppID + ",'" + sCurrDate + "','" + sCurrTime + "','" + sMsgContent + "','0')";
					else sNewsQuery = "update NEWSINFO set CACT='" + arrAction[iNeedToUpdate] + "',CNEWSDATE='" + sCurrDate + "',CNEWSTIME='" + sCurrTime + "',CMESSAGE='" + sMsgContent + "' where IMSG_ID=" + iMsgID.ToString() + " AND IAPP_ID=" + sAppID;
					m_SportsDBMgr.ExecuteNonQuery(sNewsQuery);
					m_SportsDBMgr.Close();

					//////17 Sep 2004 (Fanny) start
					//////Replicator
					//////remarks: For newsinfo table in IB, all deletion is done with the help of backup application
					//////For newsinfo in DB2, all deleteion must done by itself.
					////Replicator newsReplicator;
					////string newsRepSQL="";
					////try{
					////	newsReplicator = new Replicator(HttpContext.Current.Application["RepDBConnectionString"].ToString());
					////	newsReplicator.ApplicationType = 1;
					////	newsReplicator.ErrorLogPath = ConfigurationManager.AppSettings["errlog"];
					////	//case: delete
					////	if (arrAction[iNeedToUpdate]=="D"){
					////		newsRepSQL = "delete from NEWSINFO where IMSG_ID=" + iMsgID.ToString() + " AND IAPP_ID=" + sAppID;
					////	}
					////	//case: insert or update
					////	else{
					////		newsRepSQL = sNewsQuery;
					////	}
					////	newsReplicator.Replicate(newsRepSQL);
					////	newsReplicator.Dispose();
					////}
					////catch(Exception ex){
					////	m_SportsLog.FilePath = ConfigurationManager.AppSettings["errlog"];
					////	m_SportsLog.SetFileName(0,LOGFILESUFFIX);
					////	m_SportsLog.Open();
					////	m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " SportNews.cs.SendNews(): Replicate newsinfo throws exception: " + ex.ToString());
					////	m_SportsLog.Close();
					////}
					//////17 Sep 2004 (Fanny) end


					//Construct INI Content
					if(arrSendToPager.Length>0 && !sMsgContent.Equals("")) {
						iLogUpd++;
						LogSQLString.Remove(0,LogSQLString.Length);
						LogSQLString.Append("insert into LOG_SPORTNEWS (TIMEFLAG, SECTION, IMSG_ID, IAPP_ID, ACT, NEWSDATE, NEWSTIME, CONTENT, BATCHJOB) values ('");
						LogSQLString.Append(sCurrentTimestamp);
						LogSQLString.Append("','NEWS_',");
						LogSQLString.Append(iMsgID.ToString());
						LogSQLString.Append(",");
						LogSQLString.Append(sAppID);
						LogSQLString.Append(",'");
						LogSQLString.Append(arrAction[iNeedToUpdate]);
						LogSQLString.Append("','");
						LogSQLString.Append(sCurrDate);
						LogSQLString.Append("','");
						LogSQLString.Append(sCurrTime);
						LogSQLString.Append("','");
						LogSQLString.Append(sMsgContent);
						LogSQLString.Append("','");
						LogSQLString.Append(sBatchJob);
						LogSQLString.Append("')");
						logDBMgr.ExecuteNonQuery(LogSQLString.ToString());
						logDBMgr.Close();
					}
				}


				/**********************************
				 * GoGo2 Pager: Update matchlist
				 **********************************/
				int iMatchIdx = 0;
				sNewsQuery = "delete from NEWSINFO_MATCHLIST where IAPP_ID=" + sAppID;
				m_SportsDBMgr.ExecuteNonQuery(sNewsQuery);
				m_SportsDBMgr.Close();
				for(int i = 0; i < iMatchListLen; i++) {
					iMatchIdx = Convert.ToInt32(arrMatchList[i]);
					sNewsQuery = "insert into NEWSINFO_MATCHLIST values(" + sAppID + "," + arrMatchCount[iMatchIdx] + ",'" + arrLeague[iMatchIdx] + "','" + arrHost[iMatchIdx] + "','" + arrGuest[iMatchIdx] + "')";
					m_SportsDBMgr.ExecuteNonQuery(sNewsQuery);
					m_SportsDBMgr.Close();

					if(arrSendToPager.Length>0) {
						bUpdLogList = false;
						for(int j = 0; j < arrSendToPager.Length; j++) {
							if(!(arrSendToPager[j].Equals("1") || arrSendToPager[j].Equals("3")) && !bUpdLogList) {
								bUpdLogList = true;
								LogSQLString.Remove(0,LogSQLString.Length);
								LogSQLString.Append("insert into LOG_MATCHLIST (TIMEFLAG, SECTION, IAPP_ID, LEAGUE, HOST, GUEST, BATCHJOB) values ('");
								LogSQLString.Append(sCurrentTimestamp);
								LogSQLString.Append("','MATCHLIST',");
								LogSQLString.Append(sAppID);
								LogSQLString.Append(",'");
								LogSQLString.Append(arrLeague[iMatchIdx]);
								LogSQLString.Append("','");
								LogSQLString.Append(arrHost[iMatchIdx]);
								LogSQLString.Append("','");
								LogSQLString.Append(arrGuest[iMatchIdx]);
								LogSQLString.Append("','");
								LogSQLString.Append(sBatchJob);
								LogSQLString.Append("')");
								logDBMgr.ExecuteNonQuery(LogSQLString.ToString());
								logDBMgr.Close();
							}
						}
					}
				}

				if(iLogUpd > 0) {
					//Send Notify Message
					//Modified by Henry, 12 Feb 2004
					sptMsg.Body = sBatchJob;
					sptMsg.Timestamp = DateTime.Now.ToString("yyMMddHHmmss");
					sptMsg.AppID = "07";
					sptMsg.MsgID = "13";
					sptMsg.DeviceID = new string[0];
					for(int i = 0; i < arrSendToPager.Length; i++) {
						sptMsg.AddDeviceID((string)arrSendToPager[i]);
					}
					try {
						//Notify via MSMQ
						msgClt.MessageType = arrMessageTypes[0];
						msgClt.MessagePath = arrQueueNames[0];
						msgClt.SendMessage(sptMsg);
					}
                    ////catch (System.Messaging.MessageQueueException mqEx) {
                    catch (InvalidOperationException mqEx)
                    { 
                        try {
							m_SportsLog.FilePath = ConfigurationManager.AppSettings["errlog"];
							m_SportsLog.SetFileName(0,LOGFILESUFFIX);
							m_SportsLog.Open();
							m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " MSMQ ERROR: Sport News");
							m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " SportNews.cs.SendNews(): Notify via MSMQ throws MessageQueueException:  " + mqEx.ToString());
							m_SportsLog.Close();

							//If MSMQ fail, notify via .NET Remoting
							msgClt.MessageType = arrMessageTypes[1];
							msgClt.MessagePath = arrRemotingPath[0];
							if(!msgClt.SendMessage((object)sptMsg)) {
								m_SportsLog.FilePath = ConfigurationManager.AppSettings["errlog"];
								m_SportsLog.SetFileName(0,LOGFILESUFFIX);
								m_SportsLog.Open();
								m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Remoting ERROR: Sport News");
								m_SportsLog.Close();
							}
						}	catch(Exception ex) {
							m_SportsLog.FilePath = ConfigurationManager.AppSettings["errlog"];
							m_SportsLog.SetFileName(0,LOGFILESUFFIX);
							m_SportsLog.Open();
							m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Remoting ERROR: Sport News");
							m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " SportNews.cs.SendNews(): Notify via .NET Remoting throws exception: " + ex.ToString());
							m_SportsLog.Close();
						}
					}	catch(Exception ex) {
						m_SportsLog.FilePath = ConfigurationManager.AppSettings["errlog"];
						m_SportsLog.SetFileName(0,LOGFILESUFFIX);
						m_SportsLog.Open();
						m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " SportNews.cs.SendNews(): Notify via MSMQ throws exception: " + ex.ToString());
						m_SportsLog.Close();
					}
					//Modified end
				}
				m_SportsDBMgr.Dispose();

				//write log
				m_SportsLog.FilePath = ConfigurationManager.AppSettings["eventlog"];
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " SportNews.cs: Send " + iArrayIndex.ToString() + " news (" + HttpContext.Current.Session["user_name"] + ")");
				m_SportsLog.Close();
			} catch(Exception ex) {
				iArrayIndex = -1;
				m_SportsLog.FilePath = ConfigurationManager.AppSettings["errlog"];
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " SportNews.cs.SendNews(): " + ex.ToString());
				m_SportsLog.Close();
			}

			return iArrayIndex;
		}
	}
}