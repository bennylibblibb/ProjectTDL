/*
Objective:
Retrieval and modify matches analysis information

Last updated:
12 July 2004, Chapman Choi

C#.NET complier statement:
csc /t:library /out:..\bin\AnalysisModify.dll /r:..\bin\DBManager.dll;..\bin\Files.dll AnalysisModify.cs
*/

using System;
using System.Collections;
using System.Configuration;
using System.Data.OleDb;
using System.Reflection;
using System.Text;
using System.Web;
using TDL.DB;
using TDL.IO;
using FirebirdSql.Data.FirebirdClient;
using JC_SoccerWeb.Common;

namespace SportsUtil
{
    public class AnalysisModify
    {
        const string LOGFILESUFFIX = "log";
        const string DBCR = "(CR)";
        const string PAGECR = "\r\n";
        const int HISTORYCOUNT = 4;
        //OleDbDataReader m_SportsOleReader;
        //DBManager m_SportsDBMgr;
        TDL.IO.Files m_SportsLog;
        StringBuilder SQLString;
        public string m_Title = "";
        DBManagerFB m_SportsDBMgrFb;
        FbDataReader m_SportsOleReaderFb;

        public AnalysisModify(string Connection)
        {
            //m_SportsDBMgr = new DBManager();
            //m_SportsDBMgr.ConnectionString = Connection;
            m_SportsLog = new TDL.IO.Files();
            SQLString = new StringBuilder();
            m_SportsDBMgrFb = new DBManagerFB();
            m_SportsDBMgrFb.ConnectionString = JC_SoccerWeb.Common.AppFlag.ScoutsDBConn;
        }

