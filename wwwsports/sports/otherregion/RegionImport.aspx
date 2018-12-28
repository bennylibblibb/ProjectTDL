<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">
	int iRecCount;
	string sRegion;

	void Page_Load(Object sender,EventArgs e) {
		RegionImport regionInfo = new RegionImport((string)Application["SoccerDBConnectionString"]);

		try {
			MslotInformation.InnerHtml = regionInfo.GetAsiaMatches();
			iRecCount = regionInfo.NumberOfRecords;
			sRegion = regionInfo.Region;
			UpdateHistoryMessage((string)Application["retrieveInfoMsg"] + "亞洲體育已輸入的賽事(" + DateTime.Now.ToString("HH:mm:ss") + ")");
		} catch(NullReferenceException) {
			FormsAuthentication.SignOut();
			UpdateHistoryMessage("<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>");
		}
	}

	void ImportAction(Object sender,EventArgs e) {
		int iUpdated = 0;
		string sNow;
		RegionImport regionImpt = new RegionImport((string)Application["SoccerDBConnectionString"]);

		sNow = DateTime.Now.ToString("HH:mm:ss");
		try {
			iUpdated = regionImpt.Import();
			Page_Load(sender,e);
			if(iUpdated > 0) {
				UpdateHistoryMessage("成功匯入" + iUpdated + "場由亞洲體育輸入的賽事(" + sNow + ")");
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
	if(ImportRegionForm.SelectAllSend.checked == true) {
		<%
			for(iSendChkIndex=0;iSendChkIndex<iRecCount;iSendChkIndex++) {
				sSendChk_All = "importItem[" + iSendChkIndex.ToString() + "]";
		%>
				ImportRegionForm.<%=sSendChk_All%>.checked = true;
		<%
			}
		%>
	}
	else {
		<%
			for(iSendChkIndex=0;iSendChkIndex<iRecCount;iSendChkIndex++) {
				sSendChk_All = "importItem[" + iSendChkIndex.ToString() + "]";
		%>
				ImportRegionForm.<%=sSendChk_All%>.checked = false;
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
	if(ImportRegionForm.SelectAllSend.checked == true) {
		ImportRegionForm.importItem.checked = true;
	}
	else {
		ImportRegionForm.importItem.checked = false;
	}
}
<%
	}
%>

function filterSchedule(url) {
	parent.content_frame.location.replace(url);
}

</script>

<html>
<head>
	<META http-equiv="Content-Type" content="text/html; charset=big5">
	<LINK REL="stylesheet" HREF="/sportStyle.css" TYPE="text/css">
</head>
<body>
	<form id="ImportRegionForm" method="post" runat="server">
		<h3>匯入由亞洲體育輸入的賽事到<%=sRegion%></h3>
		<font size="2"><b>上次行動:</b><asp:Label id="historyMsg" runat="server" /></font><br>

		<table border="1" width="60%" style="font: 10pt verdana">
		<!--
		<tr style="background-color:#00BFFF">
			<th>全選<input type="checkbox" name="SelectAllSend" onClick="selectAll()" checked></th>
			<th>日期</th>
			<th>時間</th>
			<th>聯賽</th>
			<th>主隊</th>
			<th>客隊</th>
		</tr>
		-->

		<span id="MslotInformation" runat="server" />

		<tr>
			<td colspan="6" align="right">
				<input type="submit" id="ImportBtn" value="匯入" OnServerClick="ImportAction" runat="server">
			</td>
		</tr>
		</table>
	</form>
</body>
</html>