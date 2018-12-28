/*
Objective:
Update live goal for all matches and send related info to MSMQ server

Last updated:
15 Nov 2004, (Fanny Cheung) Increase orderId into 3 digits
25 Oct 2004, (Fanny Cheung) Change status field into 2 char in HKJC format for Goal Alert
1 Sept 2004, (Fanny) no need to append goal remark to mango goal alert message
9 Aug 2004, Chapman Choi
9 Aug 2004 Convert league and tem to HKJC format for Goal Alert
31 May 2004 Send MangoSports Goal Alert to a new queue
23 Mar 2004 Remark Replicator code
26 Feb 2004 Additional match status for liveodds

C#.NET complier statement:
(With Replicator) csc /t:library /out:..\bin\LiveGoal.dll /r:..\bin\DBManager.dll;..\bin\Files.dll;..\bin\MessageClient.dll;..\bin\Replicator.dll;..\bin\SportsMessage.dll LiveGoal.cs

Without Replicator (Current production):
csc /t:library /out:..\bin\LiveGoal.dll /r:..\bin\DBManager.dll;..\bin\Files.dll;..\bin\MessageClient.dll;..\bin\SportsMessage.dll LiveGoal.cs
*/

using System;
using System.Collections;
using System.Collections.Specialized;
using System.Data.OleDb;
using System.Reflection;
using System.Text;
using System.Web;
using TDL.DB;
using TDL.IO;
using TDL.Message;
//using TDL.Util;

[assembly:AssemblyCompany("TDSL")]
[assembly:AssemblyCopyright("(c) 2003 TDSL. All rights reserved.")]
[assembly:AssemblyDescription("足球資訊 -> 現場比數")]
[assembly:AssemblyProduct("Sports.NET")]
[assembly:AssemblyTitle("Sports.NET DLL")]
[assembly:AssemblyVersion("3.1.*")]
namespace SportsUtil {
	public class LiveGoal {
		const string LOGFILESUFFIX = "log";
		int m_RecordCount = 0;
		OleDbDataReader m_SportsOleReader;
		DBManager m_SportsDBMgr;
		Files m_SportsLog;
		Encoding m_Big5Encoded;
		StringBuilder SQLString;
		ArrayList leagueList;

		public LiveGoal(string Connection) {
			m_SportsDBMgr = new DBManager();
			m_SportsDBMgr.ConnectionString = Connection;
			m_SportsLog = new Files();
			m_Big5Encoded = Encoding.GetEncoding(950);
			SQLString = new StringBuilder();
			leagueList = new ArrayList();
		}

		public int NumberOfRecords {
			get {
				return m_RecordCount;
			}
		}

		public ArrayList LeagueList {
			get {
				return leagueList;
			}
		}

