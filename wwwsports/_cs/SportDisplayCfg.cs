/*
Objective:
Send instant alert message who enabled soccer1 message alert in pager

Last updated:
18 August 2005 Chris

C#.NET complier statement:
csc /t:library /out:..\bin\SportDisplayCfg.dll /r:..\bin\DBManager.dll;..\bin\Files.dll;..\bin\MessageClient.dll;..\bin\SportsMessage.dll; SportDisplayCfg.cs
*/

using System;
using System.Collections;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Data.OleDb;
using System.Reflection;
using System.Web;
using System.Runtime.InteropServices;
using System.Security.Principal;
using TDL.DB;
using TDL.IO;
using TDL.Message;
//using System.Runtime.Remoting.Channels;
//using System.Runtime.Remoting.Channels.Tcp;
//using RemoteService.Win32;
//using PagerFormatter;

[assembly:AssemblyCompany("TDSL")]
[assembly:AssemblyCopyright("(c) 2003 TDSL. All rights reserved.")]
[assembly:AssemblyDescription("其他資訊 -> 版面設定")]
[assembly:AssemblyProduct("Sports.NET")]
[assembly:AssemblyTitle("Sports.NET DLL")]
[assembly:AssemblyVersion("3.1.*")]
namespace SportsUtil {
	public class SportDisplayCfg {
		const int LOGON32_LOGON_INTERACTIVE = 2;
		const int LOGON32_PROVIDER_DEFAULT = 0;
		const int SecurityImpersonation = 2;
		const string LOGFILESUFFIX = "log";
		bool m_LoggedOn = false;
		string m_NetUser = "ASPUSER";
		string m_Password = "aspspt";
		string m_Domain = System.Environment.MachineName;
		WindowsImpersonationContext m_ImpersonationContext;
		WindowsIdentity m_TempWindowsIdentity;
		IntPtr m_Token = IntPtr.Zero;
		IntPtr m_TokenDuplicate = IntPtr.Zero;
		DBManager m_SportsDBMgr;
		OleDbDataReader m_SportsOleReader;
		Files m_SportsLog;
		Encoding m_Big5Encoded;
		char[] delimiter;
		string sSQL = "";
		String sAppID = "";
		String sChiMsgType = "";
		//SportsMessage object message
		SportsMessage sptMsg;
		NameValueCollection taskNVC;
		MessageClient msgClt;
		string[] arrQueueNames;
		string[] arrMessageTypes;
		string[] arrRemotingPath;
		string sCurrentTimestamp = null;
		string sBatchJob = null;
				
		[DllImport("advapi32.dll", SetLastError=true)]
		public static extern bool LogonUser(string lpszUserName, string lpszDomain, string lpszPassword, int dwLogonType, int dwLogonProvider, ref IntPtr phToken);

		[DllImport("advapi32.dll", SetLastError=true)]
		public extern static int DuplicateToken(IntPtr hToken, int impersonationLevel, ref IntPtr hNewToken);

		public SportDisplayCfg(string Connection) {
			m_SportsDBMgr = new DBManager();
			m_SportsDBMgr.ConnectionString = Connection;
			m_SportsLog = new Files();
			m_Big5Encoded = Encoding.GetEncoding(950);
			delimiter = new char[] {','};
			sptMsg = new SportsMessage();
			taskNVC = (NameValueCollection)HttpContext.Current.Application["HKJCAdminTaskItems"];
			arrQueueNames = (string[])HttpContext.Current.Application["QueueItems"];
			arrMessageTypes = (string[])HttpContext.Current.Application["NotifyMessageTypes"];
			arrRemotingPath = (string[])HttpContext.Current.Application["RemotingItems"];
			msgClt = new MessageClient();
			msgClt.MessageType = arrMessageTypes[0];
			msgClt.MessagePath = arrQueueNames[0];
		}
		
