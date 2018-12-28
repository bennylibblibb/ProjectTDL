/*
Objective:
Retrieval the system status such as pending job, queuing message

Last updated:
4 Mar 2004, Chapman Choi

C#.NET complier statement:
csc /t:library /out:..\bin\SystemMonitorGoGo1.dll /r:..\bin\DBManager.dll;..\bin\Files.dll SystemMonitorGoGo1.cs
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
[assembly:AssemblyDescription("系統資訊->走地積存")]
[assembly:AssemblyProduct("Sports.NET")]
[assembly:AssemblyTitle("Sports.NET DLL")]
[assembly:AssemblyVersion("1.0.*")]

namespace SportsUtil {
	public class SystemMonitorGoGo1 {
		const string LOGFILESUFFIX = "sysmon.log";
		const string QUEUEFLAG = "訊息積存";
		int m_RecordCount;
		int m_Severity;
		OleDbDataReader m_SportsOleReader;
		DBManager m_SportsDBMgr;
		DBManager m_SportsUpdateDBMgr;
		Files m_SportsLog;
		StringBuilder HTMLString;
		StringBuilder SQLString;
		StringBuilder PendingListString;

		public SystemMonitorGoGo1() {
			m_RecordCount = 0;
			m_Severity = 0;
			m_SportsDBMgr = new DBManager();
			m_SportsDBMgr.ConnectionString = (string)HttpContext.Current.Application["SoccerDBConnectionString"];
			m_SportsUpdateDBMgr = new DBManager();
			m_SportsUpdateDBMgr.ConnectionString = (string)HttpContext.Current.Application["SoccerDBConnectionString"];
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
			} else if(iFiles>10 && iFiles<=15) {
				m_Severity = 3;
				return "(B) 工作中度積存，請控制傳送工作";
			} else if(iFiles>15 && iFiles<=20) {
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
			int iSysMonNo = 0;
			string sDisplayLabel = "";
			string sDBConnection = "";
			string sSQL = "";
			string sStatusLabel = "";
			DBManager statusDBMgr = null;
			OleDbDataReader statusOleReader;

			try {
				//lookup all enabled connections
				SQLString.Remove(0,SQLString.Length);
				SQLString.Append("SELECT ISHOW_DETAILS, CDISPLAY, CDBCONN, CSQL, ISYSMON_NO FROM SYSMONCFG where IENABLED=2 group by ISHOW_DETAILS, CDISPLAY, CDBCONN, CSQL, ISYSMON_NO order by ISYSMON_NO");
				m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
				while (m_SportsOleReader.Read()) {
					if (!m_SportsOleReader.IsDBNull(0)) iShowDetails = m_SportsOleReader.GetInt32(0);
					sDisplayLabel = m_SportsOleReader.GetString(1).Trim();
					if(!sDBConnection.Equals(m_SportsOleReader.GetString(2).Trim())) {
						sDBConnection = m_SportsOleReader.GetString(2).Trim();
						if(statusDBMgr != null) statusDBMgr = null;
						statusDBMgr = new DBManager();
						statusDBMgr.ConnectionString = sDBConnection;
					}
					sSQL = m_SportsOleReader.GetString(3).Trim();
					
					if (!m_SportsOleReader.IsDBNull(4)) iSysMonNo = m_SportsOleReader.GetInt32(4);

					m_RecordCount = 0;
					PendingListString.Remove(0, PendingListString.Length);
					if (iShowDetails == 0)
						sSQL = sSQL.Replace("SELECT *", "SELECT COUNT(*)");
					try {
						statusOleReader = statusDBMgr.ExecuteQuery(sSQL);
						while(statusOleReader.Read()) {
							if (iShowDetails > 0) {
								m_RecordCount++;
								PendingListString.Append("<tr><td align=\"right\">");
								PendingListString.Append(m_RecordCount.ToString());
								PendingListString.Append("</td><td colspan=\"2\">");
								PendingListString.Append(statusOleReader.GetString(0).Trim());
								PendingListString.Append("</td></tr>");
							} else {
								if (!statusOleReader.IsDBNull(0))
									m_RecordCount = statusOleReader.GetInt32(0);
							}
						}
						statusOleReader.Close();
					} catch(Exception statEx) {
						m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
						m_SportsLog.SetFileName(0,LOGFILESUFFIX);
						m_SportsLog.Open();
						m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " SystemMonitorGoGo1.cs.CurrentStatus(): Get " + sDisplayLabel + " (" + sSQL + ") error:");
						m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " SystemMonitorGoGo1.cs.CurrentStatus(): " + statEx.ToString());
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
						HTMLString.Append("]");
						HTMLString.Append("</td><td align=\"right\">");
						if (iShowDetails > 0) {
							HTMLString.Append("<input type=radio name=\"showRadio"+iSysMonNo.ToString()+"\" value=\"0\">數量");
							HTMLString.Append("<input type=radio name=\"showRadio"+iSysMonNo.ToString()+"\" value=\"1\" checked>檔案");
						} else {
							HTMLString.Append("<input type=radio name=\"showRadio"+iSysMonNo.ToString()+"\" value=\"0\" checked>數量");
							HTMLString.Append("<input type=radio name=\"showRadio"+iSysMonNo.ToString()+"\" value=\"1\">檔案");
						}
						HTMLString.Append("&nbsp&nbsp&nbsp<input type=\"checkbox\" name=\"deleteCheckBox\" value=\""+iSysMonNo.ToString()+"\">刪除");
						HTMLString.Append("</td>");
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
							case 4:
								PendingListString.Replace("<td align=\"right\">", "<td align=\"right\" style=\"background:#87CEFA\">");
								break;
							case 3:
								PendingListString.Replace("<td align=\"right\">", "<td align=\"right\" style=\"background:#FFFF00\">");
								break;
							case 2:
								PendingListString.Replace("<td align=\"right\">", "<td align=\"right\" style=\"background:#FFA500\">");
								break;
							case 1:
								PendingListString.Replace("<td align=\"right\">", "<td align=\"right\" style=\"background:#FF0000\">");
								break;
							default:
								break;
						}
						HTMLString.Append(PendingListString.ToString());
					}
				}
				m_SportsOleReader.Close();
				m_SportsDBMgr.Close();

				m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " SystemMonitorGoGo1.cs: Status enquiry from " + HttpContext.Current.Session["user_name"]);
				m_SportsLog.Close();
			}	catch(Exception ex) {
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " SystemMonitorGoGo1.cs.CurrentStatus(): " + ex.ToString());
				m_SportsLog.Close();
				HTMLString.Remove(0,HTMLString.Length);
				HTMLString.Append((string)HttpContext.Current.Application["accessErrorMsg"]);
			}

			return HTMLString.ToString();
		}
		
		public int UpdateShowStatus() {
			int iUpdated = 0;
			//int iShowFlag = -1;
			int iSysMonNo = 0;
			string sRadioType = "";
			string sSQL = "";
			
			try {
				SQLString.Remove(0,SQLString.Length);
				SQLString.Append("SELECT ISYSMON_NO FROM SYSMONCFG where IENABLED=2 order by ISYSMON_NO");
				m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());				
				while (m_SportsOleReader.Read()) {
					if (!m_SportsOleReader.IsDBNull(0)) {
						iSysMonNo = m_SportsOleReader.GetInt32(0);
						try {
							sRadioType = "showRadio";
							sRadioType = sRadioType+iSysMonNo.ToString();
							if (HttpContext.Current.Request.Form[sRadioType] == "1") {
								sSQL = "update SYSMONCFG set ISHOW_DETAILS=1 where ISYSMON_NO="+iSysMonNo.ToString();
								m_SportsUpdateDBMgr.ExecuteNonQuery(sSQL);
							} else if (HttpContext.Current.Request.Form[sRadioType] == "0") {
								sSQL = "update SYSMONCFG set ISHOW_DETAILS=0 where ISYSMON_NO="+iSysMonNo.ToString();
								m_SportsUpdateDBMgr.ExecuteNonQuery(sSQL);
							}
							m_SportsUpdateDBMgr.Close();
						} catch(Exception updateEx) {
							m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
							m_SportsLog.SetFileName(0,LOGFILESUFFIX);
							m_SportsLog.Open();
							m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " SystemMonitorGoGo1.cs.UpdateShowStatus() Update:ISHOW_DETAILS " + updateEx.ToString());
							m_SportsLog.Close();
							iUpdated++;
						}
					}
				}
				m_SportsOleReader.Close();
				m_SportsDBMgr.Close();
			} catch(Exception ex) {
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " SystemMonitorGoGo1.cs.UpdateShowStatus() Get:ISYSMON_NO " + ex.ToString());
				m_SportsLog.Close();
				iUpdated++;
			}			
			return iUpdated;			
		}
			
		public int DeletePendingFile() {
			
			int iUpdated = 1;
			char[] delimiter = new char[] {','};			
			int iTotalRec = 0;
			int i = 0;
			string sSQL = "";
			string sSysMonNo = "0";
			string[] arrMustDelete;
			int iMustDeleteLen = 0;
			
			try {
				arrMustDelete = HttpContext.Current.Request.Form["deleteCheckBox"].Split(delimiter);
				iMustDeleteLen = arrMustDelete.Length;
			}	catch(Exception) {
				arrMustDelete = new string[0];
				iMustDeleteLen = 0;
			}
						
			try {
				for (i=0; i<iMustDeleteLen; i++) {
					sSysMonNo = arrMustDelete[i];
					SQLString.Remove(0,SQLString.Length);
					SQLString.Append("SELECT CSQL FROM SYSMONCFG where ISYSMON_NO=");
					SQLString.Append(sSysMonNo);				
					m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
					if (m_SportsOleReader.Read()) {
						if(!m_SportsOleReader.IsDBNull(0))
							sSQL = m_SportsOleReader.GetString(0).Trim();
					}
					m_SportsOleReader.Close();
					m_SportsDBMgr.Close();
					
					sSQL = sSQL.Replace("SELECT *", "SELECT COUNT(*)");
					m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(sSQL);
					if (m_SportsOleReader.Read()) {
						if(!m_SportsOleReader.IsDBNull(0))
							iTotalRec = m_SportsOleReader.GetInt32(0);
					}
					m_SportsOleReader.Close();
					m_SportsDBMgr.Close();
									
					if (iTotalRec > 0) {
						sSQL = sSQL.Replace("SELECT COUNT(*)", "DELETE");
						m_SportsDBMgr.ExecuteNonQuery(sSQL);
						m_SportsDBMgr.Close();
					}					
					//write log
					m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
					m_SportsLog.SetFileName(0,LOGFILESUFFIX);
					m_SportsLog.Open();
					m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " SystemMonitorGoGo1.cs.DeletePendingFile(): Delete System monitor number of " + sSysMonNo + " by user (" + HttpContext.Current.Session["user_name"] + ")");
					m_SportsLog.Close();
				}
				if (iMustDeleteLen == 0) {
					iUpdated = 0;
				}				
			}	catch(Exception ex) {
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " SystemMonitorGoGo1.cs.DeletePendingFile(): " + ex.ToString());
				m_SportsLog.Close();
				iUpdated++;
			}
			
			return iUpdated;			
		}
	}
}