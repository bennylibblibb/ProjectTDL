/*
Objective:
Preview and send HKJC Soccer records

Last updated:
27 Jan 2004 by Rita

C#.NET complier statement:
csc /t:library /out:..\bin\HKJCSendPage.dll /r:..\bin\DBManager.dll;..\bin\Files.dll;..\bin\MessageClient.dll;..\bin\SportsMessage.dll HKJCSendPage.cs
*/

using System;
using System.Collections;
using System.Collections.Specialized;
using System.Data.OleDb;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Web;
using TDL.DB;
using TDL.IO;
using TDL.Message;

[assembly:AssemblyCompany("TDSL")]
[assembly:AssemblyCopyright("(c) 2003 TDSL. All rights reserved. Created on 12 August 2003.")]
[assembly:AssemblyDescription("JC足智彩 -> 發送賽事")]
[assembly:AssemblyProduct("Sports.NET")]
[assembly:AssemblyTitle("Sports.NET DLL")]
[assembly:AssemblyVersion("3.1.*")]
namespace SportsUtil {
	public class HKJCSendPage {
		const string LOGFILESUFFIX = "log";
		const string COMMA = ",";
		int iRecordsInPage = 0;
		OleDbDataReader m_SportsOleReader;
		DBManager m_SportsDBMgr;
		Files m_SportsLog;
		Encoding m_Big5Encoded;
		StringBuilder SQLString;
		NameValueCollection taskNVC;

		[DllImport("advapi32.dll", SetLastError=true)]
		public static extern bool LogonUser(string lpszUserName, string lpszDomain, string lpszPassword, int dwLogonType, int dwLogonProvider, ref IntPtr phToken);

		[DllImport("advapi32.dll", SetLastError=true)]
		public extern static int DuplicateToken(IntPtr hToken, int impersonationLevel, ref IntPtr hNewToken);

		public HKJCSendPage(string Connection) {
			m_SportsDBMgr = new DBManager();
			m_SportsDBMgr.ConnectionString = Connection;
			m_SportsLog = new Files();
			m_Big5Encoded = Encoding.GetEncoding(950);
			SQLString = new StringBuilder();
			taskNVC = (NameValueCollection)HttpContext.Current.Application["HKJCAdminTaskItems"];
		}

		public int NumberOfRecords {
			get {
				return iRecordsInPage;
			}
		}

		public string Preview() {
			string sMatchCount = "";
			string uid;
			string[] weekArray;
			StringBuilder HTMLString = new StringBuilder();

			try {
				weekArray = (string[])HttpContext.Current.Application["WeekItems"];
				uid = HttpContext.Current.Session["user_id"].ToString();

				SQLString.Remove(0,SQLString.Length);
				SQLString.Append("select main.IMATCH_CNT, main.IWEEK, main.ISEQ_NO, main.MATCHDATETIME, main.CLEAGUEALIAS, main.CHOST, main.CGUEST from HKJCSOCCER_INFO main, leaginfo leag where main.CACT='U' and main.CLEAGUEALIAS = leag.alias and leaginfo.leag_id in (select cleag_id from userprofile_info where iuser_id=");
				SQLString.Append(uid);
				SQLString.Append(") order by leag.leag_order, main.MATCHDATETIME, main.IWEEK, main.ISEQ_NO");
				m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
				while(m_SportsOleReader.Read()) {
					HTMLString.Append("<tr align=\"center\">");

					//Match count (Hidden Field), week, sequence number
					sMatchCount = m_SportsOleReader.GetInt32(0).ToString();
					HTMLString.Append("<td><input type=\"hidden\" name=\"MatchCount\" value=\"");
					HTMLString.Append(sMatchCount);
					HTMLString.Append("\">");
					HTMLString.Append(weekArray[m_SportsOleReader.GetInt32(1)]);
					HTMLString.Append(" ");
					HTMLString.Append(m_SportsOleReader.GetInt32(2).ToString());
					HTMLString.Append("</td>");

					//Match date time
					HTMLString.Append("<td>");
					HTMLString.Append(m_SportsOleReader.GetDateTime(3).ToString("yyyy/MM/dd HH:mm"));
					HTMLString.Append("</td>");

					//Alias
					HTMLString.Append("<td>");
					HTMLString.Append(m_SportsOleReader.GetString(4));
					HTMLString.Append("</td>");

					//Host
					HTMLString.Append("<td>");
					HTMLString.Append(m_SportsOleReader.GetString(5));
					HTMLString.Append("</td>");

					//Guest
					HTMLString.Append("<td>");
					HTMLString.Append(m_SportsOleReader.GetString(6));
					HTMLString.Append("</td>");

					//Send checkbox
					HTMLString.Append("<td><input type=\"checkbox\" name=\"ItemToSend\" value=\"");
					HTMLString.Append(sMatchCount);
					HTMLString.Append("\"></td></tr>");
					iRecordsInPage++;
				}
				m_SportsOleReader.Close();
				m_SportsDBMgr.Close();
				m_SportsDBMgr.Dispose();
			} catch(Exception ex) {
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " HKJCSendPage.cs.Preview(): " + ex.ToString());
				m_SportsLog.Close();
				HTMLString.Remove(0,HTMLString.Length);
				HTMLString.Append((string)HttpContext.Current.Application["accessErrorMsg"]);
			}

