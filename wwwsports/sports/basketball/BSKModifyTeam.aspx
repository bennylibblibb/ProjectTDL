<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">
	void Page_Load(Object sender,EventArgs e) {
		BSKModifyTeam team = new BSKModifyTeam((string)Application["BasketballDBConnectionString"]);

		try {
			TeamInformation.InnerHtml = team.GetTeam();
		}	catch(NullReferenceException) {
			FormsAuthentication.SignOut();
			UpdateReturnMessage("<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>");
		}
	}

	void ModifyTeamAction(Object sender,EventArgs e) {
		int iUpdated = 0;
		string sNow;
		BSKModifyTeam teamModify = new BSKModifyTeam((string)Application["BasketballDBConnectionString"]);
		sNow = DateTime.Now.ToString("HH:mm:ss");
		try {
			iUpdated = teamModify.Modify();
			Page_Load(sender,e);
			if(iUpdated > 0) {
				UpdateReturnMessage("修改成功(" + sNow + ")");
			}	else if(iUpdated == 0) {
				UpdateReturnMessage("無效修改(" + sNow + ")");
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
	<form id="BSKModifyTeamForm" method="post" runat="server">
		<table border="1" width="80%">
			<span id="TeamInformation" runat="server" />

			<tr align="right">
				<td colspan="2">
					<font size="2">(有<font color="red">*</font>者必須選擇)</font>
					<input type="submit" id="ModifyBtn" value="修改" OnServerClick="ModifyTeamAction" runat="server">
					&nbsp;<input type="reset" value="重設">
				</td>
			</tr>
		</table>
		<asp:Label id="rtnMsg" runat="server" />
	</form>
	</center>
</body>
</html>