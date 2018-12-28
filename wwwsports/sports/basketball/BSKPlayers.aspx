<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">
	string sTeam;

	void Page_Load(Object sender,EventArgs e) {
		BSKPlayers players = new BSKPlayers((string)Application["BasketballDBConnectionString"]);

		try {
			PlayersInformation.InnerHtml = players.GetTeamPlayers();
			sTeam = players.Team;
			UpdateHistoryMessage((string)Application["retrieveInfoMsg"] + "�y���W��(" + DateTime.Now.ToString("HH:mm:ss") + ")");
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
					UpdateHistoryMessage("���\�ק�" + iUpdated + "�Ӳy��(" + sNow + ")");
				}	else if(iUpdated == 0) {
					UpdateHistoryMessage("�S���ק�y��(" + sNow + ")");
				} else if(iUpdated == -99) {
					UpdateHistoryMessage("�ק異�ѡA�ۦP�y���W�٤w�s�b������I");
				} else {
					UpdateHistoryMessage((string)Application["transErrorMsg"] + "(" + sNow + ")");
				}
			}	else {
				if(iUpdated > 0) {
					UpdateHistoryMessage("���\�R��" + iUpdated + "�Ӳy��(" + sNow + ")");
				}	else if(iUpdated == 0) {
					UpdateHistoryMessage("�S���R���y��(" + sNow + ")");
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
		alert('�y�����X�u�����Ʀr');
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
		<font size="2"><b>�W�����:</b><asp:Label id="historyMsg" runat="server" /></font><br>

		<table border="1" width="50%" style="font: 10pt verdana">
			<tr style="background-color:#E0FFFF" align="left">
				<th colspan="4">
					<%=sTeam%>�y��:
				</th>
			</tr>

			<tr style="background-color:#E0FFFF" align="center">
				<th><font color="#808080">���X</font></th>
				<th><font color="#808080">��m</font></th>
				<th><font color="#808080">�W��</font></th>
				<th><font color="#808080">��a</font></th>
			</tr>

			<span id="PlayersInformation" runat="server" />

			<tr>
				<td colspan="3" align="right">
					<asp:Button id="ModBtn" Text="�x�s" CommandName="MOD" OnCommand="onModifyPlayers" runat="server" />
					&nbsp;<input type="reset" value="���]">
				</td>
				<td align="right">
					<asp:Button id="DelBtn" Text="�R��" CommandName="DEL" OnCommand="onModifyPlayers" runat="server" />
				</td>
			</tr>
		</table>
	</form>
</body>
</html>