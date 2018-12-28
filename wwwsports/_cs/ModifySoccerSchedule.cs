/*
Objective:
Modify soccer schedule

Last updated:
5 Apr 2005 by Paddy, try catch the case of get Null String of hostname/guestname
23 Mar 2004, Chapman Choi
23 Mar 2004 Remark Replicator code

C#.NET complier statement:
csc /t:library /out:..\bin\ModifySoccerSchedule.dll /r:..\bin\DBManager.dll;..\bin\Files.dll;..\bin\Replicator.dll ModifySoccerSchedule.cs
csc /t:library /out:..\bin\ModifySoccerSchedule.dll /r:..\bin\DBManager.dll;..\bin\Files.dll ModifySoccerSchedule.cs
*/

using System;
using System.Collections;
using System.Data.OleDb;
using System.Reflection;
using System.Web;
using TDL.DB;
using TDL.IO;
//using TDL.Util;

[assembly:AssemblyCompany("TDSL")]
[assembly:AssemblyCopyright("(c) 2003 TDSL. All rights reserved.")]
[assembly:AssemblyDescription("足球賽程 -> 修改賽程")]
[assembly:AssemblyProduct("Sports.NET")]
[assembly:AssemblyTitle("Sports.NET DLL")]
[assembly:AssemblyVersion("1.1.*")]
namespace SportsUtil {
	public class ModifySoccerSchedule {
		const string LOGFILESUFFIX = "log";
		OleDbDataReader m_SportsOleReader;
		DBManager m_SportsDBMgr;
		Files m_SportsLog;

		public ModifySoccerSchedule(string Connection) {
			m_SportsDBMgr = new DBManager();
			m_SportsDBMgr.ConnectionString = Connection;
			m_SportsLog = new Files();
		}

