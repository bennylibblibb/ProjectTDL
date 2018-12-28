/*
Objective:
Update live goal one match only and send related info to MSMQ server
if MSMQ failed, send via .NET Remoting

Last updated:
25 Oct 2004, (Fanny Cheung) Change status field into 2 char in HKJC format for Goal Alert
9 Aug 2004, Chapman Choi
9 Aug 2004 Convert league and tem to HKJC format for Goal Alert
31 May 2004 Send MangoSports Goal Alert to a new queue
23 Mar 2004 Remark Replicator code
26 Feb 2004 Additional match status for liveodds

C#.NET complier statement:
(With Replicator) csc /t:library /out:..\bin\LiveGoalOneOnly.dll /r:..\bin\DBManager.dll;..\bin\Files.dll;..\bin\MessageClient.dll;..\bin\Replicator.dll;..\bin\SportsMessage.dll LiveGoalOneOnly.cs

Without Replicator (Current production):
csc /t:library /out:..\bin\LiveGoalOneOnly.dll /r:..\bin\DBManager.dll;..\bin\Files.dll;..\bin\MessageClient.dll;..\bin\SportsMessage.dll LiveGoalOneOnly.cs
*/

using System;
using System.Data.OleDb;
using System.Collections;
using System.Collections.Specialized;
using System.Reflection;
using System.Text;
using System.Web;
using TDL.DB;
using TDL.IO;
using TDL.Message;
//using TDL.Util;

[assembly:AssemblyCompany("TDSL")]
[assembly:AssemblyCopyright("(c) 2003 TDSL. All rights reserved.")]
[assembly:AssemblyDescription("足球資訊 -> 現場比數(單場)")]
[assembly:AssemblyProduct("Sports.NET")]
[assembly:AssemblyTitle("Sports.NET DLL")]
[assembly:AssemblyVersion("3.1.*")]
namespace SportsUtil {
	public class LiveGoalOneOnly {
		const string LOGFILESUFFIX = "log";
		OleDbDataReader m_SportsOleReader;
		DBManager m_SportsDBMgr;
		Files m_SportsLog;
		Encoding m_Big5Encoded;
		StringBuilder SQLString;

		public LiveGoalOneOnly(string Connection) {
			m_SportsDBMgr = new DBManager();
			m_SportsDBMgr.ConnectionString = Connection;
			m_SportsLog = new Files();
			m_Big5Encoded = Encoding.GetEncoding(950);
			SQLString = new StringBuilder();
		}

