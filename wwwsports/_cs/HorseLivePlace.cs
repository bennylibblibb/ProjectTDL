/*
Objective:
Send horse racing live place to backend process or send to pager via Remoting

Last updated:
04 Nov 2004, fanny
-on remoting, write log table and MSMQ message instead of web formatting message for combo device
14 Sep 2004, fanny
1. Generate liveplace message to combo device altogether
2. Replace message's tailing space with line feed
3. arrResult[6]: Message formatted flag for COMBO
4. arrResult[7]: Message formatted flag for COMBO Alert
5. Restricted to extract not more than 10 records
28 Jul 2004, Chapman Choi
28 Jul 2004 Additional field (CPREFIX) in QUEUE tables
27 Feb 2004 Enable display last record only when racing
23 Feb 2004 Enhance log indication, SendPlaceRemoting()
arrResult[0]: Message formatted flag for GOGO
arrResult[1]: Message formatted flag for HKJC
arrResult[2]: Message formatted flag for HORSE
arrResult[3]: Message notified flag for GOGO & HORSE
arrResult[4]: Message notified flag for HKJC
arrResult[5]: General process flag
19 Feb 2004 Enhance for multipath support

C#.NET complier statement:
csc /t:library /out:..\bin\HorseLivePlace.dll /r:..\bin\DBManager.dll;..\bin\Files.dll;..\bin\MessageClient.dll;..\bin\SportsMessage.dll;..\bin\MMMFormatter.dll;..\bin\Win32Message.dll HorseLivePlace.cs
*/

using System;
using System.Collections;
using System.Data.OleDb;
using System.Reflection;
using System.Text;
using System.Web;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using RemoteService.Win32;
using TDL.DB;
using TDL.IO;
using TDL.Message;
using PagerFormatter;

[assembly:AssemblyCompany("TDSL")]
[assembly:AssemblyCopyright("(c) 2003 TDSL. All rights reserved.")]
[assembly:AssemblyDescription("賽馬資訊 -> 現場走位")]
[assembly:AssemblyProduct("Sports.NET")]
[assembly:AssemblyTitle("Sports.NET DLL")]
[assembly:AssemblyVersion("3.2.*")]
namespace SportsUtil {
	public class HorseLivePlace {
		const string LOGFILESUFFIX = "log";
		DBManager m_SportsDBMgr;
		Files m_SportsLog;
		StringBuilder SQLString;
		Encoding m_Big5Encoded;
		OleDbDataReader m_SportsOleReader;
		char[] delimiter;
		string[] arrRaceTrack;
		string[] arrRacestatus;

		public HorseLivePlace(string Connection) {
			m_SportsDBMgr = new DBManager();
			m_SportsDBMgr.ConnectionString = Connection;
			m_SportsLog = new Files();
			SQLString = new StringBuilder();
			m_Big5Encoded = Encoding.GetEncoding(950);
			delimiter = new char[] {','};
			arrRaceTrack = (string[])HttpContext.Current.Application["HorseRaceTracks"];
			arrRacestatus = (string[])HttpContext.Current.Application["HorseRaceStatuses"];
		}

		public string GetPlace() {
			int iPlaceNo = 0;
			int iArrayIndex = 0;
			int iRecordCount = 0;
			string sRaceID;
			string sRaceStatus = "";
			string sRacePlace = "";
			StringBuilder HTMLString = new StringBuilder();

			try {
				sRaceID = HttpContext.Current.Request.QueryString["RaceID"].Trim();
				HTMLString.Append("<tr style=\"background-color:#7CFC00\" align=\"center\"><th colspan=\"2\">第");
				HTMLString.Append(sRaceID);
				HTMLString.Append("場<input type=\"hidden\" name=\"RaceID\" value=\"");
				HTMLString.Append(sRaceID);
				HTMLString.Append("\"></th>");
				SQLString.Remove(0,SQLString.Length);
				SQLString.Append("select CCLASS,CRACETRACK,CRACELENGTH,CRACESTATUS,CRACEDATE,CRACETIME from RACEINFO where IRACE_NO=");
				SQLString.Append(sRaceID);
				m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
				if(m_SportsOleReader.Read()) {
					HTMLString.Append("<th>");
					HTMLString.Append(m_SportsOleReader.GetString(0).Trim());
					HTMLString.Append("<input type=\"hidden\" name=\"RaceClass\" value=\"");
					HTMLString.Append(m_SportsOleReader.GetString(0).Trim());
					HTMLString.Append("\"></th><th>");
					HTMLString.Append(m_SportsOleReader.GetString(1).Trim());
					HTMLString.Append(m_SportsOleReader.GetString(2).Trim());
					HTMLString.Append("米<input type=\"hidden\" name=\"RaceTrack\" value=\"");
					HTMLString.Append(m_SportsOleReader.GetString(1).Trim());
					HTMLString.Append("\"><input type=\"hidden\" name=\"RaceLength\" value=\"");
					HTMLString.Append(m_SportsOleReader.GetString(2).Trim());
					HTMLString.Append("\"></th><th><select name=\"RaceStatus\">");
					if(!m_SportsOleReader.IsDBNull(3)) sRaceStatus = m_SportsOleReader.GetString(3).Trim();
					if(!sRaceStatus.Equals("")) {
						HTMLString.Append("<option value=\"");
						HTMLString.Append(sRaceStatus);
						HTMLString.Append("\">");
						HTMLString.Append(sRaceStatus);
					}
					for(iArrayIndex = 0; iArrayIndex < arrRacestatus.Length; iArrayIndex++) {
						if(!sRaceStatus.Equals(arrRacestatus[iArrayIndex])) {
							HTMLString.Append("<option value=\"");
							HTMLString.Append(arrRacestatus[iArrayIndex]);
							HTMLString.Append("\">");
							HTMLString.Append(arrRacestatus[iArrayIndex]);
						}
					}
					HTMLString.Append("</select><input type=\"hidden\" name=\"RaceDate\" value=\"");
					HTMLString.Append(m_SportsOleReader.GetString(4).Trim());
					HTMLString.Append("\"><input type=\"hidden\" name=\"RaceTime\" value=\"");
					HTMLString.Append(m_SportsOleReader.GetString(5).Trim());
					HTMLString.Append("\"></th>");
				}
				m_SportsDBMgr.Close();
				m_SportsOleReader.Close();

				HTMLString.Append("</tr><tr align=\"center\"><th>現在:</th><td><input name=\"First\" value=\"\" maxlength=\"2\" size=\"1\" onChange=\"FirstValidity()\"></td>");
				HTMLString.Append("<td><input name=\"Second\" value=\"\" maxlength=\"2\" size=\"1\" onChange=\"SecondValidity()\"></td>");
				HTMLString.Append("<td><input name=\"Third\" value=\"\" maxlength=\"2\" size=\"1\" onChange=\"ThirdValidity()\"></td>");
				HTMLString.Append("<td><input name=\"Fourth\" value=\"\" maxlength=\"2\" size=\"1\" onChange=\"FourthValidity()\"></td></tr>");

				SQLString.Remove(0,SQLString.Length);
				SQLString.Append("select IPLACE_NO, CFIRST,CSECOND,CTHIRD,CFOURTH,CRACESTATUS from RACEPLACEINFO where IRACE_NO=");
				SQLString.Append(sRaceID);
				SQLString.Append(" and IPLACE_NO > (select max(IPLACE_NO) from RACEPLACEINFO where IRACE_NO="+sRaceID+")-10");
				SQLString.Append(" order by IPLACE_NO desc");
				m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
				while(m_SportsOleReader.Read()) {
					iRecordCount++;
					if(!m_SportsOleReader.IsDBNull(0)) iPlaceNo = m_SportsOleReader.GetInt32(0);
					else iPlaceNo = 0;
					if(iRecordCount == 1) {
						HTMLString.Append("<tr align=\"center\"><th>上次:</th><td><input type=\"hidden\" name=\"NextPlaceNo\" value=\"");
						HTMLString.Append((iPlaceNo+1).ToString());
						HTMLString.Append("\">");
						if(!m_SportsOleReader.IsDBNull(1)) sRacePlace = m_SportsOleReader.GetString(1).Trim();
						else sRacePlace = "";
						HTMLString.Append(sRacePlace);
						HTMLString.Append("<input type=\"hidden\" name=\"HistoryFirst\" value=\"");
						if(sRacePlace.Equals("")) HTMLString.Append("0");
						else HTMLString.Append(sRacePlace);
						HTMLString.Append("\"></td><td>");
						if(!m_SportsOleReader.IsDBNull(2)) sRacePlace = m_SportsOleReader.GetString(2).Trim();
						else sRacePlace = "";
						HTMLString.Append(sRacePlace);
						HTMLString.Append("<input type=\"hidden\" name=\"HistorySecond\" value=\"");
						if(sRacePlace.Equals("")) HTMLString.Append("0");
						else HTMLString.Append(sRacePlace);
						HTMLString.Append("\"></td><td>");
						if(!m_SportsOleReader.IsDBNull(3)) sRacePlace = m_SportsOleReader.GetString(3).Trim();
						else sRacePlace = "";
						HTMLString.Append(sRacePlace);
						HTMLString.Append("<input type=\"hidden\" name=\"HistoryThird\" value=\"");
						if(sRacePlace.Equals("")) HTMLString.Append("0");
						else HTMLString.Append(sRacePlace);
						HTMLString.Append("\"></td><td>");
						if(!m_SportsOleReader.IsDBNull(4)) sRacePlace = m_SportsOleReader.GetString(4).Trim();
						else sRacePlace = "";
						HTMLString.Append(sRacePlace);
						HTMLString.Append("<input type=\"hidden\" name=\"HistoryFourth\" value=\"");
						if(sRacePlace.Equals("")) HTMLString.Append("0");
						else HTMLString.Append(sRacePlace);
						HTMLString.Append("\">");
						if(!m_SportsOleReader.IsDBNull(5)) sRaceStatus = m_SportsOleReader.GetString(5).Trim();
						else sRaceStatus = "";
						HTMLString.Append("<input type=\"hidden\" name=\"HistoryStatus\" value=\"");
						HTMLString.Append(sRaceStatus);
						HTMLString.Append("\"></td></tr>");
					} else {
						if(!m_SportsOleReader.IsDBNull(5)) sRaceStatus = m_SportsOleReader.GetString(5).Trim();
						else sRaceStatus = "";
						//if status is not 跑完 or 作實
						//if(!(sRaceStatus.Equals(arrRacestatus[7]) || sRaceStatus.Equals(arrRacestatus[11]))) {
							HTMLString.Append("<input type=\"hidden\" name=\"NextPlaceNo\" value=\"");
							HTMLString.Append((iPlaceNo+1).ToString());
							HTMLString.Append("\">");
							if(!m_SportsOleReader.IsDBNull(1)) sRacePlace = m_SportsOleReader.GetString(1).Trim();
							if(sRacePlace.Trim().Equals("")) sRacePlace = "0";
							HTMLString.Append("<input type=\"hidden\" name=\"HistoryFirst\" value=\"");
							HTMLString.Append(sRacePlace);
							HTMLString.Append("\">");
							if(!m_SportsOleReader.IsDBNull(2)) sRacePlace = m_SportsOleReader.GetString(2).Trim();
							if(sRacePlace.Trim().Equals("")) sRacePlace = "0";
							HTMLString.Append("<input type=\"hidden\" name=\"HistorySecond\" value=\"");
							HTMLString.Append(sRacePlace);
							HTMLString.Append("\">");
							if(!m_SportsOleReader.IsDBNull(3)) sRacePlace = m_SportsOleReader.GetString(3).Trim();
							if(sRacePlace.Trim().Equals("")) sRacePlace = "0";
							HTMLString.Append("<input type=\"hidden\" name=\"HistoryThird\" value=\"");
							HTMLString.Append(sRacePlace);
							HTMLString.Append("\">");
							if(!m_SportsOleReader.IsDBNull(4)) sRacePlace = m_SportsOleReader.GetString(4).Trim();
							if(sRacePlace.Trim().Equals("")) sRacePlace = "0";
							HTMLString.Append("<input type=\"hidden\" name=\"HistoryFourth\" value=\"");
							HTMLString.Append(sRacePlace);
							HTMLString.Append("\"><input type=\"hidden\" name=\"HistoryStatus\" value=\"");
							HTMLString.Append(sRaceStatus);
							HTMLString.Append("\">");
						//}
					}
				}
				m_SportsDBMgr.Close();
				m_SportsOleReader.Close();
				m_SportsDBMgr.Dispose();

				if(iRecordCount == 0) {
					HTMLString.Append("<tr align=\"center\"><th>上次:</th><td><input type=\"hidden\" name=\"NextPlaceNo\" value=\"1\"><input type=\"hidden\" name=\"HistoryFirst\" value=\"\"></td>");
					HTMLString.Append("<td><input type=\"hidden\" name=\"HistorySecond\" value=\"\"></td>");
					HTMLString.Append("<td><input type=\"hidden\" name=\"HistoryThird\" value=\"\"></td>");
					HTMLString.Append("<td><input type=\"hidden\" name=\"HistoryFourth\" value=\"\">");
					HTMLString.Append("<input type=\"hidden\" name=\"HistoryStatus\" value=\"\"></td></tr>");
				}
			} catch(Exception ex) {
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " HorseLivePlace.cs.GetPlace(): " + ex.ToString());
				m_SportsLog.Close();
				HTMLString.Remove(0,HTMLString.Length);
				HTMLString.Append((string)HttpContext.Current.Application["accessErrorMsg"]);
			}

