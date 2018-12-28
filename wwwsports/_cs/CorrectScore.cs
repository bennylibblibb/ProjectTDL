/*
Objective:
Retrieval and modify the odds of correct score, there are two kind of matrix, 4x and 5x

Last updated:
09 Feb 2004 by Henry

C#.NET complier statement:
csc /t:library /out:..\bin\CorrectScore.dll /r:..\bin\DBManager.dll;..\bin\Files.dll;..\bin\MessageClient.dll;..\bin\SportsMessage.dll CorrectScore.cs
*/

using System;
using System.Collections;
using System.Data.OleDb;
using System.Reflection;
using System.Text;
using System.Web;
using TDL.DB;
using TDL.IO;
using TDL.Message;

[assembly:AssemblyCompany("TDSL")]
[assembly:AssemblyCopyright("(c) 2003 TDSL. All rights reserved.")]
[assembly:AssemblyDescription("足球資訊 -> 波膽")]
[assembly:AssemblyProduct("Sports.NET")]
[assembly:AssemblyTitle("Sports.NET DLL")]
[assembly:AssemblyVersion("3.1.*")]
namespace SportsUtil {
	public class CorrectScore {
		const string LOGFILESUFFIX = "log";
		const string DBCR = "(CR)";
		const string PAGECR = "\r\n";
		const int HISTORYCOUNT=4;
		DBManager m_SportsDBMgr;
		Files m_SportsLog;
		Encoding m_Big5Encoded;
		StringBuilder SQLString;

		public CorrectScore(string Connection) {
			m_SportsDBMgr = new DBManager();
			m_SportsDBMgr.ConnectionString = Connection;
			m_SportsLog = new Files();
			m_Big5Encoded = Encoding.GetEncoding(950);
			SQLString = new StringBuilder();
		}

