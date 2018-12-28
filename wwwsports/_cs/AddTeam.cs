/*
Objective:
Add a new team

Last updated:
12 July 2004, Chapman Choi

Error Code:
-99:	Program Exception
-1:		Asia Team Name existed
-2:		HKJC Team Name existed
-3:		HKJC Team Alias existed
-4:		Macau Team Name existed

C#.NET complier statement:
csc /t:library /out:..\bin\AddTeam.dll /r:..\bin\DBManager.dll;..\bin\Files.dll AddTeam.cs
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
[assembly:AssemblyDescription("一般設定 -> 隊伍管理 -> 新增")]
[assembly:AssemblyProduct("Sports.NET")]
[assembly:AssemblyTitle("Sports.NET DLL")]
[assembly:AssemblyVersion("1.0.*")]
namespace SportsUtil {
	public class AddTeam {
		const string LOGFILESUFFIX = "log";
		OleDbDataReader m_SportsOleReader;
		DBManager m_SportsDBMgr;
		Files m_SportsLog;
		StringBuilder SQLString;

		public AddTeam(string Connection) {
			m_SportsDBMgr = new DBManager();
			m_SportsDBMgr.ConnectionString = Connection;
			m_SportsLog = new Files();
			SQLString = new StringBuilder();
		}

		public string GetLeagues() {
			string sRtn;

			try {
				sRtn = "<select name=\"leagID\">";
				m_SportsOleReader = m_SportsDBMgr.ExecuteQuery("select LEAG_ID,LEAGNAME from LEAGINFO");
				while(m_SportsOleReader.Read()) {
					//retrieve all leagues from table
					sRtn += "<option value=\"" + m_SportsOleReader.GetString(0).Trim() + "\">";
					sRtn += m_SportsOleReader.GetString(1).Trim() + "</option>";
				}
				m_SportsOleReader.Close();
				m_SportsDBMgr.Close();
				m_SportsDBMgr.Dispose();
				sRtn += "</select>";
			} catch(Exception ex) {
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " AddTeam.cs.GetLeagues(): " + ex.ToString());
				m_SportsLog.Close();
				sRtn = (string)HttpContext.Current.Application["accessErrorMsg"];
			}

			return sRtn;
		}

		public int Add(string sTeam, string sHKJCName, string sHKJCNameAlias, string sMCTeam) {
			int iMax = 0;
			int iRecUpd = 0;
			int iTeamExisted = 0;
			int iMappingExisted = 0;
			string sTeamID;
			string sLeagID;
			string sEngName;
			string sVenue;
			string sContinent;
			string sCountry;
			string sCity;

			sLeagID = HttpContext.Current.Request.Form["leagID"].Trim();
			sEngName = HttpContext.Current.Request.Form["EnglishTeam"];
			if(sEngName != null) {
				if(sEngName.Trim().Equals("")) sEngName = null;
				else sEngName = sEngName.Trim();
			}
			sVenue = HttpContext.Current.Request.Form["venue"];
			if(sVenue != null) {
				if(sVenue.Trim().Equals("")) sVenue = null;
				else sVenue = sVenue.Trim();
			}
			sContinent = HttpContext.Current.Request.Form["continent"];
			if(sContinent != null) {
				if(sContinent.Trim().Equals("")) sContinent = null;
				else sContinent = sContinent.Trim();
			}
			sCountry = HttpContext.Current.Request.Form["country"];
			if(sCountry != null) {
				if(sCountry.Trim().Equals("")) sCountry = null;
				else sCountry = sCountry.Trim();
			}
			sCity = HttpContext.Current.Request.Form["city"];
			if(sCity != null) {
				if(sCity.Trim().Equals("")) sCity = null;
				else sCity = sCity.Trim();
			}

			try {
				//Check for Asia Team
				SQLString.Remove(0,SQLString.Length);
				SQLString.Append("select COUNT(TEAM_ID) from TEAMINFO where TEAMNAME='");
				SQLString.Append(sTeam);
				SQLString.Append("'");
				iTeamExisted = m_SportsDBMgr.ExecuteScalar(SQLString.ToString());
				m_SportsDBMgr.Close();
				if(iTeamExisted > 0) {
					iRecUpd = -1;
					m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
					m_SportsLog.SetFileName(0,LOGFILESUFFIX);
					m_SportsLog.Open();
					m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " AddTeam.cs: Add Team failed, TEAMNAME '" + sTeam + "' existed (" + HttpContext.Current.Session["user_name"] + ")");
					m_SportsLog.Close();
				} else {
					//Check for HKJC Team
					SQLString.Remove(0,SQLString.Length);
					SQLString.Append("select COUNT(TEAM_ID) from TEAMINFO where CHKJCNAME='");
					SQLString.Append(sHKJCName);
					SQLString.Append("'");
					iTeamExisted = m_SportsDBMgr.ExecuteScalar(SQLString.ToString());
					m_SportsDBMgr.Close();
					if(iTeamExisted > 0) {
						iRecUpd = -2;
						m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
						m_SportsLog.SetFileName(0,LOGFILESUFFIX);
						m_SportsLog.Open();
						m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " AddTeam.cs: Add Team failed, CHKJCNAME '" + sHKJCName + "' existed (" + HttpContext.Current.Session["user_name"] + ")");
						m_SportsLog.Close();
					} else {
						//Check for HKJC Team Alias
						SQLString.Remove(0,SQLString.Length);
						SQLString.Append("select COUNT(TEAM_ID) from TEAMINFO where CHKJCSHORTNAME='");
						SQLString.Append(sHKJCNameAlias);
						SQLString.Append("'");
						iTeamExisted = m_SportsDBMgr.ExecuteScalar(SQLString.ToString());
						m_SportsDBMgr.Close();
						if(iTeamExisted > 0) {
							iRecUpd = -3;
							m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
							m_SportsLog.SetFileName(0,LOGFILESUFFIX);
							m_SportsLog.Open();
							m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " AddTeam.cs: Add Team failed, CHKJCSHORTNAME '" + sHKJCNameAlias + "' existed (" + HttpContext.Current.Session["user_name"] + ")");
							m_SportsLog.Close();
						} else {
							//Check for Macau Team
							SQLString.Remove(0,SQLString.Length);
							SQLString.Append("select COUNT(TEAM_ID) from TEAMINFO where CMACAUNAME='");
							SQLString.Append(sMCTeam);
							SQLString.Append("'");
							iTeamExisted = m_SportsDBMgr.ExecuteScalar(SQLString.ToString());
							m_SportsDBMgr.Close();
							if(iTeamExisted > 0) {
								iRecUpd = -4;
								m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
								m_SportsLog.SetFileName(0,LOGFILESUFFIX);
								m_SportsLog.Open();
								m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " AddTeam.cs: Add Team failed, CMACAUNAME '" + sMCTeam + "' existed (" + HttpContext.Current.Session["user_name"] + ")");
								m_SportsLog.Close();
							} else {
								//No team name conflict
								//get Max team Id from teaminfo
								SQLString.Remove(0,SQLString.Length);
								SQLString.Append("select MAX(TEAM_ID) from TEAMINFO");
								m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
								if(m_SportsOleReader.Read()) {
									if(!m_SportsOleReader.IsDBNull(0)) {
										iMax = m_SportsOleReader.GetInt32(0);
									}
								}
								m_SportsOleReader.Close();
								m_SportsDBMgr.Close();
								iMax++;
								sTeamID = iMax.ToString();

								//insert team into teaminfo
								SQLString.Remove(0,SQLString.Length);
								SQLString.Append("insert into TEAMINFO values(");
								SQLString.Append(sTeamID);
								SQLString.Append(",'");
								SQLString.Append(sTeam);
								SQLString.Append("',");
								if(sCountry == null) {
									SQLString.Append("null,");
								}	else {
									SQLString.Append("'");
									SQLString.Append(sCountry);
									SQLString.Append("',");
								}
								if(sCity == null) {
									SQLString.Append("null,");
								}	else {
									SQLString.Append("'");
									SQLString.Append(sCity);
									SQLString.Append("',");
								}
								if(sVenue == null) {
									SQLString.Append("null,");
								}	else {
									SQLString.Append("'");
									SQLString.Append(sVenue);
									SQLString.Append("',");
								}
								if(sContinent == null) {
									SQLString.Append("null,");
								}	else {
									SQLString.Append("'");
									SQLString.Append(sContinent);
									SQLString.Append("',");
								}
								if(sEngName == null) {
									SQLString.Append("null,");
								} else {
									SQLString.Append("'");
									SQLString.Append(sEngName);
									SQLString.Append("',");
								}
								SQLString.Append("'");
								SQLString.Append(sMCTeam);
								SQLString.Append("','");
								SQLString.Append(sHKJCName);
								SQLString.Append("','");
								SQLString.Append(sHKJCNameAlias);
								SQLString.Append("',CURRENT_TIMESTAMP)");
								iRecUpd = m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
								m_SportsDBMgr.Close();

								//write log
								m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
								m_SportsLog.SetFileName(0,LOGFILESUFFIX);
								m_SportsLog.Open();
								m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " AddTeam.cs: " + SQLString.ToString() + " (" + HttpContext.Current.Session["user_name"] + ")");
								m_SportsLog.Close();

								//check team-league relationship existed
								SQLString.Remove(0,SQLString.Length);
								SQLString.Append("select COUNT(LEAG_ID) from ID_INFO where TEAM_ID=");
								SQLString.Append(sTeamID);
								SQLString.Append(" and LEAG_ID='");
								SQLString.Append(sLeagID);
								SQLString.Append("'");
								iMappingExisted = m_SportsDBMgr.ExecuteScalar(SQLString.ToString());
								m_SportsDBMgr.Close();
								if(iMappingExisted == 0) {	//relationship did not existed
									SQLString.Remove(0,SQLString.Length);
									SQLString.Append("insert into ID_INFO values('");
									SQLString.Append(sLeagID);
									SQLString.Append("',");
									SQLString.Append(sTeamID);
									SQLString.Append(")");
									m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
									m_SportsDBMgr.Close();
									iRecUpd++;
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
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " AddTeam.cs.Add(): " + ex.ToString());
				m_SportsLog.Close();
			}

			return iRecUpd;
		}
	}
}