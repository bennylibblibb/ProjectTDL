/*
Objective:
modify Rank (League)

Last updated:
11 Feb 2004 by HenryCi

C#.NET complier statement:
csc /t:library /out:..\..\bin\BSKGroupRank.dll /r:..\..\bin\DBManager.dll;..\..\bin\Files.dll;..\..\bin\MessageClient.dll;..\..\bin\SportsMessage.dll BSKGroupRank.cs
*/

using System;
using System.Data.OleDb;
using System.Reflection;
using System.Text;
using System.Web;
using TDL.DB;
using TDL.IO;
using TDL.Message;

[assembly:AssemblyCompany("TDSL")]
[assembly:AssemblyCopyright("(c) 2003 TDSL. All rights reserved.")]
[assembly:AssemblyDescription("籃球資訊 -> 小組排名")]
[assembly:AssemblyProduct("Sports.NET")]
[assembly:AssemblyTitle("Sports.NET DLL")]
[assembly:AssemblyVersion("3.1.*")]
namespace SportsUtil {
	public class BSKGroupRank {	
		const string LOGFILESUFFIX = "log";	
		string m_LeagueName;
		int m_RecordCount;
		OleDbDataReader m_SportsOleReader;	
		DBManager m_SportsDBMgr;
		Files m_SportsLog;
		StringBuilder SQLString;

		public BSKGroupRank(string Connection) {
			m_SportsDBMgr = new DBManager();
			m_SportsDBMgr.ConnectionString = Connection;
			m_SportsLog = new Files();
			SQLString = new StringBuilder();
			m_LeagueName = "";
			m_RecordCount = 0;
		}

		public string SetLeaglong {
			get {
				return m_LeagueName;
			}
		}

		public int NumberOfRecords {			
			get {
				return m_RecordCount;
			}
		}

