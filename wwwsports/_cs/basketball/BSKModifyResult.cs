/*
Objective:
Modify result information

Last updated:
28 Apr 2004, Chapman Choi

C#.NET complier statement:
csc /t:library /out:..\..\bin\BSKModifyResult.dll /r:..\..\bin\DBManager.dll;..\..\bin\Files.dll BSKModifyResult.cs
*/

using System;
using System.Data.OleDb;
using System.Reflection;
using System.Text;
using System.Web;
using TDL.DB;
using TDL.IO;

[assembly:AssemblyCompany("TDSL")]
[assembly:AssemblyCopyright("(c) 2003 TDSL. All rights reserved. Created on 28 Apr 2004.")]
[assembly:AssemblyDescription("籃球資訊 -> 修改賽果")]
[assembly:AssemblyProduct("Sports.NET")]
[assembly:AssemblyTitle("Sports.NET DLL")]
[assembly:AssemblyVersion("1.0.*")]
namespace SportsUtil {
	public class BSKModifyResult {
		const string LOGFILESUFFIX = "log";
		OleDbDataReader m_SportsOleReader;
		DBManager m_SportsDBMgr;
		Files m_SportsLog;
		StringBuilder HTMLString;

		public BSKModifyResult(string Connection) {
			m_SportsDBMgr = new DBManager();
			m_SportsDBMgr.ConnectionString = Connection;
			m_SportsLog = new Files();
			HTMLString = new StringBuilder();
		}

