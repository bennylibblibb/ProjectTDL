/*
Objective:
Menu of team modify

Last updated:
24 Oct 2003 by Chapman

C#.NET complier statement:
csc /t:library /out:..\..\bin\BSKModifyTeamMenu.dll /r:..\..\bin\DBManager.dll;..\..\bin\Files.dll BSKModifyTeamMenu.cs
*/

using System;
using System.Data.OleDb;
using System.Reflection;
using System.Web;
using TDL.DB;
using TDL.IO;

[assembly:AssemblyCompany("TDSL")]
[assembly:AssemblyCopyright("(c) 2003 TDSL. All rights reserved. Created on 24 Oct 2003.")]
[assembly:AssemblyDescription("籃球資訊 -> 隊伍管理 -> 修改(隊伍選項)")]
[assembly:AssemblyProduct("Sports.NET")]
[assembly:AssemblyTitle("Sports.NET DLL")]
[assembly:AssemblyVersion("1.0.*")]
namespace SportsUtil {
	public class BSKModifyTeamMenu {
		const string LOGFILESUFFIX = "log";
		OleDbDataReader m_SportsOleReader;
		DBManager m_SportsDBMgr;
		Files m_SportsLog;

		public BSKModifyTeamMenu(string Connection) {
			m_SportsDBMgr = new DBManager();
			m_SportsDBMgr.ConnectionString = Connection;
			m_SportsLog = new Files();
		}

		public string GetTeams() {
			string sRtn;

			try {
				sRtn = "<select name=\"teamID\" onchange=\"TeamToModify(BSKModifyTeamMenuForm.teamID.value)\"><option value=\"0\">請選擇</option>";
				//retrieve all teams from table
				m_SportsOleReader = m_SportsDBMgr.ExecuteQuery("select CTEAM_ID, CTEAM from TEAM_INFO order by CTEAM");
				while(m_SportsOleReader.Read()) {
					sRtn += "<option value=\"" + m_SportsOleReader.GetString(0).Trim() + "\">";
					sRtn += m_SportsOleReader.GetString(1).Trim() + "</option>";
				}
				m_SportsOleReader.Close();
				m_SportsDBMgr.Close();
				m_SportsDBMgr.Dispose();
				sRtn += "</select>";
			}	catch(Exception ex) {
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " BSKModifyTeamMenu.cs.GetTeams(): " + ex.ToString());
				m_SportsLog.Close();
				sRtn = (string)HttpContext.Current.Application["accessErrorMsg"];
			}

			return sRtn;
		}
	}
}