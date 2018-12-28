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
function RushStartChanged(rushstart) {
	re = /^\d{4}$/
	re_val = ConfigINIForm.RushHourStart.value.search(re)
	if(re_val == -1) {
		alert('格式錯誤');
		ConfigINIForm.RushHourStart.value = rushstart;
	}	else {
		alert('繁忙時間的開始已改變');
	}
}

function RushEndChanged(rushend) {
	re = /^\d{4}$/
	re_val = ConfigINIForm.RushHourEnd.value.search(re)
	if(re_val == -1) {
		alert('格式錯誤');
		ConfigINIForm.RushHourEnd.value = rushend;
	}	else {
		alert('繁忙時間的結束已改變');
	}
}

function checkedAlert() {
	alert('選項已改變');
}

function HousekeepChanged(hkt) {
	re = /^\d{4}$/
	re_val = ConfigINIForm.HKT.value.search(re)
	if(re_val == -1) {
		alert('格式錯誤');
		ConfigINIForm.HKT.value = hkt;
	}	else {
		alert('Housekeep時間已改變');
	}
}

function HousekeepRefDateChanged(refdate) {
	re = /^\d{8}$/
	re_val = ConfigINIForm.REFDATE.value.search(re)
	if(re_val == -1) {
		alert('格式錯誤');
		ConfigINIForm.REFDATE.value = refdate;
	}	else {
		alert('Housekeep REF_DATE 已改變');
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
	<title>體育資訊 - 足球系統設定</title>
</head>
<body>
	<center>
	<form id="ConfigINIForm" method="post" runat="server">			
		<table border="1" width="80%">
		  <tr><h3>足球系統設定</h3></tr>
			<tr>
				<th colspan="2" align="center">
					系統時間:&nbsp;<%=m_CurrDateTime%>&nbsp;<a href="ConfigINI.aspx">更新</a>
				</th>
			</tr>
			<span id="ConfigINIInformation" runat="server" />
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