		public string GetDetails() {
			int iRecordCount = 0;
			int iTotal = 0;
			int iIdx = 0;
			int[] HostScore = new int[5];
			int[] GuestScore = new int[5];
			string sMatchID;
			string[] arrMatchStatus;

			sMatchID = HttpContext.Current.Request.QueryString["MatchID"].Trim();
			arrMatchStatus = (string[])HttpContext.Current.Application["BSKMatchStatusArray"];
			try {
				HTMLString.Append("<input type=\"hidden\" name=\"MatchID\" value=\"");
				HTMLString.Append(sMatchID);
				HTMLString.Append("\">");
				m_SportsOleReader = m_SportsDBMgr.ExecuteQuery("select main.CLEAGUE, main.CHOST, main.CGUEST, score.ISECTIONNO, score.IHOSTSCORE, score.IGUESTSCORE, details.CREMARKS, details.CLV_STATUS from HKMAIN_DETAILS main, HKSCOREDATA_DETAILS score, HKSCORE_DETAILS details where main.IMATCHNO=score.IMATCHNO and main.IMATCHNO=details.IMATCHNO and main.IMATCHNO=" + sMatchID + " order by score.ISECTIONNO");
				while(m_SportsOleReader.Read()) {
					if(iRecordCount == 0) {
						HTMLString.Append("<tr style=\"background-color:#F5F5DC; color:#000080\"><th align=\"right\">賽果:&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;");
						HTMLString.Append(m_SportsOleReader.GetString(0).Trim());
						HTMLString.Append("<input type=\"hidden\" name=\"League\" value=\"");
						HTMLString.Append(m_SportsOleReader.GetString(0).Trim());
						HTMLString.Append("\"></th><th align=\"left\">");
						HTMLString.Append(m_SportsOleReader.GetString(1).Trim());
						HTMLString.Append("<input type=\"hidden\" name=\"Host\" value=\"");
						HTMLString.Append(m_SportsOleReader.GetString(1).Trim());
						HTMLString.Append("\"> vs ");
						HTMLString.Append(m_SportsOleReader.GetString(2).Trim());
						HTMLString.Append("<input type=\"hidden\" name=\"Guest\" value=\"");
						HTMLString.Append(m_SportsOleReader.GetString(2).Trim());
						HTMLString.Append("\">&nbsp;&nbsp;&nbsp;<select name=\"Status\"><option value=\"");
						HTMLString.Append(m_SportsOleReader.GetString(7).Trim());
						HTMLString.Append("\">");
						HTMLString.Append(m_SportsOleReader.GetString(7).Trim());
						foreach(String sItem in arrMatchStatus) {
							if(!sItem.Equals(m_SportsOleReader.GetString(7).Trim())) {
								HTMLString.Append("<option value=" );
								HTMLString.Append(sItem);
								HTMLString.Append(">");
								HTMLString.Append(sItem);
							}
						}
						HTMLString.Append("</th></tr>");
					}
					HTMLString.Append("<tr><th align=\"right\">");
					if(iRecordCount < 4) {
						HTMLString.Append("第");
						HTMLString.Append((iRecordCount+1).ToString());
						HTMLString.Append("節");
					} else {
						HTMLString.Append("加時");
					}
					HTMLString.Append("</th><td><input type=\"hidden\" name=\"SectionNo\" value=\"");
					HTMLString.Append(m_SportsOleReader.GetInt32(3).ToString());
					HTMLString.Append("\"><input name=\"HostScore\" value=\"");
					if(!m_SportsOleReader.GetInt32(4).ToString().Equals("-1")) {
						HostScore[iRecordCount] = m_SportsOleReader.GetInt32(4);
						HTMLString.Append(m_SportsOleReader.GetInt32(4).ToString());
					} else {
						HostScore[iRecordCount] = 0;
					}
					HTMLString.Append("\" maxlength=\"2\" size=\"1\" onChange=\"HostScoreChanged()\"> : <input name=\"GuestScore\" value=\"");
					if(!m_SportsOleReader.GetInt32(5).ToString().Equals("-1")) {
						GuestScore[iRecordCount] = m_SportsOleReader.GetInt32(5);
						HTMLString.Append(m_SportsOleReader.GetInt32(5).ToString());
					} else {
						GuestScore[iRecordCount] = 0;
					}
					HTMLString.Append("\" maxlength=\"2\" size=\"1\" onChange=\"GuestScoreChanged()\"></td></tr>");

					if(iRecordCount > 3) {
						HTMLString.Append("<tr><th align=\"right\">總分</th><td><input name=\"TotalHostScore\" value=\"");
						iTotal = 0;
						for(iIdx = 0; iIdx < HostScore.Length; iIdx++) {
							iTotal += HostScore[iIdx];
						}
						HTMLString.Append(iTotal.ToString());
						HTMLString.Append("\" maxlength=\"2\" size=\"1\" style=\"background-color:#DCDCDC\" readonly> : <input name=\"TotalGuestScore\" value=\"");
						iTotal = 0;
						for(iIdx = 0; iIdx < GuestScore.Length; iIdx++) {
							iTotal += GuestScore[iIdx];
						}
						HTMLString.Append(iTotal.ToString());
						HTMLString.Append("\" maxlength=\"2\" size=\"1\" style=\"background-color:#DCDCDC\" readonly></td></tr>");

						HTMLString.Append("<tr><th align=\"right\">備註</th><td align=\"left\"><input name=\"remark\" value=\"");
						if(!m_SportsOleReader.GetString(6).Trim().Equals("-1")) {
							HTMLString.Append(m_SportsOleReader.GetString(6).Trim());
						}
						HTMLString.Append("\" maxlength=\"9\" size=\"10\"></td></tr>");
					}

					iRecordCount++;
				}
				m_SportsOleReader.Close();
				m_SportsDBMgr.Close();
			} catch(Exception ex) {
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " BSKModifyResult.cs.GetDetails(): " + ex.ToString());
				m_SportsLog.Close();
				HTMLString.Remove(0, HTMLString.Length);
				HTMLString.Append((string)HttpContext.Current.Application["accessErrorMsg"]);
			}

			return HTMLString.ToString();
		}

