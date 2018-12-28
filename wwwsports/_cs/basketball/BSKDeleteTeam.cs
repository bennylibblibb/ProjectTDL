/*
Objective:
Delete a team and its relationship with league

Last updated:
1 Nov 2003 by Chapman

C#.NET complier statement:
csc /t:library /out:..\..\bin\BSKDeleteTeam.dll /r:..\..\bin\DBManager.dll;..\..\bin\Files.dll BSKDeleteTeam.cs
*/

using System;
using System.Data.OleDb;
using System.Reflection;
using System.Web;
using TDL.DB;
using TDL.IO;

[assembly:AssemblyCompany("TDSL")]
[assembly:AssemblyCopyright("(c) 2003 TDSL. All rights reserved. Created on 24 Oct 2003.")]
[assembly:AssemblyDescription("籃球資訊 -> 隊伍管理 -> 刪除")]
[assembly:AssemblyProduct("Sports.NET")]
[assembly:AssemblyTitle("Sports.NET DLL")]
[assembly:AssemblyVersion("1.0.*")]
namespace SportsUtil {
	public class BSKDeleteTeam {
		const string LOGFILESUFFIX = "log";
		OleDbDataReader m_SportsOleReader;
		DBManager m_SportsDBMgr;
		Files m_SportsLog;

		public BSKDeleteTeam(string Connection) {
			m_SportsDBMgr = new DBManager();
			m_SportsDBMgr.ConnectionString = Connection;
			m_SportsLog = new Files();
		}

		public string GetTeam() {
			string sRtn;

			try {
				sRtn = "<select name=\"teamID\">";
				m_SportsOleReader = m_SportsDBMgr.ExecuteQuery("select CTEAM_ID, CTEAM from TEAM_INFO order by CTEAM");
				while(m_SportsOleReader.Read()) {
					//retrieve all teams from table
					sRtn += "<option value=\"" + m_SportsOleReader.GetString(0).Trim() + "\">";
					sRtn += m_SportsOleReader.GetString(1).Trim() + "</option>";
				}
				m_SportsOleReader.Close();
				m_SportsDBMgr.Close();
				m_SportsDBMgr.Dispose();
				sRtn += "</select>";
			} catch(Exception ex) {
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " BSKDeleteTeam.cs.GetTeam(): " + ex.ToString());
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
				updateQuery = "select CTEAM from TEAM_INFO where CTEAM_ID='" + sTeamID + "'";
				sTeam = m_SportsDBMgr.ExecuteQueryString(updateQuery);
				m_SportsDBMgr.Close();

				//delete player in this team
				updateQuery = "delete from PLAYERS_INFO where CTEAM_ID='" + sTeamID + "'";
				m_SportsDBMgr.ExecuteNonQuery(updateQuery);
				m_SportsDBMgr.Close();

				//delete team-league relationship from id_info
				updateQuery = "delete from IDMAP_INFO where CTEAM_ID='" + sTeamID + "'";
				m_SportsDBMgr.ExecuteNonQuery(updateQuery);
				m_SportsDBMgr.Close();

				//delete from teaminfo
				updateQuery = "delete from TEAM_INFO where CTEAM_ID='" + sTeamID + "'";
				iRecUpd = m_SportsDBMgr.ExecuteNonQuery(updateQuery);
				m_SportsDBMgr.Close();
				m_SportsDBMgr.Dispose();

				//write log
				m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " BSKDeleteTeam.cs: delete team <" + sTeam + "> (" + HttpContext.Current.Session["user_name"] + ")");
				m_SportsLog.Close();
			}	catch(Exception ex) {
				iRecUpd = -1;
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " BSKDeleteTeam.cs.GetTeam(): " + ex.ToString());
				m_SportsLog.Close();
			}

			return iRecUpd;
		}
	}
}