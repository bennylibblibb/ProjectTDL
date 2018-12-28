<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">
	void Page_Load(Object sender,EventArgs e) {
		ModifyTeamMenu root = new ModifyTeamMenu((string)Application["SoccerDBConnectionString"]);
		try {
			TeamRootMenuInformation.InnerHtml = root.GetIndex();
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
function GetTeam() {
	teamWinFeature="Resizable=Yes,ScrollBars=Yes,MenuBar=No,Directories=No,ToolBar=No,Location=No,Status=No, width=600, height=400";
	teamWindow = window.open('TeamManager_Search.aspx?action=' + ModifyTeamRootMenuForm.action.value, 'TeamList' , teamWinFeature);
	teamWindow.focus();
}
</script>

<html>
<head>
	<META http-equiv="Content-Type" content="text/html; charset=big5">
	<LINK REL="stylesheet" HREF="/sportStyle.css" TYPE="text/css">
</head>
<body style="background-color:#FFA07A">
	<form id="ModifyTeamRootMenuForm" method="post" runat="server">
		<center>
		<table border="1" width="100%">
			<tr style="background-color:#FFA07A; color=#FFFAF0">
				<th colspan="10" align="left">
					隊伍目錄(亞洲)：<asp:Label id="rtnMsg" runat="server" />
				</th>
				<th colspan="10" align="right">
					<input type="button" name="search" value="搜尋" onClick="GetTeam()">
				</th>
			</tr>
			<span id="TeamRootMenuInformation" runat="server" />
		</table>
		</center>
	</form>
</body>
</html>