/*
Objective:
Modify and send selected match recent information

Last updated:
19 Feb 2004, Chapman Choi
Add four additional field into LOG_ANALYSISRECENT (MATCHDATE, MATCHTIME, MATCHFIELD, HOSTHANDICAP)

C#.NET complier statement:
csc /t:library /out:..\bin\AnalysisRecent.dll /r:..\bin\DBManager.dll;..\bin\Files.dll;..\bin\MessageClient.dll;..\bin\SportsMessage.dll AnalysisRecent.cs
*/

using FirebirdSql.Data.FirebirdClient;
using JC_SoccerWeb.Common;
using System;
using System.Collections;
using System.Configuration;
using System.Data;
//using System.Data.OleDb;
using System.Reflection;
using System.Text;
using System.Web;
using TDL.DB;
using TDL.IO;
using TDL.Message;

 
namespace SportsUtil {
	public class AnalysisRecent {
		const int TOTALRECENTCOUNT = 5;
		const string LOGFILESUFFIX = "log";
		string[] arrFields;
        //OleDbDataReader m_SportsOleReader;
        //DBManager m_SportsDBMgr;
        TDL.IO.Files m_SportsLog;
		Encoding m_Big5Encoded;
		StringBuilder SQLString;
        public string m_Title = "";
        DBManagerFB m_SportsDBMgrFb;
        FbDataReader m_SportsOleReaderFb;

        public AnalysisRecent(string Connection) {
			//m_SportsDBMgrFb = new DBManager();
			//m_SportsDBMgrFb.ConnectionString = Connection;
			m_SportsLog = new TDL.IO.Files();
			m_Big5Encoded = Encoding.GetEncoding(950);
			SQLString = new StringBuilder();
			arrFields = (string[])HttpContext.Current.Application["fieldItemsArray"];
            m_SportsDBMgrFb = new DBManagerFB();
            m_SportsDBMgrFb.ConnectionString = JC_SoccerWeb.Common.AppFlag.ScoutsDBConn;
        }

