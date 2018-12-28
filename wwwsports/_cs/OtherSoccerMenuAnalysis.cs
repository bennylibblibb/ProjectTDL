/*
Objective:
Create a combo box contains matches

Last updated:
7 Oct 2004 by Fanny Cheung

C#.NET complier statement:
csc /t:library /out:..\bin\OtherSoccerMenuAnalysis.dll /r:..\bin\DBManager.dll;..\bin\Files.dll OtherSoccerMenuAnalysis.cs
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
	public class OtherSoccerMenuAnalysis {
		const string LOGFILESUFFIX = "log";
		DBManager m_SportsDBMgr;
		Files m_SportsLog;
		StringBuilder HTMLString;

		public OtherSoccerMenuAnalysis(string Connection) {
			m_SportsDBMgr = new DBManager();
			m_SportsDBMgr.ConnectionString = Connection;
			m_SportsLog = new Files();
			HTMLString = new StringBuilder();
		}

		public string Show() {
			string sAlias = "";
			OleDbDataReader SportsOleReader;

			try {
				//SportsOleReader = m_SportsDBMgr.ExecuteQuery("select othersoccerinfo.league, othersoccerinfo.match_cnt, othersoccerinfo.host, othersoccerinfo.guest from othersoccerinfo, leaginfo where othersoccerinfo.league=leaginfo.alias and othersoccerinfo.act='U' and leaginfo.leag_id in (select cleag_id from userprofile_info where iuser_id=" + HttpContext.Current.Session["user_id"].ToString() + ") order by leaginfo.leag_order");
				SportsOleReader = m_SportsDBMgr.ExecuteQuery("select othersoccerinfo.league, othersoccerinfo.match_cnt, othersoccerinfo.host, othersoccerinfo.guest, (select count(*) from othersoccerinfo where othersoccerinfo.league=leaginfo.alias and othersoccerinfo.act='U' and leaginfo.leag_id in (select cleag_id from userprofile_info where iuser_id=" + HttpContext.Current.Session["user_id"].ToString() + ")) from othersoccerinfo, leaginfo where othersoccerinfo.league=leaginfo.alias and othersoccerinfo.act='U' and leaginfo.leag_id in (select cleag_id from userprofile_info where iuser_id=" + HttpContext.Current.Session["user_id"].ToString() + ") order by leaginfo.leag_order");
				while(SportsOleReader.Read()) {
					if(!SportsOleReader.IsDBNull(0)) {
						if(!sAlias.Equals(SportsOleReader.GetString(0).Trim())) {
							sAlias = SportsOleReader.GetString(0).Trim();
							HTMLString.Append("<option value=\"\">");
							HTMLString.Append(sAlias);
							HTMLString.Append(" - ");
							HTMLString.Append("(" + SportsOleReader.GetInt32(4).ToString() + ")");
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
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " OtherSoccerMenuAnalysis.cs.Show(): " + ex.ToString());
				m_SportsLog.Close();
				HTMLString.Remove(0,HTMLString.Length);
				HTMLString.Append((string)HttpContext.Current.Application["accessErrorMsg"]);
			}

			return HTMLString.ToString();
		}
	}
}