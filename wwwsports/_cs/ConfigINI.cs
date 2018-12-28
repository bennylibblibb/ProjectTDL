/*
Objective:
Get and set config setting from ini file (AsianFtpServer.ini) such as Rush Hour, COM availability

Last updated:
10 June 2004, Chapman Choi
10 June 2004, Double check for read value from network
6 April 2004, Update an additional key REF_DATE

C#.NET complier statement:
csc /t:library /out:..\bin\ConfigINI.dll /r:..\bin\Files.dll;..\bin\Win32INI.dll ConfigINI.cs
*/

using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Web;
using TDL.IO;

[assembly:AssemblyCompany("TDSL")]
[assembly:AssemblyCopyright("(c) 2003 TDSL. All rights reserved.")]
[assembly:AssemblyDescription("系統設定")]
[assembly:AssemblyProduct("Sports.NET")]
[assembly:AssemblyTitle("Sports.NET DLL")]
[assembly:AssemblyVersion("3.0.*")]
namespace SportsUtil {
	public class ConfigINI {
		public const int LOGON32_LOGON_INTERACTIVE = 2;
		public const int LOGON32_PROVIDER_DEFAULT = 0;
		public const int SecurityImpersonation = 2;
		//const int MAXRETRY = 30;
		const string LOGFILESUFFIX = "log";
		const string SYS_PARA = "SYS_PARA";
		const string RUSHHOUR_START_TIME = "RUSHHOUR_START_TIME";
		const string RUSHHOUR_END_TIME = "RUSHHOUR_END_TIME";
		const string COMSEND_AVA = "COMSEND_AVA";
		const string HK_COM_SENT = "HK_COM_SENT";
		const string ST_COM_SENT = "ST_COM_SENT";
		const string REF_DATE = "REF_DATE";
		const string UPDATE_REF_DATE_TM = "UPDATE_REF_DATE_TM";
		const string ODD_HK_TIME = "ODD_HK_TIME";
		const string GOAL_HK_TIME = "GOAL_HK_TIME";
		const string DB_BACKUP_TIME1 = "DB_BACKUP_TIME1";
		const string DB_BACKUP_TIME2 = "DB_BACKUP_TIME2";
		const string MAIN = "MAIN";
		const string HK_SEND = "HK_SEND";
		const string MAC_SEND = "MAC_SEND";
		const string ST_SEND = "ST_SEND";
		const string CHK_SEND = "CHK_SEND";
		Files m_SportsLog;
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

		public ConfigINI() {
			m_SportsLog = new Files();
		}

		public string CurrentDateTime {
			get {
				return DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss");
			}
		}

