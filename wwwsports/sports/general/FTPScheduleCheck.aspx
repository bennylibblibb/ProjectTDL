<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">

	int iRecCount;
	
	void Page_Load(Object sender,EventArgs e) {
		FTPScheduleCheck scheduleCheck = new FTPScheduleCheck((string)Application["SoccerDBConnectionString"]);
		try {
			ScheduleCheckInformation.InnerHtml = scheduleCheck.ShowCheck();
			iRecCount = scheduleCheck.NumberOfRecords;
			UpdateHistoryMessage((string)Application["retrieveInfoMsg"] + "報告(" + DateTime.Now.ToString("HH:mm:ss") + ")");
		} catch(NullReferenceException) {
			FormsAuthentication.SignOut();
			UpdateHistoryMessage("<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>");
		}
	}

	void onDeleteSchedule(Object sender,EventArgs e) {
		int iUpdated = 0;
		string sNow;
		FTPScheduleCheck scheduleCheck = new FTPScheduleCheck((string)Application["SoccerDBConnectionString"]);
		sNow = DateTime.Now.ToString("HH:mm:ss");
		try {
			iUpdated = scheduleCheck.Delete();
			Page_Load(sender,e);
			if(iUpdated > 0) {
				UpdateHistoryMessage("成功刪除報告(" + sNow + ")");
			}	else if(iUpdated == 0) {
				UpdateHistoryMessage("沒有刪除報告(" + sNow + ")");
			}	else {
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
	function selectAll() {
	<%
		int iDeleteIndex;
		string sDeleteAll;
	%>
		if(ScheduleCheckForm.SelectAllDelete.checked == true) {
		<%
			for(iDeleteIndex=0;iDeleteIndex<iRecCount;iDeleteIndex++) {
				sDeleteAll = "MustDelete[" + iDeleteIndex.ToString() + "]";
		%>
				ScheduleCheckForm.<%=sDeleteAll%>.checked = true;
		<%
			}
		%>
		} else {
		<%
			for(iDeleteIndex=0;iDeleteIndex<2;iDeleteIndex++) {
				sDeleteAll = "MustDelete[" + iDeleteIndex.ToString() + "]";
		%>
				ScheduleCheckForm.<%=sDeleteAll%>.checked = false;
		<%
			}
		%>
		}
	}
</script>

<html>
<head>
	<META http-equiv="Content-Type" content="text/html; charset=big5">
	<LINK REL="stylesheet" HREF="/sportStyle.css" TYPE="text/css">
</head>
<body>
	<form id="ScheduleCheckForm" method="post" runat="server">
		<font size="2"><b>上次行動:</b><asp:Label id="historyMsg" runat="server" /></font><br>

		<table border="1" width="100%" style="font: 10pt verdana">
		<tr style="background-color:#0000CD; color=#FFFF00" align=center>
			<th align="left">報告時間</th>
			<th align="left">聯賽名稱</th>
			<th align="left">主隊名稱</th>
			<th align="left">客隊名稱</th>
			<th align="left">檔案名稱</th>
			<th align="center">刪除報告&nbsp&nbsp<input type="reset" value="重設"><br>全選<input type="checkbox" name="SelectAllDelete" onClick="selectAll()">&nbsp&nbsp&nbsp&nbsp<input type="submit" id="DeleteBtn" value="刪除" OnServerClick="onDeleteSchedule" runat="server"></th>
		</tr>
		<span id="ScheduleCheckInformation" runat="server" />
		<tr>
			<td colspan="6" align="left">
				<b>足球賽程:</b><br>
				<font color="red">以上紅色突出字體為--未有此聯賽/隊伍</font><br>
				<font color="blue">以上藍色突出字體為--未有所屬聯賽之隊伍</font>
			</td>
		</tr>
		</table>
	</form>
</body>
</html>