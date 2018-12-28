/*
Objective:
View Sports JC Match Leakage

Last updated:
14 Nov 2003 by Chapman

C#.NET complier statement:
csc /t:library /out:..\bin\JCMatchLeakage.dll /r:..\bin\Files.dll JCMatchLeakage.cs
*/

using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Web;
using TDL.IO;

[assembly:AssemblyCompany("TDSL")]
[assembly:AssemblyCopyright("(c) 2003 TDSL. All rights reserved. Created on 14 Nov 2003.")]
[assembly:AssemblyDescription("Log File")]
[assembly:AssemblyProduct("Sports.NET")]
[assembly:AssemblyTitle("Sports.NET DLL")]
[assembly:AssemblyVersion("1.0.*")]
namespace SportsUtil {
	public class JCMatchLeakage {
		public const int LOGON32_LOGON_INTERACTIVE = 2;
		public const int LOGON32_PROVIDER_DEFAULT = 0;
		public const int SecurityImpersonation = 2;
		const string LOGFILESUFFIX = "log";
		string m_FilePath;
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

		public JCMatchLeakage() {
			m_SportsLog = new Files();
			m_FilePath = HttpContext.Current.Application["JCMatchLeakageFilePath"].ToString();
		}

		public string ShowLeakageFiles() {
			string sRole = (string)HttpContext.Current.Session["user_role"];
			StringBuilder HTMLString = new StringBuilder();
			string[] arrFileName = null;

			try {
				if(Convert.ToInt32(sRole) >= 11) {
					m_LoggedOn = LogonUser(m_NetUser, m_Domain, m_Password, LOGON32_LOGON_INTERACTIVE, LOGON32_PROVIDER_DEFAULT, ref m_Token);
					if(m_LoggedOn) {
						if(DuplicateToken(m_Token, SecurityImpersonation, ref m_TokenDuplicate) != 0) {
							m_TempWindowsIdentity = new WindowsIdentity(m_TokenDuplicate);
							m_ImpersonationContext = m_TempWindowsIdentity.Impersonate();
							if(m_ImpersonationContext != null) {
								arrFileName = System.IO.Directory.GetFiles(m_FilePath, "leakage.*.htm");
								for(int i = 0; i < arrFileName.Length; i++) {
									HTMLString.Append("<tr align=\"left\"><td colspan=\"2\"><a href=\"");
									HTMLString.Append(arrFileName[i].Replace(m_FilePath,""));
									HTMLString.Append("\"  target=\"content_frame\">");
									HTMLString.Append(arrFileName[i].Replace(m_FilePath,""));
									HTMLString.Append("</a></td></tr>");
								}
							} else {
								HTMLString.Append("<tr align=\"left\"><th colspan=\"2\">檢視錯誤<th></tr>");
							}
							m_ImpersonationContext.Undo();
						} else {
							HTMLString.Append("<tr align=\"left\"><th colspan=\"2\">檢視錯誤<th></tr>");
						}
					} else {
						HTMLString.Append("<tr align=\"left\"><th colspan=\"2\">檢視錯誤<th></tr>");
					}
				} else {
					HTMLString.Append("<tr align=\"left\"><th colspan=\"2\">沒有檢視權限<th></tr>");
					m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
					m_SportsLog.SetFileName(0,LOGFILESUFFIX);
					m_SportsLog.Open();
					m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " JCMatchLeakage.cs.ShowLeakageFiles(): User (" + (string)HttpContext.Current.Session["user_name"] + ") access deny.");
					m_SportsLog.Close();
				}
			} catch(System.IO.DirectoryNotFoundException) {
				HTMLString.Remove(0,HTMLString.Length);
				HTMLString.Append("<tr align=\"left\"><th colspan=\"2\">沒有目錄<th></tr>");
			}	catch(Exception ex) {
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " JCMatchLeakage.cs.ShowLeakageFiles(): " + ex.ToString());
				m_SportsLog.Close();
				HTMLString.Append("<tr align=\"left\"><th colspan=\"2\">");
				HTMLString.Remove(0,HTMLString.Length);
				HTMLString.Append((string)HttpContext.Current.Application["accessErrorMsg"]);
				HTMLString.Append("<th></tr>");
			}

			return HTMLString.ToString();
		}
	}
}