        struct MatchHistory
        {
            public string sAlias;
            public int iMonth;
            public int iYear;
            public int iStatus;
            public int iHostScore;
            public int iGuestScore;
            public int iEventid;
        }
        public string GetMatches()
        {
            int iIdx = 0;
            //// string sMatchCount ,sEventID;
            string sEventID;
            string[] arrWeather, arrFields;
            StringBuilder HTMLString = new StringBuilder();
            //int xx = 1;
            //for (int xx = 1; xx < 100; xx++)
            //{
            sEventID = (HttpContext.Current.Request.QueryString["eventid"] == null) ? "" : HttpContext.Current.Request.QueryString["eventid"].Trim();

            ////  sMatchCount = HttpContext.Current.Request.QueryString["matchcnt"] == null ? xx.ToString() : HttpContext.Current.Request.QueryString["matchcnt"].Trim();
            arrWeather = (string[])HttpContext.Current.Application["weatherItemsArray"];
            arrFields = (string[])HttpContext.Current.Application["fieldItemsArray"];

            try
            {
                int iExisted;
                const string sEmpty = "未輸入";
                string sLeague = "", sHost = "", sHostID = "", sGuest = "", sGuestID = "", sMatchDate = "", sMatchTime = "", sLeagType = "", sVenue = "", sLeagID = "", sEventArea = "";
                string sH_City = sEmpty, sH_Country = sEmpty, sH_Continent = sEmpty, sG_City = sEmpty, sG_Country = sEmpty, sG_Continent = sEmpty;

                SQLString.Remove(0, SQLString.Length);
                SQLString.Append("select count(IMATCH_CNT) from ANALYSIS_BG_INFO where CACT='U' and IMATCH_CNT=");
                ////SQLString.Append(sMatchCount);
                SQLString.Append(sEventID);
                iExisted = m_SportsDBMgrFb.ExecuteScalar(SQLString.ToString());
                m_SportsDBMgrFb.Close();

                //retrieve information related to host and guest
                //SQLString.Remove(0, SQLString.Length);
                //SQLString.Append("select game.league, game.host, (select team_id from teaminfo where teamname=game.host), game.guest, (select team_id from teaminfo where teamname=game.guest), game.matchdate, game.matchtime, leag.LEAGUETYPE, ");
                //SQLString.Append("(select venue from teaminfo where teamname=game.host), (select city from teaminfo where teamname=game.host), (select country from teaminfo where teamname=game.host), (select continent from teaminfo where teamname=game.host), ");
                //SQLString.Append("(select city from teaminfo where teamname=game.guest), (select country from teaminfo where teamname=game.guest), (select continent from teaminfo where teamname=game.guest), leag.leag_id ");
                //SQLString.Append("from gameinfo game, leaginfo leag where game.league=leag.alias and game.match_cnt=");
                //SQLString.Append(sMatchCount);
                SQLString.Remove(0, SQLString.Length);
                SQLString.Append("select r.CLEAGUE_OUTPUT_NAME,   r.HKJCHOSTNAME_CN,r.HKJCHOSTID, r.HKJCGUESTNAME_CN, r.HKJCGUESTID,");
                SQLString.Append("cast(cast(r.CMATCHDATETIME as date) as varchar(10)), cast(r.CMATCHDATETIME as time), '' sLeagTyp , '' sVenue  ,");
                SQLString.Append("(select a.NAME from TEAMS t inner join  AREAS a on t.AREA_ID=a.id  where t.ID=E.HOME_ID) ,");
                SQLString.Append("(select a.name from TEAMS t inner join  AREAS a on t.AREA_ID=a.id  where t.ID=E.HOME_ID) , ");
                SQLString.Append("(select b2.NAME from AREAS B inner join AREAS b2 on b2.id=b.PARENT_AREA_ID where B.ID =(select AREA_ID from TEAMS where ID=e.HOME_ID)),");
                SQLString.Append(" (select  a.NAME from TEAMS t inner join  AREAS a on t.AREA_ID=a.id  where t.ID=E.GUEST_ID) , ");
                SQLString.Append("(select  a.NAME from TEAMS t inner join  AREAS a on t.AREA_ID=a.id  where t.ID=E.GUEST_ID) ,");
                SQLString.Append("(select b2.NAME from AREAS B inner join AREAS b2 on b2.id=b.PARENT_AREA_ID where B.ID =(select AREA_ID from TEAMS where ID=e.GUEST_ID)),");
                SQLString.Append(" e.id ,x.name ,e.HOME_ID,e.GUEST_ID from  EMATCHES r inner join  events e on e.id =r.EMATCHID  inner join  AREAS x on x.id =e.AREA_ID   where r.ematchid=");
                SQLString.Append(sEventID);
                m_SportsOleReaderFb = m_SportsDBMgrFb.ExecuteQuery(SQLString.ToString());
                while (m_SportsOleReaderFb.Read())
                {
                    sLeague = m_SportsOleReaderFb.GetString(0).Trim();
                    sHost = m_SportsOleReaderFb.GetString(1).Trim();
                    /// 20190215 event host/guest id sHostID = m_SportsOleReaderFb.GetInt32(2).ToString();
                    sHostID = m_SportsOleReaderFb.GetInt32(17).ToString();
                    sGuest = m_SportsOleReaderFb.GetString(3).Trim();
                    ///  sGuestID = m_SportsOleReaderFb.GetInt32(4).ToString();
                    sGuestID = m_SportsOleReaderFb.GetInt32(18).ToString();
                    sMatchDate = m_SportsOleReaderFb.GetString(5).Trim();
                    sMatchTime = m_SportsOleReaderFb.GetString(6).Trim();
                    ////  sLeagType = m_SportsOleReaderFb.GetString(7).Trim();
                    m_Title = sHost + "/" + sGuest + "-分析";
                    if (m_SportsOleReaderFb.IsDBNull(8)) sVenue = "";
                    else sVenue = m_SportsOleReaderFb.GetString(8).Trim();

                    if (!m_SportsOleReaderFb.IsDBNull(9))
                    {
                        sH_City = m_SportsOleReaderFb.GetString(9).Trim();
                        if (sH_City.Equals("")) sH_City = sEmpty;
                    }

                    if (!m_SportsOleReaderFb.IsDBNull(10))
                    {
                        sH_Country = m_SportsOleReaderFb.GetString(10).Trim();
                        if (sH_Country.Equals("")) sH_Country = sEmpty;
                    }

                    if (!m_SportsOleReaderFb.IsDBNull(11))
                    {
                        sH_Continent = m_SportsOleReaderFb.GetString(11).Trim();
                        if (sH_Continent.Equals("")) sH_Continent = sEmpty;
                    }

                    if (!m_SportsOleReaderFb.IsDBNull(12))
                    {
                        sG_City = m_SportsOleReaderFb.GetString(12).Trim();
                        if (sG_City.Equals("")) sG_City = sEmpty;
                    }

                    if (!m_SportsOleReaderFb.IsDBNull(13))
                    {
                        sG_Country = m_SportsOleReaderFb.GetString(13).Trim();
                        if (sG_Country.Equals("")) sG_Country = sEmpty;
                    }

                    if (!m_SportsOleReaderFb.IsDBNull(14))
                    {
                        sG_Continent = m_SportsOleReaderFb.GetString(14).Trim();
                        if (sG_Continent.Equals("")) sG_Continent = sEmpty;
                    }

                    sLeagID = m_SportsOleReaderFb.GetString(15).Trim();
                    sEventArea = m_SportsOleReaderFb.GetString(16).Trim();
                    if (sH_Country == sG_Country && sH_Country == sEventArea)
                    {
                        sLeagType = "2";
                    }
                    else if (sH_Country != sG_Country && sH_Country != sEventArea)
                    {
                        sLeagType = "3";
                    }
                }
                m_SportsOleReaderFb.Close();
                m_SportsDBMgrFb.Close();

                int iWeatherStatus = 0;
                string sTemperature = "";
                if (iExisted > 0)
                {   //Record exist in ANALYSIS_BG_INFO w.r.t. match count
                    //retrieve venue, weather from ANALYSIS_BG_INFO
                    SQLString.Remove(0, SQLString.Length);
                    SQLString.Append("select CMATCH_VENUE, CTEMPERATURE, IWEATHER_STATUS from ANALYSIS_BG_INFO where CACT='U' and IMATCH_CNT=");
                    //// SQLString.Append(sMatchCount);
                    SQLString.Append(sEventID);
                    m_SportsOleReaderFb = m_SportsDBMgrFb.ExecuteQuery(SQLString.ToString());
                    while (m_SportsOleReaderFb.Read())
                    {
                        if (!m_SportsOleReaderFb.IsDBNull(0)) sVenue = m_SportsOleReaderFb.GetString(0).Trim();
                        if (!m_SportsOleReaderFb.IsDBNull(1)) sTemperature = m_SportsOleReaderFb.GetString(1).Trim();
                        if (!m_SportsOleReaderFb.IsDBNull(2)) iWeatherStatus = m_SportsOleReaderFb.GetInt32(2);
                    }
                    m_SportsOleReaderFb.Close();
                    m_SportsDBMgrFb.Close();
                }

                //Display league, host, guest, match date and time
                HTMLString.Append("<tr style=\"background-color:#2F4F4F\" align=\"center\">");
                ////sMatchDate = sMatchDate.Insert(4, "/");
                ////sMatchDate = sMatchDate.Insert(7, "/");
                ////sMatchTime = sMatchTime.Insert(2, ":");
                HTMLString.Append("<th width=\"20%\"><font color=\"#F0F8FF\">");
                HTMLString.Append(sMatchDate);
                HTMLString.Append("&nbsp;");
                HTMLString.Append(sMatchTime);
                HTMLString.Append("&nbsp;");
                HTMLString.Append(sLeague);
                HTMLString.Append("</font></th><th width=\"15%\"><font color=\"#F0F8FF\">");
                HTMLString.Append(sHost);
                HTMLString.Append("&nbsp;(主)</font></th><th width=\"15%\"><font color=\"#F0F8FF\">");
                HTMLString.Append(sGuest);
                HTMLString.Append("</font></th></tr>");

                //Display details
                string sHostValue, sGuestValue;
                HTMLString.Append("<tr align=\"center\"><th>");
                if (sLeagType.Equals("2"))
                {
                    HTMLString.Append("國家:");
                    sHostValue = sH_Country;
                    sGuestValue = sG_Country;
                }
                else if (sLeagType.Equals("3"))
                {
                    HTMLString.Append("洲份:");
                    sHostValue = sH_Continent;
                    sGuestValue = sG_Continent;
                }
                else
                {
                    HTMLString.Append("城巿:");
                    sHostValue = sH_City;
                    sGuestValue = sG_City;
                }
                //if (sLeagType.Equals("1"))
                //{
                //    HTMLString.Append("城巿:");
                //    sHostValue = sH_City;
                //    sGuestValue = sG_City;
                //}
                //else if (sLeagType.Equals("2"))
                //{
                //    HTMLString.Append("國家:");
                //    sHostValue = sH_Country;
                //    sGuestValue = sG_Country;
                //}
                //else
                //{
                //    HTMLString.Append("洲份:");
                //    sHostValue = sH_Continent;
                //    sGuestValue = sG_Continent;
                //}
                HTMLString.Append("</th><td>");
                HTMLString.Append(sHostValue);
                HTMLString.Append("</td><td>");
                HTMLString.Append(sGuestValue);
                HTMLString.Append("</td></tr>");

                HTMLString.Append("<tr align=\"center\"><th>球場:</th><td colspan=2 align=\"left\"><input type=\"text\" name=\"venue\" value=\"");
                HTMLString.Append(sVenue);
                HTMLString.Append("\" maxlength=\"10\"></td></tr>");

                HTMLString.Append("<tr align=\"center\"><th>天氣(&#186;C):</th><td align=\"left\"><input type=\"text\" name=\"temperature\" value=\"");
                HTMLString.Append(sTemperature);
                HTMLString.Append("\" maxlength=\"8\"></td><td align=\"left\"><select name=\"weatherstatus\"><option value=");
                HTMLString.Append(iWeatherStatus.ToString());
                HTMLString.Append(">");
                HTMLString.Append(arrWeather[iWeatherStatus]);
                for (iIdx = 0; iIdx < arrWeather.Length; iIdx++)
                {
                    if (!arrWeather[iIdx].Equals(arrWeather[iWeatherStatus]))
                    {
                        HTMLString.Append("<option value=");
                        HTMLString.Append(iIdx.ToString());
                        HTMLString.Append(">");
                        HTMLString.Append(arrWeather[iIdx]);
                    }
                }
                HTMLString.Append("</select></td></tr>");

                //Display history result
                //Retrieve league from table
                ArrayList LeagueAL = new ArrayList();
                SQLString.Remove(0, SQLString.Length);
                // SQLString.Append("select ALIAS from LEAGINFO order by LEAG_ORDER, ALIAS");
                SQLString.Append("SELECT R.CLEAGUE_ALIAS_NAME FROM LEAGUE_INFO r order by CLEAGUE_ALIAS_NAME, COUNTRY_CHI_NAME");
                m_SportsOleReaderFb = m_SportsDBMgrFb.ExecuteQuery(SQLString.ToString());
                while (m_SportsOleReaderFb.Read())
                {
                    LeagueAL.Add(m_SportsOleReaderFb.GetString(0).Trim());
                }
                m_SportsOleReaderFb.Close();
                m_SportsDBMgrFb.Close();
                LeagueAL.TrimToSize();

                //Retrieve records from ANALYSIS_HISTORY_INFO
                iIdx = 0;
                MatchHistory[] History = new MatchHistory[4];
                SQLString.Remove(0, SQLString.Length);
                SQLString.Append("select  first 4 LEAGUEALIAS, IMATCHMONTH, IMATCHYEAR, IMATCHSTATUS, IHOSTSCORE, IGUESTSCORE,EVENTID from ANALYSIS_HISTORY_INFO  where CACT='U' and IMATCH_CNT=");
                //SQLString.Append("select  first 4 c.alias, IMATCHMONTH, IMATCHYEAR, IMATCHSTATUS, IHOSTSCORE, IGUESTSCORE,EVENTID from ANALYSIS_HISTORY_INFO inner join COMPETITIONS c on c.id=LEAGUEALIASID where CACT='U' and IMATCH_CNT=");
                ////SQLString.Append(sMatchCount);
                SQLString.Append(sEventID);
                SQLString.Append(" order by IREC");
                m_SportsOleReaderFb = m_SportsDBMgrFb.ExecuteQuery(SQLString.ToString());
                while (m_SportsOleReaderFb.Read())
                {
                    if (!(m_SportsOleReaderFb.IsDBNull(0) || m_SportsOleReaderFb.IsDBNull(4) || m_SportsOleReaderFb.IsDBNull(5)))
                    {
                        History[iIdx].sAlias = m_SportsOleReaderFb.GetString(0).Trim();
                        History[iIdx].iMonth = m_SportsOleReaderFb.GetInt32(1);
                        History[iIdx].iYear = m_SportsOleReaderFb.GetInt32(2);
                        History[iIdx].iStatus = m_SportsOleReaderFb.GetInt32(3);
                        History[iIdx].iHostScore = m_SportsOleReaderFb.GetInt32(4);
                        History[iIdx].iGuestScore = m_SportsOleReaderFb.GetInt32(5);
                        History[iIdx].iEventid = m_SportsOleReaderFb.GetInt32(6);
                        iIdx++;
                    }
                }
                m_SportsOleReaderFb.Close();
                m_SportsDBMgrFb.Close();

                int iYear, iMonth, iNewValue = 0, iNoOfOpt = 0, iYrDiff;
                const int iYY = 20, iMM = 12;

                HTMLString.Append("<tr style=\"background-color:#5F9EA0\" align=\"left\"><th colspan=3><font color=\"#F0F8FF\">對賽往績:</font></th></tr>");
                for (iIdx = 0; iIdx < HISTORYCOUNT; iIdx++)
                {       //init 4 records
                    iYear = DateTime.Today.Year;
                    iMonth = DateTime.Today.Month;
                    HTMLString.Append("<tr align=\"center\"><td colspan=3>");
                    //Match Month
                    HTMLString.Append("<select name=\"MatchMonth\">");
                    if (History[iIdx].iMonth > 0)
                    {
                        iMonth = History[iIdx].iMonth;
                    }
                    else
                    {
                        HTMLString.Append("<option value=\"0\">月");
                    }
                    HTMLString.Append("<option value=\"");
                    HTMLString.Append(iMonth.ToString());
                    HTMLString.Append("\">");
                    HTMLString.Append(iMonth.ToString());
                    iNoOfOpt = iMonth + 1;
                    while (iNoOfOpt != iMonth)
                    {
                        if (iNoOfOpt > iMM) iNoOfOpt = 1;
                        if (!(iMonth == 1 && iNoOfOpt == 1))
                        {
                            HTMLString.Append("<option value=\"");
                            HTMLString.Append(iNoOfOpt.ToString());
                            HTMLString.Append("\">");
                            HTMLString.Append(iNoOfOpt.ToString());
                            iNoOfOpt++;
                        }
                    }
                    if (History[iIdx].iMonth > 0)
                    {
                        HTMLString.Append("<option value=\"0\">月");
                    }
                    HTMLString.Append("</select>/");

                    //Match Year
                    HTMLString.Append("<select name=\"MatchYear\">");
                    if (History[iIdx].iYear > 0)
                    {
                        iYrDiff = iYear - History[iIdx].iYear;
                        iYear = History[iIdx].iYear;
                    }
                    else
                    {
                        iYrDiff = 0;
                        HTMLString.Append("<option value=\"0\">年");
                    }
                    HTMLString.Append("<option value=\"");
                    HTMLString.Append(iYear.ToString());
                    HTMLString.Append("\">");
                    HTMLString.Append(iYear.ToString());
                    if (iYrDiff > 0)
                    {
                        for (iNoOfOpt = 1; iNoOfOpt <= iYrDiff; iNoOfOpt++)
                        {
                            iNewValue = iYear + iNoOfOpt;
                            HTMLString.Append("<option value=\"");
                            HTMLString.Append(iNewValue.ToString());
                            HTMLString.Append("\">");
                            HTMLString.Append(iNewValue.ToString());
                        }
                    }
                    for (iNoOfOpt = 1; iNoOfOpt <= (iYY - iYrDiff); iNoOfOpt++)
                    {
                        iNewValue = iYear - iNoOfOpt;
                        HTMLString.Append("<option value=\"");
                        HTMLString.Append(iNewValue.ToString());
                        HTMLString.Append("\">");
                        HTMLString.Append(iNewValue.ToString());
                    }
                    if (History[iIdx].iYear > 0)
                    {
                        HTMLString.Append("<option value=\"0\">年");
                    }
                    HTMLString.Append("</select>&nbsp;");

                    HTMLString.Append("<select name=\"leaguealias\">");
                    if (History[iIdx].sAlias != null)
                    {
                        HTMLString.Append("<option value=\"");
                        HTMLString.Append(History[iIdx].sAlias);
                        HTMLString.Append("\">");
                        HTMLString.Append(History[iIdx].sAlias);
                    }
                    for (iNoOfOpt = 0; iNoOfOpt < LeagueAL.Count; iNoOfOpt++)
                    {
                        if (!LeagueAL[iNoOfOpt].Equals(History[iIdx].sAlias))
                        {
                            HTMLString.Append("<option value=\"");
                            HTMLString.Append(LeagueAL[iNoOfOpt]);
                            HTMLString.Append("\">");
                            HTMLString.Append(LeagueAL[iNoOfOpt]);
                        }
                    }
                    HTMLString.Append("</select>&nbsp;");

                    HTMLString.Append("<select name=\"fields\"><option value=\"");
                    HTMLString.Append(History[iIdx].iStatus.ToString());
                    HTMLString.Append("\">");
                    HTMLString.Append(arrFields[History[iIdx].iStatus]);
                    for (iNoOfOpt = 0; iNoOfOpt < arrFields.Length; iNoOfOpt++)
                    {
                        if (History[iIdx].iStatus != iNoOfOpt)
                        {
                            HTMLString.Append("<option value=\"");
                            HTMLString.Append(iNoOfOpt.ToString());
                            HTMLString.Append("\">");
                            HTMLString.Append(arrFields[iNoOfOpt]);
                        }
                    }
                    HTMLString.Append("</select>&nbsp;");

                    HTMLString.Append("<input type=\"text\" name=\"hostscore\" value=\"");
                    HTMLString.Append(History[iIdx].iHostScore);
                    HTMLString.Append("\" size=1 maxlength=2 onChange=\"onHostScoreChanged(");
                    HTMLString.Append(iIdx.ToString());
                    HTMLString.Append(")\">:<input type=\"text\" name=\"guestscore\" value=\"");
                    HTMLString.Append(History[iIdx].iGuestScore);
                    HTMLString.Append("\" size=1 maxlength=2 onChange=\"onGuestScoreChanged(");
                    HTMLString.Append(iIdx.ToString());
                    HTMLString.Append(")\">");
                    HTMLString.Append("<input type =\"hidden\"  name=\"hEventid\" value=\"");
                     HTMLString.Append(History[iIdx].iEventid);
                    HTMLString.Append("\">");
                    HTMLString.Append("</td></tr>");
                }

                //Players information
                int iPos = -1, iNewLine = 0, iFirstPlayer = 0;
                string[] arrPos;
                arrPos = (string[])HttpContext.Current.Application["positionItemsArray"];

                //Host players
                HTMLString.Append("<tr style=\"background-color:#5F9EA0\" align=\"left\"><th colspan=\"3\"><font color=\"#F0F8FF\">出場名單:</font></th></tr>");
                HTMLString.Append("<tr><th>隊伍名稱:</th><td>");
                HTMLString.Append("<b>" + sHost + "</b>");
                HTMLString.Append(": <a href=\"PlayersRetrieval.aspx?teamID=");
                HTMLString.Append(sHostID);
                ////  HTMLString.Append("&leagID=");
                HTMLString.Append("&eventid=");
                ////HTMLString.Append(sLeagID);
                HTMLString.Append(sEventID);
                HTMLString.Append("\">(修改)</a></td>");

                //Guest players
                HTMLString.Append("<td>");
                HTMLString.Append("<b>" + sGuest + "</b>");
                HTMLString.Append(": <a href=\"PlayersRetrieval.aspx?teamID=");
                HTMLString.Append(sGuestID);
                ////   HTMLString.Append("&leagID=");
                HTMLString.Append("&eventid=");
                //// HTMLString.Append(sLeagID);
                HTMLString.Append(sEventID);
                HTMLString.Append("\">(修改)</a></td>");

                HTMLString.Append("<tr><th>球員名稱:</th><td>");
                //Host players
                SQLString.Remove(0, SQLString.Length);
                SQLString.Append("select IPOS, CPLAYER_NAME||'/'||cengname, IPLAYER_NO from PLAYERS_INFO where TEAM_ID=");
                SQLString.Append(sHostID);
                SQLString.Append("   AND EVENT_ID=" + sEventID + " and IROSTER=1 order by IPOS ASC, IPLAYER_NO");
                m_SportsOleReaderFb = m_SportsDBMgrFb.ExecuteQuery(SQLString.ToString());
                while (m_SportsOleReaderFb.Read())
                {
                    if (m_SportsOleReaderFb.GetInt32(0) != iPos)
                    {
                        if (iNewLine == 0)
                        {
                            iPos = m_SportsOleReaderFb.GetInt32(0);
                            HTMLString.Append(arrPos[iPos]);
                            HTMLString.Append("<br>");
                        }
                        else
                        {
                            iPos = m_SportsOleReaderFb.GetInt32(0);
                            HTMLString.Append("<br>");
                            HTMLString.Append(arrPos[iPos]);
                            HTMLString.Append("<br>");
                            iFirstPlayer = 0;
                        }
                        iNewLine++;
                    }
                    //if(iFirstPlayer > 0) HTMLString.Append("<br>");
                    iFirstPlayer++;
                    if (!m_SportsOleReaderFb.IsDBNull(2))
                    {
                        if (m_SportsOleReaderFb.GetInt32(2) != -1)
                        {
                            HTMLString.Append(m_SportsOleReaderFb.GetInt32(2).ToString() + " " + m_SportsOleReaderFb.GetString(1).Trim() + "<br>");
                        }
                        else
                        {
                            HTMLString.Append(m_SportsOleReaderFb.GetString(1).Trim() + "<br>");
                        }
                    }
                    else
                    {
                        HTMLString.Append(m_SportsOleReaderFb.GetString(1).Trim() + "<br>");
                    }
                }
                m_SportsOleReaderFb.Close();
                m_SportsDBMgrFb.Close();
                if (iNewLine == 0) HTMLString.Append(sEmpty);
                HTMLString.Append("</td>");


                HTMLString.Append("<td>");
                //Guest players
                iPos = -1;
                iNewLine = 0;
                iFirstPlayer = 0;
                SQLString.Remove(0, SQLString.Length);
                SQLString.Append("select IPOS, CPLAYER_NAME||'/'||cengname, IPLAYER_NO from PLAYERS_INFO where TEAM_ID=");
                SQLString.Append(sGuestID);
                SQLString.Append(" AND EVENT_ID=" + sEventID + " and IROSTER=1 order by IPOS ASC, IPLAYER_NO");
                m_SportsOleReaderFb = m_SportsDBMgrFb.ExecuteQuery(SQLString.ToString());
                while (m_SportsOleReaderFb.Read())
                {
                    if (m_SportsOleReaderFb.GetInt32(0) != iPos)
                    {
                        if (iNewLine == 0)
                        {
                            iPos = m_SportsOleReaderFb.GetInt32(0);
                            HTMLString.Append(arrPos[iPos]);
                            HTMLString.Append("<br>");
                        }
                        else
                        {
                            iPos = m_SportsOleReaderFb.GetInt32(0);
                            HTMLString.Append("<br>");
                            HTMLString.Append(arrPos[iPos]);
                            HTMLString.Append("<br>");
                            iFirstPlayer = 0;
                        }
                        iNewLine++;
                    }
                    //if(iFirstPlayer > 0) HTMLString.Append("<br>");
                    iFirstPlayer++;
                    if (!m_SportsOleReaderFb.IsDBNull(2))
                    {
                        if (m_SportsOleReaderFb.GetInt32(2) != -1)
                        {
                            HTMLString.Append(m_SportsOleReaderFb.GetInt32(2).ToString() + " " + m_SportsOleReaderFb.GetString(1).Trim() + "<br>");
                        }
                        else
                        {
                            HTMLString.Append(m_SportsOleReaderFb.GetString(1).Trim() + "<br>");
                        }
                    }
                    else
                    {
                        HTMLString.Append(m_SportsOleReaderFb.GetString(1).Trim() + "<br>");
                    }
                }
                m_SportsOleReaderFb.Close();
                m_SportsDBMgrFb.Close();
                if (iNewLine == 0) HTMLString.Append(sEmpty);
                HTMLString.Append("</td></tr>");

                //Remarks - Free text
                string sRemarks = "";
                SQLString.Remove(0, SQLString.Length);
                SQLString.Append("select CREMARKS from ANALYSIS_REMARK_INFO where CACT='U' and IMATCH_CNT=");
                ////SQLString.Append(sMatchCount);
                SQLString.Append(sEventID);
                m_SportsOleReaderFb = m_SportsDBMgrFb.ExecuteQuery(SQLString.ToString());
                while (m_SportsOleReaderFb.Read())
                {
                    if (!m_SportsOleReaderFb.IsDBNull(0))
                    {
                        sRemarks = m_SportsOleReaderFb.GetString(0).Trim();
                    }
                }
                m_SportsOleReaderFb.Close();
                m_SportsDBMgrFb.Close();
                HTMLString.Append("<tr style=\"background-color:#5F9EA0\" align=\"left\"><th colspan=3><font color=\"#F0F8FF\">附加訊息: (最多300個位元)</font></th></tr><tr align=\"center\"><td colspan=3><textarea style=\"height:100%;width:80%\"  name=\"remarks\" rows=12 cols=20 onChange=\"checkRemarkByte()\">");
                HTMLString.Append(sRemarks.Replace(DBCR, PAGECR));
                HTMLString.Append("</textarea></td></tr><input type=\"hidden\" name=\"matchcount\" value=\"");
                //// HTMLString.Append(sMatchCount);
                HTMLString.Append(sEventID);
                HTMLString.Append("\">");

                //return HTMLString.ToString();
            }
            catch (Exception ex)
            {
                m_SportsLog.FilePath = ConfigurationManager.AppSettings["errlog"];
                m_SportsLog.SetFileName(0, LOGFILESUFFIX);
                m_SportsLog.Open();
                m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " AnalysisModify.cs.GetMatches(): " + ex.ToString());
                m_SportsLog.Close();
                HTMLString.Remove(0, HTMLString.Length);
                HTMLString.Append(ConfigurationManager.AppSettings["accessErrorMsg"]);
            }
            //}

