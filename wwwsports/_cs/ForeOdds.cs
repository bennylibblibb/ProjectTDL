/*
Objective:
Retrieval and modify foreodds only, not send to pager

Last updated:
19 Feb 2004, Chapman Choi

C#.NET complier statement:
csc /t:library /out:..\bin\ForeOdds.dll /r:..\bin\DBManager.dll;..\bin\Files.dll;..\bin\MessageClient.dll;..\bin\SportsMessage.dll ForeOdds.cs
*/

using System;
using System.Data.OleDb;
using System.Text;
using System.Reflection;
using System.Web;
using TDL.DB;
using TDL.IO;
using TDL.Message;

[assembly:AssemblyCompany("TDSL")]
[assembly:AssemblyCopyright("(c) 2003 TDSL. All rights reserved. Created on 4 Nov 2003.")]
[assembly:AssemblyDescription("足球資訊 -> 預報指數")]
[assembly:AssemblyProduct("Sports.NET")]
[assembly:AssemblyTitle("Sports.NET DLL")]
[assembly:AssemblyVersion("3.1.*")]
namespace SportsUtil {
	public class ForeOdds {
		const string LOGFILESUFFIX = "log";
		int m_RecordCount = 0;
		OleDbDataReader m_SportsOleReader;
		DBManager m_SportsDBMgr;
		Files m_SportsLog;
		Encoding m_Big5Encoded;
		StringBuilder SQLString;

		public ForeOdds(string Connection) {
			m_SportsDBMgr = new DBManager();
			m_SportsDBMgr.ConnectionString = Connection;
			m_SportsLog = new Files();
			m_Big5Encoded = Encoding.GetEncoding(950);
			SQLString = new StringBuilder();
		}

		public int NumberOfRecords {
			get {
				return m_RecordCount;
			}
		}

