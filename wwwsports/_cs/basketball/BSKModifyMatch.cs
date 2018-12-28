/*
Objective:
Modify Matches

Last updated:
20 Feb 2004, Chapman Choi

C#.NET complier statement:
csc /t:library /out:..\..\bin\BSKModifyMatch.dll /r:..\..\bin\DBManager.dll;..\..\bin\Files.dll;..\..\bin\MessageClient.dll;..\..\bin\SportsMessage.dll BSKModifyMatch.cs
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
[assembly:AssemblyDescription("籃球資訊 -> 修改賽事")]
[assembly:AssemblyProduct("Sports.NET")]
[assembly:AssemblyTitle("Sports.NET DLL")]
[assembly:AssemblyVersion("3.1.*")]
namespace SportsUtil {
	public class BSKModifyMatch {
		const string LOGFILESUFFIX = "log";
		OleDbDataReader m_SportsOleReader;
		DBManager m_SportsDBMgr;
		Files m_SportsLog;
		StringBuilder SQLString;

		public BSKModifyMatch(string Connection) {
			m_SportsDBMgr = new DBManager();
			m_SportsDBMgr.ConnectionString = Connection;
			m_SportsLog = new Files();
			SQLString = new StringBuilder();
		}

		public string GetMatches() {
			int iYear, iMonth, iDay, iHour, iMinute, iNoOfOpt = 0, iNewValue = 0;
			const int iYY=1, iMM=12, iDD=31, iHH=24, imm=60;
			string sRecordStr, sMatchCount, sLeagID = "", sHost, sGuest;
			const string sZeroPad = "0";
			string[] arrRecordSet;
			int iAlertType = 0;
			ArrayList teamAL = new ArrayList();
			teamAL.Capacity = 20;
			StringBuilder HTMLString = new StringBuilder();

			arrRecordSet = new string[13];
			sMatchCount = HttpContext.Current.Request.QueryString["leagID"];

			try {
				SQLString.Remove(0,SQLString.Length);
				SQLString.Append("select leag.CLEAG_ID from LEAGUE_INFO leag, NBAGAME_INFO game where game.CLEAG = leag.CLEAGUE and game.IMATCH_COUNT=");
				SQLString.Append(sMatchCount);
				m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
				if(m_SportsOleReader.Read()) sLeagID = m_SportsOleReader.GetString(0).Trim();
				m_SportsDBMgr.Close();
				m_SportsOleReader.Close();

				SQLString.Remove(0,SQLString.Length);
				SQLString.Append("select team_info.CTEAM from league_info, team_info, idmap_info where league_info.CLEAG_ID=idmap_info.CLEAG_ID and team_info.CTEAM_ID=idmap_info.CTEAM_ID and idmap_info.CLEAG_ID='");
				SQLString.Append(sLeagID);
				SQLString.Append("' order by team_info.CTEAM");
				m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
				while(m_SportsOleReader.Read()) {
					sRecordStr = m_SportsOleReader.GetString(0).Trim();
					teamAL.Add(sRecordStr);
				}
				m_SportsDBMgr.Close();
				m_SportsOleReader.Close();
				teamAL.TrimToSize();

				SQLString.Remove(0,SQLString.Length);
				SQLString.Append("select CMATCH_DATE, CMATCH_TIME, CLEAG, CHOST, CGUEST,CMATCH_FIELD,CHOST_HANDI,CSCORE_HANDI,CSH_ODDS,CSCORE_TARGET,CST_ODDS,CWH_ODDS,CWG_ODDS,IALERT_TYRE from NBAGAME_INFO where IMATCH_COUNT=");
				SQLString.Append(sMatchCount);
				m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
				if(m_SportsOleReader.Read()) {
					arrRecordSet[0] = m_SportsOleReader.GetString(0);
					arrRecordSet[1] = m_SportsOleReader.GetString(1);
					arrRecordSet[2] = m_SportsOleReader.GetString(2).Trim();
					arrRecordSet[3] = m_SportsOleReader.GetString(3).Trim();
					arrRecordSet[4] = m_SportsOleReader.GetString(4).Trim();
					arrRecordSet[5] = m_SportsOleReader.GetString(5).Trim();
					arrRecordSet[6] = m_SportsOleReader.GetString(6).Trim();
					arrRecordSet[7] = m_SportsOleReader.GetString(7).Trim();
					arrRecordSet[8] = m_SportsOleReader.GetString(8).Trim();
					arrRecordSet[9] = m_SportsOleReader.GetString(9).Trim();
					arrRecordSet[10] = m_SportsOleReader.GetString(10).Trim();
					arrRecordSet[11] = m_SportsOleReader.GetString(11).Trim();
					arrRecordSet[12] = m_SportsOleReader.GetString(12).Trim();
					iAlertType = m_SportsOleReader.GetInt32(13);
				}
				m_SportsDBMgr.Close();
				m_SportsOleReader.Close();
				m_SportsDBMgr.Dispose();

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
				HTMLString.Append("</select>/<select name=\"MatchMonth\"><option value=\"");
				if(iMonth<10) HTMLString.Append( sZeroPad);
				HTMLString.Append(iMonth.ToString());
				HTMLString.Append("\">");
				HTMLString.Append(iMonth.ToString());
				iNoOfOpt = iMonth+1;
				while(iNoOfOpt != iMonth) {
					if(iNoOfOpt>iMM) iNoOfOpt=1;
					if(!(iMonth == 1 && iNoOfOpt == 1)) {
						HTMLString.Append("<option value=\"");
						if(iNoOfOpt<10) HTMLString.Append( sZeroPad);
						HTMLString.Append(iNoOfOpt.ToString());
						HTMLString.Append("\">");
						HTMLString.Append(iNoOfOpt.ToString());
						iNoOfOpt++;
					}
				}
				HTMLString.Append("</select>/<select name=\"MatchDate\"><option value=\"");
				if(iDay<10) HTMLString.Append( sZeroPad);
				HTMLString.Append(iDay.ToString());
				HTMLString.Append("\">");
				HTMLString.Append(iDay.ToString());
				iNoOfOpt = iDay+1;
				while(iNoOfOpt != iDay) {
					if(iNoOfOpt>iDD) iNoOfOpt=1;
					if(!(iDay == 1 && iNoOfOpt == 1)) {
						HTMLString.Append("<option value=\"");
						if(iNoOfOpt<10) HTMLString.Append( sZeroPad);
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
				if(iHour<10) HTMLString.Append( sZeroPad);
				HTMLString.Append(iHour.ToString() );
				HTMLString.Append("\">");
				HTMLString.Append(iHour.ToString());
				iNoOfOpt = iHour+1;
				while(iNoOfOpt != iHour) {
					if(iNoOfOpt>=iHH) iNoOfOpt=0;
					if(!(iHour == 0 && iNoOfOpt == 0)) {
						HTMLString.Append("<option value=\"");
						if(iNoOfOpt<10) HTMLString.Append( sZeroPad);
						HTMLString.Append(iNoOfOpt.ToString());
						HTMLString.Append("\">");
						HTMLString.Append(iNoOfOpt.ToString());
						iNoOfOpt++;
					}
				}
				HTMLString.Append("</select>:<select name=\"MatchMinute\"><option value=\"");
				if(iMinute<10) HTMLString.Append( sZeroPad);
				HTMLString.Append(iMinute.ToString());
				HTMLString.Append("\">");
				HTMLString.Append(iMinute.ToString());
				iNoOfOpt = iMinute+1;
				while(iNoOfOpt != iMinute) {
					if(iNoOfOpt>=imm) iNoOfOpt=0;
					if(!(iMinute == 0 && iNoOfOpt == 0)) {
						HTMLString.Append("<option value=\"");
						if(iNoOfOpt<10) HTMLString.Append( sZeroPad);
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

				// MatchCount (Hidden Field)				
				HTMLString.Append("<input type=\"hidden\" name=\"MatchCount\" value=\"");
				HTMLString.Append(sMatchCount);
				HTMLString.Append("\"><input type=\"hidden\" name=\"MatchField\" value=\"");
				HTMLString.Append(arrRecordSet[5]);
				HTMLString.Append("\"><input type=\"hidden\" name=\"HostHandi\" value=\"");
				HTMLString.Append(arrRecordSet[6]);
				HTMLString.Append("\"><input type=\"hidden\" name=\"ScoreHandi\" value=\"");
				HTMLString.Append(arrRecordSet[7]);
				HTMLString.Append("\"><input type=\"hidden\" name=\"sHOdds\" value=\"");
				HTMLString.Append(arrRecordSet[8]);
				HTMLString.Append("\"><input type=\"hidden\" name=\"ScoreTarget\" value=\"");
				HTMLString.Append(arrRecordSet[9]);
				HTMLString.Append("\"><input type=\"hidden\" name=\"ST_Odds\" value=\"");
				HTMLString.Append(arrRecordSet[10]);
				HTMLString.Append("\"><input type=\"hidden\" name=\"WH_Odds\" value=\"");
				HTMLString.Append(arrRecordSet[11]);
				HTMLString.Append("\"><input type=\"hidden\" name=\"WG_Odds\" value=\"");
				HTMLString.Append(arrRecordSet[12]);
				HTMLString.Append("\"><input type=\"hidden\" name=\"Alert_Type\" value=\"");
				HTMLString.Append(iAlertType);
				HTMLString.Append("\">");
			} catch(Exception ex) {
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " BSKModifyMatch.cs.GetMatches(): " + ex.ToString());
				m_SportsLog.Close();
				HTMLString.Remove(0,HTMLString.Length);
				HTMLString.Append((string)HttpContext.Current.Application["accessErrorMsg"]);
			}

			return HTMLString.ToString();
		}

		public int Modify() {
			int iRecUpd = 0;
			//int iExisted = 0;
			string sAction;
			string sMatchCount;
			string sLeague;
			string sOrg_Host;
			string sOrg_Guest;
			string sOrg_MatchDate;
			string sOrg_MatchTime;
			string sCurrentTimestamp = null;
			string sBatchJob = null;
			string[] arrMsgType;
			string[] arrRemotingPath;
			string[] arrSendToPager;
			char[] delimiter = new char[] {','};

			arrMsgType = (string[])HttpContext.Current.Application["messageType"];
			sAction = HttpContext.Current.Request.Form["action"];
			sMatchCount = HttpContext.Current.Request.Form["MatchCount"];
			sLeague = HttpContext.Current.Request.Form["League"];
			sOrg_Host = HttpContext.Current.Request.Form["Org_Host"];
			sOrg_Guest = HttpContext.Current.Request.Form["Org_Guest"];
			sOrg_MatchDate = HttpContext.Current.Request.Form["Org_MatchDate"];
			sOrg_MatchTime = HttpContext.Current.Request.Form["Org_MatchTime"];
			arrRemotingPath = (string[])HttpContext.Current.Application["RemotingItems"];
			
			try {
				arrSendToPager = HttpContext.Current.Request.Form["SendToPager"].Split(delimiter);
			} catch(Exception) {
				arrSendToPager = new string[0];
			}
			try {
				if(sAction.Equals("D")) {	//delete match w.r.t match count
					string sMatchField;
					string sHostHandi;
					string sScrHandi;
					string sHOdds;
					string sScrTarget;
					string sSTOdds;
					string sWHOdds;
					string sWGOdds;
					string sAlertType;

					sMatchField = HttpContext.Current.Request.Form["MatchField"];
					sHostHandi = HttpContext.Current.Request.Form["HostHandi"];
					sScrHandi = HttpContext.Current.Request.Form["ScoreHandi"];
					sHOdds = HttpContext.Current.Request.Form["sHOdds"];
					sScrTarget = HttpContext.Current.Request.Form["ScoreTarget"];
					sSTOdds = HttpContext.Current.Request.Form["ST_Odds"];
					sWHOdds = HttpContext.Current.Request.Form["WH_Odds"];
					sWGOdds = HttpContext.Current.Request.Form["WG_Odds"];
					sAlertType = HttpContext.Current.Request.Form["Alert_Type"];
					//SportsMessage object message
					SportsMessage sptMsg = new SportsMessage();
					StringBuilder LogSQLString = new StringBuilder();
					DBManager logDBMgr = new DBManager();
					logDBMgr.ConnectionString = m_SportsDBMgr.ConnectionString;

					/*****************************
					 * GoGo Pager2 alert message *
					 *****************************/						
					string[] arrQueueNames;
					string[] arrMessageTypes;
					arrMsgType = (string[])HttpContext.Current.Application["messageType"];

					sCurrentTimestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
					sBatchJob = DateTime.Now.ToString("yyMMddHHmmss_fffffff") + "." + HttpContext.Current.Session["user_name"].ToString() + "." + arrMsgType[29] + ".del.ini";
					arrQueueNames = (string[])HttpContext.Current.Application["QueueItems"];
					arrMessageTypes = (string[])HttpContext.Current.Application["NotifyMessageTypes"];
					MessageClient msgClt = new MessageClient();
					msgClt.MessageType = arrMessageTypes[0];
					msgClt.MessagePath = arrQueueNames[0];
					//generate INI for deleting match

					//insert into LOG_BSK_ALLODDS					
					LogSQLString.Remove(0,LogSQLString.Length);
					LogSQLString.Append("insert into LOG_BSK_ALLODDS (TIMEFLAG,Section,Act,League,Host,Guest,MatchDate,MatchTime,MatchField,Handicap,ScoreHandicap,SH_odds,ScoreTarget,ST_Odds,WH_Odds,WG_Odds,Alert_Type,BATCHJOB) values('");
					LogSQLString.Append(sCurrentTimestamp);
					LogSQLString.Append("','MATCH_','D','");
					LogSQLString.Append(sLeague);
					LogSQLString.Append("','");
					LogSQLString.Append(sOrg_Host);
					LogSQLString.Append("','");
					LogSQLString.Append(sOrg_Guest);
					LogSQLString.Append("','");
					LogSQLString.Append(sOrg_MatchDate);
					LogSQLString.Append("','");
					LogSQLString.Append(sOrg_MatchTime);
					LogSQLString.Append("','");
					LogSQLString.Append(sMatchField);
					LogSQLString.Append("','");
					LogSQLString.Append(sHostHandi);
					LogSQLString.Append("','");
					if(sScrHandi.Equals("")) sScrHandi = "-1";
					LogSQLString.Append(sScrHandi);
					LogSQLString.Append("','");
					if(sHOdds.Equals("")) sHOdds = "-1";
					LogSQLString.Append(sHOdds);
					LogSQLString.Append("','");
					if(sScrTarget.Equals("")) sScrTarget = "-1";
					LogSQLString.Append(sScrTarget);
					LogSQLString.Append("','");
					if(sSTOdds.Equals("")) sSTOdds = "-1";	
					LogSQLString.Append(sSTOdds);
					LogSQLString.Append("','");	
					if(sWHOdds.Equals("")) sWHOdds = "-1";	
					LogSQLString.Append(sWHOdds);
					LogSQLString.Append("','");	
					if(sWGOdds.Equals("")) sWGOdds = "-1";	
					LogSQLString.Append(sWGOdds);
					LogSQLString.Append("','");
					LogSQLString.Append(sAlertType);
					LogSQLString.Append("','");
					LogSQLString.Append(sBatchJob);
					LogSQLString.Append("')");
					logDBMgr.ExecuteNonQuery(LogSQLString.ToString());
					logDBMgr.Close();		
					//end insert into LOG_BSK_ALLODDS

					//update related tables
					SQLString.Remove(0,SQLString.Length);
					SQLString.Append("update NBAGAME_INFO  set CACTION='D' where IMATCH_COUNT=");
					SQLString.Append(sMatchCount);
					m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
					m_SportsDBMgr.Close();
					iRecUpd++;

					//send message to the msmq
					//Modified by Henry, 11 Feb 2004
					sptMsg.Body = sBatchJob;
					sptMsg.Timestamp = DateTime.Now.ToString("yyMMddHHmmss");
					sptMsg.AppID = "07";
					sptMsg.MsgID = "27";
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
							m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " MSMQ ERROR: BSK Modify Match");
							m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " BSKModifyMatch.cs.Modify(): Notify via MSMQ throws MessageQueueException:  " + mqEx.ToString());
							m_SportsLog.Close();

							//If MSMQ fail, notify via .NET Remoting
							msgClt.MessageType = arrMessageTypes[1];
							msgClt.MessagePath = arrRemotingPath[0];
							if(!msgClt.SendMessage((object)sptMsg)) {
								m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
								m_SportsLog.SetFileName(0,LOGFILESUFFIX);
								m_SportsLog.Open();
								m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Remoting ERROR: BSK Modify Match");
								m_SportsLog.Close();
							}
						}	catch(Exception ex) {
							m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
							m_SportsLog.SetFileName(0,LOGFILESUFFIX);
							m_SportsLog.Open();
							m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Remoting ERROR: BSK Modify Match");
							m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " BSKModifyMatch.cs.Modify(): Notify via .NET Remoting throws exception: " + ex.ToString());
							m_SportsLog.Close();
						}							
					}	catch(Exception ex) {
						m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
						m_SportsLog.SetFileName(0,LOGFILESUFFIX);
						m_SportsLog.Open();
						m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " BSKModifyMatch.cs.Modify(): Notify via MSMQ throws exception: " + ex.ToString());
						m_SportsLog.Close();
					}							
					//Modified end
					
					//write log
					m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
					m_SportsLog.SetFileName(0,LOGFILESUFFIX);
					m_SportsLog.Open();
					m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " BSKModifyMatch.cs: Mark delete <L:" + sLeague + ", H:" + sOrg_Host + ", G:" + sOrg_Guest + "> (" + HttpContext.Current.Session["user_name"] + ")");
					m_SportsLog.Close();
					m_SportsDBMgr.Dispose();
				}	else {	//update match w.r.t match count
					string sMatchDate;
					string sMatchTime;
					string sMatchYear;
					string sMatchMonth;
					string sMatchDay;
					string sMatchHour;
					string sMatchMinute;
					string sHost;
					string sGuest;

					sMatchYear = HttpContext.Current.Request.Form["MatchYear"];
					sMatchMonth = HttpContext.Current.Request.Form["MatchMonth"];
					sMatchDay = HttpContext.Current.Request.Form["MatchDate"];
					sMatchHour = HttpContext.Current.Request.Form["MatchHour"];
					sMatchMinute = HttpContext.Current.Request.Form["MatchMinute"];
					sHost = HttpContext.Current.Request.Form["Host"];
					sGuest = HttpContext.Current.Request.Form["Guest"];

					//construct new match date and time
					sMatchDate = sMatchYear + sMatchMonth + sMatchDay;
					sMatchTime = sMatchHour + sMatchMinute;
/*
					SQLString.Remove(0,SQLString.Length);
					SQLString.Append("select count(IMATCH_COUNT) from NBAGAME_INFO where CLEAG='");
					SQLString.Append(sLeague);
					SQLString.Append("' and CHOST='");
					SQLString.Append(sHost);
					SQLString.Append("' and CGUEST='");
					SQLString.Append(sGuest);
					SQLString.Append("'");
					iExisted = m_SportsDBMgr.ExecuteScalar(SQLString.ToString());
					m_SportsDBMgr.Close();
*/
					if(sMatchDate.Equals(sOrg_MatchDate) && sMatchTime.Equals(sOrg_MatchTime) && sHost.Equals(sOrg_Host) && sGuest.Equals(sOrg_Guest)) {	//Nothing to be changed
						iRecUpd = 0;
						m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
						m_SportsLog.SetFileName(0,LOGFILESUFFIX);
						m_SportsLog.Open();
						m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " ModifyMatch.cs: Nothing to modify <L:" + sLeague + ", H:" + sOrg_Host + ", G:" + sOrg_Guest + "> (" + HttpContext.Current.Session["user_name"] + ")");
						m_SportsLog.Close();
					} else {	//something should be changed
/*						
					if(iExisted == 0) {
						if(sMatchDate.Equals(sOrg_MatchDate) && sMatchTime.Equals(sOrg_MatchTime) && sHost.Equals(sOrg_Host) && sGuest.Equals(sOrg_Guest)) {	//Nothing to be changed
							iRecUpd = 0;

							//write log
							m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
							m_SportsLog.SetFileName(0,LOGFILESUFFIX);
							m_SportsLog.Open();
							m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " ModifyMatch.cs: Nothing to modify <L:" + sLeague + ", H:" + sOrg_Host + ", G:" + sOrg_Guest + "> (" + HttpContext.Current.Session["user_name"] + ")");
							m_SportsLog.Close();
						} else { // modify the match
*/
						iRecUpd++;
						//SportsMessage object message
						SportsMessage sptMsg = new SportsMessage();
						StringBuilder LogSQLString = new StringBuilder();
						DBManager logDBMgr = new DBManager();
						logDBMgr.ConnectionString = m_SportsDBMgr.ConnectionString;

						/*****************************
						 * GoGo Pager2 alert message *
				 		*****************************/						
						string[] arrQueueNames;
						string[] arrMessageTypes;
						arrMsgType = (string[])HttpContext.Current.Application["messageType"];

						sCurrentTimestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
						sBatchJob = DateTime.Now.ToString("yyMMddHHmmss_fffffff") + "." + HttpContext.Current.Session["user_name"].ToString() + "." + arrMsgType[33] + ".ini";
						arrQueueNames = (string[])HttpContext.Current.Application["QueueItems"];
						arrMessageTypes = (string[])HttpContext.Current.Application["NotifyMessageTypes"];
						MessageClient msgClt = new MessageClient();
						msgClt.MessageType = arrMessageTypes[0];
						msgClt.MessagePath = arrQueueNames[0];

						//insert into LOG_BSK_MODMATCH
						LogSQLString.Remove(0,LogSQLString.Length);
						LogSQLString.Append("insert into LOG_BSK_MODMATCH (TIMEFLAG,Section,CMD,PARAM1,PARAM2,PARAM3,PARAM4,PARAM5,PARAM6,PARAM7,PARAM8,PARAM9,BATCHJOB) values('");
						LogSQLString.Append(sCurrentTimestamp);
						LogSQLString.Append("','COMMAND_','RN_ODDS_KEY','");
						LogSQLString.Append(sLeague);
						LogSQLString.Append("','");
						LogSQLString.Append(sOrg_Host);
						LogSQLString.Append("','");
						LogSQLString.Append(sHost);
						LogSQLString.Append("','");
						LogSQLString.Append(sOrg_Guest);
						LogSQLString.Append("','");
						LogSQLString.Append(sGuest);
						LogSQLString.Append("','");
						LogSQLString.Append(sOrg_MatchDate);
						LogSQLString.Append("','");
						LogSQLString.Append(sMatchDate);
						LogSQLString.Append("','");
						LogSQLString.Append(sOrg_MatchTime);
						LogSQLString.Append("','");
						LogSQLString.Append(sMatchTime);
						LogSQLString.Append("','");
						LogSQLString.Append(sBatchJob);
						LogSQLString.Append("')");
						logDBMgr.ExecuteNonQuery(LogSQLString.ToString());
						logDBMgr.Close();	
						//end insert into LOG_BSK_MODMATCH

						//update related tables								
						SQLString.Remove(0,SQLString.Length);			
						SQLString.Append("update NBAGAME_INFO  set CHOST='");
						SQLString.Append(sHost);
						SQLString.Append("',CGUEST='");
						SQLString.Append(sGuest);
						SQLString.Append("',CMATCH_DATE='");
						SQLString.Append(sMatchDate);
						SQLString.Append("',CMATCH_TIME='");
						SQLString.Append(sMatchTime);
						SQLString.Append("'  where IMATCH_COUNT=");
						SQLString.Append(sMatchCount);
						m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
						m_SportsDBMgr.Close();

						//Modified by Henry, 11 Feb 2004
						sptMsg.Body = sBatchJob;
						sptMsg.Timestamp = DateTime.Now.ToString("yyMMddHHmmss");
						sptMsg.AppID = "07";
						sptMsg.MsgID = "30"; //30為定義修改賽事
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
								m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " MSMQ ERROR: BSK Modify Match");
								m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " BSKModifyMatch.cs.Modify(): Notify via MSMQ throws MessageQueueException:  " + mqEx.ToString());
								m_SportsLog.Close();
		
								//If MSMQ fail, notify via .NET Remoting
								msgClt.MessageType = arrMessageTypes[1];
								msgClt.MessagePath = arrRemotingPath[0];
								if(!msgClt.SendMessage((object)sptMsg)) {
									m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
									m_SportsLog.SetFileName(0,LOGFILESUFFIX);
									m_SportsLog.Open();
									m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Remoting ERROR: BSK Modify Match");
									m_SportsLog.Close();
								}
							}	catch(Exception ex) {
								m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
								m_SportsLog.SetFileName(0,LOGFILESUFFIX);
								m_SportsLog.Open();
								m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Remoting ERROR: BSK Modify Match");
								m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " BSKModifyMatch.cs.Modify(): Notify via .NET Remoting throws exception: " + ex.ToString());
								m_SportsLog.Close();
							}							
						}	catch(Exception ex) {
							m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
							m_SportsLog.SetFileName(0,LOGFILESUFFIX);
							m_SportsLog.Open();
							m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " BSKModifyMatch.cs.Modify(): Notify via MSMQ throws exception: " + ex.ToString());
							m_SportsLog.Close();
						}									
						//Modified end
							
						//write log
						m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
						m_SportsLog.SetFileName(0,LOGFILESUFFIX);
						m_SportsLog.Open();
						m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " BSKModifyMatch.cs: Modify match <L:" + sLeague + ", H:" + sOrg_Host + ", G:" + sOrg_Guest + "> (" + HttpContext.Current.Session["user_name"] + ")");
						m_SportsLog.Close();
						m_SportsDBMgr.Dispose();
					}
				}/* else {
					iRecUpd = -2;
					//write log
					m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
					m_SportsLog.SetFileName(0,LOGFILESUFFIX);
					m_SportsLog.Open();
					m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " BSKModifyMatch.cs: Modify failed, New match <L:" + sLeague + ", H:" + sHost + ", G:" + sGuest + "> existed. (" + HttpContext.Current.Session["user_name"] + ")");
					m_SportsLog.Close();
				}
			}*/
			} catch(Exception ex) {
				iRecUpd = -1;
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " BSKModifyMatch.cs.Modify(): " + ex.ToString());
				m_SportsLog.Close();
			}

			return iRecUpd;			
		}
	}
}