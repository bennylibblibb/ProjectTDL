/*
Objective:
FTP Error Checking

C#.NET complier statement:
csc /t:library /out:..\bin\FTPCheckMenu.dll /r:..\bin\DBManager.dll;..\bin\Files.dll FTPCheckMenu.cs
*/

using System;
using System.Data.OleDb;
using System.Reflection;
using System.Text;
using System.Web;
using TDL.DB;
using TDL.IO;

[assembly:AssemblyCompany("TDSL")]
[assembly:AssemblyCopyright("(c) 2003 TDSL. All rights reserved.")]
[assembly:AssemblyDescription("FTP ¿À¨d")]
[assembly:AssemblyProduct("Sports.NET")]
[assembly:AssemblyTitle("Sports.NET DLL")]
[assembly:AssemblyVersion("1.0.*")]
namespace SportsUtil {
	public class FTPCheckMenu {
		const string LOGFILESUFFIX = "log";
		DBManager m_SportsDBMgr;
		Files m_SportsLog;
		StringBuilder HTMLString;

		public FTPCheckMenu(string Connection) {
			m_SportsDBMgr = new DBManager();
			m_SportsDBMgr.ConnectionString = Connection;
			m_SportsLog = new Files();
			HTMLString = new StringBuilder();
		}

		public string Show() {
			OleDbDataReader SportsOleReader;

			try {
				SportsOleReader = m_SportsDBMgr.ExecuteQuery("select distinct ILINK_NO, CLINK_NAME from FTP_CHECK_MENU where CLINK_TYPE='FTP' and CLINK_ENABLE='1' order by ILINK_NO");
				while(SportsOleReader.Read()) {
					HTMLString.Append("<option value=\"");
					HTMLString.Append(SportsOleReader.GetInt32(0).ToString());
					HTMLString.Append("\">");
					HTMLString.Append(SportsOleReader.GetString(1).Trim());
					HTMLString.Append("</option>");
				}
				SportsOleReader.Close();
				m_SportsDBMgr.Close();
				m_SportsDBMgr.Dispose();
			} catch(Exception ex) {
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " SoccerMenuLeague.cs.Show(): " + ex.ToString());
				m_SportsLog.Close();
				HTMLString.Remove(0,HTMLString.Length);
				HTMLString.Append((string)HttpContext.Current.Application["accessErrorMsg"]);
			}

			return HTMLString.ToString();
		}
	}
}