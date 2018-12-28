/*
Objective:
Read HKJC admin task from web.config <HKJCAdminTask>

Last updated:
28 July 2003 by Chapman

C#.NET complier statement:
csc /t:library /out:..\bin\HKJCAdminTaskHandler.dll HKJCAdminTaskHandler.cs
*/

using System;
using System.Collections;
using System.Configuration;
using System.Reflection;
using System.Xml;

[assembly:AssemblyCompany("TDSL")]
[assembly:AssemblyCopyright("(c) 2003 TDSL. All rights reserved.")]
[assembly:AssemblyDescription("體育資訊 -> Read HKJC Admin task config")]
[assembly:AssemblyProduct("Sports.NET")]
[assembly:AssemblyTitle("Sports.NET DLL")]
[assembly:AssemblyVersion("1.0.*")]
namespace SportsItemHandler {
	public class HKJCAdminTaskHandler:IConfigurationSectionHandler {
		public Object Create(Object parent, Object configContext, System.Xml.XmlNode section) {
			String _id, _name;
			XmlNodeList HKJCAdminTaskNodeList;
			ArrayList HKJCAdminTaskList = new ArrayList();
			HKJCAdminTaskNodeList = section.SelectNodes("task");

			foreach(XmlNode pathNode in HKJCAdminTaskNodeList) {
				_name = pathNode.Attributes.GetNamedItem("name").Value;
				_id = pathNode.Attributes.GetNamedItem("id").Value;

				HKJCAdminTaskList.Add(new HKJCTaskEntity(_id, _name));
			}

			return HKJCAdminTaskList;
		}
	}

	public class HKJCTaskEntity {
		private String sID, sName;

		public HKJCTaskEntity(String _inID, String _inName) {
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