/*
Objective:
Read match status from web.config <matchStatusType>

Last updated:
19 June 2003 by Chapman

C#.NET complier statement:
csc /t:library /out:..\bin\matchStatusTypeHandler.dll matchStatusTypeHandler.cs
*/

using System;
using System.Collections;
using System.Configuration;
using System.Reflection;
using System.Xml;

[assembly:AssemblyCompany("TDSL")]
[assembly:AssemblyCopyright("(c) 2003 TDSL. All rights reserved.")]
[assembly:AssemblyDescription("體育資訊 -> Read match status config")]
[assembly:AssemblyProduct("Sports.NET")]
[assembly:AssemblyTitle("Sports.NET DLL")]
[assembly:AssemblyVersion("1.0.*")]
namespace SportsItemHandler {
	public class matchStatusTypeHandler:IConfigurationSectionHandler {
		public Object Create(Object parent, Object configContext, System.Xml.XmlNode section) {
			String _id, _name;
			XmlNodeList matchStatusNodeList;
			ArrayList matchStatusList = new ArrayList();
			matchStatusNodeList = section.SelectNodes("matchStatus");

			foreach(XmlNode pathNode in matchStatusNodeList) {
				_id = pathNode.Attributes.GetNamedItem("id").Value;
				_name = pathNode.Attributes.GetNamedItem("name").Value;

				matchStatusList.Add(new matchStatusEntity(_id, _name));
			}

			return matchStatusList;
		}
	}

	public class matchStatusEntity {
		private String sID, sName;

		public matchStatusEntity(String _inID, String _inName) {
			sID = _inID;
			sName = _inName;
		}

		public String ID {
			get {
				return sID;
			}
		}

		public String NAME {
			get {
				return sName;
			}
		}
	}
}