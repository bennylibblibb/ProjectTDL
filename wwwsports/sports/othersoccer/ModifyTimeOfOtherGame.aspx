<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">
	void Page_Load(Object sender,EventArgs e) {
		ModifyTimeOfOtherGame game = new ModifyTimeOfOtherGame((string)Application["SoccerDBConnectionString"]);

		try {
			GameInformation.InnerHtml = game.GetMatch();
			UpdateHistoryMessage((string)Application["retrieveInfoMsg"] + "�ɨƮɶ�(" + DateTime.Now.ToString("HH:mm:ss") + ")");
		} catch(NullReferenceException) {
			FormsAuthentication.SignOut();
			UpdateHistoryMessage("<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>");
		}
	}

	void UpdateTime(Object sender,EventArgs e) {
		int iUpdated = 0;
		string sNow;
		ModifyTimeOfOtherGame match = new ModifyTimeOfOtherGame((string)Application["SoccerDBConnectionString"]);

		sNow = DateTime.Now.ToString("HH:mm:ss");
		try {
			iUpdated = match.UpdateStartTime();
			if(iUpdated > 0) {
				UpdateHistoryMessage("���\��s�ɨƶi��ɶ�(" + sNow + ")");
			}	else if(iUpdated == 0) {
				UpdateHistoryMessage("�S����s�ɨƶi��ɶ�(" + sNow + ")");
			}	else {
				UpdateHistoryMessage((string)Application["transErrorMsg"] + "(" + sNow + ")");
			}
			Response.Redirect("redirectToOtherSoccerLiveGoal.htm");
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
function onTimeOfGameChanged() {
	re = /^\d{0,3}$/
	re_val = TimeOfGameForm.timeofgame.value.search(re)
	if(re_val == -1) {
		alert('�ɶ��u�����Ʀr');
		TimeOfGameForm.timeofgame.value = '';
	}
}
</script>

<html>
<head>
	<META http-equiv="Content-Type" content="text/html; charset=big5">
	<LINK REL="stylesheet" HREF="/sportStyle.css" TYPE="text/css">
</head>
<body>
	<form id="TimeOfGameForm" method="post" runat="server">
		<font size="2"><b>�W�����:</b><asp:Label id="historyMsg" runat="server" /></font><br>

		<table border="1" width="60%" style="font: 10pt verdana">
			<tr style="background-color:#00bfff">
				<th>���</th>
				<th>�ɶ�</th>
				<th>�ɬq</th>
				<th>�p��</th>
				<th>�D��</th>
				<th>�D�����</th>
				<th>�ȶ�</th>
				<th>�ȶ����</th>
				<th>�i��ɶ�</th>
			</tr>

			<span id="GameInformation" runat="server" />

			<tr>
				<td colspan="9" align="right">
					<!--
						Value of SendToPager is Device ID defined in DEVICE_TYPE
					-->
					<input type="hidden" name="SendToPager" value="2">
					<input type="hidden" name="SendToPager" value="3">
					<input type="hidden" name="SendToPager" value="4">
					<input type="hidden" name="SendToPager" value="5">
					<input type="submit" id="SendBtn" value="�ǰe" OnServerClick="UpdateTime" runat="server">
				</td>
			</tr>
		</table>
	</form>
</body>
</html>