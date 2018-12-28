/*
Objective:
Modify HKJC schedule config

Last updated:
02 May 2005 - Chris Tsui
3 June 2004 - Rita

C#.NET complier statement:
csc /t:library /out:..\bin\HKJCSchedule.dll /r:..\bin\DBManager.dll;..\bin\Files.dll;..\bin\Win32INI.dll;..\bin\MessageClient.dll;..\bin\SportsMessage.dll HKJCSchedule.cs


*/

using System;
using System.Collections;
using System.Collections.Specialized;
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
[assembly:AssemblyCopyright("(c) 2003 TDSL. All rights reserved. Created on 30 July 2003.")]
[assembly:AssemblyDescription("馬會機設定 -> 設定賽程")]
[assembly:AssemblyProduct("Sports.NET")]
[assembly:AssemblyTitle("Sports.NET DLL")]
[assembly:AssemblyVersion("1.1.*")]
namespace SportsUtil {
	public class HKJCSchedule {
		
		const string LOGFILESUFFIX = "log";
		DBManager m_SportsDBMgr;
		Files m_SportsLog;
		StringBuilder SQLString;	
		Encoding m_Big5Encoded;
		NameValueCollection taskNVC;
		MessageClient msgClt;
		string[] arrQueueNames;
		string[] arrMessageTypes;
		string[] arrRemotingPath;
		string m_Role;
		SportsMessage sptMsg;	//SportsMessage object message
		
		public const int LOGON32_LOGON_INTERACTIVE = 2;
		public const int LOGON32_PROVIDER_DEFAULT = 0;
		public const int SecurityImpersonation = 2;		

		const string MAIN = "MAIN";		
		const string REF_DATE = "REF_DATE";
		const string HK_DTM = "HK_DTM";
		const string INI_DTM = "INI_DTM";
		const string START_MATCH_DTM = "START_MATCH_DTM";
		const string END_MATCH_DTM = "END_MATCH_DTM";
		const string RESET_DTM = "RESET_DTM";
		const string MATCH_READY = "MATCH_READY";	
		
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


		public HKJCSchedule(string Connection) {
			m_SportsDBMgr = new DBManager();
			m_SportsDBMgr.ConnectionString = Connection;
			m_SportsLog = new Files();
			SQLString = new StringBuilder();
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

		public string Retrieve() {
			int iDate = 0;
			OleDbDataReader m_SportsOleReader;
			StringBuilder HTMLString = new StringBuilder();
			string sRefDate = "";
			string sHKDTM = "";
			string sINIDTM = "";
			string sStartMatchDTM = "";
			string sEndMatchDTM = "";
			string sResetDTM = "";
			string sMatchReady = "";
			
			try {
					//Modified by Rita, 3 June 2004				
					m_LoggedOn = LogonUser(m_NetUser, m_Domain, m_Password, LOGON32_LOGON_INTERACTIVE, LOGON32_PROVIDER_DEFAULT, ref m_Token);
					if(m_LoggedOn) {
						if(DuplicateToken(m_Token, SecurityImpersonation, ref m_TokenDuplicate) != 0) {
							m_TempWindowsIdentity = new WindowsIdentity(m_TokenDuplicate);
							m_ImpersonationContext = m_TempWindowsIdentity.Impersonate();
							if(m_ImpersonationContext != null) {
								Win32INI Config_INI = new Win32INI();
								Config_INI.FilePath = HttpContext.Current.Application["JCPrepSystemPathFilePath"].ToString();																
								sRefDate= Config_INI.GetValue(MAIN,REF_DATE);
								sHKDTM = Config_INI.GetValue(MAIN,HK_DTM);
								sINIDTM = Config_INI.GetValue(MAIN,INI_DTM);
								sStartMatchDTM = Config_INI.GetValue(MAIN,START_MATCH_DTM);							
								sEndMatchDTM = Config_INI.GetValue(MAIN,END_MATCH_DTM);	
								sResetDTM = Config_INI.GetValue(MAIN,RESET_DTM);
								sMatchReady = Config_INI.GetValue(MAIN,MATCH_READY);
								m_ImpersonationContext.Undo();

							} else {
								m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
								m_SportsLog.SetFileName(0,LOGFILESUFFIX);
								m_SportsLog.Open();
								m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " HKJCSchedule.cs.Retrieve(): m_ImpersonationContext is null.");
								m_SportsLog.Close();
							}
						} else {
							m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
							m_SportsLog.SetFileName(0,LOGFILESUFFIX);
							m_SportsLog.Open();
							m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " HKJCSchedule.cs.Retrieve(): DuplicateToken error, code = " + Marshal.GetLastWin32Error());
							m_SportsLog.Close();
						}
					} else {
						m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
						m_SportsLog.SetFileName(0,LOGFILESUFFIX);
						m_SportsLog.Open();
						m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " HKJCSchedule.cs.Retrieve(): LogonUser error, code = " + Marshal.GetLastWin32Error());
						m_SportsLog.Close();
					}
					
					
				HTMLString.Append("<table border=\"1\" width=\"50%\" style=\"font: 10pt verdana\">");
				HTMLString.Append("<tr><td colspan=6>");
				
