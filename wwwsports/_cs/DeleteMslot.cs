/*
Objective:
Delete www.macauslot.com match data

Last updated:
19 June 2003 by Chapman

C#.NET complier statement:
csc /t:library /out:..\bin\DeleteMslot.dll /r:..\bin\DBManager.dll;..\bin\Files.dll DeleteMslot.cs
*/

using System;
using System.Data.OleDb;
using System.Reflection;
using System.Web;
using TDL.DB;
using TDL.IO;

[assembly:AssemblyCompany("TDSL")]
[assembly:AssemblyCopyright("(c) 2003 TDSL. All rights reserved.")]
[assembly:AssemblyDescription("其他地區 -> 澳門網自動更新設定 -> 清除")]
[assembly:AssemblyProduct("Sports.NET")]
[assembly:AssemblyTitle("Sports.NET DLL")]
[assembly:AssemblyVersion("1.0.*")]
namespace SportsUtil {
	public class DeleteMslot {
		const string LOGFILESUFFIX = "log";
		int m_iRecordCount = 0;
		OleDbDataReader m_DeleteReader;
		DBManager m_MacauDB;
		Files m_MacauFile;

		public DeleteMslot(string Connection) {
			m_MacauDB = new DBManager();
			m_MacauDB.ConnectionString = Connection;
			m_MacauFile = new Files();
		}

		public int NumberOfRecords {
			get {
				return m_iRecordCount;
			}
		}