		public string GetRecent() {
			int iItemIdx = 0;
			int iRecordCount = 0;
            ////string sMatchCnt = "";
            string sEventID = "";
            string sLeague = "";
			string sHost = "";
			string sGuest = "";
			string sMatchDate = "";
			string sMatchTime = "";
			string sMatchField = "";
			string sHostHandicap = "";
			StringBuilder HTMLString = new StringBuilder();
			StringBuilder TempHTMLString = new StringBuilder();
			ArrayList HostRecentHTMLList = new ArrayList(5);
			ArrayList GuestRecentHTMLList = new ArrayList(5);
            //for (int xx = 1; xx < 100; xx++)
            //{
                try
                {

                    ////  sMatchCnt = HttpContext.Current.Request.QueryString["matchcnt"] == null ? xx.ToString() : HttpContext.Current.Request.QueryString["matchcnt"];
                    sEventID = (HttpContext.Current.Request.QueryString["eventid"] == null) ? "" : HttpContext.Current.Request.QueryString["eventid"].Trim();

                    //get league, host, guest from gameinfo
                    SQLString.Remove(0, SQLString.Length);
                //// SQLString.Append("select leaglong, host, guest, MATCHDATE, MATCHTIME, FIELD, HOST_HANDI from gameinfo where match_cnt=");
                SQLString.Append("select r.CLEAGUE_OUTPUT_NAME,   r.HKJCHOSTNAME_CN,  r.HKJCGUESTNAME_CN, cast(cast(r.CMATCHDATETIME as date) as varchar(10)), cast(r.CMATCHDATETIME as time),");
                SQLString.Append("'H', '1'  from EMATCHES r  where r.ematchid = ");
                ////SQLString.Append(sMatchCnt);
                 SQLString.Append(sEventID);
                    m_SportsOleReaderFb = m_SportsDBMgrFb.ExecuteQuery(SQLString.ToString());
                    if (m_SportsOleReaderFb.Read())
                    {
                        sLeague = m_SportsOleReaderFb.GetString(0).Trim();
                        sHost = m_SportsOleReaderFb.GetString(1).Trim();
                        sGuest = m_SportsOleReaderFb.GetString(2).Trim();
                        sMatchDate = m_SportsOleReaderFb.GetString(3).Trim();
                        sMatchTime = m_SportsOleReaderFb.GetString(4).Trim();
                        sMatchField = m_SportsOleReaderFb.GetString(5).Trim();
                        sHostHandicap = m_SportsOleReaderFb.GetString(6).Trim();
                    }
                  m_SportsDBMgrFb.Close();
                  m_SportsOleReaderFb.Close();
                  m_Title = sHost + "/" + sGuest + "-近績";
                HTMLString.Append("<tr style=\"background-color:#FFDAB9\"><th><select name=\"Action\" onChange=\"OnActionChanged()\"><option value=\"U\">更新<option value=\"D\">刪除&nbsp;<input type=\"hidden\" name=\"league\" value=\"");
                    HTMLString.Append(sLeague);
                    HTMLString.Append("\">");
                    HTMLString.Append(sLeague);
                    HTMLString.Append("</th><th><input type=\"hidden\" name=\"host\" value=\"");
                    HTMLString.Append(sHost);
                    HTMLString.Append("\">");
                    HTMLString.Append(sHost);
                    HTMLString.Append("</th><th><input type=\"hidden\" name=\"guest\" value=\"");
                    HTMLString.Append(sGuest);
                    HTMLString.Append("\">");
                    HTMLString.Append(sGuest);
                ////  HTMLString.Append("<input type=\"hidden\" name=\"matchcount\" value=\"");
                HTMLString.Append("<input type=\"hidden\" name=\"eventid\" value=\"");
                ////HTMLString.Append(sMatchCnt);
                HTMLString.Append(sEventID);
                    HTMLString.Append("\"><input type=\"hidden\" name=\"matchdate\" value=\"");
                    HTMLString.Append(sMatchDate);
                    HTMLString.Append("\"><input type=\"hidden\" name=\"matchtime\" value=\"");
                    HTMLString.Append(sMatchTime);
                    HTMLString.Append("\"><input type=\"hidden\" name=\"matchfield\" value=\"");
                    HTMLString.Append(sMatchField);
                    HTMLString.Append("\"><input type=\"hidden\" name=\"hosthandicap\" value=\"");
                    HTMLString.Append(sHostHandicap);
                    HTMLString.Append("\"></th></tr><tr style=\"background-color:#FFE4B5\"><th>下仗</th>");

                    //Next game for Host
                    HTMLString.Append("<th><select name=\"HNextMatchField\">");
                    SQLString.Remove(0, SQLString.Length);
                ///   SQLString.Append("select IMATCHSTATUS, CCHALLENGER, CLEAGUEALIAS from ANALYSIS_RECENT_INFO where CACT='U' AND CTEAMFLAG='H' AND IHOSTSCORE= -1 AND IGUESTSCORE =-1 AND IMATCH_CNT=");
                SQLString.Append("select  IMATCHSTATUS, CCHALLENGER, LEAGUEALIAS, EVENTID  from ANALYSIS_RECENT_INFO where CACT='U' AND CTEAMFLAG='H' AND IHOSTSCORE= -1 AND IGUESTSCORE =-1 AND IMATCH_CNT=");
                ////SQLString.Append(sMatchCnt);
                SQLString.Append(sEventID);
                    m_SportsOleReaderFb = m_SportsDBMgrFb.ExecuteQuery(SQLString.ToString());
                if (m_SportsOleReaderFb.Read())
                {
                    //Match Field
                    if (!m_SportsOleReaderFb.IsDBNull(0))
                    {
                        if (m_SportsOleReaderFb.GetInt32(0) != -1)
                        {
                            HTMLString.Append("<option value=");
                            HTMLString.Append(m_SportsOleReaderFb.GetInt32(0).ToString());
                            HTMLString.Append(" selected>");
                            HTMLString.Append(arrFields[m_SportsOleReaderFb.GetInt32(0)]);
                            for (iItemIdx = 0; iItemIdx < arrFields.Length; iItemIdx++)
                            {
                                if (iItemIdx != m_SportsOleReaderFb.GetInt32(0))
                                {
                                    HTMLString.Append("<option value=");
                                    HTMLString.Append(iItemIdx.ToString());
                                    HTMLString.Append(">");
                                    HTMLString.Append(arrFields[iItemIdx]);
                                }
                            }
                            HTMLString.Append("<option value=\"-1\">不適用");
                            HTMLString.Append("</select>");
                        }
                        else
                        {
                            HTMLString.Append("<option value=\"-1\" selected>不適用");
                            for (iItemIdx = 0; iItemIdx < arrFields.Length; iItemIdx++)
                            {
                                HTMLString.Append("<option value=");
                                HTMLString.Append(iItemIdx.ToString());
                                HTMLString.Append(">");
                                HTMLString.Append(arrFields[iItemIdx]);
                            }
                            HTMLString.Append("</select>");
                        }
                    }
                    else
                    {
                        HTMLString.Append("<option value=\"-1\" selected>不適用");
                        for (iItemIdx = 0; iItemIdx < arrFields.Length; iItemIdx++)
                        {
                            HTMLString.Append("<option value=");
                            HTMLString.Append(iItemIdx.ToString());
                            HTMLString.Append(">");
                            HTMLString.Append(arrFields[iItemIdx]);
                        }
                        HTMLString.Append("</select>");
                    }

                    //Challenger
                    HTMLString.Append("&nbsp;隊伍:<input name=\"HNextChallenger\" maxlength=\"4\" size=\"3\" value=\"");
                    if (!m_SportsOleReaderFb.IsDBNull(1))
                    {
                        if (!m_SportsOleReaderFb.GetString(1).Trim().Equals("-1"))
                        {
                            HTMLString.Append(m_SportsOleReaderFb.GetString(1).Trim());
                        }
                    }
                    HTMLString.Append("\" onChange=\"checkHNextChallengerLength()\">");

                    //Match League
                    HTMLString.Append("&nbsp;聯賽:<input name=\"HNextLeague\" maxlength=\"4\" size=\"3\" value=\"");
                    if (!m_SportsOleReaderFb.IsDBNull(2))
                    {
                        if (!m_SportsOleReaderFb.GetString(2).Trim().Equals("-1"))
                        {
                            HTMLString.Append(m_SportsOleReaderFb.GetString(2).Trim());
                        }
                    }
                    HTMLString.Append("\" onChange=\"checkHNextLeagueLength()\">");
                    //id
                    if (!m_SportsOleReaderFb.IsDBNull(3))
                    {
                        if (!m_SportsOleReaderFb.GetInt32(3).Equals(-1))
                        {
                            HTMLString.Append("<input  type=\"hidden\" name=\"matchEventid1\"  size=\"3\" value=\"");
                            HTMLString.Append(m_SportsOleReaderFb.GetInt32(3));
                            HTMLString.Append("\">");
                        }
                    }
                }
                else
                {
                    HTMLString.Append("<option value=\"-1\" selected>不適用");
                    for (iItemIdx = 0; iItemIdx < arrFields.Length; iItemIdx++)
                    {
                        HTMLString.Append("<option value=");
                        HTMLString.Append(iItemIdx.ToString());
                        HTMLString.Append(">");
                        HTMLString.Append(arrFields[iItemIdx]);
                    }
                    HTMLString.Append("</select>");
                    HTMLString.Append("&nbsp;隊伍:<input name=\"HNextChallenger\" maxlength=\"4\" size=\"3\" value=\"\" onChange=\"checkHNextChallengerLength()\">");
                    HTMLString.Append("&nbsp;聯賽:<input name=\"HNextLeague\" maxlength=\"4\" size=\"3\" value=\"\" onChange=\"checkHNextLeagueLength()\">");
                    HTMLString.Append("<input type=\"hidden\" name=\"matchEventid1\"  size=\"3\" value=\"\" \">");
                }
                    m_SportsOleReaderFb.Close();
                    m_SportsDBMgrFb.Close();
                    HTMLString.Append("</th>");

                    //Next game for Guest
                    HTMLString.Append("<th><select name=\"GNextMatchField\">");
                    SQLString.Remove(0, SQLString.Length);
                //  SQLString.Append("select IMATCHSTATUS, CCHALLENGER, CLEAGUEALIAS from ANALYSIS_RECENT_INFO where CACT='U' AND CTEAMFLAG='G' AND IHOSTSCORE= -1 AND IGUESTSCORE =-1 AND IMATCH_CNT=");
                SQLString.Append("select IMATCHSTATUS, CCHALLENGER, LEAGUEALIAS,EVENTID from ANALYSIS_RECENT_INFO where CACT='U' AND CTEAMFLAG='G' AND IHOSTSCORE= -1 AND IGUESTSCORE =-1 AND IMATCH_CNT=");
                ////  SQLString.Append(sMatchCnt);
                SQLString.Append(sEventID);
                    m_SportsOleReaderFb = m_SportsDBMgrFb.ExecuteQuery(SQLString.ToString());
                    if (m_SportsOleReaderFb.Read())
                    {
                        //Match Field
                        if (!m_SportsOleReaderFb.IsDBNull(0))
                        {
                            if (m_SportsOleReaderFb.GetInt32(0) != -1)
                            {
                                HTMLString.Append("<option value=");
                                HTMLString.Append(m_SportsOleReaderFb.GetInt32(0).ToString());
                                HTMLString.Append(" selected>");
                                HTMLString.Append(arrFields[m_SportsOleReaderFb.GetInt32(0)]);
                                for (iItemIdx = 0; iItemIdx < arrFields.Length; iItemIdx++)
                                {
                                    if (iItemIdx != m_SportsOleReaderFb.GetInt32(0))
                                    {
                                        HTMLString.Append("<option value=");
                                        HTMLString.Append(iItemIdx.ToString());
                                        HTMLString.Append(">");
                                        HTMLString.Append(arrFields[iItemIdx]);
                                    }
                                }
                                HTMLString.Append("<option value=\"-1\">不適用");
                                HTMLString.Append("</select>");
                            }
                            else
                            {
                                HTMLString.Append("<option value=\"-1\" selected>不適用");
                                for (iItemIdx = 0; iItemIdx < arrFields.Length; iItemIdx++)
                                {
                                    HTMLString.Append("<option value=");
                                    HTMLString.Append(iItemIdx.ToString());
                                    HTMLString.Append(">");
                                    HTMLString.Append(arrFields[iItemIdx]);
                                }
                                HTMLString.Append("</select>");
                            }
                        }
                        else
                        {
                            HTMLString.Append("<option value=\"-1\" selected>不適用");
                            for (iItemIdx = 0; iItemIdx < arrFields.Length; iItemIdx++)
                            {
                                HTMLString.Append("<option value=");
                                HTMLString.Append(iItemIdx.ToString());
                                HTMLString.Append(">");
                                HTMLString.Append(arrFields[iItemIdx]);
                            }
                            HTMLString.Append("</select>");
                        }

                        //Challenger
                        HTMLString.Append("&nbsp;隊伍:<input name=\"GNextChallenger\" maxlength=\"4\" size=\"3\" value=\"");
                        if (!m_SportsOleReaderFb.IsDBNull(1))
                        {
                            if (!m_SportsOleReaderFb.GetString(1).Trim().Equals("-1"))
                            {
                                HTMLString.Append(m_SportsOleReaderFb.GetString(1).Trim());
                            }
                        }
                        HTMLString.Append("\" onChange=\"checkGNextChallengerLength()\">");

                        //Match League
                        HTMLString.Append("&nbsp;聯賽:<input name=\"GNextLeague\" maxlength=\"4\" size=\"3\" value=\"");
                        if (!m_SportsOleReaderFb.IsDBNull(2))
                        {
                            if (!m_SportsOleReaderFb.GetString(2).Trim().Equals("-1"))
                            {
                                HTMLString.Append(m_SportsOleReaderFb.GetString(2).Trim());
                            }
                        }
                        HTMLString.Append("\" onChange=\"checkGNextLeagueLength()\">");

                    //id
                    if (!m_SportsOleReaderFb.IsDBNull(3))
                    {
                        if (!m_SportsOleReaderFb.GetInt32(3).Equals(-1))
                        {
                            HTMLString.Append("<input type=\"hidden\"  name=\"matchEventid2\"  size=\"3\" value=\"");
                            HTMLString.Append(m_SportsOleReaderFb.GetInt32(3));
                            HTMLString.Append("\">");
                        }
                    }
                }
                    else
                    {
                        HTMLString.Append("<option value=\"-1\" selected>不適用");
                        for (iItemIdx = 0; iItemIdx < arrFields.Length; iItemIdx++)
                        {
                            HTMLString.Append("<option value=");
                            HTMLString.Append(iItemIdx.ToString());
                            HTMLString.Append(">");
                            HTMLString.Append(arrFields[iItemIdx]);
                        }
                        HTMLString.Append("</select>");
                        HTMLString.Append("&nbsp;隊伍:<input name=\"GNextChallenger\" maxlength=\"4\" size=\"3\" value=\"\" onChange=\"checkGNextChallengerLength()\">");
                        HTMLString.Append("&nbsp;聯賽:<input name=\"GNextLeague\" maxlength=\"4\" size=\"3\" value=\"\" onChange=\"checkGNextLeagueLength()\">");
                    HTMLString.Append("<input type=\"hidden\" name=\"matchEventid2\"  size=\"3\" value=\"\" \">");
                }
                    m_SportsOleReaderFb.Close();
                    m_SportsDBMgrFb.Close();
                    HTMLString.Append("</th>");

                    //Host recent result
                    iRecordCount = 0;
                    SQLString.Remove(0, SQLString.Length);
                    SQLString.Append("select IMATCHSTATUS, IHOSTSCORE, IGUESTSCORE, CCHALLENGER, LEAGUEALIAS ,EVENTID from ANALYSIS_RECENT_INFO where CACT='U' AND CTEAMFLAG='H' AND IHOSTSCORE<>-1 AND IGUESTSCORE<>-1 AND IMATCH_CNT=");
                //// SQLString.Append(sMatchCnt);
                SQLString.Append(sEventID);
                    SQLString.Append(" order by IREC");
                    m_SportsOleReaderFb = m_SportsDBMgrFb.ExecuteQuery(SQLString.ToString());
                    while (m_SportsOleReaderFb.Read())
                    {
                        TempHTMLString.Remove(0, TempHTMLString.Length);
                        TempHTMLString.Append("<th><select name=\"HRecentMatchField\">");
                        TempHTMLString.Append("<option value=");
                        TempHTMLString.Append(m_SportsOleReaderFb.GetInt32(0).ToString());
                        TempHTMLString.Append(" selected>");
                        TempHTMLString.Append(arrFields[m_SportsOleReaderFb.GetInt32(0)]);
                        for (iItemIdx = 0; iItemIdx < arrFields.Length; iItemIdx++)
                        {
                            if (iItemIdx != m_SportsOleReaderFb.GetInt32(0))
                            {
                                TempHTMLString.Append("<option value=");
                                TempHTMLString.Append(iItemIdx.ToString());
                                TempHTMLString.Append(">");
                                TempHTMLString.Append(arrFields[iItemIdx]);
                            }
                        }
                        TempHTMLString.Append("</select>");

                        //Host Score
                        TempHTMLString.Append("<input name=\"HRecentHScore\" maxlength=\"2\" size=\"1\" value=\"");
                        TempHTMLString.Append(m_SportsOleReaderFb.GetInt32(1).ToString());
                        TempHTMLString.Append("\" onChange=\"checkHRecentHScoreFormat(");
                        TempHTMLString.Append(iRecordCount.ToString());
                        TempHTMLString.Append(")\">:");

                        //Guest Score
                        TempHTMLString.Append("<input name=\"HRecentGScore\" maxlength=\"2\" size=\"1\" value=\"");
                        TempHTMLString.Append(m_SportsOleReaderFb.GetInt32(2).ToString());
                        TempHTMLString.Append("\" onChange=\"checkHRecentGScoreFormat(");
                        TempHTMLString.Append(iRecordCount.ToString());
                        TempHTMLString.Append(")\">");

                        //Challenger
                        TempHTMLString.Append("&nbsp;隊伍:<input name=\"HRecentChallenger\" maxlength=\"4\" size=\"3\" value=\"");
                        TempHTMLString.Append(m_SportsOleReaderFb.GetString(3).Trim());
                        TempHTMLString.Append("\" onChange=\"checkHRecentChallengerLength(");
                        TempHTMLString.Append(iRecordCount.ToString());
                        TempHTMLString.Append(")\">");

                        //Match League
                        TempHTMLString.Append("&nbsp;聯賽:<input name=\"HRecentLeague\" maxlength=\"4\" size=\"3\" value=\"");
                        TempHTMLString.Append(m_SportsOleReaderFb.GetString(4).Trim());
                        TempHTMLString.Append("\" onChange=\"checkHRecentLeagueLength(");
                        TempHTMLString.Append(iRecordCount.ToString());
                        TempHTMLString.Append(")\">");

                    //id
                    if (!m_SportsOleReaderFb.IsDBNull(5))
                    {
                        if (!m_SportsOleReaderFb.GetInt32(5).Equals(-1))
                        {
                            TempHTMLString.Append("<input type=\"hidden\"  name=\"matchEventid3\"  size=\"3\" value=\"");
                            TempHTMLString.Append(m_SportsOleReaderFb.GetInt32(5));
                            TempHTMLString.Append("\">");
                        }
                    }

                    TempHTMLString.Append("</th>");
                        HostRecentHTMLList.Add(TempHTMLString.ToString());
                        iRecordCount++;
                    }
                    m_SportsOleReaderFb.Close();
                    m_SportsDBMgrFb.Close();
                    while (iRecordCount < TOTALRECENTCOUNT)
                    {
                        TempHTMLString.Remove(0, TempHTMLString.Length);
                        TempHTMLString.Append("<th><select name=\"HRecentMatchField\">");
                        for (iItemIdx = 0; iItemIdx < arrFields.Length; iItemIdx++)
                        {
                            TempHTMLString.Append("<option value=");
                            TempHTMLString.Append(iItemIdx.ToString());
                            TempHTMLString.Append(">");
                            TempHTMLString.Append(arrFields[iItemIdx]);
                        }
                        TempHTMLString.Append("</select><input name=\"HRecentHScore\" maxlength=\"2\" size=\"1\" value=\"\" onChange=\"checkHRecentHScoreFormat(");
                        TempHTMLString.Append(iRecordCount.ToString());
                        TempHTMLString.Append(")\">:<input name=\"HRecentGScore\" maxlength=\"2\" size=\"1\" value=\"\" onChange=\"checkHRecentGScoreFormat(");
                        TempHTMLString.Append(iRecordCount.ToString());
                        TempHTMLString.Append(")\">&nbsp;隊伍:<input name=\"HRecentChallenger\" maxlength=\"4\" size=\"3\" value=\"\" onChange=\"checkHRecentChallengerLength(");
                        TempHTMLString.Append(iRecordCount.ToString());
                        TempHTMLString.Append(")\">&nbsp;聯賽:<input name=\"HRecentLeague\" maxlength=\"4\" size=\"3\" value=\"\" onChange=\"checkHRecentLeagueLength(");
                        TempHTMLString.Append(iRecordCount.ToString());
                    TempHTMLString.Append(")\"><input type=\"hidden\" name=\"matchEventid3\"  size=\"3\" value=\"\" \">");
                       TempHTMLString.Append("</th>");
                        HostRecentHTMLList.Add(TempHTMLString.ToString());
                        iRecordCount++;
                    }

                    //Guest recent result
                    iRecordCount = 0;
                    SQLString.Remove(0, SQLString.Length);
                    SQLString.Append("select IMATCHSTATUS, IHOSTSCORE, IGUESTSCORE, CCHALLENGER, LEAGUEALIAS,EVENTID  from ANALYSIS_RECENT_INFO where CACT='U' AND CTEAMFLAG='G' AND IHOSTSCORE<>-1 AND IGUESTSCORE<>-1 AND IMATCH_CNT=");
                ////  SQLString.Append(sMatchCnt);
                SQLString.Append(sEventID);
                    SQLString.Append(" order by IREC");
                    m_SportsOleReaderFb = m_SportsDBMgrFb.ExecuteQuery(SQLString.ToString());
                    while (m_SportsOleReaderFb.Read())
                    {
                        TempHTMLString.Remove(0, TempHTMLString.Length);
                        TempHTMLString.Append("<th><select name=\"GRecentMatchField\">");
                        TempHTMLString.Append("<option value=");
                        TempHTMLString.Append(m_SportsOleReaderFb.GetInt32(0).ToString());
                        TempHTMLString.Append(" selected>");
                        TempHTMLString.Append(arrFields[m_SportsOleReaderFb.GetInt32(0)]);
                        for (iItemIdx = 0; iItemIdx < arrFields.Length; iItemIdx++)
                        {
                            if (iItemIdx != m_SportsOleReaderFb.GetInt32(0))
                            {
                                TempHTMLString.Append("<option value=");
                                TempHTMLString.Append(iItemIdx.ToString());
                                TempHTMLString.Append(">");
                                TempHTMLString.Append(arrFields[iItemIdx]);
                            }
                        }
                        TempHTMLString.Append("</select>");

                        //Host Score
                        TempHTMLString.Append("<input name=\"GRecentHScore\" maxlength=\"2\" size=\"1\" value=\"");
                        TempHTMLString.Append(m_SportsOleReaderFb.GetInt32(1).ToString());
                        TempHTMLString.Append("\" onChange=\"checkGRecentHScoreFormat(");
                        TempHTMLString.Append(iRecordCount.ToString());
                        TempHTMLString.Append(")\">:");

                        //Guest Score
                        TempHTMLString.Append("<input name=\"GRecentGScore\" maxlength=\"2\" size=\"1\" value=\"");
                        TempHTMLString.Append(m_SportsOleReaderFb.GetInt32(2).ToString());
                        TempHTMLString.Append("\" onChange=\"checkGRecentGScoreFormat(");
                        TempHTMLString.Append(iRecordCount.ToString());
                        TempHTMLString.Append(")\">");

                        //Challenger
                        TempHTMLString.Append("&nbsp;隊伍:<input name=\"GRecentChallenger\" maxlength=\"4\" size=\"3\" value=\"");
                        TempHTMLString.Append(m_SportsOleReaderFb.GetString(3).Trim());
                        TempHTMLString.Append("\" onChange=\"checkGRecentChallengerLength(");
                        TempHTMLString.Append(iRecordCount.ToString());
                        TempHTMLString.Append(")\">");

                        //Match League
                        TempHTMLString.Append("&nbsp;聯賽:<input name=\"GRecentLeague\" maxlength=\"4\" size=\"3\" value=\"");
                        TempHTMLString.Append(m_SportsOleReaderFb.GetString(4).Trim());
                        TempHTMLString.Append("\" onChange=\"checkGRecentLeagueLength(");
                        TempHTMLString.Append(iRecordCount.ToString());
                        TempHTMLString.Append(")\">");
                    //id
                    if (!m_SportsOleReaderFb.IsDBNull(5))
                    {
                        if (!m_SportsOleReaderFb.GetInt32(5).Equals(-1))
                        {
                            TempHTMLString.Append("<input type=\"hidden\" name=\"matchEventid4\"  size=\"3\" value=\"");
                            TempHTMLString.Append(m_SportsOleReaderFb.GetInt32(5));
                            TempHTMLString.Append("\">");
                        }
                    }
                    TempHTMLString.Append("</th>");
                        GuestRecentHTMLList.Add(TempHTMLString.ToString());
                        iRecordCount++;
                    }
                    m_SportsOleReaderFb.Close();
                    m_SportsDBMgrFb.Close();
                    m_SportsDBMgrFb.Dispose();
                    while (iRecordCount < TOTALRECENTCOUNT)
                    {
                        TempHTMLString.Remove(0, TempHTMLString.Length);
                        TempHTMLString.Append("<th><select name=\"GRecentMatchField\">");
                        for (iItemIdx = 0; iItemIdx < arrFields.Length; iItemIdx++)
                        {
                            TempHTMLString.Append("<option value=");
                            TempHTMLString.Append(iItemIdx.ToString());
                            TempHTMLString.Append(">");
                            TempHTMLString.Append(arrFields[iItemIdx]);
                        }
                        TempHTMLString.Append("</select><input name=\"GRecentHScore\" maxlength=\"2\" size=\"1\" value=\"\" onChange=\"checkGRecentHScoreFormat(");
                        TempHTMLString.Append(iRecordCount.ToString());
                        TempHTMLString.Append(")\">:<input name=\"GRecentGScore\" maxlength=\"2\" size=\"1\" value=\"\" onChange=\"checkGRecentGScoreFormat(");
                        TempHTMLString.Append(iRecordCount.ToString());
                        TempHTMLString.Append(")\">&nbsp;隊伍:<input name=\"GRecentChallenger\" maxlength=\"4\" size=\"3\" value=\"\" onChange=\"checkGRecentChallengerLength(");
                        TempHTMLString.Append(iRecordCount.ToString());
                        TempHTMLString.Append(")\">&nbsp;聯賽:<input name=\"GRecentLeague\" maxlength=\"4\" size=\"3\" value=\"\" onChange=\"checkGRecentLeagueLength(");
                        TempHTMLString.Append(iRecordCount.ToString());
                    TempHTMLString.Append(")\"><input type=\"hidden\" name=\"matchEventid4\"  size=\"3\" value=\"\" \">");
                    TempHTMLString.Append("</th>");
                    GuestRecentHTMLList.Add(TempHTMLString.ToString());
                        iRecordCount++;
                    }

                    for (iItemIdx = 0; iItemIdx < TOTALRECENTCOUNT; iItemIdx++)
                    {
                        HTMLString.Append("<tr>");
                        if (iItemIdx == 0)
                        {
                            HTMLString.Append("<th rowspan=\"");
                            HTMLString.Append(TOTALRECENTCOUNT.ToString());
                            HTMLString.Append("\">近績</th>");
                        }
                        HTMLString.Append(HostRecentHTMLList[iItemIdx]);
                        HTMLString.Append(GuestRecentHTMLList[iItemIdx]);
                        HTMLString.Append("</tr>");
                    }
                    //return HTMLString.ToString();
                }
                catch (Exception ex)
                {
                    m_SportsLog.FilePath = ConfigurationManager.AppSettings["errlog"];
                    m_SportsLog.SetFileName(0, LOGFILESUFFIX);
                    m_SportsLog.Open();
                    m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " AnalysisRecent.cs.GetRecent(): " + ex.ToString());
                    m_SportsLog.Close();
                    HTMLString.Remove(0, HTMLString.Length);
                    HTMLString.Append(ConfigurationManager.AppSettings["accessErrorMsg"]);
                }
            //}

