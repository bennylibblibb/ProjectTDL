/*
Objective:
Create a combo box contains matches

Last updated:
31 Oct 2003 by Chapman

C#.NET complier statement:
csc /t:library /out:..\bin\SoccerMenuAnalysis.dll /r:..\bin\DBManager.dll;..\bin\Files.dll SoccerMenuAnalysis.cs
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
[assembly:AssemblyDescription("足球資訊 -> 賽事選項")]
[assembly:AssemblyProduct("Sports.NET")]
[assembly:AssemblyTitle("Sports.NET DLL")]
[assembly:AssemblyVersion("1.0.*")]
namespace SportsUtil {
	public class SoccerMenuAnalysis {
		const string LOGFILESUFFIX = "log";
		DBManager m_SportsDBMgr;
		Files m_SportsLog;
		StringBuilder HTMLString;

		public SoccerMenuAnalysis(string Connection) {
			m_SportsDBMgr = new DBManager();
			m_SportsDBMgr.ConnectionString = Connection;
			m_SportsLog = new Files();
			HTMLString = new StringBuilder();
		}

		public string Show() {
			string sAlias = "";
			OleDbDataReader SportsOleReader;

			try {
				//SportsOleReader = m_SportsDBMgr.ExecuteQuery("select gameinfo.league, gameinfo.match_cnt, gameinfo.host, gameinfo.guest from gameinfo, leaginfo where gameinfo.league=leaginfo.alias and gameinfo.act='U' and leaginfo.leag_id in (select cleag_id from userprofile_info where iuser_id=" + HttpContext.Current.Session["user_id"].ToString() + ") order by leaginfo.leag_order, leaginfo.alias, gameinfo.host, gameinfo.guest");
				SportsOleReader = m_SportsDBMgr.ExecuteQuery("select gameinfo.league, gameinfo.match_cnt, gameinfo.host, gameinfo.guest, (select count(*) from gameinfo where gameinfo.league=leaginfo.alias and gameinfo.act='U' and leaginfo.leag_id in (select cleag_id from userprofile_info where iuser_id=" + HttpContext.Current.Session["user_id"].ToString() + ")), (select count(*) from gameinfo where gameinfo.league=leaginfo.alias and gameinfo.act='U' and gameinfo.webact='X' and leaginfo.leag_id in (select cleag_id from userprofile_info where iuser_id=" + HttpContext.Current.Session["user_id"].ToString() + ")) from gameinfo, leaginfo where gameinfo.league=leaginfo.alias and gameinfo.act='U' and leaginfo.leag_id in (select cleag_id from userprofile_info where iuser_id=" + HttpContext.Current.Session["user_id"].ToString() + ") order by leaginfo.leag_order, leaginfo.alias, gameinfo.host, gameinfo.guest");
				while(SportsOleReader.Read()) {
					if(!SportsOleReader.IsDBNull(0)) {
						if(!sAlias.Equals(SportsOleReader.GetString(0).Trim())) {
							sAlias = SportsOleReader.GetString(0).Trim();
							HTMLString.Append("<option value=\"\">");
							HTMLString.Append(sAlias);
							HTMLString.Append(" - ");
							HTMLString.Append(SportsOleReader.GetInt32(4).ToString() + " (" + SportsOleReader.GetInt32(5).ToString() + ")");
							HTMLString.Append("</option>");
						}
						HTMLString.Append("<option value=\"");
						HTMLString.Append(SportsOleReader.GetInt32(1).ToString());
						HTMLString.Append("\">");
						HTMLString.Append(SportsOleReader.GetString(2).Trim());
						HTMLString.Append(" vs ");
						HTMLString.Append(SportsOleReader.GetString(3).Trim());
						HTMLString.Append("</option>");
					}
				}
				SportsOleReader.Close();
				m_SportsDBMgr.Close();
				m_SportsDBMgr.Dispose();
			} catch(Exception ex) {
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " SoccerMenuAnalysis.cs.Show(): " + ex.ToString());
				m_SportsLog.Close();
				HTMLString.Remove(0,HTMLString.Length);
				HTMLString.Append((string)HttpContext.Current.Application["accessErrorMsg"]);
			}

			return HTMLString.ToString();
		}
	}
}