<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">
	int iRecCount;

	void Page_Load(Object sender,EventArgs e) {
		SoccerSchedule schedule = new SoccerSchedule((string)Application["SoccerDBConnectionString"]);

		try {
			ScheduleInformation.InnerHtml = schedule.list();
			iRecCount = schedule.RecordCount;
			UpdateHistoryMessage((string)Application["retrieveInfoMsg"] + "賽程作匯入(" + DateTime.Now.ToString("HH:mm:ss") + ")");
		} catch(NullReferenceException) {
			FormsAuthentication.SignOut();
			UpdateHistoryMessage("<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>");
		}
	}

	void ImportMatch(Object sender,EventArgs e) {
		int iUpdated = 0;
		string sNow;
		SoccerSchedule schedule = new SoccerSchedule((string)Application["SoccerDBConnectionString"]);

		sNow = DateTime.Now.ToString("HH:mm:ss");
		try {
			iUpdated = schedule.import();
			if(iUpdated > 0) {
				UpdateHistoryMessage("成功匯入" + iUpdated.ToString() + "場賽程(" + sNow + ")");
			}	else if(iUpdated == 0) {
				UpdateHistoryMessage("沒有賽程被匯入(" + sNow + ")");
			}	else if(iUpdated == -99) {
				UpdateHistoryMessage("賽程已存在，沒有匯入(" + sNow + ")");
			}	else {
				UpdateHistoryMessage(""+(string)Application["transErrorMsg"] + "(" + sNow + ")");
			}
			//Page_Load(sender,e);
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
<%
	if(iRecCount > 1) {
%>
function selectAll() {
	<%
		int iBGIdx;
		string sBG;
	%>
	if(ScheduleForm.all.checked == true) {
		<%
			for(iBGIdx=0;iBGIdx<iRecCount;iBGIdx++) {
				sBG = "importedMatch[" + iBGIdx.ToString() + "]";
		%>
				ScheduleForm.<%=sBG%>.checked = true;
		<%
			}
		%>
	}
	else {
		<%
			for(iBGIdx=0;iBGIdx<iRecCount;iBGIdx++) {
				sBG = "importedMatch[" + iBGIdx.ToString() + "]";
		%>
				ScheduleForm.<%=sBG%>.checked = false;
		<%
			}
		%>
	}
}
<%
	} else {
%>
function selectAll() {
	if(ScheduleForm.all.checked == true) {
		ScheduleForm.importedMatch.checked = true;
	}
	else {
		ScheduleForm.importedMatch.checked = false;
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
	<form id="ScheduleForm" method="post" runat="server">
		<font size="2"><b>上次行動:</b><asp:Label id="historyMsg" runat="server" /></font><br>

		<table border="1" width="70%" style="font: 10pt verdana">
			<span id="ScheduleInformation" runat="server" />

			<tr>
				<td colspan="4" align="right">
					<input type="submit" id="ImportBtn" value="匯入" OnServerClick="ImportMatch" runat="server">&nbsp;
					<input type="reset" value="重設">
				</td>
			</tr>
		</table>
	</form>
</body>
</html>