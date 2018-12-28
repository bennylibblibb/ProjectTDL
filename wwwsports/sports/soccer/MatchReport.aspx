<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">
	void Page_Load(Object sender,EventArgs e) {
		MatchReport recentReport = new MatchReport((string)Application["GOGO2SOCDBConnectionString"],(string)Application["SoccerDBConnectionString"]);		
		
		try {
			MatchReportInformation.InnerHtml = recentReport.GetRecent();
			UpdateHistoryMessage((string)Application["retrieveInfoMsg"] + "�ɫ���i(" + DateTime.Now.ToString("HH:mm:ss") + ")");
		} catch(NullReferenceException) {
			FormsAuthentication.SignOut();
		
		}
	}
	
	void onSendReportReview(Object sender,EventArgs e) {
		int iUpdated = 0;
		string sNow;
		MatchReport sendReport = new MatchReport((string)Application["GOGO2SOCDBConnectionString"],(string)Application["SoccerDBConnectionString"]);

		sNow = DateTime.Now.ToString("HH:mm:ss");
		try {
			iUpdated = sendReport.Update();
			Page_Load(sender,e);
			if(iUpdated >= 0) {
				UpdateHistoryMessage("���\�ק��ɫ���i(" + sNow + ")");
			} else {
				UpdateHistoryMessage("�S���ק��ɫ���i(" + sNow + ")");
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

function checkMatchVenueLength() {
	str = document.MatchReportForm.matchvenue.value;
	strlen = LengthOfString(str);
	if(strlen > 10) {
		alert('�y���W�٤���h��10�Ӧ줸');
		document.MatchReportForm.matchvenue.value = '';
	}
}

function checkAttentionFormat() {
	var re = /^((\d*))$/ 			
	if(!re.test(document.MatchReportForm.attention.value)) {
		alert('�J�y�H�ƥu�����Ʀr');
		document.MatchReportForm.attention.value = '';
	}
}

function checkAttentionRateFormat() {
	var re  = /^((\d*)|(\d{1}\d*\.\d{1})|(\d{1}\d*\.\d{2}))$/ 	
	if(!re.test(document.MatchReportForm.attentionrate.value)) {
		alert('�J�y�v�u�����Ʀr');
		document.MatchReportForm.attentionrate.value = '';
	}		
}

function LengthOfString(s) {
	len = 0;
	for(i = 0; i < s.length; i++) {
		if((s.charCodeAt(i) <= 255) && (s.charCodeAt(i) >= 0)) {
			len++;
		} else {
			len = len + 2;
		}
	}
	return len;
}
function checkReportByte() {
	str = document.MatchReportForm.report.value;
	strlen = LengthOfString(str);
	if(strlen > 400) {
		alert('���i�Ա�����h��400�Ӧ줸');
	}
}

</script>

<html>
<head>
	<META http-equiv="Content-Type" content="text/html; charset=big5">
	<LINK REL="stylesheet" HREF="/sportStyle.css" TYPE="text/css">
</head>
<body >
	<form id="MatchReportForm"  runat="server" >
		<font size="2"><b>�W�����:</b><asp:Label id="historyMsg" runat="server" /></font><br>
		<table border="1" width="100%" style="font: 10pt verdana">
		<span id="MatchReportInformation" runat="server" />
		<tr>
			<td>&nbsp;</td>
			<td align="right">
				<!--
					Value of SendToPager is Device ID defined in DEVICE_TYPE
				-->
				<input type="checkbox" name="SendToPager" value="2" checked>GOGO2&nbsp;
				<input type="checkbox" name="SendToPager" value="5" checked>GOGO3Combo&nbsp;
				<input type="submit" id="SendBtn" value="�o�e" OnServerClick="onSendReportReview" runat="server">&nbsp;
				<input type="reset" value="���]">
			</td>
			
		</table>
	</form>
</body>
</html>