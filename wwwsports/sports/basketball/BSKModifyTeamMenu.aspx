<%@ Page EnableViewState="false" codepage="950"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">
	void Page_Load(Object sender,EventArgs e) {
		BSKModifyTeamMenu team = new BSKModifyTeamMenu((string)Application["BasketballDBConnectionString"]);
		try {
			TeamInformation.InnerHtml = team.GetTeams();
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
function TeamToModify(teamSelected) {
	if(teamSelected != '' && teamSelected != '0') {
		parent.content_frame.location.replace('BSKModifyTeam.aspx?teamID=' + teamSelected);
	}
}
</script>

<html>
<head>
	<META http-equiv="Content-Type" content="text/html; charset=big5">
	<LINK REL="stylesheet" HREF="/sportStyle.css" TYPE="text/css">
</head>
<body>
	<center>
	<form id="BSKModifyTeamMenuForm" method="post" runat="server">
		<table border="1" width="50%">
			<tr align="center">
				<th>
					請從下拉式選單選擇隊伍修改<br>
					<font size=2>如你的Internet Explorer 6已安裝Service Pack 1, 便可以輸入相關字詞搜尋</font>
				</th>
			</tr>
			<tr align="center">
				<td>
					<span id="TeamInformation" runat="server" />
				</td>
			</tr>
		</table>
		<asp:Label id="rtnMsg" runat="server" />
	</form>
	</center>
</body>
</html>