<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">
	void Page_Load(Object sender,EventArgs e) {
		HorseRacingInfo RaceInfo = new HorseRacingInfo((string)Application["SoccerDBConnectionString"]);

		try {
			RacesInformation.InnerHtml = RaceInfo.RetrievalRace();
			UpdateHistoryMessage((string)Application["retrieveInfoMsg"] + "是日賽事(" + DateTime.Now.ToString("HH:mm:ss") + ")");
		} catch(NullReferenceException) {
			FormsAuthentication.SignOut();
			UpdateHistoryMessage("<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>");
		}
	}

	void SaveRace(Object sender,EventArgs e) {
		int iUpdated = 0;
		string sNow;
		HorseRacingInfo RaceSave = new HorseRacingInfo((string)Application["SoccerDBConnectionString"]);
		sNow = DateTime.Now.ToString("HH:mm:ss");
		try {
			iUpdated = RaceSave.SaveRaces();
			Page_Load(sender,e);
			if(iUpdated > 0) {
				UpdateHistoryMessage("成功新增賽馬資訊(" + sNow + ")");
			}	else if(iUpdated == 0) {
				UpdateHistoryMessage("沒有新增賽馬資訊(" + sNow + ")");
			}	else {
				UpdateHistoryMessage((string)Application["transErrorMsg"] + "(" + sNow + ")");
			}
		} catch(NullReferenceException nullex) {
			FormsAuthentication.SignOut();
			UpdateHistoryMessage("<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>");
		}
	}

	void DeleteRace(Object sender,EventArgs e) {
		int iUpdated = 0;
		string sNow;
		HorseRacingInfo RaceDelete = new HorseRacingInfo((string)Application["SoccerDBConnectionString"]);
		sNow = DateTime.Now.ToString("HH:mm:ss");
		try {
			iUpdated = RaceDelete.DeleteRaces();
			Page_Load(sender,e);
			if(iUpdated > 0) {
				UpdateHistoryMessage("成功刪除賽馬資訊(" + sNow + ")");
			}	else if(iUpdated == 0) {
				UpdateHistoryMessage("沒有賽馬資訊可刪除(" + sNow + ")");
			}	else {
				UpdateHistoryMessage((string)Application["transErrorMsg"] + "(" + sNow + ")");
			}
		}	catch(NullReferenceException nullex) {
			FormsAuthentication.SignOut();
			UpdateHistoryMessage("<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>");
		}
	}

	void UpdateHistoryMessage(string sMsg) {
		historyMsg.Text = sMsg;
	}
</script>

<script language="JavaScript">
function RaceDateValidity() {
	date_re = /^\d{8}$/
	re_val = HorseRacingInfoForm.RaceDate.value.search(date_re)
	if(re_val == -1) {
		alert('賽事日期只接受8位數字');
		HorseRacingInfoForm.RaceDate.value = '';
	}
}

function RaceLengthValidity(index) {
	length_re = /^\d{0,5}$/
	re_val = HorseRacingInfoForm.RaceLength[index].value.search(length_re)
	if(re_val == -1) {
		alert('賽事路程只接受數字');
		HorseRacingInfoForm.RaceLength[index].value = '';
	}
}

function RaceTimeValidity(index) {
	time_re = /^\d{0,4}$/
	re_val = HorseRacingInfoForm.RaceTime[index].value.search(time_re)
	if(re_val == -1) {
		alert('賽事時間只接受數字');
		HorseRacingInfoForm.RaceTime[index].value = '';
	}
}
</script>

<html>
<head>
	<META http-equiv="Content-Type" content="text/html; charset=big5">
	<LINK REL="stylesheet" HREF="/sportStyle.css" TYPE="text/css">
</head>
<body>
	<form id="HorseRacingInfoForm" method="post" runat="server">
		<font size="2"><b>上次行動:</b><asp:Label id="historyMsg" runat="server" /></font><br>

		<table border="1" width="60%" style="font: 10pt verdana">
		<span id="RacesInformation" runat="server" />
		<tr>
			<td colspan="4" align="right">
				<!--
					Value of SendToPager is Path ID
					1: GOGO1 Sender
					4: HKJC Sender
					15: GOGO3Combo Asia 
				-->
				<input type="hidden" name="SendToPager" value="1">
				<input type="hidden" name="SendToPager" value="4">
				<input type="hidden" name="SendToPager" value="15">
				<font color="red">在每次輸入新賽馬日資訊前，必需刪除過往賽事</font>
				<input type="submit" id="SaveBtn" value="儲存" OnServerClick="SaveRace" runat="server">&nbsp;
				<input type="reset" value="重設">&nbsp;
				<input type="submit" id="DeleteBtn" value="刪除所有賽事" OnServerClick="DeleteRace" runat="server">
			</td>
		</tr>
		</table>
	</form>
</body>
</html>