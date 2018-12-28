/*
Objective:
Add a new match in other region

Last updated:
4 August 2003 by Chapman

C#.NET complier statement:
csc /t:library /out:..\bin\RegionNew.dll /r:..\bin\DBManager.dll;..\bin\Files.dll RegionNew.cs
*/

using System;
using System.Collections;
using System.Data.OleDb;
using System.Reflection;
using System.Text;
using System.Web;
using TDL.DB;
using TDL.IO;
//using TDL.Util;

[assembly:AssemblyCompany("TDSL")]
[assembly:AssemblyCopyright("(c) 2003 TDSL. All rights reserved.")]
[assembly:AssemblyDescription("其他地區 -> 新增賽事")]
[assembly:AssemblyProduct("Sports.NET")]
[assembly:AssemblyTitle("Sports.NET DLL")]
[assembly:AssemblyVersion("1.0.*")]
namespace SportsUtil {
	public class RegionNew {
		const string LOGFILESUFFIX = "log";
		const int RECORDSOINPAGE = 10;
		OleDbDataReader m_SportsOleReader;
		DBManager m_SportsDBMgr;
		Files m_SportsLog;
		StringBuilder SQLString;

		public RegionNew(string Connection) {
			m_SportsDBMgr = new DBManager();
			m_SportsDBMgr.ConnectionString = Connection;
			m_SportsLog = new Files();
			SQLString = new StringBuilder();
		}

