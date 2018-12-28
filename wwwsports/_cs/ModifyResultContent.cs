/*
Objective:
Details of housekeeped match

Last updated:
30 Apr 2004, Chapman Choi

C#.NET complier statement:
csc /t:library /out:..\bin\ModifyResultContent.dll /r:..\bin\DBManager.dll;..\bin\Files.dll ModifyResultContent.cs
*/

using System;
using System.Collections.Specialized;
using System.Data.OleDb;
using System.Reflection;
using System.Text;
using System.Web;
using TDL.DB;
using TDL.IO;

[assembly:AssemblyCompany("TDSL")]
[assembly:AssemblyCopyright("(c) 2004 TDSL. All rights reserved. Created on 28 Apr 2004.")]
[assembly:AssemblyDescription("足球資訊 -> 修改賽果")]
[assembly:AssemblyProduct("Sports.NET")]
[assembly:AssemblyTitle("Sports.NET DLL")]
[assembly:AssemblyVersion("1.0.*")]
namespace SportsUtil {
	public class ModifyResultContent {
		const int TOTALRECORDS = 12;
		const string LOGFILESUFFIX = "log";
		Files m_SportsLog;
		NameValueCollection matchStatusNVC;
		NameValueCollection matchTimeNVC;

		public ModifyResultContent() {
			m_SportsLog = new Files();
			matchStatusNVC = (NameValueCollection)HttpContext.Current.Application["matchStatusItems"];
			matchTimeNVC = (NameValueCollection)HttpContext.Current.Application["matchTimeItems"];
		}

