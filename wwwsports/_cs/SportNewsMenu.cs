/*
Objective:
Menu of sports news

Last updated:
5 Nov 2003 by Chapman

C#.NET complier statement:
csc /t:library /out:..\bin\SportNewsMenu.dll /r:..\bin\DBManager.dll;..\bin\Files.dll SportNewsMenu.cs
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
[assembly:AssemblyDescription("其他資訊 -> 更新菜單")]
[assembly:AssemblyProduct("Sports.NET")]
[assembly:AssemblyTitle("Sports.NET DLL")]
[assembly:AssemblyVersion("1.0.*")]

namespace SportsUtil {
	public class SportNewsMenu {
		const string LOGFILESUFFIX = "log";
		DBManager m_MenuDB;
		Files m_MenuFile;

		public SportNewsMenu(string Connection) {
			m_MenuDB = new DBManager();
			m_MenuDB.ConnectionString = Connection;
			m_MenuFile = new Files();
		}

		public string GetMenu() {
			int iIndex = 0;
			int iCellsCnt = 0;
			int iDelimiterIdx = 0;
			string sRtn;
			string sMenuQuery;
			string sAppType;
			string sLastAppType = "";
			ArrayList newsAL = new ArrayList(5);
			ArrayList appAL = new ArrayList(5);
			OleDbDataReader menuReader;

			try {
				sRtn = "<tr align=\"left\"><th>其他資訊</th>";
				sRtn += "<td><a href=\"../index.htm\" target=\"_top\">返回主頁</a></td>";
				sMenuQuery = "select distinct CAPPTYPE from NEWS_CFG where IDISPLAY_NUM=1 order by ISEQNO";
				menuReader = m_MenuDB.ExecuteQuery(sMenuQuery);
				while(menuReader.Read()) {
					sAppType = menuReader.GetString(0).Trim();
					iDelimiterIdx = sAppType.IndexOf("[");
					if(iDelimiterIdx > 0) {
						sAppType = sAppType.Substring(0,iDelimiterIdx);
						if(!sAppType.Equals(sLastAppType)) {
							if(!newsAL.Contains(sAppType)) {
								newsAL.Add(sAppType);
								sRtn += "<td>" + sAppType + "</td>";
								sLastAppType = sAppType;
							}
						}
					}
				}
				menuReader.Close();
				m_MenuDB.Close();
				
//				sRtn += "<td>版面設定</td>";

				sRtn += "</tr><tr align=\"left\"><th></th>";
				sRtn += "<td><a href=\"redirectToMenu.htm\" target=\"menu_frame\">更新選單</a></td>";
				for(iIndex = 0; iIndex < newsAL.Count; iIndex++) {
					sRtn += "<td><select name=\"SportNewsID\" onChange=\"goToNews(" + iIndex.ToString() + ",SportNewsMenuForm.SportNewsID[" + iIndex.ToString() + "].value)\"><option value=\"0\">請選擇</option>";
					sMenuQuery = "select ISEQNO, CINFOTYPE, IHDRIDSTART, IHDRIDEND from NEWS_CFG where CAPPTYPE like '%" + newsAL[iIndex] + "%' order by ISEQNO";
					menuReader = m_MenuDB.ExecuteQuery(sMenuQuery);
					while(menuReader.Read()) {
						iCellsCnt = menuReader.GetInt32(3) - menuReader.GetInt32(2) + 1;
						sRtn += "<option value=\"SportNews.aspx?AppID=" + menuReader.GetInt32(0).ToString() + "&MsgCnt=" + iCellsCnt.ToString() + "\">" + menuReader.GetString(1).Trim() + "</option>";
					}
					m_MenuDB.Close();
					menuReader.Close();
					sRtn += "</select></td>";
				}
/*				
				sRtn += "<td><select name=\"SportNewsID\" onChange=\"goToDisplayCfg(" + iIndex.ToString() + ",SportNewsMenuForm.SportNewsID[" + iIndex.ToString() + "].value)\"><option value=\"0\">請選擇</option>";
				for(iIndex = 0; iIndex < newsAL.Count; iIndex++) {
					
					int iAppID = iIndex+1;
					
					// only 其他運動
					if (newsAL[iIndex].ToString() == "其他運動") {
						sMenuQuery = "select INEWS_ID from NEWSGROUP_CONVERTER where CHI_NAME = '"+newsAL[iIndex]+"'";
						menuReader = m_MenuDB.ExecuteQuery(sMenuQuery);
	
						if (!menuReader.Read()) {
							m_MenuDB.Close();
							sMenuQuery = "insert into NEWSGROUP_CONVERTER values ("+iAppID.ToString()+", '"+newsAL[iIndex]+"')";
							m_MenuDB.ExecuteNonQuery(sMenuQuery);
						} else {
							m_MenuDB.Close();
							sMenuQuery = "update NEWSGROUP_CONVERTER set INEWS_ID="+iAppID.ToString()+" where CHI_NAME='"+newsAL[iIndex]+"'";
							m_MenuDB.ExecuteNonQuery(sMenuQuery);
						}
						m_MenuDB.Close();
						menuReader.Close();
						sRtn += "<option value=\"SportDisplayCfg.aspx?AppID="+iAppID+"\">"+newsAL[iIndex]+"</option>";
					}
				}
					
				sRtn += "</select></td>";
*/
				sRtn += "</tr>";
				
			} catch(Exception ex) {
				m_MenuFile.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_MenuFile.SetFileName(0,LOGFILESUFFIX);
				m_MenuFile.Open();
				m_MenuFile.Write(DateTime.Now.ToString("HH:mm:ss") + " SportNewsMenu.cs.GetMenu(): " + ex.ToString());
				m_MenuFile.Close();
				sRtn = (string)HttpContext.Current.Application["accessErrorMsg"];
			}

			return sRtn;
		}
	}
}