            return HTMLString.ToString();

        }
        //      public string GetMatchesx() {
        //	int iIdx = 0;
        //	string sMatchCount;
        //	string[] arrWeather, arrFields;
        //	StringBuilder HTMLString = new StringBuilder();
        //          //int xx = 1;
        //          for (int xx = 1; xx < 100; xx++)
        //          {
        //              sMatchCount = HttpContext.Current.Request.QueryString["matchcnt"] == null ? xx.ToString() : HttpContext.Current.Request.QueryString["matchcnt"].Trim();
        //              arrWeather = (string[])HttpContext.Current.Application["weatherItemsArray"];
        //              arrFields = (string[])HttpContext.Current.Application["fieldItemsArray"];

        //              try
        //              {
        //                  int iExisted;
        //                  const string sEmpty = "未輸入";
        //                  string sLeague = "", sHost = "", sHostID = "", sGuest = "", sGuestID = "", sMatchDate = "", sMatchTime = "", sLeagType = "", sVenue = "", sLeagID = "";
        //                  string sH_City = sEmpty, sH_Country = sEmpty, sH_Continent = sEmpty, sG_City = sEmpty, sG_Country = sEmpty, sG_Continent = sEmpty;

        //                  SQLString.Remove(0, SQLString.Length);
        //                  SQLString.Append("select count(IMATCH_CNT) from ANALYSIS_BG_INFO where CACT='U' and IMATCH_CNT=");
        //                  SQLString.Append(sMatchCount);
        //                  iExisted = m_SportsDBMgr.ExecuteScalar(SQLString.ToString());
        //                  m_SportsDBMgr.Close();

