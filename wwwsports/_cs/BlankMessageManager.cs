/*
Objective:
Set or send the message to replace the word [沒有訊息]

Last updated:
12 Feb 2004 by HenryCi

C#.NET complier statement:
csc /t:library /out:..\bin\BlankMessageManager.dll /r:..\bin\DBManager.dll;..\bin\Files.dll;..\bin\MessageClient.dll;..\bin\SportsMessage.dll BlankMessageManager.cs
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
[assembly:AssemblyCopyright("(c) 2003 TDSL. All rights reserved. Created on 19 June 2003.")]
[assembly:AssemblyDescription("一般設定 -> 取代[沒有訊息]")]
[assembly:AssemblyProduct("Sports.NET")]
[assembly:AssemblyTitle("Sports.NET DLL")]
[assembly:AssemblyVersion("3.1.*")]
namespace SportsUtil {
	public class BlankMessageManager {
		const string LOGFILESUFFIX = "log";
		const string STANDARDMESSAGE = "沒有訊息";
		OleDbDataReader m_SportsOleReader;
		DBManager m_SportsDBMgr;
		Files m_SportsLog;
		StringBuilder SQLString;

		public BlankMessageManager(string Connection) {
			m_SportsDBMgr = new DBManager();
			m_SportsDBMgr.ConnectionString = Connection;
			m_SportsLog = new Files();
			SQLString = new StringBuilder();
		}

		public string GetMessage() {
			string sRtn = "";

			try {
				m_SportsOleReader = m_SportsDBMgr.ExecuteQuery("select MSGID, MSGTYPE, MSGCONTENT from SCHEDULEINFO order by MSGID");
				while(m_SportsOleReader.Read()) {
					//Hidden field: message ID
					sRtn += "<tr align=\"center\"><th><input type=\"hidden\" name=\"msgID\" value=\"";
					sRtn += m_SportsOleReader.GetInt32(0) + "\">";

					//message type
					sRtn += m_SportsOleReader.GetString(1).Trim() + "<input type=\"hidden\" name=\"msgType\" value=\"" + m_SportsOleReader.GetString(1).Trim() + "\"></th>";

					//message content
					sRtn += "<td><input type=\"text\" name=\"msgContent\" value=\"";
					if(m_SportsOleReader.IsDBNull(2)) sRtn += STANDARDMESSAGE;
					else sRtn += m_SportsOleReader.GetString(2).Trim();
					sRtn += "\" length=\"30\" maxlength=\"30\"></td></tr>";
				}
				m_SportsOleReader.Close();
				m_SportsDBMgr.Close();
				m_SportsDBMgr.Dispose();
			} catch(Exception ex) {
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " BlankMessageManager.cs.GetMessage(): " + ex.ToString());
				m_SportsLog.Close();
				sRtn = (string)HttpContext.Current.Application["accessErrorMsg"];
			}

			return sRtn;
		}

		public int SetMessage() {
			int iUpdIdx = 0;
			string sMsg = "";
			string sBatchJob = null;
			string sCurrentTimestamp = null;
			string[] arrBlankMsgContent;
			string[] arrBlankMsgID;
			string[] arrBlankMsgType;
			string[] arrRemotingPath;
			string[] arrSendToPager;
			char[] delimiter = new char[] {','};

			arrBlankMsgID = HttpContext.Current.Request.Form["msgID"].Split(delimiter);
			arrBlankMsgContent = HttpContext.Current.Request.Form["msgContent"].Split(delimiter);
			arrBlankMsgType = HttpContext.Current.Request.Form["msgType"].Split(delimiter);
			arrRemotingPath = (string[])HttpContext.Current.Application["RemotingItems"];
			
			//SportsMessage object message
			SportsMessage sptMsg = new SportsMessage();

			/*****************************
			 * GoGo Pager2 alert message *
			 *****************************/
			string[] arrMsgType;
			string[] arrQueueNames;
			string[] arrMessageTypes;
			arrMsgType = (string[])HttpContext.Current.Application["messageType"];
			arrQueueNames = (string[])HttpContext.Current.Application["QueueItems"];
			arrMessageTypes = (string[])HttpContext.Current.Application["NotifyMessageTypes"];
			MessageClient msgClt = new MessageClient();
			msgClt.MessageType = arrMessageTypes[0];
			msgClt.MessagePath = arrQueueNames[0];

			try {
				arrSendToPager = HttpContext.Current.Request.Form["SendToPager"].Split(delimiter);
			} catch(Exception) {
				arrSendToPager = new string[0];
			}

			try {
				sCurrentTimestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
				sBatchJob = DateTime.Now.ToString("yyMMddHHmmss_fffffff") + "." + HttpContext.Current.Session["user_name"].ToString() + "." + arrMsgType[0] + ".ini";
				for(iUpdIdx = 0; iUpdIdx < arrBlankMsgID.Length; iUpdIdx++) {		//update blank message to table (SCHEDULEINFO)
					sMsg = arrBlankMsgContent[iUpdIdx].Trim();
					if(sMsg.Equals("")) sMsg = STANDARDMESSAGE;
					SQLString.Remove(0,SQLString.Length);
					SQLString.Append("update SCHEDULEINFO set MSGCONTENT='");
					SQLString.Append(sMsg);
					SQLString.Append("' where MSGID=");
					SQLString.Append(arrBlankMsgID[iUpdIdx]);
					m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
					m_SportsDBMgr.Close();

					SQLString.Remove(0,SQLString.Length);
					SQLString.Append("insert into LOG_BLANKMESSAGE (TIMEFLAG, IITEMSEQ_NO, SECTION, MsgType, MsgContent, BATCHJOB) values ('");
					SQLString.Append(sCurrentTimestamp);
					SQLString.Append("', ");
					SQLString.Append((iUpdIdx + 1).ToString());
					SQLString.Append(", 'SCHEDULE_', '");
					SQLString.Append(arrBlankMsgType[iUpdIdx]);
					SQLString.Append("', '");
					SQLString.Append(sMsg);
					SQLString.Append("', '");
					SQLString.Append(sBatchJob);
					SQLString.Append("')");
					m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
					m_SportsDBMgr.Close();
				}

				if(iUpdIdx > 0) {
					if(arrSendToPager.Length > 0) {
						//Modified by Henry,12 Feb 2004
						sptMsg.Body = sBatchJob;
						sptMsg.Timestamp = DateTime.Now.ToString("yyMMddHHmmss");
						sptMsg.AppID = "07";
						sptMsg.MsgID = "05";
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
								m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " MSMQ ERROR: Blank Message Manager");
								m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " BlankMessageManager.cs.SetMessage(): Notify via MSMQ throws MessageQueueException:  " + mqEx.ToString());
								m_SportsLog.Close();

								//If MSMQ fail, notify via .NET Remoting
								msgClt.MessageType = arrMessageTypes[1];
								msgClt.MessagePath = arrRemotingPath[0];
								if(!msgClt.SendMessage((object)sptMsg)) {
									m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
									m_SportsLog.SetFileName(0,LOGFILESUFFIX);
									m_SportsLog.Open();
									m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Remoting ERROR: Blank Message Manager");
									m_SportsLog.Close();
								}
							}	catch(Exception ex) {
								m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
								m_SportsLog.SetFileName(0,LOGFILESUFFIX);
								m_SportsLog.Open();
								m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Remoting ERROR: Blank Message Manager");
								m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " BlankMessageManager.cs.SetMessage(): Notify via .NET Remoting throws exception: " + ex.ToString());
								m_SportsLog.Close();
							}
						}	catch(Exception ex) {
							m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
							m_SportsLog.SetFileName(0,LOGFILESUFFIX);
							m_SportsLog.Open();
							m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " BlankMessageManager.cs.SetMessage(): Notify via MSMQ throws exception: " + ex.ToString());
							m_SportsLog.Close();
						}
					}
				}

				//write log
				m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " BlankMessageManager.cs: Set " + iUpdIdx.ToString() + " messages (" + HttpContext.Current.Session["user_name"] + ")");
				m_SportsLog.Close();
			}	catch(Exception ex) {
				iUpdIdx = -1;
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " BlankMessageManager.cs.SetMessage(): " + ex.ToString());
				m_SportsLog.Close();
			}

			return iUpdIdx;
		}
	}
}