		public string InitFields() {
			const int DEFAULTYEAR=1;
			const int DEFAULTMONTH=12;
			const int DEFAULTDAY=31;
			const int DEFAULTHOUR=24;
			const int DEFAULTMINUTE=60;
			const string EMPTYTEAMMESSAGE = "沒有隊伍在此聯賽!";
			const string ZEROPAD = "0";
			int iYear, iMonth, iDay, iNewValue = 0, iNoOfOpt = 0, iIndex = 0;
			string sRegionID, sLeagID, sRegion = "", sLeag = "", sAlias = "", sRecordStr = "";
			ArrayList teamAL = new ArrayList();
			teamAL.Capacity = 20;
			StringBuilder HTMLString = new StringBuilder();

			iYear = DateTime.Today.Year;
			iMonth = DateTime.Today.Month;
			iDay = DateTime.Today.Day;
			sLeagID = HttpContext.Current.Request.QueryString["LeagID"].Trim();
			sRegionID = HttpContext.Current.Request.QueryString["RegionID"].Trim();
			try {
				SQLString.Remove(0,SQLString.Length);
				SQLString.Append("select CREGION from OTHERREGION_CFG where IREGION_ID=");
				SQLString.Append(sRegionID);
				sRegion = m_SportsDBMgr.ExecuteQueryString(SQLString.ToString());

				SQLString.Remove(0,SQLString.Length);
				SQLString.Append("select teaminfo.teamname, leaginfo.leagname, leaginfo.alias from leaginfo, teaminfo, id_info where leaginfo.leag_id=id_info.leag_id and teaminfo.team_id=id_info.team_id and id_info.leag_id='");
				SQLString.Append(sLeagID);
				SQLString.Append("' order by teaminfo.teamname");
				m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
				while(m_SportsOleReader.Read()) {
					if(m_SportsOleReader.IsDBNull(0)) {
						HTMLString.Append("<tr style=\"background-color:#DDA0DD\"><th colspan=12>");
						HTMLString.Append(EMPTYTEAMMESSAGE);
						HTMLString.Append("</th></tr>");
						break;
					}	else {
						//get team into ArrayList
						sRecordStr = m_SportsOleReader.GetString(0).Trim();
						teamAL.Add(sRecordStr);

						//get League Name
						if(iIndex == 0) {
							sLeag = m_SportsOleReader.GetString(1).Trim();
							sAlias = m_SportsOleReader.GetString(2).Trim();
						}
						iIndex++;
					}
				}
				m_SportsDBMgr.Close();
				m_SportsOleReader.Close();
				teamAL.TrimToSize();

				//Display League Name
				HTMLString.Append("<input type=\"hidden\" name=\"Region\" value=\"");
				HTMLString.Append(sRegion);
				HTMLString.Append("\"><input type=\"hidden\" name=\"League\" value=\"");
				HTMLString.Append(sLeag);
				HTMLString.Append("\"><input type=\"hidden\" name=\"Alias\" value=\"");
				HTMLString.Append(sAlias);
				HTMLString.Append("\"><tr style=\"background-color:#CD5C5C\"><th colspan=5 align=\"left\">新增");
				HTMLString.Append(sRegion);
				HTMLString.Append("賽事 - <font color=#FFD700>");
				HTMLString.Append(sLeag);
				HTMLString.Append("</font></th><td align=\"center\"><a href=\"RegionNewMenu.aspx?RegionID=");
				HTMLString.Append(sRegionID);
				HTMLString.Append("\">上頁</a></td></tr><tr><th>年/月/日</th><th>時:分</th><th>主隊</th><th>客隊</th><th>主讓</th><th>中立場</th></tr>");

				iIndex = 0;
				while(RECORDSOINPAGE > iIndex) {
					HTMLString.Append("<tr align=\"center\">");
					//MatchDate
					HTMLString.Append("<td><select name=\"MatchYear\"><option value=\"");
					HTMLString.Append(iYear.ToString());
					HTMLString.Append("\">");
					HTMLString.Append(iYear.ToString());
					for(iNoOfOpt=1;iNoOfOpt<=DEFAULTYEAR;iNoOfOpt++) {
						iNewValue = iYear+iNoOfOpt;
						HTMLString.Append("<option value=\"");
						HTMLString.Append(iNewValue.ToString());
						HTMLString.Append("\">");
						HTMLString.Append(iNewValue.ToString());
					}
					HTMLString.Append("</select>");

					HTMLString.Append("/<select name=\"MatchMonth\"><option value=\"");
					if(iMonth<10) HTMLString.Append(ZEROPAD);
					HTMLString.Append(iMonth.ToString());
					HTMLString.Append("\">");
					HTMLString.Append(iMonth.ToString());
					iNoOfOpt = iMonth+1;
					while(iNoOfOpt != iMonth) {
						if(iNoOfOpt>DEFAULTMONTH) iNoOfOpt=1;
						if(!(iMonth == 1 && iNoOfOpt == 1)) {
							HTMLString.Append("<option value=\"");
							if(iNoOfOpt<10) HTMLString.Append(ZEROPAD);
							HTMLString.Append(iNoOfOpt.ToString());
							HTMLString.Append("\">");
							HTMLString.Append(iNoOfOpt.ToString());
							iNoOfOpt++;
						}
					}
					HTMLString.Append("</select>");

					HTMLString.Append("/<select name=\"MatchDate\"><option value=\"");
					if(iDay<10) HTMLString.Append(ZEROPAD);
					HTMLString.Append(iDay.ToString());
					HTMLString.Append("\">");
					HTMLString.Append(iDay.ToString());
					iNoOfOpt = iDay+1;
					while(iNoOfOpt != iDay) {
						if(iNoOfOpt>DEFAULTDAY) iNoOfOpt=1;
						if(!(iDay == 1 && iNoOfOpt == 1)) {
							HTMLString.Append("<option value=\"");
							if(iNoOfOpt<10) HTMLString.Append(ZEROPAD);
							HTMLString.Append(iNoOfOpt.ToString());
							HTMLString.Append("\">");
							HTMLString.Append(iNoOfOpt.ToString());
							iNoOfOpt++;
						}
					}
					HTMLString.Append("</select></td>");

					//MacthTime
					HTMLString.Append("<td><select name=\"MatchHour\">");
					for(iNoOfOpt=DEFAULTHOUR;iNoOfOpt>0;iNoOfOpt--) {
						HTMLString.Append("<option value=\"");
						if((iNoOfOpt-1)<10) HTMLString.Append(ZEROPAD);
						HTMLString.Append((iNoOfOpt-1).ToString());
						HTMLString.Append("\">");
						HTMLString.Append((iNoOfOpt-1).ToString());
					}
					HTMLString.Append("</select>:<select name=\"MatchMinute\">");
					for(iNoOfOpt=0;iNoOfOpt<DEFAULTMINUTE;iNoOfOpt++) {
						HTMLString.Append("<option value=\"");
						if(iNoOfOpt<10) HTMLString.Append(ZEROPAD);
						HTMLString.Append(iNoOfOpt.ToString());
						HTMLString.Append("\">");
						HTMLString.Append(iNoOfOpt.ToString());
					}
					HTMLString.Append("</select></td>");

					//Host
					HTMLString.Append("<td><select name=\"Host\">");
					for(iNoOfOpt=0;iNoOfOpt<teamAL.Count;iNoOfOpt++) {
						HTMLString.Append("<option value=\"");
						HTMLString.Append(teamAL[iNoOfOpt]);
						HTMLString.Append("\">");
						HTMLString.Append(teamAL[iNoOfOpt]);
					}
					HTMLString.Append("</select></td>");

					//Guest
					HTMLString.Append("<td><select name=\"Guest\">");
					for(iNoOfOpt=0;iNoOfOpt<teamAL.Count;iNoOfOpt++) {
						HTMLString.Append("<option value=\"");
						HTMLString.Append(teamAL[iNoOfOpt]);
						HTMLString.Append("\">");
						HTMLString.Append(teamAL[iNoOfOpt]);
					}
					HTMLString.Append("</select></td>");

					//Host Handicap check box
					HTMLString.Append("<td><input type=\"checkbox\" name=\"HostHandicap\" value=\"");
					HTMLString.Append(iIndex.ToString());
					HTMLString.Append("\" checked></td>");
					//Match Field
					HTMLString.Append("<td><input type=\"checkbox\" name=\"MatchField\" value=\"");
					HTMLString.Append(iIndex.ToString());
					HTMLString.Append("\"></td></tr>");
					iIndex++;
				}
				m_SportsDBMgr.Dispose();
			}	catch(Exception ex) {
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " RegionNew.cs.InitFields(): " + ex.ToString());
				m_SportsLog.Close();
				HTMLString.Remove(0,HTMLString.Length);
				HTMLString.Append((string)HttpContext.Current.Application["accessErrorMsg"]);
			}
			return HTMLString.ToString();
		}

