/*
Objective:
Retrieval and modify the odds of correct score for JC Soccer

Last updated:
15 August 2003 by Chapman

C#.NET complier statement:
csc /t:library /out:..\bin\HKJCCRS.dll /r:..\bin\DBManager.dll;..\bin\Files.dll HKJCCRS.cs
*/

using System;
using System.Data.OleDb;
using System.Reflection;
using System.Text;
using System.Web;
using TDL.DB;
using TDL.IO;

[assembly:AssemblyCompany("TDSL")]
[assembly:AssemblyCopyright("(c) 2003 TDSL. All rights reserved. Created on 11 August 2003.")]
[assembly:AssemblyDescription("JC足智彩 -> 波膽")]
[assembly:AssemblyProduct("Sports.NET")]
[assembly:AssemblyTitle("Sports.NET DLL")]
[assembly:AssemblyVersion("1.0.*")]
namespace SportsUtil {
	public class HKJCCRS {
		const string LOGFILESUFFIX = "log";
		DBManager m_SportsDBMgr;
		Files m_SportsLog;
		StringBuilder SQLString;

		public HKJCCRS(string Connection) {
			m_SportsDBMgr = new DBManager();
			m_SportsDBMgr.ConnectionString = Connection;
			m_SportsLog = new Files();
			SQLString = new StringBuilder();
		}

		public string GetCRS() {
			int iRecordCount = 0;
			int iSeqNo = 0;
			string sMatchCount;
			string sLeague = "";
			string sHost = "";
			string sGuest = "";
			string sMatchField = "(主)";
			string sMatchDateTime = "";
			string sWeek = "";
			string[] weekArray;
			string[] OtherOddsArray = new string[3];
			OleDbDataReader SportsOleReader;
			StringBuilder HTMLString = new StringBuilder();

			sMatchCount = HttpContext.Current.Request.QueryString["matchcnt"].Trim();
			weekArray = (string[])HttpContext.Current.Application["WeekItems"];
			try {
				//select for League, Host, Guest
				SQLString.Remove(0,SQLString.Length);
				SQLString.Append("select CLEAGUE, CHOST, CGUEST, IWEEK, ISEQ_NO, CFIELD, MATCHDATETIME from HKJCSOCCER_INFO where IMATCH_CNT=");
				SQLString.Append(sMatchCount);
				SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
				if(SportsOleReader.Read()) {
					sLeague = SportsOleReader.GetString(0).Trim();
					sHost = SportsOleReader.GetString(1).Trim();
					sGuest = SportsOleReader.GetString(2).Trim();
					sWeek = weekArray[SportsOleReader.GetInt32(3)];
					iSeqNo = SportsOleReader.GetInt32(4);
					if(SportsOleReader.GetString(5).Equals("M")) sMatchField = "(中)";
					sMatchDateTime = SportsOleReader.GetDateTime(6).ToString("yyyy/MM/dd HH:mm");
				}
				SportsOleReader.Close();
				m_SportsDBMgr.Close();

				//Get other CRS odds in sequence
				int iIdx = 0;
				SQLString.Remove(0,SQLString.Length);
				SQLString.Append("select CODDS from HKJCSOCCRS_INFO where IMATCH_CNT=");
				SQLString.Append(sMatchCount);
				SQLString.Append(" AND IHOME_SCORE=-1 order by IAWAY_SCORE");
				SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
				while(SportsOleReader.Read()) {
					if(!SportsOleReader.IsDBNull(0)) OtherOddsArray[iIdx] = SportsOleReader.GetString(0).Trim();
					else OtherOddsArray[iIdx] = "";
					iIdx++;
				}
				SportsOleReader.Close();
				m_SportsDBMgr.Close();

				HTMLString.Append("<tr align=\"center\" style=\"background-color:#EBFFD7\"><th colspan=\"2\">JC足智彩 - 波膽</th><th colspan=\"6\" align=\"left\">球賽編號: ");
				HTMLString.Append(sWeek);
				HTMLString.Append(" ");
				HTMLString.Append(iSeqNo.ToString());
				HTMLString.Append(", ");
				HTMLString.Append(sMatchDateTime);
				HTMLString.Append(", ");
				HTMLString.Append(sLeague);
				HTMLString.Append("</th></tr><tr align=\"center\"><th colspan=\"2\"></th><th>0</th><th>1</th><th>2</th><th>3</th><th>4</th><th>&gt;4</th></tr>");

				HTMLString.Append("<tr align=\"center\">");
				HTMLString.Append("<th width=\"15%\" rowspan=\"6\">");
				HTMLString.Append(sMatchField);
				HTMLString.Append(" ");
				HTMLString.Append(sHost);
				HTMLString.Append("</th>");
				//Get CRS odds in sequence
				SQLString.Remove(0,SQLString.Length);
				SQLString.Append("select CODDS from HKJCSOCCRS_INFO where IMATCH_CNT=");
				SQLString.Append(sMatchCount);
				SQLString.Append(" AND IHOME_SCORE<>-1 order by IHOME_SCORE, IAWAY_SCORE");
				SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
				while(SportsOleReader.Read()) {
					if(iRecordCount%5 == 0) {
						if(iRecordCount != 0) {
							HTMLString.Append("<tr align=\"center\">");
						}
						HTMLString.Append("<th>");
						HTMLString.Append((iRecordCount/5).ToString());
						HTMLString.Append("</th>");
					}
					HTMLString.Append("<td><input name=\"odds\" value=\"");
					if(!SportsOleReader.IsDBNull(0)) HTMLString.Append(SportsOleReader.GetString(0).Trim());
					HTMLString.Append("\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(");
					HTMLString.Append(iRecordCount.ToString());
					HTMLString.Append(")\"></td>");

					if((iRecordCount+1)%5 == 0) {
						HTMLString.Append("<td>");
						if(iRecordCount == 4) {	//input for AO
							HTMLString.Append("<input name=\"AO_Odds\" value=\"");
							HTMLString.Append(OtherOddsArray[1]);
							HTMLString.Append("\" size=\"1\" maxlength=\"5\" onChange=\"AOOddsValidity()\">");
						}
						HTMLString.Append("</td></tr>");
					}
					iRecordCount++;
				}
				SportsOleReader.Close();
				m_SportsDBMgr.Close();
				m_SportsDBMgr.Dispose();
				HTMLString.Append("<tr align=\"center\">");
				HTMLString.Append("<th>&gt;4</th>");
				HTMLString.Append("<td><input name=\"HO_Odds\" value=\"");
				HTMLString.Append(OtherOddsArray[0]);
				HTMLString.Append("\" size=\"1\" maxlength=\"5\" onChange=\"HOOddsValidity()\"></td>");
				HTMLString.Append("<td colspan=\"4\"></td>");
				HTMLString.Append("<td><input name=\"DO_Odds\" value=\"");
				HTMLString.Append(OtherOddsArray[2]);
				HTMLString.Append("\" size=\"1\" maxlength=\"5\" onChange=\"DOOddsValidity()\"></td></tr>");
				HTMLString.Append("<tr align=\"center\"><th colspan=\"8\">(客) ");
				HTMLString.Append(sGuest);
				HTMLString.Append("</th></tr>");
				HTMLString.Append("<input type=\"hidden\" name=\"matchcount\" value=\"");
				HTMLString.Append(sMatchCount);
				HTMLString.Append("\">");
			}	catch(Exception ex) {
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " HKJCCRS.cs.GetCRS(): " + ex.ToString());
				m_SportsLog.Close();
				HTMLString.Remove(0,HTMLString.Length);
				HTMLString.Append((string)HttpContext.Current.Application["accessErrorMsg"]);
			}

