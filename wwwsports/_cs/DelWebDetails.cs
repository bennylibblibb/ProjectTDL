/*
Objective:
Create command ini file to delete pager and web data

Last updated:
19 Feb 2004, Chapman Choi

C#.NET complier statement:
csc /t:library /out:..\bin\DelWebDetails.dll /r:..\bin\DBManager.dll;..\bin\Files.dll;..\bin\MessageClient.dll;..\bin\SportsMessage.dll DelWebDetails.cs
*/

using System;
using System.Collections;
using System.Data.OleDb;
using System.Reflection;
using System.Text;
using System.Web;
using TDL.DB;
using TDL.IO;
using TDL.Message;

[assembly:AssemblyCompany("TDSL")]
[assembly:AssemblyCopyright("(c) 2003 TDSL. All rights reserved.")]
[assembly:AssemblyDescription("重發/刪除資訊 -> 刪除網頁資訊")]
[assembly:AssemblyProduct("Sports.NET")]
[assembly:AssemblyTitle("Sports.NET DLL")]
[assembly:AssemblyVersion("3.1.*")]
namespace SportsUtil {
	public class DelWebDetails {
		const string CMDTYPE1 = "CLR_ALL";
		const string CMDTYPE2 = "OTHODDS_ALLDEL";
		const string LOGFILESUFFIX = "log";
		string m_Role;
		Files m_SportsLog;
		Encoding m_Big5Encoded;
		StringBuilder SQLString;
		OleDbDataReader m_SportsOleReader;
		DBManager m_SportsDBMgr;

		public DelWebDetails(string Connection) {
			m_SportsDBMgr = new DBManager();
			m_SportsDBMgr.ConnectionString = Connection;
			m_SportsLog = new Files();
			m_Big5Encoded = Encoding.GetEncoding(950);
			m_Role = (string)HttpContext.Current.Session["user_role"];
			if(m_Role == null) m_Role = "0";
			if(m_Role.Equals("")) m_Role = "0";
			SQLString = new StringBuilder();
		}

		public string ShowItems() {
			StringBuilder HTMLString = new StringBuilder();
			string sSvcID = "";
			sSvcID = HttpContext.Current.Request.QueryString["svc"].Trim();

			try {
				if(Convert.ToInt32(m_Role) > 10) {
					string sSvcRight = "";
					string sSvcName = "";
					//Double check for user right
					SQLString.Remove(0,SQLString.Length);
					SQLString.Append("select CACCESS_RIGHT, CSVC_NAME from SVCCFG where ISVC_NO=");
					SQLString.Append(sSvcID);
					SQLString.Append(" and DEL_TYPE='W'");
					m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
					if(m_SportsOleReader.Read()) {
						sSvcRight = m_SportsOleReader.GetString(0).Trim();
						sSvcName = m_SportsOleReader.GetString(1).Trim();
					}
					m_SportsOleReader.Close();
					m_SportsDBMgr.Close();

					if(Convert.ToInt32(m_Role) >= Convert.ToInt32(sSvcRight)) {
						int iItemCount = 0;
						int iItemRow = 0;
						string sCategory = "";
						SQLString.Remove(0,SQLString.Length);
						SQLString.Append("select CCATEGORY, CPARAM, CCMD_TYPE, CDISPLAY from ADMINCFG where IADMIN_NO in (select IADMIN_NO from ADMINSVCMAP where ISVC_NO=");
						SQLString.Append(sSvcID);
						SQLString.Append(") AND (CCMD_TYPE='");
						SQLString.Append(CMDTYPE1);
						SQLString.Append("' OR CCMD_TYPE='");
						SQLString.Append(CMDTYPE2);
						SQLString.Append("') group by CCATEGORY, CDISPLAY, CPARAM, CCMD_TYPE order by CCATEGORY, IADMIN_NO");
						m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
						while(m_SportsOleReader.Read()) {
							if(!sCategory.Equals(m_SportsOleReader.GetString(0).Trim())) {
								sCategory = m_SportsOleReader.GetString(0).Trim();
								HTMLString.Append("<tr align=\"left\"><th colspan=\"5\" bgcolor=\"#F3A0A0\">");
								HTMLString.Append(sSvcName);
								HTMLString.Append(" - ");
								HTMLString.Append(sCategory);
								HTMLString.Append("</th></tr>");
								iItemRow = 0;
							}
							if(iItemRow%5 == 0) {
								HTMLString.Append("<tr align=\"left\">");
							}
							HTMLString.Append("<td><input type=\"checkbox\" name=\"ItemToDelete\" value=\"");
							HTMLString.Append(iItemCount.ToString());
							if (sSvcID == "14") {
								HTMLString.Append("\" checked><input type=\"hidden\" name=\"param\" value=\"");
							} else if (sSvcID == "15") {
								HTMLString.Append("\"><input type=\"hidden\" name=\"param\" value=\"");
							}
							HTMLString.Append(m_SportsOleReader.GetString(1).Trim());
							HTMLString.Append("\"><input type=\"hidden\" name=\"cmdtype\" value=\"");
							HTMLString.Append(m_SportsOleReader.GetString(2).Trim());
							HTMLString.Append("\">");
							HTMLString.Append(m_SportsOleReader.GetString(3).Trim());
							HTMLString.Append("</td>");
							if(iItemRow%5 == 4) {
								HTMLString.Append("</tr>");
							}
							iItemRow++;
							iItemCount++;
						}
						m_SportsOleReader.Close();
						m_SportsDBMgr.Close();
						m_SportsDBMgr.Dispose();
						if(HTMLString.Length == 0) {
							HTMLString.Append("<tr><th bgcolor=\"#F3A0A0\">不適用</th></tr>");
						}
						HTMLString.Append("<input type=\"hidden\" name=\"svcID\" value=\"");
						HTMLString.Append(sSvcID);
						HTMLString.Append("\">");
					} else {
						HTMLString.Append("<tr><th bgcolor=\"#F3A0A0\"沒有權限</th></tr>");
					}
				} else {
					HTMLString.Append("<tr><th bgcolor=\"#F3A0A0\"沒有權限</th></tr>");
				}
			} catch(Exception ex) {
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " DeleteAllData.cs.ShowItems(): " + ex.ToString());
				m_SportsLog.Close();
				HTMLString.Remove(0,HTMLString.Length);
				HTMLString.Append((string)HttpContext.Current.Application["accessErrorMsg"]);
			}

