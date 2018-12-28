/*
Objective:
Modify Personal Rank (得分/籃板/助攻)

Last updated:
11 Feb 2004 by HenryCi

C#.NET complier statement:
csc /t:library /out:..\..\bin\BSKPersonalRank.dll /r:..\..\bin\DBManager.dll;..\..\bin\Files.dll;..\..\bin\MessageClient.dll;..\..\bin\SportsMessage.dll BSKPersonalRank.cs
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
[assembly:AssemblyCopyright("(c) 2003 TDSL. All rights reserved. Created on 29 Oct 2003.")]
[assembly:AssemblyDescription("籃球資訊 -> 個人統計")]
[assembly:AssemblyProduct("Sports.NET")]
[assembly:AssemblyTitle("Sports.NET DLL")]
[assembly:AssemblyVersion("3.1.*")]

namespace SportsUtil {
	public class BSKPersonalRank {	
		const string LOGFILESUFFIX = "log";	
		const int iRecordNum = 3;
		string sRankName = "";
		OleDbDataReader m_SportsOleReader;	
		DBManager m_SportsDBMgr;
		Files m_SportsLog;
		StringBuilder SQLString;
		
		public BSKPersonalRank(string Connection) {
			m_SportsDBMgr = new DBManager();
			m_SportsDBMgr.ConnectionString = Connection;
			m_SportsLog = new Files();
			SQLString = new StringBuilder();
		}

		public string SetRankName {
			get {
				return 	sRankName;
			}
		}

		public string GetRankPres() {
			string sLeagueID;
			string sCorg = "";	
			object[] TeamNameObj;
			object[] obTeamIdObj;
			object[] PlayerNameobj;
			ArrayList arrTeamIdAL = new ArrayList();
			ArrayList arrChiNameAL = new ArrayList();
			ArrayList arrRankDataAL = new ArrayList();
			StringBuilder HTMLString = new StringBuilder();
			sLeagueID = HttpContext.Current.Request.QueryString["leagID"].Trim();

			//get the cleague and CORG from LEAGUE_INFO 
			SQLString.Remove(0,SQLString.Length);			
			SQLString.Append("Select cleague, CORG from league_info where cleag_id='");
			SQLString.Append(sLeagueID);
			SQLString.Append("'");
			try {
				m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
				if(m_SportsOleReader.Read()) {
					sRankName = m_SportsOleReader.GetString(0).Trim();
					if(!m_SportsOleReader.IsDBNull(1)) sCorg = m_SportsOleReader.GetString(1).Trim();
				}
				m_SportsDBMgr.Close();
				m_SportsOleReader.Close();
				//End the get  league_info

				//get present rank
				SQLString.Remove(0,SQLString.Length);
				SQLString.Append("select CTeam_ID, CCHI_NAME, CRANK_DATA FROM RANKPERSONAL_INFO WHERE CLEAG_ID = '");
				SQLString.Append(sLeagueID);
				SQLString.Append("' order by IRANK");
				m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
				while(m_SportsOleReader.Read()) {
					arrTeamIdAL.Add(m_SportsOleReader.GetString(0).Trim());
					arrChiNameAL.Add(m_SportsOleReader.GetString(1).Trim());
					if(!m_SportsOleReader.IsDBNull(2)) {
						arrRankDataAL.Add(m_SportsOleReader.GetString(2).Trim());
					}
				}
				m_SportsDBMgr.Close();
				m_SportsOleReader.Close();
				arrTeamIdAL.TrimToSize();
				arrChiNameAL.TrimToSize();			
				arrRankDataAL.TrimToSize();		
				TeamNameObj = getTeamNameArray(out obTeamIdObj);

				for(int index = 0;index < iRecordNum;index++ ) {
					HTMLString.Append("<TR ALIGN=\"CENTER\"><TD >");
					HTMLString.Append((index+1).ToString());
					HTMLString.Append("<INPUT NAME=\"RANK\" TYPE=\"HIDDEN\" VALUE=\"");
					HTMLString.Append((index+1).ToString());
					HTMLString.Append("\"></TD>");	

					//球隊
					HTMLString.Append("<TD><SELECT NAME=\"Team\" onchange=\"changeList");
					HTMLString.Append(index);
					HTMLString.Append("(this)\"><OPTION VALUE=\"\">選擇球隊");
					if(obTeamIdObj.Length>0) {
						for(int iteamindex =0; iteamindex < obTeamIdObj.Length;iteamindex++) {
							HTMLString.Append( "<OPTION VALUE=\"");
							HTMLString.Append(obTeamIdObj[iteamindex].ToString());			
							if((arrTeamIdAL.Count > 0)&&(index < arrTeamIdAL.Count)) {
								if(obTeamIdObj[iteamindex].ToString()==arrTeamIdAL[index].ToString()) {
									HTMLString.Append("\" selected>");
									HTMLString.Append(TeamNameObj[iteamindex].ToString());
								} else {
									HTMLString.Append("\">");
									HTMLString.Append(TeamNameObj[iteamindex].ToString());
								}
							} else {
								HTMLString.Append("\">");	
								HTMLString.Append(TeamNameObj[iteamindex].ToString());	
							}								
						}
					} //end if obTeamIdObj
					HTMLString.Append("</select></TD>");

					// 球員
					HTMLString.Append("<TD><SELECT NAME=\"player");
					HTMLString.Append(index);
					HTMLString.Append("\"><OPTION VALUE=\"\">選擇球員");
					if(arrTeamIdAL.Count>0) {
						if((arrTeamIdAL.Count > 0)&&(index < arrTeamIdAL.Count)) {
							PlayerNameobj = GetPlayerNameArray(arrTeamIdAL[index].ToString());
							for(int iplayerindex = 0;iplayerindex < PlayerNameobj.Length;iplayerindex++ ){
								HTMLString.Append( "<OPTION VALUE=\"");
								if(PlayerNameobj[iplayerindex].ToString()==arrChiNameAL[index].ToString()) {
									HTMLString.Append(PlayerNameobj[iplayerindex].ToString());
									HTMLString.Append("\" selected>");
								} else {
									HTMLString.Append(PlayerNameobj[iplayerindex].ToString()); 
									HTMLString.Append("\" >");
								}
								HTMLString.Append(PlayerNameobj[iplayerindex].ToString());
								HTMLString.Append("&nbsp");
							}
						}
					}
					HTMLString.Append("</select>&nbsp</TD>");

					//平均得分	
					if(index < arrRankDataAL.Count) {
						if(arrRankDataAL[index].Equals("-1")) {
							arrRankDataAL[index] = "";
						}
						HTMLString.Append("<TD><INPUT NAME=\"RANKDATA\"  VALUE=\"");
						HTMLString.Append(arrRankDataAL[index].ToString());
						HTMLString.Append("\" TYPE=\"TEXT\" SIZE=\"10\" MAXLENGTH=\"5\"></TD></TR>");
					} else  {
						HTMLString.Append("<TD><INPUT NAME=\"RANKDATA\"  VALUE=\"\" TYPE=\"TEXT\" SIZE=\"10\" MAXLENGTH=\"5\"></TD></TR>");
					}
				} //end for
				HTMLString.Append("<INPUT NAME=\"LeagueId\" TYPE=\"HIDDEN\" VALUE=\"");
				HTMLString.Append(sLeagueID);
				HTMLString.Append("\"><INPUT NAME=\"CORG\" TYPE=\"HIDDEN\" VALUE=\"");
				HTMLString.Append(sCorg);
				HTMLString.Append("\">");	
			}	catch(Exception ex) {
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " BSKPersonalRank.cs.GetRankPres(): " + ex.ToString());
				m_SportsLog.Close();
				HTMLString.Remove(0,HTMLString.Length);
				HTMLString.Append((string)HttpContext.Current.Application["accessErrorMsg"]);
			}

			return HTMLString.ToString();
		}

		public int AddRankPres() {		
			int iRecUpd = 0	;
			string sLeagueID;
			string sPresonalType;
			string sPlayer_Name;
			string sRankData = "-1";
			string sCurrentTimestamp = null;
			string sBatchJob = null;
			string[] arrSendToPager;			
			string[] arrRankNo;
			string[] arrRankData;
			string[] arrMsgType;
			string[] arrTeamId;			
			string sTeamName = "-1" ;	
			string[] arrRemotingPath;
			char[] delimiter = new char[] {','};		


			//SportsMessage object message
			SportsMessage sptMsg = new SportsMessage();
			StringBuilder LogSQLString = new StringBuilder();
			DBManager logDBMgr = new DBManager();
			logDBMgr.ConnectionString = m_SportsDBMgr.ConnectionString;
			
			sPresonalType = HttpContext.Current.Request.Form["CORG"];
			sLeagueID = HttpContext.Current.Request.Form["LeagueId"];
			arrRankNo = HttpContext.Current.Request.Form["RANK"].Split(delimiter);
			arrRankData = HttpContext.Current.Request.Form["RANKDATA"].Split(delimiter);
			arrTeamId = HttpContext.Current.Request.Form["Team"].Split(delimiter);
			try {
				arrSendToPager = HttpContext.Current.Request.Form["SendToPager"].Split(delimiter);
			} catch(Exception) {
				arrSendToPager = new string[0];
			}
			
			try {	
				if(sPresonalType.Equals("")) sPresonalType = "-1";

				SQLString.Remove(0,SQLString.Length);			
				SQLString.Append("delete from RANKPERSONAL_INFO where CLEAG_ID='");
				SQLString.Append(sLeagueID);
				SQLString.Append("'");
				m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
				m_SportsDBMgr.Close();

				/*****************************
				 * GoGo Pager2 alert message *
				 *****************************/					
				string[] arrQueueNames;
				string[] arrMessageTypes;
				arrMsgType = (string[])HttpContext.Current.Application["messageType"];

				sCurrentTimestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
				sBatchJob = DateTime.Now.ToString("yyMMddHHmmss_fffffff") + "." + HttpContext.Current.Session["user_name"].ToString() + "." + arrMsgType[32] + ".ini";
				arrQueueNames = (string[])HttpContext.Current.Application["QueueItems"];
				arrMessageTypes = (string[])HttpContext.Current.Application["NotifyMessageTypes"];
				arrRemotingPath = (string[])HttpContext.Current.Application["RemotingItems"];
				MessageClient msgClt = new MessageClient();
				msgClt.MessageType = arrMessageTypes[0];
				msgClt.MessagePath = arrQueueNames[0];

				for(int index = 0;index < arrRankNo.Length ;index++) {	
					sPlayer_Name = HttpContext.Current.Request.Form["PLAYER"+index.ToString()];	
					if(arrRankData[index].Equals("")) sRankData = "-1";
					else sRankData = arrRankData[index];			
					if(!arrTeamId[index].Equals("")) {
						SQLString.Remove(0,SQLString.Length);
						SQLString.Append("insert into RANKPERSONAL_INFO (CLEAG_ID,CTEAM_ID,CCHI_NAME ,IRANK,CRANK_DATA ) values ('");
						SQLString.Append(sLeagueID);
						SQLString.Append("', '");
						SQLString.Append(arrTeamId[index].ToString());
						SQLString.Append(" ', '");
						SQLString.Append(sPlayer_Name);
						SQLString.Append(" ', ");
						SQLString.Append(Convert.ToInt32(arrRankNo[index]));
						SQLString.Append(",'");
						SQLString.Append(sRankData);
						SQLString.Append("')");
						m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
						m_SportsDBMgr.Close();

						SQLString.Remove(0,SQLString.Length);
						SQLString.Append("select cteam from team_info where cteam_id='");
						SQLString.Append(arrTeamId[index].ToString());
						SQLString.Append("'");
						m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
						if(m_SportsOleReader.Read()) {
							sTeamName = m_SportsOleReader.GetString(0).Trim();
						}
						m_SportsOleReader.Close();
						m_SportsDBMgr.Close();

						if(arrSendToPager.Length > 0) {
							//insert into LOG_BSK_RANK
							++iRecUpd;	
							LogSQLString.Remove(0,LogSQLString.Length);
							LogSQLString.Append("insert into LOG_BSK_RANK (TIMEFLAG,IITEMSEQ_NO,Section,TYPE_NAME,ACT,RANKNO,PARAM_1,PARAM_2,PARAM_3,PARAM_4,PARAM_5,BATCHJOB) values('");
							LogSQLString.Append(sCurrentTimestamp);
							LogSQLString.Append("','");
							LogSQLString.Append(iRecUpd);
							LogSQLString.Append("','RANK_','");
							LogSQLString.Append(sPresonalType);
							LogSQLString.Append("','U','");
							LogSQLString.Append(arrRankNo[index]);
							LogSQLString.Append("','");
							LogSQLString.Append(sTeamName);
							LogSQLString.Append("','");
							LogSQLString.Append(sPlayer_Name);
							LogSQLString.Append("','");
							LogSQLString.Append(sRankData);
							LogSQLString.Append("',null,null,'");
							LogSQLString.Append(sBatchJob);
							LogSQLString.Append("')");
							logDBMgr.ExecuteNonQuery(LogSQLString.ToString());
							logDBMgr.Close();	
						}				
					} //End insert into LOG_BSK_RANK
				}

				//send message to the msmq
				if(iRecUpd > 0) {
					//Modified by Henry, 11 Feb 2004
					sptMsg.Body = sBatchJob;
					sptMsg.Timestamp = DateTime.Now.ToString("yyMMddHHmmss");
					sptMsg.AppID = "07";
					sptMsg.MsgID = "29";	//29為定義個人排名
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
							m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " MSMQ ERROR: BSK Personal Rank");
							m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " BSKPersonalRank.cs.AddRankPres(): Notify via MSMQ throws MessageQueueException:  " + mqEx.ToString());
							m_SportsLog.Close();

							//If MSMQ fail, notify via .NET Remoting
							msgClt.MessageType = arrMessageTypes[1];
							msgClt.MessagePath = arrRemotingPath[0];
							if(!msgClt.SendMessage((object)sptMsg)) {
								m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
								m_SportsLog.SetFileName(0,LOGFILESUFFIX);
								m_SportsLog.Open();
								m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Remoting ERROR: BSK Personal Rank");
								m_SportsLog.Close();
							}
						}	catch(Exception ex) {
							m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
							m_SportsLog.SetFileName(0,LOGFILESUFFIX);
							m_SportsLog.Open();
							m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Remoting ERROR: BSK Personal Rank");
							m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " BSKPersonalRank.cs.AddRankPres(): Notify via .NET Remoting throws exception: " + ex.ToString());
							m_SportsLog.Close();
						}							
					}	catch(Exception ex) {
						m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
						m_SportsLog.SetFileName(0,LOGFILESUFFIX);
						m_SportsLog.Open();
						m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " BSKPersonalRank.cs.AddRankPres(): Notify via MSMQ throws exception: " + ex.ToString());
						m_SportsLog.Close();
					}	
					//Modified end
				}

				//write log
				m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " BSKPersonalRank.cs: Update Personal Rank, " + iRecUpd.ToString() + " records. (" + HttpContext.Current.Session["user_name"] + ")");
				m_SportsLog.Close();
				m_SportsDBMgr.Dispose();
			} catch(Exception ex) {
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " BSKPersonalRank.cs.AddRankPres(): " + ex.ToString());
				m_SportsLog.Close();
			}

			return iRecUpd;				
		}

		public object[] getTeamNameArray(out object[] Array) {	//得到所有存在的Team Name, team id (JavaScript code segment)
			ArrayList arrTeam_NameAL,arrTeam_IdAL;
			object[] AliasArray;

			arrTeam_NameAL = new ArrayList();
			arrTeam_IdAL = new ArrayList();
			SQLString.Remove(0,SQLString.Length);
			SQLString.Append("select distinct team.Cteam,team.cteam_id from team_info team,league_info league,idmap_info map where team.cteam_id=map.cteam_id and league.cleag_id=map.cleag_id");
			m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
			while(m_SportsOleReader.Read()) {
				arrTeam_NameAL.Add(m_SportsOleReader.GetString(0).Trim());
				arrTeam_IdAL.Add(m_SportsOleReader.GetString(1).Trim());
			}
			m_SportsOleReader.Close();
			m_SportsDBMgr.Close();
			arrTeam_NameAL.TrimToSize();
			arrTeam_IdAL.TrimToSize();
			AliasArray = new object[arrTeam_NameAL.Count];
			AliasArray = arrTeam_NameAL.ToArray();
			Array = new object[arrTeam_IdAL.Count];
			Array = arrTeam_IdAL.ToArray();

			return AliasArray;
		}

		public object[] GetPlayerNameArray(string strTeamID) {	// 輸入一個小組ID得到一組Player (JavaScript code segment)
			object[] PlayerArray;
			ArrayList PlayerAL;		
			PlayerAL = new ArrayList();			
			SQLString.Remove(0,SQLString.Length);
			SQLString.Append("select distinct player.CCHI_NAME FROM PLAYERS_INFO PLAYER,IDMAP_INFO MAP WHERE PLAYER.CTEAM_ID=MAP.CTEAM_ID AND PLAYER.CTEAM_ID='");
			SQLString.Append(strTeamID);
			SQLString.Append("'");
			m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
			while(m_SportsOleReader.Read()) {
				PlayerAL.Add(m_SportsOleReader.GetString(0).Trim());
			}
			m_SportsOleReader.Close();
			m_SportsDBMgr.Close();
			PlayerAL.TrimToSize();
			PlayerArray = new object[PlayerAL.Count];
			PlayerArray = PlayerAL.ToArray();

			return PlayerArray;
		}

		public string GetJavaScript() {
			int[] iArrayLength;
			object[] arrTeamIDobj;
			object[] arrPlayerNameobj;
			object[] sTeamNameStr;
			StringBuilder JavaScriptString = new StringBuilder();

			getTeamNameArray(out arrTeamIDobj);
			object[][] objName = new object[arrTeamIDobj.Length][];	//申請一個不規則數組
			iArrayLength = new int[arrTeamIDobj.Length];
			sTeamNameStr = new object[arrTeamIDobj.Length];
			JavaScriptString.Append("var lists = new Array();");
			for(int i = 0; i < arrTeamIDobj.Length; i++ ) {
				sTeamNameStr[i] = "";
				arrPlayerNameobj = GetPlayerNameArray(arrTeamIDobj[i].ToString());
				iArrayLength[i] = arrPlayerNameobj.Length;
				if(iArrayLength[i] > 0) {
					objName[i] = new object[arrPlayerNameobj.Length];//給不規則數組的一維確定需要的個數
					for(int j = 0; j < arrPlayerNameobj.Length; j++) {
						objName[i][j] = arrPlayerNameobj[j];
					} 
					if(arrPlayerNameobj.Length-1>0) {
						for(int iTeamNameNum = 0;iTeamNameNum< arrPlayerNameobj.Length-1;iTeamNameNum++) {
							sTeamNameStr[i] += "'" + arrPlayerNameobj[iTeamNameNum] + "',";
						}
						sTeamNameStr[i] += "'" + arrPlayerNameobj[arrPlayerNameobj.Length-1] + "'";
					} else {
						sTeamNameStr[i] = "'" + arrPlayerNameobj[0] + "'";
					}
					JavaScriptString.Append("lists['");
					JavaScriptString.Append(arrTeamIDobj[i].ToString());
					JavaScriptString.Append("'] = new Array(");
					JavaScriptString.Append(sTeamNameStr[i].ToString());
					JavaScriptString.Append(");");
				} else {
					objName[i] = new object[1];
					objName[i][0] = "暫無球員";
					JavaScriptString.Append("lists['");
					JavaScriptString.Append(arrTeamIDobj[i].ToString());
					JavaScriptString.Append("'] = new Array('暫無球員');");
				}					
			}
			JavaScriptString.Append("lists[''] = new Array('選擇球員');");

			return JavaScriptString.ToString();
		}
	}
}