/*
Objective:
Import other region matches from Asia Sports

Last updated:
4 August 2003 by Chapman

C#.NET complier statement:
csc /t:library /out:..\bin\RegionImport.dll /r:..\bin\DBManager.dll;..\bin\Files.dll RegionImport.cs
*/

using System;
using System.Data.OleDb;
using System.Reflection;
using System.Text;
using System.Web;
using System.Collections;
using TDL.DB;
using TDL.IO;

[assembly:AssemblyCompany("TDSL")]
[assembly:AssemblyCopyright("(c) 2003 TDSL. All rights reserved.")]
[assembly:AssemblyDescription("其他地區 -> 匯入賽事")]
[assembly:AssemblyProduct("Sports.NET")]
[assembly:AssemblyTitle("Sports.NET DLL")]
[assembly:AssemblyVersion("1.0.*")]
namespace SportsUtil {
	public class RegionImport {
		const string LOGFILESUFFIX = "log";
		int m_RecordCount = 0;
		string m_Region;
		OleDbDataReader m_SportsOleReader;
		DBManager m_SportsDBMgr;
		Files m_SportsLog;
		StringBuilder SQLString;

		public RegionImport(string Connection) {
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

		public string Region {
			get {
				return m_Region;
			}
		}

		public string GetAsiaMatches() {
			string sRecordStr;
			string sRegionID;			
			string sEnquiryOut = "";
			string sLeagID;
			string sMatchYear;
			string sMatchMonth;
			string sMatchDay;
			string sTempYear = "";
			string sYearOption = "";
			string sTempMonth = "";
			string sMonthOption = "";
			string sTempDate = "";
			string sDateOption = "";
			ArrayList monthList = new ArrayList(12);
			ArrayList dateList = new ArrayList(15);
			string uid;
			StringBuilder HTMLString = new StringBuilder();

			try {
				uid = HttpContext.Current.Session["user_id"].ToString();
				sLeagID = HttpContext.Current.Request.QueryString["leagID"];
				sMatchYear = HttpContext.Current.Request.QueryString["Year"];
				sMatchMonth = HttpContext.Current.Request.QueryString["Month"];
				sMatchDay = HttpContext.Current.Request.QueryString["Date"];
				sRegionID = HttpContext.Current.Request.QueryString["RegionID"];
				
				SQLString.Remove(0,SQLString.Length);
				SQLString.Append("select CREGION from OTHERREGION_CFG where IREGION_ID=");
				SQLString.Append(sRegionID);
				m_Region = m_SportsDBMgr.ExecuteQueryString(SQLString.ToString());
				m_SportsDBMgr.Close();
				
				// Sorting
				sEnquiryOut += "<tr><th colspan=\"6\">顯示<select name=\"leagID\"><option value=\"0000\">所有</option>";
				SQLString.Remove(0,SQLString.Length);
				SQLString.Append("select distinct leag.LEAG_ID, leag.ALIAS from LEAGINFO leag, SOCCERSCHEDULE sch where leag.leag_id in (select cleag_id from userprofile_info where iuser_id=");
				SQLString.Append(uid);
				SQLString.Append(") AND leag.leag_id = sch.cleag_id order by leag.LEAG_ORDER, leag.LEAG_ID");
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
				
				sEnquiryOut += sYearOption + sMonthOption + sDateOption + "<input type=\"button\" value=\"顯示\" ";
				sEnquiryOut += "onClick=\"filterSchedule('RegionImport.aspx?RegionID=' +"+sRegionID+"+ '&leagID=' + ImportRegionForm.leagID.value + '&Year=' + ImportRegionForm.Year.value + '&Month=' + ImportRegionForm.Month.value + '&Date=' + ImportRegionForm.Date.value)\"></th</tr>";
				
				HTMLString.Append(sEnquiryOut);
				
				HTMLString.Append("<tr style=\"background-color:#00BFFF\">");
				HTMLString.Append("<th>日期</th>");
				HTMLString.Append("<th>時間</th>");
				HTMLString.Append("<th>聯賽</th>");
				HTMLString.Append("<th>主隊</th>");
				HTMLString.Append("<th>客隊</th>");
				HTMLString.Append("<th>全選<input type=\"checkbox\" name=\"SelectAllSend\" onClick=\"selectAll()\"></th>");
				HTMLString.Append("</tr>");

				SQLString.Remove(0,SQLString.Length);
				SQLString.Append("select sch.MATCHDATETIME, leag.ALIAS, leag.LEAGNAME, (select teamname from teaminfo where team_id=sch.ihost_team_id) CHOST, (select teamname from teaminfo where team_id=sch.iguest_team_id) CGUEST, sch.CFIELD, sch.CHOST_HANDI from SOCCERSCHEDULE sch, LEAGINFO leag ");
				SQLString.Append("where sch.cleag_id=leag.leag_id ");
				if(sLeagID != null) {
					if(!sLeagID.Equals("0000")) {
						SQLString.Append("and sch.cleag_id='");
						SQLString.Append(sLeagID);
						SQLString.Append("' ");
					}
				}
				if(sMatchYear != null) {
					if(!sMatchYear.Equals("0000")) {
						SQLString.Append("and extract(YEAR from MATCHDATETIME)=");
						SQLString.Append(sMatchYear);
						SQLString.Append(" ");
					}
				}
				if(sMatchMonth != null) {
					if(!sMatchMonth.Equals("0000")) {
						SQLString.Append("and extract(MONTH from MATCHDATETIME)=");
						SQLString.Append(sMatchMonth);
						SQLString.Append(" ");
					}
				}
				if(sMatchDay != null) {
					if(!sMatchDay.Equals("0000")) {
						SQLString.Append("and extract(DAY from MATCHDATETIME)=");
						SQLString.Append(sMatchDay);
						SQLString.Append(" ");
					}
				}
				//SQLString.Append("order by leag.LEAG_ORDER, game.MATCHDATE, game.MATCHTIME");
				SQLString.Append("and CSTATUS <> 'C' ");
				SQLString.Append("order by MATCHDATETIME, ALIAS");
				m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
				while(m_SportsOleReader.Read()) {

					//Match date time
					HTMLString.Append("<tr align=\"center\"><td align=\"center\">");
					HTMLString.Append(m_SportsOleReader.GetDateTime(0).ToString("yyyy/MM/dd"));
					HTMLString.Append("</td><td align=\"center\">");
					HTMLString.Append(m_SportsOleReader.GetDateTime(0).ToString("HH:mm"));
					HTMLString.Append("<input type=\"hidden\" name=\"MatchDate\" value=\"");
					HTMLString.Append(m_SportsOleReader.GetDateTime(0).ToString("yyyyMMdd"));
					HTMLString.Append("\"><input type=\"hidden\" name=\"MatchTime\" value=\"");
					HTMLString.Append(m_SportsOleReader.GetDateTime(0).ToString("HHmm"));
					HTMLString.Append("\"></td>");

					//Alias and League
					sRecordStr = m_SportsOleReader.GetString(1).Trim();
					HTMLString.Append("<td><input type=\"hidden\" name=\"Alias\" value=\"");
					HTMLString.Append(sRecordStr);
					HTMLString.Append("\">");
					sRecordStr = m_SportsOleReader.GetString(2).Trim();
					HTMLString.Append(sRecordStr);
					HTMLString.Append("<input type=\"hidden\" name=\"League\" value=\"");
					HTMLString.Append(sRecordStr);
					HTMLString.Append("\"></td>");

					//Host
					HTMLString.Append("<td>");
					try {
						sRecordStr = m_SportsOleReader.GetString(3).Trim();
						HTMLString.Append(sRecordStr);
					}catch{
						sRecordStr = "N\\A";
						HTMLString.Append("<FONT COLOR=\"#ff0000\">");
						HTMLString.Append(sRecordStr);
						HTMLString.Append("</FONT>");
					}
					HTMLString.Append("<input type=\"hidden\" name=\"Host\" value=\"");
					HTMLString.Append(sRecordStr);
					HTMLString.Append("\"></td>");

					//Guest
					HTMLString.Append("<td>");
					try {
						sRecordStr = m_SportsOleReader.GetString(4).Trim();
						HTMLString.Append(sRecordStr);
					}catch{
						sRecordStr = "N\\A";
						HTMLString.Append("<FONT COLOR=\"#ff0000\">");
						HTMLString.Append(sRecordStr);
						HTMLString.Append("</FONT>");
					}
					HTMLString.Append("<input type=\"hidden\" name=\"Guest\" value=\"");
					HTMLString.Append(sRecordStr);
					HTMLString.Append("\">");

					//Match Field (Hidden field)
					sRecordStr = "";
					if(!m_SportsOleReader.IsDBNull(5)) {
						sRecordStr = m_SportsOleReader.GetString(5).Trim();
						if(sRecordStr.Equals("-1")) sRecordStr = "";
					}
					HTMLString.Append("<input type=\"hidden\" name=\"MatchField\" value=\"");
					HTMLString.Append(sRecordStr);
					HTMLString.Append("\">");

					sRecordStr = "";
					if(!m_SportsOleReader.IsDBNull(6)) {
						sRecordStr = m_SportsOleReader.GetString(6).Trim();
						if(sRecordStr.Equals("-1")) sRecordStr = "";
					}
					HTMLString.Append("<input type=\"hidden\" name=\"HostHandicap\" value=\"");
					HTMLString.Append(sRecordStr);
					HTMLString.Append("\"></td>");
					
					HTMLString.Append("<td><input type=\"checkbox\" name=\"importItem\" value=\"");
					HTMLString.Append(m_RecordCount);
					HTMLString.Append("\"></td></tr>");
					m_RecordCount++;
				}
				m_SportsDBMgr.Close();
				m_SportsOleReader.Close();
				m_SportsDBMgr.Dispose();
				HTMLString.Append("<input type=\"hidden\" name=\"RecordCount\" value=\"");
				HTMLString.Append(m_RecordCount.ToString());
				HTMLString.Append("\"><input type=\"hidden\" name=\"RegionID\" value=\"");
				HTMLString.Append(sRegionID);
				HTMLString.Append("\">");
			} catch(Exception ex) {
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " RegionImport.cs.GetAsiaMatches(): " + ex.ToString());
				m_SportsLog.Close();
				HTMLString.Remove(0,HTMLString.Length);
				HTMLString.Append((string)HttpContext.Current.Application["accessErrorMsg"]);
			}

			return HTMLString.ToString();
		}

