<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">
	void Page_Load(Object sender,EventArgs e) {
		DelLeague DelForm = new DelLeague((string)Application["SoccerDBConnectionString"]);

		try {
			LeagueInformation.InnerHtml = DelForm.GetLeague();
		}	catch(NullReferenceException) {
			FormsAuthentication.SignOut();
			UpdateReturnMessage("<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>");
		}
	}

	void DeleteLeagueAction(Object sender,EventArgs e) {
		int iUpdated = 0;
		string sNow;
		DelLeague DelLeag = new DelLeague((string)Application["SoccerDBConnectionString"]);

		sNow = DateTime.Now.ToString("HH:mm:ss");
		try {
			iUpdated = DelLeag.Delete();
			Page_Load(sender,e);
			if(iUpdated > 0) {
				UpdateReturnMessage("刪除成功(" + sNow + ")");
			}	else if(iUpdated == 0) {
				UpdateReturnMessage("無效刪除(" + sNow + ")");
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
	<form id="DeleteLeagueForm" method="post" runat="server">
	<b><font color="#FF0000">刪除聯賽</font></b> <asp:Label id="rtnMsg" runat="server" />
		<table border="1" width="50%">
			<tr align="center">
				<th colspan="2" style="background-color:#6FAFB1">
					<span id="LeagueInformation" runat="server" />
				</th>
			</tr>

			<tr align="right">
				<td align="center" style="background-color:#F5FFFA">
					<a href="LeagueManagerDeleteFrame.htm" target="content_frame">其他聯賽</a>
				</td>
				<td>
					<input type="submit" id="DelBtn" value="刪除" OnServerClick="DeleteLeagueAction" runat="server">
					&nbsp;<input type="reset" value="重設">
				</td>
			</tr>
		</table>
	</form>
</center>
</body>
</html>