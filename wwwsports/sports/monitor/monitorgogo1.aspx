<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">
	void Page_Load(Object sender,EventArgs e) {
		SystemMonitorGoGo1 sysmon = new SystemMonitorGoGo1();

		try {
			SystemInformation.InnerHtml = sysmon.CurrentStatus();
			rtnMsg.Text = "�t�Ϊ��p@" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
		} catch(NullReferenceException) {
			FormsAuthentication.SignOut();
			rtnMsg.Text = "<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>";
		}
	}
	
	void ShowStatus (Object sender,EventArgs e) {
		SystemMonitorGoGo1 sysmon = new SystemMonitorGoGo1();
		int iUpdated = 0;

		try {
			iUpdated = sysmon.UpdateShowStatus();
			SystemInformation.InnerHtml = sysmon.CurrentStatus();
			if(iUpdated == 0) {
				rtnMsg.Text = "�t�Ϊ��p@" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
			} else {
				rtnMsg.Text = "<font color=\"red\"><b>��ܨt�Ϊ��p���~�I</b></font>@" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
			}
		} catch(NullReferenceException) {
			FormsAuthentication.SignOut();
			rtnMsg.Text = "<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>";
		}
	}
	
	void ClearDB(Object sender,EventArgs e) {
		SystemMonitorGoGo1 sysmon = new SystemMonitorGoGo1();
		int iUpdated = 0;

		try {
			iUpdated = sysmon.DeletePendingFile();
			SystemInformation.InnerHtml = sysmon.CurrentStatus();
			if(iUpdated == 0) {
				rtnMsg.Text = "<font color=\"red\"><b>����R��</b></font>�I  �t�Ϊ��p@" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
		 	} else if(iUpdated == 1) {
				rtnMsg.Text = "�t�Ϊ��p@" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
	 		} else {
				rtnMsg.Text = "<font color=\"red\"><b>�R���t�Ϊ��p���~�I</b></font>@" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
			}
		} catch(NullReferenceException) {
			FormsAuthentication.SignOut();
			rtnMsg.Text = "<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>";
		}
	}
	
</script>

<script language="JavaScript">
function checkPagers() {
	var question = confirm('�T�w�R���H')
	if (question == true)
		return true;
	else
		return false;
}
</script>

<html>
<title>��|��T - �t�Ϊ��p</title>
<head>
	<META http-equiv="Content-Type" content="text/html; charset=big5">
	<LINK REL="stylesheet" HREF="/sportStyle.css" TYPE="text/css">
</head>
<body>
	<form id="MonitorGoGoForm" method="post" runat="server">
		<center>
		<table border="1" width="100%" style="font: 10pt verdana">
			<tr>
				<td align="left">
					<asp:Label id="rtnMsg" runat="server" />
				</td>
				<td colspan="2" align="right">
					<input type="submit" id="update1" value="��s" OnServerClick="Page_Load" runat="server" />
				</td>
			</tr>

			<span id="SystemInformation" runat="server" />

			<tr align="right">
				<td colspan="3">
					<input type="submit" id="showBtn" value="���" OnServerClick="ShowStatus" runat="server">					
					<input type="submit" id="deleteBtn" value="�R��" OnServerClick="ClearDB"  OnClick="return checkPagers()" runat="server">
					<input type="submit" id="update2" value="��s" OnServerClick="Page_Load" runat="server">
				</td>
			</tr>
		</table>
		</center>
	</form>
</body>
</html>