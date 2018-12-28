/*
Objective:
Add a new team

Last updated:
1 Nov 2003 by Chapman

C#.NET complier statement:
csc /t:library /out:..\..\bin\BSKNewTeam.dll /r:..\..\bin\DBManager.dll;..\..\bin\Files.dll BSKNewTeam.cs
*/

using System;
using System.Data.OleDb;
using System.Reflection;
using System.Web;
using TDL.DB;
using TDL.IO;

[assembly:AssemblyCompany("TDSL")]
[assembly:AssemblyCopyright("(c) 2003 TDSL. All rights reserved. Created on 24 Oct 2003.")]
[assembly:AssemblyDescription("籃球資訊 -> 隊伍管理 -> 新增")]
[assembly:AssemblyProduct("Sports.NET")]
[assembly:AssemblyTitle("Sports.NET DLL")]
[assembly:AssemblyVersion("1.0.*")]
namespace SportsUtil {
	public class BSKNewTeam {
		const string LOGFILESUFFIX = "log";
		OleDbDataReader m_SportsOleReader;
		DBManager m_SportsDBMgr;
		Files m_SportsLog;

		public BSKNewTeam(string Connection) {
			m_SportsDBMgr = new DBManager();
			m_SportsDBMgr.ConnectionString = Connection;
			m_SportsLog = new Files();
		}

		public string GetLeagues() {
			string sRtn;

			try {
				sRtn = "<select name=\"leagID\">";
				m_SportsOleReader = m_SportsDBMgr.ExecuteQuery("select CLEAG_ID, CLEAGUE from LEAGUE_INFO where CLEAGUETYPE='0'");
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
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " BSKNewTeam.cs.GetLeagues(): " + ex.ToString());
				m_SportsLog.Close();
				sRtn = (string)HttpContext.Current.Application["accessErrorMsg"];
			}

			return sRtn;
		}

		public int Add(string sTeam) {
			int iRecUpd = 0;
			int iTeamExisted = 0;
			int iMappingExisted = 0;
			int iMax = 0;
			string sTeamID;
			string sLeagID;
			string sContinent;
			string sCountry;
			string sCity;
			string sVenue;
			string updateQuery;

			sLeagID = HttpContext.Current.Request.Form["leagID"].Trim();
			sContinent = HttpContext.Current.Request.Form["continent"].Trim();
			if(sContinent == "") sContinent = null;
			sCountry = HttpContext.Current.Request.Form["country"].Trim();
			if(sCountry == "") sCountry = null;
			sCity = HttpContext.Current.Request.Form["city"].Trim();
			if(sCity == "") sCity = null;
			sVenue = HttpContext.Current.Request.Form["venue"].Trim();
			if(sVenue == "") sVenue = null;

			try {
				//check existed team
				updateQuery = "select COUNT(CTEAM_ID) from TEAM_INFO where CTEAM='" + sTeam + "'";
				iTeamExisted = m_SportsDBMgr.ExecuteScalar(updateQuery);
				m_SportsDBMgr.Close();
				if(iTeamExisted == 0) {	//team did not existed
					//get Max team Id from teaminfo
					m_SportsOleReader = m_SportsDBMgr.ExecuteQuery("select MAX(CTEAM_ID) from TEAM_INFO");
					if(m_SportsOleReader.Read()) {
						if(!m_SportsOleReader.IsDBNull(0)) {
							iMax = Convert.ToInt32(m_SportsOleReader.GetString(0).Trim());
						}
					}
					m_SportsOleReader.Close();
					m_SportsDBMgr.Close();
					iMax++;
					sTeamID = iMax.ToString("D3");

					//insert team into teaminfo
					updateQuery = "insert into TEAM_INFO values('" + sTeamID + "','" + sTeam + "','" + sCountry + "','" + sCity + "','" + sVenue + "','" + sContinent + "')";
					m_SportsDBMgr.ExecuteNonQuery(updateQuery);
					m_SportsDBMgr.Close();
					iRecUpd++;

					//check team-league relationship existed
					updateQuery = "select COUNT(CLEAG_ID) from IDMAP_INFO where CTEAM_ID='" + sTeamID + "' and CLEAG_ID='" + sLeagID + "'";
					iMappingExisted = m_SportsDBMgr.ExecuteScalar(updateQuery);
					m_SportsDBMgr.Close();
					if(iMappingExisted == 0) {	//relationship did not existed
						updateQuery = "insert into IDMAP_INFO values('" + sLeagID + "','" + sTeamID + "')";
						m_SportsDBMgr.ExecuteNonQuery(updateQuery);
						m_SportsDBMgr.Close();
						iRecUpd++;
					}

					//write log
					m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
					m_SportsLog.SetFileName(0,LOGFILESUFFIX);
					m_SportsLog.Open();
					m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " BSKNewTeam.cs: Add a new team <" + sTeam + "> (" + HttpContext.Current.Session["user_name"] + ")");
					m_SportsLog.Close();
				}	else {	//team existed
					iRecUpd = -99;
					//write log
					m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
					m_SportsLog.SetFileName(0,LOGFILESUFFIX);
					m_SportsLog.Open();
					m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " BSKNewTeam.cs: Team cannot be added because <" + sTeam + "> existed (" + HttpContext.Current.Session["user_name"] + ")");
					m_SportsLog.Close();
				}
				m_SportsDBMgr.Dispose();
			}	catch(Exception ex) {
				iRecUpd = -1;
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " BSKNewTeam.cs.Add(): " + ex.ToString());
				m_SportsLog.Close();
			}

			return iRecUpd;
		}
	}
}