/*
Objective:
Read .NET Remoting info such as name, uri from web.config <BroadcastChannel>

Last updated:
11 Feb 2004 by Chapman

C#.NET complier statement:
csc /t:library /out:..\bin\BroadcastChannelHandler.dll BroadcastChannelHandler.cs
*/

using System;
using System.Collections;
using System.Configuration;
using System.Reflection;
using System.Xml;

[assembly:AssemblyCompany("TDSL")]
[assembly:AssemblyCopyright("(c) 2004 TDSL. All rights reserved.")]
[assembly:AssemblyDescription("體育資訊 -> Read .NET BroadcastChannel config")]
[assembly:AssemblyProduct("Sports.NET")]
[assembly:AssemblyTitle("Sports.NET DLL")]
[assembly:AssemblyVersion("1.0.*")]
namespace SportsItemHandler {
	public class BroadcastChannelHandler:IConfigurationSectionHandler {
		public Object Create(Object parent, Object configContext, System.Xml.XmlNode section) {
			string _name, _uri;
			XmlNodeList channelNodeList;
			ArrayList channelList = new ArrayList();
			channelNodeList = section.SelectNodes("channel");

			foreach(XmlNode channelNode in channelNodeList) {
				_name = channelNode.Attributes.GetNamedItem("name").Value;
				_uri = channelNode.Attributes.GetNamedItem("uri").Value;

				channelList.Add(new channelEntity(_name, _uri));
			}

			return channelList;
		}
	}

	public class channelEntity {
		private string sName, sUri;

		public channelEntity(string _inName, string _inUri) {
			sName = _inName;
			sUri = _inUri;
		}

		public string NAME {
			get {
				return sName;
			}
		}

		public string URI {
			get {
				return sUri;
			}
		}
	}
}