				HTMLString.Append("<table border=\"0\" width=\"100%\" style=\"font: 10pt verdana\">");
				HTMLString.Append("<tr><th style=\"background-color:#FEEAB6\" align=\"right\">");
				HTMLString.Append("參考日子時間(REF_DATE)");
				HTMLString.Append("</th><td >");
				HTMLString.Append(sRefDate);
				HTMLString.Append("</td></tr>");
				HTMLString.Append("<tr><th style=\"background-color:#FEEAB6\" align=\"right\">");
				HTMLString.Append("清除及轉移賽事、比數至賽果時間(HK_DTM)");
				HTMLString.Append("</th><td>");
				HTMLString.Append(sHKDTM);
				HTMLString.Append("</td></tr>");
				HTMLString.Append("<tr><th style=\"background-color:#FEEAB6\" align=\"right\">");
				HTMLString.Append("啟動時間(INI_DTM)");				
				HTMLString.Append("</th><td >");			
				HTMLString.Append(sINIDTM);
				HTMLString.Append("</td></tr>");
				HTMLString.Append("<tr><th style=\"background-color:#FEEAB6\" align=\"right\">");
				HTMLString.Append("賽程開始時間(START_MATCH_DTM)");	
				HTMLString.Append("</th><td >");
				HTMLString.Append(sStartMatchDTM);
				HTMLString.Append("</td></tr>");
				HTMLString.Append("<tr><th style=\"background-color:#FEEAB6\" align=\"right\">");
				HTMLString.Append("賽程終止時間(END_MATCH_DTM)");	
				HTMLString.Append("</th><td >");
				HTMLString.Append(sEndMatchDTM);
				HTMLString.Append("</td></tr>");
				HTMLString.Append("<tr><th style=\"background-color:#FEEAB6\" align=\"right\">");
				HTMLString.Append("重設資料時間(RESET_DTM)");	
				HTMLString.Append("</th><td >");
				HTMLString.Append(sResetDTM);
				HTMLString.Append("</td></tr>");
				HTMLString.Append("<tr><th style=\"background-color:#FEEAB6\" align=\"right\">");
				HTMLString.Append("傳送資訊狀況(MATCH_READY)");	
				HTMLString.Append("</th><td>");
				HTMLString.Append(sMatchReady);
				HTMLString.Append("</td></tr>");		
				HTMLString.Append("</table>");
						
				HTMLString.Append("</td></tr>");			
				
