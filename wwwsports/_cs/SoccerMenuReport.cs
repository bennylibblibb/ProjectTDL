/*
Objective:
Create a combo box contains housekeeped matches

Last updated:
20 Jan 2004 by Chapman

C#.NET complier statement:
csc /t:library /out:..\bin\SoccerMenuReport.dll /r:..\bin\DBManager.dll;..\bin\Files.dll SoccerMenuReport.cs
*/

using System;
using System.Collections;
using System.Data.OleDb;
using System.Reflection;
using System.Text;
using System.Web;
using TDL.DB;
using TDL.IO;

[assembly:AssemblyCompany("TDSL")]
[assembly:AssemblyCopyright("(c) 2003 TDSL. All rights reserved.")]
[assembly:AssemblyDescription("足球資訊 -> 賽後報告(菜單)")]
[assembly:AssemblyProduct("Sports.NET")]
[assembly:AssemblyTitle("Sports.NET DLL")]
[assembly:AssemblyVersion("1.0.*")]
namespace SportsUtil {
	public class SoccerMenuReport {
		const string LOGFILESUFFIX = "log";
		DBManager m_SportsDBMgr;
		DBManager m_GOGO1DBMgr;
		Files m_SportsLog;
		StringBuilder HTMLString;
		StringBuilder SQLString;
		ArrayList leagueList;

		public SoccerMenuReport(string Connection, string GOGO1Connection) {
			m_SportsDBMgr = new DBManager();
			m_SportsDBMgr.ConnectionString = Connection;
			m_GOGO1DBMgr = new DBManager();
			m_GOGO1DBMgr.ConnectionString = GOGO1Connection;			
			m_SportsLog = new Files();
			HTMLString = new StringBuilder();
			SQLString = new StringBuilder();
			leagueList = new ArrayList();
		}

		public string Show() {
			string sAlias = "";
			OleDbDataReader SportsOleReader;

			try {
				//Retrieve worker league from GOGO1 DB
				SportsOleReader = m_GOGO1DBMgr.ExecuteQuery("select distinct LEAGNAME from LEAGINFO where leag_id in (select cleag_id from userprofile_info where iuser_id=" + HttpContext.Current.Session["user_id"].ToString() + ") order by LEAG_ORDER, LEAG_ID");
				while(SportsOleReader.Read()) {
					leagueList.Add(SportsOleReader.GetString(0).Trim());
				}
				SportsOleReader.Close();
				m_GOGO1DBMgr.Close();
				m_GOGO1DBMgr.Dispose();
				leagueList.TrimToSize();

				//Retrieve housekeeped matches from GOGO2 DB according to list of leagues
				if(leagueList.Count> 0) {
					SQLString.Append("select main.cleague, main.irec_id, main.chost, main.cguest from hk_sports_master main, HK_GOALMENU menu where main.chost<>'-1' and main.cguest<>'-1' and main.IREC_ID=menu.IREC_ID and main.cleague in (");
					for(int i = 0; i < leagueList.Count; i++) {
						SQLString.Append("'");
						SQLString.Append(leagueList[i]);
						SQLString.Append("'");
						if((leagueList.Count - i) > 1) SQLString.Append(",");
					}
					SQLString.Append(") order by menu.ILEAGUEORDER, menu.ITEAMORDER");
					SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
					while(SportsOleReader.Read()) {
						if(!SportsOleReader.IsDBNull(0)) {
							if(!sAlias.Equals(SportsOleReader.GetString(0).Trim())) {
								sAlias = SportsOleReader.GetString(0).Trim();
								HTMLString.Append("<option value=\"\">");
								HTMLString.Append(sAlias);
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
				}
			} catch(Exception ex) {
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " SoccerMenuReport.cs.Show(): " + ex.ToString());
				m_SportsLog.Close();
				HTMLString.Remove(0,HTMLString.Length);
				HTMLString.Append((string)HttpContext.Current.Application["accessErrorMsg"]);
			}

			return HTMLString.ToString();
		}
	}
}