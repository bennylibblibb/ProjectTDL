<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">
	void Page_Load(Object sender,EventArgs e) {
		OtherSoccerHiddenMatch match = new OtherSoccerHiddenMatch((string)Application["SoccerDBConnectionString"]);
		try {
			MatchInformation.InnerHtml = match.Show();
			UpdateHistoryMessage((string)Application["retrieveInfoMsg"] + "�w���ê��ɨ�(" + DateTime.Now.ToString("HH:mm:ss") + ")");
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
	<title>�w���ê��ɨ�</title>
</head>
<body>
	<form id="HiddenMatchForm" method="post" runat="server">
		<font size="2"><b>�W�����:</b><asp:Label id="historyMsg" runat="server" /></font><br>

		<table border="1" width="100%" style="font: 10pt verdana">
			<tr align="center" style="background-color:#C4FFC4; color:#191970">
				<th>�p��</th>
				<th>�D��</th>
				<th>�ȶ�</th>
			</tr>

			<span id="MatchInformation" runat="server" />
		</table>
	</form>
</body>
</html>