/*
Objective:
Create basketball menu

Last updated:
28 Apr 2004, Chapman Choi
28 Apr 2004, Chapman Choi, Allow user to modify result

C#.NET complier statement:
csc /t:library /out:..\..\bin\BSKMenu.dll /r:..\..\bin\DBManager.dll;..\..\bin\Files.dll BSKMenu.cs
*/

using System;
using System.Data.OleDb;
using System.Reflection;
using System.Text;
using System.Web;
using TDL.DB;
using TDL.IO;

[assembly:AssemblyCompany("TDSL")]
[assembly:AssemblyCopyright("(c) 2003 TDSL. All rights reserved. Created on 24 Oct 2003.")]
[assembly:AssemblyDescription("籃球資訊 -> 項目選項")]
[assembly:AssemblyProduct("Sports.NET")]
[assembly:AssemblyTitle("Sports.NET DLL")]
[assembly:AssemblyVersion("1.0.*")]
namespace SportsUtil {
	public class BSKMenu {
		const string LOGFILESUFFIX = "log";
		DBManager m_SportsDBMgr;
		Files m_SportsLog;
		StringBuilder HTMLString;
		OleDbDataReader m_SportsOleReader;

		public BSKMenu(string Connection) {
			m_SportsDBMgr = new DBManager();
			m_SportsDBMgr.ConnectionString = Connection;
			m_SportsLog = new Files();
			HTMLString = new StringBuilder();
		}

		public string GetMatchLeagues() {
			HTMLString.Remove(0,HTMLString.Length);
			try {
				m_SportsOleReader = m_SportsDBMgr.ExecuteQuery("select CLEAG_ID, CLEAGUE from LEAGUE_INFO where CLEAGUETYPE='0' order by CLEAG_ID");
				while(m_SportsOleReader.Read()) {
					HTMLString.Append("<option value=\"");
					HTMLString.Append(m_SportsOleReader.GetString(0).Trim());
					HTMLString.Append("\">");
					HTMLString.Append(m_SportsOleReader.GetString(1).Trim());
					HTMLString.Append("</option>");
				}
				m_SportsOleReader.Close();
				m_SportsDBMgr.Close();
			} catch(Exception ex) {
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " BSKMenu.cs.GetMatchLeagues(): " + ex.ToString());
				m_SportsLog.Close();
				HTMLString.Remove(0,HTMLString.Length);
				HTMLString.Append((string)HttpContext.Current.Application["accessErrorMsg"]);
			}

			return HTMLString.ToString();
		}

		public string GetGroupRankLeagues() {
			HTMLString.Remove(0,HTMLString.Length);
			try {
				m_SportsOleReader = m_SportsDBMgr.ExecuteQuery("select CLEAG_ID, CLEAGUE from LEAGUE_INFO where CLEAGUETYPE='1' order by CLEAG_ID");
				while(m_SportsOleReader.Read()) {
					HTMLString.Append("<option value=\"");
					HTMLString.Append(m_SportsOleReader.GetString(0).Trim());
					HTMLString.Append("\">");
					HTMLString.Append(m_SportsOleReader.GetString(1).Trim());
					HTMLString.Append("</option>");
				}
				m_SportsOleReader.Close();
				m_SportsDBMgr.Close();
			} catch(Exception ex) {
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " BSKMenu.cs.GetGroupRankLeagues(): " + ex.ToString());
				m_SportsLog.Close();
				HTMLString.Remove(0,HTMLString.Length);
				HTMLString.Append((string)HttpContext.Current.Application["accessErrorMsg"]);
			}

