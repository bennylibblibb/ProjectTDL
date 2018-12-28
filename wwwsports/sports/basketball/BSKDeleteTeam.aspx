<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">
	void Page_Load(Object sender,EventArgs e) {
		BSKDeleteTeam team = new BSKDeleteTeam((string)Application["BasketballDBConnectionString"]);
		try {
			TeamInformation.InnerHtml = team.GetTeam();
		}	catch(NullReferenceException) {
			FormsAuthentication.SignOut();
			UpdateReturnMessage("<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>");
		}
	}

	void DeleteLeagueAction(Object sender,EventArgs e) {
		int iUpdated = 0;
		string sNow;
		BSKDeleteTeam teamDelete = new BSKDeleteTeam((string)Application["BasketballDBConnectionString"]);

		sNow = DateTime.Now.ToString("HH:mm:ss");
		try {
			iUpdated = teamDelete.Delete();
			Page_Load(sender,e);
			if(iUpdated > 0) {
				UpdateReturnMessage("刪除成功(" + sNow + ")");
			}	else if(iUpdated == 0) {
				UpdateReturnMessage("無效刪除(" + sNow + ")");
			}	else {
				UpdateReturnMessage((string)Application["transErrorMsg"] + "(" + sNow + ")");
			}
		}	catch(NullReferenceException nullex) {
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
	<center>
	<form id="BSKDeleteTeamForm" method="post" runat="server">
		<table border="1" width="50%">
			<tr align="center">
				<th colspan="2">
					請從下拉式選單選擇隊伍刪除<br>
					<font size=2>如你的Internet Explorer 6已安裝Service Pack 1, 便可以輸入相關字詞搜尋</font>
				</th>
			</tr>

			<tr>
				<th align="center">選擇隊伍:</th>
				<td>
					<span id="TeamInformation" runat="server" />
				</td>
			</tr>

			<tr align="right">
				<td colspan="2">
					<input type="submit" id="DelBtn" value="刪除" OnServerClick="DeleteLeagueAction" runat="server">
					&nbsp;<input type="reset" value="重設">
				</td>
			</tr>
		</table>
		<asp:Label id="rtnMsg" runat="server" />
	</form>
	</center>
</body>
</html>