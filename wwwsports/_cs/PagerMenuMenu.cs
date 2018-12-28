/*
Objective:
Selection menu for resending pager menu

Last updated:
5 Nov 2003 by Chapman

C#.NET complier statement:
csc /t:library /out:..\bin\PagerMenuMenu.dll /r:..\bin\DBManager.dll;..\bin\Files.dll PagerMenuMenu.cs
*/

using System;
using System.Collections;
using System.Data.OleDb;
using System.Reflection;
using System.Text;
using System.Web;
using TDL.DB;
using TDL.IO;

[assembly:AssemblyCompany("TDSL")]
[assembly:AssemblyCopyright("(c) 2003 TDSL. All rights reserved. Created on 20 Oct 2003.")]
[assembly:AssemblyDescription("管理項目 -> 重發菜單(菜單選項)")]
[assembly:AssemblyProduct("Sports.NET")]
[assembly:AssemblyTitle("Sports.NET DLL")]
[assembly:AssemblyVersion("3.0.*")]
namespace SportsUtil {
	public class PagerMenuMenu {
		const string LOGFILESUFFIX = "log";
		OleDbDataReader m_SportsOleReader;
		DBManager m_SportsDBMgr;
		Files m_SportsLog;
		StringBuilder SQLString;

		public PagerMenuMenu(string Connection) {
			m_SportsDBMgr = new DBManager();
			m_SportsDBMgr.ConnectionString = Connection;
			m_SportsLog = new Files();
			SQLString = new StringBuilder();
		}

		public string ShowMenu() {
			StringBuilder HTMLString = new StringBuilder();
			ArrayList MenuTypeList = new ArrayList(5);

			try {
				//Retrieve menu types
				HTMLString.Append("<tr align=\"left\"><th>菜單</th><td><a href=\"../index.htm\" target=\"_top\">返回主頁</a></td>");
				SQLString.Remove(0,SQLString.Length);
				SQLString.Append("select ITYPE_ID, CTYPE from MENUTYPE_CFG order by ITYPE_ID");
				m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
				while(m_SportsOleReader.Read()) {
					MenuTypeList.Add(m_SportsOleReader.GetInt32(0).ToString());
					HTMLString.Append("<td>");
					HTMLString.Append(m_SportsOleReader.GetString(1).Trim());
					HTMLString.Append("</td>");
				}
				m_SportsOleReader.Close();
				m_SportsDBMgr.Close();
				MenuTypeList.TrimToSize();
				HTMLString.Append("</tr><tr align=\"left\"><td></td><td><a href=\"PagerMenuMenu.aspx\" target=\"menu_frame\">更新選單</a></td>");

				for(int i = 0; i < MenuTypeList.Count; i++) {
					HTMLString.Append("<td><select name=\"PagerMenu\" onChange=\"goToMenu(");
					HTMLString.Append(i.ToString());
					HTMLString.Append(",PagerMenuForm.PagerMenu[");
					HTMLString.Append(i.ToString());
					HTMLString.Append("].value)\"><option value=\"0\">請選擇</option>");
					SQLString.Remove(0,SQLString.Length);
					SQLString.Append("select gp.IGROUP_ID, gp.CGROUP from MENUGROUP_CFG gp, MENUMAP map where map.ITYPE_ID = ");
					SQLString.Append((string)MenuTypeList[i]);
					SQLString.Append(" and map.IGROUP_ID = gp.IGROUP_ID group by gp.CGROUP, gp.IGROUP_ID order by gp.IGROUP_ID");
					m_SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
					while(m_SportsOleReader.Read()) {
						HTMLString.Append("<option value=\"PagerMenu.aspx?typeID=");
						HTMLString.Append((string)MenuTypeList[i]);
						HTMLString.Append("&gpID=");
						HTMLString.Append(m_SportsOleReader.GetInt32(0).ToString());
						HTMLString.Append("\">");
						HTMLString.Append(m_SportsOleReader.GetString(1).Trim());
						HTMLString.Append("</option>");
					}
					m_SportsOleReader.Close();
					m_SportsDBMgr.Close();
					HTMLString.Append("</select></td>");
				}
				m_SportsDBMgr.Dispose();
				HTMLString.Append("</tr>");
			} catch(Exception ex) {
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " PagerMenuMenu.cs.ShowMenu(): " + ex.ToString());
				m_SportsLog.Close();
				HTMLString.Remove(0,HTMLString.Length);
				HTMLString.Append((string)HttpContext.Current.Application["accessErrorMsg"]);
			}

			return HTMLString.ToString();
		}
	}
}