		public string GetDetails() {
			int iRecordIdx = 1;
			string sTarget;
			string sMatchID;
			string[] arrMatchStatus;
			DBManager SportsDBMgr;
			OleDbDataReader SportsOleReader;
			StringBuilder HTMLString = new StringBuilder();

			sTarget = HttpContext.Current.Request.QueryString["target"].Trim();
			sMatchID = HttpContext.Current.Request.QueryString["id"].Trim();
			arrMatchStatus = (string[])HttpContext.Current.Application["matchItemsArray"];
			try {
				HTMLString.Append("<input type=\"hidden\" name=\"target\" value=\"");
				HTMLString.Append(sTarget);
				HTMLString.Append("\"><input type=\"hidden\" name=\"id\" value=\"");
				HTMLString.Append(sMatchID);
				HTMLString.Append("\">");

				SportsDBMgr = new DBManager();
				switch(sTarget) {
					case "gogo1":
						SportsDBMgr.ConnectionString = HttpContext.Current.Application["SoccerDBConnectionString"].ToString();
						SportsOleReader = SportsDBMgr.ExecuteQuery("select main.CLEAGUE, main.CHOST, main.IH_GOAL, main.CGUEST, main.IG_GOAL, main.CSTATUS, res.CSTATUS, main.CCOMMENT, details.CSTATUS, details.CREDFLAG, details.ITIME, details.CGOAL, details.CPLAYER from HKGOAL_DETAILS main INNER JOIN HKRES_DETAILS res ON main.CLEAGUE=res.CLEAGUE and main.CHOST=res.CHOST and main.CGUEST=res.CGUEST LEFT OUTER JOIN HKSCORE_DETAILS details ON main.CLEAGUE=details.CLEAGUE and main.CHOST=details.CHOST and main.CGUEST=details.CGUEST and main.IHEADER_ID=" + sMatchID + " order by details.ISEQ_NO");
						while(SportsOleReader.Read()) {
							if(iRecordIdx == 1) {
								//League, Host, Guest and their total score
								HTMLString.Append("<tr style=\"background-color:#E6E9EF; color:#000080\"><th colspan=\"2\" align=\"left\">GOGO1賽果：");

								//Match Status in details
								HTMLString.Append("&nbsp;&nbsp;<select name=\"DetailsStatus\">");
								if(!SportsOleReader.GetString(6).Trim().Equals("-1")) {
									HTMLString.Append("<option value=\"");
									HTMLString.Append(SportsOleReader.GetString(6).Trim());
									HTMLString.Append("\">");
									HTMLString.Append(matchStatusNVC.Get(SportsOleReader.GetString(6).Trim()));
									foreach(String sItem in matchStatusNVC.AllKeys) {
										if(!sItem.Equals(SportsOleReader.GetString(6).Trim())) {
											HTMLString.Append("<option value=" );
											HTMLString.Append(sItem);
											HTMLString.Append(">");
											HTMLString.Append(matchStatusNVC.Get(sItem));
										}
									}
								} else {
									HTMLString.Append("<option value=\"Y\">");
									HTMLString.Append(matchStatusNVC.Get("Y"));
									foreach(String sItem in matchStatusNVC.AllKeys) {
										if(!sItem.Equals("Y")) {
											HTMLString.Append("<option value=" );
											HTMLString.Append(sItem);
											HTMLString.Append(">");
											HTMLString.Append(matchStatusNVC.Get(sItem));
										}
									}
								}
								HTMLString.Append("</select></th>");

								HTMLString.Append("<th colspan=\"4\">");
								HTMLString.Append(SportsOleReader.GetString(0).Trim());
								HTMLString.Append("<input type=\"hidden\" name=\"League\" value=\"");
								HTMLString.Append(SportsOleReader.GetString(0).Trim());
								HTMLString.Append("\"> - ");
								HTMLString.Append(SportsOleReader.GetString(1).Trim());
								HTMLString.Append("<input type=\"hidden\" name=\"Host\" value=\"");
								HTMLString.Append(SportsOleReader.GetString(1).Trim());
								HTMLString.Append("\"><input name=\"HostTotalGoal\" value=\"");
								HTMLString.Append(SportsOleReader.GetInt32(2).ToString());
								HTMLString.Append("\" maxlength=\"2\" size=\"1\"> vs ");
								HTMLString.Append(SportsOleReader.GetString(3).Trim());
								HTMLString.Append("<input type=\"hidden\" name=\"Guest\" value=\"");
								HTMLString.Append(SportsOleReader.GetString(3).Trim());
								HTMLString.Append("\"><input name=\"GuestTotalGoal\" value=\"");
								HTMLString.Append(SportsOleReader.GetInt32(4).ToString());
								HTMLString.Append("\" maxlength=\"2\" size=\"1\">");

								//Match Status in menu
								HTMLString.Append("&nbsp;&nbsp;<select name=\"MenuStatus\">");
								foreach(String sItem in arrMatchStatus) {
									if(sItem.Substring(0,1).Equals(SportsOleReader.GetString(5).Trim())) {
										HTMLString.Append("<option value=\"");
										HTMLString.Append(SportsOleReader.GetString(5).Trim());
										HTMLString.Append("\">");
										HTMLString.Append(sItem);
										break;
									}
								}
								foreach(String sItem in arrMatchStatus) {
									if(!sItem.Substring(0,1).Equals(SportsOleReader.GetString(5).Trim())) {
										HTMLString.Append("<option value=\"");
										HTMLString.Append(sItem.Substring(0,1));
										HTMLString.Append("\">");
										HTMLString.Append(sItem);
									}
								}
								HTMLString.Append("</select></th></tr>");

/*
								//Remark
								HTMLString.Append("<th colspan=\"4\">備註:&nbsp;<input name=\"remark\" value=\"");
								if(!SportsOleReader.IsDBNull(7)) {
									if(!SportsOleReader.GetString(7).Trim().Equals("-1")) {
										HTMLString.Append(SportsOleReader.GetString(7).Trim());
									}
								}
								HTMLString.Append("\" maxlength=\"9\" size=\"10\"></th></tr>");
*/
								//Column header
								HTMLString.Append("<tr style=\"background-color:#E6E9EF; color:#000080\"><th>時段</th><th>入球 / 紅牌</th><th>時間</th><th>主隊比數</th><th>客隊比數</th><th>球員 / 球隊</th></tr>");
							}

							//Goal Details
							//Match Period
							HTMLString.Append("<tr align=\"center\"><td>");
							HTMLString.Append(iRecordIdx.ToString());
							HTMLString.Append(".&nbsp;<select name=\"MatchPeriod\">");
							if(!SportsOleReader.IsDBNull(8)) {
								if(!SportsOleReader.GetString(8).Trim().Equals("-1")) {
									HTMLString.Append("<option value=\"");
									HTMLString.Append(SportsOleReader.GetString(8).Trim());
									HTMLString.Append("\">");
									HTMLString.Append(matchTimeNVC.Get(SportsOleReader.GetString(8).Trim()));
									foreach(String sItem in matchTimeNVC.AllKeys) {
										if(!sItem.Equals(SportsOleReader.GetString(8).Trim())) {
											HTMLString.Append("<option value=" );
											HTMLString.Append(sItem);
											HTMLString.Append(">");
											HTMLString.Append(matchTimeNVC.Get(sItem));
										}
									}
								} else {
									foreach(String sItem in matchTimeNVC.AllKeys) {
										HTMLString.Append("<option value=" );
										HTMLString.Append(sItem);
										HTMLString.Append(">");
										HTMLString.Append(matchTimeNVC.Get(sItem));
									}
								}
							} else {
								foreach(String sItem in matchTimeNVC.AllKeys) {
									HTMLString.Append("<option value=" );
									HTMLString.Append(sItem);
									HTMLString.Append(">");
									HTMLString.Append(matchTimeNVC.Get(sItem));
								}
							}
							HTMLString.Append("</select></td>");

							//Score / Red Card Flag
							HTMLString.Append("<td><select name=\"ScoreFlag\">");
							if(!SportsOleReader.IsDBNull(9)) {
								if(SportsOleReader.GetString(9).Trim().Equals("R")) {
									HTMLString.Append("<option value=\"R\">紅牌");
									HTMLString.Append("<option value=\"S\">入球");
								} else {
									HTMLString.Append("<option value=\"S\">入球");
									HTMLString.Append("<option value=\"R\">紅牌");
								}
							} else {
								HTMLString.Append("<option value=\"S\">入球");
								HTMLString.Append("<option value=\"R\">紅牌");
							}
							HTMLString.Append("</select></td>");

							//Match Time per record
							HTMLString.Append("<td><input name=\"MatchTime\" value=\"");
							if(!SportsOleReader.IsDBNull(10)) {
								if(SportsOleReader.GetInt32(10) != -1) {
									HTMLString.Append(SportsOleReader.GetInt32(10).ToString());
								}
							}
							HTMLString.Append("\" maxlength=\"3\" size=\"2\"></td>");

							//Host score
							HTMLString.Append("<td><input name=\"HostGoal\" value=\"");
							if(!SportsOleReader.IsDBNull(11)) {
								if(!SportsOleReader.GetString(11).Trim().Equals("-1")) {
									HTMLString.Append(Convert.ToInt32(SportsOleReader.GetString(11).Trim().Substring(0,2)).ToString());
								}
							}
							HTMLString.Append("\" maxlength=\"2\" size=\"1\"></td>");

							//Guest score
							HTMLString.Append("<td><input name=\"GuestGoal\" value=\"");
							if(!SportsOleReader.IsDBNull(11)) {
								if(!SportsOleReader.GetString(11).Trim().Equals("-1")) {
									HTMLString.Append(Convert.ToInt32(SportsOleReader.GetString(11).Trim().Substring(2)).ToString());
								}
							}
							HTMLString.Append("\" maxlength=\"2\" size=\"1\"></td>");

							//Player / Remark
							HTMLString.Append("<td><input name=\"Player\" value=\"");
							if(!SportsOleReader.IsDBNull(12)) {
								if(!SportsOleReader.GetString(12).Trim().Equals("-1")) {
									HTMLString.Append(SportsOleReader.GetString(12).Trim());
								}
							}
							HTMLString.Append("\" maxlength=\"5\" size=\"8\"></td>");
							HTMLString.Append("</tr>");

							iRecordIdx++;
						}
						SportsOleReader.Close();
						SportsDBMgr.Close();
						
						if(iRecordIdx == 1) {	//Empty GoalDetails record
							HTMLString.Append("<tr style=\"background-color:#E6E9EF; color:#FF0000\"><th align=\"center\" colspan=\"6\">沒有輸入比數詳情，不能修改賽果！</th></tr>");
						} else {
							//Print the rest entries
							for(; iRecordIdx <= TOTALRECORDS; iRecordIdx++) {
								//Match Period
								HTMLString.Append("<tr align=\"center\"><td>");
								HTMLString.Append(iRecordIdx.ToString());
								HTMLString.Append(".&nbsp;<select name=\"MatchPeriod\">");
								foreach(String sItem in matchTimeNVC.AllKeys) {
									HTMLString.Append("<option value=" );
									HTMLString.Append(sItem);
									HTMLString.Append(">");
									HTMLString.Append(matchTimeNVC.Get(sItem));
								}
								HTMLString.Append("</select></td>");

								//Score / Red Card Flag
								HTMLString.Append("<td><select name=\"ScoreFlag\"><option value=\"S\">入球<option value=\"R\">紅牌</select></td>");

								//Match Time per record
								HTMLString.Append("<td><input name=\"MatchTime\" value=\"\" maxlength=\"3\" size=\"2\"></td>");

								//Host score
								HTMLString.Append("<td><input name=\"HostGoal\" value=\"\" maxlength=\"2\" size=\"1\"></td>");

								//Guest score
								HTMLString.Append("<td><input name=\"GuestGoal\" value=\"\" maxlength=\"2\" size=\"1\"></td>");

								//Player / Remark
								HTMLString.Append("<td><input name=\"Player\" value=\"\" maxlength=\"5\" size=\"8\"></td>");
								HTMLString.Append("</tr>");
							}
						}
						break;

					case "gogo2":
						SportsDBMgr.ConnectionString = HttpContext.Current.Application["GOGO2SOCDBConnectionString"].ToString();
						SportsOleReader = SportsDBMgr.ExecuteQuery("select main.CLEAGUE, main.CHOST, menu.IHOST_SCORE, main.CGUEST, menu.IGUEST_SCORE, menu.CSTATUS, details.CSTATUS, menu.CCOMMENT, details.CSCORESTATUS, details.CREDFLAG, details.ITIME, details.IHOST_SCORE, details.IGUEST_SCORE, details.CPLAYER from HK_SPORTS_MASTER main, HK_GOALMENU menu, HK_GOALDETAILS details where main.IREC_ID=menu.IREC_ID and main.IREC_ID=details.IREC_ID and main.IREC_ID=" + sMatchID + " order by details.ISEQ_NO");
						while(SportsOleReader.Read()) {
							if(iRecordIdx == 1) {
								//League, Host, Guest and their total score
								HTMLString.Append("<tr style=\"background-color:#E6E9EF; color:#000080\"><th colspan=\"2\" align=\"left\">GOGO2賽果：");

								//Match Status in details
								HTMLString.Append("&nbsp;&nbsp;<select name=\"DetailsStatus\">");
								if(!SportsOleReader.GetString(6).Trim().Equals("-1")) {
									HTMLString.Append("<option value=\"");
									HTMLString.Append(SportsOleReader.GetString(6).Trim());
									HTMLString.Append("\">");
									HTMLString.Append(matchStatusNVC.Get(SportsOleReader.GetString(6).Trim()));
									foreach(String sItem in matchStatusNVC.AllKeys) {
										if(!sItem.Equals(SportsOleReader.GetString(6).Trim())) {
											HTMLString.Append("<option value=" );
											HTMLString.Append(sItem);
											HTMLString.Append(">");
											HTMLString.Append(matchStatusNVC.Get(sItem));
										}
									}
								} else {
									HTMLString.Append("<option value=\"Y\">");
									HTMLString.Append(matchStatusNVC.Get("Y"));
									foreach(String sItem in matchStatusNVC.AllKeys) {
										if(!sItem.Equals("Y")) {
											HTMLString.Append("<option value=" );
											HTMLString.Append(sItem);
											HTMLString.Append(">");
											HTMLString.Append(matchStatusNVC.Get(sItem));
										}
									}
								}
								HTMLString.Append("</select></th>");

								HTMLString.Append("<th colspan=\"4\">");
								HTMLString.Append(SportsOleReader.GetString(0).Trim());
								HTMLString.Append("<input type=\"hidden\" name=\"League\" value=\"");
								HTMLString.Append(SportsOleReader.GetString(0).Trim());
								HTMLString.Append("\"> - ");
								HTMLString.Append(SportsOleReader.GetString(1).Trim());
								HTMLString.Append("<input type=\"hidden\" name=\"Host\" value=\"");
								HTMLString.Append(SportsOleReader.GetString(1).Trim());
								HTMLString.Append("\"><input name=\"HostTotalGoal\" value=\"");
								HTMLString.Append(SportsOleReader.GetInt32(2).ToString());
								HTMLString.Append("\" maxlength=\"2\" size=\"1\"> vs ");
								HTMLString.Append(SportsOleReader.GetString(3).Trim());
								HTMLString.Append("<input type=\"hidden\" name=\"Guest\" value=\"");
								HTMLString.Append(SportsOleReader.GetString(3).Trim());
								HTMLString.Append("\"><input name=\"GuestTotalGoal\" value=\"");
								HTMLString.Append(SportsOleReader.GetInt32(4).ToString());
								HTMLString.Append("\" maxlength=\"2\" size=\"1\">");

								//Match Status in menu
								HTMLString.Append("&nbsp;&nbsp;<select name=\"MenuStatus\">");
								foreach(String sItem in arrMatchStatus) {
									if(sItem.Substring(0,1).Equals(SportsOleReader.GetString(5).Trim())) {
										HTMLString.Append("<option value=\"");
										HTMLString.Append(SportsOleReader.GetString(5).Trim());
										HTMLString.Append("\">");
										HTMLString.Append(sItem);
										break;
									}
								}
								foreach(String sItem in arrMatchStatus) {
									if(!sItem.Substring(0,1).Equals(SportsOleReader.GetString(5).Trim())) {
										HTMLString.Append("<option value=\"");
										HTMLString.Append(sItem.Substring(0,1));
										HTMLString.Append("\">");
										HTMLString.Append(sItem);
									}
								}
								HTMLString.Append("</select></th></tr>");

/*
								//Remark
								HTMLString.Append("<th colspan=\"4\">備註:&nbsp;<input name=\"remark\" value=\"");
								if(!SportsOleReader.IsDBNull(7)) {
									if(!SportsOleReader.GetString(7).Trim().Equals("-1")) {
										HTMLString.Append(SportsOleReader.GetString(7).Trim());
									}
								}
								HTMLString.Append("\" maxlength=\"9\" size=\"10\"></th></tr>");
*/
								//Column header
								HTMLString.Append("<tr style=\"background-color:#E6E9EF; color:#000080\"><th>時段</th><th>入球 / 紅牌</th><th>時間</th><th>主隊比數</th><th>客隊比數</th><th>球員 / 球隊</th></tr>");
							}

							//Goal Details
							//Match Period
							HTMLString.Append("<tr align=\"center\"><td>");
							HTMLString.Append(iRecordIdx.ToString());
							HTMLString.Append(".&nbsp;<select name=\"MatchPeriod\">");
							if(!SportsOleReader.GetString(8).Trim().Equals("-1")) {
								HTMLString.Append("<option value=\"");
								HTMLString.Append(SportsOleReader.GetString(8).Trim());
								HTMLString.Append("\">");
								HTMLString.Append(matchTimeNVC.Get(SportsOleReader.GetString(8).Trim()));
								foreach(String sItem in matchTimeNVC.AllKeys) {
									if(!sItem.Equals(SportsOleReader.GetString(8).Trim())) {
										HTMLString.Append("<option value=" );
										HTMLString.Append(sItem);
										HTMLString.Append(">");
										HTMLString.Append(matchTimeNVC.Get(sItem));
									}
								}
							} else {
								foreach(String sItem in matchTimeNVC.AllKeys) {
									HTMLString.Append("<option value=" );
									HTMLString.Append(sItem);
									HTMLString.Append(">");
									HTMLString.Append(matchTimeNVC.Get(sItem));
								}
							}
							HTMLString.Append("</select></td>");

							//Score / Red Card Flag
							HTMLString.Append("<td><select name=\"ScoreFlag\">");
							if(SportsOleReader.GetString(9).Trim().Equals("R")) {
								HTMLString.Append("<option value=\"R\">紅牌");
								HTMLString.Append("<option value=\"S\">入球");
							} else {
								HTMLString.Append("<option value=\"S\">入球");
								HTMLString.Append("<option value=\"R\">紅牌");
							}
							HTMLString.Append("</select></td>");

							//Match Time per record
							HTMLString.Append("<td><input name=\"MatchTime\" value=\"");
							if(SportsOleReader.GetInt32(10) != -1) {
								HTMLString.Append(SportsOleReader.GetInt32(10).ToString());
							}
							HTMLString.Append("\" maxlength=\"3\" size=\"2\"></td>");

							//Host score
							HTMLString.Append("<td><input name=\"HostGoal\" value=\"");
							if(SportsOleReader.GetInt32(11) != -1) {
								HTMLString.Append(SportsOleReader.GetInt32(11).ToString());
							}
							HTMLString.Append("\" maxlength=\"2\" size=\"1\"></td>");

							//Guest score
							HTMLString.Append("<td><input name=\"GuestGoal\" value=\"");
							if(SportsOleReader.GetInt32(12) != -1) {
								HTMLString.Append(SportsOleReader.GetInt32(12).ToString());
							}
							HTMLString.Append("\" maxlength=\"2\" size=\"1\"></td>");

							//Player / Remark
							HTMLString.Append("<td><input name=\"Player\" value=\"");
							if(!SportsOleReader.GetString(13).Trim().Equals("-1")) {
								HTMLString.Append(SportsOleReader.GetString(13).Trim());
							}
							HTMLString.Append("\" maxlength=\"5\" size=\"8\"></td>");
							HTMLString.Append("</tr>");

							iRecordIdx++;
						}
						SportsOleReader.Close();
						SportsDBMgr.Close();
						
						if(iRecordIdx == 1) {	//Empty GoalDetails record
							HTMLString.Append("<tr style=\"background-color:#E6E9EF; color:#FF0000\"><th align=\"center\" colspan=\"6\">沒有輸入比數詳情，不能修改賽果！</th></tr>");
						} else {
							//Print the rest entries
							for(; iRecordIdx <= TOTALRECORDS; iRecordIdx++) {
								//Match Period
								HTMLString.Append("<tr align=\"center\"><td>");
								HTMLString.Append(iRecordIdx.ToString());
								HTMLString.Append(".&nbsp;<select name=\"MatchPeriod\">");
								foreach(String sItem in matchTimeNVC.AllKeys) {
									HTMLString.Append("<option value=" );
									HTMLString.Append(sItem);
									HTMLString.Append(">");
									HTMLString.Append(matchTimeNVC.Get(sItem));
								}
								HTMLString.Append("</select></td>");

								//Score / Red Card Flag
								HTMLString.Append("<td><select name=\"ScoreFlag\"><option value=\"S\">入球<option value=\"R\">紅牌</select></td>");

								//Match Time per record
								HTMLString.Append("<td><input name=\"MatchTime\" value=\"\" maxlength=\"3\" size=\"2\"></td>");

								//Host score
								HTMLString.Append("<td><input name=\"HostGoal\" value=\"\" maxlength=\"2\" size=\"1\"></td>");

								//Guest score
								HTMLString.Append("<td><input name=\"GuestGoal\" value=\"\" maxlength=\"2\" size=\"1\"></td>");

								//Player / Remark
								HTMLString.Append("<td><input name=\"Player\" value=\"\" maxlength=\"5\" size=\"8\"></td>");
								HTMLString.Append("</tr>");
							}
						}
						break;

					case "hkjc":
						SportsDBMgr.ConnectionString = HttpContext.Current.Application["HKJCSOCConnectionString"].ToString();
						SportsOleReader = SportsDBMgr.ExecuteQuery("select main.CLEAGUE, main.CHOST, menu.IHOST_SCORE, main.CGUEST, menu.IGUEST_SCORE, menu.CSTATUS, details.CSTATUS, menu.CCOMMENT, details.CSCORESTATUS, details.CREDFLAG, details.ITIME, details.IHOST_SCORE, details.IGUEST_SCORE, details.CPLAYER from HK_SPORTS_MASTER main, HK_GOALMENU menu, HK_GOALDETAILS details where main.IREC_ID=menu.IREC_ID and main.IREC_ID=details.IREC_ID and main.IREC_ID=" + sMatchID + " order by details.ISEQ_NO");
						while(SportsOleReader.Read()) {
							if(iRecordIdx == 1) {
								//League, Host, Guest and their total score
								HTMLString.Append("<tr style=\"background-color:#E6E9EF; color:#000080\"><th colspan=\"2\" align=\"left\">馬會機賽果：");

								//Match Status in details
								HTMLString.Append("&nbsp;&nbsp;<select name=\"DetailsStatus\">");
								if(!SportsOleReader.GetString(6).Trim().Equals("-1")) {
									HTMLString.Append("<option value=\"");
									HTMLString.Append(SportsOleReader.GetString(6).Trim());
									HTMLString.Append("\">");
									HTMLString.Append(matchStatusNVC.Get(SportsOleReader.GetString(6).Trim()));
									foreach(String sItem in matchStatusNVC.AllKeys) {
										if(!sItem.Equals(SportsOleReader.GetString(6).Trim())) {
											HTMLString.Append("<option value=" );
											HTMLString.Append(sItem);
											HTMLString.Append(">");
											HTMLString.Append(matchStatusNVC.Get(sItem));
										}
									}
								} else {
									HTMLString.Append("<option value=\"Y\">");
									HTMLString.Append(matchStatusNVC.Get("Y"));
									foreach(String sItem in matchStatusNVC.AllKeys) {
										if(!sItem.Equals("Y")) {
											HTMLString.Append("<option value=" );
											HTMLString.Append(sItem);
											HTMLString.Append(">");
											HTMLString.Append(matchStatusNVC.Get(sItem));
										}
									}
								}
								HTMLString.Append("</select></th>");

								HTMLString.Append("<th colspan=\"4\">");
								HTMLString.Append(SportsOleReader.GetString(0).Trim());
								HTMLString.Append("<input type=\"hidden\" name=\"League\" value=\"");
								HTMLString.Append(SportsOleReader.GetString(0).Trim());
								HTMLString.Append("\"> - ");
								HTMLString.Append(SportsOleReader.GetString(1).Trim());
								HTMLString.Append("<input type=\"hidden\" name=\"Host\" value=\"");
								HTMLString.Append(SportsOleReader.GetString(1).Trim());
								HTMLString.Append("\"><input name=\"HostTotalGoal\" value=\"");
								HTMLString.Append(SportsOleReader.GetInt32(2).ToString());
								HTMLString.Append("\" maxlength=\"2\" size=\"1\"> vs ");
								HTMLString.Append(SportsOleReader.GetString(3).Trim());
								HTMLString.Append("<input type=\"hidden\" name=\"Guest\" value=\"");
								HTMLString.Append(SportsOleReader.GetString(3).Trim());
								HTMLString.Append("\"><input name=\"GuestTotalGoal\" value=\"");
								HTMLString.Append(SportsOleReader.GetInt32(4).ToString());
								HTMLString.Append("\" maxlength=\"2\" size=\"1\">");

								//Match Status in menu
								HTMLString.Append("&nbsp;&nbsp;<select name=\"MenuStatus\">");
								foreach(String sItem in arrMatchStatus) {
									if(sItem.Substring(0,1).Equals(SportsOleReader.GetString(5).Trim())) {
										HTMLString.Append("<option value=\"");
										HTMLString.Append(SportsOleReader.GetString(5).Trim());
										HTMLString.Append("\">");
										HTMLString.Append(sItem);
										break;
									}
								}
								foreach(String sItem in arrMatchStatus) {
									if(!sItem.Substring(0,1).Equals(SportsOleReader.GetString(5).Trim())) {
										HTMLString.Append("<option value=\"");
										HTMLString.Append(sItem.Substring(0,1));
										HTMLString.Append("\">");
										HTMLString.Append(sItem);
									}
								}
								HTMLString.Append("</select></th></tr>");

/*
								//Remark
								HTMLString.Append("<th colspan=\"4\">備註:&nbsp;<input name=\"remark\" value=\"");
								if(!SportsOleReader.IsDBNull(7)) {
									if(!SportsOleReader.GetString(7).Trim().Equals("-1")) {
										HTMLString.Append(SportsOleReader.GetString(7).Trim());
									}
								}
								HTMLString.Append("\" maxlength=\"9\" size=\"10\"></th></tr>");
*/
								//Column header
								HTMLString.Append("<tr style=\"background-color:#E6E9EF; color:#000080\"><th>時段</th><th>入球 / 紅牌</th><th>時間</th><th>主隊比數</th><th>客隊比數</th><th>球員 / 球隊</th></tr>");
							}

							//Goal Details
							//Match Period
							HTMLString.Append("<tr align=\"center\"><td>");
							HTMLString.Append(iRecordIdx.ToString());
							HTMLString.Append(".&nbsp;<select name=\"MatchPeriod\">");
							if(!SportsOleReader.GetString(8).Trim().Equals("-1")) {
								HTMLString.Append("<option value=\"");
								HTMLString.Append(SportsOleReader.GetString(8).Trim());
								HTMLString.Append("\">");
								HTMLString.Append(matchTimeNVC.Get(SportsOleReader.GetString(8).Trim()));
								foreach(String sItem in matchTimeNVC.AllKeys) {
									if(!sItem.Equals(SportsOleReader.GetString(8).Trim())) {
										HTMLString.Append("<option value=" );
										HTMLString.Append(sItem);
										HTMLString.Append(">");
										HTMLString.Append(matchTimeNVC.Get(sItem));
									}
								}
							} else {
								foreach(String sItem in matchTimeNVC.AllKeys) {
									HTMLString.Append("<option value=" );
									HTMLString.Append(sItem);
									HTMLString.Append(">");
									HTMLString.Append(matchTimeNVC.Get(sItem));
								}
							}
							HTMLString.Append("</select></td>");

							//Score / Red Card Flag
							HTMLString.Append("<td><select name=\"ScoreFlag\">");
							if(SportsOleReader.GetString(9).Trim().Equals("R")) {
								HTMLString.Append("<option value=\"R\">紅牌");
								HTMLString.Append("<option value=\"S\">入球");
							} else {
								HTMLString.Append("<option value=\"S\">入球");
								HTMLString.Append("<option value=\"R\">紅牌");
							}
							HTMLString.Append("</select></td>");

							//Match Time per record
							HTMLString.Append("<td><input name=\"MatchTime\" value=\"");
							if(SportsOleReader.GetInt32(10) != -1) {
								HTMLString.Append(SportsOleReader.GetInt32(10).ToString());
							}
							HTMLString.Append("\" maxlength=\"3\" size=\"2\"></td>");

							//Host score
							HTMLString.Append("<td><input name=\"HostGoal\" value=\"");
							if(SportsOleReader.GetInt32(11) != -1) {
								HTMLString.Append(SportsOleReader.GetInt32(11).ToString());
							}
							HTMLString.Append("\" maxlength=\"2\" size=\"1\"></td>");

							//Guest score
							HTMLString.Append("<td><input name=\"GuestGoal\" value=\"");
							if(SportsOleReader.GetInt32(12) != -1) {
								HTMLString.Append(SportsOleReader.GetInt32(12).ToString());
							}
							HTMLString.Append("\" maxlength=\"2\" size=\"1\"></td>");

							//Player / Remark
							HTMLString.Append("<td><input name=\"Player\" value=\"");
							if(!SportsOleReader.GetString(13).Trim().Equals("-1")) {
								HTMLString.Append(SportsOleReader.GetString(13).Trim());
							}
							HTMLString.Append("\" maxlength=\"5\" size=\"8\"></td>");
							HTMLString.Append("</tr>");

							iRecordIdx++;
						}
						SportsOleReader.Close();
						SportsDBMgr.Close();
						
						if(iRecordIdx == 1) {	//Empty GoalDetails record
							HTMLString.Append("<tr style=\"background-color:#E6E9EF; color:#FF0000\"><th align=\"center\" colspan=\"6\">沒有輸入比數詳情，不能修改賽果！</th></tr>");
						} else {
							//Print the rest entries
							for(; iRecordIdx <= TOTALRECORDS; iRecordIdx++) {
								//Match Period
								HTMLString.Append("<tr align=\"center\"><td>");
								HTMLString.Append(iRecordIdx.ToString());
								HTMLString.Append(".&nbsp;<select name=\"MatchPeriod\">");
								foreach(String sItem in matchTimeNVC.AllKeys) {
									HTMLString.Append("<option value=" );
									HTMLString.Append(sItem);
									HTMLString.Append(">");
									HTMLString.Append(matchTimeNVC.Get(sItem));
								}
								HTMLString.Append("</select></td>");

								//Score / Red Card Flag
								HTMLString.Append("<td><select name=\"ScoreFlag\"><option value=\"S\">入球<option value=\"R\">紅牌</select></td>");

								//Match Time per record
								HTMLString.Append("<td><input name=\"MatchTime\" value=\"\" maxlength=\"3\" size=\"2\"></td>");

								//Host score
								HTMLString.Append("<td><input name=\"HostGoal\" value=\"\" maxlength=\"2\" size=\"1\"></td>");

								//Guest score
								HTMLString.Append("<td><input name=\"GuestGoal\" value=\"\" maxlength=\"2\" size=\"1\"></td>");

								//Player / Remark
								HTMLString.Append("<td><input name=\"Player\" value=\"\" maxlength=\"5\" size=\"8\"></td>");
								HTMLString.Append("</tr>");
							}
						}
						break;
					
					case "jccombo":
						String sMatchDayCode = sMatchID.Substring(0, 3);
						String sMatchNum = sMatchID.Substring(3, sMatchID.Length-3);
						SportsDBMgr.ConnectionString = HttpContext.Current.Application["JCCOMBODBConnectionString"].ToString();
						
						// Modified by Victor 20140716
            String sSql = "select main.CLEAGUE, main.CHOST, main.CHOST_GOAL, main.CGUEST, main.CGUEST_GOAL, main.CSTATUS, goal.CCUR_STATUS, goal.CSTATUS, goal.CREDFLAG_IND, goal.CGOAL_TM, goal.CHOST_GOAL, goal.CGUEST_GOAL, goal.CPLAYER, main.CCOMMENT from HK_GOAL_INFO main, HK_GOAL_DETAIL goal where main.CMATCHDAYCODE='" +sMatchDayCode+ "' and main.IMATCHNUM=" +sMatchNum+ " and main.CMATCHDAYCODE=goal.CMATCHDAYCODE and main.IMATCHNUM=goal.IMATCHNUM and main.CHOST<>'-1' and main.CGUEST<>'-1' order by  main.CLEAGUE, main.CHOST, main.CGUEST, goal.CGOAL_TM";
            m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
						m_SportsLog.SetFileName(0,LOGFILESUFFIX);
						m_SportsLog.Open();
						m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " ModifyResultContent.cs: " + sSql);
						m_SportsLog.Close();            						
						SportsOleReader = SportsDBMgr.ExecuteQuery(sSql);
						while(SportsOleReader.Read()) {
							if(iRecordIdx == 1) {
								//League, Host, Guest and their total score
								HTMLString.Append("<tr style=\"background-color:#E6E9EF; color:#000080\"><th colspan=\"2\" align=\"left\">JCCombo賽果：");

								//Match Status in details
								HTMLString.Append("&nbsp;&nbsp;<select name=\"DetailsStatus\">");
								if(!SportsOleReader.GetString(6).Trim().Equals("-1")) {
									HTMLString.Append("<option value=\"");
									HTMLString.Append(SportsOleReader.GetString(6).Trim());
									HTMLString.Append("\">");
									HTMLString.Append(matchStatusNVC.Get(SportsOleReader.GetString(6).Trim()));
									foreach(String sItem in matchStatusNVC.AllKeys) {
										if(!sItem.Equals(SportsOleReader.GetString(6).Trim())) {
											HTMLString.Append("<option value=" );
											HTMLString.Append(sItem);
											HTMLString.Append(">");
											HTMLString.Append(matchStatusNVC.Get(sItem));
										}
									}
								} else {
									HTMLString.Append("<option value=\"Y\">");
									HTMLString.Append(matchStatusNVC.Get("Y"));
									foreach(String sItem in matchStatusNVC.AllKeys) {
										if(!sItem.Equals("Y")) {
											HTMLString.Append("<option value=" );
											HTMLString.Append(sItem);
											HTMLString.Append(">");
											HTMLString.Append(matchStatusNVC.Get(sItem));
										}
									}
								}
								HTMLString.Append("</select></th>");

								HTMLString.Append("<th colspan=\"4\">");
								HTMLString.Append(SportsOleReader.GetString(0).Trim());
								HTMLString.Append("<input type=\"hidden\" name=\"League\" value=\"");
								HTMLString.Append(SportsOleReader.GetString(0).Trim());
								HTMLString.Append("\"> - ");
								HTMLString.Append(SportsOleReader.GetString(1).Trim());
								HTMLString.Append("<input type=\"hidden\" name=\"Host\" value=\"");
								HTMLString.Append(SportsOleReader.GetString(1).Trim());
								HTMLString.Append("\"><input name=\"HostTotalGoal\" value=\"");
								HTMLString.Append(SportsOleReader.GetString(2).Trim());
								HTMLString.Append("\" maxlength=\"2\" size=\"1\"> vs ");
								HTMLString.Append(SportsOleReader.GetString(3).Trim());
								HTMLString.Append("<input type=\"hidden\" name=\"Guest\" value=\"");
								HTMLString.Append(SportsOleReader.GetString(3).Trim());
								HTMLString.Append("\"><input name=\"GuestTotalGoal\" value=\"");
								HTMLString.Append(SportsOleReader.GetString(4).Trim());
								HTMLString.Append("\" maxlength=\"2\" size=\"1\">");
								
								// Modified by Victor 20140716
                // Match Comment
								HTMLString.Append("&nbsp;&nbsp;備註<input name=\"Comment\" value=\"");
								if(!SportsOleReader.GetString(13).Trim().Equals("-1")) {
								  HTMLString.Append(SportsOleReader.GetString(13).Trim());
								}  
								HTMLString.Append("\" maxlength=\"20\" size=\"24\">");

								//Match Status in menu
								HTMLString.Append("&nbsp;&nbsp;<select name=\"MenuStatus\">");
								foreach(String sItem in arrMatchStatus) {
									if(sItem.Substring(0,1).Equals(SportsOleReader.GetString(5).Trim())) {
										HTMLString.Append("<option value=\"");
										HTMLString.Append(SportsOleReader.GetString(5).Trim());
										HTMLString.Append("\">");
										HTMLString.Append(sItem);
										break;
									}
								}
								foreach(String sItem in arrMatchStatus) {
									if(!sItem.Substring(0,1).Equals(SportsOleReader.GetString(5).Trim())) {
										HTMLString.Append("<option value=\"");
										HTMLString.Append(sItem.Substring(0,1));
										HTMLString.Append("\">");
										HTMLString.Append(sItem);
									}
								}
								HTMLString.Append("</select></th></tr>");

								//Column header
								HTMLString.Append("<tr style=\"background-color:#E6E9EF; color:#000080\"><th>時段</th><th>入球 / 紅牌</th><th>時間</th><th>主隊比數</th><th>客隊比數</th><th>球員 / 球隊</th></tr>");
							}

							//Goal Details
							//Match Period
							HTMLString.Append("<tr align=\"center\"><td>");
							HTMLString.Append(iRecordIdx.ToString());
							HTMLString.Append(".&nbsp;<select name=\"MatchPeriod\">");
							if(!SportsOleReader.GetString(7).Trim().Equals("-1")) {
								HTMLString.Append("<option value=\"");
								HTMLString.Append(SportsOleReader.GetString(7).Trim());
								HTMLString.Append("\">");
								HTMLString.Append(matchTimeNVC.Get(SportsOleReader.GetString(7).Trim()));
								foreach(String sItem in matchTimeNVC.AllKeys) {
									if(!sItem.Equals(SportsOleReader.GetString(7).Trim())) {
										HTMLString.Append("<option value=" );
										HTMLString.Append(sItem);
										HTMLString.Append(">");
										HTMLString.Append(matchTimeNVC.Get(sItem));
									}
								}
							} else {
								foreach(String sItem in matchTimeNVC.AllKeys) {
									HTMLString.Append("<option value=" );
									HTMLString.Append(sItem);
									HTMLString.Append(">");
									HTMLString.Append(matchTimeNVC.Get(sItem));
								}
							}
							HTMLString.Append("</select></td>");

							//Score / Red Card Flag
							HTMLString.Append("<td><select name=\"ScoreFlag\">");
							if(SportsOleReader.GetString(8).Trim().Equals("R")) {
								HTMLString.Append("<option value=\"R\">紅牌");
								HTMLString.Append("<option value=\"S\">入球");
							} else {
								HTMLString.Append("<option value=\"S\">入球");
								HTMLString.Append("<option value=\"R\">紅牌");
							}
							HTMLString.Append("</select></td>");

							//Match Time per record
							HTMLString.Append("<td><input name=\"MatchTime\" value=\"");
							if(SportsOleReader.GetInt16(9) != -1) {
								HTMLString.Append(SportsOleReader.GetInt16(9).ToString());
							}
							HTMLString.Append("\" maxlength=\"3\" size=\"2\"></td>");

							//Host score
							HTMLString.Append("<td><input name=\"HostGoal\" value=\"");
							if(SportsOleReader.GetString(10).Trim() != "-1") {
								HTMLString.Append(SportsOleReader.GetString(10).Trim());
							}
							HTMLString.Append("\" maxlength=\"2\" size=\"1\"></td>");

							//Guest score
							HTMLString.Append("<td><input name=\"GuestGoal\" value=\"");
							if(SportsOleReader.GetString(11).Trim() != "-1") {
								HTMLString.Append(SportsOleReader.GetString(11).Trim());
							}
							HTMLString.Append("\" maxlength=\"2\" size=\"1\"></td>");

							//Player / Remark
							HTMLString.Append("<td><input name=\"Player\" value=\"");
							if(!SportsOleReader.GetString(12).Trim().Equals("-1")) {
								HTMLString.Append(SportsOleReader.GetString(12).Trim());
							}
							HTMLString.Append("\" maxlength=\"5\" size=\"8\"></td>");
							HTMLString.Append("</tr>");

							iRecordIdx++;
						}
						SportsOleReader.Close();
						SportsDBMgr.Close();
						
						if(iRecordIdx == 1) {	//Empty GoalDetails record
							HTMLString.Append("<tr style=\"background-color:#E6E9EF; color:#FF0000\"><th align=\"center\" colspan=\"6\">沒有輸入比數詳情，不能修改賽果！</th></tr>");
						} else {
							//Print the rest entries
							for(; iRecordIdx <= TOTALRECORDS; iRecordIdx++) {
								//Match Period
								HTMLString.Append("<tr align=\"center\"><td>");
								HTMLString.Append(iRecordIdx.ToString());
								HTMLString.Append(".&nbsp;<select name=\"MatchPeriod\">");
								foreach(String sItem in matchTimeNVC.AllKeys) {
									HTMLString.Append("<option value=" );
									HTMLString.Append(sItem);
									HTMLString.Append(">");
									HTMLString.Append(matchTimeNVC.Get(sItem));
								}
								HTMLString.Append("</select></td>");

								//Score / Red Card Flag
								HTMLString.Append("<td><select name=\"ScoreFlag\"><option value=\"S\">入球<option value=\"R\">紅牌</select></td>");

								//Match Time per record
								HTMLString.Append("<td><input name=\"MatchTime\" value=\"\" maxlength=\"3\" size=\"2\"></td>");

								//Host score
								HTMLString.Append("<td><input name=\"HostGoal\" value=\"\" maxlength=\"2\" size=\"1\"></td>");

								//Guest score
								HTMLString.Append("<td><input name=\"GuestGoal\" value=\"\" maxlength=\"2\" size=\"1\"></td>");

								//Player / Remark
								HTMLString.Append("<td><input name=\"Player\" value=\"\" maxlength=\"5\" size=\"8\"></td>");
								HTMLString.Append("</tr>");
							}
						}
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
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " ModifyResultContent.cs.GetDetails(): " + ex.ToString());
				m_SportsLog.Close();
				HTMLString.Remove(0,HTMLString.Length);
				HTMLString.Append((string)HttpContext.Current.Application["accessErrorMsg"]);
			}