		public string GetOdds() {
			string sMatchDate;
			string sMatchTime;
			string sScoreHandicap;
			StringBuilder HTMLString = new StringBuilder();

			SQLString.Append("select fore.IREC_NO, fore.IMATCH_DATE, fore.IMATCH_TIME, fore.CLEAGUEALIAS, fore.CHOST, fore.CGUEST, fore.CFSTHANDI, fore.CFSTSCOREHANDI, fore.CFSTODDSVALUE, fore.IFSTDATE, fore.CSECHANDI, fore.CSECSCOREHANDI, fore.CSECODDSVALUE, fore.ISECDATE from FOREODDS_DETAILS fore, leaginfo leag where fore.CLEAGUEALIAS = leag.ALIAS and leag.leag_id in (select cleag_id from userprofile_info where iuser_id=");
			SQLString.Append(HttpContext.Current.Session["user_id"].ToString());
			SQLString.Append(") order by fore.IHEADER_ID");
			try {
				m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
				while(m_SportsOleReader.Read()) {
					//IREC_NO (Hidden field: Primary Key)
					HTMLString.Append("<tr align=\"center\"><td><input type=\"hidden\" name=\"RecNo\" value=\"");
					HTMLString.Append(m_SportsOleReader.GetInt32(0));
					HTMLString.Append("\">");

					//Match Date and Time
					sMatchDate = m_SportsOleReader.GetInt32(1).ToString("D8");
					sMatchDate = sMatchDate.Insert(4,"/");
					sMatchDate = sMatchDate.Insert(7,"/");
					sMatchTime = m_SportsOleReader.GetInt32(2).ToString("D4");
					sMatchTime = sMatchTime.Insert(2,":");
					HTMLString.Append(sMatchDate);
					HTMLString.Append(" ");
					HTMLString.Append(sMatchTime);
					HTMLString.Append("</td><td>");

					//Alias
					HTMLString.Append(m_SportsOleReader.GetString(3).Trim());
					HTMLString.Append("<input type=\"hidden\" name=\"Alias\" value=\"");
					HTMLString.Append(m_SportsOleReader.GetString(3).Trim());
					HTMLString.Append("\"></td><td>");

					//Host
					HTMLString.Append(m_SportsOleReader.GetString(4).Trim());
					HTMLString.Append("<input type=\"hidden\" name=\"Host\" value=\"");
					HTMLString.Append(m_SportsOleReader.GetString(4).Trim());
					HTMLString.Append("\"></td><td>");

					//Guest
					HTMLString.Append(m_SportsOleReader.GetString(5).Trim());
					HTMLString.Append("<input type=\"hidden\" name=\"Guest\" value=\"");
					HTMLString.Append(m_SportsOleReader.GetString(5).Trim());
					HTMLString.Append("\"></td><td style=\"background-color:#F5F5DC\">");
/*
					//Match Field
					HTMLString.Append("<input type=\"checkbox\" name=\"MatchField\" ");
					if(m_SportsOleReader.GetString(6).Equals("M")) HTMLString.Append("checked");
					HTMLString.Append(" onClick=\"onChecked(");
					HTMLString.Append(m_RecordCount.ToString());
					HTMLString.Append(")\" value=\"");
					HTMLString.Append(m_RecordCount.ToString());
					HTMLString.Append("\"></td><td>");
*/
					//First Host Handicap
					HTMLString.Append("<input type=\"checkbox\" name=\"HostHandicap\" ");
					if(m_SportsOleReader.GetString(6).Equals("1")) HTMLString.Append("checked");
					HTMLString.Append(" onClick=\"onChecked(");
					HTMLString.Append(m_RecordCount.ToString());
					HTMLString.Append(")\" value=\"");
					HTMLString.Append(m_RecordCount.ToString());
					HTMLString.Append("\"></td><td style=\"background-color:#F5F5DC\">");

					//First Score Handicap1/Handicap2
					if(!m_SportsOleReader.IsDBNull(7)) {
						sScoreHandicap = m_SportsOleReader.GetString(7).Trim();
					} else {
						sScoreHandicap = "";
					}
					HTMLString.Append("<input type=\"text\" name=\"ScoreHandicap1\" maxlength=\"3\" size=\"1\" value=\"");
					if(!sScoreHandicap.Equals("")) {
						if(!sScoreHandicap.Equals("-1")) {
							if(sScoreHandicap.IndexOf("/") != -1) {
								HTMLString.Append(sScoreHandicap.Substring(0,sScoreHandicap.IndexOf("/")));
							} else {
								HTMLString.Append(sScoreHandicap);
							}
						}
					}
					HTMLString.Append("\" onChange=\"ScoreHandicap1Validity(");
					HTMLString.Append(m_RecordCount.ToString());
					HTMLString.Append(")\">/<input type=\"text\" name=\"ScoreHandicap2\" maxlength=\"3\" size=\"1\" value=\"");
					if(!sScoreHandicap.Equals("")) {
						if(!sScoreHandicap.Equals("-1")) {
							if(sScoreHandicap.IndexOf("/") != -1) {
								HTMLString.Append(sScoreHandicap.Substring((sScoreHandicap.IndexOf("/") + 1)));
							}
						}
					}
					HTMLString.Append("\" onChange=\"ScoreHandicap2Validity(");
					HTMLString.Append(m_RecordCount.ToString());
					HTMLString.Append(")\"></td><td style=\"background-color:#F5F5DC\">");

					//First Odds
					HTMLString.Append("<input type=\"text\" name=\"Odds\" maxlength=\"5\" size=\"2\" value=\"");
					if(!m_SportsOleReader.IsDBNull(8)) {
						if(!m_SportsOleReader.GetString(8).Trim().Equals("")) {
							if(!m_SportsOleReader.GetString(8).Trim().Equals("-1")) {
								HTMLString.Append(m_SportsOleReader.GetString(8).Trim());
							}
						}
					}
					HTMLString.Append("\" onChange=\"OddsValidity(");
					HTMLString.Append(m_RecordCount.ToString());
					HTMLString.Append(")\"></td><td style=\"background-color:#F5F5DC\">");

					//Fore Odds Init Date
					HTMLString.Append("<input type=\"text\" name=\"InitDate\" maxlength=\"8\" size=\"5\" value=\"");
					if(!m_SportsOleReader.IsDBNull(9)) {
						if(m_SportsOleReader.GetInt32(9) != -1) {
							HTMLString.Append(m_SportsOleReader.GetInt32(9).ToString());
						}
					}
					HTMLString.Append("\" onChange=\"onChecked(");
					HTMLString.Append(m_RecordCount.ToString());
					HTMLString.Append(")\"></td><td style=\"background-color:#B0C4DE\">");

					//Second Host Handicap
					HTMLString.Append("<input type=\"checkbox\" name=\"LastHostHandicap\" ");
					if(m_SportsOleReader.GetString(10).Equals("1")) HTMLString.Append("checked");
					HTMLString.Append(" onClick=\"onChecked(");
					HTMLString.Append(m_RecordCount.ToString());
					HTMLString.Append(")\" value=\"");
					HTMLString.Append(m_RecordCount.ToString());
					HTMLString.Append("\"></td><td style=\"background-color:#B0C4DE\">");

					//Second Score Handicap1/Handicap2
					if(!m_SportsOleReader.IsDBNull(11)) {
						sScoreHandicap = m_SportsOleReader.GetString(11).Trim();
					} else {
						sScoreHandicap = "";
					}
					HTMLString.Append("<input type=\"text\" name=\"LastScoreHandicap1\" maxlength=\"3\" size=\"1\" value=\"");
					if(!sScoreHandicap.Equals("")) {
						if(!sScoreHandicap.Equals("-1")) {
							if(sScoreHandicap.IndexOf("/") != -1) {
								HTMLString.Append(sScoreHandicap.Substring(0,sScoreHandicap.IndexOf("/")));
							} else {
								HTMLString.Append(sScoreHandicap);
							}
						}
					}
					HTMLString.Append("\" onChange=\"LastScoreHandicap1Validity(");
					HTMLString.Append(m_RecordCount.ToString());
					HTMLString.Append(")\">/<input type=\"text\" name=\"LastScoreHandicap2\" maxlength=\"3\" size=\"1\" value=\"");
					if(!sScoreHandicap.Equals("")) {
						if(!sScoreHandicap.Equals("-1")) {
							if(sScoreHandicap.IndexOf("/") != -1) {
								HTMLString.Append(sScoreHandicap.Substring((sScoreHandicap.IndexOf("/") + 1)));
							}
						}
					}
					HTMLString.Append("\" onChange=\"LastScoreHandicap2Validity(");
					HTMLString.Append(m_RecordCount.ToString());
					HTMLString.Append(")\"></td><td style=\"background-color:#B0C4DE\">");

					//Second Odds
					HTMLString.Append("<input type=\"text\" name=\"LastOdds\" maxlength=\"5\" size=\"2\" value=\"");
					if(!m_SportsOleReader.IsDBNull(12)) {
						if(!m_SportsOleReader.GetString(12).Trim().Equals("")) {
							if(!m_SportsOleReader.GetString(12).Trim().Equals("-1")) {
								HTMLString.Append(m_SportsOleReader.GetString(12).Trim());
							}
						}
					}
					HTMLString.Append("\" onChange=\"LastOddsValidity(");
					HTMLString.Append(m_RecordCount.ToString());
					HTMLString.Append(")\"></td><td style=\"background-color:#B0C4DE\">");

					//Fore Odds 2nd Date
					HTMLString.Append("<input type=\"text\" name=\"LastDate\" maxlength=\"8\" size=\"5\" value=\"");
					if(!m_SportsOleReader.IsDBNull(13)) {
						if(m_SportsOleReader.GetInt32(13) != -1) {
							HTMLString.Append(m_SportsOleReader.GetInt32(13).ToString());
						}
					}
					HTMLString.Append("\" onChange=\"onChecked(");
					HTMLString.Append(m_RecordCount.ToString());
					HTMLString.Append(")\"></td><td>");

					//MUST Send/Save
					HTMLString.Append("<input type=\"checkbox\" name=\"MUSTSendChk\" value=\"");
					HTMLString.Append(m_RecordCount.ToString());
					HTMLString.Append("\"></td></tr>");

					m_RecordCount++;
				}
				m_SportsOleReader.Close();
				m_SportsDBMgr.Close();
				m_SportsDBMgr.Dispose();

				if(m_RecordCount == 0) {
					HTMLString.Append("<tr><th colspan=\"10\">沒有預報指數</th></tr>");
				}
				HTMLString.Append("<input type=\"hidden\" name=\"RecordCount\" value=\"" + m_RecordCount.ToString() + "\">");
			} catch(Exception ex) {
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " ForeOdds.cs.GetOdds(): " + ex.ToString());
				m_SportsLog.Close();
				HTMLString.Remove(0,HTMLString.Length);
				HTMLString.Append((string)HttpContext.Current.Application["accessErrorMsg"]);
			}

