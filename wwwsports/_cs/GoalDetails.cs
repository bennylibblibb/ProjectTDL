/*
Objective:
Update and send the details information of match such as scorer and score time

Remarks:
Use HOST_PLAYERNO in RESULTINFO to indicate the record is sent out or not
GUEST_PLAYERNO in RESULTINFO is available

Last updated:
26 July 2004, Chapman Choi

C#.NET complier statement:
csc /t:library /out:..\bin\GoalDetails.dll /r:..\bin\DBManager.dll;..\bin\Files.dll;..\bin\MessageClient.dll;..\bin\SportsMessage.dll GoalDetails.cs
*/

using System;
using System.Collections;
using System.Collections.Specialized;
using System.Data.OleDb;
using System.Reflection;
using System.Text;
using System.Web;
using TDL.DB;
using TDL.IO;
using TDL.Message;

[assembly:AssemblyCompany("TDSL")]
[assembly:AssemblyCopyright("(c) 2003 TDSL. All rights reserved.")]
[assembly:AssemblyDescription("足球資訊 -> 比數詳情")]
[assembly:AssemblyProduct("Sports.NET")]
[assembly:AssemblyTitle("Sports.NET DLL")]
[assembly:AssemblyVersion("3.1.*")]
namespace SportsUtil {
	public class GoalDetails {
		const string LOGFILESUFFIX = "log";
		OleDbDataReader m_SportsOleReader;
		DBManager m_SportsDBMgr;
		Files m_SportsLog;
		Encoding m_Big5Encoded;
		StringBuilder SQLString;

		public GoalDetails(string Connection) {
			m_SportsDBMgr = new DBManager();
			m_SportsDBMgr.ConnectionString = Connection;
			m_SportsLog = new Files();
			m_Big5Encoded = Encoding.GetEncoding(950);
			SQLString = new StringBuilder();
		}

