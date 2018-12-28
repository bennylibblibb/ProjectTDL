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
				UpdateReturnMessage("�R�����\(" + sNow + ")");
			}	else if(iUpdated == 0) {
				UpdateReturnMessage("�L�ħR��(" + sNow + ")");
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
					�бq�U�Ԧ�����ܶ���R��<br>
					<font size=2>�p�A��Internet Explorer 6�w�w��Service Pack 1, �K�i�H��J�����r���j�M</font>
				</th>
			</tr>

			<tr>
				<th align="center">��ܶ���:</th>
				<td>
					<span id="TeamInformation" runat="server" />
				</td>
			</tr>

			<tr align="right">
				<td colspan="2">
					<input type="submit" id="DelBtn" value="�R��" OnServerClick="DeleteLeagueAction" runat="server">
					&nbsp;<input type="reset" value="���]">
				</td>
			</tr>
		</table>
		<asp:Label id="rtnMsg" runat="server" />
	</form>
	</center>
</body>
</html>