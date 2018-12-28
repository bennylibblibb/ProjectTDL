<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">
	int iRecCount;

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

		AnalysisStat matchStat = new AnalysisStat(ConfigurationManager.AppSettings["SoccerDBConnectionString"]);

		try {
			AnalysisStatInformation.InnerHtml = matchStat.GetStat();
			iRecCount = matchStat.NumberOfRecords;
			UpdateHistoryMessage(ConfigurationManager.AppSettings["retrieveInfoMsg"] + "�ɨƼƾ�(" + DateTime.Now.ToString("HH:mm:ss") + ")");
		} catch(NullReferenceException) {
			FormsAuthentication.SignOut();
			UpdateHistoryMessage("<a href=\"/sports/index.htm\" target=\"_top\">" + ConfigurationManager.AppSettings["sessionExpiredMsg"]+ "</a>");
		}
	}

	void onSendMatchStat(Object sender,EventArgs e) {
		int[] iUpdated;
		string sNow;
		AnalysisStat statAnly = new AnalysisStat(ConfigurationManager.AppSettings["SoccerDBConnectionString"]);
		sNow = DateTime.Now.ToString("HH:mm:ss");
		try {
			iUpdated = statAnly.Update();
			Page_Load(sender,e);
			if((iUpdated[0] > 0) || (iUpdated[1] > 0)) {
				UpdateHistoryMessage("���\�ק�" + iUpdated[0] + "�ΧR��" + iUpdated[1] + "���ɨƼƾ�(" + sNow + ")");
			}	else {
				UpdateHistoryMessage("�S���ק��ɨƼƾ�(" + sNow + ")");
			}
		}	catch(NullReferenceException nullex) {
			FormsAuthentication.SignOut();
			UpdateHistoryMessage("<a href=\"/sports/index.htm\" target=\"_top\">" + ConfigurationManager.AppSettings["sessionExpiredMsg"]+ "</a>");
		}
	}
	
	void onSaveMatchStat(Object sender,EventArgs e) {
		int[] iUpdated;
		string sNow;
		AnalysisStat statAnly = new AnalysisStat(ConfigurationManager.AppSettings["SoccerDBConnectionString"]);
		sNow = DateTime.Now.ToString("HH:mm:ss");
		try {
			iUpdated = statAnly.SaveRecord();
			Page_Load(sender,e);
			if (iUpdated[0] > 0) {
				UpdateHistoryMessage("���\�x�s" + iUpdated[0] + "���ɨƼƾ�(" + sNow + ")");
			}	else {
				UpdateHistoryMessage("�S���x�s�Y���ɨƼƾ�(" + sNow + ")");
			}
		}	catch(NullReferenceException nullex) {
			FormsAuthentication.SignOut();
			UpdateHistoryMessage("<a href=\"/sports/index.htm\" target=\"_top\">" + ConfigurationManager.AppSettings["sessionExpiredMsg"]+ "</a>");
		}
	}

	void UpdateHistoryMessage(string sMsg) {
		historyMsg.Text = sMsg;
	}
</script>

