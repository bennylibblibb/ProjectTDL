<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>
<%--<%@ Register TagPrefix="uc1" TagName="MenuTabs" Src="~/UserControl/MenuTabs.ascx" %>--%>
<script language="C#" runat="server">
    int iRecCount;

    void Page_Load(Object sender,EventArgs e) {

        ////         string sLeagueOption = ""; 
        ////SoccerMenuLeague MenuLeague = new SoccerMenuLeague(ConfigurationManager.AppSettings["SoccerDBConnectionString"]);
        //// try {
        ////	sLeagueOption = MenuLeague.Show();

        ////	PlayerInformation.InnerHtml = sLeagueOption;
        ////} catch(NullReferenceException) {
        //// }


        Players playerinfo = new Players(ConfigurationManager.AppSettings["SoccerDBConnectionString"]);

        try {
            PlayersInformation.InnerHtml = playerinfo.GetTeamPlayers();
            iRecCount = playerinfo.NumberOfRecords;
            UpdateHistoryMessage(ConfigurationManager.AppSettings["retrieveInfoMsg"] + "球員名單(" + DateTime.Now.ToString("HH:mm:ss") + ")");
            this.Title =playerinfo.m_Title;
        } catch(NullReferenceException) {
            //FormsAuthentication.SignOut();
            UpdateHistoryMessage("<a href=\"/sports/index.htm\" target=\"_top\">" + ConfigurationManager.AppSettings["sessionExpiredMsg"]+ "</a>");
        }
    }

    void onModifyPlayers(Object sender,CommandEventArgs e) {
        int iUpdated = 0;
        string sNow, sCmd;
        Players modifyPlayer = new Players(ConfigurationManager.AppSettings["SoccerDBConnectionString"]);

        sCmd = (string)e.CommandName;
        sNow = DateTime.Now.ToString("HH:mm:ss");
        try {
            iUpdated = modifyPlayer.Update(sCmd);
            Page_Load(sender,e);
            if(sCmd.Equals("MOD")) {
                if(iUpdated > 0) {
                    UpdateHistoryMessage("成功修改" + iUpdated + "個球員(" + sNow + ")");
                }   else if(iUpdated == 0) {
                    UpdateHistoryMessage("沒有修改球員(" + sNow + ")");
                } else if(iUpdated == -99) {
                    UpdateHistoryMessage("修改失敗，相同球員名稱已存在此隊伍！");
                } else {
                    UpdateHistoryMessage(ConfigurationManager.AppSettings["transErrorMsg"] + "(" + sNow + ")");
                }
            }   else {
                if(iUpdated > 0) {
                    UpdateHistoryMessage("成功刪除" + iUpdated + "個球員(" + sNow + ")");
                }   else if(iUpdated == 0) {
                    UpdateHistoryMessage("沒有刪除球員(" + sNow + ")");
                } else {
                    UpdateHistoryMessage(ConfigurationManager.AppSettings["transErrorMsg"] + "(" + sNow + ")");
                }
            }
        }   catch(NullReferenceException nullex) {
            //FormsAuthentication.SignOut();
            UpdateHistoryMessage("<a href=\"/sports/index.htm\" target=\"_top\">" + ConfigurationManager.AppSettings["sessionExpiredMsg"]+ "</a>");
        }
    }

    void UpdateHistoryMessage(string sMsg) {
        historyMsg.Text = sMsg;
    }
</script>

<script language="JavaScript">

    
function goToTeam(selectedTeam) {
    if (selectedTeam != '' && selectedTeam != '0') {
        window.location.replace('PlayersRetrieval.aspx?teamID=000&leagID=' + selectedTeam);
	} 
	PlayersForm.soccerMenuPlayer.value = '0';
    }


function changeTeam(selectedTeam, selectedLeague) {
	//parent.content_frame.location.replace('PlayersRetrieval.aspx?teamID=' + selectedTeam + '&leagID=' + selectedLeague);
    window.location.replace('PlayersRetrieval.aspx?teamID=' + selectedTeam + '&eventid=' + selectedLeague);
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
</script>

<html>
<head runat="server">
	<META http-equiv="Content-Type" content="text/html; charset=big5">
	<LINK REL="stylesheet" HREF="/sportStyle.css" TYPE="text/css">
     <LINK href="../CentaSmsStyle.css" type="text/css" rel="stylesheet">
      <title>陣容</title>
</head>
<body>
	<form id="PlayersForm" method="post" runat="server">
           <%-- <uc1:menutabs id="MenuTabs1" runat="server"   ></uc1:menutabs>--%>
		<font size="2"><b>上次行動:</b><asp:Label id="historyMsg" runat="server" /></font><br>
      <%--<select name="soccerMenuPlayer" onChange="goToTeam(PlayersForm.soccerMenuPlayer.value)">
						<option value="0">請選擇</option>
						<span id="PlayerInformation" runat="server" />
					</select>--%>
		<table border="1" width="80%" style="font: 10pt verdana">
		<span id="PlayersInformation" runat="server" />
		<tr>
			<td colspan="6" align="right">
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