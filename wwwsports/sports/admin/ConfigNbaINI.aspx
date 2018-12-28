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
				rtnMsg.Text = "修改系統設定成功";
			}	else if(iResult == 0) {
				rtnMsg.Text = "系統設定沒有修改";
			}	else if(iResult == -99) {
				rtnMsg.Text = "沒有修改權限";
			}	else {
				rtnMsg.Text = "修改系統設定失敗";
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
		alert('格式錯誤');
		ConfigNbaINIForm.HKT.value = hkt;
	}
	else {
		alert('Housekeep時間已改變');
	}
}
function HousekeepRefDateChanged(hkrefdate) {
	re = /^\d{8}$/
	re_val = ConfigNbaINIForm.HKRefDate.value.search(re)
	if(re_val == -1) {
		alert('格式錯誤');
		ConfigNbaINIForm.HKRefDate.value = hkrefdate;
	}
	else {
		alert('Housekeep REF_DATE 已改變');
	}
}
function checkedAlert() {
	alert('選項已改變');
}

function ResetHousekeep() {
	ConfigNbaINIForm.HKT.value = '1500';
}

</script>

<html>
<head>
	<META http-equiv="Content-Type" content="text/html; charset=big5">
	<LINK REL="stylesheet" HREF="/sportStyle.css" TYPE="text/css">
	<title>體育資訊 - 系統設定</title>
</head>
<body>
	<center>
	<form id="ConfigNbaINIForm" method="post" runat="server">

		<table border="1" width="60%">
		  <tr><h3>籃球系統設定</h3></tr>
			<tr>
				<th colspan="2" align="center">
					系統時間:&nbsp;<%=m_CurrDateTime%>&nbsp;<a href="ConfigNbaINI.aspx">更新</a>
				</th>
			</tr>
			<span id="ConfigNbaINIInformation" runat="server" />
			<tr align="right">
				<td align="left">
					<input type="button" name="resetHKBtn" value="預設Housekeep時間" onClick="ResetHousekeep()">&nbsp;需按<b>儲存</b>確定更新
				</td>
				<td>
					<input type="button" id="SaveBtn" value="儲存" OnServerClick="ModifyINIAction" runat="server">&nbsp;
					<input type="reset" value="重設">
				</td>
			</tr>
		</table>
		<asp:Label id="rtnMsg" runat="server" />
		</form>
	</center>
</body>
</html>