/*
Objective:
Add, delete user; change password

Last updated:
26 Nov 2003 by Chapman

C#.NET complier statement:
csc /t:library /out:..\bin\UserManager.dll /r:..\bin\DBManager.dll;..\bin\Files.dll UserManager.cs
*/

using System;
using System.Data.OleDb;
using System.Reflection;
using System.Text;
using System.Web;
using TDL.DB;
using TDL.IO;

[assembly:AssemblyCompany("TDSL")]
[assembly:AssemblyCopyright("(c) 2003 TDSL. All rights reserved.")]
[assembly:AssemblyDescription("使用者設定")]
[assembly:AssemblyProduct("Sports.NET")]
[assembly:AssemblyTitle("Sports.NET DLL")]
[assembly:AssemblyVersion("1.0.*")]
namespace SportsUtil {
	public class UserManager {
		const string LOGFILESUFFIX = "log";
		int m_TotalGp;
		DBManager m_SportsDBMgr;
		Files m_SportsLog;
		OleDbDataReader m_SportsOleReader;
		StringBuilder SQLString;

		public UserManager(string Connection) {
			m_SportsDBMgr = new DBManager();
			m_SportsDBMgr.ConnectionString = Connection;
			m_SportsLog = new Files();
			SQLString = new StringBuilder();
			m_TotalGp = 0;
		}

		public int TotalGp {
			get {
				return m_TotalGp;
			}
		}