		public int Add() {
			bool bUpdFlag = false;
			int iUpdIndex, iRecUpd = 0, iMax = 0, iArrayIndex = 0;
			char[] delimiter = new char[] {','};
			string sRegion, sMatchDate = "", sMatchTime = "";
			string sLeague, sAlias;
			string[] arrHost, arrGuest, arrYear, arrMonth, arrDate, arrHour, arrMinute, arrField, arrHostHandicap;
			//Replicator MatchReplicator = new Replicator(HttpContext.Current.Application["RepDBConnectionString"].ToString());
			//MatchReplicator.ApplicationType = 1;

			//get value from page/memory
			sRegion = HttpContext.Current.Request.Form["Region"];
			sLeague = HttpContext.Current.Request.Form["League"];
			sAlias = HttpContext.Current.Request.Form["Alias"];
			arrHost = HttpContext.Current.Request.Form["Host"].Split(delimiter);
			arrGuest = HttpContext.Current.Request.Form["Guest"].Split(delimiter);
			arrYear = HttpContext.Current.Request.Form["MatchYear"].Split(delimiter);
			arrMonth = HttpContext.Current.Request.Form["MatchMonth"].Split(delimiter);
			arrDate = HttpContext.Current.Request.Form["MatchDate"].Split(delimiter);
			arrHour = HttpContext.Current.Request.Form["MatchHour"].Split(delimiter);
			arrMinute = HttpContext.Current.Request.Form["MatchMinute"].Split(delimiter);
			try {	//get match field
				arrField = HttpContext.Current.Request.Form["MatchField"].Split(delimiter);
			}	catch(Exception) {
				arrField = new string[0];
			}
			try {	//get host handicap
				arrHostHandicap = HttpContext.Current.Request.Form["HostHandicap"].Split(delimiter);
			}	catch(Exception) {
				arrHostHandicap = new string[0];
			}

			try {
				for(iUpdIndex=0;iUpdIndex<RECORDSOINPAGE;iUpdIndex++) {	//check for records needed to be inserted
					if(!arrHost[iUpdIndex].Equals(arrGuest[iUpdIndex])) {	//insert for different host and guest
						//get Match date time
						sMatchDate = arrYear[iUpdIndex] + arrMonth[iUpdIndex] + arrDate[iUpdIndex];
						sMatchTime = arrHour[iUpdIndex] + arrMinute[iUpdIndex];

						try {
							//Add a new match: insert into OTHERODDSINFO
							SQLString.Remove(0,SQLString.Length);
							SQLString.Append("select MAX(MATCH_CNT) from OTHERODDSINFO");
							m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
							if(m_SportsOleReader.Read()) {
								if(!m_SportsOleReader.IsDBNull(0)) iMax = m_SportsOleReader.GetInt32(0);
							}
							m_SportsDBMgr.Close();
							m_SportsOleReader.Close();
							iMax++;

							SQLString.Remove(0,SQLString.Length);
							SQLString.Append("INSERT INTO OTHERODDSINFO values ('");
							SQLString.Append(sRegion);
							SQLString.Append("','");
							SQLString.Append(sLeague);
							SQLString.Append("','");
							SQLString.Append(sAlias);
							SQLString.Append("','");
							SQLString.Append(arrHost[iUpdIndex]);
							SQLString.Append("','");
							SQLString.Append(arrGuest[iUpdIndex]);
							SQLString.Append("','");
							SQLString.Append(iMax.ToString());
							SQLString.Append("','U','");
							SQLString.Append(sMatchDate);
							SQLString.Append("','");
							SQLString.Append(sMatchTime);
							SQLString.Append("',");

							for(iArrayIndex=0;iArrayIndex<arrField.Length;iArrayIndex++) {
								if(Convert.ToInt32(arrField[iArrayIndex]) == iUpdIndex) {
									bUpdFlag = true;
									break;
								}
							}
							if(bUpdFlag) SQLString.Append("'M',");
							else SQLString.Append("'H',");
							bUpdFlag = false;

							for(iArrayIndex=0;iArrayIndex<arrHostHandicap.Length;iArrayIndex++) {
								if(Convert.ToInt32(arrHostHandicap[iArrayIndex]) == iUpdIndex) {
									bUpdFlag = true;
									break;
								}
							}
							if(bUpdFlag) SQLString.Append("'1',");
							else SQLString.Append("'0',");
							bUpdFlag = false;

							SQLString.Append("'','','','','','','','0','0','V','-1','1')");
							m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
							//MatchReplicator.Replicate(SQLString.ToString());
							iRecUpd++;
						}	catch(OleDbException) {
							iRecUpd = -99;
						}
					}	else {	//break for same host and guest
						break;
					}
				}
				if(iRecUpd > 0) {
					//MatchReplicator.Dispose();
					m_SportsDBMgr.Dispose();
				}

				//write log
				m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " RegionNew.cs: Add " + iRecUpd.ToString() + " matches (" + HttpContext.Current.Session["user_name"] + ")");
				m_SportsLog.Close();
			} catch(Exception ex) {
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " RegionNew.cs.Add(): " + ex.ToString());
				m_SportsLog.Close();
			}

			return iRecUpd;
		}
	}
}