/*
Objective:
Retrieval the system status such as pending job, queuing message

Last updated:
4 Mar 2004, Chapman Choi

C#.NET complier statement:
csc /t:library /out:..\bin\SystemMonitor.dll /r:..\bin\DBManager.dll;..\bin\Files.dll SystemMonitor.cs
*/

using System;
using System.Data.OleDb;
using System.Reflection;
using System.Text;
using System.Web;
using TDL.DB;
using TDL.IO;

[assembly:AssemblyCompany("TDSL")]
[assembly:AssemblyCopyright("(c) 2003 TDSL. All rights reserved. Created 5 August 2003.")]
[assembly:AssemblyDescription("系統資訊")]
[assembly:AssemblyProduct("Sports.NET")]
[assembly:AssemblyTitle("Sports.NET DLL")]
[assembly:AssemblyVersion("1.0.*")]
namespace SportsUtil {
	public class SystemMonitor {
		const string LOGFILESUFFIX = "sysmon.log";
		const string QUEUEFLAG = "訊息積存";
		int m_RecordCount;
		int m_Severity;
		OleDbDataReader m_SportsOleReader;
		DBManager m_SportsDBMgr;
		Files m_SportsLog;
		StringBuilder HTMLString;
		StringBuilder SQLString;
		StringBuilder PendingListString;

		public SystemMonitor() {
			m_RecordCount = 0;
			m_Severity = 0;
			m_SportsDBMgr = new DBManager();
			m_SportsDBMgr.ConnectionString = (string)HttpContext.Current.Application["SoccerDBConnectionString"];
			m_SportsLog = new Files();
			HTMLString = new StringBuilder();
			SQLString = new StringBuilder();
			PendingListString = new StringBuilder();
		}

		private string PendingfileStatus(int iFiles) {
			if(iFiles<=5) {
				m_Severity = 5;
				return "(N) 系統正常";
			} else if(iFiles>5 && iFiles<=10) {
				m_Severity = 4;
				return "(A) 工作少量積存，請留意系統狀況";
			} else if(iFiles>10 && iFiles<=18) {
				m_Severity = 3;
				return "(B) 工作中度積存，請控制傳送工作";
			} else if(iFiles>18 && iFiles<=30) {
				m_Severity = 2;
				return "(C) 工作大量積存，請減少傳送工作";
			} else {
				m_Severity = 1;
				return "(X) 工作嚴重積存，請<b>立即停止</b>傳送及聯絡電訊當值人員";
			}
		}

		private string PendingQueueStatus(int iJobs) {
			if(iJobs<=30) {
				m_Severity = 5;
				return "(N) 系統正常";
			}	else if(iJobs>30 && iJobs<=100) {
				m_Severity = 4;
				return "(A) 訊息少量積存，請留意系統狀況";
			} else if(iJobs>100 && iJobs<=200) {
				m_Severity = 3;
				return "(B) 訊息中度積存，請控制傳送工作";
			} else if(iJobs>200 && iJobs<=300) {
				m_Severity = 2;
				return "(C) 訊息大量積存，請減少傳送工作";
			}	else {
				m_Severity = 1;
				return "(X) 訊息嚴重積存，請<b>立即停止</b>傳送及聯絡電訊當值人員";
			}
		}

