/*
Objective:
modify all match (odds)

Last updated:
12 July 2004, Chapman Choi

C#.NET complier statement:
csc /t:library /out:..\..\bin\BSKAllMatch.dll /r:..\..\bin\DBManager.dll;..\..\bin\Files.dll;..\..\bin\MessageClient.dll;..\..\bin\SportsMessage.dll BSKAllMatch.cs
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
[assembly:AssemblyCopyright("(c) 2003 TDSL. All rights reserved. Created on 24 Oct 2003.")]
[assembly:AssemblyDescription("籃球資訊 -> 所有賽事")]
[assembly:AssemblyProduct("Sports.NET")]
[assembly:AssemblyTitle("Sports.NET DLL")]
[assembly:AssemblyVersion("3.1.*")]
namespace SportsUtil {
	public class BSKAllMatch {
		const string LOGFILESUFFIX = "log";
		int m_RecordCount = 0;
		OleDbDataReader m_SportsOleReader;
		DBManager m_SportsDBMgr;
		Files m_SportsLog;
		StringBuilder SQLString;

		public BSKAllMatch(string Connection) {
			m_SportsDBMgr = new DBManager();
			m_SportsDBMgr.ConnectionString = Connection;
			m_SportsLog = new Files();
			SQLString = new StringBuilder();
		}

		public int NumberOfRecords {
			get {
				return m_RecordCount;
			}
		}

		public string GetAllMatches() {
			int iArrayIndex = 0;
			int iRecordStr = 0;
			string sRecordStr = "";
			string sSortType;
			string[] arrOddsState;
			StringBuilder HTMLString = new StringBuilder();
			arrOddsState = (string[])HttpContext.Current.Application["oddsItemsArray"];
			sSortType = HttpContext.Current.Request.QueryString["sort"];
			if(sSortType != null) {
				if(sSortType.Trim().Equals("")) {
					sSortType = (string)HttpContext.Current.Session["user_sortType"];
				} else {
					HttpContext.Current.Session["user_sortType"] = sSortType;
				}
			} else {
				sSortType = (string)HttpContext.Current.Session["user_sortType"];
			}

			try {
				SQLString.Remove(0,SQLString.Length);			
				SQLString.Append("select ISEQ_NO,CMATCH_DATE,CMATCH_TIME,CLEAG,CHOST,CGUEST,CMATCH_FIELD ,CHOST_HANDI,CSCORE_HANDI,CSH_ODDS,CSCORE_TARGET,CST_ODDS,CWH_ODDS,CWG_ODDS,IALERT_TYRE,IMATCH_COUNT  from NBAGAME_INFO where CWEB_VIEW='V' and CACTION='U' ");
				if(sSortType.Equals("0")) {		//order by league and then match date and time
					SQLString.Append("order by CLEAG, CMATCH_DATE, CMATCH_TIME");
				} else {		//order by order id
					SQLString.Append("order by ISEQ_NO, CMATCH_DATE, CMATCH_TIME");
				}			
				m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
				while(m_SportsOleReader.Read()) {
					HTMLString.Append("<tr align=\"center\">");

					//Order ID				
					HTMLString.Append("<td><input type=\"text\" name=\"orderID\" maxlength=\"2\" size=\"1\" value=\"");
					if(!m_SportsOleReader.IsDBNull(0)) {
						HTMLString.Append(m_SportsOleReader.GetInt32(0).ToString());
					}
					HTMLString.Append("\" tabindex=\"");
					HTMLString.Append((m_RecordCount + 500).ToString());
					HTMLString.Append("\" onChange=\"onOrder_IDChanged(" );
					HTMLString.Append(m_RecordCount.ToString());
					HTMLString.Append(")\"></td>");

					//Match Date and Time (Hidden Field)
					sRecordStr = m_SportsOleReader.GetString(1).Trim();
					HTMLString.Append("<td><input type=\"hidden\" name=\"MatchDate\" value=\"");
					HTMLString.Append(sRecordStr);
					HTMLString.Append("\">");
					if(!sRecordStr.Equals("")) {
						sRecordStr = sRecordStr.Insert(4,"/");
						sRecordStr = sRecordStr.Insert(7,"/");
					}
					HTMLString.Append(sRecordStr);
					HTMLString.Append("</td>");					 					
					sRecordStr = m_SportsOleReader.GetString(2).Trim();
					HTMLString.Append("<td><input type=\"hidden\" name=\"MatchTime\" value=\"" );
					HTMLString.Append(sRecordStr);
					HTMLString.Append("\">");
					if(!sRecordStr.Equals("")) {
						sRecordStr = sRecordStr.Insert(2,":");
					}
					HTMLString.Append(sRecordStr);
					HTMLString.Append("</td>");

					//Alias	
					HTMLString.Append("<td>");
					HTMLString.Append(m_SportsOleReader.GetString(3).Trim());
					HTMLString.Append("<input type=\"hidden\" name=\"Alias\" value=\"");
					HTMLString.Append(m_SportsOleReader.GetString(3).Trim());
					HTMLString.Append("\"></td>");

					//Host
					HTMLString.Append("<td>");
					HTMLString.Append(m_SportsOleReader.GetString(4).Trim());
					HTMLString.Append("<input type=\"hidden\" name=\"Host\" value=\"");
					HTMLString.Append(m_SportsOleReader.GetString(4).Trim());
					HTMLString.Append("\"></td>");

					//Guest
					HTMLString.Append("<td>");
					HTMLString.Append(m_SportsOleReader.GetString(5).Trim());
					HTMLString.Append("<input type=\"hidden\" name=\"Guest\" value=\"");
					HTMLString.Append(m_SportsOleReader.GetString(5).Trim());
					HTMLString.Append("\"></td>");

					//中立場
					HTMLString.Append("<td><input type=\"checkbox\" name=\"MatchField\" value=\"");
					HTMLString.Append(m_RecordCount.ToString());
					HTMLString.Append("\" onChange=\"onStatusChanged(");
					HTMLString.Append(m_RecordCount.ToString());
					HTMLString.Append(")\"");
					if(m_SportsOleReader.GetString(6).Trim().Equals("M")) {
						HTMLString.Append(" checked");
					}
					HTMLString.Append("></td>");

					//主讓
					HTMLString.Append("<td><input type=\"checkbox\" name=\"HostHandicap\" value=\"" );
					HTMLString.Append(m_RecordCount.ToString());
					HTMLString.Append("\" onChange=\"onStatusChanged(");
					HTMLString.Append(m_RecordCount.ToString());
					HTMLString.Append(")\"");
					if(m_SportsOleReader.GetString(7).Trim().Equals("1")) {
						HTMLString.Append(" checked");
					}
					HTMLString.Append("></td>");

					//讓分(賠率)
					HTMLString.Append("<td><input type=\"text\" name=\"Score1\" maxlength=\"4\" size=\"1\" value=\"");
					HTMLString.Append(m_SportsOleReader.GetString(8).Trim());
					HTMLString.Append("\" tabindex=\"");
					HTMLString.Append((m_RecordCount*6 + 1).ToString());
					HTMLString.Append("\" onChange=\"onScore1Changed(");
					HTMLString.Append(m_RecordCount.ToString());
					HTMLString.Append(")\">/<input type=\"text\" name=\"Score2\" maxlength=\"5\" size=\"2\" value=\"");
					HTMLString.Append(m_SportsOleReader.GetString(9).Trim());
					HTMLString.Append("\" tabindex=\"");
					HTMLString.Append((m_RecordCount*6 + 2).ToString());
					HTMLString.Append("\" onChange=\"onScore2Changed(");
					HTMLString.Append(m_RecordCount.ToString());
					HTMLString.Append(")\"></td>");

					//大小(賠率)
					HTMLString.Append("<td><input type=\"text\" name=\"Odds1\" maxlength=\"5\" size=\"2\" value=\"");
					HTMLString.Append(m_SportsOleReader.GetString(10).Trim());
					HTMLString.Append("\" tabindex=\"");
					HTMLString.Append((m_RecordCount*6 + 3).ToString());
					HTMLString.Append("\" onChange=\"onOdds1Changed(");
					HTMLString.Append(m_RecordCount.ToString());
					HTMLString.Append(")\">/<input type=\"text\" name=\"Odds2\" maxlength=\"5\" size=\"2\" value=\"");
					HTMLString.Append(m_SportsOleReader.GetString(11).Trim());
					HTMLString.Append("\" tabindex=\"");
					HTMLString.Append((m_RecordCount*6 + 4).ToString());
					HTMLString.Append("\" onChange=\"onOdds2Changed(");
					HTMLString.Append(m_RecordCount.ToString());
					HTMLString.Append(")\"></td>");

					//單雙(賠率)
					HTMLString.Append("<td><input type=\"text\" name=\"Handicap1\" maxlength=\"5\" size=\"2\" value=\"");
					HTMLString.Append(m_SportsOleReader.GetString(12).Trim());
					HTMLString.Append("\" tabindex=\"");
					HTMLString.Append((m_RecordCount*6 + 5).ToString());
					HTMLString.Append("\" onChange=\"onHandicap1Changed(");
					HTMLString.Append(m_RecordCount.ToString());
					HTMLString.Append(")\">/<input type=\"text\" name=\"Handicap2\" maxlength=\"5\" size=\"2\" value=\"");
					HTMLString.Append(m_SportsOleReader.GetString(13).Trim());
					HTMLString.Append("\" tabindex=\"");
					HTMLString.Append((m_RecordCount*6 + 6).ToString());
					HTMLString.Append("\" onChange=\"onHandicap2Changed(");
					HTMLString.Append(m_RecordCount.ToString());
					HTMLString.Append(")\"></td>");		
															
					//賠率狀況
					iArrayIndex = 0;
					iRecordStr = 0;
					iRecordStr = m_SportsOleReader.GetInt32(14);					
					HTMLString.Append("<td><select name=\"OddsState\" onChange=\"onStatusChanged(");
					HTMLString.Append(m_RecordCount.ToString());	
					HTMLString.Append(")\"><option value=\"");
					HTMLString.Append(iRecordStr.ToString());
					HTMLString.Append("\" selected>");
					HTMLString.Append(arrOddsState[iRecordStr]);
					foreach(String sItem in arrOddsState) {
						if(!sItem.Equals(arrOddsState[iRecordStr])&& iArrayIndex != 3) {
							HTMLString.Append("<option value=\"");
							HTMLString.Append(iArrayIndex.ToString());
							HTMLString.Append("\">");
							HTMLString.Append(sItem);
						}
						iArrayIndex++;
					}
					HTMLString.Append("</select></td>");
								
					//MUST Send
					HTMLString.Append("<td><input type=\"checkbox\" name=\"MUSTSendChk\" value=\"");
					HTMLString.Append(m_RecordCount.ToString());
					HTMLString.Append("\"></td>");

					//Hidden Check
					HTMLString.Append("<td><input type=\"checkbox\" name=\"hiddenChk\" value=\"");
					HTMLString.Append(m_RecordCount.ToString());
					HTMLString.Append("\" ></td>");

					//Match Count
					iRecordStr = m_SportsOleReader.GetInt32(15);
					HTMLString.Append("<input type=\"hidden\" name=\"iMatchcnt\" value=\"");
					HTMLString.Append(iRecordStr.ToString());
					HTMLString.Append("\"></tr>");
					m_RecordCount++;
				}
				m_SportsOleReader.Close();
				m_SportsDBMgr.Close();
				HTMLString.Append("<input type=\"hidden\" name=\"RecordCount\" value=\"");
				HTMLString.Append(m_RecordCount.ToString());
				HTMLString.Append("\">");
			} catch(Exception ex) {
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " BSKAllMatch.cs.GetAllMatches(): " + ex.ToString());
				m_SportsLog.Close();
				HTMLString.Remove(0,HTMLString.Length);
				HTMLString.Append((string)HttpContext.Current.Application["accessErrorMsg"]);
			}

			return HTMLString.ToString();
		}

		public int Update(string sType) {
			int iUpdIndex = 0;
			int iRecUpd = 0;
			int iTempItem = -1;
			int iOddsState;
			int iMustSendLen;
			int iHiddenLen;
			string sCurrentTimestamp = null;
			string sBatchJob = null;
			string sTempItem;
			string sHostHandicap;
			char[] delimiter = new char[] {','};
			string[] arrSendToPager;
			string[] arrMustSend;
			string[] arrOrderID;
			string[] arrMatchCnt;
			string[] arrHidden;	
			string[] arrRemotingPath;

			//SportsMessage object message
			SportsMessage sptMsg = new SportsMessage();
			StringBuilder LogSQLString = new StringBuilder();
			DBManager logDBMgr = new DBManager();
			logDBMgr.ConnectionString = m_SportsDBMgr.ConnectionString;

			arrMatchCnt = HttpContext.Current.Request.Form["iMatchcnt"].Split(delimiter);
			arrOrderID = HttpContext.Current.Request.Form["orderID"].Split(delimiter);
			arrRemotingPath = (string[])HttpContext.Current.Application["RemotingItems"];
			
			try {
				arrSendToPager = HttpContext.Current.Request.Form["SendToPager"].Split(delimiter);
			} catch(Exception) {
				arrSendToPager = new string[0];
			}
			
			try {
				arrHidden = HttpContext.Current.Request.Form["hiddenChk"].Split(delimiter);
				iHiddenLen = arrHidden.Length;
			} catch(Exception) {
				arrHidden = new string[0];
				iHiddenLen = 0 ;
			}

			try {
				arrMustSend = HttpContext.Current.Request.Form["MUSTSendChk"].Split(delimiter);
				iMustSendLen = arrMustSend.Length;
			}	catch(Exception) {
				arrMustSend = new string[0];
				iMustSendLen = 0;
			}

			try {
				switch(sType) {
					case "SEND":	//case: Sending match	
						int iLogUpd = 0;			
						string sScore1;
						string sScore2;
						string sOdds1;
						string sOdds2;
						string sHandicap1;
						string sHandicap2;
						string sMatchField;
						string[] arrMatchDate;
						string[] arrMatchTime;
						string[] arrAlias;
						string[] arrHost;
						string[] arrGuest;
						string[] arrScore1;
						string[] arrScore2;
						string[] arrOdds1;
						string[] arrOdds2;
						string[] arrHandicap1;
						string[] arrHandicap2;
						string[] arrOddsState;
						string[] arrMatchField;
						string[] arrHostHandicap;
						string[] arrMsgType;

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

						arrMatchDate = HttpContext.Current.Request.Form["MatchDate"].Split(delimiter);
						arrMatchTime = HttpContext.Current.Request.Form["MatchTime"].Split(delimiter);		
						arrAlias = HttpContext.Current.Request.Form["Alias"].Split(delimiter);
						arrHost = HttpContext.Current.Request.Form["Host"].Split(delimiter);
						arrGuest = HttpContext.Current.Request.Form["Guest"].Split(delimiter);
						arrScore1 = HttpContext.Current.Request.Form["Score1"].Split(delimiter);
						arrScore2 = HttpContext.Current.Request.Form["Score2"].Split(delimiter);
						arrOdds1 = HttpContext.Current.Request.Form["Odds1"].Split(delimiter);
						arrOdds2 = HttpContext.Current.Request.Form["Odds2"].Split(delimiter);
						arrHandicap1 = HttpContext.Current.Request.Form["Handicap1"].Split(delimiter);
						arrHandicap2 = HttpContext.Current.Request.Form["Handicap2"].Split(delimiter);
						arrOddsState = HttpContext.Current.Request.Form["OddsState"].Split(delimiter);

						try {
							arrMatchField = HttpContext.Current.Request.Form["MatchField"].Split(delimiter);
						}	catch(Exception) {
							arrMatchField = new string[0];
						}
						try {
							arrHostHandicap = HttpContext.Current.Request.Form["HostHandicap"].Split(delimiter);
						}	catch(Exception) {
							arrHostHandicap = new string[0];
						}

						arrMsgType = (string[])HttpContext.Current.Application["messageType"];
						sCurrentTimestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
						sBatchJob = DateTime.Now.ToString("yyMMddHHmmss_fffffff") + "." + HttpContext.Current.Session["user_name"].ToString() + "." + arrMsgType[29] + ".ini";
						for(iRecUpd=0;iRecUpd<iMustSendLen;iRecUpd++) {
							//update the checked(MUSTSend) records only
							iUpdIndex = Convert.ToInt32(arrMustSend[iRecUpd]);
							iOddsState = Convert.ToInt32(arrOddsState[iUpdIndex]);
							sScore1 = arrScore1[iUpdIndex];
							if(sScore1 == null) sScore1 = "";
							else sScore1 = sScore1.Trim();

							sScore2 = arrScore2[iUpdIndex];
							if(sScore2 == null) sScore2 = "";
							else sScore2 = sScore2.Trim();

							sOdds1 = arrOdds1[iUpdIndex];
							if(sOdds1 == null) sOdds1 = "";
							else sOdds1 = sOdds1.Trim();

							sOdds2 = arrOdds2[iUpdIndex];
							if(sOdds2 == null) sOdds2 = "";
							else sOdds2 = sOdds2.Trim();

							sHandicap1 = arrHandicap1[iUpdIndex];
							if(sHandicap1 == null) sHandicap1 = "";
							else sHandicap1 = sHandicap1.Trim();

							sHandicap2 = arrHandicap2[iUpdIndex];
							if(sHandicap2 == null) sHandicap2 = "";
							else sHandicap2 = sHandicap2.Trim();						

							sTempItem = "";
							for(int iFieldUpd=0;iFieldUpd<arrMatchField.Length;iFieldUpd++) {
								if(arrMatchField[iFieldUpd] == iUpdIndex.ToString()) {
									sTempItem = "1";
									break;
								}
							}
							if(sTempItem.Equals("1")) sMatchField = "M";
							else sMatchField = "H";

							sHostHandicap = "";
							for(int iHandicapUpd=0;iHandicapUpd<arrHostHandicap.Length;iHandicapUpd++) {
								if(arrHostHandicap[iHandicapUpd] == iUpdIndex.ToString()) {
									sHostHandicap = "1";
									break;
								}
							}
							if(!sHostHandicap.Equals("1")) sHostHandicap = "0";

							//update NBAGAME_INFO	
							SQLString.Remove(0,SQLString.Length);		
							SQLString.Append("update NBAGAME_INFO set CMATCH_FIELD='");
							SQLString.Append(sMatchField);
							SQLString.Append("', CHOST_HANDI='");
							SQLString.Append(sHostHandicap);
							SQLString.Append("', CSCORE_HANDI='");
							SQLString.Append(sScore1);
							SQLString.Append("', CSH_ODDS='");
							SQLString.Append(sScore2);
							SQLString.Append("', CSCORE_TARGET='");
							SQLString.Append(sOdds1);
							SQLString.Append("', CST_ODDS='");
							SQLString.Append(sOdds2) ;
							SQLString.Append( "', CWH_ODDS='");
							SQLString.Append(sHandicap1);
							SQLString.Append("', CWG_ODDS='");
							SQLString.Append(sHandicap2);
							SQLString.Append("', IALERT_TYRE=");
							SQLString.Append(iOddsState);
							SQLString.Append("  where IMATCH_COUNT = ");
							SQLString.Append(arrMatchCnt[iUpdIndex]);  
							m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
							m_SportsDBMgr.Close();
							//generate the Match INI file if required
							if((arrSendToPager.Length > 0) && (iMustSendLen>0)) {
								iLogUpd++;
								//insert into LOG_BSK_ALLODDS
								LogSQLString.Remove(0,LogSQLString.Length);
								LogSQLString.Append("insert into LOG_BSK_ALLODDS (TIMEFLAG,Section,Act,League,Host,Guest,MatchDate,MatchTime,MatchField,Handicap,ScoreHandicap,SH_odds,ScoreTarget,ST_Odds,WH_Odds,WG_Odds,Alert_Type,BATCHJOB) values('");
								LogSQLString.Append(sCurrentTimestamp);
								LogSQLString.Append("','MATCH_','U','");
								LogSQLString.Append(arrAlias[iUpdIndex]);
								LogSQLString.Append("','");
								LogSQLString.Append(arrHost[iUpdIndex]);
								LogSQLString.Append("','");
								LogSQLString.Append(arrGuest[iUpdIndex]);
								LogSQLString.Append("','");
								LogSQLString.Append(arrMatchDate[iUpdIndex]);
								LogSQLString.Append("','");
								LogSQLString.Append(arrMatchTime[iUpdIndex]);
								LogSQLString.Append("','");
								LogSQLString.Append(sMatchField);
								LogSQLString.Append("','");
								LogSQLString.Append(sHostHandicap);
								LogSQLString.Append("','");
								if(sScore1.Equals("")) sScore1 = "-1";
								LogSQLString.Append(sScore1);
								LogSQLString.Append("','");
								if(sScore2.Equals("")) sScore2 = "-1";
								LogSQLString.Append(sScore2);
								LogSQLString.Append("','");
								if(sOdds1.Equals("")) sOdds1 = "-1";
								LogSQLString.Append(sOdds1);
								LogSQLString.Append("','");
								if(sOdds2.Equals("")) sOdds2 = "-1";
								LogSQLString.Append(sOdds2);
								LogSQLString.Append("','");	
								if(sHandicap1.Equals("")) sHandicap1 = "-1";	
								LogSQLString.Append(sHandicap1);
								LogSQLString.Append("','");	
								if(sHandicap2.Equals("")) sHandicap2 = "-1";	
								LogSQLString.Append(sHandicap2);
								LogSQLString.Append("','");
								LogSQLString.Append(arrOddsState[iUpdIndex]);
								LogSQLString.Append("','");
								LogSQLString.Append(sBatchJob);
								LogSQLString.Append("')");
								logDBMgr.ExecuteNonQuery(LogSQLString.ToString());
								logDBMgr.Close();			
							}
						}	//insert into LOG_BSK_ALLODDS

						if(iLogUpd > 0) {
							//Send Notify Message
							//Modified by Henry, 11 Feb 2004
							sptMsg.Body = sBatchJob;
							sptMsg.Timestamp = DateTime.Now.ToString("yyMMddHHmmss");
							sptMsg.AppID = "07";
							sptMsg.MsgID = "27"; //27為定義籃球所有賽事
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
									m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " MSMQ ERROR: Basketball All Match");
									m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " BSKAllMatch.cs.Update(): Notify via MSMQ throws MessageQueueException:  " + mqEx.ToString());
									m_SportsLog.Close();

									//If MSMQ fail, notify via .NET Remoting
									msgClt.MessageType = arrMessageTypes[1];
									msgClt.MessagePath = arrRemotingPath[0];
									if(!msgClt.SendMessage((object)sptMsg)) {
										m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
										m_SportsLog.SetFileName(0,LOGFILESUFFIX);
										m_SportsLog.Open();
										m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Remoting ERROR: Basketball All Match");
										m_SportsLog.Close();
									}
								}	catch(Exception ex) {
									m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
									m_SportsLog.SetFileName(0,LOGFILESUFFIX);
									m_SportsLog.Open();
									m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Remoting ERROR: Basketball All Match");
									m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " BSKAllMatch.cs.Update(): Notify via .NET Remoting throws exception: " + ex.ToString());
									m_SportsLog.Close();
								}							
							}	catch(Exception ex) {
								m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
								m_SportsLog.SetFileName(0,LOGFILESUFFIX);
								m_SportsLog.Open();
								m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " BSKAllMatch.cs.Update(): Notify via MSMQ throws exception: " + ex.ToString());
								m_SportsLog.Close();
							}				
							//Modified end
						}

						if(iRecUpd > 0) {
							m_SportsDBMgr.Dispose();
						}

						//write log
						m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
						m_SportsLog.SetFileName(0,LOGFILESUFFIX);
						m_SportsLog.Open();
						m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " BSKAllMatch.cs: Send " + iRecUpd.ToString() + " matches (" + HttpContext.Current.Session["user_name"] + ")");
						m_SportsLog.Close();
						break;
					case "SORT":	//case: Sorting match order
						for(iRecUpd=0;iRecUpd<arrOrderID.Length;iRecUpd++) {
							sTempItem = arrOrderID[iRecUpd];
							if(sTempItem == null) {
								iTempItem = -1;
							}	else if(sTempItem.Equals("")) {
								iTempItem = -1;
							}	else {
								iTempItem = Convert.ToInt32(sTempItem);							
							}
							SQLString.Remove(0,SQLString.Length);
							if(iTempItem != -1) {
								SQLString.Append("update NBAGAME_INFO set ISEQ_NO=");	
								SQLString.Append(iTempItem);
							} else {
								SQLString.Append("update NBAGAME_INFO set ISEQ_NO=null ");
							}
							SQLString.Append(" where IMATCH_COUNT=");
							SQLString.Append(arrMatchCnt[iRecUpd].Trim());
							m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
							m_SportsDBMgr.Close();
						}
						if(iRecUpd > 0) {
							m_SportsDBMgr.Dispose();
						}

						//write log
						m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
						m_SportsLog.SetFileName(0,LOGFILESUFFIX);
						m_SportsLog.Open();
						m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " BSKAllMatch.cs: Sort " + iRecUpd.ToString() + " matches (" + HttpContext.Current.Session["user_name"] + ")");
						m_SportsLog.Close();
						break;
					case "HIDE":	//case: Hidden specific match
						for(iRecUpd=0;iRecUpd<iHiddenLen;iRecUpd++) {
							iUpdIndex = Convert.ToInt32(arrHidden[iRecUpd]);
							sTempItem = arrMatchCnt[iUpdIndex].Trim();
							if(iRecUpd == 0) {
								SQLString.Remove(0,SQLString.Length);
								SQLString.Append("update NBAGAME_INFO set CWEB_VIEW='X' where IMATCH_COUNT=");
								SQLString.Append(sTempItem);
							}	else {
								SQLString.Append(" or IMATCH_COUNT=");
								SQLString.Append(sTempItem);
							}
							m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
							m_SportsDBMgr.Close();
						}
						if(iRecUpd > 0) {
							m_SportsDBMgr.Dispose();
						}

						//write log
						m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
						m_SportsLog.SetFileName(0,LOGFILESUFFIX);
						m_SportsLog.Open();
						m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " BSKAllMatch.cs: Hide " + iRecUpd.ToString() + " matches (" + HttpContext.Current.Session["user_name"] + ")");
						m_SportsLog.Close();
						break;
					case "SHOW":	//case: Set all matches to visible
						SQLString.Remove(0,SQLString.Length);
						SQLString.Append("update NBAGAME_INFO set CWEB_VIEW='V'");
						iRecUpd = m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
						m_SportsDBMgr.Close();

						//write log
						m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
						m_SportsLog.SetFileName(0,LOGFILESUFFIX);
						m_SportsLog.Open();
						m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " BSKAllMatch.cs: " + HttpContext.Current.Session["user_name"] + " set " + iRecUpd.ToString() + " matches to visible");
						m_SportsLog.Close();
						break;
					default:
						iRecUpd = 0;
						break;
				}
			} catch(Exception ex) {
				iRecUpd = -1;
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " BSKAllMatch.cs.Update(): " + ex.ToString());
				m_SportsLog.Close();
			}

			return iRecUpd;
		}
	}
}