/*
Objective:
Create command ini file to delete pager data

Last updated:
19 Feb 2004, Chapman Choi

C#.NET complier statement:
csc /t:library /out:..\bin\DeletePagerDetails.dll /r:..\bin\DBManager.dll;..\bin\Files.dll;..\bin\MessageClient.dll;..\bin\SportsMessage.dll DeletePagerDetails.cs
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
[assembly:AssemblyDescription("重發/刪除資訊 -> 刪除傳呼機資訊")]
[assembly:AssemblyProduct("Sports.NET")]
[assembly:AssemblyTitle("Sports.NET DLL")]
[assembly:AssemblyVersion("3.1.*")]
namespace SportsUtil {
	public class DeletePagerDetails {
		const string CMDTYPE1 = "CLR_TBL";
		const string CMDTYPE2 = "OTHODDS_DEL";
		const string LOGFILESUFFIX = "log";
		string m_Role;
		Files m_SportsLog;
		Encoding m_Big5Encoded;
		StringBuilder SQLString;
		OleDbDataReader m_SportsOleReader;
		DBManager m_SportsDBMgr;

		public DeletePagerDetails(string Connection) {
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
			string sSvcID = "";
			StringBuilder HTMLString = new StringBuilder();
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
						int iItemCount = 0;
						int iItemRow = 0;
						string sCategory = "";
						SQLString.Remove(0,SQLString.Length);
						SQLString.Append("select CCATEGORY, CPARAM, CCMD_TYPE, CDISPLAY from ADMINCFG where IADMIN_NO in (select IADMIN_NO from ADMINSVCMAP where ISVC_NO=");
						SQLString.Append(sSvcID);
						SQLString.Append(") AND (CCMD_TYPE='");
						SQLString.Append(CMDTYPE1);
						SQLString.Append("' OR CCMD_TYPE='");
						SQLString.Append(CMDTYPE2);
						SQLString.Append("') group by CCATEGORY, CDISPLAY, CPARAM, CCMD_TYPE order by CCATEGORY, IADMIN_NO");
						m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
						while(m_SportsOleReader.Read()) {
							if(!sCategory.Equals(m_SportsOleReader.GetString(0).Trim())) {
								sCategory = m_SportsOleReader.GetString(0).Trim();
								HTMLString.Append("<tr align=\"left\"><th colspan=\"5\" bgcolor=\"#FF7F50\">");
								HTMLString.Append(sSvcName);
								HTMLString.Append(" - ");
								HTMLString.Append(sCategory);
								HTMLString.Append("</th></tr>");
								iItemRow = 0;
							}
							if(iItemRow%5 == 0) {
								HTMLString.Append("<tr align=\"left\">");
							}
							HTMLString.Append("<td><input type=\"checkbox\" name=\"DeleteIndex\" value=\"");
							HTMLString.Append(iItemCount.ToString());
							HTMLString.Append("\"><input type=\"hidden\" name=\"param\" value=\"");
							HTMLString.Append(m_SportsOleReader.GetString(1).Trim());
							HTMLString.Append("\"><input type=\"hidden\" name=\"cmdtype\" value=\"");
							HTMLString.Append(m_SportsOleReader.GetString(2).Trim());
							HTMLString.Append("\">");
							HTMLString.Append(m_SportsOleReader.GetString(3).Trim());
							HTMLString.Append("</td>");
							if(iItemRow%5 == 4) {
								HTMLString.Append("</tr>");
							}
							iItemRow++;
							iItemCount++;
						}
						m_SportsOleReader.Close();
						m_SportsDBMgr.Close();
						m_SportsDBMgr.Dispose();
						if(HTMLString.Length == 0) {
							HTMLString.Append("<tr><th bgcolor=\"#FF7F50\">不適用</th></tr>");
						}
						HTMLString.Append("<input type=\"hidden\" name=\"svcID\" value=\"");
						HTMLString.Append(sSvcID);
						HTMLString.Append("\">");
					} else {
						HTMLString.Append("<tr><th bgcolor=\"#FF7F50\"沒有權限</th></tr>");
					}
				} else {
					HTMLString.Append("<tr><th bgcolor=\"#FF7F50\"沒有權限</th></tr>");
				}
			} catch(Exception ex) {
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " DeletePagerDetails.cs.ShowItems(): " + ex.ToString());
				m_SportsLog.Close();
				HTMLString.Remove(0,HTMLString.Length);
				HTMLString.Append((string)HttpContext.Current.Application["accessErrorMsg"]);
			}

			return HTMLString.ToString();
		}

		public int Delete() {
			int iDeleteItems;
			int iDeleted = 0;
			int iDelIdx = 0;
			string sSvcID = "";
			string sCurrentTimestamp = null;
			string sBatchJob = null;
			string[] arrRemotingPath;
			sSvcID = HttpContext.Current.Request.Form["svcID"].Trim();

			try {
				if(Convert.ToInt32(m_Role) > 10) {
					/*****************************
					 * GoGo Pager2 alert message *
					 *****************************/
					string[] arrQueueNames;
					string[] arrMessageTypes;
					arrQueueNames = (string[])HttpContext.Current.Application["QueueItems"];
					arrMessageTypes = (string[])HttpContext.Current.Application["NotifyMessageTypes"];
					arrRemotingPath = (string[])HttpContext.Current.Application["RemotingItems"];
					MessageClient msgClt = new MessageClient();
					msgClt.MessageType = arrMessageTypes[0];
					msgClt.MessagePath = arrQueueNames[0];
					//SportsMessage object message
					SportsMessage sptMsg = new SportsMessage();

					char[] delimiter = new char[] {','};
					string[] arrDeleteItems;
					string[] arrMsgType;
					string[] arrCmdType;
					string[] arrParam;
					arrMsgType = (string[])HttpContext.Current.Application["messageType"];
					try {
						arrDeleteItems = (string[])HttpContext.Current.Request.Form["DeleteIndex"].Split(delimiter);
						iDeleteItems = arrDeleteItems.Length;
					} catch(Exception) {
						arrDeleteItems = new string[0];
						iDeleteItems = 0;
					}
					try {
						arrParam = (string[])HttpContext.Current.Request.Form["param"].Split(delimiter);
						arrCmdType = (string[])HttpContext.Current.Request.Form["cmdtype"].Split(delimiter);
					} catch(Exception) {
						arrParam = new string[0];
						arrCmdType = new string[0];
					}

					//Double check for user right
					SQLString.Remove(0,SQLString.Length);
					SQLString.Append("select CACCESS_RIGHT from SVCCFG where ISVC_NO=");
					SQLString.Append(sSvcID);
					SQLString.Append(" and DEL_TYPE='P'");
					string sSvcRight = m_SportsDBMgr.ExecuteQueryString(SQLString.ToString());
					m_SportsDBMgr.Close();

					if(Convert.ToInt32(m_Role) >= Convert.ToInt32(sSvcRight)) {
						if(iDeleteItems > 0) {	//resend item(s)
							for(iDeleted = 0; iDeleted < iDeleteItems; iDeleted++) {
								iDelIdx = Convert.ToInt32(arrDeleteItems[iDeleted]);
								sCurrentTimestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
								sBatchJob = DateTime.Now.ToString("yyMMddHHmmss_fffffff") + "." + HttpContext.Current.Session["user_name"].ToString() + "." + arrMsgType[13] + "." + iDeleted.ToString() + ".ini";
								SQLString.Remove(0,SQLString.Length);
								SQLString.Append("insert into LOG_ADMINTASK (TIMEFLAG, SECTION, CMD, PARAM, BATCHJOB) values ('");
								SQLString.Append(sCurrentTimestamp);
								SQLString.Append("','COMMAND_','");
								SQLString.Append(arrCmdType[iDelIdx]);
								SQLString.Append("','");
								SQLString.Append(arrParam[iDelIdx]);
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
										m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " MSMQ ERROR: Delete Pager Details");
										m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " DeletePagerDetails.cs.Delete(): Notify via MSMQ throws MessageQueueException:  " + mqEx.ToString());
										m_SportsLog.Close();
	
										//If MSMQ fail, notify via .NET Remoting
										msgClt.MessageType = arrMessageTypes[1];
										msgClt.MessagePath = arrRemotingPath[0];
										if(!msgClt.SendMessage((object)sptMsg)) {
											m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
											m_SportsLog.SetFileName(0,LOGFILESUFFIX);
											m_SportsLog.Open();
											m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Remoting ERROR: Delete Pager Details");
											m_SportsLog.Close();
										}
									}	catch(Exception ex) {
										m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
										m_SportsLog.SetFileName(0,LOGFILESUFFIX);
										m_SportsLog.Open();
										m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Remoting ERROR: Delete Pager Details");
										m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " DeletePagerDetails.cs.Delete(): Notify via .NET Remoting throws exception: " + ex.ToString());
										m_SportsLog.Close();
									}							
								}	catch(Exception ex) {
									m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
									m_SportsLog.SetFileName(0,LOGFILESUFFIX);
									m_SportsLog.Open();
									m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " DeletePagerDetails.cs.Delete(): Notify via MSMQ throws exception: " + ex.ToString());
									m_SportsLog.Close();
								}
								System.Threading.Thread.Sleep(1500);
							}
						}

						//write log
						m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
						m_SportsLog.SetFileName(0,LOGFILESUFFIX);
						m_SportsLog.Open();
						m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " DeletePagerDetails.cs: delete " + iDeleted.ToString() + " items (" + HttpContext.Current.Session["user_name"] + ")");
						m_SportsLog.Close();
					} else {
						iDeleted = -99;
						m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
						m_SportsLog.SetFileName(0,LOGFILESUFFIX);
						m_SportsLog.Open();
						m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " DeletePagerDetails.cs: " + HttpContext.Current.Session["user_name"].ToString() + " did not allow to resend pager data (" + HttpContext.Current.Session["user_name"] + ")");
						m_SportsLog.Close();
					}	//no right to resend info
				} else {
					iDeleted = -99;
					m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
					m_SportsLog.SetFileName(0,LOGFILESUFFIX);
					m_SportsLog.Open();
					m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " DeletePagerDetails.cs: " + HttpContext.Current.Session["user_name"].ToString() + " did not allow to resend pager data (" + HttpContext.Current.Session["user_name"] + ")");
					m_SportsLog.Close();
				}	//no right to resend info
			} catch(Exception ex) {
				iDeleted = -1;
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " DeletePagerDetails.cs.Delete(): " + ex.ToString());
				m_SportsLog.Close();
			}

			return iDeleted;
		}
	}
}