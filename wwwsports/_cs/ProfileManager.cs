/*
Objective:
Update user profile, league to do or not

Last updated:
12 July 2004, Chapman Choi

C#.NET complier statement:
csc /t:library /out:..\bin\ProfileManager.dll /r:..\bin\DBManager.dll;..\bin\Files.dll ProfileManager.cs
*/

using System;
using System.Collections;
using System.Data.OleDb;
using System.Reflection;
using System.Web;
using TDL.DB;
using TDL.IO;

[assembly:AssemblyCompany("TDSL")]
[assembly:AssemblyCopyright("(c) 2003 TDSL. All rights reserved.")]
[assembly:AssemblyDescription("一般設定 -> 個人處理聯賽項目")]
[assembly:AssemblyProduct("Sports.NET")]
[assembly:AssemblyTitle("Sports.NET DLL")]
[assembly:AssemblyVersion("1.0.*")]
namespace SportsUtil {
	public class ProfileManager {
		const string LOGFILESUFFIX = "log";
		int m_RecordCount = 0;
		DBManager m_ProfileDB;
		Files m_PManFile;

		public ProfileManager(string Connection) {
			m_ProfileDB = new DBManager();
			m_ProfileDB.ConnectionString = Connection;
			m_PManFile = new Files();
		}

		public int NumberOfRecords {
			get {
				return m_RecordCount;
			}
		}

		public string GetUserProfile() {
			string retrieveQuery;
			string sRecordStr;
			string sRtn = "";
			ArrayList leagIDAL = new ArrayList(10);
			OleDbDataReader profileReader;

			try {
				retrieveQuery = "select cleag_id from userprofile_info where iuser_id=" + (string)HttpContext.Current.Session["user_id"] + " and cleag_id not like 'x%'";
				profileReader = m_ProfileDB.ExecuteQuery(retrieveQuery);
				while(profileReader.Read()) {
					leagIDAL.Add(profileReader.GetString(0).Trim());
				}
				profileReader.Close();
				m_ProfileDB.Close();

				//select all leagues
				retrieveQuery = "select leagname, leag_id from leaginfo order by LEAG_ORDER";
				profileReader = m_ProfileDB.ExecuteQuery(retrieveQuery);
				while(profileReader.Read()) {
					sRtn += "<tr align=\"center\">";
					//League Full Name
					sRecordStr = profileReader.GetString(0).Trim();
					sRtn += "<td>" + sRecordStr + "</td>";

					//Hidden field: League ID
					sRecordStr = profileReader.GetString(1).Trim();
					sRtn += "<td><input type=\"checkbox\" name=\"selectedLeague\" value=\"" + m_RecordCount.ToString() + "\" ";
					if(leagIDAL.Contains(sRecordStr)) sRtn += "checked";
					sRtn += "><input type=\"hidden\" name=\"LeagID\" value=\"" + sRecordStr + "\"></td>";
					sRtn += "</tr>";
					m_RecordCount++;
				}
				profileReader.Close();
				m_ProfileDB.Close();
			} catch(Exception ex) {
				m_PManFile.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_PManFile.SetFileName(0,LOGFILESUFFIX);
				m_PManFile.Open();
				m_PManFile.Write(DateTime.Now.ToString("HH:mm:ss") + " ProfileManager.cs.GetUserProfile(): " + ex.ToString());
				m_PManFile.Close();
				sRtn = (string)HttpContext.Current.Application["accessErrorMsg"];
			}

			return sRtn;
		}