		public string GetMatches() {
			int iCurrYear, iYear=0, iMonth=0, iDay=0, iHour=0, iMinute=0, iNoOfOpt = 0, iNewValue = 0;
			const int iYY=2, iMM=12, iDD=31, iHH=24, imm=60;
			string sQuery, sMatchCount, sTempRecord = "", sLeagID = "", sMatchDate = "", sMatchTime = "", sAlias = "", sHost = "", sGuest = "", sHandicap = "", sMatchField= "", sRtn = "", sStatus = "", sHostTeamId = "", sGuestTeamId = "";
			const string sZeroPad = "0";
			ArrayList teamAL = new ArrayList();
			ArrayList teamIdAL = new ArrayList();
			teamAL.Capacity = 20;
			teamIdAL.Capacity = 20;

			iCurrYear = Convert.ToInt32(DateTime.Now.ToString("yyyy"));
			sMatchCount = HttpContext.Current.Request.QueryString["MatchCount"];
			try {
				sQuery = "select leag.LEAG_ID from LEAGINFO leag, SOCCERSCHEDULE sch where sch.cleag_id = leag.leag_id and sch.IMATCH_CNT=" + sMatchCount;
				m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(sQuery);
				if(m_SportsOleReader.Read()) sLeagID = m_SportsOleReader.GetString(0).Trim();
				m_SportsOleReader.Close();
				m_SportsDBMgr.Close();

				sQuery = "select teaminfo.teamname , teaminfo.team_id from leaginfo, teaminfo, id_info where leaginfo.leag_id=id_info.leag_id and teaminfo.team_id=id_info.team_id ";
				sQuery += "and id_info.leag_id='" + sLeagID + "' order by teaminfo.teamname";
				m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(sQuery);
				while(m_SportsOleReader.Read()) {
					sTempRecord = m_SportsOleReader.GetString(0).Trim();
					teamAL.Add(sTempRecord);
					teamIdAL.Add(m_SportsOleReader.GetInt32(1));
				}
				m_SportsOleReader.Close();
				m_SportsDBMgr.Close();
				teamAL.TrimToSize();
				teamIdAL.TrimToSize();

				sQuery = "select MATCHDATETIME, (select alias from leaginfo where leag_id=sch.cleag_id) calias, ";
				sQuery += "(select teamname from teaminfo where team_id=sch.ihost_team_id) CHOST,";
				sQuery += "(select teamname from teaminfo where team_id=sch.iguest_team_id) CGUEST, CHOST_HANDI, CFIELD, CSTATUS, ihost_team_id, iguest_team_id from SOCCERSCHEDULE sch where IMATCH_CNT=" + sMatchCount;
				m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(sQuery);
				if(m_SportsOleReader.Read()) {
					sMatchDate = m_SportsOleReader.GetDateTime(0).ToString("yyyyMMdd");
					sMatchTime = m_SportsOleReader.GetDateTime(0).ToString("HHmm");
					iYear = Convert.ToInt32(m_SportsOleReader.GetDateTime(0).ToString("yyyy"));
					iMonth = Convert.ToInt32(m_SportsOleReader.GetDateTime(0).ToString("MM"));
					iDay = Convert.ToInt32(m_SportsOleReader.GetDateTime(0).ToString("dd"));
					iHour = Convert.ToInt32(m_SportsOleReader.GetDateTime(0).ToString("HH"));
					iMinute = Convert.ToInt32(m_SportsOleReader.GetDateTime(0).ToString("mm"));
					sAlias = m_SportsOleReader.GetString(1).Trim();
					try{
						sHost = m_SportsOleReader.GetString(2).Trim();
					}catch{
						sHost = "NULL";
					}
					try{
						sGuest = m_SportsOleReader.GetString(3).Trim();
					}catch{
						sGuest = "NULL";
					}
					sHandicap = m_SportsOleReader.GetString(4);
					sMatchField = m_SportsOleReader.GetString(5);
					sStatus = m_SportsOleReader.GetString(6);
					sHostTeamId = Convert.ToString(m_SportsOleReader.GetInt32(7));
					sGuestTeamId = Convert.ToString(m_SportsOleReader.GetInt32(8));
				}
				m_SportsOleReader.Close();
				m_SportsDBMgr.Close();
				//m_SportsDBMgr.Dispose();

				//Original Match Date and its hidden field
				sRtn += "<td><select name=\"MatchYear\"><option value=\"" + iYear.ToString() + "\">" + iYear.ToString();
				iNewValue = iYear+1;
				while(iNewValue != iYear) {
					if(iNewValue>(iCurrYear+iYY)) iNewValue=iCurrYear-1;
					if(iNewValue != iYear) {
						sRtn += "<option value=\"" + iNewValue.ToString() + "\">" + iNewValue.ToString();
						iNewValue++;
					}
				}
				sRtn += "</select>";

				sRtn += "/<select name=\"MatchMonth\"><option value=\"";
				if(iMonth<10) sRtn += sZeroPad;
				sRtn += iMonth.ToString() + "\">" + iMonth.ToString();
				iNoOfOpt = iMonth+1;
				while(iNoOfOpt != iMonth) {
					if(iNoOfOpt>iMM) iNoOfOpt=1;
					if(!(iMonth == 1 && iNoOfOpt == 1)) {
						sRtn += "<option value=\"";
						if(iNoOfOpt<10) sRtn += sZeroPad;
						sRtn += iNoOfOpt.ToString() + "\">" + iNoOfOpt.ToString();
						iNoOfOpt++;
					}
				}
				sRtn += "</select>";

				sRtn += "/<select name=\"MatchDate\"><option value=\"";
				if(iDay<10) sRtn += sZeroPad;
				sRtn += iDay.ToString() + "\">" + iDay.ToString();
				iNoOfOpt = iDay+1;
				while(iNoOfOpt != iDay) {
					if(iNoOfOpt>iDD) iNoOfOpt=1;
					if(!(iDay == 1 && iNoOfOpt == 1)) {
						sRtn += "<option value=\"";
						if(iNoOfOpt<10) sRtn += sZeroPad;
						sRtn += iNoOfOpt.ToString() + "\">" + iNoOfOpt.ToString();
						iNoOfOpt++;
					}
				}
				sRtn += "</select></td>";

				//Original Match Time and its hidden field
				sRtn += "<td><select name=\"MatchHour\"><option value=\"";
				if(iHour<10) sRtn += sZeroPad;
				sRtn += iHour.ToString() + "\">" + iHour.ToString();
				iNoOfOpt = iHour+1;
				while(iNoOfOpt != iHour) {
					if(iNoOfOpt>=iHH) iNoOfOpt=0;
					if(!(iHour == 0 && iNoOfOpt == 0)) {
						sRtn += "<option value=\"";
						if(iNoOfOpt<10) sRtn += sZeroPad;
						sRtn += iNoOfOpt.ToString() + "\">" + iNoOfOpt.ToString();
						iNoOfOpt++;
					}
				}
				sRtn += "</select>:";

				sRtn += "<select name=\"MatchMinute\"><option value=\"";
				if(iMinute<10) sRtn += sZeroPad;
				sRtn += iMinute.ToString() + "\">" + iMinute.ToString();
				iNoOfOpt = iMinute+1;
				while(iNoOfOpt != iMinute) {
					if(iNoOfOpt>=imm) iNoOfOpt=0;
					if(!(iMinute == 0 && iNoOfOpt == 0)) {
						sRtn += "<option value=\"";
						if(iNoOfOpt<10) sRtn += sZeroPad;
						sRtn += iNoOfOpt.ToString() + "\">" + iNoOfOpt.ToString();
						iNoOfOpt++;
					}
				}
				sRtn += "</select></td>";

				//Alias
				sRtn += "<td>" + sAlias + "<input type=\"hidden\" name=\"LeagId\" value=\"" + sLeagID + "\"></td>";

				//Host
				sRtn += "<td><select name=\"Host\"><option value=\"" + sHostTeamId + "\">" + sHost;
				for(iNoOfOpt=0;iNoOfOpt<teamAL.Count;iNoOfOpt++) {
					if(!teamAL[iNoOfOpt].Equals(sHost)) {
						sRtn += "<option value=\"" + teamIdAL[iNoOfOpt] + "\">" + teamAL[iNoOfOpt];
					}
				}
				sRtn += "</select></td>";

				//Guest
				sRtn += "<td><select name=\"Guest\"><option value=\"" + sGuestTeamId + "\">" + sGuest;
				for(iNoOfOpt=0;iNoOfOpt<teamAL.Count;iNoOfOpt++) {
					if(!teamAL[iNoOfOpt].Equals(sGuest)) {
						sRtn += "<option value=\"" + teamIdAL[iNoOfOpt] + "\">" + teamAL[iNoOfOpt];
					}
				}
				sRtn += "</select></td>";

				//Host Handicap, MatchField
				sRtn += "<td><input type=\"checkbox\" name=\"Handicap\" value=\"1\" ";
				if(sHandicap.Equals("1")) sRtn += "checked";
				sRtn += "></td>";
				sRtn += "<td><input type=\"checkbox\" name=\"MatchField\" value=\"1\" ";
				if(sMatchField.Equals("M")) sRtn += "checked";
				sRtn += "></td>";
				sRtn += "<td><input type=\"checkbox\" name=\"Status\" value=\"1\" ";
				if(sStatus.Equals("C")) sRtn += "checked";
				sRtn += "></td>";

				//MatchCount (Hidden Field)
				sRtn += "<input type=\"hidden\" name=\"MatchCount\" value=\"" + sMatchCount + "\">";
			} catch(Exception ex) {
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " ModifySoccerSchedule.cs.GetMatches(): " + ex.ToString());
				m_SportsLog.Close();
				sRtn = (string)HttpContext.Current.Application["accessErrorMsg"];
			}

