/*
Objective:
Modify league information

Last updated:
28 Oct 2003 by Chapman

C#.NET complier statement:
csc /t:library /out:..\..\bin\BSKModifyLeague.dll /r:..\..\bin\DBManager.dll;..\..\bin\Files.dll BSKModifyLeague.cs
*/

using System;
using System.Data.OleDb;
using System.Reflection;
using System.Web;
using TDL.DB;
using TDL.IO;

[assembly:AssemblyCompany("TDSL")]
[assembly:AssemblyCopyright("(c) 2003 TDSL. All rights reserved. Created on 24 Oct 2003.")]
[assembly:AssemblyDescription("籃球資訊 -> 聯賽管理 -> 修改")]
[assembly:AssemblyProduct("Sports.NET")]
[assembly:AssemblyTitle("Sports.NET DLL")]
[assembly:AssemblyVersion("1.0.*")]
namespace SportsUtil {
	public class BSKModifyLeague {
		const string LOGFILESUFFIX = "log";
		OleDbDataReader m_SportsOleReader;
		DBManager m_SportsDBMgr;
		Files m_SportsLog;

		public BSKModifyLeague(string Connection) {
			m_SportsDBMgr = new DBManager();
			m_SportsDBMgr.ConnectionString = Connection;
			m_SportsLog = new Files();
		}

		public string GetLeagues() {
			string sRtn;
			string sLeagID;
			string sLeague = "";
			string sAlias = "";
			string sLeagType = "";

			sLeagID = HttpContext.Current.Request.QueryString["leagID"].Trim();
			try {
				m_SportsOleReader = m_SportsDBMgr.ExecuteQuery("select CLEAGUE, CALIAS, CLEAGUETYPE from LEAGUE_INFO where CLEAG_ID='" + sLeagID + "'");
				if(m_SportsOleReader.Read()) {
					sLeague = m_SportsOleReader.GetString(0).Trim();
					if(!m_SportsOleReader.IsDBNull(1)) sAlias = m_SportsOleReader.GetString(1).Trim();
					if(!m_SportsOleReader.IsDBNull(2)) sLeagType = m_SportsOleReader.GetString(2).Trim();
				}
				m_SportsOleReader.Close();
				m_SportsDBMgr.Close();

				sRtn = "<tr><th>聯賽全名:</th><td align=\"left\">" + sLeague + "</td></tr>";
				sRtn += "<tr><th>聯賽簡稱:</th><td align=\"left\"><input name=\"alias\" value=\"" + sAlias + "\"></td></tr>";
				sRtn += "<tr><th>聯賽類別:</th><td><select name=\"leaguetype\">";
				//User cannot modify type of league dynamically
				if(sLeagType.Equals("0")) {
					sRtn += "<option value=\"0\">賽事聯賽</option>";
					//sRtn += "<option value=\"1\">小組排名</option>";
					//sRtn += "<option value=\"2\">個人統計</option>";
				} else if(sLeagType.Equals("1")) {
					//sRtn += "<option value=\"1\">小組排名</option>";
					sRtn += "<option value=\"0\">賽事聯賽</option>";
					//sRtn += "<option value=\"2\">個人統計</option>";
				} else {
					//sRtn += "<option value=\"2\">個人統計</option>";
					sRtn += "<option value=\"0\">賽事聯賽</option>";
					//sRtn += "<option value=\"1\">小組排名</option>";
				}
				sRtn += "</select></td></tr>";
				sRtn += "<input type=\"hidden\" name=\"leagID\" value=\"" + sLeagID + "\">";
			} catch(Exception ex) {
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " BSKModifyLeague.cs.GetLeagues(): " + ex.ToString());
				m_SportsLog.Close();
				sRtn = (string)HttpContext.Current.Application["accessErrorMsg"];
			}

			return sRtn;
		}

		public int Modify() {
			int iRecUpd = 0;
			string sType;
			string sLeagID;
			string sAlias;

			sAlias = HttpContext.Current.Request.Form["alias"].Trim();
			sLeagID = HttpContext.Current.Request.Form["leagID"];
			sType = HttpContext.Current.Request.Form["leaguetype"];

			try {
				//modify the type of league
				iRecUpd = m_SportsDBMgr.ExecuteNonQuery("update LEAGUE_INFO set CALIAS='" + sAlias + "', CLEAGUETYPE='" + sType + "' where CLEAG_ID='" + sLeagID + "'");
				m_SportsDBMgr.Close();
				m_SportsDBMgr.Dispose();

				//write log
				m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " BSKModifyLeague.cs: modify league <" + sAlias + "> (" + HttpContext.Current.Session["user_name"] + ")");
				m_SportsLog.Close();
			}	catch(Exception ex) {
				iRecUpd = -1;
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " BSKModifyLeague.cs.Modify(): " + ex.ToString());
				m_SportsLog.Close();
			}

			return iRecUpd;
		}
	}
}