			return HTMLString.ToString();
		}

		public int Modify() {
			int iUpd = 0;
			int iSeqNo = 0;
			char[] delimiter = new char[] {','};
			DBManager SportsDBMgr;
			OleDbDataReader SportsOleReader;
			StringBuilder SQLString = new StringBuilder();

			string sTarget;
			string sMatchID;
			string sLeague;
			string sHost;
			string sGuest;
			string sHostTTG;
			string sGuestTTG;
			string sMenuStatus;
			string sDetailsStatus;
			//string sRemark;
			string sComment;
			string[] arrMatchPeriod;
			string[] arrScoreFlag;
			string[] arrMatchTime;
			string[] arrHostGoal;
			string[] arrGuestGoal;
			string[] arrPlayer;

			sTarget = HttpContext.Current.Request.Form["target"];
			sMatchID = HttpContext.Current.Request.Form["id"];
			sLeague = HttpContext.Current.Request.Form["League"];
			sHost = HttpContext.Current.Request.Form["Host"];
			sGuest = HttpContext.Current.Request.Form["Guest"];
			sHostTTG = HttpContext.Current.Request.Form["HostTotalGoal"];
			sGuestTTG = HttpContext.Current.Request.Form["GuestTotalGoal"];
			sMenuStatus = HttpContext.Current.Request.Form["MenuStatus"];
			sDetailsStatus = HttpContext.Current.Request.Form["DetailsStatus"];
			//sRemark = HttpContext.Current.Request.Form["remark"];
			sComment = HttpContext.Current.Request.Form["Comment"];

			arrMatchPeriod = HttpContext.Current.Request.Form["MatchPeriod"].Split(delimiter);
			arrScoreFlag = HttpContext.Current.Request.Form["ScoreFlag"].Split(delimiter);
			arrMatchTime = HttpContext.Current.Request.Form["MatchTime"].Split(delimiter);
			arrHostGoal = HttpContext.Current.Request.Form["HostGoal"].Split(delimiter);
			arrGuestGoal = HttpContext.Current.Request.Form["GuestGoal"].Split(delimiter);
			arrPlayer = HttpContext.Current.Request.Form["Player"].Split(delimiter);

			try {
				SportsDBMgr = new DBManager();
				switch(sTarget) {
					case "gogo1":
						int iRecID = 0;
						SportsDBMgr.ConnectionString = HttpContext.Current.Application["SoccerDBConnectionString"].ToString();

						//Update HKGOAL_DETAILS (menu)
						SQLString.Append("update HKGOAL_DETAILS set IH_GOAL=");
						SQLString.Append(sHostTTG);
						SQLString.Append(", IG_GOAL=");
						SQLString.Append(sGuestTTG);
						SQLString.Append(", CSTATUS='");
						SQLString.Append(sMenuStatus);
						SQLString.Append("' where IHEADER_ID=");
						SQLString.Append(sMatchID);
						iUpd = SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
						SportsDBMgr.Close();

						//Update HKRES_DETAILS (details header)
						SQLString.Remove(0, SQLString.Length);
						SQLString.Append("update HKRES_DETAILS set CSTATUS='");
						SQLString.Append(sDetailsStatus);
						SQLString.Append("' where CLEAGUE='");
						SQLString.Append(sLeague);
						SQLString.Append("' and CHOST='");
						SQLString.Append(sHost);
						SQLString.Append("' and CGUEST='");
						SQLString.Append(sGuest);
						SQLString.Append("'");
						SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
						SportsDBMgr.Close();

						//Update HKSCORE_DETAILS (details entries)
						//clear old record first
						SQLString.Remove(0, SQLString.Length);
						SQLString.Append("delete from HKSCORE_DETAILS where CLEAGUE='");
						SQLString.Append(sLeague);
						SQLString.Append("' and CHOST='");
						SQLString.Append(sHost);
						SQLString.Append("' and CGUEST='");
						SQLString.Append(sGuest);
						SQLString.Append("'");
						SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
						SportsDBMgr.Close();

						//Get Max. ID
						SQLString.Remove(0, SQLString.Length);
						SQLString.Append("select max(IREC_NO) from HKSCORE_DETAILS");
						SportsOleReader = SportsDBMgr.ExecuteQuery(SQLString.ToString());
						if(SportsOleReader.Read()) {
							if(!SportsOleReader.IsDBNull(0)) {
								iRecID = SportsOleReader.GetInt32(0) + 1;
							} else {
								iRecID = 1;
							}
						} else {
							iRecID = 1;
						}
						SportsOleReader.Close();
						SportsDBMgr.Close();

						//insert new records
						iSeqNo = 0;
						for(int i = 0; i < arrMatchPeriod.Length; i++) {
							if(!(arrMatchTime[i].Equals("") && arrHostGoal[i].Equals("") && arrGuestGoal[i].Equals(""))) {
								SQLString.Remove(0, SQLString.Length);
								SQLString.Append("insert into HKSCORE_DETAILS values('");
								SQLString.Append(sLeague);
								SQLString.Append("','");
								SQLString.Append(sHost);
								SQLString.Append("','");
								SQLString.Append(sGuest);
								SQLString.Append("','");
								SQLString.Append(arrMatchPeriod[i]);
								SQLString.Append("',");
								if(!arrMatchTime[i].Equals("")) {
									SQLString.Append(arrMatchTime[i]);
								} else {
									SQLString.Append("-1");
								}
								SQLString.Append(",'");
								if(!arrHostGoal[i].Equals("")) {
									SQLString.Append(Convert.ToInt32(arrHostGoal[i]).ToString("D2"));
								} else {
									SQLString.Append("00");
								}
								if(!arrGuestGoal[i].Equals("")) {
									SQLString.Append(Convert.ToInt32(arrGuestGoal[i]).ToString("D2"));
								} else {
									SQLString.Append("00");
								}
								SQLString.Append("','");
								if(!arrPlayer[i].Equals("")) {
									SQLString.Append(arrPlayer[i]);
								} else {
									SQLString.Append("-1");
								}
								SQLString.Append("','");
								SQLString.Append(arrScoreFlag[i]);
								SQLString.Append("',");
								SQLString.Append(iSeqNo.ToString());
								SQLString.Append(",'-1','-1',");
								SQLString.Append(iRecID.ToString());
								SQLString.Append(")");
								SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
								SportsDBMgr.Close();
								iSeqNo++;
								iRecID++;
							}
						}

						m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
						m_SportsLog.SetFileName(0,LOGFILESUFFIX);
						m_SportsLog.Open();
						m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " ModifyResultContent.cs: Modify GOGO1 result [L:" + sLeague + ", H:" + sHost + ", G:" + sGuest + "] (" + HttpContext.Current.Session["user_name"] + ")");
						m_SportsLog.Close();
						break;

					case "gogo2":
						int iGoalDetailsID = -1;
						SportsDBMgr.ConnectionString = HttpContext.Current.Application["GOGO2SOCDBConnectionString"].ToString();

						//Update HK_GOALMENU (menu)
						SQLString.Append("update HK_GOALMENU set IHOST_SCORE=");
						SQLString.Append(sHostTTG);
						SQLString.Append(", IGUEST_SCORE=");
						SQLString.Append(sGuestTTG);
						SQLString.Append(", CSTATUS='");
						SQLString.Append(sMenuStatus);
						SQLString.Append("' where IREC_ID=");
						SQLString.Append(sMatchID);
						iUpd = SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
						SportsDBMgr.Close();

						//Update HK_GOALDETAILS (details entries)
						//Get IGOALDETAILS_ID first
						SQLString.Remove(0, SQLString.Length);
						SQLString.Append("select IGOALDETAILS_ID from HK_GOALDETAILS where IREC_ID=");
						SQLString.Append(sMatchID);
						SportsOleReader = SportsDBMgr.ExecuteQuery(SQLString.ToString());
						if(SportsOleReader.Read()) {
							if(!SportsOleReader.IsDBNull(0)) {
								iGoalDetailsID = SportsOleReader.GetInt32(0);
							}
						}
						SportsOleReader.Close();
						SportsDBMgr.Close();

						if(iGoalDetailsID != -1) {	//ID was assigned by Message Handler
							//clear old record first
							SQLString.Remove(0, SQLString.Length);
							SQLString.Append("delete from HK_GOALDETAILS where IREC_ID=");
							SQLString.Append(sMatchID);
							SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
							SportsDBMgr.Close();

							//insert new records
							iSeqNo = 0;
							for(int i = 0; i < arrMatchPeriod.Length; i++) {
								if(!(arrMatchTime[i].Equals("") && arrHostGoal[i].Equals("") && arrGuestGoal[i].Equals(""))) {
									SQLString.Remove(0, SQLString.Length);
									SQLString.Append("insert into HK_GOALDETAILS values(");
									SQLString.Append(sMatchID);
									SQLString.Append(",");
									SQLString.Append(iSeqNo.ToString());
									SQLString.Append(",");
									SQLString.Append(iGoalDetailsID.ToString());
									SQLString.Append(",'U','");
									SQLString.Append(sDetailsStatus);
									SQLString.Append("','");
									SQLString.Append(arrMatchPeriod[i]);
									SQLString.Append("',");
									if(!arrMatchTime[i].Equals("")) {
										SQLString.Append(arrMatchTime[i]);
									} else {
										SQLString.Append("-1");
									}
									SQLString.Append(",");
									if(!arrHostGoal[i].Equals("")) {
										SQLString.Append(arrHostGoal[i]);
									} else {
										SQLString.Append("0");
									}
									SQLString.Append(",");
									if(!arrGuestGoal[i].Equals("")) {
										SQLString.Append(arrGuestGoal[i]);
									} else {
										SQLString.Append("0");
									}
									SQLString.Append(",'");
									if(!arrPlayer[i].Equals("")) {
										SQLString.Append(arrPlayer[i]);
									} else {
										SQLString.Append("-1");
									}
									SQLString.Append("','");
									SQLString.Append(arrScoreFlag[i]);
									SQLString.Append("','-1','-1','-1',CURRENT_TIMESTAMP)");
									SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
									SportsDBMgr.Close();
									iSeqNo++;
								}
							}

							//Default record if there was no GoalDetails
							if(iSeqNo == 0) {
								SQLString.Remove(0, SQLString.Length);
								SQLString.Append("insert into HK_GOALDETAILS values(");
								SQLString.Append(sMatchID);
								SQLString.Append(",");
								SQLString.Append(iSeqNo.ToString());
								SQLString.Append(",");
								SQLString.Append(iGoalDetailsID.ToString());
								SQLString.Append(",'U','");
								SQLString.Append(sDetailsStatus);
								SQLString.Append("','-1',-1,-1,-1,'-1','-1','-1','-1','-1',CURRENT_TIMESTAMP)");
								SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
								SportsDBMgr.Close();
								iSeqNo++;
							}
						}

						m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
						m_SportsLog.SetFileName(0,LOGFILESUFFIX);
						m_SportsLog.Open();
						m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " ModifyResultContent.cs: Modify GOGO2 result [L:" + sLeague + ", H:" + sHost + ", G:" + sGuest + "] (" + HttpContext.Current.Session["user_name"] + ")");
						m_SportsLog.Close();
						break;

					case "hkjc":
						int iHKJCGoalDetailsID = -1;
						SportsDBMgr.ConnectionString = HttpContext.Current.Application["HKJCSOCConnectionString"].ToString();

						//Update HK_GOALMENU (menu)
						SQLString.Append("update HK_GOALMENU set IHOST_SCORE=");
						SQLString.Append(sHostTTG);
						SQLString.Append(", IGUEST_SCORE=");
						SQLString.Append(sGuestTTG);
						SQLString.Append(", CSTATUS='");
						SQLString.Append(sMenuStatus);
						SQLString.Append("' where IREC_ID=");
						SQLString.Append(sMatchID);
						iUpd = SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
						SportsDBMgr.Close();

						//Update HK_GOALDETAILS (details entries)
						//Get IGOALDETAILS_ID first
						SQLString.Remove(0, SQLString.Length);
						SQLString.Append("select IGOALDETAILS_ID from HK_GOALDETAILS where IREC_ID=");
						SQLString.Append(sMatchID);
						SportsOleReader = SportsDBMgr.ExecuteQuery(SQLString.ToString());
						if(SportsOleReader.Read()) {
							if(!SportsOleReader.IsDBNull(0)) {
								iHKJCGoalDetailsID = SportsOleReader.GetInt32(0);
							}
						}
						SportsOleReader.Close();
						SportsDBMgr.Close();

						if(iHKJCGoalDetailsID != -1) {	//ID was assigned by Message Handler
							//clear old record first
							SQLString.Remove(0, SQLString.Length);
							SQLString.Append("delete from HK_GOALDETAILS where IREC_ID=");
							SQLString.Append(sMatchID);
							SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
							SportsDBMgr.Close();

							//insert new records
							iSeqNo = 0;
							for(int i = 0; i < arrMatchPeriod.Length; i++) {
								if(!(arrMatchTime[i].Equals("") && arrHostGoal[i].Equals("") && arrGuestGoal[i].Equals(""))) {
									SQLString.Remove(0, SQLString.Length);
									SQLString.Append("insert into HK_GOALDETAILS values(");
									SQLString.Append(sMatchID);
									SQLString.Append(",");
									SQLString.Append(iSeqNo.ToString());
									SQLString.Append(",");
									SQLString.Append(iHKJCGoalDetailsID.ToString());
									SQLString.Append(",'U','");
									SQLString.Append(sDetailsStatus);
									SQLString.Append("','");
									SQLString.Append(arrMatchPeriod[i]);
									SQLString.Append("',");
									if(!arrMatchTime[i].Equals("")) {
										SQLString.Append(arrMatchTime[i]);
									} else {
										SQLString.Append("-1");
									}
									SQLString.Append(",");
									if(!arrHostGoal[i].Equals("")) {
										SQLString.Append(arrHostGoal[i]);
									} else {
										SQLString.Append("0");
									}
									SQLString.Append(",");
									if(!arrGuestGoal[i].Equals("")) {
										SQLString.Append(arrGuestGoal[i]);
									} else {
										SQLString.Append("0");
									}
									SQLString.Append(",'");
									if(!arrPlayer[i].Equals("")) {
										SQLString.Append(arrPlayer[i]);
									} else {
										SQLString.Append("-1");
									}
									SQLString.Append("','-1','");
									SQLString.Append(arrScoreFlag[i]);
									SQLString.Append("','-1','-1','-1',CURRENT_TIMESTAMP)");
									SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
									SportsDBMgr.Close();
									iSeqNo++;
								}
							}

							//Default record if there was no GoalDetails
							if(iSeqNo == 0) {
								SQLString.Remove(0, SQLString.Length);
								SQLString.Append("insert into HK_GOALDETAILS values(");
								SQLString.Append(sMatchID);
								SQLString.Append(",");
								SQLString.Append(iSeqNo.ToString());
								SQLString.Append(",");
								SQLString.Append(iHKJCGoalDetailsID.ToString());
								SQLString.Append(",'U','");
								SQLString.Append(sDetailsStatus);
								SQLString.Append("','-1',-1,-1,-1,'-1','-1','-1','-1','-1','-1',CURRENT_TIMESTAMP)");
								SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
								SportsDBMgr.Close();
								iSeqNo++;
							}
						}

						m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
						m_SportsLog.SetFileName(0,LOGFILESUFFIX);
						m_SportsLog.Open();
						m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " ModifyResultContent.cs: Modify HKJC result [L:" + sLeague + ", H:" + sHost + ", G:" + sGuest + "] (" + HttpContext.Current.Session["user_name"] + ")");
						m_SportsLog.Close();
						break;
						
