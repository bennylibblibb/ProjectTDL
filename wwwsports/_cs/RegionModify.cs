/*
Objective:
Modify odds provided by other region

Last updated:
15 Nov 2004, (Fanny Cheung) Increment orderId into 3 digits
3 May 2004, Chapman Choi

C#.NET complier statement:
csc /t:library /out:..\bin\RegionModify.dll /r:..\bin\DBManager.dll;..\bin\Files.dll;..\bin\MessageClient.dll;..\bin\SportsMessage.dll RegionModify.cs
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
using TDL.Message;

[assembly:AssemblyCompany("TDSL")]
[assembly:AssemblyCopyright("(c) 2003 TDSL. All rights reserved.")]
[assembly:AssemblyDescription("其他地區 -> 賠率更新")]
[assembly:AssemblyProduct("Sports.NET")]
[assembly:AssemblyTitle("Sports.NET DLL")]
[assembly:AssemblyVersion("3.1.*")]
namespace SportsUtil {
	public class RegionModify {
		const string LOGFILESUFFIX = "log";
		int m_RecordCount = 0;
		string m_Region;
		string m_TotalOdds;
		string m_RegionID;
		OleDbDataReader m_SportsOleReader;
		DBManager m_SportsDBMgr;
		Files m_SportsLog;
		Encoding m_Big5Encoded;
		StringBuilder SQLString;
		ArrayList leagueList;

		public RegionModify(string Connection) {
			m_SportsDBMgr = new DBManager();
			m_SportsDBMgr.ConnectionString = Connection;
			m_SportsLog = new Files();
			m_Big5Encoded = Encoding.GetEncoding(950);
			SQLString = new StringBuilder();
			leagueList = new ArrayList();
		}

		public int NumberOfRecords {
			get {
				return m_RecordCount;
			}
		}

		public string RegionID {
			get {
				return m_RegionID;
			}
		}

		public string Region {
			get {
				return m_Region;
			}
		}

		public string TotalOdds {
			get {
				return m_TotalOdds;
			}
		}

		public ArrayList LeagueList {
			get {
				return leagueList;
			}
		}

		public string GetAllMatches() {
			int iLeagueGp = -1;
			int iArrayIndex = 0;
			int iStatus = 0;
			string uid;
			string sSortType;
			//string m_RegionID;
			string[] oddsArray;
			StringBuilder HTMLString = new StringBuilder();
			
			sSortType = HttpContext.Current.Request.QueryString["sort"];
			if(sSortType != null) {
				if(sSortType.Trim().Equals("")) {
					sSortType = (string)HttpContext.Current.Session["user_sortType"];
				} else {
					HttpContext.Current.Session["user_sortType"] = sSortType;
				}
			} else {
				sSortType = (string)HttpContext.Current.Session["user_sortType"];
			}
			uid = HttpContext.Current.Session["user_id"].ToString();
			m_RegionID = HttpContext.Current.Request.QueryString["RegionID"];
			oddsArray = (string[])HttpContext.Current.Application["oddsItemsArray"];
			if(m_RegionID.Equals("")) m_RegionID = "0";

			SQLString.Remove(0,SQLString.Length);
			SQLString.Append("select CREGION,CTOTAL_ODDS from OTHERREGION_CFG where IREGION_ID=");
			SQLString.Append(m_RegionID);
			try {
				m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
				if(m_SportsOleReader.Read()) {
					m_Region = m_SportsOleReader.GetString(0).Trim();
					m_TotalOdds = m_SportsOleReader.GetString(1).Trim();
				} else {
					m_Region = "";
					m_TotalOdds = "";
				}
				m_SportsOleReader.Close();
				m_SportsDBMgr.Close();

				//League Count
				SQLString.Remove(0,SQLString.Length);
				SQLString.Append("select distinct ALIAS from OTHERODDSINFO, leaginfo where ACT='U' AND WEBACT='V' AND COMPANY=(select CREGION from OTHERREGION_CFG where IREGION_ID=");
				SQLString.Append(m_RegionID);
				SQLString.Append(") AND leaginfo.leag_id in (select cleag_id from userprofile_info where iuser_id=");
				SQLString.Append(uid);
				SQLString.Append(") AND leaginfo.alias=OTHERODDSINFO.ALIAS order by leaginfo.leag_order");
				m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
				while(m_SportsOleReader.Read()) {
					leagueList.Add(m_SportsOleReader.GetString(0).Trim());
				}
				m_SportsOleReader.Close();
				m_SportsDBMgr.Close();
				leagueList.TrimToSize();

				SQLString.Remove(0,SQLString.Length);
				SQLString.Append("select MATCHDATE, MATCHTIME, LEAGUE, MATCH_CNT, MATCH_ID, ALIAS, HOST, GUEST, FIELD, HOST_HANDI, HANDICAP1, HANDICAP2, ODDS_UP, ODDS_DOWN, STATUS, EU_ODDS_WIN, EU_ODDS_LOSS, EU_ODDS_DRAW from OTHERODDSINFO, leaginfo where ACT='U' AND WEBACT='V' AND COMPANY=(select CREGION from OTHERREGION_CFG where IREGION_ID=");
				SQLString.Append(m_RegionID);
				SQLString.Append(") AND leaginfo.leag_id in (select cleag_id from userprofile_info where iuser_id=");
				SQLString.Append(uid);
				SQLString.Append(") AND leaginfo.alias=OTHERODDSINFO.ALIAS ");
				if(sSortType.Equals("0")) {		//order by league and then match date and time
					SQLString.Append("order by OTHERODDSINFO.ALIAS, OTHERODDSINFO.MATCH_ID, OTHERODDSINFO.MATCHDATE, OTHERODDSINFO.MATCHTIME");
				} else {		//order by order id
					SQLString.Append("order by OTHERODDSINFO.MATCH_ID, OTHERODDSINFO.MATCHDATE, OTHERODDSINFO.MATCHTIME, OTHERODDSINFO.ALIAS");
				}
				m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
				while(m_SportsOleReader.Read()) {
					HTMLString.Append("<tr align=\"center\">");
					//Match Date and Time (Hidden Field)
					HTMLString.Append("<td><input type=\"hidden\" name=\"MatchDate\" value=\"");
					HTMLString.Append(m_SportsOleReader.GetString(0));
					HTMLString.Append("\"><input type=\"hidden\" name=\"MatchTime\" value=\"");
					HTMLString.Append(m_SportsOleReader.GetString(1));
					HTMLString.Append("\">");

					//League and Match Count(Hidden Field)
					HTMLString.Append("<input type=\"hidden\" name=\"League\" value=\"");
					HTMLString.Append(m_SportsOleReader.GetString(2).Trim());
					HTMLString.Append("\"><input type=\"hidden\" name=\"MatchCount\" value=\"");
					HTMLString.Append(m_SportsOleReader.GetInt32(3).ToString());
					HTMLString.Append("\">");

					//Order ID
					HTMLString.Append("<input type=\"text\" name=\"orderID\" maxlength=\"3\" size=\"1\" value=\"");
					if(!m_SportsOleReader.IsDBNull(4)) {
						if(!m_SportsOleReader.GetString(4).Trim().Equals("-1")) {
							if(!m_SportsOleReader.GetString(4).Trim().Equals("")) {
								HTMLString.Append(Convert.ToInt32(m_SportsOleReader.GetString(4).Trim()).ToString());
							}
						}
					}
					HTMLString.Append("\" onChange=\"onOrder_IDChanged(");
					HTMLString.Append(m_RecordCount.ToString());
					HTMLString.Append(")\" tabindex=\"");
					HTMLString.Append((m_RecordCount+1).ToString());
					HTMLString.Append("\"></td>");

					//Alias
					iLeagueGp = leagueList.IndexOf(m_SportsOleReader.GetString(5).Trim());
					HTMLString.Append("<td>");
					HTMLString.Append(m_SportsOleReader.GetString(5).Trim());
					HTMLString.Append("<input type=\"hidden\" name=\"Alias\" value=\"");
					HTMLString.Append(m_SportsOleReader.GetString(5).Trim());
					HTMLString.Append("\"></td>");

					//Host
					HTMLString.Append("<td>");
					HTMLString.Append(m_SportsOleReader.GetString(6).Trim());
					HTMLString.Append("<input type=\"hidden\" name=\"Host\" value=\"");
					HTMLString.Append(m_SportsOleReader.GetString(6).Trim());
					HTMLString.Append("\"></td>");

					//Guest
					HTMLString.Append("<td>");
					HTMLString.Append(m_SportsOleReader.GetString(7).Trim());
					HTMLString.Append("<input type=\"hidden\" name=\"Guest\" value=\"");
					HTMLString.Append(m_SportsOleReader.GetString(7).Trim());
					HTMLString.Append("\"></td>");

					//Match Field
					HTMLString.Append("<td><input type=\"checkbox\" name=\"MatchField\" ");
					if(m_SportsOleReader.GetString(8).Equals("M")) HTMLString.Append("checked");
					HTMLString.Append(" onClick=\"onChecked(");
					HTMLString.Append(m_RecordCount.ToString());
					HTMLString.Append(")\" value=\"");
					HTMLString.Append(m_RecordCount.ToString());
					HTMLString.Append("\"></td>");

					//Host Handicap
					HTMLString.Append("<td><input type=\"checkbox\" name=\"HostHandicap\" ");
					if(m_SportsOleReader.GetString(9).Equals("1")) HTMLString.Append("checked");
					HTMLString.Append(" onClick=\"onChecked(");
					HTMLString.Append(m_RecordCount.ToString());
					HTMLString.Append(")\" value=\"");
					HTMLString.Append(m_RecordCount.ToString());
					HTMLString.Append("\"></td>");

					//Score Handicap1/Handicap2
					HTMLString.Append("<td><input type=\"text\" name=\"Handicap1\" maxlength=\"3\" size=\"1\" value=\"");
					HTMLString.Append(m_SportsOleReader.GetString(10).Trim());
					HTMLString.Append("\" onChange=\"Handicap1Validity(");
					HTMLString.Append(m_RecordCount.ToString());
					HTMLString.Append(")\">/<input type=\"text\" name=\"Handicap2\" maxlength=\"3\" size=\"1\" value=\"");
					HTMLString.Append(m_SportsOleReader.GetString(11).Trim());
					HTMLString.Append("\" onChange=\"Handicap2Validity(");
					HTMLString.Append(m_RecordCount.ToString());
					HTMLString.Append(")\"></td>");

					//Up Odds/Down Odds
					HTMLString.Append("<td><input type=\"text\" name=\"UpOdds\" maxlength=\"5\" size=\"2\" value=\"");
					HTMLString.Append(m_SportsOleReader.GetString(12).Trim());
					HTMLString.Append("\" onChange=\"UpOddsValidity(");
					HTMLString.Append(m_RecordCount.ToString());
					HTMLString.Append(")\">/<input type=\"text\" name=\"DownOdds\" maxlength=\"5\" size=\"2\" value=\"");
					HTMLString.Append(m_SportsOleReader.GetString(13).Trim());
					HTMLString.Append("\" onChange=\"DownOddsValidity(");
					HTMLString.Append(m_RecordCount.ToString());
					HTMLString.Append(")\">&nbsp;");

					//Status
					iArrayIndex = 0;
					iStatus = Convert.ToInt32(m_SportsOleReader.GetString(14)) - 1;
					HTMLString.Append("<select name=\"Status\" onChange=\"onStatusChanged(");
					HTMLString.Append(m_RecordCount.ToString());
					HTMLString.Append(")\"><option value=");
					HTMLString.Append(m_SportsOleReader.GetString(14));
					HTMLString.Append(">");
					HTMLString.Append(oddsArray[iStatus]);
					foreach(String sItem in oddsArray) {
						if(!sItem.Equals(oddsArray[iStatus]) && iArrayIndex < 3) {
							HTMLString.Append("<option value=");
							HTMLString.Append((iArrayIndex+1).ToString());
							HTMLString.Append(">");
							HTMLString.Append(sItem);
						}
						iArrayIndex++;
					}
					HTMLString.Append("</select></td>");

					//Euro Win/Loss Draw
					HTMLString.Append("<td><input type=\"text\" name=\"EuroWinOdds\" maxlength=\"4\" size=\"1\" value=\"");
					HTMLString.Append(m_SportsOleReader.GetString(15).Trim());
					HTMLString.Append("\" onChange=\"EuroWinOddsValidity(");
					HTMLString.Append(m_RecordCount.ToString());
					HTMLString.Append(")\">/<input type=\"text\" name=\"EuroLossOdds\" maxlength=\"4\" size=\"1\" value=\"");
					HTMLString.Append(m_SportsOleReader.GetString(16).Trim());
					HTMLString.Append("\" onChange=\"EuroLossOddsValidity(");
					HTMLString.Append(m_RecordCount.ToString());
					HTMLString.Append(")\">&nbsp;<input type=\"text\" name=\"EuroDrawOdds\" maxlength=\"4\" size=\"1\" value=\"");
					HTMLString.Append(m_SportsOleReader.GetString(17).Trim());
					HTMLString.Append("\" onChange=\"EuroDrawOddsValidity(");
					HTMLString.Append(m_RecordCount.ToString());
					HTMLString.Append(")\"></td>");

					//Alert
					//HTMLString.Append("<td><input type=\"checkbox\" name=\"alertChk\" onClick=\"onChecked(" + m_RecordCount.ToString() + ")\" value=\"" + m_RecordCount.ToString() + "\"></td>");

					//MUST Send
					HTMLString.Append("<td style=\"background-color:#FFFAF0\"><input type=\"checkbox\" name=\"MUSTSendChk\" value=\"");
					HTMLString.Append(m_RecordCount.ToString());
					HTMLString.Append("\"></td>");

					//Hidden Check
					HTMLString.Append("<td style=\"background-color:#FFC39C\"><input type=\"checkbox\" name=\"hiddenChk\" value=\"");
					HTMLString.Append(m_RecordCount.ToString());
					HTMLString.Append("\" >");

					//Hidden checkbox for league checked
					HTMLString.Append("<input type=\"hidden\" name=\"LeagueChk_");
					HTMLString.Append(iLeagueGp.ToString());
					HTMLString.Append("\" value=\"");
					HTMLString.Append(m_RecordCount.ToString());
					HTMLString.Append("\"></td></tr>");
					m_RecordCount++;
				}
				m_SportsOleReader.Close();
				m_SportsDBMgr.Close();
				m_SportsDBMgr.Dispose();
				HTMLString.Append("<input type=\"hidden\" name=\"RecordCount\" value=\"");
				HTMLString.Append(m_RecordCount.ToString());
				HTMLString.Append("\"><input type=\"hidden\" name=\"Region\" value=\"");
				HTMLString.Append(m_Region);
				HTMLString.Append("\">");
			} catch(Exception ex) {
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " RegionModify.cs.GetAllMatches(): " + ex.ToString());
				m_SportsLog.Close();
				HTMLString.Remove(0,HTMLString.Length);
				HTMLString.Append((string)HttpContext.Current.Application["accessErrorMsg"]);
			}

			return HTMLString.ToString();
		}

		public int UpdateMatches(string sType) {
			int iUpdIndex = 0, iRecUpd = 0, iTempItem = 0;
			int iMustSendLen, iHiddenLen;
			string sTempItem;
			string sRegion;
			string sOrder = "";
			string sCurrentTimestamp = null;
			string sBatchJob = null;
			char[] delimiter = new char[] {','};
			string[] arrMustSend;
			string[] arrOrderID;
			string[] arrMatchCnt;
			string[] arrHidden;
			string[] arrSendToPager;
			//Replicator MatchReplicator = new Replicator(HttpContext.Current.Application["RepDBConnectionString"].ToString());
			//MatchReplicator.ApplicationType = 1;

			//SportsMessage object message
			SportsMessage sptMsg = new SportsMessage();

			try {
				arrSendToPager = HttpContext.Current.Request.Form["SendToPager"].Split(delimiter);
			} catch(Exception) {
				arrSendToPager = new string[0];
			}
			sRegion = HttpContext.Current.Request.Form["Region"];

			try {
				arrOrderID = HttpContext.Current.Request.Form["orderID"].Split(delimiter);
				arrMatchCnt = HttpContext.Current.Request.Form["MatchCount"].Split(delimiter);
			} catch(Exception) {
				arrOrderID = new string[0];
				arrMatchCnt = new string[0];
			}

			try {
				arrHidden = HttpContext.Current.Request.Form["hiddenChk"].Split(delimiter);
				iHiddenLen = arrHidden.Length;
			} catch(Exception) {
				arrHidden = new string[0];
				iHiddenLen = 0 ;
			}

			try {
				arrMustSend = HttpContext.Current.Request.Form["MUSTSendChk"].Split(delimiter);
				iMustSendLen = arrMustSend.Length;
			}	catch(Exception) {
				arrMustSend = new string[0];
				iMustSendLen = 0;
			}

			try {
				switch(sType) {
					case "SEND":	//case: Sending match
						//Delcare variable used in message notify
						string[] arrQueueNames;
						string[] arrRemotingPath;
						string[] arrMessageTypes;
						arrQueueNames = (string[])HttpContext.Current.Application["QueueItems"];
						arrRemotingPath = (string[])HttpContext.Current.Application["RemotingItems"];
						arrMessageTypes = (string[])HttpContext.Current.Application["NotifyMessageTypes"];
						MessageClient msgClt = new MessageClient();

						int iLogUpd = 0;
						string sHandicap1, sHandicap2, sUpOdds, sDownOdds, sEWOdds, sELOdds, sEDOdds, sHostHandicap, sMatchField;
						string[] arrMatchDate, arrMatchTime, arrMatchField, arrHostHandicap, arrLeague, arrAlias, arrHost, arrGuest;
						string[] arrHandicap1, arrHandicap2, arrUpOdds, arrDownOdds, arrStatus, arrEWOdds, arrELOdds, arrEDOdds, arrMsgType, arrOddsStatus;
						StringBuilder LogSQLString = new StringBuilder();
						DBManager logDBMgr = new DBManager();
						logDBMgr.ConnectionString = m_SportsDBMgr.ConnectionString;

						arrMatchDate = HttpContext.Current.Request.Form["MatchDate"].Split(delimiter);
						arrMatchTime = HttpContext.Current.Request.Form["MatchTime"].Split(delimiter);
						arrLeague = HttpContext.Current.Request.Form["League"].Split(delimiter);
						arrAlias = HttpContext.Current.Request.Form["Alias"].Split(delimiter);
						arrHost = HttpContext.Current.Request.Form["Host"].Split(delimiter);
						arrGuest = HttpContext.Current.Request.Form["Guest"].Split(delimiter);
						arrHandicap1 = HttpContext.Current.Request.Form["Handicap1"].Split(delimiter);
						arrHandicap2 = HttpContext.Current.Request.Form["Handicap2"].Split(delimiter);
						arrUpOdds = HttpContext.Current.Request.Form["UpOdds"].Split(delimiter);
						arrDownOdds = HttpContext.Current.Request.Form["DownOdds"].Split(delimiter);
						arrStatus = HttpContext.Current.Request.Form["Status"].Split(delimiter);
						arrEWOdds = HttpContext.Current.Request.Form["EuroWinOdds"].Split(delimiter);
						arrELOdds = HttpContext.Current.Request.Form["EuroLossOdds"].Split(delimiter);
						arrEDOdds = HttpContext.Current.Request.Form["EuroDrawOdds"].Split(delimiter);
						try {
							arrMatchField = HttpContext.Current.Request.Form["MatchField"].Split(delimiter);
						}	catch(Exception) {
							arrMatchField = new string[0];
						}
						try {
							arrHostHandicap = HttpContext.Current.Request.Form["HostHandicap"].Split(delimiter);
						}	catch(Exception) {
							arrHostHandicap = new string[0];
						}

						arrMsgType = (string[])HttpContext.Current.Application["messageType"];
						arrOddsStatus = (string[])HttpContext.Current.Application["oddsItemsArray"];
						//sINIFileName = HttpContext.Current.Session["user_name"].ToString() + "." + arrMsgType[19] + ".ini";

						sCurrentTimestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
						sBatchJob = DateTime.Now.ToString("yyMMddHHmmss_fffffff") + "." + HttpContext.Current.Session["user_name"].ToString() + "." + arrMsgType[19] + ".ini";
						for(iRecUpd=0;iRecUpd<iMustSendLen;iRecUpd++) {
							//update the checked(MUSTSend) records only
							iUpdIndex = Convert.ToInt32(arrMustSend[iRecUpd]);

							sHandicap1 = arrHandicap1[iUpdIndex];
							if(sHandicap1 == null) sHandicap1 = "";
							else sHandicap1 = sHandicap1.Trim();

							sHandicap2 = arrHandicap2[iUpdIndex];
							if(sHandicap2 == null) sHandicap2 = "";
							else sHandicap2 = sHandicap2.Trim();

							sUpOdds = arrUpOdds[iUpdIndex];
							if(sUpOdds == null) sUpOdds = "";
							else sUpOdds = sUpOdds.Trim();

							sDownOdds = arrDownOdds[iUpdIndex];
							if(sDownOdds == null) sDownOdds = "";
							else sDownOdds = sDownOdds.Trim();

							sEWOdds = arrEWOdds[iUpdIndex];
							if(sEWOdds == null) sEWOdds = "";
							else sEWOdds = sEWOdds.Trim();

							sELOdds = arrELOdds[iUpdIndex];
							if(sELOdds == null) sELOdds = "";
							else sELOdds = sELOdds.Trim();
							sELOdds = arrELOdds[iUpdIndex].Trim();

							sEDOdds = arrEDOdds[iUpdIndex];
							if(sEDOdds == null) sEDOdds = "";
							else sEDOdds = sEDOdds.Trim();

							sTempItem = "";
							for(int iFieldUpd=0;iFieldUpd<arrMatchField.Length;iFieldUpd++) {
								if(arrMatchField[iFieldUpd] == iUpdIndex.ToString()) {
									sTempItem = "1";
									break;
								}
							}
							if(sTempItem.Equals("1")) sMatchField = "M";
							else sMatchField = "H";

							sHostHandicap = "";
							for(int iHandicapUpd=0;iHandicapUpd<arrHostHandicap.Length;iHandicapUpd++) {
								if(arrHostHandicap[iHandicapUpd] == iUpdIndex.ToString()) {
									sHostHandicap = "1";
									break;
								}
							}
							if(!sHostHandicap.Equals("1")) sHostHandicap = "0";

							//update OTHERODDSINFO
							SQLString.Remove(0,SQLString.Length);
							SQLString.Append("update OTHERODDSINFO set FIELD='");
							SQLString.Append(sMatchField);
							SQLString.Append("', HOST_HANDI='");
							SQLString.Append(sHostHandicap);
							SQLString.Append("', HANDICAP1='");
							SQLString.Append(sHandicap1);
							SQLString.Append("', HANDICAP2='");
							SQLString.Append(sHandicap2);
							SQLString.Append("', ODDS_UP='");
							SQLString.Append(sUpOdds);
							SQLString.Append("', ODDS_DOWN='");
							SQLString.Append(sDownOdds);
							SQLString.Append("', EU_ODDS_WIN='");
							SQLString.Append(sEWOdds);
							SQLString.Append("', EU_ODDS_LOSS='");
							SQLString.Append(sELOdds);
							SQLString.Append("', EU_ODDS_DRAW='");
							SQLString.Append(sEDOdds);
							SQLString.Append("', STATUS='");
							SQLString.Append(arrStatus[iUpdIndex]);
							SQLString.Append("' where COMPANY='");
							SQLString.Append(sRegion);
							SQLString.Append("' AND MATCH_CNT=");
							SQLString.Append(arrMatchCnt[iUpdIndex]);
							m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
							m_SportsDBMgr.Close();
							//MatchReplicator.Replicate("delete from OTHERODDSINFO where ACT='D'");
							//MatchReplicator.Replicate(SQLString.ToString());

							//generate Other Odds INI file if required
							if(arrSendToPager.Length>0 && iMustSendLen>0) {
								iLogUpd++;
								LogSQLString.Remove(0,LogSQLString.Length);
								LogSQLString.Append("insert into LOG_OTHERODDS (TIMEFLAG, SECTION, COMPANY, LEAGUE, ALIAS, HOST, GUEST, MATCHDATE, MATCHTIME, MATCHFIELD, ACT, HOSTHANDICAP, SCOREHANDICAP, ODDS_UP, ODDS_DOWN, EU_ODDS1, EU_ODDS2, ALERT, LIVESTATUS, BATCHJOB) values ('");
								LogSQLString.Append(sCurrentTimestamp);
								LogSQLString.Append("','OTHERCOMPANY_','");
								LogSQLString.Append(sRegion);
								LogSQLString.Append("','");
								LogSQLString.Append(arrLeague[iUpdIndex]);
								LogSQLString.Append("','");
								LogSQLString.Append(arrAlias[iUpdIndex]);
								LogSQLString.Append("','");
								LogSQLString.Append(arrHost[iUpdIndex]);
								LogSQLString.Append("','");
								LogSQLString.Append(arrGuest[iUpdIndex]);
								LogSQLString.Append("','");
								LogSQLString.Append(arrMatchDate[iUpdIndex]);
								LogSQLString.Append("','");
								LogSQLString.Append(arrMatchTime[iUpdIndex]);
								LogSQLString.Append("','");
								LogSQLString.Append(sMatchField);
								LogSQLString.Append("','U','");
								LogSQLString.Append(sHostHandicap);
								LogSQLString.Append("','");
								if(sHandicap1.Equals("")) {
									sHandicap1 = "-1";
								}	else {
									if(!sHandicap2.Equals("")) {
										sHandicap1 += "/" + sHandicap2;
									}
								}
								LogSQLString.Append(sHandicap1);
								LogSQLString.Append("','");
								if(sUpOdds.Equals("")) sUpOdds = "-1";
								LogSQLString.Append(sUpOdds);
								LogSQLString.Append("','");
								if(sDownOdds.Equals("")) sDownOdds = "-1";
								LogSQLString.Append(sDownOdds);
								LogSQLString.Append("','");
								if(sEWOdds.Equals("")) {
									sEWOdds = "-1";
								}	else {
									if(sELOdds.Equals("")) {
										sELOdds = "-1";
									}	else {
										sEWOdds += "/" + sELOdds;
									}
								}
								LogSQLString.Append(sEWOdds);
								LogSQLString.Append("','");
								if(sEDOdds.Equals("")) {
									sEDOdds = "-1";
								}
								LogSQLString.Append(sEDOdds);
								LogSQLString.Append("',0,'");
								LogSQLString.Append(arrOddsStatus[Convert.ToInt32(arrStatus[iUpdIndex])-1]);
								LogSQLString.Append("','");
								LogSQLString.Append(sBatchJob);
								LogSQLString.Append("')");
								logDBMgr.ExecuteNonQuery(LogSQLString.ToString());
								logDBMgr.Close();
							}	//insert into LOG_OTHERODDS
						}

						if(iLogUpd>0) {
							//Tell MessageDispatcher to generate Other Odds INI and notify other processes
							//Assign value to SportsMessage object
							//modified by Henry   12 Feb 2004
							sptMsg.Body = sBatchJob;
							sptMsg.Timestamp = DateTime.Now.ToString("yyMMddHHmmss");
							sptMsg.AppID = "07";
							sptMsg.MsgID = "21";
							sptMsg.DeviceID = new string[0];
							for(int i = 0; i < arrSendToPager.Length; i++) {
								sptMsg.AddDeviceID((string)arrSendToPager[i]);
							}
							try {
								//Notify via MSMQ
								msgClt.MessageType = arrMessageTypes[0];
								msgClt.MessagePath = arrQueueNames[0];
								msgClt.SendMessage(sptMsg);
							} catch(System.Messaging.MessageQueueException mqEx) {
								try {
									m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
									m_SportsLog.SetFileName(0,LOGFILESUFFIX);
									m_SportsLog.Open();
									m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " MSMQ ERROR: Other Region Odds");
									m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " RegionModify.cs.UpdateMatches(): Notify via MSMQ throws MessageQueueException: " + mqEx.ToString());
									m_SportsLog.Close();

									//If MSMQ fail, notify via .NET Remoting
									msgClt.MessageType = arrMessageTypes[1];
									msgClt.MessagePath = arrRemotingPath[0];
									if(!msgClt.SendMessage((object)sptMsg)) {
										m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
										m_SportsLog.SetFileName(0,LOGFILESUFFIX);
										m_SportsLog.Open();
										m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Remoting ERROR: Other Region Odds");
										m_SportsLog.Close();
									}
								}	catch(Exception ex) {
									m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
									m_SportsLog.SetFileName(0,LOGFILESUFFIX);
									m_SportsLog.Open();
									m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Remoting ERROR: Other Region Odds");
									m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " RegionModify.cs.UpdateMatches(): Notify via .NET Remoting throws exception: " + ex.ToString());
									m_SportsLog.Close();
								}
							} catch(Exception ex) {
								m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
								m_SportsLog.SetFileName(0,LOGFILESUFFIX);
								m_SportsLog.Open();
								m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " RegionModify.cs.UpdateMatches(): Notify via MSMQ throws exception: " + ex.ToString());
								m_SportsLog.Close();
							}
						}

						if(iRecUpd > 0) {
							//MatchReplicator.Dispose();
							m_SportsDBMgr.Dispose();
						}
/*
						//write log
						m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
						m_SportsLog.SetFileName(0,LOGFILESUFFIX);
						m_SportsLog.Open();
						m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " RegionModify.cs: Send " + iRecUpd.ToString() + " matches (" + HttpContext.Current.Session["user_name"] + ")");
						m_SportsLog.Close();
*/
						break;
					case "SORT":	//case: Sorting match order
						for(iRecUpd=0;iRecUpd<arrOrderID.Length;iRecUpd++) {
							sTempItem = arrOrderID[iRecUpd];
							if(sTempItem == null) {
								sOrder = "";
							}	else if(sTempItem.Equals("")) {
								sOrder = "";
							}	else {
								iTempItem = Convert.ToInt32(sTempItem);
								if(iTempItem < 10) sOrder = "00" + iTempItem.ToString();
								else if(iTempItem < 100) sOrder = "0" + iTempItem.ToString();
								else sOrder = iTempItem.ToString();
							}
							SQLString.Remove(0,SQLString.Length);
							SQLString.Append("update OTHERODDSINFO set MATCH_ID='");
							SQLString.Append(sOrder);
							SQLString.Append("' where COMPANY='");
							SQLString.Append(sRegion);
							SQLString.Append("' AND MATCH_CNT=");
							SQLString.Append(arrMatchCnt[iRecUpd].Trim());
							m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
							m_SportsDBMgr.Close();
							//MatchReplicator.Replicate(SQLString.ToString());
						}

						if(iRecUpd > 0) {
							//MatchReplicator.Dispose();
							m_SportsDBMgr.Dispose();
						}

						//write log
						m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
						m_SportsLog.SetFileName(0,LOGFILESUFFIX);
						m_SportsLog.Open();
						m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " RegionModify.cs: Sort " + iRecUpd.ToString() + " matches (" + HttpContext.Current.Session["user_name"] + ")");
						m_SportsLog.Close();
						break;
					case "HIDE":	//case: Hidden specific match
						for(iRecUpd=0;iRecUpd<iHiddenLen;iRecUpd++) {
							iUpdIndex = Convert.ToInt32(arrHidden[iRecUpd]);
							sTempItem = arrMatchCnt[iUpdIndex].Trim();
							if(iRecUpd == 0) {
								SQLString.Remove(0,SQLString.Length);
								SQLString.Append("update OTHERODDSINFO set WEBACT='X' where COMPANY='");
								SQLString.Append(sRegion);
								SQLString.Append("' AND MATCH_CNT=");
								SQLString.Append(sTempItem);
							}	else {
								SQLString.Append(" or MATCH_CNT=");
								SQLString.Append(sTempItem);
							}
							m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
							m_SportsDBMgr.Close();
							//MatchReplicator.Replicate(SQLString.ToString());
						}

						if(iRecUpd > 0) {
							//MatchReplicator.Dispose();
							m_SportsDBMgr.Dispose();
						}

						//write log
						m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
						m_SportsLog.SetFileName(0,LOGFILESUFFIX);
						m_SportsLog.Open();
						m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " RegionModify.cs: Hide " + iRecUpd.ToString() + " matches (" + HttpContext.Current.Session["user_name"] + ")");
						m_SportsLog.Close();
						break;
					case "SHOW":	//case: Set all matches to visible
						SQLString.Remove(0,SQLString.Length);
						SQLString.Append("update OTHERODDSINFO set WEBACT='V' where COMPANY='");
						SQLString.Append(sRegion);
						SQLString.Append("'");
						iRecUpd = m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
						m_SportsDBMgr.Close();
						m_SportsDBMgr.Dispose();
						//MatchReplicator.Replicate(SQLString.ToString());
						//MatchReplicator.Dispose();

						//write log
						m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
						m_SportsLog.SetFileName(0,LOGFILESUFFIX);
						m_SportsLog.Open();
						m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " RegionModify.cs: " + HttpContext.Current.Session["user_name"] + " set " + iRecUpd.ToString() + " matches to visible");
						m_SportsLog.Close();
						break;
					default:
						iRecUpd = 0;
						break;
				}
			} catch(Exception ex) {
				iRecUpd = -1;
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " RegionModify.cs.UpdateMatches(): " + ex.ToString());
				m_SportsLog.Close();
			}
			return iRecUpd;
		}

		public int UpdateTotalOdds() {
			int iResult;
			string sTotalOdds;
			string sRegion;

			sRegion = HttpContext.Current.Request.Form["Region"];
			if(sRegion == null) sRegion = "";
			sTotalOdds = HttpContext.Current.Request.Form["totalOdds"];
			if(sTotalOdds == null) sTotalOdds = "";
			SQLString.Remove(0,SQLString.Length);
			SQLString.Append("update OTHERREGION_CFG set CTOTAL_ODDS='");
			SQLString.Append(sTotalOdds);
			SQLString.Append("' where CREGION='");
			SQLString.Append(sRegion);
			SQLString.Append("'");
			try {
				iResult = m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
				//write log
				m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " RegionModify.cs: update " + sRegion + " total odds=" + sTotalOdds + " (" + HttpContext.Current.Session["user_name"] + ")");
				m_SportsLog.Close();
			} catch(Exception ex) {
				iResult = -1;
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " RegionModify.cs.UpdateTotalOdds(): " + ex.ToString());
				m_SportsLog.Close();
			}
			return iResult;
		}
	}
}