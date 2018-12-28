/*
Objective:
Menu of modify league information

Last updated:
28 Oct 2003 by Chapman

C#.NET complier statement:
csc /t:library /out:..\..\bin\BSKModifyLeagueMenu.dll /r:..\..\bin\DBManager.dll;..\..\bin\Files.dll BSKModifyLeagueMenu.cs
*/

using System;
using System.Data.OleDb;
using System.Reflection;
using System.Web;
using TDL.DB;
using TDL.IO;

[assembly:AssemblyCompany("TDSL")]
[assembly:AssemblyCopyright("(c) 2003 TDSL. All rights reserved. Created on 24 Oct 2003.")]
[assembly:AssemblyDescription("籃球資訊 -> 聯賽管理 -> 修改(聯賽選項)")]
[assembly:AssemblyProduct("Sports.NET")]
[assembly:AssemblyTitle("Sports.NET DLL")]
[assembly:AssemblyVersion("1.0.*")]
namespace SportsUtil {
	public class BSKModifyLeagueMenu {
		const string LOGFILESUFFIX = "log";
		OleDbDataReader m_SportsOleReader;
		DBManager m_SportsDBMgr;
		Files m_SportsLog;

		public BSKModifyLeagueMenu(string Connection) {
			m_SportsDBMgr = new DBManager();
			m_SportsDBMgr.ConnectionString = Connection;
			m_SportsLog = new Files();
		}

		public string GetLeague() {
			string getLeagueQuery, sRtn = "";

			try {
				sRtn = "<select name=\"leagID\" onchange=\"LeagueToModify(BSKModifyLeagueMenuForm.leagID.value)\"><option value=\"0\">請選擇</option>";
				//Only League related to match can be modified
				//Leagus related to Group/Personal Rank cannot be modified
				getLeagueQuery = "select CLEAG_ID, CLEAGUE from LEAGUE_INFO where CLEAGUETYPE='0' order by CLEAG_ID";
				m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(getLeagueQuery);
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
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " BSKModifyLeagueMenu.cs.GetLeague(): " + ex.ToString());
				m_SportsLog.Close();
				sRtn = (string)HttpContext.Current.Application["accessErrorMsg"];
			}

			return sRtn;
		}
	}
}