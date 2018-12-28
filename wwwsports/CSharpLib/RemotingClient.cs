/*
Objective:
Send remote message to invoke Message Dispatcher through ASP.NET page

Last updated:
12 Jan 2004 by Chapman

C#.NET complier statement:
csc /debug:full /t:library /out:../bin/debug/RemotingClient.dll /r:RemoteMessageObject.dll RemotingClient.cs
*/

using System;
using System.Reflection;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;

[assembly:AssemblyCompany("TDSL")]
[assembly:AssemblyCopyright("(c) 2003 TDSL. All rights reserved.")]
[assembly:AssemblyDescription("Remoting Client. Created on 7 July 2003.")]
[assembly:AssemblyProduct(".NET Remoting")]
[assembly:AssemblyTitle(".NET Remoting DLL")]
[assembly:AssemblyVersion("1.0.*")]
namespace TDL.Message {
	public class RemotingClient {
		string m_URL;
		TcpChannel clientChan;
		MessageDispatch.remote.RemoteMessageObject msgObj;

    public RemotingClient() {
	    clientChan = new TcpChannel();
	    if(ChannelServices.GetChannel(clientChan.ChannelName) == null) {
	    	ChannelServices.RegisterChannel(clientChan);
    	} else {
	    	if(!ChannelServices.GetChannel(clientChan.ChannelName).ChannelName.Equals("tcp")) {
		    	ChannelServices.RegisterChannel(clientChan);
	    	}
    	}
    }

    public string RemoteURL {
	    get {
		    return m_URL;
	    }
	    set {
		    m_URL = value;
	    }
    }

    private void GetObject() {
	    msgObj = (MessageDispatch.remote.RemoteMessageObject) Activator.GetObject(typeof(MessageDispatch.remote.RemoteMessageObject), m_URL);
    }

		public bool Send(object MessageObj) {
			if(msgObj == null) GetObject();
			if(!msgObj.Equals(null)) {
				msgObj.SendMessage(MessageObj);
				return true;
			} else {
				return false;
			}
		}
	}
}