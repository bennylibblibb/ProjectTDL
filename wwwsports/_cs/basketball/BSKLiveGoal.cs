/*
Objective:
Modify LiveGoal use the tables NBAGAME_INFO , NBARESULT_INFO and LOG_BSK_LIVEGOAL

Last updated:
11 Feb 2004 by HenryCi

C#.NET complier statement:
csc /t:library /out:..\..\bin\BSKLiveGoal.dll /r:..\..\bin\DBManager.dll;..\..\bin\Files.dll;..\..\bin\MessageClient.dll;..\..\bin\SportsMessage.dll BSKLiveGoal.cs
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
[assembly:AssemblyDescription("籃球資訊 -> 現場比數")]
[assembly:AssemblyProduct("Sports.NET")]
[assembly:AssemblyTitle("Sports.NET DLL")]
[assembly:AssemblyVersion("3.1.*")]
namespace SportsUtil {
	public class BSKLiveGoal {
		const string LOGFILESUFFIX = "log";
		int m_RecordCount = 0;
		OleDbDataReader m_SportsOleReader;
		DBManager m_SportsDBMgr;
		Files m_SportsLog;
		StringBuilder SQLString;

		public BSKLiveGoal(string Connection) {
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

		public string GetLiveGoal() {
			int iArrayIndex = 0;
			int iRecordStr = 0;
			string sRecordStr = "";
			string sSortType;	
			string[] arrAlertType;
			string[] arrMatchStatus;
			StringBuilder HTMLString = new StringBuilder();
			OleDbDataReader ResultDetailsOleReader;
			DBManager ResultDetailsDBMgr = new DBManager();
			ResultDetailsDBMgr.ConnectionString = m_SportsDBMgr.ConnectionString;

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
			arrAlertType = (string[])HttpContext.Current.Application["BSKAlertArray"];
			arrMatchStatus = (string[])HttpContext.Current.Application["BSKMatchStatusArray"];

			try {
				SQLString.Remove(0,SQLString.Length);
				SQLString.Append("delete from NBARESULT_INFO RESULT WHERE RESULT.IMATCH_COUNT NOT IN (SELECT IMATCH_COUNT FROM NBAGAME_INFO )");
				m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
				m_SportsDBMgr.Close();

				SQLString.Remove(0,SQLString.Length);	
				SQLString.Append("select ISEQ_NO,CMATCH_DATE,CMATCH_TIME,CLEAG,CHOST,CGUEST,IMATCH_COUNT  from NBAGAME_INFO where CWEB_VIEW='V' and CACTION='U' ");
				if(sSortType.Equals("0")) {		//order by league and then match date and time
					SQLString.Append("order by CLEAG,CMATCH_DATE,CMATCH_TIME");
				} else {		//order by order id
					SQLString.Append("order by ISEQ_NO,CLEAG,CMATCH_DATE,CMATCH_TIME");
				}			
				m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
				while(m_SportsOleReader.Read()) {
					//imatch_cnt
					iRecordStr = m_SportsOleReader.GetInt32(6);
					HTMLString.Append( "<input type=\"hidden\" name=\"iMatchcnt\" value=\"");
					HTMLString.Append(iRecordStr.ToString());
					HTMLString.Append("\">");

					int iScrHost1=-1,iScrHost2=-1,iScrHost3=-1,iScrHost4=-1,iIotScrHost5=-1,iScrGuests1=-1,iScrGuests2=-1,iScrGuests3=-1,iScrGuests4=-1,iIotScrGuest5=-1;
					string sRemark = "";
					int iMatchState = 0;

					SQLString.Remove(0,SQLString.Length);
					SQLString.Append("Select distinct result.IQUARTER1_HSCORE,result.IQUARTER2_HSCORE,result.IQUARTER3_HSCORE,result.IQUARTER4_HSCORE,result.IOT1_HSCORE ,result.IQUARTER1_GSCORE,result.IQUARTER2_GSCORE,result.IQUARTER3_GSCORE,result.IQUARTER4_GSCORE,result.IOT1_GSCORE, result.IMATCH_STATUS,result.CREMARKS from NBARESULT_INFO result,NBAGAME_INFO game where result.IMATCH_COUNT=");
					SQLString.Append(iRecordStr);
					SQLString.Append(" and game.CACTION='U' and game.CWEB_VIEW='V'");
					ResultDetailsOleReader = ResultDetailsDBMgr.ExecuteQuery(SQLString.ToString());
					if(ResultDetailsOleReader.Read()) {
						iScrHost1 = ResultDetailsOleReader.GetInt32(0);
						iScrHost2 = ResultDetailsOleReader.GetInt32(1);
						iScrHost3 = ResultDetailsOleReader.GetInt32(2);
						iScrHost4 = ResultDetailsOleReader.GetInt32(3);
						iIotScrHost5 = ResultDetailsOleReader.GetInt32(4);
						iScrGuests1 = ResultDetailsOleReader.GetInt32(5);
						iScrGuests2 = ResultDetailsOleReader.GetInt32(6);
						iScrGuests3 = ResultDetailsOleReader.GetInt32(7);
						iScrGuests4 = ResultDetailsOleReader.GetInt32(8);
						iIotScrGuest5 = ResultDetailsOleReader.GetInt32(9);
						iMatchState = ResultDetailsOleReader.GetInt32(10);
						if(!ResultDetailsOleReader.IsDBNull(11)) {
							sRemark = ResultDetailsOleReader.GetString(11).Trim();
						}
					}
					ResultDetailsOleReader.Close();
					ResultDetailsDBMgr.Close();

					//Order ID
					HTMLString.Append("<tr align=\"center\"><td><input type=\"text\" name=\"orderID\" maxlength=\"2\" size=\"1\" value=\"");
					if(!m_SportsOleReader.IsDBNull(0)) {
 						HTMLString.Append(m_SportsOleReader.GetInt32(0).ToString());
					}
					HTMLString.Append("\" tabindex=\"");
					HTMLString.Append((m_RecordCount + 500).ToString());
					HTMLString.Append("\" onChange=\"onOrder_IDChanged(");
					HTMLString.Append(m_RecordCount.ToString());
					HTMLString.Append(") \"></td>");					
					
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
					HTMLString.Append("<td><input type=\"hidden\" name=\"MatchTime\" value=\"");
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
					HTMLString.Append("<td style=\"background-color:#FFC733\">");
					HTMLString.Append(m_SportsOleReader.GetString(4).Trim());
					HTMLString.Append("<input type=\"hidden\" name=\"Host\" value=\"");
					HTMLString.Append(m_SportsOleReader.GetString(4).Trim());
					HTMLString.Append("\"></td>");
					
					HTMLString.Append("<td style=\"background-color:#FFC733\">1. <INPUT TYPE=\"TEXT\" NAME=\"QUHSCORE1\" VALUE=\"");
					if(iScrHost1!=-1) {
						HTMLString.Append(iScrHost1);
					}
					HTMLString.Append("\" SIZE=\"1\" MAXLENGTH=\"3\" tabindex=\"");
					HTMLString.Append((m_RecordCount*10 + 1).ToString());
					HTMLString.Append("\" onChange=\"onQuHScore1Changed(");
					HTMLString.Append(m_RecordCount.ToString());
					HTMLString.Append(")\"></BR>");
						
					HTMLString.Append("2. <INPUT TYPE=\"TEXT\" NAME=\"QUHSCORE2\" VALUE=\"");
					if(iScrHost2!=-1) {
						HTMLString.Append(iScrHost2);
					}
					HTMLString.Append("\" SIZE=\"1\" MAXLENGTH=\"3\" tabindex=\"");
					HTMLString.Append((m_RecordCount*10 + 3).ToString());
					HTMLString.Append("\" onChange=\"onQuHScore2Changed(");
					HTMLString.Append(m_RecordCount.ToString());
					HTMLString.Append(")\"></BR>");
									
					HTMLString.Append("3. <INPUT TYPE=\"TEXT\" NAME=\"QUHSCORE3\" VALUE=\"");
					if(iScrHost3!=-1) {
						HTMLString.Append(iScrHost3);
					}
					HTMLString.Append("\" SIZE=\"1\" MAXLENGTH=\"3\" tabindex=\"");
					HTMLString.Append((m_RecordCount*10 + 5).ToString());
					HTMLString.Append("\" onChange=\"onQuHScore3Changed(");
					HTMLString.Append(m_RecordCount.ToString());
					HTMLString.Append(")\"></BR>");
									
 					HTMLString.Append("4. <INPUT TYPE=\"TEXT\" NAME=\"QUHSCORE4\" VALUE=\"");
 					if(iScrHost4!=-1) {
 						HTMLString.Append(iScrHost4);
 					}
 					HTMLString.Append("\" SIZE=\"1\" MAXLENGTH=\"3\" tabindex=\"");
 					HTMLString.Append((m_RecordCount*10 + 7).ToString());
 					HTMLString.Append("\" onChange=\"onQuHScore4Changed(");
 					HTMLString.Append(m_RecordCount.ToString());
 					HTMLString.Append(")\"></BR>");					
 								
					HTMLString.Append("OT<INPUT TYPE=\"TEXT\" NAME=\"QUHSCORE5\" VALUE=\"");
					if(iIotScrHost5!=-1) {
						HTMLString.Append(iIotScrHost5);
					}
					HTMLString.Append("\" SIZE=\"1\" MAXLENGTH=\"3\" tabindex=\"");
					HTMLString.Append((m_RecordCount*10 + 9).ToString());
					HTMLString.Append("\" onChange=\"onQuHScore5Changed(");
					HTMLString.Append(m_RecordCount.ToString());
					HTMLString.Append(")\"></td>");	
					
					//Guest
					HTMLString.Append("<td style=\"background-color:#FFEFD5\">");
					HTMLString.Append(m_SportsOleReader.GetString(5).Trim());
					HTMLString.Append("<input type=\"hidden\" name=\"Guest\" value=\"");
					HTMLString.Append(m_SportsOleReader.GetString(5).Trim());
					HTMLString.Append("\"></td>");

					HTMLString.Append("<td style=\"background-color:#FFEFD5\">1. <INPUT TYPE=\"TEXT\" NAME=\"QUGSCORE1\" VALUE=\"");
					if(iScrGuests1!=-1) {
						HTMLString.Append(iScrGuests1);
					}
					HTMLString.Append("\" SIZE=\"1\" MAXLENGTH=\"3\" tabindex=\"");
					HTMLString.Append((m_RecordCount*10 + 2).ToString());
					HTMLString.Append("\" onChange=\"onQuGScore1Changed(");
					HTMLString.Append(m_RecordCount.ToString());
					HTMLString.Append(")\"></BR>");

					HTMLString.Append("2. <INPUT TYPE=\"TEXT\" NAME=\"QUGSCORE2\" VALUE=\"");
					if(iScrGuests2!=-1) {
						HTMLString.Append(iScrGuests2);
					}
					HTMLString.Append("\" SIZE=\"1\" MAXLENGTH=\"3\" tabindex=\"");
					HTMLString.Append((m_RecordCount*10 + 4).ToString());
					HTMLString.Append("\" onChange=\"onQuGScore2Changed(");
					HTMLString.Append(m_RecordCount.ToString());
					HTMLString.Append(")\"></BR>");

					HTMLString.Append("3. <INPUT TYPE=\"TEXT\" NAME=\"QUGSCORE3\" VALUE=\"");
					if(iScrGuests3!=-1) {
						HTMLString.Append(iScrGuests3);
					}
					HTMLString.Append("\" SIZE=\"1\" MAXLENGTH=\"3\" tabindex=\"");
					HTMLString.Append((m_RecordCount*10 + 6).ToString());
					HTMLString.Append("\" onChange=\"onQuGScore3Changed(");
					HTMLString.Append(m_RecordCount.ToString());
					HTMLString.Append(")\"></BR>");

					HTMLString.Append("4. <INPUT TYPE=\"TEXT\" NAME=\"QUGSCORE4\" VALUE=\"");
					if(iScrGuests4!=-1) {
						HTMLString.Append(iScrGuests4);
					}
					HTMLString.Append("\" SIZE=\"1\" MAXLENGTH=\"3\" tabindex=\"");
					HTMLString.Append((m_RecordCount*10 + 8).ToString());
					HTMLString.Append("\" onChange=\"onQuGScore4Changed(");
					HTMLString.Append(m_RecordCount.ToString());
					HTMLString.Append(")\"></BR>");

					HTMLString.Append("OT<INPUT TYPE=\"TEXT\" NAME=\"QUGSCORE5\" VALUE=\"");
					if(iIotScrGuest5!=-1) {
						HTMLString.Append(iIotScrGuest5);
					}
					HTMLString.Append("\" SIZE=\"1\" MAXLENGTH=\"3\" tabindex=\"");
					HTMLString.Append((m_RecordCount*10 + 10).ToString());
					HTMLString.Append("\" onChange=\"onQuGScore5Changed(");
					HTMLString.Append(m_RecordCount.ToString());
					HTMLString.Append( ")\"></td>");	

					//State
					iArrayIndex = 0;
					iRecordStr = 0;
					HTMLString.Append("<td><select name=\"MatchState\" onChange=\"onStatusChanged(");
					HTMLString.Append(m_RecordCount.ToString());
					HTMLString.Append(")\"><option value=");
					HTMLString.Append(iMatchState);
					HTMLString.Append(" selected>");
					HTMLString.Append(arrMatchStatus[iMatchState]);
					foreach(String sItem in arrMatchStatus) {
						if(!sItem.Equals(arrMatchStatus[iMatchState])) {
							HTMLString.Append("<option value=" );
							HTMLString.Append(iArrayIndex.ToString());
							HTMLString.Append(">");
							HTMLString.Append(sItem);
						}
						iArrayIndex++;
					}
					HTMLString.Append("</select></td>");	

					//SongType
					HTMLString.Append("<td><select name=\"song\" onChange=\"onStatusChanged(");
					HTMLString.Append(m_RecordCount.ToString());
					HTMLString.Append(")\"><option value=\"0000\">預設歌曲</select></td>");

					//remark備註
					HTMLString.Append("<TD><INPUT TYPE=\"TEXT\" NAME=\"REM\" VALUE=\"");
					HTMLString.Append(sRemark);
					HTMLString.Append("\" SIZE=\"9\" MAXLENGTH=\"9\" onChange=\"onRemarkChanged(");
					HTMLString.Append(m_RecordCount.ToString());
					HTMLString.Append(")\"></Td>");					
					//Alert
					iRecordStr = 0;
					iArrayIndex = 0;
					HTMLString.Append("<td><select name=\"Alerttype\" onChange=\"onStatusChanged(");
					HTMLString.Append(m_RecordCount.ToString());
					HTMLString.Append(")\">");				
					foreach(String sItem in arrAlertType) {	
						HTMLString.Append("<option value=" );
						HTMLString.Append(iArrayIndex.ToString());
						HTMLString.Append(">");
						HTMLString.Append(sItem);			
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
					HTMLString.Append("\"></td></tr>");
					m_RecordCount++;
				}
				m_SportsOleReader.Close();
				m_SportsDBMgr.Close();
			} catch(Exception ex) {
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " BSKLiveGoal.cs.GetLiveGoal(): " + ex.ToString());
				m_SportsLog.Close();
				HTMLString.Remove(0,HTMLString.Length);
				HTMLString.Append((string)HttpContext.Current.Application["accessErrorMsg"]);
			}

			return HTMLString.ToString();
		}

		public int UpdateLiveGoal(string sType) {
			int iUpdIndex = 0;
			int iRecUpd = 0;
			int iTempItem = -1;
			int iAlertType;
			int iMustSendLen;
			int iHiddenLen;
			string[] arrSendToPager;
			string sTempItem;
			string sCurrentTimestamp = null;
			string sBatchJob = null;
			char[] delimiter = new char[] {','};
			string[] arrMustSend;
			string[] arrOrderID;
			string[] arrMatchCnt;
			string[] arrHidden;
			
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

			try {
				arrMatchCnt = HttpContext.Current.Request.Form["iMatchcnt"].Split(delimiter);	
			} catch(Exception) {
				arrMatchCnt = new string[0];	
			}
			try {
				arrOrderID = HttpContext.Current.Request.Form["orderID"].Split(delimiter);
			} catch(Exception) {
				arrOrderID = new string[0];
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
						int iQuHScore1;
						int iQuHScore2;
						int iQuHScore3;
						int iQuHScore4;
						int iOtHScore;
						int iQuGScore1;
						int iQuGScore2;
						int iQuGScore3;
						int iQuGScore4;
						int iOtGScore;
						string[] arrMatchDate,arrMatchTime,arrAlias,arrHost,arrGuest,arrAlerttype;
						string[] arrMsgType,arrQuHScore1,arrQuHScore2,arrQuHScore3,arrQuHScore4,arrOtHScore;
						string[] arrQuGScore1,arrQuGScore2,arrQuGScore3,arrQuGScore4,arrOtGScore;
						string[] arrSongID;
						string[] arrRemarks;
						string[] arrMatchStatusIdx;
						string[] arrMatchStatus;
						string[] arrRemotingPath;
						
						/*****************************
						 * GoGo Pager2 alert message *
						 *****************************/						
						string[] arrQueueNames;
						string[] arrMessageTypes;

						arrMatchStatus = (string[])HttpContext.Current.Application["BSKMatchStatusArray"];
						arrMsgType = (string[])HttpContext.Current.Application["messageType"];						
						arrQueueNames = (string[])HttpContext.Current.Application["QueueItems"];
						arrMessageTypes = (string[])HttpContext.Current.Application["NotifyMessageTypes"];
						arrRemotingPath = (string[])HttpContext.Current.Application["RemotingItems"];
						MessageClient msgClt = new MessageClient();
						msgClt.MessageType = arrMessageTypes[0];
						msgClt.MessagePath = arrQueueNames[0];

						arrMatchDate = HttpContext.Current.Request.Form["MatchDate"].Split(delimiter);
						arrMatchTime = HttpContext.Current.Request.Form["MatchTime"].Split(delimiter);			
						arrAlias = HttpContext.Current.Request.Form["Alias"].Split(delimiter);
						arrHost = HttpContext.Current.Request.Form["Host"].Split(delimiter);
						arrGuest = HttpContext.Current.Request.Form["Guest"].Split(delimiter);
						arrAlerttype = HttpContext.Current.Request.Form["Alerttype"].Split(delimiter);
						arrQuHScore1 = HttpContext.Current.Request.Form["QUHSCORE1"].Split(delimiter);
						arrQuHScore2 = HttpContext.Current.Request.Form["QUHSCORE2"].Split(delimiter);
						arrQuHScore3 = HttpContext.Current.Request.Form["QUHSCORE3"].Split(delimiter);
						arrQuHScore4 = HttpContext.Current.Request.Form["QUHSCORE4"].Split(delimiter);
						arrOtHScore = HttpContext.Current.Request.Form["QUHSCORE5"].Split(delimiter);
						arrQuGScore1 = HttpContext.Current.Request.Form["QUGSCORE1"].Split(delimiter);
						arrQuGScore2 = HttpContext.Current.Request.Form["QUGSCORE2"].Split(delimiter);
						arrQuGScore3 = HttpContext.Current.Request.Form["QUGSCORE3"].Split(delimiter);
						arrQuGScore4 = HttpContext.Current.Request.Form["QUGSCORE4"].Split(delimiter);
						arrOtGScore = HttpContext.Current.Request.Form["QUGSCORE5"].Split(delimiter);
						arrMatchStatusIdx = HttpContext.Current.Request.Form["MatchState"].Split(delimiter);
						arrSongID = HttpContext.Current.Request.Form["song"].Split(delimiter);
						arrRemarks = HttpContext.Current.Request.Form["REM"].Split(delimiter);

						sCurrentTimestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
						sBatchJob = DateTime.Now.ToString("yyMMddHHmmss_fffffff") + "." + HttpContext.Current.Session["user_name"].ToString() + "." + arrMsgType[30] + ".ini";
						for(iRecUpd=0;iRecUpd<iMustSendLen;iRecUpd++) {							
							//update the checked(MUSTSend) records only
							iUpdIndex = Convert.ToInt32(arrMustSend[iRecUpd]);
							iAlertType = Convert.ToInt32(arrAlerttype[iUpdIndex]);														
							//update gameinfo
							SQLString.Remove(0,SQLString.Length);
							SQLString.Append("delete from NBARESULT_INFO where IMATCH_COUNT=");
							SQLString.Append(arrMatchCnt[iUpdIndex]);
							m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
							m_SportsDBMgr.Close();

							if(arrQuHScore1[iUpdIndex].Equals("")) iQuHScore1 = -1;
							else iQuHScore1 = Convert.ToInt32(arrQuHScore1[iUpdIndex]);

							if(arrQuGScore1[iUpdIndex].Equals("")) iQuGScore1 = -1;
							else iQuGScore1 = Convert.ToInt32(arrQuGScore1[iUpdIndex]);

							if(arrQuHScore2[iUpdIndex].Equals("")) iQuHScore2 = -1;
							else iQuHScore2 = Convert.ToInt32(arrQuHScore2[iUpdIndex]);

							if(arrQuGScore2[iUpdIndex].Equals("")) iQuGScore2 = -1;
							else iQuGScore2 = Convert.ToInt32(arrQuGScore2[iUpdIndex]);

							if(arrQuHScore3[iUpdIndex].Equals("")) iQuHScore3 = -1;
							else iQuHScore3 = Convert.ToInt32(arrQuHScore3[iUpdIndex]);

							if(arrQuGScore3[iUpdIndex].Equals("")) iQuGScore3 = -1;
							else iQuGScore3 = Convert.ToInt32(arrQuGScore3[iUpdIndex]);

							if(arrQuHScore4[iUpdIndex].Equals("")) iQuHScore4 = -1;
							else iQuHScore4 = Convert.ToInt32(arrQuHScore4[iUpdIndex]);

							if(arrQuGScore4[iUpdIndex].Equals("")) iQuGScore4 = -1;
							else iQuGScore4 = Convert.ToInt32(arrQuGScore4[iUpdIndex]);

							if(arrOtHScore[iUpdIndex].Equals("")) iOtHScore = -1;
							else iOtHScore = Convert.ToInt32(arrOtHScore[iUpdIndex]);

							if(arrOtGScore[iUpdIndex].Equals("")) iOtGScore = -1;
							else iOtGScore = Convert.ToInt32(arrOtGScore[iUpdIndex]);

							SQLString.Remove(0,SQLString.Length);
							SQLString.Append("insert into NBARESULT_INFO (IMATCH_COUNT,IMATCH_STATUS,IQUARTER1_HSCORE,IQUARTER1_GSCORE,IQUARTER2_HSCORE,IQUARTER2_GSCORE,IQUARTER3_HSCORE,IQUARTER3_GSCORE,IQUARTER4_HSCORE,IQUARTER4_GSCORE,IOT1_HSCORE,IOT1_GSCORE");
							SQLString.Append(",IOT2_HSCORE,IOT2_GSCORE,IOT3_HSCORE,IOT3_GSCORE,IOT4_HSCORE,IOT4_GSCORE,CREMARKS,CSONG_ID,CSENT_FLAG ) values(");
							SQLString.Append(Convert.ToInt32(arrMatchCnt[iUpdIndex]));
							SQLString.Append(",");
							SQLString.Append(arrMatchStatusIdx[iUpdIndex].Trim());
							SQLString.Append(",");
							SQLString.Append(iQuHScore1);
							SQLString.Append(",");
							SQLString.Append(iQuGScore1);
							SQLString.Append(",");
							SQLString.Append(iQuHScore2);
							SQLString.Append(",");
							SQLString.Append(iQuGScore2);
							SQLString.Append(",");
							SQLString.Append(iQuHScore3);
							SQLString.Append(",");
							SQLString.Append(iQuGScore3);
							SQLString.Append(",");
							SQLString.Append(iQuHScore4);
							SQLString.Append(",");
							SQLString.Append(iQuGScore4);
							SQLString.Append(",");
							SQLString.Append(iOtHScore);
							SQLString.Append(",");
							SQLString.Append(iOtGScore);
							SQLString.Append(",");
							SQLString.Append(-1);
							SQLString.Append(",");
							SQLString.Append(-1);
							SQLString.Append(",");
							SQLString.Append(-1);
							SQLString.Append(",");
							SQLString.Append(-1);
							SQLString.Append(",");
							SQLString.Append(-1);
							SQLString.Append(",");
							SQLString.Append(-1);
							SQLString.Append(",'");
							SQLString.Append(arrRemarks[iUpdIndex]);
							SQLString.Append("','");
							SQLString.Append(arrSongID[iUpdIndex]);
							SQLString.Append("','0')");
							m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
							m_SportsDBMgr.Close();
							//generate the Match INI file if required
							if((arrSendToPager.Length>0) && (iMustSendLen>0)) {
								iLogUpd++;
								//insert into LOG_BSK_LIVEGOAL
								LogSQLString.Remove(0,LogSQLString.Length);
								LogSQLString.Append("insert into LOG_BSK_LIVEGOAL (TIMEFLAG,Section,Act,League,Host,Guest,MatchDate,MatchTime,LvStatus,Rec_1_Host,Rec_1_Guest,Rec_2_Host,Rec_2_Guest,Rec_3_Host,Rec_3_Guest,Rec_4_Host,Rec_4_Guest,Rec_Ot1_Host,Rec_Ot1_Guest,Alert_Type,Song_ID,Remarks,BATCHJOB) values('");
								LogSQLString.Append(sCurrentTimestamp);
								LogSQLString.Append("','GOAL_','U','");
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
								LogSQLString.Append(arrMatchStatus[Convert.ToInt32(arrMatchStatusIdx[iUpdIndex].Trim())]);
								LogSQLString.Append("','");
								if(arrQuHScore1[iUpdIndex].Trim().Equals("")) arrQuHScore1[iUpdIndex] = "-1";
								LogSQLString.Append(arrQuHScore1[iUpdIndex]);		
								LogSQLString.Append("','");

								if(arrQuGScore1[iUpdIndex].Equals("")) arrQuGScore1[iUpdIndex] = "-1";
								LogSQLString.Append(arrQuGScore1[iUpdIndex]);
								LogSQLString.Append("','");

								if(arrQuHScore2[iUpdIndex].Trim().Equals("")) arrQuHScore2[iUpdIndex] = "-1";
								LogSQLString.Append(arrQuHScore2[iUpdIndex]);
								LogSQLString.Append("','");

								if(arrQuGScore2[iUpdIndex].Trim().Equals("")) arrQuGScore2[iUpdIndex] = "-1";
								LogSQLString.Append(arrQuGScore2[iUpdIndex]);
								LogSQLString.Append("','");

								if(arrQuHScore3[iUpdIndex].Equals("")) arrQuHScore3[iUpdIndex] = "-1";
								LogSQLString.Append(arrQuHScore3[iUpdIndex]);
								LogSQLString.Append("','");

								if(arrQuGScore3[iUpdIndex].Trim().Equals("")) arrQuGScore3[iUpdIndex] = "-1";
								LogSQLString.Append(arrQuGScore3[iUpdIndex]);
								LogSQLString.Append("','");

								if(arrQuHScore4[iUpdIndex].Trim().Equals("")) arrQuHScore4[iUpdIndex] = "-1";
								LogSQLString.Append(arrQuHScore4[iUpdIndex]);
								LogSQLString.Append("','");

								if(arrQuGScore4[iUpdIndex].Trim().Equals("")) arrQuGScore4[iUpdIndex] = "-1";
								LogSQLString.Append(arrQuGScore4[iUpdIndex]);
								LogSQLString.Append("','");

								if(arrOtHScore[iUpdIndex].Trim().Equals("")) arrOtHScore[iUpdIndex] = "-1";
								LogSQLString.Append(arrOtHScore[iUpdIndex]);
								LogSQLString.Append("','");

								if(arrOtGScore[iUpdIndex].Trim().Equals("")) arrOtGScore[iUpdIndex] = "-1";
								LogSQLString.Append(arrOtGScore[iUpdIndex]);
								LogSQLString.Append("','");

								LogSQLString.Append(iAlertType.ToString());
								LogSQLString.Append("','");
								LogSQLString.Append(arrSongID[iUpdIndex]);
								LogSQLString.Append("','");

								if(arrRemarks[iUpdIndex].Trim().Equals("")) arrRemarks[iUpdIndex] = "-1";
								LogSQLString.Append(arrRemarks[iUpdIndex]);
								LogSQLString.Append("','");
								LogSQLString.Append(sBatchJob);
								LogSQLString.Append("')");
								logDBMgr.ExecuteNonQuery(LogSQLString.ToString());
								logDBMgr.Close();
							} //insert into LOG_BSK_LIVEGOAL
						}
						if(iLogUpd > 0) {
							//Send Notify Message
						  //Modified by Henry, 11 Feb 2004
					    sptMsg.Body = sBatchJob;
							sptMsg.Timestamp = DateTime.Now.ToString("yyMMddHHmmss");
							sptMsg.AppID = "07";
							sptMsg.MsgID = "28"; //28為定義現場比數
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
									m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " MSMQ ERROR: Basketball Live Goal");
									m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " BSKLiveGoal.cs.UpdateLiveGoal(): Notify via MSMQ throws MessageQueueException:  " + mqEx.ToString());
									m_SportsLog.Close();

									//If MSMQ fail, notify via .NET Remoting
									msgClt.MessageType = arrMessageTypes[1];
									msgClt.MessagePath = arrRemotingPath[0];
									if(!msgClt.SendMessage((object)sptMsg)) {
										m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
										m_SportsLog.SetFileName(0,LOGFILESUFFIX);
										m_SportsLog.Open();
										m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Remoting ERROR: Basketball Live Goal");
										m_SportsLog.Close();
									}
								}	catch(Exception ex) {
									m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
									m_SportsLog.SetFileName(0,LOGFILESUFFIX);
									m_SportsLog.Open();
									m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Remoting ERROR: Basketball Live Goal");
									m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " BSKLiveGoal.cs.UpdateLiveGoal(): Notify via .NET Remoting throws exception: " + ex.ToString());
									m_SportsLog.Close();
								}							
							}	catch(Exception ex) {
								m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
								m_SportsLog.SetFileName(0,LOGFILESUFFIX);
								m_SportsLog.Open();
								m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " BSKLiveGoal.cs.UpdateLiveGoal(): Notify via MSMQ throws exception: " + ex.ToString());
								m_SportsLog.Close();
							}									
							//Modified end		
						}			
						//write log
						m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
						m_SportsLog.SetFileName(0,LOGFILESUFFIX);
						m_SportsLog.Open();
						m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " BSKLiveGoal.cs: Send " + iRecUpd.ToString() + " matches (" + HttpContext.Current.Session["user_name"] + ")");
						m_SportsLog.Close();
						if((iMustSendLen>0)&&(iRecUpd > 0))
							m_SportsDBMgr.Dispose();
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
						m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " BSKLiveGoal.cs: Sort " + iRecUpd.ToString() + " matches (" + HttpContext.Current.Session["user_name"] + ")");
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
								SQLString.Append(" or IMATCH_COUNT=" );
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
						m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " BSKLiveGoal.cs: Hide " + iRecUpd.ToString() + " matches (" + HttpContext.Current.Session["user_name"] + ")");
						m_SportsLog.Close();
						break;
					case "SHOW":	//case: Set all matches to visible
						SQLString.Remove(0,SQLString.Length);
						SQLString.Append("update NBAGAME_INFO set CWEB_VIEW='V'");
						iRecUpd = m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
						m_SportsDBMgr.Close();
						m_SportsDBMgr.Dispose();

						//write log
						m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
						m_SportsLog.SetFileName(0,LOGFILESUFFIX);
						m_SportsLog.Open();
						m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " BSKLiveGoal.cs: " + HttpContext.Current.Session["user_name"] + " set " + iRecUpd.ToString() + " matches to visible");
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
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " BSKLiveGoal.cs.UpdateLiveGoal(): " + ex.ToString());
				m_SportsLog.Close();
			}	
			return iRecUpd;
		}		
	}
}