        //                  //retrieve information related to host and guest
        //                  SQLString.Remove(0, SQLString.Length);
        //                  SQLString.Append("select game.league, game.host, (select team_id from teaminfo where teamname=game.host), game.guest, (select team_id from teaminfo where teamname=game.guest), game.matchdate, game.matchtime, leag.LEAGUETYPE, ");
        //                  SQLString.Append("(select venue from teaminfo where teamname=game.host), (select city from teaminfo where teamname=game.host), (select country from teaminfo where teamname=game.host), (select continent from teaminfo where teamname=game.host), ");
        //                  SQLString.Append("(select city from teaminfo where teamname=game.guest), (select country from teaminfo where teamname=game.guest), (select continent from teaminfo where teamname=game.guest), leag.leag_id ");
        //                  SQLString.Append("from gameinfo game, leaginfo leag where game.league=leag.alias and game.match_cnt=");
        //                  SQLString.Append(sMatchCount);
        //                  m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
        //                  while (m_SportsOleReader.Read())
        //                  {
        //                      sLeague = m_SportsOleReader.GetString(0).Trim();
        //                      sHost = m_SportsOleReader.GetString(1).Trim();
        //                      sHostID = m_SportsOleReader.GetInt32(2).ToString();
        //                      sGuest = m_SportsOleReader.GetString(3).Trim();
        //                      sGuestID = m_SportsOleReader.GetInt32(4).ToString();
        //                      sMatchDate = m_SportsOleReader.GetString(5).Trim();
        //                      sMatchTime = m_SportsOleReader.GetString(6).Trim();
        //                      sLeagType = m_SportsOleReader.GetString(7).Trim();

