/*
Objective:
Add a new league

Last updated:
10 July 2004, Chapman Choi

Error Code:
-99:	Program Exception
-1:		Asia League Name existed
-2:		Asia League Alias existed
-3:		HKJC League Name existed
-4:		HKJC League Alias existed
-5:		Macau League Name existed

C#.NET complier statement:
csc /t:library /out:..\bin\AddLeague.dll /r:..\bin\DBManager.dll;..\bin\Files.dll AddLeague.cs
*/

using System;
using System.Data.OleDb;
using System.Text;
using System.Reflection;
using System.Web;
using TDL.DB;
using TDL.IO;

[assembly:AssemblyCompany("TDSL")]
[assembly:AssemblyCopyright("(c) 2003 TDSL. All rights reserved. Created on 19 June 2003.")]
[assembly:AssemblyDescription("一般設定 -> 聯賽管理 -> 新增")]
[assembly:AssemblyProduct("Sports.NET")]
[assembly:AssemblyTitle("Sports.NET DLL")]
[assembly:AssemblyVersion("1.0.*")]
namespace SportsUtil {
	public class AddLeague {
		const string LOGFILESUFFIX = "log";
		OleDbDataReader m_SportsOleReader;
		DBManager m_SportsDBMgr;
		Files m_SportsLog;
		StringBuilder SQLString;

		public AddLeague(string Connection) {
			m_SportsDBMgr = new DBManager();
			m_SportsDBMgr.ConnectionString = Connection;
			m_SportsLog = new Files();
			SQLString = new StringBuilder();
		}

