<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">
	void Page_Load(Object sender,EventArgs e) {
		AnalysisRecent matchRecent = new AnalysisRecent((string)Application["SoccerDBConnectionString"]);

		try {
			AnalysisRecentInformation.InnerHtml = matchRecent.GetRecent();
			UpdateHistoryMessage((string)Application["retrieveInfoMsg"] + "隊伍近績(" + DateTime.Now.ToString("HH:mm:ss") + ")");
		} catch(NullReferenceException) {
			FormsAuthentication.SignOut();
			UpdateHistoryMessage("<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>");
		}
	}

	void onSendRecentAnalysis(Object sender,EventArgs e) {
		int iUpdated = 0;
		string sNow;
		AnalysisRecent recentAnly = new AnalysisRecent((string)Application["SoccerDBConnectionString"]);

		sNow = DateTime.Now.ToString("HH:mm:ss");
		try {
			iUpdated = recentAnly.Update();
			Page_Load(sender,e);
			if(iUpdated >= 0) {
				UpdateHistoryMessage("成功修改隊伍近績(" + sNow + ")");
			} else {
				UpdateHistoryMessage("沒有修改隊伍近績(" + sNow + ")");
			}
		}	catch(NullReferenceException nullex) {
			FormsAuthentication.SignOut();
			UpdateHistoryMessage("<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>");
		}
	}
	
	void onSaveRecentAnalysis(Object sender,EventArgs e) {
		int iUpdated = 0;
		string sNow;
		AnalysisRecent recentAnly = new AnalysisRecent((string)Application["SoccerDBConnectionString"]);

		sNow = DateTime.Now.ToString("HH:mm:ss");
		try {
			iUpdated = recentAnly.SaveRecord();
			Page_Load(sender,e);
			if(iUpdated >= 0) {
				UpdateHistoryMessage("成功儲存隊伍近績(" + sNow + ")");
			} else {
				UpdateHistoryMessage("沒有儲存隊伍近績(" + sNow + ")");
			}
		}	catch(NullReferenceException nullex) {
			FormsAuthentication.SignOut();
			UpdateHistoryMessage("<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>");
		}
	}

	void UpdateHistoryMessage(string sMsg) {
		historyMsg.Text = sMsg;
	}
</script>

<script language="javascript">
function OnActionChanged() {
	if(AnalysisRecentForm.Action.value == 'D') {
		AnalysisRecentForm.SendToPager[0].checked = true;
		AnalysisRecentForm.SendToPager[1].checked = true;
		AnalysisRecentForm.SendToPager[2].checked = true;
		//alert('請確定GOGO1,GOGO2及馬會機已選取！');
	}
}

function LengthOfString(s) {
	len = 0;
	for(i = 0; i < s.length; i++) {
		if((s.charCodeAt(i) <= 255) && (s.charCodeAt(i) >= 0)) {
			len++;
		} else {
			len = len + 2;
		}
	}
	return len;
}

function checkHNextChallengerLength() {
	str = document.AnalysisRecentForm.HNextChallenger.value;
	strlen = LengthOfString(str);
	if(strlen > 8) {
		alert('隊伍名稱不能多於8個位元');
		document.AnalysisRecentForm.HNextChallenger.value = '';
	}
}

function checkHNextLeagueLength() {
	str = document.AnalysisRecentForm.HNextLeague.value;
	strlen = LengthOfString(str);
	if(strlen > 8) {
		alert('聯賽名稱不能多於8個位元');
		document.AnalysisRecentForm.HNextLeague.value = '';
	}
}

function checkHRecentChallengerLength(idx) {
	str = document.AnalysisRecentForm.HRecentChallenger[idx].value;
	strlen = LengthOfString(str);
	if(strlen > 8) {
		alert('隊伍名稱不能多於8個位元');
		document.AnalysisRecentForm.HRecentChallenger[idx].value = '';
	}
}

function checkHRecentLeagueLength(idx) {
	str = document.AnalysisRecentForm.HRecentLeague[idx].value;
	strlen = LengthOfString(str);
	if(strlen > 8) {
		alert('聯賽名稱不能多於8個位元');
		document.AnalysisRecentForm.HRecentLeague[idx].value = '';
	}
}

function checkGNextChallengerLength() {
	str = document.AnalysisRecentForm.GNextChallenger.value;
	strlen = LengthOfString(str);
	if(strlen > 8) {
		alert('隊伍名稱不能多於8個位元');
		document.AnalysisRecentForm.GNextChallenger.value = '';
	}
}

