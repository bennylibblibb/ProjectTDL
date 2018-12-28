<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">
	int iRecCount;

	void Page_Load(Object sender,EventArgs e) {
		ListSoccerSchedule schedule = new ListSoccerSchedule((string)Application["SoccerDBConnectionString"]);

		try {
			ScheduleInformation.InnerHtml = schedule.list();
			iRecCount = schedule.RecordCount;
			UpdateHistoryMessage((string)Application["retrieveInfoMsg"] + "賽程(" + DateTime.Now.ToString("HH:mm:ss") + ")");
		} catch(NullReferenceException) {
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
function selectAllDelete() {
	<%
		int iIdx;
		string sItem;
	%>
	if(ScheduleForm.selectAll.checked == true) {
		<%
			for(iIdx=0;iIdx<iRecCount;iIdx++) {
				sItem = "selectedDelete[" + iIdx.ToString() + "]";
		%>
				ScheduleForm.<%=sItem%>.checked = true;
		<%
			}
		%>
	} else {
		<%
			for(iIdx=0;iIdx<iRecCount;iIdx++) {
				sItem = "selectedDelete[" + iIdx.ToString() + "]";
		%>
				ScheduleForm.<%=sItem%>.checked = false;
		<%
			}
		%>
	}
}
<%
	} else {
%>
function selectAllDelete() {
	if(ScheduleForm.selectAll.checked == true) {
		ScheduleForm.selectedDelete.checked = true;
	}
	else {
		ScheduleForm.selectedDelete.checked = false;
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
	<form name="ScheduleForm" method="post" action="DeleteSoccerSchedule.aspx">
		<font size="2"><b>上次行動:</b><asp:Label id="historyMsg" runat="server" /></font><br>

		<table border="1" width="70%" style="font: 10pt verdana">
			<span id="ScheduleInformation" runat="server" />
		</table>
	</form>
</body>
</html>