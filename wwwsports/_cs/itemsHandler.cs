/*
Objective:
Read various application config from web.config <item>

Last updated:
19 June 2003 by Chapman

C#.NET complier statement:
csc /t:library /out:..\bin\itemsHandler.dll itemsHandler.cs
*/

using System;
using System.Collections;
using System.Configuration;
using System.Reflection;
using System.Xml;

[assembly:AssemblyCompany("TDSL")]
[assembly:AssemblyCopyright("(c) 2003 TDSL. All rights reserved.")]
[assembly:AssemblyDescription("體育資訊 -> Read application config from <item> tag")]
[assembly:AssemblyProduct("Sports.NET")]
[assembly:AssemblyTitle("Sports.NET DLL")]
[assembly:AssemblyVersion("1.0.*")]
namespace SportsItemHandler {
	public class itemsHandler:IConfigurationSectionHandler {
		public Object Create(Object parent, Object configContext, System.Xml.XmlNode section) {
			XmlNodeList itemsNodeList;
			ArrayList itemsList = new ArrayList();
			itemsNodeList = section.SelectNodes("item");

			foreach(XmlNode elementNode in itemsNodeList) {
				itemsList.Add(elementNode.InnerText);
			}

			return itemsList;
		}
	}
}