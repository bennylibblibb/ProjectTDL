/*
Objective:
Menu of modify league information

Last updated:
19 July 2004, Chapman Choi

C#.NET complier statement:
csc /t:library /out:..\bin\ModifyLeagueMenu.dll /r:..\bin\DBManager.dll;..\bin\Files.dll ModifyLeagueMenu.cs
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
[assembly:AssemblyDescription("一般設定 -> 聯賽管理 -> 修改(聯賽選項)")]
[assembly:AssemblyProduct("Sports.NET")]
[assembly:AssemblyTitle("Sports.NET DLL")]
[assembly:AssemblyVersion("1.0.*")]
namespace SportsUtil {
	public class ModifyLeagueMenu {
		const string LOGFILESUFFIX = "log";
		string m_Action;
		OleDbDataReader m_SportsOleReader;
		DBManager m_SportsDBMgr;
		Files m_SportsLog;
		StringBuilder HTMLString;
		StringBuilder SQLString;

		public ModifyLeagueMenu(string Connection) {
			m_SportsDBMgr = new DBManager();
			m_SportsDBMgr.ConnectionString = Connection;
			m_SportsLog = new Files();
			m_Action = HttpContext.Current.Request.QueryString["ACT"];
			HTMLString = new StringBuilder();
			SQLString = new StringBuilder();
		}

		public string GetIndex() {
			int iRowCount = 0;
			int iIndex = 0;
			string sFirstWord = "";

			try {
				m_SportsOleReader = m_SportsDBMgr.ExecuteQuery("select LEAG_ID, LEAGNAME from LEAGINFO order by LEAGNAME");
				while(m_SportsOleReader.Read()) {
					if(!sFirstWord.Equals(m_SportsOleReader.GetString(1).Trim().Substring(0,1))) {
						sFirstWord = m_SportsOleReader.GetString(1).Trim().Substring(0,1);
						if(iIndex%20 == 0) {
							HTMLString.Append("<tr align=\"center\" ");
							if(iRowCount%2 == 0) {
								HTMLString.Append("style=\"background-color:#FFFAF0\"");
							} else {
								HTMLString.Append("style=\"background-color:#F0F8FF\"");
							}
							HTMLString.Append(">");
							iRowCount++;
						}
						HTMLString.Append("<td><a href=\"LeagueManager_SubMenu.aspx?leagID=");
						HTMLString.Append(m_SportsOleReader.GetString(0).Trim());
						HTMLString.Append("&ACT=");
						HTMLString.Append(m_Action);
						HTMLString.Append("\" target=\"submenu_frame\">");
						HTMLString.Append(sFirstWord);
						HTMLString.Append("</a></td>");
						if(iIndex%20 == 19) {
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
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " ModifyLeagueMenu.cs.GetIndex(): " + ex.ToString());
				m_SportsLog.Close();
				HTMLString.Remove(0, HTMLString.Length);
				HTMLString.Append((string)HttpContext.Current.Application["accessErrorMsg"]);
			}

			return HTMLString.ToString();
		}

		public string GetLeagueGroup() {
			int iRowCount = 0;
			int iIndex = 0;
			string sLeagID;
			string sWordIndex = "";

			sLeagID = HttpContext.Current.Request.QueryString["leagID"];
			try {
				SQLString.Remove(0, SQLString.Length);
				if(sLeagID != null) {
					SQLString.Append("select LEAGNAME from LEAGINFO where LEAG_ID='");
					SQLString.Append(sLeagID);
					SQLString.Append("'");
				} else {
					SQLString.Append("select LEAGNAME from LEAGINFO order by LEAGNAME");
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
				SQLString.Append("select LEAG_ID, LEAGNAME from LEAGINFO where LEAGNAME like '");
				SQLString.Append(sWordIndex);
				SQLString.Append("%' order by LEAGNAME");
				m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
				while(m_SportsOleReader.Read()) {
					if(iIndex%10 == 0) {
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
						//HTMLString.Append("LeagueManager_MOD.aspx");
						HTMLString.Append("LeagueManager_MODFrame.aspx");
					} else {
						HTMLString.Append("LeagueManager_DEL.aspx");
					}
					HTMLString.Append("?leagID=");
					HTMLString.Append(m_SportsOleReader.GetString(0).Trim());
					HTMLString.Append("\" target=\"content_frame\">");
					HTMLString.Append(m_SportsOleReader.GetString(1).Trim());
					HTMLString.Append("</a></td>");
					if(iIndex%10 == 9) {
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
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " ModifyLeagueMenu.cs.GetLeagueGroup(): " + ex.ToString());
				m_SportsLog.Close();
				HTMLString.Remove(0, HTMLString.Length);
				HTMLString.Append((string)HttpContext.Current.Application["accessErrorMsg"]);
			}

			return HTMLString.ToString();
		}
	}
}