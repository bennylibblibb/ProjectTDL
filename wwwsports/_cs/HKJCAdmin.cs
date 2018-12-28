/*
Objective:
Create command ini file to resend pager data

Last updated:
19 Feb 2004, Chapman Choi

C#.NET complier statement:
csc /t:library /out:..\bin\HKJCAdmin.dll /r:..\bin\Files.dll;..\bin\MessageClient.dll;..\bin\SportsMessage.dll HKJCAdmin.cs
*/

using System;
using System.Collections;
using System.Collections.Specialized;
using System.Reflection;
using System.Text;
using System.Web;
using TDL.IO;
using TDL.Message;

[assembly:AssemblyCompany("TDSL")]
[assembly:AssemblyCopyright("(c) 2003 TDSL. All rights reserved.")]
[assembly:AssemblyDescription("馬會足球 -> 重發資訊")]
[assembly:AssemblyProduct("Sports.NET")]
[assembly:AssemblyTitle("Sports.NET DLL")]
[assembly:AssemblyVersion("3.1.*")]
namespace SportsUtil {
	public class HKJCAdmin {
		const string LOGFILESUFFIX = "log";
		Files m_SportsLog;
		Encoding m_Big5Encoded;
		NameValueCollection taskNVC;
		MessageClient msgClt;
		string[] arrQueueNames;
		string[] arrMessageTypes;
		string[] arrRemotingPath;
		string m_Role;
		SportsMessage sptMsg;	//SportsMessage object message

		public HKJCAdmin() {
			m_SportsLog = new Files();
			m_Big5Encoded = Encoding.GetEncoding(950);
			taskNVC = (NameValueCollection)HttpContext.Current.Application["HKJCAdminTaskItems"];
			arrQueueNames = (string[])HttpContext.Current.Application["QueueItems"];
			arrMessageTypes = (string[])HttpContext.Current.Application["NotifyMessageTypes"];
			arrRemotingPath = (string[])HttpContext.Current.Application["RemotingItems"];
			msgClt = new MessageClient();
			msgClt.MessageType = arrMessageTypes[0];
			msgClt.MessagePath = arrQueueNames[0];
			m_Role = (string)HttpContext.Current.Session["user_role"];
			sptMsg = new SportsMessage();
		}

		public int NotifyProcess(string sAction, string sAppID) {
			int iRecUpd = 0;

			try {
				if(Convert.ToInt32(m_Role) > 10) {
					int iResendItems;
					int iMsgBodyLength = 0;
					char[] delimiter = new char[] {','};
					string[] arrResendItems;
					string sTaskID;
					byte[] arrByteOfMSMQBody;

					try {
						if(sAction.Equals("R")) arrResendItems = (string[])HttpContext.Current.Request.Form["resend"].Split(delimiter);
						else if(sAction.Equals("D")) arrResendItems = (string[])HttpContext.Current.Request.Form["delete"].Split(delimiter);
						else if(sAction.Equals("L")) arrResendItems = (string[])HttpContext.Current.Request.Form["reload"].Split(delimiter);
						else arrResendItems = new string[0];
						iResendItems = arrResendItems.Length;
					} catch(Exception) {
						arrResendItems = new string[0];
						iResendItems = 0;
					}

					if(iResendItems > 0) {
						for(iRecUpd = 0; iRecUpd < iResendItems; iRecUpd++) {
							sTaskID = taskNVC[arrResendItems[iRecUpd].ToString()];
							arrByteOfMSMQBody = m_Big5Encoded.GetBytes(sTaskID);
							iMsgBodyLength = arrByteOfMSMQBody.Length;
							sptMsg.IsTransaction = false;
							sptMsg.Body = iMsgBodyLength.ToString("D3") + sTaskID;
							sptMsg.Timestamp = DateTime.Now.ToString("yyMMddHHmmss");
							sptMsg.AppID = sAppID;
							sptMsg.MsgID = "00";
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
									m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " MSMQ ERROR: HKJC Admin");
									m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " HKJCAdmin.cs.NotifyProcess(): Notify via MSMQ throws MessageQueueException:  " + mqEx.ToString());
									m_SportsLog.Close();

									//If MSMQ fail, notify via .NET Remoting
									msgClt.MessageType = arrMessageTypes[1];
									msgClt.MessagePath = arrRemotingPath[0];
									if(!msgClt.SendMessage((object)sptMsg)) {
										m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
										m_SportsLog.SetFileName(0,LOGFILESUFFIX);
										m_SportsLog.Open();
										m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Remoting ERROR: HKJC Admin");
										m_SportsLog.Close();
									}
								}	catch(Exception ex) {
									m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
									m_SportsLog.SetFileName(0,LOGFILESUFFIX);
									m_SportsLog.Open();
									m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Remoting ERROR: HKJC Admin");
									m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " HKJCAdmin.cs.NotifyProcess(): Notify via .NET Remoting throws exception: " + ex.ToString());
									m_SportsLog.Close();
								}							
							}	catch(Exception ex) {
								m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
								m_SportsLog.SetFileName(0,LOGFILESUFFIX);
								m_SportsLog.Open();
								m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " HKJCAdmin.cs.NotifyProcess(): Notify via MSMQ throws exception: " + ex.ToString());
								m_SportsLog.Close();
							}								
							//Modified end					
														
						}
						//write log
						m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
						m_SportsLog.SetFileName(0,LOGFILESUFFIX);
						m_SportsLog.Open();
						m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " HKJCAdmin.cs: send " + iRecUpd.ToString() + " items, action = " + sAction + " (" + HttpContext.Current.Session["user_name"] + ")");
						m_SportsLog.Close();
					} else {
						//write log
						m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
						m_SportsLog.SetFileName(0,LOGFILESUFFIX);
						m_SportsLog.Open();
						m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " HKJCAdmin.cs: no item was selected to send (" + HttpContext.Current.Session["user_name"] + ")");
						m_SportsLog.Close();
					}
				} else {	//no right to resend info
					iRecUpd = -99;
					//write log
					m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
					m_SportsLog.SetFileName(0,LOGFILESUFFIX);
					m_SportsLog.Open();
					m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " HKJCAdmin.cs: " + HttpContext.Current.Session["user_name"].ToString() + " did not allow to send data, action = " + sAction + " (" + HttpContext.Current.Session["user_name"] + ")");
					m_SportsLog.Close();
				}
			} catch(Exception ex) {
				iRecUpd = -1;
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " HKJCAdmin.cs.Notify(): " + ex.ToString());
				m_SportsLog.Close();
			}

			return iRecUpd;
		}
	}
}