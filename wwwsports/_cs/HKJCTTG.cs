/*
Objective:
Retrieval and modify HKJC soccer TTG information

Last updated:
24 Sep 2003 by Chapman

C#.NET complier statement:
csc /t:library /out:..\bin\HKJCTTG.dll /r:..\bin\DBManager.dll;..\bin\Files.dll HKJCTTG.cs
*/

using System;
using System.Collections;
using System.Data.OleDb;
using System.Text;
using System.Reflection;
using System.Web;
using TDL.DB;
using TDL.IO;

[assembly:AssemblyCompany("TDSL")]
[assembly:AssemblyCopyright("(c) 2003 TDSL. All rights reserved. Created on 12 August 2003.")]
[assembly:AssemblyDescription("JC¨¬´¼±m -> Á`¤J²y")]
[assembly:AssemblyProduct("Sports.NET")]
[assembly:AssemblyTitle("Sports.NET DLL")]
[assembly:AssemblyVersion("1.0.*")]
namespace SportsUtil {
	public class HKJCTTG {
		const string LOGFILESUFFIX = "log";
		int m_RecordCount = 0;
		OleDbDataReader m_SportsOleReader;
		DBManager m_SportsDBMgr;
		Files m_SportsLog;
		StringBuilder SQLString;

		public HKJCTTG(string Connection) {
			m_SportsDBMgr = new DBManager();
			m_SportsDBMgr.ConnectionString = Connection;
			m_SportsLog = new Files();
			SQLString = new StringBuilder();
		}

		public string GetTTG() {
			string uid;
			string sMatchCount = "";
			string[] weekArray;
			StringBuilder HTMLString = new StringBuilder();
			OleDbDataReader TTGOddsReader;
			DBManager TTGDBMgr = new DBManager();
			TTGDBMgr.ConnectionString = m_SportsDBMgr.ConnectionString;

			weekArray = (string[])HttpContext.Current.Application["WeekItems"];
			uid = HttpContext.Current.Session["user_id"].ToString();

			SQLString.Remove(0,SQLString.Length);
			SQLString.Append("select main.IMATCH_CNT, main.IWEEK, main.ISEQ_NO, main.MATCHDATETIME, main.CLEAGUEALIAS, main.CHOST, main.CGUEST from HKJCSOCCER_INFO main, leaginfo leag where main.CACT='U' and main.CLEAGUEALIAS = leag.alias and leaginfo.leag_id in (select cleag_id from userprofile_info where iuser_id=");
			SQLString.Append(uid);
			SQLString.Append(") order by leag.leag_order, main.MATCHDATETIME, main.IWEEK, main.ISEQ_NO");
/*
			SQLString.Append("select main.IMATCH_CNT, main.IWEEK, main.ISEQ_NO, main.MATCHDATETIME, main.CLEAGUEALIAS, main.CHOST, main.CGUEST from HKJCSOCCER_INFO main, leaginfo leag where main.CACT='U' and main.CLEAGUEALIAS = leag.alias and main.CLEAGUEALIAS in (select distinct leaginfo.alias from leaginfo, userprofile_info where leaginfo.leag_id in (select cleag_id from userprofile_info where iuser_id=");
			SQLString.Append(uid);
			SQLString.Append(")) order by leag.leag_order, main.MATCHDATETIME, main.IWEEK, main.ISEQ_NO");
*/
			try {
				m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
				while(m_SportsOleReader.Read()) {
					HTMLString.Append("<tr align=\"center\">");

					//Match count (Hidden Field), week, sequence number
					sMatchCount = m_SportsOleReader.GetInt32(0).ToString();
					HTMLString.Append("<td><input type=\"hidden\" name=\"MatchCount\" value=\"");
					HTMLString.Append(sMatchCount);
					HTMLString.Append("\">");
					HTMLString.Append(weekArray[m_SportsOleReader.GetInt32(1)]);
					HTMLString.Append(" ");
					HTMLString.Append(m_SportsOleReader.GetInt32(2).ToString());
					HTMLString.Append("</td>");

					//Match date time
					HTMLString.Append("<td>");
					HTMLString.Append(m_SportsOleReader.GetDateTime(3).ToString("yyyy/MM/dd HH:mm"));
					HTMLString.Append("</td>");

					//Alias
					HTMLString.Append("<td>");
					HTMLString.Append(m_SportsOleReader.GetString(4));
					HTMLString.Append("</td>");

					//Host
					HTMLString.Append("<td>");
					HTMLString.Append(m_SportsOleReader.GetString(5));
					HTMLString.Append("</td>");

					//Guest
					HTMLString.Append("<td>");
					HTMLString.Append(m_SportsOleReader.GetString(6));
					HTMLString.Append("</td>");

					//select TTG odds
					SQLString.Remove(0,SQLString.Length);
					SQLString.Append("select CODDS from HKJCSOCTTG_INFO where IMATCH_CNT=");
					SQLString.Append(sMatchCount);
					SQLString.Append(" order by ITOTAL_GOAL");
					TTGOddsReader = TTGDBMgr.ExecuteQuery(SQLString.ToString());
					while(TTGOddsReader.Read()) {
						HTMLString.Append("<td><input name=\"TTGOdds\" value=\"");
						if(!TTGOddsReader.IsDBNull(0)) {
							HTMLString.Append(TTGOddsReader.GetString(0).Trim());
						}
						HTMLString.Append("\" maxlength=\"5\" size=\"2\" onChange=\"TTGOddsValidity(");
						HTMLString.Append(m_RecordCount.ToString());
						HTMLString.Append(")\"></td>");
						m_RecordCount++;
					}
					TTGOddsReader.Close();
					TTGDBMgr.Close();
					HTMLString.Append("</tr>");
				}
				m_SportsDBMgr.Close();
				m_SportsOleReader.Close();
				m_SportsDBMgr.Dispose();
				if(m_RecordCount > 0) TTGDBMgr.Dispose();
			} catch(Exception ex) {
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " HKJCTTG.cs.GetTTG(): " + ex.ToString());
				m_SportsLog.Close();
				HTMLString.Remove(0,HTMLString.Length);
				HTMLString.Append((string)HttpContext.Current.Application["accessErrorMsg"]);
			}

