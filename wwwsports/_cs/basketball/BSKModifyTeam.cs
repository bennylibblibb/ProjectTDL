/*
Objective:
Modify a team data

Last updated:
24 Oct 2003 by Chapman

C#.NET complier statement:
csc /t:library /out:..\..\bin\BSKModifyTeam.dll /r:..\..\bin\DBManager.dll;..\..\bin\Files.dll BSKModifyTeam.cs
*/

using System;
using System.Collections;
using System.Data.OleDb;
using System.Reflection;
using System.Web;
using TDL.DB;
using TDL.IO;

[assembly:AssemblyCompany("TDSL")]
[assembly:AssemblyCopyright("(c) 2003 TDSL. All rights reserved. Created on 24 Oct 2003.")]
[assembly:AssemblyDescription("籃球資訊 -> 隊伍管理 -> 修改")]
[assembly:AssemblyProduct("Sports.NET")]
[assembly:AssemblyTitle("Sports.NET DLL")]
[assembly:AssemblyVersion("1.0.*")]
namespace SportsUtil {
	public class BSKModifyTeam {
		const string LOGFILESUFFIX = "log";
		OleDbDataReader m_SportsOleReader;
		DBManager m_SportsDBMgr;
		Files m_SportsLog;

		public BSKModifyTeam(string Connection) {
			m_SportsDBMgr = new DBManager();
			m_SportsDBMgr.ConnectionString = Connection;
			m_SportsLog = new Files();
		}

		public string GetTeam() {
			int iLeagueCount = 0;
			int iItemCount = 0;
			string sTeamID;
			string sLeagID;
			string sContinent = "";
			string sCountry = "";
			string sCity = "";
			string sVenue = "";
			string sRtn = "";
			string sTeam = "";
			ArrayList leagIDAL = new ArrayList();

			sTeamID = HttpContext.Current.Request.QueryString["teamID"].Trim();
			try {
				//get team-league relationship from id_info
				m_SportsOleReader = m_SportsDBMgr.ExecuteQuery("select id.CLEAG_ID, team.CTEAM, team.CCOUNTRY, team.CCITY, team.CVENUE, team.CCONTINENT from IDMAP_INFO id, TEAM_INFO team where id.CTEAM_ID='" + sTeamID + "' and team.CTEAM_ID='" + sTeamID + "'");
				while(m_SportsOleReader.Read()) {
					leagIDAL.Add(m_SportsOleReader.GetString(0).Trim());
					if(iItemCount == 0) {
						sTeam = m_SportsOleReader.GetString(1).Trim();

						if(m_SportsOleReader.IsDBNull(2)) sCountry = "";
						else sCountry = m_SportsOleReader.GetString(2).Trim();

						if(m_SportsOleReader.IsDBNull(3)) sCity = "";
						else sCity = m_SportsOleReader.GetString(3).Trim();

						if(m_SportsOleReader.IsDBNull(4)) sVenue = "";
						else sVenue = m_SportsOleReader.GetString(4).Trim();

						if(m_SportsOleReader.IsDBNull(5)) sContinent = "";
						else sContinent = m_SportsOleReader.GetString(5).Trim();
					}
					iItemCount++;
				}
				m_SportsOleReader.Close();
				m_SportsDBMgr.Close();

				//retrieve all leagues from table
				sRtn += "<tr align=\"center\">";
				sRtn += "<th colspan=\"2\">修改" + sTeam + "資料";
				sRtn += "</th></tr>";
				sRtn += "<tr><th align=\"center\"><font color=\"red\">*</font>所屬聯賽:</th><td>";
				m_SportsOleReader = m_SportsDBMgr.ExecuteQuery("select CLEAG_ID, CALIAS from LEAGUE_INFO where CLEAGUETYPE<>'2'");
				while(m_SportsOleReader.Read()) {
					sLeagID = m_SportsOleReader.GetString(0).Trim();
					sRtn += "<input type=\"checkbox\" name=\"leagID\" value=\"" + sLeagID + "\" ";
					if(leagIDAL.Contains(sLeagID)) sRtn += "checked";
					sRtn += ">" + m_SportsOleReader.GetString(1).Trim();
					iLeagueCount++;
					if(iLeagueCount%10 == 0) sRtn += "<br>";
				}
				m_SportsOleReader.Close();
				m_SportsDBMgr.Close();
				m_SportsDBMgr.Dispose();
				sRtn += "</td></tr>";

				sRtn += "<tr><th align=\"center\">所屬洲份:</th>";
				sRtn += "<td><input type=\"text\" name=\"continent\" value=\"" + sContinent + "\" maxlength=\"4\"></td></tr>";

				sRtn += "<tr><th align=\"center\">所屬國家:</th>";
				sRtn += "<td><input type=\"text\" name=\"country\" value=\"" + sCountry + "\" maxlength=\"5\"></td></tr>";

				sRtn += "<tr><th align=\"center\">所屬城巿:</th>";
				sRtn += "<td><input type=\"text\" name=\"city\" value=\"" + sCity + "\" maxlength=\"5\"></td></tr>";

				sRtn += "<tr><th align=\"center\">主場名稱:</th>";
				sRtn += "<td><input type=\"text\" name=\"venue\" value=\"" + sVenue + "\" maxlength=\"10\"></td></tr>";

				sRtn += "<input type=\"hidden\" name=\"teamID\" value=\"" + sTeamID + "\">";
			}	catch(Exception ex) {
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " BSKModifyTeam.cs.GetTeam(): " + ex.ToString());
				m_SportsLog.Close();
				sRtn = (string)HttpContext.Current.Application["accessErrorMsg"];
			}