		public int[] Update() {
			int iTotal, iUpdIndex, iRecNum, iEnable = 0, iDisable = 0;
			string updateQuery, sUID;
			int[] RecUpd = new int[2];
			char[] delimiter = new char[] {','};
			string[] arrLeagID, arrSelected;

			sUID = (string)HttpContext.Current.Session["user_id"];
			arrLeagID = HttpContext.Current.Request.Form["LeagID"].Split(delimiter);
			try {
				arrSelected = HttpContext.Current.Request.Form["selectedLeague"].Split(delimiter);
				iTotal = arrLeagID.Length;
			}	catch {
				arrSelected = new string[0];
				iTotal = 0;
			}

			try {
				if(iTotal>0) {	//update userprofile_info (items had been checked or not)
					ArrayList selectedAL = new ArrayList();
					//copy elements into array list
					for(iUpdIndex=0;iUpdIndex<arrSelected.Length;iUpdIndex++) {
						selectedAL.Add(arrSelected[iUpdIndex]);
					}

					for(iUpdIndex=0;iUpdIndex<arrLeagID.Length;iUpdIndex++) {
						//get record number for w.r.t. league ID
						updateQuery = "select irec_no from userprofile_info where iuser_id=" + sUID + " and cleag_id like '%" + arrLeagID[iUpdIndex] + "'";
						iRecNum = m_ProfileDB.ExecuteScalar(updateQuery);
						m_ProfileDB.Close();

						if(selectedAL.Contains(iUpdIndex.ToString())) {	//checked item
							updateQuery = "update USERPROFILE_INFO set CLEAG_ID='" + arrLeagID[iUpdIndex] + "' where IUSER_ID=" + sUID + " and IREC_NO=" + iRecNum.ToString();
							m_ProfileDB.ExecuteNonQuery(updateQuery);
							m_ProfileDB.Close();
							iEnable++;
						}	else {	//not checked item
							updateQuery = "update USERPROFILE_INFO set CLEAG_ID='x" + arrLeagID[iUpdIndex] + "' where IUSER_ID=" + sUID + " and IREC_NO=" + iRecNum.ToString();
							m_ProfileDB.ExecuteNonQuery(updateQuery);
							m_ProfileDB.Close();
							iDisable++;
						}
					}

					//write log
					m_PManFile.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
					m_PManFile.SetFileName(0,LOGFILESUFFIX);
					m_PManFile.Open();
					m_PManFile.Write(DateTime.Now.ToString("HH:mm:ss") + " ProfileManager.cs: Update user profile enable " + iEnable.ToString() + " and disable " + iDisable.ToString() + " matches (" + HttpContext.Current.Session["user_name"] + ")");
					m_PManFile.Close();
				}	else {	//update userprofile_info (all items had not been checked)
					for(iUpdIndex=0;iUpdIndex<arrLeagID.Length;iUpdIndex++) {
						//get record number for w.r.t. league ID
						updateQuery = "select IREC_NO from USERPROFILE_INFO where IUSER_ID=" + sUID + " and CLEAG_ID like '%" + arrLeagID[iUpdIndex] + "'";
						iRecNum = m_ProfileDB.ExecuteScalar(updateQuery);
						m_ProfileDB.Close();

						updateQuery = "update USERPROFILE_INFO set CLEAG_ID='x" + arrLeagID[iUpdIndex] + "' where IUSER_ID=" + sUID + " and IREC_NO=" + iRecNum.ToString();
						m_ProfileDB.ExecuteNonQuery(updateQuery);
						m_ProfileDB.Close();
						iDisable++;
					}

					//write log
					m_PManFile.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
					m_PManFile.SetFileName(0,LOGFILESUFFIX);
					m_PManFile.Open();
					m_PManFile.Write(DateTime.Now.ToString("HH:mm:ss") + " ProfileManager.cs: Update user profile disable " + iDisable.ToString() + " matches (" + HttpContext.Current.Session["user_name"] + ")");
					m_PManFile.Close();
				}
			}	catch(Exception ex) {
				iEnable = -1;
				iDisable = -1;
				m_PManFile.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_PManFile.SetFileName(0,LOGFILESUFFIX);
				m_PManFile.Open();
				m_PManFile.Write(DateTime.Now.ToString("HH:mm:ss") + " ProfileManager.cs.Update(): " + ex.ToString());
				m_PManFile.Close();
			}
			RecUpd[0] = iEnable;
			RecUpd[1] = iDisable;
			return RecUpd;
		}
	}
}