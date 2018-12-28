<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">
	void Page_Load(Object sender,EventArgs e) {
		string sLeagueOption = "";
		SoccerMenuLeague MenuLeague = new SoccerMenuLeague((string)Application["SoccerDBConnectionString"]);
		try {
			sLeagueOption = MenuLeague.Show();
			AddInformation.InnerHtml = sLeagueOption;
		} catch(NullReferenceException) {
			FormsAuthentication.SignOut();
			UpdateReturnMessage("<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>");
		}
	}

	void UpdateReturnMessage(string sMsg) {
		rtnMsg.Text = sMsg;
	}
</script>

<script language="JavaScript">
function goToAdd(selectedLeague) {
	if(selectedLeague!='' && selectedLeague!='0') {
		parent.content_frame.location.replace('AddSoccerSchedule.aspx?leagID=' + selectedLeague);
	}

	ScheduleMenuForm.addScheduleLeague.value = '0';
}
</script>

<html>
<head>
	<META http-equiv="Content-Type" content="text/html; charset=big5">
	<LINK REL="stylesheet" HREF="/sportStyle.css" TYPE="text/css">
</head>
<body>
	<form id="ScheduleMenuForm" method="post" runat="server">
		<asp:Label id="rtnMsg" runat="server" />
		<table width="60%">
			<tr align="center">
				<th>���y�ɵ{</th>
				<td><a href="../index.htm" target="_top">��^�D��</a></td>
				<td>�s�W�ɵ{</td>
				<td><a href="ListSoccerSchedule.aspx?leagID=0000" target="content_frame">�ק�/�R���ɵ{</a></td>
			</tr>
			<tr align="center">
				<td></td>
				<td><a href="redirectToMenu.htm" target="menu_frame">��s���</a></td>
				<td>
					<select name="addScheduleLeague" onChange="goToAdd(ScheduleMenuForm.addScheduleLeague.value)">
						<option value="0">�п��</option>
						<span id="AddInformation" runat="server" />
					</select>
				</td>
			</tr>
		</table>
	</form>
</body>
</html>