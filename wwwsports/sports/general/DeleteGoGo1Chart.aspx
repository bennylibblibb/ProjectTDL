<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">
	void Page_Load(Object sender,EventArgs e) {
		DeleteGoGo1Chart chart = new DeleteGoGo1Chart((string)Application["SoccerDBConnectionString"]);

		try {
			ChartDataInformation.InnerHtml = chart.ConfirmDelete();
			UpdateHistoryMessage((string)Application["retrieveInfoMsg"] + "�T�w�R���ɨƹϪ�(" + DateTime.Now.ToString("HH:mm:ss") + ")");
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
		<font size="2"><b>�W�����:</b><asp:Label id="historyMsg" runat="server" /></font><br>

		<table border="1" width="45%" style="font: 14pt verdana">
			<tr style="background-color:#F0F8FF; color:#FF7F50">
				<th>�Ϫ�ƾڱN�����٭�A�T�w�R�������ɨƹϪ�H</th>
			</tr>

			<span id="ChartDataInformation" runat="server" />

			<tr>
				<td align="right">
					<input type="submit" id="DelBtn" value="�T�w" OnServerClick="OnDelete" runat="server">&nbsp;
					<input type="button" id="BackBtn" value="��^" onClick="redirect('Gogo1ChartData.aspx?matchcount=' + ChartDataForm.MatchCount.value)">
				</td>
			</tr>
		</table>
	</form>
</body>
</html>