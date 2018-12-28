/*
Objective:
Import live odds data from Asia Sports

Last updated:
14 Oct 2003 by Chapman

C#.NET complier statement:
csc /t:library /out:..\bin\LiveOddsImport.dll /r:..\bin\DBManager.dll;..\bin\Files.dll LiveOddsImport.cs
*/

using System;
using System.Data.OleDb;
using System.Reflection;
using System.Text;
using System.Web;
using TDL.DB;
using TDL.IO;

[assembly:AssemblyCompany("TDSL")]
[assembly:AssemblyCopyright("(c) 2003 TDSL. All rights reserved. Created on 14 Oct 2003.")]
[assembly:AssemblyDescription("現場賠率 -> 匯入賽事")]
[assembly:AssemblyProduct("Sports.NET")]
[assembly:AssemblyTitle("Sports.NET DLL")]
[assembly:AssemblyVersion("3.0.*")]
namespace SportsUtil {
	public class LiveOddsImport {
		const string LOGFILESUFFIX = "log";
		int m_RecordCount = 0;
		string m_Region;
		OleDbDataReader m_SportsOleReader;
		DBManager m_SportsDBMgr;
		Files m_SportsLog;
		StringBuilder SQLString;

		public LiveOddsImport(string Connection) {
			m_SportsDBMgr = new DBManager();
			m_SportsDBMgr.ConnectionString = Connection;
			m_SportsLog = new Files();
			SQLString = new StringBuilder();
			m_Region = null;
		}

		public int NumberOfRecords {
			get {
				return m_RecordCount;
			}
		}

		public string RegionName {
			get {
				return m_Region;
			}
		}

		public string GetAsiaMatches() {
			string sRecordStr;
			string sRegionID;
			StringBuilder HTMLString = new StringBuilder();

			try {
				sRegionID = HttpContext.Current.Request.QueryString["RegionID"];

				SQLString.Remove(0,SQLString.Length);
				SQLString.Append("select CREGION from LIVEODDS_CFG where IREGION_ID=");
				SQLString.Append(sRegionID);
				m_Region = m_SportsDBMgr.ExecuteQueryString(SQLString.ToString());
				m_SportsDBMgr.Close();

				m_SportsOleReader = m_SportsDBMgr.ExecuteQuery("select game.MATCHDATE, game.MATCHTIME, game.LEAGUE, game.LEAGLONG, game.HOST, game.GUEST, game.FIELD, game.HOST_HANDI, game.MATCH_CNT from GAMEINFO game, LEAGINFO leag where game.LEAGLONG = leag.leagname order by leag.LEAG_ORDER, game.MATCHDATE, game.MATCHTIME");
				while(m_SportsOleReader.Read()) {
					HTMLString.Append("<tr align=\"center\"><td><input type=\"checkbox\" name=\"importItem\" value=\"");
					HTMLString.Append(m_RecordCount);
					HTMLString.Append("\"></td>");

					//Match Date and Time (Hidden Field)
					sRecordStr = m_SportsOleReader.GetString(0);
					HTMLString.Append("<td><input type=\"hidden\" name=\"MatchDate\" value=\"");
					HTMLString.Append(sRecordStr);
					HTMLString.Append("\">");
					sRecordStr = sRecordStr.Insert(4,"/");
					sRecordStr = sRecordStr.Insert(7,"/");
					HTMLString.Append(sRecordStr);
					HTMLString.Append("</td>");
					sRecordStr = m_SportsOleReader.GetString(1);
					HTMLString.Append("<td><input type=\"hidden\" name=\"MatchTime\" value=\"");
					HTMLString.Append(sRecordStr);
					HTMLString.Append("\">");
					sRecordStr = sRecordStr.Insert(2,":");
					HTMLString.Append(sRecordStr);
					HTMLString.Append("</td>");

					//Alias and League
					sRecordStr = m_SportsOleReader.GetString(2).Trim();
					HTMLString.Append("<td><input type=\"hidden\" name=\"Alias\" value=\"");
					HTMLString.Append(sRecordStr);
					HTMLString.Append("\">");
					sRecordStr = m_SportsOleReader.GetString(3).Trim();
					HTMLString.Append(sRecordStr);
					HTMLString.Append("<input type=\"hidden\" name=\"League\" value=\"");
					HTMLString.Append(sRecordStr);
					HTMLString.Append("\"></td>");

					//Host
					sRecordStr = m_SportsOleReader.GetString(4).Trim();
					HTMLString.Append("<td>");
					HTMLString.Append(sRecordStr);
					HTMLString.Append("<input type=\"hidden\" name=\"Host\" value=\"");
					HTMLString.Append(sRecordStr);
					HTMLString.Append("\"></td>");

					//Guest
					sRecordStr = m_SportsOleReader.GetString(5).Trim();
					HTMLString.Append("<td>");
					HTMLString.Append(sRecordStr);
					HTMLString.Append("<input type=\"hidden\" name=\"Guest\" value=\"");
					HTMLString.Append(sRecordStr);
					HTMLString.Append("\">");

					//Match Field (Hidden field)
					sRecordStr = "";
					if(!m_SportsOleReader.IsDBNull(6)) {
						sRecordStr = m_SportsOleReader.GetString(6).Trim();
						if(sRecordStr.Equals("-1")) sRecordStr = "";
					}
					HTMLString.Append("<input type=\"hidden\" name=\"MatchField\" value=\"");
					HTMLString.Append(sRecordStr);
					HTMLString.Append("\">");

					sRecordStr = "";
					if(!m_SportsOleReader.IsDBNull(7)) {
						sRecordStr = m_SportsOleReader.GetString(7).Trim();
						if(sRecordStr.Equals("-1")) sRecordStr = "";
					}
					HTMLString.Append("<input type=\"hidden\" name=\"HostHandicap\" value=\"");
					HTMLString.Append(sRecordStr);
					HTMLString.Append("\">");

					//Match Count
					HTMLString.Append("<input type=\"hidden\" name=\"MatchCount\" value=\"");
					HTMLString.Append(m_SportsOleReader.GetInt32(8).ToString());
					HTMLString.Append("\"></td></tr>");
					m_RecordCount++;
				}
				m_SportsDBMgr.Close();
				m_SportsOleReader.Close();
				m_SportsDBMgr.Dispose();
				HTMLString.Append("<input type=\"hidden\" name=\"RecordCount\" value=\"");
				HTMLString.Append(m_RecordCount.ToString());
				HTMLString.Append("\">");
				HTMLString.Append("<input type=\"hidden\" name=\"RegionID\" value=\"");
				HTMLString.Append(sRegionID);
				HTMLString.Append("\">");
			} catch(Exception ex) {
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " LiveOddsImport.cs.GetAsiaMatches(): " + ex.ToString());
				m_SportsLog.Close();
				HTMLString.Remove(0,HTMLString.Length);
				HTMLString.Append((string)HttpContext.Current.Application["accessErrorMsg"]);
			}

