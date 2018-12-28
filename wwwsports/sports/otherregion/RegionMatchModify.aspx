<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">
	void Page_Load(Object sender,EventArgs e) {
		RegionMatchModify match = new RegionMatchModify((string)Application["SoccerDBConnectionString"]);

		try {
			MatchInformation.InnerHtml = match.GetMatches();
			UpdateHistoryMessage((string)Application["retrieveInfoMsg"] + "賽事修改(" + DateTime.Now.ToString("HH:mm:ss") + ")");
		} catch(NullReferenceException) {
			FormsAuthentication.SignOut();
			UpdateHistoryMessage("<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>");
		}
	}

	void ModifyMatchAction(Object sender,EventArgs e) {
		int iUpdated = 0;
		string sNow;
		RegionMatchModify oneMatch = new RegionMatchModify((string)Application["SoccerDBConnectionString"]);

		sNow = DateTime.Now.ToString("HH:mm:ss");
		try {
			iUpdated = oneMatch.Modify();
			Page_Load(sender, e);
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
	<form id="RegionMatchModifyForm" method="post" runat="server">
		<font size="2"><b>上次行動:</b><asp:Label id="historyMsg" runat="server" /></font><br>

		<table border="1" width="60%" style="font: 10pt verdana">
		<tr style="background-color:#DDA0DD" align="center">
			<th>日期(年/月/日)</th>
			<th>時間(時:分)</th>
			<th>聯賽</th>
			<th>主隊</th>
			<th>客隊</th>
		</tr>
		<tr align="center">
			<span id="MatchInformation" runat="server" />
		</tr>
		<tr>
			<td colspan="5" align="right">
				<!--
					Value of SendToPager is Device ID defined in DEVICE_TYPE
				-->
				<input type="checkbox" name="SendToPager" value="1" checked style="visibility: hidden;">
				<input type="checkbox" name="SendToPager" value="2" checked style="visibility: hidden;">
				<input type="checkbox" name="SendToPager" value="5" checked style="visibility: hidden;">
				<font color="red">(修改有效於此賽事的任何地區，及必需發送到傳呼機)</font>
				<input type="submit" id="SendBtn" value="修改" OnServerClick="ModifyMatchAction" runat="server">&nbsp;
			</td>
		</tr>
		<tr>
			<td colspan="6" align="left">
			<b>請注意受影響的資訊:<br>
			修改賽事:其他地區賽事<br>
			刪除賽事:其他地區賽事
			</td>
		</tr>
	</form>
</body>
</html>