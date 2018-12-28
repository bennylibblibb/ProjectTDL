/*
Objective:
Retrieval and modify the Big/Small odds

Last updated:
1 Nov 2003 by Chapman

C#.NET complier statement:
csc /t:library /out:..\bin\BigSmallOdds.dll /r:..\bin\DBManager.dll;..\bin\Files.dll BigSmallOdds.cs
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
[assembly:AssemblyCopyright("(c) 2003 TDSL. All rights reserved. Created on 15 Oct 2003.")]
[assembly:AssemblyDescription("足球資訊 -> 大小盤")]
[assembly:AssemblyProduct("Sports.NET")]
[assembly:AssemblyTitle("Sports.NET DLL")]
[assembly:AssemblyVersion("3.0.*")]
namespace SportsUtil {
	public class BigSmallOdds {
		const string LOGFILESUFFIX = "log";
		const string DBCR = "(CR)";
		int m_RecordCount;
		int m_ExportSize;
		OleDbDataReader m_SportsOleReader;
		DBManager m_SportsDBMgr;
		Files m_SportsLog;
		Encoding m_Big5Encoded;
		StringBuilder SQLString;

		public BigSmallOdds(string Connection) {
			m_SportsDBMgr = new DBManager();
			m_SportsDBMgr.ConnectionString = Connection;
			m_SportsLog = new Files();
			m_Big5Encoded = Encoding.GetEncoding(950);
			SQLString = new StringBuilder();
			m_RecordCount = 0;
			m_ExportSize = 0;
		}

		public int NumberOfRecords {
			get {
				return m_RecordCount;
			}
		}

		public int ExportSize {
			get {
				return m_ExportSize;
			}
		}

		public string GetMatch() {
			int iArrayIndex = 0;
			int iStatus = 0;
			string uid;
			string sMatchDate;
			string sMatchTime;
			string[] oddsArray;
			StringBuilder HTMLString = new StringBuilder();

			uid = HttpContext.Current.Session["user_id"].ToString();
			oddsArray = (string[])HttpContext.Current.Application["oddsItemsArray"];
			try {
				SQLString.Remove(0,SQLString.Length);
				SQLString.Append("select max(IIMPORTSIZE) from BIGSMALLODDS_INFO");
				m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
				if(m_SportsOleReader.Read()) {
					if(!m_SportsOleReader.IsDBNull(0)) {
						m_ExportSize = m_SportsOleReader.GetInt32(0);
					} else {
						m_ExportSize = 1;
					}
				} else {
					m_ExportSize = 1;
				}
				m_SportsOleReader.Close();
				m_SportsDBMgr.Close();

				SQLString.Remove(0,SQLString.Length);
				SQLString.Append("SELECT bs.IMATCH_CNT, bs.IORDER_NO, game.MATCH_CNT, game.LEAGUE, game.HOST, game.GUEST, game.MATCHDATE, game.MATCHTIME, bs.CSCORE, bs.CBIGODDS, bs.CSMALLODDS, bs.CSTATUS ");
				SQLString.Append("FROM GAMEINFO game ");
				SQLString.Append("INNER JOIN LEAGINFO leag ON game.LEAGUE = leag.alias and leag.leag_id in (select cleag_id from userprofile_info where iuser_id=");
				SQLString.Append(uid);
				SQLString.Append(") LEFT OUTER JOIN BIGSMALLODDS_INFO bs ON bs.IMATCH_CNT = game.MATCH_CNT ");
				SQLString.Append("ORDER BY bs.IORDER_NO, leag.leag_order, game.MATCHDATE, game.MATCHTIME");
				m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
				while(m_SportsOleReader.Read()) {
					HTMLString.Append("<tr align=\"center\"");
					if(!m_SportsOleReader.IsDBNull(0)) {
						HTMLString.Append(" style=\"background-color:#DDA0DD\"");
					}

					//Match Order No
					HTMLString.Append("><td><input name=\"orderID\" value=\"");
					if(!m_SportsOleReader.IsDBNull(1)) {
						HTMLString.Append(m_SportsOleReader.GetInt32(1).ToString());
					}
					HTMLString.Append("\" maxlength=\"3\" size=\"1\" onChange=\"onOrderNoChange(");
					HTMLString.Append(m_RecordCount.ToString());
					HTMLString.Append(")\" tabindex=\"");
					HTMLString.Append((m_RecordCount+1).ToString());
					HTMLString.Append("\"></td><td>");

					//Match Count, Alias, Host, Guest
					HTMLString.Append("<input type=\"hidden\" name=\"matchcount\" value=\"");
					HTMLString.Append(m_SportsOleReader.GetInt32(2).ToString());
					HTMLString.Append("\">");
					HTMLString.Append(m_SportsOleReader.GetString(3).Trim());
					HTMLString.Append("</td><td>");
					HTMLString.Append(m_SportsOleReader.GetString(4).Trim());
					HTMLString.Append("</td><td>");
					HTMLString.Append(m_SportsOleReader.GetString(5).Trim());

					//Match Date and Time
					HTMLString.Append("</td><td>");
					sMatchDate = m_SportsOleReader.GetString(6).Trim();
					sMatchDate = sMatchDate.Insert(4,"/");
					sMatchDate = sMatchDate.Insert(7,"/");
					sMatchTime = m_SportsOleReader.GetString(7).Trim();
					sMatchTime = sMatchTime.Insert(2,":");
					HTMLString.Append(sMatchDate);
					HTMLString.Append(" ");
					HTMLString.Append(sMatchTime);

					//Action
					HTMLString.Append("</td><td><select name=\"action\"><option value=\"U\">修改<option value=\"D\">刪除</select>");

					//BigSmall - total score, odds
					HTMLString.Append("</td><td><input name=\"totalscore\" maxlength=\"6\" size=\"3\" value=\"");
					if(!m_SportsOleReader.IsDBNull(8)) {
						HTMLString.Append(m_SportsOleReader.GetString(8).Trim());
					}
					HTMLString.Append("\"></td><td>大<input name=\"bigodds\" maxlength=\"5\" size=\"2\" value=\"");
					if(!m_SportsOleReader.IsDBNull(9)) {
						HTMLString.Append(m_SportsOleReader.GetString(9).Trim());
					}
					HTMLString.Append("\" onChange=\"onBigOddsChange(");
					HTMLString.Append(m_RecordCount.ToString());
					HTMLString.Append(")\">/<input name=\"smallodds\" maxlength=\"5\" size=\"2\" value=\"");
					if(!m_SportsOleReader.IsDBNull(10)) {
						HTMLString.Append(m_SportsOleReader.GetString(10).Trim());
					}
					HTMLString.Append("\" onChange=\"onSmallOddsChange(");
					HTMLString.Append(m_RecordCount.ToString());
					HTMLString.Append(")\"></td>");

					//Odds Status
					iArrayIndex = 0;
					if(!m_SportsOleReader.IsDBNull(11)) {
						iStatus = Convert.ToInt32(m_SportsOleReader.GetString(11));
					} else {
						iStatus = 0;
					}
					HTMLString.Append("<td><select name=\"status\"><option value=");
					HTMLString.Append(iStatus.ToString());
					HTMLString.Append(">");
					HTMLString.Append(oddsArray[iStatus]);
					foreach(String sItem in oddsArray) {
						if(!sItem.Equals(oddsArray[iStatus]) && iArrayIndex != 3) {
							HTMLString.Append("<option value=");
							HTMLString.Append(iArrayIndex.ToString());
							HTMLString.Append(">");
							HTMLString.Append(sItem);
						}
						iArrayIndex++;
					}
					HTMLString.Append("</select></td><td>");

					//Export checkbox
					HTMLString.Append("<input type=\"checkbox\" name=\"exported\" value=\"");
					HTMLString.Append(m_RecordCount.ToString());
					HTMLString.Append("\"></td></tr>");
					m_RecordCount++;
				}
				m_SportsOleReader.Close();
				m_SportsDBMgr.Close();
				m_SportsDBMgr.Dispose();
			}	catch(Exception ex) {
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " BigSmallOdds.cs.GetMatch(): " + ex.ToString());
				m_SportsLog.Close();
				HTMLString.Remove(0,HTMLString.Length);
				HTMLString.Append((string)HttpContext.Current.Application["accessErrorMsg"]);
			}

			return HTMLString.ToString();
		}

		public int Modify() {
			int iResult = 0;
			int iExisted = 0;
			string sImportSize;
			char[] delimiter = new char[] {','};
			string[] arrAction;
			string[] arrMatchCount;
			string[] arrTotalScore;
			string[] arrBigOdds;
			string[] arrSmallOdds;
			string[] arrStatus;
			string[] arrOrderID;

			sImportSize = HttpContext.Current.Request.Form["importsize"];
			if(sImportSize == null) sImportSize = "1";
			if(sImportSize.Trim().Equals("")) sImportSize = "1";
			sImportSize = Convert.ToInt32(sImportSize).ToString();
			arrAction = HttpContext.Current.Request.Form["action"].Split(delimiter);
			arrMatchCount = HttpContext.Current.Request.Form["matchcount"].Split(delimiter);
			arrTotalScore = HttpContext.Current.Request.Form["totalscore"].Split(delimiter);
			arrBigOdds = HttpContext.Current.Request.Form["bigodds"].Split(delimiter);
			arrSmallOdds = HttpContext.Current.Request.Form["smallodds"].Split(delimiter);
			arrStatus = HttpContext.Current.Request.Form["status"].Split(delimiter);
			arrOrderID = HttpContext.Current.Request.Form["orderID"].Split(delimiter);

			try {
				SQLString.Remove(0,SQLString.Length);
				SQLString.Append("update BIGSMALLODDS_INFO set IIMPORTSIZE=");
				SQLString.Append(sImportSize);
				m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
				m_SportsDBMgr.Close();

				for(iResult = 0; iResult < arrMatchCount.Length; iResult++) {
					SQLString.Remove(0,SQLString.Length);
					SQLString.Append("select count(IMATCH_CNT) from BIGSMALLODDS_INFO where IMATCH_CNT=");
					SQLString.Append(arrMatchCount[iResult]);
					iExisted = m_SportsDBMgr.ExecuteScalar(SQLString.ToString());
					m_SportsDBMgr.Close();

					SQLString.Remove(0,SQLString.Length);
					if(arrAction[iResult].Equals("U")) {
						if(iExisted > 0) {	//update existing record
							SQLString.Append("update BIGSMALLODDS_INFO set IORDER_NO=");
							if(!arrOrderID[iResult].Trim().Equals("")) {
								SQLString.Append(arrOrderID[iResult].Trim());
							} else {
								SQLString.Append("null");
							}
							SQLString.Append(", CACT='U', CSCORE=");
							if(!arrTotalScore[iResult].Trim().Equals("")) {
								SQLString.Append("'");
								SQLString.Append(arrTotalScore[iResult].Trim());
								SQLString.Append("'");
							} else {
								SQLString.Append("null");
							}
							SQLString.Append(", CBIGODDS=");
							if(!arrBigOdds[iResult].Trim().Equals("")) {
								SQLString.Append("'");
								SQLString.Append(arrBigOdds[iResult]);
								SQLString.Append("'");
							} else {
								SQLString.Append("null");
							}
							SQLString.Append(", CSMALLODDS=");
							if(!arrSmallOdds[iResult].Trim().Equals("")) {
								SQLString.Append("'");
								SQLString.Append(arrSmallOdds[iResult]);
								SQLString.Append("'");
							} else {
								SQLString.Append("null");
							}
							SQLString.Append(", CSTATUS=");
							if(!arrStatus[iResult].Trim().Equals("")) {
								SQLString.Append("'");
								SQLString.Append(arrStatus[iResult]);
								SQLString.Append("'");
							} else {
								SQLString.Append("null");
							}
							SQLString.Append(" where IMATCH_CNT=");
							SQLString.Append(arrMatchCount[iResult]);
						} else {	//insert a new record
							SQLString.Append("insert into BIGSMALLODDS_INFO values(");
							SQLString.Append(arrMatchCount[iResult]);
							SQLString.Append(",");
							if(!arrOrderID[iResult].Trim().Equals("")) {
								SQLString.Append(arrOrderID[iResult].Trim());
							} else {
								SQLString.Append("null");
							}
							SQLString.Append(",'U',");
							if(!arrTotalScore[iResult].Trim().Equals("")) {
								SQLString.Append("'");
								SQLString.Append(arrTotalScore[iResult]);
								SQLString.Append("'");
							} else {
								SQLString.Append("null");
							}
							SQLString.Append(",");
							if(!arrBigOdds[iResult].Trim().Equals("")) {
								SQLString.Append("'");
								SQLString.Append(arrBigOdds[iResult]);
								SQLString.Append("'");
							} else {
								SQLString.Append("null");
							}
							SQLString.Append(",");
							if(!arrSmallOdds[iResult].Trim().Equals("")) {
								SQLString.Append("'");
								SQLString.Append(arrSmallOdds[iResult]);
								SQLString.Append("'");
							} else {
								SQLString.Append("null");
							}
							SQLString.Append(",");
							if(!arrStatus[iResult].Trim().Equals("")) {
								SQLString.Append("'");
								SQLString.Append(arrStatus[iResult]);
								SQLString.Append("'");
							} else {
								SQLString.Append("null");
							}
							SQLString.Append(",");
							SQLString.Append(sImportSize);
							SQLString.Append(")");
						}
					} else {	//delete record
						SQLString.Append("delete from BIGSMALLODDS_INFO where IMATCH_CNT=");
						SQLString.Append(arrMatchCount[iResult]);
					}
					m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
					m_SportsDBMgr.Close();
				}	//end-for
				m_SportsDBMgr.Dispose();

				//write log
				m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " BigSmallOdds.cs: Update " + iResult.ToString() + " records. (" + HttpContext.Current.Session["user_name"] + ")");
				m_SportsLog.Close();
			}	catch(Exception ex) {
				iResult = -1;
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " BigSmallOdds.cs.Modify(): " + ex.ToString());
				m_SportsLog.Close();
			}

			return iResult;
		}

		public int Export() {
			const int TEAMLENGTH = 6;
			int iIdx = 0;
			int iAppID = 0;
			int iExportSize = 0;
			int iExported = 0;
			int iScoreLen = 0;
			int iMaxMsgCount = 20;
			int iMsgFormed = 0;
			string sImportSize;
			string sMsgID;
			char[] delimiter = new char[] {','};
			byte[] arrScoreByte;
			string[] oddsArray;
			string[] arrMatchCount;
			string[] arrExportedItem;
			ArrayList MsgIDList = new ArrayList();
			StringBuilder FreeTextContent = new StringBuilder();

			sImportSize = HttpContext.Current.Request.Form["importsize"];
			if(sImportSize == null) sImportSize = "1";
			if(sImportSize.Trim().Equals("")) sImportSize = "1";
			iExportSize = Convert.ToInt32(sImportSize);
			oddsArray = (string[])HttpContext.Current.Application["oddsItemsArray"];
			arrMatchCount = HttpContext.Current.Request.Form["matchcount"].Split(delimiter);
			arrExportedItem = HttpContext.Current.Request.Form["exported"].Split(delimiter);

			try {
				SQLString.Remove(0,SQLString.Length);
				SQLString.Append("update BIGSMALLODDS_INFO set IIMPORTSIZE=");
				SQLString.Append(iExportSize.ToString());
				m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
				m_SportsDBMgr.Close();

				//grep Msg ID from NEWSINFO (free text) related to 大小盤
				SQLString.Remove(0,SQLString.Length);
				SQLString.Append("select ISEQNO, IHDRIDSTART, IHDRIDEND from NEWS_CFG where CINFOTYPE like '%大小盤%'");
				m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
				if(m_SportsOleReader.Read()) {
					if(!m_SportsOleReader.IsDBNull(0)) {
						iAppID = m_SportsOleReader.GetInt32(0);
						iMaxMsgCount = m_SportsOleReader.GetInt32(2) - m_SportsOleReader.GetInt32(1) + 1;
					} else {
						iAppID = -1;
					}
				} else {
					iAppID = -1;
				}
				m_SportsOleReader.Close();
				m_SportsDBMgr.Close();

				if(iAppID != -1) {
					SQLString.Remove(0,SQLString.Length);
					SQLString.Append("select IMSG_ID from NEWSINFO where IAPP_ID=");
					SQLString.Append(iAppID.ToString());
					SQLString.Append("order by IMSG_ID");
					m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
					while(m_SportsOleReader.Read()) {
						MsgIDList.Add(m_SportsOleReader.GetInt32(0).ToString());
					}
					m_SportsOleReader.Close();
					m_SportsDBMgr.Close();
					MsgIDList.TrimToSize();

					int iMatchCnt = 0;
					//Construct content into specific record
					for(iExported = 0; iExported < arrExportedItem.Length; iExported++) {
						//reset variable
						iScoreLen = 0;

						if(iMatchCnt > 0) FreeTextContent.Append(DBCR);	//CR except last message

						iIdx = Convert.ToInt32(arrExportedItem[iExported]);
						SQLString.Remove(0,SQLString.Length);
						SQLString.Append("select game.HOST, game.GUEST, bs.CSTATUS, game.MATCHTIME, bs.CSCORE, bs.CBIGODDS, bs.CSMALLODDS from GAMEINFO game, BIGSMALLODDS_INFO bs where game.MATCH_CNT = bs.IMATCH_CNT and bs.IMATCH_CNT=");
						SQLString.Append(arrMatchCount[iIdx]);
						m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
						if(m_SportsOleReader.Read()) {
							FreeTextContent.Append(PadRightSpace(m_SportsOleReader.GetString(0).Trim(),TEAMLENGTH));
							FreeTextContent.Append("vs");
							FreeTextContent.Append(PadRightSpace(m_SportsOleReader.GetString(1).Trim(),TEAMLENGTH));
							if(!m_SportsOleReader.IsDBNull(2)) {
								if(m_SportsOleReader.GetString(2).Equals("0")) {
									FreeTextContent.Append(" ");
									FreeTextContent.Append(m_SportsOleReader.GetString(3).Trim().Substring(0,2));
									FreeTextContent.Append(":");
									FreeTextContent.Append(m_SportsOleReader.GetString(3).Trim().Substring(2));
								} else {
									FreeTextContent.Append("  ");
									FreeTextContent.Append(oddsArray[Convert.ToInt32(m_SportsOleReader.GetString(2))]);
								}
							} else {
								FreeTextContent.Append(" ");
								FreeTextContent.Append(m_SportsOleReader.GetString(3).Trim().Substring(0,2));
								FreeTextContent.Append(":");
								FreeTextContent.Append(m_SportsOleReader.GetString(3).Trim().Substring(2));
							}
							FreeTextContent.Append(DBCR);

							if(!m_SportsOleReader.IsDBNull(4)) {
								FreeTextContent.Append(m_SportsOleReader.GetString(4).Trim());
								FreeTextContent.Append("球");
								arrScoreByte = m_Big5Encoded.GetBytes(m_SportsOleReader.GetString(4).Trim() + "球");
								iScoreLen = arrScoreByte.Length;
								for(int i = iScoreLen; i < 7; i++) {
									FreeTextContent.Append(" ");
								}
							} else {
								FreeTextContent.Append("       ");
							}
							if(!m_SportsOleReader.IsDBNull(5) && !m_SportsOleReader.IsDBNull(6)) {
								FreeTextContent.Append(" 大");
							}
							if(!m_SportsOleReader.IsDBNull(5)) {
								FreeTextContent.Append(m_SportsOleReader.GetString(5).Trim());
							}
							if(!m_SportsOleReader.IsDBNull(6)) {
								FreeTextContent.Append("//");
								FreeTextContent.Append(m_SportsOleReader.GetString(6).Trim());
							}
							//FreeTextContent.Append(DBCR);

							iMatchCnt++;
						}
						m_SportsOleReader.Close();
						m_SportsDBMgr.Close();

						if((iMatchCnt == iExportSize) || (arrExportedItem.Length - iExported == 1)) {
							if((FreeTextContent.Length > 0) && (iMsgFormed < iMaxMsgCount)) {
								if(MsgIDList.Count > 0) {	//update to NEWSINFO
									sMsgID = (string)MsgIDList[0];
									SQLString.Remove(0,SQLString.Length);
									SQLString.Append("update NEWSINFO set CACT='U', CNEWSDATE='");
									SQLString.Append(DateTime.Now.ToString("yyyyMMdd"));
									SQLString.Append("', CNEWSTIME='");
									SQLString.Append(DateTime.Now.ToString("hhmm"));
									SQLString.Append("', CMESSAGE='");
									SQLString.Append(FreeTextContent.ToString());
									SQLString.Append("' where IAPP_ID=");
									SQLString.Append(iAppID.ToString());
									SQLString.Append(" and IMSG_ID=");
									SQLString.Append(sMsgID);
									m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
									m_SportsDBMgr.Close();
									MsgIDList.RemoveAt(0);
								} else {	//insert new message into NEWSINFO
									SQLString.Remove(0,SQLString.Length);
									SQLString.Append("select max(IMSG_ID) from NEWSINFO");
									m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
									if(m_SportsOleReader.Read()) {
										if(!m_SportsOleReader.IsDBNull(0)) {
											sMsgID = (m_SportsOleReader.GetInt32(0) + 1).ToString();
										} else {
											sMsgID = "1";
										}
									} else {
										sMsgID = "1";
									}
									m_SportsOleReader.Close();
									m_SportsDBMgr.Close();

									SQLString.Remove(0,SQLString.Length);
									SQLString.Append("insert into NEWSINFO values(");
									SQLString.Append(sMsgID);
									SQLString.Append(",'U',");
									SQLString.Append(iAppID.ToString());
									SQLString.Append(",'");
									SQLString.Append(DateTime.Now.ToString("yyyyMMdd"));
									SQLString.Append("','");
									SQLString.Append(DateTime.Now.ToString("hhmm"));
									SQLString.Append("','");
									SQLString.Append(FreeTextContent.ToString());
									SQLString.Append("','0')");
									m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
									m_SportsDBMgr.Close();
								}
								iMsgFormed++;
							}

							iMatchCnt = 0;
							FreeTextContent.Remove(0,FreeTextContent.Length);
						}
					}	//end-for

					if(MsgIDList.Count > 0) {
						for(int j = 0; j < MsgIDList.Count; j++) {
							SQLString.Remove(0,SQLString.Length);
							SQLString.Append("update NEWSINFO set CACT='U', CNEWSDATE='");
							SQLString.Append(DateTime.Now.ToString("yyyyMMdd"));
							SQLString.Append("', CNEWSTIME='");
							SQLString.Append(DateTime.Now.ToString("hhmm"));
							SQLString.Append("', CMESSAGE='稍後提供' where IAPP_ID=");
							SQLString.Append(iAppID.ToString());
							SQLString.Append(" and IMSG_ID=");
							SQLString.Append((string)MsgIDList[j]);
							m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
							m_SportsDBMgr.Close();
						}
					}
				} else {
					iExported = -1;
				}
				m_SportsDBMgr.Dispose();

				//write log
				m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " BigSmallOdds.cs: Export " + iExported.ToString() + " records. (" + HttpContext.Current.Session["user_name"] + ")");
				m_SportsLog.Close();
			}	catch(Exception ex) {
				iExported = -1;
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " BigSmallOdds.cs.Export(): " + ex.ToString());
				m_SportsLog.Close();
			}

			return iExported;
		}

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
	}
}