<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">
	void Page_Load(Object sender,EventArgs e) {
		DeleteGoGo1Chart chart = new DeleteGoGo1Chart((string)Application["SoccerDBConnectionString"]);

		try {
			ChartDataInformation.InnerHtml = chart.ConfirmDelete();
			UpdateHistoryMessage((string)Application["retrieveInfoMsg"] + "確定刪除賽事圖表(" + DateTime.Now.ToString("HH:mm:ss") + ")");
		} catch(NullReferenceException) {
			FormsAuthentication.SignOut();
			UpdateHistoryMessage("<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>");
		}
	}

	void OnDelete(Object sender,EventArgs e) {
		int iMatchCount = 0;
		DeleteGoGo1Chart chart = new DeleteGoGo1Chart((string)Application["SoccerDBConnectionString"]);

		try {
			iMatchCount = chart.Delete();
			Response.Redirect("Gogo1ChartData.aspx?matchcount=" + iMatchCount.ToString());
		} catch(NullReferenceException nullex) {
			FormsAuthentication.SignOut();
			UpdateHistoryMessage("<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>");
		}
	}

	void UpdateHistoryMessage(string sMsg) {
		historyMsg.Text = sMsg;
	}
</script>

<script language="JavaScript">
function redirect(url) {
	parent.content_frame.location.replace(url);
}
</script>

<html>
<head>
	<META http-equiv="Content-Type" content="text/html; charset=big5">
	<LINK REL="stylesheet" HREF="/sportStyle.css" TYPE="text/css">
</head>
<body>
	<form id="ChartDataForm" method="post" runat="server">
		<font size="2"><b>上次行動:</b><asp:Label id="historyMsg" runat="server" /></font><br>

		<table border="1" width="45%" style="font: 14pt verdana">
			<tr style="background-color:#F0F8FF; color:#FF7F50">
				<th>圖表數據將不能還原，確定刪除此場賽事圖表？</th>
			</tr>

			<span id="ChartDataInformation" runat="server" />

			<tr>
				<td align="right">
					<input type="submit" id="DelBtn" value="確定" OnServerClick="OnDelete" runat="server">&nbsp;
					<input type="button" id="BackBtn" value="返回" onClick="redirect('Gogo1ChartData.aspx?matchcount=' + ChartDataForm.MatchCount.value)">
				</td>
			</tr>
		</table>
	</form>
</body>
</html>