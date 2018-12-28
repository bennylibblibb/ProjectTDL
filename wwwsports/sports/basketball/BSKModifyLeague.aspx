<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">
	void Page_Load(Object sender,EventArgs e) {
		BSKModifyLeague leag = new BSKModifyLeague((string)Application["BasketballDBConnectionString"]);
		try {
			LeagueInformation.InnerHtml = leag.GetLeagues();
		} catch(NullReferenceException) {
			FormsAuthentication.SignOut();
			UpdateReturnMessage("<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>");
		}
	}

	void ModifyLeagueAction(Object sender,EventArgs e) {
		int iUpdated = 0;
		string sNow;
		BSKModifyLeague league = new BSKModifyLeague((string)Application["BasketballDBConnectionString"]);
		sNow = DateTime.Now.ToString("HH:mm:ss");
		try {
			iUpdated = league.Modify();
			Page_Load(sender,e);
			if(iUpdated > 0) {
				UpdateReturnMessage("修改成功(" + sNow + ")");
			}	else if(iUpdated == 0) {
				UpdateReturnMessage("無效修改(" + sNow + ")");
			}	else {
				UpdateReturnMessage((string)Application["transErrorMsg"] + "(" + sNow + ")");
			}
		} catch(NullReferenceException nullex) {
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
	<form id="BSKModifyLeagueForm" method="post" runat="server">
		<table border="1" width="50%">
			<tr align="center">
				<th colspan=2>修改聯賽</th>
			</tr>

			<span id="LeagueInformation" runat="server" />

			<tr align="right">
				<td colspan="2">
					<input type="submit" id="modifyBtn" value="修改" OnServerClick="ModifyLeagueAction" runat="server">
					&nbsp;<input type="reset" value="重設">
				</td>
			</tr>
		</table>
		<asp:Label id="rtnMsg" runat="server" />
	</form>
	</center>
</body>
</html>