		public string GetLiveGoals() {
			int iLeagueGp = -1;
			int iMatchCount;
			int iArrayIndex = 0;
			string uid, sSortType;
			string sRecordStr, sNVCID;
			string sMatchStatus;
			string[] statusArray, validityArray;
			NameValueCollection songNVC;
			DateTime dtStart;
			StringBuilder HTMLString = new StringBuilder();

			sSortType = HttpContext.Current.Request.QueryString["sort"];
			if(sSortType != null) {
				if(sSortType.Trim().Equals("")) {
					sSortType = (string)HttpContext.Current.Session["user_sortType"];
				} else {
					HttpContext.Current.Session["user_sortType"] = sSortType;
				}
			} else {
				sSortType = (string)HttpContext.Current.Session["user_sortType"];
			}
			statusArray = (string[])HttpContext.Current.Application["matchItemsArray"];
			validityArray = (string[])HttpContext.Current.Application["goalItemsArray"];
			songNVC = (NameValueCollection)HttpContext.Current.Application["songItems"];
			uid = HttpContext.Current.Session["user_id"].ToString();

			//League Count
			SQLString.Remove(0,SQLString.Length);
			SQLString.Append("select distinct gameinfo.league from gameinfo, goalinfo, leaginfo where gameinfo.match_cnt = goalinfo.match_cnt and gameinfo.league = leaginfo.alias and goalinfo.webact = 'V' and goalinfo.act='U' and leaginfo.leag_id in (select cleag_id from userprofile_info where iuser_id=");
			SQLString.Append(uid);
			SQLString.Append(") order by leaginfo.leag_order");
			m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
			while(m_SportsOleReader.Read()) {
				leagueList.Add(m_SportsOleReader.GetString(0).Trim());
			}
			m_SportsOleReader.Close();
			m_SportsDBMgr.Close();
			leagueList.TrimToSize();

			SQLString.Remove(0,SQLString.Length);
			SQLString.Append("select goalinfo.match_cnt, goalinfo.order_id, gameinfo.matchdate, gameinfo.matchtime, gameinfo.field, goalinfo.status, gameinfo.league, gameinfo.leaglong, gameinfo.host, goalinfo.h_scr, gameinfo.guest, goalinfo.g_scr, (select STARTTIME from TIMEOFGAME_INFO where IMATCH_CNT=goalinfo.match_cnt) STARTTIME, goalinfo.remark from gameinfo, goalinfo, leaginfo ");
			SQLString.Append("where gameinfo.match_cnt = goalinfo.match_cnt and gameinfo.league = leaginfo.alias and goalinfo.webact = 'V' and goalinfo.act='U' ");
			SQLString.Append("and leaginfo.leag_id in (select cleag_id from userprofile_info where iuser_id=");
			SQLString.Append(uid);
			SQLString.Append(") ");
			if(sSortType.Equals("0")) {		//order by league and then match date and time, order_id
				SQLString.Append("order by leaginfo.ALIAS, gameinfo.matchdate, gameinfo.matchtime, goalinfo.order_id");
			} else {		//order by order id
				SQLString.Append("order by goalinfo.order_id, gameinfo.matchdate, gameinfo.matchtime, leaginfo.ALIAS");
			}

			try {
				m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
				while(m_SportsOleReader.Read()) {
					HTMLString.Append("<tr align=\"center\">");

					//Order ID and Match Count
					iMatchCount = m_SportsOleReader.GetInt32(0);

					if(m_SportsOleReader.IsDBNull(1)) {
						sRecordStr = "";
					} else {
						sRecordStr = m_SportsOleReader.GetString(1).Trim();
						if(!sRecordStr.Equals("")) sRecordStr = Convert.ToInt32(sRecordStr).ToString();
					}
					HTMLString.Append("<td><input type=\"text\" name=\"orderID\" maxlength=\"3\" size=\"1\" value=\"");
					HTMLString.Append(sRecordStr);
					HTMLString.Append("\" onChange=\"onOrder_IDChanged(");
					HTMLString.Append(m_RecordCount.ToString());
					HTMLString.Append(")\" tabindex=\"");
					HTMLString.Append((m_RecordCount+1).ToString());
					HTMLString.Append("\"><input type=\"hidden\" name=\"MatchCount\" value=\"");
					HTMLString.Append(iMatchCount.ToString());
					HTMLString.Append("\"></td>");

					//Match Date
					sRecordStr = m_SportsOleReader.GetString(2);
					HTMLString.Append("<td><input type=\"hidden\" name=\"MatchDate\" value=\"");
					HTMLString.Append(sRecordStr);
					HTMLString.Append("\"><font size=\"1\">");
					sRecordStr = sRecordStr.Insert(4,"/");
					sRecordStr = sRecordStr.Insert(7,"/");
					HTMLString.Append(sRecordStr);
					HTMLString.Append("</font></td>");

					//Match Time
					sRecordStr = m_SportsOleReader.GetString(3);
					HTMLString.Append("<td><input type=\"hidden\" name=\"MatchTime\" value=\"");
					HTMLString.Append(sRecordStr);
					HTMLString.Append("\"><font size=\"1\">");
					sRecordStr = sRecordStr.Insert(2,":");
					HTMLString.Append(sRecordStr);
					HTMLString.Append("</font>");

					//Match Field (Hidden field)
					sRecordStr = m_SportsOleReader.GetString(4);
					HTMLString.Append("<input type=\"hidden\" name=\"MatchField\" value=\"");
					HTMLString.Append(sRecordStr);
					HTMLString.Append("\"></td>");

					//Match Status
					iArrayIndex = 0;
					sMatchStatus = m_SportsOleReader.GetString(5);
					HTMLString.Append("<td><select name=\"status\" onChange=\"onStatusChanged(");
					HTMLString.Append(m_RecordCount.ToString());
					HTMLString.Append(")\"><option value=");
					HTMLString.Append(sMatchStatus);
					HTMLString.Append(">");
					HTMLString.Append(statusArray[Convert.ToInt32(sMatchStatus)]);
					foreach(String sItem in statusArray) {
						if(!sItem.Equals(statusArray[Convert.ToInt32(sMatchStatus)])) {
							HTMLString.Append("<option value=");
							HTMLString.Append(iArrayIndex.ToString());
							HTMLString.Append(">");
							HTMLString.Append(sItem);
						}
						iArrayIndex++;
					}
					HTMLString.Append("</select></td>");

					//Alias
					sRecordStr = m_SportsOleReader.GetString(6).Trim();
					iLeagueGp = leagueList.IndexOf(sRecordStr);
					HTMLString.Append("<td><font size=\"1\"><a href=\"javascript:openOneGoal('");
					HTMLString.Append(iMatchCount.ToString());
					HTMLString.Append("','oneGoalWin_");
					HTMLString.Append(iMatchCount.ToString());
					HTMLString.Append("')\">");
					HTMLString.Append(sRecordStr);
					HTMLString.Append("</a></font><input type=\"hidden\" name=\"Alias\" value=\"");
					HTMLString.Append(sRecordStr);
					HTMLString.Append("\"><input type=\"hidden\" name=\"League\" value=\"");
					HTMLString.Append(m_SportsOleReader.GetString(7).Trim());
					HTMLString.Append("\"></td>");

					//Host Name
					sRecordStr = m_SportsOleReader.GetString(8).Trim();
					HTMLString.Append("<td>");
					HTMLString.Append(sRecordStr);
					HTMLString.Append("<input type=\"hidden\" name=\"Host\" value=\"");
					HTMLString.Append(sRecordStr);
					HTMLString.Append("\"></td>");

					//Host Score and its Validity
					sRecordStr = m_SportsOleReader.GetString(9).Trim();
					HTMLString.Append("<td><input type=\"text\" name=\"hostScore\" maxlength=\"2\" size=\"2\" value=\"");
					HTMLString.Append(sRecordStr);
					HTMLString.Append("\" onChange=\"onHost_ScoreChanged(");
					HTMLString.Append(m_RecordCount.ToString());
					HTMLString.Append(")\"> ");

					iArrayIndex = 0;
					HTMLString.Append("<select name=\"hostValidity\" onChange=\"onHost_VChanged(");
					HTMLString.Append(m_RecordCount.ToString());
					HTMLString.Append(")\"><option value=1>");
					HTMLString.Append(validityArray[1]);
					foreach(String sItem in validityArray) {
						if(!sItem.Equals(validityArray[1])) {
							HTMLString.Append("<option value=");
							HTMLString.Append(iArrayIndex.ToString());
							HTMLString.Append(">");
							HTMLString.Append(sItem);
						}
						iArrayIndex++;
					}
					HTMLString.Append("</select></td>");

					//Guest Name
					sRecordStr = m_SportsOleReader.GetString(10).Trim();
					HTMLString.Append("<td>");
					HTMLString.Append(sRecordStr);
					HTMLString.Append("<input type=\"hidden\" name=\"Guest\" value=\"");
					HTMLString.Append(sRecordStr);
					HTMLString.Append("\"></td>");

					//Guest Score and its Validity
					sRecordStr = m_SportsOleReader.GetString(11).Trim();
					HTMLString.Append("<td><input type=\"text\" name=\"guestScore\" maxlength=\"2\" size=\"2\" value=\"");
					HTMLString.Append(sRecordStr);
					HTMLString.Append("\" onChange=\"onGuest_ScoreChanged(");
					HTMLString.Append(m_RecordCount.ToString());
					HTMLString.Append(")\"> ");

					iArrayIndex = 0;
					HTMLString.Append("<select name=\"guestValidity\" onChange=\"onGuest_VChanged(");
					HTMLString.Append(m_RecordCount.ToString());
					HTMLString.Append(")\"><option value=1>");
					HTMLString.Append(validityArray[1]);
					foreach(String sItem in validityArray) {
						if(!sItem.Equals(validityArray[1])) {
							HTMLString.Append("<option value=");
							HTMLString.Append(iArrayIndex.ToString());
							HTMLString.Append(">");
							HTMLString.Append(sItem);
						}
						iArrayIndex++;
					}
					HTMLString.Append("</select></td>");

					//Time Of Game
					HTMLString.Append("<td><font size=\"1\"><a href=\"ModifyTimeOfGame.aspx?matchcnt=");
					HTMLString.Append(iMatchCount.ToString());
					HTMLString.Append("\">");
					if(m_SportsOleReader.IsDBNull(12)) {
						HTMLString.Append("--");
					} else {
						if(sMatchStatus.Equals("0")) {
							HTMLString.Append("0");
						}	else if(sMatchStatus.Equals("1") || sMatchStatus.Equals("3") || sMatchStatus.Equals("5")) {
							dtStart = m_SportsOleReader.GetDateTime(12);
							HTMLString.Append((int)DateTime.Now.Subtract(dtStart).TotalMinutes);
						}	else {
							HTMLString.Append("--");
						}
					}
					HTMLString.Append("</a></font></td>");

					//Song ID
					HTMLString.Append("<td><select name=\"song\" onChange=\"onSongChanged(");
					HTMLString.Append(m_RecordCount.ToString());
					HTMLString.Append(")\">");
					for(iArrayIndex = 0;iArrayIndex<songNVC.Count;iArrayIndex++) {
						sNVCID = songNVC.Keys[iArrayIndex].ToString();
						HTMLString.Append("<option value=\"");
						HTMLString.Append(sNVCID);
						HTMLString.Append("\">");
						HTMLString.Append(songNVC.Get(iArrayIndex).ToString());
					}
					HTMLString.Append("</select></td>");

					//Remark
					if(m_SportsOleReader.IsDBNull(13)) sRecordStr = "";
					else sRecordStr = m_SportsOleReader.GetString(13).Trim();
					HTMLString.Append("<td><input type=\"text\" name=\"remark\" maxlength=\"9\" size=\"5\" value=\"");
					HTMLString.Append(sRecordStr);
					HTMLString.Append("\" onChange=\"onRemarkChanged(");
					HTMLString.Append(m_RecordCount.ToString());
					HTMLString.Append(")\"></td>");

					//Goal Alert
					HTMLString.Append("<td><input type=\"checkbox\" name=\"alertChk\" onClick=\"onAlertClicked(");
					HTMLString.Append(m_RecordCount.ToString());
					HTMLString.Append(")\" value=\"");
					HTMLString.Append(m_RecordCount.ToString());
					HTMLString.Append("\"></td>");

					//MUST Send
					HTMLString.Append("<td style=\"background-color:#FFFAF0\"><input type=\"checkbox\" name=\"MUSTSendChk\" value=\"");
					HTMLString.Append(m_RecordCount.ToString());
					HTMLString.Append("\"></td>");

					//Hidden button
					HTMLString.Append("<td style=\"background-color:#FFC39C\"><input type=\"checkbox\" name=\"hiddenChk\" value=\"");
					HTMLString.Append(m_RecordCount.ToString());
					HTMLString.Append("\" >");

					//Hidden checkbox for league checked
					HTMLString.Append("<input type=\"hidden\" name=\"LeagueChk_");
					HTMLString.Append(iLeagueGp.ToString());
					HTMLString.Append("\" value=\"");
					HTMLString.Append(m_RecordCount.ToString());
					HTMLString.Append("\"></td></tr>");

					m_RecordCount++;
				}
				m_SportsOleReader.Close();
				m_SportsDBMgr.Close();
				//m_SportsDBMgr.Dispose();
				HTMLString.Append("<input type=\"hidden\" name=\"RecordCount\" value=\"");
				HTMLString.Append(m_RecordCount.ToString());
				HTMLString.Append("\">");
			} catch(Exception ex) {
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " LiveGoal.cs.GetLiveGoals(): " + ex.ToString());
				m_SportsLog.Close();
				HTMLString.Remove(0,HTMLString.Length);
				HTMLString.Append((string)HttpContext.Current.Application["accessErrorMsg"]);
			}

