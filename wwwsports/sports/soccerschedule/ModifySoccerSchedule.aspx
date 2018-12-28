<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">
	void Page_Load(Object sender,EventArgs e) {
		ModifySoccerSchedule match = new ModifySoccerSchedule((string)Application["SoccerDBConnectionString"]);

		try {
			ScheduleInformation.InnerHtml = match.GetMatches();
			UpdateHistoryMessage((string)Application["retrieveInfoMsg"] + "�ɵ{�ק�(" + DateTime.Now.ToString("HH:mm:ss") + ")");
		} catch(NullReferenceException) {
			FormsAuthentication.SignOut();
			UpdateHistoryMessage("<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>");
		}
	}

	void ModifyScheduleAction(Object sender,EventArgs e) {
		int iUpdated = 0;
		string sNow;
		ModifySoccerSchedule schedule = new ModifySoccerSchedule((string)Application["SoccerDBConnectionString"]);

		sNow = DateTime.Now.ToString("HH:mm:ss");
		try {
			iUpdated = schedule.Modify();
			if(iUpdated > 0) {
				UpdateHistoryMessage("���\�ק��ɵ{(" + sNow + ")");
			}	else if(iUpdated == 0) {
				UpdateHistoryMessage("�S���ɵ{�Q�ק�(" + sNow + ")");
			}	else if(iUpdated == -99) {
				UpdateHistoryMessage("�ɵ{�w�s�b�A�S���ק�(" + sNow + ")");
			}	else {
				UpdateHistoryMessage((string)Application["transErrorMsg"] + "(" + sNow + ")");
			}
			Response.Redirect("ListSoccerSchedule.aspx?leagID=0000");
		}	catch(NullReferenceException nullex) {
			FormsAuthentication.SignOut();
			UpdateHistoryMessage("<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>");
		}
	}

	void BackToSchedule(Object sender,EventArgs e) {
		Response.Redirect("ListSoccerSchedule.aspx?leagID=0000");
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
	<form id="ModifyScheduleForm" method="post" runat="server">
		<h3>�ק��ɵ{</h3>
		<font size="2"><b>�W�����:</b><asp:Label id="historyMsg" runat="server" /></font><br>

		<table border="1" width="60%" style="font: 10pt verdana">
		<tr style="background-color:#87CEEB; color:#FFFFE0" align="center">
			<th>���(�~/��/��)</th>
			<th>�ɶ�(��:��)</th>
			<th>�p��</th>
			<th>�D��</th>
			<th>�ȶ�</th>
			<th>�D��</th>
			<th>���߳�</th>
			<th>����</th>
		</tr>
		<tr align="center">
			<span id="ScheduleInformation" runat="server" />
		</tr>
		<tr><td colspan="8" align="right">
			<input type="submit" id="ModifyBtn" value="�ק�" OnServerClick="ModifyScheduleAction" runat="server">&nbsp;
			<input type="submit" id="BackBtn" value="��^" OnServerClick="BackToSchedule" runat="server">
		</td></tr>

	</form>
</body>
</html>