		public string GetMslotMatch() {
			string retrieveQuery;
			string sRtn = null;
			string sRecordStr = null;
			DateTime dtMatch;

			retrieveQuery = "select main.IMATCH_ID, main.matchdatetime, main.cleague, main.chost, main.cguest, winodds.CSCOREHANDI, winodds.CHOSTODDS, ";
			retrieveQuery += "winodds.CGUESTODDS, main.cmatch_field, selfwin.CEWINODDS, selfwin.CEDRAWODDS, selfwin.CELOSSODDS, leag.alias ";
			retrieveQuery += "from asiaoddsmatch_info main, winodds_info winodds, selfwin_info selfwin, leaginfo leag ";
			retrieveQuery += "where main.IMATCH_ID=winodds.IMATCH_ID and main.IMATCH_ID = selfwin.IMATCH_ID and main.cleague = leag.leagname ";
			retrieveQuery += "order by leag.LEAG_ORDER, main.matchdatetime";
			try {
				m_DeleteReader = m_MacauDB.ExecuteQuery(retrieveQuery);
				while(m_DeleteReader.Read()) {
					sRtn += "<tr align=\"center\">";
					sRtn += "<td><input type=\"checkbox\" name=\"deleteItem\" value=\"" + m_iRecordCount + "\">";

					//MatchID (Hidden field)
					sRecordStr = m_DeleteReader.GetInt32(0).ToString();
					sRtn += "<input type=\"hidden\" name=\"MatchID\" value=\"" + sRecordStr + "\"></td>";

					//Match Date and Time (Hidden Field)
					dtMatch = m_DeleteReader.GetDateTime(1);
					sRtn += "<td><input type=\"hidden\" name=\"MatchDate\" value=\"" + dtMatch.ToString("yyyyMMdd") + "\">" + dtMatch.ToString("yyyy/MM/dd") + "</td>";
					sRtn += "<td><input type=\"hidden\" name=\"MatchTime\" value=\"" + dtMatch.ToString("HHmm") + "\">" + dtMatch.ToString("HH:mm") + "</td>";

					//League
					sRecordStr = m_DeleteReader.GetString(2).Trim();
					sRtn += "<td>" + sRecordStr + "<input type=\"hidden\" name=\"League\" value=\"" + sRecordStr + "\"></td>";

					//Host
					sRecordStr = m_DeleteReader.GetString(3).Trim();
					sRtn += "<td>" + sRecordStr + "<input type=\"hidden\" name=\"Host\" value=\"" + sRecordStr + "\"></td>";

					//Guest
					sRecordStr = m_DeleteReader.GetString(4).Trim();
					sRtn += "<td>" + sRecordStr + "<input type=\"hidden\" name=\"Guest\" value=\"" + sRecordStr + "\">";

					//Match Field (Hidden field)
					sRecordStr = "";
					if(!m_DeleteReader.IsDBNull(5)) {
						sRecordStr = m_DeleteReader.GetString(5).Trim();
						if(sRecordStr.Equals("-1")) sRecordStr = "";
					}
					sRtn += "<input type=\"hidden\" name=\"ScoreHandicap\" value=\"" + sRecordStr + "\">";
					sRecordStr = "";
					if(!m_DeleteReader.IsDBNull(6)) {
						sRecordStr = m_DeleteReader.GetString(6).Trim();
						if(sRecordStr.Equals("-1")) sRecordStr = "";
					}
					sRtn += "<input type=\"hidden\" name=\"HostOdds\" value=\"" + sRecordStr + "\">";
					sRecordStr = "";
					if(!m_DeleteReader.IsDBNull(7)) {
						sRecordStr = m_DeleteReader.GetString(7).Trim();
						if(sRecordStr.Equals("-1")) sRecordStr = "";
					}
					sRtn += "<input type=\"hidden\" name=\"GuestOdds\" value=\"" + sRecordStr + "\">";
					sRecordStr = "";
					if(!m_DeleteReader.IsDBNull(8)) {
						sRecordStr = m_DeleteReader.GetString(8).Trim();
						if(sRecordStr.Equals("-1")) sRecordStr = "";
					}
					sRtn += "<input type=\"hidden\" name=\"MatchField\" value=\"" + sRecordStr + "\">";
					sRecordStr = "";
					if(!m_DeleteReader.IsDBNull(9)) {
						sRecordStr = m_DeleteReader.GetString(9).Trim();
						if(sRecordStr.Equals("-1")) sRecordStr = "";
					}
					sRtn += "<input type=\"hidden\" name=\"EWinOdds\" value=\"" + sRecordStr + "\">";
					sRecordStr = "";
					if(!m_DeleteReader.IsDBNull(10)) {
						sRecordStr = m_DeleteReader.GetString(10).Trim();
						if(sRecordStr.Equals("-1")) sRecordStr = "";
					}
					sRtn += "<input type=\"hidden\" name=\"EDrawOdds\" value=\"" + sRecordStr + "\">";
					sRecordStr = "";
					if(!m_DeleteReader.IsDBNull(11)) {
						sRecordStr = m_DeleteReader.GetString(11).Trim();
						if(sRecordStr.Equals("-1")) sRecordStr = "";
					}
					sRtn += "<input type=\"hidden\" name=\"ELossOdds\" value=\"" + sRecordStr + "\">";
					sRecordStr = "";
					if(!m_DeleteReader.IsDBNull(12)) {
						sRecordStr = m_DeleteReader.GetString(12).Trim();
						if(sRecordStr.Equals("-1")) sRecordStr = "";
					}
					sRtn += "<input type=\"hidden\" name=\"Alias\" value=\"" + sRecordStr + "\"></td>";
					sRtn += "</tr>";
					m_iRecordCount++;
				}
				m_MacauDB.Close();
				m_DeleteReader.Close();
				m_MacauDB.Dispose();
				sRtn += "<input type=\"hidden\" name=\"RecordCount\" value=\"" + m_iRecordCount.ToString() + "\">";
			} catch(Exception ex) {
				m_MacauFile.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_MacauFile.SetFileName(0,LOGFILESUFFIX);
				m_MacauFile.Open();
				m_MacauFile.Write(DateTime.Now.ToString("HH:mm:ss") + " DeleteMslot.cs.GetMslotMatch(): " + ex.ToString());
				m_MacauFile.Close();
				sRtn = (string)HttpContext.Current.Application["accessErrorMsg"];
			}

			return sRtn;
		}

