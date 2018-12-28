/*
Objective:
Read file info such as path, name from web.config <filePath>

Last updated:
19 June 2003 by Chapman

C#.NET complier statement:
csc /t:library /out:..\bin\filePathHandler.dll filePathHandler.cs
*/

using System;
using System.Collections;
using System.Configuration;
using System.Reflection;
using System.Xml;

[assembly:AssemblyCompany("TDSL")]
[assembly:AssemblyCopyright("(c) 2003 TDSL. All rights reserved.")]
[assembly:AssemblyDescription("體育資訊 -> Read file config")]
[assembly:AssemblyProduct("Sports.NET")]
[assembly:AssemblyTitle("Sports.NET DLL")]
[assembly:AssemblyVersion("1.0.*")]
namespace SportsItemHandler {
	public class filePathHandler:IConfigurationSectionHandler {
		public Object Create(Object parent, Object configContext, System.Xml.XmlNode section) {
			String _type, _path;
			XmlNodeList pathNodeList;
			ArrayList pathList = new ArrayList();
			pathNodeList = section.SelectNodes("file");

			foreach(XmlNode pathNode in pathNodeList) {
				_type = pathNode.Attributes.GetNamedItem("type").Value;
				_path = pathNode.Attributes.GetNamedItem("path").Value;

				pathList.Add(new pathEntity(_type, _path));
			}

			return pathList;
		}
	}

	public class pathEntity {
		private String sType, sPath;

		public pathEntity(String _inType, String _inPath) {
			sType = _inType;
			sPath = _inPath;
		}

		public String TYPE {
			get {
				return sType;
			}
		}

		public String PATH {
			get {
				return sPath;
			}
		}
	}
}