			return HTMLString.ToString();
		}

		public int UpdateLiveGoals(string sType) {
			int iUpdIndex = 0;
			int iTempItem = 0;
			int iTimeDiff = 0;
			int iRecIndex, iMustSendLen, iHiddenLen;
			int iLiveOdds = 0;
			int iLiveOddsExisted = 0;
			string sOrder = "", sTempItem;
			string sStatus = null;
			string sRemarks = null;
			char[] delimiter = new char[] {','};
			string[] arrMustSend, arrOrderID, arrMatchCnt, arrHidden;
			string[] arrSendToPager;
			//Replicator GoalReplicator = new Replicator(HttpContext.Current.Application["RepDBConnectionString"].ToString());
			//GoalReplicator.ApplicationType = 1;
			//GoalReplicator.ErrorLogPath = HttpContext.Current.Application["ErrorFilePath"].ToString();

			//SportsMessage object message
			SportsMessage sptMsg = new SportsMessage();

			try {
				arrSendToPager = HttpContext.Current.Request.Form["SendToPager"].Split(delimiter);
			} catch(Exception) {
				arrSendToPager = new string[0];
			}

			try {
				arrOrderID = HttpContext.Current.Request.Form["orderID"].Split(delimiter);
				arrMatchCnt = HttpContext.Current.Request.Form["MatchCount"].Split(delimiter);
			} catch(Exception) {
				arrOrderID = new string[0];
				arrMatchCnt = new string[0];
			}

			try {
				arrHidden = HttpContext.Current.Request.Form["hiddenChk"].Split(delimiter);
				iHiddenLen = arrHidden.Length;
			} catch(Exception) {
				arrHidden = new string[0];
				iHiddenLen = 0 ;
			}

			try {
				arrMustSend = HttpContext.Current.Request.Form["MUSTSendChk"].Split(delimiter);
				iMustSendLen = arrMustSend.Length;
			} catch(Exception) {
				arrMustSend = new string[0];
				iMustSendLen = 0;
			}

			try {
				switch(sType) {
					case "SEND":	//case: Sending match
						//declare local variables
						const int FIELDLENGTH = 6;
						//const int GOGOKLEAGUELENGTH = 10;
						int iTimeActivated = 0;
						string sCurrentTimestamp = null;
						string sBatchJob = null;
						string sValidity_Guest, sValidity_Host, sScore_Guest, sScore_Host, sAlert;
						string sOriginalStatus;
						string[] arrMatchDate, arrMatchTime, arrMatchField, arrScore_Host, arrValidity_Host, arrScore_Guest, arrValidity_Guest;
						string[] arrSongID, arrStatus, arrRemark, arrAlias, arrLeague, arrHost, arrGuest, arrAlert, arrShortStatus;
						//byte[] arrGoGoKMsgByte;
						ArrayList LiveOddsContentAL = new ArrayList(20);

						//Delcare variable used in message notify
			 			int iMsgBodyLength;
						string sMsgBody = null;
						string sAlertHost = null;
						string sAlertGuest = null;
						string[] arrQueueNames;
						string[] arrMessageTypes;
						string[] arrRemotingPath;
						byte[] arrByteOfMSMQBody;
						arrQueueNames = (string[])HttpContext.Current.Application["QueueItems"];
						arrMessageTypes = (string[])HttpContext.Current.Application["NotifyMessageTypes"];
						arrRemotingPath = (string[])HttpContext.Current.Application["RemotingItems"];
						MessageClient msgClt = new MessageClient();


						arrMatchDate = HttpContext.Current.Request.Form["MatchDate"].Split(delimiter);
						arrMatchTime = HttpContext.Current.Request.Form["MatchTime"].Split(delimiter);
						arrMatchField = HttpContext.Current.Request.Form["MatchField"].Split(delimiter);
						arrAlias = HttpContext.Current.Request.Form["Alias"].Split(delimiter);
						arrLeague = HttpContext.Current.Request.Form["League"].Split(delimiter);
						arrHost = HttpContext.Current.Request.Form["Host"].Split(delimiter);
						arrGuest = HttpContext.Current.Request.Form["Guest"].Split(delimiter);
						arrScore_Host = HttpContext.Current.Request.Form["hostScore"].Split(delimiter);
						arrValidity_Host = HttpContext.Current.Request.Form["hostValidity"].Split(delimiter);
						arrScore_Guest = HttpContext.Current.Request.Form["guestScore"].Split(delimiter);
						arrValidity_Guest = HttpContext.Current.Request.Form["guestValidity"].Split(delimiter);
						arrSongID = HttpContext.Current.Request.Form["song"].Split(delimiter);
						arrStatus = HttpContext.Current.Request.Form["status"].Split(delimiter);
						arrRemark = HttpContext.Current.Request.Form["remark"].Split(delimiter);
						try {
							arrAlert = HttpContext.Current.Request.Form["alertChk"].Split(delimiter);
						} catch(Exception) {
							arrAlert = new string[0];
						}
						arrShortStatus = (string[])HttpContext.Current.Application["matchItemsArray"];

						string[] arrMsgType, arrOddsItems;
						arrMsgType = (string[])HttpContext.Current.Application["messageType"];
						arrOddsItems = (string[])HttpContext.Current.Application["oddsItemsArray"];

						sCurrentTimestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
						sBatchJob = DateTime.Now.ToString("yyMMddHHmmss_fffffff") + "." + HttpContext.Current.Session["user_name"].ToString() + "." + arrMsgType[2] + ".ini";
						for(iRecIndex=0;iRecIndex<iMustSendLen;iRecIndex++) {
							//update the checked(MUSTSend) records only
							iUpdIndex = Convert.ToInt32(arrMustSend[iRecIndex]);

							//compare status
							sOriginalStatus = m_SportsDBMgr.ExecuteQueryString("select STATUS from GOALINFO where MATCH_CNT=" + arrMatchCnt[iUpdIndex]);
							m_SportsDBMgr.Close();
							iTimeActivated = m_SportsDBMgr.ExecuteScalar("select count(IMATCH_CNT) from TIMEOFGAME_INFO where IMATCH_CNT=" + arrMatchCnt[iUpdIndex]);
							m_SportsDBMgr.Close();
							if(arrStatus[iUpdIndex].Equals("1")) {	//current status = 上半
								if(sOriginalStatus.Equals("0")) {	//未開 -> 上半
									if(iTimeActivated > 0) {
										m_SportsDBMgr.ExecuteNonQuery("update TIMEOFGAME_INFO set STARTTIME=CURRENT_TIMESTAMP, CSTATUS='" + arrStatus[iUpdIndex] + "' where IMATCH_CNT=" + arrMatchCnt[iUpdIndex]);
									} else {
										m_SportsDBMgr.ExecuteNonQuery("insert into TIMEOFGAME_INFO values(" + arrMatchCnt[iUpdIndex] + ",CURRENT_TIMESTAMP,'" + arrStatus[iUpdIndex] + "')");
									}
									m_SportsDBMgr.Close();
								}
							} else if(arrStatus[iUpdIndex].Equals("3")) {	//current status = 下半
								if(sOriginalStatus.Equals("2")) {	//半休 -> 下半
									if(iTimeActivated > 0) {
										m_SportsDBMgr.ExecuteNonQuery("update TIMEOFGAME_INFO set STARTTIME=CURRENT_TIMESTAMP, CSTATUS='" + arrStatus[iUpdIndex] + "' where IMATCH_CNT=" + arrMatchCnt[iUpdIndex]);
									} else {
										m_SportsDBMgr.ExecuteNonQuery("insert into TIMEOFGAME_INFO values(" + arrMatchCnt[iUpdIndex] + ",CURRENT_TIMESTAMP,'" + arrStatus[iUpdIndex] + "')");
									}
									m_SportsDBMgr.Close();
								}
							} else if(arrStatus[iUpdIndex].Equals("5")) {	//current status = 加時
								if(sOriginalStatus.Equals("3") || sOriginalStatus.Equals("4")) {	//下半 -> 加時 or 完場 -> 加時
									if(iTimeActivated > 0) {
										m_SportsDBMgr.ExecuteNonQuery("update TIMEOFGAME_INFO set STARTTIME=CURRENT_TIMESTAMP, CSTATUS='" + arrStatus[iUpdIndex] + "' where IMATCH_CNT=" + arrMatchCnt[iUpdIndex]);
									} else {
										m_SportsDBMgr.ExecuteNonQuery("insert into TIMEOFGAME_INFO values(" + arrMatchCnt[iUpdIndex] + ",CURRENT_TIMESTAMP,'" + arrStatus[iUpdIndex] + "')");
									}
									m_SportsDBMgr.Close();
								}
							}

							//update goalinfo's SQL start
							SQLString.Remove(0,SQLString.Length);
							SQLString.Append("update goalinfo set");

							//update goalinfo's host score
							sScore_Host = arrScore_Host[iUpdIndex];
							SQLString.Append(" H_SCR=");
							if(sScore_Host.Equals("")) {
								sScore_Host = "0";
							}
							SQLString.Append("'" + sScore_Host + "'");

							//update goalinfo's guest score
							sScore_Guest = arrScore_Guest[iUpdIndex];
							SQLString.Append(", G_SCR=");
							if(sScore_Guest.Equals("")) {
								sScore_Guest = "0";
							}
							SQLString.Append("'" + sScore_Guest + "'");

							//update goalinfo's match status
							SQLString.Append(", STATUS='" + arrStatus[iUpdIndex] + "'");

							//update goalinfo's remarks
							sTempItem = arrRemark[iUpdIndex].Trim();
							SQLString.Append(", REMARK=");
							if(sTempItem.Equals("")) {
								SQLString.Append("null");
							} else {
								SQLString.Append("'" + sTempItem + "'");
							}

							//update for specific goalinfo's match count
							SQLString.Append(" where MATCH_CNT = " + arrMatchCnt[iUpdIndex]);
							m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
							m_SportsDBMgr.Close();
							//GoalReplicator.Replicate(SQLString.ToString());

							//Set Score status to variables
							sValidity_Host = arrValidity_Host[iUpdIndex];
							sValidity_Guest = arrValidity_Guest[iUpdIndex];

							//Check goal alert
							sAlert = "0";
							for(int iAlert=0;iAlert<arrAlert.Length;iAlert++) {
								if(arrAlert[iAlert] == iUpdIndex.ToString()) {
									sAlert = "1";
									break;
								}
							}

							if(arrSendToPager.Length>0) {
/*
								//Send GoGoK TCP message to MSMQ server
								sMsgBody = PadRightSpace(arrLeague[iUpdIndex],GOGOKLEAGUELENGTH) + PadRightSpace(arrHost[iUpdIndex],FIELDLENGTH) + PadRightSpace(arrGuest[iUpdIndex],FIELDLENGTH);
								sMsgBody += sScore_Host + sScore_Guest + sValidity_Host + sValidity_Guest;
								sMsgBody += arrStatus[iUpdIndex] + sAlert + arrRemark[iUpdIndex].Trim();
								arrGoGoKMsgByte = m_Big5Encoded.GetBytes(sMsgBody);
								iMsgBodyLength = arrGoGoKMsgByte.Length;
								sptMsg.Body = iMsgBodyLength.ToString("D3") + sMsgBody;
								sptMsg.Timestamp = DateTime.Now.ToString("yyMMddHHmmss");
								sptMsg.AppID = "00";
								sptMsg.MsgID = "00";
								//Send GoGoK Message
								//modified by Rita, 14 Jan 2004
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
										m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " LiveGoal.cs.UpdateLiveGoals(): Notify (LiveGoal) send gogok message via MSMQ throws MessageQueueException: " + mqEx.ToString());
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
										m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " LiveGoal.cs.UpdateLiveGoals(): Notify (LiveGoal) send gogok message via .NET Remoting throws exception: " + ex.ToString());
										m_SportsLog.Close();
									}
								}	catch(Exception ex) {
									m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
									m_SportsLog.SetFileName(0,LOGFILESUFFIX);
									m_SportsLog.Open();
									m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " LiveGoal.cs.UpdateLiveGoals(): Notify (LiveGoal) send gogok message via MSMQ throws exception: " + ex.ToString());
									m_SportsLog.Close();
								}
*/
								//modified end


								//Mango message format: AppID MsgID Priority AlertID MsgLen Alias Host Guest MatchStatus GoalType FreeText
								//Alias: Use Asia format (Alias, within 6 bytes, is not available in HKJC; HKJC short form has max. 12 bytes)
								//Host: Use HKJC format
								//Guest: Use HKJC format
								sAlertHost = "";
								sAlertHost = m_SportsDBMgr.ExecuteQueryString("select CHKJCSHORTNAME from TEAMINFO where TEAMNAME='" + arrHost[iUpdIndex] + "'");
								m_SportsDBMgr.Close();
								if(sAlertHost.Equals("")) sAlertHost = arrHost[iUpdIndex];
								sAlertGuest = "";
								sAlertGuest = m_SportsDBMgr.ExecuteQueryString("select CHKJCSHORTNAME from TEAMINFO where TEAMNAME='" + arrGuest[iUpdIndex] + "'");
								m_SportsDBMgr.Close();
								if(sAlertGuest.Equals("")) sAlertGuest = arrGuest[iUpdIndex];
								//sMsgBody = PadRightSpace(arrAlias[iUpdIndex],FIELDLENGTH) + PadRightSpace(sAlertHost,FIELDLENGTH) + PadRightSpace(sAlertGuest,FIELDLENGTH) + arrStatus[iUpdIndex];
								sMsgBody = PadRightSpace(arrAlias[iUpdIndex],FIELDLENGTH) + PadRightSpace(sAlertHost,FIELDLENGTH) + PadRightSpace(sAlertGuest,FIELDLENGTH) + PadRightSpace(arrStatus[iUpdIndex],2);
								if(Convert.ToInt32(sValidity_Host) > 2) {	//Host red card
									sptMsg.AppID = "01";
									sptMsg.MsgID = "02";
									//sMsgBody += (Convert.ToInt32(sValidity_Host) - 2).ToString() + arrHost[iUpdIndex];
									sMsgBody += (Convert.ToInt32(sValidity_Host) - 2).ToString() + "Home";
								} else if(Convert.ToInt32(sValidity_Guest) > 2) {		//Guest red card
									sptMsg.AppID = "01";
									sptMsg.MsgID = "02";
									//sMsgBody += (Convert.ToInt32(sValidity_Guest) - 2).ToString() + arrGuest[iUpdIndex];
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
									if(Convert.ToInt32(sValidity_Host) == 2) {	//Home false goal
										//sMsgBody += arrHost[iUpdIndex] + "(詐糊)";
										sMsgBody += "Home False Goal ";
									} else if(Convert.ToInt32(sValidity_Guest) == 2) {	//Guest false goal
										//sMsgBody += arrGuest[iUpdIndex] + "(詐糊)";
										sMsgBody += "Away False Goal ";
									}
									sMsgBody += sScore_Host + ":" + sScore_Guest;
*/
								}

								if(arrStatus[iUpdIndex].Equals("10")) {	//current status = 待定
									sMsgBody += " Unconfirmed";
								}

								//040901 fanny start
								//if(!arrRemark[iUpdIndex].Trim().Equals("")) sMsgBody += " " + arrRemark[iUpdIndex].Trim();
								//040901 fanny end
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
										m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " LiveGoal.cs.UpdateLiveGoals(): Notify (LiveGoal - Mango) via MSMQ throws MessageQueueException: " + mqEx.ToString());
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
										m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " LiveGoal.cs.UpdateLiveGoals(): Notify (LiveGoal - Mango) via .NET Remotingthrows exception: " + ex.ToString());
										m_SportsLog.Close();
									}
								}	catch(Exception ex) {
									m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
									m_SportsLog.SetFileName(0,LOGFILESUFFIX);
									m_SportsLog.Open();
									m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " LiveGoal.cs.UpdateLiveGoals(): Notify (LiveGoal - Mango) via MSMQ throws exception: " + ex.ToString());
									m_SportsLog.Close();
								}

							}

							//generate LiveGoal INI file if required
							if(arrSendToPager.Length>0 && iMustSendLen>0) {
								if(sScore_Host.Equals("")) sScore_Host = "-1";
								if(sScore_Guest.Equals("")) sScore_Guest = "-1";
								sStatus = arrShortStatus[Convert.ToInt32(arrStatus[iUpdIndex])];
								sRemarks = arrRemark[iUpdIndex].Trim();
								if(sRemarks.Equals("")) sRemarks = "-1";

								//Time Of Game
								m_SportsOleReader = m_SportsDBMgr.ExecuteQuery("select STARTTIME from TIMEOFGAME_INFO where IMATCH_CNT=" + arrMatchCnt[iUpdIndex]);
								if(m_SportsOleReader.Read()) {
									iTimeDiff = (int)DateTime.Now.Subtract(m_SportsOleReader.GetDateTime(0)).TotalMinutes;
									if(arrStatus[iUpdIndex].Equals("1") || arrStatus[iUpdIndex].Equals("3")) {
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

								//Insert log into LOG_LIVEGOAL
								SQLString.Remove(0,SQLString.Length);
								SQLString.Append("insert into LOG_LIVEGOAL (TIMEFLAG, SECTION, LEAGUE, HOST, GUEST, ACT, MATCHDATE, MATCHTIME, MATCHFIELD, H_GOAL, G_GOAL, H_CONFIRM, G_CONFIRM, SONGID, ALERT, STATUS, COMMENT, TIMEOFGAME, BATCHJOB) values ('");
								SQLString.Append(sCurrentTimestamp);
								SQLString.Append("','GOAL_','");
								SQLString.Append(arrLeague[iUpdIndex]);
								SQLString.Append("','");
								SQLString.Append(arrHost[iUpdIndex]);
								SQLString.Append("','");
								SQLString.Append(arrGuest[iUpdIndex]);
								SQLString.Append("','U','");
								SQLString.Append(arrMatchDate[iUpdIndex]);
								SQLString.Append("','");
								SQLString.Append(arrMatchTime[iUpdIndex]);
								SQLString.Append("','");
								SQLString.Append(arrMatchField[iUpdIndex]);
								SQLString.Append("',");
								SQLString.Append(sScore_Host);
								SQLString.Append(",");
								SQLString.Append(sScore_Guest);
								SQLString.Append(",");
								SQLString.Append(sValidity_Host);
								SQLString.Append(",");
								SQLString.Append(sValidity_Guest);
								SQLString.Append(",'");
								SQLString.Append(arrSongID[iUpdIndex]);
								SQLString.Append("',");
								SQLString.Append(sAlert);
								SQLString.Append(",'");
								SQLString.Append(sStatus.Substring(0,1));
								SQLString.Append("','");
								SQLString.Append(sRemarks);
								SQLString.Append("',");
								SQLString.Append(iTimeDiff.ToString());
								SQLString.Append(",'");
								SQLString.Append(sBatchJob);
								SQLString.Append("')");
								m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
								m_SportsDBMgr.Close();
							}

							//update LIVEODDS_INFO when status is 無效(0) and score != 0:0
							//and SendToPager, MustSend flags are checked
							if(sValidity_Host.Equals("0") || sValidity_Guest.Equals("0")) {
								if(!(sScore_Host.Equals("0") && sScore_Guest.Equals("0"))) {
									//update liveodds's status to 暫停 and current score
									//but the current minute did not update base on performance consideration
									//Moreover, it'll be hidden in the pager when the status is 暫停
									iLiveOddsExisted = 0;
									if(arrSendToPager.Length>0 && iMustSendLen>0) {
										SQLString.Remove(0,SQLString.Length);
										SQLString.Append("select CALIAS, CMATCHFIELD, CHOST_HANDI, CMATCHSTATUS, ITIMEOFGAME, CHANDICAP1, CHANDICAP2, CHODDS, IREGION_ID, CH_WEAR, CG_WEAR, CTOTALSCORE, CBIGODDS, CSMALLODDS from LIVEODDS_INFO where CLEAGUE='");
										SQLString.Append(arrLeague[iUpdIndex]);
										SQLString.Append("' and CHOST='");
										SQLString.Append(arrHost[iUpdIndex]);
										SQLString.Append("' and CGUEST='");
										SQLString.Append(arrGuest[iUpdIndex]);
										SQLString.Append("' order by IREGION_ID");
										m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
										while(m_SportsOleReader.Read()) {
											iLiveOdds++;
											iLiveOddsExisted++;

											LiveOddsContentAL.Add(m_SportsOleReader.GetString(0).Trim());
											LiveOddsContentAL.Add(arrLeague[iUpdIndex]);
											LiveOddsContentAL.Add(arrHost[iUpdIndex]);
											LiveOddsContentAL.Add(arrGuest[iUpdIndex]);
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

										if(iLiveOddsExisted > 0) {
											SQLString.Remove(0,SQLString.Length);
											SQLString.Append("update LIVEODDS_INFO set CODDSSTATUS='2', IH_SCORE=");
											SQLString.Append(sScore_Host);
											SQLString.Append(", IG_SCORE=");
											SQLString.Append(sScore_Guest);
											SQLString.Append(" where CLEAGUE='");
											SQLString.Append(arrLeague[iUpdIndex]);
											SQLString.Append("' and CHOST='");
											SQLString.Append(arrHost[iUpdIndex]);
											SQLString.Append("' and CGUEST='");
											SQLString.Append(arrGuest[iUpdIndex]);
											SQLString.Append("'");
											m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
											m_SportsDBMgr.Close();
											//GoalReplicator.Replicate(SQLString.ToString());
										}
									}
								}
							}
						}
						LiveOddsContentAL.TrimToSize();

						//Send Notify Message for Goal
						//modified by Chapman, 5 Feb 2004
						if(arrSendToPager.Length>0 && iMustSendLen > 0) {
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
									m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " LiveGoal.cs.UpdateLiveGoals(): Notify (LiveGoal) via MSMQ throws MessageQueueException: " + mqEx.ToString());
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
									m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " LiveGoal.cs.UpdateLiveGoals(): Notify (LiveGoal) via .NET Remotingthrows exception: " + ex.ToString());
									m_SportsLog.Close();
								}
							}	catch(Exception ex) {
								m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
								m_SportsLog.SetFileName(0,LOGFILESUFFIX);
								m_SportsLog.Open();
								m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " LiveGoal.cs.UpdateLiveGoals(): Notify (LiveGoal) via MSMQ throws exception: " + ex.ToString());
								m_SportsLog.Close();
							}
						}
						//modified end

						sCurrentTimestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
						sBatchJob = DateTime.Now.ToString("yyMMddHHmmss_fffffff") + "." + HttpContext.Current.Session["user_name"].ToString() + "." + arrMsgType[20] + ".ini";
						//Send Updated Live Odds
						if(iLiveOdds>0 && arrSendToPager.Length>0) {
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

							//Send Notify Message for Live odds if existed
							//modified by Chapman, 5 Feb 2004 begin
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
									m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " LiveGoal.cs.UpdateLiveGoals(): Notify (LiveOdds) via MSMQ throws MessageQueueException: " + mqEx.ToString());
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
									m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " LiveGoal.cs.UpdateLiveGoals(): Notify (LiveOdds) via .NET Remoting throws exception: " + ex.ToString());
									m_SportsLog.Close();
								}
							}	catch(Exception ex) {
								m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
								m_SportsLog.SetFileName(0,LOGFILESUFFIX);
								m_SportsLog.Open();
								m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " LiveGoal.cs.UpdateLiveGoals(): Notify (LiveOdds) via MSMQ throws exception: " + ex.ToString());
								m_SportsLog.Close();
							}

						}
						//modified end
