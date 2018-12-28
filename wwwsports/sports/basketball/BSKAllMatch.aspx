<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">
	int iRecCount;

	void Page_Load(Object sender,EventArgs e) {
		BSKAllMatch matchInfoBsk = new BSKAllMatch((string)Application["BasketballDBConnectionString"]);

		try {
			MatchesInformation.InnerHtml = matchInfoBsk.GetAllMatches();
			iRecCount = matchInfoBsk.NumberOfRecords;
			UpdateHistoryMessage((string)Application["retrieveInfoMsg"] + "所有賽事(" + DateTime.Now.ToString("HH:mm:ss") + ")");

		} catch(NullReferenceException) {
			FormsAuthentication.SignOut();
			UpdateHistoryMessage("<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>");
		}
	}

	void GetMatches(Object sender,CommandEventArgs e) {
		int iUpdated = 0;
		string sNow;
		string sCmd;
		BSKAllMatch bsk = new BSKAllMatch((string)Application["BasketballDBConnectionString"]);

		sCmd = (string)e.CommandName;
		sNow = DateTime.Now.ToString("HH:mm:ss");
		try {
			iUpdated = bsk.Update(sCmd);
			Page_Load(sender,e);
			if(sCmd.Equals("SEND")) {
				if(iUpdated > 0) {
					UpdateHistoryMessage("成功更新" + iUpdated.ToString() + "場賽事資訊(" + sNow + ")");
				}	else if(iUpdated == 0) {
					UpdateHistoryMessage("沒有更新賽事(" + sNow + ")");
				}	else {
					UpdateHistoryMessage((string)Application["transErrorMsg"] + "(" + sNow + ")");
				}
			}	else if(sCmd.Equals("SORT")) {
				if(iUpdated > 0) {
					UpdateHistoryMessage("成功更新賽事序號(" + sNow + ")");
				}	else if(iUpdated == 0) {
					UpdateHistoryMessage("沒有序號更新(" + sNow + ")");
				}	else {
					UpdateHistoryMessage((string)Application["transErrorMsg"] + "(" + sNow + ")");
				}
			}	else if(sCmd.Equals("HIDE")) {
				if(iUpdated > 0) {
					UpdateHistoryMessage("隱藏" + iUpdated.ToString() + "場賽事(" + sNow + ")");
				}	else if(iUpdated == 0) {
					UpdateHistoryMessage("沒有賽事隱藏(" + sNow + ")");
				}	else {
					UpdateHistoryMessage((string)Application["transErrorMsg"] + "(" + sNow + ")");
				}
			}	else if(sCmd.Equals("SHOW")) {
				if(iUpdated > 0) {
					UpdateHistoryMessage("顯示所有賽事(" + sNow + ")");
				} else if(iUpdated == 0) {
					UpdateHistoryMessage("沒有賽事顯示(" + sNow + ")");
				}	else {
					UpdateHistoryMessage((string)Application["transErrorMsg"] + "(" + sNow + ")");
				}
			}
		} catch(NullReferenceException nullex) {
			FormsAuthentication.SignOut();
			UpdateHistoryMessage("<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>" + nullex.ToString());
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
		BSKAllMatchForm.orderID[<%=iClear.ToString()%>].value = '';
	<%
		}
	%>
}

function onOrder_IDChanged(validate_index) {
	order_re = /^\d{0,2}$/
	re_val = BSKAllMatchForm.orderID[validate_index].value.search(order_re)
	if(re_val == -1) {
		alert('序號只接受數字');
		BSKAllMatchForm.orderID[validate_index].value = '';
	}
}

function onStatusChanged(validate_index) {
	BSKAllMatchForm.MUSTSendChk[validate_index].checked = true;
}

function onScore1Changed(validate_index) {
	if(BSKAllMatchForm.Score1[validate_index].value!="") {
		if(isNaN(BSKAllMatchForm.Score1[validate_index].value)) {
			alert('讓分只能為數字!');
			BSKAllMatchForm.Score1[validate_index].value = '';
		} else {
			BSKAllMatchForm.MUSTSendChk[validate_index].checked = true;
		}
	} else {
		BSKAllMatchForm.MUSTSendChk[validate_index].checked = true;
	}
}

function onScore2Changed(validate_index) {
	if(BSKAllMatchForm.Score2[validate_index].value!="") {
		if(isNaN(BSKAllMatchForm.Score2[validate_index].value)) {
			alert('讓分賠率只能為數字!');
			BSKAllMatchForm.Score2[validate_index].value = '';
		} else {
			BSKAllMatchForm.MUSTSendChk[validate_index].checked = true;
		}
	} else {
		BSKAllMatchForm.MUSTSendChk[validate_index].checked = true;
	}
}

