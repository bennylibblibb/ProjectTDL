/*
Objective:
Read Broadcast Option info such as type, message string from web.config <BroadcastOption>

Last updated:
29 Jan 2004 by Chapman

C#.NET complier statement:
csc /t:library /out:..\bin\BroadcastOptionHandler.dll BroadcastOptionHandler.cs
*/

using System;
using System.Collections;
using System.Configuration;
using System.Reflection;
using System.Xml;

[assembly:AssemblyCompany("TDSL")]
[assembly:AssemblyCopyright("(c) 2004 TDSL. All rights reserved.")]
[assembly:AssemblyDescription("體育資訊 -> Read Broadcast Option config")]
[assembly:AssemblyProduct("Sports.NET")]
[assembly:AssemblyTitle("Sports.NET DLL")]
[assembly:AssemblyVersion("1.0.*")]
namespace SportsItemHandler {
	public class BroadcastOptionHandler:IConfigurationSectionHandler {
		public Object Create(Object parent, Object configContext, System.Xml.XmlNode section) {
			string _type, _msg;
			XmlNodeList BCStrNodeList;
			ArrayList BCStrList = new ArrayList();
			BCStrNodeList = section.SelectNodes("option");

			foreach(XmlNode BCStrNode in BCStrNodeList) {
				_type = BCStrNode.Attributes.GetNamedItem("type").Value;
				_msg = BCStrNode.Attributes.GetNamedItem("msg").Value;

				BCStrList.Add(new BCStrEntity(_type, _msg));
			}

			return BCStrList;
		}
	}

	public class BCStrEntity {
		private string sType, sMsg;

		public BCStrEntity(string _inType, string _inMsg) {
			sType = _inType;
			sMsg = _inMsg;
		}

		public string TYPE {
			get {
				return sType;
			}
		}

		public string MSG {
			get {
				return sMsg;
			}
		}
	}
}