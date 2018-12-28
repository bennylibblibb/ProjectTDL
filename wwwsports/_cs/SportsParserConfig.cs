/*
Objective:
Enable/disable ftp data for sports parser agent

Last updated:
22 Sept 2003 by Rita

C#.NET complier statement:
csc /t:library /out:..\bin\SportsParserConfig.dll /r:..\bin\ConfigReader.dll;..\bin\Files.dll;..\bin\Win32INI.dll;..\bin\FTPMode.dll;..\bin\FTPFileTransferType.dll;..\bin\FTPConnection.dll SportsParserConfig.cs
*/

using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Web;
using TDL.IO;
using TDL.Util;
using FTPClient;

[assembly:AssemblyCompany("TDSL")]
[assembly:AssemblyCopyright("(c) 2003 TDSL. All rights reserved.")]
[assembly:AssemblyDescription("糸統設定 -> FTP ")]
[assembly:AssemblyProduct("Sports.NET")]
[assembly:AssemblyTitle("Sports.NET DLL")]
[assembly:AssemblyVersion("1.0.*")]

namespace SportsUtil {
	public class SportsParserConfig {
		
		public const int LOGON32_LOGON_INTERACTIVE = 2;
		public const int LOGON32_PROVIDER_DEFAULT = 0;
		public const int SecurityImpersonation = 2;
		const string LOGFILESUFFIX = "log";
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

