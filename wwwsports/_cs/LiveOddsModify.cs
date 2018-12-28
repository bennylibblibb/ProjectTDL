/*
Objective:
Update live odds data

Last updated:
16 Feb 2007, (Victor Cheng) Change 球衣 to another Asian Odds
15 Nov 2004, (Fanny Cheung) Increment orderId into 3 digits
19 Feb 2004, Chapman Choi
Additional field MATCHSTATUS

C#.NET complier statement:
csc /t:library /out:..\bin\LiveOddsModify.dll /r:..\bin\DBManager.dll;..\bin\Files.dll;..\bin\MessageClient.dll;..\bin\SportsMessage.dll LiveOddsModify.cs
*/

using System;
using System.Data.OleDb;
using System.Reflection;
using System.Text;
using System.Web;
using TDL.DB;
using TDL.IO;
using TDL.Message;

[assembly:AssemblyCompany("TDSL")]
[assembly:AssemblyCopyright("(c) 2003 TDSL. All rights reserved. Created on 14 Oct 2003.")]
[assembly:AssemblyDescription("現場賠率 -> 賠率更新")]
[assembly:AssemblyProduct("Sports.NET")]
[assembly:AssemblyTitle("Sports.NET DLL")]
[assembly:AssemblyVersion("3.1.*")]
namespace SportsUtil {
	public class LiveOddsModify {
		const string LOGFILESUFFIX = "log";
		int m_RecordCount = 0;
		string m_Region;
		OleDbDataReader m_SportsOleReader;
		DBManager m_SportsDBMgr;
		Files m_SportsLog;
		Encoding m_Big5Encoded;
		StringBuilder SQLString;

		public LiveOddsModify(string Connection) {
			m_SportsDBMgr = new DBManager();
			m_SportsDBMgr.ConnectionString = Connection;
			m_SportsLog = new Files();
			m_Big5Encoded = Encoding.GetEncoding(950);
			SQLString = new StringBuilder();
			m_Region = null;
		}

		public int NumberOfRecords {
			get {
				return m_RecordCount;
			}
		}

		public string CurrentTime {
			get {
				return DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
			}
		}

		public string RegionName {
			get {
				return m_Region;
			}
		}