		public string GetLiveGoal() {
			int iMatchCount;
			int iArrayIndex = 0;
			string sRecordStr;
			string sMatchCnt;
			string sAlias = "";
			string sHost = "";
			string sGuest= "";
			string sStatus = "";
			string[] validityArray;
			string[] statusArray;
			DateTime dtStart;
			StringBuilder HTMLString = new StringBuilder();

			sMatchCnt = HttpContext.Current.Request.QueryString["matchcount"];
			validityArray = (string[])HttpContext.Current.Application["goalItemsArray"];
			statusArray = (string[])HttpContext.Current.Application["matchItemsArray"];
			if(sMatchCnt == null) {
				sMatchCnt= "0";
			} else {
				if(sMatchCnt.Equals("")) sMatchCnt= "0";
			}
			SQLString.Remove(0,SQLString.Length);
			//SQLString.Append("select goalinfo.match_cnt, gameinfo.LEAGUE, gameinfo.leaglong, goalinfo.status, gameinfo.matchdate, gameinfo.matchtime, gameinfo.field, gameinfo.host, goalinfo.h_scr, gameinfo.guest, goalinfo.g_scr, (select STARTTIME from TIMEOFGAME_INFO where IMATCH_CNT=goalinfo.match_cnt) STARTTIME from gameinfo, goalinfo where gameinfo.match_cnt=goalinfo.match_cnt and goalinfo.match_cnt=");
			SQLString.Append("select goalinfo.status, (select STARTTIME from TIMEOFGAME_INFO where IMATCH_CNT=goalinfo.match_cnt) STARTTIME, goalinfo.match_cnt, gameinfo.LEAGUE, gameinfo.leaglong, gameinfo.matchdate, gameinfo.matchtime, gameinfo.field, gameinfo.host, goalinfo.h_scr, gameinfo.guest, goalinfo.g_scr from gameinfo, goalinfo where gameinfo.match_cnt=goalinfo.match_cnt and goalinfo.match_cnt=");
			SQLString.Append(sMatchCnt);
			try {
				m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
				if(m_SportsOleReader.Read()) {
					//Match Status (Hidden field)
					sStatus = m_SportsOleReader.GetString(0);
					iArrayIndex = 0;
					HTMLString.Remove(0,HTMLString.Length);
					HTMLString.Append("<tr align=\"center\" style=\"background-color:#87CEFA\"><td><select name=\"status\"><option value=");
					HTMLString.Append(sStatus);
					HTMLString.Append(">");
					HTMLString.Append(statusArray[Convert.ToInt32(sStatus)]);
					foreach(String sItem in statusArray) {
						if(!sItem.Equals(statusArray[Convert.ToInt32(sStatus)])) {
							HTMLString.Append("<option value=");
							HTMLString.Append(iArrayIndex.ToString());
							HTMLString.Append(">");
							HTMLString.Append(sItem);
						}
						iArrayIndex++;
					}
					HTMLString.Append("</select>");

					//Time Of Game
					//HTMLString.Append("&nbsp;<a href=\"ModifyTimeOfGame.aspx?matchcnt=" + iMatchCount.ToString() + "\">");
					HTMLString.Append("&nbsp;");
					if(m_SportsOleReader.IsDBNull(1)) {
						HTMLString.Append("--");
					} else {
						if(sStatus.Equals("0")) {
							HTMLString.Append("0");
						}	else if(sStatus.Equals("1") || sStatus.Equals("3") || sStatus.Equals("5")) {
							dtStart = m_SportsOleReader.GetDateTime(1);
							HTMLString.Append(((int)DateTime.Now.Subtract(dtStart).TotalMinutes).ToString());
						}	else {
							HTMLString.Append("--");
						}
					}
					HTMLString.Append("分</td>");
					//HTMLString.Append("</a>");

					//Match Count (Hidden field)
					iMatchCount = m_SportsOleReader.GetInt32(2);
					HTMLString.Append("<th style=\"background-color:#FFC733\"><input type=\"hidden\" name=\"MatchCount\" value=\"");
					HTMLString.Append(iMatchCount.ToString());
					HTMLString.Append("\">");

					//Alias (Hidden field)
					sAlias = m_SportsOleReader.GetString(3).Trim();
					HTMLString.Append("<input type=\"hidden\" name=\"Alias\" value=\"");
					HTMLString.Append(sAlias);
					HTMLString.Append("\">");

					//League (Hidden field)
					sRecordStr = m_SportsOleReader.GetString(4).Trim();
					HTMLString.Append("<input type=\"hidden\" name=\"League\" value=\"");
					HTMLString.Append(sRecordStr);
					HTMLString.Append("\">");

					//Match Date (Hidden field)
					sRecordStr = m_SportsOleReader.GetString(5);
					HTMLString.Append("<input type=\"hidden\" name=\"MatchDate\" value=\"");
					HTMLString.Append(sRecordStr);
					HTMLString.Append("\">");

					//Match Time (Hidden field)
					sRecordStr = m_SportsOleReader.GetString(6);
					HTMLString.Append("<input type=\"hidden\" name=\"MatchTime\" value=\"");
					HTMLString.Append(sRecordStr);
					HTMLString.Append("\">");

					//Match Field (Hidden field)
					sRecordStr = m_SportsOleReader.GetString(7);
					HTMLString.Append("<input type=\"hidden\" name=\"MatchField\" value=\"");
					HTMLString.Append(sRecordStr);
					HTMLString.Append("\">");

					//Host Name
					sHost = m_SportsOleReader.GetString(8).Trim();
					HTMLString.Append(sHost);
					HTMLString.Append("<input type=\"hidden\" name=\"Host\" value=\"");
					HTMLString.Append(sHost);
					HTMLString.Append("\">");

					//Host Score and its Validity
					sRecordStr = m_SportsOleReader.GetString(9).Trim();
					HTMLString.Append("<input type=\"hidden\" name=\"hostScore\" value=\"");
					HTMLString.Append(sRecordStr);
					HTMLString.Append("\">&nbsp;");
					HTMLString.Append(sRecordStr);
					HTMLString.Append("</th><th style=\"background-color:#FFEFD5\">");

					//Guest Name
					sGuest = m_SportsOleReader.GetString(10).Trim();
					HTMLString.Append(sGuest);
					HTMLString.Append("<input type=\"hidden\" name=\"Guest\" value=\"");
					HTMLString.Append(sGuest);
					HTMLString.Append("\">");

					//Guest Score and its Validity
					sRecordStr = m_SportsOleReader.GetString(11).Trim();
					HTMLString.Append("<input type=\"hidden\" name=\"guestScore\" value=\"");
					HTMLString.Append(sRecordStr);
					HTMLString.Append("\">&nbsp;");
					HTMLString.Append(sRecordStr);
					HTMLString.Append("</th></tr>");

					//new window title
					HTMLString.Append("<title>");
					HTMLString.Append(sAlias);
					HTMLString.Append(" - ");
					HTMLString.Append(sHost);
					HTMLString.Append(" vs ");
					HTMLString.Append(sGuest);
					HTMLString.Append("</title>");
				} else {
					HTMLString.Remove(0,HTMLString.Length);
					HTMLString.Append("<tr><th colspan=\"2\">沒有數據</th></tr>");
				}
				m_SportsOleReader.Close();
				m_SportsDBMgr.Close();
				//m_SportsDBMgr.Dispose();
			} catch(Exception ex) {
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " LiveGoalOneOnly.cs.GetLiveGoal(): " + ex.ToString());
				m_SportsLog.Close();
				HTMLString.Remove(0,HTMLString.Length);
				HTMLString.Append((string)HttpContext.Current.Application["accessErrorMsg"]);
			}

			return HTMLString.ToString();
		}