		public string GetDetails() {
			int i = 0;
			int iArrayIndex = 0;
			int iDetailsCount = 0;
			int iItemsIdx = 0;
			string sMatchCount;
			string sMatchDate = "";
			string sMatchTime = "";
			string sHost = "";
			string sGuest = "";
			string sLeague = "";
			string sSentOut = "0";
			string[] arrguestname;
			string[] arrplayername;
			string[] arrresperiod;
			string[] arrflag;
			string[] arrtime;
			string[] arrhscr;
			string[] arrgscr;
			string[] arrplayer;
			string[] arrhostpk;
			string[] arrguestpk;
			string[] arrmatchstate;
			string[] arrplayerno;
			NameValueCollection matchStatusNVC, matchTimeNVC;
			StringBuilder HTMLString = new StringBuilder();

			sMatchCount = HttpContext.Current.Request.QueryString["match_cnt"];
			matchStatusNVC = (NameValueCollection)HttpContext.Current.Application["matchStatusItems"];
			matchTimeNVC = (NameValueCollection)HttpContext.Current.Application["matchTimeItems"];
      try {
	      SQLString.Remove(0,SQLString.Length);
				SQLString.Append("select host, guest, leaglong, matchdate, matchtime from gameinfo where match_cnt=");
				SQLString.Append(sMatchCount);
				m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
				if(m_SportsOleReader.Read()) {
					sHost = m_SportsOleReader.GetString(0);
					sGuest = m_SportsOleReader.GetString(1);
					sLeague = m_SportsOleReader.GetString(2);
					sMatchDate = m_SportsOleReader.GetString(3);
					sMatchTime = m_SportsOleReader.GetString(4);
				}
				m_SportsDBMgr.Close();
				m_SportsOleReader.Close();

				//--------------select menber from resultinfo
				SQLString.Remove(0,SQLString.Length);
				SQLString.Append("select count(match_cnt) from resultinfo where match_cnt=");
				SQLString.Append(sMatchCount);
				SQLString.Append(" and act='U'");
				iDetailsCount = m_SportsDBMgr.ExecuteScalar(SQLString.ToString());
				arrresperiod = new string[iDetailsCount];
				arrflag = new string[iDetailsCount];
				arrtime = new string[iDetailsCount];
				arrhscr = new string[iDetailsCount];
				arrgscr = new string[iDetailsCount];
				arrplayer = new string[iDetailsCount];
				arrhostpk = new string[iDetailsCount];
				arrguestpk = new string[iDetailsCount];
				arrplayername =  new string[iDetailsCount];
				arrguestname =  new string[iDetailsCount];
				arrmatchstate = new string[iDetailsCount];
				arrplayerno = new string[iDetailsCount];
				if(iDetailsCount != 0) {
					SQLString.Remove(0,SQLString.Length);
					SQLString.Append("select sent_flag, res_period, res_flag, res_time, h_scr, g_scr, player, hostpk, guestpk, host_player, guest_player, matchstate, fgs_player_no from resultinfo where match_cnt=");
					SQLString.Append(sMatchCount);
					SQLString.Append(" and act='U' order by  result_id");
					m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
					while(m_SportsOleReader.Read()) {
						if(!sSentOut.Equals(m_SportsOleReader.GetString(0).Trim())) {
							sSentOut = m_SportsOleReader.GetString(0).Trim();
						}
						arrresperiod[iItemsIdx] = m_SportsOleReader.GetString(1).Trim();
						arrflag[iItemsIdx] = m_SportsOleReader.GetString(2).Trim();
						arrtime[iItemsIdx] = m_SportsOleReader.GetString(3).Trim();
						arrhscr[iItemsIdx] = m_SportsOleReader.GetString(4).Trim();
						arrgscr[iItemsIdx] = m_SportsOleReader.GetString(5).Trim();
						arrplayer[iItemsIdx] = m_SportsOleReader.GetString(6).Trim();
						arrhostpk[iItemsIdx] = m_SportsOleReader.GetString(7).Trim();
						arrguestpk[iItemsIdx] = m_SportsOleReader.GetString(8).Trim();
						arrplayername[iItemsIdx] = m_SportsOleReader.GetString(9).Trim();
						arrguestname[iItemsIdx] = m_SportsOleReader.GetString(10).Trim();
						arrmatchstate[iItemsIdx] = m_SportsOleReader.GetString(11).Trim();
						arrplayerno[iItemsIdx] = m_SportsOleReader.GetString(12).Trim();
						iItemsIdx++;
					}
					m_SportsDBMgr.Close();
					m_SportsOleReader.Close();
					m_SportsDBMgr.Dispose();
				} else {
					arrresperiod = new string[0];
					arrflag = new string[0];
					arrtime = new string[0];
					arrhscr = new string[0];
					arrgscr = new string[0];
					arrplayer = new string[0];
					arrhostpk = new string[0];
					arrguestpk = new string[0];
					arrplayername = new string[0];
					arrguestname = new string[0];
					arrmatchstate = new string[0];
					arrplayerno = new string[0];
				}

				sMatchDate = sMatchDate.Insert(4,"/");
				sMatchDate = sMatchDate.Insert(7,"/");
				sMatchTime = sMatchTime.Insert(2,":");
				HTMLString.Remove(0,HTMLString.Length);
				HTMLString.Append("<tr style=\"background-color:#00bfff\" align=\"left\"><th colspan=9>");
				HTMLString.Append(sLeague);
				HTMLString.Append(" - ");
				HTMLString.Append(sHost);
				HTMLString.Append("&nbsp;vs&nbsp;");
				HTMLString.Append(sGuest);
				HTMLString.Append(",&nbsp;");
				HTMLString.Append(sMatchDate);
				HTMLString.Append(",&nbsp;");
				HTMLString.Append(sMatchTime);
				HTMLString.Append("</th></tr>");

				HTMLString.Append("<tr style=\"background-color:#00bfff\"><th colspan=4>比賽狀況:<select name=\"matchstate\">");
				if(arrmatchstate.Length!=0) {
					for(iArrayIndex=0;iArrayIndex<matchStatusNVC.Count;iArrayIndex++) {
						HTMLString.Append("<option value=\"");
						HTMLString.Append(matchStatusNVC.Keys[iArrayIndex].ToString());
						HTMLString.Append("\"");
						if(matchStatusNVC.Keys[iArrayIndex].ToString().Equals(arrmatchstate[0])) HTMLString.Append(" selected");
						HTMLString.Append(">");
						HTMLString.Append(matchStatusNVC.Get(iArrayIndex).ToString());
					}
/*
					for(int j=0;j<13;j++) {
						if(matchStatusNVC.GetKey(j)==arrmatchstate[0]) {
							HTMLString.Append("<option value=\"");
							HTMLString.Append(matchStatusNVC.GetKey(j));
							HTMLString.Append("\" selected>");
							HTMLString.Append(matchStatusNVC[j]);
						} else {
							HTMLString.Append("<option value=\"");
							HTMLString.Append(matchStatusNVC.GetKey(j));
							HTMLString.Append("\">");
							HTMLString.Append(matchStatusNVC[j]);
						}
					}
*/
				}	else {
					for(iArrayIndex=0;iArrayIndex<matchStatusNVC.Count;iArrayIndex++) {
						HTMLString.Append("<option value=\"");
						HTMLString.Append(matchStatusNVC.Keys[iArrayIndex].ToString());
						HTMLString.Append("\">");
						HTMLString.Append(matchStatusNVC.Get(iArrayIndex).ToString());
					}
				}
				HTMLString.Append("</select></th>");
				//HTMLString.Append("<th colspan=4>執行動作:<select name=\"Action\" onChange=\"OnActionChanged()\"><option value=U>更新<option value=D>刪除</select></th></tr>");
				HTMLString.Append("<th colspan=5><input type=\"hidden\" name=\"Action\" value=\"U\"></th></tr>");
				HTMLString.Append("<tr style=\"background-color:#00bfff\" align=center><th>時段</th><th>動作</th><th>時間</th><th>主隊</th>");
				HTMLString.Append("<th>入球者</th><th>客隊</th><th>入球者</th><th>入球者號碼</th><th>備註</th></tr>");

				iArrayIndex=0;
				for(i = 0; i < 12; i++) {
					HTMLString.Append("<tr align=\"center\"");
					if(sSentOut.Equals("1")) {
						if(i < iDetailsCount) {
							HTMLString.Append(" style=\"background-color:#dcdcdc\"");
						}
					}
					HTMLString.Append("><TD><input type=\"hidden\" name=\"writenumber\" value=\"");
					HTMLString.Append(i.ToString());
					HTMLString.Append("\">");
					HTMLString.Append((i+1).ToString());
					HTMLString.Append("<SELECT NAME=\"ScoreEvent\" onChange=\"changeState(");
					HTMLString.Append(i.ToString());
					HTMLString.Append(")\">");
					if(i < iDetailsCount) { 	//i<iDetailsCount用來恢復以前輸入信息..以下判斷都為此作用
						//時段
						for(int j = 0; j < 6; j++) {
							if(matchTimeNVC.GetKey(j)==arrresperiod[i]) {
								HTMLString.Append("<option value=\"");
								HTMLString.Append(matchTimeNVC.GetKey(j));
								HTMLString.Append("\" selected>");
								HTMLString.Append(matchTimeNVC[j]);
							}	else {
								HTMLString.Append("<option value=\"");
								HTMLString.Append(matchTimeNVC.GetKey(j));
								HTMLString.Append("\">");
								HTMLString.Append(matchTimeNVC[j]);
							}
						}
					}	else {
						for(int j = 0; j < 6; j++) {
							HTMLString.Append("<option value=\"");
							HTMLString.Append(matchTimeNVC.GetKey(j));
							HTMLString.Append("\">");
							HTMLString.Append(matchTimeNVC[j]);
						}
					}
					HTMLString.Append("</select></td>\n");

					//動作
					HTMLString.Append("<td><select name=\"ScoreRedFlag\">");
					if(i < iDetailsCount) {
						if(arrflag[i].Equals("S")) {
							HTMLString.Append("<option value=\"S\" selected>入球");
							HTMLString.Append("<option value=\"R\">紅牌");
						}	else {
							HTMLString.Append("<option value=\"R\" selected>紅牌");
							HTMLString.Append("<option value=\"S\">入球");
						}
					}	else {
						HTMLString.Append("<option value=\"S\" selected>入球");
						HTMLString.Append("<option value=\"R\">紅牌");
					}
					HTMLString.Append("</select></td>\n");

					if(i < iDetailsCount) {  //比賽時間,,,主隊得分....主隊入球者
						HTMLString.Append("<TD><INPUT NAME=\"ScoreTime\" SIZE=2 MAXLENGTH=3 value=\"");
						HTMLString.Append(arrtime[i]);
						HTMLString.Append("\" ></TD><TD><INPUT NAME=\"HostScore\" SIZE=1 MAXLENGTH=2 value=\"");
						HTMLString.Append(arrhscr[i]);
						HTMLString.Append("\"></TD><td><input name=\"hostplayer\" size=7 maxlength=5 value=\"");
						HTMLString.Append(arrplayername[i]);
						HTMLString.Append("\"></td>");
					}	else {
						HTMLString.Append("<TD><INPUT NAME=\"ScoreTime\" SIZE=2 MAXLENGTH=3 value=\"\"></TD>\n");
						HTMLString.Append("<TD><INPUT NAME=\"HostScore\" SIZE=1 MAXLENGTH=2  value=\"\" ></TD>");
						HTMLString.Append("<td><input name=\"hostplayer\" size=7 maxlength=5 value=\"\" ></td>");
					}

					if(i<iDetailsCount) {    //客隊得分,客隊入球者
						HTMLString.Append("<TD align=center><INPUT NAME=\"guestScore\" SIZE=1 MAXLENGTH=2 value=\"");
						HTMLString.Append(arrgscr[i]);
						HTMLString.Append("\"></TD><td><input name=\"guestplayer\" size=7 maxlength=5 value=\"");
						HTMLString.Append(arrguestname[i]);
						HTMLString.Append("\" ></td>");
					}	else {
						HTMLString.Append("<TD align=center><INPUT NAME=\"guestScore\" SIZE=2 MAXLENGTH=2 value=\"\"></TD>");
						HTMLString.Append("<td><input name=\"guestplayer\" size=7 maxlength=5 value=\"\"></td>");
					}
					
					if(i<iDetailsCount) {    //入球者號碼
						HTMLString.Append("<td><input name=\"playerNo\" size=5 maxlength=3 value=\"");
						if (arrplayerno[i] != "-1") {
							HTMLString.Append(arrplayerno[i]);
						}
						HTMLString.Append("\" ></td>");
					}	else {
						HTMLString.Append("<td><input name=\"playerNo\" size=5 maxlength=3 value=\"\"></td>");
					}

					if(i<iDetailsCount) {		//備註
						HTMLString.Append("<TD><INPUT NAME=\"remarks\" SIZE=5 MAXLENGTH=5 value=\"");
						HTMLString.Append(arrplayer[i]);
						HTMLString.Append("\"></TD></tr>");
					}	else {
						HTMLString.Append("<TD><INPUT NAME=\"remarks\" SIZE=5 MAXLENGTH=5 value=\"\"></TD></tr>");
					}				
				}//end for
				HTMLString.Append("<tr align=\"center\"><td>");
				if(iDetailsCount > 0) {      //十二碼 (hidden)
					HTMLString.Append("<input type=\"hidden\" name=\"hostpk\" value=\"");
					HTMLString.Append(arrhostpk[0]);
					HTMLString.Append("\"><input type=\"hidden\" name=\"guestpk\" value=\"");
					HTMLString.Append(arrguestpk[0]);
					HTMLString.Append("\">");
				} else {
					HTMLString.Append("<input type=\"hidden\" name=\"hostpk\" value=\"\">");
					HTMLString.Append("<input type=\"hidden\" name=\"guestpk\" value=\"\">");
				}       
				HTMLString.Append("<input type=\"hidden\" name=\"matchcnt\" value=\"");
				HTMLString.Append(sMatchCount);
				HTMLString.Append("\"><a href=\"javascript:openwindow('");
				HTMLString.Append(sMatchCount);
				HTMLString.Append("','oneDetailWin_");
				HTMLString.Append(sMatchCount);
				HTMLString.Append("')\">十二碼</a></td><th colspan=\"8\" align=\"right\"><font color=\"#dcdcdc\">灰色記錄</font>代表曾發送到傳呼機</th></tr>");
			} catch(Exception ex) {
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " GoalDetails.cs.GetDetails(): " + ex.ToString());
				m_SportsLog.Close();
				HTMLString.Remove(0,HTMLString.Length);
				HTMLString.Append((string)HttpContext.Current.Application["accessErrorMsg"]);
			}