			return HTMLString.ToString();
		}

		public int UpdateForeOdds(string sType) {
			int iUpdIndex = 0;
			int iRecUpd = 0;
			int iMustSendLen;
			string sForeOddsTimestamp;
			string sCurrentTimestamp = null;
			string sBatchJob = null;
			char[] delimiter = new char[] {','};
			string[] arrMustSend;
			string[] arrSendToPager;
			string[] arrRemotingPath;
			arrRemotingPath = (string[])HttpContext.Current.Application["RemotingItems"];

			try {
				arrMustSend = HttpContext.Current.Request.Form["MUSTSendChk"].Split(delimiter);
				iMustSendLen = arrMustSend.Length;
			}	catch(Exception) {
				arrMustSend = new string[0];
				iMustSendLen = 0;
			}

			try {
				arrSendToPager = HttpContext.Current.Request.Form["SendToPager"].Split(delimiter);
			} catch(Exception) {
				arrSendToPager = new string[0];
			}

			try {
				switch(sType) {
					case "SEND":	//case: Update ForeOdds
						string sHostHandicap;
						string sHandicap1;
						string sHandicap2;
						string sOdds;
						string sInitDate;
						string s2ndHostHandicap;
						string s2ndHandicap1;
						string s2ndHandicap2;
						string s2ndOdds;
						string s2ndDate;
						string[] arrRecordNo;
						string[] arrHostHandicap;
						string[] arrHandicap1;
						string[] arrHandicap2;
						string[] arrOdds;
						string[] arrInitDate;
						string[] arr2ndHostHandicap;
						string[] arr2ndHandicap1;
						string[] arr2ndHandicap2;
						string[] arr2ndOdds;
						string[] arr2ndDate;

						arrRecordNo = HttpContext.Current.Request.Form["RecNo"].Split(delimiter);
						arrHandicap1 = HttpContext.Current.Request.Form["ScoreHandicap1"].Split(delimiter);
						arrHandicap2 = HttpContext.Current.Request.Form["ScoreHandicap2"].Split(delimiter);
						arrOdds = HttpContext.Current.Request.Form["Odds"].Split(delimiter);
						arrInitDate = HttpContext.Current.Request.Form["InitDate"].Split(delimiter);
						arr2ndHandicap1 = HttpContext.Current.Request.Form["LastScoreHandicap1"].Split(delimiter);
						arr2ndHandicap2 = HttpContext.Current.Request.Form["LastScoreHandicap2"].Split(delimiter);
						arr2ndOdds = HttpContext.Current.Request.Form["LastOdds"].Split(delimiter);
						arr2ndDate = HttpContext.Current.Request.Form["LastDate"].Split(delimiter);

						try {
							arrHostHandicap = HttpContext.Current.Request.Form["HostHandicap"].Split(delimiter);
						}	catch(Exception) {
							arrHostHandicap = new string[0];
						}
						try {
							arr2ndHostHandicap = HttpContext.Current.Request.Form["LastHostHandicap"].Split(delimiter);
						}	catch(Exception) {
							arr2ndHostHandicap = new string[0];
						}

						//SportsMessage object message
						string[] arrMsgType;
						string[] arrQueueNames;
						string[] arrMessageTypes;
						arrMsgType = (string[])HttpContext.Current.Application["messageType"];
						arrQueueNames = (string[])HttpContext.Current.Application["QueueItems"];
						arrMessageTypes = (string[])HttpContext.Current.Application["NotifyMessageTypes"];
						MessageClient msgClt = new MessageClient();
						msgClt.MessageType = arrMessageTypes[0];
						msgClt.MessagePath = arrQueueNames[0];
						SportsMessage sptMsg = new SportsMessage();

						sForeOddsTimestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
						for(iRecUpd = 0; iRecUpd < iMustSendLen; iRecUpd++) {
							//update the checked(MUSTSend) records only
							iUpdIndex = Convert.ToInt32(arrMustSend[iRecUpd]);

							sHandicap1 = arrHandicap1[iUpdIndex];
							if(sHandicap1 == null) sHandicap1 = "-1";
							else sHandicap1 = sHandicap1.Trim();
							if(sHandicap1.Equals("")) sHandicap1 = "-1";

							sHandicap2 = arrHandicap2[iUpdIndex];
							if(sHandicap2 == null) sHandicap2 = "-1";
							else sHandicap2 = sHandicap2.Trim();
							if(sHandicap2.Equals("")) sHandicap2 = "-1";

							sOdds = arrOdds[iUpdIndex];
							if(sOdds == null) sOdds = "-1";
							else sOdds = sOdds.Trim();
							if(sOdds.Equals("")) sOdds = "-1";

							sInitDate = arrInitDate[iUpdIndex];
							if(sInitDate == null) sInitDate = "-1";
							else sInitDate = sInitDate.Trim();
							if(sInitDate.Equals("")) sInitDate = "-1";

							s2ndHandicap1 = arr2ndHandicap1[iUpdIndex];
							if(s2ndHandicap1 == null) s2ndHandicap1 = "-1";
							else s2ndHandicap1 = s2ndHandicap1.Trim();
							if(s2ndHandicap1.Equals("")) s2ndHandicap1 = "-1";

							s2ndHandicap2 = arr2ndHandicap2[iUpdIndex];
							if(s2ndHandicap2 == null) s2ndHandicap2 = "-1";
							else s2ndHandicap2 = s2ndHandicap2.Trim();
							if(s2ndHandicap2.Equals("")) s2ndHandicap2 = "-1";

							s2ndOdds = arr2ndOdds[iUpdIndex];
							if(s2ndOdds == null) s2ndOdds = "-1";
							else s2ndOdds = s2ndOdds.Trim();
							if(s2ndOdds.Equals("")) s2ndOdds = "-1";

							s2ndDate = arr2ndDate[iUpdIndex];
							if(s2ndDate == null) s2ndDate = "-1";
							else s2ndDate = s2ndDate.Trim();
							if(s2ndDate.Equals("")) s2ndDate = "-1";

							sHostHandicap = "";
							for(int iHandicapUpd = 0; iHandicapUpd < arrHostHandicap.Length; iHandicapUpd++) {
								if(arrHostHandicap[iHandicapUpd] == iUpdIndex.ToString()) {
									sHostHandicap = "1";
									break;
								}
							}
							if(!sHostHandicap.Equals("1")) sHostHandicap = "0";

							s2ndHostHandicap = "";
							for(int i2ndHandicapUpd = 0; i2ndHandicapUpd < arr2ndHostHandicap.Length; i2ndHandicapUpd++) {
								if(arr2ndHostHandicap[i2ndHandicapUpd] == iUpdIndex.ToString()) {
									s2ndHostHandicap = "1";
									break;
								}
							}
							if(!s2ndHostHandicap.Equals("1")) s2ndHostHandicap = "0";

							//update FOREODDS_DETAILS
							SQLString.Remove(0,SQLString.Length);
							SQLString.Append("update FOREODDS_DETAILS set CFSTHANDI='");
							SQLString.Append(sHostHandicap);
							SQLString.Append("', CFSTSCOREHANDI='");
							if(sHandicap1.Equals("-1")) {
								SQLString.Append(sHandicap1);
							} else {
								if(!sHandicap2.Equals("-1")) {
									SQLString.Append(sHandicap1);
									SQLString.Append("/");
									SQLString.Append(sHandicap2);
								} else {
									SQLString.Append(sHandicap1);
								}
							}
							SQLString.Append("', CFSTODDSVALUE='");
							SQLString.Append(sOdds);
							SQLString.Append("', IFSTDATE=");
							SQLString.Append(sInitDate);
							SQLString.Append(", CSECHANDI='");
							SQLString.Append(s2ndHostHandicap);
							SQLString.Append("', CSECSCOREHANDI='");
							if(s2ndHandicap1.Equals("-1")) {
								SQLString.Append(s2ndHandicap1);
							} else {
								if(!s2ndHandicap2.Equals("-1")) {
									SQLString.Append(s2ndHandicap1);
									SQLString.Append("/");
									SQLString.Append(s2ndHandicap2);
								} else {
									SQLString.Append(s2ndHandicap1);
								}
							}
							SQLString.Append("', CSECODDSVALUE='");
							SQLString.Append(s2ndOdds);
							SQLString.Append("', ISECDATE=");
							SQLString.Append(s2ndDate);
							SQLString.Append(", CTIMESTAMP='");
							SQLString.Append(sForeOddsTimestamp);
							SQLString.Append("' where IREC_NO=");
							SQLString.Append(arrRecordNo[iUpdIndex]);
							m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
							m_SportsDBMgr.Close();
						}

						if(iRecUpd > 0) {
							sCurrentTimestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
							sBatchJob = DateTime.Now.ToString("yyMMddHHmmss_fffffff") + "." + HttpContext.Current.Session["user_name"].ToString() + "." + arrMsgType[34] + ".ini";
							SQLString.Remove(0,SQLString.Length);
							SQLString.Append("insert into LOG_ADMINTASK (TIMEFLAG, SECTION, CMD, PARAM, BATCHJOB) values ('");
							SQLString.Append(sCurrentTimestamp);
							SQLString.Append("','COMMAND_','RESEND_FOREODDS','");
							SQLString.Append(sForeOddsTimestamp);
							SQLString.Append("','");
							SQLString.Append(sBatchJob);
							SQLString.Append("')");
							m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
							m_SportsDBMgr.Close();
							m_SportsDBMgr.Dispose();

							//Send Notify Message indicating change of foreodds
							//Modified by Chapman, 19 Feb 2004
							sptMsg.Body = sBatchJob;
							sptMsg.Timestamp = DateTime.Now.ToString("yyMMddHHmmss");
							sptMsg.IsTransaction = false;
							sptMsg.AppID = "07";
							sptMsg.MsgID = "23";
							sptMsg.PathID = new string[0];
							for(int i = 0; i < arrSendToPager.Length; i++) {
								sptMsg.AddPathID((string)arrSendToPager[i]);
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
									m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " MSMQ ERROR: Fore Odds");
									m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " ForeOdds.cs.UpdateForeOdds(): Notify via MSMQ throws MessageQueueException:  " + mqEx.ToString());
									m_SportsLog.Close();

									//If MSMQ fail, notify via .NET Remoting
									msgClt.MessageType = arrMessageTypes[1];
									msgClt.MessagePath = arrRemotingPath[0];
									if(!msgClt.SendMessage((object)sptMsg)) {
										m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
										m_SportsLog.SetFileName(0,LOGFILESUFFIX);
										m_SportsLog.Open();
										m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Remoting ERROR: Fore Odds");
										m_SportsLog.Close();
									}
								}	catch(Exception ex) {
									m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
									m_SportsLog.SetFileName(0,LOGFILESUFFIX);
									m_SportsLog.Open();
									m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Remoting ERROR: Fore Odds");
									m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " ForeOdds.cs.UpdateForeOdds(): Notify via .NET Remoting throws exception: " + ex.ToString());
									m_SportsLog.Close();
								}							
							}	catch(Exception ex) {
								m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
								m_SportsLog.SetFileName(0,LOGFILESUFFIX);
								m_SportsLog.Open();
								m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " ForeOdds.cs.UpdateForeOdds(): Notify via MSMQ throws exception: " + ex.ToString());
								m_SportsLog.Close();
							}	
						}

						//write log
						m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
						m_SportsLog.SetFileName(0,LOGFILESUFFIX);
						m_SportsLog.Open();
						m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " ForeOdds.cs: Update " + iRecUpd.ToString() + " foreodds (" + HttpContext.Current.Session["user_name"] + ")");
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
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " ForeOdds.cs.UpdateForeOdds(): " + ex.ToString());
				m_SportsLog.Close();
			}

			return iRecUpd;
		}
	}
}