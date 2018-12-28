<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">
	int iRecCount;
	string sRegion;

	void Page_Load(Object sender,EventArgs e) {
		RegionDelete regionInfo = new RegionDelete((string)Application["SoccerDBConnectionString"]);

		try {
			MslotInformation.InnerHtml = regionInfo.GetMatches();
			iRecCount = regionInfo.NumberOfRecords;
			sRegion = regionInfo.Region;
			UpdateHistoryMessage((string)Application["retrieveInfoMsg"] + sRegion + "賽事(" + DateTime.Now.ToString("HH:mm:ss") + ")");
		} catch(NullReferenceException) {
			FormsAuthentication.SignOut();
			UpdateHistoryMessage("<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>");
		}
	}

	void DeleteMslotAction(Object sender,EventArgs e) {
		int iUpdated = 0;
		string sNow;
		RegionDelete mslotImpt = new RegionDelete((string)Application["SoccerDBConnectionString"]);
		sNow = DateTime.Now.ToString("HH:mm:ss");
		try {
			iUpdated = mslotImpt.Delete();
			Page_Load(sender,e);
			if(iUpdated > 0) {
				UpdateHistoryMessage("成功清除" + iUpdated + "場" + sRegion + "賽事(" + sNow + ")");
			} else if(iUpdated == 0) {
				UpdateHistoryMessage("沒有選擇賽事清除(" + sNow + ")");
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
	if(DeleteRegionForm.SelectAllSend.checked == true) {
		<%
			for(iSendChkIndex=0;iSendChkIndex<iRecCount;iSendChkIndex++) {
				sSendChk_All = "deleteItem[" + iSendChkIndex.ToString() + "]";
		%>
				DeleteRegionForm.<%=sSendChk_All%>.checked = true;
		<%
			}
		%>
	}
	else {
		<%
			for(iSendChkIndex=0;iSendChkIndex<iRecCount;iSendChkIndex++) {
				sSendChk_All = "deleteItem[" + iSendChkIndex.ToString() + "]";
		%>
				DeleteRegionForm.<%=sSendChk_All%>.checked = false;
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
	if(DeleteRegionForm.SelectAllSend.checked == true) {
		DeleteRegionForm.deleteItem.checked = true;
	}
	else {
		DeleteRegionForm.deleteItem.checked = false;
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
	<form id="DeleteRegionForm" method="post" runat="server">
		<h3>刪除<%=sRegion%>的數據</h3>
		<font size="2"><b>上次行動:</b><asp:Label id="historyMsg" runat="server" /></font><br>

		<table border="1" width="60%" style="font: 10pt verdana">
		<!--
		<tr style="background-color:#E6E6FA">
			<th>全選<input type="checkbox" name="SelectAllSend" onClick="selectAll()"></th>
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
				<!--
					Value of SendToPager is Device ID defined in DEVICE_TYPE
				-->
				<input type="checkbox" name="SendToPager" value="1" checked style="visibility: hidden;">
				<input type="checkbox" name="SendToPager" value="2" checked style="visibility: hidden;">
				<!--
				<input type="checkbox" name="SendToPager" value="5" checked style="visibility: hidden;">
				-->
				<input type="submit" id="DeleteBtn" value="刪除" OnServerClick="DeleteMslotAction" runat="server">
			</td>
		</tr>
		</table>
	</form>
</body>
</html>