			return HTMLString.ToString();
		}
		
		public int SaveRecord() {
			int iRecNo = 0;
			int iItemIdx = 0;
			//int iINIIdx = 0;
			char[] delimiter = new char[] {','};
            ////string sMatchCnt;
            string sEventID;
            string sLeague;
			string sHost;
			string sGuest;
			string sMatchDate;
			string sMatchTime;
			string sMatchField;
			string sHostHandicap;
			string sHNextMatchField;
			string sHNextChallenger;
			string sHNextLeague;
			string sGNextMatchField;
			string sGNextChallenger;
			string sGNextLeague;
			string[] arrHRecentMatchField;
			string[] arrHRecentHScore;
			string[] arrHRecentGScore;
			string[] arrHRecentChallenger;
			string[] arrHRecentLeague;
			string[] arrGRecentMatchField;
			string[] arrGRecentHScore;
			string[] arrGRecentGScore;
			string[] arrGRecentChallenger;
			string[] arrGRecentLeague;

            string[] arrHEventid;
            string[] arrGEventid;
            string sRHEventid;
            string sRGEventid;
            //string sHEventid;
            //string sGEventid;

            ////sMatchCnt = HttpContext.Current.Request.Form["matchcount"];
            sEventID = HttpContext.Current.Request.Form["eventid"];
            sRHEventid = HttpContext.Current.Request.Form["matchEventid1"] == null || HttpContext.Current.Request.Form["matchEventid1"]=="" ? "-1" : HttpContext.Current.Request.Form["matchEventid1"];
            sRGEventid = HttpContext.Current.Request.Form["matchEventid2"] == null || HttpContext.Current.Request.Form["matchEventid2"] == "" ? "-1" : HttpContext.Current.Request.Form["matchEventid2"];
            //sHEventid = HttpContext.Current.Request.Form["league"];
            //sGEventid = HttpContext.Current.Request.Form["league"];

            sLeague = HttpContext.Current.Request.Form["league"];
			sHost = HttpContext.Current.Request.Form["host"];
			sGuest = HttpContext.Current.Request.Form["guest"];
			sMatchDate = HttpContext.Current.Request.Form["matchdate"];
			sMatchTime = HttpContext.Current.Request.Form["matchtime"];
			sMatchField = HttpContext.Current.Request.Form["matchfield"];
			sHostHandicap = HttpContext.Current.Request.Form["hosthandicap"];
			sHNextMatchField = HttpContext.Current.Request.Form["HNextMatchField"];
			sHNextChallenger = HttpContext.Current.Request.Form["HNextChallenger"];
			sHNextLeague = HttpContext.Current.Request.Form["HNextLeague"];
			sGNextMatchField = HttpContext.Current.Request.Form["GNextMatchField"];
			sGNextChallenger = HttpContext.Current.Request.Form["GNextChallenger"];
			sGNextLeague = HttpContext.Current.Request.Form["GNextLeague"];

            arrHEventid = HttpContext.Current.Request.Form["matchEventid3"].Split(delimiter);
            arrGEventid = HttpContext.Current.Request.Form["matchEventid4"].Split(delimiter);

            arrHRecentMatchField = HttpContext.Current.Request.Form["HRecentMatchField"].Split(delimiter);
			arrHRecentHScore = HttpContext.Current.Request.Form["HRecentHScore"].Split(delimiter);
			arrHRecentGScore = HttpContext.Current.Request.Form["HRecentGScore"].Split(delimiter);
			arrHRecentChallenger = HttpContext.Current.Request.Form["HRecentChallenger"].Split(delimiter);
			arrHRecentLeague = HttpContext.Current.Request.Form["HRecentLeague"].Split(delimiter);
			arrGRecentMatchField = HttpContext.Current.Request.Form["GRecentMatchField"].Split(delimiter);
			arrGRecentHScore = HttpContext.Current.Request.Form["GRecentHScore"].Split(delimiter);
			arrGRecentGScore = HttpContext.Current.Request.Form["GRecentGScore"].Split(delimiter);
			arrGRecentChallenger = HttpContext.Current.Request.Form["GRecentChallenger"].Split(delimiter);
			arrGRecentLeague = HttpContext.Current.Request.Form["GRecentLeague"].Split(delimiter);
			
			try {
                //20190514
                //SQLString.Remove(0,SQLString.Length);
                //SQLString.Append("delete from ANALYSIS_RECENT_INFO where IMATCH_CNT=");
                // ///SQLString.Append(sMatchCnt); 
                //SQLString.Append(sEventID);
                //m_SportsDBMgrFb.ExecuteNonQuery(SQLString.ToString());
                //m_SportsDBMgrFb.Close();
                // insert into ANALYSIS_RECENT_INFO(IMATCH_CNT, IREC, CTEAMFLAG, CACT, LEAGUEALIAS, CCHALLENGER, IMATCHSTATUS, IHOSTSCORE, IGUESTSCORE, CTIMESTAMP) values(")

                if(!sHNextMatchField.Equals("-1"))
                {
                    if((sHNextChallenger != null) && (sHNextLeague != null))
                    {
                        if(!sHNextChallenger.Trim().Equals("") && !sHNextLeague.Trim().Equals(""))
                        {
                            ////update for valid match info
                            iRecNo++;
                            using (FbConnection connection2 = new FbConnection(AppFlag.ScoutsDBConn))
                            {
                                connection2.Open();
                                string sql = "UPDATE OR INSERT INTO  ANALYSIS_RECENT_INFO (IMATCH_CNT, IREC, CACT, EVENTID, START_DATE, LEAGUEALIASID, LEAGUEALIAS, CTEAMFLAG, CCHALLENGER, IMATCHSTATUS, IHOSTSCORE, IGUESTSCORE, CTIMESTAMP) VALUES ('" +
                                    sEventID + "', '"+ iRecNo.ToString() + "', 'U', '"+ sRHEventid + "', NULL, NULL, '"+ sHNextLeague.Trim() + "', 'H', '"+ sHNextChallenger.Trim() + "', '"+ sHNextMatchField + "', '-1', '-1', '"+ DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "') MATCHING  (IMATCH_CNT, IREC) ";
                                using (FbCommand cmd2 = new FbCommand(sql, connection2))
                                { 
                                    int id = Convert.ToInt32(cmd2.ExecuteScalar());
                                    if (id > -1)
                                    {
                                        JC_SoccerWeb.Common.Files.CicsWriteLog(" [Success] Insert/Update ANALYSIS_RECENT_INFO H next:" + " " + sHNextChallenger + " " +sHNextLeague);
                                    }
                                }
                                connection2.Close();
                            }
                               
                                // SQLString.Remove(0,SQLString.Length);
                                // SQLString.Append("insert into ANALYSIS_RECENT_INFO values(");
                                // ///SQLString.Append(sMatchCnt); 
                                // SQLString.Append(sEventID);
                                // SQLString.Append(",");
                                // SQLString.Append(iRecNo.ToString());
                                // SQLString.Append(",'H','U','");
                                // SQLString.Append(sHNextLeague.Trim());
                                // SQLString.Append("','");
                                // SQLString.Append(sHNextChallenger.Trim());
                                // SQLString.Append("',");
                                // SQLString.Append(sHNextMatchField);
                                // SQLString.Append(",-1,-1)");
                                // m_SportsDBMgrFb.ExecuteNonQuery(SQLString.ToString());
                                // m_SportsDBMgrFb.Close();
                            }
                    }
                }
                //update for host recent history
                iItemIdx = 0;
				//iINIIdx = 2;
				while(iItemIdx < arrHRecentMatchField.Length) {
					if(!arrHRecentHScore[iItemIdx].Trim().Equals("") && !arrHRecentGScore[iItemIdx].Trim().Equals("") && !arrHRecentChallenger[iItemIdx].Trim().Equals("") && !arrHRecentLeague[iItemIdx].Trim().Equals("")) {
						iRecNo++;
                        using (FbConnection connection2 = new FbConnection(AppFlag.ScoutsDBConn))
                        {
                            connection2.Open();
                            string sql = "UPDATE OR INSERT INTO  ANALYSIS_RECENT_INFO (IMATCH_CNT, IREC, CACT, EVENTID, START_DATE, LEAGUEALIASID, LEAGUEALIAS, CTEAMFLAG, CCHALLENGER, IMATCHSTATUS, IHOSTSCORE, IGUESTSCORE, CTIMESTAMP) VALUES ('" 
                                + sEventID + "', '" + iRecNo.ToString() + "', 'U', '" + (arrHEventid[iItemIdx].Trim() ==null||arrHEventid[iItemIdx].Trim()==""?"-1": arrHEventid[iItemIdx].Trim()) + "', NULL, NULL, '" + arrHRecentLeague[iItemIdx].Trim() + "', 'H', '" + arrHRecentChallenger[iItemIdx].Trim() + "', '" + arrHRecentMatchField[iItemIdx] + "', '"+ arrHRecentHScore[iItemIdx].Trim() + "', '"+ arrHRecentGScore[iItemIdx].Trim() + "', '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "') MATCHING  (IMATCH_CNT, IREC) ";
                            using (FbCommand cmd2 = new FbCommand(sql, connection2))
                            {
                                int id = Convert.ToInt32(cmd2.ExecuteScalar());
                                if (id > -1)
                                {
                                    JC_SoccerWeb.Common.Files.CicsWriteLog(" [Success] Insert/Update ANALYSIS_RECENT_INFO H :" + " " + arrHRecentChallenger[iItemIdx].Trim() + " " + arrHRecentLeague[iItemIdx].Trim());
                                }
                            }
                            connection2.Close();
                        }
                        //SQLString.Remove(0,SQLString.Length);
                        //SQLString.Append("insert into ANALYSIS_RECENT_INFO values(");
                        //                  ///SQLString.Append(sMatchCnt); 
                        //                  SQLString.Append(sEventID);
                        //                  SQLString.Append(",");
                        //SQLString.Append(iRecNo.ToString());
                        //SQLString.Append(",'H','U','");
                        //SQLString.Append(arrHRecentLeague[iItemIdx].Trim());
                        //SQLString.Append("','");
                        //SQLString.Append(arrHRecentChallenger[iItemIdx].Trim());
                        //SQLString.Append("',");
                        //SQLString.Append(arrHRecentMatchField[iItemIdx]);
                        //SQLString.Append(",");
                        //SQLString.Append(arrHRecentHScore[iItemIdx].Trim());
                        //SQLString.Append(",");
                        //SQLString.Append(arrHRecentGScore[iItemIdx].Trim());
                        //SQLString.Append(")");
                        //m_SportsDBMgrFb.ExecuteNonQuery(SQLString.ToString());
                        //m_SportsDBMgrFb.Close();
                        ////iINIIdx++;
                    }
					iItemIdx++;
				}
				if(!sGNextMatchField.Equals("-1")) {
					if((sGNextChallenger != null) && (sGNextLeague != null)) {
						if(!sGNextChallenger.Trim().Equals("") && !sGNextLeague.Trim().Equals("")) {
							//update for valid match info
							iRecNo++;
                            using (FbConnection connection2 = new FbConnection(AppFlag.ScoutsDBConn))
                            {
                                connection2.Open();
                                string sql = "UPDATE OR INSERT INTO  ANALYSIS_RECENT_INFO (IMATCH_CNT, IREC, CACT, EVENTID, START_DATE, LEAGUEALIASID, LEAGUEALIAS, CTEAMFLAG, CCHALLENGER, IMATCHSTATUS, IHOSTSCORE, IGUESTSCORE, CTIMESTAMP) VALUES ('" +
                                    sEventID + "', '" + iRecNo.ToString() + "', 'U', '" + sRGEventid + "', NULL, NULL, '" + sGNextLeague.Trim() + "', 'G', '" + sGNextChallenger.Trim() + "', '" + sGNextMatchField + "', '-1', '-1', '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "') MATCHING  (IMATCH_CNT, IREC) ";
                                using (FbCommand cmd2 = new FbCommand(sql, connection2))
                                {
                                    int id = Convert.ToInt32(cmd2.ExecuteScalar());
                                    if (id > -1)
                                    {
                                        JC_SoccerWeb.Common.Files.CicsWriteLog(" [Success] Insert/Update ANALYSIS_RECENT_INFO G next:" + " " + sGNextChallenger + " " + sGNextLeague);
                                    }
                                }
                                connection2.Close();
                            }
                            //SQLString.Remove(0,SQLString.Length);
                            //                     SQLString.Append("insert into ANALYSIS_RECENT_INFO (IMATCH_CNT, IREC,  CTEAMFLAG, CACT, LEAGUEALIAS, CCHALLENGER, IMATCHSTATUS, IHOSTSCORE, IGUESTSCORE,CTIMESTAMP) values(");
                            //                     ///SQLString.Append(sMatchCnt); 
                            //                     SQLString.Append(sEventID);
                            //                     SQLString.Append(",");
                            //SQLString.Append(iRecNo.ToString());
                            //SQLString.Append(",'G','U','");
                            //SQLString.Append(sGNextLeague.Trim());
                            //SQLString.Append("','");
                            //SQLString.Append(sGNextChallenger.Trim());
                            //SQLString.Append("',");
                            //SQLString.Append(sGNextMatchField);
                            //                     SQLString.Append(",-1,-1," + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + ")");
                            //                     m_SportsDBMgrFb.ExecuteNonQuery(SQLString.ToString());
                            //m_SportsDBMgrFb.Close();
                        }
					}
				}
				//update for guest recent history
				iItemIdx = 0;
				///iINIIdx = 2;
				while(iItemIdx < arrGRecentMatchField.Length) {
					if(!arrGRecentHScore[iItemIdx].Trim().Equals("") && !arrGRecentGScore[iItemIdx].Trim().Equals("") && !arrGRecentChallenger[iItemIdx].Trim().Equals("") && !arrGRecentLeague[iItemIdx].Trim().Equals("")) {
						iRecNo++;
                        using (FbConnection connection2 = new FbConnection(AppFlag.ScoutsDBConn))
                        {
                            connection2.Open();
                            string sql = "UPDATE OR INSERT INTO  ANALYSIS_RECENT_INFO (IMATCH_CNT, IREC, CACT, EVENTID, START_DATE, LEAGUEALIASID, LEAGUEALIAS, CTEAMFLAG, CCHALLENGER, IMATCHSTATUS, IHOSTSCORE, IGUESTSCORE, CTIMESTAMP) VALUES ('" +
                                sEventID + "', '" + iRecNo.ToString() + "', 'U', '" + (arrGEventid[iItemIdx].Trim() == null || arrGEventid[iItemIdx].Trim() == "" ? "-1" : arrGEventid[iItemIdx].Trim()) + "', NULL, NULL, '" + arrGRecentLeague[iItemIdx].Trim() + "', 'G', '" + arrGRecentChallenger[iItemIdx].Trim() + "', '" + arrGRecentMatchField[iItemIdx] + "', '" + arrGRecentHScore[iItemIdx].Trim() + "', '" + arrGRecentGScore[iItemIdx].Trim() + "', '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "') MATCHING  (IMATCH_CNT, IREC) ";
                            using (FbCommand cmd2 = new FbCommand(sql, connection2))
                            {
                                int id = Convert.ToInt32(cmd2.ExecuteScalar());
                                if (id > -1)
                                {
                                    JC_SoccerWeb.Common.Files.CicsWriteLog(" [Success] Insert/Update ANALYSIS_RECENT_INFO G :" + " " + arrGRecentChallenger[iItemIdx].Trim() + " " + arrGRecentLeague[iItemIdx].Trim());
                                }
                            }
                            connection2.Close();
                        }
                        //SQLString.Remove(0,SQLString.Length);
                        //SQLString.Append("insert into ANALYSIS_RECENT_INFO values(");
                        //                  ///SQLString.Append(sMatchCnt); 
                        //                  SQLString.Append(sEventID);
                        //                  SQLString.Append(",");
                        //SQLString.Append(iRecNo.ToString());
                        //SQLString.Append(",'G','U','");
                        //SQLString.Append(arrGRecentLeague[iItemIdx].Trim());
                        //SQLString.Append("','");
                        //SQLString.Append(arrGRecentChallenger[iItemIdx].Trim());
                        //SQLString.Append("',");
                        //SQLString.Append(arrGRecentMatchField[iItemIdx]);
                        //SQLString.Append(",");
                        //SQLString.Append(arrGRecentHScore[iItemIdx].Trim());
                        //SQLString.Append(",");
                        //SQLString.Append(arrGRecentGScore[iItemIdx].Trim());
                        //SQLString.Append(")");
                        //m_SportsDBMgrFb.ExecuteNonQuery(SQLString.ToString());
                        //m_SportsDBMgrFb.Close();
                        ////iINIIdx++;
                    }
					iItemIdx++;
				}
                string sMatchCount = HttpContext.Current.Request.QueryString["eventid"];
                if (sMatchCount != "")
                {
                    JC_SoccerWeb.Common.Files.CicsWriteLog(DateTime.Now.ToString("HH:mm:ss ") + "Send " + sMatchCount + " ANALYSISRECENTS/13 ");
                    string sReslut = JC_SoccerWeb.Common.ConfigManager.SendWinMsg(sMatchCount, "ANALYSISRECENTS/13");
                    //  if (sReslut != "Done") JC_SoccerWeb.Common.Files.CicsWriteLog(DateTime.Now.ToString("HH:mm:ss ")+ "[Failure] Send " + sMatchCount + " ANALYSISBGREMARK-10, " + sReslut);
                }
            }	catch(Exception ex) {
				iRecNo = -1;
				m_SportsLog.FilePath = ConfigurationManager.AppSettings["errlog"];
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " AnalysisRecent.cs.Update(): " + ex.ToString());
				m_SportsLog.Close();
			}
			return iRecNo;
		}

		public int Update() {
			bool bNextExisted = false;
			bool bSendToPager = false;
			int iRecNo = 0;
			int iItemIdx = 0;
			int iSubItemIdx = 1;
			int iINIIdx = 0;
			char[] delimiter = new char[] {','};
			string sAction;
            //string sMatchCnt;
            string sEventID;
            string sLeague;
			string sHost;
			string sGuest;
			string sMatchDate;
			string sMatchTime;
			string sMatchField;
			string sHostHandicap;
			string sHNextMatchField;
			string sHNextChallenger;
			string sHNextLeague;
			string sGNextMatchField;
			string sGNextChallenger;
			string sGNextLeague;
			string sCurrentTimestamp = null;
			string sBatchJob = null;
			string[] arrMsgType;
			string[] arrHRecentMatchField;
			string[] arrHRecentHScore;
			string[] arrHRecentGScore;
			string[] arrHRecentChallenger;
			string[] arrHRecentLeague;
			string[] arrGRecentMatchField;
			string[] arrGRecentHScore;
			string[] arrGRecentGScore;
			string[] arrGRecentChallenger;
			string[] arrGRecentLeague;
			string[] arrSendToPager;
			string[] arrRemotingPath;
			//SportsMessage object message
			SportsMessage sptMsg = new SportsMessage();
			StringBuilder LogSQLString = new StringBuilder();
			DBManager logDBMgr = new DBManager();
			logDBMgr.ConnectionString = m_SportsDBMgrFb.ConnectionString;
			arrRemotingPath = (string[])HttpContext.Current.Application["RemotingItems"];

			try {
				arrSendToPager = HttpContext.Current.Request.Form["SendToPager"].Split(delimiter);
			} catch(Exception) {
				arrSendToPager = new string[0];
			}
			if(arrSendToPager.Length>0) bSendToPager = true;
			arrMsgType = (string[])HttpContext.Current.Application["messageType"];
			sAction = HttpContext.Current.Request.Form["Action"];
            ////sMatchCnt = HttpContext.Current.Request.Form["matchcount"];
            sEventID = HttpContext.Current.Request.Form["matchcount"];
            
            sLeague = HttpContext.Current.Request.Form["league"];
			sHost = HttpContext.Current.Request.Form["host"];
			sGuest = HttpContext.Current.Request.Form["guest"];
			sMatchDate = HttpContext.Current.Request.Form["matchdate"];
			sMatchTime = HttpContext.Current.Request.Form["matchtime"];
			sMatchField = HttpContext.Current.Request.Form["matchfield"];
			sHostHandicap = HttpContext.Current.Request.Form["hosthandicap"];
			sHNextMatchField = HttpContext.Current.Request.Form["HNextMatchField"];
			sHNextChallenger = HttpContext.Current.Request.Form["HNextChallenger"];
			sHNextLeague = HttpContext.Current.Request.Form["HNextLeague"];
			sGNextMatchField = HttpContext.Current.Request.Form["GNextMatchField"];
			sGNextChallenger = HttpContext.Current.Request.Form["GNextChallenger"];
			sGNextLeague = HttpContext.Current.Request.Form["GNextLeague"];
			arrHRecentMatchField = HttpContext.Current.Request.Form["HRecentMatchField"].Split(delimiter);
			arrHRecentHScore = HttpContext.Current.Request.Form["HRecentHScore"].Split(delimiter);
			arrHRecentGScore = HttpContext.Current.Request.Form["HRecentGScore"].Split(delimiter);
			arrHRecentChallenger = HttpContext.Current.Request.Form["HRecentChallenger"].Split(delimiter);
			arrHRecentLeague = HttpContext.Current.Request.Form["HRecentLeague"].Split(delimiter);
			arrGRecentMatchField = HttpContext.Current.Request.Form["GRecentMatchField"].Split(delimiter);
			arrGRecentHScore = HttpContext.Current.Request.Form["GRecentHScore"].Split(delimiter);
			arrGRecentGScore = HttpContext.Current.Request.Form["GRecentGScore"].Split(delimiter);
			arrGRecentChallenger = HttpContext.Current.Request.Form["GRecentChallenger"].Split(delimiter);
			arrGRecentLeague = HttpContext.Current.Request.Form["GRecentLeague"].Split(delimiter);

			try {
				sCurrentTimestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
				sBatchJob = DateTime.Now.ToString("yyMMddHHmmss_fffffff") + "." + HttpContext.Current.Session["user_name"].ToString() + "." + arrMsgType[17] + ".ini";
				if(sAction.Equals("U")) {
					//clear existing records first
					SQLString.Remove(0,SQLString.Length);
					SQLString.Append("delete from ANALYSIS_RECENT_INFO where IMATCH_CNT=");
                    ///SQLString.Append(sMatchCnt); 
                    SQLString.Append(sEventID);
                    m_SportsDBMgrFb.ExecuteNonQuery(SQLString.ToString());
					m_SportsDBMgrFb.Close();

					/*******************
					 * Update for Host *
					 *******************/
					//update next match
					//Insert log into LOG_ANALYSISRECENT
					iSubItemIdx = 1;
					LogSQLString.Remove(0,LogSQLString.Length);
					LogSQLString.Append("insert into LOG_ANALYSISRECENT (TIMEFLAG, IITEMSEQ_NO, SECTION, ACT, LEAGUE, HOST, GUEST, MATCHDATE, MATCHTIME, MATCHFIELD, HOSTHANDICAP, RECENTTYPE, RECENTALIAS, RECENTCHALLENGER, RECENTFIELD, RECENTHOSTSCORE, RECENTGUESTSCORE, BATCHJOB) values ('");
					LogSQLString.Append(sCurrentTimestamp);
					LogSQLString.Append("',");
					LogSQLString.Append(iSubItemIdx.ToString());
					LogSQLString.Append(",'ANALYSISRECENT_','");
					LogSQLString.Append(sAction);
					LogSQLString.Append("','");
					LogSQLString.Append(sLeague);
					LogSQLString.Append("','");
					LogSQLString.Append(sHost);
					LogSQLString.Append("','");
					LogSQLString.Append(sGuest);
					LogSQLString.Append("','");
					LogSQLString.Append(sMatchDate);
					LogSQLString.Append("','");
					LogSQLString.Append(sMatchTime);
					LogSQLString.Append("','");
					LogSQLString.Append(sMatchField);
					LogSQLString.Append("','");
					LogSQLString.Append(sHostHandicap);
					LogSQLString.Append("','H',");
					iRecNo = 0;
					if(!sHNextMatchField.Equals("-1")) {
						if((sHNextChallenger != null) && (sHNextLeague != null)) {
							if(!sHNextChallenger.Trim().Equals("") && !sHNextLeague.Trim().Equals("")) {
								//update for valid match info
								iRecNo++;
								SQLString.Remove(0,SQLString.Length);
								SQLString.Append("insert into ANALYSIS_RECENT_INFO values(");
                                ///SQLString.Append(sMatchCnt); 
                                SQLString.Append(sEventID);
                                SQLString.Append(",");
								SQLString.Append(iRecNo.ToString());
								SQLString.Append(",'H','U','");
								SQLString.Append(sHNextLeague.Trim());
								SQLString.Append("','");
								SQLString.Append(sHNextChallenger.Trim());
								SQLString.Append("',");
								SQLString.Append(sHNextMatchField);
								SQLString.Append(",-1,-1)");
								m_SportsDBMgrFb.ExecuteNonQuery(SQLString.ToString());
								m_SportsDBMgrFb.Close();
								bNextExisted = true;
							}
						}
					}
					if(bNextExisted) {
						LogSQLString.Append("'");
						LogSQLString.Append(sHNextLeague);
						LogSQLString.Append("','");
						LogSQLString.Append(sHNextChallenger);
						LogSQLString.Append("','");
						LogSQLString.Append(arrFields[Convert.ToInt32(sHNextMatchField)]);
						LogSQLString.Append("',");
					} else {
						LogSQLString.Append("'-1','-1','-1',");
					}
					LogSQLString.Append("-1,-1,'");
					LogSQLString.Append(sBatchJob);
					LogSQLString.Append("')");
					if(bSendToPager) {
						logDBMgr.ExecuteNonQuery(LogSQLString.ToString());
						logDBMgr.Close();
					}
					bNextExisted = false;

					//update for recent history
					iItemIdx = 0;
					iINIIdx = 2;
					while(iItemIdx < arrHRecentMatchField.Length) {
						if(!arrHRecentHScore[iItemIdx].Trim().Equals("") && !arrHRecentGScore[iItemIdx].Trim().Equals("") && !arrHRecentChallenger[iItemIdx].Trim().Equals("") && !arrHRecentLeague[iItemIdx].Trim().Equals("")) {
							iRecNo++;
							SQLString.Remove(0,SQLString.Length);
							SQLString.Append("insert into ANALYSIS_RECENT_INFO values(");
                            ///SQLString.Append(sMatchCnt); 
                            SQLString.Append(sEventID);
                            SQLString.Append(",");
							SQLString.Append(iRecNo.ToString());
							SQLString.Append(",'H','U','");
							SQLString.Append(arrHRecentLeague[iItemIdx].Trim());
							SQLString.Append("','");
							SQLString.Append(arrHRecentChallenger[iItemIdx].Trim());
							SQLString.Append("',");
							SQLString.Append(arrHRecentMatchField[iItemIdx]);
							SQLString.Append(",");
							SQLString.Append(arrHRecentHScore[iItemIdx].Trim());
							SQLString.Append(",");
							SQLString.Append(arrHRecentGScore[iItemIdx].Trim());
							SQLString.Append(")");
							m_SportsDBMgrFb.ExecuteNonQuery(SQLString.ToString());
							m_SportsDBMgrFb.Close();

							iSubItemIdx++;
							LogSQLString.Remove(0,LogSQLString.Length);
							LogSQLString.Append("insert into LOG_ANALYSISRECENT (TIMEFLAG, IITEMSEQ_NO, SECTION, ACT, LEAGUE, HOST, GUEST, MATCHDATE, MATCHTIME, MATCHFIELD, HOSTHANDICAP, RECENTTYPE, RECENTALIAS, RECENTCHALLENGER, RECENTFIELD, RECENTHOSTSCORE, RECENTGUESTSCORE, BATCHJOB) values ('");
							LogSQLString.Append(sCurrentTimestamp);
							LogSQLString.Append("',");
							LogSQLString.Append(iSubItemIdx.ToString());
							LogSQLString.Append(",'ANALYSISRECENT_','");
							LogSQLString.Append(sAction);
							LogSQLString.Append("','");
							LogSQLString.Append(sLeague);
							LogSQLString.Append("','");
							LogSQLString.Append(sHost);
							LogSQLString.Append("','");
							LogSQLString.Append(sGuest);
							LogSQLString.Append("','");
							LogSQLString.Append(sMatchDate);
							LogSQLString.Append("','");
							LogSQLString.Append(sMatchTime);
							LogSQLString.Append("','");
							LogSQLString.Append(sMatchField);
							LogSQLString.Append("','");
							LogSQLString.Append(sHostHandicap);
							LogSQLString.Append("','H','");
							LogSQLString.Append(arrHRecentLeague[iItemIdx].Trim());
							LogSQLString.Append("','");
							LogSQLString.Append(arrHRecentChallenger[iItemIdx].Trim());
							LogSQLString.Append("','");
							LogSQLString.Append(arrFields[Convert.ToInt32(arrHRecentMatchField[iItemIdx])]);
							LogSQLString.Append("',");
							LogSQLString.Append(arrHRecentHScore[iItemIdx].Trim());
							LogSQLString.Append(",");
							LogSQLString.Append(arrHRecentGScore[iItemIdx].Trim());
							LogSQLString.Append(",'");
							LogSQLString.Append(sBatchJob);
							LogSQLString.Append("')");
							if(bSendToPager) {
								logDBMgr.ExecuteNonQuery(LogSQLString.ToString());
								logDBMgr.Close();
							}
							iINIIdx++;
						}
						iItemIdx++;
					}


					/********************
					 * Update for Guest *
					 ********************/
					//update next match
					//Insert log into LOG_ANALYSISRECENT
					iSubItemIdx = 1;
					LogSQLString.Remove(0,LogSQLString.Length);
					LogSQLString.Append("insert into LOG_ANALYSISRECENT (TIMEFLAG, IITEMSEQ_NO, SECTION, ACT, LEAGUE, HOST, GUEST, MATCHDATE, MATCHTIME, MATCHFIELD, HOSTHANDICAP, RECENTTYPE, RECENTALIAS, RECENTCHALLENGER, RECENTFIELD, RECENTHOSTSCORE, RECENTGUESTSCORE, BATCHJOB) values ('");
					LogSQLString.Append(sCurrentTimestamp);
					LogSQLString.Append("',");
					LogSQLString.Append(iSubItemIdx.ToString());
					LogSQLString.Append(",'ANALYSISRECENT_','");
					LogSQLString.Append(sAction);
					LogSQLString.Append("','");
					LogSQLString.Append(sLeague);
					LogSQLString.Append("','");
					LogSQLString.Append(sHost);
					LogSQLString.Append("','");
					LogSQLString.Append(sGuest);
					LogSQLString.Append("','");
					LogSQLString.Append(sMatchDate);
					LogSQLString.Append("','");
					LogSQLString.Append(sMatchTime);
					LogSQLString.Append("','");
					LogSQLString.Append(sMatchField);
					LogSQLString.Append("','");
					LogSQLString.Append(sHostHandicap);
					LogSQLString.Append("','G',");
					if(!sGNextMatchField.Equals("-1")) {
						if((sGNextChallenger != null) && (sGNextLeague != null)) {
							if(!sGNextChallenger.Trim().Equals("") && !sGNextLeague.Trim().Equals("")) {
								//update for valid match info
								iRecNo++;
								SQLString.Remove(0,SQLString.Length);
								SQLString.Append("insert into ANALYSIS_RECENT_INFO values(");
                                ///SQLString.Append(sMatchCnt); 
                                SQLString.Append(sEventID);
                                SQLString.Append(",");
								SQLString.Append(iRecNo.ToString());
								SQLString.Append(",'G','U','");
								SQLString.Append(sGNextLeague.Trim());
								SQLString.Append("','");
								SQLString.Append(sGNextChallenger.Trim());
								SQLString.Append("',");
								SQLString.Append(sGNextMatchField);
								SQLString.Append(",-1,-1)");
								m_SportsDBMgrFb.ExecuteNonQuery(SQLString.ToString());
								m_SportsDBMgrFb.Close();
								bNextExisted = true;
							}
						}
					}
					if(bNextExisted) {
						LogSQLString.Append("'");
						LogSQLString.Append(sGNextLeague);
						LogSQLString.Append("','");
						LogSQLString.Append(sGNextChallenger);
						LogSQLString.Append("','");
						LogSQLString.Append(arrFields[Convert.ToInt32(sGNextMatchField)]);
						LogSQLString.Append("',");
					} else {
						LogSQLString.Append("'-1','-1','-1',");
					}
					LogSQLString.Append("-1,-1,'");
					LogSQLString.Append(sBatchJob);
					LogSQLString.Append("')");
					if(bSendToPager) {
						logDBMgr.ExecuteNonQuery(LogSQLString.ToString());
						logDBMgr.Close();
					}
					bNextExisted = false;

					//update for recent history
					iItemIdx = 0;
					iINIIdx = 2;
					while(iItemIdx < arrGRecentMatchField.Length) {
						if(!arrGRecentHScore[iItemIdx].Trim().Equals("") && !arrGRecentGScore[iItemIdx].Trim().Equals("") && !arrGRecentChallenger[iItemIdx].Trim().Equals("") && !arrGRecentLeague[iItemIdx].Trim().Equals("")) {
							iRecNo++;
							SQLString.Remove(0,SQLString.Length);
							SQLString.Append("insert into ANALYSIS_RECENT_INFO values(");
                            ///SQLString.Append(sMatchCnt); 
                            SQLString.Append(sEventID);
                            SQLString.Append(",");
							SQLString.Append(iRecNo.ToString());
							SQLString.Append(",'G','U','");
							SQLString.Append(arrGRecentLeague[iItemIdx].Trim());
							SQLString.Append("','");
							SQLString.Append(arrGRecentChallenger[iItemIdx].Trim());
							SQLString.Append("',");
							SQLString.Append(arrGRecentMatchField[iItemIdx]);
							SQLString.Append(",");
							SQLString.Append(arrGRecentHScore[iItemIdx].Trim());
							SQLString.Append(",");
							SQLString.Append(arrGRecentGScore[iItemIdx].Trim());
							SQLString.Append(")");
							m_SportsDBMgrFb.ExecuteNonQuery(SQLString.ToString());
							m_SportsDBMgrFb.Close();

							iSubItemIdx++;
							LogSQLString.Remove(0,LogSQLString.Length);
							LogSQLString.Append("insert into LOG_ANALYSISRECENT (TIMEFLAG, IITEMSEQ_NO, SECTION, ACT, LEAGUE, HOST, GUEST, MATCHDATE, MATCHTIME, MATCHFIELD, HOSTHANDICAP, RECENTTYPE, RECENTALIAS, RECENTCHALLENGER, RECENTFIELD, RECENTHOSTSCORE, RECENTGUESTSCORE, BATCHJOB) values ('");
							LogSQLString.Append(sCurrentTimestamp);
							LogSQLString.Append("',");
							LogSQLString.Append(iSubItemIdx.ToString());
							LogSQLString.Append(",'ANALYSISRECENT_','");
							LogSQLString.Append(sAction);
							LogSQLString.Append("','");
							LogSQLString.Append(sLeague);
							LogSQLString.Append("','");
							LogSQLString.Append(sHost);
							LogSQLString.Append("','");
							LogSQLString.Append(sGuest);
							LogSQLString.Append("','");
							LogSQLString.Append(sMatchDate);
							LogSQLString.Append("','");
							LogSQLString.Append(sMatchTime);
							LogSQLString.Append("','");
							LogSQLString.Append(sMatchField);
							LogSQLString.Append("','");
							LogSQLString.Append(sHostHandicap);
							LogSQLString.Append("','G','");
							LogSQLString.Append(arrGRecentLeague[iItemIdx].Trim());
							LogSQLString.Append("','");
							LogSQLString.Append(arrGRecentChallenger[iItemIdx].Trim());
							LogSQLString.Append("','");
							LogSQLString.Append(arrFields[Convert.ToInt32(arrGRecentMatchField[iItemIdx])]);
							LogSQLString.Append("',");
							LogSQLString.Append(arrGRecentHScore[iItemIdx].Trim());
							LogSQLString.Append(",");
							LogSQLString.Append(arrGRecentGScore[iItemIdx].Trim());
							LogSQLString.Append(",'");
							LogSQLString.Append(sBatchJob);
							LogSQLString.Append("')");
							if(bSendToPager) {
								logDBMgr.ExecuteNonQuery(LogSQLString.ToString());
								logDBMgr.Close();
							}
							iINIIdx++;
						}
						iItemIdx++;
					}

					//write log
					m_SportsLog.FilePath = ConfigurationManager.AppSettings["eventlog"];
					m_SportsLog.SetFileName(0,LOGFILESUFFIX);
					m_SportsLog.Open();
					m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " AnalysisRecent.cs: update record <L:" + sLeague + ", H:" + sHost + ", G:" + sGuest + "> (" + HttpContext.Current.Session["user_name"] + ")");
					m_SportsLog.Close();
				} else {
					SQLString.Remove(0,SQLString.Length);
					SQLString.Append("update ANALYSIS_RECENT_INFO set CACT='D' where imatch_cnt=");
                    ///SQLString.Append(sMatchCnt); 
                    SQLString.Append(sEventID);
                    m_SportsDBMgrFb.ExecuteNonQuery(SQLString.ToString());
					m_SportsDBMgrFb.Close();

					LogSQLString.Remove(0,LogSQLString.Length);
					LogSQLString.Append("insert into LOG_ANALYSISRECENT (TIMEFLAG, IITEMSEQ_NO, SECTION, ACT, LEAGUE, HOST, GUEST, MATCHDATE, MATCHTIME, MATCHFIELD, HOSTHANDICAP, RECENTTYPE, RECENTALIAS, RECENTCHALLENGER, RECENTFIELD, RECENTHOSTSCORE, RECENTGUESTSCORE, BATCHJOB) values ('");
					LogSQLString.Append(sCurrentTimestamp);
					LogSQLString.Append("',");
					LogSQLString.Append(iSubItemIdx.ToString());
					LogSQLString.Append(",'ANALYSISRECENT_','");
					LogSQLString.Append(sAction);
					LogSQLString.Append("','");
					LogSQLString.Append(sLeague);
					LogSQLString.Append("','");
					LogSQLString.Append(sHost);
					LogSQLString.Append("','");
					LogSQLString.Append(sGuest);
					LogSQLString.Append("','");
					LogSQLString.Append(sMatchDate);
					LogSQLString.Append("','");
					LogSQLString.Append(sMatchTime);
					LogSQLString.Append("','");
					LogSQLString.Append(sMatchField);
					LogSQLString.Append("','");
					LogSQLString.Append(sHostHandicap);
					LogSQLString.Append("','H',null,null,null,null,null,'");
					LogSQLString.Append(sBatchJob);
					LogSQLString.Append("')");
					if(bSendToPager) {
						logDBMgr.ExecuteNonQuery(LogSQLString.ToString());
						logDBMgr.Close();
					}

					//write log
					m_SportsLog.FilePath = ConfigurationManager.AppSettings["eventlog"];
					m_SportsLog.SetFileName(0,LOGFILESUFFIX);
					m_SportsLog.Open();
					m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " AnalysisRecent.cs: delete record <L:" + sLeague + ", H:" + sHost + ", G:" + sGuest + "> (" + HttpContext.Current.Session["user_name"] + ")");
					m_SportsLog.Close();
				}

                string sMatchCount = HttpContext.Current.Request.QueryString["eventid"];
                // JC_SoccerWeb.Common.Files.CicsWriteLog(DateTime.Now.ToString("HH:mm:ss ") + "Sql: " + SQLString.ToString());
                if (sMatchCount != "")
                {
                    JC_SoccerWeb.Common.Files.CicsWriteLog(DateTime.Now.ToString("HH:mm:ss ") + "Send " + sMatchCount + " ANALYSISRECENTS/13 ");
                    string sReslut = JC_SoccerWeb.Common.ConfigManager.SendWinMsg(sMatchCount, "ANALYSISRECENTS/13");
                    //  if (sReslut != "Done") JC_SoccerWeb.Common.Files.CicsWriteLog(DateTime.Now.ToString("HH:mm:ss ")+ "[Failure] Send " + sMatchCount + " ANALYSISBGREMARK-10, " + sReslut);
                }

                if (bSendToPager)
                {
                    string[] arrQueueNames;
                    string[] arrMessageTypes;
                    arrQueueNames = (string[])HttpContext.Current.Application["QueueItems"];
                    arrMessageTypes = (string[])HttpContext.Current.Application["NotifyMessageTypes"];
                    MessageClient msgClt = new MessageClient();
                    msgClt.MessageType = arrMessageTypes[0];
                    msgClt.MessagePath = arrQueueNames[0];

                    //Send Notify Message
                    //Modified by Chapman, 19 Feb 2004
                    sptMsg.Body = sBatchJob;
                    sptMsg.Timestamp = DateTime.Now.ToString("yyMMddHHmmss");
                    sptMsg.AppID = "07";
                    sptMsg.MsgID = "12";
                    sptMsg.DeviceID = new string[0];
                    for (int i = 0; i < arrSendToPager.Length; i++)
                    {
                        sptMsg.AddDeviceID((string)arrSendToPager[i]);
                    }
                    try
                    {
                        //Notify via MSMQ
                        msgClt.MessageType = arrMessageTypes[0];
                        msgClt.MessagePath = arrQueueNames[0];
                        msgClt.SendMessage(sptMsg);
                    }
                    // catch (System.Messaging.MessageQueueException mqEx)
                    catch (InvalidOperationException mqEx)
                    {
                        try
                        {
                            m_SportsLog.FilePath = ConfigurationManager.AppSettings["errlog"];
                            m_SportsLog.SetFileName(0, LOGFILESUFFIX);
                            m_SportsLog.Open();
                            m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " MSMQ ERROR: Analysis Recent");
                            m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " AnalysisRecent.cs.Update(): Notify via MSMQ throws MessageQueueException:  " + mqEx.ToString());
                            m_SportsLog.Close();

                            //If MSMQ fail, notify via .NET Remoting
                            msgClt.MessageType = arrMessageTypes[1];
                            msgClt.MessagePath = arrRemotingPath[0];
                            if (!msgClt.SendMessage((object)sptMsg))
                            {
                                m_SportsLog.FilePath = ConfigurationManager.AppSettings["errlog"];
                                m_SportsLog.SetFileName(0, LOGFILESUFFIX);
                                m_SportsLog.Open();
                                m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Remoting ERROR: Analysis Recent");
                                m_SportsLog.Close();
                            }
                        }
                        catch (Exception ex)
                        {
                            m_SportsLog.FilePath = ConfigurationManager.AppSettings["errlog"];
                            m_SportsLog.SetFileName(0, LOGFILESUFFIX);
                            m_SportsLog.Open();
                            m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Remoting ERROR: Analysis Recent");
                            m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " AnalysisRecent.cs.Update(): Notify via .NET Remoting throws exception: " + ex.ToString());
                            m_SportsLog.Close();
                        }
                    }
                    catch (Exception ex)
                    {
                        m_SportsLog.FilePath = ConfigurationManager.AppSettings["errlog"];
                        m_SportsLog.SetFileName(0, LOGFILESUFFIX);
                        m_SportsLog.Open();
                        m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " AnalysisRecent.cs.Update(): Notify via MSMQ throws exception: " + ex.ToString());
                        m_SportsLog.Close();
                    }
                }
            }
            catch (Exception ex) {
				iRecNo = -1;
				m_SportsLog.FilePath = ConfigurationManager.AppSettings["errlog"];
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " AnalysisRecent.cs.Update(): " + ex.ToString());
				m_SportsLog.Close();
			}
			return iRecNo;
		}
	}
}