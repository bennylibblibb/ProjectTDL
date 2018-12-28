/*
Objective:
Create a combo box contains housekeeped matches

Last updated:
30 Apr 2004, Chapman Choi

C#.NET complier statement:
csc /t:library /out:..\bin\ModifyResultMenu.dll /r:..\bin\DBManager.dll;..\bin\Files.dll ModifyResultMenu.cs
*/

using System;
using System.Data.OleDb;
using System.Reflection;
using System.Text;
using System.Web;
using TDL.DB;
using TDL.IO;

[assembly:AssemblyCompany("TDSL")]
[assembly:AssemblyCopyright("(c) 2004 TDSL. All rights reserved. Created on 28 Apr 2004.")]
[assembly:AssemblyDescription("足球資訊 -> 賽果選項")]
[assembly:AssemblyProduct("Sports.NET")]
[assembly:AssemblyTitle("Sports.NET DLL")]
[assembly:AssemblyVersion("1.0.*")]
namespace SportsUtil {
	public class ModifyResultMenu {
		const string LOGFILESUFFIX = "log";
		Files m_SportsLog;
		StringBuilder HTMLString;

		public ModifyResultMenu() {
			m_SportsLog = new Files();
			HTMLString = new StringBuilder();
		}

		public string Show() {
			int iColorIdx = 0;
			string sLeague = "";
			string sTarget;
			string[] arrTextColor = new string[3];
			DBManager SportsDBMgr;
			OleDbDataReader SportsOleReader;

			sTarget = HttpContext.Current.Request.QueryString["target"].Trim();
			try {
				arrTextColor[0] = "008000";
				arrTextColor[1] = "B22222";
				arrTextColor[2] = "1E90FF";

				SportsDBMgr = new DBManager();
				switch(sTarget) {
					case "gogo1":
						SportsDBMgr.ConnectionString = HttpContext.Current.Application["SoccerDBConnectionString"].ToString();
						SportsOleReader = SportsDBMgr.ExecuteQuery("select CLEAGUE, IHEADER_ID, CHOST, CGUEST from HKGOAL_DETAILS where CHOST<>'' and CGUEST<>'' order by IHEADER_ID");
						while(SportsOleReader.Read()) {
							if(!sLeague.Equals(SportsOleReader.GetString(0).Trim())) {
								if(iColorIdx >= 2) iColorIdx = 0;
								else iColorIdx++;
								sLeague = SportsOleReader.GetString(0).Trim();
								HTMLString.Append("<option value=\"\" style=\"color:#");
								HTMLString.Append(arrTextColor[iColorIdx]);
								HTMLString.Append("\">&nbsp;---&nbsp;");
								HTMLString.Append(sLeague);
								HTMLString.Append("&nbsp;---&nbsp;</option>");
							}
							HTMLString.Append("<option value=\"ModifyResultContent.aspx?target=");
							HTMLString.Append(sTarget);
							HTMLString.Append("&id=");
							HTMLString.Append(SportsOleReader.GetInt32(1).ToString());
							HTMLString.Append("\" style=\"color:#");
							HTMLString.Append(arrTextColor[iColorIdx]);
							HTMLString.Append("\">");
							HTMLString.Append(SportsOleReader.GetString(2).Trim());
							HTMLString.Append(" vs ");
							HTMLString.Append(SportsOleReader.GetString(3).Trim());
							HTMLString.Append("</option>");
						}
						SportsOleReader.Close();
						SportsDBMgr.Close();
						break;
					case "gogo2":
						SportsDBMgr.ConnectionString = HttpContext.Current.Application["GOGO2SOCDBConnectionString"].ToString();
						SportsOleReader = SportsDBMgr.ExecuteQuery("select main.CLEAGUE, main.IREC_ID, main.CHOST, main.CGUEST from HK_SPORTS_MASTER main, HK_GOALMENU goal where main.IREC_ID=goal.IREC_ID and main.CHOST<>'-1' and main.CGUEST<>'-1' and main.CGOALCOMMENT='-1' order by main.IREC_ID");
						while(SportsOleReader.Read()) {
							if(!sLeague.Equals(SportsOleReader.GetString(0).Trim())) {
								if(iColorIdx >= 2) iColorIdx = 0;
								else iColorIdx++;
								sLeague = SportsOleReader.GetString(0).Trim();
								HTMLString.Append("<option value=\"\" style=\"color:#");
								HTMLString.Append(arrTextColor[iColorIdx]);
								HTMLString.Append("\">&nbsp;---&nbsp;");
								HTMLString.Append(sLeague);
								HTMLString.Append("&nbsp;---&nbsp;</option>");
							}
							HTMLString.Append("<option value=\"ModifyResultContent.aspx?target=");
							HTMLString.Append(sTarget);
							HTMLString.Append("&id=");
							HTMLString.Append(SportsOleReader.GetInt32(1).ToString());
							HTMLString.Append("\" style=\"color:#");
							HTMLString.Append(arrTextColor[iColorIdx]);
							HTMLString.Append("\">");
							HTMLString.Append(SportsOleReader.GetString(2).Trim());
							HTMLString.Append(" vs ");
							HTMLString.Append(SportsOleReader.GetString(3).Trim());
							HTMLString.Append("</option>");
						}
						SportsOleReader.Close();
						SportsDBMgr.Close();
						break;
					case "hkjc":
						SportsDBMgr.ConnectionString = HttpContext.Current.Application["HKJCSOCConnectionString"].ToString();
						SportsOleReader = SportsDBMgr.ExecuteQuery("select main.CLEAGUE, main.IREC_ID, main.CHOST, main.CGUEST from HK_SPORTS_MASTER main, HK_GOALMENU goal where main.IREC_ID=goal.IREC_ID and main.CHOST<>'-1' and main.CGUEST<>'-1' and main.CGOALCOMMENT='-1' order by main.IREC_ID");
						while(SportsOleReader.Read()) {
							if(!sLeague.Equals(SportsOleReader.GetString(0).Trim())) {
								if(iColorIdx >= 2) iColorIdx = 0;
								else iColorIdx++;
								sLeague = SportsOleReader.GetString(0).Trim();
								HTMLString.Append("<option value=\"\" style=\"color:#");
								HTMLString.Append(arrTextColor[iColorIdx]);
								HTMLString.Append("\">&nbsp;---&nbsp;");
								HTMLString.Append(sLeague);
								HTMLString.Append("&nbsp;---&nbsp;</option>");
							}
							HTMLString.Append("<option value=\"ModifyResultContent.aspx?target=");
							HTMLString.Append(sTarget);
							HTMLString.Append("&id=");
							HTMLString.Append(SportsOleReader.GetInt32(1).ToString());
							HTMLString.Append("\" style=\"color:#");
							HTMLString.Append(arrTextColor[iColorIdx]);
							HTMLString.Append("\">");
							HTMLString.Append(SportsOleReader.GetString(2).Trim());
							HTMLString.Append(" vs ");
							HTMLString.Append(SportsOleReader.GetString(3).Trim());
							HTMLString.Append("</option>");
						}
						SportsOleReader.Close();
						SportsDBMgr.Close();
						break;
					case "jccombo":
						SportsDBMgr.ConnectionString = HttpContext.Current.Application["JCCOMBODBConnectionString"].ToString();
						//SportsOleReader = SportsDBMgr.ExecuteQuery("select main.CLEAGUE, main.CMATCHDAYCODE, main.IMATCHNUM, main.CHOST, main.CGUEST from HK_GOAL_INFO main, HK_GOAL_DETAIL goal where main.CMATCHDAYCODE=goal.CMATCHDAYCODE and main.IMATCHNUM=goal.IMATCHNUM and main.CHOST<>'-1' and main.CGUEST<>'-1' and main.CCOMMENT='-1' group by main.CLEAGUE, main.CMATCHDAYCODE, main.IMATCHNUM, main.CHOST, main.CGUEST order by main.CLEAGUE, main.CMATCHDAYCODE, main.IMATCHNUM");
                                                SportsOleReader = SportsDBMgr.ExecuteQuery("select main.CLEAGUE, main.CMATCHDAYCODE, main.IMATCHNUM, main.CHOST, main.CGUEST from HK_GOAL_INFO main, HK_GOAL_DETAIL goal where main.CMATCHDAYCODE=goal.CMATCHDAYCODE and main.IMATCHNUM=goal.IMATCHNUM and main.CHOST<>'-1' and main.CGUEST<>'-1' group by main.CLEAGUE, main.CMATCHDAYCODE, main.IMATCHNUM, main.CHOST, main.CGUEST order by main.CLEAGUE, main.CMATCHDAYCODE, main.IMATCHNUM");
						while(SportsOleReader.Read()) {
							if(!sLeague.Equals(SportsOleReader.GetString(0).Trim())) {
								if(iColorIdx >= 2) iColorIdx = 0;
								else iColorIdx++;
								sLeague = SportsOleReader.GetString(0).Trim();
								HTMLString.Append("<option value=\"\" style=\"color:#");
								HTMLString.Append(arrTextColor[iColorIdx]);
								HTMLString.Append("\">&nbsp;---&nbsp;");
								HTMLString.Append(sLeague);
								HTMLString.Append("&nbsp;---&nbsp;</option>");
							}
							HTMLString.Append("<option value=\"ModifyResultContent.aspx?target=");
							HTMLString.Append(sTarget);
							HTMLString.Append("&id=");
							String sMatchDayCode = SportsOleReader.GetString(1).Trim();
/*							switch (sMatchDayCode) {
								case "SUN":
									sMatchDayCode = "000";
									break;
								case "MON":
									sMatchDayCode = "001";
									break;
								case "TUE":
									sMatchDayCode = "002";
									break;
								case "WED":
									sMatchDayCode = "003";
									break;
								case "THU":
									sMatchDayCode = "004";
									break;
								case "FRI":
									sMatchDayCode = "005";
									break;
								case "SAT":
									sMatchDayCode = "006";
									break;
								default:
									sMatchDayCode = "-1";
									break;
							}
*/
							HTMLString.Append(sMatchDayCode);
							HTMLString.Append(SportsOleReader.GetInt32(2).ToString());
							HTMLString.Append("\" style=\"color:#");
							HTMLString.Append(arrTextColor[iColorIdx]);
							HTMLString.Append("\">");
							HTMLString.Append(SportsOleReader.GetString(3).Trim());
							HTMLString.Append(" vs ");
							HTMLString.Append(SportsOleReader.GetString(4).Trim());
							HTMLString.Append("</option>");
						}
						SportsOleReader.Close();
						SportsDBMgr.Close();
						break;						
					default:
						SportsDBMgr.ConnectionString = "-1";
						HTMLString.Append("<option value=\"\">選擇錯誤</option>");
						break;
				}
			} catch(Exception ex) {
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " ModifyResultMenu.cs.Show(): " + ex.ToString());
				m_SportsLog.Close();
				HTMLString.Remove(0,HTMLString.Length);
				HTMLString.Append((string)HttpContext.Current.Application["accessErrorMsg"]);
			}

			return HTMLString.ToString();
		}
	}
}