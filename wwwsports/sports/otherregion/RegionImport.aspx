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
			UpdateHistoryMessage((string)Application["retrieveInfoMsg"] + "�Ȭw��|�w��J���ɨ�(" + DateTime.Now.ToString("HH:mm:ss") + ")");
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
				UpdateHistoryMessage("���\�פJ" + iUpdated + "���ѨȬw��|��J���ɨ�(" + sNow + ")");
			} else if(iUpdated == 0) {
				UpdateHistoryMessage("�S������ɨƶפJ���ɨƤw�g�פJ(" + sNow + ")");
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
		<h3>�פJ�ѨȬw��|��J���ɨƨ�<%=sRegion%></h3>
		<font size="2"><b>�W�����:</b><asp:Label id="historyMsg" runat="server" /></font><br>

		<table border="1" width="60%" style="font: 10pt verdana">
		<!--
		<tr style="background-color:#00BFFF">
			<th>����<input type="checkbox" name="SelectAllSend" onClick="selectAll()" checked></th>
			<th>���</th>
			<th>�ɶ�</th>
			<th>�p��</th>
			<th>�D��</th>
			<th>�ȶ�</th>
		</tr>
		-->

		<span id="MslotInformation" runat="server" />

		<tr>
			<td colspan="6" align="right">
				<input type="submit" id="ImportBtn" value="�פJ" OnServerClick="ImportAction" runat="server">
			</td>
		</tr>
		</table>
	</form>
</body>
</html>