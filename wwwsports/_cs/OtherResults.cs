/*
Objective:
Other result data other than live goal

Last updated:
22 Sep 2004 (Fanny) change max no. of match from 10 to 15
09 Feb 2004 by Henry

C#.NET complier statement:
csc /t:library /out:..\bin\OtherResults.dll /r:..\bin\DBManager.dll;..\bin\Files.dll;..\bin\MessageClient.dll;..\bin\SportsMessage.dll OtherResults.cs
*/

using System;
using System.Collections;
using System.Data.OleDb;
using System.Reflection;
using System.Text;
using System.Web;
using TDL.DB;
using TDL.IO;
using TDL.Message;

[assembly:AssemblyCompany("TDSL")]
[assembly:AssemblyCopyright("(c) 2003 TDSL. All rights reserved.")]
[assembly:AssemblyDescription("足球資訊 -> 其他比數")]
[assembly:AssemblyProduct("Sports.NET")]
[assembly:AssemblyTitle("Sports.NET DLL")]
[assembly:AssemblyVersion("3.1.*")]
namespace SportsUtil {
	public class OtherResults {
		DBManager m_SportsDBMgr;
		Files m_SportsLog;
		const string LOGFILESUFFIX = "log";
		const int index = 15;
		//const int index = 10;
		int m_iRecordCount = 0;
		string sLeagName = "";
		Encoding m_Big5Encoded;
		StringBuilder SQLString;

		public OtherResults(string Connection) {
			m_SportsDBMgr = new DBManager();
			m_SportsDBMgr.ConnectionString = Connection;
			m_SportsLog = new Files();
			m_Big5Encoded = Encoding.GetEncoding(950);
			SQLString = new StringBuilder();
		}

		public int NumberOfRecords {
			get {
				return m_iRecordCount;
			}
		}

		public string GetLeaglong {
			get {
				return 	sLeagName;
			}
		}