		public int UpdateLiveGoal(string sMode) {
			const int FIELDLENGTH = 6;
			//const int GOGOKLEAGUELENGTH = 10;
			int iRecUpd = 0;
			int iLiveOdds = 0;
			int iTimeActivated = 0;
			int iTimeDiff = 0;
			string sOriginalStatus;
			string sCurrentTimestamp = null;
			string sBatchJob = null;
			string sStatusStr = null;
			string sMatchDate;
			string sMatchTime;
			string sMatchField;
			string sMatchCount;
			string sAlias;
			string sLeague;
			string sStatus;
			string sHost;
			string sScore_Host;
			string sValidity_Host;
			string sGuest;
			string sScore_Guest;
			string sValidity_Guest;
			string sSongID;
			string sTempItem;
			string sAlert = "0";
			//byte[] arrGoGoKMsgByte;
			char[] delimiter = new char[] {','};
			string[] arrShortStatus;
			string[] arrMsgType;
			string[] arrOddsItems;
			string[] arrSendToPager;
			ArrayList LiveOddsContentAL = new ArrayList(20);
			NameValueCollection songNVC;
			//Replicator GoalReplicator = new Replicator(HttpContext.Current.Application["RepDBConnectionString"].ToString());
			//GoalReplicator.ApplicationType = 1;
			//GoalReplicator.ErrorLogPath = HttpContext.Current.Application["ErrorFilePath"].ToString();
			//SportsMessage object message
			SportsMessage sptMsg = new SportsMessage();

			//Delcare variable used in message notify
 			int iMsgBodyLength;
			string sMsgBody = null;
			string sAlertHost = null;
			string sAlertGuest = null;
			string[] arrQueueNames;
			string[] arrRemotingPath;
			string[] arrMessageTypes;
			byte[] arrByteOfMSMQBody;
			arrQueueNames = (string[])HttpContext.Current.Application["QueueItems"];
			arrRemotingPath = (string[])HttpContext.Current.Application["RemotingItems"];
			arrMessageTypes = (string[])HttpContext.Current.Application["NotifyMessageTypes"];
			MessageClient msgClt = new MessageClient();

			try {
				arrSendToPager = HttpContext.Current.Request.Form["SendToPager"].Split(delimiter);
			} catch(Exception) {
				arrSendToPager = new string[0];
			}

			arrMsgType = (string[])HttpContext.Current.Application["messageType"];
			arrOddsItems = (string[])HttpContext.Current.Application["oddsItemsArray"];

			songNVC = (NameValueCollection)HttpContext.Current.Application["songItems"];
			arrShortStatus = (string[])HttpContext.Current.Application["matchItemsArray"];
			sMatchCount = HttpContext.Current.Request.Form["MatchCount"];
			sAlias = HttpContext.Current.Request.Form["Alias"];
			sLeague = HttpContext.Current.Request.Form["League"];
			sStatus = HttpContext.Current.Request.Form["status"];
			sMatchDate = HttpContext.Current.Request.Form["MatchDate"];
			sMatchTime = HttpContext.Current.Request.Form["MatchTime"];
			sMatchField = HttpContext.Current.Request.Form["MatchField"];
			sHost = HttpContext.Current.Request.Form["Host"];
			sScore_Host = HttpContext.Current.Request.Form["hostScore"];
			sValidity_Host = "1";
			sGuest = HttpContext.Current.Request.Form["Guest"];
			sScore_Guest = HttpContext.Current.Request.Form["guestScore"];
			sValidity_Guest = "1";
			sSongID = songNVC.Keys[0].ToString();

			if(sMatchCount != null) {
				if(!sMatchCount.Equals("")) {
					try {
						if(sMode.Equals("STA")) {	//Update status period
							sOriginalStatus = m_SportsDBMgr.ExecuteQueryString("select STATUS from GOALINFO where MATCH_CNT=" + sMatchCount);
							m_SportsDBMgr.Close();
							iTimeActivated = m_SportsDBMgr.ExecuteScalar("select count(IMATCH_CNT) from TIMEOFGAME_INFO where IMATCH_CNT=" + sMatchCount);
							m_SportsDBMgr.Close();
							if(sStatus.Equals("1")) {	//current status = 上半
								if(sOriginalStatus.Equals("0")) {	//未開 -> 上半
									if(iTimeActivated > 0) {
										m_SportsDBMgr.ExecuteNonQuery("update TIMEOFGAME_INFO set STARTTIME=CURRENT_TIMESTAMP, CSTATUS='" + sStatus + "' where IMATCH_CNT=" + sMatchCount);
									} else {
										m_SportsDBMgr.ExecuteNonQuery("insert into TIMEOFGAME_INFO values(" + sMatchCount + ",CURRENT_TIMESTAMP,'" + sStatus + "')");
									}
									m_SportsDBMgr.Close();
								}
							} else if(sStatus.Equals("3")) {	//current status = 下半
								if(sOriginalStatus.Equals("2")) {	//半休 -> 下半
									if(iTimeActivated > 0) {
										m_SportsDBMgr.ExecuteNonQuery("update TIMEOFGAME_INFO set STARTTIME=CURRENT_TIMESTAMP, CSTATUS='" + sStatus + "' where IMATCH_CNT=" + sMatchCount);
									} else {
										m_SportsDBMgr.ExecuteNonQuery("insert into TIMEOFGAME_INFO values(" + sMatchCount + ",CURRENT_TIMESTAMP,'" + sStatus + "')");
									}
									m_SportsDBMgr.Close();
								}
							} else if(sStatus.Equals("5")) {	//current status = 加時
								if(sOriginalStatus.Equals("3") || sOriginalStatus.Equals("4")) {	//下半 -> 加時 or 完場 -> 加時
									if(iTimeActivated > 0) {
										m_SportsDBMgr.ExecuteNonQuery("update TIMEOFGAME_INFO set STARTTIME=CURRENT_TIMESTAMP, CSTATUS='" + sStatus + "' where IMATCH_CNT=" + sMatchCount);
									} else {
										m_SportsDBMgr.ExecuteNonQuery("insert into TIMEOFGAME_INFO values(" + sMatchCount + ",CURRENT_TIMESTAMP,'" + sStatus + "')");
									}
									m_SportsDBMgr.Close();
								}
							}
						} else if(sMode.Equals("HCG")) {	//Increment Host Goal
							if(sScore_Host.Equals("")) sScore_Host = "0";
							sScore_Host = (Convert.ToInt32(sScore_Host) + 1).ToString();
							sValidity_Host = "0";
						} else if(sMode.Equals("HWG")) {	//Host Goal Invalid
							sValidity_Host = "2";
							sSongID = songNVC.Keys[1].ToString();
							if(!(sScore_Host.Equals("") && sScore_Guest.Equals(""))) {
								if(!(sScore_Host.Equals("0") && sScore_Guest.Equals("0"))) {
									if(Convert.ToInt32(sScore_Host) > 0) {
										sScore_Host = (Convert.ToInt32(sScore_Host) - 1).ToString();
									}
								}
							}
						} else if(sMode.Equals("GCG")) {	//Increment Guest Goal
							if(sScore_Guest.Equals("")) sScore_Guest = "0";
							sScore_Guest = (Convert.ToInt32(sScore_Guest) + 1).ToString();
							sValidity_Guest = "0";
						} else if(sMode.Equals("GWG")) {	//Guest Goal Invalid
							sValidity_Guest = "2";
							sSongID = songNVC.Keys[1].ToString();
							if(!(sScore_Host.Equals("") && sScore_Guest.Equals(""))) {
								if(!(sScore_Host.Equals("0") && sScore_Guest.Equals("0"))) {
									if(Convert.ToInt32(sScore_Guest) > 0) {
										sScore_Guest = (Convert.ToInt32(sScore_Guest) - 1).ToString();
									}
								}
							}
						} else if(sMode.Equals("HR1")) {
							sValidity_Host = "3";
							sSongID = songNVC.Keys[1].ToString();
						} else if(sMode.Equals("HR2")) {
							sValidity_Host = "4";
							sSongID = songNVC.Keys[1].ToString();
						} else if(sMode.Equals("HR3")) {
							sValidity_Host = "5";
							sSongID = songNVC.Keys[1].ToString();
						} else if(sMode.Equals("GR1")) {
							sValidity_Guest = "3";
							sSongID = songNVC.Keys[1].ToString();
						} else if(sMode.Equals("GR2")) {
							sValidity_Guest = "4";
							sSongID = songNVC.Keys[1].ToString();
						} else if(sMode.Equals("GR3")) {
							sValidity_Guest = "5";
							sSongID = songNVC.Keys[1].ToString();
						}

						//update live goal table (goalinfo)
						SQLString.Remove(0,SQLString.Length);
						SQLString.Append("update goalinfo set H_SCR='");
						SQLString.Append(sScore_Host);
						SQLString.Append("',G_SCR='");
						SQLString.Append(sScore_Guest);
						SQLString.Append("',STATUS='");
						SQLString.Append(sStatus);
						SQLString.Append("' where MATCH_CNT=");
						SQLString.Append(sMatchCount);
						m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
						m_SportsDBMgr.Close();
						//GoalReplicator.Replicate(SQLString.ToString());

						if(sScore_Host.Equals("")) sScore_Host = "0";
						if(sScore_Guest.Equals("")) sScore_Guest = "0";
						if(Convert.ToInt32(sValidity_Host)==1 && Convert.ToInt32(sValidity_Guest)==1) sAlert = "0";
						else sAlert = "1";
						sStatusStr = arrShortStatus[Convert.ToInt32(sStatus)];
						//Time Of Game
						m_SportsOleReader = m_SportsDBMgr.ExecuteQuery("select STARTTIME from TIMEOFGAME_INFO where IMATCH_CNT=" + sMatchCount);
						if(m_SportsOleReader.Read()) {
							iTimeDiff = (int)DateTime.Now.Subtract(m_SportsOleReader.GetDateTime(0)).TotalMinutes;
							if(sStatus.Equals("1") || sStatus.Equals("3")) {
								if(iTimeDiff > 44) {
									iTimeDiff = -1;
								}
							} else {
								if(iTimeDiff > 29) {
									iTimeDiff = -1;
								}
							}
						} else {
							iTimeDiff = -1;
						}
						m_SportsOleReader.Close();
						m_SportsDBMgr.Close();

						sCurrentTimestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
						sBatchJob = DateTime.Now.ToString("yyMMddHHmmss_fffffff") + "." + HttpContext.Current.Session["user_name"].ToString() + "." + arrMsgType[1] + ".ini";
						//Insert log into LOG_LIVEGOAL
						SQLString.Remove(0,SQLString.Length);
						SQLString.Append("insert into LOG_LIVEGOAL (TIMEFLAG, SECTION, LEAGUE, HOST, GUEST, ACT, MATCHDATE, MATCHTIME, MATCHFIELD, H_GOAL, G_GOAL, H_CONFIRM, G_CONFIRM, SONGID, ALERT, STATUS, COMMENT, TIMEOFGAME, BATCHJOB) values ('");
						SQLString.Append(sCurrentTimestamp);
						SQLString.Append("','GOAL_','");
						SQLString.Append(sLeague);
						SQLString.Append("','");
						SQLString.Append(sHost);
						SQLString.Append("','");
						SQLString.Append(sGuest);
						SQLString.Append("','U','");
						SQLString.Append(sMatchDate);
						SQLString.Append("','");
						SQLString.Append(sMatchTime);
						SQLString.Append("','");
						SQLString.Append(sMatchField);
						SQLString.Append("',");
						SQLString.Append(sScore_Host);
						SQLString.Append(",");
						SQLString.Append(sScore_Guest);
						SQLString.Append(",");
						SQLString.Append(sValidity_Host);
						SQLString.Append(",");
						SQLString.Append(sValidity_Guest);
						SQLString.Append(",'");
						SQLString.Append(sSongID);
						SQLString.Append("',");
						SQLString.Append(sAlert);
						SQLString.Append(",'");
						SQLString.Append(sStatusStr.Substring(0,1));
						SQLString.Append("','-1',");
						SQLString.Append(iTimeDiff.ToString());
						SQLString.Append(",'");
						SQLString.Append(sBatchJob);
						SQLString.Append("')");
						m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
						m_SportsDBMgr.Close();


						//Tell MessageDispatcher to generate LiveGoal INI and notify other processes
						//Assign value to SportsMessage object
						//modified by Henry, 9 Feb 2004
						sptMsg.IsTransaction = true;
						sptMsg.Body = sBatchJob;
						sptMsg.Timestamp = DateTime.Now.ToString("yyMMddHHmmss");
						sptMsg.AppID = "07";
						sptMsg.MsgID = "01";
						sptMsg.DeviceID = new string[0];
						for(int i = 0; i < arrSendToPager.Length; i++) {
							sptMsg.AddDeviceID((string)arrSendToPager[i]);
						}
						try {
							//Notify via MSMQ
							msgClt.MessageType = arrMessageTypes[0];
							msgClt.MessagePath = arrQueueNames[0];
							msgClt.SendMessage(sptMsg);
						} catch(System.Messaging.MessageQueueException mqEx) {
							try {
								m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
								m_SportsLog.SetFileName(0,LOGFILESUFFIX);
								m_SportsLog.Open();
								m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " MSMQ ERROR: Live Goal");
								m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " LiveGoalOneOnly.cs.UpdateLiveGoals(): Notify (LiveGoal) via MSMQ throws MessageQueueException: " + mqEx.ToString());
								m_SportsLog.Close();

								//If MSMQ fail, notify via .NET Remoting
								msgClt.MessageType = arrMessageTypes[1];
								msgClt.MessagePath = arrRemotingPath[0];
								if(!msgClt.SendMessage((object)sptMsg)) {
									m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
									m_SportsLog.SetFileName(0,LOGFILESUFFIX);
									m_SportsLog.Open();
									m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Remoting ERROR: Live Goal");
									m_SportsLog.Close();
								}
							}	catch(Exception ex) {
								m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
								m_SportsLog.SetFileName(0,LOGFILESUFFIX);
								m_SportsLog.Open();
								m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Remoting ERROR: Live Goal");
								m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " LiveGoalOneOnly.cs.UpdateLiveGoals(): Notify (LiveGoal) via .NET Remoting throws exception: " + ex.ToString());
								m_SportsLog.Close();
							}
						}	catch(Exception ex) {
							m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
							m_SportsLog.SetFileName(0,LOGFILESUFFIX);
							m_SportsLog.Open();
							m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " LiveGoalOneOnly.cs.UpdateLiveGoals(): Notify (LiveGoal) via MSMQ throws exception: " + ex.ToString());
							m_SportsLog.Close();
						}

/*
						//Send GoGoK TCP message to MSMQ server
						sMsgBody = PadRightSpace(sLeague,GOGOKLEAGUELENGTH) + PadRightSpace(sHost,FIELDLENGTH) + PadRightSpace(sGuest,FIELDLENGTH);
						sMsgBody += sScore_Host + sScore_Guest + sValidity_Host + sValidity_Guest;
						sMsgBody += sStatus + sAlert;
						arrGoGoKMsgByte = m_Big5Encoded.GetBytes(sMsgBody);
						iMsgBodyLength = arrGoGoKMsgByte.Length;
						sptMsg.Body = iMsgBodyLength.ToString("D3") + sMsgBody;
						sptMsg.Timestamp = DateTime.Now.ToString("yyMMddHHmmss");
						sptMsg.AppID = "00";
						sptMsg.MsgID = "00";
						//Send GoGoK Message
						try {
							msgClt.MessageType = arrMessageTypes[0];
							msgClt.MessagePath = arrQueueNames[0];
							msgClt.SendMessage(sptMsg);
						} catch(Exception ex) {
							m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
							m_SportsLog.SetFileName(0,LOGFILESUFFIX);
							m_SportsLog.Open();
							m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " LiveGoalOneOnly.cs.UpdateLiveGoals(): Send gogok message throws exception: " + ex.ToString());
							m_SportsLog.Close();
						}
*/