        //                      if (m_SportsOleReader.IsDBNull(8)) sVenue = "";
        //                      else sVenue = m_SportsOleReader.GetString(8).Trim();

        //                      if (!m_SportsOleReader.IsDBNull(9))
        //                      {
        //                          sH_City = m_SportsOleReader.GetString(9).Trim();
        //                          if (sH_City.Equals("")) sH_City = sEmpty;
        //                      }

        //                      if (!m_SportsOleReader.IsDBNull(10))
        //                      {
        //                          sH_Country = m_SportsOleReader.GetString(10).Trim();
        //                          if (sH_Country.Equals("")) sH_Country = sEmpty;
        //                      }

        //                      if (!m_SportsOleReader.IsDBNull(11))
        //                      {
        //                          sH_Continent = m_SportsOleReader.GetString(11).Trim();
        //                          if (sH_Continent.Equals("")) sH_Continent = sEmpty;
        //                      }

        //                      if (!m_SportsOleReader.IsDBNull(12))
        //                      {
        //                          sG_City = m_SportsOleReader.GetString(12).Trim();
        //                          if (sG_City.Equals("")) sG_City = sEmpty;
        //                      }

        //                      if (!m_SportsOleReader.IsDBNull(13))
        //                      {
        //                          sG_Country = m_SportsOleReader.GetString(13).Trim();
        //                          if (sG_Country.Equals("")) sG_Country = sEmpty;
        //                      }

        //                      if (!m_SportsOleReader.IsDBNull(14))
        //                      {
        //                          sG_Continent = m_SportsOleReader.GetString(14).Trim();
        //                          if (sG_Continent.Equals("")) sG_Continent = sEmpty;
        //                      }

        //                      sLeagID = m_SportsOleReader.GetString(15).Trim();
        //                  }
        //                  m_SportsOleReader.Close();
        //                  m_SportsDBMgr.Close();

        //                  int iWeatherStatus = 0;
        //                  string sTemperature = "";
        //                  if (iExisted > 0)
        //                  {   //Record exist in ANALYSIS_BG_INFO w.r.t. match count
        //                      //retrieve venue, weather from ANALYSIS_BG_INFO
        //                      SQLString.Remove(0, SQLString.Length);
        //                      SQLString.Append("select CMATCH_VENUE, CTEMPERATURE, IWEATHER_STATUS from ANALYSIS_BG_INFO where CACT='U' and IMATCH_CNT=");
        //                      SQLString.Append(sMatchCount);
        //                      m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
        //                      while (m_SportsOleReader.Read())
        //                      {
        //                          if (!m_SportsOleReader.IsDBNull(0)) sVenue = m_SportsOleReader.GetString(0).Trim();
        //                          if (!m_SportsOleReader.IsDBNull(1)) sTemperature = m_SportsOleReader.GetString(1).Trim();
        //                          if (!m_SportsOleReader.IsDBNull(2)) iWeatherStatus = m_SportsOleReader.GetInt32(2);
        //                      }
        //                      m_SportsOleReader.Close();
        //                      m_SportsDBMgr.Close();
        //                  }

        //                  //Display league, host, guest, match date and time
        //                  HTMLString.Append("<tr style=\"background-color:#2F4F4F\" align=\"center\">");
        //                  sMatchDate = sMatchDate.Insert(4, "/");
        //                  sMatchDate = sMatchDate.Insert(7, "/");
        //                  sMatchTime = sMatchTime.Insert(2, ":");
        //                  HTMLString.Append("<th width=\"20%\"><font color=\"#F0F8FF\">");
        //                  HTMLString.Append(sMatchDate);
        //                  HTMLString.Append("&nbsp;");
        //                  HTMLString.Append(sMatchTime);
        //                  HTMLString.Append("&nbsp;");
        //                  HTMLString.Append(sLeague);
        //                  HTMLString.Append("</font></th><th width=\"15%\"><font color=\"#F0F8FF\">");
        //                  HTMLString.Append(sHost);
        //                  HTMLString.Append("&nbsp;(主)</font></th><th width=\"15%\"><font color=\"#F0F8FF\">");
        //                  HTMLString.Append(sGuest);
        //                  HTMLString.Append("</font></th></tr>");

        //                  //Display details
        //                  string sHostValue, sGuestValue;
        //                  HTMLString.Append("<tr align=\"center\"><th>");
        //                  if (sLeagType.Equals("1"))
        //                  {
        //                      HTMLString.Append("城巿:");
        //                      sHostValue = sH_City;
        //                      sGuestValue = sG_City;
        //                  }
        //                  else if (sLeagType.Equals("2"))
        //                  {
        //                      HTMLString.Append("國家:");
        //                      sHostValue = sH_Country;
        //                      sGuestValue = sG_Country;
        //                  }
        //                  else
        //                  {
        //                      HTMLString.Append("洲份:");
        //                      sHostValue = sH_Continent;
        //                      sGuestValue = sG_Continent;
        //                  }
        //                  HTMLString.Append("</th><td>");
        //                  HTMLString.Append(sHostValue);
        //                  HTMLString.Append("</td><td>");
        //                  HTMLString.Append(sGuestValue);
        //                  HTMLString.Append("</td></tr>");

        //                  HTMLString.Append("<tr align=\"center\"><th>球場:</th><td colspan=2 align=\"left\"><input type=\"text\" name=\"venue\" value=\"");
        //                  HTMLString.Append(sVenue);
        //                  HTMLString.Append("\" maxlength=\"10\"></td></tr>");

        //                  HTMLString.Append("<tr align=\"center\"><th>天氣(&#186;C):</th><td align=\"left\"><input type=\"text\" name=\"temperature\" value=\"");
        //                  HTMLString.Append(sTemperature);
        //                  HTMLString.Append("\" maxlength=\"8\"></td><td align=\"left\"><select name=\"weatherstatus\"><option value=");
        //                  HTMLString.Append(iWeatherStatus.ToString());
        //                  HTMLString.Append(">");
        //                  HTMLString.Append(arrWeather[iWeatherStatus]);
        //                  for (iIdx = 0; iIdx < arrWeather.Length; iIdx++)
        //                  {
        //                      if (!arrWeather[iIdx].Equals(arrWeather[iWeatherStatus]))
        //                      {
        //                          HTMLString.Append("<option value=");
        //                          HTMLString.Append(iIdx.ToString());
        //                          HTMLString.Append(">");
        //                          HTMLString.Append(arrWeather[iIdx]);
        //                      }
        //                  }
        //                  HTMLString.Append("</select></td></tr>");

        //                  //Display history result
        //                  //Retrieve league from table
        //                  ArrayList LeagueAL = new ArrayList();
        //                  SQLString.Remove(0, SQLString.Length);
        //                  SQLString.Append("select ALIAS from LEAGINFO order by LEAG_ORDER, ALIAS");
        //                  m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
        //                  while (m_SportsOleReader.Read())
        //                  {
        //                      LeagueAL.Add(m_SportsOleReader.GetString(0).Trim());
        //                  }
        //                  m_SportsOleReader.Close();
        //                  m_SportsDBMgr.Close();
        //                  LeagueAL.TrimToSize();

