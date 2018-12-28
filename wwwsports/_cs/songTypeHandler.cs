/*
Objective:
Read song type from web.config <songType>

Last updated:
19 June 2003 by Chapman

C#.NET complier statement:
csc /t:library /out:..\bin\songTypeHandler.dll songTypeHandler.cs
*/

using System;
using System.Collections;
using System.Configuration;
using System.Reflection;
using System.Xml;

[assembly:AssemblyCompany("TDSL")]
[assembly:AssemblyCopyright("(c) 2003 TDSL. All rights reserved.")]
[assembly:AssemblyDescription("體育資訊 -> Read song type config")]
[assembly:AssemblyProduct("Sports.NET")]
[assembly:AssemblyTitle("Sports.NET DLL")]
[assembly:AssemblyVersion("1.0.*")]
namespace SportsItemHandler {
	public class songTypeHandler:IConfigurationSectionHandler {
		public Object Create(Object parent, Object configContext, System.Xml.XmlNode section) {
			String _id, _name;
			XmlNodeList songNodeList;
			ArrayList songList = new ArrayList();
			songNodeList = section.SelectNodes("song");

			foreach(XmlNode pathNode in songNodeList) {
				_id = pathNode.Attributes.GetNamedItem("id").Value;
				_name = pathNode.Attributes.GetNamedItem("name").Value;

				songList.Add(new songEntity(_id, _name));
			}

			return songList;
		}
	}

	public class songEntity {
		private String sID, sName;

		public songEntity(String _inID, String _inName) {
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