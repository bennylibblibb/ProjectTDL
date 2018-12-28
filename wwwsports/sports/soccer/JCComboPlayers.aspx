<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">
	int iRecCount;

	void Page_Load(Object sender,EventArgs e) {
		JCComboPlayers playerinfo = new JCComboPlayers((string)Application["SoccerDBConnectionString"]);

		try {
			PlayersInformation.InnerHtml = playerinfo.GetTeamPlayers();
			iRecCount = playerinfo.NumberOfRecords;
			UpdateHistoryMessage((string)Application["retrieveInfoMsg"] + "球員名單(" + DateTime.Now.ToString("HH:mm:ss") + ")");
		} catch(NullReferenceException) {
			FormsAuthentication.SignOut();
			UpdateHistoryMessage("<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>");
		}
	}

	void onModifyPlayers(Object sender,CommandEventArgs e) {
		int iUpdated = 0;
		string sNow, sCmd;
		JCComboPlayers modifyPlayer = new JCComboPlayers((string)Application["SoccerDBConnectionString"]);

		sCmd = (string)e.CommandName;
		sNow = DateTime.Now.ToString("HH:mm:ss");
		try {
			iUpdated = modifyPlayer.Update(sCmd);
			Page_Load(sender,e);
			if (sCmd.Equals("MOD")) {
				if (iUpdated > 0) {
					UpdateHistoryMessage("成功修改" + iUpdated + "個球員(" + sNow + ")");
				}	else if(iUpdated == 0) {
					UpdateHistoryMessage("沒有修改球員(" + sNow + ")");
				} else if(iUpdated == -99) {
					UpdateHistoryMessage("修改失敗，相同球員名稱已存在此隊伍！");
				} else {
					UpdateHistoryMessage((string)Application["transErrorMsg"] + "(" + sNow + ")");
				}
			} else if (sCmd.Equals("COPY")) {
				if (iUpdated > 0) {
					UpdateHistoryMessage("成功複製所有球員(" + sNow + ")");
				}	else if(iUpdated == 0) {
					UpdateHistoryMessage("沒有複製球員(" + sNow + ")");
				} else {
					UpdateHistoryMessage((string)Application["transErrorMsg"] + "(" + sNow + ")");
				}
			} else {
				if (iUpdated > 0) {
					UpdateHistoryMessage("成功刪除" + iUpdated + "個球員(" + sNow + ")");
				}	else if(iUpdated == 0) {
					UpdateHistoryMessage("沒有刪除球員(" + sNow + ")");
				} else {
					UpdateHistoryMessage((string)Application["transErrorMsg"] + "(" + sNow + ")");
				}
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

<script language="JavaScript">
function changeTeam(selectedTeam, selectedLeague) {
	parent.content_frame.location.replace('JCComboPlayers.aspx?teamID=' + selectedTeam + '&leagID=' + selectedLeague);
}

function selectALL() {
	<%
		int iSendChkIndex;
		string sSendChk_All;
	%>
	if(PlayersForm.selectAll.checked == true) {
		<%
			for(iSendChkIndex=0;iSendChkIndex<iRecCount;iSendChkIndex++) {
				sSendChk_All = "player_roster[" + iSendChkIndex.ToString() + "]";
		%>
				PlayersForm.<%=sSendChk_All%>.checked = true;
		<%
			}
		%>
	}
	else {
		<%
			for(iSendChkIndex=0;iSendChkIndex<iRecCount;iSendChkIndex++) {
				sSendChk_All = "player_roster[" + iSendChkIndex.ToString() + "]";
		%>
				PlayersForm.<%=sSendChk_All%>.checked = false;
		<%
			}
		%>
	}
}

function onPosChanged(validate_index) {
	re = /^\d{0,2}$/
	re_val = PlayersForm.player_no[validate_index].value.search(re)
	if(re_val == -1) {
		alert('球員號碼只接受數字');
		PlayersForm.player_no[validate_index].value = '';
	}
}

function goToSendComboPlayers(selectedTeam) {
	if(selectedTeam!='' && selectedTeam!='0') {
		parent.content_frame.location.replace('JCComboPlayers.aspx?teamID=000&leagID=' + selectedTeam);
	}
}

function sendComboPlayers() {
	flagWinFeature="Resizable=Yes, ScrollBars=Yes, MenuBar=No, Directories=No, ToolBar=No, Location=No, Status=No, left=200, top=40 , width=600, height=600";
	flagWindow = window.open('JCComboPlayerPreview.aspx', 'FormatterFlag' , flagWinFeature);
	flagWindow.focus();
}
</script>

<html>
<head>
	<META http-equiv="Content-Type" content="text/html; charset=big5">
	<LINK REL="stylesheet" HREF="/sportStyle.css" TYPE="text/css">
</head>
<body>
	<form id="PlayersForm" method="post" runat="server">
		<font size="2"><b>上次行動:</b><asp:Label id="historyMsg" runat="server" /></font><br>

		<table border="1" width="80%" style="font: 10pt verdana">
		<span id="PlayersInformation" runat="server" />
		<tr>
			<td align="center">
				<a href="javascript:sendComboPlayers()">發送</a>
			</td>
			<td align="center">
				<asp:Button id="CopyBtn" Text="複製" CommandName="COPY" OnCommand="onModifyPlayers" runat="server" />
			</td>
			<td colspan="4" align="right">
				<asp:Button id="ModBtn" Text="儲存" CommandName="MOD" OnCommand="onModifyPlayers" runat="server" />
				&nbsp;<input type="reset" value="重設">
			</td>
			<td align="right">
				<asp:Button id="DelBtn" Text="刪除" CommandName="DEL" OnCommand="onModifyPlayers" runat="server" />
			</td>
		</table>
	</form>
</body>
</html>