						//Mango message format: AppID MsgID Priority AlertID MsgLen Alias Host Guest MatchStatus GoalType FreeText
						//Alias: Use Asia format (Alias, within 6 bytes, is not available in HKJC; HKJC short form has max. 12 bytes)
						//Host: Use HKJC format
						//Guest: Use HKJC format
						sAlertHost = "";
						sAlertHost = m_SportsDBMgr.ExecuteQueryString("select CHKJCSHORTNAME from TEAMINFO where TEAMNAME='" + sHost + "'");
						m_SportsDBMgr.Close();
						if(sAlertHost.Equals("")) sAlertHost = sHost;
						sAlertGuest = "";
						sAlertGuest = m_SportsDBMgr.ExecuteQueryString("select CHKJCSHORTNAME from TEAMINFO where TEAMNAME='" + sGuest + "'");
						m_SportsDBMgr.Close();
						if(sAlertGuest.Equals("")) sAlertGuest = sGuest;
						//sMsgBody = PadRightSpace(sAlias,FIELDLENGTH) + PadRightSpace(sAlertHost,FIELDLENGTH) + PadRightSpace(sAlertGuest,FIELDLENGTH) + sStatus;
						sMsgBody = PadRightSpace(sAlias,FIELDLENGTH) + PadRightSpace(sAlertHost,FIELDLENGTH) + PadRightSpace(sAlertGuest,FIELDLENGTH) + PadRightSpace(sStatus,2);
						if(Convert.ToInt32(sValidity_Host) > 2) {	//Host red card
							sptMsg.AppID = "01";
							sptMsg.MsgID = "02";
							//sMsgBody += (Convert.ToInt32(sValidity_Host) - 2).ToString() + sHost;
							sMsgBody += (Convert.ToInt32(sValidity_Host) - 2).ToString() + "Home";
						} else if(Convert.ToInt32(sValidity_Guest) > 2) {		//Guest red card
							sptMsg.AppID = "01";
							sptMsg.MsgID = "02";
							//sMsgBody += (Convert.ToInt32(sValidity_Guest) - 2).ToString() + sGuest;
							sMsgBody += (Convert.ToInt32(sValidity_Guest) - 2).ToString() + "Away";
						} else {	//Score
							sptMsg.AppID = "01";
							sptMsg.MsgID = "01";
							if(Convert.ToInt32(sValidity_Host) == 2) {	//Host false goal
								sMsgBody += sValidity_Host;
								sMsgBody += sScore_Host + ":" + sScore_Guest;
								//sMsgBody += arrHost[iUpdIndex] + "(詐糊)";
								sMsgBody += " Home False Goal";
							} else if(Convert.ToInt32(sValidity_Guest) == 2) {	//Guest false goal
								sMsgBody += sValidity_Guest;
								sMsgBody += sScore_Host + ":" + sScore_Guest;
								//sMsgBody += arrGuest[iUpdIndex] + "(詐糊)";
								sMsgBody += " Away False Goal";
							} else if(Convert.ToInt32(sValidity_Host) == 0) {	//Host invalid goal
								sMsgBody += sValidity_Host;
								sMsgBody += sScore_Host + ":" + sScore_Guest;
							} else if(Convert.ToInt32(sValidity_Guest) == 0) {	//Guest invalid goal
								sMsgBody += sValidity_Guest;
								sMsgBody += sScore_Host + ":" + sScore_Guest;
							} else {	//Valid goal
								sMsgBody += "1";
								sMsgBody += sScore_Host + ":" + sScore_Guest;
							}
/*
							sMsgBody += sValidity_Host;
							if(Convert.ToInt32(sValidity_Host) == 2) {
								//sMsgBody += sHost + "(詐糊)";
								sMsgBody += "Home False Goal ";
							} else if(Convert.ToInt32(sValidity_Guest) == 2) {
								//sMsgBody += sGuest + "(詐糊)";
								sMsgBody += "Away False Goal ";
							}
							sMsgBody += sScore_Host + ":" + sScore_Guest;
*/
						}

