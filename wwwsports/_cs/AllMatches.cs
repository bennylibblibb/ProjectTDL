/*
Objective:
Retrieval and modify all matches information such as odds, host handicap and match field

Last updated:
15 Nov 2004, Fanny Cheung
-increase orderId into 3 digits
23 Mar 2004, Chapman Choi
23 Mar 2004 Remark Replicator code

C#.NET complier statement:
(With Replicator)
csc /t:library /out:..\bin\AllMatches.dll /r:..\bin\DBManager.dll;..\bin\Files.dll;..\bin\MessageClient.dll;..\bin\Replicator.dll;..\bin\SportsMessage.dll AllMatches.cs
(Without Replicator - current production)
csc /t:library /out:..\bin\AllMatches.dll /r:..\bin\DBManager.dll;..\bin\Files.dll;..\bin\MessageClient.dll;..\bin\SportsMessage.dll AllMatches.cs
*/

using System;
using System.Collections;
using System.Data.OleDb;
using System.Text;
using System.Reflection;
using System.Web;
using TDL.DB;
using TDL.IO;
//using TDL.Util;
using TDL.Message;

[assembly:AssemblyCompany("TDSL")]
[assembly:AssemblyCopyright("(c) 2003 TDSL. All rights reserved.")]
[assembly:AssemblyDescription("足球資訊 -> 所有賽事")]
[assembly:AssemblyProduct("Sports.NET")]
[assembly:AssemblyTitle("Sports.NET DLL")]
[assembly:AssemblyVersion("3.1.*")]
namespace SportsUtil {
	public class AllMatches {
		const string LOGFILESUFFIX = "log";
		int m_RecordCount = 0;
		OleDbDataReader m_SportsOleReader;
		DBManager m_SportsDBMgr;
		Files m_SportsLog;
		Encoding m_Big5Encoded;
		StringBuilder SQLString;
		ArrayList leagueList;

