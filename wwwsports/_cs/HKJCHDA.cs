/*
Objective:
Retrieval and modify HKJC soccer HDA information

Last updated:
24 sep 2003 by Chapman

C#.NET complier statement:
csc /t:library /out:..\bin\HKJCHDA.dll /r:..\bin\DBManager.dll;..\bin\Files.dll HKJCHDA.cs
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
[assembly:AssemblyCopyright("(c) 2003 TDSL. All rights reserved. Created on 8 August 2003.")]
[assembly:AssemblyDescription("JC¨¬´¼±m -> ¥D«È©M")]
[assembly:AssemblyProduct("Sports.NET")]
[assembly:AssemblyTitle("Sports.NET DLL")]
[assembly:AssemblyVersion("1.0.*")]
namespace SportsUtil {
	public class HKJCHDA {
		const string LOGFILESUFFIX = "log";
		int m_RecordCount = 0;
		OleDbDataReader m_SportsOleReader;
		DBManager m_SportsDBMgr;
		Files m_SportsLog;
		StringBuilder SQLString;

		public HKJCHDA(string Connection) {
			m_SportsDBMgr = new DBManager();
			m_SportsDBMgr.ConnectionString = Connection;
			m_SportsLog = new Files();
			SQLString = new StringBuilder();
		}

		public int NumberOfRecords {
			get {
				return m_RecordCount;
			}
		}

		public string GetOdds() {
			int iArrayIndex = 0;
			string uid;
			string[] weekArray;
			string[] statusArray;
			StringBuilder HTMLString = new StringBuilder();

			weekArray = (string[])HttpContext.Current.Application["WeekItems"];
			statusArray = (string[])HttpContext.Current.Application["HKJCOddsStatus"];
			uid = HttpContext.Current.Session["user_id"].ToString();

			SQLString.Remove(0,SQLString.Length);
			SQLString.Append("select main.IMATCH_CNT, main.IWEEK, main.ISEQ_NO, main.MATCHDATETIME, main.CLEAGUEALIAS, main.CHOST, main.CGUEST, main.CSTATUS, (select CH_ODDS from HKJCSOCHDA_INFO where IMATCH_CNT=main.IMATCH_CNT) as CH_ODDS, (select CD_ODDS from HKJCSOCHDA_INFO where IMATCH_CNT=main.IMATCH_CNT) as CD_ODDS, (select CA_ODDS from HKJCSOCHDA_INFO where IMATCH_CNT=main.IMATCH_CNT) as CA_ODDS from HKJCSOCCER_INFO main, leaginfo leag where main.CACT='U' and main.CLEAGUEALIAS = leag.alias and leaginfo.leag_id in (select cleag_id from userprofile_info where iuser_id=");
			SQLString.Append(uid);
			SQLString.Append(") order by leag.leag_order, main.MATCHDATETIME, main.IWEEK, main.ISEQ_NO");
/*
			SQLString.Append("select main.IMATCH_CNT, main.IWEEK, main.ISEQ_NO, main.MATCHDATETIME, main.CLEAGUEALIAS, main.CHOST, main.CGUEST, main.CSTATUS, (select CH_ODDS from HKJCSOCHDA_INFO where IMATCH_CNT=main.IMATCH_CNT) as CH_ODDS, (select CD_ODDS from HKJCSOCHDA_INFO where IMATCH_CNT=main.IMATCH_CNT) as CD_ODDS, (select CA_ODDS from HKJCSOCHDA_INFO where IMATCH_CNT=main.IMATCH_CNT) as CA_ODDS from HKJCSOCCER_INFO main, leaginfo leag where main.CACT='U' and main.CLEAGUEALIAS = leag.alias and main.CLEAGUEALIAS in (select distinct leaginfo.alias from leaginfo, userprofile_info where leaginfo.leag_id in (select cleag_id from userprofile_info where iuser_id=");
			SQLString.Append(uid);
			SQLString.Append(")) order by leag.leag_order, main.MATCHDATETIME, main.IWEEK, main.ISEQ_NO");
*/
			try {
				m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
				while(m_SportsOleReader.Read()) {
					HTMLString.Append("<tr align=\"center\">");

					//Match count (Hidden Field), week, sequence number
					iArrayIndex = 0;
					HTMLString.Append("<td><input type=\"hidden\" name=\"MatchCount\" value=\"");
					HTMLString.Append(m_SportsOleReader.GetInt32(0).ToString());
					HTMLString.Append("\"><select name=\"week\"><option value=");
					HTMLString.Append(m_SportsOleReader.GetInt32(1).ToString());
					HTMLString.Append(">");
					HTMLString.Append(weekArray[m_SportsOleReader.GetInt32(1)]);
					foreach(string sItem in weekArray) {
						if(!sItem.Equals(weekArray[m_SportsOleReader.GetInt32(1)])) {
							HTMLString.Append("<option value=");
							HTMLString.Append(iArrayIndex.ToString());
							HTMLString.Append(">");
							HTMLString.Append(sItem);
						}
						iArrayIndex++;
					}
					HTMLString.Append("</select><input name=\"seqno\" value=\"");
					HTMLString.Append(m_SportsOleReader.GetInt32(2).ToString());
					HTMLString.Append("\" maxlength=\"2\" size=\"1\" onChange=\"SeqNoValidity(");
					HTMLString.Append(m_RecordCount.ToString());
					HTMLString.Append(")\"></td>");

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

					//Status
					iArrayIndex = 0;
					HTMLString.Append("<td><select name=\"status\"><option value=");
					HTMLString.Append(m_SportsOleReader.GetString(7));
					HTMLString.Append(">");
					HTMLString.Append(statusArray[Convert.ToInt32(m_SportsOleReader.GetString(7))]);
					foreach(string sItem in statusArray) {
						if(!sItem.Equals(statusArray[Convert.ToInt32(m_SportsOleReader.GetString(7))])) {
							HTMLString.Append("<option value=");
							HTMLString.Append(iArrayIndex.ToString());
							HTMLString.Append(">");
							HTMLString.Append(sItem);
						}
						iArrayIndex++;
					}
					HTMLString.Append("</select></td>");

					//Win odds
					HTMLString.Append("<td><input type=\"text\" name=\"WinOdds\" maxlength=\"4\" size=\"1\" value=\"");
					if(!m_SportsOleReader.IsDBNull(8)) HTMLString.Append(m_SportsOleReader.GetString(8).Trim());
					HTMLString.Append("\" onChange=\"WinOddsValidity(");
					HTMLString.Append(m_RecordCount.ToString());
					HTMLString.Append(")\"></td>");

					//Draw odds
					HTMLString.Append("<td><input type=\"text\" name=\"DrawOdds\" maxlength=\"4\" size=\"1\" value=\"");
					if(!m_SportsOleReader.IsDBNull(9)) HTMLString.Append(m_SportsOleReader.GetString(9).Trim());
					HTMLString.Append("\" onChange=\"DrawOddsValidity(");
					HTMLString.Append(m_RecordCount.ToString());
					HTMLString.Append(")\"></td>");

					//Loss odds
					HTMLString.Append("<td><input type=\"text\" name=\"LossOdds\" maxlength=\"4\" size=\"1\" value=\"");
					if(!m_SportsOleReader.IsDBNull(10)) HTMLString.Append(m_SportsOleReader.GetString(10).Trim());
					HTMLString.Append("\" onChange=\"LossOddsValidity(");
					HTMLString.Append(m_RecordCount.ToString());
					HTMLString.Append(")\"></td></tr>");
					m_RecordCount++;
				}
				m_SportsDBMgr.Close();
				m_SportsOleReader.Close();
				m_SportsDBMgr.Dispose();
			} catch(Exception ex) {
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " HKJCHDA.cs.GetOdds(): " + ex.ToString());
				m_SportsLog.Close();
				HTMLString.Remove(0,HTMLString.Length);
				HTMLString.Append((string)HttpContext.Current.Application["accessErrorMsg"]);
			}

			return HTMLString.ToString();
		}

		public int Modify() {
			int iUpdIndex = 0;
			char[] delimiter = new char[] {','};
			string[] arrMatchCount;
			string[] arrWeek;
			string[] arrSeqNo;
			string[] arrWinOdds;
			string[] arrDrawOdds;
			string[] arrLossOdds;
			string[] arrStatus;

			arrMatchCount = HttpContext.Current.Request.Form["MatchCount"].Split(delimiter);
			arrWeek = HttpContext.Current.Request.Form["week"].Split(delimiter);
			arrSeqNo = HttpContext.Current.Request.Form["seqno"].Split(delimiter);
			arrWinOdds = HttpContext.Current.Request.Form["WinOdds"].Split(delimiter);
			arrDrawOdds = HttpContext.Current.Request.Form["DrawOdds"].Split(delimiter);
			arrLossOdds = HttpContext.Current.Request.Form["LossOdds"].Split(delimiter);
			arrStatus = HttpContext.Current.Request.Form["status"].Split(delimiter);

			try {
				for(iUpdIndex = 0; iUpdIndex < arrMatchCount.Length; iUpdIndex++) {
					//update HKJCSOCCER_INFO
					SQLString.Remove(0,SQLString.Length);
					SQLString.Append("update HKJCSOCCER_INFO set IWEEK=");
					SQLString.Append((Convert.ToInt32(arrWeek[iUpdIndex])).ToString());
					SQLString.Append(", ISEQ_NO=");
					SQLString.Append((Convert.ToInt32(arrSeqNo[iUpdIndex])).ToString());
					SQLString.Append(", CSTATUS='");
					SQLString.Append(arrStatus[iUpdIndex]);
					SQLString.Append("' where IMATCH_CNT=");
					SQLString.Append(arrMatchCount[iUpdIndex]);
					m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
					m_SportsDBMgr.Close();

					//update HKJCSOCHDA_INFO
					SQLString.Remove(0,SQLString.Length);
					SQLString.Append("update HKJCSOCHDA_INFO set CH_ODDS=");
					if(arrWinOdds[iUpdIndex].Trim().Equals("")) {
						SQLString.Append("null");
					} else {
						SQLString.Append("'");
						SQLString.Append(arrWinOdds[iUpdIndex].Trim());
						SQLString.Append("'");
					}
					SQLString.Append(", CD_ODDS=");
					if(arrDrawOdds[iUpdIndex].Trim().Equals("")) {
						SQLString.Append("null");
					} else {
						SQLString.Append("'");
						SQLString.Append(arrDrawOdds[iUpdIndex].Trim());
						SQLString.Append("'");
					}
					SQLString.Append(", CA_ODDS=");
					if(arrLossOdds[iUpdIndex].Trim().Equals("")) {
						SQLString.Append("null");
					} else {
						SQLString.Append("'");
						SQLString.Append(arrLossOdds[iUpdIndex].Trim());
						SQLString.Append("'");
					}
					SQLString.Append(" where IMATCH_CNT=");
					SQLString.Append(arrMatchCount[iUpdIndex]);
					m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
					m_SportsDBMgr.Close();
				}
				if(iUpdIndex > 0) m_SportsDBMgr.Dispose();

				//write log
				m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " HKJCHDA.cs: Modify " + iUpdIndex.ToString() + " HDA records (" + HttpContext.Current.Session["user_name"] + ")");
				m_SportsLog.Close();
			} catch(Exception ex) {
				iUpdIndex = -1;
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " HKJCHDA.cs.Modify(): " + ex.ToString());
				m_SportsLog.Close();
			}
			return iUpdIndex;
		}
	}
}