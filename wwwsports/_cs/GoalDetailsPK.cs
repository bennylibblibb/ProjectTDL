/*
Objective:
Update and send the PK result

Remarks:
Use of FLAG in RESULTINFO so that to indicate which team start PK first
0: Host first
1: Guest first

Last updated:
09 Feb 2004 by HenryCi

C#.NET complier statement:
csc /t:library /out:..\bin\GoalDetailsPK.dll /r:..\bin\DBManager.dll;..\bin\Files.dll;..\bin\MessageClient.dll;..\bin\SportsMessage.dll GoalDetailsPK.cs
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
[assembly:AssemblyDescription("足球資訊 -> 比數詳情(十二碼)")]
[assembly:AssemblyProduct("Sports.NET")]
[assembly:AssemblyTitle("Sports.NET DLL")]
[assembly:AssemblyVersion("3.1.*")]
namespace SportsUtil {
	public class GoalDetailsPK {
		const string LOGFILESUFFIX = "log";
		OleDbDataReader m_SportsOleReader;
		DBManager m_SportsDBMgr;
		Files m_SportsLog;
		Encoding m_Big5Encoded;
		StringBuilder SQLString;

		public GoalDetailsPK(string Connection) {
			m_SportsDBMgr = new DBManager();
			m_SportsDBMgr.ConnectionString = Connection;
			m_SportsLog = new Files();
			m_Big5Encoded = Encoding.GetEncoding(950);
			SQLString = new StringBuilder();
		}

		public string ShowPK() {
			const int TOTALPKCOUNT = 20;
			int iresult;
			string sMatchCnt;
			string sHost = "";
			string sGuest = "";
			string sHostPK = "";
			string sGuestPK = "";
			string sStartFlag = "0";
			StringBuilder HTMLString = new StringBuilder();

			sMatchCnt = HttpContext.Current.Request.QueryString["matchcnt"];
			if(sMatchCnt == null) sMatchCnt= "0";

			try {
				SQLString.Remove(0,SQLString.Length);
				SQLString.Append("select host, guest from gameinfo, resultinfo where gameinfo.match_cnt=");
				SQLString.Append(sMatchCnt);
				m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
				if(m_SportsOleReader.Read()) {
					sHost = m_SportsOleReader.GetString(0).Trim();
					sGuest = m_SportsOleReader.GetString(1).Trim();
				}
				m_SportsDBMgr.Close();
				m_SportsOleReader.Close();

				SQLString.Remove(0,SQLString.Length);
				SQLString.Append("select count(match_cnt) from resultinfo where  match_cnt=");
				SQLString.Append(sMatchCnt);
				SQLString.Append(" and act='U'");
				iresult = m_SportsDBMgr.ExecuteScalar(SQLString.ToString());
				m_SportsDBMgr.Close();

				SQLString.Remove(0,SQLString.Length);
				SQLString.Append("select Hostpk, Guestpk, FLAG from resultinfo where match_cnt=");
				SQLString.Append(sMatchCnt);
				SQLString.Append("and act='U'");
				m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
				if(m_SportsOleReader.Read()) {
		    	sHostPK = m_SportsOleReader.GetString(0).Trim();
			    sGuestPK = m_SportsOleReader.GetString(1).Trim();

			    /*******************************
					* GoGo Pager2 							   *
					* Indicate team start PK first *
				  ********************************/
			    sStartFlag = m_SportsOleReader.GetString(2).Trim();
		    }
	  	  	m_SportsDBMgr.Close();
	    	m_SportsOleReader.Close();
		    m_SportsDBMgr.Dispose();

		    /********************************
				 * GoGo Pager2 							    *
				 * Indicate team start PK first *
			   ********************************/
			  HTMLString.Append("<tr style=\"background-color:#00bfff\" align=\"left\"><th colspan=\"21\"><input type=\"checkbox\" name=\"HostFirst\" value=\"0\" ");
			  if(sStartFlag.Equals("0")) HTMLString.Append("checked");
			  HTMLString.Append(">主隊先射</th></tr>");


				HTMLString.Append("<tr style=\"background-color:#00bfff\" align=\"center\">");
				HTMLString.Append("<th><font color=navy>12碼</font></th>");
				for(int j=0;j<TOTALPKCOUNT;j++) {
					HTMLString.Append("<th>");
					HTMLString.Append((j+1).ToString());
					HTMLString.Append("</th>");
				}
				HTMLString.Append("</tr>\n");

				HTMLString.Append("<tr><th>");
				HTMLString.Append(sHost);
				HTMLString.Append("</th>");
				if(iresult>0) {
					if(sHostPK.Length != 0) {
						for(int j=0;j<sHostPK.Length;j++) {
							HTMLString.Append("<td><INPUT NAME=\"hostpk\" SIZE=1 MAXLENGTH=1 value=\"");
							HTMLString.Append(sHostPK.Substring(j,1));
							HTMLString.Append("\" onChange=\"return numbercheck(");
							HTMLString.Append(j.ToString());
							HTMLString.Append(")\"></td>\n");
						}
						for(int j=sHostPK.Length;j<TOTALPKCOUNT;j++) {
							HTMLString.Append("<td><INPUT NAME=\"hostpk\" SIZE=1 MAXLENGTH=1 vaule=\"\" onChange=\" return numbercheck(");
							HTMLString.Append(j.ToString());
							HTMLString.Append(")\"></td>\n");
						}
					}	else {
						for(int j=0;j<TOTALPKCOUNT;j++) {
							HTMLString.Append("<td><INPUT NAME=\"hostpk\" SIZE=1 MAXLENGTH=1 value=\"\" onChange=\"return numbercheck(");
							HTMLString.Append(j.ToString());
							HTMLString.Append(")\"></td>\n");
						}
					}
				}	else {
					for(int j=0;j<TOTALPKCOUNT;j++) {
						HTMLString.Append("<td><INPUT NAME=\"hostpk\" SIZE=1 MAXLENGTH=1 vaule=\"\" onChange=\"return numbercheck(");
						HTMLString.Append(j.ToString());
						HTMLString.Append(")\"></td>\n");
					}
				}
				HTMLString.Append("</tr>\n");
				HTMLString.Append("<tr><th>");
				HTMLString.Append(sGuest);
				HTMLString.Append("</th>\n");
				if((iresult>0)&&(sGuestPK.Length!=0)) {
					for(int j=0;j<sGuestPK.Length;j++) {
						HTMLString.Append("<td><INPUT NAME=\"guestpk\" SIZE=1 MAXLENGTH=1 value=\"");
						HTMLString.Append(sGuestPK.Substring(j,1));
						HTMLString.Append("\" onChange=\"return  numbercheck(");
						HTMLString.Append(j.ToString());
						HTMLString.Append(")\"></td>\n");
					}
					for(int j=sGuestPK.Length;j<TOTALPKCOUNT;j++) {
						HTMLString.Append("<td><INPUT NAME=\"guestpk\" SIZE=1 MAXLENGTH=1 value=\"\" onChange=\"return numbercheck(");
						HTMLString.Append(j.ToString());
						HTMLString.Append(")\"></td>\n");
					}
				} else {
					for(int j=0;j<TOTALPKCOUNT;j++) {
						HTMLString.Append("<td><INPUT NAME=\"guestpk\" SIZE=1 MAXLENGTH=1 value=\"\" onChange=\" return numbercheck(");
						HTMLString.Append(j.ToString());
						HTMLString.Append(")\"></td>\n");
					}
				}
				HTMLString.Append("</tr>\n");
				HTMLString.Append("<input type=\"hidden\" name=\"matchcnt\" value=\"");
				HTMLString.Append(sMatchCnt);
				HTMLString.Append("\"><input type=\"hidden\" name=\"Host\" value=\"");
				HTMLString.Append(sHost);
				HTMLString.Append("\"><input type=\"hidden\" name=\"Guest\" value=\"");
				HTMLString.Append(sGuest);
				HTMLString.Append("\"><title>");
				HTMLString.Append(sHost);
				HTMLString.Append(" vs ");
				HTMLString.Append(sGuest);
				HTMLString.Append("</title>");
			} catch(Exception ex) {
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " GoalDetailsPK.cs.ShowPK(): " + ex.ToString());
				m_SportsLog.Close();
				HTMLString.Remove(0,HTMLString.Length);
				HTMLString.Append((string)HttpContext.Current.Application["accessErrorMsg"]);
			}
			return HTMLString.ToString();   
		}

		public int UpdatePK() {
			char[] delimiter = new char[] {','};
			int iDetailsCount = 0;
			int iresultnumber = 0;
			string sHostFirst;
			string sHostpk = "", sGuestpk = "";
			string sCurrentTimestamp = null;
			string sBatchJob = null;
			string sMatchCount, sHostname, sGuestname, siniplayer = "";
			string[] arrHostpk, arrGuestpk, arrMsgType, arrAlert, arrLeague, arrAct, arrMatchState, arrFlag, arrPeriod, arrTime, arrHscr, arrGscr, arrPlayer, arrPlayerNo, arrHostPlayer, arrGuestPlayer;
			string[] arrSendToPager;

			//SportsMessage object message
			SportsMessage sptMsg = new SportsMessage();
			StringBuilder LogSQLString = new StringBuilder();
			DBManager logDBMgr = new DBManager();
			logDBMgr.ConnectionString = m_SportsDBMgr.ConnectionString;

			//Delcare variable used in message notify
			string[] arrQueueNames;
			string[] arrRemotingPath;
			string[] arrMessageTypes;
			arrQueueNames = (string[])HttpContext.Current.Application["QueueItems"];
			arrRemotingPath = (string[])HttpContext.Current.Application["RemotingItems"];
			arrMessageTypes = (string[])HttpContext.Current.Application["NotifyMessageTypes"];
			MessageClient msgClt = new MessageClient();


			try {
				arrSendToPager = HttpContext.Current.Request.Form["SendToPager"].Split(delimiter);
			} catch(Exception) {
				arrSendToPager = new string[0];
			}
			arrMsgType = (string[])HttpContext.Current.Application["messageType"]; 
			try {   //  得到hostpk 和 guestpk 的數組值
			   arrHostpk = HttpContext.Current.Request.Form["hostpk"].Split(delimiter);
				arrGuestpk = HttpContext.Current.Request.Form["guestpk"].Split(delimiter);
	    }	catch {
				arrHostpk = new string[0];
				arrGuestpk = new string[0];
			}
			for(int k=0;k<20;k++) {   //將數組合併成一個字符串
				if(arrHostpk[k]!="") sHostpk= sHostpk+ arrHostpk[k];
				if(arrGuestpk[k]!="") sGuestpk= sGuestpk+arrGuestpk[k];
			}
			sMatchCount = HttpContext.Current.Request.Form["matchcnt"];
			sHostname = HttpContext.Current.Request.Form["Host"];
			sGuestname = HttpContext.Current.Request.Form["Guest"];

			/*******************************
			 * GoGo Pager2 							   *
			 * Indicate team start PK first *
			 ********************************/
			sHostFirst = HttpContext.Current.Request.Form["HostFirst"];
			if(sHostFirst == null) sHostFirst = "1";
			if(!sHostFirst.Equals("0")) sHostFirst = "1";

			try {
				/*******************************
			  * GoGo Pager2 							   *
			  * Indicate team start PK first *
  		  ********************************/
  		  SQLString.Remove(0,SQLString.Length);
				SQLString.Append("update resultinfo set HOSTPK='");
				SQLString.Append(sHostpk);
				SQLString.Append("', GUESTPK='");
				SQLString.Append(sGuestpk);
				SQLString.Append("', FLAG='");
				SQLString.Append(sHostFirst);
				SQLString.Append("' where MATCH_CNT=");
				SQLString.Append(sMatchCount);
				SQLString.Append(" and act='U'");
				m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
				m_SportsDBMgr.Close();

				if(arrSendToPager.Length > 0) {
					//write  ini file
					SQLString.Remove(0,SQLString.Length);
					SQLString.Append("select count(match_cnt) from resultinfo where match_cnt=");
					SQLString.Append(sMatchCount);
					SQLString.Append(" and act='U'");
					iDetailsCount = m_SportsDBMgr.ExecuteScalar(SQLString.ToString());
					m_SportsDBMgr.Close();
					if(iDetailsCount > 0) {
						arrAlert = new string[iDetailsCount];
						arrAct = new string[iDetailsCount];
						arrLeague = new string[iDetailsCount];
						arrMatchState = new string[iDetailsCount];
						arrFlag = new string[iDetailsCount];
						arrPeriod = new string[iDetailsCount];
						arrTime = new string[iDetailsCount];
						arrHscr = new string[iDetailsCount];
						arrGscr = new string[iDetailsCount];
						arrPlayer = new string[iDetailsCount];
						arrPlayerNo = new string[iDetailsCount];
						arrHostPlayer = new string[iDetailsCount];
						arrGuestPlayer = new string[iDetailsCount];

						SQLString.Remove(0,SQLString.Length);
						SQLString.Append("select alert, league, ACT, matchstate, res_flag, res_period, res_time, h_scr, g_scr, host_player, guest_player, player, fgs_player_no from resultinfo where match_cnt=");
						SQLString.Append(sMatchCount);
						SQLString.Append(" and act='U' order by result_id");
						m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
						while(m_SportsOleReader.Read()) {
							arrAlert[iresultnumber] = m_SportsOleReader.GetString(0).Trim();
							arrLeague[iresultnumber] = m_SportsOleReader.GetString(1).Trim();
							arrAct[iresultnumber] = m_SportsOleReader.GetString(2).Trim();
							arrMatchState[iresultnumber] = m_SportsOleReader.GetString(3).Trim();
							arrFlag[iresultnumber] = m_SportsOleReader.GetString(4).Trim();
							arrPeriod[iresultnumber] = m_SportsOleReader.GetString(5).Trim();
							arrTime[iresultnumber] = m_SportsOleReader.GetString(6).Trim();
							arrHscr[iresultnumber] = m_SportsOleReader.GetString(7).Trim();
							arrGscr[iresultnumber] = m_SportsOleReader.GetString(8).Trim();					
							arrHostPlayer[iresultnumber] = m_SportsOleReader.GetString(9).Trim();
							arrGuestPlayer[iresultnumber] = m_SportsOleReader.GetString(10).Trim();
							arrPlayer[iresultnumber] = m_SportsOleReader.GetString(11).Trim();
							arrPlayerNo[iresultnumber] = m_SportsOleReader.GetString(12).Trim();
							iresultnumber++;
						}
						m_SportsDBMgr.Close();
			  	  m_SportsOleReader.Close();

	  		  	sCurrentTimestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
	  		  	sBatchJob = DateTime.Now.ToString("yyMMddHHmmss_fffffff") + "." + HttpContext.Current.Session["user_name"].ToString() + "." + arrMsgType[15] + ".ini";
						for(int i=0;i<iresultnumber;i++) {
							LogSQLString.Remove(0,LogSQLString.Length);
							LogSQLString.Append("insert into LOG_GOALDETAILS (TIMEFLAG, IITEMSEQ_NO, SECTION, LEAGUE, HOST, GUEST, ACT, ALERT, CURRENT_STATUS, PK_FIRST, SCORE_REDFLAG, SCORE_STATUS, SCORE_TIME, SCORE_HOST_GOAL, SCORE_GUEST_GOAL, SCORE_PLAYER, FGS_PLAYER_NO, SCORE_HOST_PK, SCORE_GUEST_PK, BATCHJOB) values ('");
							LogSQLString.Append(sCurrentTimestamp);
							LogSQLString.Append("',");
							LogSQLString.Append((i+1).ToString());
							LogSQLString.Append(",'RES_','");
							LogSQLString.Append(arrLeague[0]);
							LogSQLString.Append("','");
							LogSQLString.Append(sHostname);
							LogSQLString.Append("','");
							LogSQLString.Append(sGuestname);
							LogSQLString.Append("','");
							LogSQLString.Append(arrAct[0]);
							LogSQLString.Append("','1");
							//LogSQLString.Append(arrAlert[i]);
							LogSQLString.Append("','T','");
							if(sHostFirst.Equals("0")) LogSQLString.Append(sHostname);
							else LogSQLString.Append(sGuestname);
							LogSQLString.Append("','");
							LogSQLString.Append(arrFlag[i]);
							LogSQLString.Append("','");
							LogSQLString.Append(arrPeriod[i]);
							LogSQLString.Append("','");

							if(arrTime[i].Equals("")) {
								arrTime[i] = "0";
							}
							LogSQLString.Append(Convert.ToInt32(arrTime[i]).ToString("D3"));
							LogSQLString.Append("','");

							if(arrHscr[i].Equals("")) {
								arrHscr[i] = "0";
							}
							LogSQLString.Append(Convert.ToInt32(arrHscr[i]).ToString("D2"));
							LogSQLString.Append("','");

							if(arrGscr[i].Equals("")) {
								arrGscr[i] = "0";
							}
							LogSQLString.Append(Convert.ToInt32(arrGscr[i]).ToString("D2"));
							LogSQLString.Append("','");
							if((arrHostPlayer[i].Equals("")) && arrGuestPlayer[i].Equals("")) {
								siniplayer = "-1";
							} else {
								if(!arrHostPlayer[i].Equals("")) {
									if(!arrPlayer[i].Equals("")) siniplayer = arrHostPlayer[i] + " ("+arrPlayer[i]+")";
									else siniplayer = arrHostPlayer[i];
								} else {
									if(!arrPlayer[i].Equals("")) siniplayer = arrGuestPlayer[i] + " ("+arrPlayer[i]+")";
									else siniplayer = arrGuestPlayer[i];
								}
							}
							LogSQLString.Append(siniplayer);
							LogSQLString.Append("','");
							LogSQLString.Append(arrPlayerNo[i]);
							LogSQLString.Append("','");

							string sinihpk = "",sinigpk = "";
							if(sHostpk.Equals("")) sinihpk = "-1";
							else sinihpk = sHostpk;
							if(sGuestpk.Equals("")) sinigpk = "-1";
							else sinigpk = sGuestpk;
							LogSQLString.Append(sinihpk);
							LogSQLString.Append("','");
							LogSQLString.Append(sinigpk);
							LogSQLString.Append("','");
							LogSQLString.Append(sBatchJob);
							LogSQLString.Append("')");
							logDBMgr.ExecuteNonQuery(LogSQLString.ToString());
							logDBMgr.Close();
		  	  			}

		  	  			//Tell MessageDispatcher to generate Goal Details PK INI and notify other processes
						//Assign value to SportsMessage object
						//modified by Henry, 09 Feb 2004 begin
						sptMsg.IsTransaction = true;
  	  					sptMsg.Body = sBatchJob;
						sptMsg.Timestamp = DateTime.Now.ToString("yyMMddHHmmss");
						sptMsg.AppID = "07";
						sptMsg.MsgID = "16";
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
								m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " MSMQ ERROR: Goal Details PK");
								m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " GoalDetailsPK.cs.UpdatePK(): Notify via MSMQ throws MessageQueueException: " + mqEx.ToString());
								m_SportsLog.Close();

								//If MSMQ fail, notify via .NET Remoting
								msgClt.MessageType = arrMessageTypes[1];
								msgClt.MessagePath = arrRemotingPath[0];
								if(!msgClt.SendMessage((object)sptMsg)) {
									m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
									m_SportsLog.SetFileName(0,LOGFILESUFFIX);
									m_SportsLog.Open();
									m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Remoting ERROR: Goal Details PK");
									m_SportsLog.Close();
								}
							}	catch(Exception ex) {
								m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
								m_SportsLog.SetFileName(0,LOGFILESUFFIX);
								m_SportsLog.Open();
								m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Remoting ERROR: Goal Details PK");
								m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " GoalDetailsPK.cs.UpdatePK(): Notify via .NET Remoting throws exception: " + ex.ToString());
								m_SportsLog.Close();
							}
						} catch(Exception ex) {
							m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
							m_SportsLog.SetFileName(0,LOGFILESUFFIX);
							m_SportsLog.Open();
							m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " GoalDetailsPK.cs.UpdatePK(): Notify via MSMQ throws exception: " + ex.ToString());
							m_SportsLog.Close();
						}
					}
				} // end modify
				m_SportsDBMgr.Dispose();
			} catch(Exception ex) {
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " GoalDetailsPK.cs.UpdatePK(): " + ex.ToString());
				m_SportsLog.Close();
				iDetailsCount = -1;
			}

			//write log
			m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
			m_SportsLog.SetFileName(0,LOGFILESUFFIX);
			m_SportsLog.Open();
			m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " GoalDetailsPK.cs: Send " + iDetailsCount.ToString() + " PK details records (" + HttpContext.Current.Session["user_name"] + ")");
			m_SportsLog.Close();

			return iDetailsCount;
		}
	}
}