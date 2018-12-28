/*
Objective:
Read DB info such as connection string from web.config <DBConfig>

Last updated:
19 June 2003 by Chapman

C#.NET complier statement:
csc /t:library /out:..\bin\DBConfigHandler.dll DBConfigHandler.cs
*/

using System;
using System.Collections;
using System.Configuration;
using System.Reflection;
using System.Xml;

[assembly:AssemblyCompany("TDSL")]
[assembly:AssemblyCopyright("(c) 2003 TDSL. All rights reserved.")]
[assembly:AssemblyDescription("體育資訊 -> Read DB config")]
[assembly:AssemblyProduct("Sports.NET")]
[assembly:AssemblyTitle("Sports.NET DLL")]
[assembly:AssemblyVersion("1.0.*")]
namespace SportsItemHandler {
	public class DBConfigHandler:IConfigurationSectionHandler {
		public Object Create(Object parent, Object configContext, System.Xml.XmlNode section) {
			String _app, _src, _connStr;
			XmlNodeList DBNodeList;
			ArrayList DBList = new ArrayList();
			DBNodeList = section.SelectNodes("DB");

			foreach(XmlNode dbNode in DBNodeList) {
				_app = dbNode.Attributes.GetNamedItem("app").Value;
				_src = dbNode.Attributes.GetNamedItem("src").Value;
				_connStr = dbNode.Attributes.GetNamedItem("connStr").Value;

				DBList.Add(new DBEntity(_app, _src, _connStr));
			}

			return DBList;
		}
	}

	public class DBEntity {
		private String sApp, sSrc, sConnStr;

		public DBEntity(String _inApp, String _inSrc, String _inConnStr) {
			sApp = _inApp;
			sSrc = _inSrc;
			sConnStr = _inConnStr;
		}

		public String APP {
			get {
				return sApp;
			}
		}

		public String SRC {
			get {
				return sSrc;
			}
		}

		public String CONNSTR {
			get {
				return sConnStr;
			}
		}
	}
}