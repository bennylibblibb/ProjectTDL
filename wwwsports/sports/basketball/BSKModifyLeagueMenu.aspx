<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">
	void Page_Load(Object sender,EventArgs e) {
		BSKModifyLeagueMenu menu = new BSKModifyLeagueMenu((string)Application["BasketballDBConnectionString"]);
		try {
			LeagueMenuInformation.InnerHtml = menu.GetLeague();
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
function LeagueToModify(leagueSelected) {
	if(leagueSelected != '' && leagueSelected != '0') {
		parent.content_frame.location.replace('BSKModifyLeague.aspx?leagID=' + leagueSelected);
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
	<form id="BSKModifyLeagueMenuForm" method="post" runat="server">
		<table border="1" width="50%">
			<tr align="center">
				<th>
					請從下拉式選單選擇聯賽修改<br>
					<font size=2>如你的Internet Explorer 6已安裝Service Pack 1, 便可以輸入相關字詞搜尋</font>
				</th>
			</tr>

			<tr align="center">
				<td>
					<span id="LeagueMenuInformation" runat="server" />
				</td>
			</tr>
		</table>
		<asp:Label id="rtnMsg" runat="server" />
	</form>
	</center>
</body>
</html>