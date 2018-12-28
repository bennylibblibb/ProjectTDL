/*
Objective:
Activate chart plotting or sync clock or prune chart

Last updated:
19 Feb 2004, Chapman Choi

C#.NET complier statement:
csc /t:library /out:..\bin\GOGO2Chart.dll /r:..\bin\Files.dll;..\bin\MessageClient.dll;..\bin\SportsMessage.dll GOGO2Chart.cs
*/

using System;
using System.Collections.Specialized;
using System.Reflection;
using System.Text;
using System.Web;
using TDL.IO;
using TDL.Message;

[assembly:AssemblyCompany("TDSL")]
[assembly:AssemblyCopyright("(c) 2003 TDSL. All rights reserved. Created on 14 August 2003.")]
[assembly:AssemblyDescription("GOGO2指數圖表")]
[assembly:AssemblyProduct("Sports.NET")]
[assembly:AssemblyTitle("Sports.NET DLL")]
[assembly:AssemblyVersion("3.1.*")]
namespace SportsUtil {
	public class GOGO2Chart {
		bool bOpr;
		const string LOGFILESUFFIX = "log";
		string m_TaskID;
		string[] arrQueueNames;
		string[] arrMessageTypes;
		string[] arrRemotingPath;
		Files m_SportsLog;
		MessageClient msgClt;
		Encoding m_Big5Encoded;
		NameValueCollection taskNVC;
		//SportsMessage object message
		SportsMessage sptMsg;
	

		public GOGO2Chart() {
			m_TaskID = "1";
			m_Big5Encoded = Encoding.GetEncoding(950);
			m_SportsLog = new Files();
			bOpr = false;
			arrQueueNames = (string[])HttpContext.Current.Application["QueueItems"];
			arrMessageTypes = (string[])HttpContext.Current.Application["NotifyMessageTypes"];
			arrRemotingPath = (string[])HttpContext.Current.Application["RemotingItems"];
			msgClt = new MessageClient();
			msgClt.MessageType = arrMessageTypes[0];
			msgClt.MessagePath = arrQueueNames[0];
			taskNVC = (NameValueCollection)HttpContext.Current.Application["HKJCAdminTaskItems"];
			sptMsg = new SportsMessage();
		}

		public bool ActivateChart() {
			int iMsgBodyLength = 0;
			string sHandleCode;
			byte[] arrByteOfMSMQBody;

			//Send Chart Message
			sHandleCode = (string)HttpContext.Current.Request.Form["handlecode"];
			m_TaskID = taskNVC[sHandleCode];
			
			//Modified by Chapman, 19 Feb 2004
			arrByteOfMSMQBody = m_Big5Encoded.GetBytes(m_TaskID);
			iMsgBodyLength = arrByteOfMSMQBody.Length;
			sptMsg.IsTransaction = false;
			sptMsg.Body = iMsgBodyLength.ToString("D3") + m_TaskID;
			sptMsg.Timestamp = DateTime.Now.ToString("yyMMddHHmmss");
			sptMsg.AppID = "06";
			sptMsg.MsgID = "00";
			try {
				//Notify via MSMQ
				msgClt.MessageType = arrMessageTypes[0];
				msgClt.MessagePath = arrQueueNames[0];
				msgClt.SendMessage(sptMsg);
				bOpr = true;
			} catch(System.Messaging.MessageQueueException mqEx) {
				try {
					m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
					m_SportsLog.SetFileName(0,LOGFILESUFFIX);
					m_SportsLog.Open();
					m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " MSMQ ERROR: Gogo2 Chart");
					m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Gogo2Chart.cs.ActiveChart(): Notify via MSMQ throws MessageQueueException:  " + mqEx.ToString());
					m_SportsLog.Close();

					//If MSMQ fail, notify via .NET Remoting
					msgClt.MessageType = arrMessageTypes[1];
					msgClt.MessagePath = arrRemotingPath[0];
					bOpr = true;
					
					if(!msgClt.SendMessage((object)sptMsg)) {
						m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
						m_SportsLog.SetFileName(0,LOGFILESUFFIX);
						m_SportsLog.Open();
						m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Remoting ERROR: Gogo2 Chart");
						m_SportsLog.Close();					
					}
					
				}	catch(Exception ex) {
					m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
					m_SportsLog.SetFileName(0,LOGFILESUFFIX);
					m_SportsLog.Open();
					m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Remoting ERROR: Gogo2 Chart");
					m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Gogo2Chart.cs.ActiveChart(): Notify via .NET Remoting throws exception: " + ex.ToString());
					m_SportsLog.Close();
					bOpr = false;
				}							
			}	catch(Exception ex) {
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Gogo2Chart.cs.ActiveChart(): Notify via MSMQ throws exception: " + ex.ToString());
				m_SportsLog.Close();
				bOpr = false;
			}

			return bOpr;
		}