<script language="JavaScript">
<%
	if(iRecCount > 1) {
%>
function DeviceCheck() {
	<%
		int iActIdx;
		string sAction;

		for(iActIdx=0;iActIdx<iRecCount;iActIdx++) {
			sAction = "Action[" + iActIdx.ToString() + "]";
	%>
			if(AnalysisStatForm.<%=sAction%>.value == 'D') {
				AnalysisStatForm.SendToPager[0].checked = true;
				AnalysisStatForm.SendToPager[1].checked = true;
				AnalysisStatForm.SendToPager[2].checked = true;
			} else {
				if((AnalysisStatForm.SendToPager[0].checked == true) || (AnalysisStatForm.SendToPager[1].checked == true)) {
					AnalysisStatForm.SendToPager[0].checked = true;
					AnalysisStatForm.SendToPager[1].checked = true;
				}
			}
	<%
		}
	%>
}

function SendAllStat() {
	<%
		int iIdx;
		string sItem;
	%>
	if(AnalysisStatForm.sendall.checked == true) {
		<%
			for(iIdx=0;iIdx<iRecCount;iIdx++) {
				sItem = "send[" + iIdx.ToString() + "]";
		%>
				AnalysisStatForm.<%=sItem%>.checked = true;
		<%
			}
		%>
	}	else {
		<%
			for(iIdx=0;iIdx<iRecCount;iIdx++) {
				sItem = "send[" + iIdx.ToString() + "]";
		%>
				AnalysisStatForm.<%=sItem%>.checked = false;
		<%
			}
		%>
	}
}

function onHostWinChange(index) {
	re = /^\d{1,3}$/
	re_val = AnalysisStatForm.hostwin[index].value.search(re)
	if(re_val == -1) {
		alert('�ʤ��v�u�������');
		AnalysisStatForm.hostwin[index].value = '0';
	}	else {
		AnalysisStatForm.send[index].checked = true;
	}
}

function onHostDrawChange(index) {
	re = /^\d{1,3}$/
	re_val = AnalysisStatForm.hostdraw[index].value.search(re)
	if(re_val == -1) {
		alert('�ʤ��v�u�������');
		AnalysisStatForm.hostdraw[index].value = '0';
	}	else {
		AnalysisStatForm.hostloss[index].value = 100 - AnalysisStatForm.hostwin[index].value - AnalysisStatForm.hostdraw[index].value;
		if(AnalysisStatForm.hostloss[index].value < 0) {
			alert('�ʤ��v����j��100');
			AnalysisStatForm.hostdraw[index].value = '0';
			AnalysisStatForm.hostloss[index].value = '0';
		}	else {
			AnalysisStatForm.send[index].checked = true;
		}
	}
}

function onHostLossChange(index) {
	re = /^\d{1,3}$/
	re_val = AnalysisStatForm.hostloss[index].value.search(re)
	if(re_val == -1) {
		alert('�ʤ��v�u�������');
		AnalysisStatForm.hostloss[index].value = '0';
	}	else {
		AnalysisStatForm.send[index].checked = true;
	}
}

function onGuestWinChange(index) {
	re = /^\d{1,3}$/
	re_val = AnalysisStatForm.guestwin[index].value.search(re)
	if(re_val == -1) {
		alert('�ʤ��v�u�������');
		AnalysisStatForm.guestwin[index].value = '0';
	}	else {
		AnalysisStatForm.send[index].checked = true;
	}
}

function onGuestDrawChange(index) {
	re = /^\d{1,3}$/
	re_val = AnalysisStatForm.guestdraw[index].value.search(re)
	if(re_val == -1) {
		alert('�ʤ��v�u�������');
		AnalysisStatForm.guestdraw[index].value = '0';
	}	else {
		AnalysisStatForm.guestloss[index].value = 100 - AnalysisStatForm.guestwin[index].value - AnalysisStatForm.guestdraw[index].value;
		if(AnalysisStatForm.guestloss[index].value < 0) {
			alert('�ʤ��v����j��100');
			AnalysisStatForm.guestdraw[index].value = '0';
			AnalysisStatForm.guestloss[index].value = '0';
		}	else {
			AnalysisStatForm.send[index].checked = true;
		}
	}
}

function onGuestLossChange(index) {
	re = /^\d{1,3}$/
	re_val = AnalysisStatForm.guestloss[index].value.search(re)
	if(re_val == -1) {
		alert('�ʤ��v�u�������');
		AnalysisStatForm.guestloss[index].value = '0';
	}	else {
		AnalysisStatForm.send[index].checked = true;
	}
}

function actionChange(index) {
	AnalysisStatForm.send[index].checked = true;
	if(AnalysisStatForm.Action[index].value == 'D') {
		AnalysisStatForm.SendToPager[0].checked = true;
		AnalysisStatForm.SendToPager[1].checked = true;
		AnalysisStatForm.SendToPager[2].checked = true;
		//alert('�нT�wGOGO1,GOGO2�ΰ��|���w����I');
	}
}
<%
}
else {
%>
function DeviceCheck() {
	if(AnalysisStatForm.Action.value == 'D') {
		AnalysisStatForm.SendToPager[0].checked = true;
		AnalysisStatForm.SendToPager[1].checked = true;
		AnalysisStatForm.SendToPager[2].checked = true;
	} else {
		if((AnalysisStatForm.SendToPager[0].checked == true) || (AnalysisStatForm.SendToPager[1].checked == true)) {
			AnalysisStatForm.SendToPager[0].checked = true;
			AnalysisStatForm.SendToPager[1].checked = true;
		}
	}
}

function SendAllStat() {
	if(AnalysisStatForm.sendall.checked == true) {
		AnalysisStatForm.send.checked = true;
	}	else {
		AnalysisStatForm.send.checked = false;
	}
}

function onHostWinChange(index) {
	re = /^\d{1,3}$/
	re_val = AnalysisStatForm.hostwin.value.search(re)
	if(re_val == -1) {
		alert('�ʤ��v�u�������');
		AnalysisStatForm.hostwin.value = '0';
	}	else {
		AnalysisStatForm.send.checked = true;
	}
}

function onHostDrawChange(index) {
	re = /^\d{1,3}$/
	re_val = AnalysisStatForm.hostdraw.value.search(re)
	if(re_val == -1) {
		alert('�ʤ��v�u�������');
		AnalysisStatForm.hostdraw.value = '0';
	}	else {
		AnalysisStatForm.hostloss.value = 100 - AnalysisStatForm.hostwin.value - AnalysisStatForm.hostdraw.value;
		if(AnalysisStatForm.hostloss.value < 0) {
			alert('�ʤ��v����j��100');
			AnalysisStatForm.hostdraw.value = '0';
			AnalysisStatForm.hostloss.value = '0';
		}	else {
			AnalysisStatForm.send.checked = true;
		}
	}
}

function onHostLossChange(index) {
	re = /^\d{1,3}$/
	re_val = AnalysisStatForm.hostloss.value.search(re)
	if(re_val == -1) {
		alert('�ʤ��v�u�������');
		AnalysisStatForm.hostloss.value = '0';
	}	else {
		AnalysisStatForm.send.checked = true;
	}
}

function onGuestWinChange(index) {
	re = /^\d{1,3}$/
	re_val = AnalysisStatForm.guestwin.value.search(re)
	if(re_val == -1) {
		alert('�ʤ��v�u�������');
		AnalysisStatForm.guestwin.value = '0';
	}	else {
		AnalysisStatForm.send.checked = true;
	}
}

function onGuestDrawChange(index) {
	re = /^\d{1,3}$/
	re_val = AnalysisStatForm.guestdraw.value.search(re)
	if(re_val == -1) {
		alert('�ʤ��v�u�������');
		AnalysisStatForm.guestdraw.value = '0';
	}	else {
		AnalysisStatForm.guestloss.value = 100 - AnalysisStatForm.guestwin.value - AnalysisStatForm.guestdraw.value;
		if(AnalysisStatForm.guestloss.value < 0) {
			alert('�ʤ��v����j��100');
			AnalysisStatForm.guestdraw.value = '0';
			AnalysisStatForm.guestloss.value = '0';
		}	else {
			AnalysisStatForm.send.checked = true;
		}
	}
}

function onGuestLossChange(index) {
	re = /^\d{1,3}$/
	re_val = AnalysisStatForm.guestloss.value.search(re)
	if(re_val == -1) {
		alert('�ʤ��v�u�������');
		AnalysisStatForm.guestloss.value = '0';
	}	else {
		AnalysisStatForm.send.checked = true;
	}
}

function actionChange(index) {
	AnalysisStatForm.send.checked = true;
	if(AnalysisStatForm.Action.value == 'D') {
		AnalysisStatForm.SendToPager[0].checked = true;
		AnalysisStatForm.SendToPager[1].checked = true;
		AnalysisStatForm.SendToPager[2].checked = true;
		//alert('�нT�wGOGO1,GOGO2�ΰ��|���w����I');
	}
}
<%
	}
