/*
Objective:
Add a new league

Last updated:
1 Nov 2003 by Chapman

C#.NET complier statement:
csc /t:library /out:..\..\bin\BSKNewLeague.dll /r:..\..\bin\DBManager.dll;..\..\bin\Files.dll BSKNewLeague.cs
*/

using System;
using System.Data.OleDb;
using System.Reflection;
using System.Web;
using TDL.DB;
using TDL.IO;

[assembly:AssemblyCompany("TDSL")]
[assembly:AssemblyCopyright("(c) 2003 TDSL. All rights reserved. Created on 24 Oct 2003.")]
[assembly:AssemblyDescription("籃球資訊 -> 聯賽管理 -> 新增")]
[assembly:AssemblyProduct("Sports.NET")]
[assembly:AssemblyTitle("Sports.NET DLL")]
[assembly:AssemblyVersion("1.0.*")]
namespace SportsUtil {
	public class BSKNewLeague {
		const string LOGFILESUFFIX = "log";
		OleDbDataReader m_SportsOleReader;
		DBManager m_SportsDBMgr;
		Files m_SportsLog;

		public BSKNewLeague(string Connection) {
			m_SportsDBMgr = new DBManager();
			m_SportsDBMgr.ConnectionString = Connection;
			m_SportsLog = new Files();
		}

		public int Add(string sLeague, string sAlias) {
			int iRecUpd = 0;
			int iExisted = 0;
			int iMax = 0;
			string updateQuery;
			string sType;
			string sLeagID;

			sType = HttpContext.Current.Request.Form["leaguetype"];
			try {
				//check existed league
				if(sLeague != null && sAlias != null) {
					if(!sLeague.Equals("") && !sAlias.Equals("")) {
						updateQuery = "SELECT COUNT(CLEAG_ID) FROM LEAGUE_INFO WHERE CLEAGUE='" + sLeague + "' OR CALIAS='" + sAlias + "'";
						iExisted = m_SportsDBMgr.ExecuteScalar(updateQuery);
						if(iExisted == 0) {
							//get Max League ID from LEAGUE_INFO
							m_SportsOleReader = m_SportsDBMgr.ExecuteQuery("select MAX(CLEAG_ID) from LEAGUE_INFO");
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
							updateQuery = "insert into LEAGUE_INFO values ('"+ sLeagID + "','" + sLeague + "','" + sAlias + "',null ,'" + sType + "')";
							iRecUpd = m_SportsDBMgr.ExecuteNonQuery(updateQuery);

							//write log
							m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
							m_SportsLog.SetFileName(0,LOGFILESUFFIX);
							m_SportsLog.Open();
							m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " BSKNewLeague.cs: Add a new league <" + sLeague + "> (" + HttpContext.Current.Session["user_name"] + ")");
							m_SportsLog.Close();
						}	else {
							iRecUpd = -99;
							//write log
							m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
							m_SportsLog.SetFileName(0,LOGFILESUFFIX);
							m_SportsLog.Open();
							m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " BSKNewLeague.cs: League cannot be added because <" + sLeague + "> existed (" + HttpContext.Current.Session["user_name"] + ")");
							m_SportsLog.Close();
						}
						m_SportsDBMgr.Dispose();
					}	else {
						iRecUpd = 0;
					}
				}	else {
					iRecUpd = 0;
				}

				if(iRecUpd == 0) {
					//write log
					m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
					m_SportsLog.SetFileName(0,LOGFILESUFFIX);
					m_SportsLog.Open();
					m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " BSKNewLeague.cs: League cannot be added because league name is null or empty (" + HttpContext.Current.Session["user_name"] + ")");
					m_SportsLog.Close();
				}
			}	catch(Exception ex) {
				iRecUpd = -1;
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " BSKNewLeague.cs.Add(): " + ex.ToString());
				m_SportsLog.Close();
			}

			return iRecUpd;
		}
	}
}