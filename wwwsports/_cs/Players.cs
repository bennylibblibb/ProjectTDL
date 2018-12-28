/*
Objective:
Update players information such as player name, number, position, country etc.

Last updated:
12 July 2004, Chapman Choi

C#.NET complier statement:
csc /t:library /out:..\bin\Players.dll /r:..\bin\DBManager.dll;..\bin\Files.dll Players.cs
*/

using System;
using System.Collections;
using System.Collections.Specialized;
using System.Data.OleDb;
using System.Reflection;
using System.Web;
using TDL.DB;
using TDL.IO;

[assembly:AssemblyCompany("TDSL")]
[assembly:AssemblyCopyright("(c) 2003 TDSL. All rights reserved.")]
[assembly:AssemblyDescription("足球資訊 -> 球員")]
[assembly:AssemblyProduct("Sports.NET")]
[assembly:AssemblyTitle("Sports.NET DLL")]
[assembly:AssemblyVersion("1.0.*")]
namespace SportsUtil {
	public class Players {
		const int TOTALRECORDS = 35;
		const string LOGFILESUFFIX = "log";
		const string DEFAULTTEAMID = "000";
		OleDbDataReader m_SportsOleReader;
		DBManager m_SportsDBMgr;
		Files m_SportsLog;

		public Players(string Connection) {
			m_SportsDBMgr = new DBManager();
			m_SportsDBMgr.ConnectionString = Connection;
			m_SportsLog = new Files();
		}

		public int NumberOfRecords {
			get {
				return TOTALRECORDS;
			}
		}

