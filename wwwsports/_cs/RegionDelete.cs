/*
Objective:
Delete other region matches

Last updated:
12 Feb 2004 by HenryCi

C#.NET complier statement:
csc /t:library /out:..\bin\RegionDelete.dll /r:..\bin\DBManager.dll;..\bin\Files.dll;..\bin\MessageClient.dll;..\bin\SportsMessage.dll RegionDelete.cs
*/

using System;
using System.Data.OleDb;
using System.Reflection;
using System.Text;
using System.Web;
using System.Collections;
using TDL.DB;
using TDL.IO;
using TDL.Message;

[assembly:AssemblyCompany("TDSL")]
[assembly:AssemblyCopyright("(c) 2003 TDSL. All rights reserved.")]
[assembly:AssemblyDescription("其他地區 -> 刪除賽事")]
[assembly:AssemblyProduct("Sports.NET")]
[assembly:AssemblyTitle("Sports.NET DLL")]
[assembly:AssemblyVersion("3.1.*")]
namespace SportsUtil {
	public class RegionDelete {
		const string LOGFILESUFFIX = "log";
		int m_iRecordCount = 0;
		string m_Region;
		OleDbDataReader m_SportsOleReader;
		DBManager m_SportsDBMgr;
		Files m_SportsLog;
		Encoding m_Big5Encoded;
		StringBuilder SQLString;

		public RegionDelete(string Connection) {
			m_SportsDBMgr = new DBManager();
			m_SportsDBMgr.ConnectionString = Connection;
			m_SportsLog = new Files();
			m_Big5Encoded = Encoding.GetEncoding(950);
			SQLString = new StringBuilder();
		}

		public int NumberOfRecords {
			get {
				return m_iRecordCount;
			}
		}

		public string Region {
			get {
				return m_Region;
			}
		}