		public string GetINISetting() {
			//int iRetried = 0;
			string sRole = (string)HttpContext.Current.Session["user_role"];
			StringBuilder HTMLString = new StringBuilder();

			if(Convert.ToInt32(sRole) >= 11) {
				try {
					string sRushStart = "";
					string sRushEnd = "";
					string sComAva = "";
					string sHKCom = "";
					string sSTCom = "";
					string sRefDate = "";
					string sRefDateTime = "";
					string sOddHKTime = "";
					string sGoalHKTime = "";
					string sDBBK1 = "";
					string sDBBK2 = "";
					string sHKIPSend = "";
					string sMCIPSend = "";
					string sSTIPSend = "";
					string sCHIPSend = "";

					m_LoggedOn = LogonUser(m_NetUser, m_Domain, m_Password, LOGON32_LOGON_INTERACTIVE, LOGON32_PROVIDER_DEFAULT, ref m_Token);
					if(m_LoggedOn) {
						if(DuplicateToken(m_Token, SecurityImpersonation, ref m_TokenDuplicate) != 0) {
							m_TempWindowsIdentity = new WindowsIdentity(m_TokenDuplicate);
							m_ImpersonationContext = m_TempWindowsIdentity.Impersonate();
							if(m_ImpersonationContext != null) {
								Win32INI Config_INI = new Win32INI();
/*
								while(iRetried < MAXRETRY) {
*/
									Config_INI.FilePath = HttpContext.Current.Application["SoccerINIFilePath"].ToString();
									sRushStart = Config_INI.GetValue(SYS_PARA,RUSHHOUR_START_TIME);
									sRushEnd = Config_INI.GetValue(SYS_PARA,RUSHHOUR_END_TIME);
									sComAva = Config_INI.GetValue(SYS_PARA,COMSEND_AVA);
									sHKCom = Config_INI.GetValue(SYS_PARA,HK_COM_SENT);
									sSTCom = Config_INI.GetValue(SYS_PARA,ST_COM_SENT);
									sRefDate = Config_INI.GetValue(SYS_PARA,REF_DATE);
									sRefDateTime = Config_INI.GetValue(SYS_PARA,UPDATE_REF_DATE_TM);
									sOddHKTime = Config_INI.GetValue(SYS_PARA,ODD_HK_TIME);
									sGoalHKTime = Config_INI.GetValue(SYS_PARA,GOAL_HK_TIME);
									sDBBK1 = Config_INI.GetValue(SYS_PARA,DB_BACKUP_TIME1);
									sDBBK2 = Config_INI.GetValue(SYS_PARA,DB_BACKUP_TIME2);

									Config_INI.FilePath = HttpContext.Current.Application["IPMuxINIFilePath"].ToString();
									sHKIPSend = Config_INI.GetValue(MAIN,HK_SEND);
									sMCIPSend = Config_INI.GetValue(MAIN,MAC_SEND);
									sSTIPSend = Config_INI.GetValue(MAIN,ST_SEND);
									sCHIPSend = Config_INI.GetValue(MAIN,CHK_SEND);
/*
									iRetried++;
									if(!sRushStart.Trim().Equals("")) break;
									else System.Threading.Thread.Sleep(100);
								}
*/
								m_ImpersonationContext.Undo();
							} else {
								m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
								m_SportsLog.SetFileName(0,LOGFILESUFFIX);
								m_SportsLog.Open();
								m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " ConfigINI.cs.GetINISetting(): m_ImpersonationContext is null.");
								m_SportsLog.Close();
							}
						} else {
							m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
							m_SportsLog.SetFileName(0,LOGFILESUFFIX);
							m_SportsLog.Open();
							m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " ConfigINI.cs.GetINISetting(): DuplicateToken error, code = " + Marshal.GetLastWin32Error());
							m_SportsLog.Close();
						}
					} else {
						m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
						m_SportsLog.SetFileName(0,LOGFILESUFFIX);
						m_SportsLog.Open();
						m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " ConfigINI.cs.GetINISetting(): LogonUser error, code = " + Marshal.GetLastWin32Error());
						m_SportsLog.Close();
					}
/*
					if((iRetried == MAXRETRY) && sRushStart.Trim().Equals("")) {
						HTMLString.Append("<tr><th colspan=\"2\" style=\"background-color:#ff0000\" align=\"center\">讀取數值出現問題，請重試！</th></tr>");
						m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
						m_SportsLog.SetFileName(0,LOGFILESUFFIX);
						m_SportsLog.Open();
						m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " ConfigINI.cs: Get INI value failed");
						m_SportsLog.Close();
					} else {
						m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
						m_SportsLog.SetFileName(0,LOGFILESUFFIX);
						m_SportsLog.Open();
						m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " ConfigINI.cs: Get INI value success in " + iRetried + " times");
						m_SportsLog.Close();
					}
*/
					HTMLString.Append("<tr><th style=\"background-color:#00bfff\" align=\"right\">繁忙時間開始(RUSHHOUR_START_TIME):</th>");
					HTMLString.Append("<td align=\"center\"><input type=\"text\" name=\"RushHourStart\" value=\"");
					HTMLString.Append(sRushStart);
					HTMLString.Append("\" maxlength=\"4\" size=\"2\" onChange=\"RushStartChanged(");
					HTMLString.Append(sRushStart);
					HTMLString.Append(")\"></td></tr>");
					HTMLString.Append("<tr><th style=\"background-color:#00bfff\" align=\"right\">繁忙時間結束(RUSHHOUR_END_TIME):</th>");
					HTMLString.Append("<td align=\"center\"><input type=\"text\" name=\"RushHourEnd\" value=\"");
					HTMLString.Append(sRushEnd);
					HTMLString.Append("\" maxlength=\"4\" size=\"2\" onChange=\"RushEndChanged(");
					HTMLString.Append(sRushEnd);
					HTMLString.Append(")\"></td></tr>");
					if(Convert.ToInt32(sRole) >= 988) {
						HTMLString.Append("<tr><th style=\"background-color:#00bfff\" align=\"right\">允許發送到傳呼機(COMSEND_AVA):</th>");
						HTMLString.Append("<td align=\"center\"><input type=\"checkbox\" name=\"ComSendAva\" value=\"1\" ");
						if(sComAva.Equals("1")) HTMLString.Append("checked ");
						HTMLString.Append("onClick=\"checkedAlert()\"></td></tr>");
						HTMLString.Append("<tr><th style=\"background-color:#00bfff\" align=\"right\">啟動香港頻道(HK_COM_SENT):</th>");
						HTMLString.Append("<td align=\"center\"><input type=\"checkbox\" name=\"HKCom\" value=\"1\" ");
						if(sHKCom.Equals("1")) HTMLString.Append("checked ");
						HTMLString.Append("onClick=\"checkedAlert()\"></td></tr>");
						HTMLString.Append("<tr><th style=\"background-color:#00bfff\" align=\"right\">啟動汕頭頻道(ST_COM_SENT):</th>");
						HTMLString.Append("<td align=\"center\"><input type=\"checkbox\" name=\"STCom\" value=\"1\" ");
						if(sSTCom.Equals("1")) HTMLString.Append("checked ");
						HTMLString.Append("onClick=\"checkedAlert()\"></td></tr>");
					}

