<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>
<%@ Register TagPrefix="uc1" TagName="MenuTabs" Src="~/UserControl/MenuTabs.ascx" %>
<script language="C#" runat="server">
    void Page_Load(Object sender,EventArgs e) {


        //string sLeagueOption = "";
        string sAnalysisOption = "";
        //SoccerMenuLeague MenuLeague = new SoccerMenuLeague(ConfigurationManager.AppSettings["SoccerDBConnectionString"]);
        SoccerMenuAnalysis MenuAnalysis = new SoccerMenuAnalysis(ConfigurationManager.AppSettings["SoccerDBConnectionString"]);
        try {

           // sLeagueOption = MenuLeague.Show();
            //RankInformation.InnerHtml = sLeagueOption;
            //ScorersInformation.InnerHtml = sLeagueOption;
            //PlayerInformation.InnerHtml = sLeagueOption;
            ////JCComboPlayerInformation.InnerHtml = sLeagueOption;

            sAnalysisOption = MenuAnalysis.Show();
            //AnalysisModifyInformation.InnerHtml = sAnalysisOption;
            CorrectScoreInformation.InnerHtml = sAnalysisOption;
            //AnalysisRecentInformation.InnerHtml = sAnalysisOption;

        } catch(NullReferenceException) {
        }


        AnalysisRecent matchRecent = new AnalysisRecent(ConfigurationManager.AppSettings["SoccerDBConnectionString"]);

        try {
            AnalysisRecentInformation.InnerHtml = matchRecent.GetRecent();
            UpdateHistoryMessage(ConfigurationManager.AppSettings["retrieveInfoMsg"] + "������Z(" + DateTime.Now.ToString("HH:mm:ss") + ")");
        } catch(NullReferenceException) {
            FormsAuthentication.SignOut();
            UpdateHistoryMessage("<a href=\"/sports/index.htm\" target=\"_top\">" + (ConfigurationManager.AppSettings["sessionExpiredMsg"])+ "</a>");
        }
    }

    void onSendRecentAnalysis(Object sender,EventArgs e) {
        int iUpdated = 0;
        string sNow;
        AnalysisRecent recentAnly = new AnalysisRecent(ConfigurationManager.AppSettings["SoccerDBConnectionString"]);

        sNow = DateTime.Now.ToString("HH:mm:ss");
        try {
            iUpdated = recentAnly.Update();
            Page_Load(sender,e);
            if(iUpdated >= 0) {
                UpdateHistoryMessage("���\�קﶤ����Z(" + sNow + ")");
            } else {
                UpdateHistoryMessage("�S���קﶤ����Z(" + sNow + ")");
            }
        }   catch(NullReferenceException nullex) {
            FormsAuthentication.SignOut();
            UpdateHistoryMessage("<a href=\"/sports/index.htm\" target=\"_top\">" + ConfigurationManager.AppSettings["sessionExpiredMsg"]+ "</a>");
        }
    }

    void onSaveRecentAnalysis(Object sender,EventArgs e) {
        int iUpdated = 0;
        string sNow;
        AnalysisRecent recentAnly = new AnalysisRecent(ConfigurationManager.AppSettings["SoccerDBConnectionString"]);

        sNow = DateTime.Now.ToString("HH:mm:ss");
        try {
            iUpdated = recentAnly.SaveRecord();
            Page_Load(sender,e);
            if(iUpdated >= 0) {
                UpdateHistoryMessage("���\�x�s������Z(" + sNow + ")");
            } else {
                UpdateHistoryMessage("�S���x�s������Z(" + sNow + ")");
            }
        }   catch(NullReferenceException nullex) {
            FormsAuthentication.SignOut();
            UpdateHistoryMessage("<a href=\"/sports/index.htm\" target=\"_top\">" + ConfigurationManager.AppSettings["sessionExpiredMsg"]+ "</a>");
        }
    }

    void UpdateHistoryMessage(string sMsg) {
        historyMsg.Text = sMsg;
    }
