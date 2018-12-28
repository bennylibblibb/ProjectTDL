/*
Objective:
Import www.macauslot.com match data to Asia Sports

Last updated:
19 June 2003 by Chapman

C#.NET complier statement:
csc /t:library /out:..\bin\ImportMslot.dll /r:..\bin\DBManager.dll;..\bin\Files.dll ImportMslot.cs
*/

using System;
using System.Data.OleDb;
using System.Reflection;
using System.Web;
using TDL.DB;
using TDL.IO;

[assembly:AssemblyCompany("TDSL")]
[assembly:AssemblyCopyright("(c) 2003 TDSL. All rights reserved.")]
[assembly:AssemblyDescription("其他地區 -> 澳門網自動更新設定 -> 匯入")]
[assembly:AssemblyProduct("Sports.NET")]
[assembly:AssemblyTitle("Sports.NET DLL")]
[assembly:AssemblyVersion("1.0.*")]
namespace SportsUtil {
	public class ImportMslot {
		const string LOGFILESUFFIX = "log";
		int m_RecordCount = 0;
		OleDbDataReader m_ImportReader;
		DBManager m_MacauDB;
		Files m_MacauFile;

		public ImportMslot(string Connection) {
			m_MacauDB = new DBManager();
			m_MacauDB.ConnectionString = Connection;
			m_MacauFile = new Files();
		}

		public int NumberOfRecords {
			get {
				return m_RecordCount;
			}
		}

		public string GetMslotMatches() {
			string sRecordStr;
			string retrieveQuery;
			string sRtn = null;
			DateTime dtMatch;

			retrieveQuery = "select main.matchdatetime, main.cleague, main.chost, main.cguest, winodds.CSCOREHANDI, winodds.CHOSTODDS, ";
			retrieveQuery += "winodds.CGUESTODDS, main.cmatch_field, selfwin.CEWINODDS, selfwin.CEDRAWODDS, selfwin.CELOSSODDS, leag.alias ";
			retrieveQuery += "from asiaoddsmatch_info main, winodds_info winodds, selfwin_info selfwin, leaginfo leag ";
			retrieveQuery += "where main.IMATCH_ID=winodds.IMATCH_ID and main.IMATCH_ID = selfwin.IMATCH_ID and main.cleague = leag.leagname ";
			retrieveQuery += "order by leag.LEAG_ORDER, main.matchdatetime";
			try {
				m_ImportReader = m_MacauDB.ExecuteQuery(retrieveQuery);
				while(m_ImportReader.Read()) {
					sRtn += "<tr align=\"center\">";
					sRtn += "<td><input type=\"checkbox\" name=\"importItem\" value=\"" + m_RecordCount + "\" checked></td>";

					//Match Date and Time (Hidden Field)
					dtMatch = m_ImportReader.GetDateTime(0);
					sRtn += "<td><input type=\"hidden\" name=\"MatchDate\" value=\"" + dtMatch.ToString("yyyyMMdd") + "\">" + dtMatch.ToString("yyyy/MM/dd") + "</td>";
					sRtn += "<td><input type=\"hidden\" name=\"MatchTime\" value=\"" + dtMatch.ToString("HHmm") + "\">" + dtMatch.ToString("HH:mm") + "</td>";

					//League
					sRecordStr = m_ImportReader.GetString(1).Trim();
					sRtn += "<td>" + sRecordStr + "<input type=\"hidden\" name=\"League\" value=\"" + sRecordStr + "\"></td>";

					//Host
					sRecordStr = m_ImportReader.GetString(2).Trim();
					sRtn += "<td>" + sRecordStr + "<input type=\"hidden\" name=\"Host\" value=\"" + sRecordStr + "\"></td>";

					//Guest
					sRecordStr = m_ImportReader.GetString(3).Trim();
					sRtn += "<td>" + sRecordStr + "<input type=\"hidden\" name=\"Guest\" value=\"" + sRecordStr + "\">";

					//Match Field (Hidden field)
					sRecordStr = "";
					if(!m_ImportReader.IsDBNull(4)) {
						sRecordStr = m_ImportReader.GetString(4).Trim();
						if(sRecordStr.Equals("-1")) sRecordStr = "";
					}
					sRtn += "<input type=\"hidden\" name=\"ScoreHandicap\" value=\"" + sRecordStr + "\">";
					sRecordStr = "";
					if(!m_ImportReader.IsDBNull(5)) {
						sRecordStr = m_ImportReader.GetString(5).Trim();
						if(sRecordStr.Equals("-1")) sRecordStr = "";
					}
					sRtn += "<input type=\"hidden\" name=\"HostOdds\" value=\"" + sRecordStr + "\">";
					sRecordStr = "";
					if(!m_ImportReader.IsDBNull(6)) {
						sRecordStr = m_ImportReader.GetString(6).Trim();
						if(sRecordStr.Equals("-1")) sRecordStr = "";
					}
					sRtn += "<input type=\"hidden\" name=\"GuestOdds\" value=\"" + sRecordStr + "\">";

					sRecordStr = "";
					if(!m_ImportReader.IsDBNull(7)) {
						sRecordStr = m_ImportReader.GetString(7).Trim();
						if(sRecordStr.Equals("-1")) sRecordStr = "";
					}
					sRtn += "<input type=\"hidden\" name=\"MatchField\" value=\"" + sRecordStr + "\">";

					sRecordStr = "";
					if(!m_ImportReader.IsDBNull(8)) {
						sRecordStr = m_ImportReader.GetString(8).Trim();
						if(sRecordStr.Equals("-1")) sRecordStr = "";
					}
					sRtn += "<input type=\"hidden\" name=\"EWinOdds\" value=\"" + sRecordStr + "\">";

					sRecordStr = "";
					if(!m_ImportReader.IsDBNull(9)) {
						sRecordStr = m_ImportReader.GetString(9).Trim();
						if(sRecordStr.Equals("-1")) sRecordStr = "";
					}
					sRtn += "<input type=\"hidden\" name=\"EDrawOdds\" value=\"" + sRecordStr + "\">";
					sRecordStr = "";
					if(!m_ImportReader.IsDBNull(10)) {
						sRecordStr = m_ImportReader.GetString(10).Trim();
						if(sRecordStr.Equals("-1")) sRecordStr = "";
					}
					sRtn += "<input type=\"hidden\" name=\"ELossOdds\" value=\"" + sRecordStr + "\">";

					sRecordStr = "";
					if(!m_ImportReader.IsDBNull(11)) {
						sRecordStr = m_ImportReader.GetString(11).Trim();
						if(sRecordStr.Equals("-1")) sRecordStr = "";
					}
					sRtn += "<input type=\"hidden\" name=\"Alias\" value=\"" + sRecordStr + "\"></td>";
					sRtn += "</tr>";
					m_RecordCount++;
				}
				m_MacauDB.Close();
				m_ImportReader.Close();
				m_MacauDB.Dispose();
				sRtn += "<input type=\"hidden\" name=\"RecordCount\" value=\"" + m_RecordCount.ToString() + "\">";
			} catch(Exception ex) {
				m_MacauFile.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_MacauFile.SetFileName(0,LOGFILESUFFIX);
				m_MacauFile.Open();
				m_MacauFile.Write(DateTime.Now.ToString("HH:mm:ss") + " ImportMslot.cs.GetMslotMatches(): " + ex.ToString());
				m_MacauFile.Close();
				sRtn = (string)HttpContext.Current.Application["accessErrorMsg"];
			}

			return sRtn;
		}