						if(sStatus.Equals("10")) {	//current status = 待定
							sMsgBody += " Unconfirmed";
						}

						arrByteOfMSMQBody = m_Big5Encoded.GetBytes(sMsgBody);
						iMsgBodyLength = arrByteOfMSMQBody.Length;
						sptMsg.Body = iMsgBodyLength.ToString("D3") + sMsgBody;
						sptMsg.Timestamp = DateTime.Now.ToString("yyMMddHHmmss");
						//Send Mango Message
						try {
							//Notify via MSMQ
							msgClt.MessageType = arrMessageTypes[0];
							//msgClt.MessagePath = arrQueueNames[0];	//Original message queue
							msgClt.MessagePath = arrQueueNames[1];	//MangoSports Goal Alert Queue
							msgClt.SendMessage(sptMsg);
						} catch(System.Messaging.MessageQueueException mqEx) {
							try {
								m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
								m_SportsLog.SetFileName(0,LOGFILESUFFIX);
								m_SportsLog.Open();
								m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " MSMQ ERROR: Live Goal - Mango");
								m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " LiveGoalOneOnly.cs.UpdateLiveGoals(): Notify (LiveGoal - Mango) via MSMQ throws MessageQueueException: " + mqEx.ToString());
								m_SportsLog.Close();

