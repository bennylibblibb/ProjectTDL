/*
Objective:
Modify league information

Last updated:
07 April 2005 (Chris) Write MSMQ or Remoting to notify MessageDispatcher about league name modification

C#.NET complier statement:
csc /t:library /out:..\bin\HKJCModifyName.dll /r:..\bin\DBManager.dll;..\bin\Files.dll;..\bin\MessageClient.dll;..\bin\SportsMessage.dll HKJCModifyName.cs
for Debugging
csc /debug:full /t:library /out:..\bin\HKJCModifyName.dll /r:..\bin\DBManager.dll;..\bin\Files.dll;..\bin\MessageClient.dll;..\bin\SportsMessage.dll HKJCModifyName.cs
*/

using System;
using System.Data.OleDb; // for OleDbDataReader
using System.Text;
using System.Reflection;
using System.Web;
using TDL.DB; // for DBManager
using TDL.IO; // for Files
using TDL.Message;

[assembly:AssemblyCompany("TDSL")]
[assembly:AssemblyCopyright("(c) 2003 TDSL. All rights reserved. Created on 19 June 2003.")]
[assembly:AssemblyDescription("馬會機設定 -> 馬會機賽事")]
[assembly:AssemblyProduct("Sports.NET")]
[assembly:AssemblyTitle("Sports.NET DLL")]
[assembly:AssemblyVersion("1.0.*")]
namespace SportsUtil {
	public class HKJCModifyName {
		const string LOGFILESUFFIX = "log";
		Files m_SportsLog;

		public HKJCModifyName() {
			m_SportsLog = new Files();
		}

		public string GetLeaguesName() {
			int iRecordIdx = 1;
			string sJCLeague = "";
			string sHost = "";
			string sGuest = "";

			DBManager m_SportsDBMgr;
			OleDbDataReader SportsOleReader;
			StringBuilder HTMLString =  new StringBuilder();
			try {
				m_SportsDBMgr = new DBManager();
				m_SportsDBMgr.ConnectionString = HttpContext.Current.Application["HKJCSOCConnectionString"].ToString();
				SportsOleReader = m_SportsDBMgr.ExecuteQuery("select CJCLEAGUE, CHOST, CGUEST from SPORTS_MASTER order by CJCLEAGUE");
				while (SportsOleReader.Read()) {
					if (iRecordIdx == 1) {
						sJCLeague = SportsOleReader.GetString(0).Trim();
						sHost = SportsOleReader.GetString(1).Trim();
						sGuest = SportsOleReader.GetString(2).Trim();
						HTMLString.Append("<tr><td>");
						HTMLString.Append(sJCLeague);
						HTMLString.Append("</td><td>");
						HTMLString.Append(sHost);
						HTMLString.Append("</td><td>");
						HTMLString.Append(sGuest);
						HTMLString.Append("</td></tr>");
					}
				}
				SportsOleReader.Close();
				m_SportsDBMgr.Close();
			}
			 catch(Exception ex) {
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " HKJCModifyName.cs.GetLeaguesName(): " + ex.ToString());
				m_SportsLog.Close();
				HTMLString.Remove(0,HTMLString.Length);
				HTMLString.Append((string)HttpContext.Current.Application["accessErrorMsg"]);
			}
			return HTMLString.ToString();
		}
		
