<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">
	string sTeam;
	string sLeagID;

	void Page_Load(Object sender,EventArgs e) {
		Scorers scorer = new Scorers(ConfigurationManager.AppSettings["SoccerDBConnectionString"]);

		try {
			ScorersSelectionInformation.InnerHtml = scorer.ShowPlayers();
			sTeam = scorer.Team;
			sLeagID = scorer.LeagID;
		} catch(NullReferenceException) {
			FormsAuthentication.SignOut();
			UpdateHistoryMessage("<a href=\"/sports/index.htm\" target=\"_top\">" + ConfigurationManager.AppSettings["sessionExpiredMsg"] + "</a>");
		}
	}

	void UpdateHistoryMessage(string sMsg) {
		historyMsg.Text = sMsg;
	}
</script>

<script language="JavaScript">
function GetAbbr(team) {
	abbr = '';
	if((team.charCodeAt(0) <= 127) && (team.charCodeAt(0) >= 0)) {	//First character is single-byte
		if((team.charCodeAt(1) <= 127) && (team.charCodeAt(1) >= 0)) {	//Second character is single-byte
			if((team.charCodeAt(2) <= 127) && (team.charCodeAt(2) >= 0)) {	//Third character is single-byte
				if((team.charCodeAt(3) <= 127) && (team.charCodeAt(3) >= 0)) {	//Fourth character is single-byte
					abbr = team.substr(0,4);
				} else {	//Fourth character is double-byte
					abbr = team.substr(0,3);
				}
			} else {	//Third character is double-byte
				abbr = team.substr(0,3);
			}
		} else {	//Second character is double-byte
			abbr = team.substr(0,2);
		}
	} else {	//First character is double-byte
		if((team.charCodeAt(1) <= 127) && (team.charCodeAt(1) >= 0)) {	//Second character is single-byte
			if((team.charCodeAt(2) <= 127) && (team.charCodeAt(2) >= 0)) {	//Third character is single-byte
				abbr = team.substr(0,3);
			} else {	//Third character is double-byte
				abbr = team.substr(0,2);
			}
		} else {	//Second character is double-byte
			abbr = team.substr(0,2);
		}
	}

	return abbr;
}

function UpdateMainWin() {
	team = document.ScorersSelectionForm.teamname.value;
    if (team != '') {
        var strName = document.ScorersSelectionForm.player.value;   
        var indexStr = document.ScorersSelectionForm.player.value.indexOf('|');
         var indexStr2 = document.ScorersSelectionForm.player.value.indexOf('||');
        var planycnname = strName.substring(0, indexStr);  
        var planyenname = strName.substring(indexStr + 1,  indexStr2);  
        var planerID = strName.substring(indexStr2+2, strName.length); 
         alert( strName+'    --'+ planycnname + ' a ' + planyenname+ ' b ' + planerID);
		abbr = GetAbbr(team);
		window.opener.document.ScorersForm.abbr[Number(document.ScorersSelectionForm.RecordIndex.value)].value = abbr;
		window.opener.document.ScorersForm.team[Number(document.ScorersSelectionForm.RecordIndex.value)].value = team;
        window.opener.document.ScorersForm.teamid[Number(document.ScorersSelectionForm.RecordIndex.value)].value = document.ScorersSelectionForm.teamID.value;
        window.opener.document.ScorersForm.playerenname[Number(document.ScorersSelectionForm.RecordIndex.value)].value = planyenname;
        window.opener.document.ScorersForm.player[Number(document.ScorersSelectionForm.RecordIndex.value)].value = planycnname;
        window.opener.document.ScorersForm.playid[Number(document.ScorersSelectionForm.RecordIndex.value)].value = planerID;
	}
	window.close();
}
</script>

<html>
<head>
	<META http-equiv="Content-Type" content="text/html; charset=big5">
	<LINK REL="stylesheet" HREF="/sportStyle.css" TYPE="text/css">
	<title><%=sTeam%>球員</title>
</head>
<body>
	<form id="ScorersSelectionForm" method="post" ONSUBMIT="UpdateMainWin()" runat="server">
		<font size="2"><asp:Label id="historyMsg" runat="server" /></font><br>

		<table border="1" width="100%" style="font: 10pt verdana">
			<tr align="center" style="background-color:#3A6A7E; color=#FFFFFF">
				<th>請選擇球員</th>
			</tr>

			<span id="ScorersSelectionInformation" runat="server" />

			<tr align="right">
				<td>
					<input type="submit" id="SendBtn" value="選擇">&nbsp;
					<input type="reset" value="重設">
				</td>
			</tr>
		</table>
	</form>
</body>
</html>