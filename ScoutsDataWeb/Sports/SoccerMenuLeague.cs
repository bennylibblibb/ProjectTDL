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
//using System.Data.OleDb;
using FirebirdSql.Data.FirebirdClient;
using System.Reflection;
using System.Text;
using System.Web;
using TDL.DB;
using TDL.IO;

 
namespace SportsUtil {
	public class SoccerMenuLeague {
		const string LOGFILESUFFIX = "log";
		DBManagerFB m_SportsDBMgr;
		Files m_SportsLog;
		StringBuilder HTMLString;

		public SoccerMenuLeague(string Connection) {
			m_SportsDBMgr = new DBManagerFB();
			m_SportsDBMgr.ConnectionString = JC_SoccerWeb.Common.AppFlag.ScoutsDBConn; ;
			m_SportsLog = new Files();
			HTMLString = new StringBuilder();
		}

		public string Show() {
			FbDataReader SportsOleReader;

			try {
               string uid = HttpContext.Current.Session["user_id"] == null ? "3" : HttpContext.Current.Session["user_id"].ToString();
                //  SportsOleReader = m_SportsDBMgr.ExecuteQuery("select distinct LEAG_ID, ALIAS from LEAGINFO where leag_id in (select cleag_id from userprofile_info where iuser_id=" + HttpContext.Current.Session["user_id"].ToString() + ") order by LEAG_ORDER, ALIAS");
                //SportsOleReader = m_SportsDBMgr.ExecuteQuery("select distinct LEAG_ID, ALIAS from LEAGINFO where leag_id in (select cleag_id from userprofile_info where iuser_id=" + uid + ") order by LEAG_ORDER, ALIAS");
                SportsOleReader = m_SportsDBMgr.ExecuteQuery("SELECT  c.ID LEAG_ID, l.LEAGUE_CHI_NAME ALIAS FROM COMPETITIONS c inner join LEAGUE_INFO l on  l.CLEAGUE_ALIAS_NAME = c.ALIAS  where alias is not null ");
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

            return HTMLString.ToString();//.Insert(7, " selected"); ;
		}
	}
}