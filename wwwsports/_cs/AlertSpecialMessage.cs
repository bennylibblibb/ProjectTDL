/*
Objective:
Send instant alert message who enabled soccer1 message alert in pager

Last updated:
18 August 2005 Chris

C#.NET complier statement:
csc /t:library /out:..\bin\AlertSpecialMessage.dll /r:..\bin\DBManager.dll;..\bin\Files.dll;..\bin\MessageClient.dll;..\bin\SportsMessage.dll;..\bin\MMMFormatter.dll;..\bin\Win32Message.dll AlertSpecialMessage.cs
*/

using System;
using System.Collections;
using System.Text;
using System.Data.OleDb;
using System.Reflection;
using System.Web;
using TDL.DB;
using TDL.IO;
using TDL.Message;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using RemoteService.Win32;
using PagerFormatter;

[assembly:AssemblyCompany("TDSL")]
[assembly:AssemblyCopyright("(c) 2003 TDSL. All rights reserved.")]
[assembly:AssemblyDescription("特定訊息")]
[assembly:AssemblyProduct("Sports.NET")]
[assembly:AssemblyTitle("Sports.NET DLL")]
[assembly:AssemblyVersion("3.1.*")]
namespace SportsUtil {
	public class AlertSpecialMessage {
		const string DBCR = "(CR)";
		const string PAGECR = "\r\n";
		const string LOGFILESUFFIX = "log";
		DBManager m_SportsDBMgr;
		StringBuilder SQLString;
		OleDbDataReader m_SportsOleReader;
		Files m_SportsLog;
		Encoding m_Big5Encoded;
		char[] delimiter;

		public AlertSpecialMessage(string Connection) {
			m_SportsDBMgr = new DBManager();
			SQLString = new StringBuilder();
			m_SportsDBMgr.ConnectionString = Connection;
			m_SportsLog = new Files();
			m_Big5Encoded = Encoding.GetEncoding(950);
			delimiter = new char[] {','};
		}
		
		public string GetMessagePage() {
			String sChiMsgType;
			String sEngMsgType;
			StringBuilder HTMLString = new StringBuilder();
			
			SQLString.Remove(0, SQLString.Length);
			SQLString.Append("select conv.CHI_NAME, conv.ENG_NAME from PGMSG_GOGO1_CFG gogo, PGMSG_HKJC_CFG hkjc, ENG_CHI_CONVERTER conv where gogo.CMSGTYPE = conv.ENG_NAME and hkjc.CMSGTYPE = conv.ENG_NAME and gogo.CMSGTYPE <> 'FreeMessage' and hkjc.CMSGTYPE <> 'FreeMessage'");
			m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
			HTMLString.Append("<input type=radio name=\"alertTypeRadio\" value=\"0\" onclick=\"detect()\">Free Message Header ID: <br>");
			HTMLString.Append("&nbsp&nbsp&nbsp&nbsp<input name=\"HeaderID\" value=\"\" maxlength=\"6\" size=\"6\" disabled><br><br>");
			HTMLString.Append("<input type=radio name=\"alertTypeRadio\" value=\"1\" onclick=\"detect()\" checked>Message Type: <br>");
			HTMLString.Append("&nbsp&nbsp&nbsp&nbsp<select name=\"PagerMessageType\" onChange=\"filter()\"><option value=\"0\">請選擇</option>");
			while(m_SportsOleReader.Read()) {
				HTMLString.Append("<option value=");
				sChiMsgType = m_SportsOleReader.GetString(0).Trim();
				sEngMsgType = m_SportsOleReader.GetString(1).Trim();
				HTMLString.Append(sEngMsgType);
				HTMLString.Append(">");
				HTMLString.Append(sChiMsgType);
				HTMLString.Append("</option>");
			}
			HTMLString.Append("</select>");
			HTMLString.Append("</td><td><textarea name=\"alertMsg\" rows=10 cols=40></textarea>");
			m_SportsOleReader.Close();
			m_SportsDBMgr.Close();
			
			return HTMLString.ToString();
		}
		
