/*
Objective:
Create command ini file to resend pager data

Last updated:
19 Feb 2004, Chapman Choi

C#.NET complier statement:
csc /t:library /out:..\bin\ResendPagerDetails.dll /r:..\bin\DBManager.dll;..\bin\Files.dll;..\bin\MessageClient.dll;..\bin\SportsMessage.dll ResendPagerDetails.cs
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
[assembly:AssemblyDescription("重發/刪除資訊 -> 重發傳呼機資訊")]
[assembly:AssemblyProduct("Sports.NET")]
[assembly:AssemblyTitle("Sports.NET DLL")]
[assembly:AssemblyVersion("3.1.*")]
namespace SportsUtil {
	public class ResendPagerDetails {
		const string CMDTYPE = "RESEND";
		const string LOGFILESUFFIX = "log";
		string m_Role;
		Files m_SportsLog;
		Encoding m_Big5Encoded;
		StringBuilder SQLString;
		OleDbDataReader m_SportsOleReader;
		DBManager m_SportsDBMgr;

		public ResendPagerDetails(string Connection) {
			m_SportsDBMgr = new DBManager();
			m_SportsDBMgr.ConnectionString = Connection;
			m_SportsLog = new Files();
			m_Big5Encoded = Encoding.GetEncoding(950);
			m_Role = (string)HttpContext.Current.Session["user_role"];
			if(m_Role == null) m_Role = "0";
			if(m_Role.Equals("")) m_Role = "0";
			SQLString = new StringBuilder();
		}

		public string ShowItems() {
			StringBuilder HTMLString = new StringBuilder();
			string sSvcID = "";
			sSvcID = HttpContext.Current.Request.QueryString["svc"].Trim();

			try {
				if(Convert.ToInt32(m_Role) > 10) {
					string sSvcRight = "";
					string sSvcName = "";
					//Double check for user right
					SQLString.Remove(0,SQLString.Length);
					SQLString.Append("select CACCESS_RIGHT, CSVC_NAME from SVCCFG where ISVC_NO=");
					SQLString.Append(sSvcID);
					SQLString.Append(" and DEL_TYPE='P'");
					m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
					if(m_SportsOleReader.Read()) {
						sSvcRight = m_SportsOleReader.GetString(0).Trim();
						sSvcName = m_SportsOleReader.GetString(1).Trim();
					}
					m_SportsOleReader.Close();
					m_SportsDBMgr.Close();

					if(Convert.ToInt32(m_Role) >= Convert.ToInt32(sSvcRight)) {
						int iItemRow = 0;
						string sCategory = "";
						SQLString.Remove(0,SQLString.Length);
						SQLString.Append("select CCATEGORY, CPARAM, CDISPLAY from ADMINCFG where IADMIN_NO in (select IADMIN_NO from ADMINSVCMAP where ISVC_NO=");
						SQLString.Append(sSvcID);
						SQLString.Append(") AND CCMD_TYPE='");
						SQLString.Append(CMDTYPE);
						SQLString.Append("' group by CCATEGORY, CDISPLAY, CPARAM order by CCATEGORY, IADMIN_NO");
						m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
						while(m_SportsOleReader.Read()) {
							if(!sCategory.Equals(m_SportsOleReader.GetString(0).Trim())) {
								sCategory = m_SportsOleReader.GetString(0).Trim();
								HTMLString.Append("<tr align=\"left\"><th colspan=\"5\" bgcolor=\"#FF69B4\">");
								HTMLString.Append(sSvcName);
								HTMLString.Append(" - ");
								HTMLString.Append(sCategory);
								HTMLString.Append("</th></tr>");
								iItemRow = 0;
							}
							if(iItemRow%5 == 0) {
								HTMLString.Append("<tr align=\"left\">");
							}
							HTMLString.Append("<td><input type=\"checkbox\" name=\"ItemToResend\" value=\"");
							HTMLString.Append(m_SportsOleReader.GetString(1).Trim());
							HTMLString.Append("\">");
							HTMLString.Append(m_SportsOleReader.GetString(2).Trim());
							HTMLString.Append("</td>");
							if(iItemRow%5 == 4) {
								HTMLString.Append("</tr>");
							}
							iItemRow++;
						}
						m_SportsOleReader.Close();
						m_SportsDBMgr.Close();
						m_SportsDBMgr.Dispose();
						if(HTMLString.Length == 0) {
							HTMLString.Append("<tr><th bgcolor=\"#FF69B4\">不適用</th></tr>");
						}
						HTMLString.Append("<input type=\"hidden\" name=\"svcID\" value=\"");
						HTMLString.Append(sSvcID);
						HTMLString.Append("\">");
					} else {
						HTMLString.Append("<tr><th bgcolor=\"#FF69B4\"沒有權限</th></tr>");
					}
				} else {
					HTMLString.Append("<tr><th bgcolor=\"#FF69B4\"沒有權限</th></tr>");
				}
			} catch(Exception ex) {
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " ResendPagerDetails.cs.ShowItems(): " + ex.ToString());
				m_SportsLog.Close();
				HTMLString.Remove(0,HTMLString.Length);
				HTMLString.Append((string)HttpContext.Current.Application["accessErrorMsg"]);
			}

			return HTMLString.ToString();
		}

		public int Resend() {
			int iResendItems;
			int iResended = 0;
			int iRecUpd = 0;
			string sSvcID = "";
			string sCurrentTimestamp = null;
			string sBatchJob = null;
			string[] arrRemotingPath;
			sSvcID = HttpContext.Current.Request.Form["svcID"].Trim();
			arrRemotingPath = (string[])HttpContext.Current.Application["RemotingItems"];

			try {
				if(Convert.ToInt32(m_Role) > 10) {
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

					char[] delimiter = new char[] {','};
					string[] arrResendItems;
					string[] arrMsgType;
					arrMsgType = (string[])HttpContext.Current.Application["messageType"];
					try {
						arrResendItems = (string[])HttpContext.Current.Request.Form["ItemToResend"].Split(delimiter);
						iResendItems = arrResendItems.Length;
					} catch(Exception) {
						arrResendItems = new string[0];
						iResendItems = 0;
					}

					//Double check for user right
					SQLString.Remove(0,SQLString.Length);
					SQLString.Append("select CACCESS_RIGHT from SVCCFG where ISVC_NO=");
					SQLString.Append(sSvcID);
					SQLString.Append(" and DEL_TYPE='P'");
					string sSvcRight = m_SportsDBMgr.ExecuteQueryString(SQLString.ToString());
					m_SportsDBMgr.Close();

					if(Convert.ToInt32(m_Role) >= Convert.ToInt32(sSvcRight)) {
						if(iResendItems > 0) {	//resend item(s)
							for(iResended = 0; iResended < iResendItems; iResended++) {
								sCurrentTimestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
								sBatchJob = DateTime.Now.ToString("yyMMddHHmmss_fffffff") + "." + HttpContext.Current.Session["user_name"].ToString() + "." + arrMsgType[12] + "." + iResended.ToString() + ".ini";
								SQLString.Remove(0,SQLString.Length);
								SQLString.Append("insert into LOG_ADMINTASK (TIMEFLAG, SECTION, CMD, PARAM, BATCHJOB) values ('");
								SQLString.Append(sCurrentTimestamp);
								SQLString.Append("','COMMAND_','");
								SQLString.Append(CMDTYPE);
								SQLString.Append("','");
								SQLString.Append(arrResendItems[iResended]);
								SQLString.Append("','");
								SQLString.Append(sBatchJob);
								SQLString.Append("')");
								m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
								m_SportsDBMgr.Close();

								//Send Notify Message
								//Modified by Chapman, 19 Feb 2004
								sptMsg.IsTransaction = false;
								sptMsg.Body = sBatchJob;
								sptMsg.Timestamp = DateTime.Now.ToString("yyMMddHHmmss");
								sptMsg.AppID = "07";
								sptMsg.MsgID = "23";
								sptMsg.PathID = new string[0];
								sptMsg.AddPathID(sSvcID);
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
										m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " MSMQ ERROR: Resend Pager Details");
										m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " ResendPagerDetails.cs.Resend(): Notify via MSMQ throws MessageQueueException:  " + mqEx.ToString());
										m_SportsLog.Close();
	
										//If MSMQ fail, notify via .NET Remoting
										msgClt.MessageType = arrMessageTypes[1];
										msgClt.MessagePath = arrRemotingPath[0];
										if(!msgClt.SendMessage((object)sptMsg)) {
											m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
											m_SportsLog.SetFileName(0,LOGFILESUFFIX);
											m_SportsLog.Open();
											m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Remoting ERROR: Resend Pager Details");
											m_SportsLog.Close();
										}
									}	catch(Exception ex) {
										m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
										m_SportsLog.SetFileName(0,LOGFILESUFFIX);
										m_SportsLog.Open();
										m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Remoting ERROR: Resend Pager Details");
										m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " ResendPagerDetails.cs.Resend(): Notify via .NET Remoting throws exception: " + ex.ToString());
										m_SportsLog.Close();
									}							
								}	catch(Exception ex) {
									m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
									m_SportsLog.SetFileName(0,LOGFILESUFFIX);
									m_SportsLog.Open();
									m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " ResendPagerDetails.cs.Resend(): Notify via MSMQ throws exception: " + ex.ToString());
									m_SportsLog.Close();
								}
								System.Threading.Thread.Sleep(1500);
							}
						}
						iRecUpd = iResended;

						//write log
						m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
						m_SportsLog.SetFileName(0,LOGFILESUFFIX);
						m_SportsLog.Open();
						m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " ResendPagerDetails.cs: resend " + iRecUpd.ToString() + " items (" + HttpContext.Current.Session["user_name"] + ")");
						m_SportsLog.Close();
					} else {
						iRecUpd = -99;
						m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
						m_SportsLog.SetFileName(0,LOGFILESUFFIX);
						m_SportsLog.Open();
						m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " ResendPagerDetails.cs: " + HttpContext.Current.Session["user_name"].ToString() + " did not allow to resend pager data (" + HttpContext.Current.Session["user_name"] + ")");
						m_SportsLog.Close();
					}	//no right to resend info
				} else {
					iRecUpd = -99;
					m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
					m_SportsLog.SetFileName(0,LOGFILESUFFIX);
					m_SportsLog.Open();
					m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " ResendPagerDetails.cs: " + HttpContext.Current.Session["user_name"].ToString() + " did not allow to resend pager data (" + HttpContext.Current.Session["user_name"] + ")");
					m_SportsLog.Close();
				}	//no right to resend info
			} catch(Exception ex) {
				iRecUpd = -1;
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " ResendPagerDetails.cs.Resend(): " + ex.ToString());
				m_SportsLog.Close();
			}

			return iRecUpd;
		}
	}
}