<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">
	void Page_Load(Object sender,EventArgs e) {
		GoalDetailsPK PKInfo = new GoalDetailsPK((string)Application["SoccerDBConnectionString"]);

		try {
			GoalDetailsPKInformation.InnerHtml = PKInfo.ShowPK();
			UpdateHistoryMessage((string)Application["retrieveInfoMsg"] + "比數詳情 - 十二碼(" + DateTime.Now.ToString("HH:mm:ss") + ")");
		}	catch(NullReferenceException) {
			FormsAuthentication.SignOut();
			UpdateHistoryMessage("<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>");
		}
	}

	void UpdatePKAction(Object sender,CommandEventArgs e) {
		int iUpdated = 0;
		string sNow;
		GoalDetailsPK PKSend = new GoalDetailsPK((string)Application["SoccerDBConnectionString"]);
		sNow = DateTime.Now.ToString("HH:mm:ss");
		try {
			iUpdated = PKSend.UpdatePK();
			Page_Load(sender,e);
			if(iUpdated > 0) {
				UpdateHistoryMessage("成功更新十二碼資訊(" + sNow + ")");
			}	else if(iUpdated == 0) {
				UpdateHistoryMessage("沒有更新十二碼(" + sNow + ")");
			}	else {
				UpdateHistoryMessage((string)Application["transErrorMsg"] + "(" + sNow + ")");
			}
		}	catch(NullReferenceException nullex) {
			FormsAuthentication.SignOut();
			UpdateHistoryMessage("<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>");
		}
	}

	void UpdateHistoryMessage(string sMsg) {
		historyMsg.Text = sMsg;
	}
</script>

<script language="JavaScript">
function numbercheck(validate_index) {
	order_re = /^[01]{0,1}$/
	re_hostval = GoalDetailsPKForm.hostpk[validate_index].value.search(order_re);
	if(re_hostval == -1) {
		alert('十二碼序號只接受數字:0或1');
		GoalDetailsPKForm.hostpk[validate_index].value ="";
		return false;
	}

	re_val = GoalDetailsPKForm.guestpk[validate_index].value.search(order_re);
	if(re_val == -1) {
		alert('十二碼只接受數字:0或1');

		GoalDetailsPKForm.guestpk[validate_index].value ="";
		return false;
	}
	return true;
}
</script>

<html>
<head>
	<META http-equiv="Content-Type" content="text/html; charset=big5">
	<LINK REL="stylesheet" HREF="/sportStyle.css" TYPE="text/css">
</head>
<body>
	<form id="GoalDetailsPKForm" method="post" runat="server">
		<font size="2"><b>上次行動:</b><asp:Label id="historyMsg" runat="server" /></font>
		<br>
		<table border="1" width="100%" style="font: 10pt verdana">
			<span id="GoalDetailsPKInformation" runat="server" />&nbsp;
			<tr>
				<th colspan=21>
					<!--
						Value of SendToPager is Device ID defined in DEVICE_TYPE
					-->
					<input type="checkbox" name="SendToPager" value="1" checked>GOGO1&nbsp;
					<input type="checkbox" name="SendToPager" value="2" checked>GOGO2&nbsp;
					<input type="checkbox" name="SendToPager" value="3" checked>馬會機&nbsp;
					<input type="checkbox" name="SendToPager" value="4" checked>JCCombo&nbsp;
					<input type="checkbox" name="SendToPager" value="5" checked>GOGO3Combo&nbsp;
					<asp:Button id="SendBtn" Text="傳送" CommandName="SEND" OnCommand="UpdatePKAction" runat="server" />
					<input type="button" id="RefreshBtn" value="更新" OnServerClick="Page_Load" runat="server">
					12碼註解:&nbsp;0:不入&nbsp;&nbsp;&nbsp;1:射入
				</th>
			</tr>
		</table>
	</form>
</body>
</html>