			return sRtn;
		}

		public int Modify() {
			int iRecUpd = 0;
			int iLeagSelected = 0;
			int iUpdIdx = 0;
			string sTeamID;
			string sContinent;
			string sCountry;
			string sCity;
			string sVenue;
			string updateQuery;
			string sTeam = "";
			string[] arrLeagID;
			char[] delimiter = new char[] {','};

			sTeamID = HttpContext.Current.Request.Form["teamID"].Trim();
			try {
				arrLeagID = HttpContext.Current.Request.Form["leagID"].Split(delimiter);
				iLeagSelected = arrLeagID.Length;
			} catch(Exception) {
				arrLeagID = new string[0];
				iLeagSelected = 0;
			}
			sContinent = HttpContext.Current.Request.Form["continent"].Trim();
			if(sContinent == "") sContinent = null;
			sCountry = HttpContext.Current.Request.Form["country"].Trim();
			if(sCountry == "") sCountry = null;
			sCity = HttpContext.Current.Request.Form["city"].Trim();
			if(sCity == "") sCity = null;
			sVenue = HttpContext.Current.Request.Form["venue"].Trim();
			if(sVenue == "") sVenue = null;

			try {
				//get team name
				sTeam = m_SportsDBMgr.ExecuteQueryString("select CTEAM from TEAM_INFO where CTEAM_ID='" + sTeamID + "'");
				m_SportsDBMgr.Close();

				//update teaminfo
				updateQuery = "update TEAM_INFO set CCOUNTRY=";
				if(sCountry == null) updateQuery += "null";
				else updateQuery += "'" + sCountry + "'";
				updateQuery += ", CCITY=";
				if(sCity == null) updateQuery += "null";
				else updateQuery += "'" + sCity + "'";
				updateQuery += ", CVENUE=";
				if(sVenue == null) updateQuery += "null";
				else updateQuery += "'" + sVenue + "'";
				updateQuery += ", CCONTINENT=";
				if(sContinent == null) updateQuery += "null";
				else updateQuery += "'" + sContinent + "'";
				updateQuery += " where CTEAM_ID='" + sTeamID + "'";
				iRecUpd = m_SportsDBMgr.ExecuteNonQuery(updateQuery);
				m_SportsDBMgr.Close();

				//update team-league relationship
				//reset id_info w.r.t. team first
				updateQuery = "delete from IDMAP_INFO where CTEAM_ID='" + sTeamID + "'";
				m_SportsDBMgr.ExecuteNonQuery(updateQuery);
				m_SportsDBMgr.Close();

				if(iLeagSelected != 0) {	//teams join to id_info
					for(iUpdIdx=0;iUpdIdx<iLeagSelected;iUpdIdx++) {
						updateQuery = "insert into IDMAP_INFO values('" + arrLeagID[iUpdIdx] + "','" + sTeamID + "')";
						m_SportsDBMgr.ExecuteNonQuery(updateQuery);
						m_SportsDBMgr.Close();
					}
				}
				m_SportsDBMgr.Dispose();

				//write log
				m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " BSKModifyTeam.cs: modify team <" + sTeam + "> (" + HttpContext.Current.Session["user_name"] + ")");
				m_SportsLog.Close();
			}	catch(Exception ex) {
				iRecUpd = -1;
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " BSKModifyTeam.cs.Modiy(): " + ex.ToString());
				m_SportsLog.Close();
			}

			return iRecUpd;
		}
	}
}