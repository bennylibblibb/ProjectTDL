/*
Objective:
Retrieval and modify matches analysis information

Last updated:
12 July 2004, Chapman Choi

C#.NET complier statement:
csc /t:library /out:..\bin\FTPScheduleCheck.dll /r:..\bin\DBManager.dll;..\bin\Files.dll FTPScheduleCheck.cs
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
[assembly:AssemblyDescription("FTP 檢查 -> 檢查選擇 -> 足球賽程")]
[assembly:AssemblyProduct("Sports.NET")]
[assembly:AssemblyTitle("Sports.NET DLL")]
[assembly:AssemblyVersion("1.0.*")]
namespace SportsUtil {
	public class FTPScheduleCheck {
		const string LOGFILESUFFIX = "log";
		int m_RecordCount = 0;
		OleDbDataReader m_SportsOleReader;
		DBManager m_SportsDBMgr;
		Files m_SportsLog;
		StringBuilder SQLString;

		public FTPScheduleCheck(string Connection) {
			m_SportsDBMgr = new DBManager();
			m_SportsDBMgr.ConnectionString = Connection;
			m_SportsLog = new Files();
			SQLString = new StringBuilder();
		}
		
		public int NumberOfRecords {
			get {
				return m_RecordCount;
			}
		}
		
		public string ShowCheck() {
			
			StringBuilder HTMLString = new StringBuilder();
			string sReportID, sLastUpdate, sLeagID, sLeagName, sLeagFlag, sHostID, sHostName, sHostFlag, sGuestID, sGuestName, sGuestFlag, sBatchJob;
			
			try {				
				SQLString.Remove(0,SQLString.Length);
				SQLString.Append("select IREPORT_ID, LASTUPDATE, CLEAG_ID, CLEAGNAME, CLEAG_FLAG, IHOST_ID, CHOST_NAME, CHOST_FLAG, IGUEST_ID, CGUEST_NAME, CGUEST_FLAG, CBATCHJOB from SOCCERSCHEDULE_MAPPING order by IREPORT_ID");
				m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
				while(m_SportsOleReader.Read()) {
					sReportID = m_SportsOleReader.GetInt32(0).ToString();
					sLastUpdate = m_SportsOleReader.GetDateTime(1).ToString("yyyy/MM/dd HH:mm:ss");
					
					if (!m_SportsOleReader.IsDBNull(2)) {
						sLeagID = m_SportsOleReader.GetString(2).Trim();
					} else {
						sLeagID = "";
					}
					if (!m_SportsOleReader.IsDBNull(3)) {
						sLeagName = m_SportsOleReader.GetString(3).Trim();
					} else {
						sLeagName = "";
					}
					
					sLeagFlag = m_SportsOleReader.GetString(4).Trim();
					
					if (!m_SportsOleReader.IsDBNull(5)) {
						sHostID = m_SportsOleReader.GetInt32(5).ToString();
					} else {
						sHostID = "";
					}
					if (!m_SportsOleReader.IsDBNull(6)) {
						sHostName = m_SportsOleReader.GetString(6).Trim();
					} else {
						sHostName = "";
					}
					
					sHostFlag = m_SportsOleReader.GetString(7).Trim();
					
					if (!m_SportsOleReader.IsDBNull(8)) {
						sGuestID = m_SportsOleReader.GetInt32(8).ToString();
					} else {
						sGuestID = "";
					}
					if (!m_SportsOleReader.IsDBNull(9)) {
						sGuestName = m_SportsOleReader.GetString(9).Trim();
					} else {
						sGuestName = "";
					}
					
					sGuestFlag = m_SportsOleReader.GetString(10).Trim();
					
					sBatchJob = m_SportsOleReader.GetString(11).Trim();
					
					HTMLString.Append("<tr align=\"center\">");
					
					// Report ID
					HTMLString.Append("<input type=\"hidden\" name=\"ReportID\" value=\"" +sReportID+ "\">");
					
					// Update time
					HTMLString.Append("<td align=\"center\">" +sLastUpdate+ "</td>");
					
					// League
					HTMLString.Append("<input type=\"hidden\" name=\"LeagueID\" value=\"" +sLeagID+ "\">");
					if (sLeagFlag == "2") {
						HTMLString.Append("<td align=\"center\"><a href=\"LeagueManager_ADD.aspx");
						HTMLString.Append("\"><font color=\"red\">" +sLeagName+ "</font></a></td>");
					} else {
						HTMLString.Append("<td align=\"center\">" +sLeagName+ "</td>");
					}
					
					// Host
					HTMLString.Append("<input type=\"hidden\" name=\"HostID\" value=\"" +sHostID+ "\">");
					if (sHostFlag == "1") {
						HTMLString.Append("<td align=\"center\"><a href=\"TeamManager_MOD.aspx?teamID=");
						HTMLString.Append(sHostID);
						HTMLString.Append("\"><font color=\"blue\">" +sHostName+ "</font></a></td>");
					} else if (sHostFlag == "2") {
						HTMLString.Append("<td align=\"center\"><a href=\"TeamManager_ADD.aspx");
						HTMLString.Append("\"><font color=\"red\">" +sHostName+ "</font></a></td>");
					} else {
						HTMLString.Append("<td align=\"center\">" +sHostName+ "</td>");
					}
					
					// Guest
					HTMLString.Append("<input type=\"hidden\" name=\"GuestID\" value=\"" +sGuestID+ "\">");
					if (sGuestFlag == "1") {
						HTMLString.Append("<td align=\"center\"><a href=\"TeamManager_MOD.aspx?teamID=");
						HTMLString.Append(sGuestID);
						HTMLString.Append("\"><font color=\"blue\">" +sGuestName+ "</font></a></td>");
					} else if (sGuestFlag == "2") {
						HTMLString.Append("<td align=\"center\"><a href=\"TeamManager_ADD.aspx");
						HTMLString.Append("\"><font color=\"red\">" +sGuestName+ "</font></a></td>");
					} else {
						HTMLString.Append("<td align=\"center\">" +sGuestName+ "</td>");
					}
					
					// File
					HTMLString.Append("<td align=\"center\">" +sBatchJob+ "</td>");
					
					// Action
					HTMLString.Append("<td align=\"center\"><input type=\"checkbox\" name=\"MustDelete\" value=\"");
					HTMLString.Append(m_RecordCount.ToString());
					HTMLString.Append("\"></td>");
					HTMLString.Append("</tr>");
					m_RecordCount++;
				}
				m_SportsOleReader.Close();
				m_SportsDBMgr.Close();
			} catch(Exception ex) {
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " SoccerScheduleCheck.cs.ShowCheck(): " + ex.ToString());
				m_SportsLog.Close();
				HTMLString.Remove(0,HTMLString.Length);
				HTMLString.Append((string)HttpContext.Current.Application["accessErrorMsg"]);
			}

			return HTMLString.ToString();
		}
		
		public int Delete() {
			
			int iMustDeleteLen = 0;
			int iRecDelete = 0;
			int iDelIndex = 0;
			char[] delimiter = new char[] {','};
			string[] arrMustDelete;
			string[] arrReportID;
			string sReportID = "";
			
			try {
				arrMustDelete = HttpContext.Current.Request.Form["MustDelete"].Split(delimiter);
				iMustDeleteLen = arrMustDelete.Length;
			}	catch(Exception) {
				arrMustDelete = new string[0];
				iMustDeleteLen = 0;
			}
			
			try {
				arrReportID = HttpContext.Current.Request.Form["ReportID"].Split(delimiter);
			} catch(Exception) {
				arrReportID = new string[0];
			}
			
			try {			
				for (iRecDelete=0; iRecDelete<iMustDeleteLen; iRecDelete++) {
					//update the checked(MustDelete) records only
					iDelIndex = Convert.ToInt32(arrMustDelete[iRecDelete]);
	
					sReportID = arrReportID[iDelIndex];
					
					SQLString.Remove(0,SQLString.Length);
					SQLString.Append("delete from SOCCERSCHEDULE_MAPPING where IREPORT_ID=");
					SQLString.Append(sReportID);
					m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
					m_SportsDBMgr.Close();
				}
			} catch(Exception ex) {
				iRecDelete = -1;
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " SoccerScheduleCheck.cs.Delete(): " + ex.ToString());
				m_SportsLog.Close();
			}
			
			return iRecDelete;
		}
	}
}