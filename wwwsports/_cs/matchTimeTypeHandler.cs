/*
Objective:
Read match time from web.config <matchTimeType>

Last updated:
19 June 2003 by Chapman

C#.NET complier statement:
csc /t:library /out:..\bin\matchTimeTypeHandler.dll matchTimeTypeHandler.cs
*/

using System;
using System.Collections;
using System.Configuration;
using System.Reflection;
using System.Xml;

[assembly:AssemblyCompany("TDSL")]
[assembly:AssemblyCopyright("(c) 2003 TDSL. All rights reserved.")]
[assembly:AssemblyDescription("體育資訊 -> Read match time config")]
[assembly:AssemblyProduct("Sports.NET")]
[assembly:AssemblyTitle("Sports.NET DLL")]
[assembly:AssemblyVersion("1.0.*")]
namespace SportsItemHandler {
	public class matchTimeTypeHandler:IConfigurationSectionHandler {
		public Object Create(Object parent, Object configContext, System.Xml.XmlNode section) {
			String _id, _name;
			XmlNodeList matchTimeNodeList;
			ArrayList matchTimeList = new ArrayList();
			matchTimeNodeList = section.SelectNodes("matchTime");

			foreach(XmlNode pathNode in matchTimeNodeList) {
				_id = pathNode.Attributes.GetNamedItem("id").Value;
				_name = pathNode.Attributes.GetNamedItem("name").Value;

				matchTimeList.Add(new matchTimeEntity(_id, _name));
			}

			return matchTimeList;
		}
	}

	public class matchTimeEntity {
		private String sID, sName;

		public matchTimeEntity(String _inID, String _inName) {
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