function onOdds1Changed(validate_index) {
	if(BSKAllMatchForm.Odds1[validate_index].value!="") {
		if(isNaN(BSKAllMatchForm.Odds1[validate_index].value)) {
			alert('大小分只能為數字!');
			BSKAllMatchForm.Odds1[validate_index].value = '';
		} else {
			BSKAllMatchForm.MUSTSendChk[validate_index].checked = true;
		}
	} else {
		BSKAllMatchForm.MUSTSendChk[validate_index].checked = true;
	}
}

function onOdds2Changed(validate_index) {
	if(BSKAllMatchForm.Odds2[validate_index].value!="") {
		if(isNaN(BSKAllMatchForm.Odds2[validate_index].value)) {
			alert('大小賠率只能為數字!');
			BSKAllMatchForm.Odds2[validate_index].value = '';
		} else {
			BSKAllMatchForm.MUSTSendChk[validate_index].checked = true;
		}
	} else {
		BSKAllMatchForm.MUSTSendChk[validate_index].checked = true;
	}
}

function onHandicap1Changed(validate_index) {
	if(BSKAllMatchForm.Handicap1[validate_index].value!="") {
		if(isNaN(BSKAllMatchForm.Handicap1[validate_index].value)) {
			alert('單賠率只能為數字!');
			BSKAllMatchForm.Handicap1[validate_index].value = '';
		} else {
			BSKAllMatchForm.MUSTSendChk[validate_index].checked = true;
		}
	}	else {
		BSKAllMatchForm.MUSTSendChk[validate_index].checked = true;
	}
}

function onHandicap2Changed(validate_index) {
	if(BSKAllMatchForm.Handicap2[validate_index].value != "") {
		if(isNaN(BSKAllMatchForm.Handicap2[validate_index].value)) {
			alert('雙賠率只能為數字!');
			BSKAllMatchForm.Handicap2[validate_index].value = '';
		} else {
			BSKAllMatchForm.MUSTSendChk[validate_index].checked = true;
		}
	} else {
		BSKAllMatchForm.MUSTSendChk[validate_index].checked = true;
	}
}

