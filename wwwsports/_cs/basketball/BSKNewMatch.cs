/*
Objective:
add match

Last updated:
5 Nov 2003 by Chapman

C#.NET complier statement:
csc /t:library /out:..\..\bin\BSKNewMatch.dll /r:..\..\bin\DBManager.dll;..\..\bin\Files.dll BSKNewMatch.cs
*/

using System;
using System.Collections;
using System.Collections.Specialized;
using System.Data.OleDb;
using System.Reflection;
using System.Text;
using System.Web;
using TDL.DB;
using TDL.IO;

[assembly:AssemblyCompany("TDSL")]
[assembly:AssemblyCopyright("(c) 2003 TDSL. All rights reserved. Created on 26 June 2003.")]
[assembly:AssemblyDescription("籃球資訊 -> 新增賽事")]
[assembly:AssemblyProduct("Sports.NET")]
[assembly:AssemblyTitle("Sports.NET DLL")]
[assembly:AssemblyVersion("1.0.*")]

namespace SportsUtil {
	public class BSKNewMatch {
		const string LOGFILESUFFIX = "log";
		const int RECORDSOINPAGE = 10;
		string sLeagueName = "";
		DBManager m_SportsDBMgr;
		Files m_SportsLog;
		StringBuilder SQLString;
		OleDbDataReader m_SportsOleReader;

