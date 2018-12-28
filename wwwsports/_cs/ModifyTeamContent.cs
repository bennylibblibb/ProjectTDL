/*
Objective:
Modify a team data

Last updated:
22 Sep 2004 (Fanny) Write MSMQ or Remoting to notify MessageDispatcher about team modification
12 July 2004, Chapman Choi

C#.NET complier statement:
csc /t:library /out:..\bin\ModifyTeamContent.dll /r:..\bin\DBManager.dll;..\bin\Files.dll;..\bin\MessageClient.dll;..\bin\SportsMessage.dll ModifyTeamContent.cs
*/

using System;
using System.Collections;
using System.Data.OleDb;
using System.Text;
using System.Reflection;
using System.Web;
using TDL.DB;
using TDL.IO;
using TDL.Message;

[assembly:AssemblyCompany("TDSL")]
[assembly:AssemblyCopyright("(c) 2003 TDSL. All rights reserved. Created on 19 June 2003.")]
[assembly:AssemblyDescription("一般設定 -> 隊伍管理 -> 修改")]
[assembly:AssemblyProduct("Sports.NET")]
[assembly:AssemblyTitle("Sports.NET DLL")]
[assembly:AssemblyVersion("1.0.*")]
namespace SportsUtil {
	public class ModifyTeamContent {
		const string LOGFILESUFFIX = "log";
		OleDbDataReader m_SportsOleReader;
		DBManager m_SportsDBMgr;
		Files m_SportsLog;
		StringBuilder SQLString;

		public ModifyTeamContent(string Connection) {
			m_SportsDBMgr = new DBManager();
			m_SportsDBMgr.ConnectionString = Connection;
			m_SportsLog = new Files();
			SQLString = new StringBuilder();
		}

