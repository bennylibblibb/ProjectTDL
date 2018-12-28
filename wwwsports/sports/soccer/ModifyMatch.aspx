<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">
	void Page_Load(Object sender,EventArgs e) {
		ModifyMatch oneMatchInfo = new ModifyMatch((string)Application["SoccerDBConnectionString"]);

		try {
			OneMatchInformation.InnerHtml = oneMatchInfo.GetMatches();
			UpdateHistoryMessage((string)Application["retrieveInfoMsg"] + "�ɨƭק�(" + DateTime.Now.ToString("HH:mm:ss") + ")");
		} catch(NullReferenceException) {
			FormsAuthentication.SignOut();
			UpdateHistoryMessage("<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>");
		}
	}

	void ModifyMatchAction(Object sender,EventArgs e) {
		int iUpdated = 0;
		string sNow;
		ModifyMatch oneMatch = new ModifyMatch((string)Application["SoccerDBConnectionString"]);

		sNow = DateTime.Now.ToString("HH:mm:ss");
		try {
			iUpdated = oneMatch.Modify();
			Response.Redirect("AllMatchesRetrieval.aspx");
			if(iUpdated > 0) {
				UpdateHistoryMessage("���\�ק��ɨƸ�T(" + sNow + ")");
			}	else if(iUpdated == 0) {
				UpdateHistoryMessage("�S���ɨƳQ�ק�(" + sNow + ")");
			}	else {
				UpdateHistoryMessage((string)Application["transErrorMsg"] + "(" + sNow + ")");
			}
		}	catch(NullReferenceException nullex) {
			FormsAuthentication.SignOut();
			UpdateHistoryMessage("<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>");
		}
	}

	void BackToMatchAction(Object sender,EventArgs e) {
		Response.Redirect("AllMatchesRetrieval.aspx");
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
	<form id="MatchModifyForm" method="post" runat="server">
		<b>�ɨƭק�</b>&nbsp;
		<font size="2"><b>�W�����:</b><asp:Label id="historyMsg" runat="server" /></font><br>

		<table border="1" width="60%" style="font: 10pt verdana">
		<tr style="background-color:#FFC0CB" align="center">
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
		<tr>
			<td colspan="6" align="right">
				<!--
					Value of SendToPager is Device ID defined in DEVICE_TYPE
				-->
				<input type="checkbox" name="SendToPager" value="1" checked style="visibility: hidden;">
				<input type="checkbox" name="SendToPager" value="2" checked style="visibility: hidden;">
				<input type="checkbox" name="SendToPager" value="3" checked style="visibility: hidden;">
				<input type="checkbox" name="SendToPager" value="4" checked style="visibility: hidden;">
				<input type="checkbox" name="SendToPager" value="5" checked style="visibility: hidden;">
				<font color="red">(�����ק沈�ݵo�e��ǩI��)</font>
				<input type="submit" id="SendBtn" value="�ǰe" OnServerClick="ModifyMatchAction" runat="server">&nbsp;
				<input type="submit" id="BackBtn" value="��^" OnServerClick="BackToMatchAction" runat="server">
			</td>
		</tr>
		<tr>
			<td colspan="6" align="left">
			<b>�Ъ`�N���v�T����T:<br>
			�ק��ɨ�:�ɨơA�w�����ơA�{����ơA��ƸԱ��A�i�x�A���R�ɨơA���R�ƾڡA���R���Z�β{���߲v<br>
			�R���ɨ�:�ɨơA�w�����ơA�{����ơA��ƸԱ��A�i�x�A���R�ɨơA���R�ƾڡA���R���Z�A<font color="red">���]�A�{���߲v</font>
			</td>
		</tr>
		</table>
	</form>
</body>
</html>