<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">
	int iRecCount;

	void Page_Load(Object sender,EventArgs e) {
		Gogo1ChartData chart = new Gogo1ChartData((string)Application["SoccerDBConnectionString"]);

		try {
			ChartDataInformation.InnerHtml = chart.ShowData();
			iRecCount = chart.NumberOfRecords;
			UpdateHistoryMessage((string)Application["retrieveInfoMsg"] + "�ɨƹϪ�(" + DateTime.Now.ToString("HH:mm:ss") + ")");
		} catch(NullReferenceException) {
			FormsAuthentication.SignOut();
			UpdateHistoryMessage("<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>");
		}
	}

	void OnModify(Object sender,EventArgs e) {
		int iUpdated = 0;
		string sNow;
		Gogo1ChartData chart = new Gogo1ChartData((string)Application["SoccerDBConnectionString"]);

		sNow = DateTime.Now.ToString("HH:mm:ss");
		try {
			iUpdated = chart.ModifyData();
			Page_Load(sender,e);
			if(iUpdated > 0) {
				UpdateHistoryMessage("���\��s" + iUpdated.ToString() + "�����ƹϪ�ƾ�(" + sNow + ")");
			}	else if(iUpdated == 0) {
				UpdateHistoryMessage("�S����s���ƹϪ�(" + sNow + ")");
			}	else {
				UpdateHistoryMessage((string)Application["transErrorMsg"] + "(" + sNow + ")");
			}
		} catch(NullReferenceException nullex) {
			FormsAuthentication.SignOut();
			UpdateHistoryMessage("<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>");
		}
	}

	void OnResend(Object sender,EventArgs e) {
		int iUpdated = 0;
		string sNow;
		Gogo1ChartData chart = new Gogo1ChartData((string)Application["SoccerDBConnectionString"]);

		sNow = DateTime.Now.ToString("HH:mm:ss");
		try {
			iUpdated = chart.Resend();
			Page_Load(sender,e);
			if(iUpdated > 0) {
				UpdateHistoryMessage("���\���o�Ϫ�(" + sNow + ")");
			}	else if(iUpdated == 0) {
				UpdateHistoryMessage("�S���Ϫ�i���o(" + sNow + ")");
			}	else {
				UpdateHistoryMessage((string)Application["transErrorMsg"] + "(" + sNow + ")");
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
function HandicapValidity(validate_index) {
	if(ChartDataForm.Handicap[validate_index].value != '') {
		var len = ChartDataForm.Handicap[validate_index].value.length;
		if(eval(len) == 1) {
			re=/\d/
			x = ChartDataForm.Handicap[validate_index].value.search(re);
			if(x == -1) {
				ChartDataForm.Handicap[validate_index].value = '';
				alert('�����T���y�Ʀr');
			}
		}	else if(eval(len) == 2) {
			re=/\d{2,2}/
			x = ChartDataForm.Handicap[validate_index].value.search(re);
			if(x == -1) {
				ChartDataForm.Handicap[validate_index].value = '';
				alert('�����T���y�Ʀr');
			}
		}	else if(eval(len) == 3) {
			re=/\d\x2e\d/
			x = ChartDataForm.Handicap[validate_index].value.search(re);
			if(x == -1) {
				ChartDataForm.Handicap[validate_index].value = '';
				alert('�����T���y�Ʀr');
			}
		}	else if(eval(len) == 4) {
			ChartDataForm.Handicap[validate_index].value = '';
			alert('�����T���y�Ʀr');
		}	else if(eval(len) == 5) {
			re=/\d\x2f\d\x2e\d/
			x = ChartDataForm.Handicap[validate_index].value.search(re);
			if(x == -1) {
				re2=/\d\x2e\d\x2f\d/
				x2 = ChartDataForm.Handicap[validate_index].value.search(re2);
				if(x2 == -1) {
					ChartDataForm.Handicap[validate_index].value = '';
					alert('�����T���y�Ʀr');
				}
			}
		}
	}
}

function OddsValidity(validate_index) {
	if(ChartDataForm.Odds[validate_index].value != '') {
		var len = ChartDataForm.Odds[validate_index].value.length;
		if(eval(len) == 1) {
			re=/\d/
			x = ChartDataForm.Odds[validate_index].value.search(re);
			if(x == -1) {
				ChartDataForm.Odds[validate_index].value = '';
				alert('�����T�߲v�Ʀr');
			}
		}	else if(eval(len) == 2) {
			re=/\d\d{2,2}/
			x = ChartDataForm.Odds[validate_index].value.search(re);
			if(x == -1) {
				ChartDataForm.Odds[validate_index].value = '';
				alert('�����T�߲v�Ʀr');
			}
		}	else if(eval(len) == 3) {
			re=/\d\x2e\d/
			x = ChartDataForm.Odds[validate_index].value.search(re);
			if(x == -1) {
				ChartDataForm.Odds[validate_index].value = '';
				alert('�����T�߲v�Ʀr');
			}
		}	else if(eval(len) == 4) {
			re=/\d\x2e\d{2,2}/
			x = ChartDataForm.Odds[validate_index].value.search(re);
			if(x == -1) {
				re2=/\d{2,2}\x2e\d/
				x2 = ChartDataForm.Odds[validate_index].value.search(re2);
				if(x2 == -1) {
					ChartDataForm.Odds[validate_index].value = '';
					alert('�����T�߲v�Ʀr');
				}
			}
		}	else if(eval(len) == 5) {
			re=/\d\x2e\d{3,3}/
			x = ChartDataForm.Odds[validate_index].value.search(re);
			if(x == -1) {
				re2=/\d{2,2}\x2e\d{2,2}/
				x2 = ChartDataForm.Odds[validate_index].value.search(re2);
				if(x2 == -1) {
					re3=/\d{3,3}\x2e\d/
					x3 = ChartDataForm.Odds[validate_index].value.search(re3);
					if(x3 == -1) {
						ChartDataForm.Odds[validate_index].value = '';
						alert('�����T�߲v�Ʀr');
					}
				}
			}
		}
	}
}

function selectAll() {
	<%
		int iSendChkIndex;
		string sSendChk_All;
	%>
	if(ChartDataForm.SelectAllSend.checked == true) {
		<%
			for(iSendChkIndex=0;iSendChkIndex<iRecCount;iSendChkIndex++) {
				sSendChk_All = "deleteChk[" + iSendChkIndex.ToString() + "]";
		%>
				ChartDataForm.<%=sSendChk_All%>.checked = true;
		<%
			}
		%>
	} else {
		<%
			for(iSendChkIndex=0;iSendChkIndex<iRecCount;iSendChkIndex++) {
				sSendChk_All = "deleteChk[" + iSendChkIndex.ToString() + "]";
		%>
				ChartDataForm.<%=sSendChk_All%>.checked = false;
		<%
			}
		%>
	}
}
<%
} else {
%>
function HandicapValidity(validate_index) {
	if(ChartDataForm.Handicap.value != '') {
		var len = ChartDataForm.Handicap.value.length;
		if(eval(len) == 1) {
			re=/\d/
			x = ChartDataForm.Handicap.value.search(re);
			if(x == -1) {
				ChartDataForm.Handicap.value = '';
				alert('�����T���y�Ʀr');
			}
		}	else if(eval(len) == 2) {
			re=/\d{2,2}/
			x = ChartDataForm.Handicap.value.search(re);
			if(x == -1) {
				ChartDataForm.Handicap.value = '';
				alert('�����T���y�Ʀr');
			}
		}	else if(eval(len) == 3) {
			re=/\d\x2e\d/
			x = ChartDataForm.Handicap.value.search(re);
			if(x == -1) {
				ChartDataForm.Handicap.value = '';
				alert('�����T���y�Ʀr');
			}
		}	else if(eval(len) == 4) {
			ChartDataForm.Handicap.value = '';
			alert('�����T���y�Ʀr');
		}	else if(eval(len) == 5) {
			re=/\d\x2f\d\x2e\d/
			x = ChartDataForm.Handicap.value.search(re);
			if(x == -1) {
				re2=/\d\x2e\d\x2f\d/
				x2 = ChartDataForm.Handicap.value.search(re2);
				if(x2 == -1) {
					ChartDataForm.Handicap.value = '';
					alert('�����T���y�Ʀr');
				}
			}
		}
	}
}

function OddsValidity(validate_index) {
	if(ChartDataForm.Odds.value != '') {
		var len = ChartDataForm.Odds.value.length;
		if(eval(len) == 1) {
			re=/\d/
			x = ChartDataForm.Odds.value.search(re);
			if(x == -1) {
				ChartDataForm.Odds.value = '';
				alert('�����T�߲v�Ʀr');
			}
		}	else if(eval(len) == 2) {
			re=/\d\d{2,2}/
			x = ChartDataForm.Odds.value.search(re);
			if(x == -1) {
				ChartDataForm.Odds.value = '';
				alert('�����T�߲v�Ʀr');
			}
		}	else if(eval(len) == 3) {
			re=/\d\x2e\d/
			x = ChartDataForm.Odds.value.search(re);
			if(x == -1) {
				ChartDataForm.Odds.value = '';
				alert('�����T�߲v�Ʀr');
			}
		}	else if(eval(len) == 4) {
			re=/\d\x2e\d{2,2}/
			x = ChartDataForm.Odds.value.search(re);
			if(x == -1) {
				re2=/\d{2,2}\x2e\d/
				x2 = ChartDataForm.Odds.value.search(re2);
				if(x2 == -1) {
					ChartDataForm.Odds.value = '';
					alert('�����T�߲v�Ʀr');
				}
			}
		}	else if(eval(len) == 5) {
			re=/\d\x2e\d{3,3}/
			x = ChartDataForm.Odds.value.search(re);
			if(x == -1) {
				re2=/\d{2,2}\x2e\d{2,2}/
				x2 = ChartDataForm.Odds.value.search(re2);
				if(x2 == -1) {
					re3=/\d{3,3}\x2e\d/
					x3 = ChartDataForm.Odds.value.search(re3);
					if(x3 == -1) {
						ChartDataForm.Odds.value = '';
						alert('�����T�߲v�Ʀr');
					}
				}
			}
		}
	}
}

function selectAll() {
	if(ChartDataForm.SelectAllSend.checked == true) {
		ChartDataForm.deleteChk.checked = true;
	} else {
		ChartDataForm.deleteChk.checked = false;
	}
}
<%
	}
%>

function redirect(url) {
	parent.content_frame.location.replace(url);
	//alert(url)
}
</script>

<html>
<head>
	<META http-equiv="Content-Type" content="text/html; charset=big5">
	<LINK REL="stylesheet" HREF="/sportStyle.css" TYPE="text/css">
</head>
<body>
	<form id="ChartDataForm" method="post" runat="server">
		<font size="2"><b>�W�����:</b><asp:Label id="historyMsg" runat="server" /></font><br>

		<table border="1" width="45%" style="font: 10pt verdana">
			<tr style="background-color:#C0C0C0">
				<th>�I��</th>
				<th>�ɶ�</th>
				<th>���y</th>
				<th>�߲v</th>
				<th>�R��<font size=1>(����<input type="checkbox" name="SelectAllSend" onClick="selectAll()">)</font></th>
			</tr>

			<span id="ChartDataInformation" runat="server" />

			<tr>
				<td colspan="2">
					<input type="button" id="DelAllBtn" value="�����R��" onClick="redirect('DeleteGoGo1Chart.aspx?matchcount=' + ChartDataForm.MatchCount.value)">
				</td>
				<td colspan="2" align="center">
					<input type="button" id="AddBtn" value="�s�W�ƾ�" onClick="redirect('AddGoGo1Chart.aspx?matchcount=' + ChartDataForm.MatchCount.value)">
				</td>
				<td align="right">
					<input type="submit" id="SaveBtn" value="�x�s�ק�" OnServerClick="OnModify" runat="server">
				</td>
			</tr>
			<tr>
				<th colspan="5">�ק�Ϫ�ƾڤ��|�ߧY�ǰe�A���ƹϪ�۰ʩ�C�b�p�ɵo�e�@���C</th>
			</tr>
			<tr></tr>
			<tr>
				<td colspan="5" align="right">
					<!--
						Value of SendToPager is Device ID defined in DEVICE_TYPE
					-->
					<input type="hidden" name="SendToPager" value="1">
					<input type="hidden" name="SendToPager" value="5">
					<input type="submit" id="ResendBtn" value="���o�Ϫ�" OnServerClick="OnResend" runat="server">
				</td>
			</tr>
		</table>
	</form>
</body>
</html>