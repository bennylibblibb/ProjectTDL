/*
Objective:
Send instant alert message who enabled soccer1 message alert in pager

Last updated:
19 Feb 2004, Chapman Choi

C#.NET complier statement:
csc /t:library /out:..\bin\AlertMessage.dll /r:..\bin\DBManager.dll;..\bin\Files.dll;..\bin\MessageClient.dll;..\bin\SportsMessage.dll AlertMessage.cs
*/

using System;
using System.Reflection;
using System.Text;
using System.Web;
using TDL.DB;
using TDL.IO;
using TDL.Message;

[assembly:AssemblyCompany("TDSL")]
[assembly:AssemblyCopyright("(c) 2003 TDSL. All rights reserved.")]
[assembly:AssemblyDescription("¯S©w°T®§")]
[assembly:AssemblyProduct("Sports.NET")]
[assembly:AssemblyTitle("Sports.NET DLL")]
[assembly:AssemblyVersion("3.1.*")]
namespace SportsUtil {
	public class AlertMessage {
		const string DBCR = "(CR)";
		const string PAGECR = "\r\n";
		const string LOGFILESUFFIX = "log";
		DBManager m_SportsDBMgr;
		Files m_SportsLog;
		Encoding m_Big5Encoded;

		public AlertMessage(string Connection) {
			m_SportsDBMgr = new DBManager();
			m_SportsDBMgr.ConnectionString = Connection;
			m_SportsLog = new Files();
			m_Big5Encoded = Encoding.GetEncoding(950);
		}

		public int Send() {
			char[] delimiter = new char[] {','};
			int iRecUpd = 0;
			string sAlertMsg;
			string sSQL;
			string sBatchJob = null;
			string[] arrSendToPager;
			string[] arrRemotingPath;
			
			//SportsMessage object message
			SportsMessage sptMsg = new SportsMessage();

			/*****************************
			 * GoGo Pager2 alert message *
			 *****************************/
			string[] arrQueueNames;
			string[] arrMessageTypes;
			arrQueueNames = (string[])HttpContext.Current.Application["QueueItems"];
			arrMessageTypes = (string[])HttpContext.Current.Application["NotifyMessageTypes"];
			arrRemotingPath = (string[])HttpContext.Current.Application["RemotingItems"];
			MessageClient msgClt = new MessageClient();
			msgClt.MessageType = arrMessageTypes[0];
			msgClt.MessagePath = arrQueueNames[0];

			sAlertMsg = HttpContext.Current.Request.Form["alertMsg"];
			try {
				arrSendToPager = HttpContext.Current.Request.Form["SendToPager"].Split(delimiter);
			} catch(Exception) {
				arrSendToPager = new string[0];
			}

			try {
				if(sAlertMsg != null) {
					if(!sAlertMsg.Trim().Equals("")) {
						if(arrSendToPager.Length > 0) {
							string[] arrMsgType;
							arrMsgType = (string[])HttpContext.Current.Application["messageType"];
							sAlertMsg = sAlertMsg.Trim().Replace(PAGECR,DBCR);
							sBatchJob = DateTime.Now.ToString("yyMMddHHmmss_fffffff") + "." + HttpContext.Current.Session["user_name"].ToString() + "." + arrMsgType[21] + ".ini";

							sSQL = "insert into LOG_ALERTMSG (TIMEFLAG, SECTION, MSG_1, BATCHJOB) values (CURRENT_TIMESTAMP, 'FREEMSG', '";
							sSQL += sAlertMsg;
							sSQL += "','";
							sSQL += sBatchJob;
							sSQL += "')";
							m_SportsDBMgr.ExecuteNonQuery(sSQL);
							m_SportsDBMgr.Close();
							m_SportsDBMgr.Dispose();											

							//Modified by Chapman, 19 Feb 2004
							sptMsg.IsTransaction = false;
							sptMsg.Body = sBatchJob;
							sptMsg.Timestamp = DateTime.Now.ToString("yyMMddHHmmss");
							sptMsg.AppID = "07";
							sptMsg.MsgID = "24";
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
									m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " MSMQ ERROR: Alert Message");
									m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " AlertMessage.cs.Send(): Notify via MSMQ throws MessageQueueException:  " + mqEx.ToString());
									m_SportsLog.Close();

									//If MSMQ fail, notify via .NET Remoting
									msgClt.MessageType = arrMessageTypes[1];
									msgClt.MessagePath = arrRemotingPath[0];
									if(!msgClt.SendMessage((object)sptMsg)) {
										m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
										m_SportsLog.SetFileName(0,LOGFILESUFFIX);
										m_SportsLog.Open();
										m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Remoting ERROR: Alert Message");
										m_SportsLog.Close();
									}
								}	catch(Exception ex) {
									m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
									m_SportsLog.SetFileName(0,LOGFILESUFFIX);
									m_SportsLog.Open();
									m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Remoting ERROR: Alert Message");
									m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " AlertMessage.cs.Send(): Notify via .NET Remoting throws exception: " + ex.ToString());
									m_SportsLog.Close();
								}							
							}	catch(Exception ex) {
								m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
								m_SportsLog.SetFileName(0,LOGFILESUFFIX);
								m_SportsLog.Open();
								m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " AlertMessage.cs.Send(): Notify via MSMQ throws exception: " + ex.ToString());
								m_SportsLog.Close();
							}
							iRecUpd++;
						} else {
							//write log
							m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
							m_SportsLog.SetFileName(0,LOGFILESUFFIX);
							m_SportsLog.Open();
							m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " AlertMessage.cs: No destination was selected, cannot be sent (" + HttpContext.Current.Session["user_name"] + ")");
							m_SportsLog.Close();
						}
					} else {
						//write log
						m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
						m_SportsLog.SetFileName(0,LOGFILESUFFIX);
						m_SportsLog.Open();
						m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " AlertMessage.cs: Trimed message is empty, cannot be sent (" + HttpContext.Current.Session["user_name"] + ")");
						m_SportsLog.Close();
					}
				} else {
					//write log
					m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
					m_SportsLog.SetFileName(0,LOGFILESUFFIX);
					m_SportsLog.Open();
					m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " AlertMessage.cs: Message is null, cannot be sent (" + HttpContext.Current.Session["user_name"] + ")");
					m_SportsLog.Close();
				}
			} catch(Exception ex) {
				iRecUpd = -1;
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " AlertMessage.cs.Send(): " + ex.ToString());
				m_SportsLog.Close();
			}

			return iRecUpd;
		}
	}
}