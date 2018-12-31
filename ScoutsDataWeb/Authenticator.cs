/*
Objective:
Authenticate user identity when log in this site

Code Remark:
-1		Wrong password/user or invalid user
-2		Access deny due to early access time
-3		Access deny due to late access time
-4		Access deny due to expiring access time
-5		Access deny due to invalid user right
-99		System Exception

Last updated:
9 Dec 2003 by Chapman

C#.NET complier statement:
csc /t:library /out:..\bin\Authenticator.dll /r:..\bin\DBManager.dll;..\bin\Files.dll Authenticator.cs
*/

using System;
using System.Configuration;
using System.Data.OleDb;
using System.Reflection;
using System.Web;
using TDL.DB;
using TDL.IO;
 
namespace SportsUtil {
	public class Authenticator {
		const string LOGFILESUFFIX = "log";
		string m_Connection;
		Files m_SportsLog;
		int m_AccessCode;

		public Authenticator(string Connection) {
			m_Connection = Connection;
			m_SportsLog = new Files();
			m_AccessCode = -1;
		}

		public int AccessCode {
			get {
				return m_AccessCode;
			}
		}

		public string[] UserAuthenticate(string sUserName, string sPassword) {
			string[] arrUserInfo = new string[2];
			arrUserInfo[0] = m_AccessCode.ToString();
			arrUserInfo[1] = m_AccessCode.ToString();
			OleDbDataReader SportsOleReader;
			DBManager SportsDBMgr;
			SportsDBMgr = new DBManager();
			SportsDBMgr.ConnectionString = m_Connection;
			DateTime dtAccessStart;
			DateTime dtAccessEnd;

			try {
				int iExisted = SportsDBMgr.ExecuteScalar("SELECT COUNT(USER_ID) FROM LOGININFO WHERE USERNAME='" + sUserName + "' AND PWD='" + sPassword + "'");
				SportsDBMgr.Close();
				if(iExisted > 0) {
					SportsOleReader = SportsDBMgr.ExecuteQuery("SELECT ACCESS_START, ACCESS_END, USER_ID, ACCESS_RIGHT FROM LOGININFO WHERE USERNAME='" + sUserName + "'");
					if(SportsOleReader.Read()) {
						if(Convert.ToInt32(SportsOleReader.GetString(3).Trim()) != 800) {
							if(SportsOleReader.IsDBNull(0) && SportsOleReader.IsDBNull(1)) {	//both are null, grant access all the time
								m_AccessCode = 1;
							} else {
								if(SportsOleReader.IsDBNull(0) ^ SportsOleReader.IsDBNull(1)) {	//either one is null
									if(!SportsOleReader.IsDBNull(0)) {	//grant access from start time forever
										dtAccessStart = SportsOleReader.GetDateTime(0);
										if(DateTime.Compare(DateTime.Now, dtAccessStart) > 0) {
											m_AccessCode = 1;
										} else {
											m_AccessCode = -2;
										}
									} else {	//grant access until end time
										dtAccessEnd = SportsOleReader.GetDateTime(1);
										if(DateTime.Compare(DateTime.Now, dtAccessEnd) < 0) {
											m_AccessCode = 1;
										} else {
											m_AccessCode = -3;
										}
									}
								} else {	//both are not null
									dtAccessStart = SportsOleReader.GetDateTime(0);
									dtAccessEnd = SportsOleReader.GetDateTime(1);
									if((DateTime.Compare(DateTime.Now, dtAccessStart) > 0) && (DateTime.Compare(DateTime.Now, dtAccessEnd) < 0)) {
										m_AccessCode = 1;
									} else {
										m_AccessCode = -4;
									}
								}
							}

							if(m_AccessCode > 0) {
								arrUserInfo[0] = SportsOleReader.GetInt32(2).ToString();
								arrUserInfo[1] = SportsOleReader.GetString(3).Trim();
							} else {
								arrUserInfo[0] = m_AccessCode.ToString();
								arrUserInfo[1] = m_AccessCode.ToString();
							}
						} else {
							m_AccessCode = -5;
						}
					}
					SportsOleReader.Close();
					SportsDBMgr.Close();
				}
				SportsDBMgr.Dispose();

				m_SportsLog.FilePath = ConfigurationManager.AppSettings["eventlog"];
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				if(m_AccessCode > 0) {
					m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Authenticator.cs: " + sUserName + " login success.");
				} else if(m_AccessCode == -1) {
					m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Authenticator.cs: " + sUserName + " login failed. Reason: Wrong password/user or invalid user.");
				} else if(m_AccessCode == -2) {
					m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Authenticator.cs: " + sUserName + " login failed. Reason: Access deny due to early access time.");
				} else if(m_AccessCode == -3) {
					m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Authenticator.cs: " + sUserName + " login failed. Reason: Access deny due to late access time.");
				} else if(m_AccessCode == -4) {
					m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Authenticator.cs: " + sUserName + " login failed. Reason: Access deny due to expiring access time.");
				} else if(m_AccessCode == -5) {
					m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Authenticator.cs: " + sUserName + " login failed. Reason: Access deny due to invalid user right.");
				} else {
					m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Authenticator.cs: " + sUserName + " login failed. Reason: Unknow.");
				}
				m_SportsLog.Close();
			} catch(Exception ex) {
				arrUserInfo[0] = "-99";
				arrUserInfo[1] = "-99";
				m_SportsLog.FilePath = ConfigurationManager.AppSettings["errlog"];
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Authenticator.cs.UserAuthenticate(): " + ex.ToString());
				m_SportsLog.Close();
			}

			return arrUserInfo;
		}

		public void Logout(string sUser) {
			m_SportsLog.FilePath = ConfigurationManager.AppSettings["eventlog"];
			m_SportsLog.SetFileName(0,LOGFILESUFFIX);
			m_SportsLog.Open();
			m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Authenticator.cs: " + sUser + " logout success");
			m_SportsLog.Close();
		}
	}
}