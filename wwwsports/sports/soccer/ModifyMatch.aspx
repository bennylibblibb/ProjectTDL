<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">
	void Page_Load(Object sender,EventArgs e) {
		ModifyMatch oneMatchInfo = new ModifyMatch((string)Application["SoccerDBConnectionString"]);

		try {
			OneMatchInformation.InnerHtml = oneMatchInfo.GetMatches();
			UpdateHistoryMessage((string)Application["retrieveInfoMsg"] + "賽事修改(" + DateTime.Now.ToString("HH:mm:ss") + ")");
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
				UpdateHistoryMessage("成功修改賽事資訊(" + sNow + ")");
			}	else if(iUpdated == 0) {
				UpdateHistoryMessage("沒有賽事被修改(" + sNow + ")");
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
		<b>賽事修改</b>&nbsp;
		<font size="2"><b>上次行動:</b><asp:Label id="historyMsg" runat="server" /></font><br>

		<table border="1" width="60%" style="font: 10pt verdana">
		<tr style="background-color:#FFC0CB" align="center">
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
				<font color="red">(此項修改必需發送到傳呼機)</font>
				<input type="submit" id="SendBtn" value="傳送" OnServerClick="ModifyMatchAction" runat="server">&nbsp;
				<input type="submit" id="BackBtn" value="返回" OnServerClick="BackToMatchAction" runat="server">
			</td>
		</tr>
		<tr>
			<td colspan="6" align="left">
			<b>請注意受影響的資訊:<br>
			修改賽事:賽事，預報指數，現場比數，比數詳情，波膽，分析賽事，分析數據，分析近績及現場賠率<br>
			刪除賽事:賽事，預報指數，現場比數，比數詳情，波膽，分析賽事，分析數據，分析近績，<font color="red">不包括現場賠率</font>
			</td>
		</tr>
		</table>
	</form>
</body>
</html>