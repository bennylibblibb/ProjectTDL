/*
Objective:
Preview and send HKJC Soccer records

Last updated:
24 Sep 2003 by Chapman

C#.NET complier statement:
csc /t:library /out:..\bin\HKJCDeletePage.dll /r:..\bin\DBManager.dll;..\bin\Files.dll HKJCDeletePage.cs
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
[assembly:AssemblyCopyright("(c) 2003 TDSL. All rights reserved. Created on 14 August 2003.")]
[assembly:AssemblyDescription("JC¨¬´¼±m -> §R°£ÁÉ¨Æ")]
[assembly:AssemblyProduct("Sports.NET")]
[assembly:AssemblyTitle("Sports.NET DLL")]
[assembly:AssemblyVersion("1.0.*")]
namespace SportsUtil {
	public class HKJCDeletePage {
		const string LOGFILESUFFIX = "log";
		const string COMMA = ",";
		int iRecordsInPage = 0;
		OleDbDataReader m_SportsOleReader;
		DBManager m_SportsDBMgr;
		Files m_SportsLog;
		StringBuilder SQLString;

		public HKJCDeletePage(string Connection) {
			m_SportsDBMgr = new DBManager();
			m_SportsDBMgr.ConnectionString = Connection;
			m_SportsLog = new Files();
			SQLString = new StringBuilder();
		}

		public int NumberOfRecords {
			get {
				return iRecordsInPage;
			}
		}

		public string Preview() {
			string sMatchCount = "";
			string uid;
			string[] weekArray;
			StringBuilder HTMLString = new StringBuilder();

			try {
				weekArray = (string[])HttpContext.Current.Application["WeekItems"];
				uid = HttpContext.Current.Session["user_id"].ToString();

				SQLString.Remove(0,SQLString.Length);
				SQLString.Append("select main.IMATCH_CNT, main.IWEEK, main.ISEQ_NO, main.MATCHDATETIME, main.CLEAGUEALIAS, main.CHOST, main.CGUEST from HKJCSOCCER_INFO main, leaginfo leag where main.CACT='U' and main.CLEAGUEALIAS = leag.alias and leaginfo.leag_id in (select cleag_id from userprofile_info where iuser_id=");
				SQLString.Append(uid);
				SQLString.Append(") group by main.CLEAGUEALIAS, main.MATCHDATETIME, main.IWEEK, main.ISEQ_NO, main.IMATCH_CNT,main.CHOST, main.CGUEST order by main.MATCHDATETIME, main.CLEAGUEALIAS, main.IWEEK, main.ISEQ_NO, main.IMATCH_CNT,main.CHOST, main.CGUEST");
/*
				SQLString.Append("select main.IMATCH_CNT, main.IWEEK, main.ISEQ_NO, main.MATCHDATETIME, main.CLEAGUEALIAS, main.CHOST, main.CGUEST from HKJCSOCCER_INFO main, leaginfo leag where main.CACT='U' and main.CLEAGUEALIAS = leag.alias and main.CLEAGUEALIAS in (select distinct leaginfo.alias from leaginfo, userprofile_info where leaginfo.leag_id in (select cleag_id from userprofile_info where iuser_id=");
				SQLString.Append(uid);
				SQLString.Append(")) group by main.CLEAGUEALIAS, main.MATCHDATETIME, main.IWEEK, main.ISEQ_NO, main.IMATCH_CNT,main.CHOST, main.CGUEST order by main.MATCHDATETIME, main.CLEAGUEALIAS, main.IWEEK, main.ISEQ_NO, main.IMATCH_CNT,main.CHOST, main.CGUEST");
*/
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

					//Delete checkbox
					HTMLString.Append("<td><input type=\"checkbox\" name=\"ItemToDelete\" value=\"");
					HTMLString.Append(sMatchCount);
					HTMLString.Append("\"></td></tr>");
					iRecordsInPage++;
				}
				m_SportsOleReader.Close();
				m_SportsDBMgr.Close();
				m_SportsDBMgr.Dispose();
			} catch(Exception ex) {
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " HKJCDeletePage.cs.Preview(): " + ex.ToString());
				m_SportsLog.Close();
				HTMLString.Remove(0,HTMLString.Length);
				HTMLString.Append((string)HttpContext.Current.Application["accessErrorMsg"]);
			}

			return HTMLString.ToString();
		}

		public int Delete() {
			int iIdx = 0;
			string sMatchCount = "";
			char[] delimiter = new char[] {','};
			string[] arrMatchCnt;
			string[] arrItemsSend;

			arrMatchCnt = HttpContext.Current.Request.Form["MatchCount"].Split(delimiter);
			try {
				arrItemsSend = HttpContext.Current.Request.Form["ItemToDelete"].Split(delimiter);
			}	catch(Exception) {
				arrItemsSend = new string[0];
			}

			try {
				//Delete background information
				if(arrItemsSend.Length>0) {
					for(iIdx=0;iIdx<arrItemsSend.Length;iIdx++) {
						sMatchCount = arrItemsSend[iIdx].Trim();
						SQLString.Remove(0,SQLString.Length);
						SQLString.Append("delete from HKJCSOCTTG_INFO where IMATCH_CNT=");
						SQLString.Append(sMatchCount);
						m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
						m_SportsDBMgr.Close();

						SQLString.Remove(0,SQLString.Length);
						SQLString.Append("delete from HKJCSOCHDA_INFO where IMATCH_CNT=");
						SQLString.Append(sMatchCount);
						m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
						m_SportsDBMgr.Close();

						SQLString.Remove(0,SQLString.Length);
						SQLString.Append("delete from HKJCSOCCRS_INFO where IMATCH_CNT=");
						SQLString.Append(sMatchCount);
						m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
						m_SportsDBMgr.Close();

						SQLString.Remove(0,SQLString.Length);
						SQLString.Append("delete from HKJCSOCCER_INFO where IMATCH_CNT=");
						SQLString.Append(sMatchCount);
						m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
						m_SportsDBMgr.Close();
					}
					if(iIdx > 0) m_SportsDBMgr.Dispose();
					//write log
					m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
					m_SportsLog.SetFileName(0,LOGFILESUFFIX);
					m_SportsLog.Open();
					m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " HKJCDeletePage.cs: Delete " + iIdx.ToString() + " matchess (" + HttpContext.Current.Session["user_name"] + ")");
					m_SportsLog.Close();
				}
			}	catch(Exception ex) {
				iIdx = -1;
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " HKJCDeletePage.cs.Delete(): " + ex.ToString());
				m_SportsLog.Close();
			}

			return iIdx;
		}
	}
}