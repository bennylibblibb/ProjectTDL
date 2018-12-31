<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">
	string sAlias;

	void Page_Load(Object sender,EventArgs e) {
		Rank leagRank = new Rank(ConfigurationManager.AppSettings["SoccerDBConnectionString"]);

		try {
			RankInformation.InnerHtml = leagRank.ShowRank();
			sAlias = leagRank.Alias;
			UpdateHistoryMessage(ConfigurationManager.AppSettings["retrieveInfoMsg"] + "�p�ɱƦW(" + DateTime.Now.ToString("HH:mm:ss") + ")");
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
				UpdateHistoryMessage("���\��s" + iUpdated.ToString() + "���ƦW(" + sNow + ")");
			}	else if(iUpdated == 0) {
				UpdateHistoryMessage("�S����s�ƦW");
			} else {
				UpdateHistoryMessage("��s�ƦW����");
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
		alert('�W���u�����Ʀr');
		RankForm.rank[index].value = '';
	}
}

function GamesValidity(index) {
	games_re = /^\d{0,2}$/
	re_val = RankForm.games[index].value.search(games_re)
	if(re_val == -1) {
		alert('���ƥu�����Ʀr');
		RankForm.games[index].value = '';
	}
}

function ScoreValidity(index) {
	score_re = /^\d{0,3}$/
	re_val = RankForm.score[index].value.search(score_re)
	if(re_val == -1) {
		alert('�n���u�����Ʀr');
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
		//alert('�нT�wGOGO1,GOGO2�ΰ��|���w����I');
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
		<font size="2"><b>�W�����:</b><asp:Label id="historyMsg" runat="server" /></font><br>
		<table border="1" width="50%" style="font: 10pt verdana">
		<tr style="background-color:#FFD700">
			<th><select name="action" onChange="ActionChanged()"><option value="U">��s<option value="D">�R��<option value="P">�u�M���ǩI��</select>&nbsp;<font color="#9932CC"><%=sAlias%></font><font color="#808080">�ƦW</font></th>
			<th colspan="4">�Y�O���g��]�A�Цb�n���@���J�J�y�Ʀr</th>
		</tr>
		<tr style="background-color:#FFD700">
			<th>�W��</th>
			<th>�Ȭw����</th>
			<th>���|����</th>
			<th>����</th>
			<th>�n��</th>
		</tr>
		<span id="RankInformation" runat="server" />
		<tr>
			<td colspan="4" align="right">
				<!--
					Value of SendToPager is Device ID defined in DEVICE_TYPE
				-->
				<input type="checkbox" name="SendToPager" value="1" checked>GOGO1&nbsp;
				<input type="checkbox" name="SendToPager" value="1" checked>GOGO2&nbsp;
				<input type="checkbox" name="SendToPager" value="3" checked>���|��&nbsp;
				<input type="checkbox" name="SendToPager" value="4" checked>Combo
				<input type="submit" id="SaveBtn" value="�o�e" OnServerClick="SendRank" runat="server">&nbsp;
				<input type="reset" value="���]">
			</td>
		</tr>
		<tr>
			<td colspan="4" align="left">
				�p����ʧ@�O<font color="blue">�u�M���ǩI��</font>�A�u���ǩI���ƾڷ|�Q�R���C
			</td>
		</table>
	</form>
</body>
</html>