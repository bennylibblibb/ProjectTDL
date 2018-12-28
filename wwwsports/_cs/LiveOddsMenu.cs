/*
Objective:
Live odds menu

Last updated:
14 Oct 2003 by Chapman

C#.NET complier statement:
csc /t:library /out:..\bin\LiveOddsMenu.dll /r:..\bin\DBManager.dll;..\bin\Files.dll LiveOddsMenu.cs
*/

using System;
using System.Collections;
using System.Data.OleDb;
using System.Reflection;
using System.Web;
using TDL.DB;
using TDL.IO;

[assembly:AssemblyCompany("TDSL")]
[assembly:AssemblyCopyright("(c) 2003 TDSL. All rights reserved. Created on 14 Oct 2003.")]
[assembly:AssemblyDescription("現場賠率 -> 菜單")]
[assembly:AssemblyProduct("Sports.NET")]
[assembly:AssemblyTitle("Sports.NET DLL")]
[assembly:AssemblyVersion("3.0.*")]
namespace SportsUtil {
	public class LiveOddsMenu {
		const string LOGFILESUFFIX = "log";
		DBManager m_SportsDBMgr;
		ArrayList m_RegionIDList;
		Files m_SportsLog;

		public LiveOddsMenu(string Connection) {
			m_SportsDBMgr = new DBManager();
			m_SportsDBMgr.ConnectionString = Connection;
			m_RegionIDList = new ArrayList(5);
			m_SportsLog = new Files();
		}

		public string GetName() {
			string sRegionQuery;
			string sRtn = "";
			OleDbDataReader SportsOleReader;
			sRegionQuery = "select IREGION_ID, CREGION from LIVEODDS_CFG order by IORDER, IREGION_ID";
			try {
				SportsOleReader = m_SportsDBMgr.ExecuteQuery(sRegionQuery);
				while(SportsOleReader.Read()) {
					m_RegionIDList.Add(SportsOleReader.GetInt32(0));
					sRtn += "<td>" + SportsOleReader.GetString(1).Trim() + "</td>";
				}
				m_SportsDBMgr.Close();
				SportsOleReader.Close();
				m_SportsDBMgr.Dispose();
				m_RegionIDList.TrimToSize();
			} catch(Exception ex) {
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " LiveOddsMenu.cs.GetName(): " + ex.ToString());
				m_SportsLog.Close();
				sRtn = (string)HttpContext.Current.Application["accessErrorMsg"];
			}
			return sRtn;
		}

		public string GetLink() {
			int iLinkIndex=0;
			string sRtn = "";
			try {
				if (m_RegionIDList.Count==1){
					sRtn += "<td><select name=\"LiveoddsAction\" onChange=\"goToPage(" + iLinkIndex.ToString() + ",LiveoddsMenuForm.LiveoddsAction.value)\">";
					sRtn += "<option value=\"0\">請選擇</option>";
					sRtn += "<option value=\"LiveOddsImport.aspx?RegionID=" + m_RegionIDList[iLinkIndex] + "\">匯入賽事</option>";
					sRtn += "<option value=\"LiveOddsModify.aspx?RegionID=" + m_RegionIDList[iLinkIndex] + "\">賠率更新</option>";
					sRtn += "<option value=\"LiveOddsDelete.aspx?RegionID=" + m_RegionIDList[iLinkIndex] + "\">刪除賽事</option>";
					sRtn += "</select></td>";
				}
				else{
					for(iLinkIndex = 0; iLinkIndex < m_RegionIDList.Count; iLinkIndex++) {
						sRtn += "<td><select name=\"LiveoddsAction\" onChange=\"goToPage(" + iLinkIndex.ToString() + ",LiveoddsMenuForm.LiveoddsAction[" + iLinkIndex.ToString() + "].value)\">";
						sRtn += "<option value=\"0\">請選擇</option>";
						sRtn += "<option value=\"LiveOddsImport.aspx?RegionID=" + m_RegionIDList[iLinkIndex] + "\">匯入賽事</option>";
						sRtn += "<option value=\"LiveOddsModify.aspx?RegionID=" + m_RegionIDList[iLinkIndex] + "\">賠率更新</option>";
						sRtn += "<option value=\"LiveOddsDelete.aspx?RegionID=" + m_RegionIDList[iLinkIndex] + "\">刪除賽事</option>";
						sRtn += "</select></td>";
					}
				}
			} catch(Exception ex) {
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " LiveOddsMenu.cs.GetLink(): " + ex.ToString());
				m_SportsLog.Close();
			}
			return sRtn;
		}
	}
}