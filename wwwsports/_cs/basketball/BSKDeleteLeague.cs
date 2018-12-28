/*
Objective:
Delete a league and its relationship with team

Last updated:
28 Oct 2003 by Chapman

C#.NET complier statement:
csc /t:library /out:..\..\bin\BSKDeleteLeague.dll /r:..\..\bin\DBManager.dll;..\..\bin\Files.dll BSKDeleteLeague.cs
*/

using System;
using System.Data.OleDb;
using System.Reflection;
using System.Web;
using TDL.DB;
using TDL.IO;

[assembly:AssemblyCompany("TDSL")]
[assembly:AssemblyCopyright("(c) 2003 TDSL. All rights reserved. Created on 24 Oct 2003.")]
[assembly:AssemblyDescription("籃球資訊 -> 聯賽管理 -> 刪除")]
[assembly:AssemblyProduct("Sports.NET")]
[assembly:AssemblyTitle("Sports.NET DLL")]
[assembly:AssemblyVersion("1.0.*")]
namespace SportsUtil {
	public class BSKDeleteLeague {
		const string LOGFILESUFFIX = "log";
		OleDbDataReader m_SportsOleReader;
		DBManager m_SportsDBMgr;
		Files m_SportsLog;

		public BSKDeleteLeague(string Connection) {
			m_SportsDBMgr = new DBManager();
			m_SportsDBMgr.ConnectionString = Connection;
			m_SportsLog = new Files();
		}

		public string GetLeague() {
			string sRtn;

			try {
				sRtn = "<th>聯賽全名:</th><td><select name=\"leagueToDelete\"><option value=\"0\">請選擇聯賽</option>";
				//User cannot delete League related to Group/Personal Rank
				m_SportsOleReader = m_SportsDBMgr.ExecuteQuery("select CLEAG_ID, CLEAGUE from LEAGUE_INFO where CLEAGUETYPE='0' order by CLEAG_ID");
				while(m_SportsOleReader.Read()) {
					sRtn += "<option value=\"" + m_SportsOleReader.GetString(0).Trim() + "\">" + m_SportsOleReader.GetString(1).Trim() + "</option>";
				}
				m_SportsOleReader.Close();
				m_SportsDBMgr.Close();
				m_SportsDBMgr.Dispose();
				sRtn += "</select></td>";
			} catch(Exception ex) {
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " BSKDeleteLeague.cs.GetLeague(): " + ex.ToString());
				m_SportsLog.Close();
				sRtn = (string)HttpContext.Current.Application["accessErrorMsg"];
			}

			return sRtn;
		}

		public int Delete() {
			int iRecUpd = 0;
			string updateQuery;
			string sLeagID;
			string sLeague;

			sLeagID = HttpContext.Current.Request.Form["leagueToDelete"];
			try {
				updateQuery = "select CLEAGUE from LEAGUE_INFO where CLEAG_ID='" + sLeagID + "'";
				sLeague = m_SportsDBMgr.ExecuteQueryString(updateQuery);
				m_SportsDBMgr.Close();

				//delete from id_info (remove league-team relationship)
				updateQuery = "delete from IDMAP_INFO where CLEAG_ID='" + sLeagID + "'";
				m_SportsDBMgr.ExecuteNonQuery(updateQuery);
				m_SportsDBMgr.Close();

				//delete from leaginfo w.r.t. league id
				updateQuery = "delete from LEAGUE_INFO where CLEAG_ID='" + sLeagID + "'";
				iRecUpd = m_SportsDBMgr.ExecuteNonQuery(updateQuery);
				m_SportsDBMgr.Close();
				m_SportsDBMgr.Dispose();

				//write log
				m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " BSKDeleteLeague.cs: delete league <" + sLeague + "> (" + HttpContext.Current.Session["user_name"] + ")");
				m_SportsLog.Close();
			} catch(Exception ex) {
				iRecUpd = -1;
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " BSKDeleteLeague.cs.Delete(): " + ex.ToString());
				m_SportsLog.Close();
			}

			return iRecUpd;
		}
	}
}