		public string GetRankLeag() {
			string sLeagID;
			string sRankType = "";
			StringBuilder HTMLString = new StringBuilder();	
			sLeagID = HttpContext.Current.Request.QueryString["leagID"].Trim();

			try {
				SQLString.Remove(0,SQLString.Length);
				SQLString.Append("select leag.CLEAGUE, leag.CORG, rank.IRANK, team.CTEAM, team.CTEAM_ID, rank.IWIN, rank.ILOSS, rank.ICONSECTIVE_TYPE, rank.ICONSECTIVE from LEAGUE_INFO leag LEFT OUTER JOIN TEAM_INFO team ON team.CTEAM_ID in (select CTEAM_ID from IDMAP_INFO where CLEAG_ID='");
				SQLString.Append(sLeagID);
				SQLString.Append("') LEFT OUTER JOIN RANK_LEAGUE_INFO rank ON rank.CLEAG_ID=leag.CLEAG_ID and rank.CLEAG_ID='");
				SQLString.Append(sLeagID);
				SQLString.Append("' and rank.CTEAM_ID=team.CTEAM_ID ORDER BY rank.IRANK");
				m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
				while(m_SportsOleReader.Read()) {
					if(!m_SportsOleReader.IsDBNull(0)) {
						if(m_LeagueName.Equals("")) {
							m_LeagueName = m_SportsOleReader.GetString(0).Trim();
						}
					}

					if(!m_SportsOleReader.IsDBNull(1)) {
						if(sRankType.Equals("")) {
							sRankType = m_SportsOleReader.GetString(1).Trim();
						}
					}

					//排名
					HTMLString.Append("<tr align=\"center\"><TD><INPUT NAME=\"RankNumber\" TYPE=\"TEXT\" SIZE=\"3\" MAXLENGTH=\"2\" VALUE=\"");
					if(!m_SportsOleReader.IsDBNull(2)) {
						if(m_SportsOleReader.GetInt32(2) != -1) {
							HTMLString.Append(m_SportsOleReader.GetInt32(2));
						}
					}
					HTMLString.Append("\"></TD>");

					//Team name and ID
					HTMLString.Append("<TD>");
					HTMLString.Append(m_SportsOleReader.GetString(3).Trim());
					HTMLString.Append("</TD>");
					HTMLString.Append("<INPUT NAME=\"TEAMName\" TYPE=\"HIDDEN\" VALUE=\"");
					HTMLString.Append(m_SportsOleReader.GetString(3).Trim());
					HTMLString.Append("\"><INPUT NAME=\"TEAMID\" TYPE=\"HIDDEN\" VALUE=\"");
					HTMLString.Append(m_SportsOleReader.GetString(4).Trim());
					HTMLString.Append("\">");

					//勝/敗
					HTMLString.Append("<TD><INPUT NAME=\"WIN\" TYPE=\"TEXT\" VALUE=\"");
					if(!m_SportsOleReader.IsDBNull(5)) {
						if(m_SportsOleReader.GetInt32(5) != -1) {
							HTMLString.Append(m_SportsOleReader.GetInt32(5));
						}
					}
					HTMLString.Append("\" SIZE=\"4\" MAXLENGTH=\"2\">/<INPUT NAME=\"LOSS\" TYPE=\"TEXT\" VALUE=\"");
					if(!m_SportsOleReader.IsDBNull(6)) {
						if(m_SportsOleReader.GetInt32(6) != -1) {
							HTMLString.Append(m_SportsOleReader.GetInt32(6));
						}
					}
					HTMLString.Append("\" SIZE=\"4\" MAXLENGTH=\"2\"></TD>");

					//連勝/敗
					HTMLString.Append("<TD>連勝<INPUT TYPE=\"RADIO\" NAME=\"TYPE");
					HTMLString.Append(m_RecordCount.ToString());
					HTMLString.Append("\" VALUE=\"0\"");
					if(!m_SportsOleReader.IsDBNull(7)) {
						if(m_SportsOleReader.GetInt32(7) != 1) {
							HTMLString.Append(" checked");
						}
					} else {
						HTMLString.Append(" checked");	//Default value
					}
					HTMLString.Append(">/連敗<INPUT TYPE=\"RADIO\" NAME=\"TYPE");
					HTMLString.Append(m_RecordCount.ToString());
					HTMLString.Append("\" VALUE=\"1\"");
					if(!m_SportsOleReader.IsDBNull(7)) {
						if(m_SportsOleReader.GetInt32(7) == 1) {
							HTMLString.Append(" checked");
						}
					}
					HTMLString.Append("><INPUT NAME=\"DATA\" TYPE=\"TEXT\" VALUE=\"");
					if(!m_SportsOleReader.IsDBNull(8)) {
						if(m_SportsOleReader.GetInt32(8) != -1) {
							HTMLString.Append(m_SportsOleReader.GetInt32(8));
						}
					}
					HTMLString.Append("\" SIZE=\"3\" MAXLENGTH=\"2\"></TD></tr>");

					m_RecordCount++;
				}
				m_SportsOleReader.Close();
				m_SportsDBMgr.Close();
				m_SportsDBMgr.Dispose();

				HTMLString.Append("<INPUT NAME=\"RankType\" TYPE=\"HIDDEN\" VALUE=\"");
				HTMLString.Append(sRankType);
				HTMLString.Append("\"><INPUT NAME=\"leagueID\" TYPE=\"HIDDEN\" VALUE=\"");
				HTMLString.Append(sLeagID);
				HTMLString.Append("\">");
			}	catch(Exception ex) {
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " BSKGroupRank.cs.GetRankLeag(): " + ex.ToString());
				m_SportsLog.Close();
				HTMLString.Remove(0,HTMLString.Length);
				HTMLString.Append((string)HttpContext.Current.Application["accessErrorMsg"]);
			}

			return HTMLString.ToString();
		}