		//Format message immediately and send to pager via .NET Remoting
		public int[] SendRemotingMessage() {
			const int SUCCESS_CODE = 100000;
			int iGOGOFormatted = 0;
			int iHKJCFormatted = 0;
			int iJCComboFormatted = 0;			
			string sAlertMsg;
			string sTempMsg;
			string sPagerMessageType;
			string sHeaderID;
			string sBatchJob = null;
			string[] arrSendToPager;
			int[] arrResult = new int[7];
			int i, iIndexOfMsg, iAlertMsgLen, iCountLength;
			
			ArrayList encodedList = new ArrayList();
			ArrayList encodedFinList = new ArrayList();
			ArrayList cb_encodedFinList = new ArrayList();
			StringBuilder PagerScreen = new StringBuilder();	//Used to construct Pager Screen
			MMMFormatter lvplFormatter = new MMMFormatter();	//LivePlace 3M formatter
			sPagerMessageType = "";
			sHeaderID = "";
			sTempMsg = "";
			iIndexOfMsg = 0;
			iCountLength = 0;
			i = 0;

			sAlertMsg = HttpContext.Current.Request.Form["alertMsg"];
			if (HttpContext.Current.Request.Form["alertTypeRadio"] == "1") {
				sPagerMessageType = HttpContext.Current.Request.Form["PagerMessageType"];
				if (sPagerMessageType == "0") {
					arrResult[6] = -2;
					return arrResult;
				} else
					arrResult[6] = 1;
			} else {
				sPagerMessageType = "FreeMessage";
				sHeaderID = HttpContext.Current.Request.Form["HeaderID"];
				if (sHeaderID == "") {
					arrResult[6] = -2;
					return arrResult;
				} else
					arrResult[6] = 1;
			}
			
			try {
				arrSendToPager = HttpContext.Current.Request.Form["SendToPager"].Split(delimiter);
			} catch(Exception) {
				arrSendToPager = new string[0];
			}
			
			if (sAlertMsg != null) {
				sTempMsg = sAlertMsg;
			} else {
				//write log
				m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " AlertSpecialMessage.cs: Message is null, cannot be sent (" + HttpContext.Current.Session["user_name"] + ")");
				m_SportsLog.Close();
			}
			
			try {
				m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " AlertSpecialMessage.cs: [RECV] Constructing pager content (" + HttpContext.Current.Session["user_name"] + ")");
				m_SportsLog.Close();

				//clear buffer
				PagerScreen.Remove(0, PagerScreen.Length);
				if (encodedList.Count>0) encodedList.Clear();
				if (encodedFinList.Count>0) encodedFinList.Clear();
				if (cb_encodedFinList.Count>0) cb_encodedFinList.Clear();

				//Construct pager screen
				//set to fit PagerScreen margin
				//PagerScreen.Append(PadRightSpace(sAlertMsg, lvplFormatter.PagerScreenWidth));
				
				iAlertMsgLen = sAlertMsg.Length;
				