		public string GetTeam() {
			int iLeagueCount = 0;
			int iItemCount = 0;
			int iRowCount = 0;
			string sTeamID;
			string sLeagID;
			string sTeam = "";
			string sHKJCName = "";
			string sHKJCNameAlias = "";
			string sMCName = "";
			string sEngName = "";
			string sContinent = "";
			string sCountry = "";
			string sCity = "";
			string sVenue = "";
			ArrayList leagIDAL = new ArrayList();
			StringBuilder HTMLString = new StringBuilder();

			sTeamID = HttpContext.Current.Request.QueryString["teamID"].Trim();
			try {
				//get team-league relationship from id_info
				SQLString.Remove(0,SQLString.Length);
				SQLString.Append("select id.LEAG_ID, team.TEAMNAME, team.CHKJCNAME, team.CHKJCSHORTNAME, team.CMACAUNAME, team.CENGNAME, team.COUNTRY, team.CITY, team.VENUE, team.CONTINENT from TEAMINFO team LEFT OUTER JOIN ID_INFO id ON id.TEAM_ID=");
				SQLString.Append(sTeamID);
				SQLString.Append(" where team.TEAM_ID=");
				SQLString.Append(sTeamID);
				SQLString.Append(" order by id.LEAG_ID");
				m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
				while(m_SportsOleReader.Read()) {
					if(!m_SportsOleReader.IsDBNull(0)) {
						leagIDAL.Add(m_SportsOleReader.GetString(0).Trim());
					}
					if(iItemCount == 0) {
						sTeam = m_SportsOleReader.GetString(1).Trim();
						sHKJCName = m_SportsOleReader.GetString(2).Trim();
						sHKJCNameAlias = m_SportsOleReader.GetString(3).Trim();
						sMCName = m_SportsOleReader.GetString(4).Trim();
						if(!m_SportsOleReader.IsDBNull(5)) sEngName = m_SportsOleReader.GetString(5).Trim();
						if(!m_SportsOleReader.IsDBNull(6)) sCountry = m_SportsOleReader.GetString(6).Trim();
						if(!m_SportsOleReader.IsDBNull(7)) sCity = m_SportsOleReader.GetString(7).Trim();
						if(!m_SportsOleReader.IsDBNull(8)) sVenue = m_SportsOleReader.GetString(8).Trim();
						if(!m_SportsOleReader.IsDBNull(9)) sContinent = m_SportsOleReader.GetString(9).Trim();
					}
					iItemCount++;
				}
				m_SportsOleReader.Close();
				m_SportsDBMgr.Close();

				//retrieve all leagues from table
				HTMLString.Append("<tr align=\"center\" style=\"background-color:#FFA07A\"><th colspan=\"2\">修改<font color=\"#FFFAF0\">");
				HTMLString.Append(sTeam);
				HTMLString.Append("</font>資料&nbsp;(按<font color=\"blue\">聯賽名稱</font>的連結，可修改聯賽)</th></tr><tr><th align=\"center\"><font color=\"red\">*</font>所屬聯賽:</th><td><center><table border=\"1\">");
				SQLString.Remove(0,SQLString.Length);
				SQLString.Append("select LEAG_ID, ALIAS from LEAGINFO order by LEAG_ORDER, LEAG_ID");
				m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
				while(m_SportsOleReader.Read()) {
					if(iLeagueCount%10 == 0) {
						HTMLString.Append("<tr align=\"center\" ");
						if(iRowCount%2 == 0) {
							HTMLString.Append("style=\"background-color:#FFFAF0\"");
						} else {
							HTMLString.Append("style=\"background-color:#F0F8FF\"");
						}
						HTMLString.Append(">");
						iRowCount++;
					}

					sLeagID = m_SportsOleReader.GetString(0).Trim();
					HTMLString.Append("<td><input type=\"checkbox\" name=\"leagID\" value=\"");
					HTMLString.Append(sLeagID);
					HTMLString.Append("\" ");
					if(leagIDAL.Contains(sLeagID)) HTMLString.Append("checked");
					HTMLString.Append("><a href=\"LeagueManager_MODFrame.aspx?leagID=");
					HTMLString.Append(sLeagID);
					HTMLString.Append("\" target=\"content_frame\">");
					HTMLString.Append(m_SportsOleReader.GetString(1).Trim());
					HTMLString.Append("</a></td>");

					if(iLeagueCount%10 == 9) {
						HTMLString.Append("</tr>");
					}
					iLeagueCount++;
				}
				m_SportsOleReader.Close();
				m_SportsDBMgr.Close();
				HTMLString.Append("</table></center></td></tr>");

				HTMLString.Append("<tr><th align=\"center\"><font color=\"red\">#</font>隊伍名稱(亞洲):</th><td><input type=\"text\" name=\"AsiaName\" value=\"");
				HTMLString.Append(sTeam);
				HTMLString.Append("\" maxlength=\"10\" size=\"10\" onChange=\"AsiaNameChg.value='1'\"><input type=\"hidden\" name=\"AsiaOldName\" value=\"");
				HTMLString.Append(sTeam);
				HTMLString.Append("\"><input type=\"hidden\" name=\"AsiaNameChg\" value=\"0\"></td></tr>");

				HTMLString.Append("<tr><th align=\"center\">馬會全名:</th><td><input type=\"text\" name=\"HKJCName\" value=\"");
				HTMLString.Append(sHKJCName);
				HTMLString.Append("\" maxlength=\"20\" size=\"20\"></td></tr>");

				HTMLString.Append("<tr><th align=\"center\">馬會簡稱:</th><td><input type=\"text\" name=\"HKJCNameAlias\" value=\"");
				HTMLString.Append(sHKJCNameAlias);
				HTMLString.Append("\" maxlength=\"10\" size=\"10\"></td></tr>");

				HTMLString.Append("<tr><th align=\"center\">澳門名稱:</th><td><input type=\"text\" name=\"MCTeam\" value=\"");
				HTMLString.Append(sMCName);
				HTMLString.Append("\" maxlength=\"20\" size=\"20\"></td></tr>");

				HTMLString.Append("<tr><th align=\"center\">英文名稱:</th><td><input type=\"text\" name=\"EnglishName\" value=\"");
				HTMLString.Append(sEngName);
				HTMLString.Append("\" maxlength=\"50\" size=\"40\"></td></tr>");

				HTMLString.Append("<tr><th align=\"center\">主場名稱:</th><td><input type=\"text\" name=\"venue\" value=\"");
				HTMLString.Append(sVenue);
				HTMLString.Append("\" maxlength=\"10\" size=\"10\"></td></tr>");

				HTMLString.Append("<tr><th align=\"center\">所屬洲份:</th><td><input type=\"text\" name=\"continent\" value=\"");
				HTMLString.Append(sContinent);
				HTMLString.Append("\" maxlength=\"4\" size=\"10\"></td></tr>");

				HTMLString.Append("<tr><th align=\"center\">所屬國家:</th><td><input type=\"text\" name=\"country\" value=\"");
				HTMLString.Append(sCountry);
				HTMLString.Append("\" maxlength=\"5\" size=\"10\"></td></tr>");

				HTMLString.Append("<tr><th align=\"center\">所屬城巿:</th><td><input type=\"text\" name=\"city\" value=\"");
				HTMLString.Append(sCity);
				HTMLString.Append("\" maxlength=\"5\" size=\"10\"></td></tr>");

				HTMLString.Append("<input type=\"hidden\" name=\"teamID\" value=\"");
				HTMLString.Append(sTeamID);
				HTMLString.Append("\">");
			}	catch(Exception ex) {
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " ModifyTeamContent.cs.GetTeam(): " + ex.ToString());
				m_SportsLog.Close();
				HTMLString.Remove(0,HTMLString.Length);
				HTMLString.Append((string)HttpContext.Current.Application["accessErrorMsg"]);
			}

