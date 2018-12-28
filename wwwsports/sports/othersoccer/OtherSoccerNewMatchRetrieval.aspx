<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">
	void Page_Load(Object sender,EventArgs e) {
		OtherSoccerNewMatch aMatch = new OtherSoccerNewMatch((string)Application["SoccerDBConnectionString"]);

		try {
			NewMatchInformation.InnerHtml = aMatch.InitFields();
			UpdateHistoryMessage((string)Application["retrieveInfoMsg"] + "�ɨƪ��(" + DateTime.Now.ToString("HH:mm:ss") + ")");
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
				UpdateHistoryMessage("���\�s�W" + iUpdated.ToString() + "���ɨ�(" + sNow + ")");
			}	else if(iUpdated == -99) {
				UpdateHistoryMessage("�s�W���ѡA�ۦP�ɨƤw�s�b�I");
			} else {
				UpdateHistoryMessage("�S���s�W�ɨ�(" + sNow + ")");
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
		<font size="2"><b>�W�����:</b><asp:Label id="historyMsg" runat="server" /></font><br>

		<table border="1" width="60%" style="font: 10pt verdana">
		<span id="NewMatchInformation" runat="server" />
		<tr>
			<td colspan="6" align="right">
				<input type="submit" id="SaveBtn" value="�x�s" OnServerClick="onAddMatch" runat="server">&nbsp;
				<input type="reset" value="���]">
			</td>
		</table>
	</form>
</body>
</html>