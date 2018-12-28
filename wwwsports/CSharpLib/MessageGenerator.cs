/*
Objective:
Generate and send message to MSMQ server

Last updated:
26 August 2003 by Chapman

C#.NET complier statement:
csc /debug:full /t:library /out:../bin/debug/MessageGenerator.dll MessageGenerator.cs
*/

using System;
using System.Messaging;
using System.Reflection;

[assembly:AssemblyCompany("TDSL")]
[assembly:AssemblyCopyright("(c) 2003 TDSL. All rights reserved.")]
[assembly:AssemblyDescription("Message generator")]
[assembly:AssemblyProduct(".NET message")]
[assembly:AssemblyTitle(".NET message DLL")]
[assembly:AssemblyVersion("1.0.*")]
namespace TDL.Message {
	public class MessageGenerator {
		string m_MQPath;
		MessageQueue m_MQ;

		public MessageGenerator() {
			m_MQ = new MessageQueue();
			//m_MQ.Formatter = new BinaryMessageFormatter();
		}

		public string MQPath {
			set {
				m_MQPath = value;
			}
			get {
				return m_MQPath;
			}
		}

		public bool Send(object MessageObj) {
			m_MQ.Path = m_MQPath;
			m_MQ.Send(MessageObj);
			m_MQ.Close();
			return true;
		}
	}
}