/*
Objective:
Resend menu

Last updated:
19 Feb 2004, Chapman Choi

C#.NET complier statement:
csc /t:library /out:..\bin\PagerMenu.dll /r:..\bin\DBManager.dll;..\bin\Files.dll;..\bin\Win32INI.dll;..\bin\MessageClient.dll;..\bin\SportsMessage.dll PagerMenu.cs
*/

using System;
using System.Collections;
using System.Data.OleDb;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Web;
using TDL.DB;
using TDL.IO;
using TDL.Message;

[assembly:AssemblyCompany("TDSL")]
[assembly:AssemblyCopyright("(c) 2003 TDSL. All rights reserved. Created on 20 Oct 2003.")]
[assembly:AssemblyDescription("管理項目 -> 重發菜單")]
[assembly:AssemblyProduct("Sports.NET")]
[assembly:AssemblyTitle("Sports.NET DLL")]
[assembly:AssemblyVersion("3.1.*")]
namespace SportsUtil {
	public class PagerMenu {
		public const int LOGON32_LOGON_INTERACTIVE = 2;
		public const int LOGON32_PROVIDER_DEFAULT = 0;
		public const int SecurityImpersonation = 2;
		const string LOGFILESUFFIX = "log";
		DBManager m_SportsDBMgr;
		OleDbDataReader m_SportsOleReader;
		Files m_SportsLog;
		StringBuilder SQLString;

		bool m_LoggedOn = false;
		string m_NetUser = "ASPUSER";
		string m_Password = "aspspt";
		string m_Domain = System.Environment.MachineName;
		WindowsImpersonationContext m_ImpersonationContext;
		WindowsIdentity m_TempWindowsIdentity;
		IntPtr m_Token = IntPtr.Zero;
		IntPtr m_TokenDuplicate = IntPtr.Zero;

		[DllImport("advapi32.dll", SetLastError=true)]
		public static extern bool LogonUser(string lpszUserName, string lpszDomain, string lpszPassword, int dwLogonType, int dwLogonProvider, ref IntPtr phToken);

		[DllImport("advapi32.dll", SetLastError=true)]
		public extern static int DuplicateToken(IntPtr hToken, int impersonationLevel, ref IntPtr hNewToken);

		public PagerMenu(string Connection) {
			m_SportsDBMgr = new DBManager();
			m_SportsDBMgr.ConnectionString = Connection;
			m_SportsLog = new Files();
			SQLString = new StringBuilder();
		}

