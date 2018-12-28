<%@ Page EnableViewState="false" codepage="950"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">
	void Page_Load(Object sender,EventArgs e) {
		ModifyLeagueMenu root = new ModifyLeagueMenu((string)Application["SoccerDBConnectionString"]);
		try {
			LeagueRootMenuInformation.InnerHtml = root.GetIndex();
		} catch(NullReferenceException) {
			FormsAuthentication.SignOut();
			UpdateReturnMessage("<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>");
		}
	}

	void UpdateReturnMessage(string sMsg) {
		rtnMsg.Text = sMsg;
	}
</script>

<script language="JavaScript">
function GetLeague() {
	leagWinFeature="Resizable=Yes,ScrollBars=Yes,MenuBar=No,Directories=No,ToolBar=No,Location=No,Status=No, width=600, height=400";
	leagWindow = window.open('LeagueManager_Search.aspx?action=' + ModifyLeagueRootMenuForm.action.value, 'LeagueList' , leagWinFeature);
	leagWindow.focus();
}
</script>

<html>
<head>
	<META http-equiv="Content-Type" content="text/html; charset=big5">
	<LINK REL="stylesheet" HREF="/sportStyle.css" TYPE="text/css">
</head>
<body style="background-color:#6FAFB1">
	<form id="ModifyLeagueRootMenuForm" method="post" runat="server">
		<center>
		<table border="1" width="100%">
			<tr style="background-color:#6FAFB1; color=#F0F8FF">
				<th colspan="10" align="left">
					聯賽目錄(亞洲)：<asp:Label id="rtnMsg" runat="server" />
				</th>
				<th colspan="10" align="right">
					<input type="button" name="search" value="搜尋" onClick="GetLeague()">
				</th>
			</tr>
			<span id="LeagueRootMenuInformation" runat="server" />
		</table>
		</center>
	</form>
</body>
</html>