		public string GetLiveOdds() {
			int iNoOfOpt;
			int iMatchStatus = 0;
			string strOstatus = "";
			string uid;
			string sMatchDate = "";
			string sMatchTime = "";
			string sRegionID;
			string sScoreHandi = "";
			string[] arroddsItem;
			string[] arrScoreHandi;
			string[] arrBigSmallScore;
			string[] arrMatchStatus;
			StringBuilder HTMLString = new StringBuilder();

			sRegionID = HttpContext.Current.Request.QueryString["RegionID"];
			uid = HttpContext.Current.Session["user_id"].ToString();
			arroddsItem = (string[])HttpContext.Current.Application["oddsItemsArray"];
			arrScoreHandi = (string[])HttpContext.Current.Application["SOCScoreHandiArray"];
			arrBigSmallScore = (string[])HttpContext.Current.Application["SOCScoreHandiArray"];
			arrMatchStatus = (string[])HttpContext.Current.Application["lvoddsMatchItemsArray"];

			try {
				SQLString.Remove(0,SQLString.Length);
				SQLString.Append("select CREGION from LIVEODDS_CFG where IREGION_ID=");
				SQLString.Append(sRegionID);
				m_Region = m_SportsDBMgr.ExecuteQueryString(SQLString.ToString());
				m_SportsDBMgr.Close();

				SQLString.Remove(0,SQLString.Length);
				SQLString.Append("select lvodds.IMATCHORDER, lvodds.CHOST, lvodds.IH_SCORE, lvodds.CH_WEAR, lvodds.CGUEST, lvodds.IG_SCORE, lvodds.CG_WEAR, lvodds.CHOST_HANDI, lvodds.CMATCHDATE, lvodds.CMATCHTIME, lvodds.CMATCHSTATUS, lvodds.ITIMEOFGAME, lvodds.CHANDICAP1, lvodds.CHANDICAP2, lvodds.CHODDS, lvodds.CTOTALSCORE, lvodds.CBIGODDS, lvodds.CSMALLODDS, lvodds.CODDSSTATUS, lvodds.CMATCHFIELD, lvodds.IMATCH_CNT, lvodds.CALIAS, lvodds.CLEAGUE from LIVEODDS_INFO lvodds, leaginfo leag where lvodds.IREGION_ID=");
				SQLString.Append(sRegionID);
				SQLString.Append(" and lvodds.CACT='U' and lvodds.CALIAS = leag.alias and leag.leag_id in (select cleag_id from userprofile_info where iuser_id=");
				SQLString.Append(uid);
				SQLString.Append(") order by lvodds.IMATCHORDER, leag.leag_order");
				m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
				while(m_SportsOleReader.Read()) {
					//IMATCHORDER
					HTMLString.Append("<tr align=\"center\"><td><input type=\"text\" name=\"orderID\" maxlength=\"3\" size=\"1\" value=\"");
					if(!m_SportsOleReader.IsDBNull(0)) {
						HTMLString.Append(m_SportsOleReader.GetInt32(0).ToString());
					}
					HTMLString.Append("\" onChange=\"orderID_Changed(");
					HTMLString.Append(m_RecordCount.ToString());
					HTMLString.Append(")\" tabindex=\"");
					HTMLString.Append((m_RecordCount+1).ToString());
					HTMLString.Append("\"></td>");

					//Host / Host Score <br> Host Wear
					HTMLString.Append("<td><input type=\"hidden\" name=\"hostname\" value=\"");
					HTMLString.Append(m_SportsOleReader.GetString(1).Trim());
					HTMLString.Append("\"><font color=\"#8B0000\">");
					HTMLString.Append(m_SportsOleReader.GetString(1).Trim());
					HTMLString.Append("</font>&nbsp;/&nbsp;<input name=\"hostscore\" size=\"1\" maxlength=\"2\" value=\"");
					if(!m_SportsOleReader.IsDBNull(2)) {
						HTMLString.Append(m_SportsOleReader.GetInt32(2).ToString());
					}
					HTMLString.Append("\" onChange=\"onScrChange(");
					HTMLString.Append(m_RecordCount.ToString());
					HTMLString.Append(")\"><br><i>球衣</i><input name=\"hostwear\" size=\"3\" maxlength=\"10\" value=\"");
					if(!m_SportsOleReader.IsDBNull(3)) {
						HTMLString.Append(m_SportsOleReader.GetString(3).Trim());
					}
					HTMLString.Append("\" onChange=\"onChangeSent(");
					HTMLString.Append(m_RecordCount.ToString());
					HTMLString.Append(")\"></td>");

					//Guest / Guest Score <br> Guest Wear
					HTMLString.Append("<td><input type=\"hidden\" name=\"guestname\" value=\"");
					HTMLString.Append(m_SportsOleReader.GetString(4).Trim());
					HTMLString.Append("\"><font color=\"#20B2AA\">");
					HTMLString.Append(m_SportsOleReader.GetString(4).Trim());
					HTMLString.Append("</font>&nbsp;/&nbsp;<input name=\"guestscore\" size=\"1\" maxlength=\"2\" value=\"");
					if(!m_SportsOleReader.IsDBNull(5)) {
						HTMLString.Append(m_SportsOleReader.GetInt32(5).ToString());
					}
					HTMLString.Append("\" onChange=\"onScrChange(");
					HTMLString.Append(m_RecordCount.ToString());
					HTMLString.Append(")\"><br><i>球衣</i><input name=\"guestwear\" size=\"3\" maxlength=\"10\" value=\"");
					if(!m_SportsOleReader.IsDBNull(6)) {
						HTMLString.Append(m_SportsOleReader.GetString(6).Trim());
					}
					HTMLString.Append("\" onChange=\"onChangeSent(");
					HTMLString.Append(m_RecordCount.ToString());
					HTMLString.Append(")\"></td>");

					//Host Handicap Flag
					HTMLString.Append("<td><input type=\"checkbox\" name=\"HostHandicap\" value=\"");
					HTMLString.Append(m_RecordCount.ToString());
					HTMLString.Append("\" onClick=\"onChangeSent(");
					HTMLString.Append(m_RecordCount.ToString());
					HTMLString.Append(")\" ");
					if(!m_SportsOleReader.IsDBNull(7)) {
						if(m_SportsOleReader.GetString(7).Equals("1")) HTMLString.Append("checked");
					}
					HTMLString.Append("></td>");

					//Match Date <br> Time
					HTMLString.Append("<td><input name=\"matchdate\" maxlength=\"8\" size=\"5\" value=\"");
					if(!m_SportsOleReader.IsDBNull(8)) {
						HTMLString.Append(m_SportsOleReader.GetString(8).Trim());
						sMatchDate = m_SportsOleReader.GetString(8).Trim();
					}
					HTMLString.Append("\" onChange=\"onChangeDate(");
					HTMLString.Append(m_RecordCount.ToString());
					HTMLString.Append(",");
					HTMLString.Append(DateTime.Now.ToString("yyyyMMdd"));
					HTMLString.Append(")\"><br><input name=\"matchtime\" maxlength=\"4\" size=\"1\" value=\"");
					if(!m_SportsOleReader.IsDBNull(9)) {
						HTMLString.Append(m_SportsOleReader.GetString(9).Trim());
						sMatchTime = m_SportsOleReader.GetString(9).Trim();
					}
					HTMLString.Append("\" onChange=\"onChangeTime(");
					HTMLString.Append(m_RecordCount.ToString());
					HTMLString.Append(")\"></td>");

					//時段 mstatus
					HTMLString.Append("<td><select name=\"MatchStatus\" onChange=\"onChangeSent(");
					HTMLString.Append(m_RecordCount.ToString());
					HTMLString.Append(")\">");
					if(m_SportsOleReader.IsDBNull(10)) {
						for(iNoOfOpt=0;iNoOfOpt<arrMatchStatus.Length;iNoOfOpt++) {
							HTMLString.Append("<option value=\"");
							HTMLString.Append(iNoOfOpt.ToString());
							HTMLString.Append("\">");
							HTMLString.Append(arrMatchStatus[iNoOfOpt]);
						}
					}	else {
						if(!m_SportsOleReader.GetString(10).Trim().Equals("")) {
							iMatchStatus = Convert.ToInt32(m_SportsOleReader.GetString(10).Trim());
							HTMLString.Append("<option value=\"");
							HTMLString.Append(iMatchStatus.ToString());
							HTMLString.Append("\">");
							HTMLString.Append(arrMatchStatus[iMatchStatus]);
							for(iNoOfOpt=0;iNoOfOpt<arrMatchStatus.Length;iNoOfOpt++) {
								if(iNoOfOpt != iMatchStatus) {
									HTMLString.Append("<option value=\"");
									HTMLString.Append(iNoOfOpt.ToString());
									HTMLString.Append("\">");
									HTMLString.Append(arrMatchStatus[iNoOfOpt]);
								}
							}
						} else {
							for(iNoOfOpt=0;iNoOfOpt<arrMatchStatus.Length;iNoOfOpt++) {
								HTMLString.Append("<option value=\"");
								HTMLString.Append(iNoOfOpt.ToString());
								HTMLString.Append("\">");
								HTMLString.Append(arrMatchStatus[iNoOfOpt]);
							}
						}
					}
					HTMLString.Append("</select></td>");

					//Time of game
					sMatchDate = sMatchDate.Insert(4,"/");
					sMatchDate = sMatchDate.Insert(7,"/");
					sMatchTime = sMatchTime.Insert(2,":");
					if(m_SportsOleReader.IsDBNull(11)) {
						HTMLString.Append("<td>0 分</td>");
					}	else {
						if(iMatchStatus != 2)	{
							if(!sMatchTime.Trim().Equals("")) {
								HTMLString.Append("<td>");
								HTMLString.Append(TimeGoneInMinute(DateTime.Parse(sMatchDate + " " + sMatchTime)));
								HTMLString.Append(" 分</td>");
							} else {
								HTMLString.Append("<td>");
								HTMLString.Append(m_SportsOleReader.GetInt32(11).ToString());
								HTMLString.Append(" 分</td>");
							}
						}
						else HTMLString.Append("<td>- 分</td>");
					}

					//Asia Odds - Handicap1 / Handicap2
					if((!m_SportsOleReader.IsDBNull(12))&&(!m_SportsOleReader.IsDBNull(13))){
						if(m_SportsOleReader.GetString(13).Trim().Equals(""))
							sScoreHandi = m_SportsOleReader.GetString(12).Trim();
						else
							sScoreHandi = m_SportsOleReader.GetString(12).Trim() +"/"+ m_SportsOleReader.GetString(13).Trim();
						HTMLString.Append("\n<td><select name=\"ScoreHandicap\" onChange=\"onChangeSent(");
						HTMLString.Append(m_RecordCount.ToString());
						HTMLString.Append(")\">");
						for(iNoOfOpt=0 ;iNoOfOpt<arrScoreHandi.Length ;iNoOfOpt++ ) {
								HTMLString.Append("<option value=\"");
								HTMLString.Append(arrScoreHandi[iNoOfOpt].ToString());
								if(sScoreHandi == arrScoreHandi[iNoOfOpt].Trim())
									HTMLString.Append("\" selected>");
								else
									HTMLString.Append("\">");
								HTMLString.Append(arrScoreHandi[iNoOfOpt]);
							}
					} else {
						HTMLString.Append("\n<td><select name=\"ScoreHandicap\" onChange=\"onChangeSent(");
						HTMLString.Append(m_RecordCount.ToString());
						HTMLString.Append(")\">");
						for(iNoOfOpt=0 ;iNoOfOpt<arrScoreHandi.Length ;iNoOfOpt++ ) {
								HTMLString.Append("<option value=\"");
								HTMLString.Append(arrScoreHandi[iNoOfOpt].ToString());
								HTMLString.Append("\">");
								HTMLString.Append(arrScoreHandi[iNoOfOpt]);
						}
					}
					HTMLString.Append("</select></td>\n");

					//Asia odds - HODDS
					HTMLString.Append("<td><input name=\"odds\" maxlength=\"5\" size=\"2\" value=\"");
					if(!m_SportsOleReader.IsDBNull(14)) {
						HTMLString.Append(m_SportsOleReader.GetString(14).Trim());
					}
					HTMLString.Append("\" onChange=\"onChangeOdds(");
					HTMLString.Append(m_RecordCount.ToString());
					HTMLString.Append(")\"></td>");

					//BigSmall Odds - TotalScore
					HTMLString.Append("<td><select name=\"totalScore\" onChange=\"onChangeSent(");
					HTMLString.Append(m_RecordCount.ToString());
					HTMLString.Append(")\">");
					if(m_SportsOleReader.IsDBNull(15)) {
						for(iNoOfOpt = 0;iNoOfOpt<arrBigSmallScore.Length ;iNoOfOpt++ ) {
							HTMLString.Append("<option value=\"");
							HTMLString.Append(arrBigSmallScore[iNoOfOpt].ToString());
							HTMLString.Append("\">");
							HTMLString.Append(arrBigSmallScore[iNoOfOpt]);
						}
					} else {
						for(iNoOfOpt = 0;iNoOfOpt<arrBigSmallScore.Length ;iNoOfOpt++ ) {
							HTMLString.Append("<option value=\"");
							HTMLString.Append(arrBigSmallScore[iNoOfOpt].ToString());
							if(m_SportsOleReader.GetString(15).Trim()==arrBigSmallScore[iNoOfOpt].ToString().Trim())
								HTMLString.Append("\" selected>");
							else
							    HTMLString.Append("\">");
							HTMLString.Append(arrBigSmallScore[iNoOfOpt]);
						}
					}
					HTMLString.Append("</select></td>");

					//BigSmall Odds - BigOdds / SmallOdds
					HTMLString.Append("<td><input name=\"bigodds\" maxlength=\"5\" size=\"2\" value=\"");
					if(!m_SportsOleReader.IsDBNull(16)) {
						HTMLString.Append(m_SportsOleReader.GetString(16).Trim());
					}
					HTMLString.Append("\" onChange=\"onChangeBigOdds(");
					HTMLString.Append(m_RecordCount.ToString());
					HTMLString.Append(")\">");
					HTMLString.Append("</td>");
					//Odds Status
					HTMLString.Append("<td><select name=\"OddsStatus\" onChange=\"onChangeSent(");
					HTMLString.Append(m_RecordCount.ToString());
					HTMLString.Append(")\">");
					if(m_SportsOleReader.IsDBNull(18)) {
						for(iNoOfOpt=0;iNoOfOpt<arroddsItem.Length;iNoOfOpt++) {
							if (iNoOfOpt != 3) {
								HTMLString.Append("<option value=\"");
								HTMLString.Append(iNoOfOpt.ToString());
								HTMLString.Append("\">");
								HTMLString.Append(arroddsItem[iNoOfOpt]);
							}
						}
					}	else {
						if(!m_SportsOleReader.GetString(18).Trim().Equals("")) {
							strOstatus = m_SportsOleReader.GetString(18).Trim();
							iNoOfOpt = Convert.ToInt32(strOstatus);
							if (iNoOfOpt > 2) {
								iNoOfOpt++;
								strOstatus = iNoOfOpt.ToString();
								HTMLString.Append("<option value=\"");
								HTMLString.Append(strOstatus);
								HTMLString.Append("\" selected>");
								HTMLString.Append(arroddsItem[Convert.ToInt32(strOstatus)]);
							} else {
								HTMLString.Append("<option value=\"");
								HTMLString.Append(strOstatus);
								HTMLString.Append("\" selected>");
								HTMLString.Append(arroddsItem[Convert.ToInt32(strOstatus)]);
							}
							for(iNoOfOpt=0;iNoOfOpt<arroddsItem.Length;iNoOfOpt++) {
								if(!iNoOfOpt.ToString().Equals(strOstatus))	{
									if (iNoOfOpt != 3) {
										HTMLString.Append("<option value=\"");
										HTMLString.Append(iNoOfOpt.ToString());
										HTMLString.Append("\">");
										HTMLString.Append(arroddsItem[iNoOfOpt]);
									}
								}
							}
						} else {
							for(iNoOfOpt=0;iNoOfOpt<arroddsItem.Length;iNoOfOpt++) {
								if (iNoOfOpt != 3) {
									HTMLString.Append("<option value=\"");
									HTMLString.Append(iNoOfOpt.ToString());
									HTMLString.Append("\">");
									HTMLString.Append(arroddsItem[iNoOfOpt]);
								}
							}
						}
					}
					HTMLString.Append("</select></td>");

					//Alert Flag
					HTMLString.Append("<td><input type=\"checkbox\" name=\"Alert\" value=\"");
					HTMLString.Append(m_RecordCount.ToString());
					HTMLString.Append("\" onClick=\"onChangeSent(");
					HTMLString.Append(m_RecordCount.ToString());
					HTMLString.Append(")\"></td>");

					//Match Field
					HTMLString.Append("<input type=\"hidden\" name=\"MatchField\" value=\"");
					if(!m_SportsOleReader.IsDBNull(19)) {
						HTMLString.Append(m_SportsOleReader.GetString(19).Trim());
					}
					HTMLString.Append("\">");

					//Fore to Send Flag
					HTMLString.Append("<td><input type=\"checkbox\" name=\"mustsend\" value=\"");
					HTMLString.Append(m_RecordCount.ToString());
					HTMLString.Append("\"><input type=\"hidden\" name=\"match_cnt\" value=\"");
					HTMLString.Append(m_SportsOleReader.GetInt32(20).ToString());
					HTMLString.Append("\"><input type=\"hidden\" name=\"Alias\" value=\"");
					HTMLString.Append(m_SportsOleReader.GetString(21).Trim());
					HTMLString.Append("\"><input type=\"hidden\" name=\"League\" value=\"");
					HTMLString.Append(m_SportsOleReader.GetString(22).Trim());
					HTMLString.Append("\"></td></tr>");
					m_RecordCount++;
				}	//end while
				m_SportsOleReader.Close();
				m_SportsDBMgr.Close();
				m_SportsDBMgr.Dispose();
				HTMLString.Append("<input type=\"hidden\" name=\"RegionID\" value=\"");
				HTMLString.Append(sRegionID);
				HTMLString.Append("\">");
			}	catch(Exception ex) {
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " LiveOddsModify.cs.GetLiveOdds(): " + ex.ToString());
				m_SportsLog.Close();
				HTMLString.Remove(0,HTMLString.Length);
				HTMLString.Append((string)HttpContext.Current.Application["accessErrorMsg"]);
			}