		public bool SyncClock() {
			int iMsgBodyLength = 0;
			string sHandleCode;
			byte[] arrByteOfMSMQBody;

			sHandleCode = (string)HttpContext.Current.Request.Form["handlecode"];
			m_TaskID = taskNVC[sHandleCode];
			
			//Modified by Chapman, 19 Feb 2004
			arrByteOfMSMQBody = m_Big5Encoded.GetBytes(m_TaskID);
			iMsgBodyLength = arrByteOfMSMQBody.Length;
			sptMsg.IsTransaction = false;
			sptMsg.Body = iMsgBodyLength.ToString("D3") + m_TaskID;
			sptMsg.Timestamp = DateTime.Now.ToString("yyMMddHHmmss");
			sptMsg.AppID = "06";
			sptMsg.MsgID = "00";
			try {
				//Notify via MSMQ
				msgClt.MessageType = arrMessageTypes[0];
				msgClt.MessagePath = arrQueueNames[0];
				msgClt.SendMessage(sptMsg);
				bOpr = true;
			} catch(System.Messaging.MessageQueueException mqEx) {
				try {
					m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
					m_SportsLog.SetFileName(0,LOGFILESUFFIX);
					m_SportsLog.Open();
					m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " MSMQ ERROR: Gogo2 Chart");
					m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Gogo2Chart.cs.SyncClock(): Notify via MSMQ throws MessageQueueException:  " + mqEx.ToString());
					m_SportsLog.Close();

					//If MSMQ fail, notify via .NET Remoting
					msgClt.MessageType = arrMessageTypes[1];
					msgClt.MessagePath = arrRemotingPath[0];
					bOpr = true;
					
					if(!msgClt.SendMessage((object)sptMsg)) {
						m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
						m_SportsLog.SetFileName(0,LOGFILESUFFIX);
						m_SportsLog.Open();
						m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Remoting ERROR: Gogo2 Chart");
						m_SportsLog.Close();				
					}
					
				}	catch(Exception ex) {
					m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
					m_SportsLog.SetFileName(0,LOGFILESUFFIX);
					m_SportsLog.Open();
					m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Remoting ERROR: Gogo2 Chart");
					m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Gogo2Chart.cs.SyncClock(): Notify via .NET Remoting throws exception: " + ex.ToString());
					m_SportsLog.Close();
					bOpr = false;
				}							
			}	catch(Exception ex) {
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Gogo2Chart.cs.SyncClock(): Notify via MSMQ throws exception: " + ex.ToString());
				m_SportsLog.Close();
				bOpr = false;
			}

			return bOpr;
		}

		public bool PruneChart() {
			int iMsgBodyLength = 0;
			string sHandleCode;
			byte[] arrByteOfMSMQBody;

			sHandleCode = (string)HttpContext.Current.Request.Form["handlecode"];
			m_TaskID = taskNVC[sHandleCode];
			
			//Modified by Chapman, 19 Feb 2004
			arrByteOfMSMQBody = m_Big5Encoded.GetBytes(m_TaskID);
			iMsgBodyLength = arrByteOfMSMQBody.Length;
			sptMsg.IsTransaction = false;
			sptMsg.Body = iMsgBodyLength.ToString("D3") + m_TaskID;
			sptMsg.Timestamp = DateTime.Now.ToString("yyMMddHHmmss");
			sptMsg.AppID = "06";
			sptMsg.MsgID = "00";
			try {
				//Notify via MSMQ
				msgClt.MessageType = arrMessageTypes[0];
				msgClt.MessagePath = arrQueueNames[0];
				msgClt.SendMessage(sptMsg);
				bOpr = true;
			} catch(System.Messaging.MessageQueueException mqEx) {
				try {
					m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
					m_SportsLog.SetFileName(0,LOGFILESUFFIX);
					m_SportsLog.Open();
					m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " MSMQ ERROR: Gogo2 Chart");
					m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Gogo2Chart.cs.PruneChart(): Notify via MSMQ throws MessageQueueException:  " + mqEx.ToString());
					m_SportsLog.Close();

					//If MSMQ fail, notify via .NET Remoting
					msgClt.MessageType = arrMessageTypes[1];
					msgClt.MessagePath = arrRemotingPath[0];
					bOpr = true;
					
					if(!msgClt.SendMessage((object)sptMsg)) {
						m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
						m_SportsLog.SetFileName(0,LOGFILESUFFIX);
						m_SportsLog.Open();
						m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Remoting ERROR: Gogo2 Chart");
						m_SportsLog.Close();					
					}
					
				}	catch(Exception ex) {
					m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
					m_SportsLog.SetFileName(0,LOGFILESUFFIX);
					m_SportsLog.Open();
					m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Remoting ERROR: Gogo2 Chart");
					m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Gogo2Chart.cs.PruneChart(): Notify via .NET Remoting throws exception: " + ex.ToString());
					m_SportsLog.Close();
					bOpr = false;
				}							
			}	catch(Exception ex) {
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Gogo2Chart.cs.PruneChart(): Notify via MSMQ throws exception: " + ex.ToString());
				m_SportsLog.Close();
				bOpr = false;
			}

			return bOpr;
		}
	}
}