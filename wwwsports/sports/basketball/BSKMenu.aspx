<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">
	void Page_Load(Object sender,EventArgs e) {
		BSKMenu menu = new BSKMenu((string)Application["BasketballDBConnectionString"]);
		try {
			MatchInformation.InnerHtml = menu.GetMatchLeagues();
			ModifyInformation.InnerHtml = menu.GetMatches();
			GroupRankInformation.InnerHtml = menu.GetGroupRankLeagues();
			PersonalRankInformation.InnerHtml = menu.GetPersonalRankLeagues();
			PlayerInformation.InnerHtml = menu.GetTeams();
			ResultInformation.InnerHtml = menu.GetResultMatch();
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
function onAddMatch(leagID) {
	if(leagID!='' && leagID!='0') {
		parent.content_frame.location.replace('BSKNewMatch.aspx?leagID=' + leagID);
	}

	BasketballMenuForm.AddMatch.value = '0';
}

function onModifyMatch(leagID) {
	if(leagID!='' && leagID!='0') {
		parent.content_frame.location.replace('BSKModifyMatch.aspx?leagID=' + leagID);
	}

	BasketballMenuForm.ModifyMatch.value = '0';
}

function changeGroupRank(leagID) {
	if(leagID!='' && leagID!='0') {
		parent.content_frame.location.replace('BSKGroupRank.aspx?leagID=' + leagID);
	}

	BasketballMenuForm.GroupRank.value = '0';
}

function changePersonalRank(leagID) {
	if(leagID!='' && leagID!='0') {
		parent.content_frame.location.replace('BSKPersonalRank.aspx?leagID=' + leagID);
	}

	BasketballMenuForm.PersonalRank.value = '0';
}

function goToModifyResult(MatchID) {
	if(MatchID!='' && MatchID!='0') {
		parent.content_frame.location.replace('BSKModifyResult.aspx?MatchID=' + MatchID);
	}

	BasketballMenuForm.ModifyResult.value = '0';
}

function goToLeagAdmin(url) {
	if(url!='' && url!='0') {
		parent.content_frame.location.replace(url);
	}

	BasketballMenuForm.LeagueAdmin.value = '0';
}

function goToTeamAdmin(url) {
	if(url!='' && url!='0') {
		parent.content_frame.location.replace(url);
	}

	BasketballMenuForm.TeamAdmin.value = '0';
}

function goToPlayerAdmin(teamID) {
	if(teamID!='' && teamID!='0') {
		parent.content_frame.location.replace('BSKPlayers.aspx?teamID=' + teamID);
	}

	BasketballMenuForm.TeamID.value = '0';
}
</script>

<html>
<head>
	<META http-equiv="Content-Type" content="text/html; charset=big5">
	<LINK REL="stylesheet" HREF="/sportStyle.css" TYPE="text/css">
</head>
<body>
	<form id="BasketballMenuForm" method="post" runat="server">
		<asp:Label id="rtnMsg" runat="server" />
		<table width="100%">
			<tr align="left">
				<th>籃球</th>
				<td><a href="../index.htm" target="_top">返回主頁</a></td>
				<td>新增賽事</td>
				<td><a href="redirectToBSKAllMatch.htm" target="content_frame">所有賽事</a></td>
				<td><a href="redirectToBSKLiveGoal.htm" target="content_frame">現場比數</a></td>
				<td>小組排名</td>
				<td>個人統計</td>
				<td>修改賽果</td>
				<td>聯賽管理</td>
				<td>隊伍管理</td>
				<td>球員管理</td>
			</tr>
			<tr align="left">
				<td></td>
				<td>
					<a href="redirectToBSKMenu.htm" target="menu_frame">更新選單</a>
				</td>
				<td>
					<select name="AddMatch" onChange="onAddMatch(BasketballMenuForm.AddMatch.value)">
						<option value="0">請選擇</option>
						<span id="MatchInformation" runat="server" />
					</select>
				</td>
				<td>
					<select name="ModifyMatch" onChange="onModifyMatch(BasketballMenuForm.ModifyMatch.value)">
						<option value="0">修改賽事</option>
						<span id="ModifyInformation" runat="server" />
					</select>
				</td>
				<td></td>
				<td>
					<select name="GroupRank" onChange="changeGroupRank(BasketballMenuForm.GroupRank.value)">
						<option value="0">請選擇</option>
						<span id="GroupRankInformation" runat="server" />
					</select>
				</td>
				<td>
					<select name="PersonalRank" onChange="changePersonalRank(BasketballMenuForm.PersonalRank.value)">
						<option value="0">請選擇</option>
						<span id="PersonalRankInformation" runat="server" />
					</select>
				</td>
				<td>
					<select name="ModifyResult" onChange="goToModifyResult(BasketballMenuForm.ModifyResult.value)">
						<option value="0">請選擇</option>
						<span id="ResultInformation" runat="server" />
					</select>
				</td>
				<td>
					<select name="LeagueAdmin" onChange="goToLeagAdmin(BasketballMenuForm.LeagueAdmin.value)">
						<option value="0">請選擇</option>
						<option value="BSKNewLeague.aspx">新增</option>
						<option value="BSKModifyLeagueMenu.aspx">修改</option>
						<option value="BSKDeleteLeague.aspx">刪除</option>
					</select>
				</td>
				<td>
					<select name="TeamAdmin" onChange="goToTeamAdmin(BasketballMenuForm.TeamAdmin.value)">
						<option value="0">請選擇</option>
						<option value="BSKNewTeam.aspx">新增</option>
						<option value="BSKModifyTeamMenu.aspx">修改</option>
						<option value="BSKDeleteTeam.aspx">刪除</option>
					</select>
				</td>
				<td>
					<select name="TeamID" onChange="goToPlayerAdmin(BasketballMenuForm.TeamID.value)">
						<option value="0">請選擇</option>
						<span id="PlayerInformation" runat="server" />
					</select>
				</td>
			</tr>
		</table>
	</form>
</body>
</html>