/*
Objective:
Preview and send matches analysis information

Last updated:
13 June 2006, Christopher

C#.NET complier statement:
csc /t:library /out:..\bin\JCComboPlayerPreview.dll /r:..\bin\DBManager.dll;..\bin\Files.dll;..\bin\MessageClient.dll;..\bin\SportsMessage.dll JCComboPlayerPreview.cs
*/

using System;
using System.Collections;
using System.Data.OleDb;
using System.Reflection;
using System.Text;
using System.Web;
using TDL.DB;
using TDL.IO;
using TDL.Message;

[assembly:AssemblyCompany("TDSL")]
[assembly:AssemblyCopyright("(c) 2003 TDSL. All rights reserved.")]
[assembly:AssemblyDescription("足球資訊 -> JCCombo球員 -> 發送")]
[assembly:AssemblyProduct("Sports.NET")]
[assembly:AssemblyTitle("Sports.NET DLL")]
[assembly:AssemblyVersion("3.1.*")]
namespace SportsUtil {
	public class JCComboPlayerPreview {
		const string LOGFILESUFFIX = "log";
		int iRecordsInPage = 0;
		OleDbDataReader m_SportsOleReader;
		DBManager m_SportsDBMgr;
		Files m_SportsLog;
		Encoding m_Big5Encoded;
		StringBuilder SQLString;

		public JCComboPlayerPreview(string Connection) {
			m_SportsDBMgr = new DBManager();
			m_SportsDBMgr.ConnectionString = Connection;
			m_SportsLog = new Files();
			m_Big5Encoded = Encoding.GetEncoding(950);
			SQLString = new StringBuilder();
		}

		public int NumberOfRecords {
			get {
				return iRecordsInPage;
			}
		}

		public string PreviewMatches() {
			int iMatchCnt = 0;
			string sMatchDate = "";
			string sMatchTime = "";
			string uid;
			StringBuilder HTMLString = new StringBuilder();

			try {
				uid = HttpContext.Current.Session["user_id"].ToString();
				SQLString.Remove(0,SQLString.Length);
				SQLString.Append("select gameinfo.matchdate, gameinfo.matchtime, gameinfo.leaglong, gameinfo.league, gameinfo.host, gameinfo.guest, gameinfo.match_cnt, leaginfo.LEAGUETYPE from gameinfo, leaginfo, analysis_bg_info where gameinfo.match_cnt=analysis_bg_info.imatch_cnt and gameinfo.league=leaginfo.alias and analysis_bg_info.cact='U' and leaginfo.leag_id in (select cleag_id from userprofile_info where iuser_id=");
				SQLString.Append(uid);
				SQLString.Append(") order by leaginfo.leag_order, gameinfo.matchdate, gameinfo.matchtime");
				m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
				while (m_SportsOleReader.Read()) {
					sMatchDate = m_SportsOleReader.GetString(0).Trim();
					sMatchTime = m_SportsOleReader.GetString(1).Trim();
					sMatchDate = sMatchDate.Insert(4,"/");
					sMatchDate = sMatchDate.Insert(7,"/");
					sMatchTime = sMatchTime.Insert(2,":");

					HTMLString.Append("<tr align=\"center\"><td>");
					HTMLString.Append(sMatchDate);
					HTMLString.Append("&nbsp;");
					HTMLString.Append(sMatchTime);
					HTMLString.Append("</td><td><input type=\"hidden\" name=\"league\" value=\"");
					HTMLString.Append(m_SportsOleReader.GetString(2).Trim());
					HTMLString.Append("\">");
					HTMLString.Append(m_SportsOleReader.GetString(3).Trim());
					HTMLString.Append("</td><td><input type=\"hidden\" name=\"host\" value=\"");
					HTMLString.Append(m_SportsOleReader.GetString(4).Trim());
					HTMLString.Append("\">");
					HTMLString.Append(m_SportsOleReader.GetString(4).Trim());
					HTMLString.Append("</td><td><input type=\"hidden\" name=\"guest\" value=\"");
					HTMLString.Append(m_SportsOleReader.GetString(5).Trim());
					HTMLString.Append("\">");
					HTMLString.Append(m_SportsOleReader.GetString(5).Trim());
					HTMLString.Append("</td><td><select name=\"Action\" onChange=\"OnActionChanged(");
					HTMLString.Append(iRecordsInPage.ToString());
					HTMLString.Append(")\"><option value=\"U\">發送<option value=\"D\">刪除</select></td>");

					iMatchCnt = m_SportsOleReader.GetInt32(6);
					HTMLString.Append("<td><input type=\"hidden\" name=\"matchcount\" value=\"");
					HTMLString.Append(iMatchCnt.ToString());
					HTMLString.Append("\"><input type=\"hidden\" name=\"leaguetype\" value=\"");
					HTMLString.Append(m_SportsOleReader.GetString(7).Trim());
					HTMLString.Append("\"><input type=\"checkbox\" name=\"analysis_players\" value=\"");
					HTMLString.Append(iMatchCnt.ToString());
					HTMLString.Append("\"></td></tr>");
					iRecordsInPage++;
				}
				HTMLString.Append("<tr><td colspan=\"4\">&nbsp&nbsp&nbsp&nbsp提及資料&nbsp<input type=\"text\" name=\"reference\" size=20 maxlength=11 value=\"暫定陣容，只供參考\"</td>");
				HTMLString.Append("<td colspan=\"2\" align=\"center\"><input type=radio name=\"alertTypeRadio\" value=\"0\" onclick=\"detect()\">Option 1");
				HTMLString.Append("&nbsp&nbsp&nbsp<input type=radio name=\"alertTypeRadio\" value=\"1\" onclick=\"detect()\">Option 2</td></tr>");
				m_SportsOleReader.Close();
				m_SportsDBMgr.Close();
			} catch (Exception ex) {
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " JCComboPlayerPreview.cs.PreviewMatches(): " + ex.ToString());
				m_SportsLog.Close();
				HTMLString.Remove(0,HTMLString.Length);
				HTMLString.Append((string)HttpContext.Current.Application["accessErrorMsg"]);
			}