		public int Add(string sLeague, string sAlias, string sHKJCLeague, string sHKJCLeagueAlias, string sMCLeague) {
			int iRecUpd = 0;
			int iExisted = 0;
			int iMax = 0;
			int iUpdIdx = 0;
			string sType;
			string sOrg;
			string sLeagID;
			string sEnglishLeague;

			sType = HttpContext.Current.Request.Form["leaguetype"];
			sOrg = HttpContext.Current.Request.Form["org"];
			if(sOrg != null) {
				if(sOrg.Trim().Equals("")) sOrg = null;
				else sOrg = sOrg.Trim();
			}
			sEnglishLeague = HttpContext.Current.Request.Form["EnglishLeague"];
			if(sEnglishLeague != null) {
				if(sEnglishLeague.Trim().Equals("")) sEnglishLeague = null;
				else sEnglishLeague = sEnglishLeague.Trim();
			}

			try {
				//Check for Asia League
				SQLString.Remove(0, SQLString.Length);
				SQLString.Append("SELECT COUNT(LEAG_ID) FROM LEAGINFO WHERE LEAGNAME='");
				SQLString.Append(sLeague);
				SQLString.Append("'");
				iExisted = m_SportsDBMgr.ExecuteScalar(SQLString.ToString());
				m_SportsDBMgr.Close();
				if(iExisted > 0) {
					iRecUpd = -1;
					m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
					m_SportsLog.SetFileName(0,LOGFILESUFFIX);
					m_SportsLog.Open();
					m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " AddLeague.cs: Add League failed, LEAGNAME '" + sLeague + "' existed (" + HttpContext.Current.Session["user_name"] + ")");
					m_SportsLog.Close();
				} else {
					//Check for Asia League Alias
					SQLString.Remove(0, SQLString.Length);
					SQLString.Append("SELECT COUNT(LEAG_ID) FROM LEAGINFO WHERE ALIAS='");
					SQLString.Append(sAlias);
					SQLString.Append("'");
					iExisted = m_SportsDBMgr.ExecuteScalar(SQLString.ToString());
					m_SportsDBMgr.Close();
					if(iExisted > 0) {
						iRecUpd = -2;
						m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
						m_SportsLog.SetFileName(0,LOGFILESUFFIX);
						m_SportsLog.Open();
						m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " AddLeague.cs: Add League failed, ALIAS '" + sAlias + "' existed (" + HttpContext.Current.Session["user_name"] + ")");
						m_SportsLog.Close();
					} else {
						//Check for HKJC League
						SQLString.Remove(0, SQLString.Length);
						SQLString.Append("SELECT COUNT(LEAG_ID) FROM LEAGINFO WHERE CHKJCNAME='");
						SQLString.Append(sHKJCLeague);
						SQLString.Append("'");
						iExisted = m_SportsDBMgr.ExecuteScalar(SQLString.ToString());
						m_SportsDBMgr.Close();
						if(iExisted > 0) {
							iRecUpd = -3;
							m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
							m_SportsLog.SetFileName(0,LOGFILESUFFIX);
							m_SportsLog.Open();
							m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " AddLeague.cs: Add League failed, CHKJCNAME '" + sHKJCLeague + "' existed (" + HttpContext.Current.Session["user_name"] + ")");
							m_SportsLog.Close();
						} else {
							//Check for HKJC League Alias
							SQLString.Remove(0, SQLString.Length);
							SQLString.Append("SELECT COUNT(LEAG_ID) FROM LEAGINFO WHERE CHKJCSHORTNAME='");
							SQLString.Append(sHKJCLeagueAlias);
							SQLString.Append("'");
							iExisted = m_SportsDBMgr.ExecuteScalar(SQLString.ToString());
							m_SportsDBMgr.Close();
							if(iExisted > 0) {
								iRecUpd = -4;
								m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
								m_SportsLog.SetFileName(0,LOGFILESUFFIX);
								m_SportsLog.Open();
								m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " AddLeague.cs: Add League failed, CHKJCSHORTNAME '" + sHKJCLeagueAlias + "' existed (" + HttpContext.Current.Session["user_name"] + ")");
								m_SportsLog.Close();
							} else {
								//Check for Macau League
								SQLString.Remove(0, SQLString.Length);
								SQLString.Append("SELECT COUNT(LEAG_ID) FROM LEAGINFO WHERE CMACAUNAME='");
								SQLString.Append(sMCLeague);
								SQLString.Append("'");
								iExisted = m_SportsDBMgr.ExecuteScalar(SQLString.ToString());
								m_SportsDBMgr.Close();
								if(iExisted > 0) {
									iRecUpd = -5;
									m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
									m_SportsLog.SetFileName(0,LOGFILESUFFIX);
									m_SportsLog.Open();
									m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " AddLeague.cs: Add League failed, CMACAUNAME '" + sMCLeague + "' existed (" + HttpContext.Current.Session["user_name"] + ")");
									m_SportsLog.Close();
								} else {
									//No league name conflict
									string[] arrUserID;

									//get Max League ID from leaginfo
									SQLString.Remove(0, SQLString.Length);
									SQLString.Append("select MAX(LEAG_ID) from LEAGINFO");
									m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
									if(m_SportsOleReader.Read()) {
										if(!m_SportsOleReader.IsDBNull(0)) {
											iMax = Convert.ToInt32(m_SportsOleReader.GetString(0).Trim());
										}
									}
									m_SportsOleReader.Close();
									m_SportsDBMgr.Close();
									iMax++;
									sLeagID = iMax.ToString("D3");

									//insert into leaginfo
									SQLString.Remove(0, SQLString.Length);
									SQLString.Append("insert into LEAGINFO values('");
									SQLString.Append(sLeagID);
									SQLString.Append("','");
									SQLString.Append(sLeague);
									SQLString.Append("',");
									if(sOrg == null) {
										SQLString.Append("null");
									} else {
										SQLString.Append("'");
										SQLString.Append(sOrg);
										SQLString.Append("'");
									}
									SQLString.Append(",'");
									SQLString.Append(sAlias);
									SQLString.Append("','");
									SQLString.Append(sType);
									SQLString.Append("',null,");
									if(sEnglishLeague == null) {
										SQLString.Append("null");
									} else {
										SQLString.Append("'");
										SQLString.Append(sEnglishLeague);
										SQLString.Append("'");
									}
									SQLString.Append(",'");
									SQLString.Append(sMCLeague);
									SQLString.Append("','");
									SQLString.Append(sHKJCLeague);
									SQLString.Append("','");
									SQLString.Append(sHKJCLeagueAlias);
									SQLString.Append("',CURRENT_TIMESTAMP)");
									iRecUpd = m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
									m_SportsDBMgr.Close();

									//write log
									m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
									m_SportsLog.SetFileName(0,LOGFILESUFFIX);
									m_SportsLog.Open();
									m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " AddLeague.cs: " + SQLString.ToString() + " (" + HttpContext.Current.Session["user_name"] + ")");
									m_SportsLog.Close();

									//select max rec_no from userprofile_info
									SQLString.Remove(0, SQLString.Length);
									SQLString.Append("SELECT MAX(IREC_NO) FROM USERPROFILE_INFO");
									iMax = m_SportsDBMgr.ExecuteScalar(SQLString.ToString());
									m_SportsDBMgr.Close();

									//get all User ID into array
									SQLString.Remove(0, SQLString.Length);
									SQLString.Append("select count(USER_ID) from LOGININFO");
									arrUserID = new string[m_SportsDBMgr.ExecuteScalar(SQLString.ToString())];
									m_SportsDBMgr.Close();

									SQLString.Remove(0, SQLString.Length);
									SQLString.Append("select USER_ID from LOGININFO");
									m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
									while(m_SportsOleReader.Read()) {
										arrUserID[iUpdIdx] = m_SportsOleReader.GetInt32(0).ToString();
										iUpdIdx++;
									}
									m_SportsOleReader.Close();
									m_SportsDBMgr.Close();

									//insert relation into userprofile_info
									for(iUpdIdx=0;iUpdIdx<arrUserID.Length;iUpdIdx++) {
										iMax++;
										SQLString.Remove(0, SQLString.Length);
										SQLString.Append("insert into USERPROFILE_INFO values(");
										SQLString.Append(iMax.ToString());
										SQLString.Append(",");
										SQLString.Append(arrUserID[iUpdIdx]);
										SQLString.Append(",'");
										SQLString.Append(sLeagID);
										SQLString.Append("')");
										m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
										m_SportsDBMgr.Close();
									}
								}
							}
						}
					}
				}
			}	catch(Exception ex) {
				iRecUpd = -99;
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " AddLeague.cs.Add(): " + ex.ToString());
				m_SportsLog.Close();
			}

			return iRecUpd;
		}
	}
}