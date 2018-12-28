/*
Objective:
Modify league information

Last updated:
22 Sep 2004 (Fanny) Write MSMQ or Remoting to notify MessageDispatcher about league modification
12 July 2004, Chapman Choi

C#.NET complier statement:
csc /t:library /out:..\bin\ModifyLeague.dll /r:..\bin\DBManager.dll;..\bin\Files.dll;..\bin\MessageClient.dll;..\bin\SportsMessage.dll ModifyLeague.cs
*/

using System;
using System.Data.OleDb;
using System.Text;
using System.Reflection;
using System.Web;
using TDL.DB;
using TDL.IO;
using TDL.Message;

[assembly:AssemblyCompany("TDSL")]
[assembly:AssemblyCopyright("(c) 2003 TDSL. All rights reserved. Created on 19 June 2003.")]
[assembly:AssemblyDescription("一般設定 -> 聯賽管理 -> 修改")]
[assembly:AssemblyProduct("Sports.NET")]
[assembly:AssemblyTitle("Sports.NET DLL")]
[assembly:AssemblyVersion("1.0.*")]
namespace SportsUtil {
	public class ModifyLeague {
		const string LOGFILESUFFIX = "log";
		OleDbDataReader m_SportsOleReader;
		DBManager m_SportsDBMgr;
		Files m_SportsLog;
		StringBuilder SQLString;

		public ModifyLeague(string Connection) {
			m_SportsDBMgr = new DBManager();
			m_SportsDBMgr.ConnectionString = Connection;
			m_SportsLog = new Files();
			SQLString = new StringBuilder();
		}

		public string GetLeagues() {
			string sLeagID;
			string sLeague = "";
			string sAlias = "";
			string sLeagType = "";
			string sEngName = "";
			string sMCName = "";
			string sHKJCName = "";
			string sHKJCNameAlias = "";
			StringBuilder HTMLString = new StringBuilder();

			sLeagID = HttpContext.Current.Request.QueryString["leagID"].Trim();
			SQLString.Append("select LEAGNAME, ALIAS, LEAGUETYPE, CHKJCNAME, CHKJCSHORTNAME, CMACAUNAME, CENGNAME from LEAGINFO where LEAG_ID='");
			SQLString.Append(sLeagID);
			SQLString.Append("'");
			try {
				m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
				if(m_SportsOleReader.Read()) {
					sLeague = m_SportsOleReader.GetString(0).Trim();
					sAlias = m_SportsOleReader.GetString(1).Trim();
					if(!m_SportsOleReader.IsDBNull(2)) sLeagType = m_SportsOleReader.GetString(2).Trim();
					sHKJCName = m_SportsOleReader.GetString(3).Trim();
					sHKJCNameAlias = m_SportsOleReader.GetString(4).Trim();
					sMCName = m_SportsOleReader.GetString(5).Trim();
					if(!m_SportsOleReader.IsDBNull(6)) sEngName = m_SportsOleReader.GetString(6).Trim();
				}
				m_SportsOleReader.Close();
				m_SportsDBMgr.Close();
				HTMLString.Append("<tr><th><font color=\"red\">#</font>聯賽名稱(亞洲):</th><td align=\"left\"><input name=\"leagueName\" value=\"");
				HTMLString.Append(sLeague);
				HTMLString.Append("\" maxlength=\"5\" onChange=\"leagueNameChg.value='1'\"><input type=\"hidden\" name=\"leagueOldName\" value=\"");
				HTMLString.Append(sLeague);
				HTMLString.Append("\"><input type=\"hidden\" name=\"leagueNameChg\" value=\"0\"></td>");
				HTMLString.Append("</tr><tr><th><font color=\"red\">#</font>聯賽簡稱(亞洲):</th><td align=\"left\"><input name=\"alias\" value=\"");
				HTMLString.Append(sAlias);
				HTMLString.Append("\" maxlength=\"3\" size=\"10\" onChange=\"aliasChg.value='1'\"><input type=\"hidden\" name=\"aliasChg\" value=\"0\"></tr>");
				HTMLString.Append("<tr><th>聯賽類別:</th><td><select name=\"leaguetype\">");
				if(sLeagType.Equals("1")) {
					HTMLString.Append("<option value=\"1\">國內</option>");
					HTMLString.Append("<option value=\"2\">歐洲</option>");
					HTMLString.Append("<option value=\"3\">國際</option>");
				} else if(sLeagType.Equals("2")) {
					HTMLString.Append("<option value=\"2\">歐洲</option>");
					HTMLString.Append("<option value=\"1\">國內</option>");
					HTMLString.Append("<option value=\"3\">國際</option>");
				} else {
					HTMLString.Append("<option value=\"3\">國際</option>");
					HTMLString.Append("<option value=\"1\">國內</option>");
					HTMLString.Append("<option value=\"2\">歐洲</option>");
				}
				HTMLString.Append("</select></td></tr>");

				HTMLString.Append("<tr><th>馬會名稱:</th><td align=\"left\"><input name=\"HKJCName\" value=\"");
				HTMLString.Append(sHKJCName);
				HTMLString.Append("\" maxlength=\"20\" size=\"30\"></td></tr>");
				HTMLString.Append("<tr><th>馬會簡稱:</th><td align=\"left\"><input name=\"HKJCNameAlias\" value=\"");
				HTMLString.Append(sHKJCNameAlias);
				HTMLString.Append("\" maxlength=\"5\" size=\"10\"></td></tr>");
				HTMLString.Append("<tr><th>澳門名稱:</th><td align=\"left\"><input name=\"MCLeague\" value=\"");
				HTMLString.Append(sMCName);
				HTMLString.Append("\" maxlength=\"20\" size=\"30\"></td></tr>");
				HTMLString.Append("<tr><th>英文名稱:</th><td align=\"left\"><input name=\"engName\" value=\"");
				HTMLString.Append(sEngName);
				HTMLString.Append("\" maxlength=\"50\" size=\"40\"></td></tr>");
				HTMLString.Append("<input type=\"hidden\" name=\"leagID\" value=\"");
				HTMLString.Append(sLeagID);
				HTMLString.Append("\">");
			} catch(Exception ex) {
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " ModifyLeague.cs.GetLeagues(): " + ex.ToString());
				m_SportsLog.Close();
				HTMLString.Remove(0,HTMLString.Length);
				HTMLString.Append((string)HttpContext.Current.Application["accessErrorMsg"]);
			}

