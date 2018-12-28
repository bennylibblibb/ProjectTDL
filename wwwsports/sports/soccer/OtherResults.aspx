<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">
	int iRecCount;

	void Page_Load(Object sender,EventArgs e) {
		OtherResults otherresult = new OtherResults((string)Application["SoccerDBConnectionString"]);

		try {
			othresultInformation.InnerHtml = otherresult.GetOtherResults();
			iRecCount = otherresult.NumberOfRecords;
			leaguename.Text = otherresult.GetLeaglong;
			UpdateHistoryMessage((string)Application["retrieveInfoMsg"] + "��L���(" + DateTime.Now.ToString("HH:mm:ss") + ")");
		} catch(NullReferenceException) {
			FormsAuthentication.SignOut();
			UpdateHistoryMessage("<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>");
		}
	}

	void modifyOtherResults(Object sender,CommandEventArgs e) {
		int iUpdated = 0;
		string sNow;
		OtherResults otherresult = new OtherResults((string)Application["SoccerDBConnectionString"]);

		sNow = DateTime.Now.ToString("HH:mm:ss");
		try {
			iUpdated = otherresult.SendOthersResult();
			Page_Load(sender,e);
			if(iUpdated > 0) {
				UpdateHistoryMessage("���\�o�e" + iUpdated.ToString() + "��L���(" + sNow + ")");
			}	else if(iUpdated == 0) {
				UpdateHistoryMessage("�S���o�e���");
			} else {
				UpdateHistoryMessage("�o�e��ƥ���");
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
function ActionChange(validate_index) {
	OtherResModifyForm.send[validate_index].checked = true;
}

function alertClicked(validate_index) {
	OtherResModifyForm.send[validate_index].checked = true;
}

function onMatchDateChanged(validate_index){
	matchdate_re = /^\d{8}$/
	re_val = OtherResModifyForm.MatchDate[validate_index].value.search(matchdate_re)
	if(re_val == -1) {
		alert('�}�ɤ�����ݬO8��Ʀr');
		OtherResModifyForm.MatchDate[validate_index].value = '';
	} else {
		OtherResModifyForm.send[validate_index].checked = true;
	}
}

function onMatchTimeChanged(validate_index){
	matchtime_re = /^\d{4}$/
	re_val = OtherResModifyForm.MatchTime[validate_index].value.search(matchtime_re)
	if(re_val == -1) {
		alert('�}�ɮɶ����ݬO4��Ʀr');
		OtherResModifyForm.MatchTime[validate_index].value = '';
	} else {
		OtherResModifyForm.send[validate_index].checked = true;
	}
}

function onStatusChanged(validate_index) {
	OtherResModifyForm.send[validate_index].checked = true;
}

function onHostChanged(validate_index){
	OtherResModifyForm.send[validate_index].checked = true;
}

function onGuestChanged(validate_index){
	OtherResModifyForm.send[validate_index].checked = true;
}

function onHscrChanged(validate_index){
	hscrID_re = /^\d{1,2}$/
	re_val = OtherResModifyForm.h_score[validate_index].value.search(hscrID_re)
	if(re_val == -1) {
		alert('����u�����Ʀr');
		OtherResModifyForm.h_score[validate_index].value = '';
	}
	OtherResModifyForm.send[validate_index].checked = true;
}

function onGscrChanged(validate_index){
	hscrID_re = /^\d{1,2}$/
	re_val = OtherResModifyForm.g_score[validate_index].value.search(hscrID_re)
	if(re_val == -1) {
		alert('����u�����Ʀr');
		OtherResModifyForm.g_score[validate_index].value = '';
	}
	OtherResModifyForm.send[validate_index].checked = true;
}

function CheckRes() {
	<%
		int iSendChkIndex;
		string sSend,sMatchTime,sMatchDate,sHostName,sGuestName,sAction;	
		for(iSendChkIndex=0;iSendChkIndex<10;iSendChkIndex++) {
			sSend = "send[" + iSendChkIndex.ToString() + "]";
			sMatchTime = "MatchTime["+iSendChkIndex.ToString() + "]";
			sMatchDate = "MatchDate["+iSendChkIndex.ToString() + "]";
			sHostName = "host["+iSendChkIndex.ToString() + "]";
			sGuestName = "guest["+iSendChkIndex.ToString() + "]";
			sAction = "act["+iSendChkIndex.ToString() + "]";
	%>
	if(OtherResModifyForm.<%=sAction%>.value == 'D') {
		OtherResModifyForm.SendToPager[0].checked = true;
		OtherResModifyForm.SendToPager[1].checked = true;
	}

	if(OtherResModifyForm.<%=sSend%>.checked==true) {
		if(OtherResModifyForm.<%= sMatchDate%>.value == '') {
			alert('�}�ɤ�����ݬO8��Ʀr');
			return false;
		} else {		
			matchdate_re = /^\d{8}$/
			re_val = OtherResModifyForm.<%= sMatchDate%>.value.search(matchdate_re)
			if(re_val == -1) {
				alert('�}�ɤ�����ݬO8��Ʀr');
				return false;
			}
		}
		if(OtherResModifyForm.<%= sMatchTime%>.value == '') {
			alert('�}�ɮɶ����ݬO4��Ʀr');
			return false;
		} else {
			matchtime_re = /^\d{4}$/
			re_val = OtherResModifyForm.<%= sMatchTime%>.value.search(matchtime_re)
			if(re_val == -1) {
				alert('�}�ɮɶ����ݬO4��Ʀr');
				OtherResModifyForm.MatchTime[validate_index].value = '';
			}				
		}
		if(OtherResModifyForm.<%= sHostName%>.value==OtherResModifyForm.<%= sGuestName%>.value) {
			alert('�п�ܤ��P�y��');
			return false;
		}
		if((OtherResModifyForm.<%= sHostName%>.value =='')||(OtherResModifyForm.<%= sGuestName%>.value=='')) {
			alert('�п�ܲy��');
			return false;
		}
	}
	<%  
	}
	%>
	else {
		return true;	
	}
	
}
</script>
<html>
<head>
	<META http-equiv="Content-Type" content="text/html; charset=big5">
	<LINK REL="stylesheet" HREF="/sportStyle.css" TYPE="text/css">
</head>
<body>
	<form id="OtherResModifyForm" method="post" runat="server" ONSUBMIT="return CheckRes()" >
		<font size="2"><b>�W�����:</b><asp:Label id="historyMsg" runat="server" /></font><br>

		<table border="1" width="65%" style="font: 10pt verdana">
		<tr style="color:#FDF5E6; background-color:#a2B436">
			<th colspan="8"><asp:Label id="leaguename" runat="server" />��L���</th>
		</tr>
		<tr style="color:#FDF5E6; background-color:#a2B436" align=center>
			<th>����ʧ@</th>
			<th>���<br>yyyyMMdd</th>
			<th>�ɶ�<br>hhmm</th>
			<th>�ɬq</th>
			<th>�D��-���</th>
			<th>�ȶ�-���</th>
			<th>�T��</th>
			<th>�o�e</th>
		</tr>
		<span id="othresultInformation" runat="server" />
		<tr>
			<td colspan="8" align="right">
				<!--
					Value of SendToPager is Device ID defined in DEVICE_TYPE
				-->
				<input type="checkbox" name="SendToPager" value="1" checked>GOGO1&nbsp;
				<input type="checkbox" name="SendToPager" value="1" checked>GOGO2&nbsp;
				<input type="checkbox" name="SendToPager" value="5" checked>GOGO3Combo&nbsp;
				<asp:Button id="ModBtn" Text="�ǰe" CommandName="MOD" OnCommand="modifyOtherResults" runat="server" />				
			</td>
			
		</table>
	</form>
</body>
</html>