			return HTMLString.ToString();
		}

		public int Send() {
			const int LOGON32_LOGON_INTERACTIVE = 2;
			const int LOGON32_PROVIDER_DEFAULT = 0;
			const int SecurityImpersonation = 2;
			bool m_LoggedOn = false;
			const string m_NetUser = "ASPUSER";
			const string m_Password = "aspspt";
			string m_Domain = System.Environment.MachineName;
			WindowsImpersonationContext m_ImpersonationContext;
			WindowsIdentity m_TempWindowsIdentity;
			IntPtr m_Token = IntPtr.Zero;
			IntPtr m_TokenDuplicate = IntPtr.Zero;

			bool bLogValid = false;
			int iIdx = 0;
			int iItemCount = 0;
			string sINIFileName = "";
			string sMatchCount = "";
			string sHandleCode;
			char[] delimiter = new char[] {','};
			string[] arrMatchCnt;
			string[] arrItemsSend;
			string[] arrWeek = new string[8];
			string[] arrMsgType;
			string[] arrRemotingPath;
			ArrayList ODD_INIContentAL = new ArrayList(32);
			StringBuilder TempINIContent = new StringBuilder();

			//SportsMessage object message
			SportsMessage sptMsg = new SportsMessage();

			arrWeek[0] = "SUN";
			arrWeek[1] = "MON";
			arrWeek[2] = "TUE";
			arrWeek[3] = "WED";
			arrWeek[4] = "THU";
			arrWeek[5] = "FRI";
			arrWeek[6] = "SAT";
			arrWeek[7] = "OTH";
			sHandleCode = (string)HttpContext.Current.Request.Form["handlecode"];
			arrMsgType = (string[])HttpContext.Current.Application["messageType"];
			arrRemotingPath = (string[])HttpContext.Current.Application["RemotingItems"];
			arrMatchCnt = HttpContext.Current.Request.Form["MatchCount"].Split(delimiter);
			try {
				arrItemsSend = HttpContext.Current.Request.Form["ItemToSend"].Split(delimiter);
			}	catch(Exception) {
				arrItemsSend = new string[0];
			}

			try {
				/*****************************
				 * HKJC Soccer alert message *
				 *****************************/
				int iMsgBodyLength;
				string sMsgBody = null;
				string[] arrQueueNames;
				string[] arrMessageTypes;
				byte[] arrByteOfMSMQBody;
				arrQueueNames = (string[])HttpContext.Current.Application["QueueItems"];
				arrMessageTypes = (string[])HttpContext.Current.Application["NotifyMessageTypes"];
				MessageClient msgClt = new MessageClient();
				msgClt.MessageType = arrMessageTypes[0];
				msgClt.MessagePath = arrQueueNames[0];


				if(arrItemsSend.Length>0) {
					for(iIdx=0;iIdx<arrItemsSend.Length;iIdx++) {
						bLogValid = false;
						ODD_INIContentAL.Add("[ODD_" + (iIdx+1).ToString() + "]");
						sMatchCount = arrItemsSend[iIdx].Trim();
						SQLString.Remove(0,SQLString.Length);
						SQLString.Append("select CLEAGUEALIAS, CLEAGUE, MATCHDATETIME, CFIELD, CHOST, CGUEST, IWEEK, ISEQ_NO, CSTATUS from HKJCSOCCER_INFO where IMATCH_CNT=");
						SQLString.Append(sMatchCount);
						m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
						if(m_SportsOleReader.Read()) {
							bLogValid = true;
							ODD_INIContentAL.Add("LeagueAlias=" + m_SportsOleReader.GetString(0).Trim());
							ODD_INIContentAL.Add("League=" + m_SportsOleReader.GetString(1).Trim());
							ODD_INIContentAL.Add("MatchDate=" + m_SportsOleReader.GetDateTime(2).ToString("yyyyMMdd"));
							ODD_INIContentAL.Add("MatchTime=" + m_SportsOleReader.GetDateTime(2).ToString("HHmm"));
							ODD_INIContentAL.Add("MatchField=" + m_SportsOleReader.GetString(3).Trim());
							ODD_INIContentAL.Add("Host=" + m_SportsOleReader.GetString(4).Trim());
							ODD_INIContentAL.Add("Guest=" + m_SportsOleReader.GetString(5).Trim());
							ODD_INIContentAL.Add("Action=U");
							ODD_INIContentAL.Add("Handicap=-1");
							ODD_INIContentAL.Add("T_HandiOwner=-1");
							ODD_INIContentAL.Add("M_HandiOwner=-1");
							ODD_INIContentAL.Add("T_Handi=-1");
							ODD_INIContentAL.Add("M_Handi=-1");
							ODD_INIContentAL.Add("T_LiveOdds=正常");
							ODD_INIContentAL.Add("M_LiveOdds=正常");
							ODD_INIContentAL.Add("T_Odds=-1");
							ODD_INIContentAL.Add("M_Odds=-1");
							ODD_INIContentAL.Add("Alert=0");
							ODD_INIContentAL.Add("Interval=0");
							ODD_INIContentAL.Add("MatchDay=" + arrWeek[m_SportsOleReader.GetInt32(6)]);
							ODD_INIContentAL.Add("MatchNo=" + m_SportsOleReader.GetInt32(7).ToString());
							ODD_INIContentAL.Add("Status=" + m_SportsOleReader.GetString(8).Trim());
						}
						m_SportsOleReader.Close();
						m_SportsDBMgr.Close();

						if(bLogValid) {
							//Get HDA info
							SQLString.Remove(0,SQLString.Length);
							SQLString.Append("select CH_ODDS, CA_ODDS, CD_ODDS from HKJCSOCHDA_INFO where IMATCH_CNT=");
							SQLString.Append(sMatchCount);
							m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
							if(m_SportsOleReader.Read()) {
								if((!m_SportsOleReader.IsDBNull(0)) || (!m_SportsOleReader.IsDBNull(1))) {
									if(m_SportsOleReader.GetString(0).Trim().Equals("") || m_SportsOleReader.GetString(1).Trim().Equals("")) {
										ODD_INIContentAL.Add("E_Odds1=-1/-1");
									} else {
										ODD_INIContentAL.Add("E_Odds1=" + m_SportsOleReader.GetString(0).Trim() + "/" + m_SportsOleReader.GetString(1).Trim());
									}
								} else {
									ODD_INIContentAL.Add("E_Odds1=-1/-1");
								}

								if(!m_SportsOleReader.IsDBNull(2)) {
									if(m_SportsOleReader.GetString(2).Trim().Equals("")) {
										ODD_INIContentAL.Add("E_Odds2=-1");
									} else {
										ODD_INIContentAL.Add("E_Odds2=" + m_SportsOleReader.GetString(2).Trim());
									}
								} else {
									ODD_INIContentAL.Add("E_Odds2=-1");
								}
							}
							m_SportsOleReader.Close();
							m_SportsDBMgr.Close();


							//Get CRS info
							iItemCount = 0;
							SQLString.Remove(0,SQLString.Length);
							SQLString.Append("select CODDS from HKJCSOCCRS_INFO where IMATCH_CNT=");
							SQLString.Append(sMatchCount);
							SQLString.Append(" AND IHOME_SCORE<>-1 order by IHOME_SCORE, IAWAY_SCORE");
							m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
							while(m_SportsOleReader.Read()) {
								if(iItemCount%5 == 0) {
									TempINIContent.Remove(0,TempINIContent.Length);
								}

								if(!m_SportsOleReader.IsDBNull(0)) {
									if(!m_SportsOleReader.GetString(0).Trim().Equals("")) {
										TempINIContent.Append(m_SportsOleReader.GetString(0).Trim());
									} else {
										TempINIContent.Append("-1");
									}
								} else {
									TempINIContent.Append("-1");
								}
								TempINIContent.Append(COMMA);
								iItemCount++;

								if(iItemCount%5 == 0) {
									ODD_INIContentAL.Add("CRS_" + ((iItemCount/5)-1).ToString() + "=" + TempINIContent.ToString());
								}
							}
							m_SportsOleReader.Close();
							m_SportsDBMgr.Close();

							TempINIContent.Remove(0,TempINIContent.Length);
							SQLString.Remove(0,SQLString.Length);
							SQLString.Append("select CODDS from HKJCSOCCRS_INFO where IMATCH_CNT=");
							SQLString.Append(sMatchCount);
							SQLString.Append(" AND IHOME_SCORE=-1 order by IAWAY_SCORE");
							m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
							while(m_SportsOleReader.Read()) {
								if(!m_SportsOleReader.IsDBNull(0)) {
									if(!m_SportsOleReader.GetString(0).Trim().Equals("")) {
										TempINIContent.Append(m_SportsOleReader.GetString(0).Trim());
									} else {
										TempINIContent.Append("-1");
									}
								} else {
									TempINIContent.Append("-1");
								}
								TempINIContent.Append(COMMA);
							}
							m_SportsOleReader.Close();
							m_SportsDBMgr.Close();
							ODD_INIContentAL.Add("CRS_-1=" + TempINIContent.ToString());


							//Get TTG info
							TempINIContent.Remove(0,TempINIContent.Length);
							SQLString.Remove(0,SQLString.Length);
							SQLString.Append("select CODDS from HKJCSOCTTG_INFO where IMATCH_CNT=");
							SQLString.Append(sMatchCount);
							SQLString.Append(" order by ITOTAL_GOAL");
							m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
							while(m_SportsOleReader.Read()) {
								if(!m_SportsOleReader.IsDBNull(0)) {
									if(!m_SportsOleReader.GetString(0).Trim().Equals("")) {
										TempINIContent.Append(m_SportsOleReader.GetString(0).Trim());
									} else {
										TempINIContent.Append("-1");
									}
								} else {
									TempINIContent.Append("-1");
								}
								TempINIContent.Append(COMMA);
							}
							m_SportsOleReader.Close();
							m_SportsDBMgr.Close();
							ODD_INIContentAL.Add("TTG=" + TempINIContent.ToString());
						}	//end-if(bLogValid)
					}	//end-for
					ODD_INIContentAL.TrimToSize();

					sINIFileName = HttpContext.Current.Session["user_name"].ToString() + "." + arrMsgType[26] + ".ini";
					m_LoggedOn = LogonUser(m_NetUser, m_Domain, m_Password, LOGON32_LOGON_INTERACTIVE, LOGON32_PROVIDER_DEFAULT, ref m_Token);
					if(m_LoggedOn) {
						if(DuplicateToken(m_Token, SecurityImpersonation, ref m_TokenDuplicate) != 0) {
							m_TempWindowsIdentity = new WindowsIdentity(m_TokenDuplicate);
							m_ImpersonationContext = m_TempWindowsIdentity.Impersonate();
							if(m_ImpersonationContext != null) {
								m_SportsLog.FilePath = HttpContext.Current.Application["JCDescFilePath"].ToString();
								m_SportsLog.SetFileName(1,sINIFileName);
								m_SportsLog.Open();
								m_SportsLog.Write(ODD_INIContentAL);
								m_SportsLog.Close();
								m_ImpersonationContext.Undo();
							} else {
								m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
								m_SportsLog.SetFileName(0,LOGFILESUFFIX);
								m_SportsLog.Open();
								m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " HKJCSendPage.cs.Send(): m_ImpersonationContext is null.");
								m_SportsLog.Close();
							}
						} else {
							m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
							m_SportsLog.SetFileName(0,LOGFILESUFFIX);
							m_SportsLog.Open();
							m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " HKJCSendPage.cs.Send(): DuplicateToken error, code = " + Marshal.GetLastWin32Error());
							m_SportsLog.Close();
						}
					} else {
						m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
						m_SportsLog.SetFileName(0,LOGFILESUFFIX);
						m_SportsLog.Open();
						m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " HKJCSendPage.cs.Send(): LogonUser error, code = " + Marshal.GetLastWin32Error());
						m_SportsLog.Close();
					}

					DBManager JCSportsDBMgr = new DBManager();
					JCSportsDBMgr.ConnectionString = (string)HttpContext.Current.Application["JCSOCCERDBConnectionString"];
					SQLString.Remove(0,SQLString.Length);
					SQLString.Append("insert into PENDINGLIST (CNAME) values('");
					SQLString.Append(m_SportsLog.FileName);
					SQLString.Append("')");
					JCSportsDBMgr.ExecuteNonQuery(SQLString.ToString());
					JCSportsDBMgr.Close();
					JCSportsDBMgr.Dispose();

					/*****************************
					 * HKJC soccer alert message *
					 *****************************/
					//HKJC_Soccer> Alert message format: AppID MsgID Priority AlertID MsgLen WParam
					sptMsg.AppID = "05";
					sptMsg.MsgID = "00";
					sMsgBody = taskNVC[sHandleCode];
					arrByteOfMSMQBody = m_Big5Encoded.GetBytes(sMsgBody);
					iMsgBodyLength = arrByteOfMSMQBody.Length;
					sptMsg.Body = iMsgBodyLength.ToString("D3") + sMsgBody;
					sptMsg.Timestamp = DateTime.Now.ToString("yyMMddHHmmss");
					//Send Notify Message
					//Modified by Rita, 27 Jan 2004	
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
							m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " MSMQ ERROR: HKJC Send Page");
							m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " HKJCSendPage.cs.Send(): Notify via MSMQ throws MessageQueueException:  " + mqEx.ToString());
							m_SportsLog.Close();

							//If MSMQ fail, notify via .NET Remoting
							msgClt.MessageType = arrMessageTypes[1];
							msgClt.MessagePath = arrRemotingPath[0];
							if(!msgClt.SendMessage((object)sptMsg)) {
								m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
								m_SportsLog.SetFileName(0,LOGFILESUFFIX);
								m_SportsLog.Open();
								m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Remoting ERROR: HKJC Send Page");
								m_SportsLog.Close();
							}
						}	catch(Exception ex) {
							m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
							m_SportsLog.SetFileName(0,LOGFILESUFFIX);
							m_SportsLog.Open();
							m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Remoting ERROR: HKJC Send Page");
							m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " HKJCSendPage.cs.Send(): Notify via .NET Remoting throws exception: " + ex.ToString());
							m_SportsLog.Close();
						}							
					}	catch(Exception ex) {
						m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
						m_SportsLog.SetFileName(0,LOGFILESUFFIX);
						m_SportsLog.Open();
						m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " HKJCSendPage.cs.Send(): Notify via MSMQ throws exception: " + ex.ToString());
						m_SportsLog.Close();
					}		
					/*
					try {
						msgClt.SendMessage(sptMsg);
					} catch(Exception ex) {
						m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
						m_SportsLog.SetFileName(0,LOGFILESUFFIX);
						m_SportsLog.Open();
						m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " HKJCSendPage.cs.Send(): Send notify message throws exception: " + ex.ToString());
						m_SportsLog.Close();
					}
					*/
					//Modified end		
					//write log
					m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
					m_SportsLog.SetFileName(0,LOGFILESUFFIX);
					m_SportsLog.Open();
					m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " HKJCSendPage.cs: Send " + iIdx.ToString() + " matchess (" + HttpContext.Current.Session["user_name"] + ")");
					m_SportsLog.Close();
				}
			}	catch(Exception ex) {
				iIdx = -1;
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " HKJCSendPage.cs.Send(): " + ex.ToString());
				m_SportsLog.Close();
			}

			return iIdx;
		}
	}
}