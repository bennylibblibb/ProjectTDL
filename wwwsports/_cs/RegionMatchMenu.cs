/*
Objective:
Create a combo box contains matches for other region

Last updated:
31 Oct 2003 by Chapman

C#.NET complier statement:
csc /t:library /out:..\bin\RegionMatchMenu.dll /r:..\bin\DBManager.dll;..\bin\Files.dll RegionMatchMenu.cs
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
[assembly:AssemblyDescription("其他地區 -> 修改賽事 -> 選單")]
[assembly:AssemblyProduct("Sports.NET")]
[assembly:AssemblyTitle("Sports.NET DLL")]
[assembly:AssemblyVersion("1.0.*")]
namespace SportsUtil {
	public class RegionMatchMenu {
		const string LOGFILESUFFIX = "log";
		DBManager m_SportsDBMgr;
		Files m_SportsLog;
		StringBuilder HTMLString;

		public RegionMatchMenu(string Connection) {
			m_SportsDBMgr = new DBManager();
			m_SportsDBMgr.ConnectionString = Connection;
			m_SportsLog = new Files();
			HTMLString = new StringBuilder();
		}

		public string Show() {
			string retrieveQuery;
			string sAlias = "";
			OleDbDataReader SportsOleReader;

			retrieveQuery = "select oth.alias, min(oth.MATCH_CNT), oth.host, oth.guest from OTHERODDSINFO oth JOIN LEAGINFO leag ON leag.LEAG_ID in (select cleag_id from userprofile_info where iuser_id=";
			retrieveQuery += HttpContext.Current.Session["user_id"].ToString();
			retrieveQuery += ") and oth.ALIAS=leag.alias GROUP BY oth.alias, oth.host, oth.guest order by leag.LEAG_ORDER, oth.MATCH_CNT";
/*
			retrieveQuery = "select distinct alias, host, guest from OTHERODDSINFO, LEAGINFO WHERE LEAGINFO.leag_id in (select cleag_id from userprofile_info where iuser_id=";
			retrieveQuery += HttpContext.Current.Session["user_id"].ToString();
			retrieveQuery += ") AND LEAGINFO.alias=OTHERODDSINFO.ALIAS order by LEAGINFO.LEAG_ORDER";
*/
			try {
				SportsOleReader = m_SportsDBMgr.ExecuteQuery(retrieveQuery);
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
			} catch(Exception ex) {
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " RegionMatchMenu.cs.Show(): " + ex.ToString());
				m_SportsLog.Close();
				HTMLString.Remove(0,HTMLString.Length);
				HTMLString.Append((string)HttpContext.Current.Application["accessErrorMsg"]);
			}

			return HTMLString.ToString();
		}
	}
}