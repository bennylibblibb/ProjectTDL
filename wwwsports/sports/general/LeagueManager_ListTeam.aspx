<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">
	void Page_Load(Object sender,EventArgs e) {
		ModifyLeague ModifyForm = new ModifyLeague((string)Application["SoccerDBConnectionString"]);
		try {
			ListTeamsInformation.InnerHtml = ModifyForm.GetTeams();
		} catch(NullReferenceException) {
			FormsAuthentication.SignOut();
			UpdateReturnMessage("<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>");
		}
	}

	void UpdateReturnMessage(string sMsg) {
		rtnMsg.Text = sMsg;
	}
</script>

<html>
<head>
	<META http-equiv="Content-Type" content="text/html; charset=big5">
	<LINK REL="stylesheet" HREF="/sportStyle.css" TYPE="text/css">
</head>
<body>
	<form id="ModifyLeagueListTeamsForm" method="post" runat="server">
		<b>修改相關隊伍：</b><asp:Label id="rtnMsg" runat="server" /><br>
		<table border="1" width="100%">
			<span id="ListTeamsInformation" runat="server" />
		</table>
	</form>
</body>
</html>