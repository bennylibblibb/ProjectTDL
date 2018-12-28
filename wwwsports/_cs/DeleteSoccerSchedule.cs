/*
Objective:
Delete soccer schedule

Last updated:
5 Apr 2005 by Paddy, try catch the case of get Null String of hostname/guestname
23 Mar 2004, Chapman Choi
23 Mar 2004 Remark Replicator code

C#.NET complier statement:
(With Replicator)
csc /t:library /out:..\bin\DeleteSoccerSchedule.dll /r:..\bin\DBManager.dll;..\bin\Files.dll;..\bin\Replicator.dll DeleteSoccerSchedule.cs
(Without Replicator-Current Production)
csc /t:library /out:..\bin\DeleteSoccerSchedule.dll /r:..\bin\DBManager.dll;..\bin\Files.dll DeleteSoccerSchedule.cs
*/

using System;
using System.Data.OleDb;
using System.Reflection;
using System.Web;
using TDL.DB;
using TDL.IO;
//using TDL.Util;

[assembly:AssemblyCompany("TDSL")]
[assembly:AssemblyCopyright("(c) 2003 TDSL. All rights reserved.")]
[assembly:AssemblyDescription("足球賽程 -> 刪除賽程")]
[assembly:AssemblyProduct("Sports.NET")]
[assembly:AssemblyTitle("Sports.NET DLL")]
[assembly:AssemblyVersion("1.1.*")]
namespace SportsUtil {
	public class DeleteSoccerSchedule {
		const string LOGFILESUFFIX = "log";
		char[] delimiter = new char[] {','};
		OleDbDataReader m_SportsOleReader;
		DBManager m_SportsDBMgr;
		Files m_SportsLog;

		public DeleteSoccerSchedule(string Connection) {
			m_SportsDBMgr = new DBManager();
			m_SportsDBMgr.ConnectionString = Connection;
			m_SportsLog = new Files();
		}

		public string getMatch() {
			int iDeletedCnt = 0;
			string sQuery;
			string sRtn = "";
			string[] arrDeleted;

			try {
				arrDeleted = HttpContext.Current.Request.Form["selectedDelete"].Split(delimiter);
				iDeletedCnt = arrDeleted.Length;
			} catch(Exception) {
				arrDeleted = new string[0];
			}

			if(iDeletedCnt > 0) {
				sQuery = "select MATCHDATETIME, ALIAS, CHOST_HANDI, CFIELD, (select teamname from teaminfo where team_id=sch.ihost_team_id) CHOST, (select teamname from teaminfo where team_id=sch.iguest_team_id) CGUEST, IMATCH_CNT from SOCCERSCHEDULE sch, leaginfo lg where IMATCH_CNT in (";
				for(int i = 0; i < iDeletedCnt; i++) {
					if(i == 0) sQuery += arrDeleted[i];
					else sQuery += "," + arrDeleted[i];
				}
				sQuery += ") and sch.cleag_id=lg.leag_id order by MATCHDATETIME, ALIAS";
				try {
					m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(sQuery);
					while(m_SportsOleReader.Read()) {
						//Match date time
						sRtn += "<tr><th style=\"background-color:#FFFF00\">" + m_SportsOleReader.GetDateTime(0).ToString("yyyy/MM/dd HH:mm") + " ";

						//Alias
						sRtn += m_SportsOleReader.GetString(1).Trim() + " - ";

						//String format as same as pager -> 賽事
						if(m_SportsOleReader.GetString(2).Trim().Equals("1")) {
							if(m_SportsOleReader.GetString(3).Trim().Equals("H")) {
								sRtn += "(主) ";
							} else {
								sRtn += "(中) ";
							}
							try{
								sRtn += m_SportsOleReader.GetString(4);
							}catch{
								sRtn += "NULL";
							}
							sRtn += " vs ";
							try{
								sRtn += m_SportsOleReader.GetString(5);
							}catch{
								sRtn += "NULL";
							}
						} else {
							if(m_SportsOleReader.GetString(3).Trim().Equals("H")) {
								sRtn += "(客) ";
							}else{
								sRtn += "(中) ";
							}
							try{
								sRtn += m_SportsOleReader.GetString(5);
							}catch{
								sRtn += "NULL";
							}
							sRtn += " vs ";
							try{
								sRtn += m_SportsOleReader.GetString(4);
							}catch{
								sRtn += "NULL";
							}
						}

						//MatchCount (Hidden Field)
						sRtn += "<input type=\"hidden\" name=\"MatchCount\" value=\"" + m_SportsOleReader.GetInt32(6).ToString() + "\">";
						sRtn += "</th></tr>";
					}
					m_SportsOleReader.Close();
					m_SportsDBMgr.Close();
					//m_SportsDBMgr.Dispose();
				} catch(Exception ex) {
					m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
					m_SportsLog.SetFileName(0,LOGFILESUFFIX);
					m_SportsLog.Open();
					m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " DeleteSoccerSchedule.cs.GetMatches(): " + ex.ToString());
					m_SportsLog.Close();
					sRtn = (string)HttpContext.Current.Application["accessErrorMsg"];
				}
			} else {
				sRtn = "<tr><th style=\"background-color:#FFFF00\">沒有選擇賽程作刪除!</th></tr>";
			}

			return sRtn;
		}

		public int Delete() {
			int iDeletedCnt = 0;
			string updateQuery;
			string[] arrDeleted;

			try {
				arrDeleted = HttpContext.Current.Request.Form["MatchCount"].Split(delimiter);
				iDeletedCnt = arrDeleted.Length;
			} catch(Exception) {
				arrDeleted = new string[0];
			}

			if(iDeletedCnt > 0) {
				try {
					//Replicator SptReplicator = new Replicator(HttpContext.Current.Application["RepDBConnectionString"].ToString());
					//SptReplicator.ApplicationType = 1;
					//SptReplicator.ErrorLogPath = HttpContext.Current.Application["ErrorFilePath"].ToString();

					updateQuery = "delete from SOCCERSCHEDULE where IMATCH_CNT in (";
					for(int i = 0; i < iDeletedCnt; i++) {
						if(i == 0) updateQuery += arrDeleted[i];
						else updateQuery += "," + arrDeleted[i];
					}
					updateQuery += ")";
					m_SportsDBMgr.ExecuteNonQuery(updateQuery);
					m_SportsDBMgr.Close();
					//SptReplicator.Replicate(updateQuery);

					//write log
					m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
					m_SportsLog.SetFileName(0,LOGFILESUFFIX);
					m_SportsLog.Open();
					m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " DeleteSoccerSchedule.cs: Delete " + iDeletedCnt.ToString() + " schedule (" + HttpContext.Current.Session["user_name"] + ")");
					m_SportsLog.Close();
					//m_SportsDBMgr.Dispose();
					//SptReplicator.Dispose();
					//iExisted++;
				} catch(OleDbException dbEx) {
					iDeletedCnt = -99;
					m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
					m_SportsLog.SetFileName(0,LOGFILESUFFIX);
					m_SportsLog.Open();
					m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " DeleteSoccerSchedule.cs.Modify(): " + dbEx.ToString());
					m_SportsLog.Close();
				}	catch(Exception ex) {
					iDeletedCnt = -1;
					m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
					m_SportsLog.SetFileName(0,LOGFILESUFFIX);
					m_SportsLog.Open();
					m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " DeleteSoccerSchedule.cs.Modify(): " + ex.ToString());
					m_SportsLog.Close();
				}
			}

			return iDeletedCnt;
		}
	}
}