			return HTMLString.ToString();
		}

		public string GetPersonalRankLeagues() {
			HTMLString.Remove(0,HTMLString.Length);
			try {
				m_SportsOleReader = m_SportsDBMgr.ExecuteQuery("select CLEAG_ID, CLEAGUE from LEAGUE_INFO where CLEAGUETYPE='2' order by CLEAG_ID");
				while(m_SportsOleReader.Read()) {
					HTMLString.Append("<option value=\"");
					HTMLString.Append(m_SportsOleReader.GetString(0).Trim());
					HTMLString.Append("\">");
					HTMLString.Append(m_SportsOleReader.GetString(1).Trim());
					HTMLString.Append("</option>");
				}
				m_SportsOleReader.Close();
				m_SportsDBMgr.Close();
			} catch(Exception ex) {
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " BSKMenu.cs.GetPersonalRankLeagues(): " + ex.ToString());
				m_SportsLog.Close();
				HTMLString.Remove(0,HTMLString.Length);
				HTMLString.Append((string)HttpContext.Current.Application["accessErrorMsg"]);
			}

			return HTMLString.ToString();
		}

		public string GetTeams() {
			HTMLString.Remove(0,HTMLString.Length);
			try {
				m_SportsOleReader = m_SportsDBMgr.ExecuteQuery("select CTEAM_ID, CTEAM from TEAM_INFO order by CTEAM");
				while(m_SportsOleReader.Read()) {
					HTMLString.Append("<option value=\"");
					HTMLString.Append(m_SportsOleReader.GetString(0).Trim());
					HTMLString.Append("\">");
					HTMLString.Append(m_SportsOleReader.GetString(1).Trim());
					HTMLString.Append("</option>");
				}
				m_SportsOleReader.Close();
				m_SportsDBMgr.Close();
			} catch(Exception ex) {
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " BSKMenu.cs.GetTeams(): " + ex.ToString());
				m_SportsLog.Close();
				HTMLString.Remove(0,HTMLString.Length);
				HTMLString.Append((string)HttpContext.Current.Application["accessErrorMsg"]);
			}

			return HTMLString.ToString();
		}

		public string GetMatches() {
			string sAlias = "";

			HTMLString.Remove(0,HTMLString.Length);
			try {
				m_SportsOleReader = m_SportsDBMgr.ExecuteQuery("select CLEAG, CHOST, CGUEST, IMATCH_COUNT from NBAGAME_INFO where CACTION='U' order by ISEQ_NO, IMATCH_COUNT");
				while(m_SportsOleReader.Read()) {
					if(!m_SportsOleReader.IsDBNull(0)) {
						if(!sAlias.Equals(m_SportsOleReader.GetString(0).Trim())) {
							sAlias = m_SportsOleReader.GetString(0).Trim();
							HTMLString.Append("<option value=\"\">");
							HTMLString.Append(sAlias);
							HTMLString.Append("</option>");
						}
						HTMLString.Append("<option value=\"");
						HTMLString.Append(m_SportsOleReader.GetInt32(3).ToString());
						HTMLString.Append("\">");
						HTMLString.Append(m_SportsOleReader.GetString(1).Trim());
						HTMLString.Append(" vs ");
						HTMLString.Append(m_SportsOleReader.GetString(2).Trim());
						HTMLString.Append("</option>");
					}
				}
				m_SportsOleReader.Close();
				m_SportsDBMgr.Close();
			} catch(Exception ex) {
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " BSKMenu.cs.GetMatches(): " + ex.ToString());
				m_SportsLog.Close();
				HTMLString.Remove(0,HTMLString.Length);
				HTMLString.Append((string)HttpContext.Current.Application["accessErrorMsg"]);
			}

			return HTMLString.ToString();
		}

		public string GetResultMatch() {
			string sLeague = "";

			HTMLString.Remove(0,HTMLString.Length);
			try {
				m_SportsOleReader = m_SportsDBMgr.ExecuteQuery("select IMATCHNO, CLEAGUE, CHOST, CGUEST from HKMAIN_DETAILS where CHOST<>'9999' or CGUEST<>'9999' order by IMATCHNO");
				while(m_SportsOleReader.Read()) {
					if(!sLeague.Equals(m_SportsOleReader.GetString(1).Trim())) {
						sLeague = m_SportsOleReader.GetString(1).Trim();
						HTMLString.Append("<option value=\"\">");
						HTMLString.Append(sLeague);
						HTMLString.Append("</option>");
					}
					HTMLString.Append("<option value=\"");
					HTMLString.Append(m_SportsOleReader.GetInt32(0).ToString());
					HTMLString.Append("\">");
					HTMLString.Append(m_SportsOleReader.GetString(2).Trim());
					HTMLString.Append(" vs ");
					HTMLString.Append(m_SportsOleReader.GetString(3).Trim());
					HTMLString.Append("</option>");
				}
				m_SportsOleReader.Close();
				m_SportsDBMgr.Close();
			} catch(Exception ex) {
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " BSKMenu.cs.GetResultMatch(): " + ex.ToString());
				m_SportsLog.Close();
				HTMLString.Remove(0,HTMLString.Length);
				HTMLString.Append((string)HttpContext.Current.Application["accessErrorMsg"]);
			}

			return HTMLString.ToString();
		}

	}
}