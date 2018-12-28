<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">
	void Page_Load(Object sender,EventArgs e) {
		BSKModifyMatch oneMatchInfo = new BSKModifyMatch((string)Application["BasketballDBConnectionString"]);

		try {
			OneMatchInformation.InnerHtml = oneMatchInfo.GetMatches();
			UpdateHistoryMessage((string)Application["retrieveInfoMsg"] + "�ɨƭק�(" + DateTime.Now.ToString("HH:mm:ss") + ")");
		} catch(NullReferenceException) {
			FormsAuthentication.SignOut();
			UpdateHistoryMessage("<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>");
		}
	}

	void BskModifyMatchAction(Object sender,EventArgs e) {
		int iUpdated = 0;
		string sNow;
		BSKModifyMatch oneMatch = new BSKModifyMatch((string)Application["BasketballDBConnectionString"]);

		sNow = DateTime.Now.ToString("HH:mm:ss");
		try {
			iUpdated = oneMatch.Modify();
			Response.Redirect("BskAllMatch.aspx");
			if(iUpdated > 0) {
				UpdateHistoryMessage("���\�ק��ɨƸ�T(" + sNow + ")");
			}	else if(iUpdated == 0) {
				UpdateHistoryMessage("�S���ɨƳQ�ק�(" + sNow + ")");
			}	else if(iUpdated == -2) {
				UpdateHistoryMessage("�ɨƤw�s�b�A����ק�(" + sNow + ")");
			}	else {
				UpdateHistoryMessage((string)Application["transErrorMsg"] + "(" + sNow + ")");
			}
		}	catch(NullReferenceException nullex) {
			FormsAuthentication.SignOut();
			UpdateHistoryMessage("<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>");
		}
	}

	void BackToMatchAction(Object sender,EventArgs e) {
		Response.Redirect("BskAllMatch.aspx");
	}

	void UpdateHistoryMessage(string sMsg) {
		historyMsg.Text = sMsg;
	}
</script>

<script language="JavaScript">
function  CheckValue() {
	if(MatchModifyForm.Host.value==MatchModifyForm.Guest.value) {
		alert('�D�ȶ�����P�W!');
		return false;
	}
}
</script>

<html>
<head>
	<META http-equiv="Content-Type" content="text/html; charset=big5">
	<LINK REL="stylesheet" HREF="/sportStyle.css" TYPE="text/css">
</head>
<body>
	<form id="MatchModifyForm" method="post" runat="server" ONSUBMIT="return CheckValue()">
		<b>�ɨƭק�</b>&nbsp;
		<font size="2"><b>�W�����:</b><asp:Label id="historyMsg" runat="server" /></font><br>

		<table border="1" width="60%" style="font: 10pt verdana">
		<tr style="background-color:#aaC047" align="center">
			<th>����ʧ@</th>
			<th>���(�~/��/��)</th>
			<th>�ɶ�(��:��)</th>
			<th>�p��</th>
			<th>�D��</th>
			<th>�ȶ�</th>
		</tr>
		<tr align="center">
			<td>
				<select name="action">
					<option value="U">�ק�
					<option value="D">�R��
				</select>
			</td>
			<span id="OneMatchInformation" runat="server" />
		</tr>
		<tr><td colspan="6" align="right">
			<font color="red">(�����ק沈�ݵo�e��ǩI��)</font>
			<!--
				Value of SendToPager is Device ID defined in DEVICE_TYPE
			-->
			<input type="hidden" name="SendToPager" value="1">
			<input type="hidden" name="SendToPager" value="1">
			<input type="hidden" name="SendToPager" value="3">
			<!--
			<input type="hidden" name="SendToPager" value="4">
			<input type="hidden" name="SendToPager" value="5">
			-->
			<input type="submit" id="SendBtn" value="�ǰe" OnServerClick="BskModifyMatchAction" runat="server">&nbsp;
			<input type="submit" id="BackBtn" value="��^" OnServerClick="BackToMatchAction" runat="server">
		</td></tr>

	</form>
</body>
</html>