		public int Import() {
			const string REGION = "澳門";
			int iRecIndex, iSendRequiredLen, iMatchCnt = 0, iExisted = 0, iUpdIndex = 0, iSuccessUpd = 0;
			string updateQuery = "", sMatchDate, sMatchTime, sLeague, sHost, sGuest, sHostHandicap, sScoreHandicap1, sScoreHandicap2, sMatchField, sHostOdds, sGuestOdds, sEWOdds, sELOdds, sEDOdds, sAlias;
			char[] delimiter = new char[] {','};
			char[] ScoreHandicapDelimiter = new char[] {'/'};
			string[] arrSendRequired, arrMatchDate, arrMatchTime, arrLeague, arrHost, arrGuest, arrScoreHandicap, arrHandicap;
			string[] arrMatchField, arrHostOdds, arrGuestOdds, arrEWinOdds, arrEDrawOdds, arrELossOdds, arrAlias;

			try {
				arrSendRequired = HttpContext.Current.Request.Form["importItem"].Split(delimiter);
				iSendRequiredLen = arrSendRequired.Length;
			} catch(Exception) {
				arrSendRequired = new string[0];
				iSendRequiredLen = 0;
			}
			arrMatchDate = HttpContext.Current.Request.Form["MatchDate"].Split(delimiter);
			arrMatchTime = HttpContext.Current.Request.Form["MatchTime"].Split(delimiter);
			arrLeague = HttpContext.Current.Request.Form["League"].Split(delimiter);
			arrHost = HttpContext.Current.Request.Form["Host"].Split(delimiter);
			arrGuest = HttpContext.Current.Request.Form["Guest"].Split(delimiter);
			arrScoreHandicap = HttpContext.Current.Request.Form["ScoreHandicap"].Split(delimiter);
			arrHostOdds = HttpContext.Current.Request.Form["HostOdds"].Split(delimiter);
			arrGuestOdds = HttpContext.Current.Request.Form["GuestOdds"].Split(delimiter);
			arrMatchField = HttpContext.Current.Request.Form["MatchField"].Split(delimiter);
			arrEWinOdds = HttpContext.Current.Request.Form["EWinOdds"].Split(delimiter);
			arrEDrawOdds = HttpContext.Current.Request.Form["EDrawOdds"].Split(delimiter);
			arrELossOdds = HttpContext.Current.Request.Form["ELossOdds"].Split(delimiter);
			arrAlias = HttpContext.Current.Request.Form["Alias"].Split(delimiter);

			try {
				for(iRecIndex=0; iRecIndex<iSendRequiredLen; iRecIndex++) {
					iUpdIndex = Convert.ToInt32(arrSendRequired[iRecIndex]);
					sMatchDate = arrMatchDate[iUpdIndex];
					sMatchTime = arrMatchTime[iUpdIndex];
					sLeague = arrLeague[iUpdIndex];
					sHost = arrHost[iUpdIndex];
					sGuest = arrGuest[iUpdIndex];
					sScoreHandicap1 = arrScoreHandicap[iUpdIndex];
					sMatchField = arrMatchField[iUpdIndex];
					sHostOdds = arrHostOdds[iUpdIndex];
					sGuestOdds = arrGuestOdds[iUpdIndex];
					sEWOdds = arrEWinOdds[iUpdIndex];
					sELOdds = arrEDrawOdds[iUpdIndex];
					sEDOdds = arrELossOdds[iUpdIndex];
					sAlias = arrAlias[iUpdIndex];
					if(sMatchField.Equals("b")) {
						sHostHandicap = "0";
						sMatchField = "H";
					} else if(sMatchField.Equals("c")) {
						sHostHandicap = "1";
						sMatchField = "M";
					} else {
						sHostHandicap = "1";
						sMatchField = "H";
					}
					arrHandicap = sScoreHandicap1.Split(ScoreHandicapDelimiter);
					if(arrHandicap.Length > 1) {
						sScoreHandicap1 = arrHandicap[0];
						sScoreHandicap2 = arrHandicap[1];
					} else {
						sScoreHandicap2 = "";
					}

					updateQuery = "select count(MATCH_CNT) from OTHERODDSINFO where COMPANY='" + REGION + "' and LEAGUE='" + sLeague + "' and HOST='" + sHost + "' and GUEST='" + sGuest + "'";
					iExisted = m_MacauDB.ExecuteScalar(updateQuery);

					if(iExisted == 0) {
						updateQuery = "select max(MATCH_CNT) from OTHERODDSINFO";
						m_ImportReader = m_MacauDB.ExecuteQuery(updateQuery);
						if(m_ImportReader.Read()) {
							if(!m_ImportReader.IsDBNull(0)) {
								iMatchCnt = m_ImportReader.GetInt32(0);
							}
						}
						m_MacauDB.Close();
						m_ImportReader.Close();
						iMatchCnt++;

						updateQuery = "INSERT INTO OTHERODDSINFO values ('" + REGION + "',";
						updateQuery += "'" + sLeague + "',";
						updateQuery += "'" + sAlias + "',";
						updateQuery += "'" + sHost + "',";
						updateQuery += "'" + sGuest + "',";
						updateQuery += "'" + iMatchCnt + "',";
						updateQuery += "'U',";
						updateQuery += "'" + sMatchDate + "',";
						updateQuery += "'" + sMatchTime + "',";
						updateQuery += "'" + sMatchField + "',";
						updateQuery += "'" + sHostHandicap + "',";
						updateQuery += "'" + sScoreHandicap1 + "',";
						updateQuery += "'" + sScoreHandicap2 + "',";
						updateQuery += "'" + sHostOdds + "',";
						updateQuery += "'" + sGuestOdds + "',";
						updateQuery += "'" + sEWOdds + "',";
						updateQuery += "'" + sEDOdds + "',";
						updateQuery += "'" + sELOdds + "',";
						updateQuery += "'0','1','V','-1','1')";
						m_MacauDB.ExecuteNonQuery(updateQuery);
						iSuccessUpd++;
					}
				}
				if(iRecIndex > 0) m_MacauDB.Dispose();

				//write log
				m_MacauFile.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
				m_MacauFile.SetFileName(0,LOGFILESUFFIX);
				m_MacauFile.Open();
				m_MacauFile.Write(DateTime.Now.ToString("HH:mm:ss") + " ImportMslot.cs: Import " + iSuccessUpd.ToString() + " matches from www.macauslot.com (" + HttpContext.Current.Session["user_name"] + ")");
				m_MacauFile.Close();
			} catch(Exception ex) {
				iSuccessUpd = -1;
				m_MacauFile.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_MacauFile.SetFileName(0,LOGFILESUFFIX);
				m_MacauFile.Open();
				m_MacauFile.Write(DateTime.Now.ToString("HH:mm:ss") + " ImportMslot.cs.Import(): " + ex.ToString());
				m_MacauFile.Close();
			}
			return iSuccessUpd;
		}
	}
}