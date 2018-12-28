/*
Objective:
Delete live odds data

Last updated:
26 Feb 2004, Chapman Choi

C#.NET complier statement:
csc /t:library /out:..\bin\LiveOddsDelete.dll /r:..\bin\DBManager.dll;..\bin\Files.dll;..\bin\MessageClient.dll;..\bin\SportsMessage.dll LiveOddsDelete.cs
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
[assembly:AssemblyDescription("現場賠率 -> 刪除賽事")]
[assembly:AssemblyProduct("Sports.NET")]
[assembly:AssemblyTitle("Sports.NET DLL")]
[assembly:AssemblyVersion("3.1.*")]
namespace SportsUtil {
	public class LiveOddsDelete {
		const string LOGFILESUFFIX = "log";
		int m_iRecordCount = 0;
		string m_Region;
		OleDbDataReader m_SportsOleReader;
		DBManager m_SportsDBMgr;
		Files m_SportsLog;
		Encoding m_Big5Encoded;
		StringBuilder SQLString;

		public LiveOddsDelete(string Connection) {
			m_SportsDBMgr = new DBManager();
			m_SportsDBMgr.ConnectionString = Connection;
			m_SportsLog = new Files();
			m_Big5Encoded = Encoding.GetEncoding(950);
			SQLString = new StringBuilder();
			m_Region = null;
		}

		public int NumberOfRecords {
			get {
				return m_iRecordCount;
			}
		}

		public string RegionName {
			get {
				return m_Region;
			}
		}

		public string GetMatches() {
			string sRtn = null;
			string sRecordStr = null;
			string sRegionID;

			try {
				sRegionID = HttpContext.Current.Request.QueryString["RegionID"];

				SQLString.Remove(0,SQLString.Length);
				SQLString.Append("select CREGION from LIVEODDS_CFG where IREGION_ID=");
				SQLString.Append(sRegionID);
				m_Region = m_SportsDBMgr.ExecuteQueryString(SQLString.ToString());
				m_SportsDBMgr.Close();

				SQLString.Remove(0,SQLString.Length);
				SQLString.Append("select IMATCH_CNT, CMATCHDATE, CMATCHTIME, CLEAGUE, CALIAS, CHOST, CGUEST, CMATCHSTATUS from LIVEODDS_INFO where CACT='U' and IREGION_ID=" + sRegionID + " order by IMATCHORDER, IMATCH_CNT");
				m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
				while(m_SportsOleReader.Read()) {
					sRtn += "<tr align=\"center\">";
					sRtn += "<td><input type=\"checkbox\" name=\"deleteItem\" value=\"" + m_iRecordCount + "\">";
					//Match Count (Hidden field)
					sRecordStr = m_SportsOleReader.GetInt32(0).ToString();
					sRtn += "<input type=\"hidden\" name=\"MatchCount\" value=\"" + sRecordStr + "\"></td>";

					//Match Date and Time (Hidden Field)
					sRecordStr = m_SportsOleReader.GetString(1);
					sRtn += "<td><input type=\"hidden\" name=\"MatchDate\" value=\"" + sRecordStr + "\">";
					sRecordStr = sRecordStr.Insert(4,"/");
					sRecordStr = sRecordStr.Insert(7,"/");
					sRtn += sRecordStr + "</td>";
					sRecordStr = m_SportsOleReader.GetString(2);
					sRtn += "<td><input type=\"hidden\" name=\"MatchTime\" value=\"" + sRecordStr + "\">";
					sRecordStr = sRecordStr.Insert(2,":");
					sRtn += sRecordStr + "</td>";

					//League, Alias(Hidden)
					sRecordStr = m_SportsOleReader.GetString(3).Trim();
					sRtn += "<td>" + sRecordStr + "<input type=\"hidden\" name=\"League\" value=\"" + sRecordStr + "\">";
					sRecordStr = m_SportsOleReader.GetString(4).Trim();
					sRtn += "<input type=\"hidden\" name=\"Alias\" value=\"" + sRecordStr + "\"></td>";

					//Host
					sRecordStr = m_SportsOleReader.GetString(5).Trim();
					sRtn += "<td>" + sRecordStr + "<input type=\"hidden\" name=\"Host\" value=\"" + sRecordStr + "\"></td>";

					//Guest
					sRecordStr = m_SportsOleReader.GetString(6).Trim();
					sRtn += "<td>" + sRecordStr + "<input type=\"hidden\" name=\"Guest\" value=\"" + sRecordStr + "\">";

					//Match Status
					sRecordStr = m_SportsOleReader.GetString(7).Trim();
					sRtn += "<input type=\"hidden\" name=\"MatchStatus\" value=\"" + sRecordStr + "\"></td></tr>";
					m_iRecordCount++;
				}
				m_SportsOleReader.Close();
				m_SportsDBMgr.Close();
				m_SportsDBMgr.Dispose();
				sRtn += "<input type=\"hidden\" name=\"RecordCount\" value=\"" + m_iRecordCount.ToString() + "\">";
				sRtn += "<input type=\"hidden\" name=\"RegionID\" value=\"" + sRegionID + "\">";
			} catch(Exception ex) {
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " LiveOddsDelete.cs.GetMatches(): " + ex.ToString());
				m_SportsLog.Close();
				sRtn = (string)HttpContext.Current.Application["accessErrorMsg"];
			}

			return sRtn;
		}

		public int Delete() {
			char[] delimiter = new char[] {','};
			int iRecIndex;
			int iDeletedItemLen;
			int iUpdIndex = 0;
			int iSuccessUpd = 0;
			string sCurrentTimestamp = null;
			string sBatchJob = null;
			string sRegionID;
			string[] arrMatchCnt;
			string[] arrSendRequired;
			string[] arrLeague;
			string[] arrAlias;
			string[] arrHost;
			string[] arrGuest;
			string[] arrMatchStatus;
			string[] arrSendToPager;
			//SportsMessage object message
			SportsMessage sptMsg = new SportsMessage();

			//declare local variables
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
			try {
				arrSendRequired = HttpContext.Current.Request.Form["deleteItem"].Split(delimiter);
				iDeletedItemLen = arrSendRequired.Length;
			} catch(Exception) {
				arrSendRequired = new string[0];
				iDeletedItemLen = 0;
			}
			arrMatchCnt = HttpContext.Current.Request.Form["MatchCount"].Split(delimiter);
			arrLeague = HttpContext.Current.Request.Form["League"].Split(delimiter);
			arrAlias = HttpContext.Current.Request.Form["Alias"].Split(delimiter);
			arrHost = HttpContext.Current.Request.Form["Host"].Split(delimiter);
			arrGuest = HttpContext.Current.Request.Form["Guest"].Split(delimiter);
			arrMatchStatus = HttpContext.Current.Request.Form["MatchStatus"].Split(delimiter);
			sRegionID = HttpContext.Current.Request.Form["RegionID"];

			if(iDeletedItemLen > 0 && arrSendToPager.Length > 0) {
				try {
					string[] arrMsgType = (string[])HttpContext.Current.Application["messageType"];
					sCurrentTimestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
					sBatchJob = DateTime.Now.ToString("yyMMddHHmmss_fffffff") + "." + HttpContext.Current.Session["user_name"].ToString() + "." + arrMsgType[20] + ".ini";
					for(iRecIndex = 0; iRecIndex < iDeletedItemLen; iRecIndex++) {
						iUpdIndex = Convert.ToInt32(arrSendRequired[iRecIndex]);
						SQLString.Remove(0,SQLString.Length);
						SQLString.Append("update LIVEODDS_INFO set CACT='D' where IMATCH_CNT=");
						SQLString.Append(arrMatchCnt[iUpdIndex]);
						SQLString.Append(" and IREGION_ID=");
						SQLString.Append(sRegionID);
						m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
						m_SportsDBMgr.Close();

						SQLString.Remove(0,SQLString.Length);
						SQLString.Append("insert into LOG_LIVEODDS (TIMEFLAG, SECTION, LEAGUEALIAS, LEAGUE, HOST, GUEST, MATCHFIELD, HANDICAP, MATCHSTATUS, ACT, H_GOAL, G_GOAL, CURRTIME, HANDI, ODDS, ODDSSTATUS, ALERT, REGIONID, HOSTWEAR, GUESTWEAR, TOTALSCORE, BIGSMALLODDS, BATCHJOB) values ('");
						SQLString.Append(sCurrentTimestamp);
						SQLString.Append("','LIVE_','");
						SQLString.Append(arrAlias[iUpdIndex]);
						SQLString.Append("','");
						SQLString.Append(arrLeague[iUpdIndex]);
						SQLString.Append("','");
						SQLString.Append(arrHost[iUpdIndex]);
						SQLString.Append("','");
						SQLString.Append(arrGuest[iUpdIndex]);
						SQLString.Append("','H','1','");
						SQLString.Append(arrMatchStatus[iUpdIndex]);
						SQLString.Append("','D',-1,-1,-1,'-1','-1','-1',0,");
						SQLString.Append(sRegionID);
						SQLString.Append(",'-1','-1','-1','-1','");
						SQLString.Append(sBatchJob);
						SQLString.Append("')");
						m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
						m_SportsDBMgr.Close();
						iSuccessUpd++;
					}

					if(iSuccessUpd > 0) {
						//Send Notify Message
						//modified by Chapman, 19 Jan 2004
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
								m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " MSMQ ERROR: Live Odds Delete");
								m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " LiveOddsDelete.cs.Delete(): Notify via MSMQ throws MessageQueueException:  " + mqEx.ToString());
								m_SportsLog.Close();

								//If MSMQ fail, notify via .NET Remoting
								msgClt.MessageType = arrMessageTypes[1];
								msgClt.MessagePath = arrRemotingPath[0];
								if(!msgClt.SendMessage((object)sptMsg)) {
									m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
									m_SportsLog.SetFileName(0,LOGFILESUFFIX);
									m_SportsLog.Open();
									m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Remoting ERROR: Live Odds Delete");
									m_SportsLog.Close();
								}
							}	catch(Exception ex) {
								m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
								m_SportsLog.SetFileName(0,LOGFILESUFFIX);
								m_SportsLog.Open();
								m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Remoting ERROR: Live Odds Delete");									
								m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " LiveOddsDelete.cs.Delete(): Notify via .NET Remoting throws exception: " + ex.ToString());
								m_SportsLog.Close();
							}							
						}	catch(Exception ex) {
							m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
							m_SportsLog.SetFileName(0,LOGFILESUFFIX);
							m_SportsLog.Open();
							m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " LiveOddsDelete.cs.Delete(): Notify via MSMQ throws exception: " + ex.ToString());
							m_SportsLog.Close();
						}
					}

					//write log
					m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
					m_SportsLog.SetFileName(0,LOGFILESUFFIX);
					m_SportsLog.Open();
					m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " LiveOddsDelete.cs: Delete " + iSuccessUpd.ToString() + " matches (" + HttpContext.Current.Session["user_name"] + ")");
					m_SportsLog.Close();
				} catch(Exception ex) {
					iSuccessUpd = -1;
					m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
					m_SportsLog.SetFileName(0,LOGFILESUFFIX);
					m_SportsLog.Open();
					m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " LiveOddsDelete.cs.Delete(): " + ex.ToString());
					m_SportsLog.Close();
				}
			}

			return iSuccessUpd;
		}
	}
}