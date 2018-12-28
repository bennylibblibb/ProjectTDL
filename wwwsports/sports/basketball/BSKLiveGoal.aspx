<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">
	int iRecCount;

	void Page_Load(Object sender,EventArgs e) {
		BSKLiveGoal LiveGoalBsk = new BSKLiveGoal((string)Application["BasketballDBConnectionString"]);

		try {
			MatchesInformation.InnerHtml = LiveGoalBsk.GetLiveGoal();
			iRecCount = LiveGoalBsk.NumberOfRecords;
			UpdateHistoryMessage((string)Application["retrieveInfoMsg"] + "現場比數(" + DateTime.Now.ToString("HH:mm:ss") + ")");

		} catch(NullReferenceException) {
			FormsAuthentication.SignOut();
			UpdateHistoryMessage("<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>");
		}
	}

	void GetMatchesAction(Object sender,CommandEventArgs e) {
		int iUpdated = 0;
		string sNow;
		string sCmd;
		BSKLiveGoal matchUpdBsk = new BSKLiveGoal((string)Application["BasketballDBConnectionString"]);

		sCmd = (string)e.CommandName;
		sNow = DateTime.Now.ToString("HH:mm:ss");
		try {
			iUpdated = matchUpdBsk.UpdateLiveGoal(sCmd);
			Page_Load(sender,e);
			if(sCmd.Equals("SEND")) {
				if(iUpdated > 0) {
					UpdateHistoryMessage("成功更新" + iUpdated.ToString() + "現場比數資訊(" + sNow + ")");
				}	else if(iUpdated == 0) {
					UpdateHistoryMessage("沒有更新現場比數(" + sNow + ")");
				}	else {
					UpdateHistoryMessage((string)Application["transErrorMsg"] + "(" + sNow + ")");
				}
			}	else if(sCmd.Equals("SORT")) {
				if(iUpdated > 0) {
					UpdateHistoryMessage("成功更新現場比數序號(" + sNow + ")");
				}	else if(iUpdated == 0) {
					UpdateHistoryMessage("沒有序號更新(" + sNow + ")");
				}	else {
					UpdateHistoryMessage((string)Application["transErrorMsg"] + "(" + sNow + ")");
				}
			}	else if(sCmd.Equals("HIDE")) {
				if(iUpdated > 0) {
					UpdateHistoryMessage("隱藏" + iUpdated.ToString() + "現場比數(" + sNow + ")");
				}	else if(iUpdated == 0) {
					UpdateHistoryMessage("沒有現場比數隱藏(" + sNow + ")");
				}	else {
					UpdateHistoryMessage((string)Application["transErrorMsg"] + "(" + sNow + ")");
				}
			}	else if(sCmd.Equals("SHOW")) {
				if(iUpdated > 0) {
					UpdateHistoryMessage("顯示所有現場比數(" + sNow + ")");
				} else if(iUpdated == 0) {
					UpdateHistoryMessage("沒有現場比數顯示(" + sNow + ")");
				}	else {
					UpdateHistoryMessage((string)Application["transErrorMsg"] + "(" + sNow + ")");
				}
			}
		} catch(NullReferenceException nullex) {
			FormsAuthentication.SignOut();
			UpdateHistoryMessage("<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>");
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
function ClearOrderNo() {
	<%
		for(int iClear=0;iClear<iRecCount;iClear++) {
	%>
		LiveGoalBskForm.orderID[<%=iClear.ToString()%>].value = '';
	<%
		}
	%>
}

function onOrder_IDChanged(validate_index) {
	order_re = /^\d{0,2}$/
	re_val = LiveGoalBskForm.orderID[validate_index].value.search(order_re)
	if(re_val == -1) {
		alert('序號只接受數字');
		LiveGoalBskForm.orderID[validate_index].value = '';
	}
}

function onStatusChanged(validate_index) {
	LiveGoalBskForm.MUSTSendChk[validate_index].checked = true;
}

function onRemarkChanged(validate_index) {
	LiveGoalBskForm.MUSTSendChk[validate_index].checked = true;
}
<%
	int iRecodeIndex,iIndex;
	string sQuHscore,sQuGscore;
%>
<%
	for(iIndex = 1;iIndex <=5 ;iIndex++) {
%>
 function onQuHScore<%=iIndex%>Changed(validate_index) {
	host_re = /^\d{0,3}$/
 <%
		for(iRecodeIndex = 0;iRecodeIndex < iRecCount; iRecodeIndex++) {

		sQuHscore = "QUHSCORE"+iIndex+"[" + iRecodeIndex.ToString() + "]";
%>
	re_val = LiveGoalBskForm.<%=sQuHscore%>.value.search(host_re)
	if(re_val == -1) {
		alert('比數只接受數字');
		LiveGoalBskForm.<%=sQuHscore%>.value = '0';
	}
	else {
		LiveGoalBskForm.MUSTSendChk[validate_index].checked = true;
	}

<%
 	}
%>
 }
<%
	}
%>
<%
	for(iIndex = 1;iIndex <=5 ;iIndex++) {
%>
 function onQuGScore<%=iIndex%>Changed(validate_index) {
	host_re = /^\d{0,3}$/
 <%
		for(iRecodeIndex = 0;iRecodeIndex < iRecCount; iRecodeIndex++) {

		sQuGscore = "QUGSCORE"+iIndex+"[" + iRecodeIndex.ToString() + "]";
%>
	re_val = LiveGoalBskForm.<%=sQuGscore%>.value.search(host_re)
	if(re_val == -1) {
		alert('比數只接受數字');
		LiveGoalBskForm.<%=sQuGscore%>.value = '0';
	}
	else {
		LiveGoalBskForm.MUSTSendChk[validate_index].checked = true;
	}

<%  	}
%>
 }
<%
	}
%>
function selectAll(iType) {
	<%
		int iSendChkIndex;
		string sSendChk_All;
	%>
	if(iType == 0) {
		if(LiveGoalBskForm.SelectAllSend.checked == false) {
			<%
				for(iSendChkIndex=0;iSendChkIndex<iRecCount;iSendChkIndex++) {
					sSendChk_All = "MUSTSendChk[" + iSendChkIndex.ToString() + "]";
			%>
					LiveGoalBskForm.<%=sSendChk_All%>.checked = false;
			<%
				}
			%>
		}
		else {
			if(LiveGoalBskForm.SelectAllSend.checked == true) {
			<%
				for(iSendChkIndex=0;iSendChkIndex<iRecCount;iSendChkIndex++) {
					sSendChk_All = "MUSTSendChk[" + iSendChkIndex.ToString() + "]";
			%>
					LiveGoalBskForm.<%=sSendChk_All%>.checked = true;
			<%
				}
			%>
			}
		}
	}
	else if(iType == 1) {
		if(LiveGoalBskForm.SelectAllHide.checked == true) {
			<%
				for(iSendChkIndex=0;iSendChkIndex<iRecCount;iSendChkIndex++) {
					sSendChk_All = "hiddenChk[" + iSendChkIndex.ToString() + "]";
			%>
					LiveGoalBskForm.<%=sSendChk_All%>.checked = true;
			<%
				}
			%>
		}
		else {
			<%
				for(iSendChkIndex=0;iSendChkIndex<iRecCount;iSendChkIndex++) {
					sSendChk_All = "hiddenChk[" + iSendChkIndex.ToString() + "]";
			%>
					LiveGoalBskForm.<%=sSendChk_All%>.checked = false;
			<%
				}
			%>
		}
	}
}
<%
}
else {
	if(iRecCount > 0) {
%>
<%
	int iIndexNum;
	string sQuHscore,sQuGscore;
	for(iIndexNum = 1;iIndexNum <=5 ;iIndexNum++) {
		sQuHscore =  "QUHSCORE"+iIndexNum;
		sQuGscore =  "QUGSCORE"+iIndexNum;
%>
function onQuHScore<%=iIndexNum%>Changed(validate_index) {
	host_re = /^\d{0,3}$/
	re_val = LiveGoalBskForm.<%=sQuHscore%>.value.search(host_re)
	if(re_val == -1) {
		alert('比數只接受數字');
		LiveGoalBskForm.<%=sQuHscore%>.value = '0';
	}
	else {
		LiveGoalBskForm.MUSTSendChk.checked = true;
	}
}
function onQuGScore<%=iIndexNum%>Changed(validate_index) {
	host_re = /^\d{0,3}$/
	re_val = LiveGoalBskForm.<%=sQuGscore%>.value.search(host_re)
	if(re_val == -1) {
		alert('比數只接受數字');
		LiveGoalBskForm.<%=sQuGscore%>.value = '0';
	}
	else {
		LiveGoalBskForm.MUSTSendChk.checked = true;
	}
}
<%
   }
%>

function ClearOrderNo() {
	LiveGoalBskForm.orderID.value = '';
}

function onOrder_IDChanged(validate_index) {
	order_re = /^\d{0,2}$/
	re_val = LiveGoalBskForm.orderID.value.search(order_re)
	if(re_val == -1) {
		alert('序號只接受數字');
		LiveGoalBskForm.orderID.value = '';
	}
}

function onStatusChanged(validate_index) {
	LiveGoalBskForm.MUSTSendChk.checked = true;
}

function onRemarkChanged(validate_index) {
	LiveGoalBskForm.MUSTSendChk.checked = true;
}

function selectAll(iType) {
	if(iType == 0) {
		if(LiveGoalBskForm.SelectAllSend.checked == true) {
			LiveGoalBskForm.MUSTSendChk.checked = true;
		}
		else {
			LiveGoalBskForm.MUSTSendChk.checked = false;
		}
	}
	else if(iType == 1) {
		if(LiveGoalBskForm.SelectAllHide.checked == true) {
			LiveGoalBskForm.hiddenChk.checked = true;
		}
		else {
			LiveGoalBskForm.hiddenChk.checked = false;
		}
	}
}
<%
	} else {

%>
function selectAll(iType) {
}
function ClearOrderNo() {
}
<%
	}
}
%>

function ReSort(sortType) {
	parent.content_frame.location.replace('BSKLiveGoal.aspx?sort=' + sortType);
}
</script>

<html>
<head>
	<META http-equiv="Content-Type" content="text/html; charset=big5">
	<LINK REL="stylesheet" HREF="/sportStyle.css" TYPE="text/css">
</head>
<body>
	<form id="LiveGoalBskForm" method="post" runat="server">
		<font size="2"><b>上次行動:</b><asp:Label id="historyMsg" runat="server" /></font><br>

		<table border="1" width="100%" style="font:10pt verdana">
		<tr style="background-color:#A8D5C8">
			<th colspan="4" align="left">
				<input type="button" name="clearOrder" value="清除序號" onClick="ClearOrderNo()">&nbsp;&nbsp;
				順序依
				<select name="sortType" onChange="ReSort(LiveGoalBskForm.sortType.value)">
				<%
					if(Session["user_sortType"].ToString().Equals("0")) {
				%>
					<option value="0">聯賽</option>
					<option value="1">序號</option>
				<%
					} else {
				%>
					<option value="1">序號</option>
					<option value="0">聯賽</option>
				<%
					}
				%>
				</select>
			</th>
			<th colspan="4">比數</th>
			<th colspan="4"></th>
			<th>全選<input type="checkbox" name="SelectAllSend" onClick="selectAll(0)"></th>
			<th>全選<input type="checkbox" name="SelectAllHide" onClick="selectAll(1)"></th>
		</tr>

		<tr style="background-color:#A8D5C8">
			<th>排序</th>
			<th>日期</th>
			<th>時間</th>
			<th>聯賽</th>
			<th colspan="2">主隊</th>
			<th colspan="2">客隊</th>
			<th>狀況</th>
			<th>歌曲選擇</th>
			<th>備註</th>
			<th>響機</th>
			<th>強制傳送</th>
			<th>隱藏</th>
		</tr>

		<span id="MatchesInformation" runat="server" />

		<tr>
			<td colspan="13" align="right">
				<!--
					Value of SendToPager is Device ID defined in DEVICE_TYPE
				-->
				<input type="checkbox" name="SendToPager" value="1" checked>GOGO1&nbsp;
				<input type="checkbox" name="SendToPager" value="1" checked>GOGO2&nbsp;
				<input type="checkbox" name="SendToPager" value="3" checked>馬會機&nbsp;
				<!--
				<input type="checkbox" name="SendToPager" value="4" checked>JCCombo&nbsp;
				<input type="checkbox" name="SendToPager" value="5" checked>GOGO3Combo&nbsp;
				-->
				<asp:Button id="SendBtn" Text="傳送" CommandName="SEND" OnCommand="GetMatchesAction" tabindex="499" runat="server" />
			</td>
			<td></td>
		</tr>
		<tr>
			<td colspan="11"><asp:Button id="SortBtn" Text="儲存序號" CommandName="SORT" OnCommand="GetMatchesAction" tabindex="800" runat="server" /></td>
			<td colspan="2" align="right"><asp:Button id="ShowBtn" Text="顯示所有" CommandName="SHOW" OnCommand="GetMatchesAction" runat="server" /></td>
			<td align="center"><asp:Button id="HideBtn" Text="隱藏" CommandName="HIDE" OnCommand="GetMatchesAction" runat="server" /></td>
		</tr>
		</table>
	</form>
</body>
</html>