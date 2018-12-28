/*
Objective:
Display a list of soccer schedule provided modify and delete options
Enquiry by league, date is allowed

Last updated:
5 Apr 2005 by Paddy, try catch the case of get Null String of hostname/guestname
31 Oct 2003 by Chapman

C#.NET complier statement:
csc /t:library /out:..\bin\ListSoccerSchedule.dll /r:..\bin\DBManager.dll;..\bin\Files.dll ListSoccerSchedule.cs
*/

using System;
using System.Collections;
using System.Data.OleDb;
using System.Reflection;
using System.Text;
using System.Web;
using TDL.DB;
using TDL.IO;

[assembly:AssemblyCompany("TDSL")]
[assembly:AssemblyCopyright("(c) 2003 TDSL. All rights reserved.")]
[assembly:AssemblyDescription("���y�ɵ{ -> ����ɵ{")]
[assembly:AssemblyProduct("Sports.NET")]
[assembly:AssemblyTitle("Sports.NET DLL")]
[assembly:AssemblyVersion("1.0.*")]
namespace SportsUtil {
	public class ListSoccerSchedule {
		int m_RecordCount = 0;
		const string LOGFILESUFFIX = "log";
		OleDbDataReader m_SportsReader;
		DBManager m_SportsDB;
		Files m_SportsLog;

		public ListSoccerSchedule(string Connection) {
			m_SportsDB = new DBManager();
			m_SportsDB.ConnectionString = Connection;
			m_SportsLog = new Files();
		}

		public int RecordCount {
			get {
				return m_RecordCount;
			}
		}