		public int AddRankLeag() {
			int iRecUpd = 0;
			int iRankId = -1;
			int iWin = -1;
			int iLoss = -1;
			int iData = -1;
			string sLeagId;
			string sType = "1";
			string[] arrSendToPager;
			string sRankType;
			string sCurrentTimestamp = null;
			string sBatchJob = null;
			char[] delimiter = new char[] {','};
			string[] arrRankNumber;
			string[] arrTeamId;
			string[] arrWin;
			string[] arrLoss;
			string[] arrData;
			string[] arrMsgType;
			string[] arrTeamName;
			string[] arrRemotingPath;

			//SportsMessage object message
			SportsMessage sptMsg = new SportsMessage();
			StringBuilder LogSQLString = new StringBuilder();
			DBManager logDBMgr = new DBManager();
			logDBMgr.ConnectionString = m_SportsDBMgr.ConnectionString;
			try {
				arrSendToPager = HttpContext.Current.Request.Form["SendToPager"].Split(delimiter);
			} catch(Exception) {
				arrSendToPager = new string[0];
			}

			sRankType = HttpContext.Current.Request.Form["RankType"];
			sLeagId = HttpContext.Current.Request.Form["leagueID"];
			arrRankNumber = HttpContext.Current.Request.Form["RankNumber"].Split(delimiter);
			arrTeamId = HttpContext.Current.Request.Form["TEAMID"].Split(delimiter);
			arrWin = HttpContext.Current.Request.Form["WIN"].Split(delimiter);
			arrLoss = HttpContext.Current.Request.Form["LOSS"].Split(delimiter);
			arrData = HttpContext.Current.Request.Form["DATA"].Split(delimiter);
			arrTeamName = HttpContext.Current.Request.Form["TEAMName"].Split(delimiter);
			arrRemotingPath = (string[])HttpContext.Current.Application["RemotingItems"];

			try {
				if(sRankType.Equals("")) sRankType = "-1";

				SQLString.Remove(0,SQLString.Length);
				SQLString.Append("delete from RANK_LEAGUE_INFO where CLEAG_ID='");
				SQLString.Append(sLeagId);
				SQLString.Append("'");
				m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
				m_SportsDBMgr.Close();

				/*****************************
				 * GoGo Pager2 alert message *
				 *****************************/					
				string[] arrQueueNames;
				string[] arrMessageTypes;
				arrMsgType = (string[])HttpContext.Current.Application["messageType"];
				arrQueueNames = (string[])HttpContext.Current.Application["QueueItems"];
				arrMessageTypes = (string[])HttpContext.Current.Application["NotifyMessageTypes"];
				MessageClient msgClt = new MessageClient();
				msgClt.MessageType = arrMessageTypes[0];
				msgClt.MessagePath = arrQueueNames[0];

				for(int index = 0; index < arrRankNumber.Length; index++) {
					sType = HttpContext.Current.Request.Form["Type" + index.ToString()];
					if(arrRankNumber[index].Equals("")) {
						iRankId = -1;
					} else {
						iRankId = Convert.ToInt32(arrRankNumber[index]);
					}

					if(arrWin[index].Equals("")) iWin = -1;
					else iWin = Convert.ToInt32(arrWin[index]);	

					if(arrLoss[index].Equals("")) iLoss = -1;
					else iLoss = Convert.ToInt32(arrLoss[index]);

					if(arrData[index].Equals("")) iData = -1;
					else iData = Convert.ToInt32(arrData[index]);	

					if(iRankId != -1) {
						SQLString.Remove(0,SQLString.Length);
						SQLString.Append("insert into RANK_LEAGUE_INFO (CLEAG_ID,CTEAM_ID,CACTION,IRANK,IWIN,ILOSS,ICONSECTIVE,ICONSECTIVE_TYPE ) values(");
						SQLString.Append("'");
						SQLString.Append(sLeagId);
						SQLString.Append("','");
						SQLString.Append(arrTeamId[index]);
						SQLString.Append("','U',");
						SQLString.Append(iRankId);
						SQLString.Append(",");
						SQLString.Append(iWin);
						SQLString.Append(",");
						SQLString.Append(iLoss);
						SQLString.Append(",");
						SQLString.Append(iData);
						SQLString.Append(",");
						SQLString.Append(Convert.ToInt32(sType));
						SQLString.Append(" )");
						m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
						m_SportsDBMgr.Close();
					}
				}

				if(arrSendToPager.Length > 0) {
					sCurrentTimestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
					sBatchJob = DateTime.Now.ToString("yyMMddHHmmss_fffffff") + "." + HttpContext.Current.Session["user_name"].ToString() + "." + arrMsgType[31] + ".ini";

					SQLString.Remove(0,SQLString.Length);
					SQLString.Append("select rank.IRANK, team.CTEAM, rank.IWIN, rank.ILOSS, rank.ICONSECTIVE, rank.ICONSECTIVE_TYPE from RANK_LEAGUE_INFO rank, LEAGUE_INFO leag, TEAM_INFO team where rank.CACTION='U' and rank.CLEAG_ID=leag.CLEAG_ID and rank.CTEAM_ID=team.CTEAM_ID and rank.CLEAG_ID='");
					SQLString.Append(sLeagId);
					SQLString.Append("' order by rank.IRANK");
					m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
					while(m_SportsOleReader.Read()) {	//insert into LOG_BSK_RANK
						iRecUpd++;
						LogSQLString.Remove(0,LogSQLString.Length);
						LogSQLString.Append("insert into LOG_BSK_RANK (TIMEFLAG,IITEMSEQ_NO,Section,TYPE_NAME,ACT,RANKNO,PARAM_1,PARAM_2,PARAM_3,PARAM_4,PARAM_5,BATCHJOB) values('");
						LogSQLString.Append(sCurrentTimestamp);
						LogSQLString.Append("','");
						LogSQLString.Append(iRecUpd.ToString());
						LogSQLString.Append("','RANK_','");
						LogSQLString.Append(sRankType);
						LogSQLString.Append("','U','");
						LogSQLString.Append(m_SportsOleReader.GetInt32(0).ToString());
						LogSQLString.Append("','");
						LogSQLString.Append(m_SportsOleReader.GetString(1).Trim());
						LogSQLString.Append("','");
						LogSQLString.Append(m_SportsOleReader.GetInt32(2).ToString());
						LogSQLString.Append("','");
						LogSQLString.Append(m_SportsOleReader.GetInt32(3).ToString());
						LogSQLString.Append("','");
						LogSQLString.Append(m_SportsOleReader.GetInt32(4).ToString());
						LogSQLString.Append("','");
						if(m_SportsOleReader.GetInt32(5) == 0) {
							if(m_SportsOleReader.GetInt32(4) > 1) LogSQLString.Append("連勝");
							else LogSQLString.Append("勝");
						} else {
							if(m_SportsOleReader.GetInt32(4) > 1) LogSQLString.Append("連負");
							else LogSQLString.Append("負");
						}
						LogSQLString.Append("','");
						LogSQLString.Append(sBatchJob);
						LogSQLString.Append("')");
						logDBMgr.ExecuteNonQuery(LogSQLString.ToString());
						logDBMgr.Close();
					}	// end insert into LOG_BSK_RANK

					//send message to the msmq
					//Modified by Henry, 11 Feb 2004
					if(iRecUpd > 0) {
						sptMsg.Body = sBatchJob;
						sptMsg.Timestamp = DateTime.Now.ToString("yyMMddHHmmss");
						sptMsg.AppID = "07";
						sptMsg.MsgID = "29"; //29為定義小組排名
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
								m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " MSMQ ERROR:Basketball Group Rank");
								m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " BSKGroupRank.cs.AddRankLeag(): Notify via MSMQ throws MessageQueueException:  " + mqEx.ToString());
								m_SportsLog.Close();

								//If MSMQ fail, notify via .NET Remoting
								msgClt.MessageType = arrMessageTypes[1];
								msgClt.MessagePath = arrRemotingPath[0];
								if(!msgClt.SendMessage((object)sptMsg)) {
									m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
									m_SportsLog.SetFileName(0,LOGFILESUFFIX);
									m_SportsLog.Open();
									m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Remoting ERROR:Basketball Group Rank");
									m_SportsLog.Close();
								}
							}	catch(Exception ex) {
								m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
								m_SportsLog.SetFileName(0,LOGFILESUFFIX);
								m_SportsLog.Open();
								m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Remoting ERROR:Basketball Group Rank");
								m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " BSKGroupRank.cs.AddRankLeag(): Notify via .NET Remoting throws exception: " + ex.ToString());
								m_SportsLog.Close();
							}							
						}	catch(Exception ex) {
							m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
							m_SportsLog.SetFileName(0,LOGFILESUFFIX);
							m_SportsLog.Open();
							m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " BSKGroupRank.cs.AddRankLeag(): Notify via MSMQ throws exception: " + ex.ToString());
							m_SportsLog.Close();
						}						
						//Modified end
					}
				}

				//write log
				m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " BSKGroupRank.cs: Update group rank, " + iRecUpd.ToString() + " records. (" + HttpContext.Current.Session["user_name"] + ")");
				m_SportsLog.Close();
				m_SportsDBMgr.Dispose();
			} catch(Exception ex) {
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " BSKGroupRank.cs.AddRankLeag(): " + ex.ToString());
				m_SportsLog.Close();
			}

			return iRecUpd;
		}
	}
}