		public string CurrentStatus() {
			int iShowDetails = 0;
			string sDisplayLabel = "";
			string sDBConnection = "";
			string sSQL = null;
			string sStatusLabel = "";
			DBManager statusDBMgr = null;
			OleDbDataReader statusOleReader;

			try {
				//lookup all enabled connections
				SQLString.Remove(0,SQLString.Length);
				SQLString.Append("SELECT ISHOW_DETAILS, CDISPLAY, CDBCONN, CSQL FROM SYSMONCFG where IENABLED=1 group by ISHOW_DETAILS, CDISPLAY, CDBCONN, CSQL order by ISHOW_DETAILS desc, CDISPLAY");
				m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
				while(m_SportsOleReader.Read()) {
					if(!m_SportsOleReader.IsDBNull(0)) iShowDetails = m_SportsOleReader.GetInt32(0);
					sDisplayLabel = m_SportsOleReader.GetString(1).Trim();
					if(!sDBConnection.Equals(m_SportsOleReader.GetString(2).Trim())) {
						sDBConnection = m_SportsOleReader.GetString(2).Trim();
						if(statusDBMgr != null) statusDBMgr = null;
						statusDBMgr = new DBManager();
						statusDBMgr.ConnectionString = sDBConnection;
					}
					sSQL = m_SportsOleReader.GetString(3).Trim();

					m_RecordCount = 0;
					PendingListString.Remove(0, PendingListString.Length);
					try {
						statusOleReader = statusDBMgr.ExecuteQuery(sSQL);
						while(statusOleReader.Read()) {
							m_RecordCount++;
							if(iShowDetails > 0) {
								PendingListString.Append("<tr><td align=\"right\">");
								PendingListString.Append(m_RecordCount.ToString());
								PendingListString.Append("</td><td>");
								PendingListString.Append(statusOleReader.GetString(0).Trim());
								PendingListString.Append("</td></tr>");
							}
						}
						statusOleReader.Close();
					} catch(Exception statEx) {
						m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
						m_SportsLog.SetFileName(0,LOGFILESUFFIX);
						m_SportsLog.Open();
						m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " SystemMonitor.cs.CurrentStatus(): Get " + sDisplayLabel + "(" + sSQL + ") error:");
						m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " SystemMonitor.cs.CurrentStatus(): " + statEx.ToString());
						m_SportsLog.Close();

						m_RecordCount = -1;
					}
					statusDBMgr.Close();

					m_Severity = 0;
					if(m_RecordCount != -1) {
						if(sDisplayLabel.IndexOf(QUEUEFLAG) == -1) {
							sStatusLabel = PendingfileStatus(m_RecordCount);
						} else {
							sStatusLabel = PendingQueueStatus(m_RecordCount);
						}
					}

					HTMLString.Append("<tr><th");
					switch(m_Severity) {
						case 4:	HTMLString.Append(" style=\"background:#87CEFA\"");
										break;
						case 3:	HTMLString.Append(" style=\"background:#FFFF00\"");
										break;
						case 2:	HTMLString.Append(" style=\"background:#FFA500\"");
										break;
						case 1:	HTMLString.Append(" style=\"background:#FF0000\"");
										break;
						default:	break;
					}
					HTMLString.Append(" align=\"right\" width=\"40%\">");
					HTMLString.Append(sDisplayLabel);
					HTMLString.Append("</th>");
					if(m_RecordCount != -1) {
						HTMLString.Append("<td>");
						HTMLString.Append(sStatusLabel);
						HTMLString.Append(" [");
						HTMLString.Append(m_RecordCount.ToString());
						HTMLString.Append("]</td>");
					} else {
						HTMLString.Append("<th align=\"left\" style=\"color:#FF0000\">");
						if(sDisplayLabel.IndexOf(QUEUEFLAG) == -1) {
							HTMLString.Append("此工作列存取錯誤，請聯絡電訊當值人員。");
						} else {
							HTMLString.Append("此訊息列存取錯誤，請聯絡電訊當值人員。");
						}
						HTMLString.Append("</th>");
					}
					HTMLString.Append("</tr>");
					if(PendingListString.Length > 0) {
						switch(m_Severity) {
							case 4:	PendingListString.Replace("<td align=\"right\">", "<td align=\"right\" style=\"background:#87CEFA\">");
											break;
							case 3:	PendingListString.Replace("<td align=\"right\">", "<td align=\"right\" style=\"background:#FFFF00\">");
											break;
							case 2:	PendingListString.Replace("<td align=\"right\">", "<td align=\"right\" style=\"background:#FFA500\">");
											break;
							case 1:	PendingListString.Replace("<td align=\"right\">", "<td align=\"right\" style=\"background:#FF0000\">");
											break;
							default:	break;
						}
						HTMLString.Append(PendingListString.ToString());
					}
				}
				m_SportsOleReader.Close();
				m_SportsDBMgr.Close();

				m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " SystemMonitor.cs: Status enquiry from " + HttpContext.Current.Session["user_name"]);
				m_SportsLog.Close();
			}	catch(Exception ex) {
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " SystemMonitor.cs.CurrentStatus(): " + ex.ToString());
				m_SportsLog.Close();
				HTMLString.Remove(0,HTMLString.Length);
				HTMLString.Append((string)HttpContext.Current.Application["accessErrorMsg"]);
			}

			return HTMLString.ToString();
		}
	}
}