		public int Delete() {
			const string REGION = "澳門";
			char[] delimiter = new char[] {','};
			int iRecIndex;
			int iDeletedItemLen;
			int iUpdIndex = 0;
			int iSuccessUpd = 0;
			string sDeleteSELFWIN_INFO = null;
			string sDeleteWINODDS_INFO = null;
			string sDeleteASIAODDSMATCH_INFO = null;
			string sDeleteOTHERODDSINFO = null;
			string[] arrMatchID;
			string[] arrSendRequired;
			string[] arrLeague;
			string[] arrHost;
			string[] arrGuest;

			try {
				arrSendRequired = HttpContext.Current.Request.Form["deleteItem"].Split(delimiter);
				iDeletedItemLen = arrSendRequired.Length;
			} catch(Exception) {
				arrSendRequired = new string[0];
				iDeletedItemLen = 0;
			}
			arrMatchID = HttpContext.Current.Request.Form["MatchID"].Split(delimiter);
			arrLeague = HttpContext.Current.Request.Form["League"].Split(delimiter);
			arrHost = HttpContext.Current.Request.Form["Host"].Split(delimiter);
			arrGuest = HttpContext.Current.Request.Form["Guest"].Split(delimiter);

			if(iDeletedItemLen > 0) {
				try {
					sDeleteSELFWIN_INFO = "delete from SELFWIN_INFO where IMATCH_ID in (";
					sDeleteWINODDS_INFO = "delete from WINODDS_INFO where IMATCH_ID in (";
					sDeleteASIAODDSMATCH_INFO = "delete from ASIAODDSMATCH_INFO where IMATCH_ID in (";
					for(iRecIndex = 0; iRecIndex < iDeletedItemLen; iRecIndex++) {
						iUpdIndex = Convert.ToInt32(arrSendRequired[iRecIndex]);
						if(iRecIndex > 0) {
							sDeleteSELFWIN_INFO += ",";
							sDeleteWINODDS_INFO += ",";
							sDeleteASIAODDSMATCH_INFO += ",";
						}
						sDeleteSELFWIN_INFO += arrMatchID[iUpdIndex];
						sDeleteWINODDS_INFO += arrMatchID[iUpdIndex];
						sDeleteASIAODDSMATCH_INFO += arrMatchID[iUpdIndex];

						sDeleteOTHERODDSINFO = "delete from OTHERODDSINFO where COMPANY='" + REGION + "' AND LEAGUE='" + arrLeague[iUpdIndex] + "' AND HOST='" + arrHost[iUpdIndex] + "' AND GUEST='" + arrGuest[iUpdIndex] + "'";
						m_MacauDB.ExecuteNonQuery(sDeleteOTHERODDSINFO);
						iSuccessUpd++;
					}
					sDeleteSELFWIN_INFO += ")";
					sDeleteWINODDS_INFO += ")";
					sDeleteASIAODDSMATCH_INFO += ")";

					m_MacauDB.ExecuteNonQuery(sDeleteSELFWIN_INFO);
					m_MacauDB.ExecuteNonQuery(sDeleteWINODDS_INFO);
					m_MacauDB.ExecuteNonQuery(sDeleteASIAODDSMATCH_INFO);
					m_MacauDB.Dispose();
				} catch(Exception ex) {
					iSuccessUpd = -1;
					m_MacauFile.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
					m_MacauFile.SetFileName(0,LOGFILESUFFIX);
					m_MacauFile.Open();
					m_MacauFile.Write(DateTime.Now.ToString("HH:mm:ss") + " DeleteMslot.cs.Delete(): " + ex.ToString());
					m_MacauFile.Close();
				}
			}
			//write log
			m_MacauFile.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
			m_MacauFile.SetFileName(0,LOGFILESUFFIX);
			m_MacauFile.Open();
			m_MacauFile.Write(DateTime.Now.ToString("HH:mm:ss") + " DeleteMslot.cs: Delete " + iSuccessUpd.ToString() + " matches (" + HttpContext.Current.Session["user_name"] + ")");
			m_MacauFile.Close();

			return iSuccessUpd;
		}
	}
}