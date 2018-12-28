<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">
	void Page_Load(Object sender,EventArgs e) {
		HKJCSchedule schedule = new HKJCSchedule((string)Application["HKJCSOCConnectionString"]);

		try {
			HKJCScheduleInformation.InnerHtml = schedule.Retrieve();
			UpdateHistoryMessage((string)Application["retrieveInfoMsg"] + "馬會賽程設定(" + DateTime.Now.ToString("HH:mm:ss") + ")");
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
				UpdateHistoryMessage("成功修改馬會賽程設定(" + sNow + ")");
			}	else if(iUpdated == 0) {
				UpdateHistoryMessage("沒有修改馬會賽程設定");
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
					alert('不正確日期數字');
				}
			} else if(eval(len) == 2) {
				re=/\d{2}/
				x = HKJCScheduleForm.dayOfSchedule[validate_index].value.search(re);
				if(x == -1) {
					HKJCScheduleForm.dayOfSchedule[validate_index].value = '1';
					alert('不正確日期數字');
				}
			}
		} else {
			HKJCScheduleForm.dayOfSchedule[validate_index].value = '1';
			alert('日期必需是正數');
		}
	} else {
		HKJCScheduleForm.dayOfSchedule[validate_index].value = '1';
		alert('日期必需是數字');
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
					alert('不正確清除時間數字');
				}
			} else if(eval(len) == 2) {
				re=/\d{2}/
				x = HKJCScheduleForm.housekeep[validate_index].value.search(re);
				if(x == -1) {
					HKJCScheduleForm.housekeep[validate_index].value = '1';
					alert('不正確清除時間數字');
				}
			} else if(eval(len) == 3) {
				re=/\d{3}/
				x = HKJCScheduleForm.housekeep[validate_index].value.search(re);
				if(x == -1) {
					HKJCScheduleForm.housekeep[validate_index].value = '1';
					alert('不正確清除時間數字');
				}
			} else if(eval(len) == 4) {
				re=/\d{4}/
				x = HKJCScheduleForm.housekeep[validate_index].value.search(re);
				if(x == -1) {
					HKJCScheduleForm.housekeep[validate_index].value = '1';
					alert('不正確清除時間數字');
				}
			}
		} else {
			HKJCScheduleForm.housekeep[validate_index].value = '1';
			alert('清除時間必需在1至2359之間');
		}
	} else {
		HKJCScheduleForm.housekeep[validate_index].value = '1';
		alert('清除時間必需數字');
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
					alert('不正確啟動時間數字');
				}
			} else if(eval(len) == 2) {
				re=/\d{2}/
				x = HKJCScheduleForm.activate[validate_index].value.search(re);
				if(x == -1) {
					HKJCScheduleForm.activate[validate_index].value = '1';
					alert('不正確啟動時間數字');
				}
			} else if(eval(len) == 3) {
				re=/\d{3}/
				x = HKJCScheduleForm.activate[validate_index].value.search(re);
				if(x == -1) {
					HKJCScheduleForm.activate[validate_index].value = '1';
					alert('不正確啟動時間數字');
				}
			} else if(eval(len) == 4) {
				re=/\d{4}/
				x = HKJCScheduleForm.activate[validate_index].value.search(re);
				if(x == -1) {
					HKJCScheduleForm.activate[validate_index].value = '1';
					alert('不正確啟動時間數字');
				}
			}
		} else {
			HKJCScheduleForm.activate[validate_index].value = '1';
			alert('啟動時間必需在1至2359之間');
		}
	} else {
		HKJCScheduleForm.activate[validate_index].value = '1';
		alert('啟動時間必需數字');
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
					alert('不正確賽程終止時間數字');
				}
			} else if(eval(len) == 2) {
				re=/\d{2}/
				x = HKJCScheduleForm.endMatch[validate_index].value.search(re);
				if(x == -1) {
					HKJCScheduleForm.endMatch[validate_index].value = '1';
					alert('不正確賽程終止時間數字');
				}
			} else if(eval(len) == 3) {
				re=/\d{3}/
				x = HKJCScheduleForm.endMatch[validate_index].value.search(re);
				if(x == -1) {
					HKJCScheduleForm.endMatch[validate_index].value = '1';
					alert('不正確賽程終止時間數字');
				}
			} else if(eval(len) == 4) {
				re=/\d{4}/
				x = HKJCScheduleForm.endMatch[validate_index].value.search(re);
				if(x == -1) {
					HKJCScheduleForm.endMatch[validate_index].value = '1';
					alert('不正確賽程終止時間數字');
				}
			}
		} else {
			HKJCScheduleForm.endMatch[validate_index].value = '1';
			alert('賽程終止時間必需在1至2359之間');
		}
	} else {
		HKJCScheduleForm.endMatch[validate_index].value = '1';
		alert('賽程終止時間必需數字');
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
					alert('不正確重設資料時間數字');
				}
			} else if(eval(len) == 2) {
				re=/\d{2}/
				x = HKJCScheduleForm.resetTime[validate_index].value.search(re);
				if(x == -1) {
					HKJCScheduleForm.resetTime[validate_index].value = '1';
					alert('不正確重設資料時間數字');
				}
			} else if(eval(len) == 3) {
				re=/\d{3}/
				x = HKJCScheduleForm.resetTime[validate_index].value.search(re);
				if(x == -1) {
					HKJCScheduleForm.resetTime[validate_index].value = '1';
					alert('不正確重設資料時間數字');
				}
			} else if(eval(len) == 4) {
				re=/\d{4}/
				x = HKJCScheduleForm.resetTime[validate_index].value.search(re);
				if(x == -1) {
					HKJCScheduleForm.resetTime[validate_index].value = '1';
					alert('不正確重設資料時間數字');
				}
			}
		} else {
			HKJCScheduleForm.resetTime[validate_index].value = '1';
			alert('重設資料時間必需在1至2359之間');
		}
	} else {
		HKJCScheduleForm.resetTime[validate_index].value = '1';
		alert('重設資料時間必需數字');
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
		<font size="2"><b>上次行動:</b><asp:Label id="historyMsg" runat="server" /></font><br>

		
			<span id="HKJCScheduleInformation" runat="server" />
			<tr>
				<td colspan="6" align="right">
					<input type="submit" id="SaveBtn" value="儲存" OnServerClick="ModifySchedule" runat="server">&nbsp;
					<input type="reset" value="重設">
				</td>
			</tr>
		</table>
	</form>
</body>
</html>