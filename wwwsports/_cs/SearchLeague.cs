/*
Objective:
Search league according to keyword

Last updated:
26 July 2004, Chapman Choi

C#.NET complier statement:
csc /t:library /out:..\bin\SearchLeague.dll /r:..\bin\DBManager.dll;..\bin\Files.dll SearchLeague.cs
*/

using System;
using System.Data.OleDb;
using System.Reflection;
using System.Text;
using System.Web;
using TDL.DB;
using TDL.IO;

[assembly:AssemblyCompany("TDSL")]
[assembly:AssemblyCopyright("(c) 2003 TDSL. All rights reserved. Created on 19 July 2004")]
[assembly:AssemblyDescription("足球資訊 -> 聯賽搜尋")]
[assembly:AssemblyProduct("Sports.NET")]
[assembly:AssemblyTitle("Sports.NET DLL")]
[assembly:AssemblyVersion("1.0.*")]
namespace SportsUtil {
	public class SearchLeague {
		const string LOGFILESUFFIX = "log";
		DBManager m_SportsDBMgr;
		Files m_SportsLog;
		OleDbDataReader m_SportsOleReader;
		StringBuilder SQLString;

		public SearchLeague(string Connection) {
			m_SportsDBMgr = new DBManager();
			m_SportsDBMgr.ConnectionString = Connection;
			m_SportsLog = new Files();
			SQLString = new StringBuilder();
		}

		//search league from POST method
		public string ShowLeagues() {
			char[] delimiter = new char[] {','};
			string sAction;
			string sKeyword;
			string[] arrSearchFlag;
			StringBuilder HTMLString = new StringBuilder();

			sAction = HttpContext.Current.Request.Form["action"];
			sKeyword = HttpContext.Current.Request.Form["keyword"];
			if(sKeyword != null) {
				if(sKeyword.Trim().Equals("")) sKeyword = "-1";
				else sKeyword = sKeyword.Trim();
			} else {
				sKeyword = "-1";
			}
			try {
				arrSearchFlag = HttpContext.Current.Request.Form["searchType"].Split(delimiter);
			} catch(Exception) {
				arrSearchFlag = new string[0];
			}

			try {
				if(!sKeyword.Equals("-1")) {
					if(arrSearchFlag.Length > 0) {
						SQLString.Remove(0,SQLString.Length);
						SQLString.Append("SELECT LEAG_ID, LEAGNAME, ALIAS, CHKJCNAME, CHKJCSHORTNAME, CMACAUNAME FROM LEAGINFO WHERE ");
						for(int i = 0; i < arrSearchFlag.Length; i++) {
							if(arrSearchFlag[i].Equals("A")) {
								if(i > 0) SQLString.Append("OR ");
								SQLString.Append("LEAGNAME like '%");
								SQLString.Append(sKeyword);
								SQLString.Append("%' OR ALIAS like '%");
								SQLString.Append(sKeyword);
								SQLString.Append("%' ");
							} else if(arrSearchFlag[i].Equals("J")) {
								if(i > 0) SQLString.Append("OR ");
								SQLString.Append("CHKJCNAME like '%");
								SQLString.Append(sKeyword);
								SQLString.Append("%' OR CHKJCSHORTNAME like '%");
								SQLString.Append(sKeyword);
								SQLString.Append("%' ");
							} else if(arrSearchFlag[i].Equals("M")) {
								if(i > 0) SQLString.Append("OR ");
								SQLString.Append("CMACAUNAME like '%");
								SQLString.Append(sKeyword);
								SQLString.Append("%' ");
							}
						}
						SQLString.Append("ORDER BY LEAG_ID");
						m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
						while(m_SportsOleReader.Read()) {
							//if(sAction.Equals("MOD")) HTMLString.Append("<tr><td><a href=\"LeagueManager_MODFrame.aspx?leagID=");
							//else HTMLString.Append("<tr><td><a href=\"LeagueManager_DEL.aspx?leagID=");
							if(sAction.Equals("DEL")) HTMLString.Append("<tr><td><a href=\"LeagueManager_DEL.aspx?leagID=");
							else HTMLString.Append("<tr><td><a href=\"LeagueManager_MODFrame.aspx?leagID=");
							HTMLString.Append(m_SportsOleReader.GetString(0).Trim());
							HTMLString.Append("\" target=\"content_frame\" onClick=\"setFocus()\">");
							HTMLString.Append(m_SportsOleReader.GetString(1).Trim().Replace(sKeyword, "<font style=\"background-color:#F0F8FF; color=#6FAFB1\">" + sKeyword + "</font>"));
							HTMLString.Append("</a></td><td>");
							HTMLString.Append(m_SportsOleReader.GetString(2).Trim().Replace(sKeyword, "<font style=\"background-color:#F0F8FF; color=#6FAFB1\">" + sKeyword + "</font>"));
							HTMLString.Append("</td><td>");
							HTMLString.Append(m_SportsOleReader.GetString(3).Trim().Replace(sKeyword, "<font style=\"background-color:#F0F8FF; color=#6FAFB1\">" + sKeyword + "</font>"));
							HTMLString.Append("</td><td>");
							HTMLString.Append(m_SportsOleReader.GetString(4).Trim().Replace(sKeyword, "<font style=\"background-color:#F0F8FF; color=#6FAFB1\">" + sKeyword + "</font>"));
							HTMLString.Append("</td><td>");
							HTMLString.Append(m_SportsOleReader.GetString(5).Trim().Replace(sKeyword, "<font style=\"background-color:#F0F8FF; color=#6FAFB1\">" + sKeyword + "</font>"));
							HTMLString.Append("</td></tr>");
						}
						m_SportsOleReader.Close();
						m_SportsDBMgr.Close();
					} else {
						HTMLString.Append("<tr><th colspan=\"5\">請選擇查詢條件！</th></tr>");
					}
				} else {
					HTMLString.Append("<tr><th colspan=\"5\">請輸入關鍵詞查詢！</th></tr>");
				}
			}	catch(Exception ex) {
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " SearchLeague.cs.ShowLeagues(): " + ex.ToString());
				m_SportsLog.Close();
				HTMLString.Remove(0,HTMLString.Length);
				HTMLString.Append("<tr><th colspan=\"5\">");
				HTMLString.Append((string)HttpContext.Current.Application["accessErrorMsg"]);
				HTMLString.Append("</th></tr>");
			}