		public int ModifyName() {
			int iRecUpd = 0;
			int iRecUpdHost = 0;
			int iRecUpdGuest = 0;
			string sOldTeam = "";
			string sNewTeam = "";
			StringBuilder SQLString = new StringBuilder();
			
			DBManager m_SportsDBMgr;
			try {
				m_SportsDBMgr = new DBManager();
				m_SportsDBMgr.ConnectionString = HttpContext.Current.Application["HKJCSOCConnectionString"].ToString();
				sNewTeam = HttpContext.Current.Request.Form["newTeam"].Trim();
				sOldTeam = HttpContext.Current.Request.Form["oldTeam"].Trim();
				
				if (CheckHostName() > 0) {									
					SQLString.Append("update SPORTS_MASTER set CHOST='");
					SQLString.Append(sNewTeam);
					SQLString.Append("' where CHOST='");
					SQLString.Append(sOldTeam);
					SQLString.Append("'");
					iRecUpdHost = m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
					m_SportsDBMgr.Close();
					
					//write log
					m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
					m_SportsLog.SetFileName(0,LOGFILESUFFIX);
					m_SportsLog.Open();
					m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " HKJCModifyName.cs: " + SQLString.ToString() + " (" + HttpContext.Current.Session["user_name"] + ")");
					m_SportsLog.Close();
					SQLString.Remove(0,SQLString.Length);
					iRecUpd += iRecUpdHost;
				}
				if (CheckGuestName() > 0) {	
					SQLString.Append("update SPORTS_MASTER set CGUEST='");
					SQLString.Append(sNewTeam);
					SQLString.Append("' where CGUEST='");
					SQLString.Append(sOldTeam);
					SQLString.Append("'");
					iRecUpdGuest = m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
					m_SportsDBMgr.Close();
					
					//write log
					m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
					m_SportsLog.SetFileName(0,LOGFILESUFFIX);
					m_SportsLog.Open();
					m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " HKJCModifyName.cs: " + SQLString.ToString() + " (" + HttpContext.Current.Session["user_name"] + ")");
					SQLString.Remove(0,SQLString.Length);
					m_SportsLog.Close();
					iRecUpd += iRecUpdGuest;
				}
			}
			catch (Exception ex) {
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " HKJCModifyName.cs.ModifyName(): " + ex.ToString());
				m_SportsLog.Close();
			}
			return iRecUpd;
		}
		
		public int CheckHostName(){
			int iRecCnt=0;
			string sOldTeam = "";
			StringBuilder SQLString = new StringBuilder();
			
			DBManager m_SportsDBMgr;
			try {
				m_SportsDBMgr = new DBManager();
				m_SportsDBMgr.ConnectionString = HttpContext.Current.Application["HKJCSOCConnectionString"].ToString();
				sOldTeam = HttpContext.Current.Request.Form["oldTeam"].Trim();

				//check team exist or not in sports_master table
				SQLString.Remove(0,SQLString.Length);
				SQLString.Append("select count(*) from SPORTS_MASTER where CHOST='" +sOldTeam+ "'");
				iRecCnt = m_SportsDBMgr.ExecuteScalar(SQLString.ToString());
				SQLString.Remove(0,SQLString.Length);
				
				m_SportsDBMgr.Close();

			}	catch(Exception ex) {
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " HKJCModifyName.cs.CheckHostName(): " + ex.ToString());
				m_SportsLog.Close();
			}
			return iRecCnt;
		}
		
		public int CheckGuestName(){
			int iRecCnt=0;
			string sOldTeam = "";
			StringBuilder SQLString = new StringBuilder();
			
			DBManager m_SportsDBMgr;
			try {
				m_SportsDBMgr = new DBManager();
				m_SportsDBMgr.ConnectionString = HttpContext.Current.Application["HKJCSOCConnectionString"].ToString();
				sOldTeam = HttpContext.Current.Request.Form["oldTeam"].Trim();

				//check team exist or not in sports_master table
				SQLString.Remove(0,SQLString.Length);
				SQLString.Append("select count(*) from SPORTS_MASTER where CGUEST='" +sOldTeam+ "'");
				iRecCnt = m_SportsDBMgr.ExecuteScalar(SQLString.ToString());
				SQLString.Remove(0,SQLString.Length);
				
				m_SportsDBMgr.Close();

			}	catch(Exception ex) {
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " HKJCModifyName.cs.CheckGuestName(): " + ex.ToString());
				m_SportsLog.Close();
			}
			return iRecCnt;
		}
	}
}