		public int Modify() {
			int iRecUpd = 0;
			string sMatchID;
			string sLeague;
			string sHost;
			string sGuest;
			string sRemark;
			string sStatus;
			char[] delimiter = new char[] {','};
			string[] arrSectionNo;
			string[] arrHostScore;
			string[] arrGuestScore;
			StringBuilder SQLString = new StringBuilder();
			string sHostScore;
			string sGuestScore;
			string sTotalHostScore;
			string sTotalGuestScore;
			string sMenuMessage = "";
			string sResultMessage = "";
			bool bShowed = false;
			int iIndex = 0;

			sMatchID = HttpContext.Current.Request.Form["MatchID"];
			sLeague = HttpContext.Current.Request.Form["League"];
			sHost = HttpContext.Current.Request.Form["Host"];
			sGuest = HttpContext.Current.Request.Form["Guest"];
			sTotalHostScore = HttpContext.Current.Request.Form["TotalHostScore"];
			sTotalGuestScore = HttpContext.Current.Request.Form["TotalGuestScore"];
			sStatus = HttpContext.Current.Request.Form["Status"];			
			sRemark = HttpContext.Current.Request.Form["remark"].Trim();
			if(sRemark.Trim().Equals("")) {
				sRemark = "-1";
			}

			try {
				arrSectionNo = HttpContext.Current.Request.Form["SectionNo"].Split(delimiter);
			} catch(Exception) {
				arrSectionNo = new string[0];
			}

			try {
				arrHostScore = HttpContext.Current.Request.Form["HostScore"].Split(delimiter);
			} catch(Exception) {
				arrHostScore = new string[0];
			}

			try {
				arrGuestScore = HttpContext.Current.Request.Form["GuestScore"].Split(delimiter);
			} catch(Exception) {
				arrGuestScore = new string[0];
			}

			try {
				//Update to GOGO NBA and HKJC NBA
				DBManager HKJCDBMgr = new DBManager();
				HKJCDBMgr.ConnectionString = (string)HttpContext.Current.Application["HKJCNBAConnectionString"];

				SQLString.Append("update HKSCORE_DETAILS set CREMARKS='");
				SQLString.Append(sRemark);
				SQLString.Append("', CLV_STATUS='");
				SQLString.Append(sStatus);
				SQLString.Append("' where IMATCHNO=");
				SQLString.Append(sMatchID);
				m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
				m_SportsDBMgr.Close();
				HKJCDBMgr.ExecuteNonQuery(SQLString.ToString());
				HKJCDBMgr.Close();

				for(int i = 0; i < arrSectionNo.Length; i++) {
					SQLString.Remove(0, SQLString.Length);
					SQLString.Append("update HKSCOREDATA_DETAILS set IHOSTSCORE=");
					if(!arrHostScore[i].Trim().Equals("")) {
						SQLString.Append(arrHostScore[i].Trim());
					} else {
						SQLString.Append("-1");
					}
					SQLString.Append(", IGUESTSCORE= ");
					if(!arrGuestScore[i].Trim().Equals("")) {
						SQLString.Append(arrGuestScore[i].Trim());
					} else {
						SQLString.Append("-1");
					}
					SQLString.Append(" where IMATCHNO=");
					SQLString.Append(sMatchID);
					SQLString.Append(" and ISECTIONNO=");
					SQLString.Append(arrSectionNo[i].Trim());
					m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
					m_SportsDBMgr.Close();
					HKJCDBMgr.ExecuteNonQuery(SQLString.ToString());
					HKJCDBMgr.Close();
				}
				
				//Update to Combo NBA
				DBManager ComboDBMgr = new DBManager();
				ComboDBMgr.ConnectionString = (string)HttpContext.Current.Application["JCCOMBODBConnectionString"];
				
				SQLString.Remove(0,SQLString.Length);
				SQLString.Append("select CMENU, CMESSAGE from NBA_RESULT where CLEAGUE='"+sLeague+"' and CHOST='"+sHost+"' and CGUEST='"+sGuest+"'");
				m_SportsOleReader = ComboDBMgr.ExecuteQuery(SQLString.ToString());
				if (m_SportsOleReader.Read()) {
					if(!m_SportsOleReader.IsDBNull(0)) {
						sResultMessage = m_SportsOleReader.GetString(1).Trim();
						sMenuMessage = m_SportsOleReader.GetString(0).Trim();
					}
				}				
				m_SportsOleReader.Close();				
				ComboDBMgr.Close();
				
				if (sResultMessage != "" && (sStatus == "腰斬" || sStatus == "完場")) {
					sMenuMessage = sHost+" "+sTotalHostScore.PadLeft(3, ' ')+" "+sGuest+" "+sTotalGuestScore.PadLeft(3, ' ')+sStatus;					
					iIndex = sResultMessage.IndexOf("第1節");
					sResultMessage = sResultMessage.Substring(0, iIndex-1);
					for (iIndex=0; iIndex<4; iIndex++) {
						if(!arrHostScore[iIndex].Trim().Equals("") && !arrGuestScore[iIndex].Trim().Equals("")) {
							sResultMessage += "\n";
							sResultMessage += "第"+(iIndex+1).ToString()+"節";
							sResultMessage += "  ";
							sHostScore = arrHostScore[iIndex].Trim();							
							sResultMessage += sHostScore.PadLeft(2, ' ');
							sResultMessage += ":";
							sGuestScore = arrGuestScore[iIndex].Trim();
							sResultMessage += sGuestScore;														
						} else {
							if (sStatus == "腰斬" && !bShowed) {
								sResultMessage += " (";
								sResultMessage += sStatus;
								sResultMessage += ")";
								bShowed = true;
							}
							sResultMessage += "\n";
							sResultMessage += "第"+(iIndex+1).ToString()+"節";
						}						
					}
					
					if(!arrHostScore[4].Trim().Equals("") && !arrGuestScore[4].Trim().Equals("")) {
						sResultMessage += "\n";
						sResultMessage += " 加時";
						sResultMessage += "  ";
						sHostScore = arrHostScore[4].Trim();
						sResultMessage += sHostScore.PadLeft(2, ' ');
						sResultMessage += ":";
						sGuestScore = arrGuestScore[4].Trim();
						sResultMessage += sGuestScore;						
						if (sStatus == "腰斬" && !bShowed) {
							sResultMessage += " (";
							sResultMessage += sStatus;
							sResultMessage += ")";
							bShowed = true;
						}
					} else {
						if (sStatus == "腰斬" && !bShowed) {
							sResultMessage += " (";
							sResultMessage += sStatus;
							sResultMessage += ")";
							bShowed = true;
						}
						sResultMessage += "\n";
						sResultMessage += " 加時";
					}
					sResultMessage += "\n";
					sResultMessage += " 總分  "+sTotalHostScore+":"+sTotalGuestScore;
					
					SQLString.Remove(0,SQLString.Length);
					SQLString.Append("update NBA_RESULT set CMENU='"+sMenuMessage+"', CMESSAGE='"+sResultMessage+"' where CLEAGUE='"+sLeague+"' and CHOST='"+sHost+"' and CGUEST='"+sGuest+"'");
					ComboDBMgr.ExecuteNonQuery(SQLString.ToString());
					ComboDBMgr.Close();
				}
				ComboDBMgr.Dispose();

				m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " BSKModifyResult.cs: Modify result [L:" + sLeague + ", H:" + sHost + ", G:" + sGuest + "] (" + HttpContext.Current.Session["user_name"] + ")");
				m_SportsLog.Close();

				iRecUpd++;
			}	catch(Exception ex) {
				iRecUpd = -1;
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " BSKModifyResult.cs.Modify(): " + ex.ToString());
				m_SportsLog.Close();
			}

			return iRecUpd;
		}
	}
}