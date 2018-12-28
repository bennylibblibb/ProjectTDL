/*
Objective:
Add a new match to Asia2 Sports

Last updated:
7 Oct 2004, Fanny Cheung
23 Mar 2004 Remark Replicator code

C#.NET complier statement:

(With Replication)
csc /t:library /out:..\bin\OtherSoccerNewMatch.dll /r:..\bin\DBManager.dll;..\bin\Win32INI.dll;..\bin\Files.dll;..\bin\Replicator.dll OtherSoccerNewMatch.cs

(Without Replication - Current Production Version)
csc /t:library /out:..\bin\OtherSoccerNewMatch.dll /r:..\bin\DBManager.dll;..\bin\Win32INI.dll;..\bin\Files.dll OtherSoccerNewMatch.cs
*/

using System;
using System.Collections;
using System.Collections.Specialized;
using System.Data.OleDb;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Web;
using TDL.DB;
using TDL.IO;
//using TDL.Util;

[assembly:AssemblyCompany("TDSL")]
[assembly:AssemblyCopyright("(c) 2003 TDSL. All rights reserved.")]
[assembly:AssemblyDescription("足球資訊 -> 新增賽事")]
[assembly:AssemblyProduct("Sports.NET")]
[assembly:AssemblyTitle("Sports.NET DLL")]
[assembly:AssemblyVersion("1.1.*")]
namespace SportsUtil {
	public class OtherSoccerNewMatch {
		const int LOGON32_LOGON_INTERACTIVE = 2;
		const int LOGON32_PROVIDER_DEFAULT = 0;
		const int SecurityImpersonation = 2;
		const string LOGFILESUFFIX = "log";
		const int RECORDSOINPAGE = 10;
		OleDbDataReader m_SportsOleReader;
		DBManager m_SportsDBMgr;
		Files m_SportsLog;
		StringBuilder SQLString;

		[DllImport("advapi32.dll", SetLastError=true)]
		public static extern bool LogonUser(string lpszUserName, string lpszDomain, string lpszPassword, int dwLogonType, int dwLogonProvider, ref IntPtr phToken);

		[DllImport("advapi32.dll", SetLastError=true)]
		public extern static int DuplicateToken(IntPtr hToken, int impersonationLevel, ref IntPtr hNewToken);

		public OtherSoccerNewMatch(string Connection) {
			m_SportsDBMgr = new DBManager();
			m_SportsDBMgr.ConnectionString = Connection;
			m_SportsLog = new Files();
			SQLString = new StringBuilder();
		}

		public string InitFields() {
			const int DEFAULTYEAR=1;
			const int DEFAULTMONTH=12;
			const int DEFAULTDAY=31;
			const int DEFAULTHOUR=24;
			const int DEFAULTMINUTE=60;
			const string EMPTYTEAMMESSAGE = "沒有隊伍在此聯賽!";
			const string ZEROPAD = "0";
			int iYear, iMonth, iDay, iNewValue = 0, iNoOfOpt = 0, iIndex = 0;
			string sLeagID, sLeag = "", sAlias = "", sRecordStr = "";
			ArrayList teamAL = new ArrayList();
			teamAL.Capacity = 20;
			StringBuilder HTMLString = new StringBuilder();

			iYear = DateTime.Today.Year;
			iMonth = DateTime.Today.Month;
			iDay = DateTime.Today.Day;
			sLeagID = HttpContext.Current.Request.QueryString["leagID"].Trim();
			try {
				SQLString.Remove(0,SQLString.Length);
				SQLString.Append("select teaminfo.teamname, leaginfo.leagname, leaginfo.alias from leaginfo, teaminfo, id_info where leaginfo.leag_id=id_info.leag_id and teaminfo.team_id=id_info.team_id and id_info.leag_id='");
				SQLString.Append(sLeagID);
				SQLString.Append("' order by teaminfo.teamname");
				m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
				while(m_SportsOleReader.Read()) {
					if(m_SportsOleReader.IsDBNull(0)) {
						HTMLString.Append("<tr style=\"background-color:#FF59AC\"><th colspan=12>");
						HTMLString.Append(EMPTYTEAMMESSAGE);
						HTMLString.Append("</th></tr>");
						break;
					}	else {
						//get team into ArrayList
						sRecordStr = m_SportsOleReader.GetString(0).Trim();
						teamAL.Add(sRecordStr);

						//get League Name
						if(iIndex == 0) {
							sLeag = m_SportsOleReader.GetString(1).Trim();
							sAlias = m_SportsOleReader.GetString(2).Trim();
						}
						iIndex++;
					}
				}
				m_SportsOleReader.Close();
				m_SportsDBMgr.Close();
				teamAL.TrimToSize();

				//Display League Name
				HTMLString.Append("<input type=\"hidden\" name=\"League\" value=\"");
				HTMLString.Append(sLeag);
				HTMLString.Append("\"><input type=\"hidden\" name=\"Alias\" value=\"");
				HTMLString.Append(sAlias);
				HTMLString.Append("\"><tr style=\"background-color:#FF59AC\"><th colspan=6 align=\"left\">新增<font color=#FFD700>");
				HTMLString.Append(sLeag);
				HTMLString.Append("</font>賽事</th></tr><tr><th>年/月/日</th><th>時:分</th><th>主隊</th><th>客隊</th><th>主讓</th><th>中立場</th></tr>");

				iIndex = 0;
				while(RECORDSOINPAGE > iIndex) {
					HTMLString.Append("<tr align=\"center\">");
					//MatchDate
					HTMLString.Append("<td><select name=\"MatchYear\"><option value=\"");
					HTMLString.Append(iYear.ToString());
					HTMLString.Append("\">");
					HTMLString.Append(iYear.ToString());
					for(iNoOfOpt=1;iNoOfOpt<=DEFAULTYEAR;iNoOfOpt++) {
						iNewValue = iYear+iNoOfOpt;
						HTMLString.Append("<option value=\"");
						HTMLString.Append(iNewValue.ToString());
						HTMLString.Append("\">");
						HTMLString.Append(iNewValue.ToString());
					}
					HTMLString.Append("</select>");

					HTMLString.Append("/<select name=\"MatchMonth\"><option value=\"");
					if(iMonth<10) HTMLString.Append(ZEROPAD);
					HTMLString.Append(iMonth.ToString());
					HTMLString.Append("\">");
					HTMLString.Append(iMonth.ToString());
					iNoOfOpt = iMonth+1;
					while(iNoOfOpt != iMonth) {
						if(iNoOfOpt>DEFAULTMONTH) iNoOfOpt=1;
						if(!(iMonth == 1 && iNoOfOpt == 1)) {
							HTMLString.Append("<option value=\"");
							if(iNoOfOpt<10) HTMLString.Append(ZEROPAD);
							HTMLString.Append(iNoOfOpt.ToString());
							HTMLString.Append("\">");
							HTMLString.Append(iNoOfOpt.ToString());
							iNoOfOpt++;
						}
					}
					HTMLString.Append("</select>");

					HTMLString.Append("/<select name=\"MatchDate\"><option value=\"");
					if(iDay<10) HTMLString.Append(ZEROPAD);
					HTMLString.Append(iDay.ToString());
					HTMLString.Append("\">");
					HTMLString.Append(iDay.ToString());
					iNoOfOpt = iDay+1;
					while(iNoOfOpt != iDay) {
						if(iNoOfOpt>DEFAULTDAY) iNoOfOpt=1;
						if(!(iDay == 1 && iNoOfOpt == 1)) {
							HTMLString.Append("<option value=\"");
							if(iNoOfOpt<10) HTMLString.Append(ZEROPAD);
							HTMLString.Append(iNoOfOpt.ToString());
							HTMLString.Append("\">");
							HTMLString.Append(iNoOfOpt.ToString());
							iNoOfOpt++;
						}
					}
					HTMLString.Append("</select></td>");

					//MacthTime
					HTMLString.Append("<td><select name=\"MatchHour\">");
					for(iNoOfOpt=DEFAULTHOUR;iNoOfOpt>0;iNoOfOpt--) {
						HTMLString.Append("<option value=\"");
						if((iNoOfOpt-1)<10) HTMLString.Append(ZEROPAD);
						HTMLString.Append((iNoOfOpt-1).ToString());
						HTMLString.Append("\">");
						HTMLString.Append((iNoOfOpt-1).ToString());
					}
					HTMLString.Append("</select>:<select name=\"MatchMinute\">");
					for(iNoOfOpt=0;iNoOfOpt<DEFAULTMINUTE;iNoOfOpt++) {
						HTMLString.Append("<option value=\"");
						if(iNoOfOpt<10) HTMLString.Append(ZEROPAD);
						HTMLString.Append(iNoOfOpt.ToString());
						HTMLString.Append("\">");
						HTMLString.Append(iNoOfOpt.ToString());
					}
					HTMLString.Append("</select></td>");

					//Host
					HTMLString.Append("<td><select name=\"Host\">");
					for(iNoOfOpt=0;iNoOfOpt<teamAL.Count;iNoOfOpt++) {
						HTMLString.Append("<option value=\"");
						HTMLString.Append(teamAL[iNoOfOpt]);
						HTMLString.Append("\">");
						HTMLString.Append(teamAL[iNoOfOpt]);
					}
					HTMLString.Append("</select></td>");

					//Guest
					HTMLString.Append("<td><select name=\"Guest\">");
					for(iNoOfOpt=0;iNoOfOpt<teamAL.Count;iNoOfOpt++) {
						HTMLString.Append("<option value=\"");
						HTMLString.Append(teamAL[iNoOfOpt]);
						HTMLString.Append("\">");
						HTMLString.Append(teamAL[iNoOfOpt]);
					}
					HTMLString.Append("</select></td>");

					//Host Handicap check box
					HTMLString.Append("<td><input type=\"checkbox\" name=\"HostHandicap\" value=\"");
					HTMLString.Append(iIndex.ToString());
					HTMLString.Append("\" checked></td>");
					//Match Field
					HTMLString.Append("<td><input type=\"checkbox\" name=\"MatchField\" value=\"");
					HTMLString.Append(iIndex.ToString());
					HTMLString.Append("\"></td></tr>");
					iIndex++;
				}
				//m_SportsDBMgr.Dispose();
			}	catch(Exception ex) {
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " OtherSoccerNewMatch.cs.InitFields(): " + ex.ToString());
				m_SportsLog.Close();
				HTMLString.Remove(0,HTMLString.Length);
				HTMLString.Append((string)HttpContext.Current.Application["accessErrorMsg"]);
			}
			return HTMLString.ToString();
		}

		public int Add() {
			const int MINTIMESLOT=65;
			const double XAXISCONSTANT = 64;
			const string ZEROFLAG = "0";
			const string DEFAULTFLAG = "1";
			const string DEFAULTWEBACT = "V";
			const string DEFAULTACT = "U";
			bool bUpdFlag = false;
			int iInterval = 0, iUpdIndex, iRecUpd = 0, iMax = 0, iArrayIndex = 0;
			double dDiff = 0;
			char[] delimiter = new char[] {','};
			string sDayToAdd = null, sRefStartDateTime = null, sRefEndDateTime = null, sFormatPattern, sMatchDate = "", sMatchTime = "";
			string sLeague, sAlias, sOrder = "";
			string[] arrHost, arrGuest, arrYear, arrMonth, arrDate, arrHour, arrMinute, arrField, arrHostHandicap;
			TimeSpan TimeDiff;
			DateTime dtRefStart, dtRefEnd, dtMatch;
			CultureInfo formatter;
			NameValueCollection songNVC;
			//Replicator MatchReplicator = new Replicator(HttpContext.Current.Application["RepDBConnectionString"].ToString());
			//MatchReplicator.ApplicationType = 1;
			//MatchReplicator.ErrorLogPath = HttpContext.Current.Application["ErrorFilePath"].ToString();

			dtRefStart = new DateTime(0);
			dtRefEnd = new DateTime(0);
			dtMatch = new DateTime(0);
			formatter = new CultureInfo("en-US");
			sFormatPattern = "yyyyMMddHHmm";

			bool m_LoggedOn = false;
			string m_NetUser = "ASPUSER";
			string m_Password = "aspspt";
			string m_Domain = System.Environment.MachineName;
			WindowsImpersonationContext m_ImpersonationContext;
			WindowsIdentity m_TempWindowsIdentity;
			IntPtr m_Token = IntPtr.Zero;
			IntPtr m_TokenDuplicate = IntPtr.Zero;

			//get value from page/memory
			songNVC = (NameValueCollection)HttpContext.Current.Application["songItems"];
			sLeague = HttpContext.Current.Request.Form["League"];
			sAlias = HttpContext.Current.Request.Form["Alias"];
			arrHost = HttpContext.Current.Request.Form["Host"].Split(delimiter);
			arrGuest = HttpContext.Current.Request.Form["Guest"].Split(delimiter);
			arrYear = HttpContext.Current.Request.Form["MatchYear"].Split(delimiter);
			arrMonth = HttpContext.Current.Request.Form["MatchMonth"].Split(delimiter);
			arrDate = HttpContext.Current.Request.Form["MatchDate"].Split(delimiter);
			arrHour = HttpContext.Current.Request.Form["MatchHour"].Split(delimiter);
			arrMinute = HttpContext.Current.Request.Form["MatchMinute"].Split(delimiter);
			try {	//get match field
				arrField = HttpContext.Current.Request.Form["MatchField"].Split(delimiter);
			}	catch(Exception) {
				arrField = new string[0];
			}
			try {	//get host handicap
				arrHostHandicap = HttpContext.Current.Request.Form["HostHandicap"].Split(delimiter);
			}	catch(Exception) {
				arrHostHandicap = new string[0];
			}

			try {
				m_LoggedOn = LogonUser(m_NetUser, m_Domain, m_Password, LOGON32_LOGON_INTERACTIVE, LOGON32_PROVIDER_DEFAULT, ref m_Token);
				if(m_LoggedOn) {
					if(DuplicateToken(m_Token, SecurityImpersonation, ref m_TokenDuplicate) != 0) {
						m_TempWindowsIdentity = new WindowsIdentity(m_TokenDuplicate);
						m_ImpersonationContext = m_TempWindowsIdentity.Impersonate();
						if(m_ImpersonationContext != null) {
							//get config value from INI file
							Win32INI NewMacth_INI = new Win32INI(HttpContext.Current.Application["SoccerINIFilePath"].ToString());
							sRefEndDateTime = NewMacth_INI.GetValue("SYS_PARA","REF_DATE");
							sRefStartDateTime = sRefEndDateTime;
							sRefEndDateTime += NewMacth_INI.GetValue("SYS_PARA","END_BET_TIME");
							sRefStartDateTime += NewMacth_INI.GetValue("SYS_PARA","START_BET_TIME");
							sDayToAdd = NewMacth_INI.GetValue("SYS_PARA","BET_DAYS");
							m_ImpersonationContext.Undo();

							//write log
							m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
							m_SportsLog.SetFileName(0,LOGFILESUFFIX);
							m_SportsLog.Open();
							m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " OtherSoccerNewMatch.cs.Add(): Impersonate success.");
							m_SportsLog.Close();
						} else {
							m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
							m_SportsLog.SetFileName(0,LOGFILESUFFIX);
							m_SportsLog.Open();
							m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " OtherSoccerNewMatch.cs.Add(): m_ImpersonationContext is null.");
							m_SportsLog.Close();
						}
					} else {
						m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
						m_SportsLog.SetFileName(0,LOGFILESUFFIX);
						m_SportsLog.Open();
						m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " OtherSoccerNewMatch.cs.Add(): DuplicateToken error, code = " + Marshal.GetLastWin32Error());
						m_SportsLog.Close();
					}
				} else {
					m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
					m_SportsLog.SetFileName(0,LOGFILESUFFIX);
					m_SportsLog.Open();
					m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " OtherSoccerNewMatch.cs.Add(): LogonUser error, code = " + Marshal.GetLastWin32Error());
					m_SportsLog.Close();
				}

				//get reference start date time
				dtRefStart = DateTime.ParseExact(sRefStartDateTime,sFormatPattern,formatter);
				//get reference end date time
				dtRefEnd = DateTime.ParseExact(sRefEndDateTime,sFormatPattern,formatter);
				dtRefEnd = dtRefEnd.AddDays(Convert.ToDouble(sDayToAdd));

				for(iUpdIndex=0;iUpdIndex<RECORDSOINPAGE;iUpdIndex++) {	//check for records needed to be inserted
					if(!arrHost[iUpdIndex].Equals(arrGuest[iUpdIndex])) {	//insert for different host and guest
						//get Match date time
						sMatchDate = arrYear[iUpdIndex] + arrMonth[iUpdIndex] + arrDate[iUpdIndex];
						sMatchTime = arrHour[iUpdIndex] + arrMinute[iUpdIndex];
						dtMatch = DateTime.ParseExact(sMatchDate+sMatchTime,sFormatPattern,formatter);

						//Calculate interval: use for chart-plotting
						TimeDiff = dtMatch.Subtract(dtRefEnd);
						dDiff = TimeDiff.TotalSeconds;
						if(dDiff > 0) {	//outstanding match
							TimeDiff = dtRefEnd.Subtract(dtRefStart);
						}	else {	//match in current period
							TimeDiff = dtMatch.Subtract(dtRefStart);
						}
						dDiff = TimeDiff.TotalSeconds;
						iInterval = (int)(dDiff/XAXISCONSTANT);
						if(iInterval < MINTIMESLOT) iInterval = MINTIMESLOT;

						try {
							//Add a new match: insert into othersoccerinfo
							//get max of match count
							SQLString.Remove(0,SQLString.Length);
							SQLString.Append("select MAX(MATCH_CNT) from othersoccerinfo");
							m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
							if(m_SportsOleReader.Read()) {
								if(!m_SportsOleReader.IsDBNull(0)) iMax = m_SportsOleReader.GetInt32(0);
							}
							m_SportsOleReader.Close();
							m_SportsDBMgr.Close();
							iMax++;
							/*
							if(iMax < 10) sOrder = "0" + iMax.ToString();
							else if(iMax > 99) sOrder = "99";
							else sOrder = iMax.ToString();
							*/
							if(iMax < 10) sOrder = "00" + iMax.ToString();
							else if(iMax < 100) sOrder = "0" + iMax.ToString();
							else if(iMax > 999) sOrder = "999";
							else sOrder = iMax.ToString();

							SQLString.Remove(0,SQLString.Length);
							SQLString.Append("insert into othersoccerinfo values('");
							SQLString.Append(sAlias);
							SQLString.Append("','");
							SQLString.Append(arrHost[iUpdIndex]);
							SQLString.Append("','");
							SQLString.Append(arrGuest[iUpdIndex]);
							SQLString.Append("','");
							SQLString.Append(DEFAULTACT);
							SQLString.Append("','");
							SQLString.Append(sOrder);
							SQLString.Append("','");
							SQLString.Append(sMatchDate);
							SQLString.Append("','");
							SQLString.Append(sMatchTime);
							SQLString.Append("',");
							for(iArrayIndex=0;iArrayIndex<arrField.Length;iArrayIndex++) {
								if(Convert.ToInt32(arrField[iArrayIndex]) == iUpdIndex) {
									bUpdFlag = true;
									break;
								}
							}
							if(bUpdFlag) SQLString.Append("'M',");
							else SQLString.Append("'H',");
							bUpdFlag = false;

							for(iArrayIndex=0;iArrayIndex<arrHostHandicap.Length;iArrayIndex++) {
								if(Convert.ToInt32(arrHostHandicap[iArrayIndex]) == iUpdIndex) {
									bUpdFlag = true;
									break;
								}
							}
							SQLString.Append("'");
							if(bUpdFlag) SQLString.Append(DEFAULTFLAG);
							else SQLString.Append(ZEROFLAG);
							SQLString.Append("',");
							bUpdFlag = false;

							SQLString.Append("'','','','','");
							SQLString.Append(DEFAULTFLAG);
							SQLString.Append("','','");
							SQLString.Append(DEFAULTFLAG);
							SQLString.Append("','");
							SQLString.Append(DEFAULTFLAG);
							SQLString.Append("','','");
							SQLString.Append(DEFAULTFLAG);
							SQLString.Append("','','','','");
							SQLString.Append(ZEROFLAG);
							SQLString.Append("','");
							SQLString.Append(ZEROFLAG);
							SQLString.Append("','");
							SQLString.Append(ZEROFLAG);
							SQLString.Append("','");
							SQLString.Append(ZEROFLAG);
							SQLString.Append("','");
							SQLString.Append(ZEROFLAG);
							SQLString.Append("','");
							SQLString.Append(ZEROFLAG);
							SQLString.Append("','");
							SQLString.Append(sLeague);
							SQLString.Append("',");
							SQLString.Append(iMax.ToString());
							SQLString.Append(",'");
							SQLString.Append(DEFAULTWEBACT);
							SQLString.Append("',");
							SQLString.Append(iInterval.ToString());
							SQLString.Append(")");
							m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
							m_SportsDBMgr.Close();
							//MatchReplicator.Replicate(SQLString.ToString());
							iRecUpd++;

							//Init a new othersoccer live goal: insert into othersoccergoalinfo
							SQLString.Remove(0,SQLString.Length);
							SQLString.Append("insert into OTHERSOCCERGOALINFO values(");
							SQLString.Append(iMax.ToString());
							SQLString.Append(",");
							SQLString.Append(iMax.ToString());
							SQLString.Append(",'");
							SQLString.Append(sOrder);
							SQLString.Append("','");
							SQLString.Append(DEFAULTACT);
							SQLString.Append("','");
							SQLString.Append(DEFAULTWEBACT);
							SQLString.Append("','");
							SQLString.Append(ZEROFLAG);
							SQLString.Append("','");
							SQLString.Append(DEFAULTFLAG);
							SQLString.Append("','");
							SQLString.Append(ZEROFLAG);
							SQLString.Append("','");
							SQLString.Append(DEFAULTFLAG);
							SQLString.Append("','");
							SQLString.Append(ZEROFLAG);
							SQLString.Append("','");
							SQLString.Append(songNVC.Keys[0].ToString());
							SQLString.Append("','");
							SQLString.Append(ZEROFLAG);
							SQLString.Append("','");
							SQLString.Append(ZEROFLAG);
							SQLString.Append("','");
							SQLString.Append(sMatchDate);
							SQLString.Append("','");
							SQLString.Append(sMatchTime);
							SQLString.Append("',null)");
							m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
							m_SportsDBMgr.Close();

							//MatchReplicator.Replicate(SQLString.ToString());
						}	catch(OleDbException) {
							iRecUpd = -99;
						}
					}	else {	//break for same host and guest
						break;
					}
				}
/*
				if(iRecUpd > 0) {
					MatchReplicator.Dispose();
					m_SportsDBMgr.Dispose();
				}
*/
				//write log
				m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " OtherSoccerNewMatch.cs: Add " + iRecUpd.ToString() + " matches (" + HttpContext.Current.Session["user_name"] + ")");
				m_SportsLog.Close();
			} catch(Exception ex) {
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " OtherSoccerNewMatch.cs.Add(): " + ex.ToString());
				m_SportsLog.Close();
			}

			return iRecUpd;
		}
	}
}