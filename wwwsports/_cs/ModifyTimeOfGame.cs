/*
Objective:
Modify time of game for live goal

Last updated:
09 Feb 2004 by HenryCi

C#.NET complier statement:
csc /t:library /out:..\bin\ModifyTimeOfGame.dll /r:..\bin\DBManager.dll;..\bin\Files.dll;..\bin\MessageClient.dll;..\bin\SportsMessage.dll ModifyTimeOfGame.cs
*/

using System;
using System.Data.OleDb;
using System.Reflection;
using System.Text;
using System.Web;
using TDL.DB;
using TDL.IO;
using TDL.Message;

[assembly:AssemblyCompany("TDSL")]
[assembly:AssemblyCopyright("(c) 2003 TDSL. All rights reserved. Created on 22 July 2003.")]
[assembly:AssemblyDescription("足球資訊 -> 現場比數(修改進行時間)")]
[assembly:AssemblyProduct("Sports.NET")]
[assembly:AssemblyTitle("Sports.NET DLL")]
[assembly:AssemblyVersion("3.1.*")]
namespace SportsUtil {
	public class ModifyTimeOfGame {
		const string LOGFILESUFFIX = "log";
		OleDbDataReader m_SportsOleReader;
		DBManager m_SportsDBMgr;
		Files m_SportsLog;
		Encoding m_Big5Encoded;
		StringBuilder SQLString;

		public ModifyTimeOfGame(string Connection) {
			m_SportsDBMgr = new DBManager();
			m_SportsDBMgr.ConnectionString = Connection;
			m_SportsLog = new Files();
			m_Big5Encoded = Encoding.GetEncoding(950);
			SQLString = new StringBuilder();
		}

