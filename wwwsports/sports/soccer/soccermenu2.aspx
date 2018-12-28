<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">
	void Page_Load(Object sender,EventArgs e) {
		string sLeagueOption = "";
		string sAnalysisOption = "";
		SoccerMenuLeague MenuLeague = new SoccerMenuLeague((string)Application["SoccerDBConnectionString"]);
		SoccerMenuAnalysis MenuAnalysis = new SoccerMenuAnalysis((string)Application["SoccerDBConnectionString"]);
		try {
			sLeagueOption = MenuLeague.Show();
			RankInformation.InnerHtml = sLeagueOption;
			ScorersInformation.InnerHtml = sLeagueOption;
			PlayerInformation.InnerHtml = sLeagueOption;
			//JCComboPlayerInformation.InnerHtml = sLeagueOption;

			sAnalysisOption = MenuAnalysis.Show();
			AnalysisModifyInformation.InnerHtml = sAnalysisOption;
			CorrectScoreInformation.InnerHtml = sAnalysisOption;
			AnalysisRecentInformation.InnerHtml = sAnalysisOption;
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
function goToRank(selectedLeague) {
	if(selectedLeague!='' && selectedLeague!='0') {
		parent.content_frame.location.replace('Rank.aspx?leagID=' + selectedLeague);
	}

	SoccerMenuForm.RankDetails.value = '0';
}

function goToScorers(selectedLeague) {
	if(selectedLeague!='' && selectedLeague!='0') {
		parent.content_frame.location.replace('Scorers.aspx?leagID=' + selectedLeague);
	}

	SoccerMenuForm.Scorers.value = '0';
}

function goToTeam(selectedTeam) {
	if(selectedTeam!='' && selectedTeam!='0') {
		parent.content_frame.location.replace('PlayersRetrieval.aspx?teamID=000&leagID=' + selectedTeam);
	}

	SoccerMenuForm.soccerMenuPlayer.value = '0';
}

function goToComboTeam(selectedTeam) {
	if(selectedTeam!='' && selectedTeam!='0') {
		parent.content_frame.location.replace('JCComboPlayers.aspx?teamID=000&leagID=' + selectedTeam);
	}

	SoccerMenuForm.JCComboSoccerMenuPlayer.value = '0';
}

function goToAnalysisModify(selectedMatchModify) {
	if(selectedMatchModify!='' && selectedMatchModify!='0') {
		parent.content_frame.location.replace('AnalysisModify.aspx?matchcnt=' + selectedMatchModify);
	}

	SoccerMenuForm.soccerMenuAnalysisModify.value = '0';
}

function goToCorrectScore(selectedMatchModify) {
	if(selectedMatchModify!='' && selectedMatchModify!='0') {
		parent.content_frame.location.replace('CorrectScore.aspx?matchcnt=' + selectedMatchModify);
	}

	SoccerMenuForm.soccerCorrectScore.value = '0';
}

function goToAnalysisRecent(selectedRecentModify) {
	if(selectedRecentModify!='' && selectedRecentModify!='0') {
		parent.content_frame.location.replace('AnalysisRecent.aspx?matchcnt=' + selectedRecentModify);
	}

	SoccerMenuForm.AnalysisRecentModify.value = '0';
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
				<td><a href="redirectToMenu1.htm" target="menu_frame">&lt;&lt;</a></td>
				<td><a href="../index.htm" target="_top">��^�D��</a></td>
				<td>�i�x</td>
				<td>�p�ɱƦW</td>
				<td>�g��]</td>
				<td>�y��</td>
<!--
				<td>JCCombo�y��</td>
-->
				<td>�ק��ɨƤ��R</td>
				<td><a href="AnalysisPreview.aspx" target="content_frame">�o�e���R</a></td>
				<td><a href="redirectToAnlyStat.htm" target="content_frame">�ƾ�</a></td>
				<td>���Z</td>
				<td><a href="AnalysisSinglePreview.aspx" target="content_frame">�o�e���Z</a></td>
				<td><a href="redirectToMenu3.htm" target="menu_frame">&gt;&gt;</a></td>
			</tr>
			<tr align="left">
				<td></td><td></td>
				<td><a href="redirectToMenu2.htm" target="menu_frame">��s���</a></td>
				<td>
					<select name="soccerCorrectScore" onChange="goToCorrectScore(SoccerMenuForm.soccerCorrectScore.value)">
						<option value="0">�п��</option>
						<span id="CorrectScoreInformation" runat="server" />
					</select>
				</td>
				<td>
					<select name="RankDetails" onChange="goToRank(SoccerMenuForm.RankDetails.value)">
						<option value="0">�п��</option>
						<span id="RankInformation" runat="server" />
					</select>
				</td>
				<td>
					<select name="Scorers" onChange="goToScorers(SoccerMenuForm.Scorers.value)">
						<option value="0">�п��</option>
						<span id="ScorersInformation" runat="server" />
					</select>
				</td>
				<td>
					<select name="soccerMenuPlayer" onChange="goToTeam(SoccerMenuForm.soccerMenuPlayer.value)">
						<option value="0">�п��</option>
						<span id="PlayerInformation" runat="server" />
					</select>
				</td>
<!--
				<td>
					<select name="JCComboSoccerMenuPlayer" onChange="goToComboTeam(SoccerMenuForm.JCComboSoccerMenuPlayer.value)">
						<option value="0">�п��</option>
						<span id="JCComboPlayerInformation" runat="server" />
					</select>
				</td>
-->
				<td colspan="3">
					<select name="soccerMenuAnalysisModify" onChange="goToAnalysisModify(SoccerMenuForm.soccerMenuAnalysisModify.value)">
						<option value="0">�п��</option>
						<span id="AnalysisModifyInformation" runat="server" />
					</select>
				</td>
				<td colspan="2">
					<select name="AnalysisRecentModify" onChange="goToAnalysisRecent(SoccerMenuForm.AnalysisRecentModify.value)">
						<option value="0">�п��</option>
						<span id="AnalysisRecentInformation" runat="server" />
					</select>
				</td>
				<td></td>
			</tr>
		</table>
	</form>
</body>
</html>