<%@ Page EnableViewState="false" codepage="950"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">
	void Page_Load(Object sender,EventArgs e) {
		ModifyLeagueMenu sub = new ModifyLeagueMenu((string)Application["SoccerDBConnectionString"]);
		try {
			LeagueSubMenuInformation.InnerHtml = sub.GetLeagueGroup();
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
	<form id="ModifyLeagueSubMenuForm" method="post" runat="server">
	<b><font color="#800000">請選擇聯賽：</font></b><asp:Label id="rtnMsg" runat="server" />
	<center>
		<table border="1" width="100%">
			<tr align="center">
				<td>
					<span id="LeagueSubMenuInformation" runat="server" />
				</td>
			</tr>
		</table>
	</center>
	</form>
</body>
</html>