		public string GetMatrix() {
			int iMatrixSize = 6;	//default matrix size
			string sMatchCount;
			string sLeague = "";
			string sHost = "";
			string sGuest = "";
			string sMatchField = "(主)";
			string sMatchDate = "";
			string sMatchTime = "";
			ArrayList oddsList = new ArrayList(40);
			OleDbDataReader SportsOleReader;
			StringBuilder HTMLString = new StringBuilder();

			sMatchCount = HttpContext.Current.Request.QueryString["matchcnt"].Trim();
			try {
				SQLString.Remove(0,SQLString.Length);
				SQLString.Append("select LEAGLONG, host, guest, FIELD, MATCHDATE, MATCHTIME from GAMEINFO where MATCH_CNT=");
				SQLString.Append(sMatchCount);
				SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
				if(SportsOleReader.Read()) {
					sLeague = SportsOleReader.GetString(0).Trim();
					sHost = SportsOleReader.GetString(1).Trim();
					sGuest = SportsOleReader.GetString(2).Trim();
					if(SportsOleReader.GetString(3).Trim().Equals("H")) sMatchField = "(中)";
					sMatchDate = SportsOleReader.GetString(4).Trim();
					sMatchTime = SportsOleReader.GetString(5).Trim();
				}
				SportsOleReader.Close();
				m_SportsDBMgr.Close();

				SQLString.Remove(0,SQLString.Length);
				SQLString.Append("select IMATRIXSIZE, CODDS from CORRECTSCORE_INFO where CACT<>'D' AND IMATCH_CNT=");
				SQLString.Append(sMatchCount);
				SQLString.Append(" order by ICORRECTSCORE_CNT");
				SportsOleReader = m_SportsDBMgr.ExecuteQuery(SQLString.ToString());
				while(SportsOleReader.Read()) {
					iMatrixSize = SportsOleReader.GetInt32(0);
					if(!SportsOleReader.IsDBNull(1)) {
						if(!SportsOleReader.GetString(1).Trim().Equals("-1")) {
							oddsList.Add(SportsOleReader.GetString(1).Trim());
						} else {
							oddsList.Add("");
						}
					} else oddsList.Add("");
				}
				SportsOleReader.Close();
				m_SportsDBMgr.Close();
				oddsList.TrimToSize();

				HTMLString.Append("<tr align=\"center\" style=\"background-color:#E0FFFF\"><th colspan=\"2\">波膽</th><td colspan=\"4\"><b>陣列選項：</b>");
				if(iMatrixSize == 6) {
					if(oddsList.Count > 0) {	//contains odds data
						HTMLString.Append("<input type=\"radio\" name=\"MatrixSize\" value=\"5\" onClick=\"setMatrixSize()\">淨勝4球以上&nbsp;<input type=\"radio\" name=\"MatrixSize\" value=\"6\" onClick=\"setMatrixSize()\" checked>淨勝5球以上</td><th colspan=\"3\">執行動作：<select name=\"Action\"><option value=\"U\">新增/修改</option><option value=\"D\">刪除</option></select></th></tr>");
						HTMLString.Append("<tr align=\"center\"><th colspan=\"2\"><input type=\"hidden\" name=\"league\" value=\"" + sLeague + "\">" + sLeague + "</th><th>0</th><th>1</th><th>2</th><th>3</th><th>4</th><th><label id=\"g_5\">5</label></th><th><label id=\"g_gt5\">&gt;5</label></th></tr>");
						HTMLString.Append("<tr align=\"center\"><th width=\"10%\" rowspan=\"7\"><input type=\"hidden\" name=\"host\" value=\"" + sHost + "\">" + sHost + "<br>" + sMatchField + "</th><th>0</th>");
						HTMLString.Append("<td><input name=\"odds\" value=\"" + oddsList[0] + "\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(0)\"></td>");
						HTMLString.Append("<td><input name=\"odds\" value=\"" + oddsList[1] + "\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(1)\"></td>");
						HTMLString.Append("<td><input name=\"odds\" value=\"" + oddsList[2] + "\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(2)\"></td>");
						HTMLString.Append("<td><input name=\"odds\" value=\"" + oddsList[3] + "\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(3)\"></td>");
						HTMLString.Append("<td><input name=\"odds\" value=\"" + oddsList[4] + "\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(4)\"></td>");
						HTMLString.Append("<td><input name=\"odds\" value=\"" + oddsList[5] + "\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(5)\"></td>");
						HTMLString.Append("<td><input name=\"odds\" value=\"" + oddsList[6] + "\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(6)\"></td>");
						HTMLString.Append("</tr>");
						HTMLString.Append("<tr align=\"center\"><th>1</th>");
						HTMLString.Append("<td><input name=\"odds\" value=\"" + oddsList[7] + "\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(7)\"></td>");
						HTMLString.Append("<td><input name=\"odds\" value=\"" + oddsList[8] + "\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(8)\"></td>");
						HTMLString.Append("<td><input name=\"odds\" value=\"" + oddsList[9] + "\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(9)\"></td>");
						HTMLString.Append("<td><input name=\"odds\" value=\"" + oddsList[10] + "\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(10)\"></td>");
						HTMLString.Append("<td><input name=\"odds\" value=\"" + oddsList[11] + "\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(11)\"></td>");
						HTMLString.Append("<td><input name=\"odds\" value=\"" + oddsList[12] + "\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(12)\"></td>");
						HTMLString.Append("<td></td></tr>");
						HTMLString.Append("<tr align=\"center\"><th>2</th>");
						HTMLString.Append("<td><input name=\"odds\" value=\"" + oddsList[13] + "\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(13)\"></td>");
						HTMLString.Append("<td><input name=\"odds\" value=\"" + oddsList[14] + "\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(14)\"></td>");
						HTMLString.Append("<td><input name=\"odds\" value=\"" + oddsList[15] + "\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(15)\"></td>");
						HTMLString.Append("<td><input name=\"odds\" value=\"" + oddsList[16] + "\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(16)\"></td>");
						HTMLString.Append("<td><input name=\"odds\" value=\"" + oddsList[17] + "\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(17)\"></td>");
						HTMLString.Append("<td><input name=\"odds\" value=\"" + oddsList[18] + "\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(18)\"></td>");
						HTMLString.Append("<td></td></tr>");
						HTMLString.Append("<tr align=\"center\"><th>3</th>");
						HTMLString.Append("<td><input name=\"odds\" value=\"" + oddsList[19] + "\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(19)\"></td>");
						HTMLString.Append("<td><input name=\"odds\" value=\"" + oddsList[20] + "\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(20)\"></td>");
						HTMLString.Append("<td><input name=\"odds\" value=\"" + oddsList[21] + "\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(21)\"></td>");
						HTMLString.Append("<td><input name=\"odds\" value=\"" + oddsList[22] + "\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(22)\"></td>");
						HTMLString.Append("<td><input name=\"odds\" value=\"" + oddsList[23] + "\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(23)\"></td>");
						HTMLString.Append("<td><input name=\"odds\" value=\"" + oddsList[24] + "\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(24)\"></td>");
						HTMLString.Append("<td></td></tr>");
						HTMLString.Append("<tr align=\"center\"><th>4</th>");
						HTMLString.Append("<td><input name=\"odds\" value=\"" + oddsList[25] + "\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(25)\"></td>");
						HTMLString.Append("<td><input name=\"odds\" value=\"" + oddsList[26] + "\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(26)\"></td>");
						HTMLString.Append("<td><input name=\"odds\" value=\"" + oddsList[27] + "\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(27)\"></td>");
						HTMLString.Append("<td><input name=\"odds\" value=\"" + oddsList[28] + "\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(28)\"></td>");
						HTMLString.Append("<td><input name=\"odds\" value=\"" + oddsList[29] + "\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(29)\"></td>");
						HTMLString.Append("<td><input name=\"odds\" value=\"" + oddsList[30] + "\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(30)\"></td>");
						HTMLString.Append("<td></td></tr>");
						HTMLString.Append("<tr align=\"center\"><th><label id=\"h_5\">5</label></th>");
						HTMLString.Append("<td><input name=\"odds\" value=\"" + oddsList[31] + "\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(31)\"></td>");
						HTMLString.Append("<td><input name=\"odds\" value=\"" + oddsList[32] + "\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(32)\"></td>");
						HTMLString.Append("<td><input name=\"odds\" value=\"" + oddsList[33] + "\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(33)\"></td>");
						HTMLString.Append("<td><input name=\"odds\" value=\"" + oddsList[34] + "\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(34)\"></td>");
						HTMLString.Append("<td><input name=\"odds\" value=\"" + oddsList[35] + "\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(35)\"></td>");
						HTMLString.Append("<td><input name=\"odds\" value=\"" + oddsList[36] + "\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(36)\"></td>");
						HTMLString.Append("<td></td></tr>");
						HTMLString.Append("<tr align=\"center\"><th><label id=\"h_gt5\">&gt;5</label></th>");
						HTMLString.Append("<td><input name=\"odds\" value=\"" + oddsList[37] + "\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(37)\"></td>");
						HTMLString.Append("<td></td><td></td><td></td><td></td><td></td>");
						HTMLString.Append("<td><input name=\"odds\" value=\"" + oddsList[38] + "\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(38)\"></td></tr>");
						HTMLString.Append("<tr align=\"center\"><th colspan=\"9\"><input type=\"hidden\" name=\"guest\" value=\"" + sGuest + "\">(客) " + sGuest + "</th></tr>");
					} else {
						HTMLString.Append("<input type=\"radio\" name=\"MatrixSize\" value=\"5\" onClick=\"setMatrixSize()\">淨勝4球以上&nbsp;<input type=\"radio\" name=\"MatrixSize\" value=\"6\" onClick=\"setMatrixSize()\" checked>淨勝5球以上</td><th colspan=\"3\">執行動作：<select name=\"Action\"><option value=\"U\">新增/修改</option><option value=\"D\">刪除</option></select></th></tr>");
						HTMLString.Append("<tr align=\"center\"><th colspan=\"2\"><input type=\"hidden\" name=\"league\" value=\"" + sLeague + "\">" + sLeague + "</th><th>0</th><th>1</th><th>2</th><th>3</th><th>4</th><th><label id=\"g_5\">5</label></th><th><label id=\"g_gt5\">&gt;5</label></th></tr>");
						HTMLString.Append("<tr align=\"center\"><th width=\"10%\" rowspan=\"7\"><input type=\"hidden\" name=\"host\" value=\"" + sHost + "\">" + sHost + "<br>" + sMatchField + "</th><th>0</th>");
						HTMLString.Append("<td><input name=\"odds\" value=\"\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(0)\"></td>");
						HTMLString.Append("<td><input name=\"odds\" value=\"\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(1)\"></td>");
						HTMLString.Append("<td><input name=\"odds\" value=\"\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(2)\"></td>");
						HTMLString.Append("<td><input name=\"odds\" value=\"\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(3)\"></td>");
						HTMLString.Append("<td><input name=\"odds\" value=\"\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(4)\"></td>");
						HTMLString.Append("<td><input name=\"odds\" value=\"\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(5)\"></td>");
						HTMLString.Append("<td><input name=\"odds\" value=\"\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(6)\"></td>");
						HTMLString.Append("</tr>");
						HTMLString.Append("<tr align=\"center\"><th>1</th>");
						HTMLString.Append("<td><input name=\"odds\" value=\"\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(7)\"></td>");
						HTMLString.Append("<td><input name=\"odds\" value=\"\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(8)\"></td>");
						HTMLString.Append("<td><input name=\"odds\" value=\"\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(9)\"></td>");
						HTMLString.Append("<td><input name=\"odds\" value=\"\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(10)\"></td>");
						HTMLString.Append("<td><input name=\"odds\" value=\"\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(11)\"></td>");
						HTMLString.Append("<td><input name=\"odds\" value=\"\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(12)\"></td>");
						HTMLString.Append("<td></td></tr>");
						HTMLString.Append("<tr align=\"center\"><th>2</th>");
						HTMLString.Append("<td><input name=\"odds\" value=\"\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(13)\"></td>");
						HTMLString.Append("<td><input name=\"odds\" value=\"\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(14)\"></td>");
						HTMLString.Append("<td><input name=\"odds\" value=\"\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(15)\"></td>");
						HTMLString.Append("<td><input name=\"odds\" value=\"\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(16)\"></td>");
						HTMLString.Append("<td><input name=\"odds\" value=\"\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(17)\"></td>");
						HTMLString.Append("<td><input name=\"odds\" value=\"\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(18)\"></td>");
						HTMLString.Append("<td></td></tr>");
						HTMLString.Append("<tr align=\"center\"><th>3</th>");
						HTMLString.Append("<td><input name=\"odds\" value=\"\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(19)\"></td>");
						HTMLString.Append("<td><input name=\"odds\" value=\"\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(20)\"></td>");
						HTMLString.Append("<td><input name=\"odds\" value=\"\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(21)\"></td>");
						HTMLString.Append("<td><input name=\"odds\" value=\"\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(22)\"></td>");
						HTMLString.Append("<td><input name=\"odds\" value=\"\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(23)\"></td>");
						HTMLString.Append("<td><input name=\"odds\" value=\"\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(24)\"></td>");
						HTMLString.Append("<td></td></tr>");
						HTMLString.Append("<tr align=\"center\"><th>4</th>");
						HTMLString.Append("<td><input name=\"odds\" value=\"\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(25)\"></td>");
						HTMLString.Append("<td><input name=\"odds\" value=\"\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(26)\"></td>");
						HTMLString.Append("<td><input name=\"odds\" value=\"\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(27)\"></td>");
						HTMLString.Append("<td><input name=\"odds\" value=\"\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(28)\"></td>");
						HTMLString.Append("<td><input name=\"odds\" value=\"\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(29)\"></td>");
						HTMLString.Append("<td><input name=\"odds\" value=\"\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(30)\"></td>");
						HTMLString.Append("<td></td></tr>");
						HTMLString.Append("<tr align=\"center\"><th><label id=\"h_5\">5</label></th>");
						HTMLString.Append("<td><input name=\"odds\" value=\"\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(31)\"></td>");
						HTMLString.Append("<td><input name=\"odds\" value=\"\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(32)\"></td>");
						HTMLString.Append("<td><input name=\"odds\" value=\"\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(33)\"></td>");
						HTMLString.Append("<td><input name=\"odds\" value=\"\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(34)\"></td>");
						HTMLString.Append("<td><input name=\"odds\" value=\"\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(35)\"></td>");
						HTMLString.Append("<td><input name=\"odds\" value=\"\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(36)\"></td>");
						HTMLString.Append("<td></td></tr>");
						HTMLString.Append("<tr align=\"center\"><th><label id=\"h_gt5\">&gt;5</label></th>");
						HTMLString.Append("<td><input name=\"odds\" value=\"\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(37)\"></td>");
						HTMLString.Append("<td></td><td></td><td></td><td></td><td></td>");
						HTMLString.Append("<td><input name=\"odds\" value=\"\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(38)\"></td></tr>");
						HTMLString.Append("<tr align=\"center\"><th colspan=\"9\"><input type=\"hidden\" name=\"guest\" value=\"" + sGuest + "\">(客) " + sGuest + "</th></tr>");
					}
				} else {
					if(oddsList.Count > 0) {	//contains odds data
						HTMLString.Append("<input type=\"radio\" name=\"MatrixSize\" value=\"5\" onClick=\"setMatrixSize()\" checked>淨勝4球以上&nbsp;<input type=\"radio\" name=\"MatrixSize\" value=\"6\" onClick=\"setMatrixSize()\">淨勝5球以上</td><th colspan=\"3\">執行動作：<select name=\"Action\"><option value=\"U\">新增/修改</option><option value=\"D\">刪除</option></select></th></tr>");
						HTMLString.Append("<tr align=\"center\"><th colspan=\"2\"><input type=\"hidden\" name=\"league\" value=\"" + sLeague + "\">" + sLeague + "</th><th>0</th><th>1</th><th>2</th><th>3</th><th>4</th><th><label id=\"g_5\">&gt;4</label></th><th><label id=\"g_gt5\">-</label></th></tr>");
						HTMLString.Append("<tr align=\"center\"><th width=\"10%\" rowspan=\"7\"><input type=\"hidden\" name=\"host\" value=\"" + sHost + "\">" + sHost + "<br>" + sMatchField + "</th><th>0</th>");
						HTMLString.Append("<td><input name=\"odds\" value=\"" + oddsList[0] + "\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(0)\"></td>");
						HTMLString.Append("<td><input name=\"odds\" value=\"" + oddsList[1] + "\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(1)\"></td>");
						HTMLString.Append("<td><input name=\"odds\" value=\"" + oddsList[2] + "\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(2)\"></td>");
						HTMLString.Append("<td><input name=\"odds\" value=\"" + oddsList[3] + "\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(3)\"></td>");
						HTMLString.Append("<td><input name=\"odds\" value=\"" + oddsList[4] + "\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(4)\"></td>");
						HTMLString.Append("<td><input name=\"odds\" value=\"" + oddsList[5] + "\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(5)\"></td>");
						HTMLString.Append("<td><input name=\"odds\" value=\"\" size=\"1\" maxlength=\"5\" style=\"background:#D3D3D3\"  onChange=\"OddsValidity(6)\" disabled></td>");
						HTMLString.Append("</tr>");
						HTMLString.Append("<tr align=\"center\"><th>1</th>");
						HTMLString.Append("<td><input name=\"odds\" value=\"" + oddsList[6] + "\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(7)\"></td>");
						HTMLString.Append("<td><input name=\"odds\" value=\"" + oddsList[7] + "\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(8)\"></td>");
						HTMLString.Append("<td><input name=\"odds\" value=\"" + oddsList[8] + "\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(9)\"></td>");
						HTMLString.Append("<td><input name=\"odds\" value=\"" + oddsList[9] + "\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(10)\"></td>");
						HTMLString.Append("<td><input name=\"odds\" value=\"" + oddsList[10] + "\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(11)\"></td>");
						HTMLString.Append("<td><input name=\"odds\" value=\"\" size=\"1\" maxlength=\"5\" style=\"background:#D3D3D3\" onChange=\"OddsValidity(12)\" disabled></td>");
						HTMLString.Append("<td></td></tr>");
						HTMLString.Append("<tr align=\"center\"><th>2</th>");
						HTMLString.Append("<td><input name=\"odds\" value=\"" + oddsList[11] + "\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(13)\"></td>");
						HTMLString.Append("<td><input name=\"odds\" value=\"" + oddsList[12] + "\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(14)\"></td>");
						HTMLString.Append("<td><input name=\"odds\" value=\"" + oddsList[13] + "\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(15)\"></td>");
						HTMLString.Append("<td><input name=\"odds\" value=\"" + oddsList[14] + "\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(16)\"></td>");
						HTMLString.Append("<td><input name=\"odds\" value=\"" + oddsList[15] + "\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(17)\"></td>");
						HTMLString.Append("<td><input name=\"odds\" value=\"\" size=\"1\" maxlength=\"5\" style=\"background:#D3D3D3\" onChange=\"OddsValidity(18)\" disabled></td>");
						HTMLString.Append("<td></td></tr>");
						HTMLString.Append("<tr align=\"center\"><th>3</th>");
						HTMLString.Append("<td><input name=\"odds\" value=\"" + oddsList[16] + "\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(19)\"></td>");
						HTMLString.Append("<td><input name=\"odds\" value=\"" + oddsList[17] + "\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(20)\"></td>");
						HTMLString.Append("<td><input name=\"odds\" value=\"" + oddsList[18] + "\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(21)\"></td>");
						HTMLString.Append("<td><input name=\"odds\" value=\"" + oddsList[19] + "\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(22)\"></td>");
						HTMLString.Append("<td><input name=\"odds\" value=\"" + oddsList[20] + "\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(23)\"></td>");
						HTMLString.Append("<td><input name=\"odds\" value=\"\" size=\"1\" maxlength=\"5\" style=\"background:#D3D3D3\" onChange=\"OddsValidity(24)\" disabled></td>");
						HTMLString.Append("<td></td></tr>");
						HTMLString.Append("<tr align=\"center\"><th>4</th>");
						HTMLString.Append("<td><input name=\"odds\" value=\"" + oddsList[21] + "\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(25)\"></td>");
						HTMLString.Append("<td><input name=\"odds\" value=\"" + oddsList[22] + "\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(26)\"></td>");
						HTMLString.Append("<td><input name=\"odds\" value=\"" + oddsList[23] + "\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(27)\"></td>");
						HTMLString.Append("<td><input name=\"odds\" value=\"" + oddsList[24] + "\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(28)\"></td>");
						HTMLString.Append("<td><input name=\"odds\" value=\"" + oddsList[25] + "\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(29)\"></td>");
						HTMLString.Append("<td><input name=\"odds\" value=\"\" size=\"1\" maxlength=\"5\" style=\"background:#D3D3D3\" onChange=\"OddsValidity(30)\" disabled></td>");
						HTMLString.Append("<td></td></tr>");
						HTMLString.Append("<tr align=\"center\"><th><label id=\"h_5\">&gt;4</label></th>");
						HTMLString.Append("<td><input name=\"odds\" value=\"" + oddsList[26] + "\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(31)\"></td>");
						HTMLString.Append("<td><input name=\"odds\" value=\"\" size=\"1\" maxlength=\"5\" style=\"background:#D3D3D3\" onChange=\"OddsValidity(32)\" disabled></td>");
						HTMLString.Append("<td><input name=\"odds\" value=\"\" size=\"1\" maxlength=\"5\" style=\"background:#D3D3D3\" onChange=\"OddsValidity(33)\" disabled></td>");
						HTMLString.Append("<td><input name=\"odds\" value=\"\" size=\"1\" maxlength=\"5\" style=\"background:#D3D3D3\" onChange=\"OddsValidity(34)\" disabled></td>");
						HTMLString.Append("<td><input name=\"odds\" value=\"\" size=\"1\" maxlength=\"5\" style=\"background:#D3D3D3\" onChange=\"OddsValidity(35)\" disabled></td>");
						HTMLString.Append("<td><input name=\"odds\" value=\"\" size=\"1\" maxlength=\"5\" style=\"background:#D3D3D3\" onChange=\"OddsValidity(36)\" disabled></td>");
						HTMLString.Append("<td></td></tr>");
						HTMLString.Append("<tr align=\"center\"><th><label id=\"h_gt5\">-</label></th>");
						HTMLString.Append("<td><input name=\"odds\" value=\"\" size=\"1\" maxlength=\"5\" style=\"background:#D3D3D3\" onChange=\"OddsValidity(37)\" disabled></td>");
						HTMLString.Append("<td></td><td></td><td></td><td></td><td></td>");
						HTMLString.Append("<td><input name=\"odds\" value=\"" + oddsList[27] + "\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(38)\"></td></tr>");
						HTMLString.Append("<tr align=\"center\"><th colspan=\"9\"><input type=\"hidden\" name=\"guest\" value=\"" + sGuest + "\">(客) " + sGuest + "</th></tr>");
					} else {
						HTMLString.Append("<input type=\"radio\" name=\"MatrixSize\" value=\"5\" onClick=\"setMatrixSize()\" checked>淨勝4球以上&nbsp;<input type=\"radio\" name=\"MatrixSize\" value=\"6\" onClick=\"setMatrixSize()\">淨勝5球以上</td><th colspan=\"3\">執行動作：<select name=\"Action\"><option value=\"U\">新增/修改</option><option value=\"D\">刪除</option></select></th></tr>");
						HTMLString.Append("<tr align=\"center\"><th colspan=\"2\"><input type=\"hidden\" name=\"league\" value=\"" + sLeague + "\">" + sLeague + "</th><th>0</th><th>1</th><th>2</th><th>3</th><th>4</th><th><label id=\"g_5\">&gt;4</label></th><th><label id=\"g_gt5\">-</label></th></tr>");
						HTMLString.Append("<tr align=\"center\"><th width=\"10%\" rowspan=\"7\"><input type=\"hidden\" name=\"host\" value=\"" + sHost + "\">" + sHost + "<br>" + sMatchField + "</th><th>0</th>");
						HTMLString.Append("<td><input name=\"odds\" value=\"\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(0)\"></td>");
						HTMLString.Append("<td><input name=\"odds\" value=\"\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(1)\"></td>");
						HTMLString.Append("<td><input name=\"odds\" value=\"\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(2)\"></td>");
						HTMLString.Append("<td><input name=\"odds\" value=\"\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(3)\"></td>");
						HTMLString.Append("<td><input name=\"odds\" value=\"\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(4)\"></td>");
						HTMLString.Append("<td><input name=\"odds\" value=\"\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(5)\"></td>");
						HTMLString.Append("<td><input name=\"odds\" value=\"\" size=\"1\" maxlength=\"5\" style=\"background:#D3D3D3\" onChange=\"OddsValidity(6)\" disabled></td>");
						HTMLString.Append("</tr>");
						HTMLString.Append("<tr align=\"center\"><th>1</th>");
						HTMLString.Append("<td><input name=\"odds\" value=\"\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(7)\"></td>");
						HTMLString.Append("<td><input name=\"odds\" value=\"\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(8)\"></td>");
						HTMLString.Append("<td><input name=\"odds\" value=\"\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(9)\"></td>");
						HTMLString.Append("<td><input name=\"odds\" value=\"\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(10)\"></td>");
						HTMLString.Append("<td><input name=\"odds\" value=\"\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(11)\"></td>");
						HTMLString.Append("<td><input name=\"odds\" value=\"\" size=\"1\" maxlength=\"5\" style=\"background:#D3D3D3\" onChange=\"OddsValidity(12)\" disabled></td>");
						HTMLString.Append("<td></td></tr>");
						HTMLString.Append("<tr align=\"center\"><th>2</th>");
						HTMLString.Append("<td><input name=\"odds\" value=\"\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(13)\"></td>");
						HTMLString.Append("<td><input name=\"odds\" value=\"\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(14)\"></td>");
						HTMLString.Append("<td><input name=\"odds\" value=\"\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(15)\"></td>");
						HTMLString.Append("<td><input name=\"odds\" value=\"\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(16)\"></td>");
						HTMLString.Append("<td><input name=\"odds\" value=\"\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(17)\"></td>");
						HTMLString.Append("<td><input name=\"odds\" value=\"\" size=\"1\" maxlength=\"5\" style=\"background:#D3D3D3\" onChange=\"OddsValidity(18)\" disabled></td>");
						HTMLString.Append("<td></td></tr>");
						HTMLString.Append("<tr align=\"center\"><th>3</th>");
						HTMLString.Append("<td><input name=\"odds\" value=\"\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(19)\"></td>");
						HTMLString.Append("<td><input name=\"odds\" value=\"\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(20)\"></td>");
						HTMLString.Append("<td><input name=\"odds\" value=\"\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(21)\"></td>");
						HTMLString.Append("<td><input name=\"odds\" value=\"\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(22)\"></td>");
						HTMLString.Append("<td><input name=\"odds\" value=\"\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(23)\"></td>");
						HTMLString.Append("<td><input name=\"odds\" value=\"\" size=\"1\" maxlength=\"5\" style=\"background:#D3D3D3\" onChange=\"OddsValidity(24)\" disabled></td>");
						HTMLString.Append("<td></td></tr>");
						HTMLString.Append("<tr align=\"center\"><th>4</th>");
						HTMLString.Append("<td><input name=\"odds\" value=\"\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(25)\"></td>");
						HTMLString.Append("<td><input name=\"odds\" value=\"\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(26)\"></td>");
						HTMLString.Append("<td><input name=\"odds\" value=\"\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(27)\"></td>");
						HTMLString.Append("<td><input name=\"odds\" value=\"\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(28)\"></td>");
						HTMLString.Append("<td><input name=\"odds\" value=\"\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(29)\"></td>");
						HTMLString.Append("<td><input name=\"odds\" value=\"\" size=\"1\" maxlength=\"5\" style=\"background:#D3D3D3\" onChange=\"OddsValidity(30)\" disabled></td>");
						HTMLString.Append("<td></td></tr>");
						HTMLString.Append("<tr align=\"center\"><th><label id=\"h_5\">&gt;4</label></th>");
						HTMLString.Append("<td><input name=\"odds\" value=\"\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(31)\"></td>");
						HTMLString.Append("<td><input name=\"odds\" value=\"\" size=\"1\" maxlength=\"5\" style=\"background:#D3D3D3\" onChange=\"OddsValidity(32)\" disabled></td>");
						HTMLString.Append("<td><input name=\"odds\" value=\"\" size=\"1\" maxlength=\"5\" style=\"background:#D3D3D3\" onChange=\"OddsValidity(33)\" disabled></td>");
						HTMLString.Append("<td><input name=\"odds\" value=\"\" size=\"1\" maxlength=\"5\" style=\"background:#D3D3D3\" onChange=\"OddsValidity(34)\" disabled></td>");
						HTMLString.Append("<td><input name=\"odds\" value=\"\" size=\"1\" maxlength=\"5\" style=\"background:#D3D3D3\" onChange=\"OddsValidity(35)\" disabled></td>");
						HTMLString.Append("<td><input name=\"odds\" value=\"\" size=\"1\" maxlength=\"5\" style=\"background:#D3D3D3\" onChange=\"OddsValidity(36)\" disabled></td>");
						HTMLString.Append("<td></td></tr>");
						HTMLString.Append("<tr align=\"center\"><th><label id=\"h_gt5\">-</label></th>");
						HTMLString.Append("<td><input name=\"odds\" value=\"\" size=\"1\" maxlength=\"5\" style=\"background:#D3D3D3\" onChange=\"OddsValidity(37)\" disabled></td>");
						HTMLString.Append("<td></td><td></td><td></td><td></td><td></td>");
						HTMLString.Append("<td><input name=\"odds\" value=\"\" size=\"1\" maxlength=\"5\" onChange=\"OddsValidity(38)\"></td></tr>");
						HTMLString.Append("<tr align=\"center\"><th colspan=\"9\"><input type=\"hidden\" name=\"guest\" value=\"" + sGuest + "\">(客) " + sGuest + "</th></tr>");
					}
				}
				HTMLString.Append("<input type=\"hidden\" name=\"matchcount\" value=\"" + sMatchCount + "\">");
				HTMLString.Append("<input type=\"hidden\" name=\"matchdate\" value=\"" + sMatchDate + "\">");
				HTMLString.Append("<input type=\"hidden\" name=\"matchtime\" value=\"" + sMatchTime + "\">");
			}	catch(Exception ex) {
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " CorrectScore.cs.GetMatrix(): " + ex.ToString());
				m_SportsLog.Close();
				HTMLString.Remove(0,HTMLString.Length);
				HTMLString.Append((string)HttpContext.Current.Application["accessErrorMsg"]);
			}

