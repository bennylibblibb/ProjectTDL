<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">
	void Page_Load(Object sender,EventArgs e) {
		SystemMonitor sysmon = new SystemMonitor();

		try {
			SystemInformation.InnerHtml = sysmon.CurrentStatus();
			rtnMsg.Text = "系統狀況@" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
		} catch(NullReferenceException) {
			FormsAuthentication.SignOut();
			rtnMsg.Text = "<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>";
		}
	}
</script>

<html>
<title>體育資訊 - 系統狀況</title>
<head>
	<META http-equiv="Content-Type" content="text/html; charset=big5">
	<LINK REL="stylesheet" HREF="/sportStyle.css" TYPE="text/css">
</head>
<body>
	<form method="post" runat="server">
		<center>
		<table border="1" width="100%" style="font: 10pt verdana">
			<tr>
				<td align="left">
					<asp:Label id="rtnMsg" runat="server" />
				</td>
				<td align="right">
					<asp:Button id="update1" Text="更新" OnClick="Page_Load" runat="server" />
				</td>
			</tr>

			<span id="SystemInformation" runat="server" />

			<tr align="right">
				<td colspan="2">
					<asp:Button id="update2" Text="更新" OnClick="Page_Load" runat="server" />
				</td>
			</tr>
		</table>
		</center>
	</form>
</body>
</html>