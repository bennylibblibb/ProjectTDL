/*
Objective:
Retrieve hidden match according to app type

Last updated:
7 Oct 2004, Fanny Cheung

C#.NET complier statement:
csc /t:library /out:..\bin\OtherSoccerHiddenMatch.dll /r:..\bin\DBManager.dll;..\bin\Files.dll OtherSoccerHiddenMatch.cs
*/

using System;
using System.Data.OleDb;
using System.Text;
using System.Reflection;
using System.Web;
using TDL.DB;
using TDL.IO;

[assembly:AssemblyCompany("TDSL")]
[assembly:AssemblyCopyright("(c) 2004 TDSL. All rights reserved. Creadted on 3 May 2004")]
[assembly:AssemblyDescription("足球2資訊 -> 顯示已隱藏的賽事")]
[assembly:AssemblyProduct("Sports.NET")]
[assembly:AssemblyTitle("Sports.NET DLL")]
[assembly:AssemblyVersion("3.1.*")]
namespace SportsUtil {
	public class OtherSoccerHiddenMatch {
		const string LOGFILESUFFIX = "log";
		OleDbDataReader m_SportsOleReader;
		DBManager m_SportsDBMgr;
		Files m_SportsLog;
		StringBuilder SQLString;

		public OtherSoccerHiddenMatch(string Connection) {
			m_SportsDBMgr = new DBManager();
			m_SportsDBMgr.ConnectionString = Connection;
			m_SportsLog = new Files();
			SQLString = new StringBuilder();
		}

		public string Show() {
			string sType;
			StringBuilder HTMLString = new StringBuilder();

			sType = HttpContext.Current.Request.QueryString["type"];

			try {
				switch(sType) {
					case "odds":
						SQLString.Remove(0,SQLString.Length);
						SQLString.Append("select othersoccerinfo.LEAGUE, othersoccerinfo.HOST, othersoccerinfo.GUEST from othersoccerinfo, leaginfo where othersoccerinfo.WEBACT='X' and othersoccerinfo.ACT='U' and othersoccerinfo.league = leaginfo.alias order by leaginfo.leag_order");
						m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
						while(m_SportsOleReader.Read()) {
							HTMLString.Append("<tr align=\"center\"><td>");
							HTMLString.Append(m_SportsOleReader.GetString(0).Trim());
							HTMLString.Append("</td><td>");
							HTMLString.Append(m_SportsOleReader.GetString(1).Trim());
							HTMLString.Append("</td><td>");
							HTMLString.Append(m_SportsOleReader.GetString(2).Trim());
							HTMLString.Append("</td></tr>");
						}
						m_SportsOleReader.Close();
						m_SportsDBMgr.Close();
						break;
					case "goal":
						SQLString.Remove(0,SQLString.Length);
						SQLString.Append("select game.LEAGUE, game.HOST, game.GUEST from othersoccerinfo game, othersoccergoalinfo goal, leaginfo where game.match_cnt = goal.match_cnt and goal.WEBACT='X' and goal.ACT='U' and game.league = leaginfo.alias order by leaginfo.leag_order");
						m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
						while(m_SportsOleReader.Read()) {
							HTMLString.Append("<tr align=\"center\"><td>");
							HTMLString.Append(m_SportsOleReader.GetString(0).Trim());
							HTMLString.Append("</td><td>");
							HTMLString.Append(m_SportsOleReader.GetString(1).Trim());
							HTMLString.Append("</td><td>");
							HTMLString.Append(m_SportsOleReader.GetString(2).Trim());
							HTMLString.Append("</td></tr>");
						}
						m_SportsOleReader.Close();
						m_SportsDBMgr.Close();
						break;

					case "otherodds":
						string sRegionID = HttpContext.Current.Request.QueryString["regionID"];

						SQLString.Remove(0,SQLString.Length);
						SQLString.Append("select ALIAS, HOST, GUEST from OTHERODDSINFO, leaginfo where ACT='U' AND WEBACT='X' AND COMPANY=(select CREGION from OTHERREGION_CFG where IREGION_ID=");
						SQLString.Append(sRegionID);
						SQLString.Append(") AND leaginfo.alias=OTHERODDSINFO.ALIAS order by leaginfo.leag_order, OTHERODDSINFO.MATCH_ID");
						m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
						while(m_SportsOleReader.Read()) {
							HTMLString.Append("<tr align=\"center\"><td>");
							HTMLString.Append(m_SportsOleReader.GetString(0).Trim());
							HTMLString.Append("</td><td>");
							HTMLString.Append(m_SportsOleReader.GetString(1).Trim());
							HTMLString.Append("</td><td>");
							HTMLString.Append(m_SportsOleReader.GetString(2).Trim());
							HTMLString.Append("</td></tr>");
						}
						m_SportsOleReader.Close();
						m_SportsDBMgr.Close();
						break;

					default:
						HTMLString.Append("<tr align=\"center\"><th colspan=\"3\">檢視錯誤</th></tr>");
						break;
				}
			} catch(Exception ex) {
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " OtherSoccerHiddenMatch.cs.Show(): " + ex.ToString());
				m_SportsLog.Close();
				HTMLString.Remove(0,HTMLString.Length);
				HTMLString.Append((string)HttpContext.Current.Application["accessErrorMsg"]);
			}

			return HTMLString.ToString();
		}

	}
}