function selectAll(iType) {
	<%
		int iSendChkIndex;
		string sSendChk_All;
	%>
	if(iType == 0) {
		if(BSKAllMatchForm.SelectAllSend.checked == false) {
			<%
				for(iSendChkIndex=0;iSendChkIndex<iRecCount;iSendChkIndex++) {
					sSendChk_All = "MUSTSendChk[" + iSendChkIndex.ToString() + "]";
			%>
					BSKAllMatchForm.<%=sSendChk_All%>.checked = false;
			<%
				}
			%>
		} else {
			if(BSKAllMatchForm.SelectAllSend.checked == true) {
			<%
				for(iSendChkIndex=0;iSendChkIndex<iRecCount;iSendChkIndex++) {
					sSendChk_All = "MUSTSendChk[" + iSendChkIndex.ToString() + "]";
			%>
					BSKAllMatchForm.<%=sSendChk_All%>.checked = true;
			<%
				}
			%>
			}
		}
	} else if(iType == 1) {
		if(BSKAllMatchForm.SelectAllHide.checked == true) {
			<%
				for(iSendChkIndex=0;iSendChkIndex<iRecCount;iSendChkIndex++) {
					sSendChk_All = "hiddenChk[" + iSendChkIndex.ToString() + "]";
			%>
					BSKAllMatchForm.<%=sSendChk_All%>.checked = true;
			<%
				}
			%>
		} else {
			<%
				for(iSendChkIndex=0;iSendChkIndex<iRecCount;iSendChkIndex++) {
					sSendChk_All = "hiddenChk[" + iSendChkIndex.ToString() + "]";
			%>
					BSKAllMatchForm.<%=sSendChk_All%>.checked = false;
			<%
				}
			%>
		}
	}
}
<%
} else {
	if(iRecCount==1) {
%>

function ClearOrderNo() {
	BSKAllMatchForm.orderID.value = '';
}

function onOrder_IDChanged(validate_index) {
	order_re = /^\d{0,2}$/
	re_val = BSKAllMatchForm.orderID.value.search(order_re)
	if(re_val == -1) {
		alert('序號只接受數字');
		BSKAllMatchForm.orderID.value = '';
	}
}

function onStatusChanged(validate_index) {
	BSKAllMatchForm.MUSTSendChk.checked = true;
}

function onScore1Changed(validate_index) {
	if(BSKAllMatchForm.Score1.value!="") {
		if(isNaN(BSKAllMatchForm.Score1.value)) {
			alert('讓分只能為數字!');
			BSKAllMatchForm.Score1.value = '';
		} else {
			BSKAllMatchForm.MUSTSendChk.checked = true;
		}
	} else {
		BSKAllMatchForm.MUSTSendChk.checked = true;
	}
}

function onScore2Changed(validate_index) {
	if(BSKAllMatchForm.Score2.value!="") {
		if(isNaN(BSKAllMatchForm.Score2.value)) {
			alert('讓分賠率只能為數字!');
			BSKAllMatchForm.Score2.value = '';
		} else {
			BSKAllMatchForm.MUSTSendChk.checked = true;
		}
	} else {
		BSKAllMatchForm.MUSTSendChk.checked = true;
	}
}

function onOdds1Changed(validate_index) {
	if(BSKAllMatchForm.Odds1.value!="") {
		if(isNaN(BSKAllMatchForm.Odds1.value)) {
			alert('大小分只能為數字!');
			BSKAllMatchForm.Odds1.value = '';
		} else {
			BSKAllMatchForm.MUSTSendChk.checked = true;
		}
	} else {
		BSKAllMatchForm.MUSTSendChk.checked = true;
	}
}

function onOdds2Changed(validate_index) {
	if(BSKAllMatchForm.Odds2.value!="") {
		if(isNaN(BSKAllMatchForm.Odds2.value)) {
			alert('大小賠率只能為數字!');
			BSKAllMatchForm.Odds2.value = '';
		} else {
			BSKAllMatchForm.MUSTSendChk.checked = true;
		}
	} else {
		BSKAllMatchForm.MUSTSendChk.checked = true;
	}
}

function onHandicap1Changed(validate_index) {
	if(BSKAllMatchForm.Handicap1.value!="") {
		if(isNaN(BSKAllMatchForm.Handicap1.value)) {
			alert('單賠率只能為數字!');
			BSKAllMatchForm.Handicap1.value = '';
		} else {
			BSKAllMatchForm.MUSTSendChk.checked = true;
		}
	} else {
		BSKAllMatchForm.MUSTSendChk.checked = true;
	}
}

function onHandicap2Changed(validate_index) {
	if(BSKAllMatchForm.Handicap2.value!="") {
		if(isNaN(BSKAllMatchForm.Handicap2.value)) {
			alert('雙賠率只能為數字!');
			BSKAllMatchForm.Handicap2.value = '';
		} else {
			BSKAllMatchForm.MUSTSendChk.checked = true;
		}
	} else {
		BSKAllMatchForm.MUSTSendChk.checked = true;
	}
}

function selectAll(iType) {
	if(iType == 0) {
		if(BSKAllMatchForm.SelectAllSend.checked == true) {
			BSKAllMatchForm.MUSTSendChk.checked = true;
		} else {
			BSKAllMatchForm.MUSTSendChk.checked = false;
		}
	} else if(iType == 1) {
		if(BSKAllMatchForm.SelectAllHide.checked == true) {
			BSKAllMatchForm.hiddenChk.checked = true;
		} else {
			BSKAllMatchForm.hiddenChk.checked = false;
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
	parent.content_frame.location.replace('BSKAllMatch.aspx?sort=' + sortType);
}
</script>

<html>
<head>
	<META http-equiv="Content-Type" content="text/html; charset=big5">
	<LINK REL="stylesheet" HREF="/sportStyle.css" TYPE="text/css">
</head>
<body>
	<form id="BSKAllMatchForm" method="post" runat="server">
		<font size="2"><b>上次行動:</b><asp:Label id="historyMsg" runat="server" /></font><br>

		<table border="1" width="100%" style="font: 10pt verdana">
		<tr style="background-color:#93D38B">
			<th colspan="6" align="left">
				<input type="button" name="clearOrder" value="清除序號" onClick="ClearOrderNo()">&nbsp;&nbsp;
				順序依
				<select name="sortType" onChange="ReSort(BSKAllMatchForm.sortType.value)">
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
			<th colspan="6"><font color="#FF0000">賠率格式：&nbsp;#.###</font>&nbsp;&nbsp;&nbsp;(#：&nbsp;任何數字)</th>
			<th>全選<input type="checkbox" name="SelectAllSend" onClick="selectAll(0)"></th>
			<th>全選<input type="checkbox" name="SelectAllHide" onClick="selectAll(1)"></th>
		</tr>

		<tr style="background-color:#93D38B">
			<th>排序</th>
			<th>日期</th>
			<th>時間</th>
			<th>聯賽</th>
			<th>主隊</th>
			<th>客隊</th>
			<th>中立</th>
			<th>主讓</th>
			<th>讓分/賠率</th>
			<th>大小分/賠率</th>
			<th>單賠率/雙賠率</th>
			<th>賠率狀況</th>
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
				<asp:Button id="SendBtn" Text="傳送" CommandName="SEND" OnCommand="GetMatches" tabindex="499" runat="server" />
			</td>
			<td></td>
		</tr>
		<tr>
			<td colspan="11"><asp:Button id="SortMatchBtn" Text="儲存序號" CommandName="SORT" OnCommand="GetMatches" tabindex="800" runat="server" /></td>
			<td colspan="2" align="right"><asp:Button id="ShowMatchBtn" Text="顯示所有" CommandName="SHOW" OnCommand="GetMatches" runat="server" /></td>
			<td align="center"><asp:Button id="HideMatchBtn" Text="隱藏" CommandName="HIDE" OnCommand="GetMatches" runat="server" /></td>
		</tr>
		</table>
	</form>
</body>
</html>