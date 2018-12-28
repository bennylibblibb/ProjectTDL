<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">
	void Page_Load(Object sender,EventArgs e) {
		OtherSoccerHiddenMatch match = new OtherSoccerHiddenMatch((string)Application["SoccerDBConnectionString"]);
		try {
			MatchInformation.InnerHtml = match.Show();
			UpdateHistoryMessage((string)Application["retrieveInfoMsg"] + "已隱藏的賽事(" + DateTime.Now.ToString("HH:mm:ss") + ")");
		} catch(NullReferenceException) {
			FormsAuthentication.SignOut();
			UpdateHistoryMessage("<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>");
		}
	}

	void UpdateHistoryMessage(string sMsg) {
		historyMsg.Text = sMsg;
	}
</script>

<html>
<head>
	<META http-equiv="Content-Type" content="text/html; charset=big5">
	<LINK REL="stylesheet" HREF="/sportStyle.css" TYPE="text/css">
	<title>已隱藏的賽事</title>
</head>
<body>
	<form id="HiddenMatchForm" method="post" runat="server">
		<font size="2"><b>上次行動:</b><asp:Label id="historyMsg" runat="server" /></font><br>

		<table border="1" width="100%" style="font: 10pt verdana">
			<tr align="center" style="background-color:#C4FFC4; color:#191970">
				<th>聯賽</th>
				<th>主隊</th>
				<th>客隊</th>
			</tr>

			<span id="MatchInformation" runat="server" />
		</table>
	</form>
</body>
</html>