		public string ShowForm(string sAction) {
			string sRole = (string)HttpContext.Current.Session["user_role"];
			if(sRole == null) sRole = "0";
			string sUserName = (string)HttpContext.Current.Session["user_name"];
			if(sUserName == null) sUserName = "Invalid User (Please Logout!)";
			StringBuilder HTMLString = new StringBuilder();

			switch(sAction) {
				case "M":
					try {
						HTMLString.Append("<tr align=\"center\" style=\"background-color:#FFF5EE; color:#4B0082\"><th colspan=\"2\">修改");
						HTMLString.Append(sUserName);
						HTMLString.Append("資訊</th></tr>");
						HTMLString.Append("<tr align=\"center\"><th>舊密碼:</th><td><input type=\"password\" name=\"oldpasswd\" maxlength=\"20\" value=\"\"></td></tr>");
						HTMLString.Append("<tr align=\"center\"><th>新密碼:</th><td><input type=\"password\" name=\"newpasswd1\" maxlength=\"20\" value=\"\"></td></tr>");
						HTMLString.Append("<tr align=\"center\"><th>確認新密碼:</th><td><input type=\"password\" name=\"newpasswd2\" maxlength=\"20\" value=\"\"></td></tr>");
						HTMLString.Append("<input type=\"hidden\" name=\"group\" value=\"\">");
						HTMLString.Append("<input type=\"hidden\" name=\"username\" value=\"\">");
						HTMLString.Append("<input type=\"hidden\" name=\"right\" value=\"\">");
						HTMLString.Append("<input type=\"hidden\" name=\"act\" value=\"M\">");
					}	catch(Exception ex) {
						m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
						m_SportsLog.SetFileName(0,LOGFILESUFFIX);
						m_SportsLog.Open();
						m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " UserManager.cs.ShowForm(Modify): " + ex.ToString());
						m_SportsLog.Close();
						HTMLString.Remove(0,HTMLString.Length);
						HTMLString.Append("<tr align=\"center\" style=\"background-color:#FFF5EE; color:#4B0082\"><th colspan=\"2\">");
						HTMLString.Append((string)HttpContext.Current.Application["accessErrorMsg"]);
						HTMLString.Append("</th></tr>");
					}
					break;
				case "A":
					try {
						if(Convert.ToInt32(sRole) >= 999) {
							HTMLString.Append("<tr align=\"center\" style=\"background-color:#FFFAF0; color:#6495ED\"><th colspan=\"2\">請輸入使用者資訊</th></tr>");
							HTMLString.Append("<tr align=\"center\"><th>名稱:</th><td><input name=\"username\" maxlength=\"6\" value=\"\"></td></tr>");
							HTMLString.Append("<tr align=\"center\"><th>密碼:</th><td><input type=\"password\" name=\"oldpasswd\" maxlength=\"20\" value=\"\"></td></tr>");

							HTMLString.Append("<tr align=\"center\"><th>群組:</th><td>");
							SQLString.Remove(0, SQLString.Length);
							SQLString.Append("select IGP_ID, CDESC from LOGINGROUP order by IGP_ID");
							m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
							while(m_SportsOleReader.Read()) {
								HTMLString.Append("<input type=\"checkbox\" name=\"group\" value=\"");
								HTMLString.Append(m_SportsOleReader.GetInt32(0).ToString());
								HTMLString.Append("\" onClick=\"GpCheck()\">");
								HTMLString.Append(m_SportsOleReader.GetString(1).Trim());
								m_TotalGp++;
								if(m_TotalGp%3 == 0) HTMLString.Append("<br>");
							}
							m_SportsOleReader.Close();
							m_SportsDBMgr.Close();
							HTMLString.Append("</td></tr>");

							HTMLString.Append("<tr align=\"center\"><th>權限:</th><td><select name=\"right\">");
							SQLString.Remove(0, SQLString.Length);
							SQLString.Append("select ACCESS_RIGHT, CDESC from LOGINRIGHT order by ACCESS_RIGHT");
							m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
							while(m_SportsOleReader.Read()) {
								HTMLString.Append("<option value=\"");
								HTMLString.Append(m_SportsOleReader.GetString(0).Trim());
								HTMLString.Append("\">");
								HTMLString.Append(m_SportsOleReader.GetString(1).Trim());
								HTMLString.Append("</option>");
							}
							m_SportsOleReader.Close();
							m_SportsDBMgr.Close();
							HTMLString.Append("</select></td></tr>");
							HTMLString.Append("<input type=\"hidden\" name=\"newpasswd1\" value=\"\">");
							HTMLString.Append("<input type=\"hidden\" name=\"newpasswd2\" value=\"\">");
							HTMLString.Append("<input type=\"hidden\" name=\"act\" value=\"A\">");
						} else {
							HTMLString.Append("<tr align=\"center\" style=\"background-color:#FFFAF0; color:#6495ED\"><th colspan=\"2\">沒有權限</th></tr>");
						}
					}	catch(Exception ex) {
						m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
						m_SportsLog.SetFileName(0,LOGFILESUFFIX);
						m_SportsLog.Open();
						m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " UserManager.cs.ShowForm(Add): " + ex.ToString());
						m_SportsLog.Close();
						HTMLString.Remove(0,HTMLString.Length);
						HTMLString.Append("<tr align=\"center\" style=\"background-color:#FFFAF0; color:#6495ED\"><th colspan=\"2\">");
						HTMLString.Append((string)HttpContext.Current.Application["accessErrorMsg"]);
						HTMLString.Append("</th></tr>");
					}
					break;
				case "D":
					try {
						if(Convert.ToInt32(sRole) >= 999) {
							HTMLString.Append("<tr align=\"center\" style=\"background-color:#F0FFFF; color:#FF0000\"><th colspan=\"2\">請選擇使用者刪除</th></tr>");
							HTMLString.Append("<tr align=\"center\"><th>名稱:</th><td><select name=\"username\">");
							SQLString.Remove(0, SQLString.Length);
							SQLString.Append("select USERNAME from LOGININFO order by USER_ID");
							m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
							while(m_SportsOleReader.Read()) {
								HTMLString.Append("<option value=\"");
								HTMLString.Append(m_SportsOleReader.GetString(0).Trim());
								HTMLString.Append("\">");
								HTMLString.Append(m_SportsOleReader.GetString(0).Trim());
								HTMLString.Append("</option>");
							}
							m_SportsOleReader.Close();
							m_SportsDBMgr.Close();
							HTMLString.Append("</select></td></tr>");
							HTMLString.Append("<input type=\"hidden\" name=\"oldpasswd\" value=\"\">");
							HTMLString.Append("<input type=\"hidden\" name=\"newpasswd1\" value=\"\">");
							HTMLString.Append("<input type=\"hidden\" name=\"newpasswd2\" value=\"\">");
							HTMLString.Append("<input type=\"hidden\" name=\"group\" value=\"\">");
							HTMLString.Append("<input type=\"hidden\" name=\"right\" value=\"\">");
							HTMLString.Append("<input type=\"hidden\" name=\"act\" value=\"D\">");
						} else {
							HTMLString.Append("<tr align=\"center\" style=\"background-color:#F0FFFF; color:#FF0000\"><th colspan=\"2\">沒有權限</th></tr>");
						}
					}	catch(Exception ex) {
						m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
						m_SportsLog.SetFileName(0,LOGFILESUFFIX);
						m_SportsLog.Open();
						m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " UserManager.cs.ShowForm(Delete): " + ex.ToString());
						m_SportsLog.Close();
						HTMLString.Remove(0,HTMLString.Length);
						HTMLString.Append("<tr align=\"center\" style=\"background-color:#F0FFFF; color:#FF0000\"><th colspan=\"2\">");
						HTMLString.Append((string)HttpContext.Current.Application["accessErrorMsg"]);
						HTMLString.Append("</th></tr>");
					}
					break;
				default:
					m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
					m_SportsLog.SetFileName(0,LOGFILESUFFIX);
					m_SportsLog.Open();
					m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " UserManager.cs.ShowForm(Default): Empty action");
					m_SportsLog.Close();
					HTMLString.Append("<tr align=\"center\"><th colspan=\"2\">");
					HTMLString.Append((string)HttpContext.Current.Application["accessErrorMsg"]);
					HTMLString.Append("</th></tr>");
					break;
			}

			return HTMLString.ToString();
		}

		public int Update() {
			int iRecUpd = 0;
			int iExisted = 0;
			string sRole;
			string sAction;
			string sUserID;
			string sUserName = "";
			string sPassword = "";
			string sNewPassword = "";
			string sUserRight = "";
			string[] arrUserGroup = null;

			sAction = HttpContext.Current.Request.Form["act"];
			if(sAction == null) sAction = "";
			sRole = (string)HttpContext.Current.Session["user_role"];
			if(sRole == null) sRole = "0";
			sUserID = (string)HttpContext.Current.Session["user_id"];
			if(sUserID == null) sUserID = "-1";

			if(sAction.Equals("M")) {	//Modify Password
				sPassword = HttpContext.Current.Request.Form["oldpasswd"].Trim();
				sNewPassword = HttpContext.Current.Request.Form["newpasswd1"].Trim();

				try {
					//Check User ID with original password
					SQLString.Remove(0, SQLString.Length);
					SQLString.Append("select count(USER_ID) from LOGININFO where USER_ID=");
					SQLString.Append(sUserID);
					SQLString.Append(" and PWD='");
					SQLString.Append(sPassword);
					SQLString.Append("'");
					iExisted = m_SportsDBMgr.ExecuteScalar(SQLString.ToString());
					m_SportsDBMgr.Close();
					if(iExisted > 0) {
						SQLString.Remove(0, SQLString.Length);
						SQLString.Append("update LOGININFO set PWD='");
						SQLString.Append(sNewPassword);
						SQLString.Append("' where USER_ID=");
						SQLString.Append(sUserID);
						iRecUpd = m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
						m_SportsDBMgr.Close();

						m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
						m_SportsLog.SetFileName(0,LOGFILESUFFIX);
						m_SportsLog.Open();
						m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " UserManager.cs: User (" + (string)HttpContext.Current.Session["user_name"] + ") modified password.");
						m_SportsLog.Close();
					} else {	//No related user
						iRecUpd = -11;
						m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
						m_SportsLog.SetFileName(0,LOGFILESUFFIX);
						m_SportsLog.Open();
						m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " UserManager.cs.Update(Modify): Modify password failed, user (" + (string)HttpContext.Current.Session["user_name"] + ") did not exist or wrong original password.");
						m_SportsLog.Close();
					}
				} catch(Exception ex) {
					iRecUpd = -10;
					m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
					m_SportsLog.SetFileName(0,LOGFILESUFFIX);
					m_SportsLog.Open();
					m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " UserManager.cs.Update(Modify): " + ex.ToString());
					m_SportsLog.Close();
				}
			} else if(sAction.Equals("A")) {
				if(Convert.ToInt32(sRole) >= 999) {
					char[] delimiter = new char[] {','};
					sUserName = HttpContext.Current.Request.Form["username"].Trim();
					sPassword = HttpContext.Current.Request.Form["oldpasswd"].Trim();
					sUserRight = HttpContext.Current.Request.Form["right"];
					try {
						arrUserGroup = HttpContext.Current.Request.Form["group"].Split(delimiter);
					} catch(Exception) {
						arrUserGroup = new string[1];
						arrUserGroup[0] = "0";
					}

					try {
						//Check User existed or not
						SQLString.Remove(0, SQLString.Length);
						SQLString.Append("select count(USER_ID) from LOGININFO where USERNAME='");
						SQLString.Append(sUserName);
						SQLString.Append("'");
						iExisted = m_SportsDBMgr.ExecuteScalar(SQLString.ToString());
						m_SportsDBMgr.Close();
						if(iExisted == 0) {
							int iUsrID = 0;
							int iMax = 0;
							int iUpdIdx = 0;
							string[] arrLeague;

							//get MAX User ID
							SQLString.Remove(0, SQLString.Length);
							SQLString.Append("SELECT MAX(USER_ID) FROM LOGININFO");
							iUsrID = m_SportsDBMgr.ExecuteScalar(SQLString.ToString());
							m_SportsDBMgr.Close();
							iUsrID++;

							//add user information into logininfo
							SQLString.Remove(0, SQLString.Length);
							SQLString.Append("insert into LOGININFO values(");
							SQLString.Append(iUsrID.ToString());
							SQLString.Append(",'");
							SQLString.Append(sUserName);
							SQLString.Append("','");
							SQLString.Append(sPassword);
							SQLString.Append("','");
							SQLString.Append(sUserRight);
							SQLString.Append("',null,null)");
							iRecUpd = m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
							m_SportsDBMgr.Close();

							//select league id to array
							SQLString.Remove(0, SQLString.Length);
							SQLString.Append("select count(LEAG_ID) from LEAGINFO");
							arrLeague = new string[m_SportsDBMgr.ExecuteScalar(SQLString.ToString())];
							m_SportsDBMgr.Close();

							SQLString.Remove(0, SQLString.Length);
							SQLString.Append("select LEAG_ID from LEAGINFO");
							m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
							while(m_SportsOleReader.Read()) {
								arrLeague[iUpdIdx] = m_SportsOleReader.GetString(0).Trim();
								iUpdIdx++;
							}
							m_SportsOleReader.Close();
							m_SportsDBMgr.Close();

							//select max rec_no from userprofile_info
							SQLString.Remove(0, SQLString.Length);
							SQLString.Append("SELECT MAX(IREC_NO) FROM USERPROFILE_INFO");
							iMax = m_SportsDBMgr.ExecuteScalar(SQLString.ToString());
							m_SportsDBMgr.Close();

							//add user information into userprofile_info
							for(iUpdIdx = 0; iUpdIdx < arrLeague.Length; iUpdIdx++) {
								iMax++;
								SQLString.Remove(0, SQLString.Length);
								SQLString.Append("insert into userprofile_info values(");
								SQLString.Append(iMax.ToString());
								SQLString.Append(",");
								SQLString.Append(iUsrID.ToString());
								SQLString.Append(",'");
								SQLString.Append(arrLeague[iUpdIdx].Trim());
								SQLString.Append("')");
								m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
								m_SportsDBMgr.Close();
							}

							//add user into USERPROFILE_GROUP
							for(iUpdIdx = 0; iUpdIdx < arrUserGroup.Length; iUpdIdx++) {
								SQLString.Remove(0, SQLString.Length);
								SQLString.Append("insert into USERPROFILE_GROUP values(");
								SQLString.Append(iUsrID.ToString());
								SQLString.Append(",");
								SQLString.Append(arrUserGroup[iUpdIdx].Trim());
								SQLString.Append(")");
								m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
								m_SportsDBMgr.Close();
							}

							m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
							m_SportsLog.SetFileName(0,LOGFILESUFFIX);
							m_SportsLog.Open();
							m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " UserManager.cs: New user (" + sUserName + ") was added by " + (string)HttpContext.Current.Session["user_name"]);
							m_SportsLog.Close();
						} else {
							iRecUpd = -21;
							m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
							m_SportsLog.SetFileName(0,LOGFILESUFFIX);
							m_SportsLog.Open();
							m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " UserManager.cs.Update(Add): Add user failed, user (" + sUserName + ") existed already.");
							m_SportsLog.Close();
						}
					} catch(Exception ex) {
						iRecUpd = -20;
						m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
						m_SportsLog.SetFileName(0,LOGFILESUFFIX);
						m_SportsLog.Open();
						m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " UserManager.cs.Update(Add): " + ex.ToString());
						m_SportsLog.Close();
					}
				} else {
					iRecUpd = -99;	//Access deny
					m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
					m_SportsLog.SetFileName(0,LOGFILESUFFIX);
					m_SportsLog.Open();
					m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " UserManager.cs.Update(Add): Add user failed, user (" + (string)HttpContext.Current.Session["user_name"] + ") access deny.");
					m_SportsLog.Close();
				}
			} else if(sAction.Equals("D")) {
				if(Convert.ToInt32(sRole) >= 999) {
					sUserName = HttpContext.Current.Request.Form["username"].Trim();

					try {
						//Check User existed or not
						SQLString.Remove(0, SQLString.Length);
						SQLString.Append("select count(USER_ID) from LOGININFO where USERNAME='");
						SQLString.Append(sUserName);
						SQLString.Append("'");
						iExisted = m_SportsDBMgr.ExecuteScalar(SQLString.ToString());
						m_SportsDBMgr.Close();
						if(iExisted > 0) {
							//delete user information from userprofile_info
							SQLString.Remove(0, SQLString.Length);
							SQLString.Append("delete from userprofile_info where iuser_id = (select user_id from logininfo where username='");
							SQLString.Append(sUserName);
							SQLString.Append("')");
							m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
							m_SportsDBMgr.Close();

							//delete user information from USERPROFILE_GROUP
							SQLString.Remove(0, SQLString.Length);
							SQLString.Append("delete from USERPROFILE_GROUP where IUSER_ID = (select user_id from logininfo where username='");
							SQLString.Append(sUserName);
							SQLString.Append("')");
							m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
							m_SportsDBMgr.Close();

							//delete user information from logininfo
							SQLString.Remove(0, SQLString.Length);
							SQLString.Append("delete from logininfo where username='");
							SQLString.Append(sUserName);
							SQLString.Append("'");
							iRecUpd = m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
							m_SportsDBMgr.Close();

							m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
							m_SportsLog.SetFileName(0,LOGFILESUFFIX);
							m_SportsLog.Open();
							m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " UserManager.cs: User (" + sUserName + ") was deleted by " + (string)HttpContext.Current.Session["user_name"]);
							m_SportsLog.Close();
						} else {
							iRecUpd = -31;
							m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
							m_SportsLog.SetFileName(0,LOGFILESUFFIX);
							m_SportsLog.Open();
							m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " UserManager.cs.Update(Delete): Delete user failed, user (" + sUserName + ") existed already.");
							m_SportsLog.Close();
						}
					} catch(Exception ex) {
						iRecUpd = -30;
						m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
						m_SportsLog.SetFileName(0,LOGFILESUFFIX);
						m_SportsLog.Open();
						m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " UserManager.cs.Update(Delete): " + ex.ToString());
						m_SportsLog.Close();
					}
				} else {
					iRecUpd = -99;	//Access deny
					m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
					m_SportsLog.SetFileName(0,LOGFILESUFFIX);
					m_SportsLog.Open();
					m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " UserManager.cs.Update(Delete): Add user failed, user (" + (string)HttpContext.Current.Session["user_name"] + ") access deny.");
					m_SportsLog.Close();
				}
			} else {
				iRecUpd = -40;	//Empty Action Error
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " UserManager.cs.Update(): Empty action.");
				m_SportsLog.Close();
			}

			return iRecUpd;
		}
	}
}