			return HTMLString.ToString();
		}

		public int Modify() {
			int iRecUpd = 0;
			int iLeagSelected = 0;
			int iUpdIdx = 0;
			string sTeamID;
			string sTeam;
			string sHKJCName;
			string sHKJCNameAlias;
			string sMCName;
			string sEngName;
			string sVenue;
			string sContinent;
			string sCountry;
			string sCity;
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
			sTeam = HttpContext.Current.Request.Form["AsiaName"];
			sHKJCName = HttpContext.Current.Request.Form["HKJCName"];
			sHKJCNameAlias = HttpContext.Current.Request.Form["HKJCNameAlias"];
			sMCName = HttpContext.Current.Request.Form["MCTeam"];
			sEngName = HttpContext.Current.Request.Form["EnglishName"];
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
				//update teaminfo
				SQLString.Remove(0,SQLString.Length);
				SQLString.Append("update TEAMINFO set TEAMNAME='");
				SQLString.Append(sTeam);
				SQLString.Append("', COUNTRY=");;
				if(sCountry == null) {
					SQLString.Append("null");
				}	else {
					SQLString.Append("'");
					SQLString.Append(sCountry);
					SQLString.Append("'");
				}
				SQLString.Append(", CITY=");
				if(sCity == null) {
					SQLString.Append("null");
				}	else {
					SQLString.Append("'");
					SQLString.Append(sCity);
					SQLString.Append("'");
				}
				SQLString.Append(", VENUE=");
				if(sVenue == null) {
					SQLString.Append("null");
				}	else {
					SQLString.Append("'");
					SQLString.Append(sVenue);
					SQLString.Append("'");
				}
				SQLString.Append(", CONTINENT=");
				if(sContinent == null) {
					SQLString.Append("null");
				}	else {
					SQLString.Append("'");
					SQLString.Append(sContinent);
					SQLString.Append("'");
				}
				SQLString.Append(", CENGNAME=");
				if(sEngName == null) {
					SQLString.Append("null");
				}	else {
					SQLString.Append("'");
					SQLString.Append(sEngName);
					SQLString.Append("'");
				}
				SQLString.Append(", CMACAUNAME='");
				SQLString.Append(sMCName);
				SQLString.Append("', CHKJCNAME='");
				SQLString.Append(sHKJCName);
				SQLString.Append("', CHKJCSHORTNAME='");
				SQLString.Append(sHKJCNameAlias);
				SQLString.Append("', LASTUPDATE=CURRENT_TIMESTAMP where TEAM_ID=");
				SQLString.Append(sTeamID);
				iRecUpd = m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
				m_SportsDBMgr.Close();

				//write log
				m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " ModifyTeamContent.cs: " + SQLString.ToString() + " (" + HttpContext.Current.Session["user_name"] + ")");
				m_SportsLog.Close();

				//update team-league relationship
				//reset id_info w.r.t. team first
				SQLString.Remove(0,SQLString.Length);
				SQLString.Append("delete from ID_INFO where TEAM_ID=");
				SQLString.Append(sTeamID);
				m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
				m_SportsDBMgr.Close();

