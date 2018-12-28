/*
Objective:
Create command ini file to reorder

Last updated:
19 Feb 2004, Chapman Choi

C#.NET complier statement:
csc /t:library /out:..\bin\LiveOddsOrder.dll /r:..\bin\DBManager.dll;..\bin\Files.dll;..\bin\MessageClient.dll;..\bin\SportsMessage.dll LiveOddsOrder.cs
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
[assembly:AssemblyDescription("現場賠率 -> 修改排序")]
[assembly:AssemblyProduct("Sports.NET")]
[assembly:AssemblyTitle("Sports.NET DLL")]
[assembly:AssemblyVersion("3.1.*")]
namespace SportsUtil {
	public class LiveOddsOrder {
		const string CMDTYPE = "RESEND";
		const string LOGFILESUFFIX = "log";
		Files m_SportsLog;
		StringBuilder SQLString;
		OleDbDataReader m_SportsOleReader;
		DBManager m_SportsDBMgr;

		public LiveOddsOrder(string Connection) {
			m_SportsDBMgr = new DBManager();
			m_SportsDBMgr.ConnectionString = Connection;
			m_SportsLog = new Files();
			SQLString = new StringBuilder();
		}

		public string ShowRegion() {
			int iIdx = 0;
			StringBuilder HTMLString = new StringBuilder();

			try {
				SQLString.Remove(0,SQLString.Length);
				SQLString.Append("select IORDER, CREGION, IREGION_ID from LIVEODDS_CFG order by IORDER, IREGION_ID");
				m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
				while(m_SportsOleReader.Read()) {
					HTMLString.Append("<tr align=\"center\"><td><input name=\"order\" maxlength=\"2\" size=\"1\" value=\"");
					HTMLString.Append(m_SportsOleReader.GetInt32(0).ToString());
					HTMLString.Append("\" onChange=\"OrderChanged(");
					HTMLString.Append(iIdx.ToString());
					HTMLString.Append(")\"></td><td>");
					HTMLString.Append(m_SportsOleReader.GetString(1).Trim());
					HTMLString.Append("<input type=\"hidden\" name=\"regionID\" value=\"");
					HTMLString.Append(m_SportsOleReader.GetInt32(2).ToString());
					HTMLString.Append("\"></td></tr>");
					iIdx++;
				}
				m_SportsOleReader.Close();
				m_SportsDBMgr.Close();
				m_SportsDBMgr.Dispose();
			} catch(Exception ex) {
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " LiveOddsOrder.cs.ShowRegion(): " + ex.ToString());
				m_SportsLog.Close();
				HTMLString.Remove(0,HTMLString.Length);
				HTMLString.Append((string)HttpContext.Current.Application["accessErrorMsg"]);
			}

			return HTMLString.ToString();
		}

		public bool ReOrder() {
			bool bUpdSuccess = false;
			int iIdx = 0;
			const string CDISPLAY = "現場賠率排序";
			string sCurrentTimestamp = null;
			string sBatchJob = null;
			string sParam = null;
			char[] delimiter = new char[] {','};
			string[] arrSendToPager;
			string[] arrOrder;
			string[] arrRegionID;
			string[] arrRemotingPath;

			arrOrder = HttpContext.Current.Request.Form["order"].Split(delimiter);
			arrRegionID = HttpContext.Current.Request.Form["regionID"].Split(delimiter);
			arrRemotingPath = (string[])HttpContext.Current.Application["RemotingItems"];

			try {
				arrSendToPager = HttpContext.Current.Request.Form["SendToPager"].Split(delimiter);
			} catch(Exception) {
				arrSendToPager = new string[0];
			}
			try {
				/*****************************
				 * GoGo Pager2 alert message *
				 *****************************/
				string[] arrQueueNames;
				string[] arrMessageTypes;
				string[] arrMsgType;
				arrQueueNames = (string[])HttpContext.Current.Application["QueueItems"];
				arrMessageTypes = (string[])HttpContext.Current.Application["NotifyMessageTypes"];
				arrMsgType = (string[])HttpContext.Current.Application["messageType"];
				MessageClient msgClt = new MessageClient();
				msgClt.MessageType = arrMessageTypes[0];
				msgClt.MessagePath = arrQueueNames[0];
				//SportsMessage object message
				SportsMessage sptMsg = new SportsMessage();

				SQLString.Remove(0,SQLString.Length);
				SQLString.Append("select CPARAM from ADMINCFG where CDISPLAY='");
				SQLString.Append(CDISPLAY);
				SQLString.Append("'");
				sParam = m_SportsDBMgr.ExecuteQueryString(SQLString.ToString());
				m_SportsDBMgr.Close();

				for(iIdx = 0; iIdx < arrRegionID.Length; iIdx++) {
					SQLString.Remove(0,SQLString.Length);
					SQLString.Append("update LIVEODDS_CFG set IORDER=");
					if(arrOrder[iIdx].Trim().Equals("")) {
						SQLString.Append("null");
					} else {
						SQLString.Append(arrOrder[iIdx].Trim());
					}
					SQLString.Append(" where IREGION_ID=");
					SQLString.Append(arrRegionID[iIdx]);
					m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
					m_SportsDBMgr.Close();
				}

				sCurrentTimestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
				sBatchJob = DateTime.Now.ToString("yyMMddHHmmss_fffffff") + "." + HttpContext.Current.Session["user_name"].ToString() + "." + arrMsgType[20] + ".reorder.ini";
				SQLString.Remove(0,SQLString.Length);
				SQLString.Append("insert into LOG_ADMINTASK (TIMEFLAG, SECTION, CMD, PARAM, BATCHJOB) values ('");
				SQLString.Append(sCurrentTimestamp);
				SQLString.Append("','COMMAND_','");
				SQLString.Append(CMDTYPE);
				SQLString.Append("','");
				SQLString.Append(sParam);
				SQLString.Append("','");
				SQLString.Append(sBatchJob);
				SQLString.Append("')");
				m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
				m_SportsDBMgr.Close();
				m_SportsDBMgr.Dispose();

				//Send Notify Message
				try {
					sptMsg.IsTransaction = false;
					sptMsg.Body = sBatchJob;
					sptMsg.Timestamp = DateTime.Now.ToString("yyMMddHHmmss");
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
							m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " MSMQ ERROR:Live Odds Order");
							m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " LiveOddsOrder.cs.ReOrder(): Notify via MSMQ throws MessageQueueException:  " + mqEx.ToString());
							m_SportsLog.Close();

							//If MSMQ fail, notify via .NET Remoting
							msgClt.MessageType = arrMessageTypes[1];
							msgClt.MessagePath = arrRemotingPath[0];
							if(!msgClt.SendMessage((object)sptMsg)) {
								m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
								m_SportsLog.SetFileName(0,LOGFILESUFFIX);
								m_SportsLog.Open();
								m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Remoting ERROR:Live Odds Order");
								m_SportsLog.Close();
							}
						}	catch(Exception ex) {
							m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
							m_SportsLog.SetFileName(0,LOGFILESUFFIX);
							m_SportsLog.Open();
							m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Remoting ERROR:Live Odds Order");
							m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " LiveOddsOrder.cs.ReOrder(): Notify via .NET Remoting throws exception: " + ex.ToString());
							m_SportsLog.Close();
						}							
					}	catch(Exception ex) {
						m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
						m_SportsLog.SetFileName(0,LOGFILESUFFIX);
						m_SportsLog.Open();
						m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " LiveOddsOrder.cs.ReOrder(): Notify via MSMQ throws exception: " + ex.ToString());
						m_SportsLog.Close();
					}
				} catch(Exception ex) {
					m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
					m_SportsLog.SetFileName(0,LOGFILESUFFIX);
					m_SportsLog.Open();
					m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " LiveOddsOrder.cs.ReOrder(): Send reorder notify message throws exception: " + ex.ToString());
					m_SportsLog.Close();
				}

				//write log
				m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " LiveOddsOrder.cs: Region re-ordered (" + HttpContext.Current.Session["user_name"] + ")");
				m_SportsLog.Close();
				bUpdSuccess = true;
			} catch(Exception ex) {
				bUpdSuccess = false;
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " LiveOddsOrder.cs.ReOrder(): " + ex.ToString());
				m_SportsLog.Close();
			}

			return bUpdSuccess;
		}
	}
}