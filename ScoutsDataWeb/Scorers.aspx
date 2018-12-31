<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">
	string sAlias;

	void Page_Load(Object sender,EventArgs e) {
		Scorers scorer = new Scorers(ConfigurationManager.AppSettings["SoccerDBConnectionString"]);

		try {
			ScorersInformation.InnerHtml = scorer.ShowRank();
			sAlias = scorer.Alias;
			UpdateHistoryMessage(ConfigurationManager.AppSettings["retrieveInfoMsg"] + "�g��](" + DateTime.Now.ToString("HH:mm:ss") + ")");
		} catch(NullReferenceException) {
			FormsAuthentication.SignOut();
			UpdateHistoryMessage("<a href=\"/sports/index.htm\" target=\"_top\">" + ConfigurationManager.AppSettings["sessionExpiredMsg"] + "</a>");
		}
	}

	void SendScorer(Object sender,EventArgs e) {
		int iUpdated = 0;
		string sNow;
		Scorers updRank = new Scorers(ConfigurationManager.AppSettings["SoccerDBConnectionString"]);

		sNow = DateTime.Now.ToString("HH:mm:ss");
		try {
			iUpdated = updRank.Update();
			Page_Load(sender,e);
			if(iUpdated > 0) {
				UpdateHistoryMessage("���\��s�g��](" + sNow + ")");
			}	else if(iUpdated == 0) {
				UpdateHistoryMessage("�S����s�g��]");
			} else {
				UpdateHistoryMessage("��s�g��]����");
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
function InputHelp() {
	helpWinFeature="Resizable=Yes,ScrollBars=Yes,MenuBar=No,Directories=No,ToolBar=No,Location=No,Status=No, width=600, height=500";
	helpWindow = window.open('../help/ScorersHelp.htm', 'Help' , helpWinFeature);
	helpWindow.focus();
}

function GetPlayer(leagID, teamID, RecordIndex) {
	if(teamID != "0") {
		playerWinFeature="Resizable=Yes,ScrollBars=Yes,MenuBar=No,Directories=No,ToolBar=No,Location=No,Status=No, width=200, height=180";
		playerWindow = window.open('ScorersSelection.aspx?leagID=' + leagID + '&teamID=' + teamID + '&RecordIndex=' + RecordIndex, 'PlayersList' , playerWinFeature);
		playerWindow.focus();
		ScorersForm.teamref[RecordIndex].value = '0';
	}
}

function RankValidity(index) {
	scorer_re = /^\d{0,2}$/
	re_val = ScorersForm.rank[index].value.search(scorer_re)
	if(re_val == -1) {
		alert('�ƦW�u�����Ʀr');
		ScorersForm.rank[index].value = '';
	}
}

function GoalsValidity(index) {
	score_re = /^\d{0,2}$/
	re_val = ScorersForm.goals[index].value.search(score_re)
	if(re_val == -1) {
		alert('�J�y�u�����Ʀr');
		ScorersForm.goals[index].value = '';
	}
}

function DeviceCheck() {
	if(ScorersForm.action.value == 'D') {
		ScorersForm.SendToPager[0].checked = true;
		ScorersForm.SendToPager[1].checked = true;
	}
}

function ActionChanged() {
	if(ScorersForm.action.value == 'D') {
		ScorersForm.SendToPager[0].checked = true;
		ScorersForm.SendToPager[1].checked = true;
		//alert('�нT�wGOGO2�ΰ��|���w����I');
	}
}

function AbbrChange(index) {
	str = ScorersForm.abbr[index].value;
	strlen = LengthOfString(str);
	if(strlen > 4) {
		alert('�y�|²�٤���h��4�Ӧ줸');
		ScorersForm.abbr[index].value = '';
	}
}

function LengthOfString(s) {
	len = 0;
	for(i = 0; i < s.length; i++) {
		if((s.charCodeAt(i) <= 127) && (s.charCodeAt(i) >= 0)) {
			len++;
		} else {
			len = len + 2;
		}
	}
	return len;
}
</script>

<html>
<head>
	<META http-equiv="Content-Type" content="text/html; charset=big5">
	<LINK REL="stylesheet" HREF="/sportStyle.css" TYPE="text/css">
</head>
<body>
	<form id="ScorersForm" method="post" runat="server" onsubmit="DeviceCheck()">
		<font size="2"><b>�W�����:</b><asp:Label id="historyMsg" runat="server" /></font><br>

		<table border="1" width="55%" style="font: 10pt verdana">
			<tr align="center" style="background-color:#BFDFEF">
				<th colspan="5">
					�Х��F�Ѯg��]��<a href="javascript:InputHelp()">��J����</a>
				</th>
			</tr>
			<tr align="left" style="background-color:#3A6A7E; color=#FFFFFF">
				<th colspan="3"><font color="#F0FFF0"><%=sAlias%></font>�g��]</th>
				<th colspan="2">����ʧ@:<select name="action" onChange="ActionChanged()"><option value="U">��s<option value="D">�R��<option value="P">�u�M���ǩI��</select></th>
			</tr>
			<tr align="center" style="background-color:#3A6A7E; color=#FFFFFF">
				<th></th>
				<th>�y�|(²��)</th>
				<th>�y��</th>
				<th>�ƦW</th>
				<th>�J�y</th>
			</tr>

			<span id="ScorersInformation" runat="server" />

			<tr align="right">
				<td colspan="5">
					<!--
						Value of SendToPager is Device ID defined in DEVICE_TYPE
					-->
					<input type="checkbox" name="SendToPager" value="2" checked>GOGO2&nbsp;
					<input type="checkbox" name="SendToPager" value="3" checked>���|��&nbsp;
					<input type="checkbox" name="SendToPager" value="4" checked>Combo
					<input type="submit" id="SendBtn" value="�o�e" OnServerClick="SendScorer" tabindex="50" runat="server">&nbsp;
					<input type="reset" value="���]">
				</td>
			</tr>
		</table>
	</form>
</body>
</html>