			return HTMLString.ToString();
		}

		public int Modify() {
			int iScoreCount;
			string sLeague;
			string sHost;
			string sGuest;
			string sMatchCount;
			string sAction;
			string sMatrixSize;
			string sOdds;
			string sMatchDate;
			string sMatchTime;
			string sCurrentTimestamp = null;
			string sBatchJob = null;
			char[] delimiter = new char[] {','};
			string[] arrOdds;
			string[] arrMsgType;
			string[] arrSendToPager;
			string[] arrRemotingPath;

			//SportsMessage object message
			SportsMessage sptMsg = new SportsMessage();
			StringBuilder LogSQLString = new StringBuilder();
			DBManager logDBMgr = new DBManager();
			logDBMgr.ConnectionString = m_SportsDBMgr.ConnectionString;
			arrRemotingPath = (string[])HttpContext.Current.Application["RemotingItems"];

			try {
				arrSendToPager = HttpContext.Current.Request.Form["SendToPager"].Split(delimiter);
			} catch(Exception) {
				arrSendToPager = new string[0];
			}
			arrMsgType = (string[])HttpContext.Current.Application["messageType"];
			sLeague = HttpContext.Current.Request.Form["league"];
			sHost = HttpContext.Current.Request.Form["host"];
			sGuest = HttpContext.Current.Request.Form["guest"];
			sMatchCount = HttpContext.Current.Request.Form["matchcount"];
			sAction = HttpContext.Current.Request.Form["Action"];
			sMatrixSize = HttpContext.Current.Request.Form["MatrixSize"];
			sMatchDate = HttpContext.Current.Request.Form["matchdate"];
			sMatchTime = HttpContext.Current.Request.Form["matchtime"];
			arrOdds = HttpContext.Current.Request.Form["odds"].Split(delimiter);
			iScoreCount = arrOdds.Length;

			/*****************************
			 * GoGo Pager2 alert message *
			 *****************************/
			string[] arrQueueNames;
			string[] arrMessageTypes;
			arrQueueNames = (string[])HttpContext.Current.Application["QueueItems"];
			arrMessageTypes = (string[])HttpContext.Current.Application["NotifyMessageTypes"];
			MessageClient msgClt = new MessageClient();
			msgClt.MessageType = arrMessageTypes[0];
			msgClt.MessagePath = arrQueueNames[0];

			try {
				sCurrentTimestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
				sBatchJob = DateTime.Now.ToString("yyMMddHHmmss_fffffff") + "." + HttpContext.Current.Session["user_name"].ToString() + "." + arrMsgType[24] + ".ini";
				if(sAction.Equals("U")) {
					//Clear previous record first
					SQLString.Remove(0,SQLString.Length);
					SQLString.Append("delete from CORRECTSCORE_INFO where IMATCH_CNT=");
					SQLString.Append(sMatchCount);
					m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
					m_SportsDBMgr.Close();

					//insert odds into CORRECTSCORE_INFO and LOG_CORRECTSCORE
					for(int i = 0; i < iScoreCount; i++) {
						if(arrOdds[i].Trim().Equals("")) sOdds = "-1";
						else sOdds = arrOdds[i].Trim();
						SQLString.Remove(0,SQLString.Length);
						SQLString.Append("insert into CORRECTSCORE_INFO values(");
						SQLString.Append(sMatchCount);
						SQLString.Append(",");
						SQLString.Append(i.ToString());
						SQLString.Append(",'");
						SQLString.Append(sOdds);
						SQLString.Append("','U',");
						SQLString.Append(sMatrixSize);
						SQLString.Append(")");
						m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
						m_SportsDBMgr.Close();

						if(arrSendToPager.Length>0) {
							LogSQLString.Remove(0,LogSQLString.Length);
							LogSQLString.Append("insert into LOG_CORRECTSCORE (TIMEFLAG, IITEMSEQ_NO, SECTION, LEAGUE, HOST, GUEST, ACT, MATCHDATE, MATCHTIME, MATRIXSIZE, ODDS, BATCHJOB) values ('");
							LogSQLString.Append(sCurrentTimestamp);
							LogSQLString.Append("',");
							LogSQLString.Append((i+1).ToString());
							LogSQLString.Append(",'CORRECTSCORE_','");
							LogSQLString.Append(sLeague);
							LogSQLString.Append("','");
							LogSQLString.Append(sHost);
							LogSQLString.Append("','");
							LogSQLString.Append(sGuest);
							LogSQLString.Append("','");
							LogSQLString.Append(sAction);
							LogSQLString.Append("','");
							LogSQLString.Append(sMatchDate);
							LogSQLString.Append("','");
							LogSQLString.Append(sMatchTime);
							LogSQLString.Append("',");
							LogSQLString.Append(sMatrixSize);
							LogSQLString.Append(",'");
							LogSQLString.Append(sOdds);
							LogSQLString.Append("','");
							LogSQLString.Append(sBatchJob);
							LogSQLString.Append("')");
							logDBMgr.ExecuteNonQuery(LogSQLString.ToString());
							logDBMgr.Close();
						}
					}

					if(arrSendToPager.Length>0) {
						//Send Notify Message				
						//Modified by Henry, 09 Feb 2004
						sptMsg.Body = sBatchJob;
						sptMsg.Timestamp = DateTime.Now.ToString("yyMMddHHmmss");
						sptMsg.AppID = "07";
						sptMsg.MsgID = "19";
						sptMsg.DeviceID = new string[0];						
						for(int i = 0; i < arrSendToPager.Length; i++) {
							sptMsg.AddDeviceID((string)arrSendToPager[i]);
						}						
						try {
							//Notify via MSMQ
							msgClt.MessageType = arrMessageTypes[0];
							msgClt.MessagePath = arrQueueNames[0];
							msgClt.SendMessage(sptMsg);
						} catch(System.Messaging.MessageQueueException mqEx) {
							try {
								m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
								m_SportsLog.SetFileName(0,LOGFILESUFFIX);
								m_SportsLog.Open();
								m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " MSMQ ERROR: Correct Score");
								m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " CorrectScore.cs.Modify() Notify via MSMQ throws MessageQueueException:  " + mqEx.ToString());
								m_SportsLog.Close();

								//If MSMQ fail, notify via .NET Remoting
								msgClt.MessageType = arrMessageTypes[1];
								msgClt.MessagePath = arrRemotingPath[0];
								if(!msgClt.SendMessage((object)sptMsg)) {
									m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
									m_SportsLog.SetFileName(0,LOGFILESUFFIX);
									m_SportsLog.Open();
									m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Remoting ERROR: Correct Score");
									m_SportsLog.Close();
								}
							}	catch(Exception ex) {
								m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
								m_SportsLog.SetFileName(0,LOGFILESUFFIX);
								m_SportsLog.Open();
								m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Remoting ERROR: Correct Score");
								m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " CorrectScore.cs.Modify() Notify via .NET Remoting throws exception: " + ex.ToString());
								m_SportsLog.Close();
							}							
						}	catch(Exception ex) {
							m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
							m_SportsLog.SetFileName(0,LOGFILESUFFIX);
							m_SportsLog.Open();
							m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " CorrectScore.cs.Modify() Notify via MSMQ throws exception: " + ex.ToString());
							m_SportsLog.Close();
						}		
					