		public string GetOtherResults() {
			int iArrayIndex = 0;
			int inum = 0;
			int iTemp = 0;
			string sLeagID = "";
			string sLeagAlias = "";
			string[] statusArray;
			OleDbDataReader SportsOleReader;
			StringBuilder HTMLString = new StringBuilder();

			sLeagID = HttpContext.Current.Request.QueryString["leagID"].Trim();
			statusArray = (string[])HttpContext.Current.Application["matchItemsArray"];
			try {
				ArrayList arrHostAL,arrGuestAL,arrMatchDateAL,arrMatchTimeAL,arrStatusAL,arrHscrAL,arrGscrAL,arrSendAL;
				ArrayList arrTeamNameAL;
				arrHostAL = new ArrayList();
				arrGuestAL = new ArrayList();
				arrMatchDateAL = new ArrayList();
				arrMatchTimeAL = new ArrayList();
				arrStatusAL = new ArrayList();
				arrHscrAL = new ArrayList();
				arrGscrAL = new ArrayList();
				arrSendAL = new ArrayList();
				arrTeamNameAL = new ArrayList();
				SQLString.Remove(0,SQLString.Length);
				SQLString.Append("select distinct leagname,alias from leaginfo where leag_id='");
				SQLString.Append(sLeagID);
				SQLString.Append("'");
				SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
				if(SportsOleReader.Read()) {
					sLeagName = SportsOleReader.GetString(0).Trim();
					sLeagAlias = SportsOleReader.GetString(1).Trim();
				}
				SportsOleReader.Close();
				m_SportsDBMgr.Close();

				SQLString.Remove(0,SQLString.Length);
				SQLString.Append("select otherresinfo.host,otherresinfo.guest,otherresinfo.matchdate,otherresinfo.matchtime,otherresinfo.status,otherresinfo.h_score,otherresinfo.g_score,otherresinfo.csend from leaginfo ,otherresinfo where leaginfo.leagname=otherresinfo.league and leaginfo.leag_id='");
				SQLString.Append(sLeagID);
				SQLString.Append("'");
				SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
				while(SportsOleReader.Read()) {
					arrHostAL.Add(SportsOleReader.GetString(0).Trim());
					arrGuestAL.Add(SportsOleReader.GetString(1).Trim());
					arrMatchDateAL.Add(SportsOleReader.GetString(2).Trim());
					arrMatchTimeAL.Add(SportsOleReader.GetString(3).Trim());
					arrStatusAL.Add(SportsOleReader.GetString(4).Trim());
					arrHscrAL.Add(SportsOleReader.GetString(5).Trim());
					arrGscrAL.Add(SportsOleReader.GetString(6).Trim());
					arrSendAL.Add(SportsOleReader.GetString(7).Trim());
				}
				SportsOleReader.Close();
				m_SportsDBMgr.Close();
				arrHostAL.TrimToSize();
				arrGuestAL.TrimToSize();
				arrMatchDateAL.TrimToSize();
				arrMatchTimeAL.TrimToSize();
				arrStatusAL.TrimToSize();
				arrHscrAL.TrimToSize();
				arrGscrAL.TrimToSize();
				arrSendAL.TrimToSize();

				SQLString.Remove(0,SQLString.Length);
				SQLString.Append("select teaminfo.teamname from id_info,teaminfo where id_info.team_id=teaminfo.team_id and leag_id='");
				SQLString.Append(sLeagID);
				SQLString.Append("'");
				SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
				while(SportsOleReader.Read()) {
					arrTeamNameAL.Add(SportsOleReader.GetString(0).Trim());
				}
				SportsOleReader.Close();
				m_SportsDBMgr.Close();
				m_SportsDBMgr.Dispose();
				arrTeamNameAL.TrimToSize();

				for(inum = 0; inum < index; inum++ ) {
					//執行動作
					iArrayIndex = 0;
					HTMLString.Append("<tr align=\"center\"><td><select name=\"act\" onChange=\"ActionChange(");
					HTMLString.Append(m_iRecordCount.ToString());
					HTMLString.Append(")\"><option value=\"U\">新增/修改<option value=\"D\">刪除</select></td>");
					if(inum < arrHostAL.Count) {
						//日期
						if(!arrMatchDateAL[inum].Equals("")) {
							HTMLString.Append("<td><input name=\"MatchDate\" size=8 maxlength=8 value=\"");
							HTMLString.Append(arrMatchDateAL[inum].ToString());
							HTMLString.Append("\" onChange=\"onMatchDateChanged(");
							HTMLString.Append(m_iRecordCount.ToString());
							HTMLString.Append(")\"></td>");
						}	else {
							HTMLString.Append("<td><input name=\"MatchDate\" size=8 maxlength=8 value=\"\" onChange=\"onMatchDateChanged(");
							HTMLString.Append(m_iRecordCount.ToString());
							HTMLString.Append(")\"></td>");
						}

						//時間
						if(!arrMatchTimeAL[inum].Equals("")) {
							HTMLString.Append("<td><input name=\"MatchTime\" size=4 maxlength=4 value=\"");
							HTMLString.Append(arrMatchTimeAL[inum].ToString());
							HTMLString.Append("\" onChange=\"onMatchTimeChanged(");
							HTMLString.Append(m_iRecordCount.ToString());
							HTMLString.Append(")\"></td>");
						} else {
							HTMLString.Append("<td><input name=\"MatchTime\" size=4 maxlength=4 value=\"\" onChange=\"onMatchTimeChanged(");
							HTMLString.Append(m_iRecordCount.ToString());
							HTMLString.Append(")\"></td>");
						}

						//時段
						iArrayIndex = 0;
						iTemp = Convert.ToInt32(arrStatusAL[inum].ToString());
						HTMLString.Append("<td><select name=\"status\" onChange=\"onStatusChanged(");
						HTMLString.Append(m_iRecordCount.ToString());
						HTMLString.Append(")\"><option value=\"");
						HTMLString.Append(arrStatusAL[inum].ToString());
						HTMLString.Append("\">");
						HTMLString.Append(statusArray[iTemp]);
						foreach(String sItem in statusArray) {
							if(iArrayIndex != iTemp) {
								HTMLString.Append("<option value=");
								HTMLString.Append(iArrayIndex.ToString());
								HTMLString.Append(">");
								HTMLString.Append(sItem);
							}
							iArrayIndex++;
						}
						HTMLString.Append("</select></td>");

						//主隊-比數
						HTMLString.Append("<td><select name=\"host\" onChange=\"onHostChanged(");
						HTMLString.Append(m_iRecordCount.ToString());
						HTMLString.Append(")\"><option value=\"");
						HTMLString.Append(arrHostAL[inum].ToString());
						HTMLString.Append("\" selected>");
						HTMLString.Append(arrHostAL[inum].ToString());
						foreach(String sItem in arrTeamNameAL) {
							if(sItem!=arrHostAL[inum].ToString()) {
								HTMLString.Append("<option value=\"");
								HTMLString.Append(sItem.ToString());
								HTMLString.Append("\">");
								HTMLString.Append(sItem);
							}
						}
						HTMLString.Append("</select><input name=\"h_score\" size=2 maxlength=2 value=\"");
						HTMLString.Append(arrHscrAL[inum].ToString());
						HTMLString.Append("\" onChange=\"onHscrChanged(");
						HTMLString.Append(m_iRecordCount.ToString());
						HTMLString.Append(")\"></td>");

						//客隊-比數
						HTMLString.Append("<td><select name=\"guest\" onChange=\"onGuestChanged(");
						HTMLString.Append(m_iRecordCount.ToString());
						HTMLString.Append(")\"><option value=\"");
						HTMLString.Append(arrGuestAL[inum].ToString());
						HTMLString.Append("\" selected>");
						HTMLString.Append(arrGuestAL[inum].ToString());
						foreach(String sItem in arrTeamNameAL) {
							if(sItem!=arrGuestAL[inum].ToString()) {
								HTMLString.Append("<option value=\"");
								HTMLString.Append(sItem.ToString());
								HTMLString.Append("\">");
								HTMLString.Append(sItem);
							}
						}
						HTMLString.Append("</select><input name=\"g_score\" size=2 maxlength=2 value=\"");
						HTMLString.Append(arrGscrAL[inum].ToString());
						HTMLString.Append("\" onChange=\"onGscrChanged(");
						HTMLString.Append(m_iRecordCount.ToString());
						HTMLString.Append(")\"></td>");

						//響機, 發送
						HTMLString.Append("<td><input type=\"checkbox\" name=\"alert\" value=\"");
						HTMLString.Append(m_iRecordCount.ToString());
						HTMLString.Append("\" onClick=\"alertClicked(");
						HTMLString.Append(m_iRecordCount.ToString());
						HTMLString.Append(")\"></td><td><input type=\"checkbox\" name=\"send\" value=\"");
						HTMLString.Append(m_iRecordCount.ToString());
						HTMLString.Append("\"></td>");
					}	else {
						iArrayIndex = 0;
						//日期, 時間, 時段
						HTMLString.Append("<td><input name=\"MatchDate\" size=8 maxlength=8 value=\"\" onChange=\"onMatchDateChanged(");
						HTMLString.Append(m_iRecordCount.ToString());
						HTMLString.Append(")\"></td><td><input name=\"MatchTime\" size=4 maxlength=4 value=\"\" onChange=\"onMatchTimeChanged(");
						HTMLString.Append(m_iRecordCount.ToString());
						HTMLString.Append(")\"></td><td><select name=\"status\" onChange=\"onStatusChanged(");
						HTMLString.Append(m_iRecordCount.ToString());
						HTMLString.Append(")\">");
						foreach(String sItem in statusArray) {
							HTMLString.Append("<option value=");
							HTMLString.Append(iArrayIndex.ToString());
							HTMLString.Append(">");
							HTMLString.Append(sItem);
							iArrayIndex++;
						}

						//主隊-比數
						HTMLString.Append("<td><select name=\"host\" onChange=\"onHostChanged(");
						HTMLString.Append(m_iRecordCount.ToString());
						HTMLString.Append(")\">");
						if(arrTeamNameAL.Count>0) {
							foreach(String sItem in arrTeamNameAL) {
								HTMLString.Append("<option value=\"");
								HTMLString.Append(sItem.ToString());
								HTMLString.Append("\" >");
								HTMLString.Append(sItem);
							}
						} else {
							HTMLString.Append("<option value=\"\">暫無球隊");
						}
						HTMLString.Append("</select><input name=\"h_score\" size=2 maxlength=2 value=\"\" onChange=\"onHscrChanged(");
						HTMLString.Append(m_iRecordCount.ToString());
						HTMLString.Append(")\"></td>");

						//客隊-比數
						HTMLString.Append("<td><select name=\"guest\" onChange=\"onGuestChanged(");
						HTMLString.Append(m_iRecordCount.ToString());
						HTMLString.Append(")\">");
						if(arrTeamNameAL.Count>0) {
							foreach(String sItem in arrTeamNameAL) {
								HTMLString.Append("<option value=\"");
								HTMLString.Append(sItem.ToString());
								HTMLString.Append("\">");
								HTMLString.Append(sItem);
							}
						} else {
							HTMLString.Append("<option value=\"\">暫無球隊");
						}
						HTMLString.Append("</select><input name=\"g_score\" size=2 maxlength=2 value=\"\" onChange=\"onGscrChanged(");
						HTMLString.Append(m_iRecordCount.ToString());
						HTMLString.Append(")\"></td>");

						//響機, 發送
						HTMLString.Append("<td><input type=\"checkbox\" name=\"alert\" value=\"");
						HTMLString.Append(m_iRecordCount.ToString());
						HTMLString.Append("\" onClick=\"alertClicked(");
						HTMLString.Append(m_iRecordCount.ToString());
						HTMLString.Append(")\"></td><td><input type=\"checkbox\" name=\"send\" value=\"");
						HTMLString.Append(m_iRecordCount.ToString());
						HTMLString.Append("\" ></td>");
					}
					HTMLString.Append("</tr>");
					m_iRecordCount++;
				}
				HTMLString.Append("<input type=\"hidden\" name=\"sLeagName\" value=\"");
				HTMLString.Append(sLeagName);
				HTMLString.Append("\"><input type=\"hidden\" name=\"sLeagAlias\" value=\"");
				HTMLString.Append(sLeagAlias);
				HTMLString.Append("\">");
			} catch(Exception ex) {
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " OtherResults.cs.GetOtherResults(): " + ex.ToString());
				m_SportsLog.Close();
				HTMLString.Remove(0,HTMLString.Length);
				HTMLString.Append((string)HttpContext.Current.Application["accessErrorMsg"]);
			}
			return HTMLString.ToString();
		}

