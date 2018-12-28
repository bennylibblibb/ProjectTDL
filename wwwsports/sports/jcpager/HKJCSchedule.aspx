<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">
	void Page_Load(Object sender,EventArgs e) {
		HKJCSchedule schedule = new HKJCSchedule((string)Application["HKJCSOCConnectionString"]);

		try {
			HKJCScheduleInformation.InnerHtml = schedule.Retrieve();
			UpdateHistoryMessage((string)Application["retrieveInfoMsg"] + "���|�ɵ{�]�w(" + DateTime.Now.ToString("HH:mm:ss") + ")");
		} catch(NullReferenceException) {
			FormsAuthentication.SignOut();
			UpdateHistoryMessage("<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>");
		}
	}

	void ModifySchedule(Object sender,EventArgs e) {
		int iUpdated = 0;
		string sNow;
		HKJCSchedule schedule = new HKJCSchedule((string)Application["HKJCSOCConnectionString"]);

		sNow = DateTime.Now.ToString("HH:mm:ss");
		try {
			iUpdated = schedule.Modify();
			Page_Load(sender,e);
			if(iUpdated > 0) {
				UpdateHistoryMessage("���\�קﰨ�|�ɵ{�]�w(" + sNow + ")");
			}	else if(iUpdated == 0) {
				UpdateHistoryMessage("�S���קﰨ�|�ɵ{�]�w");
			} else {
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

<script Language="JavaScript">
function DateValidity(validate_index) {
	if(HKJCScheduleForm.dayOfSchedule[validate_index].value != '') {
		if(Number(HKJCScheduleForm.dayOfSchedule[validate_index].value) > -1) {
			var len = HKJCScheduleForm.dayOfSchedule[validate_index].value.length;
			if(eval(len) == 1) {
				re=/\d/
				x = HKJCScheduleForm.dayOfSchedule[validate_index].value.search(re);
				if(x == -1) {
					HKJCScheduleForm.dayOfSchedule[validate_index].value = '1';
					alert('�����T����Ʀr');
				}
			} else if(eval(len) == 2) {
				re=/\d{2}/
				x = HKJCScheduleForm.dayOfSchedule[validate_index].value.search(re);
				if(x == -1) {
					HKJCScheduleForm.dayOfSchedule[validate_index].value = '1';
					alert('�����T����Ʀr');
				}
			}
		} else {
			HKJCScheduleForm.dayOfSchedule[validate_index].value = '1';
			alert('������ݬO����');
		}
	} else {
		HKJCScheduleForm.dayOfSchedule[validate_index].value = '1';
		alert('������ݬO�Ʀr');
	}
}

function HousekeepValidity(validate_index) {
	if(HKJCScheduleForm.housekeep[validate_index].value != '') {
		if((Number(HKJCScheduleForm.housekeep[validate_index].value) > 0) && (Number(HKJCScheduleForm.housekeep[validate_index].value) < 2360)) {
			var len = HKJCScheduleForm.housekeep[validate_index].value.length;
			if(eval(len) == 1) {
				re=/\d/
				x = HKJCScheduleForm.housekeep[validate_index].value.search(re);
				if(x == -1) {
					HKJCScheduleForm.housekeep[validate_index].value = '1';
					alert('�����T�M���ɶ��Ʀr');
				}
			} else if(eval(len) == 2) {
				re=/\d{2}/
				x = HKJCScheduleForm.housekeep[validate_index].value.search(re);
				if(x == -1) {
					HKJCScheduleForm.housekeep[validate_index].value = '1';
					alert('�����T�M���ɶ��Ʀr');
				}
			} else if(eval(len) == 3) {
				re=/\d{3}/
				x = HKJCScheduleForm.housekeep[validate_index].value.search(re);
				if(x == -1) {
					HKJCScheduleForm.housekeep[validate_index].value = '1';
					alert('�����T�M���ɶ��Ʀr');
				}
			} else if(eval(len) == 4) {
				re=/\d{4}/
				x = HKJCScheduleForm.housekeep[validate_index].value.search(re);
				if(x == -1) {
					HKJCScheduleForm.housekeep[validate_index].value = '1';
					alert('�����T�M���ɶ��Ʀr');
				}
			}
		} else {
			HKJCScheduleForm.housekeep[validate_index].value = '1';
			alert('�M���ɶ����ݦb1��2359����');
		}
	} else {
		HKJCScheduleForm.housekeep[validate_index].value = '1';
		alert('�M���ɶ����ݼƦr');
	}
}

function ActivateValidity(validate_index) {
	if(HKJCScheduleForm.activate[validate_index].value != '') {
		if((Number(HKJCScheduleForm.activate[validate_index].value) > 0) && (Number(HKJCScheduleForm.activate[validate_index].value) < 2360)) {
			var len = HKJCScheduleForm.activate[validate_index].value.length;
			if(eval(len) == 1) {
				re=/\d/
				x = HKJCScheduleForm.activate[validate_index].value.search(re);
				if(x == -1) {
					HKJCScheduleForm.activate[validate_index].value = '1';
					alert('�����T�Ұʮɶ��Ʀr');
				}
			} else if(eval(len) == 2) {
				re=/\d{2}/
				x = HKJCScheduleForm.activate[validate_index].value.search(re);
				if(x == -1) {
					HKJCScheduleForm.activate[validate_index].value = '1';
					alert('�����T�Ұʮɶ��Ʀr');
				}
			} else if(eval(len) == 3) {
				re=/\d{3}/
				x = HKJCScheduleForm.activate[validate_index].value.search(re);
				if(x == -1) {
					HKJCScheduleForm.activate[validate_index].value = '1';
					alert('�����T�Ұʮɶ��Ʀr');
				}
			} else if(eval(len) == 4) {
				re=/\d{4}/
				x = HKJCScheduleForm.activate[validate_index].value.search(re);
				if(x == -1) {
					HKJCScheduleForm.activate[validate_index].value = '1';
					alert('�����T�Ұʮɶ��Ʀr');
				}
			}
		} else {
			HKJCScheduleForm.activate[validate_index].value = '1';
			alert('�Ұʮɶ����ݦb1��2359����');
		}
	} else {
		HKJCScheduleForm.activate[validate_index].value = '1';
		alert('�Ұʮɶ����ݼƦr');
	}
}

function EndMatchValidity(validate_index) {
	if(HKJCScheduleForm.endMatch[validate_index].value != '') {
		if((Number(HKJCScheduleForm.endMatch[validate_index].value) > 0) && (Number(HKJCScheduleForm.endMatch[validate_index].value) < 2360)) {
			var len = HKJCScheduleForm.endMatch[validate_index].value.length;
			if(eval(len) == 1) {
				re=/\d/
				x = HKJCScheduleForm.endMatch[validate_index].value.search(re);
				if(x == -1) {
					HKJCScheduleForm.endMatch[validate_index].value = '1';
					alert('�����T�ɵ{�פ�ɶ��Ʀr');
				}
			} else if(eval(len) == 2) {
				re=/\d{2}/
				x = HKJCScheduleForm.endMatch[validate_index].value.search(re);
				if(x == -1) {
					HKJCScheduleForm.endMatch[validate_index].value = '1';
					alert('�����T�ɵ{�פ�ɶ��Ʀr');
				}
			} else if(eval(len) == 3) {
				re=/\d{3}/
				x = HKJCScheduleForm.endMatch[validate_index].value.search(re);
				if(x == -1) {
					HKJCScheduleForm.endMatch[validate_index].value = '1';
					alert('�����T�ɵ{�פ�ɶ��Ʀr');
				}
			} else if(eval(len) == 4) {
				re=/\d{4}/
				x = HKJCScheduleForm.endMatch[validate_index].value.search(re);
				if(x == -1) {
					HKJCScheduleForm.endMatch[validate_index].value = '1';
					alert('�����T�ɵ{�פ�ɶ��Ʀr');
				}
			}
		} else {
			HKJCScheduleForm.endMatch[validate_index].value = '1';
			alert('�ɵ{�פ�ɶ����ݦb1��2359����');
		}
	} else {
		HKJCScheduleForm.endMatch[validate_index].value = '1';
		alert('�ɵ{�פ�ɶ����ݼƦr');
	}
}

function ResetTimeValidity(validate_index) {
	if(HKJCScheduleForm.resetTime[validate_index].value != '') {
		if((Number(HKJCScheduleForm.resetTime[validate_index].value) > 0) && (Number(HKJCScheduleForm.resetTime[validate_index].value) < 2360)) {
			var len = HKJCScheduleForm.resetTime[validate_index].value.length;
			if(eval(len) == 1) {
				re=/\d/
				x = HKJCScheduleForm.resetTime[validate_index].value.search(re);
				if(x == -1) {
					HKJCScheduleForm.resetTime[validate_index].value = '1';
					alert('�����T���]��Ʈɶ��Ʀr');
				}
			} else if(eval(len) == 2) {
				re=/\d{2}/
				x = HKJCScheduleForm.resetTime[validate_index].value.search(re);
				if(x == -1) {
					HKJCScheduleForm.resetTime[validate_index].value = '1';
					alert('�����T���]��Ʈɶ��Ʀr');
				}
			} else if(eval(len) == 3) {
				re=/\d{3}/
				x = HKJCScheduleForm.resetTime[validate_index].value.search(re);
				if(x == -1) {
					HKJCScheduleForm.resetTime[validate_index].value = '1';
					alert('�����T���]��Ʈɶ��Ʀr');
				}
			} else if(eval(len) == 4) {
				re=/\d{4}/
				x = HKJCScheduleForm.resetTime[validate_index].value.search(re);
				if(x == -1) {
					HKJCScheduleForm.resetTime[validate_index].value = '1';
					alert('�����T���]��Ʈɶ��Ʀr');
				}
			}
		} else {
			HKJCScheduleForm.resetTime[validate_index].value = '1';
			alert('���]��Ʈɶ����ݦb1��2359����');
		}
	} else {
		HKJCScheduleForm.resetTime[validate_index].value = '1';
		alert('���]��Ʈɶ����ݼƦr');
	}
}
</script>
<html>
<head>
	<META http-equiv="Content-Type" content="text/html; charset=big5">
	<LINK REL="stylesheet" HREF="/sportStyle.css" TYPE="text/css">
</head>
<body>
	<form id="HKJCScheduleForm" method="post" runat="server">
		<font size="2"><b>�W�����:</b><asp:Label id="historyMsg" runat="server" /></font><br>

		
			<span id="HKJCScheduleInformation" runat="server" />
			<tr>
				<td colspan="6" align="right">
					<input type="submit" id="SaveBtn" value="�x�s" OnServerClick="ModifySchedule" runat="server">&nbsp;
					<input type="reset" value="���]">
				</td>
			</tr>
		</table>
	</form>
</body>
</html>