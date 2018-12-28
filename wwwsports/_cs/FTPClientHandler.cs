/*
Objective:
Read .NET Remoting info such as Address, Port from web.config <FTPClient>

Last updated:
22 Sept 2004 by Rita

C#.NET complier statement:
csc /t:library /out:..\bin\FTPClientHandler.dll FTPClientHandler.cs
*/

using System;
using System.Collections;
using System.Configuration;
using System.Reflection;
using System.Xml;

[assembly:AssemblyCompany("TDSL")]
[assembly:AssemblyCopyright("(c) 2004 TDSL. All rights reserved.")]
[assembly:AssemblyDescription("體育資訊 -> Read .NET FTPClient config")]
[assembly:AssemblyProduct("Sports.NET")]
[assembly:AssemblyTitle("Sports.NET DLL")]
[assembly:AssemblyVersion("1.0.*")]

namespace SportsItemHandler {
	public class FTPClientHandler:IConfigurationSectionHandler {
		public Object Create(Object parent, Object configContext, System.Xml.XmlNode section) {
			string _Name,  _Address, _Port, _Mode;
			XmlNodeList FTPNodeList;
			ArrayList FTPList = new ArrayList();
			FTPNodeList = section.SelectNodes("FTP");

			foreach(XmlNode FTPNode in FTPNodeList) {
				_Name = FTPNode.Attributes.GetNamedItem("name").Value;
				_Address = FTPNode.Attributes.GetNamedItem("address").Value;
				_Port = FTPNode.Attributes.GetNamedItem("port").Value;
				_Mode = FTPNode.Attributes.GetNamedItem("mode").Value;

				FTPList.Add(new FTPEntity(_Name, _Address, _Port, _Mode));
			}

			return FTPList;
		}
	}

	public class FTPEntity {
		private string sName, sAddress, sPort, sMode;

		public FTPEntity(string _inName, string _inAddress, string _inPort, string _inMode) {
			sName = _inName;
			sAddress = _inAddress;
			sPort = _inPort;
			sMode = _inMode;
		}
		
		public string Name {
			get {
				return sName;
			}
		}
		
		public string Address {
			get {
				return sAddress;
			}
		}

		public string Port {
			get {
				return sPort;
			}
		}
		
		public string Mode {
			get {
				return sMode;
			}
		}
	}
}