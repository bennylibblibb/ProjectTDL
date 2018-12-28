<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">
	void Page_Load(Object sender,EventArgs e) {
		BSKModifyMatch oneMatchInfo = new BSKModifyMatch((string)Application["BasketballDBConnectionString"]);

		try {
			OneMatchInformation.InnerHtml = oneMatchInfo.GetMatches();
			UpdateHistoryMessage((string)Application["retrieveInfoMsg"] + "賽事修改(" + DateTime.Now.ToString("HH:mm:ss") + ")");
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
				UpdateHistoryMessage("成功修改賽事資訊(" + sNow + ")");
			}	else if(iUpdated == 0) {
				UpdateHistoryMessage("沒有賽事被修改(" + sNow + ")");
			}	else if(iUpdated == -2) {
				UpdateHistoryMessage("賽事已存在，不能修改(" + sNow + ")");
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
		alert('主客隊不能同名!');
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
		<b>賽事修改</b>&nbsp;
		<font size="2"><b>上次行動:</b><asp:Label id="historyMsg" runat="server" /></font><br>

		<table border="1" width="60%" style="font: 10pt verdana">
		<tr style="background-color:#aaC047" align="center">
			<th>執行動作</th>
			<th>日期(年/月/日)</th>
			<th>時間(時:分)</th>
			<th>聯賽</th>
			<th>主隊</th>
			<th>客隊</th>
		</tr>
		<tr align="center">
			<td>
				<select name="action">
					<option value="U">修改
					<option value="D">刪除
				</select>
			</td>
			<span id="OneMatchInformation" runat="server" />
		</tr>
		<tr><td colspan="6" align="right">
			<font color="red">(此項修改必需發送到傳呼機)</font>
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
			<input type="submit" id="SendBtn" value="傳送" OnServerClick="BskModifyMatchAction" runat="server">&nbsp;
			<input type="submit" id="BackBtn" value="返回" OnServerClick="BackToMatchAction" runat="server">
		</td></tr>

	</form>
</body>
</html>