<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">

	int iRecCount;
	
	void Page_Load(Object sender,EventArgs e) {
		FTPScheduleCheck scheduleCheck = new FTPScheduleCheck((string)Application["SoccerDBConnectionString"]);
		try {
			ScheduleCheckInformation.InnerHtml = scheduleCheck.ShowCheck();
			iRecCount = scheduleCheck.NumberOfRecords;
			UpdateHistoryMessage((string)Application["retrieveInfoMsg"] + "���i(" + DateTime.Now.ToString("HH:mm:ss") + ")");
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
				UpdateHistoryMessage("���\�R�����i(" + sNow + ")");
			}	else if(iUpdated == 0) {
				UpdateHistoryMessage("�S���R�����i(" + sNow + ")");
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
		<font size="2"><b>�W�����:</b><asp:Label id="historyMsg" runat="server" /></font><br>

		<table border="1" width="100%" style="font: 10pt verdana">
		<tr style="background-color:#0000CD; color=#FFFF00" align=center>
			<th align="left">���i�ɶ�</th>
			<th align="left">�p�ɦW��</th>
			<th align="left">�D���W��</th>
			<th align="left">�ȶ��W��</th>
			<th align="left">�ɮצW��</th>
			<th align="center">�R�����i&nbsp&nbsp<input type="reset" value="���]"><br>����<input type="checkbox" name="SelectAllDelete" onClick="selectAll()">&nbsp&nbsp&nbsp&nbsp<input type="submit" id="DeleteBtn" value="�R��" OnServerClick="onDeleteSchedule" runat="server"></th>
		</tr>
		<span id="ScheduleCheckInformation" runat="server" />
		<tr>
			<td colspan="6" align="left">
				<b>���y�ɵ{:</b><br>
				<font color="red">�H�W�����X�r�鬰--�������p��/����</font><br>
				<font color="blue">�H�W�Ŧ��X�r�鬰--���������p�ɤ�����</font>
			</td>
		</tr>
		</table>
	</form>
</body>
</html>