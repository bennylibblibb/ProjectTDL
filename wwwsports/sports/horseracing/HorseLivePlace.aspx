<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">
	void Page_Load(Object sender,EventArgs e) {
		//Set formatter flag to true if it is null, default value
		if(Application["EnableFormat_LVPL"] == null) {
			Application["EnableFormat_LVPL"] = true;
		}

		HorseLivePlace PlaceInfo = new HorseLivePlace((string)Application["SoccerDBConnectionString"]);

		try {
			PlaceInformation.InnerHtml = PlaceInfo.GetPlace();
			UpdateHistoryMessage((string)Application["retrieveInfoMsg"] + "�ɰ�����(" + DateTime.Now.ToString("HH:mm:ss") + ")");
		} catch(NullReferenceException) {
			FormsAuthentication.SignOut();
			UpdateHistoryMessage("<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>");
		}
	}

	void SendRace(Object sender,EventArgs e) {
		int iUpdated = 0;
		//int iCBUpdated = 0;
		string sNow;
		string sCodeStatus = "";
		HorseLivePlace RacePlace = new HorseLivePlace((string)Application["SoccerDBConnectionString"]);
		sNow = DateTime.Now.ToString("HH:mm:ss");

		try {
			if((bool)Application["EnableFormat_LVPL"]) {
				//iCBUpdated = RacePlace.SendCBPlace();
				int[] arrCode = RacePlace.SendPlaceRemoting();
				Page_Load(sender,e);
				if(arrCode[5] > 0) {
					if(arrCode[0]>=0 && arrCode[1]>=0 && arrCode[2]>=0 && arrCode[3]==100000 && arrCode[4]==100000) {
						sCodeStatus = "���\�o�e�ɰ����� (" + sNow + ")";
					} else {
						if(arrCode[0] == -1) {
							sCodeStatus += "<br><b>�g�JGOGO������T�����~�I</b>";
						}
						if(arrCode[1] == -1) {
							sCodeStatus += "<br><b>�g�J���|������T�����~�I</b>";
						}
						if(arrCode[2] == -1) {
							sCodeStatus += "<br><b>�g�J�M�~�ɰ�������T�����~�I</b>";
						}
						if(arrCode[3] == -1) {
							sCodeStatus += "<br><b>�ǰeGOGO��������~�A�t�ΰ��D�I</b>";
						}
						if(arrCode[3] == -2) {
							sCodeStatus += "<br><b>�ǰeGOGO��������~�A�s�����ѡI</b>";
						}
						if(arrCode[3] == -3) {
							sCodeStatus += "<br><b>�S��GOGO������i�ǰe�I</b>";
						}
						if(arrCode[4] == -1) {
							sCodeStatus += "<br><b>�ǰe���|��������~�A�t�ΰ��D�I</b>";
						}
						if(arrCode[4] == -2) {
							sCodeStatus += "<br><b>�ǰe���|��������~�A�s�����ѡI</b>";
						}
						if(arrCode[4] == -3) {
							sCodeStatus += "<br><b>�S�����|������i�ǰe�I</b>";
						}
					}
				} else {
					sCodeStatus = "<br><b>" + (string)Application["transErrorMsg"] + "</b> (" + sNow + ")";
				}
/*
				if (iCBUpdated == 0){
					sCodeStatus = (string)Application["transErrorMsg"] + "(" + sNow + ")";
				}
*/
				UpdateHistoryMessage(sCodeStatus);
			} else {
				iUpdated = RacePlace.SendPlace();
				Page_Load(sender,e);
				if(iUpdated > 0) {
					UpdateHistoryMessage("���\�o�e�ɰ����� (" + sNow + ")");
				} else {
					UpdateHistoryMessage((string)Application["transErrorMsg"] + "(" + sNow + ")");
				}
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
function FirstValidity() {
	place_re = /^\d{0,2}$/
	re_val = HorseLivePlaceInfoForm.First.value.search(place_re)
	if(re_val == -1) {
		alert('����u�����Ʀr');
		HorseLivePlaceInfoForm.First.value = '';
	}
}
function SecondValidity() {
	place_re = /^\d{0,2}$/
	re_val = HorseLivePlaceInfoForm.Second.value.search(place_re)
	if(re_val == -1) {
		alert('����u�����Ʀr');
		HorseLivePlaceInfoForm.Second.value = '';
	}
}
function ThirdValidity() {
	place_re = /^\d{0,2}$/
	re_val = HorseLivePlaceInfoForm.Third.value.search(place_re)
	if(re_val == -1) {
		alert('����u�����Ʀr');
		HorseLivePlaceInfoForm.Third.value = '';
	}
}
function FourthValidity() {
	place_re = /^\d{0,2}$/
	re_val = HorseLivePlaceInfoForm.Fourth.value.search(place_re)
	if(re_val == -1) {
		alert('����u�����Ʀr');
		HorseLivePlaceInfoForm.Fourth.value = '';
	}
}
function SetFlag() {
	flagWinFeature="Resizable=Yes,ScrollBars=Yes,MenuBar=No,Directories=No,ToolBar=No,Location=No,Status=No, width=600, height=200";
	flagWindow = window.open('../admin/FormatFlagController.aspx', 'FormatterFlag' , flagWinFeature);
	flagWindow.focus();
}
</script>

<html>
<head>
	<META http-equiv="Content-Type" content="text/html; charset=big5">
	<LINK REL="stylesheet" HREF="/sportStyle.css" TYPE="text/css">
</head>
<body>
	<form id="HorseLivePlaceInfoForm" method="post" runat="server">
		<font size="2"><b>�W�����:</b><asp:Label id="historyMsg" runat="server" /></font><br>

		<table border="1" width="50%" style="font: 10pt verdana">
		<span id="PlaceInformation" runat="server" />
		<tr>
			<td colspan="5" align="right">
				<%
					if(Convert.ToInt32(Session["user_role"]) >= 988) {
				%>
						<a href="javascript:SetFlag()">�ק�Format�~�|</a>&nbsp;&nbsp;
				<%
					}
				%>

				<!--
					Value of SendToPager is Device ID defined in DEVICE_TYPE
				-->
				<input type="hidden" name="SendToPager" value="1">
				<input type="hidden" name="SendToPager" value="3">
				<input type="hidden" name="SendToPager" value="4">
				<input type="checkbox" name="PlaceAlert" value="1">�T��&nbsp;
				<input type="submit" id="SendBtn" value="�o�e" OnServerClick="SendRace" runat="server">&nbsp;
				<input type="reset" value="���]">&nbsp;
			</td>
		</tr>
		</table>
	</form>
</body>
</html>