		public string GetMenuItems() {
			string sRole;
			StringBuilder HTMLString = new StringBuilder();

			sRole = (string)HttpContext.Current.Session["user_role"];
			try {
				if(Convert.ToInt32(sRole) >= 988) {
					int iIndex = 0;
					string sTypeID;
					string sGpID;
					string sMenuType;
					string sMenuGroup;
					string sMenuSrcPath;
					string sSection;
					string sValue;
					string[] arrKeys;

					sTypeID = HttpContext.Current.Request.QueryString["typeID"];
					sGpID = HttpContext.Current.Request.QueryString["gpID"];
					sSection = (string)HttpContext.Current.Application["MenuSection"];
					arrKeys = (string[])HttpContext.Current.Application["MenuKeys"];

					//Get Source path of Menu
					SQLString.Remove(0,SQLString.Length);
					SQLString.Append("select CPATH from MENUSOURCE_CFG where IPATH_ID in (select IPATH_ID from MENUMAP where ITYPE_ID=");
					SQLString.Append(sTypeID);
					SQLString.Append(" and IGROUP_ID=");
					SQLString.Append(sGpID);
					SQLString.Append(")");
					sMenuSrcPath = m_SportsDBMgr.ExecuteQueryString(SQLString.ToString());
					m_SportsDBMgr.Close();
					if(sMenuSrcPath == null) sMenuSrcPath = "";

					//Get Type of Menu
					SQLString.Remove(0,SQLString.Length);
					SQLString.Append("select CTYPE from MENUTYPE_CFG where ITYPE_ID=");
					SQLString.Append(sTypeID);
					sMenuType = m_SportsDBMgr.ExecuteQueryString(SQLString.ToString());
					m_SportsDBMgr.Close();

					//Get Group of Menu
					SQLString.Remove(0,SQLString.Length);
					SQLString.Append("select CGROUP from MENUGROUP_CFG where IGROUP_ID=");
					SQLString.Append(sGpID);
					sMenuGroup = m_SportsDBMgr.ExecuteQueryString(SQLString.ToString());
					m_SportsDBMgr.Close();

					HTMLString.Append("<tr align=\"left\" style=\"background-color:#00bfff\"><th colspan=\"2\">");
					if(sMenuType != null) HTMLString.Append(sMenuType);
					HTMLString.Append(" - ");
					if(sMenuGroup != null) HTMLString.Append(sMenuGroup);
					HTMLString.Append("菜單</th></tr>");

					m_LoggedOn = LogonUser(m_NetUser, m_Domain, m_Password, LOGON32_LOGON_INTERACTIVE, LOGON32_PROVIDER_DEFAULT, ref m_Token);
					if(m_LoggedOn) {
						if(DuplicateToken(m_Token, SecurityImpersonation, ref m_TokenDuplicate) != 0) {
							m_TempWindowsIdentity = new WindowsIdentity(m_TokenDuplicate);
							m_ImpersonationContext = m_TempWindowsIdentity.Impersonate();
							if(m_ImpersonationContext != null) {
								//get menu from all_menu.ini file
								Win32INI Menu_INI = new Win32INI(sMenuSrcPath);

								SQLString.Remove(0,SQLString.Length);
								SQLString.Append("select map.IKEY_NO from MENUMAP map, MENUSOURCE_CFG src where map.ITYPE_ID=");
								SQLString.Append(sTypeID);
								SQLString.Append(" and map.IGROUP_ID=");
								SQLString.Append(sGpID);
								SQLString.Append(" and src.CPATH='");
								SQLString.Append(sMenuSrcPath);
								SQLString.Append("' order by map.IKEY_NO");
								m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
								while(m_SportsOleReader.Read()) {
									iIndex++;
									sValue = Menu_INI.GetValue(sSection, arrKeys[0] + m_SportsOleReader.GetInt32(0).ToString());
									HTMLString.Append("<tr><td>");
									HTMLString.Append(iIndex.ToString());
									HTMLString.Append("<input type=\"checkbox\" name=\"menuSelected\" value=\"");
									HTMLString.Append(sValue);
									HTMLString.Append("\"></td><td align=\"left\">");
									HTMLString.Append(sValue);
									HTMLString.Append("</td></tr>");
								}
								m_SportsOleReader.Close();
								m_SportsDBMgr.Close();
								m_ImpersonationContext.Undo();

								//write log
								m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
								m_SportsLog.SetFileName(0,LOGFILESUFFIX);
								m_SportsLog.Open();
								m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " PagerMenu.cs.GetMenuItems(): Impersonate (Retrieve " + sMenuType + " - " + sMenuGroup + "Menu) success.");
								m_SportsLog.Close();
							} else {
								m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
								m_SportsLog.SetFileName(0,LOGFILESUFFIX);
								m_SportsLog.Open();
								m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " PagerMenu.cs.GetMenuItems(): m_ImpersonationContext is null.");
								m_SportsLog.Close();
							}
						} else {
							m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
							m_SportsLog.SetFileName(0,LOGFILESUFFIX);
							m_SportsLog.Open();
							m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " PagerMenu.cs.GetMenuItems(): DuplicateToken error, code = " + Marshal.GetLastWin32Error());
							m_SportsLog.Close();
						}
					} else {
						m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
						m_SportsLog.SetFileName(0,LOGFILESUFFIX);
						m_SportsLog.Open();
						m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " PagerMenu.cs.GetMenuItems(): LogonUser error, code = " + Marshal.GetLastWin32Error());
						m_SportsLog.Close();
					}

