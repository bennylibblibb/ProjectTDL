<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">
	int iRecCount;
	string sRegion;

	void Page_Load(Object sender,EventArgs e) {
		LiveOddsDelete liveoddsInfo = new LiveOddsDelete((string)Application["SoccerDBConnectionString"]);

		try {
			LiveOddsInformation.InnerHtml = liveoddsInfo.GetMatches();
			iRecCount = liveoddsInfo.NumberOfRecords;
			sRegion = liveoddsInfo.RegionName;
			UpdateHistoryMessage((string)Application["retrieveInfoMsg"] + "現場賠率(" + DateTime.Now.ToString("HH:mm:ss") + ")");
		} catch(NullReferenceException) {
			FormsAuthentication.SignOut();
			UpdateHistoryMessage("<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>");
		}
	}

	void DeleteMslotAction(Object sender,EventArgs e) {
		int iUpdated = 0;
		string sNow;
		LiveOddsDelete liveoddsDel = new LiveOddsDelete((string)Application["SoccerDBConnectionString"]);
		sNow = DateTime.Now.ToString("HH:mm:ss");
		try {
			iUpdated = liveoddsDel.Delete();
			Page_Load(sender,e);
			if(iUpdated > 0) {
				UpdateHistoryMessage("成功清除" + iUpdated + "場現場賠率(" + sNow + ")");
			} else if(iUpdated == 0) {
				UpdateHistoryMessage("沒有選擇現場賠率刪除(" + sNow + ")");
			} else {
				UpdateHistoryMessage((string)Application["transErrorMsg"] + "(" + sNow + ")");
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

<script language="JavaScript">
<%
	if(iRecCount > 1) {
%>
function selectAll() {
	<%
		int iSendChkIndex;
		string sSendChk_All;
	%>
	if(DeleteLiveOddsForm.SelectAllSend.checked == true) {
		<%
			for(iSendChkIndex=0;iSendChkIndex<iRecCount;iSendChkIndex++) {
				sSendChk_All = "deleteItem[" + iSendChkIndex.ToString() + "]";
		%>
				DeleteLiveOddsForm.<%=sSendChk_All%>.checked = true;
		<%
			}
		%>
	}
	else {
		<%
			for(iSendChkIndex=0;iSendChkIndex<iRecCount;iSendChkIndex++) {
				sSendChk_All = "deleteItem[" + iSendChkIndex.ToString() + "]";
		%>
				DeleteLiveOddsForm.<%=sSendChk_All%>.checked = false;
		<%
			}
		%>
	}
}
<%
}
else {
%>
function selectAll() {
	if(DeleteLiveOddsForm.SelectAllSend.checked == true) {
		DeleteLiveOddsForm.deleteItem.checked = true;
	}
	else {
		DeleteLiveOddsForm.deleteItem.checked = false;
	}
}
<%
	}
%>
</script>

<html>
<head>
	<META http-equiv="Content-Type" content="text/html; charset=big5">
	<LINK REL="stylesheet" HREF="/sportStyle.css" TYPE="text/css">
</head>
<body>
	<form id="DeleteLiveOddsForm" method="post" runat="server">
		<h3>刪除<%=sRegion%>現場賠率</h3>
		<font size="2"><b>上次行動:</b><asp:Label id="historyMsg" runat="server" /></font><br>

		<table border="1" width="60%" style="font: 10pt verdana">
		<tr style="background-color:#FFF5EE">
			<th>全選<input type="checkbox" name="SelectAllSend" onClick="selectAll()"></th>
			<th>日期</th>
			<th>時間</th>
			<th>聯賽</th>
			<th>主隊</th>
			<th>客隊</th>
		</tr>

		<span id="LiveOddsInformation" runat="server" />

		<tr>
			<td colspan="6" align="right">
				<!--
					Value of SendToPager is Device ID defined in DEVICE_TYPE
				-->
				<input type="checkbox" name="SendToPager" value="1" checked style="visibility: hidden;">
				<input type="checkbox" name="SendToPager" value="2" checked style="visibility: hidden;">
				<input type="checkbox" name="SendToPager" value="5" checked style="visibility: hidden;">
				<input type="submit" id="DeleteBtn" value="刪除" OnServerClick="DeleteMslotAction" runat="server">
			</td>
		</tr>
		</table>
	</form>
</body>
</html>