		public string GetMatch() {
			string sRecordStr;
			string sMatchStatus;
			string sMatchCount;
			string[] statusArray;
			DateTime dtStart;
			StringBuilder HTMLString = new StringBuilder();

			sMatchCount = HttpContext.Current.Request.QueryString["matchcnt"];
			statusArray = (string[])HttpContext.Current.Application["matchItemsArray"];

			SQLString.Remove(0,SQLString.Length);
			SQLString.Append("select gameinfo.matchdate, gameinfo.matchtime, gameinfo.field, goalinfo.status, gameinfo.league, gameinfo.leaglong, gameinfo.host, goalinfo.h_scr, ");
			SQLString.Append("gameinfo.guest, goalinfo.g_scr, (select STARTTIME from TIMEOFGAME_INFO where IMATCH_CNT=goalinfo.match_cnt) STARTTIME, goalinfo.remark from gameinfo, goalinfo ");
			SQLString.Append("where gameinfo.match_cnt = goalinfo.match_cnt and goalinfo.match_cnt=");
			SQLString.Append(sMatchCount);
			try {
				m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
				if(m_SportsOleReader.Read()) {
					HTMLString.Append("<tr align=\"center\"><input type=\"hidden\" name=\"MatchCount\" value=\"");
					HTMLString.Append(sMatchCount);
					HTMLString.Append("\">");

					//Match Date
					sRecordStr = m_SportsOleReader.GetString(0);
					HTMLString.Append("<td><input type=\"hidden\" name=\"MatchDate\" value=\"");
					HTMLString.Append(sRecordStr);
					HTMLString.Append("\">");
					sRecordStr = sRecordStr.Insert(4,"/");
					sRecordStr = sRecordStr.Insert(7,"/");
					HTMLString.Append(sRecordStr);
					HTMLString.Append("</td>");

					//Match Time
					sRecordStr = m_SportsOleReader.GetString(1);
					HTMLString.Append("<td><input type=\"hidden\" name=\"MatchTime\" value=\"");
					HTMLString.Append(sRecordStr);
					HTMLString.Append("\">");
					sRecordStr = sRecordStr.Insert(2,":");
					HTMLString.Append(sRecordStr);

					//Match Field (Hidden field)
					sRecordStr = m_SportsOleReader.GetString(2);
					HTMLString.Append("<input type=\"hidden\" name=\"MatchField\" value=\"");
					HTMLString.Append(sRecordStr);
					HTMLString.Append("\"></td>");

					//Match Status
					sMatchStatus = m_SportsOleReader.GetString(3);
					HTMLString.Append("<td>");
					HTMLString.Append(statusArray[Convert.ToInt32(sMatchStatus)]);
					HTMLString.Append("<input type=\"hidden\" name=\"status\" value=\"");
					HTMLString.Append(sMatchStatus);
					HTMLString.Append("\"></td>");

					//League
					sRecordStr = m_SportsOleReader.GetString(4).Trim();
					HTMLString.Append("<td>");
					HTMLString.Append(sRecordStr);
					HTMLString.Append("<input type=\"hidden\" name=\"Alias\" value=\"");
					HTMLString.Append(sRecordStr);
					HTMLString.Append("\"><input type=\"hidden\" name=\"League\" value=\"");
					HTMLString.Append(m_SportsOleReader.GetString(5).Trim());
					HTMLString.Append("\"></td>");

					//Host Name
					sRecordStr = m_SportsOleReader.GetString(6).Trim();
					HTMLString.Append("<td>");
					HTMLString.Append(sRecordStr);
					HTMLString.Append("<input type=\"hidden\" name=\"Host\" value=\"");
					HTMLString.Append(sRecordStr);
					HTMLString.Append("\"></td>");

					//Host Score
					sRecordStr = m_SportsOleReader.GetString(7).Trim();
					HTMLString.Append("<td>");
					HTMLString.Append(sRecordStr);
					HTMLString.Append("<input type=\"hidden\" name=\"hostScore\" value=\"");
					HTMLString.Append(sRecordStr);
					HTMLString.Append("\"></td>");

					//Guest Name
					sRecordStr = m_SportsOleReader.GetString(8).Trim();
					HTMLString.Append("<td>");
					HTMLString.Append(sRecordStr);
					HTMLString.Append("<input type=\"hidden\" name=\"Guest\" value=\"");
					HTMLString.Append(sRecordStr);
					HTMLString.Append("\"></td>");

					//Guest Score
					sRecordStr = m_SportsOleReader.GetString(9).Trim();
					HTMLString.Append("<td>");
					HTMLString.Append(sRecordStr);
					HTMLString.Append("<input type=\"hidden\" name=\"guestScore\" value=\"");
					HTMLString.Append(sRecordStr);
					HTMLString.Append("\"></td>");

					//Time Of Game
					HTMLString.Append("<td><input type=\"text\" name=\"timeofgame\" maxlength=\"3\" size=\"2\" value=\"");
					if(!m_SportsOleReader.IsDBNull(10)) {
						if(sMatchStatus.Equals("0")) {
							HTMLString.Append("0");
						}	else if(sMatchStatus.Equals("1") || sMatchStatus.Equals("3") || sMatchStatus.Equals("5")) {
							dtStart = m_SportsOleReader.GetDateTime(10);
							HTMLString.Append(((int)DateTime.Now.Subtract(dtStart).TotalMinutes).ToString());
						}
					}
					HTMLString.Append("\" onChange=\"onTimeOfGameChanged()\"></td>");

					//Remark
					if(m_SportsOleReader.IsDBNull(11)) sRecordStr = "";
					else sRecordStr = m_SportsOleReader.GetString(11).Trim();
					HTMLString.Append("<input type=\"hidden\" name=\"remark\" value=\"");
					HTMLString.Append(sRecordStr);
					HTMLString.Append("\"></tr>");
				} else {
					HTMLString.Append("<tr><th colspan=\"9\">沒有賽事</th></tr>");
				}
				m_SportsDBMgr.Close();
				m_SportsOleReader.Close();
				m_SportsDBMgr.Dispose();
			} catch(Exception ex) {
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " ModifyTimeOfGame.cs.GetMatch(): " + ex.ToString());
				m_SportsLog.Close();
				HTMLString.Remove(0,HTMLString.Length);
				HTMLString.Append((string)HttpContext.Current.Application["accessErrorMsg"]);
			}

			return HTMLString.ToString();
		}

		public int UpdateStartTime() {
			int iTimeActivated = 0;
			int iRecIndex = 0;
			int iTimeDiff = 0;
			string sMatchCount;
			string sMatchDate;
			string sMatchTime;
			string sMatchField;
			string sAlias;
			string sLeague;
			string sStatus;
			string sHost;
			string sGuest;
			string sHostScore;
			string sGuestScore;
			string sTimeOfGame;
			string sRemark;
			string sCurrentTimestamp = null;
			string sBatchJob = null;
			string[] arrSendToPager;
			char[] delimiter = new char[] {','};

			//SportsMessage object message
			SportsMessage sptMsg = new SportsMessage();

			try {
				arrSendToPager = HttpContext.Current.Request.Form["SendToPager"].Split(delimiter);
			} catch(Exception) {
				arrSendToPager = new string[0];
			}
			sMatchCount = HttpContext.Current.Request.Form["MatchCount"];
			sMatchDate = HttpContext.Current.Request.Form["MatchDate"];
			sMatchTime = HttpContext.Current.Request.Form["MatchTime"];
			sMatchField = HttpContext.Current.Request.Form["MatchField"];
			sAlias = HttpContext.Current.Request.Form["Alias"];
			sLeague = HttpContext.Current.Request.Form["League"];
			sStatus = HttpContext.Current.Request.Form["status"];
			sHost = HttpContext.Current.Request.Form["Host"];
			sGuest = HttpContext.Current.Request.Form["Guest"];
			sHostScore = HttpContext.Current.Request.Form["hostScore"];
			sGuestScore = HttpContext.Current.Request.Form["guestScore"];
			sTimeOfGame = HttpContext.Current.Request.Form["timeofgame"];
			if(sTimeOfGame == null) sTimeOfGame = "";
			else sTimeOfGame = sTimeOfGame.Trim();
			sRemark = HttpContext.Current.Request.Form["remark"];
			if(sRemark == null) sRemark = "";
			if(sRemark.Equals("")) sRemark = "-1";

			try {
				string sTempItem;
				string sINIFileName;
				string[] arrShortStatus;
				string[] arrMsgType;
				string[] arrOddsItems;
				string[] arrRemotingPath;
				arrMsgType = (string[])HttpContext.Current.Application["messageType"];
				arrOddsItems = (string[])HttpContext.Current.Application["oddsItemsArray"];
				sINIFileName = HttpContext.Current.Session["user_name"].ToString() + "." + arrMsgType[2] + ".ini";
				arrShortStatus = (string[])HttpContext.Current.Application["matchItemsArray"];
				arrRemotingPath = (string[])HttpContext.Current.Application["RemotingItems"];

				//Delcare variable used in message notify
				string[] arrQueueNames;
				string[] arrMessageTypes;
				arrQueueNames = (string[])HttpContext.Current.Application["QueueItems"];
				arrMessageTypes = (string[])HttpContext.Current.Application["NotifyMessageTypes"];
				MessageClient msgClt = new MessageClient();

				iTimeActivated = m_SportsDBMgr.ExecuteScalar("select count(IMATCH_CNT) from TIMEOFGAME_INFO where IMATCH_CNT=" + sMatchCount);
				m_SportsDBMgr.Close();

				if(!sTimeOfGame.Equals("")) {
					double dTimeOfGame = Convert.ToDouble(sTimeOfGame);
					if(iTimeActivated > 0) {
						m_SportsDBMgr.ExecuteNonQuery("update TIMEOFGAME_INFO set STARTTIME='" + DateTime.Now.Subtract(TimeSpan.FromMinutes(dTimeOfGame)).ToString("yyyy-MM-dd HH:mm:ss") + "', CSTATUS='" + sStatus + "' where IMATCH_CNT=" + sMatchCount);
					} else {
						m_SportsDBMgr.ExecuteNonQuery("insert into TIMEOFGAME_INFO values(" + sMatchCount + ",'" + DateTime.Now.Subtract(TimeSpan.FromMinutes(dTimeOfGame)).ToString("yyyy-MM-dd HH:mm:ss") + "','" + sStatus + "')");
					}

					//write log
					m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
					m_SportsLog.SetFileName(0,LOGFILESUFFIX);
					m_SportsLog.Open();
					m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " ModifyTimeOfGame.cs: Modify time of game (" + HttpContext.Current.Session["user_name"] + ")");
					m_SportsLog.Close();
				} else {
					if(iTimeActivated > 0) {
						m_SportsDBMgr.ExecuteNonQuery("update TIMEOFGAME_INFO set STARTTIME=null, CSTATUS='" + sStatus + "' where IMATCH_CNT=" + sMatchCount);
					} else {
						m_SportsDBMgr.ExecuteNonQuery("insert into TIMEOFGAME_INFO values(" + sMatchCount + ",null,'" + sStatus + "')");
					}

					//write log
					m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
					m_SportsLog.SetFileName(0,LOGFILESUFFIX);
					m_SportsLog.Open();
					m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " ModifyTimeOfGame.cs: Reset time of game to null (" + HttpContext.Current.Session["user_name"] + ")");
					m_SportsLog.Close();
				}

				iRecIndex = 1;
				if(arrSendToPager.Length>0) {
					sCurrentTimestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
					sBatchJob = DateTime.Now.ToString("yyMMddHHmmss_fffffff") + "." + HttpContext.Current.Session["user_name"].ToString() + "." + arrMsgType[2] + ".ini";

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
					SQLString.Append(sHostScore);
					SQLString.Append(",");
					SQLString.Append(sGuestScore);
					SQLString.Append(",1,1,'0112',0,'");
					sTempItem = arrShortStatus[Convert.ToInt32(sStatus)];
					SQLString.Append(sTempItem.Substring(0,1));
					SQLString.Append("','");
					SQLString.Append(sRemark);
					SQLString.Append("',");
					SQLString.Append(iTimeDiff.ToString());
					SQLString.Append(",'");
					SQLString.Append(sBatchJob);
					SQLString.Append("')");
					m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
					m_SportsDBMgr.Close();

					//Send Notify Message
					//modified by Henry, 09 Feb 2004
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
							m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " MSMQ ERROR: Modify Time of Game");
							m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " ModifyTimeOfGame.cs.UpdateStartTime(): Notify via MSMQ throws MessageQueueException: " + mqEx.ToString());
							m_SportsLog.Close();
							
							//If MSMQ fail, notify via .NET Remoting
							msgClt.MessageType = arrMessageTypes[1];
							msgClt.MessagePath = arrRemotingPath[0];
							if(!msgClt.SendMessage((object)sptMsg)) {
								m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
								m_SportsLog.SetFileName(0,LOGFILESUFFIX);
								m_SportsLog.Open();
								m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Remoting ERROR: Modify Time of Game");
								m_SportsLog.Close();
							}
						}	catch(Exception ex) {
							m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
							m_SportsLog.SetFileName(0,LOGFILESUFFIX);
							m_SportsLog.Open();
							m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Remoting ERROR: Modify Time of Game");									
							m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " ModifyTimeOfGame.cs.UpdateStartTime(): Notify via .NET Remoting throws exception: " + ex.ToString());
							m_SportsLog.Close();
						}							
					}	catch(Exception ex) {
						m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
						m_SportsLog.SetFileName(0,LOGFILESUFFIX);
						m_SportsLog.Open();
						m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " ModifyTimeOfGame.cs.UpdateStartTime(): Notify via MSMQ throws exception:  " + ex.ToString());
						m_SportsLog.Close();
					}		
					
					//modified end
				}
				m_SportsDBMgr.Dispose();
			} catch(Exception ex) {
				iRecIndex = -1;
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " ModifyTimeOfGame.cs.UpdateLiveGoals(): " + ex.ToString());
				m_SportsLog.Close();
			}
			return iRecIndex;
		}
	}
}