			return HTMLString.ToString();
		}

		public int Modify() {
			int iRecUpd = 0;
			string sType;
			string sLeagID;
			string sLeague;
			string sAlias;
			string sHKJCName;
			string sHKJCNameAlias;
			string sMCLeague;
			string sEngName;

			sLeague = HttpContext.Current.Request.Form["leagueName"].Trim();
			sAlias = HttpContext.Current.Request.Form["alias"].Trim();
			sLeagID = HttpContext.Current.Request.Form["leagID"];
			sType = HttpContext.Current.Request.Form["leaguetype"];
			sHKJCName = HttpContext.Current.Request.Form["HKJCName"].Trim();
			sHKJCNameAlias = HttpContext.Current.Request.Form["HKJCNameAlias"].Trim();
			sMCLeague = HttpContext.Current.Request.Form["MCLeague"].Trim();
			sEngName = HttpContext.Current.Request.Form["engName"];
			if(sEngName != null) {
				if(sEngName.Trim().Equals("")) sEngName = null;
				else sEngName = sEngName.Trim();
			}

			try {
				//modify the type of league
				SQLString.Remove(0,SQLString.Length);
				SQLString.Append("update LEAGINFO set LEAGNAME='");
				SQLString.Append(sLeague);
				SQLString.Append("', ALIAS='");
				SQLString.Append(sAlias);
				SQLString.Append("', LEAGUETYPE='");
				SQLString.Append(sType);
				SQLString.Append("', CENGNAME=");
				if(sEngName == null) {
					SQLString.Append("null");
				} else {
					SQLString.Append("'");
					SQLString.Append(sEngName);
					SQLString.Append("'");
				}
				SQLString.Append(", CMACAUNAME='");
				SQLString.Append(sMCLeague);
				SQLString.Append("', CHKJCNAME='");
				SQLString.Append(sHKJCName);
				SQLString.Append("', CHKJCSHORTNAME='");
				SQLString.Append(sHKJCNameAlias);
				SQLString.Append("', LASTUPDATE=CURRENT_TIMESTAMP where LEAG_ID='");
				SQLString.Append(sLeagID);
				SQLString.Append("'");
				iRecUpd = m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
				m_SportsDBMgr.Close();

				//write log
				m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " ModifyLeague.cs: " + SQLString.ToString() + " (" + HttpContext.Current.Session["user_name"] + ")");
				m_SportsLog.Close();


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
					SQLString.Append("select ireg_id from registry_string where cregstr='REFRESH_LEAGUE'");
					sRegId = m_MsgDispManager.ExecuteScalar(SQLString.ToString()).ToString();

					//Tell MessageDispatcher to about leaginfo modification, so MessageDispatcher to reload its team-league mapping
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
						m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " ModifyLeague.cs: (MSMQ) Send alert to MessageDispatcher for league refreshing (AppId="+sptMsg.AppID+",MsgId="+sptMsg.MsgID+",RegId="+sRegId+")");
						m_SportsLog.Close();

					} catch(System.Messaging.MessageQueueException mqEx) {
						try {
							m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
							m_SportsLog.SetFileName(0,LOGFILESUFFIX);
							m_SportsLog.Open();
							m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " ModifyLeague.cs.Modify(): MSMQ ERROR: MessageDispatcher for league refreshing (AppId="+sptMsg.AppID+",MsgId="+sptMsg.MsgID+",RegId="+sRegId+")");
							m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " ModifyLeague.cs.Modify(): Notify (MessageDispatcher for league refreshing) via MSMQ throws MessageQueueException: " + mqEx.ToString());
							m_SportsLog.Close();

							//If MSMQ fail, notify via .NET Remoting
							msgClt.MessageType = arrMessageTypes[1];
							msgClt.MessagePath = arrRemotingPath[0];
							if(!msgClt.SendMessage((object)sptMsg)) {
								m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
								m_SportsLog.SetFileName(0,LOGFILESUFFIX);
								m_SportsLog.Open();
								m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " ModifyLeague.cs.Modify(): Remoting ERROR: MessageDispatcher for league refreshing (AppId="+sptMsg.AppID+",MsgId="+sptMsg.MsgID+",RegId="+sRegId+")");
								m_SportsLog.Close();
							}
							else{
								m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
								m_SportsLog.SetFileName(0,LOGFILESUFFIX);
								m_SportsLog.Open();
								m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " ModifyLeague.cs: (Remoting) Send alert to MessageDispatcher for league refreshing (AppId="+sptMsg.AppID+",MsgId="+sptMsg.MsgID+",RegId="+sRegId+")");
								m_SportsLog.Close();
							}
						}	catch(Exception ex) {
							m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
							m_SportsLog.SetFileName(0,LOGFILESUFFIX);
							m_SportsLog.Open();
							m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " ModifyLeague.cs.Modify(): Remoting ERROR: MessageDispatcher for league refreshing (AppId="+sptMsg.AppID+",MsgId="+sptMsg.MsgID+",RegId="+sRegId+")");
							m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " ModifyLeague.cs.Modify(): Notify (MessageDispatcher for league refreshing) via .NET Remoting throws exception: " + ex.ToString());
							m_SportsLog.Close();
						}
					}	catch(Exception ex) {
						m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
						m_SportsLog.SetFileName(0,LOGFILESUFFIX);
						m_SportsLog.Open();
						m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " ModifyLeague.cs.Modify(): Notify (MessageDispatcher for league refreshing) via MSMQ throws exception: " + ex.ToString());
						m_SportsLog.Close();
					}
				}
				catch(Exception ex){
					m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
					m_SportsLog.SetFileName(0,LOGFILESUFFIX);
					m_SportsLog.Open();
					m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " ModifyLeague.cs.Modify(): Notify (MessageDispatcher for league refreshing) throws exception: " + ex.ToString());
					m_SportsLog.Close();
				}

				//22 Sep 2004 fanny end

			}	catch(Exception ex) {
				iRecUpd = -99;
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " ModifyLeague.cs.Modify(): " + ex.ToString());
				m_SportsLog.Close();
			}

			return iRecUpd;
		}

		public string GetTeams() {
			int iRowCount = 0;
			int iIndex = 0;
			string sLeagID;
			StringBuilder HTMLString = new StringBuilder();

			sLeagID = HttpContext.Current.Request.QueryString["leagID"].Trim();
			try {
				SQLString.Remove(0, SQLString.Length);
				SQLString.Append("select id.TEAM_ID, team.TEAMNAME from ID_INFO id, TEAMINFO team where id.TEAM_ID=team.TEAM_ID and id.LEAG_ID='");
				SQLString.Append(sLeagID);
				SQLString.Append("' order by id.TEAM_ID");
				m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
				while(m_SportsOleReader.Read()) {
					if(iIndex%15 == 0) {
						HTMLString.Append("<tr align=\"left\" ");
						if(iRowCount%2 == 0) {
							HTMLString.Append("style=\"background-color:#E6E6FA\"");
						} else {
							HTMLString.Append("style=\"background-color:#E0FFFF\"");
						}
						HTMLString.Append(">");
						iRowCount++;
					}
					HTMLString.Append("<td><a href=\"TeamManager_MOD.aspx?teamID=");
					HTMLString.Append(m_SportsOleReader.GetInt32(0).ToString());
					HTMLString.Append("\" target=\"content_frame\">");
					HTMLString.Append(m_SportsOleReader.GetString(1).Trim());
					HTMLString.Append("</a></td>");
					if(iIndex%15 == 14) {
						HTMLString.Append("</tr>");
					}
					iIndex++;
				}
				m_SportsOleReader.Close();
				m_SportsDBMgr.Close();
			}	catch(Exception ex) {
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " ModifyLeague.cs.GetTeams(): " + ex.ToString());
				m_SportsLog.Close();
				HTMLString.Remove(0, HTMLString.Length);
				HTMLString.Append((string)HttpContext.Current.Application["accessErrorMsg"]);
			}
			return HTMLString.ToString();
		}

		public string CheckLeagReference(){

			string sLeagueOldName="";
			string sLeagueChg="";
			string sAliasChg="";
			string sRtnMsg="";
			int iRecCnt=0;
			sLeagueOldName = HttpContext.Current.Request.Form["leagueOldName"].Trim();
			sLeagueChg = HttpContext.Current.Request.Form["leagueNameChg"].Trim();
			sAliasChg = HttpContext.Current.Request.Form["aliasChg"].Trim();

			if (sLeagueChg.Equals("0") && sAliasChg.Equals("0")) return sRtnMsg;

			try{

				//check league exist or not in gameinfo table
				SQLString.Remove(0,SQLString.Length);
				SQLString.Append("select count(*) from gameinfo where leaglong='"+sLeagueOldName+"'");
				iRecCnt = m_SportsDBMgr.ExecuteScalar(SQLString.ToString());
				if (iRecCnt>0) sRtnMsg = "足球資訊，";

				//check league exist or not in othersoccerinfo table
				SQLString.Remove(0,SQLString.Length);
				SQLString.Append("select count(*) from othersoccerinfo where leaglong='"+sLeagueOldName+"'");
				iRecCnt = m_SportsDBMgr.ExecuteScalar(SQLString.ToString());
				if (iRecCnt>0) sRtnMsg += "足球２資訊，";

				//check league exist or not in otheroddsinfo table
				SQLString.Remove(0,SQLString.Length);
				SQLString.Append("select count(*) from otheroddsinfo where league='"+sLeagueOldName+"'");
				iRecCnt = m_SportsDBMgr.ExecuteScalar(SQLString.ToString());
				if (iRecCnt>0) sRtnMsg += "其他地區，";

				//check league exist or not in hkjcsoccer_info table
				SQLString.Remove(0,SQLString.Length);
				SQLString.Append("select count(*) from hkjcsoccer_info where cleague='"+sLeagueOldName+"'");
				iRecCnt = m_SportsDBMgr.ExecuteScalar(SQLString.ToString());
				if (iRecCnt>0) sRtnMsg += "本地賽事，";

				if (sRtnMsg.Length>0){
					sRtnMsg = "<font color=\"red\">警告：</font>由於本聯賽賽事已被匯入於<font color=\"red\">" + sRtnMsg + "</font>所以聯賽名稱會有不協調的情況出現，請跟進所受影響的賽事。";
				}

				m_SportsDBMgr.Close();

			}	catch(Exception ex) {
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " ModifyLeague.cs.CheckLeagReference(): " + ex.ToString());
				m_SportsLog.Close();
				sRtnMsg = "";
			}

			return sRtnMsg;

		}

	}

}