				if(iLeagSelected != 0) {	//teams join to id_info
					for(iUpdIdx = 0; iUpdIdx < iLeagSelected; iUpdIdx++) {
						SQLString.Remove(0,SQLString.Length);
						SQLString.Append("insert into ID_INFO values('");
						SQLString.Append(arrLeagID[iUpdIdx]);
						SQLString.Append("',");
						SQLString.Append(sTeamID);
						SQLString.Append(")");
						m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
						m_SportsDBMgr.Close();
						iRecUpd++;
					}
				}

				//22 Sep 2004 fanny start

				string sRegId="0";
				string[] arrQueueNames;
				string[] arrRemotingPath;
				string[] arrMessageTypes;
				DBManager m_MsgDispManager;
				MessageClient msgClt;
				SportsMessage sptMsg;
				try{
					m_MsgDispManager = new DBManager();
					msgClt = new MessageClient();
					sptMsg = new SportsMessage();
					m_MsgDispManager.ConnectionString = (string) HttpContext.Current.Application["MSGDISPATCHERConnectionString"];
					arrQueueNames = (string[])HttpContext.Current.Application["QueueItems"];
					arrRemotingPath = (string[])HttpContext.Current.Application["RemotingItems"];
					arrMessageTypes = (string[])HttpContext.Current.Application["NotifyMessageTypes"];

					//Get registration id
					SQLString.Remove(0,SQLString.Length);
					SQLString.Append("select ireg_id from registry_string where cregstr='REFRESH_TEAM'");
					sRegId = m_MsgDispManager.ExecuteScalar(SQLString.ToString()).ToString();

					//Tell MessageDispatcher to about teaminfo modification, so MessageDispatcher to reload its team-league mapping
					//Assign value to SportsMessage object
					sptMsg.IsTransaction = false;
					sptMsg.Timestamp = DateTime.Now.ToString("yyMMddHHmmss");
					sptMsg.AppID = "00";
					sptMsg.MsgID = "90";
					sptMsg.Body = System.Text.Encoding.GetEncoding(950).GetBytes(sRegId).Length.ToString("D3") + sRegId;

					try{

						//Notify via MSMQ
						msgClt.MessageType = arrMessageTypes[0];
						msgClt.MessagePath = arrQueueNames[0];
						msgClt.SendMessage(sptMsg);

						m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
						m_SportsLog.SetFileName(0,LOGFILESUFFIX);
						m_SportsLog.Open();
						m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " ModifyTeamContent.cs: (MSMQ) Send alert to MessageDispatcher for team refreshing (AppId="+sptMsg.AppID+",MsgId="+sptMsg.MsgID+",RegId="+sRegId+")");
						m_SportsLog.Close();

					} catch(System.Messaging.MessageQueueException mqEx) {
						try {
							m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
							m_SportsLog.SetFileName(0,LOGFILESUFFIX);
							m_SportsLog.Open();
							m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " ModifyTeamContent.cs.Modify(): MSMQ ERROR: MessageDispatcher for team refreshing (AppId="+sptMsg.AppID+",MsgId="+sptMsg.MsgID+",RegId="+sRegId+")");
							m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " ModifyTeamContent.cs.Modify(): Notify (MessageDispatcher for team refreshing) via MSMQ throws MessageQueueException: " + mqEx.ToString());
							m_SportsLog.Close();