		public string GetMatches() {
			string sRecordStr = null;
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
				SQLString.Append("select distinct leag.LEAG_ID, leag.ALIAS from LEAGINFO leag, OTHERODDSINFO game where leag.leag_id in (select cleag_id from userprofile_info where iuser_id=");
				SQLString.Append(uid);
				SQLString.Append(") AND leag.leagname = game.LEAGUE order by leag.LEAG_ORDER, leag.LEAG_ID");
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
				SQLString.Append("select SubStr(MATCHDATE, 1, 4), SubStr(MATCHDATE, 5, 2), SubStr(MATCHDATE, 7, 2) from OTHERODDSINFO order by MATCHDATE, MATCHTIME");
				m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
				while(m_SportsOleReader.Read()) {
					if(!sTempYear.Equals(m_SportsOleReader.GetString(0).Trim())) {
						sTempYear = m_SportsOleReader.GetString(0).Trim();
						sYearOption += "<option value=\"" + sTempYear + "\">" + sTempYear + "</option>";
					}

					sTempMonth = m_SportsOleReader.GetString(1).Trim();
					if(!monthList.Contains(sTempMonth)) {
						monthList.Add(sTempMonth);
						sMonthOption += "<option value=\"" + sTempMonth + "\">" + sTempMonth + "</option>";
					}

					sTempDate = m_SportsOleReader.GetString(2).Trim();
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
				sEnquiryOut += "onClick=\"filterSchedule('RegionDelete.aspx?RegionID=' +"+sRegionID+"+ '&leagID=' + DeleteRegionForm.leagID.value + '&Year=' + DeleteRegionForm.Year.value + '&Month=' + DeleteRegionForm.Month.value + '&Date=' + DeleteRegionForm.Date.value)\"></th</tr>";
				HTMLString.Append(sEnquiryOut);
				
				HTMLString.Append("<tr style=\"background-color:#E6E6FA\">");
				HTMLString.Append("<th>日期</th>");
				HTMLString.Append("<th>時間</th>");
				HTMLString.Append("<th>聯賽</th>");
				HTMLString.Append("<th>主隊</th>");
				HTMLString.Append("<th>客隊</th>");
				HTMLString.Append("<th>全選<input type=\"checkbox\" name=\"SelectAllSend\" onClick=\"selectAll()\"></th>");
				HTMLString.Append("</tr>");

				SQLString.Remove(0,SQLString.Length);
				SQLString.Append("select game.MATCH_CNT, game.MATCHDATE, game.MATCHTIME, game.LEAGUE, game.ALIAS, game.HOST, game.GUEST ");
				SQLString.Append("from OTHERODDSINFO game, LEAGINFO leag where game.COMPANY='");
				SQLString.Append(m_Region);
				SQLString.Append("' and leag.leagname = game.LEAGUE and ACT='U'");
				
				if(sLeagID != null) {
					if(!sLeagID.Equals("0000")) {
						SQLString.Append("and leag.LEAG_ID='");
						SQLString.Append(sLeagID);
						SQLString.Append("' ");
					}
				}
				if(sMatchYear != null) {
					if(!sMatchYear.Equals("0000")) {
						SQLString.Append("and SubStr(game.MATCHDATE, 1, 4)='");
						SQLString.Append(sMatchYear);
						SQLString.Append("' ");
					}
				}
				if(sMatchMonth != null) {
					if(!sMatchMonth.Equals("0000")) {
						SQLString.Append("and SubStr(game.MATCHDATE, 5, 2)='");
						SQLString.Append(sMatchMonth);
						SQLString.Append("' ");
					}
				}
				if(sMatchDay != null) {
					if(!sMatchDay.Equals("0000")) {
						SQLString.Append("and SubStr(game.MATCHDATE, 7, 2)='");
						SQLString.Append(sMatchDay);
						SQLString.Append("' ");
					}
				}
				SQLString.Append("order by game.MATCH_ID, game.MATCH_CNT");
				m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
				while(m_SportsOleReader.Read()) {
					HTMLString.Append("<tr align=\"center\">");
					
					//Match Count (Hidden field)
					sRecordStr = m_SportsOleReader.GetInt32(0).ToString();
					HTMLString.Append("<input type=\"hidden\" name=\"MatchCount\" value=\"");
					HTMLString.Append(sRecordStr);
					HTMLString.Append("\"></td>");

					//Match Date and Time (Hidden Field)
					sRecordStr = m_SportsOleReader.GetString(1);
					HTMLString.Append("<td><input type=\"hidden\" name=\"MatchDate\" value=\"");
					HTMLString.Append(sRecordStr);
					HTMLString.Append("\">");
					sRecordStr = sRecordStr.Insert(4,"/");
					sRecordStr = sRecordStr.Insert(7,"/");
					HTMLString.Append(sRecordStr);
					HTMLString.Append("</td>");
					sRecordStr = m_SportsOleReader.GetString(2);
					HTMLString.Append("<td><input type=\"hidden\" name=\"MatchTime\" value=\"");
					HTMLString.Append(sRecordStr);
					HTMLString.Append("\">");
					sRecordStr = sRecordStr.Insert(2,":");
					HTMLString.Append(sRecordStr);
					HTMLString.Append("</td>");

					//League, Region and Alias(Hidden)
					sRecordStr = m_SportsOleReader.GetString(3).Trim();
					HTMLString.Append("<td>");
					HTMLString.Append(sRecordStr);
					HTMLString.Append("<input type=\"hidden\" name=\"League\" value=\"");
					HTMLString.Append(sRecordStr);
					HTMLString.Append("\">");
					sRecordStr = m_SportsOleReader.GetString(4).Trim();
					HTMLString.Append("<input type=\"hidden\" name=\"Alias\" value=\"");
					HTMLString.Append(sRecordStr);
					HTMLString.Append("\"></td>");

					//Host
					sRecordStr = m_SportsOleReader.GetString(5).Trim();
					HTMLString.Append("<td>");
					HTMLString.Append(sRecordStr);
					HTMLString.Append("<input type=\"hidden\" name=\"Host\" value=\"");
					HTMLString.Append(sRecordStr);
					HTMLString.Append("\"></td>");

					//Guest
					sRecordStr = m_SportsOleReader.GetString(6).Trim();
					HTMLString.Append("<td>");
					HTMLString.Append(sRecordStr);
					HTMLString.Append("<input type=\"hidden\" name=\"Guest\" value=\"");
					HTMLString.Append(sRecordStr);
					HTMLString.Append("\">");
					
					HTMLString.Append("<td><input type=\"checkbox\" name=\"deleteItem\" value=\"");
					HTMLString.Append(m_iRecordCount.ToString());
					HTMLString.Append("\"></tr>");
					m_iRecordCount++;
				}
				m_SportsDBMgr.Close();
				m_SportsOleReader.Close();
				m_SportsDBMgr.Dispose();
				HTMLString.Append("<input type=\"hidden\" name=\"RecordCount\" value=\"");
				HTMLString.Append(m_iRecordCount.ToString());
				HTMLString.Append("\"><input type=\"hidden\" name=\"Region\" value=\"");
				HTMLString.Append(m_Region);
				HTMLString.Append("\">");
			} catch(Exception ex) {
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " RegionDelete.cs.GetMatches(): " + ex.ToString());
				m_SportsLog.Close();
				HTMLString.Remove(0,HTMLString.Length);
				HTMLString.Append((string)HttpContext.Current.Application["accessErrorMsg"]);
			}

			return HTMLString.ToString();
		}

