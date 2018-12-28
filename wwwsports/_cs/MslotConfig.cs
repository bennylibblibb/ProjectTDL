/*
Objective:
Enable/disable auto sent-out for www.macauslot.com

Last updated:
19 June 2003 by Chapman

C#.NET complier statement:
csc /t:library /out:..\bin\MslotConfig.dll /r:..\bin\ConfigReader.dll;..\bin\Files.dll MslotConfig.cs
*/

using System;
using System.Reflection;
using System.Web;
using TDL.IO;
using TDL.Util;

[assembly:AssemblyCompany("TDSL")]
[assembly:AssemblyCopyright("(c) 2003 TDSL. All rights reserved.")]
[assembly:AssemblyDescription("其他地區 -> 澳門網自動更新設定 -> 設定")]
[assembly:AssemblyProduct("Sports.NET")]
[assembly:AssemblyTitle("Sports.NET DLL")]
[assembly:AssemblyVersion("1.0.*")]
namespace SportsUtil {
	public class MslotConfig {
		const string LOGFILESUFFIX = "log";

		public string GetXMLConfig(string sPath, string sItem) {
			string sRtn, sFlag;
			ConfigReader cr = new ConfigReader();
			cr.SetPath(sPath);
			sFlag = cr.GetValue(sItem);
			if(sFlag.Equals("1")) {
				sRtn = "<input type=\"checkbox\" name=\"MslotEnable\" value=\"1\" checked>";
			} else {
				sRtn = "<input type=\"checkbox\" name=\"MslotEnable\" value=\"1\">";
			}
			return sRtn;
		}

		public string SetXMLConfig(string sPath, string sItem) {
			string sFlag;
			Files mslotFile = new Files();
			sFlag = HttpContext.Current.Request.Form["MslotEnable"];
			if(sFlag == null) sFlag = "0";
			ConfigReader crUpd = new ConfigReader();
			try {
				crUpd.SetPath(sPath);
				crUpd.SetValue(sItem,sFlag);

				//write log
				mslotFile.FilePath = HttpContext.Current.Application["EventFilePath"].ToString();
				mslotFile.SetFileName(0,LOGFILESUFFIX);
				mslotFile.Open();
				mslotFile.Write(DateTime.Now.ToString("HH:mm:ss") + " MslotConfig.cs: Set auto-update flag=" + sFlag + " (" + HttpContext.Current.Session["user_name"] + ")");
				mslotFile.Close();
			} catch(Exception ex) {
				mslotFile.FilePath = HttpContext.Current.Application["ErrorFilePath"].ToString();
				mslotFile.SetFileName(0,LOGFILESUFFIX);
				mslotFile.Open();
				mslotFile.Write(DateTime.Now.ToString("HH:mm:ss") + " MslotConfig.cs.SetXMLConfig(): " + ex.ToString());
				mslotFile.Close();
			}

			return sFlag;
		}
	}
}