							//If MSMQ fail, notify via .NET Remoting
							msgClt.MessageType = arrMessageTypes[1];
							msgClt.MessagePath = arrRemotingPath[0];
							if(!msgClt.SendMessage((object)sptMsg)) {
								m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
								m_SportsLog.SetFileName(0,LOGFILESUFFIX);
								m_SportsLog.Open();
								m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " ModifyTeamContent.cs.Modify(): Remoting ERROR: MessageDispatcher for team refreshing (AppId="+sptMsg.AppID+",MsgId="+sptMsg.MsgID+",RegId="+sRegId+")");
								m_SportsLog.Close();
							}
							else{
								m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
								m_SportsLog.SetFileName(0,LOGFILESUFFIX);
								m_SportsLog.Open();
								m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " ModifyTeamContent.cs: (Remoting) Send alert to MessageDispatcher for team refreshing (AppId="+sptMsg.AppID+",MsgId="+sptMsg.MsgID+",RegId="+sRegId+")");
								m_SportsLog.Close();
							}
						}	catch(Exception ex) {
							m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
							m_SportsLog.SetFileName(0,LOGFILESUFFIX);
							m_SportsLog.Open();
							m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " ModifyTeamContent.cs.Modify(): Remoting ERROR: MessageDispatcher for team refreshing (AppId="+sptMsg.AppID+",MsgId="+sptMsg.MsgID+",RegId="+sRegId+")");
							m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " ModifyTeamContent.cs.Modify(): Notify (MessageDispatcher for team refreshing) via .NET Remoting throws exception: " + ex.ToString());
							m_SportsLog.Close();
						}
					}	catch(Exception ex) {
						m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
						m_SportsLog.SetFileName(0,LOGFILESUFFIX);
						m_SportsLog.Open();
						m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " ModifyTeamContent.cs.Modify(): Notify (MessageDispatcher for team refreshing) via MSMQ throws exception: " + ex.ToString());
						m_SportsLog.Close();
					}
				}
				catch(Exception ex){
					m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
					m_SportsLog.SetFileName(0,LOGFILESUFFIX);
					m_SportsLog.Open();
					m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " ModifyTeamContent.cs.Modify(): Notify (MessageDispatcher for team refreshing) throws exception: " + ex.ToString());
					m_SportsLog.Close();
				}
				//22 Sep 2004 fanny end


			}	catch(Exception ex) {
				iRecUpd = -99;
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " ModifyTeamContent.cs.Modify(): " + ex.ToString());
				m_SportsLog.Close();
			}

			return iRecUpd;
		}

		public string CheckLeagReference(){

			string sAsiaOldName="";
			string sAsiaNameChg="";
			string sRtnMsg="";
			int iRecCnt=0;
			sAsiaOldName = HttpContext.Current.Request.Form["AsiaOldName"].Trim();
			sAsiaNameChg = HttpContext.Current.Request.Form["AsiaNameChg"].Trim();

			if (sAsiaNameChg.Equals("0")) return sRtnMsg;

			try{

				//check team exist or not in gameinfo table
				SQLString.Remove(0,SQLString.Length);
				SQLString.Append("select count(*) from gameinfo where host='"+sAsiaOldName+"' or guest='"+sAsiaOldName+"'");
				iRecCnt = m_SportsDBMgr.ExecuteScalar(SQLString.ToString());
				if (iRecCnt>0) sRtnMsg = "足球資訊，";

				//check league exist or not in othersoccerinfo table
				SQLString.Remove(0,SQLString.Length);
				SQLString.Append("select count(*) from othersoccerinfo where host='"+sAsiaOldName+"' or guest='"+sAsiaOldName+"'");
				iRecCnt = m_SportsDBMgr.ExecuteScalar(SQLString.ToString());
				if (iRecCnt>0) sRtnMsg += "足球２資訊，";

				//check league exist or not in otheroddsinfo table
				SQLString.Remove(0,SQLString.Length);
				SQLString.Append("select count(*) from otheroddsinfo where host='"+sAsiaOldName+"' or guest='"+sAsiaOldName+"'");
				iRecCnt = m_SportsDBMgr.ExecuteScalar(SQLString.ToString());
				if (iRecCnt>0) sRtnMsg += "其他地區，";

				//check league exist or not in hkjcsoccer_info table
				SQLString.Remove(0,SQLString.Length);
				SQLString.Append("select count(*) from hkjcsoccer_info where chost='"+sAsiaOldName+"' or cguest='"+sAsiaOldName+"'");
				iRecCnt = m_SportsDBMgr.ExecuteScalar(SQLString.ToString());
				if (iRecCnt>0) sRtnMsg += "本地賽事，";

				if (sRtnMsg.Length>0){
					sRtnMsg = "<font color=\"red\">警告：</font>由於本隊伍賽事已被匯入於<font color=\"red\">" + sRtnMsg + "</font>所以隊伍名稱會有不協調的情況出現，請執行賽事修改。";
				}

				m_SportsDBMgr.Close();

			}	catch(Exception ex) {
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " ModifyTeamContent.cs.CheckLeagReference(): " + ex.ToString());
				m_SportsLog.Close();
				sRtnMsg = "";
			}

			return sRtnMsg;

		}

	}
}