        //                  //Retrieve records from ANALYSIS_HISTORY_INFO
        //                  iIdx = 0;
        //                  MatchHistory[] History = new MatchHistory[4];
        //                  SQLString.Remove(0, SQLString.Length);
        //                  SQLString.Append("select CLEAGUEALIAS, IMATCHMONTH, IMATCHYEAR, IMATCHSTATUS, IHOSTSCORE, IGUESTSCORE from ANALYSIS_HISTORY_INFO where CACT='U' and IMATCH_CNT=");
        //                  SQLString.Append(sMatchCount);
        //                  SQLString.Append(" order by IREC");
        //                  m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
        //                  while (m_SportsOleReader.Read())
        //                  {
        //                      if (!(m_SportsOleReader.IsDBNull(0) || m_SportsOleReader.IsDBNull(4) || m_SportsOleReader.IsDBNull(5)))
        //                      {
        //                          History[iIdx].sAlias = m_SportsOleReader.GetString(0).Trim();
        //                          History[iIdx].iMonth = m_SportsOleReader.GetInt32(1);
        //                          History[iIdx].iYear = m_SportsOleReader.GetInt32(2);
        //                          History[iIdx].iStatus = m_SportsOleReader.GetInt32(3);
        //                          History[iIdx].iHostScore = m_SportsOleReader.GetInt32(4);
        //                          History[iIdx].iGuestScore = m_SportsOleReader.GetInt32(5);
        //                          iIdx++;
        //                      }
        //                  }
        //                  m_SportsOleReader.Close();
        //                  m_SportsDBMgr.Close();

        //                  int iYear, iMonth, iNewValue = 0, iNoOfOpt = 0, iYrDiff;
        //                  const int iYY = 20, iMM = 12;

        //                  HTMLString.Append("<tr style=\"background-color:#5F9EA0\" align=\"left\"><th colspan=3><font color=\"#F0F8FF\">對賽往績:</font></th></tr>");
        //                  for (iIdx = 0; iIdx < HISTORYCOUNT; iIdx++)
        //                  {       //init 4 records
        //                      iYear = DateTime.Today.Year;
        //                      iMonth = DateTime.Today.Month;
        //                      HTMLString.Append("<tr align=\"center\"><td colspan=3>");
        //                      //Match Month
        //                      HTMLString.Append("<select name=\"MatchMonth\">");
        //                      if (History[iIdx].iMonth > 0)
        //                      {
        //                          iMonth = History[iIdx].iMonth;
        //                      }
        //                      else
        //                      {
        //                          HTMLString.Append("<option value=\"0\">月");
        //                      }
        //                      HTMLString.Append("<option value=\"");
        //                      HTMLString.Append(iMonth.ToString());
        //                      HTMLString.Append("\">");
        //                      HTMLString.Append(iMonth.ToString());
        //                      iNoOfOpt = iMonth + 1;
        //                      while (iNoOfOpt != iMonth)
        //                      {
        //                          if (iNoOfOpt > iMM) iNoOfOpt = 1;
        //                          if (!(iMonth == 1 && iNoOfOpt == 1))
        //                          {
        //                              HTMLString.Append("<option value=\"");
        //                              HTMLString.Append(iNoOfOpt.ToString());
        //                              HTMLString.Append("\">");
        //                              HTMLString.Append(iNoOfOpt.ToString());
        //                              iNoOfOpt++;
        //                          }
        //                      }
        //                      if (History[iIdx].iMonth > 0)
        //                      {
        //                          HTMLString.Append("<option value=\"0\">月");
        //                      }
        //                      HTMLString.Append("</select>/");

        //                      //Match Year
        //                      HTMLString.Append("<select name=\"MatchYear\">");
        //                      if (History[iIdx].iYear > 0)
        //                      {
        //                          iYrDiff = iYear - History[iIdx].iYear;
        //                          iYear = History[iIdx].iYear;
        //                      }
        //                      else
        //                      {
        //                          iYrDiff = 0;
        //                          HTMLString.Append("<option value=\"0\">年");
        //                      }
        //                      HTMLString.Append("<option value=\"");
        //                      HTMLString.Append(iYear.ToString());
        //                      HTMLString.Append("\">");
        //                      HTMLString.Append(iYear.ToString());
        //                      if (iYrDiff > 0)
        //                      {
        //                          for (iNoOfOpt = 1; iNoOfOpt <= iYrDiff; iNoOfOpt++)
        //                          {
        //                              iNewValue = iYear + iNoOfOpt;
        //                              HTMLString.Append("<option value=\"");
        //                              HTMLString.Append(iNewValue.ToString());
        //                              HTMLString.Append("\">");
        //                              HTMLString.Append(iNewValue.ToString());
        //                          }
        //                      }
        //                      for (iNoOfOpt = 1; iNoOfOpt <= (iYY - iYrDiff); iNoOfOpt++)
        //                      {
        //                          iNewValue = iYear - iNoOfOpt;
        //                          HTMLString.Append("<option value=\"");
        //                          HTMLString.Append(iNewValue.ToString());
        //                          HTMLString.Append("\">");
        //                          HTMLString.Append(iNewValue.ToString());
        //                      }
        //                      if (History[iIdx].iYear > 0)
        //                      {
        //                          HTMLString.Append("<option value=\"0\">年");
        //                      }
        //                      HTMLString.Append("</select>&nbsp;");

        //                      HTMLString.Append("<select name=\"leaguealias\">");
        //                      if (History[iIdx].sAlias != null)
        //                      {
        //                          HTMLString.Append("<option value=\"");
        //                          HTMLString.Append(History[iIdx].sAlias);
        //                          HTMLString.Append("\">");
        //                          HTMLString.Append(History[iIdx].sAlias);
        //                      }
        //                      for (iNoOfOpt = 0; iNoOfOpt < LeagueAL.Count; iNoOfOpt++)
        //                      {
        //                          if (!LeagueAL[iNoOfOpt].Equals(History[iIdx].sAlias))
        //                          {
        //                              HTMLString.Append("<option value=\"");
        //                              HTMLString.Append(LeagueAL[iNoOfOpt]);
        //                              HTMLString.Append("\">");
        //                              HTMLString.Append(LeagueAL[iNoOfOpt]);
        //                          }
        //                      }
        //                      HTMLString.Append("</select>&nbsp;");

        //                      HTMLString.Append("<select name=\"fields\"><option value=\"");
        //                      HTMLString.Append(History[iIdx].iStatus.ToString());
        //                      HTMLString.Append("\">");
        //                      HTMLString.Append(arrFields[History[iIdx].iStatus]);
        //                      for (iNoOfOpt = 0; iNoOfOpt < arrFields.Length; iNoOfOpt++)
        //                      {
        //                          if (History[iIdx].iStatus != iNoOfOpt)
        //                          {
        //                              HTMLString.Append("<option value=\"");
        //                              HTMLString.Append(iNoOfOpt.ToString());
        //                              HTMLString.Append("\">");
        //                              HTMLString.Append(arrFields[iNoOfOpt]);
        //                          }
        //                      }
        //                      HTMLString.Append("</select>&nbsp;");

        //                      HTMLString.Append("<input type=\"text\" name=\"hostscore\" value=\"");
        //                      HTMLString.Append(History[iIdx].iHostScore);
        //                      HTMLString.Append("\" size=1 maxlength=2 onChange=\"onHostScoreChanged(");
        //                      HTMLString.Append(iIdx.ToString());
        //                      HTMLString.Append(")\">:<input type=\"text\" name=\"guestscore\" value=\"");
        //                      HTMLString.Append(History[iIdx].iGuestScore);
        //                      HTMLString.Append("\" size=1 maxlength=2 onChange=\"onGuestScoreChanged(");
        //                      HTMLString.Append(iIdx.ToString());
        //                      HTMLString.Append(")\"></td></tr>");
        //                  }

        //                  //Players information
        //                  int iPos = -1, iNewLine = 0, iFirstPlayer = 0;
        //                  string[] arrPos;
        //                  arrPos = (string[])HttpContext.Current.Application["positionItemsArray"];

        //                  //Host players
        //                  HTMLString.Append("<tr style=\"background-color:#5F9EA0\" align=\"left\"><th colspan=\"3\"><font color=\"#F0F8FF\">出場名單:</font></th></tr>");
        //                  HTMLString.Append("<tr><th>隊伍名稱:</th><td>");
        //                  HTMLString.Append("<b>" + sHost + "</b>");
        //                  HTMLString.Append(": <a href=\"PlayersRetrieval.aspx?teamID=");
        //                  HTMLString.Append(sHostID);
        //                  HTMLString.Append("&leagID=");
        //                  HTMLString.Append(sLeagID);
        //                  HTMLString.Append("\">(修改)</a></td>");

        //                  //Guest players
        //                  HTMLString.Append("<td>");
        //                  HTMLString.Append("<b>" + sGuest + "</b>");
        //                  HTMLString.Append(": <a href=\"PlayersRetrieval.aspx?teamID=");
        //                  HTMLString.Append(sGuestID);
        //                  HTMLString.Append("&leagID=");
        //                  HTMLString.Append(sLeagID);
        //                  HTMLString.Append("\">(修改)</a></td>");