		public int  SendOthersResult() {
			int irecord = 0;
			int iUpdIndex;
			int iLoggedUpd = 0;
			int iLoggedDel = 0;
			string sLeagLong;
			string sLeagAlias;
			string sAlert = null;
			string sCurrentTimestamp = null;
			string sBatchJob = null;
			char[] delimiter = new char[] {','};
			string[] arrHost;
			string[] arrGuest;
			string[] arrMatchDate;
			string[] arrMatchTime;
			string[] arrStatus;
			string[] arrHscr;
			string[] arrGscr;
			string[] arrSend;
			string[] arrAct;
			string[] statusArray;
			string[] arrAlert;
			string[] arrMsgType;
			string[] arrSendToPager;
			string[] arrRemotingPath;

			//SportsMessage object message
			SportsMessage sptMsg = new SportsMessage();
			StringBuilder LogSQLString = new StringBuilder();
			DBManager logDBMgr = new DBManager();
			logDBMgr.ConnectionString = m_SportsDBMgr.ConnectionString;

			statusArray = (string[])HttpContext.Current.Application["matchItemsArray"];
			arrMsgType = (string[])HttpContext.Current.Application["messageType"];
			sLeagLong = HttpContext.Current.Request.Form["sLeagName"];
			sLeagAlias = HttpContext.Current.Request.Form["sLeagAlias"];
			arrHost = HttpContext.Current.Request.Form["host"].Split(delimiter);
			arrGuest = HttpContext.Current.Request.Form["guest"].Split(delimiter);
			arrMatchDate = HttpContext.Current.Request.Form["MatchDate"].Split(delimiter);
			arrMatchTime = HttpContext.Current.Request.Form["MatchTime"].Split(delimiter);
			arrStatus = HttpContext.Current.Request.Form["status"].Split(delimiter);
			arrHscr = HttpContext.Current.Request.Form["h_score"].Split(delimiter);
			arrGscr = HttpContext.Current.Request.Form["g_score"].Split(delimiter);
			arrAct = HttpContext.Current.Request.Form["act"].Split(delimiter);
			arrRemotingPath = (string[])HttpContext.Current.Application["RemotingItems"];

			try {
				arrSendToPager = HttpContext.Current.Request.Form["SendToPager"].Split(delimiter);
			} catch(Exception) {
				arrSendToPager = new string[0];
			}
			try {
				arrSend = HttpContext.Current.Request.Form["send"].Split(delimiter);
			} catch {
				arrSend = new string[0];
			}
			try {
				arrAlert = HttpContext.Current.Request.Form["alert"].Split(delimiter);
			} catch {
				arrAlert = new string[0];
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

				SQLString.Remove(0,SQLString.Length);
				SQLString.Append("delete from otherresinfo where league='");
				SQLString.Append(sLeagLong);
				SQLString.Append("'");
				m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
				m_SportsDBMgr.Close();

				for(int iNumber = 0; iNumber < arrMatchDate.Length; iNumber++ ) {
					if((arrAct[iNumber]=="U") && (arrMatchDate[iNumber]!="")) {
						SQLString.Remove(0,SQLString.Length);
						SQLString.Append("insert into otherresinfo (league,host,guest,matchdate,matchtime,status,act,h_score,g_score,flag,csend ) values (");
						SQLString.Append("'");
						SQLString.Append(sLeagLong);
						SQLString.Append("','");
						SQLString.Append(arrHost[iNumber]);
						SQLString.Append("','");
						SQLString.Append(arrGuest[iNumber]);
						SQLString.Append("','");
						SQLString.Append(arrMatchDate[iNumber]);
						SQLString.Append("','");
						SQLString.Append(arrMatchTime[iNumber]);
						SQLString.Append("','");
						SQLString.Append(arrStatus[iNumber]);
						SQLString.Append("','U','");
						SQLString.Append(arrHscr[iNumber]);
						SQLString.Append("','");
						SQLString.Append(arrGscr[iNumber]);
						SQLString.Append("','0',1)");
						m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
						m_SportsDBMgr.Close();
					}
				}

				//Action = D
				sCurrentTimestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
				sBatchJob = DateTime.Now.ToString("yyMMddHHmmss_fffffff") + "." + HttpContext.Current.Session["user_name"].ToString() + "." + arrMsgType[23] + ".del.ini";
				for(int iNumber = 0; iNumber < arrSend.Length; iNumber++) {
					sAlert = "0";
					iUpdIndex = Convert.ToInt32(arrSend[iNumber]);
					if(arrHscr[iUpdIndex].Equals("")) arrHscr[iUpdIndex] = "0";
					if(arrGscr[iUpdIndex].Equals("")) arrGscr[iUpdIndex] = "0";
					if(arrSendToPager.Length > 0) {
						if(arrAct[iUpdIndex].Equals("D")) {
							iLoggedDel++;
							LogSQLString.Remove(0,LogSQLString.Length);
							LogSQLString.Append("insert into LOG_OTHERGOAL (TIMEFLAG, IITEMSEQ_NO, SECTION, LEAGUE, LEAGUEALIAS, HOST, GUEST, ACT, H_GOAL, G_GOAL, STATUS, MATCHDATE, MATCHTIME, ALERT, BATCHJOB) values ('");
							LogSQLString.Append(sCurrentTimestamp);
							LogSQLString.Append("',");
							LogSQLString.Append(iLoggedDel.ToString());
							LogSQLString.Append(",'OTHGOAL_','");
							LogSQLString.Append(sLeagLong);
							LogSQLString.Append("','");
							LogSQLString.Append(sLeagAlias);
							LogSQLString.Append("','");
							LogSQLString.Append(arrHost[iUpdIndex]);
							LogSQLString.Append("','");
							LogSQLString.Append(arrGuest[iUpdIndex]);
							LogSQLString.Append("','D',");
							LogSQLString.Append(arrHscr[iUpdIndex]);
							LogSQLString.Append(",");
							LogSQLString.Append(arrGscr[iUpdIndex]);
							LogSQLString.Append(",'");
							LogSQLString.Append(statusArray[Convert.ToInt32(arrStatus[iUpdIndex])].Substring(0,1));
							LogSQLString.Append("','");
							LogSQLString.Append(arrMatchDate[iUpdIndex]);
							LogSQLString.Append("','");
							LogSQLString.Append(arrMatchTime[iUpdIndex]);
							LogSQLString.Append("','");
							for(int i = 0; i < arrAlert.Length; i++) {
								if(Convert.ToInt32(arrAlert[i]) == iUpdIndex) {
									sAlert = "1";
									break;
								}
							}
							LogSQLString.Append(sAlert);
							LogSQLString.Append("','");
							LogSQLString.Append(sBatchJob);
							LogSQLString.Append("')");
							logDBMgr.ExecuteNonQuery(LogSQLString.ToString());
							logDBMgr.Close();
						}	//Insert log into LOG_OTHERGOAL
					}
				}
				if(iLoggedDel > 0) {
					//Modified by Henry, 09 Feb 2004
					//Send Notify Message
					sptMsg.IsTransaction = true;
					sptMsg.Body = sBatchJob;
					sptMsg.Timestamp = DateTime.Now.ToString("yyMMddHHmmss");
					sptMsg.AppID = "07";
					sptMsg.MsgID = "17";
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
							m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " MSMQ ERROR: Other Results");
							m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " OtherResults.cs.SendOthersResult(): Notify via MSMQ throws MessageQueueException:  " + mqEx.ToString());
							m_SportsLog.Close();

							//If MSMQ fail, notify via .NET Remoting
							msgClt.MessageType = arrMessageTypes[1];
							msgClt.MessagePath = arrRemotingPath[0];
							if(!msgClt.SendMessage((object)sptMsg)) {
								m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
								m_SportsLog.SetFileName(0,LOGFILESUFFIX);
								m_SportsLog.Open();
								m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Remoting ERROR: Other Results");
								m_SportsLog.Close();
							}
						}	catch(Exception ex) {
							m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
							m_SportsLog.SetFileName(0,LOGFILESUFFIX);
							m_SportsLog.Open();
							m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Remoting ERROR: Other Results");
							m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " OtherResults.cs.SendOthersResult(): Notify via .NET Remoting throws exception: " + ex.ToString());
							m_SportsLog.Close();
						}
					}	catch(Exception ex) {
						m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
						m_SportsLog.SetFileName(0,LOGFILESUFFIX);
						m_SportsLog.Open();
						m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " OtherResults.cs.SendOthersResult(): Notify via MSMQ throws exception: " + ex.ToString());
						m_SportsLog.Close();
					}
					//end modify
				}

				//Action = U
				sCurrentTimestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
				sBatchJob = DateTime.Now.ToString("yyMMddHHmmss_fffffff") + "." + HttpContext.Current.Session["user_name"].ToString() + "." + arrMsgType[23] + ".ini";
				for(int iNumber = 0; iNumber < arrSend.Length; iNumber++) {
					sAlert = "0";
					iUpdIndex = Convert.ToInt32(arrSend[iNumber]);
					if(arrHscr[iUpdIndex].Equals("")) arrHscr[iUpdIndex] = "0";
					if(arrGscr[iUpdIndex].Equals("")) arrGscr[iUpdIndex] = "0";
					if(arrSendToPager.Length > 0) {
						if(arrAct[iUpdIndex].Equals("U")) {
							iLoggedUpd++;
							LogSQLString.Remove(0,LogSQLString.Length);
							LogSQLString.Append("insert into LOG_OTHERGOAL (TIMEFLAG, IITEMSEQ_NO, SECTION, LEAGUE, LEAGUEALIAS, HOST, GUEST, ACT, H_GOAL, G_GOAL, STATUS, MATCHDATE, MATCHTIME, ALERT, BATCHJOB) values ('");
							LogSQLString.Append(sCurrentTimestamp);
							LogSQLString.Append("',");
							LogSQLString.Append(iLoggedUpd.ToString());
							LogSQLString.Append(",'OTHGOAL_','");
							LogSQLString.Append(sLeagLong);
							LogSQLString.Append("','");
							LogSQLString.Append(sLeagAlias);
							LogSQLString.Append("','");
							LogSQLString.Append(arrHost[iUpdIndex]);
							LogSQLString.Append("','");
							LogSQLString.Append(arrGuest[iUpdIndex]);
							LogSQLString.Append("','U',");
							LogSQLString.Append(arrHscr[iUpdIndex]);
							LogSQLString.Append(",");
							LogSQLString.Append(arrGscr[iUpdIndex]);
							LogSQLString.Append(",'");
							LogSQLString.Append(statusArray[Convert.ToInt32(arrStatus[iUpdIndex])].Substring(0,1));
							LogSQLString.Append("','");
							LogSQLString.Append(arrMatchDate[iUpdIndex]);
							LogSQLString.Append("','");
							LogSQLString.Append(arrMatchTime[iUpdIndex]);
							LogSQLString.Append("','");
							for(int i = 0; i < arrAlert.Length; i++) {
								if(Convert.ToInt32(arrAlert[i]) == iUpdIndex) {
									sAlert = "1";
									break;
								}
							}
							LogSQLString.Append(sAlert);
							LogSQLString.Append("','");
							LogSQLString.Append(sBatchJob);
							LogSQLString.Append("')");
							logDBMgr.ExecuteNonQuery(LogSQLString.ToString());
							logDBMgr.Close();
						}	//Insert log into LOG_OTHERGOAL
					}
				}
				if(iLoggedUpd > 0) {
					//Modified by Henry, 09 Feb 2004
					//Send Notify Message
					sptMsg.IsTransaction = true;
					sptMsg.Body = sBatchJob;
					sptMsg.Timestamp = DateTime.Now.ToString("yyMMddHHmmss");
					sptMsg.AppID = "07";
					sptMsg.MsgID = "17";
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
							m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " MSMQ ERROR: Other Results");
							m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " OtherResults.cs.SendOthersResult(): Notify via MSMQ throws MessageQueueException:  " + mqEx.ToString());
							m_SportsLog.Close();

							//If MSMQ fail, notify via .NET Remoting
							msgClt.MessageType = arrMessageTypes[1];
							msgClt.MessagePath = arrRemotingPath[0];
							if(!msgClt.SendMessage((object)sptMsg)) {
								m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
								m_SportsLog.SetFileName(0,LOGFILESUFFIX);
								m_SportsLog.Open();
								m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Remoting ERROR: Other Results");
								m_SportsLog.Close();
							}
						}	catch(Exception ex) {
							m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
							m_SportsLog.SetFileName(0,LOGFILESUFFIX);
							m_SportsLog.Open();
							m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Remoting ERROR: Other Results");
							m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " OtherResults.cs.SendOthersResult(): Notify via .NET Remoting throws exception: " + ex.ToString());
							m_SportsLog.Close();
						}
					}	catch(Exception ex) {
						m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
						m_SportsLog.SetFileName(0,LOGFILESUFFIX);
						m_SportsLog.Open();
						m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " OtherResults.cs.SendOthersResult(): Notify via MSMQ throws exception: " + ex.ToString());
						m_SportsLog.Close();
					}	//end modify
				}

				irecord = iLoggedUpd + iLoggedDel;
				m_SportsDBMgr.Dispose();

				//write log
				m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " OtherResults.cs: Send " + irecord.ToString() + " send OtherResults records (" + HttpContext.Current.Session["user_name"] + ")");
				m_SportsLog.Close();
			} catch(Exception ex) {
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " OtherResults.cs.SendOthersResult(): " + ex.ToString());
				m_SportsLog.Close();
			}
			return irecord;
		}
	}
}