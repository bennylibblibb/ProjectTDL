/*
Objective:
Save horse racing info such as race time, venue, length, track etc.

Last updated:
19 Feb 2004, Chapman Choi

C#.NET complier statement:
csc /t:library /out:..\bin\HorseRacingInfo.dll /r:..\bin\DBManager.dll;..\bin\Files.dll;..\bin\MessageClient.dll;..\bin\SportsMessage.dll HorseRacingInfo.cs
*/

using System;
using System.Data.OleDb;
using System.Reflection;
using System.Web;
using TDL.DB;
using TDL.IO;
using TDL.Message;

[assembly:AssemblyCompany("TDSL")]
[assembly:AssemblyCopyright("(c) 2003 TDSL. All rights reserved.")]
[assembly:AssemblyDescription("賽馬資訊 -> 新增賽事")]
[assembly:AssemblyProduct("Sports.NET")]
[assembly:AssemblyTitle("Sports.NET DLL")]
[assembly:AssemblyVersion("3.1.*")]
namespace SportsUtil {
	public class HorseRacingInfo {
		const string CMDTYPE = "CLR_TBL";
		const string LOGFILESUFFIX = "log";
		const int m_RecordCount = 15;
		DBManager m_SportsDBMgr;
		OleDbDataReader m_SportsOleReader;
		Files m_SportsLog;

		public HorseRacingInfo(string Connection) {
			m_SportsDBMgr = new DBManager();
			m_SportsDBMgr.ConnectionString = Connection;
			m_SportsLog = new Files();
		}