</script>

<script language="javascript">

    function goToAnalysisRecent(selectedRecentModify) {
        if (selectedRecentModify != '' && selectedRecentModify != '0') {
            window.location.replace('AnalysisRecent.aspx?matchcnt=' + selectedRecentModify);
	}

	AnalysisRecentForm.AnalysisRecentModify.value = '0';
}

function OnActionChanged() {
	if(AnalysisRecentForm.Action.value == 'D') {
		AnalysisRecentForm.SendToPager[0].checked = true;
		AnalysisRecentForm.SendToPager[1].checked = true;
		AnalysisRecentForm.SendToPager[2].checked = true;
		//alert('�нT�wGOGO1,GOGO2�ΰ��|���w����I');
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
		alert('����W�٤���h��8�Ӧ줸');
		document.AnalysisRecentForm.HNextChallenger.value = '';
	}
}

function checkHNextLeagueLength() {
	str = document.AnalysisRecentForm.HNextLeague.value;
	strlen = LengthOfString(str);
	if(strlen > 8) {
		alert('�p�ɦW�٤���h��8�Ӧ줸');
		document.AnalysisRecentForm.HNextLeague.value = '';
	}
}

function checkHRecentChallengerLength(idx) {
	str = document.AnalysisRecentForm.HRecentChallenger[idx].value;
	strlen = LengthOfString(str);
	if(strlen > 8) {
		alert('����W�٤���h��8�Ӧ줸');
		document.AnalysisRecentForm.HRecentChallenger[idx].value = '';
	}
}

function checkHRecentLeagueLength(idx) {
	str = document.AnalysisRecentForm.HRecentLeague[idx].value;
	strlen = LengthOfString(str);
	if(strlen > 8) {
		alert('�p�ɦW�٤���h��8�Ӧ줸');
		document.AnalysisRecentForm.HRecentLeague[idx].value = '';
	}
}

function checkGNextChallengerLength() {
	str = document.AnalysisRecentForm.GNextChallenger.value;
	strlen = LengthOfString(str);
	if(strlen > 8) {
		alert('����W�٤���h��8�Ӧ줸');
		document.AnalysisRecentForm.GNextChallenger.value = '';
	}
}

function checkGNextLeagueLength() {
	str = document.AnalysisRecentForm.GNextLeague.value;
	strlen = LengthOfString(str);
	if(strlen > 8) {
		alert('�p�ɦW�٤���h��8�Ӧ줸');
		document.AnalysisRecentForm.GNextLeague.value = '';
	}
}

function checkGRecentChallengerLength(idx) {
	str = document.AnalysisRecentForm.GRecentChallenger[idx].value;
	strlen = LengthOfString(str);
	if(strlen > 8) {
		alert('����W�٤���h��8�Ӧ줸');
		document.AnalysisRecentForm.GRecentChallenger[idx].value = '';
	}
}

function checkGRecentLeagueLength(idx) {
	str = document.AnalysisRecentForm.GRecentLeague[idx].value;
	strlen = LengthOfString(str);
	if(strlen > 8) {
		alert('�p�ɦW�٤���h��8�Ӧ줸');
		document.AnalysisRecentForm.GRecentLeague[idx].value = '';
	}
}

function checkHRecentHScoreFormat(idx) {
	re = /^\d{0,2}$/
	re_val = document.AnalysisRecentForm.HRecentHScore[idx].value.search(re)
	if(re_val == -1) {
		alert('��ƥu�����Ʀr');
		document.AnalysisRecentForm.HRecentHScore[idx].value = '';
	}
}

function checkHRecentGScoreFormat(idx) {
	re = /^\d{0,2}$/
	re_val = document.AnalysisRecentForm.HRecentGScore[idx].value.search(re)
	if(re_val == -1) {
		alert('��ƥu�����Ʀr');
		document.AnalysisRecentForm.HRecentGScore[idx].value = '';
	}
}

