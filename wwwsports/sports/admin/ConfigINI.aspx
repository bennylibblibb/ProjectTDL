<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">
	string m_CurrDateTime;

	void Page_Load(Object sender,EventArgs e) {
		ConfigINI configInfo = new ConfigINI();

		try {
			ConfigINIInformation.InnerHtml = configInfo.GetINISetting();
			m_CurrDateTime = configInfo.CurrentDateTime;
		}	catch(NullReferenceException) {
			FormsAuthentication.SignOut();
			rtnMsg.Text = "<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>";
		}
	}

	void ModifyINIAction(Object sender,EventArgs e) {
		int iResult = 0;
		ConfigINI updConfig = new ConfigINI();

		try {
			iResult = updConfig.SetINISetting();
			Page_Load(sender,e);
			if(iResult > 0) {
				rtnMsg.Text = "�ק�t�γ]�w���\";
			}	else if(iResult == 0) {
				rtnMsg.Text = "�t�γ]�w�S���ק�";
			}	else if(iResult == -99) {
				rtnMsg.Text = "�S���ק��v��";
			}	else {
				rtnMsg.Text = "�ק�t�γ]�w����";
			}
		}	catch (NullReferenceException) {
			FormsAuthentication.SignOut();
			rtnMsg.Text = "<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>";
		}
	}
</script>

<script language="JavaScript">
function RushStartChanged(rushstart) {
	re = /^\d{4}$/
	re_val = ConfigINIForm.RushHourStart.value.search(re)
	if(re_val == -1) {
		alert('�榡���~');
		ConfigINIForm.RushHourStart.value = rushstart;
	}	else {
		alert('�c���ɶ����}�l�w����');
	}
}

function RushEndChanged(rushend) {
	re = /^\d{4}$/
	re_val = ConfigINIForm.RushHourEnd.value.search(re)
	if(re_val == -1) {
		alert('�榡���~');
		ConfigINIForm.RushHourEnd.value = rushend;
	}	else {
		alert('�c���ɶ��������w����');
	}
}

function checkedAlert() {
	alert('�ﶵ�w����');
}

function HousekeepChanged(hkt) {
	re = /^\d{4}$/
	re_val = ConfigINIForm.HKT.value.search(re)
	if(re_val == -1) {
		alert('�榡���~');
		ConfigINIForm.HKT.value = hkt;
	}	else {
		alert('Housekeep�ɶ��w����');
	}
}

function HousekeepRefDateChanged(refdate) {
	re = /^\d{8}$/
	re_val = ConfigINIForm.REFDATE.value.search(re)
	if(re_val == -1) {
		alert('�榡���~');
		ConfigINIForm.REFDATE.value = refdate;
	}	else {
		alert('Housekeep REF_DATE �w����');
	}
}

function ResetHousekeep() {
	ConfigINIForm.HKT.value = '0905';
}
</script>

<html>
<head>
	<META http-equiv="Content-Type" content="text/html; charset=big5">
	<LINK REL="stylesheet" HREF="/sportStyle.css" TYPE="text/css">
	<title>��|��T - ���y�t�γ]�w</title>
</head>
<body>
	<center>
	<form id="ConfigINIForm" method="post" runat="server">			
		<table border="1" width="80%">
		  <tr><h3>���y�t�γ]�w</h3></tr>
			<tr>
				<th colspan="2" align="center">
					�t�ήɶ�:&nbsp;<%=m_CurrDateTime%>&nbsp;<a href="ConfigINI.aspx">��s</a>
				</th>
			</tr>
			<span id="ConfigINIInformation" runat="server" />
			<tr align="right">
				<td align="left">
					<input type="button" name="resetHKBtn" value="�w�]Housekeep�ɶ�" onClick="ResetHousekeep()">&nbsp;�ݫ�<b>�x�s</b>�T�w��s
				</td>
				<td>
					<input type="button" id="SaveBtn" value="�x�s" OnServerClick="ModifyINIAction" runat="server">&nbsp;
					<input type="reset" value="���]">
				</td>
			</tr>
		</table>
		<asp:Label id="rtnMsg" runat="server" />
		</form>
	</center>
</body>
</html>