		public BSKNewMatch(string Connection) {
			m_SportsDBMgr = new DBManager();
			m_SportsDBMgr.ConnectionString = Connection;
			m_SportsLog = new Files();
			SQLString = new StringBuilder();
		}
		public string SetLeaglong {
			get {
				return 	sLeagueName;
			}
		}
		public string GetInterface() {
			const int DEFAULTYEAR=1;
			const int DEFAULTMONTH=12;
			const int DEFAULTDAY=31;
			const int DEFAULTHOUR=24;
			const int DEFAULTMINUTE=60;
			const string ZEROPAD = "0";
			int iYear, iMonth, iDay, iNewValue = 0, iNoOfOpt = 0, iIndex = 0;
			string  sLeagID, sRecordStr = "";
			StringBuilder HTMLString = new StringBuilder();
			ArrayList teamAL = new ArrayList();
			teamAL.Capacity = 20;
			iYear = DateTime.Today.Year;
			iMonth = DateTime.Today.Month;
			iDay = DateTime.Today.Day;
			sLeagID = HttpContext.Current.Request.QueryString["leagID"].Trim();
			
			try {		
				SQLString.Remove(0,SQLString.Length);		
				SQLString.Append("Select cleague from league_info where cleag_id='");
				SQLString.Append(sLeagID);
				SQLString.Append("'");
				m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
				if(m_SportsOleReader.Read()){
					sLeagueName = m_SportsOleReader.GetString(0).Trim();
				}
				m_SportsDBMgr.Close();
				m_SportsOleReader.Close(); 
				
				SQLString.Remove(0,SQLString.Length);
				SQLString.Append("select team_info.cteam from league_info, team_info, idmap_info  where league_info.cleag_id=idmap_info.cleag_id and team_info.cteam_id=idmap_info.cteam_id and idmap_info.cleag_id='");
				SQLString.Append(sLeagID);
				SQLString.Append("' order by team_info.cteam");
				m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());				
				if(m_SportsOleReader.Read()) {
					sRecordStr = m_SportsOleReader.GetString(0).Trim();
					teamAL.Add(sRecordStr);
					while(m_SportsOleReader.Read()) {											
							//get team into ArrayList
							sRecordStr = m_SportsOleReader.GetString(0).Trim();
							teamAL.Add(sRecordStr);						
					}
				} else {
					teamAL.Add("暫無隊伍");
				}
				m_SportsOleReader.Close();
				m_SportsDBMgr.Close();
				teamAL.TrimToSize();				
				//Display League Name
				HTMLString.Append("<input type=\"hidden\" name=\"Leaguename\" value=\"");
				HTMLString.Append(sLeagueName);
				HTMLString.Append("\">");							
				HTMLString.Append("<tr><th>年/月/日</th><th>時:分</th><th>主隊</th><th>客隊</th><th>主讓</th><th>中立場</th></tr>");
				iIndex = 0;
				while(RECORDSOINPAGE > iIndex) {
					//MatchDate
					HTMLString.Append("<tr align=\"center\"><td><select name=\"MatchYear\"><option value=\"");
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
					HTMLString.Append("</select>/<select name=\"MatchMonth\"><option value=\"");
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
					HTMLString.Append("</select>/<select name=\"MatchDay\"><option value=\"");
					if(iDay<10) HTMLString.Append(ZEROPAD);
					HTMLString.Append( iDay.ToString());
					HTMLString.Append("\">");
					HTMLString.Append(iDay.ToString());
					iNoOfOpt = iDay+1;
					while(iNoOfOpt != iDay) {
						if(iNoOfOpt>DEFAULTDAY) iNoOfOpt=1;
						if(!(iDay == 1 && iNoOfOpt == 1)) {
							HTMLString.Append( "<option value=\"");
							if(iNoOfOpt<10) HTMLString.Append(ZEROPAD);
							HTMLString.Append( iNoOfOpt.ToString());
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
						HTMLString.Append("<option value=\"" );
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
					HTMLString.Append("<td><input type=\"checkbox\" name=\"HostHandicap\" value=\"" );
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
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " BSKNewMatch.cs.GetInterface(): " + ex.ToString());
				m_SportsLog.Close();
				
				HTMLString.Remove(0,HTMLString.Length);
				HTMLString.Append((string)HttpContext.Current.Application["accessErrorMsg"]);
			}
			return HTMLString.ToString();;
		}

		public int AddMatch() {
			int iRecUpd = 0;
			int iMax = 0;
			int iUpdIndex;
			int iArrayIndex = 0;
			int iExisted = 0;
			string sLeagueName;
			string sMatchDate = "";
			string sMatchTime = "";
			string sMatchField = "H";
			string sHostHandicap = "0";
			char[] delimiter = new char[] {','};
			string[] arrYear;
			string[] arrMonth;
			string[] arrDay;
			string[] arrHour;
			string[] arrMinute;
			string[] arrHost;
			string[] arrGuest;
			string[] arrField;
			string[] arrHostHandicap;

			sLeagueName = HttpContext.Current.Request.Form["Leaguename"];
			arrHost = HttpContext.Current.Request.Form["Host"].Split(delimiter);
			arrGuest = HttpContext.Current.Request.Form["Guest"].Split(delimiter);
			arrYear = HttpContext.Current.Request.Form["MatchYear"].Split(delimiter);
			arrMonth = HttpContext.Current.Request.Form["MatchMonth"].Split(delimiter);
			arrDay = HttpContext.Current.Request.Form["MatchDay"].Split(delimiter);
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
				SQLString.Remove(0,SQLString.Length);
				SQLString.Append("select MAX(IMATCH_COUNT) from NBAGAME_INFO");
				m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
				if(m_SportsOleReader.Read()) {
					if(!m_SportsOleReader.IsDBNull(0)) iMax = m_SportsOleReader.GetInt32(0);
				}
				m_SportsOleReader.Close();
				m_SportsDBMgr.Close();

				for(iUpdIndex = 0; iUpdIndex < RECORDSOINPAGE; iUpdIndex++) {
					sMatchField = "H";
					sHostHandicap = "0";
					if(!arrHost[iUpdIndex].Equals(arrGuest[iUpdIndex])) {
						iExisted = 0;
						SQLString.Remove(0,SQLString.Length);
						SQLString.Append("select count(IMATCH_COUNT) from NBAGAME_INFO where CLEAG='");
						SQLString.Append(sLeagueName);
						SQLString.Append("' and CHOST='");
						SQLString.Append(arrHost[iUpdIndex]);
						SQLString.Append("' and CGUEST='");
						SQLString.Append(arrGuest[iUpdIndex]);
						SQLString.Append("'");	
						iExisted = m_SportsDBMgr.ExecuteScalar(SQLString.ToString());
						m_SportsDBMgr.Close();

						if(iExisted == 0) {
							iMax++;
							sMatchDate = arrYear[iUpdIndex] + arrMonth[iUpdIndex] + arrDay[iUpdIndex];
							sMatchTime = arrHour[iUpdIndex] + arrMinute[iUpdIndex];
							for(iArrayIndex = 0; iArrayIndex < arrHostHandicap.Length; iArrayIndex++) {
								if(Convert.ToInt32(arrHostHandicap[iArrayIndex]) == iUpdIndex) {
									sHostHandicap = "1";
									break;
								}
							}

							for(iArrayIndex = 0; iArrayIndex < arrField.Length; iArrayIndex++) {
								if(Convert.ToInt32(arrField[iArrayIndex]) == iUpdIndex) {
									sMatchField = "M";
									break;
								}
							}

							SQLString.Remove(0,SQLString.Length);
							SQLString.Append("insert into NBAGAME_INFO (IMATCH_COUNT,CLEAG,CHOST,CGUEST,ISEQ_NO,CACTION,CMATCH_DATE,CMATCH_TIME,CMATCH_FIELD,CHOST_HANDI,CSCORE_HANDI ,CSH_ODDS,CSCORE_TARGET,CST_ODDS,CWH_ODDS,CWG_ODDS,IALERT_TYRE,CSENT_FLAG,CWEB_VIEW,IINTERVAL) values (");
							SQLString.Append(Convert.ToInt32(iMax));
							SQLString.Append(",'");
							SQLString.Append(sLeagueName);
							SQLString.Append("','");
							SQLString.Append(arrHost[iUpdIndex]);
							SQLString.Append("','");
							SQLString.Append(arrGuest[iUpdIndex]);
							SQLString.Append("',null,'U',");
							SQLString.Append("'");
							SQLString.Append(sMatchDate);
							SQLString.Append("','");
							SQLString.Append(sMatchTime);
							SQLString.Append("','");
							SQLString.Append(sMatchField);
							SQLString.Append("','");
							SQLString.Append(sHostHandicap);
							SQLString.Append("','','','','','','',0,'0','V',0"+")");
							m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
							iRecUpd++; 
						}
					} else {
						break;
					}
				}

				//write log
				m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " BSKNewMatch.cs: AddMatch() " + iRecUpd.ToString() + " matches (" + HttpContext.Current.Session["user_name"] + ")");
				m_SportsLog.Close();
			} catch(Exception ex) {
				iRecUpd = -1;
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " BSKNewMatch.cs().AddMatch(): " + ex.ToString());
				m_SportsLog.Close();
			}

			return iRecUpd;
		}
	}
}