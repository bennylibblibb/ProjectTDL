<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">
	void Page_Load(Object sender,EventArgs e) {
		HorseRacingInfo RaceInfo = new HorseRacingInfo((string)Application["SoccerDBConnectionString"]);

		try {
			RacesInformation.InnerHtml = RaceInfo.RetrievalRace();
			UpdateHistoryMessage((string)Application["retrieveInfoMsg"] + "�O���ɨ�(" + DateTime.Now.ToString("HH:mm:ss") + ")");
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
				UpdateHistoryMessage("���\�s�W�ɰ���T(" + sNow + ")");
			}	else if(iUpdated == 0) {
				UpdateHistoryMessage("�S���s�W�ɰ���T(" + sNow + ")");
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
				UpdateHistoryMessage("���\�R���ɰ���T(" + sNow + ")");
			}	else if(iUpdated == 0) {
				UpdateHistoryMessage("�S���ɰ���T�i�R��(" + sNow + ")");
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
		alert('�ɨƤ���u����8��Ʀr');
		HorseRacingInfoForm.RaceDate.value = '';
	}
}

function RaceLengthValidity(index) {
	length_re = /^\d{0,5}$/
	re_val = HorseRacingInfoForm.RaceLength[index].value.search(length_re)
	if(re_val == -1) {
		alert('�ɨƸ��{�u�����Ʀr');
		HorseRacingInfoForm.RaceLength[index].value = '';
	}
}

function RaceTimeValidity(index) {
	time_re = /^\d{0,4}$/
	re_val = HorseRacingInfoForm.RaceTime[index].value.search(time_re)
	if(re_val == -1) {
		alert('�ɨƮɶ��u�����Ʀr');
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
		<font size="2"><b>�W�����:</b><asp:Label id="historyMsg" runat="server" /></font><br>

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
				<font color="red">�b�C����J�s�ɰ����T�e�A���ݧR���L���ɨ�</font>
				<input type="submit" id="SaveBtn" value="�x�s" OnServerClick="SaveRace" runat="server">&nbsp;
				<input type="reset" value="���]">&nbsp;
				<input type="submit" id="DeleteBtn" value="�R���Ҧ��ɨ�" OnServerClick="DeleteRace" runat="server">
			</td>
		</tr>
		</table>
	</form>
</body>
</html>