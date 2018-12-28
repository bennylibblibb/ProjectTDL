/*
Objective:
Send message to invoke Message Dispatcher through ASP.NET page, depend on Message Type

Last updated:
12 Jan 2004 by Chapman

C#.NET complier statement:
csc /debug:full /t:library /out:../bin/debug/MessageClient.dll /r:../bin/debug/RemotingClient.dll;../bin/debug/MessageGenerator.dll MessageClient.cs
*/

using System;
using System.Reflection;

[assembly:AssemblyCompany("TDSL")]
[assembly:AssemblyCopyright("(c) 2003 TDSL. All rights reserved.")]
[assembly:AssemblyDescription("Message Client. Created on 8 July 2003.")]
[assembly:AssemblyProduct(".NET Message")]
[assembly:AssemblyTitle(".NET Message DLL")]
[assembly:AssemblyVersion("1.0.*")]
namespace TDL.Message {
	public class MessageClient {
		string m_MessageType;
		string m_MessagePath;
		RemotingClient rmClt;
		MessageGenerator mqClt;

		public MessageClient() {}

		public string MessageType {
			get {
				return m_MessageType;
			}
			set {
				m_MessageType = value;
			}
		}

		public string MessagePath {
			get {
				return m_MessagePath;
			}
			set {
				m_MessagePath = value;
			}
		}

		public bool SendMessage(object MessageObj) {
			try {
				if(m_MessageType.Equals("Remoting")) {	//Remoting
					if(rmClt == null) rmClt = new RemotingClient();
					rmClt.RemoteURL = m_MessagePath;
					return rmClt.Send(MessageObj);
				} else if(m_MessageType.Equals("MSMQ")) {	//MSMQ
					if(mqClt == null) mqClt = new MessageGenerator();
					mqClt.MQPath = m_MessagePath;
					return mqClt.Send(MessageObj);
				} else {	//unavailable message type
					return false;
				}
			} catch(Exception ex) {
				throw ex;
			}
		}
	}
}