				if (sTempMsg.IndexOf("\r\n") != -1) {
					while (sTempMsg.IndexOf("\r\n") != -1) {
						iIndexOfMsg = sTempMsg.IndexOf("\r\n");
						//clear buffer
						PagerScreen.Remove(0,PagerScreen.Length);
						if (encodedList.Count>0) encodedList.Clear();
						PagerScreen.Append(sTempMsg.Substring(0, iIndexOfMsg));
						encodedList.AddRange((byte[])lvplFormatter.ConvertToCNS(PagerScreen.ToString()));
						encodedFinList.AddRange((byte[])lvplFormatter.PadShift((byte[])encodedList.ToArray(typeof(byte))));
						encodedFinList.Add(lvplFormatter.Linefeed);
	
						iIndexOfMsg += 2;
						iCountLength += iIndexOfMsg;
						iAlertMsgLen -= (iIndexOfMsg);
						
						sTempMsg = sTempMsg.Substring(iIndexOfMsg, iAlertMsgLen);
					}
				} else {
					PagerScreen.Remove(0,PagerScreen.Length);
					PagerScreen.Append(sTempMsg);
					encodedList.AddRange((byte[])lvplFormatter.ConvertToCNS(PagerScreen.ToString()));
					iCountLength = iAlertMsgLen;
				}
				if (sAlertMsg.Length > iCountLength) {
					//clear buffer
					PagerScreen.Remove(0,PagerScreen.Length);
					if (encodedList.Count>0) encodedList.Clear();
					PagerScreen.Append(sTempMsg.Substring(0, iAlertMsgLen));
					encodedList.AddRange((byte[])lvplFormatter.ConvertToCNS(PagerScreen.ToString()));
					encodedFinList.AddRange((byte[])lvplFormatter.PadShift((byte[])encodedList.ToArray(typeof(byte))));
				} else {
					encodedFinList.AddRange((byte[])lvplFormatter.PadShift((byte[])encodedList.ToArray(typeof(byte))));
				}

				//clear buffer
				PagerScreen.Remove(0,PagerScreen.Length);
				if (encodedList.Count>0) encodedList.Clear();

				//set array fit to size
				encodedFinList.TrimToSize();

				//Retrieve pager information
				const string UPDATE_SIGN = "+";
				const string PREFIXGROUP = "999999999999";
				bool GOGOFormatted = true;
				bool HKJCFormatted = true;
				bool JCComboFormatted = true;
				int iMsgID = 0;
				int iPriority = 1;
				string sPrefix = "";
				string sCapcode = "";
				string sTone = "";
				string sGOGOHeaderID = "";
				string sHKJCHeaderID = "";
				string sSong = "0000";
				string sGOGODB = "";
				string sHKJCDB = "";
				byte[] level1Content;
				byte[] level2Content;
				int iRtn;
				