			return HTMLString.ToString();
		}

		private int TimeGoneInMinute(DateTime dtMatch) {
			int iTimeGone = 0;

			try {
				iTimeGone = (int)(DateTime.Now.Subtract(dtMatch)).TotalMinutes;
				if((iTimeGone < 0) || (iTimeGone > 55)) {
					iTimeGone = 0;
				}
			} catch(Exception) {
				throw (new Exception("LiveOddsModify.cs.TimeGoneInMinute() exception"));
			}
			return iTimeGone;
		}

		public int RenewMatchTimeGone() {
			int iMinute = 0;
			int iRenewStatus = 0;
			string sRegionID;
			string sMatchDate;
			string sMatchTime;
			char[] delimiter = new char[] {','};
			string[] arrMatchCnt;
			string[] arrMatchDate;
			string[] arrMatchTime;

			sRegionID = HttpContext.Current.Request.Form["RegionID"];
			arrMatchCnt = HttpContext.Current.Request.Form["match_cnt"].Split(delimiter);
			arrMatchDate = HttpContext.Current.Request.Form["matchdate"].Split(delimiter);
			arrMatchTime = HttpContext.Current.Request.Form["matchtime"].Split(delimiter);
			try {
				for(iRenewStatus = 0; iRenewStatus < arrMatchCnt.Length; iRenewStatus++) {
					sMatchDate = arrMatchDate[iRenewStatus].Trim();
					sMatchDate = sMatchDate.Insert(4,"/");
					sMatchDate = sMatchDate.Insert(7,"/");
					sMatchTime = arrMatchTime[iRenewStatus].Trim();
					sMatchTime = sMatchTime.Insert(2,":");
					iMinute = TimeGoneInMinute(DateTime.Parse(sMatchDate + " " + sMatchTime));
					SQLString.Remove(0,SQLString.Length);
					SQLString.Append("update LIVEODDS_INFO set ITIMEOFGAME=");
					SQLString.Append(iMinute.ToString());
					SQLString.Append(", CMATCHDATE='");
					SQLString.Append(arrMatchDate[iRenewStatus].Trim());
					SQLString.Append("', CMATCHTIME='");
					SQLString.Append(arrMatchTime[iRenewStatus].Trim());
					SQLString.Append("' where IMATCH_CNT=");
					SQLString.Append(arrMatchCnt[iRenewStatus]);
					SQLString.Append(" and IREGION_ID=");
					SQLString.Append(sRegionID);
					m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
					m_SportsDBMgr.Close();
				}
				if(iRenewStatus > 0) m_SportsDBMgr.Dispose();

				//write log
				m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " LiveOddsModify.cs: Renew " + iRenewStatus.ToString() + " matches time gone (" + HttpContext.Current.Session["user_name"] + ")");
				m_SportsLog.Close();
			} catch(Exception ex) {
				iRenewStatus = -1;
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " LiveOddsModify.cs.RenewMatchTimeGone(): " + ex.ToString());
				m_SportsLog.Close();
			}
			return iRenewStatus;
		}

		public int Sort() {
			int iSortIdx = 0;
			string sRegionID;
			char[] delimiter = new char[] {','};
			string[] arrOrder;
			string[] arrMatchCnt;

			arrOrder = HttpContext.Current.Request.Form["orderID"].Split(delimiter);
			arrMatchCnt = HttpContext.Current.Request.Form["match_cnt"].Split(delimiter);
			sRegionID = HttpContext.Current.Request.Form["RegionID"];
			try {
				for(iSortIdx = 0; iSortIdx < arrMatchCnt.Length; iSortIdx++) {
					SQLString.Remove(0,SQLString.Length);
					SQLString.Append("update LIVEODDS_INFO set IMATCHORDER=");
					if(!arrOrder[iSortIdx].Trim().Equals("")) {
						SQLString.Append(Convert.ToInt32(arrOrder[iSortIdx].Trim()).ToString());
					} else {
						SQLString.Append("null");
					}
					SQLString.Append(" where IMATCH_CNT=");
					SQLString.Append(arrMatchCnt[iSortIdx]);
					SQLString.Append(" and IREGION_ID=");
					SQLString.Append(sRegionID);
					m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
					m_SportsDBMgr.Close();
				}
				if(iSortIdx > 0) m_SportsDBMgr.Dispose();

				//write log
				m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " LiveOddsModify.cs: Sort " + iSortIdx.ToString() + " matches (" + HttpContext.Current.Session["user_name"] + ")");
				m_SportsLog.Close();
			} catch(Exception ex) {
				iSortIdx = -1;
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " LiveOddsModify.cs.Sort(): " + ex.ToString());
				m_SportsLog.Close();
			}
			return iSortIdx;
		}

		public int UpdateOdds() {
			int iTimeGone;
			int iSubIdx;
			int iRecIndex;
			int iMustSendItems;
			int iUpdIndex = 0;
			int iLogUpd = 0;
			int iPos;
			int len = 0;
			int iOddsStatus = 0;
			string sAlert = "";
			string sScoreHandicap = "";
			string sHostHandicap = "";
			string sBigSmallOdds = "-1";
			string sCurrentTimestamp = null;
			string sBatchJob = null;
			string sRegionID;
			string sMatchDate;
			string sMatchTime;
			string sScoreHandicap1 = "";
			string sScoreHandicap2 = "";
			char[] delimiter = new char[] {','};
			string[] arrHostName;
			string[] arrHostScr;
			string[] arrMatchDate;
			string[] arrMatchTime;
			string[] arrMatchStatus;
			string[] arrGuestName;
			string[] arrGuestScr;
			string[] arrMustSend;
			string[] arrMatchCnt;
			string[] arrAlert;
			string[] arrHostHandicap;
			string[] arrScoreHandicap;
			string[] arrOdds;
			string[] arrOddsStatus;
			string[] arrMatchField;
			string[] arrAlias;
			string[] arrLeague;
			string[] arrHostWear;
			string[] arrGuestWear;
			string[] arrTotalScore;
			string[] arrBigOdds;
			string[] arrMsgType;
			string[] arroddsItem;
			string[] arrSendToPager;

			//SportsMessage object message
			SportsMessage sptMsg = new SportsMessage();

			//Delcare variable used in message notify
			string[] arrQueueNames;
			string[] arrMessageTypes;
			string[] arrRemotingPath;
			arrQueueNames = (string[])HttpContext.Current.Application["QueueItems"];
			arrMessageTypes = (string[])HttpContext.Current.Application["NotifyMessageTypes"];
			arrRemotingPath = (string[])HttpContext.Current.Application["RemotingItems"];
			MessageClient msgClt = new MessageClient();

			try {
				arrSendToPager = HttpContext.Current.Request.Form["SendToPager"].Split(delimiter);
			} catch(Exception) {
				arrSendToPager = new string[0];
			}
			arroddsItem = (string[])HttpContext.Current.Application["oddsItemsArray"];
			arrHostName = HttpContext.Current.Request.Form["hostname"].Split(delimiter);
			arrHostScr = HttpContext.Current.Request.Form["hostscore"].Split(delimiter);
			arrGuestName = HttpContext.Current.Request.Form["guestname"].Split(delimiter);
			arrGuestScr = HttpContext.Current.Request.Form["guestscore"].Split(delimiter);
			arrMatchDate = HttpContext.Current.Request.Form["matchdate"].Split(delimiter);
			arrMatchTime = HttpContext.Current.Request.Form["matchtime"].Split(delimiter);
			arrMatchStatus = HttpContext.Current.Request.Form["MatchStatus"].Split(delimiter);
			arrScoreHandicap = HttpContext.Current.Request.Form["ScoreHandicap"].Split(delimiter);
			arrOdds = HttpContext.Current.Request.Form["odds"].Split(delimiter);
			arrOddsStatus = HttpContext.Current.Request.Form["OddsStatus"].Split(delimiter);
			arrAlias = HttpContext.Current.Request.Form["Alias"].Split(delimiter);
			arrLeague = HttpContext.Current.Request.Form["League"].Split(delimiter);
			arrMatchCnt = HttpContext.Current.Request.Form["match_cnt"].Split(delimiter);
			arrMatchField = HttpContext.Current.Request.Form["MatchField"].Split(delimiter);
			sRegionID = HttpContext.Current.Request.Form["RegionID"];
			arrHostWear = HttpContext.Current.Request.Form["hostwear"].Split(delimiter);
			arrGuestWear = HttpContext.Current.Request.Form["guestwear"].Split(delimiter);
			arrTotalScore = HttpContext.Current.Request.Form["totalscore"].Split(delimiter);
			arrBigOdds = HttpContext.Current.Request.Form["bigodds"].Split(delimiter);

			try {
				arrAlert = HttpContext.Current.Request.Form["Alert"].Split(delimiter);
			}	catch(Exception) {
				arrAlert = new string[0];
			}
			try {
				arrHostHandicap = HttpContext.Current.Request.Form["HostHandicap"].Split(delimiter);
			} catch(Exception) {
				arrHostHandicap = new string[0];
			}
			try {
				arrMustSend = HttpContext.Current.Request.Form["mustsend"].Split(delimiter);
				iMustSendItems = arrMustSend.Length;
			}	catch(Exception) {
				arrMustSend = new string[0];
				iMustSendItems = 0;
			}

			try {
				arrMsgType = (string[])HttpContext.Current.Application["messageType"];
				sCurrentTimestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
				sBatchJob = DateTime.Now.ToString("yyMMddHHmmss_fffffff") + "." + HttpContext.Current.Session["user_name"].ToString() + "." + arrMsgType[20] + ".ini";
				for(iRecIndex=0;iRecIndex<iMustSendItems;iRecIndex++) {
					iUpdIndex = Convert.ToInt32(arrMustSend[iRecIndex]);

					sAlert = "0";
					for(iSubIdx = 0; iSubIdx < arrAlert.Length; iSubIdx++) {
						if(arrAlert[iSubIdx] == iUpdIndex.ToString()) {
							sAlert = "1";
							break;
						}
					}

					sHostHandicap = "0";
					for(iSubIdx = 0; iSubIdx < arrHostHandicap.Length; iSubIdx++) {
						if(arrHostHandicap[iSubIdx] == iUpdIndex.ToString()) {
							sHostHandicap = "1";
							break;
						}
					}

					sMatchDate = arrMatchDate[iUpdIndex].Trim();
					sMatchDate = sMatchDate.Insert(4,"/");
					sMatchDate = sMatchDate.Insert(7,"/");
					sMatchTime = arrMatchTime[iUpdIndex].Trim();
					sMatchTime = sMatchTime.Insert(2,":");
					iTimeGone = TimeGoneInMinute(DateTime.Parse(sMatchDate + " " + sMatchTime));
					SQLString.Remove(0,SQLString.Length);
					SQLString.Append("update LIVEODDS_INFO set CMATCHTIME='");
					SQLString.Append(arrMatchTime[iUpdIndex]);
					SQLString.Append("', CMATCHDATE='");
					SQLString.Append(arrMatchDate[iUpdIndex]);
					SQLString.Append("', CMATCHSTATUS='");
					SQLString.Append(arrMatchStatus[iUpdIndex]);
					SQLString.Append("', CHOST_HANDI='");
					SQLString.Append(sHostHandicap);
					SQLString.Append("', IH_SCORE=");
					SQLString.Append(Convert.ToInt32(arrHostScr[iUpdIndex].Trim()));
					SQLString.Append(", IG_SCORE=");
					SQLString.Append(Convert.ToInt32(arrGuestScr[iUpdIndex].Trim()));
					SQLString.Append(", CH_WEAR='");
					SQLString.Append(arrHostWear[iUpdIndex].Trim());
					SQLString.Append("', CG_WEAR='");
					SQLString.Append(arrGuestWear[iUpdIndex].Trim());
					SQLString.Append("', ITIMEOFGAME=");
					SQLString.Append(iTimeGone);
					iPos = arrScoreHandicap[iUpdIndex].IndexOf("/");
					if(iPos > -1) {
						sScoreHandicap1 = arrScoreHandicap[iUpdIndex].Substring(0,iPos);
						len = arrScoreHandicap[iUpdIndex].Length;
						sScoreHandicap2 = arrScoreHandicap[iUpdIndex].Substring(iPos+1);
					} else {
						sScoreHandicap1 = arrScoreHandicap[iUpdIndex].ToString();
						sScoreHandicap2 = "";
					}
					SQLString.Append(", CHANDICAP1='");
					SQLString.Append(sScoreHandicap1.Trim());
					SQLString.Append("', CHANDICAP2='");
					SQLString.Append(sScoreHandicap2.Trim());
					SQLString.Append("', CHODDS='");
					SQLString.Append(arrOdds[iUpdIndex].Trim());
					SQLString.Append("', CTOTALSCORE='");
					SQLString.Append(arrTotalScore[iUpdIndex].Trim());
					SQLString.Append("', CBIGODDS='");
					SQLString.Append(arrBigOdds[iUpdIndex].Trim());
					SQLString.Append("', CSMALLODDS=''");
					SQLString.Append(", CODDSSTATUS='");
					iOddsStatus = Convert.ToInt32(arrOddsStatus[iUpdIndex]);
					if (iOddsStatus > 3) {
						iOddsStatus--;
					}
					SQLString.Append(iOddsStatus.ToString());
					SQLString.Append("' where IMATCH_CNT=");
					SQLString.Append(arrMatchCnt[iUpdIndex]);
					SQLString.Append(" and IREGION_ID=");
					SQLString.Append(sRegionID);
					m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
					m_SportsDBMgr.Close();

					if(arrSendToPager.Length>0) {
						iLogUpd++;
						SQLString.Remove(0,SQLString.Length);
						SQLString.Append("insert into LOG_LIVEODDS (TIMEFLAG, SECTION, LEAGUEALIAS, LEAGUE, HOST, GUEST, MATCHFIELD, HANDICAP, MATCHSTATUS, ACT, H_GOAL, G_GOAL, CURRTIME, HANDI, ODDS, ODDSSTATUS, ALERT, REGIONID, HOSTWEAR, GUESTWEAR, TOTALSCORE, BIGSMALLODDS, BATCHJOB) values ('");
						SQLString.Append(sCurrentTimestamp);
						SQLString.Append("','LIVE_','");
						SQLString.Append(arrAlias[iUpdIndex]);
						SQLString.Append("','");
						SQLString.Append(arrLeague[iUpdIndex]);
						SQLString.Append("','");
						SQLString.Append(arrHostName[iUpdIndex]);
						SQLString.Append("','");
						SQLString.Append(arrGuestName[iUpdIndex]);
						SQLString.Append("','");
						SQLString.Append(arrMatchField[iUpdIndex]);
						SQLString.Append("','");
						SQLString.Append(sHostHandicap);
						SQLString.Append("','");
						SQLString.Append(arrMatchStatus[iUpdIndex]);
						SQLString.Append("','U',");
						if(arrHostScr[iUpdIndex].Equals("")) SQLString.Append("-1");
						else SQLString.Append(arrHostScr[iUpdIndex]);
						SQLString.Append(",");
						if(arrGuestScr[iUpdIndex].Equals("")) SQLString.Append("-1");
						else SQLString.Append(arrGuestScr[iUpdIndex]);
						SQLString.Append(",");
						if(arrMatchStatus[iUpdIndex].Equals("1")) iTimeGone += 45;
						SQLString.Append(iTimeGone.ToString());
						SQLString.Append(",'");

						sScoreHandicap = arrScoreHandicap[iUpdIndex].ToString();
						if(!arrOdds[iUpdIndex].Equals("")) {
							SQLString.Append(sScoreHandicap);
						} else {
							SQLString.Append("-1");
						}
						SQLString.Append("','");
						if(arrOdds[iUpdIndex].Equals("")) SQLString.Append("-1");
						else SQLString.Append(arrOdds[iUpdIndex].Trim());
						SQLString.Append("','");
						SQLString.Append(arroddsItem[Convert.ToInt32(arrOddsStatus[iUpdIndex])]);
						SQLString.Append("',");
						SQLString.Append(sAlert);
						SQLString.Append(",");
						SQLString.Append(sRegionID);
						SQLString.Append(",'");
						if(!arrHostWear[iUpdIndex].Trim().Equals("")) {
							SQLString.Append(arrHostWear[iUpdIndex].Trim());
						} else {
							SQLString.Append("-1");
						}
						SQLString.Append("','");
						if(!arrGuestWear[iUpdIndex].Trim().Equals("")) {
							SQLString.Append(arrGuestWear[iUpdIndex].Trim());
						} else {
							SQLString.Append("-1");
						}
						SQLString.Append("','");

						if(arrBigOdds[iUpdIndex].Trim().Equals("")) {
							sBigSmallOdds = "-1";
						} else {
							sBigSmallOdds = arrBigOdds[iUpdIndex].Trim();
						}
						if(!arrTotalScore[iUpdIndex].Trim().Equals("")) {
							if(!sBigSmallOdds.Equals("-1")) {
								SQLString.Append(arrTotalScore[iUpdIndex].Trim());
							} else {
								SQLString.Append("-1");
							}
						} else {
							SQLString.Append("-1");
						}
						SQLString.Append("','");
						SQLString.Append(sBigSmallOdds);
						SQLString.Append("','");
						SQLString.Append(sBatchJob);
						SQLString.Append("')");
						m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
						m_SportsDBMgr.Close();
					}	//insert into LOG_LIVEODDS
				}	// end-for

				if(iLogUpd>0) {
					//Send Notify Message
					//modified by Chapman, 19 Feb 2004
					sptMsg.Body = sBatchJob;
					sptMsg.Timestamp = DateTime.Now.ToString("yyMMddHHmmss");
					sptMsg.AppID = "07";
					sptMsg.MsgID = "06";
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
							m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " MSMQ ERROR: Live Odds Modify");
							m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " LiveOddsModify.cs.UpdateOdds(): Notify via MSMQ throws MessageQueueException: " + mqEx.ToString());
							m_SportsLog.Close();

							//If MSMQ fail, notify via .NET Remoting
							msgClt.MessageType = arrMessageTypes[1];
							msgClt.MessagePath = arrRemotingPath[0];
							if(!msgClt.SendMessage((object)sptMsg)) {
								m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
								m_SportsLog.SetFileName(0,LOGFILESUFFIX);
								m_SportsLog.Open();
								m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Remoting ERROR: Live Odds Modify");
								m_SportsLog.Close();
							}
						}	catch(Exception ex) {
							m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
							m_SportsLog.SetFileName(0,LOGFILESUFFIX);
							m_SportsLog.Open();
							m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Remoting ERROR: Live Odds Modify");
							m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " LiveOddsModify.cs.UpdateOdds(): Notify via .NET Remoting throws exception: " + ex.ToString());
							m_SportsLog.Close();
						}

					}	catch(Exception ex) {
						m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
						m_SportsLog.SetFileName(0,LOGFILESUFFIX);
						m_SportsLog.Open();
						m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " LiveOddsModify.cs.UpdateOdds(): Notify via MSMQ throws exception: " + ex.ToString());
						m_SportsLog.Close();
					}
				}
				if(iRecIndex > 0) m_SportsDBMgr.Dispose();
/*
				//write log
				m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " LiveOddsModify.cs: Send " + iRecIndex.ToString() + " liveodds (" + HttpContext.Current.Session["user_name"] + ")");
				m_SportsLog.Close();
*/
			}	catch(Exception ex) {
				iRecIndex = -1;
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " LiveOddsModify.cs.UpdateOdds(): " + ex.ToString());
				m_SportsLog.Close();
			}

			return iRecIndex;
		}
	}
}