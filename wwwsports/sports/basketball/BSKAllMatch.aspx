<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">
	int iRecCount;

	void Page_Load(Object sender,EventArgs e) {
		BSKAllMatch matchInfoBsk = new BSKAllMatch((string)Application["BasketballDBConnectionString"]);

		try {
			MatchesInformation.InnerHtml = matchInfoBsk.GetAllMatches();
			iRecCount = matchInfoBsk.NumberOfRecords;
			UpdateHistoryMessage((string)Application["retrieveInfoMsg"] + "�Ҧ��ɨ�(" + DateTime.Now.ToString("HH:mm:ss") + ")");

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
					UpdateHistoryMessage("���\��s" + iUpdated.ToString() + "���ɨƸ�T(" + sNow + ")");
				}	else if(iUpdated == 0) {
					UpdateHistoryMessage("�S����s�ɨ�(" + sNow + ")");
				}	else {
					UpdateHistoryMessage((string)Application["transErrorMsg"] + "(" + sNow + ")");
				}
			}	else if(sCmd.Equals("SORT")) {
				if(iUpdated > 0) {
					UpdateHistoryMessage("���\��s�ɨƧǸ�(" + sNow + ")");
				}	else if(iUpdated == 0) {
					UpdateHistoryMessage("�S���Ǹ���s(" + sNow + ")");
				}	else {
					UpdateHistoryMessage((string)Application["transErrorMsg"] + "(" + sNow + ")");
				}
			}	else if(sCmd.Equals("HIDE")) {
				if(iUpdated > 0) {
					UpdateHistoryMessage("����" + iUpdated.ToString() + "���ɨ�(" + sNow + ")");
				}	else if(iUpdated == 0) {
					UpdateHistoryMessage("�S���ɨ�����(" + sNow + ")");
				}	else {
					UpdateHistoryMessage((string)Application["transErrorMsg"] + "(" + sNow + ")");
				}
			}	else if(sCmd.Equals("SHOW")) {
				if(iUpdated > 0) {
					UpdateHistoryMessage("��ܩҦ��ɨ�(" + sNow + ")");
				} else if(iUpdated == 0) {
					UpdateHistoryMessage("�S���ɨ����(" + sNow + ")");
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
		alert('�Ǹ��u�����Ʀr');
		BSKAllMatchForm.orderID[validate_index].value = '';
	}
}

function onStatusChanged(validate_index) {
	BSKAllMatchForm.MUSTSendChk[validate_index].checked = true;
}

function onScore1Changed(validate_index) {
	if(BSKAllMatchForm.Score1[validate_index].value!="") {
		if(isNaN(BSKAllMatchForm.Score1[validate_index].value)) {
			alert('�����u�ର�Ʀr!');
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
			alert('�����߲v�u�ର�Ʀr!');
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
			alert('�j�p���u�ର�Ʀr!');
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
			alert('�j�p�߲v�u�ର�Ʀr!');
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
			alert('��߲v�u�ର�Ʀr!');
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
			alert('���߲v�u�ର�Ʀr!');
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
		alert('�Ǹ��u�����Ʀr');
		BSKAllMatchForm.orderID.value = '';
	}
}

function onStatusChanged(validate_index) {
	BSKAllMatchForm.MUSTSendChk.checked = true;
}

function onScore1Changed(validate_index) {
	if(BSKAllMatchForm.Score1.value!="") {
		if(isNaN(BSKAllMatchForm.Score1.value)) {
			alert('�����u�ର�Ʀr!');
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
			alert('�����߲v�u�ର�Ʀr!');
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
			alert('�j�p���u�ର�Ʀr!');
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
			alert('�j�p�߲v�u�ର�Ʀr!');
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
			alert('��߲v�u�ର�Ʀr!');
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
			alert('���߲v�u�ର�Ʀr!');
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
		<font size="2"><b>�W�����:</b><asp:Label id="historyMsg" runat="server" /></font><br>

		<table border="1" width="100%" style="font: 10pt verdana">
		<tr style="background-color:#93D38B">
			<th colspan="6" align="left">
				<input type="button" name="clearOrder" value="�M���Ǹ�" onClick="ClearOrderNo()">&nbsp;&nbsp;
				���Ǩ�
				<select name="sortType" onChange="ReSort(BSKAllMatchForm.sortType.value)">
				<%
					if(Session["user_sortType"].ToString().Equals("0")) {
				%>
					<option value="0">�p��</option>
					<option value="1">�Ǹ�</option>
				<%
					} else {
				%>
					<option value="1">�Ǹ�</option>
					<option value="0">�p��</option>
				<%
					}
				%>
				</select>
			</th>
			<th colspan="6"><font color="#FF0000">�߲v�榡�G&nbsp;#.###</font>&nbsp;&nbsp;&nbsp;(#�G&nbsp;����Ʀr)</th>
			<th>����<input type="checkbox" name="SelectAllSend" onClick="selectAll(0)"></th>
			<th>����<input type="checkbox" name="SelectAllHide" onClick="selectAll(1)"></th>
		</tr>

		<tr style="background-color:#93D38B">
			<th>�Ƨ�</th>
			<th>���</th>
			<th>�ɶ�</th>
			<th>�p��</th>
			<th>�D��</th>
			<th>�ȶ�</th>
			<th>����</th>
			<th>�D��</th>
			<th>����/�߲v</th>
			<th>�j�p��/�߲v</th>
			<th>��߲v/���߲v</th>
			<th>�߲v���p</th>
			<th>�j��ǰe</th>
			<th>����</th>
		</tr>

		<span id="MatchesInformation" runat="server" />

		<tr>
			<td colspan="13" align="right">
				<!--
					Value of SendToPager is Device ID defined in DEVICE_TYPE
				-->
				<input type="checkbox" name="SendToPager" value="1" checked>GOGO1&nbsp;
				<input type="checkbox" name="SendToPager" value="1" checked>GOGO2&nbsp;
				<input type="checkbox" name="SendToPager" value="3" checked>���|��&nbsp;
				<!--
				<input type="checkbox" name="SendToPager" value="4" checked>JCCombo&nbsp;
				<input type="checkbox" name="SendToPager" value="5" checked>GOGO3Combo&nbsp;
				-->
				<asp:Button id="SendBtn" Text="�ǰe" CommandName="SEND" OnCommand="GetMatches" tabindex="499" runat="server" />
			</td>
			<td></td>
		</tr>
		<tr>
			<td colspan="11"><asp:Button id="SortMatchBtn" Text="�x�s�Ǹ�" CommandName="SORT" OnCommand="GetMatches" tabindex="800" runat="server" /></td>
			<td colspan="2" align="right"><asp:Button id="ShowMatchBtn" Text="��ܩҦ�" CommandName="SHOW" OnCommand="GetMatches" runat="server" /></td>
			<td align="center"><asp:Button id="HideMatchBtn" Text="����" CommandName="HIDE" OnCommand="GetMatches" runat="server" /></td>
		</tr>
		</table>
	</form>
</body>
</html>