		public AllMatches(string Connection) {
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

		public ArrayList LeagueList {
			get {
				return leagueList;
			}
		}

		public string GetAllMatches() {
			int iInterval;
			int iArrayIndex = 0;
			int iStatus = 0;
			int iLeagueGp = -1;
			string uid;
			string sSortType;
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
			oddsArray = (string[])HttpContext.Current.Application["oddsItemsArray"];
			uid = HttpContext.Current.Session["user_id"].ToString();

			//League Count
			SQLString.Remove(0,SQLString.Length);
			SQLString.Append("select distinct gameinfo.LEAGUE from gameinfo, leaginfo where gameinfo.WEBACT='V' and gameinfo.ACT='U' and gameinfo.league = leaginfo.alias and leaginfo.leag_id in (select cleag_id from userprofile_info where iuser_id=");
			SQLString.Append(uid);
			SQLString.Append(") order by leaginfo.leag_order");
			m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
			while(m_SportsOleReader.Read()) {
				leagueList.Add(m_SportsOleReader.GetString(0).Trim());
			}
			m_SportsOleReader.Close();
			m_SportsDBMgr.Close();
			leagueList.TrimToSize();

			//Retrieve all match according to user profile
			SQLString.Remove(0,SQLString.Length);
			SQLString.Append("select gameinfo.MATCHDATE, gameinfo.MATCHTIME, gameinfo.LEAGLONG, gameinfo.MATCH_CNT, gameinfo.INTERVAL, gameinfo.MATCH_ID, gameinfo.LEAGUE, gameinfo.HOST, gameinfo.GUEST, gameinfo.FIELD, gameinfo.HOST_HANDI, gameinfo.T_HANDI1, gameinfo.T_HANDI2, gameinfo.TY1_ODDS, gameinfo.TY1_STATUS, gameinfo.M_HANDI1, gameinfo.M_HANDI2, gameinfo.TY2_ODDS, gameinfo.TY2_STATUS, gameinfo.EU_W_ODDS, gameinfo.EU_L_ODDS, gameinfo.EU_D_ODDS from gameinfo, leaginfo where gameinfo.WEBACT='V' and gameinfo.ACT='U' and gameinfo.league = leaginfo.alias and leaginfo.leag_id in (select cleag_id from userprofile_info where iuser_id=");
			SQLString.Append(uid);
			SQLString.Append(") ");
			if(sSortType.Equals("0")) {		//order by league and then match date and time
				SQLString.Append("order by leaginfo.ALIAS, gameinfo.MATCHDATE, gameinfo.MATCHTIME, gameinfo.MATCH_ID");
			} else {		//order by order id
				SQLString.Append("order by gameinfo.MATCH_ID, gameinfo.MATCHDATE, gameinfo.MATCHTIME, leaginfo.ALIAS");
			}
			try {
				m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
				while(m_SportsOleReader.Read()) {
					//Match Date and Time (Hidden Field)
					HTMLString.Append("<tr align=\"center\"><td><input type=\"hidden\" name=\"MatchDate\" value=\"");
					HTMLString.Append(m_SportsOleReader.GetString(0));
					HTMLString.Append("\"><input type=\"hidden\" name=\"MatchTime\" value=\"");
					HTMLString.Append(m_SportsOleReader.GetString(1));
					HTMLString.Append("\">");

					//League and Match Count and Interval (Hidden Field)
					HTMLString.Append("<input type=\"hidden\" name=\"League\" value=\"");
					HTMLString.Append(m_SportsOleReader.GetString(2).Trim());
					HTMLString.Append("\"><input type=\"hidden\" name=\"MatchCount\" value=\"");
					HTMLString.Append(m_SportsOleReader.GetInt32(3).ToString());
					HTMLString.Append("\"><input type=\"hidden\" name=\"Interval\" value=\"");
					iInterval = m_SportsOleReader.GetInt32(4);
					HTMLString.Append(iInterval.ToString());
					HTMLString.Append("\">");

					//Order ID
					HTMLString.Append("<input type=\"text\" name=\"orderID\" maxlength=\"3\" size=\"1\" value=\"");
					if(!m_SportsOleReader.IsDBNull(5)) {
						if(!m_SportsOleReader.GetString(5).Trim().Equals("-1")) {
							if(!m_SportsOleReader.GetString(5).Trim().Equals("")) {
								HTMLString.Append(Convert.ToInt32(m_SportsOleReader.GetString(5).Trim()).ToString());
							}
						}
					}
					HTMLString.Append("\" onChange=\"onOrder_IDChanged(");
					HTMLString.Append(m_RecordCount.ToString());
					HTMLString.Append(")\" tabindex=\"");
					HTMLString.Append((m_RecordCount+1).ToString());
					HTMLString.Append("\"></td>");

					//Alias
					iLeagueGp = leagueList.IndexOf(m_SportsOleReader.GetString(6).Trim());
					HTMLString.Append("<td>");
					HTMLString.Append(m_SportsOleReader.GetString(6).Trim());
					HTMLString.Append("<input type=\"hidden\" name=\"Alias\" value=\"");
					HTMLString.Append(m_SportsOleReader.GetString(6).Trim());
					HTMLString.Append("\"></td>");

					//Host
					HTMLString.Append("<td>");
					HTMLString.Append(m_SportsOleReader.GetString(7).Trim());
					HTMLString.Append("<input type=\"hidden\" name=\"Host\" value=\"");
					HTMLString.Append(m_SportsOleReader.GetString(7).Trim());
					HTMLString.Append("\"></td>");

					//Guest
					HTMLString.Append("<td>");
					HTMLString.Append(m_SportsOleReader.GetString(8).Trim());
					HTMLString.Append("<input type=\"hidden\" name=\"Guest\" value=\"");
					HTMLString.Append(m_SportsOleReader.GetString(8).Trim());
					HTMLString.Append("\"></td>");

					//Match Field
					HTMLString.Append("<td><input type=\"checkbox\" name=\"MatchField\" ");
					if(m_SportsOleReader.GetString(9).Equals("M")) HTMLString.Append("checked");
					HTMLString.Append(" onClick=\"onChecked(");
					HTMLString.Append(m_RecordCount.ToString());
					HTMLString.Append(")\" value=\"");
					HTMLString.Append(m_RecordCount.ToString());
					HTMLString.Append("\"></td>");

					//Host Handicap
					HTMLString.Append("<td><input type=\"checkbox\" name=\"HostHandicap\" ");
					if(m_SportsOleReader.GetString(10).Equals("1")) HTMLString.Append("checked");
					HTMLString.Append(" onClick=\"onChecked(");
					HTMLString.Append(m_RecordCount.ToString());
					HTMLString.Append(")\" value=\"");
					HTMLString.Append(m_RecordCount.ToString());
					HTMLString.Append("\"></td>");

					//Up Score Handicap1/Handicap2
					HTMLString.Append("<td><input type=\"text\" name=\"UpHandicap1\" maxlength=\"3\" size=\"1\" value=\"");
					HTMLString.Append(m_SportsOleReader.GetString(11).Trim());
					HTMLString.Append("\" onChange=\"UpHandicap1Validity(");
					HTMLString.Append(m_RecordCount.ToString());
					HTMLString.Append(")\">/<input type=\"text\" name=\"UpHandicap2\" maxlength=\"3\" size=\"1\" value=\"");
					HTMLString.Append(m_SportsOleReader.GetString(12).Trim());
					HTMLString.Append("\" onChange=\"UpHandicap2Validity(");
					HTMLString.Append(m_RecordCount.ToString());
					HTMLString.Append(")\"></td>");

					//Up Odds
					HTMLString.Append("<td><input type=\"text\" name=\"UpOdds\" maxlength=\"5\" size=\"2\" value=\"");
					HTMLString.Append(m_SportsOleReader.GetString(13).Trim());
					HTMLString.Append("\" onChange=\"UpOddsValidity(");
					HTMLString.Append(m_RecordCount.ToString());
					HTMLString.Append(")\">");

					//Up Odds Status
					iArrayIndex = 0;
					iStatus = Convert.ToInt32(m_SportsOleReader.GetString(14)) - 1;
					HTMLString.Append("<select name=\"upStatus\" onChange=\"onStatusChanged(");
					HTMLString.Append(m_RecordCount.ToString());
					HTMLString.Append(")\"><option value=");
					HTMLString.Append(m_SportsOleReader.GetString(14));
					HTMLString.Append(">");
					HTMLString.Append(oddsArray[iStatus]);
					foreach(String sItem in oddsArray) {
						if(!sItem.Equals(oddsArray[iStatus]) && iArrayIndex < 4) {
							HTMLString.Append("<option value=");
							HTMLString.Append((iArrayIndex+1).ToString());
							HTMLString.Append(">");
							HTMLString.Append(sItem);
						}
						iArrayIndex++;
					}
					HTMLString.Append("</select></td>");

					//Big/Small - Total Score
					HTMLString.Append("<td><input type=\"text\" name=\"BSTTS\" maxlength=\"5\" size=\"2\" value=\"");
					if(!m_SportsOleReader.IsDBNull(15)) {
						if(!m_SportsOleReader.GetString(15).Trim().Equals("")) {
							HTMLString.Append(m_SportsOleReader.GetString(15).Trim());
						}
					}
					if(!m_SportsOleReader.IsDBNull(16)) {
						if(!m_SportsOleReader.GetString(16).Trim().Equals("")) {
							HTMLString.Append("/");
							HTMLString.Append(m_SportsOleReader.GetString(16).Trim());
						}
					}
					HTMLString.Append("\" onChange=\"BSTTSValidity(");
					HTMLString.Append(m_RecordCount.ToString());
					HTMLString.Append(")\"></td>");
/*
					HTMLString.Append("<td><input type=\"text\" name=\"DownHandicap1\" maxlength=\"3\" size=\"1\" value=\"");
					HTMLString.Append(m_SportsOleReader.GetString(15).Trim());
					HTMLString.Append("\" onChange=\"DownHandicap1Validity(");
					HTMLString.Append(m_RecordCount.ToString());
					HTMLString.Append(")\">/<input type=\"text\" name=\"DownHandicap2\" maxlength=\"3\" size=\"1\" value=\"");
					HTMLString.Append(m_SportsOleReader.GetString(16).Trim());
					HTMLString.Append("\" onChange=\"DownHandicap2Validity(");
					HTMLString.Append(m_RecordCount.ToString());
					HTMLString.Append(")\"></td>");
*/
					//Big/Small - Odds
					HTMLString.Append("<td><input type=\"text\" name=\"DownOdds\" maxlength=\"5\" size=\"2\" value=\"");
					HTMLString.Append(m_SportsOleReader.GetString(17).Trim());
					HTMLString.Append("\" onChange=\"DownOddsValidity(");
					HTMLString.Append(m_RecordCount.ToString());
					HTMLString.Append(")\">");

					//Big/Small - Odds Status
					iArrayIndex = 0;
					iStatus = Convert.ToInt32(m_SportsOleReader.GetString(18)) - 1;
					HTMLString.Append("<select name=\"downStatus\" onChange=\"onStatusChanged(");
					HTMLString.Append(m_RecordCount.ToString());
					HTMLString.Append(")\">");
					HTMLString.Append("<option value=");
					HTMLString.Append(m_SportsOleReader.GetString(18));
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
					HTMLString.Append(m_SportsOleReader.GetString(19).Trim());
					HTMLString.Append("\" onChange=\"EuroWinOddsValidity(");
					HTMLString.Append(m_RecordCount.ToString());
					HTMLString.Append(")\">/<input type=\"text\" name=\"EuroLossOdds\" maxlength=\"4\" size=\"1\" value=\"");
					HTMLString.Append(m_SportsOleReader.GetString(20).Trim());
					HTMLString.Append("\" onChange=\"EuroLossOddsValidity(");
					HTMLString.Append(m_RecordCount.ToString());
					HTMLString.Append(")\">&nbsp;<input type=\"text\" name=\"EuroDrawOdds\" maxlength=\"4\" size=\"1\" value=\"");
					HTMLString.Append(m_SportsOleReader.GetString(21).Trim());
					HTMLString.Append("\" onChange=\"EuroDrawOddsValidity(");
					HTMLString.Append(m_RecordCount.ToString());
					HTMLString.Append(")\"></td>");

					//Alert
					HTMLString.Append("<td><input type=\"checkbox\" name=\"alertChk\" onClick=\"onChecked(");
					HTMLString.Append(m_RecordCount.ToString());
					HTMLString.Append(")\" value=\"");
					HTMLString.Append(m_RecordCount.ToString());
					HTMLString.Append("\"></td>");

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
				//m_SportsDBMgr.Dispose();
				HTMLString.Append("<input type=\"hidden\" name=\"RecordCount\" value=\"" + m_RecordCount.ToString() + "\">");
			} catch(Exception ex) {
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " AllMatches.cs.GetAllMatches(): " + ex.ToString());
				m_SportsLog.Close();
				HTMLString.Remove(0,HTMLString.Length);
				HTMLString.Append((string)HttpContext.Current.Application["accessErrorMsg"]);
			}

			return HTMLString.ToString();
		}

		public int UpdateMatches(string sType) {
			int iUpdIndex = 0, iRecUpd = 0, iTempItem = 0;
			int iMustSendLen, iHiddenLen;
			string sOrder = "";
			string sTempItem;
			string sCurrentTimestamp = null;
			string sBatchJob = null;
			char[] delimiter = new char[] {','};
			string[] arrMustSend;
			string[] arrOrderID;
			string[] arrMatchCnt;
			string[] arrHidden;
			string[] arrInterval;
			string[] arrSendToPager;
			//Replicator MatchReplicator = new Replicator(HttpContext.Current.Application["RepDBConnectionString"].ToString());
			//MatchReplicator.ApplicationType = 1;
			//MatchReplicator.ErrorLogPath = HttpContext.Current.Application["ErrorFilePath"].ToString();

			//SportsMessage object message
			SportsMessage sptMsg = new SportsMessage();
			StringBuilder LogSQLString = new StringBuilder();
			DBManager logDBMgr = new DBManager();
			logDBMgr.ConnectionString = m_SportsDBMgr.ConnectionString;

			try {
				arrSendToPager = HttpContext.Current.Request.Form["SendToPager"].Split(delimiter);
			} catch(Exception) {
				arrSendToPager = new string[0];
			}

			try {
				arrOrderID = HttpContext.Current.Request.Form["orderID"].Split(delimiter);
				arrMatchCnt = HttpContext.Current.Request.Form["MatchCount"].Split(delimiter);
				arrInterval = HttpContext.Current.Request.Form["Interval"].Split(delimiter);
			} catch(Exception) {
				arrOrderID = new string[0];
				arrMatchCnt = new string[0];
				arrInterval = new string[0];
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
						int iLogUpd = 0;
						double dOriginalOdds = -1;
						double dUpdatedOdds = -1;
						string sUpHandicap1, sUpHandicap2, sUpOdds, sDownHandicap1, sDownHandicap2, sDownOdds, sEWOdds, sELOdds, sEDOdds, sHostHandicap, sMatchField;
						string[] arrMatchDate, arrMatchTime, arrMatchField, arrHostHandicap, arrLeague, arrAlias, arrHost, arrGuest, arrAlert, arrOddsStatus, arrMsgType;
						string[] arrUpHandicap1, arrUpHandicap2, arrUpOdds, arrUpStatus;
						string[] arrBSTTS, arrDownOdds, arrDownStatus, arrEWOdds, arrELOdds, arrEDOdds;

						//Delcare variable used in message notify
						string[] arrQueueNames;
						string[] arrMessageTypes;
						string[] arrRemotingPath;
						arrQueueNames = (string[])HttpContext.Current.Application["QueueItems"];
						arrMessageTypes = (string[])HttpContext.Current.Application["NotifyMessageTypes"];
						arrRemotingPath = (string[])HttpContext.Current.Application["RemotingItems"];
						MessageClient msgClt = new MessageClient();

						arrMatchDate = HttpContext.Current.Request.Form["MatchDate"].Split(delimiter);
						arrMatchTime = HttpContext.Current.Request.Form["MatchTime"].Split(delimiter);
						arrLeague = HttpContext.Current.Request.Form["League"].Split(delimiter);
						arrAlias = HttpContext.Current.Request.Form["Alias"].Split(delimiter);
						arrHost = HttpContext.Current.Request.Form["Host"].Split(delimiter);
						arrGuest = HttpContext.Current.Request.Form["Guest"].Split(delimiter);
						arrUpHandicap1 = HttpContext.Current.Request.Form["UpHandicap1"].Split(delimiter);
						arrUpHandicap2 = HttpContext.Current.Request.Form["UpHandicap2"].Split(delimiter);
						arrUpOdds = HttpContext.Current.Request.Form["UpOdds"].Split(delimiter);
						arrUpStatus = HttpContext.Current.Request.Form["upStatus"].Split(delimiter);
						arrBSTTS = HttpContext.Current.Request.Form["BSTTS"].Split(delimiter);
						arrDownOdds = HttpContext.Current.Request.Form["DownOdds"].Split(delimiter);
						arrDownStatus = HttpContext.Current.Request.Form["downStatus"].Split(delimiter);
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
						try {
							arrAlert = HttpContext.Current.Request.Form["alertChk"].Split(delimiter);
						}	catch(Exception) {
							arrAlert = new string[0];
						}
						arrMsgType = (string[])HttpContext.Current.Application["messageType"];
						arrOddsStatus = (string[])HttpContext.Current.Application["oddsItemsArray"];

						sCurrentTimestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
						sBatchJob = DateTime.Now.ToString("yyMMddHHmmss_fffffff") + "." + HttpContext.Current.Session["user_name"].ToString() + "." + arrMsgType[3] + ".ini";
						for(iRecUpd=0;iRecUpd<iMustSendLen;iRecUpd++) {
							//update the checked(MUSTSend) records only
							iUpdIndex = Convert.ToInt32(arrMustSend[iRecUpd]);

							sUpHandicap1 = arrUpHandicap1[iUpdIndex];
							if(sUpHandicap1 == null) sUpHandicap1 = "";
							else sUpHandicap1 = sUpHandicap1.Trim();

							sUpHandicap2 = arrUpHandicap2[iUpdIndex];
							if(sUpHandicap2 == null) sUpHandicap2 = "";
							else sUpHandicap2 = sUpHandicap2.Trim();

							sUpOdds = arrUpOdds[iUpdIndex];
							if(sUpOdds == null) sUpOdds = "";
							else sUpOdds = sUpOdds.Trim();

							sDownHandicap1 = arrBSTTS[iUpdIndex];
							if(sDownHandicap1 == null) sDownHandicap1 = "";
							else sDownHandicap1 = sDownHandicap1.Trim();
							if(sDownHandicap1.IndexOf("/") != -1) {
								sDownHandicap2 = sDownHandicap1.Substring(sDownHandicap1.IndexOf("/") + 1);
								sDownHandicap1 = sDownHandicap1.Substring(0, sDownHandicap1.IndexOf("/"));
							} else {
								sDownHandicap2 = "";
							}

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

							/*****************************
							 * GoGo Pager2 							 *
							 * Indicate change of odds   *
							 *****************************/
							SQLString.Remove(0,SQLString.Length);
							SQLString.Append("select TY1_ODDS from GAMEINFO where MATCH_CNT=" + arrMatchCnt[iUpdIndex]);
							m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
							if(m_SportsOleReader.Read()) {
								if(!m_SportsOleReader.IsDBNull(0)) {
									if(!m_SportsOleReader.GetString(0).Trim().Equals("")) {
										dOriginalOdds = Convert.ToDouble(m_SportsOleReader.GetString(0).Trim());
									}
								}
							}
							m_SportsOleReader.Close();
							m_SportsDBMgr.Close();

							//update gameinfo
							SQLString.Remove(0,SQLString.Length);
							SQLString.Append("update GAMEINFO set FIELD='" + sMatchField + "', HOST_HANDI='" + sHostHandicap + "', T_HANDI1='" + sUpHandicap1 + "', T_HANDI2='" + sUpHandicap2 + "', M_HANDI1='" + sDownHandicap1 + "', M_HANDI2='" + sDownHandicap2 + "', ");
							SQLString.Append("TY1_ODDS='" + sUpOdds + "', TY1_STATUS='" + arrUpStatus[iUpdIndex] + "', TY2_ODDS='" + sDownOdds + "', TY2_STATUS='" + arrDownStatus[iUpdIndex] + "', EU_W_ODDS='" + sEWOdds + "', EU_L_ODDS='" + sELOdds + "', EU_D_ODDS='" + sEDOdds + "' where MATCH_CNT=" + arrMatchCnt[iUpdIndex]);
							m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
							m_SportsDBMgr.Close();
							//MatchReplicator.Replicate(SQLString.ToString());

/*
							//Mango message format: AppID MsgID Priority AlertID MsgLen Alias Host Guest OddsType HostHandicap FreeText
							sMangoHandicap = "";
							sptMsg.AppID = "01";
							sptMsg.MsgID = "03";
							if(!sUpHandicap1.Equals("")) {
								if(!sUpHandicap2.Equals("")) sMangoHandicap = sUpHandicap1 + "/" + sUpHandicap2;
								else sMangoHandicap = sUpHandicap1;
								sMangoHandicap = m_SportsDBMgr.ExecuteQueryString("select CMCHANDICAP from MCHANDICAP_MAP where CHANDICAP='" + sMangoHandicap + "'");
							}
							sMsgBody = PadRightSpace(arrAlias[iUpdIndex],FIELDLENGTH) + PadRightSpace(arrHost[iUpdIndex],FIELDLENGTH) + PadRightSpace(arrGuest[iUpdIndex],FIELDLENGTH) + sHostHandicap + arrUpStatus[iUpdIndex];
							if(!sMangoHandicap.Equals("") && !sMangoHandicap.Equals("-1")) sMsgBody += sMangoHandicap;
							if(!sUpOdds.Equals("") && !sUpOdds.Equals("-1")) sMsgBody += " " + sUpOdds;
							arrByteOfMSMQBody = m_Big5Encoded.GetBytes(sMsgBody);
							iMsgBodyLength = arrByteOfMSMQBody.Length;
							sptMsg.Body = iMsgBodyLength.ToString("D3") + sMsgBody;
							sptMsg.Timestamp = DateTime.Now.ToString("yyMMddHHmmss");
							sptMsg.Priority = "2";
							sptMsg.AlertID = "1";
							//Send Mango Message
							try {
								msgClt.SendMessage(sptMsg);
							} catch(Exception ex) {
								m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
								m_SportsLog.SetFileName(0,LOGFILESUFFIX);
								m_SportsLog.Open();
								m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " AllMatches.cs.UpdateMatches(): Send mango message throws exception: " + ex.ToString());
								m_SportsLog.Close();
							}
*/

							//generate AllOdds INI file if required
							if(arrSendToPager.Length>0 && iMustSendLen>0) {
								iLogUpd++;
								LogSQLString.Remove(0,LogSQLString.Length);
								LogSQLString.Append("insert into LOG_ALLODDS (TIMEFLAG, SECTION, LEAGUEALIAS, LEAGUE, MATCHDATE, MATCHTIME, MATCHFIELD, HOST, GUEST, ACT, HANDICAP, T_HANDIOWNER, M_HANDIOWNER, T_HANDI, M_HANDI, T_LIVEODDS, M_LIVEODDS, T_ODDS, M_ODDS, E_ODDS1, E_ODDS2, ALERT, INTERVAL, ODDS_TREND, BATCHJOB) values ('");
								LogSQLString.Append(sCurrentTimestamp);
								LogSQLString.Append("','ODD_','");
								LogSQLString.Append(arrAlias[iUpdIndex]);
								LogSQLString.Append("','");
								LogSQLString.Append(arrLeague[iUpdIndex]);
								LogSQLString.Append("','");
								LogSQLString.Append(arrMatchDate[iUpdIndex]);
								LogSQLString.Append("','");
								LogSQLString.Append(arrMatchTime[iUpdIndex]);
								LogSQLString.Append("','");
								LogSQLString.Append(sMatchField);
								LogSQLString.Append("','");
								LogSQLString.Append(arrHost[iUpdIndex]);
								LogSQLString.Append("','");
								LogSQLString.Append(arrGuest[iUpdIndex]);
								LogSQLString.Append("','U','");
								LogSQLString.Append(sHostHandicap);
								LogSQLString.Append("','泰','馬','");
								if(sUpHandicap1.Equals("")) {
									sUpHandicap1 = "-1";
								}	else {
									if(!sUpHandicap2.Equals("")) {
										sUpHandicap1 += "/" + sUpHandicap2;
									}
								}
								LogSQLString.Append(sUpHandicap1);
								LogSQLString.Append("','");
								if(sDownHandicap1.Equals("")) {
									sDownHandicap1 = "-1";
								}	else {
									if(!sDownHandicap2.Equals("")) {
										sDownHandicap1 += "/" + sDownHandicap2;
									}
								}
								LogSQLString.Append(sDownHandicap1);
								LogSQLString.Append("','");
								LogSQLString.Append(arrOddsStatus[Convert.ToInt32(arrUpStatus[iUpdIndex])-1]);
								LogSQLString.Append("','");
								LogSQLString.Append(arrOddsStatus[Convert.ToInt32(arrDownStatus[iUpdIndex])-1]);
								LogSQLString.Append("','");
								if(sUpOdds.Equals("")) {
									sUpOdds = "-1";
								}
								LogSQLString.Append(sUpOdds);
								LogSQLString.Append("','");
								if(sDownOdds.Equals("")) {
									sDownOdds = "-1";
								}
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
								LogSQLString.Append("',");
								sTempItem = "";
								for(int iAlert=0;iAlert<arrAlert.Length;iAlert++) {
									if(arrAlert[iAlert] == iUpdIndex.ToString()) {
										sTempItem = "1";
										break;
									}
								}
								if(sTempItem.Equals("1")) LogSQLString.Append("1");
								else LogSQLString.Append("0");
								LogSQLString.Append(",");
								LogSQLString.Append(arrInterval[iUpdIndex]);
								LogSQLString.Append(",'");
								if(!sUpOdds.Trim().Equals("") && !sUpOdds.Trim().Equals("-1")) dUpdatedOdds = Convert.ToDouble(sUpOdds);
								if(dOriginalOdds != -1 && dUpdatedOdds != -1) {
									if(dUpdatedOdds > dOriginalOdds) LogSQLString.Append("UP");
									else if(dUpdatedOdds < dOriginalOdds) LogSQLString.Append("DN");
									else LogSQLString.Append("EQ");
								} else LogSQLString.Append("EQ");
								LogSQLString.Append("','");
								LogSQLString.Append(sBatchJob);
								LogSQLString.Append("')");
								logDBMgr.ExecuteNonQuery(LogSQLString.ToString());
								logDBMgr.Close();
								dOriginalOdds = -1;
								dUpdatedOdds = -1;
							}	//insert into LOG_ALLODDS
						}

						if(iLogUpd>0) {
							//Tell MessageDispatcher to generate Asia Odds INI and notify other processes
							//modified by Henry, 09 Feb 2004
							sptMsg.IsTransaction = true;
							sptMsg.Body = sBatchJob;
							sptMsg.Timestamp = DateTime.Now.ToString("yyMMddHHmmss");
							sptMsg.AppID = "07";
							sptMsg.MsgID = "03";
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
									m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " MSMQ ERROR: Asia Odds");
									m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " AllMatches.cs.UpdateMatches(): Notify via MSMQ throws MessageQueueException:  " + mqEx.ToString());
									m_SportsLog.Close();

									//If MSMQ fail, notify via .NET Remoting
									msgClt.MessageType = arrMessageTypes[1];
									msgClt.MessagePath = arrRemotingPath[0];
									if(!msgClt.SendMessage((object)sptMsg)) {
										m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
										m_SportsLog.SetFileName(0,LOGFILESUFFIX);
										m_SportsLog.Open();
										m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Remoting ERROR: Asia Odds");
										m_SportsLog.Close();
									}
								}	catch(Exception ex) {
									m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
									m_SportsLog.SetFileName(0,LOGFILESUFFIX);
									m_SportsLog.Open();
									m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Remoting ERROR: Asia Odds");
									m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " AllMatches.cs.UpdateMatches(): Notify via .NET Remoting throws exception: " + ex.ToString());
									m_SportsLog.Close();
								}
							}	catch(Exception ex) {
								m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
								m_SportsLog.SetFileName(0,LOGFILESUFFIX);
								m_SportsLog.Open();
								m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " AllMatches.cs.UpdateMatches(): Notify via MSMQ throws exception: " + ex.ToString());
								m_SportsLog.Close();
							}
						}