								//If MSMQ fail, notify via .NET Remoting
								msgClt.MessageType = arrMessageTypes[1];
								msgClt.MessagePath = arrRemotingPath[1];
								if(!msgClt.SendMessage((object)sptMsg)) {
									m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
									m_SportsLog.SetFileName(0,LOGFILESUFFIX);
									m_SportsLog.Open();
									m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Remoting ERROR: Live Goal - Mango");
									m_SportsLog.Close();
								}
							}	catch(Exception ex) {
								m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
								m_SportsLog.SetFileName(0,LOGFILESUFFIX);
								m_SportsLog.Open();
								m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Remoting ERROR: Live Goal - Mango");
								m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " LiveGoalOneOnly.cs.UpdateLiveGoals(): Notify (LiveGoal - Mango) via .NET Remoting throws exception: " + ex.ToString());
								m_SportsLog.Close();
							}
						}	catch(Exception ex) {
							m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
							m_SportsLog.SetFileName(0,LOGFILESUFFIX);
							m_SportsLog.Open();
							m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " LiveGoalOneOnly.cs.UpdateLiveGoals(): Notify (LiveGoal - Mango) via MSMQ throws exception: " + ex.ToString());
							m_SportsLog.Close();
						}



						if(!sMode.Equals("STA")) {
							//update live odds table (hlinfo)
							SQLString.Remove(0,SQLString.Length);
							SQLString.Append("select CALIAS, CMATCHFIELD, CHOST_HANDI, CMATCHSTATUS, ITIMEOFGAME, CHANDICAP1, CHANDICAP2, CHODDS, IREGION_ID, CH_WEAR, CG_WEAR, CTOTALSCORE, CBIGODDS, CSMALLODDS from LIVEODDS_INFO where CLEAGUE='");
							SQLString.Append(sLeague);
							SQLString.Append("' and CHOST='");
							SQLString.Append(sHost);
							SQLString.Append("' and CGUEST='");
							SQLString.Append(sGuest);
							SQLString.Append("' order by IREGION_ID");
							m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
							while(m_SportsOleReader.Read()) {	//if record existed, prepare liveOdds's INI content
								iLiveOdds++;

								LiveOddsContentAL.Add(m_SportsOleReader.GetString(0).Trim());
								LiveOddsContentAL.Add(sLeague);
								LiveOddsContentAL.Add(sHost);
								LiveOddsContentAL.Add(sGuest);
								LiveOddsContentAL.Add(m_SportsOleReader.GetString(1));
								LiveOddsContentAL.Add(m_SportsOleReader.GetString(2));
								LiveOddsContentAL.Add(m_SportsOleReader.GetString(3));
								LiveOddsContentAL.Add("U");
								LiveOddsContentAL.Add(sScore_Host);
								LiveOddsContentAL.Add(sScore_Guest);

								if(m_SportsOleReader.IsDBNull(4)) {
									sTempItem = "-1";
								} else {
									sTempItem = m_SportsOleReader.GetInt32(4).ToString();
									if(sTempItem.Equals("")) {
										sTempItem = "-1";
									}
								}
								LiveOddsContentAL.Add(sTempItem);

								if(m_SportsOleReader.IsDBNull(5)) {
									sTempItem = "-1";
								} else {
									sTempItem = m_SportsOleReader.GetString(5).Trim();
									if(sTempItem.Equals("")) {
										sTempItem = "-1";
									}
								}
								if(!m_SportsOleReader.IsDBNull(6)) {
									if(m_SportsOleReader.GetString(6).Trim() != "") sTempItem += "/" + m_SportsOleReader.GetString(6).Trim();
								}
								LiveOddsContentAL.Add(sTempItem);

								if(m_SportsOleReader.IsDBNull(7)) {
									sTempItem = "-1";
								} else {
									sTempItem = m_SportsOleReader.GetString(7).Trim();
									if(sTempItem.Equals("")) {
										sTempItem = "-1";
									}
								}
								LiveOddsContentAL.Add(sTempItem);
								LiveOddsContentAL.Add(arrOddsItems[2]);
								LiveOddsContentAL.Add("0");

								//Region ID
								LiveOddsContentAL.Add(m_SportsOleReader.GetInt32(8).ToString());

								//Host, Guest Wear
								if(m_SportsOleReader.IsDBNull(9)) {
									sTempItem = "-1";
								} else {
									sTempItem = m_SportsOleReader.GetString(9).Trim();
									if(sTempItem.Equals("")) {
										sTempItem = "-1";
									}
								}
								LiveOddsContentAL.Add(sTempItem);
								if(m_SportsOleReader.IsDBNull(10)) {
									sTempItem = "-1";
								} else {
									sTempItem = m_SportsOleReader.GetString(10).Trim();
									if(sTempItem.Equals("")) {
										sTempItem = "-1";
									}
								}
								LiveOddsContentAL.Add(sTempItem);

								//Total Score
								if(m_SportsOleReader.IsDBNull(11)) {
									sTempItem = "-1";
								} else {
									sTempItem = m_SportsOleReader.GetString(11).Trim();
									if(sTempItem.Equals("")) {
										sTempItem = "-1";
									}
								}
								LiveOddsContentAL.Add(sTempItem);

								//BigSmall Odds
								sTempItem = "";
								if(!m_SportsOleReader.IsDBNull(12)) {
									if(!m_SportsOleReader.GetString(12).Trim().Equals("")) {
										sTempItem = m_SportsOleReader.GetString(12).Trim();
									}
								}
								if(!m_SportsOleReader.IsDBNull(13)) {
									if(!m_SportsOleReader.GetString(13).Trim().Equals("")) {
										sTempItem += "/" + m_SportsOleReader.GetString(13).Trim();
									}
								}
								if(sTempItem.Equals("")) sTempItem = "-1";
								LiveOddsContentAL.Add(sTempItem);
							}
							m_SportsOleReader.Close();
							m_SportsDBMgr.Close();
							LiveOddsContentAL.TrimToSize();
						}

						if(iLiveOdds > 0) {	//update live odds
							SQLString.Remove(0,SQLString.Length);
							SQLString.Append("update LIVEODDS_INFO set CODDSSTATUS='2', IH_SCORE=");
							SQLString.Append(sScore_Host);
							SQLString.Append(", IG_SCORE=");
							SQLString.Append(sScore_Guest);
							SQLString.Append(" where CLEAGUE='");
							SQLString.Append(sLeague);
							SQLString.Append("' and CHOST='");
							SQLString.Append(sHost);
							SQLString.Append("' and CGUEST='");
							SQLString.Append(sGuest);
							SQLString.Append("'");
							m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
							m_SportsDBMgr.Close();
							//GoalReplicator.Replicate(SQLString.ToString());

							sCurrentTimestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
							sBatchJob = DateTime.Now.ToString("yyMMddHHmmss_fffffff") + "." + HttpContext.Current.Session["user_name"].ToString() + "." + arrMsgType[20] + ".ini";
							for(int i = 0; i < iLiveOdds; i++) {
								SQLString.Remove(0,SQLString.Length);
								SQLString.Append("insert into LOG_LIVEODDS (TIMEFLAG, SECTION, LEAGUEALIAS, LEAGUE, HOST, GUEST, MATCHFIELD, HANDICAP, MATCHSTATUS, ACT, H_GOAL, G_GOAL, CURRTIME, HANDI, ODDS, ODDSSTATUS, ALERT, REGIONID, HOSTWEAR, GUESTWEAR, TOTALSCORE, BIGSMALLODDS, BATCHJOB) values ('");
								SQLString.Append(sCurrentTimestamp);
								SQLString.Append("','LIVE_','");
								SQLString.Append(LiveOddsContentAL[(20*i)]);
								SQLString.Append("','");
								SQLString.Append(LiveOddsContentAL[(20*i+1)]);
								SQLString.Append("','");
								SQLString.Append(LiveOddsContentAL[(20*i+2)]);
								SQLString.Append("','");
								SQLString.Append(LiveOddsContentAL[(20*i+3)]);
								SQLString.Append("','");
								SQLString.Append(LiveOddsContentAL[(20*i+4)]);
								SQLString.Append("','");
								SQLString.Append(LiveOddsContentAL[(20*i+5)]);
								SQLString.Append("','");
								SQLString.Append(LiveOddsContentAL[(20*i+6)]);
								SQLString.Append("','");
								SQLString.Append(LiveOddsContentAL[(20*i+7)]);
								SQLString.Append("',");
								SQLString.Append(LiveOddsContentAL[(20*i+8)]);
								SQLString.Append(",");
								SQLString.Append(LiveOddsContentAL[(20*i+9)]);
								SQLString.Append(",");
								SQLString.Append(LiveOddsContentAL[(20*i+10)]);
								SQLString.Append(",'");
								SQLString.Append(LiveOddsContentAL[(20*i+11)]);
								SQLString.Append("','");
								SQLString.Append(LiveOddsContentAL[(20*i+12)]);
								SQLString.Append("','");
								SQLString.Append(LiveOddsContentAL[(20*i+13)]);
								SQLString.Append("',");
								SQLString.Append(LiveOddsContentAL[(20*i+14)]);
								SQLString.Append(",");
								SQLString.Append(LiveOddsContentAL[(20*i+15)]);
								SQLString.Append(",'");
								SQLString.Append(LiveOddsContentAL[(20*i+16)]);
								SQLString.Append("','");
								SQLString.Append(LiveOddsContentAL[(20*i+17)]);
								SQLString.Append("','");
								SQLString.Append(LiveOddsContentAL[(20*i+18)]);
								SQLString.Append("','");
								SQLString.Append(LiveOddsContentAL[(20*i+19)]);
								SQLString.Append("','");
								SQLString.Append(sBatchJob);
								SQLString.Append("')");
								m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
								m_SportsDBMgr.Close();
							}

							//Tell MessageDispatcher to generate LiveOdds INI (if existed) and notify other processes
							//Assign value to SportsMessage object
							//modified by Henry, 9 Feb 2004
							sptMsg.IsTransaction = true;
							sptMsg.Body = sBatchJob;
							sptMsg.Timestamp = DateTime.Now.ToString("yyMMddHHmmss");
							sptMsg.AppID = "07";
							sptMsg.MsgID = "06";
							sptMsg.DeviceID = new string[0];
							for(int i = 0; i < arrSendToPager.Length; i++) {
								sptMsg.AddDeviceID((string)arrSendToPager[i]);
							}
							try {
								//Notify via MSMQ
								msgClt.MessageType = arrMessageTypes[0];
								msgClt.MessagePath = arrQueueNames[0];
								msgClt.SendMessage(sptMsg);
							} catch(System.Messaging.MessageQueueException mqEx) {
								try {
									m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
									m_SportsLog.SetFileName(0,LOGFILESUFFIX);
									m_SportsLog.Open();
									m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " MSMQ ERROR: Live Odds");
									m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " LiveGoalOneOnly.cs.UpdateLiveGoals(): Notify (LiveOdds) via MSMQ throws MessageQueueException: " + mqEx.ToString());
									m_SportsLog.Close();

									//If MSMQ fail, notify via .NET Remoting
									msgClt.MessageType = arrMessageTypes[1];
									msgClt.MessagePath = arrRemotingPath[0];
									if(!msgClt.SendMessage((object)sptMsg)) {
										m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
										m_SportsLog.SetFileName(0,LOGFILESUFFIX);
										m_SportsLog.Open();
										m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Remoting ERROR: Live Odds");
										m_SportsLog.Close();
									}
								}	catch(Exception ex) {
									m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
									m_SportsLog.SetFileName(0,LOGFILESUFFIX);
									m_SportsLog.Open();
									m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Remoting ERROR: Live Odds");
									m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " LiveGoalOneOnly.cs.UpdateLiveGoals(): Notify (LiveOdds) via .NET Remoting throws exception: " + ex.ToString());
									m_SportsLog.Close();
								}
							}	catch(Exception ex) {
								m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
								m_SportsLog.SetFileName(0,LOGFILESUFFIX);
								m_SportsLog.Open();
								m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " LiveGoalOneOnly.cs.UpdateLiveGoals(): Notify (LiveOdds) via MSMQ throws exception: " + ex.ToString());
								m_SportsLog.Close();
							}
						}
						//end modify
						iRecUpd++;
					} catch(Exception ex) {
						iRecUpd = -1;
						m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
						m_SportsLog.SetFileName(0,LOGFILESUFFIX);
						m_SportsLog.Open();
						m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " LiveGoalOneOnly.cs.UpdateLiveGoal(): " + ex.ToString());
						m_SportsLog.Close();
					}
				}
			} else {
				iRecUpd = 0;
			}
/*
			if(iRecUpd > 0) {
				GoalReplicator.Dispose();
				m_SportsDBMgr.Dispose();
			}
*/
/*
			//write log
			m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
			m_SportsLog.SetFileName(0,LOGFILESUFFIX);
			m_SportsLog.Open();
			m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " LiveGoalOneOnly.cs: Send " + iRecUpd.ToString() + " goal (" + HttpContext.Current.Session["user_name"] + ")");
			m_SportsLog.Close();
*/
			return iRecUpd;
		}

		private string PadRightSpace(string sItem, int iSpaceCount) {
			int iLen;
			byte[] arrItemByte;
			StringBuilder sbRefined;
			if(sItem != null) {
				sbRefined = new StringBuilder(sItem);
				arrItemByte = m_Big5Encoded.GetBytes(sItem);
				iLen = arrItemByte.Length;
				sbRefined.Append(' ',iSpaceCount-iLen);
			} else {
				sbRefined = new StringBuilder(new String(' ',iSpaceCount));
			}

			return sbRefined.ToString();
		}

	}
}