		public SportsParserConfig() {
			m_SportsLog = new Files();
		}		
		
		
		public string GetXMLConfig(string sPath, string sItem) {
			
			string sRole = (string)HttpContext.Current.Session["user_role"];
			StringBuilder HTMLString = new StringBuilder();
			string sRtn, sFlag;
			if(Convert.ToInt32(sRole) >= 11) {
				
				try {
					
					m_LoggedOn = LogonUser(m_NetUser, m_Domain, m_Password, LOGON32_LOGON_INTERACTIVE, LOGON32_PROVIDER_DEFAULT, ref m_Token);
					if(m_LoggedOn) {
						if(DuplicateToken(m_Token, SecurityImpersonation, ref m_TokenDuplicate) != 0) {
							m_TempWindowsIdentity = new WindowsIdentity(m_TokenDuplicate);
							m_ImpersonationContext = m_TempWindowsIdentity.Impersonate();
							if(m_ImpersonationContext != null) {
								
								ConfigReader cr = new ConfigReader();
								cr.SetPath(sPath);
								sFlag = cr.GetValue(sItem);
								
								if(sFlag.Equals("1")) {
									sRtn = "<input type=\"checkbox\" name=\"SportsParserEnable\" value=\"1\" checked>";
									
									//sRtn = sFlag;
								} else {
									sRtn = "<input type=\"checkbox\" name=\"SportsParserEnable\" value=\"1\">";
									//sRtn = sFlag;
								}							
								
								m_ImpersonationContext.Undo();
							} else {
								sRtn = "-99";
								m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
								m_SportsLog.SetFileName(0,LOGFILESUFFIX);
								m_SportsLog.Open();
								m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " SportsParserConfig.cs.GetINISetting(): m_ImpersonationContext is null.");
								m_SportsLog.Close();
							}
						} else {
							sRtn = "-99";
							m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
							m_SportsLog.SetFileName(0,LOGFILESUFFIX);
							m_SportsLog.Open();
							m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " SportsParserConfig.cs.GetINISetting(): DuplicateToken error, code = " + Marshal.GetLastWin32Error());
							m_SportsLog.Close();
						}
					} else {
						sRtn = "-99";
						m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
						m_SportsLog.SetFileName(0,LOGFILESUFFIX);
						m_SportsLog.Open();
						m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " SportsParserConfig.cs.GetINISetting(): LogonUser error, code = " + Marshal.GetLastWin32Error());
						m_SportsLog.Close();
					}
					return sRtn;
				} catch(Exception ex) {					
					
					m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
					m_SportsLog.SetFileName(0,LOGFILESUFFIX);
					m_SportsLog.Open();
					m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " SportsParserConfig.cs.GetINISetting(): " + ex.ToString());
					m_SportsLog.Close();
					HTMLString.Remove(0,HTMLString.Length);
					HTMLString.Append("<tr><th colspan=\"2\">");
					HTMLString.Append((string)HttpContext.Current.Application["accessErrorMsg"]);
					HTMLString.Append("</th></tr>");
					return "-99";
				}
				
			} else {
				
				HTMLString.Remove(0,HTMLString.Length);
				//HTMLString.Append("<tr><th colspan=\"2\">沒有檢視權限</th></tr>");
				return "-100";
			}	
		}

		public string SetXMLConfig(string sPath, string sItem) {
			string sRole;
			string sFlag;
			string ftpUser = "ASPUSER";
			string ftpPassword = "aspspt";
			string ftpAddress = HttpContext.Current.Application["SportsParserFTPAddress"].ToString();
			string ftpPort = HttpContext.Current.Application["SportsParserFTPPort"].ToString();
			string ftpMode = HttpContext.Current.Application["SportsParserFTPMode"].ToString();
			string sFileName;		
			string sParam="";
			string FtpFileString;	
			
			Files SportsParserFile = new Files();
			sFlag = HttpContext.Current.Request.Form["SportsParserEnable"];			
			if(sFlag == null) sFlag = "0";
			if(sFlag.Equals("on")) sFlag="1";
			
			if(sFlag.Equals("1"))
				sParam = "Start";
			else if (sFlag.Equals("0"))	
				sParam = "Stop";	
						
			sRole = (string)HttpContext.Current.Session["user_role"];
			if(Convert.ToInt32(sRole) >= 11) {
				
				try {
			
					//ConfigReader crUpd = new ConfigReader();
					//crUpd.SetPath(sPath);
					//crUpd.SetValue(sItem,sFlag);
					
					FTPConnection ftp = new FTPConnection();
					//ftp.Open(ftpAddress,  ftpUser, ftpPassword);
					if(ftpMode.Equals("Active"))
						ftp.Open(ftpAddress, Convert.ToInt32(ftpPort) , ftpUser, ftpPassword, FTPMode.Active);
						
					else	
						ftp.Open(ftpAddress, Convert.ToInt32(ftpPort) , ftpUser, ftpPassword, FTPMode.Passive);
					
					FtpFileString = 	"<?xml version=\"1.0\" encoding=\"big5\"?>"+ "\r\n" +
														"<Command Name='ProcessFlag'>	"+ "\r\n" +
														"<Param>"+sParam+"</Param>	"+ "\r\n" +
														"</Command>"+ "\r\n";
														
					byte[] cmdBytes = System.Text.Encoding.ASCII.GetBytes(FtpFileString);					
					System.IO.MemoryStream stream = new System.IO.MemoryStream(cmdBytes.Length);
					stream.Write(cmdBytes, 0, cmdBytes.Length);
					stream.Seek(0, System.IO.SeekOrigin.Begin);					
					
					sFileName = DateTime.Now.ToString("yyMMddHHmmss_9999999")+".ASPNetftp.Command.xml";
					ftp.SendStream(stream, sFileName, FTPFileTransferType.ASCII);
					
					stream.Close();					
					ftp.Close(); 					
					
					//write log
					SportsParserFile.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
					SportsParserFile.SetFileName(0,LOGFILESUFFIX);
					SportsParserFile.Open();
					SportsParserFile.Write(DateTime.Now.ToString("HH:mm:ss") + " SportsParserConfig.cs: Set Sport Parser Agent flag=" + sFlag + " (" + HttpContext.Current.Session["user_name"] + ")");
					SportsParserFile.Close();
					
				} catch(Exception ex) {
					SportsParserFile.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
					SportsParserFile.SetFileName(0,LOGFILESUFFIX);
					SportsParserFile.Open();
					SportsParserFile.Write(DateTime.Now.ToString("HH:mm:ss") + " SportsParserConfig.cs.SetXMLConfig(): " + ex.ToString());
					SportsParserFile.Close();
					
				}
				
			} else {
				sFlag = "-99";
			}
			return sFlag;
		}
	}
}