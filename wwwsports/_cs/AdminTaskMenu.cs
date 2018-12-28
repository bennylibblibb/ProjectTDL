/*
Objective:
Create a combo box contains services

Last updated:
8 Sep 2003 by Chapman

C#.NET complier statement:
csc /t:library /out:..\bin\AdminTaskMenu.dll /r:..\bin\DBManager.dll;..\bin\Files.dll AdminTaskMenu.cs
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
[assembly:AssemblyDescription("管理項目 -> 重發/刪除資訊 -> 選單")]
[assembly:AssemblyProduct("Sports.NET")]
[assembly:AssemblyTitle("Sports.NET DLL")]
[assembly:AssemblyVersion("1.0.*")]
namespace SportsUtil {
	public class AdminTaskMenu {
		const string LOGFILESUFFIX = "log";
		DBManager m_SportsDBMgr;
		Files m_SportsLog;
		StringBuilder HTMLString;

		public AdminTaskMenu(string Connection) {
			m_SportsDBMgr = new DBManager();
			m_SportsDBMgr.ConnectionString = Connection;
			m_SportsLog = new Files();
			HTMLString = new StringBuilder();
		}

		public string Show() {
			string uid;
			string retrieveQuery;
			OleDbDataReader SportsOleReader;

			HTMLString.Remove(0,HTMLString.Length);		//reset StringBuilder
			uid = HttpContext.Current.Session["user_id"].ToString();
			retrieveQuery = "select ISVC_NO, CSVC_NAME from SVCCFG where CACCESS_RIGHT<=(select ACCESS_RIGHT from LOGININFO where USER_ID=" + uid + ") and DEL_TYPE='P' order by CSVC_NAME, ISVC_NO";
			try {
				SportsOleReader = m_SportsDBMgr.ExecuteQuery(retrieveQuery);
				while(SportsOleReader.Read()) {
					HTMLString.Append("<option value=\"");
					HTMLString.Append(SportsOleReader.GetInt32(0).ToString());
					HTMLString.Append("\">");
					HTMLString.Append(SportsOleReader.GetString(1).Trim());
					HTMLString.Append("</option>");
				}
				m_SportsDBMgr.Close();
				SportsOleReader.Close();
				m_SportsDBMgr.Dispose();

				if(HTMLString.Length == 0) {
					HTMLString.Append("<option value=\"0\">沒有權限</option>");
				}
			} catch(Exception ex) {
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " AdminTaskMenu.cs.Show(): " + ex.ToString());
				m_SportsLog.Close();
				HTMLString.Remove(0,HTMLString.Length);
				HTMLString.Append((string)HttpContext.Current.Application["accessErrorMsg"]);
			}

			return HTMLString.ToString();
		}
		
		public string WebShow() {
			string uid;
			string retrieveQuery;
			OleDbDataReader SportsOleReader;

			HTMLString.Remove(0,HTMLString.Length);		//reset StringBuilder
			uid = HttpContext.Current.Session["user_id"].ToString();
			retrieveQuery = "select ISVC_NO, CSVC_NAME from SVCCFG where CACCESS_RIGHT<=(select ACCESS_RIGHT from LOGININFO where USER_ID=" + uid + ") and DEL_TYPE='W' order by CSVC_NAME, ISVC_NO";
			try {
				SportsOleReader = m_SportsDBMgr.ExecuteQuery(retrieveQuery);
				while(SportsOleReader.Read()) {
					HTMLString.Append("<option value=\"");
					HTMLString.Append(SportsOleReader.GetInt32(0).ToString());
					HTMLString.Append("\">");
					HTMLString.Append(SportsOleReader.GetString(1).Trim());
					HTMLString.Append("</option>");
				}
				m_SportsDBMgr.Close();
				SportsOleReader.Close();
				m_SportsDBMgr.Dispose();

				if(HTMLString.Length == 0) {
					HTMLString.Append("<option value=\"0\">沒有權限</option>");
				}
			} catch(Exception ex) {
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " AdminTaskMenu.cs.WebShow(): " + ex.ToString());
				m_SportsLog.Close();
				HTMLString.Remove(0,HTMLString.Length);
				HTMLString.Append((string)HttpContext.Current.Application["accessErrorMsg"]);
			}

			return HTMLString.ToString();
		}
	}
}