/*
Objective:
Update players information such as player name, number, position, country etc.

Last updated:
28 Oct 2003 by Chapman

C#.NET complier statement:
csc /t:library /out:..\..\bin\BSKPlayers.dll /r:..\..\bin\DBManager.dll;..\..\bin\Files.dll BSKPlayers.cs
*/

using System;
using System.Data.OleDb;
using System.Reflection;
using System.Web;
using TDL.DB;
using TDL.IO;

[assembly:AssemblyCompany("TDSL")]
[assembly:AssemblyCopyright("(c) 2003 TDSL. All rights reserved. Created on 24 Oct 2003.")]
[assembly:AssemblyDescription("籃球資訊 -> 球員管理")]
[assembly:AssemblyProduct("Sports.NET")]
[assembly:AssemblyTitle("Sports.NET DLL")]
[assembly:AssemblyVersion("1.0.*")]
namespace SportsUtil {
	public class BSKPlayers {
		const int TOTALRECORDS = 15;
		const string LOGFILESUFFIX = "log";
		string m_Team;
		OleDbDataReader m_SportsOleReader;
		DBManager m_SportsDBMgr;
		Files m_SportsLog;

		public BSKPlayers(string Connection) {
			m_SportsDBMgr = new DBManager();
			m_SportsDBMgr.ConnectionString = Connection;
			m_SportsLog = new Files();
			m_Team = "";
		}

		public string Team {
			get {
				return m_Team;
			}
		}

		public string GetTeamPlayers() {
			int iIdx = 0;
			int iPosIdx = 0;
			int iRec = 0;
			string sTeamID;
			string sRtn = "";
			string[] arrPos;

			sTeamID = HttpContext.Current.Request.QueryString["teamID"].Trim();
			arrPos = (string[])HttpContext.Current.Application["BSKPositionArray"];
			try {
				m_Team = m_SportsDBMgr.ExecuteQueryString("select CTEAM from TEAM_INFO where CTEAM_ID='" + sTeamID + "'");
				m_SportsDBMgr.Close();

				//retrieve player information w.r.t. team id
				iRec = 0;
				m_SportsOleReader = m_SportsDBMgr.ExecuteQuery("select IPLAYER_NO, IPOS, CCHI_NAME, CCOUNTRY from PLAYERS_INFO where CTEAM_ID='" + sTeamID + "' order by IPLAYER_NO, IPOS, CCHI_NAME");
				while(m_SportsOleReader.Read()) {
					sRtn += "<tr align=\"center\"><td><input type=\"text\" name=\"player_no\" value=\"";
					if(!m_SportsOleReader.IsDBNull(0)) {
						sRtn += m_SportsOleReader.GetInt32(0).ToString();
					}
					sRtn += "\" size=1 maxlength=2 onChange=\"onPosChanged(" + iRec.ToString() + ")\"></td>";

					sRtn += "<td><select name=\"player_pos\">";
					sRtn += "<option value=\"" + m_SportsOleReader.GetInt32(1).ToString() + "\">" + arrPos[m_SportsOleReader.GetInt32(1)];
					for(iPosIdx = 0; iPosIdx < arrPos.Length; iPosIdx++) {
						if(m_SportsOleReader.GetInt32(1) != iPosIdx) {
							sRtn += "<option value=\"" + iPosIdx.ToString() + "\">" + arrPos[iPosIdx];
						}
					}
					sRtn += "</td>";

					sRtn += "<td><input type=\"text\" name=\"chi_name\" value=\"";
					if(!m_SportsOleReader.IsDBNull(2)) {
						sRtn += m_SportsOleReader.GetString(2).Trim();
					}
					sRtn += "\" size=6 maxlength=5></td>";

					sRtn += "<td><input type=\"text\" name=\"player_country\" value=\"";
					if(!m_SportsOleReader.IsDBNull(3)) {
						sRtn += m_SportsOleReader.GetString(3).Trim();
					}
					sRtn += "\" size=6 maxlength=5></td></tr>";

					iRec++;
				}
				m_SportsOleReader.Close();
				m_SportsDBMgr.Close();
				m_SportsDBMgr.Dispose();

				for(iIdx = iRec; iIdx < TOTALRECORDS; iIdx++) {
					sRtn += "<tr align=\"center\"><td><input type=\"text\" name=\"player_no\" value=\"\" size=1 maxlength=2 onChange=\"onPosChanged(" + iIdx.ToString() + ")\"></td>";
					sRtn += "<td><select name=\"player_pos\">";
					for(iPosIdx = 0; iPosIdx < arrPos.Length; iPosIdx++) {
						sRtn += "<option value=\"" + iPosIdx.ToString() + "\">" + arrPos[iPosIdx];
					}
					sRtn += "</td>";

					sRtn += "<td><input type=\"text\" name=\"chi_name\" value=\"\" size=6 maxlength=5></td>";
					sRtn += "<td><input type=\"text\" name=\"player_country\" value=\"\" size=6 maxlength=5></td></tr>";
				}
				sRtn += "<input type=\"hidden\" name=\"team_id\" value=\"" + sTeamID + "\">";
			}	catch(Exception ex) {
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " BSKPlayers.cs.GetTeamPlayers(): " + ex.ToString());
				m_SportsLog.Close();
				sRtn = (string)HttpContext.Current.Application["accessErrorMsg"];
			}

