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
using System.Configuration;
using System.Data.OleDb;
using System.Reflection;
using System.Web;
using TDL.DB;
using TDL.IO;
using FirebirdSql.Data.FirebirdClient;
using JC_SoccerWeb.Common;

namespace SportsUtil {
	public class Players {
		const int TOTALRECORDS = 35;
		const string LOGFILESUFFIX = "log";
		const string DEFAULTTEAMID = "000";
		//OleDbDataReader m_SportsOleReader;
        FbDataReader m_SportsOleReaderFb;
        // DBManager m_SportsDBMgr; 
        TDL.IO.Files m_SportsLog;
        public string m_Title = "";
        DBManagerFB m_SportsDBMgrFb;
        public Players(string Connection) {
			//m_SportsDBMgr = new DBManager();
			//m_SportsDBMgr.ConnectionString = Connection;
			m_SportsLog = new TDL.IO.Files();
            m_SportsDBMgrFb = new DBManagerFB();
            m_SportsDBMgrFb.ConnectionString = JC_SoccerWeb.Common.AppFlag.ScoutsDBConn;
        }

		public int NumberOfRecords {
			get {
				return TOTALRECORDS;
			}
		}
        public string GetTeamPlayers()
        {
            string log = "log";
            int iIdx = 0, iRec = 0;
            string retrieveQuery,sEventID, sLeagID="-1", sTeamID="", sLeague = "", sAlias = "",sEventName="", sRtn = "";
            string[] arrPos;
            NameValueCollection teamNVC = new NameValueCollection();

            //sTeamID = HttpContext.Current.Request.QueryString["teamID"].Trim();
            //sLeagID = HttpContext.Current.Request.QueryString["leagID"].Trim();
            sTeamID = (HttpContext.Current.Request.QueryString["teamID"] == null) ? "000" : HttpContext.Current.Request.QueryString["teamID"].Trim();
            //sLeagID = (HttpContext.Current.Request.QueryString["leagID"] == null) ? "001" : HttpContext.Current.Request.QueryString["leagID"].Trim();
            sEventID = (HttpContext.Current.Request.QueryString["eventid"] == null) ? "" : HttpContext.Current.Request.QueryString["eventid"].Trim();

            arrPos = (string[])HttpContext.Current.Application["positionItemsArray"];
              log += ":";
            try
            {
                //retrieve teams w.r.t. leag id
                // retrieveQuery = "select team.team_id, team.teamname, leag.leagname from teaminfo team, leaginfo leag, id_info id ";
                ///retrieveQuery = "select team.HKJC_ID, team.HKJC_NAME_CN, m.CLEAGUE_OUTPUT_NAME,m.HKJCHOSTNAME_CN||'/'||m.HKJCGUESTNAME_CN,m.CLEAGUEALIAS_OUTPUT_NAME, team.HKJC_NAME,  e.id  from teams team ";
               retrieveQuery = "select distinct team.id, team.HKJC_NAME_CN, m.CLEAGUE_OUTPUT_NAME,m.HKJCHOSTNAME_CN||'/'||m.HKJCGUESTNAME_CN,m.CLEAGUEALIAS_OUTPUT_NAME, team.HKJC_NAME,  e.id  from teams team ";
                retrieveQuery += "inner join events e on e.HOME_ID = team.id or e.GUEST_ID = team.id inner join EMATCHES m on e.id=m.EMATCHID and      e.START_DATE  -1  < m.CMATCHDATETIME and m.CMATCHDATETIME <  e.START_DATE  +1 " +//  and e.START_DATE =m.CMATCHDATETIME  " +
                    " where e.id = '"+ sEventID + "'";
               // JC_SoccerWeb.Common.Files.CicsWriteLog(DateTime.Now.ToString("HH:mm:ss ") + "Sql: " + retrieveQuery);

                m_SportsOleReaderFb = m_SportsDBMgrFb.ExecuteQuery(retrieveQuery);
                while (m_SportsOleReaderFb.Read())
                {
                    if (!m_SportsOleReaderFb.IsDBNull(0))
                    {
                        teamNVC.Add(m_SportsOleReaderFb.GetInt32(0).ToString(), m_SportsOleReaderFb.GetString(1).Trim());
                        iRec++;
                    }
                    if (sLeague.Equals("")) sLeague = m_SportsOleReaderFb.GetString(2).Trim();
                    if (sEventName.Equals(""))  sEventName = m_SportsOleReaderFb.GetString(3).Trim();
                }
                m_SportsOleReaderFb.Close();
                m_SportsDBMgrFb.Close();
                log +=" sql 1 / ";
                //assign team id if it is default value
                if (sTeamID.Equals(DEFAULTTEAMID)) sTeamID = teamNVC.GetKey(0);
                sAlias = teamNVC[sTeamID];
                m_Title = sLeague+" "+ sEventName +" ["+ sAlias + "]-陣容";

                sRtn += "<tr style=\"background-color:#E0FFFF\" align=\"center\">";
                sRtn += "<th align=\"center\" colspan=\"2\"><font color=\"#808080\">" + sLeague + "&nbsp;-&nbsp;"+ sEventName+ "&nbsp;-&nbsp;";
                sRtn += "<select name=\"teamID\" onChange=\"changeTeam(PlayersForm.teamID.value,'" + sEventID + "')\"><option value=\"" + sTeamID + "\">" + sAlias;
                for (iIdx = 0; iIdx < iRec; iIdx++)
                {
                    if (!sAlias.Equals(teamNVC[iIdx]))
                    {
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
                log += " f2  / ";
                //retrieve player information w.r.t. team id
                iRec = 0;
                //retrieveQuery = "select IPLAYER_NO, IPOS, CPLAYER_NAME, CENGNAME, CCOUNTRY, IROSTER,PLAYER_ID from PLAYERS_INFO where TEAM_ID=" + sTeamID + " order by IPLAYER_NO, IPOS, CPLAYER_NAME";
                // retrieveQuery = "select IPLAYER_NO, IPOS, CPLAYER_NAME, CENGNAME, CCOUNTRY, IROSTER,PLAYER_ID from PLAYERS_INFO where TEAM_ID=" + sTeamID + " order by  IROSTER desc,IPOS asc, CPLAYER_NAME desc   ";

                // retrieveQuery = "select x.IPLAYER_NO,  x.IPOS,  x.CPLAYER_NAME,  x.CENGNAME,  x.CCOUNTRY, x.IROSTER, x.PLAYER_ID from PLAYERS_INFO x " +
                ////   "inner join  PLAYERS p on p.TEAM_ID =x.TEAM_ID  and p.id=PLAYER_ID " 
                //    " inner join events e on e.SEASON_ID = X.SEASON_ID " +
                //   "where  x.TEAM_ID=" + sTeamID + " and e.id=" + sEventID + " order by   x.IROSTER desc, x.IPOS asc,  x.CPLAYER_NAME desc   ";

                //retrieveQuery = "select distinct x.IPLAYER_NO,  x.IPOS,  x.CPLAYER_NAME,  x.CENGNAME,  x.CCOUNTRY, x.IROSTER, x.PLAYER_ID from PLAYERS_INFO x " +
                //    "inner join  PLAYERS p on p.TEAM_ID =x.TEAM_ID  and p.id=PLAYER_ID " + " inner join events e on e.SEASON_ID = p.SEASON_ID " +
                //    "where  x.TEAM_ID=" + sTeamID + " and e.id=" + sEventID + " order by   x.IROSTER desc, x.IPOS asc,  x.CPLAYER_NAME desc   ";

                retrieveQuery = "select   x.IPLAYER_NO,  x.IPOS,  x.CPLAYER_NAME,  x.CENGNAME,  x.CCOUNTRY, x.IROSTER, x.PLAYER_ID from PLAYERS_INFO x " +
                    " where  x.TEAM_ID=" + sTeamID + " and EVENT_ID=" + sEventID + " order by   x.IROSTER desc, x.IPOS asc,  x.CPLAYER_NAME desc   ";
                m_SportsOleReaderFb = m_SportsDBMgrFb.ExecuteQuery(retrieveQuery);
                while (m_SportsOleReaderFb.Read())
                {
                    sRtn += "<tr align=\"center\"><td><input type=\"text\" name=\"player_no\" value=\"";
                    if (!m_SportsOleReaderFb.IsDBNull(0))
                    {
                        sRtn += m_SportsOleReaderFb.GetInt32(0).ToString();
                    }
                    sRtn += "\" size=1 maxlength=2 onChange=\"onPosChanged(" + iRec.ToString() + ")\"></td>";

                    sRtn += "<td><select name=\"player_pos\">";
                    sRtn += "<option value=\"" + m_SportsOleReaderFb.GetInt32(1).ToString() + "\">" + arrPos[m_SportsOleReaderFb.GetInt32(1)];
                  ///  sRtn += (m_SportsOleReaderFb.GetInt32(0) == -1) ? "<option value=\"" + "5" + "\">" + arrPos[5] : "<option value=\"" + m_SportsOleReaderFb.GetInt32(1).ToString() + "\">" + arrPos[m_SportsOleReaderFb.GetInt32(1)];
                    for (iIdx = 0; iIdx < arrPos.Length; iIdx++)
                    {
                        if (m_SportsOleReaderFb.GetInt32(1) != iIdx)
                        {
                            sRtn += "<option value=\"" + iIdx.ToString() + "\">" + arrPos[iIdx];
                        }
                    }
                    sRtn += "</td>";

                    sRtn += "<td><input type=\"text\" name=\"player_name\" value=\"";
                    sRtn += m_SportsOleReaderFb.GetString(2).Trim();
                    sRtn += "\" size=6 maxlength=5></td>";
                    sRtn += "<td><input type=\"text\" name=\"player_engname\" value=\"";
                    if (!m_SportsOleReaderFb.IsDBNull(3))
                    {
                        sRtn += m_SportsOleReaderFb.GetString(3).Trim();
                    }
                    sRtn += "\" size=20 maxlength=50 readonly=\"readonly\" ></td>";

                    sRtn += "<td><input type=\"text\" name=\"player_country\" value=\"";
                    if (!m_SportsOleReaderFb.IsDBNull(4))
                    {
                        sRtn += m_SportsOleReaderFb.GetString(4).Trim();
                    }
                    sRtn += "\" size=6 maxlength=5></td>";

                    sRtn += "<td><input type=\"checkbox\" name=\"player_roster\" value=\"" + iRec.ToString() + "\" ";
                    if (m_SportsOleReaderFb.GetInt32(5) == 1)
                    {
                        sRtn += "checked";
                    }
                    sRtn += "></td><td><input type=\"checkbox\" name=\"player_delete\" value=\"" + m_SportsOleReaderFb.GetString(2).Trim() + "\"></td>";
                    sRtn += "<td  style=\"display:none\"><input type=\"text\" name=\"player_id\" value=\"";
                    sRtn += m_SportsOleReaderFb.GetString(6).Trim();
                    sRtn += "\" size=6 maxlength=5 ></td></tr>";
                    iRec++;
                }
                m_SportsOleReaderFb.Close();
                m_SportsDBMgrFb.Close();
                log += " f3  / ";
                for (iIdx = iRec; iIdx < TOTALRECORDS; iIdx++)
                {
                    sRtn += "<tr align=\"center\"><td><input type=\"text\" name=\"player_no\" value=\"\" size=1 maxlength=2 onChange=\"onPosChanged(" + iIdx.ToString() + ")\"></td>";
                    sRtn += "<td><select name=\"player_pos\">";
                    for (int iPosIdx = 0; iPosIdx < arrPos.Length; iPosIdx++)
                    {
                        sRtn += "<option value=\"" + iPosIdx.ToString() + "\">" + arrPos[iPosIdx];
                    }
                    sRtn += "</td>";

                    sRtn += "<td><input type=\"text\" name=\"player_name\" value=\"\" size=6 maxlength=5></td>";
                    sRtn += "<td><input type=\"text\" name=\"player_engname\" value=\"\" size=20 maxlength=50 ></td>";
                    sRtn += "<td><input type=\"text\" name=\"player_country\" value=\"\" size=6 maxlength=5></td>";
                    sRtn += "<td><input type=\"checkbox\" name=\"player_roster\" value=\"" + iIdx.ToString() + "\"></td>";
                    sRtn += "<td><input type=\"checkbox\" name=\"player_delete\" value=\"-1\"></td>";
                    sRtn += "<td  style=\"display:none\"><input type=\"text\" name=\"player_id\" value=\"0\" size=6 maxlength=5></td></tr>";
                }
                sRtn += "<input type=\"hidden\" name=\"team_id\" value=\"" + sTeamID + "\">";
                log += " e3  / ";
            }
            catch (Exception ex)
            {
                m_SportsLog.FilePath = ConfigurationManager.AppSettings["errlog"];
                m_SportsLog.SetFileName(0, LOGFILESUFFIX);
                m_SportsLog.Open();
                m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Players.cs.GetTeamPlayers(): " + ex.ToString()+"\r\n "+ log);
                m_SportsLog.Close();
                sRtn = ConfigurationManager.AppSettings["accessErrorMsg"];
            }

            return sRtn;
        }
        //public string GetTeamPlayers2()
        //{
        //    int iIdx = 0, iRec = 0;
        //    string retrieveQuery, sLeagID, sTeamID, sLeague = "", sAlias = "", sRtn = "";
        //    string[] arrPos;
        //    NameValueCollection teamNVC = new NameValueCollection();

        //    //sTeamID = HttpContext.Current.Request.QueryString["teamID"].Trim();
        //    //sLeagID = HttpContext.Current.Request.QueryString["leagID"].Trim();
        //    sTeamID = (HttpContext.Current.Request.QueryString["teamID"] == null) ? "000" : HttpContext.Current.Request.QueryString["teamID"].Trim();
        //    sLeagID = (HttpContext.Current.Request.QueryString["leagID"] == null) ? "001" : HttpContext.Current.Request.QueryString["leagID"].Trim();
        //    arrPos = (string[])HttpContext.Current.Application["positionItemsArray"];

        //    try
        //    {
        //        //retrieve teams w.r.t. leag id
        //        retrieveQuery = "select team.team_id, team.teamname, leag.leagname from teaminfo team, leaginfo leag, id_info id ";
        //        retrieveQuery += "where leag.leag_id='" + sLeagID + "' and leag.leag_id=id.leag_id ";
        //        retrieveQuery += "and team.team_id=id.team_id order by team.team_id";
        //        m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(retrieveQuery);
        //        while (m_SportsOleReader.Read())
        //        {
        //            if (!m_SportsOleReader.IsDBNull(0))
        //            {
        //                teamNVC.Add(m_SportsOleReader.GetInt32(0).ToString(), m_SportsOleReader.GetString(1).Trim());
        //                iRec++;
        //            }
        //            if (sLeague.Equals("")) sLeague = m_SportsOleReader.GetString(2).Trim();
        //        }
        //        m_SportsOleReader.Close();
        //        m_SportsDBMgr.Close();

        //        //assign team id if it is default value
        //        if (sTeamID.Equals(DEFAULTTEAMID)) sTeamID = teamNVC.GetKey(0);
        //        sAlias = teamNVC[sTeamID];

        //        sRtn += "<tr style=\"background-color:#E0FFFF\" align=\"center\">";
        //        sRtn += "<th align=\"center\" colspan=\"2\"><font color=\"#808080\">" + sLeague + "&nbsp;-&nbsp;";
        //        sRtn += "<select name=\"teamID\" onChange=\"changeTeam(PlayersForm.teamID.value,'" + sLeagID + "')\"><option value=\"" + sTeamID + "\">" + sAlias;
        //        for (iIdx = 0; iIdx < iRec; iIdx++)
        //        {
        //            if (!sAlias.Equals(teamNVC[iIdx]))
        //            {
        //                sRtn += "<option value=\"" + teamNVC.GetKey(iIdx) + "\">" + teamNVC[iIdx];
        //            }
        //        }
        //        sRtn += "</select></font></th>";
        //        sRtn += "<th colspan=\"2\"><font color=\"#808080\">球員名稱</font></th>";
        //        sRtn += "<td></td><th align=\"right\"><font color=\"#808080\">全選<input type=\"checkbox\" name=\"selectAll\" onClick=\"selectALL()\"></font></th><td></td></tr>";

        //        sRtn += "<tr style=\"background-color:#E0FFFF\" align=\"center\">";
        //        sRtn += "<th><font color=\"#808080\">號碼</font></th>";
        //        sRtn += "<th><font color=\"#808080\">位置</font></th>";
        //        sRtn += "<th><font color=\"#808080\">中文</font></th>";
        //        sRtn += "<th><font color=\"#808080\">英文</font></th>";
        //        sRtn += "<th><font color=\"#808080\">國家</font></th>";
        //        sRtn += "<th><font color=\"#808080\">出場</font></th>";
        //        sRtn += "<th><font color=\"#808080\">刪除</font></th></tr>";

        //        //retrieve player information w.r.t. team id
        //        iRec = 0;
        //        retrieveQuery = "select IPLAYER_NO, IPOS, CPLAYER_NAME, CENGNAME, CCOUNTRY, IROSTER from PLAYERS_INFO where TEAM_ID=" + sTeamID + " order by IPLAYER_NO, IPOS, CPLAYER_NAME";
        //        m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(retrieveQuery);
        //        while (m_SportsOleReader.Read())
        //        {
        //            sRtn += "<tr align=\"center\"><td><input type=\"text\" name=\"player_no\" value=\"";
        //            if (!m_SportsOleReader.IsDBNull(0))
        //            {
        //                sRtn += m_SportsOleReader.GetInt32(0).ToString();
        //            }
        //            sRtn += "\" size=1 maxlength=2 onChange=\"onPosChanged(" + iRec.ToString() + ")\"></td>";

        //            sRtn += "<td><select name=\"player_pos\">";
        //            sRtn += "<option value=\"" + m_SportsOleReader.GetInt32(1).ToString() + "\">" + arrPos[m_SportsOleReader.GetInt32(1)];
        //            for (iIdx = 0; iIdx < arrPos.Length; iIdx++)
        //            {
        //                if (m_SportsOleReader.GetInt32(1) != iIdx)
        //                {
        //                    sRtn += "<option value=\"" + iIdx.ToString() + "\">" + arrPos[iIdx];
        //                }
        //            }
        //            sRtn += "</td>";

        //            sRtn += "<td><input type=\"text\" name=\"player_name\" value=\"";
        //            sRtn += m_SportsOleReader.GetString(2).Trim();
        //            sRtn += "\" size=6 maxlength=5></td>";
        //            sRtn += "<td><input type=\"text\" name=\"player_engname\" value=\"";
        //            if (!m_SportsOleReader.IsDBNull(3))
        //            {
        //                sRtn += m_SportsOleReader.GetString(3).Trim();
        //            }
        //            sRtn += "\" size=20 maxlength=50></td>";

        //            sRtn += "<td><input type=\"text\" name=\"player_country\" value=\"";
        //            if (!m_SportsOleReader.IsDBNull(4))
        //            {
        //                sRtn += m_SportsOleReader.GetString(4).Trim();
        //            }
        //            sRtn += "\" size=6 maxlength=5></td>";

        //            sRtn += "<td><input type=\"checkbox\" name=\"player_roster\" value=\"" + iRec.ToString() + "\" ";
        //            if (m_SportsOleReader.GetInt32(5) == 1)
        //            {
        //                sRtn += "checked";
        //            }
        //            sRtn += "></td><td><input type=\"checkbox\" name=\"player_delete\" value=\"" + m_SportsOleReader.GetString(2).Trim() + "\"></td></tr>";

        //            iRec++;
        //        }
        //        m_SportsOleReader.Close();
        //        m_SportsDBMgr.Close();

        //        for (iIdx = iRec; iIdx < TOTALRECORDS; iIdx++)
        //        {
        //            sRtn += "<tr align=\"center\"><td><input type=\"text\" name=\"player_no\" value=\"\" size=1 maxlength=2 onChange=\"onPosChanged(" + iIdx.ToString() + ")\"></td>";
        //            sRtn += "<td><select name=\"player_pos\">";
        //            for (int iPosIdx = 0; iPosIdx < arrPos.Length; iPosIdx++)
        //            {
        //                sRtn += "<option value=\"" + iPosIdx.ToString() + "\">" + arrPos[iPosIdx];
        //            }
        //            sRtn += "</td>";

        //            sRtn += "<td><input type=\"text\" name=\"player_name\" value=\"\" size=6 maxlength=5></td>";
        //            sRtn += "<td><input type=\"text\" name=\"player_engname\" value=\"\" size=20 maxlength=50></td>";
        //            sRtn += "<td><input type=\"text\" name=\"player_country\" value=\"\" size=6 maxlength=5></td>";
        //            sRtn += "<td><input type=\"checkbox\" name=\"player_roster\" value=\"" + iIdx.ToString() + "\"></td>";
        //            sRtn += "<td><input type=\"checkbox\" name=\"player_delete\" value=\"-1\"></td></tr>";
        //        }
        //        sRtn += "<input type=\"hidden\" name=\"team_id\" value=\"" + sTeamID + "\">";
        //    }
        //    catch (Exception ex)
        //    {
        //        m_SportsLog.FilePath = ConfigurationManager.AppSettings["errlog"];
        //        m_SportsLog.SetFileName(0, LOGFILESUFFIX);
        //        m_SportsLog.Open();
        //        m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Players.cs.GetTeamPlayers(): " + ex.ToString());
        //        m_SportsLog.Close();
        //        sRtn = ConfigurationManager.AppSettings["accessErrorMsg"];
        //    }

        //    return sRtn;
        //}

        public int Update(string sType) {
			char[] delimiter = new char[] {','};
			int iRecUpd = 0;
			string sTeamID, sMatchCount;
			string playerQuery = "";

            sMatchCount = HttpContext.Current.Request.QueryString["eventid"];
            sTeamID = HttpContext.Current.Request.Form["team_id"];
			switch(sType) {
				case "MOD":	//case: modify players information
					int iIdx = 0;
					const string sNULL = "null";
					string[] arrNum, arrPos, arrName, arrEngName, arrCountry, arrPlay_ID, arrRoster;

					arrNum = HttpContext.Current.Request.Form["player_no"].Split(delimiter);
					arrPos = HttpContext.Current.Request.Form["player_pos"].Split(delimiter);
					arrName = HttpContext.Current.Request.Form["player_name"].Split(delimiter);
					arrEngName = HttpContext.Current.Request.Form["player_engname"].Split(delimiter);
					arrCountry = HttpContext.Current.Request.Form["player_country"].Split(delimiter);
                    arrPlay_ID = HttpContext.Current.Request.Form["player_id"].Split(delimiter);
                    try {	//get player roster
						arrRoster = HttpContext.Current.Request.Form["player_roster"].Split(delimiter);
					}	catch(Exception) {
						arrRoster = new string[0];
					}

					try {
						//////reset table reocrds first w.r.t. team id
						////playerQuery = "delete from PLAYERS_INFO where TEAM_ID=" + sTeamID;
						////m_SportsDBMgrFb.ExecuteNonQuery(playerQuery);
      ////                  m_SportsDBMgrFb.Close();

						//insert into table
						string sPlayerName = "",sPlayID="-1", sTemp, sRoster = "0";
                        for (iIdx = 0; iIdx < arrName.Length; iIdx++)
                        {
                            sPlayerName = arrName[iIdx].Trim();
                            sPlayID = arrPlay_ID[iIdx]; 
                            if (!sPlayerName.Equals(""))
                            {
                                if (sPlayID == "0")
                                {
                                  // playerQuery = "insert into PLAYERS_INFO values(" + sTeamID + "," + new Random().Next(100000,1000000) +",";
                                    playerQuery = "insert into PLAYERS_INFO values(" + sTeamID + "," + "-1" + ",";
                                    sTemp = arrNum[iIdx];
                                    if (!sTemp.Equals("")) playerQuery += sTemp;
                                    else playerQuery += sNULL;
                                    playerQuery += "," + arrPos[iIdx] + ",'" + sPlayerName + "',";

                                    sTemp = arrCountry[iIdx];
                                    if (!sTemp.Equals("")) playerQuery += "'" + sTemp.Trim() + "'";
                                    else playerQuery += sNULL;
                                    playerQuery += ",";

                                    sRoster = "0";
                                    for (int iRoster = 0; iRoster < arrRoster.Length; iRoster++)
                                    {
                                        if (arrRoster[iRoster] == iIdx.ToString())
                                        {
                                            sRoster = "1";
                                            break;
                                        }
                                    }
                                    playerQuery += sRoster + ",";

                                    if (arrEngName[iIdx].Trim().Equals("")) playerQuery += "null";
                                    else playerQuery += "'" + arrEngName[iIdx].Trim().Replace("'", "''") + "'";
                                    playerQuery += ",'";
                                    playerQuery += DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff");
                                    playerQuery += "'";
                                    playerQuery += ")";
                                }
                                else
                                {
                                    playerQuery = "update  PLAYERS_INFO set IPLAYER_NO=";
                                    sTemp = arrNum[iIdx];
                                    if (!sTemp.Equals("")) playerQuery += sTemp;
                                    else playerQuery += sNULL;
                                    playerQuery += ", IPOS=" + arrPos[iIdx] + ", CPLAYER_NAME='" + sPlayerName + "', CCOUNTRY=";

                                    sTemp = arrCountry[iIdx];
                                    if (!sTemp.Equals("")) playerQuery += "'" + sTemp.Trim() + "'";
                                    else playerQuery += sNULL;
                                    playerQuery += ", IROSTER=";

                                    sRoster = "0";
                                    for (int iRoster = 0; iRoster < arrRoster.Length; iRoster++)
                                    {
                                        if (arrRoster[iRoster] == iIdx.ToString())
                                        {
                                            sRoster = "1";
                                            break;
                                        }
                                    }
                                    playerQuery += sRoster + ", CTIMESTAMP='";

                                    playerQuery += DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff");
                                    playerQuery += "'";

                                    playerQuery += " where TEAM_ID=" + sTeamID + " and  PLAYER_ID=" + sPlayID + "" +( sPlayID=="-1"? " and CENGNAME='"+ arrEngName[iIdx].Replace("'","''") + "'" : "");
                                }
                                m_SportsDBMgrFb.ExecuteNonQuery(playerQuery);
                                m_SportsDBMgrFb.Close();
                                iRecUpd++;
                            }
                            else
                            {
                              //  break;
                            }
                        }

                        JC_SoccerWeb.Common.Files.CicsWriteLog(DateTime.Now.ToString("HH:mm:ss ") + "Sql: " + playerQuery);
                        if (sMatchCount != "")
                        {
                            JC_SoccerWeb.Common.Files.CicsWriteLog(DateTime.Now.ToString("HH:mm:ss ") + "Send " + sMatchCount + " ANALYSISPLAYERLIST/12 ");
                            string sReslut = ConfigManager.SendWinMsg(sMatchCount, "ANALYSISPLAYERLIST/12");
                            //  if (sReslut != "Done") JC_SoccerWeb.Common.Files.CicsWriteLog(DateTime.Now.ToString("HH:mm:ss ")+ "[Failure] Send " + sMatchCount + " ANALYSISBGREMARK-10, " + sReslut);
                        }

                        //write log
                        m_SportsLog.FilePath = ConfigurationManager.AppSettings["eventlog"];
						m_SportsLog.SetFileName(0,LOGFILESUFFIX);
						m_SportsLog.Open();
						m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Players.cs: insert " + iRecUpd.ToString() + " players with team ID=" + sTeamID + " (" + HttpContext.Current.Session["user_name"] + ")");
						m_SportsLog.Close();
					}	catch(OleDbException OleDbEx) {
						iRecUpd = -99;
						m_SportsLog.FilePath = ConfigurationManager.AppSettings["errlog"];
						m_SportsLog.SetFileName(0,LOGFILESUFFIX);
						m_SportsLog.Open();
						m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Players.cs.Update().MOD: " + OleDbEx.ToString());
						m_SportsLog.Close();
					}	catch(Exception ex) {
						iRecUpd = -1;
						m_SportsLog.FilePath = ConfigurationManager.AppSettings["errlog"];
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
                                m_SportsDBMgrFb.ExecuteNonQuery(playerQuery);
                                m_SportsDBMgrFb.Close();
								iRecUpd++;

								//write log
								m_SportsLog.FilePath = ConfigurationManager.AppSettings["eventlog"];
								m_SportsLog.SetFileName(0,LOGFILESUFFIX);
								m_SportsLog.Open();
								m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Players.cs: delete " + arrDelete[i] + " with Team ID: " + sTeamID + " (" + HttpContext.Current.Session["user_name"] + ")");
								m_SportsLog.Close();
							}
						}
					}	catch(Exception ex) {
						iRecUpd = -1;
						m_SportsLog.FilePath = ConfigurationManager.AppSettings["errlog"];
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