        //                  HTMLString.Append("<tr><th>球員名稱:</th><td>");
        //                  //Host players
        //                  SQLString.Remove(0, SQLString.Length);
        //                  SQLString.Append("select IPOS, CPLAYER_NAME, IPLAYER_NO from PLAYERS_INFO where TEAM_ID=");
        //                  SQLString.Append(sHostID);
        //                  SQLString.Append(" and IROSTER=1 order by IPOS desc, IPLAYER_NO");
        //                  m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
        //                  while (m_SportsOleReader.Read())
        //                  {
        //                      if (m_SportsOleReader.GetInt32(0) != iPos)
        //                      {
        //                          if (iNewLine == 0)
        //                          {
        //                              iPos = m_SportsOleReader.GetInt32(0);
        //                              HTMLString.Append(arrPos[iPos]);
        //                              HTMLString.Append("<br>");
        //                          }
        //                          else
        //                          {
        //                              iPos = m_SportsOleReader.GetInt32(0);
        //                              HTMLString.Append("<br>");
        //                              HTMLString.Append(arrPos[iPos]);
        //                              HTMLString.Append("<br>");
        //                              iFirstPlayer = 0;
        //                          }
        //                          iNewLine++;
        //                      }
        //                      //if(iFirstPlayer > 0) HTMLString.Append("<br>");
        //                      iFirstPlayer++;
        //                      if (!m_SportsOleReader.IsDBNull(2))
        //                      {
        //                          if (m_SportsOleReader.GetInt32(2) != -1)
        //                          {
        //                              HTMLString.Append(m_SportsOleReader.GetInt32(2).ToString() + " " + m_SportsOleReader.GetString(1).Trim() + "<br>");
        //                          }
        //                          else
        //                          {
        //                              HTMLString.Append(m_SportsOleReader.GetString(1).Trim() + "<br>");
        //                          }
        //                      }
        //                      else
        //                      {
        //                          HTMLString.Append(m_SportsOleReader.GetString(1).Trim() + "<br>");
        //                      }
        //                  }
        //                  m_SportsOleReader.Close();
        //                  m_SportsDBMgr.Close();
        //                  if (iNewLine == 0) HTMLString.Append(sEmpty);
        //                  HTMLString.Append("</td>");


        //                  HTMLString.Append("<td>");
        //                  //Guest players
        //                  iPos = -1;
        //                  iNewLine = 0;
        //                  iFirstPlayer = 0;
        //                  SQLString.Remove(0, SQLString.Length);
        //                  SQLString.Append("select IPOS, CPLAYER_NAME, IPLAYER_NO from PLAYERS_INFO where TEAM_ID=");
        //                  SQLString.Append(sGuestID);
        //                  SQLString.Append(" and IROSTER=1 order by IPOS desc, IPLAYER_NO");
        //                  m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
        //                  while (m_SportsOleReader.Read())
        //                  {
        //                      if (m_SportsOleReader.GetInt32(0) != iPos)
        //                      {
        //                          if (iNewLine == 0)
        //                          {
        //                              iPos = m_SportsOleReader.GetInt32(0);
        //                              HTMLString.Append(arrPos[iPos]);
        //                              HTMLString.Append("<br>");
        //                          }
        //                          else
        //                          {
        //                              iPos = m_SportsOleReader.GetInt32(0);
        //                              HTMLString.Append("<br>");
        //                              HTMLString.Append(arrPos[iPos]);
        //                              HTMLString.Append("<br>");
        //                              iFirstPlayer = 0;
        //                          }
        //                          iNewLine++;
        //                      }
        //                      //if(iFirstPlayer > 0) HTMLString.Append("<br>");
        //                      iFirstPlayer++;
        //                      if (!m_SportsOleReader.IsDBNull(2))
        //                      {
        //                          if (m_SportsOleReader.GetInt32(2) != -1)
        //                          {
        //                              HTMLString.Append(m_SportsOleReader.GetInt32(2).ToString() + " " + m_SportsOleReader.GetString(1).Trim() + "<br>");
        //                          }
        //                          else
        //                          {
        //                              HTMLString.Append(m_SportsOleReader.GetString(1).Trim() + "<br>");
        //                          }
        //                      }
        //                      else
        //                      {
        //                          HTMLString.Append(m_SportsOleReader.GetString(1).Trim() + "<br>");
        //                      }
        //                  }
        //                  m_SportsOleReader.Close();
        //                  m_SportsDBMgr.Close();
        //                  if (iNewLine == 0) HTMLString.Append(sEmpty);
        //                  HTMLString.Append("</td></tr>");

        //                  //Remarks - Free text
        //                  string sRemarks = "";
        //                  SQLString.Remove(0, SQLString.Length);
        //                  SQLString.Append("select CREMARKS from ANALYSIS_REMARK_INFO where CACT='U' and IMATCH_CNT=");
        //                  SQLString.Append(sMatchCount);
        //                  m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
        //                  while (m_SportsOleReader.Read())
        //                  {
        //                      if (!m_SportsOleReader.IsDBNull(0))
        //                      {
        //                          sRemarks = m_SportsOleReader.GetString(0).Trim();
        //                      }
        //                  }
        //                  m_SportsOleReader.Close();
        //                  m_SportsDBMgr.Close();
        //                  HTMLString.Append("<tr style=\"background-color:#5F9EA0\" align=\"left\"><th colspan=3><font color=\"#F0F8FF\">附加訊息: (最多300個位元)</font></th></tr><tr align=\"center\"><td colspan=3><textarea name=\"remarks\" rows=12 cols=20 onChange=\"checkRemarkByte()\">");
        //                  HTMLString.Append(sRemarks.Replace(DBCR, PAGECR));
        //                  HTMLString.Append("</textarea></td></tr><input type=\"hidden\" name=\"matchcount\" value=\"");
        //                  HTMLString.Append(sMatchCount);
        //                  HTMLString.Append("\">");

        //                  return HTMLString.ToString();
        //              }
        //              catch (Exception ex)
        //              {
        //                  m_SportsLog.FilePath = ConfigurationManager.AppSettings["errlog"];
        //                  m_SportsLog.SetFileName(0, LOGFILESUFFIX);
        //                  m_SportsLog.Open();
        //                  m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " AnalysisModify.cs.GetMatches(): " + ex.ToString());
        //                  m_SportsLog.Close();
        //                  HTMLString.Remove(0, HTMLString.Length);
        //                  HTMLString.Append(ConfigurationManager.AppSettings["accessErrorMsg"]);
        //              }
        //          }

        //          return HTMLString.ToString();

        //}