			return HTMLString.ToString();
		}

		public int Modify() {
			int iMatchIndex = 0;
			int iGoalIndex = 0;
			char[] delimiter = new char[] {','};
			string[] arrMatchCount;
			string[] arrTTGOdds;

			arrMatchCount = HttpContext.Current.Request.Form["MatchCount"].Split(delimiter);
			arrTTGOdds = HttpContext.Current.Request.Form["TTGOdds"].Split(delimiter);
			try {
				for(iMatchIndex = 0; iMatchIndex < arrMatchCount.Length; iMatchIndex++) {
					for(iGoalIndex = 0; iGoalIndex < 8; iGoalIndex++) {
						SQLString.Remove(0,SQLString.Length);
						SQLString.Append("update HKJCSOCTTG_INFO set CODDS=");
						if(arrTTGOdds[(8*iMatchIndex+iGoalIndex)].Trim().Equals("")) {
							SQLString.Append("null");
						}	else {
							SQLString.Append("'");
							SQLString.Append(arrTTGOdds[(8*iMatchIndex+iGoalIndex)].Trim());
							SQLString.Append("'");
						}
						SQLString.Append(" where IMATCH_CNT=");
						SQLString.Append(arrMatchCount[iMatchIndex]);
						SQLString.Append(" AND ITOTAL_GOAL=");
						SQLString.Append(iGoalIndex.ToString());
						m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
						m_SportsDBMgr.Close();
					}
				}
				if(iMatchIndex > 0) m_SportsDBMgr.Dispose();

				//write log
				m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " HKJCTTG.cs: Modify " + iMatchIndex.ToString() + " TTG records (" + HttpContext.Current.Session["user_name"] + ")");
				m_SportsLog.Close();
			} catch(Exception ex) {
				iMatchIndex = -1;
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " HKJCTTG.cs.Modify(): " + ex.ToString());
				m_SportsLog.Close();
			}
			return iMatchIndex;
		}
	}
}