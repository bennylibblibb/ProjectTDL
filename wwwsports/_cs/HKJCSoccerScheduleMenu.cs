/*
Objective:
Create a combo box contains matches

Last updated:
24 Sep 2003 by Chapman

C#.NET complier statement:
csc /t:library /out:..\bin\HKJCSoccerScheduleMenu.dll /r:..\bin\DBManager.dll;..\bin\Files.dll HKJCSoccerScheduleMenu.cs
*/

using System;
using System.Data.OleDb;
using System.Reflection;
using System.Text;
using System.Web;
using TDL.DB;
using TDL.IO;

[assembly:AssemblyCompany("TDSL")]
[assembly:AssemblyCopyright("(c) 2003 TDSL. All rights reserved. Created on 8 August 2003.")]
[assembly:AssemblyDescription("JC足智彩 -> 賽事選項")]
[assembly:AssemblyProduct("Sports.NET")]
[assembly:AssemblyTitle("Sports.NET DLL")]
[assembly:AssemblyVersion("1.0.*")]
namespace SportsUtil {
	public class HKJCSoccerScheduleMenu {
		const string LOGFILESUFFIX = "log";
		DBManager m_SportsDBMgr;
		Files m_SportsLog;
		StringBuilder HTMLString;

		public HKJCSoccerScheduleMenu(string Connection) {
			m_SportsDBMgr = new DBManager();
			m_SportsDBMgr.ConnectionString = Connection;
			m_SportsLog = new Files();
			HTMLString = new StringBuilder();
		}

		public string Show() {
			string uid;
			string retrieveQuery;
			OleDbDataReader SportsOleReader;
			string[] weekArray;

			weekArray = (string[])HttpContext.Current.Application["WeekItems"];
			HTMLString.Remove(0,HTMLString.Length);
			uid = HttpContext.Current.Session["user_id"].ToString();
			//retrieveQuery = "select hkjc.IMATCH_CNT, hkjc.IWEEK, hkjc.ISEQ_NO, hkjc.CLEAGUEALIAS, hkjc.CHOST, hkjc.CGUEST, hkjc.MATCHDATETIME from HKJCSOCCER_INFO hkjc, leaginfo leag where hkjc.CLEAGUEALIAS=leag.alias and hkjc.CACT='U' and hkjc.CLEAGUEALIAS in (select distinct leaginfo.alias from leaginfo, userprofile_info where leaginfo.leag_id in (select cleag_id from userprofile_info where iuser_id=" + uid + ")) order by leag.leag_order, hkjc.MATCHDATETIME, hkjc.IWEEK, hkjc.ISEQ_NO";
			retrieveQuery = "select hkjc.IMATCH_CNT, hkjc.IWEEK, hkjc.ISEQ_NO, hkjc.CLEAGUEALIAS, hkjc.CHOST, hkjc.CGUEST, hkjc.MATCHDATETIME from HKJCSOCCER_INFO hkjc, leaginfo leag where hkjc.CLEAGUEALIAS=leag.alias and hkjc.CACT='U' and leaginfo.leag_id in (select cleag_id from userprofile_info where iuser_id=" + uid + ") order by leag.leag_order, hkjc.MATCHDATETIME, hkjc.IWEEK, hkjc.ISEQ_NO";
			try {
				SportsOleReader = m_SportsDBMgr.ExecuteQuery(retrieveQuery);
				while(SportsOleReader.Read()) {
					HTMLString.Append("<option value=\"");
					HTMLString.Append(SportsOleReader.GetInt32(0).ToString());
					HTMLString.Append("\">");
					HTMLString.Append(weekArray[SportsOleReader.GetInt32(1)]);
					HTMLString.Append(" ");
					HTMLString.Append(SportsOleReader.GetInt32(2).ToString());
					HTMLString.Append(" ");
					HTMLString.Append(SportsOleReader.GetString(3).Trim());
					HTMLString.Append("-");
					HTMLString.Append(SportsOleReader.GetString(4).Trim());
					HTMLString.Append(" vs ");
					HTMLString.Append(SportsOleReader.GetString(5).Trim());
					HTMLString.Append("</option>");
				}
				m_SportsDBMgr.Close();
				SportsOleReader.Close();
				m_SportsDBMgr.Dispose();
			} catch(Exception ex) {
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " HKJCSoccerScheduleMenu.cs.Show(): " + ex.ToString());
				m_SportsLog.Close();
				HTMLString.Remove(0,HTMLString.Length);
				HTMLString.Append((string)HttpContext.Current.Application["accessErrorMsg"]);
			}

			return HTMLString.ToString();
		}
	}
}