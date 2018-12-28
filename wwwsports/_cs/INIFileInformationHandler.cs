/*
Objective:
Read the key and section value of ini file from web.config <INIFileInformation>

Last updated:
19 June 2003 by Chapman

C#.NET complier statement:
csc /t:library /out:..\bin\INIFileInformationHandler.dll INIFileInformationHandler.cs
*/

using System;
using System.Collections;
using System.Configuration;
using System.Reflection;
using System.Xml;

[assembly:AssemblyCompany("TDSL")]
[assembly:AssemblyCopyright("(c) 2003 TDSL. All rights reserved.")]
[assembly:AssemblyDescription("體育資訊 -> Read ini file key section value")]
[assembly:AssemblyProduct("Sports.NET")]
[assembly:AssemblyTitle("Sports.NET DLL")]
[assembly:AssemblyVersion("1.0.*")]
namespace SportsItemHandler {
	public class INIFileInformationHandler:IConfigurationSectionHandler {
		public Object Create(Object parent, Object configContext, System.Xml.XmlNode section) {
			String _type, _section;
			XmlNodeList sectionNodeList, keyNodeList;
			ArrayList iniList = new ArrayList();
			sectionNodeList = section.SelectNodes("ini");

			foreach(XmlNode iniNode in sectionNodeList) {
				_type = iniNode.Attributes.GetNamedItem("type").Value;
				_section = iniNode.Attributes.GetNamedItem("section").Value;
				ArrayList keys = new ArrayList();
				keyNodeList = iniNode.SelectNodes("key");
				if (!(keyNodeList == null)) {
					foreach(XmlNode keyNode in keyNodeList) {
						keys.Add(keyNode.InnerText);
					}
				}

				iniList.Add(new iniEntity(_type, _section, keys));
			}

			return iniList;
		}
	}

	public class iniEntity {
		private String sType, sSection;
		private ArrayList ALKeys;

		public iniEntity(String _inType, String _inSection, ArrayList _inKeyList) {
			sType = _inType;
			sSection = _inSection;
			ALKeys = _inKeyList;
		}

		public String TYPE {
			get {
				return sType;
			}
		}

		public String SECTION {
			get {
				return sSection;
			}
		}

		public ArrayList KEYS {
			get {
				return ALKeys;
			}
		}
	}
}