		public int Delete() {
			char[] delimiter = new char[] {','};
			int iRecIndex;
			int iDeletedItemLen;
			int iUpdIndex = 0;
			int iSuccessUpd = 0;
			string sRegion;
			string sCurrentTimestamp = null;
			string sBatchJob = null;
			string[] arrMatchCnt;
			string[] arrSendRequired;
			string[] arrLeague;
			string[] arrAlias;
			string[] arrHost;
			string[] arrGuest;
			string[] arrMatchDate;
			string[] arrMatchTime;
			string[] arrSendToPager;

			//SportsMessage object message
			SportsMessage sptMsg = new SportsMessage();

			try {
				arrSendToPager = HttpContext.Current.Request.Form["SendToPager"].Split(delimiter);
			} catch(Exception) {
				arrSendToPager = new string[0];
			}
			try {
				arrSendRequired = HttpContext.Current.Request.Form["deleteItem"].Split(delimiter);
				iDeletedItemLen = arrSendRequired.Length;
			} catch(Exception) {
				arrSendRequired = new string[0];
				iDeletedItemLen = 0;
			}
			sRegion = HttpContext.Current.Request.Form["Region"];
			if(sRegion == null) sRegion = "";
			arrMatchCnt = HttpContext.Current.Request.Form["MatchCount"].Split(delimiter);
			arrLeague = HttpContext.Current.Request.Form["League"].Split(delimiter);
			arrAlias = HttpContext.Current.Request.Form["Alias"].Split(delimiter);
			arrHost = HttpContext.Current.Request.Form["Host"].Split(delimiter);
			arrGuest = HttpContext.Current.Request.Form["Guest"].Split(delimiter);
			arrMatchDate = HttpContext.Current.Request.Form["MatchDate"].Split(delimiter);
			arrMatchTime = HttpContext.Current.Request.Form["MatchTime"].Split(delimiter);

			if(iDeletedItemLen > 0 && arrSendToPager.Length > 0) {
				try {
					//Delcare variable used in message notify
					string[] arrQueueNames;
					string[] arrRemotingPath;
					string[] arrMessageTypes;
					arrQueueNames = (string[])HttpContext.Current.Application["QueueItems"];
					arrRemotingPath = (string[])HttpContext.Current.Application["RemotingItems"];
					arrMessageTypes = (string[])HttpContext.Current.Application["NotifyMessageTypes"];
					MessageClient msgClt = new MessageClient();

					string[] arrMsgType;
					arrMsgType = (string[])HttpContext.Current.Application["messageType"];
					StringBuilder LogSQLString = new StringBuilder();
					DBManager logDBMgr = new DBManager();
					logDBMgr.ConnectionString = m_SportsDBMgr.ConnectionString;

					sCurrentTimestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
					sBatchJob = DateTime.Now.ToString("yyMMddHHmmss_fffffff") + "." + HttpContext.Current.Session["user_name"].ToString() + "." + arrMsgType[19] + ".ini";
					for(iRecIndex = 0; iRecIndex < iDeletedItemLen; iRecIndex++) {
						iUpdIndex = Convert.ToInt32(arrSendRequired[iRecIndex]);
						SQLString.Remove(0,SQLString.Length);
						SQLString.Append("update OTHERODDSINFO set ACT='D' where COMPANY='");
						SQLString.Append(sRegion);
						SQLString.Append("' AND MATCH_CNT=");
						SQLString.Append(arrMatchCnt[iUpdIndex]);
						m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
						m_SportsDBMgr.Close();

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
						LogSQLString.Append("','H','D','1','-1','-1','-1','-1','-1',0,'-1','");
						LogSQLString.Append(sBatchJob);
						LogSQLString.Append("')");
						logDBMgr.ExecuteNonQuery(LogSQLString.ToString());
						logDBMgr.Close();
						iSuccessUpd++;
					}

					if(iSuccessUpd > 0) {
						//Tell MessageDispatcher to generate Delete Other Odds INI and notify other processes
						//Assign value to SportsMessage object
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
								m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " MSMQ ERROR: Delete Other Region Odds");
								m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " RegionDelete.cs.Delete(): Notify via MSMQ throws MessageQueueException: " + mqEx.ToString());
								m_SportsLog.Close();

								//If MSMQ fail, notify via .NET Remoting
								msgClt.MessageType = arrMessageTypes[1];
								msgClt.MessagePath = arrRemotingPath[0];
								if(!msgClt.SendMessage((object)sptMsg)) {
									m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
									m_SportsLog.SetFileName(0,LOGFILESUFFIX);
									m_SportsLog.Open();
									m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Remoting ERROR: Delete Other Region Odds");
									m_SportsLog.Close();
								}
							}	catch(Exception ex) {
								m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
								m_SportsLog.SetFileName(0,LOGFILESUFFIX);
								m_SportsLog.Open();
								m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Remoting ERROR: Delete Other Region Odds");
								m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " RegionDelete.cs.Delete(): Notify via .NET Remoting throws exception: " + ex.ToString());
								m_SportsLog.Close();
							}
						} catch(Exception ex) {
							m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
							m_SportsLog.SetFileName(0,LOGFILESUFFIX);
							m_SportsLog.Open();
							m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " RegionDelete.cs.Delete(): Notify via MSMQ throws exception: " + ex.ToString());
							m_SportsLog.Close();
						}
						m_SportsDBMgr.Dispose();
					}

					//write log
					m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
					m_SportsLog.SetFileName(0,LOGFILESUFFIX);
					m_SportsLog.Open();
					m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " RegionDelete.cs: Delete " + iSuccessUpd.ToString() + " matches (" + HttpContext.Current.Session["user_name"] + ")");
					m_SportsLog.Close();
				} catch(Exception ex) {
					iSuccessUpd = -1;
					m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
					m_SportsLog.SetFileName(0,LOGFILESUFFIX);
					m_SportsLog.Open();
					m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " RegionDelete.cs.Delete(): " + ex.ToString());
					m_SportsLog.Close();
				}
			}

			return iSuccessUpd;
		}
	}
}