					//select Target Path ID
					SQLString.Remove(0,SQLString.Length);
					SQLString.Append("select ITARGET_ID from MENUSRCTARGET_MAP where IPATH_ID=(select IPATH_ID from MENUSOURCE_CFG where CPATH='");
					SQLString.Append(sMenuSrcPath);
					SQLString.Append("') order by ITARGET_ID");
					m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
					while(m_SportsOleReader.Read()) {
						HTMLString.Append("<input type=\"hidden\" name=\"pathID\" value=\"");
						HTMLString.Append(m_SportsOleReader.GetInt32(0).ToString());
						HTMLString.Append("\">");
					}
					m_SportsOleReader.Close();
					m_SportsDBMgr.Close();

					HTMLString.Append("<input type=\"hidden\" name=\"typeID\" value=\"");
					HTMLString.Append(sTypeID);
					HTMLString.Append("\"><input type=\"hidden\" name=\"gpID\" value=\"");
					HTMLString.Append(sGpID);
					HTMLString.Append("\">");
				} else {
					HTMLString.Remove(0, HTMLString.Length);
					HTMLString.Append("<tr align=\"left\" style=\"background-color:#00bfff\"><th colspan=\"2\">沒有存取權限</th></tr>");
				}
			} catch(Exception ex) {
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " PagerMenu.cs.GetMenuItems(): " + ex.ToString());
				m_SportsLog.Close();
				HTMLString.Remove(0, HTMLString.Length);
				HTMLString.Append((string)HttpContext.Current.Application["accessErrorMsg"]);
			}

			return HTMLString.ToString();
		}

		public int Resend() {
			int iIndex = 0;
			string sRole;
			string[] arrRemotingPath;

			sRole = (string)HttpContext.Current.Session["user_role"];
			try {
				if(Convert.ToInt32(sRole) >= 988) {
					string sGpID;
					string sTypeID;
					string sSection;
					string[] arrMenu;
					string[] arrKeys;
					string[] arrPathID;
					string[] arrMsgType;
					char[] delimiter = new char[] {','};

					sGpID = HttpContext.Current.Request.Form["gpID"];
					sTypeID = HttpContext.Current.Request.Form["typeID"];
					sSection = (string)HttpContext.Current.Application["MenuSection"];
					arrKeys = (string[])HttpContext.Current.Application["MenuKeys"];
					arrMsgType = (string[])HttpContext.Current.Application["messageType"];
					arrRemotingPath = (string[])HttpContext.Current.Application["RemotingItems"];
					
					try {
						arrMenu = (string[])HttpContext.Current.Request.Form["menuSelected"].Split(delimiter);
					} catch(Exception) {
						arrMenu = new string[0];
					}
					try {
						arrPathID = (string[])HttpContext.Current.Request.Form["pathID"].Split(delimiter);
					} catch(Exception) {
						arrPathID = new string[0];
					}

					if(arrMenu.Length > 0) {	//resend menu if option was selected in PagerMenu.aspx
						m_LoggedOn = LogonUser(m_NetUser, m_Domain, m_Password, LOGON32_LOGON_INTERACTIVE, LOGON32_PROVIDER_DEFAULT, ref m_Token);
						if(m_LoggedOn) {
							if(DuplicateToken(m_Token, SecurityImpersonation, ref m_TokenDuplicate) != 0) {
								m_TempWindowsIdentity = new WindowsIdentity(m_TokenDuplicate);
								m_ImpersonationContext = m_TempWindowsIdentity.Impersonate();
								if(m_ImpersonationContext != null) {
									/*****************************
									 * GoGo Pager2 alert message *
									 *****************************/
									string sMenuGroup;
									string sCurrentTimestamp = null;
									string sBatchJob = null;
									string[] arrQueueNames;
									string[] arrMessageTypes;
									arrQueueNames = (string[])HttpContext.Current.Application["QueueItems"];
									arrMessageTypes = (string[])HttpContext.Current.Application["NotifyMessageTypes"];
									MessageClient msgClt = new MessageClient();
									msgClt.MessageType = arrMessageTypes[0];
									msgClt.MessagePath = arrQueueNames[0];
									//SportsMessage object message
									SportsMessage sptMsg = new SportsMessage();

									//Get Group of Menu
									SQLString.Remove(0,SQLString.Length);
									SQLString.Append("select CGROUP from MENUGROUP_CFG where IGROUP_ID=");
									SQLString.Append(sGpID);
									sMenuGroup = m_SportsDBMgr.ExecuteQueryString(SQLString.ToString());
									m_SportsDBMgr.Close();

									for(iIndex = 0; iIndex < arrMenu.Length; iIndex++) {
										sCurrentTimestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
										sBatchJob = DateTime.Now.ToString("yyMMddHHmmss_fffffff") + "." + HttpContext.Current.Session["user_name"].ToString() + "." + arrMsgType[6] + "." + iIndex.ToString() + ".ini";
										SQLString.Remove(0,SQLString.Length);
										SQLString.Append("insert into LOG_MENU (TIMEFLAG, SECTION, MENU, BATCHJOB) values ('");
										SQLString.Append(sCurrentTimestamp);
										SQLString.Append("','MENU','");
										SQLString.Append(arrMenu[iIndex]);
										SQLString.Append("','");
										SQLString.Append(sBatchJob);
										SQLString.Append("')");
										m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
										m_SportsDBMgr.Close();

										//Send Notify Message
										//Modified by Chapman, 19 Feb 2004
										sptMsg.IsTransaction = false;
										sptMsg.Body = sBatchJob;
										sptMsg.Timestamp = DateTime.Now.ToString("yyMMddHHmmss");
										sptMsg.AppID = "07";
										sptMsg.MsgID = "25";
										sptMsg.PathID = new string[0];
										for(int i = 0; i < arrPathID.Length; i++) {
											sptMsg.AddPathID(arrPathID[i]);
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
												m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " MSMQ ERROR:Pager Menu");
												m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " PagerMenu.cs.Resend(): Notify via MSMQ throws MessageQueueException:  " + mqEx.ToString());
												m_SportsLog.Close();
				
												//If MSMQ fail, notify via .NET Remoting
												msgClt.MessageType = arrMessageTypes[1];
												msgClt.MessagePath = arrRemotingPath[0];
												if(!msgClt.SendMessage((object)sptMsg)) {
													m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
													m_SportsLog.SetFileName(0,LOGFILESUFFIX);
													m_SportsLog.Open();
													m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Remoting ERROR:Pager Menu");
													m_SportsLog.Close();
												}
											}	catch(Exception ex) {
												m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
												m_SportsLog.SetFileName(0,LOGFILESUFFIX);
												m_SportsLog.Open();
												m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Remoting ERROR:Pager Menu");
												m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " PagerMenu.cs.Resend(): Notify via .NET Remoting throws exception: " + ex.ToString());
												m_SportsLog.Close();
											}							
										}	catch(Exception ex) {
											m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
											m_SportsLog.SetFileName(0,LOGFILESUFFIX);
											m_SportsLog.Open();
											m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " PagerMenu.cs.Resend(): Notify via MSMQ throws exception: " + ex.ToString());
											m_SportsLog.Close();
										}

										if(sMenuGroup.Trim().Equals("足球2")) {
											sCurrentTimestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
											sBatchJob = DateTime.Now.ToString("yyMMddHHmmss_fffffff") + "." + HttpContext.Current.Session["user_name"].ToString() + "." + arrMsgType[6] + ".soc2.ini";
											SQLString.Remove(0,SQLString.Length);
											SQLString.Append("insert into LOG_ADMINTASK (TIMEFLAG, SECTION, CMD, PARAM, BATCHJOB) values ('");
											SQLString.Append(sCurrentTimestamp);
											SQLString.Append("','COMMAND_','RESEND','SPT2_MENU','");
											SQLString.Append(sBatchJob);
											SQLString.Append("')");
											m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
											m_SportsDBMgr.Close();

											//Send Notify Message
											//Modified by Chapman, 19 Feb 2004
											sptMsg.IsTransaction = false;
											sptMsg.Body = sBatchJob;
											sptMsg.Timestamp = DateTime.Now.ToString("yyMMddHHmmss");
											sptMsg.AppID = "07";
											sptMsg.MsgID = "23";
											sptMsg.PathID = new string[0];
											for(int i = 0; i < arrPathID.Length; i++) {
												sptMsg.AddPathID(arrPathID[i]);
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
													m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " MSMQ ERROR:Pager Menu");
													m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " PagerMenu.cs.Resend(): Notify via MSMQ throws MessageQueueException:  " + mqEx.ToString());
													m_SportsLog.Close();
					
													//If MSMQ fail, notify via .NET Remoting
													msgClt.MessageType = arrMessageTypes[1];
													msgClt.MessagePath = arrRemotingPath[0];
													if(!msgClt.SendMessage((object)sptMsg)) {
														m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
														m_SportsLog.SetFileName(0,LOGFILESUFFIX);
														m_SportsLog.Open();
														m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Remoting ERROR:Pager Menu");
														m_SportsLog.Close();
													}
												}	catch(Exception ex) {
													m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
													m_SportsLog.SetFileName(0,LOGFILESUFFIX);
													m_SportsLog.Open();
													m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Remoting ERROR:Pager Menu");
													m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " PagerMenu.cs.Resend(): Notify via .NET Remoting throws exception: " + ex.ToString());
													m_SportsLog.Close();
												}							
											}	catch(Exception ex) {
												m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
												m_SportsLog.SetFileName(0,LOGFILESUFFIX);
												m_SportsLog.Open();
												m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " PagerMenu.cs.Resend(): Notify via MSMQ throws exception: " + ex.ToString());
												m_SportsLog.Close();
											}
										}	//generate dynamic menu for 足球2
									}

									m_ImpersonationContext.Undo();
								} else {
									m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
									m_SportsLog.SetFileName(0,LOGFILESUFFIX);
									m_SportsLog.Open();
									m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " PagerMenu.cs.Resend(): m_ImpersonationContext is null.");
									m_SportsLog.Close();
								}
							} else {
								m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
								m_SportsLog.SetFileName(0,LOGFILESUFFIX);
								m_SportsLog.Open();
								m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " PagerMenu.cs.Resend(): DuplicateToken error, code = " + Marshal.GetLastWin32Error());
								m_SportsLog.Close();
							}
						} else {
							m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
							m_SportsLog.SetFileName(0,LOGFILESUFFIX);
							m_SportsLog.Open();
							m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " PagerMenu.cs.Resend(): LogonUser error, code = " + Marshal.GetLastWin32Error());
							m_SportsLog.Close();
						}
					} else {	//nothing to be selected in PagerMenu.aspx
						iIndex = 0;
					}

					//write log
					m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
					m_SportsLog.SetFileName(0,LOGFILESUFFIX);
					m_SportsLog.Open();
					m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " PagerMenu.cs: resend " + iIndex.ToString() + " menus <Type=" + sTypeID + "> (" + HttpContext.Current.Session["user_name"] + ")");
					m_SportsLog.Close();
				} else {	//no right to resend menu
					iIndex = -99;
					//write log
					m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
					m_SportsLog.SetFileName(0,LOGFILESUFFIX);
					m_SportsLog.Open();
					m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " PagerMenu.cs: " + HttpContext.Current.Session["user_name"].ToString() + " did not allow to resend menu (" + HttpContext.Current.Session["user_name"] + ")");
					m_SportsLog.Close();
				}
			} catch(Exception ex) {
				iIndex = -1;
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " PagerMenu.cs.Resend(): " + ex.ToString());
				m_SportsLog.Close();
			}

			return iIndex;
		}
	}
}