				for (i=0; i < arrSendToPager.Length; i++) {					
					if (arrSendToPager[i] == "2") {
						//GOGO Pager include GOGO1 and GOGO2
						SQLString.Remove(0, SQLString.Length);
						SQLString.Append("select IMSG_ID, CPREFIX, CCAPCODE, CTONE, IHDRIDSTART, CSONGID, IPRIORITY from PGMSG_GOGO1_CFG where CMSGTYPE='");
						SQLString.Append(sPagerMessageType);
						SQLString.Append("'");
						m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
						if(m_SportsOleReader.Read()) {
							iMsgID = m_SportsOleReader.GetInt32(0);
							sPrefix = m_SportsOleReader.GetString(1).Trim();
							sCapcode = m_SportsOleReader.GetString(2).Trim();
							sTone = m_SportsOleReader.GetString(3).Trim();
							if (sPagerMessageType == "FreeMessage")
								sGOGOHeaderID = sHeaderID;
							else
								sGOGOHeaderID = m_SportsOleReader.GetInt32(4).ToString("D4");
							sSong = m_SportsOleReader.GetString(5).Trim();
							iPriority = m_SportsOleReader.GetInt32(6);
						}
						m_SportsOleReader.Close();
						m_SportsDBMgr.Close();
						
						//Unboxing return value of lvplFormatter.FormatMessage to original type, i.e. byte[]
						level1Content = (byte[])encodedFinList.ToArray(typeof(byte));
						//byte[] level1Content = (byte[])lvplFormatter.PadShift((byte[])encodedList.ToArray(typeof(byte)));
						level2Content = (byte[])lvplFormatter.InsertPrefixGroup(sSong + PREFIXGROUP, level1Content);
						byte[] gogoLevel3Content = (byte[])lvplFormatter.InsertHeaderWithShiftIn(UPDATE_SIGN + sGOGOHeaderID, level2Content);
						byte[] gogoResultMsg = (byte[])lvplFormatter.InsertHeaderFromString(sPrefix + sCapcode + sTone, gogoLevel3Content);
		
						//Check queue to be inserted
						SQLString.Remove(0, SQLString.Length);
						SQLString.Append("select db.CDBCONN, queue.CSQL, queue.CCHANNEL from MSGREG_LVPL_CFG reg, IPMUXADPT_DBCFG db, QUEUE_SQLCFG queue where reg.CPAGER='GOGO1' and reg.IMSG_ID=");
						SQLString.Append(iMsgID.ToString());
						SQLString.Append(" and reg.IENABLED=1 and reg.IDB_ID=db.IDB_ID and reg.IQUEUE_ID=queue.IQUEUE_ID order by reg.IDB_ID, reg.IQUEUE_ID");
						m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
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
								m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " AlertSpecialMessage.cs: [SENT] GOGO - " + m_SportsOleReader.GetString(2).Trim() + " (" + m_Big5Encoded.GetString(gogoResultMsg) + ")");
								m_SportsLog.Close();
		
								iGOGOFormatted++;
							} catch(Exception ex) {
								m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
								m_SportsLog.SetFileName(0,LOGFILESUFFIX);
								m_SportsLog.Open();
								m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " AlertSpecialMessage.cs.SendRemotingMessage(): Append GOGO message to queue error:");
								m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " " + ex.ToString());
								m_SportsLog.Close();
								GOGOFormatted = false;
							}
						}
						m_SportsOleReader.Close();
						m_SportsDBMgr.Close();
						if(GOGOFormatted) {
							arrResult[0] = iGOGOFormatted;
						} else {
							arrResult[0] = -1;
						}
					} else if (arrSendToPager[i] == "5") {	
						//HKJC Pager
						iMsgID = 0;
						iPriority = 1;
						sPrefix = "";
						sCapcode = "";
						sTone = "";
						SQLString.Remove(0, SQLString.Length);
						SQLString.Append("select IMSG_ID, CPREFIX, CCAPCODE, CTONE, IHDRIDSTART, IPRIORITY from PGMSG_HKJC_CFG where CMSGTYPE='");
						SQLString.Append(sPagerMessageType);
						SQLString.Append("'");
						m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
						if(m_SportsOleReader.Read()) {
							iMsgID = m_SportsOleReader.GetInt32(0);
							sPrefix = m_SportsOleReader.GetString(1).Trim();
							sCapcode = m_SportsOleReader.GetString(2).Trim();
							sTone = m_SportsOleReader.GetString(3).Trim();
							if (sPagerMessageType == "FreeMessage")
								sHKJCHeaderID = sHeaderID;
							else
								sHKJCHeaderID = m_SportsOleReader.GetInt32(4).ToString("D4");
							iPriority = m_SportsOleReader.GetInt32(5);
						}
						m_SportsOleReader.Close();
						m_SportsDBMgr.Close();
		
						//Unboxing return value of lvplFormatter.FormatMessage to original type, i.e. byte[]
						level1Content = (byte[])encodedFinList.ToArray(typeof(byte));
						level2Content = (byte[])lvplFormatter.InsertPrefixGroup(sSong + PREFIXGROUP, level1Content);
						byte[] hkjcLevel3Content = (byte[])lvplFormatter.InsertHeaderWithShiftIn(UPDATE_SIGN + sHKJCHeaderID, level2Content);
						byte[] hkjcResultMsg = (byte[])lvplFormatter.InsertHeaderFromString(sPrefix + sCapcode + sTone, hkjcLevel3Content);
		
						//Check queue to be inserted
						SQLString.Remove(0, SQLString.Length);
						SQLString.Append("select db.CDBCONN, queue.CSQL, queue.CCHANNEL from MSGREG_LVPL_CFG reg, IPMUXADPT_DBCFG db, QUEUE_SQLCFG queue where reg.CPAGER='HKJC' and reg.IMSG_ID=");
						SQLString.Append(iMsgID.ToString());
						SQLString.Append(" and reg.IENABLED=1 and reg.IDB_ID=db.IDB_ID and reg.IQUEUE_ID=queue.IQUEUE_ID order by reg.IDB_ID, reg.IQUEUE_ID");
						m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
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
								m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " AlertSpecialMessage.cs: [SENT] HKJC - " + m_SportsOleReader.GetString(2).Trim() + " (" + m_Big5Encoded.GetString(hkjcResultMsg) + ")");
								m_SportsLog.Close();
		
								iHKJCFormatted++;
							} catch(Exception ex) {
								m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
								m_SportsLog.SetFileName(0,LOGFILESUFFIX);
								m_SportsLog.Open();
								m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " AlertSpecialMessage.cs.SendRemotingMessage(): Append HKJC message to queue error:");
								m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " " + ex.ToString());
								m_SportsLog.Close();
								HKJCFormatted = false;
							}
						}
						m_SportsOleReader.Close();
						m_SportsDBMgr.Close();
						if(HKJCFormatted) {
							arrResult[1] = iHKJCFormatted;
						} else {
							arrResult[1] = -1;
						}
					
					} else if (arrSendToPager[i] == "11") {
						string sCurrentTimestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
						string sCurrDate = DateTime.Now.ToString("yyyyMMdd");
						string sCurrTime = DateTime.Now.ToString("HHmm");
						int iAppID;

						iAppID = -1;
						iMsgID = -1;
		
						try {
							if(!sAlertMsg.Trim().Equals("")) {
								string[] arrMsgType;
								arrMsgType = (string[])HttpContext.Current.Application["messageType"];
								sAlertMsg = sAlertMsg.Trim().Replace(PAGECR,DBCR);
								sBatchJob = DateTime.Now.ToString("yyMMddHHmmss_fffffff") + "." + HttpContext.Current.Session["user_name"].ToString() + "." + arrMsgType[18] + ".ini";
									
								SQLString.Remove(0, SQLString.Length);
								SQLString.Append("select IAPP_ID, IMSG_ID from ENG_CHI_CONVERTER where ENG_NAME ='");
								SQLString.Append(sPagerMessageType);
								SQLString.Append("'");
								m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
								if(m_SportsOleReader.Read()) {
									iAppID = m_SportsOleReader.GetInt32(0);
									if (sPagerMessageType == "FreeMessage")
										iMsgID = Convert.ToInt32(sHeaderID);
									else
										iMsgID = m_SportsOleReader.GetInt32(1);
								}
								
								if (iAppID != -1 || iMsgID !=-1) {
									DBManager SptLogDBMgr = new DBManager();
									SptLogDBMgr.ConnectionString = (string)HttpContext.Current.Application["SoccerDBConnectionString"];
									
									SQLString.Remove(0, SQLString.Length);
									SQLString.Append("insert into LOG_SPORTNEWS (TIMEFLAG, SECTION, IMSG_ID, IAPP_ID, ACT, NEWSDATE, NEWSTIME, CONTENT, BATCHJOB) values ('");
									SQLString.Append(sCurrentTimestamp);
									SQLString.Append("','NEWS_',");
									SQLString.Append(iMsgID.ToString());
									SQLString.Append(",");
									SQLString.Append(iAppID.ToString());
									SQLString.Append(",'U','");
									SQLString.Append(sCurrDate);
									SQLString.Append("','");
									SQLString.Append(sCurrTime);
									SQLString.Append("','");
									SQLString.Append(sAlertMsg);
									SQLString.Append("','");
									SQLString.Append(sBatchJob);
									SQLString.Append("')");
									SptLogDBMgr.ExecuteNonQuery(SQLString.ToString());
									SptLogDBMgr.Close();
									iJCComboFormatted++;
								} else {
									m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
									m_SportsLog.SetFileName(0,LOGFILESUFFIX);
									m_SportsLog.Open();
									m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " AlertSpecialMessage.cs: Empty of AppID and MsgID, cannot be sent (" + HttpContext.Current.Session["user_name"] + ")");
									m_SportsLog.Close();
									arrResult[5] = -4;
								}
							} else {
								//write log
								m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
								m_SportsLog.SetFileName(0,LOGFILESUFFIX);
								m_SportsLog.Open();
								m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " AlertSpecialMessage.cs: Trimed message is empty, cannot be sent (" + HttpContext.Current.Session["user_name"] + ")");
								m_SportsLog.Close();
								arrResult[5] = -3;
							}
						} catch(Exception ex) {
							m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
							m_SportsLog.SetFileName(0,LOGFILESUFFIX);
							m_SportsLog.Open();
							m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " AlertSpecialMessage.cs.Send(): " + ex.ToString());
							m_SportsLog.Close();
							JCComboFormatted = false;
						}
						if(JCComboFormatted) {
							arrResult[2] = iJCComboFormatted;
						} else {
							arrResult[2] = -1;
						}
					}
				}
				
				//Notfiy IPMux Adapter
				for (i=0; i < arrSendToPager.Length; i++) {	
					//Notfiy IPMux Adapter for GOGO
					if (arrSendToPager[i] == "2") {
						iRtn = 0;
						if(GOGOFormatted) {
							try {
								Win32Message gogomsg = (Win32Message)Activator.GetObject(typeof(RemoteService.Win32.Win32Message), HttpContext.Current.Application["Broadcast2GOGOObjURI"].ToString());
								iRtn = gogomsg.Broadcast(HttpContext.Current.Application["IPMUXAdptBCStr"].ToString());
								if(iRtn == SUCCESS_CODE) {
									m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
									m_SportsLog.SetFileName(0,LOGFILESUFFIX);
									m_SportsLog.Open();
									m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " AlertSpecialMessage.cs: [POST] GOGO - Notfiy IPMux Adapter success (" + HttpContext.Current.Session["user_name"] + ")");
									m_SportsLog.Close();
									arrResult[3] = SUCCESS_CODE;
								} else {
									m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
									m_SportsLog.SetFileName(0,LOGFILESUFFIX);
									m_SportsLog.Open();
									m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " AlertSpecialMessage.cs: [POST] GOGO - Notfiy IPMux Adapter error, code: " + iRtn.ToString());
									m_SportsLog.Close();
									arrResult[3] = -1;
								}
							} catch(Exception ex) {
								m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
								m_SportsLog.SetFileName(0,LOGFILESUFFIX);
								m_SportsLog.Open();
								m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " AlertSpecialMessage.cs.SendRemotingMessage(): GOGO - Notfiy IPMux Adapter throws exception: " + ex.ToString());
								m_SportsLog.Close();
								arrResult[3] = -2;
							}
						} else {
							arrResult[3] = -3;
						}
					} else if (arrSendToPager[i] == "5") {
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
									m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " AlertSpecialMessage.cs: [POST] HKJC - Notfiy IPMux Adapter success (" + HttpContext.Current.Session["user_name"] + ")");
									m_SportsLog.Close();
									arrResult[4] = SUCCESS_CODE;
								} else {
									m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
									m_SportsLog.SetFileName(0,LOGFILESUFFIX);
									m_SportsLog.Open();
									m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " AlertSpecialMessage.cs: [POST] HKCJ - Notfiy IPMux Adapter error, code: " + iRtn.ToString());
									m_SportsLog.Close();
									arrResult[4] = -1;
								}
							} catch(Exception ex) {
								m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
								m_SportsLog.SetFileName(0,LOGFILESUFFIX);
								m_SportsLog.Open();
								m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " AlertSpecialMessage.cs.SendRemotingMessage(): HKJC - Notfiy IPMux Adapter throws exception: " + ex.ToString());
								m_SportsLog.Close();
								arrResult[4] = -2;
							}
						} else {
							arrResult[4] = -3;
						}
					} else if (arrSendToPager[i] == "11"){
						if (arrResult[2] > 0) {
							//SportsMessage object message
							SportsMessage sptMsg = new SportsMessage();
	
							//JC Combo alert message						 
							string[] arrQueueNames;
							string[] arrMessageTypes;
							string[] arrRemotingPath;
							
							arrQueueNames = (string[])HttpContext.Current.Application["QueueItems"];
							arrMessageTypes = (string[])HttpContext.Current.Application["NotifyMessageTypes"];
							arrRemotingPath = (string[])HttpContext.Current.Application["RemotingItems"];
							MessageClient msgClt = new MessageClient();
							msgClt.MessageType = arrMessageTypes[0];
							msgClt.MessagePath = arrQueueNames[0];
							
							sptMsg.IsTransaction = false;
							sptMsg.Body = sBatchJob;
							sptMsg.Timestamp = DateTime.Now.ToString("yyMMddHHmmss");
							sptMsg.AppID = "07";
							sptMsg.MsgID = "13";
							sptMsg.PathID = new string[0];
							sptMsg.AddPathID((string)arrSendToPager[i]);
							
							try {
								//Notify via MSMQ
								msgClt.MessageType = arrMessageTypes[0];
								msgClt.MessagePath = arrQueueNames[0];
								msgClt.SendMessage(sptMsg);
								arrResult[5] = SUCCESS_CODE;
							} catch(System.Messaging.MessageQueueException mqEx) {
								try {
									m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
									m_SportsLog.SetFileName(0,LOGFILESUFFIX);
									m_SportsLog.Open();
									m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " MSMQ ERROR: Alert Special Message");
									m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " AlertSpecialMessage.cs.Send(): Notify via MSMQ throws MessageQueueException:  " + mqEx.ToString());
									m_SportsLog.Close();
	
									//If MSMQ fail, notify via .NET Remoting
									msgClt.MessageType = arrMessageTypes[1];
									msgClt.MessagePath = arrRemotingPath[0];
									if(!msgClt.SendMessage((object)sptMsg)) {
										m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
										m_SportsLog.SetFileName(0,LOGFILESUFFIX);
										m_SportsLog.Open();
										m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Remoting ERROR: Alert Special Message");
										m_SportsLog.Close();
									} else
										arrResult[5] = SUCCESS_CODE;
								} catch(Exception ex) {
									m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
									m_SportsLog.SetFileName(0,LOGFILESUFFIX);
									m_SportsLog.Open();
									m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Remoting ERROR: Alert Special Message");
									m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " AlertSpecialMessage.cs.Send(): Notify via .NET Remoting throws exception: " + ex.ToString());
									m_SportsLog.Close();
									arrResult[5] = -2;
								}							
							} catch(Exception ex) {
								m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
								m_SportsLog.SetFileName(0,LOGFILESUFFIX);
								m_SportsLog.Open();
								m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " AlertSpecialMessage.cs.Send(): Notify via MSMQ throws exception: " + ex.ToString());
								m_SportsLog.Close();
							}
						}
					}
				}
			} catch(Exception ex) {
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " AlertSpecialMessage.cs.SendRemotingMessage(): " + ex.ToString());
				m_SportsLog.Close();
				arrResult[6] = -1;
			}			
			return arrResult;
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
		
		public string[] GetSendToPager() {
			string[] arrSendToPager;
			
			try {
				arrSendToPager = HttpContext.Current.Request.Form["SendToPager"].Split(delimiter);
			} catch(Exception) {
				arrSendToPager = new string[0];
			}
			return arrSendToPager;
		}
	}
}