/*
						if(iRecIndex > 0) {
							//GoalReplicator.Dispose();
							m_SportsDBMgr.Dispose();
						}
*/
						break;
					case "SORT":	//case: Sorting match order
						for(iRecIndex=0;iRecIndex<arrOrderID.Length;iRecIndex++) {
							sTempItem = arrOrderID[iRecIndex];
							if(sTempItem == null) {
								sOrder = null;
							} else if(sTempItem.Equals("")) {
								sOrder = null;
							}	else {
								iTempItem = Convert.ToInt32(sTempItem);
								if(iTempItem < 10) sOrder = "00" + iTempItem.ToString();
								else if(iTempItem < 100) sOrder = "0" + iTempItem.ToString();
								else sOrder = iTempItem.ToString();
							}
							SQLString.Remove(0,SQLString.Length);
							SQLString.Append("update goalinfo set ORDER_ID='");
							SQLString.Append(sOrder);
							SQLString.Append("' where MATCH_CNT=");
							SQLString.Append(arrMatchCnt[iRecIndex].Trim());
							m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
							m_SportsDBMgr.Close();
							SQLString.Remove(0,SQLString.Length);
							SQLString.Append("select count(*) from gameinfo where MATCH_CNT=");
							SQLString.Append(arrMatchCnt[iRecIndex].Trim());
							if (m_SportsDBMgr.ExecuteScalar(SQLString.ToString()) > 0) {
								SQLString.Remove(0,SQLString.Length);
								SQLString.Append("update gameinfo set MATCH_ID='");
								SQLString.Append(sOrder);
								SQLString.Append("' where MATCH_CNT=");
								SQLString.Append(arrMatchCnt[iRecIndex].Trim());
								m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
							}
							m_SportsDBMgr.Close();
							//GoalReplicator.Replicate(SQLString.ToString());
						}
