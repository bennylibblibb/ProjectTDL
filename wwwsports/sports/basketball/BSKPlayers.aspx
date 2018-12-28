<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">
	string sTeam;

	void Page_Load(Object sender,EventArgs e) {
		BSKPlayers players = new BSKPlayers((string)Application["BasketballDBConnectionString"]);

		try {
			PlayersInformation.InnerHtml = players.GetTeamPlayers();
			sTeam = players.Team;
			UpdateHistoryMessage((string)Application["retrieveInfoMsg"] + "球員名單(" + DateTime.Now.ToString("HH:mm:ss") + ")");
		} catch(NullReferenceException) {
			FormsAuthentication.SignOut();
			UpdateHistoryMessage("<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>");
		}
	}

	void onModifyPlayers(Object sender,CommandEventArgs e) {
		int iUpdated = 0;
		string sNow, sCmd;
		BSKPlayers player = new BSKPlayers((string)Application["BasketballDBConnectionString"]);

		sCmd = (string)e.CommandName;
		sNow = DateTime.Now.ToString("HH:mm:ss");
		try {
			iUpdated = player.Update(sCmd);
			Page_Load(sender,e);
			if(sCmd.Equals("MOD")) {
				if(iUpdated > 0) {
					UpdateHistoryMessage("成功修改" + iUpdated + "個球員(" + sNow + ")");
				}	else if(iUpdated == 0) {
					UpdateHistoryMessage("沒有修改球員(" + sNow + ")");
				} else if(iUpdated == -99) {
					UpdateHistoryMessage("修改失敗，相同球員名稱已存在此隊伍！");
				} else {
					UpdateHistoryMessage((string)Application["transErrorMsg"] + "(" + sNow + ")");
				}
			}	else {
				if(iUpdated > 0) {
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
function onPosChanged(validate_index) {
	re = /^\d{0,2}$/
	re_val = BSKPlayersForm.player_no[validate_index].value.search(re)
	if(re_val == -1) {
		alert('球員號碼只接受數字');
		BSKPlayersForm.player_no[validate_index].value = '';
	}
}
</script>

<html>
<head>
	<META http-equiv="Content-Type" content="text/html; charset=big5">
	<LINK REL="stylesheet" HREF="/sportStyle.css" TYPE="text/css">
</head>
<body>
	<form id="BSKPlayersForm" method="post" runat="server">
		<font size="2"><b>上次行動:</b><asp:Label id="historyMsg" runat="server" /></font><br>

		<table border="1" width="50%" style="font: 10pt verdana">
			<tr style="background-color:#E0FFFF" align="left">
				<th colspan="4">
					<%=sTeam%>球員:
				</th>
			</tr>

			<tr style="background-color:#E0FFFF" align="center">
				<th><font color="#808080">號碼</font></th>
				<th><font color="#808080">位置</font></th>
				<th><font color="#808080">名稱</font></th>
				<th><font color="#808080">國家</font></th>
			</tr>

			<span id="PlayersInformation" runat="server" />

			<tr>
				<td colspan="3" align="right">
					<asp:Button id="ModBtn" Text="儲存" CommandName="MOD" OnCommand="onModifyPlayers" runat="server" />
					&nbsp;<input type="reset" value="重設">
				</td>
				<td align="right">
					<asp:Button id="DelBtn" Text="刪除" CommandName="DEL" OnCommand="onModifyPlayers" runat="server" />
				</td>
			</tr>
		</table>
	</form>
</body>
</html>