			return HTMLString.ToString();
		}

		//search league from URL parameter
		public string ShowLeagues(string sKeyParam) {
			StringBuilder HTMLString = new StringBuilder();

			try {
				SQLString.Remove(0,SQLString.Length);
				SQLString.Append("SELECT LEAG_ID, LEAGNAME, ALIAS, CHKJCNAME, CHKJCSHORTNAME, CMACAUNAME FROM LEAGINFO WHERE ");
				SQLString.Append("LEAGNAME like '%");
				SQLString.Append(sKeyParam);
				SQLString.Append("%' OR ALIAS like '%");
				SQLString.Append(sKeyParam);
				SQLString.Append("%' OR CHKJCNAME like '%");
				SQLString.Append(sKeyParam);
				SQLString.Append("%' OR CHKJCSHORTNAME like '%");
				SQLString.Append(sKeyParam);
				SQLString.Append("%' OR CMACAUNAME like '%");
				SQLString.Append(sKeyParam);
				SQLString.Append("%' ORDER BY LEAG_ID");
				m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
				while(m_SportsOleReader.Read()) {
					HTMLString.Append("<tr><td><a href=\"LeagueManager_MODFrame.aspx?leagID=");
					HTMLString.Append(m_SportsOleReader.GetString(0).Trim());
					HTMLString.Append("\" target=\"content_frame\" onClick=\"setFocus()\">");
					HTMLString.Append(m_SportsOleReader.GetString(1).Trim().Replace(sKeyParam, "<font style=\"background-color:#F0F8FF; color=#6FAFB1\">" + sKeyParam + "</font>"));
					HTMLString.Append("</a></td><td>");
					HTMLString.Append(m_SportsOleReader.GetString(2).Trim().Replace(sKeyParam, "<font style=\"background-color:#F0F8FF; color=#6FAFB1\">" + sKeyParam + "</font>"));
					HTMLString.Append("</td><td>");
					HTMLString.Append(m_SportsOleReader.GetString(3).Trim().Replace(sKeyParam, "<font style=\"background-color:#F0F8FF; color=#6FAFB1\">" + sKeyParam + "</font>"));
					HTMLString.Append("</td><td>");
					HTMLString.Append(m_SportsOleReader.GetString(4).Trim().Replace(sKeyParam, "<font style=\"background-color:#F0F8FF; color=#6FAFB1\">" + sKeyParam + "</font>"));
					HTMLString.Append("</td><td>");
					HTMLString.Append(m_SportsOleReader.GetString(5).Trim().Replace(sKeyParam, "<font style=\"background-color:#F0F8FF; color=#6FAFB1\">" + sKeyParam + "</font>"));
					HTMLString.Append("</td></tr>");
				}
				m_SportsOleReader.Close();
				m_SportsDBMgr.Close();
			}	catch(Exception ex) {
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " SearchLeague.cs.ShowLeagues(string sKeyParam): " + ex.ToString());
				m_SportsLog.Close();
				HTMLString.Remove(0,HTMLString.Length);
				HTMLString.Append("<tr><th colspan=\"5\">");
				HTMLString.Append((string)HttpContext.Current.Application["accessErrorMsg"]);
				HTMLString.Append("</th></tr>");
			}

			return HTMLString.ToString();
		}

	}
}