			return HTMLString.ToString();
		}

		public int Delete() {
			int iDeleteItems;
			int iDeleted = 0;
			int iDelIdx = 0;
			string sSvcID = "";
			
			sSvcID = HttpContext.Current.Request.Form["svcID"].Trim();

			try {
				if(Convert.ToInt32(m_Role) > 10) {
					char[] delimiter = new char[] {','};
					string[] arrDeleteItems;
					string[] arrCmdType;
					string[] arrParam;
					try {
						arrDeleteItems = (string[])HttpContext.Current.Request.Form["ItemToDelete"].Split(delimiter);
						iDeleteItems = arrDeleteItems.Length;
					} catch(Exception) {
						arrDeleteItems = new string[0];
						iDeleteItems = 0;
					}
					try {
						arrParam = (string[])HttpContext.Current.Request.Form["param"].Split(delimiter);
						arrCmdType = (string[])HttpContext.Current.Request.Form["cmdtype"].Split(delimiter);
					} catch(Exception) {
						arrParam = new string[0];
						arrCmdType = new string[0];
					}

					//Double check for user right
					SQLString.Remove(0,SQLString.Length);
					SQLString.Append("select CACCESS_RIGHT from SVCCFG where ISVC_NO=");
					SQLString.Append(sSvcID);
					SQLString.Append(" and DEL_TYPE='W'");
					string sSvcRight = m_SportsDBMgr.ExecuteQueryString(SQLString.ToString());
					m_SportsDBMgr.Close();

					if(Convert.ToInt32(m_Role) >= Convert.ToInt32(sSvcRight)) {
						if(iDeleteItems > 0) {	//delete item(s)
							for(iDeleted = 0; iDeleted < iDeleteItems; iDeleted++) {
								iDelIdx = Convert.ToInt32(arrDeleteItems[iDeleted]);
								SQLString.Remove(0,SQLString.Length);
								if (sSvcID == "14") {
									if (arrParam[iDelIdx] == "OTHODDS_SPT") {
										SQLString.Append("delete from OTHERSOCCERINFO");
										m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
										m_SportsDBMgr.Close();
									} else if (arrParam[iDelIdx] == "OTHGOAL") {
										SQLString.Append("delete from OTHERSOCCERGOALINFO");
										m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
										m_SportsDBMgr.Close();
										SQLString.Remove(0,SQLString.Length);
										SQLString.Append("delete from TIMEOFOTHERGAME_INFO");
										m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
										m_SportsDBMgr.Close();
									} else if (arrParam[iDelIdx] == "RES") {
										SQLString.Append("delete from OTHRESULTINFO");
										m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
										m_SportsDBMgr.Close();
									}
								} else if (sSvcID == "15") {
									if (arrParam[iDelIdx] == "Analysis") {
										SQLString.Append("delete from ANALYSIS_BG_INFO");
										m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
										m_SportsDBMgr.Close();
										SQLString.Remove(0,SQLString.Length);
										SQLString.Append("delete from ANALYSIS_HISTORY_INFO");
										m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
										m_SportsDBMgr.Close();
										SQLString.Remove(0,SQLString.Length);
										SQLString.Append("delete from ANALYSIS_REMARK_INFO");
										m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
										m_SportsDBMgr.Close();
									} else if (arrParam[iDelIdx] == "AnalyStat") {
										SQLString.Append("delete from ANALYSIS_STAT_INFO");
										m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
										m_SportsDBMgr.Close();
									} else if (arrParam[iDelIdx] == "AnalyRecent") {
										SQLString.Append("delete from ANALYSIS_RECENT_INFO");
										m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
										m_SportsDBMgr.Close();
									}
								}
							}
						}

						//write log
						m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
						m_SportsLog.SetFileName(0,LOGFILESUFFIX);
						m_SportsLog.Open();
						m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " DelWebDetails.cs: delete " + iDeleteItems.ToString() + " items (" + HttpContext.Current.Session["user_name"] + ")");
						m_SportsLog.Close();
					} else {
						iDeleted = -99;
						m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
						m_SportsLog.SetFileName(0,LOGFILESUFFIX);
						m_SportsLog.Open();
						m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " DelWebDetails.cs: " + HttpContext.Current.Session["user_name"].ToString() + " did not allow to delete web data (" + HttpContext.Current.Session["user_name"] + ")");
						m_SportsLog.Close();
					}	//no right to resend info
				} else {
					iDeleted = -99;
					m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
					m_SportsLog.SetFileName(0,LOGFILESUFFIX);
					m_SportsLog.Open();
					m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " DelWebDetails.cs: " + HttpContext.Current.Session["user_name"].ToString() + " did not allow to delete web data (" + HttpContext.Current.Session["user_name"] + ")");
					m_SportsLog.Close();
				}	//no right to resend info
			} catch(Exception ex) {
				iDeleted = -1;
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " DelWebDetails.cs.Delete(): " + ex.ToString());
				m_SportsLog.Close();
			}

			return iDeleted;
		}
	}
}