        public int Modify()
        {
            char[] delimiter = new char[] { ',' };
            int iWeather, iExisted = 0, iIdx = 0;
            const int iDefaultFlag = 0;
            const string sDefaultAction = "U";
            string sMatchCount, sVenue, sTemperature, sRemarks;
            string[] arrMonth, arrYear, arrLeagueAlias, arrFields, arrHostScore, arrGuestScore;

            sMatchCount = HttpContext.Current.Request.Form["matchcount"];
            sVenue = HttpContext.Current.Request.Form["venue"].Trim();
            sTemperature = HttpContext.Current.Request.Form["temperature"].Trim();
            iWeather = Convert.ToInt32(HttpContext.Current.Request.Form["weatherstatus"]);
            arrMonth = HttpContext.Current.Request.Form["MatchMonth"].Split(delimiter);
            arrYear = HttpContext.Current.Request.Form["MatchYear"].Split(delimiter);
            arrLeagueAlias = HttpContext.Current.Request.Form["leaguealias"].Split(delimiter);
            arrFields = HttpContext.Current.Request.Form["fields"].Split(delimiter);
            arrHostScore = HttpContext.Current.Request.Form["hostscore"].Split(delimiter);
            arrGuestScore = HttpContext.Current.Request.Form["guestscore"].Split(delimiter);
            sRemarks = HttpContext.Current.Request.Form["remarks"].Trim();
            sRemarks = sRemarks.Replace(PAGECR, DBCR);

            try
            {
                //update background information
                SQLString.Remove(0, SQLString.Length);
                SQLString.Append("select count(IMATCH_CNT) from ANALYSIS_BG_INFO where IMATCH_CNT=");
                SQLString.Append(sMatchCount);
                iExisted = m_SportsDBMgrFb.ExecuteScalar(SQLString.ToString());
                m_SportsDBMgrFb.Close();

                SQLString.Remove(0, SQLString.Length);
                if (iExisted > 0)
                {   //update existing record: ANALYSIS_BG_INFO
                    SQLString.Append("update ANALYSIS_BG_INFO set CMATCH_VENUE='");
                    SQLString.Append(sVenue);
                    SQLString.Append("', CTEMPERATURE='");
                    SQLString.Append(sTemperature);
                    SQLString.Append("', IWEATHER_STATUS=");
                    SQLString.Append(iWeather.ToString());
                    SQLString.Append(", CACT='U',CTIMESTAMP='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "' where IMATCH_CNT=");
                    SQLString.Append(sMatchCount);
                }
                else
                {   //insert a new record: ANALYSIS_BG_INFO
                    SQLString.Append("insert into ANALYSIS_BG_INFO values(");
                    SQLString.Append(sMatchCount);
                    SQLString.Append(",'");
                    SQLString.Append(sVenue);
                    SQLString.Append("','");
                    SQLString.Append(sTemperature);
                    SQLString.Append("',");
                    SQLString.Append(iWeather.ToString());
                    SQLString.Append(",'");
                    SQLString.Append(sDefaultAction);
                    SQLString.Append("',");
                    SQLString.Append(iDefaultFlag.ToString());
                    SQLString.Append(",");
                    SQLString.Append(iDefaultFlag.ToString());
                    SQLString.Append(",");
                    SQLString.Append(iDefaultFlag.ToString());
                    SQLString.Append(",'");
                    SQLString.Append(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                    SQLString.Append("')");
                    iExisted = 1;
                }
                m_SportsDBMgrFb.ExecuteNonQuery(SQLString.ToString());
                m_SportsDBMgrFb.Close();

                JC_SoccerWeb.Common.Files.CicsWriteLog(DateTime.Now.ToString("HH:mm:ss ") + "Sql: " + SQLString.ToString());
                //3/28/2019
				//update match history information: delete first, and then insert
				//SQLString.Remove(0,SQLString.Length);
				//SQLString.Append("delete from ANALYSIS_HISTORY_INFO where IMATCH_CNT=");
				//SQLString.Append(sMatchCount);
				//m_SportsDBMgrFb.ExecuteNonQuery(SQLString.ToString());
                //m_SportsDBMgrFb.Close();
				for(iIdx=0;iIdx<HISTORYCOUNT;iIdx++) {
					if(!(arrMonth[iIdx].Equals("0") || arrYear[iIdx].Equals("0"))) {
						if(arrHostScore[iIdx].Equals("")) arrHostScore[iIdx] = "0";
						if(arrGuestScore[iIdx].Equals("")) arrGuestScore[iIdx] = "0";
                        using (FbConnection connection2 = new FbConnection(AppFlag.ScoutsDBConn))
                        {
                            connection2.Open();
                            string sql = "UPDATE OR INSERT INTO ANALYSIS_HISTORY_INFO (IMATCH_CNT, IREC, CACT, EVENTID, HOME_ID, GUEST_ID, START_DATE, LEAGUEALIASID, LEAGUEALIAS,IMATCHYEAR, IMATCHMONTH,  IMATCHSTATUS, IHOSTSCORE, IGUESTSCORE, CTIMESTAMP) VALUES" +
                                " ('"+ sMatchCount + "', '"+ iIdx.ToString() + "', 'U', '-1', '-1', '-1',null, '-1', '"+ arrLeagueAlias[iIdx] + "', '"+ arrYear[iIdx] + "', '"+ arrMonth[iIdx] + "', '"+ arrFields[iIdx] + "', '"+ arrHostScore[iIdx] + "', '"+ arrGuestScore[iIdx] + "', '"+ DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "') MATCHING  (IMATCH_CNT, IREC)";
                            using (FbCommand cmd2 = new FbCommand(sql, connection2))
                            {
                                int id = Convert.ToInt32(cmd2.ExecuteScalar());
                                if (id > -1)
                                {
                                     JC_SoccerWeb.Common.Files.CicsWriteLog(" [Success] Insert/Update ANALYSIS_HISTORY_INFO "+ sMatchCount + " :" + " " + arrLeagueAlias[iIdx].Trim());
                                }
                            }
                            connection2.Close();
                        }
                        //SQLString.Remove(0,SQLString.Length);
                        //SQLString.Append("insert into ANALYSIS_HISTORY_INFO values(");
                        //SQLString.Append(sMatchCount);
                        //SQLString.Append(",");
                        //SQLString.Append(iIdx.ToString());
                        //SQLString.Append(",'");
                        //SQLString.Append(sDefaultAction);
                        //SQLString.Append("','");
                        //SQLString.Append(arrLeagueAlias[iIdx]);
                        //SQLString.Append("',");
                        //SQLString.Append(arrMonth[iIdx]);
                        //SQLString.Append(",");
                        //SQLString.Append(arrYear[iIdx]);
                        //SQLString.Append(",");
                        //SQLString.Append(arrFields[iIdx]);
                        //SQLString.Append(",");
                        //SQLString.Append(arrHostScore[iIdx]);
                        //SQLString.Append(",");
                        //SQLString.Append(arrGuestScore[iIdx]);
                        //SQLString.Append(")");
                        //                  m_SportsDBMgrFb.ExecuteNonQuery(SQLString.ToString());
                        //                  m_SportsDBMgrFb.Close();
                    }	else {
						break;
					}
				}
                
                //update analysis remarks information: delete first, and then insert
                SQLString.Remove(0, SQLString.Length);
                SQLString.Append("delete from ANALYSIS_REMARK_INFO where IMATCH_CNT=");
                SQLString.Append(sMatchCount);
                m_SportsDBMgrFb.ExecuteNonQuery(SQLString.ToString());
                m_SportsDBMgrFb.Close();

                SQLString.Remove(0, SQLString.Length);
                SQLString.Append("insert into ANALYSIS_REMARK_INFO values(");
                SQLString.Append(sMatchCount);
                SQLString.Append(",'");
                SQLString.Append(sDefaultAction);
                SQLString.Append("','");
                SQLString.Append(sRemarks);
                SQLString.Append("','");
                SQLString.Append(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                SQLString.Append("')");
                m_SportsDBMgrFb.ExecuteNonQuery(SQLString.ToString());
                m_SportsDBMgrFb.Close();

                //write log
                m_SportsLog.FilePath = ConfigurationManager.AppSettings["eventlog"];
                m_SportsLog.SetFileName(0, LOGFILESUFFIX);
                m_SportsLog.Open();
                if (iExisted > 0) m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " AnalysisModify.cs: Update " + iExisted.ToString() + " records (" + HttpContext.Current.Session["user_name"] + ")");
                else m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " AnalysisModify.cs: Add a new record (" + HttpContext.Current.Session["user_name"] + ")");
                m_SportsLog.Close();

                JC_SoccerWeb.Common.Files.CicsWriteLog(DateTime.Now.ToString("HH:mm:ss ") + "Sql: " + SQLString.ToString());
                if (sMatchCount != "")
                {
                    JC_SoccerWeb.Common.Files.CicsWriteLog(DateTime.Now.ToString("HH:mm:ss ") + "Send " + sMatchCount + " ANALYSISHISTORYS/11 ");
                    string sReslut = ConfigManager.SendWinMsg(sMatchCount, "ANALYSISHISTORYS/11");
                    JC_SoccerWeb.Common.Files.CicsWriteLog(DateTime.Now.ToString("HH:mm:ss ") + "Send " + sMatchCount + " ANALYSISBGREMARK/10 "); 
                     sReslut = ConfigManager.SendWinMsg(sMatchCount,"ANALYSISBGREMARK/10"  );
                  //  if (sReslut != "Done") JC_SoccerWeb.Common.Files.CicsWriteLog(DateTime.Now.ToString("HH:mm:ss ")+ "[Failure] Send " + sMatchCount + " ANALYSISBGREMARK-10, " + sReslut);
                }  
            }
            catch (Exception ex)
            {
                iExisted = -1;
                m_SportsLog.FilePath = ConfigurationManager.AppSettings["errlog"];
                m_SportsLog.SetFileName(0, LOGFILESUFFIX);
                m_SportsLog.Open();
                m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " AnalysisModify.cs.Modify(): " + ex.ToString());
                m_SportsLog.Close();
            }

            return iExisted;
        }
    }
}