%>
</script>

<html>
<head>
     <title>�ƾ�</title>
	<META http-equiv="Content-Type" content="text/html; charset=big5">
	<LINK REL="stylesheet" HREF="/sportStyle.css" TYPE="text/css">
</head>
<body>
	<form id="AnalysisStatForm" method="post" runat="server" onsubmit="DeviceCheck()">
		<font size="2"><b>�W�����:</b><asp:Label id="historyMsg" runat="server" /></font><br>
         	<select name="soccerCorrectScore" onChange="goToCorrectScore(SoccerMenuForm.soccerCorrectScore.value)">
						<option value="0">�п��</option>
						<span id="CorrectScoreInformation" runat="server" />
					</select>
		<table border="1" width="100%" style="font: 10pt verdana">
		<tr style="background-color:#87CEEB" align="center">
			<th>�p��</th>
			<th>�D��</th>
			<th>��</th>
			<th>�M</th>
			<th>�t</th>
			<th>�ȶ�</th>
			<th>��</th>
			<th>�M</th>
			<th>�t</th>
			<th>����ʧ@</th>
			<th>�o�e<br>����<input type="checkbox" name="sendall" value="1" onClick="SendAllStat()"></th>
		</tr>
		<span id="AnalysisStatInformation" runat="server" />
		<tr>
			<th colspan="5" align="left">
				<font color="#FFB573">���O���w�s�b�y�ƾڡz��Ʈw</font><br>
				<font color="red">�b�R����ƮɡA�нT�w�Ҧ��ǩI���w����A�H�קK�ǩI����Ʀ����~�I</font>
			</th>
			<td colspan="5" align="right">
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
				<input type="submit" id="SaveBtn" value="�x�s" OnServerClick="onSaveMatchStat" runat="server">&nbsp;
				<input type="reset" value="���]">
			</td>
			<td align="center">
				<input type="submit" id="SendBtn" value="�o�e" OnServerClick="onSendMatchStat" runat="server">
			</td>
		</table>
	</form>
</body>
</html>