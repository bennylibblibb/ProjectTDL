/*
Objective:
Delete all GOGO1 chart data for specific match

Last updated:
11 August 2003 by Chapman

C#.NET complier statement:
csc /t:library /out:..\bin\DeleteGoGo1Chart.dll /r:..\bin\DBManager.dll;..\bin\Files.dll DeleteGoGo1Chart.cs
*/

using System;
using System.Data.OleDb;
using System.Text;
using System.Reflection;
using System.Web;
using TDL.DB;
using TDL.IO;

[assembly:AssemblyCompany("TDSL")]
[assembly:AssemblyCopyright("(c) 2003 TDSL. All rights reserved. Created on 11 August 2003.")]
[assembly:AssemblyDescription("GOGO1 指數圖表 -> 圖表數據(刪除全部)")]
[assembly:AssemblyProduct("Sports.NET")]
[assembly:AssemblyTitle("Sports.NET DLL")]
[assembly:AssemblyVersion("1.0.*")]
namespace SportsUtil {
	public class DeleteGoGo1Chart {
		const string LOGFILESUFFIX = "log";
		DBManager m_SportsDBMgr;
		Files m_SportsLog;
		StringBuilder SQLString;

		public DeleteGoGo1Chart(string Connection) {
			m_SportsDBMgr = new DBManager();
			m_SportsDBMgr.ConnectionString = Connection;
			m_SportsLog = new Files();
			SQLString = new StringBuilder();
		}

		public string ConfirmDelete() {
			string sLeague = "";
			string sHost = "";
			string sGuest = "";
			string sMatchCount;
			StringBuilder HTMLString = new StringBuilder();
			OleDbDataReader SportsOleReader;

			sMatchCount = HttpContext.Current.Request.QueryString["matchcount"].Trim();
			try {
				if(!sMatchCount.Equals("")) {
					SQLString.Append("select LEAGLONG, HOST, GUEST from GAMEINFO where MATCH_CNT=");
					SQLString.Append(sMatchCount);
					SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
					if(SportsOleReader.Read()) {
						sLeague = SportsOleReader.GetString(0).Trim();
						sHost = SportsOleReader.GetString(1).Trim();
						sGuest = SportsOleReader.GetString(2).Trim();
					}
					SportsOleReader.Close();
					m_SportsDBMgr.Close();
					m_SportsDBMgr.Dispose();

					//League (Label and Hidden Field)
					HTMLString.Append("<tr style=\"background-color:#F0F8FF\"><th><input type=\"hidden\" name=\"League\" value=\"");
					HTMLString.Append(sLeague);
					HTMLString.Append("\">");
					HTMLString.Append(sLeague);
					HTMLString.Append(" - ");

					//Host (Label and Hidden Field)
					HTMLString.Append(sHost);
					HTMLString.Append("<input type=\"hidden\" name=\"Host\" value=\"");
					HTMLString.Append(sHost);
					HTMLString.Append("\"> vs ");

					//Guest (Label and Hidden Field)
					HTMLString.Append(sGuest);
					HTMLString.Append("<input type=\"hidden\" name=\"Guest\" value=\"");
					HTMLString.Append(sGuest);
					HTMLString.Append("\"><input type=\"hidden\" name=\"MatchCount\" value=\"");
					HTMLString.Append(sMatchCount);
					HTMLString.Append("\"></th></tr>");
				} else {
					HTMLString.Append("<tr><th><input type=\"hidden\" name=\"MatchCount\" value=\"\">此賽事沒有圖表數據</th></tr>");
				}
			} catch(Exception ex) {
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " DeleteGoGo1Chart.cs.ConfirmDelete(): " + ex.ToString());
				m_SportsLog.Close();
				HTMLString.Remove(0,HTMLString.Length);
				HTMLString.Append("<tr><th>");
				HTMLString.Append((string)HttpContext.Current.Application["accessErrorMsg"]);
				HTMLString.Append("</th></tr>");
			}

			return HTMLString.ToString();
		}

		public int Delete() {
			int iMatchCount;
			string sLeague;
			string sHost;
			string sGuest;

			sLeague = HttpContext.Current.Request.Form["League"];
			sHost = HttpContext.Current.Request.Form["Host"];
			sGuest = HttpContext.Current.Request.Form["Guest"];
			iMatchCount = Convert.ToInt32(HttpContext.Current.Request.Form["MatchCount"]);
			try {
				SQLString.Remove(0,SQLString.Length);
				SQLString.Append("delete from ODDSHISTORY where LEAGUE='");
				SQLString.Append(sLeague);
				SQLString.Append("' AND HOST='");
				SQLString.Append(sHost);
				SQLString.Append("' AND GUEST='");
				SQLString.Append(sGuest);
				SQLString.Append("'");
				m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
				m_SportsDBMgr.Close();

				//write log
				m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " DeleteGoGo1Chart.cs: Delete all chart records <L:" + sLeague + ", H:" + sHost + ", G:" + sGuest + "> (" + HttpContext.Current.Session["user_name"] + ")");
				m_SportsLog.Close();
			} catch(Exception ex) {
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " DeleteGoGo1Chart.cs.Delete(): " + ex.ToString());
				m_SportsLog.Close();
			}

			return iMatchCount;
		}
	}
}