/*
Objective:
Modify a match information such as match date, time, host, guest etc.

Last updated:
7 Oct 2004, Fanny Cheung

C#.NET complier statement:

(With Replication)
csc /t:library /out:..\bin\OtherSoccerModifyMatch.dll /r:..\bin\DBManager.dll;..\bin\Files.dll;..\bin\Win32INI.dll;..\bin\Replicator.dll;..\bin\MessageClient.dll;..\bin\SportsMessage.dll OtherSoccerModifyMatch.cs

(Without Replication - current productiong version)
csc /t:library /out:..\bin\OtherSoccerModifyMatch.dll /r:..\bin\DBManager.dll;..\bin\Files.dll;..\bin\Win32INI.dll;..\bin\MessageClient.dll;..\bin\SportsMessage.dll OtherSoccerModifyMatch.cs
*/

using System;
using System.Collections;
using System.Data.OleDb;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Web;
using TDL.DB;
using TDL.IO;
using TDL.Message;
//using TDL.Util;

[assembly:AssemblyCompany("TDSL")]
[assembly:AssemblyCopyright("(c) 2003 TDSL. All rights reserved.")]
[assembly:AssemblyDescription("足球資訊 -> 修改賽事")]
[assembly:AssemblyProduct("Sports.NET")]
[assembly:AssemblyTitle("Sports.NET DLL")]
[assembly:AssemblyVersion("3.1.*")]
namespace SportsUtil {
	public class OtherSoccerModifyMatch {
		const int LOGON32_LOGON_INTERACTIVE = 2;
		const int LOGON32_PROVIDER_DEFAULT = 0;
		const int SecurityImpersonation = 2;
		const string LOGFILESUFFIX = "log";
		OleDbDataReader m_SportsOleReader;
		DBManager m_SportsDBMgr;
		Files m_SportsLog;
		Encoding m_Big5Encoded;
		StringBuilder SQLString;

		[DllImport("advapi32.dll", SetLastError=true)]
		public static extern bool LogonUser(string lpszUserName, string lpszDomain, string lpszPassword, int dwLogonType, int dwLogonProvider, ref IntPtr phToken);

		[DllImport("advapi32.dll", SetLastError=true)]
		public extern static int DuplicateToken(IntPtr hToken, int impersonationLevel, ref IntPtr hNewToken);

		public OtherSoccerModifyMatch(string Connection) {
			m_SportsDBMgr = new DBManager();
			m_SportsDBMgr.ConnectionString = Connection;
			m_SportsLog = new Files();
			m_Big5Encoded = Encoding.GetEncoding(950);
			SQLString = new StringBuilder();
		}

