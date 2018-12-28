/*
Objective:
Update menu of other region

Last updated:
14 May 2004, Chapman Choi

C#.NET complier statement:
csc /t:library /out:..\bin\RegionMenu.dll /r:..\bin\DBManager.dll;..\bin\Files.dll RegionMenu.cs
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
[assembly:AssemblyDescription("其他地區 -> 菜單")]
[assembly:AssemblyProduct("Sports.NET")]
[assembly:AssemblyTitle("Sports.NET DLL")]
[assembly:AssemblyVersion("1.0.*")]
namespace SportsUtil {
	public class RegionMenu {
		const string LOGFILESUFFIX = "log";
		DBManager m_RegionDB;
		ArrayList m_RegionIDList;
		Files m_RegionFile;

		public RegionMenu(string Connection) {
			m_RegionDB = new DBManager();
			m_RegionDB.ConnectionString = Connection;
			m_RegionIDList = new ArrayList(5);
			m_RegionFile = new Files();
		}

		public string GetName() {
			string sRegionQuery;
			string sRtn = "";
			OleDbDataReader regionReader;
			sRegionQuery = "select IREGION_ID,CREGION from OTHERREGION_CFG order by IREGION_ID";
			try {
				regionReader = m_RegionDB.ExecuteQuery(sRegionQuery);
				while(regionReader.Read()) {
					m_RegionIDList.Add(regionReader.GetInt32(0));
					sRtn += "<td>" + regionReader.GetString(1).Trim() + "</td>";
				}
				m_RegionDB.Close();
				regionReader.Close();
				m_RegionDB.Dispose();
				m_RegionIDList.TrimToSize();
			} catch(Exception ex) {
				m_RegionFile.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_RegionFile.SetFileName(0,LOGFILESUFFIX);
				m_RegionFile.Open();
				m_RegionFile.Write(DateTime.Now.ToString("HH:mm:ss") + " RegionMenu.cs.GetName(): " + ex.ToString());
				m_RegionFile.Close();
				sRtn = (string)HttpContext.Current.Application["accessErrorMsg"];
			}
			return sRtn;
		}

		public string GetLink() {
			int iLinkIndex=0;
			string sRtn = "";
			try {
				if (m_RegionIDList.Count==1){
					sRtn += "<td><select name=\"MslotAction\" onChange=\"goToPage(" + (Convert.ToInt32(m_RegionIDList[iLinkIndex]) - 1).ToString() + ",OtherRegionMenuForm.MslotAction.value)\">";
					sRtn += "<option value=\"0\">請選擇</option>";
					sRtn += "<option value=\"RegionImport.aspx?RegionID=" + m_RegionIDList[iLinkIndex] + "\">匯入賽事</option>";
					sRtn += "<option value=\"RegionModify.aspx?RegionID=" + m_RegionIDList[iLinkIndex] + "\">賠率更新</option>";
					sRtn += "<option value=\"RegionNewMenu.aspx?RegionID=" + m_RegionIDList[iLinkIndex] + "\">新增賽事</option>";
					sRtn += "<option value=\"RegionDelete.aspx?RegionID=" + m_RegionIDList[iLinkIndex] + "\">刪除賽事</option>";
					sRtn += "</select></td>";
				}
				else{
					for(iLinkIndex = 0; iLinkIndex < m_RegionIDList.Count; iLinkIndex++) {
						sRtn += "<td><select name=\"MslotAction\" onChange=\"goToPage(" + (Convert.ToInt32(m_RegionIDList[iLinkIndex]) - 1).ToString() + ",OtherRegionMenuForm.MslotAction[" + (Convert.ToInt32(m_RegionIDList[iLinkIndex]) - 1).ToString() + "].value)\">";
						sRtn += "<option value=\"0\">請選擇</option>";
						sRtn += "<option value=\"RegionImport.aspx?RegionID=" + m_RegionIDList[iLinkIndex] + "\">匯入賽事</option>";
						sRtn += "<option value=\"RegionModify.aspx?RegionID=" + m_RegionIDList[iLinkIndex] + "\">賠率更新</option>";
						sRtn += "<option value=\"RegionNewMenu.aspx?RegionID=" + m_RegionIDList[iLinkIndex] + "\">新增賽事</option>";
						sRtn += "<option value=\"RegionDelete.aspx?RegionID=" + m_RegionIDList[iLinkIndex] + "\">刪除賽事</option>";
						sRtn += "</select></td>";
					}//end-for
				}//end-if
			} catch(Exception ex) {
				m_RegionFile.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_RegionFile.SetFileName(0,LOGFILESUFFIX);
				m_RegionFile.Open();
				m_RegionFile.Write(DateTime.Now.ToString("HH:mm:ss") + " RegionMenu.cs.GetLink(): " + ex.ToString());
				m_RegionFile.Close();
			}
			return sRtn;
		}
	}
}