			return HTMLString.ToString();
		}

		public int Update() {
			int icount = 0;
			int iDataRecords = 0;
			string sHostpk;
			string sGuestpk;
			string siniplayer;
			string sCurrentStatus;
			string sAction;
			string sAlert;
			string sMatchCount;
			string sLeague = "";
			string sGuest = "";
			string sHost = "";
			string sCurrentTimestamp = null;
			string sBatchJob = null;
			char[] delimiter = new char[] {','};
			string[] arrScoreEvent;
			string[] arrScoreRedFlag;
			string[] arrScoreTime;
			string[] arrHostScore;
			string[] arrGuestScore;
			string[] arrGuestPlayer;
			string[] arrHostPlayer;
			string[] arrPlayerNo;
			string[] arrRemarks;
			string[] arrMsgType;
			string[] arrSendToPager;

			//SportsMessage object message
			SportsMessage sptMsg = new SportsMessage();
			StringBuilder LogSQLString = new StringBuilder();
			DBManager logDBMgr = new DBManager();
			logDBMgr.ConnectionString = m_SportsDBMgr.ConnectionString;

			//Delcare variable used in message notify
			string[] arrQueueNames;
			string[] arrRemotingPath;
			string[] arrMessageTypes;
			arrQueueNames = (string[])HttpContext.Current.Application["QueueItems"];
			arrRemotingPath = (string[])HttpContext.Current.Application["RemotingItems"];
			arrMessageTypes = (string[])HttpContext.Current.Application["NotifyMessageTypes"];
			MessageClient msgClt = new MessageClient();

			try {
				arrSendToPager = HttpContext.Current.Request.Form["SendToPager"].Split(delimiter);
			} catch(Exception) {
				arrSendToPager = new string[0];
			}
			try {
				arrScoreEvent = HttpContext.Current.Request.Form["ScoreEvent"].Split(delimiter);
				arrScoreRedFlag = HttpContext.Current.Request.Form["ScoreRedFlag"].Split(delimiter);
				arrHostScore = HttpContext.Current.Request.Form["HostScore"].Split(delimiter);
				arrGuestScore = HttpContext.Current.Request.Form["guestScore"].Split(delimiter);
				arrGuestPlayer = HttpContext.Current.Request.Form["guestplayer"].Split(delimiter);
				arrHostPlayer = HttpContext.Current.Request.Form["hostplayer"].Split(delimiter);
				arrPlayerNo = HttpContext.Current.Request.Form["playerNo"].Split(delimiter);
				arrRemarks = HttpContext.Current.Request.Form["remarks"].Split(delimiter);
			} catch {
				arrScoreEvent = new string[12];
				arrScoreRedFlag = new string[12];
				arrHostScore = new string[12];
				arrGuestScore = new string[12];
				arrGuestPlayer = new string[12];
				arrHostPlayer = new string[12];
				arrPlayerNo = new string[12];
				arrRemarks = new string[12];
				for(int n = 0; n < 12; n++) {
					arrScoreEvent[n] = "";
					arrScoreRedFlag[n] = "";
					arrHostScore[n] = "";
					arrGuestScore[n] = "";
					arrGuestPlayer[n] = "";
					arrHostPlayer[n] = "";
					arrPlayerNo[n] = "";
					arrRemarks[n] = "";
				}
			}
			arrMsgType = (string[])HttpContext.Current.Application["messageType"];            
			sMatchCount = HttpContext.Current.Request.Form["matchcnt"];
			sGuestpk = HttpContext.Current.Request.Form["guestpk"];
	    	sHostpk = HttpContext.Current.Request.Form["hostpk"];
			sCurrentStatus = HttpContext.Current.Request.Form["matchstate"];
			sAlert = HttpContext.Current.Request.Form["Alert"];
			sAction = HttpContext.Current.Request.Form["Action"];		//used value=0 (更新)   value=1 (刪除)

			if(sHostpk == null) sHostpk = "";
			if(sGuestpk == null) sGuestpk = "";
			if(sAlert == null) sAlert = "0";
			if(sAlert.Trim().Equals("")) sAlert = "0";

			if(sCurrentStatus.Trim().Equals("")) sCurrentStatus = "F";
			try {
				arrScoreTime=HttpContext.Current.Request.Form["ScoreTime"].Split(delimiter);
			} catch {
				arrScoreTime = new string[12];
				for(int n = 0;n<12;n++) {
					arrScoreTime[n] = "";
				}
			}            
			for(int k = 0; k < arrScoreTime.Length; k++) {
				if(arrScoreTime[k] != "") ++iDataRecords;
			}

			try {
				SQLString.Remove(0,SQLString.Length);
				SQLString.Append("select leaglong,host ,guest from gameinfo where match_cnt=");
				SQLString.Append(sMatchCount);
				m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
				if(m_SportsOleReader.Read()) {
					sLeague = m_SportsOleReader.GetString(0);
					sHost = m_SportsOleReader.GetString(1);
					sGuest = m_SportsOleReader.GetString(2);
				}
				m_SportsOleReader.Close();
				m_SportsDBMgr.Close();

				//passed: 刪除和sMatchCount相等值的聯賽的紀錄
				SQLString.Remove(0,SQLString.Length);
				SQLString.Append("delete from resultinfo where match_cnt=");
				SQLString.Append(sMatchCount);
				m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
				m_SportsDBMgr.Close();

				SQLString.Remove(0,SQLString.Length);
				SQLString.Append("select leaglong,host ,guest from gameinfo where match_cnt=");
				SQLString.Append(sMatchCount);
				m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
				if(m_SportsOleReader.Read()) {
					sLeague = m_SportsOleReader.GetString(0);
					sHost = m_SportsOleReader.GetString(1);
					sGuest = m_SportsOleReader.GetString(2);
				}
				m_SportsDBMgr.Close();
				m_SportsOleReader.Close();

				SQLString.Remove(0,SQLString.Length);
				SQLString.Append("select count(result_id) from resultinfo");
				icount = m_SportsDBMgr.ExecuteScalar(SQLString.ToString());
				m_SportsDBMgr.Close();
				if(icount != 0) {
					SQLString.Remove(0,SQLString.Length);
					SQLString.Append("select Max(result_id) from resultinfo");
					icount = m_SportsDBMgr.ExecuteScalar(SQLString.ToString());
					m_SportsDBMgr.Close();
				}

				if(iDataRecords > 0) {
/*
					SQLString.Remove(0,SQLString.Length);
					SQLString.Append("select leaglong,host ,guest from gameinfo where match_cnt=");
					SQLString.Append(sMatchCount);
					m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
					if(m_SportsOleReader.Read()) {
						sLeague = m_SportsOleReader.GetString(0);
						sHost = m_SportsOleReader.GetString(1);
						sGuest = m_SportsOleReader.GetString(2);
					}
					m_SportsDBMgr.Close();
					m_SportsOleReader.Close();

					//passed: 刪除和sMatchCount相等值的聯賽的紀錄
					SQLString.Remove(0,SQLString.Length);
					SQLString.Append("delete from resultinfo where match_cnt=");
					SQLString.Append(sMatchCount);
					m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
					m_SportsDBMgr.Close();

					SQLString.Remove(0,SQLString.Length);
					SQLString.Append("select count(result_id) from resultinfo");
					icount = m_SportsDBMgr.ExecuteScalar(SQLString.ToString());
					m_SportsDBMgr.Close();
					if(icount != 0) {
						SQLString.Remove(0,SQLString.Length);
						SQLString.Append("select Max(result_id) from resultinfo");
						icount = m_SportsDBMgr.ExecuteScalar(SQLString.ToString());
						m_SportsDBMgr.Close();
					}
*/
					//when Pager was checked set it value=0                   
					for(int i = 0; i < iDataRecords; i++) {		//判斷要循環的次數來逐次添加紀錄										
						++icount;
						SQLString.Remove(0,SQLString.Length);
						SQLString.Append("insert into resultinfo (result_id, match_cnt, league, act, matchstate, res_flag, res_period, res_time, h_scr, g_scr, flag, alert, player, host_player, sent_flag, guest_player, fgs_player_no, hostpk, guestpk) values (");
						SQLString.Append(icount.ToString());
						SQLString.Append(",");
						SQLString.Append(sMatchCount);
						SQLString.Append(",'");
						SQLString.Append(sLeague);
						SQLString.Append("','");
						SQLString.Append(sAction);
						SQLString.Append("','");
						SQLString.Append(sCurrentStatus);
						SQLString.Append("','");
						SQLString.Append(arrScoreRedFlag[i]);
						SQLString.Append("','");
						SQLString.Append(arrScoreEvent[i]);
						SQLString.Append("','");
						SQLString.Append(arrScoreTime[i]);
						SQLString.Append("','");
						SQLString.Append(arrHostScore[i]);
						SQLString.Append("','");
						SQLString.Append(arrGuestScore[i]);
						SQLString.Append("','0','");
						SQLString.Append(sAlert);
						SQLString.Append("','");
						SQLString.Append(arrRemarks[i]);
						SQLString.Append("','");
						SQLString.Append(arrHostPlayer[i]);
						SQLString.Append("','");
						if(arrSendToPager.Length > 0) {
							SQLString.Append("1");
						} else {
							SQLString.Append("0");
						}
						SQLString.Append("','");
						SQLString.Append(arrGuestPlayer[i]);
						SQLString.Append("','");
						if (arrPlayerNo[i] == "") {
							arrPlayerNo[i] = "-1";
						}
						SQLString.Append(arrPlayerNo[i]);
						SQLString.Append("','");
						SQLString.Append(sHostpk);
						SQLString.Append("','");
						SQLString.Append(sGuestpk);
						SQLString.Append("')");
						m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
						m_SportsDBMgr.Close();
					}	//end for

					if(arrSendToPager.Length > 0) {
						if(sHostpk.Equals("")) sHostpk = "-1";
						if(sGuestpk.Equals("")) sGuestpk = "-1";

						sCurrentTimestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
						sBatchJob = DateTime.Now.ToString("yyMMddHHmmss_fffffff") + "." + HttpContext.Current.Session["user_name"].ToString() + "." + arrMsgType[15] + ".ini";
						for(int i = 0; i < iDataRecords; i++) {	//Insert log into LOG_GOALDETAILS
							LogSQLString.Remove(0,LogSQLString.Length);
							LogSQLString.Append("insert into LOG_GOALDETAILS (TIMEFLAG, IITEMSEQ_NO, SECTION, LEAGUE, HOST, GUEST, ACT, ALERT, CURRENT_STATUS, PK_FIRST, SCORE_REDFLAG, SCORE_STATUS, SCORE_TIME, SCORE_HOST_GOAL, SCORE_GUEST_GOAL, SCORE_PLAYER, FGS_PLAYER_NO, SCORE_HOST_PK, SCORE_GUEST_PK, BATCHJOB) values ('");
							LogSQLString.Append(sCurrentTimestamp);
							LogSQLString.Append("',");
							LogSQLString.Append((i+1).ToString());
							LogSQLString.Append(",'RES_','");
							LogSQLString.Append(sLeague);
							LogSQLString.Append("','");
							LogSQLString.Append(sHost);
							LogSQLString.Append("','");
							LogSQLString.Append(sGuest);
							LogSQLString.Append("','");
							LogSQLString.Append(sAction);
							LogSQLString.Append("','");
							LogSQLString.Append(sAlert);
							LogSQLString.Append("','");
							LogSQLString.Append(sCurrentStatus);
							LogSQLString.Append("','-1','");
							LogSQLString.Append(arrScoreRedFlag[i]);
							LogSQLString.Append("','");
							LogSQLString.Append(arrScoreEvent[i]);
							LogSQLString.Append("','");

							if(arrScoreTime[i].Equals("")) {
								arrScoreTime[i] = "0";
							}
							LogSQLString.Append(Convert.ToInt32(arrScoreTime[i]).ToString("D3"));
							LogSQLString.Append("','");

							if(arrHostScore[i].Equals("")) {
								arrHostScore[i] = "0";
							}
							LogSQLString.Append(Convert.ToInt32(arrHostScore[i]).ToString("D2"));
							LogSQLString.Append("','");

							if(arrGuestScore[i].Equals("")) {
								arrGuestScore[i] = "0";
							}
							LogSQLString.Append(Convert.ToInt32(arrGuestScore[i]).ToString("D2"));
							LogSQLString.Append("','");
							siniplayer = "";
							if(!arrHostPlayer[i].Trim().Equals("")) siniplayer += arrHostPlayer[i].Trim();
							else if(!arrGuestPlayer[i].Trim().Equals("")) siniplayer += arrGuestPlayer[i].Trim();
							if(!arrRemarks[i].Equals("")) {
								if(siniplayer.Equals("")) siniplayer += arrRemarks[i].Trim();
								else siniplayer += " (" + arrRemarks[i].Trim() + ")";
							}
							if(siniplayer.Equals("")) siniplayer = "-1";
							LogSQLString.Append(siniplayer);
							LogSQLString.Append("','");
							if (arrPlayerNo[i] == "") {
								arrPlayerNo[i] = "-1";
							}
							LogSQLString.Append(arrPlayerNo[i]);
							LogSQLString.Append("','");
							LogSQLString.Append(sHostpk);
							LogSQLString.Append("','");
							LogSQLString.Append(sGuestpk);
							LogSQLString.Append("','");
							LogSQLString.Append(sBatchJob);
							LogSQLString.Append("')");
							logDBMgr.ExecuteNonQuery(LogSQLString.ToString());
							logDBMgr.Close();
						}	//end for

						//Tell MessageDispatcher to generate Goal Details INI and notify other processes
						//Assign value to SportsMessage object
						//modified by Henry, 09 Feb 2004 begin
						sptMsg.IsTransaction = true;
						sptMsg.Body = sBatchJob;
						sptMsg.Timestamp = DateTime.Now.ToString("yyMMddHHmmss");
						sptMsg.AppID = "07";
						sptMsg.MsgID = "16";
						sptMsg.DeviceID = new string[0];						
						for(int i = 0; i < arrSendToPager.Length; i++) {
							sptMsg.AddDeviceID((string)arrSendToPager[i]);
						}
						try {
							//Notify via MSMQ
							msgClt.MessageType = arrMessageTypes[0];
							msgClt.MessagePath = arrQueueNames[0];
							msgClt.SendMessage(sptMsg);
						} catch(System.Messaging.MessageQueueException mqEx) {
							try {
								m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
								m_SportsLog.SetFileName(0,LOGFILESUFFIX);
								m_SportsLog.Open();
								m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " MSMQ ERROR: Goal Details");
								m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " GoalDetails.cs.Update(): Notify via MSMQ throws MessageQueueException: " + mqEx.ToString());
								m_SportsLog.Close();

								//If MSMQ fail, notify via .NET Remoting
								msgClt.MessageType = arrMessageTypes[1];
								msgClt.MessagePath = arrRemotingPath[0];
								if(!msgClt.SendMessage((object)sptMsg)) {
									m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
									m_SportsLog.SetFileName(0,LOGFILESUFFIX);
									m_SportsLog.Open();
									m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Remoting ERROR: Goal Details");
									m_SportsLog.Close();
								}
							}	catch(Exception ex) {
								m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
								m_SportsLog.SetFileName(0,LOGFILESUFFIX);
								m_SportsLog.Open();
								m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Remoting ERROR: Goal Details");
								m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " GoalDetails.cs.Update(): Notify via .NET Remoting throws exception: " + ex.ToString());
								m_SportsLog.Close();
							}
						} catch(Exception ex) {
							m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
							m_SportsLog.SetFileName(0,LOGFILESUFFIX);
							m_SportsLog.Open();
							m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " GoalDetails.cs.Update(): Notify via MSMQ throws exception: " + ex.ToString());
							m_SportsLog.Close();
						}
					}	//end if
					//end modify
				} else {	//end if
					iDataRecords = 0;

					SQLString.Remove(0,SQLString.Length);
					SQLString.Append("insert into resultinfo (result_id, match_cnt, league, act, matchstate, res_flag, res_period, res_time, h_scr, g_scr, flag, alert, player, host_player, sent_flag, guest_player, fgs_player_no, hostpk, guestpk) values (");
					SQLString.Append((icount+1).ToString());
					SQLString.Append(",");
					SQLString.Append(sMatchCount);
					SQLString.Append(",'");
					SQLString.Append(sLeague);
					SQLString.Append("','");
					SQLString.Append(sAction);
					SQLString.Append("','");
					SQLString.Append(sCurrentStatus);
					SQLString.Append("','S','00','','','','0','0','','','0','','-1','','')");
					m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
					m_SportsDBMgr.Close();

					if(arrSendToPager.Length > 0) {
						sCurrentTimestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
						sBatchJob = DateTime.Now.ToString("yyMMddHHmmss_fffffff") + "." + HttpContext.Current.Session["user_name"].ToString() + "." + arrMsgType[15] + ".ini";
						LogSQLString.Remove(0,LogSQLString.Length);
						LogSQLString.Append("insert into LOG_GOALDETAILS (TIMEFLAG, IITEMSEQ_NO, SECTION, LEAGUE, HOST, GUEST, ACT, ALERT, CURRENT_STATUS, PK_FIRST, SCORE_REDFLAG, SCORE_STATUS, SCORE_TIME, SCORE_HOST_GOAL, SCORE_GUEST_GOAL, SCORE_PLAYER, FGS_PLAYER_NO, SCORE_HOST_PK, SCORE_GUEST_PK, BATCHJOB) values ('");
						LogSQLString.Append(sCurrentTimestamp);
						LogSQLString.Append("',1,'RES_','");
						LogSQLString.Append(sLeague);
						LogSQLString.Append("','");
						LogSQLString.Append(sHost);
						LogSQLString.Append("','");
						LogSQLString.Append(sGuest);
						LogSQLString.Append("','");
						LogSQLString.Append(sAction);
						LogSQLString.Append("','");
						LogSQLString.Append(sAlert);
						LogSQLString.Append("','");
						LogSQLString.Append(sCurrentStatus);
						LogSQLString.Append("','-1',null,null,null,null,null,null,null,null,null,'");
						LogSQLString.Append(sBatchJob);
						LogSQLString.Append("')");
						logDBMgr.ExecuteNonQuery(LogSQLString.ToString());
						logDBMgr.Close();

						//Tell MessageDispatcher to generate Goal Details INI and notify other processes
						//Assign value to SportsMessage object
						//modified by Henry, 09 Feb 2004 begin
						sptMsg.IsTransaction = true;
						sptMsg.Body = sBatchJob;
						sptMsg.Timestamp = DateTime.Now.ToString("yyMMddHHmmss");
						sptMsg.AppID = "07";
						sptMsg.MsgID = "16";
						sptMsg.DeviceID = new string[0];						
						for(int i = 0; i < arrSendToPager.Length; i++) {
							sptMsg.AddDeviceID((string)arrSendToPager[i]);
						}
						try {
							//Notify via MSMQ
							msgClt.MessageType = arrMessageTypes[0];
							msgClt.MessagePath = arrQueueNames[0];
							msgClt.SendMessage(sptMsg);
						} catch(System.Messaging.MessageQueueException mqEx) {
							try {
								m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
								m_SportsLog.SetFileName(0,LOGFILESUFFIX);
								m_SportsLog.Open();
								m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " MSMQ ERROR: Goal Details");
								m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " GoalDetails.cs.Update(): Notify via MSMQ throws MessageQueueException: " + mqEx.ToString());
								m_SportsLog.Close();

								//If MSMQ fail, notify via .NET Remoting
								msgClt.MessageType = arrMessageTypes[1];
								msgClt.MessagePath = arrRemotingPath[0];
								if(!msgClt.SendMessage((object)sptMsg)) {
									m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
									m_SportsLog.SetFileName(0,LOGFILESUFFIX);
									m_SportsLog.Open();
									m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Remoting ERROR: Goal Details");
									m_SportsLog.Close();
								}
							}	catch(Exception ex) {
								m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
								m_SportsLog.SetFileName(0,LOGFILESUFFIX);
								m_SportsLog.Open();
								m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Remoting ERROR: Goal Details");
								m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " GoalDetails.cs.Update(): Notify via .NET Remoting throws exception: " + ex.ToString());
								m_SportsLog.Close();
							}
						} catch(Exception ex) {
							m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
							m_SportsLog.SetFileName(0,LOGFILESUFFIX);
							m_SportsLog.Open();
							m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " GoalDetails.cs.Update(): Notify via MSMQ throws exception: " + ex.ToString());
							m_SportsLog.Close();
						}
					}	//end if 
					//end modify
				}
				m_SportsDBMgr.Dispose();

				//write log
				m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " GoalDetails.cs: Send " + iDataRecords.ToString() + " goal details records (" + HttpContext.Current.Session["user_name"] + ")");
				m_SportsLog.Close();
			}	catch(Exception ex) {	//end try
				iDataRecords = -1;
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " GoalDetails.cs.Update(): " + ex.ToString());
				m_SportsLog.Close();
			}

			return iDataRecords;
		}
	}
}