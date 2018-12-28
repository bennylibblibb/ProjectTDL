/*
Objective:
Retrieval and modify GOGO1 chart data for specific match

Last updated:
19 Feb 2004, Chapman Choi

C#.NET complier statement:
csc /t:library /out:..\bin\Gogo1ChartData.dll /r:..\bin\DBManager.dll;..\bin\Files.dll;..\bin\MessageClient.dll;..\bin\SportsMessage.dll Gogo1ChartData.cs
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
[assembly:AssemblyCopyright("(c) 2003 TDSL. All rights reserved. Created on 11 August 2003.")]
[assembly:AssemblyDescription("GOGO1 指數圖表 -> 圖表數據")]
[assembly:AssemblyProduct("Sports.NET")]
[assembly:AssemblyTitle("Sports.NET DLL")]
[assembly:AssemblyVersion("3.1.*")]
namespace SportsUtil {
	public class Gogo1ChartData {
		const string LOGFILESUFFIX = "log";
		int m_RecordCount = 0;
		OleDbDataReader m_SportsOleReader;
		DBManager m_SportsDBMgr;
		Files m_SportsLog;
		StringBuilder SQLString;

		public Gogo1ChartData(string Connection) {
			m_SportsDBMgr = new DBManager();
			m_SportsDBMgr.ConnectionString = Connection;
			m_SportsLog = new Files();
			SQLString = new StringBuilder();
		}

		public int NumberOfRecords {
			get {
				return m_RecordCount;
			}
		}

		public string ShowData() {
			double dOdds;
			string sDataTime;
			string sMatchCount;
			string sHandicap;
			StringBuilder HTMLString = new StringBuilder();

			sMatchCount = HttpContext.Current.Request.QueryString["matchcount"].Trim();
			SQLString.Remove(0,SQLString.Length);
			SQLString.Append("select LEAGUE, HOST, GUEST, IREC_NO, CTIMESTAMP, T_HANDI, T_ODDS from ODDSHISTORY where LEAGUE=(select LEAGLONG from GAMEINFO where MATCH_CNT=");
			SQLString.Append(sMatchCount);
			SQLString.Append(") AND HOST=(select HOST from GAMEINFO where MATCH_CNT=");
			SQLString.Append(sMatchCount);
			SQLString.Append(") AND GUEST=(select GUEST from GAMEINFO where MATCH_CNT=");
			SQLString.Append(sMatchCount);
			SQLString.Append(") order by CTIMESTAMP");
			try {
				m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
				while(m_SportsOleReader.Read()) {
					if(m_RecordCount == 0) {
						//League (Label and Hidden Field)
						HTMLString.Append("<tr style=\"background-color:#C0C0C0\"><th colspan=\"5\"><input type=\"hidden\" name=\"League\" value=\"");
						HTMLString.Append(m_SportsOleReader.GetString(0).Trim());
						HTMLString.Append("\">");
						HTMLString.Append(m_SportsOleReader.GetString(0).Trim());
						HTMLString.Append(" - ");

						//Host (Label and Hidden Field)
						HTMLString.Append(m_SportsOleReader.GetString(1).Trim());
						HTMLString.Append("<input type=\"hidden\" name=\"Host\" value=\"");
						HTMLString.Append(m_SportsOleReader.GetString(1).Trim());
						HTMLString.Append("\"> vs ");

						//Guest (Label and Hidden Field)
						HTMLString.Append(m_SportsOleReader.GetString(2).Trim());
						HTMLString.Append("<input type=\"hidden\" name=\"Guest\" value=\"");
						HTMLString.Append(m_SportsOleReader.GetString(2).Trim());
						HTMLString.Append("\"></th></tr>");
					}

					HTMLString.Append("<tr align=\"center\"><th>");
					HTMLString.Append((m_RecordCount+1).ToString());
					HTMLString.Append("</th>");

					//Rec number (Hidden Field)
					HTMLString.Append("<td><input type=\"hidden\" name=\"recno\" value=\"");
					HTMLString.Append(m_SportsOleReader.GetInt32(3).ToString());
					HTMLString.Append("\">");

					//Data timestamp
					sDataTime = m_SportsOleReader.GetString(4).Trim();
					HTMLString.Append(sDataTime.Substring(0,4));
					HTMLString.Append("/");
					HTMLString.Append(sDataTime.Substring(4,2));
					HTMLString.Append("/");
					HTMLString.Append(sDataTime.Substring(6,2));
					HTMLString.Append(" ");
					HTMLString.Append(sDataTime.Substring(8,2));
					HTMLString.Append(":");
					HTMLString.Append(sDataTime.Substring(10,2));
					HTMLString.Append(":");
					HTMLString.Append(sDataTime.Substring(12,2));
					HTMLString.Append("<input type=\"hidden\" name=\"timestamp\" value=\"");
					HTMLString.Append(sDataTime);
					HTMLString.Append("\"></td><td>");

					//Handicap
					if(!m_SportsOleReader.IsDBNull(5)) sHandicap = m_SportsOleReader.GetString(5).Trim();
					else sHandicap = "";
					HTMLString.Append("<input type=\"text\" name=\"Handicap\" maxlength=\"5\" size=\"2\" value=\"");
					if(!sHandicap.Trim().Equals("-1")) HTMLString.Append(sHandicap);
					HTMLString.Append("\" onChange=\"HandicapValidity(");
					HTMLString.Append(m_RecordCount.ToString());
					HTMLString.Append(")\"></td><td>");

					//Odds
					if(!m_SportsOleReader.IsDBNull(6)) {
						dOdds = m_SportsOleReader.GetFloat(6);
						dOdds = Math.Round(dOdds,3);
					} else {
						dOdds = -1;
					}
					HTMLString.Append("<input type=\"text\" name=\"Odds\" maxlength=\"5\" size=\"2\" value=\"");
					if(dOdds != -1) HTMLString.Append(dOdds.ToString());
					HTMLString.Append("\" onChange=\"OddsValidity(");
					HTMLString.Append(m_RecordCount.ToString());
					HTMLString.Append(")\"></td>");

					//Delete
					HTMLString.Append("<td><input type=\"checkbox\" name=\"deleteChk\" value=\"");
					HTMLString.Append(m_RecordCount.ToString());
					HTMLString.Append("\"></td></tr>");
					m_RecordCount++;
				}
				m_SportsDBMgr.Close();
				m_SportsOleReader.Close();
				m_SportsDBMgr.Dispose();

				if(m_RecordCount == 0) {
					HTMLString.Remove(0,HTMLString.Length);
					HTMLString.Append("<tr><th colspan=\"5\">此賽事沒有圖表數據</th></tr>");
				}
				HTMLString.Append("<input type=\"hidden\" name=\"RecordCount\" value=\"");
				HTMLString.Append(m_RecordCount.ToString());
				HTMLString.Append("\">");
				HTMLString.Append("<input type=\"hidden\" name=\"MatchCount\" value=\"");
				HTMLString.Append(sMatchCount);
				HTMLString.Append("\">");
			} catch(Exception ex) {
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Gogo1ChartData.cs.ShowData(): " + ex.ToString());
				m_SportsLog.Close();
				HTMLString.Remove(0,HTMLString.Length);
				HTMLString.Append("<tr><th colspan=\"5\">");
				HTMLString.Append((string)HttpContext.Current.Application["accessErrorMsg"]);
				HTMLString.Append("</th></tr>");
			}

			return HTMLString.ToString();
		}

		public int ModifyData() {
			int iUpdIndex = 0;
			int iDelIndex = 0;
			int iActiveIndex = 0;
			string sLeague;
			string sHost;
			string sGuest;
			char[] delimiter = new char[] {','};
			string[] arrRecNo;
			string[] arrTimestamp;
			string[] arrHandicap;
			string[] arrOdds;
			string[] arrDeletedItems;

			sLeague = HttpContext.Current.Request.Form["League"];
			sHost = HttpContext.Current.Request.Form["Host"];
			sGuest = HttpContext.Current.Request.Form["Guest"];
			arrRecNo = HttpContext.Current.Request.Form["recno"].Split(delimiter);
			arrTimestamp = HttpContext.Current.Request.Form["timestamp"].Split(delimiter);
			arrHandicap = HttpContext.Current.Request.Form["Handicap"].Split(delimiter);
			arrOdds = HttpContext.Current.Request.Form["Odds"].Split(delimiter);
			try {
				arrDeletedItems = HttpContext.Current.Request.Form["deleteChk"].Split(delimiter);
			} catch(Exception) {
				arrDeletedItems = new string[0];
			}

			try {
				for(iUpdIndex = 0; iUpdIndex < arrRecNo.Length; iUpdIndex++) {
					SQLString.Remove(0,SQLString.Length);
					SQLString.Append("update ODDSHISTORY set T_ODDS=");
					if(arrOdds[iUpdIndex].Trim().Equals("")) {
						SQLString.Append("-1");
					} else {
						SQLString.Append(arrOdds[iUpdIndex]);
					}
					SQLString.Append(", T_HANDI='");
					if(arrHandicap[iUpdIndex].Trim().Equals("")) {
						SQLString.Append("-1");
					} else {
						SQLString.Append(arrHandicap[iUpdIndex]);
					}
					SQLString.Append("' where LEAGUE='");
					SQLString.Append(sLeague);
					SQLString.Append("' AND HOST='");
					SQLString.Append(sHost);
					SQLString.Append("' AND GUEST='");
					SQLString.Append(sGuest);
					SQLString.Append("' AND CTIMESTAMP='");
					if(arrTimestamp[iUpdIndex].Trim().Equals("")) {
						SQLString.Append("-1");
					} else {
						SQLString.Append(arrTimestamp[iUpdIndex]);
					}
					SQLString.Append("' AND IREC_NO=");
					if(arrRecNo[iUpdIndex].Trim().Equals("")) {
						SQLString.Append("-1");
					} else {
						SQLString.Append(arrRecNo[iUpdIndex]);
					}
					m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
					m_SportsDBMgr.Close();
				}

				if(arrDeletedItems.Length > 0) {	//have items to delete
					for(iDelIndex = 0; iDelIndex < arrDeletedItems.Length; iDelIndex++) {
						iActiveIndex = Convert.ToInt32(arrDeletedItems[iDelIndex]);
						SQLString.Remove(0,SQLString.Length);
						SQLString.Append("delete from ODDSHISTORY where LEAGUE='");
						SQLString.Append(sLeague);
						SQLString.Append("' AND HOST='");
						SQLString.Append(sHost);
						SQLString.Append("' AND GUEST='");
						SQLString.Append(sGuest);
						SQLString.Append("' AND CTIMESTAMP ='");
						SQLString.Append(arrTimestamp[iActiveIndex]);
						SQLString.Append("' AND IREC_NO=");
						SQLString.Append(arrRecNo[iActiveIndex]);
						m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
						m_SportsDBMgr.Close();
					}
				}

				//write log
				m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Gogo1ChartData.cs: Update " + iUpdIndex.ToString() + " records and delete " + iDelIndex.ToString() + " records (" + HttpContext.Current.Session["user_name"] + ")");
				m_SportsLog.Close();
			} catch(Exception ex) {
				iUpdIndex = -1;
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Gogo1ChartData.cs.ModifyData(): " + ex.ToString());
				m_SportsLog.Close();
			}

			return iUpdIndex;
		}

		public int Resend() {
			int iUpdIndex = 0;
			string sLeague;
			string sHost;
			string sGuest;
			string sCurrentTimestamp = null;
			string sBatchJob = null;
			char[] delimiter = new char[] {','};
			string[] arrMsgType;
			string[] arrRemotingPath;
			string[] arrSendToPager;

			sLeague = HttpContext.Current.Request.Form["League"];
			sHost = HttpContext.Current.Request.Form["Host"];
			sGuest = HttpContext.Current.Request.Form["Guest"];

			try {
				arrSendToPager = HttpContext.Current.Request.Form["SendToPager"].Split(delimiter);
			} catch(Exception) {
				arrSendToPager = new string[0];
			}

			//SportsMessage object message
			SportsMessage sptMsg = new SportsMessage();
			string[] arrQueueNames;
			string[] arrMessageTypes;
			arrMsgType = (string[])HttpContext.Current.Application["messageType"];
			arrQueueNames = (string[])HttpContext.Current.Application["QueueItems"];
			arrMessageTypes = (string[])HttpContext.Current.Application["NotifyMessageTypes"];
			arrRemotingPath = (string[])HttpContext.Current.Application["RemotingItems"];
			MessageClient msgClt = new MessageClient();
			msgClt.MessageType = arrMessageTypes[0];
			msgClt.MessagePath = arrQueueNames[0];

			try {
				if(!(sLeague == null || sHost == null || sGuest == null)) {
					if(!(sLeague.Equals("") || sHost.Equals("") || sGuest.Equals(""))) {
						iUpdIndex++;
						sCurrentTimestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
						sBatchJob = DateTime.Now.ToString("yyMMddHHmmss_fffffff") + "." + HttpContext.Current.Session["user_name"].ToString() + "." + arrMsgType[28] + ".ini";
						SQLString.Remove(0,SQLString.Length);
						SQLString.Append("insert into LOG_CHARTRESEND (TIMEFLAG, SECTION, LEAGUE, HOST, GUEST, BATCHJOB) values ('");
						SQLString.Append(sCurrentTimestamp);
						SQLString.Append("','CHARTRESEND','");
						SQLString.Append(sLeague);
						SQLString.Append("','");
						SQLString.Append(sHost);
						SQLString.Append("','");
						SQLString.Append(sGuest);
						SQLString.Append("','");
						SQLString.Append(sBatchJob);
						SQLString.Append("')");
						m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
						m_SportsDBMgr.Close();
						
						
						//Modified by Chapman, 19 Feb 2004
						sptMsg.Body = sBatchJob;
						sptMsg.Timestamp = DateTime.Now.ToString("yyMMddHHmmss");
						sptMsg.AppID = "07";
						sptMsg.MsgID = "26";
						sptMsg.DeviceID = new string[0];
						for(int i = 0; i < arrSendToPager.Length; i++) {
							sptMsg.AddDeviceID((string)arrSendToPager[i]);
						}
						//Send Notify Message
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
								m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " MSMQ ERROR: Gogo1 Chart Data");
								m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Gogo1ChartData.cs.Resend(): Notify via MSMQ throws MessageQueueException:  " + mqEx.ToString());
								m_SportsLog.Close();

								//If MSMQ fail, notify via .NET Remoting
								msgClt.MessageType = arrMessageTypes[1];
								msgClt.MessagePath = arrRemotingPath[0];
								if(!msgClt.SendMessage((object)sptMsg)) {
									m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
									m_SportsLog.SetFileName(0,LOGFILESUFFIX);
									m_SportsLog.Open();
									m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Remoting ERROR: Gogo1 Chart Data");
									m_SportsLog.Close();
								}
							}	catch(Exception ex) {
								m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
								m_SportsLog.SetFileName(0,LOGFILESUFFIX);
								m_SportsLog.Open();
								m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Remoting ERROR: Gogo1 Chart Data");
								m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Gogo1ChartData.cs.Resend(): Notify via .NET Remoting throws exception: " + ex.ToString());
								m_SportsLog.Close();
							}							
						}	catch(Exception ex) {
							m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
							m_SportsLog.SetFileName(0,LOGFILESUFFIX);
							m_SportsLog.Open();
							m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Gogo1ChartData.cs.Resend(): Notify via MSMQ throws exception: " + ex.ToString());
							m_SportsLog.Close();
						}

						//write log
						m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
						m_SportsLog.SetFileName(0,LOGFILESUFFIX);
						m_SportsLog.Open();
						m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Gogo1ChartData.cs: Resend chart <L:" + sLeague + ", H:" + sHost + ", G:" + sGuest + "> (" + HttpContext.Current.Session["user_name"] + ")");
						m_SportsLog.Close();
					}
				}

				if(iUpdIndex == 0) {
					//write log
					m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
					m_SportsLog.SetFileName(0,LOGFILESUFFIX);
					m_SportsLog.Open();
					m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Gogo1ChartData.cs: No available chart to resend (" + HttpContext.Current.Session["user_name"] + ")");
					m_SportsLog.Close();
				}
			} catch(Exception ex) {
				iUpdIndex = -1;
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Gogo1ChartData.cs.Resend(): " + ex.ToString());
				m_SportsLog.Close();
			}

			return iUpdIndex;
		}
	}
}