<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">
	void Page_Load(Object sender,EventArgs e) {
		string sLeagueOption = "";
		string sAnalysisOption = "";
		string sReviewOption = "";
		SoccerMenuLeague MenuLeague = new SoccerMenuLeague((string)Application["SoccerDBConnectionString"]);
		SoccerMenuAnalysis MenuAnalysis = new SoccerMenuAnalysis((string)Application["SoccerDBConnectionString"]);
		SoccerMenuReport MenuReport = new SoccerMenuReport((string)Application["GOGO2SOCDBConnectionString"], (string)Application["SoccerDBConnectionString"]);
		try {
			sLeagueOption = MenuLeague.Show();
			LeagueInformation.InnerHtml = sLeagueOption;
			//OtherResultInformation.InnerHtml = sLeagueOption;

			sAnalysisOption = MenuAnalysis.Show();
			matchModifyInformation.InnerHtml = sAnalysisOption;
			GoalDetailsInformation.InnerHtml = sAnalysisOption;

			sReviewOption = MenuReport.Show();
			MatchReportInformation.InnerHtml = sReviewOption;

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
		parent.content_frame.location.replace('NewMatchRetrieval.aspx?leagID=' + selectedLeague);
	}

	SoccerMenuForm.soccerMenuLeague.value = '0';
}
/*
function goToOtherResult(selectedLeague) {
	if(selectedLeague!='' && selectedLeague!='0') {
		parent.content_frame.location.replace('OtherResults.aspx?leagID=' + selectedLeague);
	}

	SoccerMenuForm.otherResult.value = '0';
}
*/
function goToMatchModify(matchModify) {
	if(matchModify!='' && matchModify!='0') {
		parent.content_frame.location.replace('ModifyMatch.aspx?matchcount=' + matchModify);
	}

	SoccerMenuForm.matchModify.value = '0';
}

function goToGoalDetails(details) {
	if(details!='' && details !='0') {
		parent.content_frame.location.replace('GoalDetails.aspx?match_cnt=' + details);
	}

	SoccerMenuForm.goalDetails.value = '0';
}

function goToMatchReport(details) {
	if(details!='' && details !='0') {
		parent.content_frame.location.replace('MatchReport.aspx?irecID=' + details);
	}

	SoccerMenuForm.MatchReport.value = '0';
}
</script>

<html>
<head>
	<META http-equiv="Content-Type" content="text/html; charset=big5">
	<LINK REL="stylesheet" HREF="/sportStyle.css" TYPE="text/css">
</head>
<body>
	<form id="SoccerMenuForm" method="post" runat="server">
		<asp:Label id="rtnMsg" runat="server" />
		<table width="100%">
			<tr align="left">
				<th>���y</th>
				<td><a href="../index.htm" target="_top">��^�D��</a></td>
				<td><a href="SoccerSchedule.aspx?leagID=0000" target="content_frame">�פJ�ɨ�</a></td>
				<td>�s�W�ɨ�</td>
				<td><a href="redirectToAllMatches.htm" target="content_frame">�Ҧ��ɨ�</a></td>
				<td><a href="redirectToLiveGoals.htm" target="content_frame">�{�����</a></td>
				<td>��ƸԱ�</td>
				<!--
				<td>��L���</td>
				<td>�ɫ���i</td>
				-->
				<td><a href="ForeOdds.aspx" target="content_frame">�w������</a></td>
				<td><a href="redirectToMenu2.htm" target="menu_frame">&gt;&gt;</a></td>
			</tr>
			<tr align="left">
				<td></td>
				<td colspan="2"><a href="redirectToMenu1.htm" target="menu_frame">��s���</a></td>
				<td>
					<select name="soccerMenuLeague" onChange="goToLeague(SoccerMenuForm.soccerMenuLeague.value)">
						<option value="0">�п��</option>
						<span id="LeagueInformation" runat="server" />
					</select>
				</td>
				<td>
					<select name="matchModify" onChange="goToMatchModify(SoccerMenuForm.matchModify.value)">
						<option value="0">�ק��ɨ�</option>
						<span id="matchModifyInformation" runat="server" />
					</select>
				</td>
				<td></td>
				<td>
					<select name="goalDetails" onChange="goToGoalDetails(SoccerMenuForm.goalDetails.value)">
						<option value="0">�п��</option>
						<span id="GoalDetailsInformation" runat="server" />
					</select>
				</td>
				<!--
				<td>
					<select name="otherResult" onChange="goToOtherResult(SoccerMenuForm.otherResult.value)">
						<option value="0">�п��</option>
						<span id="OtherResultInformation" runat="server" />
					</select>
				</td>
				<td>
					<select name="MatchReport" onChange="goToMatchReport(SoccerMenuForm.MatchReport.value)">
						<option value="0">�п��</option>
						<span id="MatchReportInformation" runat="server" />
					</select>
				</td>
				-->
				<td></td>
			</tr>
		</table>
	</form>
</body>
</html>