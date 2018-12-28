/*
Objective:
Backup or restore chart data

Last updated:
11 August 2003 by Chapman

C#.NET complier statement:
csc /t:library /out:..\bin\BackupChartData.dll /r:..\bin\DBManager.dll;..\bin\Files.dll BackupChartData.cs
*/

using System;
using System.Collections;
using System.Reflection;
using System.Web;
using TDL.DB;
using TDL.IO;

[assembly:AssemblyCompany("TDSL")]
[assembly:AssemblyCopyright("(c) 2003 TDSL. All rights reserved.")]
[assembly:AssemblyDescription("GOGO1 指數圖表 -> 備份還原")]
[assembly:AssemblyProduct("Sports.NET")]
[assembly:AssemblyTitle("Sports.NET DLL")]
[assembly:AssemblyVersion("1.0.*")]
namespace SportsUtil {
	public class BackupChartData {
		const string LOGFILESUFFIX = "log";
		DBManager m_SportsDBMgr;
		Files m_SportsLog;

		public BackupChartData(string Connection) {
			m_SportsDBMgr = new DBManager();
			m_SportsDBMgr.ConnectionString = Connection;
			m_SportsLog = new Files();
		}

		public string BackupRestore() {
			int iUpd = 0;
			string sAction;
			string updateQuery;

			try {
				sAction = HttpContext.Current.Request.Form["chartaction"];
				if(sAction == null) sAction = "0";

				if(sAction.Equals("1")) {		//Backup data
					updateQuery = "DELETE from oddshistorybak";
					m_SportsDBMgr.ExecuteNonQuery(updateQuery);
					updateQuery = "insert into oddshistorybak select * from oddshistory";
					iUpd = m_SportsDBMgr.ExecuteNonQuery(updateQuery);
					m_SportsDBMgr.Dispose();

					//write log
					m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
					m_SportsLog.SetFileName(0,LOGFILESUFFIX);
					m_SportsLog.Open();
					m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " BackupChartData.cs: Backup " + iUpd.ToString() + " chart data (" + HttpContext.Current.Session["user_name"] + ")");
					m_SportsLog.Close();
				} else if(sAction.Equals("2")) {		//Restore data
					updateQuery = "insert into oddshistory select * from oddshistorybak bkup where not exists (select distinct league, host, guest, ctimestamp from oddshistory live where bkup.league=live.league and bkup.host=live.host and bkup.guest=live.guest  and bkup.ctimestamp=live.ctimestamp)";
					iUpd = m_SportsDBMgr.ExecuteNonQuery(updateQuery);
					m_SportsDBMgr.Dispose();

					//write log
					m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
					m_SportsLog.SetFileName(0,LOGFILESUFFIX);
					m_SportsLog.Open();
					m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " BackupChartData.cs: Restore " + iUpd.ToString() + " chart data (" + HttpContext.Current.Session["user_name"] + ")");
					m_SportsLog.Close();
				}
			} catch(Exception ex) {
				sAction = "-1";
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " BackupChartData.cs.BackupRestore(): " + ex.ToString());
				m_SportsLog.Close();
			}

			return sAction;
		}
	}
}