					//Modified end
					}
					//write log
					m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
					m_SportsLog.SetFileName(0,LOGFILESUFFIX);
					m_SportsLog.Open();
					m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " CorrectScore.cs: Update " + iScoreCount.ToString() + " records <L:" + sLeague + ", H:" + sHost + ", G:" + sGuest + "> (" + HttpContext.Current.Session["user_name"] + ")");
					m_SportsLog.Close();
				} else {
					SQLString.Remove(0,SQLString.Length);
					SQLString.Append("update CORRECTSCORE_INFO set CACT='D' where IMATCH_CNT=");
					SQLString.Append(sMatchCount);
					m_SportsDBMgr.ExecuteNonQuery(SQLString.ToString());
					m_SportsDBMgr.Close();

					if(arrSendToPager.Length>0) {
						LogSQLString.Remove(0,LogSQLString.Length);
						LogSQLString.Append("insert into LOG_CORRECTSCORE (TIMEFLAG, IITEMSEQ_NO, SECTION, LEAGUE, HOST, GUEST, ACT, MATCHDATE, MATCHTIME, MATRIXSIZE, ODDS, BATCHJOB) values ('");
						LogSQLString.Append(sCurrentTimestamp);
						LogSQLString.Append("',null,'CORRECTSCORE_','");
						LogSQLString.Append(sLeague);
						LogSQLString.Append("','");
						LogSQLString.Append(sHost);
						LogSQLString.Append("','");
						LogSQLString.Append(sGuest);
						LogSQLString.Append("','");
						LogSQLString.Append(sAction);
						LogSQLString.Append("','");
						LogSQLString.Append(sMatchDate);
						LogSQLString.Append("','");
						LogSQLString.Append(sMatchTime);
						LogSQLString.Append("',");
						LogSQLString.Append(sMatrixSize);
						LogSQLString.Append(",null,'");
						LogSQLString.Append(sBatchJob);
						LogSQLString.Append("')");
						logDBMgr.ExecuteNonQuery(LogSQLString.ToString());
						logDBMgr.Close();

						//Send Notify Message
						
						//Modified by Henry, 09 Feb 2004	
						sptMsg.Body = sBatchJob;
						sptMsg.Timestamp = DateTime.Now.ToString("yyMMddHHmmss");
						sptMsg.AppID = "07";
						sptMsg.MsgID = "19";
						sptMsg.DeviceID = new string[0];						
						for(int i = 0; i < arrSendToPager.Length; i++) {
							sptMsg.AddDeviceID((string)arrSendToPager[i]);
						}					
						try {
							//Notify via MSMQ
							msgClt.MessageType = arrMessageTypes[0];
							msgClt.MessagePath = arrQueueNames[0];
							msgClt.SendMessage(sptMsg);
						} catch(System.Messaging.MessageQueueException mqEx) {
							try {
								m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
								m_SportsLog.SetFileName(0,LOGFILESUFFIX);
								m_SportsLog.Open();
								m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " MSMQ ERROR: Correct Score");
								m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " CorrectScore.cs.Modify() Notify via MSMQ throws MessageQueueException:  " + mqEx.ToString());
								m_SportsLog.Close();

								//If MSMQ fail, notify via .NET Remoting
								msgClt.MessageType = arrMessageTypes[1];
								msgClt.MessagePath = arrRemotingPath[0];
								if(!msgClt.SendMessage((object)sptMsg)) {
									m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
									m_SportsLog.SetFileName(0,LOGFILESUFFIX);
									m_SportsLog.Open();
									m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Remoting ERROR: Correct Score");
									m_SportsLog.Close();
								}
							}	catch(Exception ex) {
								m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
								m_SportsLog.SetFileName(0,LOGFILESUFFIX);
								m_SportsLog.Open();
								m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " Remoting ERROR: Correct Score");
								m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " CorrectScore.cs.Modify() Notify via .NET Remoting throws exception: " + ex.ToString());
								m_SportsLog.Close();
							}							
						}	catch(Exception ex) {
							m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
							m_SportsLog.SetFileName(0,LOGFILESUFFIX);
							m_SportsLog.Open();
							m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " CorrectScore.cs.Modify() Notify via MSMQ throws exception: " + ex.ToString());
							m_SportsLog.Close();
						}
						//Modified end
					}

					//write log
					m_SportsLog.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
					m_SportsLog.SetFileName(0,LOGFILESUFFIX);
					m_SportsLog.Open();
					m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " CorrectScore.cs: Delete records <L:" + sLeague + ", H:" + sHost + ", G:" + sGuest + "> (" + HttpContext.Current.Session["user_name"] + ")");
					m_SportsLog.Close();
				}
				m_SportsDBMgr.Dispose();
			}	catch(Exception ex) {
				iScoreCount = -1;
				m_SportsLog.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SportsLog.SetFileName(0,LOGFILESUFFIX);
				m_SportsLog.Open();
				m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " CorrectScore.cs.Modify(): " + ex.ToString());
				m_SportsLog.Close();
			}

			return iScoreCount;
		}
	}
}