			return sRtn;
		}

		public int Update(string sType) {
			int iRecUpd = 0;
			string sTeamID;
			string playerQuery = "";

			sTeamID = HttpContext.Current.Request.Form["team_id"];
			switch(sType) {
				case "MOD":	//case: modify players information
					int iIdx = 0;
					char[] delimiter = new char[] {','};
					const string sNULL = "null";
					string[] arrNum;
					string[] arrPos;
					string[] arrChiName;
					string[] arrCountry;

					arrNum = HttpContext.Current.Request.Form["player_no"].Split(delimiter);
					arrPos = HttpContext.Current.Request.Form["player_pos"].Split(delimiter);
					arrChiName = HttpContext.Current.Request.Form["chi_name"].Split(delimiter);
					arrCountry = HttpContext.Current.Request.Form["player_country"].Split(delimiter);

					try {
						//reset table reocrds first w.r.t. team id
						playerQuery = "delete from PLAYERS_INFO where CTEAM_ID='" + sTeamID + "'";
						m_SportsDBMgr.ExecuteNonQuery(playerQuery);
						m_SportsDBMgr.Close();

						//insert into table
						string sPlayerName = "";
						string sTemp;
						for(iIdx = 0; iIdx < arrChiName.Length; iIdx++) {
							sPlayerName = arrChiName[iIdx].Trim();
							if(!sPlayerName.Equals("")) {
								playerQuery = "insert into PLAYERS_INFO values('" + sTeamID + "',";

								sTemp = arrNum[iIdx];
								if(!sTemp.Equals("")) playerQuery += sTemp;
								else playerQuery += sNULL;
								playerQuery += "," + arrPos[iIdx] + ",'" + sPlayerName + "',";

								sTemp = arrCountry[iIdx];
								if(!sTemp.Equals("")) playerQuery += "'" + sTemp.Trim() + "'";
								else playerQuery += sNULL;
								playerQuery += ",0)";
								m_SportsDBMgr.ExecuteNonQuery(playerQuery);
								m_SportsDBMgr.Close();
								iRecUpd++;
							}	else {
								break;
							}
						}
						m_SportsDBMgr.Dispose();

						//write log
						m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
						m_SportsLog.SetFileName(0,LOGFILESUFFIX);
						m_SportsLog.Open();
						m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " BSKPlayers.cs: insert " + iRecUpd.ToString() + " players with team ID=" + sTeamID + " (" + HttpContext.Current.Session["user_name"] + ")");
						m_SportsLog.Close();
					}	catch(OleDbException OleDbEx) {
						iRecUpd = -99;
						m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
						m_SportsLog.SetFileName(0,LOGFILESUFFIX);
						m_SportsLog.Open();
						m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " BSKPlayers.cs.Update().MOD: " + OleDbEx.ToString());
						m_SportsLog.Close();
					}	catch(Exception ex) {
						iRecUpd = -1;
						m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
						m_SportsLog.SetFileName(0,LOGFILESUFFIX);
						m_SportsLog.Open();
						m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " BSKPlayers.cs.Update().MOD: " + ex.ToString());
						m_SportsLog.Close();
					}
					break;
				case "DEL":	//case: delete players information w.r.t. team id
					try {
						playerQuery = "delete from PLAYERS_INFO where CTEAM_ID='" + sTeamID + "'";
						iRecUpd = m_SportsDBMgr.ExecuteNonQuery(playerQuery);
						m_SportsDBMgr.Close();
						m_SportsDBMgr.Dispose();

						//write log
						m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
						m_SportsLog.SetFileName(0,LOGFILESUFFIX);
						m_SportsLog.Open();
						m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " BSKPlayers.cs: delete " + iRecUpd.ToString() + " players with team ID=" + sTeamID + " (" + HttpContext.Current.Session["user_name"] + ")");
						m_SportsLog.Close();
					}	catch(Exception ex) {
						iRecUpd = -1;
						m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
						m_SportsLog.SetFileName(0,LOGFILESUFFIX);
						m_SportsLog.Open();
						m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " BSKPlayers.cs.Update().DEL: " + ex.ToString());
						m_SportsLog.Close();
					}
					break;
			}

			return iRecUpd;
		}
	}
}