		public string GetMessagePage() {
			int iNewsMsgID;
			StringBuilder HTMLString = new StringBuilder();
			
			sAppID = HttpContext.Current.Request.QueryString["AppID"];
			try {
				sSQL = "select CHI_NAME from NEWSGROUP_CONVERTER where INEWS_ID="+sAppID;
				m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(sSQL);
				if (m_SportsOleReader.Read())
					sChiMsgType = m_SportsOleReader.GetString(0).Trim();
				m_SportsOleReader.Close();
				m_SportsDBMgr.Close();
				
				sSQL = "select ISEQNO, CINFOTYPE from WEB_NEWS_CFG where CAPPTYPE like '"+sChiMsgType+"%' order by ISEQNO";
				m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(sSQL);			
				
				HTMLString.Append("<select name=\"OldMsgName\"><option value=\"0\">請選擇</option>");
				while(m_SportsOleReader.Read()) {
					iNewsMsgID = m_SportsOleReader.GetInt32(0);
					sChiMsgType = m_SportsOleReader.GetString(1).Trim();
					sChiMsgType = sChiMsgType.Substring(0,2);
					HTMLString.Append("<option value=");				
					HTMLString.Append(iNewsMsgID.ToString());
					HTMLString.Append(">");
					HTMLString.Append(sChiMsgType);
					HTMLString.Append("</option>");
				}
				HTMLString.Append("</select>");
				HTMLString.Append("</td><td><br><input name=\"NewMsgName\" value=\"\" maxlength=\"2\" size=\"6\"><br><br>");
				m_SportsOleReader.Close();
				m_SportsDBMgr.Close();
			} catch(Exception ex) {
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " SportDisplayCfg.cs.Send(): " + ex.ToString());
				m_SportsLog.Close();
			}
			return HTMLString.ToString();
		}
		