		public string RetrievalRace() {
			bool bExisted = false;
			int iArrayIndex = 0;
			int iAppItemsIndex = 0;
			int iRecordCount = 0;
			string sRtn;
			string sRaceQuery;
			string sMatchDate = null;
			string sRaceCourse = null;
			string sRaceClass = null;
			string sParam;
			string[] arrRaceTrack;
			string[] arrRaceClass;

			arrRaceTrack = (string[])HttpContext.Current.Application["HorseRaceTracks"];
			arrRaceClass = (string[])HttpContext.Current.Application["HorseRaceClasses"];
			try {
				sRaceQuery = "select CRACEDATE,CRACETRACK from RACEINFO";
				m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(sRaceQuery);
				if(m_SportsOleReader.Read()) {
					sMatchDate = m_SportsOleReader.GetString(0);
					sRaceCourse = m_SportsOleReader.GetString(1).Trim();
					bExisted = true;
				}
				m_SportsDBMgr.Close();
				m_SportsOleReader.Close();
				sRtn = "<tr style=\"background-color:#FFD700\" align=\"center\"><th align=\"right\">賽事日期(yyyyMMdd):</th><td><input type=\"text\" name=\"RaceDate\" maxlength=\"8\" size=\"8\" value=\"";
				if(sMatchDate != null) sRtn += sMatchDate;
				else sRtn += DateTime.Now.ToString("yyyyMMdd");
				sRtn += "\" onChange=\"RaceDateValidity()\"></td><th>賽事地點:</th><td><select name=\"RaceTrack\">";
				if(sRaceCourse != null) {
					sRtn += "<option value=\"" + sRaceCourse + "\">" + sRaceCourse + "</option>";
					for(iArrayIndex = 0; iArrayIndex < arrRaceTrack.Length; iArrayIndex++) {
						if(!sRaceCourse.Equals(arrRaceTrack[iArrayIndex])) {
							sRtn += "<option value=\"" + arrRaceTrack[iArrayIndex] + "\">" + arrRaceTrack[iArrayIndex] + "</option>";
						}
					}
				} else {
					for(iArrayIndex = 0; iArrayIndex < arrRaceTrack.Length; iArrayIndex++) {
						sRtn += "<option value=\"" + arrRaceTrack[iArrayIndex] + "\">" + arrRaceTrack[iArrayIndex] + "</option>";
					}
				}
				sRtn += "</select></td></tr><tr style=\"background-color:#FFD700\"><th align=\"right\">場次</th><th>時間(hhnn)</th><th>班級</th><th>路程(米)</th></tr>";

				if(bExisted) {
					sRaceQuery = "select CRACETIME,CCLASS,CRACELENGTH from RACEINFO";
					m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(sRaceQuery);
					while(m_SportsOleReader.Read()) {
						iRecordCount++;
						sRtn += "<tr align=\"center\"><th align=\"right\">" + iRecordCount.ToString() + "</th><td><input type=\"text\" name=\"RaceTime\" value=\"" + m_SportsOleReader.GetString(0) + "\" maxlength=\"4\" size=\"3\" onChange=\"RaceTimeValidity("+(iRecordCount-1)+")\"></td><td><select name=\"RaceClass\">";
						sRaceClass = m_SportsOleReader.GetString(1).Trim();
						sRtn += "<option value=\"" + sRaceClass + "\">" + sRaceClass + "</option>";
						for(iArrayIndex = 0; iArrayIndex < arrRaceClass.Length; iArrayIndex++) {
							if(!sRaceClass.Equals(arrRaceClass[iArrayIndex])) {
								sRtn += "<option value=\"" + arrRaceClass[iArrayIndex] + "\">" + arrRaceClass[iArrayIndex] + "</option>";
							}
						}
						sRtn += "</select></td><td><input type=\"text\" name=\"RaceLength\" value=\"" + m_SportsOleReader.GetString(2).Trim() + "\" maxlength=\"5\" size=\"4\" onChange=\"RaceLengthValidity("+(iRecordCount-1)+")\"></td>";
					}
					m_SportsDBMgr.Close();
					m_SportsOleReader.Close();
					for(iArrayIndex = iRecordCount; iArrayIndex < m_RecordCount; iArrayIndex++) {
						sRtn += "<tr align=\"center\"><th align=\"right\">" + (iArrayIndex+1).ToString() + "</th><td><input type=\"text\" name=\"RaceTime\" value=\"\" maxlength=\"4\" size=\"3\" onChange=\"RaceTimeValidity("+iArrayIndex+")\"></td><td><select name=\"RaceClass\">";
						for(iAppItemsIndex = 0; iAppItemsIndex < arrRaceClass.Length; iAppItemsIndex++) {
							sRtn += "<option value=\"" + arrRaceClass[iAppItemsIndex] + "\">" + arrRaceClass[iAppItemsIndex] + "</option>";
						}
						sRtn += "</select></td><td><input type=\"text\" name=\"RaceLength\" value=\"\" maxlength=\"5\" size=\"4\" onChange=\"RaceLengthValidity("+iArrayIndex+")\"></td>";
					}
				} else {
					for(iArrayIndex = 0; iArrayIndex < m_RecordCount; iArrayIndex++) {
						sRtn += "<tr align=\"center\"><th align=\"right\">" + (iArrayIndex+1).ToString() + "</th><td><input type=\"text\" name=\"RaceTime\" value=\"\" maxlength=\"4\" size=\"3\" onChange=\"RaceTimeValidity("+iArrayIndex+")\"></td><td><select name=\"RaceClass\">";
						for(iAppItemsIndex = 0; iAppItemsIndex < arrRaceClass.Length; iAppItemsIndex++) {
							sRtn += "<option value=\"" + arrRaceClass[iAppItemsIndex] + "\">" + arrRaceClass[iAppItemsIndex] + "</option>";
						}
						sRtn += "</select></td><td><input type=\"text\" name=\"RaceLength\" value=\"\" maxlength=\"5\" size=\"4\" onChange=\"RaceLengthValidity("+iArrayIndex+")\"></td>";
					}
				}

				sParam = m_SportsDBMgr.ExecuteQueryString("select CPARAM from ADMINCFG where CDISPLAY='現場走位' AND CCMD_TYPE='" + CMDTYPE + "'");
				m_SportsDBMgr.Close();
				sRtn += "<input type=\"hidden\" name=\"delParam\" value=\"" + sParam + "\">";
				m_SportsDBMgr.Dispose();
			} catch(Exception ex) {
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " HorseRacingInfo.cs.RetrievalRace(): " + ex.ToString());
				m_SportsLog.Close();
				sRtn = (string)HttpContext.Current.Application["accessErrorMsg"];
			}

			return sRtn;
		}

