<%@ Page EnableViewState="false"%>

<%@ Import Namespace="SportsUtil"%>

<script language="C#" runat="server">
	string m_CurrDateTime;

	void Page_Load(Object sender,EventArgs e) {
		ConfigNbaINI configInfo = new ConfigNbaINI();

		try {
			ConfigNbaINIInformation.InnerHtml = configInfo.GetINISetting();
			m_CurrDateTime = configInfo.CurrentDateTime;
		}	catch(NullReferenceException) {
			FormsAuthentication.SignOut();
			rtnMsg.Text = "<a href=\"/sports/index.htm\" target=\"_top\">" + (string)Application["sessionExpiredMsg"] + "</a>";
		}
	}

	void ModifyINIAction(Object sender,EventArgs e) {
		int iResult = 0;
		ConfigNbaINI updConfig = new ConfigNbaINI();

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

function HousekeepChanged(hkt) {
	re = /^\d{4}$/
	re_val = ConfigNbaINIForm.HKT.value.search(re)
	if(re_val == -1) {
		alert('�榡���~');
		ConfigNbaINIForm.HKT.value = hkt;
	}
	else {
		alert('Housekeep�ɶ��w����');
	}
}
function HousekeepRefDateChanged(hkrefdate) {
	re = /^\d{8}$/
	re_val = ConfigNbaINIForm.HKRefDate.value.search(re)
	if(re_val == -1) {
		alert('�榡���~');
		ConfigNbaINIForm.HKRefDate.value = hkrefdate;
	}
	else {
		alert('Housekeep REF_DATE �w����');
	}
}
function checkedAlert() {
	alert('�ﶵ�w����');
}

function ResetHousekeep() {
	ConfigNbaINIForm.HKT.value = '1500';
}

</script>

<html>
<head>
	<META http-equiv="Content-Type" content="text/html; charset=big5">
	<LINK REL="stylesheet" HREF="/sportStyle.css" TYPE="text/css">
	<title>��|��T - �t�γ]�w</title>
</head>
<body>
	<center>
	<form id="ConfigNbaINIForm" method="post" runat="server">

		<table border="1" width="60%">
		  <tr><h3>�x�y�t�γ]�w</h3></tr>
			<tr>
				<th colspan="2" align="center">
					�t�ήɶ�:&nbsp;<%=m_CurrDateTime%>&nbsp;<a href="ConfigNbaINI.aspx">��s</a>
				</th>
			</tr>
			<span id="ConfigNbaINIInformation" runat="server" />
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