/*
						//modified end
						if(iRecUpd > 0) {
							MatchReplicator.Dispose();
							m_SportsDBMgr.Dispose();
						}
*/
/*
						//write log
						m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
						m_SportsLog.SetFileName(0,LOGFILESUFFIX);
						m_SportsLog.Open();
						m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " AllMatches.cs: Send " + iRecUpd.ToString() + " matches (" + HttpContext.Current.Session["user_name"] + ")");
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
							SQLString.Append("update GAMEINFO set MATCH_ID='" + sOrder + "' where MATCH_CNT=" + arrMatchCnt[iRecUpd].Trim());
							m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
							m_SportsDBMgr.Close();
							SQLString.Remove(0,SQLString.Length);
							SQLString.Append("select count(*) from GOALINFO where MATCH_CNT=");
							SQLString.Append(arrMatchCnt[iRecUpd].Trim());
							if (m_SportsDBMgr.ExecuteScalar(SQLString.ToString()) > 0) {
								SQLString.Remove(0,SQLString.Length);
								SQLString.Append("update GOALINFO set ORDER_ID='" + sOrder + "' where MATCH_CNT=" + arrMatchCnt[iRecUpd].Trim());
								m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
							}
							m_SportsDBMgr.Close();
							//MatchReplicator.Replicate(SQLString.ToString());
						}
/*
						if(iRecUpd > 0) {
							MatchReplicator.Dispose();
							m_SportsDBMgr.Dispose();
						}
*/
						//write log
						m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
						m_SportsLog.SetFileName(0,LOGFILESUFFIX);
						m_SportsLog.Open();
						m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " AllMatches.cs: Sort " + iRecUpd.ToString() + " matches (" + HttpContext.Current.Session["user_name"] + ")");
						m_SportsLog.Close();
						break;
					case "HIDE":	//case: Hidden specific match
						for(iRecUpd=0;iRecUpd<iHiddenLen;iRecUpd++) {
							iUpdIndex = Convert.ToInt32(arrHidden[iRecUpd]);
							sTempItem = arrMatchCnt[iUpdIndex].Trim();
							if(iRecUpd == 0) {
								SQLString.Remove(0,SQLString.Length);
								SQLString.Append("update GAMEINFO set WEBACT='X' where MATCH_CNT=" + sTempItem);
							}	else {
								SQLString.Append(" or MATCH_CNT=" + sTempItem);
							}
							m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
							m_SportsDBMgr.Close();
							//MatchReplicator.Replicate(SQLString.ToString());
						}