			return HTMLString.ToString();
		}

		//Send message to MSMQ and then notify Sender to process received INI file
		public int SendPlace() {
			int iRecordSent = 0;
			int iArrayIndex = 0;
			int iHistoryCount = 0;
			string sRaceID;
			string sRaceFirst;
			string sRaceSecond;
			string sRaceThird;
			string sRaceFourth;
			string sRaceStatus;
			string sRaceAlert;
			string sCurrentTimestamp = null;
			string sBatchJob = null;
			string[] arrSendToPager;
			string[] arrRacePlaceNo;
			string[] arrRaceHistoryFirst;
			string[] arrRaceHistorySecond;
			string[] arrRaceHistoryThird;
			string[] arrRaceHistoryFourth;
			string[] arrRaceHistoryStatus;
			string[] arrMsgType;

			//Delcare variable used in message notify
			string[] arrQueueNames;
			string[] arrRemotingPath;
			string[] arrMessageTypes;
			arrQueueNames = (string[])HttpContext.Current.Application["QueueItems"];
			arrRemotingPath = (string[])HttpContext.Current.Application["RemotingItems"];
			arrMessageTypes = (string[])HttpContext.Current.Application["NotifyMessageTypes"];
			SportsMessage sptMsg = new SportsMessage();
			MessageClient msgClt = new MessageClient();

			try {
				arrSendToPager = HttpContext.Current.Request.Form["SendToPager"].Split(delimiter);
			} catch(Exception) {
				arrSendToPager = new string[0];
			}

			sRaceID = HttpContext.Current.Request.Form["RaceID"];
			sRaceFirst = HttpContext.Current.Request.Form["First"];
			if(sRaceFirst.Equals("")) sRaceFirst = "0";
			sRaceSecond = HttpContext.Current.Request.Form["Second"];
			if(sRaceSecond.Equals("")) sRaceSecond = "0";
			sRaceThird = HttpContext.Current.Request.Form["Third"];
			if(sRaceThird.Equals("")) sRaceThird = "0";
			sRaceFourth = HttpContext.Current.Request.Form["Fourth"];
			if(sRaceFourth.Equals("")) sRaceFourth = "0";
			sRaceStatus = HttpContext.Current.Request.Form["RaceStatus"];
			sRaceAlert = HttpContext.Current.Request.Form["PlaceAlert"];
			if(sRaceAlert == null) sRaceAlert = "0";

			arrRacePlaceNo = HttpContext.Current.Request.Form["NextPlaceNo"].Split(delimiter);
			arrRaceHistoryFirst = HttpContext.Current.Request.Form["HistoryFirst"].Split(delimiter);
			arrRaceHistorySecond = HttpContext.Current.Request.Form["HistorySecond"].Split(delimiter);
			arrRaceHistoryThird = HttpContext.Current.Request.Form["HistoryThird"].Split(delimiter);
			arrRaceHistoryFourth = HttpContext.Current.Request.Form["HistoryFourth"].Split(delimiter);
			arrRaceHistoryStatus = HttpContext.Current.Request.Form["HistoryStatus"].Split(delimiter);
			iHistoryCount = arrRacePlaceNo.Length;

			try {
				iRecordSent++;
				arrMsgType = (string[])HttpContext.Current.Application["messageType"];
				sCurrentTimestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
				sBatchJob = DateTime.Now.ToString("yyMMddHHmmss_fffffff") + "." + HttpContext.Current.Session["user_name"].ToString() + "." + arrMsgType[16] + ".ini";

				//Insert log into LOG_LIVEPLACE
				SQLString.Remove(0,SQLString.Length);
				SQLString.Append("insert into LOG_LIVEPLACE (TIMEFLAG, IITEMSEQ_NO, SECTION, RACEDATE, RACETIME, RACETRACK, RACENO, RACECLASS, RACELENGTH, RACEALERT, RACE_STATUS, RACE_1, RACE_2, RACE_3, RACE_4, BATCHJOB) values ('");
				SQLString.Append(sCurrentTimestamp);
				SQLString.Append("',1,'LIVEPLACE','");
				SQLString.Append(HttpContext.Current.Request.Form["RaceDate"]);
				SQLString.Append("','");
				SQLString.Append(HttpContext.Current.Request.Form["RaceTime"]);
				SQLString.Append("','");
				SQLString.Append(HttpContext.Current.Request.Form["RaceTrack"]);
				SQLString.Append("',");
				SQLString.Append(sRaceID);
				SQLString.Append(",'");
				SQLString.Append(HttpContext.Current.Request.Form["RaceClass"]);
				SQLString.Append("','");
				SQLString.Append(HttpContext.Current.Request.Form["RaceLength"]);
				SQLString.Append("米',");
				SQLString.Append(sRaceAlert);
				SQLString.Append(",'");
				SQLString.Append(sRaceStatus);
				SQLString.Append("',");
				SQLString.Append(sRaceFirst);
				SQLString.Append(",");
				SQLString.Append(sRaceSecond);
				SQLString.Append(",");
				SQLString.Append(sRaceThird);
				SQLString.Append(",");
				SQLString.Append(sRaceFourth);
				SQLString.Append(",'");
				SQLString.Append(sBatchJob);
				SQLString.Append("')");
				m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
				m_SportsDBMgr.Close();

				if(Convert.ToInt32(arrRacePlaceNo[0]) > 1) {	//History records existed
					for(iArrayIndex = 0; iArrayIndex < iHistoryCount; iArrayIndex++) {
						SQLString.Remove(0,SQLString.Length);
						SQLString.Append("insert into LOG_LIVEPLACE (TIMEFLAG, IITEMSEQ_NO, SECTION, RACEDATE, RACETIME, RACETRACK, RACENO, RACECLASS, RACELENGTH, RACEALERT, RACE_STATUS, RACE_1, RACE_2, RACE_3, RACE_4, BATCHJOB) values ('");
						SQLString.Append(sCurrentTimestamp);
						SQLString.Append("',");
						SQLString.Append((iArrayIndex+2).ToString());
						SQLString.Append(",'LIVEPLACE','");
						SQLString.Append(HttpContext.Current.Request.Form["RaceDate"]);
						SQLString.Append("','");
						SQLString.Append(HttpContext.Current.Request.Form["RaceTime"]);
						SQLString.Append("','");
						SQLString.Append(HttpContext.Current.Request.Form["RaceTrack"]);
						SQLString.Append("',");
						SQLString.Append(sRaceID);
						SQLString.Append(",'");
						SQLString.Append(HttpContext.Current.Request.Form["RaceClass"]);
						SQLString.Append("','");
						SQLString.Append(HttpContext.Current.Request.Form["RaceLength"]);
						SQLString.Append("米',");
						SQLString.Append(sRaceAlert);
						SQLString.Append(",'");
						SQLString.Append(arrRaceHistoryStatus[iArrayIndex]);
						SQLString.Append("',");
						SQLString.Append(arrRaceHistoryFirst[iArrayIndex]);
						SQLString.Append(",");
						SQLString.Append(arrRaceHistorySecond[iArrayIndex]);
						SQLString.Append(",");
						SQLString.Append(arrRaceHistoryThird[iArrayIndex]);
						SQLString.Append(",");
						SQLString.Append(arrRaceHistoryFourth[iArrayIndex]);
						SQLString.Append(",'");
						SQLString.Append(sBatchJob);
						SQLString.Append("')");
						m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
						m_SportsDBMgr.Close();
					}
				}

				//Tell MessageDispatcher to generate LivePlace INI and notify other processes
				//Assign value to SportsMessage object
				sptMsg.Body = sBatchJob;
				sptMsg.Timestamp = DateTime.Now.ToString("yyMMddHHmmss");
				sptMsg.AppID = "07";
				sptMsg.MsgID = "04";
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
						m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " MSMQ ERROR: Horse Racing Live Place");
						m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " HorseLivePlace.cs.SendPlace(): Notify via MSMQ throws MessageQueueException: " + mqEx.ToString());
						m_SportsLog.Close();

