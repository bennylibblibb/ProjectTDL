/*
Objective:
Delete a league and its relationship with user profile and team

Last updated:
10 July 2004, Chapman Choi

C#.NET complier statement:
csc /t:library /out:..\bin\DelLeague.dll /r:..\bin\DBManager.dll;..\bin\Files.dll DelLeague.cs
*/

using System;
using System.Data.OleDb;
using System.Text;
using System.Reflection;
using System.Web;
using TDL.DB;
using TDL.IO;

[assembly:AssemblyCompany("TDSL")]
[assembly:AssemblyCopyright("(c) 2003 TDSL. All rights reserved. Created on 19 June 2003.")]
[assembly:AssemblyDescription("一般設定 -> 聯賽管理 -> 刪除")]
[assembly:AssemblyProduct("Sports.NET")]
[assembly:AssemblyTitle("Sports.NET DLL")]
[assembly:AssemblyVersion("1.0.*")]
namespace SportsUtil {
	public class DelLeague {
		const string LOGFILESUFFIX = "log";
		OleDbDataReader m_SportsOleReader;
		DBManager m_SportsDBMgr;
		Files m_SportsLog;
		StringBuilder SQLString;

		public DelLeague(string Connection) {
			m_SportsDBMgr = new DBManager();
			m_SportsDBMgr.ConnectionString = Connection;
			m_SportsLog = new Files();
			SQLString = new StringBuilder();
		}

		public string GetLeague() {
			int iHasTeam = 0;
			string sRtn;
			string sLeagID;

			sLeagID = HttpContext.Current.Request.QueryString["leagID"];
			try {
				sRtn = "決定刪除";
				m_SportsOleReader = m_SportsDBMgr.ExecuteQuery("select LEAGNAME from LEAGINFO where LEAG_ID='" + sLeagID + "'");
				if(m_SportsOleReader.Read()) {
					if(!m_SportsOleReader.IsDBNull(0)) {
						sRtn += "<font color=\"#F0F8FF\" size=\"5\">" + m_SportsOleReader.GetString(0).Trim() + "</font>";
						sRtn += "<input type=\"hidden\" name=\"leagID\" value=\"" + sLeagID + "\">";
						iHasTeam++;
					}
				}
				m_SportsOleReader.Close();
				m_SportsDBMgr.Close();
				m_SportsDBMgr.Dispose();
				sRtn += "？";
				if(iHasTeam == 0) sRtn = "請選擇其他聯賽";
			} catch(Exception ex) {
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " DelLeague.cs.GetLeague(): " + ex.ToString());
				m_SportsLog.Close();
				sRtn = (string)HttpContext.Current.Application["accessErrorMsg"];
			}

			return sRtn;
		}

		public int Delete() {
			int iRecUpd = 0;
			string sLeagID;
			string sLeague;

			sLeagID = HttpContext.Current.Request.Form["leagID"];
			try {
				SQLString.Remove(0,SQLString.Length);
				SQLString.Append("select LEAGNAME from LEAGINFO where LEAG_ID='");
				SQLString.Append(sLeagID);
				SQLString.Append("'");
				sLeague = m_SportsDBMgr.ExecuteQueryString(SQLString.ToString());
				m_SportsDBMgr.Close();

				//delete from userprofile_info w.r.t. league id / mark deleted id
				SQLString.Remove(0,SQLString.Length);
				SQLString.Append("delete from USERPROFILE_INFO where CLEAG_ID='");
				SQLString.Append(sLeagID);
				SQLString.Append("' OR CLEAG_ID='x");
				SQLString.Append(sLeagID);
				SQLString.Append("'");
				m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
				m_SportsDBMgr.Close();

				//delete from id_info (remove league-team relationship)
				SQLString.Remove(0,SQLString.Length);
				SQLString.Append("delete from ID_INFO where LEAG_ID='");
				SQLString.Append(sLeagID);
				SQLString.Append("'");
				m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
				m_SportsDBMgr.Close();

				//delete from leaginfo w.r.t. league id
				SQLString.Remove(0,SQLString.Length);
				SQLString.Append("delete from LEAGINFO where LEAG_ID='");
				SQLString.Append(sLeagID);
				SQLString.Append("'");
				iRecUpd = m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
				m_SportsDBMgr.Close();
				m_SportsDBMgr.Dispose();

				//write log
				m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " DelLeague.cs: delete league [" + sLeague + "] (" + HttpContext.Current.Session["user_name"] + ")");
				m_SportsLog.Close();
			} catch(Exception ex) {
				iRecUpd = -1;
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " DelLeague.cs.Delete(): " + ex.ToString());
				m_SportsLog.Close();
			}

			return iRecUpd;
		}
	}
}