		//Format message immediately and send to pager via .NET Remoting
		public int ModifyName() {

			int iUpdated = 1;
			int i = 0;
			bool bGetSelection = true;
			
			// Updated database variable
			string sOldMsgID = "";
			string sOldMsgName = "";
			string sNewMsgName = "";
			string sChangeName = "";
			string[] arrMsgType;
			string[] arrSendToPager;
			string sPathID = "0";
			
			// Updated ini file variable
			Encoding m_CodePage;
		    FileStream m_FS;
		    StreamReader m_SR;
		    StreamWriter m_SW;
		    string sGetText = "";
		    string sChangeText = "";		    
		    int iChangeIndex = 0;
		    string sMenuID = "";
		    string sMessage = "";
		    string m_CR = "\r\n";
	        string path = "";
	        
	        // Message Dispatcher
	        int iIndex = 0;
	        string[] arrMenu;
	        string sValue = "";
	        bool bExist = false;
	        
	        		    
			try {
				arrSendToPager = HttpContext.Current.Request.Form["SendToPager"].Split(delimiter);
			} catch(Exception) {
				arrSendToPager = new string[0];
			}
			
			arrMsgType = (string[])HttpContext.Current.Application["messageType"];
			sOldMsgID = HttpContext.Current.Request.Form["OldMsgName"];
			sNewMsgName = HttpContext.Current.Request.Form["NewMsgName"];
			sAppID = HttpContext.Current.Request.QueryString["AppID"];
			
			try {
				m_LoggedOn = LogonUser(m_NetUser, m_Domain, m_Password, LOGON32_LOGON_INTERACTIVE, LOGON32_PROVIDER_DEFAULT, ref m_Token);

				if(m_LoggedOn) {
					if(DuplicateToken(m_Token, SecurityImpersonation, ref m_TokenDuplicate) != 0) {
						
						m_TempWindowsIdentity = new WindowsIdentity(m_TokenDuplicate);
						m_ImpersonationContext = m_TempWindowsIdentity.Impersonate();
						
						if(m_ImpersonationContext != null) {
							
							// To check new menu name
							sSQL = "select CHI_NAME from NEWSGROUP_CONVERTER where INEWS_ID="+sAppID;
							m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(sSQL);
							if (m_SportsOleReader.Read())
								sChiMsgType = m_SportsOleReader.GetString(0).Trim();
							m_SportsOleReader.Close();
							m_SportsDBMgr.Close();
							
							sSQL = "select CINFOTYPE from WEB_NEWS_CFG where CAPPTYPE like '"+sChiMsgType+"%' order by ISEQNO";
							m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(sSQL);
							while (m_SportsOleReader.Read()) {
								sChiMsgType = m_SportsOleReader.GetString(0).Trim();
								if (sChiMsgType != "") {
									iChangeIndex = sChiMsgType.IndexOf(sNewMsgName);
									if (iChangeIndex != -1) {
										iUpdated++;
										break;
									}
								}
							}
							m_SportsOleReader.Close();
							m_SportsDBMgr.Close();
							
							if (iUpdated > 1) {
								iUpdated = -3;
								return iUpdated;
							}
							
							// process modify menu name
							sSQL = "select CINFOTYPE from WEB_NEWS_CFG where ISEQNO="+sOldMsgID;
							m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(sSQL);
							if (m_SportsOleReader.Read())
								sOldMsgName = m_SportsOleReader.GetString(0).Trim();
							m_SportsOleReader.Close();
							m_SportsDBMgr.Close();
							
							sChangeName = sOldMsgName.Substring(0, 2);
							sOldMsgName = sOldMsgName.Replace(sOldMsgName.Substring(0, 2), sNewMsgName.Trim());
							sSQL = "update WEB_NEWS_CFG set CINFOTYPE='"+sOldMsgName+"' where ISEQNO="+sOldMsgID;
							m_SportsDBMgr.ExecuteNonQuery(sSQL);
							m_SportsDBMgr.Close();
							
							arrMenu = new string[3];
							for (i=0; i < arrSendToPager.Length; i++) {
								if (arrSendToPager[i] == "1") {
									sMenuID = "Menu6=";
									sPathID = "1";
								} else if (arrSendToPager[i] == "2") {
									sMenuID = "Menu82=";
									sPathID = "1";
								} else if (arrSendToPager[i] == "3") {
									sMenuID = "Menu14=";
									sPathID = "2";
								} else if (arrSendToPager[i] == "4" && Convert.ToInt32(sPathID) < 3) {
									sMenuID = "OTHER_SPORTS";
									sPathID = "3";
									bGetSelection = true;
								} else if (arrSendToPager[i] == "4" && Convert.ToInt32(sPathID) < 4) {
									sMenuID = "OTHER_SPORTS";
									sPathID = "4";
									bGetSelection = true;
								}
								
								sSQL = "select CPATH from MENUSOURCE_CFG where IPATH_ID="+sPathID;
								m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(sSQL);
								if (m_SportsOleReader.Read())
									path = m_SportsOleReader.GetString(0).Trim();
								m_SportsOleReader.Close();
								m_SportsDBMgr.Close();
								
								string dtNow = DateTime.Now.ToString("yyyy-MM-dd.HH.mm.ss");
								string path2 = path+"."+arrSendToPager[i]+"."+dtNow;
								m_CodePage = Encoding.GetEncoding(950);
								
						        // Ensure that the target does not exist.    
					            if (File.Exists(path)) {
						            
						            // Copy the file.
							        File.Copy(path, path2);
							        m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
									m_SportsLog.SetFileName(0,LOGFILESUFFIX);
									m_SportsLog.Open();
									m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " SportDisplayCfg.cs: "+path+" was copied to "+path2);
									m_SportsLog.Close();
									
									// Delete the old file
						            File.Delete(path);
							        
						            // Open the file to read from.
							        m_FS = new FileStream(path2,FileMode.Open,FileAccess.Read,FileShare.ReadWrite);
							        m_SR = new StreamReader(m_FS,m_CodePage);
						            
							        sMessage = "";
						            while ((sGetText = m_SR.ReadLine()) != null) {
							            if (sGetText != "") {
							            	iChangeIndex = sGetText.IndexOf(sMenuID);
							            	if (iChangeIndex != -1) {
								            	// Only for JCCombo Device
								            	if (arrSendToPager[i] == "4" && bGetSelection == true) {
									            	sMenuID = sChangeName;
								            		bGetSelection = false;
							            		}
								            
									            iChangeIndex = sGetText.IndexOf(sChangeName);
									            if (iChangeIndex != -1) {
										            sChangeText = sGetText.Substring(iChangeIndex, 2);
										            sGetText = sGetText.Replace(sChangeText, sNewMsgName);
										            if (arrSendToPager[i] != "4") {
										            	iChangeIndex = sGetText.IndexOf("=");
										            	iChangeIndex++;
										            	sValue = sGetText.Substring(iChangeIndex, sGetText.Length-iChangeIndex);
														arrMenu[iIndex] = sValue;
										            	iIndex++;
									            	}
										            sMessage += sGetText;
										            sMessage += m_CR;
								            	}else {
									            	sMessage += sGetText;
									            	sMessage += m_CR;
								            	}
							            	} else {
								            	sMessage += sGetText;
								            	sMessage += m_CR;
								            }
							        	} else {
							            	sMessage += sGetText;
							            	sMessage += m_CR;
						            	}
						            }
						            m_SR.Close();
							        m_FS.Close();
						            	
						            m_FS = new FileStream(path,FileMode.Append,FileAccess.Write,FileShare.ReadWrite);
						            m_SW = new StreamWriter(m_FS,m_CodePage);
						            m_SW.Write(sMessage);
						            m_SW.Close();
						            m_FS.Close();
					        	} else {
						        	m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
									m_SportsLog.SetFileName(0,LOGFILESUFFIX);
									m_SportsLog.Open();
									m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " SportDisplayCfg.cs: File of "+path+" is not exist");
									m_SportsLog.Close();
									iUpdated = -1;
				        		}
				        		if (arrSendToPager[i] == "4" && Convert.ToInt32(sPathID) < 4) {
					        		i--;
				        		}
							}
							if (iIndex > 0) {
								bExist = true;
								iIndex = 0;
							}
							for (i=0; i < arrSendToPager.Length; i++) {
								string[] arrPathID;								
								
								
								// send to dispatcher
								if (arrSendToPager[i] == "4") {
									arrPathID = new string[2];
									arrPathID[0] = "11";
									arrPathID[1] = "13";
									
									sCurrentTimestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
									sBatchJob = DateTime.Now.ToString("yyMMddHHmmss_fffffff") + "." + HttpContext.Current.Session["user_name"].ToString() + "." + arrMsgType[12] + "." + iIndex.ToString() + ".ini";
									sSQL = "insert into LOG_ADMINTASK (TIMEFLAG, SECTION, CMD, PARAM, BATCHJOB) values ('" +sCurrentTimestamp+ "','COMMAND_','RESEND','SPORTNEWS_MENU','" +sBatchJob+ "')";
					        		m_SportsDBMgr.ExecuteNonQuery(sSQL);
									m_SportsDBMgr.Close();
									
									sptMsg.IsTransaction = false;
									sptMsg.Body = sBatchJob;
									sptMsg.Timestamp = DateTime.Now.ToString("yyMMddHHmmss");
									sptMsg.AppID = "07";
									sptMsg.MsgID = "23";
									sptMsg.PathID = new string[0];
									for(int j = 0; j< arrPathID.Length; j++) {
										sptMsg.AddPathID(arrPathID[j]);
									}
								} else {
									if (arrSendToPager[i] == "1")
										sPathID = "1";
									else if (arrSendToPager[i] == "2")
										sPathID = "1";
									else if (arrSendToPager[i] == "3")
										sPathID = "4";
									if (bExist) {
										sCurrentTimestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
										sBatchJob = DateTime.Now.ToString("yyMMddHHmmss_fffffff") + "." + HttpContext.Current.Session["user_name"].ToString() + "." + arrMsgType[6] + "." + iIndex.ToString() + ".ini";
										sSQL = "insert into LOG_MENU (TIMEFLAG, SECTION, MENU, BATCHJOB) values ('" +sCurrentTimestamp+ "','MENU','" +arrMenu[iIndex]+ "','" +sBatchJob+ "')";
										m_SportsDBMgr.ExecuteNonQuery(sSQL);
										m_SportsDBMgr.Close();
	
										//Send Notify Message
										sptMsg.IsTransaction = false;
										sptMsg.Body = sBatchJob;
										sptMsg.Timestamp = DateTime.Now.ToString("yyMMddHHmmss");
										sptMsg.AppID = "07";
										sptMsg.MsgID = "25";
										sptMsg.PathID = new string[0];
										sptMsg.AddPathID(sPathID);
										
										iIndex++;
									}
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
										m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " MSMQ ERROR: Sports Menu Setting");
										m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " SportDisplayCfg.cs.ModifyName(): Notify via MSMQ throws MessageQueueException:  " + mqEx.ToString());
										m_SportsLog.Close();
								
										//If MSMQ fail, notify via .NET Remoting
										msgClt.MessageType = arrMessageTypes[1];
										msgClt.MessagePath = arrRemotingPath[0];
										if(!msgClt.SendMessage((object)sptMsg)) {
											m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
											m_SportsLog.SetFileName(0,LOGFILESUFFIX);
											m_SportsLog.Open();
											m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Remoting ERROR: Sports Menu Setting");
											m_SportsLog.Close();
										}
									}	catch(Exception ex) {
										iUpdated = 0;
										m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
										m_SportsLog.SetFileName(0,LOGFILESUFFIX);
										m_SportsLog.Open();
										m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Remoting ERROR: Sports Menu Setting");
										m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " SportDisplayCfg.cs.ModifyName(): Notify via .NET Remoting throws exception: " + ex.ToString());
										m_SportsLog.Close();
									}							
								} catch(Exception ex) {
									iUpdated = 0;
									m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
									m_SportsLog.SetFileName(0,LOGFILESUFFIX);
									m_SportsLog.Open();
									m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " SportDisplayCfg.cs.ModifyName(): Notify via MSMQ throws exception: " + ex.ToString());
									m_SportsLog.Close();
								}	
							}// end for
							
							m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
							m_SportsLog.SetFileName(0,LOGFILESUFFIX);
							m_SportsLog.Open();
							m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " HKJCSchedule.cs: Modify Sports Menu: (" + HttpContext.Current.Session["user_name"] + ")");
							m_SportsLog.Close();
							m_ImpersonationContext.Undo();
						} else {
							iUpdated = 0;
							m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
							m_SportsLog.SetFileName(0,LOGFILESUFFIX);
							m_SportsLog.Open();
							m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " SportDisplayCfg.cs.GetINISetting(): m_ImpersonationContext is null.");
							m_SportsLog.Close();
						}
					} else {
						iUpdated = 0;
						m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
						m_SportsLog.SetFileName(0,LOGFILESUFFIX);
						m_SportsLog.Open();
						m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " SportDisplayCfg.cs.GetINISetting(): DuplicateToken error, code = " + Marshal.GetLastWin32Error());
						m_SportsLog.Close();
					}
				} else {
					iUpdated = 0;
					m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
					m_SportsLog.SetFileName(0,LOGFILESUFFIX);
					m_SportsLog.Open();
					m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " SportDisplayCfg.cs.GetINISetting(): LogonUser error, code = " + Marshal.GetLastWin32Error());
					m_SportsLog.Close();
				}
			} catch(NullReferenceException nullRefEx) {
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " SportDisplayCfg.cs.Send(): " + nullRefEx.ToString());
				m_SportsLog.Close();
				iUpdated = -2;
			} catch(Exception ex) {
				iUpdated = 0;
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " SportDisplayCfg.cs.Send(): " + ex.ToString());
				m_SportsLog.Close();
			}
			return iUpdated;
		}
	}
}