		public int SaveRaces() {
			int iRecordSave = 0;
			char[] delimiter = new char[] {','};
			string sRaceQuery;
			string sRaceDate;
			string sRaceTrack;
			string[] arrRaceTime;
			string[] arrRaceClass;
			string[] arrRaceLength;			

			sRaceDate = HttpContext.Current.Request.Form["RaceDate"].Trim();
			sRaceTrack = HttpContext.Current.Request.Form["RaceTrack"].Trim();
			arrRaceTime = HttpContext.Current.Request.Form["RaceTime"].Split(delimiter);
			arrRaceClass = HttpContext.Current.Request.Form["RaceClass"].Split(delimiter);
			arrRaceLength = HttpContext.Current.Request.Form["RaceLength"].Split(delimiter);
			
			try {
				m_SportsDBMgr.ExecuteNonQuery("delete from RACEPLACEINFO");
				m_SportsDBMgr.Close();
				m_SportsDBMgr.ExecuteNonQuery("delete from RACEINFO");
				m_SportsDBMgr.Close();
				for(iRecordSave = 0; iRecordSave < m_RecordCount; iRecordSave++) {
					if((arrRaceTime[iRecordSave].Trim().Equals("")) || (arrRaceLength[iRecordSave].Trim().Equals(""))) {
						break;
					} else {
						sRaceQuery = "insert into RACEINFO values(" + (iRecordSave+1).ToString() + ",'" + sRaceDate + "','" + arrRaceTime[iRecordSave] + "','" + sRaceTrack + "','" + arrRaceClass[iRecordSave] + "','" + arrRaceLength[iRecordSave] + "','')";
						m_SportsDBMgr.ExecuteNonQuery(sRaceQuery);
						m_SportsDBMgr.Close();
					}
				}

				//write log
				m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " HorseRacingInfo.cs: Save " + iRecordSave.ToString() + " matches (" + HttpContext.Current.Session["user_name"] + ")");
				m_SportsLog.Close();
			} catch(Exception ex) {
				iRecordSave = -1;
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " HorseRacingInfo.cs.SaveRaces(): " + ex.ToString());
				m_SportsLog.Close();
			} finally {
				m_SportsDBMgr.Dispose();
			}

			return iRecordSave;
		}

		public int DeleteRaces() {
			int iDeleted = 0;
			string sParam;
			string sCurrentTimestamp = null;
			string sBatchJob = null;
			string[] arrMsgType;
			string[] arrRemotingPath;
			string[] arrSendToPager;
			char[] delimiter = new char[] {','};
			
			sParam = HttpContext.Current.Request.Form["delParam"].Trim();
			arrMsgType = (string[])HttpContext.Current.Application["messageType"];
			arrRemotingPath = (string[])HttpContext.Current.Application["RemotingItems"];

			try {
				arrSendToPager = HttpContext.Current.Request.Form["SendToPager"].Split(delimiter);
			} catch(Exception) {
				arrSendToPager = new string[0];
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
				//SportsMessage object message
				SportsMessage sptMsg = new SportsMessage();

				iDeleted = m_SportsDBMgr.ExecuteNonQuery("delete from RACEPLACEINFO");
				m_SportsDBMgr.Close();
				iDeleted += m_SportsDBMgr.ExecuteNonQuery("delete from RACEINFO");
				m_SportsDBMgr.Close();

				sCurrentTimestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
				sBatchJob = DateTime.Now.ToString("yyMMddHHmmss_fffffff") + "." + HttpContext.Current.Session["user_name"].ToString() + "." + arrMsgType[16] + ".ini";
				m_SportsDBMgr.ExecuteNonQuery("insert into LOG_ADMINTASK (TIMEFLAG, SECTION, CMD, PARAM, BATCHJOB) values ('" + sCurrentTimestamp + "','COMMAND_','" + CMDTYPE + "','" + sParam + "', '" + sBatchJob + "')");
				m_SportsDBMgr.Close();

				//Send Notify Message				
				//Modified by Chapman, 19 Feb 2004
				sptMsg.IsTransaction = false;
				sptMsg.Body = sBatchJob;
				sptMsg.Timestamp = DateTime.Now.ToString("yyMMddHHmmss");
				sptMsg.AppID = "07";
				sptMsg.MsgID = "23";
				sptMsg.PathID = new string[0];
				for(int i = 0; i < arrSendToPager.Length; i++) {
					sptMsg.AddPathID((string)arrSendToPager[i]);
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
						m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " MSMQ ERROR: Horse Racing Info");
						m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " HorseRacingInfo.cs.DeleteRaces(): Notify via MSMQ throws MessageQueueException:  " + mqEx.ToString());
						m_SportsLog.Close();

						//If MSMQ fail, notify via .NET Remoting
						msgClt.MessageType = arrMessageTypes[1];
						msgClt.MessagePath = arrRemotingPath[0];
						if(!msgClt.SendMessage((object)sptMsg)) {
							m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
							m_SportsLog.SetFileName(0,LOGFILESUFFIX);
							m_SportsLog.Open();
							m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Remoting ERROR: Horse Racing Info");
							m_SportsLog.Close();
						}
					}	catch(Exception ex) {
						m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
						m_SportsLog.SetFileName(0,LOGFILESUFFIX);
						m_SportsLog.Open();
						m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Remoting ERROR: Horse Racing Info");
						m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " HorseRacingInfo.cs.DeleteRaces(): Notify via .NET Remoting throws exception: " + ex.ToString());
						m_SportsLog.Close();
					}							
				}	catch(Exception ex) {
					m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
					m_SportsLog.SetFileName(0,LOGFILESUFFIX);
					m_SportsLog.Open();
					m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " HorseRacingInfo.cs.DeleteRaces(): Notify via MSMQ throws exception: " + ex.ToString());
					m_SportsLog.Close();
				}

				//write log
				m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " HorseRacingInfo.cs: Delete all matches including livePlace records (" + HttpContext.Current.Session["user_name"] + ")");
				m_SportsLog.Close();
			} catch(Exception ex) {
				iDeleted = -1;
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " HorseRacingInfo.cs.DeleteRaces(): " + ex.ToString());
				m_SportsLog.Close();
			} finally {
				m_SportsDBMgr.Dispose();
			}
			return iDeleted;
		}
	}
}