					case "jccombo":
						int iJCCOMBOGoalDetailsID = -1;
						String sMatchDayCode = sMatchID.Substring(0, 3);
						String sMatchNum = sMatchID.Substring(3, sMatchID.Length-3);
						SportsDBMgr.ConnectionString = HttpContext.Current.Application["JCCOMBODBConnectionString"].ToString();

						//Update HK_GOAL_INFO (menu)
						SQLString.Append("update HK_GOAL_INFO set CHOST_GOAL='");
						SQLString.Append(sHostTTG);
						SQLString.Append("', CGUEST_GOAL='");
						SQLString.Append(sGuestTTG);
						SQLString.Append("', CSTATUS='");
						SQLString.Append(sMenuStatus);
						SQLString.Append("', CCOMMENT='");
						SQLString.Append(sComment);						
						SQLString.Append("' where CMATCHDAYCODE='");
						SQLString.Append(sMatchDayCode);
						SQLString.Append("' and IMATCHNUM=");
						SQLString.Append(sMatchNum);
						iUpd = SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
						SportsDBMgr.Close();
						
						SQLString.Remove(0, SQLString.Length);
						SQLString.Append("update HK_GOAL_INFO set ITM_OF_GM=0, CCOUNT_MIN='0' where CSTATUS='完'");
						SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
						SportsDBMgr.Close();