function checkGNextLeagueLength() {
	str = document.AnalysisRecentForm.GNextLeague.value;
	strlen = LengthOfString(str);
	if(strlen > 8) {
		alert('聯賽名稱不能多於8個位元');
		document.AnalysisRecentForm.GNextLeague.value = '';
	}
}

function checkGRecentChallengerLength(idx) {
	str = document.AnalysisRecentForm.GRecentChallenger[idx].value;
	strlen = LengthOfString(str);
	if(strlen > 8) {
		alert('隊伍名稱不能多於8個位元');
		document.AnalysisRecentForm.GRecentChallenger[idx].value = '';
	}
}

function checkGRecentLeagueLength(idx) {
	str = document.AnalysisRecentForm.GRecentLeague[idx].value;
	strlen = LengthOfString(str);
	if(strlen > 8) {
		alert('聯賽名稱不能多於8個位元');
		document.AnalysisRecentForm.GRecentLeague[idx].value = '';
	}
}

function checkHRecentHScoreFormat(idx) {
	re = /^\d{0,2}$/
	re_val = document.AnalysisRecentForm.HRecentHScore[idx].value.search(re)
	if(re_val == -1) {
		alert('比數只接受數字');
		document.AnalysisRecentForm.HRecentHScore[idx].value = '';
	}
}

function checkHRecentGScoreFormat(idx) {
	re = /^\d{0,2}$/
	re_val = document.AnalysisRecentForm.HRecentGScore[idx].value.search(re)
	if(re_val == -1) {
		alert('比數只接受數字');
		document.AnalysisRecentForm.HRecentGScore[idx].value = '';
	}
}

function checkGRecentHScoreFormat(idx) {
	re = /^\d{0,2}$/
	re_val = document.AnalysisRecentForm.GRecentHScore[idx].value.search(re)
	if(re_val == -1) {
		alert('比數只接受數字');
		document.AnalysisRecentForm.GRecentHScore[idx].value = '';
	}
}

function checkGRecentGScoreFormat(idx) {
	re = /^\d{0,2}$/
	re_val = document.AnalysisRecentForm.GRecentGScore[idx].value.search(re)
	if(re_val == -1) {
		alert('比數只接受數字');
		document.AnalysisRecentForm.GRecentGScore[idx].value = '';
	}
}

function DeviceCheck() {
	if(AnalysisRecentForm.Action.value == 'D') {
		AnalysisRecentForm.SendToPager[0].checked = true;
		AnalysisRecentForm.SendToPager[1].checked = true;
		AnalysisRecentForm.SendToPager[2].checked = true;
	} else {
		if((AnalysisRecentForm.SendToPager[0].checked == true) || (AnalysisRecentForm.SendToPager[1].checked == true)) {
			AnalysisRecentForm.SendToPager[0].checked = true;
			AnalysisRecentForm.SendToPager[1].checked = true;
		}
	}
}
</script>

<html>
<head>
	<META http-equiv="Content-Type" content="text/html; charset=big5">
	<LINK REL="stylesheet" HREF="/sportStyle.css" TYPE="text/css">
</head>
<body >
	<form id="AnalysisRecentForm" method="post" runat="server" onsubmit="DeviceCheck()">
		<font size="2"><b>上次行動:</b><asp:Label id="historyMsg" runat="server" /></font><br>
		<table border="1" width="100%" style="font: 10pt verdana">
		<span id="AnalysisRecentInformation" runat="server" />
		<tr>
			<th colspan="2" align="left">
				<font color="red">在刪除資料時，請確定所有傳呼機已選取，以避免傳呼機資料有錯誤！</font>
			</th>
			<td align="right">
				<!--
					Value of SendToPager is Device ID defined in DEVICE_TYPE
				-->
				<input type="checkbox" name="SendToPager" value="1" checked>GOGO1&nbsp;
				<input type="checkbox" name="SendToPager" value="2" checked>GOGO2&nbsp;
				<input type="checkbox" name="SendToPager" value="3" checked>馬會機&nbsp;
				<input type="checkbox" name="SendToPager" value="4" checked>JCCombo&nbsp;
				<!--
				<input type="checkbox" name="SendToPager" value="5" checked>GOGO3Combo&nbsp;
				-->
				<input type="submit" id="SaveBtn" value="儲存" OnServerClick="onSaveRecentAnalysis" runat="server">&nbsp;
				<input type="reset" value="重設">
				<input type="submit" id="SendBtn" value="發送" OnServerClick="onSendRecentAnalysis" runat="server">
			</td>
		</table>
	</form>
</body>
</html>