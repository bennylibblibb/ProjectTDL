<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">
	void Page_Load(Object sender,EventArgs e) {
		LiveGoalOneOnly oneGoalInfo = new LiveGoalOneOnly((string)Application["SoccerDBConnectionString"]);

		try {
			OneGoalInformation.InnerHtml = oneGoalInfo.GetLiveGoal();
			UpdateHistoryMessage((string)Application["retrieveInfoMsg"] + "���@" + DateTime.Now.ToString("HH:mm"));
		}	catch(NullReferenceException) {
			FormsAuthentication.SignOut();
			UpdateHistoryMessage("<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>");
		}
	}

	void OnUpdatePeriod(Object sender,EventArgs e) {
		int iUpdated = 0;
		string sNow;
		LiveGoalOneOnly oneGoal = new LiveGoalOneOnly((string)Application["SoccerDBConnectionString"]);

		sNow = DateTime.Now.ToString("HH:mm");
		try {
			iUpdated = oneGoal.UpdateLiveGoal("STA");
			Page_Load(sender,e);
			if(iUpdated > 0) {
				UpdateHistoryMessage("��s�ɬq@" + sNow);
			}	else {
				UpdateHistoryMessage((string)Application["transErrorMsg"] + "@" + sNow);
			}
		}	catch(NullReferenceException nullex) {
			FormsAuthentication.SignOut();
			UpdateHistoryMessage("<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>");
		}
	}

	void UpdateHistoryMessage(string sMsg) {
		historyMsg.Text = sMsg;
	}

	void OnHostGoal(Object sender,EventArgs e) {
		int iUpdated = 0;
		string sNow;
		LiveGoalOneOnly oneGoal = new LiveGoalOneOnly((string)Application["SoccerDBConnectionString"]);

		sNow = DateTime.Now.ToString("HH:mm");
		try {
			iUpdated = oneGoal.UpdateLiveGoal("HCG");
			Page_Load(sender,e);
			if(iUpdated > 0) {
				UpdateHistoryMessage("��s���@" + sNow);
			}	else {
				UpdateHistoryMessage((string)Application["transErrorMsg"] + "(" + sNow + ")");
			}
		}	catch(NullReferenceException nullex) {
			FormsAuthentication.SignOut();
			UpdateHistoryMessage("<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>");
		}
	}

	void OnHostWrongGoal(Object sender,EventArgs e) {
		int iUpdated = 0;
		string sNow;
		LiveGoalOneOnly oneGoal = new LiveGoalOneOnly((string)Application["SoccerDBConnectionString"]);

		sNow = DateTime.Now.ToString("HH:mm");
		try {
			iUpdated = oneGoal.UpdateLiveGoal("HWG");
			Page_Load(sender,e);
			if(iUpdated > 0) {
				UpdateHistoryMessage("�D���B�k@" + sNow);
			}	else {
				UpdateHistoryMessage((string)Application["transErrorMsg"] + "(" + sNow + ")");
			}
		}	catch(NullReferenceException nullex) {
			FormsAuthentication.SignOut();
			UpdateHistoryMessage("<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>");
		}
	}

	void OnGuestGoal(Object sender,EventArgs e) {
		int iUpdated = 0;
		string sNow;
		LiveGoalOneOnly oneGoal = new LiveGoalOneOnly((string)Application["SoccerDBConnectionString"]);

		sNow = DateTime.Now.ToString("HH:mm");
		try {
			iUpdated = oneGoal.UpdateLiveGoal("GCG");
			Page_Load(sender,e);
			if(iUpdated > 0) {
				UpdateHistoryMessage("��s���@" + sNow);
			}	else {
				UpdateHistoryMessage((string)Application["transErrorMsg"] + "(" + sNow + ")");
			}
		}	catch(NullReferenceException nullex) {
			FormsAuthentication.SignOut();
			UpdateHistoryMessage("<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>");
		}
	}

	void OnGuestWrongGoal(Object sender,EventArgs e) {
		int iUpdated = 0;
		string sNow;
		LiveGoalOneOnly oneGoal = new LiveGoalOneOnly((string)Application["SoccerDBConnectionString"]);

		sNow = DateTime.Now.ToString("HH:mm");
		try {
			iUpdated = oneGoal.UpdateLiveGoal("GWG");
			Page_Load(sender,e);
			if(iUpdated > 0) {
				UpdateHistoryMessage("�ȶ��B�k@" + sNow);
			}	else {
				UpdateHistoryMessage((string)Application["transErrorMsg"] + "(" + sNow + ")");
			}
		}	catch(NullReferenceException nullex) {
			FormsAuthentication.SignOut();
			UpdateHistoryMessage("<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>");
		}
	}

	void OnHostRed1(Object sender,EventArgs e) {
		int iUpdated = 0;
		string sNow;
		LiveGoalOneOnly oneGoal = new LiveGoalOneOnly((string)Application["SoccerDBConnectionString"]);

		sNow = DateTime.Now.ToString("HH:mm");
		try {
			iUpdated = oneGoal.UpdateLiveGoal("HR1");
			Page_Load(sender,e);
			if(iUpdated > 0) {
				UpdateHistoryMessage("�D�����P1@" + sNow);
			}	else {
				UpdateHistoryMessage((string)Application["transErrorMsg"] + "(" + sNow + ")");
			}
		}	catch(NullReferenceException nullex) {
			FormsAuthentication.SignOut();
			UpdateHistoryMessage("<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>");
		}
	}

	void OnHostRed2(Object sender,EventArgs e) {
		int iUpdated = 0;
		string sNow;
		LiveGoalOneOnly oneGoal = new LiveGoalOneOnly((string)Application["SoccerDBConnectionString"]);

		sNow = DateTime.Now.ToString("HH:mm");
		try {
			iUpdated = oneGoal.UpdateLiveGoal("HR2");
			Page_Load(sender,e);
			if(iUpdated > 0) {
				UpdateHistoryMessage("�D�����P2@" + sNow);
			}	else {
				UpdateHistoryMessage((string)Application["transErrorMsg"] + "(" + sNow + ")");
			}
		}	catch(NullReferenceException nullex) {
			FormsAuthentication.SignOut();
			UpdateHistoryMessage("<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>");
		}
	}

	void OnHostRed3(Object sender,EventArgs e) {
		int iUpdated = 0;
		string sNow;
		LiveGoalOneOnly oneGoal = new LiveGoalOneOnly((string)Application["SoccerDBConnectionString"]);

		sNow = DateTime.Now.ToString("HH:mm");
		try {
			iUpdated = oneGoal.UpdateLiveGoal("HR3");
			Page_Load(sender,e);
			if(iUpdated > 0) {
				UpdateHistoryMessage("�D�����P3@" + sNow);
			}	else {
				UpdateHistoryMessage((string)Application["transErrorMsg"] + "(" + sNow + ")");
			}
		}	catch(NullReferenceException nullex) {
			FormsAuthentication.SignOut();
			UpdateHistoryMessage("<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>");
		}
	}

	void OnGuestRed1(Object sender,EventArgs e) {
		int iUpdated = 0;
		string sNow;
		LiveGoalOneOnly oneGoal = new LiveGoalOneOnly((string)Application["SoccerDBConnectionString"]);

		sNow = DateTime.Now.ToString("HH:mm");
		try {
			iUpdated = oneGoal.UpdateLiveGoal("GR1");
			Page_Load(sender,e);
			if(iUpdated > 0) {
				UpdateHistoryMessage("�ȶ����P1@" + sNow);
			}	else {
				UpdateHistoryMessage((string)Application["transErrorMsg"] + "(" + sNow + ")");
			}
		}	catch(NullReferenceException nullex) {
			FormsAuthentication.SignOut();
			UpdateHistoryMessage("<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>");
		}
	}

	void OnGuestRed2(Object sender,EventArgs e) {
		int iUpdated = 0;
		string sNow;
		LiveGoalOneOnly oneGoal = new LiveGoalOneOnly((string)Application["SoccerDBConnectionString"]);

		sNow = DateTime.Now.ToString("HH:mm");
		try {
			iUpdated = oneGoal.UpdateLiveGoal("GR2");
			Page_Load(sender,e);
			if(iUpdated > 0) {
				UpdateHistoryMessage("�ȶ����P2@" + sNow);
			}	else {
				UpdateHistoryMessage((string)Application["transErrorMsg"] + "(" + sNow + ")");
			}
		}	catch(NullReferenceException nullex) {
			FormsAuthentication.SignOut();
			UpdateHistoryMessage("<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>");
		}
	}

	void OnGuestRed3(Object sender,EventArgs e) {
		int iUpdated = 0;
		string sNow;
		LiveGoalOneOnly oneGoal = new LiveGoalOneOnly((string)Application["SoccerDBConnectionString"]);

		sNow = DateTime.Now.ToString("HH:mm");
		try {
			iUpdated = oneGoal.UpdateLiveGoal("GR3");
			Page_Load(sender,e);
			if(iUpdated > 0) {
				UpdateHistoryMessage("�ȶ����P3@" + sNow);
			}	else {
				UpdateHistoryMessage((string)Application["transErrorMsg"] + "(" + sNow + ")");
			}
		}	catch(NullReferenceException nullex) {
			FormsAuthentication.SignOut();
			UpdateHistoryMessage("<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>");
		}
	}
