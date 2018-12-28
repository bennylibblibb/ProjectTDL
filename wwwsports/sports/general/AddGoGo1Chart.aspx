<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">
	void Page_Load(Object sender,EventArgs e) {
		AddGoGo1Chart chart = new AddGoGo1Chart((string)Application["SoccerDBConnectionString"]);

		try {
			ChartDataInformation.InnerHtml = chart.Display();
			UpdateHistoryMessage((string)Application["retrieveInfoMsg"] + "新增賽事圖表(" + DateTime.Now.ToString("HH:mm:ss") + ")");
		} catch(NullReferenceException) {
			FormsAuthentication.SignOut();
			UpdateHistoryMessage("<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>");
		}
	}

	void OnAdd(Object sender,EventArgs e) {
		int iMatchCount = 0;
		AddGoGo1Chart chart = new AddGoGo1Chart((string)Application["SoccerDBConnectionString"]);

		try {
			iMatchCount = chart.Add();
			Response.Redirect("Gogo1ChartData.aspx?matchcount=" + iMatchCount.ToString());
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
function TimestampValidity() {
	if(ChartDataForm.timestamp.value != '') {
		var len = ChartDataForm.timestamp.value.length;
		if(eval(len) == 14) {
			re=/\d{14,14}/
			x = ChartDataForm.timestamp.value.search(re);
			if(x == -1) {
				ChartDataForm.timestamp.value = '';
				alert('不正確時間');
			}
		}	else {
			ChartDataForm.timestamp.value = '';
			alert('不正確時間');
		}
	}
}

function HandicapValidity() {
	if(ChartDataForm.Handicap.value != '') {
		var len = ChartDataForm.Handicap.value.length;
		if(eval(len) == 1) {
			re=/\d/
			x = ChartDataForm.Handicap.value.search(re);
			if(x == -1) {
				ChartDataForm.Handicap.value = '';
				alert('不正確讓球數字');
			}
		}	else if(eval(len) == 2) {
			re=/\d{2,2}/
			x = ChartDataForm.Handicap.value.search(re);
			if(x == -1) {
				ChartDataForm.Handicap.value = '';
				alert('不正確讓球數字');
			}
		}	else if(eval(len) == 3) {
			re=/\d\x2e\d/
			x = ChartDataForm.Handicap.value.search(re);
			if(x == -1) {
				ChartDataForm.Handicap.value = '';
				alert('不正確讓球數字');
			}
		}	else if(eval(len) == 4) {
			ChartDataForm.Handicap.value = '';
			alert('不正確讓球數字');
		}	else if(eval(len) == 5) {
			re=/\d\x2f\d\x2e\d/
			x = ChartDataForm.Handicap.value.search(re);
			if(x == -1) {
				re2=/\d\x2e\d\x2f\d/
				x2 = ChartDataForm.Handicap.value.search(re2);
				if(x2 == -1) {
					ChartDataForm.Handicap.value = '';
					alert('不正確讓球數字');
				}
			}
		}
	}
}

function OddsValidity() {
	if(ChartDataForm.Odds.value != '') {
		var len = ChartDataForm.Odds.value.length;
		if(eval(len) == 1) {
			re=/\d/
			x = ChartDataForm.Odds.value.search(re);
			if(x == -1) {
				ChartDataForm.Odds.value = '';
				alert('不正確賠率數字');
			}
		}	else if(eval(len) == 2) {
			re=/\d\d{2,2}/
			x = ChartDataForm.Odds.value.search(re);
			if(x == -1) {
				ChartDataForm.Odds.value = '';
				alert('不正確賠率數字');
			}
		}	else if(eval(len) == 3) {
			re=/\d\x2e\d/
			x = ChartDataForm.Odds.value.search(re);
			if(x == -1) {
				ChartDataForm.Odds.value = '';
				alert('不正確賠率數字');
			}
		}	else if(eval(len) == 4) {
			re=/\d\x2e\d{2,2}/
			x = ChartDataForm.Odds.value.search(re);
			if(x == -1) {
				re2=/\d{2,2}\x2e\d/
				x2 = ChartDataForm.Odds.value.search(re2);
				if(x2 == -1) {
					ChartDataForm.Odds.value = '';
					alert('不正確賠率數字');
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
						alert('不正確賠率數字');
					}
				}
			}
		}
	}
}

function redirect(url) {
	parent.content_frame.location.replace(url);
}


</script>

<html>
<head>
	<META http-equiv="Content-Type" content="text/html; charset=big5">
	<LINK REL="stylesheet" HREF="/sportStyle.css" TYPE="text/css">
</head>
<body>
	<form id="ChartDataForm" method="post" runat="server">
		<font size="2"><b>上次行動:</b><asp:Label id="historyMsg" runat="server" /></font><br>

		<table border="1" width="45%" style="font: 10pt verdana">
			<span id="ChartDataInformation" runat="server" />
			<tr style="background-color:#7FFFD4">
				<th>時間(yyyyMMddHHmmss)</th>
				<th>讓球</th>
				<th>賠率</th>
			</tr>

			<tr align="center">
				<td><input name="timestamp" maxlength="14" size="14" onChange="TimestampValidity()"></td>
				<td><input name="Handicap" maxlength="5" size="2" onChange="HandicapValidity()"></td>
				<td><input name="Odds" maxlength="5" size="2" onChange="OddsValidity()"></td>
			</tr>

			<tr>
				<td colspan="3" align="right">
					<input type="submit" id="AddBtn" value="新增" OnServerClick="OnAdd" runat="server">&nbsp;
					<input type="button" id="BackBtn" value="返回" onClick="redirect('Gogo1ChartData.aspx?matchcount=' + ChartDataForm.MatchCount.value)">
				</td>
			</tr>
		</table>
	</form>
</body>
</html>