		public int Import() {
			int iRecIndex, iSendRequiredLen, iMatchCnt = 0, iExisted = 0, iUpdIndex = 0, iSuccessUpd = 0;
			string sRegion, sRegionID, sMatchDate, sMatchTime, sLeague, sAlias, sHost, sGuest, sMatchField, sHostHandicap;
			char[] delimiter = new char[] {','};
			string[] arrSendRequired, arrMatchDate, arrMatchTime, arrLeague, arrAlias, arrHost, arrGuest;
			string[] arrMatchField, arrHostHandicap;

			sRegionID = HttpContext.Current.Request.Form["RegionID"];
			try {
				SQLString.Remove(0,SQLString.Length);
				SQLString.Append("select CREGION from OTHERREGION_CFG where IREGION_ID=");
				SQLString.Append(sRegionID);
				sRegion = m_SportsDBMgr.ExecuteQueryString(SQLString.ToString());
				try {
					arrSendRequired = HttpContext.Current.Request.Form["importItem"].Split(delimiter);
					iSendRequiredLen = arrSendRequired.Length;
				} catch(Exception) {
					arrSendRequired = new string[0];
					iSendRequiredLen = 0;
				}
				arrMatchDate = HttpContext.Current.Request.Form["MatchDate"].Split(delimiter);
				arrMatchTime = HttpContext.Current.Request.Form["MatchTime"].Split(delimiter);
				arrAlias = HttpContext.Current.Request.Form["Alias"].Split(delimiter);
				arrLeague = HttpContext.Current.Request.Form["League"].Split(delimiter);
				arrHost = HttpContext.Current.Request.Form["Host"].Split(delimiter);
				arrGuest = HttpContext.Current.Request.Form["Guest"].Split(delimiter);
				arrHostHandicap = HttpContext.Current.Request.Form["HostHandicap"].Split(delimiter);
				arrMatchField = HttpContext.Current.Request.Form["MatchField"].Split(delimiter);

				for(iRecIndex=0; iRecIndex<iSendRequiredLen; iRecIndex++) {
					iUpdIndex = Convert.ToInt32(arrSendRequired[iRecIndex]);
					sMatchDate = arrMatchDate[iUpdIndex];
					sMatchTime = arrMatchTime[iUpdIndex];
					sLeague = arrLeague[iUpdIndex];
					sHost = arrHost[iUpdIndex];
					sGuest = arrGuest[iUpdIndex];
					sAlias = arrAlias[iUpdIndex];
					sHostHandicap = arrHostHandicap[iUpdIndex];
					sMatchField = arrMatchField[iUpdIndex];

					SQLString.Remove(0,SQLString.Length);
					SQLString.Append("select count(MATCH_CNT) from OTHERODDSINFO where COMPANY='");
					SQLString.Append(sRegion);
					SQLString.Append("' and LEAGUE='");
					SQLString.Append(sLeague);
					SQLString.Append("' and HOST='");
					SQLString.Append(sHost);
					SQLString.Append("' and GUEST='");
					SQLString.Append(sGuest);
					SQLString.Append("'");
					iExisted = m_SportsDBMgr.ExecuteScalar(SQLString.ToString());
					if(iExisted == 0) {
						SQLString.Remove(0,SQLString.Length);
						SQLString.Append("select max(MATCH_CNT) from OTHERODDSINFO");
						m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
						if(m_SportsOleReader.Read()) {
							if(!m_SportsOleReader.IsDBNull(0)) {
								iMatchCnt = m_SportsOleReader.GetInt32(0);
							}
						}
						m_SportsDBMgr.Close();
						m_SportsOleReader.Close();
						iMatchCnt++;

						SQLString.Remove(0,SQLString.Length);
						SQLString.Append("INSERT INTO OTHERODDSINFO values ('");
						SQLString.Append(sRegion);
						SQLString.Append("','");
						SQLString.Append(sLeague);
						SQLString.Append("','");
						SQLString.Append(sAlias);
						SQLString.Append("','");
						SQLString.Append(sHost);
						SQLString.Append("','");
						SQLString.Append(sGuest);
						SQLString.Append("','");
						SQLString.Append(iMatchCnt.ToString());
						SQLString.Append("','U','");
						SQLString.Append(sMatchDate);
						SQLString.Append("','");
						SQLString.Append(sMatchTime);
						SQLString.Append("','");
						SQLString.Append(sMatchField);
						SQLString.Append("','");
						SQLString.Append(sHostHandicap);
						SQLString.Append("','','','','','','','','0','0','V','-1','1')");
						m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
						iSuccessUpd++;
					}
				}
				m_SportsDBMgr.Dispose();

				//write log
				m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " RegionImport.cs: Import " + iSuccessUpd.ToString() + " matches from Asia Sports (" + HttpContext.Current.Session["user_name"] + ")");
				m_SportsLog.Close();
			} catch(Exception ex) {
				iSuccessUpd = -1;
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " RegionImport.cs.Import(): " + ex.ToString());
				m_SportsLog.Close();
			}
			return iSuccessUpd;
		}
	}
}