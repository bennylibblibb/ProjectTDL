<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">
	void Page_Load(Object sender,EventArgs e) {
		string sLeagueOption = "";
		string sAnalysisOption = "";
		SoccerMenuLeague MenuLeague = new SoccerMenuLeague((string)Application["SoccerDBConnectionString"]);
		OtherSoccerMenuAnalysis MenuAnalysis = new OtherSoccerMenuAnalysis((string)Application["SoccerDBConnectionString"]);
		try {
			sLeagueOption = MenuLeague.Show();
			LeagueInformation.InnerHtml = sLeagueOption;

			sAnalysisOption = MenuAnalysis.Show();
			matchModifyInformation.InnerHtml = sAnalysisOption;
			GoalDetailsInformation.InnerHtml = sAnalysisOption;

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
function goToLeague(selectedLeague) {
	if(selectedLeague!='' && selectedLeague!='0') {
		parent.content_frame.location.replace('OtherSoccerNewMatchRetrieval.aspx?leagID=' + selectedLeague);
	}

	OtherSoccerMenuForm.soccerMenuLeague.value = '0';
}

function goToMatchModify(matchModify) {
	if(matchModify!='' && matchModify!='0') {
		parent.content_frame.location.replace('OtherSoccerModifyMatch.aspx?matchcount=' + matchModify);
	}

	OtherSoccerMenuForm.matchModify.value = '0';
}

function goToGoalDetails(details) {
	if(details!='' && details !='0') {
		parent.content_frame.location.replace('OtherSoccerGoalDetails.aspx?match_cnt=' + details);
	}

	OtherSoccerMenuForm.goalDetails.value = '0';
}
</script>

<html>
<head>
	<META http-equiv="Content-Type" content="text/html; charset=big5">
	<LINK REL="stylesheet" HREF="/sportStyle.css" TYPE="text/css">
</head>
<body>
	<form id="OtherSoccerMenuForm" method="post" runat="server">
		<asp:Label id="rtnMsg" runat="server" />
		<table width="100%">
			<tr align="left">
				<th>�Ȭw 2 ���y</th>
				<td><a href="../index.htm" target="_top">��^�D��</a></td>
				<td><a href="OtherSoccerSchedule.aspx?leagID=0000" target="content_frame">�פJ�ɨ�</a></td>
				<td>�s�W�ɨ�</td>
				<td><a href="redirectToOtherSoccerAllMatches.htm" target="content_frame">�Ҧ��ɨ�</a></td>
				<td>��ƸԱ�</td>
				<td><a href="redirectToOtherSoccerLiveGoal.htm" target="content_frame">�{�����</a></td>
			</tr>
			<tr align="left">
				<td></td>
				<td colspan="2"><a href="redirectToMenu1.htm" target="menu_frame">��s���</a></td>
				<td>
					<select name="soccerMenuLeague" onChange="goToLeague(OtherSoccerMenuForm.soccerMenuLeague.value)">
						<option value="0">�п��</option>
						<span id="LeagueInformation" runat="server" />
					</select>
				</td>
				<td>
					<select name="matchModify" onChange="goToMatchModify(OtherSoccerMenuForm.matchModify.value)">
						<option value="0">�ק��ɨ�</option>
						<span id="matchModifyInformation" runat="server" />
					</select>
				</td></td>
				<td>
					<select name="goalDetails" onChange="goToGoalDetails(OtherSoccerMenuForm.goalDetails.value)">
						<option value="0">�п��</option>
						<span id="GoalDetailsInformation" runat="server" />
					</select>
				</td>
				<td></td>
			</tr>
		</table>
	</form>
</body>
</html>