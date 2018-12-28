/*
Objective:
Sort league display and sent order

Last updated:
12 July 2004, Chapman Choi

C#.NET complier statement:
csc /t:library /out:..\bin\LeagueSort.dll /r:..\bin\DBManager.dll;..\bin\Files.dll LeagueSort.cs
*/

using System;
using System.Data.OleDb;
using System.Reflection;
using System.Web;
using TDL.DB;
using TDL.IO;

[assembly:AssemblyCompany("TDSL")]
[assembly:AssemblyCopyright("(c) 2003 TDSL. All rights reserved.")]
[assembly:AssemblyDescription("一般設定 -> 修改聯賽序號")]
[assembly:AssemblyProduct("Sports.NET")]
[assembly:AssemblyTitle("Sports.NET DLL")]
[assembly:AssemblyVersion("1.0.*")]
namespace SportsUtil {
	public class LeagueSort {
		const string LOGFILESUFFIX = "log";
		int m_RecordCount = 0;
		OleDbDataReader m_SortLeagReader;
		DBManager m_SortDB;
		Files m_SortFile;

		public LeagueSort(string Connection) {
			m_SortDB = new DBManager();
			m_SortDB.ConnectionString = Connection;
			m_SortFile = new Files();
		}

		public int NumberOfRecords {
			get {
				return m_RecordCount;
			}
		}

		public string GetLeagues() {
			string retrieveQuery, sRecordStr, sRtn = "";
			
			retrieveQuery = "select leag_order, alias, leagname, leag_id from leaginfo order by leag_order, alias, leagname";
			try {
				m_SortLeagReader = m_SortDB.ExecuteQuery(retrieveQuery);
				while(m_SortLeagReader.Read()) {
					sRtn += "<tr align=\"center\">";

					//League Order
					if(m_SortLeagReader.IsDBNull(0)) sRecordStr = "";
					else sRecordStr = m_SortLeagReader.GetInt32(0).ToString();
					sRtn += "<td><input type=\"text\" name=\"leag_order\" maxlength=\"2\" size=\"2\" ";
					sRtn += "value=\"" + sRecordStr + "\" onChange=\"OrderValidator(" + m_RecordCount.ToString() + ")\"></td>";

					//League Alias
					sRecordStr = m_SortLeagReader.GetString(1);
					sRtn += "<td>" + sRecordStr + "</td>";

					//League Full Name
					sRecordStr = m_SortLeagReader.GetString(2);
					sRtn += "<td>" + sRecordStr;

					//Hidden field: League ID
					sRecordStr = m_SortLeagReader.GetString(3);
					sRtn += "<input type=\"hidden\" name=\"LeagID\" value=\"" + sRecordStr + "\"></td></tr>";
					m_RecordCount++;
				}
				m_SortLeagReader.Close();
				m_SortDB.Close();
			} catch(Exception ex) {
				m_SortFile.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SortFile.SetFileName(0,LOGFILESUFFIX);
				m_SortFile.Open();
				m_SortFile.Write(DateTime.Now.ToString("HH:mm:ss") + " LeagueSort.cs.GetLeagues(): " + ex.ToString());
				m_SortFile.Close();
				sRtn = (string)HttpContext.Current.Application["accessErrorMsg"];
			}

			return sRtn;
		}

		public int Sort() {
			int iUpdIndex, iRecUpd = 0;
			string sTempOrder, updateQuery = "";
			char[] delimiter = new char[] {','};
			string[] sOrderArr, sLeagIDArr;

			sOrderArr = HttpContext.Current.Request.Form["leag_order"].Split(delimiter);
			sLeagIDArr = HttpContext.Current.Request.Form["LeagID"].Split(delimiter);
			iRecUpd = sOrderArr.Length;
			try {
				for(iUpdIndex=0;iUpdIndex<iRecUpd;iUpdIndex++) {
					sTempOrder = sOrderArr[iUpdIndex];
					if(sTempOrder == null) sTempOrder = "null";
					if(sTempOrder.Equals("")) sTempOrder = "null";
					updateQuery = "update leaginfo set leag_order=" + sTempOrder + " where leag_id = '" + sLeagIDArr[iUpdIndex].Trim() + "'";
					m_SortDB.ExecuteNonQuery(updateQuery);
					m_SortDB.Close();
				}

				//write log
				m_SortFile.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
				m_SortFile.SetFileName(0,LOGFILESUFFIX);
				m_SortFile.Open();
				m_SortFile.Write(DateTime.Now.ToString("HH:mm:ss") + " LeagueSort.cs: Sort " + iRecUpd.ToString() + " leagues (" + HttpContext.Current.Session["user_name"] + ")");
				m_SortFile.Close();
			} catch(Exception ex) {
				iRecUpd = -1;
				m_SortFile.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				m_SortFile.SetFileName(0,LOGFILESUFFIX);
				m_SortFile.Open();
				m_SortFile.Write(DateTime.Now.ToString("HH:mm:ss") + " LeagueSort.cs.Sort(): " + ex.ToString());
				m_SortFile.Close();
			}

			return iRecUpd;
		}
	}
}