<%@ Page EnableViewState="false" codepage="950"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">
	void Page_Load(Object sender,EventArgs e) {
		ModifyTeamMenu sub = new ModifyTeamMenu((string)Application["SoccerDBConnectionString"]);
		try {
			TeamSubMenuInformation.InnerHtml = sub.GetTeamGroup();
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
	<form id="ModifyTeamSubMenuForm" method="post" runat="server">
	<b><font color="#B22222">請選擇隊伍：</font></b><asp:Label id="rtnMsg" runat="server" />
	<center>
		<table border="1" width="100%">
			<tr align="center">
				<td>
					<span id="TeamSubMenuInformation" runat="server" />
				</td>
			</tr>
		</table>
	</center>
	</form>
</body>
</html>