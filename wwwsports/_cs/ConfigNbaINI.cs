/*
Objective:
Get and set config setting from ini file (AsianNBAServer.ini) such as Rush Hour, COM availability

Last updated:
10 Jun 2004, Chapman Choi

C#.NET complier statement:
csc /t:library /out:..\bin\ConfigNbaINI.dll /r:..\bin\Files.dll;..\bin\Win32INI.dll ConfigNbaINI.cs
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
[assembly:AssemblyDescription("系統設定 - 籃球")]
[assembly:AssemblyProduct("Sports.NET")]
[assembly:AssemblyTitle("Sports.NET DLL")]
[assembly:AssemblyVersion("3.0.*")]

namespace SportsUtil {
	public class ConfigNbaINI {
		public const int LOGON32_LOGON_INTERACTIVE = 2;
		public const int LOGON32_PROVIDER_DEFAULT = 0;
		public const int SecurityImpersonation = 2;
		const string LOGFILESUFFIX = "log";
		const string SYS_PARA = "SYS_PARA";
		const string REF_DATE = "REF_DATE";
		const string UPDATE_REF_DATE_TM = "UPDATE_REF_DATE_TM";
		const string MATCH_HK_TIME = "MATCH_HK_TIME";
		const string DB_BACKUP_TIME1 = "DB_BACKUP_TIME1";
		const string DB_BACKUP_TIME2 = "DB_BACKUP_TIME2";
		const string RESEND_ITEM_7 = "RESEND_ITEM_7";
		const string SLOT_1 = "SLOT_1";

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

		public ConfigNbaINI() {
			m_SportsLog = new Files();
		}

		public string CurrentDateTime {
			get {
				return DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss");
			}
		}

		public string GetINISetting() {
			string sRole = (string)HttpContext.Current.Session["user_role"];
			StringBuilder HTMLString = new StringBuilder();

			if(Convert.ToInt32(sRole) >= 11) {
				try {
					string sRefDate = "";
					string sRefDateTM = "";
					string sMatchHKTime = "";
					string sDBBK1 = "";
					string sDBBK2 = "";

					m_LoggedOn = LogonUser(m_NetUser, m_Domain, m_Password, LOGON32_LOGON_INTERACTIVE, LOGON32_PROVIDER_DEFAULT, ref m_Token);
					if(m_LoggedOn) {
						if(DuplicateToken(m_Token, SecurityImpersonation, ref m_TokenDuplicate) != 0) {
							m_TempWindowsIdentity = new WindowsIdentity(m_TokenDuplicate);
							m_ImpersonationContext = m_TempWindowsIdentity.Impersonate();
							if(m_ImpersonationContext != null) {
								Win32INI Config_INI = new Win32INI();
								Config_INI.FilePath = HttpContext.Current.Application["NbaINIFilePath"].ToString();
								sRefDate = Config_INI.GetValue(SYS_PARA,REF_DATE);
								sRefDateTM= Config_INI.GetValue(SYS_PARA,UPDATE_REF_DATE_TM);
								sMatchHKTime = Config_INI.GetValue(SYS_PARA,MATCH_HK_TIME);
								sDBBK1 = Config_INI.GetValue(SYS_PARA,DB_BACKUP_TIME1);
								sDBBK2 = Config_INI.GetValue(SYS_PARA,DB_BACKUP_TIME2);
								m_ImpersonationContext.Undo();

							} else {
								m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
								m_SportsLog.SetFileName(0,LOGFILESUFFIX);
								m_SportsLog.Open();
								m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " ConfigNbaINI.cs.GetINISetting(): m_ImpersonationContext is null.");
								m_SportsLog.Close();
							}
						} else {
							m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
							m_SportsLog.SetFileName(0,LOGFILESUFFIX);
							m_SportsLog.Open();
							m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " ConfigNbaINI.cs.GetINISetting(): DuplicateToken error, code = " + Marshal.GetLastWin32Error());
							m_SportsLog.Close();
						}
					} else {
						m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
						m_SportsLog.SetFileName(0,LOGFILESUFFIX);
						m_SportsLog.Open();
						m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " ConfigNbaINI.cs.GetINISetting(): LogonUser error, code = " + Marshal.GetLastWin32Error());
						m_SportsLog.Close();
					}

					//HouseKeep setting
					HTMLString.Append("<tr><th style=\"background-color:#40E0D0\" align=\"right\">Housekeep(清除及轉移賽事、比數至賽果)時間<font color=\"red\">[亞洲體育提供]</font>:</th>");
					HTMLString.Append("<td align=\"center\"><input type=\"text\" name=\"HKT\" value=\"");
					HTMLString.Append(sMatchHKTime);
					HTMLString.Append("\" maxlength=\"4\" size=\"2\" onChange=\"HousekeepChanged(");
					HTMLString.Append(sMatchHKTime);
					HTMLString.Append(")\"></td></tr>");
					HTMLString.Append("<input type=\"hidden\" name=\"org_HKT\" value=\"");
					HTMLString.Append(sMatchHKTime);
					HTMLString.Append("\">");

					HTMLString.Append("<tr><th style=\"background-color:#40E0D0\" align=\"right\">Housekeep (REF_DATE):</th>");
					HTMLString.Append("<td align=\"center\"><input type=\"text\" name=\"HKRefDate\" value=\"");
					HTMLString.Append(sRefDate);
					HTMLString.Append("\" maxlength=\"8\" size=\"8\" onChange=\"HousekeepRefDateChanged(");
					HTMLString.Append(sRefDate);
					HTMLString.Append(")\"></td></tr>");
					HTMLString.Append("<input type=\"hidden\" name=\"org_HKRefDate\" value=\"");
					HTMLString.Append(sRefDate);
					HTMLString.Append("\">");

					if(Convert.ToInt32(sRole) >= 988) {
						HTMLString.Append("<tr><th colspan=\"2\" style=\"background-color:#40E0D0\" align=\"left\"><font color=\"red\">HKT</font>代表Housekeep時間，更新<font color=\"red\">HKT</font>會自動更新以下4項數值:</th></tr>");
						HTMLString.Append("<tr><th style=\"background-color:#40E0D0\" align=\"right\">更新圖表參考日子時間<font color=\"red\">[HKT + 30分鐘]</font>(UPDATE_REF_DATE_TM):</th><td align=\"center\">");
						HTMLString.Append(sRefDateTM);
						HTMLString.Append("</td></tr>");
						HTMLString.Append("<tr><th style=\"background-color:#40E0D0\" align=\"right\">清除及轉移賽事時間<font color=\"red\">[等於HKT]</font> (MATCH_HK_TIME):</th><td align=\"center\">");
						HTMLString.Append(sMatchHKTime);
						HTMLString.Append("</td></tr>");
						HTMLString.Append("<tr><th style=\"background-color:#40E0D0\" align=\"right\">第一次自動備份數據庫時間<font color=\"red\">[HKT - 10分鐘]</font> (DB_BACKUP_TIME1):</th><td align=\"center\">");
						HTMLString.Append(sDBBK1);
						HTMLString.Append("</td></tr>");
						HTMLString.Append("<tr><th style=\"background-color:#40E0D0\" align=\"right\">第二次自動備份數據庫時間<font color=\"red\">[HKT + 10分鐘]</font> (DB_BACKUP_TIME2):</th><td align=\"center\">");
						HTMLString.Append(sDBBK2);
						HTMLString.Append("</td></tr>");
					}

				} catch(Exception ex) {
					m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
					m_SportsLog.SetFileName(0,LOGFILESUFFIX);
					m_SportsLog.Open();
					m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " ConfigNbaINI.cs.GetINISetting(): " + ex.ToString());
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
			string NbaINIFilePath = HttpContext.Current.Application["NbaINIFilePath"].ToString();
			string JCNbaINIFilePath = HttpContext.Current.Application["JCNbaINIFilePath"].ToString();
			string ComboNbaINIFilePath = HttpContext.Current.Application["ComboNbaINIFilePath"].ToString();

			DateTime dtHK;
			sRole = (string)HttpContext.Current.Session["user_role"];

			if(Convert.ToInt32(sRole) >= 11) {
				string sRefDateTM, sDBBK1 , sDBBK2 , sRefDate;
				string sHKT, sOrgHKT, sOrgRefDate;

				try {
					sOrgRefDate = HttpContext.Current.Request.Form["org_HKRefDate"];
					sOrgHKT = HttpContext.Current.Request.Form["org_HKT"];
					sRefDate = HttpContext.Current.Request.Form["HKRefDate"];
					sHKT = HttpContext.Current.Request.Form["HKT"];
					dtHK = Convert.ToDateTime(sHKT.Substring(0,2) + ":" + sHKT.Substring(2) + ":00");
					sRefDateTM = dtHK.AddMinutes(30).ToString("HHmm");
					sDBBK1 = dtHK.AddMinutes(-10).ToString("HHmm");
					sDBBK2 = dtHK.AddMinutes(10).ToString("HHmm");

					m_LoggedOn = LogonUser(m_NetUser, m_Domain, m_Password, LOGON32_LOGON_INTERACTIVE, LOGON32_PROVIDER_DEFAULT, ref m_Token);
					if(m_LoggedOn) {
						if(DuplicateToken(m_Token, SecurityImpersonation, ref m_TokenDuplicate) != 0) {
							m_TempWindowsIdentity = new WindowsIdentity(m_TokenDuplicate);
							m_ImpersonationContext = m_TempWindowsIdentity.Impersonate();
							if(m_ImpersonationContext != null) {
								Win32INI Config_INI = new Win32INI();
								Config_INI.FilePath = NbaINIFilePath;
								Config_INI.SetValue(SYS_PARA,REF_DATE,sRefDate);
								Config_INI.SetValue(SYS_PARA,UPDATE_REF_DATE_TM,sRefDateTM);
								Config_INI.SetValue(SYS_PARA,MATCH_HK_TIME,sHKT);
								Config_INI.SetValue(SYS_PARA,DB_BACKUP_TIME1,sDBBK1);
								Config_INI.SetValue(SYS_PARA,DB_BACKUP_TIME2,sDBBK2);

								Config_INI.FilePath = JCNbaINIFilePath;
								Config_INI.SetValue(SYS_PARA,REF_DATE,sRefDate);
								Config_INI.SetValue(SYS_PARA,UPDATE_REF_DATE_TM,sRefDateTM);
								Config_INI.SetValue(SYS_PARA,MATCH_HK_TIME,sHKT);
								Config_INI.SetValue(SYS_PARA,DB_BACKUP_TIME1,sDBBK1);
								Config_INI.SetValue(SYS_PARA,DB_BACKUP_TIME2,sDBBK2);
								
								Config_INI.FilePath = ComboNbaINIFilePath;
								Config_INI.SetValue(RESEND_ITEM_7,SLOT_1,sHKT.Substring(0,2)+":"+sHKT.Substring(2,2));
								m_ImpersonationContext.Undo();

								//write log
								m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
								m_SportsLog.SetFileName(0,LOGFILESUFFIX);
								m_SportsLog.Open();
								m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " ConfigNbaINI.cs: Modify NBA INI [REFER_DATE="+ sRefDate + " (Original="+ sOrgRefDate +") ,HOUSEKEEP_TIME=" + sHKT + " (original=" + sOrgHKT + ")] (" + HttpContext.Current.Session["user_name"] + ")");
								m_SportsLog.Close();
								iRecUpd = 1;
							} else {
								m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
								m_SportsLog.SetFileName(0,LOGFILESUFFIX);
								m_SportsLog.Open();
								m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " ConfigNbaINI.cs.SetINISetting(): m_ImpersonationContext is null.");
								m_SportsLog.Close();
								iRecUpd = -1;
							}
						} else {
							m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
							m_SportsLog.SetFileName(0,LOGFILESUFFIX);
							m_SportsLog.Open();
							m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " ConfigNbaINI.cs.SetINISetting(): DuplicateToken error, code = " + Marshal.GetLastWin32Error());
							m_SportsLog.Close();
							iRecUpd = -1;
						}
					} else {
						m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
						m_SportsLog.SetFileName(0,LOGFILESUFFIX);
						m_SportsLog.Open();
						m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " ConfigNbaINI.cs.SetINISetting(): LogonUser error, code = " + Marshal.GetLastWin32Error());
						m_SportsLog.Close();
						iRecUpd = -1;
					}

				} catch(Exception ex) {
					m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
					m_SportsLog.SetFileName(0,LOGFILESUFFIX);
					m_SportsLog.Open();
					m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " ConfigNbaINI.cs.SetINISetting(): " + ex.ToString());
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