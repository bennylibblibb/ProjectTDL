<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">
	void Page_Load(Object sender,EventArgs e) {
		ModifyResultContent result = new ModifyResultContent();
		try {
			ResultContentInformation.InnerHtml = result.GetDetails();
			UpdateReturnMessage((string)Application["retrieveInfoMsg"] + "賽果詳情" + "(" + DateTime.Now.ToString("HH:mm:ss") + ")");
		} catch(NullReferenceException) {
			FormsAuthentication.SignOut();
			UpdateReturnMessage("<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>");
		}
	}

	void onModify(Object sender,EventArgs e) {
		ModifyResultContent result = new ModifyResultContent();
		try {
			int iUpd = result.Modify();
			Page_Load(sender, e);
			if(iUpd > 0) {
				UpdateReturnMessage("成功更新足球賽果(" + DateTime.Now.ToString("HH:mm:ss") + ")");
			} else if(iUpd == 0) {
				UpdateReturnMessage("沒有更新足球賽果(" + DateTime.Now.ToString("HH:mm:ss") + ")");
			} else {
				UpdateReturnMessage((string)Application["transErrorMsg"] + "(" + DateTime.Now.ToString("HH:mm:ss") + ")");
			}
		} catch(NullReferenceException) {
			FormsAuthentication.SignOut();
			UpdateReturnMessage("<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>");
		}
	}

	void UpdateReturnMessage(string sMsg) {
		rtnMsg.Text = sMsg;
	}
</script>

<script language="JavaScript">
function checkScore() {
	pass = true;
	ttscore_re = /^\d{1,2}$/
	score_re = /^\d{0,2}$/
	time_re = /^\d{0,3}$/

	re_val = ModifyResultContentForm.HostTotalGoal.value.search(ttscore_re)
	if(re_val == -1) {
		alert('主隊總比數只接受數字');
		ModifyResultContentForm.HostTotalGoal.select();
		pass = false;
	}

	re_val = ModifyResultContentForm.GuestTotalGoal.value.search(ttscore_re)
	if(re_val == -1) {
		alert('客隊總比數只接受數字');
		ModifyResultContentForm.GuestTotalGoal.select();
		pass = false;
	}

	for(i = 0; i < ModifyResultContentForm.MatchTime.length; i++) {
		re_val = ModifyResultContentForm.MatchTime[i].value.search(time_re)
		if(re_val == -1) {
			alert('時間只接受數字');
			ModifyResultContentForm.MatchTime[i].select();
			pass = false;
		}
	}

	for(i = 0; i < ModifyResultContentForm.HostGoal.length; i++) {
		re_val = ModifyResultContentForm.HostGoal[i].value.search(score_re)
		if(re_val == -1) {
			alert('主隊比數只接受數字');
			ModifyResultContentForm.HostGoal[i].select();
			pass = false;
		}
	}

	for(i = 0; i < ModifyResultContentForm.GuestGoal.length; i++) {
		re_val = ModifyResultContentForm.GuestGoal[i].value.search(score_re)
		if(re_val == -1) {
			alert('客隊比數只接受數字');
			ModifyResultContentForm.GuestGoal[i].select();
			pass = false;
		}
	}

	return pass;
}

function showBubbleText() {
	document.getElementById('bubbleText').style.visibility = "visible";
	document.getElementById('bubbleText').style.position = "absolute";
	document.getElementById('bubbleText').style.left = event.clientX - 180;
	document.getElementById('bubbleText').style.top = event.clientY + 150;
}

function hideBubbleText() {
	document.getElementById('bubbleText').style.visibility = "hidden";
}
</script>

<html>
<head>
	<META http-equiv="Content-Type" content="text/html; charset=big5">
	<LINK REL="stylesheet" HREF="/sportStyle.css" TYPE="text/css">
</head>
<body>
	<center>
	<form id="ModifyResultContentForm" method="post" runat="server" onSubmit="return checkScore()">
		<font size="2"><b>上次行動:</b><asp:Label id="rtnMsg" runat="server" /></asp:Label></font>
		<table border="1" width="80%" style="font: 10pt verdana">
			<span id="bubbleText" style="visibility:hidden"><font color="#FF0000"><b>請仔細觀看下列說明</b></font></span>
			<span id="ResultContentInformation" runat="server" />

			<tr>
				<td colspan="5"></td>
				<td align="center">
					<input type="submit" id="SaveBtn" value="修改" OnServerClick="onModify" runat="server" onmouseover="showBubbleText()" onmouseout="hideBubbleText()">&nbsp;
					<input type="reset" value="重設">
				</td>
			</tr>
			<tr>
				<th colspan="6" align="left">
					已修改的數據不會發送，請分別重發賽果到GOGO1機、GOGO2機及馬會機<br>
					<font color="#FF0000">重發/刪除資訊 -> 重發傳呼機資訊 -> GOGO1 Sender -> 賽果</font><br>
					<font color="#FF0000">重發/刪除資訊 -> 重發傳呼機資訊 -> GOGO2 Handler 1 -> 賽果</font><br>
					<font color="#FF0000">重發/刪除資訊 -> 重發傳呼機資訊 -> 馬會 Handler 1 -> 賽果</font><br>
					<font color="#FF0000">重發/刪除資訊 -> 重發傳呼機資訊 -> JC Combo Handler -> 賽果</font><br>
					<font color="#CD5C5C">如欲刪除一行數據，請把那行數據的<font color="#000080">時間</font>、<font color="#000080">主隊比數</font>及<font color="#000080">客隊比數</font>填空，再按<font color="#000000">修改</font></font>
				</th>
			</tr>
		</table>
	</form>
	</center>
</body>
</html>