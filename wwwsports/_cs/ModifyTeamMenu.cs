/*
Objective:
Menu of team modify

Last updated:
19 July 2004, Chapman Choi

C#.NET complier statement:
csc /t:library /out:..\bin\ModifyTeamMenu.dll /r:..\bin\DBManager.dll;..\bin\Files.dll ModifyTeamMenu.cs
*/

using System;
using System.Data.OleDb;
using System.Text;
using System.Reflection;
using System.Web;
using TDL.DB;
using TDL.IO;

[assembly:AssemblyCompany("TDSL")]
[assembly:AssemblyCopyright("(c) 2003 TDSL. All rights reserved. Created on 19 June 2003.")]
[assembly:AssemblyDescription("一般設定 -> 隊伍管理 -> 修改(隊伍選項)")]
[assembly:AssemblyProduct("Sports.NET")]
[assembly:AssemblyTitle("Sports.NET DLL")]
[assembly:AssemblyVersion("1.0.*")]
namespace SportsUtil {
	public class ModifyTeamMenu {
		const int INDEXCOLS = 20;
		const int ITEMCOLS = 12;
		const string LOGFILESUFFIX = "log";
		string m_Action;
		OleDbDataReader m_SportsOleReader;
		DBManager m_SportsDBMgr;
		Files m_SportsLog;
		StringBuilder HTMLString;
		StringBuilder SQLString;

		public ModifyTeamMenu(string Connection) {
			m_SportsDBMgr = new DBManager();
			m_SportsDBMgr.ConnectionString = Connection;
			m_SportsLog = new Files();
			HTMLString = new StringBuilder();
			SQLString = new StringBuilder();
			m_Action = HttpContext.Current.Request.QueryString["ACT"];
		}

		public string GetIndex() {
			int iRowCount = 0;
			int iIndex = 0;
			string sFirstWord = "";

			try {
				m_SportsOleReader = m_SportsDBMgr.ExecuteQuery("select TEAM_ID, TEAMNAME from TEAMINFO order by TEAMNAME");
				while(m_SportsOleReader.Read()) {
					if(!sFirstWord.Equals(m_SportsOleReader.GetString(1).Trim().Substring(0,1))) {
						sFirstWord = m_SportsOleReader.GetString(1).Trim().Substring(0,1);
						if(iIndex%INDEXCOLS == 0) {
							HTMLString.Append("<tr align=\"center\" ");
							if(iRowCount%2 == 0) {
								HTMLString.Append("style=\"background-color:#FFFAF0\"");
							} else {
								HTMLString.Append("style=\"background-color:#F0F8FF\"");
							}
							HTMLString.Append(">");
							iRowCount++;
						}
						HTMLString.Append("<td><a href=\"TeamManager_SubMenu.aspx?teamID=");
						HTMLString.Append(m_SportsOleReader.GetInt32(0).ToString());
						HTMLString.Append("&ACT=");
						HTMLString.Append(m_Action);
						HTMLString.Append("\" target=\"submenu_frame\">");
						HTMLString.Append(sFirstWord);
						HTMLString.Append("</a></td>");
						if(iIndex%INDEXCOLS == INDEXCOLS-1) {
							HTMLString.Append("</tr>");
						}
						iIndex++;
					}
				}
				m_SportsOleReader.Close();
				m_SportsDBMgr.Close();
				HTMLString.Append("<input type=\"hidden\" name=\"action\" value=\"");
				HTMLString.Append(m_Action);
				HTMLString.Append("\">");
			}	catch(Exception ex) {
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " ModifyTeamMenu.cs.GetIndex(): " + ex.ToString());
				m_SportsLog.Close();
				HTMLString.Remove(0, HTMLString.Length);
				HTMLString.Append((string)HttpContext.Current.Application["accessErrorMsg"]);
			}

			return HTMLString.ToString();
		}

		public string GetTeamGroup() {
			int iRowCount = 0;
			int iIndex = 0;
			string sTeamID;
			string sWordIndex = "";

			sTeamID = HttpContext.Current.Request.QueryString["teamID"];
			try {
				SQLString.Remove(0, SQLString.Length);
				if(sTeamID != null) {
					SQLString.Append("select TEAMNAME from TEAMINFO where TEAM_ID=");
					SQLString.Append(sTeamID);
				} else {
					SQLString.Append("select TEAMNAME from TEAMINFO order by TEAMNAME");
				}
				m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
				if(m_SportsOleReader.Read()) {
					if(!m_SportsOleReader.IsDBNull(0)) {
						sWordIndex = m_SportsOleReader.GetString(0).Trim().Substring(0,1);
					}
				}
				m_SportsOleReader.Close();
				m_SportsDBMgr.Close();

				SQLString.Remove(0, SQLString.Length);
				SQLString.Append("select TEAM_ID, TEAMNAME from TEAMINFO where TEAMNAME like '");
				SQLString.Append(sWordIndex);
				SQLString.Append("%' order by TEAMNAME");
				m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
				while(m_SportsOleReader.Read()) {
					if(iIndex%ITEMCOLS == 0) {
						HTMLString.Append("<tr align=\"left\" ");
						if(iRowCount%2 == 0) {
							HTMLString.Append("style=\"background-color:#E6E6FA\"");
						} else {
							HTMLString.Append("style=\"background-color:#E0FFFF\"");
						}
						HTMLString.Append(">");
						iRowCount++;
					}
					HTMLString.Append("<td><a href=\"");
					if(m_Action.Equals("MOD")) {
						HTMLString.Append("TeamManager_MOD.aspx");
					} else {
						HTMLString.Append("TeamManager_DEL.aspx");
					}
					HTMLString.Append("?teamID=");
					HTMLString.Append(m_SportsOleReader.GetInt32(0).ToString());
					HTMLString.Append("\" target=\"content_frame\">");
					HTMLString.Append(m_SportsOleReader.GetString(1).Trim());
					HTMLString.Append("</a></td>");
					if(iIndex%ITEMCOLS == ITEMCOLS-1) {
						HTMLString.Append("</tr>");
					}
					iIndex++;
				}
				m_SportsOleReader.Close();
				m_SportsDBMgr.Close();
			}	catch(Exception ex) {
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " ModifyTeamMenu.cs.GetTeamGroup(): " + ex.ToString());
				m_SportsLog.Close();
				HTMLString.Remove(0, HTMLString.Length);
				HTMLString.Append((string)HttpContext.Current.Application["accessErrorMsg"]);
			}

			return HTMLString.ToString();
		}
	}
}