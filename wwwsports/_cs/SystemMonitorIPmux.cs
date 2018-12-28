/*
Objective:
Retrieval the system status such as pending job, queuing message

Last updated:
4 Mar 2004, Chapman Choi

C#.NET complier statement:
csc /t:library /out:..\bin\SystemMonitorIPmux.dll /r:..\bin\DBManager.dll;..\bin\Files.dll;..\bin\Win32Message.dll SystemMonitorIPmux.cs
*/

using System;
using System.Data.OleDb;
using System.Reflection;
using System.Text;
using System.Web;
using TDL.DB;
using TDL.IO;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using RemoteService.Win32;

[assembly:AssemblyCompany("TDSL")]
[assembly:AssemblyCopyright("(c) 2003 TDSL. All rights reserved. Created 5 August 2003.")]
[assembly:AssemblyDescription("系統資訊->IPMUX")]
[assembly:AssemblyProduct("Sports.NET")]
[assembly:AssemblyTitle("Sports.NET DLL")]
[assembly:AssemblyVersion("1.0.*")]
namespace SportsUtil {
	public class SystemMonitorIPmux {
		const string LOGFILESUFFIX = "sysmon.log";
		const string QUEUEFLAG = "訊息積存";
		OleDbDataReader m_SportsOleReader;
		DBManager m_SportsDBMgr;
		Files m_SportsLog;
		StringBuilder HTMLString;
		StringBuilder SQLString;
		StringBuilder PendingListString;

		public SystemMonitorIPmux() {
			m_SportsDBMgr = new DBManager();
			m_SportsDBMgr.ConnectionString = (string)HttpContext.Current.Application["SoccerDBConnectionString"];
			m_SportsLog = new Files();
			HTMLString = new StringBuilder();
			SQLString = new StringBuilder();
			PendingListString = new StringBuilder();
		}

		public string CurrentStatus() {
			int iShowDetails = 0;
			string sDisplayLabel = "";
			string sChannelRef = "";
			string sChannelIP = "";
			string sChanelPort = "";
			string sObjectUri = "";
			string sIPmuxIP = "";
			string sIPmuxPort = "";
			string sResultStatus = "";

			try {
				//lookup all enabled connections
				SQLString.Remove(0,SQLString.Length);
				SQLString.Append("SELECT ISHOW_DETAILS, CDISPLAY, CCHANNEL_REF, CCHANNEL_IP, CCHANNEL_PORT, COBJECT_URI, CIPMUX_IP, CIPMUX_PORT FROM SYSMONIPMUXCFG where IENABLED=1 order by ISHOW_DETAILS desc, CDISPLAY");
				m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
				while(m_SportsOleReader.Read()) {
					if(!m_SportsOleReader.IsDBNull(0)) iShowDetails = m_SportsOleReader.GetInt32(0);
					sDisplayLabel = m_SportsOleReader.GetString(1).Trim();
					sChannelRef = m_SportsOleReader.GetString(2).Trim();;
					sChannelIP = m_SportsOleReader.GetString(3).Trim();;
					sChanelPort = m_SportsOleReader.GetString(4).Trim();;
					sObjectUri = m_SportsOleReader.GetString(5).Trim();;
					sIPmuxIP = m_SportsOleReader.GetString(6).Trim();;
					sIPmuxPort = m_SportsOleReader.GetString(7).Trim();;
					
					Win32Message ipmuxmsg = (Win32Message)Activator.GetObject(typeof(RemoteService.Win32.Win32Message), sChannelRef+"://"+sChannelIP+":"+sChanelPort+"/"+sObjectUri);
					
					sResultStatus = ipmuxmsg.IPmuxStatus(sIPmuxIP, sIPmuxPort);
					//sResultStatus = "N";
					HTMLString.Append("<tr><th");
					HTMLString.Append(" align=\"right\" width=\"40%\">");
					HTMLString.Append(sDisplayLabel);
					HTMLString.Append("</th>");
					HTMLString.Append("<td>");
					HTMLString.Append(sResultStatus);
					HTMLString.Append("</td>");
				}
				m_SportsOleReader.Close();
				m_SportsDBMgr.Close();

				m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " SystemMonitorIPmux.cs: Status enquiry from " + HttpContext.Current.Session["user_name"]);
				m_SportsLog.Close();
			}	catch(Exception ex) {
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " SystemMonitorIPmux.cs.CurrentStatus(): " + ex.ToString());
				m_SportsLog.Close();
				HTMLString.Remove(0,HTMLString.Length);
				HTMLString.Append((string)HttpContext.Current.Application["accessErrorMsg"]);
			}

			return HTMLString.ToString();
		}
	}
}