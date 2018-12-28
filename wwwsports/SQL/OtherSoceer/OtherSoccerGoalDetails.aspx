<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">
	void Page_Load(Object sender,EventArgs e) {
		string strMsg;
		OtherSoccerGoalDetails details = new OtherSoccerGoalDetails((string)Application["SoccerDBConnectionString"]);

		try {
			GoalDetailsInformation.InnerHtml = details.GetDetails();
			strMsg = (string)Application["retrieveInfoMsg"] + "比數詳情" + "(" +DateTime.Now.ToString("HH:mm:ss") + ")";
			UpdateReturnMessage(strMsg);
		} catch(NullReferenceException) {
			FormsAuthentication.SignOut();
			UpdateReturnMessage("<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>");
		}
	}

	void UpdateGoalDetailsAction(Object sender,CommandEventArgs e) {
		int iUpdated = 0;
		string sNow;
		OtherSoccerGoalDetails details = new OtherSoccerGoalDetails((string)Application["SoccerDBConnectionString"]);
		sNow = DateTime.Now.ToString("HH:mm:ss");
		try {
			iUpdated = details.Update();
			Page_Load(sender,e);
			if(iUpdated > 0) {
				UpdateReturnMessage("成功更新比數詳情(" + sNow + ")");
			}	else if(iUpdated == 0) {
				UpdateReturnMessage("沒有更新比數詳情(" + sNow + ")");
			}	else {
				UpdateReturnMessage((string)Application["transErrorMsg"] + "(" + sNow + ")");
			}
		}	catch(NullReferenceException nullex) {
			FormsAuthentication.SignOut();
			UpdateReturnMessage("<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>");
		}
	}

	void UpdateReturnMessage(string sMsg) {
		rtnMsg.Text = sMsg;
	}
</script>
<script language="JavaScript">
function OnActionChanged() {
	if(document.GoalDetailsForm.Action.value == 'D') {
		document.GoalDetailsForm.SendToPager[0].checked = true;
		document.GoalDetailsForm.SendToPager[1].checked = true;
		document.GoalDetailsForm.SendToPager[2].checked = true;
		alert('請確定GOGO1,GOGO2及馬會機已選取！');
	}
}

function validation() {
	for(var validate_index=0;validate_index<12;validate_index++) {
		if((GoalDetailsForm.HostScore[validate_index].value != "") || (GoalDetailsForm.guestScore[validate_index].value != "")||(GoalDetailsForm.hostplayer[validate_index].value!="")||(GoalDetailsForm.guestplayer[validate_index].value!="")) {
			if(GoalDetailsForm.ScoreTime[validate_index].value.length == 1)
				re=/\d/;
			else if(GoalDetailsForm.ScoreTime[validate_index].value.length == 2)
				re=/\d\d/;
			else
				re=/\d\d\d/;

			x=GoalDetailsForm.ScoreTime[validate_index].value.search(re);
			if(x==-1) {
				alert('必須輸入正確時間!');
				return false;
			}
		}

		if(isNaN(GoalDetailsForm.HostScore[validate_index].value)) {
			GoalDetailsForm.HostScore[validate_index].value = "";
			alert('入球必須是數字!');
			return false;
		}

		if(isNaN(GoalDetailsForm.guestScore[validate_index].value)) {
			GoalDetailsForm.guestScore[validate_index].value = "";
			alert('入球必須是數字!');
			return false;
		}

		if(isNaN(GoalDetailsForm.ScoreTime[validate_index].value)) {
			GoalDetailsForm.ScoreTime[validate_index].value = "";
			alert('必須輸入正確時間!');
			return false; 
		}

		if((GoalDetailsForm.hostplayer[validate_index].value!="") && (GoalDetailsForm.guestplayer[validate_index].value!="")) {
			alert('主客隊球員只應寫一個!');
			return false;
		}	
	}					
	return true;
}
function OnAlertCheck() {
	if(GoalDetailsForm.Alert.checked==true) {
		GoalDetailsForm.Alert.value="1";
		
	}
	else {
		GoalDetailsForm.Alert.value="0";
		
	}
}

function changeState(validate_index) {
	if(GoalDetailsForm.ScoreEvent[validate_index].value == "00")
		GoalDetailsForm.matchstate.value = "F";
	else if(GoalDetailsForm.ScoreEvent[validate_index].value == "01")
		GoalDetailsForm.matchstate.value = "S";
	else if(GoalDetailsForm.ScoreEvent[validate_index].value == "02")
		GoalDetailsForm.matchstate.value = "G";
	else if(GoalDetailsForm.ScoreEvent[validate_index].value == "03")
		GoalDetailsForm.matchstate.value = "I";
	else if(GoalDetailsForm.ScoreEvent[validate_index].value == "04")
		GoalDetailsForm.matchstate.value = "Y";
	else if(GoalDetailsForm.ScoreEvent[validate_index].value == "05")
		GoalDetailsForm.matchstate.value = "T";
}
</script>
<script language="JavaScript">
function openwindow(matchcnt, oneGoalWinName) {
	oneGoalWinFeature="Resizable=Yes,ScrollBars=Yes,MenuBar=No,Directories=No,ToolBar=No,Location=No,Status=No,width=1010, height=300";
	oneGoalWindow = window.open('OtherSoccerGoalDetailsPK.aspx?matchcnt=' + matchcnt,oneGoalWinName,oneGoalWinFeature);
	oneGoalWindow.moveTo(0,0);
	oneGoalWindow.focus();
}
</script>
<html>
<head>
	<META http-equiv="Content-Type" content="text/html; charset=big5">
	<LINK REL="stylesheet" HREF="/sportStyle.css" TYPE="text/css">
</head>
<body>
	<form id="GoalDetailsForm" method="post" Onsubmit="return validation()" runat="server">
		<font size="2"><b>上次行動:</b><asp:label id="rtnMsg" runat="server" ></asp:Label></font>
		<table border="1" width="90%" style="font: 10pt verdana">
			<span id="GoalDetailsInformation" runat="server"/>
			<tr align="right">
				<td colspan="8">
					<font color="red"><b>在刪除資料時，請確定所有傳呼機已選取，以避免傳呼機資料有錯誤！</b></font>&nbsp;&nbsp;&nbsp;
					<!--
						Value of SendToPager is Device ID defined in DEVICE_TYPE
					-->
					<input type="checkbox" name="SendToPager" value="1" checked>GOGO1&nbsp;
					<input type="checkbox" name="SendToPager" value="2" checked>GOGO2&nbsp;
					<input type="checkbox" name="SendToPager" value="3" checked>馬會機&nbsp;
					<input type="checkbox" name="SendToPager" value="4" checked>JCCombo&nbsp;
					<input type="checkbox" name="SendToPager" value="5" checked>GOGO3Combo&nbsp;
					<INPUT TYPE=CHECKBOX NAME="Alert" VALUE="1" onChange ="OnAlertCheck()">響
					<asp:Button id="SendBtn" Text="傳送" CommandName="SEND" OnCommand="UpdateGoalDetailsAction" runat="server" />&nbsp;
					<input type=reset value="重設">
				</td>
			</tr>
		</table>
	</form>
</body>
</html>