				HTMLString.Append("<tr align=\"center\" style=\"background-color:#FFD700\">");
				HTMLString.Append("<th style=\"background-color:#7CFC00\">馬會足球</th>");
				HTMLString.Append("<th colspan=\"5\">賽程設定<br><font color=red>請輸入整數，如09:30=930；23:35=2335。</font></th>");
				HTMLString.Append("</tr>");
				HTMLString.Append("<tr style=\"background-color:#FFD700\" align=\"center\">");
				HTMLString.Append("<th style=\"background-color:#7CFC00\">本月日期</th>");
				HTMLString.Append("<th>提供日數</th>");				
				HTMLString.Append("<th>當日啟動時間(國際時間)</th>");
				HTMLString.Append("<th>翌日清除時間(國際時間)</th>");
				HTMLString.Append("<th>賽程終止時間(國際時間)</th>");
				HTMLString.Append("<th>重設資料時間(國際時間)</th>");
				HTMLString.Append("</tr>");			
				
								
				// modify end 060427	
				SQLString.Remove(0,SQLString.Length);
				SQLString.Append("select IGEN_DAY_CNT, INEXT_DATE_HK_TM, ICURRENT_DATE_INI_TM, IEND_MATCH_TM, IRESET_TM from MATCH_SCHEDULE order by IDATE");
				m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
				while(m_SportsOleReader.Read()) {
					iDate++;
					HTMLString.Append("<tr align=\"center\"><td>");
					HTMLString.Append(iDate.ToString());
					HTMLString.Append("</td><td><input name=\"dayOfSchedule\" value=\"");
					if(!m_SportsOleReader.IsDBNull(0)) HTMLString.Append(m_SportsOleReader.GetInt32(0).ToString());
					HTMLString.Append("\" maxlength=\"2\" size=\"1\" onChange=\"DateValidity(");
					HTMLString.Append((iDate-1).ToString());					
					HTMLString.Append(")\"></td><td><input name=\"activate\" value=\"");
					if(!m_SportsOleReader.IsDBNull(2)) HTMLString.Append(m_SportsOleReader.GetInt32(2).ToString());
					HTMLString.Append("\" maxlength=\"4\" size=\"3\" onChange=\"ActivateValidity(");
					HTMLString.Append((iDate-1).ToString());
					HTMLString.Append(")\"></td><td><input name=\"housekeep\" value=\"");
					if(!m_SportsOleReader.IsDBNull(1)) HTMLString.Append(m_SportsOleReader.GetInt32(1).ToString());
					HTMLString.Append("\" maxlength=\"4\" size=\"3\" onChange=\"HousekeepValidity(");
					HTMLString.Append((iDate-1).ToString());
					HTMLString.Append(")\"></td><td><input name=\"endMatch\" value=\"");
					if(!m_SportsOleReader.IsDBNull(1)) HTMLString.Append(m_SportsOleReader.GetInt32(3).ToString());
					HTMLString.Append("\" maxlength=\"4\" size=\"3\" onChange=\"EndMatchValidity(");
					HTMLString.Append((iDate-1).ToString());
					HTMLString.Append(")\"></td><td><input name=\"resetTime\" value=\"");
					if(!m_SportsOleReader.IsDBNull(1)) HTMLString.Append(m_SportsOleReader.GetInt32(4).ToString());
					HTMLString.Append("\" maxlength=\"4\" size=\"3\" onChange=\"ResetTimeValidity(");
					HTMLString.Append((iDate-1).ToString());
					HTMLString.Append(")\"></td></tr>");
				}
				m_SportsDBMgr.Close();
				m_SportsOleReader.Close();
				m_SportsDBMgr.Dispose();
			}	catch(Exception ex) {
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " HKJCSchedule.cs.InitFields(): " + ex.ToString());
				m_SportsLog.Close();
				HTMLString.Remove(0,HTMLString.Length);
				HTMLString.Append((string)HttpContext.Current.Application["accessErrorMsg"]);
			}
			return HTMLString.ToString();
		}

		public int Modify() {
			int iUpdIndex = 0;
			int iTotalDays;
			char[] delimiter = new char[] {','};
			string[] arrDayOfSchedule;
			string[] arrHousekeep;
			string[] arrActivate;
			string[] arrEndMatch;
			string[] arrResetTime;
			string sTaskID;
			int iMsgBodyLength = 0;
			byte[] arrByteOfMSMQBody;

			arrDayOfSchedule = HttpContext.Current.Request.Form["dayOfSchedule"].Split(delimiter);
			arrHousekeep = HttpContext.Current.Request.Form["housekeep"].Split(delimiter);
			arrActivate = HttpContext.Current.Request.Form["activate"].Split(delimiter);
			arrEndMatch = HttpContext.Current.Request.Form["endMatch"].Split(delimiter);
			arrResetTime = HttpContext.Current.Request.Form["resetTime"].Split(delimiter);
			iTotalDays = arrDayOfSchedule.Length;
			try {
				for(iUpdIndex = 0; iUpdIndex < iTotalDays; iUpdIndex++) {
					SQLString.Remove(0,SQLString.Length);
					SQLString.Append("update MATCH_SCHEDULE set IGEN_DAY_CNT=");
					if(arrDayOfSchedule[iUpdIndex] != null) {
						if(!arrDayOfSchedule[iUpdIndex].Trim().Equals("")) {
							SQLString.Append(Convert.ToInt32(arrDayOfSchedule[iUpdIndex]).ToString());	//ensure it is integer format
						} else {
							SQLString.Append("1");
						}
					} else {
						SQLString.Append("1");
					}
					SQLString.Append(", INEXT_DATE_HK_TM=");
					if(arrHousekeep[iUpdIndex] != null) {
						if(!arrHousekeep[iUpdIndex].Trim().Equals("")) {
							SQLString.Append(Convert.ToInt32(arrHousekeep[iUpdIndex]).ToString());	//ensure it is integer format
						} else {
							SQLString.Append("null");
						}
					} else {
						SQLString.Append("null");
					}
					SQLString.Append(", ICURRENT_DATE_INI_TM=");
					if(arrActivate[iUpdIndex] != null) {
						if(!arrActivate[iUpdIndex].Trim().Equals("")) {
							SQLString.Append(Convert.ToInt32(arrActivate[iUpdIndex]).ToString());	//ensure it is integer format
						} else {
							SQLString.Append("null");
						}
					} else {
						SQLString.Append("null");
					}
					SQLString.Append(", IEND_MATCH_TM=");
					if(arrEndMatch[iUpdIndex] != null) {
						if(!arrEndMatch[iUpdIndex].Trim().Equals("")) {
							SQLString.Append(Convert.ToInt32(arrEndMatch[iUpdIndex]).ToString());	//ensure it is integer format
						} else {
							SQLString.Append("null");
						}
					} else {
						SQLString.Append("null");
					}
					SQLString.Append(", IRESET_TM=");
					if(arrResetTime[iUpdIndex] != null) {
						if(!arrResetTime[iUpdIndex].Trim().Equals("")) {
							SQLString.Append(Convert.ToInt32(arrResetTime[iUpdIndex]).ToString());	//ensure it is integer format
						} else {
							SQLString.Append("null");
						}
					} else {
						SQLString.Append("null");
					}
					SQLString.Append(" where IDATE=");
					SQLString.Append((iUpdIndex+1).ToString());
					m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
					m_SportsDBMgr.Close();
				}
				m_SportsDBMgr.Dispose();
				
				//Modified by Rita, 3 June 2004				
				/*****************************
				* HKJC soccer alert message *
				*****************************/
				if(iTotalDays>0){								
						
						sTaskID = taskNVC["schedule"];						
						arrByteOfMSMQBody = m_Big5Encoded.GetBytes(sTaskID);
						iMsgBodyLength = arrByteOfMSMQBody.Length;
						sptMsg.Body = iMsgBodyLength.ToString("D3") + sTaskID;
						sptMsg.Timestamp = DateTime.Now.ToString("yyMMddHHmmss");
						sptMsg.IsTransaction = false;
						sptMsg.AppID = "08";
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
								m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " MSMQ ERROR: HKJC Schedule");
								m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " HKJCSchedule.cs.Modify(): Notify via MSMQ throws MessageQueueException:  " + mqEx.ToString());
								m_SportsLog.Close();
						
								//If MSMQ fail, notify via .NET Remoting
								msgClt.MessageType = arrMessageTypes[1];
								msgClt.MessagePath = arrRemotingPath[0];
								if(!msgClt.SendMessage((object)sptMsg)) {
									m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
									m_SportsLog.SetFileName(0,LOGFILESUFFIX);
									m_SportsLog.Open();
									m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Remoting ERROR: HKJC Schedule");
									m_SportsLog.Close();
								}
							}	catch(Exception ex) {
								m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
								m_SportsLog.SetFileName(0,LOGFILESUFFIX);
								m_SportsLog.Open();
								m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Remoting ERROR: HKJC Schedule");
								m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " HKJCSchedule.cs.Modify(): Notify via .NET Remoting throws exception: " + ex.ToString());
								m_SportsLog.Close();
							}							
						}	catch(Exception ex) {
							m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
							m_SportsLog.SetFileName(0,LOGFILESUFFIX);
							m_SportsLog.Open();
							m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " HKJCSchedule.cs.Modify(): Notify via MSMQ throws exception: " + ex.ToString());
							m_SportsLog.Close();
						}		
				}
				//Modified end

				//write log
				m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " HKJCSchedule.cs: Modify schedule (" + HttpContext.Current.Session["user_name"] + ")");
				m_SportsLog.Close();
			} catch(Exception ex) {
				iUpdIndex = -1;
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " HKJCSchedule.cs.Modify(): " + ex.ToString());
				m_SportsLog.Close();
			}

			return iUpdIndex;
		}
	}
}