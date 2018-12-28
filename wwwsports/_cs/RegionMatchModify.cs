/*
Objective:
Modify a match information such as match date, time, host, guest etc.

Last updated:
12 Feb 2004 by HenryCi

C#.NET complier statement:
csc /t:library /out:..\bin\RegionMatchModify.dll /r:..\bin\DBManager.dll;..\bin\Files.dll;..\bin\Replicator.dll;..\bin\MessageClient.dll;..\bin\SportsMessage.dll RegionMatchModify.cs
csc /t:library /out:..\bin\RegionMatchModify.dll /r:..\bin\DBManager.dll;..\bin\Files.dll;..\bin\MessageClient.dll;..\bin\SportsMessage.dll RegionMatchModify.cs
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
//using TDL.Util;

[assembly:AssemblyCompany("TDSL")]
[assembly:AssemblyCopyright("(c) 2003 TDSL. All rights reserved.")]
[assembly:AssemblyDescription("其他地區 -> 修改賽事")]
[assembly:AssemblyProduct("Sports.NET")]
[assembly:AssemblyTitle("Sports.NET DLL")]
[assembly:AssemblyVersion("3.1.*")]
namespace SportsUtil {
	public class RegionMatchModify {
		const string LOGFILESUFFIX = "log";
		OleDbDataReader m_SportsOleReader;
		DBManager m_SportsDBMgr;
		Files m_SportsLog;
		Encoding m_Big5Encoded;
		StringBuilder SQLString;

		public RegionMatchModify(string Connection) {
			m_SportsDBMgr = new DBManager();
			m_SportsDBMgr.ConnectionString = Connection;
			m_SportsLog = new Files();
			m_Big5Encoded = Encoding.GetEncoding(950);
			SQLString = new StringBuilder();
		}

		public string GetMatches() {
			int iYear, iMonth, iDay, iHour, iMinute, iNoOfOpt = 0, iNewValue = 0;
			const int iYY=1, iMM=12, iDD=31, iHH=24, imm=60;
			string sMatchCount;
			string sLeagID = "";
			string sMatchDate = null;
			string sMatchTime = null;
			string sLeague = null;
			string sHost = null;
			string sGuest = null;
			const string sZeroPad = "0";
			ArrayList teamAL = new ArrayList();
			teamAL.Capacity = 20;
			StringBuilder HTMLString = new StringBuilder();
			ArrayList RegionList = new ArrayList(5);

			sMatchCount = HttpContext.Current.Request.QueryString["matchcnt"];
			try {
				SQLString.Remove(0,SQLString.Length);
				SQLString.Append("select leag.LEAG_ID from LEAGINFO leag, OTHERODDSINFO other where other.ALIAS = leag.ALIAS and other.MATCH_CNT=");
				SQLString.Append(sMatchCount);
				m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
				if(m_SportsOleReader.Read()) sLeagID = m_SportsOleReader.GetString(0).Trim();
				m_SportsDBMgr.Close();
				m_SportsOleReader.Close();

				SQLString.Remove(0,SQLString.Length);
				SQLString.Append("select teaminfo.teamname from leaginfo, teaminfo, id_info where leaginfo.leag_id=id_info.leag_id and teaminfo.team_id=id_info.team_id and id_info.leag_id='");
				SQLString.Append(sLeagID);
				SQLString.Append("' order by teaminfo.teamname");
				m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
				while(m_SportsOleReader.Read()) {
					teamAL.Add(m_SportsOleReader.GetString(0).Trim());
				}
				m_SportsDBMgr.Close();
				m_SportsOleReader.Close();
				teamAL.TrimToSize();

				SQLString.Remove(0,SQLString.Length);
				SQLString.Append("select LEAGUE, HOST, GUEST from OTHERODDSINFO where MATCH_CNT=");
				SQLString.Append(sMatchCount);
				m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
				while(m_SportsOleReader.Read()) {
					sLeague = m_SportsOleReader.GetString(0).Trim();
					sHost = m_SportsOleReader.GetString(1).Trim();
					sGuest = m_SportsOleReader.GetString(2).Trim();
				}
				m_SportsOleReader.Close();
				m_SportsDBMgr.Close();

				SQLString.Remove(0,SQLString.Length);
				SQLString.Append("select MATCHDATE, MATCHTIME, COMPANY from OTHERODDSINFO where LEAGUE='");
				SQLString.Append(sLeague);
				SQLString.Append("' and HOST='");
				SQLString.Append(sHost);
				SQLString.Append("' and GUEST='");
				SQLString.Append(sGuest);
				SQLString.Append("'");
				m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
				while(m_SportsOleReader.Read()) {
					sMatchDate = m_SportsOleReader.GetString(0);
					sMatchTime = m_SportsOleReader.GetString(1);
					RegionList.Add(m_SportsOleReader.GetString(2).Trim());
				}
				m_SportsOleReader.Close();
				m_SportsDBMgr.Close();
				m_SportsDBMgr.Dispose();
				RegionList.TrimToSize();

				//Original Match Date and its hidden field
				iYear = Convert.ToInt32(sMatchDate.Substring(0,4));
				iMonth = Convert.ToInt32(sMatchDate.Substring(4,2));
				iDay = Convert.ToInt32(sMatchDate.Substring(6,2));
				HTMLString.Append("<td><input type=\"hidden\" name=\"Org_MatchDate\" value=\"");
				HTMLString.Append(sMatchDate);
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
				HTMLString.Append("<td><input type=\"hidden\" name=\"Org_MatchTime\" value=\"");
				HTMLString.Append(sMatchTime);
				HTMLString.Append("\">");
				iHour = Convert.ToInt32(sMatchTime.Substring(0,2));
				iMinute = Convert.ToInt32(sMatchTime.Substring(2,2));

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
				HTMLString.Append(sLeague);
				HTMLString.Append("<input type=\"hidden\" name=\"League\" value=\"");
				HTMLString.Append(sLeague);
				HTMLString.Append("\"></td>");

				//Host
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

				//Company List (Hidden Field)
				for(int i = 0; i < RegionList.Count; i++) {
					HTMLString.Append("<input type=\"hidden\" name=\"company\" value=\"");
					HTMLString.Append(RegionList[i]);
					HTMLString.Append("\">");
				}
			} catch(Exception ex) {
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " RegionMatchModify.cs.GetMatches(): " + ex.ToString());
				m_SportsLog.Close();
				HTMLString.Remove(0,HTMLString.Length);
				HTMLString.Append((string)HttpContext.Current.Application["accessErrorMsg"]);
			}

			return HTMLString.ToString();
		}

		public int Modify() {
			int iRecUpd = 0;
			string sLeague;
			string sOrg_Host;
			string sOrg_Guest;
			string sCurrentTimestamp = null;
			string sBatchJob = null;
			char[] delimiter = new char[] {','};
			string[] arrMsgType;
			string[] arrSendToPager;
			string[] arrCompany;
/*
			Replicator MatchReplicator = new Replicator(HttpContext.Current.Application["RepDBConnectionString"].ToString());
			MatchReplicator.ApplicationType = 1;
			MatchReplicator.ErrorLogPath = HttpContext.Current.Application["ErrorFilePath"].ToString();
*/
			//SportsMessage object message
			SportsMessage sptMsg = new SportsMessage();

			try {
				arrCompany = HttpContext.Current.Request.Form["company"].Split(delimiter);
			} catch(Exception) {
				arrCompany = new string[0];
			}

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

			arrMsgType = (string[])HttpContext.Current.Application["messageType"];
			sLeague = HttpContext.Current.Request.Form["League"];
			sOrg_Host = HttpContext.Current.Request.Form["Org_Host"];
			sOrg_Guest = HttpContext.Current.Request.Form["Org_Guest"];

			try {
				if(arrSendToPager.Length>0) {
					string sMatchDate;
					string sMatchTime;
					string sMatchYear;
					string sMatchMonth;
					string sMatchDay;
					string sMatchHour;
					string sMatchMinute;
					string sHost;
					string sGuest;
					string sOrg_MatchDate;
					string sOrg_MatchTime;

					sMatchYear = HttpContext.Current.Request.Form["MatchYear"];
					sMatchMonth = HttpContext.Current.Request.Form["MatchMonth"];
					sMatchDay = HttpContext.Current.Request.Form["MatchDate"];
					sMatchHour = HttpContext.Current.Request.Form["MatchHour"];
					sMatchMinute = HttpContext.Current.Request.Form["MatchMinute"];
					sHost = HttpContext.Current.Request.Form["Host"];
					sGuest = HttpContext.Current.Request.Form["Guest"];
					sOrg_MatchDate = HttpContext.Current.Request.Form["Org_MatchDate"];
					sOrg_MatchTime = HttpContext.Current.Request.Form["Org_MatchTime"];

					//construct new match date and time
					sMatchDate = sMatchYear + sMatchMonth + sMatchDay;
					sMatchTime = sMatchHour + sMatchMinute;

					sCurrentTimestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
					sBatchJob = DateTime.Now.ToString("yyMMddHHmmss_fffffff") + "." + HttpContext.Current.Session["user_name"].ToString() + "." + arrMsgType[27] + ".ini";
					if(sMatchDate.Equals(sOrg_MatchDate) && sMatchTime.Equals(sOrg_MatchTime) && sHost.Equals(sOrg_Host) && sGuest.Equals(sOrg_Guest)) {	//Nothing to be changed
						iRecUpd = 0;

						//write log
						m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
						m_SportsLog.SetFileName(0,LOGFILESUFFIX);
						m_SportsLog.Open();
						m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " RegionMatchModify.cs: Nothing to modify <L:" + sLeague + ", H:" + sOrg_Host + ", G:" + sOrg_Guest + "> (" + HttpContext.Current.Session["user_name"] + ")");
						m_SportsLog.Close();
					}	else {	//something had been changed
						//init INIFile object
						for(int i = 0; i <arrCompany.Length; i++) {
							iRecUpd++;
							SQLString.Remove(0,SQLString.Length);
							SQLString.Append("insert into LOG_OTHERODDSMATCHMODIFY (TIMEFLAG, SECTION, CMD, PARAM1, PARAM2, PARAM3, PARAM4, PARAM5, PARAM6, PARAM7, PARAM8, PARAM9, PARAM10, BATCHJOB) values ('");
							SQLString.Append(sCurrentTimestamp);
							SQLString.Append("','COMMAND_','RN_OTHODDS_KEY','");
							SQLString.Append(sLeague);
							SQLString.Append("','");
							SQLString.Append(sOrg_Host);
							SQLString.Append("','");
							SQLString.Append(sHost);
							SQLString.Append("','");
							SQLString.Append(sOrg_Guest);
							SQLString.Append("','");
							SQLString.Append(sGuest);
							SQLString.Append("','");
							SQLString.Append(sOrg_MatchDate);
							SQLString.Append("','");
							SQLString.Append(sMatchDate);
							SQLString.Append("','");
							SQLString.Append(sOrg_MatchTime);
							SQLString.Append("','");
							SQLString.Append(sMatchTime);
							SQLString.Append("','");
							SQLString.Append(arrCompany[i]);
							SQLString.Append("','");
							SQLString.Append(sBatchJob);
							SQLString.Append("')");
							m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
							m_SportsDBMgr.Close();
						}

						//update otheroddsinfo: host, guest
						SQLString.Remove(0,SQLString.Length);
						SQLString.Append("update OTHERODDSINFO set MATCHDATE='");
						SQLString.Append(sMatchDate);
						SQLString.Append("', MATCHTIME='");
						SQLString.Append(sMatchTime);
						SQLString.Append("', HOST='");
						SQLString.Append(sHost);
						SQLString.Append("', GUEST='");
						SQLString.Append(sGuest);
						SQLString.Append("' where LEAGUE='");
						SQLString.Append(sLeague);
						SQLString.Append("' and HOST='");
						SQLString.Append(sOrg_Host);
						SQLString.Append("' and GUEST='");
						SQLString.Append(sOrg_Guest);
						SQLString.Append("'");
						m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
						m_SportsDBMgr.Close();
						//MatchReplicator.Replicate(SQLString.ToString());

						//Tell MessageDispatcher to generate Modify Match of Other Odds INI and notify other processes
						//Assign value to SportsMessage object
						sptMsg.Body = sBatchJob;
						sptMsg.Timestamp = DateTime.Now.ToString("yyMMddHHmmss");
						sptMsg.AppID = "07";
						sptMsg.MsgID = "15";
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
								m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " MSMQ ERROR: Modify Match of Other Region Odds");
								m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " RegionMatchModify.cs.Modify(): Notify via MSMQ throws MessageQueueException: " + mqEx.ToString());
								m_SportsLog.Close();

								//If MSMQ fail, notify via .NET Remoting
								msgClt.MessageType = arrMessageTypes[1];
								msgClt.MessagePath = arrRemotingPath[0];
								if(!msgClt.SendMessage((object)sptMsg)) {
									m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
									m_SportsLog.SetFileName(0,LOGFILESUFFIX);
									m_SportsLog.Open();
									m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Remoting ERROR: Modify Match of Other Region Odds");
									m_SportsLog.Close();
								}
							}	catch(Exception ex) {
								m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
								m_SportsLog.SetFileName(0,LOGFILESUFFIX);
								m_SportsLog.Open();
								m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Remoting ERROR: Modify Match of Other Region Odds");
								m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " RegionMatchModify.cs.Modify(): Notify via .NET Remoting throws exception: " + ex.ToString());
								m_SportsLog.Close();
							}
						} catch(Exception ex) {
							m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
							m_SportsLog.SetFileName(0,LOGFILESUFFIX);
							m_SportsLog.Open();
							m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " RegionMatchModify.cs.Modify(): Notify via MSMQ throws exception: " + ex.ToString());
							m_SportsLog.Close();
						}

						//write log
						m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
						m_SportsLog.SetFileName(0,LOGFILESUFFIX);
						m_SportsLog.Open();
						m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " RegionMatchModify.cs: Modify match <L:" + sLeague + ", H:" + sOrg_Host + ", G:" + sOrg_Guest + "> (" + HttpContext.Current.Session["user_name"] + ")");
						m_SportsLog.Close();
					}
				}
				//MatchReplicator.Dispose();
				if(iRecUpd > 0) m_SportsDBMgr.Dispose();
			} catch(Exception ex) {
				iRecUpd = -1;
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " RegionMatchModify.cs.Modify(): " + ex.ToString());
				m_SportsLog.Close();
			}

			return iRecUpd;
		}
	}
}