</script>

<html>
<head>
	<META http-equiv="Content-Type" content="text/html; charset=big5">
	<LINK REL="stylesheet" HREF="/sportStyle.css" TYPE="text/css">
</head>
<body>
	<form id="OneLiveGoalForm" method="post" runat="server">
		<table border="1" width="100%" style="font: 10pt verdana">
			<span id="OneGoalInformation" runat="server" />
			<tr align="center">
				<td>
					<a ID="UpdatePeriodLink" OnServerClick="OnUpdatePeriod" runat="server">�o�e�ɬq</a>&nbsp;&nbsp;
					<a ID="RefreshLink" OnServerClick="Page_Load" runat="server">��s����</a>
					<br>
					<font size="1">
					<asp:Label id="historyMsg" runat="server" />
					</font>
				</td>
				<td style="background-color:#FFC733">
					<a ID="HostGoalLink" OnServerClick="OnHostGoal" runat="server">�J�y</a>&nbsp;&nbsp;
					<a ID="HostWrongGoalLink" OnServerClick="OnHostWrongGoal" runat="server">�B�k</a>
					<br>
					<font color="red">���P</font>&nbsp;
					<a ID="HostRed1Link" OnServerClick="OnHostRed1" runat="server">1</a>&nbsp;&nbsp;
					<a ID="HostRed2Link" OnServerClick="OnHostRed2" runat="server">2</a>&nbsp;&nbsp;
					<a ID="HostRed3Link" OnServerClick="OnHostRed3" runat="server">3</a>
				</td>
				<td style="background-color:#FFEFD5">
					<a ID="GuestGoalLink" OnServerClick="OnGuestGoal" runat="server">�J�y</a>&nbsp;&nbsp;
					<a ID="GuestWrongGoalLink" OnServerClick="OnGuestWrongGoal" runat="server">�B�k</a>
					<br>
					<font color="red">���P</font>&nbsp;
					<a ID="GuestRed1Link" OnServerClick="OnGuestRed1" runat="server">1</a>&nbsp;&nbsp;
					<a ID="GuestRed2Link" OnServerClick="OnGuestRed2" runat="server">2</a>&nbsp;&nbsp;
					<a ID="GuestRed3Link" OnServerClick="OnGuestRed3" runat="server">3</a>
				</td>
			</tr>
			<!--
				Value of SendToPager is Device ID defined in DEVICE_TYPE
			-->
			<input type="hidden" name="SendToPager" value="1">
			<input type="hidden" name="SendToPager" value="2">
			<input type="hidden" name="SendToPager" value="3">
			<input type="hidden" name="SendToPager" value="4">
			<input type="hidden" name="SendToPager" value="5">
		</table>
	</form>
</body>
</html>