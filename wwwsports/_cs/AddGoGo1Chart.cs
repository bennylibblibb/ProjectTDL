/*
Objective:
Add a GOGO1 chart data for specific match

Last updated:
11 Mar 2004, Chapman Choi

C#.NET complier statement:
csc /t:library /out:..\bin\AddGoGo1Chart.dll /r:..\bin\DBManager.dll;..\bin\Files.dll AddGoGo1Chart.cs
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
[assembly:AssemblyDescription("GOGO1 指數圖表 -> 圖表數據(新增)")]
[assembly:AssemblyProduct("Sports.NET")]
[assembly:AssemblyTitle("Sports.NET DLL")]
[assembly:AssemblyVersion("1.0.*")]
namespace SportsUtil {
	public class AddGoGo1Chart {
		const string LOGFILESUFFIX = "log";
		OleDbDataReader m_SportsOleReader;
		DBManager m_SportsDBMgr;
		Files m_SportsLog;
		StringBuilder SQLString;

		public AddGoGo1Chart(string Connection) {
			m_SportsDBMgr = new DBManager();
			m_SportsDBMgr.ConnectionString = Connection;
			m_SportsLog = new Files();
			SQLString = new StringBuilder();
		}

		public string Display() {
			string sLeague = "";
			string sHost = "";
			string sGuest = "";
			string sMatchCount;
			StringBuilder HTMLString = new StringBuilder();

			sMatchCount = HttpContext.Current.Request.QueryString["matchcount"].Trim();
			try {
				if(!sMatchCount.Equals("")) {
					SQLString.Append("select LEAGLONG, HOST, GUEST from GAMEINFO where MATCH_CNT=");
					SQLString.Append(sMatchCount);
					m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
					if(m_SportsOleReader.Read()) {
						sLeague = m_SportsOleReader.GetString(0).Trim();
						sHost = m_SportsOleReader.GetString(1).Trim();
						sGuest = m_SportsOleReader.GetString(2).Trim();
					}
					m_SportsOleReader.Close();
					m_SportsDBMgr.Close();
					m_SportsDBMgr.Dispose();

					//League (Label and Hidden Field)
					HTMLString.Append("<tr style=\"background-color:#7FFFD4\"><th colspan=\"3\"><input type=\"hidden\" name=\"League\" value=\"");
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
					HTMLString.Append("<tr><th colspan=\"3\"><input type=\"hidden\" name=\"MatchCount\" value=\"\">此賽事沒有圖表數據</th></tr>");
				}
			} catch(Exception ex) {
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " AddGoGo1Chart.cs.Display(): " + ex.ToString());
				m_SportsLog.Close();
				HTMLString.Remove(0,HTMLString.Length);
				HTMLString.Append("<tr><th colspan=\"3\">");
				HTMLString.Append((string)HttpContext.Current.Application["accessErrorMsg"]);
				HTMLString.Append("</th></tr>");
			}

			return HTMLString.ToString();
		}

		public int Add() {
			int iMaxRecNo;
			int iMatchCount;
			string sLeague;
			string sHost;
			string sGuest;
			string sTimestamp;
			string sHandicap;
			string sOdds;

			sLeague = HttpContext.Current.Request.Form["League"];
			sHost = HttpContext.Current.Request.Form["Host"];
			sGuest = HttpContext.Current.Request.Form["Guest"];
			iMatchCount = Convert.ToInt32(HttpContext.Current.Request.Form["MatchCount"]);
			sTimestamp = HttpContext.Current.Request.Form["timestamp"];
			sHandicap = HttpContext.Current.Request.Form["Handicap"];
			if(sHandicap.Trim().Equals("")) sHandicap = "-1";
			sOdds = HttpContext.Current.Request.Form["Odds"];
			if(sOdds.Trim().Equals("")) sOdds = "-1";
			try {
				if(!sTimestamp.Trim().Equals("")) {
					SQLString.Remove(0,SQLString.Length);
					SQLString.Append("select max(IREC_NO) from ODDSHISTORY");
					m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
					if(m_SportsOleReader.Read()) {
						if(!m_SportsOleReader.IsDBNull(0)) {
							iMaxRecNo = m_SportsOleReader.GetInt32(0) + 1;
						} else {
							iMaxRecNo = 1;
						}
					} else {
						iMaxRecNo = 1;
					}
					m_SportsOleReader.Close();
					m_SportsDBMgr.Close();

					SQLString.Remove(0,SQLString.Length);
					SQLString.Append("insert into ODDSHISTORY values('");
					SQLString.Append(sLeague);
					SQLString.Append("','");
					SQLString.Append(sHost);
					SQLString.Append("','");
					SQLString.Append(sGuest);
					SQLString.Append("',");
					SQLString.Append(sOdds);
					SQLString.Append(",'");
					SQLString.Append(sHandicap);
					SQLString.Append("','");
					SQLString.Append(sTimestamp);
					SQLString.Append("',");
					SQLString.Append(iMaxRecNo.ToString());
					SQLString.Append(")");
					m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
					m_SportsDBMgr.Close();

					//write log
					m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
					m_SportsLog.SetFileName(0,LOGFILESUFFIX);
					m_SportsLog.Open();
					m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " AddGoGo1Chart.cs: Insert a chart record <L:" + sLeague + ", H:" + sHost + ", G:" + sGuest + "> (" + HttpContext.Current.Session["user_name"] + ")");
					m_SportsLog.Close();
				} else {
					//write log
					m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
					m_SportsLog.SetFileName(0,LOGFILESUFFIX);
					m_SportsLog.Open();
					m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " AddGoGo1Chart.cs: Empty timestamp chart record did not add (" + HttpContext.Current.Session["user_name"] + ")");
					m_SportsLog.Close();
				}
			} catch(Exception ex) {
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " AddGoGo1Chart.cs.Add(): " + ex.ToString());
				m_SportsLog.Close();
			}

			return iMatchCount;
		}
	}
}