					//HouseKeep setting
					HTMLString.Append("<tr><th style=\"background-color:#40E0D0\" align=\"right\">Housekeep(清除及轉移賽事、比數至賽果)時間<font color=\"red\">[亞洲體育提供]</font>:</th>");
					HTMLString.Append("<td align=\"center\"><input type=\"text\" name=\"HKT\" value=\"");
					HTMLString.Append(sOddHKTime);
					HTMLString.Append("\" maxlength=\"4\" size=\"2\" onChange=\"HousekeepChanged(");
					HTMLString.Append(sOddHKTime);
					HTMLString.Append(")\"></td></tr>");
					if(Convert.ToInt32(sRole) >= 988) {
						//Reference Date
						HTMLString.Append("<tr><th style=\"background-color:#40E0D0\" align=\"right\">Housekeep (REF_DATE):</th>");
						HTMLString.Append("<td align=\"center\"><input type=\"text\" name=\"REFDATE\" value=\"");
						HTMLString.Append(sRefDate);
						HTMLString.Append("\" maxlength=\"8\" size=\"6\" onChange=\"HousekeepRefDateChanged(");
						HTMLString.Append(sRefDate);
						HTMLString.Append(")\"></td></tr>");

						HTMLString.Append("<tr><th colspan=\"2\" style=\"background-color:#40E0D0\" align=\"left\"><font color=\"red\">HKT</font>代表Housekeep時間，更新<font color=\"red\">HKT</font>會自動更新以下5項數值:</th></tr>");
						HTMLString.Append("<tr><th style=\"background-color:#40E0D0\" align=\"right\">更新圖表參考日子時間<font color=\"red\">[HKT + 30分鐘]</font> (UPDATE_REF_DATE_TM):</th><td align=\"center\">");
						HTMLString.Append(sRefDateTime);
						HTMLString.Append("</td></tr>");
						HTMLString.Append("<tr><th style=\"background-color:#40E0D0\" align=\"right\">清除及轉移賽事時間<font color=\"red\">[等於HKT]</font> (ODD_HK_TIME):</th><td align=\"center\">");
						HTMLString.Append(sOddHKTime);
						HTMLString.Append("</td></tr>");
						HTMLString.Append("<tr><th style=\"background-color:#40E0D0\" align=\"right\">清除及轉移比數時間<font color=\"red\">[等於HKT]</font> (GOAL_HK_TIME):</th><td align=\"center\">");
						HTMLString.Append(sGoalHKTime);
						HTMLString.Append("</td></tr>");
						HTMLString.Append("<tr><th style=\"background-color:#40E0D0\" align=\"right\">第一次自動備份數據庫時間<font color=\"red\">[HKT - 10分鐘]</font> (DB_BACKUP_TIME1):</th><td align=\"center\">");
						HTMLString.Append(sDBBK1);
						HTMLString.Append("</td></tr>");
						HTMLString.Append("<tr><th style=\"background-color:#40E0D0\" align=\"right\">第二次自動備份數據庫時間<font color=\"red\">[HKT + 10分鐘]</font> (DB_BACKUP_TIME2):</th><td align=\"center\">");
						HTMLString.Append(sDBBK2);
						HTMLString.Append("</td></tr>");

						//Send Flag Control
						HTMLString.Append("<tr><th style=\"background-color:#DA70D6\" align=\"right\">IPMuxAdapter - 香港頻:</th>");
						HTMLString.Append("<td align=\"center\"><input type=\"checkbox\" name=\"HKIPSend\" value=\"1\" ");
						if(sHKIPSend.Equals("1")) HTMLString.Append("checked ");
						HTMLString.Append("onClick=\"checkedAlert()\"></td></tr>");
						HTMLString.Append("<tr><th style=\"background-color:#DA70D6\" align=\"right\">IPMuxAdapter - 澳門頻:</th>");
						HTMLString.Append("<td align=\"center\"><input type=\"checkbox\" name=\"MCIPSend\" value=\"1\" ");
						if(sMCIPSend.Equals("1")) HTMLString.Append("checked ");
						HTMLString.Append("onClick=\"checkedAlert()\"></td></tr>");
						HTMLString.Append("<tr><th style=\"background-color:#DA70D6\" align=\"right\">IPMuxAdapter - 汕頭頻:</th>");
						HTMLString.Append("<td align=\"center\"><input type=\"checkbox\" name=\"STIPSend\" value=\"1\" ");
						if(sSTIPSend.Equals("1")) HTMLString.Append("checked ");
						HTMLString.Append("onClick=\"checkedAlert()\"></td></tr>");
						HTMLString.Append("<tr><th style=\"background-color:#DA70D6\" align=\"right\">IPMuxAdapter - 中港頻:</th>");
						HTMLString.Append("<td align=\"center\"><input type=\"checkbox\" name=\"CHIPSend\" value=\"1\" ");
						if(sCHIPSend.Equals("1")) HTMLString.Append("checked ");
						HTMLString.Append("onClick=\"checkedAlert()\"></td></tr>");
					}