		public string GetTeamPlayers() {
			int iIdx = 0, iRec = 0;
			string retrieveQuery, sLeagID, sTeamID, sLeague = "", sAlias = "", sRtn = "";
			string[] arrPos;
			NameValueCollection teamNVC = new NameValueCollection();

			sTeamID = HttpContext.Current.Request.QueryString["teamID"].Trim();
			sLeagID = HttpContext.Current.Request.QueryString["leagID"].Trim();
			arrPos = (string[])HttpContext.Current.Application["positionItemsArray"];

			try {
				//retrieve teams w.r.t. leag id
				retrieveQuery = "select team.team_id, team.teamname, leag.leagname from teaminfo team, leaginfo leag, id_info id ";
				retrieveQuery += "where leag.leag_id='" + sLeagID + "' and leag.leag_id=id.leag_id ";
				retrieveQuery += "and team.team_id=id.team_id order by team.team_id";
				m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(retrieveQuery);
				while(m_SportsOleReader.Read()) {
					if(!m_SportsOleReader.IsDBNull(0)) {
						teamNVC.Add(m_SportsOleReader.GetInt32(0).ToString(),m_SportsOleReader.GetString(1).Trim());
						iRec++;
					}
					if(sLeague.Equals("")) sLeague = m_SportsOleReader.GetString(2).Trim();
				}
				m_SportsOleReader.Close();
				m_SportsDBMgr.Close();

				//assign team id if it is default value
				if(sTeamID.Equals(DEFAULTTEAMID)) sTeamID = teamNVC.GetKey(0);
				sAlias = teamNVC[sTeamID];

				sRtn += "<tr style=\"background-color:#E0FFFF\" align=\"center\">";
				sRtn += "<th align=\"center\" colspan=\"2\"><font color=\"#808080\">" + sLeague + "&nbsp;-&nbsp;";
				sRtn += "<select name=\"teamID\" onChange=\"changeTeam(PlayersForm.teamID.value,'" + sLeagID + "')\"><option value=\"" + sTeamID + "\">" + sAlias;
				for(iIdx=0;iIdx<iRec;iIdx++) {
					if(!sAlias.Equals(teamNVC[iIdx])) {
						sRtn += "<option value=\"" + teamNVC.GetKey(iIdx) + "\">" + teamNVC[iIdx];
					}
				}
				sRtn += "</select></font></th>";
				sRtn += "<th colspan=\"2\"><font color=\"#808080\">球員名稱</font></th>";
				sRtn += "<td></td><th align=\"right\"><font color=\"#808080\">全選<input type=\"checkbox\" name=\"selectAll\" onClick=\"selectALL()\"></font></th><td></td></tr>";

				sRtn += "<tr style=\"background-color:#E0FFFF\" align=\"center\">";
				sRtn += "<th><font color=\"#808080\">號碼</font></th>";
				sRtn += "<th><font color=\"#808080\">位置</font></th>";
				sRtn += "<th><font color=\"#808080\">中文</font></th>";
				sRtn += "<th><font color=\"#808080\">英文</font></th>";
				sRtn += "<th><font color=\"#808080\">國家</font></th>";
				sRtn += "<th><font color=\"#808080\">出場</font></th>";
				sRtn += "<th><font color=\"#808080\">刪除</font></th></tr>";

				//retrieve player information w.r.t. team id
				iRec = 0;
				retrieveQuery = "select IPLAYER_NO, IPOS, CPLAYER_NAME, CENGNAME, CCOUNTRY, IROSTER from PLAYERS_INFO where TEAM_ID=" + sTeamID + " order by IPLAYER_NO, IPOS, CPLAYER_NAME";
				m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(retrieveQuery);
				while(m_SportsOleReader.Read()) {
					sRtn += "<tr align=\"center\"><td><input type=\"text\" name=\"player_no\" value=\"";
					if(!m_SportsOleReader.IsDBNull(0)) {
						sRtn += m_SportsOleReader.GetInt32(0).ToString();
					}
					sRtn += "\" size=1 maxlength=2 onChange=\"onPosChanged(" + iRec.ToString() + ")\"></td>";

					sRtn += "<td><select name=\"player_pos\">";
					sRtn += "<option value=\"" + m_SportsOleReader.GetInt32(1).ToString() + "\">" + arrPos[m_SportsOleReader.GetInt32(1)];
					for(iIdx=0;iIdx<arrPos.Length;iIdx++) {
						if(m_SportsOleReader.GetInt32(1) != iIdx) {
							sRtn += "<option value=\"" + iIdx.ToString() + "\">" + arrPos[iIdx];
						}
					}
					sRtn += "</td>";

					sRtn += "<td><input type=\"text\" name=\"player_name\" value=\"";
					sRtn += m_SportsOleReader.GetString(2).Trim();
					sRtn += "\" size=6 maxlength=5></td>";
					sRtn += "<td><input type=\"text\" name=\"player_engname\" value=\"";
					if(!m_SportsOleReader.IsDBNull(3)) {
						sRtn += m_SportsOleReader.GetString(3).Trim();
					}
					sRtn += "\" size=20 maxlength=50></td>";

					sRtn += "<td><input type=\"text\" name=\"player_country\" value=\"";
					if(!m_SportsOleReader.IsDBNull(4)) {
						sRtn += m_SportsOleReader.GetString(4).Trim();
					}
					sRtn += "\" size=6 maxlength=5></td>";

					sRtn += "<td><input type=\"checkbox\" name=\"player_roster\" value=\"" + iRec.ToString() + "\" ";
					if(m_SportsOleReader.GetInt32(5) == 1) {
						sRtn += "checked";
					}
					sRtn += "></td><td><input type=\"checkbox\" name=\"player_delete\" value=\"" + m_SportsOleReader.GetString(2).Trim() + "\"></td></tr>";

					iRec++;
				}
				m_SportsOleReader.Close();
				m_SportsDBMgr.Close();

				for(iIdx=iRec;iIdx<TOTALRECORDS;iIdx++) {
					sRtn += "<tr align=\"center\"><td><input type=\"text\" name=\"player_no\" value=\"\" size=1 maxlength=2 onChange=\"onPosChanged(" + iIdx.ToString() + ")\"></td>";
					sRtn += "<td><select name=\"player_pos\">";
					for(int iPosIdx=0;iPosIdx<arrPos.Length;iPosIdx++) {
						sRtn += "<option value=\"" + iPosIdx.ToString() + "\">" + arrPos[iPosIdx];
					}
					sRtn += "</td>";

					sRtn += "<td><input type=\"text\" name=\"player_name\" value=\"\" size=6 maxlength=5></td>";
					sRtn += "<td><input type=\"text\" name=\"player_engname\" value=\"\" size=20 maxlength=50></td>";
					sRtn += "<td><input type=\"text\" name=\"player_country\" value=\"\" size=6 maxlength=5></td>";
					sRtn += "<td><input type=\"checkbox\" name=\"player_roster\" value=\"" + iIdx.ToString() + "\"></td>";
					sRtn += "<td><input type=\"checkbox\" name=\"player_delete\" value=\"-1\"></td></tr>";
				}
				sRtn += "<input type=\"hidden\" name=\"team_id\" value=\"" + sTeamID + "\">";
			}	catch(Exception ex) {
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Players.cs.GetTeamPlayers(): " + ex.ToString());
				m_SportsLog.Close();
				sRtn = (string)HttpContext.Current.Application["accessErrorMsg"];
			}

			return sRtn;
		}