			return HTMLString.ToString();
		}

		public int Modify() {
			int iScoreCount;
			string sMatchCount;
			char[] delimiter = new char[] {','};
			string[] arrOdds;
			string[] OtherOddsArray = new string[3];

			sMatchCount = HttpContext.Current.Request.Form["matchcount"];
			OtherOddsArray[0] = HttpContext.Current.Request.Form["HO_Odds"];
			OtherOddsArray[1] = HttpContext.Current.Request.Form["AO_Odds"];
			OtherOddsArray[2] = HttpContext.Current.Request.Form["DO_Odds"];
			arrOdds = HttpContext.Current.Request.Form["odds"].Split(delimiter);
			iScoreCount = arrOdds.Length + OtherOddsArray.Length;
			try {
				//Update other CRS odds - HO, AO, DO
				for(int i = 0; i < OtherOddsArray.Length; i++) {
					SQLString.Remove(0,SQLString.Length);
					SQLString.Append("update HKJCSOCCRS_INFO set CODDS=");
					if(OtherOddsArray[i].Trim().Equals("")) {
						SQLString.Append("null");
					} else {
						SQLString.Append("'");
						SQLString.Append(OtherOddsArray[i].Trim());
						SQLString.Append("'");
					}
					SQLString.Append(" where IMATCH_CNT=");
					SQLString.Append(sMatchCount);
					SQLString.Append(" AND IHOME_SCORE=-1 AND IAWAY_SCORE=");
					SQLString.Append(i.ToString());
					m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
					m_SportsDBMgr.Close();
				}

				//update CRS odds
				for(int j = 0; j < 5; j++) {
					for(int k = 0; k < 5; k++) {
						SQLString.Remove(0,SQLString.Length);
						SQLString.Append("update HKJCSOCCRS_INFO set CODDS=");
						if(arrOdds[(5*j+k)].Trim().Equals("")) {
							SQLString.Append("null");
						} else {
							SQLString.Append("'");
							SQLString.Append(arrOdds[(5*j+k)].Trim());
							SQLString.Append("'");
						}
						SQLString.Append(" where IMATCH_CNT=");
						SQLString.Append(sMatchCount);
						SQLString.Append(" AND IHOME_SCORE=");
						SQLString.Append(j.ToString());
						SQLString.Append(" AND IAWAY_SCORE=");
						SQLString.Append(k.ToString());
						m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
						m_SportsDBMgr.Close();
					}
				}
				m_SportsDBMgr.Dispose();

				//write log
				m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " HKJCCRS.cs: Update " + iScoreCount.ToString() + " CRS records, Match Count = " + sMatchCount + " (" + HttpContext.Current.Session["user_name"] + ")");
				m_SportsLog.Close();
			}	catch(Exception ex) {
				iScoreCount = -1;
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " HKJCCRS.cs.Modify(): " + ex.ToString());
				m_SportsLog.Close();
			}

			return iScoreCount;
		}
	}
}