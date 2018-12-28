/*
Objective:
Import soccer schedule to Asia Sports
Enquiry by league, date is allowed

Last updated:
5 Apr 2005 by Paddy, try catch the case of get Null String of hostname/guestname
15 Nov 2004 (Fanny Cheung) Increment orderId into 3 digits
31 Oct 2003 by Chapman

C#.NET complier statement:
csc /t:library /out:..\bin\SoccerSchedule.dll /r:..\bin\DBManager.dll;..\bin\Win32INI.dll;..\bin\Files.dll SoccerSchedule.cs
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

[assembly:AssemblyCompany("TDSL")]
[assembly:AssemblyCopyright("(c) 2003 TDSL. All rights reserved.")]
[assembly:AssemblyDescription("足球資訊 -> 匯入賽程")]
[assembly:AssemblyProduct("Sports.NET")]
[assembly:AssemblyTitle("Sports.NET DLL")]
[assembly:AssemblyVersion("1.0.*")]
namespace SportsUtil {
	public class SoccerSchedule {
		const int LOGON32_LOGON_INTERACTIVE = 2;
		const int LOGON32_PROVIDER_DEFAULT = 0;
		const int SecurityImpersonation = 2;
		const string LOGFILESUFFIX = "log";
		int m_RecordCount = 0;
		OleDbDataReader m_SportsOleReader;
		DBManager m_SportsDBMgr;
		Files m_SportsLog;
		StringBuilder SQLString;

		[DllImport("advapi32.dll", SetLastError=true)]
		public static extern bool LogonUser(string lpszUserName, string lpszDomain, string lpszPassword, int dwLogonType, int dwLogonProvider, ref IntPtr phToken);

		[DllImport("advapi32.dll", SetLastError=true)]
		public extern static int DuplicateToken(IntPtr hToken, int impersonationLevel, ref IntPtr hNewToken);

		public SoccerSchedule(string Connection) {
			m_SportsDBMgr = new DBManager();
			m_SportsDBMgr.ConnectionString = Connection;
			m_SportsLog = new Files();
			SQLString = new StringBuilder();
		}

		public int RecordCount {
			get {
				return m_RecordCount;
			}
		}

		public string list() {
			string sLeagID;
			string sEnquiryOut = "";
			string sMatchYear;
			string sMatchMonth;
			string sMatchDay;
			string sTempYear = "";
			string sYearOption = "";
			string sTempMonth = "";
			string sMonthOption = "";
			string sTempDate = "";
			string sDateOption = "";
			string uid;
			ArrayList monthList = new ArrayList(12);
			ArrayList dateList = new ArrayList(15);
			StringBuilder HTMLString = new StringBuilder();

			uid = HttpContext.Current.Session["user_id"].ToString();
			sLeagID = HttpContext.Current.Request.QueryString["leagID"];
			sMatchYear = HttpContext.Current.Request.QueryString["Year"];
			sMatchMonth = HttpContext.Current.Request.QueryString["Month"];
			sMatchDay = HttpContext.Current.Request.QueryString["Date"];
			try {
				sEnquiryOut += "<tr><th colspan=\"4\">顯示<select name=\"leagID\"><option value=\"0000\">所有</option>";
				SQLString.Remove(0,SQLString.Length);
				SQLString.Append("select distinct leag.LEAG_ID, leag.ALIAS from LEAGINFO leag, SOCCERSCHEDULE sch where leag.leag_id in (select cleag_id from userprofile_info where iuser_id=");
				SQLString.Append(uid);
				SQLString.Append(") AND leag.leag_id = sch.cleag_id order by leag.LEAG_ORDER, leag.LEAG_ID");
/*
				SQLString.Append("select distinct leaginfo.LEAG_ID, leaginfo.ALIAS from LEAGINFO, userprofile_info, SOCCERSCHEDULE where leaginfo.leag_id in (select distinct cleag_id from userprofile_info where iuser_id=");
				SQLString.Append(uid);
				SQLString.Append(") AND leaginfo.ALIAS = SOCCERSCHEDULE.CLEAGUEALIAS order by LEAG_ORDER, LEAG_ID");
*/
				m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
				while(m_SportsOleReader.Read()) {
					sEnquiryOut += "<option value=\"" + m_SportsOleReader.GetString(0).Trim() + "\">" + m_SportsOleReader.GetString(1).Trim() + "</option>";
				}
				m_SportsOleReader.Close();
				m_SportsDBMgr.Close();
				sEnquiryOut += "</select>聯賽 及 ";

				sYearOption += "<select name=\"Year\"><option value=\"0000\">所有</option>";
				sMonthOption += "<select name=\"Month\"><option value=\"0000\">所有</option>";
				sDateOption += "<select name=\"Date\"><option value=\"0000\">所有</option>";
				SQLString.Remove(0,SQLString.Length);
				SQLString.Append("select MATCHDATETIME from SOCCERSCHEDULE order by MATCHDATETIME");
				m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
				while(m_SportsOleReader.Read()) {
					if(!sTempYear.Equals(m_SportsOleReader.GetDateTime(0).ToString("yyyy"))) {
						sTempYear = m_SportsOleReader.GetDateTime(0).ToString("yyyy");
						sYearOption += "<option value=\"" + sTempYear + "\">" + sTempYear + "</option>";
					}

					sTempMonth = m_SportsOleReader.GetDateTime(0).ToString("MM");
					if(!monthList.Contains(sTempMonth)) {
						monthList.Add(sTempMonth);
						sMonthOption += "<option value=\"" + sTempMonth + "\">" + sTempMonth + "</option>";
					}

					sTempDate = m_SportsOleReader.GetDateTime(0).ToString("dd");
					if(!dateList.Contains(sTempDate)) {
						dateList.Add(sTempDate);
						sDateOption += "<option value=\"" + sTempDate + "\">" + sTempDate + "</option>";
					}
				}
				sYearOption += "</select>年 ";
				sMonthOption += "</select>月 ";
				sDateOption += "</select>日 ";
				m_SportsOleReader.Close();
				m_SportsDBMgr.Close();
				sEnquiryOut += sYearOption + sMonthOption + sDateOption + "<input type=\"button\" value=\"顯示\" onClick=\"filterSchedule('SoccerSchedule.aspx?leagID=' + ScheduleForm.leagID.value + '&Year=' + ScheduleForm.Year.value + '&Month=' + ScheduleForm.Month.value + '&Date=' + ScheduleForm.Date.value)\"></th</tr>";

				HTMLString.Append(sEnquiryOut);
				HTMLString.Append("<tr align=\"center\" style=\"background-color:#D02090; color=#FFF5EE\"><th>賽事日期</th><th>聯賽</th><th>賽事</th><th><input type=\"checkbox\" name=\"all\" value=\"1\" onClick=\"selectAll()\">全選</th></tr>");
				SQLString.Remove(0,SQLString.Length);
				SQLString.Append("select MATCHDATETIME, ALIAS, LEAGNAME, CHOST_HANDI, CFIELD, (select teamname from teaminfo where team_id=sch.ihost_team_id) CHOST, (select teamname from teaminfo where team_id=sch.iguest_team_id) CGUEST, IMATCH_CNT from SOCCERSCHEDULE sch, leaginfo lg ");
				SQLString.Append("where sch.cleag_id=lg.leag_id ");
				if(sLeagID != null) {
					if(!sLeagID.Equals("0000")) {
						SQLString.Append("and sch.cleag_id='");
						SQLString.Append(sLeagID);
						SQLString.Append("' ");
					}
				}
				if(sMatchYear != null) {
					if(!sMatchYear.Equals("0000")) {
						SQLString.Append("and extract(YEAR from MATCHDATETIME)=");
						SQLString.Append(sMatchYear);
						SQLString.Append(" ");
					}
				}
				if(sMatchMonth != null) {
					if(!sMatchMonth.Equals("0000")) {
						SQLString.Append("and extract(MONTH from MATCHDATETIME)=");
						SQLString.Append(sMatchMonth);
						SQLString.Append(" ");
					}
				}
				if(sMatchDay != null) {
					if(!sMatchDay.Equals("0000")) {
						SQLString.Append("and extract(DAY from MATCHDATETIME)=");
						SQLString.Append(sMatchDay);
						SQLString.Append(" ");
					}
				}
				SQLString.Append("and CSTATUS <> 'C' ");
				SQLString.Append("order by MATCHDATETIME, ALIAS");
				m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
				while(m_SportsOleReader.Read()) {
					//Match date time
					HTMLString.Append("<tr align=\"center\"><td align=\"center\">");
					HTMLString.Append(m_SportsOleReader.GetDateTime(0).ToString("yyyy/MM/dd HH:mm"));
					HTMLString.Append("<input type=\"hidden\" name=\"MatchDate\" value=\"");
					HTMLString.Append(m_SportsOleReader.GetDateTime(0).ToString("yyyyMMdd"));
					HTMLString.Append("\"><input type=\"hidden\" name=\"MatchTime\" value=\"");
					HTMLString.Append(m_SportsOleReader.GetDateTime(0).ToString("HHmm"));
					HTMLString.Append("\"></td>");

					//Alias
					HTMLString.Append("<td align=\"center\">");
					HTMLString.Append(m_SportsOleReader.GetString(1).Trim());
					HTMLString.Append("<input type=\"hidden\" name=\"Alias\" value=\"");
					HTMLString.Append(m_SportsOleReader.GetString(1).Trim());
					HTMLString.Append("\"><input type=\"hidden\" name=\"League\" value=\"");
					HTMLString.Append(m_SportsOleReader.GetString(2).Trim());
					HTMLString.Append("\"></td>");

					//String format as same as pager -> 賽事
					HTMLString.Append("<td align=\"center\">");
					if(m_SportsOleReader.GetString(3).Trim().Equals("1")) {
						if(m_SportsOleReader.GetString(4).Trim().Equals("H")) {
							HTMLString.Append("(主) ");
							try{
								HTMLString.Append(m_SportsOleReader.GetString(5));
							}catch{
								HTMLString.Append("<FONT COLOR=\"#ff0000\">N\\A</FONT>");	
							}
							HTMLString.Append(" vs ");
							try{
								HTMLString.Append(m_SportsOleReader.GetString(6));
							}catch{
								HTMLString.Append("<FONT COLOR=\"#ff0000\">N\\A</FONT>");	
							}
						} else {				
							HTMLString.Append("(中) ");
							try{
								HTMLString.Append(m_SportsOleReader.GetString(5));
							}catch{
								HTMLString.Append("<FONT COLOR=\"#ff0000\">N\\A</FONT>");
							}
							HTMLString.Append(" vs ");
							try{
								HTMLString.Append(m_SportsOleReader.GetString(6));
							}catch{
								HTMLString.Append("<FONT COLOR=\"#ff0000\">N\\A</FONT>");	
							}	
						}
					} else {
						if(m_SportsOleReader.GetString(4).Trim().Equals("H")) {
							HTMLString.Append("(客) ");
							try{
								HTMLString.Append(m_SportsOleReader.GetString(6));
							}catch{
								HTMLString.Append("<FONT COLOR=\"#ff0000\">N\\A</FONT>");	
							}
							HTMLString.Append(" vs ");
							try{
								HTMLString.Append(m_SportsOleReader.GetString(5));
							}catch{
								HTMLString.Append("<FONT COLOR=\"#ff0000\">N\\A</FONT>");	
							}
						} else {
							HTMLString.Append("(中) ");
							try{
								HTMLString.Append(m_SportsOleReader.GetString(6));
							}catch{
								HTMLString.Append("<FONT COLOR=\"#ff0000\">N\\A</FONT>");
							}
							HTMLString.Append(" vs ");
							try{
								HTMLString.Append(m_SportsOleReader.GetString(5));
							}catch{
								HTMLString.Append("<FONT COLOR=\"#ff0000\">N\\A</FONT>");	
							}
						}
					}
					HTMLString.Append("<input type=\"hidden\" name=\"HostHandicap\" value=\"");
					HTMLString.Append(m_SportsOleReader.GetString(3));
					HTMLString.Append("\"><input type=\"hidden\" name=\"MatchField\" value=\"");
					HTMLString.Append(m_SportsOleReader.GetString(4));
					HTMLString.Append("\">");
					HTMLString.Append("<input type=\"hidden\" name=\"Host\" value=\"");
					try{
						HTMLString.Append(m_SportsOleReader.GetString(5).Trim());
					}
					catch{
						HTMLString.Append("N\\A");
					}
					HTMLString.Append("\"><input type=\"hidden\" name=\"Guest\" value=\"");
					try{
						HTMLString.Append(m_SportsOleReader.GetString(6).Trim());
					}
					catch{
						HTMLString.Append("N\\A");
					}
					HTMLString.Append("\"></td>");

					//Import
					HTMLString.Append("<td align=\"center\"><input type=\"checkbox\" name=\"importedMatch\" value=\"");
					HTMLString.Append(m_RecordCount.ToString());
					HTMLString.Append("\">");

					//Hidden match count
					HTMLString.Append("<input type=\"hidden\" name=\"MatchCount\" value=\"");
					HTMLString.Append(m_SportsOleReader.GetInt32(7).ToString());
					HTMLString.Append("\"></td></tr>");
					m_RecordCount++;
				}
				m_SportsDBMgr.Close();
				m_SportsOleReader.Close();
				m_SportsDBMgr.Dispose();

				if(m_RecordCount == 0) {
					HTMLString.Remove(0,HTMLString.Length);
					HTMLString.Append(sEnquiryOut);
					HTMLString.Append("<tr><th colspan=\"4\">沒有賽程或已取消</th></tr>");
					if(sLeagID != null) {
						if(!sLeagID.Equals("0000")) {
							HTMLString.Remove(0,HTMLString.Length);
							HTMLString.Append(sEnquiryOut);
							HTMLString.Append("<tr><th colspan=\"4\">沒有賽程在此聯賽或已取消</th></tr>");
						}
					}
				}
			}	catch(Exception ex) {
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " SoccerSchedule.cs.list(): " + ex.ToString());
				m_SportsLog.Close();
				HTMLString.Remove(0,HTMLString.Length);
				HTMLString.Append((string)HttpContext.Current.Application["accessErrorMsg"]);
			}
			return HTMLString.ToString();
		}

		public int import() {
			const int MINTIMESLOT=65;
			const double XAXISCONSTANT = 64;
			int iImported = 0;
			int iImportIdx = 0;
			int iErrCnt = 0;
			int iMatchCnt = 0;
			int iImportCount;
			int iInterval = 0;
			double dDiff = 0;
			char[] delimiter = new char[] {','};
			string sOrder = "";
			string sDayToAdd = null;
			string sRefStartDateTime = null;
			string sRefEndDateTime = null;
			string sFormatPattern;
			string[] arrImport;
			string[] arrMatchDate;
			string[] arrMatchTime;
			string[] arrAlias;
			string[] arrLeague;
			string[] arrHost;
			string[] arrGuest;
			string[] arrHostHandicap;
			string[] arrMatchField;
			NameValueCollection songNVC;
			TimeSpan TimeDiff;
			DateTime dtRefStart, dtRefEnd, dtMatch;
			CultureInfo formatter;

			dtRefStart = new DateTime(0);
			dtRefEnd = new DateTime(0);
			dtMatch = new DateTime(0);
			formatter = new CultureInfo("en-US");
			sFormatPattern = "yyyyMMddHHmm";
			songNVC = (NameValueCollection)HttpContext.Current.Application["songItems"];

			bool m_LoggedOn = false;
			string m_NetUser = "ASPUSER";
			string m_Password = "aspspt";
			string m_Domain = System.Environment.MachineName;
			WindowsImpersonationContext m_ImpersonationContext;
			WindowsIdentity m_TempWindowsIdentity;
			IntPtr m_Token = IntPtr.Zero;
			IntPtr m_TokenDuplicate = IntPtr.Zero;

			try {
				arrImport = HttpContext.Current.Request.Form["importedMatch"].Split(delimiter);
				iImportCount = arrImport.Length;
				arrMatchDate = HttpContext.Current.Request.Form["MatchDate"].Split(delimiter);
				arrMatchTime = HttpContext.Current.Request.Form["MatchTime"].Split(delimiter);
				arrAlias = HttpContext.Current.Request.Form["Alias"].Split(delimiter);
				arrLeague = HttpContext.Current.Request.Form["League"].Split(delimiter);
				arrHost = HttpContext.Current.Request.Form["Host"].Split(delimiter);
				arrGuest = HttpContext.Current.Request.Form["Guest"].Split(delimiter);
				arrHostHandicap = HttpContext.Current.Request.Form["HostHandicap"].Split(delimiter);
				arrMatchField = HttpContext.Current.Request.Form["MatchField"].Split(delimiter);
			}	catch(Exception) {
				arrImport = new string[0];
				iImportCount = 0;
				arrMatchDate = new string[0];
				arrMatchTime = new string[0];
				arrAlias = new string[0];
				arrLeague = new string[0];
				arrHost = new string[0];
				arrGuest = new string[0];
				arrHostHandicap = new string[0];
				arrMatchField = new string[0];
			}

			try {
				//Delete record which marked as 'D'
				SQLString.Remove(0,SQLString.Length);
				SQLString.Append("delete from GAMEINFO where ACT='D'");
				m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
				m_SportsDBMgr.Close();
				SQLString.Remove(0,SQLString.Length);
				SQLString.Append("delete from GOALINFO where ACT='D'");
				m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
				m_SportsDBMgr.Close();

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
							m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " SoccerSchedule.cs.import(): Impersonate success.");
							m_SportsLog.Close();
						} else {
							m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
							m_SportsLog.SetFileName(0,LOGFILESUFFIX);
							m_SportsLog.Open();
							m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " SoccerSchedule.cs.import(): m_ImpersonationContext is null.");
							m_SportsLog.Close();
						}
					} else {
						m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
						m_SportsLog.SetFileName(0,LOGFILESUFFIX);
						m_SportsLog.Open();
						m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " SoccerSchedule.cs.import(): DuplicateToken error, code = " + Marshal.GetLastWin32Error());
						m_SportsLog.Close();
					}
				} else {
					m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
					m_SportsLog.SetFileName(0,LOGFILESUFFIX);
					m_SportsLog.Open();
					m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " SoccerSchedule.cs.import(): LogonUser error, code = " + Marshal.GetLastWin32Error());
					m_SportsLog.Close();
				}

				//get reference start date time
				dtRefStart = DateTime.ParseExact(sRefStartDateTime,sFormatPattern,formatter);
				//get reference end date time
				dtRefEnd = DateTime.ParseExact(sRefEndDateTime,sFormatPattern,formatter);
				dtRefEnd = dtRefEnd.AddDays(Convert.ToDouble(sDayToAdd));

				for(int itemIdx = 0; itemIdx < iImportCount; itemIdx++) {
					iImportIdx = Convert.ToInt32(arrImport[itemIdx]);
					try {
						//get Match date time
						dtMatch = DateTime.ParseExact(arrMatchDate[iImportIdx]+arrMatchTime[iImportIdx],sFormatPattern,formatter);

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

						SQLString.Remove(0,SQLString.Length);
						SQLString.Append("select max(MATCH_CNT) from GAMEINFO");
						m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
						if(m_SportsOleReader.Read()) {
							if(!m_SportsOleReader.IsDBNull(0)) {
								iMatchCnt = m_SportsOleReader.GetInt32(0);
								iMatchCnt++;
							} else {
								iMatchCnt = 1;
							}
						} else iMatchCnt = 1;
						m_SportsOleReader.Close();
						m_SportsDBMgr.Close();

						//if(iMatchCnt < 10) sOrder = "0" + iMatchCnt.ToString();
						//else if(iMatchCnt > 99) sOrder = "99";
						//else sOrder = iMatchCnt.ToString();
						if(iMatchCnt < 10) sOrder = "00" + iMatchCnt.ToString();
						else if(iMatchCnt < 100) sOrder = "0" + iMatchCnt.ToString();
						else if(iMatchCnt > 999) sOrder = "999";
						else sOrder = iMatchCnt.ToString();

						SQLString.Remove(0,SQLString.Length);
						SQLString.Append("insert into GAMEINFO values('");
						SQLString.Append(arrAlias[iImportIdx]);
						SQLString.Append("','");
						SQLString.Append(arrHost[iImportIdx]);
						SQLString.Append("','");
						SQLString.Append(arrGuest[iImportIdx]);
						SQLString.Append("','U','");
						SQLString.Append(sOrder);
						SQLString.Append("','");
						SQLString.Append(arrMatchDate[iImportIdx]);
						SQLString.Append("','");
						SQLString.Append(arrMatchTime[iImportIdx]);
						SQLString.Append("','");
						SQLString.Append(arrMatchField[iImportIdx]);
						SQLString.Append("','");
						SQLString.Append(arrHostHandicap[iImportIdx]);
						SQLString.Append("',");
						SQLString.Append("'','','','','1','','1','1','','1','','','','0','0','0','0','0','0','");
						SQLString.Append(arrLeague[iImportIdx]);
						SQLString.Append("',");
						SQLString.Append(iMatchCnt.ToString());
						SQLString.Append(",'V',");
						SQLString.Append(iInterval.ToString());
						SQLString.Append(")");
						m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
						m_SportsDBMgr.Close();

						//Init a new live goal: insert into goalinfo
						SQLString.Remove(0,SQLString.Length);
						SQLString.Append("insert into GOALINFO values(");
						SQLString.Append(iMatchCnt.ToString());
						SQLString.Append(",");
						SQLString.Append(iMatchCnt.ToString());
						SQLString.Append(",'");
						SQLString.Append(sOrder);
						SQLString.Append("','U','V','0','1','0','1','0','");
						SQLString.Append(songNVC.Keys[0].ToString());
						SQLString.Append("','0','0','");
						SQLString.Append(arrMatchDate[iImportIdx]);
						SQLString.Append("','");
						SQLString.Append(arrMatchTime[iImportIdx]);
						SQLString.Append("',null)");
						m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
						m_SportsDBMgr.Close();
					} catch(OleDbException dbEx) {
						m_SportsDBMgr.Close();
						m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
						m_SportsLog.SetFileName(0,LOGFILESUFFIX);
						m_SportsLog.Open();
						m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " SoccerSchedule.cs.import(): Duplicate record existed.");
						m_SportsLog.Write(dbEx.ToString());
						m_SportsLog.Close();
						iErrCnt++;
					}
				}
				if(iImportCount > 0) m_SportsDBMgr.Dispose();
				iImported = iImportCount - iErrCnt;

				//write log
				m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " SoccerSchedule.cs.import(): Import " + iImported.ToString() + " matches (" + HttpContext.Current.Session["user_name"] + ")");
				m_SportsLog.Close();
			} catch(Exception ex) {
				iImported = -1;
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " SoccerSchedule.cs.import(): " + ex.ToString());
				m_SportsLog.Close();
			}

			return iImported;
		}
	}
}