			return HTMLString.ToString();
		}

		public int Send() {
			int iRecUpd = 0, iIdx = 0, iRecordCount = 0, iRecordIdx = 0;
			int iBgCnt = 0;
			int iHistoryCnt = 0;
			int iPlayerCnt = 0;
			int iRemarksCnt = 0;
			string sAction = "";
			string sCurrentTimestamp = null;
			string sBatchJob = null;
			char[] delimiter = new char[] {','};
			string[] arrLeague, arrHost, arrGuest, arrMatchCnt, arrAction, arrLeagType, arrPlayerChecked;
			string[] arrSendToPager;
			string[] arrRemotingPath;
			ArrayList matchCnt_AL = new ArrayList();
			//SportsMessage object message
			SportsMessage sptMsg = new SportsMessage();
			StringBuilder LogSQLString = new StringBuilder();
			DBManager logDBMgr = new DBManager();
			logDBMgr.ConnectionString = m_SportsDBMgr.ConnectionString;
			arrRemotingPath = (string[])HttpContext.Current.Application["RemotingItems"];
			string sReference = "";

			try {
				arrSendToPager = HttpContext.Current.Request.Form["SendToPager"].Split(delimiter);
			} catch(Exception) {
				arrSendToPager = new string[0];
			}
			arrLeague = HttpContext.Current.Request.Form["league"].Split(delimiter);
			arrHost = HttpContext.Current.Request.Form["host"].Split(delimiter);
			arrGuest = HttpContext.Current.Request.Form["guest"].Split(delimiter);
			arrMatchCnt = HttpContext.Current.Request.Form["matchcount"].Split(delimiter);
			arrAction = HttpContext.Current.Request.Form["Action"].Split(delimiter);
			arrLeagType = HttpContext.Current.Request.Form["leaguetype"].Split(delimiter);
			sReference = HttpContext.Current.Request.Form["reference"];

			try {
				arrPlayerChecked = HttpContext.Current.Request.Form["analysis_players"].Split(delimiter);
			}	catch(Exception) {
				arrPlayerChecked = new string[0];
			}

			try {
				/*****************************
				 * GoGo Pager2 alert message *
				 *****************************/
				string[] arrQueueNames;
				string[] arrMessageTypes;
				arrQueueNames = (string[])HttpContext.Current.Application["QueueItems"];
				arrMessageTypes = (string[])HttpContext.Current.Application["NotifyMessageTypes"];
				MessageClient msgClt = new MessageClient();
				msgClt.MessageType = arrMessageTypes[0];
				msgClt.MessagePath = arrQueueNames[0];


				//init INI file
				//string sINIValue = "";
				string[] arrMsgType;
				arrMsgType = (string[])HttpContext.Current.Application["messageType"];

				//copy array element into ArrayList
				iRecordCount = arrMatchCnt.Length;
				for (iIdx=0;iIdx<iRecordCount;iIdx++) {
					matchCnt_AL.Add(arrMatchCnt[iIdx]);
				}
				matchCnt_AL.TrimToSize();

				/***************************
					Send players information
				 ***************************/
				if (arrSendToPager.Length>0 && arrPlayerChecked.Length>0) {
					int iPlayerSubIdx = 0, iPlayerNo = 0, iHostCount = 0, iGuestCount = 0;
					string sTeam = "";
					string[] arrPos;
					arrPos = (string[])HttpContext.Current.Application["positionItemsArray"];

					sCurrentTimestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
					sBatchJob = DateTime.Now.ToString("yyMMddHHmmss_fffffff") + "." + HttpContext.Current.Session["user_name"].ToString() + "." + arrMsgType[9] + ".ini";
					for (iIdx=0; iIdx<arrPlayerChecked.Length; iIdx++) {
						iRecordIdx = matchCnt_AL.IndexOf(arrPlayerChecked[iIdx]);
						//check for Action
						sAction = arrAction[iRecordIdx];

						sTeam = "HOST";
						SQLString.Remove(0,SQLString.Length);
						SQLString.Append("select count(play.CPLAYER_NAME) from GAMEINFO game, JCCOMBO_PLAYERS_INFO play where play.IROSTER=1 and game.match_cnt=");
						SQLString.Append(arrPlayerChecked[iIdx]);
						SQLString.Append(" and play.TEAM_ID=(select TEAM_ID from teaminfo where TEAMNAME=game.");
						SQLString.Append(sTeam);
						SQLString.Append(")");
						iHostCount = m_SportsDBMgr.ExecuteScalar(SQLString.ToString());
						m_SportsDBMgr.Close();

						sTeam = "GUEST";
						SQLString.Remove(0,SQLString.Length);
						SQLString.Append("select count(play.CPLAYER_NAME) from GAMEINFO game, JCCOMBO_PLAYERS_INFO play where play.IROSTER=1 and game.match_cnt=");
						SQLString.Append(arrPlayerChecked[iIdx]);
						SQLString.Append(" and play.TEAM_ID=(select TEAM_ID from teaminfo where TEAMNAME=game.");
						SQLString.Append(sTeam);
						SQLString.Append(")");
						iGuestCount = m_SportsDBMgr.ExecuteScalar(SQLString.ToString());
						m_SportsDBMgr.Close();

						iPlayerSubIdx = 0;
						if (sAction.Equals("D")) {
							iPlayerCnt++;
							iPlayerSubIdx++;
							LogSQLString.Remove(0,LogSQLString.Length);
							LogSQLString.Append("insert into LOG_ANALYSISPLAYERS (TIMEFLAG, IITEMSEQ_NO, SECTION, ACT, LEAGUE, HOST, GUEST, PLAYERTYPE, PLAYERNAME, PLAYERNO, PLAYERPOS, BATCHJOB) values ('");
							LogSQLString.Append(sCurrentTimestamp);
							LogSQLString.Append("',");
							LogSQLString.Append(iPlayerSubIdx.ToString());
							LogSQLString.Append(",'ANALYSISPLAYERS_','D','");
							LogSQLString.Append(arrLeague[iRecordIdx]);
							LogSQLString.Append("','");
							LogSQLString.Append(arrHost[iRecordIdx]);
							LogSQLString.Append("','");
							LogSQLString.Append(arrGuest[iRecordIdx]);
							LogSQLString.Append("','H',null,null,null,'");
							LogSQLString.Append(sBatchJob);
							LogSQLString.Append("')");
							logDBMgr.ExecuteNonQuery(LogSQLString.ToString());
							logDBMgr.Close();
						} else {
							if (iHostCount>0 || iGuestCount>0) {
								iPlayerCnt++;
								//host player retrieval
								sTeam = "HOST";
								SQLString.Remove(0,SQLString.Length);
								SQLString.Append("select play.CPLAYER_NAME, play.IPLAYER_NO, play.IPOS ");
								SQLString.Append("from GAMEINFO game, JCCOMBO_PLAYERS_INFO play ");
								SQLString.Append("where play.IROSTER=1 and game.match_cnt=");
								SQLString.Append(arrPlayerChecked[iIdx]);
								SQLString.Append(" and play.TEAM_ID=(select TEAM_ID from teaminfo where TEAMNAME=game.");
								SQLString.Append(sTeam);
								SQLString.Append(") order by IPOS");
								m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
								while (m_SportsOleReader.Read()) {
									iPlayerSubIdx++;
									LogSQLString.Remove(0,LogSQLString.Length);
									LogSQLString.Append("insert into LOG_ANALYSISPLAYERS (TIMEFLAG, IITEMSEQ_NO, SECTION, ACT, LEAGUE, HOST, GUEST, PLAYERTYPE, PLAYERNAME, PLAYERNO, PLAYERPOS, BATCHJOB) values ('");
									LogSQLString.Append(sCurrentTimestamp);
									LogSQLString.Append("',");
									LogSQLString.Append(iPlayerSubIdx.ToString());
									LogSQLString.Append(",'ANALYSISPLAYERS_','U','");
									LogSQLString.Append(arrLeague[iRecordIdx]);
									LogSQLString.Append("','");
									LogSQLString.Append(arrHost[iRecordIdx]);
									LogSQLString.Append("','");
									LogSQLString.Append(arrGuest[iRecordIdx]);
									LogSQLString.Append("','H','");
									LogSQLString.Append(m_SportsOleReader.GetString(0).Trim());
									LogSQLString.Append("',");
									if(m_SportsOleReader.IsDBNull(1)) iPlayerNo = -1;
									else iPlayerNo = m_SportsOleReader.GetInt32(1);
									LogSQLString.Append(iPlayerNo.ToString());
									LogSQLString.Append(",'");
									LogSQLString.Append(arrPos[m_SportsOleReader.GetInt32(2)]);
									LogSQLString.Append("','");
									LogSQLString.Append(sBatchJob);
									LogSQLString.Append("')");
									logDBMgr.ExecuteNonQuery(LogSQLString.ToString());
									logDBMgr.Close();
								}
								m_SportsOleReader.Close();
								m_SportsDBMgr.Close();

								//guest player retrieval
								iPlayerSubIdx = 0;
								sTeam = "GUEST";
								SQLString.Remove(0,SQLString.Length);
								SQLString.Append("select play.CPLAYER_NAME, play.IPLAYER_NO, play.IPOS ");
								SQLString.Append("from GAMEINFO game, JCCOMBO_PLAYERS_INFO play ");
								SQLString.Append("where play.IROSTER=1 and game.match_cnt=");
								SQLString.Append(arrPlayerChecked[iIdx]);
								SQLString.Append(" and play.TEAM_ID=(select TEAM_ID from teaminfo where TEAMNAME=game.");
								SQLString.Append(sTeam);
								SQLString.Append(") order by IPOS");
								m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
								while(m_SportsOleReader.Read()) {
									iPlayerSubIdx++;
									LogSQLString.Remove(0,LogSQLString.Length);
									LogSQLString.Append("insert into LOG_ANALYSISPLAYERS (TIMEFLAG, IITEMSEQ_NO, SECTION, ACT, LEAGUE, HOST, GUEST, PLAYERTYPE, PLAYERNAME, PLAYERNO, PLAYERPOS, BATCHJOB) values ('");
									LogSQLString.Append(sCurrentTimestamp);
									LogSQLString.Append("',");
									LogSQLString.Append(iPlayerSubIdx.ToString());
									LogSQLString.Append(",'ANALYSISPLAYERS_','U','");
									LogSQLString.Append(arrLeague[iRecordIdx]);
									LogSQLString.Append("','");
									LogSQLString.Append(arrHost[iRecordIdx]);
									LogSQLString.Append("','");
									LogSQLString.Append(arrGuest[iRecordIdx]);
									LogSQLString.Append("','G','");
									LogSQLString.Append(m_SportsOleReader.GetString(0).Trim());
									LogSQLString.Append("',");
									if(m_SportsOleReader.IsDBNull(1)) iPlayerNo = -1;
									else iPlayerNo = m_SportsOleReader.GetInt32(1);
									LogSQLString.Append(iPlayerNo.ToString());
									LogSQLString.Append(",'");
									LogSQLString.Append(arrPos[m_SportsOleReader.GetInt32(2)]);
									LogSQLString.Append("','");
									LogSQLString.Append(sBatchJob);
									LogSQLString.Append("')");
									logDBMgr.ExecuteNonQuery(LogSQLString.ToString());
									logDBMgr.Close();
								}
								// addication reference record
								iPlayerSubIdx++;
								LogSQLString.Remove(0,LogSQLString.Length);
								LogSQLString.Append("insert into LOG_ANALYSISPLAYERS (TIMEFLAG, IITEMSEQ_NO, SECTION, ACT, LEAGUE, HOST, GUEST, PLAYERTYPE, PLAYERNAME, PLAYERNO, PLAYERPOS, BATCHJOB) values ('");
								LogSQLString.Append(sCurrentTimestamp);
								LogSQLString.Append("',");
								LogSQLString.Append(iPlayerSubIdx.ToString());
								LogSQLString.Append(",'ANALYSISPLAYERS_','U','");
								LogSQLString.Append(arrLeague[iRecordIdx]);
								LogSQLString.Append("','");
								LogSQLString.Append(arrHost[iRecordIdx]);
								LogSQLString.Append("','");
								LogSQLString.Append(arrGuest[iRecordIdx]);
								LogSQLString.Append("','G','");
								LogSQLString.Append(sReference);
								LogSQLString.Append("',");
								LogSQLString.Append("-1");
								LogSQLString.Append(",'");
								LogSQLString.Append("資");
								LogSQLString.Append("','");
								LogSQLString.Append(sBatchJob);
								LogSQLString.Append("')");
								logDBMgr.ExecuteNonQuery(LogSQLString.ToString());
								logDBMgr.Close();
								m_SportsOleReader.Close();
								m_SportsDBMgr.Close();
							}
						}
					}

					if (iPlayerCnt > 0) {
						//Send Notify Message
						//Modified by Henry, 10 Feb 2004
						sptMsg.Body = sBatchJob;
						sptMsg.Timestamp = DateTime.Now.ToString("yyMMddHHmmss");
						sptMsg.AppID = "07";
						sptMsg.MsgID = "09";
						sptMsg.DeviceID = new string[0];						
						for (int i=0; i<arrSendToPager.Length; i++) {
							// For World Cup 2006 Only
								sptMsg.AddDeviceID((string)arrSendToPager[i]);
						}					
						try {
							//Notify via MSMQ
							msgClt.MessageType = arrMessageTypes[0];
							msgClt.MessagePath = arrQueueNames[0];
							msgClt.SendMessage(sptMsg);
						} catch (System.Messaging.MessageQueueException mqEx) {
							try {
								m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
								m_SportsLog.SetFileName(0,LOGFILESUFFIX);
								m_SportsLog.Open();
								m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " MSMQ ERROR: Analysis Preview");
								m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " JCComboPlayerPreview.cs.Send(): Notify via MSMQ throws MessageQueueException:  " + mqEx.ToString());
								m_SportsLog.Close();
	
								//If MSMQ fail, notify via .NET Remoting
								msgClt.MessageType = arrMessageTypes[1];
								msgClt.MessagePath = arrRemotingPath[0];
								if (!msgClt.SendMessage((object)sptMsg)) {
									m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
									m_SportsLog.SetFileName(0,LOGFILESUFFIX);
									m_SportsLog.Open();
									m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Remoting ERROR: Analysis Preview");
									m_SportsLog.Close();
								}
							} catch (Exception ex) {
								m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
								m_SportsLog.SetFileName(0,LOGFILESUFFIX);
								m_SportsLog.Open();
								m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Remoting ERROR: Analysis Preview");
								m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " JCComboPlayerPreview.cs.Send(): Notify via .NET Remoting throws exception: " + ex.ToString());
								m_SportsLog.Close();
							}							
						} catch (Exception ex) {
							m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
							m_SportsLog.SetFileName(0,LOGFILESUFFIX);
							m_SportsLog.Open();
							m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " JCComboPlayerPreview.cs.Send(): Notify via MSMQ throws exception: " + ex.ToString());
							m_SportsLog.Close();
						}													
						//Modified end
					}
					
					//write log
					m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
					m_SportsLog.SetFileName(0,LOGFILESUFFIX);
					m_SportsLog.Open();
					m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " JCComboPlayerPreview.cs: Send " + iPlayerCnt.ToString() + " players (" + HttpContext.Current.Session["user_name"] + ")");
					m_SportsLog.Close();
				}
			}	catch(Exception ex) {
				iRecUpd = -1;
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " JCComboPlayerPreview.cs.Send(): " + ex.ToString());
				m_SportsLog.Close();
			}
			iRecUpd = iBgCnt + iHistoryCnt + iPlayerCnt + iRemarksCnt;
			return iRecUpd;
		}
	}
}