		public string GetMatches() {
			int iYear, iMonth, iDay, iHour, iMinute, iNoOfOpt = 0, iNewValue = 0;
			const int iYY=1, iMM=12, iDD=31, iHH=24, imm=60;
			string sRecordStr, sMatchCount, sLeagID = "", sHost, sGuest;
			const string sZeroPad = "0";
			string[] arrRecordSet;
			ArrayList teamAL = new ArrayList();
			teamAL.Capacity = 20;
			StringBuilder HTMLString = new StringBuilder();

			arrRecordSet = new string[8];
			sMatchCount = HttpContext.Current.Request.QueryString["matchcount"];

			try {
				SQLString.Remove(0,SQLString.Length);
				SQLString.Append("select leag.LEAG_ID from LEAGINFO leag, othersoccerinfo game where game.LEAGUE = leag.ALIAS and game.MATCH_CNT=");
				SQLString.Append(sMatchCount);
				m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
				if(m_SportsOleReader.Read()) sLeagID = m_SportsOleReader.GetString(0).Trim();
				m_SportsOleReader.Close();
				m_SportsDBMgr.Close();

				SQLString.Remove(0,SQLString.Length);
				SQLString.Append("select teaminfo.teamname from leaginfo, teaminfo, id_info where leaginfo.leag_id=id_info.leag_id and teaminfo.team_id=id_info.team_id and id_info.leag_id='");
				SQLString.Append(sLeagID);
				SQLString.Append("' order by teaminfo.teamname");
				m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
				while(m_SportsOleReader.Read()) {
					sRecordStr = m_SportsOleReader.GetString(0).Trim();
					teamAL.Add(sRecordStr);
				}
				m_SportsOleReader.Close();
				m_SportsDBMgr.Close();
				teamAL.TrimToSize();

				SQLString.Remove(0,SQLString.Length);
				SQLString.Append("select MATCHDATE, MATCHTIME, LEAGLONG, HOST, GUEST, INTERVAL, FIELD, HOST_HANDI from othersoccerinfo where MATCH_CNT=");
				SQLString.Append(sMatchCount);
				m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
				if(m_SportsOleReader.Read()) {
					arrRecordSet[0] = m_SportsOleReader.GetString(0);
					arrRecordSet[1] = m_SportsOleReader.GetString(1);
					arrRecordSet[2] = m_SportsOleReader.GetString(2).Trim();
					arrRecordSet[3] = m_SportsOleReader.GetString(3).Trim();
					arrRecordSet[4] = m_SportsOleReader.GetString(4).Trim();
					arrRecordSet[5] = m_SportsOleReader.GetInt32(5).ToString();
					arrRecordSet[6] = m_SportsOleReader.GetString(6).Trim();
					arrRecordSet[7] = m_SportsOleReader.GetString(7).Trim();
				}
				m_SportsOleReader.Close();
				m_SportsDBMgr.Close();
				//m_SportsDBMgr.Dispose();

				//Original Match Date and its hidden field
				sRecordStr = arrRecordSet[0];
				iYear = Convert.ToInt32(sRecordStr.Substring(0,4));
				iMonth = Convert.ToInt32(sRecordStr.Substring(4,2));
				iDay = Convert.ToInt32(sRecordStr.Substring(6,2));
				HTMLString.Append("<td><input type=\"hidden\" name=\"Org_MatchDate\" value=\"");
				HTMLString.Append(sRecordStr);
				HTMLString.Append("\"><select name=\"MatchYear\"><option value=\"");
				HTMLString.Append(iYear.ToString());
				HTMLString.Append("\">");
				HTMLString.Append(iYear.ToString());
				for(iNoOfOpt=1;iNoOfOpt<=iYY;iNoOfOpt++) {
					iNewValue = iYear+iNoOfOpt;
					HTMLString.Append("<option value=\"");
					HTMLString.Append(iNewValue.ToString());
					HTMLString.Append("\">");
					HTMLString.Append(iNewValue.ToString());
				}
				HTMLString.Append("</select>");

				HTMLString.Append("/<select name=\"MatchMonth\"><option value=\"");
				if(iMonth<10) HTMLString.Append(sZeroPad);
				HTMLString.Append(iMonth.ToString());
				HTMLString.Append("\">");
				HTMLString.Append(iMonth.ToString());
				iNoOfOpt = iMonth+1;
				while(iNoOfOpt != iMonth) {
					if(iNoOfOpt>iMM) iNoOfOpt=1;
					if(!(iMonth == 1 && iNoOfOpt == 1)) {
						HTMLString.Append("<option value=\"");
						if(iNoOfOpt<10) HTMLString.Append(sZeroPad);
						HTMLString.Append(iNoOfOpt.ToString());
						HTMLString.Append("\">");
						HTMLString.Append(iNoOfOpt.ToString());
						iNoOfOpt++;
					}
				}
				HTMLString.Append("</select>");

				HTMLString.Append("/<select name=\"MatchDate\"><option value=\"");
				if(iDay<10) HTMLString.Append(sZeroPad);
				HTMLString.Append(iDay.ToString());
				HTMLString.Append("\">");
				HTMLString.Append(iDay.ToString());
				iNoOfOpt = iDay+1;
				while(iNoOfOpt != iDay) {
					if(iNoOfOpt>iDD) iNoOfOpt=1;
					if(!(iDay == 1 && iNoOfOpt == 1)) {
						HTMLString.Append("<option value=\"");
						if(iNoOfOpt<10) HTMLString.Append(sZeroPad);
						HTMLString.Append(iNoOfOpt.ToString());
						HTMLString.Append("\">");
						HTMLString.Append(iNoOfOpt.ToString());
						iNoOfOpt++;
					}
				}
				HTMLString.Append("</select></td>");

				//Original Match Time and its hidden field
				sRecordStr = arrRecordSet[1];
				HTMLString.Append("<td><input type=\"hidden\" name=\"Org_MatchTime\" value=\"");
				HTMLString.Append(sRecordStr);
				HTMLString.Append("\">");
				iHour = Convert.ToInt32(sRecordStr.Substring(0,2));
				iMinute = Convert.ToInt32(sRecordStr.Substring(2,2));

				HTMLString.Append("<select name=\"MatchHour\"><option value=\"");
				if(iHour<10) HTMLString.Append(sZeroPad);
				HTMLString.Append(iHour.ToString());
				HTMLString.Append("\">");
				HTMLString.Append(iHour.ToString());
				iNoOfOpt = iHour+1;
				while(iNoOfOpt != iHour) {
					if(iNoOfOpt>=iHH) iNoOfOpt=0;
					if(!(iHour == 0 && iNoOfOpt == 0)) {
						HTMLString.Append("<option value=\"");
						if(iNoOfOpt<10) HTMLString.Append(sZeroPad);
						HTMLString.Append(iNoOfOpt.ToString());
						HTMLString.Append("\">");
						HTMLString.Append(iNoOfOpt.ToString());
						iNoOfOpt++;
					}
				}
				HTMLString.Append("</select>:");

				HTMLString.Append("<select name=\"MatchMinute\"><option value=\"");
				if(iMinute<10) HTMLString.Append(sZeroPad);
				HTMLString.Append(iMinute.ToString());
				HTMLString.Append("\">");
				HTMLString.Append(iMinute.ToString());
				iNoOfOpt = iMinute+1;
				while(iNoOfOpt != iMinute) {
					if(iNoOfOpt>=imm) iNoOfOpt=0;
					if(!(iMinute == 0 && iNoOfOpt == 0)) {
						HTMLString.Append("<option value=\"");
						if(iNoOfOpt<10) HTMLString.Append(sZeroPad);
						HTMLString.Append(iNoOfOpt.ToString());
						HTMLString.Append("\">");
						HTMLString.Append(iNoOfOpt.ToString());
						iNoOfOpt++;
					}
				}
				HTMLString.Append("</select></td>");

				//League
				HTMLString.Append("<td>");
				HTMLString.Append(arrRecordSet[2]);
				HTMLString.Append("<input type=\"hidden\" name=\"League\" value=\"");
				HTMLString.Append(arrRecordSet[2]);
				HTMLString.Append("\"></td>");

				//Host
				sHost = arrRecordSet[3];
				HTMLString.Append("<td><input type=\"hidden\" name=\"Org_Host\" value=\"");
				HTMLString.Append(sHost);
				HTMLString.Append("\"><select name=\"Host\"><option value=\"");
				HTMLString.Append(sHost);
				HTMLString.Append("\">");
				HTMLString.Append(sHost);
				for(iNoOfOpt=0;iNoOfOpt<teamAL.Count;iNoOfOpt++) {
					if(!teamAL[iNoOfOpt].Equals(sHost)) {
						HTMLString.Append("<option value=\"");
						HTMLString.Append(teamAL[iNoOfOpt]);
						HTMLString.Append("\">");
						HTMLString.Append(teamAL[iNoOfOpt]);
					}
				}
				HTMLString.Append("</select></td>");

				//Guest
				sGuest = arrRecordSet[4];
				HTMLString.Append("<td><input type=\"hidden\" name=\"Org_Guest\" value=\"");
				HTMLString.Append(sGuest);
				HTMLString.Append("\"><select name=\"Guest\"><option value=\"");
				HTMLString.Append(sGuest);
				HTMLString.Append("\">");
				HTMLString.Append(sGuest);
				for(iNoOfOpt=0;iNoOfOpt<teamAL.Count;iNoOfOpt++) {
					if(!teamAL[iNoOfOpt].Equals(sGuest)) {
						HTMLString.Append("<option value=\"");
						HTMLString.Append(teamAL[iNoOfOpt]);
						HTMLString.Append("\">");
						HTMLString.Append(teamAL[iNoOfOpt]);
					}
				}
				HTMLString.Append("</select></td>");

				//Interval, MatchCount (Hidden Field)
				HTMLString.Append("<input type=\"hidden\" name=\"Org_Interval\" value=\"");
				HTMLString.Append(arrRecordSet[5]);
				HTMLString.Append("\"><input type=\"hidden\" name=\"MatchCount\" value=\"");
				HTMLString.Append(sMatchCount);
				HTMLString.Append("\"><input type=\"hidden\" name=\"MatchField\" value=\"");
				HTMLString.Append(arrRecordSet[6]);
				HTMLString.Append("\"><input type=\"hidden\" name=\"HostHandicap\" value=\"");
				HTMLString.Append(arrRecordSet[7]);
				HTMLString.Append("\">");
			} catch(Exception ex) {
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " OtherSoccerModifyMatch.cs.GetMatches(): " + ex.ToString());
				m_SportsLog.Close();
				HTMLString.Remove(0,HTMLString.Length);
				HTMLString.Append((string)HttpContext.Current.Application["accessErrorMsg"]);
			}