					//Hidden field for Housekeep time and ref date
					HTMLString.Append("<input type=\"hidden\" name=\"HKT_OLD\" value=\"");
					HTMLString.Append(sOddHKTime);
					HTMLString.Append("\"><input type=\"hidden\" name=\"REFDATE_OLD\" value=\"");
					HTMLString.Append(sRefDate);
					HTMLString.Append("\">");
				} catch(Exception ex) {
					m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
					m_SportsLog.SetFileName(0,LOGFILESUFFIX);
					m_SportsLog.Open();
					m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " ConfigINI.cs.GetINISetting(): " + ex.ToString());
					m_SportsLog.Close();
					HTMLString.Remove(0,HTMLString.Length);
					HTMLString.Append("<tr><th colspan=\"2\">");
					HTMLString.Append((string)HttpContext.Current.Application["accessErrorMsg"]);
					HTMLString.Append("</th></tr>");
				}
			} else {
				HTMLString.Remove(0,HTMLString.Length);
				HTMLString.Append("<tr><th colspan=\"2\">沒有檢視權限</th></tr>");
			}

			return HTMLString.ToString();
		}

		public int SetINISetting() {
			int iRecUpd = 0;
			string sRole;
			string SoccerINIFilePath = HttpContext.Current.Application["SoccerINIFilePath"].ToString();
			string WebServAppINIFilePath = HttpContext.Current.Application["WebServAppINIFilePath"].ToString();
			string IPMuxINIFilePath = HttpContext.Current.Application["IPMuxINIFilePath"].ToString();
			DateTime dtHK;

			sRole = (string)HttpContext.Current.Session["user_role"];
			if(Convert.ToInt32(sRole) >= 11) {
				string sRushStart, sRushEnd, sComAva, sHKCom, sSTCom;
				string sOldHKT, sOldRefDate, sHKT, sRefDate, sRefDateTime, sOddHKTime, sGoalHKTime, sDBBK1, sDBBK2;
				string sHKIPSend, sMCIPSend, sSTIPSend, sCHIPSend;

				try {
					sOldHKT = HttpContext.Current.Request.Form["HKT_OLD"];
					sOldRefDate = HttpContext.Current.Request.Form["REFDATE_OLD"];
					sRefDate = HttpContext.Current.Request.Form["REFDATE"];
					if(sRefDate == null) sRefDate = sOldRefDate;
					if(sRefDate.Trim().Equals("")) sRefDate = sOldRefDate;
					sHKT = HttpContext.Current.Request.Form["HKT"];
					dtHK = Convert.ToDateTime(sHKT.Substring(0,2) + ":" + sHKT.Substring(2) + ":00");
 					sRefDateTime = dtHK.AddMinutes(30).ToString("HHmm");
					sOddHKTime = sHKT;
					sGoalHKTime = sHKT;
					sDBBK1 = dtHK.AddMinutes(-10).ToString("HHmm");
					sDBBK2 = dtHK.AddMinutes(10).ToString("HHmm");

					sRushStart = HttpContext.Current.Request.Form["RushHourStart"];
					sRushEnd = HttpContext.Current.Request.Form["RushHourEnd"];
					sComAva = HttpContext.Current.Request.Form["ComSendAva"];
					if(sComAva == null) {
						sComAva = "0";
					}	else {
						if(sComAva.Equals("")) sComAva = "0";
					}

					sHKCom = HttpContext.Current.Request.Form["HKCom"];
					if(sHKCom == null) {
						sHKCom = "0";
					}	else {
						if(sHKCom.Equals("")) sHKCom = "0";
					}

					sSTCom = HttpContext.Current.Request.Form["STCom"];
					if(sSTCom == null) {
						sSTCom = "0";
					}	else {
						if(sSTCom.Equals("")) sSTCom = "0";
					}

					sHKIPSend = HttpContext.Current.Request.Form["HKIPSend"];
					if(sHKIPSend == null) {
						sHKIPSend = "0";
					}	else {
						if(sHKIPSend.Equals("")) sHKIPSend = "0";
					}

					sMCIPSend = HttpContext.Current.Request.Form["MCIPSend"];
					if(sMCIPSend == null) {
						sMCIPSend = "0";
					}	else {
						if(sMCIPSend.Equals("")) sMCIPSend = "0";
					}

					sSTIPSend = HttpContext.Current.Request.Form["STIPSend"];
					if(sSTIPSend == null) {
						sSTIPSend = "0";
					}	else {
						if(sSTIPSend.Equals("")) sSTIPSend = "0";
					}

					sCHIPSend = HttpContext.Current.Request.Form["CHIPSend"];
					if(sCHIPSend == null) {
						sCHIPSend = "0";
					}	else {
						if(sCHIPSend.Equals("")) sCHIPSend = "0";
					}

					m_LoggedOn = LogonUser(m_NetUser, m_Domain, m_Password, LOGON32_LOGON_INTERACTIVE, LOGON32_PROVIDER_DEFAULT, ref m_Token);
					if(m_LoggedOn) {
						if(DuplicateToken(m_Token, SecurityImpersonation, ref m_TokenDuplicate) != 0) {
							m_TempWindowsIdentity = new WindowsIdentity(m_TokenDuplicate);
							m_ImpersonationContext = m_TempWindowsIdentity.Impersonate();
							if(m_ImpersonationContext != null) {
								Win32INI Config_INI = new Win32INI();
								Config_INI.FilePath = SoccerINIFilePath;
								Config_INI.SetValue(SYS_PARA,RUSHHOUR_START_TIME,sRushStart);
								Config_INI.SetValue(SYS_PARA,RUSHHOUR_END_TIME,sRushEnd);
								Config_INI.SetValue(SYS_PARA,UPDATE_REF_DATE_TM,sRefDateTime);
								Config_INI.SetValue(SYS_PARA,ODD_HK_TIME,sOddHKTime);
								Config_INI.SetValue(SYS_PARA,GOAL_HK_TIME,sGoalHKTime);
								Config_INI.SetValue(SYS_PARA,DB_BACKUP_TIME1,sDBBK1);
								Config_INI.SetValue(SYS_PARA,DB_BACKUP_TIME2,sDBBK2);
								if(Convert.ToInt32(sRole) >= 988) {
									Config_INI.SetValue(SYS_PARA,REF_DATE,sRefDate);
									Config_INI.SetValue(SYS_PARA,COMSEND_AVA,sComAva);
									Config_INI.SetValue(SYS_PARA,HK_COM_SENT,sHKCom);
									Config_INI.SetValue(SYS_PARA,ST_COM_SENT,sSTCom);
								}

								Config_INI.FilePath = WebServAppINIFilePath;
								Config_INI.SetValue(SYS_PARA,RUSHHOUR_START_TIME,sRushStart);
								Config_INI.SetValue(SYS_PARA,RUSHHOUR_END_TIME,sRushEnd);
								Config_INI.SetValue(SYS_PARA,UPDATE_REF_DATE_TM,sRefDateTime);
								Config_INI.SetValue(SYS_PARA,ODD_HK_TIME,sOddHKTime);
								Config_INI.SetValue(SYS_PARA,GOAL_HK_TIME,sGoalHKTime);
								Config_INI.SetValue(SYS_PARA,DB_BACKUP_TIME1,sDBBK1);
								Config_INI.SetValue(SYS_PARA,DB_BACKUP_TIME2,sDBBK2);
								if(Convert.ToInt32(sRole) >= 988) {
									Config_INI.SetValue(SYS_PARA,REF_DATE,sRefDate);
									Config_INI.SetValue(SYS_PARA,COMSEND_AVA,sComAva);
									Config_INI.SetValue(SYS_PARA,HK_COM_SENT,sHKCom);
									Config_INI.SetValue(SYS_PARA,ST_COM_SENT,sSTCom);
								}

								if(Convert.ToInt32(sRole) >= 988) {
									Config_INI.FilePath = IPMuxINIFilePath;
									Config_INI.SetValue(MAIN,HK_SEND,sHKIPSend);
									Config_INI.SetValue(MAIN,MAC_SEND,sMCIPSend);
									Config_INI.SetValue(MAIN,ST_SEND,sSTIPSend);
									Config_INI.SetValue(MAIN,CHK_SEND,sCHIPSend);
								}
								m_ImpersonationContext.Undo();

								//write log
								m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
								m_SportsLog.SetFileName(0,LOGFILESUFFIX);
								m_SportsLog.Open();
								m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " ConfigINI.cs: Modify INI [RUSHHOUR_START_TIME=" + sRushStart + "; RUSHHOUR_END_TIME=" + sRushEnd + "; HOUSEKEEP_TIME=" + sOddHKTime + " (original=" + sOldHKT + "); REF_DATE=" + sRefDate + " (original=" + sOldRefDate + "); HK_SEND=" + sHKIPSend + "; MAC_SEND=" + sMCIPSend + "; ST_SEND=" + sSTIPSend + "; CHK_SEND=" + sCHIPSend + "] (" + HttpContext.Current.Session["user_name"] + ")");
								m_SportsLog.Close();
								iRecUpd = 1;
							} else {
								m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
								m_SportsLog.SetFileName(0,LOGFILESUFFIX);
								m_SportsLog.Open();
								m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " ConfigINI.cs.SetINISetting(): m_ImpersonationContext is null.");
								m_SportsLog.Close();
								iRecUpd = -1;
							}
						} else {
							m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
							m_SportsLog.SetFileName(0,LOGFILESUFFIX);
							m_SportsLog.Open();
							m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " ConfigINI.cs.SetINISetting(): DuplicateToken error, code = " + Marshal.GetLastWin32Error());
							m_SportsLog.Close();
							iRecUpd = -1;
						}
					} else {
						m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
						m_SportsLog.SetFileName(0,LOGFILESUFFIX);
						m_SportsLog.Open();
						m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " ConfigINI.cs.SetINISetting(): LogonUser error, code = " + Marshal.GetLastWin32Error());
						m_SportsLog.Close();
						iRecUpd = -1;
					}
				} catch(Exception ex) {
					m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
					m_SportsLog.SetFileName(0,LOGFILESUFFIX);
					m_SportsLog.Open();
					m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " ConfigINI.cs.SetINISetting(): " + ex.ToString());
					m_SportsLog.Close();
					iRecUpd = -1;
				}
			} else {
				iRecUpd = -99;
			}

			return iRecUpd;
		}
	}
}