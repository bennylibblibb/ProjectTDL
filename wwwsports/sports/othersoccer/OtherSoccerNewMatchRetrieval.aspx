<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">
	void Page_Load(Object sender,EventArgs e) {
		OtherSoccerNewMatch aMatch = new OtherSoccerNewMatch((string)Application["SoccerDBConnectionString"]);

		try {
			NewMatchInformation.InnerHtml = aMatch.InitFields();
			UpdateHistoryMessage((string)Application["retrieveInfoMsg"] + "賽事表格(" + DateTime.Now.ToString("HH:mm:ss") + ")");
		} catch(NullReferenceException) {
			FormsAuthentication.SignOut();
			UpdateHistoryMessage("<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>");
		}
	}

	void onAddMatch(Object sender,EventArgs e) {
		int iUpdated = 0;
		string sNow;
		OtherSoccerNewMatch addMatch = new OtherSoccerNewMatch((string)Application["SoccerDBConnectionString"]);

		sNow = DateTime.Now.ToString("HH:mm:ss");
		try {
			iUpdated = addMatch.Add();
			if(iUpdated > 0) {
				UpdateHistoryMessage("成功新增" + iUpdated.ToString() + "場賽事(" + sNow + ")");
			}	else if(iUpdated == -99) {
				UpdateHistoryMessage("新增失敗，相同賽事已存在！");
			} else {
				UpdateHistoryMessage("沒有新增賽事(" + sNow + ")");
			}
		} catch(NullReferenceException nullex) {
			FormsAuthentication.SignOut();
			UpdateHistoryMessage("<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>");
		}
	}

	void UpdateHistoryMessage(string sMsg) {
		historyMsg.Text = sMsg;
	}
</script>

<html>
<head>
	<META http-equiv="Content-Type" content="text/html; charset=big5">
	<LINK REL="stylesheet" HREF="/sportStyle.css" TYPE="text/css">
</head>
<body>
	<form id="NewMatchForm" method="post" runat="server">
		<font size="2"><b>上次行動:</b><asp:Label id="historyMsg" runat="server" /></font><br>

		<table border="1" width="60%" style="font: 10pt verdana">
		<span id="NewMatchInformation" runat="server" />
		<tr>
			<td colspan="6" align="right">
				<input type="submit" id="SaveBtn" value="儲存" OnServerClick="onAddMatch" runat="server">&nbsp;
				<input type="reset" value="重設">
			</td>
		</table>
	</form>
</body>
</html>