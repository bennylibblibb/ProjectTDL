/*
Objective:
Update horse racing menu

Last updated:
19 June 2003 by Chapman

C#.NET complier statement:
csc /t:library /out:..\bin\HorseRacingMenu.dll /r:..\bin\DBManager.dll;..\bin\Files.dll HorseRacingMenu.cs
*/

using System;
using System.Data.OleDb;
using System.Reflection;
using System.Web;
using TDL.DB;
using TDL.IO;

[assembly:AssemblyCompany("TDSL")]
[assembly:AssemblyCopyright("(c) 2003 TDSL. All rights reserved.")]
[assembly:AssemblyDescription("賽馬資訊 -> 更新菜單")]
[assembly:AssemblyProduct("Sports.NET")]
[assembly:AssemblyTitle("Sports.NET DLL")]
[assembly:AssemblyVersion("1.0.*")]
namespace SportsUtil {
	public class HorseRacingMenu {
		const string LOGFILESUFFIX = "log";
		DBManager m_MenuDB;
		Files m_MenuFile;

		public HorseRacingMenu(string Connection) {
			m_MenuDB = new DBManager();
			m_MenuDB.ConnectionString = Connection;
			m_MenuFile = new Files();
		}

		public string GetMenu() {
			string sRtn = "";
			string sMenuQuery;
			OleDbDataReader menuReader;

			try {
				sMenuQuery = "select IRACE_NO from RACEINFO order by IRACE_NO";
				menuReader = m_MenuDB.ExecuteQuery(sMenuQuery);
				while(menuReader.Read()) {
					sRtn += "<option value=\"" + menuReader.GetInt32(0).ToString() + "\">第" + menuReader.GetInt32(0).ToString() + "場</option>";
				}
				m_MenuDB.Close();
				menuReader.Close();
				m_MenuDB.Dispose();
			} catch(Exception ex) {
				m_MenuFile.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_MenuFile.SetFileName(0,LOGFILESUFFIX);
				m_MenuFile.Open();
				m_MenuFile.Write(DateTime.Now.ToString("HH:mm:ss") + " HorseRacingMenu.cs.GetMenu(): " + ex.ToString());
				m_MenuFile.Close();
				sRtn = (string)HttpContext.Current.Application["accessErrorMsg"];
			}
			return sRtn;
		}
	}
}