						//Update HK_GOAL_DETAIL (details entries)
						//Get CMATCHDAYCODE and IMATCHNUM first
						SQLString.Remove(0, SQLString.Length);
						SQLString.Append("select count(*) from HK_GOAL_DETAIL where CMATCHDAYCODE='");
						SQLString.Append(sMatchDayCode);
						SQLString.Append("' and IMATCHNUM=");
						SQLString.Append(sMatchNum);
						SportsOleReader = SportsDBMgr.ExecuteQuery(SQLString.ToString());
						if(SportsOleReader.Read()) {
							if(!SportsOleReader.IsDBNull(0)) {
								iJCCOMBOGoalDetailsID = SportsOleReader.GetInt32(0);
							}
						}
						SportsOleReader.Close();
						SportsDBMgr.Close();

						if(iJCCOMBOGoalDetailsID != -1) {	//ID was assigned by Message Handler
							//clear old record first
							SQLString.Remove(0, SQLString.Length);
							SQLString.Append("delete from HK_GOAL_DETAIL where CMATCHDAYCODE='");
							SQLString.Append(sMatchDayCode);
							SQLString.Append("' and IMATCHNUM=");
							SQLString.Append(sMatchNum);
							SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
							SportsDBMgr.Close();

							//insert new records
							iSeqNo = 0;
							for(int i = 0; i < arrMatchPeriod.Length; i++) {
								if(!(arrMatchTime[i].Equals("") && arrHostGoal[i].Equals("") && arrGuestGoal[i].Equals(""))) {
									SQLString.Remove(0, SQLString.Length);
									SQLString.Append("insert into HK_GOAL_DETAIL values('");
									SQLString.Append(sMatchDayCode);
									SQLString.Append("',");
									SQLString.Append(sMatchNum);
									SQLString.Append(",'");
									SQLString.Append("0");
									SQLString.Append("','");
									SQLString.Append(sDetailsStatus);
									SQLString.Append("','");
									SQLString.Append(arrScoreFlag[i]);
									SQLString.Append("','");
									SQLString.Append(arrMatchPeriod[i]);
									SQLString.Append("',");
									if(!arrMatchTime[i].Equals("")) {
										SQLString.Append(arrMatchTime[i]);
									} else {
										SQLString.Append("-1");
									}
									SQLString.Append(",'");
									if(!arrHostGoal[i].Equals("")) {
										SQLString.Append(arrHostGoal[i]);
									} else {
										SQLString.Append("0");
									}
									SQLString.Append("','");
									if(!arrGuestGoal[i].Equals("")) {
										SQLString.Append(arrGuestGoal[i]);
									} else {
										SQLString.Append("0");
									}
									SQLString.Append("','");
									if(!arrPlayer[i].Equals("")) {
										SQLString.Append(arrPlayer[i]);
									} else {
										SQLString.Append("-1");
									}
									SQLString.Append("',");
									SQLString.Append("'-1','-1','-1')");
									SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
									SportsDBMgr.Close();
									iSeqNo++;
								}
							}

							//Default record if there was no HK_GOAL_DETAIL
							if(iSeqNo == 0) {
								SQLString.Remove(0, SQLString.Length);
								SQLString.Append("insert into HK_GOAL_DETAIL values('");
								SQLString.Append(sMatchDayCode);
								SQLString.Append("',");
								SQLString.Append(sMatchNum);
								SQLString.Append(",'");
								SQLString.Append("0");
								SQLString.Append("','");
								SQLString.Append(sDetailsStatus);
								SQLString.Append("',");
								SQLString.Append("'-1','-1',-1,'-1','-1','-1','-1','-1','-1')");
								SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
								SportsDBMgr.Close();
								iSeqNo++;
							}
						}

						m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
						m_SportsLog.SetFileName(0,LOGFILESUFFIX);
						m_SportsLog.Open();
						m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " ModifyResultContent.cs: Modify HKJC result [L:" + sLeague + ", H:" + sHost + ", G:" + sGuest + "] (" + HttpContext.Current.Session["user_name"] + ")");
						m_SportsLog.Close();
						break;

					default:
						iSeqNo = 0;
						iUpd = 0;
						break;
				}
			} catch(Exception ex) {
				iSeqNo = -1;
				iUpd = 0;
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " ModifyResultContent.cs.Modify(): " + ex.ToString());
				m_SportsLog.Close();
			}

			return (iSeqNo + iUpd);
		}

	}
}