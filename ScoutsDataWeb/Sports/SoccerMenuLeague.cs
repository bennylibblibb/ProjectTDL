/*
Objective:
Create a combo box contains leagues

Last updated:
31 Oct 2003 by Chapman

C#.NET complier statement:
csc /t:library /out:..\bin\SoccerMenuLeague.dll /r:..\bin\DBManager.dll;..\bin\Files.dll SoccerMenuLeague.cs
*/

using System;
using System.Configuration;
using System.Data.OleDb;
using System.Reflection;
using System.Text;
using System.Web;
using TDL.DB;
using TDL.IO;

 
namespace SportsUtil {
	public class SoccerMenuLeague {
		const string LOGFILESUFFIX = "log";
		DBManager m_SportsDBMgr;
		Files m_SportsLog;
		StringBuilder HTMLString;

		public SoccerMenuLeague(string Connection) {
			m_SportsDBMgr = new DBManager();
			m_SportsDBMgr.ConnectionString = Connection;
			m_SportsLog = new Files();
			HTMLString = new StringBuilder();
		}

		public string Show() {
			OleDbDataReader SportsOleReader;

			try {
               string uid = HttpContext.Current.Session["user_id"] == null ? "3" : HttpContext.Current.Session["user_id"].ToString();
                //  SportsOleReader = m_SportsDBMgr.ExecuteQuery("select distinct LEAG_ID, ALIAS from LEAGINFO where leag_id in (select cleag_id from userprofile_info where iuser_id=" + HttpContext.Current.Session["user_id"].ToString() + ") order by LEAG_ORDER, ALIAS");
                SportsOleReader = m_SportsDBMgr.ExecuteQuery("select distinct LEAG_ID, ALIAS from LEAGINFO where leag_id in (select cleag_id from userprofile_info where iuser_id=" + uid + ") order by LEAG_ORDER, ALIAS");
                while (SportsOleReader.Read()) {
					HTMLString.Append("<option value=\"");
					HTMLString.Append(SportsOleReader.GetString(0).Trim());
					HTMLString.Append("\">");
					HTMLString.Append(SportsOleReader.GetString(1).Trim());
					HTMLString.Append("</option>");
				}
				SportsOleReader.Close();
				m_SportsDBMgr.Close();
				m_SportsDBMgr.Dispose();
			} catch(Exception ex) {
                m_SportsLog.FilePath = ConfigurationManager.AppSettings["eventlog"];

                m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " SoccerMenuLeague.cs.Show(): " + ex.ToString());
				m_SportsLog.Close();
				HTMLString.Remove(0,HTMLString.Length);
				HTMLString.Append(ConfigurationManager.AppSettings["accessErrorMsg"]);
			}

			return HTMLString.ToString();
		}
	}
}