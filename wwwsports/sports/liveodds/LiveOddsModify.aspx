<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">
	int iRecCount;
	string sCurrentTime;
	string sRegion;

	void Page_Load(Object sender,EventArgs e) {
		LiveOddsModify liveoddsInfo = new LiveOddsModify((string)Application["SoccerDBConnectionString"]);

		try {
			LiveOddsInformation.InnerHtml = liveoddsInfo.GetLiveOdds();
			iRecCount = liveoddsInfo.NumberOfRecords;
			sRegion = liveoddsInfo.RegionName;
			UpdateHistoryMessage((string)Application["retrieveInfoMsg"] + "現場賠率(" + DateTime.Now.ToString("HH:mm:ss") + ")");
			sCurrentTime = liveoddsInfo.CurrentTime;
		} catch(NullReferenceException) {
			FormsAuthentication.SignOut();
			UpdateHistoryMessage("<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>");
		}
	}

	void ModifyLiveOdds(Object sender,CommandEventArgs e) {
		int iUpdated = 0;
		string sNow, sCmd;
		LiveOddsModify liveoddsInfo = new LiveOddsModify((string)Application["SoccerDBConnectionString"]);
		sNow = DateTime.Now.ToString("HH:mm:ss");
		sCmd = (string)e.CommandName;
		try {
			if(sCmd.Equals("SEND")) {
				iUpdated = liveoddsInfo.UpdateOdds();
				Page_Load(sender,e);
				if(iUpdated > 0) {
					UpdateHistoryMessage("成功更新" + iUpdated.ToString() + "場現場賠率(" + sNow + ")");
				}	else if(iUpdated == 0) {
					UpdateHistoryMessage("沒有現場賠率可更新(" + sNow + ")");
				}	else {
					UpdateHistoryMessage((string)Application["transErrorMsg"] + "(" + sNow + ")");
				}
			} else if(sCmd.Equals("RENEW")) {
				iUpdated = liveoddsInfo.RenewMatchTimeGone();
				Page_Load(sender,e);
				if(iUpdated > 0) {
					UpdateHistoryMessage("成功更新" + iUpdated.ToString() + "場賽事時間(" + sNow + ")");
				}	else if(iUpdated == 0) {
					UpdateHistoryMessage("沒有賽事時間可更新(" + sNow + ")");
				}	else {
					UpdateHistoryMessage((string)Application["transErrorMsg"] + "(" + sNow + ")");
				}
			} else if(sCmd.Equals("SORT")) {
				iUpdated = liveoddsInfo.Sort();
				Page_Load(sender,e);
				if(iUpdated > 0) {
					UpdateHistoryMessage("成功更新" + iUpdated.ToString() + "場序號(" + sNow + ")");
				}	else if(iUpdated == 0) {
					UpdateHistoryMessage("沒有序號可更新(" + sNow + ")");
				}	else {
					UpdateHistoryMessage((string)Application["transErrorMsg"] + "(" + sNow + ")");
				}
			} else {
				Page_Load(sender,e);
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
function DeviceCheck() {
	if(LiveOddsModifyForm.SendToPager[1].checked == true) {
		LiveOddsModifyForm.SendToPager[0].checked = true;
	}
}

<%
	if(iRecCount > 1) {
%>
function ClearOrderNo() {
	<%
		for(int iClear=0;iClear<iRecCount;iClear++) {
	%>
		LiveOddsModifyForm.orderID[<%=iClear.ToString()%>].value = '';
	<%
		}
	%>
}

function onChangeSent(validate_index) {
	LiveOddsModifyForm.mustsend[validate_index].checked = true;
}

function orderID_Changed(validate_index) {
	orderID_re = /^\d{0,3}$/
	re_val = LiveOddsModifyForm.orderID[validate_index].value.search(orderID_re)
	if(re_val == -1) {
		alert('序號只接受數字');
		LiveOddsModifyForm.orderID[validate_index].value = '';
	}
	LiveOddsModifyForm.mustsend[validate_index].checked = true;
}

function onScrChange(validate_index) {
	orderID_re = /^\d{1,2}$/
	re_val = LiveOddsModifyForm.hostscore[validate_index].value.search(orderID_re)
	if(re_val == -1) {
		alert('比分只接受數字');
		LiveOddsModifyForm.hostscore[validate_index].value = '0';
	}
	re_guestval = LiveOddsModifyForm.guestscore[validate_index].value.search(orderID_re)
	if(re_guestval == -1) {
		alert('比分只接受數字');
		LiveOddsModifyForm.guestscore[validate_index].value = '0';
	}
	LiveOddsModifyForm.mustsend[validate_index].checked = true;
}

function onChangeTime(validate_index) {
	matchtime_re = /^\d{4}$/
	re_val = LiveOddsModifyForm.matchtime[validate_index].value.search(matchtime_re)
	if(re_val == -1) {
		alert('開賽時間必需是4位數字');
		LiveOddsModifyForm.matchtime[validate_index].value = '0000';
	}
	LiveOddsModifyForm.mustsend[validate_index].checked = true;
}

function onChangeDate(validate_index, currDate) {
	matchdate_re = /^\d{8}$/
	re_val = LiveOddsModifyForm.matchdate[validate_index].value.search(matchdate_re)
	if(re_val == -1) {
		alert('開賽日期必需是8位數字');
		LiveOddsModifyForm.matchdate[validate_index].value = currDate;
	}
	LiveOddsModifyForm.mustsend[validate_index].checked = true;
}

function onChangeOdds(validate_index) {
	if(LiveOddsModifyForm.odds[validate_index].value != '') {
		if(isNaN(LiveOddsModifyForm.odds[validate_index].value)) {
			LiveOddsModifyForm.odds[validate_index].value = '';
			alert('不正確賠率數字');
		} else {
			LiveOddsModifyForm.mustsend[validate_index].checked = true;
		}
	}	else {
		LiveOddsModifyForm.mustsend[validate_index].checked = true;
	}
}

function onChangeBigOdds(validate_index) {
	if(LiveOddsModifyForm.bigodds[validate_index].value != '') {
		if(isNaN(LiveOddsModifyForm.bigodds[validate_index].value)) {
			LiveOddsModifyForm.bigodds[validate_index].value = '';
			alert('不正確賠率數字');
		} else {
			LiveOddsModifyForm.mustsend[validate_index].checked = true;
		}
	}	else {
		LiveOddsModifyForm.mustsend[validate_index].checked = true;
	}
}

function selectAll() {
	<%
		int iSendChkIndex;
		string sSendChk_All;
	%>
		if(LiveOddsModifyForm.SelectAllSend.checked == true) {
			<%
				for(iSendChkIndex=0;iSendChkIndex<iRecCount;iSendChkIndex++) {
					sSendChk_All = "mustsend[" + iSendChkIndex.ToString() + "]";
			%>
					LiveOddsModifyForm.<%=sSendChk_All%>.checked = true;
			<%
				}
			%>
		}	else {
			<%
				for(iSendChkIndex=0;iSendChkIndex<iRecCount;iSendChkIndex++) {
					sSendChk_All = "mustsend[" + iSendChkIndex.ToString() + "]";
			%>
					LiveOddsModifyForm.<%=sSendChk_All%>.checked = false;
			<%
				}
			%>
		}
}
<%
	} else {
%>
function ClearOrderNo() {
	LiveOddsModifyForm.orderID.value = '';
}

function onChangeSent(validate_index) {
	LiveOddsModifyForm.mustsend.checked = true;
}

function orderID_Changed(validate_index) {
	orderID_re = /^\d{0,3}$/
	re_val = LiveOddsModifyForm.orderID.value.search(orderID_re)
	if(re_val == -1) {
		alert('序號只接受數字');
		LiveOddsModifyForm.orderID.value = '';
	}
	LiveOddsModifyForm.mustsend.checked = true;
}
function onScrChange(validate_index) {
	orderID_re = /^\d{1,2}$/
	re_val = LiveOddsModifyForm.hostscore.value.search(orderID_re)
	if(re_val == -1) {
		alert('比分只接受數字');
		LiveOddsModifyForm.hostscore.value = '0';
	}
	re_guestval = LiveOddsModifyForm.guestscore.value.search(orderID_re)
	if(re_guestval == -1) {
		alert('比分只接受數字');
		LiveOddsModifyForm.guestscore.value = '0';
	}
	LiveOddsModifyForm.mustsend.checked = true;
}

function onChangeTime(validate_index) {
	matchtime_re = /^\d{4}$/
	re_val = LiveOddsModifyForm.matchtime.value.search(matchtime_re)
	if(re_val == -1) {
		alert('開賽時間必需是4位數字');
		LiveOddsModifyForm.matchtime.value = '';
	}
	LiveOddsModifyForm.mustsend.checked = true;
}

function onChangeDate(validate_index, currDate) {
	matchdate_re = /^\d{8}$/
	re_val = LiveOddsModifyForm.matchdate.value.search(matchdate_re)
	if(re_val == -1) {
		alert('開賽日期必需是8位數字');
		LiveOddsModifyForm.matchdate.value = currDate;
	}
	LiveOddsModifyForm.mustsend.checked = true;
}

function onChangeOdds(validate_index) {
	if(LiveOddsModifyForm.odds.value != '') {
		if(isNaN(LiveOddsModifyForm.odds.value)) {
			LiveOddsModifyForm.odds.value = '';
			alert('不正確賠率數字');
		} else {
			LiveOddsModifyForm.mustsend.checked = true;
		}
	}	else {
		LiveOddsModifyForm.mustsend.checked = true;
	}
}

function onChangeBigOdds(validate_index) {
	if(LiveOddsModifyForm.bigodds.value != '') {
		if(isNaN(LiveOddsModifyForm.bigodds.value)) {
			LiveOddsModifyForm.bigodds.value = '';
			alert('不正確賠率數字');
		} else {
			LiveOddsModifyForm.mustsend.checked = true;
		}
	}	else {
		LiveOddsModifyForm.mustsend.checked = true;
	}
}

function selectAll() {
	if(LiveOddsModifyForm.SelectAllSend.checked == true) {
		LiveOddsModifyForm.mustsend.checked = true;
	}	else {
		LiveOddsModifyForm.mustsend.checked = false;
	}
}
<%
	}
%>
</script>

<html>
<head>
	<META http-equiv="Content-Type" content="text/html; charset=big5">
	<LINK REL="stylesheet" HREF="/sportStyle.css" TYPE="text/css">
</head>
<body>
	<form id="LiveOddsModifyForm" method="post" runat="server" onsubmit="DeviceCheck()">
		<h2><%=sRegion%>現場賠率</h2>
		<font size="2"><b>上次行動:</b><asp:Label id="historyMsg" runat="server" /></font><br>

		<table border="1" width="100%" style="font: 10pt verdana">
		<tr style="background-color:#D2B48C">
			<th colspan="7" align="left">
				<input type="button" name="clearOrder" value="清除序號" onClick="ClearOrderNo()">&nbsp;&nbsp;
			</th>
			<th colspan="2" align="center">亞洲盤</th>
			<th colspan="2" align="center">大小盤</th>
			<th colspan="2" align="center"></th>
			<th>全選<input type="checkbox" name="SelectAllSend" onClick="selectAll()"></th>
		</tr>
		<tr style="background-color:#D2B48C" align="center">
			<th>排序</th>
			<th>主隊<br>名稱&nbsp;/&nbsp;比數</th>
			<th>客隊<br>名稱&nbsp;/&nbsp;比數</th>
			<th>主隊<br>讓賽</th>
			<th>開賽時間<br><i>yyyyMMdd</i><br><i>hhmm</i></th>
			<th>時段</th>
			<th>已進行<br>分鐘</th>
			<th>讓球</th>
			<th>賠率</th>
			<th>總入球</th>
			<th>賠率</th>
			<th>狀況</th>
			<th>響機</th>
			<th>強制<br>傳送</th>
		</tr>
		<tr>
		<span id="LiveOddsInformation" runat="server" />
		</tr>
		<tr align="right">
			<th colspan="6" align="right">
				現在系統時間：<%=sCurrentTime%>
			</th>
			<td colspan="8">
				<!--
					Value of SendToPager is Device ID defined in DEVICE_TYPE
				-->
				<input type="checkbox" name="SendToPager" value="1" checked>GOGO1&nbsp;
				<input type="checkbox" name="SendToPager" value="2" checked>GOGO2&nbsp;
				<input type="checkbox" name="SendToPager" value="5" checked>GOGO3Combo&nbsp;
				<asp:Button id="SendBtn" Text="傳送" CommandName="SEND" OnCommand="ModifyLiveOdds" runat="server" />&nbsp;
				<input type=reset value="重設">
			</td>
		</tr>
		<tr>
			<td colspan="4"><asp:Button id="SortBtn" Text="儲存序號" CommandName="SORT" tabindex="500" OnCommand="ModifyLiveOdds" runat="server" /><font color="red">(<b>不會</b>發送到傳呼機)</font></td>
			<td colspan="10"><asp:Button id="RenewBtn" Text="更新版面" CommandName="RENEW" OnCommand="ModifyLiveOdds" runat="server" /><font color="red">(數據<b>不會</b>發送到傳呼機)</font></td>
		</tr>

	</form>
</body>
</html>