/*
						if(iRecIndex > 0) {
							//GoalReplicator.Dispose();
							m_SportsDBMgr.Dispose();
						}
*/
						//write log
						m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
						m_SportsLog.SetFileName(0,LOGFILESUFFIX);
						m_SportsLog.Open();
						m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " LiveGoal.cs: Sort " + iRecIndex.ToString() + " goals (" + HttpContext.Current.Session["user_name"] + ")");
						m_SportsLog.Close();
						break;
					case "HIDE":	//case: Hidden specific match
						for(iRecIndex=0;iRecIndex<iHiddenLen;iRecIndex++) {
							iUpdIndex = Convert.ToInt32(arrHidden[iRecIndex]);
							sTempItem = arrMatchCnt[iUpdIndex].Trim();
							if(iRecIndex == 0) {
								SQLString.Remove(0,SQLString.Length);
								SQLString.Append("update goalinfo set WEBACT='X' where MATCH_CNT=");
								SQLString.Append(sTempItem);
							}	else {
								SQLString.Append(" or MATCH_CNT=");
								SQLString.Append(sTempItem);
							}
							m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
							m_SportsDBMgr.Close();
							//GoalReplicator.Replicate(SQLString.ToString());
						}
/*
						if(iRecIndex > 0) {
							//GoalReplicator.Dispose();
							m_SportsDBMgr.Dispose();
						}
*/
						//write log
						m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
						m_SportsLog.SetFileName(0,LOGFILESUFFIX);
						m_SportsLog.Open();
						m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " LiveGoal.cs: Hide " + iRecIndex.ToString() + " goals (" + HttpContext.Current.Session["user_name"] + ")");
						m_SportsLog.Close();
						break;
					case "SHOW":	//case: Set all matches to visible
						SQLString.Remove(0,SQLString.Length);
						SQLString.Append("update goalinfo set WEBACT='V'");
						iRecIndex = m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
						m_SportsDBMgr.Close();
						//GoalReplicator.Replicate(SQLString.ToString());
						//GoalReplicator.Dispose();
						//m_SportsDBMgr.Dispose();

						//write log
						m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
						m_SportsLog.SetFileName(0,LOGFILESUFFIX);
						m_SportsLog.Open();
						m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " LiveGoal.cs: " + HttpContext.Current.Session["user_name"] + " set " + iRecIndex.ToString() + " goals to visible");
						m_SportsLog.Close();
						break;
					default:
						iRecIndex = 0;
						break;
				}
			} catch(Exception ex) {
				iRecIndex = -1;
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " LiveGoal.cs.UpdateLiveGoals(): " + ex.ToString());
				m_SportsLog.Close();
			}
			return iRecIndex;
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