function checkGRecentHScoreFormat(idx) {
	re = /^\d{0,2}$/
	re_val = document.AnalysisRecentForm.GRecentHScore[idx].value.search(re)
	if(re_val == -1) {
		alert('��ƥu�����Ʀr');
		document.AnalysisRecentForm.GRecentHScore[idx].value = '';
	}
}

function checkGRecentGScoreFormat(idx) {
	re = /^\d{0,2}$/
	re_val = document.AnalysisRecentForm.GRecentGScore[idx].value.search(re)
	if(re_val == -1) {
		alert('��ƥu�����Ʀr');
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
     <title>���Z</title>
	<META http-equiv="Content-Type" content="text/html; charset=big5">
	<LINK href="../CentaSmsStyle.css" type="text/css" rel="stylesheet">
    <LINK REL="stylesheet" HREF="/sportStyle.css" TYPE="text/css">
</head>
<body >
	<form id="AnalysisRecentForm" method="post" runat="server" onsubmit="DeviceCheck()">
	   <uc1:menutabs id="MenuTabs1" runat="server" Visible="false" ></uc1:menutabs>
        <font size="2"><b>�W�����:</b><asp:Label id="historyMsg" runat="server" /></font><br>
       	<select name="AnalysisRecentModify" onChange="goToAnalysisRecent(AnalysisRecentForm.AnalysisRecentModify.value)">
						<option value="0">�п��</option>
						<span id="CorrectScoreInformation" runat="server" />
					</select>
				 
					<%--<select name="RankDetails" onChange="goToRank(SoccerMenuForm.RankDetails.value)">
						<option value="0">�п��</option>
						<span id="RankInformation" runat="server" />
					</select>
				 
					<select name="Scorers" onChange="goToScorers(SoccerMenuForm.Scorers.value)">
						<option value="0">�п��</option>
						<span id="ScorersInformation" runat="server" />
					</select>
			 
					<select name="soccerMenuPlayer" onChange="goToTeam(SoccerMenuForm.soccerMenuPlayer.value)">
						<option value="0">�п��</option>
						<span id="PlayerInformation" runat="server" />
					</select>
				  
				 
					<select name="soccerMenuAnalysisModify" onChange="goToAnalysisModify(SoccerMenuForm.soccerMenuAnalysisModify.value)">
						<option value="0">�п��</option>
						<span id="AnalysisModifyInformation" runat="server" />
					</select>
				 
					<select name="AnalysisRecentModify" onChange="goToAnalysisRecent(SoccerMenuForm.AnalysisRecentModify.value)">
						<option value="0">�п��</option>
						<span id="Span1" runat="server" />
					</select>--%>
		<table border="1" width="100%" style="font: 10pt verdana">
		<span id="AnalysisRecentInformation" runat="server" />
		<tr>
			<th colspan="2" align="left">
				<font color="red">�b�R����ƮɡA�нT�w�Ҧ��ǩI���w����A�H�קK�ǩI����Ʀ����~�I</font>
			</th>
			<td align="right">
				<!--
					Value of SendToPager is Device ID defined in DEVICE_TYPE
				-->
				<input type="checkbox" name="SendToPager" value="1" checked>GOGO1&nbsp;
				<input type="checkbox" name="SendToPager" value="2" checked>GOGO2&nbsp;
				<input type="checkbox" name="SendToPager" value="3" checked>���|��&nbsp;
				<input type="checkbox" name="SendToPager" value="4" checked>JCCombo&nbsp;
				<!--
				<input type="checkbox" name="SendToPager" value="5" checked>GOGO3Combo&nbsp;
				-->
				<input type="submit" id="SaveBtn" value="�x�s" OnServerClick="onSaveRecentAnalysis" runat="server">&nbsp;
				<input type="reset" value="���]">
				<input type="submit" id="SendBtn" value="�o�e" OnServerClick="onSendRecentAnalysis" runat="server">
			</td>
		</table>
	</form>
</body>
</html>