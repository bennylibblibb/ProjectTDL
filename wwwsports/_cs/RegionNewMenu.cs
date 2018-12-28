/*
Objective:
List of league for adding a new match in other region

Last updated:
31 Oct 2003 by Chapman

C#.NET complier statement:
csc /t:library /out:..\bin\RegionNewMenu.dll /r:..\bin\DBManager.dll;..\bin\Files.dll RegionNewMenu.cs
*/

using System;
using System.Data.OleDb;
using System.Reflection;
using System.Web;
using TDL.DB;
using TDL.IO;

[assembly:AssemblyCompany("TDSL")]
[assembly:AssemblyCopyright("(c) 2003 TDSL. All rights reserved.")]
[assembly:AssemblyDescription("其他地區 -> 新增賽事(聯賽選項)")]
[assembly:AssemblyProduct("Sports.NET")]
[assembly:AssemblyTitle("Sports.NET DLL")]
[assembly:AssemblyVersion("1.0.*")]
namespace SportsUtil {
	public class RegionNewMenu {
		const string LOGFILESUFFIX = "log";
		string m_Region;
		DBManager m_SportsDBMgr;
		Files m_SportsLog;

		public RegionNewMenu(string Connection) {
			m_SportsDBMgr = new DBManager();
			m_SportsDBMgr.ConnectionString = Connection;
			m_SportsLog = new Files();
			m_Region = "";
		}

		public string Region {
			get {
				return m_Region;
			}
		}

		public string Show() {
			string retrieveQuery;
			string sRtn = "";
			string sRegionID;
			OleDbDataReader SportsOleReader;

			sRegionID = HttpContext.Current.Request.QueryString["RegionID"];
			try {
				retrieveQuery = retrieveQuery = "select CREGION from OTHERREGION_CFG where IREGION_ID=" + sRegionID;
				m_Region = m_SportsDBMgr.ExecuteQueryString(retrieveQuery);
				m_SportsDBMgr.Close();

				retrieveQuery = "select distinct leag.LEAG_ID, leag.ALIAS from LEAGINFO leag, SOCCERSCHEDULE sch where leag.leag_id in (select cleag_id from userprofile_info where iuser_id=";
				retrieveQuery += HttpContext.Current.Session["user_id"].ToString();
				retrieveQuery += ") AND leag.leag_id = sch.cleag_id order by leag.LEAG_ORDER, leag.LEAG_ID";
/*
				retrieveQuery = "select distinct leaginfo.LEAG_ID, leaginfo.ALIAS from LEAGINFO, userprofile_info ";
				retrieveQuery += "where leaginfo.leag_id in (select distinct cleag_id from userprofile_info where iuser_id=" + HttpContext.Current.Session["user_id"].ToString() + ") ";
				retrieveQuery += "order by LEAG_ORDER, LEAG_ID";
*/
				SportsOleReader = m_SportsDBMgr.ExecuteQuery(retrieveQuery);
				while(SportsOleReader.Read()) {
					sRtn += "<option value=\"RegionNew.aspx?RegionID=" + sRegionID + "&LeagID=" + SportsOleReader.GetString(0).Trim() + "\">" + SportsOleReader.GetString(1).Trim() + "</option>";
				}
				SportsOleReader.Close();
				m_SportsDBMgr.Close();
				m_SportsDBMgr.Dispose();
			} catch(Exception ex) {
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " RegionNewMenu.cs.Show(): " + ex.ToString());
				m_SportsLog.Close();
				sRtn = (string)HttpContext.Current.Application["accessErrorMsg"];
			}

			return sRtn;
		}
	}
}