<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">
	int iRecCount;

	void Page_Load(Object sender,EventArgs e) {
		ImportMslot mslotInfo = new ImportMslot((string)Application["SoccerDBConnectionString"]);
		try {
			MslotInformation.InnerHtml = mslotInfo.GetMslotMatches();
			iRecCount = mslotInfo.NumberOfRecords;
			UpdateHistoryMessage((string)Application["retrieveInfoMsg"] + "澳門網賽事(" + DateTime.Now.ToString("HH:mm:ss") + ")");
		} catch(NullReferenceException) {
			FormsAuthentication.SignOut();
			UpdateHistoryMessage("<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>");
		}
	}

	void ImportMslotAction(Object sender,EventArgs e) {
		int iUpdated = 0;
		string sNow;
		ImportMslot mslotImpt = new ImportMslot((string)Application["SoccerDBConnectionString"]);

		sNow = DateTime.Now.ToString("HH:mm:ss");
		try {
			iUpdated = mslotImpt.Import();
			Page_Load(sender,e);
			if(iUpdated > 0) {
				UpdateHistoryMessage("成功匯入" + iUpdated + "場澳門網賽事(" + sNow + ")");
			} else if(iUpdated == 0) {
				UpdateHistoryMessage("沒有選擇賽事匯入或賽事已經匯入(" + sNow + ")");
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
	if(ImportMslotForm.SelectAllSend.checked == true) {
		<%
			for(iSendChkIndex=0;iSendChkIndex<iRecCount;iSendChkIndex++) {
				sSendChk_All = "importItem[" + iSendChkIndex.ToString() + "]";
		%>
				ImportMslotForm.<%=sSendChk_All%>.checked = true;
		<%
			}
		%>
	}
	else {
		<%
			for(iSendChkIndex=0;iSendChkIndex<iRecCount;iSendChkIndex++) {
				sSendChk_All = "importItem[" + iSendChkIndex.ToString() + "]";
		%>
				ImportMslotForm.<%=sSendChk_All%>.checked = false;
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
	if(ImportMslotForm.SelectAllSend.checked == true) {
		ImportMslotForm.importItem.checked = true;
	}
	else {
		ImportMslotForm.importItem.checked = false;
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
	<form id="ImportMslotForm" method="post" runat="server">
		<h3>匯入澳門網(www.macauslot.com)的賽事</h3>
		<font size="2"><b>上次行動:</b><asp:Label id="historyMsg" runat="server" /></font><br>

		<table border="1" width="60%" style="font: 10pt verdana">
		<tr style="background-color:#3CB371">
			<th>全選<input type="checkbox" name="SelectAllSend" onClick="selectAll()" checked></th>
			<th>日期</th>
			<th>時間</th>
			<th>聯賽</th>
			<th>主隊</th>
			<th>客隊</th>
		</tr>

		<span id="MslotInformation" runat="server" />

		<tr>
			<td colspan="6" align="right">
				<input type="submit" id="ImportBtn" value="匯入" OnServerClick="ImportMslotAction" runat="server">
			</td>
		</tr>
		</table>
	</form>
</body>
</html>