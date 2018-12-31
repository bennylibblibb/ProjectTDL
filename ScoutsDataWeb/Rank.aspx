<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">
	string sAlias;

	void Page_Load(Object sender,EventArgs e) {
		Rank leagRank = new Rank(ConfigurationManager.AppSettings["SoccerDBConnectionString"]);

		try {
			RankInformation.InnerHtml = leagRank.ShowRank();
			sAlias = leagRank.Alias;
			UpdateHistoryMessage(ConfigurationManager.AppSettings["retrieveInfoMsg"] + "聯賽排名(" + DateTime.Now.ToString("HH:mm:ss") + ")");
		} catch(NullReferenceException) {
			FormsAuthentication.SignOut();
			UpdateHistoryMessage("<a href=\"/sports/index.htm\" target=\"_top\">" + ConfigurationManager.AppSettings["sessionExpiredMsg"] + "</a>");
		}
	}

	void SendRank(Object sender,EventArgs e) {
		int iUpdated = 0;
		string sNow;
		Rank updRank = new Rank(ConfigurationManager.AppSettings["SoccerDBConnectionString"]);

		sNow = DateTime.Now.ToString("HH:mm:ss");
		try {
			iUpdated = updRank.Update();
			Page_Load(sender,e);
			if(iUpdated > 0) {
				UpdateHistoryMessage("成功更新" + iUpdated.ToString() + "隊排名(" + sNow + ")");
			}	else if(iUpdated == 0) {
				UpdateHistoryMessage("沒有更新排名");
			} else {
				UpdateHistoryMessage("更新排名失敗");
			}
		} catch(NullReferenceException nullex) {
			FormsAuthentication.SignOut();
			UpdateHistoryMessage("<a href=\"/sports/index.htm\" target=\"_top\">" + ConfigurationManager.AppSettings["sessionExpiredMsg"] + "</a>");
		}
	}

	void UpdateHistoryMessage(string sMsg) {
		historyMsg.Text = sMsg;
	}
</script>

<script language="JavaScript">
function RankValidity(index) {
	rank_re = /^\d{0,2}$/
	re_val = RankForm.rank[index].value.search(rank_re)
	if(re_val == -1) {
		alert('名次只接受數字');
		RankForm.rank[index].value = '';
	}
}

function GamesValidity(index) {
	games_re = /^\d{0,2}$/
	re_val = RankForm.games[index].value.search(games_re)
	if(re_val == -1) {
		alert('場數只接受數字');
		RankForm.games[index].value = '';
	}
}

function ScoreValidity(index) {
	score_re = /^\d{0,3}$/
	re_val = RankForm.score[index].value.search(score_re)
	if(re_val == -1) {
		alert('積分只接受數字');
		RankForm.score[index].value = '';
	}
}

function DeviceCheck() {
	if(RankForm.action.value == 'D') {
		RankForm.SendToPager[0].checked = true;
		RankForm.SendToPager[1].checked = true;
		RankForm.SendToPager[2].checked = true;
	}
}

function ActionChanged() {
	if(RankForm.action.value == 'D') {
		RankForm.SendToPager[0].checked = true;
		RankForm.SendToPager[1].checked = true;
		RankForm.SendToPager[2].checked = true;
		//alert('請確定GOGO1,GOGO2及馬會機已選取！');
	}
}
</script>

<html>
<head>
	<META http-equiv="Content-Type" content="text/html; charset=big5">
	<LINK REL="stylesheet" HREF="/sportStyle.css" TYPE="text/css">
</head>
<body>
	<form id="RankForm" method="post" runat="server" onsubmit="DeviceCheck()">
		<font size="2"><b>上次行動:</b><asp:Label id="historyMsg" runat="server" /></font><br>
		<table border="1" width="50%" style="font: 10pt verdana">
		<tr style="background-color:#FFD700">
			<th><select name="action" onChange="ActionChanged()"><option value="U">更新<option value="D">刪除<option value="P">只清除傳呼機</select>&nbsp;<font color="#9932CC"><%=sAlias%></font><font color="#808080">排名</font></th>
			<th colspan="4">若是神射手榜，請在積分一欄輸入入球數字</th>
		</tr>
		<tr style="background-color:#FFD700">
			<th>名次</th>
			<th>亞洲隊伍</th>
			<th>馬會隊伍</th>
			<th>場數</th>
			<th>積分</th>
		</tr>
		<span id="RankInformation" runat="server" />
		<tr>
			<td colspan="4" align="right">
				<!--
					Value of SendToPager is Device ID defined in DEVICE_TYPE
				-->
				<input type="checkbox" name="SendToPager" value="1" checked>GOGO1&nbsp;
				<input type="checkbox" name="SendToPager" value="1" checked>GOGO2&nbsp;
				<input type="checkbox" name="SendToPager" value="3" checked>馬會機&nbsp;
				<input type="checkbox" name="SendToPager" value="4" checked>Combo
				<input type="submit" id="SaveBtn" value="發送" OnServerClick="SendRank" runat="server">&nbsp;
				<input type="reset" value="重設">
			</td>
		</tr>
		<tr>
			<td colspan="4" align="left">
				如執行動作是<font color="blue">只清除傳呼機</font>，只有傳呼機數據會被刪除。
			</td>
		</table>
	</form>
</body>
</html>