/*
						if(iRecUpd > 0) {
							MatchReplicator.Dispose();
							m_SportsDBMgr.Dispose();
						}
*/
						//write log
						m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
						m_SportsLog.SetFileName(0,LOGFILESUFFIX);
						m_SportsLog.Open();
						m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " AllMatches.cs: Hide " + iRecUpd.ToString() + " matches (" + HttpContext.Current.Session["user_name"] + ")");
						m_SportsLog.Close();
						break;
					case "SHOW":	//case: Set all matches to visible
						SQLString.Remove(0,SQLString.Length);
						SQLString.Append("update GAMEINFO set WEBACT='V'");
						iRecUpd = m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
						m_SportsDBMgr.Close();
						//MatchReplicator.Replicate(SQLString.ToString());
						//MatchReplicator.Dispose();
						//m_SportsDBMgr.Dispose();

						//write log
						m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
						m_SportsLog.SetFileName(0,LOGFILESUFFIX);
						m_SportsLog.Open();
						m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " AllMatches.cs: " + HttpContext.Current.Session["user_name"] + " set " + iRecUpd.ToString() + " matches to visible");
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
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " AllMatches.cs.UpdateMatches(): " + ex.ToString());
				m_SportsLog.Close();
			}

			return iRecUpd;
		}
/*
		private string PadRightSpace(string sItem, int iSpaceCount) {
			int iLen;
			byte[] arrItemByte;
			StringBuilder sbRefined;
			if(sItem != null) {
				sbRefined = new StringBuilder(sItem);
				arrItemByte = m_Big5Encoded.GetBytes(sItem);
				iLen = arrItemByte.Length;
				sbRefined.Append(' ',iSpaceCount-iLen);
			} else {
				sbRefined = new StringBuilder(new String(' ',iSpaceCount));
			}

			return sbRefined.ToString();
		}
*/
	}
}
