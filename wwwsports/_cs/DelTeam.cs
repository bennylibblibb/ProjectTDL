/*
Objective:
Delete a team and its relationship with league

Last updated:
12 July 2004, Chapman Choi

C#.NET complier statement:
csc /t:library /out:..\bin\DelTeam.dll /r:..\bin\DBManager.dll;..\bin\Files.dll DelTeam.cs
*/

using System;
using System.Data.OleDb;
using System.Reflection;
using System.Web;
using TDL.DB;
using TDL.IO;

[assembly:AssemblyCompany("TDSL")]
[assembly:AssemblyCopyright("(c) 2003 TDSL. All rights reserved. Created on 19 June 2003.")]
[assembly:AssemblyDescription("一般設定 -> 隊伍管理 -> 刪除")]
[assembly:AssemblyProduct("Sports.NET")]
[assembly:AssemblyTitle("Sports.NET DLL")]
[assembly:AssemblyVersion("1.0.*")]
namespace SportsUtil {
	public class DelTeam {
		const string LOGFILESUFFIX = "log";
		OleDbDataReader m_SportsOleReader;
		DBManager m_SportsDBMgr;
		Files m_SportsLog;

		public DelTeam(string Connection) {
			m_SportsDBMgr = new DBManager();
			m_SportsDBMgr.ConnectionString = Connection;
			m_SportsLog = new Files();
		}

		public string GetTeam() {
			int iHasTeam = 0;
			string sRtn;
			string sTeamID;

			sTeamID = HttpContext.Current.Request.QueryString["teamID"];
			try {
				sRtn = "決定刪除";
				m_SportsOleReader = m_SportsDBMgr.ExecuteQuery("select TEAMNAME from TEAMINFO where TEAM_ID=" + sTeamID);
				if(m_SportsOleReader.Read()) {
					if(!m_SportsOleReader.IsDBNull(0)) {
						sRtn += "<font color=\"#FFFAF0\" size=\"5\">" + m_SportsOleReader.GetString(0).Trim() + "</font>";
						sRtn += "<input type=\"hidden\" name=\"teamID\" value=\"" + sTeamID + "\">";
						iHasTeam++;
					}
				}
				m_SportsOleReader.Close();
				m_SportsDBMgr.Close();
				sRtn += "？";
				if(iHasTeam == 0) sRtn = "請選擇其他隊伍";
			} catch(Exception ex) {
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " DelTeam.cs.GetTeam(): " + ex.ToString());
				m_SportsLog.Close();
				sRtn = (string)HttpContext.Current.Application["accessErrorMsg"];
			}

			return sRtn;
		}

		public int Delete() {
			int iRecUpd = 0;
			string sTeamID;
			string updateQuery;
			string sTeam;

			sTeamID = HttpContext.Current.Request.Form["teamID"].Trim();
			try {
				updateQuery = "select TEAMNAME from TEAMINFO where TEAM_ID=" + sTeamID;
				sTeam = m_SportsDBMgr.ExecuteQueryString(updateQuery);
				m_SportsDBMgr.Close();

				//delete team-league relationship from id_info
				updateQuery = "delete from ID_INFO where TEAM_ID=" + sTeamID;
				m_SportsDBMgr.ExecuteNonQuery(updateQuery);
				m_SportsDBMgr.Close();

				//delete from teaminfo
				updateQuery = "delete from TEAMINFO where TEAM_ID=" + sTeamID;
				iRecUpd = m_SportsDBMgr.ExecuteNonQuery(updateQuery);
				m_SportsDBMgr.Close();

				//write log
				m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " DelTeam.cs: delete team [" + sTeam + "] (" + HttpContext.Current.Session["user_name"] + ")");
				m_SportsLog.Close();
			}	catch(Exception ex) {
				iRecUpd = -1;
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " DelTeam.cs.GetTeam(): " + ex.ToString());
				m_SportsLog.Close();
			}

			return iRecUpd;
		}
	}
}