						//If MSMQ fail, notify via .NET Remoting
						msgClt.MessageType = arrMessageTypes[1];
						msgClt.MessagePath = arrRemotingPath[0];
						if(!msgClt.SendMessage((object)sptMsg)) {
							m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
							m_SportsLog.SetFileName(0,LOGFILESUFFIX);
							m_SportsLog.Open();
							m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Remoting ERROR: Horse Racing Live Place");
							m_SportsLog.Close();
						}
					}	catch(Exception ex) {
						m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
						m_SportsLog.SetFileName(0,LOGFILESUFFIX);
						m_SportsLog.Open();
						m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Remoting ERROR: Horse Racing Live Place");
						m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " HorseLivePlace.cs.SendPlace(): Notify via .NET Remoting throws exception: " + ex.ToString());
						m_SportsLog.Close();
					}
				}	catch(Exception ex) {
					m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
					m_SportsLog.SetFileName(0,LOGFILESUFFIX);
					m_SportsLog.Open();
					m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " HorseLivePlace.cs.SendPlace(): Notify via MSMQ throws exception: " + ex.ToString());
					m_SportsLog.Close();
				}

				//update RACEINFO and RACEPLACEINFO
				SQLString.Remove(0,SQLString.Length);
				SQLString.Append("update RACEINFO set CRACESTATUS='");
				SQLString.Append(sRaceStatus);
				SQLString.Append("' where IRACE_NO=");
				SQLString.Append(sRaceID);
				m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
				m_SportsDBMgr.Close();
				if(sRaceFirst.Equals("0")) sRaceFirst = "";
				if(sRaceSecond.Equals("0")) sRaceSecond = "";
				if(sRaceThird.Equals("0")) sRaceThird = "";
				if(sRaceFourth.Equals("0")) sRaceFourth = "";
				SQLString.Remove(0,SQLString.Length);
				SQLString.Append("insert into RACEPLACEINFO values (");
				SQLString.Append(arrRacePlaceNo[0]);
				SQLString.Append(",");
				SQLString.Append(sRaceID);
				SQLString.Append(",'");
				SQLString.Append(sRaceFirst);
				SQLString.Append("','");
				SQLString.Append(sRaceSecond);
				SQLString.Append("','");
				SQLString.Append(sRaceThird);
				SQLString.Append("','");
				SQLString.Append(sRaceFourth);
				SQLString.Append("','0','");
				SQLString.Append(sRaceStatus);
				SQLString.Append("')");
				m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
				m_SportsDBMgr.Close();
				m_SportsDBMgr.Dispose();
			} catch(Exception ex) {
				iRecordSent = -1;
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " HorseLivePlace.cs.SendPlace(): " + ex.ToString());
				m_SportsLog.Close();
			}

			return iRecordSent;
		}

		//Format message immediately and send to pager via .NET Remoting
		public int[] SendPlaceRemoting() {
			const int SUCCESS_CODE = 100000;
			//bool RaceStart = false;
			int iGOGOFormatted = 0;
			int iHKJCFormatted = 0;
			int iHORSEFormatted = 0;
			//int iCOMBOFormatted = 0;
			//int iCOMBOAlertFormatted = 0;
			int iArrayIndex = 0;
			int iHistoryCount = 0;
			string sRaceTrack;
			string sRaceDate;
			string sRaceTime;
			string sRaceID;
			string sRaceClass;
			string sRaceLength;
			string sRaceFirst;
			string sRaceSecond;
			string sRaceThird;
			string sRaceFourth;
			string sRaceStatus;
			string sRaceAlert;
			//int[] arrResult = new int[8];
			int[] arrResult = new int[6];
			string[] arrRacePlaceNo;
			string[] arrRaceHistoryFirst;
			string[] arrRaceHistorySecond;
			string[] arrRaceHistoryThird;
			string[] arrRaceHistoryFourth;
			string[] arrRaceHistoryStatus;
			ArrayList encodedList = new ArrayList();
			ArrayList encodedFinList = new ArrayList();
			ArrayList cb_encodedFinList = new ArrayList();
			//ArrayList comboAlertResultMsg = new ArrayList();	//Used to construct alert string for combo pager only
			StringBuilder PagerScreen = new StringBuilder();	//Used to construct Pager Screen

			DBManager SptAppCfgDBMgr = new DBManager();
			SptAppCfgDBMgr.ConnectionString = (string)HttpContext.Current.Application["SportAppCfgConnectionString"];
			MMMFormatter lvplFormatter = new MMMFormatter();	//LivePlace 3M formatter

			sRaceTrack = HttpContext.Current.Request.Form["RaceTrack"];
			sRaceDate = HttpContext.Current.Request.Form["RaceDate"];
			sRaceTime = HttpContext.Current.Request.Form["RaceTime"];
			sRaceID = HttpContext.Current.Request.Form["RaceID"];
			sRaceClass = HttpContext.Current.Request.Form["RaceClass"];
			sRaceLength = HttpContext.Current.Request.Form["RaceLength"];
			sRaceFirst = HttpContext.Current.Request.Form["First"];
			if(sRaceFirst.Equals("0")) sRaceFirst = "";
			sRaceSecond = HttpContext.Current.Request.Form["Second"];
			if(sRaceSecond.Equals("0")) sRaceSecond = "";
			sRaceThird = HttpContext.Current.Request.Form["Third"];
			if(sRaceThird.Equals("0")) sRaceThird = "";
			sRaceFourth = HttpContext.Current.Request.Form["Fourth"];
			if(sRaceFourth.Equals("0")) sRaceFourth = "";
			sRaceStatus = HttpContext.Current.Request.Form["RaceStatus"];
			sRaceAlert = HttpContext.Current.Request.Form["PlaceAlert"];
			if(sRaceAlert == null) sRaceAlert = "0";
			if(!sRaceAlert.Trim().Equals("1")) sRaceAlert = "0";

			arrRacePlaceNo = HttpContext.Current.Request.Form["NextPlaceNo"].Split(delimiter);
			arrRaceHistoryFirst = HttpContext.Current.Request.Form["HistoryFirst"].Split(delimiter);
			arrRaceHistorySecond = HttpContext.Current.Request.Form["HistorySecond"].Split(delimiter);
			arrRaceHistoryThird = HttpContext.Current.Request.Form["HistoryThird"].Split(delimiter);
			arrRaceHistoryFourth = HttpContext.Current.Request.Form["HistoryFourth"].Split(delimiter);
			arrRaceHistoryStatus = HttpContext.Current.Request.Form["HistoryStatus"].Split(delimiter);
			iHistoryCount = arrRacePlaceNo.Length;

			try {
				m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " HorseLivePlace.cs: [RECV] Constructing pager content (" + HttpContext.Current.Session["user_name"] + ")");
				m_SportsLog.Close();

				//clear buffer
				PagerScreen.Remove(0, PagerScreen.Length);
				if (encodedList.Count>0) encodedList.Clear();
				if (encodedFinList.Count>0) encodedFinList.Clear();
				if (cb_encodedFinList.Count>0) cb_encodedFinList.Clear();

				//Construct pager screen
				//race match datetime
				PagerScreen.Append(PadRightSpace(sRaceTrack + " " + sRaceDate.Substring(4,2) + "/" + sRaceDate.Substring(6) + " " + sRaceTime.Substring(0,2) + ":" + sRaceTime.Substring(2), lvplFormatter.PagerScreenWidth));
				encodedList.AddRange((byte[])lvplFormatter.ConvertToCNS(PagerScreen.ToString()));
				//prepare for non-combo pager
				encodedFinList.AddRange((byte[])lvplFormatter.PadShift((byte[])encodedList.ToArray(typeof(byte))));
				//prepare for combo pager
				cb_encodedFinList.AddRange((byte[])lvplFormatter.PadShift((byte[])encodedList.ToArray(typeof(byte))));
				cb_encodedFinList.Add(lvplFormatter.Linefeed);

				//clear buffer
				PagerScreen.Remove(0,PagerScreen.Length);
				if (encodedList.Count>0) encodedList.Clear();

				//race match information
				PagerScreen.Append(PadRightSpace("第" + sRaceID + "場 " + sRaceClass + " " + sRaceLength + "米", lvplFormatter.PagerScreenWidth));
				encodedList.AddRange((byte[])lvplFormatter.ConvertToCNS(PagerScreen.ToString()));
				//prepare for non-combo pager
				encodedFinList.AddRange((byte[])lvplFormatter.PadShift((byte[])encodedList.ToArray(typeof(byte))));
				//prepare for combo pager
				cb_encodedFinList.AddRange((byte[])lvplFormatter.PadShift((byte[])encodedList.ToArray(typeof(byte))));
				cb_encodedFinList.Add(lvplFormatter.Linefeed);

				//clear buffer
				PagerScreen.Remove(0,PagerScreen.Length);
				if (encodedList.Count>0) encodedList.Clear();

				//Check status is not (未開賽 or 入閘 or 開跑)
				if(!(sRaceStatus.Equals(arrRacestatus[0]) || sRaceStatus.Equals(arrRacestatus[1]) || sRaceStatus.Equals(arrRacestatus[2]))) {
					//prepare for non-combo pager
					encodedFinList.Add(lvplFormatter.ShiftIn);
					encodedFinList.Add(lvplFormatter.HighLight);
					//prepare for combo pager
					cb_encodedFinList.Add(lvplFormatter.ShiftIn);
					cb_encodedFinList.Add(lvplFormatter.HighLight);

					PagerScreen.Append(PadRightSpace(sRaceFirst.Trim(), 4));
					PagerScreen.Append(PadRightSpace(sRaceSecond.Trim(), 4));
					PagerScreen.Append(PadRightSpace(sRaceThird.Trim(), 4));
					PagerScreen.Append(PadRightSpace(sRaceFourth.Trim(), 2));
					PagerScreen.Append(PadRightSpace(sRaceStatus, 6));
					encodedList.AddRange((byte[])lvplFormatter.ConvertToCNS(PagerScreen.ToString()));

					//prepare for non-combo pager
					encodedFinList.AddRange((byte[])lvplFormatter.PadShift((byte[])encodedList.ToArray(typeof(byte))));
					encodedFinList.Add(lvplFormatter.ShiftIn);
					encodedFinList.Add(lvplFormatter.HighLight);
					//prepare for combo pager
					cb_encodedFinList.AddRange((byte[])lvplFormatter.PadShift((byte[])encodedList.ToArray(typeof(byte))));
					cb_encodedFinList.Add(lvplFormatter.ShiftIn);
					cb_encodedFinList.Add(lvplFormatter.HighLight);
					cb_encodedFinList.Add(lvplFormatter.Linefeed);

					//RaceStart = true;
				} else {
					PagerScreen.Append(PadRightSpace(sRaceStatus, lvplFormatter.PagerScreenWidth));
					//convert status row into cns and add into final list
					encodedList.AddRange((byte[])lvplFormatter.ConvertToCNS(PagerScreen.ToString()));
					//prepare for non-combo pager
					encodedFinList.AddRange((byte[])lvplFormatter.PadShift((byte[])encodedList.ToArray(typeof(byte))));
					//prepare for combo pager
					cb_encodedFinList.AddRange((byte[])lvplFormatter.PadShift((byte[])encodedList.ToArray(typeof(byte))));
					cb_encodedFinList.Add(lvplFormatter.Linefeed);
					//RaceStart = false;
				}

				//clear buffer
				PagerScreen.Remove(0,PagerScreen.Length);
				if (encodedList.Count>0) encodedList.Clear();

				//Other rows of live place
				if(Convert.ToInt32(arrRacePlaceNo[0]) > 1) {	//History records existed
					for(iArrayIndex = 0; iArrayIndex < iHistoryCount; iArrayIndex++) {
						if(iArrayIndex < 5) {	//BLOB filed limit to about 240 bytes
							if(!(arrRaceHistoryStatus[iArrayIndex].Equals(arrRacestatus[0]) || arrRaceHistoryStatus[iArrayIndex].Equals(arrRacestatus[1]) || arrRaceHistoryStatus[iArrayIndex].Equals(arrRacestatus[2]))) {
								if(!arrRaceHistoryFirst[iArrayIndex].Equals("0")) {
									PagerScreen.Append(PadRightSpace(arrRaceHistoryFirst[iArrayIndex], 4));
								} else {
									PagerScreen.Append(PadRightSpace(" ", 4));
								}
								if(!arrRaceHistorySecond[iArrayIndex].Equals("0")) {
									PagerScreen.Append(PadRightSpace(arrRaceHistorySecond[iArrayIndex], 4));
								} else {
									PagerScreen.Append(PadRightSpace(" ", 4));
								}
								if(!arrRaceHistoryThird[iArrayIndex].Equals("0")) {
									PagerScreen.Append(PadRightSpace(arrRaceHistoryThird[iArrayIndex], 4));
								} else {
									PagerScreen.Append(PadRightSpace(" ", 4));
								}
								if(!arrRaceHistoryFourth[iArrayIndex].Equals("0")) {
									PagerScreen.Append(PadRightSpace(arrRaceHistoryFourth[iArrayIndex], 2));
								} else {
									PagerScreen.Append(PadRightSpace(" ", 2));
								}
								PagerScreen.Append(PadRightSpace(arrRaceHistoryStatus[iArrayIndex], 6));
								encodedList.AddRange((byte[])lvplFormatter.ConvertToCNS(PagerScreen.ToString()));
								//prepare for non-combo pager
								encodedFinList.AddRange((byte[])lvplFormatter.PadShift((byte[])encodedList.ToArray(typeof(byte))));
								//don't insert linefeed char at the last line of page
								//prepare for combo pager
								cb_encodedFinList.AddRange((byte[])lvplFormatter.PadShift((byte[])encodedList.ToArray(typeof(byte))));
								cb_encodedFinList.Add(lvplFormatter.Linefeed);

							} else {
								if(!arrRaceHistoryStatus[iArrayIndex].Equals(arrRacestatus[0])) {
									PagerScreen.Append(PadRightSpace(arrRaceHistoryStatus[iArrayIndex], lvplFormatter.PagerScreenWidth));
									encodedList.AddRange((byte[])lvplFormatter.ConvertToCNS(PagerScreen.ToString()));
									//prepare for non-combo pager
									encodedFinList.AddRange((byte[])lvplFormatter.PadShift((byte[])encodedList.ToArray(typeof(byte))));
									//don't insert linefeed char at the last line of page
									//prepare for combo pager
									cb_encodedFinList.AddRange((byte[])lvplFormatter.PadShift((byte[])encodedList.ToArray(typeof(byte))));
									cb_encodedFinList.Add(lvplFormatter.Linefeed);
								}
							}
						} else {
							break;
						}
						//Break the loop if status is not (跑完 or 作實)
						if(!(sRaceStatus.Equals(arrRacestatus[7]) || sRaceStatus.Equals(arrRacestatus[11]))) {
							break;
						}
						//clear buffer
						PagerScreen.Remove(0,PagerScreen.Length);
						if (encodedList.Count>0) encodedList.Clear();
					}//end-for
				}//end-if: check history record exist or not

				//set array fit to size
				encodedFinList.TrimToSize();
				cb_encodedFinList.TrimToSize();

				//encodedList.AddRange((byte[])lvplFormatter.ConvertToCNS(PagerScreen.ToString()));
				//encodedList.TrimToSize();

				//Append Highlight for the 1st entry if race started
				/*
				if(RaceStart) {
					encodedList.Insert(40, lvplFormatter.HighLight);
					encodedList.Insert(55, lvplFormatter.HighLight);
				}
				*/

				//Retrieve pager information
				const string HKLVPL = "HKlivePlace";
				const string MCLVPL = "MClivePlace";
				const string UPDATE_SIGN = "+";
				const string PREFIXGROUP = "999999999999";
				bool GOGOFormatted = true;
				bool HKJCFormatted = true;
				bool HORSEFormatted = true;
				//bool COMBOFormatted = true;
				//bool COMBOAlertFormatted = true;
				int iMsgID = 0;
				int iPriority = 1;
				string sPrefix = "";
				string sCapcode = "";
				string sTone = "";
				string sGOGOHeaderID = "";
				string sHKJCHeaderID = "";
				string sHorseHeaderID = "";
				//string sComboHeaderID = "";
				//string sComboAlertHeaderID = "";
				string sSong = "0000";
				string sGOGODB = "";
				string sHKJCDB = "";

				//GOGO Pager include GOGO1 and GOGO2
				SQLString.Remove(0, SQLString.Length);
				SQLString.Append("select IMSG_ID, CPREFIX, CCAPCODE, CTONE, IHDRIDSTART, CSONGID, IPRIORITY from PGMSG_GOGO1_CFG where CMSGTYPE='");
				if(sRaceTrack.Equals(arrRaceTrack[2])) {	//Macau
					SQLString.Append(MCLVPL);
				} else {	//Hong Kong
					SQLString.Append(HKLVPL);
				}
				SQLString.Append("'");
				m_SportsOleReader = SptAppCfgDBMgr.ExecuteQuery(SQLString.ToString());
				if(m_SportsOleReader.Read()) {
					iMsgID = m_SportsOleReader.GetInt32(0);
					sPrefix = m_SportsOleReader.GetString(1).Trim();
					sCapcode = m_SportsOleReader.GetString(2).Trim();
					sTone = m_SportsOleReader.GetString(3).Trim();
					sGOGOHeaderID = m_SportsOleReader.GetInt32(4).ToString("D4");
					if(!sRaceAlert.Equals("0")) {
						sSong = m_SportsOleReader.GetString(5).Trim();
					}
					iPriority = m_SportsOleReader.GetInt32(6);
				}
				m_SportsOleReader.Close();
				SptAppCfgDBMgr.Close();


				//Unboxing return value of lvplFormatter.FormatMessage to original type, i.e. byte[]
				byte[] level1Content = (byte[])encodedFinList.ToArray(typeof(byte));
				//byte[] level1Content = (byte[])lvplFormatter.PadShift((byte[])encodedList.ToArray(typeof(byte)));
				byte[] level2Content = (byte[])lvplFormatter.InsertPrefixGroup(sSong + PREFIXGROUP, level1Content);
				byte[] gogoLevel3Content = (byte[])lvplFormatter.InsertHeaderWithShiftIn(UPDATE_SIGN + sGOGOHeaderID, level2Content);
				byte[] gogoResultMsg = (byte[])lvplFormatter.InsertHeaderFromString(sPrefix + sCapcode + sTone, gogoLevel3Content);

				//Check queue to be inserted
				SQLString.Remove(0, SQLString.Length);
				SQLString.Append("select db.CDBCONN, queue.CSQL, queue.CCHANNEL from MSGREG_LVPL_CFG reg, IPMUXADPT_DBCFG db, QUEUE_SQLCFG queue where reg.CPAGER='GOGO1' and reg.IMSG_ID=");
				SQLString.Append(iMsgID.ToString());
				SQLString.Append(" and reg.IENABLED=1 and reg.IDB_ID=db.IDB_ID and reg.IQUEUE_ID=queue.IQUEUE_ID order by reg.IDB_ID, reg.IQUEUE_ID");
				m_SportsOleReader = SptAppCfgDBMgr.ExecuteQuery(SQLString.ToString());
				while(m_SportsOleReader.Read()) {
					if(m_SportsOleReader.IsDBNull(0)) break;
					if(sGOGODB.Equals("")) sGOGODB = m_SportsOleReader.GetString(0).Trim();
					try {
						OleDbConnection queueConn = new OleDbConnection(m_SportsOleReader.GetString(0).Trim());
						OleDbCommand queueCmd = new OleDbCommand(m_SportsOleReader.GetString(1).Trim(), queueConn);
						queueCmd.Parameters.Add(new OleDbParameter("PRI", OleDbType.Integer));
						queueCmd.Parameters[0].Value = iPriority;

						queueCmd.Parameters.Add(new OleDbParameter("HDRID", OleDbType.VarChar, 7));
						queueCmd.Parameters[1].Value = sGOGOHeaderID;

						queueCmd.Parameters.Add(new OleDbParameter("VQUERY", OleDbType.VarChar, 1000));
						queueCmd.Parameters[2].Value = "-1";

						queueCmd.Parameters.Add(new OleDbParameter("CAPCODE", OleDbType.VarChar, 10));
						queueCmd.Parameters[3].Value = sCapcode;

						queueCmd.Parameters.Add(new OleDbParameter("BQUERY", OleDbType.VarBinary));
						queueCmd.Parameters[4].Value = gogoResultMsg;

						queueCmd.Parameters.Add(new OleDbParameter("CPREFIX", OleDbType.VarChar, 5));
						queueCmd.Parameters[5].Value = sPrefix;

						queueConn.Open();
						queueCmd.ExecuteNonQuery();
						queueConn.Close();

						m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
						m_SportsLog.SetFileName(0,LOGFILESUFFIX);
						m_SportsLog.Open();
						m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " HorseLivePlace.cs: [SENT] GOGO - " + m_SportsOleReader.GetString(2).Trim() + " (" + m_Big5Encoded.GetString(gogoResultMsg) + ")");
						m_SportsLog.Close();

						iGOGOFormatted++;
					} catch(Exception ex) {
						m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
						m_SportsLog.SetFileName(0,LOGFILESUFFIX);
						m_SportsLog.Open();
						m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " HorseLivePlace.cs.SendPlaceRemoting(): Append GOGO message to queue error:");
						m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " " + ex.ToString());
						m_SportsLog.Close();
						GOGOFormatted = false;
					}
				}
				m_SportsOleReader.Close();
				SptAppCfgDBMgr.Close();
				if(GOGOFormatted) {
					arrResult[0] = iGOGOFormatted;
				} else {
					arrResult[0] = -1;
				}


				//HKJC Pager
				iMsgID = 0;
				iPriority = 1;
				sPrefix = "";
				sCapcode = "";
				sTone = "";
				SQLString.Remove(0, SQLString.Length);
				SQLString.Append("select IMSG_ID, CPREFIX, CCAPCODE, CTONE, IHDRIDSTART, IPRIORITY from PGMSG_HKJC_CFG where CMSGTYPE='");
				if(sRaceTrack.Equals(arrRaceTrack[2])) {	//Macau
					SQLString.Append(MCLVPL);
				} else {	//Hong Kong
					SQLString.Append(HKLVPL);
				}
				SQLString.Append("'");
				m_SportsOleReader = SptAppCfgDBMgr.ExecuteQuery(SQLString.ToString());
				if(m_SportsOleReader.Read()) {
					iMsgID = m_SportsOleReader.GetInt32(0);
					sPrefix = m_SportsOleReader.GetString(1).Trim();
					sCapcode = m_SportsOleReader.GetString(2).Trim();
					sTone = m_SportsOleReader.GetString(3).Trim();
					sHKJCHeaderID = m_SportsOleReader.GetInt32(4).ToString("D4");
					iPriority = m_SportsOleReader.GetInt32(5);
				}
				m_SportsOleReader.Close();
				SptAppCfgDBMgr.Close();

				//Unboxing return value of lvplFormatter.FormatMessage to original type, i.e. byte[]
				byte[] hkjcLevel3Content = (byte[])lvplFormatter.InsertHeaderWithShiftIn(UPDATE_SIGN + sHKJCHeaderID, level2Content);
				byte[] hkjcResultMsg = (byte[])lvplFormatter.InsertHeaderFromString(sPrefix + sCapcode + sTone, hkjcLevel3Content);

				//Check queue to be inserted
				SQLString.Remove(0, SQLString.Length);
				SQLString.Append("select db.CDBCONN, queue.CSQL, queue.CCHANNEL from MSGREG_LVPL_CFG reg, IPMUXADPT_DBCFG db, QUEUE_SQLCFG queue where reg.CPAGER='HKJC' and reg.IMSG_ID=");
				SQLString.Append(iMsgID.ToString());
				SQLString.Append(" and reg.IENABLED=1 and reg.IDB_ID=db.IDB_ID and reg.IQUEUE_ID=queue.IQUEUE_ID order by reg.IDB_ID, reg.IQUEUE_ID");
				m_SportsOleReader = SptAppCfgDBMgr.ExecuteQuery(SQLString.ToString());
				while(m_SportsOleReader.Read()) {
					if(m_SportsOleReader.IsDBNull(0)) break;
					if(sHKJCDB.Equals("")) sHKJCDB = m_SportsOleReader.GetString(0).Trim();
					try {
						OleDbConnection queueConn = new OleDbConnection(m_SportsOleReader.GetString(0).Trim());
						OleDbCommand queueCmd = new OleDbCommand(m_SportsOleReader.GetString(1).Trim(), queueConn);
						queueCmd.Parameters.Add(new OleDbParameter("PRI", OleDbType.Integer));
						queueCmd.Parameters[0].Value = iPriority;

						queueCmd.Parameters.Add(new OleDbParameter("HDRID", OleDbType.VarChar, 7));
						queueCmd.Parameters[1].Value = sHKJCHeaderID;

						queueCmd.Parameters.Add(new OleDbParameter("VQUERY", OleDbType.VarChar, 1000));
						queueCmd.Parameters[2].Value = "-1";

						queueCmd.Parameters.Add(new OleDbParameter("CAPCODE", OleDbType.VarChar, 10));
						queueCmd.Parameters[3].Value = sCapcode;

						queueCmd.Parameters.Add(new OleDbParameter("BQUERY", OleDbType.VarBinary));
						queueCmd.Parameters[4].Value = hkjcResultMsg;

						queueCmd.Parameters.Add(new OleDbParameter("CPREFIX", OleDbType.VarChar, 5));
						queueCmd.Parameters[5].Value = sPrefix;

						queueConn.Open();
						queueCmd.ExecuteNonQuery();
						queueConn.Close();

						m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
						m_SportsLog.SetFileName(0,LOGFILESUFFIX);
						m_SportsLog.Open();
						m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " HorseLivePlace.cs: [SENT] HKJC - " + m_SportsOleReader.GetString(2).Trim() + " (" + m_Big5Encoded.GetString(hkjcResultMsg) + ")");
						m_SportsLog.Close();

						iHKJCFormatted++;
					} catch(Exception ex) {
						m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
						m_SportsLog.SetFileName(0,LOGFILESUFFIX);
						m_SportsLog.Open();
						m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " HorseLivePlace.cs.SendPlaceRemoting(): Append HKJC message to queue error:");
						m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " " + ex.ToString());
						m_SportsLog.Close();
						HKJCFormatted = false;
					}
				}
				m_SportsOleReader.Close();
				SptAppCfgDBMgr.Close();
				if(HKJCFormatted) {
					arrResult[1] = iHKJCFormatted;
				} else {
					arrResult[1] = -1;
				}

				//HORSE Pager
				iMsgID = 0;
				iPriority = 1;
				sPrefix = "";
				sCapcode = "";
				sTone = "";
				SQLString.Remove(0, SQLString.Length);
				SQLString.Append("select IMSG_ID, CPREFIX, CCAPCODE, CTONE, IHDRIDSTART, IPRIORITY from PGMSG_HORSE_CFG where CMSGTYPE='");
				if(sRaceTrack.Equals(arrRaceTrack[2])) {	//Macau
					SQLString.Append(MCLVPL);
				} else {	//Hong Kong
					SQLString.Append(HKLVPL);
				}
				SQLString.Append("'");
				m_SportsOleReader = SptAppCfgDBMgr.ExecuteQuery(SQLString.ToString());
				if(m_SportsOleReader.Read()) {
					iMsgID = m_SportsOleReader.GetInt32(0);
					sPrefix = m_SportsOleReader.GetString(1).Trim();
					sCapcode = m_SportsOleReader.GetString(2).Trim();
					sTone = m_SportsOleReader.GetString(3).Trim();
					sHorseHeaderID = m_SportsOleReader.GetInt32(4).ToString("D4");
					iPriority = m_SportsOleReader.GetInt32(5);
				}
				m_SportsOleReader.Close();
				SptAppCfgDBMgr.Close();

				//Unboxing return value of lvplFormatter.FormatMessage to original type, i.e. byte[]
				byte[] horseLevel3Content = (byte[])lvplFormatter.InsertHeaderWithShiftIn(UPDATE_SIGN + sHorseHeaderID, level2Content);
				byte[] horseResultMsg = (byte[])lvplFormatter.InsertHeaderFromString(sPrefix + sCapcode + sTone, horseLevel3Content);

				//Check queue to be inserted
				SQLString.Remove(0, SQLString.Length);
				SQLString.Append("select db.CDBCONN, queue.CSQL, queue.CCHANNEL from MSGREG_LVPL_CFG reg, IPMUXADPT_DBCFG db, QUEUE_SQLCFG queue where reg.CPAGER='HORSE' and reg.IMSG_ID=");
				SQLString.Append(iMsgID.ToString());
				SQLString.Append(" and reg.IENABLED=1 and reg.IDB_ID=db.IDB_ID and reg.IQUEUE_ID=queue.IQUEUE_ID order by reg.IDB_ID, reg.IQUEUE_ID");
				m_SportsOleReader = SptAppCfgDBMgr.ExecuteQuery(SQLString.ToString());
				while(m_SportsOleReader.Read()) {
					if(m_SportsOleReader.IsDBNull(0)) break;
					try {
						OleDbConnection queueConn = new OleDbConnection(m_SportsOleReader.GetString(0).Trim());
						OleDbCommand queueCmd = new OleDbCommand(m_SportsOleReader.GetString(1).Trim(), queueConn);
						queueCmd.Parameters.Add(new OleDbParameter("PRI", OleDbType.Integer));
						queueCmd.Parameters[0].Value = iPriority;

						queueCmd.Parameters.Add(new OleDbParameter("HDRID", OleDbType.VarChar, 7));
						queueCmd.Parameters[1].Value = sHorseHeaderID;

						queueCmd.Parameters.Add(new OleDbParameter("VQUERY", OleDbType.VarChar, 1000));
						queueCmd.Parameters[2].Value = "-1";

						queueCmd.Parameters.Add(new OleDbParameter("CAPCODE", OleDbType.VarChar, 10));
						queueCmd.Parameters[3].Value = sCapcode;

						queueCmd.Parameters.Add(new OleDbParameter("BQUERY", OleDbType.VarBinary));
						queueCmd.Parameters[4].Value = horseResultMsg;

						queueCmd.Parameters.Add(new OleDbParameter("CPREFIX", OleDbType.VarChar, 5));
						queueCmd.Parameters[5].Value = sPrefix;

						queueConn.Open();
						queueCmd.ExecuteNonQuery();
						queueConn.Close();

						m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
						m_SportsLog.SetFileName(0,LOGFILESUFFIX);
						m_SportsLog.Open();
						m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " HorseLivePlace.cs: [SENT] HORSE - " + m_SportsOleReader.GetString(2).Trim() + " (" + m_Big5Encoded.GetString(horseResultMsg) + ")");
						m_SportsLog.Close();

						iHORSEFormatted++;
					} catch(Exception ex) {
						m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
						m_SportsLog.SetFileName(0,LOGFILESUFFIX);
						m_SportsLog.Open();
						m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " HorseLivePlace.cs.SendPlaceRemoting(): Append HORSE message to queue error:");
						m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " " + ex.ToString());
						m_SportsLog.Close();
						HORSEFormatted = false;
					}
				}
				m_SportsOleReader.Close();
				SptAppCfgDBMgr.Close();
				if(HORSEFormatted) {
					arrResult[2] = iHORSEFormatted;
				} else {
					arrResult[2] = -1;
				}

				//Combo Pager
				/*
				iMsgID = 0;
				iPriority = 1;
				sPrefix = "";
				sCapcode = "";
				sTone = "";

				SQLString.Remove(0, SQLString.Length);
				SQLString.Append("select IMSG_ID, CPREFIX, CCAPCODE, CTONE, IHDRIDSTART, IPRIORITY from PGMSG_COMBO_CFG where CMSGTYPE='");
				if(sRaceTrack.Equals(arrRaceTrack[2])) {	//Macau
					SQLString.Append(MCLVPL);
				} else {	//Hong Kong
					SQLString.Append(HKLVPL);
				}
				SQLString.Append("'");
				m_SportsOleReader = SptAppCfgDBMgr.ExecuteQuery(SQLString.ToString());
				if(m_SportsOleReader.Read()) {
					iMsgID = m_SportsOleReader.GetInt32(0);
					sPrefix = m_SportsOleReader.GetString(1).Trim();
					sCapcode = m_SportsOleReader.GetString(2).Trim();
					sTone = m_SportsOleReader.GetString(3).Trim();
					sComboHeaderID = m_SportsOleReader.GetInt32(4).ToString("D4");
					iPriority = m_SportsOleReader.GetInt32(5);
				}
				m_SportsOleReader.Close();
				SptAppCfgDBMgr.Close();

				//Unboxing return value of lvplFormatter.FormatMessage to original type, i.e. byte[]
				byte[] combolevel1Content = (byte[])cb_encodedFinList.ToArray(typeof(byte));
				byte[] comboLevel3Content = (byte[])lvplFormatter.InsertHeaderWithShiftIn(UPDATE_SIGN + sComboHeaderID, combolevel1Content);
				byte[] comboResultMsg = (byte[])lvplFormatter.InsertHeaderFromString(sPrefix + sCapcode + sTone, comboLevel3Content);

				//Check queue to be inserted
				//combo pager uses hk_queue of gogo1 to send message
				SQLString.Remove(0, SQLString.Length);
				SQLString.Append("select db.CDBCONN, queue.CSQL, queue.CCHANNEL from MSGREG_LVPL_CFG reg, IPMUXADPT_DBCFG db, QUEUE_SQLCFG queue where reg.CPAGER='COMBO' and reg.IMSG_ID=");
				SQLString.Append(iMsgID.ToString());
				SQLString.Append(" and reg.IENABLED=1 and reg.IDB_ID=db.IDB_ID and reg.IQUEUE_ID=queue.IQUEUE_ID order by reg.IDB_ID, reg.IQUEUE_ID");
				m_SportsOleReader = SptAppCfgDBMgr.ExecuteQuery(SQLString.ToString());
				while(m_SportsOleReader.Read()) {
					if(m_SportsOleReader.IsDBNull(0)) break;
					try {
						OleDbConnection queueConn = new OleDbConnection(m_SportsOleReader.GetString(0).Trim());
						OleDbCommand queueCmd = new OleDbCommand(m_SportsOleReader.GetString(1).Trim(), queueConn);
						queueCmd.Parameters.Add(new OleDbParameter("PRI", OleDbType.Integer));
						queueCmd.Parameters[0].Value = iPriority;

						queueCmd.Parameters.Add(new OleDbParameter("HDRID", OleDbType.VarChar, 7));
						queueCmd.Parameters[1].Value = sComboHeaderID;

						queueCmd.Parameters.Add(new OleDbParameter("VQUERY", OleDbType.VarChar, 1000));
						queueCmd.Parameters[2].Value = "-1";

						queueCmd.Parameters.Add(new OleDbParameter("CAPCODE", OleDbType.VarChar, 10));
						queueCmd.Parameters[3].Value = sCapcode;

						queueCmd.Parameters.Add(new OleDbParameter("BQUERY", OleDbType.VarBinary));
						queueCmd.Parameters[4].Value = comboResultMsg;

						queueCmd.Parameters.Add(new OleDbParameter("CPREFIX", OleDbType.VarChar, 5));
						queueCmd.Parameters[5].Value = sPrefix;

						queueConn.Open();
						queueCmd.ExecuteNonQuery();
						queueConn.Close();

						m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
						m_SportsLog.SetFileName(0,LOGFILESUFFIX);
						m_SportsLog.Open();
						m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " HorseLivePlace.cs: [SENT] COMBO - " + m_SportsOleReader.GetString(2).Trim() + " (" + m_Big5Encoded.GetString(comboResultMsg) + ")");
						m_SportsLog.Close();

						iCOMBOFormatted++;
					} catch(Exception ex) {
						m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
						m_SportsLog.SetFileName(0,LOGFILESUFFIX);
						m_SportsLog.Open();
						m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " HorseLivePlace.cs.SendPlaceRemoting(): Append COMBO message to queue error:");
						m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " " + ex.ToString());
						m_SportsLog.Close();
						COMBOFormatted = false;
					}
				}
				m_SportsOleReader.Close();
				SptAppCfgDBMgr.Close();
				if(COMBOFormatted) {
					arrResult[6] = iCOMBOFormatted;
				} else {
					arrResult[6] = -1;
				}

				//Combo pager (alert string)
				//case: content format is success and alert flag is set
				if (COMBOFormatted && sRaceAlert.Equals("1")){

					iMsgID = 0;
					iPriority = 1;
					sPrefix = "";
					sCapcode = "";
					sTone = "";

					SQLString.Remove(0, SQLString.Length);
					SQLString.Append("select IMSG_ID, CPREFIX, CCAPCODE, CTONE, IHDRIDSTART, IPRIORITY from PGMSG_COMBO_CFG where CMSGTYPE='");
					if(sRaceTrack.Equals(arrRaceTrack[2])) {	//Macau
						SQLString.Append(MCLVPL+"Alert");
					} else {	//Hong Kong
						SQLString.Append(HKLVPL+"Alert");
					}
					SQLString.Append("'");
					m_SportsOleReader = SptAppCfgDBMgr.ExecuteQuery(SQLString.ToString());
					if(m_SportsOleReader.Read()) {
						iMsgID = m_SportsOleReader.GetInt32(0);
						sPrefix = m_SportsOleReader.GetString(1).Trim();
						sCapcode = m_SportsOleReader.GetString(2).Trim();
						sTone = m_SportsOleReader.GetString(3).Trim();
						sComboAlertHeaderID = m_SportsOleReader.GetInt32(4).ToString("D4");
						iPriority = m_SportsOleReader.GetInt32(5);
					}
					m_SportsOleReader.Close();
					SptAppCfgDBMgr.Close();

					//Unboxing return value of lvplFormatter.FormatMessage to original type, i.e. byte[]
					comboAlertResultMsg.AddRange((byte[])m_Big5Encoded.GetBytes(sPrefix + sCapcode + sTone));
					//load combo alert hex string
					comboAlertResultMsg.AddRange((byte[])HttpContext.Current.Application["ComboAlertHexString"]);

					//Check queue to be inserted
					//combo pager uses hk_queue of gogo1 to send message
					SQLString.Remove(0, SQLString.Length);
					SQLString.Append("select db.CDBCONN, queue.CSQL, queue.CCHANNEL from MSGREG_LVPL_CFG reg, IPMUXADPT_DBCFG db, QUEUE_SQLCFG queue where reg.CPAGER='COMBO' and reg.IMSG_ID=");
					SQLString.Append(iMsgID.ToString());
					SQLString.Append(" and reg.IENABLED=1 and reg.IDB_ID=db.IDB_ID and reg.IQUEUE_ID=queue.IQUEUE_ID order by reg.IDB_ID, reg.IQUEUE_ID");
					m_SportsOleReader = SptAppCfgDBMgr.ExecuteQuery(SQLString.ToString());
					while(m_SportsOleReader.Read()) {
						if(m_SportsOleReader.IsDBNull(0)) break;
						try {
							OleDbConnection queueConn = new OleDbConnection(m_SportsOleReader.GetString(0).Trim());
							OleDbCommand queueCmd = new OleDbCommand(m_SportsOleReader.GetString(1).Trim(), queueConn);
							queueCmd.Parameters.Add(new OleDbParameter("PRI", OleDbType.Integer));
							queueCmd.Parameters[0].Value = iPriority;

							queueCmd.Parameters.Add(new OleDbParameter("HDRID", OleDbType.VarChar, 7));
							queueCmd.Parameters[1].Value = sComboAlertHeaderID;

							queueCmd.Parameters.Add(new OleDbParameter("VQUERY", OleDbType.VarChar, 1000));
							queueCmd.Parameters[2].Value = "-1";

							queueCmd.Parameters.Add(new OleDbParameter("CAPCODE", OleDbType.VarChar, 10));
							queueCmd.Parameters[3].Value = sCapcode;

							queueCmd.Parameters.Add(new OleDbParameter("BQUERY", OleDbType.VarBinary));
							queueCmd.Parameters[4].Value = (byte[])comboAlertResultMsg.ToArray(typeof(byte));

							queueCmd.Parameters.Add(new OleDbParameter("CPREFIX", OleDbType.VarChar, 5));
							queueCmd.Parameters[5].Value = sPrefix;

							queueConn.Open();
							queueCmd.ExecuteNonQuery();
							queueConn.Close();

							m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
							m_SportsLog.SetFileName(0,LOGFILESUFFIX);
							m_SportsLog.Open();
							m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " HorseLivePlace.cs: [SENT] COMBO ALERT - " + m_SportsOleReader.GetString(2).Trim() + " (" + m_Big5Encoded.GetString((byte[])comboAlertResultMsg.ToArray(typeof(byte))) + ")");
							m_SportsLog.Close();

							iCOMBOAlertFormatted++;
						} catch(Exception ex) {
							m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
							m_SportsLog.SetFileName(0,LOGFILESUFFIX);
							m_SportsLog.Open();
							m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " HorseLivePlace.cs.SendPlaceRemoting(): Append COMBO Alert message to queue error:");
							m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " " + ex.ToString());
							m_SportsLog.Close();
							COMBOAlertFormatted = false;
						}
					}
					m_SportsOleReader.Close();
					SptAppCfgDBMgr.Close();
					if(COMBOAlertFormatted) {
						arrResult[7] = iCOMBOAlertFormatted;
					} else {
						arrResult[7] = -1;
					}
				}
				*/

				//Notfiy IPMux Adapter for GOGO
				int iRtn = 0;
				//if(GOGOFormatted || HORSEFormatted || COMBOFormatted || COMBOAlertFormatted) {
				if(GOGOFormatted || HORSEFormatted) {
					try {
						Win32Message gogomsg = (Win32Message)Activator.GetObject(typeof(RemoteService.Win32.Win32Message), HttpContext.Current.Application["Broadcast2GOGOObjURI"].ToString());
						iRtn = gogomsg.Broadcast(HttpContext.Current.Application["IPMUXAdptBCStr"].ToString());
						if(iRtn == SUCCESS_CODE) {
							m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
							m_SportsLog.SetFileName(0,LOGFILESUFFIX);
							m_SportsLog.Open();
							m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " HorseLivePlace.cs: [POST] GOGO - Notfiy IPMux Adapter success (" + HttpContext.Current.Session["user_name"] + ")");
							m_SportsLog.Close();
							arrResult[3] = SUCCESS_CODE;
						} else {
							m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
							m_SportsLog.SetFileName(0,LOGFILESUFFIX);
							m_SportsLog.Open();
							m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " HorseLivePlace.cs: [POST] GOGO - Notfiy IPMux Adapter error, code: " + iRtn.ToString());
							m_SportsLog.Close();
							arrResult[3] = -1;
						}
					} catch(Exception ex) {
						m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
						m_SportsLog.SetFileName(0,LOGFILESUFFIX);
						m_SportsLog.Open();
						m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " HorseLivePlace.cs.SendPlaceRemoting(): GOGO - Notfiy IPMux Adapter throws exception: " + ex.ToString());
						m_SportsLog.Close();
						arrResult[3] = -2;
					}
				} else {
					arrResult[3] = -3;
				}

				//Notfiy IPMux Adapter for HKJC
				iRtn = 0;
				if(HKJCFormatted) {
					try {
						Win32Message jcmsg = (Win32Message)Activator.GetObject(typeof(RemoteService.Win32.Win32Message), HttpContext.Current.Application["Broadcast2HKJCObjURI"].ToString());
						iRtn = jcmsg.Broadcast(HttpContext.Current.Application["IPMUXAdptBCStr"].ToString());
						if(iRtn == SUCCESS_CODE) {
							m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
							m_SportsLog.SetFileName(0,LOGFILESUFFIX);
							m_SportsLog.Open();
							m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " HorseLivePlace.cs: [POST] HKJC - Notfiy IPMux Adapter success (" + HttpContext.Current.Session["user_name"] + ")");
							m_SportsLog.Close();
							arrResult[4] = SUCCESS_CODE;
						} else {
							m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
							m_SportsLog.SetFileName(0,LOGFILESUFFIX);
							m_SportsLog.Open();
							m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " HorseLivePlace.cs: [POST] HKCJ - Notfiy IPMux Adapter error, code: " + iRtn.ToString());
							m_SportsLog.Close();
							arrResult[4] = -1;
						}
					} catch(Exception ex) {
						m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
						m_SportsLog.SetFileName(0,LOGFILESUFFIX);
						m_SportsLog.Open();
						m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " HorseLivePlace.cs.SendPlaceRemoting(): HKJC - Notfiy IPMux Adapter throws exception: " + ex.ToString());
						m_SportsLog.Close();
						arrResult[4] = -2;
					}
				} else {
					arrResult[4] = -3;
				}


				//update RACEINFO, RACEPLACEINFO
				SQLString.Remove(0,SQLString.Length);
				SQLString.Append("update RACEINFO set CRACESTATUS='");
				SQLString.Append(sRaceStatus);
				SQLString.Append("' where IRACE_NO=");
				SQLString.Append(sRaceID);
				m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
				m_SportsDBMgr.Close();

				SQLString.Remove(0,SQLString.Length);
				SQLString.Append("insert into RACEPLACEINFO values (");
				SQLString.Append(arrRacePlaceNo[0]);
				SQLString.Append(",");
				SQLString.Append(sRaceID);
				SQLString.Append(",'");
				SQLString.Append(sRaceFirst);
				SQLString.Append("','");
				SQLString.Append(sRaceSecond);
				SQLString.Append("','");
				SQLString.Append(sRaceThird);
				SQLString.Append("','");
				SQLString.Append(sRaceFourth);
				SQLString.Append("','0','");
				SQLString.Append(sRaceStatus);
				SQLString.Append("')");
				m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
				m_SportsDBMgr.Close();

				//PLACE_DETAILS, RACEPLACE_DETAILS for GOGODB
				if(!sGOGODB.Equals("")) {
					DBManager GOGODBMgr = new DBManager();
					GOGODBMgr.ConnectionString = sGOGODB;
					SQLString.Remove(0,SQLString.Length);
					SQLString.Append("delete from PLACE_DETAILS");
					GOGODBMgr.ExecuteNonQuery(SQLString.ToString());
					GOGODBMgr.Close();
					SQLString.Remove(0,SQLString.Length);
					SQLString.Append("insert into PLACE_DETAILS values (");
					SQLString.Append(sRaceID);
					SQLString.Append(",0,");
					if(!sRaceFirst.Trim().Equals("")) SQLString.Append(sRaceFirst);
					else SQLString.Append("-1");
					SQLString.Append(",");
					if(!sRaceSecond.Trim().Equals("")) SQLString.Append(sRaceSecond);
					else SQLString.Append("-1");
					SQLString.Append(",");
					if(!sRaceThird.Trim().Equals("")) SQLString.Append(sRaceThird);
					else SQLString.Append("-1");
					SQLString.Append(",");
					if(!sRaceFourth.Trim().Equals("")) SQLString.Append(sRaceFourth);
					else SQLString.Append("-1");
					SQLString.Append(",'");
					SQLString.Append(sRaceStatus);
					SQLString.Append("',1)");
					GOGODBMgr.ExecuteNonQuery(SQLString.ToString());
					GOGODBMgr.Close();

					SQLString.Remove(0,SQLString.Length);
					SQLString.Append("delete from RACEPLACE_DETAILS");
					GOGODBMgr.ExecuteNonQuery(SQLString.ToString());
					GOGODBMgr.Close();
					SQLString.Remove(0,SQLString.Length);
					SQLString.Append("insert into RACEPLACE_DETAILS values (");
					SQLString.Append(sGOGOHeaderID);
					SQLString.Append(",");
					SQLString.Append(sRaceDate);
					SQLString.Append(",");
					SQLString.Append(sRaceTime);
					SQLString.Append(",'");
					SQLString.Append(sRaceTrack);
					SQLString.Append("',");
					SQLString.Append(sRaceID);
					SQLString.Append(",'");
					SQLString.Append(sRaceClass);
					SQLString.Append("','");
					SQLString.Append(sRaceLength);
					SQLString.Append("米',");
					SQLString.Append(sRaceAlert);
					SQLString.Append(",1,null,'");
					SQLString.Append(DateTime.Now.ToString("yyyyMMddHHmmss"));
					SQLString.Append("',1)");
					GOGODBMgr.ExecuteNonQuery(SQLString.ToString());
					GOGODBMgr.Close();
				}

				//PLACE_DETAILS, RACEPLACE_DETAILS for HKJCDB
				if(!sHKJCDB.Equals("")) {
					DBManager HKJCDBMgr = new DBManager();
					HKJCDBMgr.ConnectionString = sHKJCDB;
					SQLString.Remove(0,SQLString.Length);
					SQLString.Append("delete from PLACE_DETAILS");
					HKJCDBMgr.ExecuteNonQuery(SQLString.ToString());
					HKJCDBMgr.Close();
					SQLString.Remove(0,SQLString.Length);
					SQLString.Append("insert into PLACE_DETAILS values (");
					SQLString.Append(sRaceID);
					SQLString.Append(",0,");
					if(!sRaceFirst.Trim().Equals("")) SQLString.Append(sRaceFirst);
					else SQLString.Append("-1");
					SQLString.Append(",");
					if(!sRaceSecond.Trim().Equals("")) SQLString.Append(sRaceSecond);
					else SQLString.Append("-1");
					SQLString.Append(",");
					if(!sRaceThird.Trim().Equals("")) SQLString.Append(sRaceThird);
					else SQLString.Append("-1");
					SQLString.Append(",");
					if(!sRaceFourth.Trim().Equals("")) SQLString.Append(sRaceFourth);
					else SQLString.Append("-1");
					SQLString.Append(",'");
					SQLString.Append(sRaceStatus);
					SQLString.Append("',1)");
					HKJCDBMgr.ExecuteNonQuery(SQLString.ToString());
					HKJCDBMgr.Close();

					SQLString.Remove(0,SQLString.Length);
					SQLString.Append("delete from RACEPLACE_DETAILS");
					HKJCDBMgr.ExecuteNonQuery(SQLString.ToString());
					HKJCDBMgr.Close();
					SQLString.Remove(0,SQLString.Length);
					SQLString.Append("insert into RACEPLACE_DETAILS values (");
					SQLString.Append(sHKJCHeaderID);
					SQLString.Append(",");
					SQLString.Append(sRaceDate);
					SQLString.Append(",");
					SQLString.Append(sRaceTime);
					SQLString.Append(",'");
					SQLString.Append(sRaceTrack);
					SQLString.Append("',");
					SQLString.Append(sRaceID);
					SQLString.Append(",'");
					SQLString.Append(sRaceClass);
					SQLString.Append("','");
					SQLString.Append(sRaceLength);
					SQLString.Append("米',");
					SQLString.Append(sRaceAlert);
					SQLString.Append(",1,null,'");
					SQLString.Append(DateTime.Now.ToString("yyyyMMddHHmmss"));
					SQLString.Append("',1)");
					HKJCDBMgr.ExecuteNonQuery(SQLString.ToString());
					HKJCDBMgr.Close();
				}
				arrResult[5] = 1;
			} catch(Exception ex) {
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " HorseLivePlace.cs.SendPlaceRemoting(): " + ex.ToString());
				m_SportsLog.Close();
				arrResult[5] = -1;
			}

			return arrResult;
		}

		//Send message to MSMQ and then notify Sender to process received INI file (only for combo device)
		public int SendCBPlace() {

			//terminate if it is Macau type
			string sRaceTrack;
			sRaceTrack = HttpContext.Current.Request.Form["RaceTrack"];
			if(sRaceTrack.Equals(arrRaceTrack[2])) {
				return 1;
			}

			int iRecordSent = 0;
			int iArrayIndex = 0;
			int iHistoryCount = 0;
			string sRaceID;
			string sRaceFirst;
			string sRaceSecond;
			string sRaceThird;
			string sRaceFourth;
			string sRaceStatus;
			string sRaceAlert;
			string sCurrentTimestamp = null;
			string sBatchJob = null;
			string[] arrSendToPager;
			string[] arrRacePlaceNo;
			string[] arrRaceHistoryFirst;
			string[] arrRaceHistorySecond;
			string[] arrRaceHistoryThird;
			string[] arrRaceHistoryFourth;
			string[] arrRaceHistoryStatus;
			string[] arrMsgType;

			//Delcare variable used in message notify
			string[] arrQueueNames;
			string[] arrRemotingPath;
			string[] arrMessageTypes;
			arrQueueNames = (string[])HttpContext.Current.Application["QueueItems"];
			arrRemotingPath = (string[])HttpContext.Current.Application["RemotingItems"];
			arrMessageTypes = (string[])HttpContext.Current.Application["NotifyMessageTypes"];
			SportsMessage sptMsg = new SportsMessage();
			MessageClient msgClt = new MessageClient();

			try {
				arrSendToPager = HttpContext.Current.Request.Form["SendToPager"].Split(delimiter);
			} catch(Exception) {
				arrSendToPager = new string[0];
			}

			sRaceID = HttpContext.Current.Request.Form["RaceID"];
			sRaceFirst = HttpContext.Current.Request.Form["First"];
			if(sRaceFirst.Equals("")) sRaceFirst = "0";
			sRaceSecond = HttpContext.Current.Request.Form["Second"];
			if(sRaceSecond.Equals("")) sRaceSecond = "0";
			sRaceThird = HttpContext.Current.Request.Form["Third"];
			if(sRaceThird.Equals("")) sRaceThird = "0";
			sRaceFourth = HttpContext.Current.Request.Form["Fourth"];
			if(sRaceFourth.Equals("")) sRaceFourth = "0";
			sRaceStatus = HttpContext.Current.Request.Form["RaceStatus"];
			sRaceAlert = HttpContext.Current.Request.Form["PlaceAlert"];
			if(sRaceAlert == null) sRaceAlert = "0";

			arrRacePlaceNo = HttpContext.Current.Request.Form["NextPlaceNo"].Split(delimiter);
			arrRaceHistoryFirst = HttpContext.Current.Request.Form["HistoryFirst"].Split(delimiter);
			arrRaceHistorySecond = HttpContext.Current.Request.Form["HistorySecond"].Split(delimiter);
			arrRaceHistoryThird = HttpContext.Current.Request.Form["HistoryThird"].Split(delimiter);
			arrRaceHistoryFourth = HttpContext.Current.Request.Form["HistoryFourth"].Split(delimiter);
			arrRaceHistoryStatus = HttpContext.Current.Request.Form["HistoryStatus"].Split(delimiter);
			iHistoryCount = arrRacePlaceNo.Length;

			try {
				iRecordSent++;
				arrMsgType = (string[])HttpContext.Current.Application["messageType"];
				sCurrentTimestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
				sBatchJob = DateTime.Now.ToString("yyMMddHHmmss_fffffff") + "." + HttpContext.Current.Session["user_name"].ToString() + "." + arrMsgType[41] + ".ini";

				//Insert log into LOG_LIVEPLACE
				SQLString.Remove(0,SQLString.Length);
				SQLString.Append("insert into LOG_LIVEPLACE (TIMEFLAG, IITEMSEQ_NO, SECTION, RACEDATE, RACETIME, RACETRACK, RACENO, RACECLASS, RACELENGTH, RACEALERT, RACE_STATUS, RACE_1, RACE_2, RACE_3, RACE_4, BATCHJOB) values ('");
				SQLString.Append(sCurrentTimestamp);
				SQLString.Append("',1,'LIVEPLACE','");
				SQLString.Append(HttpContext.Current.Request.Form["RaceDate"]);
				SQLString.Append("','");
				SQLString.Append(HttpContext.Current.Request.Form["RaceTime"]);
				SQLString.Append("','");
				SQLString.Append(HttpContext.Current.Request.Form["RaceTrack"]);
				SQLString.Append("',");
				SQLString.Append(sRaceID);
				SQLString.Append(",'");
				SQLString.Append(HttpContext.Current.Request.Form["RaceClass"]);
				SQLString.Append("','");
				SQLString.Append(HttpContext.Current.Request.Form["RaceLength"]);
				SQLString.Append("米',");
				SQLString.Append(sRaceAlert);
				SQLString.Append(",'");
				SQLString.Append(sRaceStatus);
				SQLString.Append("',");
				SQLString.Append(sRaceFirst);
				SQLString.Append(",");
				SQLString.Append(sRaceSecond);
				SQLString.Append(",");
				SQLString.Append(sRaceThird);
				SQLString.Append(",");
				SQLString.Append(sRaceFourth);
				SQLString.Append(",'");
				SQLString.Append(sBatchJob);
				SQLString.Append("')");
				m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
				m_SportsDBMgr.Close();

				if(Convert.ToInt32(arrRacePlaceNo[0]) > 1) {	//History records existed
					for(iArrayIndex = 0; iArrayIndex < iHistoryCount; iArrayIndex++) {
						SQLString.Remove(0,SQLString.Length);
						SQLString.Append("insert into LOG_LIVEPLACE (TIMEFLAG, IITEMSEQ_NO, SECTION, RACEDATE, RACETIME, RACETRACK, RACENO, RACECLASS, RACELENGTH, RACEALERT, RACE_STATUS, RACE_1, RACE_2, RACE_3, RACE_4, BATCHJOB) values ('");
						SQLString.Append(sCurrentTimestamp);
						SQLString.Append("',");
						SQLString.Append((iArrayIndex+2).ToString());
						SQLString.Append(",'LIVEPLACE','");
						SQLString.Append(HttpContext.Current.Request.Form["RaceDate"]);
						SQLString.Append("','");
						SQLString.Append(HttpContext.Current.Request.Form["RaceTime"]);
						SQLString.Append("','");
						SQLString.Append(HttpContext.Current.Request.Form["RaceTrack"]);
						SQLString.Append("',");
						SQLString.Append(sRaceID);
						SQLString.Append(",'");
						SQLString.Append(HttpContext.Current.Request.Form["RaceClass"]);
						SQLString.Append("','");
						SQLString.Append(HttpContext.Current.Request.Form["RaceLength"]);
						SQLString.Append("米',");
						SQLString.Append(sRaceAlert);
						SQLString.Append(",'");
						SQLString.Append(arrRaceHistoryStatus[iArrayIndex]);
						SQLString.Append("',");
						SQLString.Append(arrRaceHistoryFirst[iArrayIndex]);
						SQLString.Append(",");
						SQLString.Append(arrRaceHistorySecond[iArrayIndex]);
						SQLString.Append(",");
						SQLString.Append(arrRaceHistoryThird[iArrayIndex]);
						SQLString.Append(",");
						SQLString.Append(arrRaceHistoryFourth[iArrayIndex]);
						SQLString.Append(",'");
						SQLString.Append(sBatchJob);
						SQLString.Append("')");
						m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
						m_SportsDBMgr.Close();
					}
				}

				//Tell MessageDispatcher to generate CBLivePlace INI and notify other processes
				//Assign value to SportsMessage object
				sptMsg.Body = sBatchJob;
				sptMsg.Timestamp = DateTime.Now.ToString("yyMMddHHmmss");
				sptMsg.AppID = "07";
				sptMsg.MsgID = "36";
				sptMsg.DeviceID = new string[0];
				//combo device only
				sptMsg.AddDeviceID("4");
				//for(int i = 0; i < arrSendToPager.Length; i++) {
				//	sptMsg.AddDeviceID((string)arrSendToPager[i]);
				//}
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
						m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " MSMQ ERROR: Horse Racing CB Live Place");
						m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " HorseLivePlace.cs.SendCBPlace(): Notify via MSMQ throws MessageQueueException: " + mqEx.ToString());
						m_SportsLog.Close();

						//If MSMQ fail, notify via .NET Remoting
						msgClt.MessageType = arrMessageTypes[1];
						msgClt.MessagePath = arrRemotingPath[0];
						if(!msgClt.SendMessage((object)sptMsg)) {
							m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
							m_SportsLog.SetFileName(0,LOGFILESUFFIX);
							m_SportsLog.Open();
							m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Remoting ERROR: CB Horse Racing Live Place");
							m_SportsLog.Close();
						}
					}	catch(Exception ex) {
						m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
						m_SportsLog.SetFileName(0,LOGFILESUFFIX);
						m_SportsLog.Open();
						m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Remoting ERROR: Horse Racing CB Live Place");
						m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " HorseLivePlace.cs.SendCBPlace(): Notify via .NET Remoting throws exception: " + ex.ToString());
						m_SportsLog.Close();
					}
				}	catch(Exception ex) {
					m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
					m_SportsLog.SetFileName(0,LOGFILESUFFIX);
					m_SportsLog.Open();
					m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " HorseLivePlace.cs.SendCBPlace(): Notify via MSMQ throws exception: " + ex.ToString());
					m_SportsLog.Close();
				}

				m_SportsDBMgr.Dispose();
			} catch(Exception ex) {
				iRecordSent = -1;
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " HorseLivePlace.cs.SendCBPlace(): " + ex.ToString());
				m_SportsLog.Close();
			}

			return iRecordSent;
		}

		private string PadRightSpace(string sItem, int iSpaceCount) {
			int iLen;
			byte[] arrItemByte;
			StringBuilder sbRefined;
			if(sItem != null) {
				sbRefined = new StringBuilder(sItem);
				arrItemByte = m_Big5Encoded.GetBytes(sItem);
				iLen = arrItemByte.Length;
				sbRefined.Append(' ',iSpaceCount-iLen);
			} else {
				sbRefined = new StringBuilder(new String(' ',iSpaceCount));
			}

			return sbRefined.ToString();
		}

	}
}