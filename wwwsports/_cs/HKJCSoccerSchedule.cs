/*
Objective:
Import soccer schedule to HKJC Soccer
Enquiry by league, date is allowed

Last updated:
31 Oct 2003 by Chapman

C#.NET complier statement:
csc /t:library /out:..\bin\HKJCSoccerSchedule.dll /r:..\bin\DBManager.dll;..\bin\Files.dll HKJCSoccerSchedule.cs
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
[assembly:AssemblyCopyright("(c) 2003 TDSL. All rights reserved. Created on 8 August 2003.")]
[assembly:AssemblyDescription("JC足智彩 -> 匯入賽程")]
[assembly:AssemblyProduct("Sports.NET")]
[assembly:AssemblyTitle("Sports.NET DLL")]
[assembly:AssemblyVersion("1.0.*")]
namespace SportsUtil {
	public class HKJCSoccerSchedule {
		const string LOGFILESUFFIX = "log";
		int m_RecordCount = 0;
		OleDbDataReader m_SportsOleReader;
		DBManager m_SportsDBMgr;
		Files m_SportsLog;
		StringBuilder SQLString;

		public HKJCSoccerSchedule(string Connection) {
			m_SportsDBMgr = new DBManager();
			m_SportsDBMgr.ConnectionString = Connection;
			m_SportsLog = new Files();
			SQLString = new StringBuilder();
		}

		public int RecordCount {
			get {
				return m_RecordCount;
			}
		}

		public string list() {
			string sLeagID;
			string sEnquiryOut = "";
			string sMatchYear;
			string sMatchMonth;
			string sMatchDay;
			string sTempYear = "";
			string sYearOption = "";
			string sTempMonth = "";
			string sMonthOption = "";
			string sTempDate = "";
			string sDateOption = "";
			string uid;
			bool bCondition = false;
			ArrayList monthList = new ArrayList(12);
			ArrayList dateList = new ArrayList(15);
			StringBuilder HTMLString = new StringBuilder();

			uid = HttpContext.Current.Session["user_id"].ToString();
			sLeagID = HttpContext.Current.Request.QueryString["leagID"];
			sMatchYear = HttpContext.Current.Request.QueryString["Year"];
			sMatchMonth = HttpContext.Current.Request.QueryString["Month"];
			sMatchDay = HttpContext.Current.Request.QueryString["Date"];
			try {
				sEnquiryOut += "<tr><th colspan=\"4\">顯示<select name=\"leagID\"><option value=\"0000\">所有</option>";
				SQLString.Remove(0,SQLString.Length);
				SQLString.Append("select distinct leag.LEAG_ID, leag.ALIAS from LEAGINFO leag, SOCCERSCHEDULE sch where leag.leag_id in (select cleag_id from userprofile_info where iuser_id=");
				SQLString.Append(uid);
				SQLString.Append(") AND leag.ALIAS = sch.CLEAGUEALIAS order by leag.LEAG_ORDER, leag.LEAG_ID");
/*
				SQLString.Append("select distinct leaginfo.LEAG_ID, leaginfo.ALIAS from LEAGINFO, userprofile_info, SOCCERSCHEDULE where leaginfo.leag_id in (select distinct cleag_id from userprofile_info where iuser_id=");
				SQLString.Append(uid);
				SQLString.Append(") AND leaginfo.ALIAS = SOCCERSCHEDULE.CLEAGUEALIAS order by LEAG_ORDER, LEAG_ID");
*/
				m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
				while(m_SportsOleReader.Read()) {
					sEnquiryOut += "<option value=\"" + m_SportsOleReader.GetString(0).Trim() + "\">" + m_SportsOleReader.GetString(1).Trim() + "</option>";
				}
				m_SportsOleReader.Close();
				m_SportsDBMgr.Close();
				sEnquiryOut += "</select>聯賽 及 ";

				sYearOption += "<select name=\"Year\"><option value=\"0000\">所有</option>";
				sMonthOption += "<select name=\"Month\"><option value=\"0000\">所有</option>";
				sDateOption += "<select name=\"Date\"><option value=\"0000\">所有</option>";
				SQLString.Remove(0,SQLString.Length);
				SQLString.Append("select MATCHDATETIME from SOCCERSCHEDULE order by MATCHDATETIME");
				m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
				while(m_SportsOleReader.Read()) {
					if(!sTempYear.Equals(m_SportsOleReader.GetDateTime(0).ToString("yyyy"))) {
						sTempYear = m_SportsOleReader.GetDateTime(0).ToString("yyyy");
						sYearOption += "<option value=\"" + sTempYear + "\">" + sTempYear + "</option>";
					}

					sTempMonth = m_SportsOleReader.GetDateTime(0).ToString("MM");
					if(!monthList.Contains(sTempMonth)) {
						monthList.Add(sTempMonth);
						sMonthOption += "<option value=\"" + sTempMonth + "\">" + sTempMonth + "</option>";
					}

					sTempDate = m_SportsOleReader.GetDateTime(0).ToString("dd");
					if(!dateList.Contains(sTempDate)) {
						dateList.Add(sTempDate);
						sDateOption += "<option value=\"" + sTempDate + "\">" + sTempDate + "</option>";
					}
				}
				sYearOption += "</select>年 ";
				sMonthOption += "</select>月 ";
				sDateOption += "</select>日 ";
				m_SportsOleReader.Close();
				m_SportsDBMgr.Close();
				sEnquiryOut += sYearOption + sMonthOption + sDateOption + "<input type=\"button\" value=\"顯示\" onClick=\"filterSchedule('HKJCSoccerSchedule.aspx?leagID=' + ScheduleForm.leagID.value + '&Year=' + ScheduleForm.Year.value + '&Month=' + ScheduleForm.Month.value + '&Date=' + ScheduleForm.Date.value)\"></th</tr>";

				HTMLString.Append(sEnquiryOut);
				HTMLString.Append("<tr align=\"center\" style=\"background-color:#339966; color=#FFF5EE\"><th>賽事日期</th><th>聯賽</th><th>賽事</th><th><input type=\"checkbox\" name=\"all\" value=\"1\" onClick=\"selectAll()\">全選</th></tr>");
				SQLString.Remove(0,SQLString.Length);
				SQLString.Append("select MATCHDATETIME, CLEAGUEALIAS, CLEAGUE, CHOST_HANDI, CFIELD, CHOST, CGUEST, IMATCH_CNT from SOCCERSCHEDULE ");
				if(sLeagID != null) {
					if(!sLeagID.Equals("0000")) {
						SQLString.Append("where CLEAGUEALIAS=(select ALIAS from LEAGINFO where LEAG_ID='");
						SQLString.Append(sLeagID);
						SQLString.Append("') ");
						bCondition = true;
					}
				}
				if(sMatchYear != null) {
					if(!sMatchYear.Equals("0000")) {
						if(bCondition) SQLString.Append("and ");
						else SQLString.Append("where ");
						SQLString.Append("extract(YEAR from MATCHDATETIME)=");
						SQLString.Append(sMatchYear);
						SQLString.Append(" ");
						bCondition = true;
					}
				}
				if(sMatchMonth != null) {
					if(!sMatchMonth.Equals("0000")) {
						if(bCondition) SQLString.Append("and ");
						else SQLString.Append("where ");
						SQLString.Append("extract(MONTH from MATCHDATETIME)=");
						SQLString.Append(sMatchMonth);
						SQLString.Append(" ");
						bCondition = true;
					}
				}
				if(sMatchDay != null) {
					if(!sMatchDay.Equals("0000")) {
						if(bCondition) SQLString.Append("and ");
						else SQLString.Append("where ");
						SQLString.Append("extract(DAY from MATCHDATETIME)=");
						SQLString.Append(sMatchDay);
						SQLString.Append(" ");
						bCondition = true;
					}
				}
				if(bCondition) SQLString.Append(" and CSTATUS <> 'C' ");
				else SQLString.Append("where CSTATUS <> 'C' ");
				SQLString.Append("order by MATCHDATETIME, CLEAGUEALIAS");
				m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
				while(m_SportsOleReader.Read()) {
					//Match date time
					HTMLString.Append("<tr align=\"center\"><td align=\"center\">");
					HTMLString.Append(m_SportsOleReader.GetDateTime(0).ToString("yyyy/MM/dd HH:mm"));
					HTMLString.Append("<input type=\"hidden\" name=\"MatchDateTime\" value=\"");
					HTMLString.Append(m_SportsOleReader.GetDateTime(0).ToString("yyyy/MM/dd HH:mm:ss"));
					HTMLString.Append("\"></td>");

					//Alias
					HTMLString.Append("<td align=\"center\">");
					HTMLString.Append(m_SportsOleReader.GetString(1).Trim());
					HTMLString.Append("<input type=\"hidden\" name=\"Alias\" value=\"");
					HTMLString.Append(m_SportsOleReader.GetString(1).Trim());
					HTMLString.Append("\"><input type=\"hidden\" name=\"League\" value=\"");
					HTMLString.Append(m_SportsOleReader.GetString(2).Trim());
					HTMLString.Append("\"></td>");

					//String format as same as pager -> 賽事
					HTMLString.Append("<td align=\"center\">");
					if(m_SportsOleReader.GetString(3).Trim().Equals("1")) {
						if(m_SportsOleReader.GetString(4).Trim().Equals("H")) {
							HTMLString.Append("(主) ");
							HTMLString.Append(m_SportsOleReader.GetString(5));
							HTMLString.Append(" vs ");
							HTMLString.Append(m_SportsOleReader.GetString(6));
						} else {
							HTMLString.Append("(中) ");
							HTMLString.Append(m_SportsOleReader.GetString(5));
							HTMLString.Append(" vs ");
							HTMLString.Append(m_SportsOleReader.GetString(6));
						}
					} else {
						if(m_SportsOleReader.GetString(4).Trim().Equals("H")) {
							HTMLString.Append("(客) ");
							HTMLString.Append(m_SportsOleReader.GetString(6));
							HTMLString.Append(" vs ");
							HTMLString.Append(m_SportsOleReader.GetString(5));
						} else {
							HTMLString.Append("(中) ");
							HTMLString.Append(m_SportsOleReader.GetString(6));
							HTMLString.Append(" vs ");
							HTMLString.Append(m_SportsOleReader.GetString(5));
						}
					}
					HTMLString.Append("<input type=\"hidden\" name=\"HostHandicap\" value=\"");
					HTMLString.Append(m_SportsOleReader.GetString(3));
					HTMLString.Append("\"><input type=\"hidden\" name=\"MatchField\" value=\"");
					HTMLString.Append(m_SportsOleReader.GetString(4));
					HTMLString.Append("\">");
					HTMLString.Append("<input type=\"hidden\" name=\"Host\" value=\"");
					HTMLString.Append(m_SportsOleReader.GetString(5).Trim());
					HTMLString.Append("\"><input type=\"hidden\" name=\"Guest\" value=\"");
					HTMLString.Append(m_SportsOleReader.GetString(6).Trim());
					HTMLString.Append("\"></td>");

					//Import
					HTMLString.Append("<td align=\"center\"><input type=\"checkbox\" name=\"importedMatch\" value=\"");
					HTMLString.Append(m_RecordCount.ToString());
					HTMLString.Append("\">");

					//Hidden match count
					HTMLString.Append("<input type=\"hidden\" name=\"MatchCount\" value=\"");
					HTMLString.Append(m_SportsOleReader.GetInt32(7).ToString());
					HTMLString.Append("\"></td></tr>");
					m_RecordCount++;
				}
				m_SportsDBMgr.Close();
				m_SportsOleReader.Close();
				m_SportsDBMgr.Dispose();

				if(m_RecordCount == 0) {
					HTMLString.Remove(0,HTMLString.Length);
					HTMLString.Append(sEnquiryOut);
					HTMLString.Append("<tr><th colspan=\"4\">沒有賽程或已取消</th></tr>");
					if(sLeagID != null) {
						if(!sLeagID.Equals("0000")) {
							HTMLString.Remove(0,HTMLString.Length);
							HTMLString.Append(sEnquiryOut);
							HTMLString.Append("<tr><th colspan=\"4\">沒有賽程在此聯賽或已取消</th></tr>");
						}
					}
				}
			}	catch(Exception ex) {
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " HKJCSoccerSchedule.cs.list(): " + ex.ToString());
				m_SportsLog.Close();
				HTMLString.Remove(0,HTMLString.Length);
				HTMLString.Append((string)HttpContext.Current.Application["accessErrorMsg"]);
			}
			return HTMLString.ToString();
		}

		public int import() {
			int iImported = 0;
			int iImportIdx = 0;
			int iMatchCnt = 0;
			int iExisted = 0;
			int iImportCount;
			char[] delimiter = new char[] {','};
			string[] arrImport;
			string[] arrMatchDateTime;
			string[] arrAlias;
			string[] arrLeague;
			string[] arrHost;
			string[] arrGuest;
			string[] arrHostHandicap;
			string[] arrMatchField;

			try {
				arrImport = HttpContext.Current.Request.Form["importedMatch"].Split(delimiter);
				iImportCount = arrImport.Length;
				arrMatchDateTime = HttpContext.Current.Request.Form["MatchDateTime"].Split(delimiter);
				arrAlias = HttpContext.Current.Request.Form["Alias"].Split(delimiter);
				arrLeague = HttpContext.Current.Request.Form["League"].Split(delimiter);
				arrHost = HttpContext.Current.Request.Form["Host"].Split(delimiter);
				arrGuest = HttpContext.Current.Request.Form["Guest"].Split(delimiter);
				arrHostHandicap = HttpContext.Current.Request.Form["HostHandicap"].Split(delimiter);
				arrMatchField = HttpContext.Current.Request.Form["MatchField"].Split(delimiter);
			}	catch(Exception) {
				arrImport = new string[0];
				iImportCount = 0;
				arrMatchDateTime = new string[0];
				arrAlias = new string[0];
				arrLeague = new string[0];
				arrHost = new string[0];
				arrGuest = new string[0];
				arrHostHandicap = new string[0];
				arrMatchField = new string[0];
			}

			try {
				for(int itemIdx = 0; itemIdx < iImportCount; itemIdx++) {
					iImportIdx = Convert.ToInt32(arrImport[itemIdx]);
					try {
						iExisted = 0;
						SQLString.Remove(0,SQLString.Length);
						SQLString.Append("select count(IMATCH_CNT) from HKJCSOCCER_INFO where CLEAGUEALIAS='");
						SQLString.Append(arrAlias[iImportIdx]);
						SQLString.Append("' AND CLEAGUE='");
						SQLString.Append(arrLeague[iImportIdx]);
						SQLString.Append("' AND CHOST='");
						SQLString.Append(arrHost[iImportIdx]);
						SQLString.Append("' AND CGUEST='");
						SQLString.Append(arrGuest[iImportIdx]);
						SQLString.Append("' AND MATCHDATETIME='");
						SQLString.Append(arrMatchDateTime[iImportIdx]);
						SQLString.Append("'");
						iExisted = m_SportsDBMgr.ExecuteScalar(SQLString.ToString());
						m_SportsDBMgr.Close();

						if(iExisted == 0) {
							iImported++;
							SQLString.Remove(0,SQLString.Length);
							SQLString.Append("select max(IMATCH_CNT) from HKJCSOCCER_INFO");
							m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
							if(m_SportsOleReader.Read()) {
								if(!m_SportsOleReader.IsDBNull(0)) {
									iMatchCnt = m_SportsOleReader.GetInt32(0);
									iMatchCnt++;
								} else {
									iMatchCnt = 1;
								}
							} else iMatchCnt = 1;
							m_SportsOleReader.Close();
							m_SportsDBMgr.Close();

							//init HKJC Soccer Main Table
							SQLString.Remove(0,SQLString.Length);
							SQLString.Append("insert into HKJCSOCCER_INFO values(");
							SQLString.Append(iMatchCnt.ToString());
							SQLString.Append(",'");
							SQLString.Append(arrAlias[iImportIdx]);
							SQLString.Append("','");
							SQLString.Append(arrLeague[iImportIdx]);
							SQLString.Append("','");
							SQLString.Append(arrHost[iImportIdx]);
							SQLString.Append("','");
							SQLString.Append(arrGuest[iImportIdx]);
							SQLString.Append("','");
							SQLString.Append(arrMatchDateTime[iImportIdx]);
							SQLString.Append("',");
							if((DateTime.Parse(arrMatchDateTime[iImportIdx])).DayOfWeek.ToString().Equals("Saturday")) {
								SQLString.Append("6");
							} else if((DateTime.Parse(arrMatchDateTime[iImportIdx])).DayOfWeek.ToString().Equals("Sunday")) {
								SQLString.Append("0");
							} else if((DateTime.Parse(arrMatchDateTime[iImportIdx])).DayOfWeek.ToString().Equals("Monday")) {
								SQLString.Append("1");
							} else if((DateTime.Parse(arrMatchDateTime[iImportIdx])).DayOfWeek.ToString().Equals("Tuesday")) {
								SQLString.Append("2");
							} else if((DateTime.Parse(arrMatchDateTime[iImportIdx])).DayOfWeek.ToString().Equals("Wednesday")) {
								SQLString.Append("3");
							} else if((DateTime.Parse(arrMatchDateTime[iImportIdx])).DayOfWeek.ToString().Equals("Thursday")) {
								SQLString.Append("4");
							} else {
								SQLString.Append("5");
							}
							SQLString.Append(",");
							SQLString.Append(iMatchCnt.ToString());
							SQLString.Append(",'");
							SQLString.Append(arrMatchField[iImportIdx]);
							SQLString.Append("','");
							SQLString.Append(arrHostHandicap[iImportIdx]);
							SQLString.Append("','1','U')");
							m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
							m_SportsDBMgr.Close();

							iExisted = 0;
							SQLString.Remove(0,SQLString.Length);
							SQLString.Append("select count(IMATCH_CNT) from HKJCSOCHDA_INFO where IMATCH_CNT=");
							SQLString.Append(iMatchCnt.ToString());
							iExisted = m_SportsDBMgr.ExecuteScalar(SQLString.ToString());
							m_SportsDBMgr.Close();
							if(iExisted == 0) {
								//init HKJC Soccer HDA
								SQLString.Remove(0,SQLString.Length);
								SQLString.Append("insert into HKJCSOCHDA_INFO values(");
								SQLString.Append(iMatchCnt.ToString());
								SQLString.Append(",null,null,null)");
								m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
								m_SportsDBMgr.Close();
							}

							iExisted = 0;
							SQLString.Remove(0,SQLString.Length);
							SQLString.Append("select count(IMATCH_CNT) from HKJCSOCCRS_INFO where IMATCH_CNT=");
							SQLString.Append(iMatchCnt.ToString());
							iExisted = m_SportsDBMgr.ExecuteScalar(SQLString.ToString());
							m_SportsDBMgr.Close();
							if(iExisted == 0) {
								//init HKJC Soccer CRS
								for(int i = 0; i < 3; i++) {
									SQLString.Remove(0,SQLString.Length);
									SQLString.Append("insert into HKJCSOCCRS_INFO values(");
									SQLString.Append(iMatchCnt.ToString());
									SQLString.Append(",-1,");
									SQLString.Append(i.ToString());
									SQLString.Append(",null)");
									m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
									m_SportsDBMgr.Close();
								}
								for(int i = 0; i < 5; i++) {
									for(int j = 0; j < 5; j++) {
										SQLString.Remove(0,SQLString.Length);
										SQLString.Append("insert into HKJCSOCCRS_INFO values(");
										SQLString.Append(iMatchCnt.ToString());
										SQLString.Append(",");
										SQLString.Append(i.ToString());
										SQLString.Append(",");
										SQLString.Append(j.ToString());
										SQLString.Append(",null)");
										m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
										m_SportsDBMgr.Close();
									}
								}
							}

							iExisted = 0;
							SQLString.Remove(0,SQLString.Length);
							SQLString.Append("select count(IMATCH_CNT) from HKJCSOCTTG_INFO where IMATCH_CNT=");
							SQLString.Append(iMatchCnt.ToString());
							iExisted = m_SportsDBMgr.ExecuteScalar(SQLString.ToString());
							m_SportsDBMgr.Close();
							if(iExisted == 0) {
								//init HKJC Soccer TTG
								for(int i = 0; i < 8; i++) {
									SQLString.Remove(0,SQLString.Length);
									SQLString.Append("insert into HKJCSOCTTG_INFO values(");
									SQLString.Append(iMatchCnt.ToString());
									SQLString.Append(",");
									SQLString.Append(i.ToString());
									SQLString.Append(",null)");
									m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
									m_SportsDBMgr.Close();
								}
							}
						}
					} catch(OleDbException dbEx) {
						m_SportsDBMgr.Close();
						m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
						m_SportsLog.SetFileName(0,LOGFILESUFFIX);
						m_SportsLog.Open();
						m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " HKJCSoccerSchedule.cs.import(): Duplicate record existed.");
						m_SportsLog.Write(dbEx.ToString());
						m_SportsLog.Close();
					}
				}
				if(iImported > 0) m_SportsDBMgr.Dispose();

				//write log
				m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " HKJCSoccerSchedule.cs.import(): Import " + iImported.ToString() + " matches (" + HttpContext.Current.Session["user_name"] + ")");
				m_SportsLog.Close();
			} catch(Exception ex) {
				iImported = -1;
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " HKJCSoccerSchedule.cs.import(): " + ex.ToString());
				m_SportsLog.Close();
			}

			return iImported;
		}
	}
}