			return HTMLString.ToString();
		}

		public int Import() {
			bool bExisted = false;
			int iOrder = 0, iRecIndex, iSendRequiredLen, iMatchCnt = 0, iUpdIndex = 0, iSuccessUpd = 0;
			string sMatchDate, sMatchTime, sLeague, sAlias, sHost, sGuest, sMatchField, sHostHandicap, sRegionID;
			char[] delimiter = new char[] {','};
			string[] arrMatchCount, arrSendRequired, arrMatchDate, arrMatchTime, arrLeague, arrAlias, arrHost, arrGuest;
			string[] arrMatchField, arrHostHandicap;

			try {
				try {
					arrSendRequired = HttpContext.Current.Request.Form["importItem"].Split(delimiter);
					iSendRequiredLen = arrSendRequired.Length;
				} catch(Exception) {
					arrSendRequired = new string[0];
					iSendRequiredLen = 0;
				}
				arrMatchCount = HttpContext.Current.Request.Form["MatchCount"].Split(delimiter);
				arrMatchDate = HttpContext.Current.Request.Form["MatchDate"].Split(delimiter);
				arrMatchTime = HttpContext.Current.Request.Form["MatchTime"].Split(delimiter);
				arrAlias = HttpContext.Current.Request.Form["Alias"].Split(delimiter);
				arrLeague = HttpContext.Current.Request.Form["League"].Split(delimiter);
				arrHost = HttpContext.Current.Request.Form["Host"].Split(delimiter);
				arrGuest = HttpContext.Current.Request.Form["Guest"].Split(delimiter);
				arrHostHandicap = HttpContext.Current.Request.Form["HostHandicap"].Split(delimiter);
				arrMatchField = HttpContext.Current.Request.Form["MatchField"].Split(delimiter);
				sRegionID = HttpContext.Current.Request.Form["RegionID"];

				//Delete record which marked as 'D'
				SQLString.Remove(0,SQLString.Length);
				SQLString.Append("delete from LIVEODDS_INFO where CACT='D'");
				m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
				m_SportsDBMgr.Close();

				for(iRecIndex=0; iRecIndex<iSendRequiredLen; iRecIndex++) {
					iUpdIndex = Convert.ToInt32(arrSendRequired[iRecIndex]);
					iMatchCnt = Convert.ToInt32(arrMatchCount[iUpdIndex]);
					sMatchDate = arrMatchDate[iUpdIndex];
					sMatchTime = arrMatchTime[iUpdIndex];
					sLeague = arrLeague[iUpdIndex];
					sHost = arrHost[iUpdIndex];
					sGuest = arrGuest[iUpdIndex];
					sAlias = arrAlias[iUpdIndex];
					sHostHandicap = arrHostHandicap[iUpdIndex];
					sMatchField = arrMatchField[iUpdIndex];

					iOrder = 1;
					SQLString.Remove(0,SQLString.Length);
					SQLString.Append("select MAX(IMATCHORDER) from LIVEODDS_INFO where IREGION_ID=");
					SQLString.Append(sRegionID);
					m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
					if(m_SportsOleReader.Read()) {
						if(!m_SportsOleReader.IsDBNull(0)) {
							iOrder = m_SportsOleReader.GetInt32(0);
							iOrder++;
						}
					}
					m_SportsOleReader.Close();
					m_SportsDBMgr.Close();

					bExisted = false;
					SQLString.Remove(0,SQLString.Length);
					SQLString.Append("select IMATCH_CNT from LIVEODDS_INFO where CLEAGUE='");
					SQLString.Append(sLeague);
					SQLString.Append("' and CHOST='");
					SQLString.Append(sHost);
					SQLString.Append("' and CGUEST='");
					SQLString.Append(sGuest);
					SQLString.Append("' and IREGION_ID=");
					SQLString.Append(sRegionID);
					m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
					if(m_SportsOleReader.Read()) {
						if(!m_SportsOleReader.IsDBNull(0)) {
							iMatchCnt = m_SportsOleReader.GetInt32(0);
							bExisted = true;
						}
					}
					m_SportsOleReader.Close();
					m_SportsDBMgr.Close();

					//MATCHSTATUS: 0=上半; 1=下半; 2=停止
					//ODDSSTATUS: 0=正常; 1=停止; 2=暫停; 3=半休
					SQLString.Remove(0,SQLString.Length);
					if(!bExisted) {
						SQLString.Append("INSERT INTO LIVEODDS_INFO values (");
						SQLString.Append(iMatchCnt.ToString());
						SQLString.Append(",");
						SQLString.Append(sRegionID);
						SQLString.Append(",'");
						SQLString.Append(sLeague);
						SQLString.Append("','");
						SQLString.Append(sAlias);
						SQLString.Append("','");
						SQLString.Append(sHost);
						SQLString.Append("','");
						SQLString.Append(sGuest);
						SQLString.Append("','U',");
						SQLString.Append(iOrder.ToString());
						SQLString.Append(",'");
						SQLString.Append(sMatchDate);
						SQLString.Append("','");
						SQLString.Append(sMatchTime);
						SQLString.Append("','0','");
						SQLString.Append(sMatchField);
						SQLString.Append("','");
						SQLString.Append(sHostHandicap);
						SQLString.Append("',0,0,null,null,null,null,null,null,null,null,null,'0')");
					} else {
						SQLString.Append("UPDATE LIVEODDS_INFO SET CACT='U', CMATCHDATE='");
						SQLString.Append(sMatchDate);
						SQLString.Append("', CMATCHTIME='");
						SQLString.Append(sMatchTime);
						SQLString.Append("', CMATCHSTATUS='0', CMATCHFIELD='");
						SQLString.Append(sMatchField);
						SQLString.Append("', CHOST_HANDI='");
						SQLString.Append(sHostHandicap);
						SQLString.Append("', IH_SCORE=0, IG_SCORE=0, CH_WEAR=null, CG_WEAR=null, ITIMEOFGAME=null, CHANDICAP1=null, CHANDICAP2=null, CHODDS=null, CTOTALSCORE=null, CBIGODDS=null, CSMALLODDS=null, CODDSSTATUS='0' where IMATCH_CNT=");
						SQLString.Append(iMatchCnt.ToString());
						SQLString.Append(" AND IREGION_ID=");
						SQLString.Append(sRegionID);
					}
					m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
					iSuccessUpd++;
				}
				m_SportsDBMgr.Dispose();

				//write log
				m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " LiveOddsImport.cs: Import " + iSuccessUpd.ToString() + " matches from Asia Sports (" + HttpContext.Current.Session["user_name"] + ")");
				m_SportsLog.Close();
			} catch(Exception ex) {
				iSuccessUpd = -1;
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " LiveOddsImport.cs.Import(): " + ex.ToString());
				m_SportsLog.Close();
			}
			return iSuccessUpd;
		}
	}
}