		public int Update(string sType) {
			char[] delimiter = new char[] {','};
			int iRecUpd = 0;
			string sTeamID;
			string playerQuery = "";

			sTeamID = HttpContext.Current.Request.Form["team_id"];
			switch(sType) {
				case "MOD":	//case: modify players information
					int iIdx = 0;
					const string sNULL = "null";
					string[] arrNum, arrPos, arrName, arrEngName, arrCountry, arrRoster;

					arrNum = HttpContext.Current.Request.Form["player_no"].Split(delimiter);
					arrPos = HttpContext.Current.Request.Form["player_pos"].Split(delimiter);
					arrName = HttpContext.Current.Request.Form["player_name"].Split(delimiter);
					arrEngName = HttpContext.Current.Request.Form["player_engname"].Split(delimiter);
					arrCountry = HttpContext.Current.Request.Form["player_country"].Split(delimiter);
					try {	//get player roster
						arrRoster = HttpContext.Current.Request.Form["player_roster"].Split(delimiter);
					}	catch(Exception) {
						arrRoster = new string[0];
					}

					try {
						//reset table reocrds first w.r.t. team id
						playerQuery = "delete from PLAYERS_INFO where TEAM_ID=" + sTeamID;
						m_SportsDBMgr.ExecuteNonQuery(playerQuery);
						m_SportsDBMgr.Close();

						//insert into table
						string sPlayerName = "", sTemp, sRoster = "0";
						for(iIdx=0;iIdx<arrName.Length;iIdx++) {
							sPlayerName = arrName[iIdx].Trim();
							if(!sPlayerName.Equals("")) {
								playerQuery = "insert into PLAYERS_INFO values(" + sTeamID + ",";

								sTemp = arrNum[iIdx];
								if(!sTemp.Equals("")) playerQuery += sTemp;
								else playerQuery += sNULL;
								playerQuery += "," + arrPos[iIdx] + ",'" + sPlayerName + "',";

								sTemp = arrCountry[iIdx];
								if(!sTemp.Equals("")) playerQuery += "'" + sTemp.Trim() + "'";
								else playerQuery += sNULL;
								playerQuery += ",";

								sRoster = "0";
								for(int iRoster=0;iRoster<arrRoster.Length;iRoster++) {
									if(arrRoster[iRoster] == iIdx.ToString()) {
										sRoster = "1";
										break;
									}
								}
								playerQuery += sRoster + ",";

								if(arrEngName[iIdx].Trim().Equals("")) playerQuery += "null";
								else playerQuery += "'" + arrEngName[iIdx].Trim() + "'";
								playerQuery += ")";
								m_SportsDBMgr.ExecuteNonQuery(playerQuery);
								m_SportsDBMgr.Close();
								iRecUpd++;
							}	else {
								break;
							}
						}

						//write log
						m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
						m_SportsLog.SetFileName(0,LOGFILESUFFIX);
						m_SportsLog.Open();
						m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Players.cs: insert " + iRecUpd.ToString() + " players with team ID=" + sTeamID + " (" + HttpContext.Current.Session["user_name"] + ")");
						m_SportsLog.Close();
					}	catch(OleDbException OleDbEx) {
						iRecUpd = -99;
						m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
						m_SportsLog.SetFileName(0,LOGFILESUFFIX);
						m_SportsLog.Open();
						m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Players.cs.Update().MOD: " + OleDbEx.ToString());
						m_SportsLog.Close();
					}	catch(Exception ex) {
						iRecUpd = -1;
						m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
						m_SportsLog.SetFileName(0,LOGFILESUFFIX);
						m_SportsLog.Open();
						m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Players.cs.Update().MOD: " + ex.ToString());
						m_SportsLog.Close();
					}
					break;
				case "DEL":	//case: delete players information w.r.t. team id
					try {
						string[] arrDelete;
						try {	//get player to delete
							arrDelete = HttpContext.Current.Request.Form["player_delete"].Split(delimiter);
						}	catch(Exception) {
							arrDelete = new string[0];
						}
						for(int i = 0; i < arrDelete.Length; i++) {
							if(!arrDelete[i].Equals("")) {
								playerQuery = "delete from PLAYERS_INFO where TEAM_ID=" + sTeamID + " AND CPLAYER_NAME='" + arrDelete[i] + "'";
								m_SportsDBMgr.ExecuteNonQuery(playerQuery);
								m_SportsDBMgr.Close();
								iRecUpd++;

								//write log
								m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
								m_SportsLog.SetFileName(0,LOGFILESUFFIX);
								m_SportsLog.Open();
								m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Players.cs: delete " + arrDelete[i] + " with Team ID: " + sTeamID + " (" + HttpContext.Current.Session["user_name"] + ")");
								m_SportsLog.Close();
							}
						}
					}	catch(Exception ex) {
						iRecUpd = -1;
						m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
						m_SportsLog.SetFileName(0,LOGFILESUFFIX);
						m_SportsLog.Open();
						m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Players.cs.Update().DEL: " + ex.ToString());
						m_SportsLog.Close();
					}
					break;
			}

			return iRecUpd;
		}
	}
}