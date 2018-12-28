<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">
	void Page_Load(Object sender,EventArgs e) {
		CorrectScore cScore = new CorrectScore((string)Application["SoccerDBConnectionString"]);
		try {
			CorrectScoreInformation.InnerHtml = cScore.GetMatrix();
			UpdateHistoryMessage((string)Application["retrieveInfoMsg"] + "波膽(" + DateTime.Now.ToString("HH:mm:ss") + ")");
		} catch(NullReferenceException) {
			FormsAuthentication.SignOut();
			UpdateHistoryMessage("<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>");
		}
	}

	void ModifyCorrectScoreOdds(Object sender,EventArgs e) {
		int iUpdated = 0;
		string sNow;
		CorrectScore cOdds = new CorrectScore((string)Application["SoccerDBConnectionString"]);
		sNow = DateTime.Now.ToString("HH:mm:ss");
		try {
			iUpdated = cOdds.Modify();
			Page_Load(sender,e);
			if(iUpdated > 0) {
				UpdateHistoryMessage("成功修改波膽(" + sNow + ")");
			}	else if(iUpdated == 0) {
				UpdateHistoryMessage("沒有修改波膽(" + sNow + ")");
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
function setMatrixSize() {
	if(document.CorrectScoreForm.MatrixSize[0].checked == true) {
		document.CorrectScoreForm.odds[6].disabled = true;
		document.CorrectScoreForm.odds[6].style.background = "#D3D3D3";
		document.CorrectScoreForm.odds[12].disabled = true;
		document.CorrectScoreForm.odds[12].style.background = "#D3D3D3";
		document.CorrectScoreForm.odds[18].disabled = true;
		document.CorrectScoreForm.odds[18].style.background = "#D3D3D3";
		document.CorrectScoreForm.odds[24].disabled = true;
		document.CorrectScoreForm.odds[24].style.background = "#D3D3D3";
		document.CorrectScoreForm.odds[30].disabled = true;
		document.CorrectScoreForm.odds[30].style.background = "#D3D3D3";
		document.CorrectScoreForm.odds[32].disabled = true;
		document.CorrectScoreForm.odds[32].style.background = "#D3D3D3";
		document.CorrectScoreForm.odds[33].disabled = true;
		document.CorrectScoreForm.odds[33].style.background = "#D3D3D3";
		document.CorrectScoreForm.odds[34].disabled = true;
		document.CorrectScoreForm.odds[34].style.background = "#D3D3D3";
		document.CorrectScoreForm.odds[35].disabled = true;
		document.CorrectScoreForm.odds[35].style.background = "#D3D3D3";
		document.CorrectScoreForm.odds[36].disabled = true;
		document.CorrectScoreForm.odds[36].style.background = "#D3D3D3";
		document.CorrectScoreForm.odds[37].disabled = true;
		document.CorrectScoreForm.odds[37].style.background = "#D3D3D3";
		document.all("g_5").innerHTML = "&gt;4";
		document.all("g_gt5").innerHTML = "-";
		document.all("h_5").innerHTML = "&gt;4";
		document.all("h_gt5").innerHTML = "-";
	} else if(CorrectScoreForm.MatrixSize[1].checked == true) {
		document.CorrectScoreForm.odds[6].disabled = false;
		document.CorrectScoreForm.odds[6].style.background = "#FFFFFF";
		document.CorrectScoreForm.odds[12].disabled = false;
		document.CorrectScoreForm.odds[12].style.background = "#FFFFFF";
		document.CorrectScoreForm.odds[18].disabled = false;
		document.CorrectScoreForm.odds[18].style.background = "#FFFFFF";
		document.CorrectScoreForm.odds[24].disabled = false;
		document.CorrectScoreForm.odds[24].style.background = "#FFFFFF";
		document.CorrectScoreForm.odds[30].disabled = false;
		document.CorrectScoreForm.odds[30].style.background = "#FFFFFF";
		document.CorrectScoreForm.odds[32].disabled = false;
		document.CorrectScoreForm.odds[32].style.background = "#FFFFFF";
		document.CorrectScoreForm.odds[33].disabled = false;
		document.CorrectScoreForm.odds[33].style.background = "#FFFFFF";
		document.CorrectScoreForm.odds[34].disabled = false;
		document.CorrectScoreForm.odds[34].style.background = "#FFFFFF";
		document.CorrectScoreForm.odds[35].disabled = false;
		document.CorrectScoreForm.odds[35].style.background = "#FFFFFF";
		document.CorrectScoreForm.odds[36].disabled = false;
		document.CorrectScoreForm.odds[36].style.background = "#FFFFFF";
		document.CorrectScoreForm.odds[37].disabled = false;
		document.CorrectScoreForm.odds[37].style.background = "#FFFFFF";
		document.all("g_5").innerHTML = "5";
		document.all("g_gt5").innerHTML = "&gt;5";
		document.all("h_5").innerHTML = "5";
		document.all("h_gt5").innerHTML = "&gt;5";
	}
}

function OddsValidity(validate_index) {
	if(document.CorrectScoreForm.odds[validate_index].value != '') {
		var len = document.CorrectScoreForm.odds[validate_index].value.length;
		if(eval(len) == 1) {
			re=/\d{1}/
			x = document.CorrectScoreForm.odds[validate_index].value.search(re);
			if(x == -1) {
				document.CorrectScoreForm.odds[validate_index].value = '';
				alert('不正確賠率數字');
			}
		}	else if(eval(len) == 2) {
			re=/\d{2}/
			x = document.CorrectScoreForm.odds[validate_index].value.search(re);
			if(x == -1) {
				document.CorrectScoreForm.odds[validate_index].value = '';
				alert('不正確賠率數字');
			}
		}	else if(eval(len) == 3) {
			re=/\d\x2e\d/
			x = document.CorrectScoreForm.odds[validate_index].value.search(re);
			if(x == -1) {
				re2=/\d{3}/
				x2 = document.CorrectScoreForm.odds[validate_index].value.search(re2);
				if(x2 == -1) {
					document.CorrectScoreForm.odds[validate_index].value = '';
					alert('不正確賠率數字');
				}
			}
		}	else if(eval(len) == 4) {
			re=/\d{4}/
			x = document.CorrectScoreForm.odds[validate_index].value.search(re);
			if(x == -1) {
				re2=/\d\x2e\d{2}/
				x2 = document.CorrectScoreForm.odds[validate_index].value.search(re2);
				if(x2 == -1) {
					re3=/\d{2}\x2e\d/
					x3 = document.CorrectScoreForm.odds[validate_index].value.search(re3);
					if(x3 == -1) {
						document.CorrectScoreForm.odds[validate_index].value = '';
						alert('不正確賠率數字');
					}
				}
			}
		}	else if(eval(len) == 5) {
			re=/\d{5}/
			x = document.CorrectScoreForm.odds[validate_index].value.search(re);
			if(x == -1) {
				re2=/\d\x2e\d{3}/
				x2 = document.CorrectScoreForm.odds[validate_index].value.search(re2);
				if(x2 == -1) {
					re3=/\d{2}\x2e\d{2}/
					x3 = document.CorrectScoreForm.odds[validate_index].value.search(re3);
					if(x3 == -1) {
						re4=/\d{3}\x2e\d/
						x4 = document.CorrectScoreForm.odds[validate_index].value.search(re4);
						if(x4 == -1) {
							document.CorrectScoreForm.odds[validate_index].value = '';
							alert('不正確賠率數字');
						}
					}
				}
			}
		}
	}
}
</script>

<html>
<head>
	<META http-equiv="Content-Type" content="text/html; charset=big5">
	<LINK REL="stylesheet" HREF="/sportStyle.css" TYPE="text/css">
</head>
<body>
	<form id="CorrectScoreForm" method="post" runat="server">
		<font size="2"><b>上次行動:</b><asp:Label id="historyMsg" runat="server" /></font><br>

		<table border="1" width="55%" style="font: 10pt verdana">
		<span id="CorrectScoreInformation" runat="server" />
		<tr>
			<td colspan="9" align="right">
				<!--
					Value of SendToPager is Device ID defined in DEVICE_TYPE
				-->
				<input type="checkbox" name="SendToPager" value="2" checked>GOGO2&nbsp;
				<input type="checkbox" name="SendToPager" value="5" checked>GOGO3Combo&nbsp;
				<input type="submit" id="SendBtn" value="發送" OnServerClick="ModifyCorrectScoreOdds" runat="server">&nbsp;
				<input type="reset" value="重設">
			</td>
		</table>
	</form>
</body>
</html>