			return HTMLString.ToString();
		}

		public int Modify() {
			int iRecUpd = 0;
			string updateQuery;
			string sAction;
			string sMatchCount;
			string sLeague;
			string sOrg_Host;
			string sOrg_Guest;
			string sMatchField;
			string sHostHandicap;
			string sOrg_MatchDate;
			string sOrg_MatchTime;
			string sCurrentTimestamp = null;
			string sBatchJob = null;
			char[] delimiter = new char[] {','};
			string[] arrMsgType;
			string[] arrSendToPager;
			//Replicator MatchReplicator = new Replicator(HttpContext.Current.Application["RepDBConnectionString"].ToString());
			//MatchReplicator.ApplicationType = 1;
			//MatchReplicator.ErrorLogPath = HttpContext.Current.Application["ErrorFilePath"].ToString();

			//SportsMessage object message
			SportsMessage sptMsg = new SportsMessage();

			try {
				arrSendToPager = HttpContext.Current.Request.Form["SendToPager"].Split(delimiter);
			} catch(Exception) {
				arrSendToPager = new string[0];
			}

			//Delcare variable used in message notify
			string[] arrQueueNames;
			string[] arrRemotingPath;
			string[] arrMessageTypes;
			arrQueueNames = (string[])HttpContext.Current.Application["QueueItems"];
			arrRemotingPath = (string[])HttpContext.Current.Application["RemotingItems"];
			arrMessageTypes = (string[])HttpContext.Current.Application["NotifyMessageTypes"];
			MessageClient msgClt = new MessageClient();

			bool m_LoggedOn = false;
			string m_NetUser = "ASPUSER";
			string m_Password = "aspspt";
			string m_Domain = System.Environment.MachineName;
			WindowsImpersonationContext m_ImpersonationContext;
			WindowsIdentity m_TempWindowsIdentity;
			IntPtr m_Token = IntPtr.Zero;
			IntPtr m_TokenDuplicate = IntPtr.Zero;

			arrMsgType = (string[])HttpContext.Current.Application["messageType"];
			sAction = HttpContext.Current.Request.Form["action"];
			sMatchCount = HttpContext.Current.Request.Form["MatchCount"];
			sLeague = HttpContext.Current.Request.Form["League"];
			sOrg_Host = HttpContext.Current.Request.Form["Org_Host"];
			sOrg_Guest = HttpContext.Current.Request.Form["Org_Guest"];
			sMatchField = HttpContext.Current.Request.Form["MatchField"];
			sHostHandicap = HttpContext.Current.Request.Form["HostHandicap"];
			sOrg_MatchDate = HttpContext.Current.Request.Form["Org_MatchDate"];
			sOrg_MatchTime = HttpContext.Current.Request.Form["Org_MatchTime"];

			try {
				if(arrSendToPager.Length>0) {
					sCurrentTimestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
					//case: delete
					if(sAction.Equals("D")) {

						//delete table directly
						SQLString.Remove(0,SQLString.Length);
						//SQLString.Append("update othersoccerinfo set ACT='D' where MATCH_CNT=");
						SQLString.Append("delete from othersoccerinfo where MATCH_CNT=");
						SQLString.Append(sMatchCount);
						m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
						m_SportsDBMgr.Close();
						//MatchReplicator.Replicate(SQLString.ToString());
						iRecUpd++;

						//delete table directly
						SQLString.Remove(0,SQLString.Length);
						//SQLString.Append("update othersoccergoalinfo set ACT='D' where MATCH_CNT=");
						SQLString.Append("delete from othersoccergoalinfo where MATCH_CNT=");
						SQLString.Append(sMatchCount);
						m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
						m_SportsDBMgr.Close();
						//MatchReplicator.Replicate(SQLString.ToString());

						sBatchJob = DateTime.Now.ToString("yyMMddHHmmss_fffffff") + "." + HttpContext.Current.Session["user_name"].ToString() + "." + arrMsgType[40] + ".del.ini";
						SQLString.Remove(0,SQLString.Length);
						SQLString.Append("insert into LOG_OTHERSOCCER_LIVEGOAL (TIMEFLAG, SECTION, LEAGUE, HOST, GUEST, ACT, MATCHDATE, MATCHTIME, MATCHFIELD, H_GOAL, G_GOAL, H_CONFIRM, G_CONFIRM, SONGID, ALERT, STATUS, COMMENT, BATCHJOB) values ('");
						SQLString.Append(sCurrentTimestamp);
						SQLString.Append("','OTHGOAL_','");
						SQLString.Append(sLeague);
						SQLString.Append("','");
						SQLString.Append(sOrg_Host);
						SQLString.Append("','");
						SQLString.Append(sOrg_Guest);
						SQLString.Append("','D','-1','-1','0',0,0,0,0,'0',0,'0','-1','");
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
						sptMsg.MsgID = "35";
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
								m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " OtherSoccerModifyMatch.cs.Modify(): Notify (OtherSoccerLiveGoal) via MSMQ throws MessageQueueException: " + mqEx.ToString());
								m_SportsLog.Close();

								//If MSMQ fail, notify via .NET Remoting
								msgClt.MessageType = arrMessageTypes[1];
								msgClt.MessagePath = arrRemotingPath[0];
								if(!msgClt.SendMessage((object)sptMsg)) {
									m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
									m_SportsLog.SetFileName(0,LOGFILESUFFIX);
									m_SportsLog.Open();
									m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Remoting ERROR: OtherSoccer Live Goal");
									m_SportsLog.Close();
								}
							}	catch(Exception ex) {
								m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
								m_SportsLog.SetFileName(0,LOGFILESUFFIX);
								m_SportsLog.Open();
								m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Remoting ERROR: Live Goal");
								m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " OtherSoccerModifyMatch.cs.Modify(): Notify (OtherSoccerLiveGoal) via .NET Remoting throws exception: " + ex.ToString());
								m_SportsLog.Close();
							}
						}	catch(Exception ex) {
							m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
							m_SportsLog.SetFileName(0,LOGFILESUFFIX);
							m_SportsLog.Open();
							m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " OtherSoccerModifyMatch.cs.Modify(): Notify (OtherSoccerLiveGoal) via MSMQ throws exception: " + ex.ToString());
							m_SportsLog.Close();
						}

						//Insert log into LOG_OTHERSOCCER
						sBatchJob = DateTime.Now.ToString("yyMMddHHmmss_fffffff") + "." + HttpContext.Current.Session["user_name"].ToString() + "." + arrMsgType[38] + ".del.ini";
						SQLString.Remove(0,SQLString.Length);
						SQLString.Append("insert into LOG_OTHERSOCCER (TIMEFLAG, SECTION, LEAGUEALIAS, LEAGUE, MATCHDATE, MATCHTIME, MATCHFIELD, HOST, GUEST, ACT, HANDICAP, T_HANDIOWNER, M_HANDIOWNER, T_HANDI, M_HANDI, T_LIVEODDS, M_LIVEODDS, T_ODDS, M_ODDS, E_ODDS1, E_ODDS2, ALERT, INTERVAL, ODDS_TREND, BATCHJOB) values ('");
						SQLString.Append(sCurrentTimestamp);
						SQLString.Append("','OTHODDS_SPT_','-1','");
						SQLString.Append(sLeague);
						SQLString.Append("','-1','-1','H','");
						SQLString.Append(sOrg_Host);
						SQLString.Append("','");
						SQLString.Append(sOrg_Guest);
						SQLString.Append("','");
						SQLString.Append(sAction);
						SQLString.Append("','1','泰','馬','-1','-1','-1','-1','-1','-1','-1','-1',0,-1,'EQ','");
						SQLString.Append(sBatchJob);
						SQLString.Append("')");
						m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
						m_SportsDBMgr.Close();

						//Tell MessageDispatcher to generate Odds INI and notify other processes
						//Assign value to SportsMessage object
						sptMsg.Body = sBatchJob;
						sptMsg.Timestamp = DateTime.Now.ToString("yyMMddHHmmss");
						sptMsg.AppID = "07";
						sptMsg.MsgID = "33";
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
								m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " MSMQ ERROR: Modify Odds");
								m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " OtherSoccerModifyMatch.cs.Modify(): Notify (Odds) via MSMQ throws MessageQueueException: " + mqEx.ToString());
								m_SportsLog.Close();

								//If MSMQ fail, notify via .NET Remoting
								msgClt.MessageType = arrMessageTypes[1];
								msgClt.MessagePath = arrRemotingPath[0];
								if(!msgClt.SendMessage((object)sptMsg)) {
									m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
									m_SportsLog.SetFileName(0,LOGFILESUFFIX);
									m_SportsLog.Open();
									m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Remoting ERROR: Modify Odds");
									m_SportsLog.Close();
								}
							}	catch(Exception ex) {
								m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
								m_SportsLog.SetFileName(0,LOGFILESUFFIX);
								m_SportsLog.Open();
								m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Remoting ERROR: Modify Odds");
								m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " OtherSoccerModifyMatch.cs.Modify(): Notify (Odds) via .NET Remoting throws exception: " + ex.ToString());
								m_SportsLog.Close();
							}
						} catch(Exception ex) {
							m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
							m_SportsLog.SetFileName(0,LOGFILESUFFIX);
							m_SportsLog.Open();
							m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " OtherSoccerModifyMatch.cs.Modify(): Notify (Odds) via MSMQ throws exception: " + ex.ToString());
							m_SportsLog.Close();
						}

						//write log
						m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
						m_SportsLog.SetFileName(0,LOGFILESUFFIX);
						m_SportsLog.Open();
						m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " ModifyMatch.cs: Mark delete <L:" + sLeague + ", H:" + sOrg_Host + ", G:" + sOrg_Guest + "> (" + HttpContext.Current.Session["user_name"] + ")");
						m_SportsLog.Close();
					}
					//case: modify
					else {
						//update match w.r.t match count
						string sMatchDate, sMatchTime, sMatchYear, sMatchMonth, sMatchDay, sMatchHour, sMatchMinute, sHost, sGuest, sOrg_Interval, sRecordStr = "";

						sMatchYear = HttpContext.Current.Request.Form["MatchYear"];
						sMatchMonth = HttpContext.Current.Request.Form["MatchMonth"];
						sMatchDay = HttpContext.Current.Request.Form["MatchDate"];
						sMatchHour = HttpContext.Current.Request.Form["MatchHour"];
						sMatchMinute = HttpContext.Current.Request.Form["MatchMinute"];
						sHost = HttpContext.Current.Request.Form["Host"];
						sGuest = HttpContext.Current.Request.Form["Guest"];
						sOrg_Interval = HttpContext.Current.Request.Form["Org_Interval"];

						StringBuilder LogSQLString = new StringBuilder();
						DBManager logDBMgr = new DBManager();
						logDBMgr.ConnectionString = m_SportsDBMgr.ConnectionString;

						//construct new match date and time
						sMatchDate = sMatchYear + sMatchMonth + sMatchDay;
						sMatchTime = sMatchHour + sMatchMinute;

						if(sMatchDate.Equals(sOrg_MatchDate) && sMatchTime.Equals(sOrg_MatchTime) && sHost.Equals(sOrg_Host) && sGuest.Equals(sOrg_Guest)) {	//Nothing to be changed
							iRecUpd = 0;

							//write log
							m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
							m_SportsLog.SetFileName(0,LOGFILESUFFIX);
							m_SportsLog.Open();
							m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " ModifyMatch.cs: Nothing to modify <L:" + sLeague + ", H:" + sOrg_Host + ", G:" + sOrg_Guest + "> (" + HttpContext.Current.Session["user_name"] + ")");
							m_SportsLog.Close();
						}	else {	//something had been changed
							//init INIFile object
							iRecUpd++;
							sBatchJob = DateTime.Now.ToString("yyMMddHHmmss_fffffff") + "." + HttpContext.Current.Session["user_name"].ToString() + "." + arrMsgType[38] + ".ini";
							LogSQLString.Remove(0,LogSQLString.Length);
							LogSQLString.Append("insert into LOG_OTHERSOCCER_MODIFYMATCH (TIMEFLAG, SECTION, CMD, PARAM1, PARAM2, PARAM3, PARAM4, PARAM5, PARAM6, PARAM7, PARAM8, PARAM9, PARAM10, PARAM11, PARAM12, PARAM13, PARAM14, PARAM15, PARAM16, PARAM17, PARAM18, BATCHJOB) values ('");
							LogSQLString.Append(sCurrentTimestamp);
							LogSQLString.Append("','COMMAND_','RN_OTHODDS_SPT_KEY','");
							LogSQLString.Append(sLeague);
							LogSQLString.Append("',");

							if(!sHost.Equals(sOrg_Host) || !sGuest.Equals(sOrg_Guest)) {	//host/guest had been modify
								//update othersoccerinfo: host, guest
								updateQuery = "update othersoccerinfo set HOST='" + sHost + "', GUEST='" + sGuest + "' where MATCH_CNT=" + sMatchCount;
								m_SportsDBMgr.ExecuteNonQuery(updateQuery);
								m_SportsDBMgr.Close();
								//MatchReplicator.Replicate(updateQuery);

								LogSQLString.Append("'");
								LogSQLString.Append(sOrg_Host);
								LogSQLString.Append("','");
								LogSQLString.Append(sHost);
								LogSQLString.Append("','");
								LogSQLString.Append(sOrg_Guest);
								LogSQLString.Append("','");
								LogSQLString.Append(sGuest);
								LogSQLString.Append("',");
							} else {	//host/guest had not been modify
								LogSQLString.Append("'");
								LogSQLString.Append(sOrg_Host);
								LogSQLString.Append("','");
								LogSQLString.Append(sOrg_Host);
								LogSQLString.Append("','");
								LogSQLString.Append(sOrg_Guest);
								LogSQLString.Append("','");
								LogSQLString.Append(sOrg_Guest);
								LogSQLString.Append("',");
							}

							if(!sMatchDate.Equals(sOrg_MatchDate) || !sMatchTime.Equals(sOrg_MatchTime)) {	//match date and time had been modified
								int iInterval = 0;
								const int minTimeSlot=65;
								double dDiff = 0;
								const double xConstant = 64;
								string sDayToAdd = null, sRefStartDateTime = null, sRefEndDateTime = null, sFormatPattern;
								TimeSpan TimeDiff;
								DateTime dtRefStart, dtRefEnd, dtMatch;
								CultureInfo formatter;

								dtRefStart = new DateTime(0);
								dtRefEnd = new DateTime(0);
								dtMatch = new DateTime(0);
								formatter = new CultureInfo("en-US");
								sFormatPattern = "yyyyMMddHHmm";

								//Start to calculate new interval
								m_LoggedOn = LogonUser(m_NetUser, m_Domain, m_Password, LOGON32_LOGON_INTERACTIVE, LOGON32_PROVIDER_DEFAULT, ref m_Token);
								if(m_LoggedOn) {
									if(DuplicateToken(m_Token, SecurityImpersonation, ref m_TokenDuplicate) != 0) {
										m_TempWindowsIdentity = new WindowsIdentity(m_TokenDuplicate);
										m_ImpersonationContext = m_TempWindowsIdentity.Impersonate();
										if(m_ImpersonationContext != null) {
											//get config value from INI file
											Win32INI CFGINI = new Win32INI(HttpContext.Current.Application["SoccerINIFilePath"].ToString());
											sRefEndDateTime = CFGINI.GetValue("SYS_PARA","REF_DATE");
											sRefStartDateTime = sRefEndDateTime;
											sRefEndDateTime += CFGINI.GetValue("SYS_PARA","END_BET_TIME");
											sRefStartDateTime += CFGINI.GetValue("SYS_PARA","START_BET_TIME");
											sDayToAdd = CFGINI.GetValue("SYS_PARA","BET_DAYS");
											m_ImpersonationContext.Undo();

											//write log
											m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
											m_SportsLog.SetFileName(0,LOGFILESUFFIX);
											m_SportsLog.Open();
											m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " OtherSoccerModifyMatch.cs.Modify(): Impersonate success.");
											m_SportsLog.Close();
										} else {
											m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
											m_SportsLog.SetFileName(0,LOGFILESUFFIX);
											m_SportsLog.Open();
											m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " OtherSoccerModifyMatch.cs.Modify(): m_ImpersonationContext is null.");
											m_SportsLog.Close();
										}
									} else {
										m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
										m_SportsLog.SetFileName(0,LOGFILESUFFIX);
										m_SportsLog.Open();
										m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " OtherSoccerModifyMatch.cs.Modify(): DuplicateToken error, code = " + Marshal.GetLastWin32Error());
										m_SportsLog.Close();
									}
								} else {
									m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
									m_SportsLog.SetFileName(0,LOGFILESUFFIX);
									m_SportsLog.Open();
									m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " OtherSoccerModifyMatch.cs.Modify(): LogonUser error, code = " + Marshal.GetLastWin32Error());
									m_SportsLog.Close();
								}

								//get reference start date time
								dtRefStart = DateTime.ParseExact(sRefStartDateTime,sFormatPattern,formatter);

								//get reference end date time
								dtRefEnd = DateTime.ParseExact(sRefEndDateTime,sFormatPattern,formatter);
								dtRefEnd = dtRefEnd.AddDays(Convert.ToDouble(sDayToAdd));

								//get Match date time
								dtMatch = DateTime.ParseExact(sMatchDate+sMatchTime,sFormatPattern,formatter);

								//Calculate interval: use for chart-plotting
								TimeDiff = dtMatch.Subtract(dtRefEnd);
								dDiff = TimeDiff.TotalSeconds;
								if(dDiff > 0) {	//outstanding match
									TimeDiff = dtRefEnd.Subtract(dtRefStart);
								} else {	//match in current period
									TimeDiff = dtMatch.Subtract(dtRefStart);
								}
								dDiff = TimeDiff.TotalSeconds;
								iInterval = (int)(dDiff/xConstant);
								if(iInterval < minTimeSlot) iInterval = minTimeSlot;

								//update othersoccerinfo: matchdate, matchtime, interval
								updateQuery = "update othersoccerinfo set MATCHDATE='" + sMatchDate + "', MATCHTIME='" + sMatchTime + "', INTERVAL=" + iInterval.ToString() + " where MATCH_CNT=" + sMatchCount;
								m_SportsDBMgr.ExecuteNonQuery(updateQuery);
								m_SportsDBMgr.Close();
								//MatchReplicator.Replicate(updateQuery);

								//update othersoccergoalinfo: matchdate, matchtime
								updateQuery = "update othersoccergoalinfo set MATCHDATE='" + sMatchDate + "', MATCHTIME='" + sMatchTime + "' where MATCH_CNT=" + sMatchCount;
								m_SportsDBMgr.ExecuteNonQuery(updateQuery);
								m_SportsDBMgr.Close();
								//MatchReplicator.Replicate(updateQuery);


								LogSQLString.Append("'");
								LogSQLString.Append(sOrg_MatchDate);
								LogSQLString.Append("','");
								LogSQLString.Append(sMatchDate);
								LogSQLString.Append("','");
								LogSQLString.Append(sOrg_MatchTime);
								LogSQLString.Append("','");
								LogSQLString.Append(sMatchTime);
								LogSQLString.Append("','");
								LogSQLString.Append(sOrg_Interval);
								LogSQLString.Append("','");
								LogSQLString.Append(iInterval.ToString());
								LogSQLString.Append("',");
							} else {	//match date and time had not been modified
								LogSQLString.Append("'");
								LogSQLString.Append(sOrg_MatchDate);
								LogSQLString.Append("','");
								LogSQLString.Append(sOrg_MatchDate);
								LogSQLString.Append("','");
								LogSQLString.Append(sOrg_MatchTime);
								LogSQLString.Append("','");
								LogSQLString.Append(sOrg_MatchTime);
								LogSQLString.Append("','");
								LogSQLString.Append(sOrg_Interval);
								LogSQLString.Append("','");
								LogSQLString.Append(sOrg_Interval);
								LogSQLString.Append("',");
							}

							if(!sHost.Equals(sOrg_Host) || !sGuest.Equals(sOrg_Guest)) {	//host/guest had been modify
								//Get type of league
								string sType = "";
								updateQuery = "select LEAGUETYPE from LEAGINFO where LEAGNAME='" + sLeague + "'";
								sType = m_SportsDBMgr.ExecuteQueryString(updateQuery);

								if(!sHost.Equals(sOrg_Host)) {	//host had been modified
									string sVenue = "";
									if(sType.Equals("1"))	//region
										updateQuery = "select CITY, VENUE from TEAMINFO where TEAMNAME='" + sHost + "'";
									else if(sType.Equals("2"))	//Euro
										updateQuery = "select COUNTRY, VENUE from TEAMINFO where TEAMNAME='" + sHost + "'";
									else	//international
										updateQuery = "select CONTINENT, VENUE from TEAMINFO where TEAMNAME='" + sHost + "'";
									m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(updateQuery);
									if(m_SportsOleReader.Read()) {
										if(m_SportsOleReader.IsDBNull(0)) sRecordStr = "";
										else sRecordStr = m_SportsOleReader.GetString(0).Trim();

										if(sRecordStr.Equals("")) sRecordStr = "-1";
										if(sType.Equals("1")) {	//region
											LogSQLString.Append("'-1','");
											LogSQLString.Append(sRecordStr);
											LogSQLString.Append("','-1',");
										}	else if(sType.Equals("2")) {	//Euro
											LogSQLString.Append("'");
											LogSQLString.Append(sRecordStr);
											LogSQLString.Append("','-1','-1',");
										}	else {
											LogSQLString.Append("'-1','-1','");
											LogSQLString.Append(sRecordStr);
											LogSQLString.Append("',");
										}

										if(m_SportsOleReader.IsDBNull(1)) sVenue = "";
										else sVenue = m_SportsOleReader.GetString(1).Trim();
									} else {
										LogSQLString.Append("'-1','-1','-1',");
									}
									m_SportsOleReader.Close();
									m_SportsDBMgr.Close();

									if(sVenue.Equals("")) {
										LogSQLString.Append("'-1',");
									} else {
										LogSQLString.Append("'");
										LogSQLString.Append(sVenue);
										LogSQLString.Append("',");
									}

								}	else {
									LogSQLString.Append("'-1','-1','-1','-1',");
								}

								if(!sGuest.Equals(sOrg_Guest)) {	//guest had been modified
									if(sType.Equals("1"))	//region
										updateQuery = "select CITY from TEAMINFO where TEAMNAME='" + sGuest + "'";
									else if(sType.Equals("2"))	//Euro
										updateQuery = "select COUNTRY from TEAMINFO where TEAMNAME='" + sGuest + "'";
									else	//international
										updateQuery = "select CONTINENT from TEAMINFO where TEAMNAME='" + sGuest + "'";
									m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(updateQuery);
									if(m_SportsOleReader.Read()) {
										if(m_SportsOleReader.IsDBNull(0)) sRecordStr = "";
										else sRecordStr = m_SportsOleReader.GetString(0).Trim();

										if(sRecordStr.Equals("")) sRecordStr = "-1";
										if(sType.Equals("1")) {	//region
											LogSQLString.Append("'-1','");
											LogSQLString.Append(sRecordStr);
											LogSQLString.Append("','-1',");
										}	else if(sType.Equals("2")) {	//Euro
											LogSQLString.Append("'");
											LogSQLString.Append(sRecordStr);
											LogSQLString.Append("','-1','-1',");
										}	else {
											LogSQLString.Append("'-1','-1','");
											LogSQLString.Append(sRecordStr);
											LogSQLString.Append("',");
										}
									}	else {
										LogSQLString.Append("'-1','-1','-1',");
									}
									m_SportsOleReader.Close();
									m_SportsDBMgr.Close();
								}	else {
									LogSQLString.Append("'-1','-1','-1',");
								}
							} else {
								LogSQLString.Append("null,null,null,null,null,null,null,");
							}
							LogSQLString.Append("'");
							LogSQLString.Append(sBatchJob);
							LogSQLString.Append("')");
							logDBMgr.ExecuteNonQuery(LogSQLString.ToString());
							logDBMgr.Close();

							//Tell MessageDispatcher to generate Match INI and notify other processes
							//Assign value to SportsMessage object
							sptMsg.Body = sBatchJob;
							sptMsg.Timestamp = DateTime.Now.ToString("yyMMddHHmmss");
							sptMsg.AppID = "07";
							sptMsg.MsgID = "34";
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
									m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " MSMQ ERROR: Modify Match");
									m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " OtherSoccerModifyMatch.cs.Modify(): Notify (Match) via MSMQ throws MessageQueueException: " + mqEx.ToString());
									m_SportsLog.Close();

									//If MSMQ fail, notify via .NET Remoting
									msgClt.MessageType = arrMessageTypes[1];
									msgClt.MessagePath = arrRemotingPath[0];
									if(!msgClt.SendMessage((object)sptMsg)) {
										m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
										m_SportsLog.SetFileName(0,LOGFILESUFFIX);
										m_SportsLog.Open();
										m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Remoting ERROR: Modify Match");
										m_SportsLog.Close();
									}
								}	catch(Exception ex) {
									m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
									m_SportsLog.SetFileName(0,LOGFILESUFFIX);
									m_SportsLog.Open();
									m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Remoting ERROR: Modify Match");
									m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " OtherSoccerModifyMatch.cs.Modify(): Notify (Match) via .NET Remoting throws exception: " + ex.ToString());
									m_SportsLog.Close();
								}
							} catch(Exception ex) {
								m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
								m_SportsLog.SetFileName(0,LOGFILESUFFIX);
								m_SportsLog.Open();
								m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " OtherSoccerModifyMatch.cs.Modify(): Notify (Match) via MSMQ throws exception: " + ex.ToString());
								m_SportsLog.Close();
							}

							//write log
							m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
							m_SportsLog.SetFileName(0,LOGFILESUFFIX);
							m_SportsLog.Open();
							m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " ModifyMatch.cs: Modify match <L:" + sLeague + ", H:" + sOrg_Host + ", G:" + sOrg_Guest + "> (" + HttpContext.Current.Session["user_name"] + ")");
							m_SportsLog.Close();
						}
					}
					//MatchReplicator.Dispose();
					//m_SportsDBMgr.Dispose();
				} else {
					iRecUpd = 0;
					m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
					m_SportsLog.SetFileName(0,LOGFILESUFFIX);
					m_SportsLog.Open();
					m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " OtherSoccerModifyMatch.cs.Modify(): Empty destination.");
					m_SportsLog.Close();
				}
			} catch(Exception ex) {
				iRecUpd = -1;
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " OtherSoccerModifyMatch.cs.Modify(): " + ex.ToString());
				m_SportsLog.Close();
			}

			return iRecUpd;
		}
	}
}