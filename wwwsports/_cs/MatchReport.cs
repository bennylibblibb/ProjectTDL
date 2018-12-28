/*
Objective:
Modify and send selected match recent report

Last updated:
09 Feb 2004 by HenryCi

C#.NET complier statement:
csc /t:library /out:..\bin\MatchReport.dll /r:..\bin\DBManager.dll;..\bin\Files.dll;..\bin\MessageClient.dll;..\bin\SportsMessage.dll MatchReport.cs
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
[assembly:AssemblyDescription("足球資訊 -> 賽後報告(修改及發送)")]
[assembly:AssemblyProduct("Sports.NET")]
[assembly:AssemblyTitle("Sports.NET DLL")]
[assembly:AssemblyVersion("3.0.*")]
namespace SportsUtil {
	public class MatchReport {		
		const string LOGFILESUFFIX = "log";
		const string DBCR = "(CR)";
		const string PAGECR = "\r\n";
		OleDbDataReader m_SportsOleReader;		
		OleDbDataReader m_SoccerOleReader;
		DBManager m_SportsDBMgr;
		DBManager m_SoccerDBMgr;
		Files m_SportsLog;		
		StringBuilder SQLString;		

		public MatchReport(string GogoConnection, string SoccerConnection) {
			m_SportsDBMgr = new DBManager();
			m_SportsDBMgr.ConnectionString = GogoConnection;
			m_SoccerDBMgr = new DBManager();
			m_SoccerDBMgr.ConnectionString = SoccerConnection;			
			m_SportsLog = new Files();			
			SQLString = new StringBuilder();						
		}

		public string GetRecent() {
			string sirecID = "";
			string sLeague = "";
			string sHost = "";
			string sGuest = "";
			string sMatchField = "";
			string sMatchFieldCh = "";
			string sHostScore = "";
			string sGuestScore = "";
			string sMatchDate = "";		
						
			StringBuilder HTMLString = new StringBuilder();					
			sirecID = HttpContext.Current.Request.QueryString["irecID"];					

			try {
				SQLString.Remove(0,SQLString.Length);							
				SQLString.Append("select master.cleague, master.chost, master.cguest, goal.cmatch_field, goal.ihost_score, goal.iguest_score, master.imatchdatetime from hk_sports_master master, hk_goalmenu goal where master.irec_id = goal.irec_id and master.irec_id=");										
				SQLString.Append(sirecID);				
				
				m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());					
				
				if(m_SportsOleReader.Read()) {					
					sLeague = m_SportsOleReader.GetString(0).Trim();
					sHost  = m_SportsOleReader.GetString(1).Trim();
					sGuest = m_SportsOleReader.GetString(2).Trim();
					sMatchField = m_SportsOleReader.GetString(3).Trim();
					sHostScore = m_SportsOleReader.GetInt32(4).ToString();
					sGuestScore = m_SportsOleReader.GetInt32(5).ToString();										
					sMatchDate = m_SportsOleReader.GetDateTime(6).ToString("dd/MM/yyyy HH:mm");																		
				
					if (sMatchField.Equals("H"))
						sMatchFieldCh = "(主)";					
					else if (sMatchField.Equals("M"))
						sMatchFieldCh = "(中)";										
					
					HTMLString.Append("<tr style=\"background-color:#6699FF\"><th width=\"30%\"><select name=\"Action\"><option value=\"U\">更新<option value=\"D\">刪除&nbsp;<input type=\"hidden\" name=\"league\" value=\"");
					HTMLString.Append(sLeague);
					HTMLString.Append("\">");		
					HTMLString.Append(sMatchDate);				
					HTMLString.Append("&nbsp;");		
					HTMLString.Append("<input type=\"hidden\" name=\"matchdate\" value=\"");				
					HTMLString.Append(m_SportsOleReader.GetDateTime(6).ToString("yyyyMMdd"));	
					HTMLString.Append("\">");
					HTMLString.Append("<input type=\"hidden\" name=\"matchtime\" value=\"");				
					HTMLString.Append(m_SportsOleReader.GetDateTime(6).ToString("HHmm"));	
					HTMLString.Append("\">");
					HTMLString.Append(sLeague);
					HTMLString.Append("</th><th><input type=\"hidden\" name=\"host\" value=\"");
					HTMLString.Append(sHost);
					HTMLString.Append("\">");
					HTMLString.Append(sHost);			
					HTMLString.Append("<input type=\"hidden\" name=\"matchfield\" value=\"");	
					HTMLString.Append(sMatchField);				
					HTMLString.Append("\">");
					HTMLString.Append(sMatchFieldCh);				
					HTMLString.Append(" vs <input type=\"hidden\" name=\"guest\" value=\"");
					HTMLString.Append(sGuest);
					HTMLString.Append("\">");
					HTMLString.Append(sGuest);
					HTMLString.Append("<input type=\"hidden\" name=\"irec_no\" value=\"");
					HTMLString.Append(sirecID);
					HTMLString.Append("\"></th></tr>");
					HTMLString.Append("<tr style=\"background-color:#99CCFF\"><th width=\"30%\">賽果</th>");
					HTMLString.Append("<th><input type=\"hidden\" name=\"hostscore\" value=\"");
					HTMLString.Append(sHostScore);
					HTMLString.Append("\">");
					HTMLString.Append(sHostScore);
					HTMLString.Append(" : <input type=\"hidden\" name=\"guestscore\" value=\"");
					HTMLString.Append(sGuestScore);
					HTMLString.Append("\">");
					HTMLString.Append(sGuestScore);				
					HTMLString.Append("</th></tr>");	
				}
				m_SportsOleReader.Close();
				m_SportsDBMgr.Close();				

				SQLString.Remove(0,SQLString.Length);							
				SQLString.Append("select cmatch_venue, iattention, cattention_rate, creport from report_info where cact='U' and irec_no=");										
				SQLString.Append(sirecID);					
				m_SoccerOleReader = m_SoccerDBMgr.ExecuteQuery(SQLString.ToString());								
											
				if(m_SoccerOleReader.Read()) {				
						
					//Match Venue
					HTMLString.Append("<tr><th width=\"30%\">球場名稱</th>");
					HTMLString.Append("<th><input type=\"text\" name=\"matchvenue\" length=20  maxlength=10 value=\"");
					if(!m_SoccerOleReader.IsDBNull(0)) {
						if(!m_SoccerOleReader.GetString(0).Trim().Equals("-1")) {
							HTMLString.Append(m_SoccerOleReader.GetString(0).Trim());
						}
					}
					HTMLString.Append("\" onChange=\"checkMatchVenueLength()\">");
					HTMLString.Append("</th></tr>");	
					
					//Attention
					HTMLString.Append("<tr><th width=\"30%\">入座人數</th>");
					HTMLString.Append("<th><input type=\"text\" name=\"attention\" length=20 maxlength=6 value=\"");				
					if(!m_SoccerOleReader.IsDBNull(1)) {
						if(!m_SoccerOleReader.GetInt32(1).ToString().Equals("-1")) {
							HTMLString.Append(m_SoccerOleReader.GetInt32(1).ToString());
						}
					}
					HTMLString.Append("\" onChange=\"checkAttentionFormat()\">");
					HTMLString.Append("</th></tr>");	
					
					//Attention Rate
					HTMLString.Append("<tr><th width=\"30%\">入座率</th>");
					HTMLString.Append("<th><input type=\"text\" name=\"attentionrate\" length=4 maxlength=4 value=\"");
					if(!m_SoccerOleReader.IsDBNull(2)) {
						if(!m_SoccerOleReader.GetString(2).Trim().Equals("-1")) {
							HTMLString.Append(m_SoccerOleReader.GetString(2).Trim());
						}
					}
					HTMLString.Append("\" onChange=\"checkAttentionRateFormat()\">");
					HTMLString.Append("</th></tr>");	
					
					//Report
					HTMLString.Append("<tr><th width=\"30%\">報告詳情<br>(不能多於400個位元)</th>");
					HTMLString.Append("<th><textarea name=\"report\" rows=12 cols=30 onChange=\"checkReportByte()\">");
					if(!m_SoccerOleReader.IsDBNull(3)) {
						if(!m_SoccerOleReader.GetString(3).Trim().Equals("-1")) {							
							HTMLString.Append(m_SoccerOleReader.GetString(3).Trim().Replace(DBCR,PAGECR));
						}
					}
					HTMLString.Append("</textarea>");		
					HTMLString.Append("</th></tr>");					
									
				}else {
										
					HTMLString.Append("<tr><th width=\"30%\">球場名稱</th>");
					HTMLString.Append("<th><input type=\"text\" name=\"matchvenue\" length=20  maxlength=10 onChange=\"checkMatchVenueLength()\">");
					HTMLString.Append("</th></tr>");	
					HTMLString.Append("<tr><th width=\"30%\">入座人數</th>");
					HTMLString.Append("<th><input type=\"text\" name=\"attention\" length=20 maxlength=6 onChange=\"checkAttentionFormat()\">");				
					HTMLString.Append("</th></tr>");	
					HTMLString.Append("<tr><th width=\"30%\">入座率</th>");
					HTMLString.Append("<th><input type=\"text\" name=\"attentionrate\" length=4 maxlength=4 onChange=\"checkAttentionRateFormat()\">");
					HTMLString.Append("</th></tr>");
					HTMLString.Append("<tr><th width=\"30%\">報告詳情<br>(不能多於400個位元) </th>");
					HTMLString.Append("<th><textarea name=\"report\" rows=12 cols=30 onChange=\"checkReportByte()\"></textarea>");					
					HTMLString.Append("</th></tr>");									
					
				}						
				
				m_SoccerOleReader.Close();
				m_SoccerDBMgr.Close();	
						
							
			}catch(Exception ex){								
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " MatchReport.cs.GetRecent(): " + ex.ToString());
				m_SportsLog.Close();
				HTMLString.Remove(0,HTMLString.Length);
				HTMLString.Append((string)HttpContext.Current.Application["accessErrorMsg"]);				
			}
			
			return HTMLString.ToString();
		}

		public int Update() {
			int iRecNo = 0;	
			int iSubItemIdx = 1;
			string sRecNo = "";			
			string sLeague = "";
			string sHost = "";
			string sGuest = "";			
			string sAction;
			string sMatchDate = "";
			string sMatchTime = "";
			string sMatchField = "";
			string sMatchVenue = "";
			string sAttention = "";
			string sAttentionRate = "";
			string sReport = "";			
			string sCurrentTimestamp = null;
			string sBatchJob = null;
			string[] arrMsgType;
			string[] arrSendToPager;
			string[] arrRemotingPath;			
			bool bSendToPager = false;			
			char[] delimiter = new char[] {','};
			
			
			//SportsMessage object message
			SportsMessage sptMsg = new SportsMessage();
			StringBuilder LogSQLString = new StringBuilder();			
			
			arrRemotingPath = (string[])HttpContext.Current.Application["RemotingItems"];
		
			sRecNo = HttpContext.Current.Request.Form["irec_no"];
			sAction = HttpContext.Current.Request.Form["Action"];		
			sLeague = HttpContext.Current.Request.Form["league"];
			sHost = HttpContext.Current.Request.Form["host"];
			sGuest = HttpContext.Current.Request.Form["guest"];	
			sMatchDate = HttpContext.Current.Request.Form["matchdate"];			
			sMatchField = HttpContext.Current.Request.Form["matchfield"];		
			sMatchTime = HttpContext.Current.Request.Form["matchtime"];		
			sMatchVenue = HttpContext.Current.Request.Form["matchvenue"];			
			sAttention = HttpContext.Current.Request.Form["attention"];			
			sAttentionRate = HttpContext.Current.Request.Form["attentionrate"];
			sReport = HttpContext.Current.Request.Form["report"];								
										
			try {
				arrSendToPager = HttpContext.Current.Request.Form["SendToPager"].Split(delimiter);
			} catch(Exception) {
				arrSendToPager = new string[0];
			}
			if(arrSendToPager.Length>0) bSendToPager = true;
			arrMsgType = (string[])HttpContext.Current.Application["messageType"];			
			sCurrentTimestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
			sBatchJob = DateTime.Now.ToString("yyMMddHHmmss_fffffff") + "." + HttpContext.Current.Session["user_name"].ToString() + "." + arrMsgType[36] + ".ini";

			try {
				if(sAction.Equals("U")) {					
					//clear existing records first					
					SQLString.Remove(0,SQLString.Length);
					SQLString.Append("delete from report_info where irec_no=");
					SQLString.Append(sRecNo);									
					m_SoccerDBMgr.ExecuteNonQuery(SQLString.ToString());						
					m_SoccerDBMgr.Close();					

					if(sMatchVenue.Trim().Equals("")) sMatchVenue="-1";
					if(sAttention.Trim().Equals("")) sAttention="-1";
					if(sAttentionRate.Trim().Equals("")) sAttentionRate="-1";														
					if(sReport.Trim().Equals("")) sReport="-1";					

					/**************************
					 * Update for Report_info *
					 **************************/					 
					SQLString.Remove(0,SQLString.Length);
					SQLString.Append("insert into report_info values(");					
					SQLString.Append(sRecNo);					
					SQLString.Append(",'");
					SQLString.Append(sLeague);
					SQLString.Append("','");
					SQLString.Append(sHost);
					SQLString.Append("','");
					SQLString.Append(sGuest);
					SQLString.Append("','");
					SQLString.Append(sAction);
					SQLString.Append("','");
					SQLString.Append(sMatchVenue);
					SQLString.Append("',");
					SQLString.Append(sAttention); 
					SQLString.Append(",");
					SQLString.Append(sAttentionRate); 
					SQLString.Append(",'");
					SQLString.Append(sReport.Trim().Replace(PAGECR,DBCR));
					SQLString.Append("')");
					m_SoccerDBMgr.ExecuteNonQuery(SQLString.ToString());		
					m_SoccerDBMgr.Close();						


					/**************************
					* Update for Log_report *
					**************************/	 				 					
					LogSQLString.Remove(0,LogSQLString.Length);
					LogSQLString.Append("insert into LOG_REPORT (TIMEFLAG, IITEMSEQ_NO, SECTION, ACT, LEAGUE, HOST, GUEST, MATCHDATE, MATCHTIME, MATCHFIELD, FIXTURE, ATTENTION, ATTENTION_RATE, COMMENT, BATCHJOB) values ('");					
					LogSQLString.Append(sCurrentTimestamp);
					LogSQLString.Append("',");
					LogSQLString.Append(iSubItemIdx.ToString());
					LogSQLString.Append(",'REPORT_','");
					LogSQLString.Append(sAction);
					LogSQLString.Append("','");
					LogSQLString.Append(sLeague);
					LogSQLString.Append("','");
					LogSQLString.Append(sHost);
					LogSQLString.Append("','");
					LogSQLString.Append(sGuest);
					LogSQLString.Append("','");
					LogSQLString.Append(sMatchDate);
					LogSQLString.Append("','");
					LogSQLString.Append(sMatchTime);
					LogSQLString.Append("','");
					LogSQLString.Append(sMatchField);
					LogSQLString.Append("','");
					LogSQLString.Append(sMatchVenue);
					LogSQLString.Append("',");
					LogSQLString.Append(sAttention);
					LogSQLString.Append(",");
					LogSQLString.Append(sAttentionRate);
					LogSQLString.Append(",'");
					LogSQLString.Append(sReport.Trim().Replace(PAGECR,DBCR));
					LogSQLString.Append("','");
					LogSQLString.Append(sBatchJob);
					LogSQLString.Append("')");											

					if(bSendToPager) {
						m_SoccerDBMgr.ExecuteNonQuery(LogSQLString.ToString());				
						m_SoccerDBMgr.Close();			
					}								

					//write log										
					m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
					m_SportsLog.SetFileName(0,LOGFILESUFFIX);
					m_SportsLog.Open();
					m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " MatchReport.cs: update report info <L:" + sLeague + ", H:" + sHost + ", G:" + sGuest + "> (" + HttpContext.Current.Session["user_name"] + ")");					
					m_SportsLog.Close();
				} else { //Action.Equals("D") 
					SQLString.Remove(0,SQLString.Length);
					SQLString.Append("update report_info set CACT='D' where irec_no=");					
					SQLString.Append(sRecNo);										
					m_SoccerDBMgr.ExecuteNonQuery(SQLString.ToString());	
					m_SoccerDBMgr.Close();					

					LogSQLString.Remove(0,LogSQLString.Length);
					LogSQLString.Append("insert into LOG_REPORT (TIMEFLAG, IITEMSEQ_NO, SECTION, ACT, LEAGUE, HOST, GUEST, MATCHDATE, MATCHTIME, MATCHFIELD, FIXTURE, ATTENTION, ATTENTION_RATE, COMMENT, BATCHJOB) values ('");					
					LogSQLString.Append(sCurrentTimestamp);
					LogSQLString.Append("',");
					LogSQLString.Append(iSubItemIdx.ToString());
					LogSQLString.Append(",'REPORT_','");
					LogSQLString.Append(sAction);
					LogSQLString.Append("','");
					LogSQLString.Append(sLeague);
					LogSQLString.Append("','");
					LogSQLString.Append(sHost);
					LogSQLString.Append("','");
					LogSQLString.Append(sGuest);
					LogSQLString.Append("', null, null, null, null, null, null, null,'");					
					LogSQLString.Append(sBatchJob); 
					LogSQLString.Append("')");	

					if(bSendToPager) {
						m_SoccerDBMgr.ExecuteNonQuery(LogSQLString.ToString());				
						m_SoccerDBMgr.Close();										
					}

					//write log							
					m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
					m_SportsLog.SetFileName(0,LOGFILESUFFIX);
					m_SportsLog.Open();
					m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " MatchReport.cs: delete record <L:" + sLeague + ", H:" + sHost + ", G:" + sGuest + "> (" + HttpContext.Current.Session["user_name"] + ")");				
					m_SportsLog.Close();
				}				

				//send message to pager						
				if(bSendToPager) {
					string[] arrQueueNames;
					string[] arrMessageTypes;
					arrQueueNames = (string[])HttpContext.Current.Application["QueueItems"];
					arrMessageTypes = (string[])HttpContext.Current.Application["NotifyMessageTypes"];
					MessageClient msgClt = new MessageClient();
					msgClt.MessageType = arrMessageTypes[0];
					msgClt.MessagePath = arrQueueNames[0];

					//Send Notify Message
					//modified by Henry, 09 Feb 2004 begin
					sptMsg.IsTransaction = true;
					sptMsg.Body = sBatchJob;
					sptMsg.Timestamp = DateTime.Now.ToString("yyMMddHHmmss");
					sptMsg.AppID = "07";
					sptMsg.MsgID = "32";
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
							m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " MSMQ ERROR: Match Report");
							m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " MatchReport.cs.Update(): Notify via MSMQ throws MessageQueueException:  " + mqEx.ToString());
							m_SportsLog.Close();

							//If MSMQ fail, notify via .NET Remoting
							msgClt.MessageType = arrMessageTypes[1];
							msgClt.MessagePath = arrRemotingPath[0];
							if(!msgClt.SendMessage((object)sptMsg)) {
								m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
								m_SportsLog.SetFileName(0,LOGFILESUFFIX);
								m_SportsLog.Open();
								m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Remoting ERROR: Match Report");
								m_SportsLog.Close();
							}
						}	catch(Exception ex) {
							m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
							m_SportsLog.SetFileName(0,LOGFILESUFFIX);
							m_SportsLog.Open();
							m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Remoting ERROR: Match Report");
							m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " MatchReport.cs.Update(): Notify via .NET Remoting throws exception: " + ex.ToString());
							m_SportsLog.Close();
						}							
					}	catch(Exception ex) {
						m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
						m_SportsLog.SetFileName(0,LOGFILESUFFIX);
						m_SportsLog.Open();
						m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " MatchReport.cs.Update(): Notify via MSMQ throws exception: " + ex.ToString());
						m_SportsLog.Close();
					}			
				}				
				//end modify
			}	catch(Exception ex) {
				iRecNo = -1;				
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + "MatchReport.cs.Update(): " + ex.ToString());
				m_SportsLog.Close();
			}

			return iRecNo;
		}
	}
}