			return sRtn;
		}

		public int Modify() {
			int iExisted = 0;
			string updateQuery, sMatchCount, sLeagId, sHandicap, sMatchField;
			string sMatchDate, sMatchTime, sMatchYear, sMatchMonth, sMatchDay, sMatchHour, sMatchMinute, sHostTeamId, sGuestTeamId, sStatus;

			sMatchCount = HttpContext.Current.Request.Form["MatchCount"];
			sLeagId = HttpContext.Current.Request.Form["LeagId"];
			sMatchYear = HttpContext.Current.Request.Form["MatchYear"];
			sMatchMonth = HttpContext.Current.Request.Form["MatchMonth"];
			sMatchDay = HttpContext.Current.Request.Form["MatchDate"];
			sMatchDate = sMatchYear + "-" + sMatchMonth + "-" + sMatchDay;
			sMatchHour = HttpContext.Current.Request.Form["MatchHour"];
			sMatchMinute = HttpContext.Current.Request.Form["MatchMinute"];
			sMatchTime = sMatchHour + ":" + sMatchMinute + ":00";
			sHostTeamId = HttpContext.Current.Request.Form["Host"];
			sGuestTeamId = HttpContext.Current.Request.Form["Guest"];
			sHandicap = HttpContext.Current.Request.Form["Handicap"];
			sStatus = HttpContext.Current.Request.Form["Status"];
			if(sHandicap == null) sHandicap = "0";
			if(sHandicap.Equals("")) sHandicap = "0";
			sMatchField = HttpContext.Current.Request.Form["MatchField"];
			if(sMatchField == null) sMatchField = "H";
			if(sMatchField.Equals("1")) sMatchField = "M";
			else sMatchField = "H";
			if(sStatus == null) sStatus = "N";
			if(sStatus.Equals("1")) sStatus = "C";
			else sStatus = "N";

			try {
				//Replicator SptReplicator = new Replicator(HttpContext.Current.Application["RepDBConnectionString"].ToString());
				//SptReplicator.ApplicationType = 1;
				//SptReplicator.ErrorLogPath = HttpContext.Current.Application["ErrorFilePath"].ToString();

				updateQuery = "update SOCCERSCHEDULE set ihost_team_id=" + sHostTeamId + ", iguest_team_id=" + sGuestTeamId + ", MATCHDATETIME='" + sMatchDate + " " + sMatchTime + "', CFIELD='" + sMatchField + "', CHOST_HANDI='" + sHandicap + "', CSTATUS='" + sStatus + "' where IMATCH_CNT=" + sMatchCount;
				m_SportsDBMgr.ExecuteNonQuery(updateQuery);
				m_SportsDBMgr.Close();
				//SptReplicator.Replicate(updateQuery);

				//write log
				m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " ModifySoccerSchedule.cs: Modify schedule with MatchCount=" + sMatchCount + " (" + HttpContext.Current.Session["user_name"] + ")");
				m_SportsLog.Close();
				//m_SportsDBMgr.Dispose();
				//SptReplicator.Dispose();
				iExisted++;
			} catch(OleDbException dbEx) {
				iExisted = -99;
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " ModifySoccerSchedule.cs.Modify(): " + dbEx.ToString());
				m_SportsLog.Close();
			}	catch(Exception ex) {
				iExisted = -1;
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " ModifySoccerSchedule.cs.Modify(): " + ex.ToString());
				m_SportsLog.Close();
			}

			return iExisted;
		}
	}
}