		public string list() {
			string sEnquiryOut = "";
			string sLeagID;
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
			StringBuilder SQLString = new StringBuilder();

			uid = HttpContext.Current.Session["user_id"].ToString();
			sLeagID = HttpContext.Current.Request.QueryString["leagID"];
			sMatchYear = HttpContext.Current.Request.QueryString["Year"];
			sMatchMonth = HttpContext.Current.Request.QueryString["Month"];
			sMatchDay = HttpContext.Current.Request.QueryString["Date"];
			try {
				sEnquiryOut += "<tr><th colspan=\"6\">���<select name=\"leagID\"><option value=\"0000\">�Ҧ�</option>";

				SQLString.Remove(0,SQLString.Length);
				SQLString.Append("select distinct leag.LEAG_ID, leag.ALIAS from LEAGINFO leag, SOCCERSCHEDULE sch where leag.leag_id in (select cleag_id from userprofile_info where iuser_id=");
				SQLString.Append(uid);
				SQLString.Append(") AND leag.leag_id = sch.cleag_id order by leag.LEAG_ORDER, leag.ALIAS");
/*
				SQLString.Append("select distinct leaginfo.LEAG_ID, leaginfo.ALIAS from LEAGINFO, userprofile_info, SOCCERSCHEDULE where leaginfo.leag_id in (select distinct cleag_id from userprofile_info where iuser_id=");
				SQLString.Append(uid);
				SQLString.Append(") AND leaginfo.ALIAS = SOCCERSCHEDULE.CLEAGUEALIAS order by LEAG_ORDER, LEAG_ID");
*/
				m_SportsReader = m_SportsDB.ExecuteQuery(SQLString.ToString());
				while(m_SportsReader.Read()) {
						sEnquiryOut += "<option value=\"" + m_SportsReader.GetString(0).Trim() + "\">" + m_SportsReader.GetString(1).Trim() + "</option>";
				}
				m_SportsReader.Close();
				m_SportsDB.Close();
				sEnquiryOut += "</select>�p�� �� ";

				sYearOption += "<select name=\"Year\"><option value=\"0000\">�Ҧ�</option>";
				sMonthOption += "<select name=\"Month\"><option value=\"0000\">�Ҧ�</option>";
				sDateOption += "<select name=\"Date\"><option value=\"0000\">�Ҧ�</option>";
				SQLString.Remove(0,SQLString.Length);
				SQLString.Append("select MATCHDATETIME from SOCCERSCHEDULE order by MATCHDATETIME");
				m_SportsReader = m_SportsDB.ExecuteQuery(SQLString.ToString());
				while(m_SportsReader.Read()) {
					if(!sTempYear.Equals(m_SportsReader.GetDateTime(0).ToString("yyyy"))) {
						sTempYear = m_SportsReader.GetDateTime(0).ToString("yyyy");
						sYearOption += "<option value=\"" + sTempYear + "\">" + sTempYear + "</option>";
					}

					sTempMonth = m_SportsReader.GetDateTime(0).ToString("MM");
					if(!monthList.Contains(sTempMonth)) {
						monthList.Add(sTempMonth);
						sMonthOption += "<option value=\"" + sTempMonth + "\">" + sTempMonth + "</option>";
					}

					sTempDate = m_SportsReader.GetDateTime(0).ToString("dd");
					if(!dateList.Contains(sTempDate)) {
						dateList.Add(sTempDate);
						sDateOption += "<option value=\"" + sTempDate + "\">" + sTempDate + "</option>";
					}
				}
				sYearOption += "</select>�~ ";
				sMonthOption += "</select>�� ";
				sDateOption += "</select>�� ";
				m_SportsReader.Close();
				m_SportsDB.Close();
				sEnquiryOut += sYearOption + sMonthOption + sDateOption + "<input type=\"button\" value=\"���\" onClick=\"filterSchedule('ListSoccerSchedule.aspx?leagID=' + ScheduleForm.leagID.value + '&Year=' + ScheduleForm.Year.value + '&Month=' + ScheduleForm.Month.value + '&Date=' + ScheduleForm.Date.value)\"></th</tr>";

				HTMLString.Append(sEnquiryOut);
				HTMLString.Append("<tr align=\"center\" style=\"background-color:#0000CD; color=#FFFF00\"><th>�ɨƤ��</th><th>�p��</th><th>�ɨ�</th><th>�ק�</th><th><input type=\"submit\" value=\"�R��\">(<input type=\"checkbox\" name=\"selectAll\" value=\"1\" onClick=\"selectAllDelete()\">����)</th></tr>");
				SQLString.Remove(0,SQLString.Length);
				SQLString.Append("select MATCHDATETIME, ALIAS, CHOST_HANDI, CFIELD, ");
				SQLString.Append("(select teamname from teaminfo where team_id=sch.ihost_team_id) CHOST,");
				SQLString.Append("(select teamname from teaminfo where team_id=sch.iguest_team_id) CGUEST,");
				SQLString.Append("IMATCH_CNT, CSTATUS from LEAGINFO lg, SOCCERSCHEDULE sch ");
				SQLString.Append("where lg.leag_id=sch.cleag_id ");
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
				SQLString.Append("order by MATCHDATETIME, ALIAS");
				m_SportsReader = m_SportsDB.ExecuteQuery(SQLString.ToString());
				while(m_SportsReader.Read()) {
					//Match date time
					HTMLString.Append("<tr align=\"center\"><td align=\"center\">");
					HTMLString.Append(m_SportsReader.GetDateTime(0).ToString("yyyy/MM/dd HH:mm"));
					HTMLString.Append("</td>");

					//Alias
					HTMLString.Append("<td align=\"center\">");
					HTMLString.Append(m_SportsReader.GetString(1).Trim());
					HTMLString.Append("</td>");

					//String format as same as pager -> �ɨ�
					HTMLString.Append("<td align=\"center\">");
					if(m_SportsReader.GetString(2).Trim().Equals("1")) {
						if(m_SportsReader.GetString(3).Trim().Equals("H")) {
							HTMLString.Append("(�D) ");
							try{
								HTMLString.Append(m_SportsReader.GetString(4));
							}catch{
								HTMLString.Append("<FONT COLOR=\"#ff0000\">NULL</FONT>");
							}
							HTMLString.Append(" vs ");
							try{
								HTMLString.Append(m_SportsReader.GetString(5));
							}catch{
								HTMLString.Append("<FONT COLOR=\"#ff0000\">NULL</FONT>");	
							}
						} else {
							HTMLString.Append("(��) ");
							try{
								HTMLString.Append(m_SportsReader.GetString(4));
							}catch{
								HTMLString.Append("<FONT COLOR=\"#ff0000\">NULL</FONT>");	
							}
							HTMLString.Append(" vs ");
							try{
								HTMLString.Append(m_SportsReader.GetString(5));
							}catch{
								HTMLString.Append("<FONT COLOR=\"#ff0000\">NULL</FONT>");	
							}
						}
					} else {
						if(m_SportsReader.GetString(3).Trim().Equals("H")) {
							HTMLString.Append("(��) ");
							try{
								HTMLString.Append(m_SportsReader.GetString(5));
							}catch{
								HTMLString.Append("<FONT COLOR=\"#ff0000\">NULL</FONT>");	
							}
							HTMLString.Append(" vs ");
							try{
								HTMLString.Append(m_SportsReader.GetString(4));
							}catch{
								HTMLString.Append("<FONT COLOR=\"#ff0000\">NULL</FONT>");	
							}
						} else {
							HTMLString.Append("(��) ");
							try{
								HTMLString.Append(m_SportsReader.GetString(5));
							}catch{
								HTMLString.Append("<FONT COLOR=\"#ff0000\">NULL</FONT>");	
							}
							HTMLString.Append(" vs ");
							try{
								HTMLString.Append(m_SportsReader.GetString(4));
							}catch{
								HTMLString.Append("<FONT COLOR=\"#ff0000\">NULL</FONT>");	
							}
						}
					}
					HTMLString.Append("</td>");

					//Modify
					HTMLString.Append("<td align=\"center\"><a href=\"ModifySoccerSchedule.aspx?MatchCount=");
					HTMLString.Append(m_SportsReader.GetInt32(6).ToString());
					HTMLString.Append("\">�ק�</a></td>");

					//Delete
					HTMLString.Append("<td align=\"center\"><input type=\"checkbox\" name=\"selectedDelete\" value=\"");
					HTMLString.Append(m_SportsReader.GetInt32(6).ToString());
					HTMLString.Append("\"></td>");

					//Cancel
					HTMLString.Append("<td>");
					if(m_SportsReader.GetString(7).Equals("C")) {
						HTMLString.Append("�w����");
					} else {
						HTMLString.Append("���`");
					}
					HTMLString.Append("</td></tr>");
					m_RecordCount++;
				}
				m_SportsDB.Close();
				m_SportsReader.Close();
				m_SportsDB.Dispose();

				if(m_RecordCount == 0) {
					HTMLString.Remove(0,HTMLString.Length);
					HTMLString.Append(sEnquiryOut);
					HTMLString.Append("<tr><th colspan=\"6\">�S���ɵ{</th></tr>");
					if(sLeagID != null) {
						if(!sLeagID.Equals("0000")) {
							HTMLString.Remove(0,HTMLString.Length);
							HTMLString.Append(sEnquiryOut);
							HTMLString.Append("<tr><th colspan=\"6\">�S���ɵ{�b���p��</th></tr>");
						}
					}
				}
			}	catch(Exception ex) {
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " ListSoccerSchedule.cs.list(): " + ex.ToString());
				m_SportsLog.Close();
				HTMLString.Remove(0,HTMLString